#Region "Microsoft.VisualBasic::bc22b9378a326fe41bd6b34672191ae5, analysis\Motifs\CRISPR\CRT\SearchingModel\KMer.vb"

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

    '     Module KMer
    ' 
    '         Function: __sequenceScan, CheckLeftFlank, CheckRightFlank, GetActualRepeatLength, HasNonRepeatingSpacers
    '                   HasSimilarlySizedSpacers, Min, PatternMatches, ScanRight, Trim
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Extensions
Imports stdNum = System.Math

Namespace SearchingModel

    ''' <summary>
    ''' An occurrence of a CRISPR. Repetitive sequences are detected by reading a small search window 
    ''' and then scanning ahead for exact k-mer matches separated by a similar distance.
    ''' 
    ''' (一个可能的CRISPR位点)
    ''' </summary>
    ''' <remarks>
    ''' Given a k-mer that begins at position i, any
    ''' exact k-mer match, if one exists, should occur in the range:
    ''' 
    ''' (在任何一个k-mer候选位点之中，假若存在目标位点的话，这个位点应该会出现在下面所示的搜索范围之内)
    ''' 
    '''      [i + minR + minS .. i + maxR + maxS + k]
    ''' 
    ''' Here, minR and maxR refer to the lengths of the smallest
    ''' and largest repeats to be detected.
    ''' 
    ''' The lengths of spacers, which are the similarly sized non-repeating regions
    ''' between repeats, are referred to by minS and maxS. 
    ''' 
    ''' Since CRISPRs are to some degree evenly spaced, the distance between the initial repeats can be
    ''' used to approximate the spacing between subsequent
    ''' exact k-mer matches. Thus the size of the search range can
    ''' be reduced further, resulting in faster processing time.
    ''' The size of the search range has a direct effect on the
    ''' processing time of the algorithm, with smaller ranges
    ''' being more desirable. Thus, the algorithm runs fastest
    ''' when there is little variation between the sizes of the
    ''' smallest/largest repeats and the smallest/largest spacers.
    ''' 
    ''' <see cref="KmerProfile.minR"></see>和<see cref="KmerProfile.maxR"></see>分别为所检测到的重复位点的最小的和最大的范围
    ''' <see cref="KmerProfile.minS"></see>和<see cref="KmerProfile.maxS"></see>间隔的长度的最大值和最小值：间隔长度为非重复序列片段区域的长度
    ''' 
    ''' 
    ''' Reference:
    ''' 
    ''' Jansen, R., et al. (2002). "Identification of genes that are associated with DNA repeats in prokaryotes." Molecular Microbiology 43(6): 1565-1575.
    '''	Using in silico analysis we studied a novel family of repetitive DNA sequences that is present among both domains of the prokaryotes (Archaea and Bacteria), but absent from eukaryotes or viruses. This family is characterized by direct repeats, varying in size from 21 to 37 bp, interspaced by similarly sized nonrepetitive sequences. To appreciate their characteristic structure, we will refer to this family as the clustered regularly interspaced short palindromic repeats (CRISPR). In most species with two or more CRISPR loci, these loci were flanked on one side by a common leader sequence of 300-500 b. The direct repeats and the leader sequences were conserved within a species, but dissimilar between species. The presence of multiple chromosomal CRISPR loci suggests that CRISPRs are mobile elements. Four CRISPR-associated (cas) genes were identified in CRISPR-containing prokaryotes that were absent from CRISPR-negative prokaryotes. The cas genes were invariably located adjacent to a CRISPR locus, indicating that the cas genes and CRISPR loci have a functional relationship. The cas3 gene showed motifs characteristic for helicases of the superfamily 2, and the cas4 gene showed motifs of the RecB family of exonucleases, suggesting that these genes are involved in DNA metabolism or gene expression. The spatial coherence of CRISPR and cas genes may stimulate new research on the genesis and biological role of these repeats and genes.
    '''
    '''
    ''' </remarks>
    Public Module KMer

        ''' <summary>
        ''' identified repeats may represent only a subset of a larger repeat.  this method extends these
        ''' repeats as long as they continue to match within some range.  assumes there are at least two repeats
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="candidateCRISPR"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetActualRepeatLength(nt As NucleicAcid, CandidateCRISPR As CRISPR, profile As KmerProfile, threshold As Double) As CRISPR
            Dim numRepeats As Integer = CandidateCRISPR.NumberOfRepeats()
            Dim firstRepeatStartIndex As Integer = CandidateCRISPR.RepeatAt(0)
            Dim lastRepeatStartIndex As Integer = CandidateCRISPR.RepeatAt(numRepeats - 1)

            Dim shortestRepeatSpacing As Integer = CandidateCRISPR.RepeatAt(1) - CandidateCRISPR.RepeatAt(0)
            For i As Integer = 0 To CandidateCRISPR.NumberOfRepeats() - 2
                Dim currRepeatIndex As Integer = CandidateCRISPR.RepeatAt(i)
                Dim nextRepeatIndex As Integer = CandidateCRISPR.RepeatAt(i + 1)
                Dim currRepeatSpacing As Integer = nextRepeatIndex - currRepeatIndex
                If currRepeatSpacing < shortestRepeatSpacing Then
                    shortestRepeatSpacing = currRepeatSpacing
                End If
            Next

            Dim sequenceLength As Integer = nt.Length
            Dim rightExtensionLength As Integer = profile.k
            Dim maxRightExtensionLength As Integer = shortestRepeatSpacing - profile.minS
            Dim currRepeatStartIndex As Integer
            Dim currRepeat As String
            Dim charCountA As Integer, charCountC As Integer, charCountT As Integer, charCountG As Integer
            Dim done As Boolean = False

            '(from the right side) extend the length of the repeat to the right as long as the last base of all repeats are at least threshold
            While Not done AndAlso (rightExtensionLength <= maxRightExtensionLength) AndAlso (lastRepeatStartIndex + rightExtensionLength < sequenceLength)
                For k As Integer = 0 To CandidateCRISPR.NumberOfRepeats() - 1
                    currRepeatStartIndex = CandidateCRISPR.RepeatAt(k)
                    currRepeat = nt.ReadSegment(currRepeatStartIndex, rightExtensionLength)
                    Dim lastChar As Char = currRepeat(currRepeat.Length - 1)

                    If lastChar = "A"c Then
                        charCountA += 1
                    End If
                    If lastChar = "C"c Then
                        charCountC += 1
                    End If
                    If lastChar = "T"c Then
                        charCountT += 1
                    End If
                    If lastChar = "G"c Then
                        charCountG += 1
                    End If
                Next

                Dim percentA As Double = CDbl(charCountA) / CandidateCRISPR.NumberOfRepeats()
                Dim percentC As Double = CDbl(charCountC) / CandidateCRISPR.NumberOfRepeats()
                Dim percentT As Double = CDbl(charCountT) / CandidateCRISPR.NumberOfRepeats()
                Dim percentG As Double = CDbl(charCountG) / CandidateCRISPR.NumberOfRepeats()

                If (percentA >= threshold) OrElse (percentC >= threshold) OrElse (percentT >= threshold) OrElse (percentG >= threshold) Then
                    rightExtensionLength += 1
                    charCountC = 0
                    charCountT = 0
                    charCountG = 0
                    charCountA = 0
                Else
                    done = True
                End If
            End While
            rightExtensionLength -= 1

            Dim leftExtensionLength As Integer = 0
            charCountC = 0
            charCountT = 0
            charCountG = 0
            charCountA = 0
            done = False

            Dim maxLeftExtensionLength As Integer = shortestRepeatSpacing - profile.minS - rightExtensionLength

            '(from the left side) extends the length of the repeat to the left as long as the first base of all repeats is at least threshold
            While Not done AndAlso (leftExtensionLength <= maxLeftExtensionLength) AndAlso (firstRepeatStartIndex - leftExtensionLength >= 0)
                For k As Integer = 0 To CandidateCRISPR.NumberOfRepeats() - 1
                    currRepeatStartIndex = CandidateCRISPR.RepeatAt(k)
                    Dim firstChar As Char = nt.SequenceData(currRepeatStartIndex - leftExtensionLength)

                    If firstChar = "A"c Then
                        charCountA += 1
                    End If
                    If firstChar = "C"c Then
                        charCountC += 1
                    End If
                    If firstChar = "T"c Then
                        charCountT += 1
                    End If
                    If firstChar = "G"c Then
                        charCountG += 1
                    End If
                Next

                Dim percentA As Double = CDbl(charCountA) / CandidateCRISPR.NumberOfRepeats()
                Dim percentC As Double = CDbl(charCountC) / CandidateCRISPR.NumberOfRepeats()
                Dim percentT As Double = CDbl(charCountT) / CandidateCRISPR.NumberOfRepeats()
                Dim percentG As Double = CDbl(charCountG) / CandidateCRISPR.NumberOfRepeats()

                If (percentA >= threshold) OrElse (percentC >= threshold) OrElse (percentT >= threshold) OrElse (percentG >= threshold) Then
                    leftExtensionLength += 1
                    charCountC = 0
                    charCountT = 0
                    charCountG = 0
                    charCountA = 0
                Else
                    done = True
                End If
            End While
            leftExtensionLength -= 1

            Dim NewRepeatsPosition As List(Of Integer) = CandidateCRISPR.Repeats.AsList

            For m As Integer = 0 To NewRepeatsPosition.Count - 1
                Dim newValue As Integer = CandidateCRISPR.RepeatAt(m) - leftExtensionLength
                NewRepeatsPosition(m) = New System.Nullable(Of Integer)(newValue)
            Next

            Dim actualPatternLength As Integer = rightExtensionLength + leftExtensionLength

            Return New CRISPR(nt, NewRepeatsPosition, actualPatternLength)
        End Function

        Public Function Trim(CandidateCRISPR As CRISPR, MinRepeatLength As Integer) As CRISPR
            Dim numRepeats As Integer = CandidateCRISPR.NumberOfRepeats()
            Dim left As Integer = CandidateCRISPR.StartLeft
            Dim right As Integer = CandidateCRISPR.[End]()

            Dim currRepeat As String
            Dim charCountA As Integer, charCountC As Integer, charCountT As Integer, charCountG As Integer
            Dim done As Boolean = False

            'trim from right
            While Not done AndAlso (CandidateCRISPR.RepeatLength() > MinRepeatLength)
                For k As Integer = 0 To CandidateCRISPR.NumberOfRepeats() - 1
                    currRepeat = CandidateCRISPR.RepeatStringAt(k)
                    Dim lastChar As Char = currRepeat(currRepeat.Length - 1)

                    If lastChar = "A"c Then
                        charCountA += 1
                    End If
                    If lastChar = "C"c Then
                        charCountC += 1
                    End If
                    If lastChar = "T"c Then
                        charCountT += 1
                    End If
                    If lastChar = "G"c Then
                        charCountG += 1
                    End If
                Next

                Dim percentA As Double = CDbl(charCountA) / CandidateCRISPR.NumberOfRepeats()
                Dim percentC As Double = CDbl(charCountC) / CandidateCRISPR.NumberOfRepeats()
                Dim percentT As Double = CDbl(charCountT) / CandidateCRISPR.NumberOfRepeats()
                Dim percentG As Double = CDbl(charCountG) / CandidateCRISPR.NumberOfRepeats()

                If (percentA < 0.75) AndAlso (percentC < 0.75) AndAlso (percentT < 0.75) AndAlso (percentG < 0.75) Then
                    CandidateCRISPR.RepeatLength = CandidateCRISPR.RepeatLength() - 1
                    charCountC = 0
                    charCountT = 0
                    charCountG = 0
                    charCountA = 0
                Else
                    done = True
                End If
            End While

            charCountC = 0
            charCountT = 0
            charCountG = 0
            charCountA = 0
            done = False

            'trim from left
            While Not done AndAlso (CandidateCRISPR.RepeatLength() > MinRepeatLength)
                For k As Integer = 0 To CandidateCRISPR.NumberOfRepeats() - 1
                    currRepeat = CandidateCRISPR.RepeatStringAt(k)
                    Dim firstChar As Char = currRepeat(0)

                    If firstChar = "A"c Then
                        charCountA += 1
                    End If
                    If firstChar = "C"c Then
                        charCountC += 1
                    End If
                    If firstChar = "T"c Then
                        charCountT += 1
                    End If
                    If firstChar = "G"c Then
                        charCountG += 1
                    End If
                Next

                Dim percentA As Double = CDbl(charCountA) / CandidateCRISPR.NumberOfRepeats()
                Dim percentC As Double = CDbl(charCountC) / CandidateCRISPR.NumberOfRepeats()
                Dim percentT As Double = CDbl(charCountT) / CandidateCRISPR.NumberOfRepeats()
                Dim percentG As Double = CDbl(charCountG) / CandidateCRISPR.NumberOfRepeats()

                If (percentA < 0.75) AndAlso (percentC < 0.75) AndAlso (percentT < 0.75) AndAlso (percentG < 0.75) Then
                    ' For m As Integer = 0 To CandidateCRISPR.NumberOfRepeats() - 1
                    'Dim newValue As Integer = CandidateCRISPR.RepeatAt(m) + 1
                    'CandidateCRISPR.RepeatAt(newValue)
                    'Next
                    CandidateCRISPR.RepeatLength = CandidateCRISPR.RepeatLength() - 1
                    charCountC = 0
                    charCountT = 0
                    charCountG = 0
                    charCountA = 0
                Else
                    done = True
                End If
            End While

            Return CandidateCRISPR
        End Function

        Public Function HasSimilarlySizedSpacers(CandidateCRISPR As CRISPR, SpacerToSpacerLengthOffset As Integer, spacerToRepeatLengthOffset As Integer) As Boolean
            Dim InitialSpacerLength As Integer = CandidateCRISPR.SpacerStringAt(0).Length
            Dim repeatLength As Integer = CandidateCRISPR.RepeatLength()

            For i As Integer = 0 To CandidateCRISPR.NumberOfSpacers() - 1
                Dim currSpacerLength As Integer = CandidateCRISPR.SpacerStringAt(i).Length

                'checks that each spacer is of similar size to other spacers
                If stdNum.Abs(currSpacerLength - InitialSpacerLength) > SpacerToSpacerLengthOffset Then
                    Return False
                End If

                'checks that each spacer is of similar size to the repeats
                If stdNum.Abs(currSpacerLength - repeatLength) > spacerToRepeatLengthOffset Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' Checks first five spacers
        ''' </summary>
        ''' <param name="candidateCRISPR"></param>
        ''' <param name="maxSimilarity"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasNonRepeatingSpacers(CandidateCRISPR As CRISPR, MaxSimilarity As Double) As Boolean
            Dim FirstRepeat As String = CandidateCRISPR.RepeatStringAt(0)   'assumes at least two elements
            Dim FirstSpace As String = CandidateCRISPR.SpacerStringAt(0)

            If CandidateCRISPR.NumberOfRepeats() >= 3 Then
                Dim i As Integer = 0

                While (i < CandidateCRISPR.NumberOfSpacers() - 1)
                    'only check first 5 spacers
                    If i = 4 Then Return True

                    Dim currSpacer As String = CandidateCRISPR.SpacerStringAt(i)
                    Dim nextSpacer As String = CandidateCRISPR.SpacerStringAt(i + 1)
                    Dim currRepeat As String = CandidateCRISPR.RepeatStringAt(i)

                    'spacers should be different
                    If Similarity(currSpacer, nextSpacer) > MaxSimilarity Then
                        Return False
                    End If

                    'repeats should also be different from spacers, otherwise may be tandem repeat
                    If Similarity(currRepeat, currSpacer) > MaxSimilarity Then
                        Return False
                    End If

                    i += 1
                End While

                'checks last repeat/spacer
                If Similarity(CandidateCRISPR.RepeatStringAt(i), CandidateCRISPR.SpacerStringAt(i)) > MaxSimilarity Then
                    Return False
                End If

                Return True
            ElseIf CandidateCRISPR.NumberOfRepeats() = 2 Then     'we check that the spacer is different from the repeat
                If String.IsNullOrEmpty(FirstSpace) Then
                    Return False
                Else
                    Return Similarity(FirstSpace, FirstRepeat) < MaxSimilarity
                End If
            Else
                Return False
            End If
        End Function

        Public Function CheckLeftFlank(nt As NucleicAcid, candidateCRISPR As CRISPR,
                                       minSpacerLength%,
                                       scanRange%,
                                       spacerToSpacerMaxSimilarity#,
                                       confidence#) As CRISPR

            Dim moreToSearch As Boolean = True

            While moreToSearch
                Dim result As Integer = __sequenceScan(nt, LEFT, candidateCRISPR, minSpacerLength, scanRange, confidence)
                If result > 0 Then
                    candidateCRISPR.InsertRepeatAt(result, 0)
                Else
                    moreToSearch = False
                End If
            End While

            Return candidateCRISPR
        End Function

        Public Function CheckRightFlank(nt As NucleicAcid, candidateCRISPR As CRISPR,
                                        minSpacerLength%,
                                        scanRange%,
                                        spacerToSpacerMaxSimilarity#,
                                        confidence#) As CRISPR

            Dim moreToSearch As Boolean = True

            While moreToSearch
                Dim result As Integer = __sequenceScan(
                    nt,
                    RIGHT,
                    candidateCRISPR,
                    minSpacerLength,
                    scanRange,
                    confidence)

                If result > 0 Then
                    candidateCRISPR.AddRepeatData(result)
                Else
                    moreToSearch = False
                End If
            End While

            Return candidateCRISPR
        End Function

        Const LEFT As Integer = -1
        Const RIGHT As Integer = 1

        ''' <summary>
        ''' scan to the right and left of the first and last repeat to see if there is a region
        '''		that is similar to the repeats.  necessary in case we missed a repeat because of
        '''		inexact matches or a result of one of the filters
        ''' </summary>
        ''' <param name="side"></param>
        ''' <param name="candidateCRISPR"></param>
        ''' <param name="minSpacerLength"></param>
        ''' <param name="scanRange"></param>
        ''' <param name="confidence"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __sequenceScan(SequenceData As NucleicAcid,
                                        side As Integer,
                                        CandidateCRISPR As CRISPR,
                                        minSpacerLength As Integer,
                                        scanRange As Integer,
                                        confidence As Double) As Integer
            Dim repeatSpacing1 As Integer, repeatSpacing2 As Integer, avgRepeatSpacing As Integer
            Dim firstRepeatIndex As Integer, lastRepeatIndex As Integer, candidateRepeatIndex As Integer
            Dim repeatString As String, candidateRepeatString As String, newCandidateRepeatString As String

            Dim repeatLength As Integer = CandidateCRISPR.RepeatLength()
            Dim numRepeats As Integer = CandidateCRISPR.NumberOfRepeats()
            Dim sequenceLength As Integer = SequenceData.Length

            firstRepeatIndex = CandidateCRISPR.RepeatAt(0)
            lastRepeatIndex = CandidateCRISPR.RepeatAt(numRepeats - 1)

            If side = LEFT Then
                repeatString = CandidateCRISPR.RepeatStringAt(0)
                repeatSpacing1 = CandidateCRISPR.RepeatSpacing(0, 1)
                If numRepeats >= 3 Then
                    repeatSpacing2 = CandidateCRISPR.RepeatSpacing(1, 2)
                    avgRepeatSpacing = (repeatSpacing1 + repeatSpacing2) \ 2
                Else
                    avgRepeatSpacing = repeatSpacing1
                End If

                candidateRepeatIndex = firstRepeatIndex - avgRepeatSpacing
            Else
                repeatString = CandidateCRISPR.RepeatStringAt(numRepeats - 1)
                repeatSpacing1 = CandidateCRISPR.RepeatSpacing(numRepeats - 2, numRepeats - 1)
                If numRepeats >= 3 Then
                    repeatSpacing2 = CandidateCRISPR.RepeatSpacing(numRepeats - 3, numRepeats - 2)
                    avgRepeatSpacing = (repeatSpacing1 + repeatSpacing2) \ 2
                Else
                    avgRepeatSpacing = repeatSpacing1
                End If

                candidateRepeatIndex = lastRepeatIndex + avgRepeatSpacing
            End If

            Dim begin As Integer = candidateRepeatIndex - scanRange
            Dim [end] As Integer = candidateRepeatIndex + scanRange

            'check that we do not search too far within an existing repeat when scanning right and left
            Dim scanLeftMaxEnd As Integer = firstRepeatIndex - repeatLength - minSpacerLength
            Dim scanRightMinBegin As Integer = lastRepeatIndex + repeatLength + minSpacerLength

            If side = LEFT Then
                If [end] > scanLeftMaxEnd Then
                    [end] = scanLeftMaxEnd
                End If
            End If

            If side = RIGHT Then
                If begin < scanRightMinBegin Then
                    begin = scanRightMinBegin
                End If
            End If

            'out of bounds check for scanning left
            If (begin) < 0 Then
                Return 0
            End If

            'out of bounds check for scanning right
            If (begin + repeatLength) > sequenceLength Then
                Return 0
            End If
            If ([end] + repeatLength) > sequenceLength Then
                [end] = sequenceLength - repeatLength
            End If

            If begin >= [end] Then
                Return 0
            End If

            Dim array As Integer() = New Integer([end] - begin) {}

            Dim index As Integer = 0
            For i As Integer = begin To [end]
                candidateRepeatString = SequenceData.ReadSegment(i, repeatLength)
                array(index) = HammingDistance(repeatString, candidateRepeatString)
                index += 1
            Next

            'min(array) returns the index of the smallest value in array  in this case, it refers to
            'the candidate string theat is closest to the repeatString.  uses hamming distance as levenshteinDistance is not useful for this particular task
            Dim newCandidateRepeatIndex As Integer = begin + Min(array)
            newCandidateRepeatString = SequenceData.ReadSegment(newCandidateRepeatIndex, repeatLength)

            Dim match As Boolean = PatternMatches(repeatString, newCandidateRepeatString, confidence)

            If match Then
                Return newCandidateRepeatIndex
            Else
                Return 0
            End If
        End Function

        Public Function ScanRight(SequenceData As NucleicAcid,
                                  CandidateCRISPR As CRISPR,
                                  pattern As String,
                                  minSpacerLength As Integer,
                                  scanRange As Integer,
                                  searchUtil As BoyerMooreAlgorithmSearcher) As CRISPR

            Dim numRepeats As Integer = CandidateCRISPR.NumberOfRepeats()
            Dim patternLength As Integer = pattern.Length
            Dim sequenceLength As Integer = SequenceData.Length

            Dim lastRepeatIndex As Integer = CandidateCRISPR.RepeatAt(numRepeats - 1)
            Dim secondToLastRepeatIndex As Integer = CandidateCRISPR.RepeatAt(numRepeats - 2)
            Dim repeatSpacing As Integer = lastRepeatIndex - secondToLastRepeatIndex

            Dim candidateRepeatIndex As Integer, beginSearch As Integer, endSearch As Integer
            Dim moreToSearch As Boolean = True

            While moreToSearch
                candidateRepeatIndex = lastRepeatIndex + repeatSpacing
                beginSearch = candidateRepeatIndex - scanRange
                endSearch = candidateRepeatIndex + patternLength + scanRange

                'check that we do not search too far within an existing repeat when scanning right
                Dim scanRightMinBegin As Integer = lastRepeatIndex + patternLength + minSpacerLength

                If beginSearch < scanRightMinBegin Then
                    beginSearch = scanRightMinBegin
                End If

                If beginSearch > sequenceLength - 1 Then
                    Return CandidateCRISPR
                End If
                If endSearch > sequenceLength Then
                    endSearch = sequenceLength
                End If

                If beginSearch >= endSearch Then
                    Return CandidateCRISPR
                End If

                Dim Text As String = SequenceData.ReadSegment(beginSearch, endSearch - beginSearch)
                Dim Position As Integer = searchUtil.BoyerMooreSearch(Text, pattern)

                If Position >= 0 Then
                    CandidateCRISPR.AddRepeatData(beginSearch + Position)
                    secondToLastRepeatIndex = lastRepeatIndex
                    lastRepeatIndex = beginSearch + Position
                    repeatSpacing = lastRepeatIndex - secondToLastRepeatIndex
                    If repeatSpacing < (minSpacerLength + patternLength) Then
                        moreToSearch = False
                    End If
                Else
                    moreToSearch = False
                End If
            End While

            Return CandidateCRISPR
        End Function

        Private Function PatternMatches(pattern1 As String, pattern2 As String, confidence As Double) As Boolean
            Dim patternSimilarity As Double = Similarity(pattern1, pattern2)
            If patternSimilarity >= confidence Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Function Min(array As Integer()) As Integer
            Dim minValue As Integer = array(0)
            Dim minIndex As Integer = 0

            For i As Integer = 0 To array.Length - 1
                If array(i) < minValue Then
                    minValue = array(i)
                    minIndex = i
                End If
            Next
            Return minIndex
        End Function
    End Module
End Namespace
