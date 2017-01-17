using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV.Model;

namespace CV.Data
{
    public partial class ConsultaRepository : RepositoryBase    
    {
        public List<ExtratoMoeda> ConsultarExtratoMoeda(int? IdentificadorUsuario,int? IdentificadorViagem, int? Moeda, DateTime DataInicio)
        {
            var queryInicial = this.Context.AporteDinheiros.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d=>!d.DataExclusao.HasValue).Where(d=>d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Moeda == Moeda).Where(d => d.DataAporte < DataInicio).Select(d => new { Data = d.DataAporte.Value, Valor = d.Valor.Value });
            queryInicial = queryInicial.Union(this.Context.Gastos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Moeda == Moeda).Where(d => d.Data < DataInicio).Where(d => d.Especie.Value).Select(d => new  { Data = d.Data.Value, Valor = d.Valor.Value * -1}));

            var queryAtual = this.Context.AporteDinheiros.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Moeda == Moeda).Where(d => d.DataAporte >= DataInicio).Select(d => new { Data = d.DataAporte.Value, Valor = d.Valor.Value, Descricao="Aporte Moeda" });
            queryAtual = queryAtual.Union(this.Context.Gastos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Moeda == Moeda).Where(d => d.Data >= DataInicio).Where(d => d.Especie.Value).Select(d => new { Data = d.Data.Value, Valor = d.Valor.Value * -1, Descricao=d.Descricao }));

            decimal ValorInicial = 0;
            if (queryInicial.Count()     > 0)
                ValorInicial = queryInicial.Sum(d => d.Valor);
            List<ExtratoMoeda> Lista = new List<ExtratoMoeda>();
            Lista.Add(new ExtratoMoeda() { Data = DataInicio, TipoLinha = "S", Descricao = "Saldo Inicial", Valor = ValorInicial });
            Lista.AddRange(queryAtual.Select(d => new ExtratoMoeda() { Data = d.Data, Descricao = d.Descricao, TipoLinha = "V", Valor = d.Valor }));
            decimal ValorAnterior = ValorInicial;
            DateTime UltimaData = DataInicio;
            var ListaGrupo = Lista.Where(d => d.TipoLinha == "V").GroupBy(d => d.Data.Date);
            foreach (var itemGrupo in ListaGrupo)
            {
                ValorAnterior = ValorAnterior + itemGrupo.Sum(e => e.Valor);
                UltimaData = itemGrupo.Key;
                Lista.Add(new ExtratoMoeda() { Data = itemGrupo.Key.AddDays(1).AddMilliseconds(-1), Descricao = (UltimaData==DateTime.Today?"Saldo Atual":"Saldo Dia"), TipoLinha = "S", Valor = ValorAnterior });
            }
            if (UltimaData != DateTime.Today)
                Lista.Add(new ExtratoMoeda() { Data = DateTime.Today.AddDays(1).AddMilliseconds(-1), Descricao =  "Saldo Atual" , TipoLinha = "S", Valor = ValorAnterior });

            return Lista.OrderByDescending(d => d.Data).ToList();

        }
    }
}
