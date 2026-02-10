using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para AcquisitionTypes.
    // Actualmente actúa como fachada que delega las operaciones al DAL.
    public class BLAcquisitionTypes
    {
        // Crea un tipo de adquisición.
        public async Task<int> CreateAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            return await DALAcquisitionTypes.CreateAcquisitionTypesAsync(pAcquisitionTypes);
        }

        // Actualiza un tipo de adquisición.
        public async Task<int> UpdateAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            return await DALAcquisitionTypes.UpdateAcquisitionTypesAsync(pAcquisitionTypes);
        }

        // Elimina un tipo de adquisición.
        public async Task<int> DeleteAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            return await DALAcquisitionTypes.DeleteAcquisitionTypesAsync(pAcquisitionTypes);
        }

        // Obtiene todos los tipos de adquisición.
        public async Task<List<AcquisitionTypes>> GetAllAcquisitionTypesAsync()
        {
            return await DALAcquisitionTypes.GetAllAcquisitionTypesAsync();
        }

        // Obtiene un tipo de adquisición por id (u otros campos que use el DAL).
        public async Task<AcquisitionTypes> GetAcquisitionTypesByIdAsync(AcquisitionTypes pAcquisitionTypes)
        {
            return await DALAcquisitionTypes.GetAcquisitionTypesByIdAsync(pAcquisitionTypes);
        }

        // Consulta tipos de adquisición según criterios en el objeto.
        public async Task<List<AcquisitionTypes>> GetAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            return await DALAcquisitionTypes.GetAcquisitionTypesAsync(pAcquisitionTypes);
        }
    }
}
