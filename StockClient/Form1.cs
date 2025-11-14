using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockClient.Models;

namespace StockClient
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            #region OpçõesComboBox
            // Inicializa o ComboBox com os estados de encomenda permitidos
            comboBox1.Items.Add("P"); // Pedida
            comboBox1.Items.Add("E"); // Entregue
           // Configura o ComboBox para impedir entrada manual
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            // Inicializa o ComboBox com os estados de encomenda permitidos_4
            comboBox4.Items.Add("P"); // Pedida
            comboBox4.Items.Add("E"); // Entregue
                                      // Configura o ComboBox para impedir entrada manual
            comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;

            // Preenche o ComboBox com opções de currency
            comboBox2.Items.Add("USD");
            comboBox2.Items.Add("GBP");
            comboBox2.Items.Add("JPY");
            comboBox2.Items.Add("AUD");
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList; // Apenas seleção permitida


            // Preenche o ComboBox com opções do Estado dos produtos
            comboBoxProdutoEstado.Items.Add("A");
            comboBoxProdutoEstado.Items.Add("I");
            comboBoxProdutoEstado.Items.Add("D");
            comboBoxProdutoEstado.DropDownStyle = ComboBoxStyle.DropDownList;

            // Configura o ComboBox para movimentos
            comboBoxTipoInOut.Items.Add("I");
            comboBoxTipoInOut.Items.Add("O");
            comboBoxTipoInOut.DropDownStyle = ComboBoxStyle.DropDownList;
            #endregion OpçõesComboBox

            #region TabControl
            // Carrega os fornecedores na inicialização, pois a aba "Fornecedores" é a inicial
            LoadInitialTabData();

            // Associar o evento de mudança de aba
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
        }

        /// <summary>
        /// Carrega os dados da aba inicial ao abrir o formulário.
        /// </summary>
        private async void LoadInitialTabData()
        {
            if (tabControl1.SelectedTab.Name == "tabPage3") // Verifica se está na aba "Fornecedores"
            {
                await LoadSuppliersForGrid(); // Carrega os fornecedores automaticamente
            }
        }

        /// <summary>
        /// Evento para carregar dados ao mudar de aba.
        /// </summary>
        private async void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Name == "tabPage3") // Aba de Fornecedores
            {
                await LoadSuppliersForGrid();
            }
            else if (tabControl1.SelectedTab.Name == "tabPage4") // Aba de Encomendas
            {
                await LoadEncomendas();
                await LoadSuppliersForComboBox();
                await LoadProdutoForComboBox();

            }
            else if (tabControl1.SelectedTab.Name == "tabPage5") // Aba de Produtos
            {
                await LoadProdutoForComboBox2();
            }
            else if (tabControl1.SelectedTab.Name == "tabPage1") // Aba de Movimentos
            {
                await LoadMovimentos();
                await LoadUtilizadoresForComboBox();
            }
        }
        #endregion TabControl

        #region TAB3_Fornecedores
        /// <summary>
        /// Fetch fornecedeores
        /// </summary>
        /// <returns></returns>
        private async Task<List<Supplier>> FetchSuppliers()
        {
            try
            {
                var response = await GlobalConfig.HttpClient.GetAsync("Fornecedores/GetFornecedores");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var suppliers = JsonSerializer.Deserialize<List<Supplier>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return suppliers ?? new List<Supplier>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar fornecedores: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Supplier>();
            }
        }

        private async Task LoadSuppliersForGrid()
        {
            var suppliers = await FetchSuppliers();

            if (suppliers.Any())
            {
                dataGridView1.DataSource = suppliers;
                dataGridView1.Columns["FornecedorID"].ReadOnly = true;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
            }
            else
            {
                MessageBox.Show("Nenhum fornecedor encontrado.");
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        /// <summary>
        /// Botão gravar edição de fornecedor
        /// campo id bloqueado para edição
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button2_Click(object sender, EventArgs e)
        {

            try
            {
                // Obtém os dados do DataGridView como uma lista de fornecedores
                var suppliers = (List<Supplier>)dataGridView1.DataSource;

                foreach (var supplier in suppliers)
                {
                    // Envia os dados atualizados para a API
                    var jsonContent = JsonSerializer.Serialize(supplier);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await GlobalConfig.HttpClient.PostAsync($"Fornecedores/AtualizarFornecedor", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Erro ao atualizar fornecedor {supplier.Nome}: {response.StatusCode}");
                    }
                }

                MessageBox.Show("Fornecedores atualizados com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar alterações: {ex.Message}");
            }
        }


        /// <summary>
        /// Botão para adicionar novo fornecedor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {

            try
            {
                // Valida os campos
                if (string.IsNullOrEmpty(TextBoxForNome.Text.Trim()) ||
                    string.IsNullOrEmpty(TextBoxForCont.Text.Trim()) ||
                    string.IsNullOrEmpty(TextBoxForMor.Text.Trim()))
                {
                    MessageBox.Show("Por favor, preencha todos os campos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(TextBoxForCont.Text.Trim(), out int contacto))
                {
                    MessageBox.Show("O contacto deve conter 9 digitos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cria o objeto do fornecedor
                var newSupplier = new Supplier
                {
                    Nome = TextBoxForNome.Text.Trim(),
                    Contacto = contacto,
                    Morada = TextBoxForMor.Text.Trim()
                };

                // Serializa os dados para JSON
                var jsonContent = JsonSerializer.Serialize(newSupplier);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                  

                // Envia a requisição POST para a API
                var response = await GlobalConfig.HttpClient.PostAsync("Fornecedores/CriarFornecedor", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Fornecedor adicionado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Opcional: Limpa os campos de entrada
                    TextBoxForNome.Clear();
                    TextBoxForCont.Clear();
                    TextBoxForMor.Clear();

                    // Opcional: Atualiza a lista de fornecedores
                    await LoadSuppliersForGrid();
                }
                else
                {
                    MessageBox.Show($"Erro ao adicionar fornecedor: {response.ReasonPhrase}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar fornecedor: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Botão para apagar fornecedor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EliminarFornecedor_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida se um fornecedor foi selecionado
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Por favor, selecione um fornecedor para apagar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtém os dados do fornecedor selecionado
                var selectedRow = dataGridView1.SelectedRows[0];
                var fornecedor = new Supplier
                {
                    FornecedorID = (int)selectedRow.Cells["FornecedorID"].Value,
                    Nome = selectedRow.Cells["Nome"].Value.ToString(),
                    Contacto = (int)selectedRow.Cells["Contacto"].Value,
                    Morada = selectedRow.Cells["Morada"].Value.ToString()
                };

                // Confirma a exclusão
                var confirmResult = MessageBox.Show($"Tem certeza que deseja apagar o fornecedor {fornecedor.Nome}?",
                                                    "Confirmação",
                                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.No)
                    return;

                // Serializa o objeto fornecedor para JSON
                var jsonContent = JsonSerializer.Serialize(fornecedor);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Envia a requisição POST para remover o fornecedor
                var response = await GlobalConfig.HttpClient.PostAsync("Fornecedores/RemoverFornecedor", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Fornecedor apagado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Atualiza a lista de fornecedores
                    await LoadSuppliersForGrid();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao apagar fornecedor: {response.ReasonPhrase}\nDetalhes: {errorContent}",
                                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao apagar fornecedor: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion TAB3_Fornecedores

        #region TAB4_Encomendas

        /// <summary>
        /// Criar modelo encomendadetalhada
        /// </summary>
        /// <param name="encomendas"></param>
        /// <returns></returns>
        private List<EncomendaDetalhadaModel> TransformarDadosEncomendas(List<EncomendaModel> encomendas)
        {
            var encomendasDetalhadas = new List<EncomendaDetalhadaModel>();

            foreach (var encomenda in encomendas)
            {
                // Para cada detalhe da encomenda, cria uma linha no modelo achatado
                foreach (var detalhe in encomenda.Detalhes)
                {
                    encomendasDetalhadas.Add(new EncomendaDetalhadaModel
                    {
                        EncomendaID = encomenda.EncomendaID,
                        Data = encomenda.Data,
                        FornecedorID = encomenda.FornecedorID,
                        UtilizadorID = encomenda.UtilizadorID,
                        Estado = encomenda.Estado,
                        ProdutoID = detalhe.ProdutoID,
                        Quantidade = detalhe.Quantidade,
                        PrecoTotal = detalhe.PrecoTotal
                    });
                }
            }

            return encomendasDetalhadas;
        }

        private async Task LoadEncomendas()
        {
            try
            {
                // Faz a requisição GET para obter todas as encomendas
                var response = await GlobalConfig.HttpClient.GetAsync("Encomendas/TodasEncomendas");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // Desserializa o JSON para uma lista de EncomendaModel
                    var encomendas = JsonSerializer.Deserialize<List<EncomendaModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (encomendas != null && encomendas.Any())
                    {
                        // Transforma os dados para o modelo achatado
                        var encomendasDetalhadas = TransformarDadosEncomendas(encomendas);

                        // Configura o DataGridView
                        dataGridViewEncomendas.DataSource = encomendasDetalhadas;

                        // Configura os cabeçalhos das colunas
                        dataGridViewEncomendas.Columns["EncomendaID"].HeaderText = "ID da Encomenda";
                        dataGridViewEncomendas.Columns["Data"].HeaderText = "Data";
                        dataGridViewEncomendas.Columns["FornecedorID"].HeaderText = "Fornecedor";
                        dataGridViewEncomendas.Columns["UtilizadorID"].HeaderText = "Utilizador";
                        dataGridViewEncomendas.Columns["Estado"].HeaderText = "Estado";
                        dataGridViewEncomendas.Columns["ProdutoID"].HeaderText = "Produto";
                        dataGridViewEncomendas.Columns["Quantidade"].HeaderText = "Quantidade";
                        dataGridViewEncomendas.Columns["PrecoTotal"].HeaderText = "Preço Total";

                        // Ajuste opcional para preencher a largura do DataGridView
                        dataGridViewEncomendas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                    else
                    {
                        MessageBox.Show("Nenhuma encomenda encontrada.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dataGridViewEncomendas.DataSource = null;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter encomendas: {response.StatusCode}\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar encomendas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Verifica se há uma linha selecionada
                if (dataGridViewEncomendas.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Por favor, selecione uma encomenda para atualizar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtém a linha selecionada
                var selectedRow = dataGridViewEncomendas.SelectedRows[0];

                // Obtém o ID da encomenda (garanta que a coluna "EncomendaID" existe e está visível)
                var encomendaID = (int)selectedRow.Cells["EncomendaID"].Value;

                // Obtém o novo estado selecionado no ComboBox
                var novoEstado = comboBox1.SelectedItem?.ToString();

                // Verifica se o estado é válido
                if (string.IsNullOrEmpty(novoEstado) || (novoEstado != "P" && novoEstado != "E"))
                {
                    MessageBox.Show("Por favor, selecione um estado válido ('P' ou 'E').", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Envia a atualização para o servidor
                var content = new StringContent($"\"{novoEstado}\"", Encoding.UTF8, "application/json");
                var response = await GlobalConfig.HttpClient.PutAsync($"Encomendas/AtualizaEstado?id={encomendaID}", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Estado atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recarrega as encomendas para refletir a alteração
                    await LoadEncomendas();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao atualizar estado: {response.ReasonPhrase}\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar estado: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Obtém a taxa de câmbio de uma moeda em relação ao Euro.
        /// </summary>
        /// <param name="moeda"></param>
        /// <returns></returns>
        private async Task<decimal> ObterTaxaCambio(string moeda)
        {
            try
            {
                // Faz a requisição GET ao endpoint do servidor para obter a taxa de câmbio
                var response = await GlobalConfig.HttpClient.GetAsync($"Currency/TaxaCambio?currency={moeda}");

                // Verifica se a resposta foi bem-sucedida
                if (response.IsSuccessStatusCode)
                {
                    // Lê o conteúdo da resposta
                    var content = await response.Content.ReadAsStringAsync();

                    // Desserializa o conteúdo JSON para o modelo CurrencyRate
                    var currencyRate = JsonSerializer.Deserialize<CurrencyRate>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Ignora diferenças de maiúsculas/minúsculas
                    });

                    // Retorna a taxa de câmbio se o objeto não for nulo
                    if (currencyRate != null)
                    {
                        return currencyRate.Rate;
                    }
                    else
                    {
                        MessageBox.Show("Erro: A resposta do servidor está vazia.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0;
                    }
                }
                else
                {
                    // Exibe o erro em caso de resposta mal-sucedida
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter a taxa de câmbio: {response.StatusCode}\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                // Trata erros de conexão ou outras exceções
                MessageBox.Show($"Erro ao conectar ao servidor: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// MOSTRAR TAXA DE CÂMBIO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Obtém a moeda selecionada
                var moeda = comboBox2.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(moeda))
                {
                    label8.Text = "Selecione uma moeda válida.";
                    return;
                }

                // Obtém a taxa de câmbio da moeda selecionada
                var taxaCambio = await ObterTaxaCambio(moeda);

                if (taxaCambio > 0)
                {
                    // Exibe a taxa de câmbio no Label
                    label8.Text = $"1 {moeda} = {taxaCambio:F2} EUR";
                }
                else
                {
                    label8.Text = "Taxa não disponível.";
                }
            }
            catch (Exception ex)
            {
                label8.Text = "Erro ao obter taxa.";
                MessageBox.Show($"Erro ao obter a taxa de câmbio: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ///Desenvolvimento da parte de adicionar encomenda
        private async Task LoadSuppliersForComboBox()
        {
            var suppliers = await FetchSuppliers();

            if (suppliers.Any())
            {
                comboBox3.DataSource = suppliers;
                comboBox3.DisplayMember = "Nome"; // Mostra o nome do fornecedor
                comboBox3.ValueMember = "FornecedorID"; // Valor associado é o ID
            }
            else
            {
                MessageBox.Show("Nenhum fornecedor encontrado.");
            }
        }

        /// <summary>
        /// Combo box para fornecedores
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.SelectedItem != null)
                {
                    // Obtém o ID do fornecedor selecionado diretamente do ComboBox
                    var fornecedorID = comboBox3.SelectedValue;

                    // Operação assíncrona de exemplo (se necessário)
                    await Task.Delay(10);
                  
                }
                else
                {
                    MessageBox.Show("Por favor, selecione um fornecedor.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// combo box estado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Adicionar encomenda
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Adiciona uma nova encomenda com base nos campos preenchidos na interface.
        /// </summary>
        private async Task AdicionarEncomenda()
        {
            try
            {
                // Valida os campos obrigatórios
                if (comboBox3.SelectedValue == null)
                {
                    MessageBox.Show("Por favor, selecione um fornecedor.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (comboBox4.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, selecione um estado válido ('P' ou 'E').", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if  (string.IsNullOrEmpty(textBox2.Text) ||
                    string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Por favor, preencha todos os campos dos detalhes da encomenda.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Valida e converte os valores dos TextBoxes
                if (int.TryParse(textBox2.Text, out int quantidade) ||
                    !float.TryParse(textBox3.Text, out float precoTotal))
                {
                    MessageBox.Show("Por favor, insira valores válidos para Produto ID, Quantidade e Preço Total.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cria o objeto EncomendaDetalhadaModel
                var novaEncomenda = new NovaEncomendaModel
                {
                    FornecedorID = (int)comboBox3.SelectedValue, // Obtém o ID do fornecedor
                    UtilizadorID = GlobalConfig.UserID, // Obtém o ID do utilizador autenticado
                    Estado = comboBox4.SelectedItem?.ToString() ?? string.Empty, // Estado selecionado
                    Data = dateTimePicker1.Value, // Data da encomenda
                    Detalhes = new List<DetalheEncomendaModel>
            {
                new DetalheEncomendaModel
                {
                    ProdutoID = (int)comboBox5.SelectedValue, // Produto ID
                    Quantidade = quantidade, // Quantidade do produto
                    PrecoTotal = precoTotal // Preço total do produto
                }
            }
                };

                // Serializa o objeto para JSON
                var jsonContent = JsonSerializer.Serialize(novaEncomenda);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Envia a requisição POST para o endpoint REST
                var response = await GlobalConfig.HttpClient.PostAsync("Encomendas/AdicionarEncomenda", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Encomenda adicionada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpa os campos após adicionar a encomenda
                    LimparCamposEncomenda();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao adicionar encomenda: {response.ReasonPhrase}\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar encomenda: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Limpa os campos do formulário após adicionar a encomenda.
        /// </summary>
        private void LimparCamposEncomenda()
        {
           
            comboBox4.SelectedIndex = -1; // Limpa o estado selecionado
            textBox2.Clear(); // Limpa a quantidade
            textBox3.Clear(); // Limpa o preço total
            dateTimePicker1.Value = DateTime.Now; // Define a data para hoje
        }



        private async void button3_Click(object sender, EventArgs e)
        {

            await AdicionarEncomenda();

        }


        #endregion TAB4_Encomendas

        #region TAB5_Produtos

        private async void tabProdutos_Enter(object sender, EventArgs e)
        {
            await LoadProdutosForGrid(); // Carrega os produtos ao acessar a aba
        }


        /// <summary>
        /// Faz o fetch dos produtos a partir do REST API.
        /// </summary>
        /// <returns>Lista de produtos.</returns>
        private async Task<List<Produto>> FetchProdutos()
        {
            try
            {
                // Faz a chamada GET para o endpoint de produtos
                var response = await GlobalConfig.HttpClient.GetAsync("https://localhost:7292/api/Produtos");

                // Verifica manualmente o código de status
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Caso o endpoint não seja encontrado, exibe a mensagem e retorna uma lista vazia
                    MessageBox.Show("Endpoint não encontrado: Verifique a URL ou o servidor REST.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Produto>();
                }

                // Lança uma exceção se o código de status não for bem-sucedido
                response.EnsureSuccessStatusCode();

                // Lê o conteúdo da resposta
                var content = await response.Content.ReadAsStringAsync();

                // Desserializa a resposta para uma lista de Produto
                var produtos = JsonSerializer.Deserialize<List<Produto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return produtos ?? new List<Produto>();
            }
            catch (HttpRequestException ex)
            {
                // Exibe uma mensagem genérica apenas para erros de conexão ou rede
                MessageBox.Show($"Erro ao carregar produtos: Verifique a conexão com o servidor.\nDetalhes: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Produto>();
            }
            catch (Exception ex)
            {
                // Exibe mensagens para outros tipos de erros
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Produto>();
            }
        }

        /// <summary>
        /// Carrega os produtos no DataGridView.
        /// </summary>
        private async Task LoadProdutosForGrid()
        {
            var produtos = await FetchProdutos();

            if (produtos.Any())
            {
                dataGridViewProdutos.DataSource = produtos;

                // Configura colunas específicas para serem somente leitura
                dataGridViewProdutos.Columns["ProdutoID"].ReadOnly = true;

                // Configura o modo de seleção
                dataGridViewProdutos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridViewProdutos.MultiSelect = false;

                // Ajusta automaticamente o tamanho das colunas
                dataGridViewProdutos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                MessageBox.Show("Nenhum produto encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        /// <summary>
        /// Botão para adicionar um novo produto.
        /// </summary>
        private async void buttonAdicionarProduto_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Valida os campos
                if (string.IsNullOrEmpty(textBoxProdutoNome.Text.Trim()) ||
                    string.IsNullOrEmpty(textBoxProdutoDescricao.Text.Trim()) ||
                    string.IsNullOrEmpty(textBoxProdutoPreco.Text.Trim()) ||
                    string.IsNullOrEmpty(textBoxProdutoStock.Text.Trim()) ||
                    string.IsNullOrEmpty(comboBoxProdutoEstado.Text.Trim()))
                {
                    MessageBox.Show("Por favor, preencha todos os campos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!float.TryParse(textBoxProdutoPreco.Text.Trim(), out float preco) || preco <= 0)
                {
                    MessageBox.Show("Por favor, insira um preço válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(textBoxProdutoStock.Text.Trim(), out int stock) || stock < 0)
                {
                    MessageBox.Show("Por favor, insira um valor válido para o stock.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cria o objeto ProdutoModel
                var novoProduto = new Produto
                {
                    ProdutoNome = textBoxProdutoNome.Text.Trim(),
                    Descricao = textBoxProdutoDescricao.Text.Trim(),
                    Preco = preco,
                    Stock = stock,
                    Estado = comboBoxProdutoEstado.Text.Trim()
                };

                // Serializa o objeto para JSON
                var jsonContent = JsonSerializer.Serialize(novoProduto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Envia a requisição POST para o endpoint de criação
                var response = await GlobalConfig.HttpClient.PostAsync("Produtos/CriarProduto", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Produto adicionado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Atualiza a lista de produtos no DataGridView
                    await LoadProdutosForGrid();

                    // Limpa os campos de entrada
                    textBoxProdutoNome.Clear();
                    textBoxProdutoDescricao.Clear();
                    textBoxProdutoPreco.Clear();
                    textBoxProdutoStock.Clear();
                    comboBoxProdutoEstado.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show($"Erro ao adicionar produto: {response.ReasonPhrase}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar produto: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Botão para atualizar um produto existente.
        /// </summary>
        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtém o produto selecionado no DataGridView
                if (dataGridViewProdutos.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Por favor, selecione um produto para editar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var selectedRow = dataGridViewProdutos.SelectedRows[0];
                var produto = new Produto
                {
                    ProdutoID = (int)selectedRow.Cells["ProdutoID"].Value,
                    ProdutoNome = selectedRow.Cells["ProdutoNome"].Value.ToString(),
                    Descricao = selectedRow.Cells["Descricao"].Value.ToString(),
                    Preco = Convert.ToSingle(selectedRow.Cells["Preco"].Value),
                    Stock = (int)selectedRow.Cells["Stock"].Value,
                    Estado = selectedRow.Cells["Estado"].Value.ToString()
                };

               

                // Serializa o produto para JSON
                var jsonContent = JsonSerializer.Serialize(produto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Envia a requisição POST para o endpoint de atualização
                var response = await GlobalConfig.HttpClient.PostAsync("Produtos/AtualizarProduto", content);

                


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Produto atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Atualiza a lista de produtos no DataGridView
                    await LoadProdutosForGrid();
                }
                else
                {
                    MessageBox.Show($"Erro ao atualizar produto: {response.ReasonPhrase}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar produto: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion TAB5_Produtos

        #region TAB6_Movimentos

        /// <summary>
        /// Carrega todos os movimentos de stock e exibe-os no DataGridView.
        /// </summary>
        private async Task LoadMovimentos()
        {
            try
            {
                // Faz a requisição GET para obter todos os movimentos
                var response = await GlobalConfig.HttpClient.GetAsync("Movimentos/TodosMovimentos");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // Desserializa o JSON para uma lista de MovimentoModel
                    var movimentos = JsonSerializer.Deserialize<List<MovimentoModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (movimentos != null && movimentos.Any())
                    {
                        // Configura o DataGridView
                        dataGridViewMovimentos.DataSource = movimentos;

                        // Configura os cabeçalhos das colunas
                        dataGridViewMovimentos.Columns["MovimentoID"].HeaderText = "ID do Movimento";
                        dataGridViewMovimentos.Columns["Data"].HeaderText = "Data";
                        dataGridViewMovimentos.Columns["ProdutoID"].HeaderText = "Produto";
                        dataGridViewMovimentos.Columns["Quantidade"].HeaderText = "Quantidade";
                        dataGridViewMovimentos.Columns["TipoInOut"].HeaderText = "Tipo (In/Out)";
                        dataGridViewMovimentos.Columns["UtilizadorID"].HeaderText = "Utilizador";

                        // Ajuste opcional para preencher a largura do DataGridView
                        dataGridViewMovimentos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                    else
                    {
                        MessageBox.Show("Nenhum movimento encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dataGridViewMovimentos.DataSource = null;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter movimentos: {response.StatusCode}\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar movimentos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Adiciona um novo movimento de stock.
        /// </summary>
        private async Task AdicionarMovimento()
        {
            try
            {
                // Valida os campos obrigatórios
                if (comboBoxUtilizador.SelectedValue == null)
                {
                    MessageBox.Show("Por favor, selecione um utilizador.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (comboBoxTipoInOut.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, selecione um tipo válido ('I' ou 'O').", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Valida e converte os valores dos TextBoxes
                if (!int.TryParse(textBoxQuantidade.Text, out int quantidade))
                {
                    MessageBox.Show("Por favor, insira valores válidos para Produto ID e Quantidade.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cria o objeto MovimentoModel
                var novoMovimento = new MovimentoModel
                {
                    ProdutoID = (int)comboBox6.SelectedValue,
                    Quantidade = quantidade,
                    TipoInOut = comboBoxTipoInOut.SelectedItem?.ToString() ?? string.Empty, // Tipo de movimento (In/Out)
                    Data = dateTimePickerMovimentos.Value, // Data do movimento
                    UtilizadorID = (int)comboBoxUtilizador.SelectedValue // Utilizador responsável
                };

                // Serializa o objeto para JSON
                var jsonContent = JsonSerializer.Serialize(novoMovimento);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Envia a requisição POST para o endpoint REST
                var response = await GlobalConfig.HttpClient.PostAsync("Movimentos/AdicionarMovimento", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Movimento adicionado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recarrega os movimentos para refletir a alteração
                    await LoadMovimentos();

                    // Limpa os campos após adicionar o movimento
                    LimparCamposMovimento();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao adicionar movimento: {response.ReasonPhrase}\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar movimento: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Limpa os campos do formulário após adicionar o movimento.
        /// </summary>
        private void LimparCamposMovimento()
        {
            comboBoxTipoInOut.SelectedIndex = -1; // Limpa o tipo selecionado
            comboBoxUtilizador.SelectedIndex = -1; // Limpa o utilizador selecionado
            textBoxQuantidade.Clear(); // Limpa a quantidade
            dateTimePickerMovimentos.Value = DateTime.Now; // Define a data para hoje
        }

        /// <summary>
        /// Botão para adicionar movimento.
        /// </summary>
        private async void button7_Click(object sender, EventArgs e)
        {
            await AdicionarMovimento();
        }

        /// <summary>
        /// Carrega os utilizadores para o ComboBox.
        /// </summary>
        private async Task LoadUtilizadoresForComboBox()
        {
            var utilizadores = await FetchUtilizadores();

            if (utilizadores.Any())
            {
                comboBoxUtilizador.DataSource = utilizadores;
                comboBoxUtilizador.DisplayMember = "Nome"; // Mostra o nome do utilizador
                comboBoxUtilizador.ValueMember = "Id"; 
            }
            else
            {
                MessageBox.Show("Nenhum utilizador encontrado.");
            }
        }

        private async Task LoadProdutoForComboBox()
        {
            var produtos = await FetchProdutos();

            if (produtos.Any())
            {
                comboBox6.DataSource = produtos;
                comboBox6.DisplayMember = "ProdutoNome"; // Mostra o nome do utilizador
                comboBox6.ValueMember = "ProdutoID";

            }
            else
            {
                MessageBox.Show("Nenhum utilizador encontrado.");
            }
        }

        private async Task LoadProdutoForComboBox2()
        {
            var produtos = await FetchProdutos();

            if (produtos.Any())
            {
                comboBox5.DataSource = produtos;
                comboBox5.DisplayMember = "ProdutoNome"; // Mostra o nome do utilizador
                comboBox5.ValueMember = "ProdutoID";

            }
            else
            {
                MessageBox.Show("Nenhum utilizador encontrado.");
            }
        }

        #endregion TAB6_Movimentos

        #region TAB7_Utilizadores

        private async void tabUtilizadores_Enter(object sender, EventArgs e)
        {
            await LoadUtilizadoresForGrid(); // Carrega os utilizadores ao acessar a aba
        }


        /// <summary>
        /// Fetch utilizadores do sistema.
        /// </summary>
        /// <returns>Lista de objetos UtilizadorModel.</returns>
        private async Task<List<UtilizadorModel>> FetchUtilizadores()
        {
            try
            {
                var response = await GlobalConfig.HttpClient.GetAsync("Utilizadores/GetAllUsers");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var utilizadores = JsonSerializer.Deserialize<List<UtilizadorModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return utilizadores ?? new List<UtilizadorModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar utilizadores: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<UtilizadorModel>();
            }
        }

        /// <summary>
        /// Carrega os utilizadores para o DataGridView
        /// </summary>
        /// <returns></returns>
        private async Task LoadUtilizadoresForGrid()
        {
            var utilizadores = await FetchUtilizadores();

            if (utilizadores.Any())
            {
                dataGridViewUtilizadores.DataSource = utilizadores;
                dataGridViewUtilizadores.Columns["Id"].ReadOnly = true; // Define o ID como somente leitura
                dataGridViewUtilizadores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridViewUtilizadores.MultiSelect = false;
                dataGridViewUtilizadores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajusta automaticamente as colunas
            }
            else
            {
                MessageBox.Show("Nenhum utilizador encontrado.");
            }
        }








        #endregion TAB7_Utilizadores

        
        
    }
}