﻿#Region "Microsoft.VisualBasic::897aacbbbb48e01e48c144969ea12e61, WebCloud\SMRUCC.HTTPInternal\AppEngine\SessionManager.vb"

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

    '     Module SessionManager
    ' 
    '         Function: GetSession, GetSessionPath, NewSession
    ' 
    '         Sub: Save
    ' 
    '     Class Session
    ' 
    '         Properties: ID, Table
    ' 
    '         Function: ToString
    ' 
    '         Sub: SetValue
    ' 
    '     Class Value
    ' 
    '         Properties: Table, Value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace AppEngine

    Public Module SessionManager

        ReadOnly random As New Random

        Public Function NewSession() As Session
            Dim name$ = RandomASCIIString(32, skipSymbols:=True, seed:=random)
            Dim emptyTable As New Dictionary(Of String, Value)
            Dim session As New Session With {
                .ID = name,
                .Table = emptyTable
            }
            Dim path$ = GetSessionPath(name)

            Call session.GetJson.SaveTo(path)

            Return session
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Sub Save(session As Session)
            Call session.GetJson.SaveTo(GetSessionPath(id:=session.ID))
        End Sub

        ''' <summary>
        ''' 得到session json文件的文件路径
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function GetSessionPath(id As String) As String
            Dim dir$ = App.ProductSharedDIR & "/Sessions"
            Dim path$ = $"{dir}/{id}.json"
            Return path
        End Function

        Public Function GetSession(id As String) As Session
            Dim path$ = GetSessionPath(id)

            If path.FileExists Then
                Return path.LoadJSON(Of Session)
            Else
                Return New Session
            End If
        End Function
    End Module

    Public Class Session : Implements INamedValue

        Public Property ID As String Implements INamedValue.Key
        Public Property Table As Dictionary(Of String, Value)

        Default Public Property Item(name As String) As Value
            Get
                Return Table.TryGetValue(name)
            End Get
            Set(value As Value)
                Table(name) = value
            End Set
        End Property

        Public Sub SetValue(key$, value$)
            Item(key) = value
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{ID} => {Table.Keys.ToArray.GetJson}]"
        End Function
    End Class

    Public Class Value

        Public Property Value As String
        Public Property Table As Dictionary(Of String, Value)

        Public Overrides Function ToString() As String
            If Table.IsNullOrEmpty Then
                Return Value
            Else
                Return Table.GetJson
            End If
        End Function

        Public Shared Widening Operator CType(value As String) As Value
            Return New Value With {
                .Value = value,
                .Table = New Dictionary(Of String, Value)
            }
        End Operator
    End Class
End Namespace
