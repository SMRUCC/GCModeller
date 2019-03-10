#!/usr/bin/perl
use strict;
use warnings;
use File::Basename;
use File::Spec;

# perl ./bbh_nq21db.pl ./faa/ ./RegPrecise.fasta ./ncbi-blast-2.4.0+/bin/

my $qDIR  = $ARGV[0];
my $db    = $ARGV[1];
my $out   = "$qDIR.blastp_bbh/";
my $blast = $ARGV[2];

my $dir  = "";
my $ext  = "";
my $file = "";

mkdir $out;
print "NCBI blastp+ bbh exports to $out\n\n";
system("$blast/makeblastdb -in \"$db\" -dbtype prot");

opendir DIR,"$qDIR" or die "$qDIR:$!";  

while (my $faa = readdir(DIR)) {
	
	print "   ===> file://$faa\n";
	
	if ($faa eq ".")  {
		next; 
	}
	if ($faa eq "..") {
		next; 
	}   
	
	$faa = "$qDIR/$faa";

    my $name = $faa;
    ($name,$dir,$ext) = fileparse($faa, qr/\.[^.]*/);
	
	if ($ext ne ".faa") {
		print "suffix:=$ext, NO!\n\n";
		next;
	}
	
	system("$blast/makeblastdb -in \"$faa\" -dbtype prot");
	system("$blast/blastp -query \"$faa\" -db \"$db\" -out \"$out/$name.vs__db.txt\" -evalue 1e-5 -num_threads 2 &");
	system("$blast/blastp -db \"$faa\" -query \"$db\" -out \"$out/db.vs__$name.txt\" -evalue 1e-5 -num_threads 2");
}
closedir DIR;  