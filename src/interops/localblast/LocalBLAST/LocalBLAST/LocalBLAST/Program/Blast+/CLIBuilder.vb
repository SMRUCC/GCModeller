#Region "Microsoft.VisualBasic::6dd96e17d3a046b89ccd414e021ebaa6, LocalBLAST\LocalBLAST\LocalBLAST\Program\Blast+\CLIBuilder.vb"

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

    '     Class BlastpOptionalArguments
    ' 
    '         Properties: BestHitOverhang, BestHitScoreEdge, CompBasedStats, CullingLimit, DbHardMask
    '                     DbSize, DbSoftMask, EntrezQuery, ExportSearchStrategy, GapExtend
    '                     GapOpen, GiList, Html, ImportSearchStrategy, LCaseMasking
    '                     Matrix, MaxHspsPerSubject, MaxTargetSeqs, NegativeGiList, NumberAlignments
    '                     NumberDescriptions, NumThreads, OutFormat, ParseDeflines, QueryLocation
    '                     Remote, SearcHsp, Seg, SeqIdList, ShowGis
    '                     SoftMasking, SubjectLocation, Task, Threshold, UnGapped
    '                     UseSwTback, WindowSize, WordSize, XDropGap, XDropGapFinal
    '                     XDropUngap
    ' 
    '     Class BlastnOptionalArguments
    ' 
    '         Properties: BestHitOverhang, BestHitScoreEdge, CompBasedStats, CullingLimit, DbHardMask
    '                     DbSize, DbSoftMask, EntrezQuery, ExportSearchStrategy, GapExtend
    '                     GapOpen, GiList, Html, ImportSearchStrategy, LCaseMasking
    '                     Matrix, MaxHspsPerSubject, MaxTargetSeqs, NegativeGiList, NumberAlignments
    '                     NumberDescriptions, NumThreads, OutFormat, ParseDeflines, penalty
    '                     Query, QueryLocation, Remote, reward, SearcHsp
    '                     Seg, SeqIdList, ShowGis, SoftMasking, SubjectLocation
    '                     Task, Threshold, UnGapped, UseSwTback, WindowSize
    '                     WordSize, XDropGap, XDropGapFinal, XDropUngap
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports CLI = Microsoft.VisualBasic.CommandLine.InteropService.InteropService

Namespace LocalBLAST.Programs.CLIArgumentsBuilder

    Public Class BlastpOptionalArguments : Inherits CLI

#Region "*** Input query options"
        ''' <summary>
        ''' Location on the query sequence in 1-based offsets (Format: start-stop)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-query_loc", CLITypes.String)> Public Property QueryLocation As String
#End Region

#Region "*** General search options"
        ''' <summary>
        ''' Task to execute, Permissible values: 'blastp' 'blastp-short' 'deltablast'
        ''' Default = `blastp'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-task", CLITypes.String)> Public Property Task As String

        ''' <summary>
        ''' Word size for wordfinder algorithm, >=2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-word_size", CLITypes.Integer)> Public Property WordSize As Integer

        ''' <summary>
        ''' Cost to open a gap
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-gapopen", CLITypes.Integer)> Public Property GapOpen As String

        ''' <summary>
        ''' Cost to extend a gap
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-gapextend", CLITypes.Integer)> Public Property GapExtend As String

        ''' <summary>
        ''' Scoring matrix name (normally BLOSUM62)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-matrix", CLITypes.String)> Public Property Matrix As String

        ''' <summary>
        ''' Minimum word score such that the word is added to the BLAST lookup table, >=0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-threshold", CLITypes.Double)> Public Property Threshold As String

        ''' <summary>
        ''' Use composition-based statistics:
        ''' D or d: default (equivalent to 2 )
        ''' 0 or F or f: No composition-based statistics
        ''' 1: Composition-based statistics as in NAR 29:2994-3005, 2001
        ''' 2 or T or t : Composition-based score adjustment as in Bioinformatics 21:902-911, 2005, conditioned on sequence properties
        ''' 3: Composition-based score adjustment as in Bioinformatics 21:902-911, 2005, unconditionally
        ''' 
        ''' Default = `2'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-comp_based_stats", CLITypes.String)> Public Property CompBasedStats As String

#End Region

#Region "*** BLAST-2-Sequences options"
        ''' <summary>
        ''' Location on the subject sequence in 1-based offsets (Format: start-stop)
        ''' * Incompatible with:  db, gilist, seqidlist, negative_gilist, db_soft_mask, db_hard_mask, remote
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-subject_loc", CLITypes.String)> Public Property SubjectLocation As String
#End Region

#Region "*** Formatting options"
        ''' <summary>
        ''' Alignment view options:
        ''' 0 = pairwise,
        ''' 1 = query-anchored showing identities,
        ''' 2 = query-anchored no identities,
        ''' 3 = flat query-anchored, show identities,
        ''' 4 = flat query-anchored, no identities,
        ''' 5 = XML Blast output,
        ''' 6 = tabular,
        ''' 7 = tabular with comment lines,
        ''' 8 = Text ASN.1,
        ''' 9 = Binary ASN.1,
        ''' 10 = Comma-separated values,
        ''' 11 = BLAST archive format (ASN.1) 
        ''' 
        ''' Options 6, 7, and 10 can be additionally configured to produce a custom format specified by space delimited 
        ''' format specifiers.
        ''' The supported format specifiers are:
        '''         qseqid means Query Seq-id
        '''            qgi means Query GI
        '''           qacc means Query accesion
        '''        qaccver means Query accesion.version
        '''           qlen means Query sequence length
        '''         sseqid means Subject Seq-id
        '''      sallseqid means All subject Seq-id(s), separated by a ';'
        '''            sgi means Subject GI
        '''  	    sallgi means All subject GIs
        '''           sacc means Subject accession
        ''' 	   saccver means Subject accession.version
        ''' 	   sallacc means All subject accessions
        '''           slen means Subject sequence length
        ''' 	    qstart means Start of alignment in query
        '''  	      qend means End of alignment in query
        ''' 	    sstart means Start of alignment in subject
        ''' 	      send means End of alignment in subject
        ''' 	      qseq means Aligned part of query sequence
        '''           sseq means Aligned part of subject sequence
        '''    	    evalue means Expect value
        ''' 	  bitscore means Bit score
        '''    	     score means Raw score
        '''         length means Alignment length
        '''  	    pident means Percentage of identical matches
        '''         nident means Number of identical matches
        '''  	  mismatch means Number of mismatches
        '''       positive means Number of positive-scoring matches
        '''        gapopen means Number of gap openings
        '''           gaps means Total number of gaps
        ''' 	      ppos means Percentage of positive-scoring matches
        '''         frames means Query and subject frames separated by a '/'
        ''' 	    qframe means Query frame
        ''' 	    sframe means Subject frame
        '''  	      btop means Blast traceback operations (BTOP)
        '''  	   staxids means Subject Taxonomy ID(s), separated by a ';'
        '''  	 sscinames means Subject Scientific Name(s), separated by a ';'
        '''  	 scomnames means Subject Common Name(s), separated by a ';'
        '''    sblastnames means Subject Blast Name(s), separated by a ';'
        ''' 	                 (in alphabetical order)
        '''     sskingdoms means Subject Super Kingdom(s), separated by a ';'
        '''                      (in alphabetical order) 
        '''         stitle means Subject Title
        '''     salltitles means All Subject Title(s), separated by a 
        '''        sstrand means Subject Strand
        '''          qcovs means Query Coverage Per Subject
        '''        qcovhsp means Query Coverage Per HSP
        ''' When not provided, the default value is:
        ''' 'qseqid sseqid pident length mismatch gapopen qstart qend sstart send evalue bitscore', which is equivalent to the keyword 'std'
        ''' Default = `0'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-outfmt", CLITypes.String)> Public Property OutFormat As String

        ''' <summary>
        ''' Show NCBI GIs in deflines?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-show_gis", CLITypes.String)> Public Property ShowGis As String

        ''' <summary>
        ''' Number  >=0 of database sequences to show one-line descriptions for Not applicable for outfmt > 4
        ''' Default = `500'
        ''' * Incompatible with:  max_target_seqs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-num_descriptions", CLITypes.Integer)> Public Property NumberDescriptions As String

        ''' <summary>
        ''' Number >=0 of database sequences to show alignments for
        ''' Default = `250'
        ''' * Incompatible with:  max_target_seqs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-num_alignments", CLITypes.Integer)> Public Property NumberAlignments As String

        ''' <summary>
        ''' Produce HTML output?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-html", CLITypes.String)> Public Property Html As String
#End Region

