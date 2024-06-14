using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.Services.Default;
using Workshop.Models;
using CsQuery;
using System.Windows;
using System.Windows.Controls;

namespace Workshop.Services.ExchangeRate
{
    internal class ExchangeRateScrapper: ScrapperDefault
    {
        private readonly ExchangeRateComponentFactory componentFactory;

        private List<Uri> Links = new List<Uri>
        {
           {  new Uri("http://dolarhoje.com") },
           { new Uri("https://dolarhoje.com/euro-hoje/") },
           {  new Uri("https://dolarhoje.com/iene/") },
           { new Uri("https://dolarhoje.com/yuan-hoje/") },
           {  new Uri("https://dolarhoje.com/bitcoin-hoje/") },
           { new Uri("https://dolarhoje.com/peso-argentino/") },
           {  new Uri("https://dolarhoje.com/ouro-hoje/") },
        };
        public ExchangeRateScrapper() {
            componentFactory = new ExchangeRateComponentFactory();
        }

        public List<UIElement> getExchangeRates()
        {
            var RatesPanel = new List<UIElement>();

            foreach (var link in Links)
            {
                try
                {
                    CQ resultResponse = httpClient.GetAsync(link).Result.Content.ReadAsStringAsync().Result;

                    var exchangeRate = new ExchangeRateModel();

                    exchangeRate.Name = resultResponse["#moeda > h1"].Text().Split(" ").First();
                    exchangeRate.LocalValue = resultResponse["#nacional"].Val();
                    exchangeRate.StrangeValue = resultResponse["#estrangeiro"].Val();
                    exchangeRate.LocalCoin = resultResponse["span.cotMoeda.nacional > span.symbol"].Text();
                    exchangeRate.StrangeCoin = resultResponse["span.cotMoeda.estrangeira > span.symbol"].Text();

                    RatesPanel.Add(componentFactory.CreateCurrencyComponent(exchangeRate));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error on Exchange Rates search: {ex.Message}");
                }
            }

            return RatesPanel;
        }



    }
}
