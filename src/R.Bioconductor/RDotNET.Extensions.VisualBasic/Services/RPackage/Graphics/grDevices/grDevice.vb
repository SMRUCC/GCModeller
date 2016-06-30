Imports System.Text

Namespace grDevices

    Public MustInherit Class grDevice : Inherits IRToken

        ''' <summary>
        ''' the width of the device.
        ''' </summary>
        ''' <returns></returns>
        Public Property width As Integer = 480
        ''' <summary>
        ''' the height of the device.
        ''' </summary>
        ''' <returns></returns>
        Public Property height As Integer = 480
        ''' <summary>
        ''' A length-one character vector specifying the default font family. The default means to use the font numbers on the Windows GDI versions and "sans" on the cairographics versions.
        ''' </summary>
        ''' <returns></returns>
        Public Property family As String = ""
        ''' <summary>
        ''' the initial background colour: can be overridden by setting par("bg").
        ''' </summary>
        ''' <returns></returns>
        Public Property bg As String = "white"

        ''' <summary>
        ''' 生成创建图像文件的脚本代码
        ''' </summary>
        ''' <param name="plots">绘图的脚本表达式</param>
        ''' <returns></returns>
        Public Function Plot(plots As String) As String
            Dim script As New StringBuilder(RScript)
            Call script.AppendLine()
            Call script.AppendLine()
            Call script.AppendLine(plots)
            Call script.AppendLine("dev.off()")

            Return script.ToString
        End Function

        ''' <summary>
        ''' 生成创建图像文件的脚本代码
        ''' </summary>
        ''' <param name="plots">绘图的脚本表达式</param>
        ''' <returns></returns>
        Public Function Plot(plots As Func(Of String)) As String
            Return Plot(plots())
        End Function
    End Class
End Namespace