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

        private int? ObterBarbeariaId()
        {
            return HttpContext.Session.GetInt32("BarbeariaId");
        }

        public IActionResult Index()
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var agendamentos = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Where(a => a.BarbeariaId == barbeariaId.Value)
                .OrderBy(a => a.DataHora)
                .ToList();

            return View(agendamentos);
        }

        public IActionResult Novo()
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            CarregarCombos(barbeariaId.Value);
            return View();
        }

        [HttpPost]
        public IActionResult Novo(Agendamento agendamento)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            agendamento.BarbeariaId = barbeariaId.Value;

            ModelState.Remove("Cliente");
            ModelState.Remove("Barbeiro");
            ModelState.Remove("Barbearia");
            ModelState.Remove("BarbeariaId");

            bool clienteExiste = _context.Clientes
                .Any(c =>
                    c.Id == agendamento.ClienteId &&
                    c.BarbeariaId == barbeariaId.Value);

            bool barbeiroExiste = _context.Barbeiros
                .Any(b =>
                    b.Id == agendamento.BarbeiroId &&
                    b.BarbeariaId == barbeariaId.Value &&
                    b.Ativo);

            if (!clienteExiste)
                ModelState.AddModelError("", "Cliente inválido para esta barbearia.");

            if (!barbeiroExiste)
                ModelState.AddModelError("", "Barbeiro inválido para esta barbearia.");

            if (ModelState.IsValid)
            {
                agendamento.Status = "Agendado";

                agendamento.DataHora = DateTime.SpecifyKind(
                    agendamento.DataHora,
                    DateTimeKind.Utc
                );

                _context.Agendamentos.Add(agendamento);
                _context.SaveChanges();

                TempData["Sucesso"] = "Agendamento salvo com sucesso!";
                return RedirectToAction("Index");
            }

            CarregarCombos(barbeariaId.Value);
            return View(agendamento);
        }

        public IActionResult Excluir(int id)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var agendamento = _context.Agendamentos
                .FirstOrDefault(a =>
                    a.Id == id &&
                    a.BarbeariaId == barbeariaId.Value);

            if (agendamento != null)
            {
                _context.Agendamentos.Remove(agendamento);
                _context.SaveChanges();

                TempData["Sucesso"] = "Agendamento excluído com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Agendamento não encontrado.";
            }

            return RedirectToAction("Index");
        }

        private void CarregarCombos(int barbeariaId)
        {
            ViewBag.Clientes = new SelectList(
                _context.Clientes
                    .Where(c => c.BarbeariaId == barbeariaId)
                    .OrderBy(c => c.Nome),
                "Id",
                "Nome"
            );

            ViewBag.Barbeiros = new SelectList(
                _context.Barbeiros
                    .Where(b =>
                        b.BarbeariaId == barbeariaId &&
                        b.Ativo)
                    .OrderBy(b => b.Nome),
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