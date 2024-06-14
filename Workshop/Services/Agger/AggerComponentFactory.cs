using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Workshop.Models;
using Workshop.Services.Selenium;

namespace Workshop.Services.Agger
{
    internal class AggerComponentFactory
    {
        public AggerComponentFactory() { }

        public Border CreateAggerProductCard(Window sender, List<string> namesRegistered, List<Plan> plans, Plan plan)
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
            sender.RegisterName($"combo{plan.Key.Replace(" ", string.Empty)}", comboBox);
            namesRegistered.Add($"combo{plan.Key.Replace(" ", string.Empty)}");

            comboBox.SelectedIndex = 0;

            comboBox.SelectionChanged += (s, e) =>
            {
                string? selectedItem = comboBox.SelectedItem.ToString();

                foreach (var planItem in plans)
                {
                    foreach (var item in planItem.Licences)
                    {
                        if (selectedItem == item?.Description.ToString() && (s as ComboBox)!.Name == $"combo{planItem.Key.Replace(" ", string.Empty)}")
                        {
                            TextBlock textField = (TextBlock)sender.FindName($"{planItem.Key.Replace(" ", string.Empty)}Price");
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

            sender.RegisterName($"{plan.Key.Replace(" ", string.Empty)}Price", priceTextBlock);
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
                try
                {
                    new AggerScrapper().goToPlanAgger(sender, plan);

                }catch(Exception ex)
                {
                    MessageBox.Show($"Error Scrapping Agger: {ex.Message}");
                };
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
