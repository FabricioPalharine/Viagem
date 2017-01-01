using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV.Mobile.Controls;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace CV.Mobile.Models
{
    public class GestureCollection : ObservableCollection<GestureInterest> { }
    public class GestureInterest : BindableObject
    {
        /// <summary>
        /// The property definition for <see cref="Notification"/>
        /// </summary>
        public static BindableProperty NotifcationProperty = BindableProperty.Create("Notification", typeof(GestureNotification), typeof(GestureInterest), GestureNotification.None);
        /// <summary>
        /// The property defintion for <see cref="GestureType"/>
        /// </summary>
        public static BindableProperty GestureTypeProperty = BindableProperty.Create
            (nameof(GestureType), typeof(GestureType), typeof(GestureInterest), GestureType.Unknown);

        /// <summary>
        /// The property definitionf for <see cref="Direction"/>
        /// </summary>
        public static BindableProperty DirectionProperty = BindableProperty.Create
            (nameof(Direction), typeof(Directionality), typeof(GestureInterest), Directionality.None);


        /// <summary>
        /// The property definition for <see cref="GestureCommand"/>
        /// </summary>
        public static BindableProperty GestureCommandProperty = BindableProperty.Create
             (nameof(GestureCommand), typeof(IGesture), typeof(GestureInterest), default(IGesture));
  
        /// <summary>
        /// The property definition for <see cref="GestureParameter"/>
        /// </summary>
        public static BindableProperty GestureParameterProperty = BindableProperty.Create
             (nameof(GestureParameter), typeof(object), typeof(GestureInterest), default(object));


        /// <summary>
        /// The notification to use with this gesture
        /// </summary>
        public GestureNotification Notification
        {
            get { return (GestureNotification)GetValue(NotifcationProperty); }
            set { SetValue(NotifcationProperty, value); }
        }
        /// <summary>
        /// The Gesture type you are interested in <see cref="GestureType"/>
        /// </summary>
        public GestureType GestureType
        {
            get { return (GestureType)GetValue(GestureTypeProperty); }
            set { SetValue(GestureTypeProperty, value); }
        }

        /// <summary>
        /// The Direction (if appropiate) <see cref="Directionality"/>
        /// </summary>
        public Directionality Direction
        {
            get { return (Directionality)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// The implementation of <see cref="IGesture"/>
        /// </summary>
        public IGesture GestureCommand
        {
            get { return (IGesture)GetValue(GestureCommandProperty); }
            set { SetValue(GestureCommandProperty, value); }
        }

        /// <summary>
        /// An optional paramater passed to <see cref="IGesture.ExecuteGesture"/>
        /// and <see cref="IGesture.CanExecuteGesture"/>
        /// </summary>
        public object GestureParameter
        {
            get { return GetValue(GestureParameterProperty); }
            set { SetValue(GestureParameterProperty, value); }
        }
    }

    public interface IGesture
    {
        /// <summary>
        /// Execute the gesture
        /// </summary>
        /// <param name="result">The <see cref="GestureResult"/></param>
        /// <param name="param">the user supplied paramater</param>
        void ExecuteGesture(GestureResult result, object param);
        /// <summary>
        /// Checks to see if the gesture should execute
        /// </summary>
        /// <param name="result">The <see cref="GestureResult"/></param>
        /// <param name="param">The user supplied parameter</param>
        /// <returns>True to execute the gesture, False otherwise</returns>
        bool CanExecuteGesture(GestureResult result, object param);
    }
}
