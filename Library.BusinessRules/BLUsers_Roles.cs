using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para la relación Users_Roles.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLUsers_Roles
    {
        // Crea una relación usuario-rol.
        public async Task<int> CreateRolesAsync(Users_Roles pUsers_Roles)
        {
            return await DALUsers_Roles.CreateRolesAsync(pUsers_Roles);
        }

        // Actualiza una relación usuario-rol.
        public async Task<int> UpdateRolesAsync(Users_Roles pUsers_Roles)
        {
            return await DALUsers_Roles.UpdateRolesAsync(pUsers_Roles); 
        }

        // Elimina una relación usuario-rol.
        public async Task<int> DeleteRolesAsync(Users_Roles pUser_roles)
        {
            return await DALUsers_Roles.DeleteRolesAsync(pUser_roles);
        }

        // Obtiene todas las relaciones usuario-rol.
        public async Task<List<Users_Roles>> GetAllRolesASync()
        {
            return await DALUsers_Roles.GetAllRolesASync();
        }

        // Obtiene una relación por id (u otros campos que use el DAL).
        public async Task<Users_Roles> GetRolesByIdAsync(Users_Roles pUser_roles)
        {
            return await DALUsers_Roles.GetRolesByIdAsync(pUser_roles);
        }

        // Consulta relaciones según criterios en el objeto.
        public async Task<List<Users_Roles>> GetRolesAsync(Users_Roles pRoles)
        {
            return await DALUsers_Roles.GetRolesAsync(pRoles);
        }
    }
}
