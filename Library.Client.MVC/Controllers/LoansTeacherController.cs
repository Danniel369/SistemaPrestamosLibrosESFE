using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Library.Client.MVC.services;

// Alias para evitar conflictos
using TeacherDomain = Library.DataAccess.Domain.LoansTeacher;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class LoansTeacherController : Controller
    {
        private readonly BLTeacher teacherBL = new BLTeacher();
        private readonly BLLoanTypes loansTypesBL = new();
        private readonly BLReservationStatus reservationStatusBL = new();
        private readonly BLBooks booksBL = new();
        private readonly BLCategories categoriesBL = new();
        private readonly BLLoanDates loanDatesBL = new();
        private readonly LoanService _loanService;

        public LoansTeacherController(LoanService loanService)
        {
            _loanService = loanService;
        }

        // =====================================================
        // INDEX
        // =====================================================
        public async Task<IActionResult> Index()
        {
            // 🔹 Préstamos con includes
            var teacherList = await teacherBL.GetIncludePropertiesAsync(null);

            // 🔹 Fechas de préstamo
            var loanDates = await loanDatesBL.GetAllLoanDatesAsync();

            /*
             * IMPORTANTE:
             * El diccionario DEBE usar PERSONAL_ID
             * porque eso es lo que usa la vista
             */
            var loanDatesDict = loanDates
                .GroupBy(l => l.ID_LOAN)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var loan = g.First();
                        bool isOverdue = loan.END_DATE < DateTime.Now && loan.STATUS == 1;
                        return (loan.END_DATE, isOverdue);
                    });

            ViewBag.LoanDatesList = loanDatesDict;
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.ShowMenu = true;

            return View("~/Views/LoansTeacher/Index.cshtml", teacherList);
        }

        // =====================================================
        // CREATE (GET)
        // =====================================================
        public async Task<IActionResult> Create(Books pBooks)
        {
            await CargarViewBags(pBooks);
            ViewBag.ShowMenu = true;
            return View("~/Views/LoansTeacher/Create.cshtml");
        }

        // =====================================================
        // CREATE (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TeacherDomain pTeacher,
            DateTime? fechaInicio,
            DateTime? fechaCierre)
        {
            try
            {
                // Limpiar navegación
                ModelState.Remove("LoanTypes");
                ModelState.Remove("ReservationStatus");
                ModelState.Remove("Books");

                // Validaciones manuales
                if (pTeacher.PERSONAL_ID <= 0)
                    ModelState.AddModelError("", "Debe seleccionar un docente.");

                if (pTeacher.ID_BOOK <= 0)
                    ModelState.AddModelError("", "Debe seleccionar un libro.");

                if (pTeacher.ID_TYPE <= 0)
                    ModelState.AddModelError("", "Debe seleccionar el tipo de préstamo.");

                if (string.IsNullOrWhiteSpace(pTeacher.EMAIL))
                    ModelState.AddModelError("", "El correo es obligatorio.");

                if (!fechaInicio.HasValue || !fechaCierre.HasValue)
                    ModelState.AddModelError("", "Debe seleccionar las fechas.");

                if (!ModelState.IsValid)
                {
                    await CargarViewBags(new Books { BOOK_ID = pTeacher.ID_BOOK });
                    return View(pTeacher);
                }

                // Datos automáticos
                pTeacher.REGISTRATION_DATE = DateTime.Now;
                pTeacher.END_DATE = fechaCierre.Value;
                pTeacher.STATUS = true;
                pTeacher.ID_RESERVATION = 1;

                // Validar existencias
                var currentBook = await booksBL.GetBooksByIdAsync(
                    new Books { BOOK_ID = pTeacher.ID_BOOK });

                if (currentBook == null || currentBook.EXISTENCES <= 0)
                {
                    TempData["Alerta"] = "No hay suficientes ejemplares.";
                    await CargarViewBags(new Books { BOOK_ID = pTeacher.ID_BOOK });
                    return View(pTeacher);
                }

                // Guardar préstamo
                long newLoanId = await teacherBL.CreateTeacherAsync(pTeacher);

                if (newLoanId <= 0)
                    throw new Exception("No se pudo crear el prestamo.");


                // 🔽 DESCONTAR STOCK
                currentBook.EXISTENCES -= 1;
                await booksBL.UpdateBooksAsync(currentBook);

                TempData["Alerta"] = "Prestamo realizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                await CargarViewBags(new Books { BOOK_ID = pTeacher.ID_BOOK });
                return View(pTeacher);
            }
        }

        // =====================================================
        // VIEWBAGS
        // =====================================================
        private async Task CargarViewBags(Books pBooks)
        {
            ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
        }

        // GET: LoansTeacher/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var loanTeacher = await teacherBL.GetLoanTeacherByIdAsync(id);

            if (loanTeacher == null)
                return NotFound();

            ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.LoanDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(
                new LoanDates { ID_LOAN = id }
            );

            var libro = await booksBL.GetBooksByIdAsync(
                new Books { BOOK_ID = loanTeacher.ID_BOOK }
            );

            ViewBag.TituloB = libro?.TITLE;
            ViewBag.Portada = libro?.COVER;
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;

            return View(loanTeacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    long id,
    LoansTeacher pLoan,
    DateTime? fechaInicio,
    DateTime? fechaCierre,
    LoanDates pLoanDates
)
        {
            try
            {
                // 🔴 VALIDACIÓN DE FECHAS INCOMPLETAS
                if (fechaInicio.HasValue != fechaCierre.HasValue)
                {
                    TempData["ErrorMessage"] = "Debes asignar ambas fechas.";
                    return RedirectToAction(nameof(Edit), new { id });
                }

                // 🔴 VALIDAR FECHAS
                if (fechaInicio.HasValue && fechaCierre.HasValue)
                {
                    if (fechaInicio.Value.Date >= fechaCierre.Value.Date)
                    {
                        TempData["ErrorMessage"] = "La fecha inicio debe ser menor a la fecha cierre.";
                        return RedirectToAction(nameof(Edit), new { id });
                    }

                    var existingDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(
                        new LoanDates { ID_LOAN = id }
                    );

                    if (existingDates.Any())
                    {
                        var maxEndDate = existingDates.Max(d => d.END_DATE);
                        if (fechaCierre.Value.Date <= maxEndDate.Date)
                        {
                            TempData["ErrorMessage"] =
                                $"La fecha cierre debe ser mayor a {maxEndDate:dd/MM/yyyy}";
                            return RedirectToAction(nameof(Edit), new { id });
                        }
                    }

                    // 🔴 GUARDAR FECHAS
                    pLoanDates.ID_LOAN = id;
                    pLoanDates.START_DATE = fechaInicio.Value;
                    pLoanDates.END_DATE = fechaCierre.Value;
                    pLoanDates.STATUS = 1;

                    await loanDatesBL.CreateLoanDatesAsync(pLoanDates);
                }

                // 🔴 ACTUALIZAR PRÉSTAMO DOCENTE
                await teacherBL.UpdateLoansTeacherAsync(pLoan);

                // 🔽 SI SE DEVUELVE, SUBIR STOCK
                if (pLoan.STATUS == false)   // o == 0 si usas byte
                {
                    var libro = await booksBL.GetBooksByIdAsync(
                        new Books { BOOK_ID = pLoan.ID_BOOK }
                    );

                    libro.EXISTENCES += 1;
                    await booksBL.UpdateBooksAsync(libro);
                }

                TempData["Alerta"] = "El prestamo fue actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.ShowMenu = true;

                ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
                ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
                ViewBag.LoanDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(
                    new LoanDates { ID_LOAN = id }
                );

                var libro = await booksBL.GetBooksByIdAsync(
                    new Books { BOOK_ID = pLoan.ID_BOOK }
                );

                ViewBag.TituloB = libro?.TITLE;
                ViewBag.Portada = libro?.COVER;

                return View(pLoan);
            }
        }









    }
}
