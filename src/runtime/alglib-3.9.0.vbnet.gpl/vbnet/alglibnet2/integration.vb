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
'    Computation of nodes and weights for a Gauss quadrature formula
'
'    The algorithm generates the N-point Gauss quadrature formula  with  weight
'    function given by coefficients alpha and beta  of  a  recurrence  relation
'    which generates a system of orthogonal polynomials:
'
'    P-1(x)   =  0
'    P0(x)    =  1
'    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)
'
'    and zeroth moment Mu0
'
'    Mu0 = integral(W(x)dx,a,b)
'
'    INPUT PARAMETERS:
'        Alpha   ?  array[0..N-1], alpha coefficients
'        Beta    ?  array[0..N-1], beta coefficients
'                    Zero-indexed element is not used and may be arbitrary.
'                    Beta[I]>0.
'        Mu0     ?  zeroth moment of the weight function.
'        N       ?  number of nodes of the quadrature formula, N>=1
'
'    OUTPUT PARAMETERS:
'        Info    -   error code:
'                    * -3    internal eigenproblem solver hasn't converged
'                    * -2    Beta[i]<=0
'                    * -1    incorrect N was passed
'                    *  1    OK
'        X       -   array[0..N-1] - array of quadrature nodes,
'                    in ascending order.
'        W       -   array[0..N-1] - array of quadrature weights.
'
'      -- ALGLIB --
'         Copyright 2005-2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gqgeneraterec(alpha As Double(), beta As Double(), mu0 As Double, n As Integer, ByRef info As Integer, ByRef x As Double(), _
		ByRef w As Double())
		info = 0
		x = New Double(-1) {}
		w = New Double(-1) {}
		gq.gqgeneraterec(alpha, beta, mu0, n, info, x, _
			w)
		Return
	End Sub

	'************************************************************************
'    Computation of nodes and weights for a Gauss-Lobatto quadrature formula
'
'    The algorithm generates the N-point Gauss-Lobatto quadrature formula  with
'    weight function given by coefficients alpha and beta of a recurrence which
'    generates a system of orthogonal polynomials.
'
'    P-1(x)   =  0
'    P0(x)    =  1
'    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)
'
'    and zeroth moment Mu0
'
'    Mu0 = integral(W(x)dx,a,b)
'
'    INPUT PARAMETERS:
'        Alpha   ?  array[0..N-2], alpha coefficients
'        Beta    ?  array[0..N-2], beta coefficients.
'                    Zero-indexed element is not used, may be arbitrary.
'                    Beta[I]>0
'        Mu0     ?  zeroth moment of the weighting function.
'        A       ?  left boundary of the integration interval.
'        B       ?  right boundary of the integration interval.
'        N       ?  number of nodes of the quadrature formula, N>=3
'                    (including the left and right boundary nodes).
'
'    OUTPUT PARAMETERS:
'        Info    -   error code:
'                    * -3    internal eigenproblem solver hasn't converged
'                    * -2    Beta[i]<=0
'                    * -1    incorrect N was passed
'                    *  1    OK
'        X       -   array[0..N-1] - array of quadrature nodes,
'                    in ascending order.
'        W       -   array[0..N-1] - array of quadrature weights.
'
'      -- ALGLIB --
'         Copyright 2005-2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gqgenerategausslobattorec(alpha As Double(), beta As Double(), mu0 As Double, a As Double, b As Double, n As Integer, _
		ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
		info = 0
		x = New Double(-1) {}
		w = New Double(-1) {}
		gq.gqgenerategausslobattorec(alpha, beta, mu0, a, b, n, _
			info, x, w)
		Return
	End Sub

	'************************************************************************
'    Computation of nodes and weights for a Gauss-Radau quadrature formula
'
'    The algorithm generates the N-point Gauss-Radau  quadrature  formula  with
'    weight function given by the coefficients alpha and  beta  of a recurrence
'    which generates a system of orthogonal polynomials.
'
'    P-1(x)   =  0
'    P0(x)    =  1
'    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)
'
'    and zeroth moment Mu0
'
'    Mu0 = integral(W(x)dx,a,b)
'
'    INPUT PARAMETERS:
'        Alpha   ?  array[0..N-2], alpha coefficients.
'        Beta    ?  array[0..N-1], beta coefficients
'                    Zero-indexed element is not used.
'                    Beta[I]>0
'        Mu0     ?  zeroth moment of the weighting function.
'        A       ?  left boundary of the integration interval.
'        N       ?  number of nodes of the quadrature formula, N>=2
'                    (including the left boundary node).
'
'    OUTPUT PARAMETERS:
'        Info    -   error code:
'                    * -3    internal eigenproblem solver hasn't converged
'                    * -2    Beta[i]<=0
'                    * -1    incorrect N was passed
'                    *  1    OK
'        X       -   array[0..N-1] - array of quadrature nodes,
'                    in ascending order.
'        W       -   array[0..N-1] - array of quadrature weights.
'
'
'      -- ALGLIB --
'         Copyright 2005-2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gqgenerategaussradaurec(alpha As Double(), beta As Double(), mu0 As Double, a As Double, n As Integer, ByRef info As Integer, _
		ByRef x As Double(), ByRef w As Double())
		info = 0
		x = New Double(-1) {}
		w = New Double(-1) {}
		gq.gqgenerategaussradaurec(alpha, beta, mu0, a, n, info, _
			x, w)
		Return
	End Sub

	'************************************************************************
'    Returns nodes/weights for Gauss-Legendre quadrature on [-1,1] with N
'    nodes.
'
'    INPUT PARAMETERS:
'        N           -   number of nodes, >=1
'
'    OUTPUT PARAMETERS:
'        Info        -   error code:
'                        * -4    an  error   was   detected   when  calculating
'                                weights/nodes.  N  is  too  large   to  obtain
'                                weights/nodes  with  high   enough   accuracy.
'                                Try  to   use   multiple   precision  version.
'                        * -3    internal eigenproblem solver hasn't  converged
'                        * -1    incorrect N was passed
'                        * +1    OK
'        X           -   array[0..N-1] - array of quadrature nodes,
'                        in ascending order.
'        W           -   array[0..N-1] - array of quadrature weights.
'
'
'      -- ALGLIB --
'         Copyright 12.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gqgenerategausslegendre(n As Integer, ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
		info = 0
		x = New Double(-1) {}
		w = New Double(-1) {}
		gq.gqgenerategausslegendre(n, info, x, w)
		Return
	End Sub

	'************************************************************************
'    Returns  nodes/weights  for  Gauss-Jacobi quadrature on [-1,1] with weight
'    function W(x)=Power(1-x,Alpha)*Power(1+x,Beta).
'
'    INPUT PARAMETERS:
'        N           -   number of nodes, >=1
'        Alpha       -   power-law coefficient, Alpha>-1
'        Beta        -   power-law coefficient, Beta>-1
'
'    OUTPUT PARAMETERS:
'        Info        -   error code:
'                        * -4    an  error  was   detected   when   calculating
'                                weights/nodes. Alpha or  Beta  are  too  close
'                                to -1 to obtain weights/nodes with high enough
'                                accuracy, or, may be, N is too large.  Try  to
'                                use multiple precision version.
'                        * -3    internal eigenproblem solver hasn't converged
'                        * -1    incorrect N/Alpha/Beta was passed
'                        * +1    OK
'        X           -   array[0..N-1] - array of quadrature nodes,
'                        in ascending order.
'        W           -   array[0..N-1] - array of quadrature weights.
'
'
'      -- ALGLIB --
'         Copyright 12.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gqgenerategaussjacobi(n As Integer, alpha As Double, beta As Double, ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
		info = 0
		x = New Double(-1) {}
		w = New Double(-1) {}
		gq.gqgenerategaussjacobi(n, alpha, beta, info, x, w)
		Return
	End Sub

	'************************************************************************
'    Returns  nodes/weights  for  Gauss-Laguerre  quadrature  on  [0,+inf) with
'    weight function W(x)=Power(x,Alpha)*Exp(-x)
'
'    INPUT PARAMETERS:
'        N           -   number of nodes, >=1
'        Alpha       -   power-law coefficient, Alpha>-1
'
'    OUTPUT PARAMETERS:
'        Info        -   error code:
'                        * -4    an  error  was   detected   when   calculating
'                                weights/nodes. Alpha is too  close  to  -1  to
'                                obtain weights/nodes with high enough accuracy
'                                or, may  be,  N  is  too  large.  Try  to  use
'                                multiple precision version.
'                        * -3    internal eigenproblem solver hasn't converged
'                        * -1    incorrect N/Alpha was passed
'                        * +1    OK
'        X           -   array[0..N-1] - array of quadrature nodes,
'                        in ascending order.
'        W           -   array[0..N-1] - array of quadrature weights.
'
'
'      -- ALGLIB --
'         Copyright 12.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gqgenerategausslaguerre(n As Integer, alpha As Double, ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
		info = 0
		x = New Double(-1) {}
		w = New Double(-1) {}
		gq.gqgenerategausslaguerre(n, alpha, info, x, w)
		Return
	End Sub

	'************************************************************************
'    Returns  nodes/weights  for  Gauss-Hermite  quadrature on (-inf,+inf) with
'    weight function W(x)=Exp(-x*x)
'
'    INPUT PARAMETERS:
'        N           -   number of nodes, >=1
'
'    OUTPUT PARAMETERS:
'        Info        -   error code:
'                        * -4    an  error  was   detected   when   calculating
'                                weights/nodes.  May be, N is too large. Try to
'                                use multiple precision version.
'                        * -3    internal eigenproblem solver hasn't converged
'                        * -1    incorrect N/Alpha was passed
'                        * +1    OK
'        X           -   array[0..N-1] - array of quadrature nodes,
'                        in ascending order.
'        W           -   array[0..N-1] - array of quadrature weights.
'
'
'      -- ALGLIB --
'         Copyright 12.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gqgenerategausshermite(n As Integer, ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
		info = 0
		x = New Double(-1) {}
		w = New Double(-1) {}
		gq.gqgenerategausshermite(n, info, x, w)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Computation of nodes and weights of a Gauss-Kronrod quadrature formula
'
'    The algorithm generates the N-point Gauss-Kronrod quadrature formula  with
'    weight  function  given  by  coefficients  alpha  and beta of a recurrence
'    relation which generates a system of orthogonal polynomials:
'
'        P-1(x)   =  0
'        P0(x)    =  1
'        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)
'
'    and zero moment Mu0
'
'        Mu0 = integral(W(x)dx,a,b)
'
'
'    INPUT PARAMETERS:
'        Alpha       ?  alpha coefficients, array[0..floor(3*K/2)].
'        Beta        ?  beta coefficients,  array[0..ceil(3*K/2)].
'                        Beta[0] is not used and may be arbitrary.
'                        Beta[I]>0.
'        Mu0         ?  zeroth moment of the weight function.
'        N           ?  number of nodes of the Gauss-Kronrod quadrature formula,
'                        N >= 3,
'                        N =  2*K+1.
'
'    OUTPUT PARAMETERS:
'        Info        -   error code:
'                        * -5    no real and positive Gauss-Kronrod formula can
'                                be created for such a weight function  with  a
'                                given number of nodes.
'                        * -4    N is too large, task may be ill  conditioned -
'                                x[i]=x[i+1] found.
'                        * -3    internal eigenproblem solver hasn't converged
'                        * -2    Beta[i]<=0
'                        * -1    incorrect N was passed
'                        * +1    OK
'        X           -   array[0..N-1] - array of quadrature nodes,
'                        in ascending order.
'        WKronrod    -   array[0..N-1] - Kronrod weights
'        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
'                        corresponding to extended Kronrod nodes).
'
'      -- ALGLIB --
'         Copyright 08.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gkqgeneraterec(alpha As Double(), beta As Double(), mu0 As Double, n As Integer, ByRef info As Integer, ByRef x As Double(), _
		ByRef wkronrod As Double(), ByRef wgauss As Double())
		info = 0
		x = New Double(-1) {}
		wkronrod = New Double(-1) {}
		wgauss = New Double(-1) {}
		gkq.gkqgeneraterec(alpha, beta, mu0, n, info, x, _
			wkronrod, wgauss)
		Return
	End Sub

	'************************************************************************
'    Returns   Gauss   and   Gauss-Kronrod   nodes/weights  for  Gauss-Legendre
'    quadrature with N points.
'
'    GKQLegendreCalc (calculation) or  GKQLegendreTbl  (precomputed  table)  is
'    used depending on machine precision and number of nodes.
'
'    INPUT PARAMETERS:
'        N           -   number of Kronrod nodes, must be odd number, >=3.
'
'    OUTPUT PARAMETERS:
'        Info        -   error code:
'                        * -4    an  error   was   detected   when  calculating
'                                weights/nodes.  N  is  too  large   to  obtain
'                                weights/nodes  with  high   enough   accuracy.
'                                Try  to   use   multiple   precision  version.
'                        * -3    internal eigenproblem solver hasn't converged
'                        * -1    incorrect N was passed
'                        * +1    OK
'        X           -   array[0..N-1] - array of quadrature nodes, ordered in
'                        ascending order.
'        WKronrod    -   array[0..N-1] - Kronrod weights
'        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
'                        corresponding to extended Kronrod nodes).
'
'
'      -- ALGLIB --
'         Copyright 12.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gkqgenerategausslegendre(n As Integer, ByRef info As Integer, ByRef x As Double(), ByRef wkronrod As Double(), ByRef wgauss As Double())
		info = 0
		x = New Double(-1) {}
		wkronrod = New Double(-1) {}
		wgauss = New Double(-1) {}
		gkq.gkqgenerategausslegendre(n, info, x, wkronrod, wgauss)
		Return
	End Sub

	'************************************************************************
