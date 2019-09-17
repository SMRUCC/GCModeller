#Region "Microsoft.VisualBasic::2653a2e20c531d8522e9d0d68cdcbd44, analysis\SequenceToolkit\SequencePatterns\Topologically\Palindrome\SearchWorker\MirrorPalindrome.vb"

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

    '     Class MirrorPalindrome
    ' 
    '         Properties: ResultSet
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __return
    ' 
    '         Sub: DoSearch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically

    ''' <summary>
    ''' 种子片段在正向链找得到自己的反向片段，即为镜像回文片段
    ''' </summary>
    Public Class MirrorPalindrome : Inherits SearchWorker

        Public ReadOnly Property ResultSet As IEnumerable(Of PalindromeLoci)
            Get
                If _resultSet.Count = 0 Then
                    Return {}
                Else
                    Return __return()
                End If
            End Get
        End Property

        ''' <summary>
        ''' 在这里会对原始序列进行切割得到<see cref="PalindromeLoci.SequenceData"/>用来验证位点是否正确
        ''' </summary>
        ''' <returns></returns>
        Private Function __return() As PalindromeLoci()
            Dim LQuery = From site As PalindromeLoci
                         In _resultSet
                         Select site
                         Group site By site.Mirror Into Group

            Dim palindromes As New List(Of PalindromeLoci)

            For Each matchGroup In LQuery
                Dim siteGroup = NumberGroups.Groups(matchGroup.Group, offset:=min)
                Dim site As PalindromeLoci

                For Each g As GroupResult(Of PalindromeLoci, Integer) In siteGroup
                    site = PalindromeLoci.SelectMaxLengthSite(g.Group)
                    site.SequenceData = MyBase.seq _
                        .CutSequenceLinear(site.MappingLocation) _
                        .SequenceData
                    palindromes += site
                Next
            Next

            Return palindromes
        End Function

        Protected _resultSet As New List(Of PalindromeLoci)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(Sequence As IPolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer)

            Call MyBase.New(Sequence, Min, Max)

            ' 因为是回文重复，所以到一半长度的时候肯定是没有了的
            If Me.max > Len(Sequence.SequenceData) / 2 Then
                Me.max = Len(Sequence.SequenceData) / 2
            End If
        End Sub

        ''' <summary>
        ''' 只需要判断存不存在就行了，因为是连在一起的，所以现在短片段可能不会有回文，但是长片段却会出现回文，所以不能够在这里移除
        ''' </summary>
        ''' <param name="seed"></param>
        Protected Overrides Sub DoSearch(seed As Seed)
            Dim sites = Palindrome.FindMirrorPalindromes(seed.Sequence, seq.SequenceData).TrimNull
            Call _resultSet.Add(sites)
        End Sub
    End Class
End Namespace
