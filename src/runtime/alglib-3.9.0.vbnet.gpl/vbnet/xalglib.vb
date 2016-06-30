'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' ALGLIB 3.9.0 (source code generated 2014-12-11)
' Copyright (c) Sergey Bochkanov (ALGLIB project).
' 
' >>> SOURCE LICENSE >>>
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation (www.fsf.org); either version 2 of the 
' License, or (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' A copy of the GNU General Public License is available at
' http://www.fsf.org/licensing/licenses
' >>> END OF LICENSE >>>
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''/

Module XAlglib

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'    Callback definitions for optimizers/fitters/solvers.
'    
'    Callbacks for unparameterized (general) functions:
'    * ndimensional_func         calculates f(arg), stores result to func
'    * ndimensional_grad         calculates func = f(arg), 
'                                grad[i] = df(arg)/d(arg[i])
'    * ndimensional_hess         calculates func = f(arg),
'                                grad[i] = df(arg)/d(arg[i]),
'                                hess[i,j] = d2f(arg)/(d(arg[i])*d(arg[j]))
'    
'    Callbacks for systems of functions:
'    * ndimensional_fvec         calculates vector function f(arg),
'                                stores result to fi
'    * ndimensional_jac          calculates f[i] = fi(arg)
'                                jac[i,j] = df[i](arg)/d(arg[j])
'                                
'    Callbacks for  parameterized  functions,  i.e.  for  functions  which 
'    depend on two vectors: P and Q.  Gradient  and Hessian are calculated 
'    with respect to P only.
'    * ndimensional_pfunc        calculates f(p,q),
'                                stores result to func
'    * ndimensional_pgrad        calculates func = f(p,q),
'                                grad[i] = df(p,q)/d(p[i])
'    * ndimensional_phess        calculates func = f(p,q),
'                                grad[i] = df(p,q)/d(p[i]),
'                                hess[i,j] = d2f(p,q)/(d(p[i])*d(p[j]))
'
'    Callbacks for progress reports:
'    * ndimensional_rep          reports current position of optimization algo    
'    
'    Callbacks for ODE solvers:
'    * ndimensional_ode_rp       calculates dy/dx for given y[] and x
'    
'    Callbacks for integrators:
'    * integrator1_func          calculates f(x) for given x
'                                (additional parameters xminusa and bminusx
'                                contain x-a and b-x)
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Delegate Sub ndimensional_func(arg As Double(), ByRef func As Double, obj As Object)
Public Delegate Sub ndimensional_grad(arg As Double(), ByRef func As Double, grad As Double(), obj As Object)
Public Delegate Sub ndimensional_hess(arg As Double(), ByRef func As Double, grad As Double(), hess As Double(,), obj As Object)

Public Delegate Sub ndimensional_fvec(arg As Double(), fi As Double(), obj As Object)
Public Delegate Sub ndimensional_jac(arg As Double(), fi As Double(), jac As Double(,), obj As Object)

Public Delegate Sub ndimensional_pfunc(p As Double(), q As Double(), ByRef func As Double, obj As Object)
Public Delegate Sub ndimensional_pgrad(p As Double(), q As Double(), ByRef func As Double, grad As Double(), obj As Object)
Public Delegate Sub ndimensional_phess(p As Double(), q As Double(), ByRef func As Double, grad As Double(), hess As Double(,), obj As Object)

Public Delegate Sub ndimensional_rep(arg As Double(), func As Double, obj As Object)

Public Delegate Sub ndimensional_ode_rp(y As Double(), x As Double, dy As Double(), obj As Object)

Public Delegate Sub integrator1_func(x As Double, xminusa As Double, bminusx As Double, ByRef f As Double, obj As Object)

'
' ALGLIB exception
'
Public Class AlglibException
    Inherits System.ApplicationException
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
End Class

'
' Change number of worker threads
'
Public Sub setnworkers(ByVal nworkers As Integer)
    alglib.setnworkers(nworkers)
