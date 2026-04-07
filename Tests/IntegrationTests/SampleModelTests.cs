using EZBinarySerializer;
using SampleModel;
namespace IntegrationTests;

public class SampleModelTests {
    [Fact]
    public void TestFullModel() {
        DictionaryBook dictionary = new() {
            Title = "The School District Dictionary",
            Text = "Foreword by Dick Dictonarian",
            DefinitionsByWordName = new() {
                { "Student", "A person that attends school to learn." },
                { "Teacher", "A person that attends school to teach." },
            }
        };
        Subject subject1 = new() {
            Title = "Music Theory",
            Books = [dictionary]
        };
        Subject subject2 = new() {
            Title = "Carpentry",
            Books = []
        };
        Student student1 = new() {
            Name = "Chewbaca",
            BookBag = new Book[1] { dictionary },
            Year = 3,
            Subjects = [subject2]
        };
        Student student2 = new() {
            Name = "Mozart",
            BookBag = new Book[1] { dictionary },
            Year = 4,
            Subjects= [subject1]
        };
        Teacher teacher1 = new() {
            Name = "Bob Marley",
            SubjectTaught = subject1,
            Students = [student1]
        };
        Teacher teacher2 = new() {
            Name = "Harrison Ford",
            SubjectTaught = subject2,
            Students = [student2]
        };
        School school = new() {
            Teachers = [teacher1, teacher2],
            Students = [student1, student2],
            GeoCoordinates = new(30.35f, 109.4f)
        };
        Library library = new() {
            BooksByTitle = new() {
                { dictionary.Title, dictionary }
            },
            GeoCoordinates = new(102.23f, 80.5f)
        };
        District district = new() {
            Buildings = [
                school,
                library
            ]
        };

        var data = District.ToBinary(district);
        int bytesRead = District.FromBinary(
            data.Span,
            out IBinarySerializable deserialized
        );

        Assert.Equal(data.Length, bytesRead);
        Assert.Equal(district, (District)deserialized);
    }
}
