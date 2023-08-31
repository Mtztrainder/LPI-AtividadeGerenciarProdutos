using AtividadeGerenciarProdutos.DAO;
using AtividadeGerenciarProdutos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;

namespace AtividadeGerenciarProdutos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GerenciaProdutoController : CustomControllerBase
    {
        private ProdutoDAO _dao = new ProdutoDAO();
        private List<ProdutoViewModel> _dados;

        [HttpPost]
        [Route("[action]")]
        public IActionResult NovoProduto(ProdutoViewModel produto)
        {
            _dados = _dao.CarregaListaProduto();

            produto.Id = Guid.NewGuid();
            _dados.Add(produto);


            if (_dao.GravaListaProduto(_dados))
            {
                AddSuccessMessage("Produto criado com sucesso!");
                return CustomResponse(HttpStatusCode.Created, true, produto);
            }
            else
            {
                AddErrorMessage("Ocorreu um erro ao criar o produto!");
                return CustomResponse(HttpStatusCode.InternalServerError, false, produto);
            }
        }

        [HttpPut]
        [Route("[action]")]
        public IActionResult AlteraProduto(Guid id, ProdutoViewModel produto)
        {
            _dados = _dao.CarregaListaProduto();

            int posicaoProdutoLista = _dados.FindIndex(x => x.Id == id);
            produto.Id = id;

            if (posicaoProdutoLista == -1)
            {
                AddNotFoundMessage("Produto não encontrado!");
                return CustomResponse(HttpStatusCode.NotFound, false, produto);
            }
            else
            {
                _dados[posicaoProdutoLista] = produto;

                if (_dao.GravaListaProduto(_dados))
                {
                    AddSuccessMessage("Produto alterado com sucesso!");
                    return CustomResponse(HttpStatusCode.OK, true, produto);
                }
                else
                {
                    AddErrorMessage("Ocorreu um erro ao alterar o produto!");
                    return CustomResponse(HttpStatusCode.InternalServerError, false, produto);
                }
            }
        }

        [HttpDelete]
        [Route("[action]")]
        public IActionResult RemoveProduto(Guid id)
        {
            _dados = _dao.CarregaListaProduto();
            int posicaoProdutoLista = _dados.FindIndex(x => x.Id == id);

            if (posicaoProdutoLista == -1)
            {
                AddNotFoundMessage("Produto não encontrado!");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }
            else
            {
                _dao.RemoveFotoProduto(_dados[posicaoProdutoLista]);
                _dados.RemoveAt(posicaoProdutoLista);

                if (_dao.GravaListaProduto(_dados))
                {
                    AddSuccessMessage("Produto removido com sucesso!");
                    return CustomResponse(HttpStatusCode.OK, true);
                }
                else
                {
                    AddErrorMessage("Ocorreu um erro ao remover o produto!");
                    return CustomResponse(HttpStatusCode.InternalServerError, false);
                }
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SalvarFoto(Guid id)
        {
            if (Request.Form.Files.Count() == 0)
            {
                AddNotFoundMessage("Foto não fornecida!");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }


            _dados = _dao.CarregaListaProduto();
            var produtoLista = _dados.Find(x => x.Id == id);

            if (produtoLista == null)
            {
                AddNotFoundMessage("Produto não encontrado!");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }


            using (var memoryStream = new MemoryStream())
            {
                var arquivoRecebido = Request.Form.Files[0];
                arquivoRecebido.CopyTo(memoryStream);

                string nomeArquivoRecebido = arquivoRecebido.FileName;
                string tipoArquivoRecebido = arquivoRecebido.ContentType;

                if (!tipoArquivoRecebido.Equals("image/png"))
                {
                    AddNotAcceptableMessage("Foto de formato inválido!");
                    return CustomResponse(HttpStatusCode.NotAcceptable, false);
                }

                if (arquivoRecebido.Length / 1024 / 1024 >= 1)
                {
                    AddNotAcceptableMessage("Foto não pode ser maior ou igual a 1MB!");
                    return CustomResponse(HttpStatusCode.NotAcceptable, false);
                }

                var conteudoArquivoRecebido = memoryStream.ToArray();
                _dao.SalvarFotoProduto(produtoLista, conteudoArquivoRecebido);

                AddSuccessMessage("Foto gravada com sucesso!");
                return CustomResponse(HttpStatusCode.Created, true, produtoLista.GetNomeArquivo());
            }
        
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult BaixaFoto(Guid id)
        {
            _dados = _dao.CarregaListaProduto();
            var produtoLista = _dados.Find(x => x.Id == id);

            if (produtoLista == null)
            {
                AddNotFoundMessage("Produto não encontrado!");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }

            var conteudoFoto = _dao.BaixarFotoProduto(produtoLista);

            if (conteudoFoto == null)
            {
                AddNotFoundMessage("Foto não encontrada!");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }
            
            return File(conteudoFoto, "image/png", produtoLista.GetNomeArquivo());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DetalheProduto(Guid id)
        {
            _dados = _dao.CarregaListaProduto();
            var produtoLista = _dados.Find(x => x.Id == id);

            if (produtoLista != null)
            {
                AddSuccessMessage("Produto encontrado com sucesso!");
                return CustomResponse(HttpStatusCode.OK, true, produtoLista);
            }
            else
            {
                AddNotFoundMessage("Produto não encontrado!");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult ListaProdutosDisponiveis()
        {
            _dados = _dao.CarregaListaProduto();
            List<ProdutoViewModel> produtosDisponiveis = _dados.Where(s => s.QuantidadeEstoque > 0).ToList();

            if (produtosDisponiveis.Count > 0)
            {
                AddSuccessMessage("Há produtos disponíveis.");
                return CustomResponse(HttpStatusCode.OK, true, produtosDisponiveis);
            }
            else
            {
                AddNotFoundMessage("Nenhum produto disponível encontrado.");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult BuscaProdutoNome(string nome)
        {
            _dados = _dao.CarregaListaProduto();
            var produtoLista = _dados.Find(x => x.Nome.Contains(nome, StringComparison.CurrentCultureIgnoreCase));

            if (produtoLista != null)
            {
                AddSuccessMessage("Produtos encontrados!");
                return CustomResponse(HttpStatusCode.OK, true, produtoLista);
            }
            else
            {
                AddNotFoundMessage("Nenhum produto encontrado.");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ListaProdutos()
        {
            _dados = _dao.CarregaListaProduto();
            if (_dados.Count > 0)
            {
                AddSuccessMessage("Produtos encontrados!");
                return CustomResponse(HttpStatusCode.OK, true, _dados);
            }
            else
            {
                AddNotFoundMessage("Nenhum produto cadastrado.");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }
        }
    }
}
