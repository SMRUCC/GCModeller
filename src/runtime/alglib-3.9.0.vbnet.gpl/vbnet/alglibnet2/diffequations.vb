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
'
'    ************************************************************************

	Public Class odesolverstate
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property needdy() As Boolean
			Get
				Return _innerobj.needdy
			End Get
			Set
				_innerobj.needdy = value
			End Set
		End Property
		Public ReadOnly Property y() As Double()
			Get
				Return _innerobj.y
			End Get
		End Property
		Public ReadOnly Property dy() As Double()
			Get
				Return _innerobj.dy
			End Get
		End Property
		Public Property x() As Double
			Get
				Return _innerobj.x
			End Get
			Set
				_innerobj.x = value
			End Set
		End Property

		Public Sub New()
			_innerobj = New odesolver.odesolverstate()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New odesolverstate(DirectCast(_innerobj.make_copy(), odesolver.odesolverstate))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As odesolver.odesolverstate
		Public ReadOnly Property innerobj() As odesolver.odesolverstate
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As odesolver.odesolverstate)
			_innerobj = obj
		End Sub
	End Class


	'************************************************************************
'
'    ************************************************************************

	Public Class odesolverreport
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property nfev() As Integer
			Get
				Return _innerobj.nfev
			End Get
			Set
				_innerobj.nfev = value
			End Set
		End Property
		Public Property terminationtype() As Integer
			Get
				Return _innerobj.terminationtype
			End Get
			Set
				_innerobj.terminationtype = value
			End Set
		End Property

		Public Sub New()
			_innerobj = New odesolver.odesolverreport()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New odesolverreport(DirectCast(_innerobj.make_copy(), odesolver.odesolverreport))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As odesolver.odesolverreport
		Public ReadOnly Property innerobj() As odesolver.odesolverreport
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As odesolver.odesolverreport)
			_innerobj = obj
		End Sub
	End Class

	'************************************************************************
'    Cash-Karp adaptive ODE solver.
'
'    This subroutine solves ODE  Y'=f(Y,x)  with  initial  conditions  Y(xs)=Ys
'    (here Y may be single variable or vector of N variables).
'
'    INPUT PARAMETERS:
'        Y       -   initial conditions, array[0..N-1].
'                    contains values of Y[] at X[0]
'        N       -   system size
'        X       -   points at which Y should be tabulated, array[0..M-1]
'                    integrations starts at X[0], ends at X[M-1],  intermediate
'                    values at X[i] are returned too.
'                    SHOULD BE ORDERED BY ASCENDING OR BY DESCENDING!
'        M       -   number of intermediate points + first point + last point:
'                    * M>2 means that you need both Y(X[M-1]) and M-2 values at
'                      intermediate points
'                    * M=2 means that you want just to integrate from  X[0]  to
'                      X[1] and don't interested in intermediate values.
'                    * M=1 means that you don't want to integrate :)
'                      it is degenerate case, but it will be handled correctly.
'                    * M<1 means error
'        Eps     -   tolerance (absolute/relative error on each  step  will  be
'                    less than Eps). When passing:
'                    * Eps>0, it means desired ABSOLUTE error
'                    * Eps<0, it means desired RELATIVE error.  Relative errors
'                      are calculated with respect to maximum values of  Y seen
'                      so far. Be careful to use this criterion  when  starting
'                      from Y[] that are close to zero.
'        H       -   initial  step  lenth,  it  will  be adjusted automatically
'                    after the first  step.  If  H=0,  step  will  be  selected
'                    automatically  (usualy  it  will  be  equal  to  0.001  of
'                    min(x[i]-x[j])).
'
'    OUTPUT PARAMETERS
'        State   -   structure which stores algorithm state between  subsequent
'                    calls of OdeSolverIteration. Used for reverse communication.
'                    This structure should be passed  to the OdeSolverIteration
'                    subroutine.
'
'    SEE ALSO
'        AutoGKSmoothW, AutoGKSingular, AutoGKIteration, AutoGKResults.
'
'
'      -- ALGLIB --
'         Copyright 01.09.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub odesolverrkck(y As Double(), n As Integer, x As Double(), m As Integer, eps As Double, h As Double, _
		ByRef state As odesolverstate)
		state = New odesolverstate()
		odesolver.odesolverrkck(y, n, x, m, eps, h, _
			state.innerobj)
		Return
	End Sub
	Public Shared Sub odesolverrkck(y As Double(), x As Double(), eps As Double, h As Double, ByRef state As odesolverstate)
		Dim n As Integer
		Dim m As Integer

		state = New odesolverstate()
		n = ap.len(y)
		m = ap.len(x)
		odesolver.odesolverrkck(y, n, x, m, eps, h, _
			state.innerobj)

		Return
	End Sub

	'************************************************************************
