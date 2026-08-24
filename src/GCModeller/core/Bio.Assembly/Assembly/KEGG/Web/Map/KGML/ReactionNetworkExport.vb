#Region "Microsoft.VisualBasic::7f7b7d87f6b828ba1bcd3c224149cf93, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\ReactionNetworkExport.vb"

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

    '   Total Lines: 56
    '    Code Lines: 47 (83.93%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (16.07%)
    '     File Size: 2.49 KB


    '     Class ReactionNetworkExport
    ' 
    '         Properties: ko, koName, mapId, mapName, productId
    '                     productName, reaction, substrateId, substrateName
    ' 
    '         Function: ExtractFromKGML
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.WebServices.KGML

    Public Class ReactionNetworkExport

        Public Property substrateId As String()
        Public Property substrateName As String()
        Public Property productId As String()
        Public Property productName As String()
        Public Property reaction As String
        Public Property ko As String()
        Public Property koName As String

        Public Property mapId As String
        Public Property mapName As String

        Public Shared Iterator Function ExtractFromKGML(kgml As pathway) As IEnumerable(Of ReactionNetworkExport)
            Dim koIndex = kgml.entries _
                .Where(Function(a) a.type = "ortholog") _
                .Select(Function(k)
                            Return k.reaction.StringSplit(" ").Select(Function(rid) (rid, k))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(r) r.rid) _
                .ToDictionary(Function(r) r.Key,
                              Function(r)
                                  Return r.Select(Function(a) a.k).ToArray
                              End Function)

            If kgml.reactions Is Nothing Then
                Return
            End If

            For Each rxn As reaction In kgml.reactions
                Dim subs = rxn.substrates.Select(Function(si) si.name.GetTagValue(":").Value).ToArray
                Dim prod = rxn.products.Select(Function(si) si.name.GetTagValue(":").Value).ToArray
                Dim ko = koIndex.TryGetValue(rxn.name).SafeQuery.Select(Function(kid) kid.name).IteratesALL.Distinct.ToArray
                Dim koNames = ko.Select(Function(kid) kid.GetTagValue(":").Value).Select(Function(kid) GeneNetworkExport.koNames(kid)).ToArray

                Yield New ReactionNetworkExport With {
                    .ko = ko,
                    .koName = koNames.JoinBy("; "),
                    .reaction = rxn.name,
                    .substrateId = subs,
                    .substrateName = subs.Select(Function(cid) GeneNetworkExport.cpdNames(cid)).ToArray,
                    .productId = prod,
                    .productName = prod.Select(Function(cid) GeneNetworkExport.cpdNames(cid)).ToArray,
                    .mapId = kgml.name,
                    .mapName = kgml.title
                }
            Next
        End Function

    End Class
End Namespace
