#Region "Microsoft.VisualBasic::c7ad3788b391abc521cce76e45614c00, CLI_tools\c2\_DEBUG_MAIN_.vb"

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

    ' Module _DEBUG_MAIN_
    ' 
    '     Function: ExportRegulatorNetwork, Filte_operon
    ' 
    '     Sub: CreateRegulatorCoExpressionNetwork, ExportRegulatorRegulationNetwork, fff, GenomeWildParse, Main
    '     Class RegulatorMatching
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Invoke
    ' 
    ' 
    ' 
    ' Module CommandLines
    ' 
    '     Function: Match
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
 
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular
Imports LANS.SystemsBiology.Toolkits.RNASeq.WGCNA

Module _DEBUG_MAIN_

    Public Sub GenomeWildParse(WGCNAPath As String, weightCutOff As Double, RegulatorMap As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, MetaCyc As String, ChipData As String, Door As String, Export As String)

        Console.WriteLine("the WGCNA weight cutoff value is {0}", weightCutOff)
        '    Call pairs.SaveTo("./data/debug.csv", False)

        Dim Regulators = (From item As LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH
                          In RegulatorMap.AsDataSource(Of LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH)()
                          Let str = item.QueryName.Trim
                          Select str Distinct).ToArray '获取所有的Regulator的基因号

        Dim TryParsePromoter As c2.Workflows.RegulationNetwork.RegulationNetwork = New Workflows.RegulationNetwork.RegulationNetwork(MetaCyc, Door)

        Console.WriteLine("load WGCNA weight data...")
        Dim pairs As Weight() = WGCNAPath.AsDataSource(Of Weight)(Delimiter:=" ", explicit:=False).ToArray

        Call Console.WriteLine("WGCNA weight max:={0}, min={1}", (From item In pairs Select item.weight).Max, (From item In pairs Select item.weight).Min)

        '遍历每一个Regulator，然后查询出weight大于cutoff值的WGCNA调控对
        Dim LQuery = (From regulator As String In Regulators
                      Let items = (From item In Weight.Find(regulator, pairs) Where item.Weight >= weightCutOff Select item).ToArray
                      Let regulations = (From weight In items Select New Workflows.RegulationNetwork.RegulationNetwork.PossibleRegulation With {.Regulator = regulator, .GeneId = weight.GetOpposite(regulator), .Weight = weight.Weight}).ToArray
                      Select New KeyValuePair(Of String, Workflows.RegulationNetwork.RegulationNetwork.PossibleRegulation())(regulator, regulations)).ToArray

        Console.WriteLine("start to export promoter region....")
        Call FileIO.FileSystem.CreateDirectory(Export)
        '启动子区序列导出
        Call TryParsePromoter.ExportCoExpressionOperons(Export, LQuery, 1000, 25)
    End Sub

    Public Sub CreateRegulatorCoExpressionNetwork(WGCNAPath As String, weightCutOff As Double, RegulatorMap As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, Export As String)

        Console.WriteLine("the WGCNA weight cutoff value is {0}", weightCutOff)

        Dim Regulators = (From item As LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH
                          In RegulatorMap.AsDataSource(Of LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH)()
                          Let str = item.QueryName.Trim
                          Select str Distinct).ToArray '获取所有的Regulator的基因号

        Console.WriteLine("load WGCNA weight data...")
        Dim pairs As Weight() = (From item In WGCNAPath.AsDataSource(Of Weight)(Delimiter:=" ", explicit:=False).ToArray Select item Order By item.Weight Descending).ToArray

        Dim briefView As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Call briefView.Add(New String() {"note", "fromNode", "ToNode", "WGCNA weight"})

        Call Console.WriteLine("WGCNA weight max:={0}, min={1}", pairs.First.Weight, pairs.Last.Weight)
        Call briefView.Add(New String() {"whole genome", pairs.First.FromNode, pairs.First.ToNode, pairs.First.Weight})
        Call briefView.Add(New String() {"whole genome", pairs.Last.FromNode, pairs.Last.ToNode, pairs.Last.Weight})

        pairs = (From item In pairs Where (Array.IndexOf(Regulators, item.FromNode) > -1 AndAlso Array.IndexOf(Regulators, item.ToNode) > -1) Select item).ToArray
        Call briefView.Add(New String() {"regulators", pairs.First.FromNode, pairs.First.ToNode, pairs.First.Weight})
        Call briefView.Add(New String() {"regulators", pairs.Last.FromNode, pairs.Last.ToNode, pairs.Last.Weight})

        pairs = (From item In pairs Where item.Weight >= weightCutOff Select item).ToArray '筛选出weight大于cutoff值的对象，并且仅包含有regulator对象的共表达对
        Call briefView.Add(New String() {"regulators cutoff", pairs.First.FromNode, pairs.First.ToNode, pairs.First.Weight})
        Call briefView.Add(New String() {"regulators cutoff", pairs.Last.FromNode, pairs.Last.ToNode, pairs.Last.Weight})

        Call briefView.Save(FileIO.FileSystem.GetParentPath(Export) & "/briefView.csv", False)

        Dim gettedRegulators = (From item In pairs Select New String() {item.FromNode, item.ToNode}).ToArray
        Dim gettedRegulatorsList = New List(Of String)
        For Each row In gettedRegulators
            Call gettedRegulatorsList.AddRange(row)
        Next
        gettedRegulatorsList = gettedRegulatorsList.Distinct.ToList

        '查询出孤立的点

        Dim writeData As List(Of Weight) = pairs.ToList
        For Each regulator As String In Regulators
            If gettedRegulatorsList.IndexOf(regulator) = -1 Then '这个regulator是一个孤立的点，则生成一个新的对象
                Call writeData.Add(New Weight With {.FromNode = regulator})
            End If
        Next

        Call writeData.SaveTo(Export, False)
    End Sub

    Public Function ExportRegulatorNetwork(RegulatorMap As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, Export As String, Metacyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, door As String)
        Dim Regulators = (From item As LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH
                      In RegulatorMap.AsDataSource(Of LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH)()
                          Let str = item.QueryName.Trim
                          Select str Distinct).ToArray '获取所有的Regulator的基因号
        Dim DoorOperons = LANS.SystemsBiology.Assembly.Door.Load(door).DoorOperonView
        Dim operons = DoorOperons.Select(Regulators)

        Dim TryParsePromoter As c2.Workflows.RegulationNetwork.RegulationNetwork = New Workflows.RegulationNetwork.RegulationNetwork(Metacyc, door)
        '启动子区序列导出
        Call TryParsePromoter.ExportOperonsPromoterRegions(Export, operons, 100, 20)
        Return Nothing
    End Function

    Function Filte_operon(regulations As Regulation(), door As String) As Regulation()
        Dim Operons = LANS.SystemsBiology.Assembly.Door.Load(door).DoorOperonView
        Console.WriteLine("start to filter....")
        regulations = (From item In regulations Select item.SetSameOperon(Operons)).ToArray
        Console.WriteLine("filter job done!")
        Return regulations
    End Function

    Public Sub ExportRegulatorRegulationNetwork(RegulatorMap As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, RegulationCsv As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, Export As String)
        Dim Regulators = (From item As LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH
                   In RegulatorMap.AsDataSource(Of LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH)()
                          Let str = item.QueryName.Trim
                          Select str Distinct).ToArray '获取所有的Regulator的基因号
        Dim Regulations = (From regulation As Regulation In RegulationCsv.AsDataSource(Of Regulation)(False)
                           Where System.Math.Abs(regulation.Pcc) >= 0.6 Select regulation).ToArray

        Dim regulatorRegulations = (From item As Regulation In Regulations Where Array.IndexOf(Regulators, item.Name) > -1 AndAlso Array.IndexOf(Regulators, item.MatchedRegulator) > -1
                                    Select item).ToArray

        Call regulatorRegulations.SaveTo(Export, False)
    End Sub

    Sub Main()
        Dim hhhhhhhhhh As String =
            <fff>F9D030_9BACT/11-126 F9D030.1 PF01475.14;FUR;