'    This function provides reverse communication interface
'    Reverse communication interface is not documented or recommended to use.
'    See below for functions which provide better documented API
'    ************************************************************************

	Public Shared Function odesolveriteration(state As odesolverstate) As Boolean

		Dim result As Boolean = odesolver.odesolveriteration(state.innerobj)
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
'         Copyright 01.09.2009 by Bochkanov Sergey
'
'    ************************************************************************

	Public Shared Sub odesolversolve(state As odesolverstate, diff As ndimensional_ode_rp, obj As Object)
		If diff Is Nothing Then
			Throw New alglibexception("ALGLIB: error in 'odesolversolve()' (diff is null)")
		End If
		While alglib.odesolveriteration(state)
			If state.needdy Then
				diff(state.innerobj.y, state.innerobj.x, state.innerobj.dy, obj)
				Continue While
			End If
			Throw New alglibexception("ALGLIB: unexpected error in 'odesolversolve'")
		End While
	End Sub



	'************************************************************************
'    ODE solver results
'
'    Called after OdeSolverIteration returned False.
'
'    INPUT PARAMETERS:
'        State   -   algorithm state (used by OdeSolverIteration).
'
'    OUTPUT PARAMETERS:
'        M       -   number of tabulated values, M>=1
'        XTbl    -   array[0..M-1], values of X
'        YTbl    -   array[0..M-1,0..N-1], values of Y in X[i]
'        Rep     -   solver report:
'                    * Rep.TerminationType completetion code:
'                        * -2    X is not ordered  by  ascending/descending  or
'                                there are non-distinct X[],  i.e.  X[i]=X[i+1]
'                        * -1    incorrect parameters were specified
'                        *  1    task has been solved
'                    * Rep.NFEV contains number of function calculations
'
'      -- ALGLIB --
'         Copyright 01.09.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub odesolverresults(state As odesolverstate, ByRef m As Integer, ByRef xtbl As Double(), ByRef ytbl As Double(,), ByRef rep As odesolverreport)
		m = 0
		xtbl = New Double(-1) {}
		ytbl = New Double(-1, -1) {}
		rep = New odesolverreport()
		odesolver.odesolverresults(state.innerobj, m, xtbl, ytbl, rep.innerobj)
		Return
	End Sub

