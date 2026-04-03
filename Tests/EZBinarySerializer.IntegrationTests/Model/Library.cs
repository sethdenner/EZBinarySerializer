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
using System.Numerics;
using System.Security.Cryptography;

namespace EZBinarySerializer.IntegrationTests.Model {
    [BinarySerializable]
    public partial class Library : IBuilding {
        public Dictionary<string, Book> BooksByTitle = [];

        public Vector2 GeoCoordinates {
            get; set;
        }

        public bool Equals(IBuilding? other) {
            if (
                other is not Library library ||
                BooksByTitle.Count != library.BooksByTitle.Count
            ) {
                return false;
            }

            foreach (var item in BooksByTitle) {
                if (
                    !library.BooksByTitle.TryGetValue(item.Key, out var book) ||
                    book != BooksByTitle[item.Key]
                ) {
                    return false;
                }
            }

            return true;
        }

        public static bool operator ==(Library library1, Library library2) {
            if (library1 is null) {
                return library2 is null;
            }

            return library1.Equals(library2);
        }

        public static bool operator !=(Library library1, Library library2) {
            if (library1 is null) {
                return library2 is not null;
            }

            return !library1.Equals(library2);
        }

        public override int GetHashCode() {
            return BitConverter.ToInt32(
                MD5.HashData(
                    ToBinary(this).ToArray()
                ).AsSpan()[..sizeof(int)]
            );
        }

        public override bool Equals(object? obj) {
            return Equals(obj as Library);
        }
    }
}
