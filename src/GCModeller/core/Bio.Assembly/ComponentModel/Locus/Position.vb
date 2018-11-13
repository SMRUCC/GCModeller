Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Loci

    ''' <summary>
    ''' 百分比相对位置
    ''' </summary>
    Public Structure Position
        Public Property Left As Double
        Public Property Right As Double

        Sub New(loci As Location, len As Integer)
            Me.Left = loci.Left / len
            Me.Right = loci.Right / len
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace