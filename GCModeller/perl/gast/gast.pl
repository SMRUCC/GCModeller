#!/usr/bin/env perl

#########################################
#
# gast: compares trimmed sequences against a reference database for assigning taxonomy
#
# Author: Susan Huse, shuse@mbl.edu
#
# Date: Fri Feb 25 11:41:10 EST 2011
#
# Copyright (C) 2011 Marine Biological Laborotory, Woods Hole, MA
# 
# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU General Public License
# as published by the Free Software Foundation; either version 2
# of the License, or (at your option) any later version.
# 
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
# 
# For a copy of the GNU General Public License, write to the Free Software
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
# or visit http://www.gnu.org/copyleft/gpl.html
#
# Keywords: gast taxonomy refssu 
# 
# Assumptions: 
#
# Revisions:
#
# Programming Notes:
#
########################################
use strict;
use warnings;
#use lib "/class/stamps-software/bin";
use Taxonomy;

#######################################
#
# Set up usage statement
#
#######################################
my $script_help = "
 gast - reads a fasta file of trimmed 16S sequences, compares each sequence to
        a set of similarly trimmed ( or full-length ) 16S reference sequences and assigns taxonomy
\n";

my $usage = "
   Usage:  gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file

      ex:  gast -in mydata.trimmed.fa -ref refv3v5 -rtax refv3v5.tax -out mydata.gast
           gast -in mydata.trimmed.fa -ref refv3v5 -rtax refv3v5.tax -mp 0.80 -m 100 -out mydata.gast

 Options:  
            -in     input fasta file
            -ref    reference fasta file containing unique sequences of known taxonomy
                   The definition line should include the ID used in the reference taxonomy file.
                   Any other information on the definition line should be separated by a space or a \"|\" symbol.
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
            -maj    percent majority required for taxonomic consensus [default: 66]
            -full   input data will be compared against full length 16S reference sequences [default: not full length]
            
      Optional database output
           -host   mysql server host name
           -db     database name
           -table  database table to receive data

\n";

#######################################
#
# Definition statements
#
#######################################
my $verbose = 0;
my $self_cmd = join(" ", $0, @ARGV);
my $log_filename = "gast.log";
my $in_filename;
my $in_prefix;
my $ref_filename = '';
my $udb_filename = '';
my $reftax_filename;
my $out_filename;
my $terse = 0;

# Load into a database variables
my $db_host = 'newbpcdb2';
my $db_name = 'env454';
my $mysqlimport_log = "gast.mysqlimport.log";
my $mysqlimport_cmd = "mysqlimport -C -v -L -h $db_host $db_name ";
my $gast_table;

# USearch variables
my $usearch_cmd = "./usearch6.0";
my $min_pctid = 0.80;
my $max_accepts = 15;
my $max_rejects = 200;

# Parse USearch output file variables
my $max_gap = 10;
my $ignore_terminal_gaps = 0;  # only ignore for max gap size, still included in the distance calculations
my $ignore_all_gaps = 0;
my $save_uclust_file = 0;
my $use_full_length = 0;

# Taxonomy variables
my $majority = 66;


#######################################
#
# Test for commandline arguments
#
#######################################

if (! $ARGV[0] ) 
{
	print $script_help;
	print $usage;
	exit -1;
} 

while ((scalar @ARGV > 0) && ($ARGV[0] =~ /^-/)) 
{
	if ($ARGV[0] =~ /-h/) 
	{
		print $script_help;
		print $usage;
		exit 0;
	} elsif ($ARGV[0] eq "-in") {
		shift @ARGV;
		$in_filename = shift @ARGV;
	} elsif ($ARGV[0] eq "-out") {
		shift @ARGV;
		$out_filename = shift @ARGV;
	} elsif ($ARGV[0] eq "-ref") {
		shift @ARGV;
		$ref_filename = shift @ARGV;
	} elsif ($ARGV[0] eq "-udb") {
		shift @ARGV;
		$udb_filename = shift @ARGV;
	} elsif ($ARGV[0] eq "-rtax") {
		shift @ARGV;
		$reftax_filename = shift @ARGV;
	} elsif ($ARGV[0] eq "-terse") {
		shift @ARGV;
		$terse = 1;
	} elsif ($ARGV[0] eq "-minp") {
		shift @ARGV;
		$min_pctid = shift @ARGV;
	} elsif ($ARGV[0] eq "-maxg") {
		shift @ARGV;
		$max_gap = shift @ARGV;
	} elsif ($ARGV[0] eq "-maj") {
		shift @ARGV;
		$max_gap = shift @ARGV;
	} elsif ($ARGV[0] eq "-full") {
		shift @ARGV;
		$use_full_length = 1;
	} elsif ($ARGV[0] eq "-termg") {
		$ignore_terminal_gaps = 1;
		shift @ARGV;
	} elsif ($ARGV[0] eq "-ignoregaps") {
		$ignore_all_gaps = 1;
		shift @ARGV;
	} elsif ($ARGV[0] eq "-db") {
		shift @ARGV;
		$db_name = shift @ARGV;
	} elsif ($ARGV[0] eq "-host") {
		shift @ARGV;
		$db_host = shift @ARGV;
	} elsif ($ARGV[0] eq "-table") {
		shift @ARGV;
		$gast_table = shift @ARGV;
	} elsif ($ARGV[0] eq "-saveuc") {
		$save_uclust_file = 1;
		shift @ARGV;
	} elsif ($ARGV[0] eq "-v") {
		$verbose = 1;
		shift @ARGV;
	} elsif ($ARGV[0] =~ /^-/) { #unknown parameter, just get rid of it
		print "Unknown commandline flag \"$ARGV[0]\".\n";
		print $usage;
		exit -1;
	}
}


#######################################
#
# Parse commandline arguments, ARGV
#
#######################################

if ( (! $in_filename) || ( (! $ref_filename) && (! $udb_filename) ) ) 
{
	print "Incorrect number of arguments.\n";
	print "$usage\n";
	exit;
} 

if ( ( ($out_filename) && ($gast_table) ) || ( (! $out_filename) && (! $gast_table) ) )
{
	print "Please specify either an output file or a database table.\n";
	print "$usage\n";
	exit;
} 

#Test validity of commandline arguments
if (! -f $in_filename) 
{
	print "Unable to locate input fasta file: $in_filename.\n";
	exit -1;
}

if ( ($ref_filename) && (! -f $ref_filename) )
{
	print "Unable to locate reference fasta file: $ref_filename.\n";
	exit -1;
}

if ( ($udb_filename) && (! -f $udb_filename) )
{
	print "Unable to locate reference udb file: $udb_filename.\n";
	exit -1;
}


#######################################
#
# Create all the necessary file names
#
#######################################
open(LOG, ">$log_filename") || die("Unable to write to output log file: $log_filename.  Exiting.\n");
print LOG "$self_cmd (" . (localtime) . ")\n";

# if no outfilename, then writing to database, create tmp file to store the data
if (! $out_filename) 
{
    $out_filename = $gast_table . int(rand(9999)) . ".txt";
}

open(OUT, ">$out_filename") || die("Unable to write to output file: $out_filename.  Exiting.\n");


# determine the file prefix used by mothur(unique.seqs)
my $file_prefix = $in_filename;
my $file_suffix = $in_filename;
$file_prefix =~ s/(^.*)\..*$/$1/;
$file_suffix =~ s/^.*\.//;
my $uniques_filename = $file_prefix . ".unique." . $file_suffix;
my $names_filename = $file_prefix . ".names";
my $uclust_filename = $uniques_filename . ".uc";

#######################################
#
# Run the uniques using mothur
#
#######################################

my $mothur_cmd = "./mothur \"#unique.seqs(fasta=$in_filename);\"";
run_command($mothur_cmd);

#######################################
#
# USearch against the reference database
#    and parse the output for top hits
#
#######################################
# old version
#my $uclust_cmd = "uclust --iddef 3 --input $uniques_filename --lib $ref_filename --uc $uclust_filename --libonly --allhits --maxaccepts $max_accepts --maxrejects $max_rejects --id $min_pctid";

if ($ref_filename) 
{
    $usearch_cmd .= " -db $ref_filename ";
} else {
    $usearch_cmd .= " -db $udb_filename";
}
#$usearch_cmd .= " --gapopen 6I/1E --iddef 3 --global --query $uniques_filename --uc $uclust_filename --maxaccepts $max_accepts --maxrejects $max_rejects --id $min_pctid";
$usearch_cmd .= " -gapopen 6I/1E -usearch_global $uniques_filename -strand plus -uc $uclust_filename -maxaccepts $max_accepts -maxrejects $max_rejects -id $min_pctid";

run_command($usearch_cmd);
my $gast_results_ref = parse_uclust($uclust_filename);


#######################################
#
# Calculate consensus taxonomy
#
#######################################
print LOG "Assigning taxonomy\n";
my $ref_taxa_ref = load_reftaxa($reftax_filename);
assign_taxonomy($names_filename, $gast_results_ref, $ref_taxa_ref);

#######################################
#
# Load the file into the database
#
#######################################
if ($gast_table)
{
    $mysqlimport_cmd .= "$out_filename >> $mysqlimport_log";
    run_command($mysqlimport_cmd);
    run_command("rm $out_filename");
}

if (! $save_uclust_file)
{
    run_command("rm $uclust_filename");
}
exit;

########################## SUBROUTINES #######################################


############################################################
#
# Subroutine: run_command
#       run system commands
#
# #########################################################
sub run_command
{
    my $command = shift;
    print LOG "$command\n";
    if ($verbose) {print "$command\n";}
    my $command_err = system($command);
    if ($command_err) 
    {
        my $err_msg = "Error $command_err encountered while running: \"$command\".  Exiting.";
        print LOG "$err_msg\n";
        warn "$err_msg\n";
        exit;
    }
}

#######################################
#
# Subroutine: parse_uclust
#       Parse the USearch results and grab the top hit
#
#######################################
sub parse_uclust
{
    my $uc_file = shift;
    my %uc_aligns;
    my %refs_at_pctid;
    my %results;
    
    # read in the data
    open(UC, $uc_file);
    while (my $line = <UC>)
    {
        if ($line =~ /^H/)
        {
            #It has a valid hie
            chomp $line; 
    
            #  0=Type, 1=ClusterNr, 2=SeqLength, 3=PctId, 4=Strand, 5=QueryStart, 6=SeedStart, 7=Alignment, 8=Sequence ID, 9= Reference ID
            my @data = split(/\s+/, $line);
            my $ref = $data[9];
            my $tax = $data[9];
            $ref =~ s/\|.*$//;
            $tax =~ s/^.*\|//;
    
            # store the alignment for each read / ref
            $uc_aligns{$data[8]}{$ref} = $data[7];
    
            # create a look up for refs of a given pctID for each read
            push (@{$refs_at_pctid{$data[8]}{$data[3]}}, $ref);

        }
    }
    
    #
    # For each read
    #
    foreach my $read (keys %uc_aligns)
    {
        my $found_hit = 0;
    
        # for each read, start with the max PctID
        foreach my $pctid ( sort {$b<=>$a} keys %{$refs_at_pctid{$read}} )
        {
            foreach my $ref ( @{$refs_at_pctid{$read}{$pctid}})
            {
                #
                # Check the alignment for large gaps and/or remove terminal gaps
                # 
                my $original_align = $uc_aligns{$read}{$ref};
                my $align = $original_align; # Use this to remove terminal gaps
    
                if ( ($ignore_terminal_gaps) || ($ignore_all_gaps) )
                {
                    $align =~ s/^[0-9]*[DI]//;
                    $align =~ s/[0-9]*[DI]$//;
                }
                elsif($use_full_length)
                {
                    $align =~ s/^[0-9]*[I]//;
                    $align =~ s/[0-9]*[I]$//;
                }
        
                $found_hit = 1;

                # has internal gaps
                if ( ($use_full_length) || (! $ignore_all_gaps) ) 
                {
                    while ($align =~ /[DI]/)
                    {
                        $align =~ s/^[0-9]*[M]//;  # leading remove matches 
                        $align =~ s/^[ID]//; # remove singleton indels
        
                        if ($align =~ /^[0-9]*[ID]/)
                        {
                            my $gap = $align;
                            $align =~ s/^[0-9]*[ID]//;  # remove gap from aligment
                            $gap =~ s/[ID]$align//;     # remove alignment from gap
    
                            # if too large a gap, tends to be indicative of chimera or other non-matches
                            # then skip to the next ref
                            if ($gap > $max_gap) 
                            {
                                if ($verbose) { print "Skip $ref of $read at $pctid pctid for $gap gap.  \n";} 
                                $found_hit = 0;
                                last; 
                            }
                        }
                    }
                    if (! $found_hit) {next;} #don't print this one out.
                }
    
                #
                # convert from percent identity to distance
                #
                my $dist = (int((10* (100 -$pctid)) + 0.5)) / 1000; 
        
                #
                # print out the data
                #
                if ($verbose) {print join ("\t", $read, $ref, $pctid, $dist, $original_align) . "\n"; }
                push ( @{$results{$read}},  [ $ref, $dist, $original_align] ) ;
            }
    
            # Don't go to lower PctIDs if found a hit at this PctID.
            if ($found_hit) {last;} 
        }

    }
    return \%results;
}

