Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.ComponentModel.Annotation

Public Class ECNumberWriter : Implements IDisposable

    ReadOnly stream As StreamPack
    ReadOnly rootNames As Dictionary(Of String, String) = Enums(Of EnzymeClasses) _
        .ToDictionary(Function(c) CInt(c).ToString,
                      Function(c)
                          Return CInt(c).ToString & "." & c.Description
                      End Function)
    ReadOnly locations As New Dictionary(Of String, String())

    Private disposedValue As Boolean

    Sub New(file As Stream)
        stream = New StreamPack(file)
    End Sub

    Public Sub AddProtein(protein As entry)
        Dim ECnumbers = protein.xrefs.TryGetValue("EC")

        If Not ECnumbers Is Nothing Then
            Dim subcellularLocation As String() = getTags(protein).ToArray
            Dim seq As String = protein.ProteinSequence
            Dim uniqueId As String = protein.accessions(Scan0)

            For Each number As dbReference In ECnumbers
                Dim ec As String = number.id
                Dim tokens As String() = ec.Split("."c)
                Dim rootName As String = rootNames(tokens(Scan0))
                Dim path As String = $"/{rootName}/{tokens.Skip(1).JoinBy("/")}/{uniqueId}.txt"

                Call stream.WriteText(seq, path, Encodings.ASCII)
            Next

            If subcellularLocation.Length > 0 Then
                Call locations.Add(uniqueId, subcellularLocation)
            End If
        End If
    End Sub

    Private Shared Iterator Function getTags(protein As entry) As IEnumerable(Of String)
        Dim locs = protein.CommentList.TryGetValue("subcellular location")

        If Not locs Is Nothing Then
            For Each item As comment In locs
                For Each loc As subcellularLocation In item.subcellularLocations.SafeQuery
                    If Not loc.locations.IsNullOrEmpty Then
                        For Each tag As value In loc.locations
                            If Not tag.value.StringEmpty Then
                                Yield tag.value
                            End If
                        Next
                    End If
                Next
            Next
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call stream.WriteText(locations.GetJson, "/subcellularLocation.json")
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