End Class
Public Partial Class alglib
	Public Class odesolver
		Public Class odesolverstate
			Inherits apobject
			Public n As Integer
			Public m As Integer
			Public xscale As Double
			Public h As Double
			Public eps As Double
			Public fraceps As Boolean
			Public yc As Double()
			Public escale As Double()
			Public xg As Double()
			Public solvertype As Integer
			Public needdy As Boolean
			Public x As Double
			Public y As Double()
			Public dy As Double()
			Public ytbl As Double(,)
			Public repterminationtype As Integer
			Public repnfev As Integer
			Public yn As Double()
			Public yns As Double()
			Public rka As Double()
			Public rkc As Double()
			Public rkcs As Double()
			Public rkb As Double(,)
			Public rkk As Double(,)
			Public rstate As rcommstate
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
				yc = New Double(-1) {}
				escale = New Double(-1) {}
				xg = New Double(-1) {}
				y = New Double(-1) {}
				dy = New Double(-1) {}
				ytbl = New Double(-1, -1) {}
				yn = New Double(-1) {}
				yns = New Double(-1) {}
				rka = New Double(-1) {}
				rkc = New Double(-1) {}
				rkcs = New Double(-1) {}
				rkb = New Double(-1, -1) {}
				rkk = New Double(-1, -1) {}
				rstate = New rcommstate()
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New odesolverstate()
				_result.n = n
				_result.m = m
				_result.xscale = xscale
				_result.h = h
				_result.eps = eps
				_result.fraceps = fraceps
				_result.yc = DirectCast(yc.Clone(), Double())
				_result.escale = DirectCast(escale.Clone(), Double())
				_result.xg = DirectCast(xg.Clone(), Double())
				_result.solvertype = solvertype
				_result.needdy = needdy
				_result.x = x
				_result.y = DirectCast(y.Clone(), Double())
				_result.dy = DirectCast(dy.Clone(), Double())
				_result.ytbl = DirectCast(ytbl.Clone(), Double(,))
				_result.repterminationtype = repterminationtype
				_result.repnfev = repnfev
				_result.yn = DirectCast(yn.Clone(), Double())
				_result.yns = DirectCast(yns.Clone(), Double())
				_result.rka = DirectCast(rka.Clone(), Double())
				_result.rkc = DirectCast(rkc.Clone(), Double())
				_result.rkcs = DirectCast(rkcs.Clone(), Double())
				_result.rkb = DirectCast(rkb.Clone(), Double(,))
				_result.rkk = DirectCast(rkk.Clone(), Double(,))
				_result.rstate = DirectCast(rstate.make_copy(), rcommstate)
				Return _result
			End Function
		End Class


		Public Class odesolverreport
			Inherits apobject
			Public nfev As Integer
			Public terminationtype As Integer
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New odesolverreport()
				_result.nfev = nfev
				_result.terminationtype = terminationtype
				Return _result
			End Function
		End Class




		Public Const odesolvermaxgrow As Double = 3.0
		Public Const odesolvermaxshrink As Double = 10.0


		'************************************************************************
