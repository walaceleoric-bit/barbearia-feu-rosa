using System.ComponentModel.DataAnnotations;

namespace BarbeariaFeuRosa.Models
{
    public class Barbeiro
    {
        public int Id { get; set; }

        public int BarbeariaId { get; set; } = 1;

        public Barbearia Barbearia { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string FotoUrl { get; set; } = string.Empty;

        public string Especialidade { get; set; } = string.Empty;

        public decimal ComissaoPercentual { get; set; }

        public bool Ativo { get; set; } = true;
    }
}