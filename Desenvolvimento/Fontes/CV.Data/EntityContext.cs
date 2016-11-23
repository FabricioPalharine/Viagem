using System;
                                    using System.Collections.Generic;
                                    using System.Linq;
                                    using System.Text;
                                    using System.Data.Entity;
using CV.Model;
      using CV.Data.Configuration;

      namespace CV.Data
      {
	      public class EntityContext : DbContext, IDisposable
	      {
		      public EntityContext(string connectionName)
			      : base(connectionName)
		      {
			      this.Configuration.AutoDetectChangesEnabled = true;
			      this.Configuration.LazyLoadingEnabled = true;
			      this.Configuration.ProxyCreationEnabled = true;
			      this.Configuration.ValidateOnSaveEnabled = false;			
      			
		      }
      
		public DbSet<AluguelGasto> AluguelGastos { get; set; }
		public DbSet<Reabastecimento> Reabastecimentos { get; set; }
		public DbSet<Amigo> Amigos { get; set; }
		public DbSet<AporteDinheiro> AporteDinheiros { get; set; }
		public DbSet<Atracao> Atracoes { get; set; }
		public DbSet<AvaliacaoAerea> AvaliacaoAereas { get; set; }
		public DbSet<AvaliacaoAluguel> AvaliacaoAlugueis { get; set; }
		public DbSet<AvaliacaoAtracao> AvaliacaoAtracoes { get; set; }
		public DbSet<CalendarioPrevisto> CalendarioPrevistos { get; set; }
		public DbSet<Carro> Carros { get; set; }
		public DbSet<CarroEvento> CarroEventos { get; set; }
		public DbSet<Cidade> Cidades { get; set; }
		public DbSet<CidadeGrupo> CidadeGrupos { get; set; }
		public DbSet<Comentario> Comentarios { get; set; }
		public DbSet<CotacaoMoeda> CotacaoMoedas { get; set; }
		public DbSet<Foto> Fotos { get; set; }
		public DbSet<FotoAtracao> FotoAtracoes { get; set; }
		public DbSet<FotoHotel> FotoHoteis { get; set; }
		public DbSet<FotoItemCompra> FotoItemCompras { get; set; }
		public DbSet<FotoRefeicao> FotoRefeicoes { get; set; }
		public DbSet<Gasto> Gastos { get; set; }
		public DbSet<GastoCompra> GastoCompras { get; set; }
		public DbSet<GastoHotel> GastoHoteis { get; set; }
		public DbSet<GastoRefeicao> GastoRefeicoes { get; set; }
		public DbSet<GastoViagemAerea> GastoViagemAereas { get; set; }
		public DbSet<Hotel> Hoteis { get; set; }
		public DbSet<HotelAvaliacao> HotelAvaliacoes { get; set; }
		public DbSet<ItemCompra> ItemCompras { get; set; }
		public DbSet<ListaCompra> ListaCompras { get; set; }
		public DbSet<Loja> Lojas { get; set; }
		public DbSet<Pais> Paises { get; set; }
		public DbSet<ParticipanteViagem> ParticipanteViagemes { get; set; }
		public DbSet<Posicao> Posicoes { get; set; }
		public DbSet<ReabastecimentoGasto> ReabastecimentoGastos { get; set; }
		public DbSet<Refeicao> Refeicoes { get; set; }
		public DbSet<RequisicaoAmizade> RequisicaoAmizades { get; set; }
		public DbSet<Sugestao> Sugestoes { get; set; }
		public DbSet<Usuario> Usuarios { get; set; }
		public DbSet<Viagem> Viagemes { get; set; }
		public DbSet<ViagemAerea> ViagemAereas { get; set; }
		public DbSet<ViagemAereaAeroporto> ViagemAereaAeroportos { get; set; }
		public DbSet<RefeicaoPedido> RefeicaoPedidos { get; set; }
		public DbSet<AvaliacaoLoja> AvaliacaoLojas { get; set; }
		public DbSet<UsuarioGasto> UsuarioGastos { get; set; }
		public DbSet<HotelEvento> HotelEventos { get; set; }
		public DbSet<GastoAtracao> GastoAtracoes { get; set; }
		public DbSet<GastoDividido> GastoDivididos { get; set; }
		public DbSet<CarroDeslocamento> CarroDeslocamentos { get; set; }
		public DbSet<CarroDeslocamentoUsuario> CarroDeslocamentoUsuarios { get; set; }
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{

			Database.SetInitializer<EntityContext>(null);

			modelBuilder.Configurations.Add(new AluguelGastoConfiguration());
			modelBuilder.Configurations.Add(new ReabastecimentoConfiguration());
			modelBuilder.Configurations.Add(new AmigoConfiguration());
			modelBuilder.Configurations.Add(new AporteDinheiroConfiguration());
			modelBuilder.Configurations.Add(new AtracaoConfiguration());
			modelBuilder.Configurations.Add(new AvaliacaoAereaConfiguration());
			modelBuilder.Configurations.Add(new AvaliacaoAluguelConfiguration());
			modelBuilder.Configurations.Add(new AvaliacaoAtracaoConfiguration());
			modelBuilder.Configurations.Add(new CalendarioPrevistoConfiguration());
			modelBuilder.Configurations.Add(new CarroConfiguration());
			modelBuilder.Configurations.Add(new CarroEventoConfiguration());
			modelBuilder.Configurations.Add(new CidadeConfiguration());
			modelBuilder.Configurations.Add(new CidadeGrupoConfiguration());
			modelBuilder.Configurations.Add(new ComentarioConfiguration());
			modelBuilder.Configurations.Add(new CotacaoMoedaConfiguration());
			modelBuilder.Configurations.Add(new FotoConfiguration());
			modelBuilder.Configurations.Add(new FotoAtracaoConfiguration());
			modelBuilder.Configurations.Add(new FotoHotelConfiguration());
			modelBuilder.Configurations.Add(new FotoItemCompraConfiguration());
			modelBuilder.Configurations.Add(new FotoRefeicaoConfiguration());
			modelBuilder.Configurations.Add(new GastoConfiguration());
			modelBuilder.Configurations.Add(new GastoCompraConfiguration());
			modelBuilder.Configurations.Add(new GastoHotelConfiguration());
			modelBuilder.Configurations.Add(new GastoRefeicaoConfiguration());
			modelBuilder.Configurations.Add(new GastoViagemAereaConfiguration());
			modelBuilder.Configurations.Add(new HotelConfiguration());
			modelBuilder.Configurations.Add(new HotelAvaliacaoConfiguration());
			modelBuilder.Configurations.Add(new ItemCompraConfiguration());
			modelBuilder.Configurations.Add(new ListaCompraConfiguration());
			modelBuilder.Configurations.Add(new LojaConfiguration());
			modelBuilder.Configurations.Add(new PaisConfiguration());
			modelBuilder.Configurations.Add(new ParticipanteViagemConfiguration());
			modelBuilder.Configurations.Add(new PosicaoConfiguration());
			modelBuilder.Configurations.Add(new ReabastecimentoGastoConfiguration());
			modelBuilder.Configurations.Add(new RefeicaoConfiguration());
			modelBuilder.Configurations.Add(new RequisicaoAmizadeConfiguration());
			modelBuilder.Configurations.Add(new SugestaoConfiguration());
			modelBuilder.Configurations.Add(new UsuarioConfiguration());
			modelBuilder.Configurations.Add(new ViagemConfiguration());
			modelBuilder.Configurations.Add(new ViagemAereaConfiguration());
			modelBuilder.Configurations.Add(new ViagemAereaAeroportoConfiguration());
			modelBuilder.Configurations.Add(new RefeicaoPedidoConfiguration());
			modelBuilder.Configurations.Add(new AvaliacaoLojaConfiguration());
			modelBuilder.Configurations.Add(new UsuarioGastoConfiguration());
			modelBuilder.Configurations.Add(new HotelEventoConfiguration());
			modelBuilder.Configurations.Add(new GastoAtracaoConfiguration());
			modelBuilder.Configurations.Add(new GastoDivididoConfiguration());
			modelBuilder.Configurations.Add(new CarroDeslocamentoConfiguration());
			modelBuilder.Configurations.Add(new CarroDeslocamentoUsuarioConfiguration());
			base.OnModelCreating(modelBuilder);
		}		
	}
}
