'*************************************************************************
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
'*************************************************************************

Public Partial Class alglib
	'*******************************************************************
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
'    *******************************************************************

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

	'*******************************************************************
'    Class defining a complex number with double precision.
'    *******************************************************************

	Public Structure complex
		Public x As Double
		Public y As Double

		Public Sub New(_x As Double)
			x = _x
			y = 0
		End Sub
		Public Sub New(_x As Double, _y As Double)
			x = _x
			y = _y
		End Sub
		Public Shared Widening Operator CType(_x As Double) As complex
			Return New complex(_x)
		End Operator
		Public Shared Operator =(lhs As complex, rhs As complex) As Boolean
			Return (CDbl(lhs.x) = CDbl(rhs.x)) And (CDbl(lhs.y) = CDbl(rhs.y))
		End Operator
		Public Shared Operator <>(lhs As complex, rhs As complex) As Boolean
			Return (CDbl(lhs.x) <> CDbl(rhs.x)) Or (CDbl(lhs.y) <> CDbl(rhs.y))
		End Operator
		Public Shared Operator +(lhs As complex) As complex
			Return lhs
		End Operator
		Public Shared Operator -(lhs As complex) As complex
			Return New complex(-lhs.x, -lhs.y)
		End Operator
		Public Shared Operator +(lhs As complex, rhs As complex) As complex
			Return New complex(lhs.x + rhs.x, lhs.y + rhs.y)
		End Operator
		Public Shared Operator -(lhs As complex, rhs As complex) As complex
			Return New complex(lhs.x - rhs.x, lhs.y - rhs.y)
		End Operator
		Public Shared Operator *(lhs As complex, rhs As complex) As complex
			Return New complex(lhs.x * rhs.x - lhs.y * rhs.y, lhs.x * rhs.y + lhs.y * rhs.x)
		End Operator
		Public Shared Operator /(lhs As complex, rhs As complex) As complex
			Dim result As complex
			Dim e As Double
			Dim f As Double
            If System.Math.Abs(rhs.y) < System.Math.Abs(rhs.x) Then
                e = rhs.y / rhs.x
                f = rhs.x + rhs.y * e
                result.x = (lhs.x + lhs.y * e) / f
                result.y = (lhs.y - lhs.x * e) / f
            Else
                e = rhs.x / rhs.y
                f = rhs.y + rhs.x * e
                result.x = (lhs.y + lhs.x * e) / f
                result.y = (-lhs.x + lhs.y * e) / f
            End If
            Return result
        End Operator
        Public Overrides Function GetHashCode() As Integer
            Return x.GetHashCode() Xor y.GetHashCode()
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is Byte Then
                Return Equals(New complex(CByte(obj)))
            End If
            If TypeOf obj Is SByte Then
                Return Equals(New complex(CSByte(obj)))
            End If
            If TypeOf obj Is Short Then
                Return Equals(New complex(CShort(obj)))
            End If
            If TypeOf obj Is UShort Then
                Return Equals(New complex(CUShort(obj)))
            End If
            If TypeOf obj Is Integer Then
                Return Equals(New complex(CInt(obj)))
            End If
            If TypeOf obj Is UInteger Then
                Return Equals(New complex(CUInt(obj)))
            End If
            If TypeOf obj Is Long Then
                Return Equals(New complex(CLng(obj)))
            End If
            If TypeOf obj Is ULong Then
                Return Equals(New complex(CULng(obj)))
            End If
            If TypeOf obj Is Single Then
                Return Equals(New complex(CSng(obj)))
            End If
            If TypeOf obj Is Double Then
                Return Equals(New complex(CDbl(obj)))
            End If
            If TypeOf obj Is Decimal Then
                Return Equals(New complex(CDbl(CDec(obj))))
            End If
            Return Object.Equals(Me, obj)
        End Function
    End Structure

    '*******************************************************************
    '    Class defining an ALGLIB exception
    '    *******************************************************************

    Public Class alglibexception
        Inherits System.Exception
        Public msg As String
        Public Sub New(s As String)
            msg = s
        End Sub

    End Class

    '*******************************************************************
    '    ALGLIB object, parent  class  for  all  internal  AlgoPascal  objects
    '    managed by ALGLIB.
    '    
    '    Any internal AlgoPascal object inherits from this class.
    '    
    '    User-visible objects inherit from alglibobject (see below).
    '    *******************************************************************

    Public MustInherit Class apobject
        Public MustOverride Sub init()
        Public MustOverride Function make_copy() As apobject
    End Class

    '*******************************************************************
    '    ALGLIB object, parent class for all user-visible objects  managed  by
    '    ALGLIB.
    '    
    '    Methods:
    '        _deallocate()       deallocation:
    '                            * in managed ALGLIB it does nothing
    '                            * in native ALGLIB it clears  dynamic  memory
    '                              being  hold  by  object  and  sets internal
    '                              reference to null.
    '        make_copy()         creates deep copy of the object.
    '                            Works in both managed and native versions  of
    '                            ALGLIB.
    '    *******************************************************************

    Public MustInherit Class alglibobject
        Public Overridable Sub _deallocate()
        End Sub
        Public MustOverride Function make_copy() As alglibobject
    End Class

    '*******************************************************************
    '    Deallocation of ALGLIB object:
    '    * in managed ALGLIB this method just sets refence to null
    '    * in native ALGLIB call of this method:
    '      1) clears dynamic memory being hold by  object  and  sets  internal
    '         reference to null.
    '      2) sets to null variable being passed to this method
    '    
    '    IMPORTANT (1): in  native  edition  of  ALGLIB,  obj becomes unusable
    '                   after this call!!!  It  is  possible  to  save  a copy
    '                   of reference in another variable (original variable is
    '                   set to null), but any attempt to work with this object
    '                   will crash your program.
    '    
    '    IMPORTANT (2): memory ownen by object will be recycled by GC  in  any
    '                   case. This method just enforced IMMEDIATE deallocation.
    '    *******************************************************************

    Public Shared Sub deallocateimmediately(Of T As alglib.alglibobject)(ByRef obj As T)
        obj._deallocate()
        obj = Nothing
    End Sub

    '*******************************************************************
    '    Allocation counter:
    '    * in managed ALGLIB it always returns 0 (dummy code)
    '    * in native ALGLIB it returns current value of the allocation counter
    '      (if it was activated)
    '    *******************************************************************

    Public Shared Function alloc_counter() As Long
        Return 0
    End Function

    '*******************************************************************
    '    Activization of the allocation counter:
    '    * in managed ALGLIB it does nothing (dummy code)
    '    * in native ALGLIB it turns on allocation counting.
    '    *******************************************************************

    Public Shared Sub alloc_counter_activate()
    End Sub

    '*******************************************************************
    '    reverse communication structure
    '    *******************************************************************

    Public Class rcommstate
        Inherits apobject
        Public Sub New()
            init()
        End Sub
        Public Overrides Sub init()
            stage = -1
            ia = New Integer(-1) {}
            ba = New Boolean(-1) {}
            ra = New Double(-1) {}
            ca = New alglib.complex(-1) {}
        End Sub
        Public Overrides Function make_copy() As apobject
            Dim result As New rcommstate()
            result.stage = stage
            result.ia = DirectCast(ia.Clone(), Integer())
            result.ba = DirectCast(ba.Clone(), Boolean())
            result.ra = DirectCast(ra.Clone(), Double())
            result.ca = DirectCast(ca.Clone(), alglib.complex())
            Return result
        End Function
        Public stage As Integer
        Public ia As Integer()
        Public ba As Boolean()
        Public ra As Double()
        Public ca As alglib.complex()
    End Class

    '*******************************************************************
    '    internal functions
    '    *******************************************************************

    Public Class ap
        Public Shared Function len(Of T)(a As T()) As Integer
            Return a.Length
        End Function
        Public Shared Function rows(Of T)(a As T(,)) As Integer
            Return a.GetLength(0)
        End Function
        Public Shared Function cols(Of T)(a As T(,)) As Integer
            Return a.GetLength(1)
        End Function
        Public Shared Sub swap(Of T)(ByRef a As T, ByRef b As T)
            Dim tmp As T = a
            a = b
            b = tmp
        End Sub

        Public Shared Sub assert(cond As Boolean, s As String)
            If Not cond Then
                Throw New alglibexception(s)
            End If
        End Sub

        Public Shared Sub assert(cond As Boolean)
            assert(cond, "ALGLIB: assertion failed")
        End Sub

        '***************************************************************
        '        returns dps (digits-of-precision) value corresponding to threshold.
        '        dps(0.9)  = dps(0.5)  = dps(0.1) = 0
        '        dps(0.09) = dps(0.05) = dps(0.01) = 1
        '        and so on
        '        ***************************************************************

        Public Shared Function threshold2dps(threshold As Double) As Integer
            Dim result As Integer = 0
            Dim t As Double
            result = 0
            t = 1
            While t / 10 > threshold * (1 + 0.0000000001)


                result += 1
                t /= 10
            End While
            Return result
        End Function

        '***************************************************************
        '        prints formatted complex
        '        ***************************************************************

        Public Shared Function format(a As complex, _dps As Integer) As String
            Dim dps As Integer = System.Math.Abs(_dps)
            Dim fmt As String = If(_dps >= 0, "F", "E")
            Dim fmtx As String = [String].Format("{{0:" & fmt & "{0}}}", dps)
            Dim fmty As String = [String].Format("{{0:" & fmt & "{0}}}", dps)
            Dim result As String = [String].Format(fmtx, a.x) & (If(a.y >= 0, "+", "-")) & [String].Format(fmty, System.Math.Abs(a.y)) & "i"
            result = result.Replace(","c, "."c)
            Return result
        End Function

        '***************************************************************
        '        prints formatted array
        '        ***************************************************************

        Public Shared Function format(a As Boolean()) As String
            Dim result As String() = New String(len(a) - 1) {}
            Dim i As Integer
            For i = 0 To len(a) - 1
                If a(i) Then
                    result(i) = "true"
                Else
                    result(i) = "false"
                End If
            Next
            Return "{" & [String].Join(",", result) & "}"
        End Function

        '***************************************************************
        '        prints formatted array
        '        ***************************************************************

        Public Shared Function format(a As Integer()) As String
            Dim result As String() = New String(len(a) - 1) {}
            Dim i As Integer
            For i = 0 To len(a) - 1
                result(i) = a(i).ToString()
            Next
            Return "{" & [String].Join(",", result) & "}"
        End Function

        '***************************************************************
        '        prints formatted array
        '        ***************************************************************

        Public Shared Function format(a As Double(), _dps As Integer) As String
            Dim dps As Integer = System.Math.Abs(_dps)
            Dim sfmt As String = If(_dps >= 0, "F", "E")
            Dim fmt As String = [String].Format("{{0:" & sfmt & "{0}}}", dps)
            Dim result As String() = New String(len(a) - 1) {}
            Dim i As Integer
            For i = 0 To len(a) - 1
                result(i) = [String].Format(fmt, a(i))
                result(i) = result(i).Replace(","c, "."c)
            Next
            Return "{" & [String].Join(",", result) & "}"
        End Function

        '***************************************************************
        '        prints formatted array
        '        ***************************************************************

        Public Shared Function format(a As complex(), _dps As Integer) As String
            Dim dps As Integer = System.Math.Abs(_dps)
            Dim fmt As String = If(_dps >= 0, "F", "E")
            Dim fmtx As String = [String].Format("{{0:" & fmt & "{0}}}", dps)
            Dim fmty As String = [String].Format("{{0:" & fmt & "{0}}}", dps)
            Dim result As String() = New String(len(a) - 1) {}
            Dim i As Integer
            For i = 0 To len(a) - 1
                result(i) = [String].Format(fmtx, a(i).x) & (If(a(i).y >= 0, "+", "-")) & [String].Format(fmty, System.Math.Abs(a(i).y)) & "i"
                result(i) = result(i).Replace(","c, "."c)
            Next
            Return "{" & [String].Join(",", result) & "}"
        End Function

        '***************************************************************
        '        prints formatted matrix
        '        ***************************************************************

        Public Shared Function format(a As Boolean(,)) As String
            Dim i As Integer, j As Integer, m As Integer, n As Integer
            n = cols(a)
            m = rows(a)
            Dim line As Boolean() = New Boolean(n - 1) {}
            Dim result As String() = New String(m - 1) {}
            For i = 0 To m - 1
                For j = 0 To n - 1
                    line(j) = a(i, j)
                Next
                result(i) = format(line)
            Next
            Return "{" & [String].Join(",", result) & "}"
        End Function

        '***************************************************************
        '        prints formatted matrix
        '        ***************************************************************

        Public Shared Function format(a As Integer(,)) As String
            Dim i As Integer, j As Integer, m As Integer, n As Integer
            n = cols(a)
            m = rows(a)
            Dim line As Integer() = New Integer(n - 1) {}
            Dim result As String() = New String(m - 1) {}
            For i = 0 To m - 1
                For j = 0 To n - 1
                    line(j) = a(i, j)
                Next
                result(i) = format(line)
            Next
            Return "{" & [String].Join(",", result) & "}"
        End Function

        '***************************************************************
        '        prints formatted matrix
        '        ***************************************************************

        Public Shared Function format(a As Double(,), dps As Integer) As String
            Dim i As Integer, j As Integer, m As Integer, n As Integer
            n = cols(a)
            m = rows(a)
            Dim line As Double() = New Double(n - 1) {}
            Dim result As String() = New String(m - 1) {}
            For i = 0 To m - 1
                For j = 0 To n - 1
                    line(j) = a(i, j)
                Next
                result(i) = format(line, dps)
            Next
            Return "{" & [String].Join(",", result) & "}"
        End Function

        '***************************************************************
        '        prints formatted matrix
        '        ***************************************************************

        Public Shared Function format(a As complex(,), dps As Integer) As String
            Dim i As Integer, j As Integer, m As Integer, n As Integer
            n = cols(a)
            m = rows(a)
            Dim line As complex() = New complex(n - 1) {}
            Dim result As String() = New String(m - 1) {}
            For i = 0 To m - 1
                For j = 0 To n - 1
                    line(j) = a(i, j)
                Next
                result(i) = format(line, dps)
            Next
            Return "{" & [String].Join(",", result) & "}"
        End Function

        '***************************************************************
        '        checks that matrix is symmetric.
        '        max|A-A^T| is calculated; if it is within 1.0E-14 of max|A|,
        '        matrix is considered symmetric
        '        ***************************************************************

        Public Shared Function issymmetric(a As Double(,)) As Boolean
            Dim i As Integer, j As Integer, n As Integer
            Dim err As Double, mx As Double, v1 As Double, v2 As Double
            If rows(a) <> cols(a) Then
                Return False
            End If
            n = rows(a)
            If n = 0 Then
                Return True
            End If
            mx = 0
            err = 0
            For i = 0 To n - 1
                For j = i + 1 To n - 1
                    v1 = a(i, j)
                    v2 = a(j, i)
                    If Not Math.isfinite(v1) Then
                        Return False
                    End If
                    If Not Math.isfinite(v2) Then
                        Return False
                    End If
                    err = System.Math.Max(err, System.Math.Abs(v1 - v2))
                    mx = System.Math.Max(mx, System.Math.Abs(v1))
                    mx = System.Math.Max(mx, System.Math.Abs(v2))
                Next
                v1 = a(i, i)
                If Not Math.isfinite(v1) Then
                    Return False
                End If
                mx = System.Math.Max(mx, System.Math.Abs(v1))
            Next
            If mx = 0 Then
                Return True
            End If
            Return err / mx <= 0.00000000000001
        End Function

        '***************************************************************
        '        checks that matrix is Hermitian.
        '        max|A-A^H| is calculated; if it is within 1.0E-14 of max|A|,
        '        matrix is considered Hermitian
        '        ***************************************************************

        Public Shared Function ishermitian(a As complex(,)) As Boolean
            Dim i As Integer, j As Integer, n As Integer
            Dim err As Double, mx As Double
            Dim v1 As complex, v2 As complex, vt As complex
            If rows(a) <> cols(a) Then
                Return False
            End If
            n = rows(a)
            If n = 0 Then
                Return True
            End If
            mx = 0
            err = 0
            For i = 0 To n - 1
                For j = i + 1 To n - 1
                    v1 = a(i, j)
                    v2 = a(j, i)
                    If Not Math.isfinite(v1.x) Then
                        Return False
                    End If
                    If Not Math.isfinite(v1.y) Then
                        Return False
                    End If
                    If Not Math.isfinite(v2.x) Then
                        Return False
                    End If
                    If Not Math.isfinite(v2.y) Then
                        Return False
                    End If
                    vt.x = v1.x - v2.x
                    vt.y = v1.y + v2.y
                    err = System.Math.Max(err, Math.abscomplex(vt))
                    mx = System.Math.Max(mx, Math.abscomplex(v1))
                    mx = System.Math.Max(mx, Math.abscomplex(v2))
                Next
                v1 = a(i, i)
                If Not Math.isfinite(v1.x) Then
                    Return False
                End If
                If Not Math.isfinite(v1.y) Then
                    Return False
                End If
                err = System.Math.Max(err, System.Math.Abs(v1.y))
                mx = System.Math.Max(mx, Math.abscomplex(v1))
            Next
            If mx = 0 Then
                Return True
            End If
            Return err / mx <= 0.00000000000001
        End Function


        '***************************************************************
        '        Forces symmetricity by copying upper half of A to the lower one
        '        ***************************************************************

        Public Shared Function forcesymmetric(a As Double(,)) As Boolean
            Dim i As Integer, j As Integer, n As Integer
            If rows(a) <> cols(a) Then
                Return False
            End If
            n = rows(a)
            If n = 0 Then
                Return True
            End If
            For i = 0 To n - 1
                For j = i + 1 To n - 1
                    a(i, j) = a(j, i)
                Next
            Next
            Return True
        End Function

        '***************************************************************
        '        Forces Hermiticity by copying upper half of A to the lower one
        '        ***************************************************************

        Public Shared Function forcehermitian(a As complex(,)) As Boolean
            Dim i As Integer, j As Integer, n As Integer
            Dim v As complex
            If rows(a) <> cols(a) Then
                Return False
            End If
            n = rows(a)
            If n = 0 Then
                Return True
            End If
            For i = 0 To n - 1
                For j = i + 1 To n - 1
                    v = a(j, i)
                    a(i, j).x = v.x
                    a(i, j).y = -v.y
                Next
            Next
            Return True
        End Function
    End Class

    '*******************************************************************
    '    math functions
    '    *******************************************************************

    Public Class math
        'public static System.Random RndObject = new System.Random(System.DateTime.Now.Millisecond);
        Public Shared rndobject As New System.Random(System.DateTime.Now.Millisecond + 1000 * System.DateTime.Now.Second + 60 * 1000 * System.DateTime.Now.Minute)

        Public Const machineepsilon As Double = 0.0000000000000005
        Public Const maxrealnumber As Double = 1.0E+300
        Public Const minrealnumber As Double = 1.0E-300

        Public Shared Function isfinite(d As Double) As Boolean
            Return Not System.[Double].IsNaN(d) AndAlso Not System.[Double].IsInfinity(d)
        End Function

        Public Shared Function randomreal() As Double
            Dim r As Double = 0
            SyncLock rndobject
                r = rndobject.NextDouble()
            End SyncLock
            Return r
        End Function
        Public Shared Function randominteger(N As Integer) As Integer
            Dim r As Integer = 0
            SyncLock rndobject
                r = rndobject.[Next](N)
            End SyncLock
            Return r
        End Function
        Public Shared Function sqr(X As Double) As Double
            Return X * X
        End Function
        Public Shared Function abscomplex(z As complex) As Double
            Dim w As Double
            Dim xabs As Double
            Dim yabs As Double
            Dim v As Double

            xabs = System.Math.Abs(z.x)
            yabs = System.Math.Abs(z.y)
            w = If(xabs > yabs, xabs, yabs)
            v = If(xabs < yabs, xabs, yabs)
            If v = 0 Then
                Return w
            Else
                Dim t As Double = v / w
                Return w * System.Math.sqrt(1 + t * t)
            End If
        End Function
        Public Shared Function conj(z As complex) As complex
            Return New complex(z.x, -z.y)
        End Function
        Public Shared Function csqr(z As complex) As complex
            Return New complex(z.x * z.x - z.y * z.y, 2 * z.x * z.y)
        End Function

    End Class


	'*******************************************************************
