#!/usr/bin/perl

use warnings;
use strict;
use Getopt::Long;

#use Bio::SearchIO;
use Bio::SeqIO;
#use Bio::FeatureIO;
use Bio::SeqFeature::Generic;
use Bio::Tools::GFF;
use IO::File;

my( $local, 
    $global,
    $blastdb,
    $thresh,
    $noclean,
    $help,
    $outfile,
    $nobig,
    @exclude,
    $verbose,
    );

my $VERSION = '1.0';
my $blastcut = 0.01;
my $outputfmt = 'gff';
my $filter = 'ncbi';
my $starttime = `date`;
chomp $starttime;
my $cmdline = $0.' '.join( ' ', @ARGV );

&GetOptions( "local"         => \$local,
	     "global"        => \$global,
	     "blastdb=s"     => \$blastdb,
	     "t=s"           => \$thresh,
	     "bt=s"          => \$blastcut,
#	     "f=s"           => \$outputfmt,
	     "h"             => \$help,
	     "nobig"         => \$nobig,
	     "exclude=s@"    => \@exclude,
	     "noclean"       => \$noclean,
	     "filter=s"      => \$filter,
	     "o=s"           => \$outfile,
	     "v"             => \$verbose,
	     );

my $cmfile = shift;
my $fafile = shift;

if( $help or not $fafile ) {
    &help();
    exit(1);
}

sub help {
    print STDERR <<EOF;

$0: search a DNA fasta file against Rfam

Usage: $0 <options> cm_file fasta_file
    Options
        -h              : show this help
	-o <file>       : write the output to <file>
	-blastdb <file> : use Rfam blast database for speed
                          (otherwise do full slow CM search)

    Expert options
	-t <bits>       : specify cutoff in bits
	--bt <bits>     : specify blast evalue cutoff
	--local         : perform local mode search
	--global        : perform global mode search
	--nobig         : skip the large ribosomal RNAs
	--exclude [acc] : exlude family [acc] from the search
	--filter [ncbi|wu] : use wublast/ncbiblast (default ncbi)
        
EOF
}

if( $global or $local ) {
    print STDERR <<EOF;
WARN: Global or local model specified on the command line -- overwriting
model-specific mode.  The curated Rfam thresholds may not be meaningful.
EOF
}

if( !$blastdb ) {
    print STDERR <<EOF;
WARN: No BLAST database specified.  Your search may be extremely slow.
Also, the model-specific global/local mode is not used, therefore
curated Rfam thresholds may not be meaningful.
EOF
}

my %exclude = ();
if( $nobig ) {
    # accessions of big ribosomal RNAs
    %exclude = ( "RF00177" => 1,
		 "RF00028" => 1,
		 "RF00029" => 1,
	);
}
foreach my $acc ( @exclude ) {
    $exclude{$acc} = 1;
}

print STDERR "read fasta file\n" if( $verbose );
my $seqs;
my $in = Bio::SeqIO -> new( -file => $fafile, 
			    -format => 'Fasta' );
while( my $seq = $in->next_seq() ) {
    $seqs->{ $seq->id() } = $seq;
}

print STDERR "read CM library\n" if( $verbose );
my $cmfh = IO::File->new( $cmfile );
my $cm = read_cm_library( $cmfh );

my $features;
if( $blastdb ) {
    print STDERR "run blast pre-filter\n" if( $verbose );
    my $resfile = run_blast_pre_filter();
    print STDERR "parse blast results\n" if( $verbose );
    my $results = parse_blast_table( $resfile );

    print STDERR "run infernal search\n" if( $verbose );
    $features = run_multi_infernal_search( $cmfile, $results );
}
else {
    print STDERR "run infernal search\n" if( $verbose );
    my $resfile = run_infernal_search( $cmfile, $fafile );
    print STDERR "parse infernal results\n" if( $verbose );
    $features = parse_infernal_table( $resfile );
}

my $outfh = \*STDOUT;
if( $outfile ) {
    $outfh = IO::File->new( ">$outfile" );
}

