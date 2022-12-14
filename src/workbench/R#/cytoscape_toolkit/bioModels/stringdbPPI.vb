#Region "Microsoft.VisualBasic::765b5116518c29a1435fb6c8bf52370b, R#\cytoscape_toolkit\bioModels\stringdbPPI.vb"

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

' Module stringdbPPI
' 
'     Function: buildNetworkModel, readLayout, readNetwork
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Data.[STRING].StringDB.Tsv
Imports SMRUCC.genomics.Model.Network.KEGG
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
        Dim model As NetworkTables = stringNetwork.BuildModel(uniprot:=annotations, groupValues:=FunctionalNetwork.KOGroupTable)
        Dim graph = model.CreateGraph

        If Not coordinates.IsNullOrEmpty Then
            Return graph.applyLayout(coordinates)
        Else
            Return graph
        End If
    End Function

    <ExportAPI("read.string_interactions")>
    Public Function readNetwork(string_interactions As String) As InteractExports()
        Return string_interactions.LoadTsv(Of InteractExports).ToArray
    End Function

    <ExportAPI("read.coordinates")>
    Public Function readLayout(string_network_coordinates As String) As Coordinates()
        Return string_network_coordinates.LoadTsv(Of Coordinates).ToArray
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
    <RApiReturn(GetType(linksDetail))>
    Public Function parseStringDb(file As String) As Object
        Return linksDetail _
            .LoadFile(path:=file) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function
End Module
