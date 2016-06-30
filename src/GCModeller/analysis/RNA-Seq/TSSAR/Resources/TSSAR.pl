#!/usr/bin/perl
#Last changed Time-stamp: <2013-06-10 17:00:35 fabian>
    
use strict;
use warnings;
use Getopt::Long;
use Data::Dumper;
use Pod::Usage;
use File::Basename;
use File::Temp qw(tempfile);
use vars qw/ %READS @TSS_plus @TSS_minus %coverage/;

my $call = join(" ", $0, @ARGV);

# set variables and default parameters
my $ctdir ='./';

my $SamFile_Library_P ='';
my $SamFile_Library_M ='';

my $verbose = 0;
my $clean = 1;
my $score_mode = 'd';
my $cluster = 1;
my $prorata = 0;
my $consecutiv_range = 3;
my $mtc = '';

my $genome_size=0;
my $fasta_file='';
my $chr_name='chr';

my $max_pos=0;

## Parameter for R
my $win_size = 1000;
my $min_peak_size = 3;
my $pval_cutoff = 0.0001;
my ($library_P_1, $library_P_0, $library_M_1, $library_M_0 );

##

# parse comand-line arguments
pod2usage(-verbose => 0)
    unless GetOptions(
      "g_size:i"    => \$genome_size,
      "fasta:s"     => \$fasta_file,
      "minPeak:i"   => \$min_peak_size,
      "winSize:i"   => \$win_size,
      "pval:f"      => \$pval_cutoff,
#      "prorata!"    => \$prorata,
      "range:i"     => \$consecutiv_range,
      "verbose"     => \$verbose,
      "clean!"      => \$clean,
      "libP:s"      => \$SamFile_Library_P,
      "libM:s"      => \$SamFile_Library_M,
      "score:s"     => \$score_mode,
#      "mtc:s"       => \$mtc,
      "cluster!"    => \$cluster,
      "tmpdir:s"   => \$ctdir,
      "man"         => sub{pod2usage(-verbose => 2)},
      "help|?"      => sub{pod2usage(-verbose => 1)},
    );

# start message
my @time=localtime();
print STDERR "\n".join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " TSSAR v0.9.6 has started with the following call:\n               $call\n" if ($verbose);


#check if score mode is set either to p or d but nothing else
unless ( $score_mode=~m/p/ || $score_mode=~m/d/ ){
  print STDERR "Please specify how the TSS should be scored by <--score p|d.\n";
  print STDERR "If 'p' is given the scoring (and if applied the clustering) is based on the pValue.\n";
  print STDERR "If 'd' is given the scoring is based on the peak difference, which is also the default if optin --score is omitted.\n";
  exit;
}

# check if genome size can be deduced or is known
if ( !($fasta_file) && !($genome_size) ){
  print STDERR "Please specify genome size (--g_size) OR genomic fasta file (--fasta).\n";
  exit;
}

# check if mtc is set to a valid value
if ($mtc) {
  unless (grep {m/$mtc/} qw/fdr bonferroni holm hochberg hommel none/) {
    print STDERR "Please set the --mtc option to 'fdr', 'bonferroni', 'holm', 'hochberg' or 'hommel'.\nLeave it blank or set it to 'none' to ommite multiple testing correction\n";
    exit;
  }
  if ($mtc eq 'none'){
    $mtc = '';
  }
}

# read in the libraries
@time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... Start Reading mapped read files.\n" if ($verbose);

my $rname_last='';
my $rname_new='';

foreach my $map_file ($SamFile_Library_P, $SamFile_Library_M) {

  my $map_key = basename($map_file);
  $coverage{$map_key}->{'+'}->[1]=0;
  $coverage{$map_key}->{'-'}->[1]=0;
  
  open MAP, "< $map_file" or die "can t open <$map_file> now but why\n";
  my @lines=<MAP>;
  foreach my $line (@lines){
    
    @time=localtime();
    print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... $map_file read @ line $. \n" if ($verbose && !($. % 1000000) );
    
    next if ($line=~m/^@/);
    chomp($line);
    my @F=split"\t", $line;
    
    if (@F<11) {
      print STDERR "line $. : SAM files seems to have less then the 11 mandatory Fields\tomitted from the analysis\n";
      next;
    }
    
    if ($rname_last eq '') {
      $rname_last=$F[2];
      $rname_new=$F[2];
    }

    else {
      $rname_new=$F[2];
      if ($rname_new ne $rname_last ) {
	if ($rname_new ne '*' && $rname_last ne '*') {
	  die "Different reference sequences ($rname_new and $rname_last) were found in the specified input files.\n\t\tPlease check!\nYou might have to split the SAM files and submit for each reference sequence a separate job.\n";
	}
      }
    }
    
    my ($start, $stop, $strand, $alncount) = ();
    my ($leftpos_, $length_, $strand_) = ();
    
    if ($F[5] eq '*' && $F[9] ne '*'){
      ($leftpos_, $length_, $strand_)=($F[3], length($F[9]), $F[1]);
    }
    elsif ($F[5] ne '*') {
      ($leftpos_, $length_, $strand_)=($F[3], &cigarlength($F[5]), $F[1]);
    }
    else {
      print STDERR "in line $. CIGAR  <'$F[5]'> and/or SEQ <'$F[9]'> seems not set;\n";
      next;
    }
    
    if ($prorata && $line=~m/NH:i:(\d+)/) {
      if ($1) {
        $alncount=$1;
      }
      else {
        $alncount=1;
      }
    }
    else {
      $alncount=1;
    }

    if (!($strand_ & 4) && ($strand_ & 16)) {
      $strand='-';
      $stop=$leftpos_;
      $start=$leftpos_+$length_-1;
    }
    elsif (!($strand_ & 4)) {
      $strand='+';
      $start=$leftpos_;
      $stop=$leftpos_+$length_-1;
    }
    else {
      next;
    }
    
    my $map_key = basename($map_file);
    $coverage{$map_key}->{$strand}->[$start]+=(1/$alncount);
    $max_pos = $stop if ($stop > $max_pos);
  }
  @time=localtime();
  print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... $map_file read-in complete\n" if ($verbose);
}

