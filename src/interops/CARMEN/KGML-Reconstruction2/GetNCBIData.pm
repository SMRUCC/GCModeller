package GetNCBIData;

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

GetNCBIData.pm

=head1 DESCRIPTION

Gets different kinds of NCBI-information.

=head2 Available methods

=over

=cut


1;

#------------------------------------------------------------------------------
# Imports
#------------------------------------------------------------------------------
use strict;
use warnings;


=item getECNumbers($master,$new_contig_name)

Selects EC-numbers of all CDS and their associations to genes.

RETURNS: $hash_annotEC_genes

=cut

sub getECNumbers{
    my ($ncbi_file) = @_;

    my %hash_annotEC_genes;
    my %hash_genes_altname;

    my $check = 0;
    my $line;

    open(DATA,"< $ncbi_file") || die "can't open file: $ncbi_file\n";

    my $genename;my $altname;

    while ($line = <DATA>) {
        if ($line =~m/CDS/){
            $check = 1;
        }        
        elsif ($line =~m/\/gene=".+"/ && ($check==1)){
            ($altname) = $line =~m/\/gene="(.+)"/;
        }
        elsif ($line =~m/\sgene/){
            $check = 0;
        }
        elsif ($line =~m/locus_tag.+/ && $check==1){
            ($genename) = ($line =~m/locus_tag="(.+)"/);
            $check = 2;
        }


        if (($line =~m/\/EC_number="\d.+"/) && ($check == 2)) {

            my ($ec_number) = ($line =~m/\/EC_number="(\d.+)"/);

            # we have a complete EC number, store it in our hash
            if (defined ($hash_annotEC_genes{$ec_number})) {
                push @{$hash_annotEC_genes{$ec_number}}, $genename;
            }
            else {
                $hash_annotEC_genes{$ec_number} = [$genename];
            }
            $hash_genes_altname{$genename} = $altname;
        }
    }
    return (\%hash_annotEC_genes,\%hash_genes_altname);
}


sub getECNumbersList{
    my ($file) = @_;

    my %hash_annotEC_genes;
    my %hash_genes_altname;

    my $check = 0;
    my $line;

    open(DATA,"< $file") || die "can't open file: $file\n";

    my $genename;
    my $altname;

    while ($line = <DATA>) {

        my ($genename,$altname,$ec_number) = split("\t",$line);

            ($altname) = ($altname=~/Bv\d*[\d*|\w*]_(.+)/);

            chomp($ec_number);
            if (defined ($hash_annotEC_genes{$ec_number})) {
                push @{$hash_annotEC_genes{$ec_number}}, $genename;
            }
            else {
                $hash_annotEC_genes{$ec_number} = [$genename];
            }

            $hash_genes_altname{$genename} = $altname;
    }
    return (\%hash_annotEC_genes,\%hash_genes_altname);
}

=back