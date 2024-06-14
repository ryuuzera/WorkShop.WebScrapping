using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;


namespace Workshop.Services.Selenium
{
    internal class SeleniumService
    {
        private readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
        private string chromePath {  get; set; }
        private string chromeDriverPath { get; set; }

        private ChromeOptions chromeOptions = new ChromeOptions();
        public IWebDriver? Driver { get; set; }

        public SeleniumService() {
            chromePath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\runtime\chrome-win\chrome.exe"));

            chromeDriverPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\runtime\chrome-win"));

            chromeOptions.AddArgument("--start-maximized");

            chromeOptions.BinaryLocation = chromePath;
        }

        public SeleniumService Run()
        {
            var chromeDriverService = ChromeDriverService.CreateDefaultService(chromeDriverPath);

            chromeDriverService.HideCommandPromptWindow = true;

            Driver = new ChromeDriver(chromeDriverService, chromeOptions);

            return this;
        }
    }
}