'        Cash-Karp adaptive ODE solver.
'
'        This subroutine solves ODE  Y'=f(Y,x)  with  initial  conditions  Y(xs)=Ys
'        (here Y may be single variable or vector of N variables).
'
'        INPUT PARAMETERS:
'            Y       -   initial conditions, array[0..N-1].
'                        contains values of Y[] at X[0]
'            N       -   system size
'            X       -   points at which Y should be tabulated, array[0..M-1]
'                        integrations starts at X[0], ends at X[M-1],  intermediate
'                        values at X[i] are returned too.
'                        SHOULD BE ORDERED BY ASCENDING OR BY DESCENDING!
'            M       -   number of intermediate points + first point + last point:
'                        * M>2 means that you need both Y(X[M-1]) and M-2 values at
'                          intermediate points
'                        * M=2 means that you want just to integrate from  X[0]  to
'                          X[1] and don't interested in intermediate values.
'                        * M=1 means that you don't want to integrate :)
'                          it is degenerate case, but it will be handled correctly.
'                        * M<1 means error
'            Eps     -   tolerance (absolute/relative error on each  step  will  be
'                        less than Eps). When passing:
'                        * Eps>0, it means desired ABSOLUTE error
'                        * Eps<0, it means desired RELATIVE error.  Relative errors
'                          are calculated with respect to maximum values of  Y seen
'                          so far. Be careful to use this criterion  when  starting
'                          from Y[] that are close to zero.
'            H       -   initial  step  lenth,  it  will  be adjusted automatically
'                        after the first  step.  If  H=0,  step  will  be  selected
'                        automatically  (usualy  it  will  be  equal  to  0.001  of
'                        min(x[i]-x[j])).
'
'        OUTPUT PARAMETERS
'            State   -   structure which stores algorithm state between  subsequent
'                        calls of OdeSolverIteration. Used for reverse communication.
'                        This structure should be passed  to the OdeSolverIteration
'                        subroutine.
'
'        SEE ALSO
'            AutoGKSmoothW, AutoGKSingular, AutoGKIteration, AutoGKResults.
'
'
'          -- ALGLIB --
'             Copyright 01.09.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub odesolverrkck(y As Double(), n As Integer, x As Double(), m As Integer, eps As Double, h As Double, _
			state As odesolverstate)
			alglib.ap.assert(n >= 1, "ODESolverRKCK: N<1!")
			alglib.ap.assert(m >= 1, "ODESolverRKCK: M<1!")
			alglib.ap.assert(alglib.ap.len(y) >= n, "ODESolverRKCK: Length(Y)<N!")
			alglib.ap.assert(alglib.ap.len(x) >= m, "ODESolverRKCK: Length(X)<M!")
			alglib.ap.assert(apserv.isfinitevector(y, n), "ODESolverRKCK: Y contains infinite or NaN values!")
			alglib.ap.assert(apserv.isfinitevector(x, m), "ODESolverRKCK: Y contains infinite or NaN values!")
            alglib.ap.assert(Math.isfinite(eps), "ODESolverRKCK: Eps is not finite!")
            alglib.ap.assert(CDbl(eps) <> CDbl(0), "ODESolverRKCK: Eps is zero!")
            alglib.ap.assert(Math.isfinite(h), "ODESolverRKCK: H is not finite!")
            odesolverinit(0, y, n, x, m, eps, _
                h, state)
        End Sub


        '************************************************************************
        '
        '          -- ALGLIB --
        '             Copyright 01.09.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function odesolveriteration(state As odesolverstate) As Boolean
            Dim result As New Boolean()
            Dim n As Integer = 0
            Dim m As Integer = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim xc As Double = 0
            Dim v As Double = 0
            Dim h As Double = 0
            Dim h2 As Double = 0
            Dim gridpoint As New Boolean()
            Dim err As Double = 0
            Dim maxgrowpow As Double = 0
            Dim klimit As Integer = 0
            Dim i_ As Integer = 0


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
                n = state.rstate.ia(0)
                m = state.rstate.ia(1)
                i = state.rstate.ia(2)
                j = state.rstate.ia(3)
                k = state.rstate.ia(4)
                klimit = state.rstate.ia(5)
                gridpoint = state.rstate.ba(0)
                xc = state.rstate.ra(0)
                v = state.rstate.ra(1)
                h = state.rstate.ra(2)
                h2 = state.rstate.ra(3)
                err = state.rstate.ra(4)
                maxgrowpow = state.rstate.ra(5)
            Else
                n = -983
                m = -989
                i = -834
                j = 900
                k = -287
                klimit = 364
                gridpoint = False
                xc = -338
                v = -686
                h = 912
                h2 = 585
                err = 497
                maxgrowpow = -271
            End If
            If state.rstate.stage = 0 Then
                GoTo lbl_0
            End If

            '
            ' Routine body
            '

            '
            ' prepare
            '
            If state.repterminationtype <> 0 Then
                result = False
                Return result
            End If
            n = state.n
            m = state.m
            h = state.h
            maxgrowpow = System.Math.Pow(odesolvermaxgrow, 5)
            state.repnfev = 0

            '
            ' some preliminary checks for internal errors
            ' after this we assume that H>0 and M>1
            '
            alglib.ap.assert(CDbl(state.h) > CDbl(0), "ODESolver: internal error")
            alglib.ap.assert(m > 1, "ODESolverIteration: internal error")

            '
            ' choose solver
            '
            If state.solvertype <> 0 Then
                GoTo lbl_1
            End If

            '
            ' Cask-Karp solver
            ' Prepare coefficients table.
            ' Check it for errors
            '
            state.rka = New Double(5) {}
            state.rka(0) = 0
            state.rka(1) = CDbl(1) / CDbl(5)
            state.rka(2) = CDbl(3) / CDbl(10)
            state.rka(3) = CDbl(3) / CDbl(5)
            state.rka(4) = 1
            state.rka(5) = CDbl(7) / CDbl(8)
            state.rkb = New Double(5, 4) {}
            state.rkb(1, 0) = CDbl(1) / CDbl(5)
            state.rkb(2, 0) = CDbl(3) / CDbl(40)
            state.rkb(2, 1) = CDbl(9) / CDbl(40)
            state.rkb(3, 0) = CDbl(3) / CDbl(10)
            state.rkb(3, 1) = -(CDbl(9) / CDbl(10))
            state.rkb(3, 2) = CDbl(6) / CDbl(5)
            state.rkb(4, 0) = -(CDbl(11) / CDbl(54))
            state.rkb(4, 1) = CDbl(5) / CDbl(2)
            state.rkb(4, 2) = -(CDbl(70) / CDbl(27))
            state.rkb(4, 3) = CDbl(35) / CDbl(27)
            state.rkb(5, 0) = CDbl(1631) / CDbl(55296)
            state.rkb(5, 1) = CDbl(175) / CDbl(512)
            state.rkb(5, 2) = CDbl(575) / CDbl(13824)
            state.rkb(5, 3) = CDbl(44275) / CDbl(110592)
            state.rkb(5, 4) = CDbl(253) / CDbl(4096)
            state.rkc = New Double(5) {}
            state.rkc(0) = CDbl(37) / CDbl(378)
            state.rkc(1) = 0
            state.rkc(2) = CDbl(250) / CDbl(621)
            state.rkc(3) = CDbl(125) / CDbl(594)
            state.rkc(4) = 0
            state.rkc(5) = CDbl(512) / CDbl(1771)
            state.rkcs = New Double(5) {}
            state.rkcs(0) = CDbl(2825) / CDbl(27648)
            state.rkcs(1) = 0
            state.rkcs(2) = CDbl(18575) / CDbl(48384)
            state.rkcs(3) = CDbl(13525) / CDbl(55296)
            state.rkcs(4) = CDbl(277) / CDbl(14336)
            state.rkcs(5) = CDbl(1) / CDbl(4)
            state.rkk = New Double(5, n - 1) {}

            '
            ' Main cycle consists of two iterations:
            ' * outer where we travel from X[i-1] to X[i]
            ' * inner where we travel inside [X[i-1],X[i]]
            '
            state.ytbl = New Double(m - 1, n - 1) {}
            state.escale = New Double(n - 1) {}
            state.yn = New Double(n - 1) {}
            state.yns = New Double(n - 1) {}
            xc = state.xg(0)
            For i_ = 0 To n - 1
                state.ytbl(0, i_) = state.yc(i_)
            Next
            For j = 0 To n - 1
                state.escale(j) = 0
            Next
            i = 1
