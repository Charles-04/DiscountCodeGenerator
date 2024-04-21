using DiscountCodeGenerator.Data.Context;
using DiscountCodeGenerator.Model.Entities;
using DiscountCodeGenrator.AppConstants;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DiscountCodeGenrator.Services
{
    public class CodeGeneratorService:CodeGenerator.CodeGeneratorBase
    {
        private readonly ILogger<CodeGeneratorService> _logger;
        private readonly CodeGeneratorDbContext _dbcontext;
        private static readonly Random _random = new Random();

        public CodeGeneratorService(ILogger<CodeGeneratorService> logger,CodeGeneratorDbContext dbcontext)
        {
            _logger = logger;
            _dbcontext = dbcontext;
        }
        public override async Task<GenerateCodeResponse> GenerateCode(GenerateCodeRequest request, ServerCallContext context)
        {
            byte maxCodeLength = 8;
            byte minCodeLength = 7;
            ushort minCount = 1;
            ushort maxCount = 2000;

            if (request == null)
            {
                throw new RpcException(Status.DefaultCancelled, "Request is empty");
            }

            if (request.Count < minCount || request.Count > maxCount)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Count must be between one and two thousand"));
            }

            if (request.Length >= minCodeLength && request.Length <= maxCodeLength)
            {
                List<DiscountCode> discountCodes = await Task.Run(() => GenerateDiscountCodes(request.Count));

                await _dbcontext.AddRangeAsync(discountCodes);
                await _dbcontext.SaveChangesAsync();
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Length should be between 7 and 8"));
            }

            _logger.Log(LogLevel.Information, message: "Codes generated");
            return new GenerateCodeResponse() { Result = true };
        }

        private List<DiscountCode> GenerateDiscountCodes(uint count)
        {
           
            HashSet<string> codes = new HashSet<string>();
            List<DiscountCode> discounts = new List<DiscountCode>();
            while (codes.Count < count)
            {
                DiscountCode discountCode = new DiscountCode();
                string code = GenerateCodes(_random.Next(7, 9));
                codes.Add(code);
                discountCode.Code = code;
                discounts.Add(discountCode);
                
            }

            return discounts;
        }

         

        public string GenerateCodes(int length)
        {
            var codeChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                codeChars[i] = Constants.AllowedChars[_random.Next(Constants.AllowedChars.Length)];
            }
            return new string(codeChars);
        }
        public override async Task<GetCodesResponse> GetCodes(GetCodesRequest request, ServerCallContext context)
        {
            GetCodesResponse response = new();
            int skip = (request.PageNumber - 1 )* request.PageSize;

            IQueryable<DiscountCode> codes = _dbcontext.DiscountCodes.OrderBy(x => x.Code);
                
            int totalPages = (int)Math.Ceiling((double)codes.Count() / request.PageSize);
            

            List<DiscountCode> paginatedCodes = codes.Skip(skip)
                                                      .Take(request.PageSize).ToList();
            foreach (var code in paginatedCodes)
            {
                response.Response.Add(new GetCodeResponse()
                {
                    Code = code.Code,
                    IsUsed = code.IsUsed
                });
            }
            response.Pages = totalPages;
            return response;
        }

        public override async Task<UseCodeResponse>UseCode(UseCodeRequest request, ServerCallContext context)
        {
            DiscountCode? usedCode = await _dbcontext.DiscountCodes.Where(x => x.Code == request.Code && x.IsUsed == true).FirstOrDefaultAsync();
            if (usedCode!= null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Code has already been used"));

            int result = await _dbcontext.DiscountCodes
                                        .Where(x => x.Code == request.Code)
                                        .ExecuteUpdateAsync(x => x.SetProperty(code => code.IsUsed,true));

           if (result == 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Code Doesn't exist"));

            UseCodeResponse response = new UseCodeResponse() { Result = result};

            return response;
        }


    }
}
