Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv

''' <summary>
''' Using for paired sample T-test
''' </summary>
<Template("ExperimentDesigner")> Public Class SampleTuple

    <XmlAttribute> Public Property Sample1 As String
    <XmlAttribute> Public Property Sample2 As String

    Public Overrides Function ToString() As String
        Return $"{Sample1} vs {Sample2}"
    End Function
End Class
