using APIkeyDemo.Authentication;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region API key
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = ApiKeyAuthOptions.DefaultScheme;
        options.DefaultChallengeScheme = ApiKeyAuthOptions.DefaultScheme;
    }).AddApiKeyAuth(builder.Configuration.GetSection("Authentication").Bind);
#endregion

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();                // Added
app.UseAuthorization();

app.MapControllers();

app.Run();

