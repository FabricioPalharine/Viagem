using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Services.PlatformSpecifcs
{
    public interface IStatusBarStyleManager
    {
        void SetColoredStatusBar(string hexColor);
        void SetWhiteStatusBar();
    }
}
