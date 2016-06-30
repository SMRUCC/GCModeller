Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic

Public Module NetAPI

    ''' <summary>
    ''' 这个函数只分析出调控因子之间的调控网络
    ''' </summary>
    ''' <param name="virtualFootprints"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("TF_NET.Build")>
    <Extension>
    Public Function BuildNetwork(virtualFootprints As IEnumerable(Of PredictedRegulationFootprint), cut As Double) As FileStream.Network
        Dim allTFs As String() = (From x In virtualFootprints
                                  Where Not String.IsNullOrEmpty(x.Regulator)
                                  Select x.Regulator
                                  Distinct).ToArray
        Dim LQuery = (From x In virtualFootprints
                      Where Not String.IsNullOrEmpty(x.Regulator) AndAlso
                          Array.IndexOf(allTFs, x.ORF) > -1
                      Select x).ToArray
        Dim MyRegs = (From x As PredictedRegulationFootprint
                      In LQuery
                      Select x
                      Group x By x.Regulator Into Group) _
                           .ToDictionary(Function(x) x.Regulator,
                                         Function(x) x.Group.ToArray(Function(xx) xx.ORF).Distinct.Count)
        Dim RegsMe = (From x As PredictedRegulationFootprint
                      In LQuery
                      Select x
                      Group x By x.ORF Into Group) _
                           .ToDictionary(Function(x) x.ORF,
                                         Function(x) x.Group.ToArray(Function(xx) xx.Regulator).Distinct.Count)
        Dim TF = allTFs.ToArray(
            Function(x) New FileStream.Node(x) With {
                .NodeType = "TF",
                .Properties = New Dictionary(Of String, String) From {
 _
                    {"In", RegsMe.TryGetValue(x).ToString},
                    {"Out", MyRegs.TryGetValue(x).ToString}
                }
            })
        Dim regulates As NetworkEdge() = (From x As PredictedRegulationFootprint
                                          In LQuery.AsParallel
                                          Let edge As NetworkEdge = x.__netEdge
                                          Where edge.Confidence >= cut
                                          Select edge).ToArray
        Return New FileStream.Network(TF, regulates)
    End Function

    <Extension>
    Private Function __netEdge(x As PredictedRegulationFootprint) As NetworkEdge
        Dim prop As New Dictionary(Of String, String) From {
 _
            {"PCC", x.Pcc},
            {"sPCC", x.sPcc},
            {"RegType", If(x.Pcc > 0, "Pos", "Neg")}
        }
        Return New NetworkEdge With {
            .FromNode = x.Regulator,
            .ToNode = x.ORF,
            .Confidence = Math.Abs(0.5 * (x.Pcc + x.sPcc)),
            .InteractionType = "Regulation",
            .Properties = prop
        }
    End Function
End Module