using System;
using System.Collections.Generic;
using System.Text;

namespace EZBinarySerializer.IntegrationTests.Model {
    [BinarySerializable]
    public partial class Student : IPerson {
        public string Name { get; set; } = string.Empty;
        public int Year = 0;
        public Subject[] Subjects = new Subject[6];
    }
}
