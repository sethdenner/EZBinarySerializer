//HintName: BinarySerializer.TestNamespaceStudentValueSerializer.g.cs

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
    public class TestNamespaceStudentValueSerializer : IValueSerializer<global::TestNamespace.Student> {
        public static int FromBinary(Span<byte> data, out global::TestNamespace.Student value) {
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
            cursor += SystemStringValueSerializer.FromBinary(
                data[cursor..],
                out global::System.String __ez__name
            );
            cursor += SystemInt32ValueSerializer.FromBinary(
                data[cursor..],
                out global::System.Int32 __ez__year
            );
            cursor += TestNamespaceSubjectArrayValueSerializer.FromBinary(
                data[cursor..],
                out global::TestNamespace.Subject[] __ez__subjects
            );
            value = new() {
                Name = __ez__name,
                Year = __ez__year,
                Subjects = __ez__subjects,
            };

            return cursor;
        }
        public static Memory<byte> ToBinary(global::TestNamespace.Student value) {
            int size = 0;
            List<Memory<byte>> bytesList = [];
            bytesList.Add(SystemStringValueSerializer.ToBinary(
                value.Name
            ));
            size += bytesList[bytesList.Count - 1].Length;
            bytesList.Add(SystemInt32ValueSerializer.ToBinary(
                value.Year
            ));
            size += bytesList[bytesList.Count - 1].Length;
            bytesList.Add(TestNamespaceSubjectArrayValueSerializer.ToBinary(
                value.Subjects
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
    public partial class Student : global::EZBinarySerializer.IBinarySerializable {
        public virtual string FullyQualifiedTypeName {
            get {
                return "global::TestNamespace.Student";
            }
        }
        public static Memory<byte> ToBinary(global::EZBinarySerializer.IBinarySerializable value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceStudentValueSerializer.ToBinary((global::TestNamespace.Student)value);
        }

        public static int FromBinary(Span<byte> data, out global::EZBinarySerializer.IBinarySerializable value) {
            int size = global::EZBinarySerializer.ValueSerializers.TestNamespaceStudentValueSerializer.FromBinary(data, out global::TestNamespace.Student result);
            value = (global::EZBinarySerializer.IBinarySerializable)result;
            return size;
        }
    }
}