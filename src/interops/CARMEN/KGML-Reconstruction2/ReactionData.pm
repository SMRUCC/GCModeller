package ReactionData;

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

ReactionData.pm

=head1 DESCRIPTION

Extends the SBML de novo model with additional cofactors.

=head2 Available methods

=over

=cut

1;

use strict;
use warnings;
use XML::DOM;

use CarmenConfig qw(COFACTOR_COLOR MOLECULE_HEIGHT MOLECULE_WIDTH DEVIATION);


=item addCofactors($reaction_ref,$molecule_ref,$hash_reactionId_data,$hash_compoundId_species,$max_alias,$max_species,$hash_compoundId_name)

Updates the molecule and reaction data with cofactor information.

RETURNS: $reaction_ref,$molecule_ref

=cut

sub addCofactors{

    my ($reaction_ref,$molecule_ref,$hash_reactionId_data,$hash_compoundId_species,$max_alias,$max_species,$hash_compoundId_name,$shortcuts,$abbr) = @_;
    
    # Passes througth all reactions of the Celldesigner SBML-file
    foreach my $reaction (keys(%{$reaction_ref})){

        my $rn = $reaction_ref->{$reaction}->{'reactions'};
        if ($rn && $rn=~ /rn.+/){
            my @name_array = split(" ",$rn);
            my ($kegg_rn) = ($name_array[0] =~/rn:(R\d+)/);

            if ((defined($hash_reactionId_data->{$kegg_rn}->{'substrates'})) && defined ($hash_reactionId_data->{$kegg_rn}->{'products'})){

                # Gets all products and substrats of the kegg reaction.lst of a specific reaction
                my %kegg_substrats = %{$hash_reactionId_data->{$kegg_rn}->{'substrates'}};
                my %kegg_products = %{$hash_reactionId_data->{$kegg_rn}->{'products'}};
    
                # Gets all products and substrats of the SBML-file
                my %drawn_substrats = %{$reaction_ref->{$reaction}->{'aliasReac'}};
                my %drawn_products = %{$reaction_ref->{$reaction}->{'aliasProd'}};
    
                my @drawn_substrats = (keys(%drawn_substrats));
                my @drawn_products = (keys(%drawn_products));
    
                # Get positions
                my $subst_x = $molecule_ref->{$drawn_substrats[0]}->{'x'};
                my $prod_x = $molecule_ref->{$drawn_products[0]}->{'x'};
    
                my $subst_y = $molecule_ref->{$drawn_substrats[0]}->{'y'};
                my $prod_y = $molecule_ref->{$drawn_products[0]}->{'y'};
    
                my $x_sub;
                my $y_sub;
                my $x_prod;
                my $y_prod;
    
                # Compare the x coordinates of substrat and product, searching for vertical connections
                if (abs($subst_x-$prod_x) < DEVIATION){
                    $x_sub = $subst_x - 4*DEVIATION;
                    $x_prod = $prod_x - 4*DEVIATION;
    
                    if($subst_y-$prod_y < 0){
                        $y_sub = $subst_y + ((abs($subst_y-$prod_y))/7);
                        $y_prod = $prod_y - ((abs($subst_y-$prod_y))/7);
                    }
                    elsif ($subst_y-$prod_y > 0){
                        $y_sub = $subst_y - ((abs($subst_y-$prod_y))/7);
                        $y_prod = $prod_y + ((abs($subst_y-$prod_y))/7);
                    }
    
                } 
                # Compare the y coordinates of substrat and product, searching for horizontal connections
                elsif (abs($subst_y-$prod_y) < DEVIATION){
                    $y_sub = $subst_y + (DEVIATION/2);
                    $y_prod = $prod_y + (DEVIATION/2);
    
                    if($subst_x-$prod_x < 0){
                        $x_sub = $subst_x + ((abs($subst_x-$prod_x))/7);
                        $x_prod = $prod_x - ((abs($subst_x-$prod_x))/7);
                    }
                    elsif ($subst_x-$prod_x > 0){
                        $x_sub = $subst_x - ((abs($subst_x-$prod_x))/7);
                        $x_prod = $prod_x + ((abs($subst_x-$prod_x))/7);
                    }
    
                }
                else{
                    $x_prod = ($subst_x + $prod_x)/2 + 2*DEVIATION;
                    $x_sub = ($subst_x + $prod_x)/2 - 2*DEVIATION;
                    $y_sub = ($subst_y + $prod_y)/2 - 2*DEVIATION;
                    $y_prod = ($subst_y + $prod_y)/2 + 2*DEVIATION;
                }
    
                my %stoich;
                # Passes through all known substrats
                foreach my $substrat_alias (keys(%drawn_substrats)){

                    # Gets the compound_id of foreach substrat
                    my $sub_comp = $molecule_ref->{$substrat_alias}->{'identis'};

                    # Delete the known substrat of the kegg reaction.lst entry of this reaction
                    # Save the associated stoichiometric value
                    if ($kegg_substrats{$sub_comp}){
                        $stoich{$substrat_alias} = $kegg_substrats{$sub_comp};
                        delete $kegg_substrats{$sub_comp};
                    }
                }
                my %aliasReacLink;
                my $count=0;
                foreach my $kegg_sub (%kegg_substrats){
                    # new alias! maybe old species
                    # Get a speciesId and a new alaisId foreach detected new molecule
                    if ($kegg_substrats{$kegg_sub}){
                        $count++;
                        my $species;
                        # If this molecule is already existent
                        if ($hash_compoundId_species->{"cpd".$kegg_sub}){
                            $species = $hash_compoundId_species->{"cpd".$kegg_sub};
                        }
                        # If is a complete new molecule 
                        else{
                            $max_species++;
                            $species = $max_species;
                            $hash_compoundId_species->{"cpd".$kegg_sub} = $species;
                        }
                        # New species_alias
                        $max_alias++;
                        # Save the new data in a hash 
                        $aliasReacLink{"sa".$max_alias} = "s".$species;
    
                        # Save the stoichiometrie 
                        $stoich{"sa".$max_alias} = $hash_reactionId_data->{$kegg_rn}->{'substrates'}->{$kegg_sub};
    
                        my $name;
                        my $altname;
    
                        if($abbr eq "1"){
                            $name = $kegg_sub;
                            $altname = $kegg_sub;
                        }
                        elsif($abbr eq "3"){
                            ($name,$altname) = GetSpeciesData::getCompoundName3($kegg_sub,$hash_compoundId_name,$shortcuts);
                        }
                        else{
                            ($name,$altname) = GetSpeciesData::getCompoundName2($kegg_sub,$hash_compoundId_name); 
                        }
    
                        # Create a new hash-entry for a simple molecule in $molecule_ref
                        $molecule_ref->{"sa".$max_alias} = {    'alias' => "sa$max_alias",
                                                                'species' => "s".$species,
                                                                'x'=> $x_sub+($count* DEVIATION),
                                                                'y'=> $y_sub+($count* DEVIATION),
                                                                'w'=> MOLECULE_WIDTH,
                                                                'h'=> MOLECULE_HEIGHT,
                                                                'name' => $name,
                                                                'altnames' => $altname,
                                                                'identis' => $kegg_sub,
                                                                'color' => COFACTOR_COLOR,
                                                                'type' => "SIMPLE_MOLECULE",
                                                                'pr' => undef,
                                                                're' => undef,
                                                                'kegg_rn' => undef,
                                                                'kegg_id' => undef,
                                                                'kegg_map' => undef
                                                    }; 
                    }
                }
    
                # Passes through all known substrats
                foreach my $product_alias (keys(%drawn_products)){
    
                    # Gets the compound_id of foreach substrat
                    my $prod_comp = $molecule_ref->{$product_alias}->{'identis'};
    
                    # Delete the known substrat of the kegg reaction.lst entry of this reaction
                    # Save the associated stoichiometric value
                    if ($kegg_products{$prod_comp}){
                        $stoich{$product_alias} = $kegg_products{$prod_comp};
                        delete $kegg_products{$prod_comp};
                    }
                }
                my %aliasProdLink;
                my $count_prod=0;
                foreach my $kegg_prod (%kegg_products){
                    # new alias! maybe old species
                    # Get a speciesId and a new alaisId foreach detected new molecule
                    if ($kegg_products{$kegg_prod}){
                        $count_prod++;
                        my $species;
                        # If this molecule is already existent
                        if ($hash_compoundId_species->{"cpd".$kegg_prod}){
                            $species = $hash_compoundId_species->{"cpd".$kegg_prod};
                        }
                        # If is a complete new molecule 
                        else{
                            $max_species++;
                            $species = $max_species;
                            $hash_compoundId_species->{"cpd".$kegg_prod} = $species;
                        }
                        # New species_alias
                        $max_alias++;
                        # Save the new data in a hash 
                        $aliasProdLink{"sa".$max_alias} = "s".$species;
    
                        # Save the stoichiometrie 
                        $stoich{"sa".$max_alias} = $hash_reactionId_data->{$kegg_rn}->{'products'}->{$kegg_prod};
    
    
                        my $name;
                        my $altname;
    
                        if($abbr eq "1"){
                            $name = $kegg_prod;
                            $altname = $kegg_prod
                        }
                        elsif($abbr eq "3"){
                            ($name,$altname) = GetSpeciesData::getCompoundName3($kegg_prod,$hash_compoundId_name,$shortcuts);
                        }
                        else{
                            ($name,$altname) = GetSpeciesData::getCompoundName2($kegg_prod,$hash_compoundId_name); 
                        }
    
                        # Create a new hash-entry for a simple molecule in $molecule_ref
                        $molecule_ref->{"sa".$max_alias} = {    'alias' => "sa$max_alias",
                                                                'species' => "s".$species,
                                                                'x'=> $x_prod+($count* DEVIATION),
                                                                'y'=> $y_prod+($count* DEVIATION),
                                                                'w'=> MOLECULE_WIDTH,
                                                                'h'=> MOLECULE_HEIGHT,
                                                                'name' => $name,
                                                                'altnames' => $altname,
                                                                'identis' => $kegg_prod,
                                                                'color' => COFACTOR_COLOR,
                                                                'type' => "SIMPLE_MOLECULE",
                                                                'pr' => undef,
                                                                're' => undef,
                                                                'kegg_rn' => undef,
                                                                'kegg_id' => undef,
                                                                'kegg_map' => undef
                                                    }; 
                    }
                }
                $reaction_ref->{$reaction}->{'aliasProdLink'} = \%aliasProdLink;
                $reaction_ref->{$reaction}->{'aliasReacLink'} = \%aliasReacLink;
                $reaction_ref->{$reaction}->{'stoichiometry'} = \%stoich;
            }
        }
    }
    return ($reaction_ref,$molecule_ref);
}

