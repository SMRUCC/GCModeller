# InternalCommands
_namespace: [Microsoft.VisualBasic.Shoal](./index.md)_

This module provides some common operation in the shoal scripting.



### Methods

#### BatchInvoke
```csharp
Microsoft.VisualBasic.Shoal.InternalCommands.BatchInvoke(System.String,System.Boolean)
```
批量执行指定的文件夹之中的所有Shoal脚本

#### SourceCopy
```csharp
Microsoft.VisualBasic.Shoal.InternalCommands.SourceCopy(System.Collections.Generic.IEnumerable{System.String},System.String,System.String)
```
更加一般性的复制函数，当目标文件夹之中的文件数目非常的多的时候，可以使用这个函数进行批量的文件复制，只需要把文件名填入列表之中即可，大小写无关

_returns: 复制失败的文件名列表_


