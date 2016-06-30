Namespace RegulonDB

    <Serializable> Public MustInherit Class ObjectItem
        Protected Friend Sub New()
        End Sub
    End Class

    Public Class Regulator : Inherits LANS.SystemsBiology.ExternalDBSource.RegulonDB.ObjectItem
        <Xml.Serialization.XmlElement("REGULATOR_ID")> Public Property REGULATOR_ID As String
        <Xml.Serialization.XmlElement("PRODUCT_ID")> Public Property PRODUCT_ID As String
        <Xml.Serialization.XmlElement("REGULATOR_NAME")> Public Property REGULATOR_NAME As String
        <Xml.Serialization.XmlElement("REGULATOR_INTERNAL_COMMNET")> Public Property REGULATOR_INTERNAL_COMMNET As String
        <Xml.Serialization.XmlElement("REGULATOR_NOTE")> Public Property REGULATOR_NOTE As String
        <Xml.Serialization.XmlElement("KEY_ID_ORG")> Public Property KEY_ID_ORG As String
    End Class
End Namespace