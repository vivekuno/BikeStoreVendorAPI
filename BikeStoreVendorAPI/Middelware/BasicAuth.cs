using System.Text;

namespace BikeStoreVendor.API.Middelware
{
    public class BasicAuth
    {
        private readonly RequestDelegate _next;

        public BasicAuth(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authorization header is missing.");
                return;
            }

            var authHeaderValue = authHeader.ToString();
            if (!authHeaderValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authorization header is not Basic.");
                return;
            }

            var encodedUsernamePassword = authHeaderValue.Substring("Basic ".Length).Trim();
            var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
            var usernamePassword = decodedUsernamePassword.Split(':');

            if (usernamePassword.Length != 2)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Authorization header format.");
                return;
            }

            var username = usernamePassword[0];
            var password = usernamePassword[1];

            // Validate the username and password
            if (!IsAuthorized(username, password))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            await _next(context);
        }

        private bool IsAuthorized(string username, string password)
        {
            // Implement your user validation logic here
            return username == "vendor1" && password == "vendor@pass321";
        }
    }
}
