#Region "Microsoft.VisualBasic::d077eef7ba19009a0d45fb3e6d9d6969, nt\Index\Title.vb"

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

    ' Class TitleWriter
    ' 
    '     Function: Write
    ' 
    '     Sub: Dispose, New
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump

Public Class TitleWriter : Inherits IndexAbstract
    Implements IDisposable

    ReadOnly __titles As BinaryDataWriter
    ReadOnly __index As BinaryDataWriter

    Public Sub New(DATA$, db$, uid$)
        MyBase.New(uid)

        Dim path As New Value(Of String)
        Dim file As FileStream

        Call (path = $"{DATA}/headers/{db}/{uid}.txt").ParentPath.MakeDir

        file = IO.File.OpenWrite(path)
        __titles = New BinaryDataWriter(file, Encodings.ASCII)

        Call (path = $"{DATA}/headers/index/{db}/{uid}.index").ParentPath.MakeDir

        file = IO.File.OpenWrite(path)
        __index = New BinaryDataWriter(file, Encodings.ASCII)
    End Sub

    Dim __pointer&

    ''' <summary>
    ''' ``{gi,len,title}``
    ''' </summary>
    ''' <param name="header"></param>
    ''' <returns></returns>
    Public Function Write(header As NTheader) As Long
        Dim title As Byte() = Encoding.ASCII.GetBytes(header.description)
        Dim gi As Byte() = BitConverter.GetBytes(CLng(header.gi))
        Dim id As Byte() = Encoding.ASCII.GetBytes(header.uid)
        Dim start& = __pointer + id.Length + tab.Length

        Call __index.Write(gi)
        Call __index.Write(start)
        Call __index.Write(title.Length)

        Call __titles.Write(id)
        Call __titles.Write(tab)
        Call __titles.Write(title)
        Call __titles.Write(lf)

        __pointer = start + title.Length + lf.Length

        Return title.Length
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        Call __index.Flush()
        Call __index.Close()
        Call __index.Dispose()
        Call __titles.Flush()
        Call __titles.Close()
        Call __titles.Dispose()

        MyBase.Dispose(disposing)
    End Sub
End Class
