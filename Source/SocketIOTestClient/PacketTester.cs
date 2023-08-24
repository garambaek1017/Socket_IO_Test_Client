using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZD_PacketTester
{
    public class PacketTester
    {
        public string Url = "";
        public HttpClient HttpClient = new HttpClient();

        public async Task Send()
        {
            using (var response = await HttpClient.GetAsync(Url))
            {
                if(response.StatusCode == HttpStatusCode.OK)
                {

                }
            }

        }
    }
}
