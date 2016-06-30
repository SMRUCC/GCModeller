Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.StrPNet.TCS

    <XmlType("TCS", Namespace:="http://code.google.com/p/genome-in-code/interaction_network/tcs")> Public Class TCS
        <XmlAttribute> Public Property Chemotaxis As String

        <XmlAttribute> Public Property HK As String
        <XmlAttribute> Public Property RR As String

        <XmlAttribute("Chemotaxis_HK_Confidence")> Public Property ChemotaxisHKConfidence As Double
        <XmlAttribute("HK_RR_Confidence")> Public Property HKRRConfidence As Double
        <XmlAttribute("RR_TF_Confidence")> Public Property RRTFConfidence As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1} -> {2}", Chemotaxis, HK, RR)
        End Function

        Public Function Exists(Id As String) As Boolean
            Return String.Equals(Id, Me.Chemotaxis) OrElse String.Equals(Id, Me.HK) OrElse String.Equals(Id, Me.RR)
        End Function
    End Class
End Namespace