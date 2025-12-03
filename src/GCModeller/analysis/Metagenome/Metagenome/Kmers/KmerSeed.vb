Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Kmers

    Public Class KmerSeed

        ''' <summary>
        ''' the k-mer sequence string
        ''' </summary>
        ''' <returns></returns>
        Public Property kmer As String
        ''' <summary>
        ''' taxonomy source information
        ''' </summary>
        ''' <returns></returns>
        Public Property source As KmerSource()

        Public ReadOnly Property weight As Double
            Get
                Return 1 / source.Length
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{kmer} has {source.Length} taxonomy source"
        End Function

    End Class

    Public Class KmerSource

        ''' <summary>
        ''' sequence source <see cref="SequenceSource.id"/> of the kmer
        ''' </summary>
        ''' <returns></returns>
        Public Property seqid As UInteger
        ''' <summary>
        ''' size of <see cref="locations"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property count As Integer
        ''' <summary>
        ''' kmer location on the genome sequence of <see cref="SequenceSource"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property locations As UInteger()

        Public Overrides Function ToString() As String
            Return $"#{seqid} = {locations.GetJson}"
        End Function

    End Class

    ''' <summary>
    ''' source information of the target genome source sequence
    ''' </summary>
    Public Class SequenceSource

        ''' <summary>
        ''' the sequence source id
        ''' </summary>
        ''' <returns></returns>
        Public Property id As UInteger
        Public Property ncbi_taxid As Integer
        Public Property accession_id As String
        Public Property name As String

        Public Overrides Function ToString() As String
            Return $"[{id}, ncbi_taxid:{ncbi_taxid}] {name}"
        End Function

    End Class

    Public Class SequenceHit : Inherits SequenceSource

        Public Property reads_title As String
        Public Property identities As Double
        Public Property total As Double
        Public Property score As Double
        Public Property ratio As Double

        Sub New(info As SequenceSource)
            id = info.id
            ncbi_taxid = info.ncbi_taxid
            name = info.name
            accession_id = info.accession_id
        End Sub

        Sub New()
        End Sub

    End Class
End Namespace