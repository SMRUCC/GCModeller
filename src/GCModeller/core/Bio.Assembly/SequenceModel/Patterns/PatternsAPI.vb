#Region "Microsoft.VisualBasic::f1208918a403ee4aecd16ac92c1123d4, GCModeller\core\Bio.Assembly\SequenceModel\Patterns\PatternsAPI.vb"

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

    '   Total Lines: 179
    '    Code Lines: 102
    ' Comment Lines: 60
    '   Blank Lines: 17
    '     File Size: 8.50 KB


    '     Module PatternsAPI
    ' 
    '         Function: __frequency, __variation, (+2 Overloads) Frequency, (+2 Overloads) NTVariations, Variation
    ' 
    '         Sub: Frequency
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SequenceModel.Patterns

    Public Module PatternsAPI

        ''' <summary>
        ''' 简单的统计残基的出现频率
        ''' </summary>
        ''' <param name="Fasta"></param>
        ''' 
        <ExportAPI("NT.Frequency")>
        Public Sub Frequency(<Parameter("Path.Fasta")> Fasta As String, Optional offset As Integer = 0)
            Dim data As PatternModel = Frequency(FastaFile.Read(Fasta))
            Dim doc As New StringBuilder(NameOf(Frequency) & ",")
            Call doc.AppendLine(String.Join(",", DirectCast(data.PWM.First, SimpleSite).Alphabets.Keys.Select(Function(c) CStr(c)).ToArray))
            Call doc.AppendLine(String.Join(vbCrLf, data.PWM.Select(Function(obj, i) $"{i + offset },{String.Join(",", obj.EnumerateValues.Select(Of String)(Function(oo) CStr(oo)).ToArray)}").ToArray))

            Call doc.ToString.SaveTo(Fasta & ".csv")
        End Sub

        ''' <summary>
        ''' 返回来的数据之中的残基的字符是大写的
        ''' </summary>
        ''' <param name="Fasta"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("NT.Frequency")>
        Public Function Frequency(Fasta As FastaFile) As PatternModel
            Return Frequency(source:=Fasta)
        End Function

        ''' <summary>
        ''' Simple function for statics the alphabet frequency in the fasta source. 
        ''' The returns matrix, alphabet key char is Upper case.
        ''' (返回来的数据之中的残基的字符是大写的)
        ''' </summary>
        ''' <param name="source">
        ''' Fasta sequence source, and all of the fasta sequence 
        ''' in this source must in the same length.
        ''' </param>
        ''' <returns></returns>
        <ExportAPI("NT.Frequency")>
        Public Function Frequency(source As IEnumerable(Of FastaSeq)) As PatternModel
            Dim len As Integer = source.First.Length
            Dim n As Integer = source.Count
            Dim alphabets As Char() =
                If(source.First.IsProtSource,
                   Polypeptides.ToChar.Values.ToArray, New Char() {"A"c, "T"c, "G"c, "C"c})

            ' Converts the alphabets in the sequence data to upper case.
            Dim fasta As New FastaFile(source.Select(Function(x) x.ToUpper))
            Dim LQuery = (From pos As Integer
                          In len.Sequence.AsParallel
                          Select pos,
                              row = (From c As Char
                                     In alphabets
                                     Select c, ' Statics for the alphabet frequency at each column
                                         f = __frequency(fasta, pos, c, n)).ToArray
                          Order By pos Ascending).ToArray
            Dim Model As IEnumerable(Of SimpleSite) =
                From x
                In LQuery.SeqIterator
                Let freq As Dictionary(Of Char, Double) = (+x) _
                    .row _
                    .ToDictionary(Function(o0) o0.c,
                                  Function(o0) o0.f)
                Select New SimpleSite(freq, x.i)

            Return New PatternModel(Model)
        End Function

        ''' <summary>
        ''' The conservation percentage (%) Is defined as the number of genomes with the same letter on 
        ''' amultiple sequence alignment normalized to range from 0 to 100% for each site along the 
        ''' chromosome of a specific index genome.
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="index">参考序列在所输入的fasta序列之中的位置，默认使用第一条序列作为参考序列</param>
        Public Function NTVariations(<Parameter("Fasta",
                                                "The fasta object parameter should be the output of mega multiple alignment result. All of the sequence in this parameter should be in the same length.")>
                                     Fasta As FastaFile,
                                     <Parameter("Index",
                                                "The index of the reference genome in the fasta object parameter, default value is ZERO (The first sequence as the reference.)")>
                                     Optional index As Integer = Scan0,
                                     Optional cutoff As Double = 0.75) As Double()
            Dim ref As FASTA.FastaSeq = Fasta(index)
            Return ref.NTVariations(Fasta, cutoff)
        End Function

        ''' <summary>
        ''' The conservation percentage (%) Is defined as the number of 
        ''' genomes with the same letter on amultiple sequence alignment 
        ''' normalized to range from 0 to 100% for each site along the 
        ''' chromosome of a specific index genome.
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <param name="Fasta"></param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        <Extension>
        Public Function NTVariations(ref As FastaSeq,
                                     <Parameter("Fasta",
                                                "The fasta object parameter should be the output of mega multiple alignment result. All of the sequence in this parameter should be in the same length.")>
                                     Fasta As FASTA.FastaFile,
                                     Optional cutoff As Double = 0.75) As Double()
            Dim frq As PatternModel = Frequency(Fasta)
            Dim refSeq As String = ref.SequenceData.ToUpper
            Dim var As Double() = refSeq.Select(Function(ch, pos) __variation(ch, pos, cutoff, frq)).ToArray
            Return var
        End Function

        <Extension>
        Public Function Variation(res As IPatternSite, ref As Char, Optional cutoff As Double = 0.7) As Double
            If ref = "-"c Then
                ' 参考序列这里已经是完全的呗突变掉了
                Return 1
            End If

            ' 有频率的残基列表之中找不到参考的残基，则突变率为 1
            If Array.IndexOf(res.EnumerateKeys.ToArray, ref) = -1 Then
                Return 1
            End If

            Dim frq As Double = res(ref)
            Dim var As Double = 1 - frq

            If var < cutoff Then
                Return 0
            Else
                Return var
            End If
        End Function

        ''' <summary>
        ''' 这个是参考的碱基位点
        ''' </summary>
        ''' <param name="ch"></param>
        ''' <param name="index"></param>
        ''' <param name="cutoff"></param>
        ''' <param name="fr"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function __variation(ch As Char, index As Integer, cutoff As Double, fr As PatternModel) As Double
            Return fr(index).Variation(ch, cutoff)
        End Function

        ''' <summary>
        ''' Statics of the occurence frequency for the specific alphabet at specific 
        ''' column in the fasta source.
        ''' (因为是大小写敏感的，所以参数<see cref="Fasta"/>里面的所有的序列数据都必须是大写的)
        ''' </summary>
        ''' <param name="Fasta"></param>
        ''' <param name="p">The column number.</param>
        ''' <param name="C">Alphabet specific for the frequency statics</param>
        ''' <param name="numOfFasta">The total number of the fasta sequence</param>
        ''' <returns></returns>
        Private Function __frequency(Fasta As IEnumerable(Of FastaSeq),
                                     p As Integer,
                                     C As Char,
                                     numOfFasta As Integer) As Double

            Dim LQuery As Integer = (From nt As FastaSeq
                                     In Fasta
                                     Let chr As Char = nt.SequenceData(p)
                                     Where C = chr
                                     Select 1).Sum
            Dim f As Double = LQuery / numOfFasta
            Return f
        End Function
    End Module
End Namespace
