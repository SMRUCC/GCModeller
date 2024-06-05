#Region "Microsoft.VisualBasic::f7b70b79e34f3b674ef07004b66955c6, data\Reactome\Owl\ExtractOwl.vb"

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

    '   Total Lines: 234
    '    Code Lines: 171 (73.08%)
    ' Comment Lines: 16 (6.84%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 47 (20.09%)
    '     File Size: 12.73 KB


    ' Module ExtractOwl
    ' 
    '     Function: __fromTempId, __GenerateInputOutput, __getSTO, __process, __trim
    '               ExtractFile, ExtractMetabolites, ExtractReaction, GenerateComplex, GenerateProtein
    '               GenerateSmallMolecule, GenerateTempId, GetId
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.Reactome.ObjectModels
Imports SMRUCC.genomics.Data.Reactome.OwlDocument.Abstract
Imports SMRUCC.genomics.Data.Reactome.OwlDocument.Nodes
Imports SMRUCC.genomics.Data.Reactome.OwlDocument.XrefNodes

<Package("Reactome.ExtractOwl")>
Public Module ExtractOwl

    <ExportAPI("Extract.OwlFile")>
    Public Function ExtractFile(FilePath As String) As BioSystem
        Dim Document As OwlDocument.DocumentFile = OwlDocument.DocumentFile.Load(FilePath)
        Dim rxnEvents As String() = Nothing
        Dim mets = ExtractMetabolites(Document, rxnEvents)
        Dim Model As BioSystem = New BioSystem With {
            .Metabolites = mets,
            .Reactions = ExtractReaction(Document, mets, rxnEvents)
        }

        '  Call AssignCatalystRelationships(Document, Model)

        Return __trim(Model)
    End Function

    Private Function __trim(Model As BioSystem) As BioSystem
        For i As Integer = 0 To Model.Metabolites.Length - 1
            Model.Metabolites(i).Identifier = Regex.Match(Model.Metabolites(i).Identifier, "reactome [^]]+").Value.Split.Last
        Next
        For i As Integer = 0 To Model.Reactions.Length - 1
            Model.Reactions(i).Id = Regex.Match(Model.Reactions(i).Id, "reactome [^]]+").Value.Split.Last
        Next

        Return Model
    End Function

    Private ReadOnly _nonReversible As String() = New String() {"LEFT-TO-RIGHT", "RIGHT-TO-LEFT"}

    Private Function __getSTO(participantStoichiometry As Stoichiometry(), Metabolite As Metabolite) As Integer
        Dim ResourceId As String = Regex.Match(Metabolite.Identifier, "rdf:resource [^]]+").Value.Split.Last
        Dim LQuery = (From item In participantStoichiometry
                      Where String.Equals(ResourceId, item.physicalEntity.GetResourceId)
                      Select item).FirstOrDefault
        If LQuery Is Nothing Then
            ' Throw New Exception(String.Format("The data was broken: could not found stoichiometry value for the specific item {0}", ResourceId))
            Return 1
        Else
            Return CastInteger(LQuery.stoichiometricCoefficient)
        End If
    End Function

    Private Function __GenerateInputOutput(data As RDFresource(), participantStoichiometry As Stoichiometry(), Metabolites As Metabolite()) As CompoundSpecieReference()
        Return (From item In data Let Metabolite = __fromTempId(Metabolites, item.GetResourceId) Select __process(Metabolite, participantStoichiometry)).ToArray
    End Function

    Private Function __process(met As Metabolite, participantStoichiometry As Stoichiometry()) As CompoundSpecieReference
        Dim UniqueId As String = Regex.Match(met.Identifier, "\[reactome .+?\]").Value
        UniqueId = Mid(UniqueId, 2, Len(UniqueId) - 2)
        UniqueId = UniqueId.Split.Last

        If participantStoichiometry.IsNullOrEmpty Then
            Return New CompoundSpecieReference With {.StoiChiometry = 1, .ID = UniqueId}
        End If

        Dim n As Integer = __getSTO(participantStoichiometry, met)
        Return New CompoundSpecieReference With {.StoiChiometry = n, .ID = UniqueId}
    End Function

    <ExportAPI("Extract.Reactions")>
    Public Function ExtractReaction(doc As OwlDocument.DocumentFile, Metabolites As Metabolite(), ReactionId As String()) As Reaction()
        Dim ReactionList As List(Of Reaction) = New List(Of Reaction)

        For Each Model In (From strId As String In ReactionId Select DirectCast(doc.ResourceCollection(strId), BiochemicalReaction)).ToArray
            Dim ReactionObject As Reaction = New Reaction With {.EC = Model.eCNumber, .Comments = Model.Comments}
            Dim Xref = (From ref In Model.Xref Select DirectCast(doc.ResourceCollection(ref.GetResourceId), Xref)).AsList
            Dim st = (From stResource As RDFresource
                      In If(Model.participantStoichiometry.IsNullOrEmpty, New RDFresource() {}, Model.participantStoichiometry)
                      Select DirectCast(doc.ResourceCollection(stResource.GetResourceId), Stoichiometry)).ToArray

            ReactionObject._innerEqur = New ComponentModel.EquaionModel.DefaultTypes.Equation
            ReactionObject._innerEqur.Reversible = Array.IndexOf(_nonReversible, Model.conversionDirection) = -1
            ReactionObject.Id = GenerateTempId(Model.ResourceId, GetId(Xref, "Reactome").FirstOrDefault)
            ReactionObject.Names = Model.displayName
            ReactionObject._innerEqur.Reactants = __GenerateInputOutput(Model.left, st, Metabolites)
            ReactionObject._innerEqur.Products = __GenerateInputOutput(Model.right, st, Metabolites)

            Call ReactionList.Add(ReactionObject)
        Next

        Return ReactionList.ToArray
    End Function

    'Private Sub AssignCatalystRelationships(doc As OwlDocument.DocumentFile, Model As BioSystem)
    '    If doc.Catalysis.IsNullOrEmpty Then
    '        Return
    '    End If

    '    For Each Relationship In doc.Catalysis
    '        Dim Enzyme = DirectCast(doc.ResourceCollection(Relationship.controller.GetResourceId), OwlDocument.DocumentElements.Protein)
    '        Dim Reaction = (From item In Model.Reactions Where String.Equals(Relationship.controlled.GetResourceId, Regex.Match(item.Id, "reactome [^]]+").Value.Split.Last) Select item).FirstOrDefault

    '        Reaction.Enzymes =
    '    Next
    'End Sub

    Private Function __fromTempId(Metabolites As Metabolite(), resourceId As String) As Metabolite
        resourceId = String.Format("[rdf:resource {0}]", resourceId)
        Dim LQuery = (From item In Metabolites Where String.Equals(Regex.Match(item.Identifier, "\[rdf:resource .+?\]").Value, resourceId) Select item).FirstOrDefault
        Return LQuery
    End Function

    Private Function GenerateTempId(ResourceId As String, ReactomeId As String) As String
        If String.IsNullOrEmpty(ReactomeId) Then
            Call Console.WriteLine("ReactomeId is empty for resource {0}!", ResourceId)
            Throw New Exception
        End If

        Return String.Format("[rdf:resource {0}][reactome {1}]", ResourceId, ReactomeId)
    End Function

    <ExportAPI("Extract.Metabolites")>
    Public Function ExtractMetabolites(doc As OwlDocument.DocumentFile, ByRef ExtractedEvents As String()) As Reactome.ObjectModels.Metabolite()
        Dim MetaboliteIdCollection As List(Of String) = New List(Of String)
        Dim ReactionIdList As List(Of String) = New List(Of String)

        For Each ReactionEvent In (From item In doc.BiochemicalReactions Where String.IsNullOrEmpty(item.eCNumber) Select item).ToArray
            If ReactionEvent.left.IsNullOrEmpty OrElse ReactionEvent.right.IsNullOrEmpty Then
                Call Console.WriteLine("{0} is broken", ReactionEvent.ResourceId)
                Continue For
            End If

            Dim IdCollection As List(Of String) = New List(Of String)
            Call IdCollection.AddRange((From item In ReactionEvent.left Select item.GetResourceId).ToArray)
            Call IdCollection.AddRange((From item In ReactionEvent.right Select item.GetResourceId).ToArray)

            If (From strId As String In IdCollection Let ref = doc.ResourceCollection(strId) Where Not (TypeOf ref Is SmallMolecule) Select 1).ToArray.Length = 0 Then
                Call MetaboliteIdCollection.AddRange(IdCollection)
                Call ReactionIdList.Add(ReactionEvent.ResourceId)
            End If
        Next
        MetaboliteIdCollection = (From strId As String In MetaboliteIdCollection Select strId Distinct).AsList
        ExtractedEvents = ReactionIdList.ToArray

        Dim Metabolites = (From strid As String In MetaboliteIdCollection Select doc.ResourceCollection(strid)).ToArray
        Dim MetaboliteList As List(Of Reactome.ObjectModels.Metabolite) = New List(Of ObjectModels.Metabolite)
        For Each item As ResourceElement In Metabolites
            Dim Metabolite As Reactome.ObjectModels.Metabolite = Nothing

            If TypeOf item Is SmallMolecule Then
                Metabolite = GenerateSmallMolecule(DirectCast(item, SmallMolecule), doc)
            ElseIf TypeOf item Is Complex Then
                Metabolite = GenerateComplex(DirectCast(item, Complex), doc)
            ElseIf TypeOf item Is Protein Then
                Metabolite = GenerateProtein(DirectCast(item, Protein), doc)
            Else
                Console.WriteLine("[DEBUG] There is another type!")
            End If

            Call MetaboliteList.Add(Metabolite)
        Next

        Return MetaboliteList.ToArray
    End Function

    <ExportAPI("Creates.Protein")>
    Public Function GenerateProtein(Protein As Protein, doc As OwlDocument.DocumentFile) As ObjectModels.Metabolite
        Dim Xref As List(Of UnificationXref) = New List(Of UnificationXref)
        Call Xref.AddRange((From item In Protein.Xref Select DirectCast(doc.ResourceCollection(item.GetResourceId), UnificationXref)).ToArray)
        Call Xref.AddRange((From item In DirectCast(doc.ResourceCollection(Protein.entityReference.GetResourceId), ProteinReference).Xref
                            Select DirectCast(doc.ResourceCollection(item.GetResourceId), UnificationXref)).ToArray)

        Dim Metabolite As Metabolite = New Metabolite
        '   Metabolite .

        Return Metabolite
    End Function

    Private Function GetId(Xref As Generic.IEnumerable(Of Xref), DB As String) As String()
        Dim LQuery = (From item In Xref Where String.Equals(item.db, DB, StringComparison.OrdinalIgnoreCase) Select item.id Distinct).ToArray
        Return LQuery
    End Function

    <ExportAPI("Creates.SmallMolecule")>
    Public Function GenerateSmallMolecule(SmallMolecule As SmallMolecule, doc As OwlDocument.DocumentFile) As ObjectModels.Metabolite
        If SmallMolecule.entityReference Is Nothing AndAlso SmallMolecule.Xref.IsNullOrEmpty Then
            Throw New Exception
        End If

        Dim Xref As List(Of UnificationXref) = New List(Of UnificationXref)
        Dim Name As String() = Nothing

        If Not SmallMolecule.Xref.IsNullOrEmpty Then
            Call Xref.AddRange((From item In SmallMolecule.Xref Select DirectCast(doc.ResourceCollection(item.GetResourceId), UnificationXref)).ToArray)
        End If

        If Not SmallMolecule.entityReference Is Nothing Then
            Dim Entity = DirectCast(doc.ResourceCollection(SmallMolecule.entityReference.GetResourceId), SmallMoleculeReference)
            Call Xref.AddRange((From ref In Entity.xref Select DirectCast(doc.ResourceCollection(ref.GetResourceId), UnificationXref)).AsList)
            Name = Entity.name
        End If

        Dim Metabolite = New ObjectModels.Metabolite

        Metabolite.ChEBI = GetId(Xref, "CHEBI")
        Metabolite.CommonNames = Name
        Metabolite.MetaboliteType = ObjectModels.Metabolite.MetaboliteTypes.SmallMolecule

        Dim ReactomeId As String = GetId(Xref, "Reactome").FirstOrDefault
        If String.IsNullOrEmpty(ReactomeId) Then ReactomeId = GetId(Xref, "Reactome Database ID Release 50").FirstOrDefault

        Metabolite.Identifier = GenerateTempId(SmallMolecule.ResourceId, ReactomeId)

        Return Metabolite
    End Function

    <ExportAPI("Creates.Complex")>
    Public Function GenerateComplex(Complex As Complex, doc As OwlDocument.DocumentFile) As Metabolite
        ' Dim Entity = DirectCast(doc.ResourceCollection(Complex.entityRefrence.GetResourceId), OwlDocument.DocumentElements.SmallMoleculeReference)
        Dim Xref = (From ref In Complex.Xref Select DirectCast(doc.ResourceCollection(ref.GetResourceId), UnificationXref)).AsList
        '  Call Xref.AddRange((From item In SmallMolecule.Xref Select DirectCast(doc.ResourceCollection(item.GetResourceId), OwlDocument.DocumentElements.UnificationXref)).ToArray)

        Dim Metabolite = New ObjectModels.Metabolite
        Metabolite.ChEBI = GetId(Xref, "CHEBI")
        '  Metabolite.CommonNames = Entity.name
        Metabolite.MetaboliteType = ObjectModels.Metabolite.MetaboliteTypes.Complex
        Metabolite.Identifier = GenerateTempId(Complex.ResourceId, GetId(Xref, "Reactome").FirstOrDefault)
        Metabolite.CommonNames = Complex.displayName

        Return Metabolite
    End Function
End Module
