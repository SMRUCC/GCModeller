#!/usr/bin/perl
use strict;
use warnings;
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
# Version: combine_result.pl                                        #
# Begin   : 2014.10.9                                               #
 ###################################################################

my ($Inputfile1, $Inputfile2, $output );

GetOptions( 
            "pot=s" => \$Inputfile1,
			"ot=s" => \$Inputfile2,        #input   
            "out=s" => \$output,
          );

$Inputfile1 ||= "report_count_candidate.protospacer_POT.txt";
$Inputfile2 ||= "report_count.total_OT.txt";
$output ||= "final_count_candidate.txt";

open FILE1,"$Inputfile1"|| die "Can't open $Inputfile1 for reading!\n";
open FILE2,"$Inputfile2"|| die "Can't open $Inputfile2 for reading!\n"; #file1 to file2
open FILE3,">$output" || die "Can't open $output for writing!\n";

my %file1;

while(<FILE1>)
{
chomp;

my @a=split/\t/, $_;

$a[0] =~ s/.POT//m;

$file1{$a[0]}=$a[1];

}
###
while(<FILE2>)
{

my @q=split " ";

if(exists $file1{$q[0]})
{

print FILE3 "$q[0]\t$file1{$q[0]}\t$q[1]\n";

}
}
close FILE1;
close FILE2;
close FILE3;
