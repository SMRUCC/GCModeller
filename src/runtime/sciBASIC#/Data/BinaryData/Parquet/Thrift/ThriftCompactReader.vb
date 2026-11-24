Option Strict On
Option Explicit On
Option Infer On

Imports System.Buffers.Binary
Imports System.IO
Imports System.Text
Imports BufferPool = Microsoft.VisualBasic.Data.IO.BufferPool

Namespace Thrift

    ''' <summary>
    ''' Thrift compact protocol 读取器，带有严格的边界与长度校验。
    ''' </summary>
    Public NotInheritable Class ThriftCompactReader

        Private Const MaxVarintBytes As Integer = 10

        Private ReadOnly _stream As Stream
        Private ReadOnly _fieldStack As New Stack(Of Integer)
        Private ReadOnly _length As Long
        Private _lastFieldId As Integer

        Public Sub New(stream As Stream)
            If stream Is Nothing Then
                Throw New ArgumentNullException(NameOf(stream))
            End If
            If Not stream.CanRead Then
                Throw New ArgumentException("the input stream is not readable!", NameOf(stream))
            End If

            _stream = stream
            _length = If(stream.CanSeek, stream.Length, -1L)
        End Sub

        Public ReadOnly Property BaseStream As Stream
            Get
                Return _stream
            End Get
        End Property

        Public ReadOnly Property Position As Long
            Get
                Return _stream.Position
            End Get
        End Property

        Public Sub ReadStructBegin()
            Call _fieldStack.Push(_lastFieldId)
            _lastFieldId = 0
        End Sub

        Public Sub ReadStructEnd()
            _lastFieldId = _fieldStack.Pop()
        End Sub

        ''' <summary>
        ''' 读取下一个字段头。当返回的类型为 <see cref="ThriftType.Stop"/> 时表示结构体结束。
        ''' </summary>
        Public Function ReadFieldHeader() As (id As Integer, type As ThriftType)
            Dim b As Byte = ReadByteOrThrow()

            If b = 0 Then
                Return (0, ThriftType.Stop)
            End If

            Dim t As ThriftType = CType(b And &HF, ThriftType)
            Dim delta As Integer = (b And &HF0) >> 4

            If delta = 0 Then
                _lastFieldId = CInt(ReadZigZag())
            Else
                _lastFieldId += delta
            End If

            Return (_lastFieldId, t)
        End Function

        Public Function ReadBoolField(type As ThriftType) As Boolean
            Select Case type
                Case ThriftType.True
                    Return True
                Case ThriftType.False
                    Return False
                Case Else
                    Throw New InvalidDataException($"thrift: expected a boolean field header but got {type}!")
            End Select
        End Function

        Public Function ReadByte() As Byte
            Return ReadByteOrThrow()
        End Function

        Public Function ReadBoolValue() As Boolean
            Dim b As Byte = ReadByteOrThrow()

            Select Case b
                Case 1
                    Return True
                Case 2, 0
                    Return False
                Case Else
                    Throw New InvalidDataException($"thrift: {b} is not a valid boolean value!")
            End Select
        End Function

        Public Function ReadVarint() As ULong
            Dim result As ULong = 0UL
            Dim shift As Integer = 0
            Dim count As Integer = 0

            Do
                Dim b As Byte = ReadByteOrThrow()

                result = result Or (CULng(b And &H7F) << shift)
                count += 1

                If (b And &H80) = 0 Then
                    Return result
                End If

                shift += 7

                If count >= MaxVarintBytes Then
                    Throw New InvalidDataException("thrift: the varint value is too long, the data is corrupted!")
                End If
            Loop

        End Function

        Public Function ReadZigZag() As Long
            Dim value As ULong = ReadVarint()
            Dim raw As Long = AsSigned(value >> 1)

            If (value And 1UL) <> 0UL Then
                raw = Not raw
            End If

            Return raw
        End Function

        Public Function ReadI32() As Integer
            Dim value As Long = ReadZigZag()

            If value < Integer.MinValue OrElse value > Integer.MaxValue Then
                Throw New InvalidDataException($"thrift: the i32 value {value} is out of the range!")
            End If

            Return CInt(value)
        End Function

        Public Function ReadI16() As Short
            Return CShort(ReadI32())
        End Function

        Public Function ReadI64() As Long
            Return ReadZigZag()
        End Function

        Public Function ReadDouble() As Double
            ' 线上格式为小端：直接按小端解出，避免 8 字节临时数组与可能的 Array.Reverse
            Dim buffer As Byte() = ScratchOf(8)

            Call ReadInto(buffer, 0, 8)

            Return BinaryPrimitives.ReadDoubleLittleEndian(buffer.AsSpan(0, 8))
        End Function

        Public Function ReadBinary() As Byte()
            Dim size As Integer = ReadBinaryLength()
            Dim buffer As Byte() = New Byte(size - 1) {}

            Call ReadInto(buffer, 0, size)

            Return buffer
        End Function

        Public Function ReadString() As String
            Dim size As Integer = ReadBinaryLength()

            If size = 0 Then
                Return String.Empty
            End If

            ' 旧实现先由 ReadBinary 分配一个精确长度的字节数组再解码为字符串，
            ' 这里直接使用池化缓冲解码，避免每次读取元数据字符串都产生一次短命数组。
            Dim rented As Byte() = BufferPool.RentBytes(size)

            Try
                Call ReadInto(rented, 0, size)

                Return Encoding.UTF8.GetString(rented, 0, size)
            Finally
                BufferPool.ReturnBytes(rented)
            End Try
        End Function

        ''' <summary>
        ''' 读取 binary 字段的字节长度，并做边界校验。
        ''' </summary>
        Private Function ReadBinaryLength() As Integer
            Dim size As ULong = ReadVarint()

            If size > Integer.MaxValue Then
                Throw New InvalidDataException($"thrift: the binary length {size} is too large!")
            End If

            If _length >= 0 AndAlso CLng(size) > (_length - _stream.Position) Then
                Throw New InvalidDataException("thrift: the declared binary length is beyond the end of the stream, the data is corrupted!")
            End If

            Return CInt(size)
        End Function

        ''' <summary>
        ''' 每个线程复用的标量暂存区。
        ''' </summary>
        <ThreadStatic>
        Private Shared _tlsScratch As Byte()

        Private Shared Function ScratchOf(minimumLength As Integer) As Byte()
            If _tlsScratch Is Nothing OrElse _tlsScratch.Length < minimumLength Then
                _tlsScratch = New Byte(minimumLength - 1) {}
            End If

            Return _tlsScratch
        End Function

        Public Function ReadListHeader() As (size As Integer, type As ThriftType)
            Dim b As Byte = ReadByteOrThrow()
            Dim size As Integer = (b And &HF0) >> 4
            Dim t As ThriftType = CType(b And &HF, ThriftType)

            If size = 15 Then
                Dim longSize As ULong = ReadVarint()

                If longSize > Integer.MaxValue Then
                    Throw New InvalidDataException($"thrift: the list size {longSize} is too large!")
                End If

                size = CInt(longSize)
            End If

            Return (size, t)
        End Function

        Public Function ReadMapHeader() As (size As Integer, keyType As ThriftType, valueType As ThriftType)
            Dim size As Integer = CInt(ReadVarint())

            If size <= 0 Then
                Return (0, ThriftType.Stop, ThriftType.Stop)
            End If

            Dim b As Byte = ReadByteOrThrow()

            Return (size, CType((b And &HF0) >> 4, ThriftType), CType(b And &HF, ThriftType))
        End Function

        ''' <summary>
        ''' 跳过当前字段/元素的值，用于前向兼容（忽略不认识的字段）。
        ''' </summary>
        Public Sub Skip(type As ThriftType)
            Select Case type
                Case ThriftType.True, ThriftType.False
                    ' 布尔值已经编码在字段头中，没有附加数据
                Case ThriftType.Byte
                    Call ReadByte()
                Case ThriftType.I16, ThriftType.I32, ThriftType.I64
                    Call ReadVarint()
                Case ThriftType.Double
                    Call ReadInto(ScratchOf(8), 0, 8)
                Case ThriftType.Binary
                    Call ReadBinary()
                Case ThriftType.List, ThriftType.Set
                    Dim header = ReadListHeader()
                    Dim elementType As ThriftType = header.type

                    For i As Integer = 0 To header.size - 1
                        If elementType = ThriftType.True OrElse elementType = ThriftType.False Then
                            Call ReadByte()
                        Else
                            Call Skip(elementType)
                        End If
                    Next
                Case ThriftType.Map
                    Dim header = ReadMapHeader()

                    For i As Integer = 0 To header.size - 1
                        Call Skip(header.keyType)
                        Call Skip(header.valueType)
                    Next
                Case ThriftType.Struct
                    Call ReadStructBegin()

                    Dim field = ReadFieldHeader()

                    While field.type <> ThriftType.Stop
                        Call Skip(field.type)
                        field = ReadFieldHeader()
                    End While

                    Call ReadStructEnd()
                Case Else
                    Throw New InvalidDataException($"thrift: unsupported type {type} for skipping!")
            End Select
        End Sub

        Private Function ReadByteOrThrow() As Byte
            Dim value As Integer = _stream.ReadByte()

            If value < 0 Then
                Throw New EndOfStreamException("thrift: unexpected end of the stream while reading the metadata!")
            End If

            Return CByte(value)
        End Function

        ''' <summary>
        ''' 把 <paramref name="count"/> 个字节读入调用方提供的缓冲区，不产生新的数组分配。
        ''' </summary>
        Private Sub ReadInto(buffer As Byte(), offset As Integer, count As Integer)
            Dim done As Integer = 0

            While done < count
                Dim n As Integer = _stream.Read(buffer, offset + done, count - done)

                If n <= 0 Then
                    Throw New EndOfStreamException("thrift: unexpected end of the stream while reading the metadata!")
                End If

                done += n
            End While
        End Sub

        Private Shared Function AsSigned(value As ULong) As Long
            ' CLng 与 BitConverter.ToInt64(BitConverter.GetBytes(value)) 的位模式一致，
            ' 但不再分配一个 8 字节的临时数组。
            Return CLng(value)
        End Function

    End Class

End Namespace
