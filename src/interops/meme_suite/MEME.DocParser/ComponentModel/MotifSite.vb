Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace ComponentModel

    ''' <summary>
    ''' Motif的Csv文件输出格式之1 
    ''' </summary>
    Public Class MotifSite : Implements ISiteReader

        ''' <summary>
        ''' 位点名称，通常为基因号
        ''' </summary>
        ''' <returns></returns>
        <Column("Site")> Public Property Locus_tag As String Implements ISiteReader.ORF
        Public Property Signature As String
        Public Property Sequence As String Implements ISiteReader.SequenceData
        Public Property Start As Integer
        ''' <summary>
        ''' 从右端往前数的<see cref="start"/>的位置，假若这条序列是表示启动子区的序列的话，并且从ATG上游开始取序列至ATG位点，则这个属性值还可以作为ATGDistance
        ''' </summary>
        ''' <returns></returns>
        Public Property RightEndDownStream As Integer Implements ISiteReader.Distance
        Public Property EValue As Double
        Public Property PValue As Double
        <Column("Motif.uid")> Public Property uid As String

        Public Overrides Function ToString() As String
            Return Locus_tag
        End Function

        ''' <summary>
        ''' 位点在基因组序列上面的左端起始
        ''' </summary>
        ''' <returns></returns>
        Public Property gStart As Integer Implements ISiteReader.gStart
        ''' <summary>
        ''' 位点在基因组序列上面的右端结束
        ''' </summary>
        ''' <returns></returns>
        Public Property gStop As Integer Implements ISiteReader.gStop
        ''' <summary>
        ''' 位点在链上面的方向
        ''' </summary>
        ''' <returns></returns>
        Public Property Strand As String Implements ISiteReader.Strand

#Region "这两个属性一般是用来进行家族统计的"

        ''' <summary>
        ''' 一些其他的用户的自定义的数据
        ''' </summary>
        ''' <returns></returns>
        Public Property Tag As String
        Public Property Family As String Implements ISiteReader.Family
#End Region

    End Class
End Namespace