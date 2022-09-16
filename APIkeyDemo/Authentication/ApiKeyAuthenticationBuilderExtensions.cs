using Microsoft.AspNetCore.Authentication;

namespace APIkeyDemo.Authentication;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddApiKeyAuth(this AuthenticationBuilder builder, Action<ApiKeyAuthOptions> configureOptions)
    {
        return builder.AddScheme<ApiKeyAuthOptions, ApiKeyAuthHandler>(ApiKeyAuthOptions.DefaultScheme, configureOptions);
    }
}
