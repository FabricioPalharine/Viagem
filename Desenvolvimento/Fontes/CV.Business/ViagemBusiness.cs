using System.Collections.Generic;
using CV.Business.Library;
using CV.Data;

using CV.Model;
using CV.Model.Results;
using System;
namespace CV.Business

{
    public partial class ViagemBusiness : BusinessBase
    {
			public AluguelGasto SelecionarAluguelGasto (int? Identificador)
			{
				 LimparValidacao();
AluguelGasto RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarAluguelGasto(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<AluguelGasto> ListarAluguelGasto ()
			{
				 LimparValidacao();
IList<AluguelGasto> RetornoAcao = new List<AluguelGasto>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarAluguelGasto();
				}
			}
				return RetornoAcao;
			}
			public void SalvarAluguelGasto (AluguelGasto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioAluguelGasto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarAluguelGasto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarAluguelGasto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirAluguelGasto (AluguelGasto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoAluguelGasto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirAluguelGasto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirAluguelGasto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Reabastecimento SelecionarReabastecimento (int? Identificador)
			{
				 LimparValidacao();
Reabastecimento RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarReabastecimento(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Reabastecimento> ListarReabastecimento ()
			{
				 LimparValidacao();
IList<Reabastecimento> RetornoAcao = new List<Reabastecimento>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarReabastecimento();
				}
			}
				return RetornoAcao;
			}


        public void SalvarReabastecimento (Reabastecimento itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioReabastecimento(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarReabastecimento(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarReabastecimento_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirReabastecimento (Reabastecimento itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoReabastecimento(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirReabastecimento(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirReabastecimento_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Amigo SelecionarAmigo (int? Identificador)
			{
				 LimparValidacao();
Amigo RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarAmigo(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Amigo> ListarAmigo ()
			{
				 LimparValidacao();
IList<Amigo> RetornoAcao = new List<Amigo>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarAmigo();
				}
			}
				return RetornoAcao;
			}

     

        public void SalvarAmigo (Amigo itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioAmigo(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarAmigo(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarAmigo_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirAmigo (Amigo itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoAmigo(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirAmigo(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirAmigo_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public AporteDinheiro SelecionarAporteDinheiro (int? Identificador)
			{
				 LimparValidacao();
AporteDinheiro RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarAporteDinheiro(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<AporteDinheiro> ListarAporteDinheiro ()
			{
				 LimparValidacao();
IList<AporteDinheiro> RetornoAcao = new List<AporteDinheiro>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarAporteDinheiro();
				}
			}
				return RetornoAcao;
			}
			public void SalvarAporteDinheiro (AporteDinheiro itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioAporteDinheiro(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarAporteDinheiro(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarAporteDinheiro_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirAporteDinheiro (AporteDinheiro itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoAporteDinheiro(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirAporteDinheiro(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirAporteDinheiro_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Atracao SelecionarAtracao (int? Identificador)
			{
				 LimparValidacao();
Atracao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarAtracao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Atracao> ListarAtracao ()
			{
				 LimparValidacao();
IList<Atracao> RetornoAcao = new List<Atracao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarAtracao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarAtracao (Atracao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioAtracao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarAtracao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarAtracao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirAtracao (Atracao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoAtracao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirAtracao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirAtracao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public AvaliacaoAerea SelecionarAvaliacaoAerea (int? Identificador)
			{
				 LimparValidacao();
AvaliacaoAerea RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarAvaliacaoAerea(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<AvaliacaoAerea> ListarAvaliacaoAerea ()
			{
				 LimparValidacao();
IList<AvaliacaoAerea> RetornoAcao = new List<AvaliacaoAerea>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarAvaliacaoAerea();
				}
			}
				return RetornoAcao;
			}
			public void SalvarAvaliacaoAerea (AvaliacaoAerea itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioAvaliacaoAerea(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarAvaliacaoAerea(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarAvaliacaoAerea_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirAvaliacaoAerea (AvaliacaoAerea itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoAvaliacaoAerea(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirAvaliacaoAerea(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirAvaliacaoAerea_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public AvaliacaoAluguel SelecionarAvaliacaoAluguel (int? Identificador)
			{
				 LimparValidacao();
AvaliacaoAluguel RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarAvaliacaoAluguel(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<AvaliacaoAluguel> ListarAvaliacaoAluguel ()
			{
				 LimparValidacao();
IList<AvaliacaoAluguel> RetornoAcao = new List<AvaliacaoAluguel>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarAvaliacaoAluguel();
				}
			}
				return RetornoAcao;
			}
			public void SalvarAvaliacaoAluguel (AvaliacaoAluguel itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioAvaliacaoAluguel(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarAvaliacaoAluguel(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarAvaliacaoAluguel_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirAvaliacaoAluguel (AvaliacaoAluguel itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoAvaliacaoAluguel(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirAvaliacaoAluguel(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirAvaliacaoAluguel_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public AvaliacaoAtracao SelecionarAvaliacaoAtracao (int? Identificador)
			{
				 LimparValidacao();
AvaliacaoAtracao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarAvaliacaoAtracao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<AvaliacaoAtracao> ListarAvaliacaoAtracao ()
			{
				 LimparValidacao();
IList<AvaliacaoAtracao> RetornoAcao = new List<AvaliacaoAtracao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarAvaliacaoAtracao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarAvaliacaoAtracao (AvaliacaoAtracao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioAvaliacaoAtracao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarAvaliacaoAtracao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarAvaliacaoAtracao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirAvaliacaoAtracao (AvaliacaoAtracao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoAvaliacaoAtracao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirAvaliacaoAtracao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirAvaliacaoAtracao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public CalendarioPrevisto SelecionarCalendarioPrevisto (int? Identificador)
			{
				 LimparValidacao();
CalendarioPrevisto RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarCalendarioPrevisto(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<CalendarioPrevisto> ListarCalendarioPrevisto ()
			{
				 LimparValidacao();
IList<CalendarioPrevisto> RetornoAcao = new List<CalendarioPrevisto>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarCalendarioPrevisto();
				}
			}
				return RetornoAcao;
			}
			public void SalvarCalendarioPrevisto (CalendarioPrevisto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioCalendarioPrevisto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarCalendarioPrevisto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarCalendarioPrevisto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirCalendarioPrevisto (CalendarioPrevisto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoCalendarioPrevisto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirCalendarioPrevisto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirCalendarioPrevisto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Carro SelecionarCarro (int? Identificador)
			{
				 LimparValidacao();
Carro RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarCarro(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Carro> ListarCarro ()
			{
				 LimparValidacao();
IList<Carro> RetornoAcao = new List<Carro>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarCarro();
				}
			}
				return RetornoAcao;
			}
			public void SalvarCarro (Carro itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioCarro(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarCarro(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarCarro_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirCarro (Carro itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoCarro(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirCarro(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirCarro_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public CarroEvento SelecionarCarroEvento (int? Identificador)
			{
				 LimparValidacao();
CarroEvento RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarCarroEvento(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<CarroEvento> ListarCarroEvento ()
			{
				 LimparValidacao();
IList<CarroEvento> RetornoAcao = new List<CarroEvento>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarCarroEvento();
				}
			}
				return RetornoAcao;
			}
			public void SalvarCarroEvento (CarroEvento itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioCarroEvento(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarCarroEvento(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarCarroEvento_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirCarroEvento (CarroEvento itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoCarroEvento(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirCarroEvento(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirCarroEvento_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Cidade SelecionarCidade (int? Identificador)
			{
				 LimparValidacao();
Cidade RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarCidade(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Cidade> ListarCidade ()
			{
				 LimparValidacao();
IList<Cidade> RetornoAcao = new List<Cidade>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarCidade();
				}
			}
				return RetornoAcao;
			}
			public void SalvarCidade (Cidade itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioCidade(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarCidade(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarCidade_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirCidade (Cidade itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoCidade(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirCidade(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirCidade_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public CidadeGrupo SelecionarCidadeGrupo (int? Identificador)
			{
				 LimparValidacao();
CidadeGrupo RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarCidadeGrupo(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<CidadeGrupo> ListarCidadeGrupo ()
			{
				 LimparValidacao();
IList<CidadeGrupo> RetornoAcao = new List<CidadeGrupo>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarCidadeGrupo();
				}
			}
				return RetornoAcao;
			}
			public void SalvarCidadeGrupo (CidadeGrupo itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioCidadeGrupo(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarCidadeGrupo(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarCidadeGrupo_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirCidadeGrupo (CidadeGrupo itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoCidadeGrupo(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirCidadeGrupo(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirCidadeGrupo_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Comentario SelecionarComentario (int? Identificador)
			{
				 LimparValidacao();
Comentario RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarComentario(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Comentario> ListarComentario ()
			{
				 LimparValidacao();
IList<Comentario> RetornoAcao = new List<Comentario>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarComentario();
				}
			}
				return RetornoAcao;
			}
			public void SalvarComentario (Comentario itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioComentario(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarComentario(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarComentario_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirComentario (Comentario itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoComentario(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirComentario(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirComentario_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public CotacaoMoeda SelecionarCotacaoMoeda (int? Identificador)
			{
				 LimparValidacao();
CotacaoMoeda RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarCotacaoMoeda(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<CotacaoMoeda> ListarCotacaoMoeda ()
			{
				 LimparValidacao();
IList<CotacaoMoeda> RetornoAcao = new List<CotacaoMoeda>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarCotacaoMoeda();
				}
			}
				return RetornoAcao;
			}
			public void SalvarCotacaoMoeda (CotacaoMoeda itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioCotacaoMoeda(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarCotacaoMoeda(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarCotacaoMoeda_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirCotacaoMoeda (CotacaoMoeda itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoCotacaoMoeda(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirCotacaoMoeda(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirCotacaoMoeda_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Foto SelecionarFoto (int? Identificador)
			{
				 LimparValidacao();
Foto RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarFoto(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Foto> ListarFoto ()
			{
				 LimparValidacao();
IList<Foto> RetornoAcao = new List<Foto>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarFoto();
				}
			}
				return RetornoAcao;
			}
			public void SalvarFoto (Foto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioFoto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarFoto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarFoto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirFoto (Foto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoFoto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirFoto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirFoto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public FotoAtracao SelecionarFotoAtracao (int? Identificador)
			{
				 LimparValidacao();
FotoAtracao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarFotoAtracao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<FotoAtracao> ListarFotoAtracao ()
			{
				 LimparValidacao();
IList<FotoAtracao> RetornoAcao = new List<FotoAtracao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarFotoAtracao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarFotoAtracao (FotoAtracao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioFotoAtracao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarFotoAtracao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarFotoAtracao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirFotoAtracao (FotoAtracao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoFotoAtracao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirFotoAtracao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirFotoAtracao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public FotoHotel SelecionarFotoHotel (int? Identificador)
			{
				 LimparValidacao();
FotoHotel RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarFotoHotel(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<FotoHotel> ListarFotoHotel ()
			{
				 LimparValidacao();
IList<FotoHotel> RetornoAcao = new List<FotoHotel>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarFotoHotel();
				}
			}
				return RetornoAcao;
			}
			public void SalvarFotoHotel (FotoHotel itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioFotoHotel(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarFotoHotel(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarFotoHotel_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirFotoHotel (FotoHotel itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoFotoHotel(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirFotoHotel(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirFotoHotel_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public FotoItemCompra SelecionarFotoItemCompra (int? Identificador)
			{
				 LimparValidacao();
FotoItemCompra RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarFotoItemCompra(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<FotoItemCompra> ListarFotoItemCompra ()
			{
				 LimparValidacao();
IList<FotoItemCompra> RetornoAcao = new List<FotoItemCompra>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarFotoItemCompra();
				}
			}
				return RetornoAcao;
			}
			public void SalvarFotoItemCompra (FotoItemCompra itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioFotoItemCompra(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarFotoItemCompra(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarFotoItemCompra_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirFotoItemCompra (FotoItemCompra itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoFotoItemCompra(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirFotoItemCompra(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirFotoItemCompra_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public FotoRefeicao SelecionarFotoRefeicao (int? Identificador)
			{
				 LimparValidacao();
FotoRefeicao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarFotoRefeicao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<FotoRefeicao> ListarFotoRefeicao ()
			{
				 LimparValidacao();
IList<FotoRefeicao> RetornoAcao = new List<FotoRefeicao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarFotoRefeicao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarFotoRefeicao (FotoRefeicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioFotoRefeicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarFotoRefeicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarFotoRefeicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirFotoRefeicao (FotoRefeicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoFotoRefeicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirFotoRefeicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirFotoRefeicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Gasto SelecionarGasto (int? Identificador)
			{
				 LimparValidacao();
Gasto RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarGasto(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Gasto> ListarGasto ()
			{
				 LimparValidacao();
IList<Gasto> RetornoAcao = new List<Gasto>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarGasto();
				}
			}
				return RetornoAcao;
			}
			public void SalvarGasto (Gasto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioGasto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarGasto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarGasto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirGasto (Gasto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoGasto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirGasto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirGasto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public GastoCompra SelecionarGastoCompra (int? Identificador)
			{
				 LimparValidacao();
GastoCompra RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarGastoCompra(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<GastoCompra> ListarGastoCompra ()
			{
				 LimparValidacao();
IList<GastoCompra> RetornoAcao = new List<GastoCompra>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarGastoCompra();
				}
			}
				return RetornoAcao;
			}
			public void SalvarGastoCompra (GastoCompra itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioGastoCompra(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarGastoCompra(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarGastoCompra_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirGastoCompra (GastoCompra itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoGastoCompra(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirGastoCompra(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirGastoCompra_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public GastoHotel SelecionarGastoHotel (int? Identificador)
			{
				 LimparValidacao();
GastoHotel RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarGastoHotel(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<GastoHotel> ListarGastoHotel ()
			{
				 LimparValidacao();
IList<GastoHotel> RetornoAcao = new List<GastoHotel>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarGastoHotel();
				}
			}
				return RetornoAcao;
			}
			public void SalvarGastoHotel (GastoHotel itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioGastoHotel(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarGastoHotel(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarGastoHotel_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirGastoHotel (GastoHotel itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoGastoHotel(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirGastoHotel(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirGastoHotel_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public GastoPosicao SelecionarGastoPosicao (int? Identificador)
			{
				 LimparValidacao();
GastoPosicao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarGastoPosicao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<GastoPosicao> ListarGastoPosicao ()
			{
				 LimparValidacao();
IList<GastoPosicao> RetornoAcao = new List<GastoPosicao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarGastoPosicao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarGastoPosicao (GastoPosicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioGastoPosicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarGastoPosicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarGastoPosicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirGastoPosicao (GastoPosicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoGastoPosicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirGastoPosicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirGastoPosicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public GastoRefeicao SelecionarGastoRefeicao (int? Identificador)
			{
				 LimparValidacao();
GastoRefeicao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarGastoRefeicao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<GastoRefeicao> ListarGastoRefeicao ()
			{
				 LimparValidacao();
IList<GastoRefeicao> RetornoAcao = new List<GastoRefeicao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarGastoRefeicao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarGastoRefeicao (GastoRefeicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioGastoRefeicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarGastoRefeicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarGastoRefeicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirGastoRefeicao (GastoRefeicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoGastoRefeicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirGastoRefeicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirGastoRefeicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public GastoViagemAerea SelecionarGastoViagemAerea (int? Identificador)
			{
				 LimparValidacao();
GastoViagemAerea RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarGastoViagemAerea(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<GastoViagemAerea> ListarGastoViagemAerea ()
			{
				 LimparValidacao();
IList<GastoViagemAerea> RetornoAcao = new List<GastoViagemAerea>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarGastoViagemAerea();
				}
			}
				return RetornoAcao;
			}
			public void SalvarGastoViagemAerea (GastoViagemAerea itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioGastoViagemAerea(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarGastoViagemAerea(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarGastoViagemAerea_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirGastoViagemAerea (GastoViagemAerea itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoGastoViagemAerea(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirGastoViagemAerea(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirGastoViagemAerea_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Hotel SelecionarHotel (int? Identificador)
			{
				 LimparValidacao();
Hotel RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarHotel(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Hotel> ListarHotel ()
			{
				 LimparValidacao();
IList<Hotel> RetornoAcao = new List<Hotel>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarHotel();
				}
			}
				return RetornoAcao;
			}
			public void SalvarHotel (Hotel itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioHotel(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarHotel(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarHotel_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirHotel (Hotel itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoHotel(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirHotel(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirHotel_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public HotelAvaliacao SelecionarHotelAvaliacao (int? Identificador)
			{
				 LimparValidacao();
HotelAvaliacao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarHotelAvaliacao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<HotelAvaliacao> ListarHotelAvaliacao ()
			{
				 LimparValidacao();
IList<HotelAvaliacao> RetornoAcao = new List<HotelAvaliacao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarHotelAvaliacao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarHotelAvaliacao (HotelAvaliacao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioHotelAvaliacao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarHotelAvaliacao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarHotelAvaliacao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirHotelAvaliacao (HotelAvaliacao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoHotelAvaliacao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirHotelAvaliacao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirHotelAvaliacao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public ItemCompra SelecionarItemCompra (int? Identificador)
			{
				 LimparValidacao();
ItemCompra RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarItemCompra(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<ItemCompra> ListarItemCompra ()
			{
				 LimparValidacao();
IList<ItemCompra> RetornoAcao = new List<ItemCompra>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarItemCompra();
				}
			}
				return RetornoAcao;
			}
			public void SalvarItemCompra (ItemCompra itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioItemCompra(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarItemCompra(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarItemCompra_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirItemCompra (ItemCompra itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoItemCompra(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirItemCompra(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirItemCompra_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public ListaCompra SelecionarListaCompra (int? Identificador)
			{
				 LimparValidacao();
ListaCompra RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarListaCompra(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<ListaCompra> ListarListaCompra ()
			{
				 LimparValidacao();
IList<ListaCompra> RetornoAcao = new List<ListaCompra>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarListaCompra();
				}
			}
				return RetornoAcao;
			}
			public void SalvarListaCompra (ListaCompra itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioListaCompra(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarListaCompra(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarListaCompra_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirListaCompra (ListaCompra itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoListaCompra(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirListaCompra(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirListaCompra_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Loja SelecionarLoja (int? Identificador)
			{
				 LimparValidacao();
Loja RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarLoja(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Loja> ListarLoja ()
			{
				 LimparValidacao();
IList<Loja> RetornoAcao = new List<Loja>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarLoja();
				}
			}
				return RetornoAcao;
			}
			public void SalvarLoja (Loja itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioLoja(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarLoja(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarLoja_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirLoja (Loja itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoLoja(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirLoja(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirLoja_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Pais SelecionarPais (int? Identificador)
			{
				 LimparValidacao();
Pais RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarPais(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Pais> ListarPais ()
			{
				 LimparValidacao();
IList<Pais> RetornoAcao = new List<Pais>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarPais();
				}
			}
				return RetornoAcao;
			}
			public void SalvarPais (Pais itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioPais(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarPais(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarPais_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirPais (Pais itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoPais(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirPais(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirPais_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public ParticipanteViagem SelecionarParticipanteViagem (int? Identificador)
			{
				 LimparValidacao();
ParticipanteViagem RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarParticipanteViagem(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<ParticipanteViagem> ListarParticipanteViagem ()
			{
				 LimparValidacao();
IList<ParticipanteViagem> RetornoAcao = new List<ParticipanteViagem>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarParticipanteViagem();
				}
			}
				return RetornoAcao;
			}
			public void SalvarParticipanteViagem (ParticipanteViagem itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioParticipanteViagem(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarParticipanteViagem(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarParticipanteViagem_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirParticipanteViagem (ParticipanteViagem itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoParticipanteViagem(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirParticipanteViagem(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirParticipanteViagem_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Posicao SelecionarPosicao (int? Identificador)
			{
				 LimparValidacao();
Posicao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarPosicao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Posicao> ListarPosicao ()
			{
				 LimparValidacao();
IList<Posicao> RetornoAcao = new List<Posicao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarPosicao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarPosicao (Posicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioPosicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarPosicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarPosicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirPosicao (Posicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoPosicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirPosicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirPosicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public ReabastecimentoGasto SelecionarReabastecimentoGasto (int? Identificador)
			{
				 LimparValidacao();
ReabastecimentoGasto RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarReabastecimentoGasto(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<ReabastecimentoGasto> ListarReabastecimentoGasto ()
			{
				 LimparValidacao();
IList<ReabastecimentoGasto> RetornoAcao = new List<ReabastecimentoGasto>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarReabastecimentoGasto();
				}
			}
				return RetornoAcao;
			}
			public void SalvarReabastecimentoGasto (ReabastecimentoGasto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioReabastecimentoGasto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarReabastecimentoGasto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarReabastecimentoGasto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirReabastecimentoGasto (ReabastecimentoGasto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoReabastecimentoGasto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirReabastecimentoGasto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirReabastecimentoGasto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Refeicao SelecionarRefeicao (int? Identificador)
			{
				 LimparValidacao();
Refeicao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarRefeicao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Refeicao> ListarRefeicao ()
			{
				 LimparValidacao();
IList<Refeicao> RetornoAcao = new List<Refeicao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarRefeicao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarRefeicao (Refeicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioRefeicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarRefeicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarRefeicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirRefeicao (Refeicao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoRefeicao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirRefeicao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirRefeicao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public RequisicaoAmizade SelecionarRequisicaoAmizade (int? Identificador)
			{
				 LimparValidacao();
RequisicaoAmizade RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarRequisicaoAmizade(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<RequisicaoAmizade> ListarRequisicaoAmizade ()
			{
				 LimparValidacao();
IList<RequisicaoAmizade> RetornoAcao = new List<RequisicaoAmizade>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarRequisicaoAmizade();
				}
			}
				return RetornoAcao;
			}
			public void SalvarRequisicaoAmizade (RequisicaoAmizade itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioRequisicaoAmizade(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarRequisicaoAmizade(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarRequisicaoAmizade_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirRequisicaoAmizade (RequisicaoAmizade itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoRequisicaoAmizade(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirRequisicaoAmizade(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirRequisicaoAmizade_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Sugestao SelecionarSugestao (int? Identificador)
			{
				 LimparValidacao();
Sugestao RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarSugestao(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Sugestao> ListarSugestao ()
			{
				 LimparValidacao();
IList<Sugestao> RetornoAcao = new List<Sugestao>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarSugestao();
				}
			}
				return RetornoAcao;
			}
			public void SalvarSugestao (Sugestao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioSugestao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarSugestao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarSugestao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirSugestao (Sugestao itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoSugestao(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirSugestao(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirSugestao_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Usuario SelecionarUsuario (int? Identificador)
			{
				 LimparValidacao();
Usuario RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarUsuario(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Usuario> ListarUsuario ()
			{
				 LimparValidacao();
IList<Usuario> RetornoAcao = new List<Usuario>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarUsuario();
				}
			}
				return RetornoAcao;
			}
			public void SalvarUsuario (Usuario itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioUsuario(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarUsuario(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarUsuario_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirUsuario (Usuario itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoUsuario(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirUsuario(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirUsuario_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public Viagem SelecionarViagem (int? Identificador)
			{
				 LimparValidacao();
Viagem RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarViagem(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<Viagem> ListarViagem ()
			{
				 LimparValidacao();
IList<Viagem> RetornoAcao = new List<Viagem>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarViagem();
				}
			}
				return RetornoAcao;
			}
			public void SalvarViagem (Viagem itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioViagem(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarViagem(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarViagem_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirViagem (Viagem itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoViagem(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirViagem(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirViagem_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public ViagemAerea SelecionarViagemAerea (int? Identificador)
			{
				 LimparValidacao();
ViagemAerea RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarViagemAerea(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<ViagemAerea> ListarViagemAerea ()
			{
				 LimparValidacao();
IList<ViagemAerea> RetornoAcao = new List<ViagemAerea>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarViagemAerea();
				}
			}
				return RetornoAcao;
			}
			public void SalvarViagemAerea (ViagemAerea itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioViagemAerea(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarViagemAerea(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarViagemAerea_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirViagemAerea (ViagemAerea itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoViagemAerea(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirViagemAerea(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirViagemAerea_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public ViagemAereaAeroporto SelecionarViagemAereaAeroporto (int? Identificador)
			{
				 LimparValidacao();
ViagemAereaAeroporto RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarViagemAereaAeroporto(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<ViagemAereaAeroporto> ListarViagemAereaAeroporto ()
			{
				 LimparValidacao();
IList<ViagemAereaAeroporto> RetornoAcao = new List<ViagemAereaAeroporto>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarViagemAereaAeroporto();
				}
			}
				return RetornoAcao;
			}
			public void SalvarViagemAereaAeroporto (ViagemAereaAeroporto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioViagemAereaAeroporto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarViagemAereaAeroporto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarViagemAereaAeroporto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirViagemAereaAeroporto (ViagemAereaAeroporto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoViagemAereaAeroporto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirViagemAereaAeroporto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirViagemAereaAeroporto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public RefeicaoPedido SelecionarRefeicaoPedido (int? Identificador)
			{
				 LimparValidacao();
RefeicaoPedido RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarRefeicaoPedido(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<RefeicaoPedido> ListarRefeicaoPedido ()
			{
				 LimparValidacao();
IList<RefeicaoPedido> RetornoAcao = new List<RefeicaoPedido>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarRefeicaoPedido();
				}
			}
				return RetornoAcao;
			}
			public void SalvarRefeicaoPedido (RefeicaoPedido itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioRefeicaoPedido(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarRefeicaoPedido(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarRefeicaoPedido_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirRefeicaoPedido (RefeicaoPedido itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoRefeicaoPedido(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirRefeicaoPedido(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirRefeicaoPedido_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public AvaliacaoLoja SelecionarAvaliacaoLoja (int? Identificador)
			{
				 LimparValidacao();
AvaliacaoLoja RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarAvaliacaoLoja(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<AvaliacaoLoja> ListarAvaliacaoLoja ()
			{
				 LimparValidacao();
IList<AvaliacaoLoja> RetornoAcao = new List<AvaliacaoLoja>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarAvaliacaoLoja();
				}
			}
				return RetornoAcao;
			}
			public void SalvarAvaliacaoLoja (AvaliacaoLoja itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioAvaliacaoLoja(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarAvaliacaoLoja(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarAvaliacaoLoja_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirAvaliacaoLoja (AvaliacaoLoja itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoAvaliacaoLoja(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirAvaliacaoLoja(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirAvaliacaoLoja_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public UsuarioGasto SelecionarUsuarioGasto (string Identificador)
			{
				 LimparValidacao();
UsuarioGasto RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarUsuarioGasto(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<UsuarioGasto> ListarUsuarioGasto ()
			{
				 LimparValidacao();
IList<UsuarioGasto> RetornoAcao = new List<UsuarioGasto>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarUsuarioGasto();
				}
			}
				return RetornoAcao;
			}
			public void SalvarUsuarioGasto (UsuarioGasto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioUsuarioGasto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarUsuarioGasto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarUsuarioGasto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public void ExcluirUsuarioGasto (UsuarioGasto itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoUsuarioGasto(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirUsuarioGasto(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirUsuarioGasto_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
			public HotelEvento SelecionarHotelEvento (int? Identificador)
			{
				 LimparValidacao();
HotelEvento RetornoAcao = null;
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.SelecionarHotelEvento(Identificador);
				}
			}
				return RetornoAcao;
			}
			public IList<HotelEvento> ListarHotelEvento ()
			{
				 LimparValidacao();
IList<HotelEvento> RetornoAcao = new List<HotelEvento>();
				if (IsValid())
				{
using(					ViagemRepository data = new ViagemRepository())
				 {
					RetornoAcao = data.ListarHotelEvento();
				}
			}
				return RetornoAcao;
			}
			public void SalvarHotelEvento (HotelEvento itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasNegocioHotelEvento(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.SalvarHotelEvento(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarHotelEvento_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}

        

        public void ExcluirHotelEvento (HotelEvento itemGravar)
			{
				 LimparValidacao();
				 ValidateService(itemGravar);
					ValidarRegrasExclusaoHotelEvento(itemGravar);
				 if (IsValid())
				 {
					using(ViagemRepository data = new ViagemRepository())
				 {
					data.ExcluirHotelEvento(itemGravar);
					Message msg = new Message();
					msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirHotelEvento_OK") });
					ServiceResult resultado = new ServiceResult();
					resultado.Success = true;
					resultado.Messages.Add(msg);
					serviceResult.Add(resultado);
				 }
				 }
			}
       
    }

}
