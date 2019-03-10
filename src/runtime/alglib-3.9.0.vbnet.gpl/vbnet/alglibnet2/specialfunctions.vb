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
'    Gamma function
'
'    Input parameters:
'        X   -   argument
'
'    Domain:
'        0 < X < 171.6
'        -170 < X < 0, X is not an integer.
'
'    Relative error:
'     arithmetic   domain     # trials      peak         rms
'        IEEE    -170,-33      20000       2.3e-15     3.3e-16
'        IEEE     -33,  33     20000       9.4e-16     2.2e-16
'        IEEE      33, 171.6   20000       2.3e-15     3.2e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Original copyright 1984, 1987, 1989, 1992, 2000 by Stephen L. Moshier
'    Translated to AlgoPascal by Bochkanov Sergey (2005, 2006, 2007).
'    ************************************************************************

	Public Shared Function gammafunction(x As Double) As Double

		Dim result As Double = gammafunc.gammafunction(x)
		Return result
	End Function

	'************************************************************************
'    Natural logarithm of gamma function
'
'    Input parameters:
'        X       -   argument
'
'    Result:
'        logarithm of the absolute value of the Gamma(X).
'
'    Output parameters:
'        SgnGam  -   sign(Gamma(X))
'
'    Domain:
'        0 < X < 2.55e305
'        -2.55e305 < X < 0, X is not an integer.
'
'    ACCURACY:
'    arithmetic      domain        # trials     peak         rms
'       IEEE    0, 3                 28000     5.4e-16     1.1e-16
'       IEEE    2.718, 2.556e305     40000     3.5e-16     8.3e-17
'    The error criterion was relative when the function magnitude
'    was greater than one but absolute when it was less than one.
'
'    The following test used the relative error criterion, though
'    at certain points the relative error could be much higher than
'    indicated.
'       IEEE    -200, -4             10000     4.8e-16     1.3e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1989, 1992, 2000 by Stephen L. Moshier
'    Translated to AlgoPascal by Bochkanov Sergey (2005, 2006, 2007).
'    ************************************************************************

	Public Shared Function lngamma(x As Double, ByRef sgngam As Double) As Double
		sgngam = 0
		Dim result As Double = gammafunc.lngamma(x, sgngam)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Error function
'
'    The integral is
'
'                              x
'                               -
'                    2         | |          2
'      erf(x)  =  --------     |    exp( - t  ) dt.
'                 sqrt(pi)   | |
'                             -
'                              0
'
'    For 0 <= |x| < 1, erf(x) = x * P4(x**2)/Q5(x**2); otherwise
'    erf(x) = 1 - erfc(x).
'
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0,1         30000       3.7e-16     1.0e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function errorfunction(x As Double) As Double

		Dim result As Double = normaldistr.errorfunction(x)
		Return result
	End Function

	'************************************************************************
'    Complementary error function
'
'     1 - erf(x) =
'
'                              inf.
'                                -
'                     2         | |          2
'      erfc(x)  =  --------     |    exp( - t  ) dt
'                  sqrt(pi)   | |
'                              -
'                               x
'
'
'    For small x, erfc(x) = 1 - erf(x); otherwise rational
'    approximations are computed.
'
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0,26.6417   30000       5.7e-14     1.5e-14
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function errorfunctionc(x As Double) As Double

		Dim result As Double = normaldistr.errorfunctionc(x)
		Return result
	End Function

	'************************************************************************
'    Normal distribution function
'
'    Returns the area under the Gaussian probability density
'    function, integrated from minus infinity to x:
'
'                               x
'                                -
'                      1        | |          2
'       ndtr(x)  = ---------    |    exp( - t /2 ) dt
'                  sqrt(2pi)  | |
'                              -
'                             -inf.
'
'                =  ( 1 + erf(z) ) / 2
'                =  erfc(z) / 2
'
'    where z = x/sqrt(2). Computation is via the functions
'    erf and erfc.
'
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE     -13,0        30000       3.4e-14     6.7e-15
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function normaldistribution(x As Double) As Double

		Dim result As Double = normaldistr.normaldistribution(x)
		Return result
	End Function

	'************************************************************************
'    Inverse of the error function
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function inverf(e As Double) As Double

		Dim result As Double = normaldistr.inverf(e)
		Return result
	End Function

	'************************************************************************
'    Inverse of Normal distribution function
'
'    Returns the argument, x, for which the area under the
'    Gaussian probability density function (integrated from
'    minus infinity to x) is equal to y.
'
'
'    For small arguments 0 < y < exp(-2), the program computes
'    z = sqrt( -2.0 * log(y) );  then the approximation is
'    x = z - log(z)/z  - (1/z) P(1/z) / Q(1/z).
'    There are two rational functions P/Q, one for 0 < y < exp(-32)
'    and the other for y up to exp(-2).  For larger arguments,
'    w = y - 0.5, and  x/sqrt(2pi) = w + w**3 R(w**2)/S(w**2)).
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain        # trials      peak         rms
'       IEEE     0.125, 1        20000       7.2e-16     1.3e-16
'       IEEE     3e-308, 0.135   50000       4.6e-16     9.8e-17
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function invnormaldistribution(y0 As Double) As Double

		Dim result As Double = normaldistr.invnormaldistribution(y0)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Incomplete gamma integral
'
'    The function is defined by
'
'                              x
'                               -
'                      1       | |  -t  a-1
'     igam(a,x)  =   -----     |   e   t   dt.
'                     -      | |
'                    | (a)    -
'                              0
'
'
'    In this implementation both arguments must be positive.
'    The integral is evaluated by either a power series or
'    continued fraction expansion, depending on the relative
'    values of a and x.
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0,30       200000       3.6e-14     2.9e-15
'       IEEE      0,100      300000       9.9e-14     1.5e-14
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1985, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function incompletegamma(a As Double, x As Double) As Double

		Dim result As Double = igammaf.incompletegamma(a, x)
		Return result
	End Function

	'************************************************************************
'    Complemented incomplete gamma integral
'
'    The function is defined by
'
'
'     igamc(a,x)   =   1 - igam(a,x)
'
'                               inf.
'                                 -
'                        1       | |  -t  a-1
'                  =   -----     |   e   t   dt.
'                       -      | |
'                      | (a)    -
'                                x
'
'
'    In this implementation both arguments must be positive.
'    The integral is evaluated by either a power series or
'    continued fraction expansion, depending on the relative
'    values of a and x.
'
'    ACCURACY:
'
'    Tested at random a, x.
'                   a         x                      Relative error:
'    arithmetic   domain   domain     # trials      peak         rms
'       IEEE     0.5,100   0,100      200000       1.9e-14     1.7e-15
'       IEEE     0.01,0.5  0,100      200000       1.4e-13     1.6e-15
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1985, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function incompletegammac(a As Double, x As Double) As Double

		Dim result As Double = igammaf.incompletegammac(a, x)
		Return result
	End Function

	'************************************************************************
'    Inverse of complemented imcomplete gamma integral
'
'    Given p, the function finds x such that
'
'     igamc( a, x ) = p.
'
'    Starting with the approximate value
'
'            3
'     x = a t
'
'     where
'
'     t = 1 - d - ndtri(p) sqrt(d)
'
'    and
'
'     d = 1/9a,
'
'    the routine performs up to 10 Newton iterations to find the
'    root of igamc(a,x) - p = 0.
'
'    ACCURACY:
'
'    Tested at random a, p in the intervals indicated.
'
'                   a        p                      Relative error:
'    arithmetic   domain   domain     # trials      peak         rms
'       IEEE     0.5,100   0,0.5       100000       1.0e-14     1.7e-15
'       IEEE     0.01,0.5  0,0.5       100000       9.0e-14     3.4e-15
'       IEEE    0.5,10000  0,0.5        20000       2.3e-13     3.8e-14
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function invincompletegammac(a As Double, y0 As Double) As Double

		Dim result As Double = igammaf.invincompletegammac(a, y0)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Airy function
'
'    Solution of the differential equation
'
'    y"(x) = xy.
'
'    The function returns the two independent solutions Ai, Bi
'    and their first derivatives Ai'(x), Bi'(x).
'
'    Evaluation is by power series summation for small x,
'    by rational minimax approximations for large x.
'
'
'
'    ACCURACY:
'    Error criterion is absolute when function <= 1, relative
'    when function > 1, except * denotes relative error criterion.
'    For large negative x, the absolute error increases as x^1.5.
'    For large positive x, the relative error increases as x^1.5.
'
'    Arithmetic  domain   function  # trials      peak         rms
'    IEEE        -10, 0     Ai        10000       1.6e-15     2.7e-16
'    IEEE          0, 10    Ai        10000       2.3e-14*    1.8e-15*
'    IEEE        -10, 0     Ai'       10000       4.6e-15     7.6e-16
'    IEEE          0, 10    Ai'       10000       1.8e-14*    1.5e-15*
'    IEEE        -10, 10    Bi        30000       4.2e-15     5.3e-16
'    IEEE        -10, 10    Bi'       30000       4.9e-15     7.3e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Sub airy(x As Double, ByRef ai As Double, ByRef aip As Double, ByRef bi As Double, ByRef bip As Double)
		ai = 0
		aip = 0
		bi = 0
		bip = 0
		airyf.airy(x, ai, aip, bi, bip)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Bessel function of order zero
'
'    Returns Bessel function of order zero of the argument.
'
'    The domain is divided into the intervals [0, 5] and
'    (5, infinity). In the first interval the following rational
'    approximation is used:
'
'
'           2         2
'    (w - r  ) (w - r  ) P (w) / Q (w)
'          1         2    3       8
'
'               2
'    where w = x  and the two r's are zeros of the function.
'
'    In the second interval, the Hankel asymptotic expansion
'    is employed with two rational functions of degree 6/6
'    and 7/7.
'
'    ACCURACY:
'
'                         Absolute error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0, 30       60000       4.2e-16     1.1e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function besselj0(x As Double) As Double

		Dim result As Double = bessel.besselj0(x)
		Return result
	End Function

	'************************************************************************
'    Bessel function of order one
'
'    Returns Bessel function of order one of the argument.
'
'    The domain is divided into the intervals [0, 8] and
'    (8, infinity). In the first interval a 24 term Chebyshev
'    expansion is used. In the second, the asymptotic
'    trigonometric representation is employed using two
'    rational functions of degree 5/5.
'
'    ACCURACY:
'
'                         Absolute error:
'    arithmetic   domain      # trials      peak         rms
'       IEEE      0, 30       30000       2.6e-16     1.1e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function besselj1(x As Double) As Double

		Dim result As Double = bessel.besselj1(x)
		Return result
	End Function

	'************************************************************************
'    Bessel function of integer order
'
'    Returns Bessel function of order n, where n is a
'    (possibly negative) integer.
'
'    The ratio of jn(x) to j0(x) is computed by backward
'    recurrence.  First the ratio jn/jn-1 is found by a
'    continued fraction expansion.  Then the recurrence
'    relating successive orders is applied until j0 or j1 is
'    reached.
'
'    If n = 0 or 1 the routine for j0 or j1 is called
'    directly.
'
'    ACCURACY:
'
'                         Absolute error:
'    arithmetic   range      # trials      peak         rms
'       IEEE      0, 30        5000       4.4e-16     7.9e-17
'
'
'    Not suitable for large n or x. Use jv() (fractional order) instead.
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function besseljn(n As Integer, x As Double) As Double

		Dim result As Double = bessel.besseljn(n, x)
		Return result
	End Function

	'************************************************************************
'    Bessel function of the second kind, order zero
'
'    Returns Bessel function of the second kind, of order
'    zero, of the argument.
'
'    The domain is divided into the intervals [0, 5] and
'    (5, infinity). In the first interval a rational approximation
'    R(x) is employed to compute
'      y0(x)  = R(x)  +   2 * log(x) * j0(x) / PI.
'    Thus a call to j0() is required.
'
'    In the second interval, the Hankel asymptotic expansion
'    is employed with two rational functions of degree 6/6
'    and 7/7.
'
'
'
'    ACCURACY:
'
'     Absolute error, when y0(x) < 1; else relative error:
'
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0, 30       30000       1.3e-15     1.6e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function bessely0(x As Double) As Double

		Dim result As Double = bessel.bessely0(x)
		Return result
	End Function

	'************************************************************************
'    Bessel function of second kind of order one
'
'    Returns Bessel function of the second kind of order one
'    of the argument.
'
'    The domain is divided into the intervals [0, 8] and
'    (8, infinity). In the first interval a 25 term Chebyshev
'    expansion is used, and a call to j1() is required.
'    In the second, the asymptotic trigonometric representation
'    is employed using two rational functions of degree 5/5.
'
'    ACCURACY:
'
'                         Absolute error:
'    arithmetic   domain      # trials      peak         rms
'       IEEE      0, 30       30000       1.0e-15     1.3e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function bessely1(x As Double) As Double

		Dim result As Double = bessel.bessely1(x)
		Return result
	End Function

	'************************************************************************
'    Bessel function of second kind of integer order
'
'    Returns Bessel function of order n, where n is a
'    (possibly negative) integer.
'
'    The function is evaluated by forward recurrence on
'    n, starting with values computed by the routines
'    y0() and y1().
'
'    If n = 0 or 1 the routine for y0 or y1 is called
'    directly.
'
'    ACCURACY:
'                         Absolute error, except relative
'                         when y > 1:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0, 30       30000       3.4e-15     4.3e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function besselyn(n As Integer, x As Double) As Double

		Dim result As Double = bessel.besselyn(n, x)
		Return result
	End Function

	'************************************************************************
'    Modified Bessel function of order zero
'
'    Returns modified Bessel function of order zero of the
'    argument.
'
'    The function is defined as i0(x) = j0( ix ).
'
'    The range is partitioned into the two intervals [0,8] and
'    (8, infinity).  Chebyshev polynomial expansions are employed
'    in each interval.
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0,30        30000       5.8e-16     1.4e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function besseli0(x As Double) As Double

		Dim result As Double = bessel.besseli0(x)
		Return result
	End Function

	'************************************************************************
'    Modified Bessel function of order one
'
'    Returns modified Bessel function of order one of the
'    argument.
'
'    The function is defined as i1(x) = -i j1( ix ).
'
'    The range is partitioned into the two intervals [0,8] and
'    (8, infinity).  Chebyshev polynomial expansions are employed
'    in each interval.
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0, 30       30000       1.9e-15     2.1e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1985, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function besseli1(x As Double) As Double

		Dim result As Double = bessel.besseli1(x)
		Return result
	End Function

	'************************************************************************
'    Modified Bessel function, second kind, order zero
'
'    Returns modified Bessel function of the second kind
'    of order zero of the argument.
'
'    The range is partitioned into the two intervals [0,8] and
'    (8, infinity).  Chebyshev polynomial expansions are employed
'    in each interval.
'
'    ACCURACY:
'
'    Tested at 2000 random points between 0 and 8.  Peak absolute
'    error (relative when K0 > 1) was 1.46e-14; rms, 4.26e-15.
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0, 30       30000       1.2e-15     1.6e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function besselk0(x As Double) As Double

		Dim result As Double = bessel.besselk0(x)
		Return result
	End Function

	'************************************************************************
'    Modified Bessel function, second kind, order one
'
'    Computes the modified Bessel function of the second kind
'    of order one of the argument.
'
'    The range is partitioned into the two intervals [0,2] and
'    (2, infinity).  Chebyshev polynomial expansions are employed
'    in each interval.
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0, 30       30000       1.2e-15     1.6e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function besselk1(x As Double) As Double

		Dim result As Double = bessel.besselk1(x)
		Return result
	End Function

	'************************************************************************
'    Modified Bessel function, second kind, integer order
'
'    Returns modified Bessel function of the second kind
'    of order n of the argument.
'
'    The range is partitioned into the two intervals [0,9.55] and
'    (9.55, infinity).  An ascending power series is used in the
'    low range, and an asymptotic expansion in the high range.
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0,30        90000       1.8e-8      3.0e-10
'
'    Error is high only near the crossover point x = 9.55
'    between the two expansions used.
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1988, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function besselkn(nn As Integer, x As Double) As Double

		Dim result As Double = bessel.besselkn(nn, x)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Beta function
'
'
'                      -     -
'                     | (a) | (b)
'    beta( a, b )  =  -----------.
'                        -
'                       | (a+b)
'
'    For large arguments the logarithm of the function is
'    evaluated using lgam(), then exponentiated.
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE       0,30       30000       8.1e-14     1.1e-14
'
'    Cephes Math Library Release 2.0:  April, 1987
'    Copyright 1984, 1987 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function beta(a As Double, b As Double) As Double

		Dim result As Double = betaf.beta(a, b)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Incomplete beta integral
'
'    Returns incomplete beta integral of the arguments, evaluated
'    from zero to x.  The function is defined as
'
'                     x
'        -            -
'       | (a+b)      | |  a-1     b-1
'     -----------    |   t   (1-t)   dt.
'      -     -     | |
'     | (a) | (b)   -
'                    0
'
'    The domain of definition is 0 <= x <= 1.  In this
'    implementation a and b are restricted to positive values.
'    The integral from x to 1 may be obtained by the symmetry
'    relation
'
'       1 - incbet( a, b, x )  =  incbet( b, a, 1-x ).
'
'    The integral is evaluated by a continued fraction expansion
'    or, when b*x is small, by a power series.
'
'    ACCURACY:
'
'    Tested at uniformly distributed random points (a,b,x) with a and b
'    in "domain" and x between 0 and 1.
'                                           Relative error
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0,5         10000       6.9e-15     4.5e-16
'       IEEE      0,85       250000       2.2e-13     1.7e-14
'       IEEE      0,1000      30000       5.3e-12     6.3e-13
'       IEEE      0,10000    250000       9.3e-11     7.1e-12
'       IEEE      0,100000    10000       8.7e-10     4.8e-11
'    Outputs smaller than the IEEE gradual underflow threshold
'    were excluded from these statistics.
'
'    Cephes Math Library, Release 2.8:  June, 2000
'    Copyright 1984, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function incompletebeta(a As Double, b As Double, x As Double) As Double

		Dim result As Double = ibetaf.incompletebeta(a, b, x)
		Return result
	End Function

	'************************************************************************
'    Inverse of imcomplete beta integral
'
'    Given y, the function finds x such that
'
'     incbet( a, b, x ) = y .
'
'    The routine performs interval halving or Newton iterations to find the
'    root of incbet(a,b,x) - y = 0.
'
'
'    ACCURACY:
'
'                         Relative error:
'                   x     a,b
'    arithmetic   domain  domain  # trials    peak       rms
'       IEEE      0,1    .5,10000   50000    5.8e-12   1.3e-13
'       IEEE      0,1   .25,100    100000    1.8e-13   3.9e-15
'       IEEE      0,1     0,5       50000    1.1e-12   5.5e-15
'    With a and b constrained to half-integer or integer values:
'       IEEE      0,1    .5,10000   50000    5.8e-12   1.1e-13
'       IEEE      0,1    .5,100    100000    1.7e-14   7.9e-16
'    With a = .5, b constrained to half-integer or integer values:
'       IEEE      0,1    .5,10000   10000    8.3e-11   1.0e-11
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1996, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function invincompletebeta(a As Double, b As Double, y As Double) As Double

		Dim result As Double = ibetaf.invincompletebeta(a, b, y)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Binomial distribution
'
'    Returns the sum of the terms 0 through k of the Binomial
'    probability density:
'
'      k
'      --  ( n )   j      n-j
'      >   (   )  p  (1-p)
'      --  ( j )
'     j=0
'
'    The terms are not summed directly; instead the incomplete
'    beta integral is employed, according to the formula
'
'    y = bdtr( k, n, p ) = incbet( n-k, k+1, 1-p ).
'
'    The arguments must be positive, with p ranging from 0 to 1.
'
'    ACCURACY:
'
'    Tested at random points (a,b,p), with p between 0 and 1.
'
'                  a,b                     Relative error:
'    arithmetic  domain     # trials      peak         rms
'     For p between 0.001 and 1:
'       IEEE     0,100       100000      4.3e-15     2.6e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function binomialdistribution(k As Integer, n As Integer, p As Double) As Double

		Dim result As Double = binomialdistr.binomialdistribution(k, n, p)
		Return result
	End Function

	'************************************************************************
'    Complemented binomial distribution
'
'    Returns the sum of the terms k+1 through n of the Binomial
'    probability density:
'
'      n
'      --  ( n )   j      n-j
'      >   (   )  p  (1-p)
'      --  ( j )
'     j=k+1
'
'    The terms are not summed directly; instead the incomplete
'    beta integral is employed, according to the formula
'
'    y = bdtrc( k, n, p ) = incbet( k+1, n-k, p ).
'
'    The arguments must be positive, with p ranging from 0 to 1.
'
'    ACCURACY:
'
'    Tested at random points (a,b,p).
'
'                  a,b                     Relative error:
'    arithmetic  domain     # trials      peak         rms
'     For p between 0.001 and 1:
'       IEEE     0,100       100000      6.7e-15     8.2e-16
'     For p between 0 and .001:
'       IEEE     0,100       100000      1.5e-13     2.7e-15
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function binomialcdistribution(k As Integer, n As Integer, p As Double) As Double

		Dim result As Double = binomialdistr.binomialcdistribution(k, n, p)
		Return result
	End Function

	'************************************************************************
'    Inverse binomial distribution
'
'    Finds the event probability p such that the sum of the
'    terms 0 through k of the Binomial probability density
'    is equal to the given cumulative probability y.
'
'    This is accomplished using the inverse beta integral
'    function and the relation
'
'    1 - p = incbi( n-k, k+1, y ).
'
'    ACCURACY:
'
'    Tested at random points (a,b,p).
'
'                  a,b                     Relative error:
'    arithmetic  domain     # trials      peak         rms
'     For p between 0.001 and 1:
'       IEEE     0,100       100000      2.3e-14     6.4e-16
'       IEEE     0,10000     100000      6.6e-12     1.2e-13
'     For p between 10^-6 and 0.001:
'       IEEE     0,100       100000      2.0e-12     1.3e-14
'       IEEE     0,10000     100000      1.5e-12     3.2e-14
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function invbinomialdistribution(k As Integer, n As Integer, y As Double) As Double

		Dim result As Double = binomialdistr.invbinomialdistribution(k, n, y)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Calculation of the value of the Chebyshev polynomials of the
'    first and second kinds.
'
'    Parameters:
'        r   -   polynomial kind, either 1 or 2.
'        n   -   degree, n>=0
'        x   -   argument, -1 <= x <= 1
'
'    Result:
'        the value of the Chebyshev polynomial at x
'    ************************************************************************

	Public Shared Function chebyshevcalculate(r As Integer, n As Integer, x As Double) As Double

		Dim result As Double = chebyshev.chebyshevcalculate(r, n, x)
		Return result
	End Function

	'************************************************************************
'    Summation of Chebyshev polynomials using Clenshaw抯 recurrence formula.
'
'    This routine calculates
'        c[0]*T0(x) + c[1]*T1(x) + ... + c[N]*TN(x)
'    or
'        c[0]*U0(x) + c[1]*U1(x) + ... + c[N]*UN(x)
'    depending on the R.
'
'    Parameters:
'        r   -   polynomial kind, either 1 or 2.
'        n   -   degree, n>=0
'        x   -   argument
'
'    Result:
'        the value of the Chebyshev polynomial at x
'    ************************************************************************

	Public Shared Function chebyshevsum(c As Double(), r As Integer, n As Integer, x As Double) As Double

		Dim result As Double = chebyshev.chebyshevsum(c, r, n, x)
		Return result
	End Function

	'************************************************************************
'    Representation of Tn as C[0] + C[1]*X + ... + C[N]*X^N
'
'    Input parameters:
'        N   -   polynomial degree, n>=0
'
'    Output parameters:
'        C   -   coefficients
'    ************************************************************************

	Public Shared Sub chebyshevcoefficients(n As Integer, ByRef c As Double())
		c = New Double(-1) {}
		chebyshev.chebyshevcoefficients(n, c)
		Return
	End Sub

	'************************************************************************
'    Conversion of a series of Chebyshev polynomials to a power series.
'
'    Represents A[0]*T0(x) + A[1]*T1(x) + ... + A[N]*Tn(x) as
'    B[0] + B[1]*X + ... + B[N]*X^N.
'
'    Input parameters:
'        A   -   Chebyshev series coefficients
'        N   -   degree, N>=0
'
'    Output parameters
'        B   -   power series coefficients
'    ************************************************************************

	Public Shared Sub fromchebyshev(a As Double(), n As Integer, ByRef b As Double())
		b = New Double(-1) {}
		chebyshev.fromchebyshev(a, n, b)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Chi-square distribution
'
'    Returns the area under the left hand tail (from 0 to x)
'    of the Chi square probability density function with
'    v degrees of freedom.
'
'
'                                      x
'                                       -
'                           1          | |  v/2-1  -t/2
'     P( x | v )   =   -----------     |   t      e     dt
'                       v/2  -       | |
'                      2    | (v/2)   -
'                                      0
'
'    where x is the Chi-square variable.
'
'    The incomplete gamma integral is used, according to the
'    formula
'
'    y = chdtr( v, x ) = igam( v/2.0, x/2.0 ).
'
'    The arguments must both be positive.
'
'    ACCURACY:
'
'    See incomplete gamma function
'
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function chisquaredistribution(v As Double, x As Double) As Double

		Dim result As Double = chisquaredistr.chisquaredistribution(v, x)
		Return result
	End Function

	'************************************************************************
'    Complemented Chi-square distribution
'
'    Returns the area under the right hand tail (from x to
'    infinity) of the Chi square probability density function
'    with v degrees of freedom:
'
'                                     inf.
'                                       -
'                           1          | |  v/2-1  -t/2
'     P( x | v )   =   -----------     |   t      e     dt
'                       v/2  -       | |
'                      2    | (v/2)   -
'                                      x
'
'    where x is the Chi-square variable.
'
'    The incomplete gamma integral is used, according to the
'    formula
'
'    y = chdtr( v, x ) = igamc( v/2.0, x/2.0 ).
'
'    The arguments must both be positive.
'
'    ACCURACY:
'
'    See incomplete gamma function
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function chisquarecdistribution(v As Double, x As Double) As Double

		Dim result As Double = chisquaredistr.chisquarecdistribution(v, x)
		Return result
	End Function

	'************************************************************************
'    Inverse of complemented Chi-square distribution
'
'    Finds the Chi-square argument x such that the integral
'    from x to infinity of the Chi-square density is equal
'    to the given cumulative probability y.
'
'    This is accomplished using the inverse gamma integral
'    function and the relation
'
'       x/2 = igami( df/2, y );
'
'    ACCURACY:
'
'    See inverse incomplete gamma function
'
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function invchisquaredistribution(v As Double, y As Double) As Double

		Dim result As Double = chisquaredistr.invchisquaredistribution(v, y)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Dawson's Integral
'
'    Approximates the integral
'
'                                x
'                                -
'                         2     | |        2
'     dawsn(x)  =  exp( -x  )   |    exp( t  ) dt
'                             | |
'                              -
'                              0
'
'    Three different rational approximations are employed, for
'    the intervals 0 to 3.25; 3.25 to 6.25; and 6.25 up.
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0,10        10000       6.9e-16     1.0e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function dawsonintegral(x As Double) As Double

		Dim result As Double = dawson.dawsonintegral(x)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Complete elliptic integral of the first kind
'
'    Approximates the integral
'
'
'
'               pi/2
'                -
'               | |
'               |           dt
'    K(m)  =    |    ------------------
'               |                   2
'             | |    sqrt( 1 - m sin t )
'              -
'               0
'
'    using the approximation
'
'        P(x)  -  log x Q(x).
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE       0,1        30000       2.5e-16     6.8e-17
'
'    Cephes Math Library, Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function ellipticintegralk(m As Double) As Double

		Dim result As Double = elliptic.ellipticintegralk(m)
		Return result
	End Function

	'************************************************************************
'    Complete elliptic integral of the first kind
'
'    Approximates the integral
'
'
'
'               pi/2
'                -
'               | |
'               |           dt
'    K(m)  =    |    ------------------
'               |                   2
'             | |    sqrt( 1 - m sin t )
'              -
'               0
'
'    where m = 1 - m1, using the approximation
'
'        P(x)  -  log x Q(x).
'
'    The argument m1 is used rather than m so that the logarithmic
'    singularity at m = 1 will be shifted to the origin; this
'    preserves maximum accuracy.
'
'    K(0) = pi/2.
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE       0,1        30000       2.5e-16     6.8e-17
'
'    Cephes Math Library, Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function ellipticintegralkhighprecision(m1 As Double) As Double

		Dim result As Double = elliptic.ellipticintegralkhighprecision(m1)
		Return result
	End Function

	'************************************************************************
'    Incomplete elliptic integral of the first kind F(phi|m)
'
'    Approximates the integral
'
'
'
'                   phi
'                    -
'                   | |
'                   |           dt
'    F(phi_\m)  =    |    ------------------
'                   |                   2
'                 | |    sqrt( 1 - m sin t )
'                  -
'                   0
'
'    of amplitude phi and modulus m, using the arithmetic -
'    geometric mean algorithm.
'
'
'
'
'    ACCURACY:
'
'    Tested at random points with m in [0, 1] and phi as indicated.
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE     -10,10       200000      7.4e-16     1.0e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function incompleteellipticintegralk(phi As Double, m As Double) As Double

		Dim result As Double = elliptic.incompleteellipticintegralk(phi, m)
		Return result
	End Function

	'************************************************************************
'    Complete elliptic integral of the second kind
'
'    Approximates the integral
'
'
'               pi/2
'                -
'               | |                 2
'    E(m)  =    |    sqrt( 1 - m sin t ) dt
'             | |
'              -
'               0
'
'    using the approximation
'
'         P(x)  -  x log x Q(x).
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE       0, 1       10000       2.1e-16     7.3e-17
'
'    Cephes Math Library, Release 2.8: June, 2000
'    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function ellipticintegrale(m As Double) As Double

		Dim result As Double = elliptic.ellipticintegrale(m)
		Return result
	End Function

	'************************************************************************
'    Incomplete elliptic integral of the second kind
'
'    Approximates the integral
'
'
'                   phi
'                    -
'                   | |
'                   |                   2
'    E(phi_\m)  =    |    sqrt( 1 - m sin t ) dt
'                   |
'                 | |
'                  -
'                   0
'
'    of amplitude phi and modulus m, using the arithmetic -
'    geometric mean algorithm.
'
'    ACCURACY:
'
'    Tested at random arguments with phi in [-10, 10] and m in
'    [0, 1].
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE     -10,10      150000       3.3e-15     1.4e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1993, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function incompleteellipticintegrale(phi As Double, m As Double) As Double

		Dim result As Double = elliptic.incompleteellipticintegrale(phi, m)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Exponential integral Ei(x)
'
'                  x
'                   -     t
'                  | |   e
'       Ei(x) =   -|-   ---  dt .
'                | |     t
'                 -
'                -inf
'
'    Not defined for x <= 0.
'    See also expn.c.
'
'
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE       0,100       50000      8.6e-16     1.3e-16
'
'    Cephes Math Library Release 2.8:  May, 1999
'    Copyright 1999 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function exponentialintegralei(x As Double) As Double

		Dim result As Double = expintegrals.exponentialintegralei(x)
		Return result
	End Function

	'************************************************************************
