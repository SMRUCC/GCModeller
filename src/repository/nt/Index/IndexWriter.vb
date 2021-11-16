#Region "Microsoft.VisualBasic::b0bd861c8177a60d98db09347ac58bfc, nt\Index\IndexWriter.vb"

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

    ' Class IndexWriter
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: Dispose, Write
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump

Public Class IndexWriter : Inherits IndexAbstract

    Dim seqDB As BinaryDataWriter
    Dim index As BinaryDataWriter

    Sub New(EXPORT$, db$, index$)
        MyBase.New(index)

        Call $"{EXPORT}/index/{db}".MakeDir
        Call $"{EXPORT}/{db}".MakeDir

        Me.seqDB = New BinaryDataWriter(File.OpenWrite($"{EXPORT}/{db}/{gi}.nt"))
        Me.index = New BinaryDataWriter(File.OpenWrite($"{EXPORT}/index/{db}/{gi}.index"))
    End Sub

    Dim pointer&

    Public Sub Write(nt$, header As NTheader)
        Dim nt_bufs As Byte() = Encoding.ASCII.GetBytes(nt)
        Dim gi As Byte() = Encoding.ASCII.GetBytes(header.gi)
        Dim start& = pointer + gi.Length + tab.Length

        Call index.Write(start)
        Call index.Write(nt_bufs.Length)

        Call seqDB.Write(gi)
        Call seqDB.Write(tab)
        Call seqDB.Write(nt_bufs)
        Call seqDB.Write(lf)

        pointer = start + nt_bufs.Length + lf.Length
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        Call seqDB.Flush()
        Call seqDB.Close()
        Call seqDB.Dispose()
        Call index.Flush()
        Call index.Close()
        Call index.Dispose()

        MyBase.Dispose(disposing)
    End Sub
End Class
