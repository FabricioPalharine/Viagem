using CV.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.GPS
{
    public interface IGPSService
    {
        Task<PosicaoMapa> RetornarPosicao();
        Task IniciarGPS();
        Task PararGPS();
    }
}
