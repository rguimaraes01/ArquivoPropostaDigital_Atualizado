
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
    
public partial class TB_CLIENTE_TELEFONE
{

    public int cd_ctt { get; set; }

    public int cd_prd { get; set; }

    public int nr_ppt { get; set; }

    public int nr_seqcli { get; set; }

    public int nr_seqclitel { get; set; }

    public Nullable<int> nr_ddd { get; set; }

    public Nullable<int> nr_tel { get; set; }

    public Nullable<int> nr_ram { get; set; }

    public Nullable<int> tp_tiptel { get; set; }

    public string ds_tiptel { get; set; }

    public string nm_cnt { get; set; }

    public string id_celpri { get; set; }

    public string id_sms { get; set; }

    public string fl_celwha { get; set; }



    public virtual TB_CLIENTE TB_CLIENTE { get; set; }

}

}
