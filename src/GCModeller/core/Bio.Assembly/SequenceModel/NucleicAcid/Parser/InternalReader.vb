Namespace SequenceModel.NucleotideModels : Partial Class SegmentReader

        Protected Class InternalReader

            Const READER_WARNING_MESSAGE As String =
                "[WARNNING] SequenceModel.NucleicAcid -> GetSegment() Molecular length is not enough for the target seqeucne segment!{0}Molecular Length:={1}{2}Start Position:={3}{4}Segment Length:={5}"

            Dim _innerReader As SegmentReader

            Private ReadOnly Property SequenceData As String
                Get
                    Return _innerReader._innerNTsource.SequenceData
                End Get
            End Property

            Sub New(SegmentObject As SegmentReader)
                _innerReader = SegmentObject
            End Sub

#Region "Segment Parser"

            ''' <summary>
            ''' A method for getting a DNA segment on this DNA sequence.(获取本DNA序列之上的一个序列片段的方法)
            ''' </summary>
            ''' <param name="Start">The segment start point.(开始的位点)</param>
            ''' <param name="Length">The length of this segment.(片段的长度)</param>
            ''' <param name="DirectionDownstream">可选参数，向Start位点的上游取片段还是向Start位点的下游取片段，默认取下游片段</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetSegmentSequenceValue(Start As Long,
                                                    Length As Long,
                                                    WARN_MSG As Boolean,
                                                    Optional DirectionDownstream As Boolean = True) As String
                If True = DirectionDownstream Then
                    Return ReadDownStream(Start, Length, WARN_MSG)
                Else
                    Return __getDirectionUpStream(Start, Length, WARN_MSG)
                End If
            End Function

            ''' <summary>
            ''' 在取序列的时候请注意位置：对于Mid函数与数组操作而言，Mid函数的开始下标是从1开始的，所以对于第一个碱基，其在字符数组之中为第一个元素，下表为零，但是在字符串之中，下标为1
            ''' </summary>
            ''' <param name="Start"></param>
            ''' <param name="Length"></param>
            ''' <returns></returns>
            Public Function ReadDownStream(Start As Long, Length As Long, WARN_MSG As Boolean) As String
                If Length > Len(Me._innerReader._innerNTsource.SequenceData) - Start Then '假设需要截取的长度大于剩余的长度，则假若为环状的分子的话

                    If Me._innerReader._isCircularMolecular Then '长度不足的部分截取自开头起始部分
                        Return __readSpanningCircularDNA(Start, Length)
                    Else
                        If WARN_MSG Then
                            Call Console.WriteLine(READER_WARNING_MESSAGE, vbCrLf, Len(SequenceData), vbCrLf, Start, vbCrLf, Length)
                        End If
                        Return New NucleicAcid With {
                            .SequenceData = Mid(SequenceData, Start, Length)
                        } '为线状分子，则仅将剩余部分截取，并给出警告
                    End If
                Else
                    Return New NucleicAcid With {
                        .SequenceData = Mid(SequenceData, Start, Length)
                    }
                End If
            End Function

            ''' <summary>
            ''' 当读取器读取到DNA片段的末尾的时候，可能会长度不够读取目标长度了，假若序列对象为环状分子，则可以使用这个方法来读取连续的序列数据
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Private Function __readSpanningCircularDNA(Start As Integer, Length As Integer) As String
                Dim Sequence As String = Mid$(Me._innerReader._innerNTsource.SequenceData, Start, Length)

                Length -= Len(Sequence)
                Sequence &= Mid(Me._innerReader._innerNTsource.SequenceData, 1, Length)

                Return Sequence
            End Function

            Private Function __readUpStreamSpanningCircularDNA(Length As Integer, Start As Integer) As String
                Dim Current_Length As Integer = Length + Start
                Dim Sequence As String = Mid(Me.SequenceData, 1, Current_Length)

                Length -= Current_Length
                Sequence = Mid(Me.SequenceData, Len(Me.SequenceData) - Length, Length) & Sequence

                Return Sequence
            End Function

            Private Function __getDirectionUpStream(Starts As Long, Length As Long, WARNING As Boolean) As String
                Starts -= Length

                If Starts < 0 Then '上游部分的序列长度不足，则
                    If Me._innerReader._isCircularMolecular Then
                        Return __readUpStreamSpanningCircularDNA(Length, Starts)
                    Else '截取剩余部分并给出警告
                        If Starts < 1 Then Length += Starts
                        If Starts < 0 Then
                            Starts = 1
                        End If
                        If WARNING Then
                            Call Console.WriteLine(READER_WARNING_MESSAGE, vbCrLf, Len(Me.SequenceData), vbCrLf, Starts, vbCrLf, Length)
                        End If
                        Return New NucleicAcid With {.SequenceData = Mid(Me.SequenceData, Starts, Length)} '为线状分子，则仅将剩余部分截取，并给出警告
                    End If
                Else
                    Return New NucleicAcid With {.SequenceData = Mid(Me.SequenceData, Starts, Length)}
                End If
            End Function
#End Region
        End Class
    End Class
End Namespace