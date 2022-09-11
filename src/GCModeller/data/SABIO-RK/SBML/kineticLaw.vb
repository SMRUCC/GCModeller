#Region "Microsoft.VisualBasic::71dfa12d7ee4eb22d3892f5b8f748760, GCModeller\data\SABIO-RK\SBML\kineticLaw.vb"

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

    '   Total Lines: 73
    '    Code Lines: 58
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 2.76 KB


    '     Class kineticLaw
    ' 
    '         Properties: annotation, listOfLocalParameters, math, metaid, sboTerm
    ' 
    '     Class kineticLawAnnotation
    ' 
    '         Properties: RDF, sabiork
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class sabiorkAnnotation
    ' 
    '         Properties: experimentalConditions, kineticLawID
    ' 
    '     Class experimentalConditions
    ' 
    '         Properties: buffer, pHValue, temperature
    ' 
    '     Class temperature
    ' 
    '         Properties: startValueTemperature, temperatureUnit
    ' 
    '     Class pH
    ' 
    '         Properties: startValuepH
    ' 
    '     Class localParameter
    ' 
    '         Properties: id, name, sboTerm, units, value
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports SMRUCC.genomics.Model.SBML.Level3

Namespace SBML

    Public Class kineticLaw
        <XmlAttribute> Public Property metaid As String
        <XmlAttribute> Public Property sboTerm As String

        Public Property annotation As kineticLawAnnotation
        <XmlElement("math", Namespace:="http://www.w3.org/1998/Math/MathML")>
        Public Property math As Math
        Public Property listOfLocalParameters As localParameter()
    End Class

    Public Class kineticLawAnnotation

        <XmlElement("sabiork", [Namespace]:="http://sabiork.h-its.org")>
        Public Property sabiork As sabiorkAnnotation
        <XmlElement("RDF", [Namespace]:=RDFEntity.XmlnsNamespace)>
        Public Property RDF As AnnotationInfo

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            xmlns.Add("rdf", RDFEntity.XmlnsNamespace)
            xmlns.Add("sbrk", "http://sabiork.h-its.org")
        End Sub
    End Class

    <XmlType("sabiork", Namespace:="http://sabiork.h-its.org")>
    Public Class sabiorkAnnotation
        <XmlElement("kineticLawID", Namespace:="http://sabiork.h-its.org")>
        Public Property kineticLawID As Integer

        <XmlElement("experimentalConditions", [Namespace]:="http://sabiork.h-its.org")>
        Public Property experimentalConditions As experimentalConditions

    End Class

    Public Class experimentalConditions
        <XmlElement("temperature", [Namespace]:="http://sabiork.h-its.org")>
        Public Property temperature As temperature
        <XmlElement("pH", [Namespace]:="http://sabiork.h-its.org")>
        Public Property pHValue As pH

        <XmlElement("buffer", [Namespace]:="http://sabiork.h-its.org")>
        Public Property buffer As String
    End Class

    Public Class temperature
        <XmlElement([Namespace]:="http://sabiork.h-its.org")>
        Public Property startValueTemperature As Double
        <XmlElement([Namespace]:="http://sabiork.h-its.org")>
        Public Property temperatureUnit As String
    End Class

    Public Class pH
        <XmlElement([Namespace]:="http://sabiork.h-its.org")>
        Public Property startValuepH As Double
    End Class

    Public Class localParameter
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property value As Double
        <XmlAttribute> Public Property sboTerm As String
        <XmlAttribute> Public Property units As String
    End Class
End Namespace
