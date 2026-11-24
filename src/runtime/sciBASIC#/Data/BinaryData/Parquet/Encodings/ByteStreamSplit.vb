Option Strict On
Option Explicit On
Option Infer On

Imports BufferPool = Microsoft.VisualBasic.Data.IO.BufferPool

Namespace Encodings

    ''' <summary>
    ''' BYTE_STREAM_SPLIT 编码的解码实现（FLOAT / DOUBLE / FIXED_LEN_BYTE_ARRAY）。
    ''' </summary>
    Public Module ByteStreamSplit

        Public Function ReadSingles(reader As ByteReader, count As Integer) As Single()
            Dim total As Integer = count * 4
            Dim data As Byte() = BufferPool.RentBytes(total)

            Try
                Call reader.ReadBytes(total, data, 0)

                Dim result As Single() = New Single(count - 1) {}
                Dim buffer As Byte() = New Byte(3) {}

                For i As Integer = 0 To count - 1
                    For b As Integer = 0 To 3
                        buffer(b) = data(b * count + i)
                    Next

                    result(i) = PlainEncoding.ReadSingleAt(buffer, 0)
                Next

                Return result
            Finally
                BufferPool.ReturnBytes(data)
            End Try
        End Function

        Public Function ReadDoubles(reader As ByteReader, count As Integer) As Double()
            Dim total As Integer = count * 8
            Dim data As Byte() = BufferPool.RentBytes(total)

            Try
                Call reader.ReadBytes(total, data, 0)

                Dim result As Double() = New Double(count - 1) {}
                Dim buffer As Byte() = New Byte(7) {}

                For i As Integer = 0 To count - 1
                    For b As Integer = 0 To 7
                        buffer(b) = data(b * count + i)
                    Next

                    result(i) = PlainEncoding.ReadDoubleAt(buffer, 0)
                Next

                Return result
            Finally
                BufferPool.ReturnBytes(data)
            End Try
        End Function

        Public Function ReadFixedLen(reader As ByteReader, count As Integer, typeLength As Integer) As Byte()()
            If typeLength <= 0 Then
                Throw New ParquetException("the type_length of a FIXED_LEN_BYTE_ARRAY column is missing in the schema!")
            End If

            Dim total As Integer = count * typeLength
            Dim data As Byte() = BufferPool.RentBytes(total)

            Try
                Call reader.ReadBytes(total, data, 0)

                ' 每个元素本身即输出结果，无法复用缓冲；这里复用的是整页的数据中转缓冲。
                Dim result As Byte()() = New Byte(count - 1)() {}

                For i As Integer = 0 To count - 1
                    Dim value As Byte() = New Byte(typeLength - 1) {}

                    For b As Integer = 0 To typeLength - 1
                        value(b) = data(b * count + i)
                    Next

                    result(i) = value
                Next

                Return result
            Finally
                BufferPool.ReturnBytes(data)
            End Try
        End Function

    End Module

End Namespace
