#!/usr/bin/perl
use strict;
use warnings;
use File::Basename;
use File::Spec;

# Get command line arguments
my $db        = $ARGV[0];
my $mast      = '~/meme/bin/mast'; 
my $meme_DIR  = $ARGV[1];
my $dir       = "";
my $ext       = "";
my $file      = "";

# ($file,$dir,$ext) = fileparse($nt, qr/\.[^.]*/);

my $out       = "$meme_DIR.MAST_OUT/";

mkdir $out;
print "MAST output root is $out\n\n";


# Start batch invoke task......
opendir (meme, $meme_DIR) or die $!;

    while (my $txt = readdir(meme)) {

         print "   ===> file://$txt\n";
       
         if ($txt eq ".")  {
             next; 
         }
         if ($txt eq "..") {
             next; 
         }   

         $txt = "$meme_DIR/$txt";

         my $name = $txt;
        ($name,$dir,$ext) = fileparse($txt, qr/\.[^.]*/);

         my $outDIR = "$out/$name/"; 

         mkdir $outDIR;
         # Opening the directory
         opendir (DIR, $db) or die $!;

         # Reading the directory
         while (my $siteFa = readdir(DIR)) {
           
             print "   ===> file://$siteFa\n";
       
             if ($siteFa eq ".")  {
                 next; 
             }
             if ($siteFa eq "..") {
                 next; 
             }   
    
             $siteFa = "$db/$siteFa"; 

             my $matrix = "$siteFa";    
 
            ($matrix,$dir,$ext) = fileparse($siteFa, qr/\.[^.]*/);
  
            my $outDir = "$outDIR/$matrix/";      
            my $args   = "$siteFa $txt -oc \"$outDir\"";
               $args   = "$mast $args &";
  
            print "$args\n\n";
            system($args);
			
			# sleep(1);
        }

    closedir(DIR);
	
	sleep(10);
	
    }
  
closedir(meme);

