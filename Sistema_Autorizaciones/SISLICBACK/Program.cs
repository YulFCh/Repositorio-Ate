using QuestPDF.Infrastructure;
using SISLICBACK.Services;
using System.ComponentModel;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// --- PASO 1: Definir una política de CORS ---
var misOrigenesPermitidos = "_misOrigenesPermitidos";

builder.Services.AddCors(options => {
    options.AddPolicy(name: misOrigenesPermitidos,
                      policy => {
                          // Reemplaza con la URL de tu Angular (ej. http://localhost:4200)
                          policy.AllowAnyOrigin()
                          //.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
// -------------------------------------------

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
ServiceMain.RegisterServices(builder.Services);
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    ForwardLimit = 1
});

if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- PASO 2: Activar CORS ---
// IMPORTANTE: Debe ir ANTES de UseAuthorization y DESPUÉS de HttpsRedirection
app.UseCors(misOrigenesPermitidos);
// ----------------------------

app.UseAuthorization();

app.MapControllers();

app.Run();