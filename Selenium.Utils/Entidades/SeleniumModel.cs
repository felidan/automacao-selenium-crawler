using Selenium.Utils.Enum;
using System;
using System.Collections.Generic;

namespace Selenium.Utils.Entidades
{
    public class SeleniumModel
    {
        public SistemaEnum Sistema { get; set; }
        public TimeSpan TimeOutAplicacao { get; set; }
        public string Url { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public BrowserEnum Browser { get; set; }
        public string DriverPath { get; set; }
        public bool OcultarJanela { get; set; }
        public string NomeUsuario { get; set; }
        public List<string> Cargos { get; set; }
        public string ResumoProfissional { get; set; }
    }
}
