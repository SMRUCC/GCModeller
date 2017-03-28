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