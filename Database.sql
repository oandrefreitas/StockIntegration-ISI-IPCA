CREATE TABLE Produtos (
  ProdutoID   int IDENTITY NOT NULL, 
  ProdutoNome varchar(50) NULL, 
  Descricao   varchar(255) NULL, 
  Preco       float(10) NULL, 
  Stock       int NULL, 
  Estado      char(1) NULL, 
  PRIMARY KEY (ProdutoID));
CREATE TABLE Fornecedores (
  FornecedorID   int IDENTITY NOT NULL, 
  FornecedorNome varchar(50) NULL, 
  Contacto       int NULL, 
  Morada         varchar(100) NULL, 
  PRIMARY KEY (FornecedorID));
CREATE TABLE Detalhes_Encomendas (
  EncomendasEncomendaID int NOT NULL, 
  ProdutosProdutoID     int NOT NULL, 
  Quantidade            int NULL, 
  Preco_Total           float(10) NULL, 
  PRIMARY KEY (EncomendasEncomendaID, 
  ProdutosProdutoID));
CREATE TABLE Movimentos_Stock (
  MovimentoID              int IDENTITY NOT NULL, 
  ProdutosProdutoID        int NOT NULL, 
  Data                     datetime NULL, 
  [Tipo InOut]             char(1) NULL, 
  Quantidade               int NULL, 
  UtilizadoresUtilizadorID int NOT NULL, 
  PRIMARY KEY (MovimentoID));
CREATE TABLE Utilizadores (
  UtilizadorID    int IDENTITY NOT NULL, 
  UtilizadorNome  varchar(50) NULL, 
  UtilizadorEmail varchar(50) NULL, 
  UtilizadorSenha varchar(50) NULL, 
  Role            varchar(25) NULL, 
  PRIMARY KEY (UtilizadorID));
CREATE TABLE Encomendas (
  EncomendaID              int IDENTITY NOT NULL, 
  Data                     datetime NULL, 
  FornecedoresFornecedorID int NOT NULL, 
  UtilizadoresUtilizadorID int NOT NULL, 
  Estado                   char(1) NULL, 
  PRIMARY KEY (EncomendaID));
ALTER TABLE Detalhes_Encomendas ADD CONSTRAINT FKDetalhes_E437525 FOREIGN KEY (EncomendasEncomendaID) REFERENCES Encomendas (EncomendaID);
ALTER TABLE Encomendas ADD CONSTRAINT FKEncomendas760258 FOREIGN KEY (FornecedoresFornecedorID) REFERENCES Fornecedores (FornecedorID);
ALTER TABLE Movimentos_Stock ADD CONSTRAINT FKMovimentos658057 FOREIGN KEY (ProdutosProdutoID) REFERENCES Produtos (ProdutoID);
ALTER TABLE Encomendas ADD CONSTRAINT FKEncomendas499338 FOREIGN KEY (UtilizadoresUtilizadorID) REFERENCES Utilizadores (UtilizadorID);
ALTER TABLE Movimentos_Stock ADD CONSTRAINT FKMovimentos81700 FOREIGN KEY (UtilizadoresUtilizadorID) REFERENCES Utilizadores (UtilizadorID);
ALTER TABLE Detalhes_Encomendas ADD CONSTRAINT FKDetalhes_E231047 FOREIGN KEY (ProdutosProdutoID) REFERENCES Produtos (ProdutoID);
