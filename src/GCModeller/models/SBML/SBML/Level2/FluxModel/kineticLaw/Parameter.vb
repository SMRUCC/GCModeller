Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ComponentModel.EquaionModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Level2.Elements

    <XmlType("math")> Public Structure MathInfo

        <XmlElement()> Dim ci As String()

        Public Overrides Function ToString() As String
            Return String.Join(vbCrLf, ci)
        End Function
    End Structure

    Public Class parameter : Implements sIdEnumerable

        <XmlAttribute()> Public Property id As String Implements sIdEnumerable.Identifier
        <XmlAttribute()> Public Property value As Double
        <XmlAttribute()> Public Property units As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} = {1} ({2})", id, value, units)
        End Function
    End Class
End Namespace