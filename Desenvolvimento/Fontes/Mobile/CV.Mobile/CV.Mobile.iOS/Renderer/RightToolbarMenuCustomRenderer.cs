using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CV.Mobile.iOS.Renderer;
using CV.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Drawing;

[assembly: ExportRenderer(typeof(EdicaoCarroPage), typeof(RightToolbarMenuCustomRenderer))]

namespace CV.Mobile.iOS.Renderer
{

    public class RightToolbarMenuCustomRenderer : PageRenderer
    {

        //I used UITableView for showing the menulist of secondary toolbar items.
        List<ToolbarItem> _secondaryItems;
        UITableView table;
        ContentPage _page;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            //Get all secondary toolbar items and fill it to the gloabal list variable and remove from the content page.
            if (e.NewElement is ContentPage page)
            {
                _page = page;
                _secondaryItems = page.ToolbarItems.Where(i => i.Order == ToolbarItemOrder.Secondary).ToList();
                _secondaryItems.ForEach(t => page.ToolbarItems.Remove(t));
            }
            base.OnElementChanged(e);
        }

        public override void ViewWillAppear(bool animated)
        {
            var element = (ContentPage)Element;
            //If global secondary toolbar items are not null, I created and added a primary toolbar item with image(Overflow) I         
            // want to show.

            if (_secondaryItems != null && _secondaryItems.Count > 0)
            {
                _secondaryItems.ForEach(t => _page.ToolbarItems.Remove(t));

                if (!element.ToolbarItems.Where(d => d.Icon == "more24.png").Any())
                {
                    element.ToolbarItems.Add(new ToolbarItem()
                    {
                        Order = ToolbarItemOrder.Primary,
                        Icon = "more24.png",
                        Priority = 1,
                        Command = new Command(() =>
                        {
                            ToolClicked();
                        })
                    });
                }
            }
            base.ViewWillAppear(animated);
        }

        //Create a table instance and added it to the view.
        private void ToolClicked()
        {
            if (table == null)
            {
                //Set the table position to right side. and set height to the content height.
                var childRect = new RectangleF((float)View.Bounds.Width - 250, 0, 250, _secondaryItems.Count() * 56);
                table = new UITableView(childRect)
                {
                    Source = new TableSource(_secondaryItems) // Created Table Source Class as Mentioned in the 
                                                              //Xamarin.iOS   Official site
                };
                Add(table);
                return;
            }
            foreach (var subview in View.Subviews)
            {
                if (subview == table)
                {
                    table.RemoveFromSuperview();
                    return;
                }
            }
            Add(table);
        }
    }

   

public class TableSource : UITableViewSource
    {

        // Global variable for the secondary toolbar items and text to display in table row
        List<ToolbarItem> _tableItems;
        string[] _tableItemTexts;
        string CellIdentifier = "TableCell";

        public TableSource(List<ToolbarItem> items)
        {
            //Set the secondary toolbar items to global variables and get all text values from the toolbar items
            _tableItems = items;
            _tableItemTexts = items.Select(a => a.Text).ToArray();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _tableItemTexts.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
            string item = _tableItemTexts[indexPath.Row];
            if (cell == null)
            { cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier); }
            cell.TextLabel.Text = item;
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 56; // Set default row height.
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //Used command to excute and deselct the row and removed the table.
            var command = _tableItems[0].Command;
            command.Execute(_tableItems[0].CommandParameter);
            tableView.DeselectRow(indexPath, true);
            tableView.RemoveFromSuperview();
        }
    }
}