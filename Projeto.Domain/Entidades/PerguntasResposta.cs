using System.Collections.Generic;

namespace Projeto.Domain
{
    public class PerguntasResposta
    {
        private Erro erro;
        public string tipoSegurado { get; set; }
        public List<PerguntaRespostaDPS> perguntaRespostaDPS { get; set; }

        public PerguntasResposta(Erro erro)
        {
            this.erro = erro;
        }
    }
}