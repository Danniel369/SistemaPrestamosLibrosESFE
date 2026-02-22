using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories
{
    public class DALLoanDates
    {
        public static async Task<int> CreateLoandDateAsync(LoanDates pLoandDates)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pLoandDates);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateLoandDatesAsync(LoanDates pLoandDates)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var loanDates = await dbContext.Loan_Dates.FirstOrDefaultAsync(s => s.LOAN_DATE_ID == pLoandDates.LOAN_DATE_ID);
                loanDates.START_DATE = pLoandDates.START_DATE;
                loanDates.END_DATE = pLoandDates.END_DATE;
                dbContext.Update(loanDates);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        //Metodo para obtener el ultimo registro de fechas
        public static async Task<LoanDates> GetLastDateAsync(LoanDates pLoanDates)
        {
            var bdContexto = new DBContext();

            var lastDates = bdContexto.Loan_Dates.OrderByDescending(x => x.LOAN_DATE_ID).FirstOrDefault();

            return lastDates;
        }

        public static async Task<List<LoanDates>> GetAllLoanDatesAsync()
        {
            var loanDates = new List<LoanDates>();
            using (var dbContext = new DBContext())
            {
                loanDates = await dbContext.Loan_Dates.ToListAsync();
            }
            return loanDates;
        }
        /// <summary>
        /// Obtener fechas ya expiradas
        /// </summary>
        /// <returns></returns>
        public static async Task<List<LoanDates2>> GetExpiredDatesAsync()
        {
            var loanDates = new List<LoanDates2>();
            using (var dbContext = new DBContext())
            {
                
                loanDates = await dbContext.Loan_Dates
                    .Where(d=>d.END_DATE < DateTime.Now && d.STATUS == 1)
                        .Include(x=>x.Loans)
                        .Select(x => new LoanDates2 {ID_LOAN = x.ID_LOAN, START_DATE = x.START_DATE, END_DATE = x.END_DATE})
                        .ToListAsync();
            }
            return loanDates;
        }
        /// <summary>
        /// Obtener las fechas expiradas de un prestamo
        /// </summary>
        /// <param name="LoanId"></param>
        /// <returns></returns>
        public static async Task<List<LoanDates2>> GetExpiredDatesByIdLoanAsync(Loans pLoan)
        {
            var loanDates = new List<LoanDates2>();
            using (var dbContext = new DBContext())
            {
                
                loanDates = await dbContext.Loan_Dates
                    .Where(d=>d.END_DATE < DateTime.Now && d.STATUS == 1 && d.ID_LOAN == pLoan.LOAN_ID)
                        .Include(x=>x.Loans)
                        .Select(x => new LoanDates2 {ID_LOAN = x.ID_LOAN, START_DATE = x.START_DATE, END_DATE = x.END_DATE})
                        .ToListAsync();
            }
            return loanDates;
        }
        /// <summary>
        /// Obtener fechas que expiraran pronto
        /// </summary>
        /// <returns></returns> <summary>
        public static async Task<List<LoanDates2>> GetDatesToExpireSoonAsync()
        {
            var loanDates = new List<LoanDates2>();
            var dates2expire = new List<LoanDates2>();
            using (var dbContext = new DBContext())
            {

                loanDates = await dbContext.Loan_Dates
                    .Where(d => d.END_DATE > DateTime.Now && d.STATUS == Convert.ToByte(1))
                    .Include(x => x.Loans)
                    .Select(x => new LoanDates2 {ID_LOAN = x.ID_LOAN, START_DATE = x.START_DATE, END_DATE = x.END_DATE})
                    .ToListAsync(); // Traer los datos desde la base de datos

                foreach (var date in loanDates)
                {
                    var daysRemaining = (date.END_DATE - DateTime.Now).TotalDays;
                    if(daysRemaining > 0 && daysRemaining <= 3)
                    {
                        dates2expire.Add(new LoanDates2 
                            {
                                ID_LOAN = date.ID_LOAN, 
                                START_DATE = date.START_DATE,
                                END_DATE = date.END_DATE
                            }
                        );
                    }
                    
                }

                return dates2expire;
            }
        }


        public static async Task<LoanDates> GetLoanDatesByIdAsync(LoanDates pLoanDates)
        {
            var loanDates = new LoanDates();
            using (var dbContext = new DBContext())
            {
                loanDates = await dbContext.Loan_Dates.FirstOrDefaultAsync(s => s.LOAN_DATE_ID == pLoanDates.LOAN_DATE_ID);
            }
            return loanDates;
        }
        public static async Task<List<LoanDates2>> GetLoanDatesByIdLoanAsync(LoanDates pLoanDates)
        {
            //var loanDates = new List<LoanDates>();

            List<LoanDates2> loanDates;
            try
            {
                using (var dbContext = new DBContext())
                {
                    if (pLoanDates.ID_LOAN == 0 && pLoanDates.LOANSTEACHER_ID > 0)
                    {
                        loanDates = await dbContext.Loan_Dates.Where(s => s.ID_LOAN == pLoanDates.LOANSTEACHER_ID && s.LOANSTEACHER_ID == pLoanDates.LOANSTEACHER_ID)
                        .Include(x => x.Loans)
                        .Select(x => new LoanDates2 { ID_LOAN = x.ID_LOAN, START_DATE = x.START_DATE, END_DATE = x.END_DATE })
                        .ToListAsync();

                    }
                    else {
                        loanDates = await dbContext.Loan_Dates.Where(s => s.ID_LOAN == pLoanDates.ID_LOAN && s.LOANSTEACHER_ID == null)
                        .Include(x => x.Loans)
                        .Select(x => new LoanDates2 { ID_LOAN = x.ID_LOAN, START_DATE = x.START_DATE, END_DATE = x.END_DATE })
                        .ToListAsync();
                    }

                    //loanDates = await dbContext.Loan_Dates.Where(s => s.ID_LOAN == pLoanDates.ID_LOAN)
                    //    .Include(x=>x.Loans)
                    //    .Select(x => new LoanDates2 {ID_LOAN = x.ID_LOAN, START_DATE = x.START_DATE, END_DATE = x.END_DATE})
                    //    .ToListAsync();
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return loanDates;
        }

    }

}
