#Region "Microsoft.VisualBasic::81feb4d0cb32391838a98c3124844bfb, CLI_tools\S.M.A.R.T\CLI\CLI.vb"

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
    '     Function: BuildCache, DumpOGEE, ExportPfamString, FetchDEG, FetchGeptop
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.VFDB
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.DEG
Imports SMRUCC.genomics.Data.DEG.OGEE
Imports SMRUCC.genomics.Data.DEG.OGEE.Models
Imports SMRUCC.genomics.Data.DEG.Web
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.Interops
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.SequenceModel.FASTA

<Package("SMATRT.CLI",
                  Category:=APICategories.CLI_MAN,
                  Publisher:="xie.guigang@gcmodeller.org, amethyst.asuka@gcmodeller.org",
                  Description:="SMART protein domain structure tools CLI interface.")>
<Cite(Title:="SMART 5: domains in the context of genomes and networks", Year:=2006, Volume:=34, Issue:="Database issue",
      ISSN:="1362-4962 (Electronic)
0305-1048 (Linking)", DOI:="10.1093/nar/gkj079", Keywords:="Catalysis
Catalytic Domain
*Databases, Protein
*Genomics
Internet
Models, Biological
Multiprotein Complexes/metabolism
*Protein Structure, Tertiary/genetics
Sequence Alignment
Sequence Analysis, Protein
User-Computer Interface",
      Authors:="Letunic, I.
Copley, R. R.
Pils, B.
Pinkert, S.
Schultz, J.
Bork, P.",
      Abstract:="The Simple Modular Architecture Research Tool (SMART) is an online resource (http://smart.embl.de/) used for protein domain identification and the analysis of protein domain architectures. 
Many new features were implemented to make SMART more accessible to scientists from different fields. The new 'Genomic' mode in SMART makes it easy to analyze domain architectures in completely sequenced genomes. 
Domain annotation has been updated with a detailed taxonomic breakdown and a prediction of the catalytic activity for 50 SMART domains is now available, based on the presence of essential amino acids. Furthermore, intrinsically disordered protein regions can be identified and displayed. 
The network context is now displayed in the results page for more than 350 000 proteins, enabling easy analyses of domain interactions.",
      AuthorAddress:="EMBL, Meyerhofstrasse 1, 69012 Heidelberg, Germany.",
      Journal:="Nucleic Acids Res", PubMed:=16381859, Pages:="D257-60")>
