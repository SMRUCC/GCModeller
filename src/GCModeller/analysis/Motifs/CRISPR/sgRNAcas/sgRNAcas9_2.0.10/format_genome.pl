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
# Version: format_genome-1.0                                        #
# Begin       : 2013.12.9                                           #
# LAST REVISED: 2014.1.26                                           #
 ###################################################################


my ($file);

GetOptions( "i=s" => \$file,              #input
          );

parse_fasta($file);

exit;

sub parse_fasta{

    my ($file) = @_;

    open (FASTA, "<$file") or die "can not open $file\n";
    open (FORMAT, ">format.$file") or die "can not open format.$file\n";

    while (<FASTA>){
		
		if (/^(>\S+)/){
			
			print FORMAT "$1\n";

		}else{
			print FORMAT uc($_);
		}
	}
    
    close FASTA;
    return 0;
}