Length=116

 Score =  130  bits (326),  Expect = 9e-37, Method: Compositional matrix adjust.
 Identities = 61/119 (51%), Positives = 83/119 (70%), Gaps = 10/119 (8%)

Query  11   NIKPSVQRIAIMDYLLAHKTHPSIDEIYLALCKDIPTLSKTTVYNTLKLFVEHGAALMLT  70
             ++PS+QRIAIMDYLL H THP+++++Y  + K+I TLS+TTVYN+L+LF EH AA M+T
Sbjct  3    GLRPSMQRIAIMDYLLTHHTHPTVEDVYQGIVKEIRTLSRTTVYNSLRLFSEHNAAQMIT  62

Query  71   IDEKNACFDGDTSLHAHFLCKKCGKIFDL-----PYSNEVKKVEQIDMNGFKVDEIHQY  124
            IDE   C+DGD   H HF C+ CG++FDL     P+ +  +K+E     G  VDEI  Y
Sbjct  63   IDEHRVCYDGDIHPHVHFYCRNCGRVFDLLDEDAPHLSGPRKIE-----GNIVDEIQLY  116            </fff>

        Dim s = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel.Score.TryParse(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel.Score)(hhhhhhhhhh)




        '  Dim n = "E:\GCModeller\CompiledAssembly\test\footprints.test_network.csv".LoadCsv(Of LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint)(False)

        '   Call (From path As String In FileIO.FileSystem.GetFiles("E:\Desktop\xcb_vcell\xcb_CARMEN\kegg.compounds", FileIO.SearchOption.SearchTopLevelOnly, "*.xml").AsParallel Select path.LoadXml(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Compound)()).SaveTo("x:\sdfsdfs.csv", False)

        ' Call LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Compound.Download("C00716").GetXml.SaveTo("x:\ffff.txt")

        'Call LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.MergeSabiork.Matches("E:\GCModeller\CompiledAssembly\test\test.txt".LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.BestHit)(False).ToArray,
        '                     "E:\Desktop\xcb_vcell\SabiorkKinetics\EnzymeCatalystKineticLaws.csv".LoadCsv(Of LANS.SystemsBiology.DatabaseServices.SabiorkKineticLaws.CsvData.EnzymeCatalystKineticLaw)(False).ToArray).SaveTo("x:\dgdgdgdg.csv", False)



        '     Call LANS.SystemsBiology.DatabaseServices.SabiorkKineticLaws.SABIORK.ExportDatabase("E:\Desktop\xcb_vcell\sabio-rk", "e:\desktop\")

        'Dim expasy = LANS.SystemsBiology.Assembly.Expasy.NomenclatureDB.CreateObject("E:\Desktop\xcb_vcell\expasy\enzyme.dat")
        'Dim tr = LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.PGDB.Loadder.CreateInstance("E:\BLAST\db\MetaCyc\MetaCyc_ALL\18.0\data", False).GetReactions.GetTransportReactions
        'Dim entries = LANS.SystemsBiology.Assembly.MetaCyc.Schema.TransportReaction.GetTransportReactionExpasyEntries(tr, expasy)
        'Dim fasta = New LANS.SystemsBiology.SequenceModel.FASTA.File

        'Dim swissprotList As List(Of String) = New List(Of String)

        'For Each entryItem In entries
        '    Call swissprotList.AddRange(entryItem.Value)
        'Next

        'swissprotList = (From item In swissprotList Select item Distinct).ToList

        'Call IO.File.WriteAllLines("x:\ddd.txt", swissprotList.ToArray)

        'Dim besthits = "./data/xcb_uniprot.csv".LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.BestHit)(False)
        'Dim uniprotIdlist = (From item In besthits Select item.HitName Distinct).ToArray

        'Dim lll = From id As String In (IO.File.ReadAllLines("./data/trprot.txt")).AsParallel Where Not String.IsNullOrEmpty(id) AndAlso Array.IndexOf(uniprotIdlist, id) > -1 Select (From item In besthits Where String.Equals(id, item.HitName, StringComparison.OrdinalIgnoreCase) Select item).ToArray '
        'Dim listeeeee = New List(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.BestHit)

        'For Each line In lll.ToArray
        '    Call listeeeee.AddRange(line)
        'Next
        'Call listeeeee.SaveTo("c:\dddddddddddddddddddddddddddd.csv", False)

        MsgBox("OK")

        'Call LANS.SystemsBiology.DatabaseServices.SabiorkKineticLaws.SABIORK.Download("E:\Desktop\xcb_vcell\sabio-rk")

        'Dim nn = LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Compound.Download("c00029").GetDBLinks

        'Dim edges As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.DataVisualization.Interactions() = Nothing
        'Dim nodes As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.DataVisualization.NodeAttributes() = Nothing

        'Dim creator = New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.DataVisualization.CytoscapeGenerator("E:\Desktop\xcb_vcell\Xcc8004_CellModel\branches\Test\Xcc8004.xml")
        'Call creator.CreateNetworkFile(edges, nodes)
        'Call creator.AddStringInteractions("E:\Desktop\xcb_vcell\xcb_string-db", edges, nodes)

        'Call edges.SaveTo("x:\EDGES.csv", False)
        'Call nodes.SaveTo("x:\NODES.csv", False)

        'Dim sbml = LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile.Load("E:\BLAST\db\MetaCyc\Xanthomonas_campestris_pv._campestris_str._8004\17.0\data\metabolic-reactions.sbml")
        'Dim compounds = LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.PGDB.Loadder.CreateInstance("E:\BLAST\db\MetaCyc\Xanthomonas_campestris_pv._campestris_str._8004\17.0\data").GetCompounds

        'Dim differ = compounds.Differ(sbml.Model.listOfSpecies, Function(cp As LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Specie) cp.id)


        'Dim dd = "E:\Desktop\xcb_vcell\kegg_metacyc_equations.xml".LoadXml(Of LANS.SystemsBiology.ComponentModel.KeyValuePair())()
        'Dim lq = (From item In dd Let metacyc_Equation = Regex.Match(item.Key, "  \[.+?\]").Value Let kegg_equation = Regex.Match(item.Value, "  \[.+?\]").Value Select New EquationEquals.EquationEqualsMapping With {.Metacyc_Id = item.Key.Replace(metacyc_Equation, ""), .Metacyc_Equation = metacyc_Equation, .KEGG_Id = Replace(item.Value, kegg_equation, ""), .KEGG_Equation = kegg_equation}).ToArray
        'Call lq.SaveTo("x:\sfsf.csv", False)

        ';       MsgBox("OK")

        'Call New LANS.SystemsBiology.DatabaseServices.STrP(
        '    "E:\Desktop\xcb_vcell\xcb_strp.xml".LoadXml(Of LANS.SystemsBiology.DatabaseServices.AssembleSTrP.STrPNetwork),
        '    "E:\Desktop\xcb_vcell\Result\PathwayPromoter_250bp.csv".LoadCsv(Of LANS.SystemsBiology.DatabaseServices.Matched)(False).ToArray) _
        '.CreateObjectNetwork(New String() {"XC_1184", "XC_2252"}, "e:\desktop\1184-2252").SaveTo("x:\ddd.csv", False)

        '   MsgBox("OK")

        '   Call LANS.SystemsBiology.Assembly.NCBI.GenBank.Extensions.ExportProteins_Short(LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.File.Read("E:\Desktop\xoc_vcell\Xanthomonas_oryzae_oryzicola_BLS256_uid16740\CP003057.gbk")).Save("x:\ddd.fsa")

        'Dim nn = New Microsoft.VisualBasic.R.NetworkParameters("", "C:\Program Files\R\R-3.1.0\bin").GetNetworkParameters(numberOfFactors:=3)
        'Call nn.GetXml.SaveTo("x:\dsfsdf.xml")
        'MsgBox("OK")
        'Dim net = CreateNetworkStructures.CreateNetwork("E:\Desktop\xcb_vcell\Result\KEGG_Modules_150bp.csv".LoadCsv(Of LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.TranscriptRegulation)(False).ToArray,
        '                                                Pcc.Convert("E:\Desktop\xcb_vcell\chipData_analysis\r_script\xanChip.csv", True))
        'Call New CreateNetworkStructures(net).SaveTo("x:\dd.txt")

        '' Call Encodings.EncodingChipData("E:\Desktop\xcb_vcell\chipData_analysis\r_script\xanChip.csv", New Double() {0, 0.05, 0.5, 0.8, 0.9, 1}, "x:\xcb_chipdata\")
        'Dim keggreactions = (From path As String
        '                In FileIO.FileSystem.GetFiles("E:\Desktop\xcb_vcell\xcb_CARMEN\kegg.reactions", FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
        '                Select path.LoadXml(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Reaction)()).ToArray

        'Dim idlistddddd = (From item In keggreactions Select item.Entry).ToArray
        'Call LANS.SystemsBiology.DatabaseServices.SabiorkKineticLaws.SABIORK.QueryUsing_KEGGId(idlistddddd, "e:\desktop\sab\")

        'Dim keggcompounds = (From path As String
        '                     In FileIO.FileSystem.GetFiles("E:\Desktop\kegg.compounds", FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
        '                     Select path.LoadXml(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Compound)()).ToArray
        ''    Call New LANS.SystemsBiology.Assembly.CompoundsMapping(LANS.SystemsBiology.Assembly.MetaCyc.File.PGDB.Loadder.CreateInstance("./data/metacyc_all", False)).EffectorMapping(keggcompounds).SaveTo("./data/kegg_metacyc_compounds.csv", False)

        'Call Console.WriteLine("load data...")
        'Dim equals = New EquationEquals(LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.PGDB.Loadder.CreateInstance("E:\BLAST\db\MetaCyc\Xanthomonas_campestris_pv._campestris_str._8004\17.0\data", False), keggcompounds)
        'Call Console.WriteLine("apply analysis...")
        'Call equals.CompoundMapping.SaveTo("x:\ssfs.csv", False)

        'Dim list = equals.ApplyAnalysis(keggreactions)

        'Call list.GetXml.SaveTo("./data/xcb_vcell\kegg_metacyc_equations.xml")
        'Call list.SaveTo("x:\dddd.csv", False)

        'MsgBox("OK")

        'Dim map = New LANS.SystemsBiology.Assembly.MetaCyc.Schema.CompoundsMapping(LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.PGDB.Loadder.CreateInstance("E:\BLAST\db\MetaCyc\MetaCyc_ALL\18.0\data", False))

        'Dim m = map.EffectorMapping(keggcompounds)
        'Call m.SaveTo("x:\ddddd.csv", False)

        ''Dim nn = LANS.SystemsBiology.DatabaseServices.SABIORK.LoadDocument("E:\Desktop\xcb_vcell\sabio-rk\kinlawids_10000.sbml")

        ''Call LANS.SystemsBiology.DatabaseServices.SABIORK.Export("../test/", "../test/")
        ''Call LANS.SystemsBiology.DatabaseServices.SABIORK.Export("E:\Desktop\xcb_vcell\sabio-rk", "e:\desktop\")

        'MsgBox("OK")

        'MsgBox("OK")

        'Dim id_list = (From item In LANS.SystemsBiology.Assembly.SBML.CARMEN.Merge("E:\Desktop\xcb_vcell\xcb_CARMEN") Select item.Id Distinct).ToArray
        'Call LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Reaction.FetchTo(id_list, "e:\desktop\kegg.reactions\")
        'MsgBox("OK")

        'Dim Assembler As New LANS.SystemsBiology.DatabaseServices.AssembleSTrP("E:\Desktop\xan_vcell\xcb_string.xml".LoadXml(Of LANS.SystemsBiology.DatabaseServices.Network).Nodes,
        '                                                                      "E:\Desktop\xan_vcell\xcb_mist2.xml",
        '                                                                      "E:\Desktop\xan_vcell\xan8004_regprecise_bh_BLAST.csv".LoadCsv(Of LANS.SystemsBiology.DatabaseServices.Regprecise.BidrBhRegulator)(False).ToArray)

        'Call Assembler.Assembly("XC_1184").GetXml.SaveTo("x:\xc_1184.xml")
        'Call Assembler.CompileAssembly.GetXml.SaveTo("x:\xcb.xml")

        'Dim network = New List(Of LANS.SystemsBiology.DatabaseServices.StringDB.XmlCommon.NetworkNode)
        'Call network.AddRange("E:\Desktop\xcb_string-db\XC_1184.xml".LoadXml(Of LANS.SystemsBiology.DatabaseServices.EntrySet)().ExtractNetwork())
        'Call network.AddRange("E:\Desktop\xcb_string-db\XC_2252.xml".LoadXml(Of LANS.SystemsBiology.DatabaseServices.EntrySet)().ExtractNetwork())

        'Call network.SaveTo("x:\dddd.csv", False)


        'Dim idlist = (From item In LANS.SystemsBiology.SequenceModel.FASTA.File.Read("E:\Desktop\xan_vcell\xcb.fsa") Select item.Attributes.First.Split.First).ToArray

        'Dim dddd = Global.LANS.SystemsBiology.Assembly.RCSB.PDB.ProtInDb.Load("./data/pdb/info/")
        'Call dddd.ExportInteractions("./data/pdb_export/")



        'MsgBox("OK")
        'Dim qvs = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228.TryParse("./data\xor_regprecise.txt")
        'Dim svq = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228.TryParse("./data\regprecise_xor.txt")

        'Dim grep = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens ' ' 0")

        'Call qvs.Grep(grep.MethodPointer, grep.MethodPointer)
        'Call svq.Grep(grep.MethodPointer, grep.MethodPointer)

        'Dim qvs_csv = qvs.ExportAllBestHist
        'Dim svq_csv = svq.ExportAllBestHist

        'Call qvs_csv.Save("./data\xor_regprecise.csv")
        'Call svq_csv.Save("./data\regprecise_xor.csv")

        'Call LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.GetDiReBh2(svq_csv, qvs_csv).Save("./data\xor_regprecise_bh_BLAST.csv")

        ' ''Call Class1.Invoke(LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.ChipData.Load("E:\Desktop\xan_vcell\chipData_analysis\r_script\xanChip.csv"),
        ' ''                   LANS.SystemsBiology.Assembly.Door.Load("E:\Desktop\xan_vcell\xan_door.opr").DoorOperonView)


        ' ''Dim Parser As New GenomeWildRandomParser(LANS.SystemsBiology.Assembly.Door.Load("./data/xan_door.opr").DoorOperonView, "./data/xcb_genome.fsa", New Integer() {100, 150, 200, 250, 300, 350, 400, 450, 500})
        ' ''Call Parser.TryParse(WGCNAWeight.CreateObject("./data/CytoscapeInput-edges-brown-blue-turquoise-magenta-tan-red-yellow-green-pink-black-purple-greenyellow-grey.txt"), 0.1, 200, "./data/WGCNA_genome_wide/")

        'MsgBox("OK")


        'Dim metabolismFiles = (From file In New String() {".\wt_metabolism_flux.csv",
        '                                                  ".\dm_2736_metabolism_flux.csv",
        '                                                  ".\dm_1308_metabolism_flux.csv"} Select Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(file)).ToArray

        'Dim chunkbuffer As List(Of Double()) = New List(Of Double())

        'For Each file In metabolismFiles
        '    Dim data = (From line In file Let n = System.Math.Log((From ss In line.Skip(1).AsParallel Select Global.System.Math.Abs(Val(ss))).ToArray.Sum) Select n).ToArray
        '    Call chunkbuffer.Add(data)
        'Next

        'Dim datafile = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        'Dim idlist = (From row In metabolismFiles.First Select row.First).ToArray
        'Call datafile.Add(New String() {"rxn", "wt", "dm2736", "dm1308"})

        'For dd As Integer = 0 To idlist.Count - 1
        '    Call datafile.Add(New String() {idlist(dd), chunkbuffer(0)(dd), chunkbuffer(1)(dd), chunkbuffer(2)(dd)})
        'Next

        'Call datafile.Save(".\11111ddddd.csv")

        'MsgBox(1)


        'Dim xccCogs = "./data/xan8004_myva_COG.csv".LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.CogCategory.myvaCog)(False)

        'Dim cog_functions = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.Function.Default

        'Dim regulations = "./data/meme_regulation_analysis_final_result.csv".LoadCsv(Of LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.TranscriptRegulation)(False)
        'Dim TFList = (From item In regulations Select item.TF Distinct).ToArray

        'Dim COGStatics = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File


        'Call COGStatics.Add("Regulator")
        'Call COGStatics.First.AddRange((From item In cog_functions.Categories Select item.Class).ToArray)

        'Dim i As Integer = 0

        'For Each regulator In TFList
        '    Console.WriteLine("{0}     {1}%", regulator, i / TFList.Count)
        '    i += 1
        '    Dim lquery = (From item In regulations Where String.Equals(item.TF, regulator) Select item.OperonGeneIds).ToArray
        '    Dim list = New List(Of String)
        '    For Each line In lquery
        '        Call list.AddRange(line)
        '    Next
        '    list = list.Distinct.ToList

        '    Dim Coglist = (From item In xccCogs Where list.IndexOf(item.QueryName) > -1 Let ss = item.Cog_category Where Not String.IsNullOrEmpty(ss) Select ss).ToArray
        '    Call COGStatics.Add(New String() {regulator})
        '    Call COGStatics.Last.AddRange((From n In cog_functions.Statistics(Coglist) Select CStr(n)).ToArray)
        'Next

        'Call COGStatics.Save("x:\dddddddddddddddddddddddddd.csv")

        'MsgBox(11)
        'Dim resu = LANS.SystemsBiology.DatabaseServices.Regprecise.BidrBhRegulator.Convert("E:\Desktop\xoc_vcell\xor_regprecise_bh_BLAST.csv".LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.DireBesthit).ToArray)
        'Dim regpreciesMatch = LANS.SystemsBiology.DatabaseServices.Regprecise.BidrBhRegulator.Match(resu, "E:\Desktop\xoc_vcell\Regprecise_TranscriptionFactors_By_Genome.xml")
        'Call regpreciesMatch.SaveTo("x:\ffffff.csv")

        'Call HtmlMatching.Invoke("./data/meme.data_xor\meme_out", "./data/meme.data_xor\mast_out", "./data/meme.data_xor\fsa",
        '                 "./data/regprecise_regulator_TFBSs.fsa",
        '                 "./data\xor_regprecise_bh_matched.csv", "./data/xor_door.opr")
        'MsgBox("Job Done!")


        'Call HtmlMatching.Invoke("./data/meme.data\meme_out", "./data/meme.data\mast_out", "./data/meme.data\fsa",
        '                         "./data/regprecise_regulator_TFBSs.fsa",
        '                         "./data\xan8004_regprecise_bh_BLAST.csv",
        '                         "./data/CytoscapeInput-edges-brown-blue-turquoise-magenta-tan-red-yellow-green-pink-black-purple-greenyellow-grey.txt",
        '                         "./data/xanChip.csv", "./data/xan_door.opr")
        'MsgBox("Job Done!")
        'Call ExportRegulatorRegulationNetwork("E:\Desktop\xan_vcell\xan8004_regprecise_bh_BLAST.csv",
        '                                      "E:\Desktop\xan_vcell\255_regulators.csv", "x:\fhfhfhfh.csv")

        'Call Filte_operon("./data\255_regulators.csv".LoadCsv(Of Regulation)(False).ToArray, "./data\xan_door.opr").SaveTo("./data/255_regulators_new.csv", False)

        'MsgBox("samoperons___ok")
        'Using TryParsePromoter As c2.Workflows.RegulationNetwork.RegulationNetwork = New Workflows.RegulationNetwork.RegulationNetwork("E:\BLAST\db\MetaCyc\Xanthomonas_campestris_pv._campestris_str._8004\17.0\data",
        '                                                                                                                               "E:\Desktop\xan_vcell\xan_door.opr")
        '    Call TryParsePromoter.WholeGenomeRandomizeParsed("./data/whole_genome_promoters/whole_genome", 25, 1048576) '大约对整个基因组解析出10GB的数据
        'End Using

        'Using TryParsePromoter As c2.Workflows.RegulationNetwork.RegulationNetwork = New Workflows.RegulationNetwork.RegulationNetwork("./data/metacyc_data", "./data/xan_door.opr")
        '    Call TryParsePromoter.WholeGenomeRandomizeParsed("./data/whole_genome_promoters/whole_genome", 174762, 25) '大约对整个基因组解析出10GB的数据
        'End Using

        '     Call ExportRegulatorNetwork("E:\Desktop\xan_vcell\xan8004_regprecise_bh_BLAST.csv", "e:\desktop\xan_vcell\255regulators\255_regulators", "E:\BLAST\db\MetaCyc\Xanthomonas_campestris_pv._campestris_str._8004\17.0\data", "E:\Desktop\xan_vcell\xan_door.opr")

        'Call CreateRegulatorCoExpressionNetwork("./data/CytoscapeInput-edges-brown-blue-turquoise-magenta-tan-red-yellow-green-pink-black-purple-greenyellow-grey.txt",
        '                                        0.5,
        '                                         "./data/xan8004_regprecise_bh_BLAST.csv",
        '                                        "./network_coexpression_regulators.csv")
        '  MsgBox("OK!")


        Using compiler = New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.Compiler.Compiler()
            Call compiler.PreCompile(CommandLine.CommandLine.TryParse("-precompile -metacyc ""./data/xcb_metacyc"" -regprecise_regulator ""./data\Regprecise_TranscriptionFactors_By_Genome.xml"" -export ""./data/temp/"" -transcript_regulation ""./data/255_regulators_150bp.csv"" -mist2 ""./data\xcb_mist2.xml"" -mist2_strp ""./data\xcb_strp.xml"""))
            Call compiler.Compile(CommandLine.CommandLine.TryParse("-compile -myva_cog ""./data\xcb_myva_COG.csv"" -sabiork ""./data\CompoundSpecies.csv"" -kegg_compounds ""./data\KEGG.Compounds.csv"" " &
                                  "-carmen ""./data\xcb_CARMEN.csv"" -kegg_reactions ""./data\KEGG.Reactions.csv""")).Save("./data/xcb_cellmodels\Xcc8004.xml")
        End Using


        MsgBox("OK!!!")


        Using compiler = New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.Compiler.Compiler()
            Call compiler.PreCompile(CommandLine.CommandLine.TryParse("-precompile -metacyc ""E:\BLAST\db\MetaCyc\Xanthomonas_campestris_pv._campestris_str._8004\17.0\data"" -regprecise_regulator ""E:\Desktop\xcb_vcell\Regprecise_TranscriptionFactors_By_Genome.xml"" -export ""x:\sfsdfsd\"" -transcript_regulation ""E:\Desktop\xcb_vcell\meme_regulation_analysis_final_result.csv"" -mist2 ""E:\Desktop\xcb_vcell\xcb_mist2.xml"" -mist2_strp ""E:\Desktop\xcb_vcell\xcb_strp.xml"""))
            Call compiler.Compile(CommandLine.CommandLine.TryParse("-compile -chipdata ""E:\Desktop\xcb_vcell\chipData_analysis\r_script\xanChip.csv"" " &
                                  "-chipdata_col xan8004_1-RPKM " &
                                  "-myva_cog ""E:\Desktop\xcb_vcell\xan8004_myva_COG.csv""")).Save("E:\desktop\CellModel\Xcc8004.xml")
        End Using

        MsgBox("OK!")
        'Call LANS.SystemsBiology.DatabaseServices.Regprecise.BidrBhRegulator.Match(
        '    Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load("E:\Desktop\xan_vcell\xan8004_regprecise_bh_BLAST.csv").AsDataSource(Of LANS.SystemsBiology.DatabaseServices.Regprecise.BidrBhRegulator)(False),
        '    Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load("E:\Desktop\xan_vcell\xan8004_myva_COG.csv").AsDataSource(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.CogCategory.myvaCog)(False),
        '    "E:\Desktop\xan_vcell\Regprecise_TranscriptionFactors_By_Genome.xml",
        '    Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load("E:\Desktop\xan_vcell\xan8004_pfam-string.csv").AsDataSource(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.ProteinDomainArchitecture.PfamString)(False)).SaveTo("x:\fhgfgfgf.csv", False)

        'End

        '  Call Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load("X:\xan8004_regprecise_bh_BLAST.csv").Counts(4).Save("x:\sfsdfsdfsdfs.csv")

        'Call c2.Weight.Generate("./data/CytoscapeInput-edges-brown-blue-turquoise-magenta-tan-red-yellow-green-pink-black-purple-greenyellow-grey.txt",
        '              "./data/regulation_view/", "./data/xanChip.csv", "./minimum-WGCNA-weights with PCC large than 0.6.csv")

        'Call New LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.PartialBestBLAST(New LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus("E:\BLAST\bin"), "e:\desktop\").Peformance(
        '    "E:\Desktop\xan_vcell\xan8004.fsa", "E:\Desktop\xan_vcell\regprecise_regulator_sequence.fsa",
        '    Nothing,
        '    LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens ' ' 0").MethodPointer,
        '    , True).Save("x:\dddddd.csv")

        'Call LANS.SystemsBiology.DatabaseServices.Regprecise.BidrBhRegulator.Match("E:\Desktop\xan_vcell\regprecise_regulator_sequence.fsa",
        '                                                                           "E:\Desktop\xan_vcell\regprecise_regulator_TFBSs.fsa",
        '                                                                        "E:\Desktop\xan_vcell\xan8004.fsa",
        '                                                                        LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.CreateInstance("E:\BLAST\bin", LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.Program.BlastPlus),
        '                                                                        "tokens ' ' 0", "E:\Desktop\xan_vcell\xan8004_regprecise_bh_BLAST", True).SaveTo("x:\dddd.csv")
        ''Call LANS.SystemsBiology.DatabaseServices.DEG.Workflows.CreateReportView(LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228.TryParse("E:\Desktop\deg\ess_deg"),
        ''                                                                         LANS.SystemsBiology.DatabaseServices.DEG.Annotations.Load(AnnotionFile:="E:\Desktop\deg\degannotation-p.dat")).Save("x:\deg.csv", False)
        ' End

        'Dim ddddd = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228.TryParse("E:\meme_analysis_logs_result\xan8004_regprecise_bh_BLAST\regprecise_regulators_vs_xan8004.txt")
        'Dim fsdfsfsd = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228.TryParse("E:\meme_analysis_logs_result\xan8004_regprecise_bh_BLAST\xan8004_vs_regprecise_regulators.txt")

        'Call ddddd.Grep(LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens | 1;tokens ' ' 0").MethodPointer, LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens ' ' 0").MethodPointer)
        'Call fsdfsfsd.Grep(LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens ' ' 0").MethodPointer, LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens | 1;tokens ' ' 0").MethodPointer)

        'Dim csvddddd = ddddd.ExportAllBestHist
        'Dim csvfsfdfsfsfsf = fsdfsfsd.ExportAllBestHist

        'Call csvddddd.Save("x:\regprecise_regulators_vs_xan8004.csv")
        'Call csvfsfdfsfsfsf.Save("x:\xan8004_vs_regprecise_regulators.csv")

        'Call LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.GetDiReBh2(csvddddd, csvfsfdfsfsfsf).Save("x:\besthits.csv")

        'Dim regulations = "E:\meme_analysis_logs_result\predicted_regulation\matched\regulation_view\CoExpressionOperons_250bp.csv".LoadCsv(Of Regulation)(False)

        'Dim lll = (From item In regulations Where String.Equals(item.GeneId, "XC_1184") Select item).ToArray

        'Call lll.SaveTo("x:\xc_1184.csv", False)

        'lll = (From item In regulations Where String.Equals(item.GeneId, "XC_2252") Select item).ToArray

        'Call lll.SaveTo("x:\xc_2252.csv", False)
        'lll = (From item In regulations Where String.Equals(item.GeneId, "XC_2736") Select item).ToArray

        'Call lll.SaveTo("x:\xc_2736.csv", False)
        'lll = (From item In regulations Where String.Equals(item.GeneId, "XC_1305") Select item).ToArray

        'Call lll.SaveTo("x:\xc_1305.csv", False)
        'lll = (From item In regulations Where String.Equals(item.GeneId, "XC_1307") Select item).ToArray

        'Call lll.SaveTo("x:\xc_1307.csv", False)

        'lll = (From item In regulations Where String.Equals(item.GeneId, "XC_1308") Select item).ToArray

        'Call lll.SaveTo("x:\xc_1308.csv", False)

        'MsgBox(1)

        'Dim log = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228.TryParse("E:\meme_analysis_logs_result\xan8004_regprecise_bh_BLAST\xan8004_vs_regprecise_regulators.txt")
        'Call log.Grep(LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens ' ' 0").MethodPointer, LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens ' ' 0").MethodPointer)
        'Call log.ExportBestHit.Save("E:\meme_analysis_logs_result\xan8004_regprecise_bh_BLAST\xan8004_vs_regprecise_regulators.csv")

        'Call LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.GetDiReBh("E:\meme_analysis_logs_result\xan8004_regprecise_bh_BLAST\regprecise_regulators_vs_xan8004.csv",
        '                                                                                            "E:\meme_analysis_logs_result\xan8004_regprecise_bh_BLAST\xan8004_vs_regprecise_regulators.csv") _
        '                                                                                        .Save("E:\meme_analysis_logs_result\xan8004_regprecise_bh_BLAST\xan8004_regprecise_bh_BLAST.csv")
        'MsgBox("ok!")

        'Call c2.PathwayModuleFilter.Match("E:\meme_analysis_logs_result\xan8004_parsed_promoters\PathwayOverviews.csv",
        '                                  "E:\meme_analysis_logs_result\chipData_analysis\r_script\CytoscapeInput-nodes-brown-blue-turquoise-magenta-tan-red-yellow-green-pink-black-purple-greenyellow-grey.txt",
        '                                  "E:\meme_analysis_logs_result\xan8004_myva_COG.csv").Save("h:\desktop\dddd.csv")

        'Call LANS.SystemsBiology.DatabaseServices.Regtransbase.WebServices.WebServiceHandler.DownloadSequenceData("H:\Desktop\meme_analysis_logs_result\regprecise_tff.xml", "h:\desktop\temp.fsa").Save("h:\desktop\regulators.fsa")
        'End
        'Call fff()


        'Call c2.Workflows.MEME_Analysis_ACtions.Export("E:\meme_analysis_logs_result\meme_out\PathwayPromoters",
        '                                               "H:\Desktop\coexpression\mast_out",
        '                                               "E:\meme_analysis_logs_result\xan8004_parsed_promoters")

        'Call MatchedInformation.Match("H:\Desktop\coexpression\meme_out",
        '                            "H:\Desktop\coexpression\mast_out",
        '                            "h:\Desktop\meme_analysis_logs_result\regprecise_regulator_sequence.fsa",
        '                            "h:\Desktop\meme_analysis_logs_result\xan8004_regprecise_bh_BLAST\xan8004_regprecise_bh_BLAST.csv",
        '                            "h:\Desktop\coexpression\predicted_regulations",
        '                            "h:\Desktop\meme_analysis_logs_result\regprecise_regulator_TFBSs.fsa ",
        '                            IO.File.ReadAllLines("H:\Desktop\meme_analysis_logs_result\virulence_genes_523"),
        '                            "H:\Desktop\meme_analysis_logs_result\regprecise_tff.xml",
        '                            "E:\BLAST\db\MetaCyc\Xanthomonas_campestris_pv._campestris_str._8004\17.0\data",
        '                            "H:\Desktop\meme_analysis_logs_result\xan8004_myva_COG.csv", "H:\Desktop\meme_analysis_logs_result\xan8004_regprecise_bh_BLAST\xan8004.fsa")

        '1:.启动子序列解析()
        'Dim Parser As New Workflows.RegulationNetwork.RegulationNetwork("./data\Xanthomonas_campestris_pv._campestris_str._8004\17.0\data",
        '                                                                "./data/xanChip.csv",
        '                                                                "./data\xan_door.opr")
        'Call Parser.ExtractPromoterRegion_KEGGModules("E:\Desktop\A Comprehensive study of the genome wild regulation profile for bacteria Xanthomonas campestris pv. campestris str. 8004\parsed_promoter_region_fasta",
        '                                              "E:\Desktop\A Comprehensive study of the genome wild regulation profile for bacteria Xanthomonas campestris pv. campestris str. 8004\kegg_modules_xcb.csv",
        '                                              "E:\meme_analysis_logs_result\xan_door.opr")
        'Call Parser.ExtractPromoterRegion_Pathway("E:\Desktop\A Comprehensive study of the genome wild regulation profile for bacteria Xanthomonas campestris pv. campestris str. 8004\parsed_promoter_region_fasta",
        '                                          "E:\meme_analysis_logs_result\xan_door.opr")

        '  Call LANS.SystemsBiology.Assembly.KEGG.WebServices.Modules.Download2("xor").SaveTo("e:\desktop\xor_kegg_modules.csv", False)

        '   MsgBox("ok!")

        '1:      .启动子序列解析()
        Dim Parser As New Workflows.RegulationNetwork.RegulationNetwork("E:\Desktop\xoc_vcell\xory383407\17.0\data", "E:\Desktop\xoc_vcell\xor_door.opr")
        Call Parser.ExtractPromoterRegion_KEGGModules("E:\Desktop\xoc_vcell\xor_kegg_modules\", "E:\Desktop\xoc_vcell\xor_kegg_modules.csv")
        Call Parser.ExtractPromoterRegion_Pathway("E:\Desktop\xoc_vcell/xor_metacyc_pathways\")

        MsgBox("ok!")

        'Call New RegulatorMatching("E:\Desktop\xan_vcell\meme.data\255regulators\meme_out",
        '                        "E:\Desktop\xan_vcell\meme.data\255regulators\mast_out",
        '                         "E:\Desktop\xan_vcell\255regulators",
        '                          "E:\Desktop\xan_vcell\regprecise_regulator_sequence.fsa",
        '                           "E:\Desktop\xan_vcell\regprecise_regulator_TFBSs.fsa",
        '                           "E:\Desktop\xan_vcell\xan8004_regprecise_bh_BLAST.csv").Invoke(
        '                           "E:\Desktop\xan_vcell\xan8004_myva_COG.csv",
        '                            "E:\Desktop\xan_vcell\Regprecise_TranscriptionFactors_By_Genome.xml",
        '                            "E:\Desktop\xan_vcell\chipData_analysis\r_script\xanChip.csv",
        '                            "E:\Desktop\xan_vcell\chipData_analysis\r_script\CytoscapeInput-edges-brown-blue-turquoise-magenta-tan-red-yellow-green-pink-black-purple-greenyellow-grey.txt")

        Call New RegulatorMatching("./data\meme.data\meme_out",
                                   "./data\meme.data\mast_out",
                                    "./data\parsed_promoter_region_fasta",
                                     "./data\regprecise_regulator_sequence.fsa",
                                      "./data\regprecise_regulator_TFBSs.fsa",
                                      "./data\xan8004_regprecise_bh_BLAST.csv").Invoke(
                                      "./data\xan8004_myva_COG.csv",
                                       "./data\Regprecise_TranscriptionFactors_By_Genome.xml", "./data/xanChip.csv",
                                       "./data/CytoscapeInput-edges-brown-blue-turquoise-magenta-tan-red-yellow-green-pink-black-purple-greenyellow-grey.txt")

        'Call GenomeWildParse("./data/CytoscapeInput-edges-brown-blue-turquoise-magenta-tan-red-yellow-green-pink-black-purple-greenyellow-grey.txt", 0.036059011,
        '                       "./data/xan8004_regprecise_bh_BLAST.csv",
        '                       "./data/metacyc_data/",
        '                       "./data/xanChip.csv",
        '                       "./data/xan_door.opr",
        '                        "./data/export/WGCNA")
        MsgBox("OK")
    End Sub

    ''' <summary>
    ''' 处理MEME的输出结果
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RegulatorMatching

        Dim matchedList As KeyValuePair(Of String, Workflows.MatchedResult())()
        Dim MatchedExportDir As String
        Dim regprecise_bh_BLAST As LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MEME_Out">MEME批处理程序输出文件夹</param>
        ''' <param name="MAST_Out">MAST批处理程序输出文件夹</param>
        ''' <param name="FASTA">MEME批处理程序所需要的FASTA序列数据的文件夹</param>
        ''' <param name="besthit">目标基因组和regprecise数据库的最佳比对结果</param>
        ''' <remarks></remarks>
        Sub New(MEME_Out As String, MAST_Out As String, FASTA As String, regprecise_regulator_sequence As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile,
                regprecise_regulator_TFBSs As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile, besthit As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
            '2:      .在Linux中预测完毕后解析数据()
            Dim ExportDir As String = c2.Workflows.MEME_Analysis_ACtions.Export(MEME_Out, MAST_Out, FASTA)
            '4.做完BLAST序列比对之后将基因进行一一对应
            Me.MatchedExportDir = c2.Workflows.MEME_Analysis_ACtions.FinalRegulationMatch(ExportDir,
                                                                          regprecise_regulator_sequence,
                                                                          regprecise_regulator_TFBSs,
                                                                           besthit)
            Me.matchedList = (From path As String
                       In FileIO.FileSystem.GetFiles(MatchedExportDir,
                                                     FileIO.SearchOption.SearchTopLevelOnly, "*.csv").AsParallel
                              Select New KeyValuePair(Of String, Workflows.MatchedResult())(path, Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(path).AsDataSource(Of Workflows.MatchedResult)(False))).ToArray
            Me.regprecise_bh_BLAST = besthit.AsDataSource(Of LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH)(False)
        End Sub

        Public Sub Invoke(MyvaCog As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, Regprecise_TranscriptionFactors_By_Genome As LANS.SystemsBiology.DatabaseServices.Regprecise.TranscriptionFactors, ChipData As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
                          WGCNAPath As String)
            '            '代谢途径调控视图
            '#If DEBUG Then
            '            For Each file In matchedList
            '#Else
            '        Parallel.ForEach(matchedList, Sub(file As KeyValuePair(Of String, MEME.Workflows.MatchedResult()))
            '#End If
            '                ' Console.WriteLine("create object regulation views {0}", file.FilePath)
            '                Dim LQuery = (From row In file.Value Select String.Format("{0},{1}", row.MatchedRegulator, row.ObjectId) Distinct).ToArray
            '                Dim newfile = Me.MatchedExportDir & "/regulation_object_view/" & FileIO.FileSystem.GetName(file.Key)
            '                Console.WriteLine("writing new file {0}", newfile)
            '                Call LQuery.SaveTo(newfile)
            '#If DEBUG Then
            '            Next
            '#Else
            '                                      End Sub)
            '#End If
            '            'COG分类视图
            '            Console.WriteLine("start to create cog view")

            '#If DEBUG Then
            '            For Each file In matchedList
            '#Else
            '        Parallel.ForEach(matchedList, Sub(file As KeyValuePair(Of String, MEME.Workflows.MatchedResult()))
            '#End If
            '                Console.WriteLine("generated regulators informations {0}", file.Key)
            '                Dim regulators = (From row In file.Value.AsParallel
            '                                  Let id = row.MatchedRegulator
            '                                  Where Not (String.IsNullOrEmpty(id) OrElse String.Equals(id, LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.IBlastOutput.HITS_NOT_FOUND))
            '                       Select id Distinct).ToArray
            '                Dim staticsfile = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File

            '                Call staticsfile.Add(New String() {"regulator"})
            '                Call staticsfile.First.AddRange((From item In LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.Function.Default.Categories Select item.Class).ToArray)

            '                Dim csvLQuery = (From regulator In regulators.AsParallel
            '                                 Let geneline = Function() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Row
            '                                                    Dim rows = (From row As Workflows.MatchedResult In file.Value.AsParallel Where String.Equals(row.MatchedRegulator, regulator) Select row).ToArray
            '                                                    If Not rows.IsNullOrEmpty Then
            '                                                        Dim LQuery = (From rwo As Workflows.MatchedResult In rows Select rwo.OperonGenes).ToArray
            '                                                        Dim list = New List(Of String)
            '                                                        For Each operon In LQuery
            '                                                            Call list.AddRange(operon)
            '                                                        Next
            '                                                        list = list.Distinct.ToList

            '                                                        Dim cogList = (From gene In list
            '                                                                       Let gegg = Function() As String
            '                                                                                      Dim row = MyvaCog.FindAtColumn(gene, 0)
            '                                                                                      If Not row.IsNullOrEmpty Then
            '                                                                                          Return row.First()(3)
            '                                                                                      Else
            '                                                                                          Return ""
            '                                                                                      End If
            '                                                                                  End Function Let s = gegg() Where Not String.IsNullOrEmpty(s) Select s).ToArray

            '                                                        '获得所调控的基因的COG分类功能列表
            '                                                        Dim cog_statics = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.Function.Default.Statistics(cogList)

            '                                                        Dim newrow = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Row
            '                                                        Call newrow.Add(regulator)
            '                                                        Call newrow.AddRange((From n In cog_statics Select CStr(n)).ToArray)

            '                                                        Return newrow
            '                                                    Else
            '                                                        Return Nothing
            '                                                    End If
            '                                                End Function Select geneline()).ToArray
            '                Console.WriteLine("created file done!")

            '                Call staticsfile.AppendRange(csvLQuery)
            '                Call staticsfile.Save(FileIO.FileSystem.GetParentPath(file.Key) & "/cog_statics/" & FileIO.FileSystem.GetName(file.Key))
            '#If DEBUG Then
            '            Next
            '#Else
            '                                      End Sub)
            '#End If

            '            '基因对基因调控视图, 用于生成计算模型

            '            Dim regpreciseRegulators = Regprecise_TranscriptionFactors_By_Genome.Regulators
            '            Dim Pcc = c2.Pcc.CreateObject(ChipData, True)
            '            Dim WGCNA = WGCNAWeight.CreateObject(WGCNAPath)

            '#If DEBUG Then
            '            For Each file In matchedList
            '#Else
            '        Parallel.ForEach(matchedList, Sub(file As KeyValuePair(Of String, MEME.Workflows.MatchedResult()))
            '#End If
            '                Dim Genes As List(Of String) = New List(Of String)
            '                For Each row In file.Value
            '                    Dim list = row.OperonGenes
            '                    Call Genes.AddRange(list)
            '                Next
            '                Genes = Genes.Distinct.ToList

            '                Dim regulationfile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
            '                Call regulationfile.Add(New String() {"GeneId", "MatchedRegulator", "Family", "Effector", "BiologicalProcess", "Regulation", "RegpreciseRegulog", "WGCNAWeight", "Pcc"})

            '                For Each gene In Genes
            '                    Dim regulators = (From row In (From ddd In file.Value Where Array.IndexOf(ddd.OperonGenes, gene) > -1 Select ddd) Let id = row.MatchedRegulator
            '    Where Not (String.IsNullOrEmpty(id) OrElse String.Equals(id, LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.IBlastOutput.HITS_NOT_FOUND)) Select row.MatchedRegulator Distinct).ToArray

            '                    For Each regulator In regulators
            '                        Dim matched = (From item In Me.regprecise_bh_BLAST Where String.Equals(item.Matched, regulator) Select item).ToArray.First
            '                        Dim obj = (From item In regpreciseRegulators Where String.Equals(item.LocusTag.Key, matched.Regulator.Split(":").Last) Select item).First
            '                        Dim WGCNAPair = WGCNA.Find(gene, matched.Matched)

            '                        Dim row = New String() {gene, matched.Matched, obj.Family, obj.Effector, obj.BiologicalProcess, obj.RegulationMode, obj.Regulog.Key, If(WGCNAPair Is Nothing, 0, WGCNAPair.weight), Pcc.GetValue(gene, matched.Matched)}
            '                        '  Dim row = New String() {gene, matched.Matched, obj.Family, obj.Effector, obj.BiologicalProcess, obj.RegulationMode, obj.Regulog.Key, 0, Pcc.GetValue(gene, matched.Matched)}


            '                        Call regulationfile.Add(row)
            '                    Next
            '                Next

            '                Call regulationfile.Save(FileIO.FileSystem.GetParentPath(file.Key) & "/regulation_view/" & FileIO.FileSystem.GetName(file.Key))
            '#If DEBUG Then
            '            Next
            '#Else
            '                                      End Sub)
            '#End If
        End Sub
    End Class

    '    Public Sub generateModel()
    '        Console.WriteLine("start to generate model function ")

    '        Dim regpreciseRegulators = LANS.SystemsBiology.DatabaseServices.TranscriptionFactors.Load("./data/Regprecise_TranscriptionFactors_By_Genome.xml").Regulators
    '        Dim regpreciseMatched = Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load("./data/regprecise_regulators_vs_xan8004.csv")
    '#If DEBUG Then
    '#Else
    '        Dim Pcc As Pcc.ItemObject() = CoExpression.Calculate(c2.Pcc.Convert(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load("./data/xanChip.csv"), True))

    '        Dim PccGenes As String() = (From item In Pcc Select item.GeneId).ToArray

    '        Dim GetPCC = Function(regulatorId As String, geneId As String) As Double
    '                         Dim LQuery = (From item In Pcc.AsParallel Where String.Equals(regulatorId, item.GeneId) Select item).ToArray
    '                         If Not LQuery.IsNullOrEmpty Then
    '                             Dim idx = Array.IndexOf(PccGenes, geneId)
    '                             If idx > -1 Then
    '                                 Return LQuery.First.SampleValue(idx)
    '                             Else
    '                                 Return 0
    '                             End If
    '                         Else
    '                             Return 0
    '                         End If
    '                     End Function
    '        Dim WGCNAWeights = IO.File.ReadAllLines("./data/CytoscapeInput-edges-brown-blue-turquoise-magenta-tan-red-yellow-green-pink-black-purple-greenyellow-grey.txt").Skip(1).ToArray

    '        Dim GetWGCNAWeight = Function(RegulatorId As String(), geneId As String) As Double()
    '                                 If RegulatorId.IsNullOrEmpty Then
    '                                     Return New Double() {}
    '                                 End If

    '                                 Dim Weights As Double() = New Double(RegulatorId.Count - 1) {}
    '                                 For i As Integer = 0 To Weights.Count - 1
    '                                     Dim id As String = RegulatorId(i)
    '                                     Dim LQuery = (From line In WGCNAWeights.AsParallel Let row = line.Split Where (String.Equals(row(0), id) AndAlso String.Equals(row(1), geneId)) OrElse
    '                   (String.Equals(row(0), geneId) AndAlso String.Equals(row(1), id)) Select row(2)).ToArray
    '                                     If LQuery.IsNullOrEmpty Then
    '                                         Weights(i) = 0
    '                                     Else
    '                                         Weights(i) = Val(LQuery.First)
    '                                     End If
    '                                 Next

    '                                 Return Weights
    '                             End Function
    '#End If

    '#If DEBUG Then
    'ff:     For Each file In (From path In FileIO.FileSystem.GetFiles("./data/matched/", FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
    '                       Select Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(path)).ToArray
    '#Else
    '        Call Parallel.ForEach((From path In FileIO.FileSystem.GetFiles("./data/matched/", FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
    '                     Select Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(path)).ToArray,
    '                 Sub(file As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
    '#End If



    '                     Dim savedPath = String.Format("{0}/model_files/{1}.xml", FileIO.FileSystem.GetParentPath(file.FilePath), FileIO.FileSystem.GetName(file.FilePath))

    '                     Console.WriteLine("The generated model will be saved to {0}", savedPath)


    '                     Dim Genes As List(Of String) = New List(Of String)
    '                     For Each row In file.Skip(1)
    '                         Dim list = Split(Split(row(1), " --> ").Last, "; ")
    '                         Call Genes.AddRange(list)
    '                     Next
    '                     Genes = Genes.Distinct.ToList

    '                     Console.WriteLine("created gene list!")

    '                     '    Call IO.File.WriteAllLines("x:\temp_genes.txt", Genes)


    '                     Dim RegulatorList = (From row In file.Skip(1).AsParallel Let id = row(7) Where Not (String.IsNullOrEmpty(id) OrElse String.Equals(id, "No Hits Found")) Select regpreciseMatched.FindAtColumn(row(6), 0).First()(1) Distinct).ToArray

    '                     Console.WriteLine("created regulator list!")

    '                     '  Call IO.File.WriteAllLines("x:\temp_regulators.txt", RegulatorList)

    '                     Dim model As LANS.SystemsBiology.GCModeller.ModelSolvers.FBA.rFBA.NetworkModel = New LANS.SystemsBiology.GCModeller.ModelSolvers.FBA.rFBA.NetworkModel
    '                     model.NetworkComponents = New LANS.SystemsBiology.GCModeller.ModelSolvers.FBA.rFBA.BaseEquation(Genes.Count + RegulatorList.Count - 1) {}

    '                     Console.WriteLine("initialize regulator list in model...")
    '                     '初始化调控因子列表
    '                     For i = Genes.Count To model.NetworkComponents.Count - 1
    '                         Dim idx = i - Genes.Count
    '                         model.NetworkComponents(i) = New LANS.SystemsBiology.GCModeller.ModelSolvers.FBA.rFBA.BaseEquation With {.Name = RegulatorList(idx), .Handle = i, .Variables = New LANS.SystemsBiology.GCModeller.ModelSolvers.FBA.rFBA.BaseEquation.Variable() {}}
    '                     Next

    '                     Console.WriteLine("initialized done!")

    '                     For i As Integer = 0 To Genes.Count - 1
    '                         Dim gene = Genes(i)
    '                         Dim regulators = (From row In file.FindAtColumn(gene, 1).AsParallel Let id = row(7) Where Not (String.IsNullOrEmpty(id) OrElse String.Equals(id, "No Hits Found")) Select row(6) Distinct).ToArray
    '                         Dim Equation As LANS.SystemsBiology.GCModeller.ModelSolvers.FBA.rFBA.BaseEquation = New LANS.SystemsBiology.GCModeller.ModelSolvers.FBA.rFBA.BaseEquation
    '                         Equation.Name = gene
    '                         Equation.Handle = i
    '                         Equation.Variables = New LANS.SystemsBiology.GCModeller.ModelSolvers.FBA.rFBA.BaseEquation.Variable(regulators.Count - 1) {}

    '                         Dim idx As Integer = 0

    '                         regulators = (From regulator In regulators.AsParallel Select regpreciseMatched.FindAtColumn(regulator, 0).First()(1)).ToArray
    '                         '       Let id = regulator.Split(":").Last
    '                         'obj = (From item In regpreciseRegulators
    '                         'Where String.Equals(item.LocusTag.Key, id, StringComparison.OrdinalIgnoreCase)
    '                         '      Select item).First   '???????????????
    '#If DEBUG Then
    '#Else
    '                         Dim regulatorWeights = GetWGCNAWeight(regulators, gene)
    '#End If
    '                         For regulatorIdx = 0 To regulators.Count - 1
    '                             Dim RegulatorReference As New LANS.SystemsBiology.GCModeller.ModelSolvers.FBA.rFBA.BaseEquation.Variable()
    '                             Dim matched = regulators(regulatorIdx)
    '                             Dim [Handles] = (From item In model.NetworkComponents.AsParallel Where Not item Is Nothing AndAlso String.Equals(item.Name, matched) Select item).ToArray
    '                             If Not [Handles].IsNullOrEmpty Then
    '                                 RegulatorReference.pHandle = [Handles].First.Handle
    '                             Else
    '                                 RegulatorReference.pHandle = -1
    '                             End If
    '#If DEBUG Then
    '#Else

    '                             RegulatorReference.Pcc = GetPCC(matched, gene)
    '                             RegulatorReference.Weight = regulatorWeights(regulatorIdx)
    '#End If


    '                             Equation.Variables(idx) = RegulatorReference
    '                             idx += 1
    '                         Next
    '                         model.NetworkComponents(i) = Equation
    '                     Next
    '                     Call model.GetXml.SaveTo(savedPath)
    '#If DEBUG Then
    '        Next
    '#Else
    '                 End Sub)
    '#End If
    '    End Sub

    Sub fff()
        Dim idlist As String() = IO.File.ReadAllLines("H:\Desktop\New folder\ID.csv")
        Dim match = Function(csv As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, idListCol As Integer)
                        Dim file As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File =
                            New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
                        file.AppendLine(New String() {"Gene_ID", "ObjectId"})

                        For Each id In idlist
                            Dim LQuery = (From row In csv Where Array.IndexOf(Strings.Split(row(idListCol), "; "), id) > -1 Select row).ToArray
                            For Each row In LQuery
                                Dim newRow As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
                                Call newRow.Add(id)
                                Call newRow.AddRange(row)

                                Call file.AppendLine(newRow)
                            Next
                        Next

                        Return file
                    End Function

        Call match("H:\Desktop\New folder\kegg_modules_xcb.csv", 3).Save("H:\Desktop\New folder\vir_kegg_modules_xcb.csv", False)
        Call match("H:\Desktop\New folder\kegg_pathways_xcb.csv", 3).Save("H:\Desktop\New folder\vir_kegg_pathways_xcb.csv", False)
        Call match("H:\Desktop\New folder\pathway_overviews.csv", 4).Save("H:\Desktop\New folder\vir_biocyc_pathway_overviews.csv", False)

        End
    End Sub
End Module

Partial Module CommandLines

    <ExportAPI("-match_regulators", Usage:="-match_regulators -meme_out <meme_out_dir> " &
                                                                 "-mast_out <mast_out_dir> " &
                                                                 "-regprecise <regprecise_regulator_sequence_fasta> " &
                                                                 "-regprecise_bh_BLAST <regprecise_bh_BLAST_csv> " &
                                                                 "-exported_dir <exported_dir> " &
                                                                 "-tfbs <regprecise_regulator_TFBSs_fasta> " &
                                                                 "-virulence_genes <virulence_genes_text> " &
                                                                 "-regprecise_tff <regprecise_tff_xml> " &
                                                                 "-metacyc <metacyc_dir> " &
                                                                 "-myva_COG <myva_COG_csv> " &
                                                                 "-os_orf <os_orf_fasta>")>
    Public Function Match(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim MEME_OUT As String = CommandLine("-meme_out"),
            MAST_OUT As String = CommandLine("-mast_out"),
            RegulatorSequence As String = CommandLine("regprecise"),
            BestHitCsv As String = CommandLine("-regprecise_bh_BLAST"),
            ExportDir As String = CommandLine("-exported_dir"),
            TFBSInfo As String = CommandLine("-tfbs"),
            virulenceList As String() = IO.File.ReadAllLines(CommandLine("-virulence_genes")),
            RegpreciseTFF As String = CommandLine("-regprecise_tff"),
            MetaCycDir As String = CommandLine("-metacyc"),
            COGProfile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = CommandLine("-myva_COG"),
            Proteins As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile = CommandLine("-os_orf")

        Call MatchedInformation.Match(MEME_OUT, MAST_OUT, RegulatorSequence, BestHitCsv, ExportDir, TFBSInfo, virulenceList, RegpreciseTFF, MetaCycDir, COGProfile, Proteins)
        Return 0
    End Function

End Module
