Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.SequenceModel

Namespace Assembly.NCBI.SequenceDump

    ''' <summary>
    ''' NCBI genbank title format fasta parser
    ''' </summary>
    Public Class Protein : Inherits FASTA.FastaToken

        Public Property GI As String
        Public Property Description As String

        Private Shared Function __createObject(Fasta As FASTA.FastaToken) As Protein
            Dim ObjectModel As Protein = New Protein With {
                .SequenceData = Fasta.SequenceData
            }
            ObjectModel.GI = Fasta.Attributes(1)
            ObjectModel.Description = Fasta.Attributes(4).Trim

            Return ObjectModel
        End Function

        Public Shared Function LoadDocument(path As String) As Protein()
            Dim LQuery = (From Fasta As FASTA.FastaToken
                          In FASTA.FastaFile.Read(path).AsParallel
                          Select Protein.__createObject(Fasta)).ToArray
            Return LQuery
        End Function
    End Class
End Namespace