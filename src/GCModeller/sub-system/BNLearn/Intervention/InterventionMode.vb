Namespace Intervention

    ''' <summary>
    ''' 干预模式
    ''' </summary>
    Public Enum InterventionMode
        ''' <summary>基因敲除（设为0）</summary>
        Knockout
        ''' <summary>基因过表达（设为高值）</summary>
        Overexpression
        ''' <summary>基因下调（设为低值）</summary>
        Knockdown
        ''' <summary>自定义值干预</summary>
        Custom
    End Enum
End Namespace