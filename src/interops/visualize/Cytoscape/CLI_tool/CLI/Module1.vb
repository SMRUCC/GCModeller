#Region "Microsoft.VisualBasic::684b32f0ba1f82d063ad0f9daceb940d, visualize\Cytoscape\CLI_tool\CLI\Module1.vb"

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

    ' Module CLI
    ' 
    '     Function: Assciates, LinkageKnowledgeNetwork, PlotCytoscapeTable, replaceName
    '     Class __net
    ' 
    '         Properties: EntryName1, meta, SNP, source, target
    ' 
    '         Function: Copy
    ' 
    '     Class nodeName
    ' 
    '         Properties: Entered, GeneID, HumanSymbol, ID2, Name
    ' 
    '     Class __node
    ' 
    '         Properties: Entry, EntryName, GeneNames, Length, Organism
    '                     ProteinNames
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Cytoscape
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables

Partial Module CLI

    Public Class __net
        Public Property source As String
        Public Property target As String
        Public Property SNP As String
        Public Property EntryName1 As String
        Public Property meta As New Dictionary(Of String, String)

        Public Function Copy() As __net
            Return New __net With {.source = source, .target = target, .SNP = SNP, .EntryName1 = EntryName1, .meta = New Dictionary(Of String, String)(meta)}
        End Function
    End Class

    Public Class nodeName
        Public Property Entered As String
        <Column("Human Symbol")> Public Property HumanSymbol As String
        <Column("Gene ID")> Public Property GeneID As String
        Public Property ID2 As String
        Public Property Name As String

    End Class

    <ExportAPI("/replace", Usage:="/replace /in <net.csv> /nodes <nodes.Csv> /out <out.Csv>")>
    Public Function replaceName(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim names As String = args("/nodes")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & names.BaseName & ".Csv")
        Dim net = [in].LoadCsv(Of __net)
        Dim nodes As nodeName() = names.LoadCsv(Of nodeName)
        Dim nhash = (From x In nodes Select x Group x By x.Entered Into Group).ToDictionary(Function(x) x.Entered, Function(x) x.Group.ToArray)
        Dim reuslt As New List(Of __net)

        For Each x In net
            If nhash.ContainsKey(x.SNP) Then
                For Each hit In nhash(x.SNP)
                    Dim copy = x.Copy
                    copy.meta.Add(NameOf(hit.Entered), hit.Entered)
                    copy.meta.Add(NameOf(hit.GeneID), hit.GeneID)
                    copy.meta.Add(NameOf(hit.HumanSymbol), hit.HumanSymbol)
                    copy.meta.Add(NameOf(hit.ID2), hit.ID2)
                    copy.meta.Add(NameOf(hit.Name), hit.Name)

                    reuslt += copy
                Next
            Else
                reuslt += x
            End If
        Next

        Return reuslt.SaveTo(out).CLICode
    End Function

    Public Class __node
        Public Property Entry As String
        <Column("Entry name")> Public Property EntryName As String
        <Column("Protein names")> Public Property ProteinNames As String
        <Collection("Gene names", " ")> Public Property GeneNames As String()
        Public Property Organism As String
        Public Property Length As Integer

    End Class

    <ExportAPI("/associate", Usage:="/associate /in <net.csv> /nodes <nodes.csv> [/out <out.net.DIR>]")>
    Public Function Assciates(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim nodes As String = args("/nodes")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "/")
        Dim net = [in].LoadCsv(Of __net)
        Dim nodesContent = nodes.LoadCsv(Of __node)
        Dim nnnHash = (From o In (From x In nodesContent
                                  Where Not x.GeneNames.IsNullOrEmpty
                                  Select From name
                                             In x.GeneNames
                                         Select name, node = x).IteratesALL
                       Select o
                       Group o By o.name Into Group).ToDictionary(
                       Function(x) x.name,
                       Function(x) x.Group.Select(
                       Function(o) o.node).Distinct.ToArray)

        For Each edge In net
            If nnnHash.ContainsKey(edge.target) Then
                Dim array() = nnnHash(edge.target)
                Dim i As Integer = 1

                For Each x In array
                    edge.meta.Add(NameOf(x.Entry) & i, x.Entry)
                    edge.meta.Add(NameOf(x.EntryName) & i, x.EntryName)
                    edge.meta.Add(NameOf(x.GeneNames) & i, x.GeneNames.JoinBy("; "))
                    edge.meta.Add(NameOf(x.Length) & i, x.Length)
                    edge.meta.Add(NameOf(x.Organism) & i, x.Organism)
                    edge.meta.Add(NameOf(x.ProteinNames) & i, x.ProteinNames)

                    i += 1
                Next
            End If
        Next

        Return net.SaveTo(out & "/net.Csv").CLICode
    End Function

    <ExportAPI("/linkage.knowledge.network",
               Usage:="/linkage.knowledge.network /in <knowledge.network.csv/DIR> [/schema <material> /no-type_prefix /out <out.network.DIR>]")>
    Public Function LinkageKnowledgeNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim schema$ = args.GetValue("/schema", "material")
        Dim typePrefix As Boolean = Not args.GetBoolean("/no-type_prefix")
        Dim out As String = args.GetValue("/out", [in].ParentPath & "/" & [in].BaseName & ".knowledge_network/")
        Dim network As NetworkTables

        If [in].DirectoryExists Then
            Throw New NotImplementedException
        Else
            Dim data As File = File.Load([in])
            network = LinkageNetwork.BuildNetwork(data, typePrefix, schema)
        End If

        Return network.Save(out, Encoding.ASCII)
    End Function

    <ExportAPI("/Plot.Cytoscape.Table",
               Usage:="/Plot.Cytoscape.Table /in <table.csv> [/size <default=1600,1440> /out <out.DIR>]")>
    Public Function PlotCytoscapeTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".visualize/")
        Dim network = [in].LoadCsv(Of Edges).CytoscapeNetworkFromEdgeTable
        Dim size$ = args.GetValue("/size", "1600,1440")

        Call network.doRandomLayout
        Call network.doForceLayout
        Call network.ComputeNodeDegrees
        Call network _
            .DrawImage(canvasSize:=size, labelColorAsNodeColor:=True) _
            .Save(out & "/network.png")

        Return network.Tabular.Save(out & "/").CLICode
    End Function
End Module
