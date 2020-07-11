using Selenium.Utils.Entidades;
using Selenium.Utils.Enum;
using System.Collections.Generic;
using System.Linq;

namespace Selenium.Utils.Logs
{
    public static class LogService
    {
        private static List<Response> Logs = new List<Response>();
        
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
        }

        public static bool TemErros()
        {
            return Logs.Any(x => x.Level == LevelLogEnum.Erro);
        }
    }
}
