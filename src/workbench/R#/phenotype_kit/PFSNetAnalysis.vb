#Region "Microsoft.VisualBasic::4334c5c607b682f6063cc25d790ea149, R#\phenotype_kit\PFSNetAnalysis.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 174
    '    Code Lines: 135
    ' Comment Lines: 17
    '   Blank Lines: 22
    '     File Size: 8.26 KB


    ' Module PFSNetAnalysis
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: buildPathwayNetwork, loadPathwayNetwork, makeDataFrame, plotPFSNet, readPFSNetOutput
    '               run_pfsnet, savePathwayNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.PFSNet
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

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

    <ExportAPI("build.pathway_network")>
    Public Function buildPathwayNetwork(maps As Map(), <RRawVectorArgument> reactions As Object, Optional env As Environment = Nothing) As pipeline
        Dim reactionTable As ReactionTable()

        If reactions Is Nothing Then
            Return Internal.debug.stop({"the required KEGG reaction network data can not be nothing!"}, env)
        ElseIf TypeOf reactions Is ReactionTable() Then
            reactionTable = DirectCast(reactions, ReactionTable())
        ElseIf TypeOf reactions Is pipeline AndAlso DirectCast(reactions, pipeline).elementType Like GetType(ReactionTable) Then
            reactionTable = DirectCast(reactions, pipeline).populates(Of ReactionTable)(env).ToArray
        ElseIf TypeOf reactions Is vector AndAlso DirectCast(reactions, vector).elementType Like GetType(ReactionTable) Then
            reactionTable = DirectCast(reactions, vector).data.AsObjectEnumerator(Of ReactionTable).ToArray
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(ReactionTable), reactions.GetType, env), env)
        End If

        Return maps _
            .ReferenceCompoundNetwork(reactionTable) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("save.pathway_network")>
    <RApiReturn(GetType(Boolean))>
    Public Function savePathwayNetwork(<RRawVectorArgument> ggi As Object, file As Object, Optional env As Environment = Nothing) As Object
        Dim stream As Stream
        Dim network As GraphEdge()

        If file Is Nothing Then
            Return Internal.debug.stop({"file output can not be nothing!"}, env)
        ElseIf TypeOf file Is String Then
            stream = DirectCast(file, String).Open(, doClear:=True)
        ElseIf TypeOf file Is Stream Then
            stream = DirectCast(file, Stream)
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(Stream), file.GetType, env,, NameOf(file)), env)
        End If

        If ggi Is Nothing Then
            Return Internal.debug.stop("the required data source can not be nothing!", env)
        ElseIf TypeOf ggi Is GraphEdge() Then
            network = DirectCast(ggi, GraphEdge())
        ElseIf TypeOf ggi Is pipeline Then
            network = DirectCast(ggi, pipeline).populates(Of GraphEdge)(env).ToArray
        ElseIf TypeOf ggi Is vector Then
            network = DirectCast(ggi, vector).data.AsObjectEnumerator(Of GraphEdge).ToArray
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(GraphEdge), ggi.GetType, env), env)
        End If

        Call GraphEdgeConnector.SaveTabular(network, stream)

        Try
            Call stream.Flush()

            If TypeOf file Is String Then
                Call stream.Close()
            End If
        Catch ex As Exception

        End Try

        Return True
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
