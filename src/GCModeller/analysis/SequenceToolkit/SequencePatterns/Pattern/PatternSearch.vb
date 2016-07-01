Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.SequenceModel.FASTA.FastaFile
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Linq
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace Pattern

    ''' <summary>
    ''' 使用用户指定的正则表达式搜索指定的序列数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Module PatternSearch

        ''' <summary>
        ''' 使用用户指定的正则表达式搜索指定的序列数据
        ''' </summary>
        ''' <param name="Seq"></param>
        ''' <param name="pattern"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Match(Seq As FastaFile, pattern As String) As File
            Dim LQuery = From fa As FastaToken
                         In Seq.AsParallel
                         Let Segment As RowObject() = fa.GenerateSegment(pattern)
                         Where Segment IsNot Nothing
                         Select Segment
                         Order By Segment.First.First Ascending '

            Dim df As New File(LQuery.MatrixAsIterator)

            Return df
        End Function

        <Extension>
        Public Function GenerateSegment(Seq As FastaToken, pattern As String) As RowObject()
            Dim LQuery = RowObject.Distinct((From Segment As SegLoci
                                             In Match(Seq.SequenceData, pattern)
                                             Select Segment.ToRow).ToArray).ToArray
            Dim Head As RowObject = {Seq.ToString}, Count As RowObject = {"Hits count:", CStr(LQuery.Count)}
            Dim RowList As List(Of RowObject) =
                New List(Of RowObject) From {Head, Count}

            If LQuery.Length = 0 Then
                Return Nothing
            Else
                Call RowList.Add({"Left", "Right", "Length", "Sequence"})
                Call RowList.AddRange(LQuery)
                Call RowList.Add({" "})
                Return RowList.ToArray
            End If
        End Function

        Public Function Match(sequence As String, pattern As String) As SegLoci()
            Dim startLeft As Long = 1
            Dim LQuery As SegLoci() =
                LinqAPI.Exec(Of SegLoci) <= From m As Match
                                            In Regex.Matches(sequence, pattern) '由于StartLeft参数在被调用的时候是以ByRef的形式传递的，故而这里不可以再使用并行拓展
                                            Let Segment As SegLoci =
                                                SegLoci.CreateObject(WholeSeq:=sequence, SegmentValue:=m, startLeft:=startLeft)
                                            Select Segment
                                            Order By Segment.Left Ascending '
            Return LQuery
        End Function
    End Module
End Namespace