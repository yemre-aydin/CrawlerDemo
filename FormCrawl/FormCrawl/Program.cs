using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace FormCrawl
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string loginUrl = "https://ayko.paflima.com/index_Action.asp";
            string loginParams = "sDo=Login&USER=M009756&SIFRE&687919";
            WebRequest loginRequest = WebRequest.Create(loginUrl);
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            loginRequest.Method = "POST";
            byte[] loginParamBytes = Encoding.ASCII.GetBytes(loginParams);
            loginRequest.ContentLength = loginParamBytes.Length;
            using (Stream os = loginRequest.GetRequestStream())
            {
                os.Write(loginParamBytes, 0, loginParamBytes.Length);
            }
            WebResponse loginResponse = loginRequest.GetResponse();
            string cookieHeader = loginResponse.Headers["Set-cookie"];

            string searchUrl = "https://ayko.paflima.com/B2b_Stoklar_Ajax.asp";
            string searchParams = "Kategori=Lastik&Name=195%2F65R15&Marka=&Mevsim=&Stok=checked";
            WebRequest searchRequest = WebRequest.Create(searchUrl);
            searchRequest.ContentType = "application/x-www-form-urlencoded";
            searchRequest.Method = "POST";
            searchRequest.Headers.Add("Cookie", cookieHeader);
            byte[] searchParamBytes = Encoding.ASCII.GetBytes(searchParams);
            searchRequest.ContentLength = searchParamBytes.Length;
            using (Stream os = searchRequest.GetRequestStream())
            {
                os.Write(searchParamBytes, 0, searchParamBytes.Length);
            }
            WebResponse searchResponse = searchRequest.GetResponse();
            using (StreamReader sr = new StreamReader(searchResponse.GetResponseStream()))
            {
                var searchResponseData = sr.ReadToEnd();

                
                var searchResult = JsonSerializer.Deserialize<CrawlData>(searchResponseData);

                foreach (var data in searchResult.data)
                {
                    var urun = new Urun
                    {
                        Marka = data[0],
                        UrunKodu = data[1],
                        UrunAdi = data[2],
                        Dot = data[3],
                        ListeFiyati = data[7],
                        BayiFiyati = data[8].Replace("<B>", "").Replace("</B>", "")
                    };

                    // Veritabanına ekleme kodu buraya
                }
            }
        }

        public class CrawlData
        {
            public List<string[]> data { get; set; }
        }

        public class Urun
        {
            public string Marka { get; set; }
            public string UrunKodu { get; set; }
            public string UrunAdi { get; set; }
            public string Dot { get; set; }
            public string ListeFiyati { get; set; }
            public string BayiFiyati { get; set; }
        }
    }
}