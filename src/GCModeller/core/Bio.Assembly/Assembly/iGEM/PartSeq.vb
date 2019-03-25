Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.iGEM

    Public Class PartSeq : Implements IPolymerSequenceModel

        Public Property PartName As String
        Public Property Status As String
        Public Property Id As String
        Public Property Type As String
        Public Property Description As String
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

    End Class
End Namespace