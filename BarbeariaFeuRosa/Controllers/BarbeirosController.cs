using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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
                .Where(x => x.BarbeariaId == barbeariaId.Value)
                .OrderBy(x => x.Nome)
                .ToList();

            return View(barbeiros);
        }

        public IActionResult Novo()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Editar(
            Barbeiro barbeiro,
            IFormFile? foto,
            string usuarioLogin,
            string novaSenha,
            string statusAtivo)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");


            var barbeiroBanco = _context.Barbeiros
                .FirstOrDefault(x =>
                    x.Id == barbeiro.Id &&
                    x.BarbeariaId == barbeariaId.Value);

            if (barbeiroBanco == null)
            {
                TempData["Erro"] = "Barbeiro não encontrado.";
                return RedirectToAction("Index");
            }


            var usuario = _context.Usuarios
                .FirstOrDefault(x =>
                    x.BarbeiroId == barbeiroBanco.Id &&
                    x.BarbeariaId == barbeariaId.Value &&
                    x.Tipo == "BARBEIRO");


            ModelState.Clear();


            if (!string.IsNullOrWhiteSpace(novaSenha)
                && novaSenha.Length > 6)
            {
                TempData["Erro"] =
                    "A senha deve ter no máximo 6 caracteres.";

                ViewBag.UsuarioBarbeiro = usuario;

                return View(barbeiro);
            }


            var textoComissao =
                Request.Form["ComissaoPercentual"]
                .ToString();

            if (!decimal.TryParse(
                    textoComissao.Replace(",", "."),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var comissao))
            {
                TempData["Erro"] =
                    "Comissão inválida.";

                ViewBag.UsuarioBarbeiro = usuario;

                return View(barbeiro);
            }



            barbeiroBanco.Nome = barbeiro.Nome;
            barbeiroBanco.Telefone = barbeiro.Telefone;
            barbeiroBanco.Especialidade =
                barbeiro.Especialidade;

            barbeiroBanco.ComissaoPercentual =
                comissao;

            barbeiroBanco.Ativo =
                statusAtivo == "true";


            if (foto != null && foto.Length > 0)
            {
                barbeiroBanco.FotoUrl =
                    SalvarFoto(foto);
            }



            if (usuario != null)
            {
                usuario.Nome =
                    barbeiroBanco.Nome;


                if (!string.IsNullOrWhiteSpace(
                    usuarioLogin))
                {
                    usuario.UsuarioLogin =
                        usuarioLogin.Trim();
                }


                if (!string.IsNullOrWhiteSpace(
                    novaSenha))
                {
                    usuario.Senha =
                        novaSenha.Trim();
                }
            }



            _context.SaveChanges();

            TempData["Sucesso"] =
                "Barbeiro atualizado com sucesso!";

            return RedirectToAction("Index");
        }



        public IActionResult Editar(int id)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");


            var barbeiro = _context.Barbeiros
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.BarbeariaId == barbeariaId.Value);

            if (barbeiro == null)
                return RedirectToAction("Index");


            var usuario = _context.Usuarios
                .FirstOrDefault(x =>
                    x.BarbeiroId == barbeiro.Id &&
                    x.BarbeariaId == barbeariaId.Value &&
                    x.Tipo == "BARBEIRO");


            ViewBag.UsuarioBarbeiro = usuario;

            return View(barbeiro);
        }



        public IActionResult Excluir(int id)
        {
            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");


            var barbeiro = _context.Barbeiros
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.BarbeariaId == barbeariaId.Value);

            if (barbeiro != null)
            {
                var usuario = _context.Usuarios
                    .FirstOrDefault(x =>
                        x.BarbeiroId == barbeiro.Id);

                if (usuario != null)
                    _context.Usuarios.Remove(usuario);

                _context.Barbeiros.Remove(barbeiro);

                _context.SaveChanges();
            }


            return RedirectToAction("Index");
        }



        private string SalvarFoto(
            IFormFile arquivo)
        {
            var cloudName =
                _configuration["Cloudinary:CloudName"];

            var apiKey =
                _configuration["Cloudinary:ApiKey"];

            var apiSecret =
                _configuration["Cloudinary:ApiSecret"];


            var account =
                new Account(
                    cloudName,
                    apiKey,
                    apiSecret);

            var cloudinary =
                new Cloudinary(account);


            using var stream =
                arquivo.OpenReadStream();


            var uploadParams =
                new ImageUploadParams
                {
                    File = new FileDescription(
                        arquivo.FileName,
                        stream),

                    Folder =
                        "barbearia-feu-rosa/barbeiros"
                };


            var uploadResult =
                cloudinary.Upload(
                    uploadParams);


            return uploadResult
                .SecureUrl
                .ToString();
        }
    }
}