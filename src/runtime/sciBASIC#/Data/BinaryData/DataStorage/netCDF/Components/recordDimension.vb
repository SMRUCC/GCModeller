﻿#Region "Microsoft.VisualBasic::a284bd5f6e222f15c47a0ab5a8dd2b31, Data\BinaryData\DataStorage\netCDF\Components\recordDimension.vb"

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

    '     Class recordDimension
    ' 
    '         Properties: id, length, name, recordStep
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace netCDF.Components

    ''' <summary>
    ''' Metadata for the record dimension
    ''' </summary>
    Public Class recordDimension

        ''' <summary>
        ''' Number of elements in the record dimension
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property length As Integer
        ''' <summary>
        ''' Id number In the list Of dimensions For the record dimension
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property id As Integer
        ''' <summary>
        ''' String with the name of the record dimension
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' Number with the record variables step size
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property recordStep As Integer

        Public Overrides Function ToString() As String
            Return $"[{id}] {name} ({recordStep}x{length})"
        End Function
    End Class
End Namespace
