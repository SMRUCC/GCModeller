﻿#Region "Microsoft.VisualBasic::c9a3c4b9d3520928c8bb8ba39a67dc1f, LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\Models\Hits\Blastp.vb"

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

    '     Class BlastpSubjectHit
    ' 
    '         Properties: FragmentHits, Hsp, LengthHit, LengthQuery
    ' 
    '     Class FragmentHit
    ' 
    '         Properties: HitLength, HitName, Hsp, Score
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Class BlastpSubjectHit : Inherits SubjectHit

        Public Property FragmentHits As FragmentHit()

        Public Overrides Property Hsp As HitSegment()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return FragmentHits _
                    .Select(Function(f) f.Hsp) _
                    .IteratesALL _
                    .ToArray
            End Get
            Set(value As HitSegment())
                MyBase.Hsp = value
            End Set
        End Property

        Public Overrides ReadOnly Property LengthHit As Integer
            Get
                Dim LQuery As IEnumerable(Of Integer) =
                    LinqAPI.Exec(Of Integer) <= From Segment As HitSegment
                                                In Hsp
                                                Select From ch As Char
                                                       In Segment.Query.SequenceData
                                                       Where ch = "-"c
                                                       Select 1
                Dim value As Integer = LQuery.Sum
                Return FragmentHits.Select(Function(s) s.Score.Gaps.Denominator).Sum - value  ' 减去插入的空格就是比对上的长度了
            End Get
        End Property

        Public Overrides ReadOnly Property LengthQuery As Integer
            Get
                Dim LQuery As Integer() =
                    LinqAPI.Exec(Of Integer) <= From Segment As HitSegment
                                                In Hsp
                                                Select From ch As Char
                                                       In Segment.Subject.SequenceData
                                                       Where ch = "-"c
                                                       Select 1
                Dim value As Integer = LQuery.Sum
                Return FragmentHits.Select(Function(s) s.Score.Gaps.Denominator).Sum - value
            End Get
        End Property
    End Class

    Public Class FragmentHit

        Public Property HitName As String
        Public Property HitLength As Integer
        Public Property Score As Score
        Public Property Hsp As HitSegment()

        Public Overrides Function ToString() As String
            Return HitName
        End Function
    End Class
End Namespace
