use Bio::DB::GenBank;
use Bio::DB::Query::GenBank;
use Bio::Seq;
use Bio::SeqIO;

$query = "(\"Xanthomonas\"[Organism] OR Xanthomonas[All Fields]) AND (\"unidentified plasmid\"[Organism] OR plasmid[All Fields])";
$queryHandle = Bio::DB::Query::GenBank->new(-db => 'nucleotide', -query => $query );
$gb = Bio::DB::GenBank->new;
$query_stream = $gb->get_Stream_by_query($queryHandle);

while ($gbk = $query_stream->next_seq) {

	my $seq_io = Bio::SeqIO->new(-format => 'genbank', -file => ">/ncbi/".$gbk->accession_number.".gbk");
	
	# Call Write FileStream 
	$seq_io->write_seq($gbk);
}