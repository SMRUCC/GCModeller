Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Perl

Namespace gast

    ''' <summary>
    ''' ########################## SUBROUTINES #######################################
    ''' </summary>
    Module SUBROUTINES

        Public Sub run_command(command As String)
            If verbose Then Call command.__DEBUG_ECHO

            Dim command_err = Perl.system(command)

            If (command_err) Then
                Dim err_msg =
                    $"Error {command_err} encountered while running: ""{command}""."
                Throw New Exception(err_msg)
            End If
        End Sub
    End Module
End Namespace