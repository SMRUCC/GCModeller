Option Strict On
Option Explicit On
Option Infer On

Imports System.Buffers.Binary
Imports System.IO

Namespace Encodings

    ''' <summary>
    ''' 针对 Parquet 页数据的字节读取游标，所有多字节整数均为<b>小端</b>字节序。
    ''' </summary>
    Public NotInheritable Class ByteReader

        Private Const MaxVarintBytes As Integer = 10

        Private ReadOnly _data As Byte()
        Private _position As Integer

        Public Sub New(data As Byte(), Optional position As Integer = 0)
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If

            _data = data
            _position = position
        End Sub

        ''' <summary>
        ''' 底层缓冲区。
        ''' </summary>
        Public ReadOnly Property Data As Byte()
            Get
                Return _data
            End Get
        End Property

        Public Property Position As Integer
            Get
                Return _position
            End Get
            Set(value As Integer)
                _position = value
            End Set
        End Property

        Public ReadOnly Property Length As Integer
            Get
                Return _data.Length
            End Get
        End Property

        Public ReadOnly Property Remaining As Integer
            Get
                Return _data.Length - _position
            End Get
        End Property

        Public ReadOnly Property EndOfData As Boolean
            Get
                Return _position >= _data.Length
            End Get
        End Property

        Public Sub Ensure(count As Integer)
            If count < 0 Then
                Throw New ParquetException($"invalid data length: {count}")
            End If
            If _position + count > _data.Length Then
                Throw New ParquetException($"unexpected end of the parquet page data: required {count} bytes at position {_position}, but only {_data.Length - _position} bytes are available!")
            End If
        End Sub

        Public Sub Skip(count As Integer)
            Call Ensure(count)
            _position += count
        End Sub

        Public Function ReadByte() As Byte
            Call Ensure(1)

            Dim value As Byte = _data(_position)
            _position += 1

            Return value
        End Function

        Public Function ReadInt16() As Short
            Call Ensure(2)

            Dim value As Short = BinaryPrimitives.ReadInt16LittleEndian(_data.AsSpan(_position, 2))
            _position += 2

            Return value
        End Function

        Public Function ReadInt32() As Integer
            Call Ensure(4)

            Dim value As Integer = BinaryPrimitives.ReadInt32LittleEndian(_data.AsSpan(_position, 4))
            _position += 4

            Return value
        End Function

        Public Function ReadInt64() As Long
            Call Ensure(8)

            Dim value As Long = BinaryPrimitives.ReadInt64LittleEndian(_data.AsSpan(_position, 8))
            _position += 8

            Return value
        End Function

        Public Function ReadSingle() As Single
            Call Ensure(4)

            Dim value As Single = BinaryPrimitives.ReadSingleLittleEndian(_data.AsSpan(_position, 4))
            _position += 4

            Return value
        End Function

        Public Function ReadDouble() As Double
            Call Ensure(8)

            Dim value As Double = BinaryPrimitives.ReadDoubleLittleEndian(_data.AsSpan(_position, 8))
            _position += 8

            Return value
        End Function

        Public Function ReadBytes(count As Integer) As Byte()
            Dim buffer As Byte() = New Byte(count - 1) {}

            Call ReadBytes(count, buffer, 0)

            Return buffer
        End Function

        ''' <summary>
        ''' 把 <paramref name="count"/> 个字节读入调用方提供的缓冲区，不产生新的数组分配。
        ''' </summary>
        ''' <remarks>
        ''' 该重载不做所有权转移：缓冲区的生命周期由调用方管理。
        ''' </remarks>
        Public Sub ReadBytes(count As Integer, buffer As Byte(), offset As Integer)
            Call Ensure(count)

            If count > 0 Then
                System.Buffer.BlockCopy(_data, _position, buffer, offset, count)
            End If

            _position += count
        End Sub

        Public Function ReadVarint() As ULong
            Dim result As ULong = 0UL
            Dim shift As Integer = 0
            Dim count As Integer = 0

            Do
                Dim b As Byte = ReadByte()

                result = result Or (CULng(b And &H7F) << shift)
                count += 1

                If (b And &H80) = 0 Then
                    Return result
                End If

                shift += 7

                If count >= MaxVarintBytes Then
                    Throw New ParquetException("the parquet varint value is too long, the page data is corrupted!")
                End If
            Loop
        End Function

        Public Function ReadZigZag() As Long
            Dim value As ULong = ReadVarint()
            ' value >> 1 的最高位必然为 0，CLng 与 BitConverter.ToInt64(GetBytes(...)) 的位模式完全一致，
            ' 但无需再分配一个 8 字节的临时数组。
            Dim raw As Long = CLng(value >> 1)

            If (value And 1UL) <> 0UL Then
                raw = Not raw
            End If

            Return raw
        End Function

    End Class

    ''' <summary>
    ''' 面向输出流的字节写入辅助函数（小端字节序）。
    ''' </summary>
    Public Module ByteWriter

        ''' <summary>
        ''' 每个线程复用的 8 字节暂存区。
        ''' </summary>
        ''' <remarks>
        ''' VB.NET 没有 <c>stackalloc</c> 关键字，因此这里使用线程静态暂存区来消除
        ''' <see cref="BitConverter.GetBytes(Integer)"/> 带来的每次调用分配。
        ''' 暂存区只在首次使用时分配，之后按线程复用。
        ''' </remarks>
        <ThreadStatic>
        Private _tlsScratch As Byte()

        ''' <summary>
        ''' 取得线程静态暂存区中长度不小于 <paramref name="minimumLength"/> 的缓冲。
        ''' </summary>
        Friend Function ScratchOf(minimumLength As Integer) As Byte()
            If _tlsScratch Is Nothing OrElse _tlsScratch.Length < minimumLength Then
                _tlsScratch = New Byte(minimumLength - 1) {}
            End If

            Return _tlsScratch
        End Function

        Public Sub WriteInt32LE(stream As Stream, value As Integer)
            Dim buffer As Byte() = ScratchOf(4)

            BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(0, 4), value)
            stream.Write(buffer, 0, 4)
        End Sub

        Public Sub WriteInt64LE(stream As Stream, value As Long)
            Dim buffer As Byte() = ScratchOf(8)

            BinaryPrimitives.WriteInt64LittleEndian(buffer.AsSpan(0, 8), value)
            stream.Write(buffer, 0, 8)
        End Sub

        Public Sub WriteSingleLE(stream As Stream, value As Single)
            Dim buffer As Byte() = ScratchOf(4)

            BinaryPrimitives.WriteSingleLittleEndian(buffer.AsSpan(0, 4), value)
            stream.Write(buffer, 0, 4)
        End Sub

        Public Sub WriteDoubleLE(stream As Stream, value As Double)
            Dim buffer As Byte() = ScratchOf(8)

            BinaryPrimitives.WriteDoubleLittleEndian(buffer.AsSpan(0, 8), value)
            stream.Write(buffer, 0, 8)
        End Sub

        Public Sub WriteVarint(stream As Stream, value As ULong)
            Dim current As ULong = value

            Do
                Dim b As Byte = CByte(current And &H7FUL)

                current >>= 7

                If current <> 0UL Then
                    b = CByte(b Or &H80)
                End If

                stream.WriteByte(b)
            Loop While current <> 0UL
        End Sub

        Public Sub WriteZigZag(stream As Stream, value As Long)
            Dim zz As Long = (value << 1) Xor (value >> 63)

            ' CULng 与 BitConverter.ToUInt64(BitConverter.GetBytes(zz)) 的位模式一致，但不再分配数组。
            Call WriteVarint(stream, CULng(zz))
        End Sub

    End Module

End Namespace
