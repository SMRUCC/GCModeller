#Region "Microsoft.VisualBasic::342976ae0e27ecf7546b8d52f7ff7eda, GCModeller\analysis\SequenceToolkit\SNP\DiffVariation.vb"

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

    '   Total Lines: 153
    '    Code Lines: 114
    ' Comment Lines: 13
    '   Blank Lines: 26
    '     File Size: 5.53 KB


    ' Module DiffVariation
    ' 
    '     Function: [Date], GetSeqs, GroupByDate
    ' 
    ' Class KSeq
    ' 
    '     Properties: attrs, Diffs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module DiffVariation

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="aln">必须是经过对齐了的，第一条序列为参考序列，假若参考可选参数是缺失的话</param>
    ''' <param name="refIndex">默认是第一条序列，如果index参数是缺失的话</param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function GetSeqs(aln As IEnumerable(Of FastaSeq), Optional refIndex$ = Nothing) As IEnumerable(Of KSeq)
        Dim source As New FastaFile(aln)
        Dim index% = If(
            String.IsNullOrEmpty(refIndex),
            0,
            source.Index(refIndex))

        If index = -1 Then
            Throw New Exception(
                $"Reference sequence index value {refIndex} is not valid!")
        Else
            Call $"Using reference sequence: {source(index).Title}".__DEBUG_ECHO
        End If

        Dim ref As FastaSeq = source(index%)
        Dim refs As Char() = ref.SequenceData.ToUpper.ToCharArray

        Call source.RemoveAt(index)

        For Each seq As FastaSeq In source
            Dim nts As Char() = seq.SequenceData.ToUpper.ToCharArray
            Dim diffs As New List(Of SeqValue(Of NamedValue(Of Integer)))
            Dim b As Integer

            For Each i As SeqValue(Of Char) In refs.SeqIterator
                If i.value = "-"c Then
                    b = 0
                Else
                    If nts(i.i) = "-"c OrElse i.value = nts(i.i) Then
                        b = 0
                    Else
                        b = 1
                    End If
                End If

                diffs += New SeqValue(Of NamedValue(Of Integer)) With {
                    .i = i.i,
                    .value = New NamedValue(Of Integer) With {
                        .Name = nts(i.i).ToString,
                        .Value = b
                    }
                }
            Next

            Dim x As New KSeq With {
                .attrs = seq.Headers,
                .Diffs = diffs.ToDictionary(Function(o) o.i,
                                            Function(o) o.value)
            }
            x.Date.value = Regex.Match(seq.Headers.Last, "\d{6}").Value

            Yield x
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="cumulative">数据是否是按照日期的累计性的</param>
    ''' <param name="raw">原始的分数数据</param>
    ''' <returns></returns>
    <Extension>
    Public Function GroupByDate(source As IEnumerable(Of KSeq), Optional cumulative? As Boolean = False, Optional ByRef raw As List(Of EntityObject) = Nothing) As DataSet()
        Dim LGroup = From x As KSeq
                     In source
                     Select x
                     Group x By x.Date.value Into Group
                     Order By value Ascending

        Dim bufs = LGroup.ToArray  ' 按照日期分组的序列突变数据
        Dim out As New List(Of DataSet)
        Dim aLen As Integer =
            bufs.First.Group.First.Diffs.Count

        raw = New List(Of EntityObject)

        Dim cumuMutates%() = New Integer(aLen - 1) {}
        Dim cumuTotal%() = New Integer(aLen - 1) {}

        For Each g In bufs   ' 都是同一个年/月的
            Dim tag As String = g.value
            Dim ar As KSeq() = g.Group.ToArray
            Dim hash As New Dictionary(Of String, Double)
            Dim rawRow As New Dictionary(Of String, String)

            If Not cumulative Then
                For i As Integer = 0 To aLen - 1
                    cumuMutates(i) = Scan0
                    cumuTotal(i) = Scan0
                Next
            End If

            For i As Integer = 0 To aLen - 1
                Dim ind As Integer = i
                Dim d As Integer = (From x As KSeq
                                    In ar
                                    Let b = x.Diffs(ind).Value
                                    Where b <> 0
                                    Select 1).Count ' 当前月的突变序列数量

                cumuMutates(i) += d
                cumuTotal(i) += ar.Length

                Call rawRow.Add((i + 1).ToString, $"{cumuMutates(i)}/{cumuTotal(i)}")
                Call hash.Add((i + 1).ToString, cumuMutates(i) / cumuTotal(i))
            Next

            out += New DataSet With {
                .ID = tag,
                .Properties = hash
            }
            raw += New EntityObject With {
                .ID = tag,
                .Properties = rawRow
            }

            Call tag.__DEBUG_ECHO
        Next

        Return out
    End Function

    <Extension>
    Public Function [Date](x As KSeq) As PropertyValue(Of String)
        Return PropertyValue(Of Object).Read(Of KSeq, String)(x, NameOf([Date]))
    End Function
End Module

Public Class KSeq : Inherits DynamicPropertyBase(Of Object)

    Public Property attrs As String()
    Public Property Diffs As Dictionary(Of Integer, NamedValue(Of Integer))

End Class
