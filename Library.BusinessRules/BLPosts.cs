using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Posts.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLPosts
    {
        // Crea un post.
        public async Task<int> CreatePostAsync(Posts pPosts)
        {
            return await DALPosts.CreatePostAsync(pPosts);
        }

        // Actualiza un post.
        public async Task<int> UpdatePostAsync(Posts pPosts)
        {
            return await DALPosts.UpdatePostAsync(pPosts);
        }

        // Elimina un post.
        public async Task<int> DeletePostAsync(Posts pPosts)
        {
            return await DALPosts.DeletePostAsync(pPosts);
        }

        // Obtiene un post por id (u otros campos que use el DAL).
        public async Task<Posts> GetPostByIdAsync(Posts pPosts)
        {
            return await DALPosts.GetPostByIdAsync(pPosts);
        }

        // Obtiene todos los posts.
        public async Task<List<Posts>> GetAllPostsAsync()
        {
            return await DALPosts.GetAllPostsAsync();
        }

        // Consulta posts según criterios en el objeto.
        public async Task<List<Posts>> GetPostsAsync(Posts pPosts)
        {
            return await DALPosts.GetPostsAsync(pPosts);
        }

        // Obtiene posts filtrados para la gestión (búsqueda avanzada).
        public async Task<List<Posts>> GetSearchedManagePosts(Posts pPosts)
        {
            return await DALPosts.GetSearchedManagePosts(pPosts);
        }

        // Obtiene los posts fijados (pinned).
        public async Task<List<Posts>> GetPinnedPosts()
        {
            return await DALPosts.GetPinnedPosts();
        }

        // Obtiene los posts más recientes.
        public async Task<List<Posts>> GetLastPosts()
        {
            return await DALPosts.GetLastPosts();
        }
    }
}
