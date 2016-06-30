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

''#Pragma warning disable 162
''#Pragma warning disable 219

Public Partial Class alglib


	'************************************************************************
'    1-dimensional complex FFT.
'
'    Array size N may be arbitrary number (composite or prime).  Composite  N's
'    are handled with cache-oblivious variation of  a  Cooley-Tukey  algorithm.
'    Small prime-factors are transformed using hard coded  codelets (similar to
'    FFTW codelets, but without low-level  optimization),  large  prime-factors
'    are handled with Bluestein's algorithm.
'
'    Fastests transforms are for smooth N's (prime factors are 2, 3,  5  only),
'    most fast for powers of 2. When N have prime factors  larger  than  these,
'    but orders of magnitude smaller than N, computations will be about 4 times
'    slower than for nearby highly composite N's. When N itself is prime, speed
'    will be 6 times lower.
'
'    Algorithm has O(N*logN) complexity for any N (composite or prime).
'
'    INPUT PARAMETERS
'        A   -   array[0..N-1] - complex function to be transformed
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        A   -   DFT of a input array, array[0..N-1]
'                A_out[j] = SUM(A_in[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)
'
'
'      -- ALGLIB --
'         Copyright 29.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub fftc1d(ByRef a As complex(), n As Integer)

		fft.fftc1d(a, n)
		Return
	End Sub
	Public Shared Sub fftc1d(ByRef a As complex())
		Dim n As Integer


		n = ap.len(a)
		fft.fftc1d(a, n)

		Return
	End Sub

	'************************************************************************
'    1-dimensional complex inverse FFT.
'
'    Array size N may be arbitrary number (composite or prime).  Algorithm  has
'    O(N*logN) complexity for any N (composite or prime).
'
'    See FFTC1D() description for more information about algorithm performance.
'
'    INPUT PARAMETERS
'        A   -   array[0..N-1] - complex array to be transformed
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        A   -   inverse DFT of a input array, array[0..N-1]
'                A_out[j] = SUM(A_in[k]/N*exp(+2*pi*sqrt(-1)*j*k/N), k = 0..N-1)
'
'
'      -- ALGLIB --
'         Copyright 29.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub fftc1dinv(ByRef a As complex(), n As Integer)

		fft.fftc1dinv(a, n)
		Return
	End Sub
	Public Shared Sub fftc1dinv(ByRef a As complex())
		Dim n As Integer


		n = ap.len(a)
		fft.fftc1dinv(a, n)

		Return
	End Sub

	'************************************************************************
'    1-dimensional real FFT.
'
'    Algorithm has O(N*logN) complexity for any N (composite or prime).
'
'    INPUT PARAMETERS
'        A   -   array[0..N-1] - real function to be transformed
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        F   -   DFT of a input array, array[0..N-1]
'                F[j] = SUM(A[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)
'
'    NOTE:
'        F[] satisfies symmetry property F[k] = conj(F[N-k]),  so just one half
'    of  array  is  usually needed. But for convinience subroutine returns full
'    complex array (with frequencies above N/2), so its result may be  used  by
'    other FFT-related subroutines.
'
'
'      -- ALGLIB --
'         Copyright 01.06.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub fftr1d(a As Double(), n As Integer, ByRef f As complex())
		f = New complex(-1) {}
		fft.fftr1d(a, n, f)
		Return
	End Sub
	Public Shared Sub fftr1d(a As Double(), ByRef f As complex())
		Dim n As Integer

		f = New complex(-1) {}
		n = ap.len(a)
		fft.fftr1d(a, n, f)

		Return
	End Sub

	'************************************************************************
'    1-dimensional real inverse FFT.
'
'    Algorithm has O(N*logN) complexity for any N (composite or prime).
'
'    INPUT PARAMETERS
'        F   -   array[0..floor(N/2)] - frequencies from forward real FFT
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        A   -   inverse DFT of a input array, array[0..N-1]
'
'    NOTE:
'        F[] should satisfy symmetry property F[k] = conj(F[N-k]), so just  one
'    half of frequencies array is needed - elements from 0 to floor(N/2).  F[0]
'    is ALWAYS real. If N is even F[floor(N/2)] is real too. If N is odd,  then
'    F[floor(N/2)] has no special properties.
'
'    Relying on properties noted above, FFTR1DInv subroutine uses only elements
'    from 0th to floor(N/2)-th. It ignores imaginary part of F[0],  and in case
'    N is even it ignores imaginary part of F[floor(N/2)] too.
'
'    When you call this function using full arguments list - "FFTR1DInv(F,N,A)"
'    - you can pass either either frequencies array with N elements or  reduced
'    array with roughly N/2 elements - subroutine will  successfully  transform
'    both.
'
'    If you call this function using reduced arguments list -  "FFTR1DInv(F,A)"
'    - you must pass FULL array with N elements (although higher  N/2 are still
'    not used) because array size is used to automatically determine FFT length
'
'
'      -- ALGLIB --
'         Copyright 01.06.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub fftr1dinv(f As complex(), n As Integer, ByRef a As Double())
		a = New Double(-1) {}
		fft.fftr1dinv(f, n, a)
		Return
	End Sub
	Public Shared Sub fftr1dinv(f As complex(), ByRef a As Double())
		Dim n As Integer

		a = New Double(-1) {}
		n = ap.len(f)
		fft.fftr1dinv(f, n, a)

		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    1-dimensional complex convolution.
'
'    For given A/B returns conv(A,B) (non-circular). Subroutine can automatically
'    choose between three implementations: straightforward O(M*N)  formula  for
'    very small N (or M), overlap-add algorithm for  cases  where  max(M,N)  is
'    significantly larger than min(M,N), but O(M*N) algorithm is too slow,  and
'    general FFT-based formula for cases where two previois algorithms are  too
'    slow.
'
'    Algorithm has max(M,N)*log(max(M,N)) complexity for any M/N.
'
'    INPUT PARAMETERS
'        A   -   array[0..M-1] - complex function to be transformed
'        M   -   problem size
'        B   -   array[0..N-1] - complex function to be transformed
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        R   -   convolution: A*B. array[0..N+M-2].
'
'    NOTE:
'        It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
'    functions have non-zero values at negative T's, you  can  still  use  this
'    subroutine - just shift its result correspondingly.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub convc1d(a As complex(), m As Integer, b As complex(), n As Integer, ByRef r As complex())
		r = New complex(-1) {}
		conv.convc1d(a, m, b, n, r)
		Return
	End Sub

	'************************************************************************
'    1-dimensional complex non-circular deconvolution (inverse of ConvC1D()).
'
'    Algorithm has M*log(M)) complexity for any M (composite or prime).
'
'    INPUT PARAMETERS
'        A   -   array[0..M-1] - convolved signal, A = conv(R, B)
'        M   -   convolved signal length
'        B   -   array[0..N-1] - response
'        N   -   response length, N<=M
'
'    OUTPUT PARAMETERS
'        R   -   deconvolved signal. array[0..M-N].
'
'    NOTE:
'        deconvolution is unstable process and may result in division  by  zero
'    (if your response function is degenerate, i.e. has zero Fourier coefficient).
'
'    NOTE:
'        It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
'    functions have non-zero values at negative T's, you  can  still  use  this
'    subroutine - just shift its result correspondingly.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub convc1dinv(a As complex(), m As Integer, b As complex(), n As Integer, ByRef r As complex())
		r = New complex(-1) {}
		conv.convc1dinv(a, m, b, n, r)
		Return
	End Sub

	'************************************************************************
'    1-dimensional circular complex convolution.
'
'    For given S/R returns conv(S,R) (circular). Algorithm has linearithmic
'    complexity for any M/N.
'
'    IMPORTANT:  normal convolution is commutative,  i.e.   it  is symmetric  -
'    conv(A,B)=conv(B,A).  Cyclic convolution IS NOT.  One function - S - is  a
'    signal,  periodic function, and another - R - is a response,  non-periodic
'    function with limited length.
'
'    INPUT PARAMETERS
'        S   -   array[0..M-1] - complex periodic signal
'        M   -   problem size
'        B   -   array[0..N-1] - complex non-periodic response
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        R   -   convolution: A*B. array[0..M-1].
'
'    NOTE:
'        It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
'    negative T's, you can still use this subroutine - just  shift  its  result
'    correspondingly.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub convc1dcircular(s As complex(), m As Integer, r As complex(), n As Integer, ByRef c As complex())
		c = New complex(-1) {}
		conv.convc1dcircular(s, m, r, n, c)
		Return
	End Sub

	'************************************************************************
'    1-dimensional circular complex deconvolution (inverse of ConvC1DCircular()).
'
'    Algorithm has M*log(M)) complexity for any M (composite or prime).
'
'    INPUT PARAMETERS
'        A   -   array[0..M-1] - convolved periodic signal, A = conv(R, B)
'        M   -   convolved signal length
'        B   -   array[0..N-1] - non-periodic response
'        N   -   response length
'
'    OUTPUT PARAMETERS
'        R   -   deconvolved signal. array[0..M-1].
'
'    NOTE:
'        deconvolution is unstable process and may result in division  by  zero
'    (if your response function is degenerate, i.e. has zero Fourier coefficient).
'
'    NOTE:
'        It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
'    negative T's, you can still use this subroutine - just  shift  its  result
'    correspondingly.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub convc1dcircularinv(a As complex(), m As Integer, b As complex(), n As Integer, ByRef r As complex())
		r = New complex(-1) {}
		conv.convc1dcircularinv(a, m, b, n, r)
		Return
	End Sub

	'************************************************************************
'    1-dimensional real convolution.
'
'    Analogous to ConvC1D(), see ConvC1D() comments for more details.
'
'    INPUT PARAMETERS
'        A   -   array[0..M-1] - real function to be transformed
'        M   -   problem size
'        B   -   array[0..N-1] - real function to be transformed
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        R   -   convolution: A*B. array[0..N+M-2].
'
'    NOTE:
'        It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
'    functions have non-zero values at negative T's, you  can  still  use  this
'    subroutine - just shift its result correspondingly.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub convr1d(a As Double(), m As Integer, b As Double(), n As Integer, ByRef r As Double())
		r = New Double(-1) {}
		conv.convr1d(a, m, b, n, r)
		Return
	End Sub

	'************************************************************************
'    1-dimensional real deconvolution (inverse of ConvC1D()).
'
'    Algorithm has M*log(M)) complexity for any M (composite or prime).
'
'    INPUT PARAMETERS
'        A   -   array[0..M-1] - convolved signal, A = conv(R, B)
'        M   -   convolved signal length
'        B   -   array[0..N-1] - response
'        N   -   response length, N<=M
'
'    OUTPUT PARAMETERS
'        R   -   deconvolved signal. array[0..M-N].
'
'    NOTE:
'        deconvolution is unstable process and may result in division  by  zero
'    (if your response function is degenerate, i.e. has zero Fourier coefficient).
'
'    NOTE:
'        It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
'    functions have non-zero values at negative T's, you  can  still  use  this
'    subroutine - just shift its result correspondingly.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub convr1dinv(a As Double(), m As Integer, b As Double(), n As Integer, ByRef r As Double())
		r = New Double(-1) {}
		conv.convr1dinv(a, m, b, n, r)
		Return
	End Sub

	'************************************************************************