'    Exponential integral En(x)
'
'    Evaluates the exponential integral
'
'                    inf.
'                      -
'                     | |   -xt
'                     |    e
'         E (x)  =    |    ----  dt.
'          n          |      n
'                   | |     t
'                    -
'                     1
'
'
'    Both n and x must be nonnegative.
'
'    The routine employs either a power series, a continued
'    fraction, or an asymptotic formula depending on the
'    relative values of n and x.
'
'    ACCURACY:
'
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0, 30       10000       1.7e-15     3.6e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1985, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function exponentialintegralen(x As Double, n As Integer) As Double

		Dim result As Double = expintegrals.exponentialintegralen(x, n)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    F distribution
'
'    Returns the area from zero to x under the F density
'    function (also known as Snedcor's density or the
'    variance ratio density).  This is the density
'    of x = (u1/df1)/(u2/df2), where u1 and u2 are random
'    variables having Chi square distributions with df1
'    and df2 degrees of freedom, respectively.
'    The incomplete beta integral is used, according to the
'    formula
'
'    P(x) = incbet( df1/2, df2/2, (df1*x/(df2 + df1*x) ).
'
'
'    The arguments a and b are greater than zero, and x is
'    nonnegative.
'
'    ACCURACY:
'
'    Tested at random points (a,b,x).
'
'                   x     a,b                     Relative error:
'    arithmetic  domain  domain     # trials      peak         rms
'       IEEE      0,1    0,100       100000      9.8e-15     1.7e-15
'       IEEE      1,5    0,100       100000      6.5e-15     3.5e-16
'       IEEE      0,1    1,10000     100000      2.2e-11     3.3e-12
'       IEEE      1,5    1,10000     100000      1.1e-11     1.7e-13
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function fdistribution(a As Integer, b As Integer, x As Double) As Double

		Dim result As Double = fdistr.fdistribution(a, b, x)
		Return result
	End Function

	'************************************************************************
'    Complemented F distribution
'
'    Returns the area from x to infinity under the F density
'    function (also known as Snedcor's density or the
'    variance ratio density).
'
'
'                         inf.
'                          -
'                 1       | |  a-1      b-1
'    1-P(x)  =  ------    |   t    (1-t)    dt
'               B(a,b)  | |
'                        -
'                         x
'
'
'    The incomplete beta integral is used, according to the
'    formula
'
'    P(x) = incbet( df2/2, df1/2, (df2/(df2 + df1*x) ).
'
'
'    ACCURACY:
'
'    Tested at random points (a,b,x) in the indicated intervals.
'                   x     a,b                     Relative error:
'    arithmetic  domain  domain     # trials      peak         rms
'       IEEE      0,1    1,100       100000      3.7e-14     5.9e-16
'       IEEE      1,5    1,100       100000      8.0e-15     1.6e-15
'       IEEE      0,1    1,10000     100000      1.8e-11     3.5e-13
'       IEEE      1,5    1,10000     100000      2.0e-11     3.0e-12
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function fcdistribution(a As Integer, b As Integer, x As Double) As Double

		Dim result As Double = fdistr.fcdistribution(a, b, x)
		Return result
	End Function

	'************************************************************************
'    Inverse of complemented F distribution
'
'    Finds the F density argument x such that the integral
'    from x to infinity of the F density is equal to the
'    given probability p.
'
'    This is accomplished using the inverse beta integral
'    function and the relations
'
'         z = incbi( df2/2, df1/2, p )
'         x = df2 (1-z) / (df1 z).
'
'    Note: the following relations hold for the inverse of
'    the uncomplemented F distribution:
'
'         z = incbi( df1/2, df2/2, p )
'         x = df2 z / (df1 (1-z)).
'
'    ACCURACY:
'
'    Tested at random points (a,b,p).
'
'                 a,b                     Relative error:
'    arithmetic  domain     # trials      peak         rms
'     For p between .001 and 1:
'       IEEE     1,100       100000      8.3e-15     4.7e-16
'       IEEE     1,10000     100000      2.1e-11     1.4e-13
'     For p between 10^-6 and 10^-3:
'       IEEE     1,100        50000      1.3e-12     8.4e-15
'       IEEE     1,10000      50000      3.0e-12     4.8e-14
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function invfdistribution(a As Integer, b As Integer, y As Double) As Double

		Dim result As Double = fdistr.invfdistribution(a, b, y)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Fresnel integral
'
'    Evaluates the Fresnel integrals
'
'              x
'              -
'             | |
'    C(x) =   |   cos(pi/2 t**2) dt,
'           | |
'            -
'             0
'
'              x
'              -
'             | |
'    S(x) =   |   sin(pi/2 t**2) dt.
'           | |
'            -
'             0
'
'
'    The integrals are evaluated by a power series for x < 1.
'    For x >= 1 auxiliary functions f(x) and g(x) are employed
'    such that
'
'    C(x) = 0.5 + f(x) sin( pi/2 x**2 ) - g(x) cos( pi/2 x**2 )
'    S(x) = 0.5 - f(x) cos( pi/2 x**2 ) - g(x) sin( pi/2 x**2 )
'
'
'
'    ACCURACY:
'
'     Relative error.
'
'    Arithmetic  function   domain     # trials      peak         rms
'      IEEE       S(x)      0, 10       10000       2.0e-15     3.2e-16
'      IEEE       C(x)      0, 10       10000       1.8e-15     3.3e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Sub fresnelintegral(x As Double, ByRef c As Double, ByRef s As Double)

		fresnel.fresnelintegral(x, c, s)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Calculation of the value of the Hermite polynomial.
'
'    Parameters:
'        n   -   degree, n>=0
'        x   -   argument
'
'    Result:
'        the value of the Hermite polynomial Hn at x
'    ************************************************************************

	Public Shared Function hermitecalculate(n As Integer, x As Double) As Double

		Dim result As Double = hermite.hermitecalculate(n, x)
		Return result
	End Function

	'************************************************************************
'    Summation of Hermite polynomials using Clenshaw抯 recurrence formula.
'
'    This routine calculates
'        c[0]*H0(x) + c[1]*H1(x) + ... + c[N]*HN(x)
'
'    Parameters:
'        n   -   degree, n>=0
'        x   -   argument
'
'    Result:
'        the value of the Hermite polynomial at x
'    ************************************************************************

	Public Shared Function hermitesum(c As Double(), n As Integer, x As Double) As Double

		Dim result As Double = hermite.hermitesum(c, n, x)
		Return result
	End Function

	'************************************************************************
'    Representation of Hn as C[0] + C[1]*X + ... + C[N]*X^N
'
'    Input parameters:
'        N   -   polynomial degree, n>=0
'
'    Output parameters:
'        C   -   coefficients
'    ************************************************************************

	Public Shared Sub hermitecoefficients(n As Integer, ByRef c As Double())
		c = New Double(-1) {}
		hermite.hermitecoefficients(n, c)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Jacobian Elliptic Functions
'
'    Evaluates the Jacobian elliptic functions sn(u|m), cn(u|m),
'    and dn(u|m) of parameter m between 0 and 1, and real
'    argument u.
'
'    These functions are periodic, with quarter-period on the
'    real axis equal to the complete elliptic integral
'    ellpk(1.0-m).
'
'    Relation to incomplete elliptic integral:
'    If u = ellik(phi,m), then sn(u|m) = sin(phi),
'    and cn(u|m) = cos(phi).  Phi is called the amplitude of u.
'
'    Computation is by means of the arithmetic-geometric mean
'    algorithm, except when m is within 1e-9 of 0 or 1.  In the
'    latter case with m close to 1, the approximation applies
'    only for phi < pi/2.
'
'    ACCURACY:
'
'    Tested at random points with u between 0 and 10, m between
'    0 and 1.
'
'               Absolute error (* = relative error):
'    arithmetic   function   # trials      peak         rms
'       IEEE      phi         10000       9.2e-16*    1.4e-16*
'       IEEE      sn          50000       4.1e-15     4.6e-16
'       IEEE      cn          40000       3.6e-15     4.4e-16
'       IEEE      dn          10000       1.3e-12     1.8e-14
'
'     Peak error observed in consistency check using addition
'    theorem for sn(u+v) was 4e-16 (absolute).  Also tested by
'    the above relation to the incomplete elliptic integral.
'    Accuracy deteriorates when u is large.
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Sub jacobianellipticfunctions(u As Double, m As Double, ByRef sn As Double, ByRef cn As Double, ByRef dn As Double, ByRef ph As Double)
		sn = 0
		cn = 0
		dn = 0
		ph = 0
		jacobianelliptic.jacobianellipticfunctions(u, m, sn, cn, dn, ph)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Calculation of the value of the Laguerre polynomial.
'
'    Parameters:
'        n   -   degree, n>=0
'        x   -   argument
'
'    Result:
'        the value of the Laguerre polynomial Ln at x
'    ************************************************************************

	Public Shared Function laguerrecalculate(n As Integer, x As Double) As Double

		Dim result As Double = laguerre.laguerrecalculate(n, x)
		Return result
	End Function

	'************************************************************************
'    Summation of Laguerre polynomials using Clenshaw抯 recurrence formula.
'
'    This routine calculates c[0]*L0(x) + c[1]*L1(x) + ... + c[N]*LN(x)
'
'    Parameters:
'        n   -   degree, n>=0
'        x   -   argument
'
'    Result:
'        the value of the Laguerre polynomial at x
'    ************************************************************************

	Public Shared Function laguerresum(c As Double(), n As Integer, x As Double) As Double

		Dim result As Double = laguerre.laguerresum(c, n, x)
		Return result
	End Function

	'************************************************************************
'    Representation of Ln as C[0] + C[1]*X + ... + C[N]*X^N
'
'    Input parameters:
'        N   -   polynomial degree, n>=0
'
'    Output parameters:
'        C   -   coefficients
'    ************************************************************************

	Public Shared Sub laguerrecoefficients(n As Integer, ByRef c As Double())
		c = New Double(-1) {}
		laguerre.laguerrecoefficients(n, c)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Calculation of the value of the Legendre polynomial Pn.
'
'    Parameters:
'        n   -   degree, n>=0
'        x   -   argument
'
'    Result:
'        the value of the Legendre polynomial Pn at x
'    ************************************************************************

	Public Shared Function legendrecalculate(n As Integer, x As Double) As Double

		Dim result As Double = legendre.legendrecalculate(n, x)
		Return result
	End Function

	'************************************************************************
'    Summation of Legendre polynomials using Clenshaw抯 recurrence formula.
'
'    This routine calculates
'        c[0]*P0(x) + c[1]*P1(x) + ... + c[N]*PN(x)
'
'    Parameters:
'        n   -   degree, n>=0
'        x   -   argument
'
'    Result:
'        the value of the Legendre polynomial at x
'    ************************************************************************

	Public Shared Function legendresum(c As Double(), n As Integer, x As Double) As Double

		Dim result As Double = legendre.legendresum(c, n, x)
		Return result
	End Function

	'************************************************************************
'    Representation of Pn as C[0] + C[1]*X + ... + C[N]*X^N
'
'    Input parameters:
'        N   -   polynomial degree, n>=0
'
'    Output parameters:
'        C   -   coefficients
'    ************************************************************************

	Public Shared Sub legendrecoefficients(n As Integer, ByRef c As Double())
		c = New Double(-1) {}
		legendre.legendrecoefficients(n, c)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Poisson distribution
'
'    Returns the sum of the first k+1 terms of the Poisson
'    distribution:
'
'      k         j
'      --   -m  m
'      >   e    --
'      --       j!
'     j=0
'
'    The terms are not summed directly; instead the incomplete
'    gamma integral is employed, according to the relation
'
'    y = pdtr( k, m ) = igamc( k+1, m ).
'
'    The arguments must both be positive.
'    ACCURACY:
'
'    See incomplete gamma function
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function poissondistribution(k As Integer, m As Double) As Double

		Dim result As Double = poissondistr.poissondistribution(k, m)
		Return result
	End Function

	'************************************************************************
'    Complemented Poisson distribution
'
'    Returns the sum of the terms k+1 to infinity of the Poisson
'    distribution:
'
'     inf.       j
'      --   -m  m
'      >   e    --
'      --       j!
'     j=k+1
'
'    The terms are not summed directly; instead the incomplete
'    gamma integral is employed, according to the formula
'
'    y = pdtrc( k, m ) = igam( k+1, m ).
'
'    The arguments must both be positive.
'
'    ACCURACY:
'
'    See incomplete gamma function
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function poissoncdistribution(k As Integer, m As Double) As Double

		Dim result As Double = poissondistr.poissoncdistribution(k, m)
		Return result
	End Function

	'************************************************************************
'    Inverse Poisson distribution
'
'    Finds the Poisson variable x such that the integral
'    from 0 to x of the Poisson density is equal to the
'    given probability y.
'
'    This is accomplished using the inverse gamma integral
'    function and the relation
'
'       m = igami( k+1, y ).
'
'    ACCURACY:
'
'    See inverse incomplete gamma function
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function invpoissondistribution(k As Integer, y As Double) As Double

		Dim result As Double = poissondistr.invpoissondistribution(k, y)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Psi (digamma) function
'
'                 d      -
'      psi(x)  =  -- ln | (x)
'                 dx
'
'    is the logarithmic derivative of the gamma function.
'    For integer x,
'                      n-1
'                       -
'    psi(n) = -EUL  +   >  1/k.
'                       -
'                      k=1
'
'    This formula is used for 0 < n <= 10.  If x is negative, it
'    is transformed to a positive argument by the reflection
'    formula  psi(1-x) = psi(x) + pi cot(pi x).
'    For general positive x, the argument is made greater than 10
'    using the recurrence  psi(x+1) = psi(x) + 1/x.
'    Then the following asymptotic expansion is applied:
'
'                              inf.   B
'                               -      2k
'    psi(x) = log(x) - 1/2x -   >   -------
'                               -        2k
'                              k=1   2k x
'
'    where the B2k are Bernoulli numbers.
'
'    ACCURACY:
'       Relative error (except absolute when |psi| < 1):
'    arithmetic   domain     # trials      peak         rms
'       IEEE      0,30        30000       1.3e-15     1.4e-16
'       IEEE      -30,0       40000       1.5e-15     2.2e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1992, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function psi(x As Double) As Double

		Dim result As Double = psif.psi(x)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Student's t distribution
'
'    Computes the integral from minus infinity to t of the Student
'    t distribution with integer k > 0 degrees of freedom:
'
'                                         t
'                                         -
'                                        | |
'                 -                      |         2   -(k+1)/2
'                | ( (k+1)/2 )           |  (     x   )
'          ----------------------        |  ( 1 + --- )        dx
'                        -               |  (      k  )
'          sqrt( k pi ) | ( k/2 )        |
'                                      | |
'                                       -
'                                      -inf.
'
'    Relation to incomplete beta integral:
'
'           1 - stdtr(k,t) = 0.5 * incbet( k/2, 1/2, z )
'    where
'           z = k/(k + t**2).
'
'    For t < -2, this is the method of computation.  For higher t,
'    a direct method is derived from integration by parts.
'    Since the function is symmetric about t=0, the area under the
'    right tail of the density is found by calling the function
'    with -t instead of t.
'
'    ACCURACY:
'
'    Tested at random 1 <= k <= 25.  The "domain" refers to t.
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE     -100,-2      50000       5.9e-15     1.4e-15
'       IEEE     -2,100      500000       2.7e-15     4.9e-17
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function studenttdistribution(k As Integer, t As Double) As Double

		Dim result As Double = studenttdistr.studenttdistribution(k, t)
		Return result
	End Function

	'************************************************************************
'    Functional inverse of Student's t distribution
'
'    Given probability p, finds the argument t such that stdtr(k,t)
'    is equal to p.
'
'    ACCURACY:
'
'    Tested at random 1 <= k <= 100.  The "domain" refers to p:
'                         Relative error:
'    arithmetic   domain     # trials      peak         rms
'       IEEE    .001,.999     25000       5.7e-15     8.0e-16
'       IEEE    10^-6,.001    25000       2.0e-12     2.9e-14
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Function invstudenttdistribution(k As Integer, p As Double) As Double

		Dim result As Double = studenttdistr.invstudenttdistribution(k, p)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'    Sine and cosine integrals
'
'    Evaluates the integrals
'
'                             x
'                             -
'                            |  cos t - 1
'      Ci(x) = eul + ln x +  |  --------- dt,
'                            |      t
'                           -
'                            0
'                x
'                -
'               |  sin t
'      Si(x) =  |  ----- dt
'               |    t
'              -
'               0
'
'    where eul = 0.57721566490153286061 is Euler's constant.
'    The integrals are approximated by rational functions.
'    For x > 8 auxiliary functions f(x) and g(x) are employed
'    such that
'
'    Ci(x) = f(x) sin(x) - g(x) cos(x)
'    Si(x) = pi/2 - f(x) cos(x) - g(x) sin(x)
'
'
'    ACCURACY:
'       Test interval = [0,50].
'    Absolute error, except relative when > 1:
'    arithmetic   function   # trials      peak         rms
'       IEEE        Si        30000       4.4e-16     7.3e-17
'       IEEE        Ci        30000       6.9e-16     5.1e-17
'
'    Cephes Math Library Release 2.1:  January, 1989
'    Copyright 1984, 1987, 1989 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Sub sinecosineintegrals(x As Double, ByRef si As Double, ByRef ci As Double)
		si = 0
		ci = 0
		trigintegrals.sinecosineintegrals(x, si, ci)
		Return
	End Sub

	'************************************************************************
'    Hyperbolic sine and cosine integrals
'
'    Approximates the integrals
'
'                               x
'                               -
'                              | |   cosh t - 1
'      Chi(x) = eul + ln x +   |    -----------  dt,
'                            | |          t
'                             -
'                             0
'
'                  x
'                  -
'                 | |  sinh t
'      Shi(x) =   |    ------  dt
'               | |       t
'                -
'                0
'
'    where eul = 0.57721566490153286061 is Euler's constant.
'    The integrals are evaluated by power series for x < 8
'    and by Chebyshev expansions for x between 8 and 88.
'    For large x, both functions approach exp(x)/2x.
'    Arguments greater than 88 in magnitude return MAXNUM.
'
'
'    ACCURACY:
'
'    Test interval 0 to 88.
'                         Relative error:
'    arithmetic   function  # trials      peak         rms
'       IEEE         Shi      30000       6.9e-16     1.6e-16
'           Absolute error, except relative when |Chi| > 1:
'       IEEE         Chi      30000       8.4e-16     1.4e-16
'
'    Cephes Math Library Release 2.8:  June, 2000
'    Copyright 1984, 1987, 2000 by Stephen L. Moshier
'    ************************************************************************

	Public Shared Sub hyperbolicsinecosineintegrals(x As Double, ByRef shi As Double, ByRef chi As Double)
		shi = 0
		chi = 0
		trigintegrals.hyperbolicsinecosineintegrals(x, shi, chi)
		Return
	End Sub

End Class
Public Partial Class alglib
	Public Class gammafunc
		'************************************************************************
