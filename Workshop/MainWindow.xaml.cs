using OpenQA.Selenium;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Workshop.Services.Agger;
using Workshop.Services.ExchangeRate;
using Workshop.Services.FootballResult;


namespace Workshop
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer { get; set; } = new DispatcherTimer();

        private List<string> namesRegistered = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void InitializeTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                DateTime now = DateTime.Now;
                timeTextBlock.Text = now.ToString("HH:mm");
                dateTextBlock.Text = now.ToString("MMM, dd", new CultureInfo("en-US"));
                dayTextBlock.Text = now.ToString("dddd", new CultureInfo("en-US"));
            };
            timer.Start();
        }
        private void getDolar_Click(object sender, RoutedEventArgs e)
        {
            workshopLogo.Visibility = Visibility.Collapsed;

            HideAll();

            getCurrentValues();
        }
        private void getFootbalResults()
        {
            renderPanel.Orientation = Orientation.Vertical;

            new FootballResultScrapper().getResults(renderPanel);
        }

        private void getCurrentValues()
        {
            renderPanel.Orientation = Orientation.Vertical;

            List<UIElement> exchangeRates = new ExchangeRateScrapper().getExchangeRates();

            exchangeRates.ForEach(panel => renderPanel.Children.Add(panel));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void getFootball_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            getFootbalResults();
        }

        private void HideAll()
        {
            workshopLogo.Visibility = Visibility.Collapsed;
            renderPanel.Children.Clear();
        }

        private void getAggerProducts()
        {
            renderPanel.Orientation = Orientation.Horizontal;

            namesRegistered.ForEach(name => this.UnregisterName(name));
            namesRegistered.Clear();

            List<UIElement> aggerPlans = new AggerScrapper().getAggerPlans(this, namesRegistered);

            aggerPlans.ForEach(panel => renderPanel.Children.Add(panel));

        }

        private void buttonAggerProducts_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            getAggerProducts();
        }

    }
}