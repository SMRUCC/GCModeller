package WriteSBML2_4_CellDesigner;

#
#  Copyright (C) 2011 CeBiTec, Bielefeld University
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

WriteSBML2_4_CellDesigner.pm

=head1 DESCRIPTION

Writes a Celldesigner SBML Level 2 Version 4 file.

=head2 Methods

=over

=cut

use strict;
use warnings;
use XML::Writer;
use IO::File;

1;

=item writeDocument($molecule_ref,$heigth,$width,$reaction_ref,$hash_layer)

Coordinates the reconstruction of the SBML file.

RETURNS: none

=cut

sub writeSBML{

    my ($molecule_ref,$heigth,$width,$reaction_ref,$hash_layer,$output_file) = @_;

    my $output = new IO::File(">".$output_file."_2-4_CellDesigner.sbml");
    my $writer = new XML::Writer(OUTPUT => $output);

    $writer = writeHead($writer,$heigth,$width);

    # Write listOfSpeciesAliases
    $writer->startTag("celldesigner:listOfSpeciesAliases");
    $writer->characters("\n");

    foreach my $mol (sort {$a cmp $b} (keys(%{$molecule_ref}))){
        my $molecule = $molecule_ref->{$mol};
        $writer = writeListOfSpeciesAlias($writer,$molecule);
    }
    $writer->endTag("celldesigner:listOfSpeciesAliases");
    $writer->characters("\n");

    # Write listOfProteins
    $writer->startTag("celldesigner:listOfProteins");
    $writer->characters("\n");
    foreach my $mol (sort {$a cmp $b} (keys(%{$molecule_ref}))){
        my $molecule = $molecule_ref->{$mol};
        $writer = writeListOfProteins($writer,$molecule);
    }
    $writer->endTag("celldesigner:listOfProteins");
    $writer->characters("\n");

    # Writes empty list-tags of the SBML-file
    $writer = writeOtherLists1($writer);

    #Writes the ListOfLayers of the SBML-file, one layer for each Kegg-map
    $writer = writeListOfLayers($writer,$hash_layer);

    # Writes empty list-tags of the SBML-file
    $writer = writeOtherLists2($writer);

    # Writes ListOfCompartments of the SBML-file with a default compartment
    $writer = writeListOfCompartments($writer);

    # Writes ListOfSpecies for each protein-, map- and compoundsymbol of the SBML-file
    $writer->startTag("listOfSpecies");
    $writer->characters("\n");

    my $species_ref;
    foreach my $mol (sort {$a cmp $b} (keys(%{$molecule_ref}))){
        my $molecule = $molecule_ref->{$mol};
        ($writer,$species_ref) = writeListOfSpecies($writer,$molecule,$species_ref);
    }

    $writer->endTag("listOfSpecies");
    $writer->characters("\n");

    # Writes ListOfReactions for each reaction based on the Kegg-maps
    $writer->startTag("listOfReactions");
    $writer->characters("\n");

  #  foreach my $reaction (values(%{$reaction_ref})){
    foreach my $reac (sort {$a cmp $b} (keys(%{$reaction_ref}))){
        my $reaction = $reaction_ref->{$reac};
        if (defined($reaction) && $reaction->{'id'}){
            $writer = writeListOfReactions($writer,$reaction,$molecule_ref);
        }
    }
    $writer->endTag("listOfReactions");
    $writer->characters("\n");

    # Writes the end-tags of a SBML-file
    $writer = writeTail($writer);

    $writer->end();
    $output->close();
}

=item writeHead($writer,$x,$y)

Writes the XML-tags of the XML-head.

RETURNS: $writer

=cut

sub writeHead{

    my ($writer,$x,$y) = @_;

    $writer->xmlDecl("UTF-8");

    $writer->startTag(  "sbml",
                        "xmlns" => "http://www.sbml.org/sbml/level2/version4",
                        "xmlns:celldesigner" => "http://www.sbml.org/2001/ns/celldesigner",
                        "level" => "2",
                        "version" => "4");

    $writer->characters("\n");

    $writer->startTag(  "model",
                        "id" => "untitled");
    $writer->characters("\n");

    $writer->startTag("annotation");
    $writer->characters("\n");

    $writer->startTag("celldesigner:extension");
    $writer->characters("\n");

    $writer->startTag("celldesigner:modelVersion");
    $writer->characters("4.0");
    $writer->endTag("celldesigner:modelVersion");
    $writer->characters("\n");

    $writer->startTag(  "celldesigner:modelDisplay",
                        "sizeX" => $x,
                        "sizeY" => $y);
    $writer->endTag();
    $writer->characters("\n");

    $writer->startTag("celldesigner:listOfCompartmentAliases");
    $writer->characters("\n");
    $writer->endTag("celldesigner:listOfCompartmentAliases");
    $writer->characters("\n");

    $writer->startTag("celldesigner:listOfComplexSpeciesAliases");
    $writer->characters("\n");
    $writer->endTag("celldesigner:listOfComplexSpeciesAliases");
    $writer->characters("\n");

    return $writer;
}

