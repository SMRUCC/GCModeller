Imports LANS.SystemsBiology.SequenceModel

Namespace ComponentModel

    Public Interface ISiteReader : Inherits I_PolymerSequenceModel
        ReadOnly Property Distance As Integer
        ReadOnly Property ORF As String
        ReadOnly Property Strand As String
        ReadOnly Property gStart As Integer
        ReadOnly Property gStop As Integer
        ReadOnly Property Family As String
    End Interface
End Namespace