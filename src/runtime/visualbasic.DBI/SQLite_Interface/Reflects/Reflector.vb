#Region "Microsoft.VisualBasic::d4c06ebe6606f3a676c2204a50644d75, ..\SQLite_Interface\Reflector\Reflector.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports System.Data.Common
Imports System.Reflection
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

''' <summary>
''' Provides the reflection operation extensions for the generic collection data to updates database.
''' </summary>
''' <remarks></remarks>
Public Module Reflector

    Private ReadOnly DbFieldEntryPoint As System.Type = GetType(ColumnAttribute)

    ''' <summary>
    ''' Load the data from SELECT sql statement into a specific type collection.(Select语句加载数据)
    ''' </summary>
    ''' <typeparam name="T">The object type of the collection will be generated.</typeparam>
    ''' <param name="DbTransaction"></param>
    ''' <param name="SQL">SELECT SQL.(必须为Select语句)</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 主要解析<see cref="System.Data.Linq.Mapping.ColumnAttribute.Name"></see>，<see cref="System.Data.Linq.Mapping.ColumnAttribute.IsPrimaryKey"></see>，
    ''' <see cref="System.Data.Linq.Mapping.ColumnAttribute.DbType"></see>这几个参数
    ''' </remarks>
    <Extension> Public Iterator Function Load(Of T As Class)(DbTransaction As SQLProcedure, SQL As String) As IEnumerable(Of T)
        For Each o As Object In Load(DbTransaction, GetType(T), SQL)
            Yield DirectCast(o, T)
        Next
    End Function

    ''' <summary>
    ''' Load the data from SELECT sql statement into a specific type collection.(Select语句加载数据)
    ''' </summary>
    ''' <param name="DbTransaction"></param>
    ''' <param name="SQL">SELECT SQL.(必须为Select语句)</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 主要解析<see cref="System.Data.Linq.Mapping.ColumnAttribute.Name"></see>，<see cref="System.Data.Linq.Mapping.ColumnAttribute.IsPrimaryKey"></see>，
    ''' <see cref="System.Data.Linq.Mapping.ColumnAttribute.DbType"></see>这几个参数
    ''' </remarks>
    <Extension> Public Iterator Function Load(DbTransaction As SQLProcedure, schemaInfo As Type, SQL As String) As IEnumerable(Of Object)
        Dim Properties As SchemaCache() = Reflector.InternalGetSchemaCache(schemaInfo)
        Dim TableName As String = GetTableName(schemaInfo)
        Dim DataSource As DbDataReader =
            DbTransaction.Execute(SQL)
        Dim SchemaCache = (From Field As SchemaCache
                           In Properties
                           Let idx As Integer = DataSource.GetOrdinal(name:=Field.DbFieldName)
                           Where idx > -1
                           Select idx,
                               Field.FieldEntryPoint,
                               Field.Property).ToArray
        Dim FetchedObject As Object
        Dim value As Object

        Do While DataSource.Read
            FetchedObject = Activator.CreateInstance(type:=schemaInfo)

            For Each field In SchemaCache
                value = DataSource.GetValue(ordinal:=field.idx)
                value = Scripting.CTypeDynamic(
                    Scripting.ToString(value),
                    field.Property.PropertyType)
                Call field.Property.SetValue(FetchedObject, value)
            Next

            Yield FetchedObject
        Loop
    End Function

    ''' <summary>
    ''' Load all of the database in the database from a specific type information.
    ''' </summary>
    ''' <param name="DataSource"></param>
    ''' <param name="TypeInfo">The type information of the database table.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function Load(DataSource As DbDataReader, TypeInfo As Type) As Object()
        Dim Table As New List(Of Object)
        Dim SchemaCache = (From Field In Reflector.InternalGetSchemaCache(TypeInfo)
                           Let idx As Integer = DataSource.GetOrdinal(name:=Field.DbFieldName)
                           Where idx > -1
                           Select idx, Field.FieldEntryPoint, Field.Property).ToArray

        Do While DataSource.Read
            Dim FetchedObject As Object = Activator.CreateInstance(TypeInfo)

            For Each SchemaField In SchemaCache
                Call SchemaField.Property.SetValue(FetchedObject, value:=DataSource.GetValue(ordinal:=SchemaField.idx))
            Next

            Call Table.Add(FetchedObject)
        Loop

        Return Table.ToArray
    End Function

    ''' <summary>
    ''' 这个函数会一次性加载所有的数据
    ''' </summary>
    ''' <param name="DbTransaction"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Iterator Function Load(Of T As Class)(DbTransaction As SQLProcedure) As IEnumerable(Of T)
        Dim buf As IEnumerable(Of Object) =
            DbTransaction.Load(SchemaInfo:=GetType(T))

        For Each x As Object In buf
            Yield DirectCast(x, T)
        Next
    End Function

    ''' <summary>
    ''' 这个函数会一次性加载所有的数据
    ''' </summary>
    ''' <param name="DbTransaction"></param>
    ''' <param name="SchemaInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Iterator Function Load(DbTransaction As SQLProcedure, SchemaInfo As Type) As IEnumerable(Of Object)
        Dim TableName As String = GetTableName(SchemaInfo)
        Dim SQL As String = $"SELECT * FROM '{TableName}';"

        For Each x As Object In Load(DbTransaction, SchemaInfo, SQL)
            Yield x
        Next
    End Function

    ''' <summary>
    ''' Loading the database table schema information using the reflection operation from the meta data storted in the type schema.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function __getSchemaCache(Of T As Class)() As SchemaCache()
        Dim TypeInfo As Type = GetType(T)
        Dim Properties As SchemaCache() = InternalGetSchemaCache(TypeInfo)
        Return Properties
    End Function

    ''' <summary>
    ''' Loading the database table schema information using the reflection operation from the meta data storted in the type schema.
    ''' </summary>
    ''' <param name="type"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function InternalGetSchemaCache(type As Type) As SchemaCache()
        Dim Properties As SchemaCache() =
            LinqAPI.Exec(Of SchemaCache) <= From prop As PropertyInfo
                                            In type.GetProperties()
                                            Let attrs As Object() =
                                                prop.GetCustomAttributes(
                                                    attributeType:=DbFieldEntryPoint,
                                                    inherit:=True)
                                            Where Not attrs.IsNullOrEmpty
                                            Let field As ColumnAttribute =
                                                DirectCast(attrs.First, ColumnAttribute)
                                            Let fName As String =
                                                If(String.IsNullOrEmpty(field.Name), prop.Name, field.Name)
                                            Select New SchemaCache With {
                                                .Property = prop,
                                                .DbFieldName = fName,
                                                .FieldEntryPoint = field
                                            }

        If Properties.IsNullOrEmpty Then
            Dim msg As String =
                String.Format(SQLITE_UNABLE_LOAD_SQL_MAPPING_SCHEMA, type.FullName)
            Throw New DataException(msg)
        Else
            Return Properties
        End If
    End Function

    Const SQLITE_UNABLE_LOAD_SQL_MAPPING_SCHEMA As String = "SQLITE_UNABLE_LOAD_SQL_MAPPING_SCHEMA::Could not load any sql schema from table_schema_info ""{0}"""

    ''' <summary>
    ''' Get the table name from the type schema.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTableName(Of T As Class)() As String
        Dim type As Type = GetType(T)
        Return GetTableName(type)
    End Function

    ''' <summary>
    ''' Get the table name from the type schema.
    ''' </summary>
    ''' <param name="type"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetTableName(type As Type) As String
        Dim attrs As Object() =
            type.GetCustomAttributes(GetType(TableAttribute), inherit:=True)

        If attrs.IsNullOrEmpty Then
            Return type.Name
        Else
            Dim tblName As String = DirectCast(attrs.First, TableAttribute).Name
            Return If(String.IsNullOrEmpty(tblName), type.Name, tblName)
        End If
    End Function

    Public Function Delete(DbTransaction As SQLProcedure, obj As Object) As Boolean
        Dim TypeInfo = obj.GetType
        Dim TableName As String = Reflector.GetTableName(TypeInfo)
        Dim Properties As SchemaCache() =
            Reflector.InternalGetSchemaCache(TypeInfo)
        Dim SQL As String =
            SchemaCache.CreateDeleteSQL(Properties, obj, TableName)

        Try
            Call DbTransaction.Execute(SQL)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    <Extension> Public Function Delete(Of T As Class)(DbTransaction As SQLProcedure, obj As T) As Boolean
        Dim TableName As String = Reflector.GetTableName(Of T)()
        Dim Properties As SchemaCache() = Reflector.__getSchemaCache(Of T)()
        Dim SQL As String =
            SchemaCache.CreateDeleteSQL(Of T)(Properties, obj, TableName)

        Try
            Call DbTransaction.Execute(SQL)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 批量删除数据
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="DbTransaction"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function FlushData(Of T As Class)(DbTransaction As SQLProcedure, data As IEnumerable(Of T)) As Boolean
        Dim TableName As String = Reflector.GetTableName(Of T)()
        Dim Properties As SchemaCache() = Reflector.__getSchemaCache(Of T)()
        Dim SQLTransactions As String() =
            LinqAPI.Exec(Of String) <= From x As T
                                       In data
                                       Select SchemaCache.CreateDeleteSQL(Of T)(
                                           Properties,
                                           x,
                                           TableName)
        Return DbTransaction.ExecuteTransaction(SQLTransactions)
    End Function

    <Extension> Public Function Update(Of T As Class)(DbTransaction As SQLProcedure, obj As T) As Boolean
        Dim TableName As String = Reflector.GetTableName(Of T)()
        Dim Properties As SchemaCache() = Reflector.__getSchemaCache(Of T)()
        Dim SQL As String = SchemaCache.CreateUpdateSQL(Of T)(
            Properties,
            obj,
            TableName)

        Try
            Call DbTransaction.Execute(SQL)
        Catch ex As Exception
            ex = New Exception(SQL, ex)

            Call App.LogException(ex)
            Call ex.PrintException

            Return False
        End Try

        Return True
    End Function

    Public Function Update(DbTransaction As SQLProcedure, obj As Object) As Boolean
        Dim Table = New TableSchema(type:=obj.GetType)
        Dim SQL As String = SchemaCache.CreateUpdateSQL(
            Table.DatabaseFields,
            obj,
            Table.TableName)

        Try
            Call DbTransaction.Execute(SQL)
        Catch ex As Exception
            ex = New Exception(SQL, ex)

            Call App.LogException(ex)
            Call ex.PrintException

            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' 批量更新数据
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="DbTransaction"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function CommitData(Of T As Class)(DbTransaction As SQLProcedure, data As IEnumerable(Of T)) As Boolean
        Dim TableName As String = Reflector.GetTableName(Of T)()
        Dim Properties As SchemaCache() = Reflector.__getSchemaCache(Of T)()
        Dim SQLTransactions As String() =
            LinqAPI.Exec(Of String) <= From obj As T
                                       In data
                                       Select SchemaCache.CreateUpdateSQL(Of T)(
                                           Properties,
                                           obj,
                                           TableName)
        Return DbTransaction.ExecuteTransaction(SQLTransactions)
    End Function

    ''' <summary>
    ''' 批量添加新的数据
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="DbTransaction"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function Insert(Of T As Class)(DbTransaction As SQLProcedure, data As IEnumerable(Of T)) As Boolean
        Dim TableName As String = Reflector.GetTableName(Of T)()
        Dim Properties As SchemaCache() = Reflector.__getSchemaCache(Of T)()
        Dim SQLTransactions As String() = (From obj As T In data Select SchemaCache.CreateInsertSQL(Of T)(Properties, obj, TableName)).ToArray
        Return DbTransaction.ExecuteTransaction(SQLTransactions)
    End Function

    ''' <summary>
    ''' INSERT INTO Table VALUES (value1, value2, ....)
    ''' INSERT INTO table_name (col1, col2, ...) VALUES (value1, value2, ....)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="DbTransaction"></param>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function Insert(Of T As Class)(DbTransaction As SQLProcedure, obj As T) As Boolean
        Dim TableName As String = Reflector.GetTableName(Of T)()
        Dim Properties As SchemaCache() = Reflector.__getSchemaCache(Of T)()
        Dim SQL As String = SchemaCache.CreateInsertSQL(Of T)(Properties, obj, TableName)

        Try
            Call DbTransaction.Execute(SQL)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' INSERT INTO Table VALUES (value1, value2, ....)
    ''' INSERT INTO table_name (col1, col2, ...) VALUES (value1, value2, ....)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="DbTransaction"></param>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function Insert(Of T As Class)(DbTransaction As SQLProcedure, TableSchema As TableSchema, obj As T) As Boolean
        Dim TableName As String = TableSchema.TableName
        Dim Properties As SchemaCache() = TableSchema.DatabaseFields
        Dim SQL As String = SchemaCache.CreateInsertSQL(Of T)(Properties, obj, TableName)

        Try
            Call DbTransaction.Execute(SQL)
            Return True
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try
    End Function

    Public Function Insert(DbTransaction As SQLProcedure, obj As Object) As Boolean
        Dim TypeInfo As System.Type = obj.GetType
        Dim TableName As String = Reflector.GetTableName(TypeInfo)
        Dim Properties As SchemaCache() = Reflector.InternalGetSchemaCache(TypeInfo)
        Dim SQL As String = SchemaCache.CreateInsertSQL(Properties, obj, TableName)

        Try
            Call DbTransaction.Execute(SQL)
            Return True
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 判断某一个实体对象是否存在于数据库之中
    ''' </summary>
    ''' <param name="DbTransaction"></param>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <Extension>
    Public Function RecordExists(DbTransaction As SQLProcedure, obj As Object) As Boolean
        Dim TableSchema As New TableSchema(type:=obj.GetType)
        Dim PrimaryKey As SchemaCache = TableSchema.PrimaryKey.First
        Dim value As String = SchemaCache.__getValue(PrimaryKey, obj)
        Return DbTransaction.RecordExists(TableSchema, key:=value)
    End Function

    <Extension>
    Public Function RecordExists(DbTransaction As SQLProcedure, tableSchema As TableSchema, key As String) As Boolean
        Dim PrimaryKey As SchemaCache = tableSchema.PrimaryKey.First
        Dim tblName As String = tableSchema.TableName
        Dim SQL As String = $"SELECT * FROM '{tblName}' WHERE {PrimaryKey.DbFieldName} = {key};"
        Dim Result As DbDataReader = DbTransaction.Execute(SQL)
        Return Result.HasRows
    End Function
End Module

