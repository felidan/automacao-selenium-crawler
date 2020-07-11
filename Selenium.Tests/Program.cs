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
                Login = "",
                Senha = "",
                NomeUsuario = "",
                Cargos = new List<string>() { "" },
                ResumoProfissional = "",
                OcultarJanela = false,

                Browser = Utils.Enum.BrowserEnum.Chrome,
                DriverPath = @"D:\Projetos\AutomacaoRobo\Selenium.Utils\Drivers\",
                TimeOutAplicacao = TimeSpan.FromSeconds(30),
                Url = "https://seguro.catho.com.br/signin/",
                Sistema = Utils.Enum.SistemaEnum.Catho
            };

            CoreAutomacao core = new CoreAutomacao();

            var response = core.Processar(model);

            Console.ReadKey();
        }
    }
}
