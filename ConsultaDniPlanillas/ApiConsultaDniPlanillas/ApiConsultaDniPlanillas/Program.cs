using ApiConsultaDniPlanillas.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURAR CORS: Permitir que tu frontend consulte la API sin restricciones locales
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Registrar el servicio en el contenedor de DI
builder.Services.AddScoped<ConsultaDniService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. ACTIVAR CORS: ¡Es súper importante que vaya exactamente aquí!
// Debe estar después de builder.Build() y ANTES de UseAuthorization y MapControllers.
//app.UseHttpsRedirection();

app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();