'    serializer object (should not be used directly)
'    *******************************************************************

	Public Class serializer
		Private Enum SMODE
			[DEFAULT]
			ALLOC
			TO_STRING
			FROM_STRING
		End Enum
		Private Const SER_ENTRIES_PER_ROW As Integer = 5
		Private Const SER_ENTRY_LENGTH As Integer = 11

		Private mode As SMODE
		Private entries_needed As Integer
		Private entries_saved As Integer
		Private bytes_asked As Integer
		Private bytes_written As Integer
		Private bytes_read As Integer
		Private out_str As Char()
		Private in_str As Char()

		Public Sub New()
			mode = SMODE.[DEFAULT]
			entries_needed = 0
			bytes_asked = 0
		End Sub

		Public Sub alloc_start()
			entries_needed = 0
			bytes_asked = 0
			mode = SMODE.ALLOC
		End Sub

		Public Sub alloc_entry()
			If mode <> SMODE.ALLOC Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If
			entries_needed += 1
		End Sub

		Private Function get_alloc_size() As Integer
			Dim rows As Integer, lastrowsize As Integer, result As Integer

			' check and change mode
			If mode <> SMODE.ALLOC Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If

			' if no entries needes (degenerate case)
			If entries_needed = 0 Then
				bytes_asked = 1
				Return bytes_asked
			End If

			' non-degenerate case
			rows = entries_needed \ SER_ENTRIES_PER_ROW
			lastrowsize = SER_ENTRIES_PER_ROW
			If entries_needed Mod SER_ENTRIES_PER_ROW <> 0 Then
				lastrowsize = entries_needed Mod SER_ENTRIES_PER_ROW
				rows += 1
			End If

			' calculate result size
			result = ((rows - 1) * SER_ENTRIES_PER_ROW + lastrowsize) * SER_ENTRY_LENGTH
			result += (rows - 1) * (SER_ENTRIES_PER_ROW - 1) + (lastrowsize - 1)
			result += rows * 2
			bytes_asked = result
			Return result
		End Function

		Public Sub sstart_str()
			Dim allocsize As Integer = get_alloc_size()

			' check and change mode
			If mode <> SMODE.ALLOC Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If
			mode = SMODE.TO_STRING

			' other preparations
			out_str = New Char(allocsize - 1) {}
			entries_saved = 0
			bytes_written = 0
		End Sub

		Public Sub ustart_str(s As String)
			' check and change mode
			If mode <> SMODE.[DEFAULT] Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If
			mode = SMODE.FROM_STRING

			in_str = s.ToCharArray()
			bytes_read = 0
		End Sub

		Public Sub serialize_bool(v As Boolean)
			If mode <> SMODE.TO_STRING Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If
			bool2str(v, out_str, bytes_written)
			entries_saved += 1
			If entries_saved Mod SER_ENTRIES_PER_ROW <> 0 Then
				out_str(bytes_written) = " "C
				bytes_written += 1
			Else
				out_str(bytes_written + 0) = ControlChars.Cr
				out_str(bytes_written + 1) = ControlChars.Lf
				bytes_written += 2
			End If
		End Sub

		Public Sub serialize_int(v As Integer)
			If mode <> SMODE.TO_STRING Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If
			int2str(v, out_str, bytes_written)
			entries_saved += 1
			If entries_saved Mod SER_ENTRIES_PER_ROW <> 0 Then
				out_str(bytes_written) = " "C
				bytes_written += 1
			Else
				out_str(bytes_written + 0) = ControlChars.Cr
				out_str(bytes_written + 1) = ControlChars.Lf
				bytes_written += 2
			End If
		End Sub

		Public Sub serialize_double(v As Double)
			If mode <> SMODE.TO_STRING Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If
			double2str(v, out_str, bytes_written)
			entries_saved += 1
			If entries_saved Mod SER_ENTRIES_PER_ROW <> 0 Then
				out_str(bytes_written) = " "C
				bytes_written += 1
			Else
				out_str(bytes_written + 0) = ControlChars.Cr
				out_str(bytes_written + 1) = ControlChars.Lf
				bytes_written += 2
			End If
		End Sub

		Public Function unserialize_bool() As Boolean
			If mode <> SMODE.FROM_STRING Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If
			Return str2bool(in_str, bytes_read)
		End Function

		Public Function unserialize_int() As Integer
			If mode <> SMODE.FROM_STRING Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If
			Return str2int(in_str, bytes_read)
		End Function

		Public Function unserialize_double() As Double
			If mode <> SMODE.FROM_STRING Then
				Throw New alglib.alglibexception("ALGLIB: internal error during (un)serialization")
			End If
			Return str2double(in_str, bytes_read)
		End Function

		Public Sub [stop]()
		End Sub

		Public Function get_string() As String
			Return New String(out_str, 0, bytes_written)
		End Function


		'***********************************************************************
