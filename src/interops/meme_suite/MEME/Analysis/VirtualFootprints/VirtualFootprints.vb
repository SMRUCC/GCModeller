#Region "Microsoft.VisualBasic::d833666711435ece86e4de8b47b8ce85, meme_suite\MEME\Analysis\VirtualFootprints\VirtualFootprints.vb"

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

    '     Module VirtualFootprintAPI
    ' 
    '         Function: __createMotifSiteInfo, __motifLociAssignPosDescrib, CreateMotifSiteInfo, FamilyFromId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MAST.HTML
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.HTML
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.SequenceModel

Namespace Analysis.GenomeMotifFootPrints

    ''' <summary>
    ''' Site information data which only contains the motif information.(只有Motif位点信息的对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Module VirtualFootprintAPI

        Public Function FamilyFromId(x As VirtualFootprints) As String
            Return x.MotifId.Split("."c).First
        End Function

        Friend Function __createMotifSiteInfo(Of T As IGeneBrief)(
                                 data As MEMEOutput,
                                 GenomeSequence As IPolymerSequenceModel,
                                 GeneBriefInformation As IEnumerable(Of T),
                                 Optional ATGDistance As Integer = 500) As VirtualFootprints()

            Dim nnnnnnnnnnnnn As Integer() = {data.Start, data.Ends}
            nnnnnnnnnnnnn = {nnnnnnnnnnnnn.Max + 5, nnnnnnnnnnnnn.Min - 5} '计算的位点可能会有偏差，所以在这里将位点的选取拓宽一些，向左右两侧各延伸5bp，以保证整个Motif的序列都可以被获取
            Dim Regulation As VirtualFootprints = New VirtualFootprints With {
                .Starts = nnnnnnnnnnnnn.Min,
                .Strand = data.Strand,
                .Ends = nnnnnnnnnnnnn.Max,
                .MotifId = data.MatchedMotif,
                .Signature = data.RegularExpression
            }

            Regulation.Sequence = GenomeSequence.CutSequenceBylength(Regulation.Starts, Regulation.Length).SequenceData

            Dim ObjectStrand = GetStrand(data.Strand)
            Dim DataSource = (From GeneObject As T In GeneBriefInformation
                              Where GeneObject.Location.Strand = ObjectStrand
                              Select GeneObject).ToArray

            'Dim RelatedGeneObjects = ContextModel.GetRelatedGenes(Of T)(
            '    DataSource,
            '    LociStart:=data.Start,
            '    LociEnds:=data.Ends,
            '    ATGDistance:=ATGDistance)

            'If RelatedGeneObjects.Length = 1 AndAlso
            '    ContextModel.IsBlankSegment(RelatedGeneObjects.First.Gene) Then
            '    Return New VirtualFootprints() {Regulation}
            'End If

            'Return (From GeneSegment As ContextModel.Relationship(Of T)
            '        In RelatedGeneObjects
            '        Let Copied As VirtualFootprints = DirectCast(Regulation.MemberwiseClone, VirtualFootprints)
            '        Select __motifLociAssignPosDescrib(Of T, VirtualFootprints)(
            '            Copied, GeneSegment.Relation, GeneSegment.Gene, Regulation.Starts)).ToArray
            Throw New NotImplementedException
        End Function

        Public Function CreateMotifSiteInfo(Of T As IGeneBrief)(
                        data As MEME.LDM.Motif,
                        mast As MatchedSite,
                        GenomeSequence As IPolymerSequenceModel,
                        GeneBriefInformation As IEnumerable(Of T),
                        Optional ATGDistance As Integer = 500) As VirtualFootprints()

            Dim nnnnnnnnnnnnn As Integer() = {mast.Starts, mast.Ends}
            nnnnnnnnnnnnn = {nnnnnnnnnnnnn.Max + 5, nnnnnnnnnnnnn.Min - 5} '计算的位点可能会有偏差，所以在这里将位点的选取拓宽一些，向左右两侧各延伸5bp，以保证整个Motif的序列都可以被获取
            Dim Regulation As VirtualFootprints = New VirtualFootprints With {
                .Starts = nnnnnnnnnnnnn.Min,
                .Strand = mast.Strand,
                .Ends = nnnnnnnnnnnnn.Max,
                .MotifId = data.Id,
                .Signature = data.Signature
            }

            Regulation.Sequence = GenomeSequence.CutSequenceBylength(Regulation.Starts, Regulation.Length).SequenceData  'starts 永远是最小的

            Dim ObjectStrand = GetStrand(mast.Strand)
            Dim DataSource = (From GeneObject In GeneBriefInformation Where GeneObject.Location.Strand = ObjectStrand Select GeneObject).ToArray
            'Dim RelatedGeneObjects = ContextModel.GetRelatedGenes(Of T)(
            '    DataSource,
            '    LociStart:=Regulation.Starts,
            '    LociEnds:=Regulation.Ends,
            '    ATGDistance:=ATGDistance) ', RelationValue:=Relation)

            'If RelatedGeneObjects.Length = 1 AndAlso ContextModel.IsBlankSegment(RelatedGeneObjects.First.Gene) Then
            '    'Regulation.LociDescrib = "There is nothing here"
            '    Return New VirtualFootprints() {Regulation}
            'End If

            'Return (From GeneSegment
            '        In RelatedGeneObjects
            '        Let Copied As VirtualFootprints = DirectCast(Regulation.MemberwiseClone, VirtualFootprints)
            '        Select __motifLociAssignPosDescrib(Of T, VirtualFootprints)(Copied, GeneSegment.Relation, GeneSegment.Gene, Regulation.Starts)).ToArray
            Throw New NotImplementedException
        End Function

        Friend Function __motifLociAssignPosDescrib(Of T As IGeneBrief, TFootprint As VirtualFootprints)(
                                    PredictedRegulation As TFootprint,
                                    Relation As SegmentRelationships,
                                    GeneSegment As T,
                                    Start As Integer) As TFootprint

            If Relation = SegmentRelationships.UpStream OrElse Relation = SegmentRelationships.Inside Then
                PredictedRegulation.ORF = GeneSegment.Key
                PredictedRegulation.ORFDirection = If(GeneSegment.Location.Strand = Strands.Forward, "+", "-")
                PredictedRegulation.Distance = If(GeneSegment.Location.Strand = Strands.Forward,
                                                  Start - GeneSegment.Location.Left,
                                                  GeneSegment.Location.Right - Start)
                'If Relation = SegmentRelationships.UpStream Then
                '    PredictedRegulation.LociDescrib = $"In the promoter region with distance {PredictedRegulation.Distance}bp with ORF {GeneSegment.Identifier}"
                'Else
                '    PredictedRegulation.LociDescrib = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.LocationDescription(Relation, GeneSegment)
                'End If
            Else
                '  PredictedRegulation.LociDescrib = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.LocationDescription(Relation, GeneSegment)
            End If

            Return PredictedRegulation
        End Function
    End Module
End Namespace
