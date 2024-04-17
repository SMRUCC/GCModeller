Imports Flute.Http.Core
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf

Public Class Configuration

    Public Property x_powered_by As String = HttpProcessor.VBS_platform

    Public Shared Function Load(inifile As String) As Configuration
        If inifile.FileLength <= 0 Then
            Return New Configuration
        End If

        Return ClassMapper.LoadIni(Of Configuration)(inifile)
    End Function

    Public Shared Function Save(settings As Configuration, inifile As String) As Boolean
        Return ClassMapper.WriteClass(settings, inifile)
    End Function

End Class
