using Dapper;
using ServiceMonitor.Resource;
//using Models.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
//using Oracle.DataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using Renci.SshNet;
//using System.Web.Script.Serialization;


namespace ServiceMonitor.Models
{


    public class Dao 
    {


        /*
         * Obs: não utilizar (static): ex:  private static DbConnection _connection;
         * Diversos erros poderão ocorrer: ExecuteReader requer uma Connection aberta e disponível. O estado atual da conexão é conectando
         */
        private DbConnection _connection;
        private DbConnection _connectionServico;
        private Conexao Conexao;
        //private Utils _utils;
        //private JavaScriptSerializer serializer = new JavaScriptSerializer();


        public Dao()
        {
            Conexao = new Conexao();
            //_utils = new Utils();
        }


        //public Dao() : base() { }


        //Faz Log do Envio de email
        #region ListaTarefa
        public bool ListaTarefa(string strConexao)
        {
            string param = "";
            int tarefaCodigo = 0;

            string idLogTarefa = "";

            string SMSLogInicio = "";
            string SMSLogFim = "";

            string EmailLogInicio = "";
            string EmailLogFim = "";

            try
            {
                using (_connection = Conexao.getConnection(strConexao))
                {
                    string strSql = SQL.ListaTarefa;

                    //param = String.Format(" @EmailServidor = {0}, @EmailLayout = {1}, @Servico = {2}, @Usuario = {3} ,  @Emails = '{4}', @assunto = '{5}' , @corpo = '{6}', @Status = '{7}', @StatusMensagem = '{8}', @Origem = '{9}'", emailServidor, emailLayout, servico, usuario, emails, assunto, corpo, status, statusMensagem, origem);

                    //retorna todos os parâmetros do serviço 
                    string procedure = String.Format(" {0} {1} ", strSql, param);
                    var _data = _connection.Query(procedure).ToList();//.AsList();

                    foreach (var tarefa in _data)
                    {
                        tarefaCodigo = tarefa.Codigo != null ? Convert.ToInt32(tarefa.Codigo) : 0;

                        //Faz o envio de um SMS no início da execução da tarefa
                        if (tarefa.NotificacaoSQLSMSInicio == "S" && tarefa.NotificacaoSQLSMS != null && tarefa.NotificacaoSQLSMS != "")
                        {
                            string sql = String.Format(" {0} {1} {2}", tarefa.NotificacaoSQLSMS, ",@TarefaStatus = 'started'", ",@Tarefa = " + tarefa.Codigo);
                            SMSLogInicio = SendSms(sql, tarefa.StrConexao);
                        }

                        //grava o LOG da execução da tarefa
                        idLogTarefa = TarefaExecucaoLog(null, tarefaCodigo, null, 0, null, SMSLogInicio,null, null, null, strConexao);

                        /*
                        select * from [SM].[TarefaTipo]
                        1   Envio de SMS
                        2   Envio de E-mail
                        3   UploadFTP (sftp)
                        4   DownloadFTP (sftp)
                        */

                        //  1 : Envio de SMS
                        if (tarefa.Tipo == 1)
                        {
                            StreamWriter vWriter = new StreamWriter(@"c:\testeServico.txt", true);
                            vWriter.WriteLine("Servico Rodando: " + DateTime.Now.ToString() + " > " + tarefa.Nome);
                            vWriter.Flush();
                            vWriter.Close();

                            SendSms(tarefa.VarSQL, tarefa.StrConexao);

                        }
                        else if (tarefa.Tipo == 2)
                        {

                        }
                        else if (tarefa.Tipo == 3 || tarefa.Tipo == 4)
                        {
                            SFTP(tarefa.VarSQL, tarefa.StrConexao, tarefa.Tipo);
                        }


                        //Faz o envio de um SMS após a execução da tarefa
                        if (tarefa.NotificacaoSQLSMSFim == "S" && tarefa.NotificacaoSQLSMS != null && tarefa.NotificacaoSQLSMS != "")
                        {
                            string sql = String.Format(" {0} {1} {2}", tarefa.NotificacaoSQLSMS, ",@TarefaStatus = 'finished'", ",@Tarefa = " + tarefa.Codigo);
                            SMSLogFim = SendSms(sql, tarefa.StrConexao);
                        }

                        //atualiza o LOG da execução da tarefa
                        TarefaExecucaoLog(idLogTarefa, tarefaCodigo, "S", 0, null, SMSLogInicio, SMSLogFim, null, null, strConexao);
                    }


                    return true;
                }
            }
            catch (Exception ex)
            {
                //atualiza o LOG da execução da tarefa
                TarefaExecucaoLog(idLogTarefa, tarefaCodigo, "S", 1, ex.Message, SMSLogInicio, SMSLogFim, null, null, strConexao);
                throw new Exception(ex.Message);
            }

        }
        #endregion






