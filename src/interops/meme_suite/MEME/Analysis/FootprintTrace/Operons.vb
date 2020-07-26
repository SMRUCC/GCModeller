#Region "Microsoft.VisualBasic::717af485971df8def5e191ebbb866a58, meme_suite\MEME\Analysis\FootprintTrace\Operons.vb"

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

    '     Module OperonFootprints
    ' 
    '         Function: __copy, __expands, AssignDOOR, ExpandDOOR
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports stdNum = System.Math

Namespace Analysis.FootprintTraceAPI

    ''' <summary>
    ''' 操作Operon调控相关的信息
    ''' </summary>
    <Package("Operon.Footprints")>
    Public Module OperonFootprints

        ''' <summary>
        ''' 为调控关系之中的基因联系上操纵子的信息
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="DOOR"></param>
        ''' <returns></returns>
        <ExportAPI("Assign.DOOR")>
        <Extension>
        Public Function AssignDOOR(source As IEnumerable(Of PredictedRegulationFootprint), DOOR As DOOR) As List(Of PredictedRegulationFootprint)
            Dim LQuery = (From x In source Select x Group x By x.ORF Into Group)
            Dim result As New List(Of PredictedRegulationFootprint)

            For Each block In LQuery
                If String.IsNullOrEmpty(block.ORF) Then
                    result += block.Group
                Else
                    Dim gene = DOOR.GetGene(block.ORF)
                    Dim footprints = block.Group.ToArray

                    If Not gene Is Nothing Then ' RNA 基因没有记录，则会返回空值 
                        Dim first As Boolean = String.Equals(DOOR.DOOROperonView.GetOperon(gene.OperonID).InitialX.Synonym, gene.Synonym, StringComparison.Ordinal)
                        Dim flag As Char = If(first, "1"c, "0"c)

                        For Each x As PredictedRegulationFootprint In footprints
                            x.DoorId = gene.OperonID
                            x.InitX = flag
                        Next
                    End If
                    result += footprints
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 假若被调控的基因是操纵子的第一个基因，则后面的基因假设都会被一同调控
        ''' </summary>
        ''' <param name="footprints"></param>
        ''' <param name="DOOR"></param>
        ''' <returns>新拓展出来的数据以及原来的数据</returns>
        <ExportAPI("Expand.DOOR")>
        <Extension>
        Public Function ExpandDOOR(footprints As IEnumerable(Of PredictedRegulationFootprint),
                                   DOOR As DOOR,
                                   coors As Correlation2,
                                   cut As Double) As List(Of PredictedRegulationFootprint)
            Dim LQuery = (From x As PredictedRegulationFootprint
                          In footprints
                          Where x.InitX.ParseBoolean
                          Select x,
                              opr = DOOR.GetOperon(x.ORF))
            Dim expands = (From x In LQuery Select x.x.__expands(x.opr, coors)).IteratesALL
            Dim cuts = (From x As PredictedRegulationFootprint
                        In expands.AsParallel
                        Where stdNum.Abs(x.Pcc) >= cut OrElse
                            stdNum.Abs(x.sPcc) >= cut
                        Select x).AsList
            Return cuts + footprints
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="operon"></param>
        ''' <returns>原来的数据将不会被添加</returns>
        <Extension>
        Private Function __expands(x As PredictedRegulationFootprint, operon As Operon, corrs As Correlation2) As PredictedRegulationFootprint()
            Dim genes = (From g As OperonGene In operon.Genes
                         Where Not String.Equals(g.Synonym, x.ORF)
                         Select g) ' 由于操纵的第一个基因的调控数据已经有了，所以在这里筛选掉
            Dim LQuery = (From g As OperonGene In genes Select x.__copy(g, corrs)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 这里主要是拓展Trace信息
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="g"></param>
        ''' <param name="corrs">操纵子的数据可能会有预测错误的，所以在这里任然需要转录组数据进行筛选</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Private Function __copy(x As PredictedRegulationFootprint, g As OperonGene, corrs As Correlation2) As PredictedRegulationFootprint
            Dim footprint As PredictedRegulationFootprint = x.Clone

            ' 由于操纵子的模式是连带调控的。所以调控位点的信息不会被修改，任然是第一个基因的信息

            footprint.ORF = g.Synonym
            footprint.InitX = "0"c
            footprint.MotifTrace = g.OperonID & "@" & footprint.MotifTrace
            footprint.Pcc = corrs.GetPcc(footprint.Regulator, g.Synonym)
            footprint.sPcc = corrs.GetSPcc(footprint.Regulator, g.Synonym)

            Return footprint
        End Function
    End Module
End Namespace
