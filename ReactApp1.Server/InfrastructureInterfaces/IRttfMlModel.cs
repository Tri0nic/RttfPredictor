using Refit;
using ReactApp1.Server.DTO;

namespace ReactApp1.Server.InfrastructureInterfaces
{
    public interface IRttfMlModel
    {
        [Post("/predict")]
        Task<IApiResponse<PythonPredictResponse>> PredictAsync([Body] PythonPredictRequest request);
    }
}