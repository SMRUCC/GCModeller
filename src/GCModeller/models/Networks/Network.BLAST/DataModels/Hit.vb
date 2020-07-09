#Region "Microsoft.VisualBasic::1b935b1be1d32f59d5da59e9dd2868cd, Network.BLAST\DataModels\Hit.vb"

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

    '     Class Hit
    ' 
    '         Properties: genomePairId, query, subject, weight
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace LDM

    ''' <summary>
    ''' blast之中的一个hit相当于网络之中的一条边
    ''' </summary>
    Public Class Hit
        Implements INetworkEdge, IBlastHit

        ''' <summary>
        ''' 相当于identities值
        ''' </summary>
        ''' <returns></returns>
        Public Property weight As Double Implements INetworkEdge.value
        Public Property query As String Implements IInteraction.source, IBlastHit.queryName
        ''' <summary>
        ''' 基因组或者蛋白质家族的配对字符串，在进行字符串连接之前先按照字母顺序排序
        ''' </summary>
        ''' <returns></returns>
        Public Property genomePairId As String Implements INetworkEdge.Interaction
        Public Property subject As String Implements IInteraction.target, IBlastHit.hitName

        Public Overrides Function ToString() As String
            Return $"{query} => {subject}"
        End Function
    End Class
End Namespace
