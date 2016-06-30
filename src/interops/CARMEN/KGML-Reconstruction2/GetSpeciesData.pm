package GetSpeciesData;

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

GetSpeciesData.pm

=head1 DESCRIPTION

Joins data of the relation, reaction and entry tags of the KEGG-maps and transfers them into Celldesigner-compatible structures for symbols and reactions.

=head2 Available methods

=over

=cut

use strict;
use warnings;
use XML::DOM;
use Data::Dumper;

use CarmenConfig qw(TOTAL_HEIGHT TOTAL_WIDTH DEVIATION SCALE MOLECULE_HEIGHT MOLECULE_WIDTH MOLECULE_COLOR ENZYME_HEIGHT ENZYME_WIDTH ENZYME_COLOR MISSING_ENZYME_COLOR MAP_COLOR);

1;

=item createMolecule($molecule_ref,$entry_data_ref,$hash_compoundId_name,$heigth,$width,$max_alias,$max_species,$hash_keggId_species,$pr,$re,$hash_keggId_aliasId,$map,$hash_compoundId_species,$hash_annotEC_genes)

Joins data of the relation, reaction and entry tags of the KEGG-maps and creates a hash containing information of simple molecules, enzymes and maplinks.

RETURNS:($molecule_ref,$entry_data_ref,$hash_compoundId_name,$heigth,$width,$max_alias,$max_species,$hash_keggId_species,$pr,$re,$hash_keggId_aliasId,$map,$hash_compoundId_species,$hash_speciesId_speciesAlias)

=cut

