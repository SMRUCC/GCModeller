Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.IO

Namespace PICRUSt

    Public Class MetaBinaryReader : Implements IDisposable

        ReadOnly buffer As BinaryDataReader
        ReadOnly index As Dictionary(Of String, Long)
        ReadOnly tree As ko_13_5_precalculated

        Dim ko As String()
        Dim disposedValue As Boolean

        Sub New(file As Stream)
            buffer = New BinaryDataReader(file) With {
                .ByteOrder = ByteOrder.BigEndian,
                .Encoding = Encoding.ASCII
            }

            ' verify magic
            If buffer.ReadString(BinaryStringFormat.ZeroTerminated) <> MetaBinaryWriter.Magic Then
                Throw New InvalidDataException("invalid magic header string!")
            Else
                Dim len As Integer = buffer.ReadInt32
                Dim id As New List(Of String)

                For i As Integer = 1 To len
                    Call id.Add(buffer.ReadString(BinaryStringFormat.ZeroTerminated))
                Next

                ko = id.ToArray
            End If

            Call buffer.Seek(buffer.ReadInt64, SeekOrigin.Begin)
            Call loadIndex()
        End Sub

        Private Function loadIndex() As ko_13_5_precalculated
            Dim node As New ko_13_5_precalculated With {
                .Childs = New Dictionary(Of String, Tree(Of Long)),
                .ID = buffer.ReadInt32,
                .label = buffer.ReadString(BinaryStringFormat.ZeroTerminated),
                .taxonomy = buffer.ReadInt32,
                .ggId = buffer.ReadString(BinaryStringFormat.ZeroTerminated),
                .Data = buffer.ReadInt64
            }
            Dim size As Integer = buffer.ReadInt32

            For i As Integer = 1 To size
                Call node.Add(loadIndex())
            Next

            Return node
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    Erase ko

                    ' TODO: dispose managed state (managed objects)
                    Call buffer.Dispose()
                    Call index.Clear()
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