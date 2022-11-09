Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class ProjectWriter : Implements IDisposable

    ReadOnly stream As StreamPack
    ReadOnly proteinWriter As PtfWriter

    Private disposedValue As Boolean

    Sub New(file As Stream)
        stream = New StreamPack(file)
        proteinWriter = New PtfWriter(stream, {})
    End Sub

    Public Sub WriteProject(proj As Project)
        Dim fasta As New List(Of FastaSeq)

        Call stream.WriteText(proj.metadata.GetXml, "/metadata.xml")

        For Each protein As ProteinAnnotation In proj.proteins.proteins.Values
            Call proteinWriter.AddProtein(protein)
            Call fasta.Add(New FastaSeq With {.SequenceData = protein.sequence, .Headers = {protein.geneId}})
        Next

        ' save fasta sequence pack
        Call stream.WriteText(New FastaFile(fasta).Generate, "/workspace/protein_set.fasta", Encodings.ASCII)
    End Sub

    Public Sub WriteMetabolicNetwork(compartment As String, enzymes As IEnumerable(Of Enzyme))
        Dim path As String = $"/metabolic/{compartment}.json"
        Dim json As String = enzymes.ToArray.GetJson

        Call stream.WriteText(json, path)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call proteinWriter.Dispose()
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
