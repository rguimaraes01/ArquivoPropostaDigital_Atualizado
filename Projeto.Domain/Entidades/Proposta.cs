using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto.Domain
{
    public class Proposta
    {

        public Proposta(Erro erro)
        {
            this.erro = erro;
        }

        private Erro erro;
        private string _externalKey;
        public string externalKey
        {
            get => _externalKey;
            set
            {
                erro.valida(value, "externalKey");
                this._externalKey = value;
            }
        }
        private string _nomeProduto;

        public string nomeProduto
        {
            get => _nomeProduto;
            set
            {
                erro.valida(value, "nomeProduto");
                _nomeProduto = value;
            }
        }
        private int _numeroContrato;
        public int numeroContrato
        {
            get => _numeroContrato;
            set
            {
                erro.valida(value, "numeroContrato");
                _numeroContrato = value;
            }
        }
        private string _numeroApolice;
        public string numeroApolice
        {
            get => _numeroApolice;
            set
            {
                erro.valida(value, "numeroApolice");
                _numeroApolice = value;
            }
        }
        private string _processoSusep;

        public string processoSusep
        {
            get => _processoSusep;
            set
            {
                erro.valida(value, "processoSusep");
                _processoSusep = value;
            }
        }

        public string statusProposta
        {
            get;
            set;
        }

        private string _nome;
        public string nome
        {
            get => _nome;
            set
            {
                erro.valida(value, "nome");
                this._nome = value;
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

        private int _proposta;
        public int proposta
        {
            get => _proposta;
            set
            {
                erro.valida(value, "proposta");
                _proposta = value;
            }
        }

        private string _tipoComunicacao;
        public string tipoComunicacao
        {
            get => _tipoComunicacao;
            set
            {
                erro.valida(value, "tipoComunicacao");
                _tipoComunicacao = value;
            }
        }
        public Titular titular { get; set; }
        public Conjuge conjuge { get; set; } 
        public List<Filhos> filhos { get; set; } = new List<Filhos>();

        public List<Agregados> agregados { get; set; }
        public CalculoPremioTotal calculoPremioTotal { get; set; }
        public DadosCorretor dadosCorretor { get; set; }
        public DeclaracaoPessoalSaude declaracaoPessoalSaude { get; set; }
        public List<Beneficiario> beneficiarios { get; set; } = new List<Beneficiario>();
        //private string _valorPermioAgregados;
        //public string valorPremioAgregados
        //{
        //    get => _valorPermioAgregados;
        //    set
        //    {
        //        erro.valida(value, "valorPremioAgregados");
        //        _valorPermioAgregados = value;
        //    }
        //}
        private string _origemVenda;
        public string origemVenda
        {
            get => _origemVenda;
            set
            {
                //erro.valida(value, "origemVenda");
                _origemVenda = value;
            }
        }
        public FormaPagamento formaPagamento { get; set; }
        private DateTime _data;
        public string data
        {
            get => _data.ToShortDateString();
            set
            {
                erro.valida(value, "data");
                _data = DateTime.Parse(value);
            }
        }
        public Final final { get; set; }

    }
}