#Region "Microsoft.VisualBasic::7398a835ce062a0a729036fed0a25f63, ..\SQLite_Interface\Reflector\SchemaCache.vb"

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
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

''' <summary>
''' Cached for the database schema.
''' </summary>
''' <remarks></remarks>
Public Class SchemaCache

    Public Property [Property] As System.Reflection.PropertyInfo
    Public Property DbFieldName As String
    Public Property FieldEntryPoint As ColumnAttribute

    ''' <summary>
    ''' integer(size)    仅容纳整数。在括号内规定数字的最大位数。
    ''' int(size)
    ''' smallint(size)
    ''' tinyint(size)
    ''' decimal(size,d)  容纳带有小数的数字。"size" 规定数字的最大位数。"d" 规定小数点右侧的最大位数。
    ''' numeric(size,d)
    ''' char(size)	     容纳固定长度的字符串（可容纳字母、数字以及特殊字符）。在括号中规定字符串的长度。
    ''' 
    ''' varchar(size)	 容纳可变长度的字符串（可容纳字母、数字以及特殊的字符）。在括号中规定字符串的最大长度。
    ''' date(yyyymmdd)
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property DbType As String
        Get
            If Not String.IsNullOrEmpty(FieldEntryPoint.DbType) Then
                Return FieldEntryPoint.DbType
            End If

            Dim MyType As Type = [Property].PropertyType
            Dim LQuery As String =
                LinqAPI.DefaultFirst(Of String) <= From type As NamedValue(Of Type)
                                                   In TypeMapping.Values
                                                   Where type.Value = MyType
                                                   Select type.Name

            If String.IsNullOrEmpty(LQuery) Then
                Dim msg As String =
                    String.Format(DATA_TYPE_IS_NOT_SUPPORT, MyType.FullName)
                Throw New DataException(msg)
            Else
                Return LQuery
            End If
        End Get
    End Property

    Const DATA_TYPE_IS_NOT_SUPPORT As String = "TYPE_NOT_SUPPORT::The SQLite database is not support the type ""{0}"""

    Public Overrides Function ToString() As String
        Return DbFieldName
    End Function

    Public Shared ReadOnly Property TypeMapping As Dictionary(Of NamedValue(Of Type)) =
        New Dictionary(Of NamedValue(Of Type)) From {
 _
            {"integer", New NamedValue(Of Type)("integer", GetType(Long))},
            {"int", New NamedValue(Of Type)("int", GetType(Integer))},
            {"smallint", New NamedValue(Of Type)("smallint", GetType(Short))},
            {"tinyint", New NamedValue(Of Type)("tinyint", GetType(Byte))},
            {"decimal", New NamedValue(Of Type)("decimal", GetType(Decimal))},
            {"numeric", New NamedValue(Of Type)("numeric", GetType(Double))},
            {"char", New NamedValue(Of Type)("char", GetType(String))},
            {"varchar", New NamedValue(Of Type)("varchar", GetType(String))},
            {"date", New NamedValue(Of Type)("date", GetType(Date))}
    }

    ''' <summary>
    ''' INSERT INTO table_name (col1, col2, ...) VALUES (value1, value2, ....)
    ''' </summary>
    ''' <param name="SchemaCache"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateInsertSQL(Of T As Class)(SchemaCache As SchemaCache(), obj As T, TableName As String) As String
        Dim Values As String = String.Join(", ", (From p As SchemaCache In SchemaCache Select __getValue(p, obj)).ToArray)
        Dim Columns As String = String.Join(", ", (From p As SchemaCache In SchemaCache Select p.DbFieldName).ToArray)
        Dim SQL As String = String.Format("INSERT INTO '{0}' ({1}) VALUES ({2}) ;", TableName, Columns, Values)
        Return SQL
    End Function

    Public Shared Function CreateInsertSQL(Of T As Class)(x As T) As String
        Dim type As Type = GetType(T)
        Dim schema = Reflector.InternalGetSchemaCache(type)
        Dim tbl As String = type.GetTableName
        Return CreateInsertSQL(schema, x, tbl)
    End Function

    ''' <summary>
    ''' INSERT INTO table_name (col1, col2, ...) VALUES (value1, value2, ....)
    ''' </summary>
    ''' <param name="SchemaCache"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateInsertSQL(SchemaCache As SchemaCache(), obj As Object, TableName As String) As String
        Dim Values As String = String.Join(", ", (From p As SchemaCache In SchemaCache Select __getValue(p, obj)).ToArray)
        Dim Columns As String = String.Join(", ", (From p As SchemaCache In SchemaCache Select p.DbFieldName).ToArray)
        Dim SQL As String = String.Format("INSERT INTO '{0}' ({1}) VALUES ({2}) ;", TableName, Columns, Values)
        Return SQL
    End Function

    ''' <summary>
    ''' 请注意，函数的返回值是带有数据库之中的间隔符号'的 
    ''' </summary>
    ''' <param name="p"></param>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    Friend Shared Function __getValue(p As SchemaCache, obj As Object) As String
        Dim value = p.Property.GetValue(obj)
        Dim s As String = If(value Is Nothing, "", value.ToString)
        Return "'" & s.Replace("'", "''") & "'"
    End Function

    ''' <summary>
    ''' CREATE TABLE TableName
    ''' (
    '''   col1 dbtype,
    '''   col2 dbtype,
    '''   col3 dbtype,
    '''   ....
    ''' )
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateTableSQL(SchemaCache As SchemaCache(), TableName As String) As String
        Dim Fields As String() = (From p As SchemaCache In SchemaCache Select p.DbFieldName & " " & p.DbType & If(p.FieldEntryPoint.IsPrimaryKey, " primary key", "")).ToArray
        Dim SQL As String = String.Format(<SQL_CREATE_TABLE>CREATE TABLE {0} ( 
{1} );</SQL_CREATE_TABLE>.Value, TableName, String.Join("," & vbCrLf, Fields))
        Return SQL
    End Function

    ''' <summary>
    ''' CREATE TABLE TableName
    ''' (
    '''   col1 dbtype,
    '''   col2 dbtype,
    '''   col3 dbtype,
    '''   ....
    ''' )
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateTableSQL(SchemaCache As TableDump()) As String
        Dim Fields As String() = (From p As TableDump In SchemaCache Select p.FieldName & " " & p.DbType & If(p.IsPrimaryKey = 1, " primary key", "")).ToArray
        Dim SQL As String = String.Format(<SQL_CREATE_TABLE>CREATE TABLE {0} ( 
{1} );</SQL_CREATE_TABLE>.Value, SchemaCache.First.TableName, String.Join("," & vbCrLf, Fields))
        Return SQL
    End Function

    Public Shared Function CreateTableSQL(TypeInfo As Type) As String
        Dim SchemaCache As SchemaCache() = InternalGetSchemaCache(TypeInfo)
        Dim SQL As String = CreateTableSQL(SchemaCache, GetTableName(TypeInfo))
        Return SQL
    End Function

    ''' <summary>
    ''' UPDATE table_name SET col = value WHERE col = colName
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="SchemaCache"></param>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateUpdateSQL(Of T As Class)(SchemaCache As SchemaCache(), obj As T, TableName As String) As String
        Dim SetColumnValues As String = String.Join(", ", (From p As SchemaCache In SchemaCache Select p.DbFieldName & " = " & __getValue(p, obj)).ToArray)
        Dim PrimaryKey = (From p As SchemaCache In SchemaCache Where p.FieldEntryPoint.IsPrimaryKey Select p).FirstOrDefault
        If PrimaryKey Is Nothing Then
            PrimaryKey = SchemaCache.First
        End If
        Dim SQL As String = String.Format("UPDATE '{0}' SET {1} WHERE {2} = {3} ;", TableName, SetColumnValues, PrimaryKey.DbFieldName, __getValue(PrimaryKey, obj))
        Return SQL
    End Function

    ''' <summary>
    ''' UPDATE table_name SET col = value WHERE col = colName
    ''' </summary>
    ''' <param name="SchemaCache"></param>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateUpdateSQL(SchemaCache As SchemaCache(), obj As Object, TableName As String) As String
        Dim SetColumnValues As String = String.Join(", ", (From p As SchemaCache In SchemaCache Select p.DbFieldName & " = " & __getValue(p, obj)).ToArray)
        Dim PrimaryKey = (From p As SchemaCache In SchemaCache Where p.FieldEntryPoint.IsPrimaryKey Select p).FirstOrDefault
        If PrimaryKey Is Nothing Then
            PrimaryKey = SchemaCache.First
        End If
        Dim SQL As String = String.Format("UPDATE '{0}' SET {1} WHERE {2} = {3} ;", TableName, SetColumnValues, PrimaryKey.DbFieldName, __getValue(PrimaryKey, obj))
        Return SQL
    End Function

    ''' <summary>
    ''' DELETE FROM table_name WHERE col = value
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="SchemaCache"></param>
    ''' <param name="obj"></param>
    ''' <param name="TableName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateDeleteSQL(Of T As Class)(SchemaCache As SchemaCache(), obj As T, TableName As String) As String
        Dim PrimaryKey = (From p As SchemaCache In SchemaCache Where p.FieldEntryPoint.IsPrimaryKey Select p).FirstOrDefault
        If PrimaryKey Is Nothing Then
            PrimaryKey = SchemaCache.First
        End If
        Dim SQL As String = String.Format("DELETE FROM '{0}' WHERE {1} = {2} ;", TableName, PrimaryKey.DbFieldName, __getValue(PrimaryKey, obj))
        Return SQL
    End Function

    ''' <summary>
    ''' DELETE FROM table_name WHERE col = value
    ''' </summary>
    ''' <param name="SchemaCache"></param>
    ''' <param name="obj"></param>
    ''' <param name="TableName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateDeleteSQL(SchemaCache As SchemaCache(), obj As Object, TableName As String) As String
        Dim PrimaryKey = (From p As SchemaCache In SchemaCache Where p.FieldEntryPoint.IsPrimaryKey Select p).FirstOrDefault
        If PrimaryKey Is Nothing Then
            PrimaryKey = SchemaCache.First
        End If
        Dim SQL As String = String.Format("DELETE FROM '{0}' WHERE {1} = {2} ;", TableName, PrimaryKey.DbFieldName, __getValue(PrimaryKey, obj))
        Return SQL
    End Function
End Class
