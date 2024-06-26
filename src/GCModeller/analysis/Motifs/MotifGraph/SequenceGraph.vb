﻿#Region "Microsoft.VisualBasic::ea7adad839387680696a51078ec40882, analysis\Motifs\MotifGraph\SequenceGraph.vb"

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

    '   Total Lines: 120
    '    Code Lines: 72 (60.00%)
    ' Comment Lines: 26 (21.67%)
    '    - Xml Docs: 88.46%
    ' 
    '   Blank Lines: 22 (18.33%)
    '     File Size: 3.78 KB


    ' Class SequenceGraph
    ' 
    '     Properties: composition, graph, id, len, triple
    '                 tuple, tuple_distance
    ' 
    '     Function: (+2 Overloads) GetVector
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI

''' <summary>
''' A machine learning vector model for motif analysis
''' </summary>
Public Class SequenceGraph : Implements INamedValue

    ''' <summary>
    ''' the unique reference id of current sequence graph model
    ''' </summary>
    ''' <returns></returns>
    Public Property id As String Implements INamedValue.Key
    Public Property composition As Dictionary(Of Char, Double)
    ''' <summary>
    ''' A tuple graph
    ''' </summary>
    ''' <returns></returns>
    Public Property graph As Dictionary(Of Char, Dictionary(Of Char, Double))

    ''' <summary>
    ''' deflat of the <see cref="graph"/>.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property tuple As Dictionary(Of String, Double)
        Get
            Dim list As New Dictionary(Of String, Double)

            For Each c As Char In graph.Keys
                Dim t = graph(c)

                For Each d As Char In t.Keys
                    list($"{c}{d}") = t(d)
                Next
            Next

            Return list
        End Get
    End Property

    Public Property triple As Dictionary(Of String, Double)
    Public Property tuple_distance As Dictionary(Of String, Double)
    ''' <summary>
    ''' the sequence length
    ''' </summary>
    ''' <returns></returns>
    Public Property len As Integer

    ''' <summary>
    ''' get vector by default charset in <see cref="composition"/>
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetVector(Optional norm As Boolean = False) As Double()
        Return GetVector(components:=composition.Keys, norm)
    End Function

    Public Function GetVector(components As IReadOnlyCollection(Of Char), Optional norm As Boolean = False) As Double()
        Dim v As New List(Of Double)
        Dim g As Dictionary(Of Char, Double)
        Dim tuple_graph As String() = DistanceGraph.GetTuples(components).ToArray
        Dim tmp As New List(Of Double)

        Call v.AddRange(components.Select(Function(ci) composition(ci)))
        Call tmp.Clear()

        ' tuple
        For Each key As Char In components
            g = graph(key)
            tmp.AddRange(components.Select(Function(ci) g(ci)))
        Next

        Const eps As Double = 0.00000000000001

        If norm Then
            Call v.AddRange(New Vector(tmp) / (tmp.Max + eps))
        Else
            Call v.AddRange(tmp)
        End If

        If Not triple.IsNullOrEmpty Then
            ' protein sequence is empty for triple data
            Call tmp.Clear()

            For Each t As String In CodonBiasVector.PopulateTriples(components)
                Call tmp.Add(triple.TryGetValue(t))
            Next

            If norm Then
                Call v.AddRange(New Vector(tmp) / (tmp.Max + eps))
            Else
                Call v.AddRange(tmp)
            End If
        End If

        Call tmp.Clear()

        For Each t1 As String In tuple_graph
            For Each t2 As String In tuple_graph
                If t1 <> t2 Then
                    Call tmp.Add(tuple_distance.TryGetValue($"{t1}|{t2}"))
                End If
            Next
        Next

        If norm Then
            Call v.AddRange(New Vector(tmp) / (tmp.Max + eps))
        Else
            Call v.AddRange(tmp)
        End If

        Call tmp.Clear()

        Return v.ToArray
    End Function

End Class
