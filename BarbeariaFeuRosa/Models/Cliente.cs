using System.ComponentModel.DataAnnotations;

namespace BarbeariaFeuRosa.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public int BarbeariaId { get; set; } = 1;

        public Barbearia? Barbearia { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string WhatsApp { get; set; } = string.Empty;
    }
}