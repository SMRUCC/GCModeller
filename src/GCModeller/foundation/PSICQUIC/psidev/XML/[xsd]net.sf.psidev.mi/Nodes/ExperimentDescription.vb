#Region "Microsoft.VisualBasic::b0f17a64daae32ef2d6f9f000d954a3f, foundation\PSICQUIC\psidev\XML\[xsd]net.sf.psidev.mi\Nodes\ExperimentDescription.vb"

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

    '   Total Lines: 39
    '    Code Lines: 29
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.56 KB


    '     Class ExperimentDescription
    ' 
    '         Properties: Bibref, HostOrganismList, interactionDetectionMethod, Names, participantIdentificationMethod
    ' 
    '         Function: ToString
    '         Class __bibref
    ' 
    '             Properties: Xref
    ' 
    '         Class __hostOrganism
    ' 
    '             Properties: Names, ncbiTaxId
    ' 
    ' 
    ' 
    '     Class __interactionDetectionMethod
    ' 
    '         Properties: Names, Xref
    ' 
    '     Class __participantIdentificationMethod
    ' 
    '         Properties: Names, Xref
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace XML

    <XmlType("experimentDescription")> Public Class ExperimentDescription : Inherits DataItem

        <XmlElement("names")> Public Property Names As Names
        <XmlElement("bibref")> Public Property Bibref As __bibref

        Public Overrides Function ToString() As String
            Return Names.ToString
        End Function

        Public Class __bibref
            <XmlElement("xref")> Public Property Xref As Xref
        End Class

        <XmlArray("hostOrganismList")> Public Property HostOrganismList As __hostOrganism()

        <XmlType("hostOrganism")> Public Class __hostOrganism
            <XmlAttribute("ncbiTaxId")> Public Property ncbiTaxId As String
            <XmlElement("names")> Public Property Names As Names
        End Class

        Public Property interactionDetectionMethod As __interactionDetectionMethod
        Public Property participantIdentificationMethod As __participantIdentificationMethod
    End Class

    <XmlType("interactionDetectionMethod")> Public Class __interactionDetectionMethod
        <XmlElement("names")> Public Property Names As Names
        <XmlElement("xref")> Public Property Xref As Xref
    End Class

    <XmlType("participantIdentificationMethod")> Public Class __participantIdentificationMethod
        <XmlElement("names")> Public Property Names As Names
        <XmlElement("xref")> Public Property Xref As Xref
    End Class
End Namespace
