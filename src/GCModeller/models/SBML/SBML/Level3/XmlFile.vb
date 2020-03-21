#Region "Microsoft.VisualBasic::b567f8088cf6f9e970891b0ccd1f64b2, models\SBML\SBML\Level3\XmlFile.vb"

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

    '     Class XmlFile
    ' 
    '         Properties: Level, ModelData, Version
    ' 
    '         Function: ToString
    ' 
    '     Class Model
    ' 
    '         Properties: ListOfCompartment, ListOfReactions, ListOfSpecies
    ' 
    '     Class Compartment
    ' 
    '         Properties: Constant, Size
    ' 
    '     Class Species
    ' 
    '         Properties: constant, hasOnlySubstanceUnits
    ' 
    '     Class Reaction
    ' 
    '         Properties: id, listOfModifiers, listOfProducts, listOfReactants, name
    '                     Notes, reversible
    ' 
    '         Function: ToString
    ' 
    '     Class modifierSpeciesReference
    ' 
    '         Properties: species
    ' 
    '         Function: ToString
    ' 
    '     Class SpeciesReference
    ' 
    '         Properties: Constant
    ' 
    '     Class kineticLaw
    ' 
    '         Properties: AnnotationData, listOfLocalParameters, Math, metaid, sboTerm
    '         Class annotation
    ' 
    '             Properties: _sbrk, bqbiol, rdf, RDFData, sabiorkValue
    '             Class sabiork
    ' 
    '                 Properties: ExperimentConditionsValue, kineticLawID
    '                 Class experimentalConditions
    ' 
    '                     Properties: buffer, pHValue, TempratureValue
    ' 
    '                 Class temperature
    ' 
    '                     Properties: startValueTemperature, temperatureUnit
    ' 
    '                 Class pH
    ' 
    '                     Properties: startValuepH
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    '     Class RDF
    ' 
    '         Properties: bqbiol, bqmodel, DescriptionValue, rdf
    '         Class Description
    ' 
    '             Properties: about, isDescribedBy
    ' 
    '         Class isDescribedBy
    ' 
    '             Properties: Bag
    ' 
    '         Class Bag
    ' 
    '             Properties: li
    ' 
    '         Class li
    ' 
    '             Properties: resource
    ' 
    ' 
    ' 
    '     Class Math
    ' 
    '         Properties: applyValue
    '         Class Apply
    ' 
    '             Properties: VaslueCollection
    ' 
    ' 
    ' 
    '     Class localParameter
    ' 
    '         Properties: id, name, sboTerm, units, value
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML.Components

Namespace Level3

    <XmlRoot("sbml", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class XmlFile
        <XmlAttribute("level")> Public Property Level As Integer
        <XmlAttribute("version")> Public Property Version As Integer
        <XmlElement("model")> Public Property ModelData As Model

        Public Overrides Function ToString() As String
            Return ModelData.ToString
        End Function

        Public Shared Function LoadDocument(xml As String) As XmlFile
            Return xml.LoadXml(Of XmlFile)
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
