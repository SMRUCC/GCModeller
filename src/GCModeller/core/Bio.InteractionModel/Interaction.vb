#Region "Microsoft.VisualBasic::a64a9781890dec0c57864a0bdd2fd2af, ..\GCModeller\core\Bio.InteractionModel\Interaction.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' 默认的的相互作用的方向为从A到B
''' </summary>
''' <remarks></remarks>
Public Class Interaction(Of T As sIdEnumerable)

    Public Property ObjectA As T
    Public Property Interaction As String
    Public Property ObjectB As T

    Public ReadOnly Property Broken As Boolean
        Get
            Return ObjectB Is Nothing OrElse ObjectA Is Nothing
        End Get
    End Property

    Public Overrides Function ToString() As String
        If Broken Then
            If ObjectA Is Nothing AndAlso ObjectB Is Nothing Then
                Return ""
            ElseIf ObjectA Is Nothing Then
                Return ObjectB.Identifier
            Else
                Return ObjectA.Identifier
            End If
        End If
        Return String.Format("{0}" & vbTab & "{1}" & vbTab & "{2}", ObjectA.Identifier, Interaction, ObjectB.Identifier)
    End Function

    ''' <summary>
    ''' 判断两个Interaction对象是否相同
    ''' </summary>
    ''' <param name="objA"></param>
    ''' <param name="objB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Equals(objA As T, objB As T, Optional Direction As Boolean = False) As Boolean
        If Not Direction Then
            Return (String.Equals(objA.Identifier, ObjectA.Identifier) AndAlso String.Equals(objB.Identifier, ObjectB.Identifier)) OrElse
                       (String.Equals(objB.Identifier, ObjectA.Identifier) AndAlso String.Equals(objA.Identifier, ObjectB.Identifier))
        Else
            Return String.Equals(objA.Identifier, ObjectA.Identifier) AndAlso String.Equals(objB.Identifier, ObjectB.Identifier)
        End If
    End Function

    ''' <summary>
    ''' 判断两个Interaction对象是否相同
    ''' </summary>
    ''' <param name="objA"></param>
    ''' <param name="objB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Equals(objA As String, objB As String, Optional Direction As Boolean = False) As Boolean
        If Not Direction Then
            Return (String.Equals(objA, ObjectA.Identifier) AndAlso String.Equals(objB, ObjectB.Identifier)) OrElse
                       (String.Equals(objB, ObjectA.Identifier) AndAlso String.Equals(objA, ObjectB.Identifier))
        Else
            Return String.Equals(objA, ObjectA.Identifier) AndAlso String.Equals(objB, ObjectB.Identifier)
        End If
    End Function

    Public Shared Function Generate(Data As Interaction(Of T)()) As String()
        Dim result = (From item In Data Let iter = Function() item.ToString Select iter()).ToArray
        Return result
    End Function
End Class
