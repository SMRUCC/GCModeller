﻿#Region "Microsoft.VisualBasic::244b757cb61d6dee5fb5ce21bf2e78d5, Bio.Assembly\Assembly\NCBI\Database\COG\COGs\COGs.vb"

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

    '     Module COGs
    ' 
    '         Function: GroupRelease, SaveRelease
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.COG.COGs

    ''' <summary>
    ''' 
    ''' ############################################################
    ''' 0.	General remarks
    ''' ############################################################
    ''' 
    ''' This Is a December 2014 release of 2003-2014 COGs constructed by
    ''' Eugene Koonin 's group at the National Center for Biotechnology
    ''' Information (NCBI), National Library of Medicine (NLM), National
    ''' Institutes of Health (NIH).
    ''' 
    ''' #-----------------------------------------------------------
    ''' 0.1.	Citation
    ''' 
    ''' Galperin MY, Makarova KS, Wolf YI, Koonin EV.
    ''' 
    ''' Expanded microbial genome coverage And improved protein family
    ''' annotation in the COG database.
    ''' 
    ''' Nucleic Acids Res. 43, D261-D269, 2015
    '''  &lt;http: //www.ncbi.nlm.nih.gov/pubmed/25428365>
    ''' 
    ''' #-----------------------------------------------------------
    ''' 0.2.	Contacts
    ''' 
    ''' &lt;COGsncbi.nlm.nih.gov>
    ''' 
    ''' ############################################################
    ''' 1.	Notes
    ''' ############################################################
    ''' 
    ''' #-----------------------------------------------------------
    ''' 1.1.	2003-2014 COGs
    ''' 
    ''' This release contains 2003 COGs assigned To a representative Set Of
    ''' bacterial And archaeal genomes, available at February 2014. No New
    ''' COGs were constructed.
    ''' 
    ''' #-----------------------------------------------------------
    ''' 1.2.	GIs And Refseq IDs
    ''' 
    ''' Sequences in COGs are identified by GenBank GI numbers. GI numbers
    ''' generally are transient. There are two ways To make a more permanent
    ''' link between the protein In COGs And the outside databases: via the
    ''' RefSeq accession codes (see 2.5) And via the protein sequences (see
    ''' 2.6).
    ''' 
    ''' Note, however, that at the moment (April 02, 2015) RefSeq database Is
    ''' in a state of transition; some of the &lt;refseq-acc> entries are Not
    ''' accessible. This accession table will be updated as soon as RefSeq Is
    ''' stable.
    ''' 
    ''' </summary>
    ''' 
    <Package("NCBI.COGs", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module COGs

        ''' <summary>
        ''' 将prot2003-2014.fasta按照<see cref="ProtFasta.GenomeName"/>分组导出，以方便使用bbh进行注释分析
        ''' </summary>
        ''' <param name="Fasta"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Group.Release")>
        Public Function GroupRelease(Fasta As String) As Dictionary(Of String, ProtFasta())
            Call $"Load document from {Fasta.ToFileURL}".__DEBUG_ECHO

            Dim protFastas = ProtFasta.LoadDocument(Fasta)
            Call $"Group by genome name operations...".__DEBUG_ECHO
            Dim LQuery = (From prot As ProtFasta
                          In protFastas
                          Select prot
                          Group prot By prot.GenomeName Into Group) _
                            .ToDictionary(Function(genome) genome.GenomeName,
                                          Function(genome) genome.Group.ToArray)
            Call $"{LQuery.Count} genomes in total!".__DEBUG_ECHO
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Fasta">prot2003-2014.fasta</param>
        ''' <param name="EXPORT">数据按照基因组分组到处的结果所保存的文件夹</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Db.Install")>
        Public Function SaveRelease(Fasta As String, EXPORT As String) As Boolean
            Dim groupData = COGs.GroupRelease(Fasta)

            Call $"Save data in the repository: {EXPORT}".__DEBUG_ECHO

            For Each genome In groupData
                Dim name$ = genome.Key.NormalizePathString(True).Replace(" ", "_")
                Dim path As String = $"{EXPORT}/{name}.fasta"  ' blast+ 的序列文件路径之中不能够有空格
                Dim protFasta As New FastaFile(genome.Value)

                Call protFasta.Save(path)
                Call path.ToFileURL.__DEBUG_ECHO
            Next

            Return True
        End Function
    End Module
End Namespace
