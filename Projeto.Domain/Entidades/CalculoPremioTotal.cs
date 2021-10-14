namespace Projeto.Domain
{
    public class CalculoPremioTotal
    {
        private Erro erro;

        private string _vencimento;
        public string vencimento
        {
            get => _vencimento;
            set
            {
                erro.valida(value, "vencimento");
                _vencimento = value;
            }
        }

        private string _valorPremioAcidentesPessoais;
        public string valorPremioAcidentesPessoais
        {
            get => _valorPremioAcidentesPessoais;
            set
            {
                erro.valida(value, "valorPremioAcidentesPessoais");
                _valorPremioAcidentesPessoais = value;
            }
        }
        private string _valorPremioAgregados;
        public string valorPremioAgregados
        {
            get => _valorPremioAgregados;
            set
            {
                erro.valida(value, "valorPremioAgregados");
                _valorPremioAgregados = value;
            }
        }
        private string _valorPremioRendaMensal;
        public string valorPremioRendaMensal
        {
            get => _valorPremioRendaMensal;
            set
            {
                erro.valida(value, "valorPremioRendaMensal");
                _valorPremioRendaMensal = value;
            }
        }
        private string _valorPremioTotal;
        public string valorPremioTotal
        {
            get => _valorPremioTotal;
            set
            {
                erro.valida(value, "valorPremioTotal");
                _valorPremioTotal = value;
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

        //private string _valorPremioAssistenciaFuneral;
        //public string valorPremioAssistenciaFuneral
        //{
        //    get => _valorPremioAssistenciaFuneral;
        //    set
        //    {
        //        erro.valida(value, "valorPremioAssistenciaFuneral");
        //        _valorPremioAssistenciaFuneral = value;
        //    }
        //}
        public CalculoPremioTotal(Erro erro)
        {
            this.erro = erro;
        }
    }
}