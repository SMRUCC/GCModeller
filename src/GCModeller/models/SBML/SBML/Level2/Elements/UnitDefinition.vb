Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

Namespace Level2.Elements

    Public Class unitDefinition : Implements IReadOnlyId

        <XmlAttribute> Public Property id As String Implements IReadOnlyId.Identity
        Public Property listOfUnits As List(Of Unit)

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    <XmlType("unit")>
    Public Structure Unit
        <XmlAttribute> Dim kind As String
        <XmlAttribute> Dim scale, exponent As Integer
        <XmlAttribute> Dim multiplier As Double

        Public Overrides Function ToString() As String
            Return String.Format("kind={0}; scale={1}; multipiler={2}; exponent={3}", kind, scale, multiplier, exponent)
        End Function
    End Structure
End Namespace