using M0.Models.Entity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("USUARIO", Schema = "M0")]
    public class Usuario
    {
        [Key]
        [Column("Codigo")]
        public int Codigo { get; set; }

        [Required]
        [DisplayName("Login:")]
        [StringLength(100)]
        public string Login { get; set; }

        [DisplayName("Senha:")]
        [StringLength(100)]
        public string Senha { get; set; }

        [ForeignKey("PESSOA")]
        [Column("Pessoa")]
        public int Pessoa { get; set; }

        [Column("Tecnico")]
        public int Tecnico { get; set; }

        [DisplayName("Tipo:")]
        [StringLength(1)]
        [Column("Tipo")]
        public string Tipo { get; set; }

        [StringLength(200)]
        [Column("Dominio")]
        public string Dominio { get; set; }

        [StringLength(100)]
        [DisplayName("Email:")]
        [DataType(DataType.EmailAddress)]
        [Column("Email")]
        public string Email { get; set; }

        [StringLength(1)]
        [Column("Recuperou_Senha")]
        public string Recuperou_Senha { get; set; }

        [StringLength(1)]
        [Column("Status")]
        public string Status { get; set; }

        [Editable(false)]
        public string[] roles { get; set; }

        //public Pessoa pessoa { get; set; }

        [Editable(false)]
        public string TipoConexao { get; set; }

        [Editable(false)]
        public string Nome { get; set; }

        [Editable(false)]
        public string Celular { get; set; }

        [Editable(false)]
        public string DataNascimento { get; set; }

        [Editable(false)]
        public string Cpf { get; set; }

        public List<Modulo> Modulos { get; set; }

    }
}