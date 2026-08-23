Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.Framework.IO.CSVFile
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.Runtime

Public Module CorrelationNetwork

    Public Function LoadAdjacencyMatrix(file As String) As DataMatrix
        Dim rows As IEnumerable(Of RowObject) = file.IterateAllLines(tqdm_wrap:=True).Select(Function(a) Tokenizer.CharsParser(a))
        Dim data As IEnumerable(Of (String, Double())) = From row As RowObject In rows Select (row(0), row.Skip(1).AsDouble)
        Dim adj As New DataMatrix(data)
        Return adj
    End Function

    <Extension>
    Public Function ExportGraph(adj As DataMatrix, modules As IEnumerable(Of ModuleMembershipResult), Optional adj_thres As Double = 0.8) As NetworkGraph

    End Function
End Module
