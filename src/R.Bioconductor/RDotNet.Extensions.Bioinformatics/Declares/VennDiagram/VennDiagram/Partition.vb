#Region "Microsoft.VisualBasic::2f3fe446b081b25aad40c8ec696d33d2, ..\R.Bioconductor\RDotNet.Extensions.Bioinformatics\Declares\VennDiagram\VennDiagram\Partition.vb"

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
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace VennDiagram.ModelAPI

    ''' <summary>
    ''' A partition in the venn diagram.
    ''' </summary>
    Public Class Partition : Inherits ClassObject
        Implements INamedValue

        ''' <summary>
        ''' The name of this partition
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Name As String Implements INamedValue.Key
        ''' <summary>
        ''' The color string of the partition
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Color As String
        Public Property Title As String

        ''' <summary>
        ''' 使用数字来表示成员的一个向量
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlText> Public Property Vector As String

        Public ReadOnly Property DisplName As String
            Get
                If String.IsNullOrEmpty(Title) Then
                    Return Name
                Else
                    Return Title
                End If
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(Name As String)
            Me.Name = Name
        End Sub

        Public Function ApplyOptions([Option] As String()) As Partition
            Name = [Option].First
            Color = [Option].Get(1)
            Title = [Option].Get(2)
            Console.WriteLine("{0}(color: {1}) {2} counts.", Me.Name, Me.Color, Me.Vector.Split(CChar(",")).Length)
            Return Me
        End Function
    End Class
End Namespace
