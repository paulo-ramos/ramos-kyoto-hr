using ramos_kyoto_hr.WebApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterAllServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();