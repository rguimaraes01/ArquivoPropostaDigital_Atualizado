
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Projeto.Infra.DataContext
{

using System;
    using System.Collections.Generic;
    
public partial class Pessoa
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Pessoa()
    {

        this.CorretorVigencia = new HashSet<CorretorVigencia>();

    }


    public int cdpes { get; set; }

    public short tppes { get; set; }

    public decimal nrcgccpf { get; set; }

    public string nmpes { get; set; }

    public decimal cdadmcrocre { get; set; }

    public decimal cdcrocre { get; set; }

    public System.DateTime dtvldcrocre { get; set; }

    public short tppgt { get; set; }

    public int cditfful { get; set; }

    public Nullable<decimal> nremp { get; set; }

    public short tpcli { get; set; }

    public decimal nrinsiap { get; set; }

    public short tpclaisp { get; set; }

    public string iniseiss { get; set; }

    public decimal pciss { get; set; }

    public decimal nriscmun { get; set; }

    public string iniseir { get; set; }

    public System.DateTime dtcad { get; set; }

    public string inmcremp { get; set; }

    public string nmcot { get; set; }

    public string nmfanpes { get; set; }

    public decimal nrinsest { get; set; }

    public string iniseiof { get; set; }

    public System.DateTime dtnas { get; set; }

    public short tpsex { get; set; }

    public short stestciv { get; set; }

    public short nrdepirf { get; set; }

    public string inemintf { get; set; }

    public string inretinss { get; set; }

    public string incpf { get; set; }

    public Nullable<short> cdativpes { get; set; }

    public Nullable<decimal> vlrendapes { get; set; }

    public Nullable<decimal> nrrg { get; set; }

    public string orgaoexprg { get; set; }

    public Nullable<System.DateTime> dtexprg { get; set; }

    public string idpep { get; set; }

    public Nullable<bool> fl_vlacpf { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CorretorVigencia> CorretorVigencia { get; set; }

}

}
