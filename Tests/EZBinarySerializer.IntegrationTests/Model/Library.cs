namespace EZBinarySerializer.IntegrationTests.Model {
    [BinarySerializable]
    public partial class Library {
        public Dictionary<string, Book> BooksByTitle = [];
    }

}
