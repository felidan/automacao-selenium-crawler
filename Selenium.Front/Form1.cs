using Selenium.Utils.Entidades;
using Selenium.Utils.Logs;
using SeleniumCatho;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Selenium.Front
{
    public partial class Form1 : Form
    {
        private bool ProcessoFinalizado;
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnProcessar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                this.txtLog.Text = "";

                ProcessoFinalizado = false;

                await Task.Factory.StartNew(ExecutarProcesso);
                AtualizarLog();

                this.txtLog.Text += "PROCESSO FINALIZADO";
            }
            else
            {
                this.txtLog.Text = "Dados incompletos. Preencha os dados";
            }
        }

        private void ExecutarProcesso()
        {
            SeleniumModel model = new SeleniumModel()
            {
                Browser = Utils.Enum.BrowserEnum.Chrome,
                DriverPath = @"D:\Projetos\AutomacaoRobo\Selenium.Utils\Drivers\",
                Login = this.txtEmail.Text,
                Senha = this.txtSenha.Text,
                OcultarJanela = !this.ckVizualizar.Checked,
                TimeOutAplicacao = TimeSpan.FromSeconds(30),
                Url = "https://seguro.catho.com.br/signin/",
                Sistema = Utils.Enum.SistemaEnum.Catho,
                NomeUsuario = this.txtNome.Text.Split(' ')[0],
                Cargos = new List<string>(this.txtCargo.Text.Split(' ')),
                ResumoProfissional = this.txtResumo.Text
            };

            try
            {
                CoreAutomacao core = new CoreAutomacao();
                var response = core.Processar(model);
            }
            catch (Exception ex)
            {
            }

            ProcessoFinalizado = true;

        }

        private void AtualizarLog()
        {
            int countLogs = -1;
            while (!ProcessoFinalizado)
            {
                if (countLogs == -1 || LogService.GetLogs().Count > countLogs)
                {
                    countLogs = PrintLog(countLogs);
                }
            }

            PrintLog(countLogs);
        }

        private int PrintLog(int countLogs)
        {
            var logs = LogService.GetLogs();

            logs.RemoveRange(0, countLogs == -1 ? 0 : countLogs);

            foreach (var x in logs)
            {
                this.txtLog.Text += $"{x.Level.ToString()} - {x.Mensagem}{Environment.NewLine}";
            }

            Thread.Sleep(3000);

            return LogService.GetLogs().Count;
        }

        private bool ValidarDados()
        {
            if (!String.IsNullOrEmpty(this.txtEmail.Text)
                && !String.IsNullOrEmpty(this.txtSenha.Text)
                && !String.IsNullOrEmpty(this.txtNome.Text)
                && !String.IsNullOrEmpty(this.txtCargo.Text)
                && !String.IsNullOrEmpty(this.txtResumo.Text))
                return true;

            else
                return false;
        }
    }
}
