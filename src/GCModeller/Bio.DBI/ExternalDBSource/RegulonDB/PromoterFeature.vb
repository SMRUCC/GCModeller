Namespace RegulonDB

    Public Class PromoterFeature : Inherits LANS.SystemsBiology.ExternalDBSource.RegulonDB.ObjectItem
        <Xml.Serialization.XmlElement("PROMOTER_FEATURE_ID")> Public Property PROMOTER_FEATURE_ID As String
        <Xml.Serialization.XmlElement("PROMOTER_ID")> Public Property PROMOTER_ID As String
        <Xml.Serialization.XmlElement("BOX_10_LEFT")> Public Property BOX_10_LEFT As String
        <Xml.Serialization.XmlElement("BOX_10_RIGHT")> Public Property BOX_10_RIGHT As String
        <Xml.Serialization.XmlElement("BOX_35_LEFT")> Public Property BOX_35_LEFT As String
        <Xml.Serialization.XmlElement("BOX_35_RIGHT")> Public Property BOX_35_RIGHT As String
        <Xml.Serialization.XmlElement("BOX_10_SEQUENCE")> Public Property BOX_10_SEQUENCE As String
        <Xml.Serialization.XmlElement("BOX_35_SEQUENCE")> Public Property BOX_35_SEQUENCE As String
        <Xml.Serialization.XmlElement("SCORE")> Public Property SCORE As String
        <Xml.Serialization.XmlElement("RELATIVE_BOX_10_LEFT")> Public Property RELATIVE_BOX_10_LEFT As String
        <Xml.Serialization.XmlElement("RELATIVE_BOX_10_RIGHT")> Public Property RELATIVE_BOX_10_RIGHT As String
        <Xml.Serialization.XmlElement("RELATIVE_BOX_35_LEFT")> Public Property RELATIVE_BOX_35_LEFT As String
        <Xml.Serialization.XmlElement("RELATIVE_BOX_35_RIGHT")> Public Property RELATIVE_BOX_35_RIGHT As String
    End Class
End Namespace