Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.genomics.Model.Biopax.EntityProperties

Namespace Level3

    Public Class ResourceReader

        Dim raw As File
        Dim compounds As Dictionary(Of SmallMolecule)
        Dim cellularLocations As Dictionary(Of CellularLocationVocabulary)
        Dim stoichiometry As Dictionary(Of Stoichiometry)
        Dim unificationXrefs As Dictionary(Of UnificationXref)

        Private Sub New()
        End Sub

        Private Iterator Function GetCompoundResource(idSet As IEnumerable(Of EntityProperty), participantStoichiometry As participantStoichiometry()) As IEnumerable(Of CompoundSpecieReference)
            For Each refer As EntityProperty In idSet
                Dim stoichiometry = participantStoichiometry.Where(Function(c) c.resource.StartsWith(refer.resource)).FirstOrDefault
                Dim stoichiometryValue As Double
                Dim compound As SmallMolecule = compounds(refer.resource.Trim("#"c))

                If stoichiometry Is Nothing Then
                    stoichiometryValue = 1
                Else
                    stoichiometryValue = Me.stoichiometry(stoichiometry.resource.Trim("#"c))
                End If

                Yield New CompoundSpecieReference With {
                    .Stoichiometry = stoichiometryValue,
                    .Compartment = cellularLocations(compound.resource).term,
                    .ID = compound.displayName
                }
            Next
        End Function

        Public Iterator Function GetAllReactions() As IEnumerable(Of MetabolicReaction)
            For Each reaction As BiochemicalReaction In raw.BiochemicalReaction.SafeQuery
                Dim ecNumbers As String() = Nothing

                If Not reaction.eCNumber Is Nothing Then
                    ecNumbers = {reaction.eCNumber.value}
                End If

                Yield New MetabolicReaction With {
                    .id = reaction.RDFId,
                    .description = reaction.name,
                    .is_spontaneous = reaction.spontaneous,
                    .name = .description,
                    .is_reversible = reaction.conversionDirection <> "LEFT_TO_RIGHT",
                    .ECNumbers = ecNumbers,
                    .left = GetCompoundResource(reaction.left, reaction.participantStoichiometry).ToArray,
                    .right = GetCompoundResource(reaction.right, reaction.participantStoichiometry).ToArray
                }
            Next
        End Function

        Public Shared Function LoadResource(file As File) As ResourceReader
            Return New ResourceReader With {
                .raw = file,
                .compounds = file.SmallMolecules.ToDictionary,
                .cellularLocations = file.CellularLocationVocabulary.ToDictionary,
                .stoichiometry = file.Stoichiometry.ToDictionary,
                .unificationXrefs = file.UnificationXref.ToDictionary
            }
        End Function

    End Class
End Namespace