using Microsoft.AspNetCore.Mvc;
using BarbeariaFeuRosa.Models;
using BarbeariaFeuRosa.Data;

namespace BarbeariaFeuRosa.Controllers
{
    public class BarbeirosController : Controller
    {
        private readonly AppDbContext _context;

        public BarbeirosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var barbeiros = _context.Barbeiros
                .OrderBy(b => b.Nome)
                .ToList();

            return View(barbeiros);
        }

        public IActionResult Novo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Novo(Barbeiro barbeiro, string usuarioLogin, string senha)
        {
            if (string.IsNullOrWhiteSpace(usuarioLogin))
            {
                ModelState.AddModelError("", "Informe o usuário de login do barbeiro.");
            }

            if (string.IsNullOrWhiteSpace(senha))
            {
                ModelState.AddModelError("", "Informe a senha do barbeiro.");
            }

            if (!string.IsNullOrWhiteSpace(senha) && senha.Length > 6)
            {
                ModelState.AddModelError("", "A senha deve ter no máximo 6 caracteres.");
            }

            bool usuarioJaExiste = _context.Usuarios
                .Any(u => u.UsuarioLogin == usuarioLogin);

            if (usuarioJaExiste)
            {
                ModelState.AddModelError("", "Este usuário de login já existe.");
            }

            if (!ModelState.IsValid)
            {
                return View(barbeiro);
            }

            _context.Barbeiros.Add(barbeiro);
            _context.SaveChanges();

            var usuario = new Usuario
            {
                Nome = barbeiro.Nome,
                UsuarioLogin = usuarioLogin,
                Senha = senha,
                Tipo = "BARBEIRO",
                BarbeiroId = barbeiro.Id
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            TempData["Sucesso"] = "Barbeiro e login criados com sucesso!";

            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            var barbeiro = _context.Barbeiros.Find(id);

            if (barbeiro == null)
            {
                TempData["Erro"] = "Barbeiro não encontrado.";
                return RedirectToAction("Index");
            }

            return View(barbeiro);
        }

        [HttpPost]
        public IActionResult Editar(Barbeiro barbeiro)
        {
            if (ModelState.IsValid)
            {
                _context.Barbeiros.Update(barbeiro);
                _context.SaveChanges();

                TempData["Sucesso"] = "Barbeiro atualizado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(barbeiro);
        }

        public IActionResult Excluir(int id)
        {
            var barbeiro = _context.Barbeiros.Find(id);

            if (barbeiro != null)
            {
                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.BarbeiroId == barbeiro.Id);

                if (usuario != null)
                {
                    _context.Usuarios.Remove(usuario);
                }

                _context.Barbeiros.Remove(barbeiro);
                _context.SaveChanges();

                TempData["Sucesso"] = "Barbeiro excluído com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Barbeiro não encontrado.";
            }

            return RedirectToAction("Index");
        }
    }
}