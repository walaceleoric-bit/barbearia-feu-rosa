using Microsoft.AspNetCore.Mvc;
using BarbeariaFeuRosa.Data;
using BarbeariaFeuRosa.Models;

namespace BarbeariaFeuRosa.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly AppDbContext _context;

        private const string UsuarioSuper = "dono";
        private const string SenhaSuper = "123456";

        public SuperAdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string usuario, string senha)
        {
            if (usuario == UsuarioSuper && senha == SenhaSuper)
            {
                HttpContext.Session.SetString("SuperAdmin", "SIM");
                return RedirectToAction("Index");
            }

            ViewBag.Erro = "Usuário ou senha inválidos.";
            return View();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("SuperAdmin") != "SIM")
                return RedirectToAction("Login");

            var barbearias = _context.Barbearias
                .OrderBy(b => b.Nome)
                .ToList();

            return View(barbearias);
        }

        public IActionResult Nova()
        {
            if (HttpContext.Session.GetString("SuperAdmin") != "SIM")
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public IActionResult Nova(Barbearia barbearia, string usuarioAdm, string senhaAdm)
        {
            if (HttpContext.Session.GetString("SuperAdmin") != "SIM")
                return RedirectToAction("Login");

            barbearia.Slug = barbearia.Slug
                .Trim()
                .ToLower()
                .Replace(" ", "-");

            bool slugExiste = _context.Barbearias
                .Any(b => b.Slug == barbearia.Slug);

            if (slugExiste)
            {
                ViewBag.Erro = "Este slug já está em uso.";
                return View(barbearia);
            }

            _context.Barbearias.Add(barbearia);
            _context.SaveChanges();

            var usuario = new Usuario
            {
                Nome = "Administrador",
                UsuarioLogin = usuarioAdm,
                Senha = senhaAdm,
                Tipo = "ADM",
                BarbeariaId = barbearia.Id
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            TempData["Sucesso"] = "Barbearia criada com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            if (HttpContext.Session.GetString("SuperAdmin") != "SIM")
                return RedirectToAction("Login");

            var barbearia = _context.Barbearias.Find(id);

            if (barbearia == null)
            {
                TempData["Erro"] = "Barbearia não encontrada.";
                return RedirectToAction("Index");
            }

            return View(barbearia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Barbearia barbearia)
        {
            if (HttpContext.Session.GetString("SuperAdmin") != "SIM")
                return RedirectToAction("Login");

            var barbeariaBanco = _context.Barbearias
                .FirstOrDefault(b => b.Id == barbearia.Id);

            if (barbeariaBanco == null)
            {
                TempData["Erro"] = "Barbearia não encontrada.";
                return RedirectToAction("Index");
            }

            var novoSlug = barbearia.Slug
                .Trim()
                .ToLower()
                .Replace(" ", "-");

            bool slugExiste = _context.Barbearias
                .Any(b => b.Slug == novoSlug && b.Id != barbearia.Id);

            if (slugExiste)
            {
                TempData["Erro"] = "Este slug já está sendo usado por outra barbearia.";
                return View(barbearia);
            }

            barbeariaBanco.Nome = barbearia.Nome;
            barbeariaBanco.Slug = novoSlug;
            barbeariaBanco.Plano = barbearia.Plano;
            barbeariaBanco.ValorMensalidade = barbearia.ValorMensalidade;
            barbeariaBanco.DataVencimento = DateTime.SpecifyKind(
                barbearia.DataVencimento,
                DateTimeKind.Utc
            );
            barbeariaBanco.Observacao = barbearia.Observacao;
            barbeariaBanco.Ativa = barbearia.Ativa;
            barbeariaBanco.PagamentoEmDia = barbearia.PagamentoEmDia;

            _context.SaveChanges();

            TempData["Sucesso"] = "Barbearia atualizada com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Bloquear(int id)
        {
            if (HttpContext.Session.GetString("SuperAdmin") != "SIM")
                return RedirectToAction("Login");

            var barbearia = _context.Barbearias.Find(id);

            if (barbearia != null)
            {
                barbearia.Ativa = false;
                barbearia.PagamentoEmDia = false;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Liberar(int id)
        {
            if (HttpContext.Session.GetString("SuperAdmin") != "SIM")
                return RedirectToAction("Login");

            var barbearia = _context.Barbearias.Find(id);

            if (barbearia != null)
            {
                barbearia.Ativa = true;
                barbearia.PagamentoEmDia = true;
                barbearia.DataVencimento = DateTime.UtcNow.AddDays(30);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            if (HttpContext.Session.GetString("SuperAdmin") != "SIM")
                return RedirectToAction("Login");

            var barbearia = _context.Barbearias.Find(id);

            if (barbearia == null)
            {
                TempData["Erro"] = "Barbearia não encontrada.";
                return RedirectToAction("Index");
            }

            try
            {
                _context.Barbearias.Remove(barbearia);
                _context.SaveChanges();

                TempData["Sucesso"] = "Barbearia excluída com sucesso!";
            }
            catch
            {
                TempData["Erro"] = "Não foi possível excluir esta barbearia porque existem dados vinculados a ela.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Remove("SuperAdmin");
            return RedirectToAction("Login");
        }
    }
}