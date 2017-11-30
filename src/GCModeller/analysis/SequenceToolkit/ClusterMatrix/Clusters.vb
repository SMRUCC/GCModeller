Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Clusters

    ''' <summary>
    ''' Using first token in the fasta title as the sequence uid
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="delimiter$"></param>
    <Extension>
    Public Sub FirstTokenID(ByRef source As FastaFile, Optional delimiter$ = FastaToken.DefaultHeaderDelimiter)
        Dim tokens As Func(Of FastaToken, String())

        If delimiter = FastaToken.DefaultHeaderDelimiter Then
            tokens = Function(f) {
                f.Attributes(Scan0)
            }
        Else
            tokens = Function(f) {
                Strings.Split(f.Title, delimiter)(Scan0)
            }
        End If

        For Each f As FastaToken In source
            f.Attributes = tokens(f)
        Next
    End Sub

    <Extension>
    Public Function KMeans(data As IEnumerable(Of DataSet), Optional expected% = 20) As EntityClusterModel()
        Dim models As EntityClusterModel() = data.ToKMeansModels
        Dim clusters As EntityClusterModel() = models.Kmeans(expected:=expected)
        Return clusters
    End Function
End Module
