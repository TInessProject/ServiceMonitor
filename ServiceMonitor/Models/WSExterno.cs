using Dapper;
using M0.Resource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
//using System.Web.Script.Serialization;

namespace ServiceMonitor.Models
{
    public class WSExterno
    {


        private DbConnection _connection;
        private Conexao Conexao;

        
        public WSExterno()
        {
            Conexao = new Conexao();
            //_utils = new Utils();
        }

        //Faz Log no Cartão de Crédito
        #region CreditCardLog
        /*
        public string CreditCardLog(string codigo, string tipoTransacao, string docFinanceiro, string contrato, string valor, string quantParcelas, string valorParcelas, string creditCardNumero, string creditCardNome, string creditCardValidadeMes, string creditCardValidadeAno, string xmlEnvio, string xmlRetorno, string status, string data, string strConexao)
        {
            string param = "";

            try
            {
                using (_connection = Conexao.getConnection(strConexao))
                {
                    string strSql = Seguranca.CreditCardLog;

                    // UPDATE
                    if (codigo != null && codigo != "")
                    {
                        param = String.Format("@Codigo = {0}," +
                                              "@TipoTransacao = '{1}'," +
                                              "@DocFinanceiro = '{2}'," +
                                              "@Contrato = '{4}'," +
                                              "@Valor = {5}," +
                                              "@QuantParcelas = {6}," +
                                              "@ValorParcelas = {7}," +
                                              "@CreditCardNumero = '{8}'," +
                                              "@CreditCardNome = '{9}'," +
                                              "@CreditCardValidadeMes = '{10}'," +
                                              "@CreditCardValidadeAno = '{11}'," +
                                              "@XmlEnvio = '{12}'," +
                                              "@XmlRetorno = '{13}'," +
                                              "@Status = '{14}'," +
                                              "@Data = '{15}'",
                                              codigo,
                                              tipoTransacao,
                                              !string.IsNullOrEmpty(docFinanceiro) ? docFinanceiro : "",
                                              !string.IsNullOrEmpty(contrato) ? contrato : "",
                                              Convert.ToDecimal(valor),
                                              !string.IsNullOrEmpty(quantParcelas) ? quantParcelas : "0",
                                              !string.IsNullOrEmpty(valorParcelas) ? valorParcelas : "0",
                                              !string.IsNullOrEmpty(creditCardNumero) ? creditCardNumero : "",
                                              !string.IsNullOrEmpty(creditCardNome) ? creditCardNome : "",
                                              !string.IsNullOrEmpty(creditCardValidadeMes) ? creditCardValidadeMes : "",
                                              !string.IsNullOrEmpty(creditCardValidadeAno) ? creditCardValidadeAno : "",
                                              !string.IsNullOrEmpty(xmlEnvio) ? xmlEnvio : "",
                                              !string.IsNullOrEmpty(xmlRetorno) ? xmlRetorno : "",
                                              !string.IsNullOrEmpty(status) ? status : "",
                                              DateTime.Now.ToString());
                    }
                    // INSERT
                    else
                    {
                        param = String.Format("@TipoTransacao = '{0}'," +
                                              "@DocFinanceiro = '{1}'," +
                                              "@Contrato = '{2}'," +
                                              "@Valor = {3}," +
                                              "@QuantParcelas = {4}," +
                                              "@ValorParcelas = {5}," +
                                              "@CreditCardNumero = '{6}'," +
                                              "@CreditCardNome = '{7}'," +
                                              "@CreditCardValidadeMes = '{8}'," +
                                              "@CreditCardValidadeAno = '{9}'," +
                                              "@XmlEnvio = '{10}'," +
                                              "@XmlRetorno = '{11}'," +
                                              "@Status = '{12}'," +
                                              "@Data = '{13}'",
                                              tipoTransacao,
                                              !string.IsNullOrEmpty(docFinanceiro) ? docFinanceiro : "",
                                              !string.IsNullOrEmpty(contrato) ? contrato : "",
                                              Convert.ToDecimal(valor),
                                              !string.IsNullOrEmpty(quantParcelas) ? quantParcelas : "0",
                                              !string.IsNullOrEmpty(valorParcelas) ? valorParcelas : "0",
                                              !string.IsNullOrEmpty(creditCardNumero) ? creditCardNumero : "",
                                              !string.IsNullOrEmpty(creditCardNome) ? creditCardNome : "",
                                              !string.IsNullOrEmpty(creditCardValidadeMes) ? creditCardValidadeMes : "",
                                              !string.IsNullOrEmpty(creditCardValidadeAno) ? creditCardValidadeAno : "",
                                              !string.IsNullOrEmpty(xmlEnvio) ? xmlEnvio : "",
                                              !string.IsNullOrEmpty(xmlRetorno) ? xmlRetorno : "",
                                              !string.IsNullOrEmpty(status) ? status : "",
                                              DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
                throw new Exception(ex.Message + "(cod:ServicoLog)");
            }

        }
        */
        #endregion



