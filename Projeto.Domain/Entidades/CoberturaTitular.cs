namespace Projeto.Domain
{
    public class CoberturaTitular
    {
        public string capitalSeguradoMorteAcidencial { get; set; } 
        public string valorPremioAcidentesPessoais { get; set; } 
        public string capitalSeguradoAssistenciaEmergencial { get; set; } 
        public string valorPremioAssistenciaEmergencial { get; set; }
        public RendaMensal rendaMensal { get; set; }
    }
}