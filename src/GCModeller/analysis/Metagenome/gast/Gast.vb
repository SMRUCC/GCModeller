'#!/usr/bin/env perl

'#########################################
'#
'# gast: compares trimmed sequences against a reference database For assigning taxonomy
'#
'# Author: Susan Huse, shuse@mbl.edu
'#
'# Date: Fri Feb 25 11:41:10 EST 2011
'#
'# Copyright (C) 2011 Marine Biological Laborotory, Woods Hole, MA
'# 
'# This program Is free software; you can redistribute it And/Or
'# modify it under the terms Of the GNU General Public License
'# As published by the Free Software Foundation; either version 2
'# Of the License, Or (at your Option) any later version.
'# 
'# This program Is distributed In the hope that it will be useful,
'# but WITHOUT ANY WARRANTY; without even the implied warranty Of
'# MERCHANTABILITY Or FITNESS For A PARTICULAR PURPOSE.  See the
'# GNU General Public License For more details.
'# 
'# For a copy Of the GNU General Public License, write To the Free Software
'# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
'# Or visit http://www.gnu.org/copyleft/gpl.html
'#
'# Keywords: gast taxonomy refssu 
'# 
'# Assumptions: 
'#
'# Revisions:
'#
'# Programming Notes:
'#
'########################################
'use strict;
'use warnings;
'#use Lib "/class/stamps-software/bin";
'use Taxonomy;

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace gast

    ''' <summary>
    ''' compares trimmed sequences against a reference database for assigning taxonomy, reads 
    ''' a fasta file of trimmed 16S sequences, compares each sequence to a Set Of similarly 
    ''' trimmed ( Or full-length ) 16S reference sequences And assigns taxonomy
    ''' </summary>
    Public Module GastAPI

#Region "Set up usage statement"

        Const script_help = "
 gast - reads a fasta file of trimmed 16S sequences, compares each sequence to
        a set of similarly trimmed ( or full-length ) 16S reference sequences and assigns taxonomy
"

        Const usage = "
   Usage:  gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file

      ex:  gast -in mydata.trimmed.fa -ref refv3v5 -rtax refv3v5.tax -out mydata.gast
           gast -in mydata.trimmed.fa -ref refv3v5 -rtax refv3v5.tax -mp 0.80 -m 100 -out mydata.gast

 Options:  
            -in     input fasta file
            -ref    reference fasta file containing unique sequences of known taxonomy
                    The definition line should include the ID used in the reference taxonomy file.
                    Any other information on the definition line should be separated by a space or a ""|"" symbol.
            -wdb    use a USearch formatted wdb indexed version of the reference for speed. [NO LONGER AVAILABLE with usearch6.0+]
            -udb    use a USearch formatted udb indexed version of the reference for speed. (see http://drive5.com/usearch/manual/udb_files.html)
            -rtax   reference taxa file with taxonomy for all copies of the sequences in the reference fasta file
                    This is a tab-delimited file, three columns, describing the taxonomy of the reference sequences
                    The ID matching the reference fasta, the taxonomy and the number of reference sequences with this 
                    this same taxonomy.  
            -out    output filename
            -terse  minimal output, includes only ID, taxonomy, and distance
                    See GAST manual for description of other fields

            -minp   [Optional] minimum percent identity match to a reference.
                     If the best match is less then min_pct_id, it is not considered a match
                     default = 0.80
            -maxa   [Optional] usearch --max_accepts parameter [default: 15]
            -maxr   [Optional] usearch --max_rejects parameter [default: 200]
            -maj     percent majority required for taxonomic consensus [default: 66]
            -full    input data will be compared against full length 16S reference sequences [default: not full length]
            
      Optional database output
           -host   mysql server host name
           -db     database name
           -table  database table to receive data

"
#End Region

        ''' <summary>
        ''' gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file
        ''' </summary>
        Public Structure ARGV

            ''' <summary>
            ''' input_fasta
            ''' </summary>
            ''' <returns></returns>
            Public Property [in] As String
            ''' <summary>
            ''' reference_uniques_fasta
            ''' </summary>
            ''' <returns></returns>
            Public Property ref As String
            ''' <summary>
            ''' reference_dupes_taxonomy 
            ''' </summary>
            ''' <returns></returns>
            Public Property rtax As String
            ''' <summary>
            ''' [min_pct_id] 
            ''' </summary>
            ''' <returns></returns>
            Public Property mp As String
            ''' <summary>
            ''' [majority] 
            ''' </summary>
            ''' <returns></returns>
            Public Property m As String
            ''' <summary>
            ''' output_file
            ''' </summary>
            ''' <returns></returns>
            Public Property out As String

            Sub New(args As CommandLine)
                [in] = args - "-in"
                ref = args - "-ref"
                rtax = args - "-rtax"
                mp = args - "-mp"
                m = args - "-m"
                out = args - "-out"
            End Sub

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function
        End Structure

        <Extension>
        Public Function Invoke(args As ARGV)
            Dim verbose = 0
            Dim log_filename = "gast.log"
            Dim in_filename = args.in
            Dim in_prefix
            Dim ref_filename = ""
            Dim udb_filename = ""
            Dim reftax_filename
            Dim out_filename = args.out
            Dim terse = 0

            ' Load into a database variables
            Dim db_host = "newbpcdb2"
            Dim db_name = "env454"
            Dim mysqlimport_log = "gast.mysqlimport.log"
            Dim mysqlimport_cmd = "mysqlimport -C -v -L -h $db_host $db_name "
            Dim gast_table

            ' USearch variables
            Dim usearch_cmd = "./usearch6.0"
            Dim min_pctid = 0.8
            Dim max_accepts = 15
            Dim max_rejects = 200

            ' Parse USearch output file variables
            Dim max_gap = 10
            Dim ignore_terminal_gaps = 0 ' only ignore For max gap size, still included In the distance calculations
            Dim ignore_all_gaps = 0
            Dim save_uclust_file = 0
            Dim use_full_length = 0

            ' Taxonomy variables
            Dim majority = 66

            If ((in_filename Is Nothing) OrElse ((ref_filename Is Nothing) AndAlso (udb_filename Is Nothing))) Then
                Throw New Exception("Incorrect number of arguments.")
            End If

            If (((out_filename Is Nothing) AndAlso (gast_table Is Nothing)) OrElse ((out_filename Is Nothing) AndAlso (gast_table Is Nothing))) Then
                Throw New Exception("Please specify either an output file or a database table.")
            End If

            ' Test validity Of commandline arguments
            If (Not in_filename.FileExists) Then
                Throw New Exception($"Unable to locate input fasta file: {in_filename}.")
            End If

            If ((ref_filename Is Nothing) AndAlso (Not ref_filename.FileExists)) Then
                Throw New Exception($"Unable to locate reference fasta file: {ref_filename}.")
            End If

            If ((udb_filename Is Nothing) AndAlso (Not udb_filename.FileExists)) Then
                Throw New Exception($"Unable to locate reference udb file: {udb_filename}.")
            End If

            ' If no outfilename, Then writing To database, create tmp file To store the data
            If (out_filename Is Nothing) Then
                out_filename = gast_table & (RandomDouble() * 9999) & ".txt"
            End If

            ' determine the file prefix used by mothur(unique.seqs)
            Dim file_prefix = in_filename.TrimFileExt
            Dim file_suffix = in_filename.Split("."c).Last

            Dim uniques_filename = file_prefix & ".unique." & file_suffix
            Dim names_filename = file_prefix & ".names"
            Dim uclust_filename = uniques_filename & ".uc"
        End Function

        Public Function Invoke(args As CommandLine)
            Return New ARGV(args).Invoke
        End Function
    End Module
End Namespace