#Region "*** Query filtering options"
        ''' <summary>
        ''' Filter query sequence with SEG (Format: 'yes', 'window locut hicut', or 'no' to disable)
        ''' Default = `no'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-seg", CLITypes.String)> Public Property Seg As String

        ''' <summary>
        ''' Apply filtering locations as soft masks
        ''' Default = `false'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-soft_masking", CLITypes.String)> Public Property SoftMasking As String

        ''' <summary>
        ''' Use lower case filtering in query and subject sequence(s)?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-lcase_masking", CLITypes.String)> Public Property LCaseMasking As String
#End Region

#Region "*** Restrict search or results"
        ''' <summary>
        ''' Restrict search of database to list of GI's
        ''' * Incompatible with:  negative_gilist, seqidlist, remote, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-gilist", CLITypes.String)> Public Property GiList As String

        ''' <summary>
        ''' Restrict search of database to list of SeqId's
        ''' * Incompatible with:  gilist, negative_gilist, remote, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-seqidlist", CLITypes.String)> Public Property SeqIdList As String

        ''' <summary>
        ''' Restrict search of database to everything except the listed GIs
        ''' * Incompatible with:  gilist, seqidlist, remote, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-negative_gilist", CLITypes.String)> Public Property NegativeGiList As String

        ''' <summary>
        ''' Restrict search with the given Entrez query
        ''' * Requires:  remote
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-entrez_query", CLITypes.String)> Public Property EntrezQuery As String

        ''' <summary>
        ''' Filtering algorithm ID to apply to the BLAST database as soft masking
        ''' * Incompatible with:  db_hard_mask, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-db_soft_mask", CLITypes.String)> Public Property DbSoftMask As String

        ''' <summary>
        ''' Filtering algorithm ID to apply to the BLAST database as hard masking
        ''' * Incompatible with:  db_soft_mask, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-db_hard_mask", CLITypes.String)> Public Property DbHardMask As String

        ''' <summary>
        ''' If the query range of a hit is enveloped by that of at least this many higher-scoring hits >=0, delete the hit
        ''' * Incompatible with:  best_hit_overhang, best_hit_score_edge
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-culling_limit", CLITypes.Integer)> Public Property CullingLimit As String

        ''' <summary>
        ''' Best Hit algorithm overhang value ((0, 0.5), recommended value: 0.1)
        ''' * Incompatible with:  culling_limit
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-best_hit_overhang", CLITypes.Double)> Public Property BestHitOverhang As String

        ''' <summary>
        ''' Best Hit algorithm score edge value ((0, 0.5), recommended value: 0.1)
        ''' * Incompatible with:  culling_limit
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-best_hit_score_edge", CLITypes.Double)> Public Property BestHitScoreEdge As String

        ''' <summary>
        ''' Maximum number >=1 of aligned sequences to keep 
        ''' Not applicable for outfmt less than 4
        ''' Default = `500'
        ''' * Incompatible with:  num_descriptions, num_alignments
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-max_target_seqs", CLITypes.Integer)> Public Property MaxTargetSeqs As String
#End Region

#Region "*** Statistical options"
        ''' <summary>
        ''' Effective length of the database 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-dbsize", CLITypes.Integer)> Public Property DbSize As String

        ''' <summary>
        ''' Effective length >=0 of the search space
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-searchsp", CLITypes.Integer)> Public Property SearcHsp As String

        ''' <summary>
        ''' Override maximum number >=0 of HSPs per subject to save for ungapped searches
        ''' (0 means do not override)
        ''' Default = `0'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-max_hsps_per_subject", CLITypes.Integer)> Public Property MaxHspsPerSubject As String
#End Region

#Region "*** Search strategy options"
        ''' <summary>
        ''' Search strategy to use
        ''' * Incompatible with:  export_search_strategy
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-import_search_strategy", CLITypes.String)> Public Property ImportSearchStrategy As String

        ''' <summary>
        ''' File name to record the search strategy used
        ''' * Incompatible with:  import_search_strategy
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-export_search_strategy", CLITypes.String)> Public Property ExportSearchStrategy As String
#End Region

#Region "*** Extension options"
        ''' <summary>
        ''' X-dropoff value (in bits) for ungapped extensions
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-xdrop_ungap", CLITypes.Double)> Public Property XDropUngap As String

        ''' <summary>
        ''' X-dropoff value (in bits) for preliminary gapped extensions
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-xdrop_gap", CLITypes.Double)> Public Property XDropGap As String

        ''' <summary>
        ''' X-dropoff value (in bits) for final gapped alignment
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-xdrop_gap_final", CLITypes.Double)> Public Property XDropGapFinal As String

        ''' <summary>
        ''' Multiple hits window size >=0, use 0 to specify 1-hit algorithm
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-window_size", CLITypes.Integer)> Public Property WindowSize As String

        ''' <summary>
        ''' Perform ungapped alignment only?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-ungapped", CLITypes.String)> Public Property UnGapped As String
#End Region

#Region "*** Miscellaneous options"
        ''' <summary>
        ''' Should the query and subject defline(s) be parsed?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-parse_deflines", CLITypes.String)> Public Property ParseDeflines As String

        ''' <summary>
        ''' Number of threads >=1 (CPUs) to use in the BLAST search
        ''' Default = `1'
        ''' * Incompatible with:  remote
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-num_threads", CLITypes.Integer)> Public Property NumThreads As String

        ''' <summary>
        ''' Execute search remotely?
        ''' * Incompatible with:  gilist, seqidlist, negative_gilist, subject_loc, num_threads
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-remote", CLITypes.String)> Public Property Remote As String


        ''' <summary>
        ''' Compute locally optimal Smith-Waterman alignments?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-use_sw_tback", CLITypes.String)> Public Property UseSwTback As String
#End Region
    End Class

    Public Class BlastnOptionalArguments : Inherits CLI
#Region "*** Input query options"
        ''' <summary>
        ''' Location on the query sequence in 1-based offsets (Format: start-stop)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-query_loc", CLITypes.String)> Public Property QueryLocation As String
#End Region

#Region "*** General search options"
        ''' <summary>
        ''' Task to execute, Permissible values: 'blastp' 'blastp-short' 'deltablast'
        ''' Default = `blastp'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-task", CLITypes.String)> Public Property Task As String

        ''' <summary>
        ''' Word size for wordfinder algorithm, >=2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-word_size", CLITypes.Integer)> Public Property WordSize As Integer

        ''' <summary>
        ''' Cost to open a gap
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-gapopen", CLITypes.Integer)> Public Property GapOpen As String

        ''' <summary>
        ''' Cost to extend a gap
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-gapextend", CLITypes.Integer)> Public Property GapExtend As String

        ''' <summary>
        ''' Scoring matrix name (normally BLOSUM62)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-matrix", CLITypes.String)> Public Property Matrix As String

        ''' <summary>
        ''' Minimum word score such that the word is added to the BLAST lookup table, >=0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-threshold", CLITypes.Double)> Public Property Threshold As String

        ''' <summary>
        ''' Use composition-based statistics:
        ''' D or d: default (equivalent to 2 )
        ''' 0 or F or f: No composition-based statistics
        ''' 1: Composition-based statistics as in NAR 29:2994-3005, 2001
        ''' 2 or T or t : Composition-based score adjustment as in Bioinformatics 21:902-911, 2005, conditioned on sequence properties
        ''' 3: Composition-based score adjustment as in Bioinformatics 21:902-911, 2005, unconditionally
        ''' 
        ''' Default = `2'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-comp_based_stats", CLITypes.String)> Public Property CompBasedStats As String

#End Region

#Region "*** BLAST-2-Sequences options"
        ''' <summary>
        ''' Location on the subject sequence in 1-based offsets (Format: start-stop)
        ''' * Incompatible with:  db, gilist, seqidlist, negative_gilist, db_soft_mask, db_hard_mask, remote
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-subject_loc", CLITypes.String)> Public Property SubjectLocation As String
#End Region

