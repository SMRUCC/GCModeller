Imports RDotNet.Extensions.VisualBasic

Namespace polysat

    Public Module API

        ''' <summary>
        ''' **Calculate Wright's Pairwise FST**
        ''' 
        ''' Given a data frame of allele frequencies and population sizes, calcFst calculates a matrix of pairwise Fst values.
        ''' calcFst works by calculating HS and HT for each locus for each pair of populations, then averaging HS and HT across loci. FST is then calculated for each pair of populations as (HT-HS)/HT.
        ''' H values(expected heterozygosities for populations And combined populations) are calculated As one minus the sum Of all squared allele frequencies at a locus. To calculte HT, allele frequencies between two populations are averaged before the calculation. To calculate HS, H values are averaged after the calculation. In both cases, the averages are weighted by the relative sizes Of the two populations (As indicated by freqs$Genomes).
        ''' </summary>
        ''' <param name="freqs">A data frame of allele frequencies and population sizes such as that produced by simpleFreq or deSilvaFreq. Each population is in one row, and a column called Genomes contains the relative size of each population. All other columns contain allele frequencies. The names of these columns are the locus name and allele name, separated by a period.</param>
        ''' <param name="pops">A character vector. Populations to analyze, which should be a subset of row.names(freqs).</param>
        ''' <param name="loci">A character vector indicating which loci to analyze. These should be a subset of the locus names as used in the column names of freqs.</param>
        ''' <returns>A square matrix containing FST values. The rows and columns of the matrix are both named by population.</returns>
        ''' <remarks>
        ''' ###### Examples
        ''' 
        ''' ```R
        ''' # create a data set (typically done by reading files)
        ''' mygenotypes &lt;- New("genambig", samples = paste("ind", 16, sep=""), loci = c("loc1", "loc2"))
        ''' Genotypes(mygenotypes, loci = "loc1") &lt;- list(c(206), c(208,210), c(204,206,210),
        '''                                                  c(196,198,202,208), c(196,200), c(198,200,202,204))
        ''' Genotypes(mygenotypes, loci = "loc2") &lt;- list(c(130,134), c(138,140), c(130,136,140),
        '''                                                  c(138), c(136,140), c(130,132,136))
        ''' PopInfo(mygenotypes)  &lt;- c(1,1,1,2,2,2)
        ''' Ploidies(mygenotypes) &lt;- c(2,2,4,4,2,4)
        ''' 
        ''' # calculate allele frequencies
        ''' myfreq &lt;- simpleFreq(mygenotypes)
        ''' 
        ''' myfreq
        ''' 
        ''' #      Genomes loc1.196 loc1.198 loc1.200 loc1.202  loc1.204  loc1.206 loc1.208  loc1.210  loc2.130  loc2.132 loc2.134  loc2.136 loc2.138  loc2.140
        ''' # Pop1       8      0.0      0.0      0.0      0.0 0.1666667 0.4166667    0.125 0.2916667 0.2916667 0.0000000    0.125 0.1666667    0.125 0.2916667
        ''' # Pop2      10      0.2      0.2      0.2      0.2 0.1000000 0.0000000    0.100 0.0000000 0.1333333 0.1333333    0.000 0.2333333    0.400 0.1000000
        ''' 
        ''' # calculate pairwise FST
        ''' myfst &lt;- calcFst(myfreq)
        ''' 
        ''' # examine the results
        ''' myfst
        ''' 
        ''' #            Pop1       Pop2
        ''' # Pop1 0.00000000 0.08870835
        ''' # Pop2 0.08870835 0.00000000
        ''' ```
        ''' </remarks>
        Public Function calcFst(freqs As String,
                                Optional pops As String = "row.names(freqs)",
                                Optional loci As String = "unique(as.matrix(as.data.frame(strsplit(names(freqs), split = ""."", fixed = TRUE), stringsAsFactors = FALSE))[1, ])") As Double()
            loci = loci.Replace("freqs", freqs)
            pops = pops.Replace("freqs", freqs)
            Return $"calcFst({freqs}, pops={pops}, loci={loci})".ζ.AsNumeric.ToArray
        End Function

#Region "Example"

        ' # create a data set (typically done by reading files)
        ' mygenotypes <- new("genambig", samples = paste("ind", 16, sep=""), loci = c("loc1", "loc2"))

        ' Genotypes(mygenotypes, loci = "loc1") <- list(c(206), c(208,210), c(204,206,210), c(196,198,202,208), c(196,200), c(198,200,202,204))
        ' Genotypes(mygenotypes, loci = "loc2") <- list(c(130,134), c(138,140), c(130,136,140), c(138), c(136,140), c(130,132,136))

        ' PopInfo(mygenotypes)  <- c(1,1,1,2,2,2)
        ' Ploidies(mygenotypes) <- c(2,2,4,4,2,4)

        ' # calculate allele frequencies
        ' myfreq <- simpleFreq(mygenotypes)

        ' # calculate pairwise FST
        ' myfst <- calcFst(myfreq)

        ' # examine the results
        ' myfst

        ''' <summary>
        ''' Example data for analysis API <see cref="calcFst"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function myfreq() As String
            Call "Genomes  <- c(8, 10)".ζ
            Call "loci.196 <- c(0.0, 0.2)".ζ
            Call "loci.198 <- c(0.0, 0.2)".ζ
            Call "loci.200 <- c(0.0, 0.2)".ζ
            Call "loci.204 <- c(0.1666667, 0.1)".ζ
            REM Call "".ζ
            REM Call "".ζ
            REM Call "".ζ
            REM Call "".ζ
            REM Call "".ζ
            REM Call "".ζ
            REM Call "".ζ
            REM Call "".ζ
            REM Call "".ζ
            REM Call "".ζ
            Call $"{NameOf(myfreq)} <- data.frame(Genomes, loci.196, loci.198, loci.200, loci.204)".ζ
            Call $"rownames({NameOf(myfreq)}) <- c(""Pop1"", ""Pop2"")".ζ

            Return NameOf(myfreq)
        End Function
#End Region
    End Module
End Namespace