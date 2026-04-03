//HintName: BinarySerializer.TestNamespaceBookSystemCollectionsGenericListValueSerializer.g.cs

namespace EZBinarySerializer.ValueSerializers {
    public class TestNamespaceBookSystemCollectionsGenericListValueSerializer : IValueSerializer<global::System.Collections.Generic.List<global::TestNamespace.Book>> {
        public static int FromBinary(Span<byte> data, out global::System.Collections.Generic.List<global::TestNamespace.Book> value) {
            int cursor = 0;
            int count = BitConverter.ToInt32(data[
                cursor..(cursor += sizeof(int))
            ]);
            value = [];
            for (int i = 0; i < count; ++i) {
                cursor += TestNamespaceBookValueSerializer.FromBinary(
                    data[cursor..],
                    out global::TestNamespace.Book result
                );
                value.Add(result);
            }
            return cursor;
        }

        public static Memory<byte> ToBinary(global::System.Collections.Generic.List<global::TestNamespace.Book> value) {
            int size = 0;
            Span<byte> lengthBytes = BitConverter.GetBytes(value.Count);
            size += lengthBytes.Length;
            List<Memory<byte>> itemBytesList = [];
            for (int i = 0; i < value.Count; ++i) {
                var itemBytes = TestNamespaceBookValueSerializer.ToBinary(value[i]);
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
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out global::System.Collections.Generic.List<global::TestNamespace.Book> value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceBookSystemCollectionsGenericListValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(global::System.Collections.Generic.List<global::TestNamespace.Book> value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceBookSystemCollectionsGenericListValueSerializer.ToBinary(value);
        }
    }
}