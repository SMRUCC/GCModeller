#!/vol/perl-5.8.8/bin/perl

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

KGML_reconstruction.pl

=head1 DESCRIPTION

Perl program to reconstruct KGML-based metabolic pathways, output is stored in SBML (L2V1, L2V4 and L3V1)

=head2 Available packages

=over

=cut

=over

=item CarmenConfig.pm

=item GetXMLData.pm

=item GetSPeciesData_ext.pm

=item KEGG_Information.pm

=item GetNCBIData.pm

=item WriteSBML2_1_CellDesigner.pm

=item WriteSBML2_4.pm

=item WriteSBML2_4_CellDesigner.pm

=item WriteSBML3_1.pm

=over

=cut

use strict;
use warnings;
use Getopt::Std;
use IO::Handle;

use GetXMLData;
use GetSpeciesData;
use KEGG_Information;
use ReactionData;
use GetNCBIData;
use WriteSBML2_1_CellDesigner;
use WriteSBML2_4;
use WriteSBML2_4_CellDesigner;
use WriteSBML3_1;

use CarmenConfig qw(KEGG_COMPOUND_FILE KEGG_REACTION_FILE KEGG_EC TOTAL_HEIGHT TOTAL_WIDTH DEVIATION);

# Global variables
our($opt_o, $opt_g, $opt_n, $opt_k, $opt_c, $opt_a, $opt_e, $opt_l, $opt_f, $opt_m);
getopts('o:g:n:k:c:m:e:l:f:a:');

sub usage {
    print "\nPerl-Programm to reconstruct metabolic pathways based on KGML files of the KEGG database.\n";
    print "\nUsage: perl KGML_reconstruction.pl (-g <Genbank file> || -l <EC number list>) -o <Output> -n <Number of columns> -k <KEGG maps> [-m <metabolite abbreviation>] [-c <Cofactor integration>] [-e <EC number joining>]\n";
    print "\nExample: perl KGML_reconstruction.pl -g xccb100.gb -o output -n 1 -k \"00010 00020\" -c T\n\n";
    print "-o\tName of the SBML-output-file\n";
    print "-g\tName of the Genbank file\n";
    print "-l\tFile containing tab-separated EC number list\n";
    print "-n\tNumber of the columns of the sbml file\n";
    print "-k\tId's of the kegg-maps, for example 00010\n";
    print "-c\tCofactor integration, add T for true (default = F)\n";
    print "-a\tMetabolite abbreviation 1=compound ID 2=KEGG names 3=self-made name (default = 2)\n";
    print "-e\tEC number joining, add T for true (default = F)\n\n";
    print "-f\toutput format, 1=SBML 2.1 (for CellDesigner), 2=SBML2.4; 3=SBML 2.4 (for CellDesigner); 4=SBML 3.1; A=all (default = 3)\n\n";
    print "-m\tinclude maplinks, add T for true (default = F)\n\n";
}

if (!($opt_o) || !($opt_k)){
    usage();
    exit(1);
}

# Default settings
$opt_n = 3 if (!($opt_n));
$opt_a = 2 if (!($opt_a));
$opt_c = "F" if (!($opt_c));
$opt_e = "F" if (!($opt_e));
$opt_f = "3" if (!($opt_f));
$opt_m = "F" if (!($opt_m));


my ($hash_annotEC_genes, $hash_genes_altname);

if($opt_g){
    ($hash_annotEC_genes, $hash_genes_altname) = GetNCBIData::getECNumbers($opt_g);
}
elsif($opt_l){
    ($hash_annotEC_genes, $hash_genes_altname) = GetNCBIData::getECNumbersList($opt_l);
}
else{
    print STDERR "No EC number file found\n";
}

# Adapt this section for your annotation system!

# Create two hashes: $hash_annotEC_genes and $hash_genes_altname
# $hash_annotEC_genes = annotated EC number refers to a list of associated gene names
# $hash_genes_altname = gene name refers to its alternative name (e.g. 'pgi')


my @maps = split(' ',$opt_k);
my $maps_length = scalar(@maps);

my $row = int(($maps_length/$opt_n)+'0.999');
my $col;

if($opt_n > $maps_length){
    $col = $maps_length;
}
else{
    $col = $opt_n;
}

# New object
my $parser = XML::DOM::Parser->new();

my %shortcuts;
# Get metabolite abbreviation, which is stored in a self-made abbreviation list 
if($opt_a eq "3"){
    my $line;
    open(DATA,"<shortcuts.txt") || die "can't open shortcut file\n";
    while ($line = <DATA>) {
        if ($line =~m/C\d+\s+\w+/){
            my ($comp, $name) = $line =~m/(C\d+)\s+(\w+)/;
            $shortcuts{$comp} = $name;
        }
    }
}

# Set some values 0
my $heigth = 0;
my $width = 0;
my $maps_counter = 0;
my $max_alias = 0;
my $max_species = 0;
my $re = 0;
my $pr = 0;

# Define some hashes
my $molecule_ref = {};
my $hash_keggId_species = {};
my $reaction_ref = {};
my $hash_compoundId_species = {};
my $hash_layer = {};

my $hash_ec_modis;

# Fetchall compoundIds and theire alternative name
my $nameFile = KEGG_COMPOUND_FILE;
my $hash_compoundId_name = KEGG_Information::getCompoundNames($nameFile);

# Hashes to save the relationship of a gene name to it's protein id and it's species
my $hash_name_proteinID;
my $hash_name_species;

