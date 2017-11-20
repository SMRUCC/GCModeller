Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ValueTypes

Public Class PubDate

    <XmlAttribute> Public Property PubStatus As String
    <XmlAttribute> Public Property DateType As String

    Public Property Year As String
    Public Property Month As String
    Public Property Day As String
    Public Property Hour As String
    Public Property Minute As String

    Public Overrides Function ToString() As String
        Return CType(Me, Date).ToString
    End Function

    Public Overloads Shared Narrowing Operator CType(d As PubDate) As Date
        Return New Date(d.Year, DateTimeHelper.GetMonthInteger(d.Month), d.Day)
    End Operator
End Class