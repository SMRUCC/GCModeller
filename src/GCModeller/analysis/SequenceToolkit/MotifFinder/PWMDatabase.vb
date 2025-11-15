Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Public Class PWMDatabase : Implements IDisposable

    ReadOnly s As StreamPack

    Dim disposedValue As Boolean

    Public ReadOnly Property FamilyList As String()
        Get
            Return s.OpenFolder("/motifs/").files _
                .OfType(Of StreamGroup) _
                .Select(Function(dir) dir.fileName) _
                .ToArray
        End Get
    End Property

    Sub New(s As Stream, Optional is_readonly As Boolean = True)
        Me.s = New StreamPack(s, [readonly]:=is_readonly)
    End Sub

    Public Sub AddPWM(family As String, pwm As IEnumerable(Of Probability))
        For Each model As Probability In pwm.SafeQuery
            Dim json As JsonElement = model.CreateJSONElement
            Dim bson As MemoryStream = BSONFormat.SafeGetBuffer(json)
            Dim file As String = $"/motifs/{family}/{model.name}.motif"

            Call s.Delete(file)

            Using block As Stream = s.OpenBlock(file)
                Call block.Write(bson.ToArray, Scan0, bson.Length)
                Call block.Flush()
            End Using
        Next
    End Sub

    Public Iterator Function LoadFamilyMotifs(family As String) As IEnumerable(Of Probability)
        Dim dir As String = $"/motifs/{family}/"

        For Each file As StreamBlock In s.ListFiles(dir).OfType(Of StreamBlock)
            Dim block As Stream = s.OpenBlock(file)
            Dim json As JsonObject = BSONFormat.Load(block, leaveOpen:=True)
            Dim motif As Probability = json.CreateObject(Of Probability)(decodeMetachar:=False)

            Yield motif
        Next
    End Function

    Public Shared Function LoadMotifs(s As Stream) As Dictionary(Of String, Probability())
        Using db As New PWMDatabase(s, is_readonly:=True)
            Dim dbset As New Dictionary(Of String, Probability())

            For Each name As String In db.FamilyList
                Call dbset.Add(name, db.LoadFamilyMotifs(name).ToArray)
            Next

            Call s.Dispose()

            Return dbset
        End Using
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call s.Dispose()
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
