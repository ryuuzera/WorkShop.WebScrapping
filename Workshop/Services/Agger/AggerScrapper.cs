using OpenQA.Selenium;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Workshop.Models;
using Workshop.Services.Default;
using Workshop.Services.Selenium;

namespace Workshop.Services.Agger
{
    internal class AggerScrapper : ScrapperDefault
    {
        private readonly AggerComponentFactory componentFactory;
        private List<Plan> plans;

        private List<Uri> Links = new List<Uri>
        {
            new Uri("https://www.agger.com.br"),
            new Uri("https://serversite.azurewebsites.net/plan")
        };

        public AggerScrapper() : base()
        {
            componentFactory = new AggerComponentFactory();
            plans = [];
        }
        public List<UIElement> getAggerPlans(Window sender, List<string> namesRegistered)
        {
            List<UIElement> uIElements = new List<UIElement>();

            var result = httpClient.GetAsync(Links[1]).Result.Content.ReadAsStringAsync().Result;

            List<Plan> resultObj = JsonSerializer.Deserialize<List<Plan>>(result)!;

            foreach (var item in resultObj)
            {
                if ((item.Type != "7") || (item.Licences[0].Price.Text == "0")) continue;
                plans?.Add(item);

                uIElements.Add(componentFactory.CreateAggerProductCard(sender, namesRegistered, plans, item));
            }

            return uIElements;
        }

        public void goToPlanAgger(FrameworkElement sender, Plan plan)
        {

            var driver = new SeleniumService().Run().Driver;

            try
            {
                driver.Manage().Window.Maximize();

                driver.Navigate().GoToUrl(Links[0]);

                var button = driver.FindElement(By.CssSelector("#navbar > div > div > div.MuiBox-root.css-f1v08x > a:nth-child(4)"));

                button.Click();

                var indexKey = plan.Key.ToLower().Contains("aggilizador") ? 0 : 1;
                var licenseList = driver.FindElements(By.CssSelector("div[class*='MuiSelect-select MuiSelect-outlined MuiOutlinedInput-input MuiInputBase-input']"))[indexKey];

                var combo = sender.FindName($"combo{plan.Key.Replace(" ", string.Empty)}") as ComboBox;

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

            }
            catch (Exception)
            {
                driver.Quit();
            }

        }
    }
}
