using Dapper;
using M0.Resource;
using Models.Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace M0.Models
{
    public class Dao : Conexao
    {

        private static DbConnection _connection;

        public Dao() : base() { }

        //delegate para retorno do options (lista de opções de um campo)
        delegate Object deleg(string a, string b, dynamic c, dynamic d);


        //Delegate responsável por retornar a lista de options de um campo. Podendo ser valor padrão ou um Serviço Vinculado
        #region _DelegateOptions
        deleg _DelegateOptions = (Tipo_Entrada, Valor_Padrao, Valor_Padrao_Servico, ParametrosCondicao) => {

            JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            object retorno = new object();

            try {

                    //se o tipo de entrada do campo estiver setado como options e o campo Valor_Padrao_Servico estiver preenchido
                    if (Tipo_Entrada == "options" && Valor_Padrao_Servico != null)
                    {
                        var WSservico = _connection.Query(WS.BuscarServico, new { Servico = Valor_Padrao_Servico }).ToList();//.AsList();

                        if (WSservico != null && WSservico.Count() > 0)
                        {
                            if (WSservico[0].Procedure != null && WSservico[0].Procedure != "")
                            {

                                //monta a procedure dinamicamente
                                string strSqlProcedure = String.Format("execute {0} {1} ", WSservico[0].Procedure, ParametrosCondicao);
                                //executa a procedure
                                var data = _connection.Query(strSqlProcedure).ToList();

                                retorno = data;

                            }
                        }
                    }
                    else if (Tipo_Entrada == "options" && Valor_Padrao != null)
                    {
                        //serializo o retorno do banco, caso contrário irá transitar como string, desta forma automaticamente incluindo scapes / antes das aspas
                        dynamic resultado = serializer.DeserializeObject(Valor_Padrao);

                        retorno = resultado;

                    }
                    else
                    {
                        retorno = null;
                    }

                

            }   
            catch (Exception ex)
            {
                retorno = new
                {
                    Out_Erro = 1,
                    Out_Mensagem = ex.Message + "(cod:ExpDelegate)"

                };
            }


            return retorno;
        };
        #endregion _DelegateOptions

        //Método responsável por Listar registros para uma Grid
        #region CRUD - ListarGrid
        public object ListarGrid(int Servico, dynamic Parametros, string StrConexao, int UsuarioCodigo)
        {
            object retorno = new object();
            JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                using (_connection = getConnection(StrConexao))
                {

                    //lista todos os parâmetros do serviço
                    var servicoParametros = Utils.BuscarServicoParametros(Servico, StrConexao);

                    //lista informações do Serviço
                    var WSservico = _connection.Query(WS.BuscarServico, new { Servico = Servico }).ToList();//.AsList();

                    //configura os tipos de Memorização
                    var Memory = "";

                    //lista todos os grids memorizados par o usuario, perfil ou setor que ele pertença
                    var MemoryData = _connection.Query(WS.BuscarServicoMemorizar, new { Servico = Servico }).ToList();
                    var MemoryList = (from x in MemoryData select x).Select(dado => new {
                        Codigo = dado.Codigo,
                        Nome = dado.Nome,
                        Permissao = dado.Permissao != null ? dado.Permissao : "",
                        Json = dado.Json != null && dado.Json != "" ? serializer.DeserializeObject(dado.Json) : ""
                    }).ToList();

                    //retorna os parâmetros do tipo Saida
                    var thead = (from x in servicoParametros select x).Where(dado => dado.Tipo == "S" && dado.Tipo_Entrada != "botao").Select(dado => new { textname = dado.Nome, textshow = dado.Nome_Exibicao, show = dado.Visivel, key = dado.Chave, Mask = dado.Mascara, size = dado.Tamanho > 0 ? dado.Tamanho : "" }).ToList();

                    //recupera os parametros de pesquisa do filtro para retornar no value de cada campo
                    List<KeyValuePair<string, string>> parametrosFind = Utils.AgruparParametrosCondicaoLista(Parametros);
                    //retorna somente os campos que serão exibidos como pesquisa
                    //var find = (from x in servicoParametros select x).Where(dado => dado.Pesquisa == "S" && dado.Visivel == "S")
                    var find = (from x in servicoParametros select x).Where(dado => dado.Pesquisa == "S" )
                                                                     .Select(dado => new {
                                                                         label = dado.Nome_Exibicao,
                                                                         name = dado.Nome,
                                                                         type = dado.Tipo_Entrada != null ? dado.Tipo_Entrada : "",
                                                                         options = dado.Tipo_Entrada == "options" ? _DelegateOptions(dado.Tipo_Entrada, dado.Valor_Padrao, dado.Valor_Padrao_Servico, null) : "",
                                                                         Value = (from x in parametrosFind select x).Where(dadoFind => dadoFind.Key == dado.Nome).Select(dadoFind => dadoFind.Value).SingleOrDefault()
                                                                     }).ToList();

                    //retorna os link de uma grid
                    var DataLink = (from x in servicoParametros select x).Where(dado => dado.Tipo_Entrada == "link").Select(dado => new { LinkNome = dado.Link_Nome, LinkParametro = dado.Link_Parametro, LinkBotao = dado.Link_Botao });

                    //retorna os Botões da Grid
                    var btns = _connection.Query(WS.BuscarTipoServicoBoteosJSON, new { Servico = Servico }).ToList();
                    /*var btns = (from x in var_btns select x).Select(dado => new { dado.type, dado.label, dado.@class, dado.action, dado.element, dado.classIcon, dado.tituloAba, dado.ServicoUrl, dado.Servico,
                        (from y in DataLink).Where(dado => x.label = 
                    }).ToList();*/

                    //dado.label, dado.class, dado.action, dado.element, dado.classIcon, dado.tituloAba, dado.ServicoUrl, dado.Servico,

                    if (WSservico != null && WSservico.Count() > 0)
                    {
                        
                        if (WSservico[0].Procedure != null && WSservico[0].Procedure != "")
                        {
                            //retorna todos os parametros enviados pela requisição
                            var parametrosCondicao = Utils.AgruparParametrosCondicao(Servico, Parametros, StrConexao);
                            //monta a procedure dinamicamente
                            string strSqlProcedure = String.Format("execute {0} {1} ", WSservico[0].Procedure, parametrosCondicao);
                            //executa a procedure
                            var data = _connection.Query(strSqlProcedure ).ToList();//.AsList();

                            retorno = new
                            {
                                Out_Erro = 0,
                                Out_Mensagem = "grid retornada com sucesso",
                                servico = Servico,
                                grid = new
                                {
                                    head = new 
                                    {
                                        thead,

                                        config = new
                                        {
                                            detailed = new
                                            {

                                            },

                                            btns,
                                            DataLink,
                                            find,
                                            MemoryList
                                        }
                                    },
                                    data
                                    
                                    

                                }
                            };

                       }
                        else {
                            retorno = new
                            {
                                Out_Erro = 1,
                                Out_Mensagem = "Procedure não definida (cod:DaoAut01)"

                            };
                        }


                    }
                    else {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Serviço não encontrado (cod:DaoAut02)"

                        };
                    }


                    /*

                    _connection.Open();

                    if (UsuarioCodigo != 0)
                    {
                        LogAcesso logAcesso = new LogAcesso();
                        logAcesso = LogAcesso.ConfigurarLogAcesso(UsuarioCodigo, LogAcesso.PESQUISAR, new { });
                        _connection.Insert(logAcesso);
                    }


                    */

                  
                    
                }
            }
            catch (Exception ex)
            {
                retorno = new
                {
                    Out_Erro = 1,
                    Out_Mensagem = ex.Message + "(cod:ExpDaoAut)"

                };
            }


            return retorno;
        }
        #endregion

        //Método responsável por Insert/Update de um registro
        #region CRUD - MemorizarServico
        public object MemorizarServico(int Servico, dynamic Parametros, string StrConexao, int UsuarioCodigo)
        {
            object retorno = new object();
            JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                using (_connection = getConnection(StrConexao))
                {

                    //lista todos os parâmetros do serviço
                    var servicoParametros = Utils.BuscarServicoParametros(Servico, StrConexao);

                    //lista informações do Serviço
                    var WSservico = _connection.Query(WS.BuscarServico, new { Servico = Servico }).ToList();//.AsList();


                    if (WSservico != null && WSservico.Count() > 0 && servicoParametros.Count() > 0)
                    {

                        if (WSservico[0].Procedure != null && WSservico[0].Procedure != "")
                        {
                            //retorna todos os parametros enviados pela requisição
                            var parametrosData = Utils.AgruparParametrosData(Servico, Parametros, StrConexao);

                            //monta a procedure dinamicamente
                            string strSqlProcedure = String.Format("execute {0} {1} ", WSservico[0].Procedure, parametrosData);
                            //executa a procedure
                            var _data = _connection.Query(strSqlProcedure).ToList();//.AsList();

                            //lista todos os grids memorizados par o usuario, perfil ou setor que ele pertença
                            var ServicoMemorizar = Utils.ExtrairValueJSON(Parametros, "ServicoMemorizar") != null ? Utils.ExtrairValueJSON(Parametros, "ServicoMemorizar") : "0";
                            var MemoryData = _connection.Query(WS.BuscarServicoMemorizar, new { Servico = ServicoMemorizar }).ToList();

                            var MemoryList = (from x in MemoryData select x).Select(dado => new {
                                Codigo = dado.Codigo,
                                Nome = dado.Nome,
                                Permissao = dado.Permissao != null ? dado.Permissao : "",
                                Json = dado.Json != null && dado.Json != "" ? serializer.DeserializeObject(dado.Json) : ""
                            }).ToList();

                            /*
                            var fieldProximoServicoUrl = (from parametros in servicoParametros select parametros).Where(dado => dado.Tipo == "S" && dado.Nome == "ProximoServicoUrl").Select(dado => new { ProximoServicoUrl = dado.Valor_Padrao }).SingleOrDefault();
                            var fieldProximoServico = (from parametros in servicoParametros select parametros).Where(dado => dado.Tipo == "S" && dado.Nome == "ProximoServico").Select(dado => new { ProximoServico = dado.Servico_Vinculado }).SingleOrDefault();
                            var fieldProximoContainer = (from parametros in servicoParametros select parametros).Where(dado => dado.Tipo == "S" && dado.Nome == "ProximoContainer").Select(dado => new { ProximoContainer = dado.Valor_Padrao }).SingleOrDefault();

                            var ProximoServico = fieldProximoServicoUrl != null ? fieldProximoServico.ProximoServico : null;
                            var ProximoContainer = fieldProximoContainer != null ? fieldProximoContainer.ProximoContainer : null;

                            //concatena a URL do Serviço e o COdigo do registro
                            var ProximoServicoUrl = "";
                            if (fieldProximoServicoUrl != null && Utils.ExtrairValueIDictionary(_data, "Codigo") != null)
                            {
                                ProximoServicoUrl = fieldProximoServicoUrl.ProximoServicoUrl + Utils.ExtrairValueIDictionary(_data, "Codigo");
                            }
                            */
                            retorno = new
                            {
                                Codigo = Utils.ExtrairValueIDictionary(_data, "Codigo"),
                                Out_Erro = Utils.ExtrairValueIDictionary(_data, "Out_Erro"),
                                Out_Mensagem = Utils.ExtrairValueIDictionary(_data, "Out_Mensagem"),
                                MemoryList
                                //ProximoServicoUrl,
                                //ProximoServico,
                                //ProximoContainer
                            };


                        }
                        else
                        {
                            retorno = new
                            {
                                Out_Erro = 1,
                                Out_Mensagem = "Procedure não definida (cod:DaoMemo01)"

                            };
                        }


                    }
                    else if (servicoParametros.Count() == 0)
                    {

                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Nenhum parâmetro definido para o serviço (cod:DaoMemo02)"

                        };
                    }
                    else
                    {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Serviço não encontrado (cod:DaoMemo03)"

                        };
                    }


                    /*

                    _connection.Open();

                    if (UsuarioCodigo != 0)
                    {
                        LogAcesso logAcesso = new LogAcesso();
                        logAcesso = LogAcesso.ConfigurarLogAcesso(UsuarioCodigo, LogAcesso.PESQUISAR, new { });
                        _connection.Insert(logAcesso);
                    }


                    */



                }
            }
            catch (Exception ex)
            {
                retorno = new
                {
                    Out_Erro = 1,
                    Out_Mensagem = ex.Message + " (cod:ExpDaoMemo)"

                };
            }

            return retorno;
        }
        #endregion

        //Método responsável por criar um FORM
        #region CRUD - CriarForm
        public object CriarForm(int Servico, int Chave, dynamic Parametros, string StrConexao, int UsuarioCodigo)
        {
            object retorno = new object();
            List<dynamic> data = new List<dynamic>();
            JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                using (_connection = getConnection(StrConexao))
                {

                    //lista todos os parâmetros do serviço
                    var servicoParametros = Utils.BuscarServicoParametros(Servico, StrConexao);

                    //lista informações do Serviço
                    var WSservico = _connection.Query(WS.BuscarServico, new { Servico = Servico }).ToList();//.AsList();

                    //retorna o campo PK
                    var filedPK = (from x in servicoParametros select x)
                                  .Where(dado => dado.Chave == "PK")
                                  .Select(dado =>  new { dado.Nome })
                                  .Take(1).ToList();

                    if (filedPK.Count == 0)
                    {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Chave não definida (cod:DaoCFrm01)"
                        };

                        return retorno;
                    }


                    //retorna as informações do banco de dados para preenchimento do formulário, com base na chave enviada
                    if (WSservico != null && WSservico.Count() > 0)
                    {
                        //percorre o JSON, retornando o value para o campo PK do formulário
                        var filedPKValue = Utils.ExtrairValueJSON(Parametros, filedPK[0].Nome);
                        Chave = filedPKValue != "" ? Convert.ToInt32(filedPKValue) : 0;

                        if (WSservico[0].Procedure != null && WSservico[0].Procedure != "" 
                            && filedPK[0].Nome != null && filedPK[0].Nome != ""
                            && Chave > 0)
                        {

                            //monta a procedure dinamicamente
                            string strSqlProcedure = String.Format("execute {0} @{1} = {2} ", WSservico[0].Procedure, filedPK[0].Nome, Chave);
                            //executa a procedure
                            data = _connection.Query(strSqlProcedure).ToList();//.AsList();
                        }
                    }

                    //monta os blocos
                    var _blocks = (from x in WSservico select x).Select(dado => dado.Bloco ).SingleOrDefault();
                    dynamic Blocks = _blocks != null ? serializer.DeserializeObject(_blocks) : "";

<<<<<<< .mine

                    //retorna os link de uma grid
                    //var DataLink = (from x in servicoParametros select x).Where(dado => dado.Tipo_Entrada == "link").Select(dado => new { LinkNome = dado.Link_Nome, LinkParametro = dado.Link_Parametro, LinkBotao = dado.Link_Botao }).ToList();

                    //var DataLink = new { LinkNome = "LinkServicoCodigo", LinkParametro = "Codigo" };

||||||| .r60
=======

>>>>>>> .r73
                    //retorna os parâmetros do tipo Saida
                    var fields = (from parametros in servicoParametros
                                  where parametros.Tipo == "S" && parametros.Tipo_Entrada != "botao"
                                  select new
                                  {
                                      key = parametros.Chave,
                                      label = parametros.Nome_Exibicao,
                                      typeDados = parametros.Tipo_Dado == "string" ? "varchar" :
                                                  parametros.Tipo_Dado == "int" ? "int" : "string",
                                      size = parametros.Tamanho > 0 ? parametros.Tamanho : "",
                                      name = parametros.Nome,
                                      show = parametros.Visivel,
                                      mask = parametros.Mascara,
                                      @readonly = parametros.Somente_Leitura == "S" ? "true" : "false",
                                      required = parametros.Obrigatorio == "S" ? "true" : "false",
                                      typeInput = (parametros.Tipo_Entrada != "" && parametros.Tipo_Entrada != null ? parametros.Tipo_Entrada : ""),
                                      @value = Utils.ExtrairValueIDictionary(data, parametros.Nome),
                                      options = parametros.Tipo_Entrada == "options" ? _DelegateOptions(parametros.Tipo_Entrada, parametros.Valor_Padrao, parametros.Valor_Padrao_Servico, null) : "",
                                      Help = parametros.Ajuda != null && parametros.Ajuda != "" ? parametros.Ajuda : "",
                                      Block = parametros.Bloco != null && parametros.Bloco != "" ? serializer.DeserializeObject(parametros.Bloco) : "",
<<<<<<< .mine
                                      Events = parametros.Evento != null && parametros.Evento != "" ? serializer.DeserializeObject(parametros.Evento) : "",
                                      Grid = parametros.Tipo_Entrada != "grid" ? null :
                                      new {
                                          Url = parametros.Servico_Vinculado_Url,
                                          Servico = parametros.Servico_Vinculado,
                                          DataLink = (from x in servicoParametros select x).Where(dado => dado.Link_Botao == parametros.Nome).Select(dado => new { LinkNome = dado.Link_Nome, LinkParametro = dado.Link_Parametro }).ToList()
                }
||||||| .r60
                                      Events = parametros.Evento != null && parametros.Evento != "" ? serializer.DeserializeObject(parametros.Evento) : ""
=======
                                      Events = parametros.Evento != null && parametros.Evento != "" ? serializer.DeserializeObject(parametros.Evento) : "",
                                      Grid = parametros.Tipo_Entrada != "grid" ? null :
                                      new {
                                          Url = "http://192.168.5.124/SynergiusServicos/M0/api/ws/listargrid",
                                          Servico = parametros.Servico_Vinculado,
                                          Data = ""
                                      }
>>>>>>> .r73
                                  }).ToList();



            //retorna os Botões do Form
            var btns = _connection.Query(WS.BuscarTipoServicoBoteosJSON, new { Servico = Servico }).ToList();//.AsList();

                    if (WSservico != null && WSservico.Count() > 0)
                    {
                        retorno = new
                        {
                            Out_Erro = 0,
                            Out_Mensagem = "form retornado com sucesso",
                            servico = Servico,
                            form = new
                            {
                                Blocks,
                                fields,
                                btns
                            }
                        };
                    }
                    else
                    {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Serviço não encontrado (cod:DaoCFrm02)"
                        };
                    }


                    /*

                    _connection.Open();

                    if (UsuarioCodigo != 0)
                    {
                        LogAcesso logAcesso = new LogAcesso();
                        logAcesso = LogAcesso.ConfigurarLogAcesso(UsuarioCodigo, LogAcesso.PESQUISAR, new { });
                        _connection.Insert(logAcesso);
                    }


                    */



                }
            }
            catch (Exception ex)
            {
                retorno = new
                {
                    Out_Erro = 1,
                    Out_Mensagem = ex.Message + "(cod:ExpDaoCFrm)"

                };
            }


            return retorno;
        }
        #endregion

        //Método responsável por Excluir registros
        #region CRUD - ExcluirForm
        public object ExcluirForm(int Servico, int Chave, dynamic Parametros, string StrConexao, int UsuarioCodigo)
        {
            object retorno = new object();

            try
            {
                using (_connection = getConnection(StrConexao))
                {

                    //lista todos os parâmetros do serviço
                    var servicoParametros = Utils.BuscarServicoParametros(Servico, StrConexao);

                    //lista informações do Serviço
                    var WSservico = _connection.Query(WS.BuscarServico, new { Servico = Servico }).ToList();//.AsList();

                    if (WSservico != null && WSservico.Count() > 0 && Chave > 0)
                    {

                        if (WSservico[0].Procedure != null && WSservico[0].Procedure != "")
                        {
                            //retorna todos os parametros enviados pela requisição
                            var parametrosCondicao = Chave;
                            //monta a procedure dinamicamente
                            string strSqlProcedure = String.Format("execute {0}  @Codigo = '{1}' ", WSservico[0].Procedure, parametrosCondicao);
                            //executa a procedure
                            var data = _connection.Query(strSqlProcedure).ToList();//.AsList();

                            retorno = data[0];

                        }
                        else
                        {
                            retorno = new
                            {
                                Out_Erro = 1,
                                Out_Mensagem = "Procedure não definida"

                            };
                        }


                    }
                    else if (Chave == 0)
                    {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Parâmetro inconsistente (cod:DaoExc01)"

                        };
                    }
                    else
                    {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Serviço não encontrado (cod:DaoExc02)"

                        };
                    }


                    /*

                    _connection.Open();

                    if (UsuarioCodigo != 0)
                    {
                        LogAcesso logAcesso = new LogAcesso();
                        logAcesso = LogAcesso.ConfigurarLogAcesso(UsuarioCodigo, LogAcesso.PESQUISAR, new { });
                        _connection.Insert(logAcesso);
                    }


                    */


                }
            }
            catch (Exception ex)
            {
                retorno = new
                {
                    Out_Erro = 1,
                    Out_Mensagem = ex.Message + "(cod:ExpDaoExc)"

                };
            }


            return retorno;
        }
        #endregion

        //Método responsável por Insert/Update de um registro
        #region CRUD - SalvarForm
        public object SalvarForm(int Servico, dynamic Parametros, string StrConexao, int UsuarioCodigo)
        {
            object retorno = new object();

            try
            {
                using (_connection = getConnection(StrConexao))
                {

                    //lista todos os parâmetros do serviço
                    var servicoParametros = Utils.BuscarServicoParametros(Servico, StrConexao);

                    //lista informações do Serviço
                    var WSservico = _connection.Query(WS.BuscarServico, new { Servico = Servico }).ToList();//.AsList();

                     //retorna os Botões dO Form
                    var btns = _connection.Query(WS.BuscarTipoServicoBoteosJSON, new { Servico = Servico }).ToList();//.AsList();


                    if (WSservico != null && WSservico.Count() > 0 && servicoParametros.Count() > 0)
                    {

                        if (WSservico[0].Procedure != null && WSservico[0].Procedure != "" )
                        {
                            //retorna todos os parametros enviados pela requisição
                            var parametrosData = Utils.AgruparParametrosData(Servico, Parametros, StrConexao);
                            //monta a procedure dinamicamente
                            string strSqlProcedure = String.Format("execute {0} {1} ", WSservico[0].Procedure, parametrosData);
                            //executa a procedure
                            var _data = _connection.Query(strSqlProcedure).ToList();//.AsList();

                            var fieldProximoServico = (from parametros in servicoParametros select parametros).Where(dado => dado.Tipo == "S" && dado.Nome == "ProximoServico").Select(dado => new { ProximoServico = dado.Servico_Vinculado }).SingleOrDefault();
                            var fieldProximoServicoUrl = (from parametros in servicoParametros select parametros).Where(dado => dado.Tipo == "S" && dado.Nome == "ProximoServico").Select(dado => new { ProximoServicoUrl = dado.Servico_Vinculado_Url }).SingleOrDefault();
                            var fieldProximoServicoTipoExibicao = (from parametros in servicoParametros select parametros).Where(dado => dado.Tipo == "S" && dado.Nome == "ProximoServico").Select(dado => new { ProximoServicoTipoExibicao = dado.Servico_Vinculado_Tipo_Exibicao }).SingleOrDefault();
                            var fieldProximoContainer = (from parametros in servicoParametros select parametros).Where(dado => dado.Tipo == "S" && dado.Nome == "ProximoContainer").Select(dado => new { ProximoContainer = dado.Valor_Padrao }).SingleOrDefault();
                            /*
                            var fieldProximoServicoUrl  = (from parametros in servicoParametros select parametros).Where(dado => dado.Tipo == "S" && dado.Nome == "ProximoServicoUrl").Select(dado => new { ProximoServicoUrl = dado.Servico_Vinculado_Url }).SingleOrDefault();
                            var fieldProximoServicoTipoExibicao = (from parametros in servicoParametros select parametros).Where(dado => dado.Tipo == "S" && dado.Nome == "ProximoServicoTipoExibicao").Select(dado => new { ProximoServicoTipoExibicao = dado.Servico_Vinculado_Tipo_Exibicao }).SingleOrDefault();
                            */

                            var ProximoServico = fieldProximoServico != null ? fieldProximoServico.ProximoServico : null;
                            var ProximoServicoTipoExibicao = fieldProximoServicoTipoExibicao != null ? fieldProximoServicoTipoExibicao.ProximoServicoTipoExibicao : null;
                            var ProximoContainer = fieldProximoContainer != null ? fieldProximoContainer.ProximoContainer : null;


                            //concatena a URL do Serviço e o COdigo do registro
                            var ProximoServicoUrl = "";
                            if (fieldProximoServicoUrl != null && Utils.ExtrairValueIDictionary(_data, "Codigo") != null ) {
                                ProximoServicoUrl = fieldProximoServicoUrl.ProximoServicoUrl + Utils.ExtrairValueIDictionary(_data, "Codigo");
                            }

                             retorno = new
                            {
                                Codigo = Utils.ExtrairValueIDictionary(_data, "Codigo"),
                                Out_Erro = Utils.ExtrairValueIDictionary(_data, "Out_Erro"),
                                Out_Mensagem = Utils.ExtrairValueIDictionary(_data, "Out_Mensagem"),
                                ProximoServico,
                                ProximoServicoUrl,
                                ProximoServicoTipoExibicao,
                                ProximoContainer
                            };


                        }
                        else
                        {
                            retorno = new
                            {
                                Out_Erro = 1,
                                Out_Mensagem = "Procedure não definida (cod:DaoSFrm01)"

                            };
                        }


                    }
                    else if (servicoParametros.Count() == 0)
                    {

                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Parametros não definidos para o serviço (cod:DaoSFrm02)"

                        };
                    }
                    else
                    {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Serviço não encontrado (cod:DaoSFrm03)"

                        };
                    }


                    /*

                    _connection.Open();

                    if (UsuarioCodigo != 0)
                    {
                        LogAcesso logAcesso = new LogAcesso();
                        logAcesso = LogAcesso.ConfigurarLogAcesso(UsuarioCodigo, LogAcesso.PESQUISAR, new { });
                        _connection.Insert(logAcesso);
                    }


                    */



                }
            }
            catch (Exception ex)
            {
                retorno = new
                {
                    Out_Erro = 1,
                    Out_Mensagem = ex.Message + " (cod:ExpDaoSFrm)"

                };
            }

            return retorno;
        }
        #endregion

        //Método responsável por listar os Menus do Módulo selecionado pelo usuário
        #region CRUD - ListarMenu
        public static object ListarMenu(object objeto)
        {
            object retorno = new object();

            try
            {

                int modulo = Utils.GetPropertyValue(objeto, "Modulo");
                int usuarioCodigo = Utils.GetPropertyValue(objeto, "UsuarioCodigo");

                string SqlMenu = Seguranca.ListarModulosMenuPorUsuario;

                using (_connection = Conexao.getConnection(Utils.GetPropertyValue(objeto, "Tpc")))
                {
                    var menu = _connection.Query(SqlMenu, new { Modulo = modulo, UsuarioCodigo = usuarioCodigo }).AsList();

                    if (menu != null)
                    {

                        //Obs: este menu suporta apenas 1 nével de submenu

                        //cria o jSon contendo apenas as colunas correspondentes aos Recursos Filhos (LINQ)
                        var RecursosFilho = (from x in menu where x.RecursoPai != null select new { x.RecursoCodigo, x.RecursoRotulo, x.RecursoNome, x.RecursoIcone, x.ServicoTipoExibicao, x.Servico, x.ServicoUrl, x.RecursoPai }).ToList();

                        //cria o jSon contendo apenas as colunas correspondentes aos Recursos Pai (LINQ)
                        var Recursos = (from x in menu where x.RecursoPai == null select new { x.RecursoCodigo, x.RecursoRotulo, x.RecursoNome, x.RecursoIcone, ServicoTipoExibicao = x.PossuiFilhos == "S" ? "" : x.ServicoTipoExibicao, Servico = x.PossuiFilhos == "S" ? "" : x.Servico, ServicoUrl = x.PossuiFilhos == "S" ? "" : x.ServicoUrl
                            , sub = (from y in RecursosFilho where y.RecursoPai == x.RecursoCodigo select new { y.RecursoCodigo, y.RecursoRotulo, y.RecursoNome, y.RecursoIcone, y.ServicoTipoExibicao, y.Servico, y.ServicoUrl }).ToList()

                        }).ToList();


                        //cria o jSon contendo apenas as colunas correspondentes ao Modulo, fazendo um Top 1 (Take(1)) (LINQ)
                        var Modulos = (from x in menu select new { x.ModuloCodigo, x.ModuloRotulo, x.ModuloNome, x.ModuloIcone, Recursos }).Take(1).ToList();

                        //cria o Json de retorno
                        retorno = new
                        {
                            Out_Erro = 0,
                            Out_Mensagem = "Usuário com menu ativo",
                            Modulos

                        };

                    }
                    else
                    {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Usuário sem menu ativo"
                        };
                    }
                }


            }
            catch (Exception ex)
            {
                retorno = new
                {
                    Out_Erro = 1,
                    Out_Mensagem = ex.Message + "(cod:ExpDmn)"

                };
            }


            return retorno;
        }
        #endregion

        //Método responsável por criar um FORM
        #region FieldEvent
        public object FieldEvent(int Servico, dynamic Parametros, string StrConexao, int UsuarioCodigo)
        {
            object retorno = new object();
            List<dynamic> data = new List<dynamic>();
            JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                using (_connection = getConnection(StrConexao))
                {


                    //lista todos os parâmetros do serviço
                    var servicoParametros = Utils.BuscarServicoParametros(Servico, StrConexao);

                    //lista informações do Serviço
                    var WSservico = _connection.Query(WS.BuscarServico, new { Servico = Servico }).ToList();//.AsList();

                    //retorna as informações do banco de dados para preenchimento do formulário, com base na chave enviada
                    if (WSservico != null && WSservico.Count() > 0 && WSservico[0].Procedure != null && WSservico[0].Procedure != "")
                    {
                    
                    var parametrosCondicao = Utils.AgruparParametrosCondicao(Servico, Parametros, StrConexao);

                    var fields = (from parametros in servicoParametros
                                  where parametros.Tipo == "S"
                                  select new
                                  {
                                      key = parametros.Chave,
                                      label = parametros.Nome_Exibicao,
                                      typeDados = parametros.Tipo_Dado == "string" ? "varchar" :
                                                  parametros.Tipo_Dado == "int" ? "int" : "string",
                                      size = parametros.Tamanho > 0 ? parametros.Tamanho : "",
                                      name = parametros.Nome,
                                      show = parametros.Visivel,
                                      @readonly = parametros.Somente_Leitura == "S" ? "true" : "false",
                                      required = parametros.Obrigatorio == "S" ? "true" : "false",
                                      typeInput = (parametros.Tipo_Entrada != "" && parametros.Tipo_Entrada != null ? parametros.Tipo_Entrada : ""),
                                      @value = Utils.ExtrairValueIDictionary(data, parametros.Nome),
                                      options = parametros.Tipo_Entrada == "options" ? _DelegateOptions(parametros.Tipo_Entrada, null, Servico, parametrosCondicao) : "",
                                      Block = parametros.Bloco != null && parametros.Bloco != "" ? serializer.DeserializeObject(parametros.Bloco) : "",
                                      Events = parametros.Evento != null && parametros.Evento != "" ? serializer.DeserializeObject(parametros.Evento) : ""
                                  }).ToList();


                    retorno = new
                        {
                            Out_Erro = 0,
                            Out_Mensagem = "form retornado com sucesso",
                            servico = Servico,
                            form = new
                            {
                                fields
                            }
                        };


                    }
                    else
                    {
                        retorno = new
                        {
                            Out_Erro = 1,
                            Out_Mensagem = "Procedure não definida (cod:DaoFieldEvent01)"
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                retorno = new
                {
                    Out_Erro = 1,
                    Out_Mensagem = ex.Message + "(cod:ExpFieldEventCFrm)"

                };
            }
            finally
            {
                //_connection.Close();
                //_connection.Dispose();

            }

            return retorno;
        }
        #endregion





    }
}
