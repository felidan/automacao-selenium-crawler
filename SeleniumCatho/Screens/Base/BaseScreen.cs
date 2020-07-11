using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Utils.Enum;
using Selenium.Utils.Factory;
using System;
using System.IO;
using SeleniumExtras.WaitHelpers;
using Selenium.Utils.Logs;
using Selenium.Utils.Entidades;

namespace SeleniumCatho.Screens.Base
{
    public class BaseScreen
    {
        protected IWebDriver _webDriver;
        public BaseScreen(BrowserEnum browser, string driverPath, bool headless)
        {
            _webDriver = WebDriverFactory.GetDriver(browser, driverPath, headless);
        }

        public bool Iniciar()
        {
            var retorno = false;

            try
            {
                retorno = Executar();
            }
            catch(Exception ex)
            {
                LogService.AdicionarLog(new Response(ex.Message, LevelLogEnum.Erro));
            }
            finally
            {
                Finalizar();
            }

            return retorno;
        }

        public virtual bool Executar()
        {
            return false;
        }

        public void AguardarHtml(TimeSpan timeOut, By by)
        {
            WebDriverWait wait = new WebDriverWait(_webDriver, timeOut);
            wait.Until(ExpectedConditions.ElementIsVisible(by));
        }

        public void PrintTela(string path, string nome)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            ITakesScreenshot takesScreenshot = _webDriver as ITakesScreenshot;
            Screenshot screenshot = takesScreenshot.GetScreenshot();
            screenshot.SaveAsFile($"{path}{nome}", ScreenshotImageFormat.Png);
        }

        public void Finalizar()
        {
            _webDriver.Quit();
            _webDriver = null;
        }
    }
}
