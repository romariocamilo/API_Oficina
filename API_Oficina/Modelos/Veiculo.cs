using System.ComponentModel.DataAnnotations;

namespace API_Oficina.Modelos
{
    public class Veiculo
    {
        public int IdVeiculo { get; private set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Tipo obrigatório")]
        public string Tipo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Modelo obrigatório")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "Nome obrigatório")]
        public int IdCliente { get; set; }

        public void PreencheId(int id)
        {
            IdVeiculo = id;
        }
    }
}
