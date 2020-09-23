#Region "Microsoft.VisualBasic::2fec6cde767700c62083eb0c0fa864ba, core\Bio.Repository\KEGG\ReactionRepository\ReactionClassifier.vb"

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

    ' Class ReactionClassifier
    ' 
    '     Properties: Count
    ' 
    '     Function: buildIndex, buildTupleIndex, FromRepository, (+2 Overloads) GetReactantTransform, (+2 Overloads) haveClassification
    '               IsNullOrEmpty
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class ReactionClassifier

    Dim classes As ReactionClass()
    Dim reactionIndex As Dictionary(Of String, ReactionClass())
    Dim compoundTransformIndex As New Dictionary(Of String, ReactionClass())

    Public ReadOnly Property Count As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return classes.Length
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function haveClassification(reactionId As String) As Boolean
        Return reactionIndex.ContainsKey(reactionId)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function haveClassification(reaction As Reaction) As Boolean
        Return reactionIndex.ContainsKey(reaction.ID)
    End Function

    Public Function GetReactantTransform(reaction As Reaction) As IEnumerable(Of (from$, to$))
        If Not haveClassification(reaction) Then
            Return {}
        Else
            With reaction.ReactionModel
                Return GetReactantTransform(
                    reaction:=reaction.ID,
                    reactants:= .Reactants.Select(Function(c) c.ID).ToArray,
                    products:= .Products.Select(Function(c) c.ID).ToArray
                )
            End With
        End If
    End Function

    Public Iterator Function GetReactantTransform(reaction$, reactants$(), products$()) As IEnumerable(Of (from$, to$))
        If Not haveClassification(reaction) Then
            Return
        End If

        For Each reactant As String In reactants
            For Each product As String In products
                If compoundTransformIndex.ContainsKey($"{reactant}_{product}") Then
                    Yield (reactant, product)
                End If
                If compoundTransformIndex.ContainsKey($"{product}_{reactant}") Then
                    Yield (product, reactant)
                End If
            Next
        Next
    End Function

    Private Function buildTupleIndex() As ReactionClassifier
        compoundTransformIndex = classes _
            .Select(Function(cls)
                        Return cls _
                            .reactantPairs _
                            .Select(Function(tuple)
                                        Return (key:=$"{tuple.from}_{tuple.to}", cls:=cls)
                                    End Function)
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(transform) transform.key) _
            .ToDictionary(Function(transform) transform.Key,
                          Function(group)
                              Return group _
                                  .Select(Function(t) t.cls) _
                                  .GroupBy(Function(c) c.entryId) _
                                  .Select(Function(r)
                                              Return r.First
                                          End Function) _
                                  .ToArray
                          End Function)
        Return Me
    End Function

    Private Function buildIndex() As ReactionClassifier
        reactionIndex = classes _
            .Select(Function(cls)
                        Return cls.reactions.Keys.Select(Function(rid) (rid:=rid, cls:=cls))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(r) r.rid) _
            .ToDictionary(Function(rid) rid.Key,
                          Function(group)
                              Return group _
                                  .Select(Function(r) r.cls) _
                                  .GroupBy(Function(r) r.entryId) _
                                  .Select(Function(g)
                                              Return g.First
                                          End Function) _
                                  .ToArray
                          End Function)
        Return Me
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromRepository(directory As String) As ReactionClassifier
        Return New ReactionClassifier With {
            .classes = ReactionClass.ScanRepository(directory).ToArray
        }.buildIndex _
         .buildTupleIndex
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function IsNullOrEmpty(classifier As ReactionClassifier) As Boolean
        Return classifier Is Nothing OrElse classifier.classes.IsNullOrEmpty
    End Function
End Class
