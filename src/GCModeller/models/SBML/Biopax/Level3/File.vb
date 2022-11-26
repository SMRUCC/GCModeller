#Region "Microsoft.VisualBasic::c1e37a79c1c48ed49be36c25c62669ac, GCModeller\models\SBML\Biopax\Level3\File.vb"

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

    '   Total Lines: 48
    '    Code Lines: 32
    ' Comment Lines: 5
    '   Blank Lines: 11
    '     File Size: 2.39 KB


    '     Class DescriptionData
    ' 
    ' 
    ' 
    '     Class File
    ' 
    '         Properties: BiochemicalReaction, BioSource, Catalysis, CellularLocationVocabulary, Complex
    '                     FragmentFeature, Owl, PhysicalEntity, Protein, ProteinReference
    '                     Provenance, PublicationXref, RelationshipTypeVocabulary, RelationshipXref, SequenceInterval
    '                     SequenceSite, SmallMoleculeReference, SmallMolecules, Stoichiometry, UnificationXref
    ' 
    '         Function: LoadDoc
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Namespace Level3

    Public Class DescriptionData : Inherits Description

    End Class

    ''' <summary>
    ''' Biopax RDF xml file.
    ''' &lt;rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns:bp="http://www.biopax.org/release/biopax-level3.owl#" xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#" xmlns:owl="http://www.w3.org/2002/07/owl#" xmlns:xsd="http://www.w3.org/2001/XMLSchema#" xml:base="http://www.reactome.org/biopax/56/159879#">
    ''' </summary>
    ''' 
    <XmlType("RDF", [Namespace]:=RDFEntity.XmlnsNamespace)>
    Public Class File : Inherits RDF(Of DescriptionData)

        <XmlElement("Ontology")> Public Property Owl As OwlOntology
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

        Public Shared Function LoadDoc(path As String) As File
            Return path.LoadXml(Of File)
        End Function
    End Class
End Namespace
