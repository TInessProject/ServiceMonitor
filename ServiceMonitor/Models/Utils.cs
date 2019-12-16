using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using M0.Resource;
using System.Data.Common;
using Dapper;
//using M0.Models.Entity;
using System.Linq;
//using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Security.Principal;
using System.Dynamic;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net.Mime;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Diagnostics;
using System.Drawing;

namespace ServiceMonitor.Models
{
    public class Utils 
    {

        private static byte[] chave = { };
        private static byte[] iv = { 12, 34, 56, 78, 90, 102, 114, 126 };
        private Dao Dao;
        private DbConnection _connection;
        private Conexao Conexao;

        private static string chaveCriptografia = "M0GestaoPlanoSaude";

        public Utils()
        {
            Conexao = new Conexao();
        }


         
        // MÉTODO QUE RETORNA O VALOR DO CAMPO INFORMADO DENTRO DO OBJETO EM QUESTÃO
        #region GetPropertyValue
        public static dynamic GetPropertyValue(object objeto, string CampoNome)
        {
            return objeto.GetType().GetProperty(CampoNome).GetValue(objeto, null);
        }
        #endregion

        
        public static string CriptografarUrl(string valor)
        {
            DESCryptoServiceProvider des;
            MemoryStream ms;
            CryptoStream cs; byte[] input;

            try
            {
                des = new DESCryptoServiceProvider();
                ms = new MemoryStream();


                input = Encoding.UTF8.GetBytes(valor); chave = Encoding.UTF8.GetBytes(chaveCriptografia.Substring(0, 8));

                cs = new CryptoStream(ms, des.CreateEncryptor(chave, iv), CryptoStreamMode.Write);
                cs.Write(input, 0, input.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray()).Replace("/", "=kCabSuiGrEnys").Replace("+", "=slUpSuiGrEnys");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Descriptografa o cookie
        #region DescriptografarUrl
        public static string DescriptografarUrl(string valor)
        {
            DESCryptoServiceProvider des;
            MemoryStream ms;
            CryptoStream cs; byte[] input;

            try
            {
                des = new DESCryptoServiceProvider();
                ms = new MemoryStream();

                input = new byte[valor.Length];

                if (valor.Contains("=kCabSuiGrEnys"))
                {
                    valor = valor.Replace("=kCabSuiGrEnys", "/");
                }
                if (valor.Contains("=slUpSuiGrEnys"))
                {
                    valor = valor.Replace("=slUpSuiGrEnys", "+");
                }

                input = Convert.FromBase64String(valor.Replace(" ", "+"));

                chave = Encoding.UTF8.GetBytes(chaveCriptografia.Substring(0, 8));

                cs = new CryptoStream(ms, des.CreateDecryptor(chave, iv), CryptoStreamMode.Write);
                cs.Write(input, 0, input.Length);
                cs.FlushFinalBlock();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //Criptografa o Cookie
        #region Criptografar
        public static string Criptografar(string valor)
        {
            DESCryptoServiceProvider des;
            MemoryStream ms;
            CryptoStream cs; byte[] input;

            try
            {
                des = new DESCryptoServiceProvider();
                ms = new MemoryStream();

                input = Encoding.UTF8.GetBytes(valor); chave = Encoding.UTF8.GetBytes(chaveCriptografia.Substring(0, 8));

                cs = new CryptoStream(ms, des.CreateEncryptor(chave, iv), CryptoStreamMode.Write);
                cs.Write(input, 0, input.Length);
                cs.FlushFinalBlock();

                string sc = Convert.ToBase64String(ms.ToArray());
                string sd = Descriptografar(sc);

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //Descriptografa o cookie
        #region Descriptografar
        public static string Descriptografar(string valor)
        {
            DESCryptoServiceProvider des;
            MemoryStream ms;
            CryptoStream cs; byte[] input;

            try
            {
                des = new DESCryptoServiceProvider();
                ms = new MemoryStream();

                input = new byte[valor.Length];
                input = Convert.FromBase64String(valor.Replace(" ", "+"));

                chave = Encoding.UTF8.GetBytes(chaveCriptografia.Substring(0, 8));

                cs = new CryptoStream(ms, des.CreateDecryptor(chave, iv), CryptoStreamMode.Write);
                cs.Write(input, 0, input.Length);
                cs.FlushFinalBlock();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GerarHashMd5
        public static string GerarHashMd5(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Converter a String para array de bytes, que é como a biblioteca trabalha.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Cria-se um StringBuilder para recompôr a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop para formatar cada byte como uma String em hexadecimal
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        #endregion

        //Método responsável por validar campos obrigatórios de segurança
        #region ValidarCamposSeguranca
        public static bool ValidarCamposSeguranca(dynamic objeto)
        {
            bool retorno        = false;
            bool UsuarioCodigo  = false;
            bool Tpc            = false;
            bool Ip             = false;
            bool Mac            = false;

            try
            {


                dynamic parametros = JsonConvert.DeserializeObject(objeto.ToString());

                foreach (JProperty param in parametros)
                {
                    dynamic name  = param.Name;
                    dynamic value = param.Value.ToString();

                    if (param.Name == "Tpc" && param.Value != null && param.Value.ToString() != "")
                    {
                        Tpc = true;
                    }

                    if (param.Name == "Ip" && param.Value != null)
                    {
                        Ip = true;
                    }

                    if (param.Name == "Mac" && param.Value != null)
                    {
                        Mac = true;
                    }

                }

                //ar thead = (from x in parametros1 select x).Where(dado => dado.Tipo == "S" && !dado.Tipo_Entrada.Contains("Btn")).Select(dado => new { textname = dado.Nome, textshow = dado.Nome_Exibicao, show = dado.Visivel, key = dado.Chave, Mask = dado.Mascara, size = dado.Tamanho > 0 ? dado.Tamanho : "" }).ToList();

                //System.IO.File.WriteAllText(@"C:\salesforce\processList.json", JsonConvert.SerializeObject(parametros));

                /*
                //Descerializa jSon e converte em um Array de chave e valor
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dynamic parametros = serializer.DeserializeObject(objeto);

                foreach (KeyValuePair<string, object> param in parametros)
                {
                    //if (param.Key == "UsuarioCodigo" && param.Value != null && param.Value != "")
                    //{
                    //    UsuarioCodigo = true;
                    //}

                    if (param.Key == "Tpc" && param.Value != null && param.Value != "")
                    {
                        Tpc = true;
                    }

                    if (param.Key == "Ip" && param.Value != null && param.Value != "")
                    {
                        Ip = true;
                    }

                    if (param.Key == "Mac" && param.Value != null && param.Value != "")
                    {
                        Mac = true;
                    }
                }
                */

                if (Tpc && (Ip || Mac))
                {
                    retorno = true;
                }

            }
            catch (Exception e)
            {
                retorno = false;
            }

            return retorno;
        }
        #endregion

        //Método responsável por retornar todas as condicoes enviadas via json, em uma string formatada para utilização na chamada da procedure
        #region AgruparParamentrosCondicao
        public static string AgruparParametrosCondicao(int Servico, dynamic parametrosRequisicao, DbConnection _connection)
        {
            string condicoes = "";

            //itera sobre todos os parâmetros enviados na requisição
            foreach (JProperty param in parametrosRequisicao)
            //foreach (KeyValuePair<string, object> param in parametrosRequisicao)
            {
                //verifica se os parâmetros foram preenchidos
                if (param.Value != null )
                {
                    string campo = param.Name.ToString();


                    int qtd = 0;
                    //itera sobre ParametrosCondicao
                    if (campo == "ParametrosCondicao")
                    {
                        dynamic paramentrosCondicao = (JArray)param.Value;
                        for (int i = 0; i < paramentrosCondicao.Count; i++)
                        {
                            var tipo = "";
                            var nome = "";
                            var valor = "";

                            foreach (JProperty paramCondicao in paramentrosCondicao[i])
                            //foreach (KeyValuePair<string, object> paramCondicao in paramentrosCondicao[i])
                            {
                                string campoCondicao = paramCondicao.Name.ToString();
                                string valorCondicao = paramCondicao.Value == null ? "" : paramCondicao.Value.ToString();

                                //lista todos os parâmetros do serviço
                                var servicoParametros = Utils.BuscarServicoParametros(Servico, _connection);

                                //verifica se os parâmetros foram preenchidos
                                if (paramCondicao.Name.ToUpper() == "NOME")
                                {

                                    //recupera o tipo do campo. (deve estar habilitado em Pesquisa=S)
                                    var tipo_dado = (from x in servicoParametros select x).Where(dado => dado.Nome.ToLower() == valorCondicao.ToLower() && dado.Pesquisa == "S" && dado.Tipo_Dado != "" && dado.Tipo_Dado != null).Select(dado => dado.Tipo_Dado).ToList();


                                    //verifica se os parâmetros foram preenchidos
                                    if (tipo_dado.Count() > 0)
                                    {
                                        tipo = tipo_dado[0];
                                    }

                                    nome = valorCondicao;
                                }

                                //verifica se os parâmetros foram preenchidos
                                if (paramCondicao.Name.ToUpper() == "VALOR")
                                {
                                    valor = valorCondicao;
                                }
                            }

                            if (nome != null && nome != "" &
                                valor != null && valor != "" &
                                tipo != null && tipo != "")
                            {

                                condicoes += qtd > 0 ? " , " : "";

                                //valido o tipo de dado
                                if (tipo.ToLower() == "string")
                                {
                                    condicoes += String.Format(" @{0} = '{1}' ", nome, valor);
                                    qtd++;
                                }

                                if (tipo.ToLower() == "int")
                                {
                                    condicoes += String.Format(" @{0} = {1} ", nome, valor);
                                    qtd++;
                                }
                            }


                        }
                    }


                }
            }

            return condicoes;
        }
        #endregion

        //Método responsável por retornar todas as condicoes enviadas via json, em uma string formatada para utilização na chamada da procedure
        #region AgruparParamentrosCondicao
        public static string AgruparParametrosCondicaoSemServico(dynamic parametrosRequisicao, DbConnection _connection)
        {
            string condicoes = "";

            //itera sobre todos os parâmetros enviados na requisição
            //foreach (KeyValuePair<string, object> param in parametrosRequisicao)
            foreach (JProperty param in parametrosRequisicao)
            {
                //verifica se os parâmetros foram preenchidos
                if (param.Value != null && param.Value.ToString() != "")
                {
                    string campo = param.Name.ToString();


                    int qtd = 0;
                    //itera sobre ParametrosCondicao
                    if (campo == "ParametrosCondicao")
                    {
                        dynamic paramentrosCondicao = (JArray)param.Value;
                        for (int i = 0; i < paramentrosCondicao.Count; i++)
                        {
                            var tipo = "string";
                            var nome = "";
                            var valor = "";

                            foreach (JProperty paramCondicao in paramentrosCondicao[i])
                            //foreach (KeyValuePair<string, object> paramCondicao in paramentrosCondicao[i])
                            {
                                string campoCondicao = paramCondicao.Name.ToString();
                                string valorCondicao = paramCondicao.Value == null ? "" : paramCondicao.Value.ToString();


                                //verifica se os parâmetros foram preenchidos
                                if (paramCondicao.Name.ToUpper() == "NOME")
                                {
                                    nome = valorCondicao;
                                }

                                //verifica se os parâmetros foram preenchidos
                                if (paramCondicao.Name.ToUpper() == "VALOR")
                                {
                                    valor = valorCondicao;
                                }
                            }

                            if (nome != null && nome != "" &
                                valor != null && valor != "" &
                                tipo != null && tipo != "")
                            {

                                condicoes += qtd > 0 ? " , " : "";

                                //valido o tipo de dado
                                if (tipo.ToLower() == "string")
                                {
                                    condicoes += String.Format(" @{0} = '{1}' ", nome, valor);
                                    qtd++;
                                }

                                if (tipo.ToLower() == "int")
                                {
                                    condicoes += String.Format(" @{0} = {1} ", nome, valor);
                                    qtd++;
                                }
                            }


                        }
                    }


                }
            }

            return condicoes;
        }
        #endregion

        //Método responsável por retornar todas as condicoes enviadas via json, em uma string formatada para utilização como AND em um SELECT
        #region AgruparParamentrosCondicao
        public static string AgruparParametrosCondicaoSemServicoSELECT(dynamic parametrosRequisicao, DbConnection _connection)
        {
            string condicoes = "";

            //itera sobre todos os parâmetros enviados na requisição
            //foreach (KeyValuePair<string, object> param in parametrosRequisicao)
            foreach (JProperty param in parametrosRequisicao)
            {
                //verifica se os parâmetros foram preenchidos
                if (param.Value != null && param.Value.ToString() != "")
                {
                    string campo = param.Name.ToString();


                    int qtd = 0;
                    //itera sobre ParametrosCondicao
                    if (campo == "ParametrosCondicao")
                    {
                        dynamic paramentrosCondicao = (JArray)param.Value;
                        for (int i = 0; i < paramentrosCondicao.Count; i++)
                        {
                            var tipo = "string";
                            var nome = "";
                            var valor = "";

                            foreach (JProperty paramCondicao in paramentrosCondicao[i])
                            //foreach (KeyValuePair<string, object> paramCondicao in paramentrosCondicao[i])
                            {
                                string campoCondicao = paramCondicao.Name.ToString();
                                string valorCondicao = paramCondicao.Value == null ? "" : paramCondicao.Value.ToString();


                                //verifica se os parâmetros foram preenchidos
                                if (paramCondicao.Name.ToUpper() == "NOME")
                                {
                                    nome = valorCondicao;
                                }

                                //verifica se os parâmetros foram preenchidos
                                if (paramCondicao.Name.ToUpper() == "VALOR")
                                {
                                    valor = valorCondicao;
                                }
                            }

                            if (nome != null && nome != "" &
                                valor != null && valor != "" &
                                tipo != null && tipo != "")
                            {

                                condicoes += qtd > 0 ? " and " : " and ";

                                //valido o tipo de dado
                                if (tipo.ToLower() == "string")
                                {
                                    condicoes += String.Format(" {0} = '{1}' ", nome, valor);
                                    qtd++;
                                }

                                if (tipo.ToLower() == "int")
                                {
                                    condicoes += String.Format(" {0} = {1} ", nome, valor);
                                    qtd++;
                                }
                            }


                        }
                    }


                }
            }

            return condicoes;
        }
        #endregion

        //Método responsável por retornar todas as condicoes enviadas via json, em uma string formatada para utilização como replace em um SELECT
        #region AgruparParamentrosCondicao
        public static string AgruparParametrosCondicaoSemServicoREPLACE(string SQL, dynamic parametrosRequisicao, DbConnection _connection)
        {
            string condicoes = "";

            //itera sobre todos os parâmetros enviados na requisição
            //foreach (KeyValuePair<string, object> param in parametrosRequisicao)
            foreach (JProperty param in parametrosRequisicao)
            {
                //verifica se os parâmetros foram preenchidos
                if (param.Value != null && param.Value.ToString() != "")
                {
                    string campo = param.Name.ToString();


                    int qtd = 0;
                    //itera sobre ParametrosCondicao
                    if (campo == "ParametrosCondicao")
                    {
                        dynamic paramentrosCondicao = (JArray)param.Value;
                        for (int i = 0; i < paramentrosCondicao.Count; i++)
                        {
                            var tipo = "string";
                            var nome = "";
                            var valor = "";

                            foreach (JProperty paramCondicao in paramentrosCondicao[i])
                            //foreach (KeyValuePair<string, object> paramCondicao in paramentrosCondicao[i])
                            {
                                string campoCondicao = paramCondicao.Name.ToString();
                                string valorCondicao = paramCondicao.Value == null ? "" : paramCondicao.Value.ToString();


                                //verifica se os parâmetros foram preenchidos
                                if (paramCondicao.Name.ToUpper() == "NOME")
                                {
                                    nome = valorCondicao;
                                }

                                //verifica se os parâmetros foram preenchidos
                                if (paramCondicao.Name.ToUpper() == "VALOR")
                                {
                                    valor = valorCondicao;
                                }
                            }

                            if (nome != null && nome != "" &
                                valor != null && valor != "" &
                                tipo != null && tipo != "")
                            {

                                condicoes += qtd > 0 ? " and " : " and ";

                                //valido o tipo de dado
                                if (tipo.ToLower() == "string")
                                {
                                    SQL = SQL.Replace(nome, valor);
                                    //condicoes += String.Format(" {0} = '{1}' ", nome, valor);
                                    qtd++;
                                }
                                /*
                                if (tipo.ToLower() == "int")
                                {
                                    condicoes += String.Format(" {0} = {1} ", nome, valor);
                                    qtd++;
                                }*/
                            }


                        }
                    }


                }
            }

            return SQL;
        }
        #endregion


        //Método responsável por retornar todas as condicoes enviadas via json, e tranformar em uma lista
        //utilizados por: ListarGrid -> retornar o valor informar em um campo de pesquisa(find)
        #region AgruparParametrosCondicaoLista
        public static List<KeyValuePair<string, string>> AgruparParametrosCondicaoLista(dynamic parametrosRequisicao)
        {
            string condicoes = "";
            //List<KeyValuePair<string, object>> lista = new List<KeyValuePair<string, object>>();
            List<KeyValuePair<string, string>> lista = new List<KeyValuePair<string, string>>();

            //itera sobre todos os parâmetros enviados na requisição
            foreach (JProperty param in parametrosRequisicao)
            //foreach (KeyValuePair<string, object> param in parametrosRequisicao)
            {
                //verifica se os parâmetros foram preenchidos
                if (param.Value != null )
                {
                    string campo = param.Name.ToString();

                    //itera sobre ParametrosCondicao
                    if (campo == "ParametrosCondicao")
                    {
                        dynamic paramentrosCondicao = (JArray)param.Value;
                        for (int i = 0; i < paramentrosCondicao.Count; i++)
                        {
                            var nome = "";
                            var valor = "";

                            foreach (JProperty paramCondicao in paramentrosCondicao[i])
                            //foreach (KeyValuePair<string, object> paramCondicao in paramentrosCondicao[i])
                            {
                                //captura o nome do parametro
                                if (paramCondicao.Name.ToUpper() == "NOME")
                                {
                                    nome = paramCondicao.Value.ToString();
                                }

                                //captura o valor do parametro
                                if (paramCondicao.Name.ToUpper() == "VALOR")
                                {
                                    valor = paramCondicao.Value == null ? "" : paramCondicao.Value.ToString();
                                }

                            }

                            //add o parametro a lista
                            lista.Add(new KeyValuePair<string, string>(nome, valor));


                        }
                    }


                }
            }

            return lista;
        }
        #endregion

        //Método responsável por retornar todos os campos enviandos em um JSON, no formato de procedure @campo = 'valor'. Com base em um serviço
        //os parametros devem estar dentro um objeto que contenha um lista de objetos
        #region AgruparParametrosData
        public static string AgruparParametrosData(int Servico, dynamic parametrosRequisicao, string obj, DbConnection _connection)
        {
            string data = "";

            //itera sobre todos os parâmetros enviados na requisição
            //foreach (KeyValuePair<string, object> param in parametrosRequisicao)
            foreach (JProperty param in parametrosRequisicao)
            {
                //verifica se os parâmetros foram preenchidos
                if (param.Value != null && param.Value.ToString() != "")
                {
                    string campo = param.Name.ToString();

                    //itera sobre ParametrosCondicao
                    if (campo == obj) //ParametrosData
                    {
                        dynamic paramentrosData = (JArray)param.Value;
                        for (int i = 0; i < paramentrosData.Count; i++)
                        {
                            var campoData = "";
                            var valorData = "";

                            foreach (JProperty paramData in paramentrosData[i])
                            //foreach (KeyValuePair<string, object> paramData in paramentrosData[i])
                            {
                                
                                campoData = paramData.Name.ToString();
                                valorData = paramData.Value.ToString();

                                //lista todos os parâmetros do serviço
                                var servicoParametros = Utils.BuscarServicoParametros(Servico, _connection);

                                //recupera o tipo do campo.
                                //var tipo_dado = (from x in servicoParametros select x).Where(dado => dado.Nome.ToLower() == campoData.ToLower() && dado.Tipo == "E").Select(dado => dado.Tipo_Dado).ToList();
                                var tipo_dado = (from x in servicoParametros select x).Where(dado => dado.Nome.ToLower() == campoData.ToLower()).Select(dado => new { Tipo = dado.Tipo_Dado, dado.Tipo_Entrada }).ToList();

                                if (tipo_dado.Count() > 0)
                                {
                                    
                                    
                                    if (tipo_dado[0].Tipo_Entrada == "CRIPTOGRAFAR")
                                        valorData = Utils.Criptografar(valorData);

                                    if (tipo_dado[0].Tipo_Entrada == "DESCRIPTOGRAFAR")
                                        valorData = Utils.Descriptografar(valorData);

                                    if (tipo_dado[0].Tipo_Entrada == "MD5")
                                        valorData = Utils.GerarHashMd5(valorData);

                                    if (tipo_dado[0].Tipo == "int")
                                    {
                                        valorData = valorData.Length == 0 ? "null" : valorData;
                                        data += data.Length > 1 ? " , " : "";
                                        data += String.Format(" @{0} = {1} ", campoData, valorData);
                                    }

                                    if (tipo_dado[0].Tipo == "string")
                                    {
                                        data += data.Length > 1 ? " , " : "";
                                        data += String.Format(" @{0} = '{1}' ", campoData, valorData);
                                    }
                                }


                            }

                        }
                    }


                }
            }

            return data;
        }
        #endregion


        //Método responsável por retornar todos os campos enviandos em um JSON, no formato de procedure @campo = 'valor'.
        #region AgruparParametrosDataExt
        public static List<dynamic> AgruparParametrosDataExt(dynamic parametrosRequisicao, string obj, string objNomeExibicao, string tipoEntrada, DbConnection _connection)
        {
            List<dynamic> retorno = new List<dynamic>();
            string data = "";

            //itera sobre todos os parâmetros enviados na requisição
            foreach (JProperty param in parametrosRequisicao)
            //foreach (KeyValuePair<string, object> param in parametrosRequisicao)
            {                              

                //verifica se os parâmetros foram preenchidos
                if (param.Value != null && param.Value.ToString() != "")
                {
                    string campo = param.Name.ToString();
                    var typeValuePrincipal = param.Value.Type.ToString().ToLower();
                    //string campoComparacao = objNomeExibicao != "" && objNomeExibicao != null ? objNomeExibicao.ToLower() : obj.ToLower();

                    //itera sobre ParametrosCondicao
                    if (campo.ToLower() == obj.ToLower())
                    //if ((typeValuePrincipal == "string" || typeValuePrincipal == "int") && (campo.ToLower() == obj.ToLower()) )
                    {                        

                        //object ou array
                        //dynamic tipo        = param.Value == null ? "" : param.Value.GetType().BaseType.Name.ToLower();
                        //dynamic typeValue   = param.Value == null ? "" : param.Value.GetType().Name.ToLower();

                        //dynamic typeValue = param.Value == null ? "" : param.Value.GetType().Name.ToLower();

                        var typeValue = param.Value.Type.ToString().ToLower();


                        if (typeValue == "string")
                        {
                            var campoData = "";
                            var valorData = "";

                            //campoData = paramData.Name;
                            campoData = objNomeExibicao != "" && objNomeExibicao != null ? objNomeExibicao : obj;
                            valorData = param.Value.ToString();

                            if (tipoEntrada == "CRIPTOGRAFAR")
                                valorData = Utils.Criptografar(valorData);

                            if (tipoEntrada == "DESCRIPTOGRAFAR")
                                valorData = Utils.Descriptografar(valorData);

                            if (tipoEntrada == "MD5")
                                valorData = Utils.GerarHashMd5(valorData);


                            valorData = valorData == null || valorData == "" ? "" : valorData;
                            data += data.Length > 1 ? " , " : "";
                            //data += String.Format(" @{0} = '{1}' ", campoData, valorData);
                            data += String.Format(" @{0} = '{1}' ", campoData, valorData);

                            retorno.Add(data);

                        }


                        if (typeValue == "object")
                        {

                            dynamic paramentrosData = param.Value;

                            foreach (JProperty paramData in paramentrosData)
                            //foreach (KeyValuePair<string, object> paramData in paramentrosData)
                            {
                                dynamic typeValue2 = paramData.Value.Type.ToString().ToLower();

                                if (typeValue2 == "string")
                                {
                                    var campoData = "";
                                    var valorData = "";

                                    campoData = paramData.Name;
                                    valorData = paramData.Value.ToString();

                                    if (tipoEntrada == "CRIPTOGRAFAR")
                                        valorData = Utils.Criptografar(valorData);

                                    if (tipoEntrada == "DESCRIPTOGRAFAR")
                                        valorData = Utils.Descriptografar(valorData);

                                    if (tipoEntrada == "MD5")
                                        valorData = Utils.GerarHashMd5(valorData);

                                    valorData = valorData == null || valorData == "" ? "" : valorData;
                                    data += data.Length > 1 ? " , " : "";
                                    //data += String.Format(" @{0} = '{1}' ", campoData, valorData);
                                    data += String.Format(" @{0}{1} = '{2}' ", obj, campoData, valorData);


                                    //retorno.Add(data);

                                }
                                else if (typeValue2 == "integer")
                                {
                                    var campoData = "";
                                    var valorData = "";

                                    campoData = paramData.Name;
                                    valorData = paramData.Value.ToString();

                                    if (tipoEntrada == "CRIPTOGRAFAR")
                                        valorData = Utils.Criptografar(valorData);

                                    if (tipoEntrada == "DESCRIPTOGRAFAR")
                                        valorData = Utils.Descriptografar(valorData);

                                    if (tipoEntrada == "MD5")
                                        valorData = Utils.GerarHashMd5(valorData);

                                    valorData = valorData == null || valorData == "" ? "" : valorData;
                                    data += data.Length > 1 ? " , " : "";
                                    //data += String.Format(" @{0} = '{1}' ", campoData, valorData);
                                    data += String.Format(" @{0}{1} = '{2}' ", obj, campoData, valorData);

                                    //retorno.Add(data);

                                }


                            }

                            retorno.Add(data);

                        }



                    }
                    else if (typeValuePrincipal == "object") 
                    {

                        dynamic paramentrosData = param.Value;

                        foreach (JProperty paramData in paramentrosData)
                        //foreach (KeyValuePair<string, object> paramData in paramentrosData)
                        {

                            string campoFilho = paramData.Name.ToString();
                            dynamic typeValue2 = paramData.Value.Type.ToString().ToLower();

                            if ((campo+campoFilho).ToLower() == obj.ToLower())
                            {

                                if (typeValue2 == "string")
                                {
                                    var campoData = "";
                                    var valorData = "";

                                    //campoData = paramData.Name;
                                    campoData = objNomeExibicao != "" && objNomeExibicao != null ? objNomeExibicao : obj;
                                    valorData = paramData.Value.ToString();

                                    if (tipoEntrada == "CRIPTOGRAFAR")
                                        valorData = Utils.Criptografar(valorData);

                                    if (tipoEntrada == "DESCRIPTOGRAFAR")
                                        valorData = Utils.Descriptografar(valorData);

                                    if (tipoEntrada == "MD5")
                                        valorData = Utils.GerarHashMd5(valorData);

                                    valorData = valorData == null || valorData == "" ? "" : valorData;
                                    data += data.Length > 1 ? " , " : "";
                                    data += String.Format(" @{0} = '{1}' ", campoData, valorData);


                                    //retorno.Add(data);

                                }

                                else if (typeValue2 == "integer")
                                {
                                    var campoData = "";
                                    var valorData = "";

                                    //campoData = paramData.Name;
                                    campoData = objNomeExibicao != "" && objNomeExibicao != null ? objNomeExibicao : obj;
                                    valorData = paramData.Value.ToString();

                                    if (tipoEntrada == "CRIPTOGRAFAR")
                                        valorData = Utils.Criptografar(valorData);

                                    if (tipoEntrada == "DESCRIPTOGRAFAR")
                                        valorData = Utils.Descriptografar(valorData);

                                    if (tipoEntrada == "MD5")
                                        valorData = Utils.GerarHashMd5(valorData);

                                    valorData = valorData == null || valorData == "" ? "" : valorData;
                                    data += data.Length > 1 ? " , " : "";
                                    data += String.Format(" @{0} = '{1}' ", campoData, valorData);

                                }

                                retorno.Add(data); //02. mudei pra ca, para ver se resolve a duplicação dos campos do 01.
                            }

                            

                        }

                        //retorno.Add(data); 01. estava aqui. estava dulicando os campos
                       
                    }


                }
                               

            }

            return retorno;
        }
        #endregion
        
        //Método responsável por verificar se os parâmetros obrigatórios foram preenchidos.
        #region VerificarServicoParametrosObrigatorios
        public string VerificarServicoParametrosObrigatorios(int Servico, dynamic parametrosRequisicao, string StrConexao)
        {
            bool retorno = false;
            string camposNaoInformados = "";

            try
            {
                using (_connection = Conexao.getConnection(StrConexao))
                {
                    //_connection.Open(); 

                    string strSql = WS.BuscarServicoParametro;

                    //retorna todos os parâmetros do serviço 
                    var servicoParametros = _connection.Query(strSql, new { Servico = Servico }).ToList();//.AsList();
                    //retorna somente o nome dos parâmetros OBRIGATÓRIOS e do TIPo ENTRADA do serviço (linguagem LINQ)
                    var servicoParametrosObrigatorio = (from x in servicoParametros select x).Where(dado => dado.Obrigatorio == "S" && dado.Tipo == "E").Select(dado => dado.Nome).ToList();

                    var parametrosRequisicaoInformados = new List<string>();
                    //itera sobre todos os parâmetros enviados na requisição
                    foreach (JProperty param in parametrosRequisicao)
                    //foreach (KeyValuePair<string, object> param in parametrosRequisicao)
                    {
                        //verifica se os parâmetros foram preenchidos
                        if (param.Value != null)
                        {
                            string campo = param.Name.ToString();
                            //não inclui as tag "especiais"
                            if (campo != "ParametrosData" && campo != "ParametrosCondicao") {
                                parametrosRequisicaoInformados.Add(campo);
                            }

                            //itera sobre tag JSON do Form: Data
                            if (campo == "ParametrosData")
                            {
                                //dynamic paramentrosData = (Array)param.Value;
                                dynamic paramentrosData = (JArray)param.Value;
                                for (int i = 0; i < paramentrosData.Count; i++)
                                {

                                    foreach (JProperty paramData in paramentrosData[i])
                                    //foreach (KeyValuePair<string, object> paramData in paramentrosData[i])
                                    {
                                        string campoData = paramData.Name.ToString();

                                        //verifica se os parâmetros foram preenchidos
                                        if (paramData.Value != null)
                                        {
                                            parametrosRequisicaoInformados.Add(campoData);
                                        }
                                    }
                                }
                            }

                            //itera sobre tag JSON: ParametrosCondicao
                            if (campo == "ParametrosCondicao")
                            {
                                dynamic paramentrosCondicao = (JArray)param.Value;
                                for (int i = 0; i < paramentrosCondicao.Count; i++)
                                {

                                    foreach (JProperty paramCondicao in paramentrosCondicao[i])
                                    //foreach (KeyValuePair<string, object> paramCondicao in paramentrosCondicao[i])
                                    {
                                        string campoCondicao = paramCondicao.Name.ToString();

                                        //verifica se os parâmetros foram preenchidos
                                        if ((campoCondicao != "Tipo" && campoCondicao != "Valor") && paramCondicao.Value != null && paramCondicao.Value.ToString() != "")
                                        {
                                            parametrosRequisicaoInformados.Add(campoCondicao);
                                        }
                                    }
                                }
                            }


                        }
                    }

                    //Verifica quais parâmetros obrigatórios foram preenchidos (compara a Lista de servicoParametrosObrigatorio com ParametrosServicoRequisicao)
                    var parametrosInformados = from obrigatirios in servicoParametrosObrigatorio
                                               where parametrosRequisicaoInformados.Contains(obrigatirios.ToString())
                                               select obrigatirios;


                    var parametrosNAOInformados = from obrigatirios in servicoParametrosObrigatorio
                                                  where !parametrosRequisicaoInformados.Contains(obrigatirios.ToString())
                                                  select obrigatirios;

                    if (servicoParametrosObrigatorio.Count() != parametrosInformados.Count())
                    {
                        retorno = false;

                        foreach (var campo in parametrosNAOInformados)
                        {
                            camposNaoInformados += camposNaoInformados.Length > 1 ? " , " : "";
                            camposNaoInformados = campo;
                        }


                    }

                    //itera sobre o Array, procurando o value do key informado no parâmetro Campo


                    /*
                    var parametroRequisicaoROOT = new List<KeyValuePair<string, string>>();
                    var parametroRequisicaoCondicao = new List<KeyValuePair<string, string>>();
                    dynamic paramentrosCondicao;
                    //itera sobre todos os parâmetros enviados na requisição
                    foreach (KeyValuePair<string, object> param in ParametrosServicoRequisicao)
                    {
                        //verifica se os parâmetros foram preenchidos
                        if (param.Value != null && param.Value != "")
                        {
                            string campo = param.Key.ToString();
                            string valor = param.Value.ToString();
                            parametroRequisicaoROOT.Add(new KeyValuePair<string, string>(campo, valor));

                            //itera sobre ParametrosCondicao
                            if (campo == "ParametrosCondicao")
                            {
                                paramentrosCondicao = (Array)param.Value;
                                for (int i = 0; i < paramentrosCondicao.Length; i++)
                                {
 
                                    foreach (KeyValuePair<string, object> paramCondicao in paramentrosCondicao[i])
                                    {
                                        //verifica se os parâmetros foram preenchidos
                                        if (paramCondicao.Value != null && paramCondicao.Value != "")
                                        {
                                            string campoCondicao = paramCondicao.Key.ToString();
                                            string valorCondicao = paramCondicao.Value.ToString();
                                            parametroRequisicaoCondicao.Add(new KeyValuePair<string, string>(campoCondicao, valorCondicao));
                                        }
                                    }
                                }
                            }

                            
                        }
                    }
                    */




                    return camposNaoInformados;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(cod:VerificarServicoParametrosObrigatorios)");
            }

        }
        #endregion

        //Método responsável por listar os parâmetros de um servico específico
        #region BuscarServicoParametros
        public static List<dynamic> BuscarServicoParametros(dynamic Servico, DbConnection _connection, IDbTransaction transaction = null)
        {
            dynamic retorno = new List<dynamic>();

            try
            {
                //using (_connection)
                //{
                    //_connection.Open();

                    string strSql = WS.BuscarServicoParametro;

                    //retorna todos os parâmetros do serviço 
                    retorno = _connection.Query(strSql, new { Servico = Servico }, transaction: transaction).ToList();//.AsList();
 
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(BuscarServicoParametros cod(" + Servico + "):Util)");
            }
            finally
            {
                //_connection.Close();                
            }

            return retorno;

        }
        #endregion

        //Método responsável por listar os parâmetros de um servico específico
        #region BuscarServicoParametroChave
        public static dynamic BuscarServicoParametroChave(int Servico, DbConnection _connection)
        {
            //string filedPKOrigem = "";

            try
            {

                var servicoParametrosOrigem = Utils.BuscarServicoParametros(Servico, _connection);
                //dynamic filedPKOrigem = (from x in servicoParametrosOrigem select x).Where(dado => dado.Chave == "PK").Select(dado => new { dado.Nome }).Take(1).ToList();
                //return filedPKOrigem.Count > 0 ? filedPKOrigem[0].Nome : "";
                dynamic filedPKOrigem = (from x in servicoParametrosOrigem select x).Where(dado => dado.Chave == "PK" || dado.ParametroTransicao=="S").Select(dado => new { dado.Nome, dado.Tipo_Dado }).ToList();
                //dynamic filedPKOrigem = (from x in servicoParametrosOrigem select x).Select(dado => new { dado.Nome }).ToList();
                return filedPKOrigem.Count > 0 ? filedPKOrigem : "";

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(cod:BuscarServicoParametros)");
            }

        }
        #endregion

        //Método responsável por extrair o value de um campo específico, baseado no retorno de uma consulta SQL no Dapper
        #region ExtrairValueIDictionary
        public static string ExtrairValueIDictionary(dynamic Data, string Campo )
        {
            string valor = "";

            foreach (IDictionary<string, Object> row in Data)
            {
                
                foreach (var pair in row)
                {
                    string rsCampo = pair.Key;

                    if (rsCampo.ToLower() == Campo.ToLower())
                    {
                        valor = pair.Value == null ? "" : pair.Value.ToString();
                        return valor;
                        break;
                    }                  
                    
                }
            }


            return valor;
        }
        #endregion ExtrairValueSQL

        //Método responsável por extrair uma lista de valores de um campo específico
        #region ExtrairListaIDictionary
        public static List<dynamic> ExtrairListaIDictionary(dynamic Data, string Campo)
        {
            dynamic valor = null;
            List<dynamic> retorno = new List<dynamic>();

            foreach (IDictionary<string, Object> row in Data)
            {

                foreach (var pair in row)
                {
                    string rsCampo = pair.Key;

                    if (rsCampo.ToLower() == Campo.ToLower())
                    {
                        valor = pair.Value == null ? "" : pair.Value;
                        retorno.Add(valor);
                        return retorno;
                        break;
                    }

                }
            }


            return retorno;
        }
        #endregion ExtrairListaIDictionary


        //Método responsável por extrair uma lista de valores de um campo específico
        #region ExtrairListaKeyValuePair
        public static List<dynamic> ExtrairListaKeyValuePair(dynamic Data, string Campo)
        {
            dynamic valor = null;
            List<dynamic> retorno = new List<dynamic>();

            foreach (KeyValuePair<string, Object> row in Data)
            {

                string rsCampo = row.Key;

                if (rsCampo.ToLower() == Campo.ToLower())
                {
                    valor = row.Value == null ? "" : row.Value;
                    retorno.Add(valor);
                    return retorno;
                }

            }


            return retorno;
        }
        #endregion ExtrairListaKeyValuePair

        //Método responsável por extrair uma lista de valores de um campo específico
        #region ExtrairListaJsonConvert
        public static List<dynamic> ExtrairListaJsonConvert(dynamic Data, string Campo)
        {
            dynamic valor = null;
            List<dynamic> retorno = new List<dynamic>();

            foreach (JProperty row in Data)
            //foreach (KeyValuePair<string, Object> row in Data)
            {

                string rsCampo = row.Name;

                if (rsCampo.ToLower() == Campo.ToLower())
                {
                    valor = row.Value == null ? "" : row.Value;
                    retorno.Add(valor);
                    return retorno;
                }

            }


            return retorno;
        }
        #endregion ExtrairListaJsonConvert


        //Método responsável por extrair uma lista de valores de um campo específico
        #region ExtrairListaJObject
        public static List<dynamic> ExtrairListaJObject(dynamic Data, string Campo)
        {
            dynamic valor = null;
            List<dynamic> retorno = new List<dynamic>();

            foreach (JObject row in Data)
            {

                foreach(KeyValuePair<string, JToken> rowKey in row)
                {

                    string rsCampo = rowKey.Key;

                    if (rsCampo.ToLower() == Campo.ToLower())
                    {
                        valor = rowKey.Value == null ? "" : rowKey.Value.ToString();
                        retorno.Add(valor);
                        return retorno;
                    }

                }

            }


            return retorno;
        }
        #endregion ExtrairListaJObject

        //Método responsável por extrair uma lista de valores de um campo específico
        #region ExtrairListaIDictionary
        public static List<dynamic> ExtrairLista(dynamic Data, string Campo)
        {
            dynamic valor = null;
            List<dynamic> retorno = new List<dynamic>();

            foreach (KeyValuePair<string, Object> row in Data)
            {

                string rsCampo = row.Key;

                if (rsCampo.ToLower() == Campo.ToLower())
                {
                    valor = row.Value == null ? "" : row.Value;
                    retorno.Add(valor);
                    return retorno;
                }

            }


            return retorno;
        }
        #endregion ExtrairListaIDictionary


        //Método responsável por extrair o value de um campo específico, baseado no retorno de uma consulta SQL no Dapper (quando este utiliza foreach (IDictionary<string, object> rowData in Data))
        #region ExtrairValueJProperty
        public static string ExtrairValueJProperty(dynamic Data, string Campo)
        {
            string valor = "";

            foreach (JProperty row in Data)
            {

                string rsCampo = row.Name.ToString();
                var typeValue = row.Value.Type.ToString();

                //se vier dentro de um objeto
                if (typeValue.ToLower() == "object")
                {
                    var campoData = "";
                    var valorData = "";

                    dynamic paramentrosData = row.Value;

                    foreach (JProperty paramData in paramentrosData)
                    {
                        //dynamic typeValue2 = paramData.Value == null ? "" : paramData.Value.GetType().Name.ToLower();

                        //if (typeValue2 != "object[]")
                        //{
                        campoData = paramData.Name.ToString();
                        valorData = paramData.Value == null ? "" : paramData.Value.ToString();

                        if (campoData.ToLower() == Campo.ToLower())
                        {
                            return valorData;
                        }

                        // }

                    }



                }
                else
                {

                    if (rsCampo.ToLower() == Campo.ToLower())
                    {
                        valor = row.Value == null ? "" : row.Value.ToString();
                        return valor;
                    }

                }

            }


            return valor;
        }
        #endregion

        //Método responsável por extrair o value de um campo específico, baseado no retorno de uma consulta SQL no Dapper (quando este utiliza foreach (IDictionary<string, object> rowData in Data))
        #region ExtrairValueJProperty
        public static string ExtrairValueJObject(dynamic Data, string Campo)
        {
            string valor = "";

            foreach (var rowJObject in Data)
            {

                foreach (JProperty row in rowJObject)
                {

                    string rsCampo = row.Name.ToString();
                    var typeValue = row.Value.Type.ToString();

                    //se vier dentro de um objeto
                    if (typeValue.ToLower() == "object")
                    {
                        var campoData = "";
                        var valorData = "";

                        dynamic paramentrosData = row.Value;

                        foreach (JProperty paramData in paramentrosData)
                        {
                            //dynamic typeValue2 = paramData.Value == null ? "" : paramData.Value.GetType().Name.ToLower();

                            //if (typeValue2 != "object[]")
                            //{
                            campoData = paramData.Name.ToString();
                            valorData = paramData.Value == null ? "" : paramData.Value.ToString();

                            if (campoData.ToLower() == Campo.ToLower())
                            {
                                return valorData;
                            }

                            // }

                        }



                    }
                    else
                    {

                        if (rsCampo.ToLower() == Campo.ToLower())
                        {
                            valor = row.Value == null ? "" : row.Value.ToString();
                            return valor;
                        }

                    }

                }
            }

            return valor;
        }
        #endregion 


        //Método responsável por extrair o value de um campo específico, baseado no retorno de uma consulta SQL no Dapper (quando este utiliza foreach (IDictionary<string, object> rowData in Data))
        #region ExtrairValueKeyValuePair
        public static string ExtrairValueKeyValuePair(dynamic Data, string Campo)
        {
            string valor = "";

            foreach (KeyValuePair<string, Object> row in Data)
            {

                string rsCampo = row.Key;
                dynamic typeValue = row.Value == null ? "" : row.Value.GetType().Name.ToLower();

                //se vier dentro de um objeto
                if (typeValue == "dictionary`2")
                {
                    var campoData = "";
                    var valorData = "";

                    dynamic paramentrosData = row.Value;

                    foreach (KeyValuePair<string, object> paramData in paramentrosData)
                    {
                        //dynamic typeValue2 = paramData.Value == null ? "" : paramData.Value.GetType().Name.ToLower();

                        //if (typeValue2 != "object[]")
                        //{
                            campoData = paramData.Key.ToString();
                            valorData = paramData.Value == null ? "" : paramData.Value.ToString();

                            if (campoData.ToLower() == Campo.ToLower())
                            {
                                return valorData;
                            }

                       // }

                    }

                    

                }
                else
                {

                    if (rsCampo.ToLower() == Campo.ToLower())
                    {
                        valor = row.Value == null ? "" : row.Value.ToString();
                        return valor;
                    }

                }
                
            }


            return valor;
        }
        #endregion 


        //Método responsável por retornar um array de um objeto
        #region ExtrairArrayJProperty
        public static List<dynamic> ExtrairArrayJProperty(dynamic parametrosRequisicao, string obj)
        {
            List<dynamic> retorno = new List<dynamic>();


            //itera sobre todos os parâmetros enviados na requisição
            foreach (JProperty param in parametrosRequisicao)
            //foreach (KeyValuePair<string, object> param in parametrosRequisicao)
            {

                //verifica se os parâmetros foram preenchidos
                if (param.Value != null && param.Value.ToString() != "")
                {
                    string campo = param.Name.ToString();

                    dynamic tipo1 = param.Value.GetType().BaseType.Name;
                    //dynamic typeValue = param.Value == null ? "" : param.Value.GetType().Name.ToLower();
                    var typeValue = param.Value.Type.ToString();

                    if (campo.ToLower() == "beneficiariosanexo")
                    {
                        var sim = "";
                    }

                    //quando achar o objeto pesquisado
                    if (campo.ToLower() == obj.ToLower()) //ParametrosData
                    {
                        /*
                        //object ou array
                        dynamic tipo = param.Value.GetType().BaseType.Name;

                        if (tipo.ToLower() == "array")
                        {
                            var campoData = "";
                            var valorData = "";

                            dynamic paramentrosData = (Array)param.Value;
                            for (int i = 0; i < paramentrosData.Length; i++)
                            {
                                //retorno.Add(paramentrosData[i]);
                            }
                        }*/

                        retorno.Add(param);
                        return retorno;

                    } //ATENÇÃO: pesquisa recursiva
                    //pesquisa também dentro de subobjetos
                    else if (typeValue == "Object") // else if (typeValue == "dictionary`2" || typeValue == "object[]")
                    {
                        dynamic ret = ExtrairArrayJProperty(param.Value, obj);

                        if (ret.Count > 0)
                        {
                            foreach (var item in ret)
                            {
                                retorno.Add(item);
                            }
                        }

                    }
                }
            }

            return retorno;
        }
        #endregion



        //Método responsável por retornar um array de um objeto
        #region ExtrairArrayKeyValuePair
        public static List<dynamic> ExtrairArrayKeyValuePair(dynamic parametrosRequisicao, string obj)
        {
            List<dynamic> retorno = new List<dynamic>();


            //itera sobre todos os parâmetros enviados na requisição
            foreach (KeyValuePair<string, object> param in parametrosRequisicao)
            {

                //verifica se os parâmetros foram preenchidos
                if (param.Value != null && param.Value != "")
                {
                    string campo = param.Key.ToString();

                    dynamic tipo1 = param.Value.GetType().BaseType.Name;
                    dynamic typeValue = param.Value == null ? "" : param.Value.GetType().Name.ToLower();


                    if (campo.ToLower() == "beneficiariosanexo")
                    {
                        var sim = "";
                    }

                    //quando achar o objeto pesquisado
                    if (campo.ToLower() == obj.ToLower()) //ParametrosData
                    {
                        /*
                        //object ou array
                        dynamic tipo = param.Value.GetType().BaseType.Name;

                        if (tipo.ToLower() == "array")
                        {
                            var campoData = "";
                            var valorData = "";

                            dynamic paramentrosData = (Array)param.Value;
                            for (int i = 0; i < paramentrosData.Length; i++)
                            {
                                //retorno.Add(paramentrosData[i]);
                            }
                        }*/

                        retorno.Add(param);
                        return retorno;

                    } //ATENÇÃO: pesquisa recursiva
                    //pesquisa também dentro de subobjetos
                    else if (typeValue == "dictionary`2") // else if (typeValue == "dictionary`2" || typeValue == "object[]")
                    {
                        dynamic ret = ExtrairArrayKeyValuePair(param.Value, obj);

                        if (ret.Count > 0)
                        {
                            foreach (var item in ret)
                            {
                                retorno.Add(item);
                            }
                        }

                    }
                }
            }

            return retorno;
        }
        #endregion

        //Método responsável por extrair o value de um campo específico, baseado em um JSON
        #region ExtrairValueJSON
        public static string ExtrairValueJSON(dynamic Data, string Campo)
        {
            string valor = "";


            //IDictionary IDictionaryJSON = new Dictionary<string, string>();
            var dicionario = new Dictionary<string, string>();

            var campoData = "";
            var valorData = "";

            //itera sobre todos os parâmetros enviados na requisição

            foreach (JProperty param in Data)
            //foreach (KeyValuePair<string, object> param in Data)
            {
                //verifica se os parâmetros foram preenchidos
                if (param.Value != null)
                {
                    string campo = param.Name.ToString();
                    //var typeValue = param.Value.GetType().Name;
                    var typeValue = param.Value.Type.ToString();

                    if (typeValue.ToLower() == "string" || typeValue.ToLower() == "int" || typeValue.ToLower() == "integer") { 

                        campoData = param.Name.ToString();
                        valorData = param.Value.ToString();
                        //IDictionaryJSON

                        //não add duas chaves iguais. As vezes duas chaves são enviadas iguais em objetos distintos
                        if (!dicionario.ContainsKey(campoData))
                        {
                            dicionario.Add(campoData, valorData);
                        }


                        //valor += String.Format(" {0} = '{1}' ", campoData, valorData);

                    }
                    else if (typeValue == "Array")
                    {
                        dynamic paramentrosData = (JArray)param.Value;
                        for (int i = 0; i < paramentrosData.Count; i++)
                        {

                            foreach (JProperty paramData in paramentrosData[i])
                            //foreach (KeyValuePair<string, object> paramData in paramentrosData[i])
                            {
                                valor += valor.Length > 1 ? " , " : "";

                                campoData = paramData.Name.ToString();
                                valorData = paramData.Value.ToString();

                                //não add duas chaves iguais. As vezes duas chaves são enviadas iguais em objetos distintos
                                if (!dicionario.ContainsKey(campoData))
                                {
                                    dicionario.Add(campoData, valorData);
                                }
                                
                            }



                        }

                        /*
                        } else if (typeValue == "Object[]")
                        {
                            dynamic paramentrosData = (Array)param.Value;
                            for (int i = 0; i < paramentrosData.Length; i++)
                            {

                                foreach (KeyValuePair<string, object> paramData in paramentrosData[i])
                                {
                                    valor += valor.Length > 1 ? " , " : "";

                                    campoData = paramData.Key.ToString();
                                    valorData = paramData.Value.ToString();

                                    //não add duas chaves iguais. As vezes duas chaves são enviadas iguais em objetos distintos
                                    if (!dicionario.ContainsKey(campoData))
                                    {
                                        dicionario.Add(campoData, valorData);
                                    }


                                }



                            }*/
                        
                    }
                    /* INCLUÍDO EM 01/07/2019 [ WENDEL FREITAS ] */
                    else if (typeValue == "Object")
                    {
                        dynamic paramentrosData = param.Value;

                        foreach (JProperty paramData in paramentrosData)
                        //foreach (KeyValuePair<string, object> paramData in paramentrosData)
                        {
                            dynamic typeValue2 = paramData.Value.Type.ToString().ToLower();

                            if (typeValue2 == "string")
                            {
                                campoData = paramData.Name;
                                valorData = paramData.Value.ToString();

                                //não add duas chaves iguais. As vezes duas chaves são enviadas iguais em objetos distintos
                                if (!dicionario.ContainsKey(campoData))
                                {
                                    dicionario.Add(campoData, valorData);
                                }

                            }
                            else if (typeValue2 == "integer")
                            {
                                campoData = paramData.Name;
                                valorData = paramData.Value.ToString();

                                //não add duas chaves iguais. As vezes duas chaves são enviadas iguais em objetos distintos
                                if (!dicionario.ContainsKey(campoData))
                                {
                                    dicionario.Add(campoData, valorData);
                                }

                            }
                        }
                    }



                }
            }


            //itera sobre o Array, procurando o value do key informado no parâmetro Campo
            foreach (var pair in dicionario)
            {
                string key = pair.Key;

                if (key.ToLower() == Campo.ToLower())
                {
                    valor = pair.Value.ToString();
                    return valor;
                    break;
                }

            }




            return valor;
        }
        #endregion

        //Método responsável por extrair um array de valores de um json e depois procurar o valor o valor de cada item do array em um lista retornada pelo banco de dados
        #region MontaValueJson
        public static List<dynamic> MontaValueJson(dynamic json, string campo, dynamic values)
        {
            dynamic valor = null;

            List<dynamic> retorno = new List<dynamic>();
            var dicionario = new Dictionary<string, object>();
            var dicionario2 = new KeyValuePair<string, object>();

            List<KeyValuePair<string, object>> lista = new List<KeyValuePair<string, object>>();

            foreach (IDictionary<string, Object> row in json)
            {

                foreach (var pair in row)
                {
                    string rsCampo = pair.Key;

                    if (rsCampo.ToLower() == campo.ToLower())
                    {

                        dynamic typeValue = pair.Value == null ? "" : pair.Value.GetType().Name.ToLower();

                        valor = pair.Value == null ? "" : pair.Value;

                        if (typeValue.ToLower() == "object[]")
                        {
                            dynamic paramentrosData = (Array)valor;
                            for (int i = 0; i < valor.Length; i++)
                            {
                                var campoNome = valor[i];
                                var campoValor = Utils.ExtrairValueIDictionary(values, campoNome);
                                //dicionario.Add(campoNome, campoValor);
                                //lista.Add(new KeyValuePair<string, object>(campoNome, campoValor));


                            }
                        }
                        //retorno.Add(lista);

                        retorno.Add(dicionario);
                        //return retorno;
                    }

                }
            }


            return retorno;
        }
        #endregion MontaValueJson

        //Método responsável trabsformar um objeto em ExpandoObject
        #region ToExpandoObject
        /*
         * ExpandoObject é um objeto que pode ter membros adicionados ou removidos dinâmicamente. Com ele é possível definirmos métodos e propriedades em tempo de execução.
           Talvez muitos de nós não vejamos vantagens neste tipo de feature logo de início, mas para muitos será uma feature muito interessante e que proporcionará um grande avanço.
           Read more: http://www.linhadecodigo.com.br/artigo/2990/expandoobject-dinamismo-no-net-40.aspx#ixzz5SnnIAFdk
         */
        public static dynamic ToExpandoObject(object value)
        {
            IDictionary<string, object> dapperRowProperties = value as IDictionary<string, object>;

            IDictionary<string, object> expando = new ExpandoObject();

            foreach (KeyValuePair<string, object> property in dapperRowProperties)
                expando.Add(property.Key, property.Value);

            return expando as ExpandoObject;
        }
        #endregion

        //Método que recuperar um arquivo de uma URL
        #region GetFileUrl
        private static byte[] GetFileUrl(string url)
        {
            Stream stream = null;
            byte[] buf;

            try
            {
                WebProxy myProxy = new WebProxy();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                stream = response.GetResponseStream();

                using (BinaryReader br = new BinaryReader(stream))
                {
                    int len = (int)(response.ContentLength);
                    buf = br.ReadBytes(len);
                    br.Close();
                }

                stream.Close();
                response.Close();
            }
            catch (Exception exp)
            {
                buf = null;
            }

            return (buf);
        }
        #endregion

        //Método que converte o byte retornado de uma url para base64
        #region ConvertFileURLToBase64
        public static String ConvertFileURLToBase64(String url)
        {
            StringBuilder _sb = new StringBuilder();
            string retorno = "";

            Byte[] _byte = GetFileUrl(url);

            if (_byte != null)
            {
                retorno = _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length)).ToString();
            }
            

            return retorno;
        }
        #endregion

        /*
        //Método que converte o byte retornado do filesystem para base64
        #region ConvertFileToBase64
        public static String ConvertFileToBase64(String path)
        {
            
            byte[] _byte = null;
            StringBuilder _sb = new StringBuilder();
            string retorno = "";

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(path).Length;
            _byte = br.ReadBytes((int)numBytes);            

            if (_byte != null)
            {
                //pega o mime do arquivo
                string mimeType = MimeMapping.GetMimeMapping(path);
                //acrescenta no final do objeto String
                _sb.AppendFormat("data:{0};base64,", mimeType);
                retorno = _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length)).ToString();
            }

            return retorno;


        }
        #endregion
        */

        //Método que converte arquivo em Base64 para file
        #region SaveBase64ToFile
        public static String SaveBase64ToFile(String path, String base64)
        {
            try
            {

                var file = new FileInfo(path);

                file.Directory.Create(); //If the directory already exists, this method does nothing.

                Byte[] bytes = Convert.FromBase64String(base64);
                File.WriteAllBytes(@path, bytes);

                return "sucesso";

            } catch (Exception ex)
            {
                return "error: "+ ex;
            }
            
        }
        #endregion


        /*
        //Método responsável por validar um edereço de email
        public static bool ValidaEnderecoEmail(string enderecoEmail)
        {
            try
            {
                //define a expressão regulara para validar o email
                string texto_Validar = enderecoEmail;
                Regex expressaoRegex = new Regex(@"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}");

                // testa o email com a expressão
                if (expressaoRegex.IsMatch(texto_Validar))
                {
                    // o email é valido
                    return true;
                }
                else
                {
                    // o email é inválido
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        */


        /*
        public object SendMail(string destinatario, string assunto, string mensagem, dynamic Server, List<dynamic> anexos, int emailServidor, int emailLayout, int servico, int usuario, string origem, string strConexao)
        {

            object retorno = new object();
            string status = "SUCESSO";
            string statusMensageem = "";

            try
            {

                string  remetente   = Server.Remetente;
                string  host        = Server.Servidor;
                int     port        = Server.Porta != null && Server.Porta != "" ? Convert.ToInt32(Server.Porta) : 0;
                string  sec         = Server.Seguranca != null && Server.Seguranca != "" ? Server.Seguranca.ToLower().Trim() : "";
                string  user        = Server.Usuario;
                string  pwd         = Server.Senha;

                // valida o email
                bool bValidaEmail = ValidaEnderecoEmail(destinatario);

                if (bValidaEmail == false)
                    return "Email do destinatário inválido:" + destinatario;

                // Cria uma mensagem
                MailMessage mensagemEmail = new MailMessage(remetente, destinatario, assunto, mensagem);
                // Obtem os anexos contidos em um arquivo arraylist e inclui na mensagem
                if (anexos != null && anexos.Count > 0) {
                    foreach (var anexo in anexos)
                    {
                        //Attachment anexado = new Attachment(anexo, MediaTypeNames.Application.Octet);
                        //converte Base64 para memoryStream
                        var memoryStream = new MemoryStream(Convert.FromBase64String(anexo.Base64));
                        //converte memoryStream para byte
                        //byte[] bytes = memoryStream.ToArray();
                        Attachment anexado = new Attachment(memoryStream, anexo.Name);//, "iTextSharpPDF.pdf");
                        mensagemEmail.Attachments.Add(anexado);

                    }
                }

                //padrão HTML
                mensagemEmail.IsBodyHtml = true; // This line


                SmtpClient client = new SmtpClient(host, port);
                if (sec == "ssl")
                {
                    client.EnableSsl = true;
                }
                NetworkCredential cred = new NetworkCredential(user, pwd);
                client.Credentials = cred;

                // Inclui as credenciais
                if (sec == "ssl")
                {
                    client.UseDefaultCredentials = true;
                }
                
                // envia a mensagem
                client.Send(mensagemEmail);

                retorno = new
                {
                    OutErro = 0,
                    OutMensagem = "Mensagem enviada para " + destinatario + " às " + DateTime.Now.ToString() + "."
                };

            }
            catch (Exception ex)
            {
                //string erro     = ex.InnerException.ToString();
                string erro = ex.Message.ToString();// + erro;

                retorno = new
                {
                    OutErro = 1,
                    OutMensagem = erro
                };

                status = "ERROR";
                statusMensageem = erro;

            }
            finally
            {
                Dao Dao = new Dao();
                //Dao.EmailLog(emailServidor, emailLayout, servico, usuario, destinatario, assunto, mensagem, status, statusMensageem, origem, strConexao);
            }

            return retorno;
        }
        */

        //Geração de PDF via wkhtmltopdf 
        public static string HtmlToPdf(string pdfOutputLocation, string outputFilename, string[] urls, string options = null, string pdfHtmlToPdfExePath = null)
        {
            string urlsSeparatedBySpaces = string.Empty;
            object retorno = new object();
            try
            {
                //Determine inputs
                if ((urls == null) || (urls.Length == 0))
                    throw new Exception("No input URLs provided for HtmlToPdf");
                else
                    urlsSeparatedBySpaces = String.Join(" ", urls); //Concatenate URLs

                //outputFilename = "carta_portabilidade_20190610-063518_034.html";
                //pdfHtmlToPdfExePath = "wkhtmltopdf.exe";

                //string outputFolder = pdfOutputLocation;
                //string outputFilename = outputFilenamePrefix + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff") + ".pdf"; // assemble destination PDF file name
                var p = new System.Diagnostics.Process()
                {
                    StartInfo =
                    {
                        FileName = pdfHtmlToPdfExePath,
                        Arguments = ((options == null) ? "" : String.Join(" ", options)) + " " + urlsSeparatedBySpaces + " " + outputFilename,
                        UseShellExecute = false, // needs to be false in order to redirect output
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true, // redirect all 3, as it should be all 3 or none
                        WorkingDirectory = pdfOutputLocation
                    }
                };

                p.Start();

                // read the output here...
                //var output = p.StandardOutput.ReadToEnd();
                var errorOutput = p.StandardError.ReadToEnd();
                //SaveStream(p.StandardOutput.BaseStream, pdfOutputLocation);

                // ...then wait n milliseconds for exit (as after exit, it can't read the output)
                p.WaitForExit(60000);

                // read the exit code, close process
                int returnCode = p.ExitCode;
                p.Close();

                // if 0 or 2, it worked so return path of pdf
                if ((returnCode == 0) || (returnCode == 2))
                    return pdfOutputLocation + outputFilename;
                else
                    throw new Exception(errorOutput);
            }
            catch (Exception exc)
            {
                 throw new Exception("Problem generating PDF from HTML, URLs: " + urlsSeparatedBySpaces + ", outputFilename: " + outputFilename + "Message: "+ exc.Message, exc);

            }
        }




        /*
        public static MemoryStream GeneratePdf(StreamReader Html, MemoryStream pdf, Size pageSize)
        {
            Process p;
            StreamWriter stdin;
            ProcessStartInfo psi = new ProcessStartInfo();

            psi.FileName = @"C:\wkhtmltox\bin\wkhtmltopdf.exe";

            // run the conversion utility 
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WorkingDirectory = @"C:\inetpub\wwwroot\PdfGenerator\";

            // note that we tell wkhtmltopdf to be quiet and not run scripts 
            psi.Arguments = "-q -n --disable-smart-shrinking " + (pageSize.IsEmpty ? "" : "--page-width " + pageSize.Width + "mm --page-height " + pageSize.Height + "mm") + " - -";

            p = Process.Start(psi);

            try
            {

                stdin = p.StandardInput;
                stdin.AutoFlush = true;
                stdin.Write(Html.ReadToEnd());
                stdin.Dispose();

                //CopyStream(p.StandardOutput.BaseStream, pdf);
                SaveStream(p.StandardOutput.BaseStream, @"C:\inetpub\wwwroot\PdfGenerator\carta.pdf");
                p.StandardOutput.Close();
                pdf.Position = 0;

                p.WaitForExit(10000);

                return pdf;
            }
            catch
            {
                return null;
            }
            finally
            {
                p.Dispose();
            }
        }
        */

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }


        public static void SaveStream(Stream input, string filename)
        {
            using (Stream file = File.Create(filename))
            {
                CopyStream(input, file);
            }
        }





    }
}


