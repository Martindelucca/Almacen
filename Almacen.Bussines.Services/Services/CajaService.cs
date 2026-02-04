using Almacen.Core.Entities;
using Almacen.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Almacen.Core.Dtos;
using System.Xml;

namespace Almacen.Business.Services
{
    public class CajaService
    {
        private readonly ICajaRepository _cajaRepo;
        public CajaService (ICajaRepository cajaRepo)
        {
            _cajaRepo = cajaRepo ?? throw new ArgumentNullException (nameof(cajaRepo));
        }
        public async Task<SesionCaja> VerificarCajaAbierta(int idUsuario)
        {
            return await _cajaRepo.ObtenerSesionAbiertaAsync(idUsuario);
        }

        public async Task AbrirCaja(int idUsuario, decimal montoInicial)
        {
            // Regla de negocio: No abrir si ya tiene una abierta
            var sesion = await _cajaRepo.ObtenerSesionAbiertaAsync(idUsuario);
            if (sesion != null)
                throw new Exception("Ya tienes una caja abierta.");

            await _cajaRepo.AbrirCajaAsync(idUsuario, montoInicial);
        }

        public async Task<ResumenCierreDto> ObtenerResumenCierre(int idUsuario)
        {
            // 1. Buscamos la sesión actual
            var sesion = await _cajaRepo.ObtenerSesionAbiertaAsync(idUsuario);
            if (sesion == null) throw new Exception("No hay una caja abierta para cerrar.");

            // 2. Sumamos todas las ventas NO ANULADAS de esa sesión
            decimal ventas = await _cajaRepo.ObtenerTotalVentasSesionAsync(sesion.IdSesion);

            // 3. Retornamos el resumen
            return new ResumenCierreDto
            {
                MontoInicial = sesion.MontoInicial,
                TotalVentasEfectivo = ventas,
                // Aquí sumamos: Lo que había al inicio + Lo que vendió
                TotalEsperado = sesion.MontoInicial + ventas
            };
        }

        public async Task CerrarCaja(int idUsuario, decimal montoRealEnCaja)
        {
            var sesion = await _cajaRepo.ObtenerSesionAbiertaAsync(idUsuario);
            if (sesion == null) throw new Exception("No hay caja abierta.");

            // Calculamos de nuevo por seguridad
            decimal ventas = await _cajaRepo.ObtenerTotalVentasSesionAsync(sesion.IdSesion);
            decimal montoSistema = sesion.MontoInicial + ventas;

            // Calculamos la diferencia (Sobrante o Faltante)
            decimal diferencia = montoRealEnCaja - montoSistema;

            // Guardamos en BD (Asegúrate de tener este método en tu Repo)
            await _cajaRepo.CerrarCajaAsync(sesion.IdSesion, montoSistema, montoRealEnCaja, diferencia);
        }
    }
}
