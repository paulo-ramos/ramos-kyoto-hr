using ramos_kyoto_hr.WebApi.Configuration;
using ramos_kyoto_hr.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterAllServices();

var app = builder.Build();

// Middleware de tratamento de exceções (deve vir primeiro)
app.UseExceptionHandlingMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();