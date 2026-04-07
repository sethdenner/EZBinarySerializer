//HintName: BinarySerializer.TestNamespaceSubjectArrayValueSerializer.g.cs

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

namespace EZBinarySerializer.ValueSerializers {
    public class TestNamespaceSubjectArrayValueSerializer : IValueSerializer<global::TestNamespace.Subject[]> {
        public static int FromBinary(Span<byte> data, out global::TestNamespace.Subject[] value) {
            int cursor = 0;
            int length = BitConverter.ToInt32(data[
                cursor..(cursor += sizeof(int))
            ]);
            value = new global::TestNamespace.Subject[length];
            for (int i = 0; i < length; ++i) {
                cursor += TestNamespaceSubjectValueSerializer.FromBinary(
                    data[cursor..],
                    out global::TestNamespace.Subject result
                );
                value[i] = result;
            }
            return cursor;
        }

        public static Memory<byte> ToBinary(global::TestNamespace.Subject[] value) {
            int size = 0;
            Span<byte> lengthBytes = BitConverter.GetBytes(value.Length);
            size += lengthBytes.Length;
            List<Memory<byte>> itemBytesList = [];
            for (int i = 0; i < value.Length; ++i) {
                var itemBytes = TestNamespaceSubjectValueSerializer.ToBinary(value[i]);
                size += itemBytes.Length;
                itemBytesList.Add(itemBytes);
            }

            Memory<byte> data = new byte[size];

            int cursor = 0;
            lengthBytes.CopyTo(data[
                cursor..(cursor += lengthBytes.Length)
            ].Span);
            for (int i = 0; i < itemBytesList.Count; ++i) {
                var item = itemBytesList[i];
                item.CopyTo(data[
                    cursor..(cursor += item.Length)
                ]);
            }

            return data;
        }
    }
}