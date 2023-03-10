using OpenQA.Selenium;
using Selenium.Utils.Entidades;
using Selenium.Utils.Enum;
using Selenium.Utils.Extensions;
using Selenium.Utils.Logs;
using SeleniumCatho.Screens.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SeleniumCatho.Screens
{
    public class InfoJobsScreen : BaseScreen
    {
        private readonly SeleniumModel _dados;
        private List<Vaga> Vagas = new List<Vaga>();

        public InfoJobsScreen(SeleniumModel selenium) : base(selenium.Browser, selenium.DriverPath, selenium.OcultarJanela)
        {
            _dados = selenium;
        }

        public override bool Executar()
        {
            LogService.GerarLogArquivoFisico(true);
            bool retorno = false;

            CarregarPagina();

            RealizarLogin();

            if (!LogService.TemErros())
            {
                //BuscarVagas();
                NavegarParaVagas();
            }

            return retorno;
        }

        private void NavegarParaVagas()
        {
            LogService.AdicionarLog(new Response($"Redirecionando para a pagina de vagas", LevelLogEnum.Info));

            _webDriver.Navigate().GoToUrl("https://www.infojobs.com.br/empregos-em-sao-paulo,-sp.aspx?Poblacion=5211323,5211294,5211282,5211296");

            foreach (var cargo in _dados.Cargos)
            {
                LogService.AdicionarLog(new Response($"Iniciando busca de vagas para o cargo {cargo}", LevelLogEnum.Info));

                var valor = _webDriver.BuscarHtml(By.ClassName("js_xiticounter"), "innerText");

                _webDriver.ClicarBotao(By.Id("ctl00_phMasterPage_cFacetKeyword_txtKeyword"));
                _webDriver.IncluirValor(By.Id("ctl00_phMasterPage_cFacetKeyword_txtKeyword"), cargo);
                _webDriver.PressionarTecla(By.Id("ctl00_phMasterPage_cFacetKeyword_txtKeyword"), Keys.Enter);

                LogService.AdicionarLog(new Response($"Busca finalizada para vagas do cargo {cargo}", LevelLogEnum.Info));

                if (_webDriver.AguardarMudancaHtml(_dados.TimeOutAplicacao, By.ClassName("js_xiticounter"), valor))
                {
                    var primeiraVaga = _webDriver.BuscarElementos(By.ClassName("element-vaga")).First();
                    var detalhe = primeiraVaga.FindElement(By.ClassName("vaga"));
                    var link = detalhe.FindElement(By.TagName("a")).GetProperty("href");

                    _webDriver.Navigate().GoToUrl(link);
                    
                    // validar se carregado

                    // candidatar

                    // validar limite

                    // proximo

                    int pagina = 1;
                    
                    while(Vagas.Count < 15)
                    {
                        LogService.AdicionarLog(new Response($"Coletando vagas para o cargo {cargo} - Pagina {pagina}", LevelLogEnum.Info));
                        
                        List<string> idsVagasLimitadas = new List<string>();

                        var vagas = _webDriver.BuscarElementos(By.ClassName("element-vaga"));
                        var vagasLimitadas = _webDriver.BuscarElementos(By.ClassName("premiumLimited"));

                        vagasLimitadas.ToList().ForEach(x => idsVagasLimitadas.Add(x.GetProperty("id")));

                        foreach (var vaga in vagas)
                        {
                            var id = vaga.GetProperty("id");

                            if (id.All(char.IsNumber) && !idsVagasLimitadas.Contains(id))
                            {
                                var detalheVaga = vaga.FindElement(By.ClassName("vaga"));

                                var vagaItem = new Vaga();

                                vagaItem.Titulo = detalheVaga.GetAttribute("innerText");
                                vagaItem.Link = detalheVaga.FindElement(By.TagName("a")).GetProperty("href");

                                Vagas.Add(vagaItem);
                            }
                        }

                        pagina++;

                        LogService.AdicionarLog(new Response($"Avançando para a Pagina {pagina}", LevelLogEnum.Info));
                        
                        _webDriver.ClicarBotao(By.Id("ctl00_phMasterPage_cGrid_Paginator1_lnkNext"));

                        _webDriver.AguardarMudancaHtml(_dados.TimeOutAplicacao, By.TagName("header"), $"Página {pagina}", true);

                        LogService.AdicionarLog(new Response($"Pagina {pagina} carregada", LevelLogEnum.Info));
                    }


                    var a = 10;
                    // iniciar coleta de vagas
                }
                else
                {
                    LogService.AdicionarLog(new Response($"Tempo limite exedido para a busca do cargo {cargo}", LevelLogEnum.Erro));
                }
            }
        }

        // OLD

        private void BuscarVagas()
        {
            string response = String.Empty;

            foreach (var cargo in _dados.Cargos)
            {
                LogService.AdicionarLog(new Response($"Iniciando busca de vagas para o cargo {cargo}", LevelLogEnum.Info));

                _webDriver.IncluirValor(By.Name("Palabra"), cargo);

                LogService.AdicionarLog(new Response("Incluindo filtro para vagas em São Paulo", LevelLogEnum.Info));

                _webDriver.IncluirValor(By.ClassName("ui-state-default"), "São Paulo");

                LogService.AdicionarLog(new Response("Filtro finalizado", LevelLogEnum.Info));

                _webDriver.SubmeterBotao(By.ClassName("jsBtnSearch"));

                AguardarHtml(_dados.TimeOutAplicacao, By.ClassName("matchVacancies"));

                response = _webDriver.BuscarHtml(By.ClassName("matchVacancies"), "innerText");

                if (response.Contains("Vagas que podem ser de seu interesse"))
                {
                    LogService.AdicionarLog(new Response("Vagas encontradas", LevelLogEnum.Info));

                    if (!LogService.TemErros())
                        PegarVagas();
                    else
                        LogService.AdicionarLog(new Response("Erro não previsto", LevelLogEnum.Erro));
                }
                else
                    LogService.AdicionarLog(new Response("Erro: Vagas não localizadas", LevelLogEnum.Erro));

                _webDriver.Navigate().GoToUrl("https://www.infojobs.com.br/Candidate/");
            }
        }

        private void PegarVagas()
        {
            List<Vaga> listaVagas = new List<Vaga>();

            var primeiraVaga = _webDriver.BuscarElementos(By.ClassName("js_rowVacancy")).First();
            var vagaTag = primeiraVaga.FindElements(By.TagName("li")).First();
            vagaTag.Click();

            LogService.AdicionarLog(new Response("clicando na primeira vaga", LevelLogEnum.Info));
            while (listaVagas.Count < 5)
            {
                LogService.AdicionarLog(new Response("Aguardandp ahtml", LevelLogEnum.Info));
                AguardarHtml(_dados.TimeOutAplicacao, By.ClassName("advisor-vacancy-content"));

                LogService.AdicionarLog(new Response("fim do aguardando", LevelLogEnum.Info));
                LogService.AdicionarLog(new Response("validando carregamento descrição", LevelLogEnum.Info));
                var response = _webDriver.BuscarElementos(By.ClassName("advisor-vacancy-content")).First();
                LogService.AdicionarLog(new Response("carregamento descrição ok", LevelLogEnum.Info));

                if (response.Text.Contains("Descrição"))
                {
                    LogService.AdicionarLog(new Response("pegando o botão", LevelLogEnum.Info));
                    var botao = _webDriver.BuscarElementos(By.Id("ctl00_phMasterPage_cHeader_lnkCandidatar")).First();
                    LogService.AdicionarLog(new Response("pegando o botão ok", LevelLogEnum.Info));

                    if (botao.Text.Contains("só para Conta Premium"))
                    {
                        LogService.AdicionarLog(new Response("é premium", LevelLogEnum.Info));
                        ProximaVaga();
                    }
                    else
                    {
                        LogService.AdicionarLog(new Response("nao é premium", LevelLogEnum.Info));

                        var vaga = EnviarCurriculo();
                        listaVagas.Add(vaga);
                        ProximaVaga();
                    }
                }
                else
                {
                    // log erro
                    break;
                }
            }
        }

        private Vaga EnviarCurriculo()
        {
            Vaga vaga = RasparInformacoes();

            return vaga;
        }

        private void ProximaVaga()
        {
            LogService.AdicionarLog(new Response("indo proxima vaga", LevelLogEnum.Info));

            var botao = _webDriver.BuscarElementos(By.Id("ctl00_phMasterPage_cHeader_cNextPrev_aNext")).First();

            botao.Click();
            LogService.AdicionarLog(new Response("indo proxima vaga botao clicado", LevelLogEnum.Info));
            AguardarHtml(_dados.TimeOutAplicacao, By.ClassName("advisor-vacancy-content"));
            LogService.AdicionarLog(new Response("indo proxima vaga fim do aguardo", LevelLogEnum.Info));
        }

        private Vaga RasparInformacoes()
        {
            LogService.AdicionarLog(new Response("iniciando raspagem dados", LevelLogEnum.Info));
            var titulo = _webDriver.BuscarElementos(By.ClassName("header-content-left"))
                            .First()
                            .FindElement(By.TagName("h1"))
                            .GetAttribute("innerText");

            var dados = _webDriver.BuscarHtml(By.ClassName("divSubTitle"), "innerText").Split('\n');

            var salario = dados[0].Replace("\r", "");

            var local = dados[1];

            var descricao = _webDriver.BuscarHtml(By.ClassName("descriptionItems"), "innerText");

            var dadosSplit = descricao.Split(new string[] { "Benefícios:" }, StringSplitOptions.None);

            var beneficios = "";

            if (dadosSplit.Length >= 2)
                beneficios = dadosSplit[1];

            var empresa = _webDriver.BuscarHtml(By.Id("ctl00_phMasterPage_cHeader_aCompanyName"), "innerText");

            var vaga = new Vaga()
            {
                Beneficios = beneficios,
                Descricao = descricao,
                Local = local,
                NomeEmpresa = empresa,
                Salario = salario,
                Titulo = titulo
            };

            LogService.AdicionarLog(new Response("fim raspagem dados", LevelLogEnum.Info));

            return vaga;

        }

        private void RealizarLogin()
        {
            string response = String.Empty;

            LogService.AdicionarLog(new Response("Iniciando pagina de Login", LevelLogEnum.Info));

            AguardarHtml(_dados.TimeOutAplicacao, By.Id("Username"));
            response = _webDriver.BuscarHtml(By.ClassName("card-body"), "innerText");

            if (response.Contains("Acesso para usuários cadastrados"))
            {
                LogService.AdicionarLog(new Response("Pagina de Login iniciado", LevelLogEnum.Info));
                LogService.AdicionarLog(new Response("Inserindo Login e Senha", LevelLogEnum.Info));

                _webDriver.IncluirValor(By.Name("Username"), _dados.Login);
                _webDriver.IncluirValor(By.Name("Password"), _dados.Senha);
                _webDriver.ClicarBotao(By.Name("button"));

                LogService.AdicionarLog(new Response("Logando..", LevelLogEnum.Info));

                AguardarHtml(_dados.TimeOutAplicacao, By.Id("ctl00_cAccess_spanSession"));
                response = _webDriver.BuscarHtml(By.Id("ctl00_cAccess_spanSession"), "innerHTML");

                if (response.ToUpper().Contains(_dados.NomeUsuario.ToUpper()))
                {
                    LogService.AdicionarLog(new Response("Login efetuado com sucesso", LevelLogEnum.Info));
                    try
                    {
                        _webDriver.ClicarBotao(By.Id("AllowCookiesButton"));
                        Thread.Sleep(50);
                    }
                    catch (Exception) { }
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