'        This function converts six-bit value (from 0 to 63)  to  character  (only
'        digits, lowercase and uppercase letters, minus and underscore are used).
'
'        If v is negative or greater than 63, this function returns '?'.
'        ***********************************************************************

		Private Shared _sixbits2char_tbl As Char() = New Char(63) {"0"C, "1"C, "2"C, "3"C, "4"C, "5"C, _
			"6"C, "7"C, "8"C, "9"C, "A"C, "B"C, _
			"C"C, "D"C, "E"C, "F"C, "G"C, "H"C, _
			"I"C, "J"C, "K"C, "L"C, "M"C, "N"C, _
			"O"C, "P"C, "Q"C, "R"C, "S"C, "T"C, _
			"U"C, "V"C, "W"C, "X"C, "Y"C, "Z"C, _
			"a"C, "b"C, "c"C, "d"C, "e"C, "f"C, _
			"g"C, "h"C, "i"C, "j"C, "k"C, "l"C, _
			"m"C, "n"C, "o"C, "p"C, "q"C, "r"C, _
			"s"C, "t"C, "u"C, "v"C, "w"C, "x"C, _
			"y"C, "z"C, "-"C, "_"C}
		Private Shared Function sixbits2char(v As Integer) As Char
			If v < 0 OrElse v > 63 Then
				Return "?"C
			End If
			Return _sixbits2char_tbl(v)
		End Function

		'***********************************************************************