sub createMolecule {

    my ($molecule_ref,$entry_data_ref,$hash_compoundId_name,$heigth,$width,$max_alias,$max_species,$hash_keggId_species,$pr,$re,$hash_keggId_aliasId,$map,$hash_compoundId_species,$hash_annotEC_genes,$shortcuts,$hash_genes_altname,$hash_name_proteinID,$hash_name_alias,$abbr, $ml) = @_;

    # Save the speciesId and all associated aliasIds
    my $hash_speciesId_speciesAlias;
    # Save the kegg id and the associated species
    my $hash_keggId_speciesId;

    foreach my $entry (sort(keys(%{$entry_data_ref}))){
		next if (($entry_data_ref->{$entry}->{'entry_type'}) eq "map" && $ml eq "F");
        my $species;
        my $name;
        my $cell_type;
        my $cell_color;
        my $cell_w;
        my $cell_h;
        my $cell_x;
        my $cell_y;
        my $identis = "";
        my $altnames = "";
        my $entry_name;
        my $protein_id = "";
        my $reaction_id = "";
        my $kegg_rn = "";
        my $altshort = "";
        my $multi_protein = "no";
        my $check = 0;
        my $gene_name;

        $max_alias++;

        # Get tags associated with compounds and it's ids
        if (($entry_data_ref->{$entry}->{'entry_type'}) eq "compound"){

            $entry_name = $entry_data_ref->{$entry}->{'entry_name'};
            if ($entry_name =~ /cpd:[C|G]\d+/){
                ($identis) = ($entry_name =~ /cpd:([C|G]\d+)/);

                if($abbr eq "1"){
                    $name = $identis;
                    $altnames = $identis;
                }
                elsif($abbr eq "3"){
                    ($name,$altnames) = getCompoundName3($identis,$hash_compoundId_name,$shortcuts);
                }
                else{
                    ($name,$altnames) = getCompoundName2($identis,$hash_compoundId_name); 
                }
                $cell_type = "SIMPLE_MOLECULE";
            }
            elsif($entry_name =~ /glycan:G\d+/){
                ($identis) = ($entry_name =~ /glycan:(G\d+)/);
                $name = $identis;
                $altnames = "Glycan";
                $cell_type = "GLYCAN";
            }
            $cell_x = ($entry_data_ref->{$entry}->{'graphics_x'})*SCALE;
            $cell_y = ($entry_data_ref->{$entry}->{'graphics_y'})*SCALE;
            $cell_color = MOLECULE_COLOR;
            $cell_h = MOLECULE_HEIGHT;
            $cell_w = MOLECULE_WIDTH;
        }
        # Get tags associated with enyzmes and ec-numbers
        elsif (($entry_data_ref->{$entry}->{'entry_type'}) eq "enzyme"){

            $entry_name = $entry_data_ref->{$entry}->{'entry_name'};
            ($identis) = ($entry_name =~ /ec:(.+)/);

            $pr++;
            $re++;

            $protein_id = "pr".$pr;
            $reaction_id = "re".$re;
            if($entry_data_ref->{$entry}->{'entry_reaction'}){
                $kegg_rn = $entry_data_ref->{$entry}->{'entry_reaction'};
            }
            else{
                $kegg_rn = undef;
            }
            $name = $identis;

            $cell_x = ($entry_data_ref->{$entry}->{'graphics_x'})*SCALE;
            $cell_y = ($entry_data_ref->{$entry}->{'graphics_y'})*SCALE;
            $cell_h = ENZYME_HEIGHT;
            $cell_w = ENZYME_WIDTH;

            $cell_type = "PROTEIN";

            if ($hash_annotEC_genes->{$identis}){
                $altnames = join(",",@{$hash_annotEC_genes->{$identis}})."\n";

                my $gene_number = scalar(@{$hash_annotEC_genes->{$identis}});
                my @gene_names = @{$hash_annotEC_genes->{$identis}};
                $cell_color = ENZYME_COLOR;

                if ($gene_number eq "1"){
                   # my @gene_names = @{$hash_annotEC_genes->{$identis}};
                    $gene_name = $gene_names[0];
                    $hash_name_proteinID->{$gene_name} = $pr;
                    $check = 1;

                    if (defined($hash_genes_altname->{$gene_name})){
                        $altshort = $hash_genes_altname->{$gene_name};
                    }
                    else{
                        ($altshort) = ($gene_name =~ /.+_(\d+)/);
                    }
                }
                else{
                    $multi_protein = "yes";
                }
            }
            else{
                $altnames = "No annotated genes available!\n";
                $cell_color = MISSING_ENZYME_COLOR;
            }
        }
        # Get tags associated with maplink and ec-numbers
        elsif (($entry_data_ref->{$entry}->{'entry_type'}) eq "map"){

            $entry_name = $entry_data_ref->{$entry}->{'entry_name'};
            ($identis) = ($entry_name =~ /path:(.+)/);

            $cell_y =  ((($entry_data_ref->{$entry}->{'graphics_y'})-(($entry_data_ref->{$entry}->{'graphics_h'})/2))*SCALE);

            $cell_x = ((($entry_data_ref->{$entry}->{'graphics_x'})-(($entry_data_ref->{$entry}->{'graphics_w'})/2))*SCALE);
            $cell_type = "MAP";
            $cell_color = MAP_COLOR;
            $name = $entry_data_ref->{$entry}->{'graphics_name'};

            $cell_h = $entry_data_ref->{$entry}->{'graphics_h'};
            $cell_w = $entry_data_ref->{$entry}->{'graphics_w'};
        }
        elsif(($entry_data_ref->{$entry}->{'entry_type'}) eq "ortholog"){

            $entry_name = $entry_data_ref->{$entry}->{'entry_name'};

            if ($entry_name =~/\w+:.+/){
                ($identis) = ($entry_name =~ /\w+:(.+)/);
            }
            else{
                print STDERR "No ortholog name found $entry\n"
            }

            $pr++;
            $re++;

            $protein_id = "pr".$pr;
            $reaction_id = "re".$re;

            if($entry_data_ref->{$entry}->{'entry_reaction'}){
                $kegg_rn = $entry_data_ref->{$entry}->{'entry_reaction'};
            }
            else{
                $kegg_rn = undef;
            }
            $name = $identis;

            $cell_x = ($entry_data_ref->{$entry}->{'graphics_x'})*SCALE;
            $cell_y = ($entry_data_ref->{$entry}->{'graphics_y'})*SCALE;
            $cell_h = ENZYME_HEIGHT;
            $cell_w = ENZYME_WIDTH;

            $cell_type = "PROTEIN";
            $altnames = "No annotated genes available!\n";
            $cell_color = MISSING_ENZYME_COLOR;
         }
        elsif(($entry_data_ref->{$entry}->{'entry_type'}) eq "other"){

            $entry_name = $entry_data_ref->{$entry}->{'entry_name'};
            if ($entry_name =~ /up:.+/){
                ($identis) = ($entry_name =~ /up:(.+)/);
            }
            elsif($entry_name =~ /tr:.+/){
                ($identis) = ($entry_name =~ /tr:(.+)/);
            }
            elsif ($entry_name =~//){
                $identis = "Hidden";
            }
            $pr++;
            $re++;

            $protein_id = "pr".$pr;
            $reaction_id = "re".$re;
            if($entry_data_ref->{$entry}->{'entry_reaction'}){
                $kegg_rn = $entry_data_ref->{$entry}->{'entry_reaction'};
            }
            else{
                $kegg_rn = undef;
            }
            $name = $identis;

            $cell_x = ($entry_data_ref->{$entry}->{'graphics_x'})*SCALE;
            $cell_y = ($entry_data_ref->{$entry}->{'graphics_y'})*SCALE;
            $cell_h = ENZYME_HEIGHT;
            $cell_w = ENZYME_WIDTH;

            $cell_type = "PROTEIN";
            $altnames = "No annotated genes available!\n";
            $cell_color = MISSING_ENZYME_COLOR;
         }
        else{
            print STDERR "No compound, enzyme, ortholog or map found in Kegg_map $map.\n";
            print STDERR "Type:".$entry_data_ref->{$entry}->{'entry_type'}."\n";
            next;
        }
        # Check if the species already exists
        if ($hash_keggId_speciesId->{$identis}){
            $species = $hash_keggId_speciesId->{$identis};
        }
        else{ 
            $max_species++;
            $species = $max_species;
            $hash_keggId_speciesId->{$identis} = $species;
        }

        my $id = $entry_data_ref->{$entry}->{'entry_id'};
        $hash_keggId_aliasId->{$id} = "sa".$max_alias;

        if ($cell_type eq "SIMPLE_MOLECULE"){
           $hash_compoundId_species->{"cpd:".$identis} = $species;
           $kegg_rn = "undef";
        }
        if ($cell_type eq "GLYCAN"){
           $hash_compoundId_species->{"gl:".$identis} = $species;
           $kegg_rn = "undef";
        }

        # Get all species ids associated with one alias id (s to sa)
        if (defined ($hash_speciesId_speciesAlias->{$species})){
                push @{$hash_speciesId_speciesAlias->{$species}}, "sa".$max_alias;
        }
        else{
            $hash_speciesId_speciesAlias->{$species} = ["sa".$max_alias];
        }

        if ($check == 1){
            $hash_name_alias->{$gene_name} = $species;
        }

        # Save all information of a amolecule (simple molecules, proteins and maps)
        $molecule_ref->{"sa".$max_alias} = {    'alias' => "sa$max_alias",
                                                'species' => "s".$species,
                                                'x'=> $cell_x + $width,
                                                'y'=> $cell_y + $heigth,
                                                'w'=> $cell_w,
                                                'h'=> $cell_h,
                                                'name' => $name,
                                                'altnames' => $altnames,
                                                'identis' => $identis,
                                                'color' => $cell_color,
                                                'type' => $cell_type,
                                                'pr' => $protein_id,
                                                're' => $reaction_id,
                                                'kegg_rn' => $kegg_rn,
                                                'kegg_id' => $entry_data_ref->{$entry}->{'entry_id'},
                                                'kegg_map' => $map,
                                                'altshort' => $altshort,
                                                'protein_stat' => $multi_protein
                                        };
        #store species and alias in entry_data_ref for later use:
        $entry_data_ref->{$entry}->{'species'} = "s".$species;
        $entry_data_ref->{$entry}->{'alias'} = "sa".$max_alias;
    }
    return ($molecule_ref,$entry_data_ref,$hash_compoundId_name,$heigth,$width,$max_alias,$max_species,$hash_keggId_species,$pr,$re,$hash_keggId_aliasId,$map,$hash_compoundId_species,$hash_speciesId_speciesAlias,$hash_name_proteinID,$hash_name_alias);
}

