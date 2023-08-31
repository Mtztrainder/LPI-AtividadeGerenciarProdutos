namespace AtividadeGerenciarProdutos.ViewModel
{
    public class ProdutoViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal PrecoVenda { get; set; }
        public int QuantidadeEstoque { get; set; }

        public ProdutoViewModel() 
        {
            this.Id = Guid.NewGuid();
            this.Nome = "";
            this.PrecoVenda = 0;
            this.QuantidadeEstoque = 0;
        }

        public ProdutoViewModel(string Nome, decimal PrecoVenda, int QuantidadeEstoque)
        {
            this.Id = Guid.NewGuid();
            this.Nome = Nome;
            this.PrecoVenda = PrecoVenda;
            this.QuantidadeEstoque = QuantidadeEstoque;
        }

        public string GetNomeArquivo()
        {
            return Id + ".png"; 
        }
    }
}
