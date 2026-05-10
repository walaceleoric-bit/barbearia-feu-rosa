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

        private int? ObterBarbeariaId()
        {
            return HttpContext.Session.GetInt32("BarbeariaId");
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            CarregarTelaCliente(barbeariaId.Value);

            ViewBag.NomeCliente =
    HttpContext.Session.GetString("UsuarioNome");

            ViewBag.Servicos = _context.Servicos
                .Where(s =>
                    s.BarbeariaId == barbeariaId.Value &&
                    s.Ativo)
                .OrderBy(s => s.Nome)
                .ToList();

            ViewBag.ModoPreview = false;

            return View();
        }

        public IActionResult Preview()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "ADM")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            CarregarTelaCliente(barbeariaId.Value);

            ViewBag.Servicos = _context.Servicos
                .Where(s =>
                    s.BarbeariaId == barbeariaId.Value &&
                    s.Ativo)
                .OrderBy(s => s.Nome)
                .ToList();

            ViewBag.NomeCliente = "Cliente Visitante";
            ViewBag.ModoPreview = true;

            return View("Index");
        }

        private void CarregarTelaCliente(int barbeariaId)
        {
            var config = _context.Configuracoes
                .FirstOrDefault(c => c.BarbeariaId == barbeariaId);

            ViewBag.NomeBarbearia =
                config?.NomeBarbearia ?? "Minha Barbearia";

            ViewBag.Promocao =
                config?.PromocaoDestaque ?? "Corte + Barba com preço especial.";

            ViewBag.Logo = config?.LogoUrl;

            ViewBag.Imagem1 = config?.CarrosselImagem1;
            ViewBag.Imagem2 = config?.CarrosselImagem2;
            ViewBag.Imagem3 = config?.CarrosselImagem3;
            ViewBag.Imagem4 = config?.CarrosselImagem4;
            ViewBag.Imagem5 = config?.CarrosselImagem5;
        }
    }
}