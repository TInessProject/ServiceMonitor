using System.ComponentModel.DataAnnotations;

namespace M0.Models.Entity
{
    public class Modulo
    {
        [Editable(false)]
        public int Codigo { get; set; }

        [Editable(false)]
        public string Rotulo { get; set; }

        [Editable(false)]
        public string Icone { get; set; }

        [Editable(false)]
        public string Nome { get; set; }
    }
}