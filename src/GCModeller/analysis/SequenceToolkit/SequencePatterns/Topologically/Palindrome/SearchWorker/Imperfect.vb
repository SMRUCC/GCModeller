#Region "Microsoft.VisualBasic::25a3cab9b5932ae13db1b7062a406202, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Palindrome\SearchWorker\Imperfect.vb"

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

    '   Total Lines: 88
    '    Code Lines: 63
    ' Comment Lines: 13
    '   Blank Lines: 12
    '     File Size: 3.69 KB


    '     Class Imperfect
    ' 
    '         Properties: ResultSet
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: createOutput
    ' 
    '         Sub: DoSearch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Search
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
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
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
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
        ''' <param name="nt"></param>
        ''' <param name="Min">The minimum length of the repeat sequence loci.</param>
        ''' <param name="Max">The maximum length of the repeat sequence loci.</param>
        Sub New(nt As IPolymerSequenceModel, min%, max%, cutoff#, maxDist%, partitions%)
            Call MyBase.New(nt, min, max)

            ' 因为是回文重复，所以到一半长度的时候肯定是没有了的
            If Me.max > Len(nt.SequenceData) / 2 Then
                Me.max = CInt(Len(nt.SequenceData) / 2)
            End If

            _index = New TextIndexing(nt.SequenceData, min, max)
            _cutoff = cutoff
            _maxDist = maxDist
            _partitions = partitions
        End Sub

        Protected Overrides Sub DoSearch(seed As Seed)
            Dim palLoci As String = PalindromeLoci.GetPalindrome(seed.sequence)
            Dim pali As Map(Of TextSegment, DistResult)() = _index.Found(seed.sequence, _cutoff, _partitions)
            Dim segment As String = seed.sequence
            Dim locis%() = IScanner.FindLocation(seq, segment).ToArray
            Dim imperfects = LinqAPI.Exec(Of ImperfectPalindrome) _
 _
                () <= From loci As Map(Of TextSegment, DistResult)
                      In pali.AsParallel
                      Let palLeft As Integer = loci.Key.Index
                      Select createOutput(locis, palLeft, segment, loci)

            Call _resultSet.AddRange(imperfects)
        End Sub

        Private Function createOutput(locis%(), palLeft%, segment$, loci As Map(Of TextSegment, DistResult)) As IEnumerable(Of ImperfectPalindrome)
            Dim levenshtein As DistResult = loci.Maps

            Return From left As Integer
                   In locis
                   Let d As Integer = palLeft - left
                   Where d > 0 AndAlso d <= _maxDist
                   Select New ImperfectPalindrome With {
                       .Site = segment,
                       .Left = left,
                       .Palindrome = loci.Key.Segment,
                       .Paloci = loci.Key.Index,
                       .Distance = levenshtein.Distance,
                       .Evolr = levenshtein.DistEdits,
                       .Matches = levenshtein.Matches,
                       .Score = levenshtein.Score
                   }
        End Function
    End Class
End Namespace
