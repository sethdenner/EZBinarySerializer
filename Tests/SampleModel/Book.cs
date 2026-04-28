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

namespace SampleModel {
    [BinarySerializable]
    public abstract partial class Book : IEquatable<Book> {
        public string Title = string.Empty;
        public BookGenere Genere { get; set; }
        public abstract string Text { get; set; }
        public float PercentageRead = 0.0f;

        public static bool operator ==(Book book1, Book book2) {
            if (book1 is null) {
                return book2 is null;
            }

            return book1.Equals(book2);
        }

        public static bool operator !=(Book book1, Book book2) {
            if (book1 is null) {
                return book2 is not null;
            }

            return !book1.Equals(book2);
        }

        public override bool Equals(object? obj) {
            return (obj is Book book && book.Equals(this));
        }

        public override int GetHashCode() {
            return BitConverter.ToInt32(
                MD5.HashData(
                    ToBinary(this).ToArray()
                ).AsSpan()[..sizeof(int)]
            );
        }

        public abstract bool Equals(Book? other);
    }
}
