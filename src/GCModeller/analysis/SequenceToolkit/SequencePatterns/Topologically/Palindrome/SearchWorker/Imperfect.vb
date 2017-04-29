#Region "Microsoft.VisualBasic::c4e1d898b6b39aeabf22921c3bcc973d, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Palindrome\Imperfect.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports Microsoft.VisualBasic.Text.Search
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Pattern
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically

    ''' <summary>
    ''' 在互补链部分的回文，由于Mirror其实就是简单重复序列，所以在这里不再编写了
    ''' </summary>
    Public Class Imperfect : Inherits SearchWorker

        ReadOnly _index As TextIndexing
        ReadOnly _cutoff As Double

        ReadOnly _resultSet As New List(Of ImperfectPalindrome)

        Public Shadows ReadOnly Property ResultSet As ImperfectPalindrome()
            Get
                Return _resultSet.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 种子位点和回文位点之间的最大的距离
        ''' </summary>
        ReadOnly _maxDist As Integer
        ReadOnly _partitions As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(Sequence As IPolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer,
                cutoff As Double,
                maxDist As Integer,
                partitions As Integer)

            Call MyBase.New(Sequence, Min, Max)

            ' 因为是回文重复，所以到一半长度的时候肯定是没有了的
            If Me.max > Len(Sequence.SequenceData) / 2 Then
                Me.max = Len(Sequence.SequenceData) / 2
            End If

            _index = New TextIndexing(Sequence.SequenceData, Min, Max)
            _cutoff = cutoff
            _maxDist = maxDist
            _partitions = partitions
        End Sub

        Protected Overrides Sub DoSearch(seed As Seed)
            Dim palLoci As String = PalindromeLoci.GetPalindrome(seed.Sequence)
            Dim pali As Map(Of TextSegment, DistResult)() =
                _index.Found(seed.Sequence, _cutoff, _partitions)
            Dim segment As String = seed.Sequence
            Dim locis%() = FindLocation(seq.SequenceData, segment)
            Dim imperfects = LinqAPI.Exec(Of ImperfectPalindrome) <=
                From loci As Map(Of TextSegment, DistResult)
                In pali.AsParallel
                Let palLeft As Integer = loci.Key.Index
                Let lev As DistResult = loci.Maps
                Select From left As Integer
                       In locis
                       Let d As Integer = palLeft - left
                       Where d > 0 AndAlso d <= _maxDist
                       Select New ImperfectPalindrome With {
                           .Site = segment,
                           .Left = left,
                           .Palindrome = loci.Key.Segment,
                           .Paloci = loci.Key.Index,
                           .Distance = lev.Distance,
                           .Evolr = lev.DistEdits,
                           .Matches = lev.Matches,
                           .Score = lev.Score
                       }

            Call _resultSet.AddRange(imperfects)
        End Sub
    End Class
End Namespace
