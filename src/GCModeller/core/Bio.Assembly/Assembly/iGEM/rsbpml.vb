Imports System.Threading
Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.iGEM

    Public Class rsbpml
        Public Property part_list As Part()

        Public Shared Iterator Function FetchByIDList(idlist As IEnumerable(Of String), save$) As IEnumerable(Of String)
            For Each id As String In idlist
                Dim path$ = $"{save}/{id}.Xml"

                If path.FileLength <= 0 Then
                    Dim url$ = $"http://parts.igem.org/cgi/xml/part.cgi?part={id}"
                    Dim xml$ = url.GET

                    Call r.Replace(xml, "<![-][-].*[-][-]>", "").SaveTo(path)
                    Call Thread.Sleep(2000)
                End If

                Yield path
            Next
        End Function
    End Class

    <XmlType("part")> Public Class Part
        Public Property part_id As String
        Public Property part_name As String
        Public Property part_short_name As String
        Public Property part_short_desc As String
        Public Property part_type As String
        Public Property release_status As String
        Public Property sample_status As String
        Public Property part_results As String
        Public Property part_nickname As String
        Public Property part_rating As String
        Public Property part_url As String
        Public Property part_entered As String
        Public Property part_author As String
        ' Public Property deep_subparts As String
        ' Public Property specified_subparts As String
        'Public Property specified_subscars As String
        Public Property sequences As seq_data
        'Public Property features As String
        'Public Property parameters As String
        'Public Property categories As String
        'Public Property samples As String
        'Public Property references As String
        'Public Property groups As String
    End Class

    Public Class seq_data : Implements IPolymerSequenceModel

        <XmlText>
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
    End Class
End Namespace