Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports SMRUCC.genomics.Annotation.Ptf

Public Class PtfWriter : Implements IDisposable

    ReadOnly stream As StreamPack
    ReadOnly id_mapping As Dictionary(Of String, Dictionary(Of String, String))

    Private disposedValue As Boolean

    Sub New(file As String, id_mapping As String())
        Me.stream = StreamPack.CreateNewStream(file)
        Me.id_mapping = id_mapping _
            .ToDictionary(Function(dbname) dbname,
                          Function(any)
                              Return New Dictionary(Of String, String)
                          End Function)
    End Sub

    Public Sub AddProtein(protein As ProteinAnnotation)
        Dim file As New BinaryDataWriter(stream.OpenBlock($"/annotation/{protein.geneId}.ptf"))

        Call WriteBytes(file, protein)
        Call file.Flush()
        Call file.Dispose()
    End Sub

    Public Shared Sub WriteBytes(block As BinaryDataWriter, protein As ProteinAnnotation)

    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call stream.Dispose()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