=item writeListOfSpeciesAlias($writer,$molecule)

Writes the ListOfSpecies-entry for each protein-, map- and compoundsymbol of the SBML file.

RETURNS: $writer

=cut

sub writeListOfSpeciesAlias{

    my ($writer,$molecule) = @_;

    my $alias_id = $molecule->{'alias'};
    my $species_id = $molecule->{'species'};

    $writer->startTag(  "celldesigner:speciesAlias",
                        "id" => $alias_id,
                        "species" => $species_id);
    $writer->characters("\n");

        $writer->startTag("celldesigner:activity");
        $writer->characters("inactive");
        $writer->endTag("celldesigner:activity");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:bounds",
                            "x" => $molecule->{'x'},
                            "y" => $molecule->{'y'},
                            "w" => $molecule->{'w'},
                            "h" => $molecule->{'h'});
        $writer->endTag("celldesigner:bounds");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:view",
                            "state" => "usual");
        $writer->endTag("celldesigner:view");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:usualView");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:innerPosition",
                            "x" => "0.0",
                            "y" => "0.0");
        $writer->endTag("celldesigner:innerPosition");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:boxSize",
                            "width" => $molecule->{'w'},
                            "height" => $molecule->{'h'});
        $writer->endTag("celldesigner:boxSize");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:singleLine",
                            "width" => "1.0");
        $writer->endTag("celldesigner:singleLine");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:paint",
                            "color" => $molecule->{'color'},
                            "scheme" => "Color");
        $writer->endTag("celldesigner:paint");
        $writer->characters("\n");

        $writer->endTag("celldesigner:usualView");
        $writer->characters("\n");

        $writer->startTag("celldesigner:briefView");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:innerPosition",
                            "x" => "0.0",
                            "y" => "0.0");
        $writer->endTag("celldesigner:innerPosition");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:boxSize",
                            "width" => $molecule->{'w'},
                            "height" => $molecule->{'h'});
        $writer->endTag("celldesigner:boxSize");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:singleLine",
                            "width" => "1.0");
        $writer->endTag("celldesigner:singleLine");
        $writer->characters("\n");

        $writer->startTag(  "celldesigner:paint",
                            "color" => "3fff0000",
                            "scheme" => "Color");
        $writer->endTag("celldesigner:paint");
        $writer->characters("\n");

        $writer->endTag("celldesigner:briefView");
        $writer->characters("\n");

    $writer->endTag("celldesigner:speciesAlias");
    $writer->characters("\n");

    return $writer;

}

=item writeOtherLists1($writer)

Writes empty list-tags of the SBML file.

RETURNS: $writer

=cut

sub writeOtherLists1{

    my ($writer) = @_;

    $writer->emptyTag("celldesigner:listOfGroups");
    $writer->characters("\n");

    $writer->emptyTag("celldesigner:listOfGenes");
    $writer->characters("\n");

    $writer->emptyTag("celldesigner:listOfRNAs");
    $writer->characters("\n");

    $writer->emptyTag("celldesigner:listOfAntisenseRNAs");
    $writer->characters("\n");

    return $writer;
}


=item writeListOfLayers($writer,$hash_layer)

Writes the ListOfLayers of the SBML file, one layer for each KEGG-map.

RETURNS: $writer

=cut

