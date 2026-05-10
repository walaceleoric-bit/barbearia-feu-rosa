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
        public IActionResult Novo(Agendamento agendamento, int servicoId)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            agendamento.BarbeariaId = barbeariaId.Value;

            ModelState.Remove("Cliente");
            ModelState.Remove("Barbeiro");
            ModelState.Remove("Barbearia");
            ModelState.Remove("BarbeariaId");
            ModelState.Remove("Servico");

            bool clienteExiste = _context.Clientes
                .Any(c =>
                    c.Id == agendamento.ClienteId &&
                    c.BarbeariaId == barbeariaId.Value);

            bool barbeiroExiste = _context.Barbeiros
                .Any(b =>
                    b.Id == agendamento.BarbeiroId &&
                    b.BarbeariaId == barbeariaId.Value &&
                    b.Ativo);

            var servicoSelecionado = _context.Servicos
                .FirstOrDefault(s =>
                    s.Id == servicoId &&
                    s.BarbeariaId == barbeariaId.Value &&
                    s.Ativo);

            if (!clienteExiste)
                ModelState.AddModelError("", "Cliente inválido para esta barbearia.");

            if (!barbeiroExiste)
                ModelState.AddModelError("", "Barbeiro inválido para esta barbearia.");

            if (servicoSelecionado == null)
                ModelState.AddModelError("", "Serviço inválido para esta barbearia.");

            if (ModelState.IsValid)
            {
                agendamento.Status = "Agendado";

                agendamento.Servico = servicoSelecionado!.Nome;

                agendamento.Valor = servicoSelecionado.Valor;

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
                    .OrderBy(c => c.Nome)
                    .ToList(),
                "Id",
                "Nome"
            );

            ViewBag.Barbeiros = new SelectList(
                _context.Barbeiros
                    .Where(b =>
                        b.BarbeariaId == barbeariaId &&
                        b.Ativo)
                    .OrderBy(b => b.Nome)
                    .ToList(),
                "Id",
                "Nome"
            );

            ViewBag.Servicos = new SelectList(
                _context.Servicos
                    .Where(s =>
                        s.BarbeariaId == barbeariaId &&
                        s.Ativo)
                    .OrderBy(s => s.Nome)
                    .Select(s => new
                    {
                        Nome = s.Nome,
                        NomeValor = s.Nome + " - R$ " + s.Valor.ToString("N2")
                    })
                    .ToList(),
                "Nome",
                "NomeValor"
            );
        }
    }
}