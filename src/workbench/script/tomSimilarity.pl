#!/usr/bin/perl
use strict;
use warnings;
use File::Basename;
use File::Spec;

# Get command line arguments
my $directory = $ARGV[0];
my $meme      = '~/meme/bin/meme'; 
my $dir       = "";
my $ext       = "";
my $file      = "";
my $out       = $ARGV[1];

mkdir $out;
print "TOMTOM output root is $out\n\n";

# Opening the directory
opendir (DIR, $directory) or die $!;

# Reading the directory
while (my $ma = readdir(DIR)) {
           
    # ma is the query file.       
    print "   ===> dir://$ma\n";
       
    if ($ma eq ".")  {
        next; 
    }
    if ($ma eq "..") {
        next; 
    }   
        
    # Start batch invoke task......
    my $maDIR = "$directory/$ma";
	    
    # query data batch output directory
	mkdir "$out/$ma/";	
    
	print "try open source directory   $ma\n";
    # Opening the directory
    opendir (hitDIR, $directory) or die $!;

    # Reading the directory
    while (my $mb = readdir(hitDIR)) {
            
       if ($mb eq ".")  {
           next; 
       }
       if ($mb eq "..") {
           next; 
       }   
    
       # file is the target file
       $file = "$directory/$mb"; 
       print "   ===> file://$file\n";

       my $matrix = "$file";          
       my $outDir = "$out/$ma/$mb/";      
       my $args   = "-oc \"$outDir\" $maDIR $file";
          $args   = "~/meme/bin/tomtom $args";
  
       mkdir $outDir;
     
       print "$args\n\n";
       system($args);	   
    }
    closedir(hitDIR);  	
}
closedir(DIR);
