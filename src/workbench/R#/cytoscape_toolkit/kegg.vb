#Region "Microsoft.VisualBasic::6f6a40d3b13abcbd4ac9d20706d37d53, R#\cytoscape_toolkit\kegg.vb"

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

' Module kegg
' 
'     Function: compoundNetwork
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

''' <summary>
''' The KEGG metabolism pathway network data R# scripting plugin for cytoscape software
''' </summary>
<Package("cytoscape.kegg", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module kegg

    ''' <summary>
    ''' Create kegg metabolism network based on the given metabolite compound data.
    ''' </summary>
    ''' <param name="reactions">The kegg ``br08201`` reaction database.</param>
    ''' <param name="compounds">Kegg compound id list</param>
    ''' <returns></returns>
    <ExportAPI("compounds.network")>
    Public Function compoundNetwork(reactions As ReactionTable(), compounds$(),
                                    Optional enzymes As Dictionary(Of String, String()) = Nothing,
                                    Optional filterByEnzymes As Boolean = False,
                                    Optional extended As Boolean = False) As NetworkGraph
        Return compounds _
            .Select(Function(cpd)
                        Return New NamedValue(Of String)(cpd, cpd)
                    End Function) _
            .DoCall(Function(list)
                        Return reactions.BuildModel(
                            compounds:=list,
                            enzymes:=enzymes,
                            filterByEnzymes:=filterByEnzymes,
                            extended:=extended
                        )
                    End Function)
    End Function

    <ExportAPI("pathway_class")>
    Public Function assignPathwayClass(graph As NetworkGraph, maps As Map()) As NetworkGraph
        Dim compounds = graph.vertex.Where(Function(a) a.label.IsPattern("[GC]\d+")).ToArray
        Dim assignments As New Dictionary(Of String, List(Of String))

        For Each id In compounds
            assignments.Add(id.label, New List(Of String))
        Next

        For Each map As Map In maps
            For Each id As String In map.GetMembers
                If assignments.ContainsKey(id) Then
                    assignments(id).Add(map.id)
                End If
            Next
        Next

        Return graph
    End Function
End Module

