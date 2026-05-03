using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarbeariaFeuRosa.Data;

namespace BarbeariaFeuRosa.Controllers
{
    public class PainelBarbeiroController : Controller
    {
        private readonly AppDbContext _context;

        private const int BarbeariaAtualId = 1;

        public PainelBarbeiroController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string secao = "inicio", string filtro = "hoje")
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "BARBEIRO")
                return RedirectToAction("Login", "Auth");

            var barbeiroId = HttpContext.Session.GetInt32("BarbeiroId");

            if (barbeiroId == null)
                return RedirectToAction("Login", "Auth");

            var barbeiro = _context.Barbeiros
                .FirstOrDefault(x =>
                    x.Id == barbeiroId.Value &&
                    x.BarbeariaId == BarbeariaAtualId);

            ViewBag.NomeBarbeiro = barbeiro?.Nome ?? "Barbeiro";
            ViewBag.ComissaoPercentual = barbeiro?.ComissaoPercentual ?? 0;
            ViewBag.FiltroAtual = filtro;
            ViewBag.SecaoAtual = secao;

            var hoje = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
            var amanha = hoje.AddDays(1);
            var depoisDeAmanha = hoje.AddDays(2);
            var fimSemana = hoje.AddDays(7);

            var primeiroDiaMes = DateTime.SpecifyKind(
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                DateTimeKind.Utc
            );

            var primeiroDiaProximoMes = primeiroDiaMes.AddMonths(1);

            var query = _context.Agendamentos
                .Include(x => x.Cliente)
                .Include(x => x.Barbeiro)
                .Where(x =>
                    x.BarbeariaId == BarbeariaAtualId &&
                    x.BarbeiroId == barbeiroId.Value);

            if (filtro == "hoje")
            {
                query = query.Where(x => x.DataHora >= hoje && x.DataHora < amanha);
            }
            else if (filtro == "amanha")
            {
                query = query.Where(x => x.DataHora >= amanha && x.DataHora < depoisDeAmanha);
            }
            else if (filtro == "semana")
            {
                query = query.Where(x => x.DataHora >= hoje && x.DataHora < fimSemana);
            }

            var agendamentos = query
                .OrderBy(x => x.DataHora)
                .ToList();

            var todosAgendamentos = _context.Agendamentos
                .Include(x => x.Cliente)
                .Include(x => x.Barbeiro)
                .Where(x =>
                    x.BarbeariaId == BarbeariaAtualId &&
                    x.BarbeiroId == barbeiroId.Value)
                .ToList();

            var agendamentosHoje = todosAgendamentos
                .Where(x => x.DataHora >= hoje && x.DataHora < amanha)
                .ToList();

            var finalizadosHoje = agendamentosHoje
                .Where(x => x.Status == "Finalizado")
                .ToList();

            var finalizadosMes = todosAgendamentos
                .Where(x =>
                    x.Status == "Finalizado" &&
                    x.DataHora >= primeiroDiaMes &&
                    x.DataHora < primeiroDiaProximoMes)
                .ToList();

            decimal percentual = barbeiro?.ComissaoPercentual ?? 0;

            ViewBag.TotalHoje = agendamentosHoje.Count;

            ViewBag.PendentesHoje = agendamentosHoje
                .Count(x =>
                    x.Status != "Finalizado" &&
                    x.Status != "Cancelado" &&
                    x.Status != "Faltou");

            ViewBag.FinalizadosHoje = finalizadosHoje.Count;

            ViewBag.FaturamentoHoje = finalizadosHoje
                .Sum(x => x.Valor)
                .ToString("C");

            ViewBag.ComissaoHoje = finalizadosHoje
                .Sum(x => x.Valor * percentual / 100)
                .ToString("C");

            ViewBag.ComissaoMes = finalizadosMes
                .Sum(x => x.Valor * percentual / 100)
                .ToString("C");

            ViewBag.Proximos = todosAgendamentos
                .Where(x =>
                    x.DataHora >= DateTime.UtcNow &&
                    x.Status != "Finalizado" &&
                    x.Status != "Cancelado" &&
                    x.Status != "Faltou")
                .OrderBy(x => x.DataHora)
                .Take(5)
                .ToList();

            return View(agendamentos);
        }

        [HttpPost]
        public IActionResult AlterarStatus(int id, string status, string filtroAtual = "hoje")
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "BARBEIRO")
                return RedirectToAction("Login", "Auth");

            var barbeiroId = HttpContext.Session.GetInt32("BarbeiroId");

            if (barbeiroId == null)
                return RedirectToAction("Login", "Auth");

            var agendamento = _context.Agendamentos
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.BarbeariaId == BarbeariaAtualId &&
                    x.BarbeiroId == barbeiroId.Value);

            if (agendamento == null)
            {
                TempData["Erro"] = "Agendamento não encontrado.";

                return RedirectToAction(
                    "Index",
                    new
                    {
                        secao = "agenda",
                        filtro = filtroAtual
                    });
            }

            agendamento.Status = status;

            _context.SaveChanges();

            TempData["Sucesso"] = "Status atualizado com sucesso.";

            return RedirectToAction(
                "Index",
                new
                {
                    secao = "agenda",
                    filtro = filtroAtual
                });
        }
    }
}