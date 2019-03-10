#!/usr/bin/perl -w
use strict;
use warnings;
use Getopt::Long;
use Cwd;
#use diagnostics;

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
# Version: sgRNAcas9_2.0.7                                          #
# Begin       : 2013.12.9                                           #
# LAST REVISED: 2014.6.26                                           #
 ###################################################################

my $oldtime = time();

my ($Inputfile_Fasta, $truncat, $GC_l, $GC_m, $Genome, $Option, $Type, $Seqmap_vesion, $Num_mismatch, $offset_s, $offset_e, $path);

GetOptions( "i=s" => \$Inputfile_Fasta,        #Input file
            "x=i" => \$truncat,                #Length of sgRNA[20]
            "l=i" => \$GC_l,                   #The minimum value of GC content [20]
			"m=i" => \$GC_m,                   #The maximum value of GC content [80] 
	        "g=s" => \$Genome,                 #The reference genome sequence
			"o=s" => \$Option,                 #Searching CRISPR target sites using DNA strands based option(s/a/b)
			"t=s" => \$Type,                   #Type of gRNA searching mode(s/p) 
			"v=s" => \$Seqmap_vesion,          #Operation system [w, for windows; l, for linux-64; u, for linux-32;  m, for MacOSX-64; a, for MacOSX-32]
			"n=i" => \$Num_mismatch,           #Maximum number of mismatches [5]
			"s=i" => \$offset_s,               #The minimum value of sgRNA offset [-2]
            "e=i" => \$offset_e,               #The maximum value of sgRNA offset [32]
            "p=s" => \$path,                   #Output path
          );

#default
$truncat ||= "20";  
$GC_l ||= "20";                                #20 % < GC% < 80 %
$GC_m ||= "80";
$Seqmap_vesion ||= "l";                        #linux
$Num_mismatch ||="5";                          #number of mismatches: 5
$offset_s ||="-3";                             #sgRNA offset: -2 to 32 bp
$offset_e ||="33";
my $dir_default = getcwd;                      #default output
$path ||= $dir_default;

my $dir =$path;

mkdir("$dir/sgRNAcas9.report_$truncat.$Option",0755)||die "Can't create directory: Directory exists at $dir. Please delete, move or rename the exist directory before you run this program.$!" ;

open  (LOG, ">>$dir/sgRNAcas9.report_$truncat.$Option/sgRNAcas9.Log.txt") || die "Can't open sgRNAcas9.Log.txt for writing!" ."\n";
print  LOG "################################# Log ###########################################".        "\n\n";
#print "Writing Log information.                                                                      " ."\n";
print  LOG "#                              sgRNAcas9                                                  " ."\n";
print  LOG "#     ---a tool for fast designing CRISPR sgRNA with high specificity                     " ."\n";          
print  LOG "#                                                                                         " ."\n";
print  LOG "#       contact:  Xie Shengsong, Email: ssxieinfo\@gmail.com                                .\n\n";
######


mkdir("$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report",0755)||die "can't create directory: $!" ;

print "\n\tWelcome to sgRNAcas9\n";
print "\t---a tool for fast designing CRISPR sgRNA with high specificity\n";
print "\t---------------------------------------------------------\n";
print "Version   : 2.0"."\n";
print "Copyright : Free software"."\n";
print "Author    : Shengsong Xie"."\n";
print "Email     : ssxieinfo\@gmail.com"."\n";
print "Homepage  : www.biootools.com"."\n";

my $local_time;
$local_time = localtime();
print "Today     : $local_time\n\n";
print  LOG "# Time, begin at $local_time."."\n";
print  LOG "# Usage: perl $0 -i $Inputfile_Fasta -x $truncat -l $GC_l -m $GC_m -g $Genome -o $Option -t $Type -v $Seqmap_vesion -n $Num_mismatch -s $offset_s -e $offset_e -p $path 2>log.txt\n\n";

=pod
print "Options ([], represents the default value):\n";
-i <str>			Input file
-x <int>            Length of sgRNA [20]
-l <int>			The minimum value of GC content [20]
-m <int>			The maximum value of GC content [80]
-g <str>			The reference genome sequence
-o <str>			Searching CRISPR target sites using DNA strands based option(s/a/b)
                    [s, sense strand searching mode]
                    [a, anti-sense strand searching mode]
                    [b, both strand searching mode]
-t <str>			Type of gRNA searching mode(s/p)
				    [s, single-gRNA searching mode]
				    [p, paired-gRNA searching mode]
-v <str>			Operation system(w/l/u/m/a)
                    [w, for windows-32, 64]
					[l, for linux-64]
					[u, for linux-32]
					[m, for MacOSX-64]
					[a, for MacOSX-32]
-n <int>			Maximum number of mismatches [5]
-s <int>			The minimum value of sgRNA offset [-2]
-e <int>			The maximum value of sgRNA offset [32]
-p <str>            Output path
=cut

################################### format seq ###################################
print "Start sgRNAcas9 program........\n";
print "Step1: Format target sequences.\n";
print  LOG "# Start sgRNAcas9 program........\n";
print  LOG "# Step1: Format target sequences.\n";

open (Inseq, $Inputfile_Fasta) || die "Can't open $Inputfile_Fasta for reading!\n";
open (FASTA, ">$dir/sgRNAcas9.report_$truncat.$Option/TargetSeq.fa") || die "Can't open TargetSeq.fa for writing!\n";

my $TmpTit="";
my $TmpSeq="";
my $TmpTit_S="";
my $TmpTit_A="";
my $TmpSeq_Acomp;
my $TmpSeq_ARevcomp;

