//HintName: BinarySerializer.SystemDrawingRectangleFValueSerializer.g.cs

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

using System.Drawing;

namespace EZBinarySerializer.ValueSerializers {
    public class SystemDrawingRectangleFValueSerializer : IValueSerializer<RectangleF> {
        public static int FromBinary(Span<byte> data, out RectangleF value) {
            int cursor = 0;
            float x = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            float y = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            float width = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            float height = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            value = new RectangleF(x, y, width, height);
            return cursor;
        }

        public static Memory<byte> ToBinary(RectangleF value) {
            Span<byte> xBytes = BitConverter.GetBytes(value.X);
            Span<byte> yBytes = BitConverter.GetBytes(value.Y);
            Span<byte> widthBytes = BitConverter.GetBytes(value.Width);
            Span<byte> heightBytes = BitConverter.GetBytes(value.Height);

            int size = xBytes.Length +
                yBytes.Length +
                widthBytes.Length +
                heightBytes.Length;

            Memory<byte> data = new byte[size];
            int cursor = 0;
            xBytes.CopyTo(data[
                cursor..(cursor += xBytes.Length)
            ].Span);
            yBytes.CopyTo(data[
                cursor..(cursor += yBytes.Length)
            ].Span);
            widthBytes.CopyTo(data[
                cursor..(cursor += widthBytes.Length)
            ].Span);
            heightBytes.CopyTo(data[
                cursor..(cursor += heightBytes.Length)
            ].Span);

            return data;
        }
    }
}