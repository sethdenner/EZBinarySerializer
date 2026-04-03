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
    public partial class Subject : IEquatable<Subject> {
        public string Title { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = [];

        public bool Equals(Subject? other) {
            if (
                Title != other?.Title ||
                Books.Count != other?.Books.Count
            ) {
                return false;
            }

            for (int i = 0; i < Books.Count; ++i) {
                var book = Books[i];
                var otherBook = other.Books[i];
                if (book != otherBook) {
                    return false;
                }
            }

            return true;
        }
        public static bool operator ==(Subject subject1, Subject subject2) {
            if (subject1 is null) {
                return subject2 is null;
            }

            return subject1.Equals(subject2);
        }

        public static bool operator !=(Subject subject1, Subject subject2) {
            if (subject1 is null) {
                return subject2 is not null;
            }

            return !subject1.Equals(subject2);
        }

        public override bool Equals(object? obj) {
            return Equals(obj as Subject);
        }

        public override int GetHashCode() {
            return BitConverter.ToInt32(
                MD5.HashData(
                    ToBinary(this).ToArray()
                ).AsSpan()[..sizeof(int)]
            );
        }
    }
}
