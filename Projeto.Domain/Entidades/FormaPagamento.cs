namespace Projeto.Domain
{
    public class FormaPagamento
    {
        private Erro erro;
        private string _primeiraParcela;
        public string primeiraParcela
        {
            get => _primeiraParcela;
            set
            {
                erro.valida(value, "primeiraParcela");
                _primeiraParcela = value;
            }
        }
        private string _demaisParcelas;
        public string demaisParcelas
        {
            get => _demaisParcelas;
            set
            {
                erro.valida(value, "demaisParcelas");
                _demaisParcelas = value;
            }
        }
        public DadosDebito dadosDebito { get; set; } = new DadosDebito(new Erro());

        public FormaPagamento(Erro erro)
        {
            this.erro = erro;
        }
    }
}