#######################################
#
# Subroutine: load_reftaxa
#       get dupes of the reference sequences and their taxonomy
#
#######################################
sub load_reftaxa
{
    my $tax_file = shift;
    my %taxa;
    open(TAX, "<$tax_file") || die ("Unable to open reference taxonomy file: $tax_file.  Exiting\n");
    while (my $line = <TAX>) 
    {
        chomp $line;

        # 0=ref_id, 1 = count, 2 = taxa
        my @data = split(/\t/, $line);

        my @copies;

        # foreach instance of that taxa
        for my $i (1 .. $data[2])
        {
            # add that taxonomy to an array
            push(@copies, $data[1]);
        }

        # add that array to the array of all taxa for that ref, stored in the taxa hash
        push (@{$taxa{$data[0]}}, @copies);
     }

     return \%taxa;
} 

#######################################
#
# Subroutine: assign_taxonomy
#       get dupes from the names file and calculate consensus taxonomy
#
#######################################
sub assign_taxonomy
{
    my $names_file = shift;
    my $results_ref = shift;
    my %results = %$results_ref;
    my $ref_taxa_ref = shift;
    my %ref_taxa = %$ref_taxa_ref;

    # print the field header lines, but not if loading table to the database
    if (! $gast_table)
    {
        if ($terse) 
        {
            print OUT join("\t", "read_id", "taxonomy", "distance", "rank") . "\n";
        } else {
            print OUT join("\t", "read_id", "taxonomy", "distance", "rank", "refssu_count", "vote", "minrank", "taxa_counts", "max_pcts", "na_pcts", "refhvr_ids") . "\n";
        }
    }
    open(NAMES, "<$names_file") || die ("Unable to open names file: $names_file.  Exiting\n");
    
    while (my $line = <NAMES>)
    {
    
        # Parse the names information
        chomp $line;
        my @data = split(/\t/, $line);
        my @dupes = split(/,/, $data[1]);
        my $read = $data[0];
    
        my @taxObjects;
        my $distance;
        my %refs_for;
    
        if (! exists $results{$read})
        {
            # No valid hit in the reference database

            $results{$read} = ["Unknown", 1, "NA", 0, 0, "NA", "0;0;0;0;0;0;0;0", "0;0;0;0;0;0;0;0", "100;100;100;100;100;100;100;100"];
            $refs_for{$read} = [ "NA" ];
        } else {

            # Create an array of taxonomy objects for all the associated refssu_ids.
            for my $i ( 0 .. $#{$results{$read}} )
            {
                # %{$results{$read}} is a hash of an array of arrays, so use index $i to step through each array, 
                ### 0= $ref, 1= $dist, 2= $original_align, 3= $taxonomy_of{$ref} ];
                my $ref = $results{$read}[$i][0];

                # grab all the taxa assigned to that ref in the database, and add to the taxObjects to be used for consensus
                foreach my $t (@{$ref_taxa{$ref}})
                {
                    push ( @taxObjects, Taxonomy->new($t) ); 
                }

                # maintain the list of references hit
                push ( @{$refs_for{$read}}, $results{$read}[$i][0] );
                
                # should all be the same distance
                $distance = $results{$read}[$i][1];
            }

            # Lookup the consensus taxonomy for the array
            my @taxReturn = Taxonomy->consensus(@taxObjects, $majority);

            # 0=taxObj, 1=winning vote, 2=minrank, 3=rankCounts, 4=maxPcts, 5=naPcts;
            my $taxon = $taxReturn[0]->taxstring;
            my $rank = $taxReturn[0]->depth;
            if (! $taxon) {$taxon = "Unknown";}
        
            # (taxonomy, distance, rank, refssu_count, vote, minrank, taxa_counts, max_pcts, na_pcts)
            $results{$read} = [ $taxon, $distance, $rank, scalar @taxObjects, $taxReturn[1], $taxReturn[2], $taxReturn[3], $taxReturn[4], $taxReturn[5] ] ;
        }

        # Replace hash with final taxonomy results, for each copy of the sequence
        for my $d (@dupes)
        {
            # @dupes includes the original as well, not just its copies
            if ($terse) 
            {
                print OUT join("\t", $d, $results{$read}[0], $results{$read}[1], $results{$read}[2]) . "\n";
            } else {
                print OUT join("\t", $d, @{$results{$read}}, join(",", sort @{$refs_for{$read}})) . "\n";
            }
        }

    }
    close(NAMES);
    close(OUT);
    return (\%results);
}

