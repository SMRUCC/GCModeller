---
title: Resources
---

# Resources
_namespace: [SMRUCC.genomics.Data.Xfam.Rfam.My.Resources](N-SMRUCC.genomics.Data.Xfam.Rfam.My.Resources.html)_

A strongly-typed resource class, for looking up localized strings, etc.




### Properties

#### Culture
Overrides the current thread's CurrentUICulture property for all
 resource lookups using this strongly typed resource class.
#### ResourceManager
Returns the cached ResourceManager instance used by this class.
#### rfam_scan
Looks up a localized string similar to #!/usr/bin/perl

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
my $cmdline = $0.' '.join( ' [rest of string was truncated]";.