'    Returns   Gauss   and   Gauss-Kronrod   nodes/weights   for   Gauss-Jacobi
'    quadrature on [-1,1] with weight function
'
'        W(x)=Power(1-x,Alpha)*Power(1+x,Beta).
'
'    INPUT PARAMETERS:
'        N           -   number of Kronrod nodes, must be odd number, >=3.
'        Alpha       -   power-law coefficient, Alpha>-1
'        Beta        -   power-law coefficient, Beta>-1
'
'    OUTPUT PARAMETERS:
'        Info        -   error code:
'                        * -5    no real and positive Gauss-Kronrod formula can
'                                be created for such a weight function  with  a
'                                given number of nodes.
'                        * -4    an  error  was   detected   when   calculating
'                                weights/nodes. Alpha or  Beta  are  too  close
'                                to -1 to obtain weights/nodes with high enough
'                                accuracy, or, may be, N is too large.  Try  to
'                                use multiple precision version.
'                        * -3    internal eigenproblem solver hasn't converged
'                        * -1    incorrect N was passed
'                        * +1    OK
'                        * +2    OK, but quadrature rule have exterior  nodes,
'                                x[0]<-1 or x[n-1]>+1
'        X           -   array[0..N-1] - array of quadrature nodes, ordered in
'                        ascending order.
'        WKronrod    -   array[0..N-1] - Kronrod weights
'        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
'                        corresponding to extended Kronrod nodes).
'
'
'      -- ALGLIB --
'         Copyright 12.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gkqgenerategaussjacobi(n As Integer, alpha As Double, beta As Double, ByRef info As Integer, ByRef x As Double(), ByRef wkronrod As Double(), _
		ByRef wgauss As Double())
		info = 0
		x = New Double(-1) {}
		wkronrod = New Double(-1) {}
		wgauss = New Double(-1) {}
		gkq.gkqgenerategaussjacobi(n, alpha, beta, info, x, wkronrod, _
			wgauss)
		Return
	End Sub

	'************************************************************************
'    Returns Gauss and Gauss-Kronrod nodes for quadrature with N points.
'
'    Reduction to tridiagonal eigenproblem is used.
'
'    INPUT PARAMETERS:
'        N           -   number of Kronrod nodes, must be odd number, >=3.
'
'    OUTPUT PARAMETERS:
'        Info        -   error code:
'                        * -4    an  error   was   detected   when  calculating
'                                weights/nodes.  N  is  too  large   to  obtain
'                                weights/nodes  with  high   enough   accuracy.
'                                Try  to   use   multiple   precision  version.
'                        * -3    internal eigenproblem solver hasn't converged
'                        * -1    incorrect N was passed
'                        * +1    OK
'        X           -   array[0..N-1] - array of quadrature nodes, ordered in
'                        ascending order.
'        WKronrod    -   array[0..N-1] - Kronrod weights
'        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
'                        corresponding to extended Kronrod nodes).
'
'      -- ALGLIB --
'         Copyright 12.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gkqlegendrecalc(n As Integer, ByRef info As Integer, ByRef x As Double(), ByRef wkronrod As Double(), ByRef wgauss As Double())
		info = 0
		x = New Double(-1) {}
		wkronrod = New Double(-1) {}
		wgauss = New Double(-1) {}
		gkq.gkqlegendrecalc(n, info, x, wkronrod, wgauss)
		Return
	End Sub

	'************************************************************************
'    Returns Gauss and Gauss-Kronrod nodes for quadrature with N  points  using
'    pre-calculated table. Nodes/weights were  computed  with  accuracy  up  to
'    1.0E-32 (if MPFR version of ALGLIB is used). In standard double  precision
'    accuracy reduces to something about 2.0E-16 (depending  on your compiler's
'    handling of long floating point constants).
'
'    INPUT PARAMETERS:
'        N           -   number of Kronrod nodes.
'                        N can be 15, 21, 31, 41, 51, 61.
'
'    OUTPUT PARAMETERS:
'        X           -   array[0..N-1] - array of quadrature nodes, ordered in
'                        ascending order.
'        WKronrod    -   array[0..N-1] - Kronrod weights
'        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
'                        corresponding to extended Kronrod nodes).
'
'
'      -- ALGLIB --
'         Copyright 12.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub gkqlegendretbl(n As Integer, ByRef x As Double(), ByRef wkronrod As Double(), ByRef wgauss As Double(), ByRef eps As Double)
		x = New Double(-1) {}
		wkronrod = New Double(-1) {}
		wgauss = New Double(-1) {}
		eps = 0
		gkq.gkqlegendretbl(n, x, wkronrod, wgauss, eps)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    Integration report:
'    * TerminationType = completetion code:
'        * -5    non-convergence of Gauss-Kronrod nodes
'                calculation subroutine.
'        * -1    incorrect parameters were specified
'        *  1    OK
'    * Rep.NFEV countains number of function calculations
'    * Rep.NIntervals contains number of intervals [a,b]
'      was partitioned into.
'    ************************************************************************

	Public Class autogkreport
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property terminationtype() As Integer
			Get
				Return _innerobj.terminationtype
			End Get
			Set
				_innerobj.terminationtype = value
			End Set
		End Property
		Public Property nfev() As Integer
			Get
				Return _innerobj.nfev
			End Get
			Set
				_innerobj.nfev = value
			End Set
		End Property
		Public Property nintervals() As Integer
			Get
				Return _innerobj.nintervals
			End Get
			Set
				_innerobj.nintervals = value
			End Set
		End Property

		Public Sub New()
			_innerobj = New autogk.autogkreport()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New autogkreport(DirectCast(_innerobj.make_copy(), autogk.autogkreport))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As autogk.autogkreport
		Public ReadOnly Property innerobj() As autogk.autogkreport
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As autogk.autogkreport)
			_innerobj = obj
		End Sub
	End Class


	'************************************************************************
'    This structure stores state of the integration algorithm.
'
'    Although this class has public fields,  they are not intended for external
'    use. You should use ALGLIB functions to work with this class:
'    * autogksmooth()/AutoGKSmoothW()/... to create objects
'    * autogkintegrate() to begin integration
'    * autogkresults() to get results
'    ************************************************************************

	Public Class autogkstate
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property needf() As Boolean
			Get
				Return _innerobj.needf
			End Get
			Set
				_innerobj.needf = value
			End Set
		End Property
		Public Property x() As Double
			Get
				Return _innerobj.x
			End Get
			Set
				_innerobj.x = value
			End Set
		End Property
		Public Property xminusa() As Double
			Get
				Return _innerobj.xminusa
			End Get
			Set
				_innerobj.xminusa = value
			End Set
		End Property
		Public Property bminusx() As Double
			Get
				Return _innerobj.bminusx
			End Get
			Set
				_innerobj.bminusx = value
			End Set
		End Property
		Public Property f() As Double
			Get
				Return _innerobj.f
			End Get
			Set
				_innerobj.f = value
			End Set
		End Property

		Public Sub New()
			_innerobj = New autogk.autogkstate()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New autogkstate(DirectCast(_innerobj.make_copy(), autogk.autogkstate))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As autogk.autogkstate
		Public ReadOnly Property innerobj() As autogk.autogkstate
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As autogk.autogkstate)
			_innerobj = obj
		End Sub
	End Class

	'************************************************************************
'    Integration of a smooth function F(x) on a finite interval [a,b].
'
'    Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
'    is calculated with accuracy close to the machine precision.
'
'    Algorithm works well only with smooth integrands.  It  may  be  used  with
'    continuous non-smooth integrands, but with  less  performance.
'
'    It should never be used with integrands which have integrable singularities
'    at lower or upper limits - algorithm may crash. Use AutoGKSingular in such
'    cases.
'
'    INPUT PARAMETERS:
'        A, B    -   interval boundaries (A<B, A=B or A>B)
'
'    OUTPUT PARAMETERS
'        State   -   structure which stores algorithm state
'
'    SEE ALSO
'        AutoGKSmoothW, AutoGKSingular, AutoGKResults.
'
'
'      -- ALGLIB --
'         Copyright 06.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub autogksmooth(a As Double, b As Double, ByRef state As autogkstate)
		state = New autogkstate()
		autogk.autogksmooth(a, b, state.innerobj)
		Return
	End Sub

	'************************************************************************
'    Integration of a smooth function F(x) on a finite interval [a,b].
'
'    This subroutine is same as AutoGKSmooth(), but it guarantees that interval
'    [a,b] is partitioned into subintervals which have width at most XWidth.
'
'    Subroutine  can  be  used  when  integrating nearly-constant function with
'    narrow "bumps" (about XWidth wide). If "bumps" are too narrow, AutoGKSmooth
'    subroutine can overlook them.
'
'    INPUT PARAMETERS:
'        A, B    -   interval boundaries (A<B, A=B or A>B)
'
'    OUTPUT PARAMETERS
'        State   -   structure which stores algorithm state
'
'    SEE ALSO
'        AutoGKSmooth, AutoGKSingular, AutoGKResults.
'
'
'      -- ALGLIB --
'         Copyright 06.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub autogksmoothw(a As Double, b As Double, xwidth As Double, ByRef state As autogkstate)
		state = New autogkstate()
		autogk.autogksmoothw(a, b, xwidth, state.innerobj)
		Return
	End Sub

	'************************************************************************
'    Integration on a finite interval [A,B].
'    Integrand have integrable singularities at A/B.
'
'    F(X) must diverge as "(x-A)^alpha" at A, as "(B-x)^beta" at B,  with known
'    alpha/beta (alpha>-1, beta>-1).  If alpha/beta  are  not known,  estimates
'    from below can be used (but these estimates should be greater than -1 too).
'
'    One  of  alpha/beta variables (or even both alpha/beta) may be equal to 0,
'    which means than function F(x) is non-singular at A/B. Anyway (singular at
'    bounds or not), function F(x) is supposed to be continuous on (A,B).
'
'    Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
'    is calculated with accuracy close to the machine precision.
'
'    INPUT PARAMETERS:
'        A, B    -   interval boundaries (A<B, A=B or A>B)
'        Alpha   -   power-law coefficient of the F(x) at A,
'                    Alpha>-1
'        Beta    -   power-law coefficient of the F(x) at B,
'                    Beta>-1
'
'    OUTPUT PARAMETERS
'        State   -   structure which stores algorithm state
'
'    SEE ALSO
'        AutoGKSmooth, AutoGKSmoothW, AutoGKResults.
'
'
'      -- ALGLIB --
'         Copyright 06.05.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub autogksingular(a As Double, b As Double, alpha As Double, beta As Double, ByRef state As autogkstate)
		state = New autogkstate()
		autogk.autogksingular(a, b, alpha, beta, state.innerobj)
		Return
	End Sub

	'************************************************************************
'    This function provides reverse communication interface
'    Reverse communication interface is not documented or recommended to use.
'    See below for functions which provide better documented API
'    ************************************************************************

	Public Shared Function autogkiteration(state As autogkstate) As Boolean

		Dim result As Boolean = autogk.autogkiteration(state.innerobj)
		Return result
	End Function


	'************************************************************************
'    This function is used to launcn iterations of ODE solver
'
'    It accepts following parameters:
'        diff    -   callback which calculates dy/dx for given y and x
'        obj     -   optional object which is passed to diff; can be NULL
'
'
'      -- ALGLIB --
'         Copyright 07.05.2009 by Bochkanov Sergey
'
'    ************************************************************************

	Public Shared Sub autogkintegrate(state As autogkstate, func As integrator1_func, obj As Object)
		If func Is Nothing Then
			Throw New alglibexception("ALGLIB: error in 'autogkintegrate()' (func is null)")
		End If
		While alglib.autogkiteration(state)
			If state.needf Then
				func(state.innerobj.x, state.innerobj.xminusa, state.innerobj.bminusx, state.innerobj.f, obj)
				Continue While
			End If
			Throw New alglibexception("ALGLIB: unexpected error in 'autogksolve'")
		End While
	End Sub

	'************************************************************************
'    Adaptive integration results
'
'    Called after AutoGKIteration returned False.
'
'    Input parameters:
'        State   -   algorithm state (used by AutoGKIteration).
'
'    Output parameters:
'        V       -   integral(f(x)dx,a,b)
'        Rep     -   optimization report (see AutoGKReport description)
'
'      -- ALGLIB --
'         Copyright 14.11.2007 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub autogkresults(state As autogkstate, ByRef v As Double, ByRef rep As autogkreport)
		v = 0
		rep = New autogkreport()
		autogk.autogkresults(state.innerobj, v, rep.innerobj)
		Return
	End Sub

End Class
Public Partial Class alglib
	Public Class gq
		'************************************************************************
