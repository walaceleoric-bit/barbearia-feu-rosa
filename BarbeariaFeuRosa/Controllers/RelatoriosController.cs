using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarbeariaFeuRosa.Data;

namespace BarbeariaFeuRosa.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _context;

        public RelatoriosController(AppDbContext context)
        {
            _context = context;
        }

        private int? ObterBarbeariaId()
        {
            return HttpContext.Session.GetInt32("BarbeariaId");
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "ADM")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var agendamentos = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Where(a => a.BarbeariaId == barbeariaId.Value)
                .ToList();

            var avaliacoes = _context.AvaliacoesBarbeiros
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Where(a => a.BarbeariaId == barbeariaId.Value)
                .OrderByDescending(a => a.DataAvaliacao)
                .ToList();

            ViewBag.TotalAvaliacoes = avaliacoes.Count;

            ViewBag.MediaGeral = avaliacoes.Any()
                ? avaliacoes.Average(a => a.Nota).ToString("N1")
                : "0,0";

            ViewBag.Avaliacoes = avaliacoes;

            ViewBag.TopBarbeiros = avaliacoes
                .GroupBy(a => a.Barbeiro)
                .Select(g => new
                {
                    Nome = g.Key?.Nome ?? "Sem nome",
                    Media = g.Average(x => x.Nota),
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Media)
                .ThenByDescending(x => x.Total)
                .Take(3)
                .ToList();

            ViewBag.ClientesResumo = agendamentos
                .Where(a => a.Cliente != null)
                .GroupBy(a => a.Cliente)
                .Select(g => new
                {
                    Cliente = g.Key?.Nome ?? "Cliente",
                    Total = g.Count(),
                    Finalizados = g.Count(x => x.Status == "Finalizado"),
                    Cancelados = g.Count(x => x.Status == "Cancelado"),
                    Atrasados = g.Count(x => x.Status == "Atrasado"),
                    UltimaVisita = g
                        .Where(x => x.Status == "Finalizado")
                        .OrderByDescending(x => x.DataHora)
                        .Select(x => x.DataHora)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            ViewBag.ClientesSumidos = agendamentos
                .Where(a => a.Cliente != null && a.Status == "Finalizado")
                .GroupBy(a => a.Cliente)
                .Select(g => new
                {
                    Cliente = g.Key?.Nome ?? "Cliente",
                    UltimaVisita = g.Max(x => x.DataHora),
                    DiasSemVoltar = (DateTime.UtcNow.Date - g.Max(x => x.DataHora).Date).Days,
                    SugestaoRetorno = g.Max(x => x.DataHora).AddDays(30)
                })
                .Where(x => x.DiasSemVoltar >= 30)
                .OrderByDescending(x => x.DiasSemVoltar)
                .ToList();

            ViewBag.ServicosMaisFeitos = agendamentos
                .Where(a => a.Status == "Finalizado")
                .GroupBy(a => a.Servico)
                .Select(g => new
                {
                    Servico = g.Key,
                    Total = g.Count(),
                    Faturamento = g.Sum(x => x.Valor)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            return View();
        }
    }
}