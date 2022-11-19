#Region "Microsoft.VisualBasic::b918e54a0fa639d282c7a47cd33804be, GCModeller\core\Bio.Assembly\ComponentModel\Locus\LociSites\LociModel.vb"

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


    ' Code Statistics:

    '   Total Lines: 44
    '    Code Lines: 30
    ' Comment Lines: 6
    '   Blank Lines: 8
    '     File Size: 1.46 KB


    '     Class Loci
    ' 
    '         Properties: Left, Right, Site, TagData
    ' 
    '         Function: ToString
    ' 
    '     Structure MotifSite
    ' 
    '         Properties: Name, Site, Type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace ComponentModel.Loci

    ''' <summary>
    ''' 带标签的位置信息，只不过这个位点的位置信息是展开的
    ''' </summary>
    Public Class Loci : Implements ILocationComponent
        Implements IMotifSite

        Public Property TagData As String Implements IMotifSite.name, IMotifSite.family
        Public Property Left As Integer Implements ILocationComponent.left
        Public Property Right As Integer Implements ILocationComponent.right

        Private Property Site As Location Implements IMotifSite.site
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

        Public Property Name As String Implements IMotifSite.name
        Public Property Site As Location Implements IMotifSite.site
        Public Property Type As String Implements IMotifSite.family

    End Structure
End Namespace
