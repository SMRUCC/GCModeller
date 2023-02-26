Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.genomics.Model.Biopax.EntityProperties

Namespace Level3

    Public Class ResourceReader

        Dim raw As File
        Dim compounds As Dictionary(Of String, SmallMolecule)
        Dim cellularLocations As Dictionary(Of String, CellularLocationVocabulary)
        ''' <summary>
        ''' a subset of the reaction data should be indexed 
        ''' by <see cref="Stoichiometry.physicalEntity"/> 
        ''' </summary>
        Dim stoichiometry As Dictionary(Of String, Stoichiometry)
        Dim unificationXrefs As Dictionary(Of String, UnificationXref)

        Private Sub New()
        End Sub

        Private Iterator Function GetCompoundResource(idSet As IEnumerable(Of EntityProperty), participantStoichiometry As Dictionary(Of String, Stoichiometry)) As IEnumerable(Of CompoundSpecieReference)
            For Each refer As EntityProperty In idSet
                Dim stoichiometry = participantStoichiometry.TryGetValue(refer.resource)
                Dim stoichiometryValue As Double
                Dim compound As SmallMolecule = compounds(refer.resource.Trim("#"c))
                Dim location As String = compound.cellularLocation?.resource

                If Not location Is Nothing Then
                    location = location.Trim("#"c)
                End If
                If stoichiometry Is Nothing Then
                    stoichiometryValue = 1
                Else
                    stoichiometryValue = stoichiometry.stoichiometricCoefficient
                End If

                Yield New CompoundSpecieReference With {
                    .Stoichiometry = stoichiometryValue,
                    .Compartment = cellularLocations(location).term,
                    .ID = compound.displayName
                }
            Next
        End Function

        Public Iterator Function GetAllReactions() As IEnumerable(Of MetabolicReaction)
            For Each reaction As BiochemicalReaction In raw.BiochemicalReaction.SafeQuery
                Dim ecNumbers As String() = Nothing
                Dim participantStoichiometry = reaction.participantStoichiometry _
                    .Select(Function(ref) Me.stoichiometry(ref.resource.Trim("#"c))) _
                    .ToDictionary(Function(c)
                                      Return c.physicalEntity.resource
                                  End Function)

                If Not reaction.eCNumber Is Nothing Then
                    ecNumbers = {reaction.eCNumber.value}
                End If

                Yield New MetabolicReaction With {
                    .id = reaction.RDFId,
                    .description = reaction.name,
                    .is_spontaneous = reaction.spontaneous,
                    .name = .description,
                    .is_reversible = reaction.conversionDirection = "REVERSIBLE",
                    .ECNumbers = ecNumbers,
                    .left = GetCompoundResource(reaction.left, participantStoichiometry).ToArray,
                    .right = GetCompoundResource(reaction.right, participantStoichiometry).ToArray
                }
            Next
        End Function

        Public Shared Function LoadResource(file As File) As ResourceReader
            Dim reader As New ResourceReader With {
                .raw = file
            }

            reader.compounds = file.SmallMolecules.ToDictionary(Function(m) m.RDFId)
            reader.cellularLocations = file.CellularLocationVocabulary.ToDictionary(Function(c) c.RDFId)
            reader.stoichiometry = file.Stoichiometry.ToDictionary(Function(c) c.RDFId)
            reader.unificationXrefs = file.UnificationXref.ToDictionary(Function(x) x.RDFId)

            Return reader
        End Function

    End Class
End Namespace