﻿

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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class SiesEntities : DbContext
{
    public SiesEntities()
        : base("name=SiesEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<AtivPessoa> AtivPessoa { get; set; }

    public virtual DbSet<Colaborador> Colaborador { get; set; }

    public virtual DbSet<ControleProposta> ControleProposta { get; set; }

    public virtual DbSet<InterfaceSinafCobertura> InterfaceSinafCobertura { get; set; }

    public virtual DbSet<Pessoa> Pessoa { get; set; }

    public virtual DbSet<UnidadeCorretora> UnidadeCorretora { get; set; }

    public virtual DbSet<vPlanoInd> vPlanoInd { get; set; }

    public virtual DbSet<TB_DOMINIO_CAMPO> TB_DOMINIO_CAMPO { get; set; }

    public virtual DbSet<TB_LOG_PROCESSAMENTO> TB_LOG_PROCESSAMENTO { get; set; }

    public virtual DbSet<TB_REGISTRO_PROCESSAMENTO> TB_REGISTRO_PROCESSAMENTO { get; set; }

    public virtual DbSet<TB_DETALHE_REGISTRO> TB_DETALHE_REGISTRO { get; set; }

    public virtual DbSet<TB_ERRO_REGISTRO> TB_ERRO_REGISTRO { get; set; }

    public virtual DbSet<ArquivoPropostaDigital> ArquivoPropostaDigital { get; set; }

    public virtual DbSet<ArquivoPropostaDigitalDetalhe> ArquivoPropostaDigitalDetalhe { get; set; }

    public virtual DbSet<CorretorVigencia> CorretorVigencia { get; set; }

    public virtual DbSet<PlanoAssistenciaSenior> PlanoAssistenciaSenior { get; set; }

    public virtual DbSet<Produto> Produto { get; set; }

    public virtual DbSet<VigCaractProduto> VigCaractProduto { get; set; }

    public virtual DbSet<Emissao> Emissao { get; set; }

    public virtual DbSet<ControleDataBatch> ControleDataBatch { get; set; }

    public virtual DbSet<ControleDataSistema> ControleDataSistema { get; set; }

}

}