'        Gamma function
'
'        Input parameters:
'            X   -   argument
'
'        Domain:
'            0 < X < 171.6
'            -170 < X < 0, X is not an integer.
'
'        Relative error:
'         arithmetic   domain     # trials      peak         rms
'            IEEE    -170,-33      20000       2.3e-15     3.3e-16
'            IEEE     -33,  33     20000       9.4e-16     2.2e-16
'            IEEE      33, 171.6   20000       2.3e-15     3.2e-16
'
'        Cephes Math Library Release 2.8:  June, 2000
'        Original copyright 1984, 1987, 1989, 1992, 2000 by Stephen L. Moshier
'        Translated to AlgoPascal by Bochkanov Sergey (2005, 2006, 2007).
'        ************************************************************************

		Public Shared Function gammafunction(x As Double) As Double
			Dim result As Double = 0
			Dim p As Double = 0
			Dim pp As Double = 0
			Dim q As Double = 0
			Dim qq As Double = 0
			Dim z As Double = 0
			Dim i As Integer = 0
			Dim sgngam As Double = 0

			sgngam = 1
            q = System.Math.Abs(x)
            If CDbl(q) > CDbl(33.0) Then
                If CDbl(x) < CDbl(0.0) Then
                    p = CInt(System.Math.Truncate(System.Math.Floor(q)))
                    i = CInt(System.Math.Truncate(System.Math.Round(p)))
                    If i Mod 2 = 0 Then
                        sgngam = -1
                    End If
                    z = q - p
                    If CDbl(z) > CDbl(0.5) Then
                        p = p + 1
                        z = q - p
                    End If
                    z = q * System.Math.Sin(System.Math.PI * z)
                    z = System.Math.Abs(z)
                    z = System.Math.PI / (z * gammastirf(q))
                Else
                    z = gammastirf(x)
                End If
                result = sgngam * z
                Return result
            End If
            z = 1
            While CDbl(x) >= CDbl(3)
                x = x - 1
                z = z * x
            End While
            While CDbl(x) < CDbl(0)
                If CDbl(x) > CDbl(-0.000000001) Then
                    result = z / ((1 + 0.577215664901533 * x) * x)
                    Return result
                End If
                z = z / x
                x = x + 1
            End While
            While CDbl(x) < CDbl(2)
                If CDbl(x) < CDbl(0.000000001) Then
                    result = z / ((1 + 0.577215664901533 * x) * x)
                    Return result
                End If
                z = z / x
                x = x + 1.0
            End While
            If CDbl(x) = CDbl(2) Then
                result = z
                Return result
            End If
            x = x - 2.0
            pp = 0.000160119522476752
            pp = 0.00119135147006586 + x * pp
            pp = 0.0104213797561762 + x * pp
            pp = 0.0476367800457137 + x * pp
            pp = 0.207448227648436 + x * pp
            pp = 0.494214826801497 + x * pp
            pp = 1.0 + x * pp
            qq = -0.000023158187332412
            qq = 0.000539605580493303 + x * qq
            qq = -0.00445641913851797 + x * qq
            qq = 0.011813978522206 + x * qq
            qq = 0.0358236398605499 + x * qq
            qq = -0.234591795718243 + x * qq
            qq = 0.0714304917030273 + x * qq
            qq = 1.0 + x * qq
            result = z * pp / qq
            Return result
        End Function


        '************************************************************************
        '        Natural logarithm of gamma function
        '
        '        Input parameters:
        '            X       -   argument
        '
        '        Result:
        '            logarithm of the absolute value of the Gamma(X).
        '
        '        Output parameters:
        '            SgnGam  -   sign(Gamma(X))
        '
        '        Domain:
        '            0 < X < 2.55e305
        '            -2.55e305 < X < 0, X is not an integer.
        '
        '        ACCURACY:
        '        arithmetic      domain        # trials     peak         rms
        '           IEEE    0, 3                 28000     5.4e-16     1.1e-16
        '           IEEE    2.718, 2.556e305     40000     3.5e-16     8.3e-17
        '        The error criterion was relative when the function magnitude
        '        was greater than one but absolute when it was less than one.
        '
        '        The following test used the relative error criterion, though
        '        at certain points the relative error could be much higher than
        '        indicated.
        '           IEEE    -200, -4             10000     4.8e-16     1.3e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1989, 1992, 2000 by Stephen L. Moshier
        '        Translated to AlgoPascal by Bochkanov Sergey (2005, 2006, 2007).
        '        ************************************************************************

        Public Shared Function lngamma(x As Double, ByRef sgngam As Double) As Double
            Dim result As Double = 0
            Dim a As Double = 0
            Dim b As Double = 0
            Dim c As Double = 0
            Dim p As Double = 0
            Dim q As Double = 0
            Dim u As Double = 0
            Dim w As Double = 0
            Dim z As Double = 0
            Dim i As Integer = 0
            Dim logpi As Double = 0
            Dim ls2pi As Double = 0
            Dim tmp As Double = 0

            sgngam = 0

            sgngam = 1
            logpi = 1.1447298858494
            ls2pi = 0.918938533204673
            If CDbl(x) < CDbl(-34.0) Then
                q = -x
                w = lngamma(q, tmp)
                p = CInt(System.Math.Truncate(System.Math.Floor(q)))
                i = CInt(System.Math.Truncate(System.Math.Round(p)))
                If i Mod 2 = 0 Then
                    sgngam = -1
                Else
                    sgngam = 1
                End If
                z = q - p
                If CDbl(z) > CDbl(0.5) Then
                    p = p + 1
                    z = p - q
                End If
                z = q * System.Math.Sin(System.Math.PI * z)
                result = logpi - System.Math.Log(z) - w
                Return result
            End If
            If CDbl(x) < CDbl(13) Then
                z = 1
                p = 0
                u = x
                While CDbl(u) >= CDbl(3)
                    p = p - 1
                    u = x + p
                    z = z * u
                End While
                While CDbl(u) < CDbl(2)
                    z = z / u
                    p = p + 1
                    u = x + p
                End While
                If CDbl(z) < CDbl(0) Then
                    sgngam = -1
                    z = -z
                Else
                    sgngam = 1
                End If
                If CDbl(u) = CDbl(2) Then
                    result = System.Math.Log(z)
                    Return result
                End If
                p = p - 2
                x = x + p
                b = -1378.25152569121
                b = -38801.6315134638 + x * b
                b = -331612.992738871 + x * b
                b = -1162370.97492762 + x * b
                b = -1721737.0082084 + x * b
                b = -853555.664245765 + x * b
                c = 1
                c = -351.815701436523 + x * c
                c = -17064.2106651881 + x * c
                c = -220528.590553854 + x * c
                c = -1139334.44367983 + x * c
                c = -2532523.07177583 + x * c
                c = -2018891.41433533 + x * c
                p = x * b / c
                result = System.Math.Log(z) + p
                Return result
            End If
            q = (x - 0.5) * System.Math.Log(x) - x + ls2pi
            If CDbl(x) > CDbl(100000000) Then
                result = q
                Return result
            End If
            p = 1 / (x * x)
            If CDbl(x) >= CDbl(1000.0) Then
                q = q + ((7.93650793650794 * 0.0001 * p - 2.77777777777778 * 0.001) * p + 0.0833333333333333) / x
            Else
                a = 8.11614167470508 * 0.0001
                a = -(5.95061904284301 * 0.0001) + p * a
                a = 7.93650340457717 * 0.0001 + p * a
                a = -(2.777777777301 * 0.001) + p * a
                a = 8.33333333333332 * 0.01 + p * a
                q = q + a / x
            End If
            result = q
            Return result
        End Function


        Private Shared Function gammastirf(x As Double) As Double
            Dim result As Double = 0
            Dim y As Double = 0
            Dim w As Double = 0
            Dim v As Double = 0
            Dim stir As Double = 0

            w = 1 / x
            stir = 0.000787311395793094
            stir = -0.000229549961613378 + w * stir
            stir = -0.00268132617805781 + w * stir
            stir = 0.00347222221605459 + w * stir
            stir = 0.0833333333333482 + w * stir
            w = 1 + w * stir
            y = System.Math.Exp(x)
            If CDbl(x) > CDbl(143.01608) Then
                v = System.Math.Pow(x, 0.5 * x - 0.25)
                y = v * (v / y)
            Else
                y = System.Math.Pow(x, x - 0.5) / y
            End If
            result = 2.506628274631 * y * w
            Return result
        End Function


    End Class
    Public Class normaldistr
        '************************************************************************
        '        Error function
        '
        '        The integral is
        '
        '                                  x
        '                                   -
        '                        2         | |          2
        '          erf(x)  =  --------     |    exp( - t  ) dt.
        '                     sqrt(pi)   | |
        '                                 -
        '                                  0
        '
        '        For 0 <= |x| < 1, erf(x) = x * P4(x**2)/Q5(x**2); otherwise
        '        erf(x) = 1 - erfc(x).
        '
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0,1         30000       3.7e-16     1.0e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function errorfunction(x As Double) As Double
            Dim result As Double = 0
            Dim xsq As Double = 0
            Dim s As Double = 0
            Dim p As Double = 0
            Dim q As Double = 0

            s = System.Math.Sign(x)
            x = System.Math.Abs(x)
            If CDbl(x) < CDbl(0.5) Then
                xsq = x * x
                p = 0.00754772803341863
                p = -0.288805137207594 + xsq * p
                p = 14.3383842191748 + xsq * p
                p = 38.0140318123903 + xsq * p
                p = 3017.82788536508 + xsq * p
                p = 7404.07142710151 + xsq * p
                p = 80437.363096084 + xsq * p
                q = 0.0
                q = 1.0 + xsq * q
                q = 38.0190713951939 + xsq * q
                q = 658.07015545924 + xsq * q
                q = 6379.60017324428 + xsq * q
                q = 34216.5257924629 + xsq * q
                q = 80437.363096084 + xsq * q
                result = s * 1.12837916709551 * x * p / q
                Return result
            End If
            If CDbl(x) >= CDbl(10) Then
                result = s
                Return result
            End If
            result = s * (1 - errorfunctionc(x))
            Return result
        End Function


        '************************************************************************
        '        Complementary error function
        '
        '         1 - erf(x) =
        '
        '                                  inf.
        '                                    -
        '                         2         | |          2
        '          erfc(x)  =  --------     |    exp( - t  ) dt
        '                      sqrt(pi)   | |
        '                                  -
        '                                   x
        '
        '
        '        For small x, erfc(x) = 1 - erf(x); otherwise rational
        '        approximations are computed.
        '
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0,26.6417   30000       5.7e-14     1.5e-14
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function errorfunctionc(x As Double) As Double
            Dim result As Double = 0
            Dim p As Double = 0
            Dim q As Double = 0

            If CDbl(x) < CDbl(0) Then
                result = 2 - errorfunctionc(-x)
                Return result
            End If
            If CDbl(x) < CDbl(0.5) Then
                result = 1.0 - errorfunction(x)
                Return result
            End If
            If CDbl(x) >= CDbl(10) Then
                result = 0
                Return result
            End If
            p = 0.0
            p = 0.56418778255074 + x * p
            p = 9.67580788298727 + x * p
            p = 77.0816173036843 + x * p
            p = 368.519615471001 + x * p
            p = 1143.26207070389 + x * p
            p = 2320.43959025164 + x * p
            p = 2898.02932921677 + x * p
            p = 1826.33488422951 + x * p
            q = 1.0
            q = 17.1498094362761 + x * q
            q = 137.125596050062 + x * q
            q = 661.736120710765 + x * q
            q = 2094.38436778954 + x * q
            q = 4429.61280388368 + x * q
            q = 6089.54242327244 + x * q
            q = 4958.82756472114 + x * q
            q = 1826.33488422951 + x * q
            result = System.Math.Exp(-Math.sqr(x)) * p / q
            Return result
        End Function


        '************************************************************************
        '        Normal distribution function
        '
        '        Returns the area under the Gaussian probability density
        '        function, integrated from minus infinity to x:
        '
        '                                   x
        '                                    -
        '                          1        | |          2
        '           ndtr(x)  = ---------    |    exp( - t /2 ) dt
        '                      sqrt(2pi)  | |
        '                                  -
        '                                 -inf.
        '
        '                    =  ( 1 + erf(z) ) / 2
        '                    =  erfc(z) / 2
        '
        '        where z = x/sqrt(2). Computation is via the functions
        '        erf and erfc.
        '
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE     -13,0        30000       3.4e-14     6.7e-15
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function normaldistribution(x As Double) As Double
            Dim result As Double = 0

            result = 0.5 * (errorfunction(x / 1.4142135623731) + 1)
            Return result
        End Function


        '************************************************************************
        '        Inverse of the error function
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function inverf(e As Double) As Double
            Dim result As Double = 0

            result = invnormaldistribution(0.5 * (e + 1)) / System.Math.sqrt(2)
            Return result
        End Function


        '************************************************************************
        '        Inverse of Normal distribution function
        '
        '        Returns the argument, x, for which the area under the
        '        Gaussian probability density function (integrated from
        '        minus infinity to x) is equal to y.
        '
        '
        '        For small arguments 0 < y < exp(-2), the program computes
        '        z = sqrt( -2.0 * log(y) );  then the approximation is
        '        x = z - log(z)/z  - (1/z) P(1/z) / Q(1/z).
        '        There are two rational functions P/Q, one for 0 < y < exp(-32)
        '        and the other for y up to exp(-2).  For larger arguments,
        '        w = y - 0.5, and  x/sqrt(2pi) = w + w**3 R(w**2)/S(w**2)).
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain        # trials      peak         rms
        '           IEEE     0.125, 1        20000       7.2e-16     1.3e-16
        '           IEEE     3e-308, 0.135   50000       4.6e-16     9.8e-17
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function invnormaldistribution(y0 As Double) As Double
            Dim result As Double = 0
            Dim expm2 As Double = 0
            Dim s2pi As Double = 0
            Dim x As Double = 0
            Dim y As Double = 0
            Dim z As Double = 0
            Dim y2 As Double = 0
            Dim x0 As Double = 0
            Dim x1 As Double = 0
            Dim code As Integer = 0
            Dim p0 As Double = 0
            Dim q0 As Double = 0
            Dim p1 As Double = 0
            Dim q1 As Double = 0
            Dim p2 As Double = 0
            Dim q2 As Double = 0

            expm2 = 0.135335283236613
            s2pi = 2.506628274631
            If CDbl(y0) <= CDbl(0) Then
                result = -Math.maxrealnumber
                Return result
            End If
            If CDbl(y0) >= CDbl(1) Then
                result = Math.maxrealnumber
                Return result
            End If
            code = 1
            y = y0
            If CDbl(y) > CDbl(1.0 - expm2) Then
                y = 1.0 - y
                code = 0
            End If
            If CDbl(y) > CDbl(expm2) Then
                y = y - 0.5
                y2 = y * y
                p0 = -59.9633501014108
                p0 = 98.0010754186 + y2 * p0
                p0 = -56.676285746907 + y2 * p0
                p0 = 13.931260938728 + y2 * p0
                p0 = -1.23916583867381 + y2 * p0
                q0 = 1
                q0 = 1.95448858338142 + y2 * q0
                q0 = 4.67627912898882 + y2 * q0
                q0 = 86.3602421390891 + y2 * q0
                q0 = -225.462687854119 + y2 * q0
                q0 = 200.260212380061 + y2 * q0
                q0 = -82.0372256168333 + y2 * q0
                q0 = 15.9056225126212 + y2 * q0
                q0 = -1.1833162112133 + y2 * q0
                x = y + y * y2 * p0 / q0
                x = x * s2pi
                result = x
                Return result
            End If
            x = System.Math.sqrt(-(2.0 * System.Math.Log(y)))
            x0 = x - System.Math.Log(x) / x
            z = 1.0 / x
            If CDbl(x) < CDbl(8.0) Then
                p1 = 4.05544892305962
                p1 = 31.5251094599894 + z * p1
                p1 = 57.1628192246421 + z * p1
                p1 = 44.0805073893201 + z * p1
                p1 = 14.6849561928858 + z * p1
                p1 = 2.1866330685079 + z * p1
                p1 = -(1.40256079171354 * 0.1) + z * p1
                p1 = -(3.50424626827848 * 0.01) + z * p1
                p1 = -(8.57456785154685 * 0.0001) + z * p1
                q1 = 1
                q1 = 15.7799883256467 + z * q1
                q1 = 45.3907635128879 + z * q1
                q1 = 41.3172038254672 + z * q1
                q1 = 15.0425385692908 + z * q1
                q1 = 2.50464946208309 + z * q1
                q1 = -(1.42182922854788 * 0.1) + z * q1
                q1 = -(3.80806407691578 * 0.01) + z * q1
                q1 = -(9.33259480895457 * 0.0001) + z * q1
                x1 = z * p1 / q1
            Else
                p2 = 3.23774891776946
                p2 = 6.91522889068984 + z * p2
                p2 = 3.93881025292474 + z * p2
                p2 = 1.33303460815808 + z * p2
                p2 = 2.01485389549179 * 0.1 + z * p2
                p2 = 1.2371663481782 * 0.01 + z * p2
                p2 = 3.01581553508235 * 0.0001 + z * p2
                p2 = 2.65806974686738 * 0.000001 + z * p2
                p2 = 6.23974539184983 * 0.000000001 + z * p2
                q2 = 1
                q2 = 6.02427039364742 + z * q2
                q2 = 3.67983563856161 + z * q2
                q2 = 1.37702099489081 + z * q2
                q2 = 2.16236993594497 * 0.1 + z * q2
                q2 = 1.34204006088543 * 0.01 + z * q2
                q2 = 3.28014464682128 * 0.0001 + z * q2
                q2 = 2.89247864745381 * 0.000001 + z * q2
                q2 = 6.79019408009981 * 0.000000001 + z * q2
                x1 = z * p2 / q2
            End If
            x = x0 - x1
            If code <> 0 Then
                x = -x
            End If
            result = x
            Return result
        End Function


    End Class
    Public Class igammaf
        '************************************************************************
        '        Incomplete gamma integral
        '
        '        The function is defined by
        '
        '                                  x
        '                                   -
        '                          1       | |  -t  a-1
        '         igam(a,x)  =   -----     |   e   t   dt.
        '                         -      | |
        '                        | (a)    -
        '                                  0
        '
        '
        '        In this implementation both arguments must be positive.
        '        The integral is evaluated by either a power series or
        '        continued fraction expansion, depending on the relative
        '        values of a and x.
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0,30       200000       3.6e-14     2.9e-15
        '           IEEE      0,100      300000       9.9e-14     1.5e-14
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1985, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function incompletegamma(a As Double, x As Double) As Double
            Dim result As Double = 0
            Dim igammaepsilon As Double = 0
            Dim ans As Double = 0
            Dim ax As Double = 0
            Dim c As Double = 0
            Dim r As Double = 0
            Dim tmp As Double = 0

            igammaepsilon = 0.000000000000001
            If CDbl(x) <= CDbl(0) OrElse CDbl(a) <= CDbl(0) Then
                result = 0
                Return result
            End If
            If CDbl(x) > CDbl(1) AndAlso CDbl(x) > CDbl(a) Then
                result = 1 - incompletegammac(a, x)
                Return result
            End If
            ax = a * System.Math.Log(x) - x - gammafunc.lngamma(a, tmp)
            If CDbl(ax) < CDbl(-709.782712893384) Then
                result = 0
                Return result
            End If
            ax = System.Math.Exp(ax)
            r = a
            c = 1
            ans = 1
            Do
                r = r + 1
                c = c * x / r
                ans = ans + c
            Loop While CDbl(c / ans) > CDbl(igammaepsilon)
            result = ans * ax / a
            Return result
        End Function


        '************************************************************************
        '        Complemented incomplete gamma integral
        '
        '        The function is defined by
        '
        '
        '         igamc(a,x)   =   1 - igam(a,x)
        '
        '                                   inf.
        '                                     -
        '                            1       | |  -t  a-1
        '                      =   -----     |   e   t   dt.
        '                           -      | |
        '                          | (a)    -
        '                                    x
        '
        '
        '        In this implementation both arguments must be positive.
        '        The integral is evaluated by either a power series or
        '        continued fraction expansion, depending on the relative
        '        values of a and x.
        '
        '        ACCURACY:
        '
        '        Tested at random a, x.
        '                       a         x                      Relative error:
        '        arithmetic   domain   domain     # trials      peak         rms
        '           IEEE     0.5,100   0,100      200000       1.9e-14     1.7e-15
        '           IEEE     0.01,0.5  0,100      200000       1.4e-13     1.6e-15
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1985, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function incompletegammac(a As Double, x As Double) As Double
            Dim result As Double = 0
            Dim igammaepsilon As Double = 0
            Dim igammabignumber As Double = 0
            Dim igammabignumberinv As Double = 0
            Dim ans As Double = 0
            Dim ax As Double = 0
            Dim c As Double = 0
            Dim yc As Double = 0
            Dim r As Double = 0
            Dim t As Double = 0
            Dim y As Double = 0
            Dim z As Double = 0
            Dim pk As Double = 0
            Dim pkm1 As Double = 0
            Dim pkm2 As Double = 0
            Dim qk As Double = 0
            Dim qkm1 As Double = 0
            Dim qkm2 As Double = 0
            Dim tmp As Double = 0

            igammaepsilon = 0.000000000000001
            igammabignumber = 4.5035996273705E+15
            igammabignumberinv = 2.22044604925031 * 0.0000000000000001
            If CDbl(x) <= CDbl(0) OrElse CDbl(a) <= CDbl(0) Then
                result = 1
                Return result
            End If
            If CDbl(x) < CDbl(1) OrElse CDbl(x) < CDbl(a) Then
                result = 1 - incompletegamma(a, x)
                Return result
            End If
            ax = a * System.Math.Log(x) - x - gammafunc.lngamma(a, tmp)
            If CDbl(ax) < CDbl(-709.782712893384) Then
                result = 0
                Return result
            End If
            ax = System.Math.Exp(ax)
            y = 1 - a
            z = x + y + 1
            c = 0
            pkm2 = 1
            qkm2 = x
            pkm1 = x + 1
            qkm1 = z * x
            ans = pkm1 / qkm1
            Do
                c = c + 1
                y = y + 1
                z = z + 2
                yc = y * c
                pk = pkm1 * z - pkm2 * yc
                qk = qkm1 * z - qkm2 * yc
                If CDbl(qk) <> CDbl(0) Then
                    r = pk / qk
                    t = System.Math.Abs((ans - r) / r)
                    ans = r
                Else
                    t = 1
                End If
                pkm2 = pkm1
                pkm1 = pk
                qkm2 = qkm1
                qkm1 = qk
                If CDbl(System.Math.Abs(pk)) > CDbl(igammabignumber) Then
                    pkm2 = pkm2 * igammabignumberinv
                    pkm1 = pkm1 * igammabignumberinv
                    qkm2 = qkm2 * igammabignumberinv
                    qkm1 = qkm1 * igammabignumberinv
                End If
            Loop While CDbl(t) > CDbl(igammaepsilon)
            result = ans * ax
            Return result
        End Function


        '************************************************************************
        '        Inverse of complemented imcomplete gamma integral
        '
        '        Given p, the function finds x such that
        '
        '         igamc( a, x ) = p.
        '
        '        Starting with the approximate value
        '
        '                3
        '         x = a t
        '
        '         where
        '
        '         t = 1 - d - ndtri(p) sqrt(d)
        '
        '        and
        '
        '         d = 1/9a,
        '
        '        the routine performs up to 10 Newton iterations to find the
        '        root of igamc(a,x) - p = 0.
        '
        '        ACCURACY:
        '
        '        Tested at random a, p in the intervals indicated.
        '
        '                       a        p                      Relative error:
        '        arithmetic   domain   domain     # trials      peak         rms
        '           IEEE     0.5,100   0,0.5       100000       1.0e-14     1.7e-15
        '           IEEE     0.01,0.5  0,0.5       100000       9.0e-14     3.4e-15
        '           IEEE    0.5,10000  0,0.5        20000       2.3e-13     3.8e-14
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function invincompletegammac(a As Double, y0 As Double) As Double
            Dim result As Double = 0
            Dim igammaepsilon As Double = 0
            Dim iinvgammabignumber As Double = 0
            Dim x0 As Double = 0
            Dim x1 As Double = 0
            Dim x As Double = 0
            Dim yl As Double = 0
            Dim yh As Double = 0
            Dim y As Double = 0
            Dim d As Double = 0
            Dim lgm As Double = 0
            Dim dithresh As Double = 0
            Dim i As Integer = 0
            Dim dir As Integer = 0
            Dim tmp As Double = 0

            igammaepsilon = 0.000000000000001
            iinvgammabignumber = 4.5035996273705E+15
            x0 = iinvgammabignumber
            yl = 0
            x1 = 0
            yh = 1
            dithresh = 5 * igammaepsilon
            d = 1 / (9 * a)
            y = 1 - d - normaldistr.invnormaldistribution(y0) * System.Math.sqrt(d)
            x = a * y * y * y
            lgm = gammafunc.lngamma(a, tmp)
            i = 0
            While i < 10
                If CDbl(x) > CDbl(x0) OrElse CDbl(x) < CDbl(x1) Then
                    d = 0.0625
                    Exit While
                End If
                y = incompletegammac(a, x)
                If CDbl(y) < CDbl(yl) OrElse CDbl(y) > CDbl(yh) Then
                    d = 0.0625
                    Exit While
                End If
                If CDbl(y) < CDbl(y0) Then
                    x0 = x
                    yl = y
                Else
                    x1 = x
                    yh = y
                End If
                d = (a - 1) * System.Math.Log(x) - x - lgm
                If CDbl(d) < CDbl(-709.782712893384) Then
                    d = 0.0625
                    Exit While
                End If
                d = -System.Math.Exp(d)
                d = (y - y0) / d
                If CDbl(System.Math.Abs(d / x)) < CDbl(igammaepsilon) Then
                    result = x
                    Return result
                End If
                x = x - d
                i = i + 1
            End While
            If CDbl(x0) = CDbl(iinvgammabignumber) Then
                If CDbl(x) <= CDbl(0) Then
                    x = 1
                End If
                While CDbl(x0) = CDbl(iinvgammabignumber)
                    x = (1 + d) * x
                    y = incompletegammac(a, x)
                    If CDbl(y) < CDbl(y0) Then
                        x0 = x
                        yl = y
                        Exit While
                    End If
                    d = d + d
                End While
            End If
            d = 0.5
            dir = 0
            i = 0
            While i < 400
                x = x1 + d * (x0 - x1)
                y = incompletegammac(a, x)
                lgm = (x0 - x1) / (x1 + x0)
                If CDbl(System.Math.Abs(lgm)) < CDbl(dithresh) Then
                    Exit While
                End If
                lgm = (y - y0) / y0
                If CDbl(System.Math.Abs(lgm)) < CDbl(dithresh) Then
                    Exit While
                End If
                If CDbl(x) <= CDbl(0.0) Then
                    Exit While
                End If
                If CDbl(y) >= CDbl(y0) Then
                    x1 = x
                    yh = y
                    If dir < 0 Then
                        dir = 0
                        d = 0.5
                    Else
                        If dir > 1 Then
                            d = 0.5 * d + 0.5
                        Else
                            d = (y0 - yl) / (yh - yl)
                        End If
                    End If
                    dir = dir + 1
                Else
                    x0 = x
                    yl = y
                    If dir > 0 Then
                        dir = 0
                        d = 0.5
                    Else
                        If dir < -1 Then
                            d = 0.5 * d
                        Else
                            d = (y0 - yl) / (yh - yl)
                        End If
                    End If
                    dir = dir - 1
                End If
                i = i + 1
            End While
            result = x
            Return result
        End Function


    End Class
    Public Class airyf
        '************************************************************************
        '        Airy function
        '
        '        Solution of the differential equation
        '
        '        y"(x) = xy.
        '
        '        The function returns the two independent solutions Ai, Bi
        '        and their first derivatives Ai'(x), Bi'(x).
        '
        '        Evaluation is by power series summation for small x,
        '        by rational minimax approximations for large x.
        '
        '
        '
        '        ACCURACY:
        '        Error criterion is absolute when function <= 1, relative
        '        when function > 1, except * denotes relative error criterion.
        '        For large negative x, the absolute error increases as x^1.5.
        '        For large positive x, the relative error increases as x^1.5.
        '
        '        Arithmetic  domain   function  # trials      peak         rms
        '        IEEE        -10, 0     Ai        10000       1.6e-15     2.7e-16
        '        IEEE          0, 10    Ai        10000       2.3e-14*    1.8e-15*
        '        IEEE        -10, 0     Ai'       10000       4.6e-15     7.6e-16
        '        IEEE          0, 10    Ai'       10000       1.8e-14*    1.5e-15*
        '        IEEE        -10, 10    Bi        30000       4.2e-15     5.3e-16
        '        IEEE        -10, 10    Bi'       30000       4.9e-15     7.3e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Sub airy(x As Double, ByRef ai As Double, ByRef aip As Double, ByRef bi As Double, ByRef bip As Double)
            Dim z As Double = 0
            Dim zz As Double = 0
            Dim t As Double = 0
            Dim f As Double = 0
            Dim g As Double = 0
            Dim uf As Double = 0
            Dim ug As Double = 0
            Dim k As Double = 0
            Dim zeta As Double = 0
            Dim theta As Double = 0
            Dim domflg As Integer = 0
            Dim c1 As Double = 0
            Dim c2 As Double = 0
            Dim sqrt3 As Double = 0
            Dim sqpii As Double = 0
            Dim afn As Double = 0
            Dim afd As Double = 0
            Dim agn As Double = 0
            Dim agd As Double = 0
            Dim apfn As Double = 0
            Dim apfd As Double = 0
            Dim apgn As Double = 0
            Dim apgd As Double = 0
            Dim an As Double = 0
            Dim ad As Double = 0
            Dim apn As Double = 0
            Dim apd As Double = 0
            Dim bn16 As Double = 0
            Dim bd16 As Double = 0
            Dim bppn As Double = 0
            Dim bppd As Double = 0

            ai = 0
            aip = 0
            bi = 0
            bip = 0

            sqpii = 0.564189583547756
            c1 = 0.355028053887817
            c2 = 0.258819403792807
            sqrt3 = 1.73205080756888
            domflg = 0
            If CDbl(x) > CDbl(25.77) Then
                ai = 0
                aip = 0
                bi = Math.maxrealnumber
                bip = Math.maxrealnumber
                Return
            End If
            If CDbl(x) < CDbl(-2.09) Then
                domflg = 15
                t = System.Math.sqrt(-x)
                zeta = -(2.0 * x * t / 3.0)
                t = System.Math.sqrt(t)
                k = sqpii / t
                z = 1.0 / zeta
                zz = z * z
                afn = -0.131696323418332
                afn = afn * zz - 0.626456544431912
                afn = afn * zz - 0.693158036036933
                afn = afn * zz - 0.279779981545119
                afn = afn * zz - 0.04919001326095
                afn = afn * zz - 0.00406265923594885
                afn = afn * zz - 0.000159276496239262
                afn = afn * zz - 0.00000277649108155233
                afn = afn * zz - 0.0000000167787698489115
                afd = 1.0
                afd = afd * zz + 13.3560420706553
                afd = afd * zz + 32.6825032795225
                afd = afd * zz + 26.73670409415
                afd = afd * zz + 9.1870740290726
                afd = afd * zz + 1.47529146771666
                afd = afd * zz + 0.115687173795188
                afd = afd * zz + 0.00440291641615211
                afd = afd * zz + 0.0000754720348287414
                afd = afd * zz + 0.00000045185009297058
                uf = 1.0 + zz * afn / afd
                agn = 0.0197339932091686
                agn = agn * zz + 0.391103029615688
                agn = agn * zz + 1.06579897599596
                agn = agn * zz + 0.93916922981665
                agn = agn * zz + 0.351465656105548
                agn = agn * zz + 0.0633888919628925
                agn = agn * zz + 0.00585804113048388
                agn = agn * zz + 0.000282851600836737
                agn = agn * zz + 0.00000698793669997261
                agn = agn * zz + 0.0000000811789239554389
                agn = agn * zz + 0.000000000341551784765924
                agd = 1.0
                agd = agd * zz + 9.30892908077442
                agd = agd * zz + 19.8352928718312
                agd = agd * zz + 15.5646628932865
                agd = agd * zz + 5.47686069422975
                agd = agd * zz + 0.954293611618962
                agd = agd * zz + 0.0864580826352392
                agd = agd * zz + 0.00412656523824223
                agd = agd * zz + 0.000101259085116509
                agd = agd * zz + 0.00000117166733214414
                agd = agd * zz + 0.0000000049183457006293
                ug = z * agn / agd
                theta = zeta + 0.25 * System.Math.PI
                f = System.Math.Sin(theta)
                g = System.Math.Cos(theta)
                ai = k * (f * uf - g * ug)
                bi = k * (g * uf + f * ug)
                apfn = 0.185365624022536
                apfn = apfn * zz + 0.886712188052584
                apfn = apfn * zz + 0.987391981747399
                apfn = apfn * zz + 0.401241082318004
                apfn = apfn * zz + 0.0710304926289631
                apfn = apfn * zz + 0.00590618657995662
                apfn = apfn * zz + 0.000233051409401777
                apfn = apfn * zz + 0.00000408718778289035
                apfn = apfn * zz + 0.0000000248379932900442
                apfd = 1.0
                apfd = apfd * zz + 14.7345854687503
                apfd = apfd * zz + 37.542393343549
                apfd = apfd * zz + 31.4657751203046
                apfd = apfd * zz + 10.9969125207299
                apfd = apfd * zz + 1.78885054766999
                apfd = apfd * zz + 0.141733275753663
                apfd = apfd * zz + 0.00544066067017226
                apfd = apfd * zz + 0.0000939421290654511
                apfd = apfd * zz + 0.000000565978713036027
                uf = 1.0 + zz * apfn / apfd
                apgn = -0.0355615429033082
                apgn = apgn * zz - 0.637311518129436
                apgn = apgn * zz - 1.70856738884312
                apgn = apgn * zz - 1.50221872117317
                apgn = apgn * zz - 0.563606665822103
                apgn = apgn * zz - 0.102101031120217
                apgn = apgn * zz - 0.00948396695961445
                apgn = apgn * zz - 0.000460325307486781
                apgn = apgn * zz - 0.0000114300836484517
                apgn = apgn * zz - 0.000000133415518685547
                apgn = apgn * zz - 0.000000000563803833958894
                apgd = 1.0
                apgd = apgd * zz + 9.8586580169613
                apgd = apgd * zz + 21.6401867356586
                apgd = apgd * zz + 17.3130776389749
                apgd = apgd * zz + 6.17872175280829
                apgd = apgd * zz + 1.08848694396321
                apgd = apgd * zz + 0.0995005543440888
                apgd = apgd * zz + 0.00478468199683887
                apgd = apgd * zz + 0.000118159633322839
                apgd = apgd * zz + 0.00000137480673554219
                apgd = apgd * zz + 0.00000000579912514929148
                ug = z * apgn / apgd
                k = sqpii * t
                aip = -(k * (g * uf + f * ug))
                bip = k * (f * uf - g * ug)
                Return
            End If
            If CDbl(x) >= CDbl(2.09) Then
                domflg = 5
                t = System.Math.sqrt(x)
                zeta = 2.0 * x * t / 3.0
                g = System.Math.Exp(zeta)
                t = System.Math.sqrt(t)
                k = 2.0 * t * g
                z = 1.0 / zeta
                an = 0.346538101525629
                an = an * z + 12.0075952739646
                an = an * z + 76.2796053615235
                an = an * z + 168.089224934631
                an = an * z + 159.756391350164
                an = an * z + 70.5360906840444
                an = an * z + 14.026469116339
                an = an * z + 1.0
                ad = 0.56759453263877
                ad = ad * z + 14.7562562584847
                ad = ad * z + 84.5138970141475
                ad = ad * z + 177.3180881454
                ad = ad * z + 164.23469287153
                ad = ad * z + 71.4778400825576
                ad = ad * z + 14.0959135607834
                ad = ad * z + 1.0
                f = an / ad
                ai = sqpii * f / k
                k = -(0.5 * sqpii * t / g)
                apn = 0.613759184814036
                apn = apn * z + 14.7454670787755
                apn = apn * z + 82.0584123476061
                apn = apn * z + 171.184781360976
                apn = apn * z + 159.317847137142
                apn = apn * z + 69.9778599330103
                apn = apn * z + 13.9470856980482
                apn = apn * z + 1.0
                apd = 0.334203677749737
                apd = apd * z + 11.1810297306158
                apd = apd * z + 71.172735214786
                apd = apd * z + 158.778084372838
                apd = apd * z + 153.206427475809
                apd = apd * z + 68.675230459278
                apd = apd * z + 13.8498634758259
                apd = apd * z + 1.0
                f = apn / apd
                aip = f * k
                If CDbl(x) > CDbl(8.3203353) Then
                    bn16 = -0.253240795869364
                    bn16 = bn16 * z + 0.575285167332467
                    bn16 = bn16 * z - 0.329907036873225
                    bn16 = bn16 * z + 0.06444040689482
                    bn16 = bn16 * z - 0.00382519546641337
                    bd16 = 1.0
                    bd16 = bd16 * z - 7.15685095054035
                    bd16 = bd16 * z + 10.6039580715665
                    bd16 = bd16 * z - 5.23246636471251
                    bd16 = bd16 * z + 0.957395864378384
                    bd16 = bd16 * z - 0.055082814716355
                    f = z * bn16 / bd16
                    k = sqpii * g
                    bi = k * (1.0 + f) / t
                    bppn = 0.465461162774652
                    bppn = bppn * z - 1.08992173800494
                    bppn = bppn * z + 0.638800117371828
                    bppn = bppn * z - 0.126844349553103
                    bppn = bppn * z + 0.0076248784434211
                    bppd = 1.0
                    bppd = bppd * z - 8.70622787633159
                    bppd = bppd * z + 13.8993162704553
                    bppd = bppd * z - 7.14116144616431
                    bppd = bppd * z + 1.34008595960681
                    bppd = bppd * z - 0.0784273211323342
                    f = z * bppn / bppd
                    bip = k * t * (1.0 + f)
                    Return
                End If
            End If
            f = 1.0
            g = x
            t = 1.0
            uf = 1.0
            ug = x
            k = 1.0
            z = x * x * x
            While CDbl(t) > CDbl(Math.machineepsilon)
                uf = uf * z
                k = k + 1.0
                uf = uf / k
                ug = ug * z
                k = k + 1.0
                ug = ug / k
                uf = uf / k
                f = f + uf
                k = k + 1.0
                ug = ug / k
                g = g + ug
                t = System.Math.Abs(uf / f)
            End While
            uf = c1 * f
            ug = c2 * g
            If domflg Mod 2 = 0 Then
                ai = uf - ug
            End If
            If domflg \ 2 Mod 2 = 0 Then
                bi = sqrt3 * (uf + ug)
            End If
            k = 4.0
            uf = x * x / 2.0
            ug = z / 3.0
            f = uf
            g = 1.0 + ug
            uf = uf / 3.0
            t = 1.0
            While CDbl(t) > CDbl(Math.machineepsilon)
                uf = uf * z
                ug = ug / k
                k = k + 1.0
                ug = ug * z
                uf = uf / k
                f = f + uf
                k = k + 1.0
                ug = ug / k
                uf = uf / k
                g = g + ug
                k = k + 1.0
                t = System.Math.Abs(ug / g)
            End While
            uf = c1 * f
            ug = c2 * g
            If domflg \ 4 Mod 2 = 0 Then
                aip = uf - ug
            End If
            If domflg \ 8 Mod 2 = 0 Then
                bip = sqrt3 * (uf + ug)
            End If
        End Sub


    End Class
    Public Class bessel
        '************************************************************************
        '        Bessel function of order zero
        '
        '        Returns Bessel function of order zero of the argument.
        '
        '        The domain is divided into the intervals [0, 5] and
        '        (5, infinity). In the first interval the following rational
        '        approximation is used:
        '
        '
        '               2         2
        '        (w - r  ) (w - r  ) P (w) / Q (w)
        '              1         2    3       8
        '
        '                   2
        '        where w = x  and the two r's are zeros of the function.
        '
        '        In the second interval, the Hankel asymptotic expansion
        '        is employed with two rational functions of degree 6/6
        '        and 7/7.
        '
        '        ACCURACY:
        '
        '                             Absolute error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0, 30       60000       4.2e-16     1.1e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function besselj0(x As Double) As Double
            Dim result As Double = 0
            Dim xsq As Double = 0
            Dim nn As Double = 0
            Dim pzero As Double = 0
            Dim qzero As Double = 0
            Dim p1 As Double = 0
            Dim q1 As Double = 0

            If CDbl(x) < CDbl(0) Then
                x = -x
            End If
            If CDbl(x) > CDbl(8.0) Then
                besselasympt0(x, pzero, qzero)
                nn = x - System.Math.PI / 4
                result = System.Math.sqrt(2 / System.Math.PI / x) * (pzero * System.Math.Cos(nn) - qzero * System.Math.Sin(nn))
                Return result
            End If
            xsq = Math.sqr(x)
            p1 = 26857.8685698002
            p1 = -40504123.7183313 + xsq * p1
            p1 = 25071582855.3688 + xsq * p1
            p1 = -8085222034853.79 + xsq * p1
            p1 = 1.43435493914034E+15 + xsq * p1
            p1 = -1.36762035308817E+17 + xsq * p1
            p1 = 6.38205934107236E+18 + xsq * p1
            p1 = -1.17915762910761E+20 + xsq * p1
            p1 = 4.93378725179413E+20 + xsq * p1
            q1 = 1.0
            q1 = 1363.06365232897 + xsq * q1
            q1 = 1114636.09846299 + xsq * q1
            q1 = 669998767.298224 + xsq * q1
            q1 = 312304311494.121 + xsq * q1
            q1 = 112775673967980.0 + xsq * q1
            q1 = 3.02463561670946E+16 + xsq * q1
            q1 = 5.42891838409228E+18 + xsq * q1
            q1 = 4.93378725179413E+20 + xsq * q1
            result = p1 / q1
            Return result
        End Function


        '************************************************************************
        '        Bessel function of order one
        '
        '        Returns Bessel function of order one of the argument.
        '
        '        The domain is divided into the intervals [0, 8] and
        '        (8, infinity). In the first interval a 24 term Chebyshev
        '        expansion is used. In the second, the asymptotic
        '        trigonometric representation is employed using two
        '        rational functions of degree 5/5.
        '
        '        ACCURACY:
        '
        '                             Absolute error:
        '        arithmetic   domain      # trials      peak         rms
        '           IEEE      0, 30       30000       2.6e-16     1.1e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function besselj1(x As Double) As Double
            Dim result As Double = 0
            Dim s As Double = 0
            Dim xsq As Double = 0
            Dim nn As Double = 0
            Dim pzero As Double = 0
            Dim qzero As Double = 0
            Dim p1 As Double = 0
            Dim q1 As Double = 0

            s = System.Math.Sign(x)
            If CDbl(x) < CDbl(0) Then
                x = -x
            End If
            If CDbl(x) > CDbl(8.0) Then
                besselasympt1(x, pzero, qzero)
                nn = x - 3 * System.Math.PI / 4
                result = System.Math.sqrt(2 / System.Math.PI / x) * (pzero * System.Math.Cos(nn) - qzero * System.Math.Sin(nn))
                If CDbl(s) < CDbl(0) Then
                    result = -result
                End If
                Return result
            End If
            xsq = Math.sqr(x)
            p1 = 2701.12271089232
            p1 = -4695753.530643 + xsq * p1
            p1 = 3413234182.3017 + xsq * p1
            p1 = -1322983480332.13 + xsq * p1
            p1 = 290879526383478.0 + xsq * p1
            p1 = -3.58881756991011E+16 + xsq * p1
            p1 = 2.316433580634E+18 + xsq * p1
            p1 = -6.67210656892492E+19 + xsq * p1
            p1 = 5.81199354001606E+20 + xsq * p1
            q1 = 1.0
            q1 = 1606.93157348149 + xsq * q1
            q1 = 1501793.59499859 + xsq * q1
            q1 = 1013863514.35867 + xsq * q1
            q1 = 524371026216.765 + xsq * q1
            q1 = 208166122130761.0 + xsq * q1
            q1 = 6.09206139891752E+16 + xsq * q1
            q1 = 1.18577071219032E+19 + xsq * q1
            q1 = 1.16239870800321E+21 + xsq * q1
            result = s * x * p1 / q1
            Return result
        End Function


        '************************************************************************
        '        Bessel function of integer order
        '
        '        Returns Bessel function of order n, where n is a
        '        (possibly negative) integer.
        '
        '        The ratio of jn(x) to j0(x) is computed by backward
        '        recurrence.  First the ratio jn/jn-1 is found by a
        '        continued fraction expansion.  Then the recurrence
        '        relating successive orders is applied until j0 or j1 is
        '        reached.
        '
        '        If n = 0 or 1 the routine for j0 or j1 is called
        '        directly.
        '
        '        ACCURACY:
        '
        '                             Absolute error:
        '        arithmetic   range      # trials      peak         rms
        '           IEEE      0, 30        5000       4.4e-16     7.9e-17
        '
        '
        '        Not suitable for large n or x. Use jv() (fractional order) instead.
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function besseljn(n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim pkm2 As Double = 0
            Dim pkm1 As Double = 0
            Dim pk As Double = 0
            Dim xk As Double = 0
            Dim r As Double = 0
            Dim ans As Double = 0
            Dim k As Integer = 0
            Dim sg As Integer = 0

            If n < 0 Then
                n = -n
                If n Mod 2 = 0 Then
                    sg = 1
                Else
                    sg = -1
                End If
            Else
                sg = 1
            End If
            If CDbl(x) < CDbl(0) Then
                If n Mod 2 <> 0 Then
                    sg = -sg
                End If
                x = -x
            End If
            If n = 0 Then
                result = sg * besselj0(x)
                Return result
            End If
            If n = 1 Then
                result = sg * besselj1(x)
                Return result
            End If
            If n = 2 Then
                If CDbl(x) = CDbl(0) Then
                    result = 0
                Else
                    result = sg * (2.0 * besselj1(x) / x - besselj0(x))
                End If
                Return result
            End If
            If CDbl(x) < CDbl(Math.machineepsilon) Then
                result = 0
                Return result
            End If
            k = 53
            pk = 2 * (n + k)
            ans = pk
            xk = x * x
            Do
                pk = pk - 2.0
                ans = pk - xk / ans
                k = k - 1
            Loop While k <> 0
            ans = x / ans
            pk = 1.0
            pkm1 = 1.0 / ans
            k = n - 1
            r = 2 * k
            Do
                pkm2 = (pkm1 * r - pk * x) / x
                pk = pkm1
                pkm1 = pkm2
                r = r - 2.0
                k = k - 1
            Loop While k <> 0
            If CDbl(System.Math.Abs(pk)) > CDbl(System.Math.Abs(pkm1)) Then
                ans = besselj1(x) / pk
            Else
                ans = besselj0(x) / pkm1
            End If
            result = sg * ans
            Return result
        End Function


        '************************************************************************
        '        Bessel function of the second kind, order zero
        '
        '        Returns Bessel function of the second kind, of order
        '        zero, of the argument.
        '
        '        The domain is divided into the intervals [0, 5] and
        '        (5, infinity). In the first interval a rational approximation
        '        R(x) is employed to compute
        '          y0(x)  = R(x)  +   2 * log(x) * j0(x) / PI.
        '        Thus a call to j0() is required.
        '
        '        In the second interval, the Hankel asymptotic expansion
        '        is employed with two rational functions of degree 6/6
        '        and 7/7.
        '
        '
        '
        '        ACCURACY:
        '
        '         Absolute error, when y0(x) < 1; else relative error:
        '
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0, 30       30000       1.3e-15     1.6e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function bessely0(x As Double) As Double
            Dim result As Double = 0
            Dim nn As Double = 0
            Dim xsq As Double = 0
            Dim pzero As Double = 0
            Dim qzero As Double = 0
            Dim p4 As Double = 0
            Dim q4 As Double = 0

            If CDbl(x) > CDbl(8.0) Then
                besselasympt0(x, pzero, qzero)
                nn = x - System.Math.PI / 4
                result = System.Math.sqrt(2 / System.Math.PI / x) * (pzero * System.Math.Sin(nn) + qzero * System.Math.Cos(nn))
                Return result
            End If
            xsq = Math.sqr(x)
            p4 = -41370.3549793315
            p4 = 59152134.6568689 + xsq * p4
            p4 = -34363712229.7904 + xsq * p4
            p4 = 10255208596863.9 + xsq * p4
            p4 = -1.64860581718573E+15 + xsq * p4
            p4 = 1.37562431639934E+17 + xsq * p4
            p4 = -5.24706558111277E+18 + xsq * p4
            p4 = 6.58747327571955E+19 + xsq * p4
            p4 = -2.75028667862911E+19 + xsq * p4
            q4 = 1.0
            q4 = 1282.45277247899 + xsq * q4
            q4 = 1001702.64128891 + xsq * q4
            q4 = 579512264.070073 + xsq * q4
            q4 = 261306575504.108 + xsq * q4
            q4 = 91620380340751.9 + xsq * q4
            q4 = 2.39288304349978E+16 + xsq * q4
            q4 = 4.19241704341084E+18 + xsq * q4
            q4 = 3.72645883898617E+20 + xsq * q4
            result = p4 / q4 + 2 / System.Math.PI * besselj0(x) * System.Math.Log(x)
            Return result
        End Function


        '************************************************************************
        '        Bessel function of second kind of order one
        '
        '        Returns Bessel function of the second kind of order one
        '        of the argument.
        '
        '        The domain is divided into the intervals [0, 8] and
        '        (8, infinity). In the first interval a 25 term Chebyshev
        '        expansion is used, and a call to j1() is required.
        '        In the second, the asymptotic trigonometric representation
        '        is employed using two rational functions of degree 5/5.
        '
        '        ACCURACY:
        '
        '                             Absolute error:
        '        arithmetic   domain      # trials      peak         rms
        '           IEEE      0, 30       30000       1.0e-15     1.3e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function bessely1(x As Double) As Double
            Dim result As Double = 0
            Dim nn As Double = 0
            Dim xsq As Double = 0
            Dim pzero As Double = 0
            Dim qzero As Double = 0
            Dim p4 As Double = 0
            Dim q4 As Double = 0

            If CDbl(x) > CDbl(8.0) Then
                besselasympt1(x, pzero, qzero)
                nn = x - 3 * System.Math.PI / 4
                result = System.Math.sqrt(2 / System.Math.PI / x) * (pzero * System.Math.Sin(nn) + qzero * System.Math.Cos(nn))
                Return result
            End If
            xsq = Math.sqr(x)
            p4 = -2108847.54013312
            p4 = 3639488548.124 + xsq * p4
            p4 = -2580681702194.45 + xsq * p4
            p4 = 956993023992168.0 + xsq * p4
            p4 = -1.9658874627221402E+17 + xsq * p4
            p4 = 2.1931073399178E+19 + xsq * p4
            p4 = -1.21229755541451E+21 + xsq * p4
            p4 = 2.65547383143485E+22 + xsq * p4
            p4 = -9.96375342430692E+22 + xsq * p4
            q4 = 1.0
            q4 = 1612.361029677 + xsq * q4
            q4 = 1563282.75489958 + xsq * q4
            q4 = 1128686837.16944 + xsq * q4
            q4 = 646534088126.528 + xsq * q4
            q4 = 297663212564728.0 + xsq * q4
            q4 = 1.08225825940882E+17 + xsq * q4
            q4 = 2.95498793589715E+19 + xsq * q4
            q4 = 5.43531037718885E+21 + xsq * q4
            q4 = 5.08206736694124E+23 + xsq * q4
            result = x * p4 / q4 + 2 / System.Math.PI * (besselj1(x) * System.Math.Log(x) - 1 / x)
            Return result
        End Function


        '************************************************************************
        '        Bessel function of second kind of integer order
        '
        '        Returns Bessel function of order n, where n is a
        '        (possibly negative) integer.
        '
        '        The function is evaluated by forward recurrence on
        '        n, starting with values computed by the routines
        '        y0() and y1().
        '
        '        If n = 0 or 1 the routine for y0 or y1 is called
        '        directly.
        '
        '        ACCURACY:
        '                             Absolute error, except relative
        '                             when y > 1:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0, 30       30000       3.4e-15     4.3e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function besselyn(n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim i As Integer = 0
            Dim a As Double = 0
            Dim b As Double = 0
            Dim tmp As Double = 0
            Dim s As Double = 0

            s = 1
            If n < 0 Then
                n = -n
                If n Mod 2 <> 0 Then
                    s = -1
                End If
            End If
            If n = 0 Then
                result = bessely0(x)
                Return result
            End If
            If n = 1 Then
                result = s * bessely1(x)
                Return result
            End If
            a = bessely0(x)
            b = bessely1(x)
            For i = 1 To n - 1
                tmp = b
                b = 2 * i / x * b - a
                a = tmp
            Next
            result = s * b
            Return result
        End Function


        '************************************************************************
        '        Modified Bessel function of order zero
        '
        '        Returns modified Bessel function of order zero of the
        '        argument.
        '
        '        The function is defined as i0(x) = j0( ix ).
        '
        '        The range is partitioned into the two intervals [0,8] and
        '        (8, infinity).  Chebyshev polynomial expansions are employed
        '        in each interval.
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0,30        30000       5.8e-16     1.4e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function besseli0(x As Double) As Double
            Dim result As Double = 0
            Dim y As Double = 0
            Dim v As Double = 0
            Dim z As Double = 0
            Dim b0 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0

            If CDbl(x) < CDbl(0) Then
                x = -x
            End If
            If CDbl(x) <= CDbl(8.0) Then
                y = x / 2.0 - 2.0
                besselmfirstcheb(-4.41534164647934E-18, b0, b1, b2)
                besselmnextcheb(y, 3.33079451882224E-17, b0, b1, b2)
                besselmnextcheb(y, -0.000000000000000243127984654795, b0, b1, b2)
                besselmnextcheb(y, 0.00000000000000171539128555513, b0, b1, b2)
                besselmnextcheb(y, -0.0000000000000116853328779935, b0, b1, b2)
                besselmnextcheb(y, 0.0000000000000767618549860494, b0, b1, b2)
                besselmnextcheb(y, -0.000000000000485644678311193, b0, b1, b2)
                besselmnextcheb(y, 0.00000000000295505266312964, b0, b1, b2)
                besselmnextcheb(y, -0.0000000000172682629144156, b0, b1, b2)
                besselmnextcheb(y, 0.0000000000967580903537324, b0, b1, b2)
                besselmnextcheb(y, -0.000000000518979560163526, b0, b1, b2)
                besselmnextcheb(y, 0.00000000265982372468239, b0, b1, b2)
                besselmnextcheb(y, -0.0000000130002500998625, b0, b1, b2)
                besselmnextcheb(y, 0.0000000604699502254192, b0, b1, b2)
                besselmnextcheb(y, -0.000000267079385394061, b0, b1, b2)
                besselmnextcheb(y, 0.0000011173875391201, b0, b1, b2)
                besselmnextcheb(y, -0.00000441673835845875, b0, b1, b2)
                besselmnextcheb(y, 0.0000164484480707289, b0, b1, b2)
                besselmnextcheb(y, -0.000057541950100821, b0, b1, b2)
                besselmnextcheb(y, 0.000188502885095842, b0, b1, b2)
                besselmnextcheb(y, -0.000576375574538582, b0, b1, b2)
                besselmnextcheb(y, 0.00163947561694134, b0, b1, b2)
                besselmnextcheb(y, -0.00432430999505058, b0, b1, b2)
                besselmnextcheb(y, 0.010546460394595, b0, b1, b2)
                besselmnextcheb(y, -0.0237374148058995, b0, b1, b2)
                besselmnextcheb(y, 0.0493052842396707, b0, b1, b2)
                besselmnextcheb(y, -0.0949010970480476, b0, b1, b2)
                besselmnextcheb(y, 0.171620901522209, b0, b1, b2)
                besselmnextcheb(y, -0.304682672343198, b0, b1, b2)
                besselmnextcheb(y, 0.676795274409476, b0, b1, b2)
                v = 0.5 * (b0 - b2)
                result = System.Math.Exp(x) * v
                Return result
            End If
            z = 32.0 / x - 2.0
            besselmfirstcheb(-7.23318048787475E-18, b0, b1, b2)
            besselmnextcheb(z, -4.83050448594418E-18, b0, b1, b2)
            besselmnextcheb(z, 4.46562142029676E-17, b0, b1, b2)
            besselmnextcheb(z, 3.46122286769746E-17, b0, b1, b2)
            besselmnextcheb(z, -0.000000000000000282762398051658, b0, b1, b2)
            besselmnextcheb(z, -0.000000000000000342548561967722, b0, b1, b2)
            besselmnextcheb(z, 0.00000000000000177256013305653, b0, b1, b2)
            besselmnextcheb(z, 0.00000000000000381168066935262, b0, b1, b2)
            besselmnextcheb(z, -0.00000000000000955484669882831, b0, b1, b2)
            besselmnextcheb(z, -0.0000000000000415056934728722, b0, b1, b2)
            besselmnextcheb(z, 0.0000000000000154008621752141, b0, b1, b2)
            besselmnextcheb(z, 0.000000000000385277838274214, b0, b1, b2)
            besselmnextcheb(z, 0.000000000000718012445138367, b0, b1, b2)
            besselmnextcheb(z, -0.00000000000179417853150681, b0, b1, b2)
            besselmnextcheb(z, -0.0000000000132158118404477, b0, b1, b2)
            besselmnextcheb(z, -0.0000000000314991652796324, b0, b1, b2)
            besselmnextcheb(z, 0.0000000000118891471078464, b0, b1, b2)
            besselmnextcheb(z, 0.000000000494060238822497, b0, b1, b2)
            besselmnextcheb(z, 0.00000000339623202570839, b0, b1, b2)
            besselmnextcheb(z, 0.0000000226666899049818, b0, b1, b2)
            besselmnextcheb(z, 0.000000204891858946906, b0, b1, b2)
            besselmnextcheb(z, 0.00000289137052083476, b0, b1, b2)
            besselmnextcheb(z, 0.0000688975834691682, b0, b1, b2)
            besselmnextcheb(z, 0.00336911647825569, b0, b1, b2)
            besselmnextcheb(z, 0.804490411014109, b0, b1, b2)
            v = 0.5 * (b0 - b2)
            result = System.Math.Exp(x) * v / System.Math.sqrt(x)
            Return result
        End Function


        '************************************************************************
        '        Modified Bessel function of order one
        '
        '        Returns modified Bessel function of order one of the
        '        argument.
        '
        '        The function is defined as i1(x) = -i j1( ix ).
        '
        '        The range is partitioned into the two intervals [0,8] and
        '        (8, infinity).  Chebyshev polynomial expansions are employed
        '        in each interval.
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0, 30       30000       1.9e-15     2.1e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1985, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function besseli1(x As Double) As Double
            Dim result As Double = 0
            Dim y As Double = 0
            Dim z As Double = 0
            Dim v As Double = 0
            Dim b0 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0

            z = System.Math.Abs(x)
            If CDbl(z) <= CDbl(8.0) Then
                y = z / 2.0 - 2.0
                besselm1firstcheb(2.77791411276105E-18, b0, b1, b2)
                besselm1nextcheb(y, -2.11142121435817E-17, b0, b1, b2)
                besselm1nextcheb(y, 0.00000000000000015536319577362, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000000000110559694773539, b0, b1, b2)
                besselm1nextcheb(y, 0.00000000000000760068429473541, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000000000504218550472791, b0, b1, b2)
                besselm1nextcheb(y, 0.000000000000322379336594557, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000000198397439776494, b0, b1, b2)
                besselm1nextcheb(y, 0.0000000000117361862988909, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000000666348972350203, b0, b1, b2)
                besselm1nextcheb(y, 0.000000000362559028155212, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000188724975172283, b0, b1, b2)
                besselm1nextcheb(y, 0.00000000938153738649577, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000444505912879633, b0, b1, b2)
                besselm1nextcheb(y, 0.000000200329475355214, b0, b1, b2)
                besselm1nextcheb(y, -0.000000856872026469545, b0, b1, b2)
                besselm1nextcheb(y, 0.00000347025130813768, b0, b1, b2)
                besselm1nextcheb(y, -0.0000132731636560394, b0, b1, b2)
                besselm1nextcheb(y, 0.0000478156510755005, b0, b1, b2)
                besselm1nextcheb(y, -0.000161760815825897, b0, b1, b2)
                besselm1nextcheb(y, 0.000512285956168576, b0, b1, b2)
                besselm1nextcheb(y, -0.00151357245063125, b0, b1, b2)
                besselm1nextcheb(y, 0.00415642294431289, b0, b1, b2)
                besselm1nextcheb(y, -0.0105640848946262, b0, b1, b2)
                besselm1nextcheb(y, 0.0247264490306265, b0, b1, b2)
                besselm1nextcheb(y, -0.052945981208095, b0, b1, b2)
                besselm1nextcheb(y, 0.102643658689847, b0, b1, b2)
                besselm1nextcheb(y, -0.176416518357834, b0, b1, b2)
                besselm1nextcheb(y, 0.252587186443634, b0, b1, b2)
                v = 0.5 * (b0 - b2)
                z = v * z * System.Math.Exp(z)
            Else
                y = 32.0 / z - 2.0
                besselm1firstcheb(7.51729631084211E-18, b0, b1, b2)
                besselm1nextcheb(y, 4.41434832307171E-18, b0, b1, b2)
                besselm1nextcheb(y, -4.65030536848936E-17, b0, b1, b2)
                besselm1nextcheb(y, -3.20952592199342E-17, b0, b1, b2)
                besselm1nextcheb(y, 0.000000000000000296262899764595, b0, b1, b2)
                besselm1nextcheb(y, 0.000000000000000330820231092093, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000000000188035477551078, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000000000381440307243701, b0, b1, b2)
                besselm1nextcheb(y, 0.0000000000000104202769841288, b0, b1, b2)
                besselm1nextcheb(y, 0.0000000000000427244001671195, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000000000210154184277266, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000000040835511110922, b0, b1, b2)
                besselm1nextcheb(y, -0.000000000000719855177624591, b0, b1, b2)
                besselm1nextcheb(y, 0.00000000000203562854414709, b0, b1, b2)
                besselm1nextcheb(y, 0.0000000000141258074366138, b0, b1, b2)
                besselm1nextcheb(y, 0.0000000000325260358301549, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000000189749581235054, b0, b1, b2)
                besselm1nextcheb(y, -0.000000000558974346219658, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000383538038596424, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000263146884688952, b0, b1, b2)
                besselm1nextcheb(y, -0.000000251223623787021, b0, b1, b2)
                besselm1nextcheb(y, -0.00000388256480887769, b0, b1, b2)
                besselm1nextcheb(y, -0.000110588938762624, b0, b1, b2)
                besselm1nextcheb(y, -0.00976109749136147, b0, b1, b2)
                besselm1nextcheb(y, 0.77857623501828, b0, b1, b2)
                v = 0.5 * (b0 - b2)
                z = v * System.Math.Exp(z) / System.Math.sqrt(z)
            End If
            If CDbl(x) < CDbl(0) Then
                z = -z
            End If
            result = z
            Return result
        End Function


        '************************************************************************
        '        Modified Bessel function, second kind, order zero
        '
        '        Returns modified Bessel function of the second kind
        '        of order zero of the argument.
        '
        '        The range is partitioned into the two intervals [0,8] and
        '        (8, infinity).  Chebyshev polynomial expansions are employed
        '        in each interval.
        '
        '        ACCURACY:
        '
        '        Tested at 2000 random points between 0 and 8.  Peak absolute
        '        error (relative when K0 > 1) was 1.46e-14; rms, 4.26e-15.
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0, 30       30000       1.2e-15     1.6e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function besselk0(x As Double) As Double
            Dim result As Double = 0
            Dim y As Double = 0
            Dim z As Double = 0
            Dim v As Double = 0
            Dim b0 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0

            alglib.ap.assert(CDbl(x) > CDbl(0), "Domain error in BesselK0: x<=0")
            If CDbl(x) <= CDbl(2) Then
                y = x * x - 2.0
                besselmfirstcheb(0.000000000000000137446543561352, b0, b1, b2)
                besselmnextcheb(y, 0.0000000000000425981614279661, b0, b1, b2)
                besselmnextcheb(y, 0.0000000000103496952576338, b0, b1, b2)
                besselmnextcheb(y, 0.00000000190451637722021, b0, b1, b2)
                besselmnextcheb(y, 0.000000253479107902615, b0, b1, b2)
                besselmnextcheb(y, 0.0000228621210311945, b0, b1, b2)
                besselmnextcheb(y, 0.00126461541144693, b0, b1, b2)
                besselmnextcheb(y, 0.0359799365153615, b0, b1, b2)
                besselmnextcheb(y, 0.344289899924629, b0, b1, b2)
                besselmnextcheb(y, -0.535327393233903, b0, b1, b2)
                v = 0.5 * (b0 - b2)
                v = v - System.Math.Log(0.5 * x) * besseli0(x)
            Else
                z = 8.0 / x - 2.0
                besselmfirstcheb(5.30043377268626E-18, b0, b1, b2)
                besselmnextcheb(z, -1.64758043015242E-17, b0, b1, b2)
                besselmnextcheb(z, 5.21039150503903E-17, b0, b1, b2)
                besselmnextcheb(z, -0.000000000000000167823109680541, b0, b1, b2)
                besselmnextcheb(z, 0.000000000000000551205597852432, b0, b1, b2)
                besselmnextcheb(z, -0.00000000000000184859337734378, b0, b1, b2)
                besselmnextcheb(z, 0.00000000000000634007647740507, b0, b1, b2)
                besselmnextcheb(z, -0.0000000000000222751332699167, b0, b1, b2)
                besselmnextcheb(z, 0.0000000000000803289077536358, b0, b1, b2)
                besselmnextcheb(z, -0.000000000000298009692317273, b0, b1, b2)
                besselmnextcheb(z, 0.00000000000114034058820848, b0, b1, b2)
                besselmnextcheb(z, -0.00000000000451459788337394, b0, b1, b2)
                besselmnextcheb(z, 0.0000000000185594911495472, b0, b1, b2)
                besselmnextcheb(z, -0.0000000000795748924447711, b0, b1, b2)
                besselmnextcheb(z, 0.00000000035773972814003, b0, b1, b2)
                besselmnextcheb(z, -0.00000000169753450938906, b0, b1, b2)
                besselmnextcheb(z, 0.00000000857403401741423, b0, b1, b2)
                besselmnextcheb(z, -0.0000000466048989768795, b0, b1, b2)
                besselmnextcheb(z, 0.000000276681363944501, b0, b1, b2)
                besselmnextcheb(z, -0.00000183175552271912, b0, b1, b2)
                besselmnextcheb(z, 0.0000139498137188765, b0, b1, b2)
                besselmnextcheb(z, -0.000128495495816278, b0, b1, b2)
                besselmnextcheb(z, 0.00156988388573005, b0, b1, b2)
                besselmnextcheb(z, -0.0314481013119645, b0, b1, b2)
                besselmnextcheb(z, 2.44030308206596, b0, b1, b2)
                v = 0.5 * (b0 - b2)
                v = v * System.Math.Exp(-x) / System.Math.sqrt(x)
            End If
            result = v
            Return result
        End Function


        '************************************************************************
        '        Modified Bessel function, second kind, order one
        '
        '        Computes the modified Bessel function of the second kind
        '        of order one of the argument.
        '
        '        The range is partitioned into the two intervals [0,2] and
        '        (2, infinity).  Chebyshev polynomial expansions are employed
        '        in each interval.
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0, 30       30000       1.2e-15     1.6e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function besselk1(x As Double) As Double
            Dim result As Double = 0
            Dim y As Double = 0
            Dim z As Double = 0
            Dim v As Double = 0
            Dim b0 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0

            z = 0.5 * x
            alglib.ap.assert(CDbl(z) > CDbl(0), "Domain error in K1")
            If CDbl(x) <= CDbl(2) Then
                y = x * x - 2.0
                besselm1firstcheb(-7.02386347938629E-18, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000000000242744985051937, b0, b1, b2)
                besselm1nextcheb(y, -0.000000000000666690169419933, b0, b1, b2)
                besselm1nextcheb(y, -0.000000000141148839263353, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000221338763073473, b0, b1, b2)
                besselm1nextcheb(y, -0.00000243340614156597, b0, b1, b2)
                besselm1nextcheb(y, -0.000173028895751305, b0, b1, b2)
                besselm1nextcheb(y, -0.00697572385963986, b0, b1, b2)
                besselm1nextcheb(y, -0.122611180822657, b0, b1, b2)
                besselm1nextcheb(y, -0.353155960776545, b0, b1, b2)
                besselm1nextcheb(y, 1.52530022733895, b0, b1, b2)
                v = 0.5 * (b0 - b2)
                result = System.Math.Log(z) * besseli1(x) + v / x
            Else
                y = 8.0 / x - 2.0
                besselm1firstcheb(-5.75674448366502E-18, b0, b1, b2)
                besselm1nextcheb(y, 1.79405087314756E-17, b0, b1, b2)
                besselm1nextcheb(y, -5.68946255844286E-17, b0, b1, b2)
                besselm1nextcheb(y, 0.000000000000000183809354436664, b0, b1, b2)
                besselm1nextcheb(y, -0.000000000000000605704724837332, b0, b1, b2)
                besselm1nextcheb(y, 0.00000000000000203870316562433, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000000000701983709041831, b0, b1, b2)
                besselm1nextcheb(y, 0.000000000000024771544244813, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000000000897670518232499, b0, b1, b2)
                besselm1nextcheb(y, 0.000000000000334841966607843, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000000128917396095103, b0, b1, b2)
                besselm1nextcheb(y, 0.00000000000513963967348173, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000000212996783842757, b0, b1, b2)
                besselm1nextcheb(y, 0.0000000000921831518760501, b0, b1, b2)
                besselm1nextcheb(y, -0.00000000041903547593419, b0, b1, b2)
                besselm1nextcheb(y, 0.00000000201504975519703, b0, b1, b2)
                besselm1nextcheb(y, -0.0000000103457624656781, b0, b1, b2)
                besselm1nextcheb(y, 0.0000000574108412545005, b0, b1, b2)
                besselm1nextcheb(y, -0.000000350196060308781, b0, b1, b2)
                besselm1nextcheb(y, 0.00000240648494783722, b0, b1, b2)
                besselm1nextcheb(y, -0.0000193619797416608, b0, b1, b2)
                besselm1nextcheb(y, 0.000195215518471352, b0, b1, b2)
                besselm1nextcheb(y, -0.00285781685962278, b0, b1, b2)
                besselm1nextcheb(y, 0.103923736576817, b0, b1, b2)
                besselm1nextcheb(y, 2.72062619048444, b0, b1, b2)
                v = 0.5 * (b0 - b2)
                result = System.Math.Exp(-x) * v / System.Math.sqrt(x)
            End If
            Return result
        End Function


        '************************************************************************
        '        Modified Bessel function, second kind, integer order
        '
        '        Returns modified Bessel function of the second kind
        '        of order n of the argument.
        '
        '        The range is partitioned into the two intervals [0,9.55] and
        '        (9.55, infinity).  An ascending power series is used in the
        '        low range, and an asymptotic expansion in the high range.
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0,30        90000       1.8e-8      3.0e-10
        '
        '        Error is high only near the crossover point x = 9.55
        '        between the two expansions used.
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1988, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function besselkn(nn As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim k As Double = 0
            Dim kf As Double = 0
            Dim nk1f As Double = 0
            Dim nkf As Double = 0
            Dim zn As Double = 0
            Dim t As Double = 0
            Dim s As Double = 0
            Dim z0 As Double = 0
            Dim z As Double = 0
            Dim ans As Double = 0
            Dim fn As Double = 0
            Dim pn As Double = 0
            Dim pk As Double = 0
            Dim zmn As Double = 0
            Dim tlg As Double = 0
            Dim tox As Double = 0
            Dim i As Integer = 0
            Dim n As Integer = 0
            Dim eul As Double = 0

            eul = 0.577215664901533
            If nn < 0 Then
                n = -nn
            Else
                n = nn
            End If
            alglib.ap.assert(n <= 31, "Overflow in BesselKN")
            alglib.ap.assert(CDbl(x) > CDbl(0), "Domain error in BesselKN")
            If CDbl(x) <= CDbl(9.55) Then
                ans = 0.0
                z0 = 0.25 * x * x
                fn = 1.0
                pn = 0.0
                zmn = 1.0
                tox = 2.0 / x
                If n > 0 Then
                    pn = -eul
                    k = 1.0
                    For i = 1 To n - 1
                        pn = pn + 1.0 / k
                        k = k + 1.0
                        fn = fn * k
                    Next
                    zmn = tox
                    If n = 1 Then
                        ans = 1.0 / x
                    Else
                        nk1f = fn / n
                        kf = 1.0
                        s = nk1f
                        z = -z0
                        zn = 1.0
                        For i = 1 To n - 1
                            nk1f = nk1f / (n - i)
                            kf = kf * i
                            zn = zn * z
                            t = nk1f * zn / kf
                            s = s + t
                            alglib.ap.assert(CDbl(Math.maxrealnumber - System.Math.Abs(t)) > CDbl(System.Math.Abs(s)), "Overflow in BesselKN")
                            alglib.ap.assert(Not (CDbl(tox) > CDbl(1.0) AndAlso CDbl(Math.maxrealnumber / tox) < CDbl(zmn)), "Overflow in BesselKN")
                            zmn = zmn * tox
                        Next
                        s = s * 0.5
                        t = System.Math.Abs(s)
                        alglib.ap.assert(Not (CDbl(zmn) > CDbl(1.0) AndAlso CDbl(Math.maxrealnumber / zmn) < CDbl(t)), "Overflow in BesselKN")
                        alglib.ap.assert(Not (CDbl(t) > CDbl(1.0) AndAlso CDbl(Math.maxrealnumber / t) < CDbl(zmn)), "Overflow in BesselKN")
                        ans = s * zmn
                    End If
                End If
                tlg = 2.0 * System.Math.Log(0.5 * x)
                pk = -eul
                If n = 0 Then
                    pn = pk
                    t = 1.0
                Else
                    pn = pn + 1.0 / n
                    t = 1.0 / fn
                End If
                s = (pk + pn - tlg) * t
                k = 1.0
                Do
                    t = t * (z0 / (k * (k + n)))
                    pk = pk + 1.0 / k
                    pn = pn + 1.0 / (k + n)
                    s = s + (pk + pn - tlg) * t
                    k = k + 1.0
                Loop While CDbl(System.Math.Abs(t / s)) > CDbl(Math.machineepsilon)
                s = 0.5 * s / zmn
                If n Mod 2 <> 0 Then
                    s = -s
                End If
                ans = ans + s
                result = ans
                Return result
            End If
            If CDbl(x) > CDbl(System.Math.Log(Math.maxrealnumber)) Then
                result = 0
                Return result
            End If
            k = n
            pn = 4.0 * k * k
            pk = 1.0
            z0 = 8.0 * x
            fn = 1.0
            t = 1.0
            s = t
            nkf = Math.maxrealnumber
            i = 0
            Do
                z = pn - pk * pk
                t = t * z / (fn * z0)
                nk1f = System.Math.Abs(t)
                If i >= n AndAlso CDbl(nk1f) > CDbl(nkf) Then
                    Exit Do
                End If
                nkf = nk1f
                s = s + t
                fn = fn + 1.0
                pk = pk + 2.0
                i = i + 1
            Loop While CDbl(System.Math.Abs(t / s)) > CDbl(Math.machineepsilon)
            result = System.Math.Exp(-x) * System.Math.sqrt(System.Math.PI / (2.0 * x)) * s
            Return result
        End Function


        '************************************************************************
        '        Internal subroutine
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Private Shared Sub besselmfirstcheb(c As Double, ByRef b0 As Double, ByRef b1 As Double, ByRef b2 As Double)
            b0 = c
            b1 = 0.0
            b2 = 0.0
        End Sub


        '************************************************************************
        '        Internal subroutine
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Private Shared Sub besselmnextcheb(x As Double, c As Double, ByRef b0 As Double, ByRef b1 As Double, ByRef b2 As Double)
            b2 = b1
            b1 = b0
            b0 = x * b1 - b2 + c
        End Sub


        '************************************************************************
        '        Internal subroutine
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Private Shared Sub besselm1firstcheb(c As Double, ByRef b0 As Double, ByRef b1 As Double, ByRef b2 As Double)
            b0 = c
            b1 = 0.0
            b2 = 0.0
        End Sub


        '************************************************************************
        '        Internal subroutine
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Private Shared Sub besselm1nextcheb(x As Double, c As Double, ByRef b0 As Double, ByRef b1 As Double, ByRef b2 As Double)
            b2 = b1
            b1 = b0
            b0 = x * b1 - b2 + c
        End Sub


        Private Shared Sub besselasympt0(x As Double, ByRef pzero As Double, ByRef qzero As Double)
            Dim xsq As Double = 0
            Dim p2 As Double = 0
            Dim q2 As Double = 0
            Dim p3 As Double = 0
            Dim q3 As Double = 0

            pzero = 0
            qzero = 0

            xsq = 64.0 / (x * x)
            p2 = 0.0
            p2 = 2485.2719289574 + xsq * p2
            p2 = 153982.653262391 + xsq * p2
            p2 = 2016135.28304998 + xsq * p2
            p2 = 8413041.45655044 + xsq * p2
            p2 = 12332384.7681764 + xsq * p2
            p2 = 5393485.08386944 + xsq * p2
            q2 = 1.0
            q2 = 2615.70073692084 + xsq * q2
            q2 = 156001.727694003 + xsq * q2
            q2 = 2025066.80157013 + xsq * q2
            q2 = 8426449.0506298 + xsq * q2
            q2 = 12338310.2278633 + xsq * q2
            q2 = 5393485.08386944 + xsq * q2
            p3 = -0.0
            p3 = -4.88719939584126 + xsq * p3
            p3 = -226.26306419337 + xsq * p3
            p3 = -2365.95617077911 + xsq * p3
            p3 = -8239.06631348561 + xsq * p3
            p3 = -10381.4169874846 + xsq * p3
            p3 = -3984.61735759522 + xsq * p3
            q3 = 1.0
            q3 = 408.77146739835 + xsq * q3
            q3 = 15704.891915154 + xsq * q3
            q3 = 156021.320667929 + xsq * q3
            q3 = 533291.36342169 + xsq * q3
            q3 = 666745.423931983 + xsq * q3
            q3 = 255015.510886094 + xsq * q3
            pzero = p2 / q2
            qzero = 8 * p3 / q3 / x
        End Sub


        Private Shared Sub besselasympt1(x As Double, ByRef pzero As Double, ByRef qzero As Double)
            Dim xsq As Double = 0
            Dim p2 As Double = 0
            Dim q2 As Double = 0
            Dim p3 As Double = 0
            Dim q3 As Double = 0

            pzero = 0
            qzero = 0

            xsq = 64.0 / (x * x)
            p2 = -1611.61664432461
            p2 = -109824.055434593 + xsq * p2
            p2 = -1523529.35118114 + xsq * p2
            p2 = -6603373.24836494 + xsq * p2
            p2 = -9942246.50507764 + xsq * p2
            p2 = -4435757.81679413 + xsq * p2
            q2 = 1.0
            q2 = -1455.0094401905 + xsq * q2
            q2 = -107263.859911038 + xsq * q2
            q2 = -1511809.50663416 + xsq * q2
            q2 = -6585339.47972309 + xsq * q2
            q2 = -9934124.38993459 + xsq * q2
            q2 = -4435757.81679413 + xsq * q2
            p3 = 35.265133846636
            p3 = 1706.37542902077 + xsq * p3
            p3 = 18494.2628732239 + xsq * p3
            p3 = 66178.8365812708 + xsq * p3
            p3 = 85145.1606753357 + xsq * p3
            p3 = 33220.9134098572 + xsq * p3
            q3 = 1.0
            q3 = 863.836776960499 + xsq * q3
            q3 = 37890.2297457722 + xsq * q3
            q3 = 400294.43582267 + xsq * q3
            q3 = 1419460.66960372 + xsq * q3
            q3 = 1819458.042244 + xsq * q3
            q3 = 708712.819410287 + xsq * q3
            pzero = p2 / q2
            qzero = 8 * p3 / q3 / x
        End Sub


    End Class
    Public Class betaf
        '************************************************************************
        '        Beta function
        '
        '
        '                          -     -
        '                         | (a) | (b)
        '        beta( a, b )  =  -----------.
        '                            -
        '                           | (a+b)
        '
        '        For large arguments the logarithm of the function is
        '        evaluated using lgam(), then exponentiated.
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE       0,30       30000       8.1e-14     1.1e-14
        '
        '        Cephes Math Library Release 2.0:  April, 1987
        '        Copyright 1984, 1987 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function beta(a As Double, b As Double) As Double
            Dim result As Double = 0
            Dim y As Double = 0
            Dim sg As Double = 0
            Dim s As Double = 0

            sg = 1
            alglib.ap.assert(CDbl(a) > CDbl(0) OrElse CDbl(a) <> CDbl(CInt(System.Math.Truncate(System.Math.Floor(a)))), "Overflow in Beta")
            alglib.ap.assert(CDbl(b) > CDbl(0) OrElse CDbl(b) <> CDbl(CInt(System.Math.Truncate(System.Math.Floor(b)))), "Overflow in Beta")
            y = a + b
            If CDbl(System.Math.Abs(y)) > CDbl(171.624376956303) Then
                y = gammafunc.lngamma(y, s)
                sg = sg * s
                y = gammafunc.lngamma(b, s) - y
                sg = sg * s
                y = gammafunc.lngamma(a, s) + y
                sg = sg * s
                alglib.ap.assert(CDbl(y) <= CDbl(System.Math.Log(Math.maxrealnumber)), "Overflow in Beta")
                result = sg * System.Math.Exp(y)
                Return result
            End If
            y = gammafunc.gammafunction(y)
            alglib.ap.assert(CDbl(y) <> CDbl(0), "Overflow in Beta")
            If CDbl(a) > CDbl(b) Then
                y = gammafunc.gammafunction(a) / y
                y = y * gammafunc.gammafunction(b)
            Else
                y = gammafunc.gammafunction(b) / y
                y = y * gammafunc.gammafunction(a)
            End If
            result = y
            Return result
        End Function


    End Class
    Public Class ibetaf
        '************************************************************************
        '        Incomplete beta integral
        '
        '        Returns incomplete beta integral of the arguments, evaluated
        '        from zero to x.  The function is defined as
        '
        '                         x
        '            -            -
        '           | (a+b)      | |  a-1     b-1
        '         -----------    |   t   (1-t)   dt.
        '          -     -     | |
        '         | (a) | (b)   -
        '                        0
        '
        '        The domain of definition is 0 <= x <= 1.  In this
        '        implementation a and b are restricted to positive values.
        '        The integral from x to 1 may be obtained by the symmetry
        '        relation
        '
        '           1 - incbet( a, b, x )  =  incbet( b, a, 1-x ).
        '
        '        The integral is evaluated by a continued fraction expansion
        '        or, when b*x is small, by a power series.
        '
        '        ACCURACY:
        '
        '        Tested at uniformly distributed random points (a,b,x) with a and b
        '        in "domain" and x between 0 and 1.
        '                                               Relative error
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0,5         10000       6.9e-15     4.5e-16
        '           IEEE      0,85       250000       2.2e-13     1.7e-14
        '           IEEE      0,1000      30000       5.3e-12     6.3e-13
        '           IEEE      0,10000    250000       9.3e-11     7.1e-12
        '           IEEE      0,100000    10000       8.7e-10     4.8e-11
        '        Outputs smaller than the IEEE gradual underflow threshold
        '        were excluded from these statistics.
        '
        '        Cephes Math Library, Release 2.8:  June, 2000
        '        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function incompletebeta(a As Double, b As Double, x As Double) As Double
            Dim result As Double = 0
            Dim t As Double = 0
            Dim xc As Double = 0
            Dim w As Double = 0
            Dim y As Double = 0
            Dim flag As Integer = 0
            Dim sg As Double = 0
            Dim big As Double = 0
            Dim biginv As Double = 0
            Dim maxgam As Double = 0
            Dim minlog As Double = 0
            Dim maxlog As Double = 0

            big = 4.5035996273705E+15
            biginv = 0.000000000000000222044604925031
            maxgam = 171.624376956303
            minlog = System.Math.Log(Math.minrealnumber)
            maxlog = System.Math.Log(Math.maxrealnumber)
            alglib.ap.assert(CDbl(a) > CDbl(0) AndAlso CDbl(b) > CDbl(0), "Domain error in IncompleteBeta")
            alglib.ap.assert(CDbl(x) >= CDbl(0) AndAlso CDbl(x) <= CDbl(1), "Domain error in IncompleteBeta")
            If CDbl(x) = CDbl(0) Then
                result = 0
                Return result
            End If
            If CDbl(x) = CDbl(1) Then
                result = 1
                Return result
            End If
            flag = 0
            If CDbl(b * x) <= CDbl(1.0) AndAlso CDbl(x) <= CDbl(0.95) Then
                result = incompletebetaps(a, b, x, maxgam)
                Return result
            End If
            w = 1.0 - x
            If CDbl(x) > CDbl(a / (a + b)) Then
                flag = 1
                t = a
                a = b
                b = t
                xc = x
                x = w
            Else
                xc = w
            End If
            If (flag = 1 AndAlso CDbl(b * x) <= CDbl(1.0)) AndAlso CDbl(x) <= CDbl(0.95) Then
                t = incompletebetaps(a, b, x, maxgam)
                If CDbl(t) <= CDbl(Math.machineepsilon) Then
                    result = 1.0 - Math.machineepsilon
                Else
                    result = 1.0 - t
                End If
                Return result
            End If
            y = x * (a + b - 2.0) - (a - 1.0)
            If CDbl(y) < CDbl(0.0) Then
                w = incompletebetafe(a, b, x, big, biginv)
            Else
                w = incompletebetafe2(a, b, x, big, biginv) / xc
            End If
            y = a * System.Math.Log(x)
            t = b * System.Math.Log(xc)
            If (CDbl(a + b) < CDbl(maxgam) AndAlso CDbl(System.Math.Abs(y)) < CDbl(maxlog)) AndAlso CDbl(System.Math.Abs(t)) < CDbl(maxlog) Then
                t = System.Math.Pow(xc, b)
                t = t * System.Math.Pow(x, a)
                t = t / a
                t = t * w
                t = t * (gammafunc.gammafunction(a + b) / (gammafunc.gammafunction(a) * gammafunc.gammafunction(b)))
                If flag = 1 Then
                    If CDbl(t) <= CDbl(Math.machineepsilon) Then
                        result = 1.0 - Math.machineepsilon
                    Else
                        result = 1.0 - t
                    End If
                Else
                    result = t
                End If
                Return result
            End If
            y = y + t + gammafunc.lngamma(a + b, sg) - gammafunc.lngamma(a, sg) - gammafunc.lngamma(b, sg)
            y = y + System.Math.Log(w / a)
            If CDbl(y) < CDbl(minlog) Then
                t = 0.0
            Else
                t = System.Math.Exp(y)
            End If
            If flag = 1 Then
                If CDbl(t) <= CDbl(Math.machineepsilon) Then
                    t = 1.0 - Math.machineepsilon
                Else
                    t = 1.0 - t
                End If
            End If
            result = t
            Return result
        End Function


        '************************************************************************
        '        Inverse of imcomplete beta integral
        '
        '        Given y, the function finds x such that
        '
        '         incbet( a, b, x ) = y .
        '
        '        The routine performs interval halving or Newton iterations to find the
        '        root of incbet(a,b,x) - y = 0.
        '
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '                       x     a,b
        '        arithmetic   domain  domain  # trials    peak       rms
        '           IEEE      0,1    .5,10000   50000    5.8e-12   1.3e-13
        '           IEEE      0,1   .25,100    100000    1.8e-13   3.9e-15
        '           IEEE      0,1     0,5       50000    1.1e-12   5.5e-15
        '        With a and b constrained to half-integer or integer values:
        '           IEEE      0,1    .5,10000   50000    5.8e-12   1.1e-13
        '           IEEE      0,1    .5,100    100000    1.7e-14   7.9e-16
        '        With a = .5, b constrained to half-integer or integer values:
        '           IEEE      0,1    .5,10000   10000    8.3e-11   1.0e-11
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1996, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function invincompletebeta(a As Double, b As Double, y As Double) As Double
            Dim result As Double = 0
            Dim aaa As Double = 0
            Dim bbb As Double = 0
            Dim y0 As Double = 0
            Dim d As Double = 0
            Dim yyy As Double = 0
            Dim x As Double = 0
            Dim x0 As Double = 0
            Dim x1 As Double = 0
            Dim lgm As Double = 0
            Dim yp As Double = 0
            Dim di As Double = 0
            Dim dithresh As Double = 0
            Dim yl As Double = 0
            Dim yh As Double = 0
            Dim xt As Double = 0
            Dim i As Integer = 0
            Dim rflg As Integer = 0
            Dim dir As Integer = 0
            Dim nflg As Integer = 0
            Dim s As Double = 0
            Dim mainlooppos As Integer = 0
            Dim ihalve As Integer = 0
            Dim ihalvecycle As Integer = 0
            Dim newt As Integer = 0
            Dim newtcycle As Integer = 0
            Dim breaknewtcycle As Integer = 0
            Dim breakihalvecycle As Integer = 0

            i = 0
            alglib.ap.assert(CDbl(y) >= CDbl(0) AndAlso CDbl(y) <= CDbl(1), "Domain error in InvIncompleteBeta")

            '
            ' special cases
            '
            If CDbl(y) = CDbl(0) Then
                result = 0
                Return result
            End If
            If CDbl(y) = CDbl(1.0) Then
                result = 1
                Return result
            End If

            '
            ' these initializations are not really necessary,
            ' but without them compiler complains about 'possibly uninitialized variables'.
            '
            dithresh = 0
            rflg = 0
            aaa = 0
            bbb = 0
            y0 = 0
            x = 0
            yyy = 0
            lgm = 0
            dir = 0
            di = 0

            '
            ' normal initializations
            '
            x0 = 0.0
            yl = 0.0
            x1 = 1.0
            yh = 1.0
            nflg = 0
            mainlooppos = 0
            ihalve = 1
            ihalvecycle = 2
            newt = 3
            newtcycle = 4
            breaknewtcycle = 5
            breakihalvecycle = 6

            '
            ' main loop
            '
            While True

                '
                ' start
                '
                If mainlooppos = 0 Then
                    If CDbl(a) <= CDbl(1.0) OrElse CDbl(b) <= CDbl(1.0) Then
                        dithresh = 0.000001
                        rflg = 0
                        aaa = a
                        bbb = b
                        y0 = y
                        x = aaa / (aaa + bbb)
                        yyy = incompletebeta(aaa, bbb, x)
                        mainlooppos = ihalve
                        Continue While
                    Else
                        dithresh = 0.0001
                    End If
                    yp = -normaldistr.invnormaldistribution(y)
                    If CDbl(y) > CDbl(0.5) Then
                        rflg = 1
                        aaa = b
                        bbb = a
                        y0 = 1.0 - y
                        yp = -yp
                    Else
                        rflg = 0
                        aaa = a
                        bbb = b
                        y0 = y
                    End If
                    lgm = (yp * yp - 3.0) / 6.0
                    x = 2.0 / (1.0 / (2.0 * aaa - 1.0) + 1.0 / (2.0 * bbb - 1.0))
                    d = yp * System.Math.sqrt(x + lgm) / x - (1.0 / (2.0 * bbb - 1.0) - 1.0 / (2.0 * aaa - 1.0)) * (lgm + 5.0 / 6.0 - 2.0 / (3.0 * x))
                    d = 2.0 * d
                    If CDbl(d) < CDbl(System.Math.Log(Math.minrealnumber)) Then
                        x = 0
                        Exit While
                    End If
                    x = aaa / (aaa + bbb * System.Math.Exp(d))
                    yyy = incompletebeta(aaa, bbb, x)
                    yp = (yyy - y0) / y0
                    If CDbl(System.Math.Abs(yp)) < CDbl(0.2) Then
                        mainlooppos = newt
                        Continue While
                    End If
                    mainlooppos = ihalve
                    Continue While
                End If

                '
                ' ihalve
                '
                If mainlooppos = ihalve Then
                    dir = 0
                    di = 0.5
                    i = 0
                    mainlooppos = ihalvecycle
                    Continue While
                End If

                '
                ' ihalvecycle
                '
                If mainlooppos = ihalvecycle Then
                    If i <= 99 Then
                        If i <> 0 Then
                            x = x0 + di * (x1 - x0)
                            If CDbl(x) = CDbl(1.0) Then
                                x = 1.0 - Math.machineepsilon
                            End If
                            If CDbl(x) = CDbl(0.0) Then
                                di = 0.5
                                x = x0 + di * (x1 - x0)
                                If CDbl(x) = CDbl(0.0) Then
                                    Exit While
                                End If
                            End If
                            yyy = incompletebeta(aaa, bbb, x)
                            yp = (x1 - x0) / (x1 + x0)
                            If CDbl(System.Math.Abs(yp)) < CDbl(dithresh) Then
                                mainlooppos = newt
                                Continue While
                            End If
                            yp = (yyy - y0) / y0
                            If CDbl(System.Math.Abs(yp)) < CDbl(dithresh) Then
                                mainlooppos = newt
                                Continue While
                            End If
                        End If
                        If CDbl(yyy) < CDbl(y0) Then
                            x0 = x
                            yl = yyy
                            If dir < 0 Then
                                dir = 0
                                di = 0.5
                            Else
                                If dir > 3 Then
                                    di = 1.0 - (1.0 - di) * (1.0 - di)
                                Else
                                    If dir > 1 Then
                                        di = 0.5 * di + 0.5
                                    Else
                                        di = (y0 - yyy) / (yh - yl)
                                    End If
                                End If
                            End If
                            dir = dir + 1
                            If CDbl(x0) > CDbl(0.75) Then
                                If rflg = 1 Then
                                    rflg = 0
                                    aaa = a
                                    bbb = b
                                    y0 = y
                                Else
                                    rflg = 1
                                    aaa = b
                                    bbb = a
                                    y0 = 1.0 - y
                                End If
                                x = 1.0 - x
                                yyy = incompletebeta(aaa, bbb, x)
                                x0 = 0.0
                                yl = 0.0
                                x1 = 1.0
                                yh = 1.0
                                mainlooppos = ihalve
                                Continue While
                            End If
                        Else
                            x1 = x
                            If rflg = 1 AndAlso CDbl(x1) < CDbl(Math.machineepsilon) Then
                                x = 0.0
                                Exit While
                            End If
                            yh = yyy
                            If dir > 0 Then
                                dir = 0
                                di = 0.5
                            Else
                                If dir < -3 Then
                                    di = di * di
                                Else
                                    If dir < -1 Then
                                        di = 0.5 * di
                                    Else
                                        di = (yyy - y0) / (yh - yl)
                                    End If
                                End If
                            End If
                            dir = dir - 1
                        End If
                        i = i + 1
                        mainlooppos = ihalvecycle
                        Continue While
                    Else
                        mainlooppos = breakihalvecycle
                        Continue While
                    End If
                End If

                '
                ' breakihalvecycle
                '
                If mainlooppos = breakihalvecycle Then
                    If CDbl(x0) >= CDbl(1.0) Then
                        x = 1.0 - Math.machineepsilon
                        Exit While
                    End If
                    If CDbl(x) <= CDbl(0.0) Then
                        x = 0.0
                        Exit While
                    End If
                    mainlooppos = newt
                    Continue While
                End If

                '
                ' newt
                '
                If mainlooppos = newt Then
                    If nflg <> 0 Then
                        Exit While
                    End If
                    nflg = 1
                    lgm = gammafunc.lngamma(aaa + bbb, s) - gammafunc.lngamma(aaa, s) - gammafunc.lngamma(bbb, s)
                    i = 0
                    mainlooppos = newtcycle
                    Continue While
                End If

                '
                ' newtcycle
                '
                If mainlooppos = newtcycle Then
                    If i <= 7 Then
                        If i <> 0 Then
                            yyy = incompletebeta(aaa, bbb, x)
                        End If
                        If CDbl(yyy) < CDbl(yl) Then
                            x = x0
                            yyy = yl
                        Else
                            If CDbl(yyy) > CDbl(yh) Then
                                x = x1
                                yyy = yh
                            Else
                                If CDbl(yyy) < CDbl(y0) Then
                                    x0 = x
                                    yl = yyy
                                Else
                                    x1 = x
                                    yh = yyy
                                End If
                            End If
                        End If
                        If CDbl(x) = CDbl(1.0) OrElse CDbl(x) = CDbl(0.0) Then
                            mainlooppos = breaknewtcycle
                            Continue While
                        End If
                        d = (aaa - 1.0) * System.Math.Log(x) + (bbb - 1.0) * System.Math.Log(1.0 - x) + lgm
                        If CDbl(d) < CDbl(System.Math.Log(Math.minrealnumber)) Then
                            Exit While
                        End If
                        If CDbl(d) > CDbl(System.Math.Log(Math.maxrealnumber)) Then
                            mainlooppos = breaknewtcycle
                            Continue While
                        End If
                        d = System.Math.Exp(d)
                        d = (yyy - y0) / d
                        xt = x - d
                        If CDbl(xt) <= CDbl(x0) Then
                            yyy = (x - x0) / (x1 - x0)
                            xt = x0 + 0.5 * yyy * (x - x0)
                            If CDbl(xt) <= CDbl(0.0) Then
                                mainlooppos = breaknewtcycle
                                Continue While
                            End If
                        End If
                        If CDbl(xt) >= CDbl(x1) Then
                            yyy = (x1 - x) / (x1 - x0)
                            xt = x1 - 0.5 * yyy * (x1 - x)
                            If CDbl(xt) >= CDbl(1.0) Then
                                mainlooppos = breaknewtcycle
                                Continue While
                            End If
                        End If
                        x = xt
                        If CDbl(System.Math.Abs(d / x)) < CDbl(128.0 * Math.machineepsilon) Then
                            Exit While
                        End If
                        i = i + 1
                        mainlooppos = newtcycle
                        Continue While
                    Else
                        mainlooppos = breaknewtcycle
                        Continue While
                    End If
                End If

                '
                ' breaknewtcycle
                '
                If mainlooppos = breaknewtcycle Then
                    dithresh = 256.0 * Math.machineepsilon
                    mainlooppos = ihalve
                    Continue While
                End If
            End While

            '
            ' done
            '
            If rflg <> 0 Then
                If CDbl(x) <= CDbl(Math.machineepsilon) Then
                    x = 1.0 - Math.machineepsilon
                Else
                    x = 1.0 - x
                End If
            End If
            result = x
            Return result
        End Function


        '************************************************************************
        '        Continued fraction expansion #1 for incomplete beta integral
        '
        '        Cephes Math Library, Release 2.8:  June, 2000
        '        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Private Shared Function incompletebetafe(a As Double, b As Double, x As Double, big As Double, biginv As Double) As Double
            Dim result As Double = 0
            Dim xk As Double = 0
            Dim pk As Double = 0
            Dim pkm1 As Double = 0
            Dim pkm2 As Double = 0
            Dim qk As Double = 0
            Dim qkm1 As Double = 0
            Dim qkm2 As Double = 0
            Dim k1 As Double = 0
            Dim k2 As Double = 0
            Dim k3 As Double = 0
            Dim k4 As Double = 0
            Dim k5 As Double = 0
            Dim k6 As Double = 0
            Dim k7 As Double = 0
            Dim k8 As Double = 0
            Dim r As Double = 0
            Dim t As Double = 0
            Dim ans As Double = 0
            Dim thresh As Double = 0
            Dim n As Integer = 0

            k1 = a
            k2 = a + b
            k3 = a
            k4 = a + 1.0
            k5 = 1.0
            k6 = b - 1.0
            k7 = k4
            k8 = a + 2.0
            pkm2 = 0.0
            qkm2 = 1.0
            pkm1 = 1.0
            qkm1 = 1.0
            ans = 1.0
            r = 1.0
            n = 0
            thresh = 3.0 * Math.machineepsilon
            Do
                xk = -(x * k1 * k2 / (k3 * k4))
                pk = pkm1 + pkm2 * xk
                qk = qkm1 + qkm2 * xk
                pkm2 = pkm1
                pkm1 = pk
                qkm2 = qkm1
                qkm1 = qk
                xk = x * k5 * k6 / (k7 * k8)
                pk = pkm1 + pkm2 * xk
                qk = qkm1 + qkm2 * xk
                pkm2 = pkm1
                pkm1 = pk
                qkm2 = qkm1
                qkm1 = qk
                If CDbl(qk) <> CDbl(0) Then
                    r = pk / qk
                End If
                If CDbl(r) <> CDbl(0) Then
                    t = System.Math.Abs((ans - r) / r)
                    ans = r
                Else
                    t = 1.0
                End If
                If CDbl(t) < CDbl(thresh) Then
                    Exit Do
                End If
                k1 = k1 + 1.0
                k2 = k2 + 1.0
                k3 = k3 + 2.0
                k4 = k4 + 2.0
                k5 = k5 + 1.0
                k6 = k6 - 1.0
                k7 = k7 + 2.0
                k8 = k8 + 2.0
                If CDbl(System.Math.Abs(qk) + System.Math.Abs(pk)) > CDbl(big) Then
                    pkm2 = pkm2 * biginv
                    pkm1 = pkm1 * biginv
                    qkm2 = qkm2 * biginv
                    qkm1 = qkm1 * biginv
                End If
                If CDbl(System.Math.Abs(qk)) < CDbl(biginv) OrElse CDbl(System.Math.Abs(pk)) < CDbl(biginv) Then
                    pkm2 = pkm2 * big
                    pkm1 = pkm1 * big
                    qkm2 = qkm2 * big
                    qkm1 = qkm1 * big
                End If
                n = n + 1
            Loop While n <> 300
            result = ans
            Return result
        End Function


        '************************************************************************
        '        Continued fraction expansion #2
        '        for incomplete beta integral
        '
        '        Cephes Math Library, Release 2.8:  June, 2000
        '        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Private Shared Function incompletebetafe2(a As Double, b As Double, x As Double, big As Double, biginv As Double) As Double
            Dim result As Double = 0
            Dim xk As Double = 0
            Dim pk As Double = 0
            Dim pkm1 As Double = 0
            Dim pkm2 As Double = 0
            Dim qk As Double = 0
            Dim qkm1 As Double = 0
            Dim qkm2 As Double = 0
            Dim k1 As Double = 0
            Dim k2 As Double = 0
            Dim k3 As Double = 0
            Dim k4 As Double = 0
            Dim k5 As Double = 0
            Dim k6 As Double = 0
            Dim k7 As Double = 0
            Dim k8 As Double = 0
            Dim r As Double = 0
            Dim t As Double = 0
            Dim ans As Double = 0
            Dim z As Double = 0
            Dim thresh As Double = 0
            Dim n As Integer = 0

            k1 = a
            k2 = b - 1.0
            k3 = a
            k4 = a + 1.0
            k5 = 1.0
            k6 = a + b
            k7 = a + 1.0
            k8 = a + 2.0
            pkm2 = 0.0
            qkm2 = 1.0
            pkm1 = 1.0
            qkm1 = 1.0
            z = x / (1.0 - x)
            ans = 1.0
            r = 1.0
            n = 0
            thresh = 3.0 * Math.machineepsilon
            Do
                xk = -(z * k1 * k2 / (k3 * k4))
                pk = pkm1 + pkm2 * xk
                qk = qkm1 + qkm2 * xk
                pkm2 = pkm1
                pkm1 = pk
                qkm2 = qkm1
                qkm1 = qk
                xk = z * k5 * k6 / (k7 * k8)
                pk = pkm1 + pkm2 * xk
                qk = qkm1 + qkm2 * xk
                pkm2 = pkm1
                pkm1 = pk
                qkm2 = qkm1
                qkm1 = qk
                If CDbl(qk) <> CDbl(0) Then
                    r = pk / qk
                End If
                If CDbl(r) <> CDbl(0) Then
                    t = System.Math.Abs((ans - r) / r)
                    ans = r
                Else
                    t = 1.0
                End If
                If CDbl(t) < CDbl(thresh) Then
                    Exit Do
                End If
                k1 = k1 + 1.0
                k2 = k2 - 1.0
                k3 = k3 + 2.0
                k4 = k4 + 2.0
                k5 = k5 + 1.0
                k6 = k6 + 1.0
                k7 = k7 + 2.0
                k8 = k8 + 2.0
                If CDbl(System.Math.Abs(qk) + System.Math.Abs(pk)) > CDbl(big) Then
                    pkm2 = pkm2 * biginv
                    pkm1 = pkm1 * biginv
                    qkm2 = qkm2 * biginv
                    qkm1 = qkm1 * biginv
                End If
                If CDbl(System.Math.Abs(qk)) < CDbl(biginv) OrElse CDbl(System.Math.Abs(pk)) < CDbl(biginv) Then
                    pkm2 = pkm2 * big
                    pkm1 = pkm1 * big
                    qkm2 = qkm2 * big
                    qkm1 = qkm1 * big
                End If
                n = n + 1
            Loop While n <> 300
            result = ans
            Return result
        End Function


        '************************************************************************
        '        Power series for incomplete beta integral.
        '        Use when b*x is small and x not too close to 1.
        '
        '        Cephes Math Library, Release 2.8:  June, 2000
        '        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Private Shared Function incompletebetaps(a As Double, b As Double, x As Double, maxgam As Double) As Double
            Dim result As Double = 0
            Dim s As Double = 0
            Dim t As Double = 0
            Dim u As Double = 0
            Dim v As Double = 0
            Dim n As Double = 0
            Dim t1 As Double = 0
            Dim z As Double = 0
            Dim ai As Double = 0
            Dim sg As Double = 0

            ai = 1.0 / a
            u = (1.0 - b) * x
            v = u / (a + 1.0)
            t1 = v
            t = u
            n = 2.0
            s = 0.0
            z = Math.machineepsilon * ai
            While CDbl(System.Math.Abs(v)) > CDbl(z)
                u = (n - b) * x / n
                t = t * u
                v = t / (a + n)
                s = s + v
                n = n + 1.0
            End While
            s = s + t1
            s = s + ai
            u = a * System.Math.Log(x)
            If CDbl(a + b) < CDbl(maxgam) AndAlso CDbl(System.Math.Abs(u)) < CDbl(System.Math.Log(Math.maxrealnumber)) Then
                t = gammafunc.gammafunction(a + b) / (gammafunc.gammafunction(a) * gammafunc.gammafunction(b))
                s = s * t * System.Math.Pow(x, a)
            Else
                t = gammafunc.lngamma(a + b, sg) - gammafunc.lngamma(a, sg) - gammafunc.lngamma(b, sg) + u + System.Math.Log(s)
                If CDbl(t) < CDbl(System.Math.Log(Math.minrealnumber)) Then
                    s = 0.0
                Else
                    s = System.Math.Exp(t)
                End If
            End If
            result = s
            Return result
        End Function


    End Class
    Public Class binomialdistr
        '************************************************************************
        '        Binomial distribution
        '
        '        Returns the sum of the terms 0 through k of the Binomial
        '        probability density:
        '
        '          k
        '          --  ( n )   j      n-j
        '          >   (   )  p  (1-p)
        '          --  ( j )
        '         j=0
        '
        '        The terms are not summed directly; instead the incomplete
        '        beta integral is employed, according to the formula
        '
        '        y = bdtr( k, n, p ) = incbet( n-k, k+1, 1-p ).
        '
        '        The arguments must be positive, with p ranging from 0 to 1.
        '
        '        ACCURACY:
        '
        '        Tested at random points (a,b,p), with p between 0 and 1.
        '
        '                      a,b                     Relative error:
        '        arithmetic  domain     # trials      peak         rms
        '         For p between 0.001 and 1:
        '           IEEE     0,100       100000      4.3e-15     2.6e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function binomialdistribution(k As Integer, n As Integer, p As Double) As Double
            Dim result As Double = 0
            Dim dk As Double = 0
            Dim dn As Double = 0

            alglib.ap.assert(CDbl(p) >= CDbl(0) AndAlso CDbl(p) <= CDbl(1), "Domain error in BinomialDistribution")
            alglib.ap.assert(k >= -1 AndAlso k <= n, "Domain error in BinomialDistribution")
            If k = -1 Then
                result = 0
                Return result
            End If
            If k = n Then
                result = 1
                Return result
            End If
            dn = n - k
            If k = 0 Then
                dk = System.Math.Pow(1.0 - p, dn)
            Else
                dk = k + 1
                dk = ibetaf.incompletebeta(dn, dk, 1.0 - p)
            End If
            result = dk
            Return result
        End Function


        '************************************************************************
        '        Complemented binomial distribution
        '
        '        Returns the sum of the terms k+1 through n of the Binomial
        '        probability density:
        '
        '          n
        '          --  ( n )   j      n-j
        '          >   (   )  p  (1-p)
        '          --  ( j )
        '         j=k+1
        '
        '        The terms are not summed directly; instead the incomplete
        '        beta integral is employed, according to the formula
        '
        '        y = bdtrc( k, n, p ) = incbet( k+1, n-k, p ).
        '
        '        The arguments must be positive, with p ranging from 0 to 1.
        '
        '        ACCURACY:
        '
        '        Tested at random points (a,b,p).
        '
        '                      a,b                     Relative error:
        '        arithmetic  domain     # trials      peak         rms
        '         For p between 0.001 and 1:
        '           IEEE     0,100       100000      6.7e-15     8.2e-16
        '         For p between 0 and .001:
        '           IEEE     0,100       100000      1.5e-13     2.7e-15
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function binomialcdistribution(k As Integer, n As Integer, p As Double) As Double
            Dim result As Double = 0
            Dim dk As Double = 0
            Dim dn As Double = 0

            alglib.ap.assert(CDbl(p) >= CDbl(0) AndAlso CDbl(p) <= CDbl(1), "Domain error in BinomialDistributionC")
            alglib.ap.assert(k >= -1 AndAlso k <= n, "Domain error in BinomialDistributionC")
            If k = -1 Then
                result = 1
                Return result
            End If
            If k = n Then
                result = 0
                Return result
            End If
            dn = n - k
            If k = 0 Then
                If CDbl(p) < CDbl(0.01) Then
                    dk = -nearunityunit.nuexpm1(dn * nearunityunit.nulog1p(-p))
                Else
                    dk = 1.0 - System.Math.Pow(1.0 - p, dn)
                End If
            Else
                dk = k + 1
                dk = ibetaf.incompletebeta(dk, dn, p)
            End If
            result = dk
            Return result
        End Function


        '************************************************************************
        '        Inverse binomial distribution
        '
        '        Finds the event probability p such that the sum of the
        '        terms 0 through k of the Binomial probability density
        '        is equal to the given cumulative probability y.
        '
        '        This is accomplished using the inverse beta integral
        '        function and the relation
        '
        '        1 - p = incbi( n-k, k+1, y ).
        '
        '        ACCURACY:
        '
        '        Tested at random points (a,b,p).
        '
        '                      a,b                     Relative error:
        '        arithmetic  domain     # trials      peak         rms
        '         For p between 0.001 and 1:
        '           IEEE     0,100       100000      2.3e-14     6.4e-16
        '           IEEE     0,10000     100000      6.6e-12     1.2e-13
        '         For p between 10^-6 and 0.001:
        '           IEEE     0,100       100000      2.0e-12     1.3e-14
        '           IEEE     0,10000     100000      1.5e-12     3.2e-14
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function invbinomialdistribution(k As Integer, n As Integer, y As Double) As Double
            Dim result As Double = 0
            Dim dk As Double = 0
            Dim dn As Double = 0
            Dim p As Double = 0

            alglib.ap.assert(k >= 0 AndAlso k < n, "Domain error in InvBinomialDistribution")
            dn = n - k
            If k = 0 Then
                If CDbl(y) > CDbl(0.8) Then
                    p = -nearunityunit.nuexpm1(nearunityunit.nulog1p(y - 1.0) / dn)
                Else
                    p = 1.0 - System.Math.Pow(y, 1.0 / dn)
                End If
            Else
                dk = k + 1
                p = ibetaf.incompletebeta(dn, dk, 0.5)
                If CDbl(p) > CDbl(0.5) Then
                    p = ibetaf.invincompletebeta(dk, dn, 1.0 - y)
                Else
                    p = 1.0 - ibetaf.invincompletebeta(dn, dk, y)
                End If
            End If
            result = p
            Return result
        End Function


    End Class
    Public Class chebyshev
        '************************************************************************
        '        Calculation of the value of the Chebyshev polynomials of the
        '        first and second kinds.
        '
        '        Parameters:
        '            r   -   polynomial kind, either 1 or 2.
        '            n   -   degree, n>=0
        '            x   -   argument, -1 <= x <= 1
        '
        '        Result:
        '            the value of the Chebyshev polynomial at x
        '        ************************************************************************

        Public Shared Function chebyshevcalculate(r As Integer, n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim i As Integer = 0
            Dim a As Double = 0
            Dim b As Double = 0

            result = 0

            '
            ' Prepare A and B
            '
            If r = 1 Then
                a = 1
                b = x
            Else
                a = 1
                b = 2 * x
            End If

            '
            ' Special cases: N=0 or N=1
            '
            If n = 0 Then
                result = a
                Return result
            End If
            If n = 1 Then
                result = b
                Return result
            End If

            '
            ' General case: N>=2
            '
            For i = 2 To n
                result = 2 * x * b - a
                a = b
                b = result
            Next
            Return result
        End Function


        '************************************************************************
        '        Summation of Chebyshev polynomials using Clenshaw抯 recurrence formula.
        '
        '        This routine calculates
        '            c[0]*T0(x) + c[1]*T1(x) + ... + c[N]*TN(x)
        '        or
        '            c[0]*U0(x) + c[1]*U1(x) + ... + c[N]*UN(x)
        '        depending on the R.
        '
        '        Parameters:
        '            r   -   polynomial kind, either 1 or 2.
        '            n   -   degree, n>=0
        '            x   -   argument
        '
        '        Result:
        '            the value of the Chebyshev polynomial at x
        '        ************************************************************************

        Public Shared Function chebyshevsum(c As Double(), r As Integer, n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0
            Dim i As Integer = 0

            b1 = 0
            b2 = 0
            For i = n To 1 Step -1
                result = 2 * x * b1 - b2 + c(i)
                b2 = b1
                b1 = result
            Next
            If r = 1 Then
                result = -b2 + x * b1 + c(0)
            Else
                result = -b2 + 2 * x * b1 + c(0)
            End If
            Return result
        End Function


        '************************************************************************
        '        Representation of Tn as C[0] + C[1]*X + ... + C[N]*X^N
        '
        '        Input parameters:
        '            N   -   polynomial degree, n>=0
        '
        '        Output parameters:
        '            C   -   coefficients
        '        ************************************************************************

        Public Shared Sub chebyshevcoefficients(n As Integer, ByRef c As Double())
            Dim i As Integer = 0

            c = New Double(-1) {}

            c = New Double(n) {}
            For i = 0 To n
                c(i) = 0
            Next
            If n = 0 OrElse n = 1 Then
                c(n) = 1
            Else
                c(n) = System.Math.Exp((n - 1) * System.Math.Log(2))
                For i = 0 To n \ 2 - 1
                    c(n - 2 * (i + 1)) = -(c(n - 2 * i) * (n - 2 * i) * (n - 2 * i - 1) / 4 / (i + 1) / (n - i - 1))
                Next
            End If
        End Sub


        '************************************************************************
        '        Conversion of a series of Chebyshev polynomials to a power series.
        '
        '        Represents A[0]*T0(x) + A[1]*T1(x) + ... + A[N]*Tn(x) as
        '        B[0] + B[1]*X + ... + B[N]*X^N.
        '
        '        Input parameters:
        '            A   -   Chebyshev series coefficients
        '            N   -   degree, N>=0
        '            
        '        Output parameters
        '            B   -   power series coefficients
        '        ************************************************************************

        Public Shared Sub fromchebyshev(a As Double(), n As Integer, ByRef b As Double())
            Dim i As Integer = 0
            Dim k As Integer = 0
            Dim e As Double = 0
            Dim d As Double = 0

            b = New Double(-1) {}

            b = New Double(n) {}
            For i = 0 To n
                b(i) = 0
            Next
            d = 0
            i = 0
            Do
                k = i
                Do
                    e = b(k)
                    b(k) = 0
                    If i <= 1 AndAlso k = i Then
                        b(k) = 1
                    Else
                        If i <> 0 Then
                            b(k) = 2 * d
                        End If
                        If k > i + 1 Then
                            b(k) = b(k) - b(k - 2)
                        End If
                    End If
                    d = e
                    k = k + 1
                Loop While k <= n
                d = b(i)
                e = 0
                k = i
                While k <= n
                    e = e + b(k) * a(k)
                    k = k + 2
                End While
                b(i) = e
                i = i + 1
            Loop While i <= n
        End Sub


    End Class
    Public Class chisquaredistr
        '************************************************************************
        '        Chi-square distribution
        '
        '        Returns the area under the left hand tail (from 0 to x)
        '        of the Chi square probability density function with
        '        v degrees of freedom.
        '
        '
        '                                          x
        '                                           -
        '                               1          | |  v/2-1  -t/2
        '         P( x | v )   =   -----------     |   t      e     dt
        '                           v/2  -       | |
        '                          2    | (v/2)   -
        '                                          0
        '
        '        where x is the Chi-square variable.
        '
        '        The incomplete gamma integral is used, according to the
        '        formula
        '
        '        y = chdtr( v, x ) = igam( v/2.0, x/2.0 ).
        '
        '        The arguments must both be positive.
        '
        '        ACCURACY:
        '
        '        See incomplete gamma function
        '
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function chisquaredistribution(v As Double, x As Double) As Double
            Dim result As Double = 0

            alglib.ap.assert(CDbl(x) >= CDbl(0) AndAlso CDbl(v) >= CDbl(1), "Domain error in ChiSquareDistribution")
            result = igammaf.incompletegamma(v / 2.0, x / 2.0)
            Return result
        End Function


        '************************************************************************
        '        Complemented Chi-square distribution
        '
        '        Returns the area under the right hand tail (from x to
        '        infinity) of the Chi square probability density function
        '        with v degrees of freedom:
        '
        '                                         inf.
        '                                           -
        '                               1          | |  v/2-1  -t/2
        '         P( x | v )   =   -----------     |   t      e     dt
        '                           v/2  -       | |
        '                          2    | (v/2)   -
        '                                          x
        '
        '        where x is the Chi-square variable.
        '
        '        The incomplete gamma integral is used, according to the
        '        formula
        '
        '        y = chdtr( v, x ) = igamc( v/2.0, x/2.0 ).
        '
        '        The arguments must both be positive.
        '
        '        ACCURACY:
        '
        '        See incomplete gamma function
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function chisquarecdistribution(v As Double, x As Double) As Double
            Dim result As Double = 0

            alglib.ap.assert(CDbl(x) >= CDbl(0) AndAlso CDbl(v) >= CDbl(1), "Domain error in ChiSquareDistributionC")
            result = igammaf.incompletegammac(v / 2.0, x / 2.0)
            Return result
        End Function


        '************************************************************************
        '        Inverse of complemented Chi-square distribution
        '
        '        Finds the Chi-square argument x such that the integral
        '        from x to infinity of the Chi-square density is equal
        '        to the given cumulative probability y.
        '
        '        This is accomplished using the inverse gamma integral
        '        function and the relation
        '
        '           x/2 = igami( df/2, y );
        '
        '        ACCURACY:
        '
        '        See inverse incomplete gamma function
        '
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function invchisquaredistribution(v As Double, y As Double) As Double
            Dim result As Double = 0

            alglib.ap.assert((CDbl(y) >= CDbl(0) AndAlso CDbl(y) <= CDbl(1)) AndAlso CDbl(v) >= CDbl(1), "Domain error in InvChiSquareDistribution")
            result = 2 * igammaf.invincompletegammac(0.5 * v, y)
            Return result
        End Function


    End Class
    Public Class dawson
        '************************************************************************
        '        Dawson's Integral
        '
        '        Approximates the integral
        '
        '                                    x
        '                                    -
        '                             2     | |        2
        '         dawsn(x)  =  exp( -x  )   |    exp( t  ) dt
        '                                 | |
        '                                  -
        '                                  0
        '
        '        Three different rational approximations are employed, for
        '        the intervals 0 to 3.25; 3.25 to 6.25; and 6.25 up.
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0,10        10000       6.9e-16     1.0e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function dawsonintegral(x As Double) As Double
            Dim result As Double = 0
            Dim x2 As Double = 0
            Dim y As Double = 0
            Dim sg As Integer = 0
            Dim an As Double = 0
            Dim ad As Double = 0
            Dim bn As Double = 0
            Dim bd As Double = 0
            Dim cn As Double = 0
            Dim cd As Double = 0

            sg = 1
            If CDbl(x) < CDbl(0) Then
                sg = -1
                x = -x
            End If
            If CDbl(x) < CDbl(3.25) Then
                x2 = x * x
                an = 0.0000000000113681498971756
                an = an * x2 + 0.000000000849262267667474
                an = an * x2 + 0.0000000194434204175553
                an = an * x2 + 0.000000953151741254484
                an = an * x2 + 0.00000307828309874913
                an = an * x2 + 0.000352513368520289
                an = an * x2 + -0.000850149846724411
                an = an * x2 + 0.0422618223005547
                an = an * x2 + -0.0917480371773452
                an = an * x2 + 1.0
                ad = 0.0000000000240372073066763
                ad = ad * x2 + 0.00000000148864681368493
                ad = ad * x2 + 0.0000000521265281010542
                ad = ad * x2 + 0.00000127258478273187
                ad = ad * x2 + 0.000023249024982079
                ad = ad * x2 + 0.000325524741826058
                ad = ad * x2 + 0.00348805814657163
                ad = ad * x2 + 0.0279448531198829
                ad = ad * x2 + 0.158874241960121
                ad = ad * x2 + 0.57491862948932
                ad = ad * x2 + 1.0
                y = x * an / ad
                result = sg * y
                Return result
            End If
            x2 = 1.0 / (x * x)
            If CDbl(x) < CDbl(6.25) Then
                bn = 0.508955156417901
                bn = bn * x2 - 0.244754418142698
                bn = bn * x2 + 0.0941512335303534
                bn = bn * x2 - 0.0218711255142039
                bn = bn * x2 + 0.00366207612329569
                bn = bn * x2 - 0.000423209114460389
                bn = bn * x2 + 0.0000359641304793897
                bn = bn * x2 - 0.00000214640351719969
                bn = bn * x2 + 0.0000000910010780076391
                bn = bn * x2 - 0.00000000240274520828251
                bn = bn * x2 + 0.0000000000359233385440928
                bd = 1.0
                bd = bd * x2 - 0.631839869873368
                bd = bd * x2 + 0.236706788228249
                bd = bd * x2 - 0.0531806367003223
                bd = bd * x2 + 0.00848041718586295
                bd = bd * x2 - 0.000947996768486665
                bd = bd * x2 + 0.0000781025592944552
                bd = bd * x2 - 0.00000455875153252443
                bd = bd * x2 + 0.000000189100358111422
                bd = bd * x2 - 0.00000000491324691331921
                bd = bd * x2 + 0.0000000000718466403235735
                y = 1.0 / x + x2 * bn / (bd * x)
                result = sg * 0.5 * y
                Return result
            End If
            If CDbl(x) > CDbl(1000000000.0) Then
                result = sg * 0.5 / x
                Return result
            End If
            cn = -0.590592860534773
            cn = cn * x2 + 0.629235242724369
            cn = cn * x2 - 0.172858975380388
            cn = cn * x2 + 0.016483704782519
            cn = cn * x2 - 0.000486827613020463
            cd = 1.0
            cd = cd * x2 - 2.69820057197545
            cd = cd * x2 + 1.73270799045948
            cd = cd * x2 - 0.39370858228194
            cd = cd * x2 + 0.0344278924041233
            cd = cd * x2 - 0.000973655226040941
            y = 1.0 / x + x2 * cn / (cd * x)
            result = sg * 0.5 * y
            Return result
        End Function


    End Class
    Public Class elliptic
        '************************************************************************
        '        Complete elliptic integral of the first kind
        '
        '        Approximates the integral
        '
        '
        '
        '                   pi/2
        '                    -
        '                   | |
        '                   |           dt
        '        K(m)  =    |    ------------------
        '                   |                   2
        '                 | |    sqrt( 1 - m sin t )
        '                  -
        '                   0
        '
        '        using the approximation
        '
        '            P(x)  -  log x Q(x).
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE       0,1        30000       2.5e-16     6.8e-17
        '
        '        Cephes Math Library, Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function ellipticintegralk(m As Double) As Double
            Dim result As Double = 0

            result = ellipticintegralkhighprecision(1.0 - m)
            Return result
        End Function


        '************************************************************************
        '        Complete elliptic integral of the first kind
        '
        '        Approximates the integral
        '
        '
        '
        '                   pi/2
        '                    -
        '                   | |
        '                   |           dt
        '        K(m)  =    |    ------------------
        '                   |                   2
        '                 | |    sqrt( 1 - m sin t )
        '                  -
        '                   0
        '
        '        where m = 1 - m1, using the approximation
        '
        '            P(x)  -  log x Q(x).
        '
        '        The argument m1 is used rather than m so that the logarithmic
        '        singularity at m = 1 will be shifted to the origin; this
        '        preserves maximum accuracy.
        '
        '        K(0) = pi/2.
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE       0,1        30000       2.5e-16     6.8e-17
        '
        '        Cephes Math Library, Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function ellipticintegralkhighprecision(m1 As Double) As Double
            Dim result As Double = 0
            Dim p As Double = 0
            Dim q As Double = 0

            If CDbl(m1) <= CDbl(Math.machineepsilon) Then
                result = 1.38629436111989 - 0.5 * System.Math.Log(m1)
            Else
                p = 0.000137982864606273
                p = p * m1 + 0.00228025724005876
                p = p * m1 + 0.00797404013220415
                p = p * m1 + 0.00985821379021226
                p = p * m1 + 0.0068748968744995
                p = p * m1 + 0.00618901033637688
                p = p * m1 + 0.00879078273952744
                p = p * m1 + 0.0149380448916805
                p = p * m1 + 0.0308851465246712
                p = p * m1 + 0.096573590281169
                p = p * m1 + 1.38629436111989
                q = 0.0000294078955048598
                q = q * m1 + 0.000914184723865917
                q = q * m1 + 0.00594058303753168
                q = q * m1 + 0.0154850516649762
                q = q * m1 + 0.0239089602715925
                q = q * m1 + 0.0301204715227604
                q = q * m1 + 0.0373774314173823
                q = q * m1 + 0.0488280347570998
                q = q * m1 + 0.0703124996963957
                q = q * m1 + 0.124999999999871
                q = q * m1 + 0.5
                result = p - q * System.Math.Log(m1)
            End If
            Return result
        End Function


        '************************************************************************
        '        Incomplete elliptic integral of the first kind F(phi|m)
        '
        '        Approximates the integral
        '
        '
        '
        '                       phi
        '                        -
        '                       | |
        '                       |           dt
        '        F(phi_\m)  =    |    ------------------
        '                       |                   2
        '                     | |    sqrt( 1 - m sin t )
        '                      -
        '                       0
        '
        '        of amplitude phi and modulus m, using the arithmetic -
        '        geometric mean algorithm.
        '
        '
        '
        '
        '        ACCURACY:
        '
        '        Tested at random points with m in [0, 1] and phi as indicated.
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE     -10,10       200000      7.4e-16     1.0e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function incompleteellipticintegralk(phi As Double, m As Double) As Double
            Dim result As Double = 0
            Dim a As Double = 0
            Dim b As Double = 0
            Dim c As Double = 0
            Dim e As Double = 0
            Dim temp As Double = 0
            Dim pio2 As Double = 0
            Dim t As Double = 0
            Dim k As Double = 0
            Dim d As Integer = 0
            Dim md As Integer = 0
            Dim s As Integer = 0
            Dim npio2 As Integer = 0

            pio2 = 1.5707963267949
            If CDbl(m) = CDbl(0) Then
                result = phi
                Return result
            End If
            a = 1 - m
            If CDbl(a) = CDbl(0) Then
                result = System.Math.Log(System.Math.Tan(0.5 * (pio2 + phi)))
                Return result
            End If
            npio2 = CInt(System.Math.Truncate(System.Math.Floor(phi / pio2)))
            If npio2 Mod 2 <> 0 Then
                npio2 = npio2 + 1
            End If
            If npio2 <> 0 Then
                k = ellipticintegralk(1 - a)
                phi = phi - npio2 * pio2
            Else
                k = 0
            End If
            If CDbl(phi) < CDbl(0) Then
                phi = -phi
                s = -1
            Else
                s = 0
            End If
            b = System.Math.sqrt(a)
            t = System.Math.Tan(phi)
            If CDbl(System.Math.Abs(t)) > CDbl(10) Then
                e = 1.0 / (b * t)
                If CDbl(System.Math.Abs(e)) < CDbl(10) Then
                    e = System.Math.Atan(e)
                    If npio2 = 0 Then
                        k = ellipticintegralk(1 - a)
                    End If
                    temp = k - incompleteellipticintegralk(e, m)
                    If s < 0 Then
                        temp = -temp
                    End If
                    result = temp + npio2 * k
                    Return result
                End If
            End If
            a = 1.0
            c = System.Math.sqrt(m)
            d = 1
            md = 0
            While CDbl(System.Math.Abs(c / a)) > CDbl(Math.machineepsilon)
                temp = b / a
                phi = phi + System.Math.Atan(t * temp) + md * System.Math.PI
                md = CInt(System.Math.Truncate((phi + pio2) / System.Math.PI))
                t = t * (1.0 + temp) / (1.0 - temp * t * t)
                c = 0.5 * (a - b)
                temp = System.Math.sqrt(a * b)
                a = 0.5 * (a + b)
                b = temp
                d = d + d
            End While
            temp = (System.Math.Atan(t) + md * System.Math.PI) / (d * a)
            If s < 0 Then
                temp = -temp
            End If
            result = temp + npio2 * k
            Return result
        End Function


        '************************************************************************
        '        Complete elliptic integral of the second kind
        '
        '        Approximates the integral
        '
        '
        '                   pi/2
        '                    -
        '                   | |                 2
        '        E(m)  =    |    sqrt( 1 - m sin t ) dt
        '                 | |
        '                  -
        '                   0
        '
        '        using the approximation
        '
        '             P(x)  -  x log x Q(x).
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE       0, 1       10000       2.1e-16     7.3e-17
        '
        '        Cephes Math Library, Release 2.8: June, 2000
        '        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function ellipticintegrale(m As Double) As Double
            Dim result As Double = 0
            Dim p As Double = 0
            Dim q As Double = 0

            alglib.ap.assert(CDbl(m) >= CDbl(0) AndAlso CDbl(m) <= CDbl(1), "Domain error in EllipticIntegralE: m<0 or m>1")
            m = 1 - m
            If CDbl(m) = CDbl(0) Then
                result = 1
                Return result
            End If
            p = 0.000153552577301013
            p = p * m + 0.00250888492163602
            p = p * m + 0.0086878681656589
            p = p * m + 0.0107350949056076
            p = p * m + 0.00777395492516787
            p = p * m + 0.00758395289413515
            p = p * m + 0.0115688436810574
            p = p * m + 0.0218317996015557
            p = p * m + 0.0568051945617861
            p = p * m + 0.443147180560991
            p = p * m + 1.0
            q = 0.0000327954898576486
            q = q * m + 0.00100962792679357
            q = q * m + 0.00650609489976928
            q = q * m + 0.0168862163993311
            q = q * m + 0.0261769742454494
            q = q * m + 0.0334833904888225
            q = q * m + 0.0427180926518932
            q = q * m + 0.0585936634471101
            q = q * m + 0.0937499997197644
            q = q * m + 0.249999999999888
            result = p - q * m * System.Math.Log(m)
            Return result
        End Function


        '************************************************************************
        '        Incomplete elliptic integral of the second kind
        '
        '        Approximates the integral
        '
        '
        '                       phi
        '                        -
        '                       | |
        '                       |                   2
        '        E(phi_\m)  =    |    sqrt( 1 - m sin t ) dt
        '                       |
        '                     | |
        '                      -
        '                       0
        '
        '        of amplitude phi and modulus m, using the arithmetic -
        '        geometric mean algorithm.
        '
        '        ACCURACY:
        '
        '        Tested at random arguments with phi in [-10, 10] and m in
        '        [0, 1].
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE     -10,10      150000       3.3e-15     1.4e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1993, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function incompleteellipticintegrale(phi As Double, m As Double) As Double
            Dim result As Double = 0
            Dim pio2 As Double = 0
            Dim a As Double = 0
            Dim b As Double = 0
            Dim c As Double = 0
            Dim e As Double = 0
            Dim temp As Double = 0
            Dim lphi As Double = 0
            Dim t As Double = 0
            Dim ebig As Double = 0
            Dim d As Integer = 0
            Dim md As Integer = 0
            Dim npio2 As Integer = 0
            Dim s As Integer = 0

            pio2 = 1.5707963267949
            If CDbl(m) = CDbl(0) Then
                result = phi
                Return result
            End If
            lphi = phi
            npio2 = CInt(System.Math.Truncate(System.Math.Floor(lphi / pio2)))
            If npio2 Mod 2 <> 0 Then
                npio2 = npio2 + 1
            End If
            lphi = lphi - npio2 * pio2
            If CDbl(lphi) < CDbl(0) Then
                lphi = -lphi
                s = -1
            Else
                s = 1
            End If
            a = 1.0 - m
            ebig = ellipticintegrale(m)
            If CDbl(a) = CDbl(0) Then
                temp = System.Math.Sin(lphi)
                If s < 0 Then
                    temp = -temp
                End If
                result = temp + npio2 * ebig
                Return result
            End If
            t = System.Math.Tan(lphi)
            b = System.Math.sqrt(a)

            '
            ' Thanks to Brian Fitzgerald <fitzgb@mml0.meche.rpi.edu>
            ' for pointing out an instability near odd multiples of pi/2
            '
            If CDbl(System.Math.Abs(t)) > CDbl(10) Then

                '
                ' Transform the amplitude
                '
                e = 1.0 / (b * t)

                '
                ' ... but avoid multiple recursions.
                '
                If CDbl(System.Math.Abs(e)) < CDbl(10) Then
                    e = System.Math.Atan(e)
                    temp = ebig + m * System.Math.Sin(lphi) * System.Math.Sin(e) - incompleteellipticintegrale(e, m)
                    If s < 0 Then
                        temp = -temp
                    End If
                    result = temp + npio2 * ebig
                    Return result
                End If
            End If
            c = System.Math.sqrt(m)
            a = 1.0
            d = 1
            e = 0.0
            md = 0
            While CDbl(System.Math.Abs(c / a)) > CDbl(Math.machineepsilon)
                temp = b / a
                lphi = lphi + System.Math.Atan(t * temp) + md * System.Math.PI
                md = CInt(System.Math.Truncate((lphi + pio2) / System.Math.PI))
                t = t * (1.0 + temp) / (1.0 - temp * t * t)
                c = 0.5 * (a - b)
                temp = System.Math.sqrt(a * b)
                a = 0.5 * (a + b)
                b = temp
                d = d + d
                e = e + c * System.Math.Sin(lphi)
            End While
            temp = ebig / ellipticintegralk(m)
            temp = temp * ((System.Math.Atan(t) + md * System.Math.PI) / (d * a))
            temp = temp + e
            If s < 0 Then
                temp = -temp
            End If
            result = temp + npio2 * ebig
            Return result
        End Function


    End Class
    Public Class expintegrals
        '************************************************************************
        '        Exponential integral Ei(x)
        '
        '                      x
        '                       -     t
        '                      | |   e
        '           Ei(x) =   -|-   ---  dt .
        '                    | |     t
        '                     -
        '                    -inf
        '
        '        Not defined for x <= 0.
        '        See also expn.c.
        '
        '
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE       0,100       50000      8.6e-16     1.3e-16
        '
        '        Cephes Math Library Release 2.8:  May, 1999
        '        Copyright 1999 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function exponentialintegralei(x As Double) As Double
            Dim result As Double = 0
            Dim eul As Double = 0
            Dim f As Double = 0
            Dim f1 As Double = 0
            Dim f2 As Double = 0
            Dim w As Double = 0

            eul = 0.577215664901533
            If CDbl(x) <= CDbl(0) Then
                result = 0
                Return result
            End If
            If CDbl(x) < CDbl(2) Then
                f1 = -5.35044735781254
                f1 = f1 * x + 218.504916881661
                f1 = f1 * x - 4176.57238482669
                f1 = f1 * x + 55411.7675639356
                f1 = f1 * x - 331338.133117814
                f1 = f1 * x + 1592627.16338495
                f2 = 1.0
                f2 = f2 * x - 52.5054795911286
                f2 = f2 * x + 1259.61618678679
                f2 = f2 * x - 17565.4958197353
                f2 = f2 * x + 149306.211700273
                f2 = f2 * x - 729494.923964053
                f2 = f2 * x + 1592627.16338495
                f = f1 / f2
                result = eul + System.Math.Log(x) + x * f
                Return result
            End If
            If CDbl(x) < CDbl(4) Then
                w = 1 / x
                f1 = 0.0198180850325969
                f1 = f1 * w - 1.27164562598492
                f1 = f1 * w - 2.08816033568123
                f1 = f1 * w + 2.75554450918794
                f1 = f1 * w - 0.44095070487016
                f1 = f1 * w + 0.0466562380593589
                f1 = f1 * w - 0.00154504267967349
                f1 = f1 * w + 0.0000705998060529962
                f2 = 1.0
                f2 = f2 * w + 1.47649867091492
                f2 = f2 * w + 0.562917717482244
                f2 = f2 * w + 0.169901789787931
                f2 = f2 * w + 0.0229164717903421
                f2 = f2 * w + 0.00445015043972875
                f2 = f2 * w + 0.000172743961220652
                f2 = f2 * w + 0.0000395316719554967
                f = f1 / f2
                result = System.Math.Exp(x) * w * (1 + w * f)
                Return result
            End If
            If CDbl(x) < CDbl(8) Then
                w = 1 / x
                f1 = -1.37321537587121
                f1 = f1 * w - 0.708455913374084
                f1 = f1 * w + 1.58080685554794
                f1 = f1 * w - 0.260150042742562
                f1 = f1 * w + 0.0299467469411371
                f1 = f1 * w - 0.00103808604018874
                f1 = f1 * w + 0.0000437106442075301
                f1 = f1 * w + 0.0000021417836795226
                f2 = 1.0
                f2 = f2 * w + 0.858523142362203
                f2 = f2 * w + 0.4483285822874
                f2 = f2 * w + 0.0768793215812448
                f2 = f2 * w + 0.0244986824102189
                f2 = f2 * w + 0.00088321659419278
                f2 = f2 * w + 0.000459095229951135
                f2 = f2 * w + -0.00000472984835186652
                f2 = f2 * w + 0.00000266519553739071
                f = f1 / f2
                result = System.Math.Exp(x) * w * (1 + w * f)
                Return result
            End If
            If CDbl(x) < CDbl(16) Then
                w = 1 / x
                f1 = -2.10693460169192
                f1 = f1 * w + 1.73273386966469
                f1 = f1 * w - 0.242361917893584
                f1 = f1 * w + 0.0232272418093757
                f1 = f1 * w + 0.000237288044049318
                f1 = f1 * w - 0.0000834321956119255
                f1 = f1 * w + 0.0000136340879560525
                f1 = f1 * w - 0.000000365541232199925
                f1 = f1 * w + 0.0000000146494173397596
                f1 = f1 * w + 0.000000000617640786371036
                f2 = 1.0
                f2 = f2 * w - 0.229806223990168
                f2 = f2 * w + 0.110507704147404
                f2 = f2 * w - 0.0156654296663079
                f2 = f2 * w + 0.00276110685081735
                f2 = f2 * w - 0.000208914801228405
                f2 = f2 * w + 0.0000170852893880768
                f2 = f2 * w - 0.000000445931179635669
                f2 = f2 * w + 0.0000000139463493035385
                f2 = f2 * w + 0.000000000615086593397734
                f = f1 / f2
                result = System.Math.Exp(x) * w * (1 + w * f)
                Return result
            End If
            If CDbl(x) < CDbl(32) Then
                w = 1 / x
                f1 = -0.245811936767402
                f1 = f1 * w - 0.148338225332208
                f1 = f1 * w + 0.0724829179573555
                f1 = f1 * w - 0.0134831568738094
                f1 = f1 * w + 0.00134277506978864
                f1 = f1 * w - 0.0000794246563715971
                f1 = f1 * w + 0.00000264417951898424
                f1 = f1 * w - 0.0000000423947365931377
                f2 = 1.0
                f2 = f2 * w - 0.104422590844387
                f2 = f2 * w - 0.26764531281014
                f2 = f2 * w + 0.0969500025462198
                f2 = f2 * w - 0.0160174569271299
                f2 = f2 * w + 0.00149641489920591
                f2 = f2 * w - 0.0000846245256377849
                f2 = f2 * w + 0.00000272893840347673
                f2 = f2 * w - 0.0000000423946243181954
                f = f1 / f2
                result = System.Math.Exp(x) * w * (1 + w * f)
                Return result
            End If
            If CDbl(x) < CDbl(64) Then
                w = 1 / x
                f1 = 0.121256111810546
                f1 = f1 * w - 0.582313317904389
                f1 = f1 * w + 0.234888731455702
                f1 = f1 * w - 0.0304003431811325
                f1 = f1 * w + 0.00151008214686519
                f1 = f1 * w - 0.0000252313709549957
                f2 = 1.0
                f2 = f2 * w - 1.00225215036585
                f2 = f2 * w + 0.292870969487222
                f2 = f2 * w - 0.0333700433867401
                f2 = f2 * w + 0.00156054488112739
                f2 = f2 * w - 0.0000252313709360323
                f = f1 / f2
                result = System.Math.Exp(x) * w * (1 + w * f)
                Return result
            End If
            w = 1 / x
            f1 = -0.765784707828613
            f1 = f1 * w + 0.688619241556671
            f1 = f1 * w - 0.213259811354521
            f1 = f1 * w + 0.0334610755238419
            f1 = f1 * w - 0.00307654147734476
            f1 = f1 * w + 0.000174711931645491
            f1 = f1 * w - 0.00000610371168227417
            f1 = f1 * w + 0.000000121803276542865
            f1 = f1 * w - 0.00000000108607610279329
            f2 = 1.0
            f2 = f2 * w - 1.88880286866231
            f2 = f2 * w + 1.06669168721141
            f2 = f2 * w - 0.275191598230638
            f2 = f2 * w + 0.0393085268823382
            f2 = f2 * w - 0.00341468455860237
            f2 = f2 * w + 0.000186684437070356
            f2 = f2 * w - 0.00000634514608313052
            f2 = f2 * w + 0.000000123975428748321
            f2 = f2 * w - 0.00000000108607610279313
            f = f1 / f2
            result = System.Math.Exp(x) * w * (1 + w * f)
            Return result
        End Function


        '************************************************************************
        '        Exponential integral En(x)
        '
        '        Evaluates the exponential integral
        '
        '                        inf.
        '                          -
        '                         | |   -xt
        '                         |    e
        '             E (x)  =    |    ----  dt.
        '              n          |      n
        '                       | |     t
        '                        -
        '                         1
        '
        '
        '        Both n and x must be nonnegative.
        '
        '        The routine employs either a power series, a continued
        '        fraction, or an asymptotic formula depending on the
        '        relative values of n and x.
        '
        '        ACCURACY:
        '
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0, 30       10000       1.7e-15     3.6e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1985, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function exponentialintegralen(x As Double, n As Integer) As Double
            Dim result As Double = 0
            Dim r As Double = 0
            Dim t As Double = 0
            Dim yk As Double = 0
            Dim xk As Double = 0
            Dim pk As Double = 0
            Dim pkm1 As Double = 0
            Dim pkm2 As Double = 0
            Dim qk As Double = 0
            Dim qkm1 As Double = 0
            Dim qkm2 As Double = 0
            Dim psi As Double = 0
            Dim z As Double = 0
            Dim i As Integer = 0
            Dim k As Integer = 0
            Dim big As Double = 0
            Dim eul As Double = 0

            eul = 0.577215664901533
            big = 1.44115188075856 * System.Math.Pow(10, 17)
            If ((n < 0 OrElse CDbl(x) < CDbl(0)) OrElse CDbl(x) > CDbl(170)) OrElse (CDbl(x) = CDbl(0) AndAlso n < 2) Then
                result = -1
                Return result
            End If
            If CDbl(x) = CDbl(0) Then
                result = CDbl(1) / CDbl(n - 1)
                Return result
            End If
            If n = 0 Then
                result = System.Math.Exp(-x) / x
                Return result
            End If
            If n > 5000 Then
                xk = x + n
                yk = 1 / (xk * xk)
                t = n
                result = yk * t * (6 * x * x - 8 * t * x + t * t)
                result = yk * (result + t * (t - 2.0 * x))
                result = yk * (result + t)
                result = (result + 1) * System.Math.Exp(-x) / xk
                Return result
            End If
            If CDbl(x) <= CDbl(1) Then
                psi = -eul - System.Math.Log(x)
                For i = 1 To n - 1
                    psi = psi + CDbl(1) / CDbl(i)
                Next
                z = -x
                xk = 0
                yk = 1
                pk = 1 - n
                If n = 1 Then
                    result = 0.0
                Else
                    result = 1.0 / pk
                End If
                Do
                    xk = xk + 1
                    yk = yk * z / xk
                    pk = pk + 1
                    If CDbl(pk) <> CDbl(0) Then
                        result = result + yk / pk
                    End If
                    If CDbl(result) <> CDbl(0) Then
                        t = System.Math.Abs(yk / result)
                    Else
                        t = 1
                    End If
                Loop While CDbl(t) >= CDbl(Math.machineepsilon)
                t = 1
                For i = 1 To n - 1
                    t = t * z / i
                Next
                result = psi * t - result
                Return result
            Else
                k = 1
                pkm2 = 1
                qkm2 = x
                pkm1 = 1.0
                qkm1 = x + n
                result = pkm1 / qkm1
                Do
                    k = k + 1
                    If k Mod 2 = 1 Then
                        yk = 1
                        xk = n + CDbl(k - 1) / CDbl(2)
                    Else
                        yk = x
                        xk = CDbl(k) / CDbl(2)
                    End If
                    pk = pkm1 * yk + pkm2 * xk
                    qk = qkm1 * yk + qkm2 * xk
                    If CDbl(qk) <> CDbl(0) Then
                        r = pk / qk
                        t = System.Math.Abs((result - r) / r)
                        result = r
                    Else
                        t = 1
                    End If
                    pkm2 = pkm1
                    pkm1 = pk
                    qkm2 = qkm1
                    qkm1 = qk
                    If CDbl(System.Math.Abs(pk)) > CDbl(big) Then
                        pkm2 = pkm2 / big
                        pkm1 = pkm1 / big
                        qkm2 = qkm2 / big
                        qkm1 = qkm1 / big
                    End If
                Loop While CDbl(t) >= CDbl(Math.machineepsilon)
                result = result * System.Math.Exp(-x)
            End If
            Return result
        End Function


    End Class
    Public Class fdistr
        '************************************************************************
        '        F distribution
        '
        '        Returns the area from zero to x under the F density
        '        function (also known as Snedcor's density or the
        '        variance ratio density).  This is the density
        '        of x = (u1/df1)/(u2/df2), where u1 and u2 are random
        '        variables having Chi square distributions with df1
        '        and df2 degrees of freedom, respectively.
        '        The incomplete beta integral is used, according to the
        '        formula
        '
        '        P(x) = incbet( df1/2, df2/2, (df1*x/(df2 + df1*x) ).
        '
        '
        '        The arguments a and b are greater than zero, and x is
        '        nonnegative.
        '
        '        ACCURACY:
        '
        '        Tested at random points (a,b,x).
        '
        '                       x     a,b                     Relative error:
        '        arithmetic  domain  domain     # trials      peak         rms
        '           IEEE      0,1    0,100       100000      9.8e-15     1.7e-15
        '           IEEE      1,5    0,100       100000      6.5e-15     3.5e-16
        '           IEEE      0,1    1,10000     100000      2.2e-11     3.3e-12
        '           IEEE      1,5    1,10000     100000      1.1e-11     1.7e-13
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function fdistribution(a As Integer, b As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim w As Double = 0

            alglib.ap.assert((a >= 1 AndAlso b >= 1) AndAlso CDbl(x) >= CDbl(0), "Domain error in FDistribution")
            w = a * x
            w = w / (b + w)
            result = ibetaf.incompletebeta(0.5 * a, 0.5 * b, w)
            Return result
        End Function


        '************************************************************************
        '        Complemented F distribution
        '
        '        Returns the area from x to infinity under the F density
        '        function (also known as Snedcor's density or the
        '        variance ratio density).
        '
        '
        '                             inf.
        '                              -
        '                     1       | |  a-1      b-1
        '        1-P(x)  =  ------    |   t    (1-t)    dt
        '                   B(a,b)  | |
        '                            -
        '                             x
        '
        '
        '        The incomplete beta integral is used, according to the
        '        formula
        '
        '        P(x) = incbet( df2/2, df1/2, (df2/(df2 + df1*x) ).
        '
        '
        '        ACCURACY:
        '
        '        Tested at random points (a,b,x) in the indicated intervals.
        '                       x     a,b                     Relative error:
        '        arithmetic  domain  domain     # trials      peak         rms
        '           IEEE      0,1    1,100       100000      3.7e-14     5.9e-16
        '           IEEE      1,5    1,100       100000      8.0e-15     1.6e-15
        '           IEEE      0,1    1,10000     100000      1.8e-11     3.5e-13
        '           IEEE      1,5    1,10000     100000      2.0e-11     3.0e-12
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function fcdistribution(a As Integer, b As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim w As Double = 0

            alglib.ap.assert((a >= 1 AndAlso b >= 1) AndAlso CDbl(x) >= CDbl(0), "Domain error in FCDistribution")
            w = b / (b + a * x)
            result = ibetaf.incompletebeta(0.5 * b, 0.5 * a, w)
            Return result
        End Function


        '************************************************************************
        '        Inverse of complemented F distribution
        '
        '        Finds the F density argument x such that the integral
        '        from x to infinity of the F density is equal to the
        '        given probability p.
        '
        '        This is accomplished using the inverse beta integral
        '        function and the relations
        '
        '             z = incbi( df2/2, df1/2, p )
        '             x = df2 (1-z) / (df1 z).
        '
        '        Note: the following relations hold for the inverse of
        '        the uncomplemented F distribution:
        '
        '             z = incbi( df1/2, df2/2, p )
        '             x = df2 z / (df1 (1-z)).
        '
        '        ACCURACY:
        '
        '        Tested at random points (a,b,p).
        '
        '                     a,b                     Relative error:
        '        arithmetic  domain     # trials      peak         rms
        '         For p between .001 and 1:
        '           IEEE     1,100       100000      8.3e-15     4.7e-16
        '           IEEE     1,10000     100000      2.1e-11     1.4e-13
        '         For p between 10^-6 and 10^-3:
        '           IEEE     1,100        50000      1.3e-12     8.4e-15
        '           IEEE     1,10000      50000      3.0e-12     4.8e-14
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function invfdistribution(a As Integer, b As Integer, y As Double) As Double
            Dim result As Double = 0
            Dim w As Double = 0

            alglib.ap.assert(((a >= 1 AndAlso b >= 1) AndAlso CDbl(y) > CDbl(0)) AndAlso CDbl(y) <= CDbl(1), "Domain error in InvFDistribution")

            '
            ' Compute probability for x = 0.5
            '
            w = ibetaf.incompletebeta(0.5 * b, 0.5 * a, 0.5)

            '
            ' If that is greater than y, then the solution w < .5
            ' Otherwise, solve at 1-y to remove cancellation in (b - b*w)
            '
            If CDbl(w) > CDbl(y) OrElse CDbl(y) < CDbl(0.001) Then
                w = ibetaf.invincompletebeta(0.5 * b, 0.5 * a, y)
                result = (b - b * w) / (a * w)
            Else
                w = ibetaf.invincompletebeta(0.5 * a, 0.5 * b, 1.0 - y)
                result = b * w / (a * (1.0 - w))
            End If
            Return result
        End Function


    End Class
    Public Class fresnel
        '************************************************************************
        '        Fresnel integral
        '
        '        Evaluates the Fresnel integrals
        '
        '                  x
        '                  -
        '                 | |
        '        C(x) =   |   cos(pi/2 t**2) dt,
        '               | |
        '                -
        '                 0
        '
        '                  x
        '                  -
        '                 | |
        '        S(x) =   |   sin(pi/2 t**2) dt.
        '               | |
        '                -
        '                 0
        '
        '
        '        The integrals are evaluated by a power series for x < 1.
        '        For x >= 1 auxiliary functions f(x) and g(x) are employed
        '        such that
        '
        '        C(x) = 0.5 + f(x) sin( pi/2 x**2 ) - g(x) cos( pi/2 x**2 )
        '        S(x) = 0.5 - f(x) cos( pi/2 x**2 ) - g(x) sin( pi/2 x**2 )
        '
        '
        '
        '        ACCURACY:
        '
        '         Relative error.
        '
        '        Arithmetic  function   domain     # trials      peak         rms
        '          IEEE       S(x)      0, 10       10000       2.0e-15     3.2e-16
        '          IEEE       C(x)      0, 10       10000       1.8e-15     3.3e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Sub fresnelintegral(x As Double, ByRef c As Double, ByRef s As Double)
            Dim xxa As Double = 0
            Dim f As Double = 0
            Dim g As Double = 0
            Dim cc As Double = 0
            Dim ss As Double = 0
            Dim t As Double = 0
            Dim u As Double = 0
            Dim x2 As Double = 0
            Dim sn As Double = 0
            Dim sd As Double = 0
            Dim cn As Double = 0
            Dim cd As Double = 0
            Dim fn As Double = 0
            Dim fd As Double = 0
            Dim gn As Double = 0
            Dim gd As Double = 0
            Dim mpi As Double = 0
            Dim mpio2 As Double = 0

            mpi = 3.14159265358979
            mpio2 = 1.5707963267949
            xxa = x
            x = System.Math.Abs(xxa)
            x2 = x * x
            If CDbl(x2) < CDbl(2.5625) Then
                t = x2 * x2
                sn = -2991.8191940102
                sn = sn * t + 708840.045257739
                sn = sn * t - 62974148.6205863
                sn = sn * t + 2548908805.73376
                sn = sn * t - 44297951805.9698
                sn = sn * t + 318016297876.568
                sd = 1.0
                sd = sd * t + 281.376268889994
                sd = sd * t + 45584.7810806533
                sd = sd * t + 5173438.88770096
                sd = sd * t + 419320245.898111
                sd = sd * t + 22441179564.5341
                sd = sd * t + 607366389490.085
                cn = -0.0000000498843114573574
                cn = cn * t + 0.0000095042806282986
                cn = cn * t - 0.000645191435683965
                cn = cn * t + 0.0188843319396704
                cn = cn * t - 0.205525900955014
                cn = cn * t + 1.0
                cd = 0.00000000000399982968972496
                cd = cd * t + 0.000000000915439215774657
                cd = cd * t + 0.000000125001862479599
                cd = cd * t + 0.0000122262789024179
                cd = cd * t + 0.000868029542941784
                cd = cd * t + 0.04121420907222
                cd = cd * t + 1.0
                s = System.Math.Sign(xxa) * x * x2 * sn / sd
                c = System.Math.Sign(xxa) * x * cn / cd
                Return
            End If
            If CDbl(x) > CDbl(36974.0) Then
                c = System.Math.Sign(xxa) * 0.5
                s = System.Math.Sign(xxa) * 0.5
                Return
            End If
            x2 = x * x
            t = mpi * x2
            u = 1 / (t * t)
            t = 1 / t
            fn = 0.421543555043678
            fn = fn * u + 0.143407919780759
            fn = fn * u + 0.0115220955073586
            fn = fn * u + 0.000345017939782574
            fn = fn * u + 0.00000463613749287867
            fn = fn * u + 0.0000000305568983790258
            fn = fn * u + 0.000000000102304514164907
            fn = fn * u + 0.000000000000172010743268162
            fn = fn * u + 0.000000000000000134283276233063
            fn = fn * u + 3.76329711269988E-20
            fd = 1.0
            fd = fd * u + 0.751586398353379
            fd = fd * u + 0.116888925859191
            fd = fd * u + 0.00644051526508859
            fd = fd * u + 0.000155934409164153
            fd = fd * u + 0.00000184627567348931
            fd = fd * u + 0.0000000112699224763999
            fd = fd * u + 0.0000000000360140029589371
            fd = fd * u + 0.0000000000000588754533621578
            fd = fd * u + 4.5200143407413E-17
            fd = fd * u + 1.25443237090011E-20
            gn = 0.504442073643383
            gn = gn * u + 0.197102833525523
            gn = gn * u + 0.0187648584092575
            gn = gn * u + 0.000684079380915393
            gn = gn * u + 0.0000115138826111884
            gn = gn * u + 0.0000000982852443688422
            gn = gn * u + 0.00000000044534441586175
            gn = gn * u + 0.00000000000108268041139021
            gn = gn * u + 0.00000000000000137555460633262
            gn = gn * u + 8.36354435630677E-19
            gn = gn * u + 1.86958710162783E-22
            gd = 1.0
            gd = gd * u + 1.47495759925128
            gd = gd * u + 0.33774898912002
            gd = gd * u + 0.0253603741420339
            gd = gd * u + 0.000814679107184306
            gd = gd * u + 0.0000127545075667729
            gd = gd * u + 0.000000104314589657572
            gd = gd * u + 0.00000000046068072814652
            gd = gd * u + 0.0000000000011027321506624
            gd = gd * u + 0.00000000000000138796531259579
            gd = gd * u + 8.39158816283119E-19
            gd = gd * u + 1.86958710162783E-22
            f = 1 - u * fn / fd
            g = t * gn / gd
            t = mpio2 * x2
            cc = System.Math.Cos(t)
            ss = System.Math.Sin(t)
            t = mpi * x
            c = 0.5 + (f * ss - g * cc) / t
            s = 0.5 - (f * cc + g * ss) / t
            c = c * System.Math.Sign(xxa)
            s = s * System.Math.Sign(xxa)
        End Sub


    End Class
    Public Class hermite
        '************************************************************************
        '        Calculation of the value of the Hermite polynomial.
        '
        '        Parameters:
        '            n   -   degree, n>=0
        '            x   -   argument
        '
        '        Result:
        '            the value of the Hermite polynomial Hn at x
        '        ************************************************************************

        Public Shared Function hermitecalculate(n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim i As Integer = 0
            Dim a As Double = 0
            Dim b As Double = 0

            result = 0

            '
            ' Prepare A and B
            '
            a = 1
            b = 2 * x

            '
            ' Special cases: N=0 or N=1
            '
            If n = 0 Then
                result = a
                Return result
            End If
            If n = 1 Then
                result = b
                Return result
            End If

            '
            ' General case: N>=2
            '
            For i = 2 To n
                result = 2 * x * b - 2 * (i - 1) * a
                a = b
                b = result
            Next
            Return result
        End Function


        '************************************************************************
        '        Summation of Hermite polynomials using Clenshaw抯 recurrence formula.
        '
        '        This routine calculates
        '            c[0]*H0(x) + c[1]*H1(x) + ... + c[N]*HN(x)
        '
        '        Parameters:
        '            n   -   degree, n>=0
        '            x   -   argument
        '
        '        Result:
        '            the value of the Hermite polynomial at x
        '        ************************************************************************

        Public Shared Function hermitesum(c As Double(), n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0
            Dim i As Integer = 0

            b1 = 0
            b2 = 0
            result = 0
            For i = n To 0 Step -1
                result = 2 * (x * b1 - (i + 1) * b2) + c(i)
                b2 = b1
                b1 = result
            Next
            Return result
        End Function


        '************************************************************************
        '        Representation of Hn as C[0] + C[1]*X + ... + C[N]*X^N
        '
        '        Input parameters:
        '            N   -   polynomial degree, n>=0
        '
        '        Output parameters:
        '            C   -   coefficients
        '        ************************************************************************

        Public Shared Sub hermitecoefficients(n As Integer, ByRef c As Double())
            Dim i As Integer = 0

            c = New Double(-1) {}

            c = New Double(n) {}
            For i = 0 To n
                c(i) = 0
            Next
            c(n) = System.Math.Exp(n * System.Math.Log(2))
            For i = 0 To n \ 2 - 1
                c(n - 2 * (i + 1)) = -(c(n - 2 * i) * (n - 2 * i) * (n - 2 * i - 1) / 4 / (i + 1))
            Next
        End Sub


    End Class
    Public Class jacobianelliptic
        '************************************************************************
        '        Jacobian Elliptic Functions
        '
        '        Evaluates the Jacobian elliptic functions sn(u|m), cn(u|m),
        '        and dn(u|m) of parameter m between 0 and 1, and real
        '        argument u.
        '
        '        These functions are periodic, with quarter-period on the
        '        real axis equal to the complete elliptic integral
        '        ellpk(1.0-m).
        '
        '        Relation to incomplete elliptic integral:
        '        If u = ellik(phi,m), then sn(u|m) = sin(phi),
        '        and cn(u|m) = cos(phi).  Phi is called the amplitude of u.
        '
        '        Computation is by means of the arithmetic-geometric mean
        '        algorithm, except when m is within 1e-9 of 0 or 1.  In the
        '        latter case with m close to 1, the approximation applies
        '        only for phi < pi/2.
        '
        '        ACCURACY:
        '
        '        Tested at random points with u between 0 and 10, m between
        '        0 and 1.
        '
        '                   Absolute error (* = relative error):
        '        arithmetic   function   # trials      peak         rms
        '           IEEE      phi         10000       9.2e-16*    1.4e-16*
        '           IEEE      sn          50000       4.1e-15     4.6e-16
        '           IEEE      cn          40000       3.6e-15     4.4e-16
        '           IEEE      dn          10000       1.3e-12     1.8e-14
        '
        '         Peak error observed in consistency check using addition
        '        theorem for sn(u+v) was 4e-16 (absolute).  Also tested by
        '        the above relation to the incomplete elliptic integral.
        '        Accuracy deteriorates when u is large.
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Sub jacobianellipticfunctions(u As Double, m As Double, ByRef sn As Double, ByRef cn As Double, ByRef dn As Double, ByRef ph As Double)
            Dim ai As Double = 0
            Dim b As Double = 0
            Dim phi As Double = 0
            Dim t As Double = 0
            Dim twon As Double = 0
            Dim a As Double() = New Double(-1) {}
            Dim c As Double() = New Double(-1) {}
            Dim i As Integer = 0

            sn = 0
            cn = 0
            dn = 0
            ph = 0

            alglib.ap.assert(CDbl(m) >= CDbl(0) AndAlso CDbl(m) <= CDbl(1), "Domain error in JacobianEllipticFunctions: m<0 or m>1")
            a = New Double(8) {}
            c = New Double(8) {}
            If CDbl(m) < CDbl(0.000000001) Then
                t = System.Math.Sin(u)
                b = System.Math.Cos(u)
                ai = 0.25 * m * (u - t * b)
                sn = t - ai * b
                cn = b + ai * t
                ph = u - ai
                dn = 1.0 - 0.5 * m * t * t
                Return
            End If
            If CDbl(m) >= CDbl(0.9999999999) Then
                ai = 0.25 * (1.0 - m)
                b = System.Math.Cosh(u)
                t = System.Math.Tanh(u)
                phi = 1.0 / b
                twon = b * System.Math.Sinh(u)
                sn = t + ai * (twon - u) / (b * b)
                ph = 2.0 * System.Math.Atan(System.Math.Exp(u)) - 1.5707963267949 + ai * (twon - u) / b
                ai = ai * t * phi
                cn = phi - ai * (twon - u)
                dn = phi + ai * (twon + u)
                Return
            End If
            a(0) = 1.0
            b = System.Math.sqrt(1.0 - m)
            c(0) = System.Math.sqrt(m)
            twon = 1.0
            i = 0
            While CDbl(System.Math.Abs(c(i) / a(i))) > CDbl(Math.machineepsilon)
                If i > 7 Then
                    alglib.ap.assert(False, "Overflow in JacobianEllipticFunctions")
                    Exit While
                End If
                ai = a(i)
                i = i + 1
                c(i) = 0.5 * (ai - b)
                t = System.Math.sqrt(ai * b)
                a(i) = 0.5 * (ai + b)
                b = t
                twon = twon * 2.0
            End While
            phi = twon * a(i) * u
            Do
                t = c(i) * System.Math.Sin(phi) / a(i)
                b = phi
                phi = (System.Math.Asin(t) + phi) / 2.0
                i = i - 1
            Loop While i <> 0
            sn = System.Math.Sin(phi)
            t = System.Math.Cos(phi)
            cn = t
            dn = t / System.Math.Cos(phi - b)
            ph = phi
        End Sub


    End Class
    Public Class laguerre
        '************************************************************************
        '        Calculation of the value of the Laguerre polynomial.
        '
        '        Parameters:
        '            n   -   degree, n>=0
        '            x   -   argument
        '
        '        Result:
        '            the value of the Laguerre polynomial Ln at x
        '        ************************************************************************

        Public Shared Function laguerrecalculate(n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim a As Double = 0
            Dim b As Double = 0
            Dim i As Double = 0

            result = 1
            a = 1
            b = 1 - x
            If n = 1 Then
                result = b
            End If
            i = 2
            While CDbl(i) <= CDbl(n)
                result = ((2 * i - 1 - x) * b - (i - 1) * a) / i
                a = b
                b = result
                i = i + 1
            End While
            Return result
        End Function


        '************************************************************************
        '        Summation of Laguerre polynomials using Clenshaw抯 recurrence formula.
        '
        '        This routine calculates c[0]*L0(x) + c[1]*L1(x) + ... + c[N]*LN(x)
        '
        '        Parameters:
        '            n   -   degree, n>=0
        '            x   -   argument
        '
        '        Result:
        '            the value of the Laguerre polynomial at x
        '        ************************************************************************

        Public Shared Function laguerresum(c As Double(), n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0
            Dim i As Integer = 0

            b1 = 0
            b2 = 0
            result = 0
            For i = n To 0 Step -1
                result = (2 * i + 1 - x) * b1 / (i + 1) - (i + 1) * b2 / (i + 2) + c(i)
                b2 = b1
                b1 = result
            Next
            Return result
        End Function


        '************************************************************************
        '        Representation of Ln as C[0] + C[1]*X + ... + C[N]*X^N
        '
        '        Input parameters:
        '            N   -   polynomial degree, n>=0
        '
        '        Output parameters:
        '            C   -   coefficients
        '        ************************************************************************

        Public Shared Sub laguerrecoefficients(n As Integer, ByRef c As Double())
            Dim i As Integer = 0

            c = New Double(-1) {}

            c = New Double(n) {}
            c(0) = 1
            For i = 0 To n - 1
                c(i + 1) = -(c(i) * (n - i) / (i + 1) / (i + 1))
            Next
        End Sub


    End Class
    Public Class legendre
        '************************************************************************
        '        Calculation of the value of the Legendre polynomial Pn.
        '
        '        Parameters:
        '            n   -   degree, n>=0
        '            x   -   argument
        '
        '        Result:
        '            the value of the Legendre polynomial Pn at x
        '        ************************************************************************

        Public Shared Function legendrecalculate(n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim a As Double = 0
            Dim b As Double = 0
            Dim i As Integer = 0

            result = 1
            a = 1
            b = x
            If n = 0 Then
                result = a
                Return result
            End If
            If n = 1 Then
                result = b
                Return result
            End If
            For i = 2 To n
                result = ((2 * i - 1) * x * b - (i - 1) * a) / i
                a = b
                b = result
            Next
            Return result
        End Function


        '************************************************************************
        '        Summation of Legendre polynomials using Clenshaw抯 recurrence formula.
        '
        '        This routine calculates
        '            c[0]*P0(x) + c[1]*P1(x) + ... + c[N]*PN(x)
        '
        '        Parameters:
        '            n   -   degree, n>=0
        '            x   -   argument
        '
        '        Result:
        '            the value of the Legendre polynomial at x
        '        ************************************************************************

        Public Shared Function legendresum(c As Double(), n As Integer, x As Double) As Double
            Dim result As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0
            Dim i As Integer = 0

            b1 = 0
            b2 = 0
            result = 0
            For i = n To 0 Step -1
                result = (2 * i + 1) * x * b1 / (i + 1) - (i + 1) * b2 / (i + 2) + c(i)
                b2 = b1
                b1 = result
            Next
            Return result
        End Function


        '************************************************************************
        '        Representation of Pn as C[0] + C[1]*X + ... + C[N]*X^N
        '
        '        Input parameters:
        '            N   -   polynomial degree, n>=0
        '
        '        Output parameters:
        '            C   -   coefficients
        '        ************************************************************************

        Public Shared Sub legendrecoefficients(n As Integer, ByRef c As Double())
            Dim i As Integer = 0

            c = New Double(-1) {}

            c = New Double(n) {}
            For i = 0 To n
                c(i) = 0
            Next
            c(n) = 1
            For i = 1 To n
                c(n) = c(n) * (n + i) / 2 / i
            Next
            For i = 0 To n \ 2 - 1
                c(n - 2 * (i + 1)) = -(c(n - 2 * i) * (n - 2 * i) * (n - 2 * i - 1) / 2 / (i + 1) / (2 * (n - i) - 1))
            Next
        End Sub


    End Class
    Public Class poissondistr
        '************************************************************************
        '        Poisson distribution
        '
        '        Returns the sum of the first k+1 terms of the Poisson
        '        distribution:
        '
        '          k         j
        '          --   -m  m
        '          >   e    --
        '          --       j!
        '         j=0
        '
        '        The terms are not summed directly; instead the incomplete
        '        gamma integral is employed, according to the relation
        '
        '        y = pdtr( k, m ) = igamc( k+1, m ).
        '
        '        The arguments must both be positive.
        '        ACCURACY:
        '
        '        See incomplete gamma function
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function poissondistribution(k As Integer, m As Double) As Double
            Dim result As Double = 0

            alglib.ap.assert(k >= 0 AndAlso CDbl(m) > CDbl(0), "Domain error in PoissonDistribution")
            result = igammaf.incompletegammac(k + 1, m)
            Return result
        End Function


        '************************************************************************
        '        Complemented Poisson distribution
        '
        '        Returns the sum of the terms k+1 to infinity of the Poisson
        '        distribution:
        '
        '         inf.       j
        '          --   -m  m
        '          >   e    --
        '          --       j!
        '         j=k+1
        '
        '        The terms are not summed directly; instead the incomplete
        '        gamma integral is employed, according to the formula
        '
        '        y = pdtrc( k, m ) = igam( k+1, m ).
        '
        '        The arguments must both be positive.
        '
        '        ACCURACY:
        '
        '        See incomplete gamma function
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function poissoncdistribution(k As Integer, m As Double) As Double
            Dim result As Double = 0

            alglib.ap.assert(k >= 0 AndAlso CDbl(m) > CDbl(0), "Domain error in PoissonDistributionC")
            result = igammaf.incompletegamma(k + 1, m)
            Return result
        End Function


        '************************************************************************
        '        Inverse Poisson distribution
        '
        '        Finds the Poisson variable x such that the integral
        '        from 0 to x of the Poisson density is equal to the
        '        given probability y.
        '
        '        This is accomplished using the inverse gamma integral
        '        function and the relation
        '
        '           m = igami( k+1, y ).
        '
        '        ACCURACY:
        '
        '        See inverse incomplete gamma function
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function invpoissondistribution(k As Integer, y As Double) As Double
            Dim result As Double = 0

            alglib.ap.assert((k >= 0 AndAlso CDbl(y) >= CDbl(0)) AndAlso CDbl(y) < CDbl(1), "Domain error in InvPoissonDistribution")
            result = igammaf.invincompletegammac(k + 1, y)
            Return result
        End Function


    End Class
    Public Class psif
        '************************************************************************
        '        Psi (digamma) function
        '
        '                     d      -
        '          psi(x)  =  -- ln | (x)
        '                     dx
        '
        '        is the logarithmic derivative of the gamma function.
        '        For integer x,
        '                          n-1
        '                           -
        '        psi(n) = -EUL  +   >  1/k.
        '                           -
        '                          k=1
        '
        '        This formula is used for 0 < n <= 10.  If x is negative, it
        '        is transformed to a positive argument by the reflection
        '        formula  psi(1-x) = psi(x) + pi cot(pi x).
        '        For general positive x, the argument is made greater than 10
        '        using the recurrence  psi(x+1) = psi(x) + 1/x.
        '        Then the following asymptotic expansion is applied:
        '
        '                                  inf.   B
        '                                   -      2k
        '        psi(x) = log(x) - 1/2x -   >   -------
        '                                   -        2k
        '                                  k=1   2k x
        '
        '        where the B2k are Bernoulli numbers.
        '
        '        ACCURACY:
        '           Relative error (except absolute when |psi| < 1):
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE      0,30        30000       1.3e-15     1.4e-16
        '           IEEE      -30,0       40000       1.5e-15     2.2e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1992, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function psi(x As Double) As Double
            Dim result As Double = 0
            Dim p As Double = 0
            Dim q As Double = 0
            Dim nz As Double = 0
            Dim s As Double = 0
            Dim w As Double = 0
            Dim y As Double = 0
            Dim z As Double = 0
            Dim polv As Double = 0
            Dim i As Integer = 0
            Dim n As Integer = 0
            Dim negative As Integer = 0

            negative = 0
            nz = 0.0
            If CDbl(x) <= CDbl(0) Then
                negative = 1
                q = x
                p = CInt(System.Math.Truncate(System.Math.Floor(q)))
                If CDbl(p) = CDbl(q) Then
                    alglib.ap.assert(False, "Singularity in Psi(x)")
                    result = Math.maxrealnumber
                    Return result
                End If
                nz = q - p
                If CDbl(nz) <> CDbl(0.5) Then
                    If CDbl(nz) > CDbl(0.5) Then
                        p = p + 1.0
                        nz = q - p
                    End If
                    nz = System.Math.PI / System.Math.Tan(System.Math.PI * nz)
                Else
                    nz = 0.0
                End If
                x = 1.0 - x
            End If
            If CDbl(x) <= CDbl(10.0) AndAlso CDbl(x) = CDbl(CInt(System.Math.Truncate(System.Math.Floor(x)))) Then
                y = 0.0
                n = CInt(System.Math.Truncate(System.Math.Floor(x)))
                For i = 1 To n - 1
                    w = i
                    y = y + 1.0 / w
                Next
                y = y - 0.577215664901533
            Else
                s = x
                w = 0.0
                While CDbl(s) < CDbl(10.0)
                    w = w + 1.0 / s
                    s = s + 1.0
                End While
                If CDbl(s) < CDbl(1.0E+17) Then
                    z = 1.0 / (s * s)
                    polv = 0.0833333333333333
                    polv = polv * z - 0.0210927960927961
                    polv = polv * z + 0.00757575757575758
                    polv = polv * z - 0.00416666666666667
                    polv = polv * z + 0.00396825396825397
                    polv = polv * z - 0.00833333333333333
                    polv = polv * z + 0.0833333333333333
                    y = z * polv
                Else
                    y = 0.0
                End If
                y = System.Math.Log(s) - 0.5 / s - y - w
            End If
            If negative <> 0 Then
                y = y - nz
            End If
            result = y
            Return result
        End Function


    End Class
    Public Class studenttdistr
        '************************************************************************
        '        Student's t distribution
        '
        '        Computes the integral from minus infinity to t of the Student
        '        t distribution with integer k > 0 degrees of freedom:
        '
        '                                             t
        '                                             -
        '                                            | |
        '                     -                      |         2   -(k+1)/2
        '                    | ( (k+1)/2 )           |  (     x   )
        '              ----------------------        |  ( 1 + --- )        dx
        '                            -               |  (      k  )
        '              sqrt( k pi ) | ( k/2 )        |
        '                                          | |
        '                                           -
        '                                          -inf.
        '
        '        Relation to incomplete beta integral:
        '
        '               1 - stdtr(k,t) = 0.5 * incbet( k/2, 1/2, z )
        '        where
        '               z = k/(k + t**2).
        '
        '        For t < -2, this is the method of computation.  For higher t,
        '        a direct method is derived from integration by parts.
        '        Since the function is symmetric about t=0, the area under the
        '        right tail of the density is found by calling the function
        '        with -t instead of t.
        '
        '        ACCURACY:
        '
        '        Tested at random 1 <= k <= 25.  The "domain" refers to t.
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE     -100,-2      50000       5.9e-15     1.4e-15
        '           IEEE     -2,100      500000       2.7e-15     4.9e-17
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function studenttdistribution(k As Integer, t As Double) As Double
            Dim result As Double = 0
            Dim x As Double = 0
            Dim rk As Double = 0
            Dim z As Double = 0
            Dim f As Double = 0
            Dim tz As Double = 0
            Dim p As Double = 0
            Dim xsqk As Double = 0
            Dim j As Integer = 0

            alglib.ap.assert(k > 0, "Domain error in StudentTDistribution")
            If CDbl(t) = CDbl(0) Then
                result = 0.5
                Return result
            End If
            If CDbl(t) < CDbl(-2.0) Then
                rk = k
                z = rk / (rk + t * t)
                result = 0.5 * ibetaf.incompletebeta(0.5 * rk, 0.5, z)
                Return result
            End If
            If CDbl(t) < CDbl(0) Then
                x = -t
            Else
                x = t
            End If
            rk = k
            z = 1.0 + x * x / rk
            If k Mod 2 <> 0 Then
                xsqk = x / System.Math.sqrt(rk)
                p = System.Math.Atan(xsqk)
                If k > 1 Then
                    f = 1.0
                    tz = 1.0
                    j = 3
                    While j <= k - 2 AndAlso CDbl(tz / f) > CDbl(Math.machineepsilon)
                        tz = tz * ((j - 1) / (z * j))
                        f = f + tz
                        j = j + 2
                    End While
                    p = p + f * xsqk / z
                End If
                p = p * 2.0 / System.Math.PI
            Else
                f = 1.0
                tz = 1.0
                j = 2
                While j <= k - 2 AndAlso CDbl(tz / f) > CDbl(Math.machineepsilon)
                    tz = tz * ((j - 1) / (z * j))
                    f = f + tz
                    j = j + 2
                End While
                p = f * x / System.Math.sqrt(z * rk)
            End If
            If CDbl(t) < CDbl(0) Then
                p = -p
            End If
            result = 0.5 + 0.5 * p
            Return result
        End Function


        '************************************************************************
        '        Functional inverse of Student's t distribution
        '
        '        Given probability p, finds the argument t such that stdtr(k,t)
        '        is equal to p.
        '
        '        ACCURACY:
        '
        '        Tested at random 1 <= k <= 100.  The "domain" refers to p:
        '                             Relative error:
        '        arithmetic   domain     # trials      peak         rms
        '           IEEE    .001,.999     25000       5.7e-15     8.0e-16
        '           IEEE    10^-6,.001    25000       2.0e-12     2.9e-14
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Function invstudenttdistribution(k As Integer, p As Double) As Double
            Dim result As Double = 0
            Dim t As Double = 0
            Dim rk As Double = 0
            Dim z As Double = 0
            Dim rflg As Integer = 0

            alglib.ap.assert((k > 0 AndAlso CDbl(p) > CDbl(0)) AndAlso CDbl(p) < CDbl(1), "Domain error in InvStudentTDistribution")
            rk = k
            If CDbl(p) > CDbl(0.25) AndAlso CDbl(p) < CDbl(0.75) Then
                If CDbl(p) = CDbl(0.5) Then
                    result = 0
                    Return result
                End If
                z = 1.0 - 2.0 * p
                z = ibetaf.invincompletebeta(0.5, 0.5 * rk, System.Math.Abs(z))
                t = System.Math.sqrt(rk * z / (1.0 - z))
                If CDbl(p) < CDbl(0.5) Then
                    t = -t
                End If
                result = t
                Return result
            End If
            rflg = -1
            If CDbl(p) >= CDbl(0.5) Then
                p = 1.0 - p
                rflg = 1
            End If
            z = ibetaf.invincompletebeta(0.5 * rk, 0.5, 2.0 * p)
            If CDbl(Math.maxrealnumber * z) < CDbl(rk) Then
                result = rflg * Math.maxrealnumber
                Return result
            End If
            t = System.Math.sqrt(rk / z - rk)
            result = rflg * t
            Return result
        End Function


    End Class
    Public Class trigintegrals
        '************************************************************************
        '        Sine and cosine integrals
        '
        '        Evaluates the integrals
        '
        '                                 x
        '                                 -
        '                                |  cos t - 1
        '          Ci(x) = eul + ln x +  |  --------- dt,
        '                                |      t
        '                               -
        '                                0
        '                    x
        '                    -
        '                   |  sin t
        '          Si(x) =  |  ----- dt
        '                   |    t
        '                  -
        '                   0
        '
        '        where eul = 0.57721566490153286061 is Euler's constant.
        '        The integrals are approximated by rational functions.
        '        For x > 8 auxiliary functions f(x) and g(x) are employed
        '        such that
        '
        '        Ci(x) = f(x) sin(x) - g(x) cos(x)
        '        Si(x) = pi/2 - f(x) cos(x) - g(x) sin(x)
        '
        '
        '        ACCURACY:
        '           Test interval = [0,50].
        '        Absolute error, except relative when > 1:
        '        arithmetic   function   # trials      peak         rms
        '           IEEE        Si        30000       4.4e-16     7.3e-17
        '           IEEE        Ci        30000       6.9e-16     5.1e-17
        '
        '        Cephes Math Library Release 2.1:  January, 1989
        '        Copyright 1984, 1987, 1989 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Sub sinecosineintegrals(x As Double, ByRef si As Double, ByRef ci As Double)
            Dim z As Double = 0
            Dim c As Double = 0
            Dim s As Double = 0
            Dim f As Double = 0
            Dim g As Double = 0
            Dim sg As Integer = 0
            Dim sn As Double = 0
            Dim sd As Double = 0
            Dim cn As Double = 0
            Dim cd As Double = 0
            Dim fn As Double = 0
            Dim fd As Double = 0
            Dim gn As Double = 0
            Dim gd As Double = 0

            si = 0
            ci = 0

            If CDbl(x) < CDbl(0) Then
                sg = -1
                x = -x
            Else
                sg = 0
            End If
            If CDbl(x) = CDbl(0) Then
                si = 0
                ci = -Math.maxrealnumber
                Return
            End If
            If CDbl(x) > CDbl(1000000000.0) Then
                si = 1.5707963267949 - System.Math.Cos(x) / x
                ci = System.Math.Sin(x) / x
                Return
            End If
            If CDbl(x) <= CDbl(4) Then
                z = x * x
                sn = -0.0000000000839167827910304
                sn = sn * z + 0.0000000462591714427013
                sn = sn * z - 0.00000975759303843633
                sn = sn * z + 0.000976945438170435
                sn = sn * z - 0.0413470316229407
                sn = sn * z + 1.0
                sd = 0.00000000000203269266195952
                sd = sd * z + 0.00000000127997891179943
                sd = sd * z + 0.000000441827842801219
                sd = sd * z + 0.0000996412122043876
                sd = sd * z + 0.014208523932615
                sd = sd * z + 1.0
                s = x * sn / sd
                cn = 0.0000000000202524002389102
                cn = cn * z - 0.0000000135249504915791
                cn = cn * z + 0.00000359325051419993
                cn = cn * z - 0.000474007206873408
                cn = cn * z + 0.0289159652607555
                cn = cn * z - 1.0
                cd = 0.00000000000407746040061881
                cd = cd * z + 0.00000000306780997581888
                cd = cd * z + 0.00000123210355685883
                cd = cd * z + 0.000317442024775033
                cd = cd * z + 0.0510028056236446
                cd = cd * z + 4.0
                c = z * cn / cd
                If sg <> 0 Then
                    s = -s
                End If
                si = s
                ci = 0.577215664901533 + System.Math.Log(x) + c
                Return
            End If
            s = System.Math.Sin(x)
            c = System.Math.Cos(x)
            z = 1.0 / (x * x)
            If CDbl(x) < CDbl(8) Then
                fn = 4.23612862892217
                fn = fn * z + 5.45937717161813
                fn = fn * z + 1.62083287701538
                fn = fn * z + 0.167006611831323
                fn = fn * z + 0.00681020132472518
                fn = fn * z + 0.000108936580650329
                fn = fn * z + 0.000000548900223421374
                fd = 1.0
                fd = fd * z + 8.16496634205391
                fd = fd * z + 7.30828822505565
                fd = fd * z + 1.86792257950184
                fd = fd * z + 0.17879205296315
                fd = fd * z + 0.0070171066832279
                fd = fd * z + 0.000110034357153916
                fd = fd * z + 0.000000548900252756256
                f = fn / (x * fd)
                gn = 0.0871001698973114
                gn = gn * z + 0.611379109952219
                gn = gn * z + 0.397180296392337
                gn = gn * z + 0.0748527737628469
                gn = gn * z + 0.00538868681462177
                gn = gn * z + 0.000161999794598934
                gn = gn * z + 0.00000197963874140964
                gn = gn * z + 0.0000000078257904074409
                gd = 1.0
                gd = gd * z + 1.64402202413355
                gd = gd * z + 0.666296701268988
                gd = gd * z + 0.0988771761277689
                gd = gd * z + 0.00622396345441768
                gd = gd * z + 0.000173221081474177
                gd = gd * z + 0.00000202659182086344
                gd = gd * z + 0.00000000782579218933535
                g = z * gn / gd
            Else
                fn = 0.455880873470465
                fn = fn * z + 0.713715274100147
                fn = fn * z + 0.160300158222319
                fn = fn * z + 0.0116064229408124
                fn = fn * z + 0.000349556442447859
                fn = fn * z + 0.00000486215430826455
                fn = fn * z + 0.0000000320092790091005
                fn = fn * z + 0.0000000000941779576128513
                fn = fn * z + 0.0000000000000970507110881952
                fd = 1.0
                fd = fd * z + 0.917463611873684
                fd = fd * z + 0.178685545332075
                fd = fd * z + 0.0122253594771971
                fd = fd * z + 0.000358696481881852
                fd = fd * z + 0.00000492435064317882
                fd = fd * z + 0.0000000321956939101046
                fd = fd * z + 0.0000000000943720590350277
                fd = fd * z + 0.0000000000000970507110881952
                f = fn / (x * fd)
                gn = 0.697359953443276
                gn = gn * z + 0.330410979305632
                gn = gn * z + 0.0384878767649974
                gn = gn * z + 0.00171718239052348
                gn = gn * z + 0.0000348941165502279
                gn = gn * z + 0.000000347131167084117
                gn = gn * z + 0.00000000170404452782045
                gn = gn * z + 0.00000000000385945925430277
                gn = gn * z + 0.00000000000000314040098946363
                gd = 1.0
                gd = gd * z + 1.68548898811012
                gd = gd * z + 0.487852258695305
                gd = gd * z + 0.0467913194259626
                gd = gd * z + 0.001902844266744
                gd = gd * z + 0.0000368475504442561
                gd = gd * z + 0.000000357043223443741
                gd = gd * z + 0.00000000172693748966316
                gd = gd * z + 0.00000000000387830166023955
                gd = gd * z + 0.00000000000000314040098946363
                g = z * gn / gd
            End If
            si = 1.5707963267949 - f * c - g * s
            If sg <> 0 Then
                si = -si
            End If
            ci = f * s - g * c
        End Sub


        '************************************************************************
        '        Hyperbolic sine and cosine integrals
        '
        '        Approximates the integrals
        '
        '                                   x
        '                                   -
        '                                  | |   cosh t - 1
        '          Chi(x) = eul + ln x +   |    -----------  dt,
        '                                | |          t
        '                                 -
        '                                 0
        '
        '                      x
        '                      -
        '                     | |  sinh t
        '          Shi(x) =   |    ------  dt
        '                   | |       t
        '                    -
        '                    0
        '
        '        where eul = 0.57721566490153286061 is Euler's constant.
        '        The integrals are evaluated by power series for x < 8
        '        and by Chebyshev expansions for x between 8 and 88.
        '        For large x, both functions approach exp(x)/2x.
        '        Arguments greater than 88 in magnitude return MAXNUM.
        '
        '
        '        ACCURACY:
        '
        '        Test interval 0 to 88.
        '                             Relative error:
        '        arithmetic   function  # trials      peak         rms
        '           IEEE         Shi      30000       6.9e-16     1.6e-16
        '               Absolute error, except relative when |Chi| > 1:
        '           IEEE         Chi      30000       8.4e-16     1.4e-16
        '
        '        Cephes Math Library Release 2.8:  June, 2000
        '        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        '        ************************************************************************

        Public Shared Sub hyperbolicsinecosineintegrals(x As Double, ByRef shi As Double, ByRef chi As Double)
            Dim k As Double = 0
            Dim z As Double = 0
            Dim c As Double = 0
            Dim s As Double = 0
            Dim a As Double = 0
            Dim sg As Integer = 0
            Dim b0 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0

            shi = 0
            chi = 0

            If CDbl(x) < CDbl(0) Then
                sg = -1
                x = -x
            Else
                sg = 0
            End If
            If CDbl(x) = CDbl(0) Then
                shi = 0
                chi = -Math.maxrealnumber
                Return
            End If
            If CDbl(x) < CDbl(8.0) Then
                z = x * x
                a = 1.0
                s = 1.0
                c = 0.0
                k = 2.0
                Do
                    a = a * z / k
                    c = c + a / k
                    k = k + 1.0
                    a = a / k
                    s = s + a / k
                    k = k + 1.0
                Loop While CDbl(System.Math.Abs(a / s)) >= CDbl(Math.machineepsilon)
                s = s * x
            Else
                If CDbl(x) < CDbl(18.0) Then
                    a = (576.0 / x - 52.0) / 10.0
                    k = System.Math.Exp(x) / x
                    b0 = 1.83889230173399E-17
                    b1 = 0.0
                    chebiterationshichi(a, -9.55485532279656E-17, b0, b1, b2)
                    chebiterationshichi(a, 0.00000000000000020432610598088, b0, b1, b2)
                    chebiterationshichi(a, 0.00000000000000109896949074905, b0, b1, b2)
                    chebiterationshichi(a, -0.0000000000000131313534344093, b0, b1, b2)
                    chebiterationshichi(a, 0.0000000000000593976226264314, b0, b1, b2)
                    chebiterationshichi(a, -0.0000000000000347197010497749, b0, b1, b2)
                    chebiterationshichi(a, -0.00000000000140059764613117, b0, b1, b2)
                    chebiterationshichi(a, 0.00000000000949044626224224, b0, b1, b2)
                    chebiterationshichi(a, -0.0000000000161596181145435, b0, b1, b2)
                    chebiterationshichi(a, -0.00000000017789978443643, b0, b1, b2)
                    chebiterationshichi(a, 0.00000000135455469767247, b0, b1, b2)
                    chebiterationshichi(a, -0.00000000103257121792819, b0, b1, b2)
                    chebiterationshichi(a, -0.0000000356699611114983, b0, b1, b2)
                    chebiterationshichi(a, 0.000000144818877384267, b0, b1, b2)
                    chebiterationshichi(a, 0.000000782018215184051, b0, b1, b2)
                    chebiterationshichi(a, -0.00000539919118403805, b0, b1, b2)
                    chebiterationshichi(a, -0.000031245820216896, b0, b1, b2)
                    chebiterationshichi(a, 0.0000890136741950727, b0, b1, b2)
                    chebiterationshichi(a, 0.00202558474743847, b0, b1, b2)
                    chebiterationshichi(a, 0.0296064440855633, b0, b1, b2)
                    chebiterationshichi(a, 1.11847751047257, b0, b1, b2)
                    s = k * 0.5 * (b0 - b2)
                    b0 = -8.12435385225864E-18
                    b1 = 0.0
                    chebiterationshichi(a, 2.17586413290339E-17, b0, b1, b2)
                    chebiterationshichi(a, 5.22624394924072E-17, b0, b1, b2)
                    chebiterationshichi(a, -0.000000000000000948812110591691, b0, b1, b2)
                    chebiterationshichi(a, 0.00000000000000535546311647465, b0, b1, b2)
                    chebiterationshichi(a, -0.0000000000000121009970113733, b0, b1, b2)
                    chebiterationshichi(a, -0.0000000000000600865178553447, b0, b1, b2)
                    chebiterationshichi(a, 0.000000000000716339649156029, b0, b1, b2)
                    chebiterationshichi(a, -0.000000000002934960726076, b0, b1, b2)
                    chebiterationshichi(a, -0.00000000000140359438136491, b0, b1, b2)
                    chebiterationshichi(a, 0.0000000000876302288609055, b0, b1, b2)
                    chebiterationshichi(a, -0.000000000440092476213282, b0, b1, b2)
                    chebiterationshichi(a, -0.000000000187992075640569, b0, b1, b2)
                    chebiterationshichi(a, 0.0000000131458150989475, b0, b1, b2)
                    chebiterationshichi(a, -0.0000000475513930924765, b0, b1, b2)
                    chebiterationshichi(a, -0.000000221775018801849, b0, b1, b2)
                    chebiterationshichi(a, 0.00000194635531373272, b0, b1, b2)
                    chebiterationshichi(a, 0.00000433505889257316, b0, b1, b2)
                    chebiterationshichi(a, -0.0000613387001076494, b0, b1, b2)
                    chebiterationshichi(a, -0.000313085477492997, b0, b1, b2)
                    chebiterationshichi(a, 0.000497164789823116, b0, b1, b2)
                    chebiterationshichi(a, 0.0264347496031375, b0, b1, b2)
                    chebiterationshichi(a, 1.11446150876699, b0, b1, b2)
                    c = k * 0.5 * (b0 - b2)
                Else
                    If CDbl(x) <= CDbl(88.0) Then
                        a = (6336.0 / x - 212.0) / 70.0
                        k = System.Math.Exp(x) / x
                        b0 = -1.05311574154851E-17
                        b1 = 0.0
                        chebiterationshichi(a, 2.62446095596355E-17, b0, b1, b2)
                        chebiterationshichi(a, 8.82090135625368E-17, b0, b1, b2)
                        chebiterationshichi(a, -0.000000000000000338459811878103, b0, b1, b2)
                        chebiterationshichi(a, -0.000000000000000830608026366936, b0, b1, b2)
                        chebiterationshichi(a, 0.0000000000000039339787543705, b0, b1, b2)
                        chebiterationshichi(a, 0.0000000000000101765565969729, b0, b1, b2)
                        chebiterationshichi(a, -0.0000000000000421128170307641, b0, b1, b2)
                        chebiterationshichi(a, -0.000000000000160818204519802, b0, b1, b2)
                        chebiterationshichi(a, 0.000000000000334714954175994, b0, b1, b2)
                        chebiterationshichi(a, 0.00000000000272600352129153, b0, b1, b2)
                        chebiterationshichi(a, 0.00000000000166894954752839, b0, b1, b2)
                        chebiterationshichi(a, -0.0000000000349278141024731, b0, b1, b2)
                        chebiterationshichi(a, -0.000000000158580661666483, b0, b1, b2)
                        chebiterationshichi(a, -0.000000000179289437183356, b0, b1, b2)
                        chebiterationshichi(a, 0.00000000176281629144265, b0, b1, b2)
                        chebiterationshichi(a, 0.0000000169050228879421, b0, b1, b2)
                        chebiterationshichi(a, 0.000000125391771228487, b0, b1, b2)
                        chebiterationshichi(a, 0.00000116229947068677, b0, b1, b2)
                        chebiterationshichi(a, 0.0000161038260117376, b0, b1, b2)
                        chebiterationshichi(a, 0.000349810375601054, b0, b1, b2)
                        chebiterationshichi(a, 0.0128478065259648, b0, b1, b2)
                        chebiterationshichi(a, 1.03665722588798, b0, b1, b2)
                        s = k * 0.5 * (b0 - b2)
                        b0 = 8.06913408255156E-18
                        b1 = 0.0
                        chebiterationshichi(a, -2.08074168180148E-17, b0, b1, b2)
                        chebiterationshichi(a, -5.98111329658272E-17, b0, b1, b2)
                        chebiterationshichi(a, 0.000000000000000268533951085946, b0, b1, b2)
                        chebiterationshichi(a, 0.000000000000000452313941698905, b0, b1, b2)
                        chebiterationshichi(a, -0.00000000000000310734917335299, b0, b1, b2)
                        chebiterationshichi(a, -0.00000000000000442823207332532, b0, b1, b2)
                        chebiterationshichi(a, 0.0000000000000349639695410807, b0, b1, b2)
                        chebiterationshichi(a, 0.0000000000000663406731718912, b0, b1, b2)
                        chebiterationshichi(a, -0.000000000000371902448093119, b0, b1, b2)
                        chebiterationshichi(a, -0.00000000000127135418132338, b0, b1, b2)
                        chebiterationshichi(a, 0.00000000000274851141935315, b0, b1, b2)
                        chebiterationshichi(a, 0.0000000000233781843985453, b0, b1, b2)
                        chebiterationshichi(a, 0.0000000000271436006377612, b0, b1, b2)
                        chebiterationshichi(a, -0.000000000256600180000356, b0, b1, b2)
                        chebiterationshichi(a, -0.00000000161021375163803, b0, b1, b2)
                        chebiterationshichi(a, -0.00000000472543064876272, b0, b1, b2)
                        chebiterationshichi(a, -0.00000000300095178028682, b0, b1, b2)
                        chebiterationshichi(a, 0.0000000779387474390915, b0, b1, b2)
                        chebiterationshichi(a, 0.00000106942765566402, b0, b1, b2)
                        chebiterationshichi(a, 0.0000159503164802313, b0, b1, b2)
                        chebiterationshichi(a, 0.000349592575153778, b0, b1, b2)
                        chebiterationshichi(a, 0.0128475387530065, b0, b1, b2)
                        chebiterationshichi(a, 1.03665693917934, b0, b1, b2)
                        c = k * 0.5 * (b0 - b2)
                    Else
                        If sg <> 0 Then
                            shi = -Math.maxrealnumber
                        Else
                            shi = Math.maxrealnumber
                        End If
                        chi = Math.maxrealnumber
                        Return
                    End If
                End If
            End If
            If sg <> 0 Then
                s = -s
            End If
            shi = s
            chi = 0.577215664901533 + System.Math.Log(x) + c
        End Sub


        Private Shared Sub chebiterationshichi(x As Double, c As Double, ByRef b0 As Double, ByRef b1 As Double, ByRef b2 As Double)
            b2 = b1
            b1 = b0
            b0 = x * b1 - b2 + c
        End Sub


    End Class
End Class

