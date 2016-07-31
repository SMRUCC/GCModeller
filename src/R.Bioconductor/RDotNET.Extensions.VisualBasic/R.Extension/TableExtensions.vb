Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic
Imports System.Text

Public Module TableExtensions

    ''' <summary>
    ''' Push the table data in the VisualBasic into R system.
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="tableName"></param>
    ''' <param name="skipFirst">
    ''' If the first column is the rows name, and you don't want these names, then you should set this as TRUE to skips this data.
    ''' </param>
    <Extension>
    Public Sub PushAsTable(table As DocumentStream.File, tableName As String, Optional skipFirst As Boolean = True)
        Dim MAT As New List(Of String)
        Dim ncol As Integer

        For Each row In table.Skip(1)
            If skipFirst Then
                MAT += row.Skip(1)
            Else
                MAT += row.ToArray
            End If

            If ncol = 0 Then
                ncol = MAT.Count
            End If
        Next

        Dim R As New StringBuilder()
        Dim colNames As String = c(table.First.Skip(If(skipFirst, 1, 0)).ToArray)

        R.AppendLine($"{tableName} <- matrix(c({MAT.JoinBy(",")}),ncol={ncol},byrow=TRUE);")
        R.AppendLine($"colnames({tableName}) <- {colNames}")

        Call RServer.Evaluate(R.ToString)
    End Sub
End Module
