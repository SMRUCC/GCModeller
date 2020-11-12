#Region "Microsoft.VisualBasic::bddc4abef0436bea4e99841a1cdc332c, models\Networks\KEGG\ReactionNetwork\Builder\EdgeFilter.vb"

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

    '     Class EdgeFilter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateFilter, ReactionLinkFilter, SlotCompleteFilter
    ' 
    '     Enum EdgeFilterEngine
    ' 
    '         ReactionLinkFilter, SlotCompleteFilter
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class SlotCompleteFilter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: filter
    ' 
    '     Class ReactionLinkFilter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: filter
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace ReactionNetwork

    Public MustInherit Class EdgeFilter

        Protected ReadOnly nodes As CompoundNodeTable
        Protected ReadOnly networkBase As Dictionary(Of String, ReactionTable)
        Protected ReadOnly commonIgnores As Index(Of String)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="nodes">
        ''' the compound search range of current pathway network.
        ''' </param>
        ''' <param name="networkBase"></param>
        ''' <param name="commonIgnores"></param>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(nodes As CompoundNodeTable,
                networkBase As Dictionary(Of String, ReactionTable),
                commonIgnores As Index(Of String))

            Me.nodes = nodes
            Me.networkBase = networkBase
            Me.commonIgnores = commonIgnores
        End Sub

        Public MustOverride Function filter(reactionIds As String()) As IEnumerable(Of String)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function SlotCompleteFilter(nodes As CompoundNodeTable,
                                                  networkBase As Dictionary(Of String, ReactionTable),
                                                  commonIgnores As Index(Of String)) As EdgeFilter

            Return New SlotCompleteFilter(nodes, networkBase, commonIgnores)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ReactionLinkFilter(nodes As CompoundNodeTable,
                                                  networkBase As Dictionary(Of String, ReactionTable),
                                                  commonIgnores As Index(Of String)) As EdgeFilter

            Return New ReactionLinkFilter(nodes, networkBase, commonIgnores)
        End Function

        Public Shared Function CreateFilter(nodes As CompoundNodeTable,
                                            networkBase As Dictionary(Of String, ReactionTable),
                                            commonIgnores As Index(Of String),
                                            engine As EdgeFilterEngine) As EdgeFilter

            Select Case engine
                Case EdgeFilterEngine.ReactionLinkFilter
                    Return ReactionLinkFilter(nodes, networkBase, commonIgnores)
                Case EdgeFilterEngine.SlotCompleteFilter
                    Return SlotCompleteFilter(nodes, networkBase, commonIgnores)
                Case Else
                    Throw New NotImplementedException
            End Select
        End Function

    End Class

    Public Enum EdgeFilterEngine
        SlotCompleteFilter
        ReactionLinkFilter
    End Enum

    ''' <summary>
    ''' all compound should be exists for the required reaction model
    ''' </summary>
    Public Class SlotCompleteFilter : Inherits EdgeFilter

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(nodes As CompoundNodeTable,
                       networkBase As Dictionary(Of String, ReactionTable),
                       commonIgnores As Index(Of String))

            Call MyBase.New(nodes, networkBase, commonIgnores)
        End Sub

        Public Overrides Iterator Function filter(reactionIds() As String) As IEnumerable(Of String)
            ' only populate the reaction id
            ' which theirs compounds substrate
            ' all exists in graph nodes
            For Each reaction As ReactionTable In reactionIds.Select(Function(id) networkBase(id))
                Dim check As Boolean = True

                For Each cid As String In reaction.products.AsList + reaction.substrates
                    If Not nodes.containsKey(cid) Then
                        If Not cid Like commonIgnores Then
                            check = False
                            Exit For
                        End If
                    End If
                Next

                If check Then
                    Yield reaction.entry
                End If
            Next
        End Function
    End Class

    ''' <summary>
    ''' the given reaction should contains at least one compound on each side
    ''' </summary>
    Public Class ReactionLinkFilter : Inherits EdgeFilter

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(nodes As CompoundNodeTable,
                       networkBase As Dictionary(Of String, ReactionTable),
                       commonIgnores As Index(Of String))

            Call MyBase.New(nodes, networkBase, commonIgnores)
        End Sub

        Public Overrides Iterator Function filter(reactionIds() As String) As IEnumerable(Of String)
            For Each reaction As ReactionTable In reactionIds.Select(Function(id) networkBase(id))
                Dim check As Boolean = True

                If Not reaction.products.Any(AddressOf nodes.containsKey) Then
                    check = False
                End If
                If Not reaction.substrates.Any(AddressOf nodes.containsKey) Then
                    check = False
                End If

                If check Then
                    Yield reaction.entry
                End If
            Next
        End Function
    End Class
End Namespace
