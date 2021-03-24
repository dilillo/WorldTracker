using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

namespace WorldTrackerDomain.Views
{
    public class DomainViewWrapper
    {
        public DomainViewWrapper()
        {
        }

        public DomainViewWrapper(DomainView view)
        {
            ID = view.ID;
            ViewType = view.GetType().AssemblyQualifiedName;
            ViewData = JObject.FromObject(view);
            ArtificialPartitionKey = GetMd5Hash(view.ID);
        }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("artificialPartitionKey")]
        public string ArtificialPartitionKey { get; set; }

        [JsonProperty("viewType")]
        public string ViewType { get; set; }

        [JsonProperty("viewData")]
        public JObject ViewData { get; set; }

        public DomainView GetView()
        {
            var viewType = Type.GetType(ViewType);

            return (DomainView)ViewData.ToObject(viewType);
        }

        static string GetMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                var ouputStringBuilder = new StringBuilder();

                for (var i = 0; i < hashedBytes.Length; i++)
                {
                    ouputStringBuilder.Append(hashedBytes[i].ToString("x2"));
                }

                return ouputStringBuilder.ToString();
            }
        }
    }
}
