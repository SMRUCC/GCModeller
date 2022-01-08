Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math

Public Module Math

    <Extension>
    Public Function rowSds(expr As Matrix) As Dictionary(Of String, Double)
        Return expr.expression _
            .ToDictionary(Function(g) g.geneID,
                          Function(g)
                              Return g.experiments.SD
                          End Function)
    End Function
End Module
