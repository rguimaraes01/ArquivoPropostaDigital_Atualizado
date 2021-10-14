namespace Projeto.Domain
{
    public class Beneficiario
    {
        private Erro erro;

        public Beneficiario(Erro erro)
        {
            this.erro = erro;
        }
        public string nomeBeneficiário { get; set; } = "";
        public string parentesco { get; set; } = "";
        public string CPF { get; set; } = "";
        public string Percentual { get; set; } = "";
    }
}