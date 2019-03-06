use CommandLine;

my $cli = CommandLine->new(qw/1 2 3 4 --test=5 6 --name ?7/);
my $v1 = $cli->Argument("--test");
my $v2 = $cli->Argument("?7", "999");

print "out:\n";
print "$v1\n";
print "$v2\n";
print "end\n";
