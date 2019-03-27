Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.iGEM

    Public Class rsbpml

        Public Property part_list As Part()

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

        <XmlElement("seq_data")>
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
    End Class
End Namespace