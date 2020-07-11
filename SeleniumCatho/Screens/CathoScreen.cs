using OpenQA.Selenium;
using Selenium.Utils.Entidades;
using Selenium.Utils.Enum;
using Selenium.Utils.Extensions;
using Selenium.Utils.Logs;
using SeleniumCatho.Screens.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumCatho.Screens
{
    public class CathoScreen : BaseScreen
    {
        private readonly SeleniumModel _dados;
        public CathoScreen(SeleniumModel selenium) 
            : base(selenium.Browser, selenium.DriverPath, selenium.OcultarJanela)
        {
            _dados = selenium;
        }

        public override bool Executar()
        {
            bool retorno = false;

            CarregarPagina();

            RealizarLogin();

            if (!LogService.TemErros())
            {
                BuscarVagas();

                if (!LogService.TemErros())
                {
                    EnviarCurriculo();

                    if (!LogService.TemErros())
                    {
                        retorno = true;
                    }
                }
            }
            return retorno;
        }

        private void EnviarCurriculo()
        {
            LogService.AdicionarLog(new Response($"Iniciando envio de vagas", LevelLogEnum.Info));

            string response = String.Empty;
            List<Vaga> vagas = new List<Vaga>();

            var elementosVaga = _webDriver.BuscarElementos(By.ClassName("boxVaga"));

            LogService.AdicionarLog(new Response($"Vagas selecionadas: {elementosVaga.Count}", LevelLogEnum.Info));

            foreach (var vaga in elementosVaga)
            {
                LogService.AdicionarLog(new Response($"Pegando descrição da Vaga", LevelLogEnum.Info));

                var abrirVaga = vaga.FindElement(By.ClassName("tres-pontos"));
                abrirVaga.Click();

                var dadosVaga = RasparInformacoesVaga(vaga);

                var fecharVaga = vaga.FindElement(By.ClassName("closeVagButton"));
                fecharVaga.Click();

                LogService.AdicionarLog(new Response($"Descrição coletada", LevelLogEnum.Info));

                LogService.AdicionarLog(new Response($"Iniciando etapa final", LevelLogEnum.Info));

                var botaoEnviar = vaga.FindElement(By.ClassName("btnEnviarCurriculo"));
                var textoBotao = botaoEnviar.GetAttribute("innerHTML");

                if (textoBotao.Contains("enviar currículo"))
                {
                    botaoEnviar.Click();

                    AguardarHtml(_dados.TimeOutAplicacao, By.ClassName("pergunta"));

                    response = _webDriver.BuscarHtml(By.ClassName("pergunta"), "innerHTML");

                    LogService.AdicionarLog(new Response($"Incluindo resumo profissional", LevelLogEnum.Info));

                    if (response.Contains("Faço um pequeno texto sobre seu perfil pessoal e profissional"))
                    {
                        _webDriver.IncluirValor(By.XPath("//*[@id=\"enviarCurriculo\"]/section[2]/article/ol/li/textarea"), _dados.ResumoProfissional);

                        var botoesModel = _webDriver.BuscarElementos(By.ClassName("containerNavigation")).First();
                        var submeterCurriculo = botoesModel.FindElement(By.ClassName("buttonAzulFlat"));

                        submeterCurriculo.Click();

                        LogService.AdicionarLog(new Response($"Enviando Curriculo", LevelLogEnum.Info));

                        AguardarHtml(_dados.TimeOutAplicacao, By.ClassName("btnEnviarCurriculo"));

                        var text = botaoEnviar.GetAttribute("innerHTML");

                        if (text.Contains("currículo enviado"))
                        {
                            LogService.AdicionarLog(new Response($"Curriculo enviado", LevelLogEnum.Info));
                            vagas.Add(dadosVaga);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else if(textoBotao.Contains("vaga expirada"))
                {
                    LogService.AdicionarLog(new Response($"Vaga Expirada: {dadosVaga.Titulo}", LevelLogEnum.Info));
                }
                else
                {
                    LogService.AdicionarLog(new Response($"Erro ao ler a vaga: {dadosVaga.Titulo}", LevelLogEnum.Erro));
                }
            }
        }

        private Vaga RasparInformacoesVaga(IWebElement vaga)
        {
            var titulo = vaga.FindElements(By.TagName("a")).First().GetAttribute("innerHTML");

            var salario = vaga.FindElement(By.ClassName("salarioLocal")).GetAttribute("innerHTML");

            var totalvagas = vaga.FindElement(By.ClassName("total")).GetAttribute("innerHTML");

            var nomeEmpresa = vaga.FindElement(By.ClassName("nomeEmpresa")).GetAttribute("innerHTML");

            var descricao = vaga.FindElement(By.ClassName("descriptionComplete")).GetAttribute("fulltext");

            var beneficios = vaga.FindElement(By.Id("beneficios-da-vaga"))
                .FindElement(By.TagName("article"))
                .GetAttribute("innerHTML");

            var nacionalidadePorte = vaga.FindElement(By.Id("nacionalidade-e-porte-da-empresa"))
                .FindElement(By.TagName("article"))
                .GetAttribute("innerHTML");

            Vaga dadosVaga = new Vaga()
            {
                Titulo = titulo?.Trim(),
                Salario = salario?.Trim(),
                TotalVagas = totalvagas?.Trim()?.Replace(":", ""),
                NomeEmpresa = nomeEmpresa?.Trim().Replace("\r\n", ""),
                Descricao = descricao?.Trim(),
                Beneficios = beneficios?.Trim().Replace("<p>", "").Replace("</p>", ""),
                NacionalidadePorte = nacionalidadePorte?.Trim().Replace("<p>", "").Replace("</p>", "")
            };

            return dadosVaga;
        }

        private void BuscarVagas()
        {
            string response = String.Empty;
            
            foreach(var cargo in _dados.Cargos)
            {
                LogService.AdicionarLog(new Response($"Iniciando busca de vagas para o cargo {cargo}", LevelLogEnum.Info));

                _webDriver.IncluirValor(By.Id("barraBusca"), cargo);

                LogService.AdicionarLog(new Response("Incluindo filtro para vagas em São Paulo", LevelLogEnum.Info));

                _webDriver.ClicarBotao(By.Id("selectLocalidade"));
                _webDriver.ClicarBotao(By.XPath("//*[@id=\"select_Estado\"]/div/div[1]"));
                _webDriver.ClicarBotao(By.XPath("//*[@id=\"estado_id[]input\"]"));
                _webDriver.ClicarBotao(By.ClassName("btnOkSelectPlugin"));
                _webDriver.ClicarBotao(By.ClassName("btnOkLocalidade"));

                LogService.AdicionarLog(new Response("Filtro finalizado", LevelLogEnum.Info));

                _webDriver.SubmeterBotao(By.ClassName("btnBuscar"));

                AguardarHtml(_dados.TimeOutAplicacao, By.ClassName("infos"));

                response = _webDriver.BuscarHtml(By.ClassName("infos"), "innerHTML");

                if (response.Contains("Total de anúncios"))
                {
                    LogService.AdicionarLog(new Response("Vagas encontradas", LevelLogEnum.Info));

                    if (!LogService.TemErros())
                        EnviarCurriculo();
                    else
                        LogService.AdicionarLog(new Response("Erro não previsto", LevelLogEnum.Erro));
                }
                else
                    LogService.AdicionarLog(new Response("Erro: Vagas não localizadas", LevelLogEnum.Erro));

                _webDriver.Navigate().GoToUrl("https://www.catho.com.br/area-candidato/");
            }
        }

        private void RealizarLogin()
        {
            string response = String.Empty;

            LogService.AdicionarLog(new Response("Iniciando pagina de Login", LevelLogEnum.Info));

            AguardarHtml(_dados.TimeOutAplicacao, By.ClassName("InputLabel-obg30c-0"));
            response = _webDriver.BuscarHtml(By.ClassName("InputLabel-obg30c-0"), "innerHTML");
            
            if (response.Contains("Digite seu e-mail ou"))
            {
                LogService.AdicionarLog(new Response("Pagina de Login iniciado", LevelLogEnum.Info));
                LogService.AdicionarLog(new Response("Inserindo Login e Senha", LevelLogEnum.Info));

                _webDriver.IncluirValor(By.Name("email"), _dados.Login);
                _webDriver.IncluirValor(By.Name("password"), _dados.Senha);
                _webDriver.ClicarBotao(By.ClassName("Button__StyledButton-sc-1ovnfsw-1"));

                LogService.AdicionarLog(new Response("Logando..", LevelLogEnum.Info));
                
                AguardarHtml(_dados.TimeOutAplicacao, By.ClassName("minhaConta"));
                response = _webDriver.BuscarHtml(By.ClassName("minhaConta"), "innerHTML");
                
                if(response.ToUpper().Contains(_dados.NomeUsuario.ToUpper()))
                {
                    LogService.AdicionarLog(new Response("Login efetuado com sucesso", LevelLogEnum.Info));
                }
                else
                {
                    LogService.AdicionarLog(new Response("Login não efetuado", LevelLogEnum.Erro));
                }
            }
            else
            {
                LogService.AdicionarLog(new Response("Erro ao carregar página de Login", LevelLogEnum.Erro));
            }
        }

        private void CarregarPagina()
        {
            _webDriver.CarregarPagina(_dados.TimeOutAplicacao, _dados.Url);
            _webDriver.Manage().Window.Maximize();
        }
    }
}
