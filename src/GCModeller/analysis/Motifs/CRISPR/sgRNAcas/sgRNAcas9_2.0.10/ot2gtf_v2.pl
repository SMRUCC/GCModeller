#!/usr/bin/perl -w
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
# Version: ot2gtf.pl version 1.0.2                                  #
# Begin       : 2014.5.17                                           #
# LAST REVISED: 2014.5.18                                           #
# Revised     : 2014.8.24                                           #    
#                                                                   #
# usage: perl ot2gtf_v2.pl -i <input> -g <input2> -o <output>       #
#####################################################################

#Main code start here
my ($INPUT, $INPUT2, $OUT);

GetOptions( "i=s" => \$INPUT,       
            "g=s" => \$INPUT2,  
            "O=s" => \$OUT,      
          );

open( IN1, "$INPUT" ) || die "can't open $INPUT!";
open( OUT, ">$OUT") || die "can't open $OUT!";

my $geneID;
my $geneIDtype;

readline IN1;

while(<IN1>){
	chomp;
	#ITGB3_hsa_A_6	POT153	a - - - - - a c - - - - - - c - - - - - - a -	4M	10	3236416	-             #OT
    (my $ID, my $number, my $seq, my $mismatch, my $chr, my $location, my $strand)=split/\t/, $_;

##############
	open( IN2, "$INPUT2" ) || die "can't open $INPUT2!";
	readline IN2;
	LABEL:while(<IN2>){
		chomp;
		next unless /^[^\t]+\t[^\t]+\texon\t/;
              
		#1	pseudogene	gene	11869	14412	.	+	.	gene_id "ENSG00000223972"; gene_name "DDX11L1"; gene_source "ensembl_havana"; gene_biotype "pseudogene";
        (my $gtf_chr, my $gtf_annotation1, my $gtf_annotation2,	my $gtf_beg, my $gtf_end, my $get_dot1, my $gtf_strand, my $gtf_dot2, my $gtf_annotation3)=split/\t/, $_;
        
		next unless $gtf_chr=~ /^[0-9]|^[X]|^[Y]/;
        next if !$gtf_beg =~ /\d+/;

        #gene_id "ENSG00000223972"; transcript_id "ENST00000515242"; gene_name "DDX11L1"; gene_source "ensembl_havana"; gene_biotype "pseudogene"; transcript_name "DDX11L1-201"; transcript_source "ensembl";
        if ($gtf_annotation3=~ /".*gene_name "([^"]+)"/) {

        $geneID = $1;
        #print $geneID,"\n";

        }

###############       
		if ( ord($chr) == ord($gtf_chr) &&  $location >= $gtf_beg && $location <= $gtf_end && $strand eq $gtf_strand ) {

             print OUT $ID, "\t", $number, "\t", $seq,  "\t", $mismatch,  "\t", $chr, "\t", $location,  "\t", $strand,  "\t", $geneID, "\n" ;
		 
			 last LABEL;

            }elsif (ord($chr) == ord($gtf_chr) &&  $location >= $gtf_beg && $location <= $gtf_end && $strand ne $gtf_strand) {

             print OUT $ID, "\t", $number, "\t", $seq,  "\t", $mismatch,  "\t", $chr, "\t", $location,  "\t", $strand,  "\t", $geneID, "\n" ;
		 
			 last LABEL;

            }
	}	
	close IN2;
}
close IN1;
close OUT;
