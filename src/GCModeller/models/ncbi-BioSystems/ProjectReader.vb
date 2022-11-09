Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class ProjectReader : Implements IDisposable

    Dim buffer As StreamPack
    Dim proteins As PtfReader

    Private disposedValue As Boolean

    Public ReadOnly Property TotalProteins As Integer
        Get
            Dim text As String = buffer.ReadText("/metadata/count.txt")
            Dim n As Integer = If(text.StringEmpty, 0, Integer.Parse(text.Trim))

            Return n
        End Get
    End Property

    Public ReadOnly Property TotalEnzymes As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return GetEnzymeAnnotation.Keys.Count
        End Get
    End Property

    Sub New(stream As Stream)
        buffer = New StreamPack(stream, [readonly]:=True)
        proteins = New PtfReader(buffer)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetProteinInformation(id As String) As ProteinAnnotation
        Return proteins.GetAnnotation(id)
    End Function

    Public Function GetEnzymeAnnotation() As Dictionary(Of String, String())
        Dim json = buffer.ReadText("/models/ec_numbers.json")
        Dim list = json.LoadJSON(Of Dictionary(Of String, String()))

        Return list
    End Function

    Public Function GetLocationAnnotation() As Dictionary(Of String, String())
        Dim json = buffer.ReadText("/models/subcellular_location.json")
        Dim list = json.LoadJSON(Of Dictionary(Of String, String()))

        Return list
    End Function

    Public Function GetProteinFasta() As IEnumerable(Of FastaSeq)
        Using file As Stream = buffer.OpenBlock("/workspace/protein_set.fasta")
            Dim cacheLines As String() = New StreamReader(file, Encodings.ASCII.CodePage).ReadToEnd.LineTokens
            Dim load As IEnumerable(Of FastaSeq) = FastaFile.DocParser(cacheLines)

            Return load
        End Using
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call buffer.Dispose()
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
