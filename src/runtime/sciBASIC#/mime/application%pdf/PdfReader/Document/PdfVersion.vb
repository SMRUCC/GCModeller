﻿#Region "Microsoft.VisualBasic::74c4c24445cef1beb565c6f91e1fc367, mime\application%pdf\PdfReader\Document\PdfVersion.vb"

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

    '     Class PdfVersion
    ' 
    '         Properties: Major, Minor
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ToString
    ' 
    '         Sub: Visit
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace PdfReader
    Public Class PdfVersion
        Inherits PdfObject

        Private _Major As Integer, _Minor As Integer

        Public Sub New(ByVal parent As PdfObject, ByVal major As Integer, ByVal minor As Integer)
            MyBase.New(parent)
            Me.Major = major
            Me.Minor = minor
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Major}.{Minor}"
        End Function

        Public Overrides Sub Visit(ByVal visitor As IPdfObjectVisitor)
            visitor.Visit(Me)
        End Sub

        Public Property Major As Integer
            Get
                Return _Major
            End Get
            Private Set(ByVal value As Integer)
                _Major = value
            End Set
        End Property

        Public Property Minor As Integer
            Get
                Return _Minor
            End Get
            Private Set(ByVal value As Integer)
                _Minor = value
            End Set
        End Property
    End Class
End Namespace
