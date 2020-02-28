
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("multi_omics")>
Module multiOmics

    <ExportAPI("omics.2D_scatter")>
    Public Function omics2DScatterPlot(x As Object, y As Object,
                                       Optional xlab$ = "X",
                                       Optional ylab$ = "Y",
                                       Optional ptSize! = 10,
                                       Optional env As Environment = Nothing) As Object

        If x Is Nothing OrElse y Is Nothing Then
            Return REnv.Internal.debug.stop("data can not be null!", env)
        End If

        Return OmicsScatter2D.Plot(
            omicsX:=getData(x, xlab),
            omicsY:=getData(y, ylab),
            xlab:=xlab,
            ylab:=ylab,
            pointSize:=ptSize
        )
    End Function

    Private Function getData(x As Object, ByRef label$) As NamedValue(Of Double)()
        Dim type As Type = x.GetType

        If type Is GetType(Dictionary(Of String, Double)) Then
            Return DirectCast(x, Dictionary(Of String, Double)) _
                .Select(Function(gene)
                            Return New NamedValue(Of Double)(gene.Key, gene.Value)
                        End Function) _
                .ToArray
        ElseIf type Is GetType(list) Then
            Return DirectCast(x, list).slots _
                .Select(Function(gene)
                            Return New NamedValue(Of Double)(gene.Key, CDbl(REnv.getFirst(gene.Value)))
                        End Function) _
                .ToArray
        Else
            Return {}
        End If
    End Function
End Module
