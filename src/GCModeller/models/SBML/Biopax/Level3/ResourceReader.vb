#Region "Microsoft.VisualBasic::b4043db64ae6d36c3c629d743ef61a12, models\SBML\Biopax\Level3\ResourceReader.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 154
    '    Code Lines: 126 (81.82%)
    ' Comment Lines: 4 (2.60%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 24 (15.58%)
    '     File Size: 7.10 KB


    '     Class ResourceReader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetAllCompounds, GetAllReactions, GetCompoundResource, LoadResource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
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

                Dim cellularLocation As EntityProperties.term = Nothing

                If location IsNot Nothing Then
                    cellularLocation = cellularLocations(location).term
                End If

                Yield New CompoundSpecieReference With {
                    .Stoichiometry = stoichiometryValue,
                    .Compartment = cellularLocation,
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
                    ecNumbers = reaction.eCNumber.value
                End If

                Yield New MetabolicReaction With {
                    .id = If(reaction.about, reaction.RDFId),
                    .description = desc.UnescapeHTML,
                    .is_spontaneous = reaction.spontaneous,
                    .name = reaction.displayName,
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
                .AddRange(file.SmallMolecules, Function(m) If(m.RDFId, m.about)) _
                .AddRange(file.Protein, Function(m) If(m.RDFId, m.about)) _
                .AddRange(file.Complex, Function(m) If(m.RDFId, m.about))

            reader.cellularLocations = file.CellularLocationVocabulary.ToDictionary(Function(c) If(c.RDFId, c.about))
            reader.stoichiometry = file.Stoichiometry.ToDictionary(Function(c) If(c.RDFId, c.about))
            reader.unificationXrefs = file.UnificationXref.ToDictionary(Function(x) If(x.RDFId, x.about))

            Return reader
        End Function

    End Class
End Namespace