# deduce genome size if fasta file is given
if ( !($genome_size) && $fasta_file){
  my $seq='';
  open FASTA, "< $fasta_file" or die "can t open $fasta_file\n";
  while(<FASTA>) {
    chomp;
    if (m/^>./) {
      if(m/>([^\s]*)/){
        if($chr_name eq 'chr') {
          $chr_name=$1;
        }
        else {
          $chr_name=join(":", $chr_name, $1);
        }
      }      
      next;
    }
    else {
      $seq.=$_;
    }
  }
  close FASTA;
  $genome_size=length($seq);
}

# project the read starts onto the genome and sum up
@time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... summarizing reads\n" if ($verbose);


# check if genome size larger then largest mapped position
if ($genome_size < $max_pos) {
  print STDERR "Used genome size ($genome_size) smaller then largest mapped read position ($max_pos)\nIt seems like an other reference genome was used for mapping then for TSS annotation\nPlease correct\n";
  exit;
}

# print read start map for R into file
@time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... writing coverage files\n" if ($verbose);

foreach my $library (keys %coverage) {
  foreach my $strands (keys %{$coverage{$library}}){
    
	my $rnd = rand();
    my $coveragefile_fh = "$ctdir\\coverage_$time[0]$time[1]$time[2].$strands=$rnd";
    
    $library_P_1=$coveragefile_fh if ($strands eq '+' && $SamFile_Library_P=~m/$library/);
    $library_P_0=$coveragefile_fh if ($strands eq '-' && $SamFile_Library_P=~m/$library/);
    $library_M_1=$coveragefile_fh if ($strands eq '+' && $SamFile_Library_M=~m/$library/);
    $library_M_0=$coveragefile_fh if ($strands eq '-' && $SamFile_Library_M=~m/$library/);
    
    open OUT, ">$coveragefile_fh" or die "can t write to file $coveragefile_fh\n";
    print OUT "position\tcoverage\n";
    
    foreach my $pos (1 .. $genome_size){
      if ( defined($coverage{$library}->{$strands}->[$pos])) {
	my $rounded_coverage_value = int($coverage{$library}->{$strands}->[$pos] + 0.5);
        print OUT "$pos\t$rounded_coverage_value\n";
      }
      else {
        print OUT "$pos\t0\n";
      }
    }
    close OUT;
  }
}

# Generate R.script
my $Rscript_fh = "$ctdir\\R.script_$time[0]$time[1]$time[2].R";
my $R_pval_output_plus_fh = "$ctdir\\pVal_$time[0]$time[1]$time[2]_plus";
my $R_SeenRegion_fh = "$ctdir\\SeenRegion_$time[0]$time[1]$time[2]";
my $R_pval_output_minus_fh = "$ctdir\\pVal_$time[0]$time[1]$time[2]_minus";

@time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... generating R-script ($Rscript_fh)\n" if ($verbose);
print "Rscript_fh ==> $Rscript_fh\n";
print "R_pval_output_plus_fh ==> $R_pval_output_plus_fh\n";
print "R_SeenRegion_fh ==> $R_SeenRegion_fh\n";
print "R_pval_output_minus_fh ==> $R_pval_output_minus_fh\n";


# string replacement for the correctly path on windows
$library_P_1 =~ s/\\/\//g;
$library_P_0 =~ s/\\/\//g;
$library_M_1 =~ s/\\/\//g;
$library_M_0 =~ s/\\/\//g;

$R_pval_output_plus_fh =~ s/\\/\//g;
$R_pval_output_minus_fh =~ s/\\/\//g;
$R_SeenRegion_fh =~ s/\\/\//g;


open R, "> $Rscript_fh" or die "can't write to $Rscript_fh\n";