'        Computation of nodes and weights for a Gauss quadrature formula
'
'        The algorithm generates the N-point Gauss quadrature formula  with  weight
'        function given by coefficients alpha and beta  of  a  recurrence  relation
'        which generates a system of orthogonal polynomials:
'
'        P-1(x)   =  0
'        P0(x)    =  1
'        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)
'
'        and zeroth moment Mu0
'
'        Mu0 = integral(W(x)dx,a,b)
'
'        INPUT PARAMETERS:
'            Alpha   ?  array[0..N-1], alpha coefficients
'            Beta    ?  array[0..N-1], beta coefficients
'                        Zero-indexed element is not used and may be arbitrary.
'                        Beta[I]>0.
'            Mu0     ?  zeroth moment of the weight function.
'            N       ?  number of nodes of the quadrature formula, N>=1
'
'        OUTPUT PARAMETERS:
'            Info    -   error code:
'                        * -3    internal eigenproblem solver hasn't converged
'                        * -2    Beta[i]<=0
'                        * -1    incorrect N was passed
'                        *  1    OK
'            X       -   array[0..N-1] - array of quadrature nodes,
'                        in ascending order.
'            W       -   array[0..N-1] - array of quadrature weights.
'
'          -- ALGLIB --
'             Copyright 2005-2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub gqgeneraterec(alpha As Double(), beta As Double(), mu0 As Double, n As Integer, ByRef info As Integer, ByRef x As Double(), _
			ByRef w As Double())
			Dim i As Integer = 0
			Dim d As Double() = New Double(-1) {}
			Dim e As Double() = New Double(-1) {}
			Dim z As Double(,) = New Double(-1, -1) {}

			info = 0
			x = New Double(-1) {}
			w = New Double(-1) {}

			If n < 1 Then
				info = -1
				Return
			End If
			info = 1

			'
			' Initialize
			'
			d = New Double(n - 1) {}
			e = New Double(n - 1) {}
			For i = 1 To n - 1
				d(i - 1) = alpha(i - 1)
				If CDbl(beta(i)) <= CDbl(0) Then
					info = -2
					Return
				End If
                e(i - 1) = System.Math.sqrt(beta(i))
            Next
            d(n - 1) = alpha(n - 1)

            '
            ' EVD
            '
            If Not evd.smatrixtdevd(d, e, n, 3, z) Then
                info = -3
                Return
            End If

            '
            ' Generate
            '
            x = New Double(n - 1) {}
            w = New Double(n - 1) {}
            For i = 1 To n
                x(i - 1) = d(i - 1)
                w(i - 1) = mu0 * Math.sqr(z(0, i - 1))
            Next
        End Sub


        '************************************************************************
        '        Computation of nodes and weights for a Gauss-Lobatto quadrature formula
        '
        '        The algorithm generates the N-point Gauss-Lobatto quadrature formula  with
        '        weight function given by coefficients alpha and beta of a recurrence which
        '        generates a system of orthogonal polynomials.
        '
        '        P-1(x)   =  0
        '        P0(x)    =  1
        '        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)
        '
        '        and zeroth moment Mu0
        '
        '        Mu0 = integral(W(x)dx,a,b)
        '
        '        INPUT PARAMETERS:
        '            Alpha   ?  array[0..N-2], alpha coefficients
        '            Beta    ?  array[0..N-2], beta coefficients.
        '                        Zero-indexed element is not used, may be arbitrary.
        '                        Beta[I]>0
        '            Mu0     ?  zeroth moment of the weighting function.
        '            A       ?  left boundary of the integration interval.
        '            B       ?  right boundary of the integration interval.
        '            N       ?  number of nodes of the quadrature formula, N>=3
        '                        (including the left and right boundary nodes).
        '
        '        OUTPUT PARAMETERS:
        '            Info    -   error code:
        '                        * -3    internal eigenproblem solver hasn't converged
        '                        * -2    Beta[i]<=0
        '                        * -1    incorrect N was passed
        '                        *  1    OK
        '            X       -   array[0..N-1] - array of quadrature nodes,
        '                        in ascending order.
        '            W       -   array[0..N-1] - array of quadrature weights.
        '
        '          -- ALGLIB --
        '             Copyright 2005-2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gqgenerategausslobattorec(alpha As Double(), beta As Double(), mu0 As Double, a As Double, b As Double, n As Integer, _
            ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
            Dim i As Integer = 0
            Dim d As Double() = New Double(-1) {}
            Dim e As Double() = New Double(-1) {}
            Dim z As Double(,) = New Double(-1, -1) {}
            Dim pim1a As Double = 0
            Dim pia As Double = 0
            Dim pim1b As Double = 0
            Dim pib As Double = 0
            Dim t As Double = 0
            Dim a11 As Double = 0
            Dim a12 As Double = 0
            Dim a21 As Double = 0
            Dim a22 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0
            Dim alph As Double = 0
            Dim bet As Double = 0

            alpha = DirectCast(alpha.Clone(), Double())
            beta = DirectCast(beta.Clone(), Double())
            info = 0
            x = New Double(-1) {}
            w = New Double(-1) {}

            If n <= 2 Then
                info = -1
                Return
            End If
            info = 1

            '
            ' Initialize, D[1:N+1], E[1:N]
            '
            n = n - 2
            d = New Double(n + 1) {}
            e = New Double(n) {}
            For i = 1 To n + 1
                d(i - 1) = alpha(i - 1)
            Next
            For i = 1 To n
                If CDbl(beta(i)) <= CDbl(0) Then
                    info = -2
                    Return
                End If
                e(i - 1) = System.Math.sqrt(beta(i))
            Next

            '
            ' Caclulate Pn(a), Pn+1(a), Pn(b), Pn+1(b)
            '
            beta(0) = 0
            pim1a = 0
            pia = 1
            pim1b = 0
            pib = 1
            For i = 1 To n + 1

                '
                ' Pi(a)
                '
                t = (a - alpha(i - 1)) * pia - beta(i - 1) * pim1a
                pim1a = pia
                pia = t

                '
                ' Pi(b)
                '
                t = (b - alpha(i - 1)) * pib - beta(i - 1) * pim1b
                pim1b = pib
                pib = t
            Next

            '
            ' Calculate alpha'(n+1), beta'(n+1)
            '
            a11 = pia
            a12 = pim1a
            a21 = pib
            a22 = pim1b
            b1 = a * pia
            b2 = b * pib
            If CDbl(System.Math.Abs(a11)) > CDbl(System.Math.Abs(a21)) Then
                a22 = a22 - a12 * a21 / a11
                b2 = b2 - b1 * a21 / a11
                bet = b2 / a22
                alph = (b1 - bet * a12) / a11
            Else
                a12 = a12 - a22 * a11 / a21
                b1 = b1 - b2 * a11 / a21
                bet = b1 / a12
                alph = (b2 - bet * a22) / a21
            End If
            If CDbl(bet) < CDbl(0) Then
                info = -3
                Return
            End If
            d(n + 1) = alph
            e(n) = System.Math.sqrt(bet)

            '
            ' EVD
            '
            If Not evd.smatrixtdevd(d, e, n + 2, 3, z) Then
                info = -3
                Return
            End If

            '
            ' Generate
            '
            x = New Double(n + 1) {}
            w = New Double(n + 1) {}
            For i = 1 To n + 2
                x(i - 1) = d(i - 1)
                w(i - 1) = mu0 * Math.sqr(z(0, i - 1))
            Next
        End Sub


        '************************************************************************
        '        Computation of nodes and weights for a Gauss-Radau quadrature formula
        '
        '        The algorithm generates the N-point Gauss-Radau  quadrature  formula  with
        '        weight function given by the coefficients alpha and  beta  of a recurrence
        '        which generates a system of orthogonal polynomials.
        '
        '        P-1(x)   =  0
        '        P0(x)    =  1
        '        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)
        '
        '        and zeroth moment Mu0
        '
        '        Mu0 = integral(W(x)dx,a,b)
        '
        '        INPUT PARAMETERS:
        '            Alpha   ?  array[0..N-2], alpha coefficients.
        '            Beta    ?  array[0..N-1], beta coefficients
        '                        Zero-indexed element is not used.
        '                        Beta[I]>0
        '            Mu0     ?  zeroth moment of the weighting function.
        '            A       ?  left boundary of the integration interval.
        '            N       ?  number of nodes of the quadrature formula, N>=2
        '                        (including the left boundary node).
        '
        '        OUTPUT PARAMETERS:
        '            Info    -   error code:
        '                        * -3    internal eigenproblem solver hasn't converged
        '                        * -2    Beta[i]<=0
        '                        * -1    incorrect N was passed
        '                        *  1    OK
        '            X       -   array[0..N-1] - array of quadrature nodes,
        '                        in ascending order.
        '            W       -   array[0..N-1] - array of quadrature weights.
        '
        '
        '          -- ALGLIB --
        '             Copyright 2005-2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gqgenerategaussradaurec(alpha As Double(), beta As Double(), mu0 As Double, a As Double, n As Integer, ByRef info As Integer, _
            ByRef x As Double(), ByRef w As Double())
            Dim i As Integer = 0
            Dim d As Double() = New Double(-1) {}
            Dim e As Double() = New Double(-1) {}
            Dim z As Double(,) = New Double(-1, -1) {}
            Dim polim1 As Double = 0
            Dim poli As Double = 0
            Dim t As Double = 0

            alpha = DirectCast(alpha.Clone(), Double())
            beta = DirectCast(beta.Clone(), Double())
            info = 0
            x = New Double(-1) {}
            w = New Double(-1) {}

            If n < 2 Then
                info = -1
                Return
            End If
            info = 1

            '
            ' Initialize, D[1:N], E[1:N]
            '
            n = n - 1
            d = New Double(n) {}
            e = New Double(n - 1) {}
            For i = 1 To n
                d(i - 1) = alpha(i - 1)
                If CDbl(beta(i)) <= CDbl(0) Then
                    info = -2
                    Return
                End If
                e(i - 1) = System.Math.sqrt(beta(i))
            Next

            '
            ' Caclulate Pn(a), Pn-1(a), and D[N+1]
            '
            beta(0) = 0
            polim1 = 0
            poli = 1
            For i = 1 To n
                t = (a - alpha(i - 1)) * poli - beta(i - 1) * polim1
                polim1 = poli
                poli = t
            Next
            d(n) = a - beta(n) * polim1 / poli

            '
            ' EVD
            '
            If Not evd.smatrixtdevd(d, e, n + 1, 3, z) Then
                info = -3
                Return
            End If

            '
            ' Generate
            '
            x = New Double(n) {}
            w = New Double(n) {}
            For i = 1 To n + 1
                x(i - 1) = d(i - 1)
                w(i - 1) = mu0 * Math.sqr(z(0, i - 1))
            Next
        End Sub


        '************************************************************************
        '        Returns nodes/weights for Gauss-Legendre quadrature on [-1,1] with N
        '        nodes.
        '
        '        INPUT PARAMETERS:
        '            N           -   number of nodes, >=1
        '
        '        OUTPUT PARAMETERS:
        '            Info        -   error code:
        '                            * -4    an  error   was   detected   when  calculating
        '                                    weights/nodes.  N  is  too  large   to  obtain
        '                                    weights/nodes  with  high   enough   accuracy.
        '                                    Try  to   use   multiple   precision  version.
        '                            * -3    internal eigenproblem solver hasn't  converged
        '                            * -1    incorrect N was passed
        '                            * +1    OK
        '            X           -   array[0..N-1] - array of quadrature nodes,
        '                            in ascending order.
        '            W           -   array[0..N-1] - array of quadrature weights.
        '
        '
        '          -- ALGLIB --
        '             Copyright 12.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gqgenerategausslegendre(n As Integer, ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
            Dim alpha As Double() = New Double(-1) {}
            Dim beta As Double() = New Double(-1) {}
            Dim i As Integer = 0

            info = 0
            x = New Double(-1) {}
            w = New Double(-1) {}

            If n < 1 Then
                info = -1
                Return
            End If
            alpha = New Double(n - 1) {}
            beta = New Double(n - 1) {}
            For i = 0 To n - 1
                alpha(i) = 0
            Next
            beta(0) = 2
            For i = 1 To n - 1
                beta(i) = 1 / (4 - 1 / Math.sqr(i))
            Next
            gqgeneraterec(alpha, beta, beta(0), n, info, x, _
                w)

            '
            ' test basic properties to detect errors
            '
            If info > 0 Then
                If CDbl(x(0)) < CDbl(-1) OrElse CDbl(x(n - 1)) > CDbl(1) Then
                    info = -4
                End If
                For i = 0 To n - 2
                    If CDbl(x(i)) >= CDbl(x(i + 1)) Then
                        info = -4
                    End If
                Next
            End If
        End Sub


        '************************************************************************
        '        Returns  nodes/weights  for  Gauss-Jacobi quadrature on [-1,1] with weight
        '        function W(x)=Power(1-x,Alpha)*Power(1+x,Beta).
        '
        '        INPUT PARAMETERS:
        '            N           -   number of nodes, >=1
        '            Alpha       -   power-law coefficient, Alpha>-1
        '            Beta        -   power-law coefficient, Beta>-1
        '
        '        OUTPUT PARAMETERS:
        '            Info        -   error code:
        '                            * -4    an  error  was   detected   when   calculating
        '                                    weights/nodes. Alpha or  Beta  are  too  close
        '                                    to -1 to obtain weights/nodes with high enough
        '                                    accuracy, or, may be, N is too large.  Try  to
        '                                    use multiple precision version.
        '                            * -3    internal eigenproblem solver hasn't converged
        '                            * -1    incorrect N/Alpha/Beta was passed
        '                            * +1    OK
        '            X           -   array[0..N-1] - array of quadrature nodes,
        '                            in ascending order.
        '            W           -   array[0..N-1] - array of quadrature weights.
        '
        '
        '          -- ALGLIB --
        '             Copyright 12.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gqgenerategaussjacobi(n As Integer, alpha As Double, beta As Double, ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
            Dim a As Double() = New Double(-1) {}
            Dim b As Double() = New Double(-1) {}
            Dim alpha2 As Double = 0
            Dim beta2 As Double = 0
            Dim apb As Double = 0
            Dim t As Double = 0
            Dim i As Integer = 0
            Dim s As Double = 0

            info = 0
            x = New Double(-1) {}
            w = New Double(-1) {}

            If (n < 1 OrElse CDbl(alpha) <= CDbl(-1)) OrElse CDbl(beta) <= CDbl(-1) Then
                info = -1
                Return
            End If
            a = New Double(n - 1) {}
            b = New Double(n - 1) {}
            apb = alpha + beta
            a(0) = (beta - alpha) / (apb + 2)
            t = (apb + 1) * System.Math.Log(2) + gammafunc.lngamma(alpha + 1, s) + gammafunc.lngamma(beta + 1, s) - gammafunc.lngamma(apb + 2, s)
            If CDbl(t) > CDbl(System.Math.Log(Math.maxrealnumber)) Then
                info = -4
                Return
            End If
            b(0) = System.Math.Exp(t)
            If n > 1 Then
                alpha2 = Math.sqr(alpha)
                beta2 = Math.sqr(beta)
                a(1) = (beta2 - alpha2) / ((apb + 2) * (apb + 4))
                b(1) = 4 * (alpha + 1) * (beta + 1) / ((apb + 3) * Math.sqr(apb + 2))
                For i = 2 To n - 1
                    a(i) = 0.25 * (beta2 - alpha2) / (i * i * (1 + 0.5 * apb / i) * (1 + 0.5 * (apb + 2) / i))
                    b(i) = 0.25 * (1 + alpha / i) * (1 + beta / i) * (1 + apb / i) / ((1 + 0.5 * (apb + 1) / i) * (1 + 0.5 * (apb - 1) / i) * Math.sqr(1 + 0.5 * apb / i))
                Next
            End If
            gqgeneraterec(a, b, b(0), n, info, x, _
                w)

            '
            ' test basic properties to detect errors
            '
            If info > 0 Then
                If CDbl(x(0)) < CDbl(-1) OrElse CDbl(x(n - 1)) > CDbl(1) Then
                    info = -4
                End If
                For i = 0 To n - 2
                    If CDbl(x(i)) >= CDbl(x(i + 1)) Then
                        info = -4
                    End If
                Next
            End If
        End Sub


        '************************************************************************
        '        Returns  nodes/weights  for  Gauss-Laguerre  quadrature  on  [0,+inf) with
        '        weight function W(x)=Power(x,Alpha)*Exp(-x)
        '
        '        INPUT PARAMETERS:
        '            N           -   number of nodes, >=1
        '            Alpha       -   power-law coefficient, Alpha>-1
        '
        '        OUTPUT PARAMETERS:
        '            Info        -   error code:
        '                            * -4    an  error  was   detected   when   calculating
        '                                    weights/nodes. Alpha is too  close  to  -1  to
        '                                    obtain weights/nodes with high enough accuracy
        '                                    or, may  be,  N  is  too  large.  Try  to  use
        '                                    multiple precision version.
        '                            * -3    internal eigenproblem solver hasn't converged
        '                            * -1    incorrect N/Alpha was passed
        '                            * +1    OK
        '            X           -   array[0..N-1] - array of quadrature nodes,
        '                            in ascending order.
        '            W           -   array[0..N-1] - array of quadrature weights.
        '
        '
        '          -- ALGLIB --
        '             Copyright 12.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gqgenerategausslaguerre(n As Integer, alpha As Double, ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
            Dim a As Double() = New Double(-1) {}
            Dim b As Double() = New Double(-1) {}
            Dim t As Double = 0
            Dim i As Integer = 0
            Dim s As Double = 0

            info = 0
            x = New Double(-1) {}
            w = New Double(-1) {}

            If n < 1 OrElse CDbl(alpha) <= CDbl(-1) Then
                info = -1
                Return
            End If
            a = New Double(n - 1) {}
            b = New Double(n - 1) {}
            a(0) = alpha + 1
            t = gammafunc.lngamma(alpha + 1, s)
            If CDbl(t) >= CDbl(System.Math.Log(Math.maxrealnumber)) Then
                info = -4
                Return
            End If
            b(0) = System.Math.Exp(t)
            If n > 1 Then
                For i = 1 To n - 1
                    a(i) = 2 * i + alpha + 1
                    b(i) = i * (i + alpha)
                Next
            End If
            gqgeneraterec(a, b, b(0), n, info, x, _
                w)

            '
            ' test basic properties to detect errors
            '
            If info > 0 Then
                If CDbl(x(0)) < CDbl(0) Then
                    info = -4
                End If
                For i = 0 To n - 2
                    If CDbl(x(i)) >= CDbl(x(i + 1)) Then
                        info = -4
                    End If
                Next
            End If
        End Sub


        '************************************************************************
        '        Returns  nodes/weights  for  Gauss-Hermite  quadrature on (-inf,+inf) with
        '        weight function W(x)=Exp(-x*x)
        '
        '        INPUT PARAMETERS:
        '            N           -   number of nodes, >=1
        '
        '        OUTPUT PARAMETERS:
        '            Info        -   error code:
        '                            * -4    an  error  was   detected   when   calculating
        '                                    weights/nodes.  May be, N is too large. Try to
        '                                    use multiple precision version.
        '                            * -3    internal eigenproblem solver hasn't converged
        '                            * -1    incorrect N/Alpha was passed
        '                            * +1    OK
        '            X           -   array[0..N-1] - array of quadrature nodes,
        '                            in ascending order.
        '            W           -   array[0..N-1] - array of quadrature weights.
        '
        '
        '          -- ALGLIB --
        '             Copyright 12.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gqgenerategausshermite(n As Integer, ByRef info As Integer, ByRef x As Double(), ByRef w As Double())
            Dim a As Double() = New Double(-1) {}
            Dim b As Double() = New Double(-1) {}
            Dim i As Integer = 0

            info = 0
            x = New Double(-1) {}
            w = New Double(-1) {}

            If n < 1 Then
                info = -1
                Return
            End If
            a = New Double(n - 1) {}
            b = New Double(n - 1) {}
            For i = 0 To n - 1
                a(i) = 0
            Next
            b(0) = System.Math.sqrt(4 * System.Math.Atan(1))
            If n > 1 Then
                For i = 1 To n - 1
                    b(i) = 0.5 * i
                Next
            End If
            gqgeneraterec(a, b, b(0), n, info, x, _
                w)

            '
            ' test basic properties to detect errors
            '
            If info > 0 Then
                For i = 0 To n - 2
                    If CDbl(x(i)) >= CDbl(x(i + 1)) Then
                        info = -4
                    End If
                Next
            End If
        End Sub


    End Class
    Public Class gkq
        '************************************************************************
        '        Computation of nodes and weights of a Gauss-Kronrod quadrature formula
        '
        '        The algorithm generates the N-point Gauss-Kronrod quadrature formula  with
        '        weight  function  given  by  coefficients  alpha  and beta of a recurrence
        '        relation which generates a system of orthogonal polynomials:
        '
        '            P-1(x)   =  0
        '            P0(x)    =  1
        '            Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)
        '
        '        and zero moment Mu0
        '
        '            Mu0 = integral(W(x)dx,a,b)
        '
        '
        '        INPUT PARAMETERS:
        '            Alpha       ?  alpha coefficients, array[0..floor(3*K/2)].
        '            Beta        ?  beta coefficients,  array[0..ceil(3*K/2)].
        '                            Beta[0] is not used and may be arbitrary.
        '                            Beta[I]>0.
        '            Mu0         ?  zeroth moment of the weight function.
        '            N           ?  number of nodes of the Gauss-Kronrod quadrature formula,
        '                            N >= 3,
        '                            N =  2*K+1.
        '
        '        OUTPUT PARAMETERS:
        '            Info        -   error code:
        '                            * -5    no real and positive Gauss-Kronrod formula can
        '                                    be created for such a weight function  with  a
        '                                    given number of nodes.
        '                            * -4    N is too large, task may be ill  conditioned -
        '                                    x[i]=x[i+1] found.
        '                            * -3    internal eigenproblem solver hasn't converged
        '                            * -2    Beta[i]<=0
        '                            * -1    incorrect N was passed
        '                            * +1    OK
        '            X           -   array[0..N-1] - array of quadrature nodes,
        '                            in ascending order.
        '            WKronrod    -   array[0..N-1] - Kronrod weights
        '            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
        '                            corresponding to extended Kronrod nodes).
        '
        '          -- ALGLIB --
        '             Copyright 08.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gkqgeneraterec(alpha As Double(), beta As Double(), mu0 As Double, n As Integer, ByRef info As Integer, ByRef x As Double(), _
            ByRef wkronrod As Double(), ByRef wgauss As Double())
            Dim ta As Double() = New Double(-1) {}
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim t As Double() = New Double(-1) {}
            Dim s As Double() = New Double(-1) {}
            Dim wlen As Integer = 0
            Dim woffs As Integer = 0
            Dim u As Double = 0
            Dim m As Integer = 0
            Dim l As Integer = 0
            Dim k As Integer = 0
            Dim xgtmp As Double() = New Double(-1) {}
            Dim wgtmp As Double() = New Double(-1) {}
            Dim i_ As Integer = 0

            alpha = DirectCast(alpha.Clone(), Double())
            beta = DirectCast(beta.Clone(), Double())
            info = 0
            x = New Double(-1) {}
            wkronrod = New Double(-1) {}
            wgauss = New Double(-1) {}

            If n Mod 2 <> 1 OrElse n < 3 Then
                info = -1
                Return
            End If
            For i = 0 To CInt(System.Math.Truncate(System.Math.Ceiling(CDbl(3 * (n \ 2)) / CDbl(2))))
                If CDbl(beta(i)) <= CDbl(0) Then
                    info = -2
                    Return
                End If
            Next
            info = 1

            '
            ' from external conventions about N/Beta/Mu0 to internal
            '
            n = n \ 2
            beta(0) = mu0

            '
            ' Calculate Gauss nodes/weights, save them for later processing
            '
            gq.gqgeneraterec(alpha, beta, mu0, n, info, xgtmp, _
                wgtmp)
            If info < 0 Then
                Return
            End If

            '
            ' Resize:
            ' * A from 0..floor(3*n/2) to 0..2*n
            ' * B from 0..ceil(3*n/2)  to 0..2*n
            '
            ta = New Double(CInt(System.Math.Truncate(System.Math.Floor(CDbl(3 * n) / CDbl(2))))) {}
            For i_ = 0 To CInt(System.Math.Truncate(System.Math.Floor(CDbl(3 * n) / CDbl(2))))
                ta(i_) = alpha(i_)
            Next
            alpha = New Double(2 * n) {}
            For i_ = 0 To CInt(System.Math.Truncate(System.Math.Floor(CDbl(3 * n) / CDbl(2))))
                alpha(i_) = ta(i_)
            Next
            For i = CInt(System.Math.Truncate(System.Math.Floor(CDbl(3 * n) / CDbl(2)))) + 1 To 2 * n
                alpha(i) = 0
            Next
            ta = New Double(CInt(System.Math.Truncate(System.Math.Ceiling(CDbl(3 * n) / CDbl(2))))) {}
            For i_ = 0 To CInt(System.Math.Truncate(System.Math.Ceiling(CDbl(3 * n) / CDbl(2))))
                ta(i_) = beta(i_)
            Next
            beta = New Double(2 * n) {}
            For i_ = 0 To CInt(System.Math.Truncate(System.Math.Ceiling(CDbl(3 * n) / CDbl(2))))
                beta(i_) = ta(i_)
            Next
            For i = CInt(System.Math.Truncate(System.Math.Ceiling(CDbl(3 * n) / CDbl(2)))) + 1 To 2 * n
                beta(i) = 0
            Next

            '
            ' Initialize T, S
            '
            wlen = 2 + n \ 2
            t = New Double(wlen - 1) {}
            s = New Double(wlen - 1) {}
            ta = New Double(wlen - 1) {}
            woffs = 1
            For i = 0 To wlen - 1
                t(i) = 0
                s(i) = 0
            Next

            '
            ' Algorithm from Dirk P. Laurie, "Calculation of Gauss-Kronrod quadrature rules", 1997.
            '
            t(woffs + 0) = beta(n + 1)
            For m = 0 To n - 2
                u = 0
                For k = (m + 1) \ 2 To 0 Step -1
                    l = m - k
                    u = u + (alpha(k + n + 1) - alpha(l)) * t(woffs + k) + beta(k + n + 1) * s(woffs + k - 1) - beta(l) * s(woffs + k)
                    s(woffs + k) = u
                Next
                For i_ = 0 To wlen - 1
                    ta(i_) = t(i_)
                Next
                For i_ = 0 To wlen - 1
                    t(i_) = s(i_)
                Next
                For i_ = 0 To wlen - 1
                    s(i_) = ta(i_)
                Next
            Next
            For j = n \ 2 To 0 Step -1
                s(woffs + j) = s(woffs + j - 1)
            Next
            For m = n - 1 To 2 * n - 3
                u = 0
                For k = m + 1 - n To (m - 1) \ 2
                    l = m - k
                    j = n - 1 - l
                    u = u - (alpha(k + n + 1) - alpha(l)) * t(woffs + j) - beta(k + n + 1) * s(woffs + j) + beta(l) * s(woffs + j + 1)
                    s(woffs + j) = u
                Next
                If m Mod 2 = 0 Then
                    k = m \ 2
                    alpha(k + n + 1) = alpha(k) + (s(woffs + j) - beta(k + n + 1) * s(woffs + j + 1)) / t(woffs + j + 1)
                Else
                    k = (m + 1) \ 2
                    beta(k + n + 1) = s(woffs + j) / s(woffs + j + 1)
                End If
                For i_ = 0 To wlen - 1
                    ta(i_) = t(i_)
                Next
                For i_ = 0 To wlen - 1
                    t(i_) = s(i_)
                Next
                For i_ = 0 To wlen - 1
                    s(i_) = ta(i_)
                Next
            Next
            alpha(2 * n) = alpha(n - 1) - beta(2 * n) * s(woffs + 0) / t(woffs + 0)

            '
            ' calculation of Kronrod nodes and weights, unpacking of Gauss weights
            '
            gq.gqgeneraterec(alpha, beta, mu0, 2 * n + 1, info, x, _
                wkronrod)
            If info = -2 Then
                info = -5
            End If
            If info < 0 Then
                Return
            End If
            For i = 0 To 2 * n - 1
                If CDbl(x(i)) >= CDbl(x(i + 1)) Then
                    info = -4
                End If
            Next
            If info < 0 Then
                Return
            End If
            wgauss = New Double(2 * n) {}
            For i = 0 To 2 * n
                wgauss(i) = 0
            Next
            For i = 0 To n - 1
                wgauss(2 * i + 1) = wgtmp(i)
            Next
        End Sub


        '************************************************************************
        '        Returns   Gauss   and   Gauss-Kronrod   nodes/weights  for  Gauss-Legendre
        '        quadrature with N points.
        '
        '        GKQLegendreCalc (calculation) or  GKQLegendreTbl  (precomputed  table)  is
        '        used depending on machine precision and number of nodes.
        '
        '        INPUT PARAMETERS:
        '            N           -   number of Kronrod nodes, must be odd number, >=3.
        '
        '        OUTPUT PARAMETERS:
        '            Info        -   error code:
        '                            * -4    an  error   was   detected   when  calculating
        '                                    weights/nodes.  N  is  too  large   to  obtain
        '                                    weights/nodes  with  high   enough   accuracy.
        '                                    Try  to   use   multiple   precision  version.
        '                            * -3    internal eigenproblem solver hasn't converged
        '                            * -1    incorrect N was passed
        '                            * +1    OK
        '            X           -   array[0..N-1] - array of quadrature nodes, ordered in
        '                            ascending order.
        '            WKronrod    -   array[0..N-1] - Kronrod weights
        '            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
        '                            corresponding to extended Kronrod nodes).
        '
        '
        '          -- ALGLIB --
        '             Copyright 12.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gkqgenerategausslegendre(n As Integer, ByRef info As Integer, ByRef x As Double(), ByRef wkronrod As Double(), ByRef wgauss As Double())
            Dim eps As Double = 0

            info = 0
            x = New Double(-1) {}
            wkronrod = New Double(-1) {}
            wgauss = New Double(-1) {}

            If CDbl(Math.machineepsilon) > CDbl(1.0E-32) AndAlso (((((n = 15 OrElse n = 21) OrElse n = 31) OrElse n = 41) OrElse n = 51) OrElse n = 61) Then
                info = 1
                gkqlegendretbl(n, x, wkronrod, wgauss, eps)
            Else
                gkqlegendrecalc(n, info, x, wkronrod, wgauss)
            End If
        End Sub


        '************************************************************************
        '        Returns   Gauss   and   Gauss-Kronrod   nodes/weights   for   Gauss-Jacobi
        '        quadrature on [-1,1] with weight function
        '
        '            W(x)=Power(1-x,Alpha)*Power(1+x,Beta).
        '
        '        INPUT PARAMETERS:
        '            N           -   number of Kronrod nodes, must be odd number, >=3.
        '            Alpha       -   power-law coefficient, Alpha>-1
        '            Beta        -   power-law coefficient, Beta>-1
        '
        '        OUTPUT PARAMETERS:
        '            Info        -   error code:
        '                            * -5    no real and positive Gauss-Kronrod formula can
        '                                    be created for such a weight function  with  a
        '                                    given number of nodes.
        '                            * -4    an  error  was   detected   when   calculating
        '                                    weights/nodes. Alpha or  Beta  are  too  close
        '                                    to -1 to obtain weights/nodes with high enough
        '                                    accuracy, or, may be, N is too large.  Try  to
        '                                    use multiple precision version.
        '                            * -3    internal eigenproblem solver hasn't converged
        '                            * -1    incorrect N was passed
        '                            * +1    OK
        '                            * +2    OK, but quadrature rule have exterior  nodes,
        '                                    x[0]<-1 or x[n-1]>+1
        '            X           -   array[0..N-1] - array of quadrature nodes, ordered in
        '                            ascending order.
        '            WKronrod    -   array[0..N-1] - Kronrod weights
        '            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
        '                            corresponding to extended Kronrod nodes).
        '
        '
        '          -- ALGLIB --
        '             Copyright 12.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gkqgenerategaussjacobi(n As Integer, alpha As Double, beta As Double, ByRef info As Integer, ByRef x As Double(), ByRef wkronrod As Double(), _
            ByRef wgauss As Double())
            Dim clen As Integer = 0
            Dim a As Double() = New Double(-1) {}
            Dim b As Double() = New Double(-1) {}
            Dim alpha2 As Double = 0
            Dim beta2 As Double = 0
            Dim apb As Double = 0
            Dim t As Double = 0
            Dim i As Integer = 0
            Dim s As Double = 0

            info = 0
            x = New Double(-1) {}
            wkronrod = New Double(-1) {}
            wgauss = New Double(-1) {}

            If n Mod 2 <> 1 OrElse n < 3 Then
                info = -1
                Return
            End If
            If CDbl(alpha) <= CDbl(-1) OrElse CDbl(beta) <= CDbl(-1) Then
                info = -1
                Return
            End If
            clen = CInt(System.Math.Truncate(System.Math.Ceiling(CDbl(3 * (n \ 2)) / CDbl(2)))) + 1
            a = New Double(clen - 1) {}
            b = New Double(clen - 1) {}
            For i = 0 To clen - 1
                a(i) = 0
            Next
            apb = alpha + beta
            a(0) = (beta - alpha) / (apb + 2)
            t = (apb + 1) * System.Math.Log(2) + gammafunc.lngamma(alpha + 1, s) + gammafunc.lngamma(beta + 1, s) - gammafunc.lngamma(apb + 2, s)
            If CDbl(t) > CDbl(System.Math.Log(Math.maxrealnumber)) Then
                info = -4
                Return
            End If
            b(0) = System.Math.Exp(t)
            If clen > 1 Then
                alpha2 = Math.sqr(alpha)
                beta2 = Math.sqr(beta)
                a(1) = (beta2 - alpha2) / ((apb + 2) * (apb + 4))
                b(1) = 4 * (alpha + 1) * (beta + 1) / ((apb + 3) * Math.sqr(apb + 2))
                For i = 2 To clen - 1
                    a(i) = 0.25 * (beta2 - alpha2) / (i * i * (1 + 0.5 * apb / i) * (1 + 0.5 * (apb + 2) / i))
                    b(i) = 0.25 * (1 + alpha / i) * (1 + beta / i) * (1 + apb / i) / ((1 + 0.5 * (apb + 1) / i) * (1 + 0.5 * (apb - 1) / i) * Math.sqr(1 + 0.5 * apb / i))
                Next
            End If
            gkqgeneraterec(a, b, b(0), n, info, x, _
                wkronrod, wgauss)

            '
            ' test basic properties to detect errors
            '
            If info > 0 Then
                If CDbl(x(0)) < CDbl(-1) OrElse CDbl(x(n - 1)) > CDbl(1) Then
                    info = 2
                End If
                For i = 0 To n - 2
                    If CDbl(x(i)) >= CDbl(x(i + 1)) Then
                        info = -4
                    End If
                Next
            End If
        End Sub


        '************************************************************************
        '        Returns Gauss and Gauss-Kronrod nodes for quadrature with N points.
        '
        '        Reduction to tridiagonal eigenproblem is used.
        '
        '        INPUT PARAMETERS:
        '            N           -   number of Kronrod nodes, must be odd number, >=3.
        '
        '        OUTPUT PARAMETERS:
        '            Info        -   error code:
        '                            * -4    an  error   was   detected   when  calculating
        '                                    weights/nodes.  N  is  too  large   to  obtain
        '                                    weights/nodes  with  high   enough   accuracy.
        '                                    Try  to   use   multiple   precision  version.
        '                            * -3    internal eigenproblem solver hasn't converged
        '                            * -1    incorrect N was passed
        '                            * +1    OK
        '            X           -   array[0..N-1] - array of quadrature nodes, ordered in
        '                            ascending order.
        '            WKronrod    -   array[0..N-1] - Kronrod weights
        '            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
        '                            corresponding to extended Kronrod nodes).
        '
        '          -- ALGLIB --
        '             Copyright 12.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gkqlegendrecalc(n As Integer, ByRef info As Integer, ByRef x As Double(), ByRef wkronrod As Double(), ByRef wgauss As Double())
            Dim alpha As Double() = New Double(-1) {}
            Dim beta As Double() = New Double(-1) {}
            Dim alen As Integer = 0
            Dim blen As Integer = 0
            Dim mu0 As Double = 0
            Dim k As Integer = 0
            Dim i As Integer = 0

            info = 0
            x = New Double(-1) {}
            wkronrod = New Double(-1) {}
            wgauss = New Double(-1) {}

            If n Mod 2 <> 1 OrElse n < 3 Then
                info = -1
                Return
            End If
            mu0 = 2
            alen = CInt(System.Math.Truncate(System.Math.Floor(CDbl(3 * (n \ 2)) / CDbl(2)))) + 1
            blen = CInt(System.Math.Truncate(System.Math.Ceiling(CDbl(3 * (n \ 2)) / CDbl(2)))) + 1
            alpha = New Double(alen - 1) {}
            beta = New Double(blen - 1) {}
            For k = 0 To alen - 1
                alpha(k) = 0
            Next
            beta(0) = 2
            For k = 1 To blen - 1
                beta(k) = 1 / (4 - 1 / Math.sqr(k))
            Next
            gkqgeneraterec(alpha, beta, mu0, n, info, x, _
                wkronrod, wgauss)

            '
            ' test basic properties to detect errors
            '
            If info > 0 Then
                If CDbl(x(0)) < CDbl(-1) OrElse CDbl(x(n - 1)) > CDbl(1) Then
                    info = -4
                End If
                For i = 0 To n - 2
                    If CDbl(x(i)) >= CDbl(x(i + 1)) Then
                        info = -4
                    End If
                Next
            End If
        End Sub


        '************************************************************************
        '        Returns Gauss and Gauss-Kronrod nodes for quadrature with N  points  using
        '        pre-calculated table. Nodes/weights were  computed  with  accuracy  up  to
        '        1.0E-32 (if MPFR version of ALGLIB is used). In standard double  precision
        '        accuracy reduces to something about 2.0E-16 (depending  on your compiler's
        '        handling of long floating point constants).
        '
        '        INPUT PARAMETERS:
        '            N           -   number of Kronrod nodes.
        '                            N can be 15, 21, 31, 41, 51, 61.
        '
        '        OUTPUT PARAMETERS:
        '            X           -   array[0..N-1] - array of quadrature nodes, ordered in
        '                            ascending order.
        '            WKronrod    -   array[0..N-1] - Kronrod weights
        '            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
        '                            corresponding to extended Kronrod nodes).
        '
        '
        '          -- ALGLIB --
        '             Copyright 12.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub gkqlegendretbl(n As Integer, ByRef x As Double(), ByRef wkronrod As Double(), ByRef wgauss As Double(), ByRef eps As Double)
            Dim i As Integer = 0
            Dim ng As Integer = 0
            Dim p1 As Integer() = New Integer(-1) {}
            Dim p2 As Integer() = New Integer(-1) {}
            Dim tmp As Double = 0

            x = New Double(-1) {}
            wkronrod = New Double(-1) {}
            wgauss = New Double(-1) {}
            eps = 0


            '
            ' these initializers are not really necessary,
            ' but without them compiler complains about uninitialized locals
            '
            ng = 0

            '
            ' Process
            '
            alglib.ap.assert(((((n = 15 OrElse n = 21) OrElse n = 31) OrElse n = 41) OrElse n = 51) OrElse n = 61, "GKQNodesTbl: incorrect N!")
            x = New Double(n - 1) {}
            wkronrod = New Double(n - 1) {}
            wgauss = New Double(n - 1) {}
            For i = 0 To n - 1
                x(i) = 0
                wkronrod(i) = 0
                wgauss(i) = 0
            Next
            eps = System.Math.Max(Math.machineepsilon, 1.0E-32)
            If n = 15 Then
                ng = 4
                wgauss(0) = 0.12948496616887
                wgauss(1) = 0.279705391489277
                wgauss(2) = 0.381830050505119
                wgauss(3) = 0.417959183673469
                x(0) = 0.991455371120813
                x(1) = 0.949107912342758
                x(2) = 0.864864423359769
                x(3) = 0.741531185599394
                x(4) = 0.586087235467691
                x(5) = 0.405845151377397
                x(6) = 0.207784955007898
                x(7) = 0.0
                wkronrod(0) = 0.0229353220105292
                wkronrod(1) = 0.0630920926299786
                wkronrod(2) = 0.10479001032225
                wkronrod(3) = 0.140653259715526
                wkronrod(4) = 0.169004726639268
                wkronrod(5) = 0.190350578064785
                wkronrod(6) = 0.204432940075299
                wkronrod(7) = 0.209482141084728
            End If
            If n = 21 Then
                ng = 5
                wgauss(0) = 0.0666713443086881
                wgauss(1) = 0.149451349150581
                wgauss(2) = 0.219086362515982
                wgauss(3) = 0.269266719309996
                wgauss(4) = 0.295524224714753
                x(0) = 0.995657163025808
                x(1) = 0.973906528517172
                x(2) = 0.930157491355708
                x(3) = 0.865063366688985
                x(4) = 0.780817726586417
                x(5) = 0.679409568299024
                x(6) = 0.562757134668605
                x(7) = 0.433395394129247
                x(8) = 0.29439286270146
                x(9) = 0.148874338981631
                x(10) = 0.0
                wkronrod(0) = 0.0116946388673719
                wkronrod(1) = 0.0325581623079647
                wkronrod(2) = 0.054755896574352
                wkronrod(3) = 0.07503967481092
                wkronrod(4) = 0.0931254545836976
                wkronrod(5) = 0.109387158802298
                wkronrod(6) = 0.123491976262066
                wkronrod(7) = 0.134709217311473
                wkronrod(8) = 0.14277593857706
                wkronrod(9) = 0.147739104901338
                wkronrod(10) = 0.149445554002917
            End If
            If n = 31 Then
                ng = 8
                wgauss(0) = 0.0307532419961173
                wgauss(1) = 0.0703660474881081
                wgauss(2) = 0.107159220467172
                wgauss(3) = 0.139570677926154
                wgauss(4) = 0.166269205816994
                wgauss(5) = 0.186161000015562
                wgauss(6) = 0.198431485327112
                wgauss(7) = 0.202578241925561
                x(0) = 0.998002298693397
                x(1) = 0.987992518020485
                x(2) = 0.967739075679139
                x(3) = 0.937273392400706
                x(4) = 0.897264532344082
                x(5) = 0.848206583410427
                x(6) = 0.790418501442466
                x(7) = 0.72441773136017
                x(8) = 0.650996741297417
                x(9) = 0.570972172608539
                x(10) = 0.48508186364024
                x(11) = 0.394151347077563
                x(12) = 0.299180007153169
                x(13) = 0.201194093997435
                x(14) = 0.101142066918717
                x(15) = 0.0
                wkronrod(0) = 0.00537747987292335
                wkronrod(1) = 0.0150079473293161
                wkronrod(2) = 0.0254608473267153
                wkronrod(3) = 0.0353463607913758
                wkronrod(4) = 0.0445897513247649
                wkronrod(5) = 0.0534815246909281
                wkronrod(6) = 0.0620095678006706
                wkronrod(7) = 0.0698541213187283
                wkronrod(8) = 0.0768496807577204
                wkronrod(9) = 0.083080502823133
                wkronrod(10) = 0.0885644430562118
                wkronrod(11) = 0.0931265981708253
                wkronrod(12) = 0.0966427269836237
                wkronrod(13) = 0.099173598721792
                wkronrod(14) = 0.100769845523876
                wkronrod(15) = 0.101330007014792
            End If
            If n = 41 Then
                ng = 10
                wgauss(0) = 0.0176140071391521
                wgauss(1) = 0.0406014298003869
                wgauss(2) = 0.0626720483341091
                wgauss(3) = 0.0832767415767048
                wgauss(4) = 0.10193011981724
                wgauss(5) = 0.118194531961518
                wgauss(6) = 0.131688638449177
                wgauss(7) = 0.142096109318382
                wgauss(8) = 0.149172986472604
                wgauss(9) = 0.152753387130726
                x(0) = 0.998859031588278
                x(1) = 0.993128599185095
                x(2) = 0.98150787745025
                x(3) = 0.963971927277914
                x(4) = 0.940822633831755
                x(5) = 0.912234428251326
                x(6) = 0.878276811252282
                x(7) = 0.839116971822219
                x(8) = 0.795041428837551
                x(9) = 0.746331906460151
                x(10) = 0.693237656334751
                x(11) = 0.636053680726515
                x(12) = 0.57514044681971
                x(13) = 0.510867001950827
                x(14) = 0.443593175238725
                x(15) = 0.37370608871542
                x(16) = 0.301627868114913
                x(17) = 0.227785851141645
                x(18) = 0.152605465240923
                x(19) = 0.0765265211334973
                x(20) = 0.0
                wkronrod(0) = 0.00307358371852053
                wkronrod(1) = 0.00860026985564294
                wkronrod(2) = 0.0146261692569713
                wkronrod(3) = 0.0203883734612665
                wkronrod(4) = 0.0258821336049512
                wkronrod(5) = 0.0312873067770328
                wkronrod(6) = 0.0366001697582008
                wkronrod(7) = 0.0416688733279737
                wkronrod(8) = 0.0464348218674977
                wkronrod(9) = 0.0509445739237287
                wkronrod(10) = 0.055195105348286
                wkronrod(11) = 0.0591114008806396
                wkronrod(12) = 0.0626532375547812
                wkronrod(13) = 0.0658345971336184
                wkronrod(14) = 0.0686486729285216
                wkronrod(15) = 0.0710544235534441
                wkronrod(16) = 0.0730306903327867
                wkronrod(17) = 0.0745828754004992
                wkronrod(18) = 0.0757044976845567
                wkronrod(19) = 0.0763778676720807
                wkronrod(20) = 0.0766007119179997
            End If
            If n = 51 Then
                ng = 13
                wgauss(0) = 0.0113937985010263
                wgauss(1) = 0.0263549866150321
                wgauss(2) = 0.0409391567013063
                wgauss(3) = 0.0549046959758352
                wgauss(4) = 0.0680383338123569
                wgauss(5) = 0.080140700335001
                wgauss(6) = 0.0910282619829637
                wgauss(7) = 0.100535949067051
                wgauss(8) = 0.108519624474264
                wgauss(9) = 0.114858259145712
                wgauss(10) = 0.119455763535785
                wgauss(11) = 0.12224244299031
                wgauss(12) = 0.123176053726715
                x(0) = 0.99926210499261
                x(1) = 0.995556969790498
                x(2) = 0.988035794534077
                x(3) = 0.976663921459518
                x(4) = 0.961614986425843
                x(5) = 0.942974571228974
                x(6) = 0.920747115281702
                x(7) = 0.894991997878275
                x(8) = 0.865847065293276
                x(9) = 0.833442628760834
                x(10) = 0.7978737979985
                x(11) = 0.759259263037358
                x(12) = 0.717766406813084
                x(13) = 0.673566368473468
                x(14) = 0.626810099010317
                x(15) = 0.577662930241223
                x(16) = 0.526325284334719
                x(17) = 0.473002731445715
                x(18) = 0.417885382193038
                x(19) = 0.361172305809388
                x(20) = 0.303089538931108
                x(21) = 0.243866883720988
                x(22) = 0.183718939421049
                x(23) = 0.12286469261071
                x(24) = 0.0615444830056851
                x(25) = 0.0
                wkronrod(0) = 0.00198738389233032
                wkronrod(1) = 0.00556193213535671
                wkronrod(2) = 0.00947397338617415
                wkronrod(3) = 0.0132362291955717
                wkronrod(4) = 0.0168478177091283
                wkronrod(5) = 0.0204353711458828
                wkronrod(6) = 0.0240099456069532
                wkronrod(7) = 0.0274753175878517
                wkronrod(8) = 0.0307923001673875
                wkronrod(9) = 0.0340021302743293
                wkronrod(10) = 0.0371162714834155
                wkronrod(11) = 0.0400838255040324
                wkronrod(12) = 0.0428728450201701
                wkronrod(13) = 0.0455029130499218
                wkronrod(14) = 0.0479825371388367
                wkronrod(15) = 0.0502776790807157
                wkronrod(16) = 0.0523628858064075
                wkronrod(17) = 0.0542511298885455
                wkronrod(18) = 0.0559508112204123
                wkronrod(19) = 0.0574371163615678
                wkronrod(20) = 0.0586896800223942
                wkronrod(21) = 0.0597203403241741
                wkronrod(22) = 0.0605394553760459
                wkronrod(23) = 0.061128509717053
                wkronrod(24) = 0.0614711898714253
                wkronrod(25) = 0.0615808180678329
            End If
            If n = 61 Then
                ng = 15
                wgauss(0) = 0.00796819249616661
                wgauss(1) = 0.018466468311091
                wgauss(2) = 0.0287847078833234
                wgauss(3) = 0.0387991925696271
                wgauss(4) = 0.0484026728305941
                wgauss(5) = 0.0574931562176191
                wgauss(6) = 0.0659742298821805
                wgauss(7) = 0.0737559747377052
                wgauss(8) = 0.0807558952294202
                wgauss(9) = 0.086899787201083
                wgauss(10) = 0.0921225222377861
                wgauss(11) = 0.0963687371746443
                wgauss(12) = 0.0995934205867953
                wgauss(13) = 0.101762389748405
                wgauss(14) = 0.102852652893559
                x(0) = 0.999484410050491
                x(1) = 0.99689348407465
                x(2) = 0.991630996870405
                x(3) = 0.983668123279747
                x(4) = 0.973116322501126
                x(5) = 0.960021864968308
                x(6) = 0.94437444474856
                x(7) = 0.926200047429274
                x(8) = 0.905573307699908
                x(9) = 0.882560535792053
                x(10) = 0.857205233546061
                x(11) = 0.829565762382768
                x(12) = 0.799727835821839
                x(13) = 0.767777432104826
                x(14) = 0.733790062453227
                x(15) = 0.697850494793316
                x(16) = 0.660061064126627
                x(17) = 0.620526182989243
                x(18) = 0.579345235826362
                x(19) = 0.53662414814202
                x(20) = 0.492480467861779
                x(21) = 0.447033769538089
                x(22) = 0.400401254830394
                x(23) = 0.352704725530878
                x(24) = 0.304073202273625
                x(25) = 0.25463692616789
                x(26) = 0.20452511668231
                x(27) = 0.153869913608584
                x(28) = 0.102806937966737
                x(29) = 0.0514718425553177
                x(30) = 0.0
                wkronrod(0) = 0.00138901369867701
                wkronrod(1) = 0.00389046112709988
                wkronrod(2) = 0.00663070391593129
                wkronrod(3) = 0.00927327965951776
                wkronrod(4) = 0.0118230152534963
                wkronrod(5) = 0.0143697295070458
                wkronrod(6) = 0.0169208891890533
                wkronrod(7) = 0.0194141411939424
                wkronrod(8) = 0.0218280358216092
                wkronrod(9) = 0.0241911620780806
                wkronrod(10) = 0.0265099548823331
                wkronrod(11) = 0.0287540487650413
                wkronrod(12) = 0.0309072575623878
                wkronrod(13) = 0.0329814470574837
                wkronrod(14) = 0.03497933802806
                wkronrod(15) = 0.0368823646518212
                wkronrod(16) = 0.0386789456247276
                wkronrod(17) = 0.040374538951536
                wkronrod(18) = 0.0419698102151642
                wkronrod(19) = 0.0434525397013561
                wkronrod(20) = 0.0448148001331627
                wkronrod(21) = 0.046059238271007
                wkronrod(22) = 0.0471855465692992
                wkronrod(23) = 0.0481858617570871
                wkronrod(24) = 0.0490554345550298
                wkronrod(25) = 0.0497956834270742
                wkronrod(26) = 0.0504059214027823
                wkronrod(27) = 0.0508817958987496
                wkronrod(28) = 0.0512215478492588
                wkronrod(29) = 0.051426128537459
                wkronrod(30) = 0.0514947294294516
            End If

            '
            ' copy nodes
            '
            For i = n - 1 To n \ 2 Step -1
                x(i) = -x(n - 1 - i)
            Next

            '
            ' copy Kronrod weights
            '
            For i = n - 1 To n \ 2 Step -1
                wkronrod(i) = wkronrod(n - 1 - i)
            Next

            '
            ' copy Gauss weights
            '
            For i = ng - 1 To 0 Step -1
                wgauss(n - 2 - 2 * i) = wgauss(i)
                wgauss(1 + 2 * i) = wgauss(i)
            Next
            For i = 0 To n \ 2
                wgauss(2 * i) = 0
            Next

            '
            ' reorder
            '
            tsort.tagsort(x, n, p1, p2)
            For i = 0 To n - 1
                tmp = wkronrod(i)
                wkronrod(i) = wkronrod(p2(i))
                wkronrod(p2(i)) = tmp
                tmp = wgauss(i)
                wgauss(i) = wgauss(p2(i))
                wgauss(p2(i)) = tmp
            Next
        End Sub


    End Class
    Public Class autogk
        '************************************************************************
        '        Integration report:
        '        * TerminationType = completetion code:
        '            * -5    non-convergence of Gauss-Kronrod nodes
        '                    calculation subroutine.
        '            * -1    incorrect parameters were specified
        '            *  1    OK
        '        * Rep.NFEV countains number of function calculations
        '        * Rep.NIntervals contains number of intervals [a,b]
        '          was partitioned into.
        '        ************************************************************************

        Public Class autogkreport
            Inherits apobject
            Public terminationtype As Integer
            Public nfev As Integer
            Public nintervals As Integer
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New autogkreport()
                _result.terminationtype = terminationtype
                _result.nfev = nfev
                _result.nintervals = nintervals
                Return _result
            End Function
        End Class


        Public Class autogkinternalstate
            Inherits apobject
            Public a As Double
            Public b As Double
            Public eps As Double
            Public xwidth As Double
            Public x As Double
            Public f As Double
            Public info As Integer
            Public r As Double
            Public heap As Double(,)
            Public heapsize As Integer
            Public heapwidth As Integer
            Public heapused As Integer
            Public sumerr As Double
            Public sumabs As Double
            Public qn As Double()
            Public wg As Double()
            Public wk As Double()
            Public wr As Double()
            Public n As Integer
            Public rstate As rcommstate
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
                heap = New Double(-1, -1) {}
                qn = New Double(-1) {}
                wg = New Double(-1) {}
                wk = New Double(-1) {}
                wr = New Double(-1) {}
                rstate = New rcommstate()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New autogkinternalstate()
                _result.a = a
                _result.b = b
                _result.eps = eps
                _result.xwidth = xwidth
                _result.x = x
                _result.f = f
                _result.info = info
                _result.r = r
                _result.heap = DirectCast(heap.Clone(), Double(,))
                _result.heapsize = heapsize
                _result.heapwidth = heapwidth
                _result.heapused = heapused
                _result.sumerr = sumerr
                _result.sumabs = sumabs
                _result.qn = DirectCast(qn.Clone(), Double())
                _result.wg = DirectCast(wg.Clone(), Double())
                _result.wk = DirectCast(wk.Clone(), Double())
                _result.wr = DirectCast(wr.Clone(), Double())
                _result.n = n
                _result.rstate = DirectCast(rstate.make_copy(), rcommstate)
                Return _result
            End Function
        End Class


        '************************************************************************
        '        This structure stores state of the integration algorithm.
        '
        '        Although this class has public fields,  they are not intended for external
        '        use. You should use ALGLIB functions to work with this class:
        '        * autogksmooth()/AutoGKSmoothW()/... to create objects
        '        * autogkintegrate() to begin integration
        '        * autogkresults() to get results
        '        ************************************************************************

        Public Class autogkstate
            Inherits apobject
            Public a As Double
            Public b As Double
            Public alpha As Double
            Public beta As Double
            Public xwidth As Double
            Public x As Double
            Public xminusa As Double
            Public bminusx As Double
            Public needf As Boolean
            Public f As Double
            Public wrappermode As Integer
            Public internalstate As autogkinternalstate
            Public rstate As rcommstate
            Public v As Double
            Public terminationtype As Integer
            Public nfev As Integer
            Public nintervals As Integer
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
                internalstate = New autogkinternalstate()
                rstate = New rcommstate()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New autogkstate()
                _result.a = a
                _result.b = b
                _result.alpha = alpha
                _result.beta = beta
                _result.xwidth = xwidth
                _result.x = x
                _result.xminusa = xminusa
                _result.bminusx = bminusx
                _result.needf = needf
                _result.f = f
                _result.wrappermode = wrappermode
                _result.internalstate = DirectCast(internalstate.make_copy(), autogkinternalstate)
                _result.rstate = DirectCast(rstate.make_copy(), rcommstate)
                _result.v = v
                _result.terminationtype = terminationtype
                _result.nfev = nfev
                _result.nintervals = nintervals
                Return _result
            End Function
        End Class




        Public Const maxsubintervals As Integer = 10000


        '************************************************************************
        '        Integration of a smooth function F(x) on a finite interval [a,b].
        '
        '        Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
        '        is calculated with accuracy close to the machine precision.
        '
        '        Algorithm works well only with smooth integrands.  It  may  be  used  with
        '        continuous non-smooth integrands, but with  less  performance.
        '
        '        It should never be used with integrands which have integrable singularities
        '        at lower or upper limits - algorithm may crash. Use AutoGKSingular in such
        '        cases.
        '
        '        INPUT PARAMETERS:
        '            A, B    -   interval boundaries (A<B, A=B or A>B)
        '            
        '        OUTPUT PARAMETERS
        '            State   -   structure which stores algorithm state
        '
        '        SEE ALSO
        '            AutoGKSmoothW, AutoGKSingular, AutoGKResults.
        '            
        '
        '          -- ALGLIB --
        '             Copyright 06.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub autogksmooth(a As Double, b As Double, state As autogkstate)
            alglib.ap.assert(Math.isfinite(a), "AutoGKSmooth: A is not finite!")
            alglib.ap.assert(Math.isfinite(b), "AutoGKSmooth: B is not finite!")
            autogksmoothw(a, b, 0.0, state)
        End Sub


        '************************************************************************
        '        Integration of a smooth function F(x) on a finite interval [a,b].
        '
        '        This subroutine is same as AutoGKSmooth(), but it guarantees that interval
        '        [a,b] is partitioned into subintervals which have width at most XWidth.
        '
        '        Subroutine  can  be  used  when  integrating nearly-constant function with
        '        narrow "bumps" (about XWidth wide). If "bumps" are too narrow, AutoGKSmooth
        '        subroutine can overlook them.
        '
        '        INPUT PARAMETERS:
        '            A, B    -   interval boundaries (A<B, A=B or A>B)
        '
        '        OUTPUT PARAMETERS
        '            State   -   structure which stores algorithm state
        '
        '        SEE ALSO
        '            AutoGKSmooth, AutoGKSingular, AutoGKResults.
        '
        '
        '          -- ALGLIB --
        '             Copyright 06.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub autogksmoothw(a As Double, b As Double, xwidth As Double, state As autogkstate)
            alglib.ap.assert(Math.isfinite(a), "AutoGKSmoothW: A is not finite!")
            alglib.ap.assert(Math.isfinite(b), "AutoGKSmoothW: B is not finite!")
            alglib.ap.assert(Math.isfinite(xwidth), "AutoGKSmoothW: XWidth is not finite!")
            state.wrappermode = 0
            state.a = a
            state.b = b
            state.xwidth = xwidth
            state.needf = False
            state.rstate.ra = New Double(10) {}
            state.rstate.stage = -1
        End Sub


        '************************************************************************
        '        Integration on a finite interval [A,B].
        '        Integrand have integrable singularities at A/B.
        '
        '        F(X) must diverge as "(x-A)^alpha" at A, as "(B-x)^beta" at B,  with known
        '        alpha/beta (alpha>-1, beta>-1).  If alpha/beta  are  not known,  estimates
        '        from below can be used (but these estimates should be greater than -1 too).
        '
        '        One  of  alpha/beta variables (or even both alpha/beta) may be equal to 0,
        '        which means than function F(x) is non-singular at A/B. Anyway (singular at
        '        bounds or not), function F(x) is supposed to be continuous on (A,B).
        '
        '        Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
        '        is calculated with accuracy close to the machine precision.
        '
        '        INPUT PARAMETERS:
        '            A, B    -   interval boundaries (A<B, A=B or A>B)
        '            Alpha   -   power-law coefficient of the F(x) at A,
        '                        Alpha>-1
        '            Beta    -   power-law coefficient of the F(x) at B,
        '                        Beta>-1
        '
        '        OUTPUT PARAMETERS
        '            State   -   structure which stores algorithm state
        '
        '        SEE ALSO
        '            AutoGKSmooth, AutoGKSmoothW, AutoGKResults.
        '
        '
        '          -- ALGLIB --
        '             Copyright 06.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub autogksingular(a As Double, b As Double, alpha As Double, beta As Double, state As autogkstate)
            alglib.ap.assert(Math.isfinite(a), "AutoGKSingular: A is not finite!")
            alglib.ap.assert(Math.isfinite(b), "AutoGKSingular: B is not finite!")
            alglib.ap.assert(Math.isfinite(alpha), "AutoGKSingular: Alpha is not finite!")
            alglib.ap.assert(Math.isfinite(beta), "AutoGKSingular: Beta is not finite!")
            state.wrappermode = 1
            state.a = a
            state.b = b
            state.alpha = alpha
            state.beta = beta
            state.xwidth = 0.0
            state.needf = False
            state.rstate.ra = New Double(10) {}
            state.rstate.stage = -1
        End Sub


        '************************************************************************
        '
        '          -- ALGLIB --
        '             Copyright 07.05.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function autogkiteration(state As autogkstate) As Boolean
            Dim result As New Boolean()
            Dim s As Double = 0
            Dim tmp As Double = 0
            Dim eps As Double = 0
            Dim a As Double = 0
            Dim b As Double = 0
            Dim x As Double = 0
            Dim t As Double = 0
            Dim alpha As Double = 0
            Dim beta As Double = 0
            Dim v1 As Double = 0
            Dim v2 As Double = 0


            '
            ' Reverse communication preparations
            ' I know it looks ugly, but it works the same way
            ' anywhere from C++ to Python.
            '
            ' This code initializes locals by:
            ' * random values determined during code
            '   generation - on first subroutine call
            ' * values from previous call - on subsequent calls
            '
            If state.rstate.stage >= 0 Then
                s = state.rstate.ra(0)
                tmp = state.rstate.ra(1)
                eps = state.rstate.ra(2)
                a = state.rstate.ra(3)
                b = state.rstate.ra(4)
                x = state.rstate.ra(5)
                t = state.rstate.ra(6)
                alpha = state.rstate.ra(7)
                beta = state.rstate.ra(8)
                v1 = state.rstate.ra(9)
                v2 = state.rstate.ra(10)
            Else
                s = -983
                tmp = -989
                eps = -834
                a = 900
                b = -287
                x = 364
                t = 214
                alpha = -338
                beta = -686
                v1 = 912
                v2 = 585
            End If
            If state.rstate.stage = 0 Then
                GoTo lbl_0
            End If
            If state.rstate.stage = 1 Then
                GoTo lbl_1
            End If
            If state.rstate.stage = 2 Then
                GoTo lbl_2
            End If

            '
            ' Routine body
            '
            eps = 0
            a = state.a
            b = state.b
            alpha = state.alpha
            beta = state.beta
            state.terminationtype = -1
            state.nfev = 0
            state.nintervals = 0

            '
            ' smooth function  at a finite interval
            '
            If state.wrappermode <> 0 Then
                GoTo lbl_3
            End If

            '
            ' special case
            '
            If CDbl(a) = CDbl(b) Then
                state.terminationtype = 1
                state.v = 0
                result = False
                Return result
            End If

            '
            ' general case
            '
            autogkinternalprepare(a, b, eps, state.xwidth, state.internalstate)
