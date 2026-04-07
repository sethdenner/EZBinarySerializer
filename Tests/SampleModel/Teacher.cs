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
using EZBinarySerializer;
using System.Security.Cryptography;

namespace SampleModel {
    [BinarySerializable]
    public partial class Teacher : IPerson {
        public string Name { get; set; } = string.Empty;
        public Subject SubjectTaught = new();
        public List<Student> Students = [];

        public static bool operator ==(Teacher teacher1, Teacher teacher2) {
            if (teacher1 is null) {
                return teacher2 is null;
            }

            return teacher1.Equals(teacher2);
        }

        public static bool operator !=(Teacher teacher1, Teacher teacher2) {
            if (teacher1 is null) {
                return teacher2 is not null;
            }

            return !teacher1.Equals(teacher2);
        }

        public override int GetHashCode() {
            return BitConverter.ToInt32(
                MD5.HashData(
                    ToBinary(this).ToArray()
                ).AsSpan()[..sizeof(int)]
            );
        }

        public bool Equals(IPerson? other) {
            if (
                other is not Teacher otherTeacher ||
                Name != otherTeacher.Name ||
                SubjectTaught != otherTeacher.SubjectTaught ||
                Students.Count != otherTeacher.Students?.Count
            ) {
                return false;
            }

            for (int i = 0; i < Students.Count; ++i) {
                var student = Students[i];
                var otherStudent = otherTeacher.Students[i];
                if (student != otherStudent) {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as Teacher);
        }
    }
}