print R <<"ToRscript";

    # I'm not sure why the skellam package can not be installed into R system, so I modified the imports library skellam a little at here.
    source("{skellam-0}")	
	source("{skellam-1}")	
	source("{skellam-2}")	
	source("{skellam-3}")	
	source("{skellam-4}")	
	source("{skellam-5}")	


    # load skellam and VGAM package
    # suppressPackageStartupMessages(library("skellam"))
    suppressPackageStartupMessages(library("VGAM"))
    options(error=dump.frames)
    
    # load primary 5' coverage for both libraries and both strands
    library_P_1 <- read.table(file="$library_P_1", header=T)
    library_P_0 <- read.table(file="$library_P_0", header=T)
    library_M_1 <- read.table(file="$library_M_1", header=T)
    library_M_0 <- read.table(file="$library_M_0", header=T)
    
    # calculate read count over whole genome for both libraries
    sum_P <- sum(c(library_P_1\$coverage, library_P_0\$coverage), na.rm=TRUE )
    sum_M <- sum(c(library_M_1\$coverage, library_M_0\$coverage), na.rm=TRUE )

    # calculates normalization factors to nomalize the bigger library to the niveau of the smaller one
    normalize_P <- if (sum_P >= sum_M) (sum_M/sum_P) else 1
    normalize_M <- if (sum_M >= sum_P) (sum_P/sum_M) else 1

    # specify parameter in use
    genome_size   <- $genome_size
    win_size      <- $win_size
    min_peak_size <- $min_peak_size
    step_size     <- ceiling(win_size/10)
    
    # Calculation for (+) strand
    # initialize variables
    PVAL_1       <- array(NA, dim=c(genome_size, ceiling(win_size/step_size)+1))
    TSS_1        <- array(NA, dim=c(genome_size, 3))
    SeenRegion_1 <- vector()
    
    # sliding window
    pos <- 1
    while (pos < genome_size) {

          lambda_PP <- 9999
          lambda_MM <- 9999
          lrt0_PP   <- 9999
          lrt0_MM   <- 9999

          fit1P     <- 9999
          pfit1P    <- 9999
          pstr0P    <- 9999
          fit1M     <- 9999
          pfit1M    <- 9999
          pstr0M    <- 9999
	  
          P    <- vector()
          PP   <- vector()
          PPP  <- vector()
          M    <- vector()
          MM   <- vector()
          MMM  <- vector()
	  
          expected_value_P <- 9999
          expected_value_M <- 9999
          
          # extract sub-vector covering the sliding window
          P <- (library_P_1\$coverage[pos:min(genome_size,(pos+win_size-1))])
          M <- (library_M_1\$coverage[pos:min(genome_size,(pos+win_size-1))])
          
          # calculate lambda for each library for the current window
          if (!(length(P[P!=0])==0 | length(P[P==0])==0)) {
            PP <- P
            PP[order(PP)[length(PP)]] <- sort(PP)[length(PP)-1] # winsorizer; ev. 2. wert ersetzen??
            PP[order(PP)[1]]          <- sort(PP)[2]            # winsorizer
            
            #set.seed(123) 
            zdata <- data.frame(x2 = runif(nn <- length(PP)))
            zdata <- transform(zdata,
                               pstr01  = logit(-0.5 + 1*x2, inverse = TRUE),
                               Ps01    = logit(-0.5       , inverse = TRUE),
                               lambda1 = loge(-0.5 + 2*x2, inverse = TRUE))
            zdata <- transform(zdata, y1 = PP)
            
            try( fit1P      <- vglm(y1 ~ x2, zapoisson(zero = 1), zdata, crit = "coef"), silent=TRUE)
            try( predict(fit1P, zdata[1, ]) -> pfit1P, silent=TRUE)
            try( pstr0P     <- logit(pfit1P[1], inverse = TRUE), silent=TRUE)
            try( lambda_PP  <- loge(pfit1P[2], inverse = TRUE), silent=TRUE)
            try( lrt0_PP    <- pstr0P / dzipois(x = 0, lambda = lambda_PP, pstr0 = pstr0P), silent=TRUE)

            try( expected_value_P <- length(PP[PP==0])*lrt0_PP, silent=TRUE)
          } else if (length(P[P==0])==0){
            PP <- P
            PP[order(PP)[length(PP)]] <- sort(PP)[length(PP)-1] # winsorizer; ev. 2. wert ersetzen??
            PP[order(PP)[1]]          <- sort(PP)[2]            # winsorizer
            
            lambda_PP  <- mean(PP, na.rm=TRUE)

            expected_value_P <- 0
          } else if (length(P[P!=0])==0){
            PP <- P

            lambda_PP  <- 0

            expected_value_P  <- length(PP)
          }

          if (!(length(M[M!=0])==0 | length(M[M==0])==0)) {
            MM <- M
            MM[order(MM)[length(MM)]]<-sort(MM)[length(MM)-1] # winsorizer; ev. 2. wert ersetzen??
            MM[order(MM)[1]]<-sort(MM)[2]                     # winsorizer
            
            #set.seed(123)
            zdata <- data.frame(x2 = runif(nn <- length(MM)))
            zdata <- transform(zdata,
                               pstr01  = logit(-0.5 + 1*x2, inverse = TRUE),
                               Ps01    = logit(-0.5       , inverse = TRUE),
                               lambda1 =  loge(-0.5 + 2*x2, inverse = TRUE))
            zdata <- transform(zdata, y1 = MM)
            
            try( fit1M      <- vglm(y1 ~ x2, zapoisson(zero = 1), zdata, crit = "coef"), silent=TRUE)
            try( predict(fit1M, zdata[1, ]) -> pfit1M, silent=TRUE)
            try( pstr0M     <- logit(pfit1M[1], inverse = TRUE), silent=TRUE)
            try( lambda_MM  <- loge(pfit1M[2], inverse = TRUE), silent=TRUE)
            try( lrt0_MM    <- pstr0M / dzipois(x = 0, lambda = lambda_MM, pstr0 = pstr0M), silent=TRUE)

            try(expected_value_M <- length(MM[MM==0])*lrt0_MM, silent=TRUE)
          } else if(length(M[M==0])==0){
            MM <- M
            MM[order(MM)[length(MM)]]<-sort(MM)[length(MM)-1] # winsorizer; ev. 2. wert ersetzen??
            MM[order(MM)[1]]<-sort(MM)[2]                     # winsorizer

            lambda_MM <- mean(MM, na.rm=TRUE)

            expected_value_M <- 0
          } else if(length(M[M!=0])==0){
            MM <- M
                
            lambda_MM  <- 0

            expected_value_M  <- length(MM)
          }
	  if ( !(is.na(expected_value_P) | is.na(expected_value_M)) ) {
          if (expected_value_P != 9999 && expected_value_M != 9999) {
            # store which region on the plus strand could be modeled by zapoisson
            SeenRegion_1[pos:min(genome_size,(pos+win_size-1))] <- 1

            # remove inflated zeros from PP and MM and calculate lambdas
            expected_value_T <- mean(c(expected_value_P, expected_value_M))
              
            lrt0_P    <- expected_value_T/length(PP[PP==0])
            zeros     <- which(PP==0)
            #set.seed(123)
            rand      <- runif(length(zeros), 0, 1)
            exclude   <- which(rand<lrt0_P)
            PPP       <- c(PP[zeros[-exclude]], PP[PP>0])
            lambda_PPP<- mean(PPP, na.rm=TRUE) * normalize_P

            lrt0_M    <- expected_value_T/length(MM[MM==0])
            zeros     <- which(MM==0)
            #set.seed(123)
            rand      <- runif(length(zeros), 0, 1)
            exclude   <- which(rand<lrt0_M)
            MMM       <- c(MM[zeros[-exclude]], MM[MM>0])
            lambda_MMM<- mean(MMM, na.rm=TRUE) * normalize_M

            # calculate the difference between the two libraries
            D <- (P * normalize_P)-(M * normalize_M)
          
            # calculate the probability that each position follows a skellam distribution
	    if ( !(is.na(lambda_PPP) | is.na(lambda_MMM)) ) {
            if (lambda_PPP != 0 | lambda_MMM != 0) {
              pval <- 1-pskellam(D-.Machine\$double.xmin, lambda_PPP, lambda_MMM)
            } else {
	      pval <- rep(1, length(D))
	    }
	    } else {
	      pval <- rep(1, length(D))
	    }
          
            # keep all probabilities if the corresponding position is covered
            # by more then $min_peak_size read starts
             for (p in 1:length(P)) {
               if (!(is.na(library_P_1\$coverage[(pos+p-1)])) & library_P_1\$coverage[(pos+p-1)]>= min_peak_size) {
                 PVAL_1[pos+p-1,match(NA,PVAL_1[pos+p-1,])] <- pval[p]
               }
             }
            #PVAL_1[cbind(pos:(pos+length(pval)-1),apply(PVAL_1, 1, function(x) match(NA,x))[pos:(pos+length(pval)-1)])]<-pval
          } else {
	    for (p in 1:length(P)) {
	      PVAL_1[pos+p-1,match(NA,PVAL_1[pos+p-1,])] <- 1
	    }
	  }
	  } else {
	    for (p in 1:length(P)) {
	      PVAL_1[pos+p-1,match(NA,PVAL_1[pos+p-1,])] <- 1
	    }
	  }
	  
      
      # move sliding window by sprintf("%d", $win_size/10)
      pos <- pos+step_size
    }

    # calculate the geometric mean for all the values for each position
    for ( end_pos in 1:length(PVAL_1[,1]) ) {
      TSS_1[end_pos]   <- exp(mean(log(PVAL_1[end_pos,]),na.rm=TRUE))
      TSS_1[end_pos,2] <- library_P_1\$coverage[(end_pos)]
      TSS_1[end_pos,3] <- library_M_1\$coverage[(end_pos)]
    }

    # write output for (+) strand
    write.table(TSS_1, file="$R_pval_output_plus_fh", sep="\t", append=FALSE, col.names=FALSE)
    
    
  ### Calculation for (-) strand
    # initialize variables
    PVAL_0       <- array(NA, dim=c(genome_size, ceiling(win_size/step_size)+1))
    TSS_0        <- array(NA, dim=c(genome_size, 3))
    SeenRegion_0 <- vector()
    
    # sliding window
    pos <- 1
    while (pos < genome_size) {

      lambda_PP <- 9999
      lambda_MM <- 9999
      lrt0_PP   <- 9999
      lrt0_MM   <- 9999
      
      fit1P     <- 9999
      pfit1P    <- 9999
      pstr0P    <- 9999
      fit1M     <- 9999
      pfit1M    <- 9999
      pstr0M    <- 9999

      P    <- vector()
      PP   <- vector()
      PPP  <- vector()
      M    <- vector()
      MM   <- vector()
      MMM  <- vector()
      
      expected_value_P <- 9999
      expected_value_M <- 9999
      
      # extract sub-vector covering the sliding window
      P <- (library_P_0\$coverage[pos:min(genome_size,(pos+win_size-1))])
      M <- (library_M_0\$coverage[pos:min(genome_size,(pos+win_size-1))])
          
      # calculate lambda for each library for the current window
      if (!(length(P[P!=0])==0 | length(P[P==0])==0)) {
        PP <- P
        PP[order(PP)[length(PP)]] <- sort(PP)[length(PP)-1] # winsorizer; ev. 2. wert ersetzen??
        PP[order(PP)[1]]          <- sort(PP)[2]            # winsorizer

        #set.seed(123)
        zdata <- data.frame(x2 = runif(nn <- length(PP)))
        zdata <- transform(zdata,
                           pstr01  = logit(-0.5 + 1*x2, inverse = TRUE),
                           Ps01    = logit(-0.5       , inverse = TRUE),
                           lambda1 = loge(-0.5 + 2*x2, inverse = TRUE))
        zdata <- transform(zdata, y1 = PP)
            
        try( fit1P      <- vglm(y1 ~ x2, zapoisson(zero = 1), zdata, crit = "coef"), silent=TRUE)
        try( predict(fit1P, zdata[1, ]) -> pfit1P, silent=TRUE)
        try( pstr0P     <- logit(pfit1P[1], inverse = TRUE), silent=TRUE)
        try( lambda_PP  <- loge(pfit1P[2], inverse = TRUE), silent=TRUE)
        try( lrt0_PP    <- pstr0P / dzipois(x = 0, lambda = lambda_PP, pstr0 = pstr0P), silent=TRUE)

        try( expected_value_P <- length(PP[PP==0])*lrt0_PP, silent=TRUE)
      } else if (length(P[P==0])==0){
        PP <- P
        PP[order(PP)[length(PP)]] <- sort(PP)[length(PP)-1] # winsorizer
        PP[order(PP)[1]]          <- sort(PP)[2]            # winsorizer
      
        lambda_PP  <- mean(PP, na.rm=TRUE)

        expected_value_P <- 0
      } else if (length(P[P!=0])==0){
        PP <- P

        lambda_PP  <- 0

        expected_value_P  <- length(PP)
      }

      if (!(length(M[M!=0])==0 | length(M[M==0])==0)) {
        MM <- M
        MM[order(MM)[length(MM)]]<-sort(MM)[length(MM)-1] # winsorizer; ev. 2. wert ersetzen??
        MM[order(MM)[1]]<-sort(MM)[2]                     # winsorizer
            
        #set.seed(123)
        zdata <- data.frame(x2 = runif(nn <- length(MM)))
        zdata <- transform(zdata,
                           pstr01  = logit(-0.5 + 1*x2, inverse = TRUE),
                           Ps01    = logit(-0.5       , inverse = TRUE),
                           lambda1 =  loge(-0.5 + 2*x2, inverse = TRUE))
        zdata <- transform(zdata, y1 = MM)
           
        try( fit1M      <- vglm(y1 ~ x2, zapoisson(zero = 1), zdata, crit = "coef"), silent=TRUE)
        try( predict(fit1M, zdata[1, ]) -> pfit1M, silent=TRUE)
        try( pstr0M     <- logit(pfit1M[1], inverse = TRUE), silent=TRUE)
        try( lambda_MM  <- loge(pfit1M[2], inverse = TRUE), silent=TRUE)
        try( lrt0_MM    <- pstr0M / dzipois(x = 0, lambda = lambda_MM, pstr0 = pstr0M), silent=TRUE)

        try( expected_value_M <- length(MM[MM==0])*lrt0_MM, silent=TRUE)
      } else if(length(M[M==0])==0){
        MM <- M
        MM[order(MM)[length(MM)]]<-sort(MM)[length(MM)-1] # winsorizer; ev. 2. wert ersetzen??
        MM[order(MM)[1]]<-sort(MM)[2]                     # winsorizer

        lambda_MM <- mean(MM, na.rm=TRUE)

        expected_value_M <- 0
      } else {
        MM <- M
            
        lambda_MM  <- 0

        expected_value_M  <- length(MM)
      }

      if ( !(is.na(expected_value_P) | is.na(expected_value_M)) ) {
      if (expected_value_P != 9999 && expected_value_M != 9999) {
        # store which region on the minus strand could be modeled my zapoisson
        SeenRegion_0[pos:min(genome_size,(pos+win_size-1))] <- 1

        # remove inflated zeros from PP and MM and calculate lambdas
        expected_value_T <- mean(c(expected_value_P, expected_value_M))
              
        lrt0_P    <- expected_value_T/length(PP[PP==0])
        zeros     <- which(PP==0)
        #set.seed(123)
        rand      <- runif(length(zeros), 0, 1)
        exclude   <- which(rand<lrt0_P)
        PPP       <- c(PP[zeros[-exclude]], PP[PP>0])
        lambda_PPP<- mean(PPP, na.rm=TRUE) * normalize_P

        lrt0_M    <- expected_value_T/length(MM[MM==0])
        zeros     <- which(MM==0)
        #set.seed(123)
        rand      <- runif(length(zeros), 0, 1)
        exclude   <- which(rand<lrt0_M)
        MMM       <- c(MM[zeros[-exclude]], MM[MM>0])
        lambda_MMM<- mean(MMM, na.rm=TRUE) * normalize_M

        # calculate the difference between the two libraries
        D <- (P * normalize_P)-(M * normalize_M)
        
        # calculate the probability that each position follows a skellam distribution
	if ( !(is.na(lambda_PPP) | is.na(lambda_MMM)) ) {
        if (lambda_PPP != 0 | lambda_MMM != 0){
          pval <- 1-pskellam(D-.Machine\$double.xmin, lambda_PPP, lambda_MMM)
        } else {
          pval <- rep(1, length(D))
        }
        } else {
          pval <- rep(1, length(D))
        }
	
        # keep all probabilities if the corresponding position is covered
        # by more then $min_peak_size read starts
        for (p in 1:length(P)) {
          if (!(is.na(library_P_0\$coverage[(pos+p-1)])) & library_P_0\$coverage[(pos+p-1)]>= min_peak_size) {
            PVAL_0[pos+p-1,match(NA,PVAL_0[pos+p-1,])] <- pval[p]
          }
        }
        #PVAL_0[cbind(pos:(pos+length(pval)-1),apply(PVAL_0, 1, function(x) match(NA,x))[pos:(pos+length(pval)-1)])]<-pval
      } else {
	for (p in 1:length(P)) {
	  PVAL_0[pos+p-1,match(NA,PVAL_0[pos+p-1,])] <- 1
	}
      }
      } else {
	for (p in 1:length(P)) {
	  PVAL_0[pos+p-1,match(NA,PVAL_0[pos+p-1,])] <- 1
	}
      }
          
      # move sliding window by sprintf("%d", $win_size/10)
      pos <- pos+step_size
   }

   # Calculation for (-) strand        
   for ( end_pos in 1:length(PVAL_0[,1]) ) {
     TSS_0[end_pos]   <- exp(mean(log(PVAL_0[end_pos,]),na.rm=TRUE))
     TSS_0[end_pos,2] <- library_P_0\$coverage[(end_pos)]
     TSS_0[end_pos,3] <- library_M_0\$coverage[(end_pos)]
   }

   # write output for (-) strand
   write.table(TSS_0, file="$R_pval_output_minus_fh", sep="\t", append=FALSE, col.names=FALSE)

   # write output which regions could be analysed
   SeenRegion_1[genome_size]<-1
   SeenRegion_0[genome_size]<-1

   write.table(data.frame(PLUS=SeenRegion_1, MINUS=SeenRegion_0), file='$R_SeenRegion_fh')

