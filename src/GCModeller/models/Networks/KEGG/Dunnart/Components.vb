Namespace Dunnart

    Public Class Node

        ''' <summary>
        ''' 代谢物名称标签文本
        ''' </summary>
        ''' <returns></returns>
        Public Property label As String

        ''' <summary>
        ''' 与<see cref="index"/>几乎是等价的
        ''' </summary>
        ''' <returns></returns>
        Public Property dunnartid As String
        ''' <summary>
        ''' 与<see cref="dunnartid"/>几乎是等价的
        ''' </summary>
        ''' <returns></returns>
        Public Property index As Integer
        Public Property width As Double
        Public Property height As Double
        Public Property x As Double
        Public Property y As Double
        Public Property rx As Double
        Public Property ry As Double
    End Class

    Public Class Link

        ''' <summary>
        ''' <see cref="Node.index"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property source As Integer
        ''' <summary>
        ''' <see cref="Node.index"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property target As Integer
    End Class

    Public Class Group

        ''' <summary>
        ''' an array of <see cref="Node.index"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property leaves As Integer()
        Public Property style As String
        Public Property padding As Double

    End Class

    Public Class Constraint
        ''' <summary>
        ''' always 'alignment'?
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String = "alignment"
        ''' <summary>
        ''' value should be x or y
        ''' </summary>
        ''' <returns></returns>
        Public Property axis As String
        Public Property offsets As NodeOffset()
    End Class

    Public Class NodeOffset
        ''' <summary>
        ''' <see cref="Dunnart.Node.index"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property node As Integer
        Public Property offset As Double
    End Class
End Namespace