#Region "Microsoft.VisualBasic::f4eba4f2b4247543bf4fba674e521c09, analysis\Metagenome\Metagenome\gast\Gast.vb"

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


' Code Statistics:

'   Total Lines: 528
'    Code Lines: 275 (52.08%)
' Comment Lines: 152 (28.79%)
'    - Xml Docs: 22.37%
' 
'   Blank Lines: 101 (19.13%)
'     File Size: 24.18 KB


'     Module GastAPI
' 
'         Properties: mothur, usearch, verbose
' 
'         Function: assign_taxonomy, Invoke, load_reftaxa, parse_uclust
' 
'         Sub: Invoke
' 
' 
' /********************************************************************************/

#End Region

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

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Language.Perl

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

        Public Property usearch As String = App.HOME & "/gast/usearch.exe"
        Public Property mothur As String = App.HOME & "/gast/mothur.exe"
        Public Property verbose As Boolean = True

        ''' <summary>
        ''' reads a fasta file of trimmed 16S sequences, compares each sequence to
        ''' a set of similarly trimmed ( Or full-length ) 16S reference sequences 
        ''' And assigns taxonomy
        ''' </summary>
        ''' <param name="args"></param>
        <Extension> Public Sub Invoke(args As ARGV)
            Dim log_filename As String = "gast.log"
            Dim in_filename As String = args.in.Replace("\", "/")
            Dim ref_filename As String = args.ref
            Dim udb_filename As String = args.udb
            Dim reftax_filename As String = args.rtax
            Dim out_filename As String = args.out
            Dim terse As Boolean = args.terse

            ' Load into a database variables
            Dim mysqlimport_log As String = "gast.mysqlimport.log"
            Dim mysqlimport_cmd As String = $"mysqlimport -C -v -L -h {args.db_host} {args.db_name} "
            Dim gast_table As String = args.table

            ' USearch variables
            Dim usearch_cmd As String = gast.usearch.CLIPath
            Dim min_pctid = args.minp
            Dim max_accepts = 15
            Dim max_rejects = 200

            ' Parse USearch output file variables
            Dim max_gap As Integer = 10
            Dim ignore_terminal_gaps As Boolean = False  ' only ignore For max gap size, still included In the distance calculations
            Dim ignore_all_gaps As Boolean = True
            Dim save_uclust_file As Boolean = True
            Dim use_full_length As Boolean = args.full

            ' Taxonomy variables
            Dim majority As Double = args.maj

            If ((Not in_filename.FileExists) OrElse
                ((Not ref_filename.FileExists) AndAlso (Not udb_filename.FileExists))) Then
                Throw New Exception("Incorrect number of arguments.")
            End If

            'If (((out_filename IsNot Nothing) AndAlso (gast_table IsNot Nothing)) OrElse ((out_filename Is Nothing) AndAlso (gast_table Is Nothing))) Then
            'Throw New Exception("Please specify either an output file or a database table.")
            'End If

            ' Test validity Of commandline arguments
            If (Not in_filename.FileExists) Then
                Throw New Exception($"Unable to locate input fasta file: {in_filename}.")
            End If

            If ((ref_filename IsNot Nothing) AndAlso (Not ref_filename.FileExists)) Then
                Throw New Exception($"Unable to locate reference fasta file: {ref_filename}.")
            End If

            If ((Not udb_filename.StringEmpty) AndAlso (Not udb_filename.FileExists)) Then
                Throw New Exception($"Unable to locate reference udb file: {udb_filename}.")
            End If

            ' If no outfilename, Then writing To database, create tmp file To store the data
            If (out_filename.StringEmpty) Then
                out_filename = gast_table & (Rnd() * 9999) & ".txt"
            End If

            Using OUT As New StreamWriter(New FileStream(out_filename, FileMode.OpenOrCreate)),
                LOG As New StreamWriter(New FileStream(log_filename, FileMode.OpenOrCreate))

                ' determine the file prefix used by mothur(unique.seqs)
                Dim file_prefix = in_filename.TrimSuffix
                Dim file_suffix = in_filename.Split("."c).Last

                Dim uniques_filename = file_prefix & ".unique." & file_suffix
                Dim names_filename = file_prefix & ".names"
                Dim uclust_filename = uniques_filename & ".uc"

                '#######################################
                '#
                '# Run the uniques Using mothur
                '#
                '#######################################

                Dim mothur_cmd As String = gast.mothur.CLIPath
                mothur &= $" ""#unique.seqs(fasta={in_filename});"""
                '   Call LOG.run_command(mothur_cmd)


                '#######################################
                '#
                '# USearch against the reference database
                '#    And parse the output For top hits
                '#
                '#######################################
                '# old version
                '#my $uclust_cmd = "uclust --iddef 3 --input $uniques_filename --lib $ref_filename --uc $uclust_filename --libonly --allhits --maxaccepts $max_accepts --maxrejects $max_rejects --id $min_pctid";

                If (ref_filename IsNot Nothing) Then
                    usearch_cmd &= $" -db {ref_filename} "
                Else
                    usearch_cmd &= $" -db {udb_filename}"
                End If

                '#$usearch_cmd .= " --gapopen 6I/1E --iddef 3 --global --query $uniques_filename --uc $uclust_filename --maxaccepts $max_accepts --maxrejects $max_rejects --id $min_pctid";
                usearch_cmd &= $" -gapopen 6I/1E -usearch_global {uniques_filename} -strand plus -uc {uclust_filename} -maxaccepts {max_accepts} -maxrejects {max_rejects} -id {min_pctid}"

                '  Call LOG.run_command(usearch_cmd)
                Dim gast_results_ref = parse_uclust(uclust_filename, ignore_terminal_gaps, ignore_all_gaps, use_full_length, max_gap)


                '#######################################
                '#
                '# Calculate consensus taxonomy
                '#
                '#######################################
                LOG.WriteLine("Assigning taxonomy")
                Dim ref_taxa_ref = load_reftaxa(reftax_filename)
                OUT.assign_taxonomy(names_filename, gast_results_ref, ref_taxa_ref, majority, terse, gast_table)

                '#######################################
                '#
                '# Load the file into the database
                '#
                '#######################################
                If (Not gast_table.StringEmpty) Then
                    mysqlimport_cmd &= $"{out_filename} >> {mysqlimport_log}"
                    Call LOG.run_command(mysqlimport_cmd)
                    Call LOG.run_command($"rm {out_filename}")
                End If

                If (Not save_uclust_file) Then
                    Call LOG.run_command($"rm {uclust_filename}")
                End If
            End Using
        End Sub

        ''' <summary>
        ''' get dupes from the names file and calculate consensus taxonomy
        ''' </summary>
        ''' <param name="names_file"></param>
        ''' <param name="results_ref"></param>
        ''' <param name="ref_taxa_ref"></param>
        ''' 
        <Extension>
        Public Function assign_taxonomy(OUT As StreamWriter,
                                         names_file As String,
                                         results_ref As Dictionary(Of String, String()()),
                                         ref_taxa_ref As Dictionary(Of String, String()),
                                         majority As Double,
                                         terse As Boolean,
                                         gast_table As String) As Dictionary(Of String, String()())

            Dim results = results_ref
            Dim ref_taxa = ref_taxa_ref

            ' print the field header lines, but Not If loading table To the database
            If (gast_table.StringEmpty) Then
                If (terse) Then
                    OUT.WriteLine(String.Join(vbTab, "read_id", "taxonomy", "distance", "rank"))
                Else
                    OUT.WriteLine(String.Join(vbTab, "read_id", "taxonomy", "distance", "rank", "refssu_count", "vote", "minrank", "taxa_counts", "max_pcts", "na_pcts", "refhvr_ids"))
                End If
            End If

            For Each line As String In names_file.ReadAllLines

                ' Parse the names information
                ' chomp $line;
                Dim data = Regex.Split(line, "\t")
                Dim dupes = data(1).Split(","c)
                Dim read = data(0)

                Dim taxObjects As Taxonomy() = {}
                Dim refs_for As New Dictionary(Of String, String())

                If (Not results.ContainsKey(read)) Then
                    ' No valid hit in the reference database

                    results(read) = {New String() {"Unknown", "1", "NA", "0", "0", "NA", "0;0;0;0;0;0;0;0", "0;0;0;0;0;0;0;0", "100;100;100;100;100;100;100;100"}}
                    refs_for(read) = {"NA"}
                Else
                    Dim distance As String = ""

                    ' Create an array Of taxonomy objects For all the associated refssu_ids.
                    For i As Integer = 0 To results(read).Length - 1

                        ' %{$results{$read}} Is a hash Of an array Of arrays, so use index $i To Step through Each array, 
                        ' 0= $ref, 1= $dist, 2= $original_align, 3= $taxonomy_of{$ref} ];
                        Dim ref = results(read)(i)(0)

                        ' grab all the taxa assigned To that ref In the database, And add To the taxObjects To be used For consensus
                        For Each t In ref_taxa(ref)
                            Push(taxObjects, New Taxonomy(t))
                        Next

                        ' maintain the list Of references hit
                        If Not refs_for.ContainsKey(read) Then
                            Call refs_for.Add(read, {})
                        End If
                        Push(refs_for(read), results(read)(i)(0))

                        ' should all be the same distance
                        distance = results(read)(i)(1)
                    Next

                    ' Lookup the consensus taxonomy For the array
                    Dim taxReturn = Taxonomy.consensus(taxObjects, majority)

                    ' 0=taxObj, 1=winning vote, 2=minrank, 3=rankCounts, 4=maxPcts, 5=naPcts;
                    Dim taxon = taxReturn(0).TaxonomyString
                    Dim rank = taxReturn(0).depth.ToString

                    If (taxon Is Nothing) Then
                        taxon = "Unknown"
                    End If

                    ' (taxonomy, distance, rank, refssu_count, vote, minrank, taxa_counts, max_pcts, na_pcts)
                    results(read) = {New String() {taxon, distance, rank, taxObjects.Length, taxReturn(1).TaxonomyString, taxReturn(2).TaxonomyString, taxReturn(3).TaxonomyString, taxReturn(4).TaxonomyString, taxReturn(5).TaxonomyString}}
                End If

                ' Replace hash With final taxonomy results, For Each copy Of the sequence
                For Each d In dupes

                    ' @dupes includes the original As well, Not just its copies
                    If (terse) Then

                        OUT.WriteLine(String.Join(vbTab, d, results(read)(0), results(read)(1), results(read)(2)))
                    Else
                        OUT.WriteLine(String.Join(vbTab, {d}.Join(results(read)).Join({String.Join(",", refs_for(read).Sort)}).ToVector))
                    End If
                Next
            Next

            Return results
        End Function

        ''' <summary>
        ''' Get dupes Of the reference sequences And their taxonomy
        ''' </summary>
        ''' <param name="tax_file"></param>
        ''' <returns></returns>
        Public Function load_reftaxa(tax_file As String) As Dictionary(Of String, String())
            Dim taxa As New Dictionary(Of String, String())

            For Each line As String In tax_file.ReadAllLines
                ' chomp $line;

                ' 0=ref_id, 1 = count, 2 = taxa
                Dim data = Regex.Split(line, "\t")
                Dim copies As String() = {}

                ' foreach instance Of that taxa
                For i As Integer = 1 To data(2).ParseInteger

                    ' add that taxonomy To an array
                    Push(copies, data(1))
                Next

                If Not taxa.ContainsKey(data(0)) Then
                    Call taxa.Add(data(0), {})
                End If

                ' add that array To the array Of all taxa For that ref, stored In the taxa hash
                Push(taxa(data(0)), copies)
            Next

            Return taxa
        End Function

        ''' <summary>
        ''' ```bash
        ''' gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file
        ''' ```
        ''' </summary>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Function Invoke(args As CommandLine) As Boolean
            Try
                Call New ARGV(args).Invoke
            Catch ex As Exception
                ex = New Exception(args.ToString, ex)
                Throw ex
            End Try

            Return True
        End Function

        ''' <summary>
        ''' Parse the USearch results And grab the top hit
        ''' </summary>
        ''' <param name="uc_file"></param>
        ''' <returns></returns>
        Public Function parse_uclust(uc_file As String,
                                     ignore_terminal_gaps As Boolean,
                                     ignore_all_gaps As Boolean,
                                     use_full_length As Boolean,
                                     max_gap As Integer) As Dictionary(Of String, String()())

            Dim uc_aligns As New Dictionary(Of String, Dictionary(Of String, String))
            Dim refs_at_pctid As New Dictionary(Of String, Dictionary(Of Double, String()))
            Dim results As New Dictionary(Of String, String()())

            ' read In the data
            For Each line As String In uc_file.ReadAllLines

                If (Not line.Match("^H").StringEmpty) Then

                    ' It has a valid hie
                    ' chomp $line; 

                    '  0=Type, 1=ClusterNr, 2=SeqLength, 3=PctId, 4=Strand, 5=QueryStart, 6=SeedStart, 7=Alignment, 8=Sequence ID, 9= Reference ID
                    Dim data = Regex.Split(line, "\s+")
                    Dim ref = data(9)
                    Dim tax = data(9)
                    ref = Regex.Replace(ref, "\|.*$", "", RegexICMul)
                    tax = Regex.Replace(tax, "^.*\|", "", RegexICMul)

                    ' store the alignment for each read / ref
                    If Not uc_aligns.ContainsKey(data(8)) Then
                        Call uc_aligns.Add(data(8), New Dictionary(Of String, String))
                    End If
                    uc_aligns(data(8))(ref) = data(7)

                    ' create a look up For refs Of a given pctID For Each read
                    If Not refs_at_pctid.ContainsKey(data(8)) Then
                        Call refs_at_pctid.Add(data(8), New Dictionary(Of Double, String()))
                    End If

                    Dim p As Double = Val(data(3))
                    If Not refs_at_pctid(data(8)).ContainsKey(p) Then
                        Call refs_at_pctid(data(8)).Add(p, {})
                    End If
                    Push(refs_at_pctid(data(8))(p), ref)
                End If
            Next

            '
            ' For Each read
            '
            For Each read As String In uc_aligns.Keys

                Dim found_hit As Boolean

                ' For Each read, start With the max PctID
                For Each pctid In refs_at_pctid(read).Keys.Sort

                    For Each ref In refs_at_pctid(read)(pctid)

                        '
                        ' Check the alignment For large gaps And/Or remove terminal gaps
                        ' 
                        Dim original_align = uc_aligns(read)(ref)
                        Dim align = original_align ' Use this To remove terminal gaps

                        If ((ignore_terminal_gaps) OrElse (ignore_all_gaps)) Then

                            align = Regex.Replace(align, "^[0-9]*[DI]", "", RegexICMul)
                            align = Regex.Replace(align, "[0-9]*[DI]$", "", RegexICMul)

                        ElseIf (use_full_length) Then

                            align = Regex.Replace(align, "^[0-9]*[I]", "", RegexICMul)
                            align = Regex.Replace(align, "[0-9]*[I]$", "", RegexICMul)
                        End If

                        found_hit = True

                        ' has internal gaps
                        If ((use_full_length) OrElse (Not ignore_all_gaps)) Then

                            Do While Not align.Match("[DI]").StringEmpty

                                align = Regex.Replace(align, "^[0-9]*[M]", "", RegexICMul)  ' leading remove matches 
                                align = Regex.Replace(align, "^[ID]", "", RegexICMul) ' remove singleton indels

                                If Not align.Match("^[0-9]*[ID]").StringEmpty Then

                                    Dim gap = align
                                    align = Regex.Replace(align, "^[0-9]*[ID]", "", RegexICMul)  ' remove gap from aligment
                                    gap = Regex.Replace(gap, $"[ID]{align}", "", RegexICMul)     ' remove alignment from gap

                                    ' If too large a gap, tends To be indicative Of chimera Or other non-matches
                                    ' Then skip To the Next ref
                                    If (gap > max_gap) Then

                                        If (verbose) Then
                                            STDIO.printf($"Skip {ref} of {read} at {pctid} pctid for {gap} gap.  \n")
                                        End If

                                        found_hit = False
                                        Exit Do
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
                            STDIO.printf({read, ref, CStr(pctid), dist, original_align}.JoinBy(vbTab) & "\n")
                        End If
                        If Not results.ContainsKey(read) Then
                            Call results.Add(read, {})
                        End If
                        Push(results(read), {ref, dist, original_align})
                    Next

                    ' Don't go to lower PctIDs if found a hit at this PctID.
                    If (found_hit) Then
                        Exit For
                    End If
                Next

            Next

            Return results
        End Function
    End Module
End Namespace
