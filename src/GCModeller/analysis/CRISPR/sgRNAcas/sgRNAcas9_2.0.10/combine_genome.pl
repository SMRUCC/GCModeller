#!/usr/bin/perl
use warnings;
use strict;
#use Getopt::Std;
use Getopt::Long;

 ########################### WELCOME  ##############################
#                                                                   #
#                          sgRNAcas9                                # 
# ---a tool for fast designing CRISPR sgRNA with high specificity   #
#                                                                   #
# AUTHOR  : Xie Shengsong                                           #
# Email   : ssxieinfo\@gmail.com                                    #
# Homepage: www.biootools.com                                       #
#           BiooTools (Biological online tools)                     #  
# Shanghai Institute of Biochemistry and Cell Biology (SIBCB)       #
# Website: www.sibcb.ac.cn/eindex.asp                               #
# Version: combine_genome-1.0                                       #
# Begin       : 2014.9.17                                           #
# LAST REVISED: 2014.9.17                                           #
 ###################################################################


my ($suffix,$file_fasta);

GetOptions( "s=s" => \$suffix,       
			"o=s" => \$file_fasta,              #output
          );

$file_fasta ||= "combine_genome.fas";
$suffix ||= "fa";

my $nextname;
my @line;
my $seq;

open(OUTFILE,">$file_fasta")or die "can not open $file_fasta\n";

while(defined($nextname = <*.$suffix>)){

open(FILE,$nextname);

@line=<FILE>;

$seq=join("",@line);

print OUTFILE "$seq\n";

close(FILE);

}

close(OUTFILE);