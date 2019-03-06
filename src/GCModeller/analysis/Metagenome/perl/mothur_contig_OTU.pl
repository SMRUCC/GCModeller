#!/bin/perl

# use warnings;
use File::Basename;
use File::Copy qw(copy);
use File::Slurp qw(read_file write_file);

# 当$debug变量的值不为零的时候，表示处于调试模式，则这个时候脚本只会输出命令行，
# 而不会发生命令行的实际执行行为
my $debug = 0; 

# 使用Mothur程序的命令行模式进行paired end测序数据的拼接生成OTU contig
# 这个脚本应该是在切换进入目标数据文件夹之后再进行调用的

# 可以根据服务器的内存的大小来修改线程数量，
# 内存越大能够使用到的线程的数量就会越多
my $num_threads = 8;
# 设置mothur的文件夹位置
my $mothur_base = "/home/Mothur.linux_64";
my $data_base = "/mnt/ntfs"; 

# 237生产部署
my $mothur      = "$mothur_base/mothur";
my $silva       = "$data_base/silva.bacteria/silva.bacteria.fasta";
my $greengene   = "$data_base/greengenes/99_otus.fasta";
my $blastn      = "/home/ncbi-blast-2.8.1+/bin/blastn";

# biodeep测试用
# my $mothur      = "/home/16s/mothur/mothur"; 
# my $silva       = "/home/16s/silva.bacteria.fasta";

my $num_args    = $#ARGV + 1;
my $left        = NULL;
my $right       = NULL;

if ($num_args == 0) {

	# 没有输入任何命令行参数，则打印出脚本的用法帮助
	#
	# mothur_contig.pl left.fq right.fq workspace
	
	print "\n";
	print "  Usage:\n";
	print "\n";
	print "     mothur_contig.pl left.fq right.fq /num_threads=8\n";
	print "\n";
	print "  Where:";
	print "\n";
	print "\n";
	print "     1. left.fq and right.fq is the paired-end short reads file.\n";
	print "     2. /num_threads is an optional argument for setting up the\n";
	print "        processors that will be used in mothur program, by default\n";
	print "        is using 8 processor threads.\n";
	print "\n";
	
	exit 0;
	
} elsif ($num_args >= 2) {
	
	$left    = $ARGV[0];
	$right   = $ARGV[1];

	# 还有可能带着一个线程数量的设置参数
	if ($num_args >= 3) {
		my @param    = split("=", $ARGV[2]);
		$num_threads = $param[1];
	}
}

sub runMothur {

	my ($args, $log) = @_;
	my  $CLI         = "$mothur \"#$args;\" > $log";
	
	print "\n$CLI\n\n";
	
	if ($debug == 0) {
		system($CLI);
	}
}

# The 3 column format is used for datasets where the sequences have already had 
# the barcodes and primers removed and been split into separate files. The first 
# column is the group, the second is the forward fastq and the third column 
# contains the reverse fastq.
open(stability, ">./16s.files") or die "Unable to write data file for mothur!";
print stability "16s\t$left\t$right";
close stability;

runMothur("make.contigs(file=16s.files, processors=$num_threads)", "[1]make.contigs.txt");
# -rw-r--r-- 1 root root  3256536 Dec 10 17:24 16s.contigs.groups
# -rw-r--r-- 1 root root  4341698 Dec 10 17:24 16s.contigs.report
# -rw-r--r-- 1 root root       49 Dec 10 17:28 16s.files
# -rw-r--r-- 1 root root        0 Dec 10 17:23 16s.scrap.contigs.fasta
# -rw-r--r-- 1 root root        0 Dec 10 17:23 16s.scrap.contigs.qual
# -rw-r--r-- 1 root root 31614742 Dec 10 17:24 16s.trim.contigs.fasta
# -rw-r--r-- 1 root root 86838453 Dec 10 17:24 16s.trim.contigs.qual

my $contigs = "contig.fasta";
my $groups  = "16s.contigs.groups";

# 为了方便代码的编写，下面对生成的文件会进行一些重命名
# 经过重命名之后，后面的代码操作都可以在这个重命名的文件名基础上不需要做太多修改了
sub write_contig {

	my ($seq) = @_;

	if (-e "contig.fasta") {
		unlink "contig.fasta";
	}

	copy   $seq,  "contig.fasta";
	print "$seq => contig.fasta\n";
}

