Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Imaging

Namespace Analysis.Similarity.TOMQuery

    Public Class TomOUT

        Public Property Query As MotifScans.AnnotationModel
        Public Property Subject As MotifScans.AnnotationModel
        Public Property Alignment As DistResult

        Public ReadOnly Property QueryLength As Integer
            Get
                If Query Is Nothing Then
                    Return 0
                Else
                    Return Query.PWM.Length
                End If
            End Get
        End Property

        Public ReadOnly Property SubjectLength As Integer
            Get
                If Subject Is Nothing Then
                    Return 0
                Else
                    Return Subject.PWM.Length
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Query.ToString}, {Subject.ToString}"
        End Function

        Public Function Visual() As Image
            Return TomVisual.VisualLevEdit(Query, Subject, Alignment, False).CorpBlank(20)
        End Function

        Public Function ResultView() As TomTOm.CompareResult
            Return TomTOm.CreateResult(Query, Subject, Alignment)
        End Function
    End Class

    ''' <summary>
    ''' 使用这个对象进行高分区的绘图
    ''' </summary>
    Public Class SW_HSP : Inherits TomOUT
        <XmlAttribute> Public Property FromQ As Integer
        <XmlAttribute> Public Property FromS As Integer
        <XmlAttribute> Public Property ToQ As Integer
        <XmlAttribute> Public Property ToS As Integer
        <XmlAttribute> Public Property Score As Double

        Public Overrides Function ToString() As String
            Dim q As String = $"{Query.Uid}.[{FromQ}, {ToQ}]"
            Dim s As String = $"{Subject.Uid}.[{FromS}, {ToS}]"
            Return $"{q}, {s}"
        End Function
    End Class
End Namespace