lbl_5:
            If Not autogkinternaliteration(state.internalstate) Then
                GoTo lbl_6
            End If
            x = state.internalstate.x
            state.x = x
            state.xminusa = x - a
            state.bminusx = b - x
            state.needf = True
            state.rstate.stage = 0
            GoTo lbl_rcomm
lbl_0:
            state.needf = False
            state.nfev = state.nfev + 1
            state.internalstate.f = state.f
            GoTo lbl_5
lbl_6:
            state.v = state.internalstate.r
            state.terminationtype = state.internalstate.info
            state.nintervals = state.internalstate.heapused
            result = False
            Return result
lbl_3:

            '
            ' function with power-law singularities at the ends of a finite interval
            '
            If state.wrappermode <> 1 Then
                GoTo lbl_7
            End If

            '
            ' test coefficients
            '
            If CDbl(alpha) <= CDbl(-1) OrElse CDbl(beta) <= CDbl(-1) Then
                state.terminationtype = -1
                state.v = 0
                result = False
                Return result
            End If

            '
            ' special cases
            '
            If CDbl(a) = CDbl(b) Then
                state.terminationtype = 1
                state.v = 0
                result = False
                Return result
            End If

            '
            ' reduction to general form
            '
            If CDbl(a) < CDbl(b) Then
                s = 1
            Else
                s = -1
                tmp = a
                a = b
                b = tmp
                tmp = alpha
                alpha = beta
                beta = tmp
            End If
            alpha = System.Math.Min(alpha, 0)
            beta = System.Math.Min(beta, 0)

            '
            ' first, integrate left half of [a,b]:
            '     integral(f(x)dx, a, (b+a)/2) =
            '     = 1/(1+alpha) * integral(t^(-alpha/(1+alpha))*f(a+t^(1/(1+alpha)))dt, 0, (0.5*(b-a))^(1+alpha))
            '
            autogkinternalprepare(0, System.Math.Pow(0.5 * (b - a), 1 + alpha), eps, state.xwidth, state.internalstate)
