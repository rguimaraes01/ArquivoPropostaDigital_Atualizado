namespace Projeto.Domain
{
    public class Conjuge
    {
        public string nomeSegurado { get; set; }
        public string sexo { get; set; }
        public string cpf { get; set; }
        public string dataNascimento { get; set; }
        public string profissao { get; set; }
        public string codigoProfissao { get; set; }
        public CoberturaConjuge coberturas { get; set; } = new CoberturaConjuge();

    }
}