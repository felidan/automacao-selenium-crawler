using Selenium.Utils.Entidades;
using Selenium.Utils.Enum;
using SeleniumCatho.Screens;
using System;
using System.Text;

namespace SeleniumCatho
{
    public class CoreAutomacao
    {
        public Response Processar(SeleniumModel selenium)
        {
            bool retorno = false;
            string mensagem = ValidarDados(selenium);

            if (!String.IsNullOrEmpty(mensagem))
                return new Response(mensagem, LevelLogEnum.Erro);

            switch (selenium.Sistema)
            {
                case SistemaEnum.Catho:
                    CathoScreen catho = new CathoScreen(selenium);
                    retorno = catho.Iniciar();
                    break;
                case SistemaEnum.InfoJobs:
                    InfoJobsScreen infoJobs = new InfoJobsScreen(selenium);
                    retorno = infoJobs.Iniciar();
                    break;
            }

            if(retorno)
                return new Response("Processamento Ok", LevelLogEnum.Info);
            else
                return new Response("Erro no procesamento", LevelLogEnum.Erro);
        }

        private string ValidarDados(SeleniumModel selenium)
        {
            StringBuilder mensagem = new StringBuilder("");

            if (String.IsNullOrEmpty(selenium.Login) || String.IsNullOrEmpty(selenium.Senha))
                mensagem.Append("Credenciais inválidas");

            if (selenium.Cargos.Count == 0)
                mensagem.Append("Preencha com os cargos desejados");

            if(String.IsNullOrEmpty(selenium.DriverPath))
                mensagem.Append("URL do driver não preenchida");

            if (String.IsNullOrEmpty(selenium.NomeUsuario))
                mensagem.Append("Preencha o primeiro nome do usuário");

            if (String.IsNullOrEmpty(selenium.ResumoProfissional))
                mensagem.Append("Preencha o resumo profissional");

            if (selenium.TimeOutAplicacao.TotalSeconds <= 0)
                mensagem.Append("Configure o TimeOut da aplicação");

            if (String.IsNullOrEmpty(selenium.Url))
                mensagem.Append("URL inválida");

            return mensagem.ToString();
        }
    }
}