lbl_3:
            If i > m - 1 Then
                GoTo lbl_5
            End If
lbl_6:

            '
            ' begin inner iteration
            '
            If False Then
                GoTo lbl_7
            End If

            '
            ' truncate step if needed (beyond right boundary).
            ' determine should we store X or not
            '
            If CDbl(xc + h) >= CDbl(state.xg(i)) Then
                h = state.xg(i) - xc
                gridpoint = True
            Else
                gridpoint = False
            End If

            '
            ' Update error scale maximums
            '
            ' These maximums are initialized by zeros,
            ' then updated every iterations.
            '
            For j = 0 To n - 1
                state.escale(j) = System.Math.Max(state.escale(j), System.Math.Abs(state.yc(j)))
            Next

            '
            ' make one step:
            ' 1. calculate all info needed to do step
            ' 2. update errors scale maximums using values/derivatives
            '    obtained during (1)
            '
            ' Take into account that we use scaling of X to reduce task
            ' to the form where x[0] < x[1] < ... < x[n-1]. So X is
            ' replaced by x=xscale*t, and dy/dx=f(y,x) is replaced
            ' by dy/dt=xscale*f(y,xscale*t).
            '
            For i_ = 0 To n - 1
                state.yn(i_) = state.yc(i_)
            Next
            For i_ = 0 To n - 1
                state.yns(i_) = state.yc(i_)
            Next
            k = 0
lbl_8:
            If k > 5 Then
                GoTo lbl_10
            End If

            '
            ' prepare data for the next update of YN/YNS
            '
            state.x = state.xscale * (xc + state.rka(k) * h)
            For i_ = 0 To n - 1
                state.y(i_) = state.yc(i_)
            Next
            For j = 0 To k - 1
                v = state.rkb(k, j)
                For i_ = 0 To n - 1
                    state.y(i_) = state.y(i_) + v * state.rkk(j, i_)
                Next
            Next
            state.needdy = True
            state.rstate.stage = 0
            GoTo lbl_rcomm
