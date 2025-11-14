namespace RESTfullStock.Models
{
    /// <summary>
    /// Modelo para receber as credenciais do utilizador no login.
    /// </summary>
    public record LoginRequest(string Email, string Password);
}
