using Selenium.Utils.Enum;
using System;

namespace Selenium.Utils.Entidades
{
    public class Response
    {
        public Response(string msg, LevelLogEnum level, Exception erro = null)
        {
            Ok = level == LevelLogEnum.Info;
            Mensagem = msg;
            Erro = erro;
            Level = level;
        }

        public bool Ok { get; set; }
        public string Mensagem { get; set; }
        public Exception Erro { get; set; }
        public LevelLogEnum Level { get; set; }
    }
}
