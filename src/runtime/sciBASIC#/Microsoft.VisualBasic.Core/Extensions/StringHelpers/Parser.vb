﻿#Region "Microsoft.VisualBasic::5b6f4bd1fb367932cc365a51c38f15fc, Microsoft.VisualBasic.Core\Extensions\StringHelpers\Parser.vb"

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

    ' Module PrimitiveParser
    ' 
    '     Properties: BooleanValues
    ' 
    '     Function: Eval, IsNumeric, (+2 Overloads) ParseBoolean, ParseDate, ParseDouble
    '               ParseInteger, ParseLong, ParseSingle
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

''' <summary>
''' Simple type parser extension function for <see cref="String"/>
''' </summary>
Public Module PrimitiveParser

    ''' <summary>
    ''' Evaluate the given string expression as numeric value 
    ''' </summary>
    ''' <param name="expression$"></param>
    ''' <param name="default#"></param>
    ''' <returns></returns>
    Public Function Eval(expression$, default#) As Double
        If expression Is Nothing Then
            Return [default]
        Else
            Return Conversion.Val(expression)
        End If
    End Function

    ''' <summary>
    ''' 用于匹配任意实数的正则表达式
    ''' </summary>
    Public Const NumericPattern$ = "[-]?\d*(\.\d+)?([eE][-]?\d*)?"

    ''' <summary>
    ''' Is this token value string is a number?
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    <ExportAPI("IsNumeric", Info:="Is this token value string is a number?")>
    <Extension> Public Function IsNumeric(str$) As Boolean
        With str.GetString(ASCII.Quot)
            Dim s$ = r.Match(.ByRef, NumericPattern).Value
            Return .ByRef = s
        End With
    End Function

    ''' <summary>
    ''' <see cref="Integer"/> text parser
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ParseInteger(s As String) As Integer
        Return CInt(Val(Trim(s)))
    End Function

    ''' <summary>
    ''' <see cref="Long"/> text parser
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ParseLong(s As String) As Long
        Return CLng(Val(Trim(s)))
    End Function

    ''' <summary>
    ''' <see cref="Double"/> text parser
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ParseDouble(s As String) As Double
        If s Is Nothing Then
            Return 0
        Else
            Return ParseNumeric(s)
        End If
    End Function

    ''' <summary>
    ''' <see cref="Single"/> text parser
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ParseSingle(s As String) As Single
        Return CSng(Val(Trim(s)))
    End Function

    ''' <summary>
    ''' <see cref="Date"/> text parser
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ParseDate(s As String) As Date
        Return Date.Parse(Trim(s))
    End Function

    ''' <summary>
    ''' Convert the string value into the boolean value, this is useful to the text format configuration file into data model.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property BooleanValues As New SortedDictionary(Of String, Boolean) From {
 _
            {"t", True}, {"true", True},
            {"1", True},
            {"y", True}, {"yes", True}, {"ok", True},
            {"ok!", True},
            {"success", True}, {"successful", True}, {"successfully", True}, {"succeeded", True},
            {"right", True},
            {"wrong", False},
            {"failure", False}, {"failures", False},
            {"exception", False},
            {"error", False}, {"err", False},
            {"f", False}, {"false", False},
            {"0", False},
            {"n", False}, {"no", False}
        }

    ''' <summary>
    ''' Convert the string value into the boolean value, this is useful to the text format configuration file into data model.
    ''' (请注意，空值字符串为False，如果字符串不存在与单词表之中，则也是False)
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Get.Boolean")> <Extension> Public Function ParseBoolean(str$) As Boolean
        If String.IsNullOrEmpty(str) Then
            Return False
        Else
            str = str.ToLower.Trim
        End If

        If BooleanValues.ContainsKey(key:=str) Then
            Return BooleanValues(str)
        Else
#If DEBUG Then
            Call $"""{str}"" {NameOf([Boolean])} (null_value_definition)  ==> False".__DEBUG_ECHO
#End If
            Return False
        End If
    End Function

    <Extension> <ExportAPI("Get.Boolean")> Public Function ParseBoolean(ch As Char) As Boolean
        If ch = ASCII.NUL Then
            Return False
        End If

        Select Case ch
            Case "y"c, "Y"c, "t"c, "T"c, "1"c
                Return True
            Case "n"c, "N"c, "f"c, "F"c, "0"c
                Return False
        End Select

        Return True
    End Function
End Module