lbl_9:
            If Not autogkinternaliteration(state.internalstate) Then
                GoTo lbl_10
            End If

            '
            ' Fill State.X, State.XMinusA, State.BMinusX.
            ' Latter two are filled correctly even if B<A.
            '
            x = state.internalstate.x
            t = System.Math.Pow(x, 1 / (1 + alpha))
            state.x = a + t
            If CDbl(s) > CDbl(0) Then
                state.xminusa = t
                state.bminusx = b - (a + t)
            Else
                state.xminusa = a + t - b
                state.bminusx = -t
            End If
            state.needf = True
            state.rstate.stage = 1
            GoTo lbl_rcomm
lbl_1:
            state.needf = False
            If CDbl(alpha) <> CDbl(0) Then
                state.internalstate.f = state.f * System.Math.Pow(x, -(alpha / (1 + alpha))) / (1 + alpha)
            Else
                state.internalstate.f = state.f
            End If
            state.nfev = state.nfev + 1
            GoTo lbl_9
lbl_10:
            v1 = state.internalstate.r
            state.nintervals = state.nintervals + state.internalstate.heapused

            '
            ' then, integrate right half of [a,b]:
            '     integral(f(x)dx, (b+a)/2, b) =
            '     = 1/(1+beta) * integral(t^(-beta/(1+beta))*f(b-t^(1/(1+beta)))dt, 0, (0.5*(b-a))^(1+beta))
            '
            autogkinternalprepare(0, System.Math.Pow(0.5 * (b - a), 1 + beta), eps, state.xwidth, state.internalstate)
