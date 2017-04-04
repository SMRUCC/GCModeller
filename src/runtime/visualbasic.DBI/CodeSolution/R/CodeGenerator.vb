#Region "Microsoft.VisualBasic::ef3bcb8f82164fd4581a7fc7043ba8f3, ..\visualbasic.DBI\CodeSolution\R\CodeGenerator.vb"

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

'Imports System.Runtime.CompilerServices
'Imports System.Text
'Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
'Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

'Namespace R

'    ''' <summary>
'    ''' Code generator for R language.
'    ''' (策略是先从数据库之中读取数据，然后对每一列生成列表，
'    ''' 最后对这些列表使用data.frame就可以生成计算所需要的dataframe对象了)
'    ''' </summary>
'    Public Module CodeGenerator

'        ' field <- vector();
'        ' field <- append(field, value);
'        ' table <- data.frame(fieldName = field);

'        ''' <summary>
'        ''' 生成Class代码
'        ''' </summary>
'        ''' <param name="SQL"></param>
'        ''' <param name="[Namesapce]"></param>
'        ''' <returns></returns>
'        Public Function GenerateClass(SQL As String, [Namesapce] As String) As NamedValue(Of String)
'            Dim Table As Reflection.Schema.Table = SQLParser.ParseTable(SQL)
'            Dim R As String = CodeGenerator.GenerateCode(Table, Namesapce)

'            Return New NamedValue(Of String) With {
'                .Name = Table.TableName,
'                .Value = R
'            }
'        End Function

'        <Extension>
'        Private Function GenerateCode(table As Table, namesapce As String) As String
'            Dim Rcode As New StringBuilder

'            Call Rcode.AppendLine("# For Microsoft R Open language")
'            Call Rcode.AppendLine()
'            Call Rcode.AppendLine("library(DBI)")
'            Call Rcode.AppendLine("library(RMySQL)")
'            Call Rcode.AppendLine()
'            Call Rcode.AppendLine()
'            Call Rcode.AppendLine($"load.{table.TableName} <- function(SQL, cnn) " & "{")
'            Call Rcode.AppendLine()

'            Call Rcode.AppendLine("# Initialize database table field collection objects.")
'            For Each field As String In table.FieldNames
'                Call Rcode.AppendLine($"{field} <- vector();")
'            Next

'            Call Rcode.AppendLine("# Start MySQL query")
'            Call Rcode.AppendLine("res <- dbSendQuery(cnn, SQL);")

'            Call Rcode.AppendLine("}")

'            Return Rcode.ToString
'        End Function
'    End Module
'End Namespace
