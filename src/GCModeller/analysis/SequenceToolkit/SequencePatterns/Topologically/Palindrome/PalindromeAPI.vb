#Region "Microsoft.VisualBasic::4c3d1594355426093347337dace324b7, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Palindrome\PalindromeAPI.vb"

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

    '   Total Lines: 421
    '    Code Lines: 258
    ' Comment Lines: 116
    '   Blank Lines: 47
    '     File Size: 20.26 KB


    '     Module Palindrome
    ' 
    '         Function: __haveMirror, CreatePalindrome, (+2 Overloads) Density, FindMirrorPalindromes, Found
    '                   HaveMirror, HavePalindrome, (+2 Overloads) ImperfectPalindromeVector, PalindromeLociVector, SaveResultSet
    '                   SearchHairpinks, SearchMirrorPalindrome, SearchPalindrome, ToVector
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Search
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SimilarityMatches
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically

    ''' <summary>
    ''' === Palindromic hexamers ===
    ''' For a given sequence, any palindrome of 6 nt (e.g., AAATTT) Is given a value of 1, while 
    ''' all bases Not included inpalindromic hexamers are given a value of 0 (van et al. 2003).
    ''' -- van Noort V, Worning P, Ussery DW, Rosche WA, Sinden RR Strand misalignments lead To 
    '''    quasipalindrome correction (2003) 19:365-9
    ''' <see cref="SearchMirrorPalindrome"/> (镜像回文序列)
    ''' 
    ''' 
    ''' === Inverted Repeats ===
    ''' Local Inverted repeats are found by taking a 100 bp sequence window, And looking For the 
    ''' best match Of a 30 bp piece withinthat window, On the opposite strand, In the opposite 
    ''' direction (Jensen et al. 1999). 
    ''' Values can range from 0 (no match at all)To 1 (one Or more perfect match within the window).
    ''' -- L. J. Jensen And C. Friis And D.W. Ussery Three views of complete chromosomes (1999) 150773-777
    ''' <see cref="InvokeSearchReversed"/> (反向重复)
    ''' 
    ''' 
    ''' === Quasi-palindromes ===
    ''' "Quasi-palindromes" are short inverted repeats, which are found by taking a 30 bp piece of sequence, 
    ''' And looking for matcheswith at least 6 out of 7 nt matching, on the opposite strand, in the opposite 
    ''' direction (van et al. 2003). Values canrange from 0 (no match at all) to 1 (one Or more perfect 
    ''' match within the window).
    ''' -- van Noort V, Worning P, Ussery DW, Rosche WA, Sinden RR Strand misalignments lead 
    '''    To quasipalindrome correction (2003) 19:365-9
    ''' <see cref="Topologically.Imperfect"/> (非完全回文)
    ''' 
    ''' 
    ''' === Perfect-palindromes ===
    ''' "Perfect-palindromes" are short inverted repeats, which are found by taking a 30 bp piece of sequence, 
    ''' And looking forperfect matches of 7 nt Or longer, on the opposite strand, in the opposite direction (van et al. 2003). 
    ''' Values can rangefrom 0 (no match at all) to 1 (one Or more perfect match within the window).
    ''' -- van Noort V, Worning P, Ussery DW, Rosche WA, Sinden RR Strand misalignments lead To 
    '''    quasipalindrome correction (2003) 19:365-9
    ''' <see cref="SearchPalindrome"/>  (简单回文)
    ''' 
    ''' === Simple Repeats ===
    ''' A "simple repeat" Is a region which contains a simple oligonucleotide repeat, Like microsattelites. 
    ''' Simple repeats are foundby looking for tandem repeats of length R within a 2R-bp window. 
    ''' By using the values 12, 14, 15, 16, And 18 for R, allsimple repeats of lengths 1 through 9 are calculated, 
    ''' of length of at least 24 bp (Jensen et al. 1999). Values can range from 0(no match at all) to 1 
    ''' (one Or more perfect match within the window).
    ''' -- L. J. Jensen And C. Friis And D.W. Ussery Three views of complete chromosomes (1999) 150773-777
    ''' <see cref="SearchRepeats"/> (简单重复序列)
    ''' 
    ''' === GC Skew ===
    ''' For many genomes there Is a strand bias, such that one strand tends To have more G's, 
    ''' whilst the other strand has more C's.This GC-skew bias can be measured the number of G's 
    ''' minus the number of C's over a fixed length (e.g. 10,000 bp) of DNA(Jensen et al. 1999). 
    ''' The values can range from +1 (all G's on the examined sequence, with all C's on the other strand), 
    ''' to -1(the reverse case - all C's on the examined sequence, and all G's on the other strand). 
    ''' There is a correlation with GC-skewand the replication leading and lagging strands.
    ''' -- L. J. Jensen And C. Friis And D.W. Ussery Three views of complete chromosomes (1999) 150773-777
    ''' 
    ''' === Percent AT ===
    ''' The percent AT Is a running average Of the AT content, over a given window size. Typically For a bacterial 
    ''' genomes Of about5 Mbp, the window size Is 10,000 bp. The Percent AT can range from 0 (no AT content) To 1 (100% AT). 
    ''' The Percent AT iscorrelated With other DNA structural features, such that AT rich regions are often more readily 
    ''' melted, tend To be lessflexible And more rigid, although they can also be readily compacted chromatin proteins (Pedersen et al. 2000).
    ''' -- A.G. Pedersen And L.J. Jensen And H.H. St\aerfeldt And S. Brunak And D.W. 
    '''    Ussery A DNA structural atlas of extitE. coli (2000) 299907-930
    ''' </summary>
    <Package("Palindrome.Search", Publisher:="xie.guigang@gcmodeller.org", Url:="http://gcmodeller.org")>
    Public Module Palindrome

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="fa"></param>
        ''' <param name="min"></param>
        ''' <param name="max"></param>
        ''' <param name="cutoff"></param>
        ''' <param name="maxDist"></param>
        ''' <param name="minMatch">至少需要连续匹配这么多个碱基</param>
        ''' <returns></returns>
        <Extension>
        Public Function SearchHairpinks(fa As FastaSeq,
                                        Optional min As Integer = 6,
                                        Optional max As Integer = 8,
                                        Optional cutoff As Integer = 3,
                                        Optional maxDist As Integer = 35,
                                        Optional minMatch As Integer = 3) As ImperfectPalindrome()
            Dim search As New TextIndexing(fa.SequenceData, min, max)

            Return LinqAPI.Exec(Of ImperfectPalindrome) <=
 _
                From segment As TextSegment
                In search.cache.AsParallel
                Let result As ImperfectPalindrome =
                    Found(fa, segment, cutoff, search, maxDist, max)
                Where Not result Is Nothing AndAlso
                    result.MaxMatch >= minMatch
                Select result

        End Function

        Private Function Found(inFasta As FastaSeq,
                               segment As TextSegment,
                               cutoff As Integer,
                               search As TextIndexing,
                               maxDist As Integer,
                               max As Integer) As ImperfectPalindrome

            If Regex.Match(segment.Segment, "[-]+").Value.Equals(segment.Segment) Then
                Return Nothing
            End If

            Dim palin As String = PalindromeLoci.GetPalindrome(segment.Segment)  ' 当前片段所计算出来的完全匹配的回文位点
            Dim start As Integer = segment.Index + segment.Array.Length + maxDist * 0.95
            Dim parPiece As String = Mid(inFasta.SequenceData, start, max + 5)  ' 实际的位点
            Dim dist = DynamicProgramming.Levenshtein.ComputeDistance(palin, parPiece)

            If dist Is Nothing Then Return Nothing

            Dim maxMatch As Integer = search.IsMatch(dist.DistEdits, cutoff)

            If maxMatch <= 0 Then Return Nothing

            Dim result As New ImperfectPalindrome With {
                .Site = segment.Segment,
                .Left = segment.Index,
                .Palindrome = parPiece,
                .Paloci = start,
                .Distance = dist.Distance,
                .Evolr = dist.DistEdits,
                .Matches = dist.Matches,
                .Score = dist.Score,
                .MaxMatch = maxMatch
            }
            Return result
        End Function

        <ExportAPI("Palindrome.Vector")>
        Public Function PalindromeLociVector(DIR As String, Length As Integer) As Double()
            Return Density(Of PalindromeLoci)(DIR, size:=Length)
        End Function

        <ExportAPI("ImperfectPalindrome.Vector")>
        Public Function ImperfectPalindromeVector(DIR As String, length As Integer) As Double()
            Return Density(Of ImperfectPalindrome)(DIR, size:=length)
        End Function

        <ExportAPI("ImperfectPalindrome.Vector.TRIM")>
        Public Function ImperfectPalindromeVector(DIR As String, length As Integer, min As Integer, max As Integer) As Double()
            Call $"Start loading original data from {DIR}".__DEBUG_ECHO
            Dim files = (From file As String
                         In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                         Select file.LoadCsv(Of ImperfectPalindrome)).ToArray
            Call $"Data load done! Start to filter data...".__DEBUG_ECHO
            files = (From genome
                     In files.AsParallel
                     Select From site As ImperfectPalindrome
                             In genome
                            Where site.MaxMatch >= min AndAlso
                                 site.MaxMatch <= max AndAlso
                                 site.Palindrome.Count("-"c) <> site.Palindrome.Length AndAlso
                                 site.Site.Count("-"c) <> site.Site.Length
                            Select site).ToArray
            Call $"Generates density vector....".__DEBUG_ECHO
            Return Density(Of ImperfectPalindrome)(files, size:=length)
        End Function

        Public Function ToVector(Of TSite As Contig)(sites As IEnumerable(Of TSite), size As Integer) As Integer()
            Dim LQuery = (From i As Integer
                          In size.Sequence
                          Select (From site As TSite
                                  In sites
                                  Where site.MappingLocation.IsInside(i)
                                  Select 1).FirstOrDefault).ToArray
            Call Console.Write(".")
            Return LQuery
        End Function

        Public Function Density(Of TView As Contig)(DIR As String, size As Integer) As Double()
            Dim files = (From file As String
                         In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                         Select file.LoadCsv(Of TView)).ToArray
            Return Density(files, size)
        End Function

        Public Function Density(Of TView As Contig)(genomes As IEnumerable(Of IEnumerable(Of TView)), size As Integer) As Double()
            Dim Vecotrs = (From genome As IEnumerable(Of TView)
                           In genomes.AsParallel
                           Select vector = ToVector(genome, size)).ToArray

            Call New String("="c, 120).__DEBUG_ECHO
            Call $"genomes={Vecotrs.Count}".__DEBUG_ECHO

            Dim p_vectors As Double() = size.Sequence.Select(Function(index As Integer) As Double
                                                                 Dim site As Integer() = Vecotrs.Select(Function(genome) genome(index)).ToArray
                                                                 Dim hashRepeats = (From g As Double In site.AsParallel Where g > 0 Select g).ToArray
                                                                 Dim pHas As Double = hashRepeats.Length / site.Length
                                                                 Return pHas
                                                             End Function).ToArray
            Return p_vectors
        End Function

        ''' <summary>
        ''' Have mirror repeats?
        ''' </summary>
        ''' <param name="Segment"></param>
        ''' <param name="Sequence"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("HasMirror?")>
        Public Function HaveMirror(Segment As String, Sequence As String) As Boolean
            Dim Locations = IScanner.FindLocation(Sequence, Segment).ToArray

            If Locations.IsNullOrEmpty Then
                Return False
            End If

            Dim Mirror As String = New String(Segment.Reverse.ToArray)
            Dim l As Integer = Len(Segment)
            Dim Result = (From loci As Integer
                          In Locations
                          Let ml As Integer = __haveMirror(l, loci, Mirror, Sequence)
                          Where ml > -1
                          Select ml).ToArray

            Return Not Result.IsNullOrEmpty
        End Function

        ''' <summary>
        ''' 因为是mirror的关系，所以左边和右边的长度是一样的，所以返回的右边的位置为
        ''' <paramref name="Loci"/>+2*<paramref name="l"/>，
        ''' 即 ``mirror_start + <paramref name="l"/>``
        ''' </summary>
        ''' <param name="l"></param>
        ''' <param name="Loci"></param>
        ''' <param name="mirror"></param>
        ''' <param name="Sequence"></param>
        ''' <returns></returns>
        Private Function __haveMirror(l As Integer, Loci As Integer, mirror As String, Sequence As String) As Integer
            Dim mrStart As Integer = Loci + l
            Dim mMirr As String = Mid(Sequence, mrStart, l)

            If String.Equals(mMirr, mirror) Then
                Return mrStart + l
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' 这个函数求解的是绝对相等的
        ''' </summary>
        ''' <param name="seed"></param>
        ''' <param name="sequence"></param>
        ''' <returns></returns>
        <ExportAPI("Mirrors.Locis.Get")>
        Public Function FindMirrorPalindromes(seed As String, sequence As String) As PalindromeLoci()
            Dim locis As Integer() = IScanner.FindLocation(sequence, seed).ToArray

            If locis.IsNullOrEmpty Then
                Return Nothing
            End If

            ' 得到种子位点的反向序列，并查找出该位点后面的序列是否和这个反向序列相等，即可判断是否为镜像回文位点
            Dim mirror As New String(seed.Reverse.ToArray)
            Dim l As Integer = Len(seed)
            Dim out As PalindromeLoci() = LinqAPI.Exec(Of PalindromeLoci) <=
                From loci As Integer
                In locis
                Let ml As Integer = __haveMirror(l, loci, mirror, sequence)
                Where ml > -1
                Select New PalindromeLoci With {
                    .Loci = seed,
                    .Start = loci,
                    .PalEnd = ml,
                    .Palindrome = mirror,
                    .MirrorSite = mirror
                }

            Return out
        End Function

        ''' <summary>
        ''' 回文序列，即在互补链上面找到了自己的反向序列，这个函数找不到位点的话会返回空集合
        ''' </summary>
        ''' <param name="seed">The seed segment</param>
        ''' <param name="sequence"></param>
        ''' <returns></returns>
        <ExportAPI("Palindrome.Locis.Get")>
        <Extension>
        Public Function CreatePalindrome(seed$, sequence As String) As PalindromeLoci()
            Dim locis%() = IScanner.FindLocation(sequence, seed).ToArray

            If locis.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim reversed As New String(seed.Reverse.ToArray)         ' 对种子序列进行反向，这个是在互补链需要进行查找的序列，
            Dim mirror As String = NucleicAcid.Complement(reversed)  ' 但是为了提高性能，所以在这里对这个互补链上面的序列的互补列进行查找
            Dim l As Integer = Len(seed)
            Dim result = (From loci As Integer
                          In locis
                          Let ml As Integer = __haveMirror(l, loci, mirror, sequence) ' 只需要判断当前的链相对应的位置上面是否含有目标反向互补位点的序列即可
                          Where ml > -1
                          Select loci, ml).ToArray

            Return result _
                .Select(Function(site)
                            Return New PalindromeLoci With {
                                .Loci = seed,
                                .Start = site.loci,
                                .PalEnd = site.ml - 1,
                                .Palindrome = mirror,
                                .MirrorSite = reversed
                            }
                        End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' 搜索序列上面的镜像回文片段
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="min"></param>
        ''' <param name="max"></param>
        ''' <returns></returns>
        <ExportAPI("Search.Mirror")>
        <Extension>
        Public Function SearchMirrorPalindrome(seq As IPolymerSequenceModel,
                                               Optional min% = 3,
                                               Optional max% = 20,
                                               Optional tag$ = Nothing) As PalindromeLoci()
            Dim out As PalindromeLoci()

            With New MirrorPalindrome(seq, min, max)
                .DoSearch()
                out = .ResultSet
            End With

            If Not tag.StringEmpty Then
                For Each loci As PalindromeLoci In out
                    loci.Data = New Dictionary(Of String, String) From {
                        {NameOf(tag), tag}
                    }
                Next
            End If

            Return out
        End Function

        <ExportAPI("Search.Palindrome")>
        <Extension>
        Public Function SearchPalindrome(Sequence As IPolymerSequenceModel,
                                         Optional Min As Integer = 3,
                                         Optional Max As Integer = 20,
                                         Optional tag$ = Nothing) As PalindromeLoci()

            Dim out As PalindromeLoci()

            With New PalindromeSearch(Sequence, Min, Max)
                .DoSearch()
                out = .ResultSet
            End With

            If Not tag.StringEmpty Then
                For Each loci As PalindromeLoci In out
                    loci.Data = New Dictionary(Of String, String) From {
                        {NameOf(tag), tag}
                    }
                Next
            End If

            Return out
        End Function

        <ExportAPI("Write.Csv.PalindromeLocis")>
        Public Function SaveResultSet(rs As Generic.IEnumerable(Of PalindromeSearch), SaveTo As String) As Boolean
            Return rs.SaveTo(SaveTo)
        End Function

        ''' <summary>
        ''' Have Palindrome repeats?
        ''' </summary>
        ''' <param name="Segment"></param>
        ''' <param name="Sequence"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("HasPalindrome?")>
        Public Function HavePalindrome(Segment As String, Sequence As String) As Boolean
            Dim Locations As Integer() = IScanner.FindLocation(Sequence, Segment).ToArray

            If Locations.IsNullOrEmpty Then
                Return False
            End If

            Dim Mirror As String =
                NucleicAcid.Complement(New String(Segment.Reverse.ToArray))
            Dim l As Integer = Len(Segment)
            Dim Result As Integer() =
                LinqAPI.Exec(Of Integer) <= From loci As Integer
                                            In Locations
                                            Let ml As Integer =
                                                __haveMirror(l, loci, Mirror, Sequence)
                                            Where ml > -1
                                            Select ml
            Return Not Result.IsNullOrEmpty
        End Function
    End Module
End Namespace
