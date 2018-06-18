Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Class IDConvertor

    ReadOnly typesID As New Dictionary(Of IDTypes, NamedVector(Of String)())

    Sub New(entries As IEnumerable(Of entry))

    End Sub

    ''' <summary>
    ''' 我们假设给定的基因列表都是同一种编号类型，所以只要这个列表之中的任意若
    ''' 干个基因和某一类型的编号存在交际，则获取得到这个基因列表的编号类型
    ''' </summary>
    ''' <param name="geneSet"></param>
    ''' <returns></returns>
    Public Overloads Function [GetType](geneSet$(), Optional n% = 5) As IDTypes
        With geneSet.Indexing
            For Each type In typesID
                Dim idSets = type.Value
                Dim idType As IDTypes = type.Key
                Dim i% = 0

                For Each [set] As NamedVector(Of String) In idSets
                    If [set].vector.Any(Function(id) id.IsOneOfA(.ByRef)) Then
                        If i >= n Then
                            Return idType
                        Else
                            i += 1
                        End If
                    End If
                Next
            Next
        End With

        Return IDTypes.NA
    End Function

    ''' <summary>
    ''' 总是将特定类型的基因转换为uniprot编号类型
    ''' </summary>
    ''' <param name="geneSet"></param>
    ''' <param name="type"></param>
    ''' <returns></returns>
    Public Iterator Function Converts(geneSet$(), type As IDTypes) As IEnumerable(Of NamedVector(Of String))
        If type = IDTypes.Accession Then
            For Each id As String In geneSet
                Yield New NamedVector(Of String) With {
                    .name = id,
                    .vector = {id}
                }
            Next
        End If


    End Function
End Class
