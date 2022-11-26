#Region "Microsoft.VisualBasic::7e038ac4a09ad86b488103e330297586, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Palindrome\SearchWorker\MirrorPalindrome.vb"

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

    '   Total Lines: 77
    '    Code Lines: 47
    ' Comment Lines: 18
    '   Blank Lines: 12
    '     File Size: 3.06 KB


    '     Class MirrorPalindrome
    ' 
    '         Properties: ResultSet
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: returns
    ' 
    '         Sub: DoSearch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically

    ''' <summary>
    ''' 种子片段在正向链找得到自己的反向片段，即为镜像回文片段
    ''' </summary>
    Public Class MirrorPalindrome : Inherits SearchWorker

        Protected m_resultSet As New List(Of PalindromeLoci)

        Public ReadOnly Property ResultSet As PalindromeLoci()
            Get
                If m_resultSet.Count = 0 Then
                    Return {}
                Else
                    Return returns().ToArray
                End If
            End Get
        End Property

        ''' <summary>
        ''' 在这里会对原始序列进行切割得到<see cref="PalindromeLoci.SequenceData"/>用来验证位点是否正确
        ''' </summary>
        ''' <returns></returns>
        Private Iterator Function returns() As IEnumerable(Of PalindromeLoci)
            Dim LQuery = From site As PalindromeLoci
                         In m_resultSet
                         Select site
                         Group site By site.Mirror Into Group

            Dim loci As NucleotideLocation
            Dim plSite As PalindromeLoci

            For Each matchGroup In LQuery
                Dim siteGroup = NumberGroups.Groups(matchGroup.Group, offset:=min)

                For Each g As GroupResult(Of PalindromeLoci, Integer) In siteGroup
                    plSite = PalindromeLoci.SelectMaxLengthSite(g.Group)
                    loci = plSite.MappingLocation
                    plSite.SequenceData = CutSequence.CutSequenceLinear(Me.seq, loci.left, loci.right)

                    Yield plSite
                Next
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(seq As IPolymerSequenceModel, min As Integer, max As Integer)
            Call MyBase.New(seq, min, max)

            ' 因为是回文重复，所以到一半长度的时候肯定是没有了的
            If Me.max > Len(seq.SequenceData) / 2 Then
                Me.max = CInt(Len(seq.SequenceData) / 2)
            End If
        End Sub

        ''' <summary>
        ''' 只需要判断存不存在就行了，因为是连在一起的，所以现在短片段可能不会有回文，但是长片段却会出现回文，所以不能够在这里移除
        ''' </summary>
        ''' <param name="seed"></param>
        Protected Overrides Sub DoSearch(seed As Seed)
            Call Palindrome.FindMirrorPalindromes(seed.sequence, seq).TrimNull.DoCall(AddressOf m_resultSet.Add)
        End Sub
    End Class
End Namespace
