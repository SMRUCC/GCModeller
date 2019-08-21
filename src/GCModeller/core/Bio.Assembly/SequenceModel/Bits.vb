Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace SequenceModel

    ''' <summary>
    ''' Sequence model in bytes
    ''' </summary>
    Public Class Bits : Implements IFastaProvider

        ReadOnly bytes As Byte()

        Public ReadOnly Property seqType As SeqTypes
        Public ReadOnly Property title As String Implements IFastaProvider.title

        Private Sub New(title$, type As SeqTypes, bytes As Byte())
            Me.bytes = bytes
            Me.seqType = type
            Me.title = title
        End Sub

        Public Overrides Function ToString() As String
            Return $"{seqType.Description} {title}"
        End Function

        Public Shared Function FromNucleotide(nucl As IAbstractFastaToken) As Bits
            Return New Bits(
                title:=nucl.title,
                type:=SeqTypes.DNA,
                bytes:=NucleicAcid.Enums(nucl.SequenceData).ToArray
            )
        End Function

        Public Shared Function FromPolypeptide(prot As IAbstractFastaToken) As Bits
            Return New Bits(
                title:=prot.title,
                type:=SeqTypes.Protein,
                bytes:=Polypeptide.ConstructVector(prot.SequenceData).ToArray
            )
        End Function

        Private Function GetSequenceData() As String Implements ISequenceProvider.GetSequenceData
            Select Case seqType
                Case SeqTypes.DNA
                    Return NucleicAcid.ToString(bytes)
                Case SeqTypes.Protein
                    Return Polypeptide.ToString(bytes)
                Case Else
                    Throw New NotImplementedException(seqType.Description)
            End Select
        End Function
    End Class
End Namespace