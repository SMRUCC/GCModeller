Namespace Regprecise

    Public Class RegPreciseRegulatorMatch

        ''' <summary>
        ''' The genome query.(待注释的目标基因组的蛋白编号)
        ''' </summary>
        ''' <returns></returns>
        Public Property Query As String

        ''' <summary>
        ''' The regprecise regulator name
        ''' </summary>
        ''' <returns></returns>
        Public Property Regulator As String
        Public Property Family As String
        Public Property Description As String

        ''' <summary>
        ''' 相似度
        ''' </summary>
        ''' <returns></returns>
        Public Property Identities As Double

        ''' <summary>
        ''' 这个<see cref="Regulator"/>所调控的位点集合
        ''' </summary>
        ''' <returns></returns>
        Public Property RegulonSites As String()
    End Class
End Namespace