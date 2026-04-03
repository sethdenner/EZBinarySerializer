namespace EZBinarySerializer.IntegrationTests.Model {
    [BinarySerializable]
    public abstract partial class Book {
        public string Title = string.Empty;
        public string Text = string.Empty;
        public float PercentageRead = 0.0f;
    }
}
