#Region "Microsoft.VisualBasic::c089379eb80199ac68b6cfb2e196261f, meme_suite\MEME\Analysis\VirtualFootprints\FootprintRegulations.vb"

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

    '     Class PredictedRegulationFootprint
    ' 
    '         Properties: MotifTrace, RegulatorTrace
    ' 
    '         Function: __createRegulationObject, Clone, Copy, OperonRegulationCopies
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.HTML
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.SequenceModel

Namespace Analysis.GenomeMotifFootPrints

    ''' <summary>
    ''' Regulation information from the MEME software and regprecise database.(使用MEME软件和regprecise数据库所生成的预测的调控信息)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PredictedRegulationFootprint : Inherits RegulatesFootprints
        Implements IInteraction
        Implements ILocationComponent
        Implements INetworkEdge
        Implements IFootprintTrace

#Region "Public Properties & Fields"

        <Column("Trace.Regulator")> Public Overrides Property RegulatorTrace As String Implements IFootprintTrace.RegulatorTrace
        <Column("Trace.Site")> Public Overrides Property MotifTrace As String Implements IMotifTrace.MotifTrace

#End Region

        ''' <summary>
        ''' 建议使用这个方法来获取得到副本
        ''' </summary>
        ''' <returns></returns>
        Public Function Copy() As PredictedRegulationFootprint
            Return DirectCast(Me.MemberwiseClone, PredictedRegulationFootprint)
        End Function

        ''' <summary>
        ''' 复制出一个新的对象，这个的速度要比<see cref="Copy"/>函数要高，但是在调整数据结构的时候可能会出现BUG
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As PredictedRegulationFootprint
            Return New PredictedRegulationFootprint With {
                .Distance = Me.Distance,
                .DoorId = Me.DoorId,
                .Ends = Me.Ends,
                .InitX = Me.InitX,'                .LociDescrib = Me.LociDescrib,
                .MotifId = Me.MotifId,
                .ORF = Me.ORF,
                .ORFDirection = Me.ORFDirection,
                .Pcc = Me.Pcc,
                .Category = Me.Category,
                .[Class] = Me.Class,
                .Regulator = Me.Regulator,
                .MotifTrace = Me.MotifTrace,
                .RegulatorTrace = Me.RegulatorTrace,'                .RNAGene = Me.RNAGene,
                .Sequence = Me.Sequence,
                .Signature = Me.Signature,
                .Starts = Me.Starts,
                .Strand = Me.Strand,
                .StructGenes = Me.StructGenes,
                .Type = Type,
                .MotifFamily = Me.MotifFamily,
                .sPcc = Me.sPcc,
                .WGCNA = Me.WGCNA,
                .tag = Me.tag
            }
        End Function

        ''' <summary>
        ''' 将操作子的调控转换为一对一的调控描述
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function OperonRegulationCopies() As PredictedRegulationFootprint()
            If Me.StructGenes.IsNullOrEmpty Then
                Return {Me}
            End If

            Dim setValue = New SetValue(Of PredictedRegulationFootprint) <=
                NameOf(PredictedRegulationFootprint.ORF)
            Dim LQuery As PredictedRegulationFootprint() =
                LinqAPI.Exec(Of PredictedRegulationFootprint) <=
                    From gId As String
                    In Me.StructGenes
                    Let CloneItem As PredictedRegulationFootprint = Clone()
                    Let AssignedORF = setValue(CloneItem, gId)
                    Select AssignedORF
            Return LQuery
        End Function

        ''' <summary>
        ''' 当具有重叠数据的时候，会返回多个对象拷贝
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="GenomeSequence"></param>
        ''' <param name="Ptt"></param>
        ''' <param name="IgnoreDirection"></param>
        ''' <param name="ATGDistance"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Shared Function __createRegulationObject(data As MEMEOutput,
                                                        GenomeSequence As IPolymerSequenceModel,
                                                        Ptt As PTTDbLoader,
                                                        IgnoreDirection As Boolean,
                                                        Optional ATGDistance As Integer = 500) As PredictedRegulationFootprint()

            Dim nnnnnnnnnnnnn As Integer() = {data.Start, data.Ends}
            nnnnnnnnnnnnn = {nnnnnnnnnnnnn.Max + 5, nnnnnnnnnnnnn.Min - 5} '计算的位点可能会有偏差，所以在这里将位点的选取拓宽一些，向左右两侧各延伸5bp，以保证整个Motif的序列都可以被获取
            Dim Regulation As PredictedRegulationFootprint = New PredictedRegulationFootprint With {
                .Starts = nnnnnnnnnnnnn.Min,
                .Strand = data.Strand,
                .Ends = nnnnnnnnnnnnn.Max,
                .MotifId = data.MatchedMotif,
                .RegulatorTrace = data.MatchedRegulator,
                .Signature = data.RegularExpression
            }

            Regulation.Sequence = GenomeSequence.CutSequenceBylength(nnnnnnnnnnnnn.Min, Regulation.Length).SequenceData

            Dim RelatedGeneObjects As Relationship(Of GeneBrief)()
            '   Dim PositionRelative As SMRUCC.genomics.ComponentModel.Loci. SegmentRelationships

            If Not IgnoreDirection Then
                RelatedGeneObjects = Ptt.GetRelatedGenes(LociStart:=data.Start,
                                                         LociEnds:=data.Ends,
                                                         Strand:=GetStrand(data.Strand),
                                                         ATGDistance:=ATGDistance)
            Else
                RelatedGeneObjects = Ptt.GetRelatedGenes(LociStart:=data.Start, LociEnds:=data.Ends, ATGDistance:=ATGDistance)
            End If

            If RelatedGeneObjects.Count = 1 AndAlso RelatedGeneObjects.First.Gene.IsBlankSegment Then
                '  Regulation.LociDescrib = "There is nothing here"
                Return New PredictedRegulationFootprint() {Regulation}
            End If

            Return (From GeneSegment As Relationship(Of GeneBrief)
                    In RelatedGeneObjects
                    Let Copied As PredictedRegulationFootprint = DirectCast(Regulation.MemberwiseClone, PredictedRegulationFootprint)
                    Select __motifLociAssignPosDescrib(Of GeneBrief, PredictedRegulationFootprint)(
                        Copied, GeneSegment.Relation, GeneSegment.Gene, Regulation.Starts)).ToArray
        End Function
    End Class
End Namespace
