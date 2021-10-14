namespace Projeto.Domain
{
    public class PerguntaRespostaDPS
    {
        private Erro erro;
        public int codigoPergunta { get; set; }
        public string resposta
        {
            get;
            set;
        }
        public string justificativa { get; set; }

        public PerguntaRespostaDPS(Erro erro)
        {
            this.erro = erro;
        }
    }
}