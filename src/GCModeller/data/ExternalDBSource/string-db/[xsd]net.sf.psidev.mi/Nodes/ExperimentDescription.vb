Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace StringDB.MIF25.Nodes

    <XmlType("experimentDescription")> Public Class ExperimentDescription : Inherits StringDB.MIF25.XmlCommon.DataItem

        <XmlElement("names")> Public Property Names As StringDB.MIF25.XmlCommon.Names
        <XmlElement("bibref")> Public Property Bibref As __bibref

        Public Overrides Function ToString() As String
            Return Names.ToString
        End Function

        Public Class __bibref
            <XmlElement("xref")> Public Property Xref As StringDB.MIF25.XmlCommon.Xref
        End Class

        <XmlArray("hostOrganismList")> Public Property HostOrganismList As __hostOrganism()

        <XmlType("hostOrganism")> Public Class __hostOrganism
            <XmlAttribute("ncbiTaxId")> Public Property ncbiTaxId As String
            <XmlElement("names")> Public Property Names As StringDB.MIF25.XmlCommon.Names
        End Class

        Public Property interactionDetectionMethod As __interactionDetectionMethod
        Public Property participantIdentificationMethod As __participantIdentificationMethod
    End Class

    <XmlType("interactionDetectionMethod")> Public Class __interactionDetectionMethod
        <XmlElement("names")> Public Property Names As StringDB.MIF25.XmlCommon.Names
        <XmlElement("xref")> Public Property Xref As StringDB.MIF25.XmlCommon.Xref
    End Class

    <XmlType("participantIdentificationMethod")> Public Class __participantIdentificationMethod
        <XmlElement("names")> Public Property Names As StringDB.MIF25.XmlCommon.Names
        <XmlElement("xref")> Public Property Xref As StringDB.MIF25.XmlCommon.Xref
    End Class
End Namespace