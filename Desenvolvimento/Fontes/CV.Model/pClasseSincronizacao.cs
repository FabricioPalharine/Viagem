using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class ClasseSincronizacao
    {
        public Viagem ItemViagem { get; set; }
        public List<Cidade> CidadesAtracao { get; set; }
        public List<Cidade> CidadesHotel { get; set; }
        public List<Cidade> CidadesRefeicao { get; set; }
        public List<Cidade> CidadesSugestao{ get; set; }
        public List<Cidade> CidadesComentario { get; set; }
        public List<Cidade> CidadesViagemAerea { get; set; }
        public List<Cidade> CidadesLoja { get; set; }
        public List<CotacaoMoeda> CotacoesMoeda { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AporteDinheiro> AportesDinheiro { get; set; }
        public List<Gasto> Gastos { get; set; }
        public List<Sugestao> Sugestoes { get; set; }
        public List<CalendarioPrevisto> CalendariosPrevistos { get; set; }
        public List<ListaCompra> ListaCompra { get; set; }
        public List<Atracao> Atracoes { get; set; }
        public List<Hotel> Hoteis { get; set; }
        public List<Refeicao> Refeicoes { get; set; }
        public List<Loja> Lojas { get; set; }
        public List<Carro> Carros { get; set; }
        public List<ViagemAerea> Deslocamentos { get; set; }
        public List<GastoCompra> Compras { get; set; }
        public List<Reabastecimento> Reabastecimento { get; set; }
        public List<HotelEvento> EventosHotel { get; set; }
        public List<CarroDeslocamento> CarroDeslocamentos { get; set; }

        public List<ItemCompra> ItensComprados { get; set; }
        public List<AluguelGasto> GastosCarro { get; set; }
        public List<GastoAtracao> GastosAtracao { get; set; }
        public List<GastoHotel> GastosHotel { get; set; }
        public List<GastoRefeicao> GastosRefeicao { get; set; }
        public List<GastoViagemAerea> GastosDeslocamento { get; set; }

        public List<Amigo> Amigos { get; set; }

        public List<Posicao> Posicoes { get; set; }

    }
}
