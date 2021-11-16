#Region "Microsoft.VisualBasic::b1a1f21105e9bc6e39ca6ec774317935, CLI_tools\MEME\Cli\Regulons.vb"

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

    ' Module CLI
    ' 
    '     Function: __getRegulators, __pairs, ExportRegulon, RegulonReconstruct, RegulonReconstructs
    '               RegulonReconstructs2, RegulonTest
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Partial Module CLI

    <ExportAPI("/Regulon.Test", Usage:="/Regulon.Test /in <meme.txt> /reg <genome.bbh.regulon.xml> /bbh <maps.bbh.Csv>")>
    <Group(CLIGrouping.RegulonTools)>
    Public Function RegulonTest(args As CommandLine) As Integer
        Dim inMEME As String = args("/in")
        Dim inRegulon As String = args("/reg")
        Dim inId As String = BaseName(inMEME)
        Dim queryList = AnnotationModel.LoadDocument(inMEME)
        Dim source = inRegulon.LoadXml(Of BacteriaRegulome)
        Dim sourceHash = (From x As Regulator
                          In source.regulome.regulators
                          Let uid As String = $"{x.LocusId}.{x.locus_tag.text.Replace(":", "_")}"
                          Select uid, x)
        Dim target = (From x In sourceHash Where String.Equals(x.uid, inId, StringComparison.OrdinalIgnoreCase) Select x).FirstOrDefault
        Dim bbh As String = args("/bbh")
        Dim maps = bbh.LoadCsv(Of bbhMappings)
        Dim params As New Parameters
        Dim qResult As MotifHit() = Nothing
        Call __SWQueryCommon(inMEME, params, True, TempFileSystem.GetAppSysTempFile, qResult)
        Dim RegDb As Regulations = GCModeller.FileSystem.GetRegulations.LoadXml(Of Regulations)
        Dim GetRegulators = (From x As MotifHit
                             In qResult
                             Select x,
                                 regT = __getRegulators(x.Subject, RegDb)).ToArray
        Dim MapsRegulator = (From x In GetRegulators
                             Let regs = (From sId As String
                                         In x.regT
                                         Let map As bbhMappings = bbhMappings.GetQueryMaps(maps, sId)
                                         Where Not map Is Nothing
                                         Select map).ToArray
                             Where Not regs.IsNullOrEmpty
                             Select regs, x.x).ToArray
        Dim TF As String = target.x.LocusId
        Dim LQuery = (From x In MapsRegulator
                      Let getMap = (From xx As bbhMappings In x.regs
                                    Where String.Equals(TF, xx.hit_name)
                                    Select xx).FirstOrDefault
                      Where Not getMap Is Nothing
                      Select x.x,
                          getMap).ToArray
        If LQuery.IsNullOrEmpty Then
            Call $"{inId} is not support!".__DEBUG_ECHO
        Else
            For Each x In LQuery
                Call $"{x.x.ToString} supports {inId}!".__DEBUG_ECHO
            Next
        End If

        Return 0
    End Function

    Private Function __getRegulators(name As String, RegDb As Regulations) As String()
        Dim LDM = GCModeller.FileSystem.GetMotifLDM(name).LoadXml(Of AnnotationModel)
        Dim sites = LDM.Sites.Select(Function(site) site.Name)
        Dim regulators = sites.Select(Function(sId) RegDb.GetRegulators(sId)).ToVector
        Return regulators
    End Function

    <ExportAPI("/Regulon.Reconstruct", Usage:="/Regulon.Reconstruct /bbh <bbh.csv> /genome <RegPrecise.genome.xml> /door <operon.door> [/out <outfile.csv>]")>
    <Group(CLIGrouping.RegulonTools)>
    Public Function RegulonReconstruct(args As CommandLine) As Integer
        Dim bbh As String = args("/bbh")
        Dim genome As String = args("/genome")
        Dim door As String = args("/door")
        Dim out As String = args.GetValue("/out", $"{bbh.TrimSuffix}.{BaseName(genome)}.Regulons.Xml")
        Dim genomeGET = RegulonAPI.Reconstruct(bbh, genome, door)
        Return genomeGET.GetXml.SaveTo(out)
    End Function

    ''' <summary>
    ''' 其实bbh参数的数据类型不一定必须要严格满足bbh，只需要同时具备有query_name和hit_name就可以了
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Regulon.Reconstruct2", Usage:="/Regulon.Reconstruct2 /bbh <bbh.csv> /genome <RegPrecise.genome.DIR> /door <operons.opr> [/out <outDIR>]")>
    <Group(CLIGrouping.RegulonTools)>
    Public Function RegulonReconstructs2(args As CommandLine) As Integer
        Dim bbh As String = args("/bbh")
        Dim genome As String = args("/genome")
        Dim DOOR As String = args("/door")
        Dim out As String = args.GetValue("/out", bbh.TrimSuffix & ".Regulons/")
        Dim bbhValues = bbh.LoadCsv(Of BiDirectionalBesthit)
        Dim genomes = FileIO.FileSystem.GetFiles(genome, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
        Dim doorOperon As SMRUCC.genomics.Assembly.DOOR.DOOR
        If DOOR.FileExists Then
            doorOperon = DOOR_API.Load(DOOR)
        Else
            doorOperon = SMRUCC.genomics.Assembly.DOOR.DOOR.CreateEmpty
        End If
        Dim mapHash = bbhValues.BuildMapHash
        Dim LQuery = (From x As String In genomes
                      Let regulators = RegulonAPI.Reconstruct(mapHash, x.LoadXml(Of BacteriaRegulome), doorOperon)
                      Where Not regulators.IsNullOrEmpty
                      Let id As String = BaseName(x)
                      Select id, _genome = New BacteriaRegulome With {
                          .regulome = New Data.Regprecise.Regulome With {
                                .regulators = regulators
                          },
                          .genome = New JSON.genome With {
                                .name = "@" & id}}).ToArray

        For Each _genome In LQuery
            Dim path As String = $"{out}/{_genome.id}.xml"
            Call _genome._genome.GetXml.SaveTo(path)
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 进行Regulon的批量重建工作
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Regulon.Reconstructs",
               Info:="Doing the regulon reconstruction job in batch mode.",
               Usage:="/Regulon.Reconstructs /bbh <bbh_EXPORT_csv.DIR> /genome <RegPrecise.genome.DIR> [/door <operon.door> /out <outDIR>]")>
    <ArgumentAttribute("/bbh", False, Description:="A directory which contains the bbh export csv data from the localblast tool.")>
    <ArgumentAttribute("/genome", False, Description:="The directory which contains the RegPrecise bacterial genome downloads data from the RegPrecise web server.")>
    <ArgumentAttribute("/door", False, Description:="Door file which is the prediction data of the bacterial operon.")>
    <Group(CLIGrouping.RegulonTools)>
    Public Function RegulonReconstructs(args As CommandLine) As Integer
        Dim inDIR As String = args("/bbh")
        Dim genomeDIR As String = args("/genome")
        Dim door As String = args("/door")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/Regulons")
        Dim bbh As Dictionary(Of String, String) = inDIR.LoadSourceEntryList({"*.csv"}, True)
        Dim genomes As Dictionary(Of String, String) = genomeDIR.LoadSourceEntryList({"*.xml"}, True)
        Dim pairs = (From x As KeyValuePair(Of String, String) In genomes
                     Let paired As String = __pairs(x.Key, bbh)
                     Where Not String.IsNullOrEmpty(paired)
                     Select bbhMapped = paired,
                         genome = x.Value).ToArray
        Dim doorOperon As SMRUCC.genomics.Assembly.DOOR.DOOR
        If door.FileExists Then
            doorOperon = Assembly.DOOR.Load(door)
        Else
            doorOperon = Assembly.DOOR.DOOR.CreateEmpty
        End If
        Dim LQuery = (From x In pairs.AsParallel
                      Select x,
                          genome = RegulonAPI.Reconstruct(x.bbhMapped, x.genome, doorOperon)).ToArray

        For Each genome In LQuery
            Dim path As String = $"{out}/{genome.genome.genome.name.NormalizePathString(True)}.xml"
            Call genome.genome.GetXml.SaveTo(path)
        Next

        Return 0
    End Function

    Private Function __pairs(genome As String, bbh As Dictionary(Of String, String)) As String
        genome = genome.Replace(" ", "_")
        Dim LQuery = (From x In bbh Where InStr(x.Key, genome, CompareMethod.Text) > 0 Select x.Value).FirstOrDefault
        Return LQuery
    End Function

    <ExportAPI("/regulon.export", Usage:="/regulon.export /in <sw-tom_out.DIR> /ref <regulon.bbh.xml.DIR> [/out <out.csv>]")>
    <Group(CLIGrouping.RegulonTools)>
    Public Function ExportRegulon(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim refDIR As String = args("/ref")
        Dim out As String = args.GetValue("/out", inDIR & ".Csv")
        Dim result = RegulonDef.Export(refDIR, inDIR)
        Call result.SaveTo(out)
        result = (From x In result Where InStr(x.Hit, x.Family, CompareMethod.Text) > 0 Select x).ToArray
        out = out.TrimSuffix & ".2.Csv"
        Return result.SaveTo(out).CLICode
    End Function
End Module
