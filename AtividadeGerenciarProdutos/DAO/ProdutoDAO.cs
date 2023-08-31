using AtividadeGerenciarProdutos.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AtividadeGerenciarProdutos.DAO
{
    public class ProdutoDAO
    {
        private string _diretorioDados;
        private string _diretorioUpload;
        private string _arquivo;
        private JsonSerializerOptions _options;

        public ProdutoDAO()
        {
            _diretorioDados = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dados");
            _diretorioUpload = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads");
            _arquivo = Path.Combine(_diretorioDados, "CadProduto.json");
            _options = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public string GetDiretorioDados()
        {
            return _diretorioDados;
        }

        public string GetDiretorioUpload() {
            return _diretorioUpload;
        }


        public List<ProdutoViewModel> CarregaListaProduto()
        {
            Directory.CreateDirectory(_diretorioDados);

            List<ProdutoViewModel> dados = new();
            if (File.Exists(_arquivo))
            {
                var json = File.ReadAllText(_arquivo);
                dados = JsonSerializer.Deserialize<List<ProdutoViewModel>>(json, _options);
            }

            return dados;
        }
        public bool GravaListaProduto(List<ProdutoViewModel> dados)
        {
            Directory.CreateDirectory(_diretorioDados);

            try
            {
                string json = JsonSerializer.Serialize(dados, _options);
                File.WriteAllText(_arquivo, json);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool SalvarFotoProduto(ProdutoViewModel produto, byte[] conteudoArquivo)
        {
            Directory.CreateDirectory(_diretorioUpload);
            string nomeArquivoNovo = produto.GetNomeArquivo();
            string arquivoLocalDestino = Path.Combine(_diretorioUpload, nomeArquivoNovo);

            File.WriteAllBytes(arquivoLocalDestino, conteudoArquivo);
            return File.Exists(arquivoLocalDestino);
        }

        public bool RemoveFotoProduto(ProdutoViewModel produto)
        {
            Directory.CreateDirectory(_diretorioUpload);

            string fotoProdutoLocal = Path.Combine(_diretorioUpload, produto.GetNomeArquivo());
            //arquivo não existe?
            if (!File.Exists(fotoProdutoLocal))
                return false;

            try
            {
                //tentativa de remover o arquivo
                File.Delete(fotoProdutoLocal);
                //se o arquivo não existe mais, a remoção foi um sucesso
                return !File.Exists(fotoProdutoLocal);
            }
            catch (IOException ioExp)
            {
                //ocorreu algum erro na remoção do arquivo
                Console.WriteLine(ioExp.Message);
                return false;
            }
        }

        public byte[]? BaixarFotoProduto(ProdutoViewModel produto)
        {
            Directory.CreateDirectory(_diretorioUpload);

            byte[]? conteudoArquivo = null;
            string nomeFoto = produto.GetNomeArquivo();
            string fotoProdutoLocal = Path.Combine(_diretorioUpload, nomeFoto);

            if (File.Exists(fotoProdutoLocal))
                conteudoArquivo = File.ReadAllBytes(fotoProdutoLocal);

            return conteudoArquivo;
        } 
    }
}
