package WriteSBML2_4;

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

WriteSBML2_4.pm

=head1 DESCRIPTION

Writes a SBML Level 2 Version 4 file.

=head2 Methods

=over

=cut

use strict;
use warnings;
use Data::Dumper;
use XML::Writer;
use IO::File;

1;

=item writeDocument($molecule_ref,$heigth,$width,$reaction_ref,$hash_layer)

Coordinates the reconstruction of the SBML file.

RETURNS: none

=cut

sub writeSBML{

    my ($molecule_ref,$heigth,$width,$reaction_ref,$hash_layer,$output_file) = @_;

    my $output = new IO::File(">".$output_file."_2-4.sbml");
    my $writer = new XML::Writer(OUTPUT => $output);

    $writer = writeHead($writer,$heigth,$width);
 
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
                        "level" => "2",
                        "version" => "4");
                        #"xmlns:html" => "http://www.w3.org/1999/xhtml");
    $writer->characters("\n");
    $writer->startTag(  "model",
                        "id" => "model1",
                        "name" => "untitled");
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
                        "spatialDimensions" => "0");
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

  #  my $molecule_type = $molecule->{'type'};

   if($species_ref->{$molecule->{'species'}}){
       return $writer,$species_ref;
   }
   else{
        $species_ref->{$molecule->{'species'}} = 1;

   if ($molecule->{'type'} eq "SIMPLE_MOLECULE" || $molecule->{'type'} eq "GLYCAN") {

        $writer->emptyTag(  "species",
                            "id" => $molecule->{'species'},
                            "name" => $molecule->{'name'},
                            "compartment" => "default"); 
        $writer->characters("\n");

        return $writer,$species_ref;
    }
    elsif($molecule->{'type'} eq "PROTEIN"){

        # Replace an ec-number with its short alternative name, if it exists
        my $name;
        if ($molecule->{'altshort'}){
            $name = $molecule->{'altshort'};
        }
        else{
            $name = $molecule->{'name'}
        }
        $writer->emptyTag(  "species",
                            "id" => $molecule->{'species'},
                            "name" => $name,
                            "compartment" => "default"); 
        $writer->characters("\n");

        return $writer,$species_ref;

    }
    else{return $writer,$species_ref;}
  }
}

=item writeListOfReactions($writer,$reaction_ref)

Writes ListOfReactions for each reaction based on the KEGG-maps.

RETURNS: $writer

=cut

sub writeListOfReactions{

    my ($writer,$reaction_ref,$molecule_ref) = @_;


    if ($reaction_ref->{'id'} =~ /resa.+/){
        return $writer;
    }

    $writer->startTag(  "reaction",
                        "id" => $reaction_ref->{'id'},
                        "name" => $reaction_ref->{'name'},
                        "reversible" => $reaction_ref->{'reversible'});
    $writer->characters("\n"); 
    $writer->startTag("notes");
    $writer->characters("\n");
    $writer->startTag(  "body",
                        "xmlns" => "http://www.w3.org/1999/xhtml");
    $writer->characters("\n"); 
    $writer->characters("REACTION_ID: ".$reaction_ref->{'reactions'});

    if($reaction_ref->{'mod_id'}){
        my @modis = @{$reaction_ref->{'mod_id'}};
        for (my $l=0;$l<=$#modis;$l++){

            my $mod_alias = $reaction_ref->{'mod_alias'}->[$l];
            my $mod_name;

            if ($molecule_ref->{$mod_alias}->{'altshort'}){
                my $mod_alt = $molecule_ref->{$mod_alias}->{'altshort'};
                $writer->characters("\n"); 
                $writer->characters("PROTEIN_ASSOCIATION: ".$mod_alt);
            }
            if($molecule_ref->{$mod_alias}->{'altnames'} =~ /.+\n$/){
                ($mod_name) = ($molecule_ref->{$mod_alias}->{'altnames'} =~ /(.+)\n$/);
            }
            else{
                $mod_name = ($molecule_ref->{$mod_alias}->{'altnames'});
            }
            $writer->characters("\n"); 
            $writer->characters("GENE_ASSOCIATION: ".$mod_name);
        }
    }
    $writer->characters("\n");
    $writer->endTag("body");
    $writer->characters("\n");
    $writer->endTag("notes");
    $writer->characters("\n");
    $writer->startTag("listOfReactants");
    $writer->characters("\n");

    my %reac = %{$reaction_ref->{'aliasReac'}};
    my $stoich;
    foreach my $reac_alias (keys(%reac)){

        if(defined($reaction_ref->{'stoichiometry'}->{$reac_alias})){
            $stoich = $reaction_ref->{'stoichiometry'}->{$reac_alias};
        }
        else{
            $stoich = "1";
        }
        $writer->emptyTag(  "speciesReference",
                            "species" => $reac{$reac_alias},
                            "stoichiometry" => $stoich);
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
            $writer->emptyTag(  "speciesReference",
                                "species" => $reaction_ref->{'aliasReacLink'}->{$aliasRL},
                                "stoichiometry" => $stoich);
            $writer->characters("\n");
        }
    }
    $writer->endTag("listOfReactants");
    $writer->characters("\n");

    $writer->startTag("listOfProducts");
    $writer->characters("\n");

    my %prods = %{$reaction_ref->{'aliasProd'}};

    foreach my $prod_alias (keys(%prods)){
            if($reaction_ref->{'stoichiometry'}->{$prod_alias}){
                $stoich = $reaction_ref->{'stoichiometry'}->{$prod_alias};
            }
            else{
                $stoich = "1";
            }
        $writer->emptyTag(  "speciesReference",
                            "species" => $prods{$prod_alias},
                            "stoichiometry" => $stoich);
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
        $writer->emptyTag(  "speciesReference",
                            "species" => $reaction_ref->{'aliasProdLink'}->{$aliasRL},
                            "stoichiometry" => $stoich);
        $writer->characters("\n");
        }
    }
    $writer->endTag("listOfProducts");
    $writer->characters("\n");

    if($reaction_ref->{'mod_id'}){
        $writer->startTag("listOfModifiers");
        $writer->characters("\n");
        my @modis = @{$reaction_ref->{'mod_id'}};
        for (my $l=0;$l<=$#modis;$l++){
            $writer->emptyTag(  "modifierSpeciesReference",
                                "species" => $reaction_ref->{'mod_id'}->[$l]);
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