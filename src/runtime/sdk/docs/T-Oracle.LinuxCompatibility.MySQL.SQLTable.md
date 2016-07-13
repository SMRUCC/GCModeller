---
title: SQLTable
---

# SQLTable
_namespace: [Oracle.LinuxCompatibility.MySQL](N-Oracle.LinuxCompatibility.MySQL.html)_





### Methods

#### GetReplaceSQL
```csharp
Oracle.LinuxCompatibility.MySQL.SQLTable.GetReplaceSQL
```
如果已经存在了一条相同主键值的记录，则删除它然后在插入更新值；
 假若不存在，则直接插入新数据，这条命令几乎等价于@"M:Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.SQLTable.GetInsertSQL"命令，所不同的是这个会自动处理旧记录，可能会不安全，
 因为旧记录可能会在你不知情的情况下被意外的更新了；
 并且由于需要先判断记录是否存在，执行的速度也比直接的Insert操作要慢一些，大批量数据插入不建议这个操作