sub writeListOfLayers{

    my ($writer,$hash_layer) = @_;
    $writer->startTag("celldesigner:listOfLayers");
    $writer->characters("\n");

    foreach my $layer (values(%{$hash_layer})){
        $writer->startTag(  "celldesigner:layer",
                            "id" => $layer->{'id'},
                            "name" => $layer->{'name'},
                            "locked" => "true",
                            "visible" => "true");
        $writer->characters("\n");

        $writer->startTag("celldesigner:listOfTexts");
        $writer->characters("\n");
        $writer->startTag(  "celldesigner:layerSpeciesAlias",
                            "targetType" => "",
                            "targetId" => "",
                            "x" => "0.0",
                            "y" => "0.0");
        $writer->characters("\n");
        $writer->startTag("celldesigner:layerNotes");
        $writer->characters("\n");
        $writer->characters($layer->{'title'});
        $writer->characters("\n");
        $writer->endTag("celldesigner:layerNotes"); 
        $writer->characters("\n");
        $writer->emptyTag(  "celldesigner:bounds", 
                            "x" => $layer->{'x'}+10,
                            "y" => $layer->{'y'}+10,
                            "w" => "500",
                            "h" => "50");
        $writer->characters("\n");
        $writer->emptyTag(  "celldesigner:paint",
                            "color" => "ff000000");
        $writer->characters("\n");
        $writer->emptyTag(  "celldesigner:font",
                            "size" => "22");
        $writer->characters("\n");
        $writer->endTag("celldesigner:layerSpeciesAlias");
        $writer->characters("\n");
        $writer->endTag("celldesigner:listOfTexts");
        $writer->characters("\n");

        $writer->startTag("celldesigner:listOfSquares");
        $writer->characters("\n");
        $writer->startTag( "celldesigner:layerCompartmentAlias",
                        "type" => "Square");
        $writer->characters("\n");
        $writer->emptyTag(  "celldesigner:bounds", 
                            "x" => $layer->{'x'},
                            "y" => $layer->{'y'},
                            "w" => $layer->{'w'},
                            "h" => $layer->{'h'});
        $writer->characters("\n");
        $writer->emptyTag(  "celldesigner:paint",
                            "color" => "ff000000",
                            "scheme" => "Gradation");
        $writer->characters("\n");
        $writer->endTag("celldesigner:layerCompartmentAlias");
        $writer->characters("\n");
        $writer->endTag("celldesigner:listOfSquares");
        $writer->characters("\n");
        $writer->endTag("celldesigner:layer");
        $writer->characters("\n");
    }
    $writer->endTag("celldesigner:listOfLayers");
    $writer->characters("\n");

    return $writer;
}

=item writeOtherLists2($writer)

Writes empty list-tags of the SBML file.

RETURNS: $writer

=cut

sub writeOtherLists2{

    my ($writer) = @_;

    $writer->startTag("celldesigner:listOfBlockDiagrams");
    $writer->characters("\n");
    $writer->endTag("celldesigner:listOfBlockDiagrams");
    $writer->characters("\n");

    $writer->endTag("celldesigner:extension");
    $writer->characters("\n");
    $writer->endTag("annotation");
    $writer->characters("\n");

    return $writer;

}

=item writeListOfUnits($writer)

Writes ListOfUnits of the SBML file.

RETURNS: $writer

=cut

sub writeListOfUnitDefinitions{

    my ($writer) = @_;


    my %units = {   "substance" => "mole",
                    "volume" => "litre",
                    "areas" => "metre",
                    "length" => "metre",
                    "time" => "second"};

    $writer->startTag("listOfUnitDefinitions");
    $writer->characters("\n");

    foreach my $unit (keys(%units)){

        $writer->startTag(  "unitDefinition",
                            "id" => $unit,
                            "name" => $unit);

        if($unit eq "area"){
            $writer->startTag("listOfUnits");
            $writer->startTag("unit",
                              "kind" => $units{$unit},
                              "exponent" => "2");

            $writer->endTag("listOfUnits");
            $writer->characters("\n");
        }
        else{
            $writer->startTag("listOfUnits");
            $writer->startTag("unit",
                              "kind" => $units{$unit});

            $writer->endTag("listOfUnits");
            $writer->characters("\n");
        }
        $writer->endTag("unitDefinition");
    }
    $writer->endTag("listOfUnitDefinitions");
    $writer->characters("\n");

    return $writer;
}

=item writeListOfCompartments($writer)

Writes ListOfCompartments of the SBML file with a default compartment.

RETURNS: $writer

=cut

sub writeListOfCompartments{

    my ($writer) = @_;

    $writer->startTag("listOfCompartments");
    $writer->characters("\n");

        $writer->startTag(  "compartment",
                            "id" => "default",
                            "size" => "1",
                            "units" => "volume");

        $writer->endTag("compartment");
        $writer->characters("\n");

    $writer->endTag("listOfCompartments");
    $writer->characters("\n");

    return $writer;
}

