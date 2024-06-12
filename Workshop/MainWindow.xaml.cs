using CsQuery;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Workshop.Models;

namespace Workshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private HttpClient _httpClient;
        private JsonArray _plans;
        private List<string> _namesRegistered = new List<string>();

        public MainWindow()
        {
            _httpClient = new HttpClient();
            _plans = new JsonArray();
            InitializeComponent();
            InitializeTimer();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); 
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            timeTextBlock.Text = now.ToString("HH:mm");
            dateTextBlock.Text = now.ToString("MMM, dd", new CultureInfo("en-US"));
            dayTextBlock.Text = now.ToString("dddd", new CultureInfo("en-US"));
        }
        private void getDolar_Click(object sender, RoutedEventArgs e)
        {
            workshopLogo.Visibility = Visibility.Collapsed;
            HideAll();
            getCurrentValues();
        }

        private void getFootbalResults()
        {
            var endpoint = "https://ge.globo.com/futebol/brasileirao-serie-a/";
          
            CQ resultResponse = _httpClient.GetAsync(endpoint).Result.Content.ReadAsStringAsync().Result;

            Match match = Regex.Match(resultResponse.RenderSelection(), @"const\s+listaJogos\s*=\s*(\[.*?\])\s*;");

            if (match.Success)
            {
                string locatedString = match.Groups[1].Value.ToString();
                List<Jogo> listaJogosValue = JsonSerializer.Deserialize<List<Jogo>>(locatedString);

                for (int i = 0; i < 10; i++)
                {
                    TextBlock textField = (TextBlock)this.FindName($"resultado{i+1}");

                    if (textField != null && listaJogosValue?[i].Transmissao.Broadcast.Id == "ENCERRADA")
                    {
                        textField.Text = $"{listaJogosValue?[i].Equipes?.Mandante?.NomePopular} {listaJogosValue?[i].PlacarOficialMandante}" +
                                         $" X " +
                                         $"{listaJogosValue?[i]?.Equipes?.Visitante?.NomePopular} {listaJogosValue?[i].PlacarOficialVisitante}";
                        textField.Visibility = Visibility.Visible;
                    };
                    
                }
            }

        }
        private void getCurrentValues()
        {
            var endpoints = new Dictionary<string, string>
            {
               { "Dolar", "http://dolarhoje.com" },
               { "Euro", "https://dolarhoje.com/euro-hoje/" },
               { "Iene", "https://dolarhoje.com/iene/" },
               { "Yuan", "https://dolarhoje.com/yuan-hoje/" },
               { "Bitcoin", "https://dolarhoje.com/bitcoin-hoje/" },
               { "Peso", "https://dolarhoje.com/peso-argentino/" },
               { "Ouro", "https://dolarhoje.com/ouro-hoje/" }, 
            };

            foreach (var endpoint in endpoints)
            {
                CQ resultResponse = _httpClient.GetAsync(endpoint.Value).Result.Content.ReadAsStringAsync().Result;

                var resultValue = resultResponse["#nacional"].Val();

                TextBlock textField = (TextBlock)this.FindName($"{endpoint.Key.ToLower()}Hoje");

                if (textField != null)
                {
                    textField.Text = $"{endpoint.Key}: R$ {ConvertToBRL(resultValue)}";
                    textField.Visibility = Visibility.Visible;
                };
            }
        }
        private string ConvertToBRL(string valorString)
        {
            if (decimal.TryParse(valorString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorDecimal))
            {
                return valorDecimal.ToString("C", new CultureInfo("pt-BR"));
            }
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void getFootball_Click(object sender, RoutedEventArgs e)
        {
            workshopLogo.Visibility = Visibility.Collapsed;
            HideAll();
            getFootbalResults();
        }

        private void HideAll()
        {
            aggerProducts.Visibility = Visibility.Collapsed;
      
            foreach (UIElement element in resultPanel.Children)
            {
                if (element is TextBlock)
                {
                    (element as TextBlock).Visibility = Visibility.Collapsed;
                }      
            }
        }

        private void getAggerProducts()
        {
            aggerProducts.Visibility = Visibility.Visible;

            if (_plans?.Count > 0) _plans?.Clear();

            aggerProducts.Children.Clear();

            foreach (var name in _namesRegistered)
            {
                this.UnregisterName(name);
            }
            _namesRegistered.Clear();

            string aggerUrl = "https://serversite.azurewebsites.net/plan";

            var result = _httpClient.GetAsync(aggerUrl).Result.Content.ReadAsStringAsync().Result;

            var resultObj = JsonSerializer.Deserialize<List<JsonObject>>(result).ToList();

            foreach (var item in resultObj)
            {
                if ((item["type"]?.ToString() != "7") || (item["licences"]?[0]?["price"]?["text"]?.ToString() == "0")) continue;
                _plans?.Add(item);
                aggerProducts.Children.Add(CreateAggerProductCard(item));

            }

        }

        private void buttonAggerProducts_Click(object sender, RoutedEventArgs e)
        {
            workshopLogo.Visibility = Visibility.Collapsed;
            HideAll();
            getAggerProducts();
        }

        public Border CreateAggerProductCard(JsonObject plan)
        {

            Border card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(10)
            };

            StackPanel stackPanel = new StackPanel
            {
                Width = 200,
                Height = 325,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock titleTextBlock = new TextBlock
            {
                Text = plan["key"]?.ToString(),
                Margin = new Thickness(8, 15, 0, 0),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold
            };

            ComboBox comboBox = new ComboBox
            {
                Name = $"combo{plan["key"]?.ToString().Replace(" ", string.Empty)}",
                Margin = new Thickness(10),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            foreach (var selection in plan["licences"]?.AsArray())
            {
                comboBox.Items.Add(selection["description"].ToString());
            }

            comboBox.SelectedIndex = 0;

            comboBox.SelectionChanged += (s, e) =>
            {
                string? selectedItem = comboBox.SelectedItem.ToString();

                foreach (var planItem in _plans)
                {
                    JsonObject? planObj = JsonSerializer.Deserialize<JsonObject>(planItem.ToString());

                    foreach (var item in planObj["licences"]?.AsArray())
                    {
                        if (selectedItem == item?["description"]?.ToString() && (s as ComboBox).Name == $"combo{ planItem["key"]?.ToString().Replace(" ", string.Empty)}")
                        {
                            TextBlock textField = (TextBlock)this.FindName($"{planItem["key"]?.ToString().Replace(" ", string.Empty)}Price");
                            if (textField != null)
                            {
                                var stringContent = item["price"]?["text"]?.ToString() == "0" ? "Sob Consulta" : item["price"]?["text"]?.ToString();
                                textField.Text = stringContent;
                                if (item["price"]?["text"]?.ToString() == "0") textField.FontSize = 26; else textField.FontSize = 40;
                                break;
                            }
                        }
                    }
                   
                }
                
            };

            TextBlock currencyTextBlock = new TextBlock
            {
                Text = "R$",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };

            TextBlock priceTextBlock = new TextBlock
            {
                Name = $"{plan["key"]?.ToString().Replace(" ", string.Empty)}Price",
                Text = plan["licences"]?[0]?["price"]?["text"]?.ToString(),
                FontSize = 40,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };

            this.RegisterName($"{plan["key"]?.ToString().Replace(" ", string.Empty)}Price", priceTextBlock);
            _namesRegistered.Add($"{plan["key"]?.ToString().Replace(" ", string.Empty)}Price");

            TextBlock perMonthTextBlock = new TextBlock
            {
                Text = "/mês",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };

   
            Button button = new Button
            {
                Content = "CONTRATE",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDD00")),
                Foreground = Brushes.Black,
                Width = 150,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20)
            };

            stackPanel.Children.Add(titleTextBlock);
            stackPanel.Children.Add(comboBox);
            stackPanel.Children.Add(currencyTextBlock);
            stackPanel.Children.Add(priceTextBlock);
            stackPanel.Children.Add(perMonthTextBlock);
            stackPanel.Children.Add(button);

            card.Child = stackPanel;

            return card;
        }
    }
}