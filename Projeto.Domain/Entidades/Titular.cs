using System.Collections.Generic;

namespace Projeto.Domain
{
    public class Titular
    {
        private Erro erro;
        private string _nomeSegurado;
        public string nomeSegurado
        {
            get => _nomeSegurado;
            set
            {
                erro.valida(value, "nomeSegurado");
                this._nomeSegurado = value;
            }
        }

        private string _sexo;
        public string sexo
        {
            get => _sexo;
            set
            {
                erro.valida(value, "sexo");
                this._sexo = value;
            }
        }

        private string _cpf;
        public string cpf
        {
            get => _cpf;
            set
            {
                erro.valida(value, "cpf");
                this._cpf = value;
            }
        }

        private string _rg;
        public string rg
        {
            get => _rg;
            set
            {
                this._rg = value;
            }
        }

        private string _orgaoExpedidor;
        public string orgaoExpedidor
        {
            get => _orgaoExpedidor;
            set
            {
                this._orgaoExpedidor = value;
            }
        }

        private string _dataExpedicao;
        public string dataExpedicao
        {
            get => _dataExpedicao;
            set
            {
                this._dataExpedicao = value;
            }
        }

        private string _dataNascimento { get; set; }
        public string dataNascimento
        {
            get => _dataNascimento;
            set
            {
                erro.valida(value, "dataNascimento");
                this._dataNascimento = value;
            }
        }

        private string _estadoCivil;
        public string estadoCivil
        {
            get => _estadoCivil;
            set
            {
                erro.valida(value, "estadoCivil");
                this._estadoCivil = value;
            }
        }

        private string _rendaFamiliar;
        public string rendaFamiliar
        {
            get => _rendaFamiliar;
            set
            {
                this._rendaFamiliar = value;
            }
        }

        private string _cep;
        public string cep
        {
            get => _cep;
            set
            {
                erro.valida(value, "cep ");
                this._cep = value;
            }
        }

        private string _endereco;
        public string endereco
        {
            get => _endereco;
            set
            {
                erro.valida(value, "endereco");
                this._endereco = value;
            }
        }

        private int _numero;
        public int numero
        {
            get => _numero;
            set
            {
                this._numero = value;
            }
        }

        private string _complemento;
        public string complemento
        {
            get => _complemento;
            set
            {
                this._complemento = value;
            }
        }

        private string _bairro;
        public string bairro
        {
            get => _bairro;
            set
            {
                this._bairro = value;
            }
        }

        private string _cidade;
        public string cidade
        {
            get => _cidade;
            set
            {
                erro.valida(value, "cidade");
                this._cidade = value;
            }
        }

        private string _uf;
        public string uf
        {
            get => _uf;
            set
            {
                erro.valida(value, "uf");
                this._uf = value;
            }
        }

        private string _referencia;
        public string referencia
        {
            get => _referencia;
            set
            {
                this._referencia = value;
            }
        }

        private string _celular;
        public string celular
        {
            get => _celular;
            set
            {
                erro.valida(value, "celular");
                this._celular = value;
            }
        }

        private string _whatsApp;
        public string whatsApp
        {
            get => _whatsApp;
            set
            {
                erro.validaTelefone(value);
                this._whatsApp = value;
            }
        }

        private string _residencial;
        public string residencial
        {
            get => _residencial;
            set
            {
                erro.validaTelefone(value);
                this._residencial = value;
            }
        }

        private string _comercial;
        public string comercial
        {
            get => _comercial;
            set
            {
                erro.validaTelefone(value);
                this._comercial = value;
            }
        }

        private string _profissao;
        public string profissao
        {
            get => _profissao;
            set
            {
                erro.valida(value, "profissao");
                this._profissao = value;
            }
        }

        private string _codigoProfissao;
        public string codigoProfissao
        {
            get => _codigoProfissao;
            set
            {
                erro.valida(value, "codigoProfissao");
                this._codigoProfissao = value;
            }
        }

        private string _email;
        public string email
        {
            get => _email;
            set
            {
                this._email = value;
            }
        }

        //public Conjuge conjuge { get; set; } = new Conjuge();

        private string _assistenciaFuneral;
        public string assistenciaFuneral
        {
            get => _assistenciaFuneral;
            set
            {
                erro.valida(value, "assistenciaFuneral");
                this._assistenciaFuneral = value;
            }
        }

        private string _plano;
        public string plano
        {
            get => _plano;
            set
            {
                this._plano = value;
            }
        }
        public CoberturaTitular coberturas { get; set; }

        public Titular(Erro erro)
        {
            this.erro = erro;
        }
    }
}