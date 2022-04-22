using System.ComponentModel.DataAnnotations;

namespace API_Oficina.Modelos
{
    public class Cliente
    {
        public int IdCliente { get; private set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nome obrigatório")]
        public string Nome { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Sobrenome obrigatório")]
        public string Sobrenome { get; set; }

        public int Idade { get; private set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "DataDeNascimento obrigatório")]
        public DataBr DataDeNascimento { get; set; }

        public string? Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Telefone obrigatório")]
        public string Telefone { get; set; }

        public void PreencheId(int id)
        {
            IdCliente = id;
        }

        public void CalculaIdade(DataBr dataDeNascimento)
        {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataDeAniversario = Convert.ToDateTime($"{dataDeNascimento.Dia}/{dataDeNascimento.Mes}/{DateTime.Now.Year}");
            DateTime dataDeNascimentoFormatada = Convert.ToDateTime($"{dataDeNascimento.Dia}/{dataDeNascimento.Mes}/{dataDeNascimento.Ano}");


            if (dataAtual < dataDeAniversario)
            {
                Idade = dataDeAniversario.Year - dataDeNascimentoFormatada.Year - 1;
            }
            else
            {
                Idade = dataDeAniversario.Year - dataDeNascimentoFormatada.Year;
            }
        }
    }
}