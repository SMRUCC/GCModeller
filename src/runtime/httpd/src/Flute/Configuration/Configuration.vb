Imports System.ComponentModel
Imports Flute.Http.Core
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf

Namespace Configurations

    ''' <summary>
    ''' http server configuration
    ''' </summary>
    <ClassName("configuration")>
    Public Class Configuration

        <Description("a string for identify the http server backend.")>
        Public Property x_powered_by As String = HttpProcessor.VBS_platform

        <Description("a logical value for turn the verbose echo of the debug message on.")>
        Public Property silent As Boolean = True

        <Description("user session in server backend")>
        Public Property session As Session

        Public Shared Function [Default]() As Configuration
            Return New Configuration With {.session = New Session}
        End Function

        ''' <summary>
        ''' safe handler for load ini configuration file
        ''' </summary>
        ''' <param name="inifile"></param>
        ''' <returns>
        ''' this function returns the default configuration file if the
        ''' given <paramref name="inifile"/> missing or invalid file format.
        ''' </returns>
        Public Shared Function Load(inifile As String) As Configuration
            If inifile.FileLength <= 0 Then
                Return New Configuration
            End If

            Try
                Return ClassMapper.LoadIni(Of Configuration)(inifile)
            Catch ex As Exception
                Call App.LogException(ex)
                Return New Configuration
            End Try
        End Function

        Public Shared Function Save(settings As Configuration, inifile As String) As Boolean
            Return ClassMapper.WriteClass(settings, inifile)
        End Function

    End Class
End Namespace