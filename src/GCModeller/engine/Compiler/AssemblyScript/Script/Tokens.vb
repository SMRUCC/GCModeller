#Region "Microsoft.VisualBasic::762a10f9965ead4c385381acc7ab8f84, GCModeller\engine\Compiler\AssemblyScript\Script\Tokens.vb"

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

    '   Total Lines: 49
    '    Code Lines: 18
    ' Comment Lines: 27
    '   Blank Lines: 4
    '     File Size: 966 B


    '     Enum Tokens
    ' 
    '         assign, comma, comment, keyword, number
    '         reference, symbol, text
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class Token
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace AssemblyScript.Script

    ''' <summary>
    ''' 
    ''' </summary>
    Public Enum Tokens
        ''' <summary>
        ''' -- comment
        ''' </summary>
        comment
        ''' <summary>
        ''' word
        ''' </summary>
        keyword
        ''' <summary>
        ''' x
        ''' </summary>
        symbol
        ''' <summary>
        ''' =
        ''' </summary>
        assign
        ''' <summary>
        ''' "..."
        ''' </summary>
        text
        ''' <summary>
        ''' ,
        ''' </summary>
        comma
        ''' <summary>
        ''' ::
        ''' </summary>
        reference
        ''' <summary>
        ''' \d
        ''' </summary>
        number
    End Enum

    Public Class Token : Inherits CodeToken(Of Tokens)

        Public Sub New(name As Tokens, value$)
            Call MyBase.New(name, value)
        End Sub
    End Class
End Namespace