if( $outputfmt eq 'gff' ) {
    my $endtime = `date`;
    chomp $endtime;
    print $outfh <<EOF
##gff-version 3
# rfam_scan.pl (v$VERSION)
# command line:     $cmdline
# CM file:          $cmfile
# query FASTA file: $fafile
# start time:       $starttime
# end time:         $endtime
EOF
}

my %counts;
foreach my $f ( sort { $b->score <=> $a->score } @{$features} ) {
    if( $outputfmt eq 'gff' ) {
	my ($rfamid) = $f->get_tag_values('rfam-id');
	$counts{$rfamid}++;
	$f->add_tag_value( 'id', $rfamid.'.'.$counts{$rfamid} );
	$f->gff_format( Bio::Tools::GFF->new(-gff_version => 3) );
	print $outfh $f->gff_string, "\n";
    }
}

cleanup() if( !$noclean );

exit;


####

sub run_multi_infernal_search {
    my $cmfile = shift;
    my $results = shift;

    my @f;
    # loop over families from blast results
    foreach my $acc ( keys %{$results} ) {
	if( exists $exclude{$acc} ) { 
	    next;   # skip anything we're asked to
	}

	my $out = Bio::SeqIO->new( -file => ">/tmp/$$.seq",
				   -format => 'Fasta' );
	
	my $rfamid;
	foreach my $seqid ( keys %{ $results->{$acc} } ) {
	    foreach my $hit ( @{ $results->{$acc}->{$seqid} } ) {
		$rfamid = $hit->{-id};
		my( $start, $end ) = ( $hit->{-start},
				       $hit->{-end},
				       );
		my $newseq = $seqs->{$seqid}->trunc( $start, $end );
		$newseq->display_id( "$seqid/$start-$end" );
		$out->write_seq( $newseq );

		print STDERR "searching [$seqid/$start-$end] with [$rfamid]\n" if( $verbose );
	    }
	}
	$out->close;
	die "FATAL: can't find a file I've written in /tmp [$$.seq]" if( not -s "/tmp/$$.seq" );
    
	my $cmfile = get_cm_from_id( $cmfh, $rfamid );
	my $resfile = run_infernal_search( $cmfile, "/tmp/$$.seq", $rfamid );
	my $feat = parse_infernal_table( $resfile );
	push( @f, @{$feat} );
    }
    return \@f;
}


sub cleanup {
    unlink "/tmp/$$.res";
    unlink "/tmp/$$.blast";
    unlink "/tmp/$$.seq";
    unlink "/tmp/$$.cm";
}


sub run_blast_pre_filter {
    my $blastcmd;
    if( $filter =~ /ncbi/ ) {
	$blastcmd = "blastall -p blastn -i $fafile -d $blastdb -e $blastcut -W7 -F F -b 1000000 -v 1000000 -m 8";
    }
    elsif( $filter =~ /wu/ ) {
	$blastcmd = "wublastn $blastdb $fafile -e $blastcut W=7 B=1000000 V=1000000 -hspmax 0 -gspmax 0 -kap -mformat 2";
    }
    my $outfile = "/tmp/$$.blast";
    system "$blastcmd > $outfile" and die "FATAL: failed to run blast [$blastcmd]\n";
    return $outfile;
}


sub run_infernal_search {
    my $cmfile = shift;
    my $fafile = shift;
    my $rfamid = shift;
    my $options = "";

    if( defined $thresh ) {
	$options = "-T $thresh";
    }
    else {
	$options = " --ga";
    }

    if( $global ) {
	$options .= " -g";
    }
    elsif( $local ) {
	# default in infernal 1.0
    }
    elsif( $cm->{$rfamid}->{-options} ) {
	$options .= " ".$cm->{$rfamid}->{-options};
    }

#    system "cmsearch $options $cmfile $fafile > /tmp/$$.res" and die;
    system "cmsearch --tabfile /tmp/$$.res $options $cmfile $fafile > /dev/null" and die;
#    system "cat /tmp/$$.res";
    return "/tmp/$$.res";
}



