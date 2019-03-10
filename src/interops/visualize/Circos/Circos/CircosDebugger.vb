Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting

''' <summary>
''' Generates the circos debugger options
''' </summary>
Public Module CircosDebugger

    ReadOnly __options As DebugGroups() = Enums(Of DebugGroups)()

    ''' <summary>
    ''' ``-debug_group`` command argument name
    ''' </summary>
    Public Const Debugger As String = " -debug_group "

    ''' <summary>
    ''' Generates the ``-debug_group`` options from the enum values
    ''' </summary>
    ''' <param name="arg"></param>
    ''' <returns></returns>
    <Extension> Public Function GetOptions(arg As DebugGroups) As String
        If arg = DebugGroups.NULL Then
            Return ""
        Else
            Dim options As New List(Of String)

            For Each o As DebugGroups In __options
                If arg.HasFlag(o) Then
                    Call options.Add(o.ToString)
                End If
            Next

            Return Debugger & options.JoinBy(",")
        End If
    End Function

    Public Function EnableAllOptions() As String
        Return Debugger & __options _
            .Select(AddressOf InputHandler.ToString) _
            .JoinBy(",")
    End Function
End Module