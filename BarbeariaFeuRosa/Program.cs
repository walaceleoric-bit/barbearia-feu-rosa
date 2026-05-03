using BarbeariaFeuRosa.Data;
using BarbeariaFeuRosa.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var barbearia = context.Barbearias
        .FirstOrDefault(b => b.Id == 1);

    if (barbearia == null)
    {
        context.Barbearias.Add(new Barbearia
        {
            Id = 1,
            Nome = "Barbearia Feu Rosa",
            Slug = "feu-rosa",
            LogoUrl = "",
            Ativa = true
        });

        context.SaveChanges();
    }

    bool existeAdm = context.Usuarios
        .Any(x =>
            x.UsuarioLogin == "admin" &&
            x.BarbeariaId == 1);

    if (!existeAdm)
    {
        context.Usuarios.Add(new Usuario
        {
            Nome = "Administrador",
            UsuarioLogin = "admin",
            Senha = "123456",
            Tipo = "ADM",
            BarbeariaId = 1
        });

        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

/*
    ROTA FUTURA DO SAAS:
    Exemplo:
    /feu-rosa/Auth/Login
    /feu-rosa/Dashboard
    /feu-rosa/Clientes

    Por enquanto ela já existe, mas os controllers ainda estão usando BarbeariaId = 1.
*/
app.MapControllerRoute(
    name: "tenant",
    pattern: "{barbeariaSlug}/{controller=Auth}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "agenda",
    pattern: "Agenda/{action=Index}/{id?}",
    defaults: new { controller = "Agenda" });

app.MapControllerRoute(
    name: "agendamentoOnline",
    pattern: "AgendamentoOnline/{action=Index}/{id?}",
    defaults: new { controller = "AgendamentoOnline" });

app.MapControllerRoute(
    name: "auth",
    pattern: "Auth/{action=Login}/{id?}",
    defaults: new { controller = "Auth" });

app.MapControllerRoute(
    name: "barbeiros",
    pattern: "Barbeiros/{action=Index}/{id?}",
    defaults: new { controller = "Barbeiros" });

app.MapControllerRoute(
    name: "clientes",
    pattern: "Clientes/{action=Index}/{id?}",
    defaults: new { controller = "Clientes" });

app.MapControllerRoute(
    name: "clienteHome",
    pattern: "ClienteHome/{action=Index}/{id?}",
    defaults: new { controller = "ClienteHome" });

app.MapControllerRoute(
    name: "configuracoes",
    pattern: "Configuracoes/{action=Index}/{id?}",
    defaults: new { controller = "Configuracoes" });

app.MapControllerRoute(
    name: "dashboard",
    pattern: "Dashboard/{action=Index}/{id?}",
    defaults: new { controller = "Dashboard" });

app.MapControllerRoute(
    name: "financeiro",
    pattern: "Financeiro/{action=Index}/{id?}",
    defaults: new { controller = "Financeiro" });

app.MapControllerRoute(
    name: "historicoCliente",
    pattern: "HistoricoCliente/{action=Index}/{id?}",
    defaults: new { controller = "HistoricoCliente" });

app.MapControllerRoute(
    name: "painelBarbeiro",
    pattern: "PainelBarbeiro/{action=Index}/{id?}",
    defaults: new { controller = "PainelBarbeiro" });

app.MapControllerRoute(
    name: "home",
    pattern: "Home/{action=Index}/{id?}",
    defaults: new { controller = "Home" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();