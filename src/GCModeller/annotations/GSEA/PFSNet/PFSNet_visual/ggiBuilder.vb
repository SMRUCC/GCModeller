#Region "Microsoft.VisualBasic::c10c12f4da1f0b840457b806b3f72b20, GCModeller\annotations\GSEA\PFSNet\PFSNet_visual\ggiBuilder.vb"

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

    '   Total Lines: 52
    '    Code Lines: 40
    ' Comment Lines: 6
    '   Blank Lines: 6
    '     File Size: 2.07 KB


    ' Module ggiBuilder
    ' 
    '     Function: ReferenceCompoundNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

Public Module ggiBuilder

    ''' <summary>
    ''' apply for LC-MS data analysis
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="reactions"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function ReferenceCompoundNetwork(maps As IEnumerable(Of Map), reactions As IEnumerable(Of ReactionTable)) As IEnumerable(Of GraphEdge)
        Dim fluxIndex As Dictionary(Of String, ReactionTable) = reactions _
            .GroupBy(Function(r) r.entry) _
            .ToDictionary(Function(r) r.Key,
                          Function(r)
                              Return r.First
                          End Function)
        Dim fluxEntries As String()
        Dim compoundEntries As String()
        Dim allId As String()
        Dim mapName As String

        For Each map As Map In maps
            allId = map.GetMembers
            fluxEntries = allId.Where(Function(id) id.IsPattern("R\d+")).ToArray
            mapName = $"{map.id}: {map.Name.Replace(" - Reference pathway", "").Trim}"

            For Each reaction As ReactionTable In fluxEntries _
                .Where(AddressOf fluxIndex.ContainsKey) _
                .Select(Function(id)
                            Return fluxIndex(id)
                        End Function)

                compoundEntries = reaction.substrates.AsList + reaction.products

                For Each a As String In compoundEntries
                    For Each b As String In compoundEntries.Where(Function(id) id <> a)
                        Yield New GraphEdge With {
                            .pathwayID = mapName,
                            .g1 = a,
                            .g2 = b
                        }
                    Next
                Next
            Next
        Next
    End Function
End Module
