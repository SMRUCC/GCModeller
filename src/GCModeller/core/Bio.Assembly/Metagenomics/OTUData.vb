Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Metagenomics

    Public Class OTUData : Implements sIdEnumerable

        <Column(Name:="#OTU_num")> Public Property OTU As String Implements sIdEnumerable.Identifier
        Public Property Data As Dictionary(Of String, String)

        Sub New()
        End Sub

        Sub New(data As OTUData)
            With Me
                .OTU = data.OTU
                .Data = New Dictionary(Of String, String)(data.Data)
            End With
        End Sub

        Public Overrides Function ToString() As String
            Return OTU & " --> " & Data.GetJson
        End Function
    End Class
End Namespace