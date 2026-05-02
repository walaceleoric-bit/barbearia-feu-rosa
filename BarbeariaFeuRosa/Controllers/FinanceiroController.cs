using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarbeariaFeuRosa.Data;

namespace BarbeariaFeuRosa.Controllers
{
    public class FinanceiroController : Controller
    {
        private readonly AppDbContext _context;

        public FinanceiroController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var hoje = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
            var amanha = hoje.AddDays(1);

            var primeiroDiaMes = DateTime.SpecifyKind(
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                DateTimeKind.Utc
            );

            var primeiroDiaProximoMes = primeiroDiaMes.AddMonths(1);

            var agendamentos = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .OrderByDescending(a => a.DataHora)
                .ToList();

            var finalizados = agendamentos
                .Where(a => a.Status == "Finalizado")
                .ToList();

            ViewBag.FaturamentoHoje = finalizados
                .Where(a => a.DataHora >= hoje && a.DataHora < amanha)
                .Sum(a => a.Valor)
                .ToString("C");

            ViewBag.FaturamentoMes = finalizados
                .Where(a => a.DataHora >= primeiroDiaMes && a.DataHora < primeiroDiaProximoMes)
                .Sum(a => a.Valor)
                .ToString("C");

            ViewBag.Comissoes = finalizados
                .Sum(a => a.Valor * (a.Barbeiro != null ? a.Barbeiro.ComissaoPercentual : 0) / 100)
                .ToString("C");

            ViewBag.TotalAgendamentos = agendamentos.Count;
            ViewBag.Movimentacoes = agendamentos;

            return View();
        }
    }
}