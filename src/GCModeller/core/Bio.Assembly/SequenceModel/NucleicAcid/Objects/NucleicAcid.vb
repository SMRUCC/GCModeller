#Region "Microsoft.VisualBasic::707991f58e8ea0ae499bb630e93aec95, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Objects\NucleicAcid.vb"

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

    '   Total Lines: 379
    '    Code Lines: 226
    ' Comment Lines: 95
    '   Blank Lines: 58
    '     File Size: 14.53 KB


    '     Class NucleicAcid
    ' 
    '         Properties: GC, Length, SequenceData, Tm, UserTag
    ' 
    '         Constructor: (+6 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) Complement, CopyNT, (+2 Overloads) Counts, CreateObject, Enums
    '                   GetSegment, IEnumerable_GetEnumerator, IEnumerable_GetEnumerator1, InvalidForNt, ReadSegment
    '                   RemoveInvalids, Replace, Reverse, Split, ToArray
    '                   (+3 Overloads) ToString
    ' 
    '         Sub: convertSequence
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' The nucleotide sequence object.(核酸序列对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class NucleicAcid : Inherits ISequenceModel
        Implements IEnumerable(Of DNA)

        ''' <summary>
        ''' Cache data for maintaining the high performance on sequence operation.
        ''' </summary>
        ''' <remarks></remarks>
        Dim _innerSeqCache As String

        Protected _innerSeqModel As DNA()

        ''' <summary>
        ''' 序列的所以的碱基枚举的数组，可以看作为一个完整的序列
        ''' </summary>
        ''' <returns></returns>
        Public Function ToArray() As DNA()
            Return _innerSeqModel.ToArray
        End Function

        Public Function Counts(base As DNA) As Double
            Return Counts(_innerSeqModel, base)
        End Function

        ''' <summary>
        ''' 计算某一种碱基在序列之中的出现频率
        ''' </summary>
        ''' <param name="base">只允许``ATGC``</param>
        ''' <returns>因为可能还存在简并碱基字符，所以在这里返回一个小数</returns>
        Public Shared Function Counts(nt As DNA(), base As DNA) As Double
            Dim n# = nt.Where(Function(b) b = base).Count
            Dim dbEntries = Conversion.BaseDegenerateEntries(base)

            For Each dgBase As DNA In dbEntries
                Dim cd% = nt.Where(Function(b) b = dgBase).Count
                Dim l = 1 / Conversion.DegenerateBases(dgBase).Length

                ' 因为计算简并碱基的时候，是平均分配的，所以在这里就除以该简并碱基的可替换的碱基数量
                n += cd * l
            Next

            ' 故而包含有简并碱基的计算结果应该是带有小数的
            Return n
        End Function

        ''' <summary>
        ''' 用户定义的标签数据，有时候用于在不同的序列之间唯一的标记当前的这条序列
        ''' </summary>
        ''' <returns></returns>
        Public Property UserTag As String

        ''' <summary>
        ''' 字符串形式的序列数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Property SequenceData As String
            Get
                Return _innerSeqCache
            End Get
            Set(value As String)
                _innerSeqModel = NucleicAcid.Enums(value).AsList
                MyBase.SequenceData = value
                _innerSeqCache = value
            End Set
        End Property

        Public Shared Iterator Function Enums(sequence As String) As IEnumerable(Of DNA)
            For Each ch As Char In sequence
                If Conversion.NucleotideConvert.ContainsKey(ch) Then
                    Yield Conversion.NucleotideConvert(ch)
                Else
                    Yield DNA.NA
                End If
            Next
        End Function

        ''' <summary>
        ''' The melting temperature of P1 is Tm(P1), which is a reference temperature for a primer to perform annealing and known as the Wallace formula
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Tm As Double
            Get
                Return NucleicAcidStaticsProperty.Tm(Me.SequenceData)
            End Get
        End Property

        ''' <summary>
        ''' Calculate the GC content of the current sequence data.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GC As Double
            Get
                Return NucleicAcidStaticsProperty.GCContent(Me)
            End Get
        End Property

        Sub New(Sequence As IEnumerable(Of DNA))
            Call convertSequence(ToString(Sequence), True)
        End Sub

        ''' <summary>
        ''' Construct the nucleotide sequence from a nt sequence model interface <see cref="IPolymerSequenceModel"/>
        ''' </summary>
        ''' <param name="SequenceData"></param>
        Sub New(SequenceData As IPolymerSequenceModel)
            Call convertSequence(SequenceData.SequenceData, True)
        End Sub

        Sub New()
        End Sub

        ''' <summary>
        ''' Construct the nucleotide sequence from a ATGC based sequence string.
        ''' (从一个序列字符串之中创建一条核酸链分子对象)
        ''' </summary>
        ''' <param name="SequenceData">This sequence data can be user input from the interface or sequence data from the <see cref="FASTA.FastaSeq"/> object.</param>
        Sub New(SequenceData As String)
            Call convertSequence(SequenceData, True)
        End Sub

        ''' <summary>
        ''' Construct the nucleotide seuqnece form a nt sequence model object. The nt sequence object should inherits from the base class <see cref="SequenceModel.ISequenceModel"/>
        ''' </summary>
        ''' <param name="SequenceData"></param>
        Sub New(SequenceData As ISequenceModel)
            Call convertSequence(SequenceData.SequenceData, True)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="strict">默认参数表示当核酸序列之中存在非法字符的时候会直接抛出错误</param>
        Sub New(nt As FASTA.FastaSeq, Optional strict As Boolean = True)
            Try
                Call convertSequence(nt.SequenceData, strict)
            Catch ex As Exception
                ex = New Exception(nt.Title, ex)
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 检查序列的可用性
        ''' </summary>
        ''' <param name="seq"></param>
        Private Sub convertSequence(seq$, strict As Boolean)
            Dim nt As String = seq.ToUpper.Replace("N", "-").Replace(".", "-")
            Dim invalids As Char() = InvalidForNt(nt)

            ' 大写字母的
            seq = nt

            If invalids.Length > 0 Then  ' 有非法字符
                If strict Then
                    Dim ex As Exception = New DataException(InvalidNotAllowed)
                    ex = New Exception(invalids.GetJson, ex)
                    Throw ex
                Else
                    ' 非严格模式下，会将这些非法字符替换为-空格
                    Dim sb As New StringBuilder(seq)

                    For Each c As Char In invalids
                        Call sb.Replace(c, "-")
                    Next

                    Me.SequenceData = sb.ToString
                End If
            Else
                Me.SequenceData = seq
            End If
        End Sub

        Public Shared Function InvalidForNt(seq As String) As Char()
            Dim LQuery As Char() =
                LinqAPI.Exec(Of Char) <= From c As Char
                                         In seq
                                         Where Not Conversion.IsAValidDNAChar(c)
                                         Select c
                                         Distinct
            Return LQuery
        End Function

        ''' <summary>
        ''' Removes the invalids characters in the nt sequence. Invalids source is comes from <see cref="TypeExtensions.AA_CHARS_ALL"/>
        ''' </summary>
        ''' <param name="nt">Case insensitive.</param>
        ''' <returns></returns>
        Public Shared Function RemoveInvalids(nt As String) As String
            Dim seq As New StringBuilder(nt.ToUpper)

            For Each c As Char In TypeExtensions.AA_CHARS_ALL
                Call seq.Replace(c, "-"c)
            Next

            Return seq.ToString
        End Function

        Const InvalidNotAllowed As String = "Target fasta sequence is a protein sequence. Only allows character [ATGCN-.]..."
        Const NTCHRS As String = "ATGC-"

        Public Shared Function CopyNT(seq As String) As NucleicAcid
            Return New NucleicAcid(Replace(seq))
        End Function

        Public Shared Function Replace(seq As String) As String
            Dim lst As New List(Of Char)

            For Each ch As Char In seq.ToUpper
                If NTCHRS.IndexOf(ch) = -1 Then
                    Call lst.Add("-"c)
                Else
                    Call lst.Add(ch)
                End If
            Next

            Return New String(lst.ToArray)
        End Function

        ''' <summary>
        ''' 分割得到的小片段的长度
        ''' </summary>
        ''' <param name="segmentLen"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Split(segmentLen%) As SegmentObject()
            Dim list As New List(Of SegmentObject)
            segmentLen -= 1

            For i As Integer = 1 To Me.Length Step segmentLen + 1
                Dim strSegmentData As String = Mid(Me.SequenceData, i, segmentLen)

                list += New SegmentObject With {
                    .SequenceData = strSegmentData,
                    .Left = i,
                    .Right = i + Len(strSegmentData)
                }
            Next

            Return list
        End Function

        Public Overrides ReadOnly Property Length As Integer
            Get
                Return Len(_innerSeqCache)
            End Get
        End Property

        ''' <summary>
        ''' <paramref name="Start"></paramref>和<paramref name="End"></paramref>的值都是数组的下标，在本函数之中已经自动为这两个参数+1了
        ''' </summary>
        ''' <param name="Start">位置的左端的开始位置</param>
        ''' <param name="End">右端的结束位置</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSegment(Start As Long, [End] As Long) As NucleicAcid
            Start += 1
            [End] += 1
            Return New NucleicAcid With {
                .SequenceData = Mid(Me.SequenceData, Start, [End] - Start)
            }
        End Function

        ''' <summary>
        ''' <paramref name="Left"></paramref>的值是数组的下标，在本函数之中已经自动为这个参数+1了
        ''' </summary>
        ''' <param name="Length">片段的长度</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReadSegment(Left As Integer, Length As Integer) As String
            Left += 1
            Return Mid(Me.SequenceData, Left, Length)
        End Function

        Public Function Complement() As NucleicAcid
            Return New NucleicAcid With {
                .SequenceData = Complement(Me.SequenceData)
            }
        End Function

        Public Function Reverse() As NucleicAcid
            Return New NucleicAcid With {
                .SequenceData = Me.SequenceData.Reverse.ToArray
            }
        End Function

        Public Shared Function CreateObject(strSeq As String) As NucleicAcid
            Return New NucleicAcid With {
                .SequenceData = strSeq
            }
        End Function

        ''' <summary>
        ''' Gets the complement sequence of a nucleotide sequence.
        ''' (获取某一条核酸序列的互补序列，但是新得到的序列并不会首尾反转，
        ''' 请注意，这个函数所输入的DNA序列字符串必须是大写字母的)
        ''' </summary>
        ''' <param name="DNAseq">
        ''' The target dna nucleotide sequence to complement.(必须全部都是大写字母)
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Complement(DNAseq As String) As String
            Dim sb As New StringBuilder(DNAseq.ToUpper)

            Call sb.Replace("A"c, "1"c)
            Call sb.Replace("T"c, "2"c)
            Call sb.Replace("G"c, "3"c)
            Call sb.Replace("C"c, "4"c)

            Call sb.Replace("1"c, "T"c)
            Call sb.Replace("2"c, "A"c)
            Call sb.Replace("3"c, "C"c)
            Call sb.Replace("4"c, "G"c)

            Return sb.ToString
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("({0}bp) {1}...", Len(SequenceData), Mid(Me.SequenceData, 1, 25))
        End Function

        Public Overloads Shared Function ToString(nn As DNA) As String
            Return Conversion.NucleotideAsChar(nn).ToString
        End Function

        Public Overloads Shared Function ToString(nt As IEnumerable(Of DNA)) As String
            Dim array As Char() = nt.Select(Function(x) Conversion.NucleotideAsChar(x)).ToArray
            Return New String(array)
        End Function

        Public Shared Widening Operator CType(DNAseq As String) As NucleicAcid
            Return New NucleicAcid With {
                .SequenceData = DNAseq
            }
        End Operator

        Public Shared Narrowing Operator CType(obj As NucleicAcid) As String
            Return obj.SequenceData
        End Operator

        Public Shared Narrowing Operator CType(obj As NucleicAcid) As SegmentObject
            Return New SegmentObject With {
                .Complement = False,
                .Left = 0,
                .Right = 0,
                .SequenceData = obj.SequenceData
            }
        End Operator

        Public Shared Widening Operator CType(fasta As FASTA.FastaSeq) As NucleicAcid
            Return New NucleicAcid With {
                .SequenceData = fasta.SequenceData
            }
        End Operator

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator(Of DNA) Implements IEnumerable(Of DNA).GetEnumerator
            For Each base As DNA In _innerSeqModel
                Yield base
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield IEnumerable_GetEnumerator()
        End Function
    End Class
End Namespace
