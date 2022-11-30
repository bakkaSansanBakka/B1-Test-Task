using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Task2.Models;
using Aspose.Cells;
using Microsoft.Data.SqlClient;
using Task2.SqlCommandsHelpers;

namespace Task2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly string filePath;
        private readonly string currentDirectory = Directory.GetCurrentDirectory();
        private readonly string directoryUploaded = @"wwwroot\UploadedFiles";

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
            filePath = Path.Combine(currentDirectory, directoryUploaded);
        }

        public IActionResult Index()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var uploadedFilesDirectory = Path.Combine(currentDirectory, @"wwwroot\UploadedFiles");
            string[] files = Directory.GetFiles(uploadedFilesDirectory);
            List<FileViewModel> filesList = new List<FileViewModel>();
            foreach (var file in files)
            {
                filesList.Add(new FileViewModel { Name = Path.GetFileName(file) });
            }

            return View(filesList);
        }

        [HttpGet]
        public IActionResult ViewFile(string fileName)
        {
            DataViewModel dataViewModel = new DataViewModel();
            dataViewModel.Data = SqlHelper.GetData(fileName, connectionString);
            dataViewModel.OpeningBalanceAssetsGeneralSum = dataViewModel.Data.Sum(item => item.OpeningBalanceAssets);
            dataViewModel.OpeningBalanceLiabilitiesGeneralSum = dataViewModel.Data.Sum(item => item.OpeningBalanceLiabilities);
            dataViewModel.MoneyTurnoverDebitGeneralSum = dataViewModel.Data.Sum(item => item.MoneyTurnoverDebit);
            dataViewModel.MoneyTurnoverCreditGeneralSum = dataViewModel.Data.Sum(item => item.MoneyTurnoverCredit);
            dataViewModel.ClosingBalanceAssetsGeneralSum = dataViewModel.Data.Sum(item => item.ClosingBalanceAssets);
            dataViewModel.ClosingBalanceLiabilitiesGeneralSum = dataViewModel.Data.Sum(item => item.ClosingBalanceLiabilities);

            var fullPath = Path.Combine(filePath, fileName);
            var wb = new Workbook(fullPath);
            var worksheet = wb.Worksheets[0];

            dataViewModel.BankName = worksheet.Cells[0, 0].Value.ToString();
            dataViewModel.DocumentTitle = worksheet.Cells[1, 0].Value.ToString();
            dataViewModel.TimePeriod = worksheet.Cells[2, 0].Value.ToString();
            dataViewModel.Option = worksheet.Cells[3, 0].Value.ToString();
            dataViewModel.DateOfCreation = worksheet.Cells[5, 0].Value.ToString();
            dataViewModel.Currency = worksheet.Cells[5, 6].Value.ToString();

            return View(dataViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoadFiles()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoadFiles(IFormFile file)
        {
            var fileName = Path.GetFileName(file.FileName);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            using (FileStream stream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var fullPath = Path.Combine(filePath, fileName);
            var wb = new Workbook(fullPath);
            var worksheet = wb.Worksheets[0];

            var classId = 1;

            for (int i = 9; i < worksheet.Cells.MaxDataRow; i++)
            {
                var isInfoRow = false;

                int balanceAccountId = 0;
                decimal openingBalanceAssets = 0;
                decimal openingBalanceLiabilities = 0;
                decimal moneyTurnoverDebit = 0;
                decimal moneyTurnoverCredit = 0;

                for (int j = 0; j < 5; j++)
                {
                    if (worksheet.Cells[i, j].Value.ToString().ToUpper().StartsWith("КЛАСС"))
                    {
                        classId++;
                        isInfoRow = true;
                        break;
                    }
                    if (worksheet.Cells[i, 0].Value.ToString().ToUpper().StartsWith("ПО КЛАССУ") || int.Parse(worksheet.Cells[i, 0].Value.ToString()) < 1000)
                    {
                        isInfoRow = true;
                        break;
                    }
                    switch (j)
                    {
                        case 0:
                            balanceAccountId = int.Parse(worksheet.Cells[i, j].Value.ToString());
                            break;
                        case 1:
                            openingBalanceAssets = decimal.Parse(worksheet.Cells[i, j].Value.ToString());
                            break;
                        case 2:
                            openingBalanceLiabilities = decimal.Parse(worksheet.Cells[i, j].Value.ToString());
                            break;
                        case 3:
                            moneyTurnoverDebit = decimal.Parse(worksheet.Cells[i, j].Value.ToString());
                            break;
                        case 4:
                            moneyTurnoverCredit = decimal.Parse(worksheet.Cells[i, j].Value.ToString());
                            break;
                    }
                }
                if (!isInfoRow)
                {
                    decimal closingBalanceAssets = decimal.Parse(worksheet.Cells[i, 5].Value.ToString());
                    decimal closingBalanceLiabilities = decimal.Parse(worksheet.Cells[i, 6].Value.ToString());

                    SqlHelper.InsertBalanceAccount(balanceAccountId, classId,fileName, connectionString);
                    SqlHelper.InsertOpeningBalance(balanceAccountId, openingBalanceAssets, openingBalanceLiabilities, connectionString);
                    SqlHelper.InsertMoneyTurnover(balanceAccountId, moneyTurnoverDebit, moneyTurnoverCredit, connectionString);
                    SqlHelper.InsertClosingBalance(balanceAccountId, closingBalanceAssets, closingBalanceLiabilities, connectionString);

                }
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}