#Region "Microsoft.VisualBasic::3e919062954af381827407a30ba7c2df, GCModeller\analysis\Metagenome\Metagenome\gast\subs.vb"

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


    ' Code Statistics:

    '   Total Lines: 28
    '    Code Lines: 20
    ' Comment Lines: 3
    '   Blank Lines: 5
    '     File Size: 845 B


    '     Module SUBROUTINES
    ' 
    '         Sub: run_command
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
