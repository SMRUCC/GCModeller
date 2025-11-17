#Region "Microsoft.VisualBasic::b455f631a06d8959cfe27d806c0e36fb, core\Bio.Assembly\ComponentModel\Locus\Interfaces.vb"

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

    '   Total Lines: 59
    '    Code Lines: 18 (30.51%)
    ' Comment Lines: 34 (57.63%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (11.86%)
    '     File Size: 1.86 KB


    '     Interface ILocationSegment
    ' 
    '         Properties: Location, UniqueId
    ' 
    '     Interface IContig
    ' 
    '         Properties: Location
    ' 
    '     Interface ILocationNucleotideSegment
    ' 
    '         Properties: Strand
    ' 
    '     Interface ILocationComponent
    ' 
    '         Properties: right
    ' 
    '     Interface ILoci
    ' 
    '         Properties: left
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ComponentModel.Loci.Abstract

    Public Interface ILocationSegment
        ''' <summary>
        ''' Tag data on this location sequence segment.(当前的这个序列片段的标签信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property UniqueId As String
        ''' <summary>
        ''' The location value of this sequence segment.(这个序列片段的位置信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Location As Location
    End Interface

    ''' <summary>
    ''' Abstract model with location data.
    ''' </summary>
    Public Interface IContig
        Property Location As NucleotideLocation
    End Interface

    Public Interface ILocationNucleotideSegment : Inherits ILocationSegment
        ReadOnly Property Strand As Strands
    End Interface

    Public Interface INucleotideLocation : Inherits ILocationComponent

        Property Strand As Strands

    End Interface

    ''' <summary>
    ''' This type of the object has the loci location value attribute.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ILocationComponent : Inherits ILoci

        ''' <summary>
        ''' Right position of the loci site on sequence.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property right As Integer
    End Interface

    ''' <summary>
    ''' 只有左端起始位点的模型对象
    ''' </summary>
    Public Interface ILoci

        ''' <summary>
        ''' Left position of the loci site on sequence.(左端起始位点)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property left As Integer
    End Interface
End Namespace