lbl_11:
            If Not autogkinternaliteration(state.internalstate) Then
                GoTo lbl_12
            End If

            '
            ' Fill State.X, State.XMinusA, State.BMinusX.
            ' Latter two are filled correctly (X-A, B-X) even if B<A.
            '
            x = state.internalstate.x
            t = System.Math.Pow(x, 1 / (1 + beta))
            state.x = b - t
            If CDbl(s) > CDbl(0) Then
                state.xminusa = b - t - a
                state.bminusx = t
            Else
                state.xminusa = -t
                state.bminusx = a - (b - t)
            End If
            state.needf = True
            state.rstate.stage = 2
            GoTo lbl_rcomm
lbl_2:
            state.needf = False
            If CDbl(beta) <> CDbl(0) Then
                state.internalstate.f = state.f * System.Math.Pow(x, -(beta / (1 + beta))) / (1 + beta)
            Else
                state.internalstate.f = state.f
            End If
            state.nfev = state.nfev + 1
            GoTo lbl_11
lbl_12:
            v2 = state.internalstate.r
            state.nintervals = state.nintervals + state.internalstate.heapused

            '
            ' final result
            '
            state.v = s * (v1 + v2)
            state.terminationtype = 1
            result = False
            Return result
lbl_7:
            result = False
            Return result
