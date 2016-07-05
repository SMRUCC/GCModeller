#Region "Microsoft.VisualBasic::e63012ea6783424f4a8a16a75cfcb1e8, ..\GCModeller\sub-system\PLAS.NET\SSystem\Script\Tokens.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.ComponentModel

Namespace Script

    Public Enum Tokens
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
