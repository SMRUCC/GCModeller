package Taxonomy;

#########################################
#
# scriptname: Taxonomy.pm
#
# Author: Susan Huse, shuse@mbl.edu
#
# Date: May 2008
#
# Copyright (C) 2008 Marine Biological Laborotory, Woods Hole, MA
# 
# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU General Public License
# as published by the Free Software Foundation; either version 2
# of the License, or (at your option) any later version.
# 
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
# 
# For a copy of the GNU General Public License, write to the Free Software
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
# or visit http://www.gnu.org/copyleft/gpl.html
#
# Keywords : remove the space before the colon and list keywords separated by a space
# 
########################################

use strict;
use warnings;

our $VERSION = '1.0';

#
# Create taxonomic objects
# Return classes or full text of a taxonomy object
# Calculate consensus of an array of taxonomic objects

# Create a new taxonomy object
sub new  
{
	my $class = shift;
	my $inString = shift;

	if (! $inString) {$inString="Unknown"};
	my @data = split(";", $inString);
	#for (my $i = 0; $i <= 7; $i++)
	
	for (my $i = 0; $i < scalar @data; $i++)
	{
		#if ( (! exists $data[$i]) || ($data[$i] eq "") ) { $data[$i] = "NA"; }

		# Trim leading and trailing spaces
		$data[$i] =~ s/^\s+//;
		$data[$i] =~ s/\s+$//;
	}

	# Remove trailing NAs and replace internal blanks with "Unassigned"
	my $assigned=0;
	for (my $i = 7; $i >= 0; $i--)
	{
		if ( (exists $data[$i]) && ($i == $#data) && ( ($data[$i] eq "NA") || (! $data[$i]) ) ) {pop @data;}
		if ( (! $assigned) && (exists $data[$i]) ) { $assigned = 1; }
		if ( (! exists $data[$i]) && ($assigned == 1) ) { $data[$i] = "Unassigned";}
	}
	
	my $outString =join (";", @data);
	bless \@data, $class;
}

# Return the object as a ";" delimited string
sub taxstring
{
	my $self = shift;
	my $newString = join(";", @$self);
	$newString =~ s/;$//;
	return join(";", @$self);
}


# Return the domain of an object
sub domain 
{
	my $self = shift;
	return $$self[0];
}

# Return the phylum of an object
sub phylum
{
	my $self = shift;
	return $$self[1];
}

# Return the class of an object
sub class
{
	my $self = shift;
	return $$self[2];
}

# Return the order of an object
sub order
{
	my $self = shift;
	return $$self[3];
}

# Return the family of an object
sub family
{
	my $self = shift;
	return $$self[4];
}

# Return the genus of an object
sub genus
{
	my $self = shift;
	return $$self[5];
}

# Return the species of an object
sub species
{
	my $self = shift;
	return $$self[6];
}

# Return the strain of an object
sub strain
{
	my $self = shift;
	return $$self[7];
}

# Return the depth of an object - last rank with valid taxonomy
sub depth
{
	my $self = shift;
	my $depth = "NA";
	my @ranks = ("domain", "phylum", "class", "orderx", "family", "genus", "species", "strain" );
	#for (my $i=0; $i<=7; $i++)
	for (my $i=0; $i < scalar @{$self}; $i++)
	{
		if ( (exists $$self[$i]) && ($$self[$i] ne "NA") && ($$self[$i] ne "") && ($$self[$i] ne "Unassigned") ) 
		{$depth = $ranks[$i];}
	}
	return $depth;
}

# For an array of tax objects and a majority required, calculate a consensus taxonomy
# Return the consensus tax object, as well as stats on the agreement
sub consensus
{
	my $self = shift;
	my $majority = pop;

	# Correct for percentages 1-100
	if ($majority <= 1) {$majority = $majority * 100;}

	# Set up variables to store the results
	my @newTax; # consensus taxon
	my @rankCounts; # number of different taxa for each rank
	my @maxPcts; # percentage of most popular taxon for each rank
	my @naPcts; # percentage of each rank that has no taxonomy assigned
	my $conVote=0;
	my $taxCount = scalar @_;
	my $minRankIndex = -1;
	my $minRank = "NA";
	my @ranks = ("domain", "phylum", "class", "orderx", "family", "genus", "species", "strain" );

	# 
	# Calculate the Consensus
	#
	
	# Flesh out the taxonomies so they all have indices to 7
	foreach my $t (@_) 
	{
		for (my $i = 0; $i <= 7; $i++)
		{
			# If no value for that depth, add it
			if ( (scalar @{$t}) - 1 < $i ) { $$t[$i] = "NA";}
		}
	}

	my $done = 0;
	# For each taxonomic rank
	for (my $i = 0; $i <= 7; $i++)
	{
		# Initializes hashes with the counts of each tax assignment
		my %tallies; # for each tax value -- how many objects have this taxonomy
		my $rankCnt=0; # How many different taxa values are there for that rank
		my $maxCnt=0; # what was the size of the most common taxon
		my $naCnt=0; # how many are unassigned 
		my $topPct=0; # used to determine if we are done with the taxonomy or not

		# Step through the taxonomies and count them
		foreach my $t (@_) { $tallies{$$t[$i]}++;}

		# For each unique tax assignment
		foreach my $k (keys %tallies)
		{
			if ($k ne "NA") 
			{
				$rankCnt++;
				$minRankIndex = $i; 
				if ($tallies{$k} > $maxCnt) {$maxCnt = $tallies{$k};}
			} else {
				$naCnt = $tallies{$k};
			}

			my $vote = ($tallies{$k} / $taxCount) * 100 ;
			if (($k ne "NA") && ($vote > $topPct)) {$topPct = $vote;}
			#my $vote = int(100 * ($tallies{$k} / $taxCount) + 0.5) ;
			if ( (! $done) && ( $vote >= $majority) )
			{ 	
				push (@newTax, $k); 
				if ($k ne "NA") {$conVote = $vote;} 
			}
		}
		if ($topPct < $majority) {$done = 1;}

		#if ($#newTax < $i) {push (@newTax, "NA");}
		push(@rankCounts, $rankCnt);
		if($taxCount > 0){
		    push(@maxPcts, int(100 * ($maxCnt / $taxCount) + 0.5));
		    push(@naPcts, int(100 * ($naCnt / $taxCount) + 0.5));
		}
	}
	my @taxReturn;
    if (scalar @newTax == 0) 
    {
        push(@taxReturn, Taxonomy->new("Unknown")); #If no consensus at all, call it Unknown
    } else {
	    push(@taxReturn, Taxonomy->new(join ";", @newTax)); # taxonomy object for consensus
    }
	#if (! $taxReturn[0]) {$taxReturn[0] = "NA";}
	if (! $taxReturn[0]) {$taxReturn[0] = "Unknown";} # 20081126 - empty tax should be 'Unknown'
    if ($taxReturn[-1] eq "Unassigned") {pop @taxReturn;} # If resolved to an Unassigned rank, remove it.
	$taxReturn[1] = int($conVote + 0.5); # winning majority
	if ($minRankIndex >= 0) { $minRank = $ranks[$minRankIndex]; }

	$taxReturn[2] = $minRank; # lowest rank with valid assignment
	$taxReturn[3] = join(";", @rankCounts); # number of different taxa at each rank
	$taxReturn[4] = join(";", @maxPcts); # percentage of the most popular taxon (!= "NA")
	$taxReturn[5] = join(";", @naPcts); # percentage that are unassigned ("NA")
	return @taxReturn;
}
		
