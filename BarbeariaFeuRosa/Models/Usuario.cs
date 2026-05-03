using System.ComponentModel.DataAnnotations;

namespace BarbeariaFeuRosa.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string UsuarioLogin { get; set; } = string.Empty;

        [Required]
        [StringLength(6)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        public string Tipo { get; set; } = "CLIENTE";

        public int BarbeariaId { get; set; } = 1;

        public Barbearia? Barbearia { get; set; }

        public int? BarbeiroId { get; set; }

        public Barbeiro? Barbeiro { get; set; }
    }
}