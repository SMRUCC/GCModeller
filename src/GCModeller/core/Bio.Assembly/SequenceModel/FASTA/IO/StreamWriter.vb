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

        Sub New(s As Stream, Optional lineBreak As Integer = -1)
            Me.file = New IO.StreamWriter(s, Encodings.ASCII.CodePage)
            Me.lineBreak = lineBreak
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Add(seq As FastaSeq)
            Call file.WriteLine(seq.GenerateDocument(lineBreak))
        End Sub

        Public Sub Add(seqs As IEnumerable(Of FastaSeq), Optional filterEmpty As Boolean = False)
            For Each seq As FastaSeq In seqs
                If filterEmpty AndAlso seq.Length = 0 Then
                    Continue For
                End If

                Call Add(seq)
            Next
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