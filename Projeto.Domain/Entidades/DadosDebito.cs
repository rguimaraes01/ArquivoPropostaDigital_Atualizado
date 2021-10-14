namespace Projeto.Domain
{
    public class DadosDebito
    {
        private Erro erro;
        public string titular { get; set; }
        public string cpfTitular { get; set; }
        public int? codigoBanco { get; set; }
        public string numeroAgencia { get; set; }
        public string numeroConta { get; set; }
        public string tipoConta { get; set; }

        public DadosDebito(Erro erro)
        {
            this.erro = erro;
        }
    }
}