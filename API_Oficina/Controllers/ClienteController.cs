using API_Oficina.Modelos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_Oficina.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;
        private string _caminhoArquivo;
        private bool diretorioBancoDeDados;
        private bool arquivoBancoDeDadosCliente;

        public ClienteController(ILogger<ClienteController> logger)
        {
            _logger = logger;

            _caminhoArquivo = Directory.GetCurrentDirectory() + "\\BancosDeDados\\Cliente.json";
            diretorioBancoDeDados = Directory.Exists(Directory.GetCurrentDirectory() + "\\BancosDeDados");
            arquivoBancoDeDadosCliente = System.IO.File.Exists(_caminhoArquivo);

            if (!diretorioBancoDeDados)
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\BancosDeDados");
            }

            if (!arquivoBancoDeDadosCliente)
            {
                System.IO.File.Create(_caminhoArquivo).Close();
            }
        }

        /// <summary>
        /// Lista todos os clientes do banco de dados
        /// </summary>
        /// <returns>Clientes retornados</returns>
        /// <response code="200">Retorna todos os clientes cadastrados no banco de dados</response>
        /// <response code="500">Se uma exceção não tratada estourar</response>
        [HttpGet(Name = "RetornaClientes")]
        public IActionResult RetornaClientes()
        {
            List<Cliente> listaClientes = RetornaListaDeClientesDoBancoDeDados();

            return Ok(listaClientes);
        }

        /// <summary>
        /// Lista um determinado clinte pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Cliente retornado</returns>
        /// <response code="200">Retorna um determinado cliente cadastrado no banco de dados</response>
        /// <response code="400">Se o cliente não estiver cadastrado no banco de dados</response>
        /// <response code="500">Se uma exceção não tratada estourar</response>
        [HttpGet("/Cliente/{id}")]
        public IActionResult RetornaUmCliente(int id)
        {
            Cliente cliente = RetornaListaDeClientesDoBancoDeDados().FirstOrDefault(c => c.IdCliente == id);

            if (cliente != null)
            {
                return Ok(cliente);
            }
            else
            {
                return BadRequest("Cliente não encontrado");
            }
        }

        /// <summary>
        /// Cadastra um novo cliente
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     {
        ///        "nome": "Romário",
        ///        "sobrenome": "Camilo",
        ///        "dataDeNascimento": {
        ///           "dia": 3,
        ///           "mes": 5,
        ///           "ano": 1994
        ///         },
        ///        "email": "teste@teste.com",
        ///        "telefone": "34996791318"
        ///     }
        /// </remarks>
        /// <param name="novoCliente"></param>
        /// <returns>Novo cliente cadastrado</returns>
        /// <response code="200">Retorna o novo cliente cadastrado</response>
        /// <response code="400">Se o cliente não for criado</response>
        /// <response code="500">Se uma exceção não tratada estourar</response>
        [HttpPost]
        public IActionResult CadastraCliente(Cliente novoCliente)
        {
            int idCliente = 1;
            bool clienteJaCadastrado = false;

            List<Cliente> listaClientes = RetornaListaDeClientesDoBancoDeDados();

            if (listaClientes.Count > 0)
            {
                if (listaClientes.FirstOrDefault(c => c.Nome == novoCliente.Nome) != null) clienteJaCadastrado = true;
                idCliente = listaClientes.LastOrDefault().IdCliente + 1;
            }

            if (!clienteJaCadastrado)
            {
                novoCliente.CalculaIdade(novoCliente.DataDeNascimento);
                novoCliente.PreencheId(idCliente);
                listaClientes.Add(novoCliente);

                string jsonString = JsonConvert.SerializeObject(listaClientes, Formatting.Indented);
                System.IO.File.WriteAllText(_caminhoArquivo, jsonString);

                return Ok(novoCliente);
            }
            else
            {
                return BadRequest("Cliente já cadastrado com esse nome");
            }
        }

        /// <summary>
        /// Atualiza um cliente
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     PUT
        ///     {
        ///        "nome": "Romário",
        ///        "sobrenome": "Camilo",
        ///        "dataDeNascimento": {
        ///           "dia": 3,
        ///           "mes": 5,
        ///           "ano": 1994
        ///         },
        ///        "email": "teste@teste.com",
        ///        "telefone": "34996791318"
        ///     }
        /// </remarks>
        /// <param name="clienteAtualizado"></param>
        /// <param name="id"></param>
        /// <returns>Cliente atualizado</returns>
        /// <response code="200">Retorna o cliente atualizado</response>
        /// <response code="400">Se o cliente não for atualizado</response>
        /// <response code="500">Se uma exceção não tratada estourar</response>
        [HttpPut("/Cliente/{id}")]
        public IActionResult AtualizaCliente(Cliente clienteAtualizado, int id)
        {
            bool clienteExiste = false;
            List<Cliente> listaClientes = RetornaListaDeClientesDoBancoDeDados();

            for (int contador = 0; contador < listaClientes.Count; contador++)
            {
                if (listaClientes[contador].IdCliente == id)
                {
                    clienteAtualizado.PreencheId(id);
                    clienteAtualizado.CalculaIdade(clienteAtualizado.DataDeNascimento);

                    listaClientes[contador] = clienteAtualizado;
                    clienteExiste = true;
                }
            }

            string jsonString = JsonConvert.SerializeObject(listaClientes, Formatting.Indented);
            System.IO.File.WriteAllText(_caminhoArquivo, jsonString);

            if (clienteExiste)
            {
                return Ok(clienteAtualizado);
            }
            else
            {
                return BadRequest("Cliente não cadastrado no banco de dados");
            }
        }

        /// <summary>
        /// Deleta um cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Deleta cliente com sucesso</response>
        /// <response code="400">Se o cliente não existir no banco de dados</response>
        /// <response code="500">Se uma exceção não tratada estourar</response>
        [HttpDelete("/Cliente/{id}")]
        public IActionResult DeletaCliente(int id)
        {
            bool clienteRemovido = false;
            List<Cliente> listaClientes = RetornaListaDeClientesDoBancoDeDados();

            for (int contador = 0; contador < listaClientes.Count; contador++)
            {
                if (listaClientes[contador].IdCliente == id)
                {
                    listaClientes.Remove(listaClientes[contador]);
                    clienteRemovido = true;
                }
            }

            string jsonString = JsonConvert.SerializeObject(listaClientes, Formatting.Indented);
            System.IO.File.WriteAllText(_caminhoArquivo, jsonString);

            if (clienteRemovido)
            {
                return Ok($"Cliente com id {id} foi removido com sucesso");
            }
            else
            {
                return BadRequest("Cliente não cadastrado no banco de dados");
            }
        }

        /// <summary>
        /// Deleta todos os clientes do banco de dados
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Deleta todos os clientes com sucesso</response>
        /// <response code="400">Se o banco de dados de clientes já estiver vazio</response>
        /// <response code="500">Se uma exceção não tratada estourar</response>
        [HttpDelete("/Cliente/todos")]
        public IActionResult DeletaTodosClientes()
        {
            List<Cliente> listaClientes = RetornaListaDeClientesDoBancoDeDados();

            if (listaClientes.Count > 0)
            {
                for (int contador = 0; contador < listaClientes.Count; contador++)
                {
                    listaClientes.Remove(listaClientes[contador]);
                }

                string jsonString = JsonConvert.SerializeObject(listaClientes, Formatting.Indented);
                System.IO.File.WriteAllText(_caminhoArquivo, jsonString);

                return Ok("Clientes removidos com sucesso");
            }
            else
            {
                return BadRequest("Banco de dados de clientes já está vazio");
            }
        }



        #region Métodos auxiliares

        private List<Cliente> RetornaListaDeClientesDoBancoDeDados()
        {
            var clientesDoBancoDeDados = System.IO.File.ReadAllText(_caminhoArquivo);

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

        #endregion
    }
}