sub createProteins{

    my ($molecule_ref,$hash_genes_altname,$max_alias,$hash_speciesId_speciesAlias,$max_pr,$max_species,$hash_name_proteinID,$hash_keggId_speciesId,$hash_name_alias) = @_;

    my $altshort = "";
    my %hash_ec_modis;
 
    foreach my $molecule (sort(keys(%{$molecule_ref}))){

        if ($molecule_ref->{$molecule}->{'protein_stat'} eq "yes"){

            my $alias = $molecule_ref->{$molecule}->{'alias'};
            my $species = $molecule_ref->{$molecule}->{'species'};
            my $ec = $molecule_ref->{$molecule}->{'name'};
            my $x = $molecule_ref->{$molecule}->{'x'};
            my $y = $molecule_ref->{$molecule}->{'y'};
            my $w = $molecule_ref->{$molecule}->{'w'};
            my $h = $molecule_ref->{$molecule}->{'h'};
            my $color = $molecule_ref->{$molecule}->{'color'};
            my $type = $molecule_ref->{$molecule}->{'type'};
            my $pr =  $molecule_ref->{$molecule}->{'pr'};
            my $re = $molecule_ref->{$molecule}->{'re'};
            my $kegg_rn = $molecule_ref->{$molecule}->{'kegg_rn'};
            my $kegg_id = $molecule_ref->{$molecule}->{'kegg_id'};
            my $kegg_map = $molecule_ref->{$molecule}->{'kegg_map'};
            my $name = $molecule_ref->{$molecule}->{'name'}; 

            my @gene_names = split(",",$molecule_ref->{$molecule}->{'altnames'});

            foreach my $gene_name (@gene_names){

                if($gene_name=~/\w.+\d+\n/){
                    ($gene_name) = ($gene_name=~/(\w.+\d+)\n/);
                }
                if (defined($hash_genes_altname->{$gene_name})){
                        $altshort = $hash_genes_altname->{$gene_name};
                }
                else{
                    ($altshort) = ($gene_name =~ /.+_(\d+)/);
                }
                # Get all species ids associated with one alias id (s to sa)
                if (defined ($hash_ec_modis{$ec})){
                        push @{$hash_ec_modis{$ec}}, $alias;
                }
                else{
                    $hash_ec_modis{$ec} = [$alias];
                }
                # Save all information of a specific protein
                $molecule_ref->{$alias} = { 'alias' => $alias,
                                            'species' => $species,
                                            'x'=> $x,
                                            'y'=> $y,
                                            'w'=> $w,
                                            'h'=> $h,
                                            'name' => $name,
                                            'altnames' => $gene_name,
                                            'identis' => $ec,
                                            'color' => $color,
                                            'type' => $type,
                                            'pr' => $pr,
                                            're' =>  $re,
                                            'kegg_rn' => $kegg_rn,
                                            'kegg_id' => $kegg_id,
                                            'kegg_map' => $kegg_map,
                                            'altshort' => $altshort,
                                            'protein_stat' => "No"
                                        };

                if ($species =~ /s\d+/){
                    ($species) = ($species =~ /s(\d+)/);
                }

                $x = $x+5;
                $y = $y+5;

                if ($hash_name_proteinID->{$gene_name}){
                    $pr = "pr".$hash_name_proteinID->{$gene_name};
                    $species = "s".$hash_name_alias->{$gene_name};
                }
                else{
                    $max_pr++;
                    $pr = "pr".$max_pr;
                    $hash_name_proteinID->{$gene_name} = $max_pr;

                    $max_species++;
                    $species = $max_species;
                    $hash_name_alias->{$gene_name} = $species;
                    $species = "s".$max_species;
                }
                $max_alias++;
                $alias = "sa".$max_alias;
            }
        }
    }
    return ($molecule_ref,$max_alias,$hash_speciesId_speciesAlias,$max_pr,$max_species,\%hash_ec_modis, $hash_keggId_speciesId,$hash_name_alias);
}

