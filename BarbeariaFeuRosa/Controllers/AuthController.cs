using Microsoft.AspNetCore.Mvc;
using BarbeariaFeuRosa.Data;
using BarbeariaFeuRosa.Models;

namespace BarbeariaFeuRosa.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login(string? barbeariaSlug)
        {
            var slugAtual = ObterSlugAtual(barbeariaSlug);

            CarregarConfiguracoes(slugAtual);

            if (TempData["Bloqueio"] != null)
                ViewBag.Erro = TempData["Bloqueio"];

            return View();
        }

        [HttpPost]
        public IActionResult Login(string usuarioLogin, string senha, string? barbeariaSlug)
        {
            var slugAtual = ObterSlugAtual(barbeariaSlug);

            CarregarConfiguracoes(slugAtual);

            if (usuarioLogin == "dono" && senha == "123456")
            {
                HttpContext.Session.SetString("SuperAdmin", "SIM");
                return RedirectToAction("Index", "SuperAdmin");
            }

            Barbearia? barbearia = null;

            if (!string.IsNullOrWhiteSpace(slugAtual))
            {
                barbearia = _context.Barbearias
                    .FirstOrDefault(b => b.Slug.ToLower() == slugAtual.ToLower());

                if (barbearia == null)
                {
                    ViewBag.Erro = "Barbearia não encontrada.";
                    return View();
                }

                if (!barbearia.Ativa || !barbearia.PagamentoEmDia)
                {
                    ViewBag.Erro = "Sistema bloqueado. Entre em contato com o administrador.";
                    return View();
                }

                var usuarioComSlug = _context.Usuarios
                    .FirstOrDefault(u =>
                        u.UsuarioLogin == usuarioLogin &&
                        u.Senha == senha &&
                        u.BarbeariaId == barbearia.Id);

                if (usuarioComSlug == null)
                {
                    ViewBag.Erro = "Usuário ou senha inválidos.";
                    return View();
                }

                SalvarSessao(usuarioComSlug, barbearia);

                return RedirecionarPorTipo(usuarioComSlug);
            }

            var usuario = _context.Usuarios
                .FirstOrDefault(u =>
                    u.UsuarioLogin == usuarioLogin &&
                    u.Senha == senha);

            if (usuario == null)
            {
                ViewBag.Erro = "Usuário ou senha inválidos.";
                return View();
            }

            barbearia = _context.Barbearias
                .FirstOrDefault(b => b.Id == usuario.BarbeariaId);

            if (barbearia == null)
            {
                ViewBag.Erro = "Barbearia não encontrada para este usuário.";
                return View();
            }

            if (!barbearia.Ativa || !barbearia.PagamentoEmDia)
            {
                ViewBag.Erro = "Sistema bloqueado. Entre em contato com o administrador.";
                return View();
            }

            SalvarSessao(usuario, barbearia);

            return RedirecionarPorTipo(usuario);
        }

        public IActionResult Cadastro(string? barbeariaSlug)
        {
            var slugAtual = ObterSlugAtual(barbeariaSlug);

            CarregarConfiguracoes(slugAtual);

            return View();
        }

        [HttpPost]
        public IActionResult Cadastro(Usuario usuario, string? barbeariaSlug)
        {
            var slugAtual = ObterSlugAtual(barbeariaSlug);

            CarregarConfiguracoes(slugAtual);

            if (string.IsNullOrWhiteSpace(slugAtual))
            {
                ViewBag.Erro = "Acesse o cadastro pela URL da barbearia.";
                return View(usuario);
            }

            var barbearia = _context.Barbearias
                .FirstOrDefault(b => b.Slug.ToLower() == slugAtual.ToLower());

            if (barbearia == null)
            {
                ViewBag.Erro = "Barbearia não encontrada.";
                return View(usuario);
            }

            if (!barbearia.Ativa || !barbearia.PagamentoEmDia)
            {
                ViewBag.Erro = "Sistema bloqueado. Cadastro indisponível.";
                return View(usuario);
            }

            usuario.Nome = usuario.UsuarioLogin;
            usuario.Tipo = "CLIENTE";
            usuario.BarbeariaId = barbearia.Id;

            ModelState.Remove("Nome");
            ModelState.Remove("Tipo");
            ModelState.Remove("Barbearia");
            ModelState.Remove("BarbeariaId");
            ModelState.Remove("BarbeiroId");
            ModelState.Remove("Barbeiro");

            if (!ModelState.IsValid)
                return View(usuario);

            bool existe = _context.Usuarios
                .Any(u =>
                    u.UsuarioLogin == usuario.UsuarioLogin &&
                    u.BarbeariaId == barbearia.Id);

            if (existe)
            {
                ViewBag.Erro = "Este usuário já existe nesta barbearia.";
                return View(usuario);
            }

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            TempData["Sucesso"] = "Cadastro realizado com sucesso. Faça login.";

            return Redirect($"/{barbearia.Slug}/Auth/Login");
        }

        public IActionResult Sair()
        {
            var slug = HttpContext.Session.GetString("BarbeariaSlug");

            HttpContext.Session.Clear();

            if (!string.IsNullOrWhiteSpace(slug))
                return Redirect($"/{slug}/Auth/Login");

            return RedirectToAction("Login");
        }

        private string ObterSlugAtual(string? barbeariaSlug)
        {
            if (!string.IsNullOrWhiteSpace(barbeariaSlug))
                return barbeariaSlug.Trim().ToLower();

            var slugRota = RouteData.Values["barbeariaSlug"]?.ToString();

            if (!string.IsNullOrWhiteSpace(slugRota))
                return slugRota.Trim().ToLower();

            return "";
        }

        private void CarregarConfiguracoes(string slugAtual)
        {
            ViewBag.BarbeariaSlug = slugAtual;

            if (!string.IsNullOrWhiteSpace(slugAtual))
            {
                var barbearia = _context.Barbearias
                    .FirstOrDefault(b => b.Slug.ToLower() == slugAtual.ToLower());

                ViewBag.NomeBarbearia = barbearia?.Nome ?? "Barbearia Universo";
                return;
            }

            ViewBag.NomeBarbearia = "Barbearia Universo";
        }

        private void SalvarSessao(Usuario usuario, Barbearia barbearia)
        {
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioNome", usuario.UsuarioLogin);
            HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo);
            HttpContext.Session.SetInt32("BarbeariaId", usuario.BarbeariaId);
            HttpContext.Session.SetString("BarbeariaSlug", barbearia.Slug);

            if (usuario.BarbeiroId.HasValue)
                HttpContext.Session.SetInt32("BarbeiroId", usuario.BarbeiroId.Value);
        }

        private IActionResult RedirecionarPorTipo(Usuario usuario)
        {
            if (usuario.Tipo == "ADM")
                return RedirectToAction("Index", "Dashboard");

            if (usuario.Tipo == "BARBEIRO")
                return RedirectToAction("Index", "PainelBarbeiro");

            return RedirectToAction("Index", "ClienteHome");
        }
    }
}