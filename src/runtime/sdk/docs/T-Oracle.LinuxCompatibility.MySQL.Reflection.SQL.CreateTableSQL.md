---
title: CreateTableSQL
---

# CreateTableSQL
_namespace: [Oracle.LinuxCompatibility.MySQL.Reflection.SQL](N-Oracle.LinuxCompatibility.MySQL.Reflection.SQL.html)_

Generate the CREATE TABLE sql of the target table schema class object.
 (生成目标数据表模式的"CREATE TABLE" sql语句)

> 
>  Example SQL:
>  
>  CREATE  TABLE `Table_Name` (
>    `Field1` INT UNSIGNED ZEROFILL NOT NULL DEFAULT 4444 ,
>    `Field2` VARCHAR(45) BINARY NOT NULL DEFAULT '534534' ,
>    `Field3` INT UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT ,
>   PRIMARY KEY (`Field1`, `Field2`, `Field3`) ,
>   UNIQUE INDEX `Field1_UNIQUE` (`Field1` ASC) ,
>   UNIQUE INDEX `Field2_UNIQUE` (`Field2` ASC) );
>  


### Methods

#### FromSchema
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.SQL.CreateTableSQL.FromSchema(Oracle.LinuxCompatibility.MySQL.Reflection.Schema.Table)
```
Generate the 'CREATE TABLE' sql command.
 (生成'CREATE TABLE' sql命令)