lbl_rcomm:

            '
            ' Saving state
            '
            result = True
            state.rstate.ra(0) = s
            state.rstate.ra(1) = tmp
            state.rstate.ra(2) = eps
            state.rstate.ra(3) = a
            state.rstate.ra(4) = b
            state.rstate.ra(5) = x
            state.rstate.ra(6) = t
            state.rstate.ra(7) = alpha
            state.rstate.ra(8) = beta
            state.rstate.ra(9) = v1
            state.rstate.ra(10) = v2
            Return result
        End Function


        '************************************************************************
        '        Adaptive integration results
        '
        '        Called after AutoGKIteration returned False.
        '
        '        Input parameters:
        '            State   -   algorithm state (used by AutoGKIteration).
        '
        '        Output parameters:
        '            V       -   integral(f(x)dx,a,b)
        '            Rep     -   optimization report (see AutoGKReport description)
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2007 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub autogkresults(state As autogkstate, ByRef v As Double, rep As autogkreport)
            v = 0

            v = state.v
            rep.terminationtype = state.terminationtype
            rep.nfev = state.nfev
            rep.nintervals = state.nintervals
        End Sub


        '************************************************************************
        '        Internal AutoGK subroutine
        '        eps<0   - error
        '        eps=0   - automatic eps selection
        '
        '        width<0 -   error
        '        width=0 -   no width requirements
        '        ************************************************************************

        Private Shared Sub autogkinternalprepare(a As Double, b As Double, eps As Double, xwidth As Double, state As autogkinternalstate)

            '
            ' Save settings
            '
            state.a = a
            state.b = b
            state.eps = eps
            state.xwidth = xwidth

            '
            ' Prepare RComm structure
            '
            state.rstate.ia = New Integer(3) {}
            state.rstate.ra = New Double(8) {}
            state.rstate.stage = -1
        End Sub


        '************************************************************************
        '        Internal AutoGK subroutine
        '        ************************************************************************

        Private Shared Function autogkinternaliteration(state As autogkinternalstate) As Boolean
            Dim result As New Boolean()
            Dim c1 As Double = 0
            Dim c2 As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim intg As Double = 0
            Dim intk As Double = 0
            Dim inta As Double = 0
            Dim v As Double = 0
            Dim ta As Double = 0
            Dim tb As Double = 0
            Dim ns As Integer = 0
            Dim qeps As Double = 0
            Dim info As Integer = 0


            '
            ' Reverse communication preparations
            ' I know it looks ugly, but it works the same way
            ' anywhere from C++ to Python.
            '
            ' This code initializes locals by:
            ' * random values determined during code
            '   generation - on first subroutine call
            ' * values from previous call - on subsequent calls
            '
            If state.rstate.stage >= 0 Then
                i = state.rstate.ia(0)
                j = state.rstate.ia(1)
                ns = state.rstate.ia(2)
                info = state.rstate.ia(3)
                c1 = state.rstate.ra(0)
                c2 = state.rstate.ra(1)
                intg = state.rstate.ra(2)
                intk = state.rstate.ra(3)
                inta = state.rstate.ra(4)
                v = state.rstate.ra(5)
                ta = state.rstate.ra(6)
                tb = state.rstate.ra(7)
                qeps = state.rstate.ra(8)
            Else
                i = 497
                j = -271
                ns = -581
                info = 745
                c1 = -533
                c2 = -77
                intg = 678
                intk = -293
                inta = 316
                v = 647
                ta = -756
                tb = 830
                qeps = -871
            End If
            If state.rstate.stage = 0 Then
                GoTo lbl_0
            End If
            If state.rstate.stage = 1 Then
                GoTo lbl_1
            End If
            If state.rstate.stage = 2 Then
                GoTo lbl_2
            End If

            '
            ' Routine body
            '

            '
            ' initialize quadratures.
            ' use 15-point Gauss-Kronrod formula.
            '
            state.n = 15
            gkq.gkqgenerategausslegendre(state.n, info, state.qn, state.wk, state.wg)
            If info < 0 Then
                state.info = -5
                state.r = 0
                result = False
                Return result
            End If
            state.wr = New Double(state.n - 1) {}
            For i = 0 To state.n - 1
                If i = 0 Then
                    state.wr(i) = 0.5 * System.Math.Abs(state.qn(1) - state.qn(0))
                    Continue For
                End If
                If i = state.n - 1 Then
                    state.wr(state.n - 1) = 0.5 * System.Math.Abs(state.qn(state.n - 1) - state.qn(state.n - 2))
                    Continue For
                End If
                state.wr(i) = 0.5 * System.Math.Abs(state.qn(i - 1) - state.qn(i + 1))
            Next

            '
            ' special case
            '
            If CDbl(state.a) = CDbl(state.b) Then
                state.info = 1
                state.r = 0
                result = False
                Return result
            End If

            '
            ' test parameters
            '
            If CDbl(state.eps) < CDbl(0) OrElse CDbl(state.xwidth) < CDbl(0) Then
                state.info = -1
                state.r = 0
                result = False
                Return result
            End If
            state.info = 1
            If CDbl(state.eps) = CDbl(0) Then
                state.eps = 100000 * Math.machineepsilon
            End If

            '
            ' First, prepare heap
            ' * column 0   -   absolute error
            ' * column 1   -   integral of a F(x) (calculated using Kronrod extension nodes)
            ' * column 2   -   integral of a |F(x)| (calculated using modified rect. method)
            ' * column 3   -   left boundary of a subinterval
            ' * column 4   -   right boundary of a subinterval
            '
            If CDbl(state.xwidth) <> CDbl(0) Then
                GoTo lbl_3
            End If

            '
            ' no maximum width requirements
            ' start from one big subinterval
            '
            state.heapwidth = 5
            state.heapsize = 1
            state.heapused = 1
            state.heap = New Double(state.heapsize - 1, state.heapwidth - 1) {}
            c1 = 0.5 * (state.b - state.a)
            c2 = 0.5 * (state.b + state.a)
            intg = 0
            intk = 0
            inta = 0
            i = 0
