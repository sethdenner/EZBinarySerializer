//HintName: BinarySerializer.TestNamespaceTeacherValueSerializer.g.cs

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
    class TestNamespaceTeacherValueSerializer : IValueSerializer<global::TestNamespace.Teacher> {
        public static int FromBinary(Span<byte> data, out global::TestNamespace.Teacher value) {
            int cursor = 0;
            cursor += global::EZBinarySerializer.Tests.BinarySerializer.FromBinary(
                data[
                    cursor..
                ],
                out int size
            );
            cursor += global::EZBinarySerializer.Tests.BinarySerializer.FromBinary(
                data[
                    cursor..
                ],
                out string typeName
            );
            cursor += SystemStringValueSerializer.FromBinary(
                data[cursor..],
                out global::System.String __ez__name
            );
            cursor += TestNamespaceSubjectValueSerializer.FromBinary(
                data[cursor..],
                out global::TestNamespace.Subject __ez__subjecttaught
            );
            cursor += TestNamespaceStudentSystemCollectionsGenericListValueSerializer.FromBinary(
                data[cursor..],
                out global::System.Collections.Generic.List<global::TestNamespace.Student> __ez__students
            );
            value = new() {
                Name = __ez__name,
                SubjectTaught = __ez__subjecttaught,
                Students = __ez__students,
            };

            return cursor;
        }
        public static Memory<byte> ToBinary(global::TestNamespace.Teacher value) {
            int size = 0;
            List<Memory<byte>> bytesList = [];
            bytesList.Add(SystemStringValueSerializer.ToBinary(
                value.Name
            ));
            size += bytesList[bytesList.Count - 1].Length;
            bytesList.Add(TestNamespaceSubjectValueSerializer.ToBinary(
                value.SubjectTaught
            ));
            size += bytesList[bytesList.Count - 1].Length;
            bytesList.Add(TestNamespaceStudentSystemCollectionsGenericListValueSerializer.ToBinary(
                value.Students
            ));
            size += bytesList[bytesList.Count - 1].Length;
            var typeNameBytes = global::EZBinarySerializer.Tests.BinarySerializer.ToBinary(value.FullyQualifiedTypeName);
            size += typeNameBytes.Length;
            size += sizeof(int);
            var sizeBytes = global::EZBinarySerializer.Tests.BinarySerializer.ToBinary(size);
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
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out global::TestNamespace.Teacher value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceTeacherValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(global::TestNamespace.Teacher value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceTeacherValueSerializer.ToBinary(value);
        }
    }
}
namespace TestNamespace {
    public partial class Teacher : global::EZBinarySerializer.IBinarySerializable {
        public virtual string FullyQualifiedTypeName {
            get {
                return "global::TestNamespace.Teacher";
            }
        }
        public static Memory<byte> ToBinary(global::EZBinarySerializer.IBinarySerializable value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceTeacherValueSerializer.ToBinary((global::TestNamespace.Teacher)value);
        }

        public static int FromBinary(Span<byte> data, out global::EZBinarySerializer.IBinarySerializable value) {
            int size = global::EZBinarySerializer.ValueSerializers.TestNamespaceTeacherValueSerializer.FromBinary(data, out global::TestNamespace.Teacher result);
            value = (global::EZBinarySerializer.IBinarySerializable)result;
            return size;
        }
    }
}