#Region "Microsoft.VisualBasic::ff4d3281981419e40fe759644ab1db69, analysis\Motifs\CRISPR\CRT\SearchingModel\CRTMotifSearchTool.vb"

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

    '     Module CRTMotifSearchTool
    ' 
    '         Function: ExactKMerMatches
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace SearchingModel

    ''' <summary>
    ''' CRT's search for CRISPRs is based on finding a series of
    ''' short exact repeats of length k that are separated by a similar
    ''' distance and then extending these exact k-mer
    ''' matches to the actual repeat length.
    ''' </summary>
    ''' <remarks></remarks>
    Public Module CRTMotifSearchTool

        ''' <summary>
        ''' The approach taken is this paper is to read the characters to the left or right of all repeats and compute
        ''' occurrence percentages for each base, ACGT. If there is a character that has an occurrence percentage greater
        ''' than or equal to some preset value, p, the repeats are extended.
        ''' 
        ''' 搜索的方法通过统计一个窗口在延展的时候，对某个碱基的出现频率进行统计，假若频率变化异常，则可能检测到了一个motif或者CRISPR位点
        ''' 
        ''' 
        ''' 假若没有搜索到位点，则会向前移动继续进行搜索
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' The value in the search window represents
        ''' a candidate repeat, and each time the window reads a new
        ''' k-mer, the algorithm searches forward for exact k-mer
        ''' matches. When searching for each successive match, the
        ''' search space can be restricted to a small range, called
        ''' search range.
        ''' 每一个窗口数据都会被当作为候选的CRISPR位点
        ''' 当达到匹配条件的时候，搜索空间会被限制在一个很小的范围内
        ''' </remarks>
        ''' <seealso name="kmer.kmerprofile.k"></seealso>
        ''' The algorithm begins its search for repeats with a left-toright
        ''' scan of a sequence using a small sliding search window
        ''' of length k.
        ''' <param name="p">
        ''' This method of extending repeats works well for CRISPRs, give an appropriate value for p.(CRT uses a default value of 75%).
        ''' </param>
        Public Function ExactKMerMatches(nt As NucleicAcid, profile As KmerProfile,
                                         Optional p# = 0.75,
                                         Optional MinNumberOfRepeats% = 3) As CRISPR()

            Dim CRISPRVector As New List(Of CRISPR)
            Dim sequenceLength = nt.Length
            Dim repeatsFound As Boolean = False

            If (profile.k < 6) OrElse (profile.k > 9) Then
                'let user know that window size has changed
                profile.k = 8
            End If

            Dim spacerToSpacerMaxSimilarity As Double = 0.62
            Dim spacerToSpacerLengthDiff As Integer = 12
            Dim spacerToRepeatLengthDiff As Integer = 30

            'the mumber of bases that can be skipped while we still guarantee that the entire search
            'window will at some point in its iteration thru the sequence will not miss a any repeat
            Dim skips As Integer = profile.minR - (2 * profile.k - 1)
            If skips < 1 Then
                skips = 1
            End If

            Console.WriteLine("Searching for repeats...")

            Dim SearchTool As New BoyerMooreAlgorithmSearcher()
            Dim SearchEnd As Integer = sequenceLength - profile.maxR - profile.maxS - profile.k
            Dim j As Integer = 0

            While j <= SearchEnd

                Dim CandidateCRISPR As CRISPR = New CRISPR()
                Dim beginSearch As Integer = j + profile.minR + profile.minS
                Dim endSearch As Integer = j + profile.maxR + profile.maxS + profile.k

                If endSearch > sequenceLength Then
                    endSearch = sequenceLength
                End If

                If endSearch < beginSearch Then
                    'should never occur
                    endSearch = beginSearch
                End If

                Dim text As String = nt.ReadSegment(beginSearch, endSearch - beginSearch)
                Dim pattern As String = nt.ReadSegment(j, profile.k)

                'if pattern is found, add it to candidate list and scan right for additional similarly spaced repeats
                Dim patternInTextIndex As Integer = SearchTool.BoyerMooreSearch(text, pattern)

                If patternInTextIndex >= 0 Then
                    CandidateCRISPR.AddRepeatData(j)
                    CandidateCRISPR.AddRepeatData(beginSearch + patternInTextIndex)
                    CandidateCRISPR = KMer.ScanRight(nt, CandidateCRISPR, pattern, profile.minS, 24, SearchTool)
                End If

                If (CandidateCRISPR.NumberOfRepeats() >= MinNumberOfRepeats) Then

                    CandidateCRISPR = KMer.GetActualRepeatLength(nt, CandidateCRISPR, profile, p)    'make sure minNumRepeats is always at least 2

                    Dim ActualRepeatLength As Integer = CandidateCRISPR.RepeatLength()

                    If (ActualRepeatLength >= profile.minR) AndAlso (ActualRepeatLength <= profile.maxR) Then
                        If KMer.HasNonRepeatingSpacers(CandidateCRISPR, spacerToSpacerMaxSimilarity) Then
                            If KMer.HasSimilarlySizedSpacers(CandidateCRISPR, spacerToSpacerLengthDiff, spacerToRepeatLengthDiff) Then
                                CandidateCRISPR = KMer.CheckLeftFlank(nt, CandidateCRISPR, profile.minS, 30, spacerToSpacerMaxSimilarity, 0.7)
                                CandidateCRISPR = KMer.CheckRightFlank(nt, CandidateCRISPR, profile.minS, 30, spacerToSpacerMaxSimilarity, 0.7)
                                CandidateCRISPR = KMer.Trim(CandidateCRISPR, profile.minR)

                                Call CRISPRVector.Add(CandidateCRISPR)
                                repeatsFound = True

                                'we may skip current CRISPR (assuming CRISPRs are not interleaved)
                                j = CandidateCRISPR.[End]() + 1

                            End If
                        End If
                    End If
                End If

                j = j + skips
            End While

            Call Console.WriteLine("[Done!]")

            Return CRISPRVector.ToArray
        End Function
    End Module
End Namespace