lbl_0:
            state.needdy = False
            state.repnfev = state.repnfev + 1
            v = h * state.xscale
            For i_ = 0 To n - 1
                state.rkk(k, i_) = v * state.dy(i_)
            Next

            '
            ' update YN/YNS
            '
            v = state.rkc(k)
            For i_ = 0 To n - 1
                state.yn(i_) = state.yn(i_) + v * state.rkk(k, i_)
            Next
            v = state.rkcs(k)
            For i_ = 0 To n - 1
                state.yns(i_) = state.yns(i_) + v * state.rkk(k, i_)
            Next
            k = k + 1
            GoTo lbl_8
lbl_10:

            '
            ' estimate error
            '
            err = 0
            For j = 0 To n - 1
                If Not state.fraceps Then

                    '
                    ' absolute error is estimated
                    '
                    err = System.Math.Max(err, System.Math.Abs(state.yn(j) - state.yns(j)))
                Else

                    '
                    ' Relative error is estimated
                    '
                    v = state.escale(j)
                    If CDbl(v) = CDbl(0) Then
                        v = 1
                    End If
                    err = System.Math.Max(err, System.Math.Abs(state.yn(j) - state.yns(j)) / v)
                End If
            Next

            '
            ' calculate new step, restart if necessary
            '
            If CDbl(maxgrowpow * err) <= CDbl(state.eps) Then
                h2 = odesolvermaxgrow * h
            Else
                h2 = h * System.Math.Pow(state.eps / err, 0.2)
            End If
            If CDbl(h2) < CDbl(h / odesolvermaxshrink) Then
                h2 = h / odesolvermaxshrink
            End If
            If CDbl(err) > CDbl(state.eps) Then
                h = h2
                GoTo lbl_6
            End If

            '
            ' advance position
            '
            xc = xc + h
            For i_ = 0 To n - 1
                state.yc(i_) = state.yn(i_)
            Next

            '
            ' update H
            '
            h = h2

            '
            ' break on grid point
            '
            If gridpoint Then
                GoTo lbl_7
            End If
            GoTo lbl_6
lbl_7:

            '
            ' save result
            '
            For i_ = 0 To n - 1
                state.ytbl(i, i_) = state.yc(i_)
            Next
            i = i + 1
            GoTo lbl_3
lbl_5:
            state.repterminationtype = 1
            result = False
            Return result
lbl_1:
            result = False
            Return result
