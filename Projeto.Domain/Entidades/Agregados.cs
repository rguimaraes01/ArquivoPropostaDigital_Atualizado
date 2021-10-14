namespace Projeto.Domain
{
    public class Agregados
    {
        private Erro erro;

        public Agregados(Erro erro)
        {
            this.erro = erro;
        }

        public string nomeSegurado { get; set; }
        public string dataNascimento { get; set; }
        public string sexo { get; set; }
        public string parentesco { get; set; }
        public string plano { get; set; }

        private decimal _valorPremio;
        public decimal valorPremio
        {
            get => _valorPremio;
            set
            {
                erro.valida(value, "valorPremio");
                _valorPremio = value;
            }
        }

        private string _cpf;
        public string cpf
        {
            get => _cpf;
            set
            {
                erro.valida(value, "cpf");
                _cpf = value;
            }
        }

        //private string _valorPremioFuneralMaster;
        //public string valorPremioFuneralMaster
        //{
        //    get => _valorPremioFuneralMaster;
        //    set
        //    {
        //        erro.valida(value, "valorPremioFuneralMaster");
        //        _valorPremioFuneralMaster = value;
        //    }
        //}

    }
}