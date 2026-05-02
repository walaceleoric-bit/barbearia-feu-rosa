using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarbeariaFeuRosa.Data;

namespace BarbeariaFeuRosa.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tipo = HttpContext.Session.GetString("UsuarioTipo");

            if (tipo != "ADM")
            {
                return RedirectToAction("Login", "Auth");
            }

            var hoje = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
            var amanha = hoje.AddDays(1);

            var primeiroDiaMes = DateTime.SpecifyKind(
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                DateTimeKind.Utc
            );

            var primeiroDiaProximoMes = primeiroDiaMes.AddMonths(1);

            var config = _context.Configuracoes.FirstOrDefault();

            ViewBag.NomeBarbearia = config?.NomeBarbearia ?? "Barbearia Feu Rosa";
            ViewBag.PromocaoDestaque = config?.PromocaoDestaque ?? "Corte + Barba com preço especial.";
            ViewBag.LogoUrl = config?.LogoUrl;

            var agendamentosHoje = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Where(a => a.DataHora >= hoje && a.DataHora < amanha)
                .OrderBy(a => a.DataHora)
                .ToList();

            ViewBag.TotalClientes = _context.Clientes.Count();
            ViewBag.AgendamentosHoje = agendamentosHoje.Count;

            ViewBag.FaturamentoHoje = _context.Agendamentos
                .Where(a =>
                    a.Status == "Finalizado" &&
                    a.DataHora >= hoje &&
                    a.DataHora < amanha)
                .Sum(a => a.Valor)
                .ToString("C");

            ViewBag.FaturamentoMes = _context.Agendamentos
                .Where(a =>
                    a.Status == "Finalizado" &&
                    a.DataHora >= primeiroDiaMes &&
                    a.DataHora < primeiroDiaProximoMes)
                .Sum(a => a.Valor)
                .ToString("C");

            ViewBag.AgendaHoje = agendamentosHoje;

            return View();
        }
    }
}