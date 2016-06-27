
Namespace Assembly.MetaCyc.File.DataFiles

    Public Class tRNA : Inherits DataFile(Of Slots.tRNA)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace

