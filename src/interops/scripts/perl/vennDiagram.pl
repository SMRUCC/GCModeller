use File::Basename qw(fileparse);
use File::Copy;

# ------------------------------------------------------------------
#
# Steps for Create a venn diagram from other tools
#
# script_by:  xie.guigang@gmail.com
# time_date:  #2014/05/16 12:10:35#
#
# ------------------------------------------------------------------


# ======================variable_definitions==========================
# All you needs to do just modify these variables below:
#

# The executable file path of the venn.exe program.
my $venn="venn.exe";
# The temp data directory for the venn program.
my $Outdir="..\\out";
# The data input folder
my $originalInput="..\\besthit_original";

my @array=(
     ["8004e_vs_ecoli_published_ess.csv","8004e_vs_pa14_published_ess.csv","8004e_vs_ftn_published_ess.csv","8004e_vs_acaid_published_ess.csv"],
	 ["ecoli_vs_pa14_published_ess.csv","ecoli_vs_ftn_published_ess.csv","ecoli_vs_aciad_published_ess.csv"],
	 ["pa14_vs_ftn_published_ess.csv","pa14_vs_aciad_published_ess.csv"],
	 ["ftn_vs_aciad_published_ess.csv"]
   );
my $last_index="acaid";
my $originalIndex=0;
my $index_files="..\\index_files";

#venn_diagram_drawing
my $diagram_title="xan8004_genome_compares";
my $diagram_save_path="..\\venn.tiff";
my $rBin_directory="C:\\Program Files\\R\\R-3.1.0\\bin";
my $serials_option_pairs="xcc8004,blue;ecoli,green;pa14,yellow;ftn,black;aciad,red";
# ====================================================================



# ===========================================================================================================
# 1. Generate the csv file from other tool and then create the bi_direction besthit data file
#
# merge command is using for generate the venn drawing data which was created from other tools. The input file 
# format is a csv excel file, you can generate this file from the excel program.
#
# ===========================================================================================================
my @csv_files=glob($originalInput."\\*.csv");

$venn="\"".$venn."\"";

mkdir $Outdir;

foreach $file (@csv_files) {
  # get file information
  my ($fileName,$dir)=fileparse($file);
  print "Proceeding file: \"".$fileName."\"......\n";
  
  my $output=$Outdir."\\".$fileName;
  # venn -export_besthit -i <input_csv_file> -o <output_saved_csv>
  my $cmdl=$venn." -export_besthit -i \"".$file."\" -o \"".$output."\"";
  print "  ---> EXEC(\"".$cmdl."\")\n\n\n";
  
  system($cmdl);
}

my $Outdir_forMerged=$Outdir."\\forMerged";
mkdir $Outdir_forMerged;

for $i (0..$#array) {
  $row=$array[$i];
  
  my $str_fileList="";
  foreach $file (@{$row}) {
    $str_fileList.=$Outdir."\\".$file."|"
  }
  $strLength=length($str_fileList);
  $str_fileList=substr($str_fileList,0,$strLength-1);
  print "GET_FILE_LIST::  \"".$str_fileList."\"\n\n";
  
  my @tempArray=split(/_/,@{$row}[0]);
  my $bacterialName=$tempArray[0];
  print "---->  ".$bacterialName."\n\n";
  
  my $Outfile=$Outdir_forMerged."\\".$bacterialName.".csv";
  my $indexFile=$index_files."\\".$bacterialName.".csv";
  # -merge_besthit -i <input_file_list> -o <output_file> -os <original_idlist_sequence_file> [-osidx <id_column_index> -os_skip_first <T/F>]
  my $cmdl=$venn." -merge_besthit -i \"".$str_fileList."\" -o \"".$Outfile."\" -os \"".$indexFile."\" -osidx ".$originalIndex." -os_skip_first T";
  
  print "  ---> EXEC(\"".$cmdl."\")\n\n\n";
  
  system($cmdl);
}

my $indexFile=$index_files."\\".$last_index.".csv";
my $Outfile=$Outdir_forMerged."\\".$last_index.".csv";
my $cmdl=$venn." -copy -i \"".$indexFile."\" -os \"".$Outfile."\" -osidx ".$originalIndex." -os_skip_first T";
print "  ---> EXEC(\"".$cmdl."\")\n\n\n";

system ($cmdl);



# ===========================================================================================================
# 2. Using the merge command to generate the venn diagram drawing data.
# venn merge -d <directory> -o <output_file>
#
# ===========================================================================================================
my $mergedResult=$Outdir."\\final_merged\\for_venn_merged.csv";
$cmdl=$venn." merge -d \"".$Outdir_forMerged."\" -o \"".$mergedResult."\"";
print "  ---> EXEC(\"".$cmdl."\")\n\n\n";

system($cmdl);

my $for_checked=$Outdir."\\for_checked\\";
mkdir $for_checked;

for $i (0..$#array) {
  $row=$array[$i];
    
  my @tempArray=split(/_/,@{$row}[0]);
  my $bacterialName=$tempArray[0];
  print "----> CREATE_CHECK_MERGE_FOR:: ".$bacterialName."\n";
  
  my $for_checked_dir=$for_checked."\\".$bacterialName;
  mkdir $for_checked_dir;
    
  my $Sourcefile=$Outdir_forMerged."\\".$bacterialName.".csv";
  
  copy($Sourcefile,$for_checked_dir."\\temp.csv");
  
  my $for_checked_Data=$for_checked_dir."\\Target_merged.csv";
  $cmdl=$venn." merge -d \"".$for_checked_dir."\" -o \"".$for_checked_Data."\"";
  print "  ---> EXEC(\"".$cmdl."\")\n\n\n";

  system($cmdl);
}

print "----> CREATE_CHECK_MERGE_FOR:: ".$last_index."\n";
  
my $for_checked_dir=$for_checked."\\".$last_index;
mkdir $for_checked_dir;
    
my $Sourcefile=$Outdir_forMerged."\\".$last_index.".csv";
  
copy($Sourcefile,$for_checked_dir."\\temp.csv");
  
my $for_checked_Data=$for_checked_dir."\\Target_merged.csv";
$cmdl=$venn." merge -d \"".$for_checked_dir."\" -o \"".$for_checked_Data."\"";
print "  ---> EXEC(\"".$cmdl."\")\n\n\n";

system($cmdl);


# ===========================================================================================================
# 3. Drawing venn diagram
# venn venn_diagram -i <csv_file> [-t <diagram_title> -o <_diagram_saved_path> -s <serials_option_pairs> -rbin <r_bin_directory>]
#
# ===========================================================================================================
print "The output data \"".$mergedResult."\" maybe contains some error, and it need you to edit the data manually, when you have finish edit the data and then press ENTER to continute...";
my $nothingtodo=<>;

$cmdl=$venn." venn_diagram -i \"".$mergedResult."\" -t \"".$diagram_title."\" -o \"".$diagram_save_path."\" -s \"".$serials_option_pairs."\" -rbin \"".$rBin_directory."\"";
print "  ---> EXEC(\"".$cmdl."\")\n\n\n";

system($cmdl);
