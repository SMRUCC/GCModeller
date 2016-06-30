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
# Version: sgRPrimer-1.0                                            #
# Begin       : 2013.12.9                                           #
# LAST REVISED: 2014.1.27                                           #
 ###################################################################


my ($file_fasta, $file_ids, $sgRlength, $fprimer, $rprimer);

GetOptions( "i=s" => \$file_fasta,              #input
            "s=s" => \$file_ids,                #a file of IDs
            "l=s" => \$sgRlength,
			"f=s" => \$fprimer,                 #restriction enzyme cutting site for forward primer [accg]
			"r=s" => \$rprimer,                 #restriction enzyme cutting site for reverse primer [aaac]
          );

#default: Primer pairs for pGL3-U6-gRNA-Puromycin, Bsa1 ";
$fprimer ||= "accg";
$rprimer ||= "aaac";
$sgRlength ||= 20;

my $NsgR = 20-$sgRlength;

#my %options=();
#getopts("a",\%options);

my %hash;

parse_file_ids(\$file_ids,\%hash);

parse_fasta(\$file_fasta,\%hash);

exit; 

sub parse_fasta{

    my ($file,$hash) = @_;
    my ($id, $desc, $sequence) = ();

    open (FASTA, "$file_fasta") or die "can not open $file_fasta\n";
	open (OUTU, ">sgR.Primers_$sgRlength.txt") or die "can not open sgR.Primers_$sgRlength.txt\n";

    while (<FASTA>)
    {
        chomp;
        if (/^>(\S+)(.*)/)
        {
            $id       = $1;
            $desc     = $2;
            $sequence = "";

            while (<FASTA>){
                chomp;
                if (/^>(\S+)(.*)/){

		   # if((defined $$hash{$id} and not $options{a}) or (not defined $$hash{$id} and $options{a})){
             if(defined $$hash{$id}){

            #print OUTID ">$id$desc\n$sequence\n";

			my $SgR = $sequence;
            my $SgRN = substr($SgR,$NsgR,$sgRlength);
            my $SgRNcomp = $SgRN;                                                     
               $SgRNcomp =~ tr/ACGT/TGCA/;                                               
            my $SgRNRevcomp = reverse($SgRNcomp); 

            #for vector pGL3-U6-gRNA-Puromycin mut Bsa1 ACCG
            #my $FP1 = "accg".$SgRN;
            my $FP1 = "$fprimer".$SgRN;

            #for vector pGL3-U6-gRNA--Puromycin mut Bsa1 CCGG
            #my $FP2 ="ccgg".$SgRN;
            #for vector pUC57kan-T7-gRNA
            #my $FP3 = "tagg".$SgRN;

            my $RP1 = "$rprimer".$SgRNRevcomp;

            #defaut:"Primer pairs for pGL3-U6-gRNA-Puromycin mut Bsa1 ACCG\n";
            print OUTU "$id-$sgRlength-FP\t"."$FP1\t"."\n";
            print OUTU "$id-$sgRlength-RP\t"."$RP1\t"."\n";
            print OUTU "\n";

		    }

                    $id         = $1;
                    $desc       = $2;
                    $sequence   = "";
                    next;
                }
                $sequence .= $_;
            }


        }
    }

    #if((defined $$hash{$id} and not $options{a}) or (not defined $$hash{$id} and $options{a})){
    #	print OUTID ">$id$desc\n$sequence\n";
    #}

    close FASTA;
    return;
}


sub parse_file_ids{

    my ($file,$hash) = @_;

    open (FILE, "$file_ids") or die "can not open $file_ids\n";
    
    while (my $line=<FILE>){

	if($line=~/^(\S+)/){
	    
	    my $id=$1;
	    
	    $$hash{$id}=1;
	}
    }
    return;
}


