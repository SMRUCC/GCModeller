package CarmenConfig;

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

use strict;
use warnings;
use base qw(Exporter);

1;

=head1 NAME

CarmenConfig.pm

=head1 DESCRIPTION

This file administrates constant values that are required during the reconstruction process.

Please create a directory 'kegg_data' and adjust the KEGG_DIRECTORY constant.

To execute the KGML-based feature of CARMEN, you have to download various files and store them into the 'kegg_data' directory:

File:compound (ftp://ftp.genome.jp/pub/kegg/ligand/compound/)

File:reaction.lst (ftp://ftp.genome.jp/pub/kegg/ligand/reaction/)

For the CARMEN reconstruction process KGML files are required. Save the pathways of interest, e.g. File:ec00010.xml. 
The files can be downloaded on: ftp://ftp.genome.jp/pub/kegg/xml/kgml/metabolic/ec/


For testing you can use the attached KGML file of the glycolysis (ec00010.xml).

If you need help, please send a message to 'jschneid@cebitec.uni-bielefeld.de'!

=over

=cut


# Update path to your compound file in your kegg directory: 
use constant KEGG_COMPOUND_FILE => "/vol/biodb/kegg-mirror-2014-10/ligand/compound/compound";

# Update path to your reaction file in your kegg directory: 
use constant KEGG_REACTION_FILE => "/vol/biodb/kegg-mirror-2014-10/ligand/reaction/reaction";

# Update path to your directory containing the KEGG ec maps:
use constant KEGG_EC => "/vol/biodb/kegg-mirror-2014-10/kgml/metabolic/ec/"; #old:"/vol/biodb/KEGG/ec/";

# Set dimension of one reconstructed pathway
use constant TOTAL_HEIGHT => 2700;
use constant TOTAL_WIDTH => 2900;

# Set scale factores
use constant DEVIATION => 25;
use constant SCALE => 1,18;

# Set values for simple molecules
use constant COFACTOR_COLOR => "ffc6f73e";
use constant MOLECULE_HEIGHT => "18";
use constant MOLECULE_WIDTH => "55"; # default 55
use constant MOLECULE_COLOR => "ff81eb16";

# Set values for enzyme symboles
use constant ENZYME_HEIGHT => "18";
use constant ENZYME_WIDTH => "55"; # default 55
use constant ENZYME_COLOR => "ffffff00";
use constant MISSING_ENZYME_COLOR => "ffcdcdcd";

# Set values for map references
use constant MAP_COLOR => "ffe4e4e4";

our @EXPORT_OK=qw(  KEGG_COMPOUND_FILE
                    KEGG_REACTION_FILE
                    KEGG_EC
                    TOTAL_HEIGHT
                    TOTAL_WIDTH
                    DEVIATION
                    SCALE
                    COFACTOR_COLOR
                    MOLECULE_HEIGHT
                    MOLECULE_WIDTH
                    MOLECULE_COLOR
                    ENZYME_HEIGHT
                    ENZYME_WIDTH
                    ENZYME_COLOR
                    MISSING_ENZYME_COLOR
                    MAP_COLOR);

=back
