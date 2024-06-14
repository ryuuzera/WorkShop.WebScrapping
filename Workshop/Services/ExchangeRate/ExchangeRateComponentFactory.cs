using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using Workshop.Models;
using System.Diagnostics;

namespace Workshop.Services.ExchangeRate
{
    internal class ExchangeRateComponentFactory
    {
        public ExchangeRateComponentFactory() { }

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
        public Border CreateCurrencyComponent(ExchangeRateModel model)
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
            var coinText = CreateTextBlock(model.Name, Colors.LightGray, 22, new Thickness(5, 0, 5, 0));
            coinText.HorizontalAlignment = HorizontalAlignment.Center;
            horizontalPanel.Children.Add(coinText);

            stackPanel.Children.Add(CreateTextBlock(model.StrangeCoin, Colors.LightGray, 32));
            stackPanel.Children.Add(CreateTextBlock(model.StrangeValue, Colors.White, 32));
            stackPanel.Children.Add(CreateTextBlock("vale", (Color)ColorConverter.ConvertFromString("#8299fa"), 22, new Thickness(5, 0, 5, 0)));
            stackPanel.Children.Add(CreateTextBlock(model.LocalCoin, Colors.LightGray, 32));
            stackPanel.Children.Add(CreateTextBlock(model.LocalValue, Colors.White, 32));
            stackPanel.Children.Add(CreateTextBlock("hoje", (Color)ColorConverter.ConvertFromString("#8299fa"), 22, new Thickness(5, 0, 0, 0)));

            horizontalPanel.Children.Add(stackPanel);

            border.Child = horizontalPanel;

            return border;
        }
    }
}
