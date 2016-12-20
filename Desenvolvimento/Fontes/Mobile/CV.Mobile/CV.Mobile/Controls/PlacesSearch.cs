using CV.Mobile.Models;
using CV.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using TK.CustomMap;
using TK.CustomMap.Api;
using TK.CustomMap.Api.Google;
using TK.CustomMap.Api.OSM;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.Views
{
    public class PlacesSearch : RelativeLayout
    {
        public static readonly BindableProperty BoundsProperty = BindableProperty.Create
            (nameof(Bounds), typeof(MapSpan), typeof(PlacesSearch), default(MapSpan));

        

        private readonly bool _useSearchBar;

        private bool _textChangeItemSelected;

        private SearchBar _searchBar;
        private Entry _entry;
        private ListView _autoCompleteListView;

        private IEnumerable<GmsSearchResults> _predictions;


        public static readonly BindableProperty PlaceSelectedCommandProperty =
            BindableProperty.Create(nameof(PlaceSelectedCommand), typeof(Command<GmsSearchResults>), typeof(PlacesSearch), null);

        public static readonly BindableProperty TiposProperty =
      BindableProperty.Create(nameof(Tipos), typeof(string), typeof(PlacesSearch), string.Empty);

        public Command<GmsSearchResults> PlaceSelectedCommand
        {
            get { return (Command<GmsSearchResults>)this.GetValue(PlaceSelectedCommandProperty); }
            set { this.SetValue(PlaceSelectedCommandProperty, value); }
        }
        public double HeightOfSearchBar
        {
            get
            {
                return this._useSearchBar ? this._searchBar.Height : this._entry.Height;
            }
        }
        private string SearchText
        {
            get
            {
                return this._useSearchBar ? this._searchBar.Text : this._entry.Text;
            }
            set
            {
                if (this._useSearchBar)
                    this._searchBar.Text = value;
                else
                    this._entry.Text = value;
            }
        }
        public new MapSpan Bounds
        {
            get { return (MapSpan)this.GetValue(BoundsProperty); }
            set { this.SetValue(BoundsProperty, value); }
        }

        public string Tipos
        {
            get { return (string)this.GetValue(TiposProperty); }
            set { this.SetValue(TiposProperty, value); }

        }
        public PlacesSearch(bool useSearchBar)
        {
            this._useSearchBar = useSearchBar;
            this.Init();
        }

        public string Placeholder
        {
            get { return this._useSearchBar ? this._searchBar.Placeholder : this._entry.Placeholder; }
            set
            {
                if (this._useSearchBar)
                    this._searchBar.Placeholder = value;
                else
                    this._entry.Placeholder = value;


            }
        }
        public PlacesSearch()
        {
            this._useSearchBar = false;
            this.Init();
        }
        private void Init()
        {

            OsmNominatim.Instance.CountryCodes.Add("br");


            this._autoCompleteListView = new ListView
            {
                IsVisible = false,
                RowHeight = 40,
                HeightRequest = 0,
                BackgroundColor = Color.White
            };
            this._autoCompleteListView.ItemTemplate = new DataTemplate(() =>
            {
                var cell = new TextCell();
                cell.SetBinding(ImageCell.TextProperty, "Description");

                return cell;
            });

            View searchView;
            if (this._useSearchBar)
            {
                this._searchBar = new SearchBar
                {
                    Placeholder = "Search for address..."
                };
                this._searchBar.TextChanged += SearchTextChanged;
                this._searchBar.SearchButtonPressed += SearchButtonPressed;

                searchView = this._searchBar;

            }
            else
            {
                this._entry = new Entry
                {
                    Placeholder = "Sarch for address"
                };
                this._entry.TextChanged += SearchTextChanged;

                searchView = this._entry;
            }
            this.Children.Add(searchView,
                Constraint.Constant(0),
                Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent(l => l.Width));

            this.Children.Add(
                this._autoCompleteListView,
                Constraint.Constant(0),
                Constraint.RelativeToView(searchView, (r, v) => v.Y + v.Height));

            this._autoCompleteListView.ItemSelected += ItemSelected;

            this._textChangeItemSelected = false;
        }

        private void SearchButtonPressed(object sender, EventArgs e)
        {
            if (this._predictions != null && this._predictions.Any())
                this.HandleItemSelected(this._predictions.First());
            else
                this.Reset();
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._textChangeItemSelected)
            {
                this._textChangeItemSelected = false;
                return;
            }

            this.SearchPlaces();
        }

        private async void SearchPlaces()
        {
            try
            {
                if (string.IsNullOrEmpty(this.SearchText))
                {
                    this._autoCompleteListView.ItemsSource = null;
                    this._autoCompleteListView.IsVisible = false;
                    this._autoCompleteListView.HeightRequest = 0;
                    return;
                }

                IEnumerable<GmsSearchResults> result = null;

                using (GoogleServices srv = new GoogleServices())
                {
                    var apiResult = await srv.GetPlaces(this.SearchText, Bounds.Center, this.Tipos.Split('|'), Convert.ToInt32(Bounds.Radius.Meters > 50000 ? 50000d : Bounds.Radius.Meters));

                    if (apiResult != null)
                        result = apiResult.results;

                }

                if (result != null && result.Any())
                {
                    this._predictions = result;

                    this._autoCompleteListView.HeightRequest = result.Count() * 40;
                    this._autoCompleteListView.IsVisible = true;
                    this._autoCompleteListView.ItemsSource = this._predictions;
                }
                else
                {
                    this._autoCompleteListView.HeightRequest = 0;
                    this._autoCompleteListView.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                // TODO
            }
        }
        private void ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            var prediction = (GmsSearchResults)e.SelectedItem;

            this.HandleItemSelected(prediction);
        }

        private void HandleItemSelected(GmsSearchResults prediction)
        {
            if (this.PlaceSelectedCommand != null && this.PlaceSelectedCommand.CanExecute(this))
            {
                this.PlaceSelectedCommand.Execute(prediction);
            }

            this._textChangeItemSelected = true;

            this.SearchText = prediction.name;
            this._autoCompleteListView.SelectedItem = null;

            this.Reset();
        }
        private void Reset()
        {
            this._autoCompleteListView.ItemsSource = null;
            this._autoCompleteListView.IsVisible = false;
            this._autoCompleteListView.HeightRequest = 0;

            if (this._useSearchBar)
                this._searchBar.Unfocus();
            else
                this._entry.Unfocus();
        }
    }

}


