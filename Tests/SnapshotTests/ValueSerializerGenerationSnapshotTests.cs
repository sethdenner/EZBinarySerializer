/*
 *  EZBinarySerializer serialize objects while maintaining AOT compatibility.
 *  Copyright (C) 2026 Seth Adam Denner
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
namespace EZBinarySerializer.Tests;

public class ValueSerializerGenerationSnapshotTests {
    [Fact]
    public Task GeneratesValueSerializersCorrectly() {
        var source = @"
using EZBinarySerializer;
using System.Collections.Generic;

namespace TestNamespace {
    [BinarySerializable]
    public abstract partial class Book {
        public string Title = string.Empty;
        public string Text = string.Empty;
        public float PercentageRead = 0.0f;
    }

    [BinarySerializable]
    public partial class DictionaryBook : Book {
        public Dictionary<string, string> DefinitionsByWordName {get; set;} = [];
        [BinarySerializerIgnore]
        public int TotalWords = 0;
    }

    [BinarySerializable]
    public partial class Library {
        public Dictionary<string, Book> BooksByTitle = [];
    }

    [BinarySerializable]
    public partial class Subject {
        public string Title {get; set;} = string.Empty;
        public List<Book> Books {get; set;} = [];
    }

    [BinarySerializable]
    public partial interface IPerson {
        public string Name { get; set; }
    }

    [BinarySerializable]
    public partial class Student : IPerson {
        public string Name { get; set; } = string.Empty;
        public int Year = 0;
        public Subject[] Subjects = new Subject[6];
    }

    [BinarySerializable]
    public partial class Teacher : IPerson {
        public string Name { get; set; }= string.Empty;
        public Subject SubjectTaught = new Subject();
        public List<Student> Students = [];
    }
}
";
 
        return SourceVerification.VerifySource(source);
    }
}
