using ClosedXML.Excel;
using Library.BusinessRules;
using Library.Client.MVC.services;
using Library.DataAccess.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
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
        public async Task<IActionResult> Index(int ID_TYPE = 0, int ID_RESERVATION = 0)
        {
            // 🔹 Préstamos con includes
            var teacherList = await teacherBL.GetIncludePropertiesAsync(null);
            // 2. Aplicar filtros de búsqueda
            if (ID_TYPE > 0)
            {
                teacherList = teacherList.Where(x => x.ID_TYPE == ID_TYPE).ToList();
            }

            if (ID_RESERVATION > 0)
            {
                teacherList = teacherList.Where(x => x.ID_RESERVATION == ID_RESERVATION).ToList();
            }

            // 🔹 Lógica de fechas (Se mantiene igual)

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
            ViewBag.SelectedType = ID_TYPE;
            ViewBag.SelectedReservation = ID_RESERVATION;

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
                // 1. Guardar el registro principal del préstamo
                long newLoanId = await teacherBL.CreateTeacherAsync(pTeacher);

                if (newLoanId <= 0)
                    throw new Exception("No se pudo crear el prestamo.");

                // 🌟 2. GUARDAR LA FECHA INICIAL (Esto es lo que falta)
                // Usamos los parámetros fechaInicio y fechaCierre que recibe el método Create
                await loanDatesBL.CreateLoanDatesAsync(new LoanDates
                {
                    ID_LOAN = newLoanId,
                    LOANSTEACHER_ID = newLoanId,
                    START_DATE = fechaInicio.Value,
                    END_DATE = fechaCierre.Value,
                    STATUS = 1
                });

                // 3. 🔽 DESCONTAR STOCK
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
        // Exportación en Excel
        // =====================================================
        public async Task<IActionResult> ExportarExcel()
        {
            // 1. Obtenemos la lista con todas las propiedades relacionadas (Includes)
            // Tal como lo haces en el Index
            var teacherList = await teacherBL.GetIncludePropertiesAsync(null);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Préstamos Docentes");

                // 2. Encabezados (Basados en tu tabla de la imagen)
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Tipo";
                worksheet.Cell(1, 3).Value = "Estado";
                worksheet.Cell(1, 4).Value = "Correo";
                worksheet.Cell(1, 5).Value = "Nombre";
                worksheet.Cell(1, 6).Value = "Rol";
                worksheet.Cell(1, 7).Value = "Libro";
                worksheet.Cell(1, 8).Value = "Status";

                // Estilo para el encabezado (Color verde de tu diseño)
                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E7940");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Font.Bold = true;

                // 3. Llenado de datos
                int row = 2;
                foreach (var item in teacherList.Where(x => x.STATUS)) // Solo activos
                {
                    worksheet.Cell(row, 1).Value = item.LOANSTEACHER_ID;
                    worksheet.Cell(row, 2).Value = item.LoanTypes?.TYPES_NAME ?? "N/A";
                    worksheet.Cell(row, 3).Value = item.ReservationStatus?.STATUS_NAME ?? "N/A";
                    worksheet.Cell(row, 4).Value = item.EMAIL ?? "No disponible";
                    worksheet.Cell(row, 5).Value = item.PERSONALNAME ?? "No disponible";
                    worksheet.Cell(row, 6).Value = item.ROL ?? "No asignado";
                    worksheet.Cell(row, 7).Value = item.Books?.TITLE ?? "Sin título";
                    worksheet.Cell(row, 8).Value = "ACTIVO";
                    row++;
                }

                // Ajuste automático de columnas
                worksheet.Columns().AdjustToContents();

                // 4. Generación del archivo para descarga
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Reporte_Prestamos_Docentes_{DateTime.Now:ddMMyyyy}.xlsx"
                    );
                }
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
            ViewBag.LoanDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates
            {
                //ID_LOAN = id,
                LOANSTEACHER_ID = id
            });

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
        public async Task<IActionResult> Edit(long id, LoansTeacher pLoan, DateTime? fechaInicio, DateTime? fechaCierre, LoanDates pLoanDates)
        {
            try
            {
                // 1. VALIDACIÓN DE FECHAS INCOMPLETAS
                if (fechaInicio.HasValue != fechaCierre.HasValue)
                {
                    TempData["ErrorMessage"] = "Debes asignar ambas fechas.";
                    return RedirectToAction(nameof(Edit), new { id });
                }

                // 2. LÓGICA DE FECHAS (Aquí evitamos los duplicados)
                if (fechaInicio.HasValue && fechaCierre.HasValue)
                {
                    if (fechaInicio.Value.Date >= fechaCierre.Value.Date)
                    {
                        TempData["ErrorMessage"] = "La fecha inicio debe ser menor a la fecha cierre.";
                        return RedirectToAction(nameof(Edit), new { id });
                    }

                    // Traer fechas actuales para comparar
                    var existingDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = id });

                    // 🛡️ NOVEDAD: Solo insertar si los valores han cambiado realmente
                    bool yaExisteEsaFecha = existingDates.Any(d =>
                        d.START_DATE.Date == fechaInicio.Value.Date &&
                        d.END_DATE.Date == fechaCierre.Value.Date);

                    if (!yaExisteEsaFecha)
                    {
                        // Si vas a insertar una nueva (extensión), validar que sea mayor a la anterior
                        if (existingDates.Any())
                        {
                            var maxEndDate = existingDates.Max(d => d.END_DATE);
                            if (fechaCierre.Value.Date <= maxEndDate.Date)
                            {
                                TempData["ErrorMessage"] = $"La nueva fecha cierre debe ser mayor a la anterior: {maxEndDate:dd/MM/yyyy}";
                                return RedirectToAction(nameof(Edit), new { id });
                            }
                        }

                        pLoanDates.ID_LOAN = id;
                        pLoanDates.LOANSTEACHER_ID = id;
                        pLoanDates.START_DATE = fechaInicio.Value;
                        pLoanDates.END_DATE = fechaCierre.Value;
                        pLoanDates.STATUS = 1;

                        await loanDatesBL.CreateLoanDatesAsync(pLoanDates);
                    }
                }

                // 3. ACTUALIZAR PRÉSTAMO DOCENTE
                await teacherBL.UpdateLoansTeacherAsync(pLoan);

                // 4. GESTIÓN DE STOCK (Mejorado para evitar sumar stock infinitamente)
                // Solo sumamos stock si el préstamo pasa de STATUS=true a STATUS=false
                var prestamoAntesDeGuardar = await teacherBL.GetLoanTeacherByIdAsync(id);
                if (prestamoAntesDeGuardar.STATUS == true && pLoan.STATUS == false)
                {
                    var libro = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = pLoan.ID_BOOK });
                    if (libro != null)
                    {
                        libro.EXISTENCES += 1;
                        await booksBL.UpdateBooksAsync(libro);
                    }
                }

                TempData["Alerta"] = "El prestamo fue actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // El bloque Catch se mantiene igual para repoblar la vista
                ViewBag.Error = ex.Message;
                ViewBag.ShowMenu = true;
                ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
                ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
                ViewBag.LoanDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = id });
                var libro = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = pLoan.ID_BOOK });
                ViewBag.TituloB = libro?.TITLE;
                ViewBag.Portada = libro?.COVER;
                return View(pLoan);
            }
        }
        public async Task<IActionResult> LoansDelete(string personalName = "", int page = 1, int pageSize =10)
        {
            // 1. Instanciamos el objeto para los includes
            var pLoans = new TeacherDomain();

            // 2. Obtener los préstamos con sus propiedades de navegación
            var loans = await teacherBL.GetIncludePropertiesAsync(pLoans);

            // 3. Filtrar solo los eliminados (STATUS == false)
            // Usamos .AsEnumerable() o .ToList() antes del filtro si GetIncludePropertiesAsync devuelve IQueryable
            var loansFiltrados = loans
                .Where(l => l.STATUS == false)
                .OrderByDescending(l => l.LOANSTEACHER_ID) // Los más recientes primero suele ser mejor
                .ToList();

            // 4. Aplicar filtro de búsqueda por nombre
            if (!string.IsNullOrEmpty(personalName))
            {
                loansFiltrados = loansFiltrados
                    .Where(l => l.PERSONALNAME != null &&
                                l.PERSONALNAME.Contains(personalName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // 5. Lógica de Paginación
            int totalRegistros = loansFiltrados.Count;
            // Manejo de pageSize = -1 (Mostrar todos)
            int actualPageSize = pageSize == -1 ? (totalRegistros > 0 ? totalRegistros : 1) : pageSize;

            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / actualPageSize);

            var loansPaginados = loansFiltrados
                .Skip((page - 1) * actualPageSize)
                .Take(actualPageSize)
                .ToList();

            // 6. Llenar ViewBags para la vista
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = pageSize; // Mantenemos el valor original (-1, 10, 20...)

            // Estos ViewBags son necesarios si tu vista tiene filtros desplegables de tipos o libros
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();

            ViewData["personalName"] = personalName;

            // Asegúrate de que la vista se llame LoansDeleted.cshtml
            return View("LoansDelete",loansPaginados);
        }
    }
}