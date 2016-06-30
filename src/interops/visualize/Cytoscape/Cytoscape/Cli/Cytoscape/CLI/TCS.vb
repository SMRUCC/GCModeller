Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.DocumentFormat
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Interactions.SwissTCS
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.Serialization
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.XGMML
Imports Microsoft.VisualBasic.Language

Partial Module CLI

    <ExportAPI("--TCS",
               Usage:="--TCS /in <TCS.csv.DIR> /regulations <TCS.virtualfootprints> /out <outForCytoscape.xml> [/Fill-pcc]")>
    <ParameterInfo("/Fill-pcc", True,
                   Description:="If the predicted regulation data did'nt contains pcc correlation value, then you can using this parameter to fill default value 0.6 or just left it default as ZERO")>
    Public Function TCS(args As CommandLine.CommandLine) As Integer
        Dim TCSDir As String = args("/in")
        Dim regulations As String = args("/regulations")
        Dim out As String = args("/out")
        Dim TCSProfiles = FileIO.FileSystem.GetFiles(TCSDir, FileIO.SearchOption.SearchAllSubDirectories, "*.csv") _
            .ToArray(Function(file) _
                         file.LoadCsv(Of CrossTalks)).MatrixToList
        Dim virtualFootprints = regulations.LoadCsv(Of PredictedRegulationFootprint)

        Dim HK As String() = (From name As String In TCSProfiles.ToArray(Function(cTk) cTk.Kinase) Select name Distinct Order By name Ascending).ToArray
        Dim RR As String() = (From name As String In TCSProfiles.ToArray(Function(cTK) cTK.Regulator) Select name Distinct Order By name).ToArray
        Dim Regulators As String() =
            LinqAPI.Exec(Of String) <= From name As String
                                       In virtualFootprints.Select(Function(regulate) regulate.Regulator)
                                       Where Not String.IsNullOrEmpty(name)
                                       Select name
                                       Distinct
                                       Order By name Ascending
        Dim Hybirds As String() = StringHelpers.Intersection(HK, RR)
        Dim Nodes As New List(Of Entity)
        Call Nodes.Add((From name As String In HK Where Array.IndexOf(Hybirds, name) = -1 Select New Entity With {.Identifier = name, .NodeType = "HK"}).ToArray)
        Call Nodes.Add((From name As String In RR Where Array.IndexOf(Hybirds, name) = -1 Select New Entity With {.Identifier = name, .NodeType = "RR"}).ToArray)
        Call Nodes.Add((From name As String In Hybirds Select New Entity With {.Identifier = name, .NodeType = "Hybrids"}).ToArray)
        Call Nodes.Add((From name As String In Regulators Where Array.IndexOf(HK, name) = -1 AndAlso Array.IndexOf(RR, name) = -1 Select New Entity With {.Identifier = name, .NodeType = "TF"}).ToArray)

        Dim Edges As New List(Of Interaction)
        Dim FillZERO As Boolean = args.GetBoolean("/Fill-pcc")
        Dim __getPCC As Func(Of String, Double) = Function(value As String) As Double
                                                      Dim n As Double = Val(value)
                                                      If n = 0R AndAlso FillZERO Then
                                                          Return 0.6
                                                      Else
                                                          Return n
                                                      End If
                                                  End Function

        Call Edges.Add(TCSProfiles.ToArray(Function(cTk) New Interaction With {
                                               .Confidence = cTk.Probability,
                                               .FromNode = cTk.Kinase,
                                               .ToNode = cTk.Regulator,
                                               .InteractionType = "CrossTalk",
                                               .Family = ""}).ToArray)
        Call Edges.Add((From regulation In virtualFootprints
                        Where Not (String.IsNullOrEmpty(regulation.Regulator) OrElse String.IsNullOrEmpty(regulation.ORF))
                        Select New Interaction With {
                                    .Confidence = __getPCC(regulation.Pcc),
                                    .FromNode = regulation.Regulator,
                                    .ToNode = regulation.ORF,
                                    .InteractionType = "Regulates",
                                    .Family = regulation.MotifId}).ToArray)
        Dim doc As Graph = ExportToFile.Export(Nodes.ToArray, Edges.ToArray, "TCS Crosstalks and Regulations")
        Return doc.Save(out)
    End Function

    Public Class Entity : Inherits FileStream.Node

    End Class

    Public Class Interaction : Inherits FileStream.NetworkEdge
        Public Property Family As String
    End Class

End Module
