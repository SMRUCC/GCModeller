#Region "Microsoft.VisualBasic::6043a081a985ab2041b7e13cb56e2f6a, ..\GCModeller\core\Bio.Assembly\ComponentModel\GeneBrief.vb"

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

Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel

    ''' <summary>
    ''' The basically information of a gene object.(这个接口对象表示了一个在计算机程序之中的最基本的基因信息的载体对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IGeneBrief : Inherits ICOGDigest, IContig
    End Interface

    ''' <summary>
    ''' The COG annotation data of the genes.(基因对象的COG注释结果)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ICOGDigest : Inherits sIdEnumerable

        ''' <summary>
        ''' The gene object COG classification.(COG功能分类)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property COG As String
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
End Namespace
