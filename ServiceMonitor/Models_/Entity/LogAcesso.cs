using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("LOGACESSO", Schema = "M0")]
    public class LogAcesso
    {
        public static string SALVAR = "S";
        public static string EDITAR = "E";
        public static string EXCLUIR = "D";
        public static string PESQUISAR = "P";
        public static string VISUALIZAR = "V";

        [Key]
        [Column("Codigo")]
        public int LogAcessoCodigo { get; set; }

        [ForeignKey("usuario")]
        [Column("usuario")]
        public int UsuarioCodigo { get; set; }

        [Column("descricao")]
        [StringLength(50)]
        public string LogAcessoDescricao { get; set; }

        [Column("tipo_operacao")]
        [StringLength(50)]
        public string LogAcessoTipoOperacao { get; set; }

        [Column("data_operacao")]
        [StringLength(50)]
        public DateTime LogAcessoDataOperacao { get; set; }

        [Column("nome_tabela")]
        [StringLength(100)]
        public string LogAcessoNomeTabela { get; set; }

        [Column("nome_campo")]
        [StringLength(100)]
        public string LogAcessoNomeCampo { get; set; }

        [Column("valor_campo")]
        [StringLength(100)]
        public int LogAcessoValorCampo { get; set; }

        public Usuario usuario { get; set; }

        [Editable(false)]
        public string Tpc { get; set; }

        public static LogAcesso ConfigurarLogAcesso(int usuarioCodigo, string tipoOperacao, object classe)
        {
            LogAcesso logAcesso = new LogAcesso();
            logAcesso.UsuarioCodigo = usuarioCodigo;
            logAcesso.LogAcessoTipoOperacao = tipoOperacao;
            logAcesso.LogAcessoDataOperacao = DateTime.Now;
            logAcesso.LogAcessoNomeTabela = classe.GetType().Name;

            string tipoOperacaoDescricao = "";
            if (tipoOperacao == SALVAR) tipoOperacaoDescricao = "Salvou";
            else if (tipoOperacao == EDITAR) tipoOperacaoDescricao = "Editou";
            else if (tipoOperacao == EXCLUIR) tipoOperacaoDescricao = "Excluiu";
            else if (tipoOperacao == VISUALIZAR) tipoOperacaoDescricao = "Visualizou";
            else if (tipoOperacao == PESQUISAR) tipoOperacaoDescricao = "Pesquisou";
            if (tipoOperacao == PESQUISAR)
            {
                logAcesso.LogAcessoDescricao = tipoOperacaoDescricao + " dados da classe " + classe.GetType().Name;
            }
            else
            {
                logAcesso.LogAcessoDescricao = tipoOperacaoDescricao + " o objeto com id=idObjeto da classe " + classe.GetType().Name;
            }
            return logAcesso;
        }

        public static LogAcesso ConfigurarLogAcesso(int usuarioCodigo, string tipoOperacao, int idObjeto, object classe)
        {
            LogAcesso logAcesso = new LogAcesso();
            logAcesso.UsuarioCodigo = usuarioCodigo;
            logAcesso.LogAcessoTipoOperacao = tipoOperacao;
            logAcesso.LogAcessoDataOperacao = DateTime.Now;
            logAcesso.LogAcessoNomeTabela = classe.GetType().Name;

            string tipoOperacaoDescricao = "";
            if (tipoOperacao == SALVAR) tipoOperacaoDescricao = "Salvou";
            else if (tipoOperacao == EDITAR) tipoOperacaoDescricao = "Editou";
            else if (tipoOperacao == EXCLUIR) tipoOperacaoDescricao = "Excluiu";
            else if (tipoOperacao == VISUALIZAR) tipoOperacaoDescricao = "Visualizou";
            else if (tipoOperacao == PESQUISAR) tipoOperacaoDescricao = "Pesquisou";
            if (tipoOperacao == PESQUISAR)
            {
                logAcesso.LogAcessoDescricao = tipoOperacaoDescricao + " dados da classe " + classe.GetType().Name;
            }
            else {
                logAcesso.LogAcessoDescricao = tipoOperacaoDescricao + " o objeto com id=" + idObjeto + " da classe " + classe.GetType().Name;
            }
            return logAcesso;
        }
        public static LogAcesso ConfigurarLogAcessoErro(int usuarioCodigo, string tipoOperacao, object classe)
        {
            LogAcesso logAcesso = new LogAcesso();
            logAcesso.UsuarioCodigo = usuarioCodigo;
            logAcesso.LogAcessoTipoOperacao = tipoOperacao;
            logAcesso.LogAcessoDataOperacao = DateTime.Now;
            logAcesso.LogAcessoDescricao = "Erro ao tentar " + tipoOperacao + " o objeto da classe " + classe.GetType().Name + " com os parâmetros " + JsonConvert.SerializeObject(classe);
            return logAcesso;
        }
    }
}