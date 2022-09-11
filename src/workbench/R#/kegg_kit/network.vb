#Region "Microsoft.VisualBasic::8c35dc0043628e865116ee898bbbe4ba, R#\kegg_kit\network.vb"

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

    '   Total Lines: 60
    '    Code Lines: 42
    ' Comment Lines: 10
    '   Blank Lines: 8
    '     File Size: 2.36 KB


    ' Module network
    ' 
    '     Function: assignKeggClass, fromCompoundId
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

<Package("network")>
Module network

    <ExportAPI("fromCompounds")>
    Public Function fromCompoundId(compoundsId As String(), graph As Reaction(),
                                   Optional compounds As CompoundRepository = Nothing,
                                   Optional enzymeBridged As Boolean = True) As NetworkGraph

        ' BuildModel(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)),
        Dim template As ReactionTable() = ReactionTable.Load(graph).ToArray
        Dim cid As NamedValue(Of String)()

        If compounds Is Nothing Then
            cid = compoundsId _
                .Select(Function(c)
                            Return New NamedValue(Of String)(c, c, c)
                        End Function) _
                .ToArray
        Else
            cid = compoundsId _
                .Select(Function(c)
                            Dim model As Compound = compounds.GetByKey(c)
                            Dim name As String = If(model Is Nothing, c, If(model.commonNames.FirstOrDefault, c))

                            Return New NamedValue(Of String)(c, name, c)
                        End Function) _
                .ToArray
        End If

        Return template.BuildModel(
            compounds:=cid,
            extended:=False,
            enzymaticRelated:=False,
            ignoresCommonList:=False,
            enzymeBridged:=enzymeBridged
        )
    End Function

    ''' <summary>
    ''' assign kegg class to the graph nodes
    ''' </summary>
    ''' <param name="g">
    ''' a network graph data model, with nodes label id must 
    ''' be the kegg pathway id and compounds id or kegg KO id
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("assignKeggClass")>
    Public Function assignKeggClass(g As NetworkGraph) As NetworkGraph
        ' first assign the class id to the pathway id node

    End Function
End Module

