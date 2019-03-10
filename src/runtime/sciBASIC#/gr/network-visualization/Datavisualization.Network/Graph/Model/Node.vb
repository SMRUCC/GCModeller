﻿#Region "Microsoft.VisualBasic::78d42255adf7839b2dc3cef2b0efbf6c, gr\network-visualization\Datavisualization.Network\Graph\Model\Node.vb"

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

    '     Class Node
    ' 
    '         Properties: Data, Pinned
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: (+2 Overloads) Equals, GetHashCode, ToString
    '         Operators: <>, =
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'! 
'@file Node.cs
'@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
'		<http://github.com/juhgiyo/epForceDirectedGraph.cs>
'@date August 08, 2013
'@brief Node Interface
'@version 1.0
'
'@section LICENSE
'
'The MIT License (MIT)
'
'Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in
'all copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
'THE SOFTWARE.
'
'@section DESCRIPTION
'
'An Interface for the Node Class.
'
'

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Graph

    ''' <summary>
    ''' <see cref="Node.Label"/> -> <see cref="INamedValue.Key"/>
    ''' </summary>
    Public Class Node : Inherits GraphTheory.Network.Node
        Implements INamedValue
        Implements IGraphValueContainer(Of NodeData)

        ''' <summary>
        ''' 在这里是用的是unique id进行初始化，对于Display title则可以在<see cref="NodeData.label"/>属性上面设置
        ''' </summary>
        ''' <param name="iId"></param>
        ''' <param name="iData"></param>
        Public Sub New(iId As String, Optional iData As NodeData = Nothing)
            If iData IsNot Nothing Then
                Data = iData.Clone
            End If

            Label = iId
            Pinned = False
        End Sub

        Sub New()
            Call Me.New(Nothing, Nothing)
        End Sub

        Public Property Data As NodeData Implements IGraphValueContainer(Of NodeData).Data
        Public Property Pinned As Boolean

        Public Overrides Function GetHashCode() As Integer
            Return Label.GetHashCode()
        End Function

        Public Overrides Function ToString() As String
            If Not Data Is Nothing AndAlso Not Data.label.StringEmpty Then
                Return $"{Label} ({Data.label})"
            Else
                Return Label
            End If
        End Function

        Public Overrides Function Equals(obj As System.Object) As Boolean
            ' If parameter is null return false.
            If obj Is Nothing Then
                Return False
            Else
                ' If parameter cannot be cast to Point return false.
                Return Equals(p:=TryCast(obj, Node))
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function Equals(p As Node) As Boolean
            ' If parameter is null return false:
            If p Is Nothing Then
                Return False
            Else
                ' Return true if the fields match:
                Return (Label = p.Label)
            End If
        End Function

        Public Shared Operator =(a As Node, b As Node) As Boolean
            ' If one is null, but not both, return false.
            If a Is Nothing OrElse b Is Nothing Then
                Return False
            End If

            ' If both are null, or both are same instance, return true.
            If Object.ReferenceEquals(a, b) Then
                Return True
            Else
                Return a.Equals(p:=b)
            End If
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator <>(a As Node, b As Node) As Boolean
            Return Not (a = b)
        End Operator
    End Class
End Namespace
