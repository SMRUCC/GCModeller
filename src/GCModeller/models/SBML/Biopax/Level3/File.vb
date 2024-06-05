﻿#Region "Microsoft.VisualBasic::24c00dbb3d984195a7137884fac38728, models\SBML\Biopax\Level3\File.vb"

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

    '   Total Lines: 66
    '    Code Lines: 51 (77.27%)
    ' Comment Lines: 5 (7.58%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 10 (15.15%)
    '     File Size: 3.47 KB


    '     Class DescriptionData
    ' 
    ' 
    ' 
    '     Class File
    ' 
    '         Properties: base, BiochemicalPathwayStep, BiochemicalReaction, BioSource, Catalysis
    '                     CellularLocationVocabulary, ChemicalStructure, Complex, FragmentFeature, Interaction
    '                     OwlOntology, Pathway, PathwayStep, PhysicalEntity, Protein
    '                     ProteinReference, Provenance, PublicationXref, RelationshipTypeVocabulary, RelationshipXref
    '                     SequenceInterval, SequenceSite, SmallMoleculeReference, SmallMolecules, Stoichiometry
    '                     Transport, TransportWithBiochemicalReaction, UnificationXref
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: LoadDoc
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports Microsoft.VisualBasic.MIME.application.xml

Namespace Level3

    Public Class DescriptionData : Inherits Description

    End Class

    ''' <summary>
    ''' Biopax RDF xml file.
    ''' &lt;rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns:bp="http://www.biopax.org/release/biopax-level3.owl#" xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#" xmlns:owl="http://www.w3.org/2002/07/owl#" xmlns:xsd="http://www.w3.org/2001/XMLSchema#" xml:base="http://www.reactome.org/biopax/56/159879#">
    ''' </summary>
    ''' 
    <XmlRoot("RDF", [Namespace]:="http://smpdb.ca/pathways/#")>
    Public Class File : Inherits RDF(Of DescriptionData)

        <XmlAttribute("base")>
        Public Property base As String

        <XmlElement("Ontology")> Public Property OwlOntology As OwlOntology
        <XmlElement("SmallMolecule")> Public Property SmallMolecules As SmallMolecule()
        <XmlElement> Public Property BiochemicalReaction As BiochemicalReaction()
        <XmlElement> Public Property CellularLocationVocabulary As CellularLocationVocabulary()
        <XmlElement> Public Property SmallMoleculeReference As SmallMoleculeReference()
        <XmlElement> Public Property UnificationXref As UnificationXref()
        <XmlElement> Public Property Provenance As Provenance()
        <XmlElement> Public Property Complex As Complex()
        <XmlElement> Public Property Protein As Protein()
        <XmlElement> Public Property ProteinReference As ProteinReference()
        <XmlElement> Public Property BioSource As BioSource()
        <XmlElement> Public Property FragmentFeature As FragmentFeature()
        <XmlElement> Public Property SequenceInterval As SequenceInterval()
        <XmlElement> Public Property SequenceSite As SequenceSite()
        <XmlElement> Public Property Stoichiometry As Stoichiometry()
        <XmlElement> Public Property Catalysis As Catalysis()
        <XmlElement> Public Property RelationshipXref As RelationshipXref()
        <XmlElement> Public Property RelationshipTypeVocabulary As RelationshipTypeVocabulary()
        <XmlElement> Public Property PublicationXref As PublicationXref()
        <XmlElement> Public Property PhysicalEntity As PhysicalEntity()
        <XmlElement> Public Property PathwayStep As PathwayStep()
        <XmlElement> Public Property Pathway As pathway()
        <XmlElement> Public Property BiochemicalPathwayStep As BiochemicalPathwayStep()
        <XmlElement> Public Property ChemicalStructure As ChemicalStructure()
        <XmlElement> Public Property Interaction As Interaction()
        <XmlElement> Public Property Transport As Transport()
        <XmlElement> Public Property TransportWithBiochemicalReaction As TransportWithBiochemicalReaction()

        Public Const bp As String = "http://www.biopax.org/release/biopax-level3.owl#"
        Public Const owl As String = "http://www.w3.org/2002/07/owl#"

        Sub New()
            Call MyBase.New

            Call xmlns.Add("bp", bp)
            Call xmlns.Add("owl", owl)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadDoc(path As String) As File
            Return GraphWriter.LoadXml(Of File)(path)
        End Function
    End Class
End Namespace
