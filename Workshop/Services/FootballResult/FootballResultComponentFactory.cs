using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using Workshop.Models;
using System.Diagnostics;

namespace Workshop.Services.FootballResult
{
    internal class FootballResultComponentFactory
    {
        public FootballResultComponentFactory() { }
        public UIElement CreateScoreboard(FootbalGame game)
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
                Foreground = Brushes.Green,
                OverridesDefaultStyle = true
            };
            hyperlink.Inlines.Add((game.Transmissao.Broadcast.Id == "ENCERRADA") ? "SAIBA COMO FOI" : "FIQUE POR DENTRO");

            hyperlink.RequestNavigate += (sender, args) =>
            {
                try
                {
                    var processInfo = new ProcessStartInfo
                    {
                        FileName = args.Uri.OriginalString,
                        UseShellExecute = true,
                    };

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
    }
}

