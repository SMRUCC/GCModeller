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
		@data = shift;
	}
	
	foreach $token (@data) {
		my @params = split("=", $token);
		
		foreach $t (@params) {
			push(@tokens, $t);
		}
	}
	
	bless \@tokens, $class;
}

sub Argument {
	my $self = shift;
	my $name = shift;
	my $default;
	
	if (($#_ + 1) > 2) {
		# 有default value
		$default = @_[2];
	}
	
	for (my $i = 0; $i < scalar @{$self}; $i++) {
		if ($name eq $$self[$i]) {
			return $$self[$i + 1];
		}
	}
	
	return $default;
}