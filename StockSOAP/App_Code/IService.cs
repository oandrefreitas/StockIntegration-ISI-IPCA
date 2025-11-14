using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace WCFSTOCK
{
    /// <summary>
    /// Interface que define os contratos SOAP para operações relacionadas com Fornecedores.
    /// </summary>
    [ServiceContract]
    public interface IService
    {
        #region testar_conexao_contract
        [OperationContract]
        string TestarConexao();
        #endregion testar_conexao

        #region user_contract
        /// <summary>
        /// Valida as credenciais de um utilizador.
        /// </summary>
        /// <param name="email">Email do utilizador.</param>
        /// <param name="password">Senha do utilizador.</param>
        /// <returns>True se as credenciais forem válidas; False caso contrário.</returns>
        [OperationContract]
        bool ValidateUser(string email, string password);

        /// <summary>
        /// Obtém os detalhes de um utilizador com base no email.
        /// </summary>
        /// <param name="email">Email do utilizador.</param>
        /// <returns>Um objeto `User` correspondente ao email fornecido.</returns>
        [OperationContract]
        User GetUserByEmail(string email);

        [OperationContract]
        bool AddUser(string nome, string email, string password, string role);

        #endregion user_contract

        #region Fornecedor_contract

        /// <summary>
        /// Obtém a lista de todos os fornecedores na base de dados.
        /// </summary>
        /// <returns>Uma lista de objetos do tipo Fornecedor.</returns>
        [OperationContract]
        List<Fornecedor> GetAllFornecedores();

        /// <summary>
        /// Obtém os detalhes de um fornecedor específico, com base no seu ID.
        /// </summary>
        /// <param name="id">ID do fornecedor a ser pesquisado.</param>
        /// <returns>Um objeto do tipo Fornecedor correspondente ao ID fornecido, ou null se não encontrado.</returns>
        [OperationContract]
        Fornecedor GetFornecedorById(int id);

        /// <summary>
        /// Adiciona um novo fornecedor à base de dados.
        /// </summary>
        /// <param name="fornecedor">Objeto Fornecedor contendo os dados do novo fornecedor.</param>
        /// <returns>True se o fornecedor for adicionado com sucesso; False caso contrário.</returns>
        [OperationContract]
        bool AddFornecedor(Fornecedor fornecedor);

        /// <summary>
        /// Atualiza os dados de um fornecedor existente na base de dados.
        /// </summary>
        /// <param name="fornecedor">Objeto Fornecedor com os dados atualizados.</param>
        /// <returns>True se a atualização for bem-sucedida; False caso contrário.</returns>
        [OperationContract]
        bool UpdateFornecedor(Fornecedor fornecedor);

        /// <summary>
        /// Remove um fornecedor da base de dados, com base no seu ID.
        /// </summary>
        /// <param name="id">ID do fornecedor a ser removido.</param>
        /// <returns>True se o fornecedor for removido com sucesso; False caso contrário.</returns>
        [OperationContract]
        bool DeleteFornecedor(int id);
        #endregion

        #region Produto_contract

        /// <summary>
        /// Obtém a lista de todos os produtos na base de dados.
        /// </summary>
        /// <returns>Uma lista de objetos do tipo Produto.</returns>
        [OperationContract]
        List<Produto> GetAllProdutos();

        /// <summary>
        /// Obtém os detalhes de um produto específico, com base no seu ID.
        /// </summary>
        /// <param name="id">ID do produto a ser pesquisado.</param>
        /// <returns>Um objeto do tipo Produto correspondente ao ID fornecido, ou null se não encontrado.</returns>
        [OperationContract]
        Produto GetProdutoById(int id);

        /// <summary>
        /// Adiciona um novo produto à base de dados.
        /// </summary>
        /// <param name="produto">Objeto Produto contendo os dados do novo produto.</param>
        /// <returns>True se o produto for adicionado com sucesso; False caso contrário.</returns>
        [OperationContract]
        bool AddProduto(Produto produto);

        /// <summary>
        /// Atualiza os dados de um produto existente na base de dados.
        /// </summary>
        /// <param name="produto">Objeto Produto com os dados atualizados.</param>
        /// <returns>True se a atualização for bem-sucedida; False caso contrário.</returns>
        [OperationContract]
        bool UpdateProduto(Produto produto);

        /// <summary>
        /// Remove um produto da base de dados, com base no seu ID.
        /// </summary>
        /// <param name="id">ID do produto a ser removido.</param>
        /// <returns>True se o produto for removido com sucesso; False caso contrário.</returns>
        [OperationContract]
        bool DeleteProduto(int id);

        #endregion Produto_contract

        #region Encomenda_contract

        /// <summary>
        /// Cria uma nova encomenda com itens relacionados.
        /// </summary>
        /// <param name="encomenda">Dados da encomenda, incluindo os itens.</param>
        /// <returns>True se a encomenda for criada com sucesso; False caso contrário.</returns>
        [OperationContract]
        bool AddEncomenda(Encomenda encomenda);

        /// <summary>
        /// Atualiza o estado de uma encomenda existente.
        /// </summary>
        /// <param name="encomendaId">ID da encomenda a ser atualizada.</param>
        /// <param name="novoEstado">Novo estado da encomenda (P ou E).</param>
        /// <returns>True se o estado for atualizado com sucesso; False caso contrário.</returns>
        [OperationContract]
        bool UpdateEncomendaEstado(int encomendaId, string novoEstado);

        /// <summary>
        /// Obtém os detalhes de uma encomenda específica.
        /// </summary>
        /// <param name="encomendaId">ID da encomenda.</param>
        /// <returns>Um objeto <see cref="Encomenda"/> contendo os detalhes da encomenda.</returns>
        [OperationContract]
        Encomenda GetEncomendaById(int encomendaId);

        /// <summary>
        /// Obtém a lista de todas as encomendas na base de dados.
        /// </summary>
        /// <returns>Uma lista de objetos <see cref="Encomenda"/>.</returns>
        [OperationContract]
        List<Encomenda> GetAllEncomendas();

        #endregion Encomenda_contract

        #region Movimentos_contract

        /// <summary>
        /// Obtém a lista de todos os movimentos de stock na base de dados.
        /// </summary>
        /// <returns>Uma lista de objetos do tipo Movimento.</returns>
        [OperationContract]
        List<Movimento> GetAllMovimentos();

        /// <summary>
        /// Adiciona um novo movimento de stock à base de dados.
        /// </summary>
        /// <param name="movimento">Objeto Movimento contendo os dados do novo movimento.</param>
        /// <returns>True se o movimento for adicionado com sucesso; False caso contrário.</returns>
        [OperationContract]
        bool AddMovimento(Movimento movimento);

        /// <summary>
        /// Obtém os movimentos filtrados por produto.
        /// </summary>
        [OperationContract]
        List<Movimento> GetMovimentosByProduto(int produtoId);

        /// <summary>
        /// Verifica se existe stock suficiente para realizar uma saída.
        /// </summary>
        [OperationContract]
        bool VerificarStockDisponivel(int produtoId, int quantidade);

        /// <summary>
        /// Obtém o histórico de movimentos realizados por um utilizador.
        /// </summary>
        [OperationContract]
        List<Movimento> GetHistoricoMovimentosByUtilizador(int utilizadorId);
        #endregion

        #region Utilizadores_contract

        /// <summary>
        /// Obtém todos os utilizadores do sistema.
        /// </summary>
        /// <returns>Lista de utilizadores.</returns>
        [OperationContract]
        List<User> GetAllUsers();
    }

        #endregion Utilizadores_contract

    }

        #region Fornecedor_datacontract
    /// <summary>
    /// Modelo que representa os dados de um fornecedor.
    /// Este modelo é usado como parte do contrato de dados (Data Contract) do serviço.
    /// </summary>
    [DataContract]
    public class Fornecedor
    {
        /// <summary>
        /// ID único do fornecedor.
        /// </summary>
        [DataMember(Order = 1)]
        public int FornecedorID { get; set; }

        /// <summary>
        /// Nome do fornecedor.
        /// </summary>
        [DataMember(Order = 2)]
        public string FornecedorNome { get; set; }

        /// <summary>
        /// Número de contacto do fornecedor.
        /// </summary>
        [DataMember(Order = 3)]
        public int Contacto { get; set; }

        /// <summary>
        /// Endereço do fornecedor.
        /// </summary>
        [DataMember(Order = 4)]
        public string Morada { get; set; }
    }
    #endregion fornecedor_datacontract

        #region Produto_datacontract
    /// <summary>
    /// Modelo que representa os dados de um produto.
    /// Este modelo é usado como parte do contrato de dados (Data Contract) do serviço.
    /// </summary>
    [DataContract]
    public class Produto
    {
        /// <summary>
        /// ID único do produto.
        /// </summary>
        [DataMember(Order = 1)]
        public int ProdutoID { get; set; }

        /// <summary>
        /// Nome do produto.
        /// </summary>
        [DataMember(Order = 2)]
        public string ProdutoNome { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        [DataMember(Order = 3)]
        public string Descricao { get; set; }

        /// <summary>
        /// Preço do produto.
        /// </summary>
        [DataMember(Order = 4)]
        public float Preco { get; set; }

        /// <summary>
        /// Stock disponível do produto.
        /// </summary>
        [DataMember(Order = 5)]
        public int Stock { get; set; }

        /// <summary>
        /// Estado do produto (Ativo, Inativo, etc.).
        /// </summary>
        [DataMember(Order = 6)]
        public string Estado { get; set; }
    }
    #endregion Produto_datacontract

        #region User_datacontract
    /// <summary>
    /// Modelo que representa os dados de um utilizador.
    /// Este modelo é usado como parte do contrato de dados (Data Contract) do serviço.
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// ID único do utilizador.
        /// </summary>
        [DataMember(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        [DataMember(Order = 2)]
        public string Nome { get; set; }

        /// <summary>
        /// Endereço de email do utilizador.
        /// </summary>
        [DataMember(Order = 3)]
        public string Email { get; set; }

        /// <summary>
        /// Palavra-passe do utilizador.
        /// </summary>
        [DataMember(Order = 4)]
        public string Password { get; set; }

        /// <summary>
        /// Papel ou função do utilizador no sistema (ex.: admin, user).
        /// </summary>
        [DataMember(Order = 5)]
        public string Role { get; set; }
    }
    #endregion User_datacontract

        #region Encomenda_datacontract

    /// <summary>
    /// Modelo que representa uma encomenda.
    /// </summary>
    [DataContract]
    public class Encomenda
    {
        [DataMember(Order = 1)]
        public int EncomendaID { get; set; }

        [DataMember(Order = 2)]
        public DateTime Data { get; set; }

        [DataMember(Order = 3)]
        public int FornecedorID { get; set; }

        [DataMember(Order = 4)]
        public int UtilizadorID { get; set; }

        [DataMember(Order = 5)]
        public string Estado { get; set; }

        [DataMember(Order = 6)]
        public List<DetalheEncomenda> Detalhes { get; set; }

        /// <summary>
        /// Construtor padrão da classe Encomenda.
        /// Inicializa a lista de detalhes.
        /// </summary>
        public Encomenda()
        {
            Detalhes = new List<DetalheEncomenda>();
        }
    }
    /// <summary>
    /// Modelo que representa os detalhes de uma encomenda.
    /// </summary>
    [DataContract]
    public class DetalheEncomenda
    {
        [DataMember(Order = 1)]
        public int EncomendaID { get; set; }

        [DataMember(Order = 2)]
        public int ProdutoID { get; set; }

        [DataMember(Order = 3)]
        public int Quantidade { get; set; }

        [DataMember(Order = 4)]
        public float PrecoTotal { get; set; }
    }

    #endregion Encomenda_datacontract

        #region Movimento_datacontract
    /// <summary>
    /// Modelo que representa os dados de um movimento de stock.
    /// Este modelo é usado como parte do contrato de dados (Data Contract) do serviço.
    /// </summary>
    [DataContract]
    public class Movimento
    {
        /// <summary>
        /// ID único do movimento.
        /// </summary>
        [DataMember(Order = 1)]
        public int MovimentoID { get; set; }

        /// <summary>
        /// ID do produto relacionado ao movimento.
        /// Este campo referencia o produto cujo stock foi movimentado.
        /// </summary>
        [DataMember(Order = 2)]
        public int ProdutoID { get; set; }

        /// <summary>
        /// Data do movimento.
        /// Representa o momento em que a entrada ou saída de stock foi registada.
        /// </summary>
        [DataMember(Order = 3)]
        public DateTime Data { get; set; }

        /// <summary>
        /// Tipo do movimento.
        /// Indica se é uma entrada ("I") ou saída ("O") de stock.
        /// </summary>
        [DataMember(Order = 4)]
        public string TipoInOut { get; set; }

        /// <summary>
        /// Quantidade movimentada.
        /// Representa o número de unidades do produto que entraram ou saíram do stock.
        /// </summary>
        [DataMember(Order = 5)]
        public int Quantidade { get; set; }

        /// <summary>
        /// ID do utilizador que realizou o movimento.
        /// Este campo referencia o utilizador responsável pelo registo do movimento.
        /// </summary>
        [DataMember(Order = 6)]
        public int UtilizadorID { get; set; }
    }
#endregion Movimento_datacontract

        #region Utilizadores_datacontract

        /// <summary>
        /// Modelo de utilizador utilizado no serviço SOAP.
        /// </summary>
        [DataContract]
        public class UserModel
        {
            /// <summary>
            /// ID único do utilizador.
            /// </summary>
            [DataMember(Order = 1)]
            public int Id { get; set; }

            /// <summary>
            /// Nome do utilizador.
            /// </summary>
            [DataMember(Order = 2)]
            public string Nome { get; set; }

            /// <summary>
            /// Email do utilizador.
            /// </summary>
            [DataMember(Order = 3)]
            public string Email { get; set; }

            /// <summary>
            /// Senha do utilizador.
            /// </summary>
            [DataMember(Order = 4)]
            public string Password { get; set; }

            /// <summary>
            /// Lista de roles (perfis) associados ao utilizador.
            /// </summary>
            [DataMember(Order = 5)]
            public List<string> Roles { get; set; }
        }

        #endregion Utilizadores_datacontract