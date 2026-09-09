Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v2

    Public Class Transportation : Implements INamedValue

        ''' <summary>
        ''' the reaction model reference id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property guid As String Implements INamedValue.Key
        <XmlAttribute> Public Property membrane As String()

        <XmlElement> Public Property enzymes As String()

        Public Overrides Function ToString() As String
            Return $"{enzymes.GetJson}@{membrane.JoinBy(",")}"
        End Function

    End Class
End Namespace