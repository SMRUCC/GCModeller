package GetXMLData;

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

GetXMLData.pm

=head1 DESCRIPTION

Gets different information of the KEGG-XML-file.

=head2 Available methods

=over

=cut

use strict;
use warnings;
use Data::Dumper;
use XML::DOM;

1;

=item readMaps($map,$rootelement)

Fetches information of the KEGG-maps and saves information of the entry, relation and reaction-tags in different hashes, extracts the title of every KEGG-map. 

RETURNS: $entry_data_ref,$relation_data_ref,$reaction_data_ref,$title

=cut

sub readMaps {

    my ($map,$rootelement) = @_;
    my $entry_data_ref;
    my $relation_data_ref;
    my $reaction_data_ref;

    my $pwd = `pwd`;
    chomp $pwd;

    my $title = $rootelement->getAttribute("title");

    # Get attribute-information of the entry tag
    for my $entry ($rootelement->getElementsByTagName("entry")) {
        my $id = $entry->getAttribute("id");
        my $name = $entry->getAttribute("name");
        my $type = $entry->getAttribute("type");

        my $reaction;
        if($entry->getAttribute("reaction")){
            $reaction = $entry->getAttribute("reaction");
        }
        else{
            $reaction = "undef";
        }
        my $link;
        if((($reaction eq "undef") && $type eq "map" && ($entry->getAttribute("link")) =~ /.+\/(map\d+).html/)){
            ($link) = (($entry->getAttribute("link")) =~ /.+\/(map\d+).html/);
       #     print $link."\n";
        }
        else{
            $link = "undef";
        }
        # Get graphics-information foreach entry tag
        for my $graphics ($entry->getElementsByTagName("graphics")){
            my $x = $graphics->getAttribute("x");
            my $y = $graphics->getAttribute("y");
            my $w = $graphics->getAttribute("width");
            my $h = $graphics->getAttribute("height");
            my $g_name = $graphics->getAttribute("name");

            # Avoid map symboles with the map title
            if(!($g_name=~/TITLE.+/)){

            $entry_data_ref->{$id} = {  'entry_id' => $id,
                                        'entry_name' => $name,
                                        'entry_type' => $type,
                                        'graphics_x' => $x,
                                        'graphics_y' => $y,
                                        'graphics_w' => $w,
                                        'graphics_h' => $h,
                                        'entry_reaction' => $reaction,
                                        'title' => $title,
                                        'graphics_name' => $g_name,
                                        'entry_link' => $link
                                    };
            }
        }
    }
    my $rel_count = 1;
    # Get attribute-information of the relation tag
    for my $relation ($rootelement->getElementsByTagName("relation")) {
        my $entry1 = $relation->getAttribute("entry1");
        my $entry2 = $relation->getAttribute("entry2");
        my $type = $relation->getAttribute("type");

        # Get graphics-information foreach entry tag
        for my $relation ($relation->getElementsByTagName("subtype")){
            my $name = $relation->getAttribute("name");
            my $value = $relation->getAttribute("value");

            # if ($type ne "maplink"){
            $relation_data_ref->{$rel_count} = {    'relation_entry1' => $entry1,
                                                    'relation_entry2' => $entry2,
                                                    'relation_type' => $type,
                                                    'subtype_name' => $name,
                                                    'subtype_value' => $value
                                                };
            $rel_count++;
            # }
        }
    }
    # Get attribute-information of the reaction tag
    for my $reaction($rootelement->getElementsByTagName("reaction")) {
        my $name = $reaction->getAttribute("name");
        my $type = $reaction->getAttribute("type");
        my $id = $reaction->getAttribute("id");

        my @substrats;
        my @substrate_ids;
        my @products;
        my @product_ids;

        # Get associated substrats
        for my $substrate ($reaction->getElementsByTagName("substrate")){
            my $sub_name = $substrate->getAttribute("name");
            my $sub_id = $substrate->getAttribute("id");
            push(@substrats,$sub_name);
            push(@substrate_ids, $sub_id);
        }
        # Get associated products
        for my $product ($reaction->getElementsByTagName("product")){
            my $prod_name = $product->getAttribute("name");
            my $prod_id = $product->getAttribute("id");
            push(@products,$prod_name);
            push(@product_ids, $prod_id);
        }
        $reaction_data_ref->{$id} = {'reaction_id' => $id,
        					 'reaction_name' => $name,
                             'reaction_type' => $type,
                             'substrates' => \@substrats,
                             'products' => \@products,
                             'substrate_ids' => \@substrate_ids,
                             'product_ids' => \@product_ids,
                            };
#         if(defined($reaction_data_ref->{$id})) {
#         	my $arr_ref = $reaction_data_ref->{$id};
#         	push (@$arr_ref, $this_reaction);
#         } else {
#         	my @new_arr;
#         	push (@new_arr, $this_reaction);
#         	$reaction_data_ref->{$id} = \@new_arr;
#         }
    }
    return ($entry_data_ref,$relation_data_ref,$reaction_data_ref,$title);
}
