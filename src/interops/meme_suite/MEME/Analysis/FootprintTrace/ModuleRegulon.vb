#Region "Microsoft.VisualBasic::7df0ac3c60e2b28326b9ba31ba2e92ab, meme_suite\MEME\Analysis\FootprintTrace\ModuleRegulon.vb"

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

    '     Module ModuleRegulon
    ' 
    '         Function: __creates, FillSites, GetMapHash, GetRegulons, ToFootprints
    '                   (+2 Overloads) ToRegulons
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery

Namespace Analysis.FootprintTraceAPI

    <Package("Module.Regulons")>
    Public Module ModuleRegulon

        <ExportAPI("Maps.Hash")>
        <Extension> Public Function GetMapHash(maps As IEnumerable(Of bbhMappings)) As Dictionary(Of String, bbhMappings())
            Dim mapsHash As Dictionary(Of String, bbhMappings()) =
                (From x As bbhMappings
                     In maps
                 Select x
                 Group x By x.query_name.Split(":"c).Last Into Group) _
                      .ToDictionary(Function(x) x.Last,
                                    Function(x) x.Group.ToArray)
            Return mapsHash
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MEME">MEME_DIR</param>
        ''' <param name="motif"></param>
        ''' <param name="DOOR"></param>
        ''' <param name="mapsHash"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Hits.GetRegulons")>
        <Extension>
        Public Function GetRegulons(motif As MotifHits,
                                    MEME As AnnotationModel,
                                    DOOR As DOOR,
                                    mapsHash As Dictionary(Of String, bbhMappings()),
                                    WORK As String,
                                    isTrue As IsTrue,
                                    Optional modX As String = "") As ModuleMotif()
            Dim Families = (From x As PredictedRegulationFootprint
                            In motif.ToFootprints(DOOR, mapsHash)
                            Select x
                            Group x By x.MotifFamily Into Group)
            Dim LQuery = (From x In Families.AsParallel
                          Let trues = (From f As PredictedRegulationFootprint
                                       In x.Group
                                       Where isTrue(f.ORF, f.Regulator)
                                       Select f).ToArray
                          Where trues.Length > 0
                          Select trues.__creates(MEME, modX, WORK)).ToArray
            Return LQuery
        End Function

        <Extension>
        Private Function __creates(footprints As PredictedRegulationFootprint(),
                                   model As AnnotationModel,
                                   modX As String,
                                   WORK As String) As ModuleMotif
            Dim Family As String = footprints.First.MotifFamily
            Dim regulon As New ModuleMotif With {
                .family = Family,
                .evalue = model.Evalue,
                .module = modX,
                .motif = model.Motif,
                .source = footprints.Select(Function(x) x.ORF).Distinct.ToArray,
                .regulators = footprints.Select(Function(x) x.Regulator).Distinct.ToArray
            }

            Dim DbModels As AnnotationModel() = GetFamilyMotifs(Family)
            Dim compares = (From subject In DbModels
                            Let c = TOMQuery.SWTom.Compare(model, subject)
                            Where Not c Is Nothing AndAlso
                                Not c.HSP.IsNullOrEmpty
                            Select c
                            Order By c.Similarity Descending).ToArray         ' 比对，生成比对矩阵图片

            If compares.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim out As String = WORK & $"/{modX}-{Family}-{SecurityString.GetMd5Hash(regulon.GetJson)}.Xml"
            Dim best As Output = compares.First
            Call best.SaveAsXml(out)

            Dim bestHSP = (From x As SW_HSP
                           In best.HSP
                           Select x
                           Order By x.SubjectLength Descending).First

            Call bestHSP.Visual.CorpBlank.SaveAs(out.TrimSuffix & ".png", ImageFormats.Png)

            regulon.tom = out.BaseName

            Return regulon
        End Function

        <ExportAPI("Hits.ToFootprints")>
        Public Function ToRegulons(MEME As Dictionary(Of AnnotationModel),
                                   motif As FootprintTrace,
                                   DOOR As DOOR,
                                   mapsHash As Dictionary(Of String, bbhMappings()),
                                   isTrue As IsTrue,
                                   WORK As String) As ModuleMotif()
            Dim result As New List(Of ModuleMotif)

            For Each modX As MatchResult In motif.Footprints
                If modX.Matches.IsNullOrEmpty Then
                    Continue For
                End If

                For Each dev As MotifHits In modX.Matches
                    Dim model = MEME(dev.Trace.Replace("::", "."))
                    Dim reuslt = dev.GetRegulons(model, DOOR, mapsHash, WORK, isTrue, modX.MEME)
                    Call result.AddRange(reuslt)
                Next
            Next

            Return result.ToArray
        End Function

        <ExportAPI("Hits.ToFootprints")>
        Public Function ToRegulons(MEME As Dictionary(Of AnnotationModel),
                                   motif As FootprintTrace,
                                   DOOR As DOOR,
                                   mapsHash As IEnumerable(Of bbhMappings),
                                   isTrue As IsTrue,
                                   WORK As String) As ModuleMotif()
            Return ToRegulons(MEME, motif, DOOR, mapsHash.GetMapHash, isTrue, WORK)
        End Function

        <ExportAPI("Hits.ToFootprints")>
        <Extension>
        Public Function ToFootprints(motif As MotifHits,
                                     DOOR As DOOR,
                                     mapsHash As Dictionary(Of String, bbhMappings())) As IEnumerable(Of PredictedRegulationFootprint)
            Dim LQuery = (From site As PredictedRegulationFootprint
                          In motif.GetFootprints
                          Select site.FillSites(DOOR, mapsHash)).ToVector
            Return LQuery
        End Function

        <ExportAPI("Fill.Sites")>
        <Extension>
        Public Function FillSites(site As PredictedRegulationFootprint,
                                  DOOR As DOOR,
                                  mapsHash As Dictionary(Of String, bbhMappings())) As PredictedRegulationFootprint()

            Dim g As OperonGene = DOOR.GetGene(site.ORF)
            site.DoorId = g.OperonID
            site.ORFDirection = g.Location.Strand.GetBriefCode
            site.Strand = site.ORFDirection
            site.InitX = If(DOOR.IsOprPromoter(site.ORF), "1", "0")

            If Not mapsHash.ContainsKey(site.RegulatorTrace) Then
                Return Nothing
            End If

            Dim setValue = New SetValue(Of PredictedRegulationFootprint)() _
                .GetSet(NameOf(PredictedRegulationFootprint.Regulator))
            Dim LQuery As PredictedRegulationFootprint() =
                LinqAPI.Exec(Of PredictedRegulationFootprint) <=
                    From map As bbhMappings
                    In mapsHash(site.RegulatorTrace)
                    Let copy As PredictedRegulationFootprint = site.Copy
                    Select setValue(copy, map.hit_name)

            Return LQuery
        End Function
    End Module
End Namespace
