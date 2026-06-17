Namespace Intervention

    ''' <summary>
    ''' 单次干预定义
    ''' </summary>
    Public Class InterventionSpec

        ''' <summary>目标基因索引</summary>
        Public Property GeneIndex As Integer

        ''' <summary>目标基因名称</summary>
        Public Property GeneName As String = ""

        ''' <summary>干预模式</summary>
        Public Property Mode As InterventionMode = InterventionMode.Knockout

        ''' <summary>干预值（Custom 模式下使用）</summary>
        Public Property Value As Double = 0.0

        ''' <summary>根据干预模式获取实际干预值</summary>
        Public Function GetInterventionValue(wildtypeMean As Double, wildtypeSD As Double) As Double
            Select Case Mode
                Case InterventionMode.Knockout
                    Return 0.0
                Case InterventionMode.Overexpression
                    Return wildtypeMean + 3.0 * wildtypeSD  ' 3倍标准差过表达
                Case InterventionMode.Knockdown
                    Return wildtypeMean - 2.0 * wildtypeSD  ' 2倍标准差下调
                Case InterventionMode.Custom
                    Return Value
                Case Else
                    Return Value
            End Select
        End Function

    End Class

End Namespace