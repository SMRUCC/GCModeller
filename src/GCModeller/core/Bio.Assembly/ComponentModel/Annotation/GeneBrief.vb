#Region "Microsoft.VisualBasic::23e0f62f8a354c0baea6dafcb80fc2de, GCModeller\core\Bio.Assembly\ComponentModel\Annotation\GeneBrief.vb"

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

    '   Total Lines: 60
    '    Code Lines: 15
    ' Comment Lines: 35
    '   Blank Lines: 10
    '     File Size: 1.95 KB


    '     Interface IGeneBrief
    ' 
    '         Properties: Length, Product
    ' 
    '     Interface IFeatureDigest
    ' 
    '         Properties: Feature
    ' 
    '     Interface ICOGCatalog
    ' 
    '         Properties: Catalog, COG
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' The basically information of a gene object.(这个接口对象表示了一个在计算机程序之中的最基本的基因信息的载体对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IGeneBrief : Inherits IFeatureDigest, IContig

        ''' <summary>
        ''' The protein function annotation data of the gene coding product.(所编码的蛋白质产物的功能注释)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Product As String
        ''' <summary>
        ''' The nucleotide sequence length.(基因的长度)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Length As Integer

    End Interface

    ''' <summary>
    ''' The feature annotation data of the genes.(基因对象的特征注释结果)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IFeatureDigest : Inherits INamedValue

        ''' <summary>
        ''' The feature tag of this gene object for classification.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Feature As String

    End Interface

    ''' <summary>
    ''' 这个基因的注释结果之中除了COG编号之外，还有这个编号所属的COG分类
    ''' </summary>
    Public Interface ICOGCatalog : Inherits INamedValue

        ''' <summary>
        ''' The gene object COG classification.(COG功能分类)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property COG As String
        Property Catalog As String

    End Interface
End Namespace
