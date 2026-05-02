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

        public IActionResult Index()
        {
            var clientes = _context.Clientes
                .OrderBy(c => c.Nome)
                .ToList();

            return View(clientes);
        }

        public IActionResult Novo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Novo(Cliente cliente)
        {
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
            var cliente = _context.Clientes.Find(id);

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
            var cliente = _context.Clientes.Find(id);

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