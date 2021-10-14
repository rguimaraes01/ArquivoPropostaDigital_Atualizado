namespace Projeto.Domain
{
    public class RendaMensalConjuge
    {
        private Erro erro;

        public RendaMensalConjuge(Erro erro)
        {
            this.erro = erro;
        }

        private int _prazo;
        public int prazo
        {
            get => _prazo;
            set
            {
                erro.valida(value, "prazo");
                _prazo = value;
            }
        }

        private string _valorParcela;
        public string valorParcela
        {
            get => _valorParcela;
            set
            {
                erro.valida(value, "valorParcela");
                _valorParcela = value;
            }
        }

        private string _capitalSegurado;
        public string capitalSegurado
        {
            get => _capitalSegurado;
            set
            {
                erro.valida(value, "capitalSegurado");
                _capitalSegurado = value;
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
    }
}