        /*
        #region CARTAO DE CRÉDITO/DÉBITO
        public object CreditCard(dynamic Parametros, string Operadora, string TransacaoCodigo, string strConexao)
        {
            dynamic retorno = null;
            string endPoint = null;
            string jsonModelo = null;

            try
            {

                using (_connection = Conexao.getConnection(strConexao))
                {

                    //var creditCardOperadora = Utils.ExtrairValueJSON(Parametros, "Operadora");
                    var creditCardOperadora = Operadora;

                    var numeroCartao = Utils.ExtrairValueJSON(Parametros, "Numero");
                    var nomePropCartao = Utils.ExtrairValueJSON(Parametros, "Nome");
                    var validadeMesCartao = Utils.ExtrairValueJSON(Parametros, "ValidadeMes");
                    var validadeAnoCartao = Utils.ExtrairValueJSON(Parametros, "ValidadeAno");
                    var codSegCartao = Utils.ExtrairValueJSON(Parametros, "CodigoSeguranca");

                    //var contrato = Utils.ExtrairValueJSON(Parametros, "Contrato");
                    var valorTotal = Utils.ExtrairValueJSON(Parametros, "ValorTotal");
                    var parcelas = Utils.ExtrairValueJSON(Parametros, "Parcelas");
                    var valorParcela = Utils.ExtrairValueJSON(Parametros, "ValorParcela");
                    var tipo = Utils.ExtrairValueJSON(Parametros, "Tipo");
                    var Email = Utils.ExtrairValueJSON(Parametros, "Email");
                    var celular = Utils.ExtrairValueJSON(Parametros, "Celular");

                    
                    var quantidadeMensalidades = Utils.ExtrairValueJSON(Parametros, "QuantidadeMensalidades");

                    JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    var configCreditCard = _connection.Query(WSExt.CreditCard, new { Codigo = creditCardOperadora }).SingleOrDefault();                    
                    foreach (KeyValuePair<string, object> result in configCreditCard)
                    {
                        string campoNome = result.Key.ToString().ToLower();
                        string campoValor = result.Value != null ? result.Value.ToString() : "";

                        if (campoNome == "endpoint")
                            endPoint = campoValor;

                        if (campoNome == "jsonmodelo")
                            jsonModelo = campoValor;
                    }

                    jsonModelo = (Convert.ToString(jsonModelo)
                                  .Replace("%numeroCartao%", numeroCartao)
                                  .Replace("%nomePropCartao%", nomePropCartao)
                                  .Replace("%validadeMesCartao%", validadeMesCartao)
                                  .Replace("%validadeAnoCartao%", validadeAnoCartao)
                                  .Replace("%codSegCartao%", codSegCartao)
                                  .Replace("%transacaoCodigo%", TransacaoCodigo)
                                  .Replace("%valorTotal%", valorTotal)
                                  .Replace("%parcelas%", parcelas)
                                  .Replace("%tipo%", tipo)
                               );

                    // CONFORME INFORMADO PELO KLEBER, DEVE SER GRAVADO LOG CONTINUO DA TRANSAÇÃO.

                    //CreditCardLog(null, tipo, codigoDoc, null, valor, parcelas, null, numeroCartao, nomePropCartao, validadeMesCartao, validadeAnoCartao, strJson, null, "ENVIANDO PARA REDE", DateTime.Now.ToString(), strConexao);

                    var client = new RestClient();
                    client.EndPoint = endPoint;// @"https://synergius.unimedbelem.com.br:8080/erede/";
                    client.Method = HttpVerb.POST;
                    client.ContentType = @"application/json";

                    //strJson = serializer.Deserialize(strJson);
                    //string output = JsonConvert.SerializeObject(strJson);
                    client.PostData = jsonModelo;
                    var json = client.MakeRequest();
                    retorno = serializer.DeserializeObject(json);

                    //dynamic retorno = resultado;
                    int statusLocal = 10;
                    int statusRede = 0;

                    // verificar se o retorno da rede foi sucesso ou não
                    if (retorno["OutErro"].Equals("0"))
                    {
                        //eRede SUCESSO: TRANSAÇÃO AUTORIZADA COM SUCESSO
                        //Quando a transação é enviada a REDE e a resposta é sucesso na autorização
                        //statusLocal = 10;
                    }
                    else if (retorno["OutErro"].Equals("1"))
                    {
                        //eRede ERRO: TRANSAÇÃO NÃO AUTORIZADA
                        //Quando a transação é enviada a REDE e a resposta retorna algum erro
                        //statusLocal = 2;

                        if( retorno["RedeExceptionCode"] != null)
                        {
                            statusRede = Convert.ToInt32(retorno["RedeExceptionCode"]);
                        }
                        
                    }
                    
                    
                    //grava log via procedure
                    var p = new DynamicParameters();
                    p.Add("@Codigo", null, dbType: DbType.Int32, direction: ParameterDirection.Input);

                    p.Add("@ValorTotal", valorTotal, dbType: DbType.String, direction: ParameterDirection.Input);
                    p.Add("@QuantMensalidades", quantidadeMensalidades, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    p.Add("@QuantParcelas", parcelas, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    p.Add("@ValorParcela", valorParcela, dbType: DbType.String, direction: ParameterDirection.Input);

                    p.Add("@CreditCardNumero", numeroCartao, dbType: DbType.String, direction: ParameterDirection.Input);
                    p.Add("@CreditCardNome", nomePropCartao, dbType: DbType.String, direction: ParameterDirection.Input);
                    p.Add("@CreditCardValidadeMes", validadeMesCartao, dbType: DbType.String, direction: ParameterDirection.Input);
                    p.Add("@CreditCardValidadeAno", validadeAnoCartao, dbType: DbType.String, direction: ParameterDirection.Input);       
                    
                    p.Add("@DadosEnvio", jsonModelo, dbType: DbType.String, direction: ParameterDirection.Input);
                    p.Add("@DadosRetorno", json, dbType: DbType.String, direction: ParameterDirection.Input);
                    p.Add("@StatusLocal", statusLocal, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    p.Add("@StatusRede", statusRede, dbType: DbType.String, direction: ParameterDirection.Input);

                    p.Add("@Transacao", TransacaoCodigo, dbType: DbType.Int32, direction: ParameterDirection.Input);

                    dynamic rsProcedure = _connection.Query(WSExt.CreditCardLog, p, commandType: CommandType.StoredProcedure);
                   
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(cod:ExpWSECreditCard)");
            }

            return retorno;

        }
        #endregion
        */


    }
}
