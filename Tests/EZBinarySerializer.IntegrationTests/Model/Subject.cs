namespace EZBinarySerializer.IntegrationTests.Model {
    [BinarySerializable]
    public partial class Subject {
        public string Title { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = [];
    }
}
