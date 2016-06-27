Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel

Namespace Assembly.NCBI.SequenceDump

    ''' <summary>
    ''' NCBI genbank title format fasta parser
    ''' </summary>
    Public Class Gene : Inherits FASTA.FastaToken

#Region "ReadOnly properties"

        Public ReadOnly Property CommonName As String
        Public ReadOnly Property LocusTag As String
        Public ReadOnly Property Location As NucleotideLocation
#End Region

        Sub New(FastaObj As FASTA.FastaToken)
            Dim strTitle As String = FastaObj.Title
            Dim LocusTag As String = Regex.Match(strTitle, "locus_tag=[^]]+").Value
            Dim Location As String = Regex.Match(strTitle, "location=[^]]+").Value
            Dim CommonName As String = Regex.Match(strTitle, "gene=[^]]+").Value

            Me._LocusTag = LocusTag.Split(CChar("=")).Last
            Me._CommonName = CommonName.Split(CChar("=")).Last
            Me._Location = LociAPI.TryParse(Location)
            Me.Attributes = FastaObj.Attributes
            Me.SequenceData = FastaObj.SequenceData
        End Sub

        Public Overloads Shared Function Load(FastaFile As String) As Gene()
            Dim FASTA As FASTA.FastaFile = SequenceModel.FASTA.FastaFile.Read(FastaFile)
            Dim LQuery = (From FastaObj As FASTA.FastaToken
                          In FASTA
                          Select New Gene(FastaObj)).ToArray
            Return LQuery
        End Function
    End Class
End Namespace