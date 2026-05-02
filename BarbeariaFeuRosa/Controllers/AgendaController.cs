using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using BarbeariaFeuRosa.Data;
using BarbeariaFeuRosa.Models;

namespace BarbeariaFeuRosa.Controllers
{
    public class AgendaController : Controller
    {
        private readonly AppDbContext _context;

        public AgendaController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var agendamentos = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .OrderBy(a => a.DataHora)
                .ToList();

            return View(agendamentos);
        }

        public IActionResult Novo()
        {
            CarregarCombos();
            return View();
        }

        [HttpPost]
        public IActionResult Novo(Agendamento agendamento)
        {
            ModelState.Remove("Cliente");
            ModelState.Remove("Barbeiro");

            if (ModelState.IsValid)
            {
                agendamento.Status = "Agendado";

                // Corrige data para o PostgreSQL
                agendamento.DataHora = DateTime.SpecifyKind(agendamento.DataHora, DateTimeKind.Utc);

                _context.Agendamentos.Add(agendamento);
                _context.SaveChanges();

                TempData["Sucesso"] = "Agendamento salvo com sucesso!";
                return RedirectToAction("Index");
            }

            CarregarCombos();
            return View(agendamento);
        }

        public IActionResult Excluir(int id)
        {
            var agendamento = _context.Agendamentos.Find(id);

            if (agendamento != null)
            {
                _context.Agendamentos.Remove(agendamento);
                _context.SaveChanges();

                TempData["Sucesso"] = "Agendamento excluído com sucesso!";
            }

            return RedirectToAction("Index");
        }

        private void CarregarCombos()
        {
            ViewBag.Clientes = new SelectList(
                _context.Clientes.OrderBy(c => c.Nome),
                "Id",
                "Nome"
            );

            ViewBag.Barbeiros = new SelectList(
                _context.Barbeiros.Where(b => b.Ativo).OrderBy(b => b.Nome),
                "Id",
                "Nome"
            );

            ViewBag.Servicos = new List<SelectListItem>
            {
                new SelectListItem { Value = "Corte", Text = "Corte" },
                new SelectListItem { Value = "Barba", Text = "Barba" },
                new SelectListItem { Value = "Corte + Barba", Text = "Corte + Barba" },
                new SelectListItem { Value = "Sobrancelha", Text = "Sobrancelha" },
                new SelectListItem { Value = "Pigmentação", Text = "Pigmentação" },
                new SelectListItem { Value = "Acabamento", Text = "Acabamento" }
            };
        }
    }
}