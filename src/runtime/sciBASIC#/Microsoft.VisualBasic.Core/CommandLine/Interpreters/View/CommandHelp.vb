﻿#Region "Microsoft.VisualBasic::7d15e03f189cfb4385a3670b0c012f3f, Microsoft.VisualBasic.Core\CommandLine\Interpreters\View\CommandHelp.vb"

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

    '     Module CommandHelpExtensions
    ' 
    '         Function: PrintCommandHelp
    ' 
    '         Sub: listingCommands
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash

Namespace CommandLine.ManView

    Module CommandHelpExtensions

        <Extension>
        Public Function PrintCommandHelp(app As Interpreter, commandName$) As Integer
            Dim name As New Value(Of String)

            If app.__API_table.ContainsKey(name = commandName.ToLower) Then
                Call app.__API_table(name).PrintHelp
            Else
                Dim list$() = app.ListingRelated(commandName)

                If list.IsNullOrEmpty Then
                    Call Console.WriteLine($"Bad command, no such a command named ""{commandName}"", ? for command list.")
                    Call Console.WriteLine()
                    Call Console.WriteLine(PS1.Fedora12.ToString & " ?" & commandName)
                Else
                    Call app.listingCommands(list, commandName)
                End If

                Return -2
            End If

            Return 0
        End Function

        ''' <summary>
        ''' Bad command, no such a command named ""{0}"", but you probably want to using one of these commands:
        ''' </summary>
        Const BAD_COMMAND_LISTING_COMMANDS$ = "Bad command, no such a command named ""{0}"", but you probably want to using one of these commands:"

        <Extension>
        Friend Sub listingCommands(app As Interpreter, commands$(), commandName$)
            Call Console.WriteLine(BAD_COMMAND_LISTING_COMMANDS, commandName)
            Call Console.WriteLine()

            Dim maxLength = commands.MaxLengthString.Length

            For Each cName As String In commands
                With cName
                    Dim msg$

                    msg$ = .ByRef & New String(" "c, maxLength - .Length + 3)

                    With app.__API_table(.ToLower).Info
                        If Not .StringEmpty Then
                            msg$ &= Mid(.ByRef, 1, 60) & "..."
                        End If
                    End With

                    Call Console.WriteLine("   " & msg)
                End With
            Next
        End Sub
    End Module
End Namespace
