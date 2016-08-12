#Region "Microsoft.VisualBasic::42f38fa1482dc069bb2dbc60828b9287, ..\GCModeller\data\ExternalDBSource\string-db\[xsd]net.sf.psidev.mi\Nodes\Interactor.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace XML

    <XmlType("interactor")> Public Class Interactor : Inherits DataItem
        Implements IReadOnlyId

        <XmlElement("names")> Public Property Names As Names
        <XmlElement("xref")> Public Property Xref As Xref
        <XmlElement("interactorType")> Public Property InteractorType As __interactorType

        Public ReadOnly Property Synonym As String Implements IReadOnlyId.Identity
            Get
                Return Xref.primaryRef.Id
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Xref.primaryRef.ToString
        End Function
    End Class
    Public Class __interactorType
        <XmlElement("names")> Public Property Names As Names
        <XmlElement("xref")> Public Property Xref As Xref
    End Class

    <XmlType("interaction")> Public Class Interaction : Inherits DataItem

        <XmlArray("experimentList")> Public Property ExperimentList As ExperimentRef()

        <XmlType("experimentRef")> Public Class ExperimentRef
            <XmlText> Public Property value As Integer
        End Class

        <XmlArray("participantList")> Public Property ParticipantList As Participant()
        <XmlArray("confidenceList")> Public Property ConfidenceList As confidence()

    End Class

    <XmlType("participant")> Public Class Participant
        <XmlAttribute("id")> Public Property ID As String
        <XmlElement("interactorRef")> Public Property InteractorRef As Integer
    End Class

    Public Class confidence
        <XmlElement("unit")> Public Property Unit As __unit
        <XmlElement> Public Property value As Double

        Public Overrides Function ToString() As String
            Return value
        End Function
    End Class

    <XmlType("unit")> Public Class __unit
        <XmlElement("names")> Public Property Names As Names
    End Class
End Namespace
