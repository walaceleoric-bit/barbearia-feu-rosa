using System.ComponentModel.DataAnnotations;

namespace BarbeariaFeuRosa.Models
{
    public class Servico
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public decimal Valor { get; set; }

        public bool Ativo { get; set; } = true;

        public int BarbeariaId { get; set; }

        public Barbearia? Barbearia { get; set; }
    }
}