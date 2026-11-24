#Region "Microsoft.VisualBasic::12d09299255cdc3292adbc0a2c17d0f1, Data\BinaryData\HDF5\device\datasetReader\DatasetReader.vb"

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

    '   Total Lines: 204
    '    Code Lines: 146 (71.57%)
    ' Comment Lines: 29 (14.22%)
    '    - Xml Docs: 72.41%
    ' 
    '   Blank Lines: 29 (14.22%)
    '     File Size: 9.18 KB


    '     Module DatasetReader
    ' 
    '         Function: asDoubleBuffer, asFloatBuffer, asIntBuffer, asLongBuffer, asShortBuffer
    '                   ParseDataChunk, readDataset
    ' 
    '         Sub: fillData, fillDataUnsigned, fillFixedLengthStringData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.HDF5.struct
Imports Microsoft.VisualBasic.Data.IO.HDF5.struct.messages
Imports Microsoft.VisualBasic.Data.IO.HDF5.type

Namespace device

    ''' <summary>
    ''' <para>
    ''' This class handles converting the ByteBuffer obtained from the file
    ''' into a Java array containing the data. It makes use of Java NIO ByteBuffers
    ''' bulk read methods where possible to enable high performance IO.
    ''' </para>
    ''' Some useful information about HDF5 → Java type mappings see:
    ''' <ul>
    ''' <li><a href=
    ''' "https://support.hdfgroup.org/ftp/HDF5/prev-releases/HDF-JAVA/hdfjni-3.2.1/hdf5_java_doc/hdf/hdf5lib/H5.html">HDF5
    ''' Java wrapper H5.java</a></li>
    ''' <li><a href="http://docs.h5py.org/en/stable/faq.html">h5py FAQ</a></li>
    ''' <li><a href=
    ''' "https://docs.oracle.com/javase/tutorial/java/nutsandbolts/datatypes.html">Java
    ''' primitive types</a></li>
    ''' </ul>
    ''' 
    ''' @author James Mudd
    ''' </summary>
    ''' <remarks>
    ''' https://github.com/jamesmudd/jhdf/blob/master/jhdf/src/main/java/io/jhdf/dataset/DatasetReader.java
    ''' </remarks>
    Public Module DatasetReader

        <Extension>
        Public Function readDataset(type As DataType, address&, space As DataspaceMessage, sb As Superblock, dimensions As Integer()) As Object
            Dim buffer As Byte()
            Dim reader As BinaryReader = sb.FileReader(address)

            buffer = reader.readBytes(space.totalLength * type.size)

            Return type.ParseDataChunk(buffer, dimensions)
        End Function

        <Extension>
        Public Function ParseDataChunk(type As DataType, buffer As Byte(), dimensions As Integer()) As Object
            ' If the data is scalar make a fake one element array then remove it at the end
            Dim data As Array
            Dim isScalar As Boolean

            If dimensions.Length = 0 Then
                ' Scalar dataset
                data = Array.CreateInstance(type.TypeInfo, 1)
                isScalar = True
                ' Fake the dimensions
                dimensions = New Integer() {1}
            Else
                If dimensions.Length = 1 Then
                    data = Array.CreateInstance(type.TypeInfo, dimensions)
                Else
                    data = type.TypeInfo.Rectangle(dimensions(Scan0), dimensions(1))
                End If

                isScalar = False
            End If

            If TypeOf type Is FixedPoint Then
                Dim fixedPoint As FixedPoint = DirectCast(type, FixedPoint)
                Dim byteOrder As ByteOrder = fixedPoint.byteOrder

                If fixedPoint.signed Then
                    Select Case fixedPoint.size
                        Case 1 : fillData(data, dimensions, buffer)
                        Case 2 : fillData(data, dimensions, buffer.asShortBuffer(byteOrder))
                        Case 4 : fillData(data, dimensions, buffer.asIntBuffer(byteOrder))
                        Case 8 : fillData(data, dimensions, buffer.asLongBuffer(byteOrder))
                        Case Else
                            Throw New NotSupportedException("Unsupported signed integer type size " & fixedPoint.size & " bytes")
                    End Select
                Else
                    ' Unsigned
                    Select Case fixedPoint.size
                        Case 1 : fillDataUnsigned(data, dimensions, buffer)
                        Case 2 : fillDataUnsigned(data, dimensions, buffer.asShortBuffer(byteOrder))
                        Case 4 : fillDataUnsigned(data, dimensions, buffer.asIntBuffer(byteOrder))
                        Case 8 : fillDataUnsigned(data, dimensions, buffer.asLongBuffer(byteOrder))
                        Case Else
                            Throw New NotSupportedException("Unsupported signed integer type size " & fixedPoint.size & " bytes")
                    End Select
                End If
            ElseIf TypeOf type Is FloatingPoint Then
                Dim floatingPoint As FloatingPoint = DirectCast(type, FloatingPoint)
                Dim byteOrder As ByteOrder = floatingPoint.byteOrder

                Select Case floatingPoint.size
                    Case 4
                        Call fillData(data, dimensions, buffer.asFloatBuffer(byteOrder))
                    Case 8
                        Call fillData(data, dimensions, buffer.asDoubleBuffer(byteOrder))
                    Case Else
                        Throw New NotSupportedException("Unsupported floating point type size " & floatingPoint.size & " bytes")
                End Select
            ElseIf TypeOf type Is StringData Then
                Dim stringData As StringData = DirectCast(type, StringData)
                Dim stringLength As Integer = stringData.size

                fillFixedLengthStringData(data, dimensions, buffer, stringLength)
            Else
                Throw New NotSupportedException("DatasetReader was passed a type it cant fill. Type: " & type.TypeInfo.FullName)
            End If

            If isScalar Then
                Return data.GetValue(Scan0)
            Else
                Return data
            End If
        End Function

        Private Sub fillFixedLengthStringData(data As Array, dims As Integer(), buffer As Byte(), stringLength As Integer)
            If dims.Length > 1 Then
                For i As Integer = 0 To dims(0) - 1
                    Dim newArray As Object = data.GetValue(i)

                    fillFixedLengthStringData(newArray, dims.Skip(1).ToArray, buffer, stringLength)
                Next
            Else
                ' 逐元素直接按偏移量解码：原来的 buffer.Split(stringLength) 会为每个元素分配一个 Byte()，
                ' 百万行级别的字符串 dataset 会产生同样数量的短命数组。
                For i As Integer = 0 To dims(0) - 1
                    Dim start As Integer = i * stringLength
                    Dim count As Integer = System.Math.Min(stringLength, buffer.Length - start)

                    If count > 0 Then
                        Call data.SetValue(Encoding.ASCII.GetString(buffer, start, count).Trim(), i)
                    Else
                        Call data.SetValue(String.Empty, i)
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' 把字节缓冲按照指定元素宽度解码为数值数组。
        ''' </summary>
        ''' <remarks>
        ''' 旧实现使用 <c>buffer.Split(width).Select(...)</c>，会为每一个元素分配一个 Byte()
        ''' 并在元素上直接 <see cref="Array.Reverse(Array)"/>；对于百万元素级别的 dataset，
        ''' 这意味着百万次小数组分配。这里改为复用单个 <paramref name="scratch"/> 缓冲，
        ''' 分配次数从 O(n) 降为 O(1)。
        ''' </remarks>
        <Extension>
        Private Function asFloatBuffer(buffer As Byte(), byteOrder As ByteOrder) As Single()
            Dim n As Integer = buffer.Length \ 4
            Dim values As Single() = New Single(n - 1) {}
            Dim reversed As Boolean = NeedsReversion(byteOrder)
            Dim scratch As Byte() = New Byte(3) {}

            For i As Integer = 0 To n - 1
                Call System.Buffer.BlockCopy(buffer, i * 4, scratch, 0, 4)

                If reversed Then
                    Call Array.Reverse(scratch)
                End If

                values(i) = BitConverter.ToSingle(scratch, Scan0)
            Next

            Return values
        End Function

        <Extension>
        Private Function asDoubleBuffer(buffer As Byte(), byteOrder As ByteOrder) As Double()
            Dim n As Integer = buffer.Length \ 8
            Dim values As Double() = New Double(n - 1) {}
            Dim reversed As Boolean = NeedsReversion(byteOrder)
            Dim scratch As Byte() = New Byte(7) {}

            For i As Integer = 0 To n - 1
                Call System.Buffer.BlockCopy(buffer, i * 8, scratch, 0, 8)

                If reversed Then
                    Call Array.Reverse(scratch)
                End If

                values(i) = BitConverter.ToDouble(scratch, Scan0)
            Next

            Return values
        End Function

        <Extension>
        Private Function asIntBuffer(buffer As Byte(), byteOrder As ByteOrder) As Integer()
            Dim n As Integer = buffer.Length \ 4
            Dim values As Integer() = New Integer(n - 1) {}
            Dim scratch As Byte() = New Byte(3) {}

            For i As Integer = 0 To n - 1
                Call System.Buffer.BlockCopy(buffer, i * 4, scratch, 0, 4)
                values(i) = BinaryReader.ToInteger(scratch, byteOrder)
            Next

            Return values
        End Function

        <Extension>
        Private Function asLongBuffer(buffer As Byte(), byteOrder As ByteOrder) As Long()
            Dim n As Integer = buffer.Length \ 8
            Dim values As Long() = New Long(n - 1) {}
            Dim scratch As Byte() = New Byte(7) {}

            For i As Integer = 0 To n - 1
                Call System.Buffer.BlockCopy(buffer, i * 8, scratch, 0, 8)
                values(i) = BinaryReader.ToLong(scratch, byteOrder)
            Next

            Return values
        End Function

        <Extension>
        Private Function asShortBuffer(buffer As Byte(), byteOrder As ByteOrder) As Short()
            Dim n As Integer = buffer.Length \ 2
            Dim values As Short() = New Short(n - 1) {}
            Dim scratch As Byte() = New Byte(1) {}

            For i As Integer = 0 To n - 1
                Call System.Buffer.BlockCopy(buffer, i * 2, scratch, 0, 2)
                values(i) = BinaryReader.ToShort(scratch, byteOrder)
            Next

            Return values
        End Function

        Private Sub fillData(Of T)(data As Array, dims As Integer(), buffer As T())
            If dims.Length > 1 Then
                For i As Integer = 0 To dims(0) - 1
                    Dim newArray As Array = data.GetValue(i)

                    Call fillData(newArray, dims.Skip(1).ToArray, buffer)
                Next
            Else
                Array.ConstrainedCopy(buffer, Scan0, data, Scan0, buffer.Length)
            End If
        End Sub

        Private Sub fillDataUnsigned(Of T)(data As Array, dims As Integer(), buffer As T())
            ' Signed and unsigned integer types share the same in-memory byte
            ' layout, so a raw byte copy reinterprets each element as the matching
            ' unsigned type correctly (works for 1-D and multi-D arrays alike).
            If buffer Is Nothing OrElse buffer.Length = 0 Then
                Return
            End If

            Dim totalBytes As Integer = System.Buffer.ByteLength(buffer)

            If totalBytes > 0 Then
                Call System.Buffer.BlockCopy(buffer, Scan0, data, Scan0, totalBytes)
            End If
        End Sub
    End Module
End Namespace
