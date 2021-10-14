using Newtonsoft.Json;
using Projeto.Domain;
using Projeto.Infra.DataContext;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Projeto.Infra
{
    public class PropostaDao
    {
        public List<Proposta> GetPropostas(string usuario, string nomeArquivo)
        {
            var listaOrigemVenda = GetData().Result;
            var mapOrigemVenda = converteOrigemVendaEmDictionary(listaOrigemVenda);

            var digitacaoContext1 = new DigitacaoEntities();
            var siesContext1 = new SiesEntities();

            var dataHoje = DateTime.Now.Date;


            //var dataControleProcessamento = siesContext1.ControleDataBatch.Where(c => c.cdprosis == "GERAÇÃO PROPOSTA DIGITAL").FirstOrDefault().dtultdiaria.Value;
            var dataControleDataSistema = siesContext1.ControleDataSistema.FirstOrDefault().dtdiariaatual;
            var dataControleDataDiaPosterior = dataControleDataSistema.AddDays(1);
            var controleDataBatch = siesContext1.ControleDataBatch.Where(c => c.cdprosis == "GERAÇÃO PROPOSTA DIGITAL").FirstOrDefault();

            var dataControleProcessamento = controleDataBatch.dtultdiaria.Value;

            //var propostasDigitacao = digitacaoContext1.TB_PROPOSTA.Where(p => p.id_sitppt.Equals("PSS") && p.dt_sitppt.CompareTo(dataControleProcessamento) == 1 && p.dt_sitppt.CompareTo(dataHoje) == -1).ToList();
            //var propostasDigitacao = digitacaoContext1.TB_PROPOSTA.Where(p => p.id_sitppt.Equals("PSS") && p.dt_sitppt >= dataControleProcessamento && p.dt_sitppt < dataControleDataDiaPosterior).ToList();
            var propostasDigitacao = digitacaoContext1.TB_PROPOSTA.Where(p => p.tp_motdve.Equals("9")).ToList(); ///DESCOMENTAR


            //var propostasSies = siesContext1.ControleProposta.Where(p => (p.stprop == 1 || p.stprop == 3) && p.dtprop.Value.CompareTo(dataControleProcessamento) == 1 && p.dtprop.Value.CompareTo(dataHoje) == -1).ToList();
            var propostasSies = siesContext1.ControleProposta.Where(p => (p.stprop == 1 || p.stprop == 3) && p.dtprop.Value >= dataControleProcessamento && p.dtprop.Value < dataControleDataDiaPosterior).ToList();
            propostasSies.Clear();

            
            Console.WriteLine();
            Console.Write("Quantidade de Propostas a Processar (Digitação): ");
            Console.WriteLine(propostasDigitacao.Count());

            Console.WriteLine();
            Console.Write("Quantidade de Propostas a Processar (Sies): ");
            Console.WriteLine(propostasSies.Count());

            var numeroTotalPropostas = propostasDigitacao.Count() + propostasSies.Count();

            //Logs inicial do processamento
            var logInicial = new TB_LOG_PROCESSAMENTO();
            logInicial.cd_pss = "Proposta Digital – json";
            logInicial.cd_usu = usuario;
            logInicial.dt_inipss = dataHoje;
            logInicial.qt_totreg = numeroTotalPropostas;

            siesContext1.TB_LOG_PROCESSAMENTO.Add(logInicial);
            siesContext1.SaveChanges();
            //Fim logs

            var lista = new ConcurrentBag<Proposta>();

            var dominioCampo = converteDominioCampoEmDictionary(siesContext1.TB_DOMINIO_CAMPO.Where(c => c.nm_cam == "tpsegagr" && c.nm_tab == "ItemOutRiscPess").ToList());

            //Nela serão incluídos os dados de nome da arquivo da proposta, usuário que está gerando o processamento, a data de criação do arquivo e a quantidade total gerada. 
            var arquivoPropostaDigital = new ArquivoPropostaDigital();
            arquivoPropostaDigital.nomeArquivo = nomeArquivo;
            arquivoPropostaDigital.usuario = usuario;
            arquivoPropostaDigital.quantidade = numeroTotalPropostas;
            arquivoPropostaDigital.dataCadastro = DateTime.Now;
            siesContext1.ArquivoPropostaDigital.Add(arquivoPropostaDigital);
            siesContext1.SaveChanges();
            

            Parallel.Invoke(() =>
            {
                if (propostasDigitacao.Count() > 0)
                {

                    Parallel.ForEach(propostasDigitacao, propostaDigitacao =>
                    {
                        var erro = new Erro();

                        var digitacaoContext2 = new DigitacaoEntities();
                        var siesContext2 = new SiesEntities();
                        var tp_vldcli = digitacaoContext2.TB_PROPOSTA.Where(e => e.nr_ppt == propostaDigitacao.nr_ppt && e.fl_dadcli == true).Select(x => x.tp_vldcli).FirstOrDefault();
                        var fl_dadcli = digitacaoContext2.TB_PROPOSTA.Where(e => e.nr_ppt == propostaDigitacao.nr_ppt && e.fl_dadcli == true).Select(x => x.fl_dadcli).FirstOrDefault();

                        var meio = tp_vldcli == null ? "" : siesContext2.TB_DOMINIO_CAMPO.Where(x => x.nm_cam == "tp_vldcli" && x.ds_vlrdmn == tp_vldcli.ToString()).Select(x => x.ds_sgncam).FirstOrDefault();

                        //Log registro processamento
                        var registroProcessamento = new TB_REGISTRO_PROCESSAMENTO();
                        registroProcessamento.cd_logpss = logInicial.cd_logpss;
                        registroProcessamento.st_regpss = "1"; //st_regpss igual a 1 que significa pendente de execução.
                        siesContext2.TB_REGISTRO_PROCESSAMENTO.Add(registroProcessamento);
                        siesContext2.SaveChanges();
                        //Fim log registro processamento

                        var externalKeyProposta = digitacaoContext2.TB_PROPOSTA.Where(e => e.nr_ppt == propostaDigitacao.nr_ppt).SingleOrDefault();
                        //var versaoProposta = siesContext2.ArquivoPropostaDigitalDetalhe.Where(c => c.numeroProposta == propostaDigitacao.nr_ppt).Count() + 1;
                        var clienteTitular = digitacaoContext2.TB_CLIENTE.Where(c => c.tp_cli == 1 && c.nr_ppt == propostaDigitacao.nr_ppt).SingleOrDefault();
                        var enderecoClienteTitular = clienteTitular.TB_CLIENTE_ENDERECO.FirstOrDefault();
                        var conjuge = digitacaoContext2.TB_CLIENTE.Where(c => c.tp_cli == 3 && c.nr_ppt == propostaDigitacao.nr_ppt).SingleOrDefault();
                        var filhos = digitacaoContext2.TB_CLIENTE.Where(c => c.tp_cli == 4 && c.nr_ppt == propostaDigitacao.nr_ppt).ToList();

                        var filhosMenorIdade = filhos.Where(c => c.dt_nsccli.HasValue && (DateTime.Now.Year - c.dt_nsccli.Value.Year - (DateTime.Now.DayOfYear < c.dt_nsccli.Value.DayOfYear ? 1 : 0)) < 18).ToList();


                        var agregados = digitacaoContext2.TB_CLIENTE.Where(c => c.tp_cli == 2 && c.nr_ppt == propostaDigitacao.nr_ppt).ToList();
                        var profissaoClienteTitular = siesContext2.AtivPessoa.Where(p => p.cdativpes == clienteTitular.cd_atd && p.tppes == 0).SingleOrDefault();
                        var profissaoConjuge = conjuge == null ? null : siesContext2.AtivPessoa.Where(p => p.cdativpes == conjuge.cd_atd && p.tppes == 0).SingleOrDefault();
                        var perguntasRespostas = digitacaoContext2.TB_RESP_PERG_PROPOSTA.Where(p => p.nr_ppt == clienteTitular.nr_ppt).ToList();
                        var pessoa = siesContext2.Colaborador.Where(c => c.nrcha == clienteTitular.TB_PROPOSTA.cd_clb).SingleOrDefault();
                        var corretorVigencia = siesContext2.CorretorVigencia.Where(c => c.cdpescor == clienteTitular.TB_PROPOSTA.cd_cra).OrderByDescending(c => c.dataInicioVigencia).FirstOrDefault();
                        var unidadeCorretora = siesContext2.UnidadeCorretora.Where(c => c.cduni == pessoa.cduni).SingleOrDefault();

                        var vPlanoInd = siesContext2.vPlanoInd.Where(c => c.nrseqplnind == clienteTitular.nr_pla && c.cdprodut == propostaDigitacao.cd_prd && c.tpcobert == 3).OrderByDescending(c => c.dtinivig).FirstOrDefault();
                        var tipoFuneral = siesContext2.InterfaceSinafCobertura.Where(c => c.cdcobert == vPlanoInd.cdcobert).FirstOrDefault();
                        var descricaoFuneral = siesContext2.TB_DOMINIO_CAMPO.Where(c => c.nm_cam == "cdtipofuneral" && c.nm_tab == "InterfaceSinafCobertura" && c.ds_vlrdmn == tipoFuneral.cdtipofuneral.ToString()).SingleOrDefault();
                        var assistenciaSenior = siesContext2.PlanoAssistenciaSenior.Where(c => c.codigoProduto == propostaDigitacao.cd_prd && c.codigoPlano == clienteTitular.nr_pla).OrderByDescending(c => c.dataVigencia).FirstOrDefault();
                        var descricaoPlano = assistenciaSenior != null ? siesContext2.TB_DOMINIO_CAMPO.Where(c => c.ds_vlrdmn == assistenciaSenior.tipoPlano.ToString() && c.nm_cam == "tipoPlano" && c.nm_tab == "PlanoAssistenciaSenior").FirstOrDefault() : null;

                        var controleProposta = siesContext2.ControleProposta.Where(c => c.nrppscor == clienteTitular.TB_PROPOSTA.nr_ppt).SingleOrDefault();
                        var descricaoStatusProposta = siesContext2.TB_DOMINIO_CAMPO.Where(c => c.ds_vlrdmn == controleProposta.stprop.ToString() && c.nm_cam == "stprop" && c.nm_tab == "ControleProposta").SingleOrDefault();

                        var vencimentoProposta = digitacaoContext2.TB_PROPOSTA.Where(p => p.nr_ppt == clienteTitular.TB_PROPOSTA.nr_ppt).OrderBy(p => p.dt_inivig).FirstOrDefault();
                        var colaborador = siesContext2.Pessoa.Where(c => c.cdpes == pessoa.cdpescol).SingleOrDefault();
                        var corretor = siesContext2.Pessoa.Where(c => c.cdpes == pessoa.cdpescor).SingleOrDefault();

                        var dadosCartao = digitacaoContext2.TB_DADOS_CARTAO.Where(c => c.nr_ppt == propostaDigitacao.nr_ppt).SingleOrDefault();
                        var dadosBancarios = digitacaoContext2.TB_DADOS_BANCARIOS.Where(c => c.nr_ppt == clienteTitular.nr_ppt).SingleOrDefault();

                        var celular = this.obtemCelular(clienteTitular.TB_CLIENTE_TELEFONE.ToList());

                        var produto = siesContext2.Produto.Where(c => c.cdprodut == propostaDigitacao.cd_prd).FirstOrDefault();
                        var vigCaractProduto = siesContext2.VigCaractProduto.Where(c => c.cdprodut == propostaDigitacao.cd_prd).FirstOrDefault();
                        var emissao = siesContext2.Emissao.Where(c => c.cdconseg == propostaDigitacao.cd_ctt).FirstOrDefault();
                        var beneficiariosCliente = digitacaoContext2.TB_CLIENTE.Where(c => c.nr_ppt == propostaDigitacao.nr_ppt).ToList();


                        var externalKeyPropostaGuid = Guid.NewGuid();

                        //Log detalhe registro
                        var logDetalheRegistro = new TB_DETALHE_REGISTRO();
                        logDetalheRegistro.cd_cam = 31; // cd_cam igual a 31 significa Número da Proposta de acordo com a tabela TB_CAMPO
                        logDetalheRegistro.vl_cam = propostaDigitacao.nr_ppt.ToString();
                        logDetalheRegistro.cd_regpss = registroProcessamento.cd_regpss;
                        siesContext2.TB_DETALHE_REGISTRO.Add(logDetalheRegistro);
                        siesContext2.SaveChanges();
                        //Fim log detalhe registro 


                        var proposta = new Proposta(erro)
                        {
                            externalKey = externalKeyPropostaGuid.ToString(),
                            statusProposta = descricaoStatusProposta.ds_sgncam,
                            nome = clienteTitular?.nm_nmecli,
                            cpf = clienteTitular?.nr_cgccpfcli.ToString().PadLeft(11, '0'),
                            vencimento = propostaDigitacao.dt_inivig.Value.ToShortDateString(),
                            proposta = propostaDigitacao.nr_ppt,
                            tipoComunicacao = (clienteTitular.TB_PROPOSTA.cd_envkit == 1 ? "Digital" : "Impressa"),
                            nomeProduto = produto?.nmprodut.Trim(),
                            processoSusep = vigCaractProduto?.cdprcsusep.Trim(),
                            numeroApolice = emissao.cdorgprtsuc.ToString() + "/" + produto.cdramosg.ToString() + "/" + emissao.nrapo.ToString(),
                            numeroContrato = vencimentoProposta?.cd_ctt ?? 0,
                        };

                        #region Titular
                        proposta.titular = new Titular(erro)
                        {

                            nomeSegurado = clienteTitular?.nm_nmecli,
                            sexo = deParaSexo(clienteTitular?.tp_sexocli),
                            cpf = clienteTitular?.nr_cgccpfcli.ToString().PadLeft(11, '0'),
                            rg = clienteTitular.nr_idtcli.ToString().PadLeft(9, '0'),
                            orgaoExpedidor = clienteTitular.dc_orgexdidtcli.ToUpper(),
                            dataExpedicao = clienteTitular?.dt_exd_idtcli?.ToShortDateString(),
                            dataNascimento = clienteTitular.dt_nsccli?.ToShortDateString(),
                            estadoCivil = clienteTitular.ds_estcivcli.Trim(),
                            rendaFamiliar = clienteTitular?.vl_rdacli.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) ?? "0,00",
                            cep = enderecoClienteTitular.cd_ceppes.ToString(),
                            endereco = enderecoClienteTitular.dc_nmeend,
                            numero = enderecoClienteTitular.nr_end.GetValueOrDefault(),
                            complemento = enderecoClienteTitular.dc_cplend == null ? "" : enderecoClienteTitular.dc_cplend,
                            bairro = enderecoClienteTitular.dc_nmebai,
                            cidade = enderecoClienteTitular.dc_nmecid,
                            referencia = enderecoClienteTitular.dc_refend ?? "",
                            celular = celular,
                            whatsApp = verificaSeCelularPossuiWhatsapp(clienteTitular.TB_CLIENTE_TELEFONE.ToList(), celular),
                            residencial = obtemTelefoneResidencial(clienteTitular.TB_CLIENTE_TELEFONE.ToList()),
                            comercial = obtemTelefoneComercial(clienteTitular.TB_CLIENTE_TELEFONE.ToList()),
                            uf = enderecoClienteTitular?.sg_uf ?? "",
                            profissao = profissaoClienteTitular?.nmativpes.ToString().Trim(),
                            codigoProfissao = profissaoClienteTitular.codigoCNAE,
                            email = clienteTitular?.TB_CLIENTE_EMAIL.FirstOrDefault()?.nm_email ?? "",

                            assistenciaFuneral = descricaoFuneral.ds_sgncam,
                            plano = descricaoPlano?.ds_sgncam,
                            //capitalSeguradoMorteAcidencial = clienteTitular.vl_isap.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                            //valorPremioAcidentesPessoais = clienteTitular.vl_pmo.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR"))
                        };
                        proposta.titular.coberturas = new CoberturaTitular()
                        {
                            capitalSeguradoMorteAcidencial = clienteTitular.vl_isap.HasValue ? clienteTitular.vl_isap?.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "",
                            valorPremioAcidentesPessoais = clienteTitular.vl_pmo.HasValue ? clienteTitular.vl_pmo?.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "",
                            capitalSeguradoAssistenciaEmergencial = clienteTitular.vl_isdoecgn.HasValue ? clienteTitular.vl_isdoecgn?.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "",
                            valorPremioAssistenciaEmergencial = clienteTitular.vl_pmoassemg.HasValue ? clienteTitular.vl_pmoassemg?.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "",
                        };
                        proposta.titular.coberturas.rendaMensal = this.InstanciaRendaMensal(propostaDigitacao, erro);

                        #endregion
                        var listaAgergados = new List<Agregados>();
                        listaAgergados.Add(new Agregados(new Erro()));
                        proposta.agregados = agregados.Any() == false ? listaAgergados : GeraAgregados(agregados, dominioCampo, erro);
                        //proposta.valorPremioAgregados = this.SomaValorAgregados(agregados).ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR"));

                        proposta.calculoPremioTotal = new CalculoPremioTotal(erro)
                        {
                            vencimento = vencimentoProposta?.dt_inivig?.ToShortDateString(),
                            valorPremioAcidentesPessoais = calcularPremioAcidentesPessoais(clienteTitular, conjuge), //clienteTitular.vl_pmo.HasValue ? clienteTitular.vl_pmo.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "",
                            valorPremioAgregados = this.SomaValorAgregados(agregados).ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                            valorPremioRendaMensal = propostaDigitacao.vr_cobadccjg.HasValue ? (propostaDigitacao.vr_cobadc.Value + propostaDigitacao.vr_cobadccjg.Value).ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : propostaDigitacao.vr_cobadc.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                            valorPremioTotal = this.RetornaPremioTotal(clienteTitular, conjuge, agregados, propostaDigitacao).ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR"))
                        };

                        proposta.dadosCorretor = new DadosCorretor(erro)
                        {
                            angariador = colaborador.nmpes.Trim(),
                            codigoAngariador = (int)clienteTitular.TB_PROPOSTA.cd_clb,
                            unidade = unidadeCorretora.dsuni,
                            corretor = corretor.nmpes.Trim(),
                            registroSUSEP = corretorVigencia.cdregsus,
                        };
                        proposta.declaracaoPessoalSaude = this.InstanciaPerguntasResposta(perguntasRespostas, erro, conjuge);


                        //SEÇÃO CONJUGE
                        if(conjuge != null)
                        {
                            proposta.conjuge = this.InstanciaConjuge(conjuge, profissaoConjuge, clienteTitular?.nr_cgccpfcli.ToString().PadLeft(11, '0'));

                            proposta.conjuge.coberturas = proposta.conjuge == null || conjuge == null ? null : new CoberturaConjuge()
                            {
                                plano = clienteTitular.ds_pla,
                                capitalSeguradoMorteAcidencial = conjuge.vl_isap.HasValue ? conjuge.vl_isap.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "",
                                valorPremioAcidentesPessoais = conjuge.vl_pmo.HasValue ? conjuge.vl_pmo.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "",
                                capitalSeguradoAssistenciaEmergencial = conjuge.vl_isassemg.HasValue ? conjuge.vl_isdoecgn.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "",
                                valorPremioAssistenciaEmergencial = conjuge.vl_pmoassemg.HasValue ? conjuge.vl_pmoassemg.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "",
                            };

                            proposta.conjuge.coberturas.rendaMensal = conjuge == null || proposta.conjuge == null || proposta.conjuge.coberturas == null ? null : this.InstanciaRendaMensalConjuge(conjuge.TB_PROPOSTA, erro);
                        }
                       
                        proposta.filhos = filhos.Any() == false ? null : this.PercorreFilhos(filhos, descricaoPlano?.ds_sgncam);

                        proposta.origemVenda = mapOrigemVenda[filtraOrigemDaVenda(clienteTitular?.TB_PROPOSTA?.cd_orivnd)].Descricao;
                        proposta.formaPagamento = new FormaPagamento(erro)
                        {
                            primeiraParcela = TrocarNomenclatura(dadosCartao.tp_cba),
                            demaisParcelas = TrocarNomenclatura(dadosCartao.tp_cba) == "Cartão de Crédito" ? "Cartão de Crédito" : dadosBancarios == null ? "Boleto" : "Débito em Conta",
                            dadosDebito = dadosBancarios == null ? null : new DadosDebito(erro)
                            {
                                titular = dadosBancarios.nm_ttlcta,
                                cpfTitular = dadosBancarios.nr_cpfttlcta.Trim(),
                                codigoBanco = int.Parse(dadosBancarios.nr_bco),
                                numeroAgencia = dadosBancarios.nr_agc.Trim() + "-" + dadosBancarios.nr_dva.Trim(),
                                numeroConta = dadosBancarios.nr_cta.Trim() + "-" + dadosBancarios.nr_dvc.Trim(),
                                tipoConta = dadosBancarios.nr_tipcta == "1" ? "Corrente" : "Poupanca"
                            }
                        };

                        proposta.beneficiarios =  geraBeneficiarios(digitacaoContext2, propostaDigitacao.nr_ppt, erro); 
                        proposta.data = clienteTitular.TB_PROPOSTA.dt_vndppt?.ToShortDateString();
                        proposta.final = new Final()
                        {
                            Local = enderecoClienteTitular.dc_nmecid,
                            Telefone = this.obtemCelular(clienteTitular.TB_CLIENTE_TELEFONE.ToList()),
                            DataHora = propostaDigitacao.dt_vndppt.ToString(),
                            Meio = meio,
                            AceiteDigital = fl_dadcli.HasValue && fl_dadcli.Value ? "S" : "N"
                            };
                        
                        if (erro.ocorreu)
                        {
                            //Log registro processamento
                            registroProcessamento.st_regpss = "3"; //st_regpss igual a 3 significa erro.
                            registroProcessamento.dt_sitreg = DateTime.Now;
                            siesContext2.SaveChanges();
                            //Fim log registro processamento

                            var logErroRegistro = new TB_ERRO_REGISTRO();
                            logErroRegistro.cd_regpss = registroProcessamento.cd_regpss;
                            logErroRegistro.cd_err = 400;
                            logErroRegistro.ds_msgerr = JsonConvert.SerializeObject(erro.mensagens);
                            siesContext2.TB_ERRO_REGISTRO.Add(logErroRegistro);
                            siesContext2.SaveChanges();

                        }
                        else
                        {
                            var registroDetalhePropostaDigital = new ArquivoPropostaDigitalDetalhe();
                            registroDetalhePropostaDigital.idArquivoPropostaDigital = arquivoPropostaDigital.idArquivoPropostaDigital;
                            registroDetalhePropostaDigital.numeroProposta = propostaDigitacao.nr_ppt;
                            registroDetalhePropostaDigital.externalKey = externalKeyPropostaGuid.ToString();
                            registroDetalhePropostaDigital.statusProposta = 2;

                            registroDetalhePropostaDigital.json = JsonConvert.SerializeObject(proposta);
                            siesContext2.ArquivoPropostaDigitalDetalhe.Add(registroDetalhePropostaDigital);
                            siesContext2.SaveChanges();


                            //Log registro processamento
                            registroProcessamento.st_regpss = "2"; //st_regpss igual a 3 significa sucesso.
                            registroProcessamento.dt_sitreg = DateTime.Now;
                            siesContext2.SaveChanges();
                            //Fim log registro processamento

                            lista.Add(proposta);
                        }
                    });
                }
            },
                () =>
                {
                    if (propostasSies.Count() > 0)
                    {
                        Parallel.ForEach(propostasSies, propostaSies =>
                        {

                            var externalKeyPropostaGuid = Guid.NewGuid().ToString();
                            var erro = new Erro();
                            var proposta = new Proposta(erro);
                            var siesContext3 = new SiesEntities();
                            var descricaoStatusProposta = siesContext3.TB_DOMINIO_CAMPO.Where(c => c.ds_vlrdmn == propostaSies.stprop.ToString() && c.nm_cam == "stprop" && c.nm_tab == "ControleProposta").SingleOrDefault();

                            proposta.proposta = propostaSies.nrppscor;
                            proposta.statusProposta = descricaoStatusProposta?.ds_sgncam;
                            proposta.externalKey = externalKeyPropostaGuid;

                            var registroDetalhePropostaDigital = new ArquivoPropostaDigitalDetalhe();
                            registroDetalhePropostaDigital.idArquivoPropostaDigital = arquivoPropostaDigital.idArquivoPropostaDigital;
                            registroDetalhePropostaDigital.numeroProposta = propostaSies.nrppscor;
                            registroDetalhePropostaDigital.externalKey = externalKeyPropostaGuid;
                            registroDetalhePropostaDigital.statusProposta = propostaSies.stprop ?? 0;
                            registroDetalhePropostaDigital.json = JsonConvert.SerializeObject(proposta);
                            siesContext3.ArquivoPropostaDigitalDetalhe.Add(registroDetalhePropostaDigital);
                            siesContext3.SaveChanges();


                            //Log registro processamento
                            var registroProcessamento = new TB_REGISTRO_PROCESSAMENTO();
                            registroProcessamento.cd_logpss = logInicial.cd_logpss;
                            registroProcessamento.dt_sitreg = DateTime.Now;
                            registroProcessamento.st_regpss = "2"; //st_regpss igual a 1 que significa pendente de execução.
                            siesContext3.TB_REGISTRO_PROCESSAMENTO.Add(registroProcessamento);
                            siesContext3.SaveChanges();
                            //Fim log registro processamento

                            lista.Add(proposta);
                        });
                    }
                }
            );

            //Log atualizando status log inicial TB_LOG_PROCESSAMENTO 
            logInicial.dt_fimpss = DateTime.Now;

            //controleDataBatch.dtultdiaria = dataControleDataDiaPosterior;
            //siesContext1.Entry(controleDataBatch).State = System.Data.Entity.EntityState.Modified;
            //siesContext1.ControleDataBatch.Attach(controleDataBatch);
            siesContext1.SaveChanges();
            //Fim Log atualizando status log inicil TB_LOG_PROCESSAMENTO 

            return lista.ToList();
        }

        private string TrocarNomenclatura(string tp_cba)
        {
            switch (tp_cba)
            {
                case "Carne":
                    return "Boleto";
                case "Credito":
                    return "Cartão de Crédito";
                case "Debito":
                    return "Boleto";
                case "CartaoDebito":
                    return "Cartão de Débito";
            }
            return "";
        }

        private string filtraOrigemDaVenda(string origemVenda)
        {
            if (origemVenda != null)
            {
                return origemVenda;
            }

            return "";
        }

        private String aplicaMascaraTelefone(TB_CLIENTE_TELEFONE telefone)
        {

            string strMascara = "{0:0000-0000}";
            if (telefone.nr_tel.ToString().Length == 9)
                strMascara = "{0:00000-0000}";
            string numeroFormatado = string.Format(strMascara, telefone.nr_tel);
            return $"({telefone.nr_ddd}){numeroFormatado}";
        }

        public String obtemCelular(List<TB_CLIENTE_TELEFONE> telefones)
        {
            bool primeiraExecucao = true;
            var celular = "";
            foreach (var telefone in telefones)
            {
                if (telefone.tp_tiptel == (int)TipoTelefone.Celular)
                {
                    if (primeiraExecucao)
                    {
                        celular = aplicaMascaraTelefone(telefone);
                        primeiraExecucao = false;
                    }
                    if (telefone.id_celpri.Equals("S"))
                    {
                        celular = aplicaMascaraTelefone(telefone);
                    }
                }
            }
            return celular;
        }

        public String verificaSeCelularPossuiWhatsapp(List<TB_CLIENTE_TELEFONE> telefones, string celular)
        {
            var telefone = telefones.Find(tel => aplicaMascaraTelefone(tel).Contains(celular));

            if (telefone == null)
            {
                return "";
            }

            return telefone.fl_celwha;
        }


        public String obtemTelefoneComercial(List<TB_CLIENTE_TELEFONE> telefones)
        {
            foreach (var telefone in telefones)
            {
                if (telefone.tp_tiptel == 1)
                {
                    return aplicaMascaraTelefone(telefone);
                }
            }
            return "";
        }

        public String obtemTelefoneResidencial(List<TB_CLIENTE_TELEFONE> telefones)
        {
            foreach (var telefone in telefones)
            {
                if (telefone.tp_tiptel == 0)
                {
                    return aplicaMascaraTelefone(telefone);
                }
            }
            return "";
        }

        private String deParaSexo(int? idSexo)
        {
            if (idSexo == null)
            {
                new Exception("O sexo não pode ser nulo");
            }
            return (idSexo == 0 ? "Masculino" : "Feminino");
        }

        private Conjuge InstanciaConjuge(TB_CLIENTE clienteTipoTres, AtivPessoa ativPessoa, string CPF)
        {
            Conjuge conjuge;

            conjuge = new Conjuge
            {
                nomeSegurado = clienteTipoTres?.nm_nmecli,
                sexo = this.deParaSexo(clienteTipoTres?.tp_sexocli),
                cpf = clienteTipoTres?.nr_cgccpfcli.ToString().PadLeft(11, '0') == CPF ? "0" : clienteTipoTres?.nr_cgccpfcli.ToString().PadLeft(11, '0'),
                dataNascimento = clienteTipoTres?.dt_nsccli?.ToShortDateString(),
                profissao = ativPessoa?.nmativpes.ToString().Trim(),
                codigoProfissao = ativPessoa.codigoCNAE,
            };

            return conjuge;
        }

        /*
            Esse método formata a profissão.
            Exemplo de retorno do banco: CAI062-Caixa
            Exemplo de como fica o resultado desse método: CAIXA
         */
        /*
        private string FormataProfissao(string profissao)
        {
            int separadorIndex = profissao.IndexOf("-");

            string subString = profissao.Trim().Substring(separadorIndex + 1);

            return subString.ToUpper();
        }
        */

        private Filhos InstanciaFilho(TB_CLIENTE filho_TB_CLIENTE, string planoTitular)
        {
            Filhos filho = new Filhos
            {
                nomeSegurado = filho_TB_CLIENTE.nm_nmecli,
                dataNascimento = filho_TB_CLIENTE.dt_nsccli?.ToShortDateString(),
                sexo = this.deParaSexo(filho_TB_CLIENTE.tp_sexocli),
                cpf = filho_TB_CLIENTE.nr_cgccpfcli.ToString().PadLeft(11, '0'),
                parentesco = "Filho",
                plano = planoTitular,
                valorPremio = filho_TB_CLIENTE.vl_pmo.HasValue ? filho_TB_CLIENTE.vl_pmo.ToString() : ""
            };

            return filho;
        }

        private List<Filhos> PercorreFilhos(List<TB_CLIENTE> tbFilhos, string descricaoPlano)
        {
            List<Filhos> listaFilhos = new List<Filhos>();

            foreach (TB_CLIENTE tbFilho in tbFilhos)
            {
                Filhos filho = this.InstanciaFilho(tbFilho, descricaoPlano);
                listaFilhos.Add(filho);
            }

            return listaFilhos;
        }

        private decimal SomaValorAgregados(List<TB_CLIENTE> agregados)
        {
            decimal soma = 0;
            foreach (TB_CLIENTE agregado in agregados)
            {
                if (agregado.vl_pmo != null)
                {
                    soma += (decimal)agregado.vl_pmo;
                }
            }

            return soma;
        }

        private List<Agregados> GeraAgregados(List<TB_CLIENTE> agregados, Dictionary<string, string> dominioCampo, Erro erro)
        {
            List<Agregados> listaJsonAgregados = new List<Agregados>();
            //var valorPremioFunealAgredadosMaster = agregados.Where(x => x.dt_nsccli.HasValue && CalculaIdade(x.dt_nsccli.Value) >= 81 && CalculaIdade(x.dt_nsccli.Value) <= 85).Select(x => x.vl_pmo).Sum();
            foreach (TB_CLIENTE agregado in agregados)
            {
                Agregados objAgregado = new Agregados(erro);

                objAgregado.nomeSegurado = agregado?.nm_nmecli;
                objAgregado.dataNascimento = agregado.dt_nsccli.Value.ToShortDateString();
                objAgregado.sexo = this.deParaSexo(agregado?.tp_sexocli);
                objAgregado.parentesco = dominioCampo[agregado?.tp_tit.ToString()];
                objAgregado.plano = agregado.ds_pla.Trim();
                objAgregado.valorPremio = (decimal)agregado.vl_pmo;
                objAgregado.cpf = agregado?.nr_cgccpfcli.ToString().PadLeft(11, '0');
                //objAgregado.valorPremioFuneralMaster = valorPremioFunealAgredadosMaster.HasValue ? valorPremioFunealAgredadosMaster.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "";

                listaJsonAgregados.Add(objAgregado);
            }

            return listaJsonAgregados;
        }
        private List<Beneficiario> geraBeneficiarios(DigitacaoEntities digitacaoEntities,  int nr_ppt , Erro erro)
        {
            List<Beneficiario> listaBeneficiario = new List<Beneficiario>();

            var listaTB_Beneficiario = digitacaoEntities.TB_BENEFICIARIO.Where(x => x.nr_ppt == nr_ppt).ToList();

            foreach (var item in listaTB_Beneficiario)
            {
                Beneficiario beneficiario = new Beneficiario(erro);
                beneficiario.CPF = item.nr_cgccpfcli.ToString().PadLeft(11, '0');
                beneficiario.nomeBeneficiário = item.nm_nmecli;
                beneficiario.Percentual = item.pr_ptcben.HasValue ? item.pr_ptcben.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")) : "";
                beneficiario.parentesco = item.tp_pte.HasValue ?  digitacaoEntities.TB_DOMINIO_CAMPO.Where(c => c.nm_cam == "tp_pte" && c.ds_vlrdmn == item.tp_pte.Value.ToString()).Select(x => x.ds_sgncam).FirstOrDefault() : "";
                listaBeneficiario.Add(beneficiario);
            }
                return listaBeneficiario;
        }
        
        private RendaMensal InstanciaRendaMensal(TB_PROPOSTA proposta, Erro erro)
        {
            RendaMensal rendaMensal = new RendaMensal(erro)
            {
                prazo = (decimal)proposta.vr_cobadc > 0 ? (int)proposta.qt_przrec : 0,
                //capitalSegurado = proposta.vl_przrec.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                valorParcela = proposta.vl_przrec.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                //capitalSegurado = (proposta.vl_przrec / proposta.qt_przrec).Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                capitalSegurado = (proposta.vl_przrec * proposta.qt_przrec).Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                valorPremioRendaMensal = proposta.vr_cobadc.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
            };
            return rendaMensal;
        }

        private RendaMensalConjuge InstanciaRendaMensalConjuge(TB_PROPOSTA proposta, Erro erro)
        {
            if (proposta == null)
                return null;
            RendaMensalConjuge rendaMensalConjuge = new RendaMensalConjuge(erro)
            {
                prazo = (decimal)proposta.vr_cobadccjg > 0 ? (int)proposta.qt_przreccjg : 0,
                //capitalSegurado = proposta.vl_przrec.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                valorParcela = proposta.vl_przreccjg.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                //capitalSegurado = (proposta.vl_przrec / proposta.qt_przrec).Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                capitalSegurado = (proposta.vl_przreccjg * proposta.qt_przreccjg).Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
                valorPremioRendaMensal = proposta.vr_cobadccjg.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR")),
            };
            return rendaMensalConjuge;
        }

        static async Task<List<OrigemVenda>> GetData()
        {
            string baseUrl = "http://sigavital.vitallatina.com.br/api/OrigemVenda";

            using (HttpClient client = new HttpClient())

            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    return JsonConvert.DeserializeObject<List<OrigemVenda>>(data);

                }
                return null;
            }
        }

        private Dictionary<String, OrigemVenda> converteOrigemVendaEmDictionary(List<OrigemVenda> origensVenda)
        {
            var origensVendaMap = new Dictionary<string, OrigemVenda>();
            origensVenda.ForEach(origemVenda =>
            {
                origensVendaMap.Add(origemVenda.Identificador, origemVenda);
            });

            //Adicionado devido erro no map (Verificar forma melhor de corrigir)
            origensVendaMap.Add("", new OrigemVenda
            {
                Sequencial = 0,
                CodigoCorretora = 0,
                Identificador = "",
                Descricao = "",
                UsuarioInclusao = 0,
                DataInclusao = DateTime.Now,
            });

            return origensVendaMap;
        }


        private Dictionary<string, string> converteDominioCampoEmDictionary(List<TB_DOMINIO_CAMPO> listaTbDominioCampo)
        {
            var dominioCampo = new Dictionary<string, string>();
            listaTbDominioCampo.ForEach(elemento =>
            {
                dominioCampo.Add(elemento.ds_vlrdmn, elemento.ds_sgncam);
            });
            return dominioCampo;
        }

        private DeclaracaoPessoalSaude InstanciaPerguntasResposta(List<TB_RESP_PERG_PROPOSTA> perguntasRespostas, Erro erro, TB_CLIENTE conjuge = null)
        {
            var declaracaoPessoalSaude = new DeclaracaoPessoalSaude(erro);
            declaracaoPessoalSaude.perguntasRespostas = new List<PerguntasResposta>();

            var perguntasRespostaSeguradoPrincipal = new PerguntasResposta(erro);
            perguntasRespostaSeguradoPrincipal.tipoSegurado = "Principal";
            perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS = new List<PerguntaRespostaDPS>();


            var perguntasRespostaSeguradoConjuge = new PerguntasResposta(erro);
            perguntasRespostaSeguradoConjuge.tipoSegurado = "Cônjuge";
            perguntasRespostaSeguradoConjuge.perguntaRespostaDPS = new List<PerguntaRespostaDPS>();


            var perguntasRespostasSeguradoConjuge = new List<PerguntaRespostaDPS>();

            perguntasRespostas.ForEach(elemento =>
            {
                var perguntaRespostaDPS = new PerguntaRespostaDPS(erro);
                perguntaRespostaDPS.resposta = elemento.ds_rsp;
                perguntaRespostaDPS.justificativa = elemento.ds_obs;


                switch (elemento.nr_ord)
                {
                    case 0:
                        perguntaRespostaDPS.codigoPergunta = 1;
                        perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;
                    case 1:
                        perguntaRespostaDPS.codigoPergunta = 2;
                        perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;
                    case 2:
                        perguntaRespostaDPS.codigoPergunta = 3;
                        perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;
                    case 3:
                        perguntaRespostaDPS.codigoPergunta = 4;
                        perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;
                    case 4:
                        perguntaRespostaDPS.codigoPergunta = 5;
                        perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;
                    case 5:
                        perguntaRespostaDPS.codigoPergunta = 6;
                        perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;
                    case 16:
                        perguntaRespostaDPS.codigoPergunta = 7;
                        perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;
                    case 6:
                        perguntaRespostaDPS.codigoPergunta = 8;
                        perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;
                    case 7:
                        perguntaRespostaDPS.codigoPergunta = 9;
                        perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;

                    case 8:
                        perguntaRespostaDPS.codigoPergunta = 1;
                        perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;

                    case 9:
                        perguntaRespostaDPS.codigoPergunta = 2;
                        perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;

                    case 10:
                        perguntaRespostaDPS.codigoPergunta = 3;
                        perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;

                    case 11:
                        perguntaRespostaDPS.codigoPergunta = 4;
                        perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;

                    case 12:
                        perguntaRespostaDPS.codigoPergunta = 5;
                        perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;

                    case 13:
                        perguntaRespostaDPS.codigoPergunta = 6;
                        perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;

                    case 17:
                        perguntaRespostaDPS.codigoPergunta = 7;
                        perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;

                    case 14:
                        perguntaRespostaDPS.codigoPergunta = 8;
                        perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;

                    case 15:
                        perguntaRespostaDPS.codigoPergunta = 9;
                        perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.Add(perguntaRespostaDPS);
                        break;
                    default:
                        break;
                }

                declaracaoPessoalSaude.filhoBeneficiario = elemento.TB_CLIENTE.TB_PROPOSTA.id_bencjgflh;
                declaracaoPessoalSaude.dependenteAgregado = elemento.TB_CLIENTE.TB_PROPOSTA.id_autdpdagr;
                declaracaoPessoalSaude.conjugeProponente = elemento.TB_CLIENTE.TB_PROPOSTA.id_ppncjgpcp;
            });

            perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS = perguntasRespostaSeguradoPrincipal.perguntaRespostaDPS.OrderBy(x => x.codigoPergunta).ToList();
            perguntasRespostaSeguradoConjuge.perguntaRespostaDPS = perguntasRespostaSeguradoConjuge.perguntaRespostaDPS.OrderBy(x => x.codigoPergunta).ToList();

            declaracaoPessoalSaude.perguntasRespostas.Add(perguntasRespostaSeguradoPrincipal);

            if (conjuge != null)
            {
                declaracaoPessoalSaude.perguntasRespostas.Add(perguntasRespostaSeguradoConjuge);
            }

            return declaracaoPessoalSaude;
        }

        private decimal RetornaPremioTotal(TB_CLIENTE acidentesPessoais, TB_CLIENTE conjuge, List<TB_CLIENTE> premioAgregados, TB_PROPOSTA rendaMensal)
        {

            decimal? premioAcidentesPessoais = acidentesPessoais.vl_pmo.HasValue ? acidentesPessoais.vl_pmo.Value : 0;
            if (conjuge != null)
                premioAcidentesPessoais = conjuge.vl_pmo.HasValue ? premioAcidentesPessoais + conjuge.vl_pmo.Value : premioAcidentesPessoais;
            var premioAcidentesPessoaisAux = premioAcidentesPessoais.HasValue ? premioAcidentesPessoais.Value : 0;
            var somaPremioAgregados = this.SomaValorAgregados(premioAgregados);
            var somaPremioRendaMensal =  (decimal)rendaMensal.vr_cobadc + (decimal)rendaMensal.vr_cobadccjg;

            return premioAcidentesPessoaisAux + somaPremioAgregados + somaPremioRendaMensal;
        }
        private int CalculaIdade(DateTime dataNascimento)
        {
            int idade = DateTime.Now.Year - dataNascimento.Year;
            if (DateTime.Now.DayOfYear < dataNascimento.DayOfYear)
            {
                idade = idade - 1;
            }
            return idade;
        }

        private string calcularPremioAcidentesPessoais(TB_CLIENTE titular, TB_CLIENTE conjuge = null)
        {
            //Conjuge está sendo pego assim: TB_CLIENTE.Where(c => c.tp_cli == 3 && c.nr_ppt == propostaDigitacao.nr_ppt)
            decimal? valorRetorno = titular.vl_pmo;
            if (conjuge != null)
                valorRetorno = valorRetorno  + conjuge.vl_pmo;

            return valorRetorno.Value.ToString("N2", CultureInfo.CreateSpecificCulture("pt-BR"));
        }


    }
}