#Region "*** Formatting options"
        ''' <summary>
        ''' Alignment view options:
        ''' 0 = pairwise,
        ''' 1 = query-anchored showing identities,
        ''' 2 = query-anchored no identities,
        ''' 3 = flat query-anchored, show identities,
        ''' 4 = flat query-anchored, no identities,
        ''' 5 = XML Blast output,
        ''' 6 = tabular,
        ''' 7 = tabular with comment lines,
        ''' 8 = Text ASN.1,
        ''' 9 = Binary ASN.1,
        ''' 10 = Comma-separated values,
        ''' 11 = BLAST archive format (ASN.1) 
        ''' 
        ''' Options 6, 7, and 10 can be additionally configured to produce a custom format specified by space delimited 
        ''' format specifiers.
        ''' The supported format specifiers are:
        '''         qseqid means Query Seq-id
        '''            qgi means Query GI
        '''           qacc means Query accesion
        '''        qaccver means Query accesion.version
        '''           qlen means Query sequence length
        '''         sseqid means Subject Seq-id
        '''      sallseqid means All subject Seq-id(s), separated by a ';'
        '''            sgi means Subject GI
        '''  	    sallgi means All subject GIs
        '''           sacc means Subject accession
        ''' 	   saccver means Subject accession.version
        ''' 	   sallacc means All subject accessions
        '''           slen means Subject sequence length
        ''' 	    qstart means Start of alignment in query
        '''  	      qend means End of alignment in query
        ''' 	    sstart means Start of alignment in subject
        ''' 	      send means End of alignment in subject
        ''' 	      qseq means Aligned part of query sequence
        '''           sseq means Aligned part of subject sequence
        '''    	    evalue means Expect value
        ''' 	  bitscore means Bit score
        '''    	     score means Raw score
        '''         length means Alignment length
        '''  	    pident means Percentage of identical matches
        '''         nident means Number of identical matches
        '''  	  mismatch means Number of mismatches
        '''       positive means Number of positive-scoring matches
        '''        gapopen means Number of gap openings
        '''           gaps means Total number of gaps
        ''' 	      ppos means Percentage of positive-scoring matches
        '''         frames means Query and subject frames separated by a '/'
        ''' 	    qframe means Query frame
        ''' 	    sframe means Subject frame
        '''  	      btop means Blast traceback operations (BTOP)
        '''  	   staxids means Subject Taxonomy ID(s), separated by a ';'
        '''  	 sscinames means Subject Scientific Name(s), separated by a ';'
        '''  	 scomnames means Subject Common Name(s), separated by a ';'
        '''    sblastnames means Subject Blast Name(s), separated by a ';'
        ''' 	                 (in alphabetical order)
        '''     sskingdoms means Subject Super Kingdom(s), separated by a ';'
        '''                      (in alphabetical order) 
        '''         stitle means Subject Title
        '''     salltitles means All Subject Title(s), separated by a 
        '''        sstrand means Subject Strand
        '''          qcovs means Query Coverage Per Subject
        '''        qcovhsp means Query Coverage Per HSP
        ''' When not provided, the default value is:
        ''' 'qseqid sseqid pident length mismatch gapopen qstart qend sstart send evalue bitscore', which is equivalent to the keyword 'std'
        ''' Default = `0'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-outfmt", CLITypes.String)> Public Property OutFormat As String

        ''' <summary>
        ''' Show NCBI GIs in deflines?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-show_gis", CLITypes.String)> Public Property ShowGis As String

        ''' <summary>
        ''' Number  >=0 of database sequences to show one-line descriptions for Not applicable for outfmt > 4
        ''' Default = `500'
        ''' * Incompatible with:  max_target_seqs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-num_descriptions", CLITypes.Integer)> Public Property NumberDescriptions As String

        ''' <summary>
        ''' Number >=0 of database sequences to show alignments for
        ''' Default = `250'
        ''' * Incompatible with:  max_target_seqs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-num_alignments", CLITypes.Integer)> Public Property NumberAlignments As String

        ''' <summary>
        ''' Produce HTML output?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-html", CLITypes.String)> Public Property Html As String
#End Region

#Region "*** Query filtering options"
        ''' <summary>
        ''' Filter query sequence with SEG (Format: 'yes', 'window locut hicut', or 'no' to disable)
        ''' Default = `no'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-seg", CLITypes.String)> Public Property Seg As String

        ''' <summary>
        ''' Apply filtering locations as soft masks
        ''' Default = `false'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-soft_masking", CLITypes.String)> Public Property SoftMasking As String

        ''' <summary>
        ''' Use lower case filtering in query and subject sequence(s)?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-lcase_masking", CLITypes.String)> Public Property LCaseMasking As String
