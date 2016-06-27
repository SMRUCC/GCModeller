use Bio::DB::GenBank;
use Bio::Seq;

# Download and search gbk
my $gb     = new Bio::DB::GenBank(); 
my $gbk    = $gb->get_Seq_by_acc('[$ACCESSION_ID]'); 
my $seq_io = Bio::SeqIO->new(-format => 'genbank', -file => ">[$SAVED_FILE]");

# Call Write FileStream 
$seq_io->write_seq($gbk);