using DiscountGeneratorClient.Models;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace DiscountGeneratorClient.Controllers
{
    public class CodeGeneratorController : Controller
    {
        private readonly IConfiguration _configuration;
       
        string _address;
        public CodeGeneratorController(IConfiguration configuration)
        {
            _configuration = configuration;

            _address = _configuration.GetSection("ServerAddress:Url").Value!;
        }
        // GET: CodeGeneratorController
        public ActionResult Index([FromQuery]int pageNumber )
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            
            using var channel = GrpcChannel.ForAddress(_address);
            var client = new CodeGenerator.CodeGeneratorClient(channel);
            var codes = client.GetCodes(new GetCodesRequest() { PageNumber = pageNumber, PageSize = 10 });
            List<DiscountCodeViewModel> result = new List<DiscountCodeViewModel>();
            foreach (var item in codes.Response)
            {
                result.Add(
                    new DiscountCodeViewModel
                    {
                        Code = item.Code,
                        IsUsed = item.IsUsed
                    });

            }
            ViewBag.TotalPages = codes.Pages;
            ViewBag.PageNumber = pageNumber;
            return View(result);
        }


      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateCode([FromForm] GenerateCodesViewModel model)
        {
            
            try
            {
                using var channel = GrpcChannel.ForAddress(_address);
                var client = new CodeGenerator.CodeGeneratorClient(channel);
                var response = client.GenerateCode(new GenerateCodeRequest
                {
                    Count = model.NumberOfCodes,
                    Length = model.CodeLength,
                });
                ViewBag.Result = response;
                TempData["SuccessMsg"] = "Codes Generated";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["ErrMsg"] = ex.Message;
                return RedirectToAction(nameof(GenerateCodes));
            }
        }
        public IActionResult GenerateCodes()
        {
            return View();
            
        }


         [HttpPost]
         [ValidateAntiForgeryToken]
        public IActionResult UseACode([FromForm]UseCodeViewModel model)
        {
            
            try
            {
                using var channel = GrpcChannel.ForAddress(_address);
                var client = new CodeGenerator.CodeGeneratorClient(channel);
                var response = client.UseCode(new UseCodeRequest { Code = model.DiscountCode });
                ViewBag.Result = response;
                TempData["SuccessMsg"] = "Code Used Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrMsg"] = ex.Message;
                return RedirectToAction(nameof(UseCode));
            }
        } 
        public IActionResult UseCode()
        {
            return View();
        
        }

      
    }
}