lbl_5:
            If i > state.n - 1 Then
                GoTo lbl_7
            End If

            '
            ' obtain F
            '
            state.x = c1 * state.qn(i) + c2
            state.rstate.stage = 0
            GoTo lbl_rcomm
lbl_0:
            v = state.f

            '
            ' Gauss-Kronrod formula
            '
            intk = intk + v * state.wk(i)
            If i Mod 2 = 1 Then
                intg = intg + v * state.wg(i)
            End If

            '
            ' Integral |F(x)|
            ' Use rectangles method
            '
            inta = inta + System.Math.Abs(v) * state.wr(i)
            i = i + 1
            GoTo lbl_5
lbl_7:
            intk = intk * (state.b - state.a) * 0.5
            intg = intg * (state.b - state.a) * 0.5
            inta = inta * (state.b - state.a) * 0.5
            state.heap(0, 0) = System.Math.Abs(intg - intk)
            state.heap(0, 1) = intk
            state.heap(0, 2) = inta
            state.heap(0, 3) = state.a
            state.heap(0, 4) = state.b
            state.sumerr = state.heap(0, 0)
            state.sumabs = System.Math.Abs(inta)
            GoTo lbl_4
lbl_3:

            '
            ' maximum subinterval should be no more than XWidth.
            ' so we create Ceil((B-A)/XWidth)+1 small subintervals
            '
            ns = CInt(System.Math.Truncate(System.Math.Ceiling(System.Math.Abs(state.b - state.a) / state.xwidth))) + 1
            state.heapsize = ns
            state.heapused = ns
            state.heapwidth = 5
            state.heap = New Double(state.heapsize - 1, state.heapwidth - 1) {}
            state.sumerr = 0
            state.sumabs = 0
            j = 0
lbl_8:
            If j > ns - 1 Then
                GoTo lbl_10
            End If
            ta = state.a + j * (state.b - state.a) / ns
            tb = state.a + (j + 1) * (state.b - state.a) / ns
            c1 = 0.5 * (tb - ta)
            c2 = 0.5 * (tb + ta)
            intg = 0
            intk = 0
            inta = 0
            i = 0
lbl_11:
            If i > state.n - 1 Then
                GoTo lbl_13
            End If

            '
            ' obtain F
            '
            state.x = c1 * state.qn(i) + c2
            state.rstate.stage = 1
            GoTo lbl_rcomm
lbl_1:
            v = state.f

            '
            ' Gauss-Kronrod formula
            '
            intk = intk + v * state.wk(i)
            If i Mod 2 = 1 Then
                intg = intg + v * state.wg(i)
            End If

            '
            ' Integral |F(x)|
            ' Use rectangles method
            '
            inta = inta + System.Math.Abs(v) * state.wr(i)
            i = i + 1
            GoTo lbl_11
lbl_13:
            intk = intk * (tb - ta) * 0.5
            intg = intg * (tb - ta) * 0.5
            inta = inta * (tb - ta) * 0.5
            state.heap(j, 0) = System.Math.Abs(intg - intk)
            state.heap(j, 1) = intk
            state.heap(j, 2) = inta
            state.heap(j, 3) = ta
            state.heap(j, 4) = tb
            state.sumerr = state.sumerr + state.heap(j, 0)
            state.sumabs = state.sumabs + System.Math.Abs(inta)
            j = j + 1
            GoTo lbl_8
lbl_10:
lbl_4:
lbl_14:

            '
            ' method iterations
            '
            If False Then
                GoTo lbl_15
            End If

            '
            ' additional memory if needed
            '
            If state.heapused = state.heapsize Then
                mheapresize(state.heap, state.heapsize, 4 * state.heapsize, state.heapwidth)
            End If

            '
            ' TODO: every 20 iterations recalculate errors/sums
            '
            If CDbl(state.sumerr) <= CDbl(state.eps * state.sumabs) OrElse state.heapused >= maxsubintervals Then
                state.r = 0
                For j = 0 To state.heapused - 1
                    state.r = state.r + state.heap(j, 1)
                Next
                result = False
                Return result
            End If

            '
            ' Exclude interval with maximum absolute error
            '
            mheappop(state.heap, state.heapused, state.heapwidth)
            state.sumerr = state.sumerr - state.heap(state.heapused - 1, 0)
            state.sumabs = state.sumabs - state.heap(state.heapused - 1, 2)

            '
            ' Divide interval, create subintervals
            '
            ta = state.heap(state.heapused - 1, 3)
            tb = state.heap(state.heapused - 1, 4)
            state.heap(state.heapused - 1, 3) = ta
            state.heap(state.heapused - 1, 4) = 0.5 * (ta + tb)
            state.heap(state.heapused, 3) = 0.5 * (ta + tb)
            state.heap(state.heapused, 4) = tb
            j = state.heapused - 1
lbl_16:
            If j > state.heapused Then
                GoTo lbl_18
            End If
            c1 = 0.5 * (state.heap(j, 4) - state.heap(j, 3))
            c2 = 0.5 * (state.heap(j, 4) + state.heap(j, 3))
            intg = 0
            intk = 0
            inta = 0
            i = 0
lbl_19:
            If i > state.n - 1 Then
                GoTo lbl_21
            End If

            '
            ' F(x)
            '
            state.x = c1 * state.qn(i) + c2
            state.rstate.stage = 2
            GoTo lbl_rcomm
lbl_2:
            v = state.f

            '
            ' Gauss-Kronrod formula
            '
            intk = intk + v * state.wk(i)
            If i Mod 2 = 1 Then
                intg = intg + v * state.wg(i)
            End If

            '
            ' Integral |F(x)|
            ' Use rectangles method
            '
            inta = inta + System.Math.Abs(v) * state.wr(i)
            i = i + 1
            GoTo lbl_19
lbl_21:
            intk = intk * (state.heap(j, 4) - state.heap(j, 3)) * 0.5
            intg = intg * (state.heap(j, 4) - state.heap(j, 3)) * 0.5
            inta = inta * (state.heap(j, 4) - state.heap(j, 3)) * 0.5
            state.heap(j, 0) = System.Math.Abs(intg - intk)
            state.heap(j, 1) = intk
            state.heap(j, 2) = inta
            state.sumerr = state.sumerr + state.heap(j, 0)
            state.sumabs = state.sumabs + state.heap(j, 2)
            j = j + 1
            GoTo lbl_16
lbl_18:
            mheappush(state.heap, state.heapused - 1, state.heapwidth)
            mheappush(state.heap, state.heapused, state.heapwidth)
            state.heapused = state.heapused + 1
            GoTo lbl_14
lbl_15:
            result = False
            Return result
lbl_rcomm:

            '
            ' Saving state
            '
            result = True
            state.rstate.ia(0) = i
            state.rstate.ia(1) = j
            state.rstate.ia(2) = ns
            state.rstate.ia(3) = info
            state.rstate.ra(0) = c1
            state.rstate.ra(1) = c2
            state.rstate.ra(2) = intg
            state.rstate.ra(3) = intk
            state.rstate.ra(4) = inta
            state.rstate.ra(5) = v
            state.rstate.ra(6) = ta
            state.rstate.ra(7) = tb
            state.rstate.ra(8) = qeps
            Return result
        End Function


        Private Shared Sub mheappop(ByRef heap As Double(,), heapsize As Integer, heapwidth As Integer)
            Dim i As Integer = 0
            Dim p As Integer = 0
            Dim t As Double = 0
            Dim maxcp As Integer = 0

            If heapsize = 1 Then
                Return
            End If
            For i = 0 To heapwidth - 1
                t = heap(heapsize - 1, i)
                heap(heapsize - 1, i) = heap(0, i)
                heap(0, i) = t
            Next
            p = 0
            While 2 * p + 1 < heapsize - 1
                maxcp = 2 * p + 1
                If 2 * p + 2 < heapsize - 1 Then
                    If CDbl(heap(2 * p + 2, 0)) > CDbl(heap(2 * p + 1, 0)) Then
                        maxcp = 2 * p + 2
                    End If
                End If
                If CDbl(heap(p, 0)) < CDbl(heap(maxcp, 0)) Then
                    For i = 0 To heapwidth - 1
                        t = heap(p, i)
                        heap(p, i) = heap(maxcp, i)
                        heap(maxcp, i) = t
                    Next
                    p = maxcp
                Else
                    Exit While
                End If
            End While
        End Sub


        Private Shared Sub mheappush(ByRef heap As Double(,), heapsize As Integer, heapwidth As Integer)
            Dim i As Integer = 0
            Dim p As Integer = 0
            Dim t As Double = 0
            Dim parent As Integer = 0

            If heapsize = 0 Then
                Return
            End If
            p = heapsize
            While p <> 0
                parent = (p - 1) \ 2
                If CDbl(heap(p, 0)) > CDbl(heap(parent, 0)) Then
                    For i = 0 To heapwidth - 1
                        t = heap(p, i)
                        heap(p, i) = heap(parent, i)
                        heap(parent, i) = t
                    Next
                    p = parent
                Else
                    Exit While
                End If
            End While
        End Sub


        Private Shared Sub mheapresize(ByRef heap As Double(,), ByRef heapsize As Integer, newheapsize As Integer, heapwidth As Integer)
            Dim tmp As Double(,) = New Double(-1, -1) {}
            Dim i As Integer = 0
            Dim i_ As Integer = 0

            tmp = New Double(heapsize - 1, heapwidth - 1) {}
            For i = 0 To heapsize - 1
                For i_ = 0 To heapwidth - 1
                    tmp(i, i_) = heap(i, i_)
                Next
            Next
            heap = New Double(newheapsize - 1, heapwidth - 1) {}
            For i = 0 To heapsize - 1
                For i_ = 0 To heapwidth - 1
                    heap(i, i_) = tmp(i, i_)
                Next
            Next
            heapsize = newheapsize
        End Sub


    End Class
End Class

