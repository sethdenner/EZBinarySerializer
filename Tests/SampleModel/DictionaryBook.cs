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

namespace SampleModel {
    [BinarySerializable]
    public partial class DictionaryBook : Book {
        public Dictionary<string, string> DefinitionsByWordName { get; set; } = [];
        public override string Text {
            get; set;
        } = string.Empty;

        public override bool Equals(object? obj) {
            return Equals(obj as DictionaryBook);
        }

        public override bool Equals(Book? other) {
            if (
                other is not DictionaryBook otherBook ||
                otherBook.Text != Text ||
                otherBook.Title != Title ||
                otherBook.PercentageRead != PercentageRead
            ) {
                return false;
            }

            foreach (var item in otherBook.DefinitionsByWordName) {
                if (!DefinitionsByWordName.TryGetValue(item.Key, out string? definition)) {
                    return false;
                }

                if (definition != item.Value) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
