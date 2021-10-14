using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto.Infra
{
    public class OrigemVenda
    {
        public int Sequencial { get; set; }
        public int CodigoCorretora { get; set; }
        public string Identificador { get; set; }
        public string Descricao { get; set; }
        public int UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }
        public Nullable<int> UsuarioAlteracao { get; set; }
        public Nullable<DateTime> DataAlteracao { get; set; }
    }
}
