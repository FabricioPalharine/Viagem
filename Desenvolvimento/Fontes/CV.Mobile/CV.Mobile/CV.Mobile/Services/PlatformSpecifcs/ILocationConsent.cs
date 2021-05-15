using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.PlatformSpecifcs
{
    public interface ILocationConsent
    {
        Task GetLocationConsent();
    }
}