'        This function converts character to six-bit value (from 0 to 63).
'
'        This function is inverse of ae_sixbits2char()
'        If c is not correct character, this function returns -1.
'        ***********************************************************************

		Private Shared _char2sixbits_tbl As Integer() = New Integer(127) {-1, -1, -1, -1, -1, -1, _
			-1, -1, -1, -1, -1, -1, _
			-1, -1, -1, -1, -1, -1, _
			-1, -1, -1, -1, -1, -1, _
			-1, -1, -1, -1, -1, -1, _
			-1, -1, -1, -1, -1, -1, _
			-1, -1, -1, -1, -1, -1, _
			-1, -1, -1, 62, -1, -1, _
			0, 1, 2, 3, 4, 5, _
			6, 7, 8, 9, -1, -1, _
			-1, -1, -1, -1, -1, 10, _
			11, 12, 13, 14, 15, 16, _
			17, 18, 19, 20, 21, 22, _
			23, 24, 25, 26, 27, 28, _
			29, 30, 31, 32, 33, 34, _
			35, -1, -1, -1, -1, 63, _
			-1, 36, 37, 38, 39, 40, _
			41, 42, 43, 44, 45, 46, _
			47, 48, 49, 50, 51, 52, _
			53, 54, 55, 56, 57, 58, _
			59, 60, 61, -1, -1, -1, _
			-1, -1}
		Private Shared Function char2sixbits(c As Char) As Integer
            Dim cc As Integer = AscW(c)
            Return If((cc >= 0 AndAlso cc < 127), _char2sixbits_tbl(cc), -1)
		End Function

		'***********************************************************************
'        This function converts three bytes (24 bits) to four six-bit values 
'        (24 bits again).
'
'        src         array
'        src_offs    offset of three-bytes chunk
'        dst         array for ints
'        dst_offs    offset of four-ints chunk
'        ***********************************************************************

		Private Shared Sub threebytes2foursixbits(src As Byte(), src_offs As Integer, dst As Integer(), dst_offs As Integer)
			dst(dst_offs + 0) = src(src_offs + 0) And &H3f
			dst(dst_offs + 1) = (src(src_offs + 0) >> 6) Or ((src(src_offs + 1) And &Hf) << 2)
			dst(dst_offs + 2) = (src(src_offs + 1) >> 4) Or ((src(src_offs + 2) And &H3) << 4)
			dst(dst_offs + 3) = src(src_offs + 2) >> 2
		End Sub

		'***********************************************************************
