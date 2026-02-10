using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para la relación Post-Categoría.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLPostsCategories
    {
        // Crea una asociación post-categoría.
        public async Task<int> CreatePostCategoryAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.CreatePostCategoryAsync(pPostsCategories);
        }

        // Actualiza una asociación post-categoría.
        public async Task<int> UpdatePostCategeoryAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.UpdatePostCategeoryAsync(pPostsCategories);
        }

        // Elimina una asociación post-categoría.
        public async Task<int> DeletePostCategoryAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.DeletePostCategoryAsync(pPostsCategories);
        }

        // Obtiene una asociación por id (u otros campos que use el DAL).
        public async Task<PostsCategories> GetPostCategoryByIdAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.GetPostCategoryByIdAsync(pPostsCategories);
        }

        // Obtiene todas las asociaciones post-categoría.
        public async Task<List<PostsCategories>> GetAllPostCategoriesAsync()
        {
            return await DALPostsCategories.GetAllPostCategoriesAsync();
        }

        // Consulta asociaciones según criterios en el objeto.
        public async Task<List<PostsCategories>> GetPostCategoryAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.GetPostCategoryAsync(pPostsCategories);
        }
    }
}
