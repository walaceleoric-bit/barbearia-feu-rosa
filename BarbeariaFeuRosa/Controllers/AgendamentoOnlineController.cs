using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BarbeariaFeuRosa.Data;
using BarbeariaFeuRosa.Models;

namespace BarbeariaFeuRosa.Controllers
{
    public class AgendamentoOnlineController : Controller
    {
        private readonly AppDbContext _context;

        public AgendamentoOnlineController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            ViewBag.Barbeiros = new SelectList(
                _context.Barbeiros.Where(b => b.Ativo).OrderBy(b => b.Nome),
                "Id",
                "Nome"
            );

            ViewBag.Servicos = new List<SelectListItem>
            {
                new SelectListItem { Value = "Corte", Text = "Corte - R$ 30,00" },
                new SelectListItem { Value = "Barba", Text = "Barba - R$ 20,00" },
                new SelectListItem { Value = "Corte + Barba", Text = "Corte + Barba - R$ 50,00" },
                new SelectListItem { Value = "Sobrancelha", Text = "Sobrancelha - R$ 10,00" }
            };

            return View();
        }

        [HttpPost]
        public IActionResult Index(int barbeiroId, string servico, DateTime dataHora)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome") ?? "Cliente";

            var cliente = _context.Clientes.FirstOrDefault(c => c.Nome == usuarioNome);

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Nome = usuarioNome,
                    WhatsApp = ""
                };

                _context.Clientes.Add(cliente);
                _context.SaveChanges();
            }

            decimal valor = servico switch
            {
                "Corte" => 30,
                "Barba" => 20,
                "Corte + Barba" => 50,
                "Sobrancelha" => 10,
                _ => 0
            };

            var agendamento = new Agendamento
            {
                ClienteId = cliente.Id,
                BarbeiroId = barbeiroId,
                Servico = servico,
                DataHora = DateTime.SpecifyKind(dataHora, DateTimeKind.Utc),
                Valor = valor,
                Status = "Agendado"
            };

            _context.Agendamentos.Add(agendamento);
            _context.SaveChanges();

            TempData["Sucesso"] = "Agendamento realizado com sucesso!";

            return RedirectToAction("Index", "HistoricoCliente");
        }
    }
}