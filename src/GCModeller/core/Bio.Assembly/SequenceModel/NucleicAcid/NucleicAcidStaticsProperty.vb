#Region "Microsoft.VisualBasic::496da7e6a63e48c083d286cce7fc232d, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\NucleicAcidStaticsProperty.vb"

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

    '   Total Lines: 306
    '    Code Lines: 185
    ' Comment Lines: 85
    '   Blank Lines: 36
    '     File Size: 13.65 KB


    '     Module NucleicAcidStaticsProperty
    ' 
    '         Function: __circular, __contentCommon, __liner, ATPercent, basePercent
    '                   Count, GC_Content, (+3 Overloads) GCContent, GCData, (+2 Overloads) GetCompositionVector
    '                   Tm
    '         Delegate Function
    ' 
    '             Function: GCSkew, removesNA
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' NucleicAcid sequence property calculator
    ''' </summary>
    <Package("NucleicAcid.Property", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module NucleicAcidStaticsProperty

        ReadOnly defaultProperty As New [Default](Of  NtProperty)(AddressOf GCSkew)

        ''' <summary>
        ''' 批量计算出GCSkew或者GC%
        ''' </summary>
        ''' <param name="nts"></param>
        ''' <param name="winSize"></param>
        ''' <param name="steps"></param>
        ''' <param name="method"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GCData(nts As IEnumerable(Of FastaSeq),
                               Optional winSize As Integer = 250,
                               Optional steps As Integer = 50,
                               Optional method As NtProperty = Nothing) As NamedValue(Of Double())()

            Dim LQuery As IEnumerable(Of (genome As SeqValue(Of FastaSeq), skew As Double()))

            With method Or defaultProperty
                LQuery = From genome As SeqValue(Of FastaSeq)
                         In nts.SeqIterator.AsParallel
                         Order By genome.i Ascending  ' 排序是因为可能没有做多序列比对对齐，在这里需要使用第一条序列的长度作为参考
                         Let vector = .ByRef(genome.value, winSize, steps, True)
                         Select (genome:=genome, skew:=vector)
            End With

            Return LinqAPI.Exec(Of NamedValue(Of Double())) _
 _
                () <= From g
                      In LQuery
                      Select New NamedValue(Of Double()) With {
                          .Name = g.genome.value.ToString,
                          .Value = g.skew
                      }
        End Function

        ''' <summary>
        ''' Calculate the GC content of the target sequence data.
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GC_Content(seq As IEnumerable(Of DNA)) As Double
            Dim array As DNA() = seq.ToArray
            Dim n% = array _
                .Where(Function(nn) nn = DNA.dGMP OrElse nn = DNA.dCMP) _
                .Count

            Return n / array.Length
        End Function

        ''' <summary>
        ''' A, T, G, C
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="pA"></param>
        ''' <param name="pT"></param>
        ''' <param name="pG"></param>
        ''' <param name="pC"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCompositionVector(seq As Char(), ByRef pA#, ByRef pT#, ByRef pG#, ByRef pC#) As Integer()
            Dim c As Integer() = GetCompositionVector(seq)

            pA = c(0) / seq.Length
            pT = c(1) / seq.Length
            pG = c(2) / seq.Length
            pC = c(3) / seq.Length

            Return c
        End Function

        ''' <summary>
        ''' A, T, G, C
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <returns>A, T, G, C</returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("CompositionVector")>
        Public Function GetCompositionVector(Sequence As Char()) As Integer()
            Dim A As Integer = (From ch In Sequence Where ch = "A"c Select 1).Count
            Dim T As Integer = (From ch In Sequence Where ch = "T"c Select 1).Count
            Dim G As Integer = (From ch In Sequence Where ch = "G"c Select 1).Count
            Dim C As Integer = (From ch In Sequence Where ch = "C"c Select 1).Count

            Return New Integer() {A, T, G, C}
        End Function

        ''' <summary>
        ''' Calculate the GC content of the target sequence data.
        ''' </summary>
        ''' <param name="Sequence">序列数据大小写不敏感</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GCContent(Sequence As IPolymerSequenceModel) As Double
            Return GCContent(Sequence.SequenceData)
        End Function

        ''' <summary>
        ''' Calculate the GC content of the target sequence data.
        ''' </summary>
        ''' <param name="NT">序列数据大小写不敏感</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GCContent(NT As String) As Double
            Return (Count(NT, "G"c, "g"c) + Count(NT, "C"c, "c"c)) / Len(NT)
        End Function

        ''' <summary>
        ''' The melting temperature of P1 is Tm(P1), which is a reference temperature for a primer to perform annealing and known as the Wallace formula
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Tm(<Parameter("Primer", "Short DNA sequence which its length is less than 35nt.")> Primer As String) As Double
            Return (Count(Primer, "C"c, "c"c) + Count(Primer, "G"c, "g"c)) * 4 + (Count(Primer, "A"c, "a"c) + Count(Primer, "T"c, "t"c)) * 2
        End Function

        Public Function Count(str As String, ParamArray ch As Char()) As Integer
            If String.IsNullOrEmpty(str) OrElse ch.IsNullOrEmpty Then
                Return 0
            Else
                If ch.Length = 1 Then
                    Dim chr As Char = ch.First
                    Return str.Count(predicate:=Function(c As Char) c = chr)
                Else
                    Return str.Count(predicate:=Function(c As Char) Array.IndexOf(ch, c) > -1)
                End If
            End If
        End Function

        ''' <summary>
        ''' Calculate the GC content of the target sequence data.
        ''' </summary>
        ''' <param name="SequenceModel"></param>
        ''' <param name="SlideWindowSize"></param>
        ''' <param name="Steps"></param>
        ''' <param name="Circular"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GCContent(SequenceModel As IPolymerSequenceModel, SlideWindowSize As Integer, Steps As Integer, Circular As Boolean) As Double()
            Return __contentCommon(SequenceModel, SlideWindowSize, Steps, Circular, {"G", "C"})
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SequenceModel"></param>
        ''' <param name="SlideWindowSize"></param>
        ''' <param name="Steps"></param>
        ''' <param name="Circular"></param>
        ''' <param name="base">必须是大写的字符</param>
        ''' <returns></returns>
        Private Function __contentCommon(SequenceModel As IPolymerSequenceModel,
                                         SlideWindowSize As Integer,
                                         Steps As Integer,
                                         Circular As Boolean,
                                         base As Char()) As Double()
            If Circular Then
                Return __circular(SequenceModel, SlideWindowSize, Steps, base)
            Else
                Return __liner(SequenceModel, SlideWindowSize, Steps, base)
            End If
        End Function

        Private Function __liner(SequenceModel As IPolymerSequenceModel,
                                 SlideWindowSize As Integer,
                                 Steps As Integer,
                                 base As Char()) As Double()
            Dim SequenceData As String = SequenceModel.SequenceData.ToUpper
            Dim ChunkBuffer As List(Of Double) = New List(Of Double)

            For i As Integer = 1 To SequenceData.Length Step Steps
                Dim Segment As String = Mid(SequenceData, i, SlideWindowSize)
                Dim n As Double = basePercent(Segment, SlideWindowSize, base)
                Call ChunkBuffer.Add(n)
            Next

            Return ChunkBuffer.ToArray
        End Function

        Private Function __circular(SequenceModel As IPolymerSequenceModel,
                                    SlideWindowSize As Integer,
                                    Steps As Integer,
                                    base As Char()) As Double()
            Dim SequenceData As String = SequenceModel.SequenceData.ToUpper
            Dim ChunkBuffer As List(Of Double) = New List(Of Double)
            For i As Integer = 1 To SequenceData.Length - SlideWindowSize Step Steps
                Dim Segment As String = Mid(SequenceData, i, SlideWindowSize)
                Dim n As Double = basePercent(Segment, SlideWindowSize, base)
                Call ChunkBuffer.Add(n)
            Next
            For i As Integer = SequenceData.Length - SlideWindowSize + 1 To SequenceData.Length Step Steps
                Dim Segment As String = Mid(SequenceData, i, SlideWindowSize)
                Dim l = SlideWindowSize - Len(Segment)
                Segment &= Mid(SequenceData, 1, l)
                Dim n As Double = basePercent(Segment, SlideWindowSize, base)
                Call ChunkBuffer.Add(n)
            Next
            Return ChunkBuffer.ToArray
        End Function

        ''' <summary>
        ''' Get content percentage value of the given nucleotide bases.
        ''' </summary>
        ''' <param name="segment"></param>
        ''' <param name="winSize"></param>
        ''' <param name="base"></param>
        ''' <returns></returns>
        Private Function basePercent(segment As String, winSize As Integer, base As Char()) As Double
            Dim n As Double = Aggregate b As Char
                              In base
                              Let counts = segment.Count(b)
                              Into Sum(counts)
            n = n / winSize

            Return n
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("AT%")>
        Public Function ATPercent(SequenceModel As IPolymerSequenceModel, SlideWindowSize As Integer, Steps As Integer, Circular As Boolean) As Double()
            Return __contentCommon(SequenceModel, SlideWindowSize, Steps, Circular, {"A", "T"})
        End Function

        Public Delegate Function NtProperty(SequenceModel As IPolymerSequenceModel, SlideWindowSize As Integer, Steps As Integer, Circular As Boolean) As Double()

        ''' <summary>
        ''' Calculation the GC skew of a specific nucleotide acid sequence.
        ''' (对核酸链分子计算GC偏移量，请注意，当某一个滑窗区段内的GC是相等的话，则会出现正无穷)
        ''' </summary>
        ''' <param name="sequence">Target sequence object should be a nucleotide acid sequence.(目标对象必须为核酸链分子)</param>
        ''' <param name="isCircular"></param>
        ''' <returns>返回的矩阵是每一个核苷酸碱基上面的GC偏移量</returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function GCSkew(sequence As IPolymerSequenceModel, slideWindowSize As Integer, steps As Integer, isCircular As Boolean) As Double()
            Dim sequenceData As String = sequence.SequenceData.ToUpper
            Dim vector As New List(Of Double)

            If isCircular Then
                For i As Integer = 1 To sequenceData.Length - slideWindowSize Step steps
                    Dim Segment As String = Mid(sequenceData, i, slideWindowSize)
                    Dim G = (From ch In Segment Where ch = "G"c Select 1).Count
                    Dim C = (From ch In Segment Where ch = "C"c Select 1).Count

                    vector += (G - C) / (G + C)
                Next

                For i As Integer = sequenceData.Length - slideWindowSize + 1 To sequenceData.Length Step steps
                    Dim Segment As String = Mid(sequenceData, i, slideWindowSize)
                    Dim l = slideWindowSize - Len(Segment)
                    Segment &= Mid(sequenceData, 1, l)
                    Dim G = (From ch In Segment Where ch = "G"c Select 1).Count
                    Dim C = (From ch In Segment Where ch = "C"c Select 1).Count

                    vector += (G - C) / (G + C)
                Next
            Else
                For i As Integer = 1 To sequenceData.Length Step steps
                    Dim Segment As String = Mid(sequenceData, i, slideWindowSize)
                    Dim G = (From ch In Segment Where ch = "G"c Select 1).Count
                    Dim C = (From ch In Segment Where ch = "C"c Select 1).Count

                    vector += (G - C) / (G + C)
                Next
            End If

            ' 碱基之间是有顺序的，故而不适用并行化拓展
            Return (From n As Double
                    In vector
                    Select removesNA(n, slideWindowSize)).ToArray
        End Function

        Private Function removesNA(n As Double, winSize As Integer) As Double
            If n.IsNaNImaginary Then
                Return 1 / winSize
            Else
                Return n
            End If
        End Function
    End Module
End Namespace