'        This function converts four six-bit values (24 bits) to three bytes
'        (24 bits again).
'
'        src         pointer to four ints
'        src_offs    offset of the chunk
'        dst         pointer to three bytes
'        dst_offs    offset of the chunk
'        ***********************************************************************

		Private Shared Sub foursixbits2threebytes(src As Integer(), src_offs As Integer, dst As Byte(), dst_offs As Integer)
			dst(dst_offs + 0) = CByte(src(src_offs + 0) Or ((src(src_offs + 1) And &H3) << 6))
			dst(dst_offs + 1) = CByte((src(src_offs + 1) >> 2) Or ((src(src_offs + 2) And &Hf) << 4))
			dst(dst_offs + 2) = CByte((src(src_offs + 2) >> 4) Or (src(src_offs + 3) << 2))
		End Sub

		'***********************************************************************
'        This function serializes boolean value into buffer
'
'        v           boolean value to be serialized
'        buf         buffer, at least 11 characters wide
'        offs        offset in the buffer
'        
'        after return from this function, offs points to the char's past the value
'        being read.
'        ***********************************************************************

		Private Shared Sub bool2str(v As Boolean, buf As Char(), ByRef offs As Integer)
			Dim c As Char = If(v, "1"C, "0"C)
			Dim i As Integer
			For i = 0 To SER_ENTRY_LENGTH - 1
				buf(offs + i) = c
			Next
			offs += SER_ENTRY_LENGTH
		End Sub

		'***********************************************************************
'        This function unserializes boolean value from buffer
'
'        buf         buffer which contains value; leading spaces/tabs/newlines are 
'                    ignored, traling spaces/tabs/newlines are treated as  end  of
'                    the boolean value.
'        offs        offset in the buffer
'        
'        after return from this function, offs points to the char's past the value
'        being read.
'
'        This function raises an error in case unexpected symbol is found
'        ***********************************************************************

		Private Shared Function str2bool(buf As Char(), ByRef offs As Integer) As Boolean
			Dim was0 As Boolean, was1 As Boolean
			Dim emsg As String = "ALGLIB: unable to read boolean value from stream"

			was0 = False
			was1 = False
			While buf(offs) = " "C OrElse buf(offs) = ControlChars.Tab OrElse buf(offs) = ControlChars.Lf OrElse buf(offs) = ControlChars.Cr
				offs += 1
			End While
            While buf(offs) <> " "c AndAlso buf(offs) <> ControlChars.Tab AndAlso buf(offs) <> ControlChars.Lf AndAlso buf(offs) <> ControlChars.Cr AndAlso AscW(buf(offs)) <> 0
                If buf(offs) = "0"c Then
                    was0 = True
                    offs += 1
                    Continue While
                End If
                If buf(offs) = "1"c Then
                    was1 = True
                    offs += 1
                    Continue While
                End If
                Throw New alglib.alglibexception(emsg)
            End While
			If (Not was0) AndAlso (Not was1) Then
				Throw New alglib.alglibexception(emsg)
			End If
			If was0 AndAlso was1 Then
				Throw New alglib.alglibexception(emsg)
			End If
			Return If(was1, True, False)
		End Function

		'***********************************************************************
'        This function serializes integer value into buffer
'
'        v           integer value to be serialized
'        buf         buffer, at least 11 characters wide 
'        offs        offset in the buffer
'        
'        after return from this function, offs points to the char's past the value
'        being read.
'
'        This function raises an error in case unexpected symbol is found
'        ***********************************************************************

		Private Shared Sub int2str(v As Integer, buf As Char(), ByRef offs As Integer)
			Dim i As Integer
			Dim _bytes As Byte() = System.BitConverter.GetBytes(CInt(v))
			Dim bytes As Byte() = New Byte(8) {}
			Dim sixbits As Integer() = New Integer(11) {}
			Dim c As Byte

			'
			' copy v to array of bytes, sign extending it and 
			' converting to little endian order. Additionally, 
			' we set 9th byte to zero in order to simplify 
			' conversion to six-bit representation
			'
			If Not System.BitConverter.IsLittleEndian Then
				System.Array.Reverse(_bytes)
			End If
			c = If(v < 0, CByte(&Hff), CByte(&H0))
			For i = 0 To 4 - 1
				bytes(i) = _bytes(i)
			Next
			For i = 4 To 7
				bytes(i) = c
			Next
			bytes(8) = 0

			'
			' convert to six-bit representation, output
			'
			' NOTE: last 12th element of sixbits is always zero, we do not output it
			'
			threebytes2foursixbits(bytes, 0, sixbits, 0)
			threebytes2foursixbits(bytes, 3, sixbits, 4)
			threebytes2foursixbits(bytes, 6, sixbits, 8)
			For i = 0 To SER_ENTRY_LENGTH - 1
				buf(offs + i) = sixbits2char(sixbits(i))
			Next
			offs += SER_ENTRY_LENGTH
		End Sub

		'***********************************************************************
'        This function unserializes integer value from string
'
'        buf         buffer which contains value; leading spaces/tabs/newlines are 
'                    ignored, traling spaces/tabs/newlines are treated as  end  of
'                    the integer value.
'        offs        offset in the buffer
'        
'        after return from this function, offs points to the char's past the value
'        being read.
'
'        This function raises an error in case unexpected symbol is found
'        ***********************************************************************

		Private Shared Function str2int(buf As Char(), ByRef offs As Integer) As Integer
			Dim emsg As String = "ALGLIB: unable to read integer value from stream"
			Dim emsg3264 As String = "ALGLIB: unable to read integer value from stream (value does not fit into 32 bits)"
			Dim sixbits As Integer() = New Integer(11) {}
			Dim bytes As Byte() = New Byte(8) {}
			Dim _bytes As Byte() = New Byte(4 - 1) {}
			Dim sixbitsread As Integer, i As Integer
			Dim c As Byte

			' 
			' 1. skip leading spaces
			' 2. read and decode six-bit digits
			' 3. set trailing digits to zeros
			' 4. convert to little endian 64-bit integer representation
			' 5. check that we fit into int
			' 6. convert to big endian representation, if needed
			'
			sixbitsread = 0
			While buf(offs) = " "C OrElse buf(offs) = ControlChars.Tab OrElse buf(offs) = ControlChars.Lf OrElse buf(offs) = ControlChars.Cr
				offs += 1
			End While
            While buf(offs) <> " "c AndAlso buf(offs) <> ControlChars.Tab AndAlso buf(offs) <> ControlChars.Lf AndAlso buf(offs) <> ControlChars.Cr AndAlso AscW(buf(offs)) <> 0
                Dim d As Integer
                d = char2sixbits(buf(offs))
                If d < 0 OrElse sixbitsread >= SER_ENTRY_LENGTH Then
                    Throw New alglib.alglibexception(emsg)
                End If
                sixbits(sixbitsread) = d
                sixbitsread += 1
                offs += 1
            End While
			If sixbitsread = 0 Then
				Throw New alglib.alglibexception(emsg)
			End If
			For i = sixbitsread To 11
				sixbits(i) = 0
			Next
			foursixbits2threebytes(sixbits, 0, bytes, 0)
			foursixbits2threebytes(sixbits, 4, bytes, 3)
			foursixbits2threebytes(sixbits, 8, bytes, 6)
			c = If((bytes(4 - 1) And &H80) <> 0, CByte(&Hff), CByte(&H0))
			For i = 4 To 7
				If bytes(i) <> c Then
					Throw New alglib.alglibexception(emsg3264)
				End If
			Next
			For i = 0 To 4 - 1
				_bytes(i) = bytes(i)
			Next
			If Not System.BitConverter.IsLittleEndian Then
				System.Array.Reverse(_bytes)
			End If
			Return System.BitConverter.ToInt32(_bytes, 0)
		End Function


		'***********************************************************************
