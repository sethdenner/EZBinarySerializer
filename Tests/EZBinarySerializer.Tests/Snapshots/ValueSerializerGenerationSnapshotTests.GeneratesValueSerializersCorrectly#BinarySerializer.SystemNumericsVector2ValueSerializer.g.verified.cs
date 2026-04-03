//HintName: BinarySerializer.SystemNumericsVector2ValueSerializer.g.cs

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

namespace EZBinarySerializer.ValueSerializers {
    public class SystemNumericsVector2ValueSerializer : IValueSerializer<Vector2> {
        public static int FromBinary(Span<byte> data, out Vector2 value) {
            int cursor = 0;
            var x = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var y = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);

            value = new Vector2(x, y);
            return cursor;
        }

        public static Memory<byte> ToBinary(Vector2 value) {
            Span<byte> xBytes = BitConverter.GetBytes(value.X);
            Span<byte> yBytes = BitConverter.GetBytes(value.Y);

            int size = xBytes.Length +
                yBytes.Length;

            Memory<byte> data = new byte[size];
            int cursor = 0;
            xBytes.CopyTo(data[
                cursor..(cursor += xBytes.Length)
            ].Span);
            yBytes.CopyTo(data[
                cursor..(cursor += yBytes.Length)
            ].Span);

            return data;
        }
    }
}
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out Vector2 value) {
            return global::EZBinarySerializer.ValueSerializers.SystemNumericsVector2ValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(Vector2 value) {
            return global::EZBinarySerializer.ValueSerializers.SystemNumericsVector2ValueSerializer.ToBinary(value);
        }
    }
}