#End Region

#Region "*** Restrict search or results"
        ''' <summary>
        ''' Restrict search of database to list of GI's
        ''' * Incompatible with:  negative_gilist, seqidlist, remote, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-gilist", CLITypes.String)> Public Property GiList As String

        ''' <summary>
        ''' Restrict search of database to list of SeqId's
        ''' * Incompatible with:  gilist, negative_gilist, remote, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-seqidlist", CLITypes.String)> Public Property SeqIdList As String

        ''' <summary>
        ''' Restrict search of database to everything except the listed GIs
        ''' * Incompatible with:  gilist, seqidlist, remote, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-negative_gilist", CLITypes.String)> Public Property NegativeGiList As String

        ''' <summary>
        ''' Restrict search with the given Entrez query
        ''' * Requires:  remote
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-entrez_query", CLITypes.String)> Public Property EntrezQuery As String

        ''' <summary>
        ''' Filtering algorithm ID to apply to the BLAST database as soft masking
        ''' * Incompatible with:  db_hard_mask, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-db_soft_mask", CLITypes.String)> Public Property DbSoftMask As String

        ''' <summary>
        ''' Filtering algorithm ID to apply to the BLAST database as hard masking
        ''' * Incompatible with:  db_soft_mask, subject, subject_loc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-db_hard_mask", CLITypes.String)> Public Property DbHardMask As String

        ''' <summary>
        ''' If the query range of a hit is enveloped by that of at least this many higher-scoring hits >=0, delete the hit
        ''' * Incompatible with:  best_hit_overhang, best_hit_score_edge
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-culling_limit", CLITypes.Integer)> Public Property CullingLimit As String

        ''' <summary>
        ''' Best Hit algorithm overhang value ((0, 0.5), recommended value: 0.1)
        ''' * Incompatible with:  culling_limit
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-best_hit_overhang", CLITypes.Double)> Public Property BestHitOverhang As String

        ''' <summary>
        ''' Best Hit algorithm score edge value ((0, 0.5), recommended value: 0.1)
        ''' * Incompatible with:  culling_limit
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-best_hit_score_edge", CLITypes.Double)> Public Property BestHitScoreEdge As String

        ''' <summary>
        ''' Maximum number >=1 of aligned sequences to keep 
        ''' Not applicable for outfmt less than 4
        ''' Default = `500'
        ''' * Incompatible with:  num_descriptions, num_alignments
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-max_target_seqs", CLITypes.Integer)> Public Property MaxTargetSeqs As String
#End Region

