﻿#Region "Microsoft.VisualBasic::5095fa245cf7eab505d8bd9f7bb0aa2e, analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\SearchReversedRepeats.vb"

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

    '     Class ReversedRepeatSeacher
    ' 
    '         Properties: MinAppeared, ResultSet
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: DoSearch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Pattern
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically

    Public Class ReversedRepeatSeacher : Inherits SearchWorker

        Public ReadOnly Property MinAppeared As Integer
        Public ReadOnly Property ResultSet As New List(Of RevRepeats)

        Sub New(Sequence As IPolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer,
                MinAppeared As Integer)

            Call MyBase.New(Sequence, Min, Max)
            With Me
                ._MinAppeared = MinAppeared
            End With
        End Sub

        ''' <summary>
        ''' 虽然当前的片段没有在序列上面出现，但是上一次迭代的片段却是出现的，假若序列片段没有匹配上，则上一次迭代的序列则可能为重复序列
        ''' </summary>
        ''' <param name="seed"></param>
        Protected Overrides Sub DoSearch(seed As Seed)
            Dim segment As String = New String(seed.Sequence.ToArray.Reverse.ToArray)
            Dim rev As String = NucleicAcid.Complement(segment)

            ' 找不到反向序列或者重复的次数少于阈值的，都将会被删除
            If FindLocation(seq.SequenceData, rev).Length < MinAppeared Then
                Return
            End If

            Dim repeats = GenerateRepeats(seq.SequenceData, rev, MinAppeared)
            If repeats Is Nothing Then
                Return
            End If

            Dim RepeatsLeftLoci As Repeats = RepeatsSearchAPI.GenerateRepeats(
                seq.SequenceData,
                seed.Sequence,
                MinAppeared,
                Rev:=True)
            Dim result As RevRepeats = RevRepeats.GenerateFromBase(repeats)
            result.Locations = RepeatsLeftLoci.Locations
            Call ResultSet.Add(result)
        End Sub
    End Class
End Namespace
