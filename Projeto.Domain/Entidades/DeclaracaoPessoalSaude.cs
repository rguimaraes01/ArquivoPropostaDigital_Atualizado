using System.Collections.Generic;

namespace Projeto.Domain
{
    public class DeclaracaoPessoalSaude
    {
        private Erro erro;
        public List<PerguntasResposta> perguntasRespostas { get; set; }
        private string _filhoBeneficiario;
        public string filhoBeneficiario
        {
            get => _filhoBeneficiario;
            set
            {
                erro.valida(value, "filhoBeneficiario");
                _filhoBeneficiario = value;
            }
        }
        private string _dependenteAgregado;
        public string dependenteAgregado
        {
            get => _dependenteAgregado;
            set
            {
                erro.valida(value, "dependenteAgregado");
                _dependenteAgregado = value;
            }
        }
        private string _conjugeProponente;
        public string conjugeProponente
        {
            get => _conjugeProponente;
            set
            {
                erro.valida(value, "conjugeProponente");
                _conjugeProponente = value;
            }
        }

        public DeclaracaoPessoalSaude(Erro erro)
        {
            this.erro = erro;
        }
    }
}