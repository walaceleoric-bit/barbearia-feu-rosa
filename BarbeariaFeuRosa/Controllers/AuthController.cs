using Microsoft.AspNetCore.Mvc;
using BarbeariaFeuRosa.Data;
using BarbeariaFeuRosa.Models;

namespace BarbeariaFeuRosa.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        private const int BarbeariaAtualId = 1;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            CarregarConfiguracoes();

            if (TempData["Bloqueio"] != null)
                ViewBag.Erro = TempData["Bloqueio"];

            return View();
        }

        [HttpPost]
        public IActionResult Login(string usuarioLogin, string senha)
        {
            CarregarConfiguracoes();

            if (usuarioLogin == "dono" && senha == "123456")
            {
                HttpContext.Session.SetString("SuperAdmin", "SIM");
                return RedirectToAction("Index", "SuperAdmin");
            }

            var barbearia = _context.Barbearias
                .FirstOrDefault(b => b.Id == BarbeariaAtualId);

            if (barbearia == null || !barbearia.Ativa || !barbearia.PagamentoEmDia)
            {
                ViewBag.Erro = "Sistema bloqueado. Entre em contato com o administrador.";
                return View();
            }

            var usuario = _context.Usuarios
                .FirstOrDefault(u =>
                    u.UsuarioLogin == usuarioLogin &&
                    u.Senha == senha &&
                    u.BarbeariaId == BarbeariaAtualId);

            if (usuario == null)
            {
                ViewBag.Erro = "Usuário ou senha inválidos.";
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioNome", usuario.UsuarioLogin);
            HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo);
            HttpContext.Session.SetInt32("BarbeariaId", usuario.BarbeariaId);

            if (usuario.BarbeiroId.HasValue)
                HttpContext.Session.SetInt32("BarbeiroId", usuario.BarbeiroId.Value);

            if (usuario.Tipo == "ADM")
                return RedirectToAction("Index", "Dashboard");

            if (usuario.Tipo == "BARBEIRO")
                return RedirectToAction("Index", "PainelBarbeiro");

            return RedirectToAction("Index", "ClienteHome");
        }

        public IActionResult Cadastro()
        {
            CarregarConfiguracoes();
            return View();
        }

        [HttpPost]
        public IActionResult Cadastro(Usuario usuario)
        {
            CarregarConfiguracoes();

            var barbearia = _context.Barbearias
                .FirstOrDefault(b => b.Id == BarbeariaAtualId);

            if (barbearia == null || !barbearia.Ativa || !barbearia.PagamentoEmDia)
            {
                ViewBag.Erro = "Sistema bloqueado. Cadastro indisponível.";
                return View(usuario);
            }

            usuario.Nome = usuario.UsuarioLogin;
            usuario.Tipo = "CLIENTE";
            usuario.BarbeariaId = BarbeariaAtualId;

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
                    u.BarbeariaId == BarbeariaAtualId);

            if (existe)
            {
                ViewBag.Erro = "Este usuário já existe.";
                return View(usuario);
            }

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            TempData["Sucesso"] = "Cadastro realizado com sucesso. Faça login.";

            return RedirectToAction("Login");
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private void CarregarConfiguracoes()
        {
            var config = _context.Configuracoes.FirstOrDefault();

            ViewBag.NomeBarbearia =
                config?.NomeBarbearia ?? "Barbearia Feu Rosa";
        }
    }
}