=item writeListOfSpecies($writer,$molecule,$species_ref)

Writes ListOfSpecies for each protein-, map- and compoundsymbol of the SBML file.

RETURNS: $writer,$species_ref

=cut

sub writeListOfSpecies{

    my ($writer,$molecule,$species_ref) = @_;

    my $molecule_type = $molecule->{'type'};

    if($species_ref->{$molecule->{'species'}}){
        return $writer,$species_ref;
    }
    else{

        $species_ref->{$molecule->{'species'}} = 1;

        $writer->startTag(  "species",
                            "id" => $molecule->{'species'},
                            "name" => $molecule->{'name'},
                            "compartment" => "default",
                            "initialAmount" =>"0");
        $writer->characters("\n"); 
        $writer->startTag("notes");
        $writer->characters("\n"); 
        $writer->comment("Notes by CellDesigner");
        $writer->characters("\n"); 
        $writer->startTag("body",
                          "xmlns" => "http://www.w3.org/1999/xhtml");
        $writer->characters("\n");

        if ($molecule->{'type'} eq "SIMPLE_MOLECULE"){
            $writer->characters($molecule->{'identis'}."\n".$molecule->{'altnames'});
        }
        else{
            $writer->characters($molecule->{'altnames'}."\n");
        }
        $writer->endTag("body");
        $writer->characters("\n"); 
        $writer->endTag("notes");
        $writer->characters("\n"); 
        $writer->startTag("annotation");
        $writer->characters("\n");
        $writer->startTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->startTag("celldesigner:positionToCompartment");
        $writer->characters("inside");
        $writer->endTag("celldesigner:positionToCompartment");
        $writer->characters("\n");
        $writer->startTag("celldesigner:speciesIdentity");
        $writer->characters("\n");

        $writer->startTag("celldesigner:class");

        if (($molecule->{'type'} eq "SIMPLE_MOLECULE") || ($molecule->{'type'} eq "MAP") || ($molecule->{'type'} eq "GLYCAN")){
            $writer->characters("SIMPLE_MOLECULE");
        }
        elsif ($molecule->{'type'} eq "PROTEIN"){
            $writer->characters("PROTEIN");
        }
        else{
            print "Error, wrong molecule type in WriteXMLData.pm\n";
        }
        $writer->endTag("celldesigner:class");
        $writer->characters("\n");

        if($molecule_type eq "SIMPLE_MOLECULE" || ($molecule->{'type'} eq "GLYCAN")){
            $writer->startTag("celldesigner:name");
            $writer->characters($molecule->{'name'});
            $writer->endTag("celldesigner:name");
            $writer->characters("\n");
        $writer->endTag("celldesigner:speciesIdentity");
        $writer->characters("\n");
        }
        elsif($molecule_type eq "MAP"){
            $writer->startTag("celldesigner:name");
            $writer->characters($molecule->{'identis'});
            $writer->endTag("celldesigner:name");
            $writer->characters("\n");
        $writer->endTag("celldesigner:speciesIdentity");
        $writer->characters("\n");
        }
        elsif($molecule_type eq "PROTEIN"){
            $writer->startTag("celldesigner:proteinReference");
            $writer->characters($molecule->{'pr'});
            $writer->endTag("celldesigner:proteinReference");
            $writer->characters("\n");
            $writer->endTag("celldesigner:speciesIdentity");
            $writer->characters("\n");
            $writer->startTag("celldesigner:listOfCatalyzedReactions");
            $writer->characters("\n");
            $writer->emptyTag("celldesigner:catalyzed",
                               "reaction" => $molecule->{'re'});
            $writer->characters("\n");
            $writer->endTag("celldesigner:listOfCatalyzedReactions");
            $writer->characters("\n");
        }
        $writer->endTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->endTag("annotation");
        $writer->characters("\n");
        $writer->endTag("species");
        $writer->characters("\n");

        return $writer,$species_ref;
    }
}

=item writeListOfProteins($writer,$molecule)

Writes ListOfProteins for each protein entry of the SBML file.

RETURNS: $writer

=cut

