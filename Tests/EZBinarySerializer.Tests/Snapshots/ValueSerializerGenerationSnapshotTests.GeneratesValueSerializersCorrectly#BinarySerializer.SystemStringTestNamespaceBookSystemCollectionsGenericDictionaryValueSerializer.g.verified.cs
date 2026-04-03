//HintName: BinarySerializer.SystemStringTestNamespaceBookSystemCollectionsGenericDictionaryValueSerializer.g.cs

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
    public class SystemStringTestNamespaceBookSystemCollectionsGenericDictionaryValueSerializer : IValueSerializer<global::System.Collections.Generic.Dictionary<global::System.String, global::TestNamespace.Book>> {
        public static int FromBinary(Span<byte> data, out global::System.Collections.Generic.Dictionary<global::System.String, global::TestNamespace.Book> value) {
            int cursor = 0;
            int count = BitConverter.ToInt32(data[
                cursor..(cursor += sizeof(int))
            ]);
            value = [];
            for (int i = 0; i < count; ++i) {
                cursor += SystemStringValueSerializer.FromBinary(
                    data[cursor..],
                    out global::System.String key
                );
                cursor += TestNamespaceBookValueSerializer.FromBinary(
                    data[cursor..],
                    out global::TestNamespace.Book result
                );
                value[key] = result;
            }
            return cursor;
        }

        public static Memory<byte> ToBinary(global::System.Collections.Generic.Dictionary<global::System.String, global::TestNamespace.Book> value) {
            int size = 0;
            Span<byte> countBytes = BitConverter.GetBytes(value.Count);
            size += countBytes.Length;
            List<Memory<byte>> itemBytesList = [];
            foreach (var key in value.Keys) {
                var keyBytes = SystemStringValueSerializer.ToBinary(key);
                size += keyBytes.Length;
                itemBytesList.Add(keyBytes);
                var valueBytes = TestNamespaceBookValueSerializer.ToBinary(value[key]);
                size += valueBytes.Length;
                itemBytesList.Add(valueBytes);
            }

            Memory<byte> data = new byte[size];

            int cursor = 0;
            countBytes.CopyTo(data[
                cursor..(cursor += countBytes.Length)
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
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out global::System.Collections.Generic.Dictionary<global::System.String, global::TestNamespace.Book> value) {
            return global::EZBinarySerializer.ValueSerializers.SystemStringTestNamespaceBookSystemCollectionsGenericDictionaryValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(global::System.Collections.Generic.Dictionary<global::System.String, global::TestNamespace.Book> value) {
            return global::EZBinarySerializer.ValueSerializers.SystemStringTestNamespaceBookSystemCollectionsGenericDictionaryValueSerializer.ToBinary(value);
        }
    }
}