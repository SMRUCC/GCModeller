#Region "Microsoft.VisualBasic::0dc58851dc08a8d6a0831cbaa37e416d, annotations\GPR\ContextIndices.vb"

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

    '   Total Lines: 62
    '    Code Lines: 49 (79.03%)
    ' Comment Lines: 2 (3.23%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (17.74%)
    '     File Size: 2.60 KB


    ' Class ContextIndices
    ' 
    '     Properties: ECtoReactions, PathwayReactions, Pathways, ReactionToPathways
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: BuildIndices, FindCommonPathways, GetPathwayForReaction
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.MetabolicModel

Public Class ContextIndices

    Public Property ECtoReactions As Dictionary(Of String, List(Of MetabolicReaction))
    Public Property ReactionToPathways As Dictionary(Of String, List(Of Pathway))
    Public Property PathwayReactions As Dictionary(Of String, List(Of MetabolicReaction))

    Public Property Pathways As Pathway()

    Public Sub New(pathways As IEnumerable(Of Pathway))
        Me.ECtoReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)
        Me.ReactionToPathways = New Dictionary(Of String, List(Of Pathway))(StringComparer.OrdinalIgnoreCase)
        Me.PathwayReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)
        Me.Pathways = BuildIndices(pathways).ToArray
    End Sub

    Private Iterator Function BuildIndices(pathways As IEnumerable(Of Pathway)) As IEnumerable(Of Pathway)
        For Each pathway As Pathway In pathways.SafeQuery
            PathwayReactions(pathway.ID) = pathway.metabolicNetwork.ToList()

            For Each rxn In pathway.metabolicNetwork
                ' 构建反应到通路的映射
                If Not ReactionToPathways.ContainsKey(rxn.id) Then
                    ReactionToPathways(rxn.id) = New List(Of Pathway)()
                End If
                ReactionToPathways(rxn.id).Add(pathway)

                ' 构建EC到反应的映射（支持多EC号）
                For Each ec In rxn.ECNumbers
                    If Not ECtoReactions.ContainsKey(ec) Then
                        ECtoReactions(ec) = New List(Of MetabolicReaction)()
                    End If
                    ECtoReactions(ec).Add(rxn)
                Next
            Next

            Yield pathway
        Next
    End Function

    Public Function GetPathwayForReaction(reaction As MetabolicReaction) As IEnumerable(Of Pathway)
        If ReactionToPathways.ContainsKey(reaction.id) Then
            Return ReactionToPathways(reaction.id)
        Else
            Return {}
        End If
    End Function

    Public Iterator Function FindCommonPathways(allECs As IReadOnlyCollection(Of String)) As IEnumerable(Of Pathway)
        If allECs Is Nothing OrElse allECs.Count = 0 Then
            Return
        End If

        For Each pathway As Pathway In Pathways
            If pathway.CheckAllECNumberExists(allECs) Then
                Yield pathway
            End If
        Next
    End Function
End Class
