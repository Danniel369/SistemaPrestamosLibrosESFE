using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.DataAccess.Domain;
using Library.Client.MVC.services;
using Library.BusinessRules;
using Library.Client.MVC.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class LoansController : Controller
    {
        BLLoans loansBL = new BLLoans();
        BLLoanTypes loansTypesBL = new BLLoanTypes();
        BLReservationStatus reservationStatusBL = new BLReservationStatus();
        BLBooks booksBL = new BLBooks();
        BLCategories categoriesBL = new BLCategories();
        BLLoanDates loanDatesBL = new BLLoanDates();
        private readonly LoanService _loanService;

        public LoansController(LoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<IActionResult> Index(Loans pLoans = null, string studentCode = "", int page = 1, int pageSize = 10)
        {
            if (pLoans == null)
                pLoans = new Loans();

            // Lógica para buscar el ID_LENDER por código de estudiante
            if (!string.IsNullOrEmpty(studentCode))
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/code/{studentCode}");
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (student != null)
                        {
                            pLoans.ID_LENDER = student.Id;
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "No se pudo buscar el estudiante: " + ex.Message;
                    }
                }
            }

            var loans = await loansBL.GetIncludePropertiesAsync(pLoans);
            loans = loans.Where(l => l.ID_RESERVATION != 2 && l.ID_RESERVATION != 5 && l.STATUS == true).ToList();
            loans = loans.OrderBy(l => l.LOAN_ID).ToList();

            var loanDatesList = new Dictionary<long, (DateTime? LatestEndDate, bool IsOverdue)>();
            foreach (var loan in loans)
            {
                var dates = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = loan.LOAN_ID });
                var latestDate = dates?.OrderByDescending(ld => ld.END_DATE).FirstOrDefault();
                var isOverdue = latestDate != null && latestDate.END_DATE.Date < DateTime.Now.Date; // Cambio aquí
                loanDatesList[loan.LOAN_ID] = (latestDate?.END_DATE, isOverdue);
            }
            ViewBag.LoanDatesList = loanDatesList;

            // Aplicar paginación
            int totalRegistros = loans.Count();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize);
            var loansPaginados = loans
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = pageSize;
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewData["studentCode"] = studentCode;

            return View(loansPaginados);
        }

        // GET: BooksController/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");
            var loans = await loansBL.GetLoansByIdAsync(new Loans { LOAN_ID = id });
            if (loans == null)
                return NotFound($"Préstamo con id {id} no encontrado.");
            ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.LoanDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = id });

            var titulo = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = loans.ID_BOOK });
            ViewBag.TituloB = titulo?.TITLE;
            ViewBag.Portada = titulo?.COVER;
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View(loans);
        }

        // POST: BooksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Loans pLoans, DateTime? fechaInicio, DateTime? fechaCierre, LoanDates pLoanDates, Books2 pBooks)
        {
            try
            {

                // Actualizar existencias si aplica
                if (pLoans.ID_RESERVATION == 2 || pLoans.ID_RESERVATION == 5)
                {
                    var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = pLoans.ID_BOOK });
                    pBooks.BOOK_ID = pLoans.ID_BOOK;
                    pBooks.EXISTENCES = books.EXISTENCES + 1;
                    await booksBL.UpdateExistencesBooksAsync(pBooks);
                }

                // Actualizar estado del préstamo
                pLoans.STATUS = !(pLoans.STATUS == false && (pLoans.ID_RESERVATION == 2 || pLoans.ID_RESERVATION == 5));

                // Validar si se asignaron fechas incompletas
                if (fechaInicio.HasValue != fechaCierre.HasValue)
                {
                    TempData["ErrorMessage"] = "Debes asignar tanto la fecha de inicio como la de cierre.";
                    return RedirectToAction(nameof(Edit), new { id = id });
                }

                // Si se asignaron fechas, validar que sean correctas
                if (fechaInicio.HasValue && fechaCierre.HasValue)
                {
                    if (fechaInicio.Value.Date >= fechaCierre.Value.Date)
                    {
                        TempData["ErrorMessage"] = "La fecha de inicio debe ser menor que la fecha de cierre.";
                        return RedirectToAction(nameof(Edit), new { id = id });
                    }

                    // Validar contra la fecha máxima registrada
                    var existingDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = id });
                    if (existingDates != null && existingDates.Any())
                    {
                        var maxEndDate = existingDates.Max(d => d.END_DATE);
                        if (fechaCierre.Value.Date <= maxEndDate.Date)
                        {
                            TempData["ErrorMessage"] = $"La fecha de cierre debe ser mayor que la fecha maxima registrada: {maxEndDate.ToShortDateString()}";
                            return RedirectToAction(nameof(Edit), new { id = id });
                        }
                    }

                    // Asignar fechas al préstamo
                    pLoanDates.ID_LOAN = id;
                    pLoanDates.START_DATE = fechaInicio.Value;
                    pLoanDates.END_DATE = fechaCierre.Value;
                    pLoanDates.STATUS = 1;

                    // Ajustar estado de la reserva si aplica
                    if (pLoans.ID_RESERVATION == 1)
                        pLoans.ID_RESERVATION = 4;
                    else if (pLoans.ID_RESERVATION == 3)
                        pLoans.ID_RESERVATION = 1;

                    await loanDatesBL.CreateLoanDatesAsync(pLoanDates);
                }
                else
                {
                    // No se asignaron fechas -> revisar si ya existen
                    var existingDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = id });

                    if (existingDates == null || !existingDates.Any())
                    {
                        // Si no hay fechas registradas, validar según el tipo de préstamo
                        if (pLoans.ID_RESERVATION != 2 && pLoans.ID_RESERVATION != 5)
                        {
                            TempData["ErrorMessage"] = "Debes asignar fechas para este tipo de prestamo.";
                            return RedirectToAction(nameof(Edit), new { id = id });
                        }
                    }
                    // Si ya existen fechas, se permite cambiar solo el estado
                }

                // Actualizar el préstamo
                await loansBL.UpdateLoansAsync(pLoans);
                TempData["Alerta"] = "El prestamo se ha modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.ShowMenu = true;
                // Repoblar los ViewBag
                ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
                ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
                ViewBag.LoanDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = pLoans.LOAN_ID });
                var titulo = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = pLoans.ID_BOOK });
                ViewBag.TituloB = titulo?.TITLE;
                ViewBag.Portada = titulo?.COVER;
                return View(pLoans);
            }
        }

        // Resto de los métodos del controlador se mantienen igual
        public async Task<IActionResult> Status2Loans(Books pBooks, Loans pLoans = null)
        {
            if (pLoans == null)
                pLoans = new Loans();
            if (pLoans.Top_Aux == 0)
                pLoans.Top_Aux = 10;
            else if (pLoans.Top_Aux == -1)
                pLoans.Top_Aux = 0;

            var loans = await loansBL.GetIncludePropertiesAsync(pLoans);
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Loans = await loansBL.GetAllLoansAsync();
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);

            ViewBag.Top = pLoans.Top_Aux;
            ViewBag.ShowMenu = true;
            return View(loans);
        }

        public async Task<IActionResult> LoansDelite(Books pBooks, Loans pLoans = null, string studentCode = "", int page = 1, int pageSize = 20)
        {
            if (pLoans == null)
                pLoans = new Loans();

            // Si se ingresó un código de estudiante, buscar su ID_LENDER
            if (!string.IsNullOrEmpty(studentCode))
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/code/{studentCode}");
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (student != null)
                        {
                            pLoans.ID_LENDER = student.Id; // Asigna el ID del estudiante encontrado
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "No se pudo buscar el estudiante: " + ex.Message;
                    }
                }
            }

            var loans = await loansBL.GetIncludePropertiesAsync(pLoans);
            loans = loans.Where(l => l.STATUS == false).ToList(); // Solo préstamos eliminados
            loans = loans.OrderBy(l => l.LOAN_ID).ToList();

            // Aplicar paginación
            int totalRegistros = loans.Count();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize);
            var loansPaginados = loans
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = pageSize;
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);
            ViewData["studentCode"] = studentCode; // Para que el campo no se borre al recargar

            return View(loansPaginados);
        }

        // GET: BooksController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var loans = await loansBL.GetLoansByIdAsync(new Loans { LOAN_ID = id });
            loans.LoanTypes = await loansTypesBL.GetLoanTypesByIdAsync(new LoanTypes { TYPES_ID = loans.ID_TYPE });
            loans.ReservationStatus = await reservationStatusBL.GetReservationStatusByIdAsync(new ReservationStatus { RESERVATION_ID = loans.ID_RESERVATION });
            loans.Books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = loans.ID_BOOK });
            ViewBag.ShowMenu = true;
            return View(loans);
        }

        // GET: BooksController/Create
        public async Task<IActionResult> Create(Books pBooks)
        {
            ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    Loans pLoans,
    DateTime? fechaInicio,
    DateTime? fechaCierre)
        {
            // 🔹 Limpiar validaciones que no vienen del formulario
            ModelState.Remove("LoanTypes");
            ModelState.Remove("ReservationStatus");
            ModelState.Remove("Books");
            ModelState.Remove("LoanDates");

            // 🔹 Validaciones manuales
            if (pLoans.ID_LENDER <= 0)
                ModelState.AddModelError("", "Debe seleccionar un estudiante.");

            if (pLoans.ID_BOOK <= 0)
                ModelState.AddModelError("", "Debe seleccionar un libro.");

            if (pLoans.ID_TYPE <= 0)
                ModelState.AddModelError("", "Debe seleccionar tipo de préstamo.");

            if (!fechaInicio.HasValue || !fechaCierre.HasValue)
                ModelState.AddModelError("", "Debe seleccionar fecha de inicio y cierre.");

            if (fechaInicio >= fechaCierre)
                ModelState.AddModelError("", "La fecha de inicio debe ser menor a la fecha de cierre.");


            if (!ModelState.IsValid)
            {
                ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
                ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
                ViewBag.ShowMenu = true;
                return View(pLoans);
            }

            // 🔹 DATOS DEL PRÉSTAMO
            pLoans.REGISTRATION_DATE = DateTime.Now;
            pLoans.STATUS = true;
            pLoans.ID_RESERVATION = 1; // Activo
            pLoans.COPY = 1;

            // 🔽 Reducir el stock
            var libro = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = pLoans.ID_BOOK });

            if (libro.EXISTENCES <= 1)
            {
                TempData["ErrorMessage"] = "No hay existencias disponibles para este libro.";
                return RedirectToAction(nameof(Create));
            }

            Books2 actualizarLibro = new Books2
            {
                BOOK_ID = libro.BOOK_ID,
                EXISTENCES = libro.EXISTENCES - 1
            };

            await booksBL.UpdateExistencesBooksAsync(actualizarLibro);

            // 1️⃣ Guardar préstamo
            await loansBL.CreateLoansAsync(pLoans);



            // 2️⃣ Guardar fechas
            LoanDates loanDates = new LoanDates
            {
                ID_LOAN = pLoans.LOAN_ID,
                ////LOANSTEACHER_ID = pLoans.LOAN_ID,
                START_DATE = fechaInicio.Value,
                END_DATE = fechaCierre.Value,
                STATUS = 1
            };

            await loanDatesBL.CreateLoanDatesAsync(loanDates);

            // ✅ Todo bien
            TempData["Alerta"] = "Prestamo registrado correctamente";
            return RedirectToAction(nameof(Index));
        }


        public async Task<JsonResult> BuscarLibros(Books pBooks)
        {
            List<Books> lista = await booksBL.GetBooksAsync(pBooks);
            return Json(lista);
        }

        public async Task<JsonResult> ObtenerCategoria(Categories pCategories)
        {
            List<Categories> lista = await categoriesBL.GetCategoriesAsync(pCategories);
            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> BuscarEstudiante(string codigo)
        {
            List<Student> StudentList = new List<Student>();
            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/code/{codigo}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (student != null)
                        {
                            StudentList.Add(student);
                        }
                    }
                }
                catch { }
            }
            return Json(StudentList);
        }

        public async Task<JsonResult> BuscarEstudianteId(long id)
        {
            Student student = new Student();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }

            if (student != null)
            {
                return Json(new
                {
                    studentCode = student.StudentCode,
                    fullName = $"{student.StudentName} {student.StudentLastName}",
                    career = student.Career?.CareerName ?? "Desconocido"
                });
            }
            return Json(new { studentCode = "No disponible", fullName = "", career = "" });
        }

        public async Task<IActionResult> Delite(long id)
        {
            var loans = await loansBL.GetLoansByIdAsync(new Loans { LOAN_ID = id });
            var titulo = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = loans.ID_BOOK });
            var tituloB = titulo.TITLE;
            ViewBag.TituloB = tituloB;
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View(loans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delite(int id, Loans2 pLoans)
        {
            try
            {
                pLoans.STATUS = false;
                int result = await loansBL.UpdateLoans02Async(pLoans);
                return RedirectToAction(nameof(Status2Loans));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pLoans);
            }
        }

        [HttpGet]
        public async Task<JsonResult> PrestamosPorTipo()
        {
            var prestamos = await loansBL.GetAllLoansAsync();
            var tipos = await loansTypesBL.GetAllLoanTypesAsync();
            var resultado = prestamos
                .GroupBy(p => p.ID_TYPE)
                .Select(g => new
                {
                    Tipo = tipos.FirstOrDefault(t => t.TYPES_ID == g.Key)?.TYPES_NAME ?? "Desconocido",
                    Cantidad = g.Count()
                }).ToList();
            return Json(resultado);
        }

        [HttpGet]
        public async Task<JsonResult> PrestamosPorSemestre()
        {
            var prestamos = await loansBL.GetAllLoansAsync();
            var currentYear = DateTime.Now.Year;
            int ciclo1 = prestamos.Count(p => p.REGISTRATION_DATE.Year == currentYear && p.REGISTRATION_DATE.Month >= 1 && p.REGISTRATION_DATE.Month <= 6);
            int ciclo2 = prestamos.Count(p => p.REGISTRATION_DATE.Year == currentYear && p.REGISTRATION_DATE.Month >= 7 && p.REGISTRATION_DATE.Month <= 12);
            var resultado = new[]
            {
              new { ciclo = "Ciclo 1", cantidad = ciclo1 },
              new { ciclo = "Ciclo 2", cantidad = ciclo2 }
            };
            return Json(resultado);
        }

        public async Task<IActionResult> AllLoans(Books pBooks, Loans pLoans = null)
        {
            if (pLoans == null)
                pLoans = new Loans();

            var loans = await loansBL.GetIncludePropertiesAsync(pLoans);
            loans = loans.OrderBy(l => l.LOAN_ID).ToList();

            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Loans = await loansBL.GetAllLoansAsync();
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);
            ViewBag.ShowMenu = true;

            return View(loans);
        }


        [HttpGet]
        public async Task<JsonResult> PrestamosPorMes()
        {
            var prestamos = await loansBL.GetAllLoansAsync();
            int anioActual = DateTime.Now.Year;
            var datos = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Mes = m,
                    Cantidad = prestamos.Count(p =>
                        p.REGISTRATION_DATE.Year == anioActual &&
                        p.REGISTRATION_DATE.Month == m)
                }).ToList();
            return Json(datos);
        }


        public async Task<IActionResult> LoansOverdue(Books pBooks, Loans pLoans = null, string studentCode = "")
        {
            if (pLoans == null)
                pLoans = new Loans();
            if (pLoans.Top_Aux == 0)
                pLoans.Top_Aux = 20;
            else if (pLoans.Top_Aux == -1)
                pLoans.Top_Aux = 0;

            if (!string.IsNullOrEmpty(studentCode))
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/code/{studentCode}");
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (student != null)
                        {
                            pLoans.ID_LENDER = student.Id;
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "No se pudo buscar el estudiante: " + ex.Message;
                    }
                }
            }

            var allLoans = await loansBL.GetIncludePropertiesAsync(pLoans);
            var overdueLoans = new List<Loans>();

            foreach (var loan in allLoans)
            {
                // Obtener la lista de fechas para este préstamo
                var loanDatesList = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = loan.LOAN_ID });

                // Si hay fechas registradas, tomar la más reciente (o la primera)
                if (loanDatesList.Any())
                {
                    var loanDates = loanDatesList.OrderByDescending(ld => ld.END_DATE).First();
                    if (loanDates.END_DATE < DateTime.Now && loan.ID_RESERVATION != 2 && loan.ID_RESERVATION != 5)
                    {
                        overdueLoans.Add(loan);
                    }
                }
            }

            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Loans = overdueLoans;
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);
            ViewBag.Top = pLoans.Top_Aux;
            ViewData["studentCode"] = studentCode;
            ViewBag.ShowMenu = true;

            return View(overdueLoans);
        }

    }
}