=item getReactionData($entry_data_ref,$reaction_data_ref,$relation_data_ref,$molecule_ref,$hash_keggId_aliasId,$reaction_ref,$hash_compoundId_species,$hash_speciesId_speciesAlias,$hash_keggId_species)

Joins data of the relation, reaction and entry tags of the KEGG-maps and molecule information and creates a hash containing reaction information.

RETURNS: $reaction_ref

=cut

sub getReactionData{
	my ($entry_data_ref,$reaction_data_ref,$relation_data_ref,$molecule_ref,$hash_keggId_aliasId,$reaction_ref,$hash_compoundId_species,$hash_speciesId_speciesAlias,$hash_keggId_species, $ml) = @_;

	#create hash consisting of reaction name (rn:xxxx) and associated EC-Numbers
	foreach my $entry_id (sort(keys(%{$entry_data_ref}))) {
		next unless (($entry_data_ref->{$entry_id}->{'entry_type'}) eq "enzyme");
		next unless (defined($reaction_data_ref->{$entry_id}));
# 		$entry_data_ref->{$entry_id}->{'entry_reaction'};
		
		my $enzyme_alias = $entry_data_ref->{$entry_id}->{'alias'};
        my $reactions = $molecule_ref->{$enzyme_alias}->{'kegg_rn'}; 
        my $re = $molecule_ref->{$enzyme_alias}->{'re'};

		# Gets the reaction direction
		my $reversible;
		my $rev = $reaction_data_ref->{$entry_id}->{'reaction_type'};
		if ($rev eq "reversible"){
			$reversible = "true";
		}
		else{$reversible = "false"};

		#TODO: ec: im name ersetzen
		
		#get species and alias for substrates/products of reaction associated with current enzyme
 		my $reactions_hash = $reaction_data_ref->{$entry_id};
 		my @substrate_ids =  @{$reactions_hash->{'substrate_ids'}};
 		my %substrate_aliases;
 		foreach my $substrate_id (@substrate_ids) {
 			my $sub_alias = $entry_data_ref->{$substrate_id}->{'alias'};
 			my $sub_species = $entry_data_ref->{$substrate_id}->{'species'};
			$substrate_aliases{$sub_alias} = $sub_species;
 		}
 		my @product_ids =  @{$reactions_hash->{'product_ids'}};
 		my %product_aliases;
 		foreach my $product_id (@product_ids) {
 			my $prod_alias = $entry_data_ref->{$product_id}->{'alias'};
 			my $prod_species = $entry_data_ref->{$product_id}->{'species'};
			$product_aliases{$prod_alias} = $prod_species;
 		}
		
		#store information for this reaction:
		$reaction_ref->{$enzyme_alias} = {'reversible' => $reversible,
										  'reactions' => $reactions,
										  'name'=> $molecule_ref->{$enzyme_alias}->{'identis'},
										  'id' => $re,
										  'aliasReac' => \%substrate_aliases,
										  'aliasProd' => \%product_aliases,
										  'mod_id'=> [$molecule_ref->{$enzyme_alias}->{'species'}],
										  'mod_alias'=> [$enzyme_alias],
										  'mod_name' => [$molecule_ref->{$enzyme_alias}->{'identis'}]};

	}
	#if map-references should not be created stop here:
	return $reaction_ref if ($ml eq "F");
	
	#else create map-references:
    foreach my $relation (values(%{$relation_data_ref})){
    	next unless ($relation->{'relation_type'} eq "maplink" && $relation->{'subtype_name'} eq "compound");
        my $entry1 = $relation->{'relation_entry1'};
        my $alias1 = $hash_keggId_aliasId->{$entry1};
        my $entry2 = $relation->{'relation_entry2'};
        my $alias2 = $hash_keggId_aliasId->{$entry2};

		my $map_alias = undef;
		$map_alias = $alias1 if($molecule_ref->{$alias1}->{"type"} eq "MAP");
		$map_alias = $alias2 if($molecule_ref->{$alias2}->{"type"} eq "MAP");
		next unless defined($map_alias);
		
        my $compound = $relation->{'subtype_value'};
        my $calias = $hash_keggId_aliasId->{$compound};

        my $subst_alias_species->{$calias} = $molecule_ref->{$calias}->{'species'};
        my $produ_alias_species->{$map_alias} = $molecule_ref->{$map_alias}->{'species'};

		$reaction_ref->{"re".$calias.$map_alias} = {'reversible' => "true",
                                                        'id' => "re".$calias.$map_alias,
                                                        'aliasReac' => $subst_alias_species,
                                                        'aliasProd' => $produ_alias_species,
                                                        'reactions' => "map_connection",
                                                        'mod_id'=> undef,
                                                        'mod_alias'=> undef,
                                                        'mod_name' => undef};
    }
	return $reaction_ref;
}

