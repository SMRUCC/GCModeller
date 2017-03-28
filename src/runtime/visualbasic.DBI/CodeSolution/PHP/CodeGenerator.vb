#Region "Microsoft.VisualBasic::5272c760ac6f97c6872eca6006de3fd9, ..\visualbasic.DBI\CodeSolution\PHP\CodeGenerator.vb"

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