Public Module CLI

    <ExportAPI("/fetch.DEG")>
    <Usage("/fetch.DEG [/vfd <setB.fasta> /save <directory>]")>
    Public Function FetchDEG(args As CommandLine) As Integer
        Dim save$ = args("/save") Or "./essentialgenes/"
        Dim vfGenomes = FastaFile.Read(args("/vfd"), strict:=False) _
            .Select(Function(fa) FastaHeader.ParseHeader(fa)) _
            .BuildVFDIndex

        ' 下载数据
        Call WebParser.ParserWorkflow(save)

        ' 然后建立表格
        Using ntTable As New WriteStream(Of SeqRef)($"{save}/nt.csv"),
              prTable As New WriteStream(Of SeqRef)($"{save}/prot.csv")

            Dim nt As New FastaFile
            Dim pro As New FastaFile

            For Each genomeXml As String In save.EnumerateFiles("*.Xml")
                Dim genome As Genome = genomeXml.LoadXml(Of Genome)
                Dim vfGenome As Index(Of String)

                If vfGenomes.ContainsKey(genome.Organism) Then
                    vfGenome = vfGenomes(genome.Organism)
                Else
                    vfGenome = New Index(Of String)
                End If

                For Each gene As EssentialGene In genome.AsEnumerable
                    Dim ref As New SeqRef With {
                        .fullName = gene.FunctionDescrib,
                        .geneName = gene.Name,
                        .ID = gene.ID,
                        .Organism = genome.Organism,
                        .Reference = gene.Reference,
                        .Xref = gene.UniProt
                    }

                    If gene.Name Like vfGenome Then
                        ref.isVirulence = True
                    End If

                    ref.SequenceData = gene.Nt
                    ntTable.Flush(ref)
                    ref.SequenceData = gene.Aa
                    prTable.Flush(ref)

                    If gene.Nt <> "Not available now." Then
                        nt.Add(New FastaSeq With {.SequenceData = gene.Nt, .Headers = {$"{gene.ID} {gene.FunctionDescrib}"}})
                        pro.Add(New FastaSeq With {.SequenceData = gene.Aa, .Headers = {$"{gene.ID} {gene.FunctionDescrib}"}})
                    End If
                Next
            Next

            Call nt.Save($"{save}/essentialgenes.nt.fasta")
            Call pro.Save($"{save}/essentialgenes.aa.fasta")
        End Using

        Return 0
    End Function

    <ExportAPI("/fetch.geptop")>
    <Usage("/fetch.geptop [/vfd <setB.fasta> /save <directory>]")>
    Public Function FetchGeptop(args As CommandLine) As Integer
        Dim save$ = args("/save") Or "./geptop/"
        Dim vfGenomes = FastaFile.Read(args("/vfd")) _
            .Select(Function(fa) FastaHeader.ParseHeader(fa)) _
            .BuildVFDIndex

        Call SMRUCC.genomics.Data.Geptop.FetchData(save)

        ' 然后建立表格
        Using ntTable As New WriteStream(Of SeqRef)($"{save}/nt.csv"),
              prTable As New WriteStream(Of SeqRef)($"{save}/prot.csv")

            For Each genomeXml As String In save.EnumerateFiles("*.Xml")
                Dim genome As Geptop = genomeXml.LoadXml(Of Geptop)
                Dim vfGenome As Index(Of String)

                If vfGenomes.ContainsKey(genome.genome) Then
                    vfGenome = vfGenomes(genome.genome)
                Else
                    vfGenome = New Index(Of String)
                End If

                For Each gene As Geptop.Gene In genome.AsEnumerable
                    Dim ref As New SeqRef With {
                        .fullName = "",
                        .geneName = "",
                        .ID = gene.protein,
                        .Organism = genome.genome,
                        .Reference = "",
                        .Xref = gene.protein
                    }

                    'If gene.Name Like vfGenome Then
                    '    ref.isVirulence = True
                    'End If

                    ' ref.SequenceData = gene.Nt
                    ntTable.Flush(ref)
                    ' ref.SequenceData = gene.Aa
                    prTable.Flush(ref)
                Next
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/dump.ogee")>
    <Usage("/dump.ogee /in <data.directory> [/vfd <setB.fasta> /save <directory>]")>
    Public Function DumpOGEE(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim save$ = args("/save") Or $"{[in].TrimDIR}/ogee/"
        Dim vfGenomes = FastaFile.Read(args("/vfd")) _
            .Select(Function(fa) FastaHeader.ParseHeader(fa)) _
            .BuildVFDIndex
        Dim dataset = OGEE _
            .LoadGenes($"{in$}/genes.txt") _
            .Join(OGEE.LoadDataSet($"{in$}/datasets.txt"),
                  OGEE.LoadGeneSet($"{in$}/gene_essentiality.txt", False)
           )

        ' 然后建立表格
        Using ntTable As New WriteStream(Of SeqRef)($"{save}/nt.csv"),
              prTable As New WriteStream(Of SeqRef)($"{save}/prot.csv")

            For Each gene As geneSetInfo In dataset
                Dim ref As New SeqRef With {
                    .fullName = gene.gene.description,
                    .geneName = gene.gene.symbols,
                    .ID = gene.gene.locus,
                    .Organism = gene.dataset.sciName,
                    .Reference = gene.dataset.url,
                    .Xref = gene.gene.locus
                }

                Dim vfGenome As Index(Of String)

                If vfGenomes.ContainsKey(ref.Organism) Then
                    vfGenome = vfGenomes(ref.Organism)
                Else
                    vfGenome = New Index(Of String)
                End If

                If ref.geneName Like vfGenome Then
                    ref.isVirulence = True
                End If

                ref.SequenceData = gene.gene.codingSeq
                ntTable.Flush(ref)
                ref.SequenceData = gene.gene.proteinSeq
                prTable.Flush(ref)
            Next
        End Using

        Return 0
    End Function

    '    <Command("analysis", info:="",
    '        usage:="analysis -i <input_protein_fasta_file> -d <database_list> -o <output_result> [-e <e-value>]",
    '        example:="")>
    '    Public Shared Function SmartAnalysis(CommandLine As CommandLine) As Integer
    '        'Dim File As String = CommandLine("-i")
    '        'Dim Database As String = CommandLine("-d")
    '        'Dim Output As String = CommandLine("-o")
    '        'Dim E As String = CommandLine("-e")

    '        'If String.IsNullOrEmpty(E) Then
    '        '    E = "1e-3"
    '        'End If

    '        'Dim 

    '        'Using ModularArchitecture As ModularArchitecture = New ModularArchitecture(New NCBI.Extensions.LocalBLAST.InteropService.InitializeParameter(Program.Settings.BlastBin, Program.Settings.BlastApplication), Program.Settings.BlastDb, Global.Settings.TEMP)
    '        '    Dim FsaFile As SequenceModel.FASTA.File = SequenceModel.FASTA.File.Read(File)
    '        '    For Each fsa In FsaFile
    '        '        Dim Result = ModularArchitecture.Analysis(fsa, DbListOrDbFile:=Database, E:=E)
    '        '        Dim DomainXml = Result.GetXml
    '        '        Call FileIO.FileSystem.WriteAllText(Output, DomainXml, append:=False)
    '        '    Next
    '        'End Using

    '        'Return 0
    '    End Function

    ''' <summary>
    ''' 构建一个蛋白质序列数据库的蛋白质结构域缓存
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-build_cache",
               Usage:="-build_cache -i <fsa_file> [-o <export_file> -db <cdd_db_name> -cdd <cdd_db_path> -grep_script <script>]")>
    <ArgumentAttribute("-cdd", True,
                   Description:="The cdd database directory, if this switch value is null then system will using the default position in the profile file.")>
    Public Function BuildCache(args As CommandLine) As Integer
        Dim LocalBLAST = NCBI.Extensions.LocalBLAST.InteropService.CreateInstance(
            New NCBI.Extensions.LocalBLAST.InteropService.InitializeParameter(
                Settings.SettingsFile.BlastBin,
                NCBI.Extensions.LocalBLAST.InteropService.Program.LocalBlast))
        Dim Cdd As String = If(String.IsNullOrEmpty(args("-cdd")), Settings.SettingsFile.SMART.CDD, args("-cdd"))
        Dim CompileDomains = New CompileDomains(LocalBLAST, New CDDLoader(Cdd), Settings.TEMP)
        Dim CacheData = CompileDomains.Performance(args("-i"), args("-grep_script"), Settings.DataCache)
        Dim ExportSaved As String = args("-o")

        If Not String.IsNullOrEmpty(ExportSaved) Then
            Call (CacheData).LoadXml(Of SMARTDB)().Export.Save(ExportSaved, False)
        End If
        Return 0
    End Function

    '    <Command("query", info:="The query result will be save to a user specific directory with the query id as filename.",
    '        usage:="query -i <fasta_file> -q <query_id_list> -o <output_dir> [-e <E-value> -s <grep_script> -m <any/all>]")>
    '    <ParameterDescription("-q", example:="pfam00001,pfam00002")>
    '    Public Shared Function Query(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
    '        Dim File As String = CommandLine("-i")
    '        Dim Ids As String = CommandLine("-q")
    '        Dim OutputDir As String = CommandLine("-o")
    '        Dim E As String = CommandLine("-e")

    '        If String.IsNullOrEmpty(File) OrElse Not FileIO.FileSystem.FileExists(File) Then
    '            Call Printf("The specific file ""%s"" is not exists!", File)
    '            Return -1
    '        End If

    '        If String.IsNullOrEmpty(Ids) Then
    '            Call Printf("Not specific the id list parameter, unable to load domain information!")
    '            Return -2
    '        End If

    '        If String.IsNullOrEmpty(OutputDir) Then
    '            OutputDir = My.Computer.FileSystem.SpecialDirectories.Desktop
    '            Call Printf("Set up the output directory to user desktop directory:\n  ""%s"".", OutputDir)
    '        End If

    '        Dim QueryObject = New DomainQuery(Program.Settings.BlastDb, Program.GetBLASTInitParameter, Global.Settings.TEMP, OutputDir)
    '        Call QueryObject.SetupGrepMethods(CommandLine("-s"))
    '        Call QueryObject.Query(File, Ids.Split(","), E, String.Equals(CommandLine("-m"), "all"))

    '        Return 0
    '    End Function
    'End Class

    'Public Class QueryResult
    '    <XmlAttribute> Public Id As String, Length As Integer

    <ExportAPI("--Export.Pfam-String", Usage:="--Export.Pfam-String /in <blast_out.txt>")>
    Public Function ExportPfamString(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim blastOut = BlastPlus.Parser.TryParse(inFile)
        Dim PfamString = Pfam.CreatePfamString(blastOut, disableUltralarge:=True)
        Return PfamString.SaveTo(inFile.TrimSuffix & ".Pfam-String.Csv")
    End Function

End Module
