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

Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' 基因组序列数据的提供者。
    ''' </summary>
    ''' <remarks>
    ''' 这个抽象层使得``ORIGIN``段的序列数据既可以常驻内存(<see cref="InMemorySequenceProvider"/>),
    ''' 也可以保留在原始的gbff文件之中按需回读(<see cref="FileBackedSequenceProvider"/>)。
    ''' </remarks>
    Public Interface IGenomeSequenceProvider

        ''' <summary>
        ''' 1-based闭区间的序列切片，返回的字符串是新的内存分配的结果
        ''' </summary>
        Function Slice(left As Long, right As Long) As String

        ''' <summary>
        ''' 按照feature的多段location拼接序列，并且在需要的时候进行反向互补
        ''' </summary>
        Function GetSequence(feature As Feature) As String

        ''' <summary>
        ''' 将整条序列加载到内存之中
        ''' </summary>
        Function Materialize() As String

        ''' <summary>
        ''' 序列的长度(碱基数量)
        ''' </summary>
        ReadOnly Property Length As Long

        ''' <summary>
        ''' 序列数据是否可用
        ''' </summary>
        ReadOnly Property IsAvailable As Boolean
    End Interface

    ''' <summary>
    ''' <see cref="IGenomeSequenceProvider"/>的基础实现，提供了基于<see cref="IGenomeSequenceProvider.Slice"/>
    ''' 的多片段序列拼接逻辑。
    ''' </summary>
    Public MustInherit Class GenomeSequenceProvider : Implements IGenomeSequenceProvider

        Public MustOverride Function Slice(left As Long, right As Long) As String Implements IGenomeSequenceProvider.Slice

        Public MustOverride ReadOnly Property Length As Long Implements IGenomeSequenceProvider.Length

        Public Overridable ReadOnly Property IsAvailable As Boolean Implements IGenomeSequenceProvider.IsAvailable
            Get
                Return Length > 0
            End Get
        End Property

        ''' <summary>
        ''' 默认的整条序列的加载方式: 直接切片
        ''' </summary>
        Public Overridable Function Materialize() As String Implements IGenomeSequenceProvider.Materialize
            Return Slice(1, Length)
        End Function

        ''' <summary>
        ''' 提取feature位点所对应的核酸序列。
        ''' 
        ''' 多段位点(``join(...)``)会按照顺序拼接；对于位于互补链上面的位点，
        ''' 会对拼接之后的结果进行反向互补。
        ''' </summary>
        Public Overridable Function GetSequence(feature As Feature) As String Implements IGenomeSequenceProvider.GetSequence
            If feature Is Nothing OrElse feature.Location Is Nothing Then
                Return ""
            End If

            Dim loci As Location = feature.Location
            Dim segments As RegionSegment() = loci.SequenceSegments

            If segments.IsNullOrEmpty Then
                Return ""
            End If

            If loci.Complement Then
                ' 整个位点位于互补链: 相当于将各个片段按照倒序逐段反向互补之后拼接
                Dim pieces As New List(Of String)

                For i As Integer = segments.Length - 1 To 0 Step -1
                    Call pieces.Add(NucleotideSlice.ReverseComplement(__segmentSequence(segments(i))))
                Next

                Return NucleotideSlice.Concat(pieces)
            Else
                Dim pieces As New List(Of String)

                For i As Integer = 0 To segments.Length - 1
                    Dim piece As String = __segmentSequence(segments(i))

                    If segments(i).Complement Then
                        piece = NucleotideSlice.ReverseComplement(piece)
                    End If

                    Call pieces.Add(piece)
                Next

                Return NucleotideSlice.Concat(pieces)
            End If
        End Function

        Private Function __segmentSequence(seg As RegionSegment) As String
            If seg Is Nothing Then
                Return ""
            End If

            Dim left As Long = seg.Left
            Dim right As Long = seg.Right

            If left > right Then
                left = seg.Right
                right = seg.Left
            End If
            If left < 1 OrElse right < 1 Then
                Return ""
            End If

            Return Slice(left, right)
        End Function
    End Class

    ''' <summary>
    ''' 序列数据常驻内存的提供者: 直接包装``ORIGIN``段的序列字符串
    ''' </summary>
    Public Class InMemorySequenceProvider : Inherits GenomeSequenceProvider

        Private ReadOnly seq As String

        Sub New(sequence As String)
            Me.seq = If(sequence, "")
        End Sub

        Public Overrides Function Slice(left As Long, right As Long) As String
            Return NucleotideSlice.Slice(seq, left, right)
        End Function

        Public Overrides ReadOnly Property Length As Long
            Get
                Return seq.Length
            End Get
        End Property

        Public Overrides ReadOnly Property IsAvailable As Boolean
            Get
                Return Not String.IsNullOrEmpty(seq)
            End Get
        End Property

        Public Overrides Function Materialize() As String
            Return seq
        End Function
    End Class

    ''' <summary>
    ''' 低内存模式的序列数据提供者: 序列数据仍然保留在原始的gbff文件之中，
    ''' 在需要提取序列的时候按照文件偏移量进行按需回读。
    ''' </summary>
    ''' <remarks>
    ''' 为了减少回读的次数，``ORIGIN``段之中每隔<see cref="GbkReadOptions.SequenceAnchorInterval"/>
    ''' 行就会记录一个锚点(已经读取的碱基数量 + 该锚点所处的文件偏移量)，回读的时候会首先
    ''' 二分查找到目标位置之前最近的锚点，然后再顺序扫描到目标位置。
    ''' </remarks>
    Public NotInheritable Class FileBackedSequenceProvider : Inherits GenomeSequenceProvider : Implements IDisposable

        Private ReadOnly path As String
        Private ReadOnly originOffset As Long
        Private ReadOnly totalLetters As Long
        Private ReadOnly perLine As Integer
        Private ReadOnly anchorLetters As Long()
        Private ReadOnly anchorOffsets As Long()

        Private ReadOnly buffer As Byte() = New Byte(65535) {}
        Private bufPos As Integer
        Private bufLen As Integer

        Private stream As IO.FileStream
        Private ReadOnly sync As New Object()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">gbff文件的路径</param>
        ''' <param name="originOffset">``ORIGIN``段第一行序列数据的文件偏移量</param>
        ''' <param name="totalLetters">序列的总碱基数量</param>
        ''' <param name="perLine">每一行序列数据的碱基数量(用于估算，可以为0)</param>
        ''' <param name="anchorLetters">锚点位置之上已经读取的碱基数量</param>
        ''' <param name="anchorOffsets">锚点所处的文件偏移量</param>
        Sub New(path As String,
                originOffset As Long,
                totalLetters As Long,
                perLine As Integer,
                anchorLetters As Long(),
                anchorOffsets As Long())

            Me.path = path
            Me.originOffset = originOffset
            Me.totalLetters = totalLetters
            Me.perLine = perLine
            Me.anchorLetters = If(anchorLetters, New Long() {0})
            Me.anchorOffsets = If(anchorOffsets, New Long() {originOffset})
        End Sub

        ''' <summary>
        ''' 序列数据所在的文件路径
        ''' </summary>
        Public ReadOnly Property FilePath As String
            Get
                Return path
            End Get
        End Property

        Public Overrides ReadOnly Property Length As Long
            Get
                Return totalLetters
            End Get
        End Property

        Public Overrides ReadOnly Property IsAvailable As Boolean
            Get
                Return Not String.IsNullOrEmpty(path) AndAlso IO.File.Exists(path)
            End Get
        End Property

        ''' <summary>
        ''' 低内存模式之下，这个函数会将整条序列加载到内存之中，请谨慎使用
        ''' </summary>
        Public Overrides Function Materialize() As String
            If totalLetters <= 0 Then
                Return ""
            End If

            Return Slice(1, totalLetters)
        End Function

        Public Overrides Function Slice(left As Long, right As Long) As String
            If left < 1 Then
                left = 1
            End If
            If right > totalLetters Then
                right = totalLetters
            End If
            If right < left Then
                Return ""
            End If
            If Not IsAvailable Then
                Return ""
            End If

            Dim length As Long = right - left + 1

            If length > Integer.MaxValue Then
                Throw New GbkFormatException($"the sequence segment is too large: {left}..{right}!")
            End If

            Dim first As Long = left - 1
            Dim idx As Integer = __findAnchor(first)
            Dim skip As Long = first - anchorLetters(idx)
            Dim out As Char() = New Char(CInt(length) - 1) {}
            Dim n As Integer = 0

            SyncLock sync
                Try
                    Call __ensureStream()
                    Call stream.Seek(anchorOffsets(idx), IO.SeekOrigin.Begin)

                    bufPos = 0
                    bufLen = 0

                    Do While n < out.Length
                        Dim b As Integer = __readByte()

                        If b < 0 Then
                            Exit Do
                        End If

                        Dim c As Char = ChrW(b)

                        If NucleotideSlice.IsLetter(c) Then
                            If skip > 0 Then
                                skip -= 1
                            Else
                                out(n) = c
                                n += 1
                            End If
                        End If
                    Loop
                Catch ex As IO.IOException
                    Call App.LogException(ex)
                    Return ""
                End Try
            End SyncLock

            If n = 0 Then
                Return ""
            ElseIf n = out.Length Then
                Return New String(out)
            Else
                Return New String(out, 0, n)
            End If
        End Function

        ''' <summary>
        ''' 二分查找到小于等于目标位置的最大的锚点
        ''' </summary>
        Private Function __findAnchor(letterIndex As Long) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = anchorLetters.Length - 1
            Dim ans As Integer = 0

            Do While lo <= hi
                Dim mid As Integer = (lo + hi) \ 2

                If anchorLetters(mid) <= letterIndex Then
                    ans = mid
                    lo = mid + 1
                Else
                    hi = mid - 1
                End If
            Loop

            Return ans
        End Function

        Private Sub __ensureStream()
            If stream Is Nothing Then
                stream = New IO.FileStream(path,
                                           IO.FileMode.Open,
                                           IO.FileAccess.Read,
                                           IO.FileShare.Read,
                                           4096,
                                           IO.FileOptions.RandomAccess)
            End If
        End Sub

        Private Function __readByte() As Integer
            If bufPos >= bufLen Then
                bufLen = stream.Read(buffer, 0, buffer.Length)
                bufPos = 0

                If bufLen <= 0 Then
                    bufLen = 0
                    Return -1
                End If
            End If

            Dim b As Integer = buffer(bufPos)
            bufPos += 1

            Return b
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            SyncLock sync
                If stream IsNot Nothing Then
                    Call stream.Dispose()
                    stream = Nothing
                End If
            End SyncLock
        End Sub

        Public Overrides Function ToString() As String
            Return $"{path}:{originOffset} ({totalLetters} bp, {perLine} bp/line)"
        End Function
    End Class
End Namespace
