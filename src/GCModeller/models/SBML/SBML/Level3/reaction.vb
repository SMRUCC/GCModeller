Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML.Components

Namespace Level3

    <XmlType("reaction", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class Reaction : Inherits IPartsBase

        <XmlAttribute> Public Property reversible As Boolean
        <XmlAttribute> Public Property fast As Boolean

        Public Property notes As Notes
        Public Property annotation As annotation

        Public Property listOfReactants As List(Of SpeciesReference)
        Public Property listOfProducts As List(Of SpeciesReference)
        Public Property listOfModifiers As List(Of modifierSpeciesReference)

        '<XmlElement("kineticLaw")> Public Property kineticLaw As kineticLaw

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    Public Class modifierSpeciesReference

        <XmlAttribute>
        Public Property species As String

        Public Overrides Function ToString() As String
            Return species
        End Function
    End Class

    <XmlType("speciesReference", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class SpeciesReference : Inherits Level2.Elements.speciesReference
        <XmlAttribute("constant")> Public Property Constant As Boolean
    End Class
End Namespace