ToRscript
close R;

# Run R_script.R
@time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... run R-script (might take a while)\n" if ($verbose);

system("{R_PATH} CMD BATCH --vanilla --slave $Rscript_fh") == 0 or die "R-script system call <R CMD BATCH --vanilla --slave $Rscript_fh> failed: $?";

@time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... done with R-script\n" if ($verbose);

# multiple comparising testing
if ($mtc){

  my $R_padjust_fh = "$ctdir\\R.script_$time[0]$time[1]$time[2].R";

									 
  $R_pval_output_plus_fh =~ s/\\/\//g;
  $R_pval_output_minus_fh =~ s/\\/\//g;
  $R_pval_output_plus_fh =~ s/\\/\//g;
  $R_pval_output_minus_fh =~ s/\\/\//g;

									 
open MTC, "> $R_padjust_fh" or die "can't write to $R_padjust_fh\n";
print MTC <<"ToRscript2";

      pval_plus  <- read.table(file="$R_pval_output_plus_fh", sep="\t", row.names=1)
      pval_minus <- read.table(file="$R_pval_output_minus_fh", sep="\t", row.names=1)

      pval_plus\$V2<-p.adjust(pval_plus\$V2, method = "$mtc" )
      write.table(pval_plus, file="$R_pval_output_plus_fh", sep="\t", append=FALSE, col.names=FALSE)

      pval_minus\$V2<-p.adjust(pval_minus\$V2, method = "$mtc" )
      write.table(pval_minus, file="$R_pval_output_minus_fh", sep="\t", append=FALSE, col.names=FALSE)

ToRscript2
close MTC;

 
  system("{R_PATH} CMD BATCH --vanilla --slave $R_padjust_fh") == 0 or die "R-script system call <R CMD BATCH --vanilla --slave $R_padjust_fh> failed: $?";
}

