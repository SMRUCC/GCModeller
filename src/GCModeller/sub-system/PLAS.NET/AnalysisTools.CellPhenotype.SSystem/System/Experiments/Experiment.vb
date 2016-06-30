Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Kernel.ObjectModels

    ''' <summary>
    ''' 对系统进行的一个刺激实验
    ''' </summary>
    Public Class Experiment

        ''' <summary>
        ''' The name Id of the target.
        ''' (目标的名称)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Id As String

        ''' <summary>
        ''' The start time of this disturb.
        ''' (这个干扰动作的开始时间)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Start As Double
        ''' <summary>
        ''' The interval ticks between each kick.
        ''' (每次干扰动作执行的时间间隔)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Interval As Double
        ''' <summary>
        ''' The counts of the kicks.
        ''' (执行的次数)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Kicks As Integer
        <XmlAttribute> Public Property DisturbType As Types
        <XmlAttribute> Public Property Value As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace