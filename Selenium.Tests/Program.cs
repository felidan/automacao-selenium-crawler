using Selenium.Utils.Entidades;
using SeleniumCatho;
using System;
using System.Collections.Generic;

namespace Selenium.Tests
{
    class Program
    {
        static void Main(string[] args)
        {

            SeleniumModel model = new SeleniumModel()
            {
                Login = "felipelipe1927@hotmail.com",
                Senha = "armin123",
                NomeUsuario = "felipe",
                Cargos = new List<string>() { "ti" },
                ResumoProfissional = "aaaaaaa",
                OcultarJanela = false,

                Browser = Utils.Enum.BrowserEnum.Chrome,
                DriverPath = @"D:\Projetos\automacao-selenium-crawler\Selenium.Utils\Drivers\",
                TimeOutAplicacao = TimeSpan.FromSeconds(90),
                Url = "https://login.infojobs.com.br/Account/Login",
                Sistema = Utils.Enum.SistemaEnum.InfoJobs
            };

            CoreAutomacao core = new CoreAutomacao();

            var response = core.Processar(model);


            //SeleniumModel model = new SeleniumModel()
            //{
            //    Login = "",
            //    Senha = "",
            //    NomeUsuario = "",
            //    Cargos = new List<string>() { "" },
            //    ResumoProfissional = "",
            //    OcultarJanela = false,

            //    Browser = Utils.Enum.BrowserEnum.Chrome,
            //    DriverPath = @"D:\Projetos\automacao-selenium-crawler\Selenium.Utils\Drivers",
            //    TimeOutAplicacao = TimeSpan.FromSeconds(30),
            //    Url = "https://seguro.catho.com.br/signin/",
            //    Sistema = Utils.Enum.SistemaEnum.Catho
            //};

            //CoreAutomacao core = new CoreAutomacao();

            //var response = core.Processar(model);

            Console.ReadKey();
        }
    }
}
