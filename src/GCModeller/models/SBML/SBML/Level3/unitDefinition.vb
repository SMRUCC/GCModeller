Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML.Components

Namespace Level3

    Public Class unitDefinition : Inherits IPartsBase

        Public Property listOfUnits As unit()

    End Class

    Public Class unit

        <XmlAttribute> Public Property scale As Double
        <XmlAttribute> Public Property exponent As Double
        <XmlAttribute> Public Property multiplier As Double
        <XmlAttribute> Public Property kind As String

    End Class
End Namespace