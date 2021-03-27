﻿#Region "Microsoft.VisualBasic::5223993073b5bccf35a611da8958d7ca, mime\application%json\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: AsString, AsStringVector, ParseJson, ParseJsonFile
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

<HideModuleName> Public Module Extensions

    ''' <summary>
    ''' Parse json string
    ''' </summary>
    ''' <param name="JsonStr"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ParseJson(JsonStr As String) As JsonElement
        Return New JsonParser().OpenJSON(JsonStr)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ParseJsonFile(JsonFile As String) As JsonElement
        Return New JsonParser().Open(JsonFile)
    End Function

    ''' <summary>
    ''' try cast of the json element object as json literal value
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsString(obj As JsonElement) As String
        Return DirectCast(obj, JsonValue).GetStripString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsStringVector(array As JsonElement) As String()
        If array Is Nothing Then
            Return Nothing
        End If
        If TypeOf array Is JsonValue Then
            Return {array.AsString}
        End If

        Return DirectCast(array, JsonArray) _
            .SafeQuery _
            .Select(AddressOf AsString) _
            .ToArray
    End Function
End Module
