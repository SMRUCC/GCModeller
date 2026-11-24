#Region "Microsoft.VisualBasic::b2baf15b31db79925e47c0ecb84222ae, Data\BinaryData\BinaryData\LZ77Stream\Lzma.vb"

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

    '   Total Lines: 31
    '    Code Lines: 18 (58.06%)
    ' Comment Lines: 7 (22.58%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (19.35%)
    '     File Size: 905 B


    '     Class Lzma
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Decompress
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Option Strict On
Option Explicit On

Imports System.IO

Namespace LZ77Stream

    ''' <summary>
    ''' LZMA (alone format) decompression helper.
    ''' </summary>
    Public NotInheritable Class Lzma

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Decompresses a complete .lzma (alone format) stream read from
        ''' <paramref name="input"/> into a single byte array.
        ''' </summary>
        Public Shared Function Decompress(input As Stream) As Byte()
            If input Is Nothing Then Throw New ArgumentNullException(NameOf(input))

            ' 旧实现使用 MemoryStream 累积再由 ToArray 复制一次；改为池化缓冲增长后按有效长度输出，
            ' 消除内部缓冲的 2 倍扩张分配以及末尾的整块复制。
            Using decoder As New LzmaStream(input, leaveOpen:=True)
                Return BufferPool.ReadToEnd(decoder)
            End Using
        End Function

    End Class
End Namespace
