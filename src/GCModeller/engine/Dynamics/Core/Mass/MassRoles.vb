#Region "Microsoft.VisualBasic::87fb3367b6274af78c3340eff4a4d5e3, GCModeller\engine\Dynamics\Core\Mass\MassRoles.vb"

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

    '   Total Lines: 21
    '    Code Lines: 11
    ' Comment Lines: 9
    '   Blank Lines: 1
    '     File Size: 445 B


    '     Enum MassRoles
    ' 
    '         compound, gene, mRNA, popypeptide, protein
    '         rRNA, tRNA
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Core

    ''' <summary>
    ''' 物质的角色分类类型
    ''' </summary>
    Public Enum MassRoles
        gene
        mRNA
        tRNA
        rRNA
        popypeptide
        ''' <summary>
        ''' 蛋白包括单体蛋白或者复合物蛋白
        ''' </summary>
        protein
        ''' <summary>
        ''' 小分子化合物
        ''' </summary>
        compound
    End Enum
End Namespace