=item joinReactionConnections($reaction_ref,$molecule_ref)

Method to create the relationship of one protein to more than one reactions

RETURNS: $reaction_ref,$molecule_ref

=cut

sub joinReactionConnections{

    my ($reaction_ref,$molecule_ref)= @_;

    my %hash_enzyme;
    my %hash_data;

    # Select number of same genes
    foreach my $molecule (keys(%{$molecule_ref})){
        if ($molecule_ref->{$molecule}->{'type'} eq "PROTEIN"){

            my $species = $molecule_ref->{$molecule}->{'species'};

            if (defined ($hash_enzyme{$species})) {
                push (@{$hash_enzyme{$species}}, $molecule);
            } 
            else {
                   $hash_enzyme{$species} = [$molecule];
            }
        }
    }
    # Join one ec number to more than one reaction, if the ec number symboles are close located
    foreach my $species (keys(%hash_enzyme)){

        my @aliases = @{$hash_enzyme{$species}};
        my $len = scalar(@aliases);

        for (my $m=0; $m<$len; $m++){
            for (my $n=0; $n<$len; $n++){

                my $alias1 = $aliases[$m]; 
                my $alias2 = $aliases[$n];

                if($alias1 ne $alias2 && $molecule_ref->{$alias1} && $molecule_ref->{$alias2} && $reaction_ref->{$molecule_ref->{$alias1}->{'re'}}){

                    my $reaction1 = $molecule_ref->{$alias1}->{'re'};
                    my $x_1 = $molecule_ref->{$alias1}->{'x'};
                    my $x_2 = $molecule_ref->{$alias2}->{'x'};
                    my $x_dif = abs($x_1-$x_2);

                    my $reaction2 = $molecule_ref->{$alias2}->{'re'};
                    my $y_1 = $molecule_ref->{$alias1}->{'y'};
                    my $y_2 = $molecule_ref->{$alias2}->{'y'};
                    my $y_dif = abs($y_1-$y_2);

                    if($x_dif < 300 && $y_dif < 300){

                        my $new_x = ($x_1+$x_2)/2;
                        my $new_y = ($y_1+$y_2)/2;

                        $molecule_ref->{$alias1}->{'x'} = $new_x;
                        $molecule_ref->{$alias1}->{'y'} = $new_y;

                        my $rn1 = $reaction_ref->{$reaction1}->{'reactions'};
                        my $rn2 = $reaction_ref->{$reaction2}->{'reactions'};
                        my $rn = $rn1." ".$rn2;

                        $molecule_ref->{$alias1}->{'kegg_rn'} = $rn;

                        my @modis = @{$reaction_ref->{$reaction2}->{'mod_alias'}};
                        for (my $i=0; $i<=$#modis;$i++){ 
                            if ($modis[$i] eq $alias2){
                                $modis[$i] = $alias1;
                            }
                        }
                        $reaction_ref->{$reaction2}->{'mod_alias'} = \@modis;

                        delete($molecule_ref->{$alias2});
                    }
                }
            }
        }
    }
    return ($reaction_ref,$molecule_ref);
}

=item add_modifier($reaction_ref,$molecule_ref,$hash_ec_modis)

Method to add modifiers to a spezific reaction

RETURNS: $reaction_ref,$molecule_ref

=cut

sub add_modifier{

    my ($reaction_ref,$molecule_ref,$hash_ec_modis)= @_;

    foreach my $ec (keys(%{$hash_ec_modis})){

        my @modis =  @{$hash_ec_modis->{$ec}};

        foreach my $mod_alias (@modis){
            my $re =  $molecule_ref->{$mod_alias}->{'re'};
            my $mod_species =  $molecule_ref->{$mod_alias}->{'species'};

            push(@{$reaction_ref->{$re}->{'mod_alias'}},$mod_alias);
            push(@{$reaction_ref->{$re}->{'mod_id'}},$mod_species);
        }
    }
    return ($reaction_ref,$molecule_ref);
}

=back 
