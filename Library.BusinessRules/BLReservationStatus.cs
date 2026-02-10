using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para ReservationStatus.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLReservationStatus
    {
        // Crea un estado de reserva.
        public async Task<int> CreateReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            return await DALReservationStatus.CreateReservationStatusAsync(pReservationStatus);
        }

        // Actualiza un estado de reserva.
        public async Task<int> UpdateReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            return await DALReservationStatus.UpdateReservationStatusAsync(pReservationStatus);
        }

        // Elimina un estado de reserva.
        public async Task<int> DeleteReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            return await DALReservationStatus.DeleteReservationStatusAsync(pReservationStatus);
        }

        // Obtiene todos los estados de reserva.
        public async Task<List<ReservationStatus>> GetAllReservationStatusAsync()
        {
            return await DALReservationStatus.GetAllReservationStatusAsync();
        }

        // Obtiene un estado de reserva por id (u otros campos que use el DAL).
        public async Task<ReservationStatus> GetReservationStatusByIdAsync(ReservationStatus pReservationStatus)
        {
            return await DALReservationStatus.GetReservationStatusByIdAsync(pReservationStatus);
        }

        // Consulta estados de reserva según criterios en el objeto.
        public async Task<List<ReservationStatus>> GetReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            return await DALReservationStatus.GetReservationStatusAsync(pReservationStatus);
        }

    }
}
