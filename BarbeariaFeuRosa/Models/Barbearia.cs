using System.ComponentModel.DataAnnotations;

namespace BarbeariaFeuRosa.Models
{
    public class Barbearia
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Slug { get; set; } = string.Empty;

        public string LogoUrl { get; set; } = string.Empty;

        public bool Ativa { get; set; } = true;
    }
}