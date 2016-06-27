Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic

Namespace Settings.Programs

    Public Class GCC

        Public Structure Replacement

            <XmlAttribute> Public Property NewReplaced As String
            <XmlAttribute> Public Property Old As String

            Public Overrides Function ToString() As String
                Return String.Format("{0} --> {1}", Old, NewReplaced)
            End Function
        End Structure

        <ProfileNodeItem> <XmlElement> Public Property Filters As List(Of Replacement)
    End Class
End Namespace