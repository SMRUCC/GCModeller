#Region "Microsoft.VisualBasic::cb0850a0186416c246d062a30b724a1f, STRING\Models\Regulation.vb"

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

    '     Class TFRegulation
    ' 
    '         Properties: OperonGenes, OperonId, PromoterGene, Regulator, TFPcc
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace Models

    ''' <summary>
    ''' 本类型可以通过CSV模块兼容CSV表格类型的计算模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TFRegulation : Implements INamedValue

        ''' <summary>
        ''' 通常为属性<see cref="SMRUCC.genomics.Assembly.Door.OperonGene.OperonID"></see>的这个编号值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Operon-Id")> Public Property OperonId As String
        <Column("Regulator")> Public Property Regulator As String Implements INamedValue.Key
        <CollectionAttribute("Operon-Genes")> Public Property OperonGenes As String()
        Public Property PromoterGene As String
        ''' <summary>
        ''' Regulator对第一个基因的Pcc值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TFPcc As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", Regulator, OperonId)
        End Function
    End Class
End Namespace
