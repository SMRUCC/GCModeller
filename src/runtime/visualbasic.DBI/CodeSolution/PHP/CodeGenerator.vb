Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Namespace PHP

    Public Module CodeGenerator

        ''' <summary>
        ''' 生成Class代码
        ''' </summary>
        ''' <param name="SQL"></param>
        ''' <param name="[Namesapce]"></param>
        ''' <returns></returns>
        Public Function GenerateClass(SQL As String, [Namesapce] As String) As NamedValue(Of String)
            Dim Table As Table = SQLParser.ParseTable(SQL)
            Dim php As String = CodeGenerator.GenerateCode(Table, Namesapce)

            Return New NamedValue(Of String) With {
                .Name = Table.TableName,
                .Value = php
            }
        End Function

        <Extension>
        Private Function GenerateCode(table As Table, namesapce As String) As String
            Dim php As New StringBuilder

            Call php.AppendLine("class " & table.TableName & " extends SQLTable {")

            For Each field In table.Fields
                Call php.AppendLine($"public ${field.FieldName};")
            Next

            Call php.AppendLine("public function __toString() {")
            Call php.AppendLine("}")

            Call php.AppendLine("}")

            Return php.ToString
        End Function
    End Module
End Namespace