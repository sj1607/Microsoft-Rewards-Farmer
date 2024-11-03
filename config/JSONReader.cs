using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft_Rewards_Farmer.config
{
    internal class JSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure jSONStructure = JsonConvert.DeserializeObject<JSONStructure>(json);
                this.token = jSONStructure.token;
                this.prefix = jSONStructure.prefix;
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string token { get; set; }
        public string prefix { get; set; }
    }
}
