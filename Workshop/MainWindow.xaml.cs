using CsQuery;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Workshop.Models;


namespace Workshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer { get; set; }
        private HttpClient httpClient { get; set; }
        private List<Plan> plans { get; set; }
        private List<string> namesRegistered = new List<string>();

        private IWebDriver driver;

        public MainWindow()
        {
            httpClient = new HttpClient();
            plans = [];
            InitializeComponent();
            timer = InitializeTimer();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
           this.DragMove();
        } 

        private DispatcherTimer InitializeTimer()
        {
            var result = new DispatcherTimer();
            result.Interval = TimeSpan.FromSeconds(1);
            result.Tick += Timer_Tick;
            return result;
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
            workshopLogo.Visibility = System.Windows.Visibility.Collapsed;
            HideAll();
            getCurrentValues();
        }

        public static UIElement CreateScoreboard(Jogo game)
        {

            Border border = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Background = Brushes.White,
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                MaxWidth = 400,
            };

            StackPanel mainStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                MaxWidth = 400,
            };

            TextBlock stadiumTextBlock = new TextBlock
            {
                Text = $"{game.Sede.NomePopular} • {game.DataRealizacao} • {game.HoraRealizacao}",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            mainStackPanel.Children.Add(stadiumTextBlock);

            StackPanel teamsStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Image team1Image = new Image
            {
                Source = new BitmapImage(new Uri($"Resources/Images/times/{game.Equipes.Mandante.Escudo.Split("/").Last().Replace(".svg", ".png")}", UriKind.RelativeOrAbsolute)),
                Width = 40,
                Height = 40,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2),
                Stretch = Stretch.Uniform
            };
            teamsStackPanel.Children.Add(team1Image);
            RenderOptions.SetBitmapScalingMode(team1Image, BitmapScalingMode.HighQuality);


            TextBlock team1NameTextBlock = new TextBlock
            {
                Text = game.Equipes.Mandante.Sigla,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 16,
                Margin = new Thickness(5),
                FontWeight = FontWeights.SemiBold,
            };
            teamsStackPanel.Children.Add(team1NameTextBlock);

            TextBlock scoreTextBlock = new TextBlock
            {
                Text = $"{game.PlacarOficialMandante} x {game.PlacarOficialVisitante}",
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };
            teamsStackPanel.Children.Add(scoreTextBlock);

            TextBlock team2NameTextBlock = new TextBlock
            {
                Text = game.Equipes.Visitante.Sigla,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 16,
                Margin = new Thickness(5),
                FontWeight = FontWeights.SemiBold,
            };
            teamsStackPanel.Children.Add(team2NameTextBlock);

            Image team2Image = new Image
            {
                Source = new BitmapImage(new Uri($"Resources/Images/times/{game.Equipes.Visitante.Escudo.Split("/").Last().Replace(".svg", ".png")}", UriKind.RelativeOrAbsolute)),
                Width = 40,
                Height = 40,
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(2),
            };

            RenderOptions.SetBitmapScalingMode(team2Image, BitmapScalingMode.HighQuality);
            teamsStackPanel.Children.Add(team2Image);

            mainStackPanel.Children.Add(teamsStackPanel);

            var resultTextBlock = new TextBlock
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Green,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 3, 0, 0),
                Cursor = Cursors.Hand,
            };

            var hyperlink = new Hyperlink
            {
                NavigateUri = new Uri(game.Transmissao.Url),
                ToolTip = game.Transmissao.Url,
                Foreground= Brushes.Green,
                OverridesDefaultStyle = true
            };
            hyperlink.Inlines.Add((game.Transmissao.Broadcast.Id == "ENCERRADA") ? "SAIBA COMO FOI" : "FIQUE POR DENTRO");

            hyperlink.RequestNavigate += (sender, args) =>
            {
                try
                {
                    var processInfo = new ProcessStartInfo(args.Uri.OriginalString);

                    Process.Start(processInfo);
                }
                catch (Exception)
                {
                    //
                }  
            };

            resultTextBlock.Inlines.Add(hyperlink);
            mainStackPanel.Children.Add(resultTextBlock);

            border.Child = mainStackPanel;

            return border;
        }
        private void getFootbalResults()
        {
            renderPanel.Orientation = Orientation.Vertical;

            var endpoint = "https://ge.globo.com/futebol/brasileirao-serie-a/";
          
            CQ resultResponse = httpClient.GetAsync(endpoint).Result.Content.ReadAsStringAsync().Result;

            Match match = Regex.Match(resultResponse.RenderSelection(), @"const\s+listaJogos\s*=\s*(\[.*?\])\s*;");

            if (match.Success)
            {
                string locatedString = match.Groups[1].Value;
                List<Jogo> listaJogosValue = JsonSerializer.Deserialize<List<Jogo>>(locatedString)!;

                foreach (Jogo game in listaJogosValue)
                {
                    var scoreBoard = CreateScoreboard(game);
                    renderPanel.Children.Add(scoreBoard);
                   
                }
            }

        }

        private Border CreateCurrencyComponent(string name, string strangeValue, string strangeCoin, string localValue, string localCoin)
        {
     
            Border border = new Border
            {
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#344487")),
                Margin = new Thickness(0, 10, 0, 0),
                MinWidth = 400,
                CornerRadius = new CornerRadius(5),
            };

 
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            StackPanel horizontalPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            var coinText = CreateTextBlock(name, Colors.LightGray, 22, new Thickness(5, 0, 5, 0));
            coinText.HorizontalAlignment = HorizontalAlignment.Center;
            horizontalPanel.Children.Add(coinText);

            stackPanel.Children.Add(CreateTextBlock(strangeCoin, Colors.LightGray, 32));
            stackPanel.Children.Add(CreateTextBlock(strangeValue, Colors.White, 32));
            stackPanel.Children.Add(CreateTextBlock("vale", (Color)ColorConverter.ConvertFromString("#8299fa"), 22, new Thickness(5, 0, 5, 0)));
            stackPanel.Children.Add(CreateTextBlock(localCoin, Colors.LightGray, 32));
            stackPanel.Children.Add(CreateTextBlock(localValue, Colors.White, 32));
            stackPanel.Children.Add(CreateTextBlock("hoje", (Color)ColorConverter.ConvertFromString("#8299fa"), 22, new Thickness(5, 0, 0, 0)));

            horizontalPanel.Children.Add(stackPanel);

            border.Child = horizontalPanel;

            return border;
        }

        private TextBlock CreateTextBlock(string text, Color color, double fontSize, Thickness? margin = null)
        {
            return new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(color),
                FontSize = fontSize,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = margin ?? new Thickness(0)
            };
        }
        private void getCurrentValues()
        {
            renderPanel.Orientation = Orientation.Vertical;

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
                
                CQ resultResponse = httpClient.GetAsync(endpoint.Value).Result.Content.ReadAsStringAsync().Result;

                var name = resultResponse["#moeda > h1"].Text();
                var nationalValue = resultResponse["#nacional"].Val();
                var strangeValue = resultResponse["#estrangeiro"].Val();
                var nationalCoin = resultResponse["span.cotMoeda.nacional > span.symbol"].Text();
                var strangeCoin = resultResponse["span.cotMoeda.estrangeira > span.symbol"].Text();

                renderPanel.Children.Add(CreateCurrencyComponent(name.Split(" ").First(), strangeValue, strangeCoin, nationalValue, nationalCoin));

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void getFootball_Click(object sender, RoutedEventArgs e)
        {
            workshopLogo.Visibility = System.Windows.Visibility.Collapsed;
            HideAll();
            getFootbalResults();
        }

        private void HideAll()
        {
 
            foreach (UIElement element in resultPanel.Children)
            {
                if (element is TextBlock)
                {
                    (element as TextBlock).Visibility = System.Windows.Visibility.Collapsed;
                }      
            }

            renderPanel.Children.Clear();

        }

        private void getAggerProducts()
        {
            renderPanel.Orientation = Orientation.Horizontal;

            if (plans?.Count > 0) plans?.Clear();

            foreach (var name in namesRegistered)
            {
                this.UnregisterName(name);
            }
            namesRegistered.Clear();

            string aggerUrl = "https://serversite.azurewebsites.net/plan";

            var result = httpClient.GetAsync(aggerUrl).Result.Content.ReadAsStringAsync().Result;

            List<Plan> resultObj = JsonSerializer.Deserialize<List<Plan>>(result)!;

           
            foreach (var item in resultObj)
            {
                if ((item.Type != "7") || (item.Licences[0].Price.Text == "0")) continue;
                plans?.Add(item);
                renderPanel.Children.Add(CreateAggerProductCard(item));

            }

        }

        private void buttonAggerProducts_Click(object sender, RoutedEventArgs e)
        {
            workshopLogo.Visibility = System.Windows.Visibility.Collapsed;
            HideAll();
            getAggerProducts();
        }
        public void WaitDriver(double delay, double interval)
        {
            // Causes the WebDriver to wait for at least a fixed delay
            var now = DateTime.Now;
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(delay));
            wait.PollingInterval = TimeSpan.FromMilliseconds(interval);
            wait.Until(wd => (DateTime.Now - now) - TimeSpan.FromMilliseconds(delay) > TimeSpan.Zero);
        }
        public Border CreateAggerProductCard(Plan plan)
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
                Text = plan.Key.ToString(),
                Margin = new Thickness(8, 15, 0, 0),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold
            };

            ComboBox comboBox = new ComboBox
            {
                Name = $"combo{plan.Key.Replace(" ", string.Empty)}",
                Margin = new Thickness(10),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            foreach (var selection in plan.Licences)
            {
                comboBox.Items.Add(selection.Description);
            }
            this.RegisterName($"combo{plan.Key.Replace(" ", string.Empty)}", comboBox);
            namesRegistered.Add($"combo{plan.Key.Replace(" ", string.Empty)}");

            comboBox.SelectedIndex = 0;

            comboBox.SelectionChanged += (s, e) =>
            {
                string? selectedItem = comboBox.SelectedItem.ToString();

                foreach (var planItem in plans)
                {
                    foreach (var item in planItem.Licences)
                    {
                        if (selectedItem == item?.Description.ToString() && (s as ComboBox)!.Name == $"combo{ planItem.Key.Replace(" ", string.Empty)}")
                        {
                            TextBlock textField = (TextBlock)this.FindName($"{planItem.Key.Replace(" ", string.Empty)}Price");
                            if (textField != null)
                            {
                                var stringContent = item?.Price.Text == "0" ? "Sob Consulta" : item?.Price.Text;
                                textField.Text = stringContent;
                                if (item?.Price.Text == "0") textField.FontSize = 26; else textField.FontSize = 40;
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
                Name = $"{plan.Key.Replace(" ", string.Empty)}Price",
                Text = plan.Licences[0]?.Price.Text.ToString(),
                FontSize = 40,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };

            this.RegisterName($"{plan.Key.Replace(" ", string.Empty)}Price", priceTextBlock);
            namesRegistered.Add($"{plan.Key.Replace(" ", string.Empty)}Price");

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

            button.Click += (s, e) =>
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                string chromePath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\runtime\chrome-win\chrome.exe"));

                string chromeDriverPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\runtime\chrome-win"));

                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--start-maximized");

                chromeOptions.BinaryLocation = chromePath;

                var chromeDriverService = ChromeDriverService.CreateDefaultService(chromeDriverPath);
                chromeDriverService.HideCommandPromptWindow = true;

                driver = new ChromeDriver(chromeDriverService, chromeOptions);

                driver.Manage().Window.Maximize();

                driver.Navigate().GoToUrl("https://www.agger.com.br");

                var button = driver.FindElement(By.CssSelector("#navbar > div > div > div.MuiBox-root.css-f1v08x > a:nth-child(4)"));

                button.Click();

                var indexKey = plan.Key.ToLower().Contains("aggilizador") ? 0 : 1;
                var licenseList = driver.FindElements(By.CssSelector("div[class*='MuiSelect-select MuiSelect-outlined MuiOutlinedInput-input MuiInputBase-input']"))[indexKey];

                var combo = (ComboBox)FindName($"combo{plan.Key.Replace(" ", string.Empty)}");

                var option = combo.SelectedIndex;

                Thread.Sleep(3000);

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", licenseList);

                Thread.Sleep(1000);

                licenseList.Click();

                var listUl = driver.FindElement(By.CssSelector("ul[class*='MuiList-root MuiMenu-list']"));

                var li = listUl.FindElements(By.XPath("./*"));

                var selectItem = li[option];

                selectItem.Click();

                if (option > 4) return;

                var contractButton = driver.FindElement(By.Id(plan.Key.ToLower().Contains("aggilizador") ? "contrataraggilizador" : "contratelink"));

                contractButton.Click();
         
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