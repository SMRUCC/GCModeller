#Region "Microsoft.VisualBasic::5adb10494aabb5c803e19ff46ba112c2, R#\cytoscape_toolkit\bioModels\stringdbPPI.vb"

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

'   Total Lines: 100
'    Code Lines: 66 (66.00%)
' Comment Lines: 22 (22.00%)
'    - Xml Docs: 95.45%
' 
'   Blank Lines: 12 (12.00%)
'     File Size: 3.98 KB


' Module stringdbPPI
' 
'     Function: buildNetworkModel, intersect, parseStringDb, readLayout, readNetwork
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Data.[STRING].StringDB.Tsv
Imports SMRUCC.genomics.Model.Network.KEGG
Imports SMRUCC.genomics.Model.Network.KEGG.GraphVisualizer
Imports SMRUCC.genomics.Model.Network.STRING
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' protein-protein interaction network model from string-db
''' </summary>
<Package("bioModels.stringdb.ppi")>
Module stringdbPPI

    ''' <summary>
    ''' export string-db interaction result set as graph
    ''' </summary>
    ''' <param name="stringNetwork"></param>
    ''' <param name="uniprot">``STRING -> uniprot``</param>
    ''' <param name="coordinates"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.network")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function buildNetworkModel(stringNetwork As InteractExports(), <RRawVectorArgument> uniprot As Object,
                                      Optional coordinates As Coordinates() = Nothing,
                                      Optional env As Environment = Nothing) As Object

        Dim uniprotDb As pipeline = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If uniprotDb.isError Then
            Return uniprotDb.getError
        End If

        Dim annotations = uniprotDb.populates(Of entry)(env).StringUniprot
        Dim graph As NetworkGraph = stringNetwork.BuildModel(uniprot:=annotations, groupValues:=SimpleBuilder.KOGroupTable)

        If Not coordinates.IsNullOrEmpty Then
            Return graph.applyLayout(coordinates)
        Else
            Return graph
        End If
    End Function

    <ExportAPI("read.string_interactions")>
    Public Function readNetwork(string_interactions As String) As InteractExports()
        Return string_interactions.LoadTsv(Of InteractExports)(encoding:=Encodings.UTF8, mute:=True).ToArray
    End Function

    <ExportAPI("read.coordinates")>
    Public Function readLayout(string_network_coordinates As String) As Coordinates()
        Return string_network_coordinates.LoadTsv(Of Coordinates)(encoding:=Encodings.UTF8, mute:=True).ToArray
    End Function

    ''' <summary>
    ''' parse the string-db table file
    ''' </summary>
    ''' <param name="file">
    ''' the string db protein links data files, example like:
    ''' 
    ''' 1. 9606.protein.links.v11.5.txt
    ''' 2. 9606.protein.links.full.v11.5.txt
    ''' 3. 9606.protein.links.detailed.v11.5.txt
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.string_db")>
    <RApiReturn(GetType(linksDetail), GetType(StringIndex))>
    Public Function parseStringDb(file As String,
                                  Optional remove_taxonomyId As Boolean = True,
                                  Optional link_matrix As Boolean = False,
                                  Optional combine_score As Single = -1) As Object

        Dim links As IEnumerable(Of linksDetail) = linksDetail.LoadFile(path:=file)

        If remove_taxonomyId Then
            links = StringIndex.RemoveTaxonomyIdPrefix(links)
        End If
        If combine_score > 0 Then
            links = links.Where(Function(l) l.combined_score > combine_score)
        End If
        If link_matrix Then
            Return New StringIndex(links)
        Else
            Return pipeline.CreateFromPopulator(links)
        End If
    End Function

    <ROperator("&")>
    Public Function intersect(cor As CorrelationMatrix, stringDb As StringIndex) As CorrelationMatrix
        Return stringDb.Intersect(cor)
    End Function
End Module
