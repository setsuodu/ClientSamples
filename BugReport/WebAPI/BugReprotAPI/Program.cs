var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(x => x.AddPolicy("AllowCors", d => d.AllowAnyOrigin().AllowAnyMethod())); //跨域


var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins"); //全局允许跨域，内网localhost调试用

app.UseAuthorization();

app.MapControllers();

app.Run();
