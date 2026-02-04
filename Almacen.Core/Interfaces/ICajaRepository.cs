using Almacen.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Interfaces
{
    public  interface ICajaRepository
    {
        Task<SesionCaja> ObtenerSesionAbiertaAsync(int idUsuario);
        Task<int> AbrirCajaAsync(int idUsuario, decimal montoInicial);
        Task<decimal> ObtenerTotalVentasSesionAsync(int idSesion);
        Task CerrarCajaAsync(int idSesion, decimal sistema, decimal real, decimal diferencia);


    }
}
