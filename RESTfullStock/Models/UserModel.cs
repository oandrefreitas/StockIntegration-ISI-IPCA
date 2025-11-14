namespace RESTfullStock.Models
{

    /// <summary>
    /// Modelo para representar utilizadores.
    /// </summary>
    /// <param name="Id">Identificador único do utilizador.</param>
    /// <param name="Nome">Nome do utilizador.</param>
    /// <param name="Email">Email do utilizador.</param>
    /// <param name="Password">Senha do utilizador.</param>
    /// <param name="Roles">Lista de roles (perfis) do utilizador.</param>
    public record RestUser(
        int Id,
        string Nome,
        string Email,
        string Password,
        List<string> Roles);
}
