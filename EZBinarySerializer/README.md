[![GitHub stars](https://img.shields.io/github/stars/toeverything/AFFiNE?style=social)](https://github.com/toeverything/AFFiNE)
[![License](https://img.shields.io/badge/License-AGPLv3-green.svg)](https://opensource.org/license/agpl-3-0)

## 🚀 Quick Start

### 1. Install
```powershell
nuget install EZBinarySerializer
```

### 2. Basic Usage
```cs
using EZBinarySerializer;

namespace MyNamespace;

[BinarySerializable]                    // Use the BinarySerializable attribute.
public partial class MyClass {          // Mark your class a partial to allow extension.
    public int MyPublicValue = 0;       // Public fields and properties will be serialized.
    private int MyPrivateValue = 0;     // Private fields will be ignored.
    [BinarySerializerIgnore]            // Use BinarySerializableIgnore attribute to ignore member.
    public int MyOtherPublicValue = 0;
}

MyClass myClass = new();
Memory<byte> data = MyClass.ToBinary(myClass);  // Serialize with the static method on the class.
int bytesRead = MyClass.FromBinary(data, out MyClass deserialized);  // Deserialize the same way.

```

### 3. Build your project

Build using MSBUILD and the source generator will run and provide the code extensions needed for serialization.

⚡ **Done!**

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🦆 Polymorphism | Polymorphic serialization with interfaces and abstract classes |
| 🎨 Standard Types | Support for many standard types including selections from System.Numerics and System.Collections.Generic |
| 🕹 AOT compatible | No reflection, none, zilch |
| 🚲 Easy to use | Simple attribute based interface |
| 📏 Extend across multiple assemblies | Inherit models from other projects and make your own additions |
