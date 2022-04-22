using API_Oficina.Modelos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_Oficina.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VeiculoController : Controller
    {
        private readonly ILogger<ClienteController> _logger;
        private string _caminhoArquivoVeiculo;
        private string _caminhoArquivoCliente;
        private bool diretorioBancoDeDados;
        private bool arquivoBancoDeDadosCliente;

        public VeiculoController(ILogger<ClienteController> logger)
        {
            _logger = logger;

            _caminhoArquivoVeiculo = Directory.GetCurrentDirectory() + "\\BancosDeDados\\Veiculo.json";
            _caminhoArquivoCliente = Directory.GetCurrentDirectory() + "\\BancosDeDados\\Cliente.json";
            
            diretorioBancoDeDados = Directory.Exists(Directory.GetCurrentDirectory() + "\\BancosDeDados");
            arquivoBancoDeDadosCliente = System.IO.File.Exists(_caminhoArquivoVeiculo);

            if (!diretorioBancoDeDados)
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\BancosDeDados");
            }

            if (!arquivoBancoDeDadosCliente)
            {
                System.IO.File.Create(_caminhoArquivoVeiculo).Close();
            }
        }

        /// <summary>
        /// Lista todos os veiculos do banco de dados
        /// </summary>
        /// <returns>Veiculos retornados</returns>
        /// <response code="200">Retorna todos os veiculos cadastrados no banco de dados</response>
        /// <response code="500">Se uma exceção não tratada estourar</response>
        [HttpGet(Name = "RetornaVeiculos")]
        public IActionResult RetornaClientes()
        {
            List<Veiculo> listaClientes = RetornaListaDeVeiculosDoBancoDeDados();

            return Ok(listaClientes);
        }

        /// <summary>
        /// Lista um determinado veiculo pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Veiculo retornado</returns>
        /// <response code="200">Retorna um determinado veiculo cadastrado no banco de dados</response>
        /// <response code="400">Se o veiculo não estiver cadastrado no banco de dados</response>
        /// <response code="500">Se uma exceção não tratada estourar</response>
        [HttpGet("/Veiculo/{id}")]
        public IActionResult RetornaUmVeiculo(int id)
        {
            Veiculo veiculo = RetornaListaDeVeiculosDoBancoDeDados().FirstOrDefault(c => c.IdVeiculo == id);

            if (veiculo != null)
            {
                return Ok(veiculo);
            }
            else
            {
                return BadRequest("Veiculo não encontrado");
            }
        }

        /// <summary>
        /// Cadastra um novo veiculo
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     {
        ///         "tipo": "string",
        ///         "modelo": "string",
        ///         "idCliente": 0
        ///     }
        /// </remarks>
        /// <param name="novoVeiculo"></param>
        /// <returns>Novo veiculo cadastrado</returns>
        /// <response code="200">Retorna o novo veiculo cadastrado</response>
        /// <response code="400">Se o veiculo não for criado</response>
        /// <response code="500">Se uma exceção não tratada estourar</response>
        [HttpPost]
        public IActionResult CadastraVeiculo(Veiculo novoVeiculo)
        {
            int idVeiculo = 1;
            bool clienteExiste = false;

            List<Veiculo> listaVeiculos = RetornaListaDeVeiculosDoBancoDeDados();
            List<Cliente> listaClientes = RetornaListaDeClientesDoBancoDeDados();

            if (listaVeiculos.Count > 0)
            {
                if (listaClientes.FirstOrDefault(c => c.IdCliente == novoVeiculo.IdCliente) != null) clienteExiste = true;
                idVeiculo = listaVeiculos.LastOrDefault().IdCliente + 1;
            }

            if (!clienteExiste)
            {
                novoVeiculo.PreencheId(idVeiculo);
                listaVeiculos.Add(novoVeiculo);

                string jsonString = JsonConvert.SerializeObject(listaVeiculos, Formatting.Indented);
                System.IO.File.WriteAllText(_caminhoArquivoVeiculo, jsonString);

                return Ok(novoVeiculo);
            }
            else
            {
                return BadRequest("ID de cliente não existe no banco de dados.");
            }
        }






        private List<Veiculo> RetornaListaDeVeiculosDoBancoDeDados()
        {
            var veiculosDoBancoDeDados = System.IO.File.ReadAllText(_caminhoArquivoVeiculo);

            List<Veiculo> listaVeiculos = JsonConvert.DeserializeObject<List<Veiculo>>(veiculosDoBancoDeDados);
            List<VeiculoEspelho> listaVeiculosEspelho = JsonConvert.DeserializeObject<List<VeiculoEspelho>>(veiculosDoBancoDeDados);

            if (listaVeiculos == null)
            {
                listaVeiculos = new List<Veiculo>();
                listaVeiculosEspelho = new List<VeiculoEspelho>();
            }

            for (int contador = 0; contador < listaVeiculos.Count; contador++)
            {
                listaVeiculos[contador].PreencheId(listaVeiculosEspelho[contador].IdVeiculo);
            }

            return listaVeiculos;
        }

        private List<Cliente> RetornaListaDeClientesDoBancoDeDados()
        {
            var clientesDoBancoDeDados = System.IO.File.ReadAllText(_caminhoArquivoCliente);

            List<Cliente> listaClientes = JsonConvert.DeserializeObject<List<Cliente>>(clientesDoBancoDeDados);
            List<ClienteEspelho> listaClientesEspelho = JsonConvert.DeserializeObject<List<ClienteEspelho>>(clientesDoBancoDeDados);

            if (listaClientes == null)
            {
                listaClientes = new List<Cliente>();
                listaClientesEspelho = new List<ClienteEspelho>();
            }

            for (int contador = 0; contador < listaClientes.Count; contador++)
            {
                listaClientes[contador].PreencheId(listaClientesEspelho[contador].Id);
                listaClientes[contador].CalculaIdade(listaClientesEspelho[contador].DataDeNascimento);
            }

            return listaClientes;
        }
    }
}
