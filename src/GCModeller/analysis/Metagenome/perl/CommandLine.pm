package CommandLine;

use strict;
use warnings;

our $VERSION = '1.0';

sub new {
	my $class  = shift;
	my @data;
	my @tokens = ();
	
	if (($#_+1) == 1) {
		@data = $ARGV;
	} else {
		@data = @_;
	}
	
	foreach (@data) {
		my @params = split("=", $_);
		
		foreach my $t (@params) {
			push(@tokens, $t);
		}
	}

	bless \@tokens, $class;
}

sub Argument {
	my $self = shift;
	my $name = $_[0];
	my $default;
	
	if (($#_ + 1) > 1) {
		# 有default value
		$default = $_[1];
	}
	
	for (my $i = 0; $i < scalar @{$self}; $i++) {
		if ($name eq $$self[$i]) {
			my $value = $$self[$i + 1];

			if ($value eq "") {
				return $default;
			} else {
				return $value;
			}
		}
	}
	
	return $default;
}