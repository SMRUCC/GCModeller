Namespace Assembly.DOOR.CsvModel

    Public Class Operon

        ''' <summary>
        ''' 本操纵子在Door数据库之中的编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DoorId As String
        ''' <summary>
        ''' 转路方向
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Direction As String
        ''' <summary>
        ''' 结构基因
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Genes As String()
        ''' <summary>
        ''' 结构基因的数目
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property NumOfGenes As Integer

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}:{2}", Direction, DoorId, String.Join(", ", Genes))
        End Function
    End Class
End Namespace