---
title: Gdi32
---

# Gdi32
_namespace: [Microsoft.VisualBasic.Win32](N-Microsoft.VisualBasic.Win32.html)_





### Methods

#### Escape
```csharp
Microsoft.VisualBasic.Win32.Gdi32.Escape(System.Int64,System.Int64,System.Int64,System.String@,System.Object)
```
The Escape function allows applications to access capabilities of a particular device not directly available through GDI. Escape calls made by an application are translated and sent to the driver

|Parameter Name|Remarks|
|--------------|-------|
|hdc|Identifies the device context.|
|nEscape|Specifies the escape function to be performed. This parameter must be one of the predefined escape values. Use the ExtEscape function if your application defines a private escape value.|
|nCount|Specifies the number of bytes of data pointed to by the lpvInData parameter.|
|lpInData|Points to the input structure required for the specified escape.|
|lpOutData|
 Points to the structure that receives output from this escape. This parameter should be NULL if no data is returned
 
 If the function succeeds, the return value is greater than zero, except with the QUERYESCSUPPORT printer escape, which checks for implementation only. If the escape is not implemented, the return value is zero. 
 
 If the function fails, the return value is an error. To get extended error information, call GetLastError. 
 |



