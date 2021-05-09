using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CV.Mobile.Behaviors.Base;
using Xamarin.Forms;
using CV.Mobile.Extensions;

namespace CV.Mobile.Behaviors
{
    public class MaskedBehavior: BindableBehavior<Entry>
    {
        public static BindableProperty MaskProperty =
           BindableProperty.CreateAttached(nameof(Mask), typeof(string), typeof(MaskedBehavior), null,
               BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(OnMaskChanged));

 

        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value);
                //SetPositions();
            }
        }
        private string[] _mascaras;
        
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        IDictionary<string, IDictionary<int, char>> _mascarasPosicoes = new Dictionary<string, IDictionary<int, char>>();
        IDictionary<string, string> _mascaraLivres = new Dictionary<string, string>();

        void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _mascaras = null;
                return;
            }

            _mascaras = Mask.Split('|').OrderBy(d=>d.Length).ToArray();
            _mascaraLivres.Clear();
            _mascarasPosicoes.Clear();
            foreach (var mascara in _mascaras)
            {
                List<char> posicoesDigitacao = new List<char>();
                var list = new Dictionary<int, char>();
                for (var i = 0; i < mascara.Length; i++)
                    if (mascara[i] != 'X' && mascara[i] != '9' && mascara[i] != 'H')
                        list.Add(i, mascara[i]);
                    else
                        posicoesDigitacao.Add(mascara[i]);
                _mascaraLivres.Add(new string(posicoesDigitacao.ToArray()),mascara);
                _mascarasPosicoes.Add(mascara, list);
            }
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            var text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(Mask))
            {
                return;
            }
            if (_mascaras == null)
                SetPositions();

            if (text.Length > _mascaras.Max(d=>d.Length))
            {                
                entry.Text = args.OldTextValue;
                return;
            }

            string MascaraAtual = _mascaras.Where(d => d.Length >= (args.OldTextValue??string.Empty).Length).OrderBy(d => d.Length).FirstOrDefault();
            if (MascaraAtual == null)
                MascaraAtual = _mascaras.OrderByDescending(d => d.Length).FirstOrDefault();
            string TextoPuroAnterior = RemoverMascara((args.OldTextValue??string.Empty), MascaraAtual);
            string TextoPuroAtual = RemoverMascara(( args.NewTextValue??string.Empty), MascaraAtual);

            var itemMascara = _mascaraLivres.Where(d => d.Key.Length >= TextoPuroAtual.Length).OrderBy(d => d.Key.Length).FirstOrDefault();
            MascaraAtual = itemMascara.Value;
            text = TextoPuroAtual;
            if (TextoPuroAtual.Length > TextoPuroAnterior.Length)
            {
                int PosicaoDiferenca = -1;
                for (int i=0;i<TextoPuroAnterior.Length;i++)
                {
                    if (TextoPuroAnterior[i] != TextoPuroAtual[i] && PosicaoDiferenca < 0)
                        PosicaoDiferenca = i;
                }
                if (PosicaoDiferenca < 0)
                    PosicaoDiferenca = TextoPuroAtual.Length - 1;


                char CaracterPosicao = TextoPuroAtual[PosicaoDiferenca];
                char CaracterValidacao = itemMascara.Key[PosicaoDiferenca];
                if ((CaracterValidacao == '9' && !char.IsDigit(CaracterPosicao))
                    || (CaracterValidacao == 'H' && !CaracterPosicao.IsHex()))
                {
                    entry.Text = args.OldTextValue;
                    return;
                }
            }
          


            foreach (var position in _mascarasPosicoes[MascaraAtual])
                if (text.Length >= position.Key + 1)
                {
                    var value = position.Value.ToString();
                    if (text.Substring(position.Key, 1) != value)
                        text = text.Insert(position.Key, value);
                }

            if (entry.Text != text)
                entry.Text = text;
        }

        private string RemoverMascara(string oldTextValue, string mascaraAtual)
        {
            var lista = _mascarasPosicoes[mascaraAtual];
            foreach(var posicao in lista.Keys)
            {
                oldTextValue = oldTextValue.Replace(lista[posicao].ToString(), string.Empty);
                //if (posicao < oldTextValue.Length)
                //    oldTextValue = oldTextValue.Remove(posicao,1);
            }
            return oldTextValue;
        }

        private static void OnMaskChanged(BindableObject bindable, object oldValue, object newValue)
        {
            MaskedBehavior element = (MaskedBehavior)bindable;
            element.SetPositions();

           

        }
    }
}
