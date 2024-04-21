using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscountCodeGenrator.Services;
using DiscountCodeGenerator.Data.Context;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using DiscountCodeGenrator;
using Grpc.Core;

namespace Services.Tests
{
    [TestClass]
    public class CodeGeneratorServiceTests
    {
        private ILogger<CodeGeneratorService> _logger;
        private DbContextOptions<CodeGeneratorDbContext> _dbContextOptions;
        private ServerCallContext context;

        [TestInitialize]
        public void Setup()
        {
            // Configure DbContext options for SQL Server
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<CodeGeneratorDbContext>();
            builder.UseSqlServer("Data Source=CHARLES;Integrated Security=True;Initial Catalog=CodeGenerator;Encrypt=False;Trust Server Certificate=False;")
                   .UseInternalServiceProvider(serviceProvider);

            _dbContextOptions = builder.Options;

            // Create a logger using LoggerFactory
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger<CodeGeneratorService>();
        }


        [TestMethod]
        public void GenerateCodeSTest_GeneratedCodes_IsNotNull()
        {
            using (var dbContext = new CodeGeneratorDbContext(_dbContextOptions))
            {
                var codegeneratorService = new CodeGeneratorService(_logger, dbContext);
                int count = 50;

                var codes = codegeneratorService.GenerateCodes(count);
                Assert.IsNotNull(codes);
            }
            _logger.Log(LogLevel.Information, "Test completed");

        }
        [TestMethod]
        public void CodeGeneratorService_GeneratedCodes_LengthIsEqualToTheExpectedLength()
        {
            using (var dbContext = new CodeGeneratorDbContext(_dbContextOptions))
            {
                CodeGeneratorService codegeneratorService = new CodeGeneratorService(_logger, dbContext);
                int expectedLength = 8;

                string code = codegeneratorService.GenerateCodes(expectedLength);
                int length = code.Length;

                Assert.AreEqual(expectedLength, length);

            }
            _logger.Log(LogLevel.Information, "Test completed");

        }

        [TestMethod()]
        public void GenerateCodeTest_Codes_IsNot_Null()
        {
            using (var dbContext = new CodeGeneratorDbContext(_dbContextOptions))
            {
                CodeGeneratorService codegeneratorService = new CodeGeneratorService(_logger, dbContext);
                uint count = 30;
                int length = 8;

                GenerateCodeRequest request = new GenerateCodeRequest() { Count = count, Length = length };
                var codes = codegeneratorService.GenerateCode(request, context);

                Assert.IsNotNull(codes);

            }
            _logger.Log(LogLevel.Information, "Test completed");

        }
        [TestMethod()]
        public async Task GenerateCodeTest_Response_ShouldBeAnException()
        {
            using (var dbContext = new CodeGeneratorDbContext(_dbContextOptions))
            {
                CodeGeneratorService codegeneratorService = new CodeGeneratorService(_logger, dbContext);
                uint count = 30;
                int length = 1;

                GenerateCodeRequest request = new GenerateCodeRequest() { Count = count, Length = length };

                await Assert.ThrowsExceptionAsync<RpcException>(async () =>
                {
                    await codegeneratorService.GenerateCode(request, context);
                });
            }
            _logger.Log(LogLevel.Information, "Test completed");

        }

        [TestMethod()]
        public async Task GetCodesTest_ExpectResultToBeTypeOfResponse()
        {
            using (var dbContext = new CodeGeneratorDbContext(_dbContextOptions))
            {
                CodeGeneratorService codegeneratorService = new CodeGeneratorService(_logger, dbContext);
                GetCodesRequest request = new GetCodesRequest() { PageNumber = 1, PageSize = 5 };

                var response = await codegeneratorService.GetCodes(request, context);
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType<GetCodesResponse>(response);

            }
        }
        [TestMethod()]
        public async Task GetCodesTest_Response_ShouldNotBeNull()
        {
            using (var dbContext = new CodeGeneratorDbContext(_dbContextOptions))
            {
                CodeGeneratorService codegeneratorService = new CodeGeneratorService(_logger, dbContext);
                GetCodesRequest request = new GetCodesRequest() { PageNumber = 1, PageSize = 5 };

                var response = await codegeneratorService.GetCodes(request, context);
                Assert.IsNotNull(response);
            }
            _logger.Log(LogLevel.Information, "Test completed");

        }
    }
}