
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
    
public partial class TB_BENEFICIARIO
{

    public int cd_ctt { get; set; }

    public int cd_prd { get; set; }

    public int nr_ppt { get; set; }

    public int nr_seqben { get; set; }

    public string nm_nmecli { get; set; }

    public Nullable<decimal> pr_ptcben { get; set; }

    public Nullable<System.DateTime> dt_inivigben { get; set; }

    public Nullable<System.DateTime> dt_fimvigben { get; set; }

    public Nullable<int> tp_pte { get; set; }

    public Nullable<decimal> nr_cgccpfcli { get; set; }



    public virtual TB_PROPOSTA TB_PROPOSTA { get; set; }

}

}