#old getReactionData method, not used anymore
sub getReactionData_old{

    my ($entry_data_ref,$reaction_data_ref,$relation_data_ref,$molecule_ref,$hash_keggId_aliasId,$reaction_ref,$hash_compoundId_species,$hash_speciesId_speciesAlias,$hash_keggId_species) = @_;

    my $hash_aliasEC_aliasCompound;
    my $hash_aliasEC_aliasCompMap;
    my $ecId_mapId;

    # Fetchall information of a reaction_tag
    foreach my $relation (values(%{$relation_data_ref})){

        my $entry1 = $relation->{'relation_entry1'};
        my $alias1 = $hash_keggId_aliasId->{$entry1};
        my $entry2 = $relation->{'relation_entry2'};
        my $alias2 = $hash_keggId_aliasId->{$entry2};

        my $compound = $relation->{'subtype_value'};
        my $calias = $hash_keggId_aliasId->{$compound};

        # Get relation tag information of enzymes
        if ($relation->{'relation_type'} eq "ECrel" && $relation->{'subtype_name'} eq "compound"){

            if (defined($hash_aliasEC_aliasCompound->{$alias1})){
                $hash_aliasEC_aliasCompound->{$alias1}->{$calias} = 1;
            }
            else{
                $hash_aliasEC_aliasCompound->{$alias1} = {$calias => 1};
            }

            if (defined($hash_aliasEC_aliasCompound->{$alias2})){
                $hash_aliasEC_aliasCompound->{$alias2}->{$calias} = 1;
            }
            else{
                $hash_aliasEC_aliasCompound->{$alias2} = {$calias => 1};
            }
        }

        # Get relation tag information of maplink entries
        if ($relation->{'relation_type'} eq "maplink" && $relation->{'subtype_name'} eq "compound"){

            if (defined($hash_aliasEC_aliasCompMap->{$alias1})){
                $hash_aliasEC_aliasCompMap->{$alias1}->{$calias} = 1;
            }
            else{
                $hash_aliasEC_aliasCompMap->{$alias1} = {$calias => 1};
            }

            if (defined($hash_aliasEC_aliasCompMap->{$alias2})){
                $hash_aliasEC_aliasCompMap->{$alias2}->{$calias} = 1;
            }
            else{
                $hash_aliasEC_aliasCompMap->{$alias2} = {$calias => 1};
            }
        }
    }
    # Get all reactions with a map and a compound as reactant and product
    foreach my $ec_malias (keys(%{$hash_aliasEC_aliasCompMap})){

        my $type = $molecule_ref->{$ec_malias}->{'type'};

        if($type eq "MAP"){
  
            my $subst_save = {$ec_malias => $molecule_ref->{$ec_malias}->{'species'}};
            my $hash_aliasCompound = $hash_aliasEC_aliasCompMap->{$ec_malias};

            foreach my $comp_alias (keys(%{$hash_aliasCompound})){

                my %subst_alias_species = %{$subst_save };
                my %produ_alias_species;

                $produ_alias_species{$comp_alias} = $molecule_ref->{$comp_alias}->{'species'};

                $reaction_ref->{"re".$ec_malias.$comp_alias} = {'reversible' => "true",
                                                                'id' => "re".$ec_malias.$comp_alias,
                                                                'aliasReac' => \%subst_alias_species,
                                                                'aliasProd' => \%produ_alias_species,
                                                                'reactions' => "map_connection",
                                                                'mod_id'=> undef,
                                                                'mod_alias'=> undef,
                                                                'mod_name' => undef};
                }
        }
    }
    # Passes through all ec numbers to get associated reactions, substrats and products, as well as it's celldesigner alias- and species ids
    foreach my $ec_alias (keys(%{$hash_aliasEC_aliasCompound})){

        my $reactions = $molecule_ref->{$ec_alias}->{'kegg_rn'}; 
        my $re = $molecule_ref->{$ec_alias}->{'re'};

        if($reactions ne "undef"){
            my ($reaction) = ($reactions =~ /(rn:R\d+.*)/);

            if ((defined($reaction_data_ref->{$reaction}->{'substrates'})) && defined ($reaction_data_ref->{$reaction}->{'products'})){

                my @substrats =  @{$reaction_data_ref->{$reaction}->{'substrates'}};
                my $sub_length = scalar(@substrats);
                my @products =  @{$reaction_data_ref->{$reaction}->{'products'}};
                my $prod_length = scalar(@products);
                my $rev = $reaction_data_ref->{$reaction}->{'reaction_type'};


                # Gets the reaction direction
                my $reversible;
                if ($rev eq "reversible"){
                    $reversible = "true";
                }
                else{$reversible = "false"};

                my $hash_aliasCompound = $hash_aliasEC_aliasCompound->{$ec_alias}; 

                my %produ_alias_species;
                my %subst_alias_species;

                foreach my $alias (keys(%{$hash_aliasCompound})){
                    my $alias_compound = "cpd:".$molecule_ref->{$alias}->{'identis'};

                    foreach my $reactiontag_sub (@substrats){
                        if ($reactiontag_sub eq $alias_compound){
                            $subst_alias_species{$alias} = $molecule_ref->{$alias}->{'species'};
                        }
                    }
                    foreach my $reactiontag_prod (@products){
                        if ($reactiontag_prod eq $alias_compound){
                            $produ_alias_species{$alias} =  $molecule_ref->{$alias}->{'species'};
                        }
                    }
                }
                my $re = $molecule_ref->{$ec_alias}->{'re'};

                my $produ_hash_length = scalar(keys(%produ_alias_species));
                my $subst_hash_length = scalar(keys(%subst_alias_species));

                # Get further reactions of border products
                if(($produ_hash_length eq 0) || ($produ_hash_length < $prod_length)){
                    foreach my $reac_prod (@products){

                    #If unique, it gets the alias id of a border compound
                    my $alias_id;
                    if ($hash_compoundId_species->{$reac_prod}){
                            my $species = $hash_compoundId_species->{$reac_prod} if $hash_compoundId_species->{$reac_prod};


            my @aliases;
            if(exists($hash_speciesId_speciesAlias->{$species})){

                            @aliases = @{$hash_speciesId_speciesAlias->{$species}};}

                            my $length = scalar(@aliases);

                            if ($length == 1){
                                $alias_id = $aliases[0];

                                $produ_alias_species{$alias_id} = "s".$species;
                            }
                            else{
                            #    print STDERR "\tProblem with $reac_prod, the border compound is not unique.\n";
                            }
                        }
                    }
                }
                #Get further reactions of border substrats
                if($subst_hash_length < $sub_length || ($subst_hash_length eq 0)){
                    foreach my $reac_sub (@substrats){

                        #If unique, it gets the alias id of a border compound
                        my $alias_id;
                        if ($hash_compoundId_species->{$reac_sub}){
                            my $species = $hash_compoundId_species->{$reac_sub};

            my @aliases;
            if(exists($hash_speciesId_speciesAlias->{$species})){
                            @aliases = @{$hash_speciesId_speciesAlias->{$species}};
                }
                            my $length = scalar(@aliases);
                            if ($length == 1){
                                $alias_id = $aliases[0];
                                $subst_alias_species{$alias_id} = "s".$species;
                            }
                            else{
                            # print STDERR "\tProblem with $reac_sub, the border compound is not unique.\n";
                            }
                        }
                    }
                }
                # Defines a reaction if products and substracts of a kegg reaction available

                if(%subst_alias_species && %produ_alias_species){

                    $reaction_ref->{$re} = {    'reversible' => $reversible,
                                                'reactions' => $reactions,
                                                'name'=> $molecule_ref->{$ec_alias}->{'identis'},
                                                'id' => $re,
                                                'aliasReac' => \%subst_alias_species,
                                                'aliasProd' => \%produ_alias_species,
                                                'mod_id'=> [$molecule_ref->{$ec_alias}->{'species'}],
                                                'mod_alias'=> [$ec_alias],
                                                'mod_name' => [$molecule_ref->{$ec_alias}->{'identis'}]};
                }
            }
        }
    }
    return $reaction_ref;
}

