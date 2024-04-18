#Region "Microsoft.VisualBasic::1208c88ff5163bc81c9d1d2886d9ceac, WebCloud\SMRUCC.WebCloud.VBScript\ReflectionExtensions.vb"

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

    ' Module ReflectionExtensions
    ' 
    '     Function: CreateVariables, FlatObject, IsCollectionType, IsVariableType
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language

Module ReflectionExtensions

    Public Function IsVariableType(obj As Object) As Boolean
        With obj.GetType
            If .ByRef Is GetType(String) Then
                Return True
            ElseIf Not .ImplementInterface(GetType(IEnumerable)) Then
                Return True
            Else
                Return False
            End If
        End With
    End Function

    Public Function IsCollectionType(obj As Object) As Boolean
        With obj.GetType
            If Not .ByRef Is GetType(String) AndAlso .ImplementInterface(GetType(IEnumerable)) Then
                Return True
            Else
                Return False
            End If
        End With
    End Function

    Private Function FlatObject(key$, obj As Object) As Object
        If obj.GetType Is GetType(String) Then
            Return obj
        Else
            Return obj _
                .GetType _
                .Schema(PropertyAccess.Readable, PublicProperty, True) _
                .GetVariables(key, obj)
        End If
    End Function

    <Extension>
    Public Function CreateVariables(values As IEnumerable(Of KeyValuePair(Of String, Object))) As Dictionary(Of String, String)
        Dim table As New Dictionary(Of String, String)
        Dim value$

        For Each var In values
            With var
                If .Value.GetType Is GetType(String) Then
                    value = DirectCast(.Value, String)
                    table(.Key) = value
                Else
                    Dim vars = .Value _
                        .GetType _
                        .Schema(PropertyAccess.Readable, PublicProperty, True) _
                        .GetVariables(.Key, .Value)

                    For Each tuple As NamedValue(Of String) In vars
                        Call table.Add(tuple.Name, tuple.Value)
                    Next
                End If
            End With
        Next

        Return table
    End Function
End Module
