using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MvvmHelpers;
using CV.Mobile.Models;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CV.Mobile.Controls
{
    public class CalendarioControl : ContentView
    {

        public static readonly BindableProperty AgendaSelectedCommandProperty =
           BindableProperty.Create(nameof(AgendaSelectedCommand), typeof(Command<int?>), typeof(CalendarioControl), null);


        public static readonly BindableProperty TrocarDataCommandProperty =
         BindableProperty.Create(nameof(TrocarDataCommand), typeof(Command<DateTime>), typeof(CalendarioControl), null);


        public static readonly BindableProperty DataCalendarioProperty = BindableProperty.Create
            (nameof(DataCalendario), typeof(DateTime), typeof(CalendarioControl), default(DateTime), BindingMode.TwoWay,
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var page = bindable as CalendarioControl;
                var newValue = newvalue == null ? DateTime.Today : (DateTime)newvalue;

                page.RenderizarComponente();

            });

        public Command<int?> AgendaSelectedCommand
        {
            get { return (Command<int?>)this.GetValue(AgendaSelectedCommandProperty); }
            set { this.SetValue(AgendaSelectedCommandProperty, value); }
        }

        public Command<DateTime> TrocarDataCommand
        {
            get { return (Command<DateTime>)this.GetValue(TrocarDataCommandProperty); }
            set { this.SetValue(TrocarDataCommandProperty, value); }
        }

        public static readonly BindableProperty AgendamentosProperty = BindableProperty.Create
           (nameof(Agendamentos), typeof(ObservableRangeCollection<CalendarioPrevisto>), typeof(CalendarioControl), new ObservableRangeCollection<CalendarioPrevisto>(), BindingMode.OneWay,
           propertyChanged: (bindable, oldvalue, newvalue) =>
           {
               var page = bindable as CalendarioControl;
               var lista = newvalue as ObservableRangeCollection<CalendarioPrevisto>;
               if (lista != null)
                lista.CollectionChanged += page.Lista_CollectionChanged;
               page.RenderizarComponente();


           });

        private void Lista_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    var itemCalendario = (CalendarioPrevisto)item;
                    if (itemCalendario.DataInicio <= DataCalendario && itemCalendario.DataFim >= DataCalendario)
                        RenderizarComponente();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    var itemCalendario = (CalendarioPrevisto)item;
                    if (itemCalendario.DataInicio <= DataCalendario && itemCalendario.DataFim >= DataCalendario)
                        RenderizarComponente();

                }
            }
        }

        public DateTime DataCalendario
        {
            get { return (DateTime)this.GetValue(DataCalendarioProperty); }
            set { this.SetValue(DataCalendarioProperty, value); }
        }

        public ObservableRangeCollection<CalendarioPrevisto> Agendamentos
        {
            get { return (ObservableRangeCollection<CalendarioPrevisto>)this.GetValue(AgendamentosProperty); }
            set { this.SetValue(AgendamentosProperty, value); }

        }

        private Grid _grid = new Grid();
        private List<GestureInterest> Interests = new List<GestureInterest>();
        public CalendarioControl()
        {

            GestureInterest gi = new GestureInterest();
            gi.GestureType = GestureType.Swipe;
            gi.Direction = Directionality.Left;
            gi.GestureCommand = new RelayGesture(OnGesture);
            Interests.Add(gi);
            gi = new GestureInterest();
            gi.GestureType = GestureType.Swipe;
            gi.Direction = Directionality.Right;
            gi.GestureCommand = new RelayGesture(OnGesture);
            Interests.Add(gi);
            //RegisterInterests(_grid, Interests);
            this.Init();
        }

        private void OnGesture(GestureResult gr, object obj)
        {
            switch (gr.Direction)
            {
                case Directionality.Left:
                    DataCalendario = DataCalendario.AddDays(-1);
                    break;
                case Directionality.Right:
                    DataCalendario = DataCalendario.AddDays(1);
                    break;
            }
        }

        private async void Init()
        {
            await System.Threading.Tasks.Task.Delay(500);
            if (Agendamentos != null)
            {
                Agendamentos.CollectionChanged -= Lista_CollectionChanged;
                Agendamentos.CollectionChanged += Lista_CollectionChanged;
            }
            this.Content = _grid;
            _grid.RowSpacing = 0;
            RenderizarComponente();
        }

        private async void RenderizarComponente()
        {
            await Task.Delay(200);
            _grid.ColumnDefinitions = new ColumnDefinitionCollection();
            _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80, GridUnitType.Absolute) });
            _grid.RowDefinitions = new RowDefinitionCollection();
            for (int i = 0; i <= 23; i++)
                _grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
            _grid.Children.Clear();
            List<GrupoHorario> Grupos = new List<GrupoHorario>();
           if (Agendamentos != null)
            {
                foreach (var itemAgendamento in Agendamentos.Where(d=>(d.DataInicio.GetValueOrDefault().Date <= DataCalendario && d.DataFim.GetValueOrDefault().Date >= DataCalendario) ))
                {
                    GrupoHorario itemGrupo = new GrupoHorario()
                    {
                        LinhaInicial = itemAgendamento.DataInicio.GetValueOrDefault().Date < DataCalendario.Date ? 0 : itemAgendamento.DataInicio.GetValueOrDefault().Hour,
                        LinhaFinal = itemAgendamento.DataFim.GetValueOrDefault().Date > DataCalendario.Date ? 23 : itemAgendamento.DataFim.GetValueOrDefault().Hour,
                        Identificador = itemAgendamento.Identificador.GetValueOrDefault(),
                        Nome = itemAgendamento.Nome,
                        Prioridade = itemAgendamento.Prioridade
                    };
                    if (itemGrupo.LinhaFinal < itemGrupo.LinhaInicial)
                        itemGrupo.LinhaFinal = itemGrupo.LinhaInicial;
                    var ItensHorario = Grupos.Where(d => ( d.LinhaFinal >= itemGrupo.LinhaInicial) ).OrderBy(d => d.Coluna);
                    if (ItensHorario.Any())
                    {
                        for (int i = 1; i <= ItensHorario.Max(d => d.Coluna) + 1; i++)
                        {
                            if (!ItensHorario.Where(d => d.Coluna == i).Any())
                            {
                                itemGrupo.Coluna = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        itemGrupo.Coluna = 1;
                    }
                    Grupos.Add(itemGrupo);
                }
            }
            int TotalColunas = Grupos.Any() ? Grupos.Max(d => d.Coluna) : 1;
            for (int i=1;i<=TotalColunas;i++)
                _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            for (int i = 0;i<=23;i++)
            {
                if (i%2 == 1)
                {
                    StackLayout bv = new StackLayout() { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.FromRgb(200, 200, 200) };
                   
                    _grid.Children.Add(bv, 0, i);                    
                    Grid.SetColumnSpan(bv, TotalColunas + 1);
                  
                }
                Label lbl = new Label() { VerticalOptions = LayoutOptions.CenterAndExpand};

                lbl.Text = String.Concat(i.ToString("00"));
                _grid.Children.Add(lbl, 0, i);
            }
            foreach (var itemGrupo in Grupos)
            {
                BoxView bv = new BoxView() { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, BackgroundColor = itemGrupo.Prioridade==1?Color.Green: itemGrupo.Prioridade==2?Color.Blue: Color.Red};
                _grid.Children.Add(bv, itemGrupo.Coluna, itemGrupo.LinhaInicial);
                TapGestureRecognizer objTapGestureRecognizer = new TapGestureRecognizer();
                objTapGestureRecognizer.CommandParameter = itemGrupo.Identificador;
                objTapGestureRecognizer.Tapped += ObjTapGestureRecognizer_Tapped;
                bv.GestureRecognizers.Add(objTapGestureRecognizer);
              //  RegisterInterests(bv, RetornarInterest());
                Grid.SetRowSpan(bv, itemGrupo.LinhaInicial > itemGrupo.LinhaFinal ? 1 : (itemGrupo.LinhaFinal-itemGrupo.LinhaInicial) + 1);

                Label lbl = new Label() { VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.StartAndExpand, TextColor = Color.White};
                lbl.Text = itemGrupo.Nome;
                lbl.GestureRecognizers.Add(objTapGestureRecognizer);
           //     RegisterInterests(lbl, RetornarInterest());
                _grid.Children.Add(lbl, itemGrupo.Coluna, itemGrupo.LinhaInicial);

            }
        }

        private IEnumerable<GestureInterest> RetornarInterest()
        {
            List<GestureInterest> lista = new List<GestureInterest>();
            GestureInterest gi = new GestureInterest();
            gi.GestureType = GestureType.Swipe;
            gi.Direction = Directionality.Left;
            gi.GestureCommand = new RelayGesture(OnGesture);
            lista.Add(gi);
            gi = new GestureInterest();
            gi.GestureType = GestureType.Swipe;
            gi.Direction = Directionality.Right;
            gi.GestureCommand = new RelayGesture(OnGesture);
            lista.Add(gi);
            return lista;
        }

        private void ObjTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TappedEventArgs tap = (TappedEventArgs)e;

            if (this.AgendaSelectedCommand != null && this.AgendaSelectedCommand.CanExecute(this))
            {
                this.AgendaSelectedCommand.Execute(Convert.ToInt32(tap.Parameter));
            }
        }

        internal class GrupoHorario
        {
            internal int LinhaInicial { get; set; }
            internal int LinhaFinal { get; set; }
            internal int Coluna { get; set; }
            internal int? Identificador { get; set; }
            internal string Nome { get; set; }
            internal int? Prioridade { get; set; }
        }
    }
}
