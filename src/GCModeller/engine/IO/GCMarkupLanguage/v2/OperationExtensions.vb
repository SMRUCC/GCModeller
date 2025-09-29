﻿#Region "Microsoft.VisualBasic::598ae840a11ac28aae0ef3a011ef846d, engine\IO\GCMarkupLanguage\v2\OperationExtensions.vb"

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

    '   Total Lines: 94
    '    Code Lines: 62 (65.96%)
    ' Comment Lines: 22 (23.40%)
    '    - Xml Docs: 54.55%
    ' 
    '   Blank Lines: 10 (10.64%)
    '     File Size: 3.89 KB


    '     Module OperationExtensions
    ' 
    '         Function: DeleteMutation, isDisconnectedNode, Trim
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace v2

    Public Module OperationExtensions

        ''' <summary>
        ''' 基因组之中的基因发生了缺失突变
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="knockoutGeneList$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function DeleteMutation(model As VirtualCell, knockoutGeneList$()) As VirtualCell
            Dim suffix As String = If(knockoutGeneList.Length = 1, "-" & knockoutGeneList(0), $"-({knockoutGeneList.JoinBy(",")})")
            Dim knockouts As Index(Of String) = knockoutGeneList.Indexing

            Return New VirtualCell With {
                .cellular_id = model.cellular_id & suffix,
                .taxonomy = model.taxonomy,
                .properties = model.properties,
                .metabolismStructure = model.metabolismStructure,
                .genome = New Genome With {
                    .proteins = model.genome.proteins,
                    .regulations = model.genome.regulations,
                    .replicons = model.genome.replicons _
                        .Select(Function(rep)
                                    Return New replicon With {
                                        .genomeName = rep.genomeName & suffix,
                                        .isPlasmid = rep.isPlasmid,
                                        .RNAs = rep.RNAs _
                                            .Where(Function(r) Not (r.gene Like knockouts)) _
                                            .ToArray,
                                        .operons = rep.operons _
                                            .Select(Function(o)
                                                        Return New TranscriptUnit With {
                                                            .id = o.id,
                                                            .name = o.name,
                                                            .note = o.note,
                                                            .sites = o.sites,
                                                            .genes = o.genes _
                                                                .Where(Function(g) Not (g.locus_tag Like knockouts)) _
                                                                .ToArray
                                                        }
                                                    End Function) _
                                            .ToArray
                                    }
                                End Function) _
                        .ToArray
                }
            }
        End Function

        ''' <summary>
        ''' 删除所有没有在当前的模型之中找到对应的酶的酶促反应过程
        ''' (不推荐使用这个操作，因为可以假设缺少酶的情况下，反应过程可以以非常低的效率进行，但是删除之后反应链将会完全断裂了)
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Trim(model As VirtualCell) As VirtualCell
            ' 得到在当前的虚拟细胞模型之中所有的可以被酶催化的酶促反应过程
            ' 的编号列表
            Dim allEnzymatics = model.metabolismStructure _
                .enzymes _
                .Select(Function(enz)
                            Return enz.catalysis.Select(Function(c) c.reaction)
                        End Function) _
                .IteratesALL _
                .Distinct _
                .Indexing

            model.metabolismStructure.reactions = model _
                .metabolismStructure _
                .reactions _
                .AsEnumerable _
                .Where(Function(r) Not r.isDisconnectedNode(allEnzymatics)) _
                .ToArray

            Return model
        End Function

        <Extension>
        Private Function isDisconnectedNode(r As Reaction, allEnzymatics As Index(Of String)) As Boolean
            If Not r.is_enzymatic Then
                ' 如果不需要酶，则可以自行发生
                ' 则不是断开的节点
                Return False
            Else
                If r.ID Like allEnzymatics Then
                    Return False
                Else
                    ' 是酶促反应过程，但是在模型之中找不到对应的酶
                    ' 则是一个断开的节点
                    Return True
                End If
            End If
        End Function
    End Module
End Namespace
