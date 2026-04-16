#Region "Microsoft.VisualBasic::11d73d79e9d329e6b78d71d3f17e52f9, RNA-Seq\Rockhopper\TSSsValidation.vb"

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

    ' Module TSSsValidation
    ' 
    '     Function: AssignValue, CreateView, RockhopperGeneStructures, SaveView
    '     Class RockhopperGeneStructure
    ' 
    '         Properties: ATG, ATG_Dist, Category, Condition, DoorID
    '                     Is_sRNA, IsDoorPromoter, isPredictedOperonPromoter, IsPredictedRNA, IsRNA
    '                     Leaderless, Loci, Minus35BoxLoci, MTU_Length, predictedOperon
    '                     RelatedGeneID, RelatedGeneLoci, RelatedGeneStrand, Sigma70ATGDistance, Sigma70TSSsDist
    '                     Strand, Synonym, TGA, TSSLoci, TTSs
    ' 
    '         Function: Clone
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports LANS.SystemsBiology.AnalysisTools
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.Java.IO.Properties.Reflector

''' <summary>
''' Validation of predicted sigma 70 promoters using TSSs
''' </summary>
''' 
<[PackageNamespace]("Sigma70")>
Module TSSsValidation

    Public Class RockhopperGeneStructure : Inherits MEME_DIP

        Public Property Loci As String
        Public Property Synonym As String
        Public Property Category As String
        Public Property ATG As Integer
        Public Property Strand As String
        Public Property TSSLoci As Integer
        Public Property Minus35BoxLoci As Integer
        Public Property Is_sRNA As Boolean
        Public Property IsPredictedRNA As Boolean
        Public Property DoorID As Integer
        Public Property RelatedGeneID As String
        Public Property MTU_Length As Integer
        Public Property TGA As Integer
        Public Property ATG_Dist As Integer
        Public Property Leaderless As Boolean
        Public Property IsRNA As Boolean
        Public Property predictedOperon As Integer
        Public Property IsDoorPromoter As Boolean
        Public Property RelatedGeneStrand As String
        Public Property TTSs As Integer
        Public Property isPredictedOperonPromoter As Boolean
        Public Property Condition As String
        Public Property RelatedGeneLoci As String
        Public Property Sigma70ATGDistance As Integer
        Public Property Sigma70TSSsDist As Integer

        Public Function Clone() As RockhopperGeneStructure
            Return New RockhopperGeneStructure With {
                .ATG = ATG,
                .ATG_Dist = ATG_Dist,
                .Category = Category,
                .Condition = Condition,
                .DIP = DIP,
                .DoorID = DoorID,
                .EValue = EValue,
                .IsDoorPromoter = IsDoorPromoter,
                .isPredictedOperonPromoter = isPredictedOperonPromoter,
                .IsPredictedRNA = IsPredictedRNA,
                .IsRNA = IsRNA,
                .Is_sRNA = Is_sRNA,
                .Leaderless = Leaderless,
                .Loci = Loci,
                .Minus35BoxLoci = Minus35BoxLoci,
                .MotifGuid = MotifGuid,
                .MTU_Length = MTU_Length,
                .predictedOperon = predictedOperon,
                .PValue = PValue,
                .RelatedGeneID = RelatedGeneID,
                .RelatedGeneLoci = RelatedGeneLoci,
                .RelatedGeneStrand = RelatedGeneStrand,
                .RightEndDownStream = RightEndDownStream,
                .Sequence = Sequence,
                .Sigma70ATGDistance = Sigma70ATGDistance,
                .Sigma70TSSsDist = Sigma70TSSsDist,
                .Signature = Signature,
                .Site = Site,
                .Start = Start,
                .Strand = Strand,
                .Synonym = Synonym,
                .TGA = TGA,
                .TSSLoci = TSSLoci,
                .TTSs = TTSs}
        End Function

    End Class

    <ExportAPI("Gene.Struct_View")>
    Public Function RockhopperGeneStructures(vfCsv As String, rsCsv As String) As RockhopperGeneStructure()
        Dim vfCache = (From vf In vfCsv.LoadCsv(Of NBCR.Extensions.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.VF_DIP)(False).AsParallel Select vf, vf.MotifLocation, vf.ORF).ToArray
        Dim VirtualFootprints = (From vf In vfCache.AsParallel
                                 Where vf.MotifLocation = SegmentRelationships.UpStream AndAlso Not String.IsNullOrEmpty(vf.ORF)
                                 Select vf
                                 Group vf By vf.ORF Into Group).ToArray.ToDictionary(Function(obj) obj.ORF, elementSelector:=Function(obj) (From item In obj.Group Select item.vf).ToArray)
        Dim rgStruct = rsCsv.LoadCsv(Of RockhopperGeneStructure)(False)
        Dim ChunkBuffer As New List(Of RockhopperGeneStructure)

        For Each site In rgStruct
            Dim ORF = VirtualFootprints.TryGetValue(site.Synonym)

#If DEBUG Then
            If Not ORF.IsNullOrEmpty Then
                Call Console.WriteLine(site.Synonym)
            End If
#End If

            Dim view = CreateView(site, ORF)
            Call ChunkBuffer.AddRange(view)
        Next

        Dim result = ChunkBuffer.ToArray
        Return result

        ' 下面的LINQ有BUG？？？？ 新复制的属性值前部没有了

        Dim LQuery = (From site In rgStruct'.AsParallel
                      Let ORF = VirtualFootprints.TryGetValue(site.Synonym)
                      Let view = CreateView(site, ORF)
                      Select view).ToArray.MatrixToVector
        Return LQuery
    End Function

    <ExportAPI("Write.Csv.View")>
    Public Function SaveView(data As System.Collections.Generic.IEnumerable(Of RockhopperGeneStructure), SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function

    Private Function CreateView(entry As RockhopperGeneStructure, ORF As NBCR.Extensions.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.VF_DIP()) As RockhopperGeneStructure()
        If ORF.IsNullOrEmpty Then
            Return {entry}
        End If

        Dim LQuery = (From site In ORF Select AssignValue(entry.Clone, site)).ToArray
        Return LQuery
    End Function

    Private Function AssignValue(Clone As RockhopperGeneStructure, site As NBCR.Extensions.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.VF_DIP) As RockhopperGeneStructure
        Clone.Sigma70ATGDistance = site.Distance
        Clone.Sigma70TSSsDist = site.Distance - Clone.ATG_Dist
        Clone.MotifGuid = site.MotifId
        Clone.Signature = site.Signature

        Return Clone
    End Function

End Module
