using Microsoft.EntityFrameworkCore;
using CentralApp.Data;
using CentralApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<SyncService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CentralDbContext>(options =>
	options.UseSqlServer(builder.Configuration
		.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
		policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}); 

var app = builder.Build();

app.UseCors(); 
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles(); 

app.Run();