#Region "*** Statistical options"
        ''' <summary>
        ''' Effective length of the database 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-dbsize", CLITypes.Integer)> Public Property DbSize As String

        ''' <summary>
        ''' Effective length >=0 of the search space
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-searchsp", CLITypes.Integer)> Public Property SearcHsp As String

        ''' <summary>
        ''' Override maximum number >=0 of HSPs per subject to save for ungapped searches
        ''' (0 means do not override)
        ''' Default = `0'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-max_hsps_per_subject", CLITypes.Integer)> Public Property MaxHspsPerSubject As String
#End Region

#Region "*** Search strategy options"
        ''' <summary>
        ''' Search strategy to use
        ''' * Incompatible with:  export_search_strategy
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-import_search_strategy", CLITypes.String)> Public Property ImportSearchStrategy As String

        ''' <summary>
        ''' File name to record the search strategy used
        ''' * Incompatible with:  import_search_strategy
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-export_search_strategy", CLITypes.String)> Public Property ExportSearchStrategy As String
#End Region

#Region "*** Extension options"
        ''' <summary>
        ''' X-dropoff value (in bits) for ungapped extensions
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-xdrop_ungap", CLITypes.Double)> Public Property XDropUngap As String

        ''' <summary>
        ''' X-dropoff value (in bits) for preliminary gapped extensions
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-xdrop_gap", CLITypes.Double)> Public Property XDropGap As String

        ''' <summary>
        ''' X-dropoff value (in bits) for final gapped alignment
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-xdrop_gap_final", CLITypes.Double)> Public Property XDropGapFinal As String

        ''' <summary>
        ''' Multiple hits window size >=0, use 0 to specify 1-hit algorithm
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-window_size", CLITypes.Integer)> Public Property WindowSize As String

        ''' <summary>
        ''' Perform ungapped alignment only?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-ungapped", CLITypes.String)> Public Property UnGapped As String
#End Region

#Region "*** Miscellaneous options"
        ''' <summary>
        ''' Should the query and subject defline(s) be parsed?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-parse_deflines", CLITypes.String)> Public Property ParseDeflines As String

        ''' <summary>
        ''' Number of threads >=1 (CPUs) to use in the BLAST search
        ''' Default = `1'
        ''' * Incompatible with:  remote
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-num_threads", CLITypes.Integer)> Public Property NumThreads As String

        ''' <summary>
        ''' Execute search remotely?
        ''' * Incompatible with:  gilist, seqidlist, negative_gilist, subject_loc, num_threads
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-remote", CLITypes.String)> Public Property Remote As String


        ''' <summary>
        ''' Compute locally optimal Smith-Waterman alignments?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-use_sw_tback", CLITypes.String)> Public Property UseSwTback As String
#End Region

