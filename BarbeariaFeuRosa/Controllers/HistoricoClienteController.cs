using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarbeariaFeuRosa.Data;

namespace BarbeariaFeuRosa.Controllers
{
    public class HistoricoClienteController : Controller
    {
        private readonly AppDbContext _context;

        private const int BarbeariaAtualId = 1;

        public HistoricoClienteController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome");

            var historico = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Where(a =>
                    a.BarbeariaId == BarbeariaAtualId &&
                    a.Cliente != null &&
                    a.Cliente.Nome == usuarioNome)
                .OrderByDescending(a => a.DataHora)
                .ToList();

            return View(historico);
        }

        public IActionResult Excluir(int id)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome");

            var agendamento = _context.Agendamentos
                .Include(a => a.Cliente)
                .FirstOrDefault(a =>
                    a.Id == id &&
                    a.BarbeariaId == BarbeariaAtualId &&
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

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome");

            var historico = _context.Agendamentos
                .Include(a => a.Cliente)
                .Where(a =>
                    a.BarbeariaId == BarbeariaAtualId &&
                    a.Cliente != null &&
                    a.Cliente.Nome == usuarioNome)
                .ToList();

            _context.Agendamentos.RemoveRange(historico);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}