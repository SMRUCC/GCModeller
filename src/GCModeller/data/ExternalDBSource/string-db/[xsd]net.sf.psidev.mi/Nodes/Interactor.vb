Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace StringDB.MIF25.Nodes

    <XmlType("interactor")> Public Class Interactor : Inherits StringDB.MIF25.XmlCommon.DataItem
        Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IReadOnlyId

        <XmlElement("names")> Public Property Names As StringDB.MIF25.XmlCommon.Names
        <XmlElement("xref")> Public Property Xref As StringDB.MIF25.XmlCommon.Xref
        <XmlElement("interactorType")> Public Property InteractorType As __interactorType

        Public ReadOnly Property Synonym As String Implements IReadOnlyId.Identity
            Get
                Return Xref.PrimaryReference.Id
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Xref.PrimaryReference.ToString
        End Function
    End Class
    Public Class __interactorType
        <XmlElement("names")> Public Property Names As StringDB.MIF25.XmlCommon.Names
        <XmlElement("xref")> Public Property Xref As StringDB.MIF25.XmlCommon.Xref
    End Class

    <XmlType("interaction")> Public Class Interaction : Inherits StringDB.MIF25.XmlCommon.DataItem

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
        <XmlElement("names")> Public Property Names As StringDB.MIF25.XmlCommon.Names
    End Class
End Namespace