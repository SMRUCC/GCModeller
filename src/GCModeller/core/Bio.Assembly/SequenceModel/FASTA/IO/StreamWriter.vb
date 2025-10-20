#Region "Microsoft.VisualBasic::1a8c691aee8a0da0c1294915e84171fd, core\Bio.Assembly\SequenceModel\FASTA\IO\StreamWriter.vb"

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

    '   Total Lines: 63
    '    Code Lines: 39 (61.90%)
    ' Comment Lines: 13 (20.63%)
    '    - Xml Docs: 23.08%
    ' 
    '   Blank Lines: 11 (17.46%)
    '     File Size: 2.31 KB


    '     Class StreamWriter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: (+2 Overloads) Add, (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text

Namespace SequenceModel.FASTA

    ''' <summary>
    ''' A fasta stream writer, apply for write a huge fasta seqnce collection
    ''' </summary>
    Public Class StreamWriter : Implements IDisposable

        Dim disposedValue As Boolean
        Dim file As System.IO.StreamWriter
        Dim lineBreak As Integer = -1
        Dim deli As String

        Sub New(s As Stream, Optional lineBreak As Integer = -1, Optional deli As String = "|")
            Me.file = New IO.StreamWriter(s, Encodings.ASCII.CodePage)
            Me.deli = deli
            Me.lineBreak = lineBreak
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Add(seq As FastaSeq)
            Call file.WriteLine(seq.GenerateDocument(lineBreak, delimiter:=deli))
        End Sub

        Public Sub Add(seqs As IEnumerable(Of FastaSeq), Optional filterEmpty As Boolean = False)
            For Each seq As FastaSeq In seqs
                If filterEmpty AndAlso seq.Length = 0 Then
                    Continue For
                End If

                Call Add(seq)
            Next
        End Sub

        Public Shared Sub WriteList(seqs As IDictionary(Of String, String), file As Stream, Optional lineBreak As Integer = -1)
            Using fasta As New StreamWriter(file, lineBreak)
                For Each seq As KeyValuePair(Of String, String) In seqs
                    Call fasta.Add(New FastaSeq(seq.Value, title:=seq.Key))
                Next
            End Using
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call file.Flush()
                    Call file.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
