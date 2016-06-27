Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' This file lists all transcription factors in the PGDB and the genes that 
    ''' they regulate by binding upstream of the transcription unit containing 
    ''' those genes.
    ''' (本数据库文件中记录了所有的转录因子以及通过与包含这些基因的转录单元的上游区域
    ''' 进行结合而发挥调控作用的基因)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Regulons : Inherits DataFile(Of Slots.Regulon)

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

