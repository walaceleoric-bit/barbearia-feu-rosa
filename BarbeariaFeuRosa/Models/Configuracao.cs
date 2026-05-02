namespace BarbeariaFeuRosa.Models
{
    public class Configuracao
    {
        public int Id { get; set; }

        public string NomeBarbearia { get; set; } = "Barbearia Feu Rosa";

        public string PromocaoDestaque { get; set; } = "Corte + Barba com preço especial.";

        public string? LogoUrl { get; set; }

        public string? CarrosselImagem1 { get; set; }
        public string? CarrosselImagem2 { get; set; }
        public string? CarrosselImagem3 { get; set; }
        public string? CarrosselImagem4 { get; set; }
        public string? CarrosselImagem5 { get; set; }
    }
}