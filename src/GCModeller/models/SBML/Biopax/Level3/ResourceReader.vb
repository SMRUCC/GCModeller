﻿Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports Microsoft.VisualBasic.Text.Xml
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel

Namespace Level3

    Public Class ResourceReader

        Dim raw As File
        Dim compounds As Dictionary(Of String, Molecule)
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
                Dim compound As Molecule = compounds(refer.resource.Trim("#"c))
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

        Public Iterator Function GetAllCompounds() As IEnumerable(Of MetabolicCompound)
            Dim moleculeReference = raw.SmallMoleculeReference _
                .Select(Function(sm) DirectCast(sm, MoleculeReference)) _
                .JoinIterates(raw.ProteinReference) _
                .ToDictionary(Function(sm)
                                  Return "#" & sm.RDFId
                              End Function)

            For Each compound As Molecule In compounds.Values
                Dim metadata = moleculeReference.TryGetValue(compound.GetEntityResourceId)
                Dim dbLinks As DBLink() = Nothing

                If metadata IsNot Nothing Then
                    dbLinks = metadata.xref _
                        .Select(Function(xr)
                                    Dim xrKey As String = xr.resource.Trim("#"c)
                                    Dim xrData = unificationXrefs(xrKey)
                                    Dim link As New DBLink With {
                                        .DBName = xrData.db,
                                        .entry = xrData.id,
                                        .link = xr.resource
                                    }

                                    Return link
                                End Function) _
                        .ToArray
                End If

                Dim formula As String = Nothing
                Dim mw As Double = 0

                If TypeOf metadata Is SmallMoleculeReference Then
                    formula = DirectCast(metadata, SmallMoleculeReference).chemicalFormula
                    mw = DirectCast(metadata, SmallMoleculeReference).molecularWeight
                End If

                Yield New MetabolicCompound With {
                    .name = compound.displayName,
                    .synonym = compound.name.SafeQuery.Select(Function(name) CStr(name)).ToArray,
                    .formula = formula,
                    .moleculeWeight = mw,
                    .id = compound.RDFId,
                    .xref = dbLinks
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
                Dim desc As String

                If reaction.comment.IsNullOrEmpty Then
                    desc = reaction.name
                Else
                    desc = reaction.comment.Select(Function(c) c.value).JoinBy(vbCrLf)
                End If
                If Not reaction.eCNumber Is Nothing Then
                    ecNumbers = {reaction.eCNumber.value}
                End If

                Yield New MetabolicReaction With {
                    .id = reaction.RDFId,
                    .description = desc.UnescapeHTML,
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
                .raw = file,
                .compounds = New Dictionary(Of String, Molecule)
            }

            Call reader.compounds _
                .AddRange(file.SmallMolecules, Function(m) m.RDFId) _
                .AddRange(file.Protein, Function(m) m.RDFId) _
                .AddRange(file.Complex, Function(m) m.RDFId)

            reader.cellularLocations = file.CellularLocationVocabulary.ToDictionary(Function(c) c.RDFId)
            reader.stoichiometry = file.Stoichiometry.ToDictionary(Function(c) c.RDFId)
            reader.unificationXrefs = file.UnificationXref.ToDictionary(Function(x) x.RDFId)

            Return reader
        End Function

    End Class
End Namespace