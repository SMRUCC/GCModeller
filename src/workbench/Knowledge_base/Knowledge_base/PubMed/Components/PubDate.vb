#Region "Microsoft.VisualBasic::56bc86eea939d67d01d36bab91979c1a, ..\workbench\Knowledge_base\Knowledge_base\PubMed\Components\PubDate.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ValueTypes

Public Class PubDate

    <XmlAttribute> Public Property PubStatus As String
    <XmlAttribute> Public Property DateType As String

    Public Property Year As String
    Public Property Month As String
    Public Property Day As String
    Public Property Hour As String
    Public Property Minute As String

    Public Overrides Function ToString() As String
        Return CType(Me, Date).ToString
    End Function

    Public Overloads Shared Narrowing Operator CType(d As PubDate) As Date
        Return New Date(d.Year, DateTimeHelper.GetMonthInteger(d.Month), d.Day)
    End Operator
End Class