lbl_rcomm:

            '
            ' Saving state
            '
            result = True
            state.rstate.ia(0) = n
            state.rstate.ia(1) = m
            state.rstate.ia(2) = i
            state.rstate.ia(3) = j
            state.rstate.ia(4) = k
            state.rstate.ia(5) = klimit
            state.rstate.ba(0) = gridpoint
            state.rstate.ra(0) = xc
            state.rstate.ra(1) = v
            state.rstate.ra(2) = h
            state.rstate.ra(3) = h2
            state.rstate.ra(4) = err
            state.rstate.ra(5) = maxgrowpow
            Return result
        End Function


        '************************************************************************
        '        ODE solver results
        '
        '        Called after OdeSolverIteration returned False.
        '
        '        INPUT PARAMETERS:
        '            State   -   algorithm state (used by OdeSolverIteration).
        '
        '        OUTPUT PARAMETERS:
        '            M       -   number of tabulated values, M>=1
        '            XTbl    -   array[0..M-1], values of X
        '            YTbl    -   array[0..M-1,0..N-1], values of Y in X[i]
        '            Rep     -   solver report:
        '                        * Rep.TerminationType completetion code:
        '                            * -2    X is not ordered  by  ascending/descending  or
        '                                    there are non-distinct X[],  i.e.  X[i]=X[i+1]
        '                            * -1    incorrect parameters were specified
        '                            *  1    task has been solved
        '                        * Rep.NFEV contains number of function calculations
        '
        '          -- ALGLIB --
        '             Copyright 01.09.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub odesolverresults(state As odesolverstate, ByRef m As Integer, ByRef xtbl As Double(), ByRef ytbl As Double(,), rep As odesolverreport)
            Dim v As Double = 0
            Dim i As Integer = 0
            Dim i_ As Integer = 0

            m = 0
            xtbl = New Double(-1) {}
            ytbl = New Double(-1, -1) {}

            rep.terminationtype = state.repterminationtype
            If rep.terminationtype > 0 Then
                m = state.m
                rep.nfev = state.repnfev
                xtbl = New Double(state.m - 1) {}
                v = state.xscale
                For i_ = 0 To state.m - 1
                    xtbl(i_) = v * state.xg(i_)
                Next
                ytbl = New Double(state.m - 1, state.n - 1) {}
                For i = 0 To state.m - 1
                    For i_ = 0 To state.n - 1
                        ytbl(i, i_) = state.ytbl(i, i_)
                    Next
                Next
            Else
                rep.nfev = 0
            End If
        End Sub


        '************************************************************************
        '        Internal initialization subroutine
        '        ************************************************************************

        Private Shared Sub odesolverinit(solvertype As Integer, y As Double(), n As Integer, x As Double(), m As Integer, eps As Double, _
            h As Double, state As odesolverstate)
            Dim i As Integer = 0
            Dim v As Double = 0
            Dim i_ As Integer = 0


            '
            ' Prepare RComm
            '
            state.rstate.ia = New Integer(5) {}
            state.rstate.ba = New Boolean(0) {}
            state.rstate.ra = New Double(5) {}
            state.rstate.stage = -1
            state.needdy = False

            '
            ' check parameters.
            '
            If (n <= 0 OrElse m < 1) OrElse CDbl(eps) = CDbl(0) Then
                state.repterminationtype = -1
                Return
            End If
            If CDbl(h) < CDbl(0) Then
                h = -h
            End If

            '
            ' quick exit if necessary.
            ' after this block we assume that M>1
            '
            If m = 1 Then
                state.repnfev = 0
                state.repterminationtype = 1
                state.ytbl = New Double(0, n - 1) {}
                For i_ = 0 To n - 1
                    state.ytbl(0, i_) = y(i_)
                Next
                state.xg = New Double(m - 1) {}
                For i_ = 0 To m - 1
                    state.xg(i_) = x(i_)
                Next
                Return
            End If

            '
            ' check again: correct order of X[]
            '
            If CDbl(x(1)) = CDbl(x(0)) Then
                state.repterminationtype = -2
                Return
            End If
            For i = 1 To m - 1
                If (CDbl(x(1)) > CDbl(x(0)) AndAlso CDbl(x(i)) <= CDbl(x(i - 1))) OrElse (CDbl(x(1)) < CDbl(x(0)) AndAlso CDbl(x(i)) >= CDbl(x(i - 1))) Then
                    state.repterminationtype = -2
                    Return
                End If
            Next

            '
            ' auto-select H if necessary
            '
            If CDbl(h) = CDbl(0) Then
                v = System.Math.Abs(x(1) - x(0))
                For i = 2 To m - 1
                    v = System.Math.Min(v, System.Math.Abs(x(i) - x(i - 1)))
                Next
                h = 0.001 * v
            End If

            '
            ' store parameters
            '
            state.n = n
            state.m = m
            state.h = h
            state.eps = System.Math.Abs(eps)
            state.fraceps = CDbl(eps) < CDbl(0)
            state.xg = New Double(m - 1) {}
            For i_ = 0 To m - 1
                state.xg(i_) = x(i_)
            Next
            If CDbl(x(1)) > CDbl(x(0)) Then
                state.xscale = 1
            Else
                state.xscale = -1
                For i_ = 0 To m - 1
                    state.xg(i_) = -1 * state.xg(i_)
                Next
            End If
            state.yc = New Double(n - 1) {}
            For i_ = 0 To n - 1
                state.yc(i_) = y(i_)
            Next
            state.solvertype = solvertype
            state.repterminationtype = 0

            '
            ' Allocate arrays
            '
            state.y = New Double(n - 1) {}
            state.dy = New Double(n - 1) {}
        End Sub


	End Class
End Class

