Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace GCML_Documents.ComponentModels

    ''' <summary>
    ''' 所有的参数都可以被<see cref="Microsoft.VisualBasic.CommandLine.CommandLine">命令行解析器所解析</see>
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Experiment

        Public Enum Types As Integer
            Increase
            Decrease
            Multiplying
            Decay
            [Mod]
            ChangeTo
        End Enum

        ''' <summary>
        ''' Target Action List
        ''' </summary>
        ''' <remarks></remarks>
        <Column("Actions")> Public Property TargetAction As String

        ''' <summary>
        ''' The start time of this disturb.
        ''' (这个干扰动作的开始时间)
        ''' </summary>
        ''' <remarks></remarks>
        <Column("Trigger")> <XmlAttribute> Public Property TriggedCondition As String
        <Column("PeriodicBehavior")> <XmlAttribute> Public Property PeriodicBehavior As String

        Public Class PeriodicBahaviors
            <XmlAttribute> Public Property TICKS As Integer
            <XmlAttribute> Public Property Interval As Integer

            Public Overrides Function ToString() As String
                Return Me.GetXml
            End Function
        End Class

        Public Overrides Function ToString() As String
            Return TargetAction
        End Function
    End Class
End Namespace