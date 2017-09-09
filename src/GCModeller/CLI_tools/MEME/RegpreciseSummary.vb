#Region "Microsoft.VisualBasic::fa056bfcdabbd3d49b0181ca39f24161, ..\CLI_tools\MEME\RegpreciseSummary.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel

Namespace Analysis

    <Package("MEME.app.Genome_Footprints", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module RegpreciseSummary

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="csv"></param>
        ''' <returns></returns>
        <ExportAPI("Load.BBHH", Info:="Load the regprecise regulators annotation mappings from local csv excel data.")>
        Public Function LoadRegpreciseBBH(csv As String) _
           As <FunctionReturns("The key of the function returns dictionary value is the vimssId of the regprecise regulator in the microbial online database.
           and The value of the key value paired object is the collection of the bbh protein result in your annotated genome.")> Dictionary(Of Integer, bbhMappings())

            Dim bbh As List(Of bbhMappings) = csv.LoadCsv(Of bbhMappings)()
            Dim bbhGroup = From obj As bbhMappings
                       In bbh
                           Select obj
                           Group obj By obj.vimssId Into Group
            Dim dict = bbhGroup.ToDictionary(
                Function(group) group.vimssId,
                Function(group) group.Group.ToArray)
            Return dict
        End Function

        <ExportAPI("Load.MastSites")>
        Public Function LoadMEME(csv As String) As IEnumerable(Of MastSites)
            Return csv.LoadCsv(Of MastSites)
        End Function

        ''' <summary>
        ''' 以site位点为基准：从site找调控因子
        ''' </summary>
        ''' <param name="regulators"></param>
        ''' <param name="sites"></param>
        ''' <returns></returns>
        <ExportAPI("Regulations.Predicts")>
        Public Function GenerateRegulations(regulators As Dictionary(Of Integer, bbhMappings()),
                                            sites As IEnumerable(Of MastSites),
                                            Optional sp As String = "",
                                            Optional cutoff As Double = 0.6) As PredictedRegulationFootprint()
            Return regulators.GenerateRegulations(sites, Correlation2.LoadAuto(sp), cutoff)
        End Function

        ''' <summary>
        ''' 以site位点为基准：从site找调控因子
        ''' </summary>
        ''' <param name="regulators"></param>
        ''' <param name="sites"></param>
        ''' <returns></returns>
        <ExportAPI("Regulations.Predicts")>
        <Extension>
        Public Function GenerateRegulations(regulators As Dictionary(Of Integer, bbhMappings()),
                                            sites As IEnumerable(Of MastSites),
                                            correlations As Correlation2,
                                            Optional cutoff As Double = 0.6) As PredictedRegulationFootprint()
            Dim footprints As PredictedRegulationFootprint()
            Dim regDB As Regulations = GCModeller.FileSystem.Regulations.LoadXml(Of Regulations)

            If cutoff = 0R Then
                Call $"Program will print all regulation data...".__DEBUG_ECHO
            End If

#If DEBUG Then
            footprints = sites.ToArray(Function(site) __createSites(site, regulators, correlations, regDB)).ToVector
#Else
            footprints = sites.ToArray(Function(site) __createSites(site, regulators, correlations, regDB), Parallel:=True).ToVector
#End If
            Return footprints
        End Function

        ''' <summary>
        ''' 以site位点为基准：从site找调控因子
        ''' </summary>
        ''' <param name="regulators"></param>
        ''' <param name="sites"></param>
        ''' <returns></returns>
        <ExportAPI("Regulations.Predicts")>
        Public Function GenerateRegulations(regulators As Dictionary(Of Integer, bbhMappings()),
                                            sites As IEnumerable(Of MotifSite),
                                            Optional sp As String = "",
                                            Optional cutoff As Double = 0.6) As PredictedRegulationFootprint()
            Dim regDB = GCModeller.FileSystem.Regulations.LoadXml(Of Regulations)
            Dim KEGG = BriteHEntry.Module.LoadFromResource.ToDictionary(Function([mod]) [mod].Entry.Key)
            Dim correlations = Correlation2.CreateFromName(sp)
            Dim footprints As PredictedRegulationFootprint()

            If cutoff = 0R Then
                Call $"Program will print all regulation data...".__DEBUG_ECHO
            End If

#If DEBUG Then
            footprints = sites.ToArray(Function(site) __createSites(site, regulators, regDB, KEGG, correlations, cutoff)).ToVector
#Else
            footprints = sites.ToArray(Function(site) __createSites(site, regulators, regDB, KEGG, correlations, cutoff), Parallel:=True).ToVector
#End If
            Return footprints
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="site">没有调控因子的记录，需要从数据库之中读取数据</param>
        ''' <param name="regulators">需要进行注释的基因组和Regprecise数据库Mapping所得到的结果</param>
        ''' <returns></returns>
        Private Function __createSites(site As MotifSite,
                                       regulators As Dictionary(Of Integer, bbhMappings()),
                                       regDB As Regulations,
                                       KEGG As Dictionary(Of String, BriteHEntry.Module),
                                       correlations As ICorrelations,
                                       cutoff As Double) As List(Of PredictedRegulationFootprint)
            Dim siteUid As String = site.uid.Split("|"c).ElementAtOrDefault(Scan0) ' 得到Motif编号
            Dim regVimssId As Integer() = regDB.GetRegulations(siteUid).ToArray(Function(reg) reg.Regulator)  ' 该Motif在基因组内可能被多个调控因子调控
            Dim footprints = __regulationMappings(site, regVimssId, regulators, correlations, site.Signature, regDB, site.Tag, site.uid)
            Dim TagData As String() = (From s As String In site.Tag.Replace("\", "/").Split("/"c) Where Not String.IsNullOrEmpty(s) Select s).ToArray
            Dim modId As String = TagData.ElementAtOrDefault(1)

            If Not String.IsNullOrEmpty(modId) Then
                modId = modId.Split("_"c).Last
            End If

            Dim modKEGG As BriteHEntry.Module = Nothing

            If KEGG.ContainsKey(modId) Then
                modKEGG = KEGG(modId)
            End If

            If Not modKEGG Is Nothing Then
                For Each vf As PredictedRegulationFootprint In footprints
                    vf.Class = modKEGG.Category
                    vf.Category = modKEGG.SubCategory
                    vf.Type = modKEGG.Class
                Next
            End If

            Return footprints
        End Function

        Private Function __regulationMappings(site As ISiteReader,
                                              regVimssId As Integer(),
                                              regulators As Dictionary(Of Integer, bbhMappings()),
                                              correlations As ICorrelations,
                                              Signature As String,
                                              regDb As Regulations,
                                              siteTag As String,
                                              siteUid As String) _
                                              As List(Of PredictedRegulationFootprint)
            Dim footprints As New List(Of PredictedRegulationFootprint)

            For Each regulator As Integer In regVimssId  ' 得到了数据库之中的调控因子
                If regulators.ContainsKey(regulator) Then  'bbh没有匹配上，但是meme分析的结果里面有这个
                    Dim mapped_regulatorList = regulators(regulator)

                    For Each mapping In mapped_regulatorList

                        Dim mappedRegulator As String = mapping.hit_name    ' 注释的基因组内的调控因子的Mapping
                        Dim regpreciseRegulator As String = mapping.query_name  ' 数据库之中的调控因子
                        Dim Family As String = mapping.Family
                        Dim pcc, spcc, WGCNA As Double

                        If Not correlations Is Nothing Then
                            pcc = correlations.GetPcc(site.ORF, mappedRegulator)
                            spcc = correlations.GetSPcc(site.ORF, mappedRegulator)
                            WGCNA = correlations.GetWGCNAWeight(site.ORF, mappedRegulator)
                        Else
                            pcc = -100
                            spcc = -100
                            WGCNA = -100
                        End If

                        Dim vf As New PredictedRegulationFootprint With {
                            .Distance = -site.Distance,
                            .ORF = site.ORF,
                            .ORFDirection = site.Strand,
                            .Regulator = mappedRegulator,
                            .Pcc = pcc,
                            .sPcc = spcc,
                            .WGCNA = WGCNA,
                            .Sequence = site.SequenceData,
                            .Strand = site.Strand,
                            .Starts = site.gStart,'                    .Regprecise = regpreciseRecords,
                            .Ends = site.gStop,
                            .MotifId = siteTag,
                            .MotifFamily = Family,
                            .Signature = Signature,
                            .RegulatorTrace = regpreciseRegulator,
                            .MotifTrace = siteUid
                        }
                        Call footprints.Add(vf)
                    Next
                Else
                    Dim regpreciseRegulator As JSONLDM.regulator = regDb.GetRegulator(regulator)

                    Dim vf As New PredictedRegulationFootprint With {
                         .Distance = -site.Distance,
                         .ORF = site.ORF,
                         .ORFDirection = site.Strand,
                         .Regulator = "",
                         .Pcc = -100,
                         .Sequence = site.SequenceData,
                         .Strand = site.Strand,
                         .Starts = site.gStart,'                .RegulatorTrace = regpreciseRecords,
                         .Ends = site.gStop,
                         .MotifId = siteTag,
                         .MotifFamily = site.Family,
                         .Signature = Signature,
                         .MotifTrace = siteUid,
                         .RegulatorTrace = If(regpreciseRegulator Is Nothing, "", regpreciseRegulator.locusTag),
                         .WGCNA = -100,
                         .sPcc = -100
                     }
                    Call footprints.Add(vf)
                End If
            Next

            Return footprints
        End Function

        ''' <summary>
        ''' 直接将MastSite里面的Trace作为调控因子
        ''' </summary>
        ''' <param name="site"></param>
        ''' <param name="correlations"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Mast.Regulations")>
        Public Function SiteToRegulation(site As IEnumerable(Of MastSites), correlations As String, DOOR As String) As PredictedRegulationFootprint()
            Dim coorDb As ICorrelations = Correlation2.LoadAuto(correlations)
            Dim OprDOOR As DOOR = DOOR_API.Load(DOOR)
            Dim out As PredictedRegulationFootprint() = (From x As MastSites In site.AsParallel
                                                         Where Not x Is Nothing
                                                         Select SiteToRegulation(x, coorDb, OprDOOR)).ToArray
            Return out
        End Function

        ''' <summary>
        ''' 直接将MastSite里面的Trace作为调控因子
        ''' </summary>
        ''' <param name="site"></param>
        ''' <param name="correlations"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Mast.Regulation")>
        Public Function SiteToRegulation(site As MastSites, correlations As ICorrelations, DOOR As DOOR) As PredictedRegulationFootprint
            Dim TF As String = site.Trace.Split(":"c).First
            Dim pcc As Double = correlations.GetPcc(TF, site.Gene)
            Dim spcc As Double = correlations.GetSPcc(TF, site.Gene)
            Dim WGCNA As Double = correlations.GetWGCNAWeight(TF, site.Gene)
            Dim oprLDM = DOOR.GetGene(site.Gene)

            Dim vf As New PredictedRegulationFootprint With {
                .Category = "",
                .Class = "",
                .Distance = -site.ATGDist,
                .DoorId = If(oprLDM Is Nothing, "", oprLDM.OperonID),
                .Ends = site.gStop,   ' .IsOperonPromoter = DOOR.DoorOperonView(DOOR.GetGene(site.Gene).OperonID).InitialX.Synonym.Equals(site.Gene).Switch("+"c, "-"c),
                .MotifFamily = site.Family,
                .MotifId = site.Trace,
                .MotifTrace = site.Trace,
                .ORF = site.Gene,
                .ORFDirection = site.StrandRaw,
                .Pcc = pcc,
                .Regulator = TF,
                .RegulatorTrace = site.Trace,
                .Sequence = site.SequenceData,
                .Signature = site.SequenceData,
                .sPcc = spcc,
                .Starts = site.Start,
                .Strand = site.Strand.GetBriefCode,
                .WGCNA = WGCNA
            }
            Return vf
        End Function

        Private Function __createSites(site As MastSites,
                                       regulators As Dictionary(Of Integer, bbhMappings()),
                                       correlations As ICorrelations,
                                       regDb As Regulations) As List(Of PredictedRegulationFootprint)
            '#Region "Mappings Regulation"
            '            Dim regulatorList As New List(Of BBHH)
            '            For Each regulator As Integer In site.Regulators
            '                If regulators.ContainsKey(regulator) Then  'bbh没有匹配上，但是meme分析的结果里面有这个
            '                    Call regulatorList.AddRange(regulators(regulator))
            '                End If
            '            Next
            '            Dim mappedRegulators As String() = __getMapped(regulatorList)
            '            Dim regpreciseRecords As String() = __getregDataSet(regulatorList)
            '            Dim Families As Dictionary(Of String, String()) = (From regulator As BBHH
            '                                                           In regulatorList
            '                                                               Select regulator.hit_name,
            '                                                               regulator.Family
            '                                                               Group By hit_name Into Group) _
            '                                                                .ToDictionary(Function(regulator) regulator.hit_name,
            '                                                                              elementSelector:=Function(familiesEntry) _
            '                                                                                                   familiesEntry.Group.ToArray.ToArray(Function(family) family.Family).Distinct.ToArray)

            '            Dim filters As Dictionary(Of String, Double) = __correlationFilter(site.Gene, mappedRegulators, correlations, cutoff)
            Dim footprints = __regulationMappings(site, site.Regulators, regulators, correlations, "", regDb, site.Trace, site.Trace)
            '            Dim trace As String = If(String.IsNullOrEmpty(site.Trace), "", "@" & site.Trace)
            '#End Region

            '            If filters.IsNullOrEmpty Then
            '                Dim vf As New SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint With {
            '            .Distance = site.ATGDist,
            '            .ORF = site.Gene,
            '            .ORFDirection = site.StrandRaw,
            '            .Regulator = "",
            '            .Sequence = site.SequenceData,
            '            .Strand = site.StrandRaw,
            '            .Starts = site.Start,'            .Regprecise = regpreciseRecords,
            '            .Ends = site.Start + site.Length,
            '            .MotifId = "*" & site.Family & trace,
            '            .Pcc = -100
            '        }
            '                footprints = {vf}
            '            Else
            '                footprints = filters.ToArray(
            '                Function(regulates) New SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint With {
            '            .Distance = site.ATGDist,
            '            .ORF = site.Gene,
            '            .ORFDirection = site.StrandRaw,
            '            .Regulator = regulates.Key,
            '            .Sequence = site.SequenceData,
            '            .Strand = site.StrandRaw,
            '            .Starts = site.Start,'            .Regprecise = regpreciseRecords,
            '            .Ends = site.Start + site.Length,
            '            .MotifId = String.Join(";", Families(regulates.Key)) & trace,
            '            .Pcc = regulates.Value
            '        })
            '            End If

            Return footprints
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ORF"></param>
        ''' <param name="regulators"></param>
        ''' <param name="correlations"></param>
        ''' <param name="cutOff">一般是0.6</param>
        ''' <returns></returns>
        Private Function __correlationFilter(ORF As String,
                                             regulators As String(),
                                             correlations As ICorrelations,
                                             cutOff As Double) As Dictionary(Of String, Double)
            If correlations Is Nothing Then
                Return regulators.ToDictionary(Function(reg) reg,
                                               Function(null) -100.0R)
            End If

            Dim LQuery = (From regulator As String
                          In regulators
                          Let pcc As Double = correlations.GetPcc(ORF, regulator)
                          Let spcc As Double = correlations.GetSPcc(ORF, regulator)
                          Where Math.Abs(pcc) >= cutOff OrElse Math.Abs(spcc) >= cutOff
                          Let score As Double = If(Math.Abs(pcc) >= Math.Abs(spcc), pcc, spcc)
                          Select regulator, score) _
                                .ToDictionary(Function(regulate) regulate.regulator,
                                              Function(regulate) regulate.score)
            Return LQuery
        End Function

        ''' <summary>
        ''' 得到基因组之中被mapped到的调控因子
        ''' </summary>
        ''' <returns></returns>
        Private Function __getMapped(lstRegulator As IEnumerable(Of bbhMappings)) As String()
            Return (From regulator As bbhMappings
                    In lstRegulator
                    Select regulator.hit_name
                    Distinct
                    Order By hit_name Ascending).ToArray  ' 得到基因组之中被mapped到的调控因子
        End Function

        Private Function __getregDataSet(lstRegulator As IEnumerable(Of bbhMappings)) As String()
            Return (From regulator As bbhMappings
                    In lstRegulator
                    Select regulator.query_name
                    Distinct
                    Order By query_name Ascending).ToArray
        End Function
    End Module
End Namespace
