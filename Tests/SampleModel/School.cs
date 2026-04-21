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
using System.Numerics;
using System.Security.Cryptography;

namespace SampleModel {
    [BinarySerializable]
    public partial class School : IBuilding {
        public List<Teacher> Teachers = [];
        public List<Student> Students = [];

        public Vector2 GeoCoordinates {
            get; set;
        }

        public string BuildingName => "Lakeside Middle School";

        public static bool operator ==(School school1, School school2) {
            if (school1 is null) {
                return school2 is null;
            }

            return school1.Equals(school2);
        }

        public static bool operator !=(School school1, School school2) {
            if (school1 is null) {
                return school2 is not null;
            }

            return !school1.Equals(school2);
        }

        public override bool Equals(object? obj) {
            return Equals(obj as School);
        }

        public override int GetHashCode() {
            return BitConverter.ToInt32(
                MD5.HashData(
                    ToBinary(this).ToArray()
                ).AsSpan()[..sizeof(int)]
            );
        }

        public bool Equals(IBuilding? other) {
            if (
                other is not School otherSchool ||
                Teachers.Count != otherSchool.Teachers.Count ||
                Students.Count != otherSchool.Students.Count ||
                GeoCoordinates != otherSchool.GeoCoordinates
            ) {
                return false;
            }
            for (int i = 0; i < Teachers.Count; ++i) {
                var teacher = Teachers[i];
                var otherTeacher = otherSchool.Teachers[i];
                if (teacher != otherTeacher) {
                    return false;
                }
            }
            for (int i = 0; i < Students.Count; ++i) {
                var student = Students[i];
                var otherStudent = otherSchool.Students[i];
                if (student != otherStudent) {
                    return false;
                }
            }
            return true;
        }
    }
}
