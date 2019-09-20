#Region "Microsoft.VisualBasic::16174964213d2c8377b9559d5ec7087f, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\R\DESeq\DESeq.vb"

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

    '     Module DESeq
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __runR, (+3 Overloads) DESeq2, DiffGeneCOGs, FilterDifferentExpression, HTSeqCount
    '                   Initialize, LoadPTT, SaveResult, summarizeOverlaps
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.API.base
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.RNA_Seq.BOW

Namespace DESeq2

    ''' <summary>
    ''' Differential analysis of count data – the DESeq2 package
    ''' (M. I. Love, W. Huber, S. Anders: Moderated estimation of fold change And dispersion For RNA-Seq data With DESeq2. bioRxiv(2014).doi : 10.1101/002832)
    ''' 
    ''' A basic task in the analysis of count data from RNA-Seq is the detection of differentially
    ''' expressed genes. The count data are presented As a table which reports, For Each sample, the
    ''' number Of sequence fragments that have been assigned To Each gene. Analogous data also arise
    ''' 
    ''' For other assay types, including comparative ChIP-Seq, HiC, shRNA screening, mass spectrometry.
    ''' An important analysis question Is the quantification And statistical inference Of systematic changes
    ''' between conditions, as compared To within-condition variability. The package DESeq2 provides
    ''' methods To test For differential expression by use Of negative binomial generalized linear models;
    ''' the estimates Of dispersion And logarithmic fold changes incorporate data-driven prior distributions.
    ''' 
    ''' This vignette explains the use of the package And demonstrates typical work flows. 
    ''' Another vignette, “Beginner’s guide to using the DESeq2 package”, covers similar material but at a slower
    ''' pace, including the generation Of count tables from FASTQ files.
    ''' </summary>
    ''' <remarks>
    ''' Welcome to 'DESeq'. For improved performance, usability and functionality, please consider migrating to 'DESeq2'.
    ''' 虽然模块的名称是DESeq，但是在R之中实际调用的包确是DESeq2
    ''' </remarks>
    <Package("DESeq", Description:="Differential analysis of count data – the DESeq2 package
(M. I. Love, W. Huber, S. Anders: Moderated estimation of fold change And dispersion For RNA-Seq data With DESeq2. bioRxiv(2014).doi : 10.1101/002832)
<br />
<p>
A basic task in the analysis of count data from RNA-Seq is the detection of differentially
expressed genes. The count data are presented As a table which reports, For Each sample, the
number Of sequence fragments that have been assigned To Each gene. Analogous data also arise
<br />
For other assay types, including comparative ChIP-Seq, HiC, shRNA screening, mass spectrometry.
An important analysis question Is the quantification And statistical inference Of systematic changes
between conditions, as compared To within-condition variability. The package DESeq2 provides
methods To test For differential expression by use Of negative binomial generalized linear models;
the estimates Of dispersion And logarithmic fold changes incorporate data-driven prior distributions.
<br />
This vignette explains the use of the package And demonstrates typical work flows. 
Another vignette, “"Beginner's guide to using the DESeq2 package"”, covers similar material but at a slower
pace, including the generation Of count tables from FASTQ files.
</p>",
                        Publisher:="",
                        Cites:="Anders, S. and W. Huber (2010). ""Differential expression analysis For sequence count data."" Genome Biol 11(10): R106.
<p>	High-throughput sequencing assays such as RNA-Seq, ChIP-Seq or barcode counting provide quantitative readouts in the form of count data. To infer differential signal in such data correctly and with good statistical power, estimation of data variability throughout the dynamic range and a suitable error model are required. We propose a method based on the negative binomial distribution, with variance and mean linked by local regression and present an implementation, DESeq, as an R/Bioconductor package.

")>
    <Cite(Title:="Differential expression analysis for sequence count data", Year:=2010, Volume:=11, Issue:="10", Pages:="R106",
          Keywords:="Animals
Binomial Distribution
Chromatin Immunoprecipitation/methods
Computational Biology/*methods
Drosophila/genetics
Gene Expression Profiling/*methods
High-Throughput Nucleotide Sequencing/methods
Linear Models
Models, Genetic
Saccharomyces cerevisiae/genetics
Sequence Analysis, RNA/*methods
Stem Cells
Tissue Culture Techniques",
          Abstract:="High-throughput sequencing assays such as RNA-Seq, ChIP-Seq or barcode counting provide quantitative readouts in the form of count data. 
To infer differential signal in such data correctly and with good statistical power, estimation of data variability throughout the dynamic range and a suitable error model are required. 
We propose a method based on the negative binomial distribution, with variance and mean linked by local regression and present an implementation, DESeq, as an R/Bioconductor package.",
          AuthorAddress:="European Molecular Biology Laboratory, Mayerhofstrasse 1, 69117 Heidelberg, Germany. sanders@fs.tum.de",
          Authors:="Anders, S.
Huber, W.",
          DOI:="10.1186/gb-2010-11-10-r106",
          ISSN:="1474-760X (Electronic)
1474-7596 (Linking)",
          Journal:="Genome biology",
          PubMed:=20979621)>
    Public Module DESeq

        Sub New()
            Call Settings.Session.Initialize()
        End Sub

        ''' <summary>
        ''' 导出差异表达的基因
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Genes'DifferentExpr")>
        Public Function FilterDifferentExpression(result As String, Optional log2Fold As Double = 2) As String()
            Dim LQuery = (From Gene In result.LoadCsv(Of DESeq2Diff)(False).AsParallel
                          Where Math.Abs(Gene.log2FoldChange) >= log2Fold
                          Select Gene.locus_tag).ToArray
            Return LQuery
        End Function

        <ExportAPI("DESeq.COGs")>
        Public Function DiffGeneCOGs(result As String, PTT As PTT, Optional log2Fold As Double = 2, Optional IdenticalLog2Folds As Double = 1) As DESeqCOGs()
            Dim DiffCsv = IO.File.Load(result)
            Dim Diff = DiffCsv.AsDataSource(Of DESeq2Diff)(False)
            Dim Up = (From Gene In Diff.AsParallel
                      Where Gene.log2FoldChange >= log2Fold
                      Select Gene.locus_tag,
                          COG = PTT.GeneObject(Gene.locus_tag).COG.GetCOGCategory).ToArray
            log2Fold *= -1
            Dim Down = (From Gene In Diff.AsParallel
                        Where Gene.log2FoldChange <= log2Fold
                        Select Gene.locus_tag,
                            COG = PTT.GeneObject(Gene.locus_tag).COG.GetCOGCategory).ToArray
            Dim ExpressRanks = (From Gene In Diff Select Gene.baseMean).ToArray.Log2Ranks(100)  '首先做映射因为需要保持顺序，所以这里不可以使用并行化
            Dim IdenticalsRanks = (From i As Integer In Diff.Sequence.AsParallel
                                   Let Gene As DESeq2Diff = Diff(i)
                                   Where Gene.log2FoldChange <= IdenticalLog2Folds AndAlso Gene.log2FoldChange >= -IdenticalLog2Folds
                                   Select Gene, Ranks = ExpressRanks(i)).ToArray
            Dim InvariantLow = (From Gene In IdenticalsRanks
                                Where Gene.Ranks <= 20
                                Select Gene.Gene.locus_tag, COG = PTT.GeneObject(Gene.Gene.locus_tag).COG.GetCOGCategory).ToArray
            Dim InvariantHigh = (From Gene In IdenticalsRanks
                                 Where Gene.Ranks >= 80
                                 Select Gene.Gene.locus_tag, COG = PTT.GeneObject(Gene.Gene.locus_tag).COG.GetCOGCategory).ToArray

            Dim ResultView As New List(Of DESeqCOGs)

            For Each cat As NCBI.COG.Catalog In NCBI.COG.Function.Default.Catalogs
                For Each COG In cat.SubClasses
                    Dim View As New DESeqCOGs With {
                        .Category = cat.Class,
                        .CategoryDescription = cat.Description,
                        .COG = COG.Key,
                        .COGDescription = $"[{COG.Key}] {COG.Value}"
                    }

                    View.DiffUp = (From Gene In Up Where InStr(Gene.COG, COG.Key) > 0 Select Gene.locus_tag).ToArray
                    View.DiffDown = (From Gene In Down Where InStr(Gene.COG, COG.Key) > 0 Select Gene.locus_tag).ToArray
                    View.IdenticalLow = (From Gene In InvariantLow Where InStr(Gene.COG, COG.Key) > 0 Select Gene.locus_tag).ToArray
                    View.IdenticalHigh = (From Gene In InvariantHigh Where InStr(Gene.COG, COG.Key) > 0 Select Gene.locus_tag).ToArray

                    Call ResultView.Add(View)
                    Call Console.Write(".")
                Next
            Next

            Return ResultView.ToArray
        End Function

        <ExportAPI("Read.PTT")>
        Public Function LoadPTT(path As String) As PTT
            Return PTT.Load(path)
        End Function

        <ExportAPI("Write.Csv.DESeq2.COGs")>
        Public Function SaveResult(data As IEnumerable(Of DESeqCOGs), <Parameter("Path.Save")> SaveTo As String) As Boolean
            Return data.SaveTo(SaveTo, False)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="R_HOME"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' For counting aligned reads in genes, the summarizeOverlaps function of GenomicAlignments 
        ''' With mode="Union" Is encouraged, resulting In a SummarizedExperiment Object(easyRNASeq Is 
        ''' another Bioconductor package which can prepare SummarizedExperiment objects as input for DESeq2).
        ''' 
        ''' 为了计数基因的比对Reads数目， GenomicAlignments 包之中的summarizeOverlaps方法可以生成DESeq2所需要用到的数据
        ''' easyRNASeq包也可以用来生成这种数据
        ''' </remarks>
        <ExportAPI("init()", Info:="Initis the R session for the Differential analysis.")>
        Public Function Initialize(<Parameter("R_HOME",
                                              "The R program install location on your computer.")>
                                   Optional R_HOME As String = "") As Boolean

            Try
                If Not String.IsNullOrEmpty(R_HOME) Then
                    Call TryInit(R_HOME)
                End If

                Call library("rtracklayer")
                Call library("GenomicAlignments")
                Call library("DESeq2")

                Return True
            Catch ex As Exception
                ex = New Exception("R_HOME:  " & R_HOME, ex)
                Call App.LogException(ex)
                Call ex.PrintException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' 使用这个函数进行分析
        ''' </summary>
        ''' <param name="sampleTable"></param>
        ''' <param name="directory"></param>
        ''' <param name="design"></param>
        ''' <returns></returns>
        <ExportAPI("DESeq2")>
        Public Function DESeq2(<Parameter("dataExpr0",
                                          "For htseq-count: a data.frame with three or more columns. Each row describes one sample. " &
                                          "The first column is the sample name, the second column the file name of the count file generated by htseq-count, and the remaining columns are sample metadata which will be stored in colData")>
                               sampleTable As IO.File,
                               <Parameter("directory", "for htseq-count: the directory relative to which the filenames are specified")>
                               directory As String,
                               <Parameter("design",
                                          "A formula which specifies the design of the experiment, taking the form formula(~ x + y + z). " &
                                          "By default, the functions in this package will use the last variable in the formula (e.g. z) for presenting results (fold changes, etc.) and plotting.")>
                               design As String,
                               PTT As PTT) As Boolean

            Dim SampleTables = sampleTable.AsDataSource(Of SampleTable)(False)  '为了保持顺序，这里不再使用并行化拓展
            Dim countDataTable = HtseqCountMethod.DataFrame((From row As SampleTable
                                                             In SampleTables
                                                             Select $"{directory}\{row.fileName}").ToArray, PTT, False)
            Dim countData As String = directory & "\countData.csv"
            Dim sbr As StringBuilder = New StringBuilder(My.Resources.DEseq2_Template)
            Call sbr.Replace("{countData.MAT.csv}", countData.Replace("\", "/"))
            Call countDataTable.Save(countData, False)
            '# ({conditionName} <- factor(c({factors})))
            '{WRITE_FACTOR_CONDITIONS}
            Dim Conditions = (From column As String() In sampleTable.Columns.Skip(2)   '遍历每一个Factor，取得矩阵之中的每一列的数据作为DataFrame之中的一列
                              Let Factor As String = column(Scan0)
                              Select Factor, init = $"{Factor} <- factor(c({String.Join(", ", (From s In column.Skip(1) Select """" & s & """").ToArray)}))").ToArray
            Call sbr.Replace("{WRITE_FACTOR_CONDITIONS}", String.Join(vbCrLf, Conditions.Select(Of String)(Function(obj) obj.init)))
            Call sbr.Replace("{FACTOR_LIST}", String.Join(", ", Conditions.Select(Of String)(Function(obj) obj.Factor)))
            Call sbr.Replace("{Condition-Design}", design)
            Call sbr.Replace("{ColumnNames}", String.Join(", ", SampleTables.Select(Of String)(Function(obj) """" & obj.sampleName & """")))
            Call sbr.Replace("{DIR_EXPORT}", FileIO.FileSystem.GetParentPath(countData).Replace("\", "/"))
            Call sbr.Replace("{ConditionLength}", Conditions.Length)

            Dim path$ = Nothing
            Dim DiffCsv As IO.File = __runR(sbr.ToString, directory, path)
            Return DiffCsv.Save(path)
        End Function

        Private Function __runR(RScript As String, directory As String, ByRef diffPath$) As IO.File
            Dim ScriptPath As String = $"{directory}/R.DESeq.txt"

            Call RScript.SaveTo(ScriptPath)
            Call $"Script file saved to {ScriptPath.ToFileURL}...".__DEBUG_ECHO
            Call "Start running R analysis....".__DEBUG_ECHO

            Try
                SyncLock R
                    With R
                        Call .WriteLine(RScript).JoinBy(vbCrLf).__DEBUG_ECHO
                    End With
                End SyncLock
            Catch ex As Exception
                ex = New Exception(RScript, ex)
                Call ex.PrintException
                Call base.warning().JoinBy(vbCrLf).__DEBUG_ECHO
            End Try

            diffPath = directory & "/diffexpr-results.csv"

            If Not diffPath.FileExists Then
                Call "DESeq2 analysis run failure!".__DEBUG_ECHO
                Return False
            End If

            '替换回基因编号
            Dim ID As String() = IO.File.Load(directory & "\countData.csv").Column(0).Skip(1).ToArray
            Dim DiffCsv As IO.File = IO.File.Load(diffPath)

            DiffCsv(0)(1) = "locus_tag"  ' Gene -> locus_tag

            For i As Integer = 1 To ID.Length
                Dim row As IO.RowObject = DiffCsv(i)
                row(1) = ID(i - 1)
            Next

            Return DiffCsv
        End Function

        ''' <summary>
        ''' DESeqDataSetFromMatrix(countData, colData, design, ignoreRank = FALSE, ...)
        ''' </summary>
        ''' <param name="Experiments">
        ''' {column, experiment}, {column, experiment}
        ''' {80041, NY}, {80042, MMX}
        ''' </param>
        ''' <param name="Factors">Column name of the experiments variables</param>
        ''' <param name="countData">htseq-count::Data.Frame, For matrix input: A matrix of non-negative integers</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("DESeq2", Info:="DESeqDataSetFromMatrix(countData, colData, design, ignoreRank = FALSE, ...)")>
        Public Function DESeq2(<Parameter("Count.Data",
                                          "htseq-count::Data.Frame, For matrix input: A matrix of non-negative integers")>
                               countData As String,
                               <Parameter("Factors",
                                          "Column name of the experiments variables.")>
                               Factors As IEnumerable(Of String),
                               Experiments As IEnumerable(Of IEnumerable(Of String))) As Boolean
            Dim colDataMAT As New IO.File    ' For matrix input: a DataFrame or data.frame with at least a single column. Rows of colData correspond to columns of countData.
            Dim HeadRow = New IO.RowObject From {""}
            Call HeadRow.AddRange(Factors) '为了保持一一对应关系，在这里不再使用并行化拓展
            Call colDataMAT.Add(HeadRow)
            Call colDataMAT.AppendRange((From Experiment In Experiments Select New RowObject(Experiment)).ToArray)
            Dim colData As String = $"{FileIO.FileSystem.GetParentPath(countData)}/{BaseName(countData)}.colData.csv"
            Call colDataMAT.Save(colData, Encoding:=Encoding.ASCII)
            Dim ScriptBuilder As StringBuilder = New StringBuilder(My.Resources.DEseq2_Template)
            Call ScriptBuilder.Replace("{countData.MAT.csv}", countData.Replace("\", "/"))
            '# ({conditionName} <- factor(c({factors})))
            '{WRITE_FACTOR_CONDITIONS}
            Dim Conditions = (From i As Integer In Factors.Sequence '遍历每一个Factor，取得矩阵之中的每一列的数据作为DataFrame之中的一列
                              Let Factor As String = Factors(i)
                              Let value As String() = (From experiment In Experiments Select experiment(i + 1)).ToArray
                              Select $"{Factor} <- factor(c({String.Join(", ", (From s In value Select """" & s & """").ToArray)}))").ToArray
            Call ScriptBuilder.Replace("{WRITE_FACTOR_CONDITIONS}", String.Join(vbCrLf, Conditions))
            Call ScriptBuilder.Replace("{FACTOR_LIST}", String.Join(", ", Factors.ToArray))
            Call ScriptBuilder.Replace("{Condition-Design}", "~" & String.Join("+", Factors.ToArray))
            Call ScriptBuilder.Replace("{ColumnNames}", String.Join(", ", (From Expr In Experiments Select """" & Expr.First & """").ToArray))
            Call ScriptBuilder.Replace("{DIR_EXPORT}", FileIO.FileSystem.GetParentPath(countData).Replace("\", "/"))
            Call ScriptBuilder.Replace("{ConditionLength}", Factors.Count)

            Dim ScriptPath As String = $"{FileIO.FileSystem.GetParentPath(countData)}/{BaseName(countData)}.R.DESeq.txt"
            Call ScriptBuilder.SaveTo(ScriptPath)
            Call $"Script file saved to {ScriptPath.ToFileURL}...".__DEBUG_ECHO
            Call "Start running R analysis....".__DEBUG_ECHO

            Call TryInit(Settings.Session.GetSettingsFile.R_HOME)

            Try
                SyncLock R
                    With R
                        Call .WriteLine(ScriptBuilder.ToString) _
                                .JoinBy(vbCrLf) _
                                .__DEBUG_ECHO
                    End With
                End SyncLock
            Catch ex As Exception
                Call Console.WriteLine(ex.ToString)
            End Try

            Dim DiffPath As String = FileIO.FileSystem.GetParentPath(countData) & "/diffexpr-results.csv"
            If Not DiffPath.FileExists Then
                Return False
            End If

            '替换回基因编号
            Dim ID = IO.File.Load(countData).Column(0).Skip(1).ToArray
            Dim DiffCsv = IO.File.Load(DiffPath)

            For i As Integer = 1 To ID.Count
                DiffCsv(i)(1) = ID(i - 1)
            Next

            Call DiffCsv.Save(DiffPath)

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SAM"></param>
        ''' <param name="GFF"></param>
        ''' <param name="PairedEnd">假若是Paired-End的比对数据，则还需要首先使用samtools工具进行排序</param>
        ''' <returns></returns>
        <ExportAPI("HTSeq.Count", Info:="This function required of python, numpy and HTSeq installed on your computer.")>
        Public Function HTSeqCount(SAM As Value(Of String), GFF As String, Optional PairedEnd As Boolean = True) As Boolean
            'python -m HTSeq.scripts.count [options] <alignment_file> <gff_file>
            Call Settings.Session.Initialize()

            If Not Settings.Session.SettingsFile.Python.FileExists Then
                Call "The required python is not installed on your computer!".__DEBUG_ECHO
                Return False
            End If

            If PairedEnd Then
                '进行排序
                Dim Sorted As String = SAM.Value & ".ordered"
                Call $"The input Sam data file {SAM.Value.ToFileURL} is paired-end data, needs to be sorted.....".__DEBUG_ECHO
                Call "Start to sorting the sam file....".__DEBUG_ECHO

                '将sam转换为bam
                '根据gff文件查找*.fna基因组序列文件
                Dim GFF_ID As String = BaseName(GFF)
                Dim Fna As String = (From path As String
                                     In FileIO.FileSystem.GetFiles(FileIO.FileSystem.GetParentPath(GFF), FileIO.SearchOption.SearchTopLevelOnly, "*.fna")
                                     Let FNA_ID As String = BaseName(path)
                                     Where String.Equals(FNA_ID, GFF_ID, StringComparison.OrdinalIgnoreCase)
                                     Select path).FirstOrDefault
                If Not Fna.FileExists Then '找不到基因组序列文件
                    Call "Could not found the reference genome fasta file, could not indexing the Sam file for your paired-end data, function exit....".__DEBUG_ECHO
                    Return False
                End If

                Fna = Samtools.Indexing(Fna)
                Call Samtools.Import(SAM, Fna, SAM = (SAM.Value & ".bam"))
                Call Samtools.Sort(SAM, Sorted)
                Call $"Samtools sorts job done! Sorted file saved at {Sorted.ToFileURL}".__DEBUG_ECHO
                SAM = Sorted & ".bam"
            End If

            Dim Cli As String = If(PairedEnd,
                $"-m HTSeq.scripts.count -r name -f bam {SAM.Value.CLIPath} {GFF.CLIPath}",
                $"-m HTSeq.scripts.count {SAM.Value.CLIPath} {GFF.CLIPath}") 'bam文件还需要加一些开关来指定格式？
            Dim HTseq As IIORedirectAbstract = New IORedirectFile(Settings.Session.SettingsFile.Python, Cli)
            Dim i As Boolean = 0 = HTseq.Run()

            Call Console.WriteLine(HTseq.StandardOutput)

            Return i
        End Function

        ''' <summary>
        ''' HTSeq-Count
        ''' </summary>
        ''' <param name="TestData">
        ''' samFile, condition;
        ''' samFile, condition;
        ''' 
        ''' </param>
        ''' <returns></returns>
        <ExportAPI("DESeq2", Info:="How to use DESeq2 to analyse RNAseq.")>
        Public Function DESeq2(TestData As IEnumerable(Of Generic.IEnumerable(Of String)),
                               <Parameter("DIR.Export")> Optional Export As String = "./") As Boolean
            Dim RScript As StringBuilder = New StringBuilder(My.Resources.Invoke_DESeq2)
            Dim SamList As New List(Of String)
            Dim ConditionList As New List(Of String)

            For Each Line In TestData
                If Line.Count >= 2 Then
                    Call SamList.Add(Line(0))
                    Call ConditionList.Add(Line(1))
                End If
            Next

            Dim SourceDir As String = FileIO.FileSystem.GetFileInfo(SamList.First).Directory.FullName.Replace("\", "/")
            Dim SamFileList As String = String.Join(", ", (From path As String In SamList Select """" & FileIO.FileSystem.GetFileInfo(path).Name & """").ToArray)
            Dim Conditions As String = String.Join(", ", (From cd As String In ConditionList Select """" & cd & """").ToArray)
            Dim ConditionLvs As String = String.Join(", ", (From T As String In ConditionList.Distinct Select """" & T & """").ToArray)

            Export = FileIO.FileSystem.GetDirectoryInfo(Export).FullName.Replace("\", "/")

            Call RScript.Replace("<*.SAM_FILE_LIST>", SamFileList)
            Call RScript.Replace("<Conditions_Corresponding_TO_SampleFiles>", Conditions)
            Call RScript.Replace("<Condition_Levels>", ConditionLvs)
            Call RScript.Replace("<SAVED_RESULT_CSV>", Export & "/ddsHTSeq.csv")
            Call RScript.Replace("<MAplot.png>", Export & "/MAplot.png")
            Call RScript.Replace("<DIR.Source>", SourceDir)

            Call RScript.SaveTo(Export & "/Invoke_DESeq2.txt", Encoding.ASCII)

            SyncLock R
                With R
                    .call = RScript.ToString
                End With
            End SyncLock

            Return True
        End Function

        ''' <summary>
        ''' Counting reads with summarizeOverlaps, Perform overlap queries between reads and genomic features
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="annoGFF">GTF基因组注释文件的文件路径</param>
        ''' <param name="bamList">Mapping所得到的*.bam二进制文件的文件路径的列表</param>
        ''' <remarks>
        ''' 作者推荐使用这个函数方法的模式
        ''' 
        ''' ## mode funtions
        ''' Union(features, reads, ignore.strand=FALSE, inter.feature=TRUE)
        ''' </remarks>
        <ExportAPI("DEseq", Info:="GenomicAlignments::summarizeOverlaps  Counting reads with summarizeOverlaps")>
        Public Function summarizeOverlaps(<Parameter("List.Bam", "The file list of the binary format reads mapping file.")>
                                          bamList As IEnumerable(Of String),
                                          <Parameter("anno.gff", "The gff format genome annotation file, which can be achieved from the ncbi FTP in the same directory of ptt annotation file.")>
                                          annoGFF As String) As RDotNET.SymbolicExpression
            Dim GTF As String = annoGFF.Replace("\", "/")
            Dim anno As String =
            $"gffFile <- ""{GTF}"";
                 gff0 <- import(gffFile,format=""gff3"", version=""3"" , asRangedData=F);
                  idx <- mcols(gff0)$source == ""protein_coding"" & seqnames(gff0) == ""1"";
                  gff <- gff0[idx];

             ## adjust seqnames to match Bam files
       seqlevels(gff) <- paste(""chr"", seqlevels(gff), sep="""");
             chrgenes <- split(gff, mcols(gff)$gene_id);"  '生成基因组的注释数据

            Dim Script As String =
                $"bamlst <- c({ String.Join(", ", (From file As String In bamList Select """" & file.Replace("\", "/") & """").ToArray)});
                  bamlst <- BamFileList(bamlst);
                genehits <- summarizeOverlaps(chrgenes, bamlst, mode=""Union"");"

            Dim DESeq2R As String =
                "dds <- DESeqDataSet(se = se, design = ~ condition);
                 dds <- DESeq(dds);
                 res <- results(dds);
          resOrdered <- res[order(res$padj),];
          head(resOrdered);"

            Dim DebugScript = "library(rtracklayer)
library(GenomicAlignments)
                library(DESeq2)" &
                vbCrLf & anno & vbCrLf & Script & vbCrLf & DESeq2R

            Call DebugScript.SaveTo(Settings.DataCache & "/DEseq.r.txt")
            Call DebugScript.__DEBUG_ECHO

            SyncLock R
                With R
                    Try
                        .call = anno
                    Catch ex As Exception
                        Throw New Exception(anno & vbCrLf & vbCrLf & ex.ToString)
                    End Try

                    Try
                        .call = Script
                    Catch ex As Exception
                        Throw New Exception(Script & vbCrLf & vbCrLf & ex.ToString)
                    End Try

                    Try
                        .call = DESeq2R
                    Catch ex As Exception
                        Throw New Exception(DESeq2R & vbCrLf & vbCrLf & ex.ToString)
                    End Try

                    Dim result As RDotNET.SymbolicExpression = .GetSymbol("resOrdered")
                    Return result
                End With
            End SyncLock
        End Function
    End Module
End Namespace
