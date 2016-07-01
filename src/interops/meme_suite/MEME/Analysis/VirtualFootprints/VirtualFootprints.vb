Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME.LDM
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME.HTML
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MAST.HTML

Namespace Analysis.GenomeMotifFootPrints

    ''' <summary>
    ''' Site information data which only contains the motif information.(只有Motif位点信息的对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class VirtualFootprints : Inherits DocumentFormat.VirtualFootprints

        Public Shared Function FamilyFromId(x As DocumentFormat.VirtualFootprints) As String
            Return x.MotifId.Split("."c).First
        End Function

        Friend Shared Function __createMotifSiteInfo(Of T As IGeneBrief)(
                                 data As MEMEOutput,
                                 GenomeSequence As SegmentReader,
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

            Regulation.Sequence = GenomeSequence.TryParse(Regulation.Starts, SegLength:=Regulation.Length)

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

        Public Shared Function CreateMotifSiteInfo(Of T As IGeneBrief)(
                        data As MEME.LDM.Motif,
                        mast As MatchedSite,
                        GenomeSequence As SegmentReader,
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

            Regulation.Sequence = GenomeSequence.TryParse(Regulation.Starts, SegLength:=Regulation.Length) 'starts 永远是最小的

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

        Friend Shared Function __motifLociAssignPosDescrib(Of T As IGeneBrief, TFootprint As DocumentFormat.VirtualFootprints)(
                                    PredictedRegulation As TFootprint,
                                    Relation As SegmentRelationships,
                                    GeneSegment As T,
                                    Start As Integer) As TFootprint

            If Relation = SegmentRelationships.UpStream OrElse Relation = SegmentRelationships.Inside Then
                PredictedRegulation.ORF = GeneSegment.Identifier
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
    End Class
End Namespace