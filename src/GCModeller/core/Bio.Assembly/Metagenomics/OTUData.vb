Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Metagenomics

    ''' <summary>
    ''' <see cref="OTUData.Data"/> that associated with <see cref="OTUData.OTU"/> tag
    ''' </summary>
    Public Class OTUData : Implements INamedValue

        ''' <summary>
        ''' ``#OTU_num``
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="#OTU_num")>
        Public Property OTU As String Implements INamedValue.Key
        ''' <summary>
        ''' Usually this property is the BIOM format taxonomy information
        ''' </summary>
        ''' <returns></returns>
        Public Property Taxonomy As String
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