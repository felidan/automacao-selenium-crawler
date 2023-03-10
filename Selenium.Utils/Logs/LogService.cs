using Selenium.Utils.Entidades;
using Selenium.Utils.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Selenium.Utils.Logs
{
    public static class LogService
    {
        private static List<Response> Logs = new List<Response>();
        private static string Url = @"D:\Projetos\automacao-selenium-crawler\log.txt";
        private static bool GerarLogFile = false;

        public static void GerarLogArquivoFisico(bool flag)
        {
            GerarLogFile = flag;
        }

        public static List<Response> GetLogs()
        {
            return Logs;
        }

        public static List<Response> GetLogs(LevelLogEnum level)
        {
            return Logs.Where(x => x.Level == level).ToList();
        }

        public static void LimparLogs()
        {
            Logs.Clear();
        }

        public static void AdicionarLog(Response res)
        {
            Logs.Add(res);
            if (GerarLogFile)
            {
                GerarArquivoLog(res, true);
            }
        }

        public static bool TemErros()
        {
            return Logs.Any(x => x.Level == LevelLogEnum.Erro);
        }

        private static void GerarArquivoLog(Response response, bool append)
        {
            using(StreamWriter file = new StreamWriter(Url, append))
            {
                var dataHora = DateTime.Now.ToShortDateString() + DateTime.Now.ToLongTimeString();
                var erro = response.Erro == null ? "" : $" | Erro: {response.Erro.StackTrace.ToString()}";
                file.WriteLine($"[{dataHora}] {response.Level.ToString()} => Mensagem: {response.Mensagem}{erro}");
            }
        }
    }
}
