using System.ComponentModel.DataAnnotations;

namespace BarbeariaFeuRosa.Models
{
    public class AvaliacaoBarbeiro
    {
        public int Id { get; set; }

        public int BarbeariaId { get; set; }

        public int BarbeiroId { get; set; }

        public int ClienteId { get; set; }

        [Range(1, 5)]
        public int Nota { get; set; }

        public string Comentario { get; set; } = string.Empty;

        public DateTime DataAvaliacao { get; set; }
            = DateTime.UtcNow;

        public Barbearia? Barbearia { get; set; }

        public Barbeiro? Barbeiro { get; set; }

        public Cliente? Cliente { get; set; }
    }
}