sub parse_infernal_table {
    my $file = shift;
    my $fh = IO::File->new( $file );
    my @f;
    my $rfamid;
    while(<$fh>) {
	if( /^\#\s+CM:\s+(\S+)/ ) {
	    $rfamid = $1;
	}
	next if( /^\#/ );
	if( my( $seqid, $start, $end, $modst, $moden, $bits, $evalue, $gc ) =
	    /^\s*(\S+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\S+)\s+(\S+)\s+(\d+)/ ) {

	    my $strand = 1;
	    if( $end < $start ) {
		( $start, $end ) = ( $end, $start );
		$strand = -1;
	    }

	    # recalculate start and end if name/start-end
	    if( my( $n,$s,$e) = $seqid =~ /^(\S+)\/(\d+)-(\d+)/ ) {
		$seqid = $n;
		if( $s > $e ) {
		    ( $s, $e ) = ( $e, $s );
		    $strand = 0-$strand;
		}
		$start += ($s-1);
		$end   += ($s-1);
	    }

	    my %tags = ( 'rfam-id' => $rfamid,
			 'rfam-acc' => ($cm->{$rfamid}->{-accession} || 'unknown'),
			 'model_start' => $modst,
			 'model_end' => $moden,
			 'gc-content' => $gc,
	    );

	    if( $evalue =~ /[0-9]/ ) {
		$tags{'evalue'} = $evalue;
	    }

	    my $f = Bio::SeqFeature::Generic->new( -seq_id => $seqid,
						   -start => $start,
						   -end => $end,
						   -strand => $strand,
						   -primary_tag => 'similarity',
						   -source_tag => 'Rfam',
						   -score => $bits,
						   -tag => \%tags,
		);
	    push( @f, $f );
	}
    }
    return \@f;
}


sub parse_infernal_results {
    my $file = shift;
    my $parser = Bio::SearchIO->new( -file => $file,
				     -format => 'infernal1'
				     );

    my @f;
    while( my $res = $parser->next_result ) {
	my $rfamid = $res->query_name;
	my $rfamacc = $cm->{$rfamid}->{-accession} || 'unknown';
	
	foreach my $hit ( sort { $b->score <=> $a->score } $res->hits ) {
	    foreach my $hsp ( sort { $b->score <=> $a->score } $hit->hsps ) {
		my( $name, $start, $end ) = 
		    ( $hit->name, $hsp->start('hit'), $hsp->end('hit') );
		
		my $strand = 1;
		if( $hsp->strand('query') != $hsp->strand('hit') ) {
		    $strand = -1;
		}

		# recalculate start and end if name/start-end
		if( my( $n,$s,$e) = $name =~ /^(\S+)\/(\d+)-(\d+)/ ) {
		    $name = $n;
		    if( $s > $e ) {
			( $s, $e ) = ( $e, $s );
			$strand = 0-$strand;
		    }
		    $start += ($s-1);
		    $end   += ($s-1);
		}

		my $f = Bio::SeqFeature::Generic->new( -seq_id => $name,
						       -start => $start,
						       -end => $end,
						       -strand => $strand,
						       -primary_tag => 'similarity',
						       -source_tag => 'Rfam',
						       -score => $hsp->score,
						       -tag => { 'rfam-id' => $rfamid,
								 'rfam-acc' => $rfamacc,
								 'model_start' => $hsp->start('query'),
								 'model_end' => $hsp->end('query'),
							     },
						       );
		push( @f, $f );
	    }
	}
    }
    return \@f;
}


sub parse_blast_table {
    my $blastfile = shift;
    my $hits = {};
    my $fh = IO::File->new();
    # sort hits so they go in to add_non_overlapping_hit in coordinate
    # order
    $fh->open( "sort -k7n $blastfile |" );
    while(<$fh>) {
	next if( /^\#/ );
	my @col = split( /\s+/, $_ );

	my( $qname, $hname, $start, $end );

	if( $filter =~ /ncbi/ ) {
	    ( $qname, $hname, $start, $end ) = ( $col[0],
						 $col[1],
						 $col[6],
						 $col[7] );
	}
	elsif( $filter =~ /wu/ ) {
	    ( $qname, $hname, $start, $end ) = ( $col[0],
						 $col[1],
						 $col[17],
						 $col[18] );
	}

	my( $rfamacc, $rfamid ) = split( ';', $hname );
	my $length  = $seqs->{$qname}->length;
	my $win = $cm->{$rfamid}->{-length};

	$start -= $win;
	$end   += $win;
	$start  = 1       if( $start < 1 );
	$end    = $length if( $end   > $length );

	my $newhit = { -acc => $rfamacc,
		       -id => $rfamid,
		       -seqid => $qname,
		       -start => $start, 
		       -end => $end, 
	};
		
	add_non_overlapping_hit( $hits, $newhit );	    
    }

    return $hits;
}


sub parse_blast {
    my $blastfile = shift;
    my $hits = {};
    my $searchin = Bio::SearchIO->new( '-file' => $blastfile, 
				       '-format' => 'Blast' );

    while( my $result = $searchin->next_result() ) {
	my $qname = $result->query_name;
        while( my $hit = $result->next_hit() ) {
	    my( $rfamacc, $rfamid ) = split( ';', $hit->name );
            while( my $hsp = $hit->next_hsp() ) {
		my( $start, $end ) = ( $hsp->start('query'), 
				       $hsp->end('query') );
		my $length  = $seqs->{$qname}->length;
		my $win = $cm->{$rfamid}->{-length};

		$start -= $win;
		$end   += $win;
		$start  = 1       if( $start < 1 );
		$end    = $length if( $end   > $length );

		my $newhit = { -acc => $rfamacc,
			       -id => $rfamid,
			       -seqid => $qname,
			       -start => $start, 
			       -end => $end, 
		};
		
		add_non_overlapping_hit( $hits, $newhit );	    
	    }
	}
    }
    return $hits;
}


sub add_non_overlapping_hit {
    # this doesn't do what it is meant to, because the HSPs come at
    # us in order of score, not sorted by coordinates.
    # -- fixed by pre-sorting blast table hits
    my $hits = shift;
    my $newhit = shift;
    my( $acc, $seqid, $start, $end ) = ( $newhit->{-acc},
					 $newhit->{-seqid},
					 $newhit->{-start},
					 $newhit->{-end} );
    my $already;
    if( exists $hits->{$acc}->{$seqid} ) {
	foreach my $se ( #sort { $a->{-start} <=> $b->{-start} }
			 @{ $hits->{$acc}->{$seqid} } ) {
	    if( $se->{-start} >= $start and $se->{-start} <= $end ) {
		$se->{-start} = $start;
		$already = 1;
	    }
	    if( $se->{-end} >= $start and $se->{-end} <= $end ) {
		$se->{-end} = $end;
		$already = 1;
	    }
	    if( $se->{-start} <= $start and $se->{-end} >= $end ) {
		$already = 1;
	    }
	}
    }

    return 0 if( $already );

    push( @{ $hits->{$acc}->{$seqid} }, $newhit );
    return $hits;
}


sub read_cm_library {
    my $fh = shift;
    my $off = 0;
    my %cm;
    my $name;
    while(<$fh>) {
	if( /^NAME\s+(\S+)/ ) {
	    $name = $1;
	    $cm{$name}->{-offset} = $off;
	}
	if( /^CLEN\s+(\d+)/ ) {
	    $cm{$name}->{-length} = $1;
	}
	if( /^ACCESSION\s+(\S+)/ ) {
	    $cm{$name}->{-accession} = $1;
	}
	if( /^SCOM\s+cmsearch\s+(.*)\s+\S+\s+\S+/ ) {
	    my $options = '';
	    my @opts = split( /\s+/, $1 );
	    while( my $opt = shift @opts ) {
		# don't want to propagate some cmsearch options from the Rfam.cm file
		if( $opt =~ /^-Z$/ ) {
		    shift @opts;
		}
		elsif( $opt =~ /^-E$/ ) {
		    shift @opts;
		}
		elsif( $opt =~ /--toponly/ ) {
		}
		else {
		    $options .= $opt." ";
		}
	    }
	    $cm{$name}->{-options} = $options;
	}
	if( /^\/\// ) {
	    $off = tell($fh);
	}
    }
    return \%cm;
}

sub get_cm_from_id {
    my $fh = shift;
    my $id = shift;
    my $outfile = "/tmp/$$.cm";
    seek($fh,$cm->{$id}->{-offset},0);
    open( F, ">$outfile" ) or die "FATAL: Failed to retrieve CM [$id] from handle [$fh]\n";
    while(<$fh>) {
	print F $_;
	last if( /^\/\// );
    }
    return $outfile;
}

######



=head1 NAME

rfam_scan.pl - search a nucleotide fasta sequence against the Rfam
library of covariance models.

=head1 VERSION

This is version 1.0 of rfam_scan.pl.  It has been tested with Perl
5.6.1, Rfam 10.0, Bioperl 1.5.2/1.6 and INFERNAL 1.0.  It should work with
versions higher than these, except where file formats change!

=head1 REQUIREMENTS

 - this script

 - Perl 5.6 or higher (and maybe lower)

 - The Rfam database (downloadable from
   ftp://ftp.sanger.ac.uk/pub/databases/Rfam)

 - INFERNAL software v1.0 and up (from http://infernal.janelia.edu/)

 - NCBI BLAST binaries (from http://www.ncbi.nlm.nih.gov/Ftp/)

 - Bioperl (from http://bio.perl.org/)

The Bioperl modules directory must be in your perl library path, and
the INFERNAL and BLAST binaries must be in your executable path.

You also need to be able to read and write to /tmp on your machine.

=head1 HOW TO INSTALL RFAM LOCALLY

1. Get the Rfam database from
   ftp://ftp.sanger.ac.uk/pub/databases/Rfam/.  In particular you need
   the files Rfam.fasta and Rfam.cm

2. Unzip them if necessary
    $ gunzip Rfam*.gz

3. Grab and install INFERNAL, NCBI BLAST and Bioperl, and make sure
   your paths etc are set up correctly.

=head1 SEARCHING YOUR SEQUENCE AGAINST RFAM

The INFERNAL user manual has information about how to search sequences
using covariance models.  This is very compute intensive.  This script
provides some hacks to speed up the process.

Run rfam_scan.pl -h to get a list of options.  

=head1 THINGS TO NOTE

It is important that every sequence in your input fasta file has a
unique name.

This script can take a long while to run on big sequences,
particularly if your sequence looks anything like a ribosomal RNA.
You will want to test on something small and sensible first.
Ribosomal RNAs should be relatively easy to find using things like
BLAST, so you can omit the SSU and LSU rRNAs from Rfam searches (along
with group I and group II catalytic introns) with the --nobig option.

=head1 BUGS

   - Full cmsearch (no --blastdb option) doesn't use the global/local 
     state of the model. Curated thresholds may therefore not be 
     meaningful.
   - Many options are not rigorously tested.  
   - Error messages are uninformative.  
   - The documentation is inadequate.

=head1 HISTORY

v1.0    2010-02-28
   - New style command line
   - Use Infernal 1.0
   - Remove most of bioperl reliance (slow parsing of blast/infernal files)
   - Use either wu or ncbi blast
   - Output only in GFF format
   - Use global/local state of the model and Rfam cmsearch options from CM 
     SCOM line
   - Don't need Rfam.thr file

v0.3    2007-06-20
   - New parser (bioperl style) for INFERNAL 0.81 output
   - Probably need Bioperl 1.5, and extra Bio::SearchIO::infernal
   - Using WUBLAST, instead of NCBI

v0.2    2005-02-22
   - add --slow option
   - 'aln' format option gives you cmsearch style alignments
   - fix -o output option

v0.1    2003-11-19
   - first effort at something useful
   - return Rfam hits as tab delimited or gff format


=head1 CONTACT

Copyright (c) 2003-2006  Genome Research Ltd
Copyright (c) 2007-2010  Sam Griffiths-Jones

Please contact rfam@sanger.ac.uk for help.

=cut
