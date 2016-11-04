Imports System.Xml.Serialization
Imports SMRUCC.WebCloud.GIS.MaxMind.Views

Namespace MaxMind

    Public Class FindResult : Inherits GeographicLocation

        <XmlAttribute> Public Property continent_code As String
        <XmlAttribute> Public Property continent_name As String
        <XmlAttribute> Public Property country_iso_code As String
        <XmlAttribute> Public Property country_name As String
        <XmlAttribute> Public Property geoname_id As Long
        <XmlAttribute> Public Property subdivision_1_iso_code As String
        <XmlAttribute> Public Property subdivision_1_name As String
        <XmlAttribute> Public Property subdivision_2_iso_code As String
        <XmlAttribute> Public Property subdivision_2_name As String
        <XmlAttribute> Public Property city_name As String
        <XmlAttribute> Public Property metro_code As String
        <XmlAttribute> Public Property time_zone As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1}/{2}", city_name, subdivision_1_name, subdivision_2_name) & ";      " & String.Format("({0}){1}", country_iso_code, country_name) & "      [" & MyBase.ToString() & "]"
        End Function

        Public Shared Function Null() As FindResult
            Return New FindResult With {
                .city_name = "null",
                .country_name = "null",
                .subdivision_1_name = "null",
                .subdivision_2_name = "null"
            }
        End Function
    End Class
End Namespace