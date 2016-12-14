﻿using System;
using System.Collections;
using System.Reflection;
using Xamarin.Forms;

namespace CV.Mobile.Controls
{
    public class ExtendedPicker : Picker
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExtendedPicker()
        {
            base.SelectedIndexChanged += OnSelectedIndexChanged;
        }
        /// <summary>
        /// Identifies the <see cref="P:XLabs.Forms.Controls.ExtendedPicker.SelectedItem" /> property.
        /// </summary>
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(object), typeof(ExtendedPicker), null, BindingMode.TwoWay, null, new BindableProperty.BindingPropertyChangedDelegate(ExtendedPicker.OnSelectedItemChanged), null, null, null);
        /// <summary>
        /// Identifies the <see cref="P:XLabs.Forms.Controls.ExtendedPicker.ItemSource" /> property.
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(ExtendedPicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(ExtendedPicker.OnItemsSourceChanged), null, null, null);
        /// <summary>
        /// Identifies the <see cref="P:XLabs.Forms.Controls.ExtendedPicker.DisplayProperty" /> property.
        /// </summary>
        public static readonly BindableProperty DisplayPropertyProperty = BindableProperty.Create("DisplayProperty", typeof(string), typeof(ExtendedPicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(ExtendedPicker.OnDisplayPropertyChanged), null, null, null);
        /// <summary>
        /// Identifies the <see cref="P:XLabs.Forms.Controls.ExtendedPicker.KeyMemberPath" /> property.
        /// </summary>
        public static readonly BindableProperty KeyMemberPathProperty = BindableProperty.Create("KeyMemberPath", typeof(string), typeof(ExtendedPicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(ExtendedPicker.OnKeyMemberPathChanged), null, null, null);

        /// <summary>
        /// Accepts an <see cref="T:System.Collections.IList" /> which is used to populate the picker with data.
        /// By default the <see cref="System.Object.ToString"/> is displayed.
        /// </summary>
        /// <value>The items source.</value>
        public IList ItemsSource
        {
            get
            {
                return (IList)base.GetValue(ExtendedPicker.ItemsSourceProperty);
            }
            set
            {
                base.SetValue(ExtendedPicker.ItemsSourceProperty, value);
            }
        }
        /// <summary>
        /// Sets the item currently displayed in the UI.
        /// This works by finding the index of the item in the backing data and then updateing <see cref="P:Xamarin.Forms.Picker.SelectedIndex" />
        /// so the two object need the same ref.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get
            {
                return base.GetValue(ExtendedPicker.SelectedItemProperty);
            }
            set
            {
                base.SetValue(ExtendedPicker.SelectedItemProperty, value);
            }
        }
        /// <summary>
        /// A string used in reflection to identify the property to use as the display property of the object.
        /// If this is not specified the <see cref="System.Object.ToString"/> is used.
        /// </summary>
        /// <value>The display property.</value>
        public string DisplayProperty
        {
            get
            {
                return (string)base.GetValue(ExtendedPicker.DisplayPropertyProperty);
            }
            set
            {
                base.SetValue(ExtendedPicker.DisplayPropertyProperty, value);
            }
        }

        /// <summary>
        /// A string used in reflection to identify the property to retrieve from the object.
        /// If this is not specified the object is returned.
        /// </summary>
        /// <value>The key member path property.</value>
        public string KeyMemberPath
        {
            get
            {
                return (string)base.GetValue(ExtendedPicker.KeyMemberPathProperty);
            }
            set
            {
                base.SetValue(ExtendedPicker.KeyMemberPathProperty, value);
            }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndex > -1)
            {
                if (!string.IsNullOrEmpty(KeyMemberPath))
                {
                    var selectedItem = ItemsSource[SelectedIndex];
                    var keyProperty = selectedItem.GetType().GetRuntimeProperty(KeyMemberPath);
                    var valor = keyProperty.GetValue(selectedItem);
                    if (valor != this.SelectedItem)
                        this.SelectedItem = valor;
                }
                else
                    this.SelectedItem = ItemsSource[SelectedIndex];
            }
        }


        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ExtendedPicker bindablePicker = (ExtendedPicker)bindable;
            ExtendedPicker picker = bindable as ExtendedPicker;
            if (newValue == null)
            {
                picker.SelectedIndex = -1;
                return;
            }
            if (picker != null && picker.SelectedIndex != -1)
            {
                var selectedItem = picker.ItemsSource[picker.SelectedIndex];
                if (!string.IsNullOrWhiteSpace(picker.KeyMemberPath))
                {
                    var keyProperty = selectedItem.GetType().GetRuntimeProperty(picker.KeyMemberPath);
                    if (keyProperty == null)
                    {
                        throw new InvalidOperationException(String.Concat(picker.KeyMemberPath, " is not a property of ",
                            selectedItem.GetType().FullName));
                    }
                    picker.SelectedItem = keyProperty.GetValue(selectedItem);
                }
                else
                {
                    picker.SelectedItem = selectedItem;
                }
            }
            if (bindablePicker.ItemsSource != null && bindablePicker.SelectedItem != null)
            {
                int count = 0;
                foreach (object obj in bindablePicker.ItemsSource)
                {
                    if (!string.IsNullOrWhiteSpace(picker.KeyMemberPath))
                    {
                        var keyProperty = bindablePicker.SelectedItem;
                        var objProperty = obj.GetType().GetRuntimeProperty(picker.KeyMemberPath);
                        if (keyProperty.ToString() == objProperty.GetValue(obj).ToString())
                        {
                            bindablePicker.SelectedIndex = count;
                            break;
                        }
                    }
                    else if (obj == bindablePicker.SelectedItem)
                    {
                        bindablePicker.SelectedIndex = count;
                        break;
                    }
                    count++;
                }
            }
        }

        private static void OnDisplayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {

            ExtendedPicker bindablePicker = (ExtendedPicker)bindable;
            bindablePicker.DisplayProperty = (string)newValue;
            loadItemsAndSetSelected(bindable);

        }

        private static void OnKeyMemberPathChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ExtendedPicker picker = bindable as ExtendedPicker;
            picker.KeyMemberPath = newValue?.ToString();
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ExtendedPicker bindablePicker = (ExtendedPicker)bindable;
            bindablePicker.ItemsSource = (IList)newValue;
            loadItemsAndSetSelected(bindable);
        }
        static void loadItemsAndSetSelected(BindableObject bindable)
        {
            ExtendedPicker bindablePicker = (ExtendedPicker)bindable;
            if (bindablePicker.ItemsSource as IEnumerable != null)
            {
                PropertyInfo propertyInfo = null;
                int count = 0;
                foreach (object obj in (IEnumerable)bindablePicker.ItemsSource)
                {
                    string value = string.Empty;
                    if (bindablePicker.DisplayProperty != null)
                    {
                        if (propertyInfo == null)
                        {
                            propertyInfo = obj.GetType().GetRuntimeProperty(bindablePicker.DisplayProperty);
                            if (propertyInfo == null)
                                throw new Exception(String.Concat(bindablePicker.DisplayProperty, " is not a property of ", obj.GetType().FullName));
                        }
                        value = propertyInfo.GetValue(obj).ToString();
                    }
                    else
                    {
                        value = obj.ToString();
                    }
                    bindablePicker.Items.Add(value);
                    if (bindablePicker.SelectedItem != null)
                    {
                        if (bindablePicker.SelectedItem == obj)
                        {
                            bindablePicker.SelectedIndex = count;
                        }
                    }
                    count++;
                }
            }
        }
    }
}
