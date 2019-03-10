﻿#Region "Microsoft.VisualBasic::2aa3a0e8fd3451b671d76bef6a1c3bbc, Microsoft.VisualBasic.Core\ApplicationServices\Terminal\STDIO__\I_ConsoleDeviceHandle.vb"

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

    '     Interface I_ConsoleDeviceHandle
    ' 
    '         Function: Read, ReadLine
    ' 
    '         Sub: (+2 Overloads) WriteLine
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Terminal.STDIO__

    ''' <summary>
    ''' Represents the standard input, output, and error streams for console applications.(表示一个输入输出流控制台界面接口)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface I_ConsoleDeviceHandle

        ''' <summary>
        ''' Writes the specified string value, followed by the current line terminator, to the standard output stream.
        ''' (将指定的字符串值（后跟当前行终止符）写入输出流。)
        ''' </summary>
        ''' <param name="s"></param>
        ''' <remarks></remarks>
        Sub WriteLine(Optional s As String = "")
        ''' <summary>
        ''' Writes the text representation of the specified array of objects, followed by the current line terminator, to the standard output stream using the specified format information.
        ''' (将指定的字符串值（后跟当前行终止符）写入输出流。)
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="args"></param>
        ''' <remarks></remarks>
        Sub WriteLine(s As String, ParamArray args As String())
        ''' <summary>
        ''' Reads the next line of characters from the standard input stream.(从输入流读取下一行字符)
        ''' </summary>
        ''' <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        ''' <remarks></remarks>
        Function ReadLine() As String
        ''' <summary>
        ''' Reads the next character from the standard input stream.(从输入流读取下一个字符)
        ''' </summary>
        ''' <returns>The next character from the input stream, or negative one (-1) if there are currently no more characters to be read.</returns>
        ''' <remarks></remarks>
        Function Read() As Integer
    End Interface
End Namespace
