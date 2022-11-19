#Region "Microsoft.VisualBasic::c3dc596d58797e9b5b55ab6635d3b0eb, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\Repeats\Analysis.vb"

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

    '   Total Lines: 117
    '    Code Lines: 84
    ' Comment Lines: 17
    '   Blank Lines: 16
    '     File Size: 6.30 KB


    '     Module Analysis
    ' 
    '         Function: Density, (+2 Overloads) RepeatsDensity, RevRepeatsDensity
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Topologically

    Public Module Analysis

        ''' <summary>
        ''' 通过计算每一个基因组上面的每一个位点的重复片段的出现频率来计算出每一个位点的重复片段的热度
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="Mla">必须是经过多序列比对对齐的</param>
        ''' 
        <ExportAPI("Repeats.Density")>
        Public Function RepeatsDensity(Mla As FastaFile,
                                       Min As Integer,
                                       Max As Integer,
                                       Optional MinAppeared As Integer = 2) As KeyValuePair(Of Double(), Double())
            Dim LQuery = (From genome As FastaSeq
                          In Mla
                          Select repeats = RepeatsSearchAPI.SearchRepeats(genome, Min, Max, MinAppeared),
                              rev = RepeatsSearchAPI.SearchReversedRepeats(genome, Min, Max, MinAppeared)).ToArray
            Dim Vecotrs = (From genome In LQuery
                           Let repeatsViews = RepeatsView.TrimView(Repeats.CreateDocument(genome.repeats)),
                               revViews = ReverseRepeatsView.TrimView(genome.rev)
                           Select repeats = RepeatsView.ToVector(repeatsViews, Mla.First.Length),
                               revRepeats = RepeatsView.ToVector(revViews, Mla.First.Length)).ToArray

            ' 有重复序列的个数的百分比 * 热度的平均值
            Dim p_vectors As Double() =
                Mla.First.Length.Sequence.Select(Function(index As Integer) As Double
                                                     Dim site = Vecotrs.Select(Function(genome) genome.repeats(index)).ToArray
                                                     Dim hashRepeats = (From g In site Where g <> 0R Select g).ToArray
                                                     Dim pHas = hashRepeats.Length / site.Length
                                                     Dim hotAvg = hashRepeats.Average
                                                     Return pHas * hotAvg
                                                 End Function).ToArray
            ' 有重复序列的个数的百分比 * 热度的平均值
            Dim p_rev_vectors As Double() =
                Mla.First.Length.Sequence.Select(Function(index As Integer) As Double
                                                     Dim site = Vecotrs.Select(Function(genome) genome.revRepeats(index)).ToArray
                                                     Dim hashRepeats = (From g In site Where g <> 0R Select g).ToArray
                                                     Dim pHas = hashRepeats.Length / site.Length
                                                     Dim hotAvg = hashRepeats.Average
                                                     Return pHas * hotAvg
                                                 End Function).ToArray
            Return New KeyValuePair(Of Double(), Double())(p_vectors, p_rev_vectors)
        End Function

        <ExportAPI("Repeats.Density")>
        Public Function RepeatsDensity(DIR As String, size As Integer, ref As String, Optional cutoff As Double = 0R) As Double()
            Return Density(Of RepeatsView)(DIR, size, ref, cutoff)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="TView"></typeparam>
        ''' <param name="DIR"></param>
        ''' <param name="size"></param>
        ''' <param name="ref">作为参考的原始数据之中的csv文件名</param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        Public Function Density(Of TView As RepeatsView)(DIR As String, size As Integer, ref As String, cutoff As Double) As Double()
            Dim files = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.csv") _
                .Select(Function(file) New With {
                    .ID = BaseName(file),
                    .context = file.LoadCsv(Of TView)}).ToArray

            VBDebugger.Mute = True

            Dim Vecotrs = (From genome
                           In files.AsParallel
                           Select genome.ID,
                               vector = RepeatsView.ToVector(genome.context, size)) _
                              .ToDictionary(Function(genome) genome.ID,
                                            Function(genome) genome.vector)
            Dim refGenome As Double() = Vecotrs.TryGetValue(ref.NormalizePathString(True))

            VBDebugger.Mute = False

            If refGenome Is Nothing Then
                Call $"Reference `{ref}` is not exists in the dataset, using the first sequence as default!".__DEBUG_ECHO
                refGenome = Vecotrs.First.Value
            End If

            Call New String("="c, 120).__DEBUG_ECHO
            Call $"cutoff={cutoff}".__DEBUG_ECHO
            Call $"genomes={Vecotrs.Count}".__DEBUG_ECHO

            Dim p_vectors As Double() = size _
                .Sequence _
                .Select(Function(index As Integer) As Double
                            Dim refV As Double = refGenome(index)

                            If refV <= cutoff Then
                                Return 0
                            End If

                            Dim site = Vecotrs.Select(Function(genome) genome.Value(index)).ToArray
                            Dim hashRepeats = (From g As Double In site.AsParallel Where g >= cutoff Select g).ToArray
                            Dim pHas As Double = hashRepeats.Length / site.Length
                            Return pHas
                        End Function) _
                .ToArray

            Return p_vectors
        End Function

        <ExportAPI("rev-Repeats.Density")>
        Public Function RevRepeatsDensity(dir As String, size As Integer, ref As String, Optional cutoff As Double = 0R) As Double()
            Return Density(Of ReverseRepeatsView)(dir, size, ref, cutoff)
        End Function
    End Module
End Namespace
