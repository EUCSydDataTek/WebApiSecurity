# API Key Security!
Til mindre kr�vende beskyttelse af et WebApi kan man benytte API Key. Eller hvis det ene WebApi skal have adgang til et andet WebApi.

Opret et ASP.NET WebAPI uden sikkerhed.

Opret en folder kaldet **Authentication** med f�lgende 3 klasser:

**ApiKeyAuthOptions**

```csharp
public class ApiKeyAuthOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public string Scheme => DefaultScheme;
    public string ApiKey { get; set; }
}
```
&nbsp;

**ApiKeyAuthHandler**
```csharp
public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthOptions>
{
    const string ApiKeyIdentifier = "apikey";

    public ApiKeyAuthHandler(
        IOptionsMonitor<ApiKeyAuthOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string key = string.Empty;

        if (Request.Headers[ApiKeyIdentifier].Any())
        {
            key = Request.Headers[ApiKeyIdentifier].FirstOrDefault();
        }
        else if (Request.Query.ContainsKey(ApiKeyIdentifier))
        {
            if (Request.Query.TryGetValue(ApiKeyIdentifier, out var queryKey))
                key = queryKey;
        }

        if (string.IsNullOrWhiteSpace(key))
            return Task.FromResult(AuthenticateResult.Fail("No api key provided"));

        if (!string.Equals(key, Options.ApiKey, StringComparison.Ordinal))
            return Task.FromResult(AuthenticateResult.Fail("Invalid api key."));

        var identities = new List<ClaimsIdentity> {new ClaimsIdentity("ApiKeyIdentity")};

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```
&nbsp;

**AuthenticationBuilderExtensions**
```csharp
public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddApiKeyAuth(
        this AuthenticationBuilder builder,
        Action<ApiKeyAuthOptions> configureOptions)
    {
        return builder
            .AddScheme<ApiKeyAuthOptions, ApiKeyAuthHandler>(
                ApiKeyAuthOptions.DefaultScheme,
                configureOptions);
    }
}
```
&nbsp;

I Startup.cs tilf�jes f�lgende *ConfigureServices*:
```csharp
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = ApiKeyAuthOptions.DefaultScheme;
    options.DefaultChallengeScheme = ApiKeyAuthOptions.DefaultScheme;
}).AddApiKeyAuth(Configuration.GetSection("Authentication").Bind);
```
Og f�lgende tilf�jes lige f�r `app.Authorization()` i `Configure()`:
```csharp
app.UseAuthentication();
```
&nbsp;
#### API Key
I dette simple eksempel er API Key hardcoded og placeret i maskinens **User Secret**. L�s evt. mere her: [Safe storage of app secrets in development in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows)

H�jre klik p� projektets navn og v�lg *User Secret*. Ret `secrets.json` til:
```json
{
  "Authentication": 
  {
    "ApiKey": "12345"
  }
}
```
&nbsp;
#### Beskyt ressourcen
I demo-projektet beskyttes GET-metoden WeatherForecastController:
```csharp
 [Authorize]
```

&nbsp;

## Test i browser

Skriv: https://localhost:44383/weatherforecast?apikey=12345

&nbsp;
## Test i Postman
Ops�t en GET request: https://localhost:44383/weatherforecast

V�lg Authorization og Type = API Key.

S�t Key = apikey og Value = 12345.

Start WebApi projektet i debug-mode og lav en request. Pr�v ogs� at �ndre koden til noget forkert og stud�r loggen i Output vinduet. Pr�v ogs� at fjerne API Key fuldst�ndigt og test.
