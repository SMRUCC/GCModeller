Imports System.Text
Imports System.Xml.Serialization
Imports Biopax.MetaCyc.Biopax.Level3.Elements
Imports Microsoft.VisualBasic.DocumentFormat.RDF

Namespace MetaCyc.Biopax.Level3

    ''' <summary>
    ''' Biopax RDF xml file.
    ''' &lt;rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns:bp="http://www.biopax.org/release/biopax-level3.owl#" xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#" xmlns:owl="http://www.w3.org/2002/07/owl#" xmlns:xsd="http://www.w3.org/2001/XMLSchema#" xml:base="http://www.reactome.org/biopax/56/159879#">
    ''' </summary>
    ''' 
    <XmlType(RDF.RDF_PREFIX & "RDF")>
    Public Class File : Inherits RDF

        <XmlElement("Ontology")> Public Property Owl As owlOntology
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
            Return RDF.LoadDocument(Of File)(path, AddressOf __cleanXML)
        End Function

        Private Shared Function __cleanXML(sb As StringBuilder) As String
            Call sb.Replace("<bp:", "<")
            Call sb.Replace("<owl:", "<")
            Call sb.Replace("</bp:", "</")
            Call sb.Replace("</owl:", "</")

            Return sb.ToString
        End Function
    End Class
End Namespace