---
title: DataModelRecord
---

# DataModelRecord
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.html)_






### Properties

#### DataModel
The model data of the datamodel, it is a XML model data.
 (数据模型中的数据，本属性值为一个XML格式的数据模型，需要使用特定类型的对象进行反序列化操作方可以读取)
#### DeleteSQL
Get the delete sql text for this datamodel object instance, please notice that the table name is empty, 
 so that you should replace the string "%s" with the table name that this sql text can be functionally.
 (获取本数据模型对象实例的删除SQL命令，请注意，在获得的命令语句之中，表名属性为空，故在使用之前请将"%s"占位符替
 换为表名，本语句方能够起作用。)
#### GUID
The global unique identifier for each data model object in the GCModeller database.
 (每一个数据模型对象在GCModeller数据库之中的唯一标识符)
#### InsertSQL
Get the insert sql text for this datamodel object instance, please notice that the table name is empty, 
 so that you should replace the string "%s" with the table name that this sql text can be functionally.
 (获取本数据模型对象实例的插入SQL命令，请注意，在获得的命令语句之中，表名属性为空，故在使用之前请将"%s"占位符替
 换为表名，本语句方能够起作用。)
#### RegistryNumber
The registry number for each datamodel object in its specific category table.
 (每一个数据模型对象在其所属分类之下的数据表之中的登录号)
#### UpdateSQL
Get the update sql text for this datamodel object instance, please notice that the table name is empty, 
 so that you should replace the string "%s" with the table name that this sql text can be functionally.
 (获取本数据模型对象实例的更新SQL命令，请注意，在获得的命令语句之中，表名属性为空，故在使用之前请将"%s"占位符替
 换为表名，本语句方能够起作用。)
