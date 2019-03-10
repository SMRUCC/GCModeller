﻿#Region "Microsoft.VisualBasic::49dc398955cafc610639c9fa19231350, mime\application%netcdf\Components\Components.vb"

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

    '     Structure Dimension
    ' 
    '         Function: ToString
    ' 
    '     Class DimensionList
    ' 
    '         Properties: dimensions, HaveRecordDimension, recordId, recordName
    ' 
    '         Function: ToString
    ' 
    '     Class recordDimension
    ' 
    '         Properties: id, length, name, recordStep
    ' 
    '         Function: ToString
    ' 
    '     Class attribute
    ' 
    '         Properties: name, type, value
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class variable
    ' 
    '         Properties: attributes, dimensions, name, offset, record
    '                     size, type, value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Components

    ''' <summary>
    ''' ``[name => size]``
    ''' </summary>
    ''' 
    <XmlType("dim", [Namespace]:=Xml.netCDF)>
    Public Structure Dimension

        ''' <summary>
        ''' String with the name of the dimension
        ''' </summary>
        <XmlAttribute> Dim name As String
        ''' <summary>
        ''' Number with the size of the dimension
        ''' </summary>
        <XmlText>
        Dim size As Integer

        Public Overrides Function ToString() As String
            Return $"{name}(size={size})"
        End Function
    End Structure

    Public Class DimensionList

        <XmlAttribute> Public Property recordId As Integer?
        <XmlAttribute> Public Property recordName As String

        Public ReadOnly Property HaveRecordDimension As Boolean
            Get
                Return Not (recordId Is Nothing AndAlso recordName = "NA")
            End Get
        End Property

        Public Property dimensions As Dimension()

        Public Overrides Function ToString() As String
            Return $"[{recordId}] {recordName}"
        End Function
    End Class

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

    ''' <summary>
    ''' 属对象性,主要是记录一些注解信息
    ''' </summary>
    Public Class attribute

        ''' <summary>
        ''' String with the name of the attribute
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' String with the type of the attribute
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property type As CDFDataTypes
        ''' <summary>
        ''' A number or string with the value of the attribute
        ''' </summary>
        ''' <returns></returns>
        <XmlText>
        Public Property value As String

        Sub New()
        End Sub

        Sub New(name$, value$, type As CDFDataTypes)
            Me.name = name
            Me.value = value
            Me.type = type
        End Sub

        Public Overrides Function ToString() As String
            Return $"Dim {name} As {type.Description} = {value}"
        End Function
    End Class

    ''' <summary>
    ''' 变量对象,CDF文件之中的实验数据之类的数据都是保存于这个对象之中的
    ''' </summary>
    Public Class variable

        ''' <summary>
        ''' String with the name of the variable
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' Array with the dimension IDs of the variable.
        ''' (<see cref="Header.dimensions"/>)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property dimensions As Integer()
        ''' <summary>
        ''' Array with the attributes of the variable
        ''' </summary>
        ''' <returns></returns>
        Public Property attributes As attribute()
        ''' <summary>
        ''' String with the type of the variable
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property type As CDFDataTypes
        ''' <summary>
        ''' Number with the size of the variable.(在文件之中的数据字节大小)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property size As Integer
        ''' <summary>
        ''' Number with the offset where of the variable begins
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property offset As UInteger
        ''' <summary>
        ''' True if Is a record variable, false otherwise
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property record As Boolean

        Public Property value As CDFData

        Public Overrides Function ToString() As String
            Return $"Dim {name}[offset={offset}] As {type.Description}"
        End Function
    End Class
End Namespace
