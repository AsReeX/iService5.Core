// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.User.ActivityLogging.LoggingContext
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;

#nullable disable
namespace iService5.Core.Services.User.ActivityLogging;

[Flags]
public enum LoggingContext
{
  USER = 0,
  LOGIN = 1,
  BACKEND = 2,
  METADATA = 4,
  LOCAL = 8,
  BINARY = 16, // 0x00000010
  USERPOPUPSYNC = 32, // 0x00000020
  USERPOPUPASYNC = 64, // 0x00000040
  APPLIANCE = 128, // 0x00000080
  SECURESTORAGE = 256, // 0x00000100
  USERSESSION = 512, // 0x00000200
  PROGRAMSMM = 1024, // 0x00000400
  ERRORSMM = 2048, // 0x00000800
  PROGRAMNONSMM = 4096, // 0x00001000
  MEASURE = 8192, // 0x00002000
  MONITORING = 16384, // 0x00004000
  CODING = 32768, // 0x00008000
  BRIDGESETTINGS = 65536, // 0x00010000
  CSR = 131072, // 0x00020000
  WEBSOCKET = 262144, // 0x00040000
  PROGRAMSYMANA = 524288, // 0x00080000
  CONTROL = WEBSOCKET | CSR | BRIDGESETTINGS | CODING | MONITORING | MEASURE | PROGRAMNONSMM | ERRORSMM | PROGRAMSMM | USERSESSION | SECURESTORAGE | APPLIANCE | USERPOPUPSYNC | LOCAL, // 0x0007FFA8
}
