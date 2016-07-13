---
title: Palindrome
---

# Palindrome
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically](N-SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.html)_

=== Palindromic hexamers ===
 For a given sequence, any palindrome of 6 nt (e.g., AAATTT) Is given a value of 1, while 
 all bases Not included inpalindromic hexamers are given a value of 0 (van et al. 2003).
 -- van Noort V, Worning P, Ussery DW, Rosche WA, Sinden RR Strand misalignments lead To 
 quasipalindrome correction (2003) 19:365-9
 @"M:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Palindrome.SearchMirror(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32)" (镜像回文序列)
 
 
 === Inverted Repeats ===
 Local Inverted repeats are found by taking a 100 bp sequence window, And looking For the 
 best match Of a 30 bp piece withinthat window, On the opposite strand, In the opposite 
 direction (Jensen et al. 1999). 
 Values can range from 0 (no match at all)To 1 (one Or more perfect match within the window).
 -- L. J. Jensen And C. Friis And D.W. Ussery Three views of complete chromosomes (1999) 150773-777
 @"M:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SimilarityMatches.Repeats.InvokeSearchReversed(System.String,System.Int32,System.Int32,System.Double)" (反向重复)
 
 
 === Quasi-palindromes ===
 "Quasi-palindromes" are short inverted repeats, which are found by taking a 30 bp piece of sequence, 
 And looking for matcheswith at least 6 out of 7 nt matching, on the opposite strand, in the opposite 
 direction (van et al. 2003). Values canrange from 0 (no match at all) to 1 (one Or more perfect 
 match within the window).
 -- van Noort V, Worning P, Ussery DW, Rosche WA, Sinden RR Strand misalignments lead 
 To quasipalindrome correction (2003) 19:365-9
 @"T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Imperfect" (非完全回文)
 
 
 === Perfect-palindromes ===
 "Perfect-palindromes" are short inverted repeats, which are found by taking a 30 bp piece of sequence, 
 And looking forperfect matches of 7 nt Or longer, on the opposite strand, in the opposite direction (van et al. 2003). 
 Values can rangefrom 0 (no match at all) to 1 (one Or more perfect match within the window).
 -- van Noort V, Worning P, Ussery DW, Rosche WA, Sinden RR Strand misalignments lead To 
 quasipalindrome correction (2003) 19:365-9
 @"M:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Palindrome.SearchPalindrome(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32)" (简单回文)
 
 === Simple Repeats ===
 A "simple repeat" Is a region which contains a simple oligonucleotide repeat, Like microsattelites. 
 Simple repeats are foundby looking for tandem repeats of length R within a 2R-bp window. 
 By using the values 12, 14, 15, 16, And 18 for R, allsimple repeats of lengths 1 through 9 are calculated, 
 of length of at least 24 bp (Jensen et al. 1999). Values can range from 0(no match at all) to 1 
 (one Or more perfect match within the window).
 -- L. J. Jensen And C. Friis And D.W. Ussery Three views of complete chromosomes (1999) 150773-777
 @"M:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsSearchAPI.SearchRepeats(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32,System.Int32)" (简单重复序列)
 
 === GC Skew ===
 For many genomes there Is a strand bias, such that one strand tends To have more G's, 
 whilst the other strand has more C's.This GC-skew bias can be measured the number of G's 
 minus the number of C's over a fixed length (e.g. 10,000 bp) of DNA(Jensen et al. 1999). 
 The values can range from +1 (all G's on the examined sequence, with all C's on the other strand), 
 to -1(the reverse case - all C's on the examined sequence, and all G's on the other strand). 
 There is a correlation with GC-skewand the replication leading and lagging strands.
 -- L. J. Jensen And C. Friis And D.W. Ussery Three views of complete chromosomes (1999) 150773-777
 
 === Percent AT ===
 The percent AT Is a running average Of the AT content, over a given window size. Typically For a bacterial 
 genomes Of about5 Mbp, the window size Is 10,000 bp. The Percent AT can range from 0 (no AT content) To 1 (100% AT). 
 The Percent AT iscorrelated With other DNA structural features, such that AT rich regions are often more readily 
 melted, tend To be lessflexible And more rigid, although they can also be readily compacted chromatin proteins (Pedersen et al. 2000).
 -- A.G. Pedersen And L.J. Jensen And H.H. St\aerfeldt And S. Brunak And D.W. 
 Ussery A DNA structural atlas of extitE. coli (2000) 299907-930



### Methods

#### CreateMirrors
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Palindrome.CreateMirrors(System.String,System.String)
```
这个函数求解的是绝对相等的

|Parameter Name|Remarks|
|--------------|-------|
|Segment|-|
|Sequence|-|


#### HaveMirror
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Palindrome.HaveMirror(System.String,System.String)
```
Have mirror repeats?

|Parameter Name|Remarks|
|--------------|-------|
|Segment|-|
|Sequence|-|


#### HavePalindrome
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Palindrome.HavePalindrome(System.String,System.String)
```
Have Palindrome repeats?

|Parameter Name|Remarks|
|--------------|-------|
|Segment|-|
|Sequence|-|


#### SearchMirror
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Palindrome.SearchMirror(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32)
```
搜索序列上面的镜像回文片段

|Parameter Name|Remarks|
|--------------|-------|
|Sequence|-|
|Min|-|
|Max|-|



