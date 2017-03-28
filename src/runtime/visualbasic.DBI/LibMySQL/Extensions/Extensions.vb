#Region "Microsoft.VisualBasic::661320cc38b8d623dab7878619430dc1, ..\LibMySQL\Extensions\CommonExtension.vb"

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

Imports System.Data.Common
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Public Module Extensions

    ''' <summary>
    ''' 读取CreateTable元数据
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Function GetCreateTableMetaSQL(Of T As SQLTable)() As String
        Dim attrs As TableName = GetType(T).GetCustomAttribute(Of TableName)

        If attrs Is Nothing Then
            Return Nothing
        Else
            Return attrs.SchemaSQL
        End If
    End Function

    ''' <summary>
    ''' 如果成功，则返回空值，如果不成功，会返回错误消息
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="mysql"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ClearTable(Of T As SQLTable)(mysql As MySQL) As String
        Dim SQL As New Value(Of String)

        If mysql.Ping = -1.0R Then
            Return "No MySQL connection!"
        End If

        If Not String.IsNullOrEmpty(SQL = GetCreateTableMetaSQL(Of T)()) Then
            Call mysql.Execute(DropTableSQL(Of T))
            Call mysql.Execute(SQL)
        Else
            Return "No ``CREATE TABLE`` SQL meta data!"
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' ``DROP TABLE IF EXISTS `{<see cref="Table.GetTableName"/>(GetType(<typeparamref name="T"/>))}`;``
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Function DropTableSQL(Of T As SQLTable)() As String
        Return $"DROP TABLE IF EXISTS `{Table.GetTableName(GetType(T))}`;"
    End Function

    <Extension> Public Function AsDBI(Of Table As SQLTable)(uri As String) As Linq(Of Table)
        Dim DBI As ConnectionUri = ConnectionUri.TryParsing(uri)
        Dim Linq As New Linq(Of Table)(DBI)
        Return Linq
    End Function

    ''' <summary>
    ''' IP地址或者localhost
    ''' </summary>
    Public Const SERVERSITE As String = ".+[:]\d+"

    ''' <summary>
    ''' Get the specific type of custom attribute from a property.
    ''' (从一个属性对象中获取特定的自定义属性对象)
    ''' </summary>
    ''' <typeparam name="T">The type of the custom attribute.(自定义属性的类型)</typeparam>
    ''' <param name="Property">Target property object.(目标属性对象)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetAttribute(Of T As Attribute)([Property] As PropertyInfo) As T
        Dim Attributes As Object() = [Property].GetCustomAttributes(GetType(T), True)

        If Not Attributes Is Nothing AndAlso Attributes.Length = 1 Then
            Dim CustomAttr As T = CType(Attributes(0), T)

            If Not CustomAttr Is Nothing Then
                Return CustomAttr
            End If
        End If
        Return Nothing
    End Function

    Public ReadOnly Property MySqlDbTypes As IReadOnlyDictionary(Of Type, MySqlDbType) =
        New Dictionary(Of Type, MySqlDbType) From {
 _
            {GetType(String), MySqlDbType.Text},
            {GetType(Integer), MySqlDbType.MediumInt},
            {GetType(Long), MySqlDbType.BigInt},
            {GetType(Double), MySqlDbType.Double},
            {GetType(Decimal), MySqlDbType.Decimal},
            {GetType(Date), MySqlDbType.Date},
            {GetType(Byte), MySqlDbType.Byte},
            {GetType([Enum]), MySqlDbType.Enum}
    }

    <Extension>
    Public Function OrdinalSchema(reader As DbDataReader) As Dictionary(Of String, Integer)
        Dim schema As Dictionary(Of String, Integer) = reader.FieldCount _
            .SeqIterator _
            .ToDictionary(Function(i) reader.GetName(i),
                          Function(x) x)
        Return schema
    End Function

    ''' <summary>
    ''' Get the data type of a field in the data table.
    ''' (获取数据表之中的某一个域的数据类型)
    ''' </summary>
    ''' <param name="Type"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetDbDataType(Type As Type) As MySqlDbType
        If MySqlDbTypes.ContainsKey(Type) Then
            Return MySqlDbTypes(Type)
        Else
            Return MySqlDbType.Text
        End If
    End Function

    Public ReadOnly Property Numerics As List(Of MySqlDbType) = New List(Of MySqlDbType) From {
 _
        MySqlDbType.BigInt, MySqlDbType.Bit, MySqlDbType.Byte,
        MySqlDbType.Decimal, MySqlDbType.Double,
        MySqlDbType.Enum,
        MySqlDbType.Float,
        MySqlDbType.Int16, MySqlDbType.Int24, MySqlDbType.Int32, MySqlDbType.Int64,
        MySqlDbType.MediumInt,
        MySqlDbType.TinyInt,
        MySqlDbType.UByte, MySqlDbType.UInt16, MySqlDbType.UInt24, MySqlDbType.UInt32, MySqlDbType.UInt64,
        MySqlDbType.Year
    }

    ReadOnly __esacps As New Dictionary(Of String, String) From {
        {Text.ASCII.NUL, "\0"},
        {"'", "\'"},
        {"""", "\"""},
        {Text.ASCII.BS, "\b"},
        {Text.ASCII.LF, "\n"},
        {Text.ASCII.CR, "\r"},
        {Text.ASCII.TAB, "\t"},
        {Text.ASCII.SUB, "\Z"},
        {"%", "\%"}', {"_", "\_"}
    }

    ' {"\", "\\"}

    ''' <summary>
    ''' 处理字符串之中的特殊字符的转义
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <Extension>
    Public Function MySqlEscaping(value As String) As String
        Dim sb As New StringBuilder(value)

        Call sb.Replace("\", "\\")

        For Each x In __esacps
            Call sb.Replace(x.Key, x.Value)
        Next

        Return sb.ToString
    End Function

    ''' <summary>
    ''' 生成用于将数据集合批量导入数据库的INSERT SQL事务
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="type">
    ''' Only allowed action ``insert/update/delete/replace``, if the user custom SQL generator 
    ''' <paramref name="custom"/> is nothing, then this parameter works.
    ''' </param>
    ''' <param name="custom">
    ''' User custom SQL generator. If this parameter is not nothing, then <paramref name="type"/> will disabled.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function DumpTransaction(Of T As SQLTable)(source As IEnumerable(Of T),
                                                      Optional custom As Func(Of T, String) = Nothing,
                                                      Optional type$ = "insert") As String
        Dim SQL As Func(Of T, String)

        If custom Is Nothing Then
            Select Case LCase(type)
                Case "insert" : SQL = Function(o) o.GetInsertSQL
                Case "update" : SQL = Function(o) o.GetUpdateSQL
                Case "delete" : SQL = Function(o) o.GetDeleteSQL
                Case "replace" : SQL = Function(o) o.GetReplaceSQL
                Case Else
                    Throw New ArgumentException("Only allowes ""insert/update/delete/replace"" actions.", paramName:=NameOf(type))
            End Select
        Else
            SQL = custom
        End If

        Return source.Select(SQL).JoinBy(ASCII.LF)
    End Function

    ''' <summary>
    ''' 从<see cref="SQLTable"/>之中生成SQL语句之后保存到指定的文件句柄之上，
    ''' + 假若所输入的文件句柄是带有``.sql``后缀的话，会直接保存为该文件，
    ''' + 反之会被当作为文件夹，当前的集合对象会保存为与类型相同名称的sql文件
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="path$">
    ''' 请注意，在这里假若字符串是含有sql作为文件名后缀的话，会直接用作为文件路径来保存
    ''' 假若不是以sql为后缀的话，会被当做文件夹来处理
    ''' </param>
    ''' <param name="encoding"></param>
    ''' <returns></returns>
    <Extension>
    Public Function DumpTransaction(Of T As SQLTable)(source As IEnumerable(Of T),
                                                      path$,
                                                      Optional encoding As Encodings = Encodings.Default,
                                                      Optional type$ = "insert") As Boolean
        Dim sql$ = source.DumpTransaction(type:=type)

        If Not path.ExtensionSuffix.TextEquals("sql") Then
            Dim name$ = GetType(T).Name
            path = path & "/" & name & ".sql"
        End If

        Return sql.SaveTo(path, encoding.CodePage)
    End Function

    <Extension>
    Public Function CopySets(Of row As SQLTable, T)(o As row, list As IEnumerable(Of T), setValue As Action(Of row, T)) As row()
        Dim array As T() = list.SafeQuery.ToArray

        If array.Length = 0 Then
            Return {o}
        Else
            Dim out As New List(Of row)

            For Each x As T In list
                Dim copy As row = o.Copy.As(Of row)
                Call setValue(copy, x)
                Call out.Add(copy)
            Next

            Return out
        End If
    End Function
End Module

