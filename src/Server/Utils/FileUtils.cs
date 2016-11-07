using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Utils
{
    public class FileUtils
    {
        public static Dictionary<string, int> GetAfinnJsonFile(string pathToFile)
        {
            using (var reader = System.IO.File.OpenText(pathToFile))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            }
        }
    }
}