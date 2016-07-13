Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Perl

Namespace gast

    ''' <summary>
    ''' ########################## SUBROUTINES #######################################
    ''' </summary>
    Module SUBROUTINES

        <Extension>
        Public Sub run_command(LOG As StreamWriter, command As String)
            If verbose Then Call command.__DEBUG_ECHO
            Call LOG.WriteLine(command)

            Dim command_err = Perl.system(command)

            If (command_err) Then
                Dim err_msg =
                    $"Error {command_err} encountered while running: ""{command}""."
                Call LOG.WriteLine(err_msg)
                Throw New Exception(err_msg)
            End If
        End Sub
    End Module
End Namespace