Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel.FASTA.FastaFile
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace Pattern

    ''' <summary>
    ''' 程序所搜索到的一个序列片段
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SegLoci : Implements ILocationComponent

        ''' <summary>
        ''' 序列片段
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Sequence As String
        ''' <summary>
        ''' 该序列片段的长度
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Length As Integer
        ''' <summary>
        ''' 该序列在整条序列当中的左端长度和右端长度
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Left As Integer Implements ILocationComponent.Left
        <XmlAttribute> Public Property Right As Integer Implements ILocationComponent.Right

        Public Function ToRow() As RowObject
            Return New String() {Left, Right, Length, Sequence}
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}; <{1}, {2}>, length:= {3}", Sequence, Left, Right, Length)
        End Function

        Public Shared Function CreateObject(SegmentValue As Match, WholeSeq As String, ByRef startLeft As Long) As SegLoci
            Dim Segment As SegLoci = New SegLoci With {
                .Sequence = SegmentValue.Value,
                .Length = SegmentValue.Length
            }
            Segment.Left = InStr(Start:=startLeft, String1:=WholeSeq, String2:=Segment.Sequence, Compare:=CompareMethod.Text)
            Segment.Right = Segment.Left + Segment.Length - 1
            startLeft = Segment.Left + 1

            Return Segment
        End Function
    End Class
End Namespace