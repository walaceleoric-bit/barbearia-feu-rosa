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

            CarregarCombos(barbeariaId.Value);

            return View();
        }

        [HttpPost]
        public IActionResult Index(int barbeiroId, string servico, DateTime dataHora)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "CLIENTE")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var usuarioNome = HttpContext.Session.GetString("UsuarioNome") ?? "Cliente";

            var cliente = _context.Clientes
                .FirstOrDefault(c =>
                    c.Nome == usuarioNome &&
                    c.BarbeariaId == barbeariaId.Value);

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    BarbeariaId = barbeariaId.Value,
                    Nome = usuarioNome,
                    WhatsApp = ""
                };

                _context.Clientes.Add(cliente);
                _context.SaveChanges();
            }

            var barbeiroExiste = _context.Barbeiros
                .Any(b =>
                    b.Id == barbeiroId &&
                    b.BarbeariaId == barbeariaId.Value &&
                    b.Ativo);

            if (!barbeiroExiste)
            {
                TempData["Erro"] = "Barbeiro inválido.";
                CarregarCombos(barbeariaId.Value);
                return View();
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
                BarbeariaId = barbeariaId.Value,
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

        private void CarregarCombos(int barbeariaId)
        {
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
                new SelectListItem { Value = "Corte", Text = "Corte - R$ 30,00" },
                new SelectListItem { Value = "Barba", Text = "Barba - R$ 20,00" },
                new SelectListItem { Value = "Corte + Barba", Text = "Corte + Barba - R$ 50,00" },
                new SelectListItem { Value = "Sobrancelha", Text = "Sobrancelha - R$ 10,00" }
            };
        }
    }
}