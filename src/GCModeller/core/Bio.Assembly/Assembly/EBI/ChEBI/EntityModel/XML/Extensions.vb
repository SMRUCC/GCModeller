Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Language.UnixBash

Namespace Assembly.EBI.ChEBI.XML

    <XmlRoot("ChEBI-entity-list", [Namespace]:="http://gcmodeller.org/core/chebi/data/")>
    Public Class EntityList

        <XmlElement("chebi-entity")>
        Public Property Array As ChEBIEntity()

        Public Overrides Function ToString() As String
            If Array.IsNullOrEmpty Then
                Return "No items"
            Else
                Return $"list of {Array.Length} chebi entity: ({Array.Take(10).Keys.GetJson}...)"
            End If
        End Function
    End Class

    Public Module Extensions

        ''' <summary>
        ''' 将单个的chebi分子数据文件合并在一个大文件之中，方便进行数据的加载
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        Public Function Compile(DIR$) As EntityList
            Dim list As New Dictionary(Of ChEBIEntity)

            For Each path As String In ls - l - r - "*.XML" <= DIR
                For Each chemical As ChEBIEntity In path.LoadXml(Of ChEBIEntity())
                    If Not list & chemical Then
                        list += chemical
                    End If
                Next
            Next

            Return New EntityList With {
                .Array = list _
                    .Values _
                    .ToArray
            }
        End Function
    End Module
End Namespace