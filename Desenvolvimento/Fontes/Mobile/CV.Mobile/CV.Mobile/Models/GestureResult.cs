using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV.Mobile.Controls;
using Xamarin.Forms;

namespace CV.Mobile.Models
{
    public class GestureResult
    {
        /// <summary>
        /// The gesture type
        /// </summary>
        public GestureType GestureType { get; set; }
        /// <summary>
        /// The direction (if any) of the direction
        /// </summary>
        public Directionality Direction { get; set; }
        /// <summary>
        /// The point, relative to the start view where the 
        /// gesture started
        /// </summary>
        public Point Origin { get; set; }
        /// <summary>
        /// The point, relative to the start view where the second finger of the
        /// gesture is located (valid for GestureType.Pinch)
        /// </summary>
        public Point Origin2 { get; set; }
        /// <summary>
        /// The view that the gesture started in
        /// </summary>
        public Xamarin.Forms.View StartView { get; set; }
        /// <summary>
        /// The Vector Length of the gesture (if appropiate)
        /// </summary>
        public Double Length { get; set; }
        /// <summary>
        /// The Vertical distance the gesture travelled
        /// </summary>
        public Double VerticalDistance { get; set; }
        /// <summary>
        /// The horizontal distance the gesture travelled
        /// </summary>
        public Double HorizontalDistance { get; set; }

        /// <summary>Gets or sets the view stack.</summary>
        /// <value>A list of all view elements containing the origin point.</value>
        /// Element created at 07/11/2014,11:54 PM by Charles
        public List<Xamarin.Forms.View> ViewStack { get; set; }

    }
}
