# ts
_namespace: [RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.stats](./index.md)_






### Properties

#### class
Class to be given To the result, Or none If NULL Or "none". The Default Is "ts" For a Single series, c(Of"mts", "ts", "matrix") for multiple series.
#### data
a vector Or matrix Of the observed time-series values. A data frame will be coerced To a numeric matrix via data.matrix. (See also 'Details’.)
#### deltat
the fraction Of the sampling period between successive observations; e.g., 1/12 For monthly data. Only one Of frequency Or deltat should be provided.
#### end
the time Of the last observation, specified In the same way As start.
#### frequency
the number Of observations per unit Of time.
#### names
a character vector Of names For the series In a multiple series: defaults to the colnames of data, Or Series 1, Series 2, ....
#### start
the time Of the first observation. Either a Single number Or a vector Of two integers, which specify a natural time unit And a (1-based) number Of samples into the time unit. See the examples For the use Of the second form.
#### ts_eps
time series comparison tolerance. Frequencies are considered equal If their absolute difference Is less than ts.eps.
