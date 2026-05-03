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

        public string Plano { get; set; } = "Mensal";

        public decimal ValorMensalidade { get; set; } = 49.90m;

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public DateTime DataVencimento { get; set; }
            = DateTime.UtcNow.AddDays(30);

        public bool PagamentoEmDia { get; set; } = true;

        public string Observacao { get; set; } = string.Empty;
    }
}