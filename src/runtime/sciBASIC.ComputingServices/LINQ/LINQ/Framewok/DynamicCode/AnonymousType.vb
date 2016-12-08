#Region "Microsoft.VisualBasic::e5065ea26e9229ffda673c7c0eadc362, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\DynamicCode\AnonymousType.vb"

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

Namespace Framework.DynamicCode

    Public Class AnonymousType

        <XmlArray> Public Property Properties As [Property]()

        Default Public ReadOnly Property [Property](Name As String) As Object
            Get
                Dim LQuery = From p As [Property] In Me.Properties
                             Where String.Equals(Name, p.Name, StringComparison.OrdinalIgnoreCase)
                             Select p '
                Dim result As [Property] = LQuery.FirstOrDefault
                Return result
            End Get
        End Property
    End Class

    Public Class [Property]
        <XmlAttribute> Public Property Name As String
        <XmlElement> Public Property Value As Object

        Public Overrides Function ToString() As String
            Return String.Format("{0}:={1}", Name, Value.ToString)
        End Function
    End Class
End Namespace
