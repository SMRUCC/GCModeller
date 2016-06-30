Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace SabiorkKineticLaws.TabularDump

    Public Class EnzymeModifier : Inherits SabiorkEntity
        Implements I_PolymerSequenceModel, sIdEnumerable

        Public Property Uniprot As String
        Public Property CommonName As String
        Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Uniprot, CommonName)
        End Function

        Public Shared Function CreateObjects(SABIORK_DATA As SABIORK) As EnzymeModifier()
            Dim LQuery = (From cs As SBMLParser.CompoundSpecie
                          In SABIORK_DATA.CompoundSpecies
                          Let uniprot = GetIdentifier(cs.Identifiers, "uniprot")
                          Where Not String.IsNullOrEmpty(uniprot)
                          Select New EnzymeModifier With {
                              .CommonName = cs.Name,
                              .Uniprot = uniprot,
                              .SabiorkId = cs.Id}).ToArray
            Return LQuery
        End Function

        Public Function ConvertToFastaObject() As FastaToken
            Return New FastaToken With {
                .SequenceData = SequenceData,
                .Attributes = New String() {Me.Uniprot, Me.CommonName}
            }
        End Function
    End Class
End Namespace