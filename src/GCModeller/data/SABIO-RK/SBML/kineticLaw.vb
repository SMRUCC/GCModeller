#Region "Microsoft.VisualBasic::5c16f8635de8aaeb4d6b5ccceceda040, SBML\SBML\Level3\kineticLaw.vb"

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

    '     Class kineticLaw
    ' 
    '         Properties: AnnotationData, listOfLocalParameters, math, metaid, sboTerm
    '         Class annotation
    ' 
    '             Properties: _sbrk, bqbiol, rdf, sabiorkValue
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
    '     Class localParameter
    ' 
    '         Properties: id, name, sboTerm, units, value
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace SBML

    Public Class kineticLaw
        <XmlAttribute> Public Property metaid As String
        <XmlAttribute> Public Property sboTerm As String

        <XmlElement("annotation")>
        Public Property AnnotationData As annotation

        Public Class annotation

            <XmlAttribute("sbrk")> Public Property _sbrk As String
            <XmlAttribute> Public Property bqbiol As String
            <XmlAttribute> Public Property rdf As String
            <XmlElement("sabiork", DataType:="sbrk:sabiork", Namespace:="sbrk")>
            Public Property sabiorkValue As sabiork

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
        End Class

        Public Property math As Math
        Public Property listOfLocalParameters As localParameter()
    End Class

    Public Class localParameter
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property value As Double
        <XmlAttribute> Public Property sboTerm As String
        <XmlAttribute> Public Property units As String
    End Class
End Namespace
