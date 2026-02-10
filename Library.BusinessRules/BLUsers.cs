using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Users.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLUsers
    {
        // Crea un usuario.
        public async Task<int> CreateUsersAsync(Users pUsers)
        {
            return await DALUsers.CreateUsersAsync(pUsers);
        }

        // Actualiza un usuario.
        public async Task<int> UpdateUsersASync(Users pUsers)
        {
            return await DALUsers.UpdateUsersASync(pUsers);
        }

        // Elimina un usuario.
        public async Task<int> DeleteUsersAsync(Users pUsers)
        {
            return await DALUsers.DeleteUsersAsync(pUsers);
        }

        // Obtiene todos los usuarios.
        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await DALUsers.GetAllUsersAsync();
        }

        // Obtiene un usuario por id (u otros campos que use el DAL).
        public async Task<Users> GetUsersByIdAsync(Users pUsers)
        {
            return await DALUsers.GetUsersByIdAsync(pUsers);
        }

        // Consulta usuarios según criterios en el objeto.
        public async Task<List<Users>> GetUsersAsync(Users pUsers)
        {
            return await DALUsers.GetUsersAsync(pUsers);
        }

        // Obtiene usuarios incluyendo roles (si el DAL lo aporta).
        public async Task<List<Users>> GetIncludeRolesASync(Users pUsers)
        {
            return await DALUsers.GetIncludeRolesASync(pUsers);
        }

        // Realiza el login (validación de credenciales) delegando al DAL.
        public async Task<Users> LoginAsync(Users pUsers)
        {
            return await DALUsers.LoginAsync(pUsers);
        }

        // Cambia la contraseña, recibiendo la contraseña anterior para validación.
        public async Task<int> ChangePasswordAsync(Users pUsers, string pOldPassword)
        {
            return await DALUsers.ChangePasswordAsync(pUsers, pOldPassword);
        }
    }
}
