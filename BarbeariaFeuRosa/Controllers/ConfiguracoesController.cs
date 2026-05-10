using Microsoft.AspNetCore.Mvc;
using BarbeariaFeuRosa.Data;
using BarbeariaFeuRosa.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BarbeariaFeuRosa.Controllers
{
    public class ConfiguracoesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ConfiguracoesController(
            AppDbContext context,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
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

            var configuracao = _context.Configuracoes
                .FirstOrDefault(c => c.BarbeariaId == barbeariaId.Value);

            if (configuracao == null)
            {
                configuracao = new Configuracao
                {
                    BarbeariaId = barbeariaId.Value,
                    NomeBarbearia = "Minha Barbearia",
                    PromocaoDestaque = "Corte + Barba com preço especial."
                };

                _context.Configuracoes.Add(configuracao);
                _context.SaveChanges();
            }

            ViewBag.Servicos = _context.Servicos
                .Where(s => s.BarbeariaId == barbeariaId.Value && s.Ativo)
                .OrderBy(s => s.Nome)
                .ToList();

            return View(configuracao);
        }

        [HttpPost]
        public IActionResult Index(
            Configuracao configuracao,
            IFormFile? logo,
            IFormFile? imagem1,
            IFormFile? imagem2,
            IFormFile? imagem3,
            IFormFile? imagem4,
            IFormFile? imagem5,
            string? senhaAdmAtual,
            string? novaSenhaAdm)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "ADM")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var configAtual = _context.Configuracoes
                .FirstOrDefault(c => c.BarbeariaId == barbeariaId.Value);

            if (configAtual == null)
            {
                configAtual = new Configuracao
                {
                    BarbeariaId = barbeariaId.Value
                };

                _context.Configuracoes.Add(configAtual);
            }

            configAtual.NomeBarbearia = configuracao.NomeBarbearia;
            configAtual.PromocaoDestaque = configuracao.PromocaoDestaque;

            if (logo != null && logo.Length > 0)
                configAtual.LogoUrl = SalvarArquivo(logo);

            if (imagem1 != null && imagem1.Length > 0)
                configAtual.CarrosselImagem1 = SalvarArquivo(imagem1);

            if (imagem2 != null && imagem2.Length > 0)
                configAtual.CarrosselImagem2 = SalvarArquivo(imagem2);

            if (imagem3 != null && imagem3.Length > 0)
                configAtual.CarrosselImagem3 = SalvarArquivo(imagem3);

            if (imagem4 != null && imagem4.Length > 0)
                configAtual.CarrosselImagem4 = SalvarArquivo(imagem4);

            if (imagem5 != null && imagem5.Length > 0)
                configAtual.CarrosselImagem5 = SalvarArquivo(imagem5);

            if (!string.IsNullOrWhiteSpace(novaSenhaAdm))
            {
                if (novaSenhaAdm.Length > 6)
                {
                    TempData["Erro"] = "A nova senha do ADM deve ter no máximo 6 caracteres.";
                    return RedirectToAction("Index");
                }

                var adm = _context.Usuarios
                    .FirstOrDefault(u =>
                        u.Tipo == "ADM" &&
                        u.BarbeariaId == barbeariaId.Value);

                if (adm == null)
                {
                    TempData["Erro"] = "Usuário ADM não encontrado.";
                    return RedirectToAction("Index");
                }

                if (adm.Senha != senhaAdmAtual)
                {
                    TempData["Erro"] = "Senha atual do ADM incorreta.";
                    return RedirectToAction("Index");
                }

                adm.Senha = novaSenhaAdm;
            }

            _context.SaveChanges();

            TempData["Sucesso"] = "Configurações salvas com sucesso!";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AdicionarServico(string nome, string valor)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "ADM")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            if (string.IsNullOrWhiteSpace(nome))
            {
                TempData["Erro"] = "Informe o nome do serviço.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(valor))
            {
                TempData["Erro"] = "Informe o valor do serviço.";
                return RedirectToAction("Index");
            }

            valor = valor.Replace(",", ".");

            if (!decimal.TryParse(
                valor,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal valorConvertido))
            {
                TempData["Erro"] = "Valor inválido.";
                return RedirectToAction("Index");
            }

            if (valorConvertido <= 0)
            {
                TempData["Erro"] = "Informe um valor maior que zero.";
                return RedirectToAction("Index");
            }

            bool existe = _context.Servicos.Any(s =>
                s.BarbeariaId == barbeariaId.Value &&
                s.Ativo &&
                s.Nome.ToLower() == nome.Trim().ToLower());

            if (existe)
            {
                TempData["Erro"] = "Este serviço já existe nesta barbearia.";
                return RedirectToAction("Index");
            }

            var servico = new Servico
            {
                Nome = nome.Trim(),
                Valor = valorConvertido,
                Ativo = true,
                BarbeariaId = barbeariaId.Value
            };

            _context.Servicos.Add(servico);
            _context.SaveChanges();

            TempData["Sucesso"] = "Serviço adicionado com sucesso!";

            return RedirectToAction("Index");
        }

        public IActionResult ExcluirServico(int id)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "ADM")
                return RedirectToAction("Login", "Auth");

            var barbeariaId = ObterBarbeariaId();

            if (barbeariaId == null)
                return RedirectToAction("Login", "Auth");

            var servico = _context.Servicos
                .FirstOrDefault(s =>
                    s.Id == id &&
                    s.BarbeariaId == barbeariaId.Value);

            if (servico == null)
            {
                TempData["Erro"] = "Serviço não encontrado.";
                return RedirectToAction("Index");
            }

            servico.Ativo = false;
            _context.SaveChanges();

            TempData["Sucesso"] = "Serviço excluído com sucesso!";

            return RedirectToAction("Index");
        }

        private string SalvarArquivo(IFormFile arquivo)
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
                Folder = "barbearia-feu-rosa"
            };

            var uploadResult = cloudinary.Upload(uploadParams);

            return uploadResult.SecureUrl.ToString();
        }
    }
}