'    1-dimensional circular real convolution.
'
'    Analogous to ConvC1DCircular(), see ConvC1DCircular() comments for more details.
'
'    INPUT PARAMETERS
'        S   -   array[0..M-1] - real signal
'        M   -   problem size
'        B   -   array[0..N-1] - real response
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        R   -   convolution: A*B. array[0..M-1].
'
'    NOTE:
'        It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
'    negative T's, you can still use this subroutine - just  shift  its  result
'    correspondingly.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub convr1dcircular(s As Double(), m As Integer, r As Double(), n As Integer, ByRef c As Double())
		c = New Double(-1) {}
		conv.convr1dcircular(s, m, r, n, c)
		Return
	End Sub

	'************************************************************************
'    1-dimensional complex deconvolution (inverse of ConvC1D()).
'
'    Algorithm has M*log(M)) complexity for any M (composite or prime).
'
'    INPUT PARAMETERS
'        A   -   array[0..M-1] - convolved signal, A = conv(R, B)
'        M   -   convolved signal length
'        B   -   array[0..N-1] - response
'        N   -   response length
'
'    OUTPUT PARAMETERS
'        R   -   deconvolved signal. array[0..M-N].
'
'    NOTE:
'        deconvolution is unstable process and may result in division  by  zero
'    (if your response function is degenerate, i.e. has zero Fourier coefficient).
'
'    NOTE:
'        It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
'    negative T's, you can still use this subroutine - just  shift  its  result
'    correspondingly.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub convr1dcircularinv(a As Double(), m As Integer, b As Double(), n As Integer, ByRef r As Double())
		r = New Double(-1) {}
		conv.convr1dcircularinv(a, m, b, n, r)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    1-dimensional complex cross-correlation.
'
'    For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).
'
'    Correlation is calculated using reduction to  convolution.  Algorithm with
'    max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
'    about performance).
'
'    IMPORTANT:
'        for  historical reasons subroutine accepts its parameters in  reversed
'        order: CorrC1D(Signal, Pattern) = Pattern x Signal (using  traditional
'        definition of cross-correlation, denoting cross-correlation as "x").
'
'    INPUT PARAMETERS
'        Signal  -   array[0..N-1] - complex function to be transformed,
'                    signal containing pattern
'        N       -   problem size
'        Pattern -   array[0..M-1] - complex function to be transformed,
'                    pattern to search withing signal
'        M       -   problem size
'
'    OUTPUT PARAMETERS
'        R       -   cross-correlation, array[0..N+M-2]:
'                    * positive lags are stored in R[0..N-1],
'                      R[i] = sum(conj(pattern[j])*signal[i+j]
'                    * negative lags are stored in R[N..N+M-2],
'                      R[N+M-1-i] = sum(conj(pattern[j])*signal[-i+j]
'
'    NOTE:
'        It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
'    on [-K..M-1],  you can still use this subroutine, just shift result by K.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub corrc1d(signal As complex(), n As Integer, pattern As complex(), m As Integer, ByRef r As complex())
		r = New complex(-1) {}
		corr.corrc1d(signal, n, pattern, m, r)
		Return
	End Sub

	'************************************************************************
'    1-dimensional circular complex cross-correlation.
'
'    For given Pattern/Signal returns corr(Pattern,Signal) (circular).
'    Algorithm has linearithmic complexity for any M/N.
'
'    IMPORTANT:
'        for  historical reasons subroutine accepts its parameters in  reversed
'        order:   CorrC1DCircular(Signal, Pattern) = Pattern x Signal    (using
'        traditional definition of cross-correlation, denoting cross-correlation
'        as "x").
'
'    INPUT PARAMETERS
'        Signal  -   array[0..N-1] - complex function to be transformed,
'                    periodic signal containing pattern
'        N       -   problem size
'        Pattern -   array[0..M-1] - complex function to be transformed,
'                    non-periodic pattern to search withing signal
'        M       -   problem size
'
'    OUTPUT PARAMETERS
'        R   -   convolution: A*B. array[0..M-1].
'
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub corrc1dcircular(signal As complex(), m As Integer, pattern As complex(), n As Integer, ByRef c As complex())
		c = New complex(-1) {}
		corr.corrc1dcircular(signal, m, pattern, n, c)
		Return
	End Sub

	'************************************************************************
'    1-dimensional real cross-correlation.
'
'    For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).
'
'    Correlation is calculated using reduction to  convolution.  Algorithm with
'    max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
'    about performance).
'
'    IMPORTANT:
'        for  historical reasons subroutine accepts its parameters in  reversed
'        order: CorrR1D(Signal, Pattern) = Pattern x Signal (using  traditional
'        definition of cross-correlation, denoting cross-correlation as "x").
'
'    INPUT PARAMETERS
'        Signal  -   array[0..N-1] - real function to be transformed,
'                    signal containing pattern
'        N       -   problem size
'        Pattern -   array[0..M-1] - real function to be transformed,
'                    pattern to search withing signal
'        M       -   problem size
'
'    OUTPUT PARAMETERS
'        R       -   cross-correlation, array[0..N+M-2]:
'                    * positive lags are stored in R[0..N-1],
'                      R[i] = sum(pattern[j]*signal[i+j]
'                    * negative lags are stored in R[N..N+M-2],
'                      R[N+M-1-i] = sum(pattern[j]*signal[-i+j]
'
'    NOTE:
'        It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
'    on [-K..M-1],  you can still use this subroutine, just shift result by K.
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub corrr1d(signal As Double(), n As Integer, pattern As Double(), m As Integer, ByRef r As Double())
		r = New Double(-1) {}
		corr.corrr1d(signal, n, pattern, m, r)
		Return
	End Sub

	'************************************************************************
'    1-dimensional circular real cross-correlation.
'
'    For given Pattern/Signal returns corr(Pattern,Signal) (circular).
'    Algorithm has linearithmic complexity for any M/N.
'
'    IMPORTANT:
'        for  historical reasons subroutine accepts its parameters in  reversed
'        order:   CorrR1DCircular(Signal, Pattern) = Pattern x Signal    (using
'        traditional definition of cross-correlation, denoting cross-correlation
'        as "x").
'
'    INPUT PARAMETERS
'        Signal  -   array[0..N-1] - real function to be transformed,
'                    periodic signal containing pattern
'        N       -   problem size
'        Pattern -   array[0..M-1] - real function to be transformed,
'                    non-periodic pattern to search withing signal
'        M       -   problem size
'
'    OUTPUT PARAMETERS
'        R   -   convolution: A*B. array[0..M-1].
'
'
'      -- ALGLIB --
'         Copyright 21.07.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub corrr1dcircular(signal As Double(), m As Integer, pattern As Double(), n As Integer, ByRef c As Double())
		c = New Double(-1) {}
		corr.corrr1dcircular(signal, m, pattern, n, c)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    1-dimensional Fast Hartley Transform.
'
'    Algorithm has O(N*logN) complexity for any N (composite or prime).
'
'    INPUT PARAMETERS
'        A   -   array[0..N-1] - real function to be transformed
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        A   -   FHT of a input array, array[0..N-1],
'                A_out[k] = sum(A_in[j]*(cos(2*pi*j*k/N)+sin(2*pi*j*k/N)), j=0..N-1)
'
'
'      -- ALGLIB --
'         Copyright 04.06.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub fhtr1d(ByRef a As Double(), n As Integer)

		fht.fhtr1d(a, n)
		Return
	End Sub

	'************************************************************************
'    1-dimensional inverse FHT.
'
'    Algorithm has O(N*logN) complexity for any N (composite or prime).
'
'    INPUT PARAMETERS
'        A   -   array[0..N-1] - complex array to be transformed
'        N   -   problem size
'
'    OUTPUT PARAMETERS
'        A   -   inverse FHT of a input array, array[0..N-1]
'
'
'      -- ALGLIB --
'         Copyright 29.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub fhtr1dinv(ByRef a As Double(), n As Integer)

		fht.fhtr1dinv(a, n)
		Return
	End Sub

End Class
Public Partial Class alglib
	Public Class fft
		'************************************************************************
'        1-dimensional complex FFT.
'
'        Array size N may be arbitrary number (composite or prime).  Composite  N's
'        are handled with cache-oblivious variation of  a  Cooley-Tukey  algorithm.
'        Small prime-factors are transformed using hard coded  codelets (similar to
'        FFTW codelets, but without low-level  optimization),  large  prime-factors
'        are handled with Bluestein's algorithm.
'
'        Fastests transforms are for smooth N's (prime factors are 2, 3,  5  only),
'        most fast for powers of 2. When N have prime factors  larger  than  these,
'        but orders of magnitude smaller than N, computations will be about 4 times
'        slower than for nearby highly composite N's. When N itself is prime, speed
'        will be 6 times lower.
'
'        Algorithm has O(N*logN) complexity for any N (composite or prime).
'
'        INPUT PARAMETERS
'            A   -   array[0..N-1] - complex function to be transformed
'            N   -   problem size
'            
'        OUTPUT PARAMETERS
'            A   -   DFT of a input array, array[0..N-1]
'                    A_out[j] = SUM(A_in[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)
'
'
'          -- ALGLIB --
'             Copyright 29.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub fftc1d(ByRef a As complex(), n As Integer)
			Dim plan As New ftbase.fasttransformplan()
			Dim i As Integer = 0
			Dim buf As Double() = New Double(-1) {}

			alglib.ap.assert(n > 0, "FFTC1D: incorrect N!")
			alglib.ap.assert(alglib.ap.len(a) >= n, "FFTC1D: Length(A)<N!")
			alglib.ap.assert(apserv.isfinitecvector(a, n), "FFTC1D: A contains infinite or NAN values!")

			'
			' Special case: N=1, FFT is just identity transform.
			' After this block we assume that N is strictly greater than 1.
			'
			If n = 1 Then
				Return
			End If

			'
			' convert input array to the more convinient format
			'
			buf = New Double(2 * n - 1) {}
			For i = 0 To n - 1
				buf(2 * i + 0) = a(i).x
				buf(2 * i + 1) = a(i).y
			Next

			'
			' Generate plan and execute it.
			'
			' Plan is a combination of a successive factorizations of N and
			' precomputed data. It is much like a FFTW plan, but is not stored
			' between subroutine calls and is much simpler.
			'
			ftbase.ftcomplexfftplan(n, 1, plan)
			ftbase.ftapplyplan(plan, buf, 0, 1)

			'
			' result
			'
			For i = 0 To n - 1
				a(i).x = buf(2 * i + 0)
				a(i).y = buf(2 * i + 1)
			Next
		End Sub


		'************************************************************************
'        1-dimensional complex inverse FFT.
'
'        Array size N may be arbitrary number (composite or prime).  Algorithm  has
'        O(N*logN) complexity for any N (composite or prime).
'
'        See FFTC1D() description for more information about algorithm performance.
'
'        INPUT PARAMETERS
'            A   -   array[0..N-1] - complex array to be transformed
'            N   -   problem size
'
'        OUTPUT PARAMETERS
'            A   -   inverse DFT of a input array, array[0..N-1]
'                    A_out[j] = SUM(A_in[k]/N*exp(+2*pi*sqrt(-1)*j*k/N), k = 0..N-1)
'
'
'          -- ALGLIB --
'             Copyright 29.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub fftc1dinv(ByRef a As complex(), n As Integer)
			Dim i As Integer = 0

			alglib.ap.assert(n > 0, "FFTC1DInv: incorrect N!")
			alglib.ap.assert(alglib.ap.len(a) >= n, "FFTC1DInv: Length(A)<N!")
			alglib.ap.assert(apserv.isfinitecvector(a, n), "FFTC1DInv: A contains infinite or NAN values!")

			'
			' Inverse DFT can be expressed in terms of the DFT as
			'
			'     invfft(x) = fft(x')'/N
			'
			' here x' means conj(x).
			'
			For i = 0 To n - 1
				a(i).y = -a(i).y
			Next
			fftc1d(a, n)
			For i = 0 To n - 1
				a(i).x = a(i).x / n
				a(i).y = -(a(i).y / n)
			Next
		End Sub


		'************************************************************************
