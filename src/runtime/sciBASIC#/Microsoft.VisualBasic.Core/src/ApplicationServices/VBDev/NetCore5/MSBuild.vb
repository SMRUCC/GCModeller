Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Linq

Namespace ApplicationServices.Development.NetCore5

    ''' <summary>
    ''' .NET core msbuild command wrapper
    ''' 
    ''' ```
    ''' dotnet msbuild
    ''' ```
    ''' </summary>
    Public Class MSBuild

        ''' <summary>
        ''' get dotnet version value
        ''' </summary>
        ''' <returns>
        ''' nothing means dotnet msbuild is not installed
        ''' </returns>
        Public Shared ReadOnly Property version As Version
            Get
                Static ver As Version = getVersion()
                Return ver
            End Get
        End Property

        Private Shared Function getVersion() As Version
            Dim verStr As String = MSBuild.dotnetShell("msbuild --version") _
                    .DoCall(AddressOf Strings.Trim) _
                    .LineTokens _
                    .LastOrDefault

            If verStr.StringEmpty Then
                Return Nothing
            Else
                Dim ver As Version = Version.Parse(verStr)
                Return ver
            End If
        End Function

        ''' <summary>
        ''' shell dotnet commandline and then returns the standard output of the dotnet command.
        ''' </summary>
        ''' <param name="arguments"></param>
        ''' <returns></returns>
        Private Shared Function dotnetShell(arguments As String) As String
            Return PipelineProcess.Call("dotnet", arguments)
        End Function

        Public Shared Function BuildVsSolution(sln As String, Optional rebuild As Boolean = True) As String

        End Function

    End Class
End Namespace