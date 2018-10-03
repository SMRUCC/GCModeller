Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO

Public Module Extensions

    <Extension>
    Public Iterator Function GetMatrix(raw As Reader, module$) As IEnumerable(Of DataSet)
        For Each time As Double In raw.AllTimePoints
            Yield New DataSet With {
                .ID = time,
                .Properties = raw.Read(time, [module])
            }
        Next
    End Function
End Module
