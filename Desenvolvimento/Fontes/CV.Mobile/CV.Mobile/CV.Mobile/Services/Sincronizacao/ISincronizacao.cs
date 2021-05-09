using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.Sincronizacao
{
    public interface ISincronizacao
    {
        Task<bool> Sincronizar(bool Comandada);       
        
    }
}
