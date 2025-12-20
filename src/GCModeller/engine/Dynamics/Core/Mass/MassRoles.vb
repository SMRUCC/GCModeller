#Region "Microsoft.VisualBasic::f56ec3f31c900fe4d84bee362e1aba39, engine\Dynamics\Core\Mass\MassRoles.vb"

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

    '   Total Lines: 25
    '    Code Lines: 12 (48.00%)
    ' Comment Lines: 12 (48.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 1 (4.00%)
    '     File Size: 538 B


    '     Enum MassRoles
    ' 
    '         compound, gene, mRNA, polypeptide, protein
    '         RNA, rRNA, tRNA
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