'        1-dimensional real FFT.
'
'        Algorithm has O(N*logN) complexity for any N (composite or prime).
'
'        INPUT PARAMETERS
'            A   -   array[0..N-1] - real function to be transformed
'            N   -   problem size
'
'        OUTPUT PARAMETERS
'            F   -   DFT of a input array, array[0..N-1]
'                    F[j] = SUM(A[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)
'
'        NOTE:
'            F[] satisfies symmetry property F[k] = conj(F[N-k]),  so just one half
'        of  array  is  usually needed. But for convinience subroutine returns full
'        complex array (with frequencies above N/2), so its result may be  used  by
'        other FFT-related subroutines.
'
'
'          -- ALGLIB --
'             Copyright 01.06.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub fftr1d(a As Double(), n As Integer, ByRef f As complex())
			Dim i As Integer = 0
			Dim n2 As Integer = 0
			Dim idx As Integer = 0
			Dim hn As complex = 0
			Dim hmnc As complex = 0
			Dim v As complex = 0
			Dim buf As Double() = New Double(-1) {}
			Dim plan As New ftbase.fasttransformplan()
			Dim i_ As Integer = 0

			f = New complex(-1) {}

			alglib.ap.assert(n > 0, "FFTR1D: incorrect N!")
			alglib.ap.assert(alglib.ap.len(a) >= n, "FFTR1D: Length(A)<N!")
			alglib.ap.assert(apserv.isfinitevector(a, n), "FFTR1D: A contains infinite or NAN values!")

			'
			' Special cases:
			' * N=1, FFT is just identity transform.
			' * N=2, FFT is simple too
			'
			' After this block we assume that N is strictly greater than 2
			'
			If n = 1 Then
				f = New complex(0) {}
				f(0) = a(0)
				Return
			End If
			If n = 2 Then
				f = New complex(1) {}
				f(0).x = a(0) + a(1)
				f(0).y = 0
				f(1).x = a(0) - a(1)
				f(1).y = 0
				Return
			End If

			'
			' Choose between odd-size and even-size FFTs
			'
			If n Mod 2 = 0 Then

				'
				' even-size real FFT, use reduction to the complex task
				'
				n2 = n \ 2
				buf = New Double(n - 1) {}
				For i_ = 0 To n - 1
					buf(i_) = a(i_)
				Next
				ftbase.ftcomplexfftplan(n2, 1, plan)
				ftbase.ftapplyplan(plan, buf, 0, 1)
				f = New complex(n - 1) {}
				For i = 0 To n2
					idx = 2 * (i Mod n2)
					hn.x = buf(idx + 0)
					hn.y = buf(idx + 1)
					idx = 2 * ((n2 - i) Mod n2)
					hmnc.x = buf(idx + 0)
					hmnc.y = -buf(idx + 1)
                    v.x = -System.Math.Sin(-(2 * System.Math.PI * i / n))
                    v.y = System.Math.Cos(-(2 * System.Math.PI * i / n))
                    f(i) = hn + hmnc - v * (hn - hmnc)
                    f(i).x = 0.5 * f(i).x
                    f(i).y = 0.5 * f(i).y
                Next
                For i = n2 + 1 To n - 1
                    f(i) = Math.conj(f(n - i))
                Next
            Else

                '
                ' use complex FFT
                '
                f = New complex(n - 1) {}
                For i = 0 To n - 1
                    f(i) = a(i)
                Next
                fftc1d(f, n)
            End If
        End Sub


        '************************************************************************
        '        1-dimensional real inverse FFT.
        '
        '        Algorithm has O(N*logN) complexity for any N (composite or prime).
        '
        '        INPUT PARAMETERS
        '            F   -   array[0..floor(N/2)] - frequencies from forward real FFT
        '            N   -   problem size
        '
        '        OUTPUT PARAMETERS
        '            A   -   inverse DFT of a input array, array[0..N-1]
        '
        '        NOTE:
        '            F[] should satisfy symmetry property F[k] = conj(F[N-k]), so just  one
        '        half of frequencies array is needed - elements from 0 to floor(N/2).  F[0]
        '        is ALWAYS real. If N is even F[floor(N/2)] is real too. If N is odd,  then
        '        F[floor(N/2)] has no special properties.
        '
        '        Relying on properties noted above, FFTR1DInv subroutine uses only elements
        '        from 0th to floor(N/2)-th. It ignores imaginary part of F[0],  and in case
        '        N is even it ignores imaginary part of F[floor(N/2)] too.
        '
        '        When you call this function using full arguments list - "FFTR1DInv(F,N,A)"
        '        - you can pass either either frequencies array with N elements or  reduced
        '        array with roughly N/2 elements - subroutine will  successfully  transform
        '        both.
        '
        '        If you call this function using reduced arguments list -  "FFTR1DInv(F,A)"
        '        - you must pass FULL array with N elements (although higher  N/2 are still
        '        not used) because array size is used to automatically determine FFT length
        '
        '
        '          -- ALGLIB --
        '             Copyright 01.06.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub fftr1dinv(f As complex(), n As Integer, ByRef a As Double())
            Dim i As Integer = 0
            Dim h As Double() = New Double(-1) {}
            Dim fh As complex() = New complex(-1) {}

            a = New Double(-1) {}

            alglib.ap.assert(n > 0, "FFTR1DInv: incorrect N!")
            alglib.ap.assert(alglib.ap.len(f) >= CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2)))) + 1, "FFTR1DInv: Length(F)<Floor(N/2)+1!")
            alglib.ap.assert(Math.isfinite(f(0).x), "FFTR1DInv: F contains infinite or NAN values!")
            For i = 1 To CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2)))) - 1
                alglib.ap.assert(Math.isfinite(f(i).x) AndAlso Math.isfinite(f(i).y), "FFTR1DInv: F contains infinite or NAN values!")
            Next
            alglib.ap.assert(Math.isfinite(f(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2))))).x), "FFTR1DInv: F contains infinite or NAN values!")
            If n Mod 2 <> 0 Then
                alglib.ap.assert(Math.isfinite(f(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2))))).y), "FFTR1DInv: F contains infinite or NAN values!")
            End If

            '
            ' Special case: N=1, FFT is just identity transform.
            ' After this block we assume that N is strictly greater than 1.
            '
            If n = 1 Then
                a = New Double(0) {}
                a(0) = f(0).x
                Return
            End If

            '
            ' inverse real FFT is reduced to the inverse real FHT,
            ' which is reduced to the forward real FHT,
            ' which is reduced to the forward real FFT.
            '
            ' Don't worry, it is really compact and efficient reduction :)
            '
            h = New Double(n - 1) {}
            a = New Double(n - 1) {}
            h(0) = f(0).x
            For i = 1 To CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2)))) - 1
                h(i) = f(i).x - f(i).y
                h(n - i) = f(i).x + f(i).y
            Next
            If n Mod 2 = 0 Then
                h(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2))))) = f(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2))))).x
            Else
                h(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2))))) = f(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2))))).x - f(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2))))).y
                h(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2)))) + 1) = f(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2))))).x + f(CInt(System.Math.Truncate(System.Math.Floor(CDbl(n) / CDbl(2))))).y
            End If
            fftr1d(h, n, fh)
            For i = 0 To n - 1
                a(i) = (fh(i).x - fh(i).y) / n
            Next
        End Sub


        '************************************************************************
        '        Internal subroutine. Never call it directly!
        '
        '
        '          -- ALGLIB --
        '             Copyright 01.06.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub fftr1dinternaleven(ByRef a As Double(), n As Integer, ByRef buf As Double(), plan As ftbase.fasttransformplan)
            Dim x As Double = 0
            Dim y As Double = 0
            Dim i As Integer = 0
            Dim n2 As Integer = 0
            Dim idx As Integer = 0
            Dim hn As complex = 0
            Dim hmnc As complex = 0
            Dim v As complex = 0
            Dim i_ As Integer = 0

            alglib.ap.assert(n > 0 AndAlso n Mod 2 = 0, "FFTR1DEvenInplace: incorrect N!")

            '
            ' Special cases:
            ' * N=2
            '
            ' After this block we assume that N is strictly greater than 2
            '
            If n = 2 Then
                x = a(0) + a(1)
                y = a(0) - a(1)
                a(0) = x
                a(1) = y
                Return
            End If

            '
            ' even-size real FFT, use reduction to the complex task
            '
            n2 = n \ 2
            For i_ = 0 To n - 1
                buf(i_) = a(i_)
            Next
            ftbase.ftapplyplan(plan, buf, 0, 1)
            a(0) = buf(0) + buf(1)
            For i = 1 To n2 - 1
                idx = 2 * (i Mod n2)
                hn.x = buf(idx + 0)
                hn.y = buf(idx + 1)
                idx = 2 * (n2 - i)
                hmnc.x = buf(idx + 0)
                hmnc.y = -buf(idx + 1)
                v.x = -System.Math.Sin(-(2 * System.Math.PI * i / n))
                v.y = System.Math.Cos(-(2 * System.Math.PI * i / n))
                v = hn + hmnc - v * (hn - hmnc)
                a(2 * i + 0) = 0.5 * v.x
                a(2 * i + 1) = 0.5 * v.y
            Next
            a(1) = buf(0) - buf(1)
        End Sub


        '************************************************************************
        '        Internal subroutine. Never call it directly!
        '
        '
        '          -- ALGLIB --
        '             Copyright 01.06.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub fftr1dinvinternaleven(ByRef a As Double(), n As Integer, ByRef buf As Double(), plan As ftbase.fasttransformplan)
            Dim x As Double = 0
            Dim y As Double = 0
            Dim t As Double = 0
            Dim i As Integer = 0
            Dim n2 As Integer = 0

            alglib.ap.assert(n > 0 AndAlso n Mod 2 = 0, "FFTR1DInvInternalEven: incorrect N!")

            '
            ' Special cases:
            ' * N=2
            '
            ' After this block we assume that N is strictly greater than 2
            '
            If n = 2 Then
                x = 0.5 * (a(0) + a(1))
                y = 0.5 * (a(0) - a(1))
                a(0) = x
                a(1) = y
                Return
            End If

            '
            ' inverse real FFT is reduced to the inverse real FHT,
            ' which is reduced to the forward real FHT,
            ' which is reduced to the forward real FFT.
            '
            ' Don't worry, it is really compact and efficient reduction :)
            '
            n2 = n \ 2
            buf(0) = a(0)
            For i = 1 To n2 - 1
                x = a(2 * i + 0)
                y = a(2 * i + 1)
                buf(i) = x - y
                buf(n - i) = x + y
            Next
            buf(n2) = a(1)
            fftr1dinternaleven(buf, n, a, plan)
            a(0) = buf(0) / n
            t = CDbl(1) / CDbl(n)
            For i = 1 To n2 - 1
                x = buf(2 * i + 0)
                y = buf(2 * i + 1)
                a(i) = t * (x - y)
                a(n - i) = t * (x + y)
            Next
            a(n2) = buf(1) / n
        End Sub


    End Class
    Public Class conv
        '************************************************************************
        '        1-dimensional complex convolution.
        '
        '        For given A/B returns conv(A,B) (non-circular). Subroutine can automatically
        '        choose between three implementations: straightforward O(M*N)  formula  for
        '        very small N (or M), overlap-add algorithm for  cases  where  max(M,N)  is
        '        significantly larger than min(M,N), but O(M*N) algorithm is too slow,  and
        '        general FFT-based formula for cases where two previois algorithms are  too
        '        slow.
        '
        '        Algorithm has max(M,N)*log(max(M,N)) complexity for any M/N.
        '
        '        INPUT PARAMETERS
        '            A   -   array[0..M-1] - complex function to be transformed
        '            M   -   problem size
        '            B   -   array[0..N-1] - complex function to be transformed
        '            N   -   problem size
        '
        '        OUTPUT PARAMETERS
        '            R   -   convolution: A*B. array[0..N+M-2].
        '
        '        NOTE:
        '            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        '        functions have non-zero values at negative T's, you  can  still  use  this
        '        subroutine - just shift its result correspondingly.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convc1d(a As complex(), m As Integer, b As complex(), n As Integer, ByRef r As complex())
            r = New complex(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvC1D: incorrect N or M!")

            '
            ' normalize task: make M>=N,
            ' so A will be longer that B.
            '
            If m < n Then
                convc1d(b, n, a, m, r)
                Return
            End If
            convc1dx(a, m, b, n, False, -1, _
                0, r)
        End Sub


        '************************************************************************
        '        1-dimensional complex non-circular deconvolution (inverse of ConvC1D()).
        '
        '        Algorithm has M*log(M)) complexity for any M (composite or prime).
        '
        '        INPUT PARAMETERS
        '            A   -   array[0..M-1] - convolved signal, A = conv(R, B)
        '            M   -   convolved signal length
        '            B   -   array[0..N-1] - response
        '            N   -   response length, N<=M
        '
        '        OUTPUT PARAMETERS
        '            R   -   deconvolved signal. array[0..M-N].
        '
        '        NOTE:
        '            deconvolution is unstable process and may result in division  by  zero
        '        (if your response function is degenerate, i.e. has zero Fourier coefficient).
        '
        '        NOTE:
        '            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        '        functions have non-zero values at negative T's, you  can  still  use  this
        '        subroutine - just shift its result correspondingly.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convc1dinv(a As complex(), m As Integer, b As complex(), n As Integer, ByRef r As complex())
            Dim i As Integer = 0
            Dim p As Integer = 0
            Dim buf As Double() = New Double(-1) {}
            Dim buf2 As Double() = New Double(-1) {}
            Dim plan As New ftbase.fasttransformplan()
            Dim c1 As complex = 0
            Dim c2 As complex = 0
            Dim c3 As complex = 0
            Dim t As Double = 0

            r = New complex(-1) {}

            alglib.ap.assert((n > 0 AndAlso m > 0) AndAlso n <= m, "ConvC1DInv: incorrect N or M!")
            p = ftbase.ftbasefindsmooth(m)
            ftbase.ftcomplexfftplan(p, 1, plan)
            buf = New Double(2 * p - 1) {}
            For i = 0 To m - 1
                buf(2 * i + 0) = a(i).x
                buf(2 * i + 1) = a(i).y
            Next
            For i = m To p - 1
                buf(2 * i + 0) = 0
                buf(2 * i + 1) = 0
            Next
            buf2 = New Double(2 * p - 1) {}
            For i = 0 To n - 1
                buf2(2 * i + 0) = b(i).x
                buf2(2 * i + 1) = b(i).y
            Next
            For i = n To p - 1
                buf2(2 * i + 0) = 0
                buf2(2 * i + 1) = 0
            Next
            ftbase.ftapplyplan(plan, buf, 0, 1)
            ftbase.ftapplyplan(plan, buf2, 0, 1)
            For i = 0 To p - 1
                c1.x = buf(2 * i + 0)
                c1.y = buf(2 * i + 1)
                c2.x = buf2(2 * i + 0)
                c2.y = buf2(2 * i + 1)
                c3 = c1 / c2
                buf(2 * i + 0) = c3.x
                buf(2 * i + 1) = -c3.y
            Next
            ftbase.ftapplyplan(plan, buf, 0, 1)
            t = CDbl(1) / CDbl(p)
            r = New complex(m - n) {}
            For i = 0 To m - n
                r(i).x = t * buf(2 * i + 0)
                r(i).y = -(t * buf(2 * i + 1))
            Next
        End Sub


        '************************************************************************
        '        1-dimensional circular complex convolution.
        '
        '        For given S/R returns conv(S,R) (circular). Algorithm has linearithmic
        '        complexity for any M/N.
        '
        '        IMPORTANT:  normal convolution is commutative,  i.e.   it  is symmetric  -
        '        conv(A,B)=conv(B,A).  Cyclic convolution IS NOT.  One function - S - is  a
        '        signal,  periodic function, and another - R - is a response,  non-periodic
        '        function with limited length.
        '
        '        INPUT PARAMETERS
        '            S   -   array[0..M-1] - complex periodic signal
        '            M   -   problem size
        '            B   -   array[0..N-1] - complex non-periodic response
        '            N   -   problem size
        '
        '        OUTPUT PARAMETERS
        '            R   -   convolution: A*B. array[0..M-1].
        '
        '        NOTE:
        '            It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
        '        negative T's, you can still use this subroutine - just  shift  its  result
        '        correspondingly.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convc1dcircular(s As complex(), m As Integer, r As complex(), n As Integer, ByRef c As complex())
            Dim buf As complex() = New complex(-1) {}
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim j2 As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            c = New complex(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvC1DCircular: incorrect N or M!")

            '
            ' normalize task: make M>=N,
            ' so A will be longer (at least - not shorter) that B.
            '
            If m < n Then
                buf = New complex(m - 1) {}
                For i1 = 0 To m - 1
                    buf(i1) = 0
                Next
                i1 = 0
                While i1 < n
                    i2 = System.Math.Min(i1 + m - 1, n - 1)
                    j2 = i2 - i1
                    i1_ = (i1) - (0)
                    For i_ = 0 To j2
                        buf(i_) = buf(i_) + r(i_ + i1_)
                    Next
                    i1 = i1 + m
                End While
                convc1dcircular(s, m, buf, m, c)
                Return
            End If
            convc1dx(s, m, r, n, True, -1, _
                0, c)
        End Sub


        '************************************************************************
        '        1-dimensional circular complex deconvolution (inverse of ConvC1DCircular()).
        '
        '        Algorithm has M*log(M)) complexity for any M (composite or prime).
        '
        '        INPUT PARAMETERS
        '            A   -   array[0..M-1] - convolved periodic signal, A = conv(R, B)
        '            M   -   convolved signal length
        '            B   -   array[0..N-1] - non-periodic response
        '            N   -   response length
        '
        '        OUTPUT PARAMETERS
        '            R   -   deconvolved signal. array[0..M-1].
        '
        '        NOTE:
        '            deconvolution is unstable process and may result in division  by  zero
        '        (if your response function is degenerate, i.e. has zero Fourier coefficient).
        '
        '        NOTE:
        '            It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
        '        negative T's, you can still use this subroutine - just  shift  its  result
        '        correspondingly.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convc1dcircularinv(a As complex(), m As Integer, b As complex(), n As Integer, ByRef r As complex())
            Dim i As Integer = 0
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim j2 As Integer = 0
            Dim buf As Double() = New Double(-1) {}
            Dim buf2 As Double() = New Double(-1) {}
            Dim cbuf As complex() = New complex(-1) {}
            Dim plan As New ftbase.fasttransformplan()
            Dim c1 As complex = 0
            Dim c2 As complex = 0
            Dim c3 As complex = 0
            Dim t As Double = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            r = New complex(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvC1DCircularInv: incorrect N or M!")

            '
            ' normalize task: make M>=N,
            ' so A will be longer (at least - not shorter) that B.
            '
            If m < n Then
                cbuf = New complex(m - 1) {}
                For i = 0 To m - 1
                    cbuf(i) = 0
                Next
                i1 = 0
                While i1 < n
                    i2 = System.Math.Min(i1 + m - 1, n - 1)
                    j2 = i2 - i1
                    i1_ = (i1) - (0)
                    For i_ = 0 To j2
                        cbuf(i_) = cbuf(i_) + b(i_ + i1_)
                    Next
                    i1 = i1 + m
                End While
                convc1dcircularinv(a, m, cbuf, m, r)
                Return
            End If

            '
            ' Task is normalized
            '
            ftbase.ftcomplexfftplan(m, 1, plan)
            buf = New Double(2 * m - 1) {}
            For i = 0 To m - 1
                buf(2 * i + 0) = a(i).x
                buf(2 * i + 1) = a(i).y
            Next
            buf2 = New Double(2 * m - 1) {}
            For i = 0 To n - 1
                buf2(2 * i + 0) = b(i).x
                buf2(2 * i + 1) = b(i).y
            Next
            For i = n To m - 1
                buf2(2 * i + 0) = 0
                buf2(2 * i + 1) = 0
            Next
            ftbase.ftapplyplan(plan, buf, 0, 1)
            ftbase.ftapplyplan(plan, buf2, 0, 1)
            For i = 0 To m - 1
                c1.x = buf(2 * i + 0)
                c1.y = buf(2 * i + 1)
                c2.x = buf2(2 * i + 0)
                c2.y = buf2(2 * i + 1)
                c3 = c1 / c2
                buf(2 * i + 0) = c3.x
                buf(2 * i + 1) = -c3.y
            Next
            ftbase.ftapplyplan(plan, buf, 0, 1)
            t = CDbl(1) / CDbl(m)
            r = New complex(m - 1) {}
            For i = 0 To m - 1
                r(i).x = t * buf(2 * i + 0)
                r(i).y = -(t * buf(2 * i + 1))
            Next
        End Sub


        '************************************************************************
        '        1-dimensional real convolution.
        '
        '        Analogous to ConvC1D(), see ConvC1D() comments for more details.
        '
        '        INPUT PARAMETERS
        '            A   -   array[0..M-1] - real function to be transformed
        '            M   -   problem size
        '            B   -   array[0..N-1] - real function to be transformed
        '            N   -   problem size
        '
        '        OUTPUT PARAMETERS
        '            R   -   convolution: A*B. array[0..N+M-2].
        '
        '        NOTE:
        '            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        '        functions have non-zero values at negative T's, you  can  still  use  this
        '        subroutine - just shift its result correspondingly.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convr1d(a As Double(), m As Integer, b As Double(), n As Integer, ByRef r As Double())
            r = New Double(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvR1D: incorrect N or M!")

            '
            ' normalize task: make M>=N,
            ' so A will be longer that B.
            '
            If m < n Then
                convr1d(b, n, a, m, r)
                Return
            End If
            convr1dx(a, m, b, n, False, -1, _
                0, r)
        End Sub


        '************************************************************************
        '        1-dimensional real deconvolution (inverse of ConvC1D()).
        '
        '        Algorithm has M*log(M)) complexity for any M (composite or prime).
        '
        '        INPUT PARAMETERS
        '            A   -   array[0..M-1] - convolved signal, A = conv(R, B)
        '            M   -   convolved signal length
        '            B   -   array[0..N-1] - response
        '            N   -   response length, N<=M
        '
        '        OUTPUT PARAMETERS
        '            R   -   deconvolved signal. array[0..M-N].
        '
        '        NOTE:
        '            deconvolution is unstable process and may result in division  by  zero
        '        (if your response function is degenerate, i.e. has zero Fourier coefficient).
        '
        '        NOTE:
        '            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        '        functions have non-zero values at negative T's, you  can  still  use  this
        '        subroutine - just shift its result correspondingly.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convr1dinv(a As Double(), m As Integer, b As Double(), n As Integer, ByRef r As Double())
            Dim i As Integer = 0
            Dim p As Integer = 0
            Dim buf As Double() = New Double(-1) {}
            Dim buf2 As Double() = New Double(-1) {}
            Dim buf3 As Double() = New Double(-1) {}
            Dim plan As New ftbase.fasttransformplan()
            Dim c1 As complex = 0
            Dim c2 As complex = 0
            Dim c3 As complex = 0
            Dim i_ As Integer = 0

            r = New Double(-1) {}

            alglib.ap.assert((n > 0 AndAlso m > 0) AndAlso n <= m, "ConvR1DInv: incorrect N or M!")
            p = ftbase.ftbasefindsmootheven(m)
            buf = New Double(p - 1) {}
            For i_ = 0 To m - 1
                buf(i_) = a(i_)
            Next
            For i = m To p - 1
                buf(i) = 0
            Next
            buf2 = New Double(p - 1) {}
            For i_ = 0 To n - 1
                buf2(i_) = b(i_)
            Next
            For i = n To p - 1
                buf2(i) = 0
            Next
            buf3 = New Double(p - 1) {}
            ftbase.ftcomplexfftplan(p \ 2, 1, plan)
            fft.fftr1dinternaleven(buf, p, buf3, plan)
            fft.fftr1dinternaleven(buf2, p, buf3, plan)
            buf(0) = buf(0) / buf2(0)
            buf(1) = buf(1) / buf2(1)
            For i = 1 To p \ 2 - 1
                c1.x = buf(2 * i + 0)
                c1.y = buf(2 * i + 1)
                c2.x = buf2(2 * i + 0)
                c2.y = buf2(2 * i + 1)
                c3 = c1 / c2
                buf(2 * i + 0) = c3.x
                buf(2 * i + 1) = c3.y
            Next
            fft.fftr1dinvinternaleven(buf, p, buf3, plan)
            r = New Double(m - n) {}
            For i_ = 0 To m - n
                r(i_) = buf(i_)
            Next
        End Sub


        '************************************************************************
        '        1-dimensional circular real convolution.
        '
        '        Analogous to ConvC1DCircular(), see ConvC1DCircular() comments for more details.
        '
        '        INPUT PARAMETERS
        '            S   -   array[0..M-1] - real signal
        '            M   -   problem size
        '            B   -   array[0..N-1] - real response
        '            N   -   problem size
        '
        '        OUTPUT PARAMETERS
        '            R   -   convolution: A*B. array[0..M-1].
        '
        '        NOTE:
        '            It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
        '        negative T's, you can still use this subroutine - just  shift  its  result
        '        correspondingly.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convr1dcircular(s As Double(), m As Integer, r As Double(), n As Integer, ByRef c As Double())
            Dim buf As Double() = New Double(-1) {}
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim j2 As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            c = New Double(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvC1DCircular: incorrect N or M!")

            '
            ' normalize task: make M>=N,
            ' so A will be longer (at least - not shorter) that B.
            '
            If m < n Then
                buf = New Double(m - 1) {}
                For i1 = 0 To m - 1
                    buf(i1) = 0
                Next
                i1 = 0
                While i1 < n
                    i2 = System.Math.Min(i1 + m - 1, n - 1)
                    j2 = i2 - i1
                    i1_ = (i1) - (0)
                    For i_ = 0 To j2
                        buf(i_) = buf(i_) + r(i_ + i1_)
                    Next
                    i1 = i1 + m
                End While
                convr1dcircular(s, m, buf, m, c)
                Return
            End If

            '
            ' reduce to usual convolution
            '
            convr1dx(s, m, r, n, True, -1, _
                0, c)
        End Sub


        '************************************************************************
        '        1-dimensional complex deconvolution (inverse of ConvC1D()).
        '
        '        Algorithm has M*log(M)) complexity for any M (composite or prime).
        '
        '        INPUT PARAMETERS
        '            A   -   array[0..M-1] - convolved signal, A = conv(R, B)
        '            M   -   convolved signal length
        '            B   -   array[0..N-1] - response
        '            N   -   response length
        '
        '        OUTPUT PARAMETERS
        '            R   -   deconvolved signal. array[0..M-N].
        '
        '        NOTE:
        '            deconvolution is unstable process and may result in division  by  zero
        '        (if your response function is degenerate, i.e. has zero Fourier coefficient).
        '
        '        NOTE:
        '            It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
        '        negative T's, you can still use this subroutine - just  shift  its  result
        '        correspondingly.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convr1dcircularinv(a As Double(), m As Integer, b As Double(), n As Integer, ByRef r As Double())
            Dim i As Integer = 0
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim j2 As Integer = 0
            Dim buf As Double() = New Double(-1) {}
            Dim buf2 As Double() = New Double(-1) {}
            Dim buf3 As Double() = New Double(-1) {}
            Dim cbuf As complex() = New complex(-1) {}
            Dim cbuf2 As complex() = New complex(-1) {}
            Dim plan As New ftbase.fasttransformplan()
            Dim c1 As complex = 0
            Dim c2 As complex = 0
            Dim c3 As complex = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            r = New Double(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvR1DCircularInv: incorrect N or M!")

            '
            ' normalize task: make M>=N,
            ' so A will be longer (at least - not shorter) that B.
            '
            If m < n Then
                buf = New Double(m - 1) {}
                For i = 0 To m - 1
                    buf(i) = 0
                Next
                i1 = 0
                While i1 < n
                    i2 = System.Math.Min(i1 + m - 1, n - 1)
                    j2 = i2 - i1
                    i1_ = (i1) - (0)
                    For i_ = 0 To j2
                        buf(i_) = buf(i_) + b(i_ + i1_)
                    Next
                    i1 = i1 + m
                End While
                convr1dcircularinv(a, m, buf, m, r)
                Return
            End If

            '
            ' Task is normalized
            '
            If m Mod 2 = 0 Then

                '
                ' size is even, use fast even-size FFT
                '
                buf = New Double(m - 1) {}
                For i_ = 0 To m - 1
                    buf(i_) = a(i_)
                Next
                buf2 = New Double(m - 1) {}
                For i_ = 0 To n - 1
                    buf2(i_) = b(i_)
                Next
                For i = n To m - 1
                    buf2(i) = 0
                Next
                buf3 = New Double(m - 1) {}
                ftbase.ftcomplexfftplan(m \ 2, 1, plan)
                fft.fftr1dinternaleven(buf, m, buf3, plan)
                fft.fftr1dinternaleven(buf2, m, buf3, plan)
                buf(0) = buf(0) / buf2(0)
                buf(1) = buf(1) / buf2(1)
                For i = 1 To m \ 2 - 1
                    c1.x = buf(2 * i + 0)
                    c1.y = buf(2 * i + 1)
                    c2.x = buf2(2 * i + 0)
                    c2.y = buf2(2 * i + 1)
                    c3 = c1 / c2
                    buf(2 * i + 0) = c3.x
                    buf(2 * i + 1) = c3.y
                Next
                fft.fftr1dinvinternaleven(buf, m, buf3, plan)
                r = New Double(m - 1) {}
                For i_ = 0 To m - 1
                    r(i_) = buf(i_)
                Next
            Else

                '
                ' odd-size, use general real FFT
                '
                fft.fftr1d(a, m, cbuf)
                buf2 = New Double(m - 1) {}
                For i_ = 0 To n - 1
                    buf2(i_) = b(i_)
                Next
                For i = n To m - 1
                    buf2(i) = 0
                Next
                fft.fftr1d(buf2, m, cbuf2)
                For i = 0 To CInt(System.Math.Truncate(System.Math.Floor(CDbl(m) / CDbl(2))))
                    cbuf(i) = cbuf(i) / cbuf2(i)
                Next
                fft.fftr1dinv(cbuf, m, r)
            End If
        End Sub


        '************************************************************************
        '        1-dimensional complex convolution.
        '
        '        Extended subroutine which allows to choose convolution algorithm.
        '        Intended for internal use, ALGLIB users should call ConvC1D()/ConvC1DCircular().
        '
        '        INPUT PARAMETERS
        '            A   -   array[0..M-1] - complex function to be transformed
        '            M   -   problem size
        '            B   -   array[0..N-1] - complex function to be transformed
        '            N   -   problem size, N<=M
        '            Alg -   algorithm type:
        '                    *-2     auto-select Q for overlap-add
        '                    *-1     auto-select algorithm and parameters
        '                    * 0     straightforward formula for small N's
        '                    * 1     general FFT-based code
        '                    * 2     overlap-add with length Q
        '            Q   -   length for overlap-add
        '
        '        OUTPUT PARAMETERS
        '            R   -   convolution: A*B. array[0..N+M-1].
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convc1dx(a As complex(), m As Integer, b As complex(), n As Integer, circular As Boolean, alg As Integer, _
            q As Integer, ByRef r As complex())
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim p As Integer = 0
            Dim ptotal As Integer = 0
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim j1 As Integer = 0
            Dim j2 As Integer = 0
            Dim bbuf As complex() = New complex(-1) {}
            Dim v As complex = 0
            Dim ax As Double = 0
            Dim ay As Double = 0
            Dim bx As Double = 0
            Dim by As Double = 0
            Dim t As Double = 0
            Dim tx As Double = 0
            Dim ty As Double = 0
            Dim flopcand As Double = 0
            Dim flopbest As Double = 0
            Dim algbest As Integer = 0
            Dim plan As New ftbase.fasttransformplan()
            Dim buf As Double() = New Double(-1) {}
            Dim buf2 As Double() = New Double(-1) {}
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            r = New complex(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvC1DX: incorrect N or M!")
            alglib.ap.assert(n <= m, "ConvC1DX: N<M assumption is false!")

            '
            ' Auto-select
            '
            If alg = -1 OrElse alg = -2 Then

                '
                ' Initial candidate: straightforward implementation.
                '
                ' If we want to use auto-fitted overlap-add,
                ' flop count is initialized by large real number - to force
                ' another algorithm selection
                '
                algbest = 0
                If alg = -1 Then
                    flopbest = 2 * m * n
                Else
                    flopbest = Math.maxrealnumber
                End If

                '
                ' Another candidate - generic FFT code
                '
                If alg = -1 Then
                    If circular AndAlso ftbase.ftbaseissmooth(m) Then

                        '
                        ' special code for circular convolution of a sequence with a smooth length
                        '
                        flopcand = 3 * ftbase.ftbasegetflopestimate(m) + 6 * m
                        If CDbl(flopcand) < CDbl(flopbest) Then
                            algbest = 1
                            flopbest = flopcand
                        End If
                    Else

                        '
                        ' general cyclic/non-cyclic convolution
                        '
                        p = ftbase.ftbasefindsmooth(m + n - 1)
                        flopcand = 3 * ftbase.ftbasegetflopestimate(p) + 6 * p
                        If CDbl(flopcand) < CDbl(flopbest) Then
                            algbest = 1
                            flopbest = flopcand
                        End If
                    End If
                End If

                '
                ' Another candidate - overlap-add
                '
                q = 1
                ptotal = 1
                While ptotal < n
                    ptotal = ptotal * 2
                End While
                While ptotal <= m + n - 1
                    p = ptotal - n + 1
                    flopcand = CInt(System.Math.Truncate(System.Math.Ceiling(CDbl(m) / CDbl(p)))) * (2 * ftbase.ftbasegetflopestimate(ptotal) + 8 * ptotal)
                    If CDbl(flopcand) < CDbl(flopbest) Then
                        flopbest = flopcand
                        algbest = 2
                        q = p
                    End If
                    ptotal = ptotal * 2
                End While
                alg = algbest
                convc1dx(a, m, b, n, circular, alg, _
                    q, r)
                Return
            End If

            '
            ' straightforward formula for
            ' circular and non-circular convolutions.
            '
            ' Very simple code, no further comments needed.
            '
            If alg = 0 Then

                '
                ' Special case: N=1
                '
                If n = 1 Then
                    r = New complex(m - 1) {}
                    v = b(0)
                    For i_ = 0 To m - 1
                        r(i_) = v * a(i_)
                    Next
                    Return
                End If

                '
                ' use straightforward formula
                '
                If circular Then

                    '
                    ' circular convolution
                    '
                    r = New complex(m - 1) {}
                    v = b(0)
                    For i_ = 0 To m - 1
                        r(i_) = v * a(i_)
                    Next
                    For i = 1 To n - 1
                        v = b(i)
                        i1 = 0
                        i2 = i - 1
                        j1 = m - i
                        j2 = m - 1
                        i1_ = (j1) - (i1)
                        For i_ = i1 To i2
                            r(i_) = r(i_) + v * a(i_ + i1_)
                        Next
                        i1 = i
                        i2 = m - 1
                        j1 = 0
                        j2 = m - i - 1
                        i1_ = (j1) - (i1)
                        For i_ = i1 To i2
                            r(i_) = r(i_) + v * a(i_ + i1_)
                        Next
                    Next
                Else

                    '
                    ' non-circular convolution
                    '
                    r = New complex(m + n - 2) {}
                    For i = 0 To m + n - 2
                        r(i) = 0
                    Next
                    For i = 0 To n - 1
                        v = b(i)
                        i1_ = (0) - (i)
                        For i_ = i To i + m - 1
                            r(i_) = r(i_) + v * a(i_ + i1_)
                        Next
                    Next
                End If
                Return
            End If

            '
            ' general FFT-based code for
            ' circular and non-circular convolutions.
            '
            ' First, if convolution is circular, we test whether M is smooth or not.
            ' If it is smooth, we just use M-length FFT to calculate convolution.
            ' If it is not, we calculate non-circular convolution and wrap it arount.
            '
            ' IF convolution is non-circular, we use zero-padding + FFT.
            '
            If alg = 1 Then
                If circular AndAlso ftbase.ftbaseissmooth(m) Then

                    '
                    ' special code for circular convolution with smooth M
                    '
                    ftbase.ftcomplexfftplan(m, 1, plan)
                    buf = New Double(2 * m - 1) {}
                    For i = 0 To m - 1
                        buf(2 * i + 0) = a(i).x
                        buf(2 * i + 1) = a(i).y
                    Next
                    buf2 = New Double(2 * m - 1) {}
                    For i = 0 To n - 1
                        buf2(2 * i + 0) = b(i).x
                        buf2(2 * i + 1) = b(i).y
                    Next
                    For i = n To m - 1
                        buf2(2 * i + 0) = 0
                        buf2(2 * i + 1) = 0
                    Next
                    ftbase.ftapplyplan(plan, buf, 0, 1)
                    ftbase.ftapplyplan(plan, buf2, 0, 1)
                    For i = 0 To m - 1
                        ax = buf(2 * i + 0)
                        ay = buf(2 * i + 1)
                        bx = buf2(2 * i + 0)
                        by = buf2(2 * i + 1)
                        tx = ax * bx - ay * by
                        ty = ax * by + ay * bx
                        buf(2 * i + 0) = tx
                        buf(2 * i + 1) = -ty
                    Next
                    ftbase.ftapplyplan(plan, buf, 0, 1)
                    t = CDbl(1) / CDbl(m)
                    r = New complex(m - 1) {}
                    For i = 0 To m - 1
                        r(i).x = t * buf(2 * i + 0)
                        r(i).y = -(t * buf(2 * i + 1))
                    Next
                Else

                    '
                    ' M is non-smooth, general code (circular/non-circular):
                    ' * first part is the same for circular and non-circular
                    '   convolutions. zero padding, FFTs, inverse FFTs
                    ' * second part differs:
                    '   * for non-circular convolution we just copy array
                    '   * for circular convolution we add array tail to its head
                    '
                    p = ftbase.ftbasefindsmooth(m + n - 1)
                    ftbase.ftcomplexfftplan(p, 1, plan)
                    buf = New Double(2 * p - 1) {}
                    For i = 0 To m - 1
                        buf(2 * i + 0) = a(i).x
                        buf(2 * i + 1) = a(i).y
                    Next
                    For i = m To p - 1
                        buf(2 * i + 0) = 0
                        buf(2 * i + 1) = 0
                    Next
                    buf2 = New Double(2 * p - 1) {}
                    For i = 0 To n - 1
                        buf2(2 * i + 0) = b(i).x
                        buf2(2 * i + 1) = b(i).y
                    Next
                    For i = n To p - 1
                        buf2(2 * i + 0) = 0
                        buf2(2 * i + 1) = 0
                    Next
                    ftbase.ftapplyplan(plan, buf, 0, 1)
                    ftbase.ftapplyplan(plan, buf2, 0, 1)
                    For i = 0 To p - 1
                        ax = buf(2 * i + 0)
                        ay = buf(2 * i + 1)
                        bx = buf2(2 * i + 0)
                        by = buf2(2 * i + 1)
                        tx = ax * bx - ay * by
                        ty = ax * by + ay * bx
                        buf(2 * i + 0) = tx
                        buf(2 * i + 1) = -ty
                    Next
                    ftbase.ftapplyplan(plan, buf, 0, 1)
                    t = CDbl(1) / CDbl(p)
                    If circular Then

                        '
                        ' circular, add tail to head
                        '
                        r = New complex(m - 1) {}
                        For i = 0 To m - 1
                            r(i).x = t * buf(2 * i + 0)
                            r(i).y = -(t * buf(2 * i + 1))
                        Next
                        For i = m To m + n - 2
                            r(i - m).x = r(i - m).x + t * buf(2 * i + 0)
                            r(i - m).y = r(i - m).y - t * buf(2 * i + 1)
                        Next
                    Else

                        '
                        ' non-circular, just copy
                        '
                        r = New complex(m + n - 2) {}
                        For i = 0 To m + n - 2
                            r(i).x = t * buf(2 * i + 0)
                            r(i).y = -(t * buf(2 * i + 1))
                        Next
                    End If
                End If
                Return
            End If

            '
            ' overlap-add method for
            ' circular and non-circular convolutions.
            '
            ' First part of code (separate FFTs of input blocks) is the same
            ' for all types of convolution. Second part (overlapping outputs)
            ' differs for different types of convolution. We just copy output
            ' when convolution is non-circular. We wrap it around, if it is
            ' circular.
            '
            If alg = 2 Then
                buf = New Double(2 * (q + n - 1) - 1) {}

                '
                ' prepare R
                '
                If circular Then
                    r = New complex(m - 1) {}
                    For i = 0 To m - 1
                        r(i) = 0
                    Next
                Else
                    r = New complex(m + n - 2) {}
                    For i = 0 To m + n - 2
                        r(i) = 0
                    Next
                End If

                '
                ' pre-calculated FFT(B)
                '
                bbuf = New complex(q + n - 2) {}
                For i_ = 0 To n - 1
                    bbuf(i_) = b(i_)
                Next
                For j = n To q + n - 2
                    bbuf(j) = 0
                Next
                fft.fftc1d(bbuf, q + n - 1)

                '
                ' prepare FFT plan for chunks of A
                '
                ftbase.ftcomplexfftplan(q + n - 1, 1, plan)

                '
                ' main overlap-add cycle
                '
                i = 0
                While i <= m - 1
                    p = System.Math.Min(q, m - i)
                    For j = 0 To p - 1
                        buf(2 * j + 0) = a(i + j).x
                        buf(2 * j + 1) = a(i + j).y
                    Next
                    For j = p To q + n - 2
                        buf(2 * j + 0) = 0
                        buf(2 * j + 1) = 0
                    Next
                    ftbase.ftapplyplan(plan, buf, 0, 1)
                    For j = 0 To q + n - 2
                        ax = buf(2 * j + 0)
                        ay = buf(2 * j + 1)
                        bx = bbuf(j).x
                        by = bbuf(j).y
                        tx = ax * bx - ay * by
                        ty = ax * by + ay * bx
                        buf(2 * j + 0) = tx
                        buf(2 * j + 1) = -ty
                    Next
                    ftbase.ftapplyplan(plan, buf, 0, 1)
                    t = CDbl(1) / CDbl(q + n - 1)
                    If circular Then
                        j1 = System.Math.Min(i + p + n - 2, m - 1) - i
                        j2 = j1 + 1
                    Else
                        j1 = p + n - 2
                        j2 = j1 + 1
                    End If
                    For j = 0 To j1
                        r(i + j).x = r(i + j).x + buf(2 * j + 0) * t
                        r(i + j).y = r(i + j).y - buf(2 * j + 1) * t
                    Next
                    For j = j2 To p + n - 2
                        r(j - j2).x = r(j - j2).x + buf(2 * j + 0) * t
                        r(j - j2).y = r(j - j2).y - buf(2 * j + 1) * t
                    Next
                    i = i + p
                End While
                Return
            End If
        End Sub


        '************************************************************************
        '        1-dimensional real convolution.
        '
        '        Extended subroutine which allows to choose convolution algorithm.
        '        Intended for internal use, ALGLIB users should call ConvR1D().
        '
        '        INPUT PARAMETERS
        '            A   -   array[0..M-1] - complex function to be transformed
        '            M   -   problem size
        '            B   -   array[0..N-1] - complex function to be transformed
        '            N   -   problem size, N<=M
        '            Alg -   algorithm type:
        '                    *-2     auto-select Q for overlap-add
        '                    *-1     auto-select algorithm and parameters
        '                    * 0     straightforward formula for small N's
        '                    * 1     general FFT-based code
        '                    * 2     overlap-add with length Q
        '            Q   -   length for overlap-add
        '
        '        OUTPUT PARAMETERS
        '            R   -   convolution: A*B. array[0..N+M-1].
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub convr1dx(a As Double(), m As Integer, b As Double(), n As Integer, circular As Boolean, alg As Integer, _
            q As Integer, ByRef r As Double())
            Dim v As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim p As Integer = 0
            Dim ptotal As Integer = 0
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim j1 As Integer = 0
            Dim j2 As Integer = 0
            Dim ax As Double = 0
            Dim ay As Double = 0
            Dim bx As Double = 0
            Dim by As Double = 0
            Dim tx As Double = 0
            Dim ty As Double = 0
            Dim flopcand As Double = 0
            Dim flopbest As Double = 0
            Dim algbest As Integer = 0
            Dim plan As New ftbase.fasttransformplan()
            Dim buf As Double() = New Double(-1) {}
            Dim buf2 As Double() = New Double(-1) {}
            Dim buf3 As Double() = New Double(-1) {}
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            r = New Double(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvC1DX: incorrect N or M!")
            alglib.ap.assert(n <= m, "ConvC1DX: N<M assumption is false!")

            '
            ' handle special cases
            '
            If System.Math.Min(m, n) <= 2 Then
                alg = 0
            End If

            '
            ' Auto-select
            '
            If alg < 0 Then

                '
                ' Initial candidate: straightforward implementation.
                '
                ' If we want to use auto-fitted overlap-add,
                ' flop count is initialized by large real number - to force
                ' another algorithm selection
                '
                algbest = 0
                If alg = -1 Then
                    flopbest = 0.15 * m * n
                Else
                    flopbest = Math.maxrealnumber
                End If

                '
                ' Another candidate - generic FFT code
                '
                If alg = -1 Then
                    If (circular AndAlso ftbase.ftbaseissmooth(m)) AndAlso m Mod 2 = 0 Then

                        '
                        ' special code for circular convolution of a sequence with a smooth length
                        '
                        flopcand = 3 * ftbase.ftbasegetflopestimate(m \ 2) + CDbl(6 * m) / CDbl(2)
                        If CDbl(flopcand) < CDbl(flopbest) Then
                            algbest = 1
                            flopbest = flopcand
                        End If
                    Else

                        '
                        ' general cyclic/non-cyclic convolution
                        '
                        p = ftbase.ftbasefindsmootheven(m + n - 1)
                        flopcand = 3 * ftbase.ftbasegetflopestimate(p \ 2) + CDbl(6 * p) / CDbl(2)
                        If CDbl(flopcand) < CDbl(flopbest) Then
                            algbest = 1
                            flopbest = flopcand
                        End If
                    End If
                End If

                '
                ' Another candidate - overlap-add
                '
                q = 1
                ptotal = 1
                While ptotal < n
                    ptotal = ptotal * 2
                End While
                While ptotal <= m + n - 1
                    p = ptotal - n + 1
                    flopcand = CInt(System.Math.Truncate(System.Math.Ceiling(CDbl(m) / CDbl(p)))) * (2 * ftbase.ftbasegetflopestimate(ptotal \ 2) + 1 * (ptotal \ 2))
                    If CDbl(flopcand) < CDbl(flopbest) Then
                        flopbest = flopcand
                        algbest = 2
                        q = p
                    End If
                    ptotal = ptotal * 2
                End While
                alg = algbest
                convr1dx(a, m, b, n, circular, alg, _
                    q, r)
                Return
            End If

            '
            ' straightforward formula for
            ' circular and non-circular convolutions.
            '
            ' Very simple code, no further comments needed.
            '
            If alg = 0 Then

                '
                ' Special case: N=1
                '
                If n = 1 Then
                    r = New Double(m - 1) {}
                    v = b(0)
                    For i_ = 0 To m - 1
                        r(i_) = v * a(i_)
                    Next
                    Return
                End If

                '
                ' use straightforward formula
                '
                If circular Then

                    '
                    ' circular convolution
                    '
                    r = New Double(m - 1) {}
                    v = b(0)
                    For i_ = 0 To m - 1
                        r(i_) = v * a(i_)
                    Next
                    For i = 1 To n - 1
                        v = b(i)
                        i1 = 0
                        i2 = i - 1
                        j1 = m - i
                        j2 = m - 1
                        i1_ = (j1) - (i1)
                        For i_ = i1 To i2
                            r(i_) = r(i_) + v * a(i_ + i1_)
                        Next
                        i1 = i
                        i2 = m - 1
                        j1 = 0
                        j2 = m - i - 1
                        i1_ = (j1) - (i1)
                        For i_ = i1 To i2
                            r(i_) = r(i_) + v * a(i_ + i1_)
                        Next
                    Next
                Else

                    '
                    ' non-circular convolution
                    '
                    r = New Double(m + n - 2) {}
                    For i = 0 To m + n - 2
                        r(i) = 0
                    Next
                    For i = 0 To n - 1
                        v = b(i)
                        i1_ = (0) - (i)
                        For i_ = i To i + m - 1
                            r(i_) = r(i_) + v * a(i_ + i1_)
                        Next
                    Next
                End If
                Return
            End If

            '
            ' general FFT-based code for
            ' circular and non-circular convolutions.
            '
            ' First, if convolution is circular, we test whether M is smooth or not.
            ' If it is smooth, we just use M-length FFT to calculate convolution.
            ' If it is not, we calculate non-circular convolution and wrap it arount.
            '
            ' If convolution is non-circular, we use zero-padding + FFT.
            '
            ' We assume that M+N-1>2 - we should call small case code otherwise
            '
            If alg = 1 Then
                alglib.ap.assert(m + n - 1 > 2, "ConvR1DX: internal error!")
                If (circular AndAlso ftbase.ftbaseissmooth(m)) AndAlso m Mod 2 = 0 Then

                    '
                    ' special code for circular convolution with smooth even M
                    '
                    buf = New Double(m - 1) {}
                    For i_ = 0 To m - 1
                        buf(i_) = a(i_)
                    Next
                    buf2 = New Double(m - 1) {}
                    For i_ = 0 To n - 1
                        buf2(i_) = b(i_)
                    Next
                    For i = n To m - 1
                        buf2(i) = 0
                    Next
                    buf3 = New Double(m - 1) {}
                    ftbase.ftcomplexfftplan(m \ 2, 1, plan)
                    fft.fftr1dinternaleven(buf, m, buf3, plan)
                    fft.fftr1dinternaleven(buf2, m, buf3, plan)
                    buf(0) = buf(0) * buf2(0)
                    buf(1) = buf(1) * buf2(1)
                    For i = 1 To m \ 2 - 1
                        ax = buf(2 * i + 0)
                        ay = buf(2 * i + 1)
                        bx = buf2(2 * i + 0)
                        by = buf2(2 * i + 1)
                        tx = ax * bx - ay * by
                        ty = ax * by + ay * bx
                        buf(2 * i + 0) = tx
                        buf(2 * i + 1) = ty
                    Next
                    fft.fftr1dinvinternaleven(buf, m, buf3, plan)
                    r = New Double(m - 1) {}
                    For i_ = 0 To m - 1
                        r(i_) = buf(i_)
                    Next
                Else

                    '
                    ' M is non-smooth or non-even, general code (circular/non-circular):
                    ' * first part is the same for circular and non-circular
                    '   convolutions. zero padding, FFTs, inverse FFTs
                    ' * second part differs:
                    '   * for non-circular convolution we just copy array
                    '   * for circular convolution we add array tail to its head
                    '
                    p = ftbase.ftbasefindsmootheven(m + n - 1)
                    buf = New Double(p - 1) {}
                    For i_ = 0 To m - 1
                        buf(i_) = a(i_)
                    Next
                    For i = m To p - 1
                        buf(i) = 0
                    Next
                    buf2 = New Double(p - 1) {}
                    For i_ = 0 To n - 1
                        buf2(i_) = b(i_)
                    Next
                    For i = n To p - 1
                        buf2(i) = 0
                    Next
                    buf3 = New Double(p - 1) {}
                    ftbase.ftcomplexfftplan(p \ 2, 1, plan)
                    fft.fftr1dinternaleven(buf, p, buf3, plan)
                    fft.fftr1dinternaleven(buf2, p, buf3, plan)
                    buf(0) = buf(0) * buf2(0)
                    buf(1) = buf(1) * buf2(1)
                    For i = 1 To p \ 2 - 1
                        ax = buf(2 * i + 0)
                        ay = buf(2 * i + 1)
                        bx = buf2(2 * i + 0)
                        by = buf2(2 * i + 1)
                        tx = ax * bx - ay * by
                        ty = ax * by + ay * bx
                        buf(2 * i + 0) = tx
                        buf(2 * i + 1) = ty
                    Next
                    fft.fftr1dinvinternaleven(buf, p, buf3, plan)
                    If circular Then

                        '
                        ' circular, add tail to head
                        '
                        r = New Double(m - 1) {}
                        For i_ = 0 To m - 1
                            r(i_) = buf(i_)
                        Next
                        If n >= 2 Then
                            i1_ = (m) - (0)
                            For i_ = 0 To n - 2
                                r(i_) = r(i_) + buf(i_ + i1_)
                            Next
                        End If
                    Else

                        '
                        ' non-circular, just copy
                        '
                        r = New Double(m + n - 2) {}
                        For i_ = 0 To m + n - 2
                            r(i_) = buf(i_)
                        Next
                    End If
                End If
                Return
            End If

            '
            ' overlap-add method
            '
            If alg = 2 Then
                alglib.ap.assert((q + n - 1) Mod 2 = 0, "ConvR1DX: internal error!")
                buf = New Double(q + n - 2) {}
                buf2 = New Double(q + n - 2) {}
                buf3 = New Double(q + n - 2) {}
                ftbase.ftcomplexfftplan((q + n - 1) \ 2, 1, plan)

                '
                ' prepare R
                '
                If circular Then
                    r = New Double(m - 1) {}
                    For i = 0 To m - 1
                        r(i) = 0
                    Next
                Else
                    r = New Double(m + n - 2) {}
                    For i = 0 To m + n - 2
                        r(i) = 0
                    Next
                End If

                '
                ' pre-calculated FFT(B)
                '
                For i_ = 0 To n - 1
                    buf2(i_) = b(i_)
                Next
                For j = n To q + n - 2
                    buf2(j) = 0
                Next
                fft.fftr1dinternaleven(buf2, q + n - 1, buf3, plan)

                '
                ' main overlap-add cycle
                '
                i = 0
                While i <= m - 1
                    p = System.Math.Min(q, m - i)
                    i1_ = (i) - (0)
                    For i_ = 0 To p - 1
                        buf(i_) = a(i_ + i1_)
                    Next
                    For j = p To q + n - 2
                        buf(j) = 0
                    Next
                    fft.fftr1dinternaleven(buf, q + n - 1, buf3, plan)
                    buf(0) = buf(0) * buf2(0)
                    buf(1) = buf(1) * buf2(1)
                    For j = 1 To (q + n - 1) \ 2 - 1
                        ax = buf(2 * j + 0)
                        ay = buf(2 * j + 1)
                        bx = buf2(2 * j + 0)
                        by = buf2(2 * j + 1)
                        tx = ax * bx - ay * by
                        ty = ax * by + ay * bx
                        buf(2 * j + 0) = tx
                        buf(2 * j + 1) = ty
                    Next
                    fft.fftr1dinvinternaleven(buf, q + n - 1, buf3, plan)
                    If circular Then
                        j1 = System.Math.Min(i + p + n - 2, m - 1) - i
                        j2 = j1 + 1
                    Else
                        j1 = p + n - 2
                        j2 = j1 + 1
                    End If
                    i1_ = (0) - (i)
                    For i_ = i To i + j1
                        r(i_) = r(i_) + buf(i_ + i1_)
                    Next
                    If p + n - 2 >= j2 Then
                        i1_ = (j2) - (0)
                        For i_ = 0 To p + n - 2 - j2
                            r(i_) = r(i_) + buf(i_ + i1_)
                        Next
                    End If
                    i = i + p
                End While
                Return
            End If
        End Sub


    End Class
    Public Class corr
        '************************************************************************
        '        1-dimensional complex cross-correlation.
        '
        '        For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).
        '
        '        Correlation is calculated using reduction to  convolution.  Algorithm with
        '        max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
        '        about performance).
        '
        '        IMPORTANT:
        '            for  historical reasons subroutine accepts its parameters in  reversed
        '            order: CorrC1D(Signal, Pattern) = Pattern x Signal (using  traditional
        '            definition of cross-correlation, denoting cross-correlation as "x").
        '
        '        INPUT PARAMETERS
        '            Signal  -   array[0..N-1] - complex function to be transformed,
        '                        signal containing pattern
        '            N       -   problem size
        '            Pattern -   array[0..M-1] - complex function to be transformed,
        '                        pattern to search withing signal
        '            M       -   problem size
        '
        '        OUTPUT PARAMETERS
        '            R       -   cross-correlation, array[0..N+M-2]:
        '                        * positive lags are stored in R[0..N-1],
        '                          R[i] = sum(conj(pattern[j])*signal[i+j]
        '                        * negative lags are stored in R[N..N+M-2],
        '                          R[N+M-1-i] = sum(conj(pattern[j])*signal[-i+j]
        '
        '        NOTE:
        '            It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
        '        on [-K..M-1],  you can still use this subroutine, just shift result by K.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub corrc1d(signal As complex(), n As Integer, pattern As complex(), m As Integer, ByRef r As complex())
            Dim p As complex() = New complex(-1) {}
            Dim b As complex() = New complex(-1) {}
            Dim i As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            r = New complex(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "CorrC1D: incorrect N or M!")
            p = New complex(m - 1) {}
            For i = 0 To m - 1
                p(m - 1 - i) = Math.conj(pattern(i))
            Next
            conv.convc1d(p, m, signal, n, b)
            r = New complex(m + n - 2) {}
            i1_ = (m - 1) - (0)
            For i_ = 0 To n - 1
                r(i_) = b(i_ + i1_)
            Next
            If m + n - 2 >= n Then
                i1_ = (0) - (n)
                For i_ = n To m + n - 2
                    r(i_) = b(i_ + i1_)
                Next
            End If
        End Sub


        '************************************************************************
        '        1-dimensional circular complex cross-correlation.
        '
        '        For given Pattern/Signal returns corr(Pattern,Signal) (circular).
        '        Algorithm has linearithmic complexity for any M/N.
        '
        '        IMPORTANT:
        '            for  historical reasons subroutine accepts its parameters in  reversed
        '            order:   CorrC1DCircular(Signal, Pattern) = Pattern x Signal    (using
        '            traditional definition of cross-correlation, denoting cross-correlation
        '            as "x").
        '
        '        INPUT PARAMETERS
        '            Signal  -   array[0..N-1] - complex function to be transformed,
        '                        periodic signal containing pattern
        '            N       -   problem size
        '            Pattern -   array[0..M-1] - complex function to be transformed,
        '                        non-periodic pattern to search withing signal
        '            M       -   problem size
        '
        '        OUTPUT PARAMETERS
        '            R   -   convolution: A*B. array[0..M-1].
        '
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub corrc1dcircular(signal As complex(), m As Integer, pattern As complex(), n As Integer, ByRef c As complex())
            Dim p As complex() = New complex(-1) {}
            Dim b As complex() = New complex(-1) {}
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim i As Integer = 0
            Dim j2 As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            c = New complex(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvC1DCircular: incorrect N or M!")

            '
            ' normalize task: make M>=N,
            ' so A will be longer (at least - not shorter) that B.
            '
            If m < n Then
                b = New complex(m - 1) {}
                For i1 = 0 To m - 1
                    b(i1) = 0
                Next
                i1 = 0
                While i1 < n
                    i2 = System.Math.Min(i1 + m - 1, n - 1)
                    j2 = i2 - i1
                    i1_ = (i1) - (0)
                    For i_ = 0 To j2
                        b(i_) = b(i_) + pattern(i_ + i1_)
                    Next
                    i1 = i1 + m
                End While
                corrc1dcircular(signal, m, b, m, c)
                Return
            End If

            '
            ' Task is normalized
            '
            p = New complex(n - 1) {}
            For i = 0 To n - 1
                p(n - 1 - i) = Math.conj(pattern(i))
            Next
            conv.convc1dcircular(signal, m, p, n, b)
            c = New complex(m - 1) {}
            i1_ = (n - 1) - (0)
            For i_ = 0 To m - n
                c(i_) = b(i_ + i1_)
            Next
            If m - n + 1 <= m - 1 Then
                i1_ = (0) - (m - n + 1)
                For i_ = m - n + 1 To m - 1
                    c(i_) = b(i_ + i1_)
                Next
            End If
        End Sub


        '************************************************************************
        '        1-dimensional real cross-correlation.
        '
        '        For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).
        '
        '        Correlation is calculated using reduction to  convolution.  Algorithm with
        '        max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
        '        about performance).
        '
        '        IMPORTANT:
        '            for  historical reasons subroutine accepts its parameters in  reversed
        '            order: CorrR1D(Signal, Pattern) = Pattern x Signal (using  traditional
        '            definition of cross-correlation, denoting cross-correlation as "x").
        '
        '        INPUT PARAMETERS
        '            Signal  -   array[0..N-1] - real function to be transformed,
        '                        signal containing pattern
        '            N       -   problem size
        '            Pattern -   array[0..M-1] - real function to be transformed,
        '                        pattern to search withing signal
        '            M       -   problem size
        '
        '        OUTPUT PARAMETERS
        '            R       -   cross-correlation, array[0..N+M-2]:
        '                        * positive lags are stored in R[0..N-1],
        '                          R[i] = sum(pattern[j]*signal[i+j]
        '                        * negative lags are stored in R[N..N+M-2],
        '                          R[N+M-1-i] = sum(pattern[j]*signal[-i+j]
        '
        '        NOTE:
        '            It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
        '        on [-K..M-1],  you can still use this subroutine, just shift result by K.
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub corrr1d(signal As Double(), n As Integer, pattern As Double(), m As Integer, ByRef r As Double())
            Dim p As Double() = New Double(-1) {}
            Dim b As Double() = New Double(-1) {}
            Dim i As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            r = New Double(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "CorrR1D: incorrect N or M!")
            p = New Double(m - 1) {}
            For i = 0 To m - 1
                p(m - 1 - i) = pattern(i)
            Next
            conv.convr1d(p, m, signal, n, b)
            r = New Double(m + n - 2) {}
            i1_ = (m - 1) - (0)
            For i_ = 0 To n - 1
                r(i_) = b(i_ + i1_)
            Next
            If m + n - 2 >= n Then
                i1_ = (0) - (n)
                For i_ = n To m + n - 2
                    r(i_) = b(i_ + i1_)
                Next
            End If
        End Sub


        '************************************************************************
        '        1-dimensional circular real cross-correlation.
        '
        '        For given Pattern/Signal returns corr(Pattern,Signal) (circular).
        '        Algorithm has linearithmic complexity for any M/N.
        '
        '        IMPORTANT:
        '            for  historical reasons subroutine accepts its parameters in  reversed
        '            order:   CorrR1DCircular(Signal, Pattern) = Pattern x Signal    (using
        '            traditional definition of cross-correlation, denoting cross-correlation
        '            as "x").
        '
        '        INPUT PARAMETERS
        '            Signal  -   array[0..N-1] - real function to be transformed,
        '                        periodic signal containing pattern
        '            N       -   problem size
        '            Pattern -   array[0..M-1] - real function to be transformed,
        '                        non-periodic pattern to search withing signal
        '            M       -   problem size
        '
        '        OUTPUT PARAMETERS
        '            R   -   convolution: A*B. array[0..M-1].
        '
        '
        '          -- ALGLIB --
        '             Copyright 21.07.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub corrr1dcircular(signal As Double(), m As Integer, pattern As Double(), n As Integer, ByRef c As Double())
            Dim p As Double() = New Double(-1) {}
            Dim b As Double() = New Double(-1) {}
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim i As Integer = 0
            Dim j2 As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            c = New Double(-1) {}

            alglib.ap.assert(n > 0 AndAlso m > 0, "ConvC1DCircular: incorrect N or M!")

            '
            ' normalize task: make M>=N,
            ' so A will be longer (at least - not shorter) that B.
            '
            If m < n Then
                b = New Double(m - 1) {}
                For i1 = 0 To m - 1
                    b(i1) = 0
                Next
                i1 = 0
                While i1 < n
                    i2 = System.Math.Min(i1 + m - 1, n - 1)
                    j2 = i2 - i1
                    i1_ = (i1) - (0)
                    For i_ = 0 To j2
                        b(i_) = b(i_) + pattern(i_ + i1_)
                    Next
                    i1 = i1 + m
                End While
                corrr1dcircular(signal, m, b, m, c)
                Return
            End If

            '
            ' Task is normalized
            '
            p = New Double(n - 1) {}
            For i = 0 To n - 1
                p(n - 1 - i) = pattern(i)
            Next
            conv.convr1dcircular(signal, m, p, n, b)
            c = New Double(m - 1) {}
            i1_ = (n - 1) - (0)
            For i_ = 0 To m - n
                c(i_) = b(i_ + i1_)
            Next
            If m - n + 1 <= m - 1 Then
                i1_ = (0) - (m - n + 1)
                For i_ = m - n + 1 To m - 1
                    c(i_) = b(i_ + i1_)
                Next
            End If
        End Sub


    End Class
	Public Class fht
		'************************************************************************
'        1-dimensional Fast Hartley Transform.
'
'        Algorithm has O(N*logN) complexity for any N (composite or prime).
'
'        INPUT PARAMETERS
'            A   -   array[0..N-1] - real function to be transformed
'            N   -   problem size
'            
'        OUTPUT PARAMETERS
'            A   -   FHT of a input array, array[0..N-1],
'                    A_out[k] = sum(A_in[j]*(cos(2*pi*j*k/N)+sin(2*pi*j*k/N)), j=0..N-1)
'
'
'          -- ALGLIB --
'             Copyright 04.06.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub fhtr1d(ByRef a As Double(), n As Integer)
			Dim i As Integer = 0
			Dim fa As complex() = New complex(-1) {}

			alglib.ap.assert(n > 0, "FHTR1D: incorrect N!")

			'
			' Special case: N=1, FHT is just identity transform.
			' After this block we assume that N is strictly greater than 1.
			'
			If n = 1 Then
				Return
			End If

			'
			' Reduce FHt to real FFT
			'
			fft.fftr1d(a, n, fa)
			For i = 0 To n - 1
				a(i) = fa(i).x - fa(i).y
			Next
		End Sub


		'************************************************************************
'        1-dimensional inverse FHT.
'
'        Algorithm has O(N*logN) complexity for any N (composite or prime).
'
'        INPUT PARAMETERS
'            A   -   array[0..N-1] - complex array to be transformed
'            N   -   problem size
'
'        OUTPUT PARAMETERS
'            A   -   inverse FHT of a input array, array[0..N-1]
'
'
'          -- ALGLIB --
'             Copyright 29.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub fhtr1dinv(ByRef a As Double(), n As Integer)
			Dim i As Integer = 0

			alglib.ap.assert(n > 0, "FHTR1DInv: incorrect N!")

			'
			' Special case: N=1, iFHT is just identity transform.
			' After this block we assume that N is strictly greater than 1.
			'
			If n = 1 Then
				Return
			End If

			'
			' Inverse FHT can be expressed in terms of the FHT as
			'
			'     invfht(x) = fht(x)/N
			'
			fhtr1d(a, n)
			For i = 0 To n - 1
				a(i) = a(i) / n
			Next
		End Sub


	End Class
End Class

