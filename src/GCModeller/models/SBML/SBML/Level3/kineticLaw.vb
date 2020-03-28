Imports System.Xml.Serialization

Namespace Level3

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