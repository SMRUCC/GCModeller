Imports System.Text

Namespace RegulationSignature


    ''' <summary>
    ''' 这个属性值类型是为了在不同的基因组之间进行相互比较而设置的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneID

        ''' <summary>
        ''' 基因的分类类型
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum ClassTypes
            TF
            KO
            ''' <summary>
            ''' TF + KO
            ''' </summary>
            ''' <remarks></remarks>
            Hybrids
            Hypothetical
        End Enum

        ''' <summary>
        ''' 实际上基因号由于不同的基因之间是不同的，所以在这里使用基因名称来表示基因以尽量消除误差
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneName As String
        Public Property ClassType As ClassTypes

        ''' <summary>
        ''' 这一部分的数据是不参与比较的，但是会放置在序列的尾部作为反序列化的显示值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneTagID As String

        '    Const Seperator As String = "TTTGATTT"

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder()
            '   Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GenerateCode(GeneName))
            '   Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GenerateCode(CInt(ClassType)))

            Return sBuilder.ToString
        End Function
    End Class

End Namespace