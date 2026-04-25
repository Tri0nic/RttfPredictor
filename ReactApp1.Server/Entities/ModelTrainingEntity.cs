namespace ReactApp1.Server.Entities
{
    public class ModelTrainingEntity
    {
        public long Id { get; set; }
        public string ModelVersion { get; set; } = "";
        public int TrainingRows { get; set; }
        public bool Success { get; set; }
        public string? FeatureImportances { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
