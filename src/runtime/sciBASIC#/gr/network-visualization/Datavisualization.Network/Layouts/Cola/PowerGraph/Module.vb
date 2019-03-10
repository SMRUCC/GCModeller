﻿#Region "Microsoft.VisualBasic::e5feaef5936738085003af6f3e20dc81, gr\network-visualization\Datavisualization.Network\Layouts\Cola\PowerGraph\Module.vb"

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

    '     Class [Module]
    ' 
    '         Properties: isIsland, isLeaf, isPredefined
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: getEdges
    ' 
    '     Class ModuleSet
    ' 
    '         Properties: count
    ' 
    '         Function: contains, intersection, intersectionCount, modules
    ' 
    '         Sub: add, forAll, remove
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Layouts.Cola


    Public Class [Module]

        Public gid As Integer?
        Public id As Integer

        Public outgoing As New LinkSets
        Public incoming As New LinkSets
        Public children As New ModuleSet
        Public definition As Dictionary(Of String, Object)

        Default Public Property LinkSetItem(name As String) As LinkSets
            Get
                If name = NameOf(outgoing) Then
                    Return outgoing
                ElseIf name = NameOf(incoming) Then
                    Return incoming
                Else
                    Throw New NotImplementedException(name)
                End If
            End Get
            Set(value As LinkSets)
                If name = NameOf(outgoing) Then
                    outgoing = value
                ElseIf name = NameOf(incoming) Then
                    incoming = value
                Else
                    Throw New NotImplementedException(name)
                End If
            End Set
        End Property

        Public ReadOnly Property isLeaf() As Boolean
            Get
                Return Me.children.Count() = 0
            End Get
        End Property

        Public ReadOnly Property isIsland() As Boolean
            Get
                Return Me.outgoing.Count() = 0 AndAlso Me.incoming.Count() = 0
            End Get
        End Property

        Public ReadOnly Property isPredefined() As Boolean
            Get
                Return Me.definition IsNot Nothing
            End Get
        End Property

        Public Sub New(id%,
                       Optional outgoing As LinkSets = Nothing,
                       Optional incoming As LinkSets = Nothing,
                       Optional children As ModuleSet = Nothing,
                       Optional definition As Dictionary(Of String, Object) = Nothing)

            Me.id = id
            Me.outgoing = outgoing
            Me.incoming = incoming
            Me.children = children
            Me.definition = definition
        End Sub

        Public Sub getEdges(es As List(Of PowerEdge(Of Integer)))
            Me.outgoing.forAll(Sub(ms, edgetype)
                                   ms.forAll(Sub(target)
                                                 es.Add(New PowerEdge(Of Integer)(Me.id, target.id, edgetype))
                                             End Sub)
                               End Sub)
        End Sub
    End Class

    Public Class ModuleSet

        Dim table As New Dictionary(Of String, [Module])

        Public ReadOnly Property count() As Integer
            Get
                Return table.Count
            End Get
        End Property

        Public Function intersection(other As ModuleSet) As ModuleSet
            Dim result = New ModuleSet()
            result.table = powergraphExtensions.intersection(Me.table, other.table)
            Return result
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function intersectionCount(other As ModuleSet) As Integer
            Return Me.intersection(other).count()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function contains(id As Double) As Boolean
            Return Me.table.ContainsKey(id)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub add(m As [Module])
            Me.table(m.id.ToString) = m
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub remove(m As [Module])
            Call Me.table.Remove(m.id.ToString)
        End Sub

        Public Sub forAll(f As Action(Of [Module]))
            For Each mid As String In Me.table.Keys
                f(Me.table(mid))
            Next
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function modules() As [Module]()
            Return table.Values.Where(Function(m) m.isPredefined).ToArray
        End Function
    End Class
End Namespace
