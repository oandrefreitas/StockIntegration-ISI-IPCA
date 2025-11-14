using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.ServiceModel;

namespace WCFSTOCK
{
    /// <summary>
    /// Implementação dos contratos definidos na interface IService.
    /// </summary>
    public class Service : IService
    {

        private readonly string _connectionString;

        /// <summary>
        /// Construtor da classe Service.
        /// Inicializa a connection string a partir do Web.config.
        /// </summary>
        public Service()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;
        }

        #region Testar_conexao_service
        /// <summary>
        /// Testa a conexão com a base de dados.
        /// </summary>
        /// <returns>Mensagem indicando sucesso ou erro na conexão.</returns>
        public string TestarConexao()
        {
           
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return "Conexao com a base de dados bem-sucedida!";
                }
            }
            catch (Exception ex)
            {
                return string.Format("Erro ao conectar: {0}", ex.Message);
            }
        }
        #endregion Testar_conexao_service

        #region user_service
        /// <summary>
        /// Valida as credenciais de um utilizador.
        /// </summary>
        /// <param name="email">Email do utilizador.</param>
        /// <param name="password">Senha do utilizador.</param>
        /// <returns>True se as credenciais forem válidas; False caso contrário.</returns>
        public bool ValidateUser(string email, string password)
        {
            

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(1) FROM Utilizadores WHERE UtilizadorEmail = @Email AND UtilizadorSenha = @Password";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }



        /// <summary>
        /// Obtém os detalhes de um utilizador com base no email.
        /// </summary>
        /// <param name="email">Email do utilizador.</param>
        /// <returns>Um objeto User correspondente ao email fornecido, ou null se não encontrado.</returns>
        public User GetUserByEmail(string email)
        {
            

            using (var connection = new SqlConnection(_connectionString))
            {
                // Query para buscar os dados do utilizador pelo email
                string query = "SELECT UtilizadorId, UtilizadorNome, UtilizadorEmail, UtilizadorSenha, Role FROM Utilizadores WHERE UtilizadorEmail = @Email";

                using (var command = new SqlCommand(query, connection))
                {
                    // Adiciona o parâmetro email
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    // Verifica se o registro foi encontrado
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = (int)reader["UtilizadorId"], // Mapeia o Id
                            Nome = reader["UtilizadorNome"].ToString(), // Mapeia o Nome
                            Email = reader["UtilizadorEmail"].ToString(), // Mapeia o Email
                            Password = reader["UtilizadorSenha"].ToString(), // Mapeia a Password
                            Role = reader["Role"].ToString() // Mapeia o Role
                        };
                    }
                }
            }

            // Retorna null se o utilizador não for encontrado
            return null;
        }

        /// <summary>
        /// Adiciona um novo utilizador à base de dados.
        /// </summary>
        /// <param name="nome">Nome do utilizador.</param>
        /// <param name="email">Email do utilizador.</param>
        /// <param name="password">Senha do utilizador.</param>
        /// <param name="role">Role do utilizador.</param>
        /// <returns>True se o utilizador for adicionado com sucesso; False caso contrário.</returns>
        public bool AddUser(string nome, string email, string password, string role)
        {
           
            using (var connection = new SqlConnection(_connectionString))
            {
                // Query para inserir um novo utilizador na base de dados
                string query = @"
            INSERT INTO Utilizadores (UtilizadorNome, UtilizadorEmail, UtilizadorSenha, Role)
            VALUES (@Nome, @Email, @Password, @Role)";

                using (var command = new SqlCommand(query, connection))
                {
                    // Adiciona os parâmetros com os valores fornecidos
                    command.Parameters.AddWithValue("@Nome", nome);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0; // Retorna true se o registro foi inserido
                }
            }
        }

        #endregion user_service

        #region fornecedor_service
        /// <summary>
        /// Obtém todos os fornecedores da base de dados.
        /// </summary>
        /// <returns>Uma lista de objetos do tipo Fornecedor.</returns>
        public List<Fornecedor> GetAllFornecedores()
        {
            

            var fornecedores = new List<Fornecedor>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT FornecedorID, FornecedorNome, Contacto, Morada FROM Fornecedores";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        fornecedores.Add(new Fornecedor
                        {
                            FornecedorID = (int)reader["FornecedorID"],
                            FornecedorNome = reader["FornecedorNome"].ToString(),
                            Contacto = (int)reader["Contacto"],
                            Morada = reader["Morada"].ToString()
                        });
                    }
                }
            }

            return fornecedores;
        }