End Sub


    Public Class hqrndstate
        Public csobj As alglib.hqrndstate
    End Class


    Public Sub hqrndrandomize(ByRef state As hqrndstate)
        Try
            state = New hqrndstate()
            alglib.hqrndrandomize(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hqrndseed(ByVal s1 As Integer, ByVal s2 As Integer, ByRef state As hqrndstate)
        Try
            state = New hqrndstate()
            alglib.hqrndseed(s1, s2, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function hqrnduniformr(ByVal state As hqrndstate) As Double
        Try
            hqrnduniformr = alglib.hqrnduniformr(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hqrnduniformi(ByVal state As hqrndstate, ByVal n As Integer) As Integer
        Try
            hqrnduniformi = alglib.hqrnduniformi(state.csobj, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hqrndnormal(ByVal state As hqrndstate) As Double
        Try
            hqrndnormal = alglib.hqrndnormal(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub hqrndunit2(ByVal state As hqrndstate, ByRef x As Double, ByRef y As Double)
        Try
            alglib.hqrndunit2(state.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hqrndnormal2(ByVal state As hqrndstate, ByRef x1 As Double, ByRef x2 As Double)
        Try
            alglib.hqrndnormal2(state.csobj, x1, x2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function hqrndexponential(ByVal state As hqrndstate, ByVal lambdav As Double) As Double
        Try
            hqrndexponential = alglib.hqrndexponential(state.csobj, lambdav)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hqrnddiscrete(ByVal state As hqrndstate, ByVal x() As Double, ByVal n As Integer) As Double
        Try
            hqrnddiscrete = alglib.hqrnddiscrete(state.csobj, x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hqrndcontinuous(ByVal state As hqrndstate, ByVal x() As Double, ByVal n As Integer) As Double
        Try
            hqrndcontinuous = alglib.hqrndcontinuous(state.csobj, x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function

    Public Class kdtree
        Public csobj As alglib.kdtree
    End Class
    Public Sub kdtreeserialize(ByVal obj As kdtree, ByRef s_out As String)
        Try
            alglib.kdtreeserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Sub kdtreeunserialize(ByVal s_in As String, ByRef obj As kdtree)
        Try
            alglib.kdtreeunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreebuild(ByVal xy(,) As Double, ByVal n As Integer, ByVal nx As Integer, ByVal ny As Integer, ByVal normtype As Integer, ByRef kdt As kdtree)
        Try
            kdt = New kdtree()
            alglib.kdtreebuild(xy, n, nx, ny, normtype, kdt.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreebuild(ByVal xy(,) As Double, ByVal nx As Integer, ByVal ny As Integer, ByVal normtype As Integer, ByRef kdt As kdtree)
        Try
            kdt = New kdtree()
            alglib.kdtreebuild(xy, nx, ny, normtype, kdt.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreebuildtagged(ByVal xy(,) As Double, ByVal tags() As Integer, ByVal n As Integer, ByVal nx As Integer, ByVal ny As Integer, ByVal normtype As Integer, ByRef kdt As kdtree)
        Try
            kdt = New kdtree()
            alglib.kdtreebuildtagged(xy, tags, n, nx, ny, normtype, kdt.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreebuildtagged(ByVal xy(,) As Double, ByVal tags() As Integer, ByVal nx As Integer, ByVal ny As Integer, ByVal normtype As Integer, ByRef kdt As kdtree)
        Try
            kdt = New kdtree()
            alglib.kdtreebuildtagged(xy, tags, nx, ny, normtype, kdt.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function kdtreequeryknn(ByVal kdt As kdtree, ByVal x() As Double, ByVal k As Integer, ByVal selfmatch As Boolean) As Integer
        Try
            kdtreequeryknn = alglib.kdtreequeryknn(kdt.csobj, x, k, selfmatch)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function kdtreequeryknn(ByVal kdt As kdtree, ByVal x() As Double, ByVal k As Integer) As Integer
        Try
            kdtreequeryknn = alglib.kdtreequeryknn(kdt.csobj, x, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function kdtreequeryrnn(ByVal kdt As kdtree, ByVal x() As Double, ByVal r As Double, ByVal selfmatch As Boolean) As Integer
        Try
            kdtreequeryrnn = alglib.kdtreequeryrnn(kdt.csobj, x, r, selfmatch)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function kdtreequeryrnn(ByVal kdt As kdtree, ByVal x() As Double, ByVal r As Double) As Integer
        Try
            kdtreequeryrnn = alglib.kdtreequeryrnn(kdt.csobj, x, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function kdtreequeryaknn(ByVal kdt As kdtree, ByVal x() As Double, ByVal k As Integer, ByVal selfmatch As Boolean, ByVal eps As Double) As Integer
        Try
            kdtreequeryaknn = alglib.kdtreequeryaknn(kdt.csobj, x, k, selfmatch, eps)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function kdtreequeryaknn(ByVal kdt As kdtree, ByVal x() As Double, ByVal k As Integer, ByVal eps As Double) As Integer
        Try
            kdtreequeryaknn = alglib.kdtreequeryaknn(kdt.csobj, x, k, eps)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub kdtreequeryresultsx(ByVal kdt As kdtree, ByRef x(,) As Double)
        Try
            alglib.kdtreequeryresultsx(kdt.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsxy(ByVal kdt As kdtree, ByRef xy(,) As Double)
        Try
            alglib.kdtreequeryresultsxy(kdt.csobj, xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultstags(ByVal kdt As kdtree, ByRef tags() As Integer)
        Try
            alglib.kdtreequeryresultstags(kdt.csobj, tags)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsdistances(ByVal kdt As kdtree, ByRef r() As Double)
        Try
            alglib.kdtreequeryresultsdistances(kdt.csobj, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsxi(ByVal kdt As kdtree, ByRef x(,) As Double)
        Try
            alglib.kdtreequeryresultsxi(kdt.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsxyi(ByVal kdt As kdtree, ByRef xy(,) As Double)
        Try
            alglib.kdtreequeryresultsxyi(kdt.csobj, xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultstagsi(ByVal kdt As kdtree, ByRef tags() As Integer)
        Try
            alglib.kdtreequeryresultstagsi(kdt.csobj, tags)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsdistancesi(ByVal kdt As kdtree, ByRef r() As Double)
        Try
            alglib.kdtreequeryresultsdistancesi(kdt.csobj, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class xdebugrecord1
        Public Property i() As Integer
        Get
            Return csobj.i
        End Get
        Set(ByVal Value As Integer)
            csobj.i = Value
        End Set
        End Property
        Public Property c() As alglib.complex
        Get
            Return csobj.c
        End Get
        Set(ByVal Value As alglib.complex)
            csobj.c = Value
        End Set
        End Property
        Public Property a() As Double()
        Get
            Return csobj.a
        End Get
        Set(ByVal Value As Double())
            csobj.a = Value
        End Set
        End Property
        Public csobj As alglib.xdebugrecord1
    End Class


    Public Sub xdebuginitrecord1(ByRef rec1 As xdebugrecord1)
        Try
            rec1 = New xdebugrecord1()
            alglib.xdebuginitrecord1(rec1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function xdebugb1count(ByVal a() As Boolean) As Integer
        Try
            xdebugb1count = alglib.xdebugb1count(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub xdebugb1not(ByRef a() As Boolean)
        Try
            alglib.xdebugb1not(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugb1appendcopy(ByRef a() As Boolean)
        Try
            alglib.xdebugb1appendcopy(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugb1outeven(ByVal n As Integer, ByRef a() As Boolean)
        Try
            alglib.xdebugb1outeven(n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function xdebugi1sum(ByVal a() As Integer) As Integer
        Try
            xdebugi1sum = alglib.xdebugi1sum(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub xdebugi1neg(ByRef a() As Integer)
        Try
            alglib.xdebugi1neg(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugi1appendcopy(ByRef a() As Integer)
        Try
            alglib.xdebugi1appendcopy(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugi1outeven(ByVal n As Integer, ByRef a() As Integer)
        Try
            alglib.xdebugi1outeven(n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function xdebugr1sum(ByVal a() As Double) As Double
        Try
            xdebugr1sum = alglib.xdebugr1sum(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub xdebugr1neg(ByRef a() As Double)
        Try
            alglib.xdebugr1neg(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugr1appendcopy(ByRef a() As Double)
        Try
            alglib.xdebugr1appendcopy(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugr1outeven(ByVal n As Integer, ByRef a() As Double)
        Try
            alglib.xdebugr1outeven(n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function xdebugc1sum(ByVal a() As alglib.complex) As alglib.complex
        Try
            xdebugc1sum = alglib.xdebugc1sum(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub xdebugc1neg(ByRef a() As alglib.complex)
        Try
            alglib.xdebugc1neg(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugc1appendcopy(ByRef a() As alglib.complex)
        Try
            alglib.xdebugc1appendcopy(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugc1outeven(ByVal n As Integer, ByRef a() As alglib.complex)
        Try
            alglib.xdebugc1outeven(n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function xdebugb2count(ByVal a(,) As Boolean) As Integer
        Try
            xdebugb2count = alglib.xdebugb2count(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub xdebugb2not(ByRef a(,) As Boolean)
        Try
            alglib.xdebugb2not(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugb2transpose(ByRef a(,) As Boolean)
        Try
            alglib.xdebugb2transpose(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugb2outsin(ByVal m As Integer, ByVal n As Integer, ByRef a(,) As Boolean)
        Try
            alglib.xdebugb2outsin(m, n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function xdebugi2sum(ByVal a(,) As Integer) As Integer
        Try
            xdebugi2sum = alglib.xdebugi2sum(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub xdebugi2neg(ByRef a(,) As Integer)
        Try
            alglib.xdebugi2neg(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugi2transpose(ByRef a(,) As Integer)
        Try
            alglib.xdebugi2transpose(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugi2outsin(ByVal m As Integer, ByVal n As Integer, ByRef a(,) As Integer)
        Try
            alglib.xdebugi2outsin(m, n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function xdebugr2sum(ByVal a(,) As Double) As Double
        Try
            xdebugr2sum = alglib.xdebugr2sum(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub xdebugr2neg(ByRef a(,) As Double)
        Try
            alglib.xdebugr2neg(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugr2transpose(ByRef a(,) As Double)
        Try
            alglib.xdebugr2transpose(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugr2outsin(ByVal m As Integer, ByVal n As Integer, ByRef a(,) As Double)
        Try
            alglib.xdebugr2outsin(m, n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function xdebugc2sum(ByVal a(,) As alglib.complex) As alglib.complex
        Try
            xdebugc2sum = alglib.xdebugc2sum(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub xdebugc2neg(ByRef a(,) As alglib.complex)
        Try
            alglib.xdebugc2neg(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugc2transpose(ByRef a(,) As alglib.complex)
        Try
            alglib.xdebugc2transpose(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub xdebugc2outsincos(ByVal m As Integer, ByVal n As Integer, ByRef a(,) As alglib.complex)
        Try
            alglib.xdebugc2outsincos(m, n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function xdebugmaskedbiasedproductsum(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As Double, ByVal b(,) As Double, ByVal c(,) As Boolean) As Double
        Try
            xdebugmaskedbiasedproductsum = alglib.xdebugmaskedbiasedproductsum(m, n, a, b, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Sub cmatrixtranspose(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByRef b(,) As alglib.complex, ByVal ib As Integer, ByVal jb As Integer)
        Try
            alglib.cmatrixtranspose(m, n, a, ia, ja, b, ib, jb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixtranspose(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As Double, ByVal ia As Integer, ByVal ja As Integer, ByRef b(,) As Double, ByVal ib As Integer, ByVal jb As Integer)
        Try
            alglib.rmatrixtranspose(m, n, a, ia, ja, b, ib, jb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixenforcesymmetricity(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean)
        Try
            alglib.rmatrixenforcesymmetricity(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixcopy(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByRef b(,) As alglib.complex, ByVal ib As Integer, ByVal jb As Integer)
        Try
            alglib.cmatrixcopy(m, n, a, ia, ja, b, ib, jb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixcopy(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As Double, ByVal ia As Integer, ByVal ja As Integer, ByRef b(,) As Double, ByVal ib As Integer, ByVal jb As Integer)
        Try
            alglib.rmatrixcopy(m, n, a, ia, ja, b, ib, jb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixrank1(ByVal m As Integer, ByVal n As Integer, ByRef a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByRef u() As alglib.complex, ByVal iu As Integer, ByRef v() As alglib.complex, ByVal iv As Integer)
        Try
            alglib.cmatrixrank1(m, n, a, ia, ja, u, iu, v, iv)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixrank1(ByVal m As Integer, ByVal n As Integer, ByRef a(,) As Double, ByVal ia As Integer, ByVal ja As Integer, ByRef u() As Double, ByVal iu As Integer, ByRef v() As Double, ByVal iv As Integer)
        Try
            alglib.rmatrixrank1(m, n, a, ia, ja, u, iu, v, iv)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixmv(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByVal opa As Integer, ByVal x() As alglib.complex, ByVal ix As Integer, ByRef y() As alglib.complex, ByVal iy As Integer)
        Try
            alglib.cmatrixmv(m, n, a, ia, ja, opa, x, ix, y, iy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixmv(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As Double, ByVal ia As Integer, ByVal ja As Integer, ByVal opa As Integer, ByVal x() As Double, ByVal ix As Integer, ByRef y() As Double, ByVal iy As Integer)
        Try
            alglib.rmatrixmv(m, n, a, ia, ja, opa, x, ix, y, iy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixrighttrsm(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As alglib.complex, ByVal i1 As Integer, ByVal j1 As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x(,) As alglib.complex, ByVal i2 As Integer, ByVal j2 As Integer)
        Try
            alglib.cmatrixrighttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixrighttrsm(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As alglib.complex, ByVal i1 As Integer, ByVal j1 As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x(,) As alglib.complex, ByVal i2 As Integer, ByVal j2 As Integer)
        Try
    alglib.smp_cmatrixrighttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixlefttrsm(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As alglib.complex, ByVal i1 As Integer, ByVal j1 As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x(,) As alglib.complex, ByVal i2 As Integer, ByVal j2 As Integer)
        Try
            alglib.cmatrixlefttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixlefttrsm(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As alglib.complex, ByVal i1 As Integer, ByVal j1 As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x(,) As alglib.complex, ByVal i2 As Integer, ByVal j2 As Integer)
        Try
    alglib.smp_cmatrixlefttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixrighttrsm(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As Double, ByVal i1 As Integer, ByVal j1 As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x(,) As Double, ByVal i2 As Integer, ByVal j2 As Integer)
        Try
            alglib.rmatrixrighttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixrighttrsm(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As Double, ByVal i1 As Integer, ByVal j1 As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x(,) As Double, ByVal i2 As Integer, ByVal j2 As Integer)
        Try
    alglib.smp_rmatrixrighttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixlefttrsm(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As Double, ByVal i1 As Integer, ByVal j1 As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x(,) As Double, ByVal i2 As Integer, ByVal j2 As Integer)
        Try
            alglib.rmatrixlefttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixlefttrsm(ByVal m As Integer, ByVal n As Integer, ByVal a(,) As Double, ByVal i1 As Integer, ByVal j1 As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x(,) As Double, ByVal i2 As Integer, ByVal j2 As Integer)
        Try
    alglib.smp_rmatrixlefttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixherk(ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double, ByVal a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal beta As Double, ByRef c(,) As alglib.complex, ByVal ic As Integer, ByVal jc As Integer, ByVal isupper As Boolean)
        Try
            alglib.cmatrixherk(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixherk(ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double, ByVal a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal beta As Double, ByRef c(,) As alglib.complex, ByVal ic As Integer, ByVal jc As Integer, ByVal isupper As Boolean)
        Try
    alglib.smp_cmatrixherk(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixsyrk(ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double, ByVal a(,) As Double, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal beta As Double, ByRef c(,) As Double, ByVal ic As Integer, ByVal jc As Integer, ByVal isupper As Boolean)
        Try
            alglib.rmatrixsyrk(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixsyrk(ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double, ByVal a(,) As Double, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal beta As Double, ByRef c(,) As Double, ByVal ic As Integer, ByVal jc As Integer, ByVal isupper As Boolean)
        Try
    alglib.smp_rmatrixsyrk(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixgemm(ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As alglib.complex, ByVal a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal b(,) As alglib.complex, ByVal ib As Integer, ByVal jb As Integer, ByVal optypeb As Integer, ByVal beta As alglib.complex, ByRef c(,) As alglib.complex, ByVal ic As Integer, ByVal jc As Integer)
        Try
            alglib.cmatrixgemm(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixgemm(ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As alglib.complex, ByVal a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal b(,) As alglib.complex, ByVal ib As Integer, ByVal jb As Integer, ByVal optypeb As Integer, ByVal beta As alglib.complex, ByRef c(,) As alglib.complex, ByVal ic As Integer, ByVal jc As Integer)
        Try
    alglib.smp_cmatrixgemm(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixgemm(ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double, ByVal a(,) As Double, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal b(,) As Double, ByVal ib As Integer, ByVal jb As Integer, ByVal optypeb As Integer, ByVal beta As Double, ByRef c(,) As Double, ByVal ic As Integer, ByVal jc As Integer)
        Try
            alglib.rmatrixgemm(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixgemm(ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double, ByVal a(,) As Double, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal b(,) As Double, ByVal ib As Integer, ByVal jb As Integer, ByVal optypeb As Integer, ByVal beta As Double, ByRef c(,) As Double, ByVal ic As Integer, ByVal jc As Integer)
        Try
    alglib.smp_rmatrixgemm(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixsyrk(ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double, ByVal a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal beta As Double, ByRef c(,) As alglib.complex, ByVal ic As Integer, ByVal jc As Integer, ByVal isupper As Boolean)
        Try
            alglib.cmatrixsyrk(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixsyrk(ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double, ByVal a(,) As alglib.complex, ByVal ia As Integer, ByVal ja As Integer, ByVal optypea As Integer, ByVal beta As Double, ByRef c(,) As alglib.complex, ByVal ic As Integer, ByVal jc As Integer, ByVal isupper As Boolean)
        Try
    alglib.smp_cmatrixsyrk(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub samplemoments(ByVal x() As Double, ByVal n As Integer, ByRef mean As Double, ByRef variance As Double, ByRef skewness As Double, ByRef kurtosis As Double)
        Try
            alglib.samplemoments(x, n, mean, variance, skewness, kurtosis)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub samplemoments(ByVal x() As Double, ByRef mean As Double, ByRef variance As Double, ByRef skewness As Double, ByRef kurtosis As Double)
        Try
            alglib.samplemoments(x, mean, variance, skewness, kurtosis)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function samplemean(ByVal x() As Double, ByVal n As Integer) As Double
        Try
            samplemean = alglib.samplemean(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function samplemean(ByVal x() As Double) As Double
        Try
            samplemean = alglib.samplemean(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function samplevariance(ByVal x() As Double, ByVal n As Integer) As Double
        Try
            samplevariance = alglib.samplevariance(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function samplevariance(ByVal x() As Double) As Double
        Try
            samplevariance = alglib.samplevariance(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sampleskewness(ByVal x() As Double, ByVal n As Integer) As Double
        Try
            sampleskewness = alglib.sampleskewness(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sampleskewness(ByVal x() As Double) As Double
        Try
            sampleskewness = alglib.sampleskewness(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function samplekurtosis(ByVal x() As Double, ByVal n As Integer) As Double
        Try
            samplekurtosis = alglib.samplekurtosis(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function samplekurtosis(ByVal x() As Double) As Double
        Try
            samplekurtosis = alglib.samplekurtosis(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub sampleadev(ByVal x() As Double, ByVal n As Integer, ByRef adev As Double)
        Try
            alglib.sampleadev(x, n, adev)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sampleadev(ByVal x() As Double, ByRef adev As Double)
        Try
            alglib.sampleadev(x, adev)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub samplemedian(ByVal x() As Double, ByVal n As Integer, ByRef median As Double)
        Try
            alglib.samplemedian(x, n, median)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub samplemedian(ByVal x() As Double, ByRef median As Double)
        Try
            alglib.samplemedian(x, median)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub samplepercentile(ByVal x() As Double, ByVal n As Integer, ByVal p As Double, ByRef v As Double)
        Try
            alglib.samplepercentile(x, n, p, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub samplepercentile(ByVal x() As Double, ByVal p As Double, ByRef v As Double)
        Try
            alglib.samplepercentile(x, p, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function cov2(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer) As Double
        Try
            cov2 = alglib.cov2(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cov2(ByVal x() As Double, ByVal y() As Double) As Double
        Try
            cov2 = alglib.cov2(x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function pearsoncorr2(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer) As Double
        Try
            pearsoncorr2 = alglib.pearsoncorr2(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function pearsoncorr2(ByVal x() As Double, ByVal y() As Double) As Double
        Try
            pearsoncorr2 = alglib.pearsoncorr2(x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spearmancorr2(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer) As Double
        Try
            spearmancorr2 = alglib.spearmancorr2(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spearmancorr2(ByVal x() As Double, ByVal y() As Double) As Double
        Try
            spearmancorr2 = alglib.spearmancorr2(x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub covm(ByVal x(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef c(,) As Double)
        Try
            alglib.covm(x, n, m, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_covm(ByVal x(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef c(,) As Double)
        Try
    alglib.smp_covm(x, n, m, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub covm(ByVal x(,) As Double, ByRef c(,) As Double)
        Try
            alglib.covm(x, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_covm(ByVal x(,) As Double, ByRef c(,) As Double)
        Try
    alglib.smp_covm(x, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pearsoncorrm(ByVal x(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef c(,) As Double)
        Try
            alglib.pearsoncorrm(x, n, m, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_pearsoncorrm(ByVal x(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef c(,) As Double)
        Try
    alglib.smp_pearsoncorrm(x, n, m, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pearsoncorrm(ByVal x(,) As Double, ByRef c(,) As Double)
        Try
            alglib.pearsoncorrm(x, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_pearsoncorrm(ByVal x(,) As Double, ByRef c(,) As Double)
        Try
    alglib.smp_pearsoncorrm(x, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spearmancorrm(ByVal x(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef c(,) As Double)
        Try
            alglib.spearmancorrm(x, n, m, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spearmancorrm(ByVal x(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef c(,) As Double)
        Try
    alglib.smp_spearmancorrm(x, n, m, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spearmancorrm(ByVal x(,) As Double, ByRef c(,) As Double)
        Try
            alglib.spearmancorrm(x, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spearmancorrm(ByVal x(,) As Double, ByRef c(,) As Double)
        Try
    alglib.smp_spearmancorrm(x, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub covm2(ByVal x(,) As Double, ByVal y(,) As Double, ByVal n As Integer, ByVal m1 As Integer, ByVal m2 As Integer, ByRef c(,) As Double)
        Try
            alglib.covm2(x, y, n, m1, m2, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_covm2(ByVal x(,) As Double, ByVal y(,) As Double, ByVal n As Integer, ByVal m1 As Integer, ByVal m2 As Integer, ByRef c(,) As Double)
        Try
    alglib.smp_covm2(x, y, n, m1, m2, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub covm2(ByVal x(,) As Double, ByVal y(,) As Double, ByRef c(,) As Double)
        Try
            alglib.covm2(x, y, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_covm2(ByVal x(,) As Double, ByVal y(,) As Double, ByRef c(,) As Double)
        Try
    alglib.smp_covm2(x, y, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pearsoncorrm2(ByVal x(,) As Double, ByVal y(,) As Double, ByVal n As Integer, ByVal m1 As Integer, ByVal m2 As Integer, ByRef c(,) As Double)
        Try
            alglib.pearsoncorrm2(x, y, n, m1, m2, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_pearsoncorrm2(ByVal x(,) As Double, ByVal y(,) As Double, ByVal n As Integer, ByVal m1 As Integer, ByVal m2 As Integer, ByRef c(,) As Double)
        Try
    alglib.smp_pearsoncorrm2(x, y, n, m1, m2, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pearsoncorrm2(ByVal x(,) As Double, ByVal y(,) As Double, ByRef c(,) As Double)
        Try
            alglib.pearsoncorrm2(x, y, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_pearsoncorrm2(ByVal x(,) As Double, ByVal y(,) As Double, ByRef c(,) As Double)
        Try
    alglib.smp_pearsoncorrm2(x, y, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spearmancorrm2(ByVal x(,) As Double, ByVal y(,) As Double, ByVal n As Integer, ByVal m1 As Integer, ByVal m2 As Integer, ByRef c(,) As Double)
        Try
            alglib.spearmancorrm2(x, y, n, m1, m2, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spearmancorrm2(ByVal x(,) As Double, ByVal y(,) As Double, ByVal n As Integer, ByVal m1 As Integer, ByVal m2 As Integer, ByRef c(,) As Double)
        Try
    alglib.smp_spearmancorrm2(x, y, n, m1, m2, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spearmancorrm2(ByVal x(,) As Double, ByVal y(,) As Double, ByRef c(,) As Double)
        Try
            alglib.spearmancorrm2(x, y, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spearmancorrm2(ByVal x(,) As Double, ByVal y(,) As Double, ByRef c(,) As Double)
        Try
    alglib.smp_spearmancorrm2(x, y, c)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rankdata(ByRef xy(,) As Double, ByVal npoints As Integer, ByVal nfeatures As Integer)
        Try
            alglib.rankdata(xy, npoints, nfeatures)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rankdata(ByRef xy(,) As Double, ByVal npoints As Integer, ByVal nfeatures As Integer)
        Try
    alglib.smp_rankdata(xy, npoints, nfeatures)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rankdata(ByRef xy(,) As Double)
        Try
            alglib.rankdata(xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rankdata(ByRef xy(,) As Double)
        Try
    alglib.smp_rankdata(xy)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rankdatacentered(ByRef xy(,) As Double, ByVal npoints As Integer, ByVal nfeatures As Integer)
        Try
            alglib.rankdatacentered(xy, npoints, nfeatures)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rankdatacentered(ByRef xy(,) As Double, ByVal npoints As Integer, ByVal nfeatures As Integer)
        Try
    alglib.smp_rankdatacentered(xy, npoints, nfeatures)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rankdatacentered(ByRef xy(,) As Double)
        Try
            alglib.rankdatacentered(xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rankdatacentered(ByRef xy(,) As Double)
        Try
    alglib.smp_rankdatacentered(xy)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function pearsoncorrelation(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer) As Double
        Try
            pearsoncorrelation = alglib.pearsoncorrelation(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spearmanrankcorrelation(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer) As Double
        Try
            spearmanrankcorrelation = alglib.spearmanrankcorrelation(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Sub dsoptimalsplit2(ByVal a() As Double, ByVal c() As Integer, ByVal n As Integer, ByRef info As Integer, ByRef threshold As Double, ByRef pal As Double, ByRef pbl As Double, ByRef par As Double, ByRef pbr As Double, ByRef cve As Double)
        Try
            alglib.dsoptimalsplit2(a, c, n, info, threshold, pal, pbl, par, pbr, cve)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub dsoptimalsplit2fast(ByRef a() As Double, ByRef c() As Integer, ByRef tiesbuf() As Integer, ByRef cntbuf() As Integer, ByRef bufr() As Double, ByRef bufi() As Integer, ByVal n As Integer, ByVal nc As Integer, ByVal alpha As Double, ByRef info As Integer, ByRef threshold As Double, ByRef rms As Double, ByRef cvrms As Double)
        Try
            alglib.dsoptimalsplit2fast(a, c, tiesbuf, cntbuf, bufr, bufi, n, nc, alpha, info, threshold, rms, cvrms)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class clusterizerstate
        Public csobj As alglib.clusterizerstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure  is used to store results of the agglomerative hierarchical
    'clustering (AHC).
    '
    'Following information is returned:
    '
    '* NPoints contains number of points in the original dataset
    '
    '* Z contains information about merges performed  (see below).  Z  contains
    '  indexes from the original (unsorted) dataset and it can be used when you
    '  need to know what points were merged. However, it is not convenient when
    '  you want to build a dendrograd (see below).
    '
    '* if  you  want  to  build  dendrogram, you  can use Z, but it is not good
    '  option, because Z contains  indexes from  unsorted  dataset.  Dendrogram
    '  built from such dataset is likely to have intersections. So, you have to
    '  reorder you points before building dendrogram.
    '  Permutation which reorders point is returned in P. Another representation
    '  of  merges,  which  is  more  convenient for dendorgram construction, is
    '  returned in PM.
    '
    '* more information on format of Z, P and PM can be found below and in the
    '  examples from ALGLIB Reference Manual.
    '
    'FORMAL DESCRIPTION OF FIELDS:
    '    NPoints         number of points
    '    Z               array[NPoints-1,2],  contains   indexes   of  clusters
    '                    linked in pairs to  form  clustering  tree.  I-th  row
    '                    corresponds to I-th merge:
    '                    * Z[I,0] - index of the first cluster to merge
    '                    * Z[I,1] - index of the second cluster to merge
    '                    * Z[I,0]<Z[I,1]
    '                    * clusters are  numbered  from 0 to 2*NPoints-2,  with
    '                      indexes from 0 to NPoints-1 corresponding to  points
    '                      of the original dataset, and indexes from NPoints to
    '                      2*NPoints-2  correspond  to  clusters  generated  by
    '                      subsequent  merges  (I-th  row  of Z creates cluster
    '                      with index NPoints+I).
    '
    '                    IMPORTANT: indexes in Z[] are indexes in the ORIGINAL,
    '                    unsorted dataset. In addition to  Z algorithm  outputs
    '                    permutation which rearranges points in such  way  that
    '                    subsequent merges are  performed  on  adjacent  points
    '                    (such order is needed if you want to build dendrogram).
    '                    However,  indexes  in  Z  are  related  to   original,
    '                    unrearranged sequence of points.
    '
    '    P               array[NPoints], permutation which reorders points  for
    '                    dendrogram  construction.  P[i] contains  index of the
    '                    position  where  we  should  move  I-th  point  of the
    '                    original dataset in order to apply merges PZ/PM.
    '
    '    PZ              same as Z, but for permutation of points given  by  P.
    '                    The  only  thing  which  changed  are  indexes  of the
    '                    original points; indexes of clusters remained same.
    '
    '    MergeDist       array[NPoints-1], contains distances between  clusters
    '                    being merged (MergeDist[i] correspond to merge  stored
    '                    in Z[i,...]).
    '
    '    PM              array[NPoints-1,6], another representation of  merges,
    '                    which is suited for dendrogram construction. It  deals
    '                    with rearranged points (permutation P is applied)  and
    '                    represents merges in a form which different  from  one
    '                    used by Z.
    '                    For each I from 0 to NPoints-2, I-th row of PM represents
    '                    merge performed on two clusters C0 and C1. Here:
    '                    * C0 contains points with indexes PM[I,0]...PM[I,1]
    '                    * C1 contains points with indexes PM[I,2]...PM[I,3]
    '                    * indexes stored in PM are given for dataset sorted
    '                      according to permutation P
    '                    * PM[I,1]=PM[I,2]-1 (only adjacent clusters are merged)
    '                    * PM[I,0]<=PM[I,1], PM[I,2]<=PM[I,3], i.e. both
    '                      clusters contain at least one point
    '                    * heights of "subdendrograms" corresponding  to  C0/C1
    '                      are stored in PM[I,4]  and  PM[I,5].  Subdendrograms
    '                      corresponding   to   single-point   clusters    have
    '                      height=0. Dendrogram of the merge result has  height
    '                      H=max(H0,H1)+1.
    '
    'NOTE: there is one-to-one correspondence between merges described by Z and
    '      PM. I-th row of Z describes same merge of clusters as I-th row of PM,
    '      with "left" cluster from Z corresponding to the "left" one from PM.
    '
    '  -- ALGLIB --
    '     Copyright 10.07.2012 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class ahcreport
        Public Property npoints() As Integer
        Get
            Return csobj.npoints
        End Get
        Set(ByVal Value As Integer)
            csobj.npoints = Value
        End Set
        End Property
        Public Property p() As Integer()
        Get
            Return csobj.p
        End Get
        Set(ByVal Value As Integer())
            csobj.p = Value
        End Set
        End Property
        Public Property z() As Integer(,)
        Get
            Return csobj.z
        End Get
        Set(ByVal Value As Integer(,))
            csobj.z = Value
        End Set
        End Property
        Public Property pz() As Integer(,)
        Get
            Return csobj.pz
        End Get
        Set(ByVal Value As Integer(,))
            csobj.pz = Value
        End Set
        End Property
        Public Property pm() As Integer(,)
        Get
            Return csobj.pm
        End Get
        Set(ByVal Value As Integer(,))
            csobj.pm = Value
        End Set
        End Property
        Public Property mergedist() As Double()
        Get
            Return csobj.mergedist
        End Get
        Set(ByVal Value As Double())
            csobj.mergedist = Value
        End Set
        End Property
        Public csobj As alglib.ahcreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This  structure   is  used  to  store  results of the k-means++ clustering
    'algorithm.
    '
    'Following information is always returned:
    '* NPoints contains number of points in the original dataset
    '* TerminationType contains completion code, negative on failure, positive
    '  on success
    '* K contains number of clusters
    '
    'For positive TerminationType we return:
    '* NFeatures contains number of variables in the original dataset
    '* C, which contains centers found by algorithm
    '* CIdx, which maps points of the original dataset to clusters
    '
    'FORMAL DESCRIPTION OF FIELDS:
    '    NPoints         number of points, >=0
    '    NFeatures       number of variables, >=1
    '    TerminationType completion code:
    '                    * -5 if  distance  type  is  anything  different  from
    '                         Euclidean metric
    '                    * -3 for degenerate dataset: a) less  than  K  distinct
    '                         points, b) K=0 for non-empty dataset.
    '                    * +1 for successful completion
    '    K               number of clusters
    '    C               array[K,NFeatures], rows of the array store centers
    '    CIdx            array[NPoints], which contains cluster indexes
    '
    '  -- ALGLIB --
    '     Copyright 27.11.2012 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class kmeansreport
        Public Property npoints() As Integer
        Get
            Return csobj.npoints
        End Get
        Set(ByVal Value As Integer)
            csobj.npoints = Value
        End Set
        End Property
        Public Property nfeatures() As Integer
        Get
            Return csobj.nfeatures
        End Get
        Set(ByVal Value As Integer)
            csobj.nfeatures = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public Property k() As Integer
        Get
            Return csobj.k
        End Get
        Set(ByVal Value As Integer)
            csobj.k = Value
        End Set
        End Property
        Public Property c() As Double(,)
        Get
            Return csobj.c
        End Get
        Set(ByVal Value As Double(,))
            csobj.c = Value
        End Set
        End Property
        Public Property cidx() As Integer()
        Get
            Return csobj.cidx
        End Get
        Set(ByVal Value As Integer())
            csobj.cidx = Value
        End Set
        End Property
        Public csobj As alglib.kmeansreport
    End Class


    Public Sub clusterizercreate(ByRef s As clusterizerstate)
        Try
            s = New clusterizerstate()
            alglib.clusterizercreate(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizersetpoints(ByVal s As clusterizerstate, ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nfeatures As Integer, ByVal disttype As Integer)
        Try
            alglib.clusterizersetpoints(s.csobj, xy, npoints, nfeatures, disttype)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizersetpoints(ByVal s As clusterizerstate, ByVal xy(,) As Double, ByVal disttype As Integer)
        Try
            alglib.clusterizersetpoints(s.csobj, xy, disttype)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizersetdistances(ByVal s As clusterizerstate, ByVal d(,) As Double, ByVal npoints As Integer, ByVal isupper As Boolean)
        Try
            alglib.clusterizersetdistances(s.csobj, d, npoints, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizersetdistances(ByVal s As clusterizerstate, ByVal d(,) As Double, ByVal isupper As Boolean)
        Try
            alglib.clusterizersetdistances(s.csobj, d, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizersetahcalgo(ByVal s As clusterizerstate, ByVal algo As Integer)
        Try
            alglib.clusterizersetahcalgo(s.csobj, algo)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizersetkmeanslimits(ByVal s As clusterizerstate, ByVal restarts As Integer, ByVal maxits As Integer)
        Try
            alglib.clusterizersetkmeanslimits(s.csobj, restarts, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizerrunahc(ByVal s As clusterizerstate, ByRef rep As ahcreport)
        Try
            rep = New ahcreport()
            alglib.clusterizerrunahc(s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_clusterizerrunahc(ByVal s As clusterizerstate, ByRef rep As ahcreport)
        Try
    rep = New ahcreport()
    alglib.smp_clusterizerrunahc(s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizerrunkmeans(ByVal s As clusterizerstate, ByVal k As Integer, ByRef rep As kmeansreport)
        Try
            rep = New kmeansreport()
            alglib.clusterizerrunkmeans(s.csobj, k, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizergetdistances(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nfeatures As Integer, ByVal disttype As Integer, ByRef d(,) As Double)
        Try
            alglib.clusterizergetdistances(xy, npoints, nfeatures, disttype, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_clusterizergetdistances(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nfeatures As Integer, ByVal disttype As Integer, ByRef d(,) As Double)
        Try
    alglib.smp_clusterizergetdistances(xy, npoints, nfeatures, disttype, d)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizergetkclusters(ByVal rep As ahcreport, ByVal k As Integer, ByRef cidx() As Integer, ByRef cz() As Integer)
        Try
            alglib.clusterizergetkclusters(rep.csobj, k, cidx, cz)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizerseparatedbydist(ByVal rep As ahcreport, ByVal r As Double, ByRef k As Integer, ByRef cidx() As Integer, ByRef cz() As Integer)
        Try
            alglib.clusterizerseparatedbydist(rep.csobj, r, k, cidx, cz)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub clusterizerseparatedbycorr(ByVal rep As ahcreport, ByVal r As Double, ByRef k As Integer, ByRef cidx() As Integer, ByRef cz() As Integer)
        Try
            alglib.clusterizerseparatedbycorr(rep.csobj, r, k, cidx, cz)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub kmeansgenerate(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByVal k As Integer, ByVal restarts As Integer, ByRef info As Integer, ByRef c(,) As Double, ByRef xyc() As Integer)
        Try
            alglib.kmeansgenerate(xy, npoints, nvars, k, restarts, info, c, xyc)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class decisionforest
        Public csobj As alglib.decisionforest
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class dfreport
        Public Property relclserror() As Double
        Get
            Return csobj.relclserror
        End Get
        Set(ByVal Value As Double)
            csobj.relclserror = Value
        End Set
        End Property
        Public Property avgce() As Double
        Get
            Return csobj.avgce
        End Get
        Set(ByVal Value As Double)
            csobj.avgce = Value
        End Set
        End Property
        Public Property rmserror() As Double
        Get
            Return csobj.rmserror
        End Get
        Set(ByVal Value As Double)
            csobj.rmserror = Value
        End Set
        End Property
        Public Property avgerror() As Double
        Get
            Return csobj.avgerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgerror = Value
        End Set
        End Property
        Public Property avgrelerror() As Double
        Get
            Return csobj.avgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgrelerror = Value
        End Set
        End Property
        Public Property oobrelclserror() As Double
        Get
            Return csobj.oobrelclserror
        End Get
        Set(ByVal Value As Double)
            csobj.oobrelclserror = Value
        End Set
        End Property
        Public Property oobavgce() As Double
        Get
            Return csobj.oobavgce
        End Get
        Set(ByVal Value As Double)
            csobj.oobavgce = Value
        End Set
        End Property
        Public Property oobrmserror() As Double
        Get
            Return csobj.oobrmserror
        End Get
        Set(ByVal Value As Double)
            csobj.oobrmserror = Value
        End Set
        End Property
        Public Property oobavgerror() As Double
        Get
            Return csobj.oobavgerror
        End Get
        Set(ByVal Value As Double)
            csobj.oobavgerror = Value
        End Set
        End Property
        Public Property oobavgrelerror() As Double
        Get
            Return csobj.oobavgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.oobavgrelerror = Value
        End Set
        End Property
        Public csobj As alglib.dfreport
    End Class
    Public Sub dfserialize(ByVal obj As decisionforest, ByRef s_out As String)
        Try
            alglib.dfserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Sub dfunserialize(ByVal s_in As String, ByRef obj As decisionforest)
        Try
            alglib.dfunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub dfbuildrandomdecisionforest(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByVal nclasses As Integer, ByVal ntrees As Integer, ByVal r As Double, ByRef info As Integer, ByRef df As decisionforest, ByRef rep As dfreport)
        Try
            df = New decisionforest()
            rep = New dfreport()
            alglib.dfbuildrandomdecisionforest(xy, npoints, nvars, nclasses, ntrees, r, info, df.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub dfbuildrandomdecisionforestx1(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByVal nclasses As Integer, ByVal ntrees As Integer, ByVal nrndvars As Integer, ByVal r As Double, ByRef info As Integer, ByRef df As decisionforest, ByRef rep As dfreport)
        Try
            df = New decisionforest()
            rep = New dfreport()
            alglib.dfbuildrandomdecisionforestx1(xy, npoints, nvars, nclasses, ntrees, nrndvars, r, info, df.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub dfprocess(ByVal df As decisionforest, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.dfprocess(df.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub dfprocessi(ByVal df As decisionforest, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.dfprocessi(df.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function dfrelclserror(ByVal df As decisionforest, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            dfrelclserror = alglib.dfrelclserror(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function dfavgce(ByVal df As decisionforest, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            dfavgce = alglib.dfavgce(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function dfrmserror(ByVal df As decisionforest, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            dfrmserror = alglib.dfrmserror(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function dfavgerror(ByVal df As decisionforest, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            dfavgerror = alglib.dfavgerror(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function dfavgrelerror(ByVal df As decisionforest, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            dfavgrelerror = alglib.dfavgrelerror(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function gammafunction(ByVal x As Double) As Double
        Try
            gammafunction = alglib.gammafunction(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function lngamma(ByVal x As Double, ByRef sgngam As Double) As Double
        Try
            lngamma = alglib.lngamma(x, sgngam)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function errorfunction(ByVal x As Double) As Double
        Try
            errorfunction = alglib.errorfunction(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function errorfunctionc(ByVal x As Double) As Double
        Try
            errorfunctionc = alglib.errorfunctionc(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function normaldistribution(ByVal x As Double) As Double
        Try
            normaldistribution = alglib.normaldistribution(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function inverf(ByVal e As Double) As Double
        Try
            inverf = alglib.inverf(e)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function invnormaldistribution(ByVal y0 As Double) As Double
        Try
            invnormaldistribution = alglib.invnormaldistribution(y0)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function incompletegamma(ByVal a As Double, ByVal x As Double) As Double
        Try
            incompletegamma = alglib.incompletegamma(a, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function incompletegammac(ByVal a As Double, ByVal x As Double) As Double
        Try
            incompletegammac = alglib.incompletegammac(a, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function invincompletegammac(ByVal a As Double, ByVal y0 As Double) As Double
        Try
            invincompletegammac = alglib.invincompletegammac(a, y0)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Sub rmatrixqr(ByRef a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef tau() As Double)
        Try
            alglib.rmatrixqr(a, m, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixqr(ByRef a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef tau() As Double)
        Try
    alglib.smp_rmatrixqr(a, m, n, tau)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixlq(ByRef a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef tau() As Double)
        Try
            alglib.rmatrixlq(a, m, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixlq(ByRef a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef tau() As Double)
        Try
    alglib.smp_rmatrixlq(a, m, n, tau)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixqr(ByRef a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByRef tau() As alglib.complex)
        Try
            alglib.cmatrixqr(a, m, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixqr(ByRef a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByRef tau() As alglib.complex)
        Try
    alglib.smp_cmatrixqr(a, m, n, tau)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixlq(ByRef a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByRef tau() As alglib.complex)
        Try
            alglib.cmatrixlq(a, m, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixlq(ByRef a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByRef tau() As alglib.complex)
        Try
    alglib.smp_cmatrixlq(a, m, n, tau)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixqrunpackq(ByVal a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal tau() As Double, ByVal qcolumns As Integer, ByRef q(,) As Double)
        Try
            alglib.rmatrixqrunpackq(a, m, n, tau, qcolumns, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixqrunpackq(ByVal a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal tau() As Double, ByVal qcolumns As Integer, ByRef q(,) As Double)
        Try
    alglib.smp_rmatrixqrunpackq(a, m, n, tau, qcolumns, q)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixqrunpackr(ByVal a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef r(,) As Double)
        Try
            alglib.rmatrixqrunpackr(a, m, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixlqunpackq(ByVal a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal tau() As Double, ByVal qrows As Integer, ByRef q(,) As Double)
        Try
            alglib.rmatrixlqunpackq(a, m, n, tau, qrows, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixlqunpackq(ByVal a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal tau() As Double, ByVal qrows As Integer, ByRef q(,) As Double)
        Try
    alglib.smp_rmatrixlqunpackq(a, m, n, tau, qrows, q)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixlqunpackl(ByVal a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef l(,) As Double)
        Try
            alglib.rmatrixlqunpackl(a, m, n, l)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixqrunpackq(ByVal a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByVal tau() As alglib.complex, ByVal qcolumns As Integer, ByRef q(,) As alglib.complex)
        Try
            alglib.cmatrixqrunpackq(a, m, n, tau, qcolumns, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixqrunpackq(ByVal a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByVal tau() As alglib.complex, ByVal qcolumns As Integer, ByRef q(,) As alglib.complex)
        Try
    alglib.smp_cmatrixqrunpackq(a, m, n, tau, qcolumns, q)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixqrunpackr(ByVal a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByRef r(,) As alglib.complex)
        Try
            alglib.cmatrixqrunpackr(a, m, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixlqunpackq(ByVal a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByVal tau() As alglib.complex, ByVal qrows As Integer, ByRef q(,) As alglib.complex)
        Try
            alglib.cmatrixlqunpackq(a, m, n, tau, qrows, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixlqunpackq(ByVal a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByVal tau() As alglib.complex, ByVal qrows As Integer, ByRef q(,) As alglib.complex)
        Try
    alglib.smp_cmatrixlqunpackq(a, m, n, tau, qrows, q)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixlqunpackl(ByVal a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByRef l(,) As alglib.complex)
        Try
            alglib.cmatrixlqunpackl(a, m, n, l)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixbd(ByRef a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef tauq() As Double, ByRef taup() As Double)
        Try
            alglib.rmatrixbd(a, m, n, tauq, taup)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixbdunpackq(ByVal qp(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal tauq() As Double, ByVal qcolumns As Integer, ByRef q(,) As Double)
        Try
            alglib.rmatrixbdunpackq(qp, m, n, tauq, qcolumns, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixbdmultiplybyq(ByVal qp(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal tauq() As Double, ByRef z(,) As Double, ByVal zrows As Integer, ByVal zcolumns As Integer, ByVal fromtheright As Boolean, ByVal dotranspose As Boolean)
        Try
            alglib.rmatrixbdmultiplybyq(qp, m, n, tauq, z, zrows, zcolumns, fromtheright, dotranspose)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixbdunpackpt(ByVal qp(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal taup() As Double, ByVal ptrows As Integer, ByRef pt(,) As Double)
        Try
            alglib.rmatrixbdunpackpt(qp, m, n, taup, ptrows, pt)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixbdmultiplybyp(ByVal qp(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal taup() As Double, ByRef z(,) As Double, ByVal zrows As Integer, ByVal zcolumns As Integer, ByVal fromtheright As Boolean, ByVal dotranspose As Boolean)
        Try
            alglib.rmatrixbdmultiplybyp(qp, m, n, taup, z, zrows, zcolumns, fromtheright, dotranspose)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixbdunpackdiagonals(ByVal b(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef isupper As Boolean, ByRef d() As Double, ByRef e() As Double)
        Try
            alglib.rmatrixbdunpackdiagonals(b, m, n, isupper, d, e)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixhessenberg(ByRef a(,) As Double, ByVal n As Integer, ByRef tau() As Double)
        Try
            alglib.rmatrixhessenberg(a, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixhessenbergunpackq(ByVal a(,) As Double, ByVal n As Integer, ByVal tau() As Double, ByRef q(,) As Double)
        Try
            alglib.rmatrixhessenbergunpackq(a, n, tau, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixhessenbergunpackh(ByVal a(,) As Double, ByVal n As Integer, ByRef h(,) As Double)
        Try
            alglib.rmatrixhessenbergunpackh(a, n, h)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub smatrixtd(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByRef tau() As Double, ByRef d() As Double, ByRef e() As Double)
        Try
            alglib.smatrixtd(a, n, isupper, tau, d, e)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub smatrixtdunpackq(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal tau() As Double, ByRef q(,) As Double)
        Try
            alglib.smatrixtdunpackq(a, n, isupper, tau, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hmatrixtd(ByRef a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByRef tau() As alglib.complex, ByRef d() As Double, ByRef e() As Double)
        Try
            alglib.hmatrixtd(a, n, isupper, tau, d, e)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hmatrixtdunpackq(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal tau() As alglib.complex, ByRef q(,) As alglib.complex)
        Try
            alglib.hmatrixtdunpackq(a, n, isupper, tau, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function rmatrixbdsvd(ByRef d() As Double, ByVal e() As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal isfractionalaccuracyrequired As Boolean, ByRef u(,) As Double, ByVal nru As Integer, ByRef c(,) As Double, ByVal ncc As Integer, ByRef vt(,) As Double, ByVal ncvt As Integer) As Boolean
        Try
            rmatrixbdsvd = alglib.rmatrixbdsvd(d, e, n, isupper, isfractionalaccuracyrequired, u, nru, c, ncc, vt, ncvt)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function rmatrixsvd(ByVal a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal uneeded As Integer, ByVal vtneeded As Integer, ByVal additionalmemory As Integer, ByRef w() As Double, ByRef u(,) As Double, ByRef vt(,) As Double) As Boolean
        Try
            rmatrixsvd = alglib.rmatrixsvd(a, m, n, uneeded, vtneeded, additionalmemory, w, u, vt)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_rmatrixsvd(ByVal a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByVal uneeded As Integer, ByVal vtneeded As Integer, ByVal additionalmemory As Integer, ByRef w() As Double, ByRef u(,) As Double, ByRef vt(,) As Double) As Boolean
        Try
    smp_rmatrixsvd = alglib.smp_rmatrixsvd(a, m, n, uneeded, vtneeded, additionalmemory, w, u, vt)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function

    Public Class linearmodel
        Public csobj As alglib.linearmodel
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'LRReport structure contains additional information about linear model:
    '* C             -   covariation matrix,  array[0..NVars,0..NVars].
    '                    C[i,j] = Cov(A[i],A[j])
    '* RMSError      -   root mean square error on a training set
    '* AvgError      -   average error on a training set
    '* AvgRelError   -   average relative error on a training set (excluding
    '                    observations with zero function value).
    '* CVRMSError    -   leave-one-out cross-validation estimate of
    '                    generalization error. Calculated using fast algorithm
    '                    with O(NVars*NPoints) complexity.
    '* CVAvgError    -   cross-validation estimate of average error
    '* CVAvgRelError -   cross-validation estimate of average relative error
    '
    'All other fields of the structure are intended for internal use and should
    'not be used outside ALGLIB.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class lrreport
        Public Property c() As Double(,)
        Get
            Return csobj.c
        End Get
        Set(ByVal Value As Double(,))
            csobj.c = Value
        End Set
        End Property
        Public Property rmserror() As Double
        Get
            Return csobj.rmserror
        End Get
        Set(ByVal Value As Double)
            csobj.rmserror = Value
        End Set
        End Property
        Public Property avgerror() As Double
        Get
            Return csobj.avgerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgerror = Value
        End Set
        End Property
        Public Property avgrelerror() As Double
        Get
            Return csobj.avgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgrelerror = Value
        End Set
        End Property
        Public Property cvrmserror() As Double
        Get
            Return csobj.cvrmserror
        End Get
        Set(ByVal Value As Double)
            csobj.cvrmserror = Value
        End Set
        End Property
        Public Property cvavgerror() As Double
        Get
            Return csobj.cvavgerror
        End Get
        Set(ByVal Value As Double)
            csobj.cvavgerror = Value
        End Set
        End Property
        Public Property cvavgrelerror() As Double
        Get
            Return csobj.cvavgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.cvavgrelerror = Value
        End Set
        End Property
        Public Property ncvdefects() As Integer
        Get
            Return csobj.ncvdefects
        End Get
        Set(ByVal Value As Integer)
            csobj.ncvdefects = Value
        End Set
        End Property
        Public Property cvdefects() As Integer()
        Get
            Return csobj.cvdefects
        End Get
        Set(ByVal Value As Integer())
            csobj.cvdefects = Value
        End Set
        End Property
        Public csobj As alglib.lrreport
    End Class


    Public Sub lrbuild(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByRef info As Integer, ByRef lm As linearmodel, ByRef ar As lrreport)
        Try
            lm = New linearmodel()
            ar = New lrreport()
            alglib.lrbuild(xy, npoints, nvars, info, lm.csobj, ar.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lrbuilds(ByVal xy(,) As Double, ByVal s() As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByRef info As Integer, ByRef lm As linearmodel, ByRef ar As lrreport)
        Try
            lm = New linearmodel()
            ar = New lrreport()
            alglib.lrbuilds(xy, s, npoints, nvars, info, lm.csobj, ar.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lrbuildzs(ByVal xy(,) As Double, ByVal s() As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByRef info As Integer, ByRef lm As linearmodel, ByRef ar As lrreport)
        Try
            lm = New linearmodel()
            ar = New lrreport()
            alglib.lrbuildzs(xy, s, npoints, nvars, info, lm.csobj, ar.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lrbuildz(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByRef info As Integer, ByRef lm As linearmodel, ByRef ar As lrreport)
        Try
            lm = New linearmodel()
            ar = New lrreport()
            alglib.lrbuildz(xy, npoints, nvars, info, lm.csobj, ar.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lrunpack(ByVal lm As linearmodel, ByRef v() As Double, ByRef nvars As Integer)
        Try
            alglib.lrunpack(lm.csobj, v, nvars)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lrpack(ByVal v() As Double, ByVal nvars As Integer, ByRef lm As linearmodel)
        Try
            lm = New linearmodel()
            alglib.lrpack(v, nvars, lm.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function lrprocess(ByVal lm As linearmodel, ByVal x() As Double) As Double
        Try
            lrprocess = alglib.lrprocess(lm.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function lrrmserror(ByVal lm As linearmodel, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            lrrmserror = alglib.lrrmserror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function lravgerror(ByVal lm As linearmodel, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            lravgerror = alglib.lravgerror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function lravgrelerror(ByVal lm As linearmodel, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            lravgrelerror = alglib.lravgrelerror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Sub filtersma(ByRef x() As Double, ByVal n As Integer, ByVal k As Integer)
        Try
            alglib.filtersma(x, n, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub filtersma(ByRef x() As Double, ByVal k As Integer)
        Try
            alglib.filtersma(x, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub filterema(ByRef x() As Double, ByVal n As Integer, ByVal alpha As Double)
        Try
            alglib.filterema(x, n, alpha)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub filterema(ByRef x() As Double, ByVal alpha As Double)
        Try
            alglib.filterema(x, alpha)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub filterlrma(ByRef x() As Double, ByVal n As Integer, ByVal k As Integer)
        Try
            alglib.filterlrma(x, n, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub filterlrma(ByRef x() As Double, ByVal k As Integer)
        Try
            alglib.filterlrma(x, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function smatrixevd(ByVal a(,) As Double, ByVal n As Integer, ByVal zneeded As Integer, ByVal isupper As Boolean, ByRef d() As Double, ByRef z(,) As Double) As Boolean
        Try
            smatrixevd = alglib.smatrixevd(a, n, zneeded, isupper, d, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function smatrixevdr(ByVal a(,) As Double, ByVal n As Integer, ByVal zneeded As Integer, ByVal isupper As Boolean, ByVal b1 As Double, ByVal b2 As Double, ByRef m As Integer, ByRef w() As Double, ByRef z(,) As Double) As Boolean
        Try
            smatrixevdr = alglib.smatrixevdr(a, n, zneeded, isupper, b1, b2, m, w, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function smatrixevdi(ByVal a(,) As Double, ByVal n As Integer, ByVal zneeded As Integer, ByVal isupper As Boolean, ByVal i1 As Integer, ByVal i2 As Integer, ByRef w() As Double, ByRef z(,) As Double) As Boolean
        Try
            smatrixevdi = alglib.smatrixevdi(a, n, zneeded, isupper, i1, i2, w, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hmatrixevd(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal zneeded As Integer, ByVal isupper As Boolean, ByRef d() As Double, ByRef z(,) As alglib.complex) As Boolean
        Try
            hmatrixevd = alglib.hmatrixevd(a, n, zneeded, isupper, d, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hmatrixevdr(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal zneeded As Integer, ByVal isupper As Boolean, ByVal b1 As Double, ByVal b2 As Double, ByRef m As Integer, ByRef w() As Double, ByRef z(,) As alglib.complex) As Boolean
        Try
            hmatrixevdr = alglib.hmatrixevdr(a, n, zneeded, isupper, b1, b2, m, w, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hmatrixevdi(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal zneeded As Integer, ByVal isupper As Boolean, ByVal i1 As Integer, ByVal i2 As Integer, ByRef w() As Double, ByRef z(,) As alglib.complex) As Boolean
        Try
            hmatrixevdi = alglib.hmatrixevdi(a, n, zneeded, isupper, i1, i2, w, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function smatrixtdevd(ByRef d() As Double, ByVal e() As Double, ByVal n As Integer, ByVal zneeded As Integer, ByRef z(,) As Double) As Boolean
        Try
            smatrixtdevd = alglib.smatrixtdevd(d, e, n, zneeded, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function smatrixtdevdr(ByRef d() As Double, ByVal e() As Double, ByVal n As Integer, ByVal zneeded As Integer, ByVal a As Double, ByVal b As Double, ByRef m As Integer, ByRef z(,) As Double) As Boolean
        Try
            smatrixtdevdr = alglib.smatrixtdevdr(d, e, n, zneeded, a, b, m, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function smatrixtdevdi(ByRef d() As Double, ByVal e() As Double, ByVal n As Integer, ByVal zneeded As Integer, ByVal i1 As Integer, ByVal i2 As Integer, ByRef z(,) As Double) As Boolean
        Try
            smatrixtdevdi = alglib.smatrixtdevdi(d, e, n, zneeded, i1, i2, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rmatrixevd(ByVal a(,) As Double, ByVal n As Integer, ByVal vneeded As Integer, ByRef wr() As Double, ByRef wi() As Double, ByRef vl(,) As Double, ByRef vr(,) As Double) As Boolean
        Try
            rmatrixevd = alglib.rmatrixevd(a, n, vneeded, wr, wi, vl, vr)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Sub rmatrixrndorthogonal(ByVal n As Integer, ByRef a(,) As Double)
        Try
            alglib.rmatrixrndorthogonal(n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixrndcond(ByVal n As Integer, ByVal c As Double, ByRef a(,) As Double)
        Try
            alglib.rmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixrndorthogonal(ByVal n As Integer, ByRef a(,) As alglib.complex)
        Try
            alglib.cmatrixrndorthogonal(n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixrndcond(ByVal n As Integer, ByVal c As Double, ByRef a(,) As alglib.complex)
        Try
            alglib.cmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub smatrixrndcond(ByVal n As Integer, ByVal c As Double, ByRef a(,) As Double)
        Try
            alglib.smatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixrndcond(ByVal n As Integer, ByVal c As Double, ByRef a(,) As Double)
        Try
            alglib.spdmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hmatrixrndcond(ByVal n As Integer, ByVal c As Double, ByRef a(,) As alglib.complex)
        Try
            alglib.hmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hpdmatrixrndcond(ByVal n As Integer, ByVal c As Double, ByRef a(,) As alglib.complex)
        Try
            alglib.hpdmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixrndorthogonalfromtheright(ByRef a(,) As Double, ByVal m As Integer, ByVal n As Integer)
        Try
            alglib.rmatrixrndorthogonalfromtheright(a, m, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixrndorthogonalfromtheleft(ByRef a(,) As Double, ByVal m As Integer, ByVal n As Integer)
        Try
            alglib.rmatrixrndorthogonalfromtheleft(a, m, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixrndorthogonalfromtheright(ByRef a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer)
        Try
            alglib.cmatrixrndorthogonalfromtheright(a, m, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixrndorthogonalfromtheleft(ByRef a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer)
        Try
            alglib.cmatrixrndorthogonalfromtheleft(a, m, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub smatrixrndmultiply(ByRef a(,) As Double, ByVal n As Integer)
        Try
            alglib.smatrixrndmultiply(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hmatrixrndmultiply(ByRef a(,) As alglib.complex, ByVal n As Integer)
        Try
            alglib.hmatrixrndmultiply(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class sparsematrix
        Public csobj As alglib.sparsematrix
    End Class
    Public Class sparsebuffers
        Public csobj As alglib.sparsebuffers
    End Class


    Public Sub sparsecreate(ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByRef s As sparsematrix)
        Try
            s = New sparsematrix()
            alglib.sparsecreate(m, n, k, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecreate(ByVal m As Integer, ByVal n As Integer, ByRef s As sparsematrix)
        Try
            s = New sparsematrix()
            alglib.sparsecreate(m, n, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecreatebuf(ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal s As sparsematrix)
        Try
            alglib.sparsecreatebuf(m, n, k, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecreatebuf(ByVal m As Integer, ByVal n As Integer, ByVal s As sparsematrix)
        Try
            alglib.sparsecreatebuf(m, n, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecreatecrs(ByVal m As Integer, ByVal n As Integer, ByVal ner() As Integer, ByRef s As sparsematrix)
        Try
            s = New sparsematrix()
            alglib.sparsecreatecrs(m, n, ner, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecreatecrsbuf(ByVal m As Integer, ByVal n As Integer, ByVal ner() As Integer, ByVal s As sparsematrix)
        Try
            alglib.sparsecreatecrsbuf(m, n, ner, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecreatesks(ByVal m As Integer, ByVal n As Integer, ByVal d() As Integer, ByVal u() As Integer, ByRef s As sparsematrix)
        Try
            s = New sparsematrix()
            alglib.sparsecreatesks(m, n, d, u, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecreatesksbuf(ByVal m As Integer, ByVal n As Integer, ByVal d() As Integer, ByVal u() As Integer, ByVal s As sparsematrix)
        Try
            alglib.sparsecreatesksbuf(m, n, d, u, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecopy(ByVal s0 As sparsematrix, ByRef s1 As sparsematrix)
        Try
            s1 = New sparsematrix()
            alglib.sparsecopy(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecopybuf(ByVal s0 As sparsematrix, ByVal s1 As sparsematrix)
        Try
            alglib.sparsecopybuf(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparseswap(ByVal s0 As sparsematrix, ByVal s1 As sparsematrix)
        Try
            alglib.sparseswap(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparseadd(ByVal s As sparsematrix, ByVal i As Integer, ByVal j As Integer, ByVal v As Double)
        Try
            alglib.sparseadd(s.csobj, i, j, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparseset(ByVal s As sparsematrix, ByVal i As Integer, ByVal j As Integer, ByVal v As Double)
        Try
            alglib.sparseset(s.csobj, i, j, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function sparseget(ByVal s As sparsematrix, ByVal i As Integer, ByVal j As Integer) As Double
        Try
            sparseget = alglib.sparseget(s.csobj, i, j)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sparsegetdiagonal(ByVal s As sparsematrix, ByVal i As Integer) As Double
        Try
            sparsegetdiagonal = alglib.sparsegetdiagonal(s.csobj, i)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub sparsemv(ByVal s As sparsematrix, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.sparsemv(s.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsemtv(ByVal s As sparsematrix, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.sparsemtv(s.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsemv2(ByVal s As sparsematrix, ByVal x() As Double, ByRef y0() As Double, ByRef y1() As Double)
        Try
            alglib.sparsemv2(s.csobj, x, y0, y1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsesmv(ByVal s As sparsematrix, ByVal isupper As Boolean, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.sparsesmv(s.csobj, isupper, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function sparsevsmv(ByVal s As sparsematrix, ByVal isupper As Boolean, ByVal x() As Double) As Double
        Try
            sparsevsmv = alglib.sparsevsmv(s.csobj, isupper, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub sparsemm(ByVal s As sparsematrix, ByVal a(,) As Double, ByVal k As Integer, ByRef b(,) As Double)
        Try
            alglib.sparsemm(s.csobj, a, k, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsemtm(ByVal s As sparsematrix, ByVal a(,) As Double, ByVal k As Integer, ByRef b(,) As Double)
        Try
            alglib.sparsemtm(s.csobj, a, k, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsemm2(ByVal s As sparsematrix, ByVal a(,) As Double, ByVal k As Integer, ByRef b0(,) As Double, ByRef b1(,) As Double)
        Try
            alglib.sparsemm2(s.csobj, a, k, b0, b1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsesmm(ByVal s As sparsematrix, ByVal isupper As Boolean, ByVal a(,) As Double, ByVal k As Integer, ByRef b(,) As Double)
        Try
            alglib.sparsesmm(s.csobj, isupper, a, k, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsetrmv(ByVal s As sparsematrix, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x() As Double, ByRef y() As Double)
        Try
            alglib.sparsetrmv(s.csobj, isupper, isunit, optype, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsetrsv(ByVal s As sparsematrix, ByVal isupper As Boolean, ByVal isunit As Boolean, ByVal optype As Integer, ByRef x() As Double)
        Try
            alglib.sparsetrsv(s.csobj, isupper, isunit, optype, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparseresizematrix(ByVal s As sparsematrix)
        Try
            alglib.sparseresizematrix(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function sparseenumerate(ByVal s As sparsematrix, ByRef t0 As Integer, ByRef t1 As Integer, ByRef i As Integer, ByRef j As Integer, ByRef v As Double) As Boolean
        Try
            sparseenumerate = alglib.sparseenumerate(s.csobj, t0, t1, i, j, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sparserewriteexisting(ByVal s As sparsematrix, ByVal i As Integer, ByVal j As Integer, ByVal v As Double) As Boolean
        Try
            sparserewriteexisting = alglib.sparserewriteexisting(s.csobj, i, j, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub sparsegetrow(ByVal s As sparsematrix, ByVal i As Integer, ByRef irow() As Double)
        Try
            alglib.sparsegetrow(s.csobj, i, irow)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsegetcompressedrow(ByVal s As sparsematrix, ByVal i As Integer, ByRef colidx() As Integer, ByRef vals() As Double, ByRef nzcnt As Integer)
        Try
            alglib.sparsegetcompressedrow(s.csobj, i, colidx, vals, nzcnt)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsetransposesks(ByVal s As sparsematrix)
        Try
            alglib.sparsetransposesks(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparseconvertto(ByVal s0 As sparsematrix, ByVal fmt As Integer)
        Try
            alglib.sparseconvertto(s0.csobj, fmt)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecopytobuf(ByVal s0 As sparsematrix, ByVal fmt As Integer, ByVal s1 As sparsematrix)
        Try
            alglib.sparsecopytobuf(s0.csobj, fmt, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparseconverttohash(ByVal s As sparsematrix)
        Try
            alglib.sparseconverttohash(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecopytohash(ByVal s0 As sparsematrix, ByRef s1 As sparsematrix)
        Try
            s1 = New sparsematrix()
            alglib.sparsecopytohash(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecopytohashbuf(ByVal s0 As sparsematrix, ByVal s1 As sparsematrix)
        Try
            alglib.sparsecopytohashbuf(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparseconverttocrs(ByVal s As sparsematrix)
        Try
            alglib.sparseconverttocrs(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecopytocrs(ByVal s0 As sparsematrix, ByRef s1 As sparsematrix)
        Try
            s1 = New sparsematrix()
            alglib.sparsecopytocrs(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecopytocrsbuf(ByVal s0 As sparsematrix, ByVal s1 As sparsematrix)
        Try
            alglib.sparsecopytocrsbuf(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparseconverttosks(ByVal s As sparsematrix)
        Try
            alglib.sparseconverttosks(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecopytosks(ByVal s0 As sparsematrix, ByRef s1 As sparsematrix)
        Try
            s1 = New sparsematrix()
            alglib.sparsecopytosks(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub sparsecopytosksbuf(ByVal s0 As sparsematrix, ByVal s1 As sparsematrix)
        Try
            alglib.sparsecopytosksbuf(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function sparsegetmatrixtype(ByVal s As sparsematrix) As Integer
        Try
            sparsegetmatrixtype = alglib.sparsegetmatrixtype(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sparseishash(ByVal s As sparsematrix) As Boolean
        Try
            sparseishash = alglib.sparseishash(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sparseiscrs(ByVal s As sparsematrix) As Boolean
        Try
            sparseiscrs = alglib.sparseiscrs(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sparseissks(ByVal s As sparsematrix) As Boolean
        Try
            sparseissks = alglib.sparseissks(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub sparsefree(ByRef s As sparsematrix)
        Try
            s = New sparsematrix()
            alglib.sparsefree(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function sparsegetnrows(ByVal s As sparsematrix) As Integer
        Try
            sparsegetnrows = alglib.sparsegetnrows(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sparsegetncols(ByVal s As sparsematrix) As Integer
        Try
            sparsegetncols = alglib.sparsegetncols(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sparsegetuppercount(ByVal s As sparsematrix) As Integer
        Try
            sparsegetuppercount = alglib.sparsegetuppercount(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function sparsegetlowercount(ByVal s As sparsematrix) As Integer
        Try
            sparsegetlowercount = alglib.sparsegetlowercount(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Sub rmatrixlu(ByRef a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef pivots() As Integer)
        Try
            alglib.rmatrixlu(a, m, n, pivots)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixlu(ByRef a(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef pivots() As Integer)
        Try
    alglib.smp_rmatrixlu(a, m, n, pivots)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixlu(ByRef a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByRef pivots() As Integer)
        Try
            alglib.cmatrixlu(a, m, n, pivots)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixlu(ByRef a(,) As alglib.complex, ByVal m As Integer, ByVal n As Integer, ByRef pivots() As Integer)
        Try
    alglib.smp_cmatrixlu(a, m, n, pivots)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function hpdmatrixcholesky(ByRef a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean) As Boolean
        Try
            hpdmatrixcholesky = alglib.hpdmatrixcholesky(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_hpdmatrixcholesky(ByRef a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean) As Boolean
        Try
    smp_hpdmatrixcholesky = alglib.smp_hpdmatrixcholesky(a, n, isupper)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spdmatrixcholesky(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean) As Boolean
        Try
            spdmatrixcholesky = alglib.spdmatrixcholesky(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_spdmatrixcholesky(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean) As Boolean
        Try
    smp_spdmatrixcholesky = alglib.smp_spdmatrixcholesky(a, n, isupper)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub spdmatrixcholeskyupdateadd1(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal u() As Double)
        Try
            alglib.spdmatrixcholeskyupdateadd1(a, n, isupper, u)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskyupdatefix(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal fix() As Boolean)
        Try
            alglib.spdmatrixcholeskyupdatefix(a, n, isupper, fix)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskyupdateadd1buf(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal u() As Double, ByRef bufr() As Double)
        Try
            alglib.spdmatrixcholeskyupdateadd1buf(a, n, isupper, u, bufr)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskyupdatefixbuf(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal fix() As Boolean, ByRef bufr() As Double)
        Try
            alglib.spdmatrixcholeskyupdatefixbuf(a, n, isupper, fix, bufr)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function sparsecholeskyskyline(ByVal a As sparsematrix, ByVal n As Integer, ByVal isupper As Boolean) As Boolean
        Try
            sparsecholeskyskyline = alglib.sparsecholeskyskyline(a.csobj, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function rmatrixrcond1(ByVal a(,) As Double, ByVal n As Integer) As Double
        Try
            rmatrixrcond1 = alglib.rmatrixrcond1(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rmatrixrcondinf(ByVal a(,) As Double, ByVal n As Integer) As Double
        Try
            rmatrixrcondinf = alglib.rmatrixrcondinf(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spdmatrixrcond(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean) As Double
        Try
            spdmatrixrcond = alglib.spdmatrixrcond(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rmatrixtrrcond1(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean) As Double
        Try
            rmatrixtrrcond1 = alglib.rmatrixtrrcond1(a, n, isupper, isunit)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rmatrixtrrcondinf(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean) As Double
        Try
            rmatrixtrrcondinf = alglib.rmatrixtrrcondinf(a, n, isupper, isunit)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hpdmatrixrcond(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean) As Double
        Try
            hpdmatrixrcond = alglib.hpdmatrixrcond(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixrcond1(ByVal a(,) As alglib.complex, ByVal n As Integer) As Double
        Try
            cmatrixrcond1 = alglib.cmatrixrcond1(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixrcondinf(ByVal a(,) As alglib.complex, ByVal n As Integer) As Double
        Try
            cmatrixrcondinf = alglib.cmatrixrcondinf(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rmatrixlurcond1(ByVal lua(,) As Double, ByVal n As Integer) As Double
        Try
            rmatrixlurcond1 = alglib.rmatrixlurcond1(lua, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rmatrixlurcondinf(ByVal lua(,) As Double, ByVal n As Integer) As Double
        Try
            rmatrixlurcondinf = alglib.rmatrixlurcondinf(lua, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spdmatrixcholeskyrcond(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean) As Double
        Try
            spdmatrixcholeskyrcond = alglib.spdmatrixcholeskyrcond(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hpdmatrixcholeskyrcond(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean) As Double
        Try
            hpdmatrixcholeskyrcond = alglib.hpdmatrixcholeskyrcond(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixlurcond1(ByVal lua(,) As alglib.complex, ByVal n As Integer) As Double
        Try
            cmatrixlurcond1 = alglib.cmatrixlurcond1(lua, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixlurcondinf(ByVal lua(,) As alglib.complex, ByVal n As Integer) As Double
        Try
            cmatrixlurcondinf = alglib.cmatrixlurcondinf(lua, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixtrrcond1(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean) As Double
        Try
            cmatrixtrrcond1 = alglib.cmatrixtrrcond1(a, n, isupper, isunit)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixtrrcondinf(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean) As Double
        Try
            cmatrixtrrcondinf = alglib.cmatrixtrrcondinf(a, n, isupper, isunit)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Matrix inverse report:
    '* R1    reciprocal of condition number in 1-norm
    '* RInf  reciprocal of condition number in inf-norm
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class matinvreport
        Public Property r1() As Double
        Get
            Return csobj.r1
        End Get
        Set(ByVal Value As Double)
            csobj.r1 = Value
        End Set
        End Property
        Public Property rinf() As Double
        Get
            Return csobj.rinf
        End Get
        Set(ByVal Value As Double)
            csobj.rinf = Value
        End Set
        End Property
        Public csobj As alglib.matinvreport
    End Class


    Public Sub rmatrixluinverse(ByRef a(,) As Double, ByVal pivots() As Integer, ByVal n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixluinverse(a, pivots, n, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixluinverse(ByRef a(,) As Double, ByVal pivots() As Integer, ByVal n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_rmatrixluinverse(a, pivots, n, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixluinverse(ByRef a(,) As Double, ByVal pivots() As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixluinverse(a, pivots, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixluinverse(ByRef a(,) As Double, ByVal pivots() As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_rmatrixluinverse(a, pivots, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixinverse(ByRef a(,) As Double, ByVal n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixinverse(a, n, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixinverse(ByRef a(,) As Double, ByVal n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_rmatrixinverse(a, n, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixinverse(ByRef a(,) As Double, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixinverse(ByRef a(,) As Double, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_rmatrixinverse(a, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixluinverse(ByRef a(,) As alglib.complex, ByVal pivots() As Integer, ByVal n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixluinverse(a, pivots, n, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixluinverse(ByRef a(,) As alglib.complex, ByVal pivots() As Integer, ByVal n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_cmatrixluinverse(a, pivots, n, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixluinverse(ByRef a(,) As alglib.complex, ByVal pivots() As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixluinverse(a, pivots, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixluinverse(ByRef a(,) As alglib.complex, ByVal pivots() As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_cmatrixluinverse(a, pivots, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixinverse(ByRef a(,) As alglib.complex, ByVal n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixinverse(a, n, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixinverse(ByRef a(,) As alglib.complex, ByVal n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_cmatrixinverse(a, n, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixinverse(ByRef a(,) As alglib.complex, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixinverse(ByRef a(,) As alglib.complex, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_cmatrixinverse(a, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskyinverse(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.spdmatrixcholeskyinverse(a, n, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spdmatrixcholeskyinverse(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_spdmatrixcholeskyinverse(a, n, isupper, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskyinverse(ByRef a(,) As Double, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.spdmatrixcholeskyinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spdmatrixcholeskyinverse(ByRef a(,) As Double, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_spdmatrixcholeskyinverse(a, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixinverse(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.spdmatrixinverse(a, n, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spdmatrixinverse(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_spdmatrixinverse(a, n, isupper, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixinverse(ByRef a(,) As Double, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.spdmatrixinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spdmatrixinverse(ByRef a(,) As Double, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_spdmatrixinverse(a, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hpdmatrixcholeskyinverse(ByRef a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.hpdmatrixcholeskyinverse(a, n, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_hpdmatrixcholeskyinverse(ByRef a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_hpdmatrixcholeskyinverse(a, n, isupper, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hpdmatrixcholeskyinverse(ByRef a(,) As alglib.complex, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.hpdmatrixcholeskyinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_hpdmatrixcholeskyinverse(ByRef a(,) As alglib.complex, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_hpdmatrixcholeskyinverse(a, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hpdmatrixinverse(ByRef a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.hpdmatrixinverse(a, n, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_hpdmatrixinverse(ByRef a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_hpdmatrixinverse(a, n, isupper, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hpdmatrixinverse(ByRef a(,) As alglib.complex, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.hpdmatrixinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_hpdmatrixinverse(ByRef a(,) As alglib.complex, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_hpdmatrixinverse(a, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixtrinverse(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixtrinverse(a, n, isupper, isunit, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixtrinverse(ByRef a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_rmatrixtrinverse(a, n, isupper, isunit, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixtrinverse(ByRef a(,) As Double, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixtrinverse(a, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixtrinverse(ByRef a(,) As Double, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_rmatrixtrinverse(a, isupper, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixtrinverse(ByRef a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixtrinverse(a, n, isupper, isunit, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixtrinverse(ByRef a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal isunit As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_cmatrixtrinverse(a, n, isupper, isunit, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixtrinverse(ByRef a(,) As alglib.complex, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixtrinverse(a, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixtrinverse(ByRef a(,) As alglib.complex, ByVal isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
    rep = New matinvreport()
    alglib.smp_cmatrixtrinverse(a, isupper, info, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub fisherlda(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByVal nclasses As Integer, ByRef info As Integer, ByRef w() As Double)
        Try
            alglib.fisherlda(xy, npoints, nvars, nclasses, info, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fisherldan(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByVal nclasses As Integer, ByRef info As Integer, ByRef w(,) As Double)
        Try
            alglib.fisherldan(xy, npoints, nvars, nclasses, info, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_fisherldan(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByVal nclasses As Integer, ByRef info As Integer, ByRef w(,) As Double)
        Try
    alglib.smp_fisherldan(xy, npoints, nvars, nclasses, info, w)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Model's errors:
    '    * RelCLSError   -   fraction of misclassified cases.
    '    * AvgCE         -   acerage cross-entropy
    '    * RMSError      -   root-mean-square error
    '    * AvgError      -   average error
    '    * AvgRelError   -   average relative error
    '
    'NOTE 1: RelCLSError/AvgCE are zero on regression problems.
    '
    'NOTE 2: on classification problems  RMSError/AvgError/AvgRelError  contain
    '        errors in prediction of posterior probabilities
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class modelerrors
        Public Property relclserror() As Double
        Get
            Return csobj.relclserror
        End Get
        Set(ByVal Value As Double)
            csobj.relclserror = Value
        End Set
        End Property
        Public Property avgce() As Double
        Get
            Return csobj.avgce
        End Get
        Set(ByVal Value As Double)
            csobj.avgce = Value
        End Set
        End Property
        Public Property rmserror() As Double
        Get
            Return csobj.rmserror
        End Get
        Set(ByVal Value As Double)
            csobj.rmserror = Value
        End Set
        End Property
        Public Property avgerror() As Double
        Get
            Return csobj.avgerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgerror = Value
        End Set
        End Property
        Public Property avgrelerror() As Double
        Get
            Return csobj.avgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgrelerror = Value
        End Set
        End Property
        Public csobj As alglib.modelerrors
    End Class
    Public Class multilayerperceptron
        Public csobj As alglib.multilayerperceptron
    End Class
    Public Sub mlpserialize(ByVal obj As multilayerperceptron, ByRef s_out As String)
        Try
            alglib.mlpserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Sub mlpunserialize(ByVal s_in As String, ByRef obj As multilayerperceptron)
        Try
            alglib.mlpunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreate0(ByVal nin As Integer, ByVal nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreate0(nin, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreate1(ByVal nin As Integer, ByVal nhid As Integer, ByVal nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreate1(nin, nhid, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreate2(ByVal nin As Integer, ByVal nhid1 As Integer, ByVal nhid2 As Integer, ByVal nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreate2(nin, nhid1, nhid2, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreateb0(ByVal nin As Integer, ByVal nout As Integer, ByVal b As Double, ByVal d As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreateb0(nin, nout, b, d, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreateb1(ByVal nin As Integer, ByVal nhid As Integer, ByVal nout As Integer, ByVal b As Double, ByVal d As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreateb1(nin, nhid, nout, b, d, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreateb2(ByVal nin As Integer, ByVal nhid1 As Integer, ByVal nhid2 As Integer, ByVal nout As Integer, ByVal b As Double, ByVal d As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreateb2(nin, nhid1, nhid2, nout, b, d, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreater0(ByVal nin As Integer, ByVal nout As Integer, ByVal a As Double, ByVal b As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreater0(nin, nout, a, b, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreater1(ByVal nin As Integer, ByVal nhid As Integer, ByVal nout As Integer, ByVal a As Double, ByVal b As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreater1(nin, nhid, nout, a, b, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreater2(ByVal nin As Integer, ByVal nhid1 As Integer, ByVal nhid2 As Integer, ByVal nout As Integer, ByVal a As Double, ByVal b As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreater2(nin, nhid1, nhid2, nout, a, b, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreatec0(ByVal nin As Integer, ByVal nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreatec0(nin, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreatec1(ByVal nin As Integer, ByVal nhid As Integer, ByVal nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreatec1(nin, nhid, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreatec2(ByVal nin As Integer, ByVal nhid1 As Integer, ByVal nhid2 As Integer, ByVal nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreatec2(nin, nhid1, nhid2, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcopy(ByVal network1 As multilayerperceptron, ByRef network2 As multilayerperceptron)
        Try
            network2 = New multilayerperceptron()
            alglib.mlpcopy(network1.csobj, network2.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcopytunableparameters(ByVal network1 As multilayerperceptron, ByVal network2 As multilayerperceptron)
        Try
            alglib.mlpcopytunableparameters(network1.csobj, network2.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlprandomize(ByVal network As multilayerperceptron)
        Try
            alglib.mlprandomize(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlprandomizefull(ByVal network As multilayerperceptron)
        Try
            alglib.mlprandomizefull(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpinitpreprocessor(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal ssize As Integer)
        Try
            alglib.mlpinitpreprocessor(network.csobj, xy, ssize)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpproperties(ByVal network As multilayerperceptron, ByRef nin As Integer, ByRef nout As Integer, ByRef wcount As Integer)
        Try
            alglib.mlpproperties(network.csobj, nin, nout, wcount)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function mlpgetinputscount(ByVal network As multilayerperceptron) As Integer
        Try
            mlpgetinputscount = alglib.mlpgetinputscount(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpgetoutputscount(ByVal network As multilayerperceptron) As Integer
        Try
            mlpgetoutputscount = alglib.mlpgetoutputscount(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpgetweightscount(ByVal network As multilayerperceptron) As Integer
        Try
            mlpgetweightscount = alglib.mlpgetweightscount(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpissoftmax(ByVal network As multilayerperceptron) As Boolean
        Try
            mlpissoftmax = alglib.mlpissoftmax(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpgetlayerscount(ByVal network As multilayerperceptron) As Integer
        Try
            mlpgetlayerscount = alglib.mlpgetlayerscount(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpgetlayersize(ByVal network As multilayerperceptron, ByVal k As Integer) As Integer
        Try
            mlpgetlayersize = alglib.mlpgetlayersize(network.csobj, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub mlpgetinputscaling(ByVal network As multilayerperceptron, ByVal i As Integer, ByRef mean As Double, ByRef sigma As Double)
        Try
            alglib.mlpgetinputscaling(network.csobj, i, mean, sigma)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpgetoutputscaling(ByVal network As multilayerperceptron, ByVal i As Integer, ByRef mean As Double, ByRef sigma As Double)
        Try
            alglib.mlpgetoutputscaling(network.csobj, i, mean, sigma)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpgetneuroninfo(ByVal network As multilayerperceptron, ByVal k As Integer, ByVal i As Integer, ByRef fkind As Integer, ByRef threshold As Double)
        Try
            alglib.mlpgetneuroninfo(network.csobj, k, i, fkind, threshold)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function mlpgetweight(ByVal network As multilayerperceptron, ByVal k0 As Integer, ByVal i0 As Integer, ByVal k1 As Integer, ByVal i1 As Integer) As Double
        Try
            mlpgetweight = alglib.mlpgetweight(network.csobj, k0, i0, k1, i1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub mlpsetinputscaling(ByVal network As multilayerperceptron, ByVal i As Integer, ByVal mean As Double, ByVal sigma As Double)
        Try
            alglib.mlpsetinputscaling(network.csobj, i, mean, sigma)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpsetoutputscaling(ByVal network As multilayerperceptron, ByVal i As Integer, ByVal mean As Double, ByVal sigma As Double)
        Try
            alglib.mlpsetoutputscaling(network.csobj, i, mean, sigma)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpsetneuroninfo(ByVal network As multilayerperceptron, ByVal k As Integer, ByVal i As Integer, ByVal fkind As Integer, ByVal threshold As Double)
        Try
            alglib.mlpsetneuroninfo(network.csobj, k, i, fkind, threshold)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpsetweight(ByVal network As multilayerperceptron, ByVal k0 As Integer, ByVal i0 As Integer, ByVal k1 As Integer, ByVal i1 As Integer, ByVal w As Double)
        Try
            alglib.mlpsetweight(network.csobj, k0, i0, k1, i1, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpactivationfunction(ByVal net As Double, ByVal k As Integer, ByRef f As Double, ByRef df As Double, ByRef d2f As Double)
        Try
            alglib.mlpactivationfunction(net, k, f, df, d2f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpprocess(ByVal network As multilayerperceptron, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.mlpprocess(network.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpprocessi(ByVal network As multilayerperceptron, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.mlpprocessi(network.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function mlperror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlperror = alglib.mlperror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlperror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
    smp_mlperror = alglib.smp_mlperror(network.csobj, xy, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlperrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
            mlperrorsparse = alglib.mlperrorsparse(network.csobj, xy.csobj, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlperrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
    smp_mlperrorsparse = alglib.smp_mlperrorsparse(network.csobj, xy.csobj, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlperrorn(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal ssize As Integer) As Double
        Try
            mlperrorn = alglib.mlperrorn(network.csobj, xy, ssize)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpclserror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Integer
        Try
            mlpclserror = alglib.mlpclserror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlpclserror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Integer
        Try
    smp_mlpclserror = alglib.smp_mlpclserror(network.csobj, xy, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlprelclserror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlprelclserror = alglib.mlprelclserror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlprelclserror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
    smp_mlprelclserror = alglib.smp_mlprelclserror(network.csobj, xy, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlprelclserrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
            mlprelclserrorsparse = alglib.mlprelclserrorsparse(network.csobj, xy.csobj, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlprelclserrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
    smp_mlprelclserrorsparse = alglib.smp_mlprelclserrorsparse(network.csobj, xy.csobj, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpavgce(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlpavgce = alglib.mlpavgce(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlpavgce(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
    smp_mlpavgce = alglib.smp_mlpavgce(network.csobj, xy, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpavgcesparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
            mlpavgcesparse = alglib.mlpavgcesparse(network.csobj, xy.csobj, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlpavgcesparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
    smp_mlpavgcesparse = alglib.smp_mlpavgcesparse(network.csobj, xy.csobj, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlprmserror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlprmserror = alglib.mlprmserror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlprmserror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
    smp_mlprmserror = alglib.smp_mlprmserror(network.csobj, xy, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlprmserrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
            mlprmserrorsparse = alglib.mlprmserrorsparse(network.csobj, xy.csobj, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlprmserrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
    smp_mlprmserrorsparse = alglib.smp_mlprmserrorsparse(network.csobj, xy.csobj, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpavgerror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlpavgerror = alglib.mlpavgerror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlpavgerror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
    smp_mlpavgerror = alglib.smp_mlpavgerror(network.csobj, xy, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpavgerrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
            mlpavgerrorsparse = alglib.mlpavgerrorsparse(network.csobj, xy.csobj, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlpavgerrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
    smp_mlpavgerrorsparse = alglib.smp_mlpavgerrorsparse(network.csobj, xy.csobj, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpavgrelerror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlpavgrelerror = alglib.mlpavgrelerror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlpavgrelerror(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
    smp_mlpavgrelerror = alglib.smp_mlpavgrelerror(network.csobj, xy, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpavgrelerrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
            mlpavgrelerrorsparse = alglib.mlpavgrelerrorsparse(network.csobj, xy.csobj, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlpavgrelerrorsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal npoints As Integer) As Double
        Try
    smp_mlpavgrelerrorsparse = alglib.smp_mlpavgrelerrorsparse(network.csobj, xy.csobj, npoints)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub mlpgrad(ByVal network As multilayerperceptron, ByVal x() As Double, ByVal desiredy() As Double, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgrad(network.csobj, x, desiredy, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpgradn(ByVal network As multilayerperceptron, ByVal x() As Double, ByVal desiredy() As Double, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgradn(network.csobj, x, desiredy, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpgradbatch(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal ssize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgradbatch(network.csobj, xy, ssize, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_mlpgradbatch(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal ssize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
    alglib.smp_mlpgradbatch(network.csobj, xy, ssize, e, grad)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpgradbatchsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal ssize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgradbatchsparse(network.csobj, xy.csobj, ssize, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_mlpgradbatchsparse(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal ssize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
    alglib.smp_mlpgradbatchsparse(network.csobj, xy.csobj, ssize, e, grad)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpgradbatchsubset(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal setsize As Integer, ByVal idx() As Integer, ByVal subsetsize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgradbatchsubset(network.csobj, xy, setsize, idx, subsetsize, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_mlpgradbatchsubset(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal setsize As Integer, ByVal idx() As Integer, ByVal subsetsize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
    alglib.smp_mlpgradbatchsubset(network.csobj, xy, setsize, idx, subsetsize, e, grad)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpgradbatchsparsesubset(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal setsize As Integer, ByVal idx() As Integer, ByVal subsetsize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgradbatchsparsesubset(network.csobj, xy.csobj, setsize, idx, subsetsize, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_mlpgradbatchsparsesubset(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal setsize As Integer, ByVal idx() As Integer, ByVal subsetsize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
    alglib.smp_mlpgradbatchsparsesubset(network.csobj, xy.csobj, setsize, idx, subsetsize, e, grad)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpgradnbatch(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal ssize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgradnbatch(network.csobj, xy, ssize, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlphessiannbatch(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal ssize As Integer, ByRef e As Double, ByRef grad() As Double, ByRef h(,) As Double)
        Try
            alglib.mlphessiannbatch(network.csobj, xy, ssize, e, grad, h)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlphessianbatch(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal ssize As Integer, ByRef e As Double, ByRef grad() As Double, ByRef h(,) As Double)
        Try
            alglib.mlphessianbatch(network.csobj, xy, ssize, e, grad, h)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpallerrorssubset(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal setsize As Integer, ByVal subset() As Integer, ByVal subsetsize As Integer, ByRef rep As modelerrors)
        Try
            rep = New modelerrors()
            alglib.mlpallerrorssubset(network.csobj, xy, setsize, subset, subsetsize, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_mlpallerrorssubset(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal setsize As Integer, ByVal subset() As Integer, ByVal subsetsize As Integer, ByRef rep As modelerrors)
        Try
    rep = New modelerrors()
    alglib.smp_mlpallerrorssubset(network.csobj, xy, setsize, subset, subsetsize, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpallerrorssparsesubset(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal setsize As Integer, ByVal subset() As Integer, ByVal subsetsize As Integer, ByRef rep As modelerrors)
        Try
            rep = New modelerrors()
            alglib.mlpallerrorssparsesubset(network.csobj, xy.csobj, setsize, subset, subsetsize, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_mlpallerrorssparsesubset(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal setsize As Integer, ByVal subset() As Integer, ByVal subsetsize As Integer, ByRef rep As modelerrors)
        Try
    rep = New modelerrors()
    alglib.smp_mlpallerrorssparsesubset(network.csobj, xy.csobj, setsize, subset, subsetsize, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function mlperrorsubset(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal setsize As Integer, ByVal subset() As Integer, ByVal subsetsize As Integer) As Double
        Try
            mlperrorsubset = alglib.mlperrorsubset(network.csobj, xy, setsize, subset, subsetsize)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlperrorsubset(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal setsize As Integer, ByVal subset() As Integer, ByVal subsetsize As Integer) As Double
        Try
    smp_mlperrorsubset = alglib.smp_mlperrorsubset(network.csobj, xy, setsize, subset, subsetsize)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlperrorsparsesubset(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal setsize As Integer, ByVal subset() As Integer, ByVal subsetsize As Integer) As Double
        Try
            mlperrorsparsesubset = alglib.mlperrorsparsesubset(network.csobj, xy.csobj, setsize, subset, subsetsize)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlperrorsparsesubset(ByVal network As multilayerperceptron, ByVal xy As sparsematrix, ByVal setsize As Integer, ByVal subset() As Integer, ByVal subsetsize As Integer) As Double
        Try
    smp_mlperrorsparsesubset = alglib.smp_mlperrorsparsesubset(network.csobj, xy.csobj, setsize, subset, subsetsize)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class densesolverreport
        Public Property r1() As Double
        Get
            Return csobj.r1
        End Get
        Set(ByVal Value As Double)
            csobj.r1 = Value
        End Set
        End Property
        Public Property rinf() As Double
        Get
            Return csobj.rinf
        End Get
        Set(ByVal Value As Double)
            csobj.rinf = Value
        End Set
        End Property
        Public csobj As alglib.densesolverreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class densesolverlsreport
        Public Property r2() As Double
        Get
            Return csobj.r2
        End Get
        Set(ByVal Value As Double)
            csobj.r2 = Value
        End Set
        End Property
        Public Property cx() As Double(,)
        Get
            Return csobj.cx
        End Get
        Set(ByVal Value As Double(,))
            csobj.cx = Value
        End Set
        End Property
        Public Property n() As Integer
        Get
            Return csobj.n
        End Get
        Set(ByVal Value As Integer)
            csobj.n = Value
        End Set
        End Property
        Public Property k() As Integer
        Get
            Return csobj.k
        End Get
        Set(ByVal Value As Integer)
            csobj.k = Value
        End Set
        End Property
        Public csobj As alglib.densesolverlsreport
    End Class


    Public Sub rmatrixsolve(ByVal a(,) As Double, ByVal n As Integer, ByVal b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixsolve(a, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixsolve(ByVal a(,) As Double, ByVal n As Integer, ByVal b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
    rep = New densesolverreport()
    alglib.smp_rmatrixsolve(a, n, b, info, rep.csobj, x)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixsolvem(ByVal a(,) As Double, ByVal n As Integer, ByVal b(,) As Double, ByVal m As Integer, ByVal rfs As Boolean, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixsolvem(a, n, b, m, rfs, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixsolvem(ByVal a(,) As Double, ByVal n As Integer, ByVal b(,) As Double, ByVal m As Integer, ByVal rfs As Boolean, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
    rep = New densesolverreport()
    alglib.smp_rmatrixsolvem(a, n, b, m, rfs, info, rep.csobj, x)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixlusolve(ByVal lua(,) As Double, ByVal p() As Integer, ByVal n As Integer, ByVal b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixlusolve(lua, p, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixlusolvem(ByVal lua(,) As Double, ByVal p() As Integer, ByVal n As Integer, ByVal b(,) As Double, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixlusolvem(lua, p, n, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixmixedsolve(ByVal a(,) As Double, ByVal lua(,) As Double, ByVal p() As Integer, ByVal n As Integer, ByVal b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixmixedsolve(a, lua, p, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixmixedsolvem(ByVal a(,) As Double, ByVal lua(,) As Double, ByVal p() As Integer, ByVal n As Integer, ByVal b(,) As Double, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixmixedsolvem(a, lua, p, n, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixsolvem(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal b(,) As alglib.complex, ByVal m As Integer, ByVal rfs As Boolean, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixsolvem(a, n, b, m, rfs, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixsolvem(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal b(,) As alglib.complex, ByVal m As Integer, ByVal rfs As Boolean, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
    rep = New densesolverreport()
    alglib.smp_cmatrixsolvem(a, n, b, m, rfs, info, rep.csobj, x)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixsolve(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixsolve(a, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_cmatrixsolve(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
    rep = New densesolverreport()
    alglib.smp_cmatrixsolve(a, n, b, info, rep.csobj, x)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixlusolvem(ByVal lua(,) As alglib.complex, ByVal p() As Integer, ByVal n As Integer, ByVal b(,) As alglib.complex, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixlusolvem(lua, p, n, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixlusolve(ByVal lua(,) As alglib.complex, ByVal p() As Integer, ByVal n As Integer, ByVal b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixlusolve(lua, p, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixmixedsolvem(ByVal a(,) As alglib.complex, ByVal lua(,) As alglib.complex, ByVal p() As Integer, ByVal n As Integer, ByVal b(,) As alglib.complex, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixmixedsolvem(a, lua, p, n, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub cmatrixmixedsolve(ByVal a(,) As alglib.complex, ByVal lua(,) As alglib.complex, ByVal p() As Integer, ByVal n As Integer, ByVal b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixmixedsolve(a, lua, p, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixsolvem(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal b(,) As Double, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.spdmatrixsolvem(a, n, isupper, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spdmatrixsolvem(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal b(,) As Double, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
    rep = New densesolverreport()
    alglib.smp_spdmatrixsolvem(a, n, isupper, b, m, info, rep.csobj, x)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixsolve(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.spdmatrixsolve(a, n, isupper, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spdmatrixsolve(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
    rep = New densesolverreport()
    alglib.smp_spdmatrixsolve(a, n, isupper, b, info, rep.csobj, x)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskysolvem(ByVal cha(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal b(,) As Double, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.spdmatrixcholeskysolvem(cha, n, isupper, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskysolve(ByVal cha(,) As Double, ByVal n As Integer, ByVal isupper As Boolean, ByVal b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.spdmatrixcholeskysolve(cha, n, isupper, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hpdmatrixsolvem(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal b(,) As alglib.complex, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.hpdmatrixsolvem(a, n, isupper, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_hpdmatrixsolvem(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal b(,) As alglib.complex, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
    rep = New densesolverreport()
    alglib.smp_hpdmatrixsolvem(a, n, isupper, b, m, info, rep.csobj, x)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hpdmatrixsolve(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.hpdmatrixsolve(a, n, isupper, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_hpdmatrixsolve(ByVal a(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
    rep = New densesolverreport()
    alglib.smp_hpdmatrixsolve(a, n, isupper, b, info, rep.csobj, x)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hpdmatrixcholeskysolvem(ByVal cha(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal b(,) As alglib.complex, ByVal m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.hpdmatrixcholeskysolvem(cha, n, isupper, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hpdmatrixcholeskysolve(ByVal cha(,) As alglib.complex, ByVal n As Integer, ByVal isupper As Boolean, ByVal b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.hpdmatrixcholeskysolve(cha, n, isupper, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixsolvels(ByVal a(,) As Double, ByVal nrows As Integer, ByVal ncols As Integer, ByVal b() As Double, ByVal threshold As Double, ByRef info As Integer, ByRef rep As densesolverlsreport, ByRef x() As Double)
        Try
            rep = New densesolverlsreport()
            alglib.rmatrixsolvels(a, nrows, ncols, b, threshold, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_rmatrixsolvels(ByVal a(,) As Double, ByVal nrows As Integer, ByVal ncols As Integer, ByVal b() As Double, ByVal threshold As Double, ByRef info As Integer, ByRef rep As densesolverlsreport, ByRef x() As Double)
        Try
    rep = New densesolverlsreport()
    alglib.smp_rmatrixsolvels(a, nrows, ncols, b, threshold, info, rep.csobj, x)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class logitmodel
        Public csobj As alglib.logitmodel
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'MNLReport structure contains information about training process:
    '* NGrad     -   number of gradient calculations
    '* NHess     -   number of Hessian calculations
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mnlreport
        Public Property ngrad() As Integer
        Get
            Return csobj.ngrad
        End Get
        Set(ByVal Value As Integer)
            csobj.ngrad = Value
        End Set
        End Property
        Public Property nhess() As Integer
        Get
            Return csobj.nhess
        End Get
        Set(ByVal Value As Integer)
            csobj.nhess = Value
        End Set
        End Property
        Public csobj As alglib.mnlreport
    End Class


    Public Sub mnltrainh(ByVal xy(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByVal nclasses As Integer, ByRef info As Integer, ByRef lm As logitmodel, ByRef rep As mnlreport)
        Try
            lm = New logitmodel()
            rep = New mnlreport()
            alglib.mnltrainh(xy, npoints, nvars, nclasses, info, lm.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mnlprocess(ByVal lm As logitmodel, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.mnlprocess(lm.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mnlprocessi(ByVal lm As logitmodel, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.mnlprocessi(lm.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mnlunpack(ByVal lm As logitmodel, ByRef a(,) As Double, ByRef nvars As Integer, ByRef nclasses As Integer)
        Try
            alglib.mnlunpack(lm.csobj, a, nvars, nclasses)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mnlpack(ByVal a(,) As Double, ByVal nvars As Integer, ByVal nclasses As Integer, ByRef lm As logitmodel)
        Try
            lm = New logitmodel()
            alglib.mnlpack(a, nvars, nclasses, lm.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function mnlavgce(ByVal lm As logitmodel, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mnlavgce = alglib.mnlavgce(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mnlrelclserror(ByVal lm As logitmodel, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mnlrelclserror = alglib.mnlrelclserror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mnlrmserror(ByVal lm As logitmodel, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mnlrmserror = alglib.mnlrmserror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mnlavgerror(ByVal lm As logitmodel, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mnlavgerror = alglib.mnlavgerror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mnlavgrelerror(ByVal lm As logitmodel, ByVal xy(,) As Double, ByVal ssize As Integer) As Double
        Try
            mnlavgrelerror = alglib.mnlavgrelerror(lm.csobj, xy, ssize)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mnlclserror(ByVal lm As logitmodel, ByVal xy(,) As Double, ByVal npoints As Integer) As Integer
        Try
            mnlclserror = alglib.mnlclserror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
















    Public Class mincgstate
        Public csobj As alglib.mincgstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure stores optimization report:
    '* IterationsCount           total number of inner iterations
    '* NFEV                      number of gradient evaluations
    '* TerminationType           termination type (see below)
    '
    'TERMINATION CODES
    '
    'TerminationType field contains completion code, which can be:
    '  -8    internal integrity control detected  infinite  or  NAN  values  in
    '        function/gradient. Abnormal termination signalled.
    '  -7    gradient verification failed.
    '        See MinCGSetGradientCheck() for more information.
    '   1    relative function improvement is no more than EpsF.
    '   2    relative step is no more than EpsX.
    '   4    gradient norm is no more than EpsG
    '   5    MaxIts steps was taken
    '   7    stopping conditions are too stringent,
    '        further improvement is impossible,
    '        X contains best point found so far.
    '   8    terminated by user who called mincgrequesttermination(). X contains
    '        point which was "current accepted" when  termination  request  was
    '        submitted.
    '
    'Other fields of this structure are not documented and should not be used!
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mincgreport
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property nfev() As Integer
        Get
            Return csobj.nfev
        End Get
        Set(ByVal Value As Integer)
            csobj.nfev = Value
        End Set
        End Property
        Public Property varidx() As Integer
        Get
            Return csobj.varidx
        End Get
        Set(ByVal Value As Integer)
            csobj.varidx = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public csobj As alglib.mincgreport
    End Class


    Public Sub mincgcreate(ByVal n As Integer, ByVal x() As Double, ByRef state As mincgstate)
        Try
            state = New mincgstate()
            alglib.mincgcreate(n, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgcreate(ByVal x() As Double, ByRef state As mincgstate)
        Try
            state = New mincgstate()
            alglib.mincgcreate(x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgcreatef(ByVal n As Integer, ByVal x() As Double, ByVal diffstep As Double, ByRef state As mincgstate)
        Try
            state = New mincgstate()
            alglib.mincgcreatef(n, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgcreatef(ByVal x() As Double, ByVal diffstep As Double, ByRef state As mincgstate)
        Try
            state = New mincgstate()
            alglib.mincgcreatef(x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsetcond(ByVal state As mincgstate, ByVal epsg As Double, ByVal epsf As Double, ByVal epsx As Double, ByVal maxits As Integer)
        Try
            alglib.mincgsetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsetscale(ByVal state As mincgstate, ByVal s() As Double)
        Try
            alglib.mincgsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsetxrep(ByVal state As mincgstate, ByVal needxrep As Boolean)
        Try
            alglib.mincgsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsetcgtype(ByVal state As mincgstate, ByVal cgtype As Integer)
        Try
            alglib.mincgsetcgtype(state.csobj, cgtype)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsetstpmax(ByVal state As mincgstate, ByVal stpmax As Double)
        Try
            alglib.mincgsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsuggeststep(ByVal state As mincgstate, ByVal stp As Double)
        Try
            alglib.mincgsuggeststep(state.csobj, stp)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsetprecdefault(ByVal state As mincgstate)
        Try
            alglib.mincgsetprecdefault(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsetprecdiag(ByVal state As mincgstate, ByVal d() As Double)
        Try
            alglib.mincgsetprecdiag(state.csobj, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsetprecscale(ByVal state As mincgstate)
        Try
            alglib.mincgsetprecscale(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function mincgiteration(ByVal state As mincgstate) As Boolean
        Try
            mincgiteration = alglib.mincgiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. This function has two different implementations: one which  uses  exact
    '    (analytical) user-supplied  gradient, and one which uses function value
    '    only  and  numerically  differentiates  function  in  order  to  obtain
    '    gradient.
    ' 
    '    Depending  on  the  specific  function  used to create optimizer object
    '    (either MinCGCreate()  for analytical gradient  or  MinCGCreateF()  for
    '    numerical differentiation) you should  choose  appropriate  variant  of
    '    MinCGOptimize() - one which accepts function AND gradient or one  which
    '    accepts function ONLY.
    ' 
    '    Be careful to choose variant of MinCGOptimize()  which  corresponds  to
    '    your optimization scheme! Table below lists different  combinations  of
    '    callback (function/gradient) passed  to  MinCGOptimize()  and  specific
    '    function used to create optimizer.
    ' 
    ' 
    '                   |         USER PASSED TO MinCGOptimize()
    '    CREATED WITH   |  function only   |  function and gradient
    '    ------------------------------------------------------------
    '    MinCGCreateF() |     work                FAIL
    '    MinCGCreate()  |     FAIL                work
    ' 
    '    Here "FAIL" denotes inappropriate combinations  of  optimizer  creation
    '    function and MinCGOptimize() version. Attemps to use  such  combination
    '    (for  example,  to create optimizer with  MinCGCreateF()  and  to  pass
    '    gradient information to MinCGOptimize()) will lead to  exception  being
    '    thrown. Either  you  did  not  pass  gradient when it WAS needed or you
    '    passed gradient when it was NOT needed.
    ' 
    '   -- ALGLIB --
    '      Copyright 20.04.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub mincgoptimize(ByVal state As mincgstate, ByVal func As ndimensional_func, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.mincg.mincgstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'mincgoptimize()' (func is null)")
        End If
        Try
            While alglib.mincg.mincgiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'mincgoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub mincgoptimize(ByVal state As mincgstate, ByVal grad As ndimensional_grad, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.mincg.mincgstate = state.csobj.innerobj
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'mincgoptimize()' (grad is null)")
        End If
        Try
            While alglib.mincg.mincgiteration(innerobj)
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'mincgoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub




    Public Sub mincgresults(ByVal state As mincgstate, ByRef x() As Double, ByRef rep As mincgreport)
        Try
            rep = New mincgreport()
            alglib.mincgresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgresultsbuf(ByVal state As mincgstate, ByRef x() As Double, ByRef rep As mincgreport)
        Try
            alglib.mincgresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgrestartfrom(ByVal state As mincgstate, ByVal x() As Double)
        Try
            alglib.mincgrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgrequesttermination(ByVal state As mincgstate)
        Try
            alglib.mincgrequesttermination(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mincgsetgradientcheck(ByVal state As mincgstate, ByVal teststep As Double)
        Try
            alglib.mincgsetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class minbleicstate
        Public csobj As alglib.minbleicstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure stores optimization report:
    '* IterationsCount           number of iterations
    '* NFEV                      number of gradient evaluations
    '* TerminationType           termination type (see below)
    '
    'TERMINATION CODES
    '
    'TerminationType field contains completion code, which can be:
    '  -8    internal integrity control detected  infinite  or  NAN  values  in
    '        function/gradient. Abnormal termination signalled.
    '  -7    gradient verification failed.
    '        See MinBLEICSetGradientCheck() for more information.
    '  -3    inconsistent constraints. Feasible point is
    '        either nonexistent or too hard to find. Try to
    '        restart optimizer with better initial approximation
    '   1    relative function improvement is no more than EpsF.
    '   2    relative step is no more than EpsX.
    '   4    gradient norm is no more than EpsG
    '   5    MaxIts steps was taken
    '   7    stopping conditions are too stringent,
    '        further improvement is impossible,
    '        X contains best point found so far.
    '   8    terminated by user who called minbleicrequesttermination(). X contains
    '        point which was "current accepted" when  termination  request  was
    '        submitted.
    '
    'ADDITIONAL FIELDS
    '
    'There are additional fields which can be used for debugging:
    '* DebugEqErr                error in the equality constraints (2-norm)
    '* DebugFS                   f, calculated at projection of initial point
    '                            to the feasible set
    '* DebugFF                   f, calculated at the final point
    '* DebugDX                   |X_start-X_final|
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minbleicreport
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property nfev() As Integer
        Get
            Return csobj.nfev
        End Get
        Set(ByVal Value As Integer)
            csobj.nfev = Value
        End Set
        End Property
        Public Property varidx() As Integer
        Get
            Return csobj.varidx
        End Get
        Set(ByVal Value As Integer)
            csobj.varidx = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public Property debugeqerr() As Double
        Get
            Return csobj.debugeqerr
        End Get
        Set(ByVal Value As Double)
            csobj.debugeqerr = Value
        End Set
        End Property
        Public Property debugfs() As Double
        Get
            Return csobj.debugfs
        End Get
        Set(ByVal Value As Double)
            csobj.debugfs = Value
        End Set
        End Property
        Public Property debugff() As Double
        Get
            Return csobj.debugff
        End Get
        Set(ByVal Value As Double)
            csobj.debugff = Value
        End Set
        End Property
        Public Property debugdx() As Double
        Get
            Return csobj.debugdx
        End Get
        Set(ByVal Value As Double)
            csobj.debugdx = Value
        End Set
        End Property
        Public Property debugfeasqpits() As Integer
        Get
            Return csobj.debugfeasqpits
        End Get
        Set(ByVal Value As Integer)
            csobj.debugfeasqpits = Value
        End Set
        End Property
        Public Property debugfeasgpaits() As Integer
        Get
            Return csobj.debugfeasgpaits
        End Get
        Set(ByVal Value As Integer)
            csobj.debugfeasgpaits = Value
        End Set
        End Property
        Public Property inneriterationscount() As Integer
        Get
            Return csobj.inneriterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.inneriterationscount = Value
        End Set
        End Property
        Public Property outeriterationscount() As Integer
        Get
            Return csobj.outeriterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.outeriterationscount = Value
        End Set
        End Property
        Public csobj As alglib.minbleicreport
    End Class


    Public Sub minbleiccreate(ByVal n As Integer, ByVal x() As Double, ByRef state As minbleicstate)
        Try
            state = New minbleicstate()
            alglib.minbleiccreate(n, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleiccreate(ByVal x() As Double, ByRef state As minbleicstate)
        Try
            state = New minbleicstate()
            alglib.minbleiccreate(x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleiccreatef(ByVal n As Integer, ByVal x() As Double, ByVal diffstep As Double, ByRef state As minbleicstate)
        Try
            state = New minbleicstate()
            alglib.minbleiccreatef(n, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleiccreatef(ByVal x() As Double, ByVal diffstep As Double, ByRef state As minbleicstate)
        Try
            state = New minbleicstate()
            alglib.minbleiccreatef(x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetbc(ByVal state As minbleicstate, ByVal bndl() As Double, ByVal bndu() As Double)
        Try
            alglib.minbleicsetbc(state.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetlc(ByVal state As minbleicstate, ByVal c(,) As Double, ByVal ct() As Integer, ByVal k As Integer)
        Try
            alglib.minbleicsetlc(state.csobj, c, ct, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetlc(ByVal state As minbleicstate, ByVal c(,) As Double, ByVal ct() As Integer)
        Try
            alglib.minbleicsetlc(state.csobj, c, ct)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetcond(ByVal state As minbleicstate, ByVal epsg As Double, ByVal epsf As Double, ByVal epsx As Double, ByVal maxits As Integer)
        Try
            alglib.minbleicsetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetscale(ByVal state As minbleicstate, ByVal s() As Double)
        Try
            alglib.minbleicsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetprecdefault(ByVal state As minbleicstate)
        Try
            alglib.minbleicsetprecdefault(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetprecdiag(ByVal state As minbleicstate, ByVal d() As Double)
        Try
            alglib.minbleicsetprecdiag(state.csobj, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetprecscale(ByVal state As minbleicstate)
        Try
            alglib.minbleicsetprecscale(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetxrep(ByVal state As minbleicstate, ByVal needxrep As Boolean)
        Try
            alglib.minbleicsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetstpmax(ByVal state As minbleicstate, ByVal stpmax As Double)
        Try
            alglib.minbleicsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function minbleiciteration(ByVal state As minbleicstate) As Boolean
        Try
            minbleiciteration = alglib.minbleiciteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. This function has two different implementations: one which  uses  exact
    '    (analytical) user-supplied gradient,  and one which uses function value
    '    only  and  numerically  differentiates  function  in  order  to  obtain
    '    gradient.
    ' 
    '    Depending  on  the  specific  function  used to create optimizer object
    '    (either  MinBLEICCreate() for analytical gradient or  MinBLEICCreateF()
    '    for numerical differentiation) you should choose appropriate variant of
    '    MinBLEICOptimize() - one  which  accepts  function  AND gradient or one
    '    which accepts function ONLY.
    ' 
    '    Be careful to choose variant of MinBLEICOptimize() which corresponds to
    '    your optimization scheme! Table below lists different  combinations  of
    '    callback (function/gradient) passed to MinBLEICOptimize()  and specific
    '    function used to create optimizer.
    ' 
    ' 
    '                      |         USER PASSED TO MinBLEICOptimize()
    '    CREATED WITH      |  function only   |  function and gradient
    '    ------------------------------------------------------------
    '    MinBLEICCreateF() |     work                FAIL
    '    MinBLEICCreate()  |     FAIL                work
    ' 
    '    Here "FAIL" denotes inappropriate combinations  of  optimizer  creation
    '    function  and  MinBLEICOptimize()  version.   Attemps   to   use   such
    '    combination (for  example,  to  create optimizer with MinBLEICCreateF()
    '    and  to  pass  gradient  information  to  MinCGOptimize()) will lead to
    '    exception being thrown. Either  you  did  not pass gradient when it WAS
    '    needed or you passed gradient when it was NOT needed.
    ' 
    '   -- ALGLIB --
    '      Copyright 28.11.2010 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub minbleicoptimize(ByVal state As minbleicstate, ByVal func As ndimensional_func, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minbleic.minbleicstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minbleicoptimize()' (func is null)")
        End If
        Try
            While alglib.minbleic.minbleiciteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minbleicoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub minbleicoptimize(ByVal state As minbleicstate, ByVal grad As ndimensional_grad, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minbleic.minbleicstate = state.csobj.innerobj
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minbleicoptimize()' (grad is null)")
        End If
        Try
            While alglib.minbleic.minbleiciteration(innerobj)
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minbleicoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub




    Public Sub minbleicresults(ByVal state As minbleicstate, ByRef x() As Double, ByRef rep As minbleicreport)
        Try
            rep = New minbleicreport()
            alglib.minbleicresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicresultsbuf(ByVal state As minbleicstate, ByRef x() As Double, ByRef rep As minbleicreport)
        Try
            alglib.minbleicresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicrestartfrom(ByVal state As minbleicstate, ByVal x() As Double)
        Try
            alglib.minbleicrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicrequesttermination(ByVal state As minbleicstate)
        Try
            alglib.minbleicrequesttermination(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetgradientcheck(ByVal state As minbleicstate, ByVal teststep As Double)
        Try
            alglib.minbleicsetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class mcpdstate
        Public csobj As alglib.mcpdstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure is a MCPD training report:
    '    InnerIterationsCount    -   number of inner iterations of the
    '                                underlying optimization algorithm
    '    OuterIterationsCount    -   number of outer iterations of the
    '                                underlying optimization algorithm
    '    NFEV                    -   number of merit function evaluations
    '    TerminationType         -   termination type
    '                                (same as for MinBLEIC optimizer, positive
    '                                values denote success, negative ones -
    '                                failure)
    '
    '  -- ALGLIB --
    '     Copyright 23.05.2010 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mcpdreport
        Public Property inneriterationscount() As Integer
        Get
            Return csobj.inneriterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.inneriterationscount = Value
        End Set
        End Property
        Public Property outeriterationscount() As Integer
        Get
            Return csobj.outeriterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.outeriterationscount = Value
        End Set
        End Property
        Public Property nfev() As Integer
        Get
            Return csobj.nfev
        End Get
        Set(ByVal Value As Integer)
            csobj.nfev = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public csobj As alglib.mcpdreport
    End Class


    Public Sub mcpdcreate(ByVal n As Integer, ByRef s As mcpdstate)
        Try
            s = New mcpdstate()
            alglib.mcpdcreate(n, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdcreateentry(ByVal n As Integer, ByVal entrystate As Integer, ByRef s As mcpdstate)
        Try
            s = New mcpdstate()
            alglib.mcpdcreateentry(n, entrystate, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdcreateexit(ByVal n As Integer, ByVal exitstate As Integer, ByRef s As mcpdstate)
        Try
            s = New mcpdstate()
            alglib.mcpdcreateexit(n, exitstate, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdcreateentryexit(ByVal n As Integer, ByVal entrystate As Integer, ByVal exitstate As Integer, ByRef s As mcpdstate)
        Try
            s = New mcpdstate()
            alglib.mcpdcreateentryexit(n, entrystate, exitstate, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdaddtrack(ByVal s As mcpdstate, ByVal xy(,) As Double, ByVal k As Integer)
        Try
            alglib.mcpdaddtrack(s.csobj, xy, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdaddtrack(ByVal s As mcpdstate, ByVal xy(,) As Double)
        Try
            alglib.mcpdaddtrack(s.csobj, xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdsetec(ByVal s As mcpdstate, ByVal ec(,) As Double)
        Try
            alglib.mcpdsetec(s.csobj, ec)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdaddec(ByVal s As mcpdstate, ByVal i As Integer, ByVal j As Integer, ByVal c As Double)
        Try
            alglib.mcpdaddec(s.csobj, i, j, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdsetbc(ByVal s As mcpdstate, ByVal bndl(,) As Double, ByVal bndu(,) As Double)
        Try
            alglib.mcpdsetbc(s.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdaddbc(ByVal s As mcpdstate, ByVal i As Integer, ByVal j As Integer, ByVal bndl As Double, ByVal bndu As Double)
        Try
            alglib.mcpdaddbc(s.csobj, i, j, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdsetlc(ByVal s As mcpdstate, ByVal c(,) As Double, ByVal ct() As Integer, ByVal k As Integer)
        Try
            alglib.mcpdsetlc(s.csobj, c, ct, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdsetlc(ByVal s As mcpdstate, ByVal c(,) As Double, ByVal ct() As Integer)
        Try
            alglib.mcpdsetlc(s.csobj, c, ct)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdsettikhonovregularizer(ByVal s As mcpdstate, ByVal v As Double)
        Try
            alglib.mcpdsettikhonovregularizer(s.csobj, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdsetprior(ByVal s As mcpdstate, ByVal pp(,) As Double)
        Try
            alglib.mcpdsetprior(s.csobj, pp)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdsetpredictionweights(ByVal s As mcpdstate, ByVal pw() As Double)
        Try
            alglib.mcpdsetpredictionweights(s.csobj, pw)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdsolve(ByVal s As mcpdstate)
        Try
            alglib.mcpdsolve(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mcpdresults(ByVal s As mcpdstate, ByRef p(,) As Double, ByRef rep As mcpdreport)
        Try
            rep = New mcpdreport()
            alglib.mcpdresults(s.csobj, p, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class mlpensemble
        Public csobj As alglib.mlpensemble
    End Class
    Public Sub mlpeserialize(ByVal obj As mlpensemble, ByRef s_out As String)
        Try
            alglib.mlpeserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Sub mlpeunserialize(ByVal s_in As String, ByRef obj As mlpensemble)
        Try
            alglib.mlpeunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreate0(ByVal nin As Integer, ByVal nout As Integer, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreate0(nin, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreate1(ByVal nin As Integer, ByVal nhid As Integer, ByVal nout As Integer, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreate1(nin, nhid, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreate2(ByVal nin As Integer, ByVal nhid1 As Integer, ByVal nhid2 As Integer, ByVal nout As Integer, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreate2(nin, nhid1, nhid2, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreateb0(ByVal nin As Integer, ByVal nout As Integer, ByVal b As Double, ByVal d As Double, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreateb0(nin, nout, b, d, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreateb1(ByVal nin As Integer, ByVal nhid As Integer, ByVal nout As Integer, ByVal b As Double, ByVal d As Double, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreateb1(nin, nhid, nout, b, d, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreateb2(ByVal nin As Integer, ByVal nhid1 As Integer, ByVal nhid2 As Integer, ByVal nout As Integer, ByVal b As Double, ByVal d As Double, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreateb2(nin, nhid1, nhid2, nout, b, d, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreater0(ByVal nin As Integer, ByVal nout As Integer, ByVal a As Double, ByVal b As Double, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreater0(nin, nout, a, b, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreater1(ByVal nin As Integer, ByVal nhid As Integer, ByVal nout As Integer, ByVal a As Double, ByVal b As Double, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreater1(nin, nhid, nout, a, b, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreater2(ByVal nin As Integer, ByVal nhid1 As Integer, ByVal nhid2 As Integer, ByVal nout As Integer, ByVal a As Double, ByVal b As Double, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreater2(nin, nhid1, nhid2, nout, a, b, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreatec0(ByVal nin As Integer, ByVal nout As Integer, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreatec0(nin, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreatec1(ByVal nin As Integer, ByVal nhid As Integer, ByVal nout As Integer, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreatec1(nin, nhid, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreatec2(ByVal nin As Integer, ByVal nhid1 As Integer, ByVal nhid2 As Integer, ByVal nout As Integer, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreatec2(nin, nhid1, nhid2, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpecreatefromnetwork(ByVal network As multilayerperceptron, ByVal ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreatefromnetwork(network.csobj, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlperandomize(ByVal ensemble As mlpensemble)
        Try
            alglib.mlperandomize(ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpeproperties(ByVal ensemble As mlpensemble, ByRef nin As Integer, ByRef nout As Integer)
        Try
            alglib.mlpeproperties(ensemble.csobj, nin, nout)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function mlpeissoftmax(ByVal ensemble As mlpensemble) As Boolean
        Try
            mlpeissoftmax = alglib.mlpeissoftmax(ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub mlpeprocess(ByVal ensemble As mlpensemble, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.mlpeprocess(ensemble.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpeprocessi(ByVal ensemble As mlpensemble, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.mlpeprocessi(ensemble.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function mlperelclserror(ByVal ensemble As mlpensemble, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlperelclserror = alglib.mlperelclserror(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpeavgce(ByVal ensemble As mlpensemble, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlpeavgce = alglib.mlpeavgce(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpermserror(ByVal ensemble As mlpensemble, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlpermserror = alglib.mlpermserror(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpeavgerror(ByVal ensemble As mlpensemble, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlpeavgerror = alglib.mlpeavgerror(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function mlpeavgrelerror(ByVal ensemble As mlpensemble, ByVal xy(,) As Double, ByVal npoints As Integer) As Double
        Try
            mlpeavgrelerror = alglib.mlpeavgrelerror(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function

    Public Class minlbfgsstate
        Public csobj As alglib.minlbfgsstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure stores optimization report:
    '* IterationsCount           total number of inner iterations
    '* NFEV                      number of gradient evaluations
    '* TerminationType           termination type (see below)
    '
    'TERMINATION CODES
    '
    'TerminationType field contains completion code, which can be:
    '  -8    internal integrity control detected  infinite  or  NAN  values  in
    '        function/gradient. Abnormal termination signalled.
    '  -7    gradient verification failed.
    '        See MinLBFGSSetGradientCheck() for more information.
    '   1    relative function improvement is no more than EpsF.
    '   2    relative step is no more than EpsX.
    '   4    gradient norm is no more than EpsG
    '   5    MaxIts steps was taken
    '   7    stopping conditions are too stringent,
    '        further improvement is impossible,
    '        X contains best point found so far.
    '   8    terminated    by  user  who  called  minlbfgsrequesttermination().
    '        X contains point which was   "current accepted"  when  termination
    '        request was submitted.
    '
    'Other fields of this structure are not documented and should not be used!
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minlbfgsreport
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property nfev() As Integer
        Get
            Return csobj.nfev
        End Get
        Set(ByVal Value As Integer)
            csobj.nfev = Value
        End Set
        End Property
        Public Property varidx() As Integer
        Get
            Return csobj.varidx
        End Get
        Set(ByVal Value As Integer)
            csobj.varidx = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public csobj As alglib.minlbfgsreport
    End Class


    Public Sub minlbfgscreate(ByVal n As Integer, ByVal m As Integer, ByVal x() As Double, ByRef state As minlbfgsstate)
        Try
            state = New minlbfgsstate()
            alglib.minlbfgscreate(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgscreate(ByVal m As Integer, ByVal x() As Double, ByRef state As minlbfgsstate)
        Try
            state = New minlbfgsstate()
            alglib.minlbfgscreate(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgscreatef(ByVal n As Integer, ByVal m As Integer, ByVal x() As Double, ByVal diffstep As Double, ByRef state As minlbfgsstate)
        Try
            state = New minlbfgsstate()
            alglib.minlbfgscreatef(n, m, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgscreatef(ByVal m As Integer, ByVal x() As Double, ByVal diffstep As Double, ByRef state As minlbfgsstate)
        Try
            state = New minlbfgsstate()
            alglib.minlbfgscreatef(m, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetcond(ByVal state As minlbfgsstate, ByVal epsg As Double, ByVal epsf As Double, ByVal epsx As Double, ByVal maxits As Integer)
        Try
            alglib.minlbfgssetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetxrep(ByVal state As minlbfgsstate, ByVal needxrep As Boolean)
        Try
            alglib.minlbfgssetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetstpmax(ByVal state As minlbfgsstate, ByVal stpmax As Double)
        Try
            alglib.minlbfgssetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetscale(ByVal state As minlbfgsstate, ByVal s() As Double)
        Try
            alglib.minlbfgssetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetprecdefault(ByVal state As minlbfgsstate)
        Try
            alglib.minlbfgssetprecdefault(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetpreccholesky(ByVal state As minlbfgsstate, ByVal p(,) As Double, ByVal isupper As Boolean)
        Try
            alglib.minlbfgssetpreccholesky(state.csobj, p, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetprecdiag(ByVal state As minlbfgsstate, ByVal d() As Double)
        Try
            alglib.minlbfgssetprecdiag(state.csobj, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetprecscale(ByVal state As minlbfgsstate)
        Try
            alglib.minlbfgssetprecscale(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function minlbfgsiteration(ByVal state As minlbfgsstate) As Boolean
        Try
            minlbfgsiteration = alglib.minlbfgsiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. This function has two different implementations: one which  uses  exact
    '    (analytical) user-supplied gradient,  and one which uses function value
    '    only  and  numerically  differentiates  function  in  order  to  obtain
    '    gradient.
    ' 
    '    Depending  on  the  specific  function  used to create optimizer object
    '    (either MinLBFGSCreate() for analytical gradient  or  MinLBFGSCreateF()
    '    for numerical differentiation) you should choose appropriate variant of
    '    MinLBFGSOptimize() - one  which  accepts  function  AND gradient or one
    '    which accepts function ONLY.
    ' 
    '    Be careful to choose variant of MinLBFGSOptimize() which corresponds to
    '    your optimization scheme! Table below lists different  combinations  of
    '    callback (function/gradient) passed to MinLBFGSOptimize()  and specific
    '    function used to create optimizer.
    ' 
    ' 
    '                      |         USER PASSED TO MinLBFGSOptimize()
    '    CREATED WITH      |  function only   |  function and gradient
    '    ------------------------------------------------------------
    '    MinLBFGSCreateF() |     work                FAIL
    '    MinLBFGSCreate()  |     FAIL                work
    ' 
    '    Here "FAIL" denotes inappropriate combinations  of  optimizer  creation
    '    function  and  MinLBFGSOptimize()  version.   Attemps   to   use   such
    '    combination (for example, to create optimizer with MinLBFGSCreateF() and
    '    to pass gradient information to MinCGOptimize()) will lead to exception
    '    being thrown. Either  you  did  not pass gradient when it WAS needed or
    '    you passed gradient when it was NOT needed.
    ' 
    '   -- ALGLIB --
    '      Copyright 20.03.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub minlbfgsoptimize(ByVal state As minlbfgsstate, ByVal func As ndimensional_func, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minlbfgs.minlbfgsstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlbfgsoptimize()' (func is null)")
        End If
        Try
            While alglib.minlbfgs.minlbfgsiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlbfgsoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub minlbfgsoptimize(ByVal state As minlbfgsstate, ByVal grad As ndimensional_grad, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minlbfgs.minlbfgsstate = state.csobj.innerobj
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlbfgsoptimize()' (grad is null)")
        End If
        Try
            While alglib.minlbfgs.minlbfgsiteration(innerobj)
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlbfgsoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub




    Public Sub minlbfgsresults(ByVal state As minlbfgsstate, ByRef x() As Double, ByRef rep As minlbfgsreport)
        Try
            rep = New minlbfgsreport()
            alglib.minlbfgsresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgsresultsbuf(ByVal state As minlbfgsstate, ByRef x() As Double, ByRef rep As minlbfgsreport)
        Try
            alglib.minlbfgsresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgsrestartfrom(ByVal state As minlbfgsstate, ByVal x() As Double)
        Try
            alglib.minlbfgsrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgsrequesttermination(ByVal state As minlbfgsstate)
        Try
            alglib.minlbfgsrequesttermination(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetgradientcheck(ByVal state As minlbfgsstate, ByVal teststep As Double)
        Try
            alglib.minlbfgssetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Training report:
    '    * RelCLSError   -   fraction of misclassified cases.
    '    * AvgCE         -   acerage cross-entropy
    '    * RMSError      -   root-mean-square error
    '    * AvgError      -   average error
    '    * AvgRelError   -   average relative error
    '    * NGrad         -   number of gradient calculations
    '    * NHess         -   number of Hessian calculations
    '    * NCholesky     -   number of Cholesky decompositions
    '
    'NOTE 1: RelCLSError/AvgCE are zero on regression problems.
    '
    'NOTE 2: on classification problems  RMSError/AvgError/AvgRelError  contain
    '        errors in prediction of posterior probabilities
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mlpreport
        Public Property relclserror() As Double
        Get
            Return csobj.relclserror
        End Get
        Set(ByVal Value As Double)
            csobj.relclserror = Value
        End Set
        End Property
        Public Property avgce() As Double
        Get
            Return csobj.avgce
        End Get
        Set(ByVal Value As Double)
            csobj.avgce = Value
        End Set
        End Property
        Public Property rmserror() As Double
        Get
            Return csobj.rmserror
        End Get
        Set(ByVal Value As Double)
            csobj.rmserror = Value
        End Set
        End Property
        Public Property avgerror() As Double
        Get
            Return csobj.avgerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgerror = Value
        End Set
        End Property
        Public Property avgrelerror() As Double
        Get
            Return csobj.avgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgrelerror = Value
        End Set
        End Property
        Public Property ngrad() As Integer
        Get
            Return csobj.ngrad
        End Get
        Set(ByVal Value As Integer)
            csobj.ngrad = Value
        End Set
        End Property
        Public Property nhess() As Integer
        Get
            Return csobj.nhess
        End Get
        Set(ByVal Value As Integer)
            csobj.nhess = Value
        End Set
        End Property
        Public Property ncholesky() As Integer
        Get
            Return csobj.ncholesky
        End Get
        Set(ByVal Value As Integer)
            csobj.ncholesky = Value
        End Set
        End Property
        Public csobj As alglib.mlpreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Cross-validation estimates of generalization error
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mlpcvreport
        Public Property relclserror() As Double
        Get
            Return csobj.relclserror
        End Get
        Set(ByVal Value As Double)
            csobj.relclserror = Value
        End Set
        End Property
        Public Property avgce() As Double
        Get
            Return csobj.avgce
        End Get
        Set(ByVal Value As Double)
            csobj.avgce = Value
        End Set
        End Property
        Public Property rmserror() As Double
        Get
            Return csobj.rmserror
        End Get
        Set(ByVal Value As Double)
            csobj.rmserror = Value
        End Set
        End Property
        Public Property avgerror() As Double
        Get
            Return csobj.avgerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgerror = Value
        End Set
        End Property
        Public Property avgrelerror() As Double
        Get
            Return csobj.avgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgrelerror = Value
        End Set
        End Property
        Public csobj As alglib.mlpcvreport
    End Class
    Public Class mlptrainer
        Public csobj As alglib.mlptrainer
    End Class


    Public Sub mlptrainlm(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer, ByVal decay As Double, ByVal restarts As Integer, ByRef info As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlptrainlm(network.csobj, xy, npoints, decay, restarts, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlptrainlbfgs(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer, ByVal decay As Double, ByVal restarts As Integer, ByVal wstep As Double, ByVal maxits As Integer, ByRef info As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlptrainlbfgs(network.csobj, xy, npoints, decay, restarts, wstep, maxits, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlptraines(ByVal network As multilayerperceptron, ByVal trnxy(,) As Double, ByVal trnsize As Integer, ByVal valxy(,) As Double, ByVal valsize As Integer, ByVal decay As Double, ByVal restarts As Integer, ByRef info As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlptraines(network.csobj, trnxy, trnsize, valxy, valsize, decay, restarts, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpkfoldcvlbfgs(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer, ByVal decay As Double, ByVal restarts As Integer, ByVal wstep As Double, ByVal maxits As Integer, ByVal foldscount As Integer, ByRef info As Integer, ByRef rep As mlpreport, ByRef cvrep As mlpcvreport)
        Try
            rep = New mlpreport()
            cvrep = New mlpcvreport()
            alglib.mlpkfoldcvlbfgs(network.csobj, xy, npoints, decay, restarts, wstep, maxits, foldscount, info, rep.csobj, cvrep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpkfoldcvlm(ByVal network As multilayerperceptron, ByVal xy(,) As Double, ByVal npoints As Integer, ByVal decay As Double, ByVal restarts As Integer, ByVal foldscount As Integer, ByRef info As Integer, ByRef rep As mlpreport, ByRef cvrep As mlpcvreport)
        Try
            rep = New mlpreport()
            cvrep = New mlpcvreport()
            alglib.mlpkfoldcvlm(network.csobj, xy, npoints, decay, restarts, foldscount, info, rep.csobj, cvrep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpkfoldcv(ByVal s As mlptrainer, ByVal network As multilayerperceptron, ByVal nrestarts As Integer, ByVal foldscount As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlpkfoldcv(s.csobj, network.csobj, nrestarts, foldscount, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_mlpkfoldcv(ByVal s As mlptrainer, ByVal network As multilayerperceptron, ByVal nrestarts As Integer, ByVal foldscount As Integer, ByRef rep As mlpreport)
        Try
    rep = New mlpreport()
    alglib.smp_mlpkfoldcv(s.csobj, network.csobj, nrestarts, foldscount, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreatetrainer(ByVal nin As Integer, ByVal nout As Integer, ByRef s As mlptrainer)
        Try
            s = New mlptrainer()
            alglib.mlpcreatetrainer(nin, nout, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpcreatetrainercls(ByVal nin As Integer, ByVal nclasses As Integer, ByRef s As mlptrainer)
        Try
            s = New mlptrainer()
            alglib.mlpcreatetrainercls(nin, nclasses, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpsetdataset(ByVal s As mlptrainer, ByVal xy(,) As Double, ByVal npoints As Integer)
        Try
            alglib.mlpsetdataset(s.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpsetsparsedataset(ByVal s As mlptrainer, ByVal xy As sparsematrix, ByVal npoints As Integer)
        Try
            alglib.mlpsetsparsedataset(s.csobj, xy.csobj, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpsetdecay(ByVal s As mlptrainer, ByVal decay As Double)
        Try
            alglib.mlpsetdecay(s.csobj, decay)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpsetcond(ByVal s As mlptrainer, ByVal wstep As Double, ByVal maxits As Integer)
        Try
            alglib.mlpsetcond(s.csobj, wstep, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpsetalgobatch(ByVal s As mlptrainer)
        Try
            alglib.mlpsetalgobatch(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlptrainnetwork(ByVal s As mlptrainer, ByVal network As multilayerperceptron, ByVal nrestarts As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlptrainnetwork(s.csobj, network.csobj, nrestarts, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_mlptrainnetwork(ByVal s As mlptrainer, ByVal network As multilayerperceptron, ByVal nrestarts As Integer, ByRef rep As mlpreport)
        Try
    rep = New mlpreport()
    alglib.smp_mlptrainnetwork(s.csobj, network.csobj, nrestarts, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpstarttraining(ByVal s As mlptrainer, ByVal network As multilayerperceptron, ByVal randomstart As Boolean)
        Try
            alglib.mlpstarttraining(s.csobj, network.csobj, randomstart)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function mlpcontinuetraining(ByVal s As mlptrainer, ByVal network As multilayerperceptron) As Boolean
        Try
            mlpcontinuetraining = alglib.mlpcontinuetraining(s.csobj, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    Public Function smp_mlpcontinuetraining(ByVal s As mlptrainer, ByVal network As multilayerperceptron) As Boolean
        Try
    smp_mlpcontinuetraining = alglib.smp_mlpcontinuetraining(s.csobj, network.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub mlpebagginglm(ByVal ensemble As mlpensemble, ByVal xy(,) As Double, ByVal npoints As Integer, ByVal decay As Double, ByVal restarts As Integer, ByRef info As Integer, ByRef rep As mlpreport, ByRef ooberrors As mlpcvreport)
        Try
            rep = New mlpreport()
            ooberrors = New mlpcvreport()
            alglib.mlpebagginglm(ensemble.csobj, xy, npoints, decay, restarts, info, rep.csobj, ooberrors.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpebagginglbfgs(ByVal ensemble As mlpensemble, ByVal xy(,) As Double, ByVal npoints As Integer, ByVal decay As Double, ByVal restarts As Integer, ByVal wstep As Double, ByVal maxits As Integer, ByRef info As Integer, ByRef rep As mlpreport, ByRef ooberrors As mlpcvreport)
        Try
            rep = New mlpreport()
            ooberrors = New mlpcvreport()
            alglib.mlpebagginglbfgs(ensemble.csobj, xy, npoints, decay, restarts, wstep, maxits, info, rep.csobj, ooberrors.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlpetraines(ByVal ensemble As mlpensemble, ByVal xy(,) As Double, ByVal npoints As Integer, ByVal decay As Double, ByVal restarts As Integer, ByRef info As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlpetraines(ensemble.csobj, xy, npoints, decay, restarts, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub mlptrainensemblees(ByVal s As mlptrainer, ByVal ensemble As mlpensemble, ByVal nrestarts As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlptrainensemblees(s.csobj, ensemble.csobj, nrestarts, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_mlptrainensemblees(ByVal s As mlptrainer, ByVal ensemble As mlpensemble, ByVal nrestarts As Integer, ByRef rep As mlpreport)
        Try
    rep = New mlpreport()
    alglib.smp_mlptrainensemblees(s.csobj, ensemble.csobj, nrestarts, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub pcabuildbasis(ByVal x(,) As Double, ByVal npoints As Integer, ByVal nvars As Integer, ByRef info As Integer, ByRef s2() As Double, ByRef v(,) As Double)
        Try
            alglib.pcabuildbasis(x, npoints, nvars, info, s2, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class odesolverstate
        Public csobj As alglib.odesolverstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class odesolverreport
        Public Property nfev() As Integer
        Get
            Return csobj.nfev
        End Get
        Set(ByVal Value As Integer)
            csobj.nfev = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public csobj As alglib.odesolverreport
    End Class


    Public Sub odesolverrkck(ByVal y() As Double, ByVal n As Integer, ByVal x() As Double, ByVal m As Integer, ByVal eps As Double, ByVal h As Double, ByRef state As odesolverstate)
        Try
            state = New odesolverstate()
            alglib.odesolverrkck(y, n, x, m, eps, h, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub odesolverrkck(ByVal y() As Double, ByVal x() As Double, ByVal eps As Double, ByVal h As Double, ByRef state As odesolverstate)
        Try
            state = New odesolverstate()
            alglib.odesolverrkck(y, x, eps, h, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function odesolveriteration(ByVal state As odesolverstate) As Boolean
        Try
            odesolveriteration = alglib.odesolveriteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This function is used to launcn iterations of ODE solver
    '
    ' It accepts following parameters:
    '     diff    -   callback which calculates dy/dx for given y and x
    '     obj     -   optional object which is passed to diff; can be NULL
    '
    ' 
    '   -- ALGLIB --
    '      Copyright 01.09.2009 by Bochkanov Sergey
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''/
    Public Sub odesolversolve(state As odesolverstate, diff As ndimensional_ode_rp, obj As Object)
        If diff Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'odesolversolve()' (diff is null)")
        End If
        Dim innerobj As alglib.odesolver.odesolverstate = state.csobj.innerobj
        Try
            While alglib.odesolver.odesolveriteration(innerobj)
                If innerobj.needdy Then
                    diff(innerobj.y, innerobj.x, innerobj.dy, obj)
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: unexpected error in 'odesolversolve'")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub




    Public Sub odesolverresults(ByVal state As odesolverstate, ByRef m As Integer, ByRef xtbl() As Double, ByRef ytbl(,) As Double, ByRef rep As odesolverreport)
        Try
            rep = New odesolverreport()
            alglib.odesolverresults(state.csobj, m, xtbl, ytbl, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub fftc1d(ByRef a() As alglib.complex, ByVal n As Integer)
        Try
            alglib.fftc1d(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fftc1d(ByRef a() As alglib.complex)
        Try
            alglib.fftc1d(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fftc1dinv(ByRef a() As alglib.complex, ByVal n As Integer)
        Try
            alglib.fftc1dinv(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fftc1dinv(ByRef a() As alglib.complex)
        Try
            alglib.fftc1dinv(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fftr1d(ByVal a() As Double, ByVal n As Integer, ByRef f() As alglib.complex)
        Try
            alglib.fftr1d(a, n, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fftr1d(ByVal a() As Double, ByRef f() As alglib.complex)
        Try
            alglib.fftr1d(a, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fftr1dinv(ByVal f() As alglib.complex, ByVal n As Integer, ByRef a() As Double)
        Try
            alglib.fftr1dinv(f, n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fftr1dinv(ByVal f() As alglib.complex, ByRef a() As Double)
        Try
            alglib.fftr1dinv(f, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub convc1d(ByVal a() As alglib.complex, ByVal m As Integer, ByVal b() As alglib.complex, ByVal n As Integer, ByRef r() As alglib.complex)
        Try
            alglib.convc1d(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub convc1dinv(ByVal a() As alglib.complex, ByVal m As Integer, ByVal b() As alglib.complex, ByVal n As Integer, ByRef r() As alglib.complex)
        Try
            alglib.convc1dinv(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub convc1dcircular(ByVal s() As alglib.complex, ByVal m As Integer, ByVal r() As alglib.complex, ByVal n As Integer, ByRef c() As alglib.complex)
        Try
            alglib.convc1dcircular(s, m, r, n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub convc1dcircularinv(ByVal a() As alglib.complex, ByVal m As Integer, ByVal b() As alglib.complex, ByVal n As Integer, ByRef r() As alglib.complex)
        Try
            alglib.convc1dcircularinv(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub convr1d(ByVal a() As Double, ByVal m As Integer, ByVal b() As Double, ByVal n As Integer, ByRef r() As Double)
        Try
            alglib.convr1d(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub convr1dinv(ByVal a() As Double, ByVal m As Integer, ByVal b() As Double, ByVal n As Integer, ByRef r() As Double)
        Try
            alglib.convr1dinv(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub convr1dcircular(ByVal s() As Double, ByVal m As Integer, ByVal r() As Double, ByVal n As Integer, ByRef c() As Double)
        Try
            alglib.convr1dcircular(s, m, r, n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub convr1dcircularinv(ByVal a() As Double, ByVal m As Integer, ByVal b() As Double, ByVal n As Integer, ByRef r() As Double)
        Try
            alglib.convr1dcircularinv(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub corrc1d(ByVal signal() As alglib.complex, ByVal n As Integer, ByVal pattern() As alglib.complex, ByVal m As Integer, ByRef r() As alglib.complex)
        Try
            alglib.corrc1d(signal, n, pattern, m, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub corrc1dcircular(ByVal signal() As alglib.complex, ByVal m As Integer, ByVal pattern() As alglib.complex, ByVal n As Integer, ByRef c() As alglib.complex)
        Try
            alglib.corrc1dcircular(signal, m, pattern, n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub corrr1d(ByVal signal() As Double, ByVal n As Integer, ByVal pattern() As Double, ByVal m As Integer, ByRef r() As Double)
        Try
            alglib.corrr1d(signal, n, pattern, m, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub corrr1dcircular(ByVal signal() As Double, ByVal m As Integer, ByVal pattern() As Double, ByVal n As Integer, ByRef c() As Double)
        Try
            alglib.corrr1dcircular(signal, m, pattern, n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub fhtr1d(ByRef a() As Double, ByVal n As Integer)
        Try
            alglib.fhtr1d(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fhtr1dinv(ByRef a() As Double, ByVal n As Integer)
        Try
            alglib.fhtr1dinv(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub gqgeneraterec(ByVal alpha() As Double, ByVal beta() As Double, ByVal mu0 As Double, ByVal n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgeneraterec(alpha, beta, mu0, n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gqgenerategausslobattorec(ByVal alpha() As Double, ByVal beta() As Double, ByVal mu0 As Double, ByVal a As Double, ByVal b As Double, ByVal n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategausslobattorec(alpha, beta, mu0, a, b, n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gqgenerategaussradaurec(ByVal alpha() As Double, ByVal beta() As Double, ByVal mu0 As Double, ByVal a As Double, ByVal n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategaussradaurec(alpha, beta, mu0, a, n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gqgenerategausslegendre(ByVal n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategausslegendre(n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gqgenerategaussjacobi(ByVal n As Integer, ByVal alpha As Double, ByVal beta As Double, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategaussjacobi(n, alpha, beta, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gqgenerategausslaguerre(ByVal n As Integer, ByVal alpha As Double, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategausslaguerre(n, alpha, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gqgenerategausshermite(ByVal n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategausshermite(n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub gkqgeneraterec(ByVal alpha() As Double, ByVal beta() As Double, ByVal mu0 As Double, ByVal n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double)
        Try
            alglib.gkqgeneraterec(alpha, beta, mu0, n, info, x, wkronrod, wgauss)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gkqgenerategausslegendre(ByVal n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double)
        Try
            alglib.gkqgenerategausslegendre(n, info, x, wkronrod, wgauss)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gkqgenerategaussjacobi(ByVal n As Integer, ByVal alpha As Double, ByVal beta As Double, ByRef info As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double)
        Try
            alglib.gkqgenerategaussjacobi(n, alpha, beta, info, x, wkronrod, wgauss)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gkqlegendrecalc(ByVal n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double)
        Try
            alglib.gkqlegendrecalc(n, info, x, wkronrod, wgauss)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub gkqlegendretbl(ByVal n As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double, ByRef eps As Double)
        Try
            alglib.gkqlegendretbl(n, x, wkronrod, wgauss, eps)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Integration report:
    '* TerminationType = completetion code:
    '    * -5    non-convergence of Gauss-Kronrod nodes
    '            calculation subroutine.
    '    * -1    incorrect parameters were specified
    '    *  1    OK
    '* Rep.NFEV countains number of function calculations
    '* Rep.NIntervals contains number of intervals [a,b]
    '  was partitioned into.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class autogkreport
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public Property nfev() As Integer
        Get
            Return csobj.nfev
        End Get
        Set(ByVal Value As Integer)
            csobj.nfev = Value
        End Set
        End Property
        Public Property nintervals() As Integer
        Get
            Return csobj.nintervals
        End Get
        Set(ByVal Value As Integer)
            csobj.nintervals = Value
        End Set
        End Property
        Public csobj As alglib.autogkreport
    End Class
    Public Class autogkstate
        Public csobj As alglib.autogkstate
    End Class


    Public Sub autogksmooth(ByVal a As Double, ByVal b As Double, ByRef state As autogkstate)
        Try
            state = New autogkstate()
            alglib.autogksmooth(a, b, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub autogksmoothw(ByVal a As Double, ByVal b As Double, ByVal xwidth As Double, ByRef state As autogkstate)
        Try
            state = New autogkstate()
            alglib.autogksmoothw(a, b, xwidth, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub autogksingular(ByVal a As Double, ByVal b As Double, ByVal alpha As Double, ByVal beta As Double, ByRef state As autogkstate)
        Try
            state = New autogkstate()
            alglib.autogksingular(a, b, alpha, beta, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function autogkiteration(ByVal state As autogkstate) As Boolean
        Try
            autogkiteration = alglib.autogkiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This function is used to launcn iterations of ODE solver
    '
    ' It accepts following parameters:
    '     diff    -   callback which calculates dy/dx for given y and x
    '     obj     -   optional object which is passed to diff; can be NULL
    '
    ' 
    '   -- ALGLIB --
    '      Copyright 07.05.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub autogkintegrate(state As autogkstate, func As integrator1_func, obj As Object)
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'autogkintegrate()' (func is null)")
        End If
        Dim innerobj As alglib.autogk.autogkstate = state.csobj.innerobj
        Try
            While alglib.autogk.autogkiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x, innerobj.xminusa, innerobj.bminusx, innerobj.f, obj)
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: unexpected error in 'autogksolve'")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub autogkresults(ByVal state As autogkstate, ByRef v As Double, ByRef rep As autogkreport)
        Try
            rep = New autogkreport()
            alglib.autogkresults(state.csobj, v, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class idwinterpolant
        Public csobj As alglib.idwinterpolant
    End Class


    Public Function idwcalc(ByVal z As idwinterpolant, ByVal x() As Double) As Double
        Try
            idwcalc = alglib.idwcalc(z.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub idwbuildmodifiedshepard(ByVal xy(,) As Double, ByVal n As Integer, ByVal nx As Integer, ByVal d As Integer, ByVal nq As Integer, ByVal nw As Integer, ByRef z As idwinterpolant)
        Try
            z = New idwinterpolant()
            alglib.idwbuildmodifiedshepard(xy, n, nx, d, nq, nw, z.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub idwbuildmodifiedshepardr(ByVal xy(,) As Double, ByVal n As Integer, ByVal nx As Integer, ByVal r As Double, ByRef z As idwinterpolant)
        Try
            z = New idwinterpolant()
            alglib.idwbuildmodifiedshepardr(xy, n, nx, r, z.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub idwbuildnoisy(ByVal xy(,) As Double, ByVal n As Integer, ByVal nx As Integer, ByVal d As Integer, ByVal nq As Integer, ByVal nw As Integer, ByRef z As idwinterpolant)
        Try
            z = New idwinterpolant()
            alglib.idwbuildnoisy(xy, n, nx, d, nq, nw, z.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class barycentricinterpolant
        Public csobj As alglib.barycentricinterpolant
    End Class


    Public Function barycentriccalc(ByVal b As barycentricinterpolant, ByVal t As Double) As Double
        Try
            barycentriccalc = alglib.barycentriccalc(b.csobj, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub barycentricdiff1(ByVal b As barycentricinterpolant, ByVal t As Double, ByRef f As Double, ByRef df As Double)
        Try
            alglib.barycentricdiff1(b.csobj, t, f, df)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub barycentricdiff2(ByVal b As barycentricinterpolant, ByVal t As Double, ByRef f As Double, ByRef df As Double, ByRef d2f As Double)
        Try
            alglib.barycentricdiff2(b.csobj, t, f, df, d2f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub barycentriclintransx(ByVal b As barycentricinterpolant, ByVal ca As Double, ByVal cb As Double)
        Try
            alglib.barycentriclintransx(b.csobj, ca, cb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub barycentriclintransy(ByVal b As barycentricinterpolant, ByVal ca As Double, ByVal cb As Double)
        Try
            alglib.barycentriclintransy(b.csobj, ca, cb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub barycentricunpack(ByVal b As barycentricinterpolant, ByRef n As Integer, ByRef x() As Double, ByRef y() As Double, ByRef w() As Double)
        Try
            alglib.barycentricunpack(b.csobj, n, x, y, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub barycentricbuildxyw(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByRef b As barycentricinterpolant)
        Try
            b = New barycentricinterpolant()
            alglib.barycentricbuildxyw(x, y, w, n, b.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub barycentricbuildfloaterhormann(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal d As Integer, ByRef b As barycentricinterpolant)
        Try
            b = New barycentricinterpolant()
            alglib.barycentricbuildfloaterhormann(x, y, n, d, b.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub polynomialbar2cheb(ByVal p As barycentricinterpolant, ByVal a As Double, ByVal b As Double, ByRef t() As Double)
        Try
            alglib.polynomialbar2cheb(p.csobj, a, b, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialcheb2bar(ByVal t() As Double, ByVal n As Integer, ByVal a As Double, ByVal b As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialcheb2bar(t, n, a, b, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialcheb2bar(ByVal t() As Double, ByVal a As Double, ByVal b As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialcheb2bar(t, a, b, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbar2pow(ByVal p As barycentricinterpolant, ByVal c As Double, ByVal s As Double, ByRef a() As Double)
        Try
            alglib.polynomialbar2pow(p.csobj, c, s, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbar2pow(ByVal p As barycentricinterpolant, ByRef a() As Double)
        Try
            alglib.polynomialbar2pow(p.csobj, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialpow2bar(ByVal a() As Double, ByVal n As Integer, ByVal c As Double, ByVal s As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialpow2bar(a, n, c, s, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialpow2bar(ByVal a() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialpow2bar(a, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbuild(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuild(x, y, n, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbuild(ByVal x() As Double, ByVal y() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuild(x, y, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbuildeqdist(ByVal a As Double, ByVal b As Double, ByVal y() As Double, ByVal n As Integer, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildeqdist(a, b, y, n, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbuildeqdist(ByVal a As Double, ByVal b As Double, ByVal y() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildeqdist(a, b, y, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbuildcheb1(ByVal a As Double, ByVal b As Double, ByVal y() As Double, ByVal n As Integer, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildcheb1(a, b, y, n, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbuildcheb1(ByVal a As Double, ByVal b As Double, ByVal y() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildcheb1(a, b, y, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbuildcheb2(ByVal a As Double, ByVal b As Double, ByVal y() As Double, ByVal n As Integer, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildcheb2(a, b, y, n, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialbuildcheb2(ByVal a As Double, ByVal b As Double, ByVal y() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildcheb2(a, b, y, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function polynomialcalceqdist(ByVal a As Double, ByVal b As Double, ByVal f() As Double, ByVal n As Integer, ByVal t As Double) As Double
        Try
            polynomialcalceqdist = alglib.polynomialcalceqdist(a, b, f, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function polynomialcalceqdist(ByVal a As Double, ByVal b As Double, ByVal f() As Double, ByVal t As Double) As Double
        Try
            polynomialcalceqdist = alglib.polynomialcalceqdist(a, b, f, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function polynomialcalccheb1(ByVal a As Double, ByVal b As Double, ByVal f() As Double, ByVal n As Integer, ByVal t As Double) As Double
        Try
            polynomialcalccheb1 = alglib.polynomialcalccheb1(a, b, f, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function polynomialcalccheb1(ByVal a As Double, ByVal b As Double, ByVal f() As Double, ByVal t As Double) As Double
        Try
            polynomialcalccheb1 = alglib.polynomialcalccheb1(a, b, f, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function polynomialcalccheb2(ByVal a As Double, ByVal b As Double, ByVal f() As Double, ByVal n As Integer, ByVal t As Double) As Double
        Try
            polynomialcalccheb2 = alglib.polynomialcalccheb2(a, b, f, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function polynomialcalccheb2(ByVal a As Double, ByVal b As Double, ByVal f() As Double, ByVal t As Double) As Double
        Try
            polynomialcalccheb2 = alglib.polynomialcalccheb2(a, b, f, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function

    Public Class spline1dinterpolant
        Public csobj As alglib.spline1dinterpolant
    End Class


    Public Sub spline1dbuildlinear(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildlinear(x, y, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildlinear(ByVal x() As Double, ByVal y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildlinear(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildcubic(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal boundltype As Integer, ByVal boundl As Double, ByVal boundrtype As Integer, ByVal boundr As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildcubic(x, y, n, boundltype, boundl, boundrtype, boundr, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildcubic(ByVal x() As Double, ByVal y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildcubic(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dgriddiffcubic(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal boundltype As Integer, ByVal boundl As Double, ByVal boundrtype As Integer, ByVal boundr As Double, ByRef d() As Double)
        Try
            alglib.spline1dgriddiffcubic(x, y, n, boundltype, boundl, boundrtype, boundr, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dgriddiffcubic(ByVal x() As Double, ByVal y() As Double, ByRef d() As Double)
        Try
            alglib.spline1dgriddiffcubic(x, y, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dgriddiff2cubic(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal boundltype As Integer, ByVal boundl As Double, ByVal boundrtype As Integer, ByVal boundr As Double, ByRef d1() As Double, ByRef d2() As Double)
        Try
            alglib.spline1dgriddiff2cubic(x, y, n, boundltype, boundl, boundrtype, boundr, d1, d2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dgriddiff2cubic(ByVal x() As Double, ByVal y() As Double, ByRef d1() As Double, ByRef d2() As Double)
        Try
            alglib.spline1dgriddiff2cubic(x, y, d1, d2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dconvcubic(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal boundltype As Integer, ByVal boundl As Double, ByVal boundrtype As Integer, ByVal boundr As Double, ByVal x2() As Double, ByVal n2 As Integer, ByRef y2() As Double)
        Try
            alglib.spline1dconvcubic(x, y, n, boundltype, boundl, boundrtype, boundr, x2, n2, y2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dconvcubic(ByVal x() As Double, ByVal y() As Double, ByVal x2() As Double, ByRef y2() As Double)
        Try
            alglib.spline1dconvcubic(x, y, x2, y2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dconvdiffcubic(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal boundltype As Integer, ByVal boundl As Double, ByVal boundrtype As Integer, ByVal boundr As Double, ByVal x2() As Double, ByVal n2 As Integer, ByRef y2() As Double, ByRef d2() As Double)
        Try
            alglib.spline1dconvdiffcubic(x, y, n, boundltype, boundl, boundrtype, boundr, x2, n2, y2, d2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dconvdiffcubic(ByVal x() As Double, ByVal y() As Double, ByVal x2() As Double, ByRef y2() As Double, ByRef d2() As Double)
        Try
            alglib.spline1dconvdiffcubic(x, y, x2, y2, d2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dconvdiff2cubic(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal boundltype As Integer, ByVal boundl As Double, ByVal boundrtype As Integer, ByVal boundr As Double, ByVal x2() As Double, ByVal n2 As Integer, ByRef y2() As Double, ByRef d2() As Double, ByRef dd2() As Double)
        Try
            alglib.spline1dconvdiff2cubic(x, y, n, boundltype, boundl, boundrtype, boundr, x2, n2, y2, d2, dd2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dconvdiff2cubic(ByVal x() As Double, ByVal y() As Double, ByVal x2() As Double, ByRef y2() As Double, ByRef d2() As Double, ByRef dd2() As Double)
        Try
            alglib.spline1dconvdiff2cubic(x, y, x2, y2, d2, dd2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildcatmullrom(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal boundtype As Integer, ByVal tension As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildcatmullrom(x, y, n, boundtype, tension, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildcatmullrom(ByVal x() As Double, ByVal y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildcatmullrom(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildhermite(ByVal x() As Double, ByVal y() As Double, ByVal d() As Double, ByVal n As Integer, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildhermite(x, y, d, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildhermite(ByVal x() As Double, ByVal y() As Double, ByVal d() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildhermite(x, y, d, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildakima(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildakima(x, y, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildakima(ByVal x() As Double, ByVal y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildakima(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function spline1dcalc(ByVal c As spline1dinterpolant, ByVal x As Double) As Double
        Try
            spline1dcalc = alglib.spline1dcalc(c.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub spline1ddiff(ByVal c As spline1dinterpolant, ByVal x As Double, ByRef s As Double, ByRef ds As Double, ByRef d2s As Double)
        Try
            alglib.spline1ddiff(c.csobj, x, s, ds, d2s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dunpack(ByVal c As spline1dinterpolant, ByRef n As Integer, ByRef tbl(,) As Double)
        Try
            alglib.spline1dunpack(c.csobj, n, tbl)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dlintransx(ByVal c As spline1dinterpolant, ByVal a As Double, ByVal b As Double)
        Try
            alglib.spline1dlintransx(c.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dlintransy(ByVal c As spline1dinterpolant, ByVal a As Double, ByVal b As Double)
        Try
            alglib.spline1dlintransy(c.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function spline1dintegrate(ByVal c As spline1dinterpolant, ByVal x As Double) As Double
        Try
            spline1dintegrate = alglib.spline1dintegrate(c.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub spline1dbuildmonotone(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildmonotone(x, y, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dbuildmonotone(ByVal x() As Double, ByVal y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildmonotone(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class normestimatorstate
        Public csobj As alglib.normestimatorstate
    End Class


    Public Sub normestimatorcreate(ByVal m As Integer, ByVal n As Integer, ByVal nstart As Integer, ByVal nits As Integer, ByRef state As normestimatorstate)
        Try
            state = New normestimatorstate()
            alglib.normestimatorcreate(m, n, nstart, nits, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub normestimatorsetseed(ByVal state As normestimatorstate, ByVal seedval As Integer)
        Try
            alglib.normestimatorsetseed(state.csobj, seedval)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub normestimatorestimatesparse(ByVal state As normestimatorstate, ByVal a As sparsematrix)
        Try
            alglib.normestimatorestimatesparse(state.csobj, a.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub normestimatorresults(ByVal state As normestimatorstate, ByRef nrm As Double)
        Try
            alglib.normestimatorresults(state.csobj, nrm)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub










    Public Class minqpstate
        Public csobj As alglib.minqpstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure stores optimization report:
    '* InnerIterationsCount      number of inner iterations
    '* OuterIterationsCount      number of outer iterations
    '* NCholesky                 number of Cholesky decomposition
    '* NMV                       number of matrix-vector products
    '                            (only products calculated as part of iterative
    '                            process are counted)
    '* TerminationType           completion code (see below)
    '
    'Completion codes:
    '* -5    inappropriate solver was used:
    '        * QuickQP solver for problem with general linear constraints
    '        * Cholesky solver for semidefinite or indefinite problems
    '        * Cholesky solver for problems with non-boundary constraints
    '* -4    BLEIC-QP or QuickQP solver found unconstrained direction
    '        of negative curvature (function is unbounded from
    '        below  even  under  constraints),  no  meaningful
    '        minimum can be found.
    '* -3    inconsistent constraints (or, maybe, feasible point is
    '        too hard to find). If you are sure that constraints are feasible,
    '        try to restart optimizer with better initial approximation.
    '* -1    solver error
    '*  1..4 successful completion
    '*  5    MaxIts steps was taken
    '*  7    stopping conditions are too stringent,
    '        further improvement is impossible,
    '        X contains best point found so far.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minqpreport
        Public Property inneriterationscount() As Integer
        Get
            Return csobj.inneriterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.inneriterationscount = Value
        End Set
        End Property
        Public Property outeriterationscount() As Integer
        Get
            Return csobj.outeriterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.outeriterationscount = Value
        End Set
        End Property
        Public Property nmv() As Integer
        Get
            Return csobj.nmv
        End Get
        Set(ByVal Value As Integer)
            csobj.nmv = Value
        End Set
        End Property
        Public Property ncholesky() As Integer
        Get
            Return csobj.ncholesky
        End Get
        Set(ByVal Value As Integer)
            csobj.ncholesky = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public csobj As alglib.minqpreport
    End Class


    Public Sub minqpcreate(ByVal n As Integer, ByRef state As minqpstate)
        Try
            state = New minqpstate()
            alglib.minqpcreate(n, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetlinearterm(ByVal state As minqpstate, ByVal b() As Double)
        Try
            alglib.minqpsetlinearterm(state.csobj, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetquadraticterm(ByVal state As minqpstate, ByVal a(,) As Double, ByVal isupper As Boolean)
        Try
            alglib.minqpsetquadraticterm(state.csobj, a, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetquadraticterm(ByVal state As minqpstate, ByVal a(,) As Double)
        Try
            alglib.minqpsetquadraticterm(state.csobj, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetquadratictermsparse(ByVal state As minqpstate, ByVal a As sparsematrix, ByVal isupper As Boolean)
        Try
            alglib.minqpsetquadratictermsparse(state.csobj, a.csobj, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetstartingpoint(ByVal state As minqpstate, ByVal x() As Double)
        Try
            alglib.minqpsetstartingpoint(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetorigin(ByVal state As minqpstate, ByVal xorigin() As Double)
        Try
            alglib.minqpsetorigin(state.csobj, xorigin)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetscale(ByVal state As minqpstate, ByVal s() As Double)
        Try
            alglib.minqpsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetalgocholesky(ByVal state As minqpstate)
        Try
            alglib.minqpsetalgocholesky(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetalgobleic(ByVal state As minqpstate, ByVal epsg As Double, ByVal epsf As Double, ByVal epsx As Double, ByVal maxits As Integer)
        Try
            alglib.minqpsetalgobleic(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetalgoquickqp(ByVal state As minqpstate, ByVal epsg As Double, ByVal epsf As Double, ByVal epsx As Double, ByVal maxouterits As Integer, ByVal usenewton As Boolean)
        Try
            alglib.minqpsetalgoquickqp(state.csobj, epsg, epsf, epsx, maxouterits, usenewton)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetbc(ByVal state As minqpstate, ByVal bndl() As Double, ByVal bndu() As Double)
        Try
            alglib.minqpsetbc(state.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetlc(ByVal state As minqpstate, ByVal c(,) As Double, ByVal ct() As Integer, ByVal k As Integer)
        Try
            alglib.minqpsetlc(state.csobj, c, ct, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpsetlc(ByVal state As minqpstate, ByVal c(,) As Double, ByVal ct() As Integer)
        Try
            alglib.minqpsetlc(state.csobj, c, ct)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpoptimize(ByVal state As minqpstate)
        Try
            alglib.minqpoptimize(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpresults(ByVal state As minqpstate, ByRef x() As Double, ByRef rep As minqpreport)
        Try
            rep = New minqpreport()
            alglib.minqpresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minqpresultsbuf(ByVal state As minqpstate, ByRef x() As Double, ByRef rep As minqpreport)
        Try
            alglib.minqpresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class minlmstate
        Public csobj As alglib.minlmstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Optimization report, filled by MinLMResults() function
    '
    'FIELDS:
    '* TerminationType, completetion code:
    '    * -7    derivative correctness check failed;
    '            see Rep.WrongNum, Rep.WrongI, Rep.WrongJ for
    '            more information.
    '    * -3    constraints are inconsistent
    '    *  1    relative function improvement is no more than
    '            EpsF.
    '    *  2    relative step is no more than EpsX.
    '    *  4    gradient is no more than EpsG.
    '    *  5    MaxIts steps was taken
    '    *  7    stopping conditions are too stringent,
    '            further improvement is impossible
    '    *  8    terminated   by  user  who  called  MinLMRequestTermination().
    '            X contains point which was "current accepted" when termination
    '            request was submitted.
    '* IterationsCount, contains iterations count
    '* NFunc, number of function calculations
    '* NJac, number of Jacobi matrix calculations
    '* NGrad, number of gradient calculations
    '* NHess, number of Hessian calculations
    '* NCholesky, number of Cholesky decomposition calculations
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minlmreport
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public Property funcidx() As Integer
        Get
            Return csobj.funcidx
        End Get
        Set(ByVal Value As Integer)
            csobj.funcidx = Value
        End Set
        End Property
        Public Property varidx() As Integer
        Get
            Return csobj.varidx
        End Get
        Set(ByVal Value As Integer)
            csobj.varidx = Value
        End Set
        End Property
        Public Property nfunc() As Integer
        Get
            Return csobj.nfunc
        End Get
        Set(ByVal Value As Integer)
            csobj.nfunc = Value
        End Set
        End Property
        Public Property njac() As Integer
        Get
            Return csobj.njac
        End Get
        Set(ByVal Value As Integer)
            csobj.njac = Value
        End Set
        End Property
        Public Property ngrad() As Integer
        Get
            Return csobj.ngrad
        End Get
        Set(ByVal Value As Integer)
            csobj.ngrad = Value
        End Set
        End Property
        Public Property nhess() As Integer
        Get
            Return csobj.nhess
        End Get
        Set(ByVal Value As Integer)
            csobj.nhess = Value
        End Set
        End Property
        Public Property ncholesky() As Integer
        Get
            Return csobj.ncholesky
        End Get
        Set(ByVal Value As Integer)
            csobj.ncholesky = Value
        End Set
        End Property
        Public csobj As alglib.minlmreport
    End Class


    Public Sub minlmcreatevj(ByVal n As Integer, ByVal m As Integer, ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatevj(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatevj(ByVal m As Integer, ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatevj(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatev(ByVal n As Integer, ByVal m As Integer, ByVal x() As Double, ByVal diffstep As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatev(n, m, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatev(ByVal m As Integer, ByVal x() As Double, ByVal diffstep As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatev(m, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatefgh(ByVal n As Integer, ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefgh(n, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatefgh(ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefgh(x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmsetcond(ByVal state As minlmstate, ByVal epsg As Double, ByVal epsf As Double, ByVal epsx As Double, ByVal maxits As Integer)
        Try
            alglib.minlmsetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmsetxrep(ByVal state As minlmstate, ByVal needxrep As Boolean)
        Try
            alglib.minlmsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmsetstpmax(ByVal state As minlmstate, ByVal stpmax As Double)
        Try
            alglib.minlmsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmsetscale(ByVal state As minlmstate, ByVal s() As Double)
        Try
            alglib.minlmsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmsetbc(ByVal state As minlmstate, ByVal bndl() As Double, ByVal bndu() As Double)
        Try
            alglib.minlmsetbc(state.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmsetacctype(ByVal state As minlmstate, ByVal acctype As Integer)
        Try
            alglib.minlmsetacctype(state.csobj, acctype)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function minlmiteration(ByVal state As minlmstate) As Boolean
        Try
            minlmiteration = alglib.minlmiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     hess    -   callback which calculates function (or merit function)
    '                 value func, gradient grad and Hessian hess at given point x
    '     fvec    -   callback which calculates function vector fi[]
    '                 at given point x
    '     jac     -   callback which calculates function vector fi[]
    '                 and Jacobian jac at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. Depending on function used to create state  structure,  this  algorithm
    '    may accept Jacobian and/or Hessian and/or gradient.  According  to  the
    '    said above, there ase several versions of this function,  which  accept
    '    different sets of callbacks.
    ' 
    '    This flexibility opens way to subtle errors - you may create state with
    '    MinLMCreateFGH() (optimization using Hessian), but call function  which
    '    does not accept Hessian. So when algorithm will request Hessian,  there
    '    will be no callback to call. In this case exception will be thrown.
    ' 
    '    Be careful to avoid such errors because there is no way to find them at
    '    compile time - you can see them at runtime only.
    ' 
    '   -- ALGLIB --
    '      Copyright 10.03.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub minlmoptimize(ByVal state As minlmstate, ByVal fvec As ndimensional_fvec, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If fvec Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (fvec is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needfi Then
                    fvec(innerobj.x, innerobj.fi, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub minlmoptimize(ByVal state As minlmstate, ByVal fvec As ndimensional_fvec, ByVal jac As ndimensional_jac, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If fvec Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (fvec is null)")
        End If
        If jac Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (jac is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needfi Then
                    fvec(innerobj.x, innerobj.fi, obj)
                    Continue While
                End If
                If innerobj.needfij Then
                    jac(innerobj.x, innerobj.fi, innerobj.j, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub minlmoptimize(ByVal state As minlmstate, ByVal func As ndimensional_func, ByVal grad As ndimensional_grad, ByVal hess As ndimensional_hess, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (func is null)")
        End If
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (grad is null)")
        End If
        If hess Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (hess is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.needfgh Then
                    hess(innerobj.x, innerobj.f, innerobj.g, innerobj.h, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub minlmoptimize(ByVal state As minlmstate, ByVal func As ndimensional_func, ByVal jac As ndimensional_jac, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (func is null)")
        End If
        If jac Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (jac is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfij Then
                    jac(innerobj.x, innerobj.fi, innerobj.j, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub minlmoptimize(ByVal state As minlmstate, ByVal func As ndimensional_func, ByVal grad As ndimensional_grad, ByVal jac As ndimensional_jac, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (func is null)")
        End If
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (grad is null)")
        End If
        If jac Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (jac is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.needfij Then
                    jac(innerobj.x, innerobj.fi, innerobj.j, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub




    Public Sub minlmresults(ByVal state As minlmstate, ByRef x() As Double, ByRef rep As minlmreport)
        Try
            rep = New minlmreport()
            alglib.minlmresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmresultsbuf(ByVal state As minlmstate, ByRef x() As Double, ByRef rep As minlmreport)
        Try
            alglib.minlmresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmrestartfrom(ByVal state As minlmstate, ByVal x() As Double)
        Try
            alglib.minlmrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmrequesttermination(ByVal state As minlmstate)
        Try
            alglib.minlmrequesttermination(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatevgj(ByVal n As Integer, ByVal m As Integer, ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatevgj(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatevgj(ByVal m As Integer, ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatevgj(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatefgj(ByVal n As Integer, ByVal m As Integer, ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefgj(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatefgj(ByVal m As Integer, ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefgj(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatefj(ByVal n As Integer, ByVal m As Integer, ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefj(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmcreatefj(ByVal m As Integer, ByVal x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefj(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlmsetgradientcheck(ByVal state As minlmstate, ByVal teststep As Double)
        Try
            alglib.minlmsetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Polynomial fitting report:
    '    TaskRCond       reciprocal of task's condition number
    '    RMSError        RMS error
    '    AvgError        average error
    '    AvgRelError     average relative error (for non-zero Y[I])
    '    MaxError        maximum error
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class polynomialfitreport
        Public Property taskrcond() As Double
        Get
            Return csobj.taskrcond
        End Get
        Set(ByVal Value As Double)
            csobj.taskrcond = Value
        End Set
        End Property
        Public Property rmserror() As Double
        Get
            Return csobj.rmserror
        End Get
        Set(ByVal Value As Double)
            csobj.rmserror = Value
        End Set
        End Property
        Public Property avgerror() As Double
        Get
            Return csobj.avgerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgerror = Value
        End Set
        End Property
        Public Property avgrelerror() As Double
        Get
            Return csobj.avgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgrelerror = Value
        End Set
        End Property
        Public Property maxerror() As Double
        Get
            Return csobj.maxerror
        End Get
        Set(ByVal Value As Double)
            csobj.maxerror = Value
        End Set
        End Property
        Public csobj As alglib.polynomialfitreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Barycentric fitting report:
    '    RMSError        RMS error
    '    AvgError        average error
    '    AvgRelError     average relative error (for non-zero Y[I])
    '    MaxError        maximum error
    '    TaskRCond       reciprocal of task's condition number
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class barycentricfitreport
        Public Property taskrcond() As Double
        Get
            Return csobj.taskrcond
        End Get
        Set(ByVal Value As Double)
            csobj.taskrcond = Value
        End Set
        End Property
        Public Property dbest() As Integer
        Get
            Return csobj.dbest
        End Get
        Set(ByVal Value As Integer)
            csobj.dbest = Value
        End Set
        End Property
        Public Property rmserror() As Double
        Get
            Return csobj.rmserror
        End Get
        Set(ByVal Value As Double)
            csobj.rmserror = Value
        End Set
        End Property
        Public Property avgerror() As Double
        Get
            Return csobj.avgerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgerror = Value
        End Set
        End Property
        Public Property avgrelerror() As Double
        Get
            Return csobj.avgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgrelerror = Value
        End Set
        End Property
        Public Property maxerror() As Double
        Get
            Return csobj.maxerror
        End Get
        Set(ByVal Value As Double)
            csobj.maxerror = Value
        End Set
        End Property
        Public csobj As alglib.barycentricfitreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Spline fitting report:
    '    RMSError        RMS error
    '    AvgError        average error
    '    AvgRelError     average relative error (for non-zero Y[I])
    '    MaxError        maximum error
    '
    'Fields  below are  filled  by   obsolete    functions   (Spline1DFitCubic,
    'Spline1DFitHermite). Modern fitting functions do NOT fill these fields:
    '    TaskRCond       reciprocal of task's condition number
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class spline1dfitreport
        Public Property taskrcond() As Double
        Get
            Return csobj.taskrcond
        End Get
        Set(ByVal Value As Double)
            csobj.taskrcond = Value
        End Set
        End Property
        Public Property rmserror() As Double
        Get
            Return csobj.rmserror
        End Get
        Set(ByVal Value As Double)
            csobj.rmserror = Value
        End Set
        End Property
        Public Property avgerror() As Double
        Get
            Return csobj.avgerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgerror = Value
        End Set
        End Property
        Public Property avgrelerror() As Double
        Get
            Return csobj.avgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgrelerror = Value
        End Set
        End Property
        Public Property maxerror() As Double
        Get
            Return csobj.maxerror
        End Get
        Set(ByVal Value As Double)
            csobj.maxerror = Value
        End Set
        End Property
        Public csobj As alglib.spline1dfitreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Least squares fitting report. This structure contains informational fields
    'which are set by fitting functions provided by this unit.
    '
    'Different functions initialize different sets of  fields,  so  you  should
    'read documentation on specific function you used in order  to  know  which
    'fields are initialized.
    '
    '    TaskRCond       reciprocal of task's condition number
    '    IterationsCount number of internal iterations
    '
    '    VarIdx          if user-supplied gradient contains errors  which  were
    '                    detected by nonlinear fitter, this  field  is  set  to
    '                    index  of  the  first  component  of gradient which is
    '                    suspected to be spoiled by bugs.
    '
    '    RMSError        RMS error
    '    AvgError        average error
    '    AvgRelError     average relative error (for non-zero Y[I])
    '    MaxError        maximum error
    '
    '    WRMSError       weighted RMS error
    '
    '    CovPar          covariance matrix for parameters, filled by some solvers
    '    ErrPar          vector of errors in parameters, filled by some solvers
    '    ErrCurve        vector of fit errors -  variability  of  the  best-fit
    '                    curve, filled by some solvers.
    '    Noise           vector of per-point noise estimates, filled by
    '                    some solvers.
    '    R2              coefficient of determination (non-weighted, non-adjusted),
    '                    filled by some solvers.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class lsfitreport
        Public Property taskrcond() As Double
        Get
            Return csobj.taskrcond
        End Get
        Set(ByVal Value As Double)
            csobj.taskrcond = Value
        End Set
        End Property
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property varidx() As Integer
        Get
            Return csobj.varidx
        End Get
        Set(ByVal Value As Integer)
            csobj.varidx = Value
        End Set
        End Property
        Public Property rmserror() As Double
        Get
            Return csobj.rmserror
        End Get
        Set(ByVal Value As Double)
            csobj.rmserror = Value
        End Set
        End Property
        Public Property avgerror() As Double
        Get
            Return csobj.avgerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgerror = Value
        End Set
        End Property
        Public Property avgrelerror() As Double
        Get
            Return csobj.avgrelerror
        End Get
        Set(ByVal Value As Double)
            csobj.avgrelerror = Value
        End Set
        End Property
        Public Property maxerror() As Double
        Get
            Return csobj.maxerror
        End Get
        Set(ByVal Value As Double)
            csobj.maxerror = Value
        End Set
        End Property
        Public Property wrmserror() As Double
        Get
            Return csobj.wrmserror
        End Get
        Set(ByVal Value As Double)
            csobj.wrmserror = Value
        End Set
        End Property
        Public Property covpar() As Double(,)
        Get
            Return csobj.covpar
        End Get
        Set(ByVal Value As Double(,))
            csobj.covpar = Value
        End Set
        End Property
        Public Property errpar() As Double()
        Get
            Return csobj.errpar
        End Get
        Set(ByVal Value As Double())
            csobj.errpar = Value
        End Set
        End Property
        Public Property errcurve() As Double()
        Get
            Return csobj.errcurve
        End Get
        Set(ByVal Value As Double())
            csobj.errcurve = Value
        End Set
        End Property
        Public Property noise() As Double()
        Get
            Return csobj.noise
        End Get
        Set(ByVal Value As Double())
            csobj.noise = Value
        End Set
        End Property
        Public Property r2() As Double
        Get
            Return csobj.r2
        End Get
        Set(ByVal Value As Double)
            csobj.r2 = Value
        End Set
        End Property
        Public csobj As alglib.lsfitreport
    End Class
    Public Class lsfitstate
        Public csobj As alglib.lsfitstate
    End Class


    Public Sub lstfitpiecewiselinearrdpfixed(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByRef x2() As Double, ByRef y2() As Double, ByRef nsections As Integer)
        Try
            alglib.lstfitpiecewiselinearrdpfixed(x, y, n, m, x2, y2, nsections)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lstfitpiecewiselinearrdp(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal eps As Double, ByRef x2() As Double, ByRef y2() As Double, ByRef nsections As Integer)
        Try
            alglib.lstfitpiecewiselinearrdp(x, y, n, eps, x2, y2, nsections)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialfit(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
            p = New barycentricinterpolant()
            rep = New polynomialfitreport()
            alglib.polynomialfit(x, y, n, m, info, p.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_polynomialfit(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
    p = New barycentricinterpolant()
    rep = New polynomialfitreport()
    alglib.smp_polynomialfit(x, y, n, m, info, p.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialfit(ByVal x() As Double, ByVal y() As Double, ByVal m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
            p = New barycentricinterpolant()
            rep = New polynomialfitreport()
            alglib.polynomialfit(x, y, m, info, p.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_polynomialfit(ByVal x() As Double, ByVal y() As Double, ByVal m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
    p = New barycentricinterpolant()
    rep = New polynomialfitreport()
    alglib.smp_polynomialfit(x, y, m, info, p.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialfitwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal k As Integer, ByVal m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
            p = New barycentricinterpolant()
            rep = New polynomialfitreport()
            alglib.polynomialfitwc(x, y, w, n, xc, yc, dc, k, m, info, p.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_polynomialfitwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal k As Integer, ByVal m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
    p = New barycentricinterpolant()
    rep = New polynomialfitreport()
    alglib.smp_polynomialfitwc(x, y, w, n, xc, yc, dc, k, m, info, p.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub polynomialfitwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
            p = New barycentricinterpolant()
            rep = New polynomialfitreport()
            alglib.polynomialfitwc(x, y, w, xc, yc, dc, m, info, p.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_polynomialfitwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
    p = New barycentricinterpolant()
    rep = New polynomialfitreport()
    alglib.smp_polynomialfitwc(x, y, w, xc, yc, dc, m, info, p.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function logisticcalc4(ByVal x As Double, ByVal a As Double, ByVal b As Double, ByVal c As Double, ByVal d As Double) As Double
        Try
            logisticcalc4 = alglib.logisticcalc4(x, a, b, c, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function logisticcalc5(ByVal x As Double, ByVal a As Double, ByVal b As Double, ByVal c As Double, ByVal d As Double, ByVal g As Double) As Double
        Try
            logisticcalc5 = alglib.logisticcalc5(x, a, b, c, d, g)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub logisticfit4(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByRef a As Double, ByRef b As Double, ByRef c As Double, ByRef d As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.logisticfit4(x, y, n, a, b, c, d, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub logisticfit4ec(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal cnstrleft As Double, ByVal cnstrright As Double, ByRef a As Double, ByRef b As Double, ByRef c As Double, ByRef d As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.logisticfit4ec(x, y, n, cnstrleft, cnstrright, a, b, c, d, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub logisticfit5(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByRef a As Double, ByRef b As Double, ByRef c As Double, ByRef d As Double, ByRef g As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.logisticfit5(x, y, n, a, b, c, d, g, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub logisticfit5ec(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal cnstrleft As Double, ByVal cnstrright As Double, ByRef a As Double, ByRef b As Double, ByRef c As Double, ByRef d As Double, ByRef g As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.logisticfit5ec(x, y, n, cnstrleft, cnstrright, a, b, c, d, g, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub logisticfit45x(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal cnstrleft As Double, ByVal cnstrright As Double, ByVal is4pl As Boolean, ByVal lambdav As Double, ByVal epsx As Double, ByVal rscnt As Integer, ByRef a As Double, ByRef b As Double, ByRef c As Double, ByRef d As Double, ByRef g As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.logisticfit45x(x, y, n, cnstrleft, cnstrright, is4pl, lambdav, epsx, rscnt, a, b, c, d, g, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub barycentricfitfloaterhormannwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal k As Integer, ByVal m As Integer, ByRef info As Integer, ByRef b As barycentricinterpolant, ByRef rep As barycentricfitreport)
        Try
            b = New barycentricinterpolant()
            rep = New barycentricfitreport()
            alglib.barycentricfitfloaterhormannwc(x, y, w, n, xc, yc, dc, k, m, info, b.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_barycentricfitfloaterhormannwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal k As Integer, ByVal m As Integer, ByRef info As Integer, ByRef b As barycentricinterpolant, ByRef rep As barycentricfitreport)
        Try
    b = New barycentricinterpolant()
    rep = New barycentricfitreport()
    alglib.smp_barycentricfitfloaterhormannwc(x, y, w, n, xc, yc, dc, k, m, info, b.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub barycentricfitfloaterhormann(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef b As barycentricinterpolant, ByRef rep As barycentricfitreport)
        Try
            b = New barycentricinterpolant()
            rep = New barycentricfitreport()
            alglib.barycentricfitfloaterhormann(x, y, n, m, info, b.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_barycentricfitfloaterhormann(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef b As barycentricinterpolant, ByRef rep As barycentricfitreport)
        Try
    b = New barycentricinterpolant()
    rep = New barycentricfitreport()
    alglib.smp_barycentricfitfloaterhormann(x, y, n, m, info, b.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfitpenalized(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByVal rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitpenalized(x, y, n, m, rho, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfitpenalized(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByVal rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfitpenalized(x, y, n, m, rho, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfitpenalized(ByVal x() As Double, ByVal y() As Double, ByVal m As Integer, ByVal rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitpenalized(x, y, m, rho, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfitpenalized(ByVal x() As Double, ByVal y() As Double, ByVal m As Integer, ByVal rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfitpenalized(x, y, m, rho, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfitpenalizedw(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal m As Integer, ByVal rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitpenalizedw(x, y, w, n, m, rho, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfitpenalizedw(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal m As Integer, ByVal rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfitpenalizedw(x, y, w, n, m, rho, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfitpenalizedw(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal m As Integer, ByVal rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitpenalizedw(x, y, w, m, rho, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfitpenalizedw(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal m As Integer, ByVal rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfitpenalizedw(x, y, w, m, rho, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfitcubicwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal k As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitcubicwc(x, y, w, n, xc, yc, dc, k, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfitcubicwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal k As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfitcubicwc(x, y, w, n, xc, yc, dc, k, m, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfitcubicwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitcubicwc(x, y, w, xc, yc, dc, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfitcubicwc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfitcubicwc(x, y, w, xc, yc, dc, m, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfithermitewc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal k As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfithermitewc(x, y, w, n, xc, yc, dc, k, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfithermitewc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal n As Integer, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal k As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfithermitewc(x, y, w, n, xc, yc, dc, k, m, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfithermitewc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfithermitewc(x, y, w, xc, yc, dc, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfithermitewc(ByVal x() As Double, ByVal y() As Double, ByVal w() As Double, ByVal xc() As Double, ByVal yc() As Double, ByVal dc() As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfithermitewc(x, y, w, xc, yc, dc, m, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfitcubic(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitcubic(x, y, n, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfitcubic(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfitcubic(x, y, n, m, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfitcubic(ByVal x() As Double, ByVal y() As Double, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitcubic(x, y, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfitcubic(ByVal x() As Double, ByVal y() As Double, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfitcubic(x, y, m, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfithermite(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfithermite(x, y, n, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfithermite(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfithermite(x, y, n, m, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline1dfithermite(ByVal x() As Double, ByVal y() As Double, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfithermite(x, y, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_spline1dfithermite(ByVal x() As Double, ByVal y() As Double, ByVal m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
    s = New spline1dinterpolant()
    rep = New spline1dfitreport()
    alglib.smp_spline1dfithermite(x, y, m, info, s.csobj, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitlinearw(ByVal y() As Double, ByVal w() As Double, ByVal fmatrix(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearw(y, w, fmatrix, n, m, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_lsfitlinearw(ByVal y() As Double, ByVal w() As Double, ByVal fmatrix(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
    rep = New lsfitreport()
    alglib.smp_lsfitlinearw(y, w, fmatrix, n, m, info, c, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitlinearw(ByVal y() As Double, ByVal w() As Double, ByVal fmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearw(y, w, fmatrix, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_lsfitlinearw(ByVal y() As Double, ByVal w() As Double, ByVal fmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
    rep = New lsfitreport()
    alglib.smp_lsfitlinearw(y, w, fmatrix, info, c, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitlinearwc(ByVal y() As Double, ByVal w() As Double, ByVal fmatrix(,) As Double, ByVal cmatrix(,) As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearwc(y, w, fmatrix, cmatrix, n, m, k, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_lsfitlinearwc(ByVal y() As Double, ByVal w() As Double, ByVal fmatrix(,) As Double, ByVal cmatrix(,) As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
    rep = New lsfitreport()
    alglib.smp_lsfitlinearwc(y, w, fmatrix, cmatrix, n, m, k, info, c, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitlinearwc(ByVal y() As Double, ByVal w() As Double, ByVal fmatrix(,) As Double, ByVal cmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearwc(y, w, fmatrix, cmatrix, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_lsfitlinearwc(ByVal y() As Double, ByVal w() As Double, ByVal fmatrix(,) As Double, ByVal cmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
    rep = New lsfitreport()
    alglib.smp_lsfitlinearwc(y, w, fmatrix, cmatrix, info, c, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitlinear(ByVal y() As Double, ByVal fmatrix(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinear(y, fmatrix, n, m, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_lsfitlinear(ByVal y() As Double, ByVal fmatrix(,) As Double, ByVal n As Integer, ByVal m As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
    rep = New lsfitreport()
    alglib.smp_lsfitlinear(y, fmatrix, n, m, info, c, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitlinear(ByVal y() As Double, ByVal fmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinear(y, fmatrix, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_lsfitlinear(ByVal y() As Double, ByVal fmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
    rep = New lsfitreport()
    alglib.smp_lsfitlinear(y, fmatrix, info, c, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitlinearc(ByVal y() As Double, ByVal fmatrix(,) As Double, ByVal cmatrix(,) As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearc(y, fmatrix, cmatrix, n, m, k, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_lsfitlinearc(ByVal y() As Double, ByVal fmatrix(,) As Double, ByVal cmatrix(,) As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
    rep = New lsfitreport()
    alglib.smp_lsfitlinearc(y, fmatrix, cmatrix, n, m, k, info, c, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitlinearc(ByVal y() As Double, ByVal fmatrix(,) As Double, ByVal cmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearc(y, fmatrix, cmatrix, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub
    Public Sub smp_lsfitlinearc(ByVal y() As Double, ByVal fmatrix(,) As Double, ByVal cmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
    rep = New lsfitreport()
    alglib.smp_lsfitlinearc(y, fmatrix, cmatrix, info, c, rep.csobj)

        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatewf(ByVal x(,) As Double, ByVal y() As Double, ByVal w() As Double, ByVal c() As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByVal diffstep As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewf(x, y, w, c, n, m, k, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatewf(ByVal x(,) As Double, ByVal y() As Double, ByVal w() As Double, ByVal c() As Double, ByVal diffstep As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewf(x, y, w, c, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatef(ByVal x(,) As Double, ByVal y() As Double, ByVal c() As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByVal diffstep As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatef(x, y, c, n, m, k, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatef(ByVal x(,) As Double, ByVal y() As Double, ByVal c() As Double, ByVal diffstep As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatef(x, y, c, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatewfg(ByVal x(,) As Double, ByVal y() As Double, ByVal w() As Double, ByVal c() As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByVal cheapfg As Boolean, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewfg(x, y, w, c, n, m, k, cheapfg, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatewfg(ByVal x(,) As Double, ByVal y() As Double, ByVal w() As Double, ByVal c() As Double, ByVal cheapfg As Boolean, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewfg(x, y, w, c, cheapfg, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatefg(ByVal x(,) As Double, ByVal y() As Double, ByVal c() As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByVal cheapfg As Boolean, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatefg(x, y, c, n, m, k, cheapfg, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatefg(ByVal x(,) As Double, ByVal y() As Double, ByVal c() As Double, ByVal cheapfg As Boolean, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatefg(x, y, c, cheapfg, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatewfgh(ByVal x(,) As Double, ByVal y() As Double, ByVal w() As Double, ByVal c() As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewfgh(x, y, w, c, n, m, k, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatewfgh(ByVal x(,) As Double, ByVal y() As Double, ByVal w() As Double, ByVal c() As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewfgh(x, y, w, c, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatefgh(ByVal x(,) As Double, ByVal y() As Double, ByVal c() As Double, ByVal n As Integer, ByVal m As Integer, ByVal k As Integer, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatefgh(x, y, c, n, m, k, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitcreatefgh(ByVal x(,) As Double, ByVal y() As Double, ByVal c() As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatefgh(x, y, c, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitsetcond(ByVal state As lsfitstate, ByVal epsf As Double, ByVal epsx As Double, ByVal maxits As Integer)
        Try
            alglib.lsfitsetcond(state.csobj, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitsetstpmax(ByVal state As lsfitstate, ByVal stpmax As Double)
        Try
            alglib.lsfitsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitsetxrep(ByVal state As lsfitstate, ByVal needxrep As Boolean)
        Try
            alglib.lsfitsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitsetscale(ByVal state As lsfitstate, ByVal s() As Double)
        Try
            alglib.lsfitsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitsetbc(ByVal state As lsfitstate, ByVal bndl() As Double, ByVal bndu() As Double)
        Try
            alglib.lsfitsetbc(state.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function lsfititeration(ByVal state As lsfitstate) As Boolean
        Try
            lsfititeration = alglib.lsfititeration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear fitter
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     hess    -   callback which calculates function (or merit function)
    '                 value func, gradient grad and Hessian hess at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. this algorithm is somewhat unusual because it works with  parameterized
    '    function f(C,X), where X is a function argument (we  have  many  points
    '    which are characterized by different  argument  values),  and  C  is  a
    '    parameter to fit.
    ' 
    '    For example, if we want to do linear fit by f(c0,c1,x) = c0*x+c1,  then
    '    x will be argument, and {c0,c1} will be parameters.
    ' 
    '    It is important to understand that this algorithm finds minimum in  the
    '    space of function PARAMETERS (not arguments), so it  needs  derivatives
    '    of f() with respect to C, not X.
    ' 
    '    In the example above it will need f=c0*x+c1 and {df/dc0,df/dc1} = {x,1}
    '    instead of {df/dx} = {c0}.
    ' 
    ' 2. Callback functions accept C as the first parameter, and X as the second
    ' 
    ' 3. If  state  was  created  with  LSFitCreateFG(),  algorithm  needs  just
    '    function   and   its   gradient,   but   if   state   was  created with
    '    LSFitCreateFGH(), algorithm will need function, gradient and Hessian.
    ' 
    '    According  to  the  said  above,  there  ase  several  versions of this
    '    function, which accept different sets of callbacks.
    ' 
    '    This flexibility opens way to subtle errors - you may create state with
    '    LSFitCreateFGH() (optimization using Hessian), but call function  which
    '    does not accept Hessian. So when algorithm will request Hessian,  there
    '    will be no callback to call. In this case exception will be thrown.
    ' 
    '    Be careful to avoid such errors because there is no way to find them at
    '    compile time - you can see them at runtime only.
    ' 
    '   -- ALGLIB --
    '      Copyright 17.08.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub lsfitfit(ByVal state As lsfitstate, ByVal func As ndimensional_pfunc, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.lsfit.lsfitstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (func is null)")
        End If
        Try
            While alglib.lsfit.lsfititeration(innerobj)
                If innerobj.needf Then
                    func(innerobj.c, innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.c, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'lsfitfit' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub lsfitfit(ByVal state As lsfitstate, ByVal func As ndimensional_pfunc, ByVal grad As ndimensional_pgrad, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.lsfit.lsfitstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (func is null)")
        End If
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (grad is null)")
        End If
        Try
            While alglib.lsfit.lsfititeration(innerobj)
                If innerobj.needf Then
                    func(innerobj.c, innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfg Then
                    grad(innerobj.c, innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.c, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'lsfitfit' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub lsfitfit(ByVal state As lsfitstate, ByVal func As ndimensional_pfunc, ByVal grad As ndimensional_pgrad, ByVal hess As ndimensional_phess, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.lsfit.lsfitstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (func is null)")
        End If
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (grad is null)")
        End If
        If hess Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (hess is null)")
        End If
        Try
            While alglib.lsfit.lsfititeration(innerobj)
                If innerobj.needf Then
                    func(innerobj.c, innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfg Then
                    grad(innerobj.c, innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.needfgh Then
                    hess(innerobj.c, innerobj.x, innerobj.f, innerobj.g, innerobj.h, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.c, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'lsfitfit' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub




    Public Sub lsfitresults(ByVal state As lsfitstate, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitresults(state.csobj, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lsfitsetgradientcheck(ByVal state As lsfitstate, ByVal teststep As Double)
        Try
            alglib.lsfitsetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class pspline2interpolant
        Public csobj As alglib.pspline2interpolant
    End Class
    Public Class pspline3interpolant
        Public csobj As alglib.pspline3interpolant
    End Class


    Public Sub pspline2build(ByVal xy(,) As Double, ByVal n As Integer, ByVal st As Integer, ByVal pt As Integer, ByRef p As pspline2interpolant)
        Try
            p = New pspline2interpolant()
            alglib.pspline2build(xy, n, st, pt, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline3build(ByVal xy(,) As Double, ByVal n As Integer, ByVal st As Integer, ByVal pt As Integer, ByRef p As pspline3interpolant)
        Try
            p = New pspline3interpolant()
            alglib.pspline3build(xy, n, st, pt, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline2buildperiodic(ByVal xy(,) As Double, ByVal n As Integer, ByVal st As Integer, ByVal pt As Integer, ByRef p As pspline2interpolant)
        Try
            p = New pspline2interpolant()
            alglib.pspline2buildperiodic(xy, n, st, pt, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline3buildperiodic(ByVal xy(,) As Double, ByVal n As Integer, ByVal st As Integer, ByVal pt As Integer, ByRef p As pspline3interpolant)
        Try
            p = New pspline3interpolant()
            alglib.pspline3buildperiodic(xy, n, st, pt, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline2parametervalues(ByVal p As pspline2interpolant, ByRef n As Integer, ByRef t() As Double)
        Try
            alglib.pspline2parametervalues(p.csobj, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline3parametervalues(ByVal p As pspline3interpolant, ByRef n As Integer, ByRef t() As Double)
        Try
            alglib.pspline3parametervalues(p.csobj, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline2calc(ByVal p As pspline2interpolant, ByVal t As Double, ByRef x As Double, ByRef y As Double)
        Try
            alglib.pspline2calc(p.csobj, t, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline3calc(ByVal p As pspline3interpolant, ByVal t As Double, ByRef x As Double, ByRef y As Double, ByRef z As Double)
        Try
            alglib.pspline3calc(p.csobj, t, x, y, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline2tangent(ByVal p As pspline2interpolant, ByVal t As Double, ByRef x As Double, ByRef y As Double)
        Try
            alglib.pspline2tangent(p.csobj, t, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline3tangent(ByVal p As pspline3interpolant, ByVal t As Double, ByRef x As Double, ByRef y As Double, ByRef z As Double)
        Try
            alglib.pspline3tangent(p.csobj, t, x, y, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline2diff(ByVal p As pspline2interpolant, ByVal t As Double, ByRef x As Double, ByRef dx As Double, ByRef y As Double, ByRef dy As Double)
        Try
            alglib.pspline2diff(p.csobj, t, x, dx, y, dy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline3diff(ByVal p As pspline3interpolant, ByVal t As Double, ByRef x As Double, ByRef dx As Double, ByRef y As Double, ByRef dy As Double, ByRef z As Double, ByRef dz As Double)
        Try
            alglib.pspline3diff(p.csobj, t, x, dx, y, dy, z, dz)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline2diff2(ByVal p As pspline2interpolant, ByVal t As Double, ByRef x As Double, ByRef dx As Double, ByRef d2x As Double, ByRef y As Double, ByRef dy As Double, ByRef d2y As Double)
        Try
            alglib.pspline2diff2(p.csobj, t, x, dx, d2x, y, dy, d2y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub pspline3diff2(ByVal p As pspline3interpolant, ByVal t As Double, ByRef x As Double, ByRef dx As Double, ByRef d2x As Double, ByRef y As Double, ByRef dy As Double, ByRef d2y As Double, ByRef z As Double, ByRef dz As Double, ByRef d2z As Double)
        Try
            alglib.pspline3diff2(p.csobj, t, x, dx, d2x, y, dy, d2y, z, dz, d2z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function pspline2arclength(ByVal p As pspline2interpolant, ByVal a As Double, ByVal b As Double) As Double
        Try
            pspline2arclength = alglib.pspline2arclength(p.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function pspline3arclength(ByVal p As pspline3interpolant, ByVal a As Double, ByVal b As Double) As Double
        Try
            pspline3arclength = alglib.pspline3arclength(p.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub parametricrdpfixed(ByVal x(,) As Double, ByVal n As Integer, ByVal d As Integer, ByVal stopm As Integer, ByVal stopeps As Double, ByRef x2(,) As Double, ByRef idx2() As Integer, ByRef nsections As Integer)
        Try
            alglib.parametricrdpfixed(x, n, d, stopm, stopeps, x2, idx2, nsections)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class linlsqrstate
        Public csobj As alglib.linlsqrstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class linlsqrreport
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property nmv() As Integer
        Get
            Return csobj.nmv
        End Get
        Set(ByVal Value As Integer)
            csobj.nmv = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public csobj As alglib.linlsqrreport
    End Class


    Public Sub linlsqrcreate(ByVal m As Integer, ByVal n As Integer, ByRef state As linlsqrstate)
        Try
            state = New linlsqrstate()
            alglib.linlsqrcreate(m, n, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub linlsqrsetprecunit(ByVal state As linlsqrstate)
        Try
            alglib.linlsqrsetprecunit(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub linlsqrsetprecdiag(ByVal state As linlsqrstate)
        Try
            alglib.linlsqrsetprecdiag(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub linlsqrsetlambdai(ByVal state As linlsqrstate, ByVal lambdai As Double)
        Try
            alglib.linlsqrsetlambdai(state.csobj, lambdai)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub linlsqrsolvesparse(ByVal state As linlsqrstate, ByVal a As sparsematrix, ByVal b() As Double)
        Try
            alglib.linlsqrsolvesparse(state.csobj, a.csobj, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub linlsqrsetcond(ByVal state As linlsqrstate, ByVal epsa As Double, ByVal epsb As Double, ByVal maxits As Integer)
        Try
            alglib.linlsqrsetcond(state.csobj, epsa, epsb, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub linlsqrresults(ByVal state As linlsqrstate, ByRef x() As Double, ByRef rep As linlsqrreport)
        Try
            rep = New linlsqrreport()
            alglib.linlsqrresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub linlsqrsetxrep(ByVal state As linlsqrstate, ByVal needxrep As Boolean)
        Try
            alglib.linlsqrsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class rbfmodel
        Public csobj As alglib.rbfmodel
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'RBF solution report:
    '* TerminationType   -   termination type, positive values - success,
    '                        non-positive - failure.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class rbfreport
        Public Property arows() As Integer
        Get
            Return csobj.arows
        End Get
        Set(ByVal Value As Integer)
            csobj.arows = Value
        End Set
        End Property
        Public Property acols() As Integer
        Get
            Return csobj.acols
        End Get
        Set(ByVal Value As Integer)
            csobj.acols = Value
        End Set
        End Property
        Public Property annz() As Integer
        Get
            Return csobj.annz
        End Get
        Set(ByVal Value As Integer)
            csobj.annz = Value
        End Set
        End Property
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property nmv() As Integer
        Get
            Return csobj.nmv
        End Get
        Set(ByVal Value As Integer)
            csobj.nmv = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public csobj As alglib.rbfreport
    End Class
    Public Sub rbfserialize(ByVal obj As rbfmodel, ByRef s_out As String)
        Try
            alglib.rbfserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Sub rbfunserialize(ByVal s_in As String, ByRef obj As rbfmodel)
        Try
            alglib.rbfunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfcreate(ByVal nx As Integer, ByVal ny As Integer, ByRef s As rbfmodel)
        Try
            s = New rbfmodel()
            alglib.rbfcreate(nx, ny, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfsetpoints(ByVal s As rbfmodel, ByVal xy(,) As Double, ByVal n As Integer)
        Try
            alglib.rbfsetpoints(s.csobj, xy, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfsetpoints(ByVal s As rbfmodel, ByVal xy(,) As Double)
        Try
            alglib.rbfsetpoints(s.csobj, xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfsetalgoqnn(ByVal s As rbfmodel, ByVal q As Double, ByVal z As Double)
        Try
            alglib.rbfsetalgoqnn(s.csobj, q, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfsetalgoqnn(ByVal s As rbfmodel)
        Try
            alglib.rbfsetalgoqnn(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfsetalgomultilayer(ByVal s As rbfmodel, ByVal rbase As Double, ByVal nlayers As Integer, ByVal lambdav As Double)
        Try
            alglib.rbfsetalgomultilayer(s.csobj, rbase, nlayers, lambdav)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfsetalgomultilayer(ByVal s As rbfmodel, ByVal rbase As Double, ByVal nlayers As Integer)
        Try
            alglib.rbfsetalgomultilayer(s.csobj, rbase, nlayers)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfsetlinterm(ByVal s As rbfmodel)
        Try
            alglib.rbfsetlinterm(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfsetconstterm(ByVal s As rbfmodel)
        Try
            alglib.rbfsetconstterm(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfsetzeroterm(ByVal s As rbfmodel)
        Try
            alglib.rbfsetzeroterm(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfbuildmodel(ByVal s As rbfmodel, ByRef rep As rbfreport)
        Try
            rep = New rbfreport()
            alglib.rbfbuildmodel(s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function rbfcalc2(ByVal s As rbfmodel, ByVal x0 As Double, ByVal x1 As Double) As Double
        Try
            rbfcalc2 = alglib.rbfcalc2(s.csobj, x0, x1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rbfcalc3(ByVal s As rbfmodel, ByVal x0 As Double, ByVal x1 As Double, ByVal x2 As Double) As Double
        Try
            rbfcalc3 = alglib.rbfcalc3(s.csobj, x0, x1, x2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub rbfcalc(ByVal s As rbfmodel, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.rbfcalc(s.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfcalcbuf(ByVal s As rbfmodel, ByVal x() As Double, ByRef y() As Double)
        Try
            alglib.rbfcalcbuf(s.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfgridcalc2(ByVal s As rbfmodel, ByVal x0() As Double, ByVal n0 As Integer, ByVal x1() As Double, ByVal n1 As Integer, ByRef y(,) As Double)
        Try
            alglib.rbfgridcalc2(s.csobj, x0, n0, x1, n1, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rbfunpack(ByVal s As rbfmodel, ByRef nx As Integer, ByRef ny As Integer, ByRef xwr(,) As Double, ByRef nc As Integer, ByRef v(,) As Double)
        Try
            alglib.rbfunpack(s.csobj, nx, ny, xwr, nc, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class spline2dinterpolant
        Public csobj As alglib.spline2dinterpolant
    End Class


    Public Function spline2dcalc(ByVal c As spline2dinterpolant, ByVal x As Double, ByVal y As Double) As Double
        Try
            spline2dcalc = alglib.spline2dcalc(c.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub spline2ddiff(ByVal c As spline2dinterpolant, ByVal x As Double, ByVal y As Double, ByRef f As Double, ByRef fx As Double, ByRef fy As Double, ByRef fxy As Double)
        Try
            alglib.spline2ddiff(c.csobj, x, y, f, fx, fy, fxy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dlintransxy(ByVal c As spline2dinterpolant, ByVal ax As Double, ByVal bx As Double, ByVal ay As Double, ByVal by As Double)
        Try
            alglib.spline2dlintransxy(c.csobj, ax, bx, ay, by)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dlintransf(ByVal c As spline2dinterpolant, ByVal a As Double, ByVal b As Double)
        Try
            alglib.spline2dlintransf(c.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dcopy(ByVal c As spline2dinterpolant, ByRef cc As spline2dinterpolant)
        Try
            cc = New spline2dinterpolant()
            alglib.spline2dcopy(c.csobj, cc.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dresamplebicubic(ByVal a(,) As Double, ByVal oldheight As Integer, ByVal oldwidth As Integer, ByRef b(,) As Double, ByVal newheight As Integer, ByVal newwidth As Integer)
        Try
            alglib.spline2dresamplebicubic(a, oldheight, oldwidth, b, newheight, newwidth)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dresamplebilinear(ByVal a(,) As Double, ByVal oldheight As Integer, ByVal oldwidth As Integer, ByRef b(,) As Double, ByVal newheight As Integer, ByVal newwidth As Integer)
        Try
            alglib.spline2dresamplebilinear(a, oldheight, oldwidth, b, newheight, newwidth)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dbuildbilinearv(ByVal x() As Double, ByVal n As Integer, ByVal y() As Double, ByVal m As Integer, ByVal f() As Double, ByVal d As Integer, ByRef c As spline2dinterpolant)
        Try
            c = New spline2dinterpolant()
            alglib.spline2dbuildbilinearv(x, n, y, m, f, d, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dbuildbicubicv(ByVal x() As Double, ByVal n As Integer, ByVal y() As Double, ByVal m As Integer, ByVal f() As Double, ByVal d As Integer, ByRef c As spline2dinterpolant)
        Try
            c = New spline2dinterpolant()
            alglib.spline2dbuildbicubicv(x, n, y, m, f, d, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dcalcvbuf(ByVal c As spline2dinterpolant, ByVal x As Double, ByVal y As Double, ByRef f() As Double)
        Try
            alglib.spline2dcalcvbuf(c.csobj, x, y, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dcalcv(ByVal c As spline2dinterpolant, ByVal x As Double, ByVal y As Double, ByRef f() As Double)
        Try
            alglib.spline2dcalcv(c.csobj, x, y, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dunpackv(ByVal c As spline2dinterpolant, ByRef m As Integer, ByRef n As Integer, ByRef d As Integer, ByRef tbl(,) As Double)
        Try
            alglib.spline2dunpackv(c.csobj, m, n, d, tbl)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dbuildbilinear(ByVal x() As Double, ByVal y() As Double, ByVal f(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef c As spline2dinterpolant)
        Try
            c = New spline2dinterpolant()
            alglib.spline2dbuildbilinear(x, y, f, m, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dbuildbicubic(ByVal x() As Double, ByVal y() As Double, ByVal f(,) As Double, ByVal m As Integer, ByVal n As Integer, ByRef c As spline2dinterpolant)
        Try
            c = New spline2dinterpolant()
            alglib.spline2dbuildbicubic(x, y, f, m, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline2dunpack(ByVal c As spline2dinterpolant, ByRef m As Integer, ByRef n As Integer, ByRef tbl(,) As Double)
        Try
            alglib.spline2dunpack(c.csobj, m, n, tbl)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class spline3dinterpolant
        Public csobj As alglib.spline3dinterpolant
    End Class


    Public Function spline3dcalc(ByVal c As spline3dinterpolant, ByVal x As Double, ByVal y As Double, ByVal z As Double) As Double
        Try
            spline3dcalc = alglib.spline3dcalc(c.csobj, x, y, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub spline3dlintransxyz(ByVal c As spline3dinterpolant, ByVal ax As Double, ByVal bx As Double, ByVal ay As Double, ByVal by As Double, ByVal az As Double, ByVal bz As Double)
        Try
            alglib.spline3dlintransxyz(c.csobj, ax, bx, ay, by, az, bz)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline3dlintransf(ByVal c As spline3dinterpolant, ByVal a As Double, ByVal b As Double)
        Try
            alglib.spline3dlintransf(c.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline3dresampletrilinear(ByVal a() As Double, ByVal oldzcount As Integer, ByVal oldycount As Integer, ByVal oldxcount As Integer, ByVal newzcount As Integer, ByVal newycount As Integer, ByVal newxcount As Integer, ByRef b() As Double)
        Try
            alglib.spline3dresampletrilinear(a, oldzcount, oldycount, oldxcount, newzcount, newycount, newxcount, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline3dbuildtrilinearv(ByVal x() As Double, ByVal n As Integer, ByVal y() As Double, ByVal m As Integer, ByVal z() As Double, ByVal l As Integer, ByVal f() As Double, ByVal d As Integer, ByRef c As spline3dinterpolant)
        Try
            c = New spline3dinterpolant()
            alglib.spline3dbuildtrilinearv(x, n, y, m, z, l, f, d, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline3dcalcvbuf(ByVal c As spline3dinterpolant, ByVal x As Double, ByVal y As Double, ByVal z As Double, ByRef f() As Double)
        Try
            alglib.spline3dcalcvbuf(c.csobj, x, y, z, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline3dcalcv(ByVal c As spline3dinterpolant, ByVal x As Double, ByVal y As Double, ByVal z As Double, ByRef f() As Double)
        Try
            alglib.spline3dcalcv(c.csobj, x, y, z, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spline3dunpackv(ByVal c As spline3dinterpolant, ByRef n As Integer, ByRef m As Integer, ByRef l As Integer, ByRef d As Integer, ByRef stype As Integer, ByRef tbl(,) As Double)
        Try
            alglib.spline3dunpackv(c.csobj, n, m, l, d, stype, tbl)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function rmatrixludet(ByVal a(,) As Double, ByVal pivots() As Integer, ByVal n As Integer) As Double
        Try
            rmatrixludet = alglib.rmatrixludet(a, pivots, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rmatrixludet(ByVal a(,) As Double, ByVal pivots() As Integer) As Double
        Try
            rmatrixludet = alglib.rmatrixludet(a, pivots)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rmatrixdet(ByVal a(,) As Double, ByVal n As Integer) As Double
        Try
            rmatrixdet = alglib.rmatrixdet(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function rmatrixdet(ByVal a(,) As Double) As Double
        Try
            rmatrixdet = alglib.rmatrixdet(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixludet(ByVal a(,) As alglib.complex, ByVal pivots() As Integer, ByVal n As Integer) As alglib.complex
        Try
            cmatrixludet = alglib.cmatrixludet(a, pivots, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixludet(ByVal a(,) As alglib.complex, ByVal pivots() As Integer) As alglib.complex
        Try
            cmatrixludet = alglib.cmatrixludet(a, pivots)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixdet(ByVal a(,) As alglib.complex, ByVal n As Integer) As alglib.complex
        Try
            cmatrixdet = alglib.cmatrixdet(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function cmatrixdet(ByVal a(,) As alglib.complex) As alglib.complex
        Try
            cmatrixdet = alglib.cmatrixdet(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spdmatrixcholeskydet(ByVal a(,) As Double, ByVal n As Integer) As Double
        Try
            spdmatrixcholeskydet = alglib.spdmatrixcholeskydet(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spdmatrixcholeskydet(ByVal a(,) As Double) As Double
        Try
            spdmatrixcholeskydet = alglib.spdmatrixcholeskydet(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spdmatrixdet(ByVal a(,) As Double, ByVal n As Integer, ByVal isupper As Boolean) As Double
        Try
            spdmatrixdet = alglib.spdmatrixdet(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function spdmatrixdet(ByVal a(,) As Double) As Double
        Try
            spdmatrixdet = alglib.spdmatrixdet(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function smatrixgevd(ByVal a(,) As Double, ByVal n As Integer, ByVal isuppera As Boolean, ByVal b(,) As Double, ByVal isupperb As Boolean, ByVal zneeded As Integer, ByVal problemtype As Integer, ByRef d() As Double, ByRef z(,) As Double) As Boolean
        Try
            smatrixgevd = alglib.smatrixgevd(a, n, isuppera, b, isupperb, zneeded, problemtype, d, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function smatrixgevdreduce(ByRef a(,) As Double, ByVal n As Integer, ByVal isuppera As Boolean, ByVal b(,) As Double, ByVal isupperb As Boolean, ByVal problemtype As Integer, ByRef r(,) As Double, ByRef isupperr As Boolean) As Boolean
        Try
            smatrixgevdreduce = alglib.smatrixgevdreduce(a, n, isuppera, b, isupperb, problemtype, r, isupperr)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Sub rmatrixinvupdatesimple(ByRef inva(,) As Double, ByVal n As Integer, ByVal updrow As Integer, ByVal updcolumn As Integer, ByVal updval As Double)
        Try
            alglib.rmatrixinvupdatesimple(inva, n, updrow, updcolumn, updval)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixinvupdaterow(ByRef inva(,) As Double, ByVal n As Integer, ByVal updrow As Integer, ByVal v() As Double)
        Try
            alglib.rmatrixinvupdaterow(inva, n, updrow, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixinvupdatecolumn(ByRef inva(,) As Double, ByVal n As Integer, ByVal updcolumn As Integer, ByVal u() As Double)
        Try
            alglib.rmatrixinvupdatecolumn(inva, n, updcolumn, u)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub rmatrixinvupdateuv(ByRef inva(,) As Double, ByVal n As Integer, ByVal u() As Double, ByVal v() As Double)
        Try
            alglib.rmatrixinvupdateuv(inva, n, u, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function rmatrixschur(ByRef a(,) As Double, ByVal n As Integer, ByRef s(,) As Double) As Boolean
        Try
            rmatrixschur = alglib.rmatrixschur(a, n, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function

    Public Class minasastate
        Public csobj As alglib.minasastate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minasareport
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property nfev() As Integer
        Get
            Return csobj.nfev
        End Get
        Set(ByVal Value As Integer)
            csobj.nfev = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public Property activeconstraints() As Integer
        Get
            Return csobj.activeconstraints
        End Get
        Set(ByVal Value As Integer)
            csobj.activeconstraints = Value
        End Set
        End Property
        Public csobj As alglib.minasareport
    End Class


    Public Sub minlbfgssetdefaultpreconditioner(ByVal state As minlbfgsstate)
        Try
            alglib.minlbfgssetdefaultpreconditioner(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minlbfgssetcholeskypreconditioner(ByVal state As minlbfgsstate, ByVal p(,) As Double, ByVal isupper As Boolean)
        Try
            alglib.minlbfgssetcholeskypreconditioner(state.csobj, p, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetbarrierwidth(ByVal state As minbleicstate, ByVal mu As Double)
        Try
            alglib.minbleicsetbarrierwidth(state.csobj, mu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minbleicsetbarrierdecay(ByVal state As minbleicstate, ByVal mudecay As Double)
        Try
            alglib.minbleicsetbarrierdecay(state.csobj, mudecay)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minasacreate(ByVal n As Integer, ByVal x() As Double, ByVal bndl() As Double, ByVal bndu() As Double, ByRef state As minasastate)
        Try
            state = New minasastate()
            alglib.minasacreate(n, x, bndl, bndu, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minasacreate(ByVal x() As Double, ByVal bndl() As Double, ByVal bndu() As Double, ByRef state As minasastate)
        Try
            state = New minasastate()
            alglib.minasacreate(x, bndl, bndu, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minasasetcond(ByVal state As minasastate, ByVal epsg As Double, ByVal epsf As Double, ByVal epsx As Double, ByVal maxits As Integer)
        Try
            alglib.minasasetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minasasetxrep(ByVal state As minasastate, ByVal needxrep As Boolean)
        Try
            alglib.minasasetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minasasetalgorithm(ByVal state As minasastate, ByVal algotype As Integer)
        Try
            alglib.minasasetalgorithm(state.csobj, algotype)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minasasetstpmax(ByVal state As minasastate, ByVal stpmax As Double)
        Try
            alglib.minasasetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function minasaiteration(ByVal state As minasastate) As Boolean
        Try
            minasaiteration = alglib.minasaiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' 
    '   -- ALGLIB --
    '      Copyright 20.03.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub minasaoptimize(ByVal state As minasastate, ByVal grad As ndimensional_grad, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.mincomp.minasastate = state.csobj.innerobj
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minasaoptimize()' (grad is null)")
        End If
        Try
            While alglib.mincomp.minasaiteration(innerobj)
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minasaoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub




    Public Sub minasaresults(ByVal state As minasastate, ByRef x() As Double, ByRef rep As minasareport)
        Try
            rep = New minasareport()
            alglib.minasaresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minasaresultsbuf(ByVal state As minasastate, ByRef x() As Double, ByRef rep As minasareport)
        Try
            alglib.minasaresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minasarestartfrom(ByVal state As minasastate, ByVal x() As Double, ByVal bndl() As Double, ByVal bndu() As Double)
        Try
            alglib.minasarestartfrom(state.csobj, x, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class minnlcstate
        Public csobj As alglib.minnlcstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure stores optimization report:
    '* IterationsCount           total number of inner iterations
    '* NFEV                      number of gradient evaluations
    '* TerminationType           termination type (see below)
    '
    'TERMINATION CODES
    '
    'TerminationType field contains completion code, which can be:
    '  -8    internal integrity control detected  infinite  or  NAN  values  in
    '        function/gradient. Abnormal termination signalled.
    '  -7    gradient verification failed.
    '        See MinNLCSetGradientCheck() for more information.
    '   1    relative function improvement is no more than EpsF.
    '   2    relative step is no more than EpsX.
    '   4    gradient norm is no more than EpsG
    '   5    MaxIts steps was taken
    '   7    stopping conditions are too stringent,
    '        further improvement is impossible,
    '        X contains best point found so far.
    '
    'Other fields of this structure are not documented and should not be used!
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minnlcreport
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property nfev() As Integer
        Get
            Return csobj.nfev
        End Get
        Set(ByVal Value As Integer)
            csobj.nfev = Value
        End Set
        End Property
        Public Property varidx() As Integer
        Get
            Return csobj.varidx
        End Get
        Set(ByVal Value As Integer)
            csobj.varidx = Value
        End Set
        End Property
        Public Property funcidx() As Integer
        Get
            Return csobj.funcidx
        End Get
        Set(ByVal Value As Integer)
            csobj.funcidx = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public Property dbgphase0its() As Integer
        Get
            Return csobj.dbgphase0its
        End Get
        Set(ByVal Value As Integer)
            csobj.dbgphase0its = Value
        End Set
        End Property
        Public csobj As alglib.minnlcreport
    End Class


    Public Sub minnlccreate(ByVal n As Integer, ByVal x() As Double, ByRef state As minnlcstate)
        Try
            state = New minnlcstate()
            alglib.minnlccreate(n, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlccreate(ByVal x() As Double, ByRef state As minnlcstate)
        Try
            state = New minnlcstate()
            alglib.minnlccreate(x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlccreatef(ByVal n As Integer, ByVal x() As Double, ByVal diffstep As Double, ByRef state As minnlcstate)
        Try
            state = New minnlcstate()
            alglib.minnlccreatef(n, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlccreatef(ByVal x() As Double, ByVal diffstep As Double, ByRef state As minnlcstate)
        Try
            state = New minnlcstate()
            alglib.minnlccreatef(x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetbc(ByVal state As minnlcstate, ByVal bndl() As Double, ByVal bndu() As Double)
        Try
            alglib.minnlcsetbc(state.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetlc(ByVal state As minnlcstate, ByVal c(,) As Double, ByVal ct() As Integer, ByVal k As Integer)
        Try
            alglib.minnlcsetlc(state.csobj, c, ct, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetlc(ByVal state As minnlcstate, ByVal c(,) As Double, ByVal ct() As Integer)
        Try
            alglib.minnlcsetlc(state.csobj, c, ct)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetnlc(ByVal state As minnlcstate, ByVal nlec As Integer, ByVal nlic As Integer)
        Try
            alglib.minnlcsetnlc(state.csobj, nlec, nlic)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetcond(ByVal state As minnlcstate, ByVal epsg As Double, ByVal epsf As Double, ByVal epsx As Double, ByVal maxits As Integer)
        Try
            alglib.minnlcsetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetscale(ByVal state As minnlcstate, ByVal s() As Double)
        Try
            alglib.minnlcsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetprecinexact(ByVal state As minnlcstate)
        Try
            alglib.minnlcsetprecinexact(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetprecexactlowrank(ByVal state As minnlcstate, ByVal updatefreq As Integer)
        Try
            alglib.minnlcsetprecexactlowrank(state.csobj, updatefreq)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetprecnone(ByVal state As minnlcstate)
        Try
            alglib.minnlcsetprecnone(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetalgoaul(ByVal state As minnlcstate, ByVal rho As Double, ByVal itscnt As Integer)
        Try
            alglib.minnlcsetalgoaul(state.csobj, rho, itscnt)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetxrep(ByVal state As minnlcstate, ByVal needxrep As Boolean)
        Try
            alglib.minnlcsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function minnlciteration(ByVal state As minnlcstate) As Boolean
        Try
            minnlciteration = alglib.minnlciteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     fvec    -   callback which calculates function vector fi[]
    '                 at given point x
    '     jac     -   callback which calculates function vector fi[]
    '                 and Jacobian jac at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. This function has two different implementations: one which  uses  exact
    '    (analytical) user-supplied Jacobian, and one which uses  only  function
    '    vector and numerically  differentiates  function  in  order  to  obtain
    '    gradient.
    ' 
    '    Depending  on  the  specific  function  used to create optimizer object
    '    you should choose appropriate variant of MinNLCOptimize() -  one  which
    '    accepts function AND Jacobian or one which accepts ONLY function.
    ' 
    '    Be careful to choose variant of MinNLCOptimize()  which  corresponds to
    '    your optimization scheme! Table below lists different  combinations  of
    '    callback (function/gradient) passed to MinNLCOptimize()   and  specific
    '    function used to create optimizer.
    ' 
    ' 
    '                      |         USER PASSED TO MinNLCOptimize()
    '    CREATED WITH      |  function only   |  function and gradient
    '    ------------------------------------------------------------
    '    MinNLCCreateF()   |     works               FAILS
    '    MinNLCCreate()    |     FAILS               works
    ' 
    '    Here "FAILS" denotes inappropriate combinations  of  optimizer creation
    '    function  and  MinNLCOptimize()  version.   Attemps   to    use    such
    '    combination will lead to exception. Either  you  did  not pass gradient
    '    when it WAS needed or you passed gradient when it was NOT needed.
    ' 
    '   -- ALGLIB --
    '      Copyright 06.06.2014 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub minnlcoptimize(ByVal state As minnlcstate, ByVal fvec As ndimensional_fvec, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minnlc.minnlcstate = state.csobj.innerobj
        If fvec Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minnlcoptimize()' (fvec is null)")
        End If
        Try
            While alglib.minnlc.minnlciteration(innerobj)
                If innerobj.needfi Then
                    fvec(innerobj.x, innerobj.fi, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minnlcoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub


    Public Sub minnlcoptimize(ByVal state As minnlcstate, ByVal jac As ndimensional_jac, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.minnlc.minnlcstate = state.csobj.innerobj
        If jac Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minnlcoptimize()' (jac is null)")
        End If
        Try
            While alglib.minnlc.minnlciteration(innerobj)
                If innerobj.needfij Then
                    jac(innerobj.x, innerobj.fi, innerobj.j, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minnlcoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub




    Public Sub minnlcresults(ByVal state As minnlcstate, ByRef x() As Double, ByRef rep As minnlcreport)
        Try
            rep = New minnlcreport()
            alglib.minnlcresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcresultsbuf(ByVal state As minnlcstate, ByRef x() As Double, ByRef rep As minnlcreport)
        Try
            alglib.minnlcresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcrestartfrom(ByVal state As minnlcstate, ByVal x() As Double)
        Try
            alglib.minnlcrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub minnlcsetgradientcheck(ByVal state As minnlcstate, ByVal teststep As Double)
        Try
            alglib.minnlcsetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class lincgstate
        Public csobj As alglib.lincgstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class lincgreport
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property nmv() As Integer
        Get
            Return csobj.nmv
        End Get
        Set(ByVal Value As Integer)
            csobj.nmv = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public Property r2() As Double
        Get
            Return csobj.r2
        End Get
        Set(ByVal Value As Double)
            csobj.r2 = Value
        End Set
        End Property
        Public csobj As alglib.lincgreport
    End Class


    Public Sub lincgcreate(ByVal n As Integer, ByRef state As lincgstate)
        Try
            state = New lincgstate()
            alglib.lincgcreate(n, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lincgsetstartingpoint(ByVal state As lincgstate, ByVal x() As Double)
        Try
            alglib.lincgsetstartingpoint(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lincgsetprecunit(ByVal state As lincgstate)
        Try
            alglib.lincgsetprecunit(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lincgsetprecdiag(ByVal state As lincgstate)
        Try
            alglib.lincgsetprecdiag(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lincgsetcond(ByVal state As lincgstate, ByVal epsf As Double, ByVal maxits As Integer)
        Try
            alglib.lincgsetcond(state.csobj, epsf, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lincgsolvesparse(ByVal state As lincgstate, ByVal a As sparsematrix, ByVal isupper As Boolean, ByVal b() As Double)
        Try
            alglib.lincgsolvesparse(state.csobj, a.csobj, isupper, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lincgresults(ByVal state As lincgstate, ByRef x() As Double, ByRef rep As lincgreport)
        Try
            rep = New lincgreport()
            alglib.lincgresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lincgsetrestartfreq(ByVal state As lincgstate, ByVal srf As Integer)
        Try
            alglib.lincgsetrestartfreq(state.csobj, srf)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lincgsetrupdatefreq(ByVal state As lincgstate, ByVal freq As Integer)
        Try
            alglib.lincgsetrupdatefreq(state.csobj, freq)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub lincgsetxrep(ByVal state As lincgstate, ByVal needxrep As Boolean)
        Try
            alglib.lincgsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    Public Class nleqstate
        Public csobj As alglib.nleqstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class nleqreport
        Public Property iterationscount() As Integer
        Get
            Return csobj.iterationscount
        End Get
        Set(ByVal Value As Integer)
            csobj.iterationscount = Value
        End Set
        End Property
        Public Property nfunc() As Integer
        Get
            Return csobj.nfunc
        End Get
        Set(ByVal Value As Integer)
            csobj.nfunc = Value
        End Set
        End Property
        Public Property njac() As Integer
        Get
            Return csobj.njac
        End Get
        Set(ByVal Value As Integer)
            csobj.njac = Value
        End Set
        End Property
        Public Property terminationtype() As Integer
        Get
            Return csobj.terminationtype
        End Get
        Set(ByVal Value As Integer)
            csobj.terminationtype = Value
        End Set
        End Property
        Public csobj As alglib.nleqreport
    End Class


    Public Sub nleqcreatelm(ByVal n As Integer, ByVal m As Integer, ByVal x() As Double, ByRef state As nleqstate)
        Try
            state = New nleqstate()
            alglib.nleqcreatelm(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub nleqcreatelm(ByVal m As Integer, ByVal x() As Double, ByRef state As nleqstate)
        Try
            state = New nleqstate()
            alglib.nleqcreatelm(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub nleqsetcond(ByVal state As nleqstate, ByVal epsf As Double, ByVal maxits As Integer)
        Try
            alglib.nleqsetcond(state.csobj, epsf, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub nleqsetxrep(ByVal state As nleqstate, ByVal needxrep As Boolean)
        Try
            alglib.nleqsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub nleqsetstpmax(ByVal state As nleqstate, ByVal stpmax As Double)
        Try
            alglib.nleqsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Function nleqiteration(ByVal state As nleqstate) As Boolean
        Try
            nleqiteration = alglib.nleqiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear solver
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     jac     -   callback which calculates function vector fi[]
    '                 and Jacobian jac at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' 
    '   -- ALGLIB --
    '      Copyright 20.03.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub nleqsolve(ByVal state As nleqstate, ByVal func As ndimensional_func, ByVal jac As ndimensional_jac, ByVal rep As ndimensional_rep, ByVal obj As Object)
        Dim innerobj As alglib.nleq.nleqstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'nleqsolve()' (func is null)")
        End If
        If jac Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'nleqsolve()' (jac is null)")
        End If
        Try
            While alglib.nleq.nleqiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x,  innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfij Then
                    jac(innerobj.x, innerobj.fi, innerobj.j, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep Isnot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'nleqsolve' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.Msg)
        End Try
    End Sub




    Public Sub nleqresults(ByVal state As nleqstate, ByRef x() As Double, ByRef rep As nleqreport)
        Try
            rep = New nleqreport()
            alglib.nleqresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub nleqresultsbuf(ByVal state As nleqstate, ByRef x() As Double, ByRef rep As nleqreport)
        Try
            alglib.nleqresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub nleqrestartfrom(ByVal state As nleqstate, ByVal x() As Double)
        Try
            alglib.nleqrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class polynomialsolverreport
        Public Property maxerr() As Double
        Get
            Return csobj.maxerr
        End Get
        Set(ByVal Value As Double)
            csobj.maxerr = Value
        End Set
        End Property
        Public csobj As alglib.polynomialsolverreport
    End Class


    Public Sub polynomialsolve(ByVal a() As Double, ByVal n As Integer, ByRef x() As alglib.complex, ByRef rep As polynomialsolverreport)
        Try
            rep = New polynomialsolverreport()
            alglib.polynomialsolve(a, n, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub airy(ByVal x As Double, ByRef ai As Double, ByRef aip As Double, ByRef bi As Double, ByRef bip As Double)
        Try
            alglib.airy(x, ai, aip, bi, bip)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function besselj0(ByVal x As Double) As Double
        Try
            besselj0 = alglib.besselj0(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function besselj1(ByVal x As Double) As Double
        Try
            besselj1 = alglib.besselj1(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function besseljn(ByVal n As Integer, ByVal x As Double) As Double
        Try
            besseljn = alglib.besseljn(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function bessely0(ByVal x As Double) As Double
        Try
            bessely0 = alglib.bessely0(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function bessely1(ByVal x As Double) As Double
        Try
            bessely1 = alglib.bessely1(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function besselyn(ByVal n As Integer, ByVal x As Double) As Double
        Try
            besselyn = alglib.besselyn(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function besseli0(ByVal x As Double) As Double
        Try
            besseli0 = alglib.besseli0(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function besseli1(ByVal x As Double) As Double
        Try
            besseli1 = alglib.besseli1(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function besselk0(ByVal x As Double) As Double
        Try
            besselk0 = alglib.besselk0(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function besselk1(ByVal x As Double) As Double
        Try
            besselk1 = alglib.besselk1(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function besselkn(ByVal nn As Integer, ByVal x As Double) As Double
        Try
            besselkn = alglib.besselkn(nn, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function beta(ByVal a As Double, ByVal b As Double) As Double
        Try
            beta = alglib.beta(a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function incompletebeta(ByVal a As Double, ByVal b As Double, ByVal x As Double) As Double
        Try
            incompletebeta = alglib.incompletebeta(a, b, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function invincompletebeta(ByVal a As Double, ByVal b As Double, ByVal y As Double) As Double
        Try
            invincompletebeta = alglib.invincompletebeta(a, b, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function binomialdistribution(ByVal k As Integer, ByVal n As Integer, ByVal p As Double) As Double
        Try
            binomialdistribution = alglib.binomialdistribution(k, n, p)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function binomialcdistribution(ByVal k As Integer, ByVal n As Integer, ByVal p As Double) As Double
        Try
            binomialcdistribution = alglib.binomialcdistribution(k, n, p)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function invbinomialdistribution(ByVal k As Integer, ByVal n As Integer, ByVal y As Double) As Double
        Try
            invbinomialdistribution = alglib.invbinomialdistribution(k, n, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function chebyshevcalculate(ByVal r As Integer, ByVal n As Integer, ByVal x As Double) As Double
        Try
            chebyshevcalculate = alglib.chebyshevcalculate(r, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function chebyshevsum(ByVal c() As Double, ByVal r As Integer, ByVal n As Integer, ByVal x As Double) As Double
        Try
            chebyshevsum = alglib.chebyshevsum(c, r, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub chebyshevcoefficients(ByVal n As Integer, ByRef c() As Double)
        Try
            alglib.chebyshevcoefficients(n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub fromchebyshev(ByVal a() As Double, ByVal n As Integer, ByRef b() As Double)
        Try
            alglib.fromchebyshev(a, n, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function chisquaredistribution(ByVal v As Double, ByVal x As Double) As Double
        Try
            chisquaredistribution = alglib.chisquaredistribution(v, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function chisquarecdistribution(ByVal v As Double, ByVal x As Double) As Double
        Try
            chisquarecdistribution = alglib.chisquarecdistribution(v, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function invchisquaredistribution(ByVal v As Double, ByVal y As Double) As Double
        Try
            invchisquaredistribution = alglib.invchisquaredistribution(v, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function dawsonintegral(ByVal x As Double) As Double
        Try
            dawsonintegral = alglib.dawsonintegral(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function ellipticintegralk(ByVal m As Double) As Double
        Try
            ellipticintegralk = alglib.ellipticintegralk(m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function ellipticintegralkhighprecision(ByVal m1 As Double) As Double
        Try
            ellipticintegralkhighprecision = alglib.ellipticintegralkhighprecision(m1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function incompleteellipticintegralk(ByVal phi As Double, ByVal m As Double) As Double
        Try
            incompleteellipticintegralk = alglib.incompleteellipticintegralk(phi, m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function ellipticintegrale(ByVal m As Double) As Double
        Try
            ellipticintegrale = alglib.ellipticintegrale(m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function incompleteellipticintegrale(ByVal phi As Double, ByVal m As Double) As Double
        Try
            incompleteellipticintegrale = alglib.incompleteellipticintegrale(phi, m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function exponentialintegralei(ByVal x As Double) As Double
        Try
            exponentialintegralei = alglib.exponentialintegralei(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function exponentialintegralen(ByVal x As Double, ByVal n As Integer) As Double
        Try
            exponentialintegralen = alglib.exponentialintegralen(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function fdistribution(ByVal a As Integer, ByVal b As Integer, ByVal x As Double) As Double
        Try
            fdistribution = alglib.fdistribution(a, b, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function fcdistribution(ByVal a As Integer, ByVal b As Integer, ByVal x As Double) As Double
        Try
            fcdistribution = alglib.fcdistribution(a, b, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function invfdistribution(ByVal a As Integer, ByVal b As Integer, ByVal y As Double) As Double
        Try
            invfdistribution = alglib.invfdistribution(a, b, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Sub fresnelintegral(ByVal x As Double, ByRef c As Double, ByRef s As Double)
        Try
            alglib.fresnelintegral(x, c, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function hermitecalculate(ByVal n As Integer, ByVal x As Double) As Double
        Try
            hermitecalculate = alglib.hermitecalculate(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function hermitesum(ByVal c() As Double, ByVal n As Integer, ByVal x As Double) As Double
        Try
            hermitesum = alglib.hermitesum(c, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub hermitecoefficients(ByVal n As Integer, ByRef c() As Double)
        Try
            alglib.hermitecoefficients(n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub jacobianellipticfunctions(ByVal u As Double, ByVal m As Double, ByRef sn As Double, ByRef cn As Double, ByRef dn As Double, ByRef ph As Double)
        Try
            alglib.jacobianellipticfunctions(u, m, sn, cn, dn, ph)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function laguerrecalculate(ByVal n As Integer, ByVal x As Double) As Double
        Try
            laguerrecalculate = alglib.laguerrecalculate(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function laguerresum(ByVal c() As Double, ByVal n As Integer, ByVal x As Double) As Double
        Try
            laguerresum = alglib.laguerresum(c, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub laguerrecoefficients(ByVal n As Integer, ByRef c() As Double)
        Try
            alglib.laguerrecoefficients(n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function legendrecalculate(ByVal n As Integer, ByVal x As Double) As Double
        Try
            legendrecalculate = alglib.legendrecalculate(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function legendresum(ByVal c() As Double, ByVal n As Integer, ByVal x As Double) As Double
        Try
            legendresum = alglib.legendresum(c, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Sub legendrecoefficients(ByVal n As Integer, ByRef c() As Double)
        Try
            alglib.legendrecoefficients(n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Function poissondistribution(ByVal k As Integer, ByVal m As Double) As Double
        Try
            poissondistribution = alglib.poissondistribution(k, m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function poissoncdistribution(ByVal k As Integer, ByVal m As Double) As Double
        Try
            poissoncdistribution = alglib.poissoncdistribution(k, m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function invpoissondistribution(ByVal k As Integer, ByVal y As Double) As Double
        Try
            invpoissondistribution = alglib.invpoissondistribution(k, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function psi(ByVal x As Double) As Double
        Try
            psi = alglib.psi(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Function studenttdistribution(ByVal k As Integer, ByVal t As Double) As Double
        Try
            studenttdistribution = alglib.studenttdistribution(k, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function


    Public Function invstudenttdistribution(ByVal k As Integer, ByVal p As Double) As Double
        Try
            invstudenttdistribution = alglib.invstudenttdistribution(k, p)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Function




    Public Sub sinecosineintegrals(ByVal x As Double, ByRef si As Double, ByRef ci As Double)
        Try
            alglib.sinecosineintegrals(x, si, ci)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub hyperbolicsinecosineintegrals(ByVal x As Double, ByRef shi As Double, ByRef chi As Double)
        Try
            alglib.hyperbolicsinecosineintegrals(x, shi, chi)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub pearsoncorrelationsignificance(ByVal r As Double, ByVal n As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.pearsoncorrelationsignificance(r, n, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub spearmanrankcorrelationsignificance(ByVal r As Double, ByVal n As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.spearmanrankcorrelationsignificance(r, n, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub jarqueberatest(ByVal x() As Double, ByVal n As Integer, ByRef p As Double)
        Try
            alglib.jarqueberatest(x, n, p)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub mannwhitneyutest(ByVal x() As Double, ByVal n As Integer, ByVal y() As Double, ByVal m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.mannwhitneyutest(x, n, y, m, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub onesamplesigntest(ByVal x() As Double, ByVal n As Integer, ByVal median As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.onesamplesigntest(x, n, median, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub studentttest1(ByVal x() As Double, ByVal n As Integer, ByVal mean As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.studentttest1(x, n, mean, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub studentttest2(ByVal x() As Double, ByVal n As Integer, ByVal y() As Double, ByVal m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.studentttest2(x, n, y, m, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub unequalvariancettest(ByVal x() As Double, ByVal n As Integer, ByVal y() As Double, ByVal m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.unequalvariancettest(x, n, y, m, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub ftest(ByVal x() As Double, ByVal n As Integer, ByVal y() As Double, ByVal m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.ftest(x, n, y, m, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub


    Public Sub onesamplevariancetest(ByVal x() As Double, ByVal n As Integer, ByVal variance As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.onesamplevariancetest(x, n, variance, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




    Public Sub wilcoxonsignedranktest(ByVal x() As Double, ByVal n As Integer, ByVal e As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.wilcoxonsignedranktest(x, n, e, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.Msg)
        End Try
    End Sub




End Module
