//using M0.Resource;
using System;
using System.Data.Common;
using System.Data.SqlClient;
//using System.DirectoryServices;
using Dapper;
//using Models.Entity;
using System.Linq;
//using M0.Models.Entity;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Principal;
/*using System.Data.OracleClient;*/
//using Oracle.DataAccess.Client;
using System.Data;
using ServiceMonitor.Resource;

namespace ServiceMonitor.Models
{
    public class Conexao
    {
        //private OracleConnection con;
        private DbConnection _connection;
        private string strConexao = StrConexao.Desenvolvimento;
        public object objConsulta;
        public int id = 0;



        /*
         Validação de segurança: quando o usuário se autenticar, deve gravar na tabela de login o token e o IP que ele se logou.
         Toda requisição a um serviço, deve checar se é o mesmo token e IP registrado na tabela.    
         MultipleActiveResultSets=True; ele permite que você utilize numa mesma conexão múltiplos dataReaders.
         */

        //public SqlConnection getConnection(string TipoConexao)
        public dynamic getConnection(string TipoConexao)
        {

            try
            {
                TipoConexao = Utils.DescriptografarUrl(TipoConexao);

                //-- Faz a busca pela conexão definida no resource StrConexao
                var prop = typeof(StrConexao).GetProperty(TipoConexao,
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

                //-- Caso a conexão não existir, retorna a conexão em desenvolvimento
                //-- Caso contrário, retornará a conexão desejada

                /* comentei pelo erro: SqlConnection operação inválida. a conexão está fechada.
                 * Aparentemente em requisições async, ocorre o erro acima.
                 * estou suspeitando do código abaixo manter apenas 1 conexão ativa para todo o sistema. Como*/
                if (prop == null)
                    throw new ArgumentException("Ocorreu um erro ao acessar a base de dados, entre em contato com o administrador do sistema.");
                else
                {
                    
                    //Conexão Oracle
                    var conexao = TipoConexao.ToLower().IndexOf("oracle");
                    if (TipoConexao.ToLower().IndexOf("oracle") >= 0)
                    {
                        var connectionString = (string)prop.GetValue(default(StrConexao));
                        //OracleConnection oracleConnection = new OracleConnection(connectionString);
                        //return oracleConnection;
                        return null;
                    }//Conexão Sql Server
                    //else if (TipoConexao.ToLower().IndexOf("producao") == 0)
                    else 
                    {
                        var connectionString = (string)prop.GetValue(default(StrConexao));
                        SqlConnection sqlConnection = new SqlConnection((string)prop.GetValue(default(StrConexao)));
                        return sqlConnection;
                    }
                    //else {
                        //return null;
                    //}
                    
                    //SqlConnection sqlConnection = new SqlConnection((string)prop.GetValue(default(StrConexao)));
                    //return sqlConnection;

                }
                               
                //return new SqlConnection(StrConexao.Desenvolvimento);
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new Exception("Ocorreu um erro ao acessar a base de dados, entre em contato com o administrador do sistema.");
            }
        }


        /*
        public IDbConnection getConnectionOracle(string TipoConexao)
        {

                var oracleConnection = new OracleConnection("OracleConnectionString");
                oracleConnection.Open();
                return oracleConnection;

        }
        */


        /*
        public object Autenticar(object objeto)
        {

            object retorno = new object();

            //string Tpc = objeto.GetType().GetProperty("Tpc").GetValue(objeto).ToString();
            try
            {
                using (_connection = getConnection(Utils.GetPropertyValue(objeto, "Tpc")))
                {
                    // Instânciando o objeto USUARIO
                    Usuario usuario = new Usuario();
                    dynamic usuarioLogin = null;
                    dynamic usuarioCooperado = null;
                    //List<Modulo> modulos = new List<Modulo>();

                    string Login = "";
                    string Senha = "";
                    string Tpc = "";
                    string Ip = "";
                    string Mac = "";

                    //Montagem do sql para consulta dos dados do usuário, conforme o seu email.
                    string SqlUsuario = Seguranca.BuscarDadosUsuario;
                    string SqlModulos = Seguranca.ListarModulosPorUsuario;


                    Login = Utils.GetPropertyValue(objeto, "Login");
                    Senha = Utils.GetPropertyValue(objeto, "Senha");
                    Tpc = Utils.GetPropertyValue(objeto, "Tpc");
                    Ip = Utils.GetPropertyValue(objeto, "Ip");
                    Mac = Utils.GetPropertyValue(objeto, "Mac");

                    string SenhaCrypto = Utils.Criptografar(Senha);

                    //Consultado via Dapper na base se existe o usuario na base do Synergius
                    usuario = _connection.Query<Usuario>(SqlUsuario, new { Login = Login }).FirstOrDefault();

                    //Se o usuário não existir na tabela m0.usuario, pesquisa na tabela de m1.login
                    if (usuario == null)
                    {
                        usuarioLogin = _connection.Query(Seguranca.BuscarDadosUsuarioLogin, new { Login = Login }).FirstOrDefault();

                    }

                    if (usuario == null && usuarioLogin == null)
                    {
                        usuarioCooperado = _connection.Query(Seguranca.BuscarDadosUsuarioCooperado, new { Crm = Login }).FirstOrDefault();
                        SenhaCrypto = Utils.GerarHashMd5(Senha);

                    }

                    if (usuario != null)
                    {

                        if (usuario.Tipo == "INTERNO")
                        {
                            string ipServer = ConfigurationManager.AppSettings["LDAP"].ToString();

                            if (AutenticaLDAP(ipServer, usuario.Login, Senha))
                            {
                                //Login do usuario autenticado no windows
                                string loginWin = WindowsIdentity.GetCurrent().Name.Split('\\')[1].Trim();

                                var modulos = _connection.Query(SqlModulos, new { Login = Login }).ToList();

                                retorno = new
                                {
                                    Out_Erro = 0,
                                    Out_Mensagem = "Usuário autenticado com sucesso",
                                    usuario = new
                                    {
                                        codigo = usuario.Codigo,
                                        nome = usuario.Nome,
                                        dataNascimento = usuario.DataNascimento,
                                        cpf = usuario.Cpf,
                                        login = usuario.Login,
                                        email = usuario.Email,
                                        celular = usuario.Celular,
                                        tipo = usuario.Tipo,
                                        tecnico = usuario.Tecnico,
                                        conexao = Utils.DescriptografarUrl(Tpc),
                                        tpc = Tpc,
                                        ip = Ip,
                                        mac = Mac,
                                        usuariocodigo = usuario.Codigo,
                                        modulos
                                    }
                                };
                            }
                            else
                            {
                                retorno = new
                                {
                                    Out_Erro = 1,
                                    Out_Mensagem = "Usuario não encontrato na base de dados, verifique seu login ou sua senha",
                                };
                            }
                        }

                        //Se for usuário do tipo EXTERNO, valida a senha na base local do Synergius
                        else
                        {
                            if (SenhaCrypto == usuario.Senha)
                            {
                                var modulos = _connection.Query(SqlModulos, new { Login = Login }).ToList();
                                retorno = new
                                {
                                    Out_Erro = 0,
                                    Out_Mensagem = "Usuário autenticado com sucesso",
                                    usuario = new
                                    {
                                        codigo = usuario.Codigo,
                                        nome = usuario.Nome,
                                        dataNascimento = usuario.DataNascimento,
                                        cpf = usuario.Cpf,
                                        login = usuario.Login,
                                        email = usuario.Email,
                                        celular = usuario.Celular,
                                        tipo = usuario.Tipo,
                                        tecnico = usuario.Tecnico,
                                        conexao = Utils.DescriptografarUrl(Tpc),
                                        tpc = Tpc,
                                        ip = Ip,
                                        mac = Mac,
                                        usuariocodigo = usuario.Codigo,
                                        modulos

                                    }
                                };
                            }
                            else
                            {
                                retorno = new
                                {
                                    Out_Erro = 1,
                                    Out_Mensagem = "Usuario não encontrato na base de dados, verifique seu login ou sua senha",
                                };
                            }
                        }
                    }
                    else if (usuarioLogin != null && SenhaCrypto == usuarioLogin.LoginWebSenha)
                    {
                        //validações
                        int _outErro = usuarioLogin.LoginWebConfirmacao_Cadastro ? 0 : 1;
                        string _outMensagem = usuarioLogin.LoginWebConfirmacao_Cadastro ? "Usuário autenticado com sucesso" : "Cadastro pedente de confirmação";


                        retorno = new
                        {
                            Out_Erro = _outErro,
                            Out_Mensagem = _outMensagem,
                            usuario = new
                            {
                                codigo = usuarioLogin.LoginWebCodigo,
                                nome = usuarioLogin.LoginWebNome,
                                dataNascimento = usuarioLogin.LoginWebDataNascimento,
                                cpf = usuarioLogin.LoginWebCPF,
                                login = usuarioLogin.LoginWebCPF,
                                email = usuarioLogin.LoginWebEmail,
                                celular = usuarioLogin.LoginWebTelefone,
                                tipo = usuarioLogin.TipoUsuario,
                                tecnico = 0,
                                conexao = Utils.DescriptografarUrl(Tpc),
                                tpc = Tpc,
                                ip = Ip,
                                mac = Mac,
                                usuariocodigo = 0

                            }
                        };

                    }
                    else if (usuarioCooperado != null && SenhaCrypto == usuarioCooperado.Senha)
                    {

                        retorno = new
                        {
                            Out_Erro = 0,
                            Out_Mensagem = "Usuário autenticado com sucesso",
                            usuario = new
                            {
                                codigo = usuarioCooperado.Usuario,
                                nome = usuarioCooperado.Nome,
                                dataNascimento = usuarioCooperado.DataNascimento,
                                cpf = usuarioCooperado.cnp,
                                login = usuarioCooperado.Usuario,
                                email = usuarioCooperado.email,
                                celular = usuarioCooperado.celular,
                                tipo = "COOPERADO",
                                tecnico = 0,
                                conexao = Utils.DescriptografarUrl(Tpc),
                                tpc = Tpc,
                                ip = Ip,
                                mac = Mac,
                                usuariocodigo = 0

                            }
                        };


                    }
                    //se o usuário não existir.Verifica se é um usuário cliente (Tabela de Login).
                    else
                    {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Usuario não encontrato na base de dados, verifique seu login ou sua senha",
                        };
                    }
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno = new
                {
                    Out_Erro = 1,
                    Out_Mensagem = ex.Message,
                };
                return retorno;
            }
            finally
            {
                //_connection.Close();
                //_connection.Dispose();
            }
        }
        
        public bool AutenticaLDAP(string ipServer, string username, string password)
        {
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + ipServer, username, password);
            try
            {
                DirectorySearcher search = new DirectorySearcher(entry);
                SearchResult result;
                result = search.FindOne();

                if (result != null) return true; else return false;
            }
            catch (Exception e)
            {
                return false ;
            }
        }
        */
        /*
        public Usuario BuscarUsuarioPorLogin(object parametros, string StrConexao)
        {
            using (_connection = getConnection(StrConexao))
            {
                _connection.Open();
                string Sql = Seguranca.BuscarUsuarioPorLogin;
                var user = _connection.Query<Usuario>(Sql, param: parametros ).FirstOrDefault();

                //Inserir Logacesso login
                if (user != null)
                {
                    LogAcesso LogAcesso = new LogAcesso();
                    LogAcesso.LogAcessoDataOperacao = DateTime.Now;
                    LogAcesso.LogAcessoTipoOperacao = "L";
                    LogAcesso.LogAcessoDescricao = "Usuário " + user.Login + " logou.";
                    LogAcesso.UsuarioCodigo = user.Codigo;
                    _connection.Insert(LogAcesso);
                }

                return user;
            }
        }*/
    }
}