/// <summary>
/// Obtém os detalhes de um fornecedor específico pelo seu ID.
/// </summary>
public Fornecedor GetFornecedorById(int id)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT FornecedorID, FornecedorNome, Contacto, Morada FROM Fornecedores WHERE FornecedorID = @FornecedorID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FornecedorID", id);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Fornecedor
                        {
                            FornecedorID = (int)reader["FornecedorID"],
                            FornecedorNome = reader["FornecedorNome"].ToString(),
                            Contacto = (int)reader["Contacto"],
                            Morada = reader["Morada"].ToString()
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Adiciona um novo fornecedor à base de dados.
        /// </summary>
        public bool AddFornecedor(Fornecedor fornecedor)
        {
           

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO Fornecedores (FornecedorNome, Contacto, Morada)
                    VALUES (@FornecedorNome, @Contacto, @Morada)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FornecedorNome", fornecedor.FornecedorNome);
                    command.Parameters.AddWithValue("@Contacto", fornecedor.Contacto);
                    command.Parameters.AddWithValue("@Morada", fornecedor.Morada);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Atualiza os dados de um fornecedor existente.
        /// </summary>
        public bool UpdateFornecedor(Fornecedor fornecedor)
        {
     
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Fornecedores
                    SET FornecedorNome = @FornecedorNome,
                        Contacto = @Contacto,
                        Morada = @Morada
                    WHERE FornecedorID = @FornecedorID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FornecedorNome", fornecedor.FornecedorNome);
                    command.Parameters.AddWithValue("@Contacto", fornecedor.Contacto);
                    command.Parameters.AddWithValue("@Morada", fornecedor.Morada);
                    command.Parameters.AddWithValue("@FornecedorID", fornecedor.FornecedorID);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Remove um fornecedor da base de dados pelo ID.
        /// </summary>
        public bool DeleteFornecedor(int id)
        {
     
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Fornecedores WHERE FornecedorID = @FornecedorID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FornecedorID", id);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion fornecedor_service

        #region Produtos
        /// <summary>
        /// Obtém todos os produtos da base de dados.
        /// </summary>
        /// <returns>Uma lista de objetos do tipo Produto.</returns>
        public List<Produto> GetAllProdutos()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            var produtos = new List<Produto>();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ProdutoID, ProdutoNome, Descricao, Preco, Stock, Estado FROM Produtos";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        produtos.Add(new Produto
                        {
                            ProdutoID = (int)reader["ProdutoID"],
                            ProdutoNome = reader["ProdutoNome"].ToString(),
                            Descricao = reader["Descricao"].ToString(),
                            Preco = (float)reader["Preco"], // Converte de double para float
                            Stock = (int)reader["Stock"],
                            Estado = reader["Estado"].ToString()
                        });
                    }
                }
            }

            return produtos;
        }

        /// <summary>
        /// Obtém os detalhes de um produto específico pelo seu ID.
        /// </summary>
        public Produto GetProdutoById(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ProdutoID, ProdutoNome, Descricao, Preco, Stock, Estado FROM Produtos WHERE ProdutoID = @ProdutoID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProdutoID", id);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Produto
                        {
                            ProdutoID = (int)reader["ProdutoID"],
                            ProdutoNome = reader["ProdutoNome"].ToString(),
                            Descricao = reader["Descricao"].ToString(),
                            Preco = (float)reader["Preco"],
                            Stock = (int)reader["Stock"],
                            Estado = reader["Estado"].ToString()
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Adiciona um novo produto à base de dados.
        /// </summary>
        public bool AddProduto(Produto produto)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO Produtos (ProdutoNome, Descricao, Preco, Stock, Estado)
            VALUES (@ProdutoNome, @Descricao, @Preco, @Stock, @Estado)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProdutoNome", produto.ProdutoNome);
                    command.Parameters.AddWithValue("@Descricao", produto.Descricao);
                    command.Parameters.AddWithValue("@Preco", produto.Preco);
                    command.Parameters.AddWithValue("@Stock", produto.Stock);
                    command.Parameters.AddWithValue("@Estado", produto.Estado);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Atualiza os dados de um produto existente.
        /// </summary>
        public bool UpdateProduto(Produto produto)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE Produtos
            SET ProdutoNome = @ProdutoNome,
                Descricao = @Descricao,
                Preco = @Preco,
                Stock = @Stock,
                Estado = @Estado
            WHERE ProdutoID = @ProdutoID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProdutoNome", produto.ProdutoNome);
                    command.Parameters.AddWithValue("@Descricao", produto.Descricao);
                    command.Parameters.AddWithValue("@Preco", produto.Preco);
                    command.Parameters.AddWithValue("@Stock", produto.Stock);
                    command.Parameters.AddWithValue("@Estado", produto.Estado);
                    command.Parameters.AddWithValue("@ProdutoID", produto.ProdutoID);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Remove um produto da base de dados pelo ID.
        /// </summary>
        public bool DeleteProduto(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Produtos WHERE ProdutoID = @ProdutoID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProdutoID", id);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }


        #endregion Produtos

        #region encomenda
        // <summary>
        /// Cria uma nova encomenda com itens relacionados.
        /// </summary>
        /// <param name="encomenda">Dados da encomenda, incluindo os detalhes dos itens.</param>
        /// <returns>True se a encomenda for criada com sucesso; False caso contrário.</returns>
        public bool AddEncomenda(Encomenda encomenda)
        {
            // Obter a connection string do campo configurado na classe
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // Abrir a conexão com o banco de dados

                // Iniciar uma transação
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Inserir a encomenda na tabela Encomendas
                        var queryEncomenda = @"
                    INSERT INTO Encomendas (Data, FornecedoresFornecedorID, UtilizadoresUtilizadorID, Estado) 
                    OUTPUT INSERTED.EncomendaID 
                    VALUES (@Data, @FornecedorID, @UtilizadorID, @Estado)";

                        int encomendaId;
                        using (var cmd = new SqlCommand(queryEncomenda, connection, transaction))
                        {
                            // Adicionar os parâmetros para a query
                            cmd.Parameters.AddWithValue("@Data", encomenda.Data);
                            cmd.Parameters.AddWithValue("@FornecedorID", encomenda.FornecedorID);
                            cmd.Parameters.AddWithValue("@UtilizadorID", encomenda.UtilizadorID);
                            cmd.Parameters.AddWithValue("@Estado", encomenda.Estado);

                            // Executar a query e obter o ID gerado da encomenda
                            encomendaId = (int)cmd.ExecuteScalar();
                        }

                        // Inserir os detalhes (itens) da encomenda na tabela Detalhes_Encomendas
                        var queryDetalhes = @"
                    INSERT INTO Detalhes_Encomendas (EncomendasEncomendaID, ProdutosProdutoID, Quantidade, Preco_Total) 
                    VALUES (@EncomendaID, @ProdutoID, @Quantidade, @PrecoTotal)";

                        foreach (var detalhe in encomenda.Detalhes)
                        {
                            using (var cmd = new SqlCommand(queryDetalhes, connection, transaction))
                            {
                                // Adicionar os parâmetros para cada item
                                cmd.Parameters.AddWithValue("@EncomendaID", encomendaId);
                                cmd.Parameters.AddWithValue("@ProdutoID", detalhe.ProdutoID);
                                cmd.Parameters.AddWithValue("@Quantidade", detalhe.Quantidade);
                                cmd.Parameters.AddWithValue("@PrecoTotal", detalhe.PrecoTotal);

                                // Executar a query
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Confirmar a transação
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        // Reverter a transação em caso de erro
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Atualiza o estado de uma encomenda existente.
        /// </summary>
        /// <param name="encomendaId">ID da encomenda a ser atualizada.</param>
        /// <param name="novoEstado">Novo estado da encomenda (P ou E).</param>
        /// <returns>True se o estado for atualizado com sucesso; False caso contrário.</returns>
        public bool UpdateEncomendaEstado(int encomendaId, string novoEstado)
        {
            // Obter a connection string do campo configurado na classe
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // Abrir a conexão com o banco de dados

                try
                {
                    // Query para atualizar o estado da encomenda
                    var query = @"
                UPDATE Encomendas 
                SET Estado = @Estado 
                WHERE EncomendaID = @EncomendaID";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        // Adicionar os parâmetros para a query
                        cmd.Parameters.Add("@Estado", SqlDbType.NVarChar).Value = novoEstado;
                        cmd.Parameters.Add("@EncomendaID", SqlDbType.Int).Value = encomendaId;

                        // Executar a query e verificar se foi bem-sucedida
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch
                {
                    // Em caso de erro, retornar falso
                    return false;
                }
            }
        }

        /// <summary>
        /// Obtém os detalhes completos de uma encomenda específica.
        /// </summary>
        /// <param name="encomendaId">ID da encomenda.</param>
        /// <returns>Um objeto <see cref="Encomenda"/> contendo todos os detalhes da encomenda, ou null se não encontrado.</returns>
        public Encomenda GetEncomendaById(int encomendaId)
        {
            Encomenda encomenda = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    // Query para obter os dados gerais da encomenda
                    var queryEncomenda = @"
            SELECT EncomendaID, Data, FornecedoresFornecedorID, UtilizadoresUtilizadorID, Estado 
            FROM Encomendas 
            WHERE EncomendaID = @EncomendaID";

                    using (var cmd = new SqlCommand(queryEncomenda, connection))
                    {
                        cmd.Parameters.AddWithValue("@EncomendaID", encomendaId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Inicializa a encomenda com os dados básicos
                                encomenda = new Encomenda
                                {
                                    EncomendaID = (int)reader["EncomendaID"],
                                    Data = (DateTime)reader["Data"],
                                    FornecedorID = (int)reader["FornecedoresFornecedorID"],
                                    UtilizadorID = (int)reader["UtilizadoresUtilizadorID"],
                                    Estado = (string)reader["Estado"], // Converte para char
                                    Detalhes = new List<DetalheEncomenda>() // Inicializa a lista de detalhes
                                };
                            }
                        }
                    }

                    if (encomenda == null)
                    {
                        return null; // Retorna null se a encomenda não for encontrada
                    }

                    // Query para obter os detalhes da encomenda
                    var queryDetalhes = @"
            SELECT ProdutosProdutoID, Quantidade, Preco_Total 
            FROM Detalhes_Encomendas 
            WHERE EncomendasEncomendaID = @EncomendaID";

                    using (var cmd = new SqlCommand(queryDetalhes, connection))
                    {
                        cmd.Parameters.AddWithValue("@EncomendaID", encomendaId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Adiciona cada detalhe à lista de detalhes da encomenda
                                encomenda.Detalhes.Add(new DetalheEncomenda
                                {
                                    ProdutoID = (int)reader["ProdutosProdutoID"],
                                    Quantidade = (int)reader["Quantidade"],
                                    PrecoTotal = (float)reader["Preco_Total"] // Ajustado para decimal
                                });
                            }
                        }
                    }
                }
                catch
                {
                    // Retorna null em caso de erro
                    return null;
                }
            }

            return encomenda;
        }

        /// <summary>
        /// Obtém todas as encomendas e seus detalhes do banco de dados.
        /// </summary>
        /// <returns>Uma lista de encomendas com seus respectivos detalhes.</returns>
        public List<Encomenda> GetAllEncomendas()
        {
            var encomendas = new List<Encomenda>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    var query = @"
            SELECT 
                e.EncomendaID,
                e.Data,
                e.FornecedoresFornecedorID,
                e.UtilizadoresUtilizadorID,
                e.Estado,
                d.ProdutosProdutoID,
                d.Quantidade,
                d.Preco_Total
            FROM 
                Encomendas e
            LEFT JOIN 
                Detalhes_Encomendas d ON e.EncomendaID = d.EncomendasEncomendaID";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Mapa para evitar duplicatas de encomendas
                            var encomendasMap = new Dictionary<int, Encomenda>();

                            while (reader.Read())
                            {
                                // Obtém o ID da encomenda
                                int encomendaId = (int)reader["EncomendaID"];

                                // Verifica se a encomenda já existe no mapa
                                Encomenda encomenda;
                                if (!encomendasMap.TryGetValue(encomendaId, out encomenda))
                                {
                                    // Cria uma nova encomenda
                                    encomenda = new Encomenda
                                    {
                                        EncomendaID = encomendaId,
                                        Data = (DateTime)reader["Data"],
                                        FornecedorID = (int)reader["FornecedoresFornecedorID"],
                                        UtilizadorID = (int)reader["UtilizadoresUtilizadorID"],
                                        Estado = (string)reader["Estado"],
                                        Detalhes = new List<DetalheEncomenda>()
                                    };

                                    encomendasMap.Add(encomendaId, encomenda);
                                    encomendas.Add(encomenda);
                                }

                                // Verifica se há detalhes associados
                                if (reader["ProdutosProdutoID"] != DBNull.Value)
                                {
                                    // Adiciona um detalhe à encomenda
                                    var detalhe = new DetalheEncomenda
                                    {
                                        ProdutoID = (int)reader["ProdutosProdutoID"],
                                        Quantidade = (int)reader["Quantidade"],
                                        PrecoTotal = (float)reader["Preco_Total"] // Corrigido para decimal
                                    };

                                    encomenda.Detalhes.Add(detalhe);
                                }
                            }
                        }
                    }

                    return encomendas;
                }
                catch
                {
                    // Retorna uma lista vazia em caso de erro
                    return new List<Encomenda>();
                }
            }
        }


        #endregion encomenda

        #region Movimentos

        /// <summary>
        /// Obtém todos os movimentos de stock da base de dados.
        /// </summary>
        /// <returns>Uma lista de objetos do tipo Movimento.</returns>
        public List<Movimento> GetAllMovimentos()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            var movimentos = new List<Movimento>();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT MovimentoID, ProdutosProdutoID, Data, [Tipo InOut], Quantidade, UtilizadoresUtilizadorID
        FROM Movimentos_Stock";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        movimentos.Add(new Movimento
                        {
                            MovimentoID = (int)reader["MovimentoID"],
                            ProdutoID = (int)reader["ProdutosProdutoID"],
                            Data = (DateTime)reader["Data"],
                            TipoInOut = reader["Tipo InOut"].ToString(),
                            Quantidade = (int)reader["Quantidade"],
                            UtilizadorID = (int)reader["UtilizadoresUtilizadorID"]
                        });
                    }
                }
            }

            return movimentos;
        }

        /// <summary>
        /// Adiciona um novo movimento de stock à base de dados.
        /// </summary>
        public bool AddMovimento(Movimento movimento)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
        INSERT INTO Movimentos_Stock (ProdutosProdutoID, Data, [Tipo InOut], Quantidade, UtilizadoresUtilizadorID)
        VALUES (@ProdutoID, @Data, @TipoInOut, @Quantidade, @UtilizadorID)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProdutoID", movimento.ProdutoID);
                    command.Parameters.AddWithValue("@Data", movimento.Data);
                    command.Parameters.AddWithValue("@TipoInOut", movimento.TipoInOut);
                    command.Parameters.AddWithValue("@Quantidade", movimento.Quantidade);
                    command.Parameters.AddWithValue("@UtilizadorID", movimento.UtilizadorID);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Obtém todos os movimentos de stock relacionados a um produto específico.
        /// </summary>
        /// <param name="produtoId">O ID do produto.</param>
        /// <returns>Uma lista de objetos do tipo Movimento relacionados ao produto.</returns>
        public List<Movimento> GetMovimentosByProduto(int produtoId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            var movimentos = new List<Movimento>();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT MovimentoID, ProdutosProdutoID, Data, [Tipo InOut], Quantidade, UtilizadoresUtilizadorID
        FROM Movimentos_Stock
        WHERE ProdutosProdutoID = @ProdutoID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProdutoID", produtoId);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        movimentos.Add(new Movimento
                        {
                            MovimentoID = (int)reader["MovimentoID"],
                            ProdutoID = (int)reader["ProdutosProdutoID"],
                            Data = (DateTime)reader["Data"],
                            TipoInOut = reader["Tipo InOut"].ToString(),
                            Quantidade = (int)reader["Quantidade"],
                            UtilizadorID = (int)reader["UtilizadoresUtilizadorID"]
                        });
                    }
                }
            }

            return movimentos;
        }

        /// <summary>
        /// Verifica se existe stock suficiente para realizar uma saída.
        /// </summary>
        /// <param name="produtoId">O ID do produto a verificar.</param>
        /// <param name="quantidade">A quantidade a ser verificada.</param>
        /// <returns>True se houver stock suficiente, False caso contrário.</returns>
        public bool VerificarStockDisponivel(int produtoId, int quantidade)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            int stockDisponivel = 0;

            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT SUM(CASE WHEN [Tipo InOut] = 'I' THEN Quantidade ELSE -Quantidade END) AS StockAtual
        FROM Movimentos_Stock
        WHERE ProdutosProdutoID = @ProdutoID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProdutoID", produtoId);

                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        stockDisponivel = Convert.ToInt32(result);
                    }
                }
            }

            return stockDisponivel >= quantidade;
        }

        /// <summary>
        /// Obtém o histórico de movimentos realizados por um utilizador específico.
        /// </summary>
        /// <param name="utilizadorId">O ID do utilizador.</param>
        /// <returns>Uma lista de objetos do tipo Movimento relacionados ao utilizador.</returns>
        public List<Movimento> GetHistoricoMovimentosByUtilizador(int utilizadorId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            var movimentos = new List<Movimento>();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT MovimentoID, ProdutosProdutoID, Data, [Tipo InOut], Quantidade, UtilizadoresUtilizadorID
        FROM Movimentos_Stock
        WHERE UtilizadoresUtilizadorID = @UtilizadorID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UtilizadorID", utilizadorId);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        movimentos.Add(new Movimento
                        {
                            MovimentoID = (int)reader["MovimentoID"],
                            ProdutoID = (int)reader["ProdutosProdutoID"],
                            Data = (DateTime)reader["Data"],
                            TipoInOut = reader["Tipo InOut"].ToString(),
                            Quantidade = (int)reader["Quantidade"],
                            UtilizadorID = (int)reader["UtilizadoresUtilizadorID"]
                        });
                    }
                }
            }

            return movimentos;
        }

        #endregion Movimentos

        #region Utilizadores

        /// <summary>
        /// Obtém todos os utilizadores do sistema.
        /// </summary>
        /// <returns>Lista de utilizadores.</returns>
        /// <summary>
        /// Obtém todos os utilizadores do sistema.
        /// </summary>
        /// <returns>Lista de utilizadores do tipo User.</returns>
        public List<User> GetAllUsers()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

            var users = new List<User>();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT UtilizadorID, UtilizadorNome, UtilizadorEmail, UtilizadorSenha, Role
        FROM Utilizadores";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var user = new User
                        {
                            Id = (int)reader["UtilizadorID"],
                            Nome = reader["UtilizadorNome"].ToString(),
                            Email = reader["UtilizadorEmail"].ToString(),
                            Password = reader["UtilizadorSenha"].ToString(),
                            Role = reader["Role"].ToString()
                        };

                        users.Add(user);
                    }
                }
            }

            return users;
        }
    }
    #endregion Utilizadores
}
