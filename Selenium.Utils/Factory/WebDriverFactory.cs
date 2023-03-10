using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.Utils.Entidades;
using Selenium.Utils.Enum;
using Selenium.Utils.Logs;
using System;
using System.IO;

namespace Selenium.Utils.Factory
{
    public static class WebDriverFactory
    {
        public static IWebDriver GetDriver(BrowserEnum browser, string caminho, bool headless)
        {
            IWebDriver webDriver = null;

            var versao = VersaoDriverChrome();

            if (string.IsNullOrEmpty(versao))
                versao = "83";
            
            switch (browser)
            {
                case BrowserEnum.Chrome:
                    ChromeOptions chromeOptions = new ChromeOptions();
                    if (headless)
                    {
                        chromeOptions.AddArgument("--headless");
                    }
                    
                    chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
                    //chromeOptions.AddArgument("--use-fake-ui-for-media-stream");

                    webDriver = new ChromeDriver($"{caminho}{versao}", chromeOptions);
                    break;
                // TODO - Implementar outros drivers
            }

            return webDriver;
        }

        private static string VersaoDriverChrome()
        {
            string pathChrome = @"C:\Program Files (x86)\Google\Chrome\Application\";
            string versao = String.Empty;

            if (Directory.Exists(pathChrome))
            {
                var dirs = Directory.GetDirectories(pathChrome);

                foreach(var x in dirs)
                {
                    var folder = x.Replace(pathChrome, "");
                    if((int)folder[0] >= 49 && (int)folder[0] <= 57)
                    {
                        versao = folder.Split('.')[0];
                        break;
                    }
                }
            }

            if(String.IsNullOrEmpty(versao))
                LogService.AdicionarLog(new Response($"Utilizando Driver padrão", LevelLogEnum.Info));
            else
                LogService.AdicionarLog(new Response($"Versão {versao} do Driver localizada", LevelLogEnum.Info));

            return versao;
        }
    }
}