=item changeModifierCoordinates($reaction_ref,$molecule_ref)

Changes the position of enzyme-symboles overdrawn by reactions.

RETURNS: $molecule_ref

=cut

sub changeModifierCoordinates {

    my ($reaction_ref,$molecule_ref) = @_;

    foreach my $reaction (keys(%{$reaction_ref})){

        if ($reaction_ref->{$reaction}->{'mod_alias'}){

            my @modifier_aliases = @{$reaction_ref->{$reaction}->{'mod_alias'}};
            my $product_aliases = $reaction_ref->{$reaction}->{'aliasProd'};
            my $substrat_aliases = $reaction_ref->{$reaction}->{'aliasReac'};

            # Move the enzyme symbole if  there is only one product, substrat and modifier
            if (scalar(keys(%{$substrat_aliases})) == 1 && scalar(keys(%{$product_aliases})) == 1){

                my ($subst_alias) = keys(%{$substrat_aliases});
                my ($prod_alias) = keys(%{$product_aliases});

                my $subst_x = $molecule_ref->{$subst_alias}->{'x'};
                my $prod_x = $molecule_ref->{$prod_alias}->{'x'};

                my $subst_y = $molecule_ref->{$subst_alias}->{'y'};
                my $prod_y = $molecule_ref->{$prod_alias}->{'y'};

                for (my $i=0;$i<=$#modifier_aliases;$i++){

                    my ($mod_alias) =$modifier_aliases[$i]; 
                    my $mod_x = $molecule_ref->{$mod_alias}->{'x'};
                    my $mod_y = $molecule_ref->{$mod_alias}->{'y'};

                    # Compare the x coordinates of substrat and product, search for vertical connections
                    # Move the enzyme symbole rightwards
                    if (abs($subst_x-$prod_x) < DEVIATION && (abs($mod_x-$subst_x) < DEVIATION)){
                        $molecule_ref->{($modifier_aliases[$i])}->{'x'} = $mod_x + (DEVIATION*2);
                    } 
                    # Compare the y coordinates of substrat and product, search for horizontal connections
                    # Move the enzyme symbole upwards
                    if (abs($subst_y-$prod_y) < DEVIATION && (abs($mod_y-$subst_y) < DEVIATION)){
                        $molecule_ref->{($modifier_aliases[$i])}->{'y'} = $mod_y - DEVIATION;
                    }
                }
            }
        }
    }
    return $molecule_ref;
}