        #region FTPUpdload
        public void SFTP(string sqlPRocedure, string StrConexao, int acao)
        {
            try
            {


                if (sqlPRocedure != null && sqlPRocedure != "")
                {

                    using (_connection = Conexao.getConnection(StrConexao))
                    {
                        //monta a procedure dinamicamente
                        string strSqlProcedure = String.Format(" {0}", sqlPRocedure);
                        //executa a procedure
                        var _data = _connection.Query(strSqlProcedure).ToList();

                        //itera sobre o resultado da procedure
                        foreach (var ftp in _data)
                        {

                            //string url = ftp.FTPServer+"/"+ ftp.FileUploadDirectory;
                            string ftpServer = ftp.FTPServer;
                            int ftpServerPort = Convert.ToInt32(ftp.FTPServerPort);
                            string ftpUser = ftp.FTPUser;
                            string ftpPass = ftp.FTPPass;

                            //parâmetros de Upload
                            string fileUpload = ftp.FileUpload;
                            string fileUploadDirectory = ftp.FileUploadDirectory;

                            //parâmetros de Download
                            string fileDownload = ftp.FileDownload;
                            string fileDownloadDirectory = ftp.FileDownloadDirectory;

                            using (var sftp = new SftpClient(ftpServer, ftpServerPort, ftpUser, ftpPass))
                            {

                                sftp.Connect();

                                if (acao == 3)
                                {
                                    using (var fs = File.OpenRead(fileUpload))
                                    {
                                        sftp.UploadFile(fs, fileUploadDirectory);
                                    }

                                }else if (acao == 4)
                                {
                                    using (var file = File.OpenWrite(fileDownloadDirectory))
                                    {
                                        sftp.DownloadFile(fileDownload, file);
                                    }
                                }
                                
                                sftp.Disconnect();
                            }

                            /*
                            FileInfo arquivoInfo = new FileInfo(arquivo);
                            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(url));
                            request.Method = WebRequestMethods.Ftp.UploadFile;
                            request.Credentials = new NetworkCredential(usuario, senha);

                            request.EnableSsl = true;
                            request.KeepAlive = false;
                            request.UsePassive = true;
                            request.Proxy = null;

                            //Set the proxy server login information
                            //WebProxy wproxy = new WebProxy("acesso.unimed:3128");
                            //wproxy.Credentials = new NetworkCredential("f05720", "Unimed88");
                            //request.Proxy = wproxy;

                            request.UseBinary = true;
                            request.ContentLength = arquivoInfo.Length;
                            using (FileStream fs = arquivoInfo.OpenRead())
                            {
                                byte[] buffer = new byte[2048];
                                int bytesSent = 0;
                                int bytes = 0;
                                using (Stream stream = request.GetRequestStream())
                                {
                                    while (bytesSent < arquivoInfo.Length)
                                    {
                                        bytes = fs.Read(buffer, 0, buffer.Length);
                                        stream.Write(buffer, 0, bytes);
                                        bytesSent += bytes;
                                    }
                                }
                            }
                            */


                        }

                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion






        //Método responsável pelo envio de SMS
        #region SendSms
        public string SendSms(string sqlPRocedure, string StrConexao)
        {

            dynamic retornoToken = null;
            dynamic retornoSend = null;
            //object retorno = new object();
            string idLog = "";

            try
            {
                using (_connection = Conexao.getConnection(StrConexao))
                {

                        if (sqlPRocedure != null && sqlPRocedure != "")
                        {
                        
                            //monta a procedure dinamicamente
                            string strSqlProcedure = String.Format(" {0}", sqlPRocedure);
                            //executa a procedure
                            var _data = _connection.Query(strSqlProcedure).ToList();

                            //atualiza log
                            idLog = SMSLog("", "", 0, "Serviço do Windows", "", 0, "", "", "", "", StrConexao);

                            //itera sobre o resultado da procedure
                            foreach (var sms in _data)
                            {

                                #region Token
                                string tokenUrl = sms.TokenUrl;
                                string tokenJson = sms.TokenJson.ToString();
                                string tokenVariavel = sms.TokenVariavel;

                                var clientToken = new RestClient();
                                clientToken.EndPoint = tokenUrl;
                                clientToken.Method = HttpVerb.POST;
                                clientToken.ContentType = @"application/json";

                                clientToken.PostData = tokenJson;
                                var returnToken = clientToken.MakeRequest();
                                retornoToken = JsonConvert.DeserializeObject(returnToken);
                                #endregion

                                //atualiza log
                                string jsonSaida = "Token: <br>";
                                jsonSaida += JsonConvert.SerializeObject(retornoToken, Formatting.Indented);
                                SMSLog(idLog, "", 0, tokenJson, jsonSaida, 0, "ServiceMonitor", strSqlProcedure, "", "", StrConexao);

                                //caso retorne o Token, faz o envio do SMS
                                if (retornoToken != null && retornoToken["access_token"] != null)
                                {
                                    var access_token = retornoToken["access_token"];

                                    #region SendSMS                                
                                    string sendUrl = sms.SendUrl;
                                    string sendJson = sms.SendJson;

                                    //faz o replace da rastreabilidade
                                    sendJson = sendJson.Replace(@"%rasteabilidade%", idLog);

                                    var clientSend = new RestClient();
                                    clientSend.EndPoint = sendUrl;
                                    clientSend.Method = HttpVerb.POST;
                                    clientSend.ContentType = @"application/json";
                                    clientSend.AuthorizationBearer = access_token;

                                    clientSend.PostData = sendJson;
                                    var returnSend = clientSend.MakeRequest();
                                    retornoSend = JsonConvert.DeserializeObject(returnSend);
                                    #endregion

                                    //atualiza log
                                    jsonSaida = "<br><br>Send: <br>";
                                    jsonSaida += JsonConvert.SerializeObject(retornoSend, Formatting.Indented);
                                    SMSLog(idLog, "", 0, sendJson, jsonSaida, 0, "ServiceMonitor", "", "", "", StrConexao);

                                    //caso retorne tokenId, a mensagem foi enviada com sucesso
                                    if (retornoSend != null && retornoSend["tokenId"] != null)
                                    {
                                        string tokenId = retornoSend["tokenId"];
                                        SMSLog(idLog, "", 0, "", "", 0, "ServiceMonitor", "", tokenId, "0", StrConexao);
                                    }
                                    else
                                    {
                                        SMSLog(idLog, "", 0, "", "", 0, "ServiceMonitor", "", "", "1", StrConexao);
                                    }

                            }
                            else
                            {
                                SMSLog(idLog, "", 0, "", "", 0, "ServiceMonitor", "", "", "1", StrConexao);
                            }

                            }

                        }
                        else
                        {
                            SMSLog(idLog, "", 0, "", "Procedure não definida", 0, "ServiceMonitor", "", "", "1", StrConexao);
                        }



                    return idLog;
                }
            }
            catch (Exception ex)
            {
                SMSLog(idLog, "", 0, "", "Exceção: "+ ex.Message, 0, "ServiceMonitor", "", "", "2", StrConexao);
                return idLog;
            }


        }
        #endregion


        //Faz Log na chamada de Serviços
        #region SMSLog
        public string SMSLog(string codigo, string servicoNome, int servico, string jsonEntrada, dynamic jsonSaida, int usuario, string origem, string sqlQuery, string tokenId, string outErro, string strConexao)
        {
            string param = "";

            try
            {
                using (_connection = Conexao.getConnection(strConexao))
                {
                    string strSql = Seguranca.SMSLog;

                    if (codigo != null && codigo != "") //update
                    {
                        param = String.Format(" @Codigo = {0}, @JsonSaida = '{1}',  @Servico = {2}, @Usuario = {3}, @Origem = '{4}', @sqlQuery = '{5}', @tokenId = '{6}' , @Status = '{7}', @JsonEntrada = '{8}' ", codigo, jsonSaida.Replace(@"'", "''"), servico, usuario, origem, sqlQuery.Replace(@"'", "''"), tokenId, outErro, jsonEntrada);
                    }
                    else //insert
                    {
                        param = String.Format(" @ServicoNome = '{0}', @JsonEntrada = '{1}'", servicoNome, jsonEntrada);
                        //param = String.Format(" @Codigo = {0}, @JsonSaida = '{1}',  @Servico = {2}, @Usuario = {3}, @Origem = '{4}', @sqlQuery = '{5}', @Status = '{6}' , @tokenId = '{7}' ", codigo, jsonSaida.Replace(@"'", "''"), servico, usuario, origem, sqlQuery.Replace(@"'", "''"), tokenId, outErro);
                    }

                    //retorna todos os parâmetros do serviço 
                    string procedure = String.Format(" {0} {1} ", strSql, param);
                    var objLog = _connection.Query(procedure).SingleOrDefault();//.AsList();

                    dynamic idLog = Utils.ExtrairValueKeyValuePair(objLog, "ID");
                    idLog = idLog == null || idLog == "" ? 0 : idLog;

                    return idLog;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(cod:SMSLog)");
            }

        }
        #endregion



        //Faz Log da Tarefa
        #region TarefaLog
        public string TarefaExecucaoLog(string codigo, int Tarefa, string DataExecucaoFim, int? Situacao, string Erro, string SMSLogInicio, string SMSLogFim, string EmailLogInicio, string EmailLogFim, string strConexao)
        {
            string param = "";

            try
            {
                using (_connection = Conexao.getConnection(strConexao))
                {
                    string strSql = Seguranca.TarefaExecucaoLog;

                    if (codigo != null && codigo != "") //update
                    {                        
                        param = String.Format(" @Codigo = {0}, @DataExecucaoFim = '{1}',  @Situacao = {2}, @Erro = '{3}', @SMSLogInicio = '{4}', @SMSLogFim = '{5}', @EmailLogInicio = '{6}' , @EmailLogFim = '{7}'", codigo, DataExecucaoFim, Situacao, Erro, SMSLogInicio, SMSLogFim, EmailLogInicio, EmailLogFim);
                    }
                    else //insert
                    {
                        param = String.Format(" @Tarefa = '{0}'", Tarefa);
                    }

                    //retorna todos os parâmetros do serviço 
                    string procedure = String.Format(" {0} {1} ", strSql, param);
                    var objLog = _connection.Query(procedure).SingleOrDefault();//.AsList();

                    dynamic idLog = Utils.ExtrairValueKeyValuePair(objLog, "ID");
                    idLog = idLog == null || idLog == "" ? 0 : idLog;

                    return idLog;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(cod:SMSLog)");
            }

        }
        #endregion





    }
}