# 对输出的文件进行重命名
write_contig("16s.trim.contigs.fasta");

# RunAutoScreen
runMothur("summary.seqs(fasta=$contigs,processors=$num_threads)", "[2]summary.seqs.txt");

# 读取summary输出文件，获取需要保留下来的长度范围
my $file = '[2]summary.seqs.txt';
my $min  = 0;
my $max  = 0;

open my $info, $file or die "Could not open $file: $!";

while(my $line = <$info>)  {   
    my @cols   = split("\t", $line);
    my $header = $cols[0];

	if ($header eq "2.5%-tile:") {
		$min   = $cols[3];
	} elsif ($header eq "97.5%-tile:") {
		$max   = $cols[3];
	}
}

close $info;

runMothur("screen.seqs(fasta=$contigs,group=$groups,maxambig=0, minlength=$min, maxlength=$max)", "[3]screen.seqs.txt");
# contig.good.fasta
# contig.bad.accnos
# 16s.contigs.good.groups

$contigs = "contig.good.fasta";
$groups  = "16s.contigs.good.groups";

runMothur("unique.seqs(fasta=$contigs)", "[4]unique.seqs.txt");
# contig.good.names
# contig.good.unique.fasta

my $names = "contig.good.names"; 
runMothur("count.seqs(name=$names, group=$groups)", "[5]count.seqs.txt");
# contig.good.count_table

my $count_table = "contig.good.count_table";
runMothur("summary.seqs(fasta=contig.good.unique.fasta, count=$count_table)", "[6]summary.seqs.txt");
# contig.good.unique.summary

write_contig("contig.good.unique.fasta");

$contigs = "contig.fasta";

runMothur("align.seqs(fasta=$contigs,reference=$silva,flip=T,processors=$num_threads)","[7]align.seqs.txt");
# contig.align
# contig.align.report
# contig.flip.accnos

my $align = "contig.align";
runMothur("filter.seqs(fasta=$align,processors=$num_threads)", "[8]filter.seqs.txt");
# contig.filter
# contig.filter.fasta

write_contig("contig.filter.fasta");
runMothur("unique.seqs(fasta=$contigs)", "[9]unique.seqs.txt");
# contig.names
# contig.unique.fasta

$align = "contig.unique.fasta";
runMothur("dist.seqs(fasta=$align,calc=onegap,countends=F,cutoff=0.03,output=lt,processors=$num_threads)", "[10]dist.seqs.txt");
# contig.unique.phylip.dist

my $dist = "contig.unique.phylip.dist";
runMothur("cluster(phylip=$dist,method=furthest,cutoff=0.03,processors=$num_threads)", "[11]cluster.txt");

# contig.unique.phylip.fn.sabund
# contig.unique.phylip.fn.rabund
# contig.unique.phylip.fn.list

my $list = "contig.unique.phylip.fn.list";
runMothur("bin.seqs(list=$list,fasta=$contigs,name=contig.names)", "[12]bin.seqs.txt");

# contig.unique.phylip.fn.unique.fasta
# contig.unique.phylip.fn.0.01.fasta
# contig.unique.phylip.fn.0.02.fasta
# contig.unique.phylip.fn.0.03.fasta
runMothur("get.oturep(phylip=$dist,fasta=contig.unique.fasta,list=$list,label=0.03)", "[13]get.oturep.txt");

print "Mothur job done!\n";

# 在这里进行SILVA的16S数据库的比对操作，进行OTU序列所属的物种鉴定
# 首先需要将OTU的fasta文件之中由于前面的mothur程序align的空格和连接符都删除掉
# 否则blastn程序会报错
my $fasta   = "./contig.unique.phylip.fn.0.03.rep.fasta";
my $data    = read_file $fasta, {binmode => ':utf8'};
my $OTU_rep = "./OTU.rep.fasta"; 

$data =~ s/[.-]//g;
write_file $OTU_rep, {binmode => ':utf8'}, $data;

# 进行blastn序列比对操作来完成物种鉴定
$CLI = "$blastn -query $OTU_rep -db $greengene -out ./OTU_greengene_99.txt -evalue 1e-50 -num_threads $num_threads";
print $CLI."\n";
system($CLI);

print "greengenes OTU Taxonomy align job done!\n";