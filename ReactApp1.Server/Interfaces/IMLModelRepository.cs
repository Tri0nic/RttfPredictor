using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;

namespace ReactApp1.Server.Interfaces
{
    public interface IMLModelRepository
    {
        Task<(MethodResult, string, PythonPredictResponse?)> PredictAsync(PythonPredictRequest request);
        Task<(MethodResult, string, Dictionary<string, double>?)> GetFeatureImportanceAsync();
    }
}
