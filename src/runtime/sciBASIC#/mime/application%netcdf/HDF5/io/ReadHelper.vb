
'
' * Some part of this code is copied from NETCDF4 source code.
' * refer : http://www.unidata.ucar.edu
' * 
' * Modified by iychoi@email.arizona.edu
' 


Imports System.IO
Imports Microsoft.VisualBasic.MIME.application.netCDF.HDF5.Structure

Namespace HDF5.IO


    Public Class ReadHelper

        Public Shared Function readO([in] As BinaryReader, sb As Superblock) As Long
            If [in] Is Nothing Then
                Throw New System.ArgumentException("in is null")
            End If

            If sb Is Nothing Then
                Throw New System.ArgumentException("sb is null")
            End If

            Dim sizeOfOffsets As Integer = sb.sizeOfOffsets
            If sizeOfOffsets = 1 Then
                Return [in].readByte()
            ElseIf sizeOfOffsets = 2 Then
                Return [in].readShort()
            ElseIf sizeOfOffsets = 4 Then
                Return [in].readInt()
            ElseIf sizeOfOffsets = 8 Then
                Return [in].readLong()
            End If
            Throw New IOException("size of offsets is not specified")
        End Function

        Public Shared Function readL([in] As BinaryReader, sb As Superblock) As Long
            If [in] Is Nothing Then
                Throw New System.ArgumentException("in is null")
            End If

            If sb Is Nothing Then
                Throw New System.ArgumentException("sb is null")
            End If

            Dim sizeOfLengths As Integer = sb.sizeOfLengths
            If sizeOfLengths = 1 Then
                Return [in].readByte()
            ElseIf sizeOfLengths = 2 Then
                Return [in].readShort()
            ElseIf sizeOfLengths = 4 Then
                Return [in].readInt()
            ElseIf sizeOfLengths = 8 Then
                Return [in].readLong()
            End If
            Throw New IOException("size of lengths is not specified")
        End Function

        Public Shared Function padding(dataLen As Integer, paddingSize As Integer) As Integer
            If dataLen < 0 Then
                Throw New System.ArgumentException("dataLen is negative")
            End If

            If paddingSize <= 0 Then
                Throw New System.ArgumentException("dataLen is 0 or negative")
            End If

            Dim remain As Integer = dataLen Mod paddingSize
            If remain <> 0 Then
                remain = paddingSize - remain
            End If
            Return remain
        End Function

        Public Shared Function getNumBytesFromMax(maxNumber As Long) As Integer
            Dim size As Integer = 0
            While maxNumber <> 0
                size += 1
                ' right shift with zero extension
                maxNumber = CLng(CULng(maxNumber) >> 8)
            End While
            Return size
        End Function

        Public Shared Function readVariableSizeUnsigned([in] As BinaryReader, size As Integer) As Long
            Dim vv As Long
            If size = 1 Then
                vv = unsignedByteToShort([in].readByte())
            ElseIf size = 2 Then
                Dim s As Short = [in].readShort()
                vv = unsignedShortToInt(s)
            ElseIf size = 4 Then
                vv = unsignedIntToLong([in].readInt())
            ElseIf size = 8 Then
                vv = [in].readLong()
            Else
                vv = readVariableSizeN([in], size)
            End If
            Return vv
        End Function

        Public Shared Function readVariableSizeMax([in] As BinaryReader, maxNumber As Integer) As Long
            Dim size As Integer = getNumBytesFromMax(maxNumber)
            Return readVariableSizeUnsigned([in], size)
        End Function

        Private Shared Function readVariableSizeN([in] As BinaryReader, nbytes As Integer) As Long
            Dim ch As Integer() = New Integer(nbytes - 1) {}
            For i As Integer = 0 To nbytes - 1
                ch(i) = [in].readByte()
            Next

            Dim result As Long = ch(nbytes - 1)
            For i As Integer = nbytes - 2 To 0 Step -1
                result = result << 8
                result += ch(i)
            Next

            Return result
        End Function

        Public Shared Function unsignedIntToLong(i As Integer) As Long
            Return If((i < 0), CLng(i) + 4294967296L, CLng(i))
        End Function

        Public Shared Function unsignedShortToInt(s As Short) As Integer
            Return (s And &HFFFF)
        End Function

        Public Shared Function unsignedByteToShort(b As SByte) As Short
            Return CShort(b And &HFF)
        End Function

        Public Shared Function bytesToUnsignedInt(upper As SByte, lower As SByte) As Integer
            Return unsignedByteToShort(upper) * 256 + unsignedByteToShort(lower)
        End Function

        Public Shared Function readString8([in] As BinaryReader) As String
            Dim filePos As Long = [in].offset

            Dim str As String = [in].readASCIIString()

            Dim newFilePos As Long = [in].offset

            Dim readCount As Integer = CInt(newFilePos - filePos)

            ' skip to 8 byte boundary, note zero byte is skipped
            Dim padding As Integer = ReadHelper.padding(readCount, 8)
            [in].skipBytes(padding)

            Return str
        End Function

        Public Shared Function readVariableSizeFactor([in] As BinaryReader, sizeFactor As Integer) As Long
            Dim size As Integer = CInt(Math.Truncate(Math.Pow(2, sizeFactor)))
            Return readVariableSizeUnsigned([in], size)
        End Function
    End Class

End Namespace
