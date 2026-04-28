using System.Net;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;
using ReactApp1.Server.InfrastructureInterfaces;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Repositories
{
    public class MLModelRepository : IMLModelRepository
    {
        private readonly IRttfMlModel _mlModel;
        private readonly ILogger<MLModelRepository> _logger;

        public MLModelRepository(IRttfMlModel mlModel, ILogger<MLModelRepository> logger)
        {
            _mlModel = mlModel;
            _logger = logger;
        }

        public async Task<(MethodResult, string, PythonPredictResponse?)> PredictAsync(PythonPredictRequest request)
        {
            var response = await _mlModel.PredictAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var message = $"ML сервис вернул ошибку. StatusCode: {response.StatusCode}. Error: {response.Error?.Message}";
                _logger.LogError(message);

                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    return (MethodResult.InternalError, "ML сервис недоступен или модель не обучена", null);

                return (MethodResult.InternalError, message, null);
            }

            if (response.Content == null)
            {
                _logger.LogError("Refit десериализация вернула null. Error: {Error}", response.Error?.Message);
                return (MethodResult.InternalError, $"Ошибка десериализации ответа ML сервиса: {response.Error?.Message}", null);
            }

            return (MethodResult.Success, "", response.Content);
        }

        public async Task<(MethodResult, string, Dictionary<string, double>?)> GetFeatureImportanceAsync()
        {
            var response = await _mlModel.GetFeatureImportanceAsync();
            if (!response.IsSuccessStatusCode)
                return (MethodResult.InternalError, $"ML сервис вернул ошибку: {response.Error?.Message}", null);
            return (MethodResult.Success, "", response.Content);
        }
    }
}
