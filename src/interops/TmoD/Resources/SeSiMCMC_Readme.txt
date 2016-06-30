******************************************************************************
SeSiMCMC. Looking - for - motifs by MCMC project. (c) A. Favorov 2001-2007
Last change: $Date: 2009-01-03 01:45:23 $
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

****************************************************************************
Synopsis
****************************************************************************
SeSiMCMC [options] [<FastA]

The program reads a set of sequence fragments in the FastA format from stdin, writes log to stderr, writes the result to stdout. We use one additional symbol in FastA standard. It is 'x' (or 'X') which means "the position cannot occur inside a motif site". 

****************************************************************************
How does it work.
****************************************************************************

It starts from a collection of DNA sequences the majority of which are supposed to contain a protein binding site (or other characteristic segment). These segments carry the same signal and therefore are instances of the same motif. The objective is to classify all DNA sequence data into motif instances and the remaining background in an optimal way. Here we present a modification that is inspired by extensive practice of analysis and prediction of gene co-regulation in prokaryotes. Particularly, a signal recognized by a prokaryotic transcription factor, exhibits a structure of an inverted or direct repeat. Therefore, we included user-defined symmetry in the probabilistic motif model. A motif, either symmetric or not, can be spaced, i.e. it can contain some unimportant positions in the middle. One usually does not know in advance the motif length as well as the spacer length. So, the program estimates the optimal values for these two lengths during motif detection. The training set may erroneously contain biologically irrelevant sequences, which do not contain. To account for this possibility, we explicitly enhanced the core probabilistic model with the expectation of a motif absence in a sequence. Generally we intended to create a specialized tool for searching weak structured motifs with spacers of unknown length. To this end we designed both the probabilistic model and the optimization procedure as modifications of the classic algorithm of.

Two probabilistic models, foreground (the motif) and background, are formulated. The optimal classification is the one most probable in the Bayesian sense. The motif is represented by a positional probability matrix (PPM); the background is the modelled by independent letters with fixed probabilities. 

We maximize the posterior of the given foreground-background division of the DNA sequence data as a function of the site positions in the sequences. Such a function may have many local maxima, so the Markov Chain Monte-Carlo (MCMC) technique is a natural algorithm for its optimisation. The suggested algorithm, which is implemented in the SeSiMCMC software, also determines the length of the motif. The user can specify direct repeat or palindromic (two complementary boxes) motif structure possibly with a space of unknown length in the middle. Occurrences of non-symmetrical motifs can be searched for on one or both complementary DNA strands.

The core procedure for the selection of the best (or almost the best) set of sites is as follows. We organise a cycle of one-by-one site positions updates. At each step, we select only one sequence. For representation uniformity, we treat a site absence as a specific type ("null") of position. At each step, we collect the nucleotide statistics for the internal site positions and for the background from all the sequences except one, which being updated at this step. We estimate the positional nucleotide and the background probabilities. For each selected sequence, the probability to obtain this sequence from a Bernoulli process (i.e. the site position likelihood) is obtained from these two models. Combining priors with the likelihoods in the usual Bayesian way, we obtain the posterior distribution for a site position in the current sequence and draw the new site position (possibly the "null" one) from the distribution. The process is repeated cyclically until the chain of site sets converges (i.e. the step-to-step changes become small). The algorithm is similar to that described in (Lawrence et al, 1993), but we process the possibility of a site absence is considered in the Bayesian way at each updating step.

In fact, the algorithm optimises the self-consistence of the set of site positions, so it is very sensitive to changes in mutual location of sites, but it is quite tolerant for all-as-one shifts of the site position set. To solve this problem, we adjust the results from time to time after the core algorithm converges to a sufficient degree, and then restart the core. The adjustment is a deterministic search for the best solution among all possible cooperative shifts of the local alignment of sites.

The best set of the sites on the adjustment stage is defined by the highest information content per site letter (ICL). The information content is the sum of two components: the structural one and the spatial one. Both are the Kullbak entropy distances. The former (structural) component is the distance between the probability model for the nucleotide occurrence inside the motif and the background one. The latter (spatial) component is the distance between the distribution of the site position posterior in a sequence (including the "null" position) given the known set of sites and the prior site position distribution.

