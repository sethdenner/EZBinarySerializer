namespace EZBinarySerializer.IntegrationTests.Model {
    [BinarySerializable]
    public partial class DictionaryBook : Book {
        public Dictionary<string, string> DefinitionsByWordName { get; set; } = [];
        [BinarySerializerIgnore]
        public int TotalWords = 0;
    }
}
