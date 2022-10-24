Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class ECNumberReader : Implements IDisposable

    Private disposedValue As Boolean
    Private ReadOnly stream As StreamPack

    ReadOnly rootNames As Dictionary(Of String, String) = Enums(Of EnzymeClasses) _
        .ToDictionary(Function(c) CInt(c).ToString & "." & c.Description,
                      Function(c)
                          Return CInt(c).ToString
                      End Function)

    Sub New(file As Stream)
        Me.stream = New StreamPack(file, [readonly]:=True)
    End Sub

    Public Iterator Function QueryFasta(Optional q As String = "*") As IEnumerable(Of FastaSeq)
        If q = "*" Then
            For Each file As StreamBlock In stream.files
                Dim seq As String = New StreamReader(stream.OpenBlock(file)).ReadToEnd
                Dim tokens = file.fullName.Trim("/"c).Split("/"c)
                Dim classNumber = rootNames(tokens(0))
                Dim ECNumber = classNumber & "." & tokens.Skip(1).Take(tokens.Length - 2).JoinBy(".")
                Dim id As String = file.fileName.BaseName

                Yield New FastaSeq With {
                    .Headers = {id, ECNumber},
                    .SequenceData = seq
                }
            Next
        Else
            Throw New NotImplementedException
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call stream.Dispose()
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
