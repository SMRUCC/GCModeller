#Region "Microsoft.VisualBasic::9cb8b34f069898d2e1cbe1b695505cc8, ..\GCModeller\core\Bio.Assembly\Test.Project\DEBUG_MAIN.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Assembly

Module DEBUG_MAIN

    Public Sub ReadDatabase()

        Dim path As String = "/path/to/database/file"
        Dim gb As GBFF.File = GBFF.File.Load(path)
        Dim gbs As IEnumerable(Of GBFF.File) = GBFF.File.LoadDatabase(path)
        Dim PTT As PTT = PTT.Load(path)
        Dim GFF As GFF = GFF.LoadDocument(path)

        Dim Fasta As New FASTA.FastaFile(path)
        Dim nt As New FASTA.FastaToken(path)

        nt = FastaToken.Load(path)
        nt = FastaToken.LoadNucleotideData(path)
    End Sub

    Sub Main()

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

        Dim alllll = KEGG.DBGET.LinkDB.Pathways.AllEntries("xcb").ToArray
        Dim pwys = KEGG.DBGET.LinkDB.Pathways.Downloads("xcb", "F:\GCModeller.Core\Downloads\Xanthomonas_campestris_8004_uid15").ToArray

        Dim s = KEGG.DBGET.bGetObject.Organism.GetKEGGSpeciesCode("Agrobacterium tumefaciens str. C58 (Cereon)")


        Dim compound As Compounds = Compounds.LoadCompoundsData("G:\1.13.RegPrecise_network\FBA\xcam314565\19.0\data\compounds.dat")


        Dim rxn = bGetObject.Reaction.DownloadFrom("http://www.genome.jp/dbget-bin/www_bget?rn:R00086")
        Dim modelssss = rxn.ReactionModel

        Call rxn.SaveAsXml("x:\safsdsdfsd____rxn.xml")


        Dim model = CompilerAPI.Compile("F:\1.13.RegPrecise_network\Cellular Phenotypes\KEGG_Pathways", "F:\1.13.RegPrecise_network\Cellular Phenotypes\KEGG_Modules", "F:\GCModeller\KEGG\Reactions", "xcb")
        Call model.SaveAsXml("x:\dfsasdfsdf.kegg.xml")

        Dim rxns = FileIO.FileSystem.GetFiles("F:\GCModeller\KEGG\Reactions", FileIO.SearchOption.SearchAllSubDirectories, "*.xml").ToArray(Function(x) x.LoadXml(Of bGetObject.Reaction))



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