'        This function serializes double value into buffer
'
'        v           double value to be serialized
'        buf         buffer, at least 11 characters wide 
'        offs        offset in the buffer
'        
'        after return from this function, offs points to the char's past the value
'        being read.
'        ***********************************************************************

		Private Shared Sub double2str(v As Double, buf As Char(), ByRef offs As Integer)
			Dim i As Integer
			Dim sixbits As Integer() = New Integer(11) {}
			Dim bytes As Byte() = New Byte(8) {}

			'
			' handle special quantities
			'
			If System.[Double].IsNaN(v) Then
				buf(offs + 0) = "."C
				buf(offs + 1) = "n"C
				buf(offs + 2) = "a"C
				buf(offs + 3) = "n"C
				buf(offs + 4) = "_"C
				buf(offs + 5) = "_"C
				buf(offs + 6) = "_"C
				buf(offs + 7) = "_"C
				buf(offs + 8) = "_"C
				buf(offs + 9) = "_"C
				buf(offs + 10) = "_"C
				offs += SER_ENTRY_LENGTH
				Return
			End If
			If System.[Double].IsPositiveInfinity(v) Then
				buf(offs + 0) = "."C
				buf(offs + 1) = "p"C
				buf(offs + 2) = "o"C
				buf(offs + 3) = "s"C
				buf(offs + 4) = "i"C
				buf(offs + 5) = "n"C
				buf(offs + 6) = "f"C
				buf(offs + 7) = "_"C
				buf(offs + 8) = "_"C
				buf(offs + 9) = "_"C
				buf(offs + 10) = "_"C
				offs += SER_ENTRY_LENGTH
				Return
			End If
			If System.[Double].IsNegativeInfinity(v) Then
				buf(offs + 0) = "."C
				buf(offs + 1) = "n"C
				buf(offs + 2) = "e"C
				buf(offs + 3) = "g"C
				buf(offs + 4) = "i"C
				buf(offs + 5) = "n"C
				buf(offs + 6) = "f"C
				buf(offs + 7) = "_"C
				buf(offs + 8) = "_"C
				buf(offs + 9) = "_"C
				buf(offs + 10) = "_"C
				offs += SER_ENTRY_LENGTH
				Return
			End If

			'
			' process general case:
			' 1. copy v to array of chars
			' 2. set 9th byte to zero in order to simplify conversion to six-bit representation
			' 3. convert to little endian (if needed)
			' 4. convert to six-bit representation
			'    (last 12th element of sixbits is always zero, we do not output it)
			'
			Dim _bytes As Byte() = System.BitConverter.GetBytes(CDbl(v))
			If Not System.BitConverter.IsLittleEndian Then
				System.Array.Reverse(_bytes)
			End If
			For i = 0 To 8 - 1
				bytes(i) = _bytes(i)
			Next
			For i = 8 To 8
				bytes(i) = 0
			Next
			threebytes2foursixbits(bytes, 0, sixbits, 0)
			threebytes2foursixbits(bytes, 3, sixbits, 4)
			threebytes2foursixbits(bytes, 6, sixbits, 8)
			For i = 0 To SER_ENTRY_LENGTH - 1
				buf(offs + i) = sixbits2char(sixbits(i))
			Next
			offs += SER_ENTRY_LENGTH
		End Sub

		'***********************************************************************
'        This function unserializes double value from string
'
'        buf         buffer which contains value; leading spaces/tabs/newlines are 
'                    ignored, traling spaces/tabs/newlines are treated as  end  of
'                    the double value.
'        offs        offset in the buffer
'        
'        after return from this function, offs points to the char's past the value
'        being read.
'
'        This function raises an error in case unexpected symbol is found
'        ***********************************************************************

		Private Shared Function str2double(buf As Char(), ByRef offs As Integer) As Double
			Dim emsg As String = "ALGLIB: unable to read double value from stream"
			Dim sixbits As Integer() = New Integer(11) {}
			Dim bytes As Byte() = New Byte(8) {}
			Dim _bytes As Byte() = New Byte(8 - 1) {}
			Dim sixbitsread As Integer, i As Integer


			' 
			' skip leading spaces
			'
			While buf(offs) = " "C OrElse buf(offs) = ControlChars.Tab OrElse buf(offs) = ControlChars.Lf OrElse buf(offs) = ControlChars.Cr
				offs += 1
			End While


			'
			' Handle special cases
			'
			If buf(offs) = "."C Then
				Dim s As New String(buf, offs, SER_ENTRY_LENGTH)
				If s = ".nan_______" Then
					offs += SER_ENTRY_LENGTH
					Return System.[Double].NaN
				End If
				If s = ".posinf____" Then
					offs += SER_ENTRY_LENGTH
					Return System.[Double].PositiveInfinity
				End If
				If s = ".neginf____" Then
					offs += SER_ENTRY_LENGTH
					Return System.[Double].NegativeInfinity
				End If
				Throw New alglib.alglibexception(emsg)
			End If

			' 
			' General case:
			' 1. read and decode six-bit digits
			' 2. check that all 11 digits were read
			' 3. set last 12th digit to zero (needed for simplicity of conversion)
			' 4. convert to 8 bytes
			' 5. convert to big endian representation, if needed
			'
			sixbitsread = 0
            While buf(offs) <> " "c AndAlso buf(offs) <> ControlChars.Tab AndAlso buf(offs) <> ControlChars.Lf AndAlso buf(offs) <> ControlChars.Cr AndAlso AscW(buf(offs)) <> 0
                Dim d As Integer
                d = char2sixbits(buf(offs))
                If d < 0 OrElse sixbitsread >= SER_ENTRY_LENGTH Then
                    Throw New alglib.alglibexception(emsg)
                End If
                sixbits(sixbitsread) = d
                sixbitsread += 1
                offs += 1
            End While
			If sixbitsread <> SER_ENTRY_LENGTH Then
				Throw New alglib.alglibexception(emsg)
			End If
			sixbits(SER_ENTRY_LENGTH) = 0
			foursixbits2threebytes(sixbits, 0, bytes, 0)
			foursixbits2threebytes(sixbits, 4, bytes, 3)
			foursixbits2threebytes(sixbits, 8, bytes, 6)
			For i = 0 To 8 - 1
				_bytes(i) = bytes(i)
			Next
			If Not System.BitConverter.IsLittleEndian Then
				System.Array.Reverse(_bytes)
			End If
			Return System.BitConverter.ToDouble(_bytes, 0)
		End Function
	End Class

	'
