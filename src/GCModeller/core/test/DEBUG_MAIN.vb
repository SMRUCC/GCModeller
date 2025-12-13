#Region "Microsoft.VisualBasic::be7c0d95ed7f666bbdf582f6a97188b0, core\test\DEBUG_MAIN.vb"

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

' Module DEBUG_MAIN
' 
'     Sub: Main, ReadDatabase
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Module DEBUG_MAIN

    Public Sub ReadDatabase()

        Dim path As String = "/path/to/database/file"
        Dim gb As GBFF.File = GBFF.File.Load(path)
        Dim gbs As IEnumerable(Of GBFF.File) = GBFF.File.LoadDatabase(path)
        Dim PTT As PTT = PTT.Load(path)
        Dim GFF As GFF.GFFTable = GFFTable.LoadDocument(path)

        Dim Fasta As New FASTA.FastaFile(path)
        Dim nt As New FASTA.FastaSeq(path)

        nt = FastaSeq.Load(path)
        nt = FastaSeq.LoadNucleotideData(path)
    End Sub

    Sub Main()

        'Dim resultddddd = SMRUCC.genomics.Assembly.KEGG.WebServices.Map.ParseHTML("D:\GCModeller\src\GCModeller\core\Testing\hsa05034.html")

        '  Call resultddddd.GetXml.SaveTo("D:\GCModeller\src\GCModeller\core\hsa05034.XML")


        Pause()

        '  Call "http://www.genome.jp/dbget-bin/www_bget?pathway:hsa00010".GET.SaveTo("x:\pathway_Test.html")
        ' Call "http://www.kegg.jp/dbget-bin/www_bget?pathway+hsa00600".GET.SaveTo("x:\pathway_Test2.html")
        'Pause()
        '  Dim pathW = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway.DownloadPage("D:\KEGG\hsa\webpages\hsa00601.html")


        Pause()

        ' Dim rxn = KEGG.DBGET.bGetObject.ReactionWebAPI.Download("R00235")


        Pause()


        '   Dim cpdTest As KEGG.DBGET.bGetObject.Compound = MetaboliteWebApi.DownloadCompound("G:\GCModeller\GCModeller\test\KEGG\dbget\cpd_Test.html") 'MetabolitesDBGet.DownloadCompound("C00311")

        ' Call "http://www.kegg.jp/dbget-bin/www_bget?gl:G00112".GET.SaveTo("x:\gl_Test.html")


        '   Dim KEGG_gl As Glycan = Glycan.DownloadFrom("G:\GCModeller\GCModeller\test\KEGG\dbget\gl_Test.html")

        Pause()




        ' Dim gene = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.KEGGgenomeFetch.DownloadURL("G:\GCModeller\GCModeller\test\KEGG\dbget\human_gene.html")

        Dim htext As htext = htext.StreamParser("C:\Users\xieguigang\Downloads\br08402.keg")

        ' Call SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.DownloadWorker.DownloadDisease(htext, "x:\test\")


        ' Dim dg = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.DownloadDiseases.DownloadDrug("G:\GCModeller\GCModeller\test\KEGG\dbget\drug_Dasatinib.html")

        '  Dim dis = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.DownloadDiseases.DownloadURL("G:\GCModeller\GCModeller\test\KEGG\dbget\disease-test.html")

        ' dis = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.DownloadDiseases.DownloadURL("G:\GCModeller\GCModeller\test\KEGG\dbget\Imatinib.html")


        Dim gbbb As GenBank.GBFF.File = GBFF.File.Load("G:\Xanthomonas_campestris_8004_uid15\genbank\CP000050.1.txt")


        Pause()

        WebServiceUtils.Proxy = "http://127.0.0.1:8087"

        Dim key = "D3068"
        Dim list = $"C:\Users\xieguigang\OneDrive\1.13-xcc\KEGG\{key}-meme.txt".ReadAllLines.GetKOlist("K:\Xanthomonas_campestris_8004_uid15\Ortholog")

        Call list.ToDictionary(Function(x) x.Name, Function(x) x.Value).GetJson(True).SaveTo($"C:\Users\xieguigang\OneDrive\1.13-xcc\KEGG\{key}-meme-KO.json")

        Dim htex = BriteHText.Load_ko00001.EnumerateEntries.Where(Function(x) Not x.entryID Is Nothing).GroupBy(Function(x) x.entryID).ToDictionary(Function(x) x.Key, Function(x) x.First)
        Dim pathways = From x In list
                       Where htex.ContainsKey(x.Value)
                       Let path = htex(x.Value)
                       Let subcate = path.parent
                       Let cate = subcate.parent
                       Let cls = cate.parent
                       Select geneID = x.Name, KO = x.Value, Category = cate.description, [class] = cls.description, subCatalog = subcate.description, [function] = path.description

        '  Call pathways.ToArray.SaveTo($"C:\Users\xieguigang\OneDrive\1.13-xcc\KEGG\{key}-meme-KO.csv")

        Call list.Reconstruct(work:=$"C:\Users\xieguigang\OneDrive\1.13-xcc\KEGG\{key}-meme/")


        Pause()

        '   Dim faaaaa = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.API.CutSequence(New Location(1434741, 1435203), "xcb")

        '  Call faaaaa.Save("G:\GCModeller\GCModeller\test\XC_1184 (-100).fasta")


        Pause()
        Dim sdfdsfssdddd = SMRUCC.genomics.Assembly.Uniprot.XML.UniProtXML.Load("G:\GCModeller\GCModeller\test\uniprotExample-one_entry.xml")

        '        Call SMRUCC.genomics.Assembly.Uniprot.Web.Retrieve_IDmapping.Mapping({"UniRef90_A0A0F8AYY8",
        '"UniRef90_A0A0F8APH3", "UniRef90_A0A1A8AV97",
        '        "UniRef90_I3ITW3",
        '        "UniRef90_A0A0U4TJT5",
        '        "UniRef90_M4ANX3",
        '        "UniRef90_F1RCJ4"}, IdTypes.NF90, IdTypes.ACC, "x:\sadasdas.gz").GetJson.debug

        Pause()

        Dim dddasdad = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.htext.ko00001

        Dim nt As New FastaSeq("H:\Xanthomonas_campestris_8004_uid15\CP000050.fna")

        Dim sss = nt.CutSequenceLinear(NucleotideLocation.Parse("1434841..1435203"))
        sss = nt.CutSequenceLinear(NucleotideLocation.Parse("complement(14113..14883)"))

        sss = nt.CutSequenceCircular(New NucleotideLocation(nt.Length - 5, nt.Length, True), NucleotideLocation.Parse("complement(1..6)"))


        Dim a As New SimpleSegment With {.Start = 1, .Ends = 10, .SequenceData = "1234567890", .Strand = "+"}
        Dim b As New SimpleSegment With {.Start = 5, .Ends = 8, .SequenceData = "5678", .Strand = "+"}
        Dim c As New SimpleSegment With {.Start = 6, .Ends = 13, .SequenceData = "67890abc", .Strand = "+"}
        Dim d As New SimpleSegment With {.Start = 11, .Ends = 15, .SequenceData = "abcde", .Strand = "+"}

        Dim assembl = {a, b, c, d}.SegmentAssembler

        Dim tax As New NcbiTaxonomyTree("G:\temp\NCBI_taxonomy_tree-master\nodes.dmp", "G:\temp\NCBI_taxonomy_tree-master\names.dmp")

        Call tax.GetParents({28384, 131567}).GetJson.debug
        Call tax.GetRank({28384, 131567}).GetJson.debug
        Call tax.GetChildren({28384, 131567}).GetJson.debug
        Call tax.GetName({28384, 131567}).GetJson.debug
        Call tax.GetAscendantsWithRanksAndNames({1, 562}).GetJson.debug
        Call tax.GetAscendantsWithRanksAndNames({562}, True).GetJson.debug
        Call tax.GetDescendants(208962, 566).GetJson.debug
        Call tax.GetDescendantsWithRanksAndNames(566).GetJson.debug
        ' Call tax.GetLeaves(1).Length.debug
        Call tax.GetLeaves(561).Length.debug
        Call tax.GetLeavesWithRanksAndNames(561) '.GetJson.debug
        Call tax.GetTaxidsAtRank("superkingdom").GetJson.debug

        Dim ptt As PTT = TabularFormat.PTT.Load("G:\Xanthomonas_campestris_8004_uid15\CP000050.ptt")
        Dim loci As New NucleotideLocation(3769223, 3769149, Strands.Reverse)
        Dim genome As New ContextModel.GenomeContextProvider(Of GeneBrief)(ptt)

        loci = New NucleotideLocation(1693322, 1693314, Strands.Unknown)

        Dim rel22222223 = genome.GetAroundRelated(loci, False)


        Dim rellllll = genome.GetAroundRelated(loci)


        loci = New NucleotideLocation(3834400, 3834450) ' XC_3200, XC_3199, KEGG测试成功

        rellllll = genome.GetAroundRelated(loci, False)

        ' 3777599          ==> 3779884 #Forward
        '        3779678 ==> 3779822 #Forward

        '                  3773960, 3775024
        ' 3773579, 3773650

        Dim ff As New ContextModel.Context(New NucleotideLocation(3769097, 3769702, Strands.Forward), 500)

        Dim relsss = ff.GetRelation(loci, True)

        '    Dim gff = LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.GFF.LoadDocument("D:\Xanthomonas\Xanthomonas citri pv. citri 306\GCA_000007165.1_ASM716v1_genomic.gff")
        '   Dim all_CDS = New GFF(gff, Features.CDS)

        '       Call Language.UnixBash.LinuxRunHelper.PerlShell()

        '   Dim ddddd = LANS.SystemsBiology.Assembly.KEGG.Archives.Csv.Pathway.LoadData("F:\GCModeller.Core\Downloads\Xanthomonas_oryzae_oryzicola_BLS256_uid16740", "xor")

        '  Call ddddd.SaveTo("F:\GCModeller.Core\Downloads\Xanthomonas_oryzae_oryzicola_BLS256_uid16740/xor.Csv")

        ' Dim alllll = KEGG.DBGET.LinkDB.Pathways.AllEntries("xcb").ToArray
        '    Dim pwys = KEGG.DBGET.LinkDB.Pathways.Downloads("xcb", "F:\GCModeller.Core\Downloads\Xanthomonas_campestris_8004_uid15").ToArray

        Dim s = KEGG.DBGET.bGetObject.Organism.GetKEGGSpeciesCode("Agrobacterium tumefaciens str. C58 (Cereon)")


        Dim compound As Compounds = Compounds.LoadCompoundsData("G:\1.13.RegPrecise_network\FBA\xcam314565\19.0\data\compounds.dat")


        'Dim rxn = KEGG.DBGET.bGetObject.Reaction.DownloadFrom("http://www.genome.jp/dbget-bin/www_bget?rn:R00086")
        'Dim modelssss = rxn.ReactionModel

        'Call rxn.SaveAsXml("x:\safsdsdfsd____rxn.xml")


        Dim model = CompilerAPI.Compile("F:\1.13.RegPrecise_network\Cellular Phenotypes\KEGG_Pathways", "F:\1.13.RegPrecise_network\Cellular Phenotypes\KEGG_Modules", "F:\GCModeller\KEGG\Reactions", "xcb")
        Call model.SaveAsXml("x:\dfsasdfsdf.kegg.xml")

        Dim rxns = FileIO.FileSystem.GetFiles("F:\GCModeller\KEGG\Reactions", FileIO.SearchOption.SearchAllSubDirectories, "*.xml").Select(Function(x) x.LoadXml(Of KEGG.DBGET.bGetObject.Reaction)).ToArray



        'Dim gff = LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature.GFF.LoadDocument("E:\xcb_vcell\Xanthomonas_campestris_8004_uid15\CP000050.gff3")

        'Dim gff33333 = LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature.GFF.LoadDocument("E:\Desktop\DESeq\Xcc8004.gff")

        'Dim ptt = LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.PTT.Load("E:\xcb_vcell\Xanthomonas_campestris_8004_uid15\CP000050.ptt")

        ''修改之前的数据为 Inside the [XC_2906] gene ORF.
        'Dim r As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.SegmentRelationships
        'Dim dataffff = LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.ComponentModels.GetRelatedGenes(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.ComponentModels.GeneBrief)(ptt, 3491357, 3491377, r)
        'dataffff = LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.ComponentModels.GetRelatedGenes(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.ComponentModels.GeneBrief)(ptt, 2874066, 874095, r, 1000)

        'Call LANS.SystemsBiology.Assembly.Uniprot.UniprotFasta.LoadFasta("E:\BLAST\db\uniprot_sprot.fasta")

        'Dim nfff = LANS.SystemsBiology.Assembly.KEGG.DBGET.ReferenceMap.ReferenceMapData.Download("map00010")


        'Dim gbk = LANS.SystemsBiology.Assembly.NCBI.GenBank.File.Read("E:\Desktop\xoc_vcell\plasmid\ncbi\FP340277.1.gbk")

        'Call LANS.SystemsBiology.Assembly.NCBI.GenBank.InvokeExport(gbk, Nothing)

        'Dim query = New LANS.SystemsBiology.Assembly.NCBI.Entrez.QueryHandler("Xanthomonas")
        'Dim list = query.DownloadCurrentPage
        'Dim n = list.First.DownloadGBK("x:\")
    End Sub
End Module