# Read in R output
@time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... processing R output\n" if ($verbose);

open RES_PLUS, "< $R_pval_output_plus_fh" or die "can t read $R_pval_output_plus_fh\n";
while(<RES_PLUS>){
  chomp;
  s/"//g;
  s/NA/1/;
  
  my @F=split "\t", $_;
  
  $TSS_plus[$F[0]]=[$F[1], $F[2]-$F[3], $F[2]];
}
close RES_PLUS;

open RES_MINUS, "< $R_pval_output_minus_fh" or die "can t read $R_pval_output_minus_fh\n";
while(<RES_MINUS>){
  chomp;
  s/"//g;
  s/NA/1/;
  my @F=split"\t", $_;
  
  $TSS_minus[$F[0]]=[$F[1], $F[2]-$F[3], $F[2]];
}
close RES_MINUS;

# output the annotated TSS in bed format
@time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... writing output BED file\n" if ($verbose);

my $tss_count=0;
my $TSS_output_fh = "$ctdir\\TSS_$time[0]$time[1]$time[2].bed";

open TSSOUT, "> $TSS_output_fh" or die "can t write to file $TSS_output_fh\n";
my $index=1;
foreach my $pos (1 .. $genome_size) {
  if (defined($TSS_plus[$pos]->[0]) && !($TSS_plus[$pos]->[0]=~m/NA/i) && ($TSS_plus[$pos]->[0] <= $pval_cutoff) && ($TSS_plus[$pos]->[2] >= $min_peak_size) && ($TSS_plus[$pos]->[1] > 0)) {
    printf TSSOUT "%s\t%d\t%d\t%s\t%e\t%s\n", $chr_name, $pos-1, $pos, 'TSS_'.sprintf("%05d", $index), $TSS_plus[$pos]->[0], '+' if ($score_mode eq 'p'); # p-value is score
    printf TSSOUT "%s\t%d\t%d\t%s\t%f\t%s\n", $chr_name, $pos-1, $pos, 'TSS_'.sprintf("%05d", $index), $TSS_plus[$pos]->[1], '+' if ($score_mode eq 'd'); # Peak difference is score
    $index++;
    $tss_count++;
  }
  if (defined($TSS_minus[$pos]->[0]) && !($TSS_minus[$pos]->[0]=~m/NA/i) && ($TSS_minus[$pos]->[0] <= $pval_cutoff) && ($TSS_minus[$pos]->[2] >= $min_peak_size) && ($TSS_minus[$pos]->[1] > 0)) {
    printf TSSOUT "%s\t%d\t%d\t%s\t%e\t%s\n", $chr_name, $pos-1, $pos, 'TSS_'.sprintf("%05d", $index), $TSS_minus[$pos]->[0], '-' if ($score_mode eq 'p'); # p-value is score
    printf TSSOUT "%s\t%d\t%d\t%s\t%f\t%s\n", $chr_name, $pos-1, $pos, 'TSS_'.sprintf("%05d", $index), $TSS_minus[$pos]->[1], '-' if ($score_mode eq 'd'); # Peak difference is score
    $index++;
    $tss_count++;
  }
}