'     * Parts of alglib.smp class which are shared with GPL version of ALGLIB
'     

	Public Partial Class smp
		'#Pragma warning disable 420
		Public Const AE_LOCK_CYCLES As Integer = 512
		Public Const AE_LOCK_TESTS_BEFORE_YIELD As Integer = 16

		'
'         * This variable is used to perform spin-wait loops in a platform-independent manner
'         * (loops which should work same way on Mono and Microsoft NET). You SHOULD NEVER
'         * change this field - it must be zero during all program life.
'         

		Public Shared never_change_it As Integer = 0

		'************************************************************************
'        Lock.
'
'        This class provides lightweight spin lock
'        ************************************************************************

		Public Class ae_lock
			Public is_locked As Integer
		End Class

		'*******************************************************************
'        Shared pool: data structure used to provide thread-safe access to pool
'        of temporary variables.
'        *******************************************************************

		Public Class sharedpoolentry
			Public obj As apobject
			Public next_entry As sharedpoolentry
		End Class
		Public Class shared_pool
			Inherits apobject
			' lock object which protects pool 

			Public pool_lock As ae_lock

			' seed object (used to create new instances of temporaries) 

			Public seed_object As apobject

			'
'             * list of recycled OBJECTS:
'             * 1. entries in this list store pointers to recycled objects
'             * 2. every time we retrieve object, we retrieve first entry from this list,
'             *    move it to recycled_entries and return its obj field to caller/
'             

			Public recycled_objects As sharedpoolentry

			' 
'             * list of recycled ENTRIES:
'             * 1. this list holds entries which are not used to store recycled objects;
'             *    every time recycled object is retrieved, its entry is moved to this list.
'             * 2. every time object is recycled, we try to fetch entry for him from this list
'             *    before allocating it with malloc()
'             

			Public recycled_entries As sharedpoolentry

			' enumeration pointer, points to current recycled object

			Public enumeration_counter As sharedpoolentry

			' constructor 

			Public Sub New()
				ae_init_lock(pool_lock)
			End Sub

			' initializer - creation of empty pool 

			Public Overrides Sub init()
				seed_object = Nothing
				recycled_objects = Nothing
				recycled_entries = Nothing
				enumeration_counter = Nothing
			End Sub

			' copy constructor (it is NOT thread-safe) 

			Public Overrides Function make_copy() As apobject
				Dim ptr As sharedpoolentry, buf As sharedpoolentry
				Dim result As New shared_pool()

				' create lock 

				ae_init_lock(result.pool_lock)

				' copy seed object 

				If seed_object IsNot Nothing Then
					result.seed_object = seed_object.make_copy()
				End If

				'
'                 * copy recycled objects:
'                 * 1. copy to temporary list (objects are inserted to beginning, order is reversed)
'                 * 2. copy temporary list to output list (order is restored back to normal)
'                 

				buf = Nothing
				ptr = recycled_objects
				While ptr IsNot Nothing
					Dim tmp As New sharedpoolentry()
					tmp.obj = ptr.obj.make_copy()
					tmp.next_entry = buf
					buf = tmp
					ptr = ptr.next_entry
				End While
				result.recycled_objects = Nothing
				ptr = buf
				While ptr IsNot Nothing
					Dim next_ptr As sharedpoolentry = ptr.next_entry
					ptr.next_entry = result.recycled_objects
					result.recycled_objects = ptr
					ptr = next_ptr
				End While

				' recycled entries are not copied because they do not store any information 

				result.recycled_entries = Nothing

				' enumeration counter is reset on copying 

				result.enumeration_counter = Nothing

				Return result
			End Function
		End Class


		'***********************************************************************
'        This function performs given number of spin-wait iterations
'        ***********************************************************************

		Public Shared Sub ae_spin_wait(cnt As Integer)
			'
'             * these strange operations with ae_never_change_it are necessary to
'             * prevent compiler optimization of the loop.
'             

			Dim i As Integer

			' very unlikely because no one will wait for such amount of cycles 

			If cnt > &H12345678 Then
				never_change_it = cnt Mod 10
			End If

			' spin wait, test condition which will never be true 

			For i = 0 To cnt - 1
				If never_change_it > 0 Then
					never_change_it -= 1
				End If
			Next
		End Sub


		'***********************************************************************
'        This function causes the calling thread to relinquish the CPU. The thread
'        is moved to the end of the queue and some other thread gets to run.
'        ***********************************************************************

		Public Shared Sub ae_yield()
			System.Threading.Thread.Sleep(0)
		End Sub

		'***********************************************************************
'        This function initializes ae_lock structure and sets lock in a free mode.
'        ***********************************************************************

		Public Shared Sub ae_init_lock(ByRef obj As ae_lock)
			obj = New ae_lock()
			obj.is_locked = 0
		End Sub


		'***********************************************************************
'        This function acquires lock. In case lock is busy, we perform several
'        iterations inside tight loop before trying again.
'        ***********************************************************************

		Public Shared Sub ae_acquire_lock(obj As ae_lock)
			Dim cnt As Integer = 0
			While True
				If System.Threading.Interlocked.CompareExchange(obj.is_locked, 1, 0) = 0 Then
					Return
				End If
				ae_spin_wait(AE_LOCK_CYCLES)
				cnt += 1
				If cnt Mod AE_LOCK_TESTS_BEFORE_YIELD = 0 Then
					ae_yield()
				End If
			End While
		End Sub


		'***********************************************************************
'        This function releases lock.
'        ***********************************************************************

		Public Shared Sub ae_release_lock(obj As ae_lock)
			System.Threading.Interlocked.Exchange(obj.is_locked, 0)
		End Sub


		'***********************************************************************
'        This function frees ae_lock structure.
'        ***********************************************************************

		Public Shared Sub ae_free_lock(ByRef obj As ae_lock)
			obj = Nothing
		End Sub


		'***********************************************************************
'        This function returns True, if internal seed object was set.  It  returns
'        False for un-seeded pool.
'
'        dst                 destination pool (initialized by constructor function)
'
'        NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
'              you should NOT call it when lock can be used by another thread.
'        ***********************************************************************

		Public Shared Function ae_shared_pool_is_initialized(dst As shared_pool) As Boolean
			Return dst.seed_object IsNot Nothing
		End Function


		'***********************************************************************
'        This function sets internal seed object. All objects owned by the pool
'        (current seed object, recycled objects) are automatically freed.
'
'        dst                 destination pool (initialized by constructor function)
'        seed_object         new seed object
'
'        NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
'              you should NOT call it when lock can be used by another thread.
'        ***********************************************************************

		Public Shared Sub ae_shared_pool_set_seed(dst As shared_pool, seed_object As alglib.apobject)
			dst.seed_object = seed_object.make_copy()
			dst.recycled_objects = Nothing
			dst.enumeration_counter = Nothing
		End Sub


		'***********************************************************************
