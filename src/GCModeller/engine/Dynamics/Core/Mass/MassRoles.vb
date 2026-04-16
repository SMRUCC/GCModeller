#Region "Microsoft.VisualBasic::e227c16b79793d78f0a81fcfdf42eced, engine\Dynamics\Core\Mass\MassRoles.vb"

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

    '   Total Lines: 46
    '    Code Lines: 13 (28.26%)
    ' Comment Lines: 30 (65.22%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (6.52%)
    '     File Size: 1.33 KB


    '     Enum MassRoles
    ' 
    '         compound, gene, mRNA, polypeptide, protein
    '         RNA, rRNA, status, tRNA
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
        ''' <summary>
        ''' gene template of the RNA molecules, usually be a constant 1, binding to unchanged.
        ''' </summary>
        gene

        ''' <summary>
        ''' realtime gene instance molecule, could be translate to protein
        ''' </summary>
        mRNA
        ''' <summary>
        ''' realtime gene instance molecule
        ''' </summary>
        tRNA
        ''' <summary>
        ''' realtime gene instance molecule,
        ''' </summary>
        rRNA
        ''' <summary>
        ''' realtime gene instance molecule, other RNA molecules
        ''' </summary>
        RNA
        ''' <summary>
        ''' realtime gene instance molecule,
        ''' </summary>
        polypeptide
        ''' <summary>
        ''' realtime gene instance molecule, 蛋白包括单体蛋白或者复合物蛋白
        ''' </summary>
        protein
        ''' <summary>
        ''' metabolite instance molecule object. 小分子化合物
        ''' </summary>
        compound

        ''' <summary>
        ''' mapping to the cell status view: <see cref="StatusMapFactor"/>
        ''' </summary>
        status
    End Enum
End Namespace
