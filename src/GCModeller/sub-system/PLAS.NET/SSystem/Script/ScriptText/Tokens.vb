﻿#Region "Microsoft.VisualBasic::c61950af5b33ea23c840a33d5e18c2de, GCModeller\sub-system\PLAS.NET\SSystem\Script\ScriptText\Tokens.vb"

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

    '   Total Lines: 57
    '    Code Lines: 22
    ' Comment Lines: 30
    '   Blank Lines: 5
    '     File Size: 1.47 KB


    '     Class ScriptToken
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Enum ScriptTokens
    ' 
    '         UNDEFINE
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Script

    Public Class ScriptToken : Inherits CodeToken(Of ScriptTokens)

        Sub New(token As ScriptTokens, text As String)
            Call MyBase.New(token, text)
        End Sub
    End Class

    Public Enum ScriptTokens
        UNDEFINE

        ''' <summary>
        ''' TITLE
        ''' </summary>
        <Description("TITLE")> Title
        ''' <summary>
        ''' RXN
        ''' </summary>
        <Description("RXN")> Reaction
        ''' <summary>
        ''' INIT
        ''' </summary>
        <Description("INIT")> InitValue
        ''' <summary>
        ''' FUNC
        ''' </summary>
        <Description("FUNC")> [Function]
        ''' <summary>
        ''' FINALTIME
        ''' </summary>
        <Description("FINALTIME")> Time
        ''' <summary>
        ''' NAMED
        ''' </summary>
        <Description("NAMED")> [Alias]
        ''' <summary>
        ''' STIMULATE
        ''' </summary>
        <Description("STIMULATE")> Disturb
        ''' <summary>
        ''' COMMENT
        ''' </summary>
        <Description("COMMENT")> Comment
        ''' <summary>
        ''' REM-SUB
        ''' </summary>
        <Description("REM-SUB")> SubsComments
        ''' <summary>
        ''' CONST
        ''' </summary>
        <Description("CONST")> Constant
    End Enum
End Namespace
