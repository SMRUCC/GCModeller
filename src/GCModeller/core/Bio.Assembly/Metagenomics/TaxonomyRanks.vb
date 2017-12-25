#Region "Microsoft.VisualBasic::08e91e3c0bb4941fddd9a85f5b4fa5ab, ..\GCModeller\core\Bio.Assembly\Metagenomics\TaxonomyRanks.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Namespace Metagenomics

    ''' <summary>
    ''' 枚举值减去100即可得到index值
    ''' </summary>
    Public Enum TaxonomyRanks As Integer
        NA
        ''' <summary>
        ''' 1. 界
        ''' </summary>
        Kingdom = 100
        ''' <summary>
        ''' 2. 门
        ''' </summary>
        Phylum
        ''' <summary>
        ''' 3A. 纲
        ''' </summary>
        [Class]
        ''' <summary>
        ''' 4B. 目
        ''' </summary>
        Order
        ''' <summary>
        ''' 5C. 科
        ''' </summary>
        Family
        ''' <summary>
        ''' 6D. 属
        ''' </summary>
        Genus
        ''' <summary>
        ''' 7E. 种
        ''' </summary>
        Species
        Strain
    End Enum
End Namespace
