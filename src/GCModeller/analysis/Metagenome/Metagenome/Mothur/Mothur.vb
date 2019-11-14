#Region "Microsoft.VisualBasic::b72c55475c90a838396830e064bb8560, analysis\Metagenome\Metagenome\Mothur\Mothur.vb"

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

    ' Class Mothur
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: align_seqs, Bin_seqs, Cluster, Count_seqs, CreateMothurApp
    '               Dist_seqs, (+2 Overloads) filter_seqs, GetOTUrep, (+2 Overloads) Make_contigs, RunMothur
    '               (+2 Overloads) Screen_seqs, (+2 Overloads) Summary_seqs, Unique_seqs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Environment
Imports System.Runtime.CompilerServices
Imports Darwinism.Docker
Imports Darwinism.Docker.Arguments
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.CommandLine.Reflection

''' <summary>
''' mothur的输入文件名之中不可以存在双引号
''' </summary>
''' <remarks>
''' Mothur在windows平台上面存在着一个内存问题,所以在这里是通过Docker在Centos上面运行Mothur的
''' </remarks>
Public Class Mothur

    ReadOnly docker As DockerAppDriver
    ReadOnly cli As InteropService

    ''' <summary>
    ''' Create a new mothur docker environment.(工作于Windows Server平台之上)
    ''' </summary>
    ''' <param name="container">The docker container image ID</param>
    ''' <param name="mount">
    ''' 因为Docker容器之中仅包含有应用程序，其他的样本原始数据或者序列数据库都需要通过
    ''' 这个函数参数所设置的共享文件夹来进行获取
    ''' </param>
    ''' <param name="home">
    ''' 通过``docker pull xieguigang/gcmodeller-env``得到的容器环境之中,
    ''' Mothur应用程序的默认位置为: ``/home/Mothur.linux_64``
    ''' </param>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(container As Image, mount As Mount, Optional home$ = "/home/Mothur.linux_64")
        docker = New DockerAppDriver(container, "mothur", mount, home)
    End Sub

    ''' <summary>
    ''' 如果GCModeller运行在Linux平台上,则应该调用这个接口
    ''' </summary>
    ''' <param name="app"></param>
    Sub New(app As String)
        If Not app.FileExists Then
            Dim platform$ = OSVersion.Platform.ToString
            Dim msg$ = $"{app} is unavaliable! (platform={platform})"

            Throw New EntryPointNotFoundException(msg)
        Else
            cli = New InteropService(app)
        End If
    End Sub

    Public Function CreateMothurApp() As Mothur
        If App.IsMicrosoftPlatform Then
            ' Linux in Docker
            Return New Mothur(Settings.Mothur, Nothing)
        Else
            ' Linux
            Return New Mothur(Settings.Mothur)
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function RunMothur(args As String) As String
        If Not docker Is Nothing Then
            Return docker.Shell($"""#{args};""")
        Else
            Return cli.RunProgram($"""#{args};""").StandardOutput
        End If
    End Function

    ''' <summary>
    ''' The summary.seqs command will summarize the quality of sequences in an unaligned or aligned fasta-formatted sequence file.
    ''' </summary>
    ''' <param name="fasta"></param>
    ''' <returns></returns>
    Public Function Summary_seqs(fasta$, Optional processor% = 1) As String
        Return RunMothur($"summary.seqs(fasta={fasta}, processors={processor})")
    End Function

    Public Function Summary_seqs(fasta$, count$, Optional processors% = 1) As String
        Return RunMothur($"summary.seqs(fasta={fasta},count={count}, processors={processors})")
    End Function

    ''' <summary>
    ''' The screen.seqs command enables you to keep sequences that fulfill certain user defined criteria. Furthermore, 
    ''' it enables you to cull those sequences not meeting the criteria from a names, group, contigsreport, 
    ''' alignreport and summary file. 
    ''' </summary>
    ''' <param name="fasta$"></param>
    ''' <param name="group$"></param>
    ''' <param name="maxambig%"></param>
    ''' <param name="minlength%"></param>
    ''' <param name="maxlength%"></param>
    ''' <returns></returns>
    Public Function Screen_seqs(fasta$, group$, maxambig%, minlength%, maxlength%) As String
        Return RunMothur($"screen.seqs(fasta={fasta}, group={group}, maxambig={maxambig}, minlength={minlength}, maxlength={maxlength})")
    End Function

    Public Function Screen_seqs(fasta$, count$, summary$, start%, end%, maxhomop%, Optional processors% = 2) As String
        Return RunMothur($"screen.seqs(fasta={fasta}, count={count}, summary={summary}, start={start}, end={[end]}, maxhomop={maxhomop}, processors={processors})")
    End Function

    ''' <summary>
    ''' The ``unique.seqs`` command returns only the unique sequences found in a fasta-formatted 
    ''' sequence file and a file that indicates those sequences that are identical to the reference 
    ''' sequence. Often times a collection of sequences will have a significant number of identical 
    ''' sequences. It sucks up considerable processing time to have to align, calculate distances, 
    ''' and cluster each of these sequences individually. 
    ''' </summary>
    ''' <param name="fasta">To run the command the name of a fasta-file needs to be provided</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("unique.seqs")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Unique_seqs(fasta As String) As String
        Return RunMothur($"unique.seqs(fasta={fasta})")
    End Function

    ''' <summary>
    ''' The align.seqs command aligns a user-supplied fasta-formatted candidate sequence file to a 
    ''' user-supplied fasta-formatted template alignment. The general approach is to i) find the 
    ''' closest template for each candidate using kmer searching, blastn, or suffix tree searching; ii) 
    ''' to make a pairwise alignment between the candidate and de-gapped template sequences using the 
    ''' Needleman-Wunsch, Gotoh, or blastn algorithms; and iii) to re-insert gaps to the candidate and 
    ''' template pairwise alignments using the NAST algorithm so that the candidate sequence alignment 
    ''' is compatible with the original template alignment. We provide several alignment databases 
    ''' for 16S and 18S rRNA gene sequences that are compatible with the greengenes or SILVA alignments; 
    ''' however, custom alignments for any DNA sequence can be used as a template and users are encouraged 
    ''' to share their alignment for others to use. In general the alignment is very fast - we are able 
    ''' to align over 186,000 full-length sequences to the SILVA alignment in less than 3 hrs with a 
    ''' quality as good as the SINA aligner. Furthermore, this rate can be accelerated using multiple 
    ''' processors. While the aligner doesn't explicitly take into account the secondary structure of the 
    ''' 16S rRNA gene, if the template database is based on the secondary structure, then the resulting 
    ''' alignment will at least be implicitly based on the secondary structure.
    ''' </summary>
    ''' <param name="fasta$"></param>
    ''' <param name="reference"></param>
    ''' <param name="flip$">The flip parameter is used to specify whether or not you want mothur to try 
    ''' the reverse complement of a sequence if the sequence falls below the threshold. The default is false. 
    ''' If the flip parameter is set to true the reverse complement of the sequence is aligned and the better 
    ''' alignment is reporte</param>
    ''' <param name="processors%">
    ''' If you are a Windows user, the align.seqs command is now parallelized for you as well! If you are using 
    ''' the mpi-enabled version, processors is set to the number of processes you have running. The processors 
    ''' option enables you to accelerate the alignment by using multiple processors. You are able to use as 
    ''' many processors as your computer has with the following option:
    '''
    ''' ```
    ''' mothur> align.seqs(candidate=abrecovery.fasta, template=core_set_aligned.imputed.fasta, processors=2)
    ''' ```
    ''' Running this command On my laptop doesn't exactly cut the time in half, but it's pretty close. There is no 
    ''' software limit on the number of processors that you can use.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("align.seqs")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function align_seqs(fasta$, reference$, Optional flip$ = "F", Optional processors% = 1) As String
        Return RunMothur($"align.seqs(fasta={fasta},reference={reference},flip={flip},processors={processors})")
    End Function

    ''' <summary>
    ''' filter.seqs removes columns from alignments based on a criteria defined by the user. For example, 
    ''' alignments generated against reference alignments (e.g. from RDP, SILVA, or greengenes) often have 
    ''' columns where every character is either a '.' or a '-'. These columns are not included in calculating 
    ''' distances because they have no information in them. By removing these columns, the calculation of a 
    ''' large number of distances is accelerated. Also, people also like to mask their sequences to remove 
    ''' variable regions using a soft or hard mask (e.g. Lane's mask). This type of masking is only encouraged 
    ''' for deep-level phylogenetic analysis, not fine level analysis such as that needed with calculating OTUs.
    ''' </summary>
    ''' <param name="fasta"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("filter.seqs")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function filter_seqs(fasta As String) As String
        Return RunMothur($"filter.seqs(fasta={fasta})")
    End Function

    Public Function filter_seqs(fasta$, vertical$, trump$, Optional processors As Integer = 1) As String
        Return RunMothur($"filter.seqs(fasta={fasta}, vertical={vertical}, trump={trump}, processors={processors})")
    End Function

    ''' <summary>
    ''' The ``dist.seqs`` command will calculate uncorrected pairwise distances between aligned DNA 
    ''' sequences. This approach is better than the commonly used DNADIST because the distances are 
    ''' not stored in RAM, rather they are printed directly to a file. Furthermore, it is possible 
    ''' to ignore "large" distances that one might not be interested in. The command will generate a 
    ''' column-formatted distance matrix that is compatible with the column option in the read.dist 
    ''' command. The command is also able to generate a phylip-formatted distance matrix. There are 
    ''' several options for how to handle gap comparisons and terminal gaps.
    ''' </summary>
    ''' <param name="fasta$"></param>
    ''' <param name="calc$">
    ''' mothur has several ways of calculating a distance based on how gaps are treated. First, the 
    ''' default option - onegap - only counts a string of gaps as a single gap. For example:
    ''' 
    ''' ```
    ''' SequenceA  ATGCATGCATGC
    ''' SequenceB  ACGC---CATCC
    ''' ```
    ''' 
    ''' Would have two mismatches And one gap. The length Of the shorter sequence Is 10 nt, since the 
    ''' gap Is considered As a Single position. Therefore the distance would be 3/10 Or 0.30. This Is 
    ''' the distance calculating method employed by Sogin et al. (1995). The logic behind this type Of 
    ''' penalty Is that a gap represents an insertion And it Is likely that a gap Of any length represents 
    ''' a Single insertion. This can be called within mothur by the command:
    '''
    ''' ```
    ''' mothur > dist.seqs(fasta=amazon.unique.filter.fasta, calc=onegap)
    ''' ```
    ''' 
    ''' DNADIST actually ignores gaps. For example the two sequences above would have a distance Of 2/9 Or 
    ''' 0.2222. This type Of distance calculation does Not make much sense For sequences that are known To 
    ''' have a significant number Of insertions. This can be used In mothur Using the nogaps Option. mothur 
    ''' can use this distance calculating method With the command:
    '''
    ''' ```
    ''' mothur > dist.seqs(fasta=amazon.unique.filter.fasta, calc=nogaps)
    ''' ```
    ''' 
    ''' A final Option Is To penalize Each gap. For the two sequences above, the distance would be 5/12 Or 
    ''' 0.4167. This can be used In mothur Using the eachgap Option. mothur can use this distance calculating 
    ''' method With the command:
    '''
    ''' ```
    ''' mothur > dist.seqs(fasta=amazon.unique.filter.fasta, calc=eachgap)
    ''' ```
    ''' </param>
    ''' <param name="countends$">
    ''' There is some discussion over whether to penalize gaps that occur at the end of sequences. 
    ''' For example, consider the following sequences:
    ''' 
    ''' ```
    ''' Sequence1 ATGCATGCATGC
    ''' Sequence2 ---CAAGTA---
    ''' ```
    ''' 
    ''' If terminal gaps are penalized, As Is the Default (And we are Using the calc=onegap Option), 
    ''' Then the distance would be 4/8 Or 0.5000. If they are Not penalized, Then the distance would 
    ''' be 2/6 Or 0.3333. Ideally, all sequences would be aligned over the same region; however, If 
    ''' this Is Not possible Or desired For some reason the ends Option can be employed To tell mothur 
    ''' To ignore the penalization:
    '''
    ''' ```
    ''' mothur > dist.seqs(fasta= amazon.unique.filter.fasta, countends=F)
    ''' ```
    ''' 
    ''' The Default Is For countends To equal T.
    ''' </param>
    ''' <param name="cutoff#">
    ''' If you know that you are not going to form OTUs with distances larger than 0.10, you can tell 
    ''' mothur to not save any distances larger than 0.10. This will significantly cut down on the 
    ''' amount of hard drive space required to store the matrix. This can be done as follows:
    '''
    ''' ```
    ''' mothur > dist.seqs(fasta=amazon.unique.filter.fasta, cutoff=0.10)
    ''' ```
    ''' 
    ''' Without setting cutoff To 0.10 this command would have generated 4560 distances (i.e. 96x95/2 = 4560). 
    ''' With the cutoff only 56 distances are saved. The savings can be substantial When there are a large 
    ''' number Of distances. The actual cutoff used by the command Is 0.005 higher than the value that Is 
    ''' Set To allow For rounding In the clustering steps.
    ''' </param>
    ''' <param name="output$">
    ''' The output option allows you specify the form of the matrix generated by dist.seqs. By default, 
    ''' dist.seqs will generate a column-formatted matrix. You can set the output to "lt", for a phylip 
    ''' formatted lower triangle matrix, or to "square" for a phylip formatted square matrix. If output 
    ''' is set to lt or square the cutoff option is ignored.
    '''
    ''' ```
    ''' mothur > dist.seqs(fasta=amazon.unique.filter.fasta, output=lt)
    ''' ```
    ''' </param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("dist.seqs")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Dist_seqs(fasta$, Optional calc$ = "onegap", Optional countends$ = "F", Optional cutoff# = 0.03, Optional output$ = "lt", Optional processors% = 2) As String
        Return RunMothur($"dist.seqs(fasta={fasta},calc={calc},countends={countends},cutoff={cutoff},output={output},processors={processors})")
    End Function

    ''' <summary>
    ''' The ``make.contigs`` command reads a forward fastq file and a reverse fastq file and outputs 
    ''' new fasta and report files.
    ''' </summary>
    ''' <param name="ffastq$">The ffastq and rfastq parameters are used to provide a forward fastq 
    ''' and reverse fastq file to process. If you provide one, you must provide the other.</param>
    ''' <param name="rfastq$">The ffastq and rfastq parameters are used to provide a forward fastq 
    ''' and reverse fastq file to process. If you provide one, you must provide the other.</param>
    ''' <param name="processors">The processors parameter allows you to specify how many processors 
    ''' you would like to use. The default is 1.</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("make.contigs")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Make_contigs(ffastq$, rfastq$, Optional processors% = 1) As String
        Return RunMothur($"make.contigs(ffastq={ffastq}, rfastq={rfastq}, processors={processors})")
    End Function

    ''' <summary>
    ''' The count.seqs / make.table command counts the number of sequences represented by the representative sequence in a name file. 
    ''' If a group file is given, it will also provide the group count breakdown.
    ''' </summary>
    ''' <param name="name$"></param>
    ''' <param name="group$"></param>
    ''' <returns></returns>
    Public Function Count_seqs(name$, group$) As String
        Return RunMothur($"count.seqs(name={name}, group={group})")
    End Function

    ''' <summary>
    ''' The ``make.contigs`` command reads a forward fastq file and a reverse fastq file and outputs 
    ''' new fasta and report files.
    ''' </summary>
    ''' <param name="file$"></param>
    ''' <param name="processors"></param>
    ''' <returns></returns>
    Public Function Make_contigs(file$, Optional processors% = 1) As String
        Return RunMothur($"make.contigs(file={file}, processors={processors})")
    End Function

    ''' <summary>
    ''' Once a distance matrix gets read into mothur, the cluster command can be used to assign sequences 
    ''' to OTUs. Presently, mothur implements three clustering methods:
    '''
    ''' + Nearest neighbor:  Each of the sequences within an OTU are at most X% distant from the most similar sequence in the OTU.
    ''' + Furthest neighbor: All of the sequences within an OTU are at most X% distant from all of the other sequences within the OTU.
    ''' + Average neighbor:  This method Is a middle ground between the other two algorithms.
    ''' + AGC:
    ''' + DGC:
    ''' + Opti: OTUs are assembled Using metrics To determine the quality Of clustering.
    ''' 
    ''' If there Is an algorithm that you would Like To see implemented, please consider either contributing 
    ''' To the mothur project Or contacting the developers And we'll see what we can do. The opticlust 
    ''' algorithm is the default option. 
    ''' </summary>
    ''' <param name="phylip$">
    ''' To read in a phylip-formatted distance matrix you need to use the phylip option:
    '''
    ''' ```
    ''' mothur > cluster(phylip=final.phylip.dist)
    ''' ```
    ''' 
    ''' Whereas dotur required you To indicate whether the matrix was square Or lower-triangular, 
    ''' mothur Is able To figure this out For you.
    '''
    ''' Once you execute the command, mothur reads In the matrix And generates a progress bar:
    '''
    ''' ```
    ''' mothur > cluster(phylip=final.phylip.dist)
    ''' *******************#****#****#****#****#****#****#****#****#****#****#
    ''' Reading matrix :     |||||||||||||||||||||||||||||||||||||||||||||||||||
    ''' **********************************************************************
    ''' ```
    ''' </param>
    ''' <param name="method$">The methods available in mothur include opticlust (opti), average neighbor 
    ''' (average), furthest neighbor (furthest), nearest neighbor (nearest), Vsearch agc (agc), Vsearch dgc (dgc). 
    ''' By default cluster() uses the opticlust algorithm;</param>
    ''' <param name="cutoff#">With the opticlust method the list file is created for the cutoff you set. 
    ''' The default cutoff is 0.03. With the average neighbor, furthest neighbor and nearest neighbor 
    ''' methods the cutoff should be significantly higher than the desired distance in the list file. 
    ''' We suggest cutoff=0.20. This will provide a boost in speed and less RAM will be required than 
    ''' if you didn't set the cutoff for reading in the matrix.</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("cluster")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Cluster(phylip$, Optional method$ = "furthest", Optional cutoff# = 0.03) As String
        Return RunMothur($"cluster(phylip={phylip},method={method},cutoff={cutoff})")
    End Function

    ''' <summary>
    ''' bin.seqs prints out a fasta-formatted file where sequences are ordered according to the OTU that they belong to. 
    ''' Such an output may be helpful for generating primers specific to an OTU or for classification of sequences.
    ''' </summary>
    ''' <param name="fasta$"></param>
    ''' <param name="name$">
    ''' A names file indicating sequence names that are identical to a references sequence, may be inputted to bin.seqs 
    ''' so that the fasta and list files are complementary. The following commands illustrate this:
    ''' 
    ''' ```
    ''' mothur > bin.seqs(list=98_sq_phylip_amazon.an.list, fasta=amazon.unique.fasta, name=amazon.names)
    ''' ```
    ''' </param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("bin.seqs")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Bin_seqs(list$, fasta$, name$) As String
        Return RunMothur($"bin.seqs(list={list},fasta={fasta},name={name})")
    End Function

    ''' <summary>
    ''' While the bin.seqs command reports the OTU number for all sequences, the get.oturep command generates a fasta-formatted 
    ''' sequence file containing only a representative sequence for each OTU. A .rep.fasta and .rep.names file or .rep.count_table 
    ''' file is generated for each OTU definition.
    ''' </summary>
    ''' <param name="phylip$"></param>
    ''' <param name="fasta$">If you provide a fasta file, mothur will generate 12 fasta-formatted files (one per OTU definition 
    ''' in the list file) that each contain the same number of sequences as there are OTUs for that OTU definition. If there are 
    ''' three or more sequences in an OTU, the representative sequence is that sequence which is the minimum distance to the other 
    ''' sequences in the OTU.</param>
    ''' <param name="list$"></param>
    ''' <param name="label#">
    ''' There may only be a couple of lines in your OTU data that you are interested in running through get.oturep(). There are two options. 
    ''' You could: (i) manually delete the lines you aren't interested in from your list file; (ii) or use the label option. If you only 
    ''' want to read in the data for the lines labeled unique, 0.03, 0.05 and 0.10 you would enter:
    '''
    ''' ```
    ''' mothur> get.oturep(column=96_lt_column_amazon.dist, name=amazon.names, fasta=amazon.fasta, list=98_sq_phylip_amazon.fn.list, label=unique-0.03-0.05-0.10)
    ''' ```
    ''' </param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("get.oturep")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetOTUrep(phylip$, fasta$, list$, Optional label# = 0.03) As String
        Return RunMothur($"get.oturep(phylip={phylip},fasta={fasta},list={list},label={label})")
    End Function
End Class