foreach my $map (@maps){

    # Parse a XML-file
    my $root = $parser->parsefile(KEGG_EC."/ec$map.xml");

    # Get the root element of a XML-file
    my $rootelement = $root->getDocumentElement;

    # Fetches information of the KEGG-maps and save information of the entry, relation and reaction-tags in different hashes, extract the title
    my ($entry_data_ref,$relation_data_ref,$reaction_data_ref,$title) = GetXMLData::readMaps($map,$rootelement);
    print "\n".$title."\n";

    my $hash_keggId_aliasId;
    my $hash_speciesId_speciesAlias;


    # Joins data of the relation, reaction and entry tags of the KEGG-maps and creates a hash containing information of simple molecules, enzymes and maplinks
    ($molecule_ref,$entry_data_ref,$hash_compoundId_name,$heigth,$width,$max_alias,$max_species,$hash_keggId_species,$pr,$re,$hash_keggId_aliasId,$map,$hash_compoundId_species,$hash_speciesId_speciesAlias,$hash_name_proteinID,$hash_name_species) = GetSpeciesData::createMolecule($molecule_ref,$entry_data_ref,$hash_compoundId_name,$heigth,$width,$max_alias,$max_species,$hash_keggId_species,$pr,$re,$hash_keggId_aliasId,$map,$hash_compoundId_species,$hash_annotEC_genes,\%shortcuts, $hash_genes_altname, $hash_name_proteinID,$hash_name_species,$opt_a, $opt_m);

    ($molecule_ref,$max_alias,$hash_speciesId_speciesAlias,$pr,$max_species,$hash_ec_modis, $hash_keggId_species,$hash_name_species) = GetSpeciesData::createProteins($molecule_ref,$hash_genes_altname,$max_alias,$hash_speciesId_speciesAlias,$pr,$max_species,$hash_name_proteinID,$hash_keggId_species,$hash_name_species);

    # Joins data of the relation, reaction and entry tags of the KEGG-maps and molecule information and creates a hash containing reaction information
    $reaction_ref = GetSpeciesData::getReactionData($entry_data_ref,$reaction_data_ref,$relation_data_ref,$molecule_ref,$hash_keggId_aliasId,$reaction_ref,$hash_compoundId_species,$hash_speciesId_speciesAlias,$hash_keggId_species, $opt_m);

    ($reaction_ref,$molecule_ref) = ReactionData::add_modifier($reaction_ref,$molecule_ref,$hash_ec_modis);

    # Saves information of a layer foreach KEGG-map
    $hash_layer->{$maps_counter} = {"id" => $maps_counter,
                                    "title" => $title,
                                    "name" => $map,
                                    "y" => $heigth + 5,
                                    "x" => $width + 5,
                                    "h" => TOTAL_HEIGHT-(DEVIATION*2),
                                    "w" => TOTAL_WIDTH-(DEVIATION*2)};
    $maps_counter++;

    if (($maps_counter % $opt_n) eq 0){
        $heigth = $heigth + TOTAL_HEIGHT;
        $width = 0;
    }
    else{
        $width = $width + TOTAL_WIDTH;
    }
}

# Changes the position of enzyme-symboles overdrawn by reactions
$molecule_ref = GetSpeciesData::changeModifierCoordinates($reaction_ref,$molecule_ref);

# Updates the molecule and reaction data with cofactor information
if ($opt_c eq "T"){

    # Fetchall compoundIds and theire alternative name
    my $reactionFile = KEGG_REACTION_FILE;
    my $hash_reactionId_data = KEGG_Information::getAssociatedCompounds($reactionFile);

    # Updates the molecule and reaction data with cofacotor information
    ($reaction_ref,$molecule_ref) = ReactionData::addCofactors($reaction_ref,$molecule_ref,$hash_reactionId_data,$hash_compoundId_species,$max_alias,$max_species,$hash_compoundId_name,\%shortcuts,$opt_a);
}

# Creates the relationship of one protein to more than one reactions
if ($opt_e eq "T"){
    ($reaction_ref,$molecule_ref) = ReactionData::joinReactionConnections($reaction_ref,$molecule_ref);
}

# Writes a Celldesigner SBML-File Level 2 Version 1, SBML Level 2 Version 4 and SBML Level 3 Version 1 using XML-Writer
print STDERR "\nWriting SBML-file...";


WriteSBML2_1_CellDesigner::writeSBML($molecule_ref,TOTAL_WIDTH*$col,TOTAL_HEIGHT*$row,$reaction_ref,$hash_layer,$opt_o) if ($opt_f eq '1' || $opt_f eq 'A');
WriteSBML2_4::writeSBML($molecule_ref,TOTAL_WIDTH*$col,TOTAL_HEIGHT*$row,$reaction_ref,$hash_layer,$opt_o) if ($opt_f eq '2' || $opt_f eq 'A');
WriteSBML2_4_CellDesigner::writeSBML($molecule_ref,TOTAL_WIDTH*$col,TOTAL_HEIGHT*$row,$reaction_ref,$hash_layer,$opt_o) if ($opt_f eq '3' || $opt_f eq 'A');;
WriteSBML3_1::writeSBML($molecule_ref,TOTAL_WIDTH*$col,TOTAL_HEIGHT*$row,$reaction_ref,$hash_layer,$opt_o) if ($opt_f eq '4' || $opt_f eq 'A');

print " done.\n\n";

=back
