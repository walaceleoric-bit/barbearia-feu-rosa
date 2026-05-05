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
        public IActionResult Novo(Cliente cliente)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            cliente.BarbeariaId = barbeariaId.Value;

            ModelState.Remove("Barbearia");
            ModelState.Remove("BarbeariaId");

            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);
                _context.SaveChanges();

                TempData["Sucesso"] = "Cliente salvo com sucesso!";
                return RedirectToAction("Index");
            }

            return View(cliente);
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

            return View(cliente);
        }

        [HttpPost]
        public IActionResult Editar(Cliente cliente)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            cliente.BarbeariaId = barbeariaId.Value;

            ModelState.Remove("Barbearia");
            ModelState.Remove("BarbeariaId");

            if (ModelState.IsValid)
            {
                _context.Clientes.Update(cliente);
                _context.SaveChanges();

                TempData["Sucesso"] = "Cliente atualizado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(cliente);
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