'        This  function  retrieves  a  copy  of  the seed object from the pool and
'        stores it to target variable.
'
'        pool                pool
'        obj                 target variable
'        
'        NOTE: this function IS thread-safe.  It  acquires  pool  lock  during its
'              operation and can be used simultaneously from several threads.
'        ***********************************************************************

		Public Shared Sub ae_shared_pool_retrieve(Of T As alglib.apobject)(pool As shared_pool, ByRef obj As T)
			Dim new_obj As alglib.apobject

			' assert that pool was seeded 

			alglib.ap.assert(pool.seed_object IsNot Nothing, "ALGLIB: shared pool is not seeded, PoolRetrieve() failed")

			' acquire lock 

			ae_acquire_lock(pool.pool_lock)

			' try to reuse recycled objects 

			If pool.recycled_objects IsNot Nothing Then
				' retrieve entry/object from list of recycled objects 

				Dim result As sharedpoolentry = pool.recycled_objects
				pool.recycled_objects = pool.recycled_objects.next_entry
				new_obj = result.obj
				result.obj = Nothing

				' move entry to list of recycled entries 

				result.next_entry = pool.recycled_entries
				pool.recycled_entries = result

				' release lock 

				ae_release_lock(pool.pool_lock)

				' assign object to smart pointer 

				obj = DirectCast(new_obj, T)

				Return
			End If

			'
'             * release lock; we do not need it anymore because
'             * copy constructor does not modify source variable.
'             

			ae_release_lock(pool.pool_lock)

			' create new object from seed 

			new_obj = pool.seed_object.make_copy()

			' assign object to pointer and return 

			obj = DirectCast(new_obj, T)
		End Sub


		'***********************************************************************
'        This  function  recycles object owned by the source variable by moving it
'        to internal storage of the shared pool.
'
'        Source  variable  must  own  the  object,  i.e.  be  the only place where
'        reference  to  object  is  stored.  After  call  to  this function source
'        variable becomes NULL.
'
'        pool                pool
'        obj                 source variable
'
'        NOTE: this function IS thread-safe.  It  acquires  pool  lock  during its
'              operation and can be used simultaneously from several threads.
'        ***********************************************************************

		Public Shared Sub ae_shared_pool_recycle(Of T As alglib.apobject)(pool As shared_pool, ByRef obj As T)
			Dim new_entry As sharedpoolentry

			' assert that pool was seeded 

			alglib.ap.assert(pool.seed_object IsNot Nothing, "ALGLIB: shared pool is not seeded, PoolRecycle() failed")

			' assert that pointer non-null 

			alglib.ap.assert(obj IsNot Nothing, "ALGLIB: obj in ae_shared_pool_recycle() is NULL")

			' acquire lock 

			ae_acquire_lock(pool.pool_lock)

			' acquire shared pool entry (reuse one from recycled_entries or malloc new one) 

			If pool.recycled_entries IsNot Nothing Then
				' reuse previously allocated entry 

				new_entry = pool.recycled_entries
				pool.recycled_entries = new_entry.next_entry
			Else
				'
'                 * Allocate memory for new entry.
'                 *
'                 * NOTE: we release pool lock during allocation because new() may raise
'                 *       exception and we do not want our pool to be left in the locked state.
'                 

				ae_release_lock(pool.pool_lock)
				new_entry = New sharedpoolentry()
				ae_acquire_lock(pool.pool_lock)
			End If

			' add object to the list of recycled objects 

			new_entry.obj = obj
			new_entry.next_entry = pool.recycled_objects
			pool.recycled_objects = new_entry

			' release lock object 

			ae_release_lock(pool.pool_lock)

			' release source pointer 

			obj = Nothing
		End Sub


		'***********************************************************************
'        This function clears internal list of  recycled  objects,  but  does  not
'        change seed object managed by the pool.
'
'        pool                pool
'
'        NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
'              you should NOT call it when lock can be used by another thread.
'        ***********************************************************************

		Public Shared Sub ae_shared_pool_clear_recycled(pool As shared_pool)
			pool.recycled_objects = Nothing
		End Sub


		'***********************************************************************
'        This function allows to enumerate recycled elements of the  shared  pool.
'        It stores reference to the first recycled object in the smart pointer.
'
'        IMPORTANT:
'        * in case target variable owns non-NULL value, it is rewritten
'        * recycled object IS NOT removed from pool
'        * target variable DOES NOT become owner of the new value; you can use
'          reference to recycled object, but you do not own it.
'        * this function IS NOT thread-safe
'        * you SHOULD NOT modify shared pool during enumeration (although you  can
'          modify state of the objects retrieved from pool)
'        * in case there is no recycled objects in the pool, NULL is stored to obj
'        * in case pool is not seeded, NULL is stored to obj
'
'        pool                pool
'        obj                 reference
'        ***********************************************************************

		Public Shared Sub ae_shared_pool_first_recycled(Of T As alglib.apobject)(pool As shared_pool, ByRef obj As T)
			' modify internal enumeration counter 

			pool.enumeration_counter = pool.recycled_objects

			' exit on empty list 

			If pool.enumeration_counter Is Nothing Then
				obj = Nothing
				Return
			End If

			' assign object to smart pointer 

			obj = DirectCast(pool.enumeration_counter.obj, T)
		End Sub


		'***********************************************************************
'        This function allows to enumerate recycled elements of the  shared  pool.
'        It stores pointer to the next recycled object in the smart pointer.
'
'        IMPORTANT:
'        * in case target variable owns non-NULL value, it is rewritten
'        * recycled object IS NOT removed from pool
'        * target pointer DOES NOT become owner of the new value
'        * this function IS NOT thread-safe
'        * you SHOULD NOT modify shared pool during enumeration (although you  can
'          modify state of the objects retrieved from pool)
'        * in case there is no recycled objects left in the pool, NULL is stored.
'        * in case pool is not seeded, NULL is stored.
'
'        pool                pool
'        obj                 target variable
'        ***********************************************************************

		Public Shared Sub ae_shared_pool_next_recycled(Of T As alglib.apobject)(pool As shared_pool, ByRef obj As T)
			' exit on end of list 

			If pool.enumeration_counter Is Nothing Then
				obj = Nothing
				Return
			End If

			' modify internal enumeration counter 

			pool.enumeration_counter = pool.enumeration_counter.next_entry

			' exit on empty list 

			If pool.enumeration_counter Is Nothing Then
				obj = Nothing
				Return
			End If

			' assign object to smart pointer 

			obj = DirectCast(pool.enumeration_counter.obj, T)
		End Sub


		'***********************************************************************
'        This function clears internal list of recycled objects and  seed  object.
'        However, pool still can be used (after initialization with another seed).
'
'        pool                pool
'        state               ALGLIB environment state
'
'        NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
'              you should NOT call it when lock can be used by another thread.
'        ***********************************************************************

		Public Shared Sub ae_shared_pool_reset(pool As shared_pool)
			pool.seed_object = Nothing
			pool.recycled_objects = Nothing
			pool.enumeration_counter = Nothing
		End Sub
	End Class
End Class
Public Partial Class alglib
	Public Partial Class smp
		Public Shared cores_count As Integer = 1
		Public Shared cores_to_use As Integer = 0
	End Class
	Public Class smpselftests
		Public Shared Function runtests() As Boolean
			Return True
		End Function
	End Class
	Public Shared Sub setnworkers(nworkers As Integer)
		alglib.smp.cores_to_use = nworkers
	End Sub
End Class
