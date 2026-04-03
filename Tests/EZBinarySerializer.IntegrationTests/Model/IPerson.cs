using System;
using System.Collections.Generic;
using System.Text;

namespace EZBinarySerializer.IntegrationTests.Model {
    [BinarySerializable]
    public partial interface IPerson {
        public string Name {
            get; set;
        }
    }
}
