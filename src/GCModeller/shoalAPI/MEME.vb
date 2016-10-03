#Region "Microsoft.VisualBasic::31c9c2659561bf3d2d32e5b513698708, ..\GCModeller\shoalAPI\MEME.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq
Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Interactions.SwissTCS
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Workflows.PromoterParser
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank
Imports LANS.SystemsBiology.Assembly.DOOR

<PackageNamespace("Tools.MEME", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Public Module MEME

    <ExportAPI("TCS.Profiles.Load")>
    Public Function LoadProfiles(DIR As String) As Dictionary(Of String, String())
        Dim LoadFiles = (From file As String
                         In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchAllSubDirectories, "*.csv")
                         Let locusId As String = System.IO.Path.GetFileNameWithoutExtension(file)
                         Let getProfiles = file.LoadCsv(Of CrossTalks)
                         Let interactors As String() = getProfiles.ToArray(Function(crossTalk) abstract.GetConnectedNode(crossTalk, locusId))
                         Select locusId,
                             profile = interactors) _
                            .ToDictionary(Function(profile) profile.locusId,
                                          Function(profile) profile.profile)
        Return LoadFiles
    End Function

    <ExportAPI("Parser.Create")>
    Public Function CreateParser(Fasta As FastaToken, PTT As PTT) As GenePromoterParser
        Return New GenePromoterParser(Fasta, PTT)
    End Function

    <ExportAPI("Parser.Create")>
    Public Function CreateParser(Fasta As String, PTT As String) As GenePromoterParser
        Return CreateParser(FastaToken.LoadNucleotideData(Fasta), TabularFormat.PTT.Load(PTT))
    End Function

    <ExportAPI("MEME.Parser")>
    Public Sub MEMEParser(Parser As GenePromoterParser,
                          Profiles As Dictionary(Of String, String()),
                          DOOR As String,
                          Optional EXPORT As String = "./",
                          Optional method As GetLocusTags = GetLocusTags.UniDOOR)

        Dim opr As DOOR = DOOR_API.Load(DOOR)
        Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(opr, method)

        For Each Length As Integer In GenePromoterParser.PrefixLength
            For Each profile In Profiles
                Dim path As String = $"{EXPORT}/{Length}/{profile.Key}.fasta"
                Dim lstID As String() = profile.Value.Join(profile.Key) _
                    .ToArray(Function(id) GetDOORUni(id)).MatrixAsIterator.Distinct.ToArray
                Dim fasta = Parser.GetSequenceById(lstId:=lstID, Length:=Length)
                Call fasta.Save(path)
            Next
        Next
    End Sub

    <ExportAPI("Regprecise.Regulators.Families")>
    Public Sub RegulatorFamilies(bbh As String)
        Dim bbhResult = bbh.LoadCsv(Of LANS.SystemsBiology.DatabaseServices.Regprecise.bbhMappings)
        Dim grHits = (From hit As DatabaseServices.Regprecise.bbhMappings
                      In bbhResult
                      Select hit
                      Group hit By hit.hit_name Into Group).ToArray
        Dim FamilyHits As String() =
            (From s As String
             In bbhResult.ToArray(Function(hit) Common.Extensions.FamilyTokens(hit.Family)).ToArray.MatrixToList
             Let name As String = s.ToLower
             Select name, s
             Group name By name Into Group).ToArray _
                .ToArray(Function(name) (From s As String
                                         In name.Group.ToArray
                                         Where Char.IsUpper(s.First)
                                         Select s).FirstOrDefault(name.Group.First))

        ' 匹配上多个家族，和只匹配上一个家族的比例
        Dim proFamily = grHits.ToArray(
            Function(pro) New With {
                .locusId = pro.hit_name,
                .Family = (From s As String
                           In pro.Group.ToArray(Function(hit) Common.Extensions.FamilyTokens(hit.Family)).MatrixToList
                           Let Name As String = s.ToLower
                           Select Name, s
                           Group Name By Name Into Group).ToArray _
                                .ToArray(Function(name) (From s As String
                                                         In name.Group.ToArray
                                                         Where Char.IsUpper(s.First)
                                                         Select s).FirstOrDefault(name.Group.First))
                })
        Dim FCounts = (From Family As String
                       In FamilyHits
                       Let hits As String() = (From prot In proFamily
                                               Where Array.IndexOf(prot.Family, Family) > -1
                                               Select prot.locusId
                                               Order By locusId Ascending).ToArray
                       Select Family,
                           Numbers = hits.Length,
                           hits).ToArray

        Call FCounts.SaveTo(bbh.TrimFileExt & ".FamilyCounts.csv")

        Dim MCounts = (From n As Integer
                       In 100.Sequence.Skip(1)
                       Select Multiple = n,
                           Hits = (From prot
                                   In proFamily
                                   Where prot.Family.Length = n
                                   Select prot.locusId
                                   Order By locusId Ascending).ToArray).ToArray

        Call MCounts.SaveTo(bbh.TrimFileExt & ".MultipleFamily.csv")
    End Sub
End Module
