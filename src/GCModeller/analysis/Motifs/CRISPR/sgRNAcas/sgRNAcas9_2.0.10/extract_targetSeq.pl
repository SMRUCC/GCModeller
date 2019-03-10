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
# Version: extract_targetSeq-1.0                                    #
# Begin       : 2013.12.9                                           #
# LAST REVISED: 2014.1.27                                           #
 ###################################################################

my $oldtime = time();

my ($Inputfile, $Genome, $Length);

GetOptions( "i=s" => \$Inputfile,        #input   
            "g=s" => \$Genome,           #The reference genome sequence
			"l=i" => \$Length,           #length of flank sequences [1000]
          );

#default
$Length ||= "1000";

print "\n\tWelcome to sgRNAcas9\n";
print "\t---a tool for designing CRISPR sgRNA with high specificity\n";
print "\t---------------------------------------------------------\n";
print "Version   : 2.0"."\n";
print "Copyright : Free software"."\n";
print "Author    : Shengsong Xie"."\n";
print "Email     : ssxieinfo\@gmail.com"."\n";
print "Homepage  : www.biootools.com"."\n";

my $local_time;
   $local_time = localtime();
print "Today     : $local_time\n\n";

print "Usage: perl $0 -I <POT> -G <genome> -L <length>\n\n";

print "[OPTION]:\n";
print " -I <POT>          Genomics information of on or potential off-target(POT).\n";   
print " -G <genome>       Refence genome sequences.\n";   
print " -L <length>       Target sequence length.\n";    

print "\nPlease wait, extracting target sequences NOW!"."\n"; 
######################### extract target flank seq #######################################

open(IN, $Genome) || die "Can't open $Genome for reading!\n";

open(OUT1, ">targetSeq_$Inputfile") || die "Can't open targetSeq_$Inputfile for writing!\n";

my ($ID, $chrname, $location, $strand, $Mismatch, $start, $end, $noOfBases);

open(POT, "<$Inputfile")|| die "Can't open $Inputfile for reading!\n";

while (<POT>) {
 chomp($_);

	#(my $ID, my $chrname, my $location, my $strand,	my $Mismatch, my $SEQ, my $Type)=split/\t/, $_;
	(my $SEQ, my $Mismatch, my $ID, my $chrname, my $location, my $strand,my $Type)=split/\t/, $_;
    #print $Type."\n";

	next if $location=~/Location/;
    
    my $start = $location - ($Length/2);
    my $end   = $location + ($Length/2);
    my $noOfBases = $end-$start+1;

    seek IN, 0, 0;

open(IN, $Genome);
my ($seq, $head, $seqFlag) = ("", "", "off");

while(<IN>) {
chomp($_);
	next if $ID=~/ID/;

	if(/>/) {
		
		if($seqFlag eq "on" && $seq ne "") {
			my $len = length($seq);
			my $substr = substr($seq, $start-1, $noOfBases);
			#my @seqParts = split(/(.{100})/, $substr);
			my @seqParts = split(/\s+/, $substr);

			for my $seqs (@seqParts) {
				unless($seqs eq "") {

					if ($strand eq "+") {

			print OUT1 ">$ID mismatch=$Mismatch $Type 5'-3' $head Chr_len=$len, location=$location, extract: $start-$end ($noOfBases bp) strand($strand)\n";		
		    print OUT1 $seqs."\n";

	                }elsif ($strand eq "-") {

					my $seqs2.= $seqs;
                    my $seqs2comp = $seqs2;
                       $seqs2comp =~ tr/ACGT/TGCA/;                           
                    my $seqs2Revcomp = reverse($seqs2comp); 

			print OUT1 ">$ID mismatch=$Mismatch $Type 5'-3' $head Chr_len=$len, location=$location, extract: $start-$end ($noOfBases bp) strand($strand)\n";
            print OUT1 $seqs2Revcomp,"\n";

	            }

				}
			}
			$seq="";
			$seqFlag="off";
		}
		my @tab = split(/\s+/, $_);
		#print OUT1 $chrname."\n";
		if($tab[0] eq ">$chrname") {
			$seqFlag = "on";
			$head = $tab[0];
		}
	}
	else {
		if($seqFlag eq "on") {
			chomp($_);
			$seq.=$_;
		}
	}
}
if($seqFlag eq "on" && $seq ne "") {
	
	my $len = length($seq);
	print OUT1 ">$ID mismatch=$Mismatch $Type 5'-3' $head Chr_len=$len, location=$location, extract: $start-$end ($noOfBases bp) strand($strand)\n";
	my $substr = substr($seq, $start-1, $noOfBases);
	#my @seqParts = split(/(.{100})/, $substr);
	my @seqParts = split(/\s+/, $substr);

	for my $seqs (@seqParts) {
		unless($seqs eq "") {
			print OUT1 $seqs."\n";
		}
	}
	$seq="";
	$seqFlag="off";
}

}


my $oo = time() -  $oldtime;
printf  "\nTotal time consumption is $oo second.\n";