if ($Option eq "b") {              #B=both:sense and anti-sense strand

    while (<Inseq>) {

	  chomp $_; 
	 
      if (/^>(\S+)/){
	   
	    if ($TmpTit && $TmpSeq) {
	
			 $TmpTit_S=$TmpTit."_S";
		     $TmpTit_A=$TmpTit."_A";
		
			 $TmpSeq_Acomp=$TmpSeq;                        #Reverse-comp                                                
		     $TmpSeq_Acomp =~ tr/atucgACGUT/TAAGCTGCAA/;  	
		     $TmpSeq_ARevcomp = reverse($TmpSeq_Acomp); 
				
		     print FASTA ">$TmpTit_S\n$TmpSeq\n";	
	         print FASTA ">$TmpTit_A\n$TmpSeq_ARevcomp\n";
		    }
	
	         $TmpTit=$1;	
	         $TmpSeq="";
	
	   }elsif(/(\w+)/){
	
	         $TmpSeq.=$1;
		
	  }
	 
	}

       if ($TmpTit && $TmpSeq) {

	        my $TmpTit_S=$TmpTit."_S";
            my $TmpTit_A=$TmpTit."_A";

	        my $TmpSeq_Acomp=$TmpSeq;                        #Reverse-comp                                                
            $TmpSeq_Acomp =~ tr/atucgACGUT/TAAGCTGCAA/;  	
            my $TmpSeq_ARevcomp = reverse($TmpSeq_Acomp); 
     
            print FASTA ">$TmpTit_S\n$TmpSeq\n";
            print FASTA ">$TmpTit_A\n$TmpSeq_ARevcomp\n"; 
	
  }
}elsif ($Option eq "a") {           #Non-template: A: anti-sense strand

	while (<Inseq>) {

		chomp $_;

	if (/^>(\S+)/){
	
	    if ($TmpTit && $TmpSeq) {
		  	  
		     $TmpTit_A=$TmpTit."_A";
		
			 $TmpSeq_Acomp=$TmpSeq;                        #Reverse-comp                                                
		     $TmpSeq_Acomp =~ tr/atucgACGUT/TAAGCTGCAA/;  	
		     $TmpSeq_ARevcomp = reverse($TmpSeq_Acomp); 
				
		     print FASTA ">$TmpTit_A\n$TmpSeq_ARevcomp\n";	
		  }
	
	         $TmpTit=$1;	
	         $TmpSeq="";
	
	  }elsif(/(\w+)/){
	
	         $TmpSeq.=$1;
		
	  }
	}

       if ($TmpTit && $TmpSeq) {

            my $TmpTit_A=$TmpTit."_A";

            my $TmpSeq_Acomp=$TmpSeq;                        #Reverse-comp                                                
            $TmpSeq_Acomp =~ tr/atucgACGUT/TAAGCTGCAA/; 
            my $TmpSeq_ARevcomp = reverse($TmpSeq_Acomp); 
   
            print FASTA ">$TmpTit_A\n$TmpSeq_ARevcomp\n";
   
  }
}elsif ($Option eq "s" || $Option eq "") {     #Template: S: sense strand

	while (<Inseq>) {

		chomp $_;

	if (/^>(\S+)/){
	
	   if ($TmpTit && $TmpSeq) {
		  	  
		    $TmpTit_S=$TmpTit."_S";
				
		    print FASTA ">$TmpTit_S\n$TmpSeq\n";	
		  
	    }
	
	        $TmpTit=$1;	
	        $TmpSeq="";
	
	  }elsif(/(\w+)/){
	
	    	$TmpSeq.=$1;
		
	  } 
	}

      if ($TmpTit && $TmpSeq) {
	
		   my $TmpTit_S=$TmpTit."_S";

           print FASTA ">$TmpTit_S\n$TmpSeq\n";
    
  }
}
close(Inseq);
close(FASTA);


########################### Find CRISPR targets-single #################################
print "Step2: Find CRISPR targets.\n";
print  LOG "# Step2: Find CRISPR targets.\n";

open(IntPut, "$dir/sgRNAcas9.report_$truncat.$Option/TargetSeq.fa") || die "Can't open TargetSeq.fa for reading!\n";

open(OutPut, ">$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_protospacer_single.txt") || die "Can't open report_protospacer_single.txt for writing!\n";
open(OutPut1, ">$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_single.fa") || die "Can't open CRISPR.targets_single.fa for writing!\n";
#open(OutPut2, ">$dir/sgRNAcas9.report_$truncat.$Option/full_length_sgRNA.txt") || die "Can't open full_length_sgRNA.txt for writing!\n";
open(OutPut3, ">$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_S.txt") || die "Can't open CRISPR.targets_single.fa for writing!\n";
open(OutPut4, ">$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_A.txt") || die "Can't open CRISPR.targets_single.fa for writing!\n";

#print OutPut "Candidate sgRNA, Pattern: GGX18NGG, GX19NGG, X20NGG, GC% >=$GC %\n\n";
print OutPut "sgRID\t"."Start\t"."End\t"."CRISPR_target_sequence(5'-3')\t"."Length(nt)\t"."GC%\n";

my $ID=""; 
my $seq="";
my $dCas9handle = "GUUUUAGAGCUAGAAAUAGCAAGUUAAAAUAAGGCUAGUCCG";
my $terminator = "UUAUCAACUUGAAAAAGUGGCACCGAGUCGGUGCUUUUUUU";
my $probe_id="";

while(<IntPut>) {
	chomp $_;

	if (/^>(\S+)/){
		analysis(); $ID=$1;

  }else{
		$seq=$_;
		
  }
}
analysis();
close OutPut;
close OutPut1;
close OutPut3;
close OutPut4;

############################# Find CRISPR targets-pairs #################################
open( PA, "<$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_A.txt" ) || die "can't open CRISPR.targets_A.txt!";
open( PS, "<$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_S.txt" ) || die "can't open CRISPR.targets_S.txt!";
open( PAIRS1, ">>$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_protospacer_pairs.txt" ) || die "can't open report_protospacer_pairs.txt!";
open( PAIRS2, ">>$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_pairs.fa" ) || die "can't open CRISPR.targets_pairs.fa!";

my $sgRID_A;
my $Start_A;
my $End_A;
my $target_seq_A ="";
my $Pattern_A;
my $GC_A;

my $sgRID_S;
my $Start_S;
my $End_S;
my $target_seq_S ="";
my $Pattern_S;
my $GC_S;

my $len_target_seq_A=0;
my $len_target_seq_S=0;

my $ID_A;
my $ID_S;

print PAIRS1 "\t\tPaired-gRNA\n";
print PAIRS1 "sgRID_S\ttarget_seq_S\tStart_S\tEnd_S\tGC%_S\t\tsgRID_A\ttarget_seq_A\tStart_A\tEnd_A\tGC%_A\tsgRNA_offset(bp)\n";

while ( <PA> ) {
	chomp $_; 
	(my $sgRID_A, my $Start_A, my $End_A, my $target_seq_A,	my $Pattern_A, my $GC_A, my $emp_A)=split/\t/, $_;

	$len_target_seq_A = length($target_seq_A);

	next if $len_target_seq_A ne ($truncat+3);

	$ID_A = $sgRID_A;
	$ID_A=~ s/_A_(\d+)//m;
	
	seek PS, 0, 0;

	while ( <PS> ) {
    chomp $_; 
		(my $sgRID_S, my $Start_S, my $End_S, my $target_seq_S,	my $Pattern_S, my $GC_S, my $emp_S)=split/\t/, $_;

		next if $target_seq_S eq "";
		$len_target_seq_S = length($target_seq_S);
		next if $len_target_seq_S ne ($truncat+3);               

		$ID_S = $sgRID_S;
		$ID_S=~ s/_S_(\d+)//m;
		
		my $offset_value = $Start_S -$End_A;

		if (($ID_A eq $ID_S) and ($offset_value > "$offset_s" and $offset_value < "$offset_e")) {      # -2 to 32 bp or 5 to 35 bp

			print PAIRS1 "$sgRID_A"."\t"."$target_seq_A"."\t"."$Start_A"."\t"."$End_A"."\t"."$GC_A"."\t<->\t";
			print PAIRS1 "$sgRID_S"."\t"."$target_seq_S"."\t"."$Start_S"."\t"."$End_S"."\t"."$GC_S"."\t"."$offset_value"."\n";
			print PAIRS2 ">"."$sgRID_A"."\n"."$target_seq_A"."\n";
			print PAIRS2 ">"."$sgRID_S"."\n"."$target_seq_S"."\n";

		}
  }
}
close(PA);
close(PS);
close(PAIRS1);
close(PAIRS2);

########################### Unique pairs sgR fasta seq ######################
my %seq;
my $title;
my $infile="$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_pairs.fa";

