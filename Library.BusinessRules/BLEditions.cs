using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Editions.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLEditions
    {

        // Crea una edición.
        public async Task<int> CreateEditionsAsync(Editions pEditions)
        {
            return await DALEditions.CreateEditionsAsync(pEditions);
        }

        // Actualiza una edición.
        public async Task<int> UpdateEditionsAsync(Editions pEditions)
        {
            return await DALEditions.UpdateEditionsAsync(pEditions);
        }

        // Elimina una edición.
        public async Task<int> DeleteEditionsAsync(Editions pEditions)
        {
            return await DALEditions.DeleteEditionsAsync(pEditions);
        }

        // Obtiene todas las ediciones.
        public async Task<List<Editions>> GetAllEditionsAsync()
        {
            return await DALEditions.GetAllEditionsAsync();
        }

        // Obtiene una edición por id (u otros campos que use el DAL).
        public async Task<Editions> GetEditionsByIdAsync(Editions pEditions)
        {
            return await DALEditions.GetEditionsByIdAsync(pEditions);
        }

        // Consulta ediciones según criterios en el objeto.
        public async Task<List<Editions>> GetEditionsAsync(Editions pEditions)
        {
            return await DALEditions.GetEditionsAsync(pEditions);
        }
    }
}
