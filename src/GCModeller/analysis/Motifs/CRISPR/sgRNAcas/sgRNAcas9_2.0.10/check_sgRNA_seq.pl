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
# Version: check_sgRNA.seq-1.0                                      #
# Begin       : 2013.12.9                                           #
# LAST REVISED: 2014.1.26                                           #
 ###################################################################

my ($Inputfile, $re );

GetOptions( "i=s" => \$Inputfile,        #input   
            "r=s" => \$re,

          );

#restriction enzyme,BsaI sequences:(GAGACC)|(GGTCTC)
$re ||="(GAGACC)|(GGTCTC)";

$/ = '>';

my $seq_line;
my @seq;
my $desc;
my $i;

open(FASTA, $Inputfile) || die "Can't open $Inputfile for reading!\n";

open(CHECK, ">check.sgR_seq.txt") || die "Can't open check.sgR_seq.txt for writing!\n";

while(<FASTA>){
chomp($_);

	$seq_line = '';
	s/>//g;
	@seq = split "\n";
	if(@seq){
		$desc = $seq[0]; ##

		foreach $i (1..(@seq-1)) {

			$seq[$i] =~ s/\n//g;  ## 
			$seq_line .=  $seq[$i]; ##
		}
		
		if( $seq_line =~ /T{4,21}/g){
		
		print CHECK ">$desc #contain polyT(more than 4)\n$seq_line\n";  ##poly T(more than 4)

		}elsif ($seq_line =~ /$re/g) {
			
		print CHECK ">$desc #contain restriction enzyme site\n$seq_line\n";  ##restriction enzyme site	
		
		}elsif( ($seq_line =~ /A{5,21}|C{5,21}|G{5,21}|(AT){6,10}|(AC){6,10}|(AG){6,10}|(TA){6,10}|(TC){6,10}|(TG){6,10}|(CA){6,10}|(CT){6,10}|(CG){6,10}|(GA){6,10}|(GT){6,10}|GC{6,10}|(AAT){5,7}|(AAC){5,7}|(AAG){5,7}|(ATA){5,7}|(ATT){5,7}|(ATC){5,7}|(ATG){5,7}|(ACA){5,7}|(ACT){5,7}|(ACC){5,7}|(ACG){5,7}|(AGA){5,7}|(AGT){5,7}|(AGC){5,7}|(AGG){5,7}|(TAA){5,7}|(TAT){5,7}|(TAC){5,7}|(TAG){5,7}|(TTA){5,7}|(TTC){5,7}|(TTG){5,7}|(TCA){5,7}|(TCT){5,7}|(TCC){5,7}|(TCG){5,7}|(TGA){5,7}|(TGT){5,7}|(TGC){5,7}|(TGG){5,7}|(CAA){5,7}|(CAT){5,7}|(CAC){5,7}|(CAG){5,7}|(CTA){5,7}|(CTT){5,7}|(CTC){5,7}|(CTG){5,7}|(CCA){5,7}|(CCT){5,7}|(CCG){5,7}|(CGA){5,7}|(CGT){5,7}|(CGC){5,7}|(CGG){5,7}|(GAA){5,7}|(GAT){5,7}|(GAC){5,7}|(GAG){5,7}|(GTA){5,7}|(GTT){5,7}|(GTC){5,7}|(GTG){5,7}|(GCA){5,7}|(GCT){5,7}|(GCC){5,7}|(GCG){5,7}|(GGA){5,7}|(GGT){5,7}|(GGC){5,7}/g) ){  
       
        print CHECK ">$desc #contain repeat nucleotides\n$seq_line\n";  ## 

		}else{

		print CHECK  ">$desc\n$seq_line\n";  ## 
	}
}
}