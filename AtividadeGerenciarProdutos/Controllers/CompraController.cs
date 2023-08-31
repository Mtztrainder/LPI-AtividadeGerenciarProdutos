using AtividadeGerenciarProdutos.DAO;
using AtividadeGerenciarProdutos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace AtividadeGerenciarProdutos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : CustomControllerBase
    {
        private ProdutoDAO _dao = new ProdutoDAO();
        private List<ProdutoViewModel> _dados;

        [HttpPut]
        [Route("[action]")]
        public IActionResult AtualizaEstoque(Guid id, int QuantidadeSolicitada)
        {
            _dados = _dao.CarregaListaProduto();
            int posicaoProdutoLista = _dados.FindIndex(x => x.Id == id);

            if (posicaoProdutoLista == -1)
            {
                AddNotFoundMessage("Produto não encontrado!");
                return CustomResponse(HttpStatusCode.NotFound, false);
            }
            
            if (_dados[posicaoProdutoLista].QuantidadeEstoque - QuantidadeSolicitada < 0)
            {
                AddErrorMessage("Produto não possui estoque suficiente!");
                return CustomResponse(HttpStatusCode.BadRequest, false, _dados[posicaoProdutoLista]);
            }


            _dados[posicaoProdutoLista].QuantidadeEstoque -= QuantidadeSolicitada;

            if (_dao.GravaListaProduto(_dados))
            {
                AddSuccessMessage("Produto atualizado com sucesso!");
                return CustomResponse(HttpStatusCode.OK, true, _dados[posicaoProdutoLista]);
            }
            else
            {
                AddErrorMessage("Ocorreu um erro ao alterar o produto!");
                return CustomResponse(HttpStatusCode.InternalServerError, false, _dados[posicaoProdutoLista]);
            }
        }
    }
}
