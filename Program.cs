/*
 * Author: Littof
 * Date: 11/03/2024
*/
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft_Rewards_Farmer.config;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;

namespace Microsoft_Rewards_Farmer
{
    internal sealed class Program
    {
        
        private static DiscordClient discordClient { get; set; }

        private static SlashCommandsExtension slashCommands { get; set; }

        static async Task Main(string[] args)
        {
            //Get the details of your config.json file by deserialising it
            var configJsonFile = new JSONReader();
            await configJsonFile.ReadJSON();

            //Setting up the Bot Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJsonFile.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            discordClient = new DiscordClient(discordConfig);

            discordClient.Ready += Client_Ready;

            //Initialize SlashCommands
            slashCommands = discordClient.UseSlashCommands();

            //Register the slash commands in the assembly
            slashCommands.RegisterCommands<SlashCommands>();


            await discordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            Console.WriteLine("Bot is ready!");

            //. Define the activity and playing status
            var activity = new DiscordActivity("Microsoft Rewards Farmer" +"", ActivityType.Playing);

            //. Update the bot's status and activity
            await sender.UpdateStatusAsync(activity, UserStatus.Online);

        }

        //Define your SlashCommands class
        public class SlashCommands : ApplicationCommandModule
        {
            //Ping Command
            [SlashCommand("ping", "Replies with Pong!")]
            public async Task PingCommand(InteractionContext ctx)
            {
                await ctx.CreateResponseAsync("Pong!");
            }

            //Hello Command
            [SlashCommand("hello", "Says hello to the user")]
            public async Task HelloCommand(InteractionContext ctx)
            {
                await ctx.CreateResponseAsync($"Hello, {ctx.User.Username}!");

            }

            //Farm Command

            [SlashCommand("farm", "Farm your Microsoft Rewards automatically")]
            public async Task BingFarmerCommand(InteractionContext ctx, [Option("email", "Your Microsoft email")] string email, [Option("password", "Your Microsoft password")] string password)
            {
                await ctx.CreateResponseAsync("Microsoft Rewards farming starts...");

                bool operationSuccessful = false;
                int pointsWin = 0;
                int pointsBefore = 0;
                int pointsAfter = 0;

                var options = new EdgeOptions();

                /**
                 * 
                 * put in comment if you don't want to see the Edge app : options.AddArgument("--headless"); 
                 * 
                 **/

                options.AddArgument("--headless");

                //Array of random queries
                var queries = new[] { "how to cook pasta", "best travel destinations 2024", "latest tech news", "what is the weather in New York", "famous landmarks in Europe", "how to invest in stocks", "top movies of 2024", "best programming languages to learn", "how to meditate for beginners", "best workout routines", "top 10 video games of all time", "how to grow vegetables at home", "world history timeline", "how to start a small business", "famous quotes by Albert Einstein", "how to improve sleep quality", "best free online courses", "latest news in science", "how to bake a chocolate cake", "what is cryptocurrency", "space exploration missions", "how to make sushi at home", "famous painters in history", "most popular books in 2024", "how to learn Spanish fast", "what are NFTs", "how to train for a marathon", "top fashion trends 2024", "how to reduce carbon footprint", "how to create a website", "famous composers of classical music", "how to play guitar chords", "world population growth", "how to travel on a budget", "history of the internet", "best smartphones in 2024", "how to write a resume", "how to increase productivity", "what is artificial intelligence", "most visited cities in the world", "how to plan a road trip", "how to improve memory", "what is renewable energy", "how to build a gaming PC", "what are the symptoms of the flu", "famous architects in history", "top music festivals around the world", "how to manage stress", "how to use Excel formulas", "most popular programming languages", "best places to visit in Japan", "how to fix a flat tire", "top 10 anime series of all time", "how to make homemade pizza", "famous battles in world history", "what is the meaning of life", "how to set up a home office", "what is quantum computing", "how to start a podcast", "most popular board games", "how to repair a leaking faucet", "best outdoor activities for families", "how to improve public speaking skills", "popular diet plans in 2024", "how to create a budget plan", "top environmental issues in the world", "how to start a YouTube channel", "best practices for remote work", "how to write a business plan", "top 10 travel hacks", "how to build a personal brand" };

                try
                {
                    using (IWebDriver webDriver = new EdgeDriver(options))
                    {
                        webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                        webDriver.Navigate().GoToUrl("https://login.live.com/");

                        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));

                        //Connecting to Microsoft website.
                        var emailField = wait.Until(driver => driver.FindElement(By.Name("loginfmt")));
                        emailField.SendKeys(email);
                        webDriver.FindElement(By.Id("idSIButton9")).Click();
                        await Task.Delay(1000);

                        var passwordField = wait.Until(driver => driver.FindElement(By.Name("passwd")));
                        passwordField.SendKeys(password);
                        webDriver.FindElement(By.Id("idSIButton9")).Click();
                        await Task.Delay(2000);

                        //Retrieve points before searches.
                        webDriver.Navigate().GoToUrl("https://rewards.bing.com/");
                        await Task.Delay(2000);
                        var pointsBeforeElement = wait.Until(driver => driver.FindElement(By.CssSelector("mee-rewards-counter-animation span[mee-element-ready]")));
                        pointsBefore = int.Parse(pointsBeforeElement.Text);

                        //Perform random searches
                        var random = new Random();
                        //For example, 10 searches.
                        for (int i = 0; i < 10; i++) 
                        {
                            var query = queries[random.Next(queries.Length)];

                            //Enter search in Bing.
                            webDriver.Navigate().GoToUrl("https://bing.com");
                            var searchBox = wait.Until(driver => driver.FindElement(By.Id("sb_form_q")));
                            searchBox.SendKeys(query);
                            searchBox.Submit();
                            await Task.Delay(random.Next(3000, 5000));  //Wait between 3 and 5 seconds.


                        }

                        //Recover points after research
                        webDriver.Navigate().GoToUrl("https://rewards.bing.com/");
                        await Task.Delay(2000);
                        var pointsAfterElement = wait.Until(driver => driver.FindElement(By.CssSelector("mee-rewards-counter-animation span[mee-element-ready]")));
                        pointsAfter = int.Parse(pointsAfterElement.Text);

                        pointsWin = pointsAfter - pointsBefore;
                        operationSuccessful = pointsWin > 0;
                    }
                }
                catch (Exception ex)
                {
                    await ctx.CreateResponseAsync($"An error has occurred : {ex.Message}");
                    return;
                }

                //Creating Embed.
                var embed = new DiscordEmbedBuilder()
                {
                    Title = operationSuccessful ? "Farming completed successfully !" : "Farming failed",
                    Description = operationSuccessful
                        ? $"{ctx.User.Mention} you earned {pointsWin} Microsoft Rewards points! (Before: {pointsBefore}, After: {pointsAfter})"
                        : $"{ctx.User.Mention} Farming attempt failed. Please try again..",
                    Color = operationSuccessful ? DiscordColor.Green : DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "Microsoft Rewards Farmer",
                        IconUrl = ctx.Client.CurrentUser.AvatarUrl
                    }
                };

                await ctx.Channel.SendMessageAsync(embed: embed);
            }

        }
    }
}
