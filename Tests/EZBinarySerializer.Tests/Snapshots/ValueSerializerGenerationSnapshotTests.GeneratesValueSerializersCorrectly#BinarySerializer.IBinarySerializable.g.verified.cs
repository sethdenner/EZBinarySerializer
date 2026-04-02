//HintName: BinarySerializer.IBinarySerializable.g.cs

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

using System.Text;

namespace EZBinarySerializer {
    public interface IBinarySerializable {
        public abstract string FullyQualifiedTypeName {
            get;
        }
        public static abstract Memory<byte> ToBinary(IBinarySerializable value);
        public static abstract int FromBinary(Span<byte> data, out IBinarySerializable value);
        public static string PeekTypeName(Span<byte> data) {
            int index = sizeof(int);
            int typeNameSize = BitConverter.ToInt32(
                data[index..(index += sizeof(int))]
            );
            string typeName = Encoding.UTF8.GetString(
                data[index..(index += typeNameSize)]
            );
            return typeName;
        }

        public static int PeekSize(Memory<byte> data) {
            return BitConverter.ToInt32(
                data[0..sizeof(int)].Span
            );
        }

        public static Memory<byte> BuildBinarySerializableHeader(
            int dataSize,
            string serializedTypeName
        ) {
            Span<byte> serializedTypeNameBytes = Encoding.UTF8.GetBytes(serializedTypeName);
            Span<byte> serializedTypeNameSizeBytes = BitConverter.GetBytes(
                serializedTypeNameBytes.Length
            );
            int headerSize = sizeof(int) +
                serializedTypeNameBytes.Length +
                serializedTypeNameSizeBytes.Length;

            dataSize += headerSize;
            Span<byte> sizeBytes = BitConverter.GetBytes(dataSize);
            Memory<byte> data = new byte[headerSize];
            int index = 0;
            sizeBytes.CopyTo(
                data[index..(index += sizeBytes.Length)].Span
            );
            serializedTypeNameSizeBytes.CopyTo(
                data[index..(index += serializedTypeNameSizeBytes.Length)].Span
            );
            serializedTypeNameBytes.CopyTo(
                data[index..(index += serializedTypeNameBytes.Length)].Span
            );

            return data;
        }

        public static int GetBinarySerializableHeader(
            Span<byte> data,
            out BinarySerializableHeader header
        ) {
            int index = 0;
            int size = BitConverter.ToInt32(
                data[index..(index += sizeof(int))]
            );
            int serializedTypeNameSize = BitConverter.ToInt32(
                data[index..(index += sizeof(int))]
            );
            string serializedTypeName = Encoding.UTF8.GetString(
                data[index..(index += serializedTypeNameSize)]
            );

            header = new(
                size,
                serializedTypeNameSize,
                serializedTypeName
            );

            return index;
        }
    }
}
