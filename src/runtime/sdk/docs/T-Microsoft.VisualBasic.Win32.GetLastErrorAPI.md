---
title: GetLastErrorAPI
---

# GetLastErrorAPI
_namespace: [Microsoft.VisualBasic.Win32](N-Microsoft.VisualBasic.Win32.html)_

Wrapper for the returns value of api @"M:Microsoft.VisualBasic.Win32.GetLastErrorAPI.GetLastError"



### Methods

#### GetLastError
```csharp
Microsoft.VisualBasic.Win32.GetLastErrorAPI.GetLastError
```
针对之前调用的api函数，用这个函数取得扩展错误信息（在vb里使用：在vb中，用Err对象的GetLastError属性获取GetLastError的值。
 这样做是必要的，因为在api调用返回以及vb调用继续执行期间，vb有时会重设GetLastError的值）
 
 GetLastError返回的值通过在api函数中调用SetLastError或SetLastErrorEx设置。函数
 并无必要设置上一次错误信息，所以即使一次GetLastError调用返回的是零值，也不能
 担保函数已成功执行。只有在函数调用返回一个错误结果时，这个函数指出的错误结果
 才是有效的。通常，只有在函数返回一个错误结果，而且已知函数会设置GetLastError
 变量的前提下，才应访问GetLastError；这时能保证获得有效的结果。SetLastError函
 数主要在对api函数进行模拟的dll函数中使用。
> 
>  GetLastError返回的值通过在api函数中调用SetLastError或SetLastErrorEx设置。函数并无必要设置上一次错误信息，
>  所以即使一次GetLastError调用返回的是零值，也不能担保函数已成功执行。只有在函数调用返回一个错误结果时，
>  这个函数指出的错误结果才是有效的。通常，只有在函数返回一个错误结果，而且已知函数会设置GetLastError变量的前提下，
>  才应访问GetLastError；这时能保证获得有效的结果。SetLastError函数主要在对api函数进行模拟的dll函数中使用，
>  所以对vb应用程序来说是没有意义的
>  


