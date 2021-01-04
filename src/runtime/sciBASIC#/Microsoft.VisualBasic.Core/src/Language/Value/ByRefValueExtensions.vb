﻿#Region "Microsoft.VisualBasic::613f4b5f4e2c32a7a2c8c81fdff46410, Microsoft.VisualBasic.Core\Language\Value\ByRefValueExtensions.vb"

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

    '     Module ByRefValueExtensions
    ' 
    '         Function: CreateDelegate, (+2 Overloads) First, Split, StartsWith, ToLower
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports ByRefString = Microsoft.VisualBasic.Language.Value(Of String)

Namespace Language.Values

    Public Module ByRefValueExtensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function Split(s As ByRefString, ParamArray delimiter As Char()) As String()
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function First(str As ByRefString) As Char
            Return str.Value.First
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function StartsWith(str As ByRefString, start As String) As Boolean
            Return str.Value.StartsWith(start)
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
