#Region "Microsoft.VisualBasic::07e93f31e42f3202e64cc7b565b1072e, ..\visualbasic.DBI\LibMySQL\Reflection\Schema\Table.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Reflection
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Reflection.Schema

    ''' <summary>
    ''' The table schema that we define on the custom attributes of a Class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Table

        Protected _databaseFields As Dictionary(Of String, Field)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TableName As String
        Public Property UniqueFields As New List(Of String)
        ''' <summary>
        ''' 主键，主要根据这个属性来生成WHERE条件
        ''' </summary>
        ''' <returns></returns>
        Public Property PrimaryFields As New List(Of String)

        ''' <summary>
        ''' 获取这个数据表的列定义集合
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Fields As Field()
            Get
                Return _databaseFields.Values.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 获取这个数据表的列名称的集合
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FieldNames As String()
            Get
                Return _databaseFields.ToArray(Function(x) x.Value.FieldName)
            End Get
        End Property

        ''' <summary>
        ''' 这张数据表所在数据库的名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Database As String

        ''' <summary>
        ''' 查找不到则返回空值
        ''' </summary>
        ''' <param name="FieldName"></param>
        ''' <returns></returns>
        Public ReadOnly Property DatabaseField(FieldName As String) As Field
            Get
                Return __getField(FieldName)
            End Get
        End Property

        Private Function __getField(name As String) As Field
            If _databaseFields.ContainsKey(name) Then
                Return _databaseFields(name)
            Else
                Call $"{TableName} => {name} is not exists....".__DEBUG_ECHO
                Return Nothing
            End If
        End Function

        Public Property Comment As String

        ''' <summary>
        ''' The index field when execute the update/delete sql.
        ''' </summary>
        ''' <remarks>Long/Integer first, then the Text is second, the primary key is the last consideration.</remarks>
        Public Property Index As String
        Public Property IndexProperty As PropertyInfo
        Public Property SchemaType As Type
        ''' <summary>
        ''' Create Table SQL, optional...
        ''' </summary>
        ''' <returns></returns>
        Public Property SQL As String

        Protected Friend Sub New()
            _databaseFields = New Dictionary(Of String, Field)
        End Sub

        ''' <summary>
        ''' 从数据类型之中直接创建``schema``对象
        ''' </summary>
        ''' <param name="schema"></param>
        Sub New(schema As Type)
            Call Me.New
            Call Me.__getSchema(schema)
            SchemaType = schema
        End Sub

        ''' <summary>
        ''' 这里不进行反射解析，直接使用已经存在的数据进行数据表模型的构造
        ''' </summary>
        ''' <param name="databaseFields"></param>
        Sub New(databaseFields As Dictionary(Of String, Field))
            _databaseFields = databaseFields
        End Sub

        Public Function GetPrimaryKeyFields() As Field()
            Return PrimaryFields.ToArray(AddressOf __getField)
        End Function

        Public Overrides Function ToString() As String
            Return TableName
        End Function

        Private Sub __getSchema(schema As Type)
            Dim ItemProperty As PropertyInfo() = schema.GetProperties
            Dim Field As Field
            Dim Index2 As String = String.Empty
            Dim IndexProperty2 As PropertyInfo = Nothing

            TableName = GetTableName(schema)
            Database = GetDatabaseName(schema)

            For i As Integer = 0 To ItemProperty.Length - 1

                ' Parse the field attribute from the ctype operator, 
                ' this property must have a DatabaseField custom 
                ' Attribute to indicate that it is a database field.
                Field = ItemProperty(i)

                If Field Is Nothing Then
                    Continue For
                End If

                Call _databaseFields.Add(Field.FieldName, Field)

                If Field.PrimaryKey Then
                    PrimaryFields.Add(Field.FieldName)
                End If

                If Field.Unique Then
                    UniqueFields.Add(Field.FieldName)

                    If Extensions.Numerics.IndexOf(Field.DataType) > -1 AndAlso Field.PrimaryKey Then
                        Index = Field.FieldName
                        IndexProperty = ItemProperty(i)
                    End If

                    Index2 = Field.FieldName
                    IndexProperty2 = ItemProperty(i)
                End If
            Next

            ' If we can not found a index from its unique field, then we indexing from its primary key.
            Call __indexing(Index2, IndexProperty2, ItemProperty)
        End Sub

        ''' <summary>
        ''' Indexing from the primary key attributed field.
        ''' </summary>
        ''' <param name="Index2"></param>
        ''' <param name="Indexproperty2"></param>
        ''' <param name="ItemProperty"></param>
        ''' <remarks></remarks>
        Private Sub __indexing(Index2 As String, Indexproperty2 As PropertyInfo, ItemProperty As PropertyInfo())
            If Not String.IsNullOrEmpty(Index) Then Return ' If we have a unique index from previous work, then this operation is no more needed. 

            If String.IsNullOrEmpty(Index2) Then
                If PrimaryFields.Count > 0 Then            ' Indexing from the primary key, the primary key may be more than one
                    Dim LQuery As String =
                        LinqAPI.DefaultFirst(Of String) <=
                        From Field In Fields
                        Where (Field.PrimaryKey AndAlso
                            Extensions.Numerics.IndexOf(Field.DataType) > -1)
                        Select Field.FieldName

                    If Not String.IsNullOrEmpty(LQuery) Then
                        Index = LQuery
                    Else
                        Index = PrimaryFields.First
                    End If

                    IndexProperty =
                        LinqAPI.DefaultFirst(Of PropertyInfo) <=
                        From iPinfo As PropertyInfo
                        In ItemProperty
                        Let p As DatabaseField = GetAttribute(Of DatabaseField)(iPinfo)
                        Where Not p Is Nothing AndAlso
                            String.Equals(p.Name, Index)
                        Select iPinfo '
                End If
            Else
                Index = Index2   ' The data type of this index field is a text type. 
                IndexProperty = Indexproperty2
            End If
        End Sub

        ''' <summary>
        ''' 使用这个函数从类型的元数据之中解析出数据库表名
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Shared Function GetTableName(type As Type) As String
            Return type.GetAttribute(Of TableName).Name
        End Function

        Public Shared Function GetDatabaseName(type As Type) As String
            Return type.GetAttribute(Of TableName).Database
        End Function

        Public Shared Widening Operator CType(Schema As Type) As Table
            Return New Table(Schema)
        End Operator
    End Class
End Namespace
