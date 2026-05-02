using System.ComponentModel.DataAnnotations;

namespace BarbeariaFeuRosa.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string WhatsApp { get; set; } = string.Empty;
    }
}