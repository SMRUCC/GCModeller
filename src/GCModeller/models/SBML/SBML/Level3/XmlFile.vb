Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.SBML.Components
Imports Microsoft.VisualBasic

Namespace Level3

    <XmlRoot("sbml", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class XmlFile
        <XmlAttribute("level")> Public Property Level As Integer
        <XmlAttribute("version")> Public Property Version As Integer
        <XmlElement("model")> Public Property ModelData As Model

        Public Overrides Function ToString() As String
            Return ModelData.ToString
        End Function
    End Class

    <XmlType("model", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class Model : Inherits Components.ModelBase
        '    <XmlElement("notes")> Public Property Notes As Notes
        <XmlArray("listOfCompartments")> Public Property ListOfCompartment As Compartment()
        <XmlArray("listOfSpecies")> Public Property ListOfSpecies As Species()
        <XmlArray("listOfReactions")> Public Property ListOfReactions As Reaction()
    End Class

    <XmlType("compartment", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class Compartment : Inherits Components.Compartment
        <XmlAttribute("size")> Public Property Size As Integer
        <XmlAttribute("constant")> Public Property Constant As Boolean
    End Class

    <XmlType("species", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class Species : Inherits Components.Specie
        <XmlAttribute> Public Property hasOnlySubstanceUnits As Boolean
        <XmlAttribute> Public Property constant As Boolean
    End Class

    <XmlType("reaction", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class Reaction
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property reversible As Boolean
        <XmlElement("notes")> Public Property Notes As Notes

        Public Property listOfReactants As List(Of SpeciesReference)
        Public Property listOfProducts As List(Of SpeciesReference)
        Public Property listOfModifiers As List(Of modifierSpeciesReference)

        '<XmlElement("kineticLaw")> Public Property kineticLaw As kineticLaw

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    Public Class modifierSpeciesReference
        <XmlAttribute> Public Property species As String
        Public Overrides Function ToString() As String
            Return species
        End Function
    End Class

    <XmlType("speciesReference", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class SpeciesReference : Inherits Level2.Elements.speciesReference
        <XmlAttribute("constant")> Public Property Constant As Boolean
    End Class

    Public Class kineticLaw
        <XmlAttribute> Public Property metaid As String
        <XmlAttribute> Public Property sboTerm As String
        <XmlElement("annotation")> Public Property AnnotationData As annotation

        Public Class annotation

            <XmlAttribute("sbrk")> Public Property _sbrk As String
            <XmlAttribute> Public Property bqbiol As String
            <XmlAttribute> Public Property rdf As String
            <XmlElement("sabiork", DataType:="sbrk:sabiork", Namespace:="sbrk")> Public Property sabiorkValue As sabiork

            <XmlType("sabiork", Namespace:="http://sabiork.h-its.org")>
            Public Class sabiork
                <XmlAttribute("kineticLawID", Namespace:="sbrk")> Public Property kineticLawID As Integer
                <XmlElement("experimentalConditions")> Public Property ExperimentConditionsValue As experimentalConditions
                Public Class experimentalConditions
                    <XmlElement("temperature")> Public Property TempratureValue As temperature
                    <XmlElement("pH")> Public Property pHValue As pH
                    <XmlElement> Public Property buffer As String
                End Class
                Public Class temperature
                    Public Property startValueTemperature As Double
                    Public Property temperatureUnit As String
                End Class

                Public Class pH
                    Public Property startValuepH As Double
                End Class
            End Class

            <XmlElement("RDF")> Public Property RDFData As RDF
        End Class

        <XmlElement("math")> Public Property Math As Math
        Public Property listOfLocalParameters As localParameter()
    End Class

    Public Class RDF
        <XmlNamespaceDeclarations()> Public Property rdf As String
        <XmlNamespaceDeclarations()> Public Property bqbiol As String
        <XmlNamespaceDeclarations()> Public Property bqmodel As String
        <XmlElement("Description")> Public Property DescriptionValue As Description

        <XmlType("Description", Namespace:="http://www.w3.org/1999/02/22-rdf-syntax-ns#")>
        Public Class Description
            <XmlAttribute> Public Property about As String
            <XmlElement("isDescribedBy")> Public Property isDescribedBy As isDescribedBy
        End Class

        <XmlType("isDescribedBy", Namespace:="http://biomodels.net/biology-qualifiers/")>
        Public Class isDescribedBy
            Public Property Bag As Bag
        End Class

        <XmlType("Bag", Namespace:="http://www.w3.org/1999/02/22-rdf-syntax-ns#")>
        Public Class Bag
            Public Property li As li
        End Class

        <XmlType("li", Namespace:="http://www.w3.org/1999/02/22-rdf-syntax-ns#")>
        Public Class li
            <XmlAttribute> Public Property resource As String
        End Class
    End Class

    <XmlType("math", Namespace:="http://www.w3.org/1998/Math/MathML")>
    Public Class Math
        <XmlElement("apply")> Public Property applyValue As Apply

        Public Class Apply
            <XmlElement("ci")> Public Property VaslueCollection As String()
        End Class
    End Class

    Public Class localParameter
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property value As Double
        <XmlAttribute> Public Property sboTerm As String
        <XmlAttribute> Public Property units As String
    End Class
End Namespace
