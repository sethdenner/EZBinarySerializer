//HintName: BinarySerializer.SystemStringValueSerializer.g.cs

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

namespace EZBinarySerializer.ValueSerializers {
    public class SystemStringValueSerializer : IValueSerializer<string> {
        public static int FromBinary(Span<byte> data, out string value) {
            int cursor = 0;
            int length = BitConverter.ToInt32(data[
                cursor..(cursor += sizeof(int))
            ]);
            value = Encoding.UTF8.GetString(data[
                cursor..(cursor += length)
            ]);
            return cursor;
        }

        public static Memory<byte> ToBinary(string value) {
            int size = 0;

            Span<byte> stringBytes = Encoding.UTF8.GetBytes(value);
            size += stringBytes.Length;

            Span<byte> lengthBytes = BitConverter.GetBytes(stringBytes.Length);
            size += lengthBytes.Length;

            Memory<byte> data = new byte[size];

            int cursor = 0;
            lengthBytes.CopyTo(data[
                cursor..(cursor += lengthBytes.Length)
            ].Span);
            stringBytes.CopyTo(data[
                cursor..(cursor += stringBytes.Length)
            ].Span);

            return data;
        }
    }
}
