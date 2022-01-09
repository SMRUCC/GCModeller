Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math

''' <summary>
''' math helper for HTS matrix
''' </summary>
Public Module Math

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function rowSds(expr As Matrix) As Dictionary(Of String, Double)
        Return expr.expression _
            .ToDictionary(Function(g) g.geneID,
                          Function(g)
                              Return g.experiments.SD
                          End Function)
    End Function

    <DebuggerStepThrough>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsNumeric(m As Matrix) As Double()()
        Return (From t As DataFrameRow
                In m.expression
                Select t.experiments).ToArray
    End Function
End Module
