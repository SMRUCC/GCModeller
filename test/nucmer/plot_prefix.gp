set terminal png tiny size 800,800
set output "plot_prefix.png"
set size 1,1
set grid
unset key
set border 15
set tics scale 0
set xlabel "gnl|ECOLI|COLI-K12"
set ylabel "gnl|GCF_002734145|GEN-EL1655691"
set format "%.0f"
set mouse format "%.0f"
set mouse mouseformat "[%.0f, %.0f]"
set xrange [1:4641652]
set yrange [1:3110044]
set style line 1  lt 1 lw 3 pt 6 ps 1
set style line 2  lt 3 lw 3 pt 6 ps 1
set style line 3  lt 2 lw 3 pt 6 ps 1
plot \
 "plot_prefix.fplot" title "FWD" w lp ls 1, \
 "plot_prefix.rplot" title "REV" w lp ls 2
