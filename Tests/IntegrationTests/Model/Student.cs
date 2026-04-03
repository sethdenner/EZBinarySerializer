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
using EZBinarySerializer;
using System.Security.Cryptography;

namespace IntegrationTests.Model {
    [BinarySerializable]
    public partial class Student : IPerson {
        public string Name { get; set; } = string.Empty;
        public int Year = 0;
        public Subject[] Subjects = new Subject[1];
        public Memory<Book> BookBag = new Book[1];
        public static bool operator ==(Student student1, Student student2) {
            if (student1 is null) {
                return student2 is null;
            }

            return student1.Equals(student2);
        }

        public static bool operator !=(Student student1, Student student2) {
            if (student1 is null) {
                return student2 is not null;
            }

            return !student1.Equals(student2);
        }

        public override bool Equals(object? obj) {
            return Equals(obj as Student);
        }

        public override int GetHashCode() {
            return BitConverter.ToInt32(
                MD5.HashData(
                    ToBinary(this).ToArray()
                ).AsSpan()[..sizeof(int)]
            );
        }

        public bool Equals(IPerson? other) {
            if (
                other is not Student otherStudent ||
                Name != otherStudent.Name ||
                Year != otherStudent.Year ||
                Subjects.Length != otherStudent.Subjects.Length ||
                BookBag.Length != otherStudent.BookBag.Length
            ) {
                return false;
            }

            for (int i = 0; i < Subjects.Length; ++i) {
                var subject = Subjects[i];
                var otherSubject = otherStudent.Subjects[i];
                if (subject != otherSubject) {
                    return false;
                }
            }

            for (int i = 0; i < BookBag.Length; ++i) {
                var book = BookBag.Span[i];
                var otherBook = otherStudent.BookBag.Span[i];

                if (book != otherBook) {
                    return false;
                }
            }
            return true;
        }
    }
}
