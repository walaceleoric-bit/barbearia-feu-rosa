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

        // LOGIN
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string usuarioLogin, string senha)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u =>
                    u.UsuarioLogin == usuarioLogin &&
                    u.Senha == senha);

            if (usuario == null)
            {
                ViewBag.Erro = "Usuário ou senha inválidos.";
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioNome", usuario.UsuarioLogin);
            HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo);

            if (usuario.BarbeiroId.HasValue)
            {
                HttpContext.Session.SetInt32("BarbeiroId", usuario.BarbeiroId.Value);
            }

            // FLUXO POR PERFIL
            if (usuario.Tipo == "ADM")
                return RedirectToAction("Index", "Dashboard");

            if (usuario.Tipo == "BARBEIRO")
                return RedirectToAction("Index", "PainelBarbeiro");

            // CLIENTE
            return RedirectToAction("Index", "ClienteHome");
        }

        // CADASTRO
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastro(Usuario usuario)
        {
            usuario.Nome = usuario.UsuarioLogin;
            usuario.Tipo = "CLIENTE";

            ModelState.Remove("Nome");
            ModelState.Remove("Tipo");
            ModelState.Remove("BarbeiroId");
            ModelState.Remove("Barbeiro");

            if (!ModelState.IsValid)
                return View(usuario);

            bool existe = _context.Usuarios
                .Any(u => u.UsuarioLogin == usuario.UsuarioLogin);

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

        // LOGOUT
        public IActionResult Sair()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}