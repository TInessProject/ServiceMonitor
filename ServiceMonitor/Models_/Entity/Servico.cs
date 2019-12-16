using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace M0.Models
{
    public class Servico
    {
        [Table("SERVICO", Schema = "M0")]
        public class Especialidade
        {
            [Key]
            [Column("Codigo")]
            public int Codigo { get; set; }

            [Column("Nome")]
            [StringLength(100)]
            public string Nome { get; set; }

            [Column("Url")]
            [StringLength(1000)]
            public string Url { get; set; }

            [Column("Metodo")]
            [StringLength(10)]
            public string Metodo { get; set; }

            [Column("Formato")]
            [StringLength(10)]
            public string Formato { get; set; }

            [Column("Retorno_Sucesso")]
            public string Retorno_Sucesso { get; set; }

            [Column("Retorno_Erro")]
            public string Retorno_Erro { get; set; }

            [ForeignKey("Tipo_Servico")]
            [Column("Tipo_Servico")]
            public int Tipo_Servico { get; set; }
            
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
}