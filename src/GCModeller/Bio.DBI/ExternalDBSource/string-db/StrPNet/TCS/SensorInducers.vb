Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.StrPNet.TCS

    Public Class SensorInducers : Implements sIdEnumerable

        <Column("Sensor")> <XmlAttribute>
        Public Property SensorId As String Implements sIdEnumerable.Identifier
        <Collection("Inducers")> <XmlElement>
        Public Property Inducers As String()

        Public Overrides Function ToString() As String
            Return SensorId
        End Function
    End Class
End Namespace