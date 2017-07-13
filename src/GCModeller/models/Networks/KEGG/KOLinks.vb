Imports Microsoft.VisualBasic.Text.Xml.Models
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB

Public Class KOLinks

    Public Property entry As String
    Public Property name As String
    Public Property definition As String
    Public Property pathways As NamedValue()
    Public Property reactions As String()

    Public Shared Iterator Function Build(ko00001$) As IEnumerable(Of KOLinks)
        For Each path As String In ls - l - r - "*.XML" <= ko00001
            Dim xml As Orthology = path.LoadXml(Of Orthology)

            If xml.Pathway.IsNullOrEmpty Then
                Continue For
            End If

            Dim reactions$() = xml.xRefEntry _
                .Where(Function(l) l.Key = "RN") _
                .Select(Function(x) x.Value2) _
                .ToArray
            Dim pathways As NamedValue() = xml.Pathway _
                .Select(Function(x)
                            Return New NamedValue(x.Key, x.Value.TrimNewLine().Trim)
                        End Function) _
                .ToArray

            Yield New KOLinks With {
                .definition = xml.Definition,
                .entry = xml.Entry,
                .name = xml.Name,
                .pathways = pathways,
                .reactions = reactions
            }
        Next
    End Function
End Class
