#Region "Microsoft.VisualBasic::d606c0eb799031d7c041ea0414eb1cda, visualize\Circos\Circos\CircosDebugger.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module CircosDebugger
    ' 
    '     Function: EnableAllOptions, GetOptions
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting

''' <summary>
''' Generates the circos debugger options
''' </summary>
Public Module CircosDebugger

    ReadOnly options As DebugGroups() = Enums(Of DebugGroups)()

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

            For Each o As DebugGroups In CircosDebugger.options
                If arg.HasFlag(o) Then
                    Call options.Add(o.ToString)
                End If
            Next

            Return Debugger & options.JoinBy(",")
        End If
    End Function

    Public Function EnableAllOptions() As String
        Return Debugger & options _
            .Select(AddressOf InputHandler.ToString) _
            .JoinBy(",")
    End Function
End Module
