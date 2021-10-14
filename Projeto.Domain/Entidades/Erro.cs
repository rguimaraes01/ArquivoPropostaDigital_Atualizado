using System;
using System.Collections.Generic;

namespace Projeto.Domain
{
    public class Erro
    {
        public Boolean ocorreu { get; set; }
        public List<string> mensagens { get; set; }
        private int totalTelefones = 3;

        public Erro()
        {
            this.ocorreu = false;
            this.mensagens = new List<string>();
        }

        public void valida(string valor, string nomeCampo)
        {
            if (valor == null || valor == "")
            {
                ocorreu = true;
                mensagens.Add($"Deve existir o campo {nomeCampo}");
            }
        }

        public void valida(decimal? valor, string nomeCampo)
        {
            if (valor == null)
            {
                ocorreu = true;
                mensagens.Add($"Deve existir o campo {nomeCampo}");
            }
        }

        public void valida(int? valor, string nomeCampo)
        {
            if (valor == null)
            {
                ocorreu = true;
                mensagens.Add($"Deve existir o campo {nomeCampo}");
            }
        }


        public void valida(DateTime valor, string nomeCampo)
        {
            if (string.IsNullOrEmpty(valor.ToShortDateString()))
            {
                ocorreu = true;
                mensagens.Add($"Deve existir o campo {nomeCampo}");
            }
        }

        public void validaTelefone(string telefone)
        {
            if (telefone == null || telefone == "")
            {
                totalTelefones--;
                if (totalTelefones == 0)
                {
                    ocorreu = true;
                    mensagens.Add($"Deve existir pelo menos um telefone");
                }
            }
        }

    }
}