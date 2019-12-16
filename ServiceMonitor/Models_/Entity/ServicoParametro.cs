using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace M0.Models
{
    [Table("SERVICO_EXEMPLO", Schema = "M0")]
    public class ServicoParametro
    {
        [Key]
        [Column("Codigo")]
        public int Codigo { get; set; }

        [ForeignKey("Servico")]
        [Column("Servico")]
        public int Servico { get; set; }

        [Column("Nome")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Column("Obrigatorio")]
        [StringLength(1)]
        public string Obrigatorio { get; set; }

        [Column("Descricao")]
        [StringLength(100)]
        public string Descricao { get; set; }
        
        [Column("Tipo")]
        [StringLength(1)]
        public string Tipo { get; set; }

        [Column("Tipo_Dado")]
        [StringLength(1)]
        public string Tipo_Dado { get; set; }
        
        [Column("Dominio")]
        public string Dominio { get; set; }

        [Column("Criacao_Usuario")]
        public int Criacao_Usuario { get; set; }

        [Column("Criacao_Data")]
        public DateTime Criacao_Data { get; set; }

        [Column("Alteracao_Usuario")]
        public int Alteracao_Usuario { get; set; }

        [Column("Alteracao_Data")]
        public DateTime Alteracao_Data { get; set; }

        [Column("Exclusao_Usuario")]
        public int Exclusao_Usuario { get; set; }

        [Column("Exclusao_Data")]
        public DateTime Exclusao_Data { get; set; }

    }
}    