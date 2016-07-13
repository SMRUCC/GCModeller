---
title: DataType
---

# DataType
_namespace: [Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes](N-Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.html)_

Please notice that some data type in mysql database is not allow combine with some specific field 
 attribute, and I can't find out this potential error in this code. So, when your schema definition can't 
 create a table then you must check this combination is correct or not in the mysql.
 (请注意：在MySql数据库中有一些数据类型是不能够和一些字段的属性组合使用的，我不能够在本代码中检查出此潜在
 的错误。故，当你定义的对象类型无法创建表的时候，请检查你的字段属性的组合是否有错误？)



### Methods

#### ToMySqlDateTimeString
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.DataType.ToMySqlDateTimeString(System.DateTime)
```
可能由于操作系统的语言或者文化的差异，直接使用@"T:System.DateTime"的ToString方法所得到的字符串可能会在一些环境配置之下
 无法正确的插入MySQL数据库之中，所以需要使用本方法在将对象实例进行转换为SQL语句的时候进行转换为字符串

|Parameter Name|Remarks|
|--------------|-------|
|value|-|



