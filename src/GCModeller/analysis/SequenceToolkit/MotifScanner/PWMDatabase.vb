Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Public Class PWMDatabase : Implements IDisposable

    ReadOnly fs As IFileSystemEnvironment

    Dim disposedValue As Boolean

    Public ReadOnly Property FamilyList As String()
        Get
            Dim folder As FileSystemTree = FileSystemTree.BuildTree(fs.GetFiles("/motifs/"))
            Dim subtree = folder.GetFile("motifs").Files
            Dim dirs As FileSystemTree() = subtree.Values _
                .Where(Function(a) a.IsDirectory) _
                .ToArray

            Return dirs _
                .Select(Function(dir) dir.Name) _
                .ToArray
        End Get
    End Property

    Sub New(fs As IFileSystemEnvironment)
        Me.fs = fs
    End Sub

    Public Sub AddPWM(family As String, pwm As IEnumerable(Of Probability))
        For Each model As Probability In pwm.SafeQuery
            Dim json As JsonElement = model.CreateJSONElement
            Dim bson As MemoryStream = BSONFormat.SafeGetBuffer(json)
            Dim file As String = $"/motifs/{family}/{model.name}.motif"

            Call fs.DeleteFile(file)

            Using block As Stream = fs.OpenFile(file, FileMode.OpenOrCreate, FileAccess.Write)
                Call block.Write(bson.ToArray, Scan0, bson.Length)
                Call block.Flush()
            End Using
        Next
    End Sub

    Public Iterator Function LoadFamilyMotifs(family As String) As IEnumerable(Of Probability)
        Dim dir As String = $"/motifs/{family}/"

        For Each file As String In fs.EnumerateFiles(dir, "*.motif")
            Dim block As Stream = fs.OpenFile(file, FileMode.Open, FileAccess.Read)
            Dim json As JsonObject = BSONFormat.Load(block, leaveOpen:=True)
            Dim motif As Probability = json.CreateObject(Of Probability)(decodeMetachar:=False)

            Yield motif
        Next
    End Function

    Public Shared Function LoadMotifs(fs As IFileSystemEnvironment) As Dictionary(Of String, Probability())
        Using db As New PWMDatabase(fs)
            Dim dbset As New Dictionary(Of String, Probability())

            For Each name As String In db.FamilyList
                Call dbset.Add(name, db.LoadFamilyMotifs(name).ToArray)
            Next

            Return dbset
        End Using
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                If CObj(fs).GetType.ImplementInterface(Of IDisposable) Then
                    Call DirectCast(fs, IDisposable).Dispose()
                End If
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