#Region "*** Input query options"

        ''' <summary>
        ''' -query &lt;File_In&gt;
        ''' 
        ''' Input file name
        ''' Default = `-'
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-query")> Public Property Query As String

        ' -query_loc <String>
        '   Location on the query sequence in 1-based offsets (Format: start-stop)
        ' -strand <String, `both', `minus', `plus'>
        '   Query strand(s) to search against database/subject
        '   Default = `both'
#End Region

        Public Property penalty As Double = -1
        Public Property reward As Double = -1

#Region "*** General search options"

        ' -task <String, Permissible values: 'blastn' 'blastn-short' 'dc-megablast'
        '        'megablast' 'rmblastn' >
        '   Task to execute
        '   Default = `megablast'
        ' -db <String>
        '   BLAST database name
        '    * Incompatible with:  subject, subject_loc
        ' -out <File_Out>
        '   Output file name
        '   Default = `-'
        ' -evalue <Real>
        '   Expectation value (E) threshold for saving hits 
        '   Default = `10'
        ' -word_size <Integer, >=4>
        '   Word size for wordfinder algorithm (length of best perfect match)
        ' -gapopen <Integer>
        '   Cost to open a gap
        ' -gapextend <Integer>
        '   Cost to extend a gap
        ' -penalty <Integer, <=0>
        '   Penalty for a nucleotide mismatch
        ' -reward <Integer, >=0>
        '   Reward for a nucleotide match
        ' -use_index <Boolean>
        '   Use MegaBLAST database index
        ' -index_name <String>
        '   MegaBLAST database index name
#End Region

#Region "*** BLAST-2-Sequences options"

        ' -subject <File_In>
        '   Subject sequence(s) to search
        '    * Incompatible with:  db, gilist, seqidlist, negative_gilist,
        '   db_soft_mask, db_hard_mask
        ' -subject_loc <String>
        '   Location on the subject sequence in 1-based offsets (Format: start-stop)
        '    * Incompatible with:  db, gilist, seqidlist, negative_gilist,
        '   db_soft_mask, db_hard_mask, remote
#End Region

#Region "*** Formatting options"

        ' -outfmt <String>
        '   alignment view options:
        '     0 = pairwise,
        '     1 = query-anchored showing identities,
        '     2 = query-anchored no identities,
        '     3 = flat query-anchored, show identities,
        '     4 = flat query-anchored, no identities,
        '     5 = XML Blast output,
        '     6 = tabular,
        '     7 = tabular with comment lines,
        '     8 = Text ASN.1,
        '     9 = Binary ASN.1,
        '    10 = Comma-separated values,
        '    11 = BLAST archive format (ASN.1) 

        '   Options 6, 7, and 10 can be additionally configured to produce
        '   a custom format specified by space delimited format specifiers.
        '   The supported format specifiers are:
        '   	    qseqid means Query Seq-id
        '   	       qgi means Query GI
        '   	      qacc means Query accesion
        '   	   qaccver means Query accesion.version
        '   	      qlen means Query sequence length
        '   	    sseqid means Subject Seq-id
        '   	 sallseqid means All subject Seq-id(s), separated by a ';'
        '   	       sgi means Subject GI
        '   	    sallgi means All subject GIs
        '   	      sacc means Subject accession
        '   	   saccver means Subject accession.version
        '   	   sallacc means All subject accessions
        '   	      slen means Subject sequence length
        '   	    qstart means Start of alignment in query
        '   	      qend means End of alignment in query
        '   	    sstart means Start of alignment in subject
        '   	      send means End of alignment in subject
        '   	      qseq means Aligned part of query sequence
        '   	      sseq means Aligned part of subject sequence
        '   	    evalue means Expect value
        '   	  bitscore means Bit score
        '   	     score means Raw score
        '   	    length means Alignment length
        '   	    pident means Percentage of identical matches
        '   	    nident means Number of identical matches
        '   	  mismatch means Number of mismatches
        '   	  positive means Number of positive-scoring matches
        '   	   gapopen means Number of gap openings
        '   	      gaps means Total number of gaps
        '   	      ppos means Percentage of positive-scoring matches
        '   	    frames means Query and subject frames separated by a '/'
        '   	    qframe means Query frame
        '   	    sframe means Subject frame
        '   	      btop means Blast traceback operations (BTOP)
        '   	   staxids means Subject Taxonomy ID(s), separated by a ';'
        '   	 sscinames means Subject Scientific Name(s), separated by a ';'
        '   	 scomnames means Subject Common Name(s), separated by a ';'
        '   	sblastnames means Subject Blast Name(s), separated by a ';'
        '   			 (in alphabetical order)
        '   	sskingdoms means Subject Super Kingdom(s), separated by a ';'
        '   			 (in alphabetical order) 
        '   	    stitle means Subject Title
        '   	salltitles means All Subject Title(s), separated by a '<>'
        '   	   sstrand means Subject Strand
        '   	     qcovs means Query Coverage Per Subject
        '   	   qcovhsp means Query Coverage Per HSP
        '   When not provided, the default value is:
        '   'qseqid sseqid pident length mismatch gapopen qstart qend sstart send
        '   evalue bitscore', which is equivalent to the keyword 'std'
        '   Default = `0'
        ' -show_gis
        '   Show NCBI GIs in deflines?
        ' -num_descriptions <Integer, >=0>
        '   Number of database sequences to show one-line descriptions for
        '   Not applicable for outfmt > 4
        '   Default = `500'
        '    * Incompatible with:  max_target_seqs
        ' -num_alignments <Integer, >=0>
        '   Number of database sequences to show alignments for
        '   Default = `250'
        '    * Incompatible with:  max_target_seqs
        ' -html
        '   Produce HTML output?
#End Region

#Region "*** Query filtering options"
        ' -dust <String>
        '   Filter query sequence with DUST (Format: 'yes', 'level window linker', or
        '   'no' to disable)
        '   Default = `20 64 1'
        ' -filtering_db <String>
        '   BLAST database containing filtering elements (i.e.: repeats)
        ' -window_masker_taxid <Integer>
        '   Enable WindowMasker filtering using a Taxonomic ID
        ' -window_masker_db <String>
        '   Enable WindowMasker filtering using this repeats database.
        ' -soft_masking <Boolean>
        '   Apply filtering locations as soft masks
        '   Default = `true'
        ' -lcase_masking
        '   Use lower case filtering in query and subject sequence(s)?
#End Region

#Region "*** Restrict search or results"
        ' -gilist <String>
        '   Restrict search of database to list of GI's
        '    * Incompatible with:  negative_gilist, seqidlist, remote, subject,
        '   subject_loc
        ' -seqidlist <String>
        '   Restrict search of database to list of SeqId's
        '    * Incompatible with:  gilist, negative_gilist, remote, subject,
        '   subject_loc
        ' -negative_gilist <String>
        '   Restrict search of database to everything except the listed GIs
        '    * Incompatible with:  gilist, seqidlist, remote, subject, subject_loc
        ' -entrez_query <String>
        '   Restrict search with the given Entrez query
        '    * Requires:  remote
        ' -db_soft_mask <String>
        '   Filtering algorithm ID to apply to the BLAST database as soft masking
        '    * Incompatible with:  db_hard_mask, subject, subject_loc
        ' -db_hard_mask <String>
        '   Filtering algorithm ID to apply to the BLAST database as hard masking
        '    * Incompatible with:  db_soft_mask, subject, subject_loc
        ' -perc_identity <Real, 0..100>
        '   Percent identity
        ' -culling_limit <Integer, >=0>
        '   If the query range of a hit is enveloped by that of at least this many
        '   higher-scoring hits, delete the hit
        '    * Incompatible with:  best_hit_overhang, best_hit_score_edge
        ' -best_hit_overhang <Real, (>0 and <0.5)>
        '   Best Hit algorithm overhang value (recommended value: 0.1)
        '    * Incompatible with:  culling_limit
        ' -best_hit_score_edge <Real, (>0 and <0.5)>
        '   Best Hit algorithm score edge value (recommended value: 0.1)
        '    * Incompatible with:  culling_limit
        ' -max_target_seqs <Integer, >=1>
        '   Maximum number of aligned sequences to keep 
        '   Not applicable for outfmt <= 4
        '   Default = `500'
        '    * Incompatible with:  num_descriptions, num_alignments
#End Region

#Region "*** Discontiguous MegaBLAST options"
        ' -template_type <String, `coding', `coding_and_optimal', `optimal'>
        '   Discontiguous MegaBLAST template type
        '    * Requires:  template_length
        ' -template_length <Integer, Permissible values: '16' '18' '21' >
        '   Discontiguous MegaBLAST template length
        '    * Requires:  template_type
#End Region


#Region "*** Statistical options"
        ' -dbsize <Int8>
        '   Effective length of the database 
        ' -searchsp <Int8, >=0>
        '   Effective length of the search space
        ' -max_hsps_per_subject <Integer, >=0>
        '   Override maximum number of HSPs per subject to save for ungapped searches
        '   (0 means do not override)
        '   Default = `0'
#End Region

#Region "*** Search strategy options"
        ' -import_search_strategy <File_In>
        '   Search strategy to use
        '    * Incompatible with:  export_search_strategy
        ' -export_search_strategy <File_Out>
        '   File name to record the search strategy used
        '    * Incompatible with:  import_search_strategy
#End Region

#Region "*** Extension options"
        ' -xdrop_ungap <Real>
        '   X-dropoff value (in bits) for ungapped extensions
        ' -xdrop_gap <Real>
        '   X-dropoff value (in bits) for preliminary gapped extensions
        ' -xdrop_gap_final <Real>
        '   X-dropoff value (in bits) for final gapped alignment
        ' -no_greedy
        '   Use non-greedy dynamic programming extension
        ' -min_raw_gapped_score <Integer>
        '   Minimum raw gapped score to keep an alignment in the preliminary gapped and
        '   traceback stages
        ' -ungapped
        '   Perform ungapped alignment only?
        ' -window_size <Integer, >=0>
        '   Multiple hits window size, use 0 to specify 1-hit algorithm
        ' -off_diagonal_range <Integer, >=0>
        '   Number of off-diagonals to search for the 2nd hit, use 0 to turn off
        '   Default = `0'
#End Region

#Region "*** Miscellaneous options"
        ' -parse_deflines
        '   Should the query and subject defline(s) be parsed?
        ' -num_threads <Integer, >=1>
        '   Number of threads (CPUs) to use in the BLAST search
        '   Default = `1'
        '    * Incompatible with:  remote
        ' -remote
        '   Execute search remotely?
        '    * Incompatible with:  gilist, seqidlist, negative_gilist, subject_loc,
        '   num_threads
        '< optionalarguments("-remote" )>public property Remote as   string 
#End Region
    End Class
End Namespace
