Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Kmers

    Public Interface DatabaseWriter : Inherits IDisposable

        Sub SetKSize(k As Integer)
        ''' <summary>
        ''' add sequence into database
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="taxid"></param>
        Sub Add(seq As IFastaProvider, taxid As UInteger)
        ''' <summary>
        ''' add sequence into database
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="taxid"></param>
        Sub Add(seq As ChunkedNtFasta, taxid As UInteger)

        Function AddSequenceID(taxid As UInteger, name As String) As UInteger

    End Interface
End Namespace