# clean working directory
if ($clean) {
  my $Rout_fh = basename($Rscript_fh).'out';
  system("del $Rout_fh $library_P_1 $library_P_0 $library_M_1 $library_M_0 $Rscript_fh $R_pval_output_plus_fh $R_pval_output_minus_fh");

  
}
else {
  system("copy $Rscript_fh ./Rscript.R");
  system("copy $TSS_output_fh TSS_unclustered.bed");
  my $Rout_fh = basename($Rscript_fh).'out';
  system("copy $Rout_fh ./Rscript.Rout");
  system("copy $library_P_1 coverage_P1");
  system("copy $library_P_0 coverage_P0");
  system("copy $library_M_1 coverage_M1");
  system("copy $library_M_0 coverage_M0");#
  #system("rm -f $library_P_1 $library_P_0 $library_M_1 $library_M_0 $R_pval_output_plus_fh $R_pval_output_minus_fh");
  
}


#cluster annotated TSS if $cluster is set

@time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), " ..... beginning clustering of consecutive TSS positions\n" if ($verbose);

my $tss_c_count=0;
$tss_c_count=$tss_count unless ($cluster);

if ($cluster) {
  my (%TSS_PLUS, %TSS_MINUS);
  my (@coverage_plus, @coverage_minus);
  
  # read in TSS in bed formate
  open TSSIN, "< $TSS_output_fh" or die "can t read file $TSS_output_fh\n";
  while(<TSSIN>){
    chomp;
    next if(m/^\s*$/);
    my @F=split "\t" ,$_;

    if ($F[5] eq '+'){
      @{$TSS_PLUS{$F[2]}}=@F;
      $coverage_plus[$F[2]]=$F[4];
    }
    elsif($F[5] eq '-') {
      @{$TSS_MINUS{$F[2]}}=@F;
      $coverage_minus[$F[2]]=$F[4];
    }
    else {
      die " column 6 in file $TSS_output_fh does not contain a proper strand flag ('-' or '+') but '$F[5]\n";
    }
  }
  close TSSIN;
  #system("rm -f $TSS_output_fh");
  
  # write clustered TSS into STDOUT
  # cluster plus strand
  my @peak=();
  my @pos=();
  
  foreach my $pos (0 .. $#coverage_plus) {
    if ( defined($coverage_plus[$pos]) ) {
      push @peak, $coverage_plus[$pos];
      push @pos, $pos;
    }
    elsif ( exists($peak[0]) ){
      next if ( ($pos-$pos[-1]) <= $consecutiv_range );
      my $min_pos = '';
      $min_pos = &min_pos_d(@peak) if($score_mode eq 'd');
      $min_pos = &min_pos_p(@peak) if($score_mode eq 'p');
      
      print join("\t", @{$TSS_PLUS{$pos[$min_pos]}})."\n";
      $tss_c_count++;
      @peak=();
      @pos=();
    }
    else {
      next;
    }
  }
  if ( exists($peak[0]) ){
    my $min_pos = '';
    $min_pos = &min_pos_d(@peak) if($score_mode eq 'd');
    $min_pos = &min_pos_p(@peak) if($score_mode eq 'p');
    
    print join("\t", @{$TSS_PLUS{$pos[$min_pos]}})."\n";
    $tss_c_count++;
    @peak=();
    @pos=();
  }
  
  #cluster minus strand
  @peak=();
  @pos=();
  foreach my $pos (0 .. $#coverage_minus) {
    if ( defined($coverage_minus[$pos]) ) {
      push @peak, $coverage_minus[$pos];
      push @pos, $pos;
    }
    elsif ( exists($peak[0]) ){
      next if ( ($pos-$pos[-1]) <= $consecutiv_range );
      my $min_pos = '';
      $min_pos = &min_pos_d(@peak) if($score_mode eq 'd');
      $min_pos = &min_pos_p(@peak) if($score_mode eq 'p');
      
      print join("\t", @{$TSS_MINUS{$pos[$min_pos]}})."\n";
      $tss_c_count++;
      @peak=();
      @pos=();
    }
    else {
      next;
    }
  }
  if ( exists($peak[0]) ){
    my $min_pos = '';
    $min_pos = &min_pos_d(@peak) if($score_mode eq 'd');
    $min_pos = &min_pos_p(@peak) if($score_mode eq 'p');
    
    print join("\t", @{$TSS_MINUS{$pos[$min_pos]}})."\n";
    $tss_c_count++;
    @peak=();
    @pos=();
  }
}
else {
  system ("type $TSS_output_fh") == 0 or die "can t read/write $TSS_output_fh: $?\n";
  #system("rm -f $TSS_output_fh");
}

