Imports System.Text

Namespace HDF5.IO


    Public MustInherit Class BinaryReader
        Protected Friend m_offset As Long
        Protected Friend m_filesize As Long
        Protected Friend m_littleEndian As Boolean
        Protected Friend m_maxOffset As Long

        Public MustOverride Function readByte() As SByte

        Public MustOverride Sub close()

        Public Overridable ReadOnly Property maxOffset() As Long
            Get
                Return Me.m_maxOffset
            End Get
        End Property

        Public Overridable Sub clearMaxOffset()
            Me.m_maxOffset = 0
        End Sub

        Public Overridable Property offset() As Long
            Get
                Return Me.m_offset
            End Get
            Set
                Me.m_offset = Value
            End Set
        End Property

        Public Overridable ReadOnly Property size() As Long
            Get
                Return Me.m_filesize
            End Get
        End Property

        Public Overridable Sub setLittleEndian()
            Me.m_littleEndian = True
        End Sub

        Public Overridable Sub setBigEndian()
            Me.m_littleEndian = False
        End Sub

        Public Overridable ReadOnly Property littleEndian() As Boolean
            Get
                Return Me.m_littleEndian
            End Get
        End Property

        Public Overridable ReadOnly Property bigEndian() As Boolean
            Get
                Return Not Me.m_littleEndian
            End Get
        End Property

        Public Overridable Function readBytes(n As Integer) As SByte()
            If n < 0 Then
                Throw New System.ArgumentException("n should be greater than 0")
            End If

            Dim buf As SByte() = New SByte(n - 1) {}
            For i As Integer = 0 To n - 1
                buf(i) = readByte()
            Next
            Return buf
        End Function


        Public Overridable Sub skipBytes(n As Integer)
            If n < 0 Then
                Throw New System.ArgumentException("n should be greater than 0")
            End If

            For i As Integer = 0 To n - 1
                readByte()
            Next
        End Sub


        Public Overridable Function readInt() As Integer
            Dim data As SByte() = readBytes(4)
            Dim temp As Integer = 0

            If Me.m_littleEndian Then
                temp = (data(0) And &HFF)
                temp = temp Or (data(1) And &HFF) << 8
                temp = temp Or (data(2) And &HFF) << 16
                temp = temp Or (data(3) And &HFF) << 24
            Else
                temp = (data(0) And &HFF) << 24
                temp = temp Or (data(1) And &HFF) << 16
                temp = temp Or (data(2) And &HFF) << 8
                temp = temp Or (data(3) And &HFF)
            End If
            Return temp
        End Function


        Public Overridable Function readLong() As Long
            Dim data As SByte() = readBytes(8)
            Dim temp As Long = 0

            If Me.m_littleEndian Then
                temp = (data(0) And &HFF)
                temp = temp Or (data(1) And &HFF) << 8
                temp = temp Or (data(2) And &HFF) << 16
                temp = temp Or (data(3) And &HFF) << 24
                temp = temp Or (data(4) And &HFF) << 32
                temp = temp Or (data(5) And &HFF) << 40
                temp = temp Or (data(6) And &HFF) << 48
                temp = temp Or (data(7) And &HFF) << 56
            Else
                temp = (data(0) And &HFF) << 56
                temp = temp Or (data(1) And &HFF) << 48
                temp = temp Or (data(2) And &HFF) << 40
                temp = temp Or (data(3) And &HFF) << 32
                temp = temp Or (data(4) And &HFF) << 24
                temp = temp Or (data(5) And &HFF) << 16
                temp = temp Or (data(6) And &HFF) << 8
                temp = temp Or (data(7) And &HFF)
            End If
            Return temp
        End Function


        Public Overridable Function readShort() As Short
            Dim data As SByte() = readBytes(2)
            Dim temp As Short = 0

            If Me.m_littleEndian Then
                temp = CShort(data(0) And &HFF)
                temp = temp Or CShort((data(1) And &HFF) << 8)
            Else
                temp = CShort((data(0) And &HFF) << 8)
                temp = temp Or CShort(data(1) And &HFF)
            End If
            Return temp
        End Function


        Public Overridable Function readASCIIString() As String
            Dim sb As New StringBuilder()

            For i As Long = Me.m_offset To Me.m_filesize - 1
                Dim c As Char = ChrW(readByte())
                If c = ControlChars.NullChar Then
                    Exit For
                Else
                    sb.Append(c)
                End If
            Next

            Return sb.ToString()
        End Function

        Public Overridable Function readASCIIString(length As Integer) As String
            Dim sb As New StringBuilder()
            Dim nCount As Integer = 0

            For i As Long = 0 To length - 1
                Dim c As Char = ChrW(readByte())
                nCount += 1
                If c = ControlChars.NullChar Then
                    Exit For
                Else
                    sb.Append(c)
                End If
            Next

            If nCount < length Then
                skipBytes(length - nCount)
            End If

            Return sb.ToString()
        End Function
    End Class

End Namespace
