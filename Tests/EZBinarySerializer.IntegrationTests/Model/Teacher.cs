namespace EZBinarySerializer.IntegrationTests.Model {
    [BinarySerializable]
    public partial class Teacher : IPerson {
        public string Name { get; set; } = string.Empty;
        public Subject SubjectTaught = new Subject();
        public List<Student> Students = [];
    }
}
