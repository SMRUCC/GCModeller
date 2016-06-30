Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.StrPNet

    ''' <summary>
    ''' 本类型可以通过CSV模块兼容CSV表格类型的计算模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TFRegulation : Implements sIdEnumerable

        ''' <summary>
        ''' 通常为属性<see cref="LANS.SystemsBiology.Assembly.Door.GeneBrief.OperonID"></see>的这个编号值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Operon-Id")> Public Property OperonId As String
        <Column("Regulator")> Public Property Regulator As String Implements sIdEnumerable.Identifier
        <CollectionAttribute("Operon-Genes")> Public Property OperonGenes As String()
        Public Property PromoterGene As String
        ''' <summary>
        ''' Regulator对第一个基因的Pcc值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TFPcc As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", Regulator, OperonId)
        End Function
    End Class
End Namespace