using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BitDesktop
{
    public static class BtcDataFactory
    {
        private static WebClient _client;

        public async static Task<int> GetBtcPrice(string provider)
        {
            _client = new WebClient();

            switch(provider)
            {
                case ("BTC-E"):
                    return await BtcPriceBTCE();
                case ("Bitbay"):
                    return await BtcPriceBitBay();
                default:
                    throw new Exception("Invalid BTC provider.");
            }
        }

        private async static Task<int> BtcPriceBTCE()
        {
            var result = await _client.DownloadStringTaskAsync(new Uri("https://btc-e.com/api/3/ticker/btc_usd"));
            dynamic data = JsonConvert.DeserializeObject(result);
            return (int)data.btc_usd.avg;
        }

        private async static Task<int> BtcPriceBitBay()
        {
            var result = await _client.DownloadStringTaskAsync(new Uri("https://bitbay.net/API/Public/BTC/ticker.json"));
            dynamic data = JsonConvert.DeserializeObject(result);
            return (int)data.average;
        }
    }
}
