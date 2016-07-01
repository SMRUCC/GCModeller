#Region "Microsoft.VisualBasic::34db857677a805c13738c65119871588, ..\GCModeller\models\Networks\Network.BLAST\LDM\Hit.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH
Imports Microsoft.VisualBasic.DataVisualization.Network.Abstract

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
        Public Property weight As Double Implements INetworkEdge.Confidence
        Public Property query As String Implements IInteraction.source, IBlastHit.locusId
        ''' <summary>
        ''' 基因组或者蛋白质家族的配对字符串，在进行字符串连接之前先按照字母顺序排序
        ''' </summary>
        ''' <returns></returns>
        Public Property genomePairId As String Implements INetworkEdge.InteractionType
        Public Property subject As String Implements IInteraction.target, IBlastHit.Address

        Public Overrides Function ToString() As String
            Return $"{query} => {subject}"
        End Function
    End Class
End Namespace
