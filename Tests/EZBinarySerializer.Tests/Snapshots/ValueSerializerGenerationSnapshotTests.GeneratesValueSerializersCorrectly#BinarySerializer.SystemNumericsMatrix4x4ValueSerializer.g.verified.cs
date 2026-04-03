//HintName: BinarySerializer.SystemNumericsMatrix4x4ValueSerializer.g.cs

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
    public class SystemNumericsMatrix4x4ValueSerializer : IValueSerializer<Matrix4x4> {
        public static int FromBinary(Span<byte> data, out Matrix4x4 value) {
            int cursor = 0;
            var m11 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m12 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m13 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m14 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);

            var m21 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m22 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m23 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m24 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);

            var m31 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m32 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m33 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m34 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);

            var m41 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m42 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m43 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            var m44 = BitConverter.ToSingle(data[
                cursor..(cursor += sizeof(float))
            ]);
            value = new Matrix4x4(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
            );

            return cursor;
        }

        public static Memory<byte> ToBinary(Matrix4x4 value) {
            Span<byte> m11Bytes = BitConverter.GetBytes(value.M11);
            Span<byte> m12Bytes = BitConverter.GetBytes(value.M12);
            Span<byte> m13Bytes = BitConverter.GetBytes(value.M13);
            Span<byte> m14Bytes = BitConverter.GetBytes(value.M14);

            Span<byte> m21Bytes = BitConverter.GetBytes(value.M21);
            Span<byte> m22Bytes = BitConverter.GetBytes(value.M22);
            Span<byte> m23Bytes = BitConverter.GetBytes(value.M23);
            Span<byte> m24Bytes = BitConverter.GetBytes(value.M24);

            Span<byte> m31Bytes = BitConverter.GetBytes(value.M31);
            Span<byte> m32Bytes = BitConverter.GetBytes(value.M32);
            Span<byte> m33Bytes = BitConverter.GetBytes(value.M33);
            Span<byte> m34Bytes = BitConverter.GetBytes(value.M34);

            Span<byte> m41Bytes = BitConverter.GetBytes(value.M41);
            Span<byte> m42Bytes = BitConverter.GetBytes(value.M42);
            Span<byte> m43Bytes = BitConverter.GetBytes(value.M43);
            Span<byte> m44Bytes = BitConverter.GetBytes(value.M44);

            int size = m11Bytes.Length +
                m12Bytes.Length +
                m13Bytes.Length +
                m14Bytes.Length +
                m21Bytes.Length +
                m22Bytes.Length +
                m23Bytes.Length +
                m24Bytes.Length +
                m31Bytes.Length +
                m32Bytes.Length +
                m33Bytes.Length +
                m34Bytes.Length +
                m41Bytes.Length +
                m42Bytes.Length +
                m43Bytes.Length +
                m44Bytes.Length;

            Memory<byte> data = new byte[size];
            int cursor = 0;
            m11Bytes.CopyTo(data[
                cursor..(cursor += m11Bytes.Length)
            ].Span);
            m12Bytes.CopyTo(data[
                cursor..(cursor += m12Bytes.Length)
            ].Span);
            m13Bytes.CopyTo(data[
                cursor..(cursor += m13Bytes.Length)
            ].Span);
            m14Bytes.CopyTo(data[
                cursor..(cursor += m14Bytes.Length)
            ].Span);

            m21Bytes.CopyTo(data[
                cursor..(cursor += m21Bytes.Length)
            ].Span);
            m22Bytes.CopyTo(data[
                cursor..(cursor += m22Bytes.Length)
            ].Span);
            m23Bytes.CopyTo(data[
                cursor..(cursor += m23Bytes.Length)
            ].Span);
            m24Bytes.CopyTo(data[
                cursor..(cursor += m24Bytes.Length)
            ].Span);

            m31Bytes.CopyTo(data[
                cursor..(cursor += m31Bytes.Length)
            ].Span);
            m32Bytes.CopyTo(data[
                cursor..(cursor += m32Bytes.Length)
            ].Span);
            m33Bytes.CopyTo(data[
                cursor..(cursor += m33Bytes.Length)
            ].Span);
            m34Bytes.CopyTo(data[
                cursor..(cursor += m34Bytes.Length)
            ].Span);

            m41Bytes.CopyTo(data[
                cursor..(cursor += m41Bytes.Length)
            ].Span);
            m42Bytes.CopyTo(data[
                cursor..(cursor += m42Bytes.Length)
            ].Span);
            m43Bytes.CopyTo(data[
                cursor..(cursor += m43Bytes.Length)
            ].Span);
            m44Bytes.CopyTo(data[
                cursor..(cursor += m44Bytes.Length)
            ].Span);

            return data;
        }
    }
}
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out Matrix4x4 value) {
            return global::EZBinarySerializer.ValueSerializers.SystemNumericsMatrix4x4ValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(Matrix4x4 value) {
            return global::EZBinarySerializer.ValueSerializers.SystemNumericsMatrix4x4ValueSerializer.ToBinary(value);
        }
    }
}
