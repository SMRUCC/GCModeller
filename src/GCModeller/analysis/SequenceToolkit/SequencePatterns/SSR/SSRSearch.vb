
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports r = System.Text.RegularExpressions.Regex

''' <summary>
''' 微卫星DNA：重复单位序列最短，只有2～6bp，串联成簇，长度50～100bp，又称为短串联重复序列（Short Tandem Repeat STR)。
''' 广泛分布于基因组中。 其中富含A-T碱基对，是在研究DNA多态性标记过程中发现的。1981年Miesfeld等首次发现微卫星DNA，
''' 其重复单位长度一般为1～6个核苷酸，双核苷酸重复单位常为(CA)n和(TG)n。
''' </summary>
Public Module SSRSearch

    <Extension>
    Public Iterator Function SSR(nt As ISequenceModel, Optional range$ = "2,6") As IEnumerable(Of SSR)
        For Each x In PureSSR(nt, range)
            Yield x
        Next

        For Each x In CompoundSSR(nt, range)
            Yield x
        Next

        For Each x In InterruptedSSR(nt, range)
            Yield x
        Next
    End Function

    ''' <summary>
    ''' 所谓单纯SSR 是指由单一的重复单元所组成的序列，如(AT) n；
    ''' </summary>
    ''' <param name="range">重复片段的长度范围，默认是最短2个核苷酸，最长6个核苷酸</param>
    ''' <returns></returns>
    Public Function PureSSR(nt As ISequenceModel, Optional range$ = "2,6") As SSR()
        Dim seedRange As IntRange = range
        Dim seeds As List(Of String) = Seeding.InitializeSeeds("ATGC", seedRange.Min)
        Dim repeatUnit As New List(Of String)(seeds)

        For i As Integer = seedRange.Min + 1 To seedRange.Max
            seeds = Seeding.ExtendSequence(seeds, "ATGC")
            repeatUnit += seeds.AsEnumerable
        Next

        ' 假若短的种子不存在重复片段的话，延长一个碱基或许会有了
        ' 例如下面的重复单位为ATC，很显然AT是不可能会出现重复的，但是将种子AT延长一个碱基之后就出现重复了
        ' ATC ATC ATC ATC
        Dim SSR As New List(Of SSR)

        For Each unit As String In repeatUnit
            Dim pattern$ = "(" & unit & "){2,}"
            Dim matches = r.Matches(nt.SequenceData, pattern, RegexICSng)

            For Each match As Match In matches
                If Not match.Success Then
                    Continue For
                End If


            Next
        Next

        Return SSR
    End Function

    ''' <summary>
    ''' 复合SSR 则是由2 个或多个重复单元组成的序列，如(GT)n(AT)m；
    ''' </summary>
    ''' <returns></returns>
    Public Function CompoundSSR(nt As ISequenceModel, Optional range$ = "2,6")

    End Function

    ''' <summary>
    ''' 间隔SSR 在重复序列中有其它核苷酸夹杂其中，如(GT)nGG(GT)m。
    ''' </summary>
    ''' <returns></returns>
    Public Function InterruptedSSR(nt As ISequenceModel, Optional range$ = "2,6")

    End Function

End Module

Public Structure SSR

    Public Property Type As String
    Public Property Sequence As String
    Public Property Start As Integer
    Public Property Ends As Integer
    Public Property Strand As String
    Public Property RepeatUnit As String

    Public Function ToFasta() As FastaToken

    End Function

End Structure