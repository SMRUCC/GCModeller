#Region "Microsoft.VisualBasic::4d4dcb119b5b04370920d8a0b7a2de20, engine\AutoCAD\GeneticComponents\GeneticNode.vb"

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

    ' Class GeneticNode
    ' 
    '     Properties: [Function], Accession, GO, ID, KO
    '                 Nt, Sequence, Xref
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Polypeptides

''' <summary>
''' A node entity in the genetic component network
''' </summary>
Public Class GeneticNode

    ''' <summary>
    ''' 一般是Uniprot蛋白编号
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String
    ''' <summary>
    ''' 一般是Nt库之中的核酸序列编号
    ''' </summary>
    ''' <returns></returns>
    Public Property Accession As String
    Public Property GO As String()
    Public Property KO As String
    ''' <summary>
    ''' 蛋白序列
    ''' </summary>
    ''' <returns></returns>
    Public Property Sequence As AminoAcid()
    ''' <summary>
    ''' 核酸序列
    ''' </summary>
    ''' <returns></returns>
    Public Property Nt As DNA()
    ''' <summary>
    ''' 简单的功能描述
    ''' </summary>
    ''' <returns></returns>
    Public Property [Function] As String
    ''' <summary>
    ''' 这个节点的数据源之中的原始编号
    ''' </summary>
    ''' <returns></returns>
    Public Property Xref As String

    Public Overrides Function ToString() As String
        Return [Function]
    End Function

End Class
