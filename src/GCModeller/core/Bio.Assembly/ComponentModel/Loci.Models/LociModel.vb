#Region "Microsoft.VisualBasic::eda6721c14fa1ab05bca64a17f212fda, ..\GCModeller\core\Bio.Assembly\ComponentModel\Loci.Models\Loci.vb"

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

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace ComponentModel.Loci

    ''' <summary>
    ''' 带标签的位置信息，只不过这个位点的位置信息是展开的
    ''' </summary>
    Public Class Loci : Implements ILocationComponent
        Implements IMotifSite

        Public Property TagData As String Implements IMotifSite.Type, IMotifSite.Name
        Public Property Left As Integer Implements ILocationComponent.Left
        Public Property Right As Integer Implements ILocationComponent.Right

        Private Property Site As Location Implements IMotifSite.Site
            Get
                Return New Location(Left, Right)
            End Get
            Set(value As Location)
                With value
                    Left = .Left
                    Right = .Right
                End With
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return TagData
        End Function
    End Class

    ''' <summary>
    ''' 带标签的位点信息，只不过这个位点的位置信息是构建好的
    ''' </summary>
    Public Structure MotifSite
        Implements IMotifSite

        Public Property Name As String Implements IMotifSite.Name
        Public Property Site As Location Implements IMotifSite.Site
        Public Property Type As String Implements IMotifSite.Type

    End Structure
End Namespace