# Read in R output (Region not seen)
open RES_dump, "< $R_SeenRegion_fh" or die "can t read file $R_SeenRegion_fh\n";
my (@Seen_Plus, @Seen_Minus)=();
while(<RES_dump>){
  chomp;
  next if ($.==1);
  
  s/"//g;
  
  my @F=split;
  
  $Seen_Plus[$F[0]]=$F[1];
  $Seen_Minus[$F[0]]=$F[2];
}
close RES_dump;

my ($dump_length, $dump_count)=(0,0);
{
  open DUMP, ">Dump.bed" or die "can t write to file Dump.bed\n";
  my ($start_p, $start_m)=(1,1);
  my ($end_p, $end_m)=(1,1);
  my ($status_p, $status_m)=(0,0);
  my ($dump_id, $dump_id_index)=(1, 1);
  
  foreach my $pos (1 .. $#Seen_Plus) {
    if ($Seen_Plus[$pos] eq 'NA') {
      if ($status_p) {
        $end_p=$pos;
      }
      else {
        $start_p=$pos-1;
        $status_p=1;
      }
    }
    elsif ($Seen_Plus[$pos] == 1) {
      if ($status_p) {
        $dump_id="DumpRegion_".sprintf("%04d",$dump_id_index++);
        print DUMP "$chr_name\t$start_p\t$end_p\t$dump_id\t.\t+\n";
        $dump_length+=($end_p-$start_p);
        $dump_count++;
        $end_p=$pos;
        $start_p=$pos-1;
        $status_p=0;
      }
    }
  }
  
  foreach my $pos (1 .. $#Seen_Minus) {
    if ($Seen_Minus[$pos] eq 'NA') {
      if ($status_m) {
        $end_m=$pos;
      }
      else {
        $start_m=$pos-1;
        $status_m=1;
      }
    }
    elsif ($Seen_Minus[$pos] == 1) {
      if ($status_m) {
        $dump_id="DumpRegion_".sprintf("%04d",$dump_id_index++);
        print DUMP "$chr_name\t$start_m\t$end_m\t$dump_id\t.\t-\n";
        $dump_length+=($end_m-$start_m);
        $dump_count++;
        $end_m=$pos;
        $start_m=$pos-1;
        $status_m=0;
      }
    }
  }

  @time=localtime();
print STDERR join(":", sprintf("%02d", $time[2]), sprintf("%02d", $time[1]), sprintf("%02d", $time[0])), "  ==>  TSSAR completed\n" if ($verbose);
}

#Print Report
if ($verbose){
  printf STDERR "\n\n  XXXXXXXXXXXXX\n  XX Report: XX\n  XXXXXXXXXXXXX\n%d individual TSS were annotated (p-Value cut off %.1g). %d remained after consecutive TSS were merged together\nIn total %d region(s) (with a total length of %d nt [%.3g %% of the genome]) could not be modeled with our zero-inflated Poisson regression (see Dump.bed for details; manual inspection is advised)\n", $tss_count, $pval_cutoff, $tss_c_count, $dump_count, $dump_length, (100*$dump_length)/(2*$genome_size);
}


##############
# subroutins #
##############

