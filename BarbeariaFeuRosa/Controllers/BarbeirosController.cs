using Microsoft.AspNetCore.Mvc;
using BarbeariaFeuRosa.Models;
using BarbeariaFeuRosa.Data;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BarbeariaFeuRosa.Controllers
{
    public class BarbeirosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public BarbeirosController(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

            var barbeiros = _context.Barbeiros
                .Where(b => b.BarbeariaId == barbeariaId.Value)
                .OrderBy(b => b.Nome)
                .ToList();

            return View(barbeiros);
        }

        public IActionResult Novo()
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public IActionResult Novo(
            Barbeiro barbeiro,
            IFormFile? foto,
            string usuarioLogin,
            string senha)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            barbeiro.BarbeariaId = barbeariaId.Value;
            barbeiro.Ativo = true;

            ModelState.Remove("Barbearia");
            ModelState.Remove("BarbeariaId");

            if (string.IsNullOrWhiteSpace(usuarioLogin))
                ModelState.AddModelError("", "Informe o usuário de login do barbeiro.");

            if (string.IsNullOrWhiteSpace(senha))
                ModelState.AddModelError("", "Informe a senha do barbeiro.");

            if (!string.IsNullOrWhiteSpace(senha) && senha.Length > 6)
                ModelState.AddModelError("", "A senha deve ter no máximo 6 caracteres.");

            bool usuarioJaExiste = _context.Usuarios
                .Any(u =>
                    u.UsuarioLogin == usuarioLogin &&
                    u.BarbeariaId == barbeariaId.Value);

            if (usuarioJaExiste)
                ModelState.AddModelError("", "Este usuário de login já existe nesta barbearia.");

            if (!ModelState.IsValid)
                return View(barbeiro);

            if (foto != null && foto.Length > 0)
                barbeiro.FotoUrl = SalvarFoto(foto);

            _context.Barbeiros.Add(barbeiro);
            _context.SaveChanges();

            var usuario = new Usuario
            {
                Nome = barbeiro.Nome,
                UsuarioLogin = usuarioLogin,
                Senha = senha,
                Tipo = "BARBEIRO",
                BarbeariaId = barbeariaId.Value,
                BarbeiroId = barbeiro.Id
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            TempData["Sucesso"] = "Barbeiro e login criados com sucesso!";

            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var barbeiro = _context.Barbeiros
                .FirstOrDefault(b =>
                    b.Id == id &&
                    b.BarbeariaId == barbeariaId.Value);

            if (barbeiro == null)
            {
                TempData["Erro"] = "Barbeiro não encontrado.";
                return RedirectToAction("Index");
            }

            return View(barbeiro);
        }

        [HttpPost]
        public IActionResult Editar(
            Barbeiro barbeiro,
            IFormFile? foto)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            ModelState.Remove("Barbearia");
            ModelState.Remove("BarbeariaId");

            if (!ModelState.IsValid)
                return View(barbeiro);

            var barbeiroBanco = _context.Barbeiros
                .FirstOrDefault(b =>
                    b.Id == barbeiro.Id &&
                    b.BarbeariaId == barbeariaId.Value);

            if (barbeiroBanco == null)
            {
                TempData["Erro"] = "Barbeiro não encontrado.";
                return RedirectToAction("Index");
            }

            barbeiroBanco.Nome = barbeiro.Nome;
            barbeiroBanco.Telefone = barbeiro.Telefone;
            barbeiroBanco.Especialidade = barbeiro.Especialidade;
            barbeiroBanco.ComissaoPercentual = barbeiro.ComissaoPercentual;
            barbeiroBanco.Ativo = barbeiro.Ativo;

            if (foto != null && foto.Length > 0)
                barbeiroBanco.FotoUrl = SalvarFoto(foto);

            var usuario = _context.Usuarios
                .FirstOrDefault(u =>
                    u.BarbeiroId == barbeiroBanco.Id &&
                    u.BarbeariaId == barbeariaId.Value &&
                    u.Tipo == "BARBEIRO");

            if (usuario != null)
                usuario.Nome = barbeiroBanco.Nome;

            _context.SaveChanges();

            TempData["Sucesso"] = "Barbeiro atualizado com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Excluir(int id)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var barbeiro = _context.Barbeiros
                .FirstOrDefault(b =>
                    b.Id == id &&
                    b.BarbeariaId == barbeariaId.Value);

            if (barbeiro != null)
            {
                var usuario = _context.Usuarios
                    .FirstOrDefault(u =>
                        u.BarbeiroId == barbeiro.Id &&
                        u.BarbeariaId == barbeariaId.Value);

                if (usuario != null)
                    _context.Usuarios.Remove(usuario);

                _context.Barbeiros.Remove(barbeiro);
                _context.SaveChanges();

                TempData["Sucesso"] = "Barbeiro excluído com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Barbeiro não encontrado.";
            }

            return RedirectToAction("Index");
        }

        private string SalvarFoto(IFormFile arquivo)
        {
            var cloudName = _configuration["Cloudinary:CloudName"];
            var apiKey = _configuration["Cloudinary:ApiKey"];
            var apiSecret = _configuration["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            var cloudinary = new Cloudinary(account);

            using var stream = arquivo.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(arquivo.FileName, stream),
                Folder = "barbearia-feu-rosa/barbeiros"
            };

            var uploadResult = cloudinary.Upload(uploadParams);

            return uploadResult.SecureUrl.ToString();
        }
    }
}