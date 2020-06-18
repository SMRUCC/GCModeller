Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.PFSNet
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("PFSNet", Category:=APICategories.ResearchTools)>
Module PFSNetAnalysis

    Sub New()
        Internal.Object.Converts.addHandler(GetType(PFSNetResultOut), AddressOf makeDataFrame)
        Internal.generic.add("plot", GetType(PFSNetResultOut), AddressOf plotPFSNet)
    End Sub

    Private Function plotPFSNet(result As PFSNetResultOut, args As list, env As Environment) As Object
        Return BubblePlot.Plot(result)
    End Function

    Private Function makeDataFrame(x As Object, args As list, env As Environment) As dataframe
        Dim result As PFSNetResultOut = DirectCast(x, PFSNetResultOut)
        Dim subnetwork As Array = result.phenotype1.JoinIterates(result.phenotype2).Select(Function(a) a.Id).ToArray
        Dim phenotype As Array = result.phenotype1.Select(Function(a) "A").JoinIterates(result.phenotype2.Select(Function(a) "B")).ToArray
        Dim statistics As Array = result.phenotype1.JoinIterates(result.phenotype2).Select(Function(a) a.statistics).ToArray
        Dim pvalue As Array = result.phenotype1.JoinIterates(result.phenotype2).Select(Function(a) a.pvalue).ToArray
        Dim nodes As Array = result.phenotype1.JoinIterates(result.phenotype2).Select(Function(a) a.nodes.Length).ToArray
        Dim edges As Array = result.phenotype1.JoinIterates(result.phenotype2).Select(Function(a) a.edges.Length).ToArray
        Dim weight1 As Array = result.phenotype1.JoinIterates(result.phenotype2).Select(Function(a) a.nodes.Average(Function(n) n.weight)).ToArray
        Dim weight2 As Array = result.phenotype1.JoinIterates(result.phenotype2).Select(Function(a) a.nodes.Average(Function(n) n.weight2)).ToArray
        Dim genes As Array = result.phenotype1.JoinIterates(result.phenotype2).Select(Function(a) a.nodes.Select(Function(n) n.name).JoinBy("; ")).ToArray

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {NameOf(subnetwork), subnetwork},
                {NameOf(phenotype), phenotype},
                {NameOf(statistics), statistics},
                {NameOf(pvalue), pvalue},
                {NameOf(nodes), nodes},
                {NameOf(edges), edges},
                {NameOf(weight1), weight1},
                {NameOf(weight2), weight2},
                {NameOf(genes), genes}
            }
        }
    End Function

    <ExportAPI("load.pathway_network")>
    Public Function loadPathwayNetwork(file As String) As GraphEdge()
        Return GraphEdge.LoadData(file)
    End Function

    ''' <summary>
    ''' Finding consistent disease subnetworks using PFSNet
    ''' </summary>
    ''' <param name="expr1o"></param>
    ''' <param name="expr2o"></param>
    ''' <param name="ggi"></param>
    ''' <param name="b"></param>
    ''' <param name="t1"></param>
    ''' <param name="t2"></param>
    ''' <param name="n"></param>
    ''' <returns></returns>
    <ExportAPI("pfsnet")>
    Public Function run_pfsnet(<RRawVectorArgument> expr1o As Object, <RRawVectorArgument> expr2o As Object, ggi As GraphEdge(),
                               Optional b# = 0.5,
                               Optional t1# = 0.95,
                               Optional t2# = 0.85,
                               Optional n% = 1000) As PFSNetResultOut

        If TypeOf expr1o Is Matrix Then
            expr1o = DirectCast(expr1o, Matrix).expression
        End If
        If TypeOf expr2o Is Matrix Then
            expr2o = DirectCast(expr2o, Matrix).expression
        End If

        Return PFSNetAlgorithm.pfsnet(expr1o, expr2o, ggi, b, t1, t2, n)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="format">xml/json</param>
    ''' <returns></returns>
    <ExportAPI("read.pfsnet_result")>
    <RApiReturn(GetType(PFSNetResultOut))>
    Public Function readPFSNetOutput(file As String, Optional format As FileFormats = FileFormats.xml, Optional env As Environment = Nothing) As Object
        If Not file.FileExists Then
            Return Internal.debug.stop("the given file is not exists on your file system!", env)
        ElseIf format <> FileFormats.json AndAlso format <> FileFormats.xml Then
            Return Internal.debug.stop("the file format flag value is not supported at this api...", env)
        End If

        If format = FileFormats.xml Then
            Return file.LoadXml(Of PFSNetResultOut)
        Else
            Return file.LoadJsonFile(Of PFSNetResultOut)
        End If
    End Function
End Module
