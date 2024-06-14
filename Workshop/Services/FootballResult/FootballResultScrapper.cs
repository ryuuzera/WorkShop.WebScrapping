using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using Workshop.Models;
using Workshop.Services.Default;

namespace Workshop.Services.FootballResult
{
    internal class FootballResultScrapper: ScrapperDefault
    {
        private readonly FootballResultComponentFactory componentFactory;
        public FootballResultScrapper(): base() {
            componentFactory = new FootballResultComponentFactory();
        }

        private readonly List<Uri> Links = new List<Uri>
        {
           new Uri("https://ge.globo.com/futebol/brasileirao-serie-a/") 
        };
        public void getResults(Panel targetPanel)
        {
            CQ resultResponse = httpClient.GetAsync(Links[0]).Result.Content.ReadAsStringAsync().Result;

            Match match = Regex.Match(resultResponse.RenderSelection(), @"const\s+listaJogos\s*=\s*(\[.*?\])\s*;");

            if (match.Success)
            {
                string locatedString = match.Groups[1].Value;
                List<FootbalGame> listaJogosValue = JsonSerializer.Deserialize<List<FootbalGame>>(locatedString)!;

                foreach (FootbalGame game in listaJogosValue)
                {
                    var scoreBoard = componentFactory.CreateScoreboard(game);
                    targetPanel.Children.Add(scoreBoard);
                }
            }
        }
    }
}
