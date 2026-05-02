using Microsoft.AspNetCore.Mvc;
using BarbeariaFeuRosa.Data;

namespace BarbeariaFeuRosa.Controllers
{
    public class ClienteHomeController : Controller
    {
        private readonly AppDbContext _context;

        public ClienteHomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var config = _context.Configuracoes.FirstOrDefault();

            ViewBag.NomeBarbearia = config?.NomeBarbearia ?? "Barbearia Feu Rosa";
            ViewBag.Promocao = config?.PromocaoDestaque ?? "Corte + Barba com preço especial.";
            ViewBag.Logo = config?.LogoUrl;

            ViewBag.Imagem1 = config?.CarrosselImagem1;
            ViewBag.Imagem2 = config?.CarrosselImagem2;
            ViewBag.Imagem3 = config?.CarrosselImagem3;
            ViewBag.Imagem4 = config?.CarrosselImagem4;
            ViewBag.Imagem5 = config?.CarrosselImagem5;

            ViewBag.NomeCliente =
                HttpContext.Session.GetString("UsuarioNome");

            return View();
        }
    }
}