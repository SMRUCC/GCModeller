'************************************************************************
'ALGLIB 3.9.0 (source code generated 2014-12-11)
'Copyright (c) Sergey Bochkanov (ALGLIB project).
'
'>>> SOURCE LICENSE >>>
'This program is free software; you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation (www.fsf.org); either version 2 of the 
'License, or (at your option) any later version.
'
'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.
'
'A copy of the GNU General Public License is available at
'http://www.fsf.org/licensing/licenses
'>>> END OF LICENSE >>>
'************************************************************************

'#Pragma warning disable 162
'#Pragma warning disable 219

Public Partial Class alglib


	'************************************************************************
'    Calculation of the distribution moments: mean, variance, skewness, kurtosis.
'
'    INPUT PARAMETERS:
'        X       -   sample
'        N       -   N>=0, sample size:
'                    * if given, only leading N elements of X are processed
'                    * if not given, automatically determined from size of X
'
'    OUTPUT PARAMETERS
'        Mean    -   mean.
'        Variance-   variance.
'        Skewness-   skewness (if variance<>0; zero otherwise).
'        Kurtosis-   kurtosis (if variance<>0; zero otherwise).
'
'
'      -- ALGLIB --
'         Copyright 06.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub samplemoments(x As Double(), n As Integer, ByRef mean As Double, ByRef variance As Double, ByRef skewness As Double, ByRef kurtosis As Double)
		mean = 0
		variance = 0
		skewness = 0
		kurtosis = 0
		basestat.samplemoments(x, n, mean, variance, skewness, kurtosis)
		Return
	End Sub
	Public Shared Sub samplemoments(x As Double(), ByRef mean As Double, ByRef variance As Double, ByRef skewness As Double, ByRef kurtosis As Double)
		Dim n As Integer

		mean = 0
		variance = 0
		skewness = 0
		kurtosis = 0
		n = ap.len(x)
		basestat.samplemoments(x, n, mean, variance, skewness, kurtosis)

		Return
	End Sub

	'************************************************************************
'    Calculation of the mean.
'
'    INPUT PARAMETERS:
'        X       -   sample
'        N       -   N>=0, sample size:
'                    * if given, only leading N elements of X are processed
'                    * if not given, automatically determined from size of X
'
'    NOTE:
'
'    This function return result  which calculated by 'SampleMoments' function
'    and stored at 'Mean' variable.
'
'
'      -- ALGLIB --
'         Copyright 06.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function samplemean(x As Double(), n As Integer) As Double

		Dim result As Double = basestat.samplemean(x, n)
		Return result
	End Function
	Public Shared Function samplemean(x As Double()) As Double
		Dim n As Integer


		n = ap.len(x)
		Dim result As Double = basestat.samplemean(x, n)

		Return result
	End Function

	'************************************************************************
'    Calculation of the variance.
'
'    INPUT PARAMETERS:
'        X       -   sample
'        N       -   N>=0, sample size:
'                    * if given, only leading N elements of X are processed
'                    * if not given, automatically determined from size of X
'
'    NOTE:
'
'    This function return result  which calculated by 'SampleMoments' function
'    and stored at 'Variance' variable.
'
'
'      -- ALGLIB --
'         Copyright 06.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function samplevariance(x As Double(), n As Integer) As Double

		Dim result As Double = basestat.samplevariance(x, n)
		Return result
	End Function
	Public Shared Function samplevariance(x As Double()) As Double
		Dim n As Integer


		n = ap.len(x)
		Dim result As Double = basestat.samplevariance(x, n)

		Return result
	End Function

	'************************************************************************
'    Calculation of the skewness.
'
'    INPUT PARAMETERS:
'        X       -   sample
'        N       -   N>=0, sample size:
'                    * if given, only leading N elements of X are processed
'                    * if not given, automatically determined from size of X
'
'    NOTE:
'
'    This function return result  which calculated by 'SampleMoments' function
'    and stored at 'Skewness' variable.
'
'
'      -- ALGLIB --
'         Copyright 06.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function sampleskewness(x As Double(), n As Integer) As Double

		Dim result As Double = basestat.sampleskewness(x, n)
		Return result
	End Function
	Public Shared Function sampleskewness(x As Double()) As Double
		Dim n As Integer


		n = ap.len(x)
		Dim result As Double = basestat.sampleskewness(x, n)

		Return result
	End Function

	'************************************************************************
'    Calculation of the kurtosis.
'
'    INPUT PARAMETERS:
'        X       -   sample
'        N       -   N>=0, sample size:
'                    * if given, only leading N elements of X are processed
'                    * if not given, automatically determined from size of X
'
'    NOTE:
'
'    This function return result  which calculated by 'SampleMoments' function
'    and stored at 'Kurtosis' variable.
'
'
'      -- ALGLIB --
'         Copyright 06.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function samplekurtosis(x As Double(), n As Integer) As Double

		Dim result As Double = basestat.samplekurtosis(x, n)
		Return result
	End Function
	Public Shared Function samplekurtosis(x As Double()) As Double
		Dim n As Integer


		n = ap.len(x)
		Dim result As Double = basestat.samplekurtosis(x, n)

		Return result
	End Function

	'************************************************************************
'    ADev
'
'    Input parameters:
'        X   -   sample
'        N   -   N>=0, sample size:
'                * if given, only leading N elements of X are processed
'                * if not given, automatically determined from size of X
'
'    Output parameters:
'        ADev-   ADev
'
'      -- ALGLIB --
'         Copyright 06.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub sampleadev(x As Double(), n As Integer, ByRef adev As Double)
		adev = 0
		basestat.sampleadev(x, n, adev)
		Return
	End Sub
	Public Shared Sub sampleadev(x As Double(), ByRef adev As Double)
		Dim n As Integer

		adev = 0
		n = ap.len(x)
		basestat.sampleadev(x, n, adev)

		Return
	End Sub

	'************************************************************************
'    Median calculation.
'
'    Input parameters:
'        X   -   sample (array indexes: [0..N-1])
'        N   -   N>=0, sample size:
'                * if given, only leading N elements of X are processed
'                * if not given, automatically determined from size of X
'
'    Output parameters:
'        Median
'
'      -- ALGLIB --
'         Copyright 06.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub samplemedian(x As Double(), n As Integer, ByRef median As Double)
		median = 0
		basestat.samplemedian(x, n, median)
		Return
	End Sub
	Public Shared Sub samplemedian(x As Double(), ByRef median As Double)
		Dim n As Integer

		median = 0
		n = ap.len(x)
		basestat.samplemedian(x, n, median)

		Return
	End Sub

	'************************************************************************
'    Percentile calculation.
'
'    Input parameters:
'        X   -   sample (array indexes: [0..N-1])
'        N   -   N>=0, sample size:
'                * if given, only leading N elements of X are processed
'                * if not given, automatically determined from size of X
'        P   -   percentile (0<=P<=1)
'
'    Output parameters:
'        V   -   percentile
'
'      -- ALGLIB --
'         Copyright 01.03.2008 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub samplepercentile(x As Double(), n As Integer, p As Double, ByRef v As Double)
		v = 0
		basestat.samplepercentile(x, n, p, v)
		Return
	End Sub
	Public Shared Sub samplepercentile(x As Double(), p As Double, ByRef v As Double)
		Dim n As Integer

		v = 0
		n = ap.len(x)
		basestat.samplepercentile(x, n, p, v)

		Return
	End Sub

	'************************************************************************
'    2-sample covariance
'
'    Input parameters:
'        X       -   sample 1 (array indexes: [0..N-1])
'        Y       -   sample 2 (array indexes: [0..N-1])
'        N       -   N>=0, sample size:
'                    * if given, only N leading elements of X/Y are processed
'                    * if not given, automatically determined from input sizes
'
'    Result:
'        covariance (zero for N=0 or N=1)
'
'      -- ALGLIB --
'         Copyright 28.10.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function cov2(x As Double(), y As Double(), n As Integer) As Double

		Dim result As Double = basestat.cov2(x, y, n)
		Return result
	End Function
	Public Shared Function cov2(x As Double(), y As Double()) As Double
		Dim n As Integer
		If (ap.len(x) <> ap.len(y)) Then
			Throw New alglibexception("Error while calling 'cov2': looks like one of arguments has wrong size")
		End If

		n = ap.len(x)
		Dim result As Double = basestat.cov2(x, y, n)

		Return result
	End Function

	'************************************************************************
'    Pearson product-moment correlation coefficient
'
'    Input parameters:
'        X       -   sample 1 (array indexes: [0..N-1])
'        Y       -   sample 2 (array indexes: [0..N-1])
'        N       -   N>=0, sample size:
'                    * if given, only N leading elements of X/Y are processed
'                    * if not given, automatically determined from input sizes
'
'    Result:
'        Pearson product-moment correlation coefficient
'        (zero for N=0 or N=1)
'
'      -- ALGLIB --
'         Copyright 28.10.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function pearsoncorr2(x As Double(), y As Double(), n As Integer) As Double

		Dim result As Double = basestat.pearsoncorr2(x, y, n)
		Return result
	End Function
	Public Shared Function pearsoncorr2(x As Double(), y As Double()) As Double
		Dim n As Integer
		If (ap.len(x) <> ap.len(y)) Then
			Throw New alglibexception("Error while calling 'pearsoncorr2': looks like one of arguments has wrong size")
		End If

		n = ap.len(x)
		Dim result As Double = basestat.pearsoncorr2(x, y, n)

		Return result
	End Function

	'************************************************************************
'    Spearman's rank correlation coefficient
'
'    Input parameters:
'        X       -   sample 1 (array indexes: [0..N-1])
'        Y       -   sample 2 (array indexes: [0..N-1])
'        N       -   N>=0, sample size:
'                    * if given, only N leading elements of X/Y are processed
'                    * if not given, automatically determined from input sizes
'
'    Result:
'        Spearman's rank correlation coefficient
'        (zero for N=0 or N=1)
'
'      -- ALGLIB --
'         Copyright 09.04.2007 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function spearmancorr2(x As Double(), y As Double(), n As Integer) As Double

		Dim result As Double = basestat.spearmancorr2(x, y, n)
		Return result
	End Function
	Public Shared Function spearmancorr2(x As Double(), y As Double()) As Double
		Dim n As Integer
		If (ap.len(x) <> ap.len(y)) Then
			Throw New alglibexception("Error while calling 'spearmancorr2': looks like one of arguments has wrong size")
		End If

		n = ap.len(x)
		Dim result As Double = basestat.spearmancorr2(x, y, n)

		Return result
	End Function

	'************************************************************************
'    Covariance matrix
'
'    SMP EDITION OF ALGLIB:
'
'      ! This function can utilize multicore capabilities of  your system.  In
'      ! order to do this you have to call version with "smp_" prefix,   which
'      ! indicates that multicore code will be used.
'      !
'      ! This note is given for users of SMP edition; if you use GPL  edition,
'      ! or commercial edition of ALGLIB without SMP support, you  still  will
'      ! be able to call smp-version of this function,  but  all  computations
'      ! will be done serially.
'      !
'      ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'      ! called 'SMP support', before using parallel version of this function.
'      !
'      ! You should remember that starting/stopping worker thread always  have
'      ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'      ! large problems, we do not recommend you to use it on small problems -
'      ! with covariance matrices smaller than 128*128.
'
'    INPUT PARAMETERS:
'        X   -   array[N,M], sample matrix:
'                * J-th column corresponds to J-th variable
'                * I-th row corresponds to I-th observation
'        N   -   N>=0, number of observations:
'                * if given, only leading N rows of X are used
'                * if not given, automatically determined from input size
'        M   -   M>0, number of variables:
'                * if given, only leading M columns of X are used
'                * if not given, automatically determined from input size
'
'    OUTPUT PARAMETERS:
'        C   -   array[M,M], covariance matrix (zero if N=0 or N=1)
'
'      -- ALGLIB --
'         Copyright 28.10.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub covm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat.covm(x, n, m, c)
		Return
	End Sub


	Public Shared Sub smp_covm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat._pexec_covm(x, n, m, c)
		Return
	End Sub
	Public Shared Sub covm(x As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m As Integer

		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m = ap.cols(x)
		basestat.covm(x, n, m, c)

		Return
	End Sub


	Public Shared Sub smp_covm(x As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m As Integer

		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m = ap.cols(x)
		basestat._pexec_covm(x, n, m, c)

		Return
	End Sub

	'************************************************************************
'    Pearson product-moment correlation matrix
'
'    SMP EDITION OF ALGLIB:
'
'      ! This function can utilize multicore capabilities of  your system.  In
'      ! order to do this you have to call version with "smp_" prefix,   which
'      ! indicates that multicore code will be used.
'      !
'      ! This note is given for users of SMP edition; if you use GPL  edition,
'      ! or commercial edition of ALGLIB without SMP support, you  still  will
'      ! be able to call smp-version of this function,  but  all  computations
'      ! will be done serially.
'      !
'      ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'      ! called 'SMP support', before using parallel version of this function.
'      !
'      ! You should remember that starting/stopping worker thread always  have
'      ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'      ! large problems, we do not recommend you to use it on small problems -
'      ! with correlation matrices smaller than 128*128.
'
'    INPUT PARAMETERS:
'        X   -   array[N,M], sample matrix:
'                * J-th column corresponds to J-th variable
'                * I-th row corresponds to I-th observation
'        N   -   N>=0, number of observations:
'                * if given, only leading N rows of X are used
'                * if not given, automatically determined from input size
'        M   -   M>0, number of variables:
'                * if given, only leading M columns of X are used
'                * if not given, automatically determined from input size
'
'    OUTPUT PARAMETERS:
'        C   -   array[M,M], correlation matrix (zero if N=0 or N=1)
'
'      -- ALGLIB --
'         Copyright 28.10.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub pearsoncorrm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat.pearsoncorrm(x, n, m, c)
		Return
	End Sub


	Public Shared Sub smp_pearsoncorrm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat._pexec_pearsoncorrm(x, n, m, c)
		Return
	End Sub
	Public Shared Sub pearsoncorrm(x As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m As Integer

		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m = ap.cols(x)
		basestat.pearsoncorrm(x, n, m, c)

		Return
	End Sub


	Public Shared Sub smp_pearsoncorrm(x As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m As Integer

		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m = ap.cols(x)
		basestat._pexec_pearsoncorrm(x, n, m, c)

		Return
	End Sub

	'************************************************************************
'    Spearman's rank correlation matrix
'
'    SMP EDITION OF ALGLIB:
'
'      ! This function can utilize multicore capabilities of  your system.  In
'      ! order to do this you have to call version with "smp_" prefix,   which
'      ! indicates that multicore code will be used.
'      !
'      ! This note is given for users of SMP edition; if you use GPL  edition,
'      ! or commercial edition of ALGLIB without SMP support, you  still  will
'      ! be able to call smp-version of this function,  but  all  computations
'      ! will be done serially.
'      !
'      ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'      ! called 'SMP support', before using parallel version of this function.
'      !
'      ! You should remember that starting/stopping worker thread always  have
'      ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'      ! large problems, we do not recommend you to use it on small problems -
'      ! with correlation matrices smaller than 128*128.
'
'    INPUT PARAMETERS:
'        X   -   array[N,M], sample matrix:
'                * J-th column corresponds to J-th variable
'                * I-th row corresponds to I-th observation
'        N   -   N>=0, number of observations:
'                * if given, only leading N rows of X are used
'                * if not given, automatically determined from input size
'        M   -   M>0, number of variables:
'                * if given, only leading M columns of X are used
'                * if not given, automatically determined from input size
'
'    OUTPUT PARAMETERS:
'        C   -   array[M,M], correlation matrix (zero if N=0 or N=1)
'
'      -- ALGLIB --
'         Copyright 28.10.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub spearmancorrm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat.spearmancorrm(x, n, m, c)
		Return
	End Sub


	Public Shared Sub smp_spearmancorrm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat._pexec_spearmancorrm(x, n, m, c)
		Return
	End Sub
	Public Shared Sub spearmancorrm(x As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m As Integer

		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m = ap.cols(x)
		basestat.spearmancorrm(x, n, m, c)

		Return
	End Sub


	Public Shared Sub smp_spearmancorrm(x As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m As Integer

		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m = ap.cols(x)
		basestat._pexec_spearmancorrm(x, n, m, c)

		Return
	End Sub

	'************************************************************************
'    Cross-covariance matrix
'
'    SMP EDITION OF ALGLIB:
'
'      ! This function can utilize multicore capabilities of  your system.  In
'      ! order to do this you have to call version with "smp_" prefix,   which
'      ! indicates that multicore code will be used.
'      !
'      ! This note is given for users of SMP edition; if you use GPL  edition,
'      ! or commercial edition of ALGLIB without SMP support, you  still  will
'      ! be able to call smp-version of this function,  but  all  computations
'      ! will be done serially.
'      !
'      ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'      ! called 'SMP support', before using parallel version of this function.
'      !
'      ! You should remember that starting/stopping worker thread always  have
'      ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'      ! large problems, we do not recommend you to use it on small problems -
'      ! with covariance matrices smaller than 128*128.
'
'    INPUT PARAMETERS:
'        X   -   array[N,M1], sample matrix:
'                * J-th column corresponds to J-th variable
'                * I-th row corresponds to I-th observation
'        Y   -   array[N,M2], sample matrix:
'                * J-th column corresponds to J-th variable
'                * I-th row corresponds to I-th observation
'        N   -   N>=0, number of observations:
'                * if given, only leading N rows of X/Y are used
'                * if not given, automatically determined from input sizes
'        M1  -   M1>0, number of variables in X:
'                * if given, only leading M1 columns of X are used
'                * if not given, automatically determined from input size
'        M2  -   M2>0, number of variables in Y:
'                * if given, only leading M1 columns of X are used
'                * if not given, automatically determined from input size
'
'    OUTPUT PARAMETERS:
'        C   -   array[M1,M2], cross-covariance matrix (zero if N=0 or N=1)
'
'      -- ALGLIB --
'         Copyright 28.10.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub covm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat.covm2(x, y, n, m1, m2, c)
		Return
	End Sub


	Public Shared Sub smp_covm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat._pexec_covm2(x, y, n, m1, m2, c)
		Return
	End Sub
	Public Shared Sub covm2(x As Double(,), y As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m1 As Integer
		Dim m2 As Integer
		If (ap.rows(x) <> ap.rows(y)) Then
			Throw New alglibexception("Error while calling 'covm2': looks like one of arguments has wrong size")
		End If
		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m1 = ap.cols(x)
		m2 = ap.cols(y)
		basestat.covm2(x, y, n, m1, m2, c)

		Return
	End Sub


	Public Shared Sub smp_covm2(x As Double(,), y As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m1 As Integer
		Dim m2 As Integer
		If (ap.rows(x) <> ap.rows(y)) Then
			Throw New alglibexception("Error while calling 'covm2': looks like one of arguments has wrong size")
		End If
		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m1 = ap.cols(x)
		m2 = ap.cols(y)
		basestat._pexec_covm2(x, y, n, m1, m2, c)

		Return
	End Sub

	'************************************************************************
'    Pearson product-moment cross-correlation matrix
'
'    SMP EDITION OF ALGLIB:
'
'      ! This function can utilize multicore capabilities of  your system.  In
'      ! order to do this you have to call version with "smp_" prefix,   which
'      ! indicates that multicore code will be used.
'      !
'      ! This note is given for users of SMP edition; if you use GPL  edition,
'      ! or commercial edition of ALGLIB without SMP support, you  still  will
'      ! be able to call smp-version of this function,  but  all  computations
'      ! will be done serially.
'      !
'      ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'      ! called 'SMP support', before using parallel version of this function.
'      !
'      ! You should remember that starting/stopping worker thread always  have
'      ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'      ! large problems, we do not recommend you to use it on small problems -
'      ! with correlation matrices smaller than 128*128.
'
'    INPUT PARAMETERS:
'        X   -   array[N,M1], sample matrix:
'                * J-th column corresponds to J-th variable
'                * I-th row corresponds to I-th observation
'        Y   -   array[N,M2], sample matrix:
'                * J-th column corresponds to J-th variable
'                * I-th row corresponds to I-th observation
'        N   -   N>=0, number of observations:
'                * if given, only leading N rows of X/Y are used
'                * if not given, automatically determined from input sizes
'        M1  -   M1>0, number of variables in X:
'                * if given, only leading M1 columns of X are used
'                * if not given, automatically determined from input size
'        M2  -   M2>0, number of variables in Y:
'                * if given, only leading M1 columns of X are used
'                * if not given, automatically determined from input size
'
'    OUTPUT PARAMETERS:
'        C   -   array[M1,M2], cross-correlation matrix (zero if N=0 or N=1)
'
'      -- ALGLIB --
'         Copyright 28.10.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub pearsoncorrm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat.pearsoncorrm2(x, y, n, m1, m2, c)
		Return
	End Sub


	Public Shared Sub smp_pearsoncorrm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat._pexec_pearsoncorrm2(x, y, n, m1, m2, c)
		Return
	End Sub
	Public Shared Sub pearsoncorrm2(x As Double(,), y As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m1 As Integer
		Dim m2 As Integer
		If (ap.rows(x) <> ap.rows(y)) Then
			Throw New alglibexception("Error while calling 'pearsoncorrm2': looks like one of arguments has wrong size")
		End If
		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m1 = ap.cols(x)
		m2 = ap.cols(y)
		basestat.pearsoncorrm2(x, y, n, m1, m2, c)

		Return
	End Sub


	Public Shared Sub smp_pearsoncorrm2(x As Double(,), y As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m1 As Integer
		Dim m2 As Integer
		If (ap.rows(x) <> ap.rows(y)) Then
			Throw New alglibexception("Error while calling 'pearsoncorrm2': looks like one of arguments has wrong size")
		End If
		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m1 = ap.cols(x)
		m2 = ap.cols(y)
		basestat._pexec_pearsoncorrm2(x, y, n, m1, m2, c)

		Return
	End Sub

	'************************************************************************
'    Spearman's rank cross-correlation matrix
'
'    SMP EDITION OF ALGLIB:
'
'      ! This function can utilize multicore capabilities of  your system.  In
'      ! order to do this you have to call version with "smp_" prefix,   which
'      ! indicates that multicore code will be used.
'      !
'      ! This note is given for users of SMP edition; if you use GPL  edition,
'      ! or commercial edition of ALGLIB without SMP support, you  still  will
'      ! be able to call smp-version of this function,  but  all  computations
'      ! will be done serially.
'      !
'      ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'      ! called 'SMP support', before using parallel version of this function.
'      !
'      ! You should remember that starting/stopping worker thread always  have
'      ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'      ! large problems, we do not recommend you to use it on small problems -
'      ! with correlation matrices smaller than 128*128.
'
'    INPUT PARAMETERS:
'        X   -   array[N,M1], sample matrix:
'                * J-th column corresponds to J-th variable
'                * I-th row corresponds to I-th observation
'        Y   -   array[N,M2], sample matrix:
'                * J-th column corresponds to J-th variable
'                * I-th row corresponds to I-th observation
'        N   -   N>=0, number of observations:
'                * if given, only leading N rows of X/Y are used
'                * if not given, automatically determined from input sizes
'        M1  -   M1>0, number of variables in X:
'                * if given, only leading M1 columns of X are used
'                * if not given, automatically determined from input size
'        M2  -   M2>0, number of variables in Y:
'                * if given, only leading M1 columns of X are used
'                * if not given, automatically determined from input size
'
'    OUTPUT PARAMETERS:
'        C   -   array[M1,M2], cross-correlation matrix (zero if N=0 or N=1)
'
'      -- ALGLIB --
'         Copyright 28.10.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub spearmancorrm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat.spearmancorrm2(x, y, n, m1, m2, c)
		Return
	End Sub


	Public Shared Sub smp_spearmancorrm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
		c = New Double(-1, -1) {}
		basestat._pexec_spearmancorrm2(x, y, n, m1, m2, c)
		Return
	End Sub
	Public Shared Sub spearmancorrm2(x As Double(,), y As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m1 As Integer
		Dim m2 As Integer
		If (ap.rows(x) <> ap.rows(y)) Then
			Throw New alglibexception("Error while calling 'spearmancorrm2': looks like one of arguments has wrong size")
		End If
		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m1 = ap.cols(x)
		m2 = ap.cols(y)
		basestat.spearmancorrm2(x, y, n, m1, m2, c)

		Return
	End Sub


	Public Shared Sub smp_spearmancorrm2(x As Double(,), y As Double(,), ByRef c As Double(,))
		Dim n As Integer
		Dim m1 As Integer
		Dim m2 As Integer
		If (ap.rows(x) <> ap.rows(y)) Then
			Throw New alglibexception("Error while calling 'spearmancorrm2': looks like one of arguments has wrong size")
		End If
		c = New Double(-1, -1) {}
		n = ap.rows(x)
		m1 = ap.cols(x)
		m2 = ap.cols(y)
		basestat._pexec_spearmancorrm2(x, y, n, m1, m2, c)

		Return
	End Sub

	'************************************************************************
'    This function replaces data in XY by their ranks:
'    * XY is processed row-by-row
'    * rows are processed separately
'    * tied data are correctly handled (tied ranks are calculated)
'    * ranking starts from 0, ends at NFeatures-1
'    * sum of within-row values is equal to (NFeatures-1)*NFeatures/2
'
'    SMP EDITION OF ALGLIB:
'
'      ! This function can utilize multicore capabilities of  your system.  In
'      ! order to do this you have to call version with "smp_" prefix,   which
'      ! indicates that multicore code will be used.
'      !
'      ! This note is given for users of SMP edition; if you use GPL  edition,
'      ! or commercial edition of ALGLIB without SMP support, you  still  will
'      ! be able to call smp-version of this function,  but  all  computations
'      ! will be done serially.
'      !
'      ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'      ! called 'SMP support', before using parallel version of this function.
'      !
'      ! You should remember that starting/stopping worker thread always  have
'      ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'      ! large problems, we do not recommend you to use it on small problems -
'      ! ones where expected operations count is less than 100.000
'
'    INPUT PARAMETERS:
'        XY      -   array[NPoints,NFeatures], dataset
'        NPoints -   number of points
'        NFeatures-  number of features
'
'    OUTPUT PARAMETERS:
'        XY      -   data are replaced by their within-row ranks;
'                    ranking starts from 0, ends at NFeatures-1
'
'      -- ALGLIB --
'         Copyright 18.04.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub rankdata(ByRef xy As Double(,), npoints As Integer, nfeatures As Integer)

		basestat.rankdata(xy, npoints, nfeatures)
		Return
	End Sub


	Public Shared Sub smp_rankdata(ByRef xy As Double(,), npoints As Integer, nfeatures As Integer)

		basestat._pexec_rankdata(xy, npoints, nfeatures)
		Return
	End Sub
	Public Shared Sub rankdata(ByRef xy As Double(,))
		Dim npoints As Integer
		Dim nfeatures As Integer


		npoints = ap.rows(xy)
		nfeatures = ap.cols(xy)
		basestat.rankdata(xy, npoints, nfeatures)

		Return
	End Sub


	Public Shared Sub smp_rankdata(ByRef xy As Double(,))
		Dim npoints As Integer
		Dim nfeatures As Integer


		npoints = ap.rows(xy)
		nfeatures = ap.cols(xy)
		basestat._pexec_rankdata(xy, npoints, nfeatures)

		Return
	End Sub

	'************************************************************************
'    This function replaces data in XY by their CENTERED ranks:
'    * XY is processed row-by-row
'    * rows are processed separately
'    * tied data are correctly handled (tied ranks are calculated)
'    * centered ranks are just usual ranks, but centered in such way  that  sum
'      of within-row values is equal to 0.0.
'    * centering is performed by subtracting mean from each row, i.e it changes
'      mean value, but does NOT change higher moments
'
'    SMP EDITION OF ALGLIB:
'
'      ! This function can utilize multicore capabilities of  your system.  In
'      ! order to do this you have to call version with "smp_" prefix,   which
'      ! indicates that multicore code will be used.
'      !
'      ! This note is given for users of SMP edition; if you use GPL  edition,
'      ! or commercial edition of ALGLIB without SMP support, you  still  will
'      ! be able to call smp-version of this function,  but  all  computations
'      ! will be done serially.
'      !
'      ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'      ! called 'SMP support', before using parallel version of this function.
'      !
'      ! You should remember that starting/stopping worker thread always  have
'      ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'      ! large problems, we do not recommend you to use it on small problems -
'      ! ones where expected operations count is less than 100.000
'
'    INPUT PARAMETERS:
'        XY      -   array[NPoints,NFeatures], dataset
'        NPoints -   number of points
'        NFeatures-  number of features
'
'    OUTPUT PARAMETERS:
'        XY      -   data are replaced by their within-row ranks;
'                    ranking starts from 0, ends at NFeatures-1
'
'      -- ALGLIB --
'         Copyright 18.04.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub rankdatacentered(ByRef xy As Double(,), npoints As Integer, nfeatures As Integer)

		basestat.rankdatacentered(xy, npoints, nfeatures)
		Return
	End Sub


	Public Shared Sub smp_rankdatacentered(ByRef xy As Double(,), npoints As Integer, nfeatures As Integer)

		basestat._pexec_rankdatacentered(xy, npoints, nfeatures)
		Return
	End Sub
	Public Shared Sub rankdatacentered(ByRef xy As Double(,))
		Dim npoints As Integer
		Dim nfeatures As Integer


		npoints = ap.rows(xy)
		nfeatures = ap.cols(xy)
		basestat.rankdatacentered(xy, npoints, nfeatures)

		Return
	End Sub


	Public Shared Sub smp_rankdatacentered(ByRef xy As Double(,))
		Dim npoints As Integer
		Dim nfeatures As Integer


		npoints = ap.rows(xy)
		nfeatures = ap.cols(xy)
		basestat._pexec_rankdatacentered(xy, npoints, nfeatures)

		Return
	End Sub

	'************************************************************************
'    Obsolete function, we recommend to use PearsonCorr2().
'
'      -- ALGLIB --
'         Copyright 09.04.2007 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function pearsoncorrelation(x As Double(), y As Double(), n As Integer) As Double

		Dim result As Double = basestat.pearsoncorrelation(x, y, n)
		Return result
	End Function

	'************************************************************************
'    Obsolete function, we recommend to use SpearmanCorr2().
'
'        -- ALGLIB --
'        Copyright 09.04.2007 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function spearmanrankcorrelation(x As Double(), y As Double(), n As Integer) As Double

		Dim result As Double = basestat.spearmanrankcorrelation(x, y, n)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Pearson's correlation coefficient significance test
'
'    This test checks hypotheses about whether X  and  Y  are  samples  of  two
'    continuous  distributions  having  zero  correlation  or   whether   their
'    correlation is non-zero.
'
'    The following tests are performed:
'        * two-tailed test (null hypothesis - X and Y have zero correlation)
'        * left-tailed test (null hypothesis - the correlation  coefficient  is
'          greater than or equal to 0)
'        * right-tailed test (null hypothesis - the correlation coefficient  is
'          less than or equal to 0).
'
'    Requirements:
'        * the number of elements in each sample is not less than 5
'        * normality of distributions of X and Y.
'
'    Input parameters:
'        R   -   Pearson's correlation coefficient for X and Y
'        N   -   number of elements in samples, N>=5.
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'      -- ALGLIB --
'         Copyright 09.04.2007 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub pearsoncorrelationsignificance(r As Double, n As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		correlationtests.pearsoncorrelationsignificance(r, n, bothtails, lefttail, righttail)
		Return
	End Sub

	'************************************************************************
'    Spearman's rank correlation coefficient significance test
'
'    This test checks hypotheses about whether X  and  Y  are  samples  of  two
'    continuous  distributions  having  zero  correlation  or   whether   their
'    correlation is non-zero.
'
'    The following tests are performed:
'        * two-tailed test (null hypothesis - X and Y have zero correlation)
'        * left-tailed test (null hypothesis - the correlation  coefficient  is
'          greater than or equal to 0)
'        * right-tailed test (null hypothesis - the correlation coefficient  is
'          less than or equal to 0).
'
'    Requirements:
'        * the number of elements in each sample is not less than 5.
'
'    The test is non-parametric and doesn't require distributions X and Y to be
'    normal.
'
'    Input parameters:
'        R   -   Spearman's rank correlation coefficient for X and Y
'        N   -   number of elements in samples, N>=5.
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'      -- ALGLIB --
'         Copyright 09.04.2007 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub spearmanrankcorrelationsignificance(r As Double, n As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		correlationtests.spearmanrankcorrelationsignificance(r, n, bothtails, lefttail, righttail)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Jarque-Bera test
'
'    This test checks hypotheses about the fact that a  given  sample  X  is  a
'    sample of normal random variable.
'
'    Requirements:
'        * the number of elements in the sample is not less than 5.
'
'    Input parameters:
'        X   -   sample. Array whose index goes from 0 to N-1.
'        N   -   size of the sample. N>=5
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'    Accuracy of the approximation used (5<=N<=1951):
'
'    p-value  	    relative error (5<=N<=1951)
'    [1, 0.1]            < 1%
'    [0.1, 0.01]         < 2%
'    [0.01, 0.001]       < 6%
'    [0.001, 0]          wasn't measured
'
'    For N>1951 accuracy wasn't measured but it shouldn't be sharply  different
'    from table values.
'
'      -- ALGLIB --
'         Copyright 09.04.2007 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub jarqueberatest(x As Double(), n As Integer, ByRef p As Double)
		p = 0
		jarquebera.jarqueberatest(x, n, p)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Mann-Whitney U-test
'
'    This test checks hypotheses about whether X  and  Y  are  samples  of  two
'    continuous distributions of the same shape  and  same  median  or  whether
'    their medians are different.
'
'    The following tests are performed:
'        * two-tailed test (null hypothesis - the medians are equal)
'        * left-tailed test (null hypothesis - the median of the  first  sample
'          is greater than or equal to the median of the second sample)
'        * right-tailed test (null hypothesis - the median of the first  sample
'          is less than or equal to the median of the second sample).
'
'    Requirements:
'        * the samples are independent
'        * X and Y are continuous distributions (or discrete distributions well-
'          approximating continuous distributions)
'        * distributions of X and Y have the  same  shape.  The  only  possible
'          difference is their position (i.e. the value of the median)
'        * the number of elements in each sample is not less than 5
'        * the scale of measurement should be ordinal, interval or ratio  (i.e.
'          the test could not be applied to nominal variables).
'
'    The test is non-parametric and doesn't require distributions to be normal.
'
'    Input parameters:
'        X   -   sample 1. Array whose index goes from 0 to N-1.
'        N   -   size of the sample. N>=5
'        Y   -   sample 2. Array whose index goes from 0 to M-1.
'        M   -   size of the sample. M>=5
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'    To calculate p-values, special approximation is used. This method lets  us
'    calculate p-values with satisfactory  accuracy  in  interval  [0.0001, 1].
'    There is no approximation outside the [0.0001, 1] interval. Therefore,  if
'    the significance level outlies this interval, the test returns 0.0001.
'
'    Relative precision of approximation of p-value:
'
'    N          M          Max.err.   Rms.err.
'    5..10      N..10      1.4e-02    6.0e-04
'    5..10      N..100     2.2e-02    5.3e-06
'    10..15     N..15      1.0e-02    3.2e-04
'    10..15     N..100     1.0e-02    2.2e-05
'    15..100    N..100     6.1e-03    2.7e-06
'
'    For N,M>100 accuracy checks weren't put into  practice,  but  taking  into
'    account characteristics of asymptotic approximation used, precision should
'    not be sharply different from the values for interval [5, 100].
'
'      -- ALGLIB --
'         Copyright 09.04.2007 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub mannwhitneyutest(x As Double(), n As Integer, y As Double(), m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, _
		ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		mannwhitneyu.mannwhitneyutest(x, n, y, m, bothtails, lefttail, _
			righttail)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Sign test
'
'    This test checks three hypotheses about the median of  the  given  sample.
'    The following tests are performed:
'        * two-tailed test (null hypothesis - the median is equal to the  given
'          value)
'        * left-tailed test (null hypothesis - the median is  greater  than  or
'          equal to the given value)
'        * right-tailed test (null hypothesis - the  median  is  less  than  or
'          equal to the given value)
'
'    Requirements:
'        * the scale of measurement should be ordinal, interval or ratio  (i.e.
'          the test could not be applied to nominal variables).
'
'    The test is non-parametric and doesn't require distribution X to be normal
'
'    Input parameters:
'        X       -   sample. Array whose index goes from 0 to N-1.
'        N       -   size of the sample.
'        Median  -   assumed median value.
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'    While   calculating   p-values   high-precision   binomial    distribution
'    approximation is used, so significance levels have about 15 exact digits.
'
'      -- ALGLIB --
'         Copyright 08.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub onesamplesigntest(x As Double(), n As Integer, median As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		stest.onesamplesigntest(x, n, median, bothtails, lefttail, righttail)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    One-sample t-test
'
'    This test checks three hypotheses about the mean of the given sample.  The
'    following tests are performed:
'        * two-tailed test (null hypothesis - the mean is equal  to  the  given
'          value)
'        * left-tailed test (null hypothesis - the  mean  is  greater  than  or
'          equal to the given value)
'        * right-tailed test (null hypothesis - the mean is less than or  equal
'          to the given value).
'
'    The test is based on the assumption that  a  given  sample  has  a  normal
'    distribution and  an  unknown  dispersion.  If  the  distribution  sharply
'    differs from normal, the test will work incorrectly.
'
'    INPUT PARAMETERS:
'        X       -   sample. Array whose index goes from 0 to N-1.
'        N       -   size of sample, N>=0
'        Mean    -   assumed value of the mean.
'
'    OUTPUT PARAMETERS:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'    NOTE: this function correctly handles degenerate cases:
'          * when N=0, all p-values are set to 1.0
'          * when variance of X[] is exactly zero, p-values are set
'            to 1.0 or 0.0, depending on difference between sample mean and
'            value of mean being tested.
'
'
'      -- ALGLIB --
'         Copyright 08.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub studentttest1(x As Double(), n As Integer, mean As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		studentttests.studentttest1(x, n, mean, bothtails, lefttail, righttail)
		Return
	End Sub

	'************************************************************************
'    Two-sample pooled test
'
'    This test checks three hypotheses about the mean of the given samples. The
'    following tests are performed:
'        * two-tailed test (null hypothesis - the means are equal)
'        * left-tailed test (null hypothesis - the mean of the first sample  is
'          greater than or equal to the mean of the second sample)
'        * right-tailed test (null hypothesis - the mean of the first sample is
'          less than or equal to the mean of the second sample).
'
'    Test is based on the following assumptions:
'        * given samples have normal distributions
'        * dispersions are equal
'        * samples are independent.
'
'    Input parameters:
'        X       -   sample 1. Array whose index goes from 0 to N-1.
'        N       -   size of sample.
'        Y       -   sample 2. Array whose index goes from 0 to M-1.
'        M       -   size of sample.
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'    NOTE: this function correctly handles degenerate cases:
'          * when N=0 or M=0, all p-values are set to 1.0
'          * when both samples has exactly zero variance, p-values are set
'            to 1.0 or 0.0, depending on difference between means.
'
'      -- ALGLIB --
'         Copyright 18.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub studentttest2(x As Double(), n As Integer, y As Double(), m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, _
		ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		studentttests.studentttest2(x, n, y, m, bothtails, lefttail, _
			righttail)
		Return
	End Sub

	'************************************************************************
'    Two-sample unpooled test
'
'    This test checks three hypotheses about the mean of the given samples. The
'    following tests are performed:
'        * two-tailed test (null hypothesis - the means are equal)
'        * left-tailed test (null hypothesis - the mean of the first sample  is
'          greater than or equal to the mean of the second sample)
'        * right-tailed test (null hypothesis - the mean of the first sample is
'          less than or equal to the mean of the second sample).
'
'    Test is based on the following assumptions:
'        * given samples have normal distributions
'        * samples are independent.
'    Equality of variances is NOT required.
'
'    Input parameters:
'        X - sample 1. Array whose index goes from 0 to N-1.
'        N - size of the sample.
'        Y - sample 2. Array whose index goes from 0 to M-1.
'        M - size of the sample.
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'    NOTE: this function correctly handles degenerate cases:
'          * when N=0 or M=0, all p-values are set to 1.0
'          * when both samples has zero variance, p-values are set
'            to 1.0 or 0.0, depending on difference between means.
'          * when only one sample has zero variance, test reduces to 1-sample
'            version.
'
'      -- ALGLIB --
'         Copyright 18.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub unequalvariancettest(x As Double(), n As Integer, y As Double(), m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, _
		ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		studentttests.unequalvariancettest(x, n, y, m, bothtails, lefttail, _
			righttail)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Two-sample F-test
'
'    This test checks three hypotheses about dispersions of the given  samples.
'    The following tests are performed:
'        * two-tailed test (null hypothesis - the dispersions are equal)
'        * left-tailed test (null hypothesis  -  the  dispersion  of  the first
'          sample is greater than or equal to  the  dispersion  of  the  second
'          sample).
'        * right-tailed test (null hypothesis - the  dispersion  of  the  first
'          sample is less than or equal to the dispersion of the second sample)
'
'    The test is based on the following assumptions:
'        * the given samples have normal distributions
'        * the samples are independent.
'
'    Input parameters:
'        X   -   sample 1. Array whose index goes from 0 to N-1.
'        N   -   sample size.
'        Y   -   sample 2. Array whose index goes from 0 to M-1.
'        M   -   sample size.
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'      -- ALGLIB --
'         Copyright 19.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub ftest(x As Double(), n As Integer, y As Double(), m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, _
		ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		variancetests.ftest(x, n, y, m, bothtails, lefttail, _
			righttail)
		Return
	End Sub

	'************************************************************************
'    One-sample chi-square test
'
'    This test checks three hypotheses about the dispersion of the given sample
'    The following tests are performed:
'        * two-tailed test (null hypothesis - the dispersion equals  the  given
'          number)
'        * left-tailed test (null hypothesis - the dispersion is  greater  than
'          or equal to the given number)
'        * right-tailed test (null hypothesis  -  dispersion is  less  than  or
'          equal to the given number).
'
'    Test is based on the following assumptions:
'        * the given sample has a normal distribution.
'
'    Input parameters:
'        X           -   sample 1. Array whose index goes from 0 to N-1.
'        N           -   size of the sample.
'        Variance    -   dispersion value to compare with.
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'      -- ALGLIB --
'         Copyright 19.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub onesamplevariancetest(x As Double(), n As Integer, variance As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		variancetests.onesamplevariancetest(x, n, variance, bothtails, lefttail, righttail)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Wilcoxon signed-rank test
'
'    This test checks three hypotheses about the median  of  the  given sample.
'    The following tests are performed:
'        * two-tailed test (null hypothesis - the median is equal to the  given
'          value)
'        * left-tailed test (null hypothesis - the median is  greater  than  or
'          equal to the given value)
'        * right-tailed test (null hypothesis  -  the  median  is  less than or
'          equal to the given value)
'
'    Requirements:
'        * the scale of measurement should be ordinal, interval or  ratio (i.e.
'          the test could not be applied to nominal variables).
'        * the distribution should be continuous and symmetric relative to  its
'          median.
'        * number of distinct values in the X array should be greater than 4
'
'    The test is non-parametric and doesn't require distribution X to be normal
'
'    Input parameters:
'        X       -   sample. Array whose index goes from 0 to N-1.
'        N       -   size of the sample.
'        Median  -   assumed median value.
'
'    Output parameters:
'        BothTails   -   p-value for two-tailed test.
'                        If BothTails is less than the given significance level
'                        the null hypothesis is rejected.
'        LeftTail    -   p-value for left-tailed test.
'                        If LeftTail is less than the given significance level,
'                        the null hypothesis is rejected.
'        RightTail   -   p-value for right-tailed test.
'                        If RightTail is less than the given significance level
'                        the null hypothesis is rejected.
'
'    To calculate p-values, special approximation is used. This method lets  us
'    calculate p-values with two decimal places in interval [0.0001, 1].
'
'    "Two decimal places" does not sound very impressive, but in  practice  the
'    relative error of less than 1% is enough to make a decision.
'
'    There is no approximation outside the [0.0001, 1] interval. Therefore,  if
'    the significance level outlies this interval, the test returns 0.0001.
'
'      -- ALGLIB --
'         Copyright 08.09.2006 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub wilcoxonsignedranktest(x As Double(), n As Integer, e As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
		bothtails = 0
		lefttail = 0
		righttail = 0
		wsr.wilcoxonsignedranktest(x, n, e, bothtails, lefttail, righttail)
		Return
	End Sub

End Class
Public Partial Class alglib
	Public Class basestat
		'************************************************************************
'        Calculation of the distribution moments: mean, variance, skewness, kurtosis.
'
'        INPUT PARAMETERS:
'            X       -   sample
'            N       -   N>=0, sample size:
'                        * if given, only leading N elements of X are processed
'                        * if not given, automatically determined from size of X
'            
'        OUTPUT PARAMETERS
'            Mean    -   mean.
'            Variance-   variance.
'            Skewness-   skewness (if variance<>0; zero otherwise).
'            Kurtosis-   kurtosis (if variance<>0; zero otherwise).
'
'
'          -- ALGLIB --
'             Copyright 06.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub samplemoments(x As Double(), n As Integer, ByRef mean As Double, ByRef variance As Double, ByRef skewness As Double, ByRef kurtosis As Double)
			Dim i As Integer = 0
			Dim v As Double = 0
			Dim v1 As Double = 0
			Dim v2 As Double = 0
			Dim stddev As Double = 0

			mean = 0
			variance = 0
			skewness = 0
			kurtosis = 0

			alglib.ap.assert(n >= 0, "SampleMoments: N<0")
			alglib.ap.assert(alglib.ap.len(x) >= n, "SampleMoments: Length(X)<N!")
			alglib.ap.assert(apserv.isfinitevector(x, n), "SampleMoments: X is not finite vector")

			'
			' Init, special case 'N=0'
			'
			mean = 0
			variance = 0
			skewness = 0
			kurtosis = 0
			stddev = 0
			If n <= 0 Then
				Return
			End If

			'
			' Mean
			'
			For i = 0 To n - 1
				mean = mean + x(i)
			Next
			mean = mean / n

			'
			' Variance (using corrected two-pass algorithm)
			'
			If n <> 1 Then
				v1 = 0
				For i = 0 To n - 1
					v1 = v1 + Math.sqr(x(i) - mean)
				Next
				v2 = 0
				For i = 0 To n - 1
					v2 = v2 + (x(i) - mean)
				Next
				v2 = Math.sqr(v2) / n
				variance = (v1 - v2) / (n - 1)
				If CDbl(variance) < CDbl(0) Then
					variance = 0
				End If
				stddev = System.Math.sqrt(variance)
			End If

			'
			' Skewness and kurtosis
			'
			If CDbl(stddev) <> CDbl(0) Then
				For i = 0 To n - 1
					v = (x(i) - mean) / stddev
					v2 = Math.sqr(v)
					skewness = skewness + v2 * v
					kurtosis = kurtosis + Math.sqr(v2)
				Next
				skewness = skewness / n
				kurtosis = kurtosis / n - 3
			End If
		End Sub


		'************************************************************************
'        Calculation of the mean.
'
'        INPUT PARAMETERS:
'            X       -   sample
'            N       -   N>=0, sample size:
'                        * if given, only leading N elements of X are processed
'                        * if not given, automatically determined from size of X
'
'        NOTE:
'                        
'        This function return result  which calculated by 'SampleMoments' function
'        and stored at 'Mean' variable.
'
'
'          -- ALGLIB --
'             Copyright 06.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function samplemean(x As Double(), n As Integer) As Double
			Dim result As Double = 0
			Dim mean As Double = 0
			Dim tmp0 As Double = 0
			Dim tmp1 As Double = 0
			Dim tmp2 As Double = 0

			samplemoments(x, n, mean, tmp0, tmp1, tmp2)
			result = mean
			Return result
		End Function


		'************************************************************************
'        Calculation of the variance.
'
'        INPUT PARAMETERS:
'            X       -   sample
'            N       -   N>=0, sample size:
'                        * if given, only leading N elements of X are processed
'                        * if not given, automatically determined from size of X
'
'        NOTE:
'                        
'        This function return result  which calculated by 'SampleMoments' function
'        and stored at 'Variance' variable.
'
'
'          -- ALGLIB --
'             Copyright 06.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function samplevariance(x As Double(), n As Integer) As Double
			Dim result As Double = 0
			Dim variance As Double = 0
			Dim tmp0 As Double = 0
			Dim tmp1 As Double = 0
			Dim tmp2 As Double = 0

			samplemoments(x, n, tmp0, variance, tmp1, tmp2)
			result = variance
			Return result
		End Function


		'************************************************************************
'        Calculation of the skewness.
'
'        INPUT PARAMETERS:
'            X       -   sample
'            N       -   N>=0, sample size:
'                        * if given, only leading N elements of X are processed
'                        * if not given, automatically determined from size of X
'
'        NOTE:
'                        
'        This function return result  which calculated by 'SampleMoments' function
'        and stored at 'Skewness' variable.
'
'
'          -- ALGLIB --
'             Copyright 06.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function sampleskewness(x As Double(), n As Integer) As Double
			Dim result As Double = 0
			Dim skewness As Double = 0
			Dim tmp0 As Double = 0
			Dim tmp1 As Double = 0
			Dim tmp2 As Double = 0

			samplemoments(x, n, tmp0, tmp1, skewness, tmp2)
			result = skewness
			Return result
		End Function


		'************************************************************************
'        Calculation of the kurtosis.
'
'        INPUT PARAMETERS:
'            X       -   sample
'            N       -   N>=0, sample size:
'                        * if given, only leading N elements of X are processed
'                        * if not given, automatically determined from size of X
'
'        NOTE:
'                        
'        This function return result  which calculated by 'SampleMoments' function
'        and stored at 'Kurtosis' variable.
'
'
'          -- ALGLIB --
'             Copyright 06.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function samplekurtosis(x As Double(), n As Integer) As Double
			Dim result As Double = 0
			Dim kurtosis As Double = 0
			Dim tmp0 As Double = 0
			Dim tmp1 As Double = 0
			Dim tmp2 As Double = 0

			samplemoments(x, n, tmp0, tmp1, tmp2, kurtosis)
			result = kurtosis
			Return result
		End Function


		'************************************************************************
'        ADev
'
'        Input parameters:
'            X   -   sample
'            N   -   N>=0, sample size:
'                    * if given, only leading N elements of X are processed
'                    * if not given, automatically determined from size of X
'            
'        Output parameters:
'            ADev-   ADev
'
'          -- ALGLIB --
'             Copyright 06.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub sampleadev(x As Double(), n As Integer, ByRef adev As Double)
			Dim i As Integer = 0
			Dim mean As Double = 0

			adev = 0

			alglib.ap.assert(n >= 0, "SampleADev: N<0")
			alglib.ap.assert(alglib.ap.len(x) >= n, "SampleADev: Length(X)<N!")
			alglib.ap.assert(apserv.isfinitevector(x, n), "SampleADev: X is not finite vector")

			'
			' Init, handle N=0
			'
			mean = 0
			adev = 0
			If n <= 0 Then
				Return
			End If

			'
			' Mean
			'
			For i = 0 To n - 1
				mean = mean + x(i)
			Next
			mean = mean / n

			'
			' ADev
			'
			For i = 0 To n - 1
				adev = adev + System.Math.Abs(x(i) - mean)
			Next
			adev = adev / n
		End Sub


		'************************************************************************
'        Median calculation.
'
'        Input parameters:
'            X   -   sample (array indexes: [0..N-1])
'            N   -   N>=0, sample size:
'                    * if given, only leading N elements of X are processed
'                    * if not given, automatically determined from size of X
'
'        Output parameters:
'            Median
'
'          -- ALGLIB --
'             Copyright 06.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub samplemedian(x As Double(), n As Integer, ByRef median As Double)
			Dim i As Integer = 0
			Dim ir As Integer = 0
			Dim j As Integer = 0
			Dim l As Integer = 0
			Dim midp As Integer = 0
			Dim k As Integer = 0
			Dim a As Double = 0
			Dim tval As Double = 0

			x = DirectCast(x.Clone(), Double())
			median = 0

			alglib.ap.assert(n >= 0, "SampleMedian: N<0")
			alglib.ap.assert(alglib.ap.len(x) >= n, "SampleMedian: Length(X)<N!")
			alglib.ap.assert(apserv.isfinitevector(x, n), "SampleMedian: X is not finite vector")

			'
			' Some degenerate cases
			'
			median = 0
			If n <= 0 Then
				Return
			End If
			If n = 1 Then
				median = x(0)
				Return
			End If
			If n = 2 Then
				median = 0.5 * (x(0) + x(1))
				Return
			End If

			'
			' Common case, N>=3.
			' Choose X[(N-1)/2]
			'
			l = 0
			ir = n - 1
			k = (n - 1) \ 2
			While True
				If ir <= l + 1 Then

					'
					' 1 or 2 elements in partition
					'
					If ir = l + 1 AndAlso CDbl(x(ir)) < CDbl(x(l)) Then
						tval = x(l)
						x(l) = x(ir)
						x(ir) = tval
					End If
					Exit While
				Else
					midp = (l + ir) \ 2
					tval = x(midp)
					x(midp) = x(l + 1)
					x(l + 1) = tval
					If CDbl(x(l)) > CDbl(x(ir)) Then
						tval = x(l)
						x(l) = x(ir)
						x(ir) = tval
					End If
					If CDbl(x(l + 1)) > CDbl(x(ir)) Then
						tval = x(l + 1)
						x(l + 1) = x(ir)
						x(ir) = tval
					End If
					If CDbl(x(l)) > CDbl(x(l + 1)) Then
						tval = x(l)
						x(l) = x(l + 1)
						x(l + 1) = tval
					End If
					i = l + 1
					j = ir
					a = x(l + 1)
					While True
						Do
							i = i + 1
						Loop While CDbl(x(i)) < CDbl(a)
						Do
							j = j - 1
						Loop While CDbl(x(j)) > CDbl(a)
						If j < i Then
							Exit While
						End If
						tval = x(i)
						x(i) = x(j)
						x(j) = tval
					End While
					x(l + 1) = x(j)
					x(j) = a
					If j >= k Then
						ir = j - 1
					End If
					If j <= k Then
						l = i
					End If
				End If
			End While

			'
			' If N is odd, return result
			'
			If n Mod 2 = 1 Then
				median = x(k)
				Return
			End If
			a = x(n - 1)
			For i = k + 1 To n - 1
				If CDbl(x(i)) < CDbl(a) Then
					a = x(i)
				End If
			Next
			median = 0.5 * (x(k) + a)
		End Sub


		'************************************************************************
'        Percentile calculation.
'
'        Input parameters:
'            X   -   sample (array indexes: [0..N-1])
'            N   -   N>=0, sample size:
'                    * if given, only leading N elements of X are processed
'                    * if not given, automatically determined from size of X
'            P   -   percentile (0<=P<=1)
'
'        Output parameters:
'            V   -   percentile
'
'          -- ALGLIB --
'             Copyright 01.03.2008 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub samplepercentile(x As Double(), n As Integer, p As Double, ByRef v As Double)
			Dim i1 As Integer = 0
			Dim t As Double = 0
			Dim rbuf As Double() = New Double(-1) {}

			x = DirectCast(x.Clone(), Double())
			v = 0

			alglib.ap.assert(n >= 0, "SamplePercentile: N<0")
			alglib.ap.assert(alglib.ap.len(x) >= n, "SamplePercentile: Length(X)<N!")
			alglib.ap.assert(apserv.isfinitevector(x, n), "SamplePercentile: X is not finite vector")
            alglib.ap.assert(Math.isfinite(p), "SamplePercentile: incorrect P!")
			alglib.ap.assert(CDbl(p) >= CDbl(0) AndAlso CDbl(p) <= CDbl(1), "SamplePercentile: incorrect P!")
			tsort.tagsortfast(x, rbuf, n)
			If CDbl(p) = CDbl(0) Then
				v = x(0)
				Return
			End If
			If CDbl(p) = CDbl(1) Then
				v = x(n - 1)
				Return
			End If
			t = p * (n - 1)
			i1 = CInt(System.Math.Truncate(System.Math.Floor(t)))
			t = t - CInt(System.Math.Truncate(System.Math.Floor(t)))
			v = x(i1) * (1 - t) + x(i1 + 1) * t
		End Sub


		'************************************************************************
'        2-sample covariance
'
'        Input parameters:
'            X       -   sample 1 (array indexes: [0..N-1])
'            Y       -   sample 2 (array indexes: [0..N-1])
'            N       -   N>=0, sample size:
'                        * if given, only N leading elements of X/Y are processed
'                        * if not given, automatically determined from input sizes
'
'        Result:
'            covariance (zero for N=0 or N=1)
'
'          -- ALGLIB --
'             Copyright 28.10.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function cov2(x As Double(), y As Double(), n As Integer) As Double
			Dim result As Double = 0
			Dim i As Integer = 0
			Dim xmean As Double = 0
			Dim ymean As Double = 0
			Dim v As Double = 0
			Dim x0 As Double = 0
			Dim y0 As Double = 0
			Dim s As Double = 0
			Dim samex As New Boolean()
			Dim samey As New Boolean()

			alglib.ap.assert(n >= 0, "Cov2: N<0")
			alglib.ap.assert(alglib.ap.len(x) >= n, "Cov2: Length(X)<N!")
			alglib.ap.assert(alglib.ap.len(y) >= n, "Cov2: Length(Y)<N!")
			alglib.ap.assert(apserv.isfinitevector(x, n), "Cov2: X is not finite vector")
			alglib.ap.assert(apserv.isfinitevector(y, n), "Cov2: Y is not finite vector")

			'
			' Special case
			'
			If n <= 1 Then
				result = 0
				Return result
			End If

			'
			' Calculate mean.
			'
			'
			' Additonally we calculate SameX and SameY -
			' flag variables which are set to True when
			' all X[] (or Y[]) contain exactly same value.
			'
			' If at least one of them is True, we return zero
			' (othwerwise we risk to get nonzero covariation
			' because of roundoff).
			'
			xmean = 0
			ymean = 0
			samex = True
			samey = True
			x0 = x(0)
			y0 = y(0)
			v = CDbl(1) / CDbl(n)
			For i = 0 To n - 1
				s = x(i)
				samex = samex AndAlso CDbl(s) = CDbl(x0)
				xmean = xmean + s * v
				s = y(i)
				samey = samey AndAlso CDbl(s) = CDbl(y0)
				ymean = ymean + s * v
			Next
			If samex OrElse samey Then
				result = 0
				Return result
			End If

			'
			' covariance
			'
			v = CDbl(1) / CDbl(n - 1)
			result = 0
			For i = 0 To n - 1
				result = result + v * (x(i) - xmean) * (y(i) - ymean)
			Next
			Return result
		End Function


		'************************************************************************
'        Pearson product-moment correlation coefficient
'
'        Input parameters:
'            X       -   sample 1 (array indexes: [0..N-1])
'            Y       -   sample 2 (array indexes: [0..N-1])
'            N       -   N>=0, sample size:
'                        * if given, only N leading elements of X/Y are processed
'                        * if not given, automatically determined from input sizes
'
'        Result:
'            Pearson product-moment correlation coefficient
'            (zero for N=0 or N=1)
'
'          -- ALGLIB --
'             Copyright 28.10.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function pearsoncorr2(x As Double(), y As Double(), n As Integer) As Double
			Dim result As Double = 0
			Dim i As Integer = 0
			Dim xmean As Double = 0
			Dim ymean As Double = 0
			Dim v As Double = 0
			Dim x0 As Double = 0
			Dim y0 As Double = 0
			Dim s As Double = 0
			Dim samex As New Boolean()
			Dim samey As New Boolean()
			Dim xv As Double = 0
			Dim yv As Double = 0
			Dim t1 As Double = 0
			Dim t2 As Double = 0

			alglib.ap.assert(n >= 0, "PearsonCorr2: N<0")
			alglib.ap.assert(alglib.ap.len(x) >= n, "PearsonCorr2: Length(X)<N!")
			alglib.ap.assert(alglib.ap.len(y) >= n, "PearsonCorr2: Length(Y)<N!")
			alglib.ap.assert(apserv.isfinitevector(x, n), "PearsonCorr2: X is not finite vector")
			alglib.ap.assert(apserv.isfinitevector(y, n), "PearsonCorr2: Y is not finite vector")

			'
			' Special case
			'
			If n <= 1 Then
				result = 0
				Return result
			End If

			'
			' Calculate mean.
			'
			'
			' Additonally we calculate SameX and SameY -
			' flag variables which are set to True when
			' all X[] (or Y[]) contain exactly same value.
			'
			' If at least one of them is True, we return zero
			' (othwerwise we risk to get nonzero correlation
			' because of roundoff).
			'
			xmean = 0
			ymean = 0
			samex = True
			samey = True
			x0 = x(0)
			y0 = y(0)
			v = CDbl(1) / CDbl(n)
			For i = 0 To n - 1
				s = x(i)
				samex = samex AndAlso CDbl(s) = CDbl(x0)
				xmean = xmean + s * v
				s = y(i)
				samey = samey AndAlso CDbl(s) = CDbl(y0)
				ymean = ymean + s * v
			Next
			If samex OrElse samey Then
				result = 0
				Return result
			End If

			'
			' numerator and denominator
			'
			s = 0
			xv = 0
			yv = 0
			For i = 0 To n - 1
				t1 = x(i) - xmean
				t2 = y(i) - ymean
				xv = xv + Math.sqr(t1)
				yv = yv + Math.sqr(t2)
				s = s + t1 * t2
			Next
			If CDbl(xv) = CDbl(0) OrElse CDbl(yv) = CDbl(0) Then
				result = 0
			Else
				result = s / (System.Math.sqrt(xv) * System.Math.sqrt(yv))
			End If
			Return result
		End Function


		'************************************************************************
'        Spearman's rank correlation coefficient
'
'        Input parameters:
'            X       -   sample 1 (array indexes: [0..N-1])
'            Y       -   sample 2 (array indexes: [0..N-1])
'            N       -   N>=0, sample size:
'                        * if given, only N leading elements of X/Y are processed
'                        * if not given, automatically determined from input sizes
'
'        Result:
'            Spearman's rank correlation coefficient
'            (zero for N=0 or N=1)
'
'          -- ALGLIB --
'             Copyright 09.04.2007 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function spearmancorr2(x As Double(), y As Double(), n As Integer) As Double
			Dim result As Double = 0
			Dim buf As New apserv.apbuffers()

			x = DirectCast(x.Clone(), Double())
			y = DirectCast(y.Clone(), Double())

			alglib.ap.assert(n >= 0, "SpearmanCorr2: N<0")
			alglib.ap.assert(alglib.ap.len(x) >= n, "SpearmanCorr2: Length(X)<N!")
			alglib.ap.assert(alglib.ap.len(y) >= n, "SpearmanCorr2: Length(Y)<N!")
			alglib.ap.assert(apserv.isfinitevector(x, n), "SpearmanCorr2: X is not finite vector")
			alglib.ap.assert(apserv.isfinitevector(y, n), "SpearmanCorr2: Y is not finite vector")

			'
			' Special case
			'
			If n <= 1 Then
				result = 0
				Return result
			End If
			basicstatops.rankx(x, n, False, buf)
			basicstatops.rankx(y, n, False, buf)
			result = pearsoncorr2(x, y, n)
			Return result
		End Function


		'************************************************************************
'        Covariance matrix
'
'        SMP EDITION OF ALGLIB:
'
'          ! This function can utilize multicore capabilities of  your system.  In
'          ! order to do this you have to call version with "smp_" prefix,   which
'          ! indicates that multicore code will be used.
'          ! 
'          ! This note is given for users of SMP edition; if you use GPL  edition,
'          ! or commercial edition of ALGLIB without SMP support, you  still  will
'          ! be able to call smp-version of this function,  but  all  computations
'          ! will be done serially.
'          !
'          ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'          ! called 'SMP support', before using parallel version of this function.
'          !
'          ! You should remember that starting/stopping worker thread always  have
'          ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'          ! large problems, we do not recommend you to use it on small problems -
'          ! with covariance matrices smaller than 128*128.
'
'        INPUT PARAMETERS:
'            X   -   array[N,M], sample matrix:
'                    * J-th column corresponds to J-th variable
'                    * I-th row corresponds to I-th observation
'            N   -   N>=0, number of observations:
'                    * if given, only leading N rows of X are used
'                    * if not given, automatically determined from input size
'            M   -   M>0, number of variables:
'                    * if given, only leading M columns of X are used
'                    * if not given, automatically determined from input size
'
'        OUTPUT PARAMETERS:
'            C   -   array[M,M], covariance matrix (zero if N=0 or N=1)
'
'          -- ALGLIB --
'             Copyright 28.10.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub covm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim v As Double = 0
			Dim t As Double() = New Double(-1) {}
			Dim x0 As Double() = New Double(-1) {}
			Dim same As Boolean() = New Boolean(-1) {}
			Dim i_ As Integer = 0

			x = DirectCast(x.Clone(), Double(,))
			c = New Double(-1, -1) {}

			alglib.ap.assert(n >= 0, "CovM: N<0")
			alglib.ap.assert(m >= 1, "CovM: M<1")
			alglib.ap.assert(alglib.ap.rows(x) >= n, "CovM: Rows(X)<N!")
			alglib.ap.assert(alglib.ap.cols(x) >= m OrElse n = 0, "CovM: Cols(X)<M!")
			alglib.ap.assert(apserv.apservisfinitematrix(x, n, m), "CovM: X contains infinite/NAN elements")

			'
			' N<=1, return zero
			'
			If n <= 1 Then
				c = New Double(m - 1, m - 1) {}
				For i = 0 To m - 1
					For j = 0 To m - 1
						c(i, j) = 0
					Next
				Next
				Return
			End If

			'
			' Calculate means,
			' check for constant columns
			'
			t = New Double(m - 1) {}
			x0 = New Double(m - 1) {}
			same = New Boolean(m - 1) {}
			c = New Double(m - 1, m - 1) {}
			For i = 0 To m - 1
				t(i) = 0
				same(i) = True
			Next
			For i_ = 0 To m - 1
				x0(i_) = x(0, i_)
			Next
			v = CDbl(1) / CDbl(n)
			For i = 0 To n - 1
				For i_ = 0 To m - 1
					t(i_) = t(i_) + v * x(i, i_)
				Next
				For j = 0 To m - 1
					same(j) = same(j) AndAlso CDbl(x(i, j)) = CDbl(x0(j))
				Next
			Next

			'
			' * center variables;
			' * if we have constant columns, these columns are
			'   artificially zeroed (they must be zero in exact arithmetics,
			'   but unfortunately floating point ops are not exact).
			' * calculate upper half of symmetric covariance matrix
			'
			For i = 0 To n - 1
				For i_ = 0 To m - 1
					x(i, i_) = x(i, i_) - t(i_)
				Next
				For j = 0 To m - 1
					If same(j) Then
						x(i, j) = 0
					End If
				Next
			Next
			ablas.rmatrixsyrk(m, n, CDbl(1) / CDbl(n - 1), x, 0, 0, _
				1, 0.0, c, 0, 0, True)
			ablas.rmatrixenforcesymmetricity(c, m, True)
		End Sub


		'************************************************************************
'        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
'        ************************************************************************

		Public Shared Sub _pexec_covm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
			covm(x, n, m, c)
		End Sub


		'************************************************************************
'        Pearson product-moment correlation matrix
'
'        SMP EDITION OF ALGLIB:
'
'          ! This function can utilize multicore capabilities of  your system.  In
'          ! order to do this you have to call version with "smp_" prefix,   which
'          ! indicates that multicore code will be used.
'          ! 
'          ! This note is given for users of SMP edition; if you use GPL  edition,
'          ! or commercial edition of ALGLIB without SMP support, you  still  will
'          ! be able to call smp-version of this function,  but  all  computations
'          ! will be done serially.
'          !
'          ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'          ! called 'SMP support', before using parallel version of this function.
'          !
'          ! You should remember that starting/stopping worker thread always  have
'          ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'          ! large problems, we do not recommend you to use it on small problems -
'          ! with correlation matrices smaller than 128*128.
'
'        INPUT PARAMETERS:
'            X   -   array[N,M], sample matrix:
'                    * J-th column corresponds to J-th variable
'                    * I-th row corresponds to I-th observation
'            N   -   N>=0, number of observations:
'                    * if given, only leading N rows of X are used
'                    * if not given, automatically determined from input size
'            M   -   M>0, number of variables:
'                    * if given, only leading M columns of X are used
'                    * if not given, automatically determined from input size
'
'        OUTPUT PARAMETERS:
'            C   -   array[M,M], correlation matrix (zero if N=0 or N=1)
'
'          -- ALGLIB --
'             Copyright 28.10.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub pearsoncorrm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
			Dim t As Double() = New Double(-1) {}
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim v As Double = 0

			c = New Double(-1, -1) {}

			alglib.ap.assert(n >= 0, "PearsonCorrM: N<0")
			alglib.ap.assert(m >= 1, "PearsonCorrM: M<1")
			alglib.ap.assert(alglib.ap.rows(x) >= n, "PearsonCorrM: Rows(X)<N!")
			alglib.ap.assert(alglib.ap.cols(x) >= m OrElse n = 0, "PearsonCorrM: Cols(X)<M!")
			alglib.ap.assert(apserv.apservisfinitematrix(x, n, m), "PearsonCorrM: X contains infinite/NAN elements")
			t = New Double(m - 1) {}
			covm(x, n, m, c)
			For i = 0 To m - 1
				If CDbl(c(i, i)) > CDbl(0) Then
					t(i) = 1 / System.Math.sqrt(c(i, i))
				Else
					t(i) = 0.0
				End If
			Next
			For i = 0 To m - 1
				v = t(i)
				For j = 0 To m - 1
					c(i, j) = c(i, j) * v * t(j)
				Next
			Next
		End Sub


		'************************************************************************
'        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
'        ************************************************************************

		Public Shared Sub _pexec_pearsoncorrm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
			pearsoncorrm(x, n, m, c)
		End Sub


		'************************************************************************
'        Spearman's rank correlation matrix
'
'        SMP EDITION OF ALGLIB:
'
'          ! This function can utilize multicore capabilities of  your system.  In
'          ! order to do this you have to call version with "smp_" prefix,   which
'          ! indicates that multicore code will be used.
'          ! 
'          ! This note is given for users of SMP edition; if you use GPL  edition,
'          ! or commercial edition of ALGLIB without SMP support, you  still  will
'          ! be able to call smp-version of this function,  but  all  computations
'          ! will be done serially.
'          !
'          ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'          ! called 'SMP support', before using parallel version of this function.
'          !
'          ! You should remember that starting/stopping worker thread always  have
'          ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'          ! large problems, we do not recommend you to use it on small problems -
'          ! with correlation matrices smaller than 128*128.
'
'        INPUT PARAMETERS:
'            X   -   array[N,M], sample matrix:
'                    * J-th column corresponds to J-th variable
'                    * I-th row corresponds to I-th observation
'            N   -   N>=0, number of observations:
'                    * if given, only leading N rows of X are used
'                    * if not given, automatically determined from input size
'            M   -   M>0, number of variables:
'                    * if given, only leading M columns of X are used
'                    * if not given, automatically determined from input size
'
'        OUTPUT PARAMETERS:
'            C   -   array[M,M], correlation matrix (zero if N=0 or N=1)
'
'          -- ALGLIB --
'             Copyright 28.10.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub spearmancorrm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim buf As New apserv.apbuffers()
			Dim xc As Double(,) = New Double(-1, -1) {}
			Dim t As Double() = New Double(-1) {}
			Dim v As Double = 0
			Dim vv As Double = 0
			Dim x0 As Double = 0
			Dim b As New Boolean()

			c = New Double(-1, -1) {}

			alglib.ap.assert(n >= 0, "SpearmanCorrM: N<0")
			alglib.ap.assert(m >= 1, "SpearmanCorrM: M<1")
			alglib.ap.assert(alglib.ap.rows(x) >= n, "SpearmanCorrM: Rows(X)<N!")
			alglib.ap.assert(alglib.ap.cols(x) >= m OrElse n = 0, "SpearmanCorrM: Cols(X)<M!")
			alglib.ap.assert(apserv.apservisfinitematrix(x, n, m), "SpearmanCorrM: X contains infinite/NAN elements")

			'
			' N<=1, return zero
			'
			If n <= 1 Then
				c = New Double(m - 1, m - 1) {}
				For i = 0 To m - 1
					For j = 0 To m - 1
						c(i, j) = 0
					Next
				Next
				Return
			End If

			'
			' Allocate
			'
			t = New Double(System.Math.Max(n, m) - 1) {}
			c = New Double(m - 1, m - 1) {}

			'
			' Replace data with ranks
			'
			xc = New Double(m - 1, n - 1) {}
			ablas.rmatrixtranspose(n, m, x, 0, 0, xc, _
				0, 0)
			rankdata(xc, m, n)

			'
			' 1. Calculate means, check for constant columns
			' 2. Center variables, constant  columns are
			'   artificialy zeroed (they must be zero in exact arithmetics,
			'   but unfortunately floating point is not exact).
			'
			For i = 0 To m - 1

				'
				' Calculate:
				' * V - mean value of I-th variable
				' * B - True in case all variable values are same
				'
				v = 0
				b = True
				x0 = xc(i, 0)
				For j = 0 To n - 1
					vv = xc(i, j)
					v = v + vv
					b = b AndAlso CDbl(vv) = CDbl(x0)
				Next
				v = v / n

				'
				' Center/zero I-th variable
				'
				If b Then

					'
					' Zero
					'
					For j = 0 To n - 1
						xc(i, j) = 0.0
					Next
				Else

					'
					' Center
					'
					For j = 0 To n - 1
						xc(i, j) = xc(i, j) - v
					Next
				End If
			Next

			'
			' Calculate upper half of symmetric covariance matrix
			'
			ablas.rmatrixsyrk(m, n, CDbl(1) / CDbl(n - 1), xc, 0, 0, _
				0, 0.0, c, 0, 0, True)

			'
			' Calculate Pearson coefficients (upper triangle)
			'
			For i = 0 To m - 1
				If CDbl(c(i, i)) > CDbl(0) Then
					t(i) = 1 / System.Math.sqrt(c(i, i))
				Else
					t(i) = 0.0
				End If
			Next
			For i = 0 To m - 1
				v = t(i)
				For j = i To m - 1
					c(i, j) = c(i, j) * v * t(j)
				Next
			Next

			'
			' force symmetricity
			'
			ablas.rmatrixenforcesymmetricity(c, m, True)
		End Sub


		'************************************************************************
'        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
'        ************************************************************************

		Public Shared Sub _pexec_spearmancorrm(x As Double(,), n As Integer, m As Integer, ByRef c As Double(,))
			spearmancorrm(x, n, m, c)
		End Sub


		'************************************************************************
'        Cross-covariance matrix
'
'        SMP EDITION OF ALGLIB:
'
'          ! This function can utilize multicore capabilities of  your system.  In
'          ! order to do this you have to call version with "smp_" prefix,   which
'          ! indicates that multicore code will be used.
'          ! 
'          ! This note is given for users of SMP edition; if you use GPL  edition,
'          ! or commercial edition of ALGLIB without SMP support, you  still  will
'          ! be able to call smp-version of this function,  but  all  computations
'          ! will be done serially.
'          !
'          ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'          ! called 'SMP support', before using parallel version of this function.
'          !
'          ! You should remember that starting/stopping worker thread always  have
'          ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'          ! large problems, we do not recommend you to use it on small problems -
'          ! with covariance matrices smaller than 128*128.
'
'        INPUT PARAMETERS:
'            X   -   array[N,M1], sample matrix:
'                    * J-th column corresponds to J-th variable
'                    * I-th row corresponds to I-th observation
'            Y   -   array[N,M2], sample matrix:
'                    * J-th column corresponds to J-th variable
'                    * I-th row corresponds to I-th observation
'            N   -   N>=0, number of observations:
'                    * if given, only leading N rows of X/Y are used
'                    * if not given, automatically determined from input sizes
'            M1  -   M1>0, number of variables in X:
'                    * if given, only leading M1 columns of X are used
'                    * if not given, automatically determined from input size
'            M2  -   M2>0, number of variables in Y:
'                    * if given, only leading M1 columns of X are used
'                    * if not given, automatically determined from input size
'
'        OUTPUT PARAMETERS:
'            C   -   array[M1,M2], cross-covariance matrix (zero if N=0 or N=1)
'
'          -- ALGLIB --
'             Copyright 28.10.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub covm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim v As Double = 0
			Dim t As Double() = New Double(-1) {}
			Dim x0 As Double() = New Double(-1) {}
			Dim y0 As Double() = New Double(-1) {}
			Dim samex As Boolean() = New Boolean(-1) {}
			Dim samey As Boolean() = New Boolean(-1) {}
			Dim i_ As Integer = 0

			x = DirectCast(x.Clone(), Double(,))
			y = DirectCast(y.Clone(), Double(,))
			c = New Double(-1, -1) {}

			alglib.ap.assert(n >= 0, "CovM2: N<0")
			alglib.ap.assert(m1 >= 1, "CovM2: M1<1")
			alglib.ap.assert(m2 >= 1, "CovM2: M2<1")
			alglib.ap.assert(alglib.ap.rows(x) >= n, "CovM2: Rows(X)<N!")
			alglib.ap.assert(alglib.ap.cols(x) >= m1 OrElse n = 0, "CovM2: Cols(X)<M1!")
			alglib.ap.assert(apserv.apservisfinitematrix(x, n, m1), "CovM2: X contains infinite/NAN elements")
			alglib.ap.assert(alglib.ap.rows(y) >= n, "CovM2: Rows(Y)<N!")
			alglib.ap.assert(alglib.ap.cols(y) >= m2 OrElse n = 0, "CovM2: Cols(Y)<M2!")
			alglib.ap.assert(apserv.apservisfinitematrix(y, n, m2), "CovM2: X contains infinite/NAN elements")

			'
			' N<=1, return zero
			'
			If n <= 1 Then
				c = New Double(m1 - 1, m2 - 1) {}
				For i = 0 To m1 - 1
					For j = 0 To m2 - 1
						c(i, j) = 0
					Next
				Next
				Return
			End If

			'
			' Allocate
			'
			t = New Double(System.Math.Max(m1, m2) - 1) {}
			x0 = New Double(m1 - 1) {}
			y0 = New Double(m2 - 1) {}
			samex = New Boolean(m1 - 1) {}
			samey = New Boolean(m2 - 1) {}
			c = New Double(m1 - 1, m2 - 1) {}

			'
			' * calculate means of X
			' * center X
			' * if we have constant columns, these columns are
			'   artificially zeroed (they must be zero in exact arithmetics,
			'   but unfortunately floating point ops are not exact).
			'
			For i = 0 To m1 - 1
				t(i) = 0
				samex(i) = True
			Next
			For i_ = 0 To m1 - 1
				x0(i_) = x(0, i_)
			Next
			v = CDbl(1) / CDbl(n)
			For i = 0 To n - 1
				For i_ = 0 To m1 - 1
					t(i_) = t(i_) + v * x(i, i_)
				Next
				For j = 0 To m1 - 1
					samex(j) = samex(j) AndAlso CDbl(x(i, j)) = CDbl(x0(j))
				Next
			Next
			For i = 0 To n - 1
				For i_ = 0 To m1 - 1
					x(i, i_) = x(i, i_) - t(i_)
				Next
				For j = 0 To m1 - 1
					If samex(j) Then
						x(i, j) = 0
					End If
				Next
			Next

			'
			' Repeat same steps for Y
			'
			For i = 0 To m2 - 1
				t(i) = 0
				samey(i) = True
			Next
			For i_ = 0 To m2 - 1
				y0(i_) = y(0, i_)
			Next
			v = CDbl(1) / CDbl(n)
			For i = 0 To n - 1
				For i_ = 0 To m2 - 1
					t(i_) = t(i_) + v * y(i, i_)
				Next
				For j = 0 To m2 - 1
					samey(j) = samey(j) AndAlso CDbl(y(i, j)) = CDbl(y0(j))
				Next
			Next
			For i = 0 To n - 1
				For i_ = 0 To m2 - 1
					y(i, i_) = y(i, i_) - t(i_)
				Next
				For j = 0 To m2 - 1
					If samey(j) Then
						y(i, j) = 0
					End If
				Next
			Next

			'
			' calculate cross-covariance matrix
			'
			ablas.rmatrixgemm(m1, m2, n, CDbl(1) / CDbl(n - 1), x, 0, _
				0, 1, y, 0, 0, 0, _
				0.0, c, 0, 0)
		End Sub


		'************************************************************************
'        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
'        ************************************************************************

		Public Shared Sub _pexec_covm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
			covm2(x, y, n, m1, m2, c)
		End Sub


		'************************************************************************
'        Pearson product-moment cross-correlation matrix
'
'        SMP EDITION OF ALGLIB:
'
'          ! This function can utilize multicore capabilities of  your system.  In
'          ! order to do this you have to call version with "smp_" prefix,   which
'          ! indicates that multicore code will be used.
'          ! 
'          ! This note is given for users of SMP edition; if you use GPL  edition,
'          ! or commercial edition of ALGLIB without SMP support, you  still  will
'          ! be able to call smp-version of this function,  but  all  computations
'          ! will be done serially.
'          !
'          ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'          ! called 'SMP support', before using parallel version of this function.
'          !
'          ! You should remember that starting/stopping worker thread always  have
'          ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'          ! large problems, we do not recommend you to use it on small problems -
'          ! with correlation matrices smaller than 128*128.
'
'        INPUT PARAMETERS:
'            X   -   array[N,M1], sample matrix:
'                    * J-th column corresponds to J-th variable
'                    * I-th row corresponds to I-th observation
'            Y   -   array[N,M2], sample matrix:
'                    * J-th column corresponds to J-th variable
'                    * I-th row corresponds to I-th observation
'            N   -   N>=0, number of observations:
'                    * if given, only leading N rows of X/Y are used
'                    * if not given, automatically determined from input sizes
'            M1  -   M1>0, number of variables in X:
'                    * if given, only leading M1 columns of X are used
'                    * if not given, automatically determined from input size
'            M2  -   M2>0, number of variables in Y:
'                    * if given, only leading M1 columns of X are used
'                    * if not given, automatically determined from input size
'
'        OUTPUT PARAMETERS:
'            C   -   array[M1,M2], cross-correlation matrix (zero if N=0 or N=1)
'
'          -- ALGLIB --
'             Copyright 28.10.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub pearsoncorrm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim v As Double = 0
			Dim t As Double() = New Double(-1) {}
			Dim x0 As Double() = New Double(-1) {}
			Dim y0 As Double() = New Double(-1) {}
			Dim sx As Double() = New Double(-1) {}
			Dim sy As Double() = New Double(-1) {}
			Dim samex As Boolean() = New Boolean(-1) {}
			Dim samey As Boolean() = New Boolean(-1) {}
			Dim i_ As Integer = 0

			x = DirectCast(x.Clone(), Double(,))
			y = DirectCast(y.Clone(), Double(,))
			c = New Double(-1, -1) {}

			alglib.ap.assert(n >= 0, "PearsonCorrM2: N<0")
			alglib.ap.assert(m1 >= 1, "PearsonCorrM2: M1<1")
			alglib.ap.assert(m2 >= 1, "PearsonCorrM2: M2<1")
			alglib.ap.assert(alglib.ap.rows(x) >= n, "PearsonCorrM2: Rows(X)<N!")
			alglib.ap.assert(alglib.ap.cols(x) >= m1 OrElse n = 0, "PearsonCorrM2: Cols(X)<M1!")
			alglib.ap.assert(apserv.apservisfinitematrix(x, n, m1), "PearsonCorrM2: X contains infinite/NAN elements")
			alglib.ap.assert(alglib.ap.rows(y) >= n, "PearsonCorrM2: Rows(Y)<N!")
			alglib.ap.assert(alglib.ap.cols(y) >= m2 OrElse n = 0, "PearsonCorrM2: Cols(Y)<M2!")
			alglib.ap.assert(apserv.apservisfinitematrix(y, n, m2), "PearsonCorrM2: X contains infinite/NAN elements")

			'
			' N<=1, return zero
			'
			If n <= 1 Then
				c = New Double(m1 - 1, m2 - 1) {}
				For i = 0 To m1 - 1
					For j = 0 To m2 - 1
						c(i, j) = 0
					Next
				Next
				Return
			End If

			'
			' Allocate
			'
			t = New Double(System.Math.Max(m1, m2) - 1) {}
			x0 = New Double(m1 - 1) {}
			y0 = New Double(m2 - 1) {}
			sx = New Double(m1 - 1) {}
			sy = New Double(m2 - 1) {}
			samex = New Boolean(m1 - 1) {}
			samey = New Boolean(m2 - 1) {}
			c = New Double(m1 - 1, m2 - 1) {}

			'
			' * calculate means of X
			' * center X
			' * if we have constant columns, these columns are
			'   artificially zeroed (they must be zero in exact arithmetics,
			'   but unfortunately floating point ops are not exact).
			' * calculate column variances
			'
			For i = 0 To m1 - 1
				t(i) = 0
				samex(i) = True
				sx(i) = 0
			Next
			For i_ = 0 To m1 - 1
				x0(i_) = x(0, i_)
			Next
			v = CDbl(1) / CDbl(n)
			For i = 0 To n - 1
				For i_ = 0 To m1 - 1
					t(i_) = t(i_) + v * x(i, i_)
				Next
				For j = 0 To m1 - 1
					samex(j) = samex(j) AndAlso CDbl(x(i, j)) = CDbl(x0(j))
				Next
			Next
			For i = 0 To n - 1
				For i_ = 0 To m1 - 1
					x(i, i_) = x(i, i_) - t(i_)
				Next
				For j = 0 To m1 - 1
					If samex(j) Then
						x(i, j) = 0
					End If
					sx(j) = sx(j) + x(i, j) * x(i, j)
				Next
			Next
			For j = 0 To m1 - 1
				sx(j) = System.Math.sqrt(sx(j) / (n - 1))
			Next

			'
			' Repeat same steps for Y
			'
			For i = 0 To m2 - 1
				t(i) = 0
				samey(i) = True
				sy(i) = 0
			Next
			For i_ = 0 To m2 - 1
				y0(i_) = y(0, i_)
			Next
			v = CDbl(1) / CDbl(n)
			For i = 0 To n - 1
				For i_ = 0 To m2 - 1
					t(i_) = t(i_) + v * y(i, i_)
				Next
				For j = 0 To m2 - 1
					samey(j) = samey(j) AndAlso CDbl(y(i, j)) = CDbl(y0(j))
				Next
			Next
			For i = 0 To n - 1
				For i_ = 0 To m2 - 1
					y(i, i_) = y(i, i_) - t(i_)
				Next
				For j = 0 To m2 - 1
					If samey(j) Then
						y(i, j) = 0
					End If
					sy(j) = sy(j) + y(i, j) * y(i, j)
				Next
			Next
			For j = 0 To m2 - 1
				sy(j) = System.Math.sqrt(sy(j) / (n - 1))
			Next

			'
			' calculate cross-covariance matrix
			'
			ablas.rmatrixgemm(m1, m2, n, CDbl(1) / CDbl(n - 1), x, 0, _
				0, 1, y, 0, 0, 0, _
				0.0, c, 0, 0)

			'
			' Divide by standard deviations
			'
			For i = 0 To m1 - 1
				If CDbl(sx(i)) <> CDbl(0) Then
					sx(i) = 1 / sx(i)
				Else
					sx(i) = 0.0
				End If
			Next
			For i = 0 To m2 - 1
				If CDbl(sy(i)) <> CDbl(0) Then
					sy(i) = 1 / sy(i)
				Else
					sy(i) = 0.0
				End If
			Next
			For i = 0 To m1 - 1
				v = sx(i)
				For j = 0 To m2 - 1
					c(i, j) = c(i, j) * v * sy(j)
				Next
			Next
		End Sub


		'************************************************************************
'        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
'        ************************************************************************

		Public Shared Sub _pexec_pearsoncorrm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
			pearsoncorrm2(x, y, n, m1, m2, c)
		End Sub


		'************************************************************************
'        Spearman's rank cross-correlation matrix
'
'        SMP EDITION OF ALGLIB:
'
'          ! This function can utilize multicore capabilities of  your system.  In
'          ! order to do this you have to call version with "smp_" prefix,   which
'          ! indicates that multicore code will be used.
'          ! 
'          ! This note is given for users of SMP edition; if you use GPL  edition,
'          ! or commercial edition of ALGLIB without SMP support, you  still  will
'          ! be able to call smp-version of this function,  but  all  computations
'          ! will be done serially.
'          !
'          ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'          ! called 'SMP support', before using parallel version of this function.
'          !
'          ! You should remember that starting/stopping worker thread always  have
'          ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'          ! large problems, we do not recommend you to use it on small problems -
'          ! with correlation matrices smaller than 128*128.
'
'        INPUT PARAMETERS:
'            X   -   array[N,M1], sample matrix:
'                    * J-th column corresponds to J-th variable
'                    * I-th row corresponds to I-th observation
'            Y   -   array[N,M2], sample matrix:
'                    * J-th column corresponds to J-th variable
'                    * I-th row corresponds to I-th observation
'            N   -   N>=0, number of observations:
'                    * if given, only leading N rows of X/Y are used
'                    * if not given, automatically determined from input sizes
'            M1  -   M1>0, number of variables in X:
'                    * if given, only leading M1 columns of X are used
'                    * if not given, automatically determined from input size
'            M2  -   M2>0, number of variables in Y:
'                    * if given, only leading M1 columns of X are used
'                    * if not given, automatically determined from input size
'
'        OUTPUT PARAMETERS:
'            C   -   array[M1,M2], cross-correlation matrix (zero if N=0 or N=1)
'
'          -- ALGLIB --
'             Copyright 28.10.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub spearmancorrm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim v As Double = 0
			Dim v2 As Double = 0
			Dim vv As Double = 0
			Dim b As New Boolean()
			Dim t As Double() = New Double(-1) {}
			Dim x0 As Double = 0
			Dim y0 As Double = 0
			Dim sx As Double() = New Double(-1) {}
			Dim sy As Double() = New Double(-1) {}
			Dim xc As Double(,) = New Double(-1, -1) {}
			Dim yc As Double(,) = New Double(-1, -1) {}
			Dim buf As New apserv.apbuffers()

			c = New Double(-1, -1) {}

			alglib.ap.assert(n >= 0, "SpearmanCorrM2: N<0")
			alglib.ap.assert(m1 >= 1, "SpearmanCorrM2: M1<1")
			alglib.ap.assert(m2 >= 1, "SpearmanCorrM2: M2<1")
			alglib.ap.assert(alglib.ap.rows(x) >= n, "SpearmanCorrM2: Rows(X)<N!")
			alglib.ap.assert(alglib.ap.cols(x) >= m1 OrElse n = 0, "SpearmanCorrM2: Cols(X)<M1!")
			alglib.ap.assert(apserv.apservisfinitematrix(x, n, m1), "SpearmanCorrM2: X contains infinite/NAN elements")
			alglib.ap.assert(alglib.ap.rows(y) >= n, "SpearmanCorrM2: Rows(Y)<N!")
			alglib.ap.assert(alglib.ap.cols(y) >= m2 OrElse n = 0, "SpearmanCorrM2: Cols(Y)<M2!")
			alglib.ap.assert(apserv.apservisfinitematrix(y, n, m2), "SpearmanCorrM2: X contains infinite/NAN elements")

			'
			' N<=1, return zero
			'
			If n <= 1 Then
				c = New Double(m1 - 1, m2 - 1) {}
				For i = 0 To m1 - 1
					For j = 0 To m2 - 1
						c(i, j) = 0
					Next
				Next
				Return
			End If

			'
			' Allocate
			'
			t = New Double(System.Math.Max(System.Math.Max(m1, m2), n) - 1) {}
			sx = New Double(m1 - 1) {}
			sy = New Double(m2 - 1) {}
			c = New Double(m1 - 1, m2 - 1) {}

			'
			' Replace data with ranks
			'
			xc = New Double(m1 - 1, n - 1) {}
			yc = New Double(m2 - 1, n - 1) {}
			ablas.rmatrixtranspose(n, m1, x, 0, 0, xc, _
				0, 0)
			ablas.rmatrixtranspose(n, m2, y, 0, 0, yc, _
				0, 0)
			rankdata(xc, m1, n)
			rankdata(yc, m2, n)

			'
			' 1. Calculate means, variances, check for constant columns
			' 2. Center variables, constant  columns are
			'   artificialy zeroed (they must be zero in exact arithmetics,
			'   but unfortunately floating point is not exact).
			'
			' Description of variables:
			' * V - mean value of I-th variable
			' * V2- variance
			' * VV-temporary
			' * B - True in case all variable values are same
			'
			For i = 0 To m1 - 1
				v = 0
				v2 = 0.0
				b = True
				x0 = xc(i, 0)
				For j = 0 To n - 1
					vv = xc(i, j)
					v = v + vv
					b = b AndAlso CDbl(vv) = CDbl(x0)
				Next
				v = v / n
				If b Then
					For j = 0 To n - 1
						xc(i, j) = 0.0
					Next
				Else
					For j = 0 To n - 1
						vv = xc(i, j)
						xc(i, j) = vv - v
						v2 = v2 + (vv - v) * (vv - v)
					Next
				End If
				sx(i) = System.Math.sqrt(v2 / (n - 1))
			Next
			For i = 0 To m2 - 1
				v = 0
				v2 = 0.0
				b = True
				y0 = yc(i, 0)
				For j = 0 To n - 1
					vv = yc(i, j)
					v = v + vv
					b = b AndAlso CDbl(vv) = CDbl(y0)
				Next
				v = v / n
				If b Then
					For j = 0 To n - 1
						yc(i, j) = 0.0
					Next
				Else
					For j = 0 To n - 1
						vv = yc(i, j)
						yc(i, j) = vv - v
						v2 = v2 + (vv - v) * (vv - v)
					Next
				End If
				sy(i) = System.Math.sqrt(v2 / (n - 1))
			Next

			'
			' calculate cross-covariance matrix
			'
			ablas.rmatrixgemm(m1, m2, n, CDbl(1) / CDbl(n - 1), xc, 0, _
				0, 0, yc, 0, 0, 1, _
				0.0, c, 0, 0)

			'
			' Divide by standard deviations
			'
			For i = 0 To m1 - 1
				If CDbl(sx(i)) <> CDbl(0) Then
					sx(i) = 1 / sx(i)
				Else
					sx(i) = 0.0
				End If
			Next
			For i = 0 To m2 - 1
				If CDbl(sy(i)) <> CDbl(0) Then
					sy(i) = 1 / sy(i)
				Else
					sy(i) = 0.0
				End If
			Next
			For i = 0 To m1 - 1
				v = sx(i)
				For j = 0 To m2 - 1
					c(i, j) = c(i, j) * v * sy(j)
				Next
			Next
		End Sub


		'************************************************************************
'        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
'        ************************************************************************

		Public Shared Sub _pexec_spearmancorrm2(x As Double(,), y As Double(,), n As Integer, m1 As Integer, m2 As Integer, ByRef c As Double(,))
			spearmancorrm2(x, y, n, m1, m2, c)
		End Sub


		'************************************************************************
'        This function replaces data in XY by their ranks:
'        * XY is processed row-by-row
'        * rows are processed separately
'        * tied data are correctly handled (tied ranks are calculated)
'        * ranking starts from 0, ends at NFeatures-1
'        * sum of within-row values is equal to (NFeatures-1)*NFeatures/2
'
'        SMP EDITION OF ALGLIB:
'
'          ! This function can utilize multicore capabilities of  your system.  In
'          ! order to do this you have to call version with "smp_" prefix,   which
'          ! indicates that multicore code will be used.
'          ! 
'          ! This note is given for users of SMP edition; if you use GPL  edition,
'          ! or commercial edition of ALGLIB without SMP support, you  still  will
'          ! be able to call smp-version of this function,  but  all  computations
'          ! will be done serially.
'          !
'          ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'          ! called 'SMP support', before using parallel version of this function.
'          !
'          ! You should remember that starting/stopping worker thread always  have
'          ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'          ! large problems, we do not recommend you to use it on small problems -
'          ! ones where expected operations count is less than 100.000
'
'        INPUT PARAMETERS:
'            XY      -   array[NPoints,NFeatures], dataset
'            NPoints -   number of points
'            NFeatures-  number of features
'
'        OUTPUT PARAMETERS:
'            XY      -   data are replaced by their within-row ranks;
'                        ranking starts from 0, ends at NFeatures-1
'
'          -- ALGLIB --
'             Copyright 18.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub rankdata(xy As Double(,), npoints As Integer, nfeatures As Integer)
			Dim buf0 As New apserv.apbuffers()
			Dim buf1 As New apserv.apbuffers()
			Dim basecasecost As Integer = 0
			Dim pool As New alglib.smp.shared_pool()

			alglib.ap.assert(npoints >= 0, "RankData: NPoints<0")
			alglib.ap.assert(nfeatures >= 1, "RankData: NFeatures<1")
			alglib.ap.assert(alglib.ap.rows(xy) >= npoints, "RankData: Rows(XY)<NPoints")
			alglib.ap.assert(alglib.ap.cols(xy) >= nfeatures OrElse npoints = 0, "RankData: Cols(XY)<NFeatures")
			alglib.ap.assert(apserv.apservisfinitematrix(xy, npoints, nfeatures), "RankData: XY contains infinite/NAN elements")

			'
			' Basecase cost is a maximum cost of basecase problems.
			' Problems harded than that cost will be split.
			'
			' Problem cost is assumed to be NPoints*NFeatures*log2(NFeatures),
			' which is proportional, but NOT equal to number of FLOPs required
			' to solve problem.
			'
			basecasecost = 10000

			'
			' Try to use serial code, no SMP functionality, no shared pools.
			'
			If CDbl(apserv.inttoreal(npoints) * apserv.inttoreal(nfeatures) * apserv.logbase2(nfeatures)) < CDbl(basecasecost) Then
				rankdatabasecase(xy, 0, npoints, nfeatures, False, buf0, _
					buf1)
				Return
			End If

			'
			' Parallel code
			'
			alglib.smp.ae_shared_pool_set_seed(pool, buf0)
			rankdatarec(xy, 0, npoints, nfeatures, False, pool, _
				basecasecost)
		End Sub


		'************************************************************************
'        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
'        ************************************************************************

		Public Shared Sub _pexec_rankdata(xy As Double(,), npoints As Integer, nfeatures As Integer)
			rankdata(xy, npoints, nfeatures)
		End Sub


		'************************************************************************
'        This function replaces data in XY by their CENTERED ranks:
'        * XY is processed row-by-row
'        * rows are processed separately
'        * tied data are correctly handled (tied ranks are calculated)
'        * centered ranks are just usual ranks, but centered in such way  that  sum
'          of within-row values is equal to 0.0.
'        * centering is performed by subtracting mean from each row, i.e it changes
'          mean value, but does NOT change higher moments
'
'        SMP EDITION OF ALGLIB:
'
'          ! This function can utilize multicore capabilities of  your system.  In
'          ! order to do this you have to call version with "smp_" prefix,   which
'          ! indicates that multicore code will be used.
'          ! 
'          ! This note is given for users of SMP edition; if you use GPL  edition,
'          ! or commercial edition of ALGLIB without SMP support, you  still  will
'          ! be able to call smp-version of this function,  but  all  computations
'          ! will be done serially.
'          !
'          ! We recommend you to carefully read ALGLIB Reference  Manual,  section
'          ! called 'SMP support', before using parallel version of this function.
'          !
'          ! You should remember that starting/stopping worker thread always  have
'          ! non-zero cost. Although  multicore  version  is  pretty  efficient on
'          ! large problems, we do not recommend you to use it on small problems -
'          ! ones where expected operations count is less than 100.000
'
'        INPUT PARAMETERS:
'            XY      -   array[NPoints,NFeatures], dataset
'            NPoints -   number of points
'            NFeatures-  number of features
'
'        OUTPUT PARAMETERS:
'            XY      -   data are replaced by their within-row ranks;
'                        ranking starts from 0, ends at NFeatures-1
'
'          -- ALGLIB --
'             Copyright 18.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub rankdatacentered(xy As Double(,), npoints As Integer, nfeatures As Integer)
			Dim buf0 As New apserv.apbuffers()
			Dim buf1 As New apserv.apbuffers()
			Dim basecasecost As Integer = 0
			Dim pool As New alglib.smp.shared_pool()

			alglib.ap.assert(npoints >= 0, "RankData: NPoints<0")
			alglib.ap.assert(nfeatures >= 1, "RankData: NFeatures<1")
			alglib.ap.assert(alglib.ap.rows(xy) >= npoints, "RankData: Rows(XY)<NPoints")
			alglib.ap.assert(alglib.ap.cols(xy) >= nfeatures OrElse npoints = 0, "RankData: Cols(XY)<NFeatures")
			alglib.ap.assert(apserv.apservisfinitematrix(xy, npoints, nfeatures), "RankData: XY contains infinite/NAN elements")

			'
			' Basecase cost is a maximum cost of basecase problems.
			' Problems harded than that cost will be split.
			'
			' Problem cost is assumed to be NPoints*NFeatures*log2(NFeatures),
			' which is proportional, but NOT equal to number of FLOPs required
			' to solve problem.
			'
			basecasecost = 10000

			'
			' Try to use serial code, no SMP functionality, no shared pools.
			'
			If CDbl(apserv.inttoreal(npoints) * apserv.inttoreal(nfeatures) * apserv.logbase2(nfeatures)) < CDbl(basecasecost) Then
				rankdatabasecase(xy, 0, npoints, nfeatures, True, buf0, _
					buf1)
				Return
			End If

			'
			' Parallel code
			'
			alglib.smp.ae_shared_pool_set_seed(pool, buf0)
			rankdatarec(xy, 0, npoints, nfeatures, True, pool, _
				basecasecost)
		End Sub


		'************************************************************************
'        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
'        ************************************************************************

		Public Shared Sub _pexec_rankdatacentered(xy As Double(,), npoints As Integer, nfeatures As Integer)
			rankdatacentered(xy, npoints, nfeatures)
		End Sub


		'************************************************************************
'        Obsolete function, we recommend to use PearsonCorr2().
'
'          -- ALGLIB --
'             Copyright 09.04.2007 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function pearsoncorrelation(x As Double(), y As Double(), n As Integer) As Double
			Dim result As Double = 0

			result = pearsoncorr2(x, y, n)
			Return result
		End Function


		'************************************************************************
'        Obsolete function, we recommend to use SpearmanCorr2().
'
'            -- ALGLIB --
'            Copyright 09.04.2007 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function spearmanrankcorrelation(x As Double(), y As Double(), n As Integer) As Double
			Dim result As Double = 0

			result = spearmancorr2(x, y, n)
			Return result
		End Function


		'************************************************************************
'        Recurrent code for RankData(), splits problem into  subproblems  or  calls
'        basecase code (depending on problem complexity).
'
'        INPUT PARAMETERS:
'            XY      -   array[NPoints,NFeatures], dataset
'            I0      -   index of first row to process
'            I1      -   index of past-the-last row to process;
'                        this function processes half-interval [I0,I1).
'            NFeatures-  number of features
'            IsCentered- whether ranks are centered or not:
'                        * True      -   ranks are centered in such way that  their
'                                        within-row sum is zero
'                        * False     -   ranks are not centered
'            Pool    -   shared pool which holds temporary buffers
'                        (APBuffers structure)
'            BasecaseCost-minimum cost of the problem which will be split
'
'        OUTPUT PARAMETERS:
'            XY      -   data in [I0,I1) are replaced by their within-row ranks;
'                        ranking starts from 0, ends at NFeatures-1
'
'          -- ALGLIB --
'             Copyright 18.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub rankdatarec(xy As Double(,), i0 As Integer, i1 As Integer, nfeatures As Integer, iscentered As Boolean, pool As alglib.smp.shared_pool, _
			basecasecost As Integer)
			Dim buf0 As apserv.apbuffers = Nothing
			Dim buf1 As apserv.apbuffers = Nothing
			Dim problemcost As Double = 0
			Dim im As Integer = 0

			alglib.ap.assert(i1 >= i0, "RankDataRec: internal error")

			'
			' Recursively split problem, if it is too large
			'
			problemcost = apserv.inttoreal(i1 - i0) * apserv.inttoreal(nfeatures) * apserv.logbase2(nfeatures)
			If i1 - i0 >= 2 AndAlso CDbl(problemcost) > CDbl(basecasecost) Then
				im = (i1 + i0) \ 2
				rankdatarec(xy, i0, im, nfeatures, iscentered, pool, _
					basecasecost)
				rankdatarec(xy, im, i1, nfeatures, iscentered, pool, _
					basecasecost)
				Return
			End If

			'
			' Retrieve buffers from pool, call serial code, return buffers to pool
			'
			alglib.smp.ae_shared_pool_retrieve(pool, buf0)
			alglib.smp.ae_shared_pool_retrieve(pool, buf1)
			rankdatabasecase(xy, i0, i1, nfeatures, iscentered, buf0, _
				buf1)
			alglib.smp.ae_shared_pool_recycle(pool, buf0)
			alglib.smp.ae_shared_pool_recycle(pool, buf1)
		End Sub


		'************************************************************************
'        Basecase code for RankData(), performs actual work on subset of data using
'        temporary buffer passed as parameter.
'
'        INPUT PARAMETERS:
'            XY      -   array[NPoints,NFeatures], dataset
'            I0      -   index of first row to process
'            I1      -   index of past-the-last row to process;
'                        this function processes half-interval [I0,I1).
'            NFeatures-  number of features
'            IsCentered- whether ranks are centered or not:
'                        * True      -   ranks are centered in such way that  their
'                                        within-row sum is zero
'                        * False     -   ranks are not centered
'            Buf0    -   temporary buffers, may be empty (this function automatically
'                        allocates/reuses buffers).
'            Buf1    -   temporary buffers, may be empty (this function automatically
'                        allocates/reuses buffers).
'
'        OUTPUT PARAMETERS:
'            XY      -   data in [I0,I1) are replaced by their within-row ranks;
'                        ranking starts from 0, ends at NFeatures-1
'
'          -- ALGLIB --
'             Copyright 18.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub rankdatabasecase(xy As Double(,), i0 As Integer, i1 As Integer, nfeatures As Integer, iscentered As Boolean, buf0 As apserv.apbuffers, _
			buf1 As apserv.apbuffers)
			Dim i As Integer = 0
			Dim i_ As Integer = 0

			alglib.ap.assert(i1 >= i0, "RankDataBasecase: internal error")
			If alglib.ap.len(buf1.ra0) < nfeatures Then
				buf1.ra0 = New Double(nfeatures - 1) {}
			End If
			For i = i0 To i1 - 1
				For i_ = 0 To nfeatures - 1
					buf1.ra0(i_) = xy(i, i_)
				Next
				basicstatops.rankx(buf1.ra0, nfeatures, iscentered, buf0)
				For i_ = 0 To nfeatures - 1
					xy(i, i_) = buf1.ra0(i_)
				Next
			Next
		End Sub


	End Class
	Public Class correlationtests
		'************************************************************************
'        Pearson's correlation coefficient significance test
'
'        This test checks hypotheses about whether X  and  Y  are  samples  of  two
'        continuous  distributions  having  zero  correlation  or   whether   their
'        correlation is non-zero.
'
'        The following tests are performed:
'            * two-tailed test (null hypothesis - X and Y have zero correlation)
'            * left-tailed test (null hypothesis - the correlation  coefficient  is
'              greater than or equal to 0)
'            * right-tailed test (null hypothesis - the correlation coefficient  is
'              less than or equal to 0).
'
'        Requirements:
'            * the number of elements in each sample is not less than 5
'            * normality of distributions of X and Y.
'
'        Input parameters:
'            R   -   Pearson's correlation coefficient for X and Y
'            N   -   number of elements in samples, N>=5.
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'          -- ALGLIB --
'             Copyright 09.04.2007 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub pearsoncorrelationsignificance(r As Double, n As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
			Dim t As Double = 0
			Dim p As Double = 0

			bothtails = 0
			lefttail = 0
			righttail = 0


			'
			' Some special cases
			'
			If CDbl(r) >= CDbl(1) Then
				bothtails = 0.0
				lefttail = 1.0
				righttail = 0.0
				Return
			End If
			If CDbl(r) <= CDbl(-1) Then
				bothtails = 0.0
				lefttail = 0.0
				righttail = 1.0
				Return
			End If
			If n < 5 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' General case
			'
			t = r * System.Math.sqrt((n - 2) / (1 - Math.sqr(r)))
			p = studenttdistr.studenttdistribution(n - 2, t)
			bothtails = 2 * System.Math.Min(p, 1 - p)
			lefttail = p
			righttail = 1 - p
		End Sub


		'************************************************************************
'        Spearman's rank correlation coefficient significance test
'
'        This test checks hypotheses about whether X  and  Y  are  samples  of  two
'        continuous  distributions  having  zero  correlation  or   whether   their
'        correlation is non-zero.
'
'        The following tests are performed:
'            * two-tailed test (null hypothesis - X and Y have zero correlation)
'            * left-tailed test (null hypothesis - the correlation  coefficient  is
'              greater than or equal to 0)
'            * right-tailed test (null hypothesis - the correlation coefficient  is
'              less than or equal to 0).
'
'        Requirements:
'            * the number of elements in each sample is not less than 5.
'
'        The test is non-parametric and doesn't require distributions X and Y to be
'        normal.
'
'        Input parameters:
'            R   -   Spearman's rank correlation coefficient for X and Y
'            N   -   number of elements in samples, N>=5.
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'          -- ALGLIB --
'             Copyright 09.04.2007 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub spearmanrankcorrelationsignificance(r As Double, n As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
			Dim t As Double = 0
			Dim p As Double = 0

			bothtails = 0
			lefttail = 0
			righttail = 0


			'
			' Special case
			'
			If n < 5 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' General case
			'
			If CDbl(r) >= CDbl(1) Then
				t = 10000000000.0
			Else
				If CDbl(r) <= CDbl(-1) Then
					t = -10000000000.0
				Else
					t = r * System.Math.sqrt((n - 2) / (1 - Math.sqr(r)))
				End If
			End If
			If CDbl(t) < CDbl(0) Then
				p = spearmantail(t, n)
				bothtails = 2 * p
				lefttail = p
				righttail = 1 - p
			Else
				p = spearmantail(-t, n)
				bothtails = 2 * p
				lefttail = 1 - p
				righttail = p
			End If
		End Sub


		'************************************************************************
'        Tail(S, 5)
'        ************************************************************************

		Private Shared Function spearmantail5(s As Double) As Double
			Dim result As Double = 0

			If CDbl(s) < CDbl(0.0) Then
				result = studenttdistr.studenttdistribution(3, -s)
				Return result
			End If
			If CDbl(s) >= CDbl(3.58) Then
				result = 0.008304
				Return result
			End If
			If CDbl(s) >= CDbl(2.322) Then
				result = 0.04163
				Return result
			End If
			If CDbl(s) >= CDbl(1.704) Then
				result = 0.06641
				Return result
			End If
			If CDbl(s) >= CDbl(1.303) Then
				result = 0.1164
				Return result
			End If
			If CDbl(s) >= CDbl(1.003) Then
				result = 0.1748
				Return result
			End If
			If CDbl(s) >= CDbl(0.7584) Then
				result = 0.2249
				Return result
			End If
			If CDbl(s) >= CDbl(0.5468) Then
				result = 0.2581
				Return result
			End If
			If CDbl(s) >= CDbl(0.3555) Then
				result = 0.3413
				Return result
			End If
			If CDbl(s) >= CDbl(0.1759) Then
				result = 0.3911
				Return result
			End If
			If CDbl(s) >= CDbl(0.001741) Then
				result = 0.4747
				Return result
			End If
			If CDbl(s) >= CDbl(0.0) Then
				result = 0.5248
				Return result
			End If
			result = 0
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6)
'        ************************************************************************

		Private Shared Function spearmantail6(s As Double) As Double
			Dim result As Double = 0

			If CDbl(s) < CDbl(1.001) Then
				result = studenttdistr.studenttdistribution(4, -s)
				Return result
			End If
			If CDbl(s) >= CDbl(5.663) Then
				result = 0.001366
				Return result
			End If
			If CDbl(s) >= CDbl(3.834) Then
				result = 0.00835
				Return result
			End If
			If CDbl(s) >= CDbl(2.968) Then
				result = 0.01668
				Return result
			End If
			If CDbl(s) >= CDbl(2.43) Then
				result = 0.02921
				Return result
			End If
			If CDbl(s) >= CDbl(2.045) Then
				result = 0.05144
				Return result
			End If
			If CDbl(s) >= CDbl(1.747) Then
				result = 0.06797
				Return result
			End If
			If CDbl(s) >= CDbl(1.502) Then
				result = 0.08752
				Return result
			End If
			If CDbl(s) >= CDbl(1.295) Then
				result = 0.121
				Return result
			End If
			If CDbl(s) >= CDbl(1.113) Then
				result = 0.1487
				Return result
			End If
			If CDbl(s) >= CDbl(1.001) Then
				result = 0.178
				Return result
			End If
			result = 0
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7)
'        ************************************************************************

		Private Shared Function spearmantail7(s As Double) As Double
			Dim result As Double = 0

			If CDbl(s) < CDbl(1.001) Then
				result = studenttdistr.studenttdistribution(5, -s)
				Return result
			End If
			If CDbl(s) >= CDbl(8.159) Then
				result = 0.0002081
				Return result
			End If
			If CDbl(s) >= CDbl(5.62) Then
				result = 0.001393
				Return result
			End If
			If CDbl(s) >= CDbl(4.445) Then
				result = 0.003398
				Return result
			End If
			If CDbl(s) >= CDbl(3.728) Then
				result = 0.006187
				Return result
			End If
			If CDbl(s) >= CDbl(3.226) Then
				result = 0.012
				Return result
			End If
			If CDbl(s) >= CDbl(2.844) Then
				result = 0.01712
				Return result
			End If
			If CDbl(s) >= CDbl(2.539) Then
				result = 0.02408
				Return result
			End If
			If CDbl(s) >= CDbl(2.285) Then
				result = 0.0332
				Return result
			End If
			If CDbl(s) >= CDbl(2.068) Then
				result = 0.04406
				Return result
			End If
			If CDbl(s) >= CDbl(1.879) Then
				result = 0.05478
				Return result
			End If
			If CDbl(s) >= CDbl(1.71) Then
				result = 0.06946
				Return result
			End If
			If CDbl(s) >= CDbl(1.559) Then
				result = 0.08331
				Return result
			End If
			If CDbl(s) >= CDbl(1.42) Then
				result = 0.1001
				Return result
			End If
			If CDbl(s) >= CDbl(1.292) Then
				result = 0.118
				Return result
			End If
			If CDbl(s) >= CDbl(1.173) Then
				result = 0.1335
				Return result
			End If
			If CDbl(s) >= CDbl(1.062) Then
				result = 0.1513
				Return result
			End If
			If CDbl(s) >= CDbl(1.001) Then
				result = 0.177
				Return result
			End If
			result = 0
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8)
'        ************************************************************************

		Private Shared Function spearmantail8(s As Double) As Double
			Dim result As Double = 0

			If CDbl(s) < CDbl(2.001) Then
				result = studenttdistr.studenttdistribution(6, -s)
				Return result
			End If
			If CDbl(s) >= CDbl(11.03) Then
				result = 2.194E-05
				Return result
			End If
			If CDbl(s) >= CDbl(7.685) Then
				result = 0.0002008
				Return result
			End If
			If CDbl(s) >= CDbl(6.143) Then
				result = 0.0005686
				Return result
			End If
			If CDbl(s) >= CDbl(5.213) Then
				result = 0.001138
				Return result
			End If
			If CDbl(s) >= CDbl(4.567) Then
				result = 0.00231
				Return result
			End If
			If CDbl(s) >= CDbl(4.081) Then
				result = 0.003634
				Return result
			End If
			If CDbl(s) >= CDbl(3.697) Then
				result = 0.005369
				Return result
			End If
			If CDbl(s) >= CDbl(3.381) Then
				result = 0.007708
				Return result
			End If
			If CDbl(s) >= CDbl(3.114) Then
				result = 0.01087
				Return result
			End If
			If CDbl(s) >= CDbl(2.884) Then
				result = 0.01397
				Return result
			End If
			If CDbl(s) >= CDbl(2.682) Then
				result = 0.01838
				Return result
			End If
			If CDbl(s) >= CDbl(2.502) Then
				result = 0.02288
				Return result
			End If
			If CDbl(s) >= CDbl(2.34) Then
				result = 0.02883
				Return result
			End If
			If CDbl(s) >= CDbl(2.192) Then
				result = 0.03469
				Return result
			End If
			If CDbl(s) >= CDbl(2.057) Then
				result = 0.04144
				Return result
			End If
			If CDbl(s) >= CDbl(2.001) Then
				result = 0.04804
				Return result
			End If
			result = 0
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9)
'        ************************************************************************

		Private Shared Function spearmantail9(s As Double) As Double
			Dim result As Double = 0

			If CDbl(s) < CDbl(2.001) Then
				result = studenttdistr.studenttdistribution(7, -s)
				Return result
			End If
			If CDbl(s) >= CDbl(9.989) Then
				result = 2.306E-05
				Return result
			End If
			If CDbl(s) >= CDbl(8.069) Then
				result = 8.167E-05
				Return result
			End If
			If CDbl(s) >= CDbl(6.89) Then
				result = 0.0001744
				Return result
			End If
			If CDbl(s) >= CDbl(6.077) Then
				result = 0.0003625
				Return result
			End If
			If CDbl(s) >= CDbl(5.469) Then
				result = 0.000645
				Return result
			End If
			If CDbl(s) >= CDbl(4.991) Then
				result = 0.001001
				Return result
			End If
			If CDbl(s) >= CDbl(4.6) Then
				result = 0.001514
				Return result
			End If
			If CDbl(s) >= CDbl(4.272) Then
				result = 0.002213
				Return result
			End If
			If CDbl(s) >= CDbl(3.991) Then
				result = 0.00299
				Return result
			End If
			If CDbl(s) >= CDbl(3.746) Then
				result = 0.004101
				Return result
			End If
			If CDbl(s) >= CDbl(3.53) Then
				result = 0.005355
				Return result
			End If
			If CDbl(s) >= CDbl(3.336) Then
				result = 0.006887
				Return result
			End If
			If CDbl(s) >= CDbl(3.161) Then
				result = 0.008598
				Return result
			End If
			If CDbl(s) >= CDbl(3.002) Then
				result = 0.01065
				Return result
			End If
			If CDbl(s) >= CDbl(2.855) Then
				result = 0.01268
				Return result
			End If
			If CDbl(s) >= CDbl(2.72) Then
				result = 0.01552
				Return result
			End If
			If CDbl(s) >= CDbl(2.595) Then
				result = 0.01836
				Return result
			End If
			If CDbl(s) >= CDbl(2.477) Then
				result = 0.02158
				Return result
			End If
			If CDbl(s) >= CDbl(2.368) Then
				result = 0.02512
				Return result
			End If
			If CDbl(s) >= CDbl(2.264) Then
				result = 0.02942
				Return result
			End If
			If CDbl(s) >= CDbl(2.166) Then
				result = 0.03325
				Return result
			End If
			If CDbl(s) >= CDbl(2.073) Then
				result = 0.038
				Return result
			End If
			If CDbl(s) >= CDbl(2.001) Then
				result = 0.04285
				Return result
			End If
			result = 0
			Return result
		End Function


		'************************************************************************
'        Tail(T,N), accepts T<0
'        ************************************************************************

		Private Shared Function spearmantail(t As Double, n As Integer) As Double
			Dim result As Double = 0

			If n = 5 Then
				result = spearmantail5(-t)
				Return result
			End If
			If n = 6 Then
				result = spearmantail6(-t)
				Return result
			End If
			If n = 7 Then
				result = spearmantail7(-t)
				Return result
			End If
			If n = 8 Then
				result = spearmantail8(-t)
				Return result
			End If
			If n = 9 Then
				result = spearmantail9(-t)
				Return result
			End If
			result = studenttdistr.studenttdistribution(n - 2, t)
			Return result
		End Function


	End Class
	Public Class jarquebera
		'************************************************************************
'        Jarque-Bera test
'
'        This test checks hypotheses about the fact that a  given  sample  X  is  a
'        sample of normal random variable.
'
'        Requirements:
'            * the number of elements in the sample is not less than 5.
'
'        Input parameters:
'            X   -   sample. Array whose index goes from 0 to N-1.
'            N   -   size of the sample. N>=5
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'        Accuracy of the approximation used (5<=N<=1951):
'
'        p-value  	    relative error (5<=N<=1951)
'        [1, 0.1]            < 1%
'        [0.1, 0.01]         < 2%
'        [0.01, 0.001]       < 6%
'        [0.001, 0]          wasn't measured
'
'        For N>1951 accuracy wasn't measured but it shouldn't be sharply  different
'        from table values.
'
'          -- ALGLIB --
'             Copyright 09.04.2007 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub jarqueberatest(x As Double(), n As Integer, ByRef p As Double)
			Dim s As Double = 0

			p = 0


			'
			' N is too small
			'
			If n < 5 Then
				p = 1.0
				Return
			End If

			'
			' N is large enough
			'
			jarqueberastatistic(x, n, s)
			p = jarqueberaapprox(n, s)
		End Sub


		Private Shared Sub jarqueberastatistic(x As Double(), n As Integer, ByRef s As Double)
			Dim i As Integer = 0
			Dim v As Double = 0
			Dim v1 As Double = 0
			Dim v2 As Double = 0
			Dim stddev As Double = 0
			Dim mean As Double = 0
			Dim variance As Double = 0
			Dim skewness As Double = 0
			Dim kurtosis As Double = 0

			s = 0

			mean = 0
			variance = 0
			skewness = 0
			kurtosis = 0
			stddev = 0
			alglib.ap.assert(n > 1)

			'
			' Mean
			'
			For i = 0 To n - 1
				mean = mean + x(i)
			Next
			mean = mean / n

			'
			' Variance (using corrected two-pass algorithm)
			'
			If n <> 1 Then
				v1 = 0
				For i = 0 To n - 1
					v1 = v1 + Math.sqr(x(i) - mean)
				Next
				v2 = 0
				For i = 0 To n - 1
					v2 = v2 + (x(i) - mean)
				Next
				v2 = Math.sqr(v2) / n
				variance = (v1 - v2) / (n - 1)
				If CDbl(variance) < CDbl(0) Then
					variance = 0
				End If
				stddev = System.Math.sqrt(variance)
			End If

			'
			' Skewness and kurtosis
			'
			If CDbl(stddev) <> CDbl(0) Then
				For i = 0 To n - 1
					v = (x(i) - mean) / stddev
					v2 = Math.sqr(v)
					skewness = skewness + v2 * v
					kurtosis = kurtosis + Math.sqr(v2)
				Next
				skewness = skewness / n
				kurtosis = kurtosis / n - 3
			End If

			'
			' Statistic
			'
			s = CDbl(n) / CDbl(6) * (Math.sqr(skewness) + Math.sqr(kurtosis) / 4)
		End Sub


		Private Shared Function jarqueberaapprox(n As Integer, s As Double) As Double
			Dim result As Double = 0
			Dim vx As Double() = New Double(-1) {}
			Dim vy As Double() = New Double(-1) {}
			Dim ctbl As Double(,) = New Double(-1, -1) {}
			Dim t1 As Double = 0
			Dim t2 As Double = 0
			Dim t3 As Double = 0
			Dim t As Double = 0
			Dim f1 As Double = 0
			Dim f2 As Double = 0
			Dim f3 As Double = 0
			Dim f12 As Double = 0
			Dim f23 As Double = 0
			Dim x As Double = 0

			result = 1
			x = s
			If n < 5 Then
				Return result
			End If

			'
			' N = 5..20 are tabulated
			'
			If n >= 5 AndAlso n <= 20 Then
				If n = 5 Then
					result = System.Math.Exp(jbtbl5(x))
				End If
				If n = 6 Then
					result = System.Math.Exp(jbtbl6(x))
				End If
				If n = 7 Then
					result = System.Math.Exp(jbtbl7(x))
				End If
				If n = 8 Then
					result = System.Math.Exp(jbtbl8(x))
				End If
				If n = 9 Then
					result = System.Math.Exp(jbtbl9(x))
				End If
				If n = 10 Then
					result = System.Math.Exp(jbtbl10(x))
				End If
				If n = 11 Then
					result = System.Math.Exp(jbtbl11(x))
				End If
				If n = 12 Then
					result = System.Math.Exp(jbtbl12(x))
				End If
				If n = 13 Then
					result = System.Math.Exp(jbtbl13(x))
				End If
				If n = 14 Then
					result = System.Math.Exp(jbtbl14(x))
				End If
				If n = 15 Then
					result = System.Math.Exp(jbtbl15(x))
				End If
				If n = 16 Then
					result = System.Math.Exp(jbtbl16(x))
				End If
				If n = 17 Then
					result = System.Math.Exp(jbtbl17(x))
				End If
				If n = 18 Then
					result = System.Math.Exp(jbtbl18(x))
				End If
				If n = 19 Then
					result = System.Math.Exp(jbtbl19(x))
				End If
				If n = 20 Then
					result = System.Math.Exp(jbtbl20(x))
				End If
				Return result
			End If

			'
			' N = 20, 30, 50 are tabulated.
			' In-between values are interpolated
			' using interpolating polynomial of the second degree.
			'
			If n > 20 AndAlso n <= 50 Then
				t1 = -(1.0 / 20.0)
				t2 = -(1.0 / 30.0)
				t3 = -(1.0 / 50.0)
				t = -(1.0 / n)
				f1 = jbtbl20(x)
				f2 = jbtbl30(x)
				f3 = jbtbl50(x)
				f12 = ((t - t2) * f1 + (t1 - t) * f2) / (t1 - t2)
				f23 = ((t - t3) * f2 + (t2 - t) * f3) / (t2 - t3)
				result = ((t - t3) * f12 + (t1 - t) * f23) / (t1 - t3)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				result = System.Math.Exp(result)
				Return result
			End If

			'
			' N = 50, 65, 100 are tabulated.
			' In-between values are interpolated
			' using interpolating polynomial of the second degree.
			'
			If n > 50 AndAlso n <= 100 Then
				t1 = -(1.0 / 50.0)
				t2 = -(1.0 / 65.0)
				t3 = -(1.0 / 100.0)
				t = -(1.0 / n)
				f1 = jbtbl50(x)
				f2 = jbtbl65(x)
				f3 = jbtbl100(x)
				f12 = ((t - t2) * f1 + (t1 - t) * f2) / (t1 - t2)
				f23 = ((t - t3) * f2 + (t2 - t) * f3) / (t2 - t3)
				result = ((t - t3) * f12 + (t1 - t) * f23) / (t1 - t3)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				result = System.Math.Exp(result)
				Return result
			End If

			'
			' N = 100, 130, 200 are tabulated.
			' In-between values are interpolated
			' using interpolating polynomial of the second degree.
			'
			If n > 100 AndAlso n <= 200 Then
				t1 = -(1.0 / 100.0)
				t2 = -(1.0 / 130.0)
				t3 = -(1.0 / 200.0)
				t = -(1.0 / n)
				f1 = jbtbl100(x)
				f2 = jbtbl130(x)
				f3 = jbtbl200(x)
				f12 = ((t - t2) * f1 + (t1 - t) * f2) / (t1 - t2)
				f23 = ((t - t3) * f2 + (t2 - t) * f3) / (t2 - t3)
				result = ((t - t3) * f12 + (t1 - t) * f23) / (t1 - t3)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				result = System.Math.Exp(result)
				Return result
			End If

			'
			' N = 200, 301, 501 are tabulated.
			' In-between values are interpolated
			' using interpolating polynomial of the second degree.
			'
			If n > 200 AndAlso n <= 501 Then
				t1 = -(1.0 / 200.0)
				t2 = -(1.0 / 301.0)
				t3 = -(1.0 / 501.0)
				t = -(1.0 / n)
				f1 = jbtbl200(x)
				f2 = jbtbl301(x)
				f3 = jbtbl501(x)
				f12 = ((t - t2) * f1 + (t1 - t) * f2) / (t1 - t2)
				f23 = ((t - t3) * f2 + (t2 - t) * f3) / (t2 - t3)
				result = ((t - t3) * f12 + (t1 - t) * f23) / (t1 - t3)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				result = System.Math.Exp(result)
				Return result
			End If

			'
			' N = 501, 701, 1401 are tabulated.
			' In-between values are interpolated
			' using interpolating polynomial of the second degree.
			'
			If n > 501 AndAlso n <= 1401 Then
				t1 = -(1.0 / 501.0)
				t2 = -(1.0 / 701.0)
				t3 = -(1.0 / 1401.0)
				t = -(1.0 / n)
				f1 = jbtbl501(x)
				f2 = jbtbl701(x)
				f3 = jbtbl1401(x)
				f12 = ((t - t2) * f1 + (t1 - t) * f2) / (t1 - t2)
				f23 = ((t - t3) * f2 + (t2 - t) * f3) / (t2 - t3)
				result = ((t - t3) * f12 + (t1 - t) * f23) / (t1 - t3)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				result = System.Math.Exp(result)
				Return result
			End If

			'
			' Asymptotic expansion
			'
			If n > 1401 Then
				result = -(0.5 * x) + (jbtbl1401(x) + 0.5 * x) * System.Math.sqrt(CDbl(1401) / CDbl(n))
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				result = System.Math.Exp(result)
				Return result
			End If
			Return result
		End Function


		Private Shared Function jbtbl5(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(0.4) Then
				x = 2 * (s - 0.0) / 0.4 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.097885E-20, tj, tj1, result)
				jbcheb(x, -2.854501E-20, tj, tj1, result)
				jbcheb(x, -1.756616E-20, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(1.1) Then
				x = 2 * (s - 0.4) / 0.7 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.324545, tj, tj1, result)
				jbcheb(x, -1.075941, tj, tj1, result)
				jbcheb(x, -0.9772272, tj, tj1, result)
				jbcheb(x, 0.3175686, tj, tj1, result)
				jbcheb(x, -0.1576162, tj, tj1, result)
				jbcheb(x, 0.1126861, tj, tj1, result)
				jbcheb(x, -0.03434425, tj, tj1, result)
				jbcheb(x, -0.2790359, tj, tj1, result)
				jbcheb(x, 0.02809178, tj, tj1, result)
				jbcheb(x, -0.5479704, tj, tj1, result)
				jbcheb(x, 0.0371704, tj, tj1, result)
				jbcheb(x, -0.529417, tj, tj1, result)
				jbcheb(x, 0.02880632, tj, tj1, result)
				jbcheb(x, -0.3023344, tj, tj1, result)
				jbcheb(x, 0.01601531, tj, tj1, result)
				jbcheb(x, -0.07920403, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(518.8419 * (s - 1.1)) - 4.767297
			Return result
		End Function


		Private Shared Function jbtbl6(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(0.25) Then
				x = 2 * (s - 0.0) / 0.25 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -0.0002274707, tj, tj1, result)
				jbcheb(x, -0.0005700471, tj, tj1, result)
				jbcheb(x, -0.0003425764, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(1.3) Then
				x = 2 * (s - 0.25) / 1.05 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.339, tj, tj1, result)
				jbcheb(x, -2.011104, tj, tj1, result)
				jbcheb(x, -0.8168177, tj, tj1, result)
				jbcheb(x, -0.1085666, tj, tj1, result)
				jbcheb(x, 0.07738606, tj, tj1, result)
				jbcheb(x, 0.07022876, tj, tj1, result)
				jbcheb(x, 0.03462402, tj, tj1, result)
				jbcheb(x, 0.00690827, tj, tj1, result)
				jbcheb(x, -0.008230772, tj, tj1, result)
				jbcheb(x, -0.01006996, tj, tj1, result)
				jbcheb(x, -0.005410222, tj, tj1, result)
				jbcheb(x, -0.002893768, tj, tj1, result)
				jbcheb(x, 0.0008114564, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(1.85) Then
				x = 2 * (s - 1.3) / 0.55 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.794311, tj, tj1, result)
				jbcheb(x, -3.5787, tj, tj1, result)
				jbcheb(x, -1.394664, tj, tj1, result)
				jbcheb(x, -0.792829, tj, tj1, result)
				jbcheb(x, -0.4813273, tj, tj1, result)
				jbcheb(x, -0.3076063, tj, tj1, result)
				jbcheb(x, -0.183538, tj, tj1, result)
				jbcheb(x, -0.1013013, tj, tj1, result)
				jbcheb(x, -0.05058903, tj, tj1, result)
				jbcheb(x, -0.01856915, tj, tj1, result)
				jbcheb(x, -0.006710887, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(177.0029 * (s - 1.85)) - 13.71015
			Return result
		End Function


		Private Shared Function jbtbl7(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(1.4) Then
				x = 2 * (s - 0.0) / 1.4 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.093681, tj, tj1, result)
				jbcheb(x, -1.695911, tj, tj1, result)
				jbcheb(x, -0.7473192, tj, tj1, result)
				jbcheb(x, -0.1203236, tj, tj1, result)
				jbcheb(x, 0.06590379, tj, tj1, result)
				jbcheb(x, 0.06291876, tj, tj1, result)
				jbcheb(x, 0.03132007, tj, tj1, result)
				jbcheb(x, 0.009411147, tj, tj1, result)
				jbcheb(x, -0.001180067, tj, tj1, result)
				jbcheb(x, -0.00348761, tj, tj1, result)
				jbcheb(x, -0.002436561, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(3.0) Then
				x = 2 * (s - 1.4) / 1.6 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.947854, tj, tj1, result)
				jbcheb(x, -2.772675, tj, tj1, result)
				jbcheb(x, -0.4707912, tj, tj1, result)
				jbcheb(x, -0.1691171, tj, tj1, result)
				jbcheb(x, -0.04132795, tj, tj1, result)
				jbcheb(x, -0.0148131, tj, tj1, result)
				jbcheb(x, 0.002867536, tj, tj1, result)
				jbcheb(x, 0.0008772327, tj, tj1, result)
				jbcheb(x, 0.005033387, tj, tj1, result)
				jbcheb(x, -0.001378277, tj, tj1, result)
				jbcheb(x, -0.002497964, tj, tj1, result)
				jbcheb(x, -0.003636814, tj, tj1, result)
				jbcheb(x, -0.000958164, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(3.2) Then
				x = 2 * (s - 3.0) / 0.2 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -7.511008, tj, tj1, result)
				jbcheb(x, -0.8140472, tj, tj1, result)
				jbcheb(x, 1.682053, tj, tj1, result)
				jbcheb(x, -0.02568561, tj, tj1, result)
				jbcheb(x, -1.93393, tj, tj1, result)
				jbcheb(x, -0.8140472, tj, tj1, result)
				jbcheb(x, -3.895025, tj, tj1, result)
				jbcheb(x, -0.8140472, tj, tj1, result)
				jbcheb(x, -1.93393, tj, tj1, result)
				jbcheb(x, -0.02568561, tj, tj1, result)
				jbcheb(x, 1.682053, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(1824.116 * (s - 3.2)) - 14.4033
			Return result
		End Function


		Private Shared Function jbtbl8(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(1.3) Then
				x = 2 * (s - 0.0) / 1.3 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -0.7199015, tj, tj1, result)
				jbcheb(x, -1.095921, tj, tj1, result)
				jbcheb(x, -0.4736828, tj, tj1, result)
				jbcheb(x, -0.1047438, tj, tj1, result)
				jbcheb(x, -0.00248432, tj, tj1, result)
				jbcheb(x, 0.007937923, tj, tj1, result)
				jbcheb(x, 0.00481047, tj, tj1, result)
				jbcheb(x, 0.00213978, tj, tj1, result)
				jbcheb(x, 0.0006708443, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(2.0) Then
				x = 2 * (s - 1.3) / 0.7 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -3.378966, tj, tj1, result)
				jbcheb(x, -0.7802461, tj, tj1, result)
				jbcheb(x, 0.1547593, tj, tj1, result)
				jbcheb(x, -0.06241042, tj, tj1, result)
				jbcheb(x, 0.01203274, tj, tj1, result)
				jbcheb(x, 0.00520199, tj, tj1, result)
				jbcheb(x, -0.005125597, tj, tj1, result)
				jbcheb(x, 0.001584426, tj, tj1, result)
				jbcheb(x, 0.0002546069, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(5.0) Then
				x = 2 * (s - 2.0) / 3.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.828366, tj, tj1, result)
				jbcheb(x, -3.137533, tj, tj1, result)
				jbcheb(x, -0.5016671, tj, tj1, result)
				jbcheb(x, -0.1745637, tj, tj1, result)
				jbcheb(x, -0.05189801, tj, tj1, result)
				jbcheb(x, -0.0162161, tj, tj1, result)
				jbcheb(x, -0.006741122, tj, tj1, result)
				jbcheb(x, -0.004516368, tj, tj1, result)
				jbcheb(x, 0.0003552085, tj, tj1, result)
				jbcheb(x, 0.002787029, tj, tj1, result)
				jbcheb(x, 0.005359774, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(5.087028 * (s - 5.0)) - 10.713
			Return result
		End Function


		Private Shared Function jbtbl9(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(1.3) Then
				x = 2 * (s - 0.0) / 1.3 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -0.627932, tj, tj1, result)
				jbcheb(x, -0.9277151, tj, tj1, result)
				jbcheb(x, -0.3669339, tj, tj1, result)
				jbcheb(x, -0.07086149, tj, tj1, result)
				jbcheb(x, -0.001333816, tj, tj1, result)
				jbcheb(x, 0.003871249, tj, tj1, result)
				jbcheb(x, 0.002007048, tj, tj1, result)
				jbcheb(x, 0.0007482245, tj, tj1, result)
				jbcheb(x, 0.0002355615, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(2.0) Then
				x = 2 * (s - 1.3) / 0.7 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -2.98143, tj, tj1, result)
				jbcheb(x, -0.7972248, tj, tj1, result)
				jbcheb(x, 0.1747737, tj, tj1, result)
				jbcheb(x, -0.0380853, tj, tj1, result)
				jbcheb(x, -0.007888305, tj, tj1, result)
				jbcheb(x, 0.009001302, tj, tj1, result)
				jbcheb(x, -0.001378767, tj, tj1, result)
				jbcheb(x, -0.00110851, tj, tj1, result)
				jbcheb(x, 0.0005915372, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(7.0) Then
				x = 2 * (s - 2.0) / 5.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.387463, tj, tj1, result)
				jbcheb(x, -2.845231, tj, tj1, result)
				jbcheb(x, -0.1809956, tj, tj1, result)
				jbcheb(x, -0.07543461, tj, tj1, result)
				jbcheb(x, -0.004880397, tj, tj1, result)
				jbcheb(x, -0.01160074, tj, tj1, result)
				jbcheb(x, -0.007356527, tj, tj1, result)
				jbcheb(x, -0.004394428, tj, tj1, result)
				jbcheb(x, 0.0009619892, tj, tj1, result)
				jbcheb(x, -0.0002758763, tj, tj1, result)
				jbcheb(x, 4.790977E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(2.020952 * (s - 7.0)) - 9.516623
			Return result
		End Function


		Private Shared Function jbtbl10(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(1.2) Then
				x = 2 * (s - 0.0) / 1.2 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -0.4590993, tj, tj1, result)
				jbcheb(x, -0.656273, tj, tj1, result)
				jbcheb(x, -0.2353934, tj, tj1, result)
				jbcheb(x, -0.04069933, tj, tj1, result)
				jbcheb(x, -0.001849151, tj, tj1, result)
				jbcheb(x, 0.0008931406, tj, tj1, result)
				jbcheb(x, 0.0003636295, tj, tj1, result)
				jbcheb(x, 1.17834E-05, tj, tj1, result)
				jbcheb(x, -8.917749E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(2.0) Then
				x = 2 * (s - 1.2) / 0.8 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -2.537658, tj, tj1, result)
				jbcheb(x, -0.9962401, tj, tj1, result)
				jbcheb(x, 0.1838715, tj, tj1, result)
				jbcheb(x, 0.01055792, tj, tj1, result)
				jbcheb(x, -0.02580316, tj, tj1, result)
				jbcheb(x, 0.001781701, tj, tj1, result)
				jbcheb(x, 0.003770362, tj, tj1, result)
				jbcheb(x, -0.0004838983, tj, tj1, result)
				jbcheb(x, -0.0006999052, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(7.0) Then
				x = 2 * (s - 2.0) / 5.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.337524, tj, tj1, result)
				jbcheb(x, -1.877029, tj, tj1, result)
				jbcheb(x, 0.0473465, tj, tj1, result)
				jbcheb(x, -0.04249254, tj, tj1, result)
				jbcheb(x, 0.00332025, tj, tj1, result)
				jbcheb(x, -0.006432266, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.8711035 * (s - 7.0)) - 7.212811
			Return result
		End Function


		Private Shared Function jbtbl11(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(1.2) Then
				x = 2 * (s - 0.0) / 1.2 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -0.4339517, tj, tj1, result)
				jbcheb(x, -0.6051558, tj, tj1, result)
				jbcheb(x, -0.2000992, tj, tj1, result)
				jbcheb(x, -0.03022547, tj, tj1, result)
				jbcheb(x, -0.0009808401, tj, tj1, result)
				jbcheb(x, 0.000559287, tj, tj1, result)
				jbcheb(x, 0.0003575081, tj, tj1, result)
				jbcheb(x, 0.0002086173, tj, tj1, result)
				jbcheb(x, 6.089011E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(2.25) Then
				x = 2 * (s - 1.2) / 1.05 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -2.523221, tj, tj1, result)
				jbcheb(x, -1.068388, tj, tj1, result)
				jbcheb(x, 0.2179661, tj, tj1, result)
				jbcheb(x, -0.001555524, tj, tj1, result)
				jbcheb(x, -0.03238964, tj, tj1, result)
				jbcheb(x, 0.00736432, tj, tj1, result)
				jbcheb(x, 0.004895771, tj, tj1, result)
				jbcheb(x, -0.001762774, tj, tj1, result)
				jbcheb(x, -0.000820134, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(8.0) Then
				x = 2 * (s - 2.25) / 5.75 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.212179, tj, tj1, result)
				jbcheb(x, -1.684579, tj, tj1, result)
				jbcheb(x, 0.08299519, tj, tj1, result)
				jbcheb(x, -0.03606261, tj, tj1, result)
				jbcheb(x, 0.007310869, tj, tj1, result)
				jbcheb(x, -0.003320115, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.5715445 * (s - 8.0)) - 6.845834
			Return result
		End Function


		Private Shared Function jbtbl12(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(1.0) Then
				x = 2 * (s - 0.0) / 1.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -0.2736742, tj, tj1, result)
				jbcheb(x, -0.3657836, tj, tj1, result)
				jbcheb(x, -0.1047209, tj, tj1, result)
				jbcheb(x, -0.01319599, tj, tj1, result)
				jbcheb(x, -0.0005545631, tj, tj1, result)
				jbcheb(x, 9.280445E-05, tj, tj1, result)
				jbcheb(x, 2.815679E-05, tj, tj1, result)
				jbcheb(x, -2.213519E-05, tj, tj1, result)
				jbcheb(x, 1.256838E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(3.0) Then
				x = 2 * (s - 1.0) / 2.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -2.573947, tj, tj1, result)
				jbcheb(x, -1.515287, tj, tj1, result)
				jbcheb(x, 0.361188, tj, tj1, result)
				jbcheb(x, -0.03271311, tj, tj1, result)
				jbcheb(x, -0.06495815, tj, tj1, result)
				jbcheb(x, 0.04141186, tj, tj1, result)
				jbcheb(x, 0.0007180886, tj, tj1, result)
				jbcheb(x, -0.01388211, tj, tj1, result)
				jbcheb(x, 0.004890761, tj, tj1, result)
				jbcheb(x, 0.003233175, tj, tj1, result)
				jbcheb(x, -0.002946156, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(12.0) Then
				x = 2 * (s - 3.0) / 9.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.947819, tj, tj1, result)
				jbcheb(x, -2.034157, tj, tj1, result)
				jbcheb(x, 0.06878986, tj, tj1, result)
				jbcheb(x, -0.04078603, tj, tj1, result)
				jbcheb(x, 0.006990977, tj, tj1, result)
				jbcheb(x, -0.002866215, tj, tj1, result)
				jbcheb(x, 0.003897866, tj, tj1, result)
				jbcheb(x, 0.002512252, tj, tj1, result)
				jbcheb(x, 0.002073743, tj, tj1, result)
				jbcheb(x, 0.003022621, tj, tj1, result)
				jbcheb(x, 0.001501343, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.2877243 * (s - 12.0)) - 7.936839
			Return result
		End Function


		Private Shared Function jbtbl13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(1.0) Then
				x = 2 * (s - 0.0) / 1.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -0.2713276, tj, tj1, result)
				jbcheb(x, -0.3557541, tj, tj1, result)
				jbcheb(x, -0.09459092, tj, tj1, result)
				jbcheb(x, -0.01044145, tj, tj1, result)
				jbcheb(x, -0.0002546132, tj, tj1, result)
				jbcheb(x, 0.0001002374, tj, tj1, result)
				jbcheb(x, 2.349456E-05, tj, tj1, result)
				jbcheb(x, -7.025669E-05, tj, tj1, result)
				jbcheb(x, -1.590242E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(3.0) Then
				x = 2 * (s - 1.0) / 2.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -2.454383, tj, tj1, result)
				jbcheb(x, -1.467539, tj, tj1, result)
				jbcheb(x, 0.3270774, tj, tj1, result)
				jbcheb(x, -0.008075763, tj, tj1, result)
				jbcheb(x, -0.06611647, tj, tj1, result)
				jbcheb(x, 0.02990785, tj, tj1, result)
				jbcheb(x, 0.008109212, tj, tj1, result)
				jbcheb(x, -0.01135031, tj, tj1, result)
				jbcheb(x, 0.0005915919, tj, tj1, result)
				jbcheb(x, 0.00352239, tj, tj1, result)
				jbcheb(x, -0.001144701, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(13.0) Then
				x = 2 * (s - 3.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.736127, tj, tj1, result)
				jbcheb(x, -1.920809, tj, tj1, result)
				jbcheb(x, 0.1175858, tj, tj1, result)
				jbcheb(x, -0.04002049, tj, tj1, result)
				jbcheb(x, 0.01158966, tj, tj1, result)
				jbcheb(x, -0.003157781, tj, tj1, result)
				jbcheb(x, 0.002762172, tj, tj1, result)
				jbcheb(x, 0.0005780347, tj, tj1, result)
				jbcheb(x, -0.00119331, tj, tj1, result)
				jbcheb(x, -2.442421E-05, tj, tj1, result)
				jbcheb(x, 0.002547756, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.2799944 * (s - 13.0)) - 7.566269
			Return result
		End Function


		Private Shared Function jbtbl14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(1.0) Then
				x = 2 * (s - 0.0) / 1.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -0.2698527, tj, tj1, result)
				jbcheb(x, -0.3479081, tj, tj1, result)
				jbcheb(x, -0.08640733, tj, tj1, result)
				jbcheb(x, -0.008466899, tj, tj1, result)
				jbcheb(x, -0.0001469485, tj, tj1, result)
				jbcheb(x, 2.150009E-05, tj, tj1, result)
				jbcheb(x, 1.965975E-05, tj, tj1, result)
				jbcheb(x, -4.71021E-05, tj, tj1, result)
				jbcheb(x, -1.327808E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(3.0) Then
				x = 2 * (s - 1.0) / 2.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -2.350359, tj, tj1, result)
				jbcheb(x, -1.421365, tj, tj1, result)
				jbcheb(x, 0.2960468, tj, tj1, result)
				jbcheb(x, 0.01149167, tj, tj1, result)
				jbcheb(x, -0.06361109, tj, tj1, result)
				jbcheb(x, 0.01976022, tj, tj1, result)
				jbcheb(x, 0.010827, tj, tj1, result)
				jbcheb(x, -0.008563328, tj, tj1, result)
				jbcheb(x, -0.001453123, tj, tj1, result)
				jbcheb(x, 0.002917559, tj, tj1, result)
				jbcheb(x, -1.151067E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 3.0) / 12.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.746892, tj, tj1, result)
				jbcheb(x, -2.010441, tj, tj1, result)
				jbcheb(x, 0.1566146, tj, tj1, result)
				jbcheb(x, -0.0512969, tj, tj1, result)
				jbcheb(x, 0.01929724, tj, tj1, result)
				jbcheb(x, -0.002524227, tj, tj1, result)
				jbcheb(x, 0.003192933, tj, tj1, result)
				jbcheb(x, -0.000425473, tj, tj1, result)
				jbcheb(x, 0.001620685, tj, tj1, result)
				jbcheb(x, 0.0007289618, tj, tj1, result)
				jbcheb(x, -0.00211235, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.2590621 * (s - 15.0)) - 7.632238
			Return result
		End Function


		Private Shared Function jbtbl15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(2.0) Then
				x = 2 * (s - 0.0) / 2.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.04366, tj, tj1, result)
				jbcheb(x, -1.361653, tj, tj1, result)
				jbcheb(x, -0.3009497, tj, tj1, result)
				jbcheb(x, 0.04951784, tj, tj1, result)
				jbcheb(x, 0.04377903, tj, tj1, result)
				jbcheb(x, 0.01003253, tj, tj1, result)
				jbcheb(x, -0.001271309, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(5.0) Then
				x = 2 * (s - 2.0) / 3.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -3.582778, tj, tj1, result)
				jbcheb(x, -0.8349578, tj, tj1, result)
				jbcheb(x, 0.09476514, tj, tj1, result)
				jbcheb(x, -0.02717385, tj, tj1, result)
				jbcheb(x, 0.01222591, tj, tj1, result)
				jbcheb(x, -0.006635124, tj, tj1, result)
				jbcheb(x, 0.002815993, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(17.0) Then
				x = 2 * (s - 5.0) / 12.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.115476, tj, tj1, result)
				jbcheb(x, -1.655936, tj, tj1, result)
				jbcheb(x, 0.0840431, tj, tj1, result)
				jbcheb(x, -0.02663794, tj, tj1, result)
				jbcheb(x, 0.008868618, tj, tj1, result)
				jbcheb(x, 0.001381447, tj, tj1, result)
				jbcheb(x, 0.0009444801, tj, tj1, result)
				jbcheb(x, -0.0001581503, tj, tj1, result)
				jbcheb(x, -0.0009468696, tj, tj1, result)
				jbcheb(x, 0.001728509, tj, tj1, result)
				jbcheb(x, 0.00120647, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1927937 * (s - 17.0)) - 7.700983
			Return result
		End Function


		Private Shared Function jbtbl16(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(2.0) Then
				x = 2 * (s - 0.0) / 2.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.00257, tj, tj1, result)
				jbcheb(x, -1.298141, tj, tj1, result)
				jbcheb(x, -0.2832803, tj, tj1, result)
				jbcheb(x, 0.03877026, tj, tj1, result)
				jbcheb(x, 0.03539436, tj, tj1, result)
				jbcheb(x, 0.008439658, tj, tj1, result)
				jbcheb(x, -0.0004756911, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(5.0) Then
				x = 2 * (s - 2.0) / 3.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -3.486198, tj, tj1, result)
				jbcheb(x, -0.8242944, tj, tj1, result)
				jbcheb(x, 0.1020002, tj, tj1, result)
				jbcheb(x, -0.03130531, tj, tj1, result)
				jbcheb(x, 0.01512373, tj, tj1, result)
				jbcheb(x, -0.008054876, tj, tj1, result)
				jbcheb(x, 0.003556839, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(20.0) Then
				x = 2 * (s - 5.0) / 15.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.241608, tj, tj1, result)
				jbcheb(x, -1.832655, tj, tj1, result)
				jbcheb(x, 0.1340545, tj, tj1, result)
				jbcheb(x, -0.03361143, tj, tj1, result)
				jbcheb(x, 0.01283219, tj, tj1, result)
				jbcheb(x, 0.003484549, tj, tj1, result)
				jbcheb(x, 0.001805968, tj, tj1, result)
				jbcheb(x, -0.002057243, tj, tj1, result)
				jbcheb(x, -0.001454439, tj, tj1, result)
				jbcheb(x, -0.002177513, tj, tj1, result)
				jbcheb(x, -0.001819209, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.239158 * (s - 20.0)) - 7.963205
			Return result
		End Function


		Private Shared Function jbtbl17(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(3.0) Then
				x = 2 * (s - 0.0) / 3.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.566973, tj, tj1, result)
				jbcheb(x, -1.81033, tj, tj1, result)
				jbcheb(x, -0.04840039, tj, tj1, result)
				jbcheb(x, 0.2337294, tj, tj1, result)
				jbcheb(x, -0.0005383549, tj, tj1, result)
				jbcheb(x, -0.05556515, tj, tj1, result)
				jbcheb(x, -0.008656965, tj, tj1, result)
				jbcheb(x, 0.01404569, tj, tj1, result)
				jbcheb(x, 0.006447867, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(6.0) Then
				x = 2 * (s - 3.0) / 3.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -3.905684, tj, tj1, result)
				jbcheb(x, -0.622292, tj, tj1, result)
				jbcheb(x, 0.04146667, tj, tj1, result)
				jbcheb(x, -0.004809176, tj, tj1, result)
				jbcheb(x, 0.001057028, tj, tj1, result)
				jbcheb(x, -0.0001211838, tj, tj1, result)
				jbcheb(x, -0.0004099683, tj, tj1, result)
				jbcheb(x, 0.0001161105, tj, tj1, result)
				jbcheb(x, 0.0002225465, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(24.0) Then
				x = 2 * (s - 6.0) / 18.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.594282, tj, tj1, result)
				jbcheb(x, -1.917838, tj, tj1, result)
				jbcheb(x, 0.145598, tj, tj1, result)
				jbcheb(x, -0.02999589, tj, tj1, result)
				jbcheb(x, 0.005604263, tj, tj1, result)
				jbcheb(x, -0.003484445, tj, tj1, result)
				jbcheb(x, -0.001819937, tj, tj1, result)
				jbcheb(x, -0.00293039, tj, tj1, result)
				jbcheb(x, 0.0002771761, tj, tj1, result)
				jbcheb(x, -0.0006232581, tj, tj1, result)
				jbcheb(x, -0.0007029083, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.2127771 * (s - 24.0)) - 8.400197
			Return result
		End Function


		Private Shared Function jbtbl18(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(3.0) Then
				x = 2 * (s - 0.0) / 3.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.526802, tj, tj1, result)
				jbcheb(x, -1.762373, tj, tj1, result)
				jbcheb(x, -0.0559889, tj, tj1, result)
				jbcheb(x, 0.2189437, tj, tj1, result)
				jbcheb(x, 0.005971721, tj, tj1, result)
				jbcheb(x, -0.04823067, tj, tj1, result)
				jbcheb(x, -0.01064501, tj, tj1, result)
				jbcheb(x, 0.01014932, tj, tj1, result)
				jbcheb(x, 0.005953513, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(6.0) Then
				x = 2 * (s - 3.0) / 3.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -3.818669, tj, tj1, result)
				jbcheb(x, -0.6070918, tj, tj1, result)
				jbcheb(x, 0.04277196, tj, tj1, result)
				jbcheb(x, -0.004879817, tj, tj1, result)
				jbcheb(x, 0.0006887357, tj, tj1, result)
				jbcheb(x, 1.638451E-05, tj, tj1, result)
				jbcheb(x, 0.00015028, tj, tj1, result)
				jbcheb(x, -3.165796E-05, tj, tj1, result)
				jbcheb(x, 5.03496E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(20.0) Then
				x = 2 * (s - 6.0) / 14.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.010656, tj, tj1, result)
				jbcheb(x, -1.496296, tj, tj1, result)
				jbcheb(x, 0.1002227, tj, tj1, result)
				jbcheb(x, -0.0233825, tj, tj1, result)
				jbcheb(x, 0.004137036, tj, tj1, result)
				jbcheb(x, -0.002586202, tj, tj1, result)
				jbcheb(x, -0.0009736384, tj, tj1, result)
				jbcheb(x, 0.001332251, tj, tj1, result)
				jbcheb(x, 0.001877982, tj, tj1, result)
				jbcheb(x, -1.160963E-05, tj, tj1, result)
				jbcheb(x, -0.002547247, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1684623 * (s - 20.0)) - 7.428883
			Return result
		End Function


		Private Shared Function jbtbl19(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(3.0) Then
				x = 2 * (s - 0.0) / 3.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.490213, tj, tj1, result)
				jbcheb(x, -1.719633, tj, tj1, result)
				jbcheb(x, -0.06459123, tj, tj1, result)
				jbcheb(x, 0.2034878, tj, tj1, result)
				jbcheb(x, 0.01113868, tj, tj1, result)
				jbcheb(x, -0.04030922, tj, tj1, result)
				jbcheb(x, -0.01054022, tj, tj1, result)
				jbcheb(x, 0.007525623, tj, tj1, result)
				jbcheb(x, 0.00527736, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(6.0) Then
				x = 2 * (s - 3.0) / 3.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -3.74475, tj, tj1, result)
				jbcheb(x, -0.5977749, tj, tj1, result)
				jbcheb(x, 0.04223716, tj, tj1, result)
				jbcheb(x, -0.005363889, tj, tj1, result)
				jbcheb(x, 0.0005711774, tj, tj1, result)
				jbcheb(x, -0.0005557257, tj, tj1, result)
				jbcheb(x, 0.0004254794, tj, tj1, result)
				jbcheb(x, 9.034207E-05, tj, tj1, result)
				jbcheb(x, 5.498107E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(20.0) Then
				x = 2 * (s - 6.0) / 14.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.872768, tj, tj1, result)
				jbcheb(x, -1.430689, tj, tj1, result)
				jbcheb(x, 0.1136575, tj, tj1, result)
				jbcheb(x, -0.01726627, tj, tj1, result)
				jbcheb(x, 0.00342111, tj, tj1, result)
				jbcheb(x, -0.00158151, tj, tj1, result)
				jbcheb(x, -0.000555952, tj, tj1, result)
				jbcheb(x, -0.0006838208, tj, tj1, result)
				jbcheb(x, 0.0008428839, tj, tj1, result)
				jbcheb(x, -0.0007170682, tj, tj1, result)
				jbcheb(x, -0.0006006647, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1539373 * (s - 20.0)) - 7.206941
			Return result
		End Function


		Private Shared Function jbtbl20(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.854794, tj, tj1, result)
				jbcheb(x, -1.948947, tj, tj1, result)
				jbcheb(x, 0.1632184, tj, tj1, result)
				jbcheb(x, 0.2139397, tj, tj1, result)
				jbcheb(x, -0.1006237, tj, tj1, result)
				jbcheb(x, -0.03810031, tj, tj1, result)
				jbcheb(x, 0.0357362, tj, tj1, result)
				jbcheb(x, 0.009951242, tj, tj1, result)
				jbcheb(x, -0.01274092, tj, tj1, result)
				jbcheb(x, -0.003464196, tj, tj1, result)
				jbcheb(x, 0.004882139, tj, tj1, result)
				jbcheb(x, 0.001575144, tj, tj1, result)
				jbcheb(x, -0.001822804, tj, tj1, result)
				jbcheb(x, -0.0007061348, tj, tj1, result)
				jbcheb(x, 0.0005908404, tj, tj1, result)
				jbcheb(x, 0.0001978353, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.030989, tj, tj1, result)
				jbcheb(x, -1.327151, tj, tj1, result)
				jbcheb(x, 0.1346404, tj, tj1, result)
				jbcheb(x, -0.02840051, tj, tj1, result)
				jbcheb(x, 0.007578551, tj, tj1, result)
				jbcheb(x, -0.0009813886, tj, tj1, result)
				jbcheb(x, 5.905973E-05, tj, tj1, result)
				jbcheb(x, -0.0005358489, tj, tj1, result)
				jbcheb(x, -0.0003450795, tj, tj1, result)
				jbcheb(x, -0.0006941157, tj, tj1, result)
				jbcheb(x, -0.0007432418, tj, tj1, result)
				jbcheb(x, -0.0002070537, tj, tj1, result)
				jbcheb(x, 0.0009375654, tj, tj1, result)
				jbcheb(x, 0.0005367378, tj, tj1, result)
				jbcheb(x, 0.0009890859, tj, tj1, result)
				jbcheb(x, 0.0006679782, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -7.015854, tj, tj1, result)
				jbcheb(x, -0.7487737, tj, tj1, result)
				jbcheb(x, 0.02244254, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1318007 * (s - 25.0)) - 7.742185
			Return result
		End Function


		Private Shared Function jbtbl30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.630822, tj, tj1, result)
				jbcheb(x, -1.724298, tj, tj1, result)
				jbcheb(x, 0.07872756, tj, tj1, result)
				jbcheb(x, 0.1658268, tj, tj1, result)
				jbcheb(x, -0.03573597, tj, tj1, result)
				jbcheb(x, -0.02994157, tj, tj1, result)
				jbcheb(x, 0.005994825, tj, tj1, result)
				jbcheb(x, 0.007394303, tj, tj1, result)
				jbcheb(x, -0.0005785029, tj, tj1, result)
				jbcheb(x, -0.001990264, tj, tj1, result)
				jbcheb(x, -0.0001037838, tj, tj1, result)
				jbcheb(x, 0.0006755546, tj, tj1, result)
				jbcheb(x, 0.0001774473, tj, tj1, result)
				jbcheb(x, -0.0002821395, tj, tj1, result)
				jbcheb(x, -0.0001392603, tj, tj1, result)
				jbcheb(x, 0.0001353313, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.539322, tj, tj1, result)
				jbcheb(x, -1.197018, tj, tj1, result)
				jbcheb(x, 0.1396848, tj, tj1, result)
				jbcheb(x, -0.02804293, tj, tj1, result)
				jbcheb(x, 0.006867928, tj, tj1, result)
				jbcheb(x, -0.002768758, tj, tj1, result)
				jbcheb(x, 0.0005211792, tj, tj1, result)
				jbcheb(x, 0.0004925799, tj, tj1, result)
				jbcheb(x, 0.0005046235, tj, tj1, result)
				jbcheb(x, -9.536469E-05, tj, tj1, result)
				jbcheb(x, -0.0006489642, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.263462, tj, tj1, result)
				jbcheb(x, -0.6177316, tj, tj1, result)
				jbcheb(x, 0.02590637, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1028212 * (s - 25.0)) - 6.855288
			Return result
		End Function


		Private Shared Function jbtbl50(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.436279, tj, tj1, result)
				jbcheb(x, -1.519711, tj, tj1, result)
				jbcheb(x, 0.01148699, tj, tj1, result)
				jbcheb(x, 0.1001204, tj, tj1, result)
				jbcheb(x, -0.00320762, tj, tj1, result)
				jbcheb(x, -0.01034778, tj, tj1, result)
				jbcheb(x, -0.001220322, tj, tj1, result)
				jbcheb(x, 0.00103326, tj, tj1, result)
				jbcheb(x, 0.000258828, tj, tj1, result)
				jbcheb(x, -0.0001851653, tj, tj1, result)
				jbcheb(x, -0.0001287733, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.234645, tj, tj1, result)
				jbcheb(x, -1.189127, tj, tj1, result)
				jbcheb(x, 0.1429738, tj, tj1, result)
				jbcheb(x, -0.03058822, tj, tj1, result)
				jbcheb(x, 0.009086776, tj, tj1, result)
				jbcheb(x, -0.001445783, tj, tj1, result)
				jbcheb(x, 0.001311671, tj, tj1, result)
				jbcheb(x, -0.0007261298, tj, tj1, result)
				jbcheb(x, 0.0006496987, tj, tj1, result)
				jbcheb(x, 0.0002605249, tj, tj1, result)
				jbcheb(x, 0.0008162282, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.921095, tj, tj1, result)
				jbcheb(x, -0.5888603, tj, tj1, result)
				jbcheb(x, 0.03080113, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.09313116 * (s - 25.0)) - 6.479154
			Return result
		End Function


		Private Shared Function jbtbl65(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.360024, tj, tj1, result)
				jbcheb(x, -1.434631, tj, tj1, result)
				jbcheb(x, -0.00651458, tj, tj1, result)
				jbcheb(x, 0.07332038, tj, tj1, result)
				jbcheb(x, 0.001158197, tj, tj1, result)
				jbcheb(x, -0.005121233, tj, tj1, result)
				jbcheb(x, -0.001051056, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.148601, tj, tj1, result)
				jbcheb(x, -1.214233, tj, tj1, result)
				jbcheb(x, 0.1487977, tj, tj1, result)
				jbcheb(x, -0.0342472, tj, tj1, result)
				jbcheb(x, 0.01116715, tj, tj1, result)
				jbcheb(x, -0.004043152, tj, tj1, result)
				jbcheb(x, 0.001718149, tj, tj1, result)
				jbcheb(x, -0.001313701, tj, tj1, result)
				jbcheb(x, 0.0003097305, tj, tj1, result)
				jbcheb(x, 0.0002181031, tj, tj1, result)
				jbcheb(x, 0.0001256975, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.858951, tj, tj1, result)
				jbcheb(x, -0.5895179, tj, tj1, result)
				jbcheb(x, 0.02933237, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.09443768 * (s - 25.0)) - 6.419137
			Return result
		End Function


		Private Shared Function jbtbl100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.257021, tj, tj1, result)
				jbcheb(x, -1.313418, tj, tj1, result)
				jbcheb(x, -0.01628931, tj, tj1, result)
				jbcheb(x, 0.04264287, tj, tj1, result)
				jbcheb(x, 0.001518487, tj, tj1, result)
				jbcheb(x, -0.001499826, tj, tj1, result)
				jbcheb(x, -0.0004836044, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.056508, tj, tj1, result)
				jbcheb(x, -1.27969, tj, tj1, result)
				jbcheb(x, 0.1665746, tj, tj1, result)
				jbcheb(x, -0.04290012, tj, tj1, result)
				jbcheb(x, 0.01487632, tj, tj1, result)
				jbcheb(x, -0.005704465, tj, tj1, result)
				jbcheb(x, 0.002211669, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.866099, tj, tj1, result)
				jbcheb(x, -0.6399767, tj, tj1, result)
				jbcheb(x, 0.02498208, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1080097 * (s - 25.0)) - 6.481094
			Return result
		End Function


		Private Shared Function jbtbl130(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.207999, tj, tj1, result)
				jbcheb(x, -1.253864, tj, tj1, result)
				jbcheb(x, -0.01618032, tj, tj1, result)
				jbcheb(x, 0.03112729, tj, tj1, result)
				jbcheb(x, 0.001210546, tj, tj1, result)
				jbcheb(x, -0.0004732602, tj, tj1, result)
				jbcheb(x, -0.0002410527, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.026324, tj, tj1, result)
				jbcheb(x, -1.33199, tj, tj1, result)
				jbcheb(x, 0.1779129, tj, tj1, result)
				jbcheb(x, -0.04674749, tj, tj1, result)
				jbcheb(x, 0.01669077, tj, tj1, result)
				jbcheb(x, -0.005679136, tj, tj1, result)
				jbcheb(x, 0.0008833221, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -5.893951, tj, tj1, result)
				jbcheb(x, -0.6475304, tj, tj1, result)
				jbcheb(x, 0.03116734, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1045722 * (s - 25.0)) - 6.510314
			Return result
		End Function


		Private Shared Function jbtbl200(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.146155, tj, tj1, result)
				jbcheb(x, -1.177398, tj, tj1, result)
				jbcheb(x, -0.0129797, tj, tj1, result)
				jbcheb(x, 0.01869745, tj, tj1, result)
				jbcheb(x, 0.0001717288, tj, tj1, result)
				jbcheb(x, -0.0001982108, tj, tj1, result)
				jbcheb(x, 6.427636E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.034235, tj, tj1, result)
				jbcheb(x, -1.455006, tj, tj1, result)
				jbcheb(x, 0.1942996, tj, tj1, result)
				jbcheb(x, -0.04973795, tj, tj1, result)
				jbcheb(x, 0.01418812, tj, tj1, result)
				jbcheb(x, -0.003156778, tj, tj1, result)
				jbcheb(x, 4.896705E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.086071, tj, tj1, result)
				jbcheb(x, -0.7152176, tj, tj1, result)
				jbcheb(x, 0.03725393, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1132404 * (s - 25.0)) - 6.764034
			Return result
		End Function


		Private Shared Function jbtbl301(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.10429, tj, tj1, result)
				jbcheb(x, -1.1258, tj, tj1, result)
				jbcheb(x, -0.009595847, tj, tj1, result)
				jbcheb(x, 0.01219666, tj, tj1, result)
				jbcheb(x, 0.000150221, tj, tj1, result)
				jbcheb(x, -6.414543E-05, tj, tj1, result)
				jbcheb(x, 6.754115E-05, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.065955, tj, tj1, result)
				jbcheb(x, -1.58206, tj, tj1, result)
				jbcheb(x, 0.2004472, tj, tj1, result)
				jbcheb(x, -0.04709092, tj, tj1, result)
				jbcheb(x, 0.01105779, tj, tj1, result)
				jbcheb(x, 0.001197391, tj, tj1, result)
				jbcheb(x, -0.000838678, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.311384, tj, tj1, result)
				jbcheb(x, -0.7918763, tj, tj1, result)
				jbcheb(x, 0.03626584, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1293626 * (s - 25.0)) - 7.066995
			Return result
		End Function


		Private Shared Function jbtbl501(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.067426, tj, tj1, result)
				jbcheb(x, -1.079765, tj, tj1, result)
				jbcheb(x, -0.005463005, tj, tj1, result)
				jbcheb(x, 0.006875659, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.127574, tj, tj1, result)
				jbcheb(x, -1.740694, tj, tj1, result)
				jbcheb(x, 0.2044502, tj, tj1, result)
				jbcheb(x, -0.03746714, tj, tj1, result)
				jbcheb(x, 0.0003810594, tj, tj1, result)
				jbcheb(x, 0.001197111, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.628194, tj, tj1, result)
				jbcheb(x, -0.8846221, tj, tj1, result)
				jbcheb(x, 0.04386405, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1418332 * (s - 25.0)) - 7.468952
			Return result
		End Function


		Private Shared Function jbtbl701(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.050999, tj, tj1, result)
				jbcheb(x, -1.059769, tj, tj1, result)
				jbcheb(x, -0.00392268, tj, tj1, result)
				jbcheb(x, 0.004847054, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.192182, tj, tj1, result)
				jbcheb(x, -1.860007, tj, tj1, result)
				jbcheb(x, 0.1963942, tj, tj1, result)
				jbcheb(x, -0.02838711, tj, tj1, result)
				jbcheb(x, -0.0002893112, tj, tj1, result)
				jbcheb(x, 0.002159788, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -6.917851, tj, tj1, result)
				jbcheb(x, -0.981702, tj, tj1, result)
				jbcheb(x, 0.05383727, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.1532706 * (s - 25.0)) - 7.845715
			Return result
		End Function


		Private Shared Function jbtbl1401(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			If CDbl(s) <= CDbl(4.0) Then
				x = 2 * (s - 0.0) / 4.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -1.026266, tj, tj1, result)
				jbcheb(x, -1.030061, tj, tj1, result)
				jbcheb(x, -0.001259222, tj, tj1, result)
				jbcheb(x, 0.002536254, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(15.0) Then
				x = 2 * (s - 4.0) / 11.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -4.329849, tj, tj1, result)
				jbcheb(x, -2.095443, tj, tj1, result)
				jbcheb(x, 0.1759363, tj, tj1, result)
				jbcheb(x, -0.007751359, tj, tj1, result)
				jbcheb(x, -0.006124368, tj, tj1, result)
				jbcheb(x, -0.001793114, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			If CDbl(s) <= CDbl(25.0) Then
				x = 2 * (s - 15.0) / 10.0 - 1
				tj = 1
				tj1 = x
				jbcheb(x, -7.54433, tj, tj1, result)
				jbcheb(x, -1.225382, tj, tj1, result)
				jbcheb(x, 0.05392349, tj, tj1, result)
				If CDbl(result) > CDbl(0) Then
					result = 0
				End If
				Return result
			End If
			result = -(0.2019375 * (s - 25.0)) - 8.715788
			Return result
		End Function


		Private Shared Sub jbcheb(x As Double, c As Double, ByRef tj As Double, ByRef tj1 As Double, ByRef r As Double)
			Dim t As Double = 0

			r = r + c * tj
			t = 2 * x * tj1 - tj
			tj = tj1
			tj1 = t
		End Sub


	End Class
	Public Class mannwhitneyu
		'************************************************************************
'        Mann-Whitney U-test
'
'        This test checks hypotheses about whether X  and  Y  are  samples  of  two
'        continuous distributions of the same shape  and  same  median  or  whether
'        their medians are different.
'
'        The following tests are performed:
'            * two-tailed test (null hypothesis - the medians are equal)
'            * left-tailed test (null hypothesis - the median of the  first  sample
'              is greater than or equal to the median of the second sample)
'            * right-tailed test (null hypothesis - the median of the first  sample
'              is less than or equal to the median of the second sample).
'
'        Requirements:
'            * the samples are independent
'            * X and Y are continuous distributions (or discrete distributions well-
'              approximating continuous distributions)
'            * distributions of X and Y have the  same  shape.  The  only  possible
'              difference is their position (i.e. the value of the median)
'            * the number of elements in each sample is not less than 5
'            * the scale of measurement should be ordinal, interval or ratio  (i.e.
'              the test could not be applied to nominal variables).
'
'        The test is non-parametric and doesn't require distributions to be normal.
'
'        Input parameters:
'            X   -   sample 1. Array whose index goes from 0 to N-1.
'            N   -   size of the sample. N>=5
'            Y   -   sample 2. Array whose index goes from 0 to M-1.
'            M   -   size of the sample. M>=5
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'        To calculate p-values, special approximation is used. This method lets  us
'        calculate p-values with satisfactory  accuracy  in  interval  [0.0001, 1].
'        There is no approximation outside the [0.0001, 1] interval. Therefore,  if
'        the significance level outlies this interval, the test returns 0.0001.
'
'        Relative precision of approximation of p-value:
'
'        N          M          Max.err.   Rms.err.
'        5..10      N..10      1.4e-02    6.0e-04
'        5..10      N..100     2.2e-02    5.3e-06
'        10..15     N..15      1.0e-02    3.2e-04
'        10..15     N..100     1.0e-02    2.2e-05
'        15..100    N..100     6.1e-03    2.7e-06
'
'        For N,M>100 accuracy checks weren't put into  practice,  but  taking  into
'        account characteristics of asymptotic approximation used, precision should
'        not be sharply different from the values for interval [5, 100].
'
'          -- ALGLIB --
'             Copyright 09.04.2007 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub mannwhitneyutest(x As Double(), n As Integer, y As Double(), m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, _
			ByRef righttail As Double)
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim k As Integer = 0
			Dim t As Integer = 0
			Dim tmp As Double = 0
			Dim tmpi As Integer = 0
			Dim ns As Integer = 0
			Dim r As Double() = New Double(-1) {}
			Dim c As Integer() = New Integer(-1) {}
			Dim u As Double = 0
			Dim p As Double = 0
			Dim mp As Double = 0
			Dim s As Double = 0
			Dim sigma As Double = 0
			Dim mu As Double = 0
			Dim tiecount As Integer = 0
			Dim tiesize As Integer() = New Integer(-1) {}

			bothtails = 0
			lefttail = 0
			righttail = 0


			'
			' Prepare
			'
			If n <= 4 OrElse m <= 4 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If
			ns = n + m
			r = New Double(ns - 1) {}
			c = New Integer(ns - 1) {}
			For i = 0 To n - 1
				r(i) = x(i)
				c(i) = 0
			Next
			For i = 0 To m - 1
				r(n + i) = y(i)
				c(n + i) = 1
			Next

			'
			' sort {R, C}
			'
			If ns <> 1 Then
				i = 2
				Do
					t = i
					While t <> 1
						k = t \ 2
						If CDbl(r(k - 1)) >= CDbl(r(t - 1)) Then
							t = 1
						Else
							tmp = r(k - 1)
							r(k - 1) = r(t - 1)
							r(t - 1) = tmp
							tmpi = c(k - 1)
							c(k - 1) = c(t - 1)
							c(t - 1) = tmpi
							t = k
						End If
					End While
					i = i + 1
				Loop While i <= ns
				i = ns - 1
				Do
					tmp = r(i)
					r(i) = r(0)
					r(0) = tmp
					tmpi = c(i)
					c(i) = c(0)
					c(0) = tmpi
					t = 1
					While t <> 0
						k = 2 * t
						If k > i Then
							t = 0
						Else
							If k < i Then
								If CDbl(r(k)) > CDbl(r(k - 1)) Then
									k = k + 1
								End If
							End If
							If CDbl(r(t - 1)) >= CDbl(r(k - 1)) Then
								t = 0
							Else
								tmp = r(k - 1)
								r(k - 1) = r(t - 1)
								r(t - 1) = tmp
								tmpi = c(k - 1)
								c(k - 1) = c(t - 1)
								c(t - 1) = tmpi
								t = k
							End If
						End If
					End While
					i = i - 1
				Loop While i >= 1
			End If

			'
			' compute tied ranks
			'
			i = 0
			tiecount = 0
			tiesize = New Integer(ns - 1) {}
			While i <= ns - 1
				j = i + 1
				While j <= ns - 1
					If CDbl(r(j)) <> CDbl(r(i)) Then
						Exit While
					End If
					j = j + 1
				End While
				For k = i To j - 1
					r(k) = 1 + CDbl(i + j - 1) / CDbl(2)
				Next
				tiesize(tiecount) = j - i
				tiecount = tiecount + 1
				i = j
			End While

			'
			' Compute U
			'
			u = 0
			For i = 0 To ns - 1
				If c(i) = 0 Then
					u = u + r(i)
				End If
			Next
			u = n * m + n * (n + 1) \ 2 - u

			'
			' Result
			'
			mu = CDbl(n * m) / CDbl(2)
			tmp = ns * (Math.sqr(ns) - 1) / 12
			For i = 0 To tiecount - 1
				tmp = tmp - tiesize(i) * (Math.sqr(tiesize(i)) - 1) / 12
			Next
			sigma = System.Math.sqrt(CDbl(m * n) / CDbl(ns) / (ns - 1) * tmp)
			s = (u - mu) / sigma
			If CDbl(s) <= CDbl(0) Then
				p = System.Math.Exp(usigma(-((u - mu) / sigma), n, m))
				mp = 1 - System.Math.Exp(usigma(-((u - 1 - mu) / sigma), n, m))
			Else
				mp = System.Math.Exp(usigma((u - mu) / sigma, n, m))
				p = 1 - System.Math.Exp(usigma((u + 1 - mu) / sigma, n, m))
			End If
			bothtails = System.Math.Max(2 * System.Math.Min(p, mp), 0.0001)
			lefttail = System.Math.Max(mp, 0.0001)
			righttail = System.Math.Max(p, 0.0001)
		End Sub


		'************************************************************************
'        Sequential Chebyshev interpolation.
'        ************************************************************************

		Private Shared Sub ucheb(x As Double, c As Double, ByRef tj As Double, ByRef tj1 As Double, ByRef r As Double)
			Dim t As Double = 0

			r = r + c * tj
			t = 2 * x * tj1 - tj
			tj = tj1
			tj1 = t
		End Sub


		'************************************************************************
'        Three-point polynomial interpolation.
'        ************************************************************************

		Private Shared Function uninterpolate(p1 As Double, p2 As Double, p3 As Double, n As Integer) As Double
			Dim result As Double = 0
			Dim t1 As Double = 0
			Dim t2 As Double = 0
			Dim t3 As Double = 0
			Dim t As Double = 0
			Dim p12 As Double = 0
			Dim p23 As Double = 0

			t1 = 1.0 / 15.0
			t2 = 1.0 / 30.0
			t3 = 1.0 / 100.0
			t = 1.0 / n
			p12 = ((t - t2) * p1 + (t1 - t) * p2) / (t1 - t2)
			p23 = ((t - t3) * p2 + (t2 - t) * p3) / (t2 - t3)
			result = ((t - t3) * p12 + (t1 - t) * p23) / (t1 - t3)
			Return result
		End Function


		'************************************************************************
'        Tail(0, N1, N2)
'        ************************************************************************

		Private Shared Function usigma000(n1 As Integer, n2 As Integer) As Double
			Dim result As Double = 0
			Dim p1 As Double = 0
			Dim p2 As Double = 0
			Dim p3 As Double = 0

			p1 = uninterpolate(-0.676984, -0.6837, -0.689873, n2)
			p2 = uninterpolate(-0.6837, -0.687311, -0.690957, n2)
			p3 = uninterpolate(-0.689873, -0.690957, -0.692175, n2)
			result = uninterpolate(p1, p2, p3, n1)
			Return result
		End Function


		'************************************************************************
'        Tail(0.75, N1, N2)
'        ************************************************************************

		Private Shared Function usigma075(n1 As Integer, n2 As Integer) As Double
			Dim result As Double = 0
			Dim p1 As Double = 0
			Dim p2 As Double = 0
			Dim p3 As Double = 0

			p1 = uninterpolate(-1.445, -1.45906, -1.47063, n2)
			p2 = uninterpolate(-1.45906, -1.46856, -1.47644, n2)
			p3 = uninterpolate(-1.47063, -1.47644, -1.481, n2)
			result = uninterpolate(p1, p2, p3, n1)
			Return result
		End Function


		'************************************************************************
'        Tail(1.5, N1, N2)
'        ************************************************************************

		Private Shared Function usigma150(n1 As Integer, n2 As Integer) As Double
			Dim result As Double = 0
			Dim p1 As Double = 0
			Dim p2 As Double = 0
			Dim p3 As Double = 0

			p1 = uninterpolate(-2.6538, -2.67352, -2.69011, n2)
			p2 = uninterpolate(-2.67352, -2.68591, -2.69659, n2)
			p3 = uninterpolate(-2.69011, -2.69659, -2.70192, n2)
			result = uninterpolate(p1, p2, p3, n1)
			Return result
		End Function


		'************************************************************************
'        Tail(2.25, N1, N2)
'        ************************************************************************

		Private Shared Function usigma225(n1 As Integer, n2 As Integer) As Double
			Dim result As Double = 0
			Dim p1 As Double = 0
			Dim p2 As Double = 0
			Dim p3 As Double = 0

			p1 = uninterpolate(-4.41465, -4.4226, -4.43702, n2)
			p2 = uninterpolate(-4.4226, -4.41639, -4.41928, n2)
			p3 = uninterpolate(-4.43702, -4.41928, -4.4103, n2)
			result = uninterpolate(p1, p2, p3, n1)
			Return result
		End Function


		'************************************************************************
'        Tail(3.0, N1, N2)
'        ************************************************************************

		Private Shared Function usigma300(n1 As Integer, n2 As Integer) As Double
			Dim result As Double = 0
			Dim p1 As Double = 0
			Dim p2 As Double = 0
			Dim p3 As Double = 0

			p1 = uninterpolate(-6.89839, -6.83477, -6.8234, n2)
			p2 = uninterpolate(-6.83477, -6.74559, -6.71117, n2)
			p3 = uninterpolate(-6.8234, -6.71117, -6.64929, n2)
			result = uninterpolate(p1, p2, p3, n1)
			Return result
		End Function


		'************************************************************************
'        Tail(3.33, N1, N2)
'        ************************************************************************

		Private Shared Function usigma333(n1 As Integer, n2 As Integer) As Double
			Dim result As Double = 0
			Dim p1 As Double = 0
			Dim p2 As Double = 0
			Dim p3 As Double = 0

			p1 = uninterpolate(-8.31272, -8.17096, -8.13125, n2)
			p2 = uninterpolate(-8.17096, -8.00156, -7.93245, n2)
			p3 = uninterpolate(-8.13125, -7.93245, -7.82502, n2)
			result = uninterpolate(p1, p2, p3, n1)
			Return result
		End Function


		'************************************************************************
'        Tail(3.66, N1, N2)
'        ************************************************************************

		Private Shared Function usigma367(n1 As Integer, n2 As Integer) As Double
			Dim result As Double = 0
			Dim p1 As Double = 0
			Dim p2 As Double = 0
			Dim p3 As Double = 0

			p1 = uninterpolate(-9.98837, -9.70844, -9.62087, n2)
			p2 = uninterpolate(-9.70844, -9.41156, -9.28998, n2)
			p3 = uninterpolate(-9.62087, -9.28998, -9.11686, n2)
			result = uninterpolate(p1, p2, p3, n1)
			Return result
		End Function


		'************************************************************************
'        Tail(4.0, N1, N2)
'        ************************************************************************

		Private Shared Function usigma400(n1 As Integer, n2 As Integer) As Double
			Dim result As Double = 0
			Dim p1 As Double = 0
			Dim p2 As Double = 0
			Dim p3 As Double = 0

			p1 = uninterpolate(-12.025, -11.4911, -11.3231, n2)
			p2 = uninterpolate(-11.4911, -10.9927, -10.7937, n2)
			p3 = uninterpolate(-11.3231, -10.7937, -10.5285, n2)
			result = uninterpolate(p1, p2, p3, n1)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 5)
'        ************************************************************************

		Private Shared Function utbln5n5(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 2.611165 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -2.596264, tj, tj1, result)
			ucheb(x, -2.412086, tj, tj1, result)
			ucheb(x, -0.4858542, tj, tj1, result)
			ucheb(x, -0.05614282, tj, tj1, result)
			ucheb(x, 0.003372686, tj, tj1, result)
			ucheb(x, 0.008524731, tj, tj1, result)
			ucheb(x, 0.004435331, tj, tj1, result)
			ucheb(x, 0.001284665, tj, tj1, result)
			ucheb(x, 0.004184141, tj, tj1, result)
			ucheb(x, 0.00529836, tj, tj1, result)
			ucheb(x, 0.0007447272, tj, tj1, result)
			ucheb(x, -0.003938769, tj, tj1, result)
			ucheb(x, -0.004276205, tj, tj1, result)
			ucheb(x, -0.001138481, tj, tj1, result)
			ucheb(x, 0.0008684625, tj, tj1, result)
			ucheb(x, 0.001558104, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 6)
'        ************************************************************************

		Private Shared Function utbln5n6(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 2.738613 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -2.810459, tj, tj1, result)
			ucheb(x, -2.684429, tj, tj1, result)
			ucheb(x, -0.5712858, tj, tj1, result)
			ucheb(x, -0.08009324, tj, tj1, result)
			ucheb(x, -0.006644391, tj, tj1, result)
			ucheb(x, 0.006034173, tj, tj1, result)
			ucheb(x, 0.004953498, tj, tj1, result)
			ucheb(x, 0.003279293, tj, tj1, result)
			ucheb(x, 0.003563485, tj, tj1, result)
			ucheb(x, 0.004971952, tj, tj1, result)
			ucheb(x, 0.003506309, tj, tj1, result)
			ucheb(x, -0.0001541406, tj, tj1, result)
			ucheb(x, -0.003283205, tj, tj1, result)
			ucheb(x, -0.003016347, tj, tj1, result)
			ucheb(x, -0.001221626, tj, tj1, result)
			ucheb(x, -0.001286752, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 7)
'        ************************************************************************

		Private Shared Function utbln5n7(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 2.841993 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -2.994677, tj, tj1, result)
			ucheb(x, -2.923264, tj, tj1, result)
			ucheb(x, -0.650619, tj, tj1, result)
			ucheb(x, -0.105428, tj, tj1, result)
			ucheb(x, -0.01794587, tj, tj1, result)
			ucheb(x, 0.00172629, tj, tj1, result)
			ucheb(x, 0.00453418, tj, tj1, result)
			ucheb(x, 0.004517845, tj, tj1, result)
			ucheb(x, 0.003904428, tj, tj1, result)
			ucheb(x, 0.003882443, tj, tj1, result)
			ucheb(x, 0.003482988, tj, tj1, result)
			ucheb(x, 0.002114875, tj, tj1, result)
			ucheb(x, -0.0001515082, tj, tj1, result)
			ucheb(x, -0.001996056, tj, tj1, result)
			ucheb(x, -0.002293581, tj, tj1, result)
			ucheb(x, -0.002349444, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 8)
'        ************************************************************************

		Private Shared Function utbln5n8(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 2.9277 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.155727, tj, tj1, result)
			ucheb(x, -3.135078, tj, tj1, result)
			ucheb(x, -0.7247203, tj, tj1, result)
			ucheb(x, -0.1309697, tj, tj1, result)
			ucheb(x, -0.02993725, tj, tj1, result)
			ucheb(x, -0.003567219, tj, tj1, result)
			ucheb(x, 0.003383704, tj, tj1, result)
			ucheb(x, 0.005002188, tj, tj1, result)
			ucheb(x, 0.004487322, tj, tj1, result)
			ucheb(x, 0.003443899, tj, tj1, result)
			ucheb(x, 0.00268827, tj, tj1, result)
			ucheb(x, 0.002600339, tj, tj1, result)
			ucheb(x, 0.001874948, tj, tj1, result)
			ucheb(x, 0.0001811593, tj, tj1, result)
			ucheb(x, -0.001072353, tj, tj1, result)
			ucheb(x, -0.002659457, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 9)
'        ************************************************************************

		Private Shared Function utbln5n9(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.0 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.298162, tj, tj1, result)
			ucheb(x, -3.325016, tj, tj1, result)
			ucheb(x, -0.7939852, tj, tj1, result)
			ucheb(x, -0.1563029, tj, tj1, result)
			ucheb(x, -0.04222652, tj, tj1, result)
			ucheb(x, -0.0091952, tj, tj1, result)
			ucheb(x, 0.001445665, tj, tj1, result)
			ucheb(x, 0.005204792, tj, tj1, result)
			ucheb(x, 0.004775217, tj, tj1, result)
			ucheb(x, 0.003527781, tj, tj1, result)
			ucheb(x, 0.002221948, tj, tj1, result)
			ucheb(x, 0.002242968, tj, tj1, result)
			ucheb(x, 0.002607959, tj, tj1, result)
			ucheb(x, 0.001771285, tj, tj1, result)
			ucheb(x, 0.0006694026, tj, tj1, result)
			ucheb(x, -0.00148119, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 10)
'        ************************************************************************

		Private Shared Function utbln5n10(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.061862 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.42536, tj, tj1, result)
			ucheb(x, -3.49671, tj, tj1, result)
			ucheb(x, -0.8587658, tj, tj1, result)
			ucheb(x, -0.1812005, tj, tj1, result)
			ucheb(x, -0.05427637, tj, tj1, result)
			ucheb(x, -0.01515702, tj, tj1, result)
			ucheb(x, -0.0005406867, tj, tj1, result)
			ucheb(x, 0.004796295, tj, tj1, result)
			ucheb(x, 0.005237591, tj, tj1, result)
			ucheb(x, 0.003654249, tj, tj1, result)
			ucheb(x, 0.002181165, tj, tj1, result)
			ucheb(x, 0.002011665, tj, tj1, result)
			ucheb(x, 0.002417927, tj, tj1, result)
			ucheb(x, 0.00253488, tj, tj1, result)
			ucheb(x, 0.001791255, tj, tj1, result)
			ucheb(x, 1.871512E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 11)
'        ************************************************************************

		Private Shared Function utbln5n11(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.115427 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.539959, tj, tj1, result)
			ucheb(x, -3.652998, tj, tj1, result)
			ucheb(x, -0.9196503, tj, tj1, result)
			ucheb(x, -0.2054363, tj, tj1, result)
			ucheb(x, -0.06618848, tj, tj1, result)
			ucheb(x, -0.02109411, tj, tj1, result)
			ucheb(x, -0.002786668, tj, tj1, result)
			ucheb(x, 0.004215648, tj, tj1, result)
			ucheb(x, 0.00548422, tj, tj1, result)
			ucheb(x, 0.003935991, tj, tj1, result)
			ucheb(x, 0.002396191, tj, tj1, result)
			ucheb(x, 0.001894177, tj, tj1, result)
			ucheb(x, 0.002206979, tj, tj1, result)
			ucheb(x, 0.002519055, tj, tj1, result)
			ucheb(x, 0.002210326, tj, tj1, result)
			ucheb(x, 0.001189679, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 12)
'        ************************************************************************

		Private Shared Function utbln5n12(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.162278 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.644007, tj, tj1, result)
			ucheb(x, -3.796173, tj, tj1, result)
			ucheb(x, -0.9771177, tj, tj1, result)
			ucheb(x, -0.2290043, tj, tj1, result)
			ucheb(x, -0.07794686, tj, tj1, result)
			ucheb(x, -0.0270211, tj, tj1, result)
			ucheb(x, -0.005185959, tj, tj1, result)
			ucheb(x, 0.003416259, tj, tj1, result)
			ucheb(x, 0.005592056, tj, tj1, result)
			ucheb(x, 0.00420153, tj, tj1, result)
			ucheb(x, 0.002754365, tj, tj1, result)
			ucheb(x, 0.001978945, tj, tj1, result)
			ucheb(x, 0.002012032, tj, tj1, result)
			ucheb(x, 0.002304579, tj, tj1, result)
			ucheb(x, 0.002100378, tj, tj1, result)
			ucheb(x, 0.001728269, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 13)
'        ************************************************************************

		Private Shared Function utbln5n13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.203616 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.73912, tj, tj1, result)
			ucheb(x, -3.928117, tj, tj1, result)
			ucheb(x, -1.031605, tj, tj1, result)
			ucheb(x, -0.2519403, tj, tj1, result)
			ucheb(x, -0.08962648, tj, tj1, result)
			ucheb(x, -0.03292183, tj, tj1, result)
			ucheb(x, -0.007809293, tj, tj1, result)
			ucheb(x, 0.002465156, tj, tj1, result)
			ucheb(x, 0.005456278, tj, tj1, result)
			ucheb(x, 0.004446055, tj, tj1, result)
			ucheb(x, 0.00310949, tj, tj1, result)
			ucheb(x, 0.002218256, tj, tj1, result)
			ucheb(x, 0.001941479, tj, tj1, result)
			ucheb(x, 0.002058603, tj, tj1, result)
			ucheb(x, 0.001824402, tj, tj1, result)
			ucheb(x, 0.001830947, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 14)
'        ************************************************************************

		Private Shared Function utbln5n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.24037 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.826559, tj, tj1, result)
			ucheb(x, -4.05037, tj, tj1, result)
			ucheb(x, -1.083408, tj, tj1, result)
			ucheb(x, -0.2743164, tj, tj1, result)
			ucheb(x, -0.101203, tj, tj1, result)
			ucheb(x, -0.03884686, tj, tj1, result)
			ucheb(x, -0.01059656, tj, tj1, result)
			ucheb(x, 0.001327521, tj, tj1, result)
			ucheb(x, 0.005134026, tj, tj1, result)
			ucheb(x, 0.004584201, tj, tj1, result)
			ucheb(x, 0.003440618, tj, tj1, result)
			ucheb(x, 0.002524133, tj, tj1, result)
			ucheb(x, 0.001990007, tj, tj1, result)
			ucheb(x, 0.001887334, tj, tj1, result)
			ucheb(x, 0.001534977, tj, tj1, result)
			ucheb(x, 0.001705395, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 15)
'        ************************************************************************

		Private Shared Function utbln5n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.851572, tj, tj1, result)
			ucheb(x, -4.082033, tj, tj1, result)
			ucheb(x, -1.095983, tj, tj1, result)
			ucheb(x, -0.2814595, tj, tj1, result)
			ucheb(x, -0.1073148, tj, tj1, result)
			ucheb(x, -0.04420213, tj, tj1, result)
			ucheb(x, -0.01517175, tj, tj1, result)
			ucheb(x, -0.00234418, tj, tj1, result)
			ucheb(x, 0.002371393, tj, tj1, result)
			ucheb(x, 0.002711443, tj, tj1, result)
			ucheb(x, 0.002228569, tj, tj1, result)
			ucheb(x, 0.001683483, tj, tj1, result)
			ucheb(x, 0.001267112, tj, tj1, result)
			ucheb(x, 0.001156044, tj, tj1, result)
			ucheb(x, 0.0009131316, tj, tj1, result)
			ucheb(x, 0.001301023, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 16)
'        ************************************************************************

		Private Shared Function utbln5n16(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.85221, tj, tj1, result)
			ucheb(x, -4.077482, tj, tj1, result)
			ucheb(x, -1.091186, tj, tj1, result)
			ucheb(x, -0.2797282, tj, tj1, result)
			ucheb(x, -0.1084994, tj, tj1, result)
			ucheb(x, -0.04667054, tj, tj1, result)
			ucheb(x, -0.01843909, tj, tj1, result)
			ucheb(x, -0.005456732, tj, tj1, result)
			ucheb(x, -0.000503983, tj, tj1, result)
			ucheb(x, 0.0004723508, tj, tj1, result)
			ucheb(x, 0.0003940608, tj, tj1, result)
			ucheb(x, 0.0001478285, tj, tj1, result)
			ucheb(x, -0.0001649144, tj, tj1, result)
			ucheb(x, -0.0004237703, tj, tj1, result)
			ucheb(x, -0.000470741, tj, tj1, result)
			ucheb(x, -0.0001874293, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 17)
'        ************************************************************************

		Private Shared Function utbln5n17(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.851752, tj, tj1, result)
			ucheb(x, -4.071259, tj, tj1, result)
			ucheb(x, -1.0847, tj, tj1, result)
			ucheb(x, -0.2758898, tj, tj1, result)
			ucheb(x, -0.1073846, tj, tj1, result)
			ucheb(x, -0.04684838, tj, tj1, result)
			ucheb(x, -0.01964936, tj, tj1, result)
			ucheb(x, -0.006782442, tj, tj1, result)
			ucheb(x, -0.001956362, tj, tj1, result)
			ucheb(x, -0.0005984727, tj, tj1, result)
			ucheb(x, -0.0005196936, tj, tj1, result)
			ucheb(x, -0.0005558262, tj, tj1, result)
			ucheb(x, -0.0008690746, tj, tj1, result)
			ucheb(x, -0.001364855, tj, tj1, result)
			ucheb(x, -0.001401006, tj, tj1, result)
			ucheb(x, -0.001546748, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 18)
'        ************************************************************************

		Private Shared Function utbln5n18(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.85084, tj, tj1, result)
			ucheb(x, -4.064799, tj, tj1, result)
			ucheb(x, -1.077651, tj, tj1, result)
			ucheb(x, -0.2712659, tj, tj1, result)
			ucheb(x, -0.1049217, tj, tj1, result)
			ucheb(x, -0.04571333, tj, tj1, result)
			ucheb(x, -0.01929809, tj, tj1, result)
			ucheb(x, -0.006752044, tj, tj1, result)
			ucheb(x, -0.001949464, tj, tj1, result)
			ucheb(x, -0.0003896101, tj, tj1, result)
			ucheb(x, -4.61446E-05, tj, tj1, result)
			ucheb(x, 0.0001384357, tj, tj1, result)
			ucheb(x, -6.489113E-05, tj, tj1, result)
			ucheb(x, -0.0006445725, tj, tj1, result)
			ucheb(x, -0.0008945636, tj, tj1, result)
			ucheb(x, -0.001424653, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 19)
'        ************************************************************************

		Private Shared Function utbln5n19(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.850027, tj, tj1, result)
			ucheb(x, -4.059159, tj, tj1, result)
			ucheb(x, -1.071106, tj, tj1, result)
			ucheb(x, -0.266996, tj, tj1, result)
			ucheb(x, -0.102278, tj, tj1, result)
			ucheb(x, -0.04442555, tj, tj1, result)
			ucheb(x, -0.01851335, tj, tj1, result)
			ucheb(x, -0.006433865, tj, tj1, result)
			ucheb(x, -0.001514465, tj, tj1, result)
			ucheb(x, 0.0001332989, tj, tj1, result)
			ucheb(x, 0.0008606099, tj, tj1, result)
			ucheb(x, 0.001341945, tj, tj1, result)
			ucheb(x, 0.001402164, tj, tj1, result)
			ucheb(x, 0.001039761, tj, tj1, result)
			ucheb(x, 0.0005512831, tj, tj1, result)
			ucheb(x, -3.284427E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 20)
'        ************************************************************************

		Private Shared Function utbln5n20(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.849651, tj, tj1, result)
			ucheb(x, -4.054729, tj, tj1, result)
			ucheb(x, -1.065747, tj, tj1, result)
			ucheb(x, -0.2636243, tj, tj1, result)
			ucheb(x, -0.1003234, tj, tj1, result)
			ucheb(x, -0.04372789, tj, tj1, result)
			ucheb(x, -0.01831551, tj, tj1, result)
			ucheb(x, -0.00676309, tj, tj1, result)
			ucheb(x, -0.001830626, tj, tj1, result)
			ucheb(x, -0.0002122384, tj, tj1, result)
			ucheb(x, 0.0008108328, tj, tj1, result)
			ucheb(x, 0.001557983, tj, tj1, result)
			ucheb(x, 0.001945666, tj, tj1, result)
			ucheb(x, 0.001965696, tj, tj1, result)
			ucheb(x, 0.001493236, tj, tj1, result)
			ucheb(x, 0.001162591, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 21)
'        ************************************************************************

		Private Shared Function utbln5n21(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.849649, tj, tj1, result)
			ucheb(x, -4.051155, tj, tj1, result)
			ucheb(x, -1.06143, tj, tj1, result)
			ucheb(x, -0.2608869, tj, tj1, result)
			ucheb(x, -0.09902788, tj, tj1, result)
			ucheb(x, -0.04346562, tj, tj1, result)
			ucheb(x, -0.01874709, tj, tj1, result)
			ucheb(x, -0.007682887, tj, tj1, result)
			ucheb(x, -0.003026206, tj, tj1, result)
			ucheb(x, -0.001534551, tj, tj1, result)
			ucheb(x, -0.0004990575, tj, tj1, result)
			ucheb(x, 0.0003713334, tj, tj1, result)
			ucheb(x, 0.0009737011, tj, tj1, result)
			ucheb(x, 0.001304571, tj, tj1, result)
			ucheb(x, 0.00113311, tj, tj1, result)
			ucheb(x, 0.001123457, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 22)
'        ************************************************************************

		Private Shared Function utbln5n22(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.849598, tj, tj1, result)
			ucheb(x, -4.047605, tj, tj1, result)
			ucheb(x, -1.057264, tj, tj1, result)
			ucheb(x, -0.2579513, tj, tj1, result)
			ucheb(x, -0.09749602, tj, tj1, result)
			ucheb(x, -0.04275137, tj, tj1, result)
			ucheb(x, -0.01881768, tj, tj1, result)
			ucheb(x, -0.008177374, tj, tj1, result)
			ucheb(x, -0.003981056, tj, tj1, result)
			ucheb(x, -0.00269629, tj, tj1, result)
			ucheb(x, -0.001886803, tj, tj1, result)
			ucheb(x, -0.001085378, tj, tj1, result)
			ucheb(x, -0.0004675242, tj, tj1, result)
			ucheb(x, -5.426367E-05, tj, tj1, result)
			ucheb(x, 0.0001039613, tj, tj1, result)
			ucheb(x, 0.0002662378, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 23)
'        ************************************************************************

		Private Shared Function utbln5n23(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.849269, tj, tj1, result)
			ucheb(x, -4.043761, tj, tj1, result)
			ucheb(x, -1.052735, tj, tj1, result)
			ucheb(x, -0.2544683, tj, tj1, result)
			ucheb(x, -0.09517503, tj, tj1, result)
			ucheb(x, -0.04112082, tj, tj1, result)
			ucheb(x, -0.0178207, tj, tj1, result)
			ucheb(x, -0.007549483, tj, tj1, result)
			ucheb(x, -0.003747329, tj, tj1, result)
			ucheb(x, -0.002694263, tj, tj1, result)
			ucheb(x, -0.002147141, tj, tj1, result)
			ucheb(x, -0.001526209, tj, tj1, result)
			ucheb(x, -0.001039173, tj, tj1, result)
			ucheb(x, -0.0007235615, tj, tj1, result)
			ucheb(x, -0.0004656546, tj, tj1, result)
			ucheb(x, -0.0003014423, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 24)
'        ************************************************************************

		Private Shared Function utbln5n24(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.848925, tj, tj1, result)
			ucheb(x, -4.040178, tj, tj1, result)
			ucheb(x, -1.048355, tj, tj1, result)
			ucheb(x, -0.2510198, tj, tj1, result)
			ucheb(x, -0.09261134, tj, tj1, result)
			ucheb(x, -0.03915864, tj, tj1, result)
			ucheb(x, -0.01627423, tj, tj1, result)
			ucheb(x, -0.006307345, tj, tj1, result)
			ucheb(x, -0.002732992, tj, tj1, result)
			ucheb(x, -0.001869652, tj, tj1, result)
			ucheb(x, -0.001494176, tj, tj1, result)
			ucheb(x, -0.001047533, tj, tj1, result)
			ucheb(x, -0.0007178439, tj, tj1, result)
			ucheb(x, -0.0005424171, tj, tj1, result)
			ucheb(x, -0.0003829195, tj, tj1, result)
			ucheb(x, -0.000284081, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 25)
'        ************************************************************************

		Private Shared Function utbln5n25(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.848937, tj, tj1, result)
			ucheb(x, -4.037512, tj, tj1, result)
			ucheb(x, -1.044866, tj, tj1, result)
			ucheb(x, -0.2483269, tj, tj1, result)
			ucheb(x, -0.09063682, tj, tj1, result)
			ucheb(x, -0.03767778, tj, tj1, result)
			ucheb(x, -0.0150854, tj, tj1, result)
			ucheb(x, -0.005332756, tj, tj1, result)
			ucheb(x, -0.001881511, tj, tj1, result)
			ucheb(x, -0.001124041, tj, tj1, result)
			ucheb(x, -0.0008368456, tj, tj1, result)
			ucheb(x, -0.0004930499, tj, tj1, result)
			ucheb(x, -0.000277963, tj, tj1, result)
			ucheb(x, -0.0002029528, tj, tj1, result)
			ucheb(x, -0.0001658678, tj, tj1, result)
			ucheb(x, -0.0001289695, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 26)
'        ************************************************************************

		Private Shared Function utbln5n26(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.849416, tj, tj1, result)
			ucheb(x, -4.035915, tj, tj1, result)
			ucheb(x, -1.042493, tj, tj1, result)
			ucheb(x, -0.2466021, tj, tj1, result)
			ucheb(x, -0.08956432, tj, tj1, result)
			ucheb(x, -0.03698914, tj, tj1, result)
			ucheb(x, -0.01465689, tj, tj1, result)
			ucheb(x, -0.005035254, tj, tj1, result)
			ucheb(x, -0.001674614, tj, tj1, result)
			ucheb(x, -0.0009492734, tj, tj1, result)
			ucheb(x, -0.0007014021, tj, tj1, result)
			ucheb(x, -0.0003944953, tj, tj1, result)
			ucheb(x, -0.000225575, tj, tj1, result)
			ucheb(x, -0.0002075841, tj, tj1, result)
			ucheb(x, -0.000198933, tj, tj1, result)
			ucheb(x, -0.0002134862, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 27)
'        ************************************************************************

		Private Shared Function utbln5n27(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.85007, tj, tj1, result)
			ucheb(x, -4.034815, tj, tj1, result)
			ucheb(x, -1.04065, tj, tj1, result)
			ucheb(x, -0.2453117, tj, tj1, result)
			ucheb(x, -0.08886426, tj, tj1, result)
			ucheb(x, -0.03661702, tj, tj1, result)
			ucheb(x, -0.01452346, tj, tj1, result)
			ucheb(x, -0.005002476, tj, tj1, result)
			ucheb(x, -0.001720126, tj, tj1, result)
			ucheb(x, -0.0010014, tj, tj1, result)
			ucheb(x, -0.0007729826, tj, tj1, result)
			ucheb(x, -0.000474064, tj, tj1, result)
			ucheb(x, -0.0003206333, tj, tj1, result)
			ucheb(x, -0.0003366093, tj, tj1, result)
			ucheb(x, -0.0003193471, tj, tj1, result)
			ucheb(x, -0.0003804091, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 28)
'        ************************************************************************

		Private Shared Function utbln5n28(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.850668, tj, tj1, result)
			ucheb(x, -4.033786, tj, tj1, result)
			ucheb(x, -1.038853, tj, tj1, result)
			ucheb(x, -0.2440281, tj, tj1, result)
			ucheb(x, -0.0880602, tj, tj1, result)
			ucheb(x, -0.03612883, tj, tj1, result)
			ucheb(x, -0.01420436, tj, tj1, result)
			ucheb(x, -0.004787982, tj, tj1, result)
			ucheb(x, -0.00153523, tj, tj1, result)
			ucheb(x, -0.0008263121, tj, tj1, result)
			ucheb(x, -0.0005849609, tj, tj1, result)
			ucheb(x, -0.0002863967, tj, tj1, result)
			ucheb(x, -0.000139161, tj, tj1, result)
			ucheb(x, -0.0001720294, tj, tj1, result)
			ucheb(x, -0.0001952273, tj, tj1, result)
			ucheb(x, -0.0002901413, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 29)
'        ************************************************************************

		Private Shared Function utbln5n29(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.851217, tj, tj1, result)
			ucheb(x, -4.032834, tj, tj1, result)
			ucheb(x, -1.037113, tj, tj1, result)
			ucheb(x, -0.2427762, tj, tj1, result)
			ucheb(x, -0.08719146, tj, tj1, result)
			ucheb(x, -0.03557172, tj, tj1, result)
			ucheb(x, -0.01375498, tj, tj1, result)
			ucheb(x, -0.004452033, tj, tj1, result)
			ucheb(x, -0.001187516, tj, tj1, result)
			ucheb(x, -0.0004916936, tj, tj1, result)
			ucheb(x, -0.0002065533, tj, tj1, result)
			ucheb(x, 0.0001067301, tj, tj1, result)
			ucheb(x, 0.0002615824, tj, tj1, result)
			ucheb(x, 0.0002432244, tj, tj1, result)
			ucheb(x, 0.0001417795, tj, tj1, result)
			ucheb(x, 4.710038E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 30)
'        ************************************************************************

		Private Shared Function utbln5n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.851845, tj, tj1, result)
			ucheb(x, -4.032148, tj, tj1, result)
			ucheb(x, -1.035679, tj, tj1, result)
			ucheb(x, -0.2417758, tj, tj1, result)
			ucheb(x, -0.0865533, tj, tj1, result)
			ucheb(x, -0.03522132, tj, tj1, result)
			ucheb(x, -0.01352106, tj, tj1, result)
			ucheb(x, -0.004326911, tj, tj1, result)
			ucheb(x, -0.001064969, tj, tj1, result)
			ucheb(x, -0.0003813321, tj, tj1, result)
			ucheb(x, -5.683881E-05, tj, tj1, result)
			ucheb(x, 0.0002813346, tj, tj1, result)
			ucheb(x, 0.0004627085, tj, tj1, result)
			ucheb(x, 0.0004832107, tj, tj1, result)
			ucheb(x, 0.0003519336, tj, tj1, result)
			ucheb(x, 0.000288853, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 5, 100)
'        ************************************************************************

		Private Shared Function utbln5n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.25 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.87794, tj, tj1, result)
			ucheb(x, -4.039324, tj, tj1, result)
			ucheb(x, -1.022243, tj, tj1, result)
			ucheb(x, -0.2305825, tj, tj1, result)
			ucheb(x, -0.07960119, tj, tj1, result)
			ucheb(x, -0.03112, tj, tj1, result)
			ucheb(x, -0.01138868, tj, tj1, result)
			ucheb(x, -0.003418164, tj, tj1, result)
			ucheb(x, -0.000917452, tj, tj1, result)
			ucheb(x, -0.0005489617, tj, tj1, result)
			ucheb(x, -0.0003878301, tj, tj1, result)
			ucheb(x, -0.0001302233, tj, tj1, result)
			ucheb(x, 1.054113E-05, tj, tj1, result)
			ucheb(x, 2.458862E-05, tj, tj1, result)
			ucheb(x, -4.186591E-06, tj, tj1, result)
			ucheb(x, -2.623412E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 6)
'        ************************************************************************

		Private Shared Function utbln6n6(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 2.882307 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.054075, tj, tj1, result)
			ucheb(x, -2.998804, tj, tj1, result)
			ucheb(x, -0.6681518, tj, tj1, result)
			ucheb(x, -0.1067578, tj, tj1, result)
			ucheb(x, -0.01709435, tj, tj1, result)
			ucheb(x, 0.0009952661, tj, tj1, result)
			ucheb(x, 0.0036417, tj, tj1, result)
			ucheb(x, 0.002304572, tj, tj1, result)
			ucheb(x, 0.003336275, tj, tj1, result)
			ucheb(x, 0.004770385, tj, tj1, result)
			ucheb(x, 0.005401891, tj, tj1, result)
			ucheb(x, 0.002246148, tj, tj1, result)
			ucheb(x, -0.001442663, tj, tj1, result)
			ucheb(x, -0.002502866, tj, tj1, result)
			ucheb(x, -0.002105855, tj, tj1, result)
			ucheb(x, -0.0004739371, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 7)
'        ************************************************************************

		Private Shared Function utbln6n7(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.0 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.265287, tj, tj1, result)
			ucheb(x, -3.274613, tj, tj1, result)
			ucheb(x, -0.7582352, tj, tj1, result)
			ucheb(x, -0.1334293, tj, tj1, result)
			ucheb(x, -0.02915502, tj, tj1, result)
			ucheb(x, -0.004108091, tj, tj1, result)
			ucheb(x, 0.001546701, tj, tj1, result)
			ucheb(x, 0.002298827, tj, tj1, result)
			ucheb(x, 0.002891501, tj, tj1, result)
			ucheb(x, 0.004313717, tj, tj1, result)
			ucheb(x, 0.004989501, tj, tj1, result)
			ucheb(x, 0.003914594, tj, tj1, result)
			ucheb(x, 0.001062372, tj, tj1, result)
			ucheb(x, -0.001158841, tj, tj1, result)
			ucheb(x, -0.001596443, tj, tj1, result)
			ucheb(x, -0.001185662, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 8)
'        ************************************************************************

		Private Shared Function utbln6n8(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.098387 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.450954, tj, tj1, result)
			ucheb(x, -3.520462, tj, tj1, result)
			ucheb(x, -0.8420299, tj, tj1, result)
			ucheb(x, -0.1604853, tj, tj1, result)
			ucheb(x, -0.0416584, tj, tj1, result)
			ucheb(x, -0.01008756, tj, tj1, result)
			ucheb(x, -0.0006723402, tj, tj1, result)
			ucheb(x, 0.001843521, tj, tj1, result)
			ucheb(x, 0.002883405, tj, tj1, result)
			ucheb(x, 0.00372098, tj, tj1, result)
			ucheb(x, 0.004301709, tj, tj1, result)
			ucheb(x, 0.003948034, tj, tj1, result)
			ucheb(x, 0.002776243, tj, tj1, result)
			ucheb(x, 0.0008623736, tj, tj1, result)
			ucheb(x, -0.0003742068, tj, tj1, result)
			ucheb(x, -0.0009796927, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 9)
'        ************************************************************************

		Private Shared Function utbln6n9(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.181981 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.616113, tj, tj1, result)
			ucheb(x, -3.74165, tj, tj1, result)
			ucheb(x, -0.9204487, tj, tj1, result)
			ucheb(x, -0.1873068, tj, tj1, result)
			ucheb(x, -0.05446794, tj, tj1, result)
			ucheb(x, -0.01632286, tj, tj1, result)
			ucheb(x, -0.003266481, tj, tj1, result)
			ucheb(x, 0.001280067, tj, tj1, result)
			ucheb(x, 0.002780687, tj, tj1, result)
			ucheb(x, 0.003480242, tj, tj1, result)
			ucheb(x, 0.0035922, tj, tj1, result)
			ucheb(x, 0.003581019, tj, tj1, result)
			ucheb(x, 0.003264231, tj, tj1, result)
			ucheb(x, 0.002347174, tj, tj1, result)
			ucheb(x, 0.001167535, tj, tj1, result)
			ucheb(x, -0.0001092185, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 10)
'        ************************************************************************

		Private Shared Function utbln6n10(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.253957 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.764382, tj, tj1, result)
			ucheb(x, -3.942366, tj, tj1, result)
			ucheb(x, -0.9939896, tj, tj1, result)
			ucheb(x, -0.2137812, tj, tj1, result)
			ucheb(x, -0.0672027, tj, tj1, result)
			ucheb(x, -0.0228107, tj, tj1, result)
			ucheb(x, -0.00590106, tj, tj1, result)
			ucheb(x, 0.0003824937, tj, tj1, result)
			ucheb(x, 0.002802812, tj, tj1, result)
			ucheb(x, 0.003258132, tj, tj1, result)
			ucheb(x, 0.003233536, tj, tj1, result)
			ucheb(x, 0.00308553, tj, tj1, result)
			ucheb(x, 0.003212151, tj, tj1, result)
			ucheb(x, 0.003001329, tj, tj1, result)
			ucheb(x, 0.002226048, tj, tj1, result)
			ucheb(x, 0.001035298, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 11)
'        ************************************************************************

		Private Shared Function utbln6n11(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.316625 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.898597, tj, tj1, result)
			ucheb(x, -4.12571, tj, tj1, result)
			ucheb(x, -1.063297, tj, tj1, result)
			ucheb(x, -0.2396852, tj, tj1, result)
			ucheb(x, -0.07990126, tj, tj1, result)
			ucheb(x, -0.02927977, tj, tj1, result)
			ucheb(x, -0.0087265, tj, tj1, result)
			ucheb(x, -0.0005858745, tj, tj1, result)
			ucheb(x, 0.00265459, tj, tj1, result)
			ucheb(x, 0.003217736, tj, tj1, result)
			ucheb(x, 0.00298977, tj, tj1, result)
			ucheb(x, 0.002768493, tj, tj1, result)
			ucheb(x, 0.002924364, tj, tj1, result)
			ucheb(x, 0.003140215, tj, tj1, result)
			ucheb(x, 0.002647914, tj, tj1, result)
			ucheb(x, 0.001924802, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 12)
'        ************************************************************************

		Private Shared Function utbln6n12(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.371709 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.020941, tj, tj1, result)
			ucheb(x, -4.29425, tj, tj1, result)
			ucheb(x, -1.128842, tj, tj1, result)
			ucheb(x, -0.2650389, tj, tj1, result)
			ucheb(x, -0.09248611, tj, tj1, result)
			ucheb(x, -0.0357851, tj, tj1, result)
			ucheb(x, -0.01162852, tj, tj1, result)
			ucheb(x, -0.001746982, tj, tj1, result)
			ucheb(x, 0.002454209, tj, tj1, result)
			ucheb(x, 0.003128042, tj, tj1, result)
			ucheb(x, 0.00293665, tj, tj1, result)
			ucheb(x, 0.002530794, tj, tj1, result)
			ucheb(x, 0.002665192, tj, tj1, result)
			ucheb(x, 0.002994144, tj, tj1, result)
			ucheb(x, 0.002662249, tj, tj1, result)
			ucheb(x, 0.002368541, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 13)
'        ************************************************************************

		Private Shared Function utbln6n13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.420526 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.133167, tj, tj1, result)
			ucheb(x, -4.450016, tj, tj1, result)
			ucheb(x, -1.191088, tj, tj1, result)
			ucheb(x, -0.289822, tj, tj1, result)
			ucheb(x, -0.1050249, tj, tj1, result)
			ucheb(x, -0.04226901, tj, tj1, result)
			ucheb(x, -0.01471113, tj, tj1, result)
			ucheb(x, -0.00300747, tj, tj1, result)
			ucheb(x, 0.00204942, tj, tj1, result)
			ucheb(x, 0.003059074, tj, tj1, result)
			ucheb(x, 0.002881249, tj, tj1, result)
			ucheb(x, 0.00245278, tj, tj1, result)
			ucheb(x, 0.002441805, tj, tj1, result)
			ucheb(x, 0.002787493, tj, tj1, result)
			ucheb(x, 0.002483957, tj, tj1, result)
			ucheb(x, 0.00248159, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 14)
'        ************************************************************************

		Private Shared Function utbln6n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.45 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.201268, tj, tj1, result)
			ucheb(x, -4.542568, tj, tj1, result)
			ucheb(x, -1.226965, tj, tj1, result)
			ucheb(x, -0.3046029, tj, tj1, result)
			ucheb(x, -0.1136657, tj, tj1, result)
			ucheb(x, -0.04786757, tj, tj1, result)
			ucheb(x, -0.01843748, tj, tj1, result)
			ucheb(x, -0.005588022, tj, tj1, result)
			ucheb(x, 0.0002253029, tj, tj1, result)
			ucheb(x, 0.001667188, tj, tj1, result)
			ucheb(x, 0.00178833, tj, tj1, result)
			ucheb(x, 0.001474545, tj, tj1, result)
			ucheb(x, 0.001540494, tj, tj1, result)
			ucheb(x, 0.001951188, tj, tj1, result)
			ucheb(x, 0.001863323, tj, tj1, result)
			ucheb(x, 0.002220904, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 15)
'        ************************************************************************

		Private Shared Function utbln6n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.45 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.195689, tj, tj1, result)
			ucheb(x, -4.526567, tj, tj1, result)
			ucheb(x, -1.213617, tj, tj1, result)
			ucheb(x, -0.2975035, tj, tj1, result)
			ucheb(x, -0.111848, tj, tj1, result)
			ucheb(x, -0.04859142, tj, tj1, result)
			ucheb(x, -0.02083312, tj, tj1, result)
			ucheb(x, -0.00829872, tj, tj1, result)
			ucheb(x, -0.002766708, tj, tj1, result)
			ucheb(x, -0.001026356, tj, tj1, result)
			ucheb(x, -0.0009093113, tj, tj1, result)
			ucheb(x, -0.001135168, tj, tj1, result)
			ucheb(x, -0.001136376, tj, tj1, result)
			ucheb(x, -0.000819087, tj, tj1, result)
			ucheb(x, -0.0004435972, tj, tj1, result)
			ucheb(x, 0.0001413129, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 30)
'        ************************************************************************

		Private Shared Function utbln6n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.45 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.166269, tj, tj1, result)
			ucheb(x, -4.427399, tj, tj1, result)
			ucheb(x, -1.118239, tj, tj1, result)
			ucheb(x, -0.2360847, tj, tj1, result)
			ucheb(x, -0.07745885, tj, tj1, result)
			ucheb(x, -0.03025041, tj, tj1, result)
			ucheb(x, -0.01187179, tj, tj1, result)
			ucheb(x, -0.004432089, tj, tj1, result)
			ucheb(x, -0.001408451, tj, tj1, result)
			ucheb(x, -0.0004388774, tj, tj1, result)
			ucheb(x, -0.000279556, tj, tj1, result)
			ucheb(x, -0.0002304136, tj, tj1, result)
			ucheb(x, -0.0001258516, tj, tj1, result)
			ucheb(x, -4.180236E-05, tj, tj1, result)
			ucheb(x, -4.388679E-06, tj, tj1, result)
			ucheb(x, 4.836027E-06, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6, 100)
'        ************************************************************************

		Private Shared Function utbln6n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.45 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.18135, tj, tj1, result)
			ucheb(x, -4.417919, tj, tj1, result)
			ucheb(x, -1.094201, tj, tj1, result)
			ucheb(x, -0.2195883, tj, tj1, result)
			ucheb(x, -0.06818937, tj, tj1, result)
			ucheb(x, -0.02514202, tj, tj1, result)
			ucheb(x, -0.009125047, tj, tj1, result)
			ucheb(x, -0.003022148, tj, tj1, result)
			ucheb(x, -0.0007284181, tj, tj1, result)
			ucheb(x, -0.0001157766, tj, tj1, result)
			ucheb(x, -0.0001023752, tj, tj1, result)
			ucheb(x, -0.0001127985, tj, tj1, result)
			ucheb(x, -5.22169E-05, tj, tj1, result)
			ucheb(x, -3.516179E-06, tj, tj1, result)
			ucheb(x, 9.501398E-06, tj, tj1, result)
			ucheb(x, 9.38022E-06, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 7)
'        ************************************************************************

		Private Shared Function utbln7n7(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.130495 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.501264, tj, tj1, result)
			ucheb(x, -3.58479, tj, tj1, result)
			ucheb(x, -0.8577311, tj, tj1, result)
			ucheb(x, -0.1617002, tj, tj1, result)
			ucheb(x, -0.04145186, tj, tj1, result)
			ucheb(x, -0.01023462, tj, tj1, result)
			ucheb(x, -0.001408251, tj, tj1, result)
			ucheb(x, 0.0008626515, tj, tj1, result)
			ucheb(x, 0.002072492, tj, tj1, result)
			ucheb(x, 0.003722926, tj, tj1, result)
			ucheb(x, 0.005095445, tj, tj1, result)
			ucheb(x, 0.004842602, tj, tj1, result)
			ucheb(x, 0.002751427, tj, tj1, result)
			ucheb(x, 0.0002008927, tj, tj1, result)
			ucheb(x, -0.0009892431, tj, tj1, result)
			ucheb(x, -0.0008772386, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 8)
'        ************************************************************************

		Private Shared Function utbln7n8(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.24037 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.709965, tj, tj1, result)
			ucheb(x, -3.862154, tj, tj1, result)
			ucheb(x, -0.9504541, tj, tj1, result)
			ucheb(x, -0.1900195, tj, tj1, result)
			ucheb(x, -0.05439995, tj, tj1, result)
			ucheb(x, -0.01678028, tj, tj1, result)
			ucheb(x, -0.00448554, tj, tj1, result)
			ucheb(x, -0.0004437047, tj, tj1, result)
			ucheb(x, 0.001440092, tj, tj1, result)
			ucheb(x, 0.003114227, tj, tj1, result)
			ucheb(x, 0.004516569, tj, tj1, result)
			ucheb(x, 0.004829457, tj, tj1, result)
			ucheb(x, 0.00378755, tj, tj1, result)
			ucheb(x, 0.001761866, tj, tj1, result)
			ucheb(x, 0.0001991911, tj, tj1, result)
			ucheb(x, -0.0004533481, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 9)
'        ************************************************************************

		Private Shared Function utbln7n9(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.334314 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.89655, tj, tj1, result)
			ucheb(x, -4.112671, tj, tj1, result)
			ucheb(x, -1.037277, tj, tj1, result)
			ucheb(x, -0.2181695, tj, tj1, result)
			ucheb(x, -0.0676519, tj, tj1, result)
			ucheb(x, -0.02360116, tj, tj1, result)
			ucheb(x, -0.00769596, tj, tj1, result)
			ucheb(x, -0.001780578, tj, tj1, result)
			ucheb(x, 0.0008963843, tj, tj1, result)
			ucheb(x, 0.002616148, tj, tj1, result)
			ucheb(x, 0.003852104, tj, tj1, result)
			ucheb(x, 0.004390744, tj, tj1, result)
			ucheb(x, 0.004014041, tj, tj1, result)
			ucheb(x, 0.002888101, tj, tj1, result)
			ucheb(x, 0.001467474, tj, tj1, result)
			ucheb(x, 0.0004004611, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 10)
'        ************************************************************************

		Private Shared Function utbln7n10(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.41565 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.064844, tj, tj1, result)
			ucheb(x, -4.340749, tj, tj1, result)
			ucheb(x, -1.118888, tj, tj1, result)
			ucheb(x, -0.245973, tj, tj1, result)
			ucheb(x, -0.08097781, tj, tj1, result)
			ucheb(x, -0.03057688, tj, tj1, result)
			ucheb(x, -0.01097406, tj, tj1, result)
			ucheb(x, -0.003209262, tj, tj1, result)
			ucheb(x, 0.0004065641, tj, tj1, result)
			ucheb(x, 0.002196677, tj, tj1, result)
			ucheb(x, 0.003313994, tj, tj1, result)
			ucheb(x, 0.003827157, tj, tj1, result)
			ucheb(x, 0.003822284, tj, tj1, result)
			ucheb(x, 0.00338909, tj, tj1, result)
			ucheb(x, 0.00234085, tj, tj1, result)
			ucheb(x, 0.001395172, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 11)
'        ************************************************************************

		Private Shared Function utbln7n11(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.486817 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.217795, tj, tj1, result)
			ucheb(x, -4.549783, tj, tj1, result)
			ucheb(x, -1.195905, tj, tj1, result)
			ucheb(x, -0.2733093, tj, tj1, result)
			ucheb(x, -0.09428447, tj, tj1, result)
			ucheb(x, -0.03760093, tj, tj1, result)
			ucheb(x, -0.01431676, tj, tj1, result)
			ucheb(x, -0.004717152, tj, tj1, result)
			ucheb(x, -0.0001032199, tj, tj1, result)
			ucheb(x, 0.001832423, tj, tj1, result)
			ucheb(x, 0.002905979, tj, tj1, result)
			ucheb(x, 0.003302799, tj, tj1, result)
			ucheb(x, 0.003464371, tj, tj1, result)
			ucheb(x, 0.003456211, tj, tj1, result)
			ucheb(x, 0.002736244, tj, tj1, result)
			ucheb(x, 0.002140712, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 12)
'        ************************************************************************

		Private Shared Function utbln7n12(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.5 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.235822, tj, tj1, result)
			ucheb(x, -4.5641, tj, tj1, result)
			ucheb(x, -1.190813, tj, tj1, result)
			ucheb(x, -0.2686546, tj, tj1, result)
			ucheb(x, -0.09395083, tj, tj1, result)
			ucheb(x, -0.03967359, tj, tj1, result)
			ucheb(x, -0.01747096, tj, tj1, result)
			ucheb(x, -0.008304144, tj, tj1, result)
			ucheb(x, -0.003903198, tj, tj1, result)
			ucheb(x, -0.002134906, tj, tj1, result)
			ucheb(x, -0.001175035, tj, tj1, result)
			ucheb(x, -0.0007266224, tj, tj1, result)
			ucheb(x, -0.0001892931, tj, tj1, result)
			ucheb(x, 0.0005604706, tj, tj1, result)
			ucheb(x, 0.0009070459, tj, tj1, result)
			ucheb(x, 0.00142701, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 13)
'        ************************************************************************

		Private Shared Function utbln7n13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.5 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.222204, tj, tj1, result)
			ucheb(x, -4.5323, tj, tj1, result)
			ucheb(x, -1.164642, tj, tj1, result)
			ucheb(x, -0.2523768, tj, tj1, result)
			ucheb(x, -0.08531984, tj, tj1, result)
			ucheb(x, -0.03467857, tj, tj1, result)
			ucheb(x, -0.01483804, tj, tj1, result)
			ucheb(x, -0.006524136, tj, tj1, result)
			ucheb(x, -0.00307774, tj, tj1, result)
			ucheb(x, -0.001745218, tj, tj1, result)
			ucheb(x, -0.001602085, tj, tj1, result)
			ucheb(x, -0.001828831, tj, tj1, result)
			ucheb(x, -0.00199407, tj, tj1, result)
			ucheb(x, -0.001873879, tj, tj1, result)
			ucheb(x, -0.001341937, tj, tj1, result)
			ucheb(x, -0.0008706444, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 14)
'        ************************************************************************

		Private Shared Function utbln7n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.5 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.211763, tj, tj1, result)
			ucheb(x, -4.507542, tj, tj1, result)
			ucheb(x, -1.14364, tj, tj1, result)
			ucheb(x, -0.2395755, tj, tj1, result)
			ucheb(x, -0.0780802, tj, tj1, result)
			ucheb(x, -0.03044259, tj, tj1, result)
			ucheb(x, -0.01182308, tj, tj1, result)
			ucheb(x, -0.004057325, tj, tj1, result)
			ucheb(x, -0.0005724255, tj, tj1, result)
			ucheb(x, 0.00083039, tj, tj1, result)
			ucheb(x, 0.001113148, tj, tj1, result)
			ucheb(x, 0.0008102514, tj, tj1, result)
			ucheb(x, 0.0003559442, tj, tj1, result)
			ucheb(x, 4.634986E-05, tj, tj1, result)
			ucheb(x, -8.776476E-05, tj, tj1, result)
			ucheb(x, 1.054489E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 15)
'        ************************************************************************

		Private Shared Function utbln7n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.5 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.204898, tj, tj1, result)
			ucheb(x, -4.48996, tj, tj1, result)
			ucheb(x, -1.129172, tj, tj1, result)
			ucheb(x, -0.2316741, tj, tj1, result)
			ucheb(x, -0.07506107, tj, tj1, result)
			ucheb(x, -0.02983676, tj, tj1, result)
			ucheb(x, -0.01258013, tj, tj1, result)
			ucheb(x, -0.005262515, tj, tj1, result)
			ucheb(x, -0.001984156, tj, tj1, result)
			ucheb(x, -0.0003912108, tj, tj1, result)
			ucheb(x, 8.974023E-05, tj, tj1, result)
			ucheb(x, 6.056195E-05, tj, tj1, result)
			ucheb(x, -0.0002090842, tj, tj1, result)
			ucheb(x, -0.000523262, tj, tj1, result)
			ucheb(x, -0.0005816339, tj, tj1, result)
			ucheb(x, -0.0007020421, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 30)
'        ************************************************************************

		Private Shared Function utbln7n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.5 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.176536, tj, tj1, result)
			ucheb(x, -4.398705, tj, tj1, result)
			ucheb(x, -1.045481, tj, tj1, result)
			ucheb(x, -0.1821982, tj, tj1, result)
			ucheb(x, -0.04962304, tj, tj1, result)
			ucheb(x, -0.01698132, tj, tj1, result)
			ucheb(x, -0.006062667, tj, tj1, result)
			ucheb(x, -0.002282353, tj, tj1, result)
			ucheb(x, -0.0008014836, tj, tj1, result)
			ucheb(x, -0.0002035683, tj, tj1, result)
			ucheb(x, -1.004137E-05, tj, tj1, result)
			ucheb(x, 3.801453E-06, tj, tj1, result)
			ucheb(x, -1.920705E-05, tj, tj1, result)
			ucheb(x, -2.518735E-05, tj, tj1, result)
			ucheb(x, -1.821501E-05, tj, tj1, result)
			ucheb(x, -1.801008E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7, 100)
'        ************************************************************************

		Private Shared Function utbln7n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.5 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.188337, tj, tj1, result)
			ucheb(x, -4.386949, tj, tj1, result)
			ucheb(x, -1.022834, tj, tj1, result)
			ucheb(x, -0.1686517, tj, tj1, result)
			ucheb(x, -0.04323516, tj, tj1, result)
			ucheb(x, -0.01399392, tj, tj1, result)
			ucheb(x, -0.004644333, tj, tj1, result)
			ucheb(x, -0.001617044, tj, tj1, result)
			ucheb(x, -0.0005031396, tj, tj1, result)
			ucheb(x, -8.792066E-05, tj, tj1, result)
			ucheb(x, 2.675457E-05, tj, tj1, result)
			ucheb(x, 1.673416E-05, tj, tj1, result)
			ucheb(x, -6.258552E-06, tj, tj1, result)
			ucheb(x, -8.174214E-06, tj, tj1, result)
			ucheb(x, -3.073644E-06, tj, tj1, result)
			ucheb(x, -1.349958E-06, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 8)
'        ************************************************************************

		Private Shared Function utbln8n8(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.360672 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -3.940217, tj, tj1, result)
			ucheb(x, -4.168913, tj, tj1, result)
			ucheb(x, -1.051485, tj, tj1, result)
			ucheb(x, -0.2195325, tj, tj1, result)
			ucheb(x, -0.06775196, tj, tj1, result)
			ucheb(x, -0.02385506, tj, tj1, result)
			ucheb(x, -0.008244902, tj, tj1, result)
			ucheb(x, -0.002525632, tj, tj1, result)
			ucheb(x, 0.0002771275, tj, tj1, result)
			ucheb(x, 0.002332874, tj, tj1, result)
			ucheb(x, 0.004079599, tj, tj1, result)
			ucheb(x, 0.004882551, tj, tj1, result)
			ucheb(x, 0.004407944, tj, tj1, result)
			ucheb(x, 0.002769844, tj, tj1, result)
			ucheb(x, 0.001062433, tj, tj1, result)
			ucheb(x, 5.872535E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 9)
'        ************************************************************************

		Private Shared Function utbln8n9(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.464102 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.147004, tj, tj1, result)
			ucheb(x, -4.446939, tj, tj1, result)
			ucheb(x, -1.146155, tj, tj1, result)
			ucheb(x, -0.2488561, tj, tj1, result)
			ucheb(x, -0.08144561, tj, tj1, result)
			ucheb(x, -0.03116917, tj, tj1, result)
			ucheb(x, -0.01205667, tj, tj1, result)
			ucheb(x, -0.004515661, tj, tj1, result)
			ucheb(x, -0.0007618616, tj, tj1, result)
			ucheb(x, 0.001599011, tj, tj1, result)
			ucheb(x, 0.003457324, tj, tj1, result)
			ucheb(x, 0.004482917, tj, tj1, result)
			ucheb(x, 0.004488267, tj, tj1, result)
			ucheb(x, 0.003469823, tj, tj1, result)
			ucheb(x, 0.001957591, tj, tj1, result)
			ucheb(x, 0.0008058326, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 10)
'        ************************************************************************

		Private Shared Function utbln8n10(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.554093 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.334282, tj, tj1, result)
			ucheb(x, -4.70086, tj, tj1, result)
			ucheb(x, -1.235253, tj, tj1, result)
			ucheb(x, -0.2778489, tj, tj1, result)
			ucheb(x, -0.09527324, tj, tj1, result)
			ucheb(x, -0.03862885, tj, tj1, result)
			ucheb(x, -0.01589781, tj, tj1, result)
			ucheb(x, -0.006507355, tj, tj1, result)
			ucheb(x, -0.001717526, tj, tj1, result)
			ucheb(x, 0.0009215726, tj, tj1, result)
			ucheb(x, 0.002848696, tj, tj1, result)
			ucheb(x, 0.003918854, tj, tj1, result)
			ucheb(x, 0.004219614, tj, tj1, result)
			ucheb(x, 0.003753761, tj, tj1, result)
			ucheb(x, 0.002573688, tj, tj1, result)
			ucheb(x, 0.001602177, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 11)
'        ************************************************************************

		Private Shared Function utbln8n11(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.6 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.421882, tj, tj1, result)
			ucheb(x, -4.812457, tj, tj1, result)
			ucheb(x, -1.266153, tj, tj1, result)
			ucheb(x, -0.2849344, tj, tj1, result)
			ucheb(x, -0.09971527, tj, tj1, result)
			ucheb(x, -0.04258944, tj, tj1, result)
			ucheb(x, -0.0194482, tj, tj1, result)
			ucheb(x, -0.009894685, tj, tj1, result)
			ucheb(x, -0.005031836, tj, tj1, result)
			ucheb(x, -0.00251433, tj, tj1, result)
			ucheb(x, -0.000635166, tj, tj1, result)
			ucheb(x, 0.0006206748, tj, tj1, result)
			ucheb(x, 0.0014926, tj, tj1, result)
			ucheb(x, 0.002005338, tj, tj1, result)
			ucheb(x, 0.001780099, tj, tj1, result)
			ucheb(x, 0.001673599, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 12)
'        ************************************************************************

		Private Shared Function utbln8n12(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.6 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.398211, tj, tj1, result)
			ucheb(x, -4.762214, tj, tj1, result)
			ucheb(x, -1.226296, tj, tj1, result)
			ucheb(x, -0.2603837, tj, tj1, result)
			ucheb(x, -0.08643223, tj, tj1, result)
			ucheb(x, -0.03502438, tj, tj1, result)
			ucheb(x, -0.01544574, tj, tj1, result)
			ucheb(x, -0.007647734, tj, tj1, result)
			ucheb(x, -0.004442259, tj, tj1, result)
			ucheb(x, -0.003011484, tj, tj1, result)
			ucheb(x, -0.002384758, tj, tj1, result)
			ucheb(x, -0.001998259, tj, tj1, result)
			ucheb(x, -0.001659985, tj, tj1, result)
			ucheb(x, -0.001331046, tj, tj1, result)
			ucheb(x, -0.0008638478, tj, tj1, result)
			ucheb(x, -0.0006056785, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 13)
'        ************************************************************************

		Private Shared Function utbln8n13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.6 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.38067, tj, tj1, result)
			ucheb(x, -4.724511, tj, tj1, result)
			ucheb(x, -1.195851, tj, tj1, result)
			ucheb(x, -0.2420511, tj, tj1, result)
			ucheb(x, -0.07609928, tj, tj1, result)
			ucheb(x, -0.02893999, tj, tj1, result)
			ucheb(x, -0.01115919, tj, tj1, result)
			ucheb(x, -0.00429141, tj, tj1, result)
			ucheb(x, -0.001339664, tj, tj1, result)
			ucheb(x, -0.0001801548, tj, tj1, result)
			ucheb(x, 0.000253471, tj, tj1, result)
			ucheb(x, 0.000279325, tj, tj1, result)
			ucheb(x, 0.0001806718, tj, tj1, result)
			ucheb(x, 0.0001384624, tj, tj1, result)
			ucheb(x, 0.0001120582, tj, tj1, result)
			ucheb(x, 0.0002936453, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 14)
'        ************************************************************************

		Private Shared Function utbln8n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.6 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.368494, tj, tj1, result)
			ucheb(x, -4.697171, tj, tj1, result)
			ucheb(x, -1.17444, tj, tj1, result)
			ucheb(x, -0.2300621, tj, tj1, result)
			ucheb(x, -0.07087393, tj, tj1, result)
			ucheb(x, -0.02685826, tj, tj1, result)
			ucheb(x, -0.01085254, tj, tj1, result)
			ucheb(x, -0.004525658, tj, tj1, result)
			ucheb(x, -0.001966647, tj, tj1, result)
			ucheb(x, -0.0007453388, tj, tj1, result)
			ucheb(x, -0.0003826066, tj, tj1, result)
			ucheb(x, -0.0003501958, tj, tj1, result)
			ucheb(x, -0.0005336297, tj, tj1, result)
			ucheb(x, -0.0008251972, tj, tj1, result)
			ucheb(x, -0.0008118456, tj, tj1, result)
			ucheb(x, -0.0009415959, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 15)
'        ************************************************************************

		Private Shared Function utbln8n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.6 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.358397, tj, tj1, result)
			ucheb(x, -4.674485, tj, tj1, result)
			ucheb(x, -1.155941, tj, tj1, result)
			ucheb(x, -0.219578, tj, tj1, result)
			ucheb(x, -0.0654483, tj, tj1, result)
			ucheb(x, -0.02426183, tj, tj1, result)
			ucheb(x, -0.009309902, tj, tj1, result)
			ucheb(x, -0.003650956, tj, tj1, result)
			ucheb(x, -0.001068874, tj, tj1, result)
			ucheb(x, 0.0001538544, tj, tj1, result)
			ucheb(x, 0.0008192525, tj, tj1, result)
			ucheb(x, 0.001073905, tj, tj1, result)
			ucheb(x, 0.001079673, tj, tj1, result)
			ucheb(x, 0.0009423572, tj, tj1, result)
			ucheb(x, 0.0006579647, tj, tj1, result)
			ucheb(x, 0.0004765904, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 30)
'        ************************************************************************

		Private Shared Function utbln8n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.6 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.318823, tj, tj1, result)
			ucheb(x, -4.567159, tj, tj1, result)
			ucheb(x, -1.064864, tj, tj1, result)
			ucheb(x, -0.1688413, tj, tj1, result)
			ucheb(x, -0.04153712, tj, tj1, result)
			ucheb(x, -0.01309389, tj, tj1, result)
			ucheb(x, -0.004226861, tj, tj1, result)
			ucheb(x, -0.001523815, tj, tj1, result)
			ucheb(x, -0.0005780987, tj, tj1, result)
			ucheb(x, -0.0002166866, tj, tj1, result)
			ucheb(x, -6.922431E-05, tj, tj1, result)
			ucheb(x, -1.466397E-05, tj, tj1, result)
			ucheb(x, -5.690036E-06, tj, tj1, result)
			ucheb(x, -1.008185E-05, tj, tj1, result)
			ucheb(x, -9.271903E-06, tj, tj1, result)
			ucheb(x, -7.534751E-06, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8, 100)
'        ************************************************************************

		Private Shared Function utbln8n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.6 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.324531, tj, tj1, result)
			ucheb(x, -4.547071, tj, tj1, result)
			ucheb(x, -1.038129, tj, tj1, result)
			ucheb(x, -0.1541549, tj, tj1, result)
			ucheb(x, -0.03525605, tj, tj1, result)
			ucheb(x, -0.01044992, tj, tj1, result)
			ucheb(x, -0.003085713, tj, tj1, result)
			ucheb(x, -0.001017871, tj, tj1, result)
			ucheb(x, -0.0003459226, tj, tj1, result)
			ucheb(x, -0.0001092064, tj, tj1, result)
			ucheb(x, -2.024349E-05, tj, tj1, result)
			ucheb(x, 7.366347E-06, tj, tj1, result)
			ucheb(x, 6.385637E-06, tj, tj1, result)
			ucheb(x, 8.321722E-08, tj, tj1, result)
			ucheb(x, -1.439286E-06, tj, tj1, result)
			ucheb(x, -3.058079E-07, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9, 9)
'        ************************************************************************

		Private Shared Function utbln9n9(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.576237 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.372857, tj, tj1, result)
			ucheb(x, -4.750859, tj, tj1, result)
			ucheb(x, -1.248233, tj, tj1, result)
			ucheb(x, -0.2792868, tj, tj1, result)
			ucheb(x, -0.09559372, tj, tj1, result)
			ucheb(x, -0.03894941, tj, tj1, result)
			ucheb(x, -0.01643256, tj, tj1, result)
			ucheb(x, -0.00709137, tj, tj1, result)
			ucheb(x, -0.002285034, tj, tj1, result)
			ucheb(x, 0.0006112997, tj, tj1, result)
			ucheb(x, 0.002806229, tj, tj1, result)
			ucheb(x, 0.004150741, tj, tj1, result)
			ucheb(x, 0.004509825, tj, tj1, result)
			ucheb(x, 0.003891051, tj, tj1, result)
			ucheb(x, 0.002485013, tj, tj1, result)
			ucheb(x, 0.001343653, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9, 10)
'        ************************************************************************

		Private Shared Function utbln9n10(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.516726, tj, tj1, result)
			ucheb(x, -4.939333, tj, tj1, result)
			ucheb(x, -1.305046, tj, tj1, result)
			ucheb(x, -0.2935326, tj, tj1, result)
			ucheb(x, -0.1029141, tj, tj1, result)
			ucheb(x, -0.04420592, tj, tj1, result)
			ucheb(x, -0.0205314, tj, tj1, result)
			ucheb(x, -0.0106593, tj, tj1, result)
			ucheb(x, -0.005523581, tj, tj1, result)
			ucheb(x, -0.002544888, tj, tj1, result)
			ucheb(x, -0.0001813741, tj, tj1, result)
			ucheb(x, 0.001510631, tj, tj1, result)
			ucheb(x, 0.002536057, tj, tj1, result)
			ucheb(x, 0.002833815, tj, tj1, result)
			ucheb(x, 0.002189692, tj, tj1, result)
			ucheb(x, 0.00161505, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9, 11)
'        ************************************************************************

		Private Shared Function utbln9n11(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.481308, tj, tj1, result)
			ucheb(x, -4.867483, tj, tj1, result)
			ucheb(x, -1.249072, tj, tj1, result)
			ucheb(x, -0.259179, tj, tj1, result)
			ucheb(x, -0.08400128, tj, tj1, result)
			ucheb(x, -0.03341992, tj, tj1, result)
			ucheb(x, -0.0146368, tj, tj1, result)
			ucheb(x, -0.007487211, tj, tj1, result)
			ucheb(x, -0.004671196, tj, tj1, result)
			ucheb(x, -0.003343472, tj, tj1, result)
			ucheb(x, -0.002544146, tj, tj1, result)
			ucheb(x, -0.001802335, tj, tj1, result)
			ucheb(x, -0.001117084, tj, tj1, result)
			ucheb(x, -0.0006217443, tj, tj1, result)
			ucheb(x, -0.0002858766, tj, tj1, result)
			ucheb(x, -0.0003193687, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9, 12)
'        ************************************************************************

		Private Shared Function utbln9n12(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.456776, tj, tj1, result)
			ucheb(x, -4.817037, tj, tj1, result)
			ucheb(x, -1.209788, tj, tj1, result)
			ucheb(x, -0.2362108, tj, tj1, result)
			ucheb(x, -0.07171356, tj, tj1, result)
			ucheb(x, -0.02661557, tj, tj1, result)
			ucheb(x, -0.01026141, tj, tj1, result)
			ucheb(x, -0.004361908, tj, tj1, result)
			ucheb(x, -0.002093885, tj, tj1, result)
			ucheb(x, -0.001298389, tj, tj1, result)
			ucheb(x, -0.0009663603, tj, tj1, result)
			ucheb(x, -0.0007768522, tj, tj1, result)
			ucheb(x, -0.0005579015, tj, tj1, result)
			ucheb(x, -0.0002868677, tj, tj1, result)
			ucheb(x, -7.440652E-05, tj, tj1, result)
			ucheb(x, 0.0001523037, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9, 13)
'        ************************************************************************

		Private Shared Function utbln9n13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.43884, tj, tj1, result)
			ucheb(x, -4.779308, tj, tj1, result)
			ucheb(x, -1.180614, tj, tj1, result)
			ucheb(x, -0.2196489, tj, tj1, result)
			ucheb(x, -0.06346621, tj, tj1, result)
			ucheb(x, -0.02234857, tj, tj1, result)
			ucheb(x, -0.007796211, tj, tj1, result)
			ucheb(x, -0.002575715, tj, tj1, result)
			ucheb(x, -0.0005525647, tj, tj1, result)
			ucheb(x, 0.0001964651, tj, tj1, result)
			ucheb(x, 0.0004275235, tj, tj1, result)
			ucheb(x, 0.0004299124, tj, tj1, result)
			ucheb(x, 0.0003397416, tj, tj1, result)
			ucheb(x, 0.0002295781, tj, tj1, result)
			ucheb(x, 0.0001237619, tj, tj1, result)
			ucheb(x, 7.269692E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9, 14)
'        ************************************************************************

		Private Shared Function utbln9n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.425981, tj, tj1, result)
			ucheb(x, -4.751545, tj, tj1, result)
			ucheb(x, -1.159543, tj, tj1, result)
			ucheb(x, -0.208657, tj, tj1, result)
			ucheb(x, -0.05917446, tj, tj1, result)
			ucheb(x, -0.02120112, tj, tj1, result)
			ucheb(x, -0.008175519, tj, tj1, result)
			ucheb(x, -0.003515473, tj, tj1, result)
			ucheb(x, -0.001727772, tj, tj1, result)
			ucheb(x, -0.0009070629, tj, tj1, result)
			ucheb(x, -0.0005677569, tj, tj1, result)
			ucheb(x, -0.0003876953, tj, tj1, result)
			ucheb(x, -0.0003233502, tj, tj1, result)
			ucheb(x, -0.0003508182, tj, tj1, result)
			ucheb(x, -0.0003120389, tj, tj1, result)
			ucheb(x, -0.0003847212, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9, 15)
'        ************************************************************************

		Private Shared Function utbln9n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.414952, tj, tj1, result)
			ucheb(x, -4.727612, tj, tj1, result)
			ucheb(x, -1.140634, tj, tj1, result)
			ucheb(x, -0.1981231, tj, tj1, result)
			ucheb(x, -0.05382635, tj, tj1, result)
			ucheb(x, -0.01853575, tj, tj1, result)
			ucheb(x, -0.006571051, tj, tj1, result)
			ucheb(x, -0.002567625, tj, tj1, result)
			ucheb(x, -0.0009214197, tj, tj1, result)
			ucheb(x, -0.00024487, tj, tj1, result)
			ucheb(x, 0.0001712669, tj, tj1, result)
			ucheb(x, 0.000401505, tj, tj1, result)
			ucheb(x, 0.000543861, tj, tj1, result)
			ucheb(x, 0.0006301363, tj, tj1, result)
			ucheb(x, 0.0005309386, tj, tj1, result)
			ucheb(x, 0.0005164772, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9, 30)
'        ************************************************************************

		Private Shared Function utbln9n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.37072, tj, tj1, result)
			ucheb(x, -4.615712, tj, tj1, result)
			ucheb(x, -1.050023, tj, tj1, result)
			ucheb(x, -0.1504775, tj, tj1, result)
			ucheb(x, -0.03318265, tj, tj1, result)
			ucheb(x, -0.009646826, tj, tj1, result)
			ucheb(x, -0.002741492, tj, tj1, result)
			ucheb(x, -0.000873536, tj, tj1, result)
			ucheb(x, -0.0002966911, tj, tj1, result)
			ucheb(x, -0.0001100738, tj, tj1, result)
			ucheb(x, -4.348991E-05, tj, tj1, result)
			ucheb(x, -1.527687E-05, tj, tj1, result)
			ucheb(x, -2.917286E-06, tj, tj1, result)
			ucheb(x, 3.397466E-07, tj, tj1, result)
			ucheb(x, -2.360175E-07, tj, tj1, result)
			ucheb(x, -9.892252E-07, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9, 100)
'        ************************************************************************

		Private Shared Function utbln9n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.372506, tj, tj1, result)
			ucheb(x, -4.590966, tj, tj1, result)
			ucheb(x, -1.021758, tj, tj1, result)
			ucheb(x, -0.1359849, tj, tj1, result)
			ucheb(x, -0.02755519, tj, tj1, result)
			ucheb(x, -0.007533166, tj, tj1, result)
			ucheb(x, -0.001936659, tj, tj1, result)
			ucheb(x, -0.0005634913, tj, tj1, result)
			ucheb(x, -0.0001730053, tj, tj1, result)
			ucheb(x, -5.791845E-05, tj, tj1, result)
			ucheb(x, -2.030682E-05, tj, tj1, result)
			ucheb(x, -5.228663E-06, tj, tj1, result)
			ucheb(x, 8.631175E-07, tj, tj1, result)
			ucheb(x, 1.636749E-06, tj, tj1, result)
			ucheb(x, 4.404599E-07, tj, tj1, result)
			ucheb(x, -2.789872E-07, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 10, 10)
'        ************************************************************************

		Private Shared Function utbln10n10(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.468831, tj, tj1, result)
			ucheb(x, -4.844398, tj, tj1, result)
			ucheb(x, -1.231728, tj, tj1, result)
			ucheb(x, -0.2486073, tj, tj1, result)
			ucheb(x, -0.07781321, tj, tj1, result)
			ucheb(x, -0.02971425, tj, tj1, result)
			ucheb(x, -0.01215371, tj, tj1, result)
			ucheb(x, -0.005828451, tj, tj1, result)
			ucheb(x, -0.003419872, tj, tj1, result)
			ucheb(x, -0.002430165, tj, tj1, result)
			ucheb(x, -0.001740363, tj, tj1, result)
			ucheb(x, -0.001049211, tj, tj1, result)
			ucheb(x, -0.0003269371, tj, tj1, result)
			ucheb(x, 0.0002211393, tj, tj1, result)
			ucheb(x, 0.0004232314, tj, tj1, result)
			ucheb(x, 0.0003016081, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 10, 11)
'        ************************************************************************

		Private Shared Function utbln10n11(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.437998, tj, tj1, result)
			ucheb(x, -4.782296, tj, tj1, result)
			ucheb(x, -1.184732, tj, tj1, result)
			ucheb(x, -0.2219585, tj, tj1, result)
			ucheb(x, -0.06457012, tj, tj1, result)
			ucheb(x, -0.02296008, tj, tj1, result)
			ucheb(x, -0.008481501, tj, tj1, result)
			ucheb(x, -0.00352794, tj, tj1, result)
			ucheb(x, -0.001953426, tj, tj1, result)
			ucheb(x, -0.00156384, tj, tj1, result)
			ucheb(x, -0.001574403, tj, tj1, result)
			ucheb(x, -0.001535775, tj, tj1, result)
			ucheb(x, -0.001338037, tj, tj1, result)
			ucheb(x, -0.001002654, tj, tj1, result)
			ucheb(x, -0.0005852676, tj, tj1, result)
			ucheb(x, -0.0003318132, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 10, 12)
'        ************************************************************************

		Private Shared Function utbln10n12(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.416082, tj, tj1, result)
			ucheb(x, -4.737458, tj, tj1, result)
			ucheb(x, -1.150952, tj, tj1, result)
			ucheb(x, -0.2036884, tj, tj1, result)
			ucheb(x, -0.0560903, tj, tj1, result)
			ucheb(x, -0.01908684, tj, tj1, result)
			ucheb(x, -0.006439666, tj, tj1, result)
			ucheb(x, -0.002162647, tj, tj1, result)
			ucheb(x, -0.0006451601, tj, tj1, result)
			ucheb(x, -0.0002148757, tj, tj1, result)
			ucheb(x, -0.0001803981, tj, tj1, result)
			ucheb(x, -0.0002731621, tj, tj1, result)
			ucheb(x, -0.0003346903, tj, tj1, result)
			ucheb(x, -0.0003013151, tj, tj1, result)
			ucheb(x, -0.0001956148, tj, tj1, result)
			ucheb(x, -2.438381E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 10, 13)
'        ************************************************************************

		Private Shared Function utbln10n13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.39948, tj, tj1, result)
			ucheb(x, -4.702863, tj, tj1, result)
			ucheb(x, -1.124829, tj, tj1, result)
			ucheb(x, -0.1897428, tj, tj1, result)
			ucheb(x, -0.04979802, tj, tj1, result)
			ucheb(x, -0.01634368, tj, tj1, result)
			ucheb(x, -0.005180461, tj, tj1, result)
			ucheb(x, -0.001484926, tj, tj1, result)
			ucheb(x, -7.864376E-05, tj, tj1, result)
			ucheb(x, 0.0004186576, tj, tj1, result)
			ucheb(x, 0.0005886925, tj, tj1, result)
			ucheb(x, 0.0005836828, tj, tj1, result)
			ucheb(x, 0.0005074756, tj, tj1, result)
			ucheb(x, 0.0004209547, tj, tj1, result)
			ucheb(x, 0.0002883266, tj, tj1, result)
			ucheb(x, 0.0002380143, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 10, 14)
'        ************************************************************************

		Private Shared Function utbln10n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.386924, tj, tj1, result)
			ucheb(x, -4.676124, tj, tj1, result)
			ucheb(x, -1.10474, tj, tj1, result)
			ucheb(x, -0.1793826, tj, tj1, result)
			ucheb(x, -0.04558886, tj, tj1, result)
			ucheb(x, -0.01492462, tj, tj1, result)
			ucheb(x, -0.005052903, tj, tj1, result)
			ucheb(x, -0.001917782, tj, tj1, result)
			ucheb(x, -0.0007878696, tj, tj1, result)
			ucheb(x, -0.0003576046, tj, tj1, result)
			ucheb(x, -0.0001764551, tj, tj1, result)
			ucheb(x, -9.288778E-05, tj, tj1, result)
			ucheb(x, -4.757658E-05, tj, tj1, result)
			ucheb(x, -2.299101E-05, tj, tj1, result)
			ucheb(x, -9.265197E-06, tj, tj1, result)
			ucheb(x, -2.384503E-07, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 10, 15)
'        ************************************************************************

		Private Shared Function utbln10n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.376846, tj, tj1, result)
			ucheb(x, -4.654247, tj, tj1, result)
			ucheb(x, -1.088083, tj, tj1, result)
			ucheb(x, -0.1705945, tj, tj1, result)
			ucheb(x, -0.04169677, tj, tj1, result)
			ucheb(x, -0.01317213, tj, tj1, result)
			ucheb(x, -0.004264836, tj, tj1, result)
			ucheb(x, -0.001548024, tj, tj1, result)
			ucheb(x, -0.000663391, tj, tj1, result)
			ucheb(x, -0.0003505621, tj, tj1, result)
			ucheb(x, -0.0002658588, tj, tj1, result)
			ucheb(x, -0.0002320254, tj, tj1, result)
			ucheb(x, -0.0002175277, tj, tj1, result)
			ucheb(x, -0.0002122317, tj, tj1, result)
			ucheb(x, -0.0001675688, tj, tj1, result)
			ucheb(x, -0.0001661363, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 10, 30)
'        ************************************************************************

		Private Shared Function utbln10n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.333977, tj, tj1, result)
			ucheb(x, -4.548099, tj, tj1, result)
			ucheb(x, -1.004444, tj, tj1, result)
			ucheb(x, -0.1291014, tj, tj1, result)
			ucheb(x, -0.02523674, tj, tj1, result)
			ucheb(x, -0.006828211, tj, tj1, result)
			ucheb(x, -0.001716917, tj, tj1, result)
			ucheb(x, -0.0004894256, tj, tj1, result)
			ucheb(x, -0.0001433371, tj, tj1, result)
			ucheb(x, -4.522675E-05, tj, tj1, result)
			ucheb(x, -1.764192E-05, tj, tj1, result)
			ucheb(x, -9.140235E-06, tj, tj1, result)
			ucheb(x, -5.62923E-06, tj, tj1, result)
			ucheb(x, -3.541895E-06, tj, tj1, result)
			ucheb(x, -1.944946E-06, tj, tj1, result)
			ucheb(x, -1.72636E-06, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 10, 100)
'        ************************************************************************

		Private Shared Function utbln10n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.65 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.334008, tj, tj1, result)
			ucheb(x, -4.522316, tj, tj1, result)
			ucheb(x, -0.9769627, tj, tj1, result)
			ucheb(x, -0.115811, tj, tj1, result)
			ucheb(x, -0.0205365, tj, tj1, result)
			ucheb(x, -0.005242235, tj, tj1, result)
			ucheb(x, -0.001173571, tj, tj1, result)
			ucheb(x, -0.0003033661, tj, tj1, result)
			ucheb(x, -7.824732E-05, tj, tj1, result)
			ucheb(x, -2.08442E-05, tj, tj1, result)
			ucheb(x, -6.610036E-06, tj, tj1, result)
			ucheb(x, -2.728155E-06, tj, tj1, result)
			ucheb(x, -1.21713E-06, tj, tj1, result)
			ucheb(x, -2.340966E-07, tj, tj1, result)
			ucheb(x, 2.001235E-07, tj, tj1, result)
			ucheb(x, 1.694052E-07, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 11, 11)
'        ************************************************************************

		Private Shared Function utbln11n11(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.51976, tj, tj1, result)
			ucheb(x, -4.880694, tj, tj1, result)
			ucheb(x, -1.200698, tj, tj1, result)
			ucheb(x, -0.2174092, tj, tj1, result)
			ucheb(x, -0.06072304, tj, tj1, result)
			ucheb(x, -0.02054773, tj, tj1, result)
			ucheb(x, -0.006506613, tj, tj1, result)
			ucheb(x, -0.001813942, tj, tj1, result)
			ucheb(x, -0.0001223644, tj, tj1, result)
			ucheb(x, 0.0002417416, tj, tj1, result)
			ucheb(x, 0.0002499166, tj, tj1, result)
			ucheb(x, 0.0001194332, tj, tj1, result)
			ucheb(x, 7.369096E-05, tj, tj1, result)
			ucheb(x, 0.000196859, tj, tj1, result)
			ucheb(x, 0.0002630532, tj, tj1, result)
			ucheb(x, 0.0005061, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 11, 12)
'        ************************************************************************

		Private Shared Function utbln11n12(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.49579, tj, tj1, result)
			ucheb(x, -4.832622, tj, tj1, result)
			ucheb(x, -1.16542, tj, tj1, result)
			ucheb(x, -0.1987306, tj, tj1, result)
			ucheb(x, -0.05265621, tj, tj1, result)
			ucheb(x, -0.01723537, tj, tj1, result)
			ucheb(x, -0.005347406, tj, tj1, result)
			ucheb(x, -0.001353464, tj, tj1, result)
			ucheb(x, 6.613369E-05, tj, tj1, result)
			ucheb(x, 0.0005102522, tj, tj1, result)
			ucheb(x, 0.0005237709, tj, tj1, result)
			ucheb(x, 0.0003665652, tj, tj1, result)
			ucheb(x, 0.0001626903, tj, tj1, result)
			ucheb(x, -1.167518E-05, tj, tj1, result)
			ucheb(x, -8.564455E-05, tj, tj1, result)
			ucheb(x, -0.000104732, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 11, 13)
'        ************************************************************************

		Private Shared Function utbln11n13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.47788, tj, tj1, result)
			ucheb(x, -4.796242, tj, tj1, result)
			ucheb(x, -1.138769, tj, tj1, result)
			ucheb(x, -0.1851739, tj, tj1, result)
			ucheb(x, -0.04722104, tj, tj1, result)
			ucheb(x, -0.01548304, tj, tj1, result)
			ucheb(x, -0.005176683, tj, tj1, result)
			ucheb(x, -0.001817895, tj, tj1, result)
			ucheb(x, -0.0005842451, tj, tj1, result)
			ucheb(x, -8.93587E-05, tj, tj1, result)
			ucheb(x, 8.421777E-05, tj, tj1, result)
			ucheb(x, 0.0001238831, tj, tj1, result)
			ucheb(x, 8.867026E-05, tj, tj1, result)
			ucheb(x, 1.458255E-05, tj, tj1, result)
			ucheb(x, -3.306259E-05, tj, tj1, result)
			ucheb(x, -8.961487E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 11, 14)
'        ************************************************************************

		Private Shared Function utbln11n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.463683, tj, tj1, result)
			ucheb(x, -4.766969, tj, tj1, result)
			ucheb(x, -1.117082, tj, tj1, result)
			ucheb(x, -0.1739574, tj, tj1, result)
			ucheb(x, -0.04238865, tj, tj1, result)
			ucheb(x, -0.01350306, tj, tj1, result)
			ucheb(x, -0.004425871, tj, tj1, result)
			ucheb(x, -0.001640172, tj, tj1, result)
			ucheb(x, -0.0006660633, tj, tj1, result)
			ucheb(x, -0.0002879883, tj, tj1, result)
			ucheb(x, -0.0001349658, tj, tj1, result)
			ucheb(x, -6.271795E-05, tj, tj1, result)
			ucheb(x, -3.304544E-05, tj, tj1, result)
			ucheb(x, -3.024201E-05, tj, tj1, result)
			ucheb(x, -2.816867E-05, tj, tj1, result)
			ucheb(x, -4.596787E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 11, 15)
'        ************************************************************************

		Private Shared Function utbln11n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.452526, tj, tj1, result)
			ucheb(x, -4.74357, tj, tj1, result)
			ucheb(x, -1.099705, tj, tj1, result)
			ucheb(x, -0.1650612, tj, tj1, result)
			ucheb(x, -0.03858285, tj, tj1, result)
			ucheb(x, -0.01187036, tj, tj1, result)
			ucheb(x, -0.003689241, tj, tj1, result)
			ucheb(x, -0.00129436, tj, tj1, result)
			ucheb(x, -0.0005072623, tj, tj1, result)
			ucheb(x, -0.0002278008, tj, tj1, result)
			ucheb(x, -0.0001322382, tj, tj1, result)
			ucheb(x, -9.131558E-05, tj, tj1, result)
			ucheb(x, -7.305669E-05, tj, tj1, result)
			ucheb(x, -6.825627E-05, tj, tj1, result)
			ucheb(x, -5.332689E-05, tj, tj1, result)
			ucheb(x, -6.120973E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 11, 30)
'        ************************************************************************

		Private Shared Function utbln11n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.402621, tj, tj1, result)
			ucheb(x, -4.62744, tj, tj1, result)
			ucheb(x, -1.011333, tj, tj1, result)
			ucheb(x, -0.1224126, tj, tj1, result)
			ucheb(x, -0.02232856, tj, tj1, result)
			ucheb(x, -0.005859347, tj, tj1, result)
			ucheb(x, -0.001377381, tj, tj1, result)
			ucheb(x, -0.0003756709, tj, tj1, result)
			ucheb(x, -0.000103323, tj, tj1, result)
			ucheb(x, -2.875472E-05, tj, tj1, result)
			ucheb(x, -8.608399E-06, tj, tj1, result)
			ucheb(x, -3.102943E-06, tj, tj1, result)
			ucheb(x, -1.740693E-06, tj, tj1, result)
			ucheb(x, -1.343139E-06, tj, tj1, result)
			ucheb(x, -9.196878E-07, tj, tj1, result)
			ucheb(x, -6.658062E-07, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 11, 100)
'        ************************************************************************

		Private Shared Function utbln11n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.398795, tj, tj1, result)
			ucheb(x, -4.596486, tj, tj1, result)
			ucheb(x, -0.9814761, tj, tj1, result)
			ucheb(x, -0.1085187, tj, tj1, result)
			ucheb(x, -0.01766529, tj, tj1, result)
			ucheb(x, -0.004379425, tj, tj1, result)
			ucheb(x, -0.0008986351, tj, tj1, result)
			ucheb(x, -0.0002214705, tj, tj1, result)
			ucheb(x, -5.360075E-05, tj, tj1, result)
			ucheb(x, -1.260869E-05, tj, tj1, result)
			ucheb(x, -3.033307E-06, tj, tj1, result)
			ucheb(x, -7.727087E-07, tj, tj1, result)
			ucheb(x, -3.393883E-07, tj, tj1, result)
			ucheb(x, -2.242989E-07, tj, tj1, result)
			ucheb(x, -1.111928E-07, tj, tj1, result)
			ucheb(x, 3.898823E-09, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 12, 12)
'        ************************************************************************

		Private Shared Function utbln12n12(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.472616, tj, tj1, result)
			ucheb(x, -4.786627, tj, tj1, result)
			ucheb(x, -1.132099, tj, tj1, result)
			ucheb(x, -0.1817523, tj, tj1, result)
			ucheb(x, -0.04570179, tj, tj1, result)
			ucheb(x, -0.01479511, tj, tj1, result)
			ucheb(x, -0.004799492, tj, tj1, result)
			ucheb(x, -0.00156535, tj, tj1, result)
			ucheb(x, -0.0003530139, tj, tj1, result)
			ucheb(x, 0.0001380132, tj, tj1, result)
			ucheb(x, 0.0003242761, tj, tj1, result)
			ucheb(x, 0.0003576269, tj, tj1, result)
			ucheb(x, 0.0003018771, tj, tj1, result)
			ucheb(x, 0.0001933911, tj, tj1, result)
			ucheb(x, 9.002799E-05, tj, tj1, result)
			ucheb(x, -2.022048E-06, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 12, 13)
'        ************************************************************************

		Private Shared Function utbln12n13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.4548, tj, tj1, result)
			ucheb(x, -4.750794, tj, tj1, result)
			ucheb(x, -1.105988, tj, tj1, result)
			ucheb(x, -0.1684754, tj, tj1, result)
			ucheb(x, -0.04011826, tj, tj1, result)
			ucheb(x, -0.01262579, tj, tj1, result)
			ucheb(x, -0.004044492, tj, tj1, result)
			ucheb(x, -0.001478741, tj, tj1, result)
			ucheb(x, -0.0005322165, tj, tj1, result)
			ucheb(x, -0.0001621104, tj, tj1, result)
			ucheb(x, 4.068753E-05, tj, tj1, result)
			ucheb(x, 0.0001468396, tj, tj1, result)
			ucheb(x, 0.0002056235, tj, tj1, result)
			ucheb(x, 0.0002327375, tj, tj1, result)
			ucheb(x, 0.0001914877, tj, tj1, result)
			ucheb(x, 0.0001784191, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 12, 14)
'        ************************************************************************

		Private Shared Function utbln12n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.44091, tj, tj1, result)
			ucheb(x, -4.722404, tj, tj1, result)
			ucheb(x, -1.085254, tj, tj1, result)
			ucheb(x, -0.1579439, tj, tj1, result)
			ucheb(x, -0.03563738, tj, tj1, result)
			ucheb(x, -0.0106673, tj, tj1, result)
			ucheb(x, -0.003129346, tj, tj1, result)
			ucheb(x, -0.001014531, tj, tj1, result)
			ucheb(x, -0.0003129679, tj, tj1, result)
			ucheb(x, -8.000909E-05, tj, tj1, result)
			ucheb(x, 1.996174E-05, tj, tj1, result)
			ucheb(x, 6.377924E-05, tj, tj1, result)
			ucheb(x, 8.936304E-05, tj, tj1, result)
			ucheb(x, 0.0001051098, tj, tj1, result)
			ucheb(x, 9.02582E-05, tj, tj1, result)
			ucheb(x, 8.730585E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 12, 15)
'        ************************************************************************

		Private Shared Function utbln12n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.430123, tj, tj1, result)
			ucheb(x, -4.700008, tj, tj1, result)
			ucheb(x, -1.068971, tj, tj1, result)
			ucheb(x, -0.1499725, tj, tj1, result)
			ucheb(x, -0.03250897, tj, tj1, result)
			ucheb(x, -0.009473145, tj, tj1, result)
			ucheb(x, -0.002680008, tj, tj1, result)
			ucheb(x, -0.000848335, tj, tj1, result)
			ucheb(x, -0.0002766992, tj, tj1, result)
			ucheb(x, -9.891081E-05, tj, tj1, result)
			ucheb(x, -4.01514E-05, tj, tj1, result)
			ucheb(x, -1.977756E-05, tj, tj1, result)
			ucheb(x, -8.707414E-06, tj, tj1, result)
			ucheb(x, 1.114786E-06, tj, tj1, result)
			ucheb(x, 6.238865E-06, tj, tj1, result)
			ucheb(x, 1.381445E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 12, 30)
'        ************************************************************************

		Private Shared Function utbln12n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.380023, tj, tj1, result)
			ucheb(x, -4.585782, tj, tj1, result)
			ucheb(x, -0.9838583, tj, tj1, result)
			ucheb(x, -0.1103394, tj, tj1, result)
			ucheb(x, -0.01834015, tj, tj1, result)
			ucheb(x, -0.004635212, tj, tj1, result)
			ucheb(x, -0.0009948212, tj, tj1, result)
			ucheb(x, -0.0002574169, tj, tj1, result)
			ucheb(x, -6.74798E-05, tj, tj1, result)
			ucheb(x, -1.833672E-05, tj, tj1, result)
			ucheb(x, -5.722433E-06, tj, tj1, result)
			ucheb(x, -2.181038E-06, tj, tj1, result)
			ucheb(x, -1.206473E-06, tj, tj1, result)
			ucheb(x, -9.716003E-07, tj, tj1, result)
			ucheb(x, -7.476434E-07, tj, tj1, result)
			ucheb(x, -7.2177E-07, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 12, 100)
'        ************************************************************************

		Private Shared Function utbln12n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.7 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.374567, tj, tj1, result)
			ucheb(x, -4.553481, tj, tj1, result)
			ucheb(x, -0.9541334, tj, tj1, result)
			ucheb(x, -0.09701907, tj, tj1, result)
			ucheb(x, -0.01414757, tj, tj1, result)
			ucheb(x, -0.003404103, tj, tj1, result)
			ucheb(x, -0.0006234388, tj, tj1, result)
			ucheb(x, -0.0001453762, tj, tj1, result)
			ucheb(x, -3.31106E-05, tj, tj1, result)
			ucheb(x, -7.317501E-06, tj, tj1, result)
			ucheb(x, -1.713888E-06, tj, tj1, result)
			ucheb(x, -3.309583E-07, tj, tj1, result)
			ucheb(x, -4.019804E-08, tj, tj1, result)
			ucheb(x, 1.224829E-09, tj, tj1, result)
			ucheb(x, -1.349019E-08, tj, tj1, result)
			ucheb(x, -1.893302E-08, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 13, 13)
'        ************************************************************************

		Private Shared Function utbln13n13(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.75 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.541046, tj, tj1, result)
			ucheb(x, -4.859047, tj, tj1, result)
			ucheb(x, -1.130164, tj, tj1, result)
			ucheb(x, -0.1689719, tj, tj1, result)
			ucheb(x, -0.03950693, tj, tj1, result)
			ucheb(x, -0.01231455, tj, tj1, result)
			ucheb(x, -0.00397655, tj, tj1, result)
			ucheb(x, -0.001538455, tj, tj1, result)
			ucheb(x, -0.0007245603, tj, tj1, result)
			ucheb(x, -0.0004142647, tj, tj1, result)
			ucheb(x, -0.0002831434, tj, tj1, result)
			ucheb(x, -0.0002032483, tj, tj1, result)
			ucheb(x, -0.0001488405, tj, tj1, result)
			ucheb(x, -0.0001156927, tj, tj1, result)
			ucheb(x, -7.949279E-05, tj, tj1, result)
			ucheb(x, -7.5327E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 13, 14)
'        ************************************************************************

		Private Shared Function utbln13n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.75 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.525655, tj, tj1, result)
			ucheb(x, -4.828341, tj, tj1, result)
			ucheb(x, -1.10811, tj, tj1, result)
			ucheb(x, -0.1579552, tj, tj1, result)
			ucheb(x, -0.03488307, tj, tj1, result)
			ucheb(x, -0.01032328, tj, tj1, result)
			ucheb(x, -0.002988741, tj, tj1, result)
			ucheb(x, -0.0009766394, tj, tj1, result)
			ucheb(x, -0.000338895, tj, tj1, result)
			ucheb(x, -0.0001338179, tj, tj1, result)
			ucheb(x, -6.13344E-05, tj, tj1, result)
			ucheb(x, -3.023518E-05, tj, tj1, result)
			ucheb(x, -1.11057E-05, tj, tj1, result)
			ucheb(x, 4.202332E-06, tj, tj1, result)
			ucheb(x, 1.056132E-05, tj, tj1, result)
			ucheb(x, 1.536323E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 13, 15)
'        ************************************************************************

		Private Shared Function utbln13n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.75 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.513585, tj, tj1, result)
			ucheb(x, -4.803952, tj, tj1, result)
			ucheb(x, -1.090686, tj, tj1, result)
			ucheb(x, -0.149531, tj, tj1, result)
			ucheb(x, -0.03160314, tj, tj1, result)
			ucheb(x, -0.009073124, tj, tj1, result)
			ucheb(x, -0.002480313, tj, tj1, result)
			ucheb(x, -0.0007478239, tj, tj1, result)
			ucheb(x, -0.0002140914, tj, tj1, result)
			ucheb(x, -5.311541E-05, tj, tj1, result)
			ucheb(x, -2.677105E-06, tj, tj1, result)
			ucheb(x, 1.115464E-05, tj, tj1, result)
			ucheb(x, 1.578563E-05, tj, tj1, result)
			ucheb(x, 2.044604E-05, tj, tj1, result)
			ucheb(x, 1.888939E-05, tj, tj1, result)
			ucheb(x, 2.395644E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 13, 30)
'        ************************************************************************

		Private Shared Function utbln13n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.75 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.455999, tj, tj1, result)
			ucheb(x, -4.678434, tj, tj1, result)
			ucheb(x, -0.9995491, tj, tj1, result)
			ucheb(x, -0.10781, tj, tj1, result)
			ucheb(x, -0.0170522, tj, tj1, result)
			ucheb(x, -0.004258739, tj, tj1, result)
			ucheb(x, -0.0008671526, tj, tj1, result)
			ucheb(x, -0.0002185458, tj, tj1, result)
			ucheb(x, -5.507764E-05, tj, tj1, result)
			ucheb(x, -1.411446E-05, tj, tj1, result)
			ucheb(x, -4.044355E-06, tj, tj1, result)
			ucheb(x, -1.285765E-06, tj, tj1, result)
			ucheb(x, -5.345282E-07, tj, tj1, result)
			ucheb(x, -3.06694E-07, tj, tj1, result)
			ucheb(x, -1.962037E-07, tj, tj1, result)
			ucheb(x, -1.723644E-07, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 13, 100)
'        ************************************************************************

		Private Shared Function utbln13n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.75 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.446787, tj, tj1, result)
			ucheb(x, -4.640804, tj, tj1, result)
			ucheb(x, -0.9671552, tj, tj1, result)
			ucheb(x, -0.0936499, tj, tj1, result)
			ucheb(x, -0.01274444, tj, tj1, result)
			ucheb(x, -0.00304744, tj, tj1, result)
			ucheb(x, -0.0005161439, tj, tj1, result)
			ucheb(x, -0.0001171729, tj, tj1, result)
			ucheb(x, -2.562171E-05, tj, tj1, result)
			ucheb(x, -5.359762E-06, tj, tj1, result)
			ucheb(x, -1.275494E-06, tj, tj1, result)
			ucheb(x, -2.747635E-07, tj, tj1, result)
			ucheb(x, -5.700292E-08, tj, tj1, result)
			ucheb(x, -2.565559E-09, tj, tj1, result)
			ucheb(x, 5.005396E-09, tj, tj1, result)
			ucheb(x, 3.335794E-09, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 14, 14)
'        ************************************************************************

		Private Shared Function utbln14n14(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.75 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.510624, tj, tj1, result)
			ucheb(x, -4.798584, tj, tj1, result)
			ucheb(x, -1.087107, tj, tj1, result)
			ucheb(x, -0.1478532, tj, tj1, result)
			ucheb(x, -0.0309805, tj, tj1, result)
			ucheb(x, -0.008855986, tj, tj1, result)
			ucheb(x, -0.002409083, tj, tj1, result)
			ucheb(x, -0.0007299536, tj, tj1, result)
			ucheb(x, -0.0002176177, tj, tj1, result)
			ucheb(x, -6.479417E-05, tj, tj1, result)
			ucheb(x, -1.812761E-05, tj, tj1, result)
			ucheb(x, -5.225872E-06, tj, tj1, result)
			ucheb(x, 4.516521E-07, tj, tj1, result)
			ucheb(x, 6.730551E-06, tj, tj1, result)
			ucheb(x, 9.237563E-06, tj, tj1, result)
			ucheb(x, 1.61182E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 14, 15)
'        ************************************************************************

		Private Shared Function utbln14n15(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.75 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.498681, tj, tj1, result)
			ucheb(x, -4.774668, tj, tj1, result)
			ucheb(x, -1.070267, tj, tj1, result)
			ucheb(x, -0.1399348, tj, tj1, result)
			ucheb(x, -0.02807239, tj, tj1, result)
			ucheb(x, -0.007845763, tj, tj1, result)
			ucheb(x, -0.002071773, tj, tj1, result)
			ucheb(x, -0.0006261698, tj, tj1, result)
			ucheb(x, -0.0002011695, tj, tj1, result)
			ucheb(x, -7.305946E-05, tj, tj1, result)
			ucheb(x, -3.879295E-05, tj, tj1, result)
			ucheb(x, -2.999439E-05, tj, tj1, result)
			ucheb(x, -2.904438E-05, tj, tj1, result)
			ucheb(x, -2.944986E-05, tj, tj1, result)
			ucheb(x, -2.373908E-05, tj, tj1, result)
			ucheb(x, -2.140794E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 14, 30)
'        ************************************************************************

		Private Shared Function utbln14n30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.75 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.440378, tj, tj1, result)
			ucheb(x, -4.649587, tj, tj1, result)
			ucheb(x, -0.9807829, tj, tj1, result)
			ucheb(x, -0.09989753, tj, tj1, result)
			ucheb(x, -0.01463646, tj, tj1, result)
			ucheb(x, -0.00358658, tj, tj1, result)
			ucheb(x, -0.0006745917, tj, tj1, result)
			ucheb(x, -0.0001635398, tj, tj1, result)
			ucheb(x, -3.923172E-05, tj, tj1, result)
			ucheb(x, -9.446699E-06, tj, tj1, result)
			ucheb(x, -2.613892E-06, tj, tj1, result)
			ucheb(x, -8.214073E-07, tj, tj1, result)
			ucheb(x, -3.651683E-07, tj, tj1, result)
			ucheb(x, -2.272777E-07, tj, tj1, result)
			ucheb(x, -1.464988E-07, tj, tj1, result)
			ucheb(x, -1.109803E-07, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 14, 100)
'        ************************************************************************

		Private Shared Function utbln14n100(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 3.75 - 1, 1.0)
			tj = 1
			tj1 = x
			ucheb(x, -4.429701, tj, tj1, result)
			ucheb(x, -4.610577, tj, tj1, result)
			ucheb(x, -0.9482675, tj, tj1, result)
			ucheb(x, -0.0860555, tj, tj1, result)
			ucheb(x, -0.01062151, tj, tj1, result)
			ucheb(x, -0.002525154, tj, tj1, result)
			ucheb(x, -0.0003835983, tj, tj1, result)
			ucheb(x, -8.41144E-05, tj, tj1, result)
			ucheb(x, -1.744901E-05, tj, tj1, result)
			ucheb(x, -3.31885E-06, tj, tj1, result)
			ucheb(x, -7.6921E-07, tj, tj1, result)
			ucheb(x, -1.53627E-07, tj, tj1, result)
			ucheb(x, -3.705888E-08, tj, tj1, result)
			ucheb(x, -7.999599E-09, tj, tj1, result)
			ucheb(x, -2.908395E-09, tj, tj1, result)
			ucheb(x, 1.546923E-09, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, N1, N2)
'        ************************************************************************

		Private Shared Function usigma(s As Double, n1 As Integer, n2 As Integer) As Double
			Dim result As Double = 0
			Dim f0 As Double = 0
			Dim f1 As Double = 0
			Dim f2 As Double = 0
			Dim f3 As Double = 0
			Dim f4 As Double = 0
			Dim s0 As Double = 0
			Dim s1 As Double = 0
			Dim s2 As Double = 0
			Dim s3 As Double = 0
			Dim s4 As Double = 0

			result = 0

			'
			' N1=5, N2 = 5, 6, 7, ...
			'
			If System.Math.Min(n1, n2) = 5 Then
				If System.Math.Max(n1, n2) = 5 Then
					result = utbln5n5(s)
				End If
				If System.Math.Max(n1, n2) = 6 Then
					result = utbln5n6(s)
				End If
				If System.Math.Max(n1, n2) = 7 Then
					result = utbln5n7(s)
				End If
				If System.Math.Max(n1, n2) = 8 Then
					result = utbln5n8(s)
				End If
				If System.Math.Max(n1, n2) = 9 Then
					result = utbln5n9(s)
				End If
				If System.Math.Max(n1, n2) = 10 Then
					result = utbln5n10(s)
				End If
				If System.Math.Max(n1, n2) = 11 Then
					result = utbln5n11(s)
				End If
				If System.Math.Max(n1, n2) = 12 Then
					result = utbln5n12(s)
				End If
				If System.Math.Max(n1, n2) = 13 Then
					result = utbln5n13(s)
				End If
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln5n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln5n15(s)
				End If
				If System.Math.Max(n1, n2) = 16 Then
					result = utbln5n16(s)
				End If
				If System.Math.Max(n1, n2) = 17 Then
					result = utbln5n17(s)
				End If
				If System.Math.Max(n1, n2) = 18 Then
					result = utbln5n18(s)
				End If
				If System.Math.Max(n1, n2) = 19 Then
					result = utbln5n19(s)
				End If
				If System.Math.Max(n1, n2) = 20 Then
					result = utbln5n20(s)
				End If
				If System.Math.Max(n1, n2) = 21 Then
					result = utbln5n21(s)
				End If
				If System.Math.Max(n1, n2) = 22 Then
					result = utbln5n22(s)
				End If
				If System.Math.Max(n1, n2) = 23 Then
					result = utbln5n23(s)
				End If
				If System.Math.Max(n1, n2) = 24 Then
					result = utbln5n24(s)
				End If
				If System.Math.Max(n1, n2) = 25 Then
					result = utbln5n25(s)
				End If
				If System.Math.Max(n1, n2) = 26 Then
					result = utbln5n26(s)
				End If
				If System.Math.Max(n1, n2) = 27 Then
					result = utbln5n27(s)
				End If
				If System.Math.Max(n1, n2) = 28 Then
					result = utbln5n28(s)
				End If
				If System.Math.Max(n1, n2) = 29 Then
					result = utbln5n29(s)
				End If
				If System.Math.Max(n1, n2) > 29 Then
					f0 = utbln5n15(s)
					f1 = utbln5n30(s)
					f2 = utbln5n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1=6, N2 = 6, 7, 8, ...
			'
			If System.Math.Min(n1, n2) = 6 Then
				If System.Math.Max(n1, n2) = 6 Then
					result = utbln6n6(s)
				End If
				If System.Math.Max(n1, n2) = 7 Then
					result = utbln6n7(s)
				End If
				If System.Math.Max(n1, n2) = 8 Then
					result = utbln6n8(s)
				End If
				If System.Math.Max(n1, n2) = 9 Then
					result = utbln6n9(s)
				End If
				If System.Math.Max(n1, n2) = 10 Then
					result = utbln6n10(s)
				End If
				If System.Math.Max(n1, n2) = 11 Then
					result = utbln6n11(s)
				End If
				If System.Math.Max(n1, n2) = 12 Then
					result = utbln6n12(s)
				End If
				If System.Math.Max(n1, n2) = 13 Then
					result = utbln6n13(s)
				End If
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln6n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln6n15(s)
				End If
				If System.Math.Max(n1, n2) > 15 Then
					f0 = utbln6n15(s)
					f1 = utbln6n30(s)
					f2 = utbln6n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1=7, N2 = 7, 8, ...
			'
			If System.Math.Min(n1, n2) = 7 Then
				If System.Math.Max(n1, n2) = 7 Then
					result = utbln7n7(s)
				End If
				If System.Math.Max(n1, n2) = 8 Then
					result = utbln7n8(s)
				End If
				If System.Math.Max(n1, n2) = 9 Then
					result = utbln7n9(s)
				End If
				If System.Math.Max(n1, n2) = 10 Then
					result = utbln7n10(s)
				End If
				If System.Math.Max(n1, n2) = 11 Then
					result = utbln7n11(s)
				End If
				If System.Math.Max(n1, n2) = 12 Then
					result = utbln7n12(s)
				End If
				If System.Math.Max(n1, n2) = 13 Then
					result = utbln7n13(s)
				End If
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln7n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln7n15(s)
				End If
				If System.Math.Max(n1, n2) > 15 Then
					f0 = utbln7n15(s)
					f1 = utbln7n30(s)
					f2 = utbln7n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1=8, N2 = 8, 9, 10, ...
			'
			If System.Math.Min(n1, n2) = 8 Then
				If System.Math.Max(n1, n2) = 8 Then
					result = utbln8n8(s)
				End If
				If System.Math.Max(n1, n2) = 9 Then
					result = utbln8n9(s)
				End If
				If System.Math.Max(n1, n2) = 10 Then
					result = utbln8n10(s)
				End If
				If System.Math.Max(n1, n2) = 11 Then
					result = utbln8n11(s)
				End If
				If System.Math.Max(n1, n2) = 12 Then
					result = utbln8n12(s)
				End If
				If System.Math.Max(n1, n2) = 13 Then
					result = utbln8n13(s)
				End If
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln8n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln8n15(s)
				End If
				If System.Math.Max(n1, n2) > 15 Then
					f0 = utbln8n15(s)
					f1 = utbln8n30(s)
					f2 = utbln8n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1=9, N2 = 9, 10, ...
			'
			If System.Math.Min(n1, n2) = 9 Then
				If System.Math.Max(n1, n2) = 9 Then
					result = utbln9n9(s)
				End If
				If System.Math.Max(n1, n2) = 10 Then
					result = utbln9n10(s)
				End If
				If System.Math.Max(n1, n2) = 11 Then
					result = utbln9n11(s)
				End If
				If System.Math.Max(n1, n2) = 12 Then
					result = utbln9n12(s)
				End If
				If System.Math.Max(n1, n2) = 13 Then
					result = utbln9n13(s)
				End If
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln9n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln9n15(s)
				End If
				If System.Math.Max(n1, n2) > 15 Then
					f0 = utbln9n15(s)
					f1 = utbln9n30(s)
					f2 = utbln9n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1=10, N2 = 10, 11, ...
			'
			If System.Math.Min(n1, n2) = 10 Then
				If System.Math.Max(n1, n2) = 10 Then
					result = utbln10n10(s)
				End If
				If System.Math.Max(n1, n2) = 11 Then
					result = utbln10n11(s)
				End If
				If System.Math.Max(n1, n2) = 12 Then
					result = utbln10n12(s)
				End If
				If System.Math.Max(n1, n2) = 13 Then
					result = utbln10n13(s)
				End If
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln10n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln10n15(s)
				End If
				If System.Math.Max(n1, n2) > 15 Then
					f0 = utbln10n15(s)
					f1 = utbln10n30(s)
					f2 = utbln10n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1=11, N2 = 11, 12, ...
			'
			If System.Math.Min(n1, n2) = 11 Then
				If System.Math.Max(n1, n2) = 11 Then
					result = utbln11n11(s)
				End If
				If System.Math.Max(n1, n2) = 12 Then
					result = utbln11n12(s)
				End If
				If System.Math.Max(n1, n2) = 13 Then
					result = utbln11n13(s)
				End If
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln11n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln11n15(s)
				End If
				If System.Math.Max(n1, n2) > 15 Then
					f0 = utbln11n15(s)
					f1 = utbln11n30(s)
					f2 = utbln11n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1=12, N2 = 12, 13, ...
			'
			If System.Math.Min(n1, n2) = 12 Then
				If System.Math.Max(n1, n2) = 12 Then
					result = utbln12n12(s)
				End If
				If System.Math.Max(n1, n2) = 13 Then
					result = utbln12n13(s)
				End If
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln12n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln12n15(s)
				End If
				If System.Math.Max(n1, n2) > 15 Then
					f0 = utbln12n15(s)
					f1 = utbln12n30(s)
					f2 = utbln12n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1=13, N2 = 13, 14, ...
			'
			If System.Math.Min(n1, n2) = 13 Then
				If System.Math.Max(n1, n2) = 13 Then
					result = utbln13n13(s)
				End If
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln13n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln13n15(s)
				End If
				If System.Math.Max(n1, n2) > 15 Then
					f0 = utbln13n15(s)
					f1 = utbln13n30(s)
					f2 = utbln13n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1=14, N2 = 14, 15, ...
			'
			If System.Math.Min(n1, n2) = 14 Then
				If System.Math.Max(n1, n2) = 14 Then
					result = utbln14n14(s)
				End If
				If System.Math.Max(n1, n2) = 15 Then
					result = utbln14n15(s)
				End If
				If System.Math.Max(n1, n2) > 15 Then
					f0 = utbln14n15(s)
					f1 = utbln14n30(s)
					f2 = utbln14n100(s)
					result = uninterpolate(f0, f1, f2, System.Math.Max(n1, n2))
				End If
				Return result
			End If

			'
			' N1 >= 15, N2 >= 15
			'
			If CDbl(s) > CDbl(4) Then
				s = 4
			End If
			If CDbl(s) < CDbl(3) Then
				s0 = 0.0
				f0 = usigma000(n1, n2)
				s1 = 0.75
				f1 = usigma075(n1, n2)
				s2 = 1.5
				f2 = usigma150(n1, n2)
				s3 = 2.25
				f3 = usigma225(n1, n2)
				s4 = 3.0
				f4 = usigma300(n1, n2)
				f1 = ((s - s0) * f1 - (s - s1) * f0) / (s1 - s0)
				f2 = ((s - s0) * f2 - (s - s2) * f0) / (s2 - s0)
				f3 = ((s - s0) * f3 - (s - s3) * f0) / (s3 - s0)
				f4 = ((s - s0) * f4 - (s - s4) * f0) / (s4 - s0)
				f2 = ((s - s1) * f2 - (s - s2) * f1) / (s2 - s1)
				f3 = ((s - s1) * f3 - (s - s3) * f1) / (s3 - s1)
				f4 = ((s - s1) * f4 - (s - s4) * f1) / (s4 - s1)
				f3 = ((s - s2) * f3 - (s - s3) * f2) / (s3 - s2)
				f4 = ((s - s2) * f4 - (s - s4) * f2) / (s4 - s2)
				f4 = ((s - s3) * f4 - (s - s4) * f3) / (s4 - s3)
				result = f4
			Else
				s0 = 3.0
				f0 = usigma300(n1, n2)
				s1 = 3.333333
				f1 = usigma333(n1, n2)
				s2 = 3.666667
				f2 = usigma367(n1, n2)
				s3 = 4.0
				f3 = usigma400(n1, n2)
				f1 = ((s - s0) * f1 - (s - s1) * f0) / (s1 - s0)
				f2 = ((s - s0) * f2 - (s - s2) * f0) / (s2 - s0)
				f3 = ((s - s0) * f3 - (s - s3) * f0) / (s3 - s0)
				f2 = ((s - s1) * f2 - (s - s2) * f1) / (s2 - s1)
				f3 = ((s - s1) * f3 - (s - s3) * f1) / (s3 - s1)
				f3 = ((s - s2) * f3 - (s - s3) * f2) / (s3 - s2)
				result = f3
			End If
			Return result
		End Function


	End Class
	Public Class stest
		'************************************************************************
'        Sign test
'
'        This test checks three hypotheses about the median of  the  given  sample.
'        The following tests are performed:
'            * two-tailed test (null hypothesis - the median is equal to the  given
'              value)
'            * left-tailed test (null hypothesis - the median is  greater  than  or
'              equal to the given value)
'            * right-tailed test (null hypothesis - the  median  is  less  than  or
'              equal to the given value)
'
'        Requirements:
'            * the scale of measurement should be ordinal, interval or ratio  (i.e.
'              the test could not be applied to nominal variables).
'
'        The test is non-parametric and doesn't require distribution X to be normal
'
'        Input parameters:
'            X       -   sample. Array whose index goes from 0 to N-1.
'            N       -   size of the sample.
'            Median  -   assumed median value.
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'        While   calculating   p-values   high-precision   binomial    distribution
'        approximation is used, so significance levels have about 15 exact digits.
'
'          -- ALGLIB --
'             Copyright 08.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub onesamplesigntest(x As Double(), n As Integer, median As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
			Dim i As Integer = 0
			Dim gtcnt As Integer = 0
			Dim necnt As Integer = 0

			bothtails = 0
			lefttail = 0
			righttail = 0

			If n <= 1 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' Calculate:
			' GTCnt - count of x[i]>Median
			' NECnt - count of x[i]<>Median
			'
			gtcnt = 0
			necnt = 0
			For i = 0 To n - 1
				If CDbl(x(i)) > CDbl(median) Then
					gtcnt = gtcnt + 1
				End If
				If CDbl(x(i)) <> CDbl(median) Then
					necnt = necnt + 1
				End If
			Next
			If necnt = 0 Then

				'
				' all x[i] are equal to Median.
				' So we can conclude that Median is a true median :)
				'
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If
			bothtails = System.Math.Min(2 * binomialdistr.binomialdistribution(System.Math.Min(gtcnt, necnt - gtcnt), necnt, 0.5), 1.0)
			lefttail = binomialdistr.binomialdistribution(gtcnt, necnt, 0.5)
			righttail = binomialdistr.binomialcdistribution(gtcnt - 1, necnt, 0.5)
		End Sub


	End Class
	Public Class studentttests
		'************************************************************************
'        One-sample t-test
'
'        This test checks three hypotheses about the mean of the given sample.  The
'        following tests are performed:
'            * two-tailed test (null hypothesis - the mean is equal  to  the  given
'              value)
'            * left-tailed test (null hypothesis - the  mean  is  greater  than  or
'              equal to the given value)
'            * right-tailed test (null hypothesis - the mean is less than or  equal
'              to the given value).
'
'        The test is based on the assumption that  a  given  sample  has  a  normal
'        distribution and  an  unknown  dispersion.  If  the  distribution  sharply
'        differs from normal, the test will work incorrectly.
'
'        INPUT PARAMETERS:
'            X       -   sample. Array whose index goes from 0 to N-1.
'            N       -   size of sample, N>=0
'            Mean    -   assumed value of the mean.
'
'        OUTPUT PARAMETERS:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'        NOTE: this function correctly handles degenerate cases:
'              * when N=0, all p-values are set to 1.0
'              * when variance of X[] is exactly zero, p-values are set
'                to 1.0 or 0.0, depending on difference between sample mean and
'                value of mean being tested.
'
'
'          -- ALGLIB --
'             Copyright 08.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub studentttest1(x As Double(), n As Integer, mean As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
			Dim i As Integer = 0
			Dim xmean As Double = 0
			Dim x0 As Double = 0
			Dim v As Double = 0
			Dim samex As New Boolean()
			Dim xvariance As Double = 0
			Dim xstddev As Double = 0
			Dim v1 As Double = 0
			Dim v2 As Double = 0
			Dim stat As Double = 0
			Dim s As Double = 0

			bothtails = 0
			lefttail = 0
			righttail = 0

			If n <= 0 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' Mean
			'
			xmean = 0
			x0 = x(0)
			samex = True
			For i = 0 To n - 1
				v = x(i)
				xmean = xmean + v
				samex = samex AndAlso CDbl(v) = CDbl(x0)
			Next
			If samex Then
				xmean = x0
			Else
				xmean = xmean / n
			End If

			'
			' Variance (using corrected two-pass algorithm)
			'
			xvariance = 0
			xstddev = 0
			If n <> 1 AndAlso Not samex Then
				v1 = 0
				For i = 0 To n - 1
					v1 = v1 + Math.sqr(x(i) - xmean)
				Next
				v2 = 0
				For i = 0 To n - 1
					v2 = v2 + (x(i) - xmean)
				Next
				v2 = Math.sqr(v2) / n
				xvariance = (v1 - v2) / (n - 1)
				If CDbl(xvariance) < CDbl(0) Then
					xvariance = 0
				End If
				xstddev = System.Math.sqrt(xvariance)
			End If
			If CDbl(xstddev) = CDbl(0) Then
				If CDbl(xmean) = CDbl(mean) Then
					bothtails = 1.0
				Else
					bothtails = 0.0
				End If
				If CDbl(xmean) >= CDbl(mean) Then
					lefttail = 1.0
				Else
					lefttail = 0.0
				End If
				If CDbl(xmean) <= CDbl(mean) Then
					righttail = 1.0
				Else
					righttail = 0.0
				End If
				Return
			End If

			'
			' Statistic
			'
			stat = (xmean - mean) / (xstddev / System.Math.sqrt(n))
			s = studenttdistr.studenttdistribution(n - 1, stat)
			bothtails = 2 * System.Math.Min(s, 1 - s)
			lefttail = s
			righttail = 1 - s
		End Sub


		'************************************************************************
'        Two-sample pooled test
'
'        This test checks three hypotheses about the mean of the given samples. The
'        following tests are performed:
'            * two-tailed test (null hypothesis - the means are equal)
'            * left-tailed test (null hypothesis - the mean of the first sample  is
'              greater than or equal to the mean of the second sample)
'            * right-tailed test (null hypothesis - the mean of the first sample is
'              less than or equal to the mean of the second sample).
'
'        Test is based on the following assumptions:
'            * given samples have normal distributions
'            * dispersions are equal
'            * samples are independent.
'
'        Input parameters:
'            X       -   sample 1. Array whose index goes from 0 to N-1.
'            N       -   size of sample.
'            Y       -   sample 2. Array whose index goes from 0 to M-1.
'            M       -   size of sample.
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'        NOTE: this function correctly handles degenerate cases:
'              * when N=0 or M=0, all p-values are set to 1.0
'              * when both samples has exactly zero variance, p-values are set
'                to 1.0 or 0.0, depending on difference between means.
'
'          -- ALGLIB --
'             Copyright 18.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub studentttest2(x As Double(), n As Integer, y As Double(), m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, _
			ByRef righttail As Double)
			Dim i As Integer = 0
			Dim samex As New Boolean()
			Dim samey As New Boolean()
			Dim x0 As Double = 0
			Dim y0 As Double = 0
			Dim xmean As Double = 0
			Dim ymean As Double = 0
			Dim v As Double = 0
			Dim stat As Double = 0
			Dim s As Double = 0
			Dim p As Double = 0

			bothtails = 0
			lefttail = 0
			righttail = 0

			If n <= 0 OrElse m <= 0 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' Mean
			'
			xmean = 0
			x0 = x(0)
			samex = True
			For i = 0 To n - 1
				v = x(i)
				xmean = xmean + v
				samex = samex AndAlso CDbl(v) = CDbl(x0)
			Next
			If samex Then
				xmean = x0
			Else
				xmean = xmean / n
			End If
			ymean = 0
			y0 = y(0)
			samey = True
			For i = 0 To m - 1
				v = y(i)
				ymean = ymean + v
				samey = samey AndAlso CDbl(v) = CDbl(y0)
			Next
			If samey Then
				ymean = y0
			Else
				ymean = ymean / m
			End If

			'
			' S
			'
			s = 0
			If n + m > 2 Then
				For i = 0 To n - 1
					s = s + Math.sqr(x(i) - xmean)
				Next
				For i = 0 To m - 1
					s = s + Math.sqr(y(i) - ymean)
				Next
				s = System.Math.sqrt(s * (CDbl(1) / CDbl(n) + CDbl(1) / CDbl(m)) / (n + m - 2))
			End If
			If CDbl(s) = CDbl(0) Then
				If CDbl(xmean) = CDbl(ymean) Then
					bothtails = 1.0
				Else
					bothtails = 0.0
				End If
				If CDbl(xmean) >= CDbl(ymean) Then
					lefttail = 1.0
				Else
					lefttail = 0.0
				End If
				If CDbl(xmean) <= CDbl(ymean) Then
					righttail = 1.0
				Else
					righttail = 0.0
				End If
				Return
			End If

			'
			' Statistic
			'
			stat = (xmean - ymean) / s
			p = studenttdistr.studenttdistribution(n + m - 2, stat)
			bothtails = 2 * System.Math.Min(p, 1 - p)
			lefttail = p
			righttail = 1 - p
		End Sub


		'************************************************************************
'        Two-sample unpooled test
'
'        This test checks three hypotheses about the mean of the given samples. The
'        following tests are performed:
'            * two-tailed test (null hypothesis - the means are equal)
'            * left-tailed test (null hypothesis - the mean of the first sample  is
'              greater than or equal to the mean of the second sample)
'            * right-tailed test (null hypothesis - the mean of the first sample is
'              less than or equal to the mean of the second sample).
'
'        Test is based on the following assumptions:
'            * given samples have normal distributions
'            * samples are independent.
'        Equality of variances is NOT required.
'
'        Input parameters:
'            X - sample 1. Array whose index goes from 0 to N-1.
'            N - size of the sample.
'            Y - sample 2. Array whose index goes from 0 to M-1.
'            M - size of the sample.
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'        NOTE: this function correctly handles degenerate cases:
'              * when N=0 or M=0, all p-values are set to 1.0
'              * when both samples has zero variance, p-values are set
'                to 1.0 or 0.0, depending on difference between means.
'              * when only one sample has zero variance, test reduces to 1-sample
'                version.
'
'          -- ALGLIB --
'             Copyright 18.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub unequalvariancettest(x As Double(), n As Integer, y As Double(), m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, _
			ByRef righttail As Double)
			Dim i As Integer = 0
			Dim samex As New Boolean()
			Dim samey As New Boolean()
			Dim x0 As Double = 0
			Dim y0 As Double = 0
			Dim xmean As Double = 0
			Dim ymean As Double = 0
			Dim xvar As Double = 0
			Dim yvar As Double = 0
			Dim v As Double = 0
			Dim df As Double = 0
			Dim p As Double = 0
			Dim stat As Double = 0
			Dim c As Double = 0

			bothtails = 0
			lefttail = 0
			righttail = 0

			If n <= 0 OrElse m <= 0 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' Mean
			'
			xmean = 0
			x0 = x(0)
			samex = True
			For i = 0 To n - 1
				v = x(i)
				xmean = xmean + v
				samex = samex AndAlso CDbl(v) = CDbl(x0)
			Next
			If samex Then
				xmean = x0
			Else
				xmean = xmean / n
			End If
			ymean = 0
			y0 = y(0)
			samey = True
			For i = 0 To m - 1
				v = y(i)
				ymean = ymean + v
				samey = samey AndAlso CDbl(v) = CDbl(y0)
			Next
			If samey Then
				ymean = y0
			Else
				ymean = ymean / m
			End If

			'
			' Variance (using corrected two-pass algorithm)
			'
			xvar = 0
			If n >= 2 AndAlso Not samex Then
				For i = 0 To n - 1
					xvar = xvar + Math.sqr(x(i) - xmean)
				Next
				xvar = xvar / (n - 1)
			End If
			yvar = 0
			If m >= 2 AndAlso Not samey Then
				For i = 0 To m - 1
					yvar = yvar + Math.sqr(y(i) - ymean)
				Next
				yvar = yvar / (m - 1)
			End If

			'
			' Handle different special cases
			' (one or both variances are zero).
			'
			If CDbl(xvar) = CDbl(0) AndAlso CDbl(yvar) = CDbl(0) Then
				If CDbl(xmean) = CDbl(ymean) Then
					bothtails = 1.0
				Else
					bothtails = 0.0
				End If
				If CDbl(xmean) >= CDbl(ymean) Then
					lefttail = 1.0
				Else
					lefttail = 0.0
				End If
				If CDbl(xmean) <= CDbl(ymean) Then
					righttail = 1.0
				Else
					righttail = 0.0
				End If
				Return
			End If
			If CDbl(xvar) = CDbl(0) Then

				'
				' X is constant, unpooled 2-sample test reduces to 1-sample test.
				'
				' NOTE: right-tail and left-tail must be passed to 1-sample
				'       t-test in reverse order because we reverse order of
				'       of samples.
				'
				studentttest1(y, m, xmean, bothtails, righttail, lefttail)
				Return
			End If
			If CDbl(yvar) = CDbl(0) Then

				'
				' Y is constant, unpooled 2-sample test reduces to 1-sample test.
				'
				studentttest1(x, n, ymean, bothtails, lefttail, righttail)
				Return
			End If

			'
			' Statistic
			'
			stat = (xmean - ymean) / System.Math.sqrt(xvar / n + yvar / m)
			c = xvar / n / (xvar / n + yvar / m)
			df = (n - 1) * (m - 1) / ((m - 1) * Math.sqr(c) + (n - 1) * Math.sqr(1 - c))
			If CDbl(stat) > CDbl(0) Then
				p = 1 - 0.5 * ibetaf.incompletebeta(df / 2, 0.5, df / (df + Math.sqr(stat)))
			Else
				p = 0.5 * ibetaf.incompletebeta(df / 2, 0.5, df / (df + Math.sqr(stat)))
			End If
			bothtails = 2 * System.Math.Min(p, 1 - p)
			lefttail = p
			righttail = 1 - p
		End Sub


	End Class
	Public Class variancetests
		'************************************************************************
'        Two-sample F-test
'
'        This test checks three hypotheses about dispersions of the given  samples.
'        The following tests are performed:
'            * two-tailed test (null hypothesis - the dispersions are equal)
'            * left-tailed test (null hypothesis  -  the  dispersion  of  the first
'              sample is greater than or equal to  the  dispersion  of  the  second
'              sample).
'            * right-tailed test (null hypothesis - the  dispersion  of  the  first
'              sample is less than or equal to the dispersion of the second sample)
'
'        The test is based on the following assumptions:
'            * the given samples have normal distributions
'            * the samples are independent.
'
'        Input parameters:
'            X   -   sample 1. Array whose index goes from 0 to N-1.
'            N   -   sample size.
'            Y   -   sample 2. Array whose index goes from 0 to M-1.
'            M   -   sample size.
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'          -- ALGLIB --
'             Copyright 19.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub ftest(x As Double(), n As Integer, y As Double(), m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, _
			ByRef righttail As Double)
			Dim i As Integer = 0
			Dim xmean As Double = 0
			Dim ymean As Double = 0
			Dim xvar As Double = 0
			Dim yvar As Double = 0
			Dim df1 As Integer = 0
			Dim df2 As Integer = 0
			Dim stat As Double = 0

			bothtails = 0
			lefttail = 0
			righttail = 0

			If n <= 2 OrElse m <= 2 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' Mean
			'
			xmean = 0
			For i = 0 To n - 1
				xmean = xmean + x(i)
			Next
			xmean = xmean / n
			ymean = 0
			For i = 0 To m - 1
				ymean = ymean + y(i)
			Next
			ymean = ymean / m

			'
			' Variance (using corrected two-pass algorithm)
			'
			xvar = 0
			For i = 0 To n - 1
				xvar = xvar + Math.sqr(x(i) - xmean)
			Next
			xvar = xvar / (n - 1)
			yvar = 0
			For i = 0 To m - 1
				yvar = yvar + Math.sqr(y(i) - ymean)
			Next
			yvar = yvar / (m - 1)
			If CDbl(xvar) = CDbl(0) OrElse CDbl(yvar) = CDbl(0) Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' Statistic
			'
			df1 = n - 1
			df2 = m - 1
			stat = System.Math.Min(xvar / yvar, yvar / xvar)
			bothtails = 1 - (fdistr.fdistribution(df1, df2, 1 / stat) - fdistr.fdistribution(df1, df2, stat))
			lefttail = fdistr.fdistribution(df1, df2, xvar / yvar)
			righttail = 1 - lefttail
		End Sub


		'************************************************************************
'        One-sample chi-square test
'
'        This test checks three hypotheses about the dispersion of the given sample
'        The following tests are performed:
'            * two-tailed test (null hypothesis - the dispersion equals  the  given
'              number)
'            * left-tailed test (null hypothesis - the dispersion is  greater  than
'              or equal to the given number)
'            * right-tailed test (null hypothesis  -  dispersion is  less  than  or
'              equal to the given number).
'
'        Test is based on the following assumptions:
'            * the given sample has a normal distribution.
'
'        Input parameters:
'            X           -   sample 1. Array whose index goes from 0 to N-1.
'            N           -   size of the sample.
'            Variance    -   dispersion value to compare with.
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'          -- ALGLIB --
'             Copyright 19.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub onesamplevariancetest(x As Double(), n As Integer, variance As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
			Dim i As Integer = 0
			Dim xmean As Double = 0
			Dim xvar As Double = 0
			Dim s As Double = 0
			Dim stat As Double = 0

			bothtails = 0
			lefttail = 0
			righttail = 0

			If n <= 1 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' Mean
			'
			xmean = 0
			For i = 0 To n - 1
				xmean = xmean + x(i)
			Next
			xmean = xmean / n

			'
			' Variance
			'
			xvar = 0
			For i = 0 To n - 1
				xvar = xvar + Math.sqr(x(i) - xmean)
			Next
			xvar = xvar / (n - 1)
			If CDbl(xvar) = CDbl(0) Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If

			'
			' Statistic
			'
			stat = (n - 1) * xvar / variance
			s = chisquaredistr.chisquaredistribution(n - 1, stat)
			bothtails = 2 * System.Math.Min(s, 1 - s)
			lefttail = s
			righttail = 1 - lefttail
		End Sub


	End Class
	Public Class wsr
		'************************************************************************
'        Wilcoxon signed-rank test
'
'        This test checks three hypotheses about the median  of  the  given sample.
'        The following tests are performed:
'            * two-tailed test (null hypothesis - the median is equal to the  given
'              value)
'            * left-tailed test (null hypothesis - the median is  greater  than  or
'              equal to the given value)
'            * right-tailed test (null hypothesis  -  the  median  is  less than or
'              equal to the given value)
'
'        Requirements:
'            * the scale of measurement should be ordinal, interval or  ratio (i.e.
'              the test could not be applied to nominal variables).
'            * the distribution should be continuous and symmetric relative to  its
'              median.
'            * number of distinct values in the X array should be greater than 4
'
'        The test is non-parametric and doesn't require distribution X to be normal
'
'        Input parameters:
'            X       -   sample. Array whose index goes from 0 to N-1.
'            N       -   size of the sample.
'            Median  -   assumed median value.
'
'        Output parameters:
'            BothTails   -   p-value for two-tailed test.
'                            If BothTails is less than the given significance level
'                            the null hypothesis is rejected.
'            LeftTail    -   p-value for left-tailed test.
'                            If LeftTail is less than the given significance level,
'                            the null hypothesis is rejected.
'            RightTail   -   p-value for right-tailed test.
'                            If RightTail is less than the given significance level
'                            the null hypothesis is rejected.
'
'        To calculate p-values, special approximation is used. This method lets  us
'        calculate p-values with two decimal places in interval [0.0001, 1].
'
'        "Two decimal places" does not sound very impressive, but in  practice  the
'        relative error of less than 1% is enough to make a decision.
'
'        There is no approximation outside the [0.0001, 1] interval. Therefore,  if
'        the significance level outlies this interval, the test returns 0.0001.
'
'          -- ALGLIB --
'             Copyright 08.09.2006 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub wilcoxonsignedranktest(x As Double(), n As Integer, e As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim k As Integer = 0
			Dim t As Integer = 0
			Dim tmp As Double = 0
			Dim tmpi As Integer = 0
			Dim ns As Integer = 0
			Dim r As Double() = New Double(-1) {}
			Dim c As Integer() = New Integer(-1) {}
			Dim w As Double = 0
			Dim p As Double = 0
			Dim mp As Double = 0
			Dim s As Double = 0
			Dim sigma As Double = 0
			Dim mu As Double = 0

			x = DirectCast(x.Clone(), Double())
			bothtails = 0
			lefttail = 0
			righttail = 0


			'
			' Prepare
			'
			If n < 5 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If
			ns = 0
			For i = 0 To n - 1
				If CDbl(x(i)) = CDbl(e) Then
					Continue For
				End If
				x(ns) = x(i)
				ns = ns + 1
			Next
			If ns < 5 Then
				bothtails = 1.0
				lefttail = 1.0
				righttail = 1.0
				Return
			End If
			r = New Double(ns - 1) {}
			c = New Integer(ns - 1) {}
			For i = 0 To ns - 1
				r(i) = System.Math.Abs(x(i) - e)
				c(i) = i
			Next

			'
			' sort {R, C}
			'
			If ns <> 1 Then
				i = 2
				Do
					t = i
					While t <> 1
						k = t \ 2
						If CDbl(r(k - 1)) >= CDbl(r(t - 1)) Then
							t = 1
						Else
							tmp = r(k - 1)
							r(k - 1) = r(t - 1)
							r(t - 1) = tmp
							tmpi = c(k - 1)
							c(k - 1) = c(t - 1)
							c(t - 1) = tmpi
							t = k
						End If
					End While
					i = i + 1
				Loop While i <= ns
				i = ns - 1
				Do
					tmp = r(i)
					r(i) = r(0)
					r(0) = tmp
					tmpi = c(i)
					c(i) = c(0)
					c(0) = tmpi
					t = 1
					While t <> 0
						k = 2 * t
						If k > i Then
							t = 0
						Else
							If k < i Then
								If CDbl(r(k)) > CDbl(r(k - 1)) Then
									k = k + 1
								End If
							End If
							If CDbl(r(t - 1)) >= CDbl(r(k - 1)) Then
								t = 0
							Else
								tmp = r(k - 1)
								r(k - 1) = r(t - 1)
								r(t - 1) = tmp
								tmpi = c(k - 1)
								c(k - 1) = c(t - 1)
								c(t - 1) = tmpi
								t = k
							End If
						End If
					End While
					i = i - 1
				Loop While i >= 1
			End If

			'
			' compute tied ranks
			'
			i = 0
			While i <= ns - 1
				j = i + 1
				While j <= ns - 1
					If CDbl(r(j)) <> CDbl(r(i)) Then
						Exit While
					End If
					j = j + 1
				End While
				For k = i To j - 1
					r(k) = 1 + CDbl(i + j - 1) / CDbl(2)
				Next
				i = j
			End While

			'
			' Compute W+
			'
			w = 0
			For i = 0 To ns - 1
				If CDbl(x(c(i))) > CDbl(e) Then
					w = w + r(i)
				End If
			Next

			'
			' Result
			'
			mu = CDbl(ns * (ns + 1)) / CDbl(4)
			sigma = System.Math.sqrt(CDbl(ns * (ns + 1) * (2 * ns + 1)) / CDbl(24))
			s = (w - mu) / sigma
			If CDbl(s) <= CDbl(0) Then
				p = System.Math.Exp(wsigma(-((w - mu) / sigma), ns))
				mp = 1 - System.Math.Exp(wsigma(-((w - 1 - mu) / sigma), ns))
			Else
				mp = System.Math.Exp(wsigma((w - mu) / sigma, ns))
				p = 1 - System.Math.Exp(wsigma((w + 1 - mu) / sigma, ns))
			End If
			bothtails = System.Math.Max(2 * System.Math.Min(p, mp), 0.0001)
			lefttail = System.Math.Max(p, 0.0001)
			righttail = System.Math.Max(mp, 0.0001)
		End Sub


		'************************************************************************
'        Sequential Chebyshev interpolation.
'        ************************************************************************

		Private Shared Sub wcheb(x As Double, c As Double, ByRef tj As Double, ByRef tj1 As Double, ByRef r As Double)
			Dim t As Double = 0

			r = r + c * tj
			t = 2 * x * tj1 - tj
			tj = tj1
			tj1 = t
		End Sub


		'************************************************************************
'        Tail(S, 5)
'        ************************************************************************

		Private Shared Function w5(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(3.708099 * s) + 7.5)))
			If w >= 7 Then
				r = -0.6931
			End If
			If w = 6 Then
				r = -0.9008
			End If
			If w = 5 Then
				r = -1.163
			End If
			If w = 4 Then
				r = -1.52
			End If
			If w = 3 Then
				r = -1.856
			End If
			If w = 2 Then
				r = -2.367
			End If
			If w = 1 Then
				r = -2.773
			End If
			If w <= 0 Then
				r = -3.466
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 6)
'        ************************************************************************

		Private Shared Function w6(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(4.769696 * s) + 10.5)))
			If w >= 10 Then
				r = -0.6931
			End If
			If w = 9 Then
				r = -0.863
			End If
			If w = 8 Then
				r = -1.068
			End If
			If w = 7 Then
				r = -1.269
			End If
			If w = 6 Then
				r = -1.52
			End If
			If w = 5 Then
				r = -1.856
			End If
			If w = 4 Then
				r = -2.213
			End If
			If w = 3 Then
				r = -2.549
			End If
			If w = 2 Then
				r = -3.06
			End If
			If w = 1 Then
				r = -3.466
			End If
			If w <= 0 Then
				r = -4.159
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 7)
'        ************************************************************************

		Private Shared Function w7(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(5.91608 * s) + 14.0)))
			If w >= 14 Then
				r = -0.6325
			End If
			If w = 13 Then
				r = -0.7577
			End If
			If w = 12 Then
				r = -0.9008
			End If
			If w = 11 Then
				r = -1.068
			End If
			If w = 10 Then
				r = -1.241
			End If
			If w = 9 Then
				r = -1.451
			End If
			If w = 8 Then
				r = -1.674
			End If
			If w = 7 Then
				r = -1.908
			End If
			If w = 6 Then
				r = -2.213
			End If
			If w = 5 Then
				r = -2.549
			End If
			If w = 4 Then
				r = -2.906
			End If
			If w = 3 Then
				r = -3.243
			End If
			If w = 2 Then
				r = -3.753
			End If
			If w = 1 Then
				r = -4.159
			End If
			If w <= 0 Then
				r = -4.852
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 8)
'        ************************************************************************

		Private Shared Function w8(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(7.141428 * s) + 18.0)))
			If w >= 18 Then
				r = -0.6399
			End If
			If w = 17 Then
				r = -0.7494
			End If
			If w = 16 Then
				r = -0.863
			End If
			If w = 15 Then
				r = -0.9913
			End If
			If w = 14 Then
				r = -1.138
			End If
			If w = 13 Then
				r = -1.297
			End If
			If w = 12 Then
				r = -1.468
			End If
			If w = 11 Then
				r = -1.653
			End If
			If w = 10 Then
				r = -1.856
			End If
			If w = 9 Then
				r = -2.079
			End If
			If w = 8 Then
				r = -2.326
			End If
			If w = 7 Then
				r = -2.601
			End If
			If w = 6 Then
				r = -2.906
			End If
			If w = 5 Then
				r = -3.243
			End If
			If w = 4 Then
				r = -3.599
			End If
			If w = 3 Then
				r = -3.936
			End If
			If w = 2 Then
				r = -4.447
			End If
			If w = 1 Then
				r = -4.852
			End If
			If w <= 0 Then
				r = -5.545
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 9)
'        ************************************************************************

		Private Shared Function w9(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(8.440972 * s) + 22.5)))
			If w >= 22 Then
				r = -0.6931
			End If
			If w = 21 Then
				r = -0.7873
			End If
			If w = 20 Then
				r = -0.8912
			End If
			If w = 19 Then
				r = -1.002
			End If
			If w = 18 Then
				r = -1.12
			End If
			If w = 17 Then
				r = -1.255
			End If
			If w = 16 Then
				r = -1.394
			End If
			If w = 15 Then
				r = -1.547
			End If
			If w = 14 Then
				r = -1.717
			End If
			If w = 13 Then
				r = -1.895
			End If
			If w = 12 Then
				r = -2.079
			End If
			If w = 11 Then
				r = -2.287
			End If
			If w = 10 Then
				r = -2.501
			End If
			If w = 9 Then
				r = -2.742
			End If
			If w = 8 Then
				r = -3.019
			End If
			If w = 7 Then
				r = -3.294
			End If
			If w = 6 Then
				r = -3.599
			End If
			If w = 5 Then
				r = -3.936
			End If
			If w = 4 Then
				r = -4.292
			End If
			If w = 3 Then
				r = -4.629
			End If
			If w = 2 Then
				r = -5.14
			End If
			If w = 1 Then
				r = -5.545
			End If
			If w <= 0 Then
				r = -6.238
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 10)
'        ************************************************************************

		Private Shared Function w10(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(9.810708 * s) + 27.5)))
			If w >= 27 Then
				r = -0.6931
			End If
			If w = 26 Then
				r = -0.7745
			End If
			If w = 25 Then
				r = -0.8607
			End If
			If w = 24 Then
				r = -0.9551
			End If
			If w = 23 Then
				r = -1.057
			End If
			If w = 22 Then
				r = -1.163
			End If
			If w = 21 Then
				r = -1.279
			End If
			If w = 20 Then
				r = -1.402
			End If
			If w = 19 Then
				r = -1.533
			End If
			If w = 18 Then
				r = -1.674
			End If
			If w = 17 Then
				r = -1.826
			End If
			If w = 16 Then
				r = -1.983
			End If
			If w = 15 Then
				r = -2.152
			End If
			If w = 14 Then
				r = -2.336
			End If
			If w = 13 Then
				r = -2.525
			End If
			If w = 12 Then
				r = -2.727
			End If
			If w = 11 Then
				r = -2.942
			End If
			If w = 10 Then
				r = -3.17
			End If
			If w = 9 Then
				r = -3.435
			End If
			If w = 8 Then
				r = -3.713
			End If
			If w = 7 Then
				r = -3.987
			End If
			If w = 6 Then
				r = -4.292
			End If
			If w = 5 Then
				r = -4.629
			End If
			If w = 4 Then
				r = -4.986
			End If
			If w = 3 Then
				r = -5.322
			End If
			If w = 2 Then
				r = -5.833
			End If
			If w = 1 Then
				r = -6.238
			End If
			If w <= 0 Then
				r = -6.931
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 11)
'        ************************************************************************

		Private Shared Function w11(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(11.24722 * s) + 33.0)))
			If w >= 33 Then
				r = -0.6595
			End If
			If w = 32 Then
				r = -0.7279
			End If
			If w = 31 Then
				r = -0.8002
			End If
			If w = 30 Then
				r = -0.8782
			End If
			If w = 29 Then
				r = -0.9615
			End If
			If w = 28 Then
				r = -1.05
			End If
			If w = 27 Then
				r = -1.143
			End If
			If w = 26 Then
				r = -1.243
			End If
			If w = 25 Then
				r = -1.348
			End If
			If w = 24 Then
				r = -1.459
			End If
			If w = 23 Then
				r = -1.577
			End If
			If w = 22 Then
				r = -1.7
			End If
			If w = 21 Then
				r = -1.832
			End If
			If w = 20 Then
				r = -1.972
			End If
			If w = 19 Then
				r = -2.119
			End If
			If w = 18 Then
				r = -2.273
			End If
			If w = 17 Then
				r = -2.437
			End If
			If w = 16 Then
				r = -2.607
			End If
			If w = 15 Then
				r = -2.788
			End If
			If w = 14 Then
				r = -2.98
			End If
			If w = 13 Then
				r = -3.182
			End If
			If w = 12 Then
				r = -3.391
			End If
			If w = 11 Then
				r = -3.617
			End If
			If w = 10 Then
				r = -3.863
			End If
			If w = 9 Then
				r = -4.128
			End If
			If w = 8 Then
				r = -4.406
			End If
			If w = 7 Then
				r = -4.68
			End If
			If w = 6 Then
				r = -4.986
			End If
			If w = 5 Then
				r = -5.322
			End If
			If w = 4 Then
				r = -5.679
			End If
			If w = 3 Then
				r = -6.015
			End If
			If w = 2 Then
				r = -6.526
			End If
			If w = 1 Then
				r = -6.931
			End If
			If w <= 0 Then
				r = -7.625
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 12)
'        ************************************************************************

		Private Shared Function w12(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(12.74755 * s) + 39.0)))
			If w >= 39 Then
				r = -0.6633
			End If
			If w = 38 Then
				r = -0.7239
			End If
			If w = 37 Then
				r = -0.7878
			End If
			If w = 36 Then
				r = -0.8556
			End If
			If w = 35 Then
				r = -0.9276
			End If
			If w = 34 Then
				r = -1.003
			End If
			If w = 33 Then
				r = -1.083
			End If
			If w = 32 Then
				r = -1.168
			End If
			If w = 31 Then
				r = -1.256
			End If
			If w = 30 Then
				r = -1.35
			End If
			If w = 29 Then
				r = -1.449
			End If
			If w = 28 Then
				r = -1.552
			End If
			If w = 27 Then
				r = -1.66
			End If
			If w = 26 Then
				r = -1.774
			End If
			If w = 25 Then
				r = -1.893
			End If
			If w = 24 Then
				r = -2.017
			End If
			If w = 23 Then
				r = -2.148
			End If
			If w = 22 Then
				r = -2.285
			End If
			If w = 21 Then
				r = -2.429
			End If
			If w = 20 Then
				r = -2.581
			End If
			If w = 19 Then
				r = -2.738
			End If
			If w = 18 Then
				r = -2.902
			End If
			If w = 17 Then
				r = -3.076
			End If
			If w = 16 Then
				r = -3.255
			End If
			If w = 15 Then
				r = -3.443
			End If
			If w = 14 Then
				r = -3.645
			End If
			If w = 13 Then
				r = -3.852
			End If
			If w = 12 Then
				r = -4.069
			End If
			If w = 11 Then
				r = -4.31
			End If
			If w = 10 Then
				r = -4.557
			End If
			If w = 9 Then
				r = -4.821
			End If
			If w = 8 Then
				r = -5.099
			End If
			If w = 7 Then
				r = -5.373
			End If
			If w = 6 Then
				r = -5.679
			End If
			If w = 5 Then
				r = -6.015
			End If
			If w = 4 Then
				r = -6.372
			End If
			If w = 3 Then
				r = -6.708
			End If
			If w = 2 Then
				r = -7.219
			End If
			If w = 1 Then
				r = -7.625
			End If
			If w <= 0 Then
				r = -8.318
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 13)
'        ************************************************************************

		Private Shared Function w13(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(14.30909 * s) + 45.5)))
			If w >= 45 Then
				r = -0.6931
			End If
			If w = 44 Then
				r = -0.7486
			End If
			If w = 43 Then
				r = -0.8068
			End If
			If w = 42 Then
				r = -0.8683
			End If
			If w = 41 Then
				r = -0.9328
			End If
			If w = 40 Then
				r = -1.001
			End If
			If w = 39 Then
				r = -1.072
			End If
			If w = 38 Then
				r = -1.146
			End If
			If w = 37 Then
				r = -1.224
			End If
			If w = 36 Then
				r = -1.306
			End If
			If w = 35 Then
				r = -1.392
			End If
			If w = 34 Then
				r = -1.481
			End If
			If w = 33 Then
				r = -1.574
			End If
			If w = 32 Then
				r = -1.672
			End If
			If w = 31 Then
				r = -1.773
			End If
			If w = 30 Then
				r = -1.879
			End If
			If w = 29 Then
				r = -1.99
			End If
			If w = 28 Then
				r = -2.104
			End If
			If w = 27 Then
				r = -2.224
			End If
			If w = 26 Then
				r = -2.349
			End If
			If w = 25 Then
				r = -2.479
			End If
			If w = 24 Then
				r = -2.614
			End If
			If w = 23 Then
				r = -2.755
			End If
			If w = 22 Then
				r = -2.902
			End If
			If w = 21 Then
				r = -3.055
			End If
			If w = 20 Then
				r = -3.215
			End If
			If w = 19 Then
				r = -3.38
			End If
			If w = 18 Then
				r = -3.551
			End If
			If w = 17 Then
				r = -3.733
			End If
			If w = 16 Then
				r = -3.917
			End If
			If w = 15 Then
				r = -4.113
			End If
			If w = 14 Then
				r = -4.32
			End If
			If w = 13 Then
				r = -4.534
			End If
			If w = 12 Then
				r = -4.762
			End If
			If w = 11 Then
				r = -5.004
			End If
			If w = 10 Then
				r = -5.25
			End If
			If w = 9 Then
				r = -5.514
			End If
			If w = 8 Then
				r = -5.792
			End If
			If w = 7 Then
				r = -6.066
			End If
			If w = 6 Then
				r = -6.372
			End If
			If w = 5 Then
				r = -6.708
			End If
			If w = 4 Then
				r = -7.065
			End If
			If w = 3 Then
				r = -7.401
			End If
			If w = 2 Then
				r = -7.912
			End If
			If w = 1 Then
				r = -8.318
			End If
			If w <= 0 Then
				r = -9.011
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 14)
'        ************************************************************************

		Private Shared Function w14(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(15.92953 * s) + 52.5)))
			If w >= 52 Then
				r = -0.6931
			End If
			If w = 51 Then
				r = -0.7428
			End If
			If w = 50 Then
				r = -0.795
			End If
			If w = 49 Then
				r = -0.8495
			End If
			If w = 48 Then
				r = -0.9067
			End If
			If w = 47 Then
				r = -0.9664
			End If
			If w = 46 Then
				r = -1.029
			End If
			If w = 45 Then
				r = -1.094
			End If
			If w = 44 Then
				r = -1.162
			End If
			If w = 43 Then
				r = -1.233
			End If
			If w = 42 Then
				r = -1.306
			End If
			If w = 41 Then
				r = -1.383
			End If
			If w = 40 Then
				r = -1.463
			End If
			If w = 39 Then
				r = -1.546
			End If
			If w = 38 Then
				r = -1.632
			End If
			If w = 37 Then
				r = -1.722
			End If
			If w = 36 Then
				r = -1.815
			End If
			If w = 35 Then
				r = -1.911
			End If
			If w = 34 Then
				r = -2.011
			End If
			If w = 33 Then
				r = -2.115
			End If
			If w = 32 Then
				r = -2.223
			End If
			If w = 31 Then
				r = -2.334
			End If
			If w = 30 Then
				r = -2.45
			End If
			If w = 29 Then
				r = -2.57
			End If
			If w = 28 Then
				r = -2.694
			End If
			If w = 27 Then
				r = -2.823
			End If
			If w = 26 Then
				r = -2.956
			End If
			If w = 25 Then
				r = -3.095
			End If
			If w = 24 Then
				r = -3.238
			End If
			If w = 23 Then
				r = -3.387
			End If
			If w = 22 Then
				r = -3.541
			End If
			If w = 21 Then
				r = -3.7
			End If
			If w = 20 Then
				r = -3.866
			End If
			If w = 19 Then
				r = -4.038
			End If
			If w = 18 Then
				r = -4.215
			End If
			If w = 17 Then
				r = -4.401
			End If
			If w = 16 Then
				r = -4.592
			End If
			If w = 15 Then
				r = -4.791
			End If
			If w = 14 Then
				r = -5.004
			End If
			If w = 13 Then
				r = -5.227
			End If
			If w = 12 Then
				r = -5.456
			End If
			If w = 11 Then
				r = -5.697
			End If
			If w = 10 Then
				r = -5.943
			End If
			If w = 9 Then
				r = -6.208
			End If
			If w = 8 Then
				r = -6.485
			End If
			If w = 7 Then
				r = -6.76
			End If
			If w = 6 Then
				r = -7.065
			End If
			If w = 5 Then
				r = -7.401
			End If
			If w = 4 Then
				r = -7.758
			End If
			If w = 3 Then
				r = -8.095
			End If
			If w = 2 Then
				r = -8.605
			End If
			If w = 1 Then
				r = -9.011
			End If
			If w <= 0 Then
				r = -9.704
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 15)
'        ************************************************************************

		Private Shared Function w15(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(17.60682 * s) + 60.0)))
			If w >= 60 Then
				r = -0.6714
			End If
			If w = 59 Then
				r = -0.7154
			End If
			If w = 58 Then
				r = -0.7613
			End If
			If w = 57 Then
				r = -0.8093
			End If
			If w = 56 Then
				r = -0.8593
			End If
			If w = 55 Then
				r = -0.9114
			End If
			If w = 54 Then
				r = -0.9656
			End If
			If w = 53 Then
				r = -1.022
			End If
			If w = 52 Then
				r = -1.081
			End If
			If w = 51 Then
				r = -1.142
			End If
			If w = 50 Then
				r = -1.205
			End If
			If w = 49 Then
				r = -1.27
			End If
			If w = 48 Then
				r = -1.339
			End If
			If w = 47 Then
				r = -1.409
			End If
			If w = 46 Then
				r = -1.482
			End If
			If w = 45 Then
				r = -1.558
			End If
			If w = 44 Then
				r = -1.636
			End If
			If w = 43 Then
				r = -1.717
			End If
			If w = 42 Then
				r = -1.801
			End If
			If w = 41 Then
				r = -1.888
			End If
			If w = 40 Then
				r = -1.977
			End If
			If w = 39 Then
				r = -2.07
			End If
			If w = 38 Then
				r = -2.166
			End If
			If w = 37 Then
				r = -2.265
			End If
			If w = 36 Then
				r = -2.366
			End If
			If w = 35 Then
				r = -2.472
			End If
			If w = 34 Then
				r = -2.581
			End If
			If w = 33 Then
				r = -2.693
			End If
			If w = 32 Then
				r = -2.809
			End If
			If w = 31 Then
				r = -2.928
			End If
			If w = 30 Then
				r = -3.051
			End If
			If w = 29 Then
				r = -3.179
			End If
			If w = 28 Then
				r = -3.31
			End If
			If w = 27 Then
				r = -3.446
			End If
			If w = 26 Then
				r = -3.587
			End If
			If w = 25 Then
				r = -3.732
			End If
			If w = 24 Then
				r = -3.881
			End If
			If w = 23 Then
				r = -4.036
			End If
			If w = 22 Then
				r = -4.195
			End If
			If w = 21 Then
				r = -4.359
			End If
			If w = 20 Then
				r = -4.531
			End If
			If w = 19 Then
				r = -4.707
			End If
			If w = 18 Then
				r = -4.888
			End If
			If w = 17 Then
				r = -5.079
			End If
			If w = 16 Then
				r = -5.273
			End If
			If w = 15 Then
				r = -5.477
			End If
			If w = 14 Then
				r = -5.697
			End If
			If w = 13 Then
				r = -5.92
			End If
			If w = 12 Then
				r = -6.149
			End If
			If w = 11 Then
				r = -6.39
			End If
			If w = 10 Then
				r = -6.636
			End If
			If w = 9 Then
				r = -6.901
			End If
			If w = 8 Then
				r = -7.178
			End If
			If w = 7 Then
				r = -7.453
			End If
			If w = 6 Then
				r = -7.758
			End If
			If w = 5 Then
				r = -8.095
			End If
			If w = 4 Then
				r = -8.451
			End If
			If w = 3 Then
				r = -8.788
			End If
			If w = 2 Then
				r = -9.299
			End If
			If w = 1 Then
				r = -9.704
			End If
			If w <= 0 Then
				r = -10.4
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 16)
'        ************************************************************************

		Private Shared Function w16(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(19.33908 * s) + 68.0)))
			If w >= 68 Then
				r = -0.6733
			End If
			If w = 67 Then
				r = -0.7134
			End If
			If w = 66 Then
				r = -0.7551
			End If
			If w = 65 Then
				r = -0.7986
			End If
			If w = 64 Then
				r = -0.8437
			End If
			If w = 63 Then
				r = -0.8905
			End If
			If w = 62 Then
				r = -0.9391
			End If
			If w = 61 Then
				r = -0.9895
			End If
			If w = 60 Then
				r = -1.042
			End If
			If w = 59 Then
				r = -1.096
			End If
			If w = 58 Then
				r = -1.152
			End If
			If w = 57 Then
				r = -1.21
			End If
			If w = 56 Then
				r = -1.27
			End If
			If w = 55 Then
				r = -1.331
			End If
			If w = 54 Then
				r = -1.395
			End If
			If w = 53 Then
				r = -1.462
			End If
			If w = 52 Then
				r = -1.53
			End If
			If w = 51 Then
				r = -1.6
			End If
			If w = 50 Then
				r = -1.673
			End If
			If w = 49 Then
				r = -1.748
			End If
			If w = 48 Then
				r = -1.825
			End If
			If w = 47 Then
				r = -1.904
			End If
			If w = 46 Then
				r = -1.986
			End If
			If w = 45 Then
				r = -2.071
			End If
			If w = 44 Then
				r = -2.158
			End If
			If w = 43 Then
				r = -2.247
			End If
			If w = 42 Then
				r = -2.339
			End If
			If w = 41 Then
				r = -2.434
			End If
			If w = 40 Then
				r = -2.532
			End If
			If w = 39 Then
				r = -2.632
			End If
			If w = 38 Then
				r = -2.735
			End If
			If w = 37 Then
				r = -2.842
			End If
			If w = 36 Then
				r = -2.951
			End If
			If w = 35 Then
				r = -3.064
			End If
			If w = 34 Then
				r = -3.179
			End If
			If w = 33 Then
				r = -3.298
			End If
			If w = 32 Then
				r = -3.42
			End If
			If w = 31 Then
				r = -3.546
			End If
			If w = 30 Then
				r = -3.676
			End If
			If w = 29 Then
				r = -3.81
			End If
			If w = 28 Then
				r = -3.947
			End If
			If w = 27 Then
				r = -4.088
			End If
			If w = 26 Then
				r = -4.234
			End If
			If w = 25 Then
				r = -4.383
			End If
			If w = 24 Then
				r = -4.538
			End If
			If w = 23 Then
				r = -4.697
			End If
			If w = 22 Then
				r = -4.86
			End If
			If w = 21 Then
				r = -5.029
			End If
			If w = 20 Then
				r = -5.204
			End If
			If w = 19 Then
				r = -5.383
			End If
			If w = 18 Then
				r = -5.569
			End If
			If w = 17 Then
				r = -5.762
			End If
			If w = 16 Then
				r = -5.96
			End If
			If w = 15 Then
				r = -6.17
			End If
			If w = 14 Then
				r = -6.39
			End If
			If w = 13 Then
				r = -6.613
			End If
			If w = 12 Then
				r = -6.842
			End If
			If w = 11 Then
				r = -7.083
			End If
			If w = 10 Then
				r = -7.329
			End If
			If w = 9 Then
				r = -7.594
			End If
			If w = 8 Then
				r = -7.871
			End If
			If w = 7 Then
				r = -8.146
			End If
			If w = 6 Then
				r = -8.451
			End If
			If w = 5 Then
				r = -8.788
			End If
			If w = 4 Then
				r = -9.144
			End If
			If w = 3 Then
				r = -9.481
			End If
			If w = 2 Then
				r = -9.992
			End If
			If w = 1 Then
				r = -10.4
			End If
			If w <= 0 Then
				r = -11.09
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 17)
'        ************************************************************************

		Private Shared Function w17(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(21.12463 * s) + 76.5)))
			If w >= 76 Then
				r = -0.6931
			End If
			If w = 75 Then
				r = -0.7306
			End If
			If w = 74 Then
				r = -0.7695
			End If
			If w = 73 Then
				r = -0.8097
			End If
			If w = 72 Then
				r = -0.8514
			End If
			If w = 71 Then
				r = -0.8946
			End If
			If w = 70 Then
				r = -0.9392
			End If
			If w = 69 Then
				r = -0.9853
			End If
			If w = 68 Then
				r = -1.033
			End If
			If w = 67 Then
				r = -1.082
			End If
			If w = 66 Then
				r = -1.133
			End If
			If w = 65 Then
				r = -1.185
			End If
			If w = 64 Then
				r = -1.24
			End If
			If w = 63 Then
				r = -1.295
			End If
			If w = 62 Then
				r = -1.353
			End If
			If w = 61 Then
				r = -1.412
			End If
			If w = 60 Then
				r = -1.473
			End If
			If w = 59 Then
				r = -1.536
			End If
			If w = 58 Then
				r = -1.6
			End If
			If w = 57 Then
				r = -1.666
			End If
			If w = 56 Then
				r = -1.735
			End If
			If w = 55 Then
				r = -1.805
			End If
			If w = 54 Then
				r = -1.877
			End If
			If w = 53 Then
				r = -1.951
			End If
			If w = 52 Then
				r = -2.028
			End If
			If w = 51 Then
				r = -2.106
			End If
			If w = 50 Then
				r = -2.186
			End If
			If w = 49 Then
				r = -2.269
			End If
			If w = 48 Then
				r = -2.353
			End If
			If w = 47 Then
				r = -2.44
			End If
			If w = 46 Then
				r = -2.53
			End If
			If w = 45 Then
				r = -2.621
			End If
			If w = 44 Then
				r = -2.715
			End If
			If w = 43 Then
				r = -2.812
			End If
			If w = 42 Then
				r = -2.911
			End If
			If w = 41 Then
				r = -3.012
			End If
			If w = 40 Then
				r = -3.116
			End If
			If w = 39 Then
				r = -3.223
			End If
			If w = 38 Then
				r = -3.332
			End If
			If w = 37 Then
				r = -3.445
			End If
			If w = 36 Then
				r = -3.56
			End If
			If w = 35 Then
				r = -3.678
			End If
			If w = 34 Then
				r = -3.799
			End If
			If w = 33 Then
				r = -3.924
			End If
			If w = 32 Then
				r = -4.052
			End If
			If w = 31 Then
				r = -4.183
			End If
			If w = 30 Then
				r = -4.317
			End If
			If w = 29 Then
				r = -4.456
			End If
			If w = 28 Then
				r = -4.597
			End If
			If w = 27 Then
				r = -4.743
			End If
			If w = 26 Then
				r = -4.893
			End If
			If w = 25 Then
				r = -5.047
			End If
			If w = 24 Then
				r = -5.204
			End If
			If w = 23 Then
				r = -5.367
			End If
			If w = 22 Then
				r = -5.534
			End If
			If w = 21 Then
				r = -5.706
			End If
			If w = 20 Then
				r = -5.884
			End If
			If w = 19 Then
				r = -6.066
			End If
			If w = 18 Then
				r = -6.254
			End If
			If w = 17 Then
				r = -6.451
			End If
			If w = 16 Then
				r = -6.654
			End If
			If w = 15 Then
				r = -6.864
			End If
			If w = 14 Then
				r = -7.083
			End If
			If w = 13 Then
				r = -7.306
			End If
			If w = 12 Then
				r = -7.535
			End If
			If w = 11 Then
				r = -7.776
			End If
			If w = 10 Then
				r = -8.022
			End If
			If w = 9 Then
				r = -8.287
			End If
			If w = 8 Then
				r = -8.565
			End If
			If w = 7 Then
				r = -8.839
			End If
			If w = 6 Then
				r = -9.144
			End If
			If w = 5 Then
				r = -9.481
			End If
			If w = 4 Then
				r = -9.838
			End If
			If w = 3 Then
				r = -10.17
			End If
			If w = 2 Then
				r = -10.68
			End If
			If w = 1 Then
				r = -11.09
			End If
			If w <= 0 Then
				r = -11.78
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 18)
'        ************************************************************************

		Private Shared Function w18(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(22.96193 * s) + 85.5)))
			If w >= 85 Then
				r = -0.6931
			End If
			If w = 84 Then
				r = -0.7276
			End If
			If w = 83 Then
				r = -0.7633
			End If
			If w = 82 Then
				r = -0.8001
			End If
			If w = 81 Then
				r = -0.8381
			End If
			If w = 80 Then
				r = -0.8774
			End If
			If w = 79 Then
				r = -0.9179
			End If
			If w = 78 Then
				r = -0.9597
			End If
			If w = 77 Then
				r = -1.003
			End If
			If w = 76 Then
				r = -1.047
			End If
			If w = 75 Then
				r = -1.093
			End If
			If w = 74 Then
				r = -1.14
			End If
			If w = 73 Then
				r = -1.188
			End If
			If w = 72 Then
				r = -1.238
			End If
			If w = 71 Then
				r = -1.289
			End If
			If w = 70 Then
				r = -1.342
			End If
			If w = 69 Then
				r = -1.396
			End If
			If w = 68 Then
				r = -1.452
			End If
			If w = 67 Then
				r = -1.509
			End If
			If w = 66 Then
				r = -1.568
			End If
			If w = 65 Then
				r = -1.628
			End If
			If w = 64 Then
				r = -1.69
			End If
			If w = 63 Then
				r = -1.753
			End If
			If w = 62 Then
				r = -1.818
			End If
			If w = 61 Then
				r = -1.885
			End If
			If w = 60 Then
				r = -1.953
			End If
			If w = 59 Then
				r = -2.023
			End If
			If w = 58 Then
				r = -2.095
			End If
			If w = 57 Then
				r = -2.168
			End If
			If w = 56 Then
				r = -2.244
			End If
			If w = 55 Then
				r = -2.321
			End If
			If w = 54 Then
				r = -2.4
			End If
			If w = 53 Then
				r = -2.481
			End If
			If w = 52 Then
				r = -2.564
			End If
			If w = 51 Then
				r = -2.648
			End If
			If w = 50 Then
				r = -2.735
			End If
			If w = 49 Then
				r = -2.824
			End If
			If w = 48 Then
				r = -2.915
			End If
			If w = 47 Then
				r = -3.008
			End If
			If w = 46 Then
				r = -3.104
			End If
			If w = 45 Then
				r = -3.201
			End If
			If w = 44 Then
				r = -3.301
			End If
			If w = 43 Then
				r = -3.403
			End If
			If w = 42 Then
				r = -3.508
			End If
			If w = 41 Then
				r = -3.615
			End If
			If w = 40 Then
				r = -3.724
			End If
			If w = 39 Then
				r = -3.836
			End If
			If w = 38 Then
				r = -3.95
			End If
			If w = 37 Then
				r = -4.068
			End If
			If w = 36 Then
				r = -4.188
			End If
			If w = 35 Then
				r = -4.311
			End If
			If w = 34 Then
				r = -4.437
			End If
			If w = 33 Then
				r = -4.565
			End If
			If w = 32 Then
				r = -4.698
			End If
			If w = 31 Then
				r = -4.833
			End If
			If w = 30 Then
				r = -4.971
			End If
			If w = 29 Then
				r = -5.113
			End If
			If w = 28 Then
				r = -5.258
			End If
			If w = 27 Then
				r = -5.408
			End If
			If w = 26 Then
				r = -5.561
			End If
			If w = 25 Then
				r = -5.717
			End If
			If w = 24 Then
				r = -5.878
			End If
			If w = 23 Then
				r = -6.044
			End If
			If w = 22 Then
				r = -6.213
			End If
			If w = 21 Then
				r = -6.388
			End If
			If w = 20 Then
				r = -6.569
			End If
			If w = 19 Then
				r = -6.753
			End If
			If w = 18 Then
				r = -6.943
			End If
			If w = 17 Then
				r = -7.144
			End If
			If w = 16 Then
				r = -7.347
			End If
			If w = 15 Then
				r = -7.557
			End If
			If w = 14 Then
				r = -7.776
			End If
			If w = 13 Then
				r = -7.999
			End If
			If w = 12 Then
				r = -8.228
			End If
			If w = 11 Then
				r = -8.469
			End If
			If w = 10 Then
				r = -8.715
			End If
			If w = 9 Then
				r = -8.98
			End If
			If w = 8 Then
				r = -9.258
			End If
			If w = 7 Then
				r = -9.532
			End If
			If w = 6 Then
				r = -9.838
			End If
			If w = 5 Then
				r = -10.17
			End If
			If w = 4 Then
				r = -10.53
			End If
			If w = 3 Then
				r = -10.87
			End If
			If w = 2 Then
				r = -11.38
			End If
			If w = 1 Then
				r = -11.78
			End If
			If w <= 0 Then
				r = -12.48
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 19)
'        ************************************************************************

		Private Shared Function w19(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(24.84955 * s) + 95.0)))
			If w >= 95 Then
				r = -0.6776
			End If
			If w = 94 Then
				r = -0.7089
			End If
			If w = 93 Then
				r = -0.7413
			End If
			If w = 92 Then
				r = -0.7747
			End If
			If w = 91 Then
				r = -0.809
			End If
			If w = 90 Then
				r = -0.8445
			End If
			If w = 89 Then
				r = -0.8809
			End If
			If w = 88 Then
				r = -0.9185
			End If
			If w = 87 Then
				r = -0.9571
			End If
			If w = 86 Then
				r = -0.9968
			End If
			If w = 85 Then
				r = -1.038
			End If
			If w = 84 Then
				r = -1.08
			End If
			If w = 83 Then
				r = -1.123
			End If
			If w = 82 Then
				r = -1.167
			End If
			If w = 81 Then
				r = -1.213
			End If
			If w = 80 Then
				r = -1.259
			End If
			If w = 79 Then
				r = -1.307
			End If
			If w = 78 Then
				r = -1.356
			End If
			If w = 77 Then
				r = -1.407
			End If
			If w = 76 Then
				r = -1.458
			End If
			If w = 75 Then
				r = -1.511
			End If
			If w = 74 Then
				r = -1.565
			End If
			If w = 73 Then
				r = -1.621
			End If
			If w = 72 Then
				r = -1.678
			End If
			If w = 71 Then
				r = -1.736
			End If
			If w = 70 Then
				r = -1.796
			End If
			If w = 69 Then
				r = -1.857
			End If
			If w = 68 Then
				r = -1.919
			End If
			If w = 67 Then
				r = -1.983
			End If
			If w = 66 Then
				r = -2.048
			End If
			If w = 65 Then
				r = -2.115
			End If
			If w = 64 Then
				r = -2.183
			End If
			If w = 63 Then
				r = -2.253
			End If
			If w = 62 Then
				r = -2.325
			End If
			If w = 61 Then
				r = -2.398
			End If
			If w = 60 Then
				r = -2.472
			End If
			If w = 59 Then
				r = -2.548
			End If
			If w = 58 Then
				r = -2.626
			End If
			If w = 57 Then
				r = -2.706
			End If
			If w = 56 Then
				r = -2.787
			End If
			If w = 55 Then
				r = -2.87
			End If
			If w = 54 Then
				r = -2.955
			End If
			If w = 53 Then
				r = -3.042
			End If
			If w = 52 Then
				r = -3.13
			End If
			If w = 51 Then
				r = -3.22
			End If
			If w = 50 Then
				r = -3.313
			End If
			If w = 49 Then
				r = -3.407
			End If
			If w = 48 Then
				r = -3.503
			End If
			If w = 47 Then
				r = -3.601
			End If
			If w = 46 Then
				r = -3.702
			End If
			If w = 45 Then
				r = -3.804
			End If
			If w = 44 Then
				r = -3.909
			End If
			If w = 43 Then
				r = -4.015
			End If
			If w = 42 Then
				r = -4.125
			End If
			If w = 41 Then
				r = -4.236
			End If
			If w = 40 Then
				r = -4.35
			End If
			If w = 39 Then
				r = -4.466
			End If
			If w = 38 Then
				r = -4.585
			End If
			If w = 37 Then
				r = -4.706
			End If
			If w = 36 Then
				r = -4.83
			End If
			If w = 35 Then
				r = -4.957
			End If
			If w = 34 Then
				r = -5.086
			End If
			If w = 33 Then
				r = -5.219
			End If
			If w = 32 Then
				r = -5.355
			End If
			If w = 31 Then
				r = -5.493
			End If
			If w = 30 Then
				r = -5.634
			End If
			If w = 29 Then
				r = -5.78
			End If
			If w = 28 Then
				r = -5.928
			End If
			If w = 27 Then
				r = -6.08
			End If
			If w = 26 Then
				r = -6.235
			End If
			If w = 25 Then
				r = -6.394
			End If
			If w = 24 Then
				r = -6.558
			End If
			If w = 23 Then
				r = -6.726
			End If
			If w = 22 Then
				r = -6.897
			End If
			If w = 21 Then
				r = -7.074
			End If
			If w = 20 Then
				r = -7.256
			End If
			If w = 19 Then
				r = -7.443
			End If
			If w = 18 Then
				r = -7.636
			End If
			If w = 17 Then
				r = -7.837
			End If
			If w = 16 Then
				r = -8.04
			End If
			If w = 15 Then
				r = -8.25
			End If
			If w = 14 Then
				r = -8.469
			End If
			If w = 13 Then
				r = -8.692
			End If
			If w = 12 Then
				r = -8.921
			End If
			If w = 11 Then
				r = -9.162
			End If
			If w = 10 Then
				r = -9.409
			End If
			If w = 9 Then
				r = -9.673
			End If
			If w = 8 Then
				r = -9.951
			End If
			If w = 7 Then
				r = -10.23
			End If
			If w = 6 Then
				r = -10.53
			End If
			If w = 5 Then
				r = -10.87
			End If
			If w = 4 Then
				r = -11.22
			End If
			If w = 3 Then
				r = -11.56
			End If
			If w = 2 Then
				r = -12.07
			End If
			If w = 1 Then
				r = -12.48
			End If
			If w <= 0 Then
				r = -13.17
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 20)
'        ************************************************************************

		Private Shared Function w20(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(26.78619 * s) + 105.0)))
			If w >= 105 Then
				r = -0.6787
			End If
			If w = 104 Then
				r = -0.7078
			End If
			If w = 103 Then
				r = -0.7378
			End If
			If w = 102 Then
				r = -0.7686
			End If
			If w = 101 Then
				r = -0.8004
			End If
			If w = 100 Then
				r = -0.833
			End If
			If w = 99 Then
				r = -0.8665
			End If
			If w = 98 Then
				r = -0.901
			End If
			If w = 97 Then
				r = -0.9363
			End If
			If w = 96 Then
				r = -0.9726
			End If
			If w = 95 Then
				r = -1.01
			End If
			If w = 94 Then
				r = -1.048
			End If
			If w = 93 Then
				r = -1.087
			End If
			If w = 92 Then
				r = -1.128
			End If
			If w = 91 Then
				r = -1.169
			End If
			If w = 90 Then
				r = -1.211
			End If
			If w = 89 Then
				r = -1.254
			End If
			If w = 88 Then
				r = -1.299
			End If
			If w = 87 Then
				r = -1.344
			End If
			If w = 86 Then
				r = -1.39
			End If
			If w = 85 Then
				r = -1.438
			End If
			If w = 84 Then
				r = -1.486
			End If
			If w = 83 Then
				r = -1.536
			End If
			If w = 82 Then
				r = -1.587
			End If
			If w = 81 Then
				r = -1.639
			End If
			If w = 80 Then
				r = -1.692
			End If
			If w = 79 Then
				r = -1.746
			End If
			If w = 78 Then
				r = -1.802
			End If
			If w = 77 Then
				r = -1.859
			End If
			If w = 76 Then
				r = -1.916
			End If
			If w = 75 Then
				r = -1.976
			End If
			If w = 74 Then
				r = -2.036
			End If
			If w = 73 Then
				r = -2.098
			End If
			If w = 72 Then
				r = -2.161
			End If
			If w = 71 Then
				r = -2.225
			End If
			If w = 70 Then
				r = -2.29
			End If
			If w = 69 Then
				r = -2.357
			End If
			If w = 68 Then
				r = -2.426
			End If
			If w = 67 Then
				r = -2.495
			End If
			If w = 66 Then
				r = -2.566
			End If
			If w = 65 Then
				r = -2.639
			End If
			If w = 64 Then
				r = -2.713
			End If
			If w = 63 Then
				r = -2.788
			End If
			If w = 62 Then
				r = -2.865
			End If
			If w = 61 Then
				r = -2.943
			End If
			If w = 60 Then
				r = -3.023
			End If
			If w = 59 Then
				r = -3.104
			End If
			If w = 58 Then
				r = -3.187
			End If
			If w = 57 Then
				r = -3.272
			End If
			If w = 56 Then
				r = -3.358
			End If
			If w = 55 Then
				r = -3.446
			End If
			If w = 54 Then
				r = -3.536
			End If
			If w = 53 Then
				r = -3.627
			End If
			If w = 52 Then
				r = -3.721
			End If
			If w = 51 Then
				r = -3.815
			End If
			If w = 50 Then
				r = -3.912
			End If
			If w = 49 Then
				r = -4.011
			End If
			If w = 48 Then
				r = -4.111
			End If
			If w = 47 Then
				r = -4.214
			End If
			If w = 46 Then
				r = -4.318
			End If
			If w = 45 Then
				r = -4.425
			End If
			If w = 44 Then
				r = -4.534
			End If
			If w = 43 Then
				r = -4.644
			End If
			If w = 42 Then
				r = -4.757
			End If
			If w = 41 Then
				r = -4.872
			End If
			If w = 40 Then
				r = -4.99
			End If
			If w = 39 Then
				r = -5.109
			End If
			If w = 38 Then
				r = -5.232
			End If
			If w = 37 Then
				r = -5.356
			End If
			If w = 36 Then
				r = -5.484
			End If
			If w = 35 Then
				r = -5.614
			End If
			If w = 34 Then
				r = -5.746
			End If
			If w = 33 Then
				r = -5.882
			End If
			If w = 32 Then
				r = -6.02
			End If
			If w = 31 Then
				r = -6.161
			End If
			If w = 30 Then
				r = -6.305
			End If
			If w = 29 Then
				r = -6.453
			End If
			If w = 28 Then
				r = -6.603
			End If
			If w = 27 Then
				r = -6.757
			End If
			If w = 26 Then
				r = -6.915
			End If
			If w = 25 Then
				r = -7.076
			End If
			If w = 24 Then
				r = -7.242
			End If
			If w = 23 Then
				r = -7.411
			End If
			If w = 22 Then
				r = -7.584
			End If
			If w = 21 Then
				r = -7.763
			End If
			If w = 20 Then
				r = -7.947
			End If
			If w = 19 Then
				r = -8.136
			End If
			If w = 18 Then
				r = -8.33
			End If
			If w = 17 Then
				r = -8.53
			End If
			If w = 16 Then
				r = -8.733
			End If
			If w = 15 Then
				r = -8.943
			End If
			If w = 14 Then
				r = -9.162
			End If
			If w = 13 Then
				r = -9.386
			End If
			If w = 12 Then
				r = -9.614
			End If
			If w = 11 Then
				r = -9.856
			End If
			If w = 10 Then
				r = -10.1
			End If
			If w = 9 Then
				r = -10.37
			End If
			If w = 8 Then
				r = -10.64
			End If
			If w = 7 Then
				r = -10.92
			End If
			If w = 6 Then
				r = -11.22
			End If
			If w = 5 Then
				r = -11.56
			End If
			If w = 4 Then
				r = -11.92
			End If
			If w = 3 Then
				r = -12.25
			End If
			If w = 2 Then
				r = -12.76
			End If
			If w = 1 Then
				r = -13.17
			End If
			If w <= 0 Then
				r = -13.86
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 21)
'        ************************************************************************

		Private Shared Function w21(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(28.77064 * s) + 115.5)))
			If w >= 115 Then
				r = -0.6931
			End If
			If w = 114 Then
				r = -0.7207
			End If
			If w = 113 Then
				r = -0.7489
			End If
			If w = 112 Then
				r = -0.7779
			End If
			If w = 111 Then
				r = -0.8077
			End If
			If w = 110 Then
				r = -0.8383
			End If
			If w = 109 Then
				r = -0.8697
			End If
			If w = 108 Then
				r = -0.9018
			End If
			If w = 107 Then
				r = -0.9348
			End If
			If w = 106 Then
				r = -0.9685
			End If
			If w = 105 Then
				r = -1.003
			End If
			If w = 104 Then
				r = -1.039
			End If
			If w = 103 Then
				r = -1.075
			End If
			If w = 102 Then
				r = -1.112
			End If
			If w = 101 Then
				r = -1.15
			End If
			If w = 100 Then
				r = -1.189
			End If
			If w = 99 Then
				r = -1.229
			End If
			If w = 98 Then
				r = -1.269
			End If
			If w = 97 Then
				r = -1.311
			End If
			If w = 96 Then
				r = -1.353
			End If
			If w = 95 Then
				r = -1.397
			End If
			If w = 94 Then
				r = -1.441
			End If
			If w = 93 Then
				r = -1.486
			End If
			If w = 92 Then
				r = -1.533
			End If
			If w = 91 Then
				r = -1.58
			End If
			If w = 90 Then
				r = -1.628
			End If
			If w = 89 Then
				r = -1.677
			End If
			If w = 88 Then
				r = -1.728
			End If
			If w = 87 Then
				r = -1.779
			End If
			If w = 86 Then
				r = -1.831
			End If
			If w = 85 Then
				r = -1.884
			End If
			If w = 84 Then
				r = -1.939
			End If
			If w = 83 Then
				r = -1.994
			End If
			If w = 82 Then
				r = -2.051
			End If
			If w = 81 Then
				r = -2.108
			End If
			If w = 80 Then
				r = -2.167
			End If
			If w = 79 Then
				r = -2.227
			End If
			If w = 78 Then
				r = -2.288
			End If
			If w = 77 Then
				r = -2.35
			End If
			If w = 76 Then
				r = -2.414
			End If
			If w = 75 Then
				r = -2.478
			End If
			If w = 74 Then
				r = -2.544
			End If
			If w = 73 Then
				r = -2.611
			End If
			If w = 72 Then
				r = -2.679
			End If
			If w = 71 Then
				r = -2.748
			End If
			If w = 70 Then
				r = -2.819
			End If
			If w = 69 Then
				r = -2.891
			End If
			If w = 68 Then
				r = -2.964
			End If
			If w = 67 Then
				r = -3.039
			End If
			If w = 66 Then
				r = -3.115
			End If
			If w = 65 Then
				r = -3.192
			End If
			If w = 64 Then
				r = -3.27
			End If
			If w = 63 Then
				r = -3.35
			End If
			If w = 62 Then
				r = -3.432
			End If
			If w = 61 Then
				r = -3.515
			End If
			If w = 60 Then
				r = -3.599
			End If
			If w = 59 Then
				r = -3.685
			End If
			If w = 58 Then
				r = -3.772
			End If
			If w = 57 Then
				r = -3.861
			End If
			If w = 56 Then
				r = -3.952
			End If
			If w = 55 Then
				r = -4.044
			End If
			If w = 54 Then
				r = -4.138
			End If
			If w = 53 Then
				r = -4.233
			End If
			If w = 52 Then
				r = -4.33
			End If
			If w = 51 Then
				r = -4.429
			End If
			If w = 50 Then
				r = -4.53
			End If
			If w = 49 Then
				r = -4.632
			End If
			If w = 48 Then
				r = -4.736
			End If
			If w = 47 Then
				r = -4.842
			End If
			If w = 46 Then
				r = -4.95
			End If
			If w = 45 Then
				r = -5.06
			End If
			If w = 44 Then
				r = -5.172
			End If
			If w = 43 Then
				r = -5.286
			End If
			If w = 42 Then
				r = -5.402
			End If
			If w = 41 Then
				r = -5.52
			End If
			If w = 40 Then
				r = -5.641
			End If
			If w = 39 Then
				r = -5.763
			End If
			If w = 38 Then
				r = -5.889
			End If
			If w = 37 Then
				r = -6.016
			End If
			If w = 36 Then
				r = -6.146
			End If
			If w = 35 Then
				r = -6.278
			End If
			If w = 34 Then
				r = -6.413
			End If
			If w = 33 Then
				r = -6.551
			End If
			If w = 32 Then
				r = -6.692
			End If
			If w = 31 Then
				r = -6.835
			End If
			If w = 30 Then
				r = -6.981
			End If
			If w = 29 Then
				r = -7.131
			End If
			If w = 28 Then
				r = -7.283
			End If
			If w = 27 Then
				r = -7.439
			End If
			If w = 26 Then
				r = -7.599
			End If
			If w = 25 Then
				r = -7.762
			End If
			If w = 24 Then
				r = -7.928
			End If
			If w = 23 Then
				r = -8.099
			End If
			If w = 22 Then
				r = -8.274
			End If
			If w = 21 Then
				r = -8.454
			End If
			If w = 20 Then
				r = -8.64
			End If
			If w = 19 Then
				r = -8.829
			End If
			If w = 18 Then
				r = -9.023
			End If
			If w = 17 Then
				r = -9.223
			End If
			If w = 16 Then
				r = -9.426
			End If
			If w = 15 Then
				r = -9.636
			End If
			If w = 14 Then
				r = -9.856
			End If
			If w = 13 Then
				r = -10.08
			End If
			If w = 12 Then
				r = -10.31
			End If
			If w = 11 Then
				r = -10.55
			End If
			If w = 10 Then
				r = -10.79
			End If
			If w = 9 Then
				r = -11.06
			End If
			If w = 8 Then
				r = -11.34
			End If
			If w = 7 Then
				r = -11.61
			End If
			If w = 6 Then
				r = -11.92
			End If
			If w = 5 Then
				r = -12.25
			End If
			If w = 4 Then
				r = -12.61
			End If
			If w = 3 Then
				r = -12.95
			End If
			If w = 2 Then
				r = -13.46
			End If
			If w = 1 Then
				r = -13.86
			End If
			If w <= 0 Then
				r = -14.56
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 22)
'        ************************************************************************

		Private Shared Function w22(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(30.80179 * s) + 126.5)))
			If w >= 126 Then
				r = -0.6931
			End If
			If w = 125 Then
				r = -0.7189
			End If
			If w = 124 Then
				r = -0.7452
			End If
			If w = 123 Then
				r = -0.7722
			End If
			If w = 122 Then
				r = -0.7999
			End If
			If w = 121 Then
				r = -0.8283
			End If
			If w = 120 Then
				r = -0.8573
			End If
			If w = 119 Then
				r = -0.8871
			End If
			If w = 118 Then
				r = -0.9175
			End If
			If w = 117 Then
				r = -0.9486
			End If
			If w = 116 Then
				r = -0.9805
			End If
			If w = 115 Then
				r = -1.013
			End If
			If w = 114 Then
				r = -1.046
			End If
			If w = 113 Then
				r = -1.08
			End If
			If w = 112 Then
				r = -1.115
			End If
			If w = 111 Then
				r = -1.151
			End If
			If w = 110 Then
				r = -1.187
			End If
			If w = 109 Then
				r = -1.224
			End If
			If w = 108 Then
				r = -1.262
			End If
			If w = 107 Then
				r = -1.301
			End If
			If w = 106 Then
				r = -1.34
			End If
			If w = 105 Then
				r = -1.381
			End If
			If w = 104 Then
				r = -1.422
			End If
			If w = 103 Then
				r = -1.464
			End If
			If w = 102 Then
				r = -1.506
			End If
			If w = 101 Then
				r = -1.55
			End If
			If w = 100 Then
				r = -1.594
			End If
			If w = 99 Then
				r = -1.64
			End If
			If w = 98 Then
				r = -1.686
			End If
			If w = 97 Then
				r = -1.733
			End If
			If w = 96 Then
				r = -1.781
			End If
			If w = 95 Then
				r = -1.83
			End If
			If w = 94 Then
				r = -1.88
			End If
			If w = 93 Then
				r = -1.93
			End If
			If w = 92 Then
				r = -1.982
			End If
			If w = 91 Then
				r = -2.034
			End If
			If w = 90 Then
				r = -2.088
			End If
			If w = 89 Then
				r = -2.142
			End If
			If w = 88 Then
				r = -2.198
			End If
			If w = 87 Then
				r = -2.254
			End If
			If w = 86 Then
				r = -2.312
			End If
			If w = 85 Then
				r = -2.37
			End If
			If w = 84 Then
				r = -2.429
			End If
			If w = 83 Then
				r = -2.49
			End If
			If w = 82 Then
				r = -2.551
			End If
			If w = 81 Then
				r = -2.614
			End If
			If w = 80 Then
				r = -2.677
			End If
			If w = 79 Then
				r = -2.742
			End If
			If w = 78 Then
				r = -2.808
			End If
			If w = 77 Then
				r = -2.875
			End If
			If w = 76 Then
				r = -2.943
			End If
			If w = 75 Then
				r = -3.012
			End If
			If w = 74 Then
				r = -3.082
			End If
			If w = 73 Then
				r = -3.153
			End If
			If w = 72 Then
				r = -3.226
			End If
			If w = 71 Then
				r = -3.3
			End If
			If w = 70 Then
				r = -3.375
			End If
			If w = 69 Then
				r = -3.451
			End If
			If w = 68 Then
				r = -3.529
			End If
			If w = 67 Then
				r = -3.607
			End If
			If w = 66 Then
				r = -3.687
			End If
			If w = 65 Then
				r = -3.769
			End If
			If w = 64 Then
				r = -3.851
			End If
			If w = 63 Then
				r = -3.935
			End If
			If w = 62 Then
				r = -4.021
			End If
			If w = 61 Then
				r = -4.108
			End If
			If w = 60 Then
				r = -4.196
			End If
			If w = 59 Then
				r = -4.285
			End If
			If w = 58 Then
				r = -4.376
			End If
			If w = 57 Then
				r = -4.469
			End If
			If w = 56 Then
				r = -4.563
			End If
			If w = 55 Then
				r = -4.659
			End If
			If w = 54 Then
				r = -4.756
			End If
			If w = 53 Then
				r = -4.855
			End If
			If w = 52 Then
				r = -4.955
			End If
			If w = 51 Then
				r = -5.057
			End If
			If w = 50 Then
				r = -5.161
			End If
			If w = 49 Then
				r = -5.266
			End If
			If w = 48 Then
				r = -5.374
			End If
			If w = 47 Then
				r = -5.483
			End If
			If w = 46 Then
				r = -5.594
			End If
			If w = 45 Then
				r = -5.706
			End If
			If w = 44 Then
				r = -5.821
			End If
			If w = 43 Then
				r = -5.938
			End If
			If w = 42 Then
				r = -6.057
			End If
			If w = 41 Then
				r = -6.177
			End If
			If w = 40 Then
				r = -6.3
			End If
			If w = 39 Then
				r = -6.426
			End If
			If w = 38 Then
				r = -6.553
			End If
			If w = 37 Then
				r = -6.683
			End If
			If w = 36 Then
				r = -6.815
			End If
			If w = 35 Then
				r = -6.949
			End If
			If w = 34 Then
				r = -7.086
			End If
			If w = 33 Then
				r = -7.226
			End If
			If w = 32 Then
				r = -7.368
			End If
			If w = 31 Then
				r = -7.513
			End If
			If w = 30 Then
				r = -7.661
			End If
			If w = 29 Then
				r = -7.813
			End If
			If w = 28 Then
				r = -7.966
			End If
			If w = 27 Then
				r = -8.124
			End If
			If w = 26 Then
				r = -8.285
			End If
			If w = 25 Then
				r = -8.449
			End If
			If w = 24 Then
				r = -8.617
			End If
			If w = 23 Then
				r = -8.789
			End If
			If w = 22 Then
				r = -8.965
			End If
			If w = 21 Then
				r = -9.147
			End If
			If w = 20 Then
				r = -9.333
			End If
			If w = 19 Then
				r = -9.522
			End If
			If w = 18 Then
				r = -9.716
			End If
			If w = 17 Then
				r = -9.917
			End If
			If w = 16 Then
				r = -10.12
			End If
			If w = 15 Then
				r = -10.33
			End If
			If w = 14 Then
				r = -10.55
			End If
			If w = 13 Then
				r = -10.77
			End If
			If w = 12 Then
				r = -11.0
			End If
			If w = 11 Then
				r = -11.24
			End If
			If w = 10 Then
				r = -11.49
			End If
			If w = 9 Then
				r = -11.75
			End If
			If w = 8 Then
				r = -12.03
			End If
			If w = 7 Then
				r = -12.3
			End If
			If w = 6 Then
				r = -12.61
			End If
			If w = 5 Then
				r = -12.95
			End If
			If w = 4 Then
				r = -13.3
			End If
			If w = 3 Then
				r = -13.64
			End If
			If w = 2 Then
				r = -14.15
			End If
			If w = 1 Then
				r = -14.56
			End If
			If w <= 0 Then
				r = -15.25
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 23)
'        ************************************************************************

		Private Shared Function w23(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(32.87856 * s) + 138.0)))
			If w >= 138 Then
				r = -0.6813
			End If
			If w = 137 Then
				r = -0.7051
			End If
			If w = 136 Then
				r = -0.7295
			End If
			If w = 135 Then
				r = -0.7544
			End If
			If w = 134 Then
				r = -0.78
			End If
			If w = 133 Then
				r = -0.8061
			End If
			If w = 132 Then
				r = -0.8328
			End If
			If w = 131 Then
				r = -0.8601
			End If
			If w = 130 Then
				r = -0.888
			End If
			If w = 129 Then
				r = -0.9166
			End If
			If w = 128 Then
				r = -0.9457
			End If
			If w = 127 Then
				r = -0.9755
			End If
			If w = 126 Then
				r = -1.006
			End If
			If w = 125 Then
				r = -1.037
			End If
			If w = 124 Then
				r = -1.069
			End If
			If w = 123 Then
				r = -1.101
			End If
			If w = 122 Then
				r = -1.134
			End If
			If w = 121 Then
				r = -1.168
			End If
			If w = 120 Then
				r = -1.202
			End If
			If w = 119 Then
				r = -1.237
			End If
			If w = 118 Then
				r = -1.273
			End If
			If w = 117 Then
				r = -1.309
			End If
			If w = 116 Then
				r = -1.347
			End If
			If w = 115 Then
				r = -1.384
			End If
			If w = 114 Then
				r = -1.423
			End If
			If w = 113 Then
				r = -1.462
			End If
			If w = 112 Then
				r = -1.502
			End If
			If w = 111 Then
				r = -1.543
			End If
			If w = 110 Then
				r = -1.585
			End If
			If w = 109 Then
				r = -1.627
			End If
			If w = 108 Then
				r = -1.67
			End If
			If w = 107 Then
				r = -1.714
			End If
			If w = 106 Then
				r = -1.758
			End If
			If w = 105 Then
				r = -1.804
			End If
			If w = 104 Then
				r = -1.85
			End If
			If w = 103 Then
				r = -1.897
			End If
			If w = 102 Then
				r = -1.944
			End If
			If w = 101 Then
				r = -1.993
			End If
			If w = 100 Then
				r = -2.042
			End If
			If w = 99 Then
				r = -2.093
			End If
			If w = 98 Then
				r = -2.144
			End If
			If w = 97 Then
				r = -2.195
			End If
			If w = 96 Then
				r = -2.248
			End If
			If w = 95 Then
				r = -2.302
			End If
			If w = 94 Then
				r = -2.356
			End If
			If w = 93 Then
				r = -2.412
			End If
			If w = 92 Then
				r = -2.468
			End If
			If w = 91 Then
				r = -2.525
			End If
			If w = 90 Then
				r = -2.583
			End If
			If w = 89 Then
				r = -2.642
			End If
			If w = 88 Then
				r = -2.702
			End If
			If w = 87 Then
				r = -2.763
			End If
			If w = 86 Then
				r = -2.825
			End If
			If w = 85 Then
				r = -2.888
			End If
			If w = 84 Then
				r = -2.951
			End If
			If w = 83 Then
				r = -3.016
			End If
			If w = 82 Then
				r = -3.082
			End If
			If w = 81 Then
				r = -3.149
			End If
			If w = 80 Then
				r = -3.216
			End If
			If w = 79 Then
				r = -3.285
			End If
			If w = 78 Then
				r = -3.355
			End If
			If w = 77 Then
				r = -3.426
			End If
			If w = 76 Then
				r = -3.498
			End If
			If w = 75 Then
				r = -3.571
			End If
			If w = 74 Then
				r = -3.645
			End If
			If w = 73 Then
				r = -3.721
			End If
			If w = 72 Then
				r = -3.797
			End If
			If w = 71 Then
				r = -3.875
			End If
			If w = 70 Then
				r = -3.953
			End If
			If w = 69 Then
				r = -4.033
			End If
			If w = 68 Then
				r = -4.114
			End If
			If w = 67 Then
				r = -4.197
			End If
			If w = 66 Then
				r = -4.28
			End If
			If w = 65 Then
				r = -4.365
			End If
			If w = 64 Then
				r = -4.451
			End If
			If w = 63 Then
				r = -4.539
			End If
			If w = 62 Then
				r = -4.628
			End If
			If w = 61 Then
				r = -4.718
			End If
			If w = 60 Then
				r = -4.809
			End If
			If w = 59 Then
				r = -4.902
			End If
			If w = 58 Then
				r = -4.996
			End If
			If w = 57 Then
				r = -5.092
			End If
			If w = 56 Then
				r = -5.189
			End If
			If w = 55 Then
				r = -5.287
			End If
			If w = 54 Then
				r = -5.388
			End If
			If w = 53 Then
				r = -5.489
			End If
			If w = 52 Then
				r = -5.592
			End If
			If w = 51 Then
				r = -5.697
			End If
			If w = 50 Then
				r = -5.804
			End If
			If w = 49 Then
				r = -5.912
			End If
			If w = 48 Then
				r = -6.022
			End If
			If w = 47 Then
				r = -6.133
			End If
			If w = 46 Then
				r = -6.247
			End If
			If w = 45 Then
				r = -6.362
			End If
			If w = 44 Then
				r = -6.479
			End If
			If w = 43 Then
				r = -6.598
			End If
			If w = 42 Then
				r = -6.719
			End If
			If w = 41 Then
				r = -6.842
			End If
			If w = 40 Then
				r = -6.967
			End If
			If w = 39 Then
				r = -7.094
			End If
			If w = 38 Then
				r = -7.224
			End If
			If w = 37 Then
				r = -7.355
			End If
			If w = 36 Then
				r = -7.489
			End If
			If w = 35 Then
				r = -7.625
			End If
			If w = 34 Then
				r = -7.764
			End If
			If w = 33 Then
				r = -7.905
			End If
			If w = 32 Then
				r = -8.049
			End If
			If w = 31 Then
				r = -8.196
			End If
			If w = 30 Then
				r = -8.345
			End If
			If w = 29 Then
				r = -8.498
			End If
			If w = 28 Then
				r = -8.653
			End If
			If w = 27 Then
				r = -8.811
			End If
			If w = 26 Then
				r = -8.974
			End If
			If w = 25 Then
				r = -9.139
			End If
			If w = 24 Then
				r = -9.308
			End If
			If w = 23 Then
				r = -9.481
			End If
			If w = 22 Then
				r = -9.658
			End If
			If w = 21 Then
				r = -9.84
			End If
			If w = 20 Then
				r = -10.03
			End If
			If w = 19 Then
				r = -10.22
			End If
			If w = 18 Then
				r = -10.41
			End If
			If w = 17 Then
				r = -10.61
			End If
			If w = 16 Then
				r = -10.81
			End If
			If w = 15 Then
				r = -11.02
			End If
			If w = 14 Then
				r = -11.24
			End If
			If w = 13 Then
				r = -11.47
			End If
			If w = 12 Then
				r = -11.69
			End If
			If w = 11 Then
				r = -11.94
			End If
			If w = 10 Then
				r = -12.18
			End If
			If w = 9 Then
				r = -12.45
			End If
			If w = 8 Then
				r = -12.72
			End If
			If w = 7 Then
				r = -13.0
			End If
			If w = 6 Then
				r = -13.3
			End If
			If w = 5 Then
				r = -13.64
			End If
			If w = 4 Then
				r = -14.0
			End If
			If w = 3 Then
				r = -14.33
			End If
			If w = 2 Then
				r = -14.84
			End If
			If w = 1 Then
				r = -15.25
			End If
			If w <= 0 Then
				r = -15.94
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 24)
'        ************************************************************************

		Private Shared Function w24(s As Double) As Double
			Dim result As Double = 0
			Dim w As Integer = 0
			Dim r As Double = 0

			r = 0
			w = CInt(System.Math.Truncate(System.Math.Round(-(35.0 * s) + 150.0)))
			If w >= 150 Then
				r = -0.682
			End If
			If w = 149 Then
				r = -0.7044
			End If
			If w = 148 Then
				r = -0.7273
			End If
			If w = 147 Then
				r = -0.7507
			End If
			If w = 146 Then
				r = -0.7746
			End If
			If w = 145 Then
				r = -0.799
			End If
			If w = 144 Then
				r = -0.8239
			End If
			If w = 143 Then
				r = -0.8494
			End If
			If w = 142 Then
				r = -0.8754
			End If
			If w = 141 Then
				r = -0.902
			End If
			If w = 140 Then
				r = -0.9291
			End If
			If w = 139 Then
				r = -0.9567
			End If
			If w = 138 Then
				r = -0.9849
			End If
			If w = 137 Then
				r = -1.014
			End If
			If w = 136 Then
				r = -1.043
			End If
			If w = 135 Then
				r = -1.073
			End If
			If w = 134 Then
				r = -1.103
			End If
			If w = 133 Then
				r = -1.135
			End If
			If w = 132 Then
				r = -1.166
			End If
			If w = 131 Then
				r = -1.198
			End If
			If w = 130 Then
				r = -1.231
			End If
			If w = 129 Then
				r = -1.265
			End If
			If w = 128 Then
				r = -1.299
			End If
			If w = 127 Then
				r = -1.334
			End If
			If w = 126 Then
				r = -1.369
			End If
			If w = 125 Then
				r = -1.405
			End If
			If w = 124 Then
				r = -1.441
			End If
			If w = 123 Then
				r = -1.479
			End If
			If w = 122 Then
				r = -1.517
			End If
			If w = 121 Then
				r = -1.555
			End If
			If w = 120 Then
				r = -1.594
			End If
			If w = 119 Then
				r = -1.634
			End If
			If w = 118 Then
				r = -1.675
			End If
			If w = 117 Then
				r = -1.716
			End If
			If w = 116 Then
				r = -1.758
			End If
			If w = 115 Then
				r = -1.8
			End If
			If w = 114 Then
				r = -1.844
			End If
			If w = 113 Then
				r = -1.888
			End If
			If w = 112 Then
				r = -1.932
			End If
			If w = 111 Then
				r = -1.978
			End If
			If w = 110 Then
				r = -2.024
			End If
			If w = 109 Then
				r = -2.07
			End If
			If w = 108 Then
				r = -2.118
			End If
			If w = 107 Then
				r = -2.166
			End If
			If w = 106 Then
				r = -2.215
			End If
			If w = 105 Then
				r = -2.265
			End If
			If w = 104 Then
				r = -2.316
			End If
			If w = 103 Then
				r = -2.367
			End If
			If w = 102 Then
				r = -2.419
			End If
			If w = 101 Then
				r = -2.472
			End If
			If w = 100 Then
				r = -2.526
			End If
			If w = 99 Then
				r = -2.58
			End If
			If w = 98 Then
				r = -2.636
			End If
			If w = 97 Then
				r = -2.692
			End If
			If w = 96 Then
				r = -2.749
			End If
			If w = 95 Then
				r = -2.806
			End If
			If w = 94 Then
				r = -2.865
			End If
			If w = 93 Then
				r = -2.925
			End If
			If w = 92 Then
				r = -2.985
			End If
			If w = 91 Then
				r = -3.046
			End If
			If w = 90 Then
				r = -3.108
			End If
			If w = 89 Then
				r = -3.171
			End If
			If w = 88 Then
				r = -3.235
			End If
			If w = 87 Then
				r = -3.3
			End If
			If w = 86 Then
				r = -3.365
			End If
			If w = 85 Then
				r = -3.432
			End If
			If w = 84 Then
				r = -3.499
			End If
			If w = 83 Then
				r = -3.568
			End If
			If w = 82 Then
				r = -3.637
			End If
			If w = 81 Then
				r = -3.708
			End If
			If w = 80 Then
				r = -3.779
			End If
			If w = 79 Then
				r = -3.852
			End If
			If w = 78 Then
				r = -3.925
			End If
			If w = 77 Then
				r = -4.0
			End If
			If w = 76 Then
				r = -4.075
			End If
			If w = 75 Then
				r = -4.151
			End If
			If w = 74 Then
				r = -4.229
			End If
			If w = 73 Then
				r = -4.308
			End If
			If w = 72 Then
				r = -4.387
			End If
			If w = 71 Then
				r = -4.468
			End If
			If w = 70 Then
				r = -4.55
			End If
			If w = 69 Then
				r = -4.633
			End If
			If w = 68 Then
				r = -4.718
			End If
			If w = 67 Then
				r = -4.803
			End If
			If w = 66 Then
				r = -4.89
			End If
			If w = 65 Then
				r = -4.978
			End If
			If w = 64 Then
				r = -5.067
			End If
			If w = 63 Then
				r = -5.157
			End If
			If w = 62 Then
				r = -5.249
			End If
			If w = 61 Then
				r = -5.342
			End If
			If w = 60 Then
				r = -5.436
			End If
			If w = 59 Then
				r = -5.531
			End If
			If w = 58 Then
				r = -5.628
			End If
			If w = 57 Then
				r = -5.727
			End If
			If w = 56 Then
				r = -5.826
			End If
			If w = 55 Then
				r = -5.927
			End If
			If w = 54 Then
				r = -6.03
			End If
			If w = 53 Then
				r = -6.134
			End If
			If w = 52 Then
				r = -6.24
			End If
			If w = 51 Then
				r = -6.347
			End If
			If w = 50 Then
				r = -6.456
			End If
			If w = 49 Then
				r = -6.566
			End If
			If w = 48 Then
				r = -6.678
			End If
			If w = 47 Then
				r = -6.792
			End If
			If w = 46 Then
				r = -6.907
			End If
			If w = 45 Then
				r = -7.025
			End If
			If w = 44 Then
				r = -7.144
			End If
			If w = 43 Then
				r = -7.265
			End If
			If w = 42 Then
				r = -7.387
			End If
			If w = 41 Then
				r = -7.512
			End If
			If w = 40 Then
				r = -7.639
			End If
			If w = 39 Then
				r = -7.768
			End If
			If w = 38 Then
				r = -7.899
			End If
			If w = 37 Then
				r = -8.032
			End If
			If w = 36 Then
				r = -8.167
			End If
			If w = 35 Then
				r = -8.305
			End If
			If w = 34 Then
				r = -8.445
			End If
			If w = 33 Then
				r = -8.588
			End If
			If w = 32 Then
				r = -8.733
			End If
			If w = 31 Then
				r = -8.881
			End If
			If w = 30 Then
				r = -9.031
			End If
			If w = 29 Then
				r = -9.185
			End If
			If w = 28 Then
				r = -9.341
			End If
			If w = 27 Then
				r = -9.501
			End If
			If w = 26 Then
				r = -9.664
			End If
			If w = 25 Then
				r = -9.83
			End If
			If w = 24 Then
				r = -10.0
			End If
			If w = 23 Then
				r = -10.17
			End If
			If w = 22 Then
				r = -10.35
			End If
			If w = 21 Then
				r = -10.53
			End If
			If w = 20 Then
				r = -10.72
			End If
			If w = 19 Then
				r = -10.91
			End If
			If w = 18 Then
				r = -11.1
			End If
			If w = 17 Then
				r = -11.3
			End If
			If w = 16 Then
				r = -11.51
			End If
			If w = 15 Then
				r = -11.72
			End If
			If w = 14 Then
				r = -11.94
			End If
			If w = 13 Then
				r = -12.16
			End If
			If w = 12 Then
				r = -12.39
			End If
			If w = 11 Then
				r = -12.63
			End If
			If w = 10 Then
				r = -12.87
			End If
			If w = 9 Then
				r = -13.14
			End If
			If w = 8 Then
				r = -13.42
			End If
			If w = 7 Then
				r = -13.69
			End If
			If w = 6 Then
				r = -14.0
			End If
			If w = 5 Then
				r = -14.33
			End If
			If w = 4 Then
				r = -14.69
			End If
			If w = 3 Then
				r = -15.03
			End If
			If w = 2 Then
				r = -15.54
			End If
			If w = 1 Then
				r = -15.94
			End If
			If w <= 0 Then
				r = -16.64
			End If
			result = r
			Return result
		End Function


		'************************************************************************
'        Tail(S, 25)
'        ************************************************************************

		Private Shared Function w25(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -5.150509, tj, tj1, result)
			wcheb(x, -5.695528, tj, tj1, result)
			wcheb(x, -1.437637, tj, tj1, result)
			wcheb(x, -0.2611906, tj, tj1, result)
			wcheb(x, -0.07625722, tj, tj1, result)
			wcheb(x, -0.02579892, tj, tj1, result)
			wcheb(x, -0.01086876, tj, tj1, result)
			wcheb(x, -0.002906543, tj, tj1, result)
			wcheb(x, -0.002354881, tj, tj1, result)
			wcheb(x, 0.0001007195, tj, tj1, result)
			wcheb(x, -0.0008437327, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 26)
'        ************************************************************************

		Private Shared Function w26(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -5.117622, tj, tj1, result)
			wcheb(x, -5.635159, tj, tj1, result)
			wcheb(x, -1.395167, tj, tj1, result)
			wcheb(x, -0.2382823, tj, tj1, result)
			wcheb(x, -0.06531987, tj, tj1, result)
			wcheb(x, -0.02060112, tj, tj1, result)
			wcheb(x, -0.008203697, tj, tj1, result)
			wcheb(x, -0.001516523, tj, tj1, result)
			wcheb(x, -0.001431364, tj, tj1, result)
			wcheb(x, 0.0006384553, tj, tj1, result)
			wcheb(x, -0.0003238369, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 27)
'        ************************************************************************

		Private Shared Function w27(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -5.089731, tj, tj1, result)
			wcheb(x, -5.584248, tj, tj1, result)
			wcheb(x, -1.359966, tj, tj1, result)
			wcheb(x, -0.2203696, tj, tj1, result)
			wcheb(x, -0.05753344, tj, tj1, result)
			wcheb(x, -0.01761891, tj, tj1, result)
			wcheb(x, -0.007096897, tj, tj1, result)
			wcheb(x, -0.001419108, tj, tj1, result)
			wcheb(x, -0.001581214, tj, tj1, result)
			wcheb(x, 0.0003033766, tj, tj1, result)
			wcheb(x, -0.0005901441, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 28)
'        ************************************************************************

		Private Shared Function w28(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -5.065046, tj, tj1, result)
			wcheb(x, -5.539163, tj, tj1, result)
			wcheb(x, -1.328939, tj, tj1, result)
			wcheb(x, -0.2046376, tj, tj1, result)
			wcheb(x, -0.05061515, tj, tj1, result)
			wcheb(x, -0.01469271, tj, tj1, result)
			wcheb(x, -0.005711578, tj, tj1, result)
			wcheb(x, -0.0008389153, tj, tj1, result)
			wcheb(x, -0.001250575, tj, tj1, result)
			wcheb(x, 0.0004047245, tj, tj1, result)
			wcheb(x, -0.0005128555, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 29)
'        ************************************************************************

		Private Shared Function w29(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -5.043413, tj, tj1, result)
			wcheb(x, -5.499756, tj, tj1, result)
			wcheb(x, -1.302137, tj, tj1, result)
			wcheb(x, -0.1915129, tj, tj1, result)
			wcheb(x, -0.04516329, tj, tj1, result)
			wcheb(x, -0.01260064, tj, tj1, result)
			wcheb(x, -0.004817269, tj, tj1, result)
			wcheb(x, -0.000547813, tj, tj1, result)
			wcheb(x, -0.001111668, tj, tj1, result)
			wcheb(x, 0.0004093451, tj, tj1, result)
			wcheb(x, -0.000513586, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 30)
'        ************************************************************************

		Private Shared Function w30(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -5.024071, tj, tj1, result)
			wcheb(x, -5.464515, tj, tj1, result)
			wcheb(x, -1.278342, tj, tj1, result)
			wcheb(x, -0.180003, tj, tj1, result)
			wcheb(x, -0.04046294, tj, tj1, result)
			wcheb(x, -0.01076162, tj, tj1, result)
			wcheb(x, -0.003968677, tj, tj1, result)
			wcheb(x, -0.0001911679, tj, tj1, result)
			wcheb(x, -0.0008619185, tj, tj1, result)
			wcheb(x, 0.0005125362, tj, tj1, result)
			wcheb(x, -0.000398437, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 40)
'        ************************************************************************

		Private Shared Function w40(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -4.904809, tj, tj1, result)
			wcheb(x, -5.248327, tj, tj1, result)
			wcheb(x, -1.136698, tj, tj1, result)
			wcheb(x, -0.1170982, tj, tj1, result)
			wcheb(x, -0.01824427, tj, tj1, result)
			wcheb(x, -0.003888648, tj, tj1, result)
			wcheb(x, -0.001344929, tj, tj1, result)
			wcheb(x, 0.0002790407, tj, tj1, result)
			wcheb(x, -0.0004619858, tj, tj1, result)
			wcheb(x, 0.0003359121, tj, tj1, result)
			wcheb(x, -0.0002883026, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 60)
'        ************************************************************************

		Private Shared Function w60(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -4.809656, tj, tj1, result)
			wcheb(x, -5.077191, tj, tj1, result)
			wcheb(x, -1.029402, tj, tj1, result)
			wcheb(x, -0.07507931, tj, tj1, result)
			wcheb(x, -0.006506226, tj, tj1, result)
			wcheb(x, -0.001391278, tj, tj1, result)
			wcheb(x, -0.0004263635, tj, tj1, result)
			wcheb(x, 0.0002302271, tj, tj1, result)
			wcheb(x, -0.0002384348, tj, tj1, result)
			wcheb(x, 0.0001865587, tj, tj1, result)
			wcheb(x, -0.0001622355, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 120)
'        ************************************************************************

		Private Shared Function w120(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -4.729426, tj, tj1, result)
			wcheb(x, -4.934426, tj, tj1, result)
			wcheb(x, -0.9433231, tj, tj1, result)
			wcheb(x, -0.04492504, tj, tj1, result)
			wcheb(x, 1.673948E-05, tj, tj1, result)
			wcheb(x, -0.0006077014, tj, tj1, result)
			wcheb(x, -7.215768E-05, tj, tj1, result)
			wcheb(x, 9.086734E-05, tj, tj1, result)
			wcheb(x, -8.44798E-05, tj, tj1, result)
			wcheb(x, 6.705028E-05, tj, tj1, result)
			wcheb(x, -5.828507E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S, 200)
'        ************************************************************************

		Private Shared Function w200(s As Double) As Double
			Dim result As Double = 0
			Dim x As Double = 0
			Dim tj As Double = 0
			Dim tj1 As Double = 0

			result = 0
			x = System.Math.Min(2 * (s - 0.0) / 4.0 - 1, 1.0)
			tj = 1
			tj1 = x
			wcheb(x, -4.70024, tj, tj1, result)
			wcheb(x, -4.88308, tj, tj1, result)
			wcheb(x, -0.9132168, tj, tj1, result)
			wcheb(x, -0.03512684, tj, tj1, result)
			wcheb(x, 0.001726342, tj, tj1, result)
			wcheb(x, -0.0005189796, tj, tj1, result)
			wcheb(x, -1.628659E-06, tj, tj1, result)
			wcheb(x, 4.261786E-05, tj, tj1, result)
			wcheb(x, -4.002498E-05, tj, tj1, result)
			wcheb(x, 3.146287E-05, tj, tj1, result)
			wcheb(x, -2.727576E-05, tj, tj1, result)
			Return result
		End Function


		'************************************************************************
'        Tail(S,N), S>=0
'        ************************************************************************

		Private Shared Function wsigma(s As Double, n As Integer) As Double
			Dim result As Double = 0
			Dim f0 As Double = 0
			Dim f1 As Double = 0
			Dim f2 As Double = 0
			Dim f3 As Double = 0
			Dim f4 As Double = 0
			Dim x0 As Double = 0
			Dim x1 As Double = 0
			Dim x2 As Double = 0
			Dim x3 As Double = 0
			Dim x4 As Double = 0
			Dim x As Double = 0

			result = 0
			If n = 5 Then
				result = w5(s)
			End If
			If n = 6 Then
				result = w6(s)
			End If
			If n = 7 Then
				result = w7(s)
			End If
			If n = 8 Then
				result = w8(s)
			End If
			If n = 9 Then
				result = w9(s)
			End If
			If n = 10 Then
				result = w10(s)
			End If
			If n = 11 Then
				result = w11(s)
			End If
			If n = 12 Then
				result = w12(s)
			End If
			If n = 13 Then
				result = w13(s)
			End If
			If n = 14 Then
				result = w14(s)
			End If
			If n = 15 Then
				result = w15(s)
			End If
			If n = 16 Then
				result = w16(s)
			End If
			If n = 17 Then
				result = w17(s)
			End If
			If n = 18 Then
				result = w18(s)
			End If
			If n = 19 Then
				result = w19(s)
			End If
			If n = 20 Then
				result = w20(s)
			End If
			If n = 21 Then
				result = w21(s)
			End If
			If n = 22 Then
				result = w22(s)
			End If
			If n = 23 Then
				result = w23(s)
			End If
			If n = 24 Then
				result = w24(s)
			End If
			If n = 25 Then
				result = w25(s)
			End If
			If n = 26 Then
				result = w26(s)
			End If
			If n = 27 Then
				result = w27(s)
			End If
			If n = 28 Then
				result = w28(s)
			End If
			If n = 29 Then
				result = w29(s)
			End If
			If n = 30 Then
				result = w30(s)
			End If
			If n > 30 Then
				x = 1.0 / n
				x0 = 1.0 / 30
				f0 = w30(s)
				x1 = 1.0 / 40
				f1 = w40(s)
				x2 = 1.0 / 60
				f2 = w60(s)
				x3 = 1.0 / 120
				f3 = w120(s)
				x4 = 1.0 / 200
				f4 = w200(s)
				f1 = ((x - x0) * f1 - (x - x1) * f0) / (x1 - x0)
				f2 = ((x - x0) * f2 - (x - x2) * f0) / (x2 - x0)
				f3 = ((x - x0) * f3 - (x - x3) * f0) / (x3 - x0)
				f4 = ((x - x0) * f4 - (x - x4) * f0) / (x4 - x0)
				f2 = ((x - x1) * f2 - (x - x2) * f1) / (x2 - x1)
				f3 = ((x - x1) * f3 - (x - x3) * f1) / (x3 - x1)
				f4 = ((x - x1) * f4 - (x - x4) * f1) / (x4 - x1)
				f3 = ((x - x2) * f3 - (x - x3) * f2) / (x3 - x2)
				f4 = ((x - x2) * f4 - (x - x4) * f2) / (x4 - x2)
				f4 = ((x - x3) * f4 - (x - x4) * f3) / (x4 - x3)
				result = f4
			End If
			Return result
		End Function


	End Class
End Class