In fact, it is sufficient to maximize the structure component to find the best set among all the shifts. But as optimal motif length is not known in advance, it is estimated using the spatial component. We fix the mutual sites positions (MLA) from the outcome with the maximal ICL obtained from the preceding sampling chain and then vary the site length and the absolute position of the entire set in order to optimise the value of ICL as the sum of the two components.

This adjustment procedure is similar to the one described in (Lawrence, 1993) with the following differences. The information content calculation has an improved spatial component. The site length is evaluated at the adjustment stage. Moreover, the same procedure is used to determine whether the motif is spaced. We assume that some middle columns of MLA may be not correlated, and in this case the profile has a symmetric spacer, which corresponds to the background probabilistic model. So, we extend the spacer for every cooperative shift of sites during the adjustment until we obtain the local maximum of the ICL) for that shift.

The program work consist of two stages, the annealing and the looking for maximum. On the annealing, the program moves from random initial state to a state not far away from the optimal. The adjustments do not change the motif length. The annealing is over when the changes of the sites set from decreases to a perfect rate. We use two parameters to test it - a number of local sequential local step cycles to result in little changes and the critical changes level. On annealing, the motif length is locked and every sequence is supposed to carry a site. To save time, we can avoid to make adjustments on annealing. The looking for maximum (second stage) start releases the restrictions. We treat a maximum as global if it is not overcome after a definite steps number. Sometimes, it is convenient to describe the number by its ratio to the annealing steps number. Another necessary condition of globalisation of a maximum is that a non-forbidden (see below) adjustment to happen after the maximum before it is stated as global.

Before every adjustment, we make the the same test as for annealing finish and forbid the adjustment if the test fails. It prevents us from shifting to random-states-space of short motifs by trying to adjust a "hot" sampler. When shift adjustments during annealing are permitted, we let them to happen instead of forbidden length and position adjustment. Still, the "forbidden adjustment" do not permit program to state a maximum as global and so to finish the task. 

If we find a chain of variable motif length states forbidding more than a definite number of adjacent adjustments, we treat the chain as failed and return to the state before the last permitted adjustment. The typical time of chain failure is three times the number of adjustments which would make a maximum global. If say 3 (a given number) chains fail, the task fails, too.
When it happens, we state that the solution is not reliable and take the last maximum before the last chain has sweeped down to chaotic state as the result.

Two protocols for the motif and the spacer length optimization can be used. The default "fast" mode performs the optimisation at the local alignment adjustment stage, as described above. In the "slow mode", the motif length is changed stepwise and the full sampling procedure without the length adjustment is executed at every step. The last variation is similar to that described in (Lawrence, 1993) and is rather slow. It provides more information about the possible MLA.

****************************************************************************
The program input and output.
****************************************************************************

