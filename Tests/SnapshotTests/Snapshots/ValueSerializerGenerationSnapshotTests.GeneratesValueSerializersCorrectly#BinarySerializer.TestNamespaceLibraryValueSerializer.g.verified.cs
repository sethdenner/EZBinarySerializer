//HintName: BinarySerializer.TestNamespaceLibraryValueSerializer.g.cs

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
    public class TestNamespaceLibraryValueSerializer : IValueSerializer<global::TestNamespace.Library> {
        public static int FromBinary(Span<byte> data, out global::TestNamespace.Library value) {
            int cursor = 0;
            cursor += SystemInt32ValueSerializer.FromBinary(
                data[
                    cursor..
                ],
                out int size
            );
            cursor += SystemStringValueSerializer.FromBinary(
                data[
                    cursor..
                ],
                out string typeName
            );
            cursor += SystemStringTestNamespaceBookSystemCollectionsGenericDictionaryValueSerializer.FromBinary(
                data[cursor..],
                out global::System.Collections.Generic.Dictionary<global::System.String, global::TestNamespace.Book> __ez__booksbytitle
            );
            value = new() {
                BooksByTitle = __ez__booksbytitle,
            };

            return cursor;
        }
        public static Memory<byte> ToBinary(global::TestNamespace.Library value) {
            int size = 0;
            List<Memory<byte>> bytesList = [];
            bytesList.Add(SystemStringTestNamespaceBookSystemCollectionsGenericDictionaryValueSerializer.ToBinary(
                value.BooksByTitle
            ));
            size += bytesList[bytesList.Count - 1].Length;
            var typeNameBytes = SystemStringValueSerializer.ToBinary(value.FullyQualifiedTypeName);
            size += typeNameBytes.Length;
            size += sizeof(int);
            var sizeBytes = SystemInt32ValueSerializer.ToBinary(size);
            Memory<byte> data = new byte[size];
            int cursor = 0;
            sizeBytes.CopyTo(
                data[cursor..(cursor += sizeBytes.Length)]
            );
            typeNameBytes.CopyTo(
                data[cursor..(cursor += typeNameBytes.Length)]
            );
            foreach (var bytes in bytesList) {
                bytes.CopyTo(
                    data[cursor..(cursor += bytes.Length)]
                );
            }
            
            return data;
        }
    }
}
namespace TestNamespace {
    public partial class Library : global::EZBinarySerializer.IBinarySerializable {
        public virtual string FullyQualifiedTypeName {
            get {
                return "global::TestNamespace.Library";
            }
        }
        public static Memory<byte> ToBinary(global::EZBinarySerializer.IBinarySerializable value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceLibraryValueSerializer.ToBinary((global::TestNamespace.Library)value);
        }

        public static int FromBinary(Span<byte> data, out global::EZBinarySerializer.IBinarySerializable value) {
            int size = global::EZBinarySerializer.ValueSerializers.TestNamespaceLibraryValueSerializer.FromBinary(data, out global::TestNamespace.Library result);
            value = (global::EZBinarySerializer.IBinarySerializable)result;
            return size;
        }
    }
}