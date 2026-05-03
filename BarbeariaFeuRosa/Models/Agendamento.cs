using System.ComponentModel.DataAnnotations;

namespace BarbeariaFeuRosa.Models
{
    public class Agendamento
    {
        public int Id { get; set; }

        public int BarbeariaId { get; set; } = 1;

        public Barbearia? Barbearia { get; set; }

        [Required]
        public int ClienteId { get; set; }

        public Cliente? Cliente { get; set; }

        [Required]
        public int BarbeiroId { get; set; }

        public Barbeiro? Barbeiro { get; set; }

        [Required]
        public string Servico { get; set; } = string.Empty;

        [Required]
        public DateTime DataHora { get; set; }

        public decimal Valor { get; set; }

        public string Status { get; set; } = "Agendado";
    }
}