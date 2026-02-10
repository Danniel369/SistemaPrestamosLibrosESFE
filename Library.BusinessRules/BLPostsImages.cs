using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para PostsImages.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLPostImages
    {
        // Crea una imagen asociada a un post.
        public async Task<int> CreatePostImageAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.CreatePostImageAsync(pPostImages);
        }

        // Actualiza una imagen de post.
        public async Task<int> UpdatePostImageAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.UpdatePostImageAsync(pPostImages);
        }

        // Actualiza la imagen de un post especificando el id del post.
        public async Task<int> UpdatePostImageByIdPostAsync(Posts pPosts, PostsImages pPostsImages)
        {
            return await DALPostsImages.UpdatePostImageByIdPostAsync(pPosts, pPostsImages);
        }

        // Elimina una imagen de post.
        public async Task<int> DeletePostImageAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.DeletePostImageAsync(pPostImages);
        }

        // Elimina todas las imágenes de un post por su id.
        public async Task<int> DeletePostsImagesByIdPostAsync(Posts pPosts)
        {
            return await DALPostsImages.DeletePostsImagesByIdPostAsync(pPosts);
        }

        // Obtiene imágenes por id de imagen (puede devolver varias si la implementación lo permite).
        public async Task<List<PostsImages>> GetPostImagesByIdAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.GetPostImagesByIdAsync(pPostImages);
        }

        // Obtiene todas las imágenes asociadas a un post por id del post.
        public async Task<List<PostsImages>> GetPostImagesByIdPostAsync(Posts pPosts)
        {
            return await DALPostsImages.GetPostImagesByIdPostAsync(pPosts);
        }

        // Obtiene todas las imágenes de posts.
        public async Task<List<PostsImages>> GetAllPostImagesAsync()
        {
            return await DALPostsImages.GetAllPostImagesAsync();
        }

        // Consulta imágenes según criterios en el objeto.
        public async Task<List<PostsImages>> GetPostImagesAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.GetPostImagesAsync(pPostImages);
        }
    }
}
