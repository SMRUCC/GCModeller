#Region "Microsoft.VisualBasic::747fdb703fdc23b7030f90dcb86420cd, ..\visualbasic.DBI\LibMySQL\Reflection\SQL_LDM\SqlGenerateMethods.vb"

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
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Namespace Reflection.SQL

    ''' <summary>
    ''' 请注意，Where语句之中的变量总是<see cref="Schema.Table.Index"></see>属性值中的值
    ''' </summary>
    ''' <remarks></remarks>
    Public Module SqlGenerateMethods

        Const DELETE_SQL As String = "DELETE FROM `{0}` WHERE {1};"

        <Extension>
        Public Function GenerateDeleteSql(Schema As Table) As String
            Return String.Format(DELETE_SQL, Schema.TableName, __getWHERE(Schema.PrimaryFields))
        End Function

        Private Function __getWHERE(index As IEnumerable(Of String), Optional offset As Integer = 0) As String
            If index.Count = 1 Then
                Return $"`{index.First}` = '%s'".Replace("%s", "{%d}").Replace("%d", offset)
            End If

            Dim array As String() = index.ToArray(Function(name, idx) $"`{name}`='{"{"}{idx + offset}{"}"}'")
            Return String.Join(" and ", array)
        End Function

        <Extension>
        Public Function GenerateUpdateSql(Schema As Table) As String
            Dim sb As New StringBuilder(512)
            Dim Fields = Schema.Fields.ToArray

            sb.AppendFormat("UPDATE `{0}` SET ", Schema.TableName)

            For i As Integer = 0 To Fields.Length - 1
                sb.AppendFormat("`{0}`='%s', ", Fields(i).FieldName)
                sb.Replace("%s", "{" & i & "}")
            Next
            sb.Remove(sb.Length - 2, 2)
            sb.Append($" WHERE {__getWHERE(Schema.PrimaryFields, offset:=Schema.Fields.Length)};")

            Return sb.ToString
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Schema"></param>
        ''' <param name="trimAutoIncrement">假若有列是被标记为自动增长的，则不需要在INSERT_SQL之中在添加他的值了</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GenerateInsertSql(Schema As Table, Optional trimAutoIncrement As Boolean = False) As String
            Dim sb As New StringBuilder(512)
            Dim fields$() = LinqAPI.Exec(Of String) <=
 _
                From field As Field
                In Schema.__fields(trimAutoIncrement)  ' 因为需要与后面的值一一对应，所以在这里不进行排序不适用并行化
                Select "`" & field.FieldName & "`"

            Call sb.AppendFormat("INSERT INTO `{0}` (", Schema.TableName)   ' Create table name header
            Call sb.Append(String.Join(", ", fields))                       ' Fields generate
            Call sb.Append(") VALUES ")                                     ' Values formater generate
            Call sb.Append(GenerateInsertValues(Schema, trimAutoIncrement) & ";")

            Return sb.ToString
        End Function

        <Extension>
        Private Function __fields(schema As Table, trimAutoIncrement As Boolean) As Field()
            Dim fields As Field() = schema.Fields

            If trimAutoIncrement Then
                fields = fields _
                    .Where(Function(f) Not f.AutoIncrement) _
                    .ToArray
            End If

            Return fields
        End Function

        Public Function GenerateInsertValues(Schema As Table, Optional trimAutoIncrement As Boolean = False) As String
            Dim fields As Field() = Schema.__fields(trimAutoIncrement)
            Dim values$() = LinqAPI.Exec(Of String) <=
                From i As Integer
                In Schema.Fields.Sequence
                Select "'{0}'".Replace("0"c, i)

            Return "(" & String.Join(", ", values) & ")" ' End of the statement
        End Function
    End Module
End Namespace