=item joinMulticatalyzedReactions($reaction_ref,$molecule_ref)

Joins reactions calalyzed by more than one enzyme and changes their reaction reference.

RETURNS: $reaction_ref,$molecule_ref

=cut

sub joinMulticatalyzedReactions{

    my ($reaction_ref,$molecule_ref)= @_;
    my $hash_subalias_prodalias;

    # Pass through all reaction entries, one reaction and one enzyme
    foreach my $new_reaction (keys(%{$reaction_ref})){
        # Ignore map-connections
        if(!($new_reaction =~ /resa.+/)){
 
            # Get associated products and reactants of a reaction
            my $hash_subst = $reaction_ref->{$new_reaction}->{'aliasReac'};
            my $hash_prod = $reaction_ref->{$new_reaction}->{'aliasProd'};
            my $direction = $reaction_ref->{$new_reaction}->{'reversible'};

            my @save_aliases;
            push(@save_aliases,sort(keys(%{$hash_subst})));
            push(@save_aliases,sort(keys(%{$hash_prod})));

            my $identifier = join("_",@save_aliases)."_".$direction;

            # Check, weather a reaction with same products and reactants is still existent
            if (defined($hash_subalias_prodalias->{$identifier})){

                my $old_reaction = $hash_subalias_prodalias->{$identifier};

                # Join the modification-species-ids
                my (@old_mod_id) = @{$reaction_ref->{$old_reaction}->{'mod_id'}};
                my (@new_mod_id) = @{$reaction_ref->{$new_reaction}->{'mod_id'}};
                push(@old_mod_id,@new_mod_id);

                # Join the modification-alias-ids
                my (@old_mod_alias) = @{$reaction_ref->{$old_reaction}->{'mod_alias'}};
                my (@new_mod_alias) = @{$reaction_ref->{$new_reaction}->{'mod_alias'}};
                push(@old_mod_alias,@new_mod_alias);

                # Join associated kegg-reactions
                my $reaction = $reaction_ref->{$old_reaction}->{'reactions'}."\n".$reaction_ref->{$new_reaction}->{'reactions'};  
                my $name = $reaction_ref->{$old_reaction}->{'name'}.", ".$reaction_ref->{$new_reaction}->{'name'}; #!!!

                # Renew modification-data
                $reaction_ref->{$old_reaction}->{'mod_id'} = \@old_mod_id;
                $reaction_ref->{$old_reaction}->{'mod_alias'} = \@old_mod_alias;
                $reaction_ref->{$old_reaction}->{'reactions'} = $reaction;  $reaction_ref->{$old_reaction}->{'name'} = $name;#!!!

                $reaction_ref->{$new_reaction} = undef;

                # Change the old reaction reference of a protein
                $molecule_ref->{($new_mod_alias[0])}->{'re'} = $old_reaction;

            }
            else{
                $hash_subalias_prodalias->{$identifier} = $new_reaction;
            }

            # Avoid self-loop-reactions
            if ($save_aliases[0] eq $save_aliases[1]){
                $reaction_ref->{$new_reaction} = undef;
            }
        }
    }
    return($reaction_ref,$molecule_ref);
}

