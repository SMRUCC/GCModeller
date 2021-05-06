Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace v2

    Public Class TranscriptUnit

        <XmlAttribute>
        Public Property id As String

        ''' <summary>
        ''' 基因列表，在这个属性之中定义了该基因组之中的所有基因的数据
        ''' </summary>
        ''' <returns></returns>
        Public Property genes As XmlList(Of gene)

        Public ReadOnly Property numOfGenes As Integer
            Get
                If genes Is Nothing Then
                    Return 0
                Else
                    Return genes.size
                End If
            End Get
        End Property

    End Class
End Namespace