sub min_pos_d {
  my @arr = @_;
  my $min_pos=0;
  my $min_dist=1;
  
  #print "@arr\n";
  foreach my $pos (0 .. $#arr) {
    if ($arr[$pos]>$min_dist) {
      $min_dist=$arr[$pos];
      $min_pos=$pos;
    }
  }
  return $min_pos;
}

sub min_pos_p {
  my @arr = @_;
  my $min_pos=0;
  my $min_pval=1;
  
  foreach my $pos (0 .. $#arr) {
    if ($arr[$pos]<$min_pval) {
      $min_pval=$arr[$pos];
      $min_pos=$pos;
    }
  }
  return $min_pos;
}

sub cigarlength {
  my $cigar_string = shift;
  my $cigar_length = 0;

  while($cigar_string=~m/(\d+)[MDX=]/g){
    $cigar_length+=$1;
  }
  
  if($cigar_length == 0) {
    print STDERR "in line $. CIGAR string <'$cigar_string'> seems corrupt;\n";
  }
  return($cigar_length);
}

##############
## man page ##
##############

=pod
    
=head1 NAME
    
TSSAR
    
=head1 DESCRIPTION

B<I<T>>ranscription B<I<S>>tart B<I<S>>ites B<I<A>>nnotation B<I<R>>egime for dRNA-seq data, based on a Skellam distribution with parameter estimation by zero-inflated-poisson model regression analysis. The input are two mapped sequencing files in SAM file formate (library[+] and library[-]), the output is a *.BED file with an entry for each position which is annotated as a TSS, writen to STDOUT. Addtionally, a file named Dump.bed is created. It specifies regions where the applied regression model does not converge. Hence, those regions are omitted from analysis.

=head1 SYNOPSIS

./TSSAR --libP I<libraryP.sam> --libM I<libraryM.sam>  [--score I<p|d>] [--fasta I<genome.fa> --g_size I<INT>] [--minPeak I<INT>] [--pval I<FLOAT>] [--winSize I<INT>] [--verbose] [--noclean] [--nocluster] [-range I<INT>]] [<--tmpdir> I<DIR>] [--help|?] [--man]

=head1 OPTIONS

=over 4

=item B<--libP> I<libraryP.sam> AND B<--libM> I<libraryM.sam>
    
Input library (P .. Plus; M .. Minus) in SAM format. The plus library is the one with enriched TSS (for dRNA-seq this means that the plus library is the treated library, while the minus library is the untreated library)

=item B<--fasta> I<genome.fa> OR B<--g_size> I<INT>
    
Either the location of reference genome sequence in fasta file format OR the genome size in I<INT>. The fasta file is only used to parse the genome size so just one of the two must be specified.

=item B<--minPeak> I<INT>
    
Minimal Peak size in I<INT>. Only positions where read start count in the (+)library is greater or equal then I<INT> are evaluated to be a TSS. Positions with less reads are seen as backgroound noise and not considered. Default is I<3>.

=item B<--pval> I<FLOAT>
    
Maximal P-value for each position to be annotated as a TSS. Default is I<1e-04>.

=item B<--winSize> I<INT>
    
Size of the window which slides over the genome and defines the statistical properties of the local model. Default is I<1,000>.

=item B<--verbose>
    
If set, some progress reports are printed to STDERR during computation.

=item B<--prorata>

If set, the information from the SAM file how many times a read was mapped to the genome is used, if present. If the read maps I<n> times to the genome, each position is counted only I<1/n> times. Usefull in combination with e.g. segemehl mapper, which can report suboptimal mapping positions and/or reports all location where a read maps optimally. Default is off.

=item B<--score> I<p|d>

If score mode is I<p> the p-value is used as score in the TSS BED file. If score mode is I<d> the peak difference is used as score in the TSS BED file. Default is I<d>. Also used for clustering, which advices to use 'd', since the p-value often becomes zero for consecutive positions, thus disabling a proper merging of consecutive positions to the best one.

=item B<--nocluster> | B<--cluster>

If B<--nocluster> is set all positions annotated as TSS are reported. If B<--cluster> is set consecutive TSS positions are clustered and only the 'best' position is reported. 'Best' position depends on the setting of B<--score> (see above). Either the position with the lowest p-Value or the position with the highest peak difference between plus and minus library is reported. Default is B<--cluster>. The option B<--range> defines the maximal distance for two significant positions to be called 'consecutive'.

=item B<--range> I<INT>

The maximal distance for two significant positions to be be clustered together if option B<--cluster> is set. Default is I<3> nt. If B<--cluster> is set to B<--nocluster>, B<--range> is ignored.

=item B<--clean> | B<--noclean>
    
If B<--clean> is set, all temporary files which are created during the computation are deleted afterwards. With B<--noclean> they are stored. Mainly for debugging purpose. Default setting is B<--clean>.

=item B<--tmpdir> I<DIR>

Specifies where the temporary files should be stored. Default is I</tmp>.

=item B<--man>

Print a long version of the man-page.

=item B<--[help|?]>

Print a short version of the man-page.

=back

=head1 CONSIDERATIONS
    
This is only a beta-version which was not thoroughly tested.

=head1 VERSION

Version 0.9.6 beta B<--> Distribution is modeled locally, by assuming a mixed model between

Poisson-Part -> Transcribed Region (sampling zeros)

Zero-Part    -> Not Transcribed Region (structural zeros)

The Poisson-Part is seperated from the Zero-Part by Zero-Inflated-Poisson-Model Regression Analysis. The Parameters for Skellam is the winzorized mean over the Poisson-Part.

=head1 AUTHOR
    
Fabian Amman, afabian@bioinf.uni-leipzig.de

=head1 LICENCE

TSSAR itself comes under GNU General Public License v2.0

Please note that TSSAR uses the R libraries Skellam and VGAM. Both libraries are not our property and might have altering licencing. Please cite independantly.
    
=cut
    
