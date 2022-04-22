namespace API_Oficina.Modelos
{
    public class DataBr
    {
        public int Dia { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }

        public string RetornaDataFormatoBr()
        {
            return $"{Dia}/{Mes}/{Ano}";
        }
    }
}
