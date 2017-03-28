#Region "Microsoft.VisualBasic::b38be66c5fa64dc6c9ed5e80663028ab, ..\visualbasic.DBI\SQLite_Interface\QueryBuilder.vb"

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

Imports System.Runtime.CompilerServices

''' <summary>
''' SQL query statement builder for the SELECT query.
''' </summary>
''' <remarks></remarks>
Public Module QueryBuilder

    ''' <summary>
    ''' 转码SQlite的分割字符 ( ' --> '' )
    ''' </summary>
    ''' <param name="fieldValue"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Escaping(fieldValue As String) As String
        If fieldValue Is Nothing Then
            Return ""
        Else
            Return fieldValue.Replace("'", "''")
        End If
    End Function

    Public Enum StringCompareMethods
        ''' <summary>
        ''' 模糊匹配，大小写敏感的
        ''' </summary>
        ''' <remarks></remarks>
        [Like]
        ''' <summary>
        ''' 相等，大小写敏感的
        ''' </summary>
        ''' <remarks></remarks>
        Equals
        ''' <summary>
        ''' 模糊匹配，大小写不敏感的
        ''' </summary>
        ''' <remarks></remarks>
        LikeWithCaseInsensitive
        ''' <summary>
        ''' 字符串相等，大小写不敏感的
        ''' </summary>
        ''' <remarks></remarks>
        EqualsWithCaseInsensitive
    End Enum

    Public Const SELECT_TEMPLATE As String = "SELECT * FROM '{0}' "
    Public Const WHERE_TEMPLATE As String = "WHERE {0} {1} {2} ;"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="FieldName">数据库的表之中的列名称</param>
    ''' <param name="Keyword">所需要进行查询的关键词</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Delegate Function FieldQueryBuilder(FieldName As String, Keyword As String) As String

    Private Function [Like](Field As String, Value As String) As String
        Dim s As String = String.Format(WHERE_TEMPLATE, Field, "LIKE", "'" & Value & "'")
        Return s
    End Function

    Private Function Equals(Field As String, value As String) As String
        Dim s As String = String.Format(WHERE_TEMPLATE, Field, "=", "'" & value & "'")
        Return s
    End Function

    Private Function LikeWithCaseInsensitive(Field As String, value As String) As String
        Dim s As String = String.Format(WHERE_TEMPLATE, "lower( " & Field & " )", "LIKE", String.Format("'%'||lower( '{0}' )||'%'", value))
        Return s
    End Function

    Private Function EqualsWithCaseInsensitive(Field As String, Value As String) As String
        Dim s As String = String.Format(WHERE_TEMPLATE, "lower( " & Field & " )", "=", String.Format("lower( '{0}' )", Value))
        Return s
    End Function

    Private ReadOnly StringCompareMethodsDictionary As Dictionary(Of StringCompareMethods, FieldQueryBuilder) =
        New Dictionary(Of StringCompareMethods, FieldQueryBuilder) From {
 _
            {QueryBuilder.StringCompareMethods.Like, AddressOf QueryBuilder.[Like]},
            {QueryBuilder.StringCompareMethods.Equals, AddressOf QueryBuilder.Equals},
            {QueryBuilder.StringCompareMethods.LikeWithCaseInsensitive, AddressOf QueryBuilder.LikeWithCaseInsensitive},
            {QueryBuilder.StringCompareMethods.EqualsWithCaseInsensitive, AddressOf QueryBuilder.EqualsWithCaseInsensitive}}

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Field"></param>
    ''' <param name="value"></param>
    ''' <param name="ComparedMethod">默认是使用最宽松的匹配条件</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function QueryByStringCompares(Of T As Class)(Field As String, value As String, Optional ComparedMethod As StringCompareMethods = StringCompareMethods.LikeWithCaseInsensitive) As String
        Dim SQL As String = QueryByStringCompares(Field, value, GetTableName(Of T))
        Return SQL
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Field"></param>
    ''' <param name="value"></param>
    ''' <param name="ComparedMethod">默认是使用最宽松的匹配条件</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function QueryByStringCompares(Field As String, value As String, TableName As String, Optional ComparedMethod As StringCompareMethods = StringCompareMethods.LikeWithCaseInsensitive) As String
        Dim BuilderMethod As QueryBuilder.FieldQueryBuilder = QueryBuilder.StringCompareMethodsDictionary(ComparedMethod)
        Dim SQL As String = String.Format(SELECT_TEMPLATE, TableName) & BuilderMethod(Field, Keyword:=value)
        Return SQL
    End Function

    ''' <summary>
    ''' 将SQL语句之中的特殊字符进行移除
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function TrimmingSQLConserved(str As String) As String
        Return str.Replace("'", "_").Replace(",", "_")
    End Function
End Module
