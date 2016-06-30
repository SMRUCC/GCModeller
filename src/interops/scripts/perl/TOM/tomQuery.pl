#!/usr/bin/perl
use strict;
use warnings;
use File::Basename;
use File::Spec;

# Get command line arguments
my $query = $ARGV[0];
my $db    = $ARGV[1];
my $out   = $ARGV[2];


mkdir $out;
print "TomTOM output root is $out\n\n";
 

opendir (DIR, $query) or die $!;
while (my $meme = readdir(DIR)) {
	if ($meme eq ".")  {
        	next; 
    	}
    	if ($meme eq "..") {
        	next; 
    	}
	
	print "$meme\n";

	my $outDIR = "$out/$meme/";  
	my $queryMEME = "$query/$meme";

	mkdir $outDIR;

	# Comparing with RegPrecise family
	opendir (RegPrecise, $db) or die $!;
	while (my $subject = readdir(RegPrecise)) {
		if ($subject eq ".")  {
        		next; 
    		}
    		if ($subject eq "..") {
        		next; 
    		}
		if ($subject =~ m/\.txt/) {
  			print "Motif Family Db ==> $subject\n";
		} else {
  			next;
		}

		

		my $tomOUT = "$outDIR/$subject";
		   $subject= "$db/$subject";
		
		my $args = "/home/xieguigang/meme/bin/tomtom -oc $tomOUT -evalue -no-ssc $queryMEME $subject &";
		print "$args\n";
		system($args);
	}
	closedir(RegPrecise);
}

closedir(DIR);