open (IN,"$infile") || die "can't open $infile!";
while (<IN>){
	$_=~s/\n//;
	$_=~s/\r//;
	if ($_=~/>/){
		$title=$_;
		$title=~s/>//;
	}
	else{
		$seq{$_}=$title;
	}
}
close IN;
#remove the abundant sequences
my @seq=keys (%seq);
my @uniqueseq;
my $find=0;
foreach (@seq){
	$find=0;
	my $seq=uc($_);
	foreach (@uniqueseq){
		if ($seq=~/$_/){
			$_=$seq;#replace with longer seq
			$find=1;
		}
		if ($_=~/$seq/){
			$find=1;
		}
	}
	if ($find==0){
		push @uniqueseq,$seq;
	}
}
#outout the final result
open (OUT,">$dir/sgRNAcas9.report_$truncat.$Option/unique_pairs.fa");
foreach (@uniqueseq){
	print OUT ">$seq{$_}\n$_\n";
}
close OUT;


###################################### seqmap ###################################
print "Step3: Evaluate CRISPR potential off-target effect.\n";
print "Step3-1: Whole genome mapping.\n\n";
print  LOG "# Step3: Evaluate CRISPR potential off-target effect.\n";
print  LOG "# Step3-1: Whole genome mapping.\n";

if ($Seqmap_vesion eq "l" and $Type eq "s") {       #Linux-64

	my @args = ("seqmap-1.0.12-linux-64","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_single.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);

	if($? == -1) {
		die "system @args failed: $?";
	}

}elsif ($Seqmap_vesion eq "l" and $Type eq "p") {

	my @args = ("seqmap-1.0.12-linux-64","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/unique_pairs.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);

	if($? == -1) {
		die "system @args failed: $?";
	}

}elsif ($Seqmap_vesion eq "w" and $Type eq "s") {   #windows 32, 64-bit

	my @args = ("seqmap-1.0.12-windows.exe","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_single.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);

	if($? == -1) {
		die "system @args failed: $?";
	}

}elsif ($Seqmap_vesion eq "w" and $Type eq "p") {   

	my @args = ("seqmap-1.0.12-windows.exe","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/unique_pairs.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);

	if($? == -1) {
		die "system @args failed: $?";
	}

}elsif ($Seqmap_vesion eq "m" and $Type eq "s") {    #macOSX-64

	my @args = ("seqmap-1.0.12-mac-64","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_single.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);
	
	if($? == -1) {
		die "system @args failed: $?";
	}

}elsif ($Seqmap_vesion eq "m" and $Type eq "p") {   

	my @args = ("seqmap-1.0.12-mac-64","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/unique_pairs.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);
	
	if($? == -1) {
	  die "system @args failed: $?";
	}

}elsif ($Seqmap_vesion eq "a" and $Type eq "s") {    #macOSX-32

	my @args = ("seqmap-1.0.12-mac","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_single.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);
	
	if($? == -1) {
	  die "system @args failed: $?";
	}

}elsif ($Seqmap_vesion eq "a" and $Type eq "p") {   

	my @args = ("seqmap-1.0.12-mac","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/unique_pairs.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);
	
	if($? == -1) {
	  die "system @args failed: $?";
	}

}elsif ($Seqmap_vesion eq "u" and $Type eq "s") {    #Linux-32

	my @args = ("seqmap-1.0.12-linux","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/CRISPR.targets_single.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);
	
	if($? == -1) {
	  die "system @args failed: $?";
	}

}elsif ($Seqmap_vesion eq "u" and $Type eq "p") {   

	my @args = ("seqmap-1.0.12-linux","$Num_mismatch", "$dir/sgRNAcas9.report_$truncat.$Option/unique_pairs.fa", "$Genome", "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt", "/output_all_matches");
	system(@args);
	
	if($? == -1) {
	  die "system @args failed: $?";
	}

}



################################## count_sgR_Targetsite #########################
print "\nFinished mapping candidate CRISPR target sequences to genome: $Genome.\n";
print "\nStep3-2: Count the No.of potential off-target cleavage sites.\n";
print  LOG "# Finished mapping candidate CRISPR target sequences to genome: $Genome.\n";
print  LOG "# Step3-2: Count the No.of potential off-target cleavage sites.\n";


open(Input, "$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt") || die "Can't open seqmap_output.txt for reading!\n";
open(Out, ">$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_count.total_OT.txt") || die "Can't open report_count.total_OT.txt for writing!\n";

print Out "sgRID\t"."Total_No.OT\n";

my %ID;
my $len1=$truncat+2;
my $len2=$truncat+1;

while (<Input>) {

	chomp $_; 
	
	(my $trans_id, my $trans_coord, my $target_seq, my $probe_id, my $probe_seq, my $num_mismatch, my $strand)=split/\t/, $_;
	
	next if ($trans_id eq "trans_id" or (substr($target_seq,$len1,1) ne "G") or (substr($target_seq,$len2,1) eq "T") or (substr($target_seq,$len2,1) eq "C") ) ;

    while ($probe_id=~ /(\w[\w-]*)/g) {
		$ID{$1}++;
	}
}

foreach (sort {$ID{$a}<=>$ID{$b}}keys %ID) {
	
	next if $_=~ /probe_id/;
    
	print Out "$_\t$ID{$_}\n";

}
close Input;
close Out;


################################## format_seqmap #################################
print "Step3-3: Format and sort whole genome mapping result.\n";
print  LOG "# Step3-3: Format and sort whole genome mapping result.\n";

mkdir("$dir/sgRNAcas9.report_$truncat.$Option/Sort_OT_byID",0755)||die "can't create directory: $!" ;
mkdir("$dir/sgRNAcas9.report_$truncat.$Option/D.Type_II_POT",0755)||die "can't create directory: $!" ;
mkdir("$dir/sgRNAcas9.report_$truncat.$Option/C.Type_I_POT",0755)||die "can't create directory: $!" ;

my $trans_id;
my $trans_coord;
my $target_seq;
my $probe_id1;
my $probe_seq;
my $num_mismatch;
my $New_num_mismatch;
my $strand;

my $sgR_ID;

my $countsgR;

my $i=0;
my $j=0;

my $len3= $truncat+2; #19
my $len4= $truncat+1; #18
my $len5= $truncat-7; #10
my $len6= $truncat-12; #5

open(Seq, "<$dir/sgRNAcas9.report_$truncat.$Option/seqmap_output.txt" ) || die "can't open seqmap_output.txt!";
open(sgR, "<$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_count.total_OT.txt" ) || die "can't open report_count.total_OT.txt!";

open(Out3, ">$dir/sgRNAcas9.report_$truncat.$Option/search_OT.txt") || die "Can't open search_OT.txt for writing!\n";
print Out3 "\n\t\t------------------------------------ off-target analysis(OT:off-target) -----------------------------------\n\n";

while (<Seq>) {
	chomp $_; 

	(my $trans_id, my $trans_coord, my $target_seq, my $probe_id1, my $probe_seq, my $num_mismatch, my $strand)=split/\t/, $_;

	my $New_num_mismatch = $num_mismatch;
   
	seek sgR, 0, 0;
	
	while (<sgR>) {
		chomp $_; 
	
		(my $sgR_ID, my $countsgR)=split/\t/, $_;

		if ($probe_id1=~/$sgR_ID$/m) {
			$j++;
			    
			my @seq1 = split //, $probe_seq;
			my @seq2 = split //, $target_seq;
			
			my @Iden;
			$i++;
			
			for my $n (0..@seq1-1) {
				if   ( ($seq1 [$n] eq 'A' && $seq2 [$n] eq 'A') 
				    || ($seq1 [$n] eq 'T' && $seq2 [$n] eq 'T') 
				    || ($seq1 [$n] eq 'C' && $seq2 [$n] eq 'C') 
				    || ($seq1 [$n] eq 'G' && $seq2 [$n] eq 'G') ) {
				
				push @Iden, '-';
				}
				else {
				
					push @Iden, lc($seq2[$n]);
				
				}
			}  
   		#discard                                                        NG[N]                                  NTG                                  NCG
	    next if ($trans_id eq "trans_id" or (substr($target_seq,$len3,1) ne "G") or (substr($target_seq,$len4,1) eq "T") or (substr($target_seq,$len4,1) eq "C") ) ;
	
	
			open (Out2,">>$dir/sgRNAcas9.report_$truncat.$Option/Sort_OT_byID/$sgR_ID.txt");
	
	   #                                                     NAG                      [N]-GG                                        
	    if($num_mismatch > 0 and (substr($target_seq,$len4,1) eq "A" or (substr($target_seq,$truncat,1) ne substr($probe_seq,$truncat,1)))){
	
	    	$New_num_mismatch=$num_mismatch-1;
	
				print Out2 "$probe_id1\t"."POT$i\t"."@Iden"."\t$New_num_mismatch"."M\t"."$trans_id\t"."$trans_coord\t"."$strand\n";
	
	    	#print Out3  "\t\t"."20           13 12      8 7           1 N G G\n";
	    	print Out3 "$probe_id1\t"."@seq1"."\n"."POT$i\t\t"."@seq2"."\n"."POT$i\t\t"."@Iden"."\t$New_num_mismatch"."M\t"."$trans_id\t"."$trans_coord\t"."$strand\n";
	
			}else {
	
				print Out2 "$probe_id1\t"."POT$i\t"."@Iden"."\t$num_mismatch"."M\t"."$trans_id\t"."$trans_coord\t"."$strand\n";
	
	    	#print Out3  "\t\t"."20           13 12      8 7           1 N G G\n";
	    	print Out3 "$probe_id1\t"."@seq1"."\n"."POT$i\t\t"."@seq2"."\n"."POT$i\t\t"."@Iden"."\t$num_mismatch"."M\t"."$trans_id\t"."$trans_coord\t"."$strand\n";
	
			}
	
			open (Out4,">>$dir/sgRNAcas9.report_$truncat.$Option/D.Type_II_POT/$sgR_ID.regionI.identity.txt");
	    #print Out4  "20           13-12      8-7           1 N G G\n";
	    next if $strand =~/strand/;
	    #                                                7mer                                                        NAG                             [N]-GG 
	    if($num_mismatch > 0 and (substr($target_seq,$len5,7) eq substr($probe_seq,$len5,7)) and ((substr($target_seq,$len4,1) eq "A") or (substr($target_seq,$truncat,1) ne substr($probe_seq,$truncat,1)))){
	
		    my $New2_num_mismatch=$num_mismatch-1;
			
		    print Out4  "@Iden\t$New2_num_mismatch"."M\tPOT$i\t$trans_id\t$trans_coord\t$strand\t$probe_id1\n";
	
	    }elsif (substr($target_seq,$len5,7) eq substr($probe_seq,$len5,7)){
	
	    	print Out4  "@Iden\t$num_mismatch"."M\tPOT$i\t$trans_id\t$trans_coord\t$strand\t$probe_id1\n";
	 
			}

 
			open (Out5,">>$dir/sgRNAcas9.report_$truncat.$Option/C.Type_I_POT/$sgR_ID.seed.identity.txt");
	    next if $strand =~/strand/;
	    #                                                12mer                                                       NAG                             [N]-GG 
	    if($num_mismatch > 0 and (substr($target_seq,$len6,12) eq substr($probe_seq,$len6,12)) and ((substr($target_seq,$len4,1) eq "A") or (substr($target_seq,$truncat,1) ne substr($probe_seq,$truncat,1)))){
	
	    	my $New3_num_mismatch=$num_mismatch-1;
		
	    	print Out5  "@Iden\t$New3_num_mismatch"."M\tPOT$i\t$trans_id\t$trans_coord\t$strand\t$probe_id1\n";
	
	    }elsif(substr($target_seq,$len6,12) eq substr($probe_seq,$len6,12)){
	
	    	print Out5  "@Iden\t$num_mismatch"."M\tPOT$i\t$trans_id\t$trans_coord\t$strand\t$probe_id1\n";
	 
			}

    }
  }
}
close(Seq);
close(sgR);
close(Out2);
close(Out3);


#################### count mismatch AND sort mapping by seed-ident.0-3Mismatch ################################
print "Step3-4: Sort mapping result by mismatch location.\n\n";
print  LOG "# Step3-4: Sort mapping result by mismatch location.\n\n";

mkdir("$dir/sgRNAcas9.report_$truncat.$Option/B.Sort_POT_byID",0755)||die "can't create directory: $!" ;

my $Dir = "$dir/sgRNAcas9.report_$truncat.$Option/Sort_OT_byID" ;
my $Ext = "txt" ; #file type

opendir(DH, "$Dir") or die "Can't open: $!\n" ;

my @list = grep {/$Ext$/ && -f "$Dir/$_" } readdir(DH);

closedir(DH) ;
chdir($Dir) or die "Can't cd dir: $!\n" ;

my $file;

foreach my $file (@list){

	open(FH, "$file") || die "Can't open $file for reading!\n";
	
	$file =~ s/.txt//m;
	
	open(OUTi,">>$dir/sgRNAcas9.report_$truncat.$Option/B.Sort_POT_byID/$file.POT.txt") || die "Can't open $file.POT.txt for writing!\n";
	open(OUTM,">>$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_total_OT.txt") || die "Can't open report_total_OT.txt for writing!\n";
	#print OUTi  "20           13 12      8 7           1 N G G\n";
	print OUTM "$file:\t" ;
	
	my %ID;

	while (<FH>) {
 		chomp $_; 

  	(my  $sgRID, my $POT, my $SEQ, my $mismatch, my $Chr, my $Location, my $strand)=split/\t/, $_;

    while ($mismatch=~ /((\d)M|\w[\w-]*M)/g) {

	  	{
			$ID{$1}++;
			}
		}


		#next if ($mismatch eq "0M");
		#$truncat = 17, 18, 19, 20 nt sgRNA
		my $lentru17_7 = $truncat+3; #20
		my $lentru17_12= $truncat-7; #10
		
		my $lentru18_7 = $truncat+4; #22
		my $lentru18_12= $truncat-6; #12
		
		my $lentru19_7 = $truncat+5; #24
		my $lentru19_12= $truncat-5; #14
		
		my $lentru20_7 = $truncat+6; #26
		my $lentru20_12= $truncat-4; #16
		
		
		######                                                            7mer                                                 12mer
		#$truncat = 17
		if ($truncat ==17 and $mismatch =~/0M|1M|2M|3M/g and ((substr($SEQ,$lentru17_7,13) ne "- - - - - - -") or (substr($SEQ,$lentru17_12,23) ne "- - - - - - - - - - - -") )) {  
		
			print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."random_0_3M\t"."\n";
		
		}elsif ($truncat ==17 and (substr($SEQ,$lentru17_7,13) eq "- - - - - - -") and (substr($SEQ,$lentru17_12,23) ne "- - - - - - - - - - - -")) {
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."regionI_ident\t"."\n";
		
		}elsif ($truncat ==17 and(substr($SEQ,$lentru17_12,23) eq "- - - - - - - - - - - -")) {
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."seed_ident\t"."\n";
		
		 #$truncat = 18
		}elsif ($truncat ==18 and $mismatch =~/0M|1M|2M|3M/g and ((substr($SEQ,$lentru18_7,13) ne "- - - - - - -") or (substr($SEQ,$lentru18_12,23) ne "- - - - - - - - - - - -") )) {  
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."random_0_3M\t"."\n";
		
		}elsif ($truncat ==18 and (substr($SEQ,$lentru18_7,13) eq "- - - - - - -") and (substr($SEQ,$lentru18_12,23) ne "- - - - - - - - - - - -")) {
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."regionI_ident\t"."\n";
		
		}elsif ($truncat ==18 and(substr($SEQ,$lentru18_12,23) eq "- - - - - - - - - - - -")) {
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."seed_ident\t"."\n";
		
		 #$truncat = 19
		}elsif ($truncat ==19 and $mismatch =~/0M|1M|2M|3M/g and ((substr($SEQ,$lentru19_7,13) ne "- - - - - - -") or (substr($SEQ,$lentru19_12,23) ne "- - - - - - - - - - - -") )) {  
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."random_0_3M\t"."\n";
		
		}elsif ($truncat ==19 and (substr($SEQ,$lentru19_7,13) eq "- - - - - - -") and (substr($SEQ,$lentru19_12,23) ne "- - - - - - - - - - - -")) {
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."regionI_ident\t"."\n";
		
		}elsif ($truncat ==19 and(substr($SEQ,$lentru19_12,23) eq "- - - - - - - - - - - -")) {
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."seed_ident\t"."\n";
		
		 #$truncat = 20
		}elsif ($truncat ==20 and $mismatch =~/0M|1M|2M|3M/g and ((substr($SEQ,$lentru20_7,13) ne "- - - - - - -") or (substr($SEQ,$lentru20_12,23) ne "- - - - - - - - - - - -") )) {  
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."random_0_3M\t"."\n";
		
		}elsif ($truncat ==20 and (substr($SEQ,$lentru20_7,13) eq "- - - - - - -") and (substr($SEQ,$lentru20_12,23) ne "- - - - - - - - - - - -")) {
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."regionI_ident\t"."\n";
		
		}elsif ($truncat ==20 and(substr($SEQ,$lentru20_12,23) eq "- - - - - - - - - - - -")) {
		
		  print OUTi "$SEQ\t"."$mismatch\t"."$sgRID"."_$POT\t"."$Chr\t"."$Location\t"."$strand\t"."seed_ident\t"."\n";
		}
  }

######
    foreach (sort {$ID{$a} <=> $ID{$b}}keys %ID) {
 
			print OUTM "$_"."\t"."$ID{$_}"."\t";

    }
    print OUTM "\n";

}
close(FH) ;
close (OUTM);


############### re-count mismatch by location and No.of mismatch.0-3M ############
mkdir("$dir/sgRNAcas9.report_$truncat.$Option/F.Sort_count.POT",0755)||die "can't create directory: $!" ;

my $Dir2 = "$dir/sgRNAcas9.report_$truncat.$Option/B.Sort_POT_byID" ;
my $Ext2 = "txt" ; #file type

opendir(DH2, "$Dir2") or die "Can't open: $!\n" ;

my @list2 = grep {/$Ext2$/ && -f "$Dir2/$_" } readdir(DH2);

closedir(DH2) ;
chdir($Dir2) or die "Can't cd dir: $!\n" ;

my $file2;

foreach my $file2 (@list2){

	open(FH2, "$file2") || die "Can't open $file2 for reading!\n";
	
	$file2 =~ s/.txt//m;
	
	
	open(OUTM2,">>$dir/sgRNAcas9.report_$truncat.$Option/F.Sort_count.POT/$file2.count.txt") || die "Can't open $file2.count.txt for writing!\n";
	
	#print OUTM2 "\t$file2\n\n" ;
	
	#open(OUTM3,">>$dir/sgRNAcas9.report_$truncat.$Option/F.Sort_count.POT.combine.txt") || die "Can't open F.Sort_count.POT.combine.txt for writing!\n";
	
	#print OUTM3 "\t\n$file2\n\n" ;
	
	#open(OUTM4,">>$dir/sgRNAcas9.report_$truncat.$Option/count.sort.POT.byNoofmismatch.txt") || die "Can't open count.sort.POT.byNoofmismatch.txt for writing!\n";
	
	#print OUTM4 "\t$file2\n" ;
	
	#open(OUTM5,">>$dir/sgRNAcas9.report_$truncat.$Option/count.sort.POT.bylocationofmismatch.txt") || die "Can't open count.sort.POT.bylocationofmismatch.txt for writing!\n";
	
	#print OUTM5 "\t$file2\n" ;
	
	my %IDs;
	my %Mml;
	my $Mmlocation="";
	my $mismatch2;

	while (<FH2>) {
 		chomp $_; 
 
  	(my $SEQ2, my $mismatch2, my $sgRid, my $Chr2, my $Location2, my $strand2, my $Mmlocation)=split/\t/, $_;

    next if $mismatch2 eq "";

    while ($mismatch2=~ /((\d)M|\w[\w-]*M)/g) {

	  	{
			$IDs{$1}++;
			}
		}

###
    next if $Mmlocation eq "";

    while ($Mmlocation=~ /(\w[\w-]*)/g) {

		  {
				$Mml{$1}++;
			}
		}

###
    #print OUTM2 "\nLocation of mismatches:\n";
    #print OUTM3 "Location of mismatches:\n";

    if ($Mmlocation eq "seed_ident") {

	    print OUTM2 "Region_III\t".$mismatch2."\t".$Mmlocation."\t".$sgRid."\n";
	    #print OUTM3 "Region:III\t".$mismatch2."\t".$Mmlocation."\t".$sgRid."\n";

		}elsif ($Mmlocation eq "regionI_ident") {

	    print OUTM2 "Region_II_III\t".$mismatch2."\t".$Mmlocation."\t".$sgRid."\n";
	    #print OUTM3 "Region:II&III\t".$mismatch2."\t".$Mmlocation."\t".$sgRid."\n";

		}elsif ($Mmlocation eq "random_0_3M") {

	    print OUTM2 "Region_I_II_III\t".$mismatch2."\t".$Mmlocation."\t".$sgRid."\n";
	    #print OUTM3 "Region:I&IIorIII\t".$mismatch2."\t".$Mmlocation."\t".$sgRid."\n";
		}

	}
###

    #print OUTM3 "\nCount No.of mismatches:\n";
	#print OUTM4 "\nCount No.of mismatches:\n";

	foreach (sort {$IDs{$a} <=> $IDs{$b}}keys %IDs) {
 
		#print OUTM3 "\t"."$_"."\t"."$IDs{$_}"."\n";
		#print OUTM4 $file2."\t"."$_"."\t"."$IDs{$_}"."\n";

	}


###
    #print OUTM3 "\nCount location of mismatches:\n";
	#print OUTM5 "\nCount location of mismatches:\n";

	foreach (sort {$Mml{$a} <=> $Mml{$b}}keys %Mml) {

 		if ("$_" eq "seed_ident" ) {
 
			#print OUTM3 "Region:III\t"."$Mml{$_}"."\t"."$_"."\n";
			#print OUTM5 $file2."\t"."Region:III\t"."$Mml{$_}"."\t"."\n";

		}elsif ("$_" eq "regionI_ident") {

			#print OUTM3 "Region:II&III\t"."$Mml{$_}"."\t"."$_"."\n";
			#print OUTM5 $file2."\t"."Region:II&III\t"."$Mml{$_}"."\t"."\n";

		}elsif ("$_" eq "random_0_3M") {

			#print OUTM3 "Region:I&IIorIII\t"."$Mml{$_}"."\t"."$_"."\n";
			#print OUTM5 $file2."\t"."Region:I&IIorIII\t"."$Mml{$_}"."\t"."\n";
		}

	}

}
close(FH2) ;
close (OUTM2);
#close (OUTM3);
#close (OUTM4);
#close (OUTM5);


################
my $Dir3 = "$dir/sgRNAcas9.report_$truncat.$Option/F.Sort_count.POT" ;
my $Ext3 = "txt" ; #file type

opendir(DH3, "$Dir3") or die "Can't open: $!\n" ;

my @list3 = grep {/$Ext3$/ && -f "$Dir3/$_" } readdir(DH3);

closedir(DH3) ;
chdir($Dir3) or die "Can't cd dir: $!\n" ;

my $file3;

foreach my $file3 (@list3){

	open(FH3, "$file3") || die "Can't open $file3 for reading!\n";
	
	$file3 =~ s/.count.txt//m;
	
	
	open(OUTM6,">>$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_total_POT.txt") || die "Can't open report_total_POT.txt for writing!\n";
	
	#print OUTM6 "$file3:\n";
	
	my $type="";
	my %Ntype;
	my $line="";
	my @m   ="";

	while ($line=<FH3>){

		next if $line eq "";
    chomp($line);

		@m=split(/\s+/,$line);

    $type=$m[0]."_".$m[1];

    #print $type."\n";

 		while ($type=~ /(\w[\w-]*)/g) {

  		next if $type eq "";

  		{
			$Ntype{$1}++;
			}
		}
	}
	foreach (sort {$Ntype{$a} <=> $Ntype{$b}}keys %Ntype) {

#####
    #next if ($_ eq "Region_III_0M") and ($Ntype{$_} == 1);

		print OUTM6 $file3."\t"."$_"."\t"."$Ntype{$_}"."\n";
	#print OUTM6 "$_"."\t"."$Ntype{$_}"."\n";
	}

}



############################### Sort and format mapping ##########################
opendir(DH, "$dir/sgRNAcas9.report_$truncat.$Option/Sort_OT_byID")  or die "Couldn't open $dir for reading: $!";
open (FH, ">$dir/sgRNAcas9.report_$truncat.$Option/sgRID_filename.txt") or die "Couldn't open sgRID_filename.txt for writing: $!";

my @files = ();

while( defined ($file = readdir(DH)) ) {

	next if $file =~ /^\.\.$/;     # skip . and ..

	my $filename = "$file";
    
	print FH "$filename\n";

  push(@files, $filename);
}
closedir(DH);


############################# sort.mapping_bymismatch ##############################
mkdir("$dir/sgRNAcas9.report_$truncat.$Option/E.Type_III_POT",0755)||die "can't create directory: $!" ;
open (FH, "$dir/sgRNAcas9.report_$truncat.$Option/sgRID_filename.txt")|| die "can't open sgRID_filename.txt!";
opendir(SORT,"$dir/sgRNAcas9.report_$truncat.$Option/E.Type_III_POT") || die "Cann't open E.Type_III_POT !";

my $Inputfile="";
my $fileID;

foreach $fileID (1.. $#files) {

	$Inputfile = $files[$fileID];

	#next if $Inputfile eq "";
	#if ($Inputfile eq ".txt"){}else {

		open (Inputs, "$dir/sgRNAcas9.report_$truncat.$Option/Sort_OT_byID/$Inputfile") || die "Can't open $Inputfile for reading!\n";
		mkdir("$dir/sgRNAcas9.report_$truncat.$Option/E.Type_III_POT/$Inputfile.sort.mapping_bymismatch",0755)||die "can't create directory: $!" ;
		open(OUTM2,">>$dir/sgRNAcas9.report_$truncat.$Option/E.Type_III_POT/$Inputfile.sort.mapping_bymismatch/report_mismatch_$Inputfile") || die "Can't open report_mismatch_$Inputfile for writing!\n";

		open (Outs,  ">$dir/sgRNAcas9.report_$truncat.$Option/E.Type_III_POT/$Inputfile.sort.mapping_bymismatch/1-3mismatch_$Inputfile") || die "Can't open 1-3mismatch_$Inputfile for writing!\n";
		open (Outs1, ">$dir/sgRNAcas9.report_$truncat.$Option/E.Type_III_POT/$Inputfile.sort.mapping_bymismatch/4mismatch_$Inputfile") || die "Can't open 4mismatch_$Inputfile for writing!\n";
		open (Outs2, ">$dir/sgRNAcas9.report_$truncat.$Option/E.Type_III_POT/$Inputfile.sort.mapping_bymismatch/5mismatch_$Inputfile") || die "Can't open 5mismatch_$Inputfile for writing!\n";

		#print Outs   "20           13-12      8-7           1 N G G\n";
		print Outs   "- - - - - - - - - - - - - - - - - - - - - - -   Mismatch\tPOTid\tChromosome\tLocation\tStrand\tsgRid\n";

		#print Outs1  "20           13-12      8-7           1 N G G\n";
		print Outs1  "- - - - - - - - - - - - - - - - - - - - - - -   Mismatch\tPOTid\tChromosome\tLocation\tStrand\tsgRid\n";

		#print Outs2  "20           13-12      8-7           1 N G G\n";
		print Outs2  "- - - - - - - - - - - - - - - - - - - - - - -   Mismatch\tPOTid\tChromosome\tLocation\tStrand\tsgRid\n";

		my %Nmismatch;

		while (<Inputs>) {
  chomp $_; 

  (my  $sgRID, my $POT, my $SEQ, my $mismatch, my $Chr, my $Location, my $strand)=split/\t/, $_;

   while ($mismatch=~ /((\d)M|\w[\w-]*M)/g) {

    {
		$Nmismatch{$1}++;
	 }
    }


######
if ($mismatch =~/0M|1M|2M|3M/g) {
  
  print Outs "$SEQ\t". "$mismatch\t". "$POT\t"."$Chr\t". "$Location\t". "$strand\t"."$sgRID\n";

  }elsif ($mismatch =~/4M/g ) {

  print Outs1 "$SEQ\t". "$mismatch\t". "$POT\t"."$Chr\t". "$Location\t". "$strand\t"."$sgRID\n";

  }elsif ($mismatch =~/5M/g ) {

  print Outs2 "$SEQ\t". "$mismatch\t". "$POT\t"."$Chr\t". "$Location\t". "$strand\t"."$sgRID\n";

  }
		}

	######
    foreach (sort {$Nmismatch{$a} <=> $Nmismatch{$b}}keys %Nmismatch) {
    
	print OUTM2 "$_"."\t"."$Nmismatch{$_}"."\n";
    
    }
    print OUTM2 "\n";

	}
#}
close (Inputs);
close (Outs);
close (Outs1);
close (Outs2);
close (OUTM2);
close (FH);
closedir(SORT);

####################### classfy POT & select CRSIPR target site ##################

mkdir("$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT",0755)||die "Can't create directory: Directory exists at $dir. Please delete, move or rename the exist directory before you run this program.$!" ;
my $Input_pot="$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_total_POT.txt";

open(POTi, $Input_pot) ||die "can't open $Input_pot!" ;
open(POTo, ">$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT/POT_more.OM.txt") ||die "can't open POT_more.OM.txt!" ;
open(POTo1, ">$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT/POT_0M.txt") ||die "can't open POT_0M.txt!" ;
open(POTo2, ">$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT/POT_contain1_2M.txt") ||die "can't open POT_contain1_2M.txt!" ;
open(POTo3, ">$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT/POT_classfy.txt") ||die "can't open POT_classfy.txt!" ;

print POTo3 "Type:A, more than one target site perfect match to genome(off-target)\n";
print POTo3 "Type:B, only one target site perfect match to genome(on-target)\n";
print POTo3 "Type:C, contain 1 or 2 mismathed bases\n\n";
print POTo3 "POT_ID\tMismatches_position_number\tNo.of_target_sites\n";

my $crisp_ID;
my $Mmlocation;
my $count;

while (<POTi>) {
	chomp $_; 

	(my $crisp_ID, my $Mmlocation, my $count)=split/\t/, $_;

   	my $crisp_pot=$crisp_ID;

	$crisp_pot =~ s/.POT//m;

    $Mmlocation=~ /\_(\d+)M$/g;

    my $mismatch = $1;

  if ($Mmlocation eq "Region_III_0M" and $count >1 ) {

	  print POTo $crisp_pot."\t".$Mmlocation."\t".$count."\n";
	  print POTo3 "Type:A"."\t".$crisp_pot."\t".$Mmlocation."\t".$count."\n";

  }elsif ($Mmlocation eq "Region_III_0M" and $count ==1) {

	  print POTo1 $crisp_pot."\n";
	  print POTo3 "Type:B"."\t".$crisp_pot."\t".$Mmlocation."\t".$count."\n";

  }elsif ($mismatch == 1 or $mismatch == 2 ) {

	  print POTo2 $crisp_pot."\n";
	  print POTo3 "Type:C"."\t".$crisp_pot."\t".$Mmlocation."\t".$count."\n";
	}
}

close POTi;
close POTo;
close POTo1;
close POTo2;
close POTo3;

############## compare files POT_0M.txt & POT_contain1_2M.txt ############################
open (MC, "$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT/POT_0M.txt")||die "can't open POT_0M.txt!" ;                      
open (MM, "$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT/POT_contain1_2M.txt")||die "can't open POT_contain1_2M.txt!" ;

open (SEL, ">$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT/Select.POT_0M.txt")||die "can't Select.POT_0M.txt!" ;              
open (MMI2, ">$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT/POT_0M.contain1_2M.txt")||die "can't open POT_0M.contain1_2M.txt!" ;

my @line = <MC>;           #$pot_0:  POT_0M
my @line2 = <MM>;          #$pot_12: POT_0M.contain1_2M
my $pot_0;
my $pot_12;
my $flag;

foreach $pot_0(@line){

	$flag = 0;

	foreach $pot_12(@line2){

		if ($pot_12 eq $pot_0){

			$flag =1; 

			print MMI2 $pot_0;

			#last;
		}
	}
	if ($flag != 1) {

		print SEL $pot_0;

	}
}

close MC;
close MM;
close SEL;
close MMI2;

####################### report selected candidate CRISPR.sites #######################

my $CRISPR_id="$dir/sgRNAcas9.report_$truncat.$Option/G.Analysis.POT/Select.POT_0M.txt";
my $CRISPR_info= "$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_total_POT.txt";

open(ID, "$CRISPR_id" ) || die "can't open $CRISPR_id!";
open(INFO, "$CRISPR_info" ) || die "can't open $CRISPR_info!";
open(SELECT, ">$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_candidate_protospacer_POT.txt" ) || die "can't open report_candidate_protospacer_POT.txt!";

my $pot_id;
my $Mmlocation2;
my $count2;
my $pot_id_new;
my $select_id;

while ( <ID> ) {
	chomp();
	(my $select_id)=split/\t/, $_;

  seek INFO, 0, 0;

  while ( <INFO> ) {
	  chomp();
		(my $pot_id, my $Mmlocation2, my $count2)=split/\t/, $_;
	
		$pot_id_new = $pot_id;
		$pot_id_new=~ s/.POT//m;
	
		if ($pot_id_new eq $select_id) { 
	
			next if ($Mmlocation2 eq "Region_III_0M") and ($count2 == 1);
			#print $pot_id_new."\t".$select_id."\n";
			print SELECT $pot_id."\t".$Mmlocation2."\t".$count2."\n";
	
		}
  }
}
close(ID);
close(INFO);
close(SELECT);


############################ report count POT_total ##############################

my $total_POT_info= "$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_total_POT.txt";

my @array_pot1="";
my (%hash_pot1, $hash_pot1, $i_pot1);

open(POTin, $total_POT_info) || die "Can't open $total_POT_info for reading!\n";
open(POTout, ">$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_count_total_POT.txt") || die "Can't open report_count_total_POT.txt for writing!\n";

print POTout "sgRID\tTotal_No.POT\n";

while(<POTin>){

	chomp;
	
	@array_pot1=split/\t/;
	
	$hash_pot1{$array_pot1[0]}+=$array_pot1[2];

}

foreach $i_pot1 (sort keys %hash_pot1){

	print POTout "$i_pot1\t$hash_pot1{$i_pot1}\n";

}

close (POTin);
close (POTout);

############################ report count POT_select ##############################

my $total_POT_select= "$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_candidate_protospacer_POT.txt";

my @array_pot2="";
my (%hash_pot2, $hash_pot2, $i_pot2);

open(POTsin, $total_POT_select) || die "Can't open $total_POT_select for reading!\n";
open(POTsout, ">$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_count_candidate.protospacer_POT.txt") || die "Can't open report_count_candidate.protospacer_POT.txt for writing!\n";

print POTsout "sgRID\tTotal_No.POT\n";

while(<POTsin>){

	chomp;
	
	@array_pot2=split/\t/;
	
	$hash_pot2{$array_pot2[0]}+=$array_pot2[2];

}

foreach $i_pot2 (sort keys %hash_pot2){

	print POTsout "$i_pot2\t$hash_pot2{$i_pot2}\n";

}

close (POTsin);
close (POTsout);


############################ final OT and POT_select ##############################

my $candidate= "$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_count_candidate.protospacer_POT.txt";
my $totalOT= "$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/report_count.total_OT.txt";

open(FILE1, $candidate) || die "Can't open $candidate for reading!\n";
open(FILE2, $totalOT) || die "Can't open $totalOT for reading!\n";
            
open FILE3,">$dir/sgRNAcas9.report_$truncat.$Option/A.Final_report/final_count_POT.OT_candidate.protospacer.txt";

my %file1;

while(<FILE1>){
	chomp;
	
	my @a=split/\t/, $_;
	
	$a[0] =~ s/.POT//m;
	
	$file1{$a[0]}=$a[1];

}
###
while(<FILE2>){

	my @q=split " ";
	
	if(exists $file1{$q[0]}){
	
		print FILE3 "$q[0]\t$file1{$q[0]}\t$q[1]\n";

  }
}
close FILE1;
close FILE2;
close FILE3;


################################### subroutine ###################################
sub analysis {  
	if($ID eq "" || $seq eq "") {
	  return();
	}
	#my $seq =~ s/\n//g;
	my $SEQRNA  = uc($seq);
	my $SEQDNA  = $SEQRNA;
	$SEQDNA  =~ tr/U/T/;

	my $len = length($seq);

	my $idx_s;
	my $i = 0;

	for($idx_s=0; $idx_s<length($seq); $idx_s++) {

		my $lentruncat = $truncat+3;
		my $lentruncat2 = $truncat+1;

		if (substr($SEQDNA, $idx_s+1, $lentruncat) =~ /[ATCG]{$lentruncat2}GG/) {    #X?-NGG  
			my $sgRidx_s = $idx_s+2;                                  #For sense strand
			my $sgR = $&;
			my $sgRAT = &AT($sgR);
			my $sgRGC = &GC($sgR);
			#my $SgRsd = substr($sgR,8,12);                           #5-X12-NGG-3
			#my $SgRseed ="$SgRsd"."NGG";
			my $SgRNt = substr($sgR,0,$truncat);                      #truncat
			my $sgRlen = length($sgR);
			my $sgRidx_e = $sgRidx_s+$sgRlen-1;
			$i++;
			
			my $sgRidx_A_s =$len-$sgRidx_e+1;                         #For anti-sense strand
			my $sgRidx_A_e =$len-$sgRidx_s+1;
			
			my $sgrna = $SgRNt;
			$sgrna =~ tr/T/U/;
			my $sgRNA = $sgrna.$dCas9handle.$terminator;

				if (!($sgR =~ /T{4,18}/g)){
					#if (!($sgR =~ /A{5,21}|T{4,21}|C{6,21}|G{6,21}|(AT){6,10}|(AC){6,10}|(AG){6,10}|(TA){6,10}|(TC){6,10}|(TG){6,10}|(CA){6,10}|(CT){6,10}|(CG){6,10}|(GA){6,10}|(GT){6,10}|GC{6,10}|(AAT){5,7}|(AAC){5,7}|(AAG){5,7}|(ATA){5,7}|(ATT){5,7}|(ATC){5,7}|(ATG){5,7}|(ACA){5,7}|(ACT){5,7}|(ACC){5,7}|(ACG){5,7}|(AGA){5,7}|(AGT){5,7}|(AGC){5,7}|(AGG){5,7}|(TAA){5,7}|(TAT){5,7}|(TAC){5,7}|(TAG){5,7}|(TTA){5,7}|(TTC){5,7}|(TTG){5,7}|(TCA){5,7}|(TCT){5,7}|(TCC){5,7}|(TCG){5,7}|(TGA){5,7}|(TGT){5,7}|(TGC){5,7}|(TGG){5,7}|(CAA){5,7}|(CAT){5,7}|(CAC){5,7}|(CAG){5,7}|(CTA){5,7}|(CTT){5,7}|(CTC){5,7}|(CTG){5,7}|(CCA){5,7}|(CCT){5,7}|(CCG){5,7}|(CGA){5,7}|(CGT){5,7}|(CGC){5,7}|(CGG){5,7}|(GAA){5,7}|(GAT){5,7}|(GAC){5,7}|(GAG){5,7}|(GTA){5,7}|(GTT){5,7}|(GTC){5,7}|(GTG){5,7}|(GCA){5,7}|(GCT){5,7}|(GCC){5,7}|(GCG){5,7}|(GGA){5,7}|(GGT){5,7}|(GGC){5,7}/g) ){  

					if ($ID=~ /._S$/ and ($sgRGC >= $GC_l and $sgRGC < $GC_m)) {    #For sense strand
           
						print OutPut "$ID\_$i\t"."$sgRidx_s\t"."$sgRidx_e\t"."$sgR\t"."$sgRlen\t"."$sgRGC %\n";
						print OutPut3 "$ID\_$i\t"."$sgRidx_s\t"."$sgRidx_e\t"."$sgR\t"."$sgRlen\t"."$sgRGC %\n";
						
						print OutPut1 ">$ID\_$i\n"."$sgR\n";
						#print OutPut2 ">$ID\_$i\n"."$sgRNA"."\n";

					}elsif ($ID=~ /._A$/ and ($sgRGC >= $GC_l and $sgRGC < $GC_m) ) {   #For anti-sense strand
           
						print OutPut "$ID\_$i\t"."$sgRidx_A_s\t"."$sgRidx_A_e\t"."$sgR\t"."$sgRlen\t"."$sgRGC %\n";
						print OutPut4 "$ID\_$i\t"."$sgRidx_A_s\t"."$sgRidx_A_e\t"."$sgR\t"."$sgRlen\t"."$sgRGC %\n";
						
						print OutPut1 ">$ID\_$i\n"."$sgR\n";
						#print OutPut2 ">$ID\_$i\n"."$sgRNA"."\n";

    			}
				}
		}
	}
}


#######subroutine to calculate GC% content
sub GC { 

    my $seq2 = shift @_; 

    $seq2 = uc($seq2);

    my $seq2DNA;
    $seq2DNA = $seq2;
    $seq2DNA =~ tr/U/T/;

    my $seq2length = length($seq2DNA);

    my $count = 0;
    for (my $i = 0; $i < $seq2length; $i++) {
			my $sub = substr($seq2DNA,$i,1);
			if ($sub =~ /G|C/i) {
	    	$count++;
			}
    }
    my $gc = sprintf("%.1f",$count * 100 /$seq2length);
    return $gc;
}


#######subroutine to calculate AT% content
sub AT { 

    my $seq2 = shift @_; 

    $seq2 = uc($seq2);

    my $seq2DNA;
    $seq2DNA = $seq2;
    $seq2DNA =~ tr/U/T/;

    my $seq2length = length($seq2DNA);

    my $count = 0;
    for (my $i = 0; $i < $seq2length; $i++) {
			my $sub = substr($seq2DNA,$i,1);
			if ($sub =~ /A|T/i) {
	    	$count++;
			}
    }
    my $gc = sprintf("%.1f",$count * 100 /$seq2length);
    return $gc;
}


close IntPut;
close OutPut;
#exit;


######################################## Running Time #################################################
my $oo = time() -  $oldtime;
printf  "\nTotal time consumption is $oo second.\n";

printf LOG "# Total time consumption is $oo seconds." ."\n"; 
print "Your job is done, open sgRNAcas9.report_$truncat.$Option directory to check result.". "\n";
print  LOG "# Your job is done, open sgRNAcas9.report_$truncat.$Option directory to check result.". "\n";

print  LOG "################################# END ###########################################"."\n\n";



