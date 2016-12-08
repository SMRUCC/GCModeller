#Region "Microsoft.VisualBasic::930afb3dd9623c6c97c883eb80c87741, ..\sciBASIC.ComputingServices\ComputingServices\FileSystem\Protocols\FileOpen.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace FileSystem.Protocols

    Public Class RenameArgs
        Public Property old As String
        Public Property [New] As String
    End Class

    Public Class DeleteArgs
        Public Property obj As String
        Public Property [option] As Integer
    End Class

    ''' <summary>
    ''' Initializes a new instance of the System.IO.FileStream class with the specified
    ''' path and creation mode.
    ''' </summary>
    Public Class FileOpen : Inherits FileHandle

        ''' <summary>
        ''' Specifies how the operating system should open a file.
        ''' </summary>
        ''' <returns></returns>
        Public Property Mode As Integer
        Public Property Access As Integer = FileAccess.Read

        Public Overrides Function ToString() As String
            Return $"[{DirectCast(Mode, FileMode).ToString }] " & FileName.ToFileURL
        End Function

        ''' <summary>
        ''' Initializes a new instance of the System.IO.FileStream class with the specified
        ''' path and creation mode.
        ''' </summary>
        Public Function OpenHandle() As FileStream
            Dim mode As FileMode = DirectCast(Me.Mode, FileMode)
            Dim access As FileAccess = DirectCast(Me.Access, FileAccess)
            Return New FileStream(FileName, mode, access)
        End Function
    End Class

    Public Class ReadBuffer : Inherits FileHandle
        Implements IReadWriteBuffer

        Public Property offset As Integer Implements IReadWriteBuffer.offset
        ''' <summary>
        ''' 输入的参数的长度
        ''' </summary>
        ''' <returns></returns>
        Public Property length As Integer Implements IReadWriteBuffer.length

        Sub New()
        End Sub

        Sub New(handle As FileHandle)
            Call MyBase.New(handle)
        End Sub

        Public Function CreateBuffer() As Byte()
            Return CreateBuffer(Me)
        End Function

        Public Shared Function CreateBuffer(op As IReadWriteBuffer) As Byte()
            Dim buffer As Byte() = New Byte(op.length + op.offset - 1) {}
            Return buffer
        End Function
    End Class

    Public Interface IReadWriteBuffer
        Property offset As Integer
        Property length As Integer
    End Interface

    Public Class WriteStream : Inherits RawStream
        Implements IReadWriteBuffer

        Public Property Handle As FileHandle
        Public Property length As Integer Implements IReadWriteBuffer.length
        Public Property offset As Integer Implements IReadWriteBuffer.offset
        Public Property buffer As Byte()

        Sub New()
        End Sub

        Sub New(raw As Byte())
            Dim buf As Byte() = New Byte(INT32 - 1) {}
            Dim p As Integer = Scan0
            Dim handleLen As Integer
            Dim bufferLen As Integer

            Call Array.ConstrainedCopy(raw, p.Move(INT32), buf, Scan0, INT32) : length = BitConverter.ToInt32(buf, Scan0)
            Call Array.ConstrainedCopy(raw, p.Move(INT32), buf, Scan0, INT32) : offset = BitConverter.ToInt32(buf, Scan0)
            Call Array.ConstrainedCopy(raw, p.Move(INT32), buf, Scan0, INT32) : handleLen = BitConverter.ToInt32(buf, Scan0)
            Call Array.ConstrainedCopy(raw, p.Move(INT32), buf, Scan0, INT32) : bufferLen = BitConverter.ToInt32(buf, Scan0)

            buf = New Byte(handleLen - 1) {}
            Call Array.ConstrainedCopy(raw, p.Move(handleLen), buf, Scan0, handleLen)
            Dim json As String = System.Text.Encoding.UTF8.GetString(buf)
            Handle = json.LoadObject(Of FileHandle)
            buffer = New Byte(bufferLen - 1) {}
            Call Array.ConstrainedCopy(raw, p, buffer, Scan0, bufferLen)
        End Sub

        Public Overrides Function Serialize() As Byte()
            Dim handle As Byte() = System.Text.Encoding.UTF8.GetBytes(Me.Handle.GetJson)
            Dim length As Byte() = BitConverter.GetBytes(Me.length)
            Dim offset As Byte() = BitConverter.GetBytes(Me.offset)
            Dim chunkBuffer As Byte() = New Byte(INT32 +  ' length
                                                 INT32 +  ' offset
                                                 INT32 +  ' handle length
                                                 INT32 +  ' buffer length
                                                 handle.Length +
                                                 buffer.Length - 1) {}
            Dim p As Integer = Scan0
            Dim handleLen As Byte() = BitConverter.GetBytes(handle.Length)
            Dim bufferLen As Byte() = BitConverter.GetBytes(buffer.Length)

            Call Array.ConstrainedCopy(length, Scan0, chunkBuffer, p.Move(INT32), INT32)
            Call Array.ConstrainedCopy(offset, Scan0, chunkBuffer, p.Move(INT32), INT32)
            Call Array.ConstrainedCopy(handleLen, Scan0, chunkBuffer, p.Move(INT32), INT32)
            Call Array.ConstrainedCopy(bufferLen, Scan0, chunkBuffer, p.Move(INT32), INT32)
            Call Array.ConstrainedCopy(handle, Scan0, chunkBuffer, p.Move(handle.Length), handle.Length)
            Call Array.ConstrainedCopy(buffer, Scan0, chunkBuffer, p, buffer.Length)

            Return chunkBuffer
        End Function

        Public Function CreateBuffer() As Byte()
            Return ReadBuffer.CreateBuffer(Me)
        End Function
    End Class

    Public Class FileStreamPosition : Inherits FileHandle

        Public Const [GET] As Long = -100

        ''' <summary>
        ''' -100表示获取
        ''' </summary>
        ''' <returns></returns>
        Public Property Position As Long = [GET]

        Sub New(handle As FileHandle)
            Call MyBase.New(handle)
        End Sub

        Sub New()

        End Sub
    End Class
End Namespace
