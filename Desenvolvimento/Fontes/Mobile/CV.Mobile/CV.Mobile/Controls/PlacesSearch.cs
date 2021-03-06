﻿using CV.Mobile.Models;
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

namespace CV.Mobile.Controls
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
        private StackLayout _stackLayout;

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

        public static readonly BindableProperty TextProperty =
    BindableProperty.Create(nameof(Text), typeof(string), typeof(PlacesSearch), string.Empty, BindingMode.TwoWay);

        public static readonly BindableProperty PlaceholderProperty =
   BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(PlacesSearch), string.Empty);

        

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
                Text = value;
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
            get { return (string)this.GetValue(PlaceholderProperty); }
            set
            {
                if (this._useSearchBar)
                    this._searchBar.Placeholder = value;
                else
                    this._entry.Placeholder = value;
                this.SetValue(PlaceholderProperty, value);

            }
        }

        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value);
                this._entry.Text = value;
            }
        }

        public PlacesSearch()
        {
            this._useSearchBar = false;
            this.Init();
        }
        private async void Init()
        {
            await System.Threading.Tasks.Task.Delay(500);

            OsmNominatim.Instance.CountryCodes.Add("br");
            this._stackLayout = new StackLayout()
            {
                Spacing = 0,
                IsVisible = false,
                BackgroundColor = Color.White
            };

            Image img = new Image();
            img.Source = ImageSource.FromFile("powered_by_google_on_white.png");
            img.HeightRequest = 20;
            this._stackLayout.Children.Add(img);
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
                cell.SetBinding(ImageCell.TextProperty, "name_full");

                return cell;
            });
            this._stackLayout.Children.Add(this._autoCompleteListView);



            View searchView;
            if (this._useSearchBar)
            {
                this._searchBar = new SearchBar
                {
                    Placeholder = this.Placeholder
                };
                this._searchBar.TextChanged += SearchTextChanged;
                this._searchBar.SearchButtonPressed += SearchButtonPressed;

                searchView = this._searchBar;

            }
            else
            {
                Xamarin.Forms.TimePicker picker = new TimePicker();

                this._entry = new Entry
                {
                    Placeholder = this.Placeholder
                };
                if (!string.IsNullOrEmpty(Text))
                    this._entry.Text = Text;
                this._entry.TextChanged += SearchTextChanged;
                this._entry.Unfocused += _entry_Unfocused;

                searchView = this._entry;
            }
            this.Children.Add(searchView,
                Constraint.Constant(0),
                Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent(l => l.Width));

            this.Children.Add(
                this._stackLayout,
                Constraint.Constant(0),
                Constraint.RelativeToView(searchView, (r, v) => v.Y + v.Height));

            this._autoCompleteListView.ItemSelected += ItemSelected;

            this._textChangeItemSelected = false;
        }

        private async void _entry_Unfocused(object sender, FocusEventArgs e)
        {
            await System.Threading.Tasks.Task.Delay(500);
            this._autoCompleteListView.HeightRequest = 0;
            this._autoCompleteListView.IsVisible = false;
            this._stackLayout.IsVisible = false;
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
            Text = this._entry.Text;
            this.SearchPlaces();
        }

        private async void SearchPlaces()
        {
            try
            {
                if (string.IsNullOrEmpty(this.SearchText) || this.SearchText.Length < 3)
                {
                    this._autoCompleteListView.ItemsSource = null;
                    this._autoCompleteListView.IsVisible = false;
                    this._stackLayout.IsVisible = false;
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
                    this._stackLayout.IsVisible = true;

                    this._autoCompleteListView.ItemsSource = this._predictions;
                }
                else
                {
                    this._autoCompleteListView.HeightRequest = 0;
                    this._autoCompleteListView.IsVisible = false;
                    this._stackLayout.IsVisible = true;

                }
            }
            catch (Exception )
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
            this._stackLayout.IsVisible = true;

            this._autoCompleteListView.HeightRequest = 0;

            if (this._useSearchBar)
                this._searchBar.Unfocus();
            else
                this._entry.Unfocus();
        }
    }

}


