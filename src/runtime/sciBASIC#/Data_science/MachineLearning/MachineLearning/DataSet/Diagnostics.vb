Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Linq

Namespace StoreProcedure

    Public Module Diagnostics

        <Extension>
        Public Iterator Function CheckDataSet(data As DataSet) As IEnumerable(Of LogEntry)
            Dim nSamples = data.DataSamples.size
            Dim size As Size = data.Size

            ' check for sample datas
            For Each sample As Sample In data.DataSamples.AsEnumerable
                If sample.status.Length <> size.Width Then
                    Yield New LogEntry With {
                        .message = $"{sample.ID} sample vector size is not equals to {size.Width}!",
                        .[object] = sample.ID,
                        .time = Now,
                        .level = MSG_TYPES.ERR
                    }
                End If
            Next
        End Function
    End Module
End Namespace