The program reads a standard FastA sequence file from stdin. It outputs a log of its work to stderr if it is not suppressed by program switches. The result is written to stdout in a human-readable format, either html or plain text (see --html option). Also, the SmallBisMark (http://bioinform.genetika.ru/smallbismark.dtd) output can be requsted.

The program marks good columns in the motif by caps. A good column is a one where the information content is higher than any 3-letter mixture from the background.

An additional letter in FastA, 'x' is used so that no motif site can contain this letter. This mask can be put either manually or by a previous SeSiMCMC run on the same gene data. It masks all found motifs and can output the FastA file with masked fragment in order to look for other motifs on the next run.

****************************************************************************
Representation agreements.
****************************************************************************

All positions and sequence numbers are 0-based.

The position of a word if the sequence is read in 5'-3' direction is the position of 5' (start) point of the word. If the sequence is read as complement, the word position is the position of its 3' (the last) letter.

5' ----------|*********----------> 3'
3' <---------|*********----------  5'


****************************************************************************
Command line switches and configuration file tags.
****************************************************************************

All command-line switches can be omitted and the default values work in most cases, but sometimes it make sense to change the default behavior.

If one option can be set by command-line switch and by configuration file line, the command-line switch has priority.

If two switches are contradictory, an error is reported and the program stops.

If two or more configuration file lines are contradictory, only the last one works.





1. Command-line switches which define the general mode of the work of the program. Only these switches lack corresponding configuration file tags.


--help 
-h 

Help. Gives a text screen with a reference to this file.


--version

Prints version information.


--fake 

Use fake data. We do not read FastA stdin but explore an internally generated set of 20 sequences of length 120 with inserted word "atggccactt" to positions : 84 81 78 75 absent 69 66 63 60(compl) 50 47 44 41 38 35 32 29 26 23 20. Absent means that the 5-th sequence contains no site and (compl) means that the 8-th sequence contains a complementary site in position 60.


--config-file filename
-f file

The name of the configuration file. The format of lines in the is: "tag=value". A line starting with # is a comment. The tags are described later. Every tag has a corresponding command-line switch, and the command-line switches have priority over the configuration file tags. The order of the tags does not matter.


--write-config
-w

Does nothing but writes to stdout a configuration file describing the working mode for composition of all other switches, configuration file tags if one is given and the defaults.


--quiet
-q
-q+

Do not output the log file.


--noquiet
-q-

Do output the log even if the default is --quiet.

The default it --quiet if cgi output is required, --noquiet otherwise.


-i file
--input file
--input-file file

Input Fasta file. "-" is stdin. It is the default.


--time-limit unsigned
--time unsigned
-tl unsigned


Time limit in seconds. If exceeded, the program stops and shows the diagnostics. Default 1000.


2. Options which affect output.


-in name
--input-name name
--input-file-name name

The file name to be written to the output header. The default is "-i" parameter value.


--output-tables

Output the weight tables for the background and for the motif. It is the default if --cgi is not used.


--not-output-tables

Do not output the weight tables for the background and for the motif. It is the default if --cgi is used.


-cgi
--cgi

Implies --quiet and --output-tables as the default. 


--html

Writes a html-formatted output instead of plain text. Implies --quiet.


--log fname
--id string
--ip ip-string

Appends to fname (it is a common logfile) two lines "Task id was started from ip-string at time" and "Task with id was finished at time". Tries to open the file for 3 seconds, if fails, omits the common logfile stamping. (Do not have anything to do with verbose logging to stderr that is switched off by --quiet.)


--output-file ofname

Uses ofname instead of stdout. If the program is not able to open it, it stops.
Does not affect --help, --version, --write-config modes.
All diagnostics go to this file, too.


--output-fasta fastafilename

Outputs FastA file with found motif sites positions that are masked partially by 'x'.


--masked-part double

The part of found motif that will be masked in the FastA output. Default is 0.5.


What part of the found motif is masked, default is 0.5. The masked spot is more than zero and not more than the whole motif. The midpoint of the masked area is the same as of the site. The length of the masked part is odd or even as that of the motif.

--flanks-length
--flanks
-k

The length of flanks (non-motif parts) on the html output. The default is 5.

--show-spaced-bases
-ssb

If given, the spacers are shown on html output not as dots but by the grey color.
The behavior is default.

--not-show-spaced-bases
--hide-spaced-bases
-hsb

If given, the spacers are shown on html output like "."

--xml 

requests SmallBisMark (http://bioinform.genetika.ru/smallbismark.dtd) output instead of text or html to the output file.

Attention!!! Smallbismark requires 1-based locations of sites, so we do only for xml output.

--output-unreliable-result
--unrel
-unrel

Requires output of an unreliable motif (e.g. when all the chains has failed to converge).





3. Commonly used switches affecting the looking for motifs flow.


-1
--one-strand

-2
--two-strands

Config file line:
mode=one_strand|two_strands

The default is -2 (--two_strands).


--no-symmetry

-rp
-tr
--tandems
--repeats

-o
--palindromes

Config file line:
symmetry=no|palindromes|repeats

The default is --no-symmetry.
If symmetry=palindromes, -1 and -2 switches are ignored.


--absence-prior double
-p double

Config file line:
motif_absence_prior=double

It gives the prior for a sequence to be garbage. The default value depends on the data.


--adjust-length
--not-adjust-length
-a+
-a-

Config file line:
adjust_motif_length=boolean

The default is -a+ (yes).


--spacers
--spaced
--no-spacers
--not-spaced

Config file line:
spaced_motif=boolean


--adaptive_pseudocouts
--adap

--no-adaptive_pseudocouts
--nadap

Config file line:
adaptive_pseudocouts=boolean

If adaptive_pseudocouts is off, the sum of all pseudocounts is sqrt(#number od sequnces) during all the run. If it is on, the sum equals 1.5 during initial annealing. Default is off.

--common-background
--no-common-background

Config file line:
common_background=boolean

If set, the background is collected form all fragments not excluding the motifs themself. The default is off.

--A double
--T double
--G double
--C double
--background-A double
--background-T double
--background-G double
--background-C double

Config file lines 

background_A=double
background_T=double
background_G=double
background_C=double

Four background counters. They are to be >0. The background frequencies and pseudocount ratios are counted by their normalisation.



--length unsigned
-l unsigned

Config file line:
motif_length=unsigned

The motif length. If -a- is used it is the motif length, if -a+ it is the length to start with. The default value depends on data.


-l+ unsigned
--max-length unsigned
--maximal-length unsigned

Config file line:
maximal_motif_length=unsigned

The maximal motif length. If adjust_motif_length=off it is not used. The default value depends on the data (2/3 of the shortest sequence length).


-l- unsigned
--min-length unsigned
--minimal-length unsigned

Config file line:
minimal_motif_length=unsigned

The minimal motif length. If adjust_motif_length=off it is not used. The default value depends on the data (the least length for a motif to be significant,
we take 6).


-r+
-r-
--not-retrieve-other-sites
--retrieve-other-sites

Config file line:
retrieve_other_sites=boolean

-r unsigned(n)
-retrieve-other-sites-threshold unsigned 

Config file line:
retrieve_other_sites_threshold=unsigned

If n>0, the program scans all the sequences and extracts all sites that fit to the motif better than the n-th worst of the sites. This post-processing procedure is optional, and if this option is not selected, only the best local alignment is output.

0 is equal to -r-. 1 is equal to +r+.

-rs
--retrieve-smart
--smart-retrieve-other-sites

Config file line:
retrieve_other_sites=smart

Retrieve additional sites in a "smart" way: for each sequence, we retrieve those sites that are not worse than than the one that was idetified in this sequence. In a siteless sequence, we do the same thing as for r+.

-rps
--retrieve-pure-smart
--pure-smart-retrieve-other-sites

Config file line:
retrieve_other_sites=pure_smart

Synonym to -rs -r0. In other words, we retrieve additional sites from the sequences that has sites and omit the others.


--slow-optimisation
-s

Config file line:
slow_optimisation=boolean

The default is "off".

It is explained in first part of the document.

It changes the default for "retrieve_other_sites" to "no".



--trim-edges
--trim

--not-trim
--not-trim-edges

Config file line:

trim_edges=boolean.

If trim_edges is true, the weak positions on the edges of the final site collection will be trimmed. If symmetry is given, it will be trimmed symmetricaly and the trimming will stop if at least one edge position is strong. A profile cannot be trimmed more than down to length (6+spacer).

There are two kinds of entities to be trimmed: outer weak positions and islands of strom postions that are isolated by more than 3 times longer weak position area.

The defalt is "off".



--dumb-trim-edges
--dumb-trim

--not-dumb-trim-edges
--not-dumb-trim


Config file line:

dumb_trim_edges=boolean.

If trim_edges is off or not given, the key switches on "dumb" trimming, i.e. a trimming that stops on the first informative position instead of cheking whether the position is an isolated "island". So, only outer weak positions are trimmed.

The switch is switched off by trim_edges=on


3a. Caps interpretation.


The default behaviour of the program is  to pay no attention to the input FastA data letter case. Yet, thare are a set of options that allows to "ahchor" the search by caps in input FastA.

Caps_mode can be simple: all, one or off. 
all means that only the sites that consist of caps are allowed. 
one means that only the sites that contain at least one cap are allowed. 
off means that we do not care about caps (default behavour).

Also, caps_mode can be complex, e.g. cam be formed as: simple_mode1-simple_mode2-simple_mode3, i.e. all-one-off. Simple_mode1 is for initial choice of sites, simple_mode2 is for annealing and simple_mode3 is the final stages of motif search. In this case, initial is to be more or equal restrictive than annealing mode and the annealing is to be more or equal restrictive than refinement mode.

A simple mode "mode" is equal to the complex mode "mode-mode-mode".   

--caps caps_mode
--ca caps_mode

Config file line:
caps=caps_mode

Supported outdated synonyms:
--initial-input-caps, -iics, --iics, config file line: input_caps=initial 
are synonyms for the complex mode "one-off-off"

--annealing-input-caps, -aics, --aics, config file line: input_caps=annealing 
are synonyms for the complex mode "one-one-off"

--obligatory-input-caps, -oics, --oics, config file line: input_caps=obligatory 
are synonyms for the complex mode "one-one-one" and for the simple mode "one".

--neglect-input-caps, -nic, --nics, config lines: input_caps=neglect, input_caps=off, input_caps=on are synonyms for the complex mode "off-off-off" and for the simple mode "off".

It is the default behaviour.


--footprint
-fp 

= --caps one-one-one --common-background

--footprinta
-fpa

= --caps all-one-one --common-background




4. Advanced switches affecting the algorithm flow.


--random-seed-1 unsigned
--random-seed-2 unsigned
-seed1 unsigned
-seed2 unsigned

Config file line:
random_seed_1=unsigned
random_seed_2=unsigned

Seeds for the random number generator (we use the one by George Marsaglia and Arif Zaman). Our historical defaults are 1248312 and 7436111.


--local-cycles unsigned
-e unsigned

Config file line:
local_step_cycles_between_adjustments=unsigned

How many local step cycles are to be made between global adjustments.


--annealing-adjustments
--not-annealing-adjustments
-n+
-n-

Config file line:
adjustments_during_annealing=boolean

Whether the global adjustments are performed during annealing. The default is -n+ (yes).


--site-positions-annealing-criterion
-sac

--information-annealing-crierion
-iac

--correlation-annealing-crierion
-cac

Config file line:
annealing_criterion=(site-positions|information|correlation)

How do we measure the level of changes of the motif after a set of sequential cycles.

site-positions:  The level is counted as |M1 and M2|/|M1 or M2|, where M1 and M2 are sets of positions of sites before and after the local cycle. 

informational: The level is counted as mean Kullback informational distance between 2 distributions at "strong" ppositions.



--cycles-to-say-cold unsigned
-y unsigned

Config file line:
cycles_with_minor_change_to_say_cold=unsigned

It is the number of sequential local cycles, which are to result in minor changes to stop annealing. Default is 5.


--annealing-stop-level double
-v double

Config file line:
minor_change_level=double

Config file line:
The level of a change made by a local step cycle which is referred as a little change. It is used to decide whether to finish annealing.
Default is 0.7 for site-positions annealing crierion, 0.1 for information.


--global-area-annealing-times unsigned
-g unsigned

Config file line:
annealings_number_maximum_is_to_be_global_for=unsigned


--global-area-steps unsigned
-t unsigned

Config file line:
steps_number_maximum_is_to_be_global_for=unsigned

Both of switches show how long a maximum has to be candidate for global in order to be accepted as global (and so as the program work result). If both are given, the strongest holds, i.e. -g 10000 -t 10 means 10000 steps, but not less than 10 annealing duration periods.

The defaults is -g 10 for mode=palindromes and -g 5 for both other modes.


--chain-fails-after unsigned

Config file line:
chain_fails_after=unsigned

How much adjacent annealing durations is the sampler to be hot to treat the chain as failed.

The default is 3.


--chains-to-try unsigned

Config file line:
chains_to_try=unsigned

How many chains are to fail to treat the entire task as failed.

The default is 3.

****************************************************************************

