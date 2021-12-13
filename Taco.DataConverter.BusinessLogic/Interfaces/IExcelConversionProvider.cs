using System.Threading.Tasks;
using System.Threading;

namespace Taco.DataConverter.BusinessLogic.Interfaces
{
    public interface IExcelConversionProvider
    {
        /// <summary>
        /// Creates a new test case.
        /// </summary>
        /// <param name="testCaseForCreate">Parameters needed to create a test case.</param>
        /// <param name="cancellationToken">Optional token that can end async tasks.</param>
        Task ConvertExcel(CancellationToken cancellationToken = default);
    }
}
