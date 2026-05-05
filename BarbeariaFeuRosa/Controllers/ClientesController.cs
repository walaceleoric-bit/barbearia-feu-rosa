using Microsoft.AspNetCore.Mvc;
using BarbeariaFeuRosa.Models;
using BarbeariaFeuRosa.Data;

namespace BarbeariaFeuRosa.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        private int? ObterBarbeariaId()
        {
            return HttpContext.Session.GetInt32("BarbeariaId");
        }

        public IActionResult Index()
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var clientes = _context.Clientes
                .Where(c => c.BarbeariaId == barbeariaId.Value)
                .OrderBy(c => c.Nome)
                .ToList();

            return View(clientes);
        }

        public IActionResult Novo()
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public IActionResult Novo(Cliente cliente, string usuarioLogin, string senha)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            cliente.BarbeariaId = barbeariaId.Value;

            ModelState.Remove("Barbearia");
            ModelState.Remove("BarbeariaId");

            if (string.IsNullOrWhiteSpace(usuarioLogin))
                ModelState.AddModelError("", "Informe o usuário de login do cliente.");

            if (string.IsNullOrWhiteSpace(senha))
                ModelState.AddModelError("", "Informe a senha do cliente.");

            if (!string.IsNullOrWhiteSpace(senha) && senha.Length > 6)
                ModelState.AddModelError("", "A senha deve ter no máximo 6 caracteres.");

            bool usuarioJaExiste = _context.Usuarios
                .Any(u =>
                    u.UsuarioLogin == usuarioLogin &&
                    u.BarbeariaId == barbeariaId.Value);

            if (usuarioJaExiste)
                ModelState.AddModelError("", "Este usuário de login já existe nesta barbearia.");

            if (!ModelState.IsValid)
                return View(cliente);

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            var usuario = new Usuario
            {
                Nome = cliente.Nome,
                UsuarioLogin = usuarioLogin,
                Senha = senha,
                Tipo = "CLIENTE",
                BarbeariaId = barbeariaId.Value,
                ClienteId = cliente.Id
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            TempData["Sucesso"] = "Cliente e login criados com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var cliente = _context.Clientes
                .FirstOrDefault(c =>
                    c.Id == id &&
                    c.BarbeariaId == barbeariaId.Value);

            if (cliente == null)
            {
                TempData["Erro"] = "Cliente não encontrado.";
                return RedirectToAction("Index");
            }

            var usuario = _context.Usuarios
                .FirstOrDefault(u =>
                    u.ClienteId == cliente.Id &&
                    u.BarbeariaId == barbeariaId.Value &&
                    u.Tipo == "CLIENTE");

            ViewBag.UsuarioLogin = usuario?.UsuarioLogin ?? "";
            ViewBag.Senha = usuario?.Senha ?? "";

            return View(cliente);
        }

        [HttpPost]
        public IActionResult Editar(Cliente cliente, string usuarioLogin, string senha)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            cliente.BarbeariaId = barbeariaId.Value;

            ModelState.Remove("Barbearia");
            ModelState.Remove("BarbeariaId");

            if (string.IsNullOrWhiteSpace(usuarioLogin))
                ModelState.AddModelError("", "Informe o usuário de login do cliente.");

            if (string.IsNullOrWhiteSpace(senha))
                ModelState.AddModelError("", "Informe a senha do cliente.");

            if (!string.IsNullOrWhiteSpace(senha) && senha.Length > 6)
                ModelState.AddModelError("", "A senha deve ter no máximo 6 caracteres.");

            bool usuarioJaExiste = _context.Usuarios
                .Any(u =>
                    u.UsuarioLogin == usuarioLogin &&
                    u.BarbeariaId == barbeariaId.Value &&
                    u.ClienteId != cliente.Id);

            if (usuarioJaExiste)
                ModelState.AddModelError("", "Este usuário de login já existe nesta barbearia.");

            if (!ModelState.IsValid)
            {
                ViewBag.UsuarioLogin = usuarioLogin;
                ViewBag.Senha = senha;
                return View(cliente);
            }

            var clienteBanco = _context.Clientes
                .FirstOrDefault(c =>
                    c.Id == cliente.Id &&
                    c.BarbeariaId == barbeariaId.Value);

            if (clienteBanco == null)
            {
                TempData["Erro"] = "Cliente não encontrado.";
                return RedirectToAction("Index");
            }

            clienteBanco.Nome = cliente.Nome;
            clienteBanco.WhatsApp = cliente.WhatsApp;

            var usuario = _context.Usuarios
                .FirstOrDefault(u =>
                    u.ClienteId == cliente.Id &&
                    u.BarbeariaId == barbeariaId.Value &&
                    u.Tipo == "CLIENTE");

            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Tipo = "CLIENTE",
                    BarbeariaId = barbeariaId.Value,
                    ClienteId = cliente.Id
                };

                _context.Usuarios.Add(usuario);
            }

            usuario.Nome = cliente.Nome;
            usuario.UsuarioLogin = usuarioLogin;
            usuario.Senha = senha;

            _context.SaveChanges();

            TempData["Sucesso"] = "Cliente atualizado com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Excluir(int id)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var cliente = _context.Clientes
                .FirstOrDefault(c =>
                    c.Id == id &&
                    c.BarbeariaId == barbeariaId.Value);

            if (cliente != null)
            {
                var usuario = _context.Usuarios
                    .FirstOrDefault(u =>
                        u.ClienteId == cliente.Id &&
                        u.BarbeariaId == barbeariaId.Value);

                if (usuario != null)
                    _context.Usuarios.Remove(usuario);

                _context.Clientes.Remove(cliente);
                _context.SaveChanges();

                TempData["Sucesso"] = "Cliente excluído com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Cliente não encontrado.";
            }

            return RedirectToAction("Index");
        }
    }
}