Imports System.Xml.Serialization
Imports LANS.SystemsBiology.DatabaseServices.StringDB.StrPNet.TCS
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.StrPNet

    <XmlRoot("StrP_Network", Namespace:="http://code.google.com/p/genome-in-code/interaction_network/signal_transduction_network/")>
    Public Class Network : Implements ISaveHandle

        <XmlElement> Public Property Pathway As Pathway()

        Public Function Save(Optional Path As String = "", Optional encoding As Text.Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            Return Me.GetXml.SaveTo(Path, encoding)
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.GetEncodings)
        End Function
    End Class
End Namespace