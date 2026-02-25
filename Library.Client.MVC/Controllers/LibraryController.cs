using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.DataAccess.Domain;
using Library.BusinessRules;

using Library.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    public class LibraryController : Controller
    {
        BLBooks booksBL = new BLBooks();
        DALBooks booksDAL = new DALBooks();
        BLCategories categoriesBL = new BLCategories();
        BLAcquisitionTypes acquisitionTypesBL = new BLAcquisitionTypes();
        BLEditorials editorialsBL = new BLEditorials();
        BLAuthors authorsBL = new BLAuthors();
        BLEditions editionsBL = new BLEditions();
        BLCountries countriesBL = new BLCountries();
        BLCatalogs catalogsBL = new BLCatalogs();
        BLLoanTypes loanTypesBL = new BLLoanTypes();
        BLLoans loansBL = new BLLoans();

        Books booksEN = new Books();
        public async Task<IActionResult> Index(Books pBooks = null, int page = 1, int pageSize = 12)
        {
            if (pBooks == null)
                pBooks = new Books();

            // Obtener libros paginados y el total de registros
            var (books, totalRecords) = await booksBL.GetPaginatedBooksAsync(pBooks, page, pageSize);
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Pasar datos a la vista
            ViewBag.books = await booksBL.GetAllBooksAsync();
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
            ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
            ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
            ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
            ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
            ViewBag.Top = pBooks.Top_Aux;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TitleFilter = pBooks.TITLE;
            ViewBag.CategoryFilter = pBooks.ID_CATEGORY;
            ViewBag.AuthorFilter = pBooks.ID_AUTHOR;
            ViewBag.EditorialFilter = pBooks.ID_EDITORIAL;

            return View(books);
        }



        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> CreateLoansSt(int id)
        {
            // 1. Obtener el libro y validar si existe
            var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });

            if (books == null)
            {
                // Si el libro no existe, redirigimos o mostramos error 404
                return NotFound();
            }

            // 2. Cálculo de préstamos (Optimizado)
            var reservations = new List<int> { 1, 3, 4 };
            int cantidadPrestamos = 0;

            foreach (var resId in reservations)
            {
                var result = await loansBL.GetLoansAsync(new Loans { ID_BOOK = id, ID_RESERVATION = resId, STATUS = true });
                if (result != null)
                {
                    cantidadPrestamos += result.Count;
                }
            }

            // 3. Lógica de alertas
            if (books.EJEMPLARS <= 0)
            {
                ViewBag.AlertaLibro2 = "No hay ejemplares disponibles para reservación";
            }
            else if (cantidadPrestamos < books.EJEMPLARS && books.EXISTENCES >= 1)
            {
                ViewBag.AlertaLibro = "Disponible para Reservacion";
            }
            else
            {
                ViewBag.AlertaLibro2 = "No hay suficientes ejemplares para realizar la reservacion";
            }

            // 4. Carga de datos relacionados con validación de nulos
            ViewBag.LoanTypes = await loanTypesBL.GetAllLoanTypesAsync();

            var editorial = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
            var categoria = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });
            var authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });

            // Usamos el operador condicional nulo (?) para evitar errores si las relaciones fallan
            ViewBag.Editorial = editorial?.EDITORIAL_NAME ?? "N/A";
            ViewBag.Categoria = categoria?.CATEGORY_NAME ?? "N/A";
            ViewBag.Autor = authors?.AUTHOR_NAME ?? "N/A";

            ViewBag.Imagen = books.COVER;
            ViewBag.Titulo = books.TITLE;
            ViewBag.Year = books.YEAR;

            return View(books);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLoansSt(int id, Books pBooks, string telefono, int tipoPrestamo, Loans pLoans, int result, Books2 pBooks2, long idLender)
        {
            try
            {
                var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });

                // Declarar variables una sola vez
                var editorial = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
                var categoria = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });
                var authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });

                // Validar si el estudiante actual ya tiene un préstamo activo para este libro
                var prestamoActivoEstudiante = await loansBL.GetLoansAsync(
                    new Loans
                    {
                        ID_LENDER = idLender,
                        STATUS = true,
                    }
                );

                if (prestamoActivoEstudiante.Count > 0)
                {
                    ViewBag.AlertaPrestamoEx = "Ya tienes un prestamo activo. No puedes realizar otro prestamo hasta devolver el actual.";
                    // Cargar datos comunes para vista
                    ViewBag.LoanTypes = await loanTypesBL.GetAllLoanTypesAsync();
                    ViewBag.Editorial = editorial.EDITORIAL_NAME;
                    ViewBag.Categoria = categoria.CATEGORY_NAME;
                    ViewBag.Autor = authors.AUTHOR_NAME;
                    ViewBag.Imagen = books.COVER;
                    ViewBag.Titulo = books.TITLE;
                    ViewBag.Year = books.YEAR;
                    return View();
                }


                if (telefono != null && tipoPrestamo > 0)
                {
                    var reservations = new List<int> { 1, 3, 4 };
                    var cantidadPrestamos = reservations
                        .Select(async reservation => await loansBL.GetLoansAsync(new Loans { ID_BOOK = id, ID_RESERVATION = reservation, STATUS = true }))
                        .Select(task => task.Result.Count)
                        .Sum();

                    if (cantidadPrestamos < books.EJEMPLARS && books.EXISTENCES > 0)
                    {
                        pLoans.LENDER_CONTACT = telefono;
                        pLoans.ID_BOOK = id;
                        pLoans.ID_LENDER = idLender;
                        pLoans.ID_TYPE = tipoPrestamo;
                        pLoans.STATUS = true;
                        pLoans.ID_RESERVATION = 3;
                        pLoans.USER_ID = 1;
                        pLoans.COPY = 1;
                        pLoans.FEE = 0;
                        pLoans.REGISTRATION_DATE = DateTime.Now;
                        result = await loansBL.CreateLoansAsync(pLoans);
                        pBooks2.BOOK_ID = pLoans.ID_BOOK;
                        pBooks2.EXISTENCES = books.EXISTENCES - 1;
                        int resultUpdate = await booksBL.UpdateExistencesBooksAsync(pBooks2);
                    }
                    else
                    {
                        ViewBag.Alerta2 = "No hay suficientes ejemplares para realizar el registro";
                    }
                }
                else
                {
                    ViewBag.Alerta = "Por favor ingrese los datos del Prestamo";
                }

                // Cargar datos comunes para vista en cualquier caso
                ViewBag.LoanTypes = await loanTypesBL.GetAllLoanTypesAsync();
                ViewBag.Editorial = editorial.EDITORIAL_NAME;
                ViewBag.Categoria = categoria.CATEGORY_NAME;
                ViewBag.Autor = authors.AUTHOR_NAME;
                ViewBag.Imagen = books.COVER;
                ViewBag.Titulo = books.TITLE;
                ViewBag.Year = books.YEAR;

                if (result > 0)
                {
                    TempData["Alerta"] = "La reservacion se registro exitosamente!!";
                }
                return View();
            }
            catch (Exception ex)
            {
                // Manejo de error
                var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });
                ViewBag.LoanTypes = await loanTypesBL.GetAllLoanTypesAsync();
                var editorial = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
                var categoria = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });
                var authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });
                ViewBag.Editorial = editorial.EDITORIAL_NAME;
                ViewBag.Categoria = categoria.CATEGORY_NAME;
                ViewBag.Autor = authors.AUTHOR_NAME;
                ViewBag.Imagen = books.COVER;
                ViewBag.Titulo = books.TITLE;
                ViewBag.Year = books.YEAR;
                ViewBag.Alerta = "Por favor ingrese los datos del Préstamo";
                ViewBag.Error = ex.Message;
                return View(pBooks);
            }
        }
    }
}
