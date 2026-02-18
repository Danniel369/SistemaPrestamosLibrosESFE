using Library.BusinessRules;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;



namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class BooksController : Controller
    {
        BLBooks booksBL = new BLBooks();
        BLCategories categoriesBL = new BLCategories();
        BLAcquisitionTypes acquisitionTypesBL = new BLAcquisitionTypes();
        BLEditorials editorialsBL = new BLEditorials();
        BLAuthors authorsBL = new BLAuthors();
        BLEditions editionsBL = new BLEditions();
        BLCountries countriesBL = new BLCountries();
        BLCatalogs catalogsBL = new BLCatalogs();

        public async Task<IActionResult> Index(Books pBooks, int page = 1, int pageSize = 5)
        {
            if (pBooks == null)
                pBooks = new Books();

            // Si Top_Aux es -1, no se aplica límite (se usa para "Todos")
            if (pBooks.Top_Aux == -1)
                pBooks.Top_Aux = 0; // 0 significa "sin límite" en tu capa de negocio

            var allBooks = await booksBL.GetIncludePropertiesAsync(pBooks);
            allBooks = allBooks.OrderBy(b => b.BOOK_ID).ToList();

            // Aplica la paginación manualmente, ignorando Top_Aux
            int totalRegistros = allBooks.Count();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize);

            Console.WriteLine($"Total de registros: {totalRegistros}, Total de páginas: {totalPaginas}, Página actual: {page}");

            var books = allBooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = pageSize;
            ViewBag.ShowMenu = true;
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
            ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
            ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
            ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
            ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();

            return View(books);
        }



        // GET: BooksController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Falta terminar 
            var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });
            //Objeto anonimo
            books.Categories = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });
            books.AcquisitionTypes = await acquisitionTypesBL.GetAcquisitionTypesByIdAsync(new AcquisitionTypes { ACQUISITION_ID = books.ID_ACQUISITION });
            books.Editorials = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
            books.Authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });
            books.Editions = await editionsBL.GetEditionsByIdAsync(new Editions { EDITION_ID = books.ID_EDITION });
            books.Countries = await countriesBL.GetCountriesByIdAsync(new Countries { COUNTRY_ID = books.ID_COUNTRY });
            books.Catalogs = await catalogsBL.GetCatalogsByIdAsync(new Catalogs { CATALOG_ID = books.ID_CATALOG });

            ViewBag.ShowMenu = true;
            return View(books);
        }

        // GET: BooksController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
            ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
            ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
            ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
            ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View();
        }

        // POST: BooksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Books pBooks, IFormFile coverImage)
        {
            try
            {
                if (coverImage != null && coverImage.Length > 0)
                {
                    // Lógica para guardar la imagen en el sistema de archivos y obtener la ruta
                    string imagePath = SaveCoverImage(coverImage);

                    // Almacena la ruta en la propiedad de la entidad
                    pBooks.CoverImagePath = imagePath;
                }
                else
                {
                    pBooks.CoverImagePath = "SinPortada";
                }

                int result = await booksBL.CrearAsync(pBooks);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Maneja los errores...
                ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
                ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
                ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
                ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
                ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
                ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
                ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        private string SaveCoverImage(IFormFile coverImage)
        {
            // Lógica para guardar la imagen y obtener la ruta
            var uploadDir = Path.Combine("C:\\", "ImagenesBiblioteca");

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + coverImage.FileName;
            var filePath = Path.Combine(uploadDir, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                coverImage.CopyTo(fileStream);
            }

            return Path.Combine("/ImagenesBiblioteca", uniqueFileName);
        }



        // GET: BooksController/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });
            books.CoverImagePath = books.COVER; // Asigna la ruta de la imagen actual a CoverImagePath

            var editorial = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
            var autor = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });
            var pais = await countriesBL.GetCountriesByIdAsync(new Countries { COUNTRY_ID = books.ID_COUNTRY });
            var edicion = await editionsBL.GetEditionsByIdAsync(new Editions { EDITION_ID = books.ID_EDITION });

            ViewBag.EditorialName = editorial.EDITORIAL_NAME;
            ViewBag.AutorName = autor.AUTHOR_NAME;
            ViewBag.PaisName = pais.COUNTRY_NAME;
            ViewBag.EdicionName = edicion.EDITION_NUMBER;

            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
            ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
            ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
            ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
            ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;

            return View(books);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Books pBooks, IFormFile newcoverImage, string imageDeleted)
        {
            try
            {
                // Si se eliminó la imagen y no se subió una nueva, asigna "SinPortada"
                if (imageDeleted == "true" && (newcoverImage == null || newcoverImage.Length == 0))
                {
                    pBooks.COVER = "SinPortada";
                }
                // Si no se sube una nueva imagen, conserva la imagen actual
                else if (newcoverImage == null || newcoverImage.Length == 0)
                {
                    var existingBook = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });
                    pBooks.COVER = existingBook.COVER;
                }
                // Si se sube una nueva imagen, guárdala
                else
                {
                    string imagePath = SaveCoverImage(newcoverImage);
                    pBooks.COVER = imagePath;
                }

                int result = await booksBL.UpdateBooksAsync(pBooks);
                TempData["EditSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pBooks);
            }
        }





        // GET: BooksController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });

            books.Categories = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });

            books.AcquisitionTypes = await acquisitionTypesBL.GetAcquisitionTypesByIdAsync(new AcquisitionTypes { ACQUISITION_ID = books.ID_ACQUISITION });

            books.Editorials = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });

            books.Authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });

            books.Editions = await editionsBL.GetEditionsByIdAsync(new Editions { EDITION_ID = books.ID_EDITION });

            books.Countries = await countriesBL.GetCountriesByIdAsync(new Countries { COUNTRY_ID = books.ID_COUNTRY });

            books.Catalogs = await catalogsBL.GetCatalogsByIdAsync(new Catalogs { CATALOG_ID = books.ID_CATALOG });

            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View(books);
        }

        // POST: BooksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Books pBooks)
        {
            try
            {
                int result = await booksBL.DeleteBooksAsync(pBooks);
                TempData["DeleteSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pBooks);
            }
        }
        public async Task<JsonResult> BuscarAutor(Authors pAuthors)
        {
            List<Authors> lista = await authorsBL.GetAuthorsAsync(pAuthors);

            return Json(lista);
        }

        [HttpPost]
        public async Task<JsonResult> BuscarCiudad(Countries pCountries, int page = 1, int pageSize = 10)
        {
            List<Countries> listaCompleta = await countriesBL.GetCountriesAsync(pCountries);
            var listaPaginada = listaCompleta
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Json(new { Data = listaPaginada, TotalRecords = listaCompleta.Count });
        }


        public async Task<JsonResult> BuscarEdicion(Editions pEditions)
        {
            List<Editions> lista = await editionsBL.GetEditionsAsync(pEditions);

            return Json(lista);
        }
        public async Task<JsonResult> BuscarEditorial(Editorials pEditorials)
        {
            List<Editorials> lista = await editorialsBL.GetEditorialsAsync(pEditorials);

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> BuscarLibro(string titulo)
        {
            var libros = await DALBooks.GetBooksByTitleAsync(titulo);

            // Proyección para evitar problemas de serialización y enviar solo datos necesarios
            var resultado = libros.Select(b => new
            {
                booK_ID = b.BOOK_ID,
                title = b.TITLE,
                authorName = b.Authors != null ? b.Authors.AUTHOR_NAME : "N/A",
                editorialName = b.Editorials != null ? b.Editorials.EDITORIAL_NAME : "N/A"
            }).ToList();

            return Json(resultado);
        }

        [HttpGet]
        public async Task<JsonResult> LibrosPorCategoria()
        {
            var libros = await booksBL.GetIncludePropertiesAsync(new Books());

            var resultado = libros
                .Where(b => b.Categories != null)
                .GroupBy(b => b.Categories.CATEGORY_NAME)
                .Select(g => new
                {
                    categoria = g.Key,
                    cantidad = g.Count()
                }).ToList();

            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookSuggestions(string query)
        {
            var books = await DALBooks.GetBooksByTitleAsync(query);
            var suggestions = books
                .Select(b => b.TITLE)
                .Distinct()
                .Take(10)
                .ToList();

            return Json(suggestions);
        }

    }
}
