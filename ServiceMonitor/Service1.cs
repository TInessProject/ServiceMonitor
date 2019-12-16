using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using ServiceMonitor.Resource;
using ServiceMonitor.Models;


namespace ServiceMonitor
{
    public partial class Service1 : ServiceBase
    {
        Timer timer1;
        Dao Dao;

        public Service1()
        {
            InitializeComponent();
        }


        public void onDebug()
        {
            //OnStart(null);
            timer1_Tick(null);
        }

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer(new TimerCallback(timer1_Tick), null, 15000, 60000);
        }

        protected override void OnStop()
        {

            StreamWriter vWriter = new StreamWriter(@"c:\testeServico.txt", true);

            vWriter.WriteLine("Servico Parado: " + DateTime.Now.ToString());
            vWriter.Flush();
            vWriter.Close();
        }


        private void timer1_Tick(object sender)
        {


            Dao = new Dao();

            //por padrão busca a Lista de Tarefas no BD de Produção
            Dao.ListaTarefa("dL=slUpSuiGrEnysXaiJLY13riOUGNfPBcg==");


        }

    }
}
