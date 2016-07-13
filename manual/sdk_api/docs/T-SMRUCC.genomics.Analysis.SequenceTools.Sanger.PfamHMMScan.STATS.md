---
title: STATS
---

# STATS
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.Sanger.PfamHMMScan](N-SMRUCC.genomics.Analysis.SequenceTools.Sanger.PfamHMMScan.html)_

Statistical parameters needed for E-value calculations. <s1> is the model’s
 alignment mode configuration: currently only LOCAL Is recognized. <s2> Is the name Of the score
 distribution: currently MSV, VITERBI, and FORWARD are recognized. <f1> And <f2> are two realvalued
 parameters controlling location And slope Of Each distribution, respectively; And For
 Gumbel distributions For MSV And Viterbi scores, And And For exponential tails For Forward
 scores. values must be positive. All three lines Or none of them must be present: when all three
 are present, the model Is considered To be calibrated For E-value statistics. Optional.




