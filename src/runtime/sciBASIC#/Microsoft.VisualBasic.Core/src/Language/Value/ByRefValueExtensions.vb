﻿#Region "Microsoft.VisualBasic::c3f3bc718740e277635a2c1a6ec1b49a, Microsoft.VisualBasic.Core\src\Language\Value\ByRefValueExtensions.vb"

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

    '   Total Lines: 95
    '    Code Lines: 55 (57.89%)
    ' Comment Lines: 30 (31.58%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (10.53%)
    '     File Size: 3.89 KB


    '     Module ByRefValueExtensions
    ' 
    '         Function: CreateDelegate, (+2 Overloads) First, GetTagValue, Split, StartsWith
    '                   ToLower
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports ByRefString = Microsoft.VisualBasic.Language.Value(Of String)

Namespace Language.Values

    <HideModuleName>
    Public Module ByRefValueExtensions

        ''' <summary>
        ''' Splits a string into substrings that are based on the 
        ''' characters in the separator array.
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="delimiter">
        ''' A character array that delimits the substrings in this string, 
        ''' an empty array that contains no delimiters, Or null.
        ''' </param>
        ''' <returns>An array whose elements contain the substrings from this 
        ''' instance that are delimited by one Or more characters in separator. 
        ''' For more information, see the Remarks section.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Split(s As ByRefString, ParamArray delimiter As Char()) As String()
            Return s.Value.Split(delimiter)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ToLower(str As ByRefString) As String
            Return Strings.LCase(str.Value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function First(Of T)(list As Value(Of IEnumerable(Of T))) As T
            Return list.Value.First
        End Function

        ''' <summary>
        ''' get the first char
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function First(str As ByRefString) As Char
            If str Is Nothing OrElse
                str.Value Is Nothing OrElse
                str.Value.Length = 0 Then

                Return Nothing
            Else
                Return str.Value.First
            End If
        End Function

        <Extension>
        Public Function GetTagValue(s As Value(Of String),
                                    Optional delimiter$ = " ",
                                    Optional trim As Boolean = False,
                                    Optional failureNoName As Boolean = True) As NamedValue(Of String)
            Return s.Value.GetTagValue(delimiter, trim, failureNoName)
        End Function

        ''' <summary>
        ''' Determines whether the beginning of this string instance matches the specified
        ''' string.
        ''' </summary>
        ''' <param name="str"></param>
        ''' <param name="substr">The string to compare.</param>
        ''' <returns>true if value matches the beginning of this string; otherwise, false.</returns>
        <Extension>
        Public Function StartsWith(str As Value(Of String), substr As String) As Boolean
            If str Is Nothing OrElse str.Value Is Nothing Then
                Return False
            Else
                Return str.Value.StartsWith(substr)
            End If
        End Function

        ''' <summary>
        ''' Creates a delegate of the specified type from this method.
        ''' </summary>
        ''' <param name="methodInfo"></param>
        ''' <param name="delegateType">The type of the delegate to create.</param>
        ''' <returns>The delegate for this method.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CreateDelegate(methodInfo As Value(Of MethodInfo), delegateType As Type) As [Delegate]
            Return methodInfo.Value.CreateDelegate(delegateType)
        End Function
    End Module
End Namespace
