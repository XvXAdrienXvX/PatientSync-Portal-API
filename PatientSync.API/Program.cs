using Appointments.Module;
using Authentication.Module;
using Doctor.Module;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Module registrations
builder.Services.AddAuthenticationModule();
builder.Services.AddDoctorModule();
builder.Services.AddAppointmentsModule();

// FastEndpoints registration (will discover endpoint classes)
builder.Services.AddFastEndpoints();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
    });
}

app.UseFastEndpoints();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
