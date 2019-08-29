#Region "Microsoft.VisualBasic::47f9054671c2998f90fb58df3377d1b3, LocalBLAST\Pipeline\COG\Whog\Assigner.vb"

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

    '     Module Assigner
    ' 
    '         Function: DoAssign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Namespace Pipeline.COG.Whog

    Public Module Assigner

        ''' <summary>
        ''' Do COG category assign based on the whog data
        ''' </summary>
        ''' <param name="repo"></param>
        ''' <param name="prot"></param>
        ''' <returns></returns>
        <Extension>
        Public Function DoAssign(repo As WhogRepository, prot As MyvaCOG) As MyvaCOG
            If String.IsNullOrEmpty(prot.MyvaCOG) OrElse String.Equals(prot.MyvaCOG, IBlastOutput.HITS_NOT_FOUND) Then
                ' 没有可以分类的数据
                Return prot
            End If

            Dim Cog = (From entry As Category
                       In repo.Categories
                       Where entry.ContainsGene(prot.MyvaCOG)
                       Select entry).FirstOrDefault

            If Cog Is Nothing Then
                Call $"Could Not found the COG category id for myva cog {prot.QueryName} <-> {prot.MyvaCOG}....".Warning
                Return prot
            End If

            prot.COG = Cog.COG_id
            prot.Category = Cog.category
            prot.Description = Cog.description

            Return prot
        End Function
    End Module
End Namespace
