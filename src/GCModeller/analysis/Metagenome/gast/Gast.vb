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
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Language.Perl
Imports Microsoft.VisualBasic.Terminal

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

        Public Property verbose As Integer = 0

        <Extension>
        Public Function Invoke(args As ARGV)
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

            '#######################################
            '#
            '# Run the uniques Using mothur
            '#
            '#######################################

            Dim mothur_cmd = $"./mothur ""#unique.seqs(fasta={in_filename});"""
            Call run_command(mothur_cmd)


            '#######################################
            '#
            '# USearch against the reference database
            '#    And parse the output For top hits
            '#
            '#######################################
            '# old version
            '#my $uclust_cmd = "uclust --iddef 3 --input $uniques_filename --lib $ref_filename --uc $uclust_filename --libonly --allhits --maxaccepts $max_accepts --maxrejects $max_rejects --id $min_pctid";

            If (ref_filename IsNot Nothing) Then

                usearch_cmd &= " -db $ref_filename "
            Else
                usearch_cmd &= " -db $udb_filename"
            End If

            '#$usearch_cmd .= " --gapopen 6I/1E --iddef 3 --global --query $uniques_filename --uc $uclust_filename --maxaccepts $max_accepts --maxrejects $max_rejects --id $min_pctid";
            usearch_cmd &= $" -gapopen 6I/1E -usearch_global {uniques_filename} -strand plus -uc {uclust_filename} -maxaccepts {max_accepts} -maxrejects {max_rejects} -id {min_pctid}"

            run_command(usearch_cmd)
            Dim gast_results_ref = parse_uclust(uclust_filename)
        End Function

        Public Function Invoke(args As CommandLine)
            Return New ARGV(args).Invoke
        End Function

        ''' <summary>
        ''' Parse the USearch results And grab the top hit
        ''' </summary>
        ''' <param name="uc_file"></param>
        ''' <returns></returns>
        Public Function parse_uclust(uc_file As String)
            Dim uc_aligns As New Dictionary(Of String, Dictionary(Of String, String))
            Dim refs_at_pctid As New Dictionary(Of String, Dictionary(Of String, String()))
            Dim results

            ' read In the data
            For Each line As String In uc_file.ReadAllLines

                If (Not line.Match("^H").IsBlank) Then

                    ' It has a valid hie
                    ' chomp $line; 

                    '  0=Type, 1=ClusterNr, 2=SeqLength, 3=PctId, 4=Strand, 5=QueryStart, 6=SeedStart, 7=Alignment, 8=Sequence ID, 9= Reference ID
                    Dim data = Regex.Split(line, "\s+")
                    Dim ref = data(9)
                    Dim tax = data(9)
                    ref = Regex.Match(ref, "\|.*$").Value
                    tax = Regex.Match(tax, "^.*\|").Value

                    ' store the alignment for each read / ref
                    uc_aligns(data(8))(ref) = data(7)

                    ' create a look up For refs Of a given pctID For Each read
                    Push(refs_at_pctid(data(8))(data(3)), ref)
                End If
            Next

            '
            ' For Each read
            '
            For Each read As String In uc_aligns.Keys

                Dim found_hit = 0

                ' For Each read, start With the max PctID
                For Each pctid In refs_at_pctid(read).Keys.Sort

                    For Each ref In refs_at_pctid(read)(pctid)

                        '
                        ' Check the alignment For large gaps And/Or remove terminal gaps
                        ' 
                        Dim original_align = uc_aligns(read)(ref)
                        Dim align = original_align ' Use this To remove terminal gaps

                        If ((ignore_terminal_gaps) OrElse (ignore_all_gaps)) Then

                            align = align.Match("^[0-9]*[DI]")
                            align = align.Match("[0-9]*[DI]$")

                        ElseIf (use_full_length) Then

                            align = align.Match("^[0-9]*[I]")
                            align = align.Match("[0-9]*[I]$")
                        End If

                        found_hit = 1

                        ' has internal gaps
                        If ((use_full_length) OrElse (Not ignore_all_gaps)) Then

                            Do While Not align.Match("[DI]").IsBlank

                                align = align.Match("^[0-9]*[M]")  ' leading remove matches 
                                align = align.Match("^[ID]") ' remove singleton indels

                                If align.Match("^[0-9]*[ID]") Then

                                    Dim gap = align
                                    align = align.Match("^[0-9]*[ID]")  ' remove gap from aligment
                                    gap = gap.Match($"[ID]{align}")     ' remove alignment from gap

                                    ' If too large a gap, tends To be indicative Of chimera Or other non-matches
                                    ' Then skip To the Next ref
                                    If (gap > max_gap) Then

                                        If (verbose) Then printf($"Skip {ref} of {read()} at {pctid} pctid for {gap} gap.  \n")
                                        found_hit = 0
                                        last
                                    End If
                                End If
                            Loop
                            If (Not found_hit) Then Continue For   ' don't print this one out.
                        End If

                        '
                        ' convert from percent identity To distance
                        '
                        Dim dist As String = (Int((10 * (100 - pctid)) + 0.5)) / 1000

                        '
                        ' print out the data
                        '
                        If (verbose) Then
                            printf({read, ref, pctid, dist, original_align}.JoinBy(vbTab) & "\n")
                        End If
                        Push(results(read), {ref, dist, original_align})
                    Next

                    ' Don't go to lower PctIDs if found a hit at this PctID.
                    If (found_hit) Then
                        last
                    End If
                Next

            Next

            Return results
        End Function
    End Module
End Namespace