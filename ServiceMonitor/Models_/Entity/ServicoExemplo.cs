using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace M0.Models
{
    [Table("SERVICO_EXEMPLO", Schema = "M0")]
    public class ServicoExemplo
    {
        [Key]
        [Column("Codigo")]
        public int Codigo { get; set; }

        [ForeignKey("Servico")]
        [Column("Servico")]
        public int Servico { get; set; }

        [Column("Linguagem")]
        [StringLength(20)]
        public string Linguagem { get; set; }

        [Column("Url")]
        [StringLength(1000)]
        public string Url { get; set; }

        [Column("Exemplo")]
        public string Exemplo { get; set; }

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