=item getCompoundName($comp_id,$hash_compoundId_name)

Replaces the compound ID with a short alternative KEGG name.

RETURNS: $name or $com_id and $altname

=cut

sub getCompoundName2 {
    my ($comp_id,$hash_compoundId_name) = @_;

    my $altname = "";
    foreach my $name1 (@{$hash_compoundId_name->{$comp_id}}) {
        $altname .= ($name1."\n");
    }

    foreach my $name (@{$hash_compoundId_name->{$comp_id}}) {

        if ($name=~/[\d+|\w+].+;/){
            ($name) = $name=~/([\d+|\w+].+);/;
        }
        if(length($name)< 10) {
                return ($name,$altname);
        }
    }
    return ($comp_id,$altname);
}


=item getCompoundName($comp_id,$hash_compoundId_name)

Replaces the compound ID with a fixed shortcut name of a self-made list.

RETURNS: $name or $com_id and $altname

=cut

sub getCompoundName3 {
    my ($comp_id,$hash_compoundId_name,$shortcuts) = @_;

    my $altname = "";
    foreach my $name1 (@{$hash_compoundId_name->{$comp_id}}) {
        $altname .= ($name1."\n");
    }

    if ($shortcuts->{$comp_id}){
        my $name = $shortcuts->{$comp_id};
        return ($name,$altname);
    }
    else{
        return ($comp_id,$altname);
    }
}

=back 
