using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarbeariaFeuRosa.Data;
using BarbeariaFeuRosa.Models;

namespace BarbeariaFeuRosa.Controllers
{
    public class HistoricoClienteController : Controller
    {
        private readonly AppDbContext _context;

        public HistoricoClienteController(AppDbContext context)
        {
            _context = context;
        }

        private int? ObterBarbeariaId()
        {
            return HttpContext.Session.GetInt32("BarbeariaId");
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome");

            var historico = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Where(a =>
                    a.BarbeariaId == barbeariaId.Value &&
                    a.Cliente != null &&
                    a.Cliente.Nome == usuarioNome)
                .OrderByDescending(a => a.DataHora)
                .ToList();

            var idsAvaliados = _context.AvaliacoesBarbeiros
                .Where(a => a.BarbeariaId == barbeariaId.Value)
                .Select(a => a.BarbeiroId + "-" + a.ClienteId)
                .ToList();

            ViewBag.AvaliacoesFeitas = idsAvaliados;

            return View(historico);
        }

        public IActionResult Avaliar(int id)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome");

            var agendamento = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .FirstOrDefault(a =>
                    a.Id == id &&
                    a.BarbeariaId == barbeariaId.Value &&
                    a.Status == "Finalizado" &&
                    a.Cliente != null &&
                    a.Cliente.Nome == usuarioNome);

            if (agendamento == null)
            {
                TempData["Erro"] = "Este atendimento não pode ser avaliado.";
                return RedirectToAction("Index");
            }

            bool jaAvaliou = _context.AvaliacoesBarbeiros.Any(a =>
                a.BarbeariaId == barbeariaId.Value &&
                a.BarbeiroId == agendamento.BarbeiroId &&
                a.ClienteId == agendamento.ClienteId);

            if (jaAvaliou)
            {
                TempData["Erro"] = "Você já avaliou este barbeiro.";
                return RedirectToAction("Index");
            }

            return View(agendamento);
        }

        [HttpPost]
        public IActionResult Avaliar(int agendamentoId, int nota, string comentario)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome");

            var agendamento = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .FirstOrDefault(a =>
                    a.Id == agendamentoId &&
                    a.BarbeariaId == barbeariaId.Value &&
                    a.Status == "Finalizado" &&
                    a.Cliente != null &&
                    a.Cliente.Nome == usuarioNome);

            if (agendamento == null)
            {
                TempData["Erro"] = "Atendimento inválido.";
                return RedirectToAction("Index");
            }

            if (nota < 1 || nota > 5)
            {
                TempData["Erro"] = "Escolha uma nota de 1 a 5.";
                return RedirectToAction("Avaliar", new { id = agendamentoId });
            }

            bool jaAvaliou = _context.AvaliacoesBarbeiros.Any(a =>
                a.BarbeariaId == barbeariaId.Value &&
                a.BarbeiroId == agendamento.BarbeiroId &&
                a.ClienteId == agendamento.ClienteId);

            if (jaAvaliou)
            {
                TempData["Erro"] = "Você já avaliou este barbeiro.";
                return RedirectToAction("Index");
            }

            var avaliacao = new AvaliacaoBarbeiro
            {
                BarbeariaId = barbeariaId.Value,
                BarbeiroId = agendamento.BarbeiroId,
                ClienteId = agendamento.ClienteId,
                Nota = nota,
                Comentario = comentario ?? "",
                DataAvaliacao = DateTime.UtcNow
            };

            _context.AvaliacoesBarbeiros.Add(avaliacao);
            _context.SaveChanges();

            TempData["Sucesso"] = "Avaliação enviada com sucesso!";

            return RedirectToAction("Index");
        }

        public IActionResult Excluir(int id)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome");

            var agendamento = _context.Agendamentos
                .Include(a => a.Cliente)
                .FirstOrDefault(a =>
                    a.Id == id &&
                    a.BarbeariaId == barbeariaId.Value &&
                    a.Cliente != null &&
                    a.Cliente.Nome == usuarioNome);

            if (agendamento != null)
            {
                _context.Agendamentos.Remove(agendamento);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Limpar()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome");

            var historico = _context.Agendamentos
                .Include(a => a.Cliente)
                .Where(a =>
                    a.BarbeariaId == barbeariaId.Value &&
                    a.Cliente != null &&
                    a.Cliente.Nome == usuarioNome)
                .ToList();

            _context.Agendamentos.RemoveRange(historico);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}