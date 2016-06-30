##利用bioperl解决blast结果的处理
##姜伟
##2007，10，8

use strict;
use Bio::SearchIO; 
use Bio::AlignIO;

my $infile=$ARGV[0];
my $outfile=$ARGV[1];
my $in = new Bio::SearchIO(-format => 'blast',
						   -file   => $infile);
open(OUT,">$outfile");

my @title=qw(Query_name,hit_name,Query_length,hit_length,Score,Evalue,Identities,Positive,Length_hit,Length_query,length_hsp);

print OUT join("\t",@title);
print OUT "\n";
my %hash;
my @a;
my $c;

input: while (my $result=$in->next_result){
			if (my $num_hit=$result->num_hits <1){
				print OUT $result->query_name."\t"."No Hits Found"."\n";
			}
			else{
				while (my $hit=$result->next_hit){
					while(my $hsp=$hit->next_hsp){
						my $score=$hit->raw_score;
						my $name=$hit->name;
						$hash{$name}++;
	
						print OUT $result->query_name."\t".$hit->name."\t";
						print OUT $result->query_length."\t".$hit->length."\t";
						print OUT $hit->raw_score."\t".$hsp->evalue."\t";
						print OUT $hsp->frac_identical."\t".$hsp->frac_conserved."\t";
						print OUT $hsp->length('query')."\t".$hsp->length('hit')."\t".$hsp->hsp_length."\n";

						goto input;
					}
				}
			}
		}

close(IN);
close(OUT);
