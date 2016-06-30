Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Drawing
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.ChromosomeMap

Public Module ConfigurationCommon

    Public Function FromConfig(config As Configurations) As ChromosomeMap.DrawingDevice
        Dim Data As Conf = config.ToConfigurationModel
        Dim DeviceSize = Data.Resolution
        Dim Device As ChromosomeMap.DrawingDevice = New ChromosomeMap.DrawingDevice(Data)
        Return Device
    End Function

    <Extension> Public Function GetFlagHeight(config As Configurations) As Integer
        If String.Equals(config.FLAG_HEIGHT, Regex.Match(config.LineHeight, "\d+").Value) Then '是一个数字
            Return Val(config.FLAG_HEIGHT)
        Else '是一个表达式
            Dim value As Double = Val(Regex.Match(config.FLAG_HEIGHT, "\d+(.\d+)?").Value)
            value = config.GeneObjectHeight * value
            Return value
        End If
    End Function

    <Extension> Public Function GetLineHeight(config As Configurations) As Integer
        If String.Equals(config.LineHeight, Regex.Match(config.LineHeight, "\d+").Value) Then '是一个数字
            Return Val(config.LineHeight)
        Else '是一个表达式
            Dim value As Double = Val(Regex.Match(config.LineHeight, "\d+(.\d+)?").Value)
            value = config.GeneObjectHeight * value
            Return value
        End If
    End Function
End Module
