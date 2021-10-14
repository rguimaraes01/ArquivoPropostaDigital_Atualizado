namespace Projeto.Domain
{
    public class CoberturaConjuge
    {
        public string plano { get; set; }
        public string capitalSeguradoMorteAcidencial { get; set; } 
        public string valorPremioAcidentesPessoais { get; set; } 
        public string capitalSeguradoAssistenciaEmergencial { get; set; } 
        public string valorPremioAssistenciaEmergencial { get; set; } 
        public RendaMensalConjuge rendaMensal { get; set; } = new RendaMensalConjuge(new Erro());

    }
}