sub writeListOfProteins{

    my ($writer,$molecule) = @_;

    if($molecule->{'type'} && $molecule->{'type'} eq "PROTEIN"){

        # Replace an ec-number with its short alternative name, if it exists
        my $name;
        if ($molecule->{'altshort'}){
            $name = $molecule->{'altshort'};
        }
        else{
            $name = $molecule->{'name'}
        }

        $writer->startTag(  "celldesigner:protein",
                            "id" => $molecule->{'pr'},
                            "name" => $name,
                            "type" => "GENERIC");
        $writer->characters("\n");
        $writer->endTag("celldesigner:protein");
        $writer->characters("\n");
    }
    return $writer;
}

=item writeListOfReactions($writer,$reaction_ref)

Writes ListOfReactions for each reaction based on the KEGG-maps.

RETURNS: $writer

=cut

sub writeListOfReactions{

    my ($writer,$reaction_ref,$molecule_ref) = @_;

    $writer->startTag(  "reaction",
                        "id" => $reaction_ref->{'id'},
                        "name" => $reaction_ref->{'name'},
                        "reversible" => $reaction_ref->{'reversible'});

    $writer->characters("\n"); 
    $writer->startTag("notes");
    $writer->characters("\n"); 
    $writer->comment("Notes by CellDesigner");
    $writer->characters("\n"); 
    $writer->startTag("body",
                      "xmlns" => "http://www.w3.org/1999/xhtml");
    $writer->characters("\n");
    $writer->characters($reaction_ref->{'reactions'}."\n");
    $writer->endTag("body");
    $writer->characters("\n"); 
    $writer->endTag("notes");
    $writer->characters("\n"); 

    $writer->startTag("annotation");
    $writer->characters("\n");
    $writer->startTag("celldesigner:extension");
    $writer->characters("\n");

    $writer->startTag("celldesigner:reactionType");
    $writer->characters("STATE_TRANSITION");
    $writer->endTag("celldesigner:reactionType");
    $writer->characters("\n");

    $writer->startTag("celldesigner:baseReactants");
    $writer->characters("\n");

    my %reac = %{$reaction_ref->{'aliasReac'}};
    my @reac_array = keys(%reac);
    $writer->startTag(  "celldesigner:baseReactant",
                        "species" => $reac{$reac_array[0]},
                        "alias" => $reac_array[0]);
    $writer->characters("\n");

    $writer->endTag("celldesigner:baseReactant");
    $writer->characters("\n");

    $writer->endTag("celldesigner:baseReactants");
    $writer->characters("\n");

    if ($reac_array[1] || $reaction_ref->{'aliasReacLink'}){
        $writer->startTag("celldesigner:listOfReactantLinks");
        $writer->characters("\n");

      if ($reac_array[1]){
        for (my $s=1;$s<=$#reac_array;$s++){   
           $writer->startTag(  "celldesigner:reactantLink",
                                "reactant" => $reac{$reac_array[$s]},
                                "alias" => $reac_array[$s],
                                "targetLineIndex" => "-1,0");
            $writer->characters("\n");
            $writer->emptyTag(  "celldesigner:line",
                                "width" => "1.0",
                                "color" => "ff000000",
                                "type" => "Straight");
            $writer->characters("\n");
            $writer->endTag("celldesigner:reactantLink");
            $writer->characters("\n");
            }
        }
    if ($reaction_ref->{'aliasReacLink'}){
       foreach my $aliasRL (keys(%{$reaction_ref->{'aliasReacLink'}})){

            my $catalyse_color = "ff000000";

            $writer->startTag(  "celldesigner:reactantLink",
                                "reactant" => $reaction_ref->{'aliasReacLink'}->{$aliasRL},
                                "alias" => $aliasRL,
                                "targetLineIndex" => "-1,0");
            $writer->characters("\n");
            $writer->emptyTag(  "celldesigner:line",
                                "width" => "1.0",
                                "color" => $catalyse_color,
                                "type" => "Straight");
            $writer->characters("\n");
            $writer->endTag("celldesigner:reactantLink");
            $writer->characters("\n");
            }
        }
     $writer->endTag("celldesigner:listOfReactantLinks");
     $writer->characters("\n");
    }

    $writer->startTag("celldesigner:baseProducts");
    $writer->characters("\n");

    my %prods = %{$reaction_ref->{'aliasProd'}};

    my @prods_array = keys(%prods);

       $writer->startTag(  "celldesigner:baseProduct",
                            "species" => $prods{$prods_array[0]},
                            "alias" => $prods_array[0]);
        $writer->characters("\n");

        $writer->endTag("celldesigner:baseProduct");
        $writer->characters("\n");

    $writer->endTag("celldesigner:baseProducts");
    $writer->characters("\n");

    if ($prods_array[1] || $reaction_ref->{'aliasProdLink'}){

        $writer->startTag("celldesigner:listOfProductLinks");
        $writer->characters("\n");

        if ($prods_array[1]){
            for (my $u=1;$u<=$#prods_array;$u++){
                $writer->startTag(  "celldesigner:productLink",
                                    "reactant" => $prods{$prods_array[$u]},
                                    "alias" => $prods_array[$u],
                                    "targetLineIndex" => "-1,0");
                $writer->characters("\n");
                $writer->emptyTag(  "celldesigner:line",
                                    "width" => "1.0",
                                    "color" => "ff000000",
                                    "type" => "Straight");
                $writer->characters("\n");
                $writer->endTag("celldesigner:productLink");
                $writer->characters("\n");
            }
        }
        if ($reaction_ref->{'aliasProdLink'}){

            foreach my $aliasRL (keys(%{$reaction_ref->{'aliasProdLink'}})){

                my $catalyse_color = "ff000000";

                $writer->startTag(  "celldesigner:productLink",
                                    "reactant" => $reaction_ref->{'aliasProdLink'}->{$aliasRL},
                                    "alias" => $aliasRL,
                                    "targetLineIndex" => "-1,0");
                $writer->characters("\n");
                $writer->emptyTag(  "celldesigner:line",
                                    "width" => "1.0",
                                    "color" => $catalyse_color,
                                    "type" => "Straight");
                $writer->characters("\n");
                $writer->endTag("celldesigner:productLink");
                $writer->characters("\n");
            }
        }
        $writer->endTag("celldesigner:listOfProductLinks");
        $writer->characters("\n");
    }
    my $reac_color;
    if($reaction_ref->{'id'} =~ /resa.+/){
        $reac_color = "ffd4d4d4";
    }
    else{
        $reac_color = "ff000000";
    }

    $writer->startTag(  "celldesigner:line",
                        "width" => "1.0",
                        "color" => $reac_color);
    $writer->endTag("celldesigner:line"); 
    $writer->characters("\n");

    my @modis;
    if($reaction_ref->{'mod_id'}){
        $writer->startTag("celldesigner:listOfModification");
        $writer->characters("\n"); 

        @modis = @{$reaction_ref->{'mod_id'}};
        for (my $k=0;$k<=$#modis;$k++){
                $writer->startTag("celldesigner:modification",
                                    "type" => "CATALYSIS",
                                    "modifiers" => $reaction_ref->{'mod_id'}->[$k],
                                    "aliases" => $reaction_ref->{'mod_alias'}->[$k],
                                    "targetLineIndex" => "-1,3");
                $writer->characters("\n"); 
                        $writer->startTag(  "celldesigner:line",
                                "width" => "1.0",
                                "color" => "ff000000");
                        $writer->endTag("celldesigner:line"); 
                        $writer->characters("\n");
                $writer->endTag("celldesigner:modification");
                $writer->characters("\n");
        }
        $writer->endTag("celldesigner:listOfModification");
        $writer->characters("\n");
    }
    $writer->endTag("celldesigner:extension");
    $writer->characters("\n");
    $writer->endTag("annotation");
    $writer->characters("\n");

    $writer->startTag("listOfReactants");
    $writer->characters("\n");

    my $stoich;
    foreach my $reac_alias (keys(%reac)){

        if(defined($reaction_ref->{'stoichiometry'}->{$reac_alias})){
            $stoich = $reaction_ref->{'stoichiometry'}->{$reac_alias};
        }
        else{
            $stoich = "1";
        }

        $writer->startTag(  "speciesReference",
                            "species" => $reac{$reac_alias},
                            "stoichiometry" => $stoich);
        $writer->characters("\n");
        $writer->startTag("annotation");
        $writer->characters("\n");
        $writer->startTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->startTag("celldesigner:alias");
        $writer->characters($reac_alias);
        $writer->endTag("celldesigner:alias");
        $writer->characters("\n");
        $writer->endTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->endTag("annotation");
        $writer->characters("\n");
        $writer->endTag("speciesReference");
        $writer->characters("\n");
    }

    if ($reaction_ref->{'aliasReacLink'}){
       foreach my $aliasRL (keys(%{$reaction_ref->{'aliasReacLink'}})){
           if(defined($reaction_ref->{'stoichiometry'}->{$aliasRL})){
                $stoich = $reaction_ref->{'stoichiometry'}->{$aliasRL};
            }
            else{
                $stoich = "1";
            }
            $writer->startTag(  "speciesReference",
                                "species" => $reaction_ref->{'aliasReacLink'}->{$aliasRL},
                                "stoichiometry" => $stoich);
            $writer->characters("\n");
            $writer->startTag("annotation");
            $writer->characters("\n");
            $writer->startTag("celldesigner:extension");
            $writer->characters("\n");
            $writer->startTag("celldesigner:alias");
            $writer->characters($aliasRL);
            $writer->endTag("celldesigner:alias");
            $writer->characters("\n");
            $writer->endTag("celldesigner:extension");
            $writer->characters("\n");
            $writer->endTag("annotation");
            $writer->characters("\n");
            $writer->endTag("speciesReference");
            $writer->characters("\n");
        }
    }
    $writer->endTag("listOfReactants");
    $writer->characters("\n");

    $writer->startTag("listOfProducts");
    $writer->characters("\n");

    foreach my $prod_alias (keys(%prods)){
            if($reaction_ref->{'stoichiometry'}->{$prod_alias}){
                $stoich = $reaction_ref->{'stoichiometry'}->{$prod_alias};
            }
            else{
                $stoich = "1";
            }

        $writer->startTag(  "speciesReference",
                            "species" => $prods{$prod_alias},
                            "stoichiometry" => $stoich);
        $writer->characters("\n");
        $writer->startTag("annotation");
        $writer->characters("\n");
        $writer->startTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->startTag("celldesigner:alias");
        $writer->characters($prod_alias);
        $writer->endTag("celldesigner:alias");
        $writer->characters("\n");
        $writer->endTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->endTag("annotation");
        $writer->characters("\n");
        $writer->endTag("speciesReference");
        $writer->characters("\n");
    }
    if ($reaction_ref->{'aliasProdLink'}){
       foreach my $aliasRL (keys(%{$reaction_ref->{'aliasProdLink'}})){
             if($reaction_ref->{'stoichiometry'}->{$aliasRL}){
                    $stoich = $reaction_ref->{'stoichiometry'}->{$aliasRL};
             }
            else{
                $stoich = "1";
            }
        $writer->startTag(  "speciesReference",
                            "species" => $reaction_ref->{'aliasProdLink'}->{$aliasRL},
                            "stoichiometry" => $stoich);
        $writer->characters("\n");
        $writer->startTag("annotation");
        $writer->characters("\n");
        $writer->startTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->startTag("celldesigner:alias");
        $writer->characters($aliasRL);
        $writer->endTag("celldesigner:alias");
        $writer->characters("\n");
        $writer->endTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->endTag("annotation");
        $writer->characters("\n");
        $writer->endTag("speciesReference");
        $writer->characters("\n");
        }
    }
    $writer->endTag("listOfProducts");
    $writer->characters("\n");

    if($reaction_ref->{'mod_id'}){
    $writer->startTag("listOfModifiers");
    $writer->characters("\n");

    for (my $l=0;$l<=$#modis;$l++){
        $writer->startTag(  "modifierSpeciesReference",
                            "species" => $reaction_ref->{'mod_id'}->[$l]);
        $writer->characters("\n");
        $writer->startTag("annotation");
        $writer->characters("\n");
        $writer->startTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->startTag("celldesigner:alias");
        $writer->characters($reaction_ref->{'mod_alias'}->[$l]);
        $writer->endTag("celldesigner:alias");
        $writer->characters("\n");
        $writer->endTag("celldesigner:extension");
        $writer->characters("\n");
        $writer->endTag("annotation");
        $writer->characters("\n");
        $writer->endTag("modifierSpeciesReference");
        $writer->characters("\n");
    }
    $writer->endTag("listOfModifiers");
    $writer->characters("\n");
    }
    $writer->endTag("reaction");
    $writer->characters("\n");

    return $writer;
}

=item writeListOfTail($writer)

Writes the end-tags of a SBML file.

RETURNS: $writer

=cut

sub writeTail{

    my ($writer) = @_;

    $writer->endTag("model");
    $writer->characters("\n");
    $writer->endTag("sbml");

    return $writer;
}

=back