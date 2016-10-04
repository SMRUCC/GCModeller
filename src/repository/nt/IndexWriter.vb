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

        Call $"{EXPORT}/index/{db}".MkDIR
        Call $"{EXPORT}/{db}".MkDIR

        Me.seqDB = New BinaryDataWriter(File.OpenWrite($"{EXPORT}/{db}/{gi}.nt"))
        Me.index = New BinaryDataWriter(File.OpenWrite($"{EXPORT}/index/{db}/{gi}.index"))
    End Sub

    Dim pointer&

    ReadOnly tab As Byte() = Encoding.ASCII.GetBytes(vbTab)
    ReadOnly lf As Byte() = Encoding.ASCII.GetBytes(vbLf)

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

Public MustInherit Class IndexAbstract
    Implements sIdEnumerable
    Implements IDisposable

    Dim __gi As String

    ''' <summary>
    ''' 只读
    ''' </summary>
    ''' <returns></returns>
    Public Property gi As String Implements sIdEnumerable.Identifier
        Get
            Return __gi
        End Get
        Private Set(value As String)
            __gi = value
        End Set
    End Property

    Protected Sub New(uid$)
        gi = uid
    End Sub

    Public Overrides Function ToString() As String
        Return gi
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class