using CV.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.Fotos
{
    public interface IFoto
    {
        Task SubirFoto(string AccessId, string CodigoAlbum, byte[] DadosFoto, UploadFoto itemFoto);
        Task UpdateMediaData(List<Foto> itemFoto);
        Task UpdateMediaData(List<PontoMapa> itemFoto);
        Task UpdateMediaData( List<Timeline> itemFoto);
    }
}
