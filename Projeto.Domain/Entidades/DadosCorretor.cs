namespace Projeto.Domain
{
    public class DadosCorretor
    {
        private Erro erro;
        private string _angariador;
        public string angariador
        {
            get => _angariador;
            set
            {
                erro.valida(value, "angariador");
                _angariador = value;
            }
        }
        private int _codigoAngariador;
        public int codigoAngariador
        {
            get => _codigoAngariador;
            set
            {
                erro.valida(value, "codigoAngariador");
                _codigoAngariador = value;
            }
        }
        private string _unidade;
        public string unidade
        {
            get => _unidade;
            set
            {
                erro.valida(value, "unidade");
                _unidade = value;
            }
        }
        public string coordenador { get; set; } = "";
        public string corretor { get; set; }
        public int registroSUSEP { get; set; }

        public DadosCorretor(Erro erro)
        {
            this.erro = erro;
        }
    }
}