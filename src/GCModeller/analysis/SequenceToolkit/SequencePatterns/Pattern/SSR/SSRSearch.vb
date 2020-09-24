#Region "Microsoft.VisualBasic::a2b50e8950af39ffe721ed636b327e19, analysis\SequenceToolkit\SequencePatterns\Pattern\SSR\SSRSearch.vb"

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

    ' Module SSRSearch
    ' 
    '     Properties: Parallel
    ' 
    '     Function: CompoundSSR, InterruptedSSR, PureSSR, SeedingInternal, SSR
    ' 
    '     Sub: MatchInternal
    ' 
    ' Structure SSR
    ' 
    '     Properties: Ends, RepeatUnit, Sequence, Start, Strand
    ' 
    '     Function: ToFasta, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
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

        'For Each x In CompoundSSR(nt, range)
        '    Yield x
        'Next

        'For Each x In InterruptedSSR(nt, range)
        '    Yield x
        'Next
    End Function

    Public Property Parallel As Boolean = False

    ReadOnly ATGC As Char() = "ATGC".ToArray

    Private Function SeedingInternal(seedRange As IntRange) As List(Of String)
        Dim seeds As String() = Seeding.InitializeSeeds(ATGC, seedRange.Min)
        Dim repeatUnit As New List(Of String)(seeds)

        For i As Integer = seedRange.Min + 1 To seedRange.Max
            seeds = Seeding.ExtendSequence(seeds, ATGC).ToArray
            repeatUnit += seeds
        Next

        Return repeatUnit
    End Function

    ''' <summary>
    ''' 所谓单纯SSR 是指由单一的重复单元所组成的序列，如``(AT)n``；
    ''' </summary>
    ''' <param name="range">重复片段的长度范围，默认是最短2个核苷酸，最长6个核苷酸</param>
    ''' <returns></returns>
    Public Function PureSSR(nt As ISequenceModel, Optional range$ = "2,6", Optional minRepeats% = 3) As SSR()
        Dim repeatUnit = SeedingInternal(range) _
            .Where(Function(s) s <> New String(s.First, s.Length)) _
            .ToArray

        ' 假若短的种子不存在重复片段的话，延长一个碱基或许会有了
        ' 例如下面的重复单位为ATC，很显然AT是不可能会出现重复的，但是将种子AT延长一个碱基之后就出现重复了
        ' ATC ATC ATC ATC
        Dim SearchInternal =
            Function(strand$, seq$) As List(Of SSR)
                Dim SSR As New List(Of SSR)
                Dim searchWork = Sub(report As Action(Of String))
                                     For Each unit As String In repeatUnit
                                         Dim pattern$ = $"({unit}){{{minRepeats},}}"

                                         seq.MatchInternal(pattern, SSR, unit, strand, NameOf(PureSSR))
                                         Call report(unit)
                                     Next
                                 End Sub

                If Parallel Then
                    Call searchWork(Sub()
                                        ' Do Nothing
                                    End Sub)
                Else
                    Using progress As New ProgressBar($"Search for Pure SSR on {strand} strand...", 1, CLS:=True)
                        Dim tick As New ProgressProvider(progress, repeatUnit.Count)
                        Dim ETA$
                        Dim msg$
                        Dim work = Sub(unit$)
                                       ETA = tick.ETA().FormatTime
                                       msg = $"{unit}...  ETA: {ETA}"

                                       Call progress.SetProgress(tick.StepProgress, msg)
                                   End Sub

                        Call searchWork(report:=work)
                    End Using
                End If

                Return SSR
            End Function

        Dim sequence$ = nt.SequenceData.ToUpper

        If Not Parallel Then
            Return SearchInternal("+", sequence).AsEnumerable +
                   SearchInternal("-", NucleicAcid.Complement(sequence).Reverse.CharString)
        Else
            Dim SSR As New List(Of SSR)
            Dim forwards = ApplicationServices.TaskRun(Sub()
                                                           SSR = SearchInternal("+", sequence)
                                                       End Sub)
            Dim reverse = SearchInternal("-", NucleicAcid.Complement(sequence).Reverse.CharString)

            Call forwards.GetValue()
            Return SSR.AsEnumerable + reverse
        End If
    End Function

    <Extension>
    Private Sub MatchInternal(seq$, pattern$, out As List(Of SSR), unit$, strand$, <CallerMemberName> Optional type$ = Nothing)
        Dim matches = r.Matches(seq, pattern, RegexICSng)

        For Each match As Match In matches
            If Not match.Success Then
                Continue For
            End If

            For Each instance As Capture In match.Captures
                out += New SSR With {
                    .Start = instance.Index + 1,
                    .Ends = instance.Length + .Start,
                    .Sequence = instance.Value,
                    .Strand = strand,
                    .RepeatUnit = unit ' , .Type = type
                }
            Next
        Next
    End Sub

    ''' <summary>
    ''' 复合SSR 则是由2个或多个重复单元组成的序列，如``(GT)n(AT)m``；
    ''' </summary>
    ''' <returns></returns>
    Public Function CompoundSSR(nt As ISequenceModel, Optional range$ = "2,6", Optional minRepeats% = 3) As SSR()
        Dim seq$ = nt.SequenceData.ToUpper
        Dim repeatUnit = SeedingInternal(range) _
            .Where(Function(s) InStr(seq, s, CompareMethod.Binary) > 0) _
            .ToArray  ' 因为在下面是采用排列组合的方式进行种子的生成，所以在这里需要去除掉不存在的种子以减少数据量
        Dim SSR As New List(Of SSR)

        Dim SearchInternal =
           Sub(strand$)
               Using progress As New ProgressBar($"Search for Compound SSR on {strand} strand...", 1, CLS:=True)
                   Dim tick As New ProgressProvider(progress, repeatUnit.Count)
                   Dim ETA$
                   Dim msg$

                   For Each a$ In repeatUnit
                       For Each b$ In repeatUnit.Where(Function(s) s <> a) ' 如果相等的话就是pureSSR了，在其他的函数中已经搜索过了，就不需要再搜索了
                           Dim pattern$ = $"(({a}){{2,}}({b}){{2,}}){{{minRepeats},}}"

                           seq.MatchInternal(pattern, SSR, a & b, strand, NameOf(CompoundSSR))
                       Next

                       ETA = tick.ETA().FormatTime
                       msg = $"{a}...  ETA: {ETA}"

                       Call progress.SetProgress(tick.StepProgress, msg)
                   Next
               End Using
           End Sub

        seq = nt.SequenceData.ToUpper
        Call SearchInternal("+")
        seq = NucleicAcid.Complement(seq).Reverse.CharString
        Call SearchInternal("-")

        Return SSR
    End Function

    ''' <summary>
    ''' 间隔SSR 在重复序列中有其它核苷酸夹杂其中，如(GT)nGG(GT)m。
    ''' </summary>
    ''' <returns></returns>
    Public Function InterruptedSSR(nt As ISequenceModel, Optional range$ = "2,6") As SSR()
        Dim SSR As New List(Of SSR)

        Return SSR
    End Function

End Module

Public Structure SSR

    ' Public Property Type As String
    Public Property Sequence As String
    Public Property Start As Integer
    Public Property Ends As Integer
    Public Property Strand As String
    Public Property RepeatUnit As String

    Public Function ToFasta() As FastaSeq
        Throw New NotImplementedException
    End Function

    Public Overrides Function ToString() As String
        Return $"[{Start}, {Ends}] {Sequence}"
    End Function

End Structure
