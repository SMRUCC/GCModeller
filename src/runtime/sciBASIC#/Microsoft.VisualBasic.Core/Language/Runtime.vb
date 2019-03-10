﻿#Region "Microsoft.VisualBasic::178f142548581c8ce312c5e623367823, Microsoft.VisualBasic.Core\Language\Runtime.vb"

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

    '     Class ArgumentReference
    ' 
    '         Properties: Expression, Key
    ' 
    '         Function: ToString
    '         Operators: <>, =
    ' 
    '     Class TypeSchema
    ' 
    '         Properties: Type
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Equals, ToString
    '         Operators: (+2 Overloads) And, (+2 Overloads) Or
    ' 
    '     Class Runtime
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language.Default

Namespace Language

    Public Class ArgumentReference : Implements INamedValue

        Public name$, value

        Private Property Key As String Implements IKeyedEntity(Of String).Key
            Get
                Return name
            End Get
            Set(value As String)
                name = value
            End Set
        End Property

        Public ReadOnly Property Expression(Optional null$ = "Nothing",
                                            Optional stringEscaping As Func(Of String, String) = Nothing,
                                            Optional isVar As Assert(Of String) = Nothing) As String
            Get
                Dim val$

                Static [isNot] As New DefaultValue(Of Assert(Of String))(Function(var) False)

                If value Is Nothing Then
                    val = null
                ElseIf value.GetType Is GetType(String) Then
                    If (isVar Or [isNot])(value) Then
                        val = value
                    Else
                        val = $"""{(stringEscaping Or noEscaping)(value)}"""
                    End If
                ElseIf value.GetType Is GetType(Char) Then
                    val = $"""{value}"""
                Else
                    val = value
                End If

                Return $"{name} = {val}"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"Dim {name} As Object = {Scripting.ToString(value, "null")}"
        End Function

        ''' <summary>
        ''' Argument variable value assign
        ''' </summary>
        ''' <param name="var">The argument name</param>
        ''' <param name="value">argument value</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator =(var As ArgumentReference, value As Object) As ArgumentReference
            var.value = value
            Return var
        End Operator

        Public Shared Operator <>(var As ArgumentReference, value As Object) As ArgumentReference
            Throw New NotImplementedException
        End Operator
    End Class

    Public Class TypeSchema

        Public ReadOnly Property Type As Type

        Sub New(type As Type)
            Me.Type = type
        End Sub

        Public Overrides Function ToString() As String
            Return Type.FullName
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator And(info As TypeSchema, types As Type()) As Boolean
            Return types.All(Function(t) Equals(info.Type, base:=t))
        End Operator

        Private Overloads Shared Function Equals(info As Type, base As Type) As Boolean
            If info.IsInheritsFrom(base) Then
                Return True
            Else
                If base.IsInterface AndAlso info.ImplementInterface(base) Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator Or(info As TypeSchema, types As Type()) As Boolean
            Return types.Any(Function(t) Equals(info.Type, base:=t))
        End Operator
    End Class

    ''' <summary>
    ''' Runtime helper
    ''' 
    ''' ```vbnet
    ''' Imports VB = Microsoft.VisualBasic.Language.Runtime
    ''' 
    ''' With New VB
    '''     ' ...
    ''' End With
    ''' ```
    ''' </summary>
    Public Class Runtime

        ''' <summary>
        ''' Language syntax supports for argument list
        ''' </summary>
        ''' <param name="name$"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property Argument(name$) As ArgumentReference
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New ArgumentReference With {
                    .name = name
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return "sciBASIC for VB.NET language runtime API"
        End Function
    End Class
End Namespace
