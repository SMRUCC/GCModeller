#Region "Microsoft.VisualBasic::73dc2f6bf7341d41bc0857e99eba7f8c, ..\LibMySQL\Reflection\SQL_LDM\SqlGenerateMethods.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Namespace Reflection.SQL

    ''' <summary>
    ''' 请注意，Where语句之中的变量总是<see cref="Schema.Table.Index"></see>属性值中的值
    ''' </summary>
    ''' <remarks></remarks>
    Public Module SqlGenerateMethods

        Const DELETE_SQL As String = "DELETE FROM `{0}` WHERE {1};"

        Public Function GenerateDeleteSql(Schema As Schema.Table) As String
            Return String.Format(DELETE_SQL, Schema.TableName, __getWHERE(Schema.PrimaryFields))
        End Function

        Private Function __getWHERE(index As Generic.IEnumerable(Of String), Optional offset As Integer = 0) As String
            If index.Count = 1 Then
                Return $"`{index.First}` = '%s'".Replace("%s", "{%d}").Replace("%d", offset)
            End If

            Dim array As String() = index.ToArray(Function(name, idx) $"`{name}`='{"{"}{idx + offset}{"}"}'")
            Return String.Join(" and ", array)
        End Function

        Public Function GenerateUpdateSql(Schema As Schema.Table) As String
            Dim sBuilder As StringBuilder = New StringBuilder(512)
            Dim Fields = Schema.Fields.ToArray

            sBuilder.AppendFormat("UPDATE `{0}` SET ", Schema.TableName)

            For i As Integer = 0 To Fields.Length - 1
                sBuilder.AppendFormat("`{0}`='%s', ", Fields(i).FieldName)
                sBuilder.Replace("%s", "{" & i & "}")
            Next
            sBuilder.Remove(sBuilder.Length - 2, 2)
            sBuilder.Append($" WHERE {__getWHERE(Schema.PrimaryFields, offset:=Schema.Fields.Length)};")

            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Schema"></param>
        ''' <param name="TrimAutoIncrement">假若有列是被标记为自动增长的，则不需要在INSERT_SQL之中在添加他的值了</param>
        ''' <returns></returns>
        Public Function GenerateInsertSql(Schema As Schema.Table, Optional TrimAutoIncrement As Boolean = False) As String
            Dim sBuilder As StringBuilder = New StringBuilder(512)

            If Not TrimAutoIncrement Then
                Call sBuilder.AppendFormat("INSERT INTO `{0}` (", Schema.TableName)  'Create table name header
                Call sBuilder.Append(String.Join(", ", (From Field As Reflection.Schema.Field In Schema.Fields Select "`" & Field.FieldName & "`").ToArray)) 'Fields generate
                Call sBuilder.Append(") VALUES (")  'Values formater generate
                Call sBuilder.Append(String.Join(", ", (From i As Integer In Schema.Fields.Sequence Select "'{0}'".Replace("0"c, i)).ToArray) & ");") 'End of the statement
            Else

                Call sBuilder.AppendFormat("INSERT INTO `{0}` (", Schema.TableName)  'Create table name header
                Call sBuilder.Append(String.Join(", ", (From Field As Reflection.Schema.Field In Schema.Fields Where Not Field.AutoIncrement Select "`" & Field.FieldName & "`").ToArray)) 'Fields generate
                Call sBuilder.Append(") VALUES (")  'Values formater generate
                Call sBuilder.Append(String.Join(", ", (From i As Integer In (From Field In Schema.Fields Where Not Field.AutoIncrement Select Field).ToArray.Sequence Select "'{0}'".Replace("0"c, i)).ToArray) & ");") 'End of the statement

            End If

            Return sBuilder.ToString
        End Function
    End Module
End Namespace

