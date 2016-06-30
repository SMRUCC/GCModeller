package KEGG_Information;

#
#  Copyright (C) 2010 CeBiTec, Bielefeld University
#
#  This library is free software; you can redistribute it and/or
#  modify it under the terms of the GNU General Public License
#  version 2 as published by the Free Software Foundation.
#
#  This file is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
#  General Public License for more details.
#
#  You should have received a copy of the GNU General Public License
#  along with this file; see the file LICENSE.  If not, write to
#  the Free Software Foundation, Inc., 59 Temple Place - Suite 330,
#  Boston, MA 02111-1307, USA.
#

=head1 NAME

KEGG_Information.pm

=head1 DESCRIPTION

Gets different kinds of Kegg-information

=head2 Available methods

=over

=cut

use strict;
use warnings;
use XML::DOM;

1;

=item getCompoundNames($nameFile)

Extracts all compound ids and its associated names

RETURNS: %hash_compoundId_name

=cut

sub getCompoundNames{

    my ($nameFile) = @_;

    my $status = 'search_compound';
    my $compId;
    my @names;
    my %hash_compoundId_name;

    open(DATA, $nameFile) or die "Cannot read file $nameFile";

    while(my $line = <DATA>) {
        if ($status eq 'search_compound'){
                if(($compId) = $line =~ /^ENTRY\s+([C|G][0-9]{5})\s+Compound\s*$/){
                $status = 'search_name';
                undef(@names);
                }
        }
        elsif ($status eq 'search_name'){
            if (my ($name) = $line =~ /^NAME\s+(.+)$/){
                push(@names,$name);
                $status = 'next_name';
            }
        }
        elsif ($status eq 'next_name'){
            if (!($line =~ /^FORMULA/) && (!($line =~ /^REACTION/))){
                if (my ($nextname) = $line =~ /^\s+(.+)$/){
                    push(@names, $nextname);
                    $status = 'next_name';
                }
            }
            else {
                my @newarr = @names;
                $hash_compoundId_name{$compId}=\@newarr;
                $status = 'search_compound';
            }
        }
    }
    return \%hash_compoundId_name;
}

=back

=item getAssociatedCompounds($reactionFile)

Gets the reactionId and associated compounds
Based on the package DB_Reactions.pm of the prototype.

RETURNS: $hash_reactionId_compoundId

=cut

sub getAssociatedCompounds {

	my ($reactionFile) = @_;
	my $hash_reactionId_compoundId = {};
	open(DATA, $reactionFile) or die "Cannot read file $reactionFile";

	while(my $line = <DATA>) {


		if($line =~ /^(R[0-9]{5}):(.+)<=>(.+)$/) {
			my @subs = split(/\s+\+\s+/, $2);
			my @prod = split(/\s+\+\s+/, $3);
			my $subs_href = {};
			my $prod_href = {};
			my $problem = 0;

                        # Get all substrats of a reaction, without water and oxygen
			foreach my $chunk (@subs) {
				if($chunk =~ /^\s*([0-9]*)\(?[nm]?[\+\-]?[0-9]*\)?\s*([CG][0-9]{5}).*$/) {
					if($2 ne "C00001" && $2 ne "C00007") {
						$subs_href->{$2} = $1 eq ""?1:int($1);
					}
				} else {
					print "Problem parsing: $chunk\n";
					$problem = 1;
				}
			}
                        # Get all products of a reaction, without water and oxygen
			foreach my $chunk (@prod) {
				if($chunk =~ /^\s*([0-9]*)\(?[nm]?[\+\-]?[0-9]*\)?\s*([CG][0-9]{5}).*$/) {
					if($2 ne "C00001" && $2 ne "C00007") {
						$prod_href->{$2} = $1 eq ""?1:int($1);
					}
				} else {
					print "Problem parsing: $chunk\n";
					$problem = 1;
				}
			}
                        # Create a hash with the reaction id as key and substrat, product and reaction as values
                        # substrat contains a hash with compoundids and its number in the reaction 
                        # product contains a hash with compoundids and its number in the reaction 
                        # reaction refers to the reaction equation 
			if(!$problem) {
				$hash_reactionId_compoundId->{$1} = {
						'substrates' => $subs_href,
						'products' => $prod_href,
						'reaction' => $line,
				             };
			}
		} else {
			print "Problem parsing: $line\n";	
		}
	}
	close(DATA);
	return $hash_reactionId_compoundId;
}