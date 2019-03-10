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



End Class
Public Partial Class alglib
	Public Class apserv
		'************************************************************************
'        Buffers for internal functions which need buffers:
'        * check for size of the buffer you want to use.
'        * if buffer is too small, resize it; leave unchanged, if it is larger than
'          needed.
'        * use it.
'
'        We can pass this structure to multiple functions;  after first run through
'        functions buffer sizes will be finally determined,  and  on  a next run no
'        allocation will be required.
'        ************************************************************************

		Public Class apbuffers
			Inherits apobject
			Public ia0 As Integer()
			Public ia1 As Integer()
			Public ia2 As Integer()
			Public ia3 As Integer()
			Public ra0 As Double()
			Public ra1 As Double()
			Public ra2 As Double()
			Public ra3 As Double()
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
				ia0 = New Integer(-1) {}
				ia1 = New Integer(-1) {}
				ia2 = New Integer(-1) {}
				ia3 = New Integer(-1) {}
				ra0 = New Double(-1) {}
				ra1 = New Double(-1) {}
				ra2 = New Double(-1) {}
				ra3 = New Double(-1) {}
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New apbuffers()
				_result.ia0 = DirectCast(ia0.Clone(), Integer())
				_result.ia1 = DirectCast(ia1.Clone(), Integer())
				_result.ia2 = DirectCast(ia2.Clone(), Integer())
				_result.ia3 = DirectCast(ia3.Clone(), Integer())
				_result.ra0 = DirectCast(ra0.Clone(), Double())
				_result.ra1 = DirectCast(ra1.Clone(), Double())
				_result.ra2 = DirectCast(ra2.Clone(), Double())
				_result.ra3 = DirectCast(ra3.Clone(), Double())
				Return _result
			End Function
		End Class


		'************************************************************************
'        Structure which is used to workaround limitations of ALGLIB parallellization
'        environment.
'
'          -- ALGLIB --
'             Copyright 12.04.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Class sboolean
			Inherits apobject
			Public val As Boolean
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New sboolean()
				_result.val = val
				Return _result
			End Function
		End Class


		'************************************************************************
'        Structure which is used to workaround limitations of ALGLIB parallellization
'        environment.
'
'          -- ALGLIB --
'             Copyright 12.04.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Class sbooleanarray
			Inherits apobject
			Public val As Boolean()
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
				val = New Boolean(-1) {}
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New sbooleanarray()
				_result.val = DirectCast(val.Clone(), Boolean())
				Return _result
			End Function
		End Class


		'************************************************************************
'        Structure which is used to workaround limitations of ALGLIB parallellization
'        environment.
'
'          -- ALGLIB --
'             Copyright 12.04.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Class sinteger
			Inherits apobject
			Public val As Integer
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New sinteger()
				_result.val = val
				Return _result
			End Function
		End Class


		'************************************************************************
'        Structure which is used to workaround limitations of ALGLIB parallellization
'        environment.
'
'          -- ALGLIB --
'             Copyright 12.04.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Class sintegerarray
			Inherits apobject
			Public val As Integer()
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
				val = New Integer(-1) {}
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New sintegerarray()
				_result.val = DirectCast(val.Clone(), Integer())
				Return _result
			End Function
		End Class


		'************************************************************************
'        Structure which is used to workaround limitations of ALGLIB parallellization
'        environment.
'
'          -- ALGLIB --
'             Copyright 12.04.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Class sreal
			Inherits apobject
			Public val As Double
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New sreal()
				_result.val = val
				Return _result
			End Function
		End Class


		'************************************************************************
'        Structure which is used to workaround limitations of ALGLIB parallellization
'        environment.
'
'          -- ALGLIB --
'             Copyright 12.04.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Class srealarray
			Inherits apobject
			Public val As Double()
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
				val = New Double(-1) {}
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New srealarray()
				_result.val = DirectCast(val.Clone(), Double())
				Return _result
			End Function
		End Class


		'************************************************************************
'        Structure which is used to workaround limitations of ALGLIB parallellization
'        environment.
'
'          -- ALGLIB --
'             Copyright 12.04.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Class scomplex
			Inherits apobject
			Public val As complex
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New scomplex()
				_result.val = val
				Return _result
			End Function
		End Class


		'************************************************************************
'        Structure which is used to workaround limitations of ALGLIB parallellization
'        environment.
'
'          -- ALGLIB --
'             Copyright 12.04.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Class scomplexarray
			Inherits apobject
			Public val As complex()
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
				val = New complex(-1) {}
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New scomplexarray()
				_result.val = DirectCast(val.Clone(), alglib.complex())
				Return _result
			End Function
		End Class




		'************************************************************************
'        This function is used to set error flags  during  unit  tests.  When  COND
'        parameter is True, FLAG variable is  set  to  True.  When  COND is  False,
'        FLAG is unchanged.
'
'        The purpose of this function is to have single  point  where  failures  of
'        unit tests can be detected.
'
'        This function returns value of COND.
'        ************************************************************************

		Public Shared Function seterrorflag(ByRef flag As Boolean, cond As Boolean) As Boolean
			Dim result As New Boolean()

			If cond Then
				flag = True
			End If
			result = cond
			Return result
		End Function


		'************************************************************************
'        Internally calls SetErrorFlag() with condition:
'
'            Abs(Val-RefVal)>Tol*Max(Abs(RefVal),S)
'            
'        This function is used to test relative error in Val against  RefVal,  with
'        relative error being replaced by absolute when scale  of  RefVal  is  less
'        than S.
'
'        This function returns value of COND.
'        ************************************************************************

		Public Shared Function seterrorflagdiff(ByRef flag As Boolean, val As Double, refval As Double, tol As Double, s As Double) As Boolean
			Dim result As New Boolean()

			result = seterrorflag(flag, CDbl(System.Math.Abs(val - refval)) > CDbl(tol * System.Math.Max(System.Math.Abs(refval), s)))
			Return result
		End Function


		'************************************************************************
'        The function "touches" integer - it is used  to  avoid  compiler  messages
'        about unused variables (in rare cases when we do NOT want to remove  these
'        variables).
'
'          -- ALGLIB --
'             Copyright 17.09.2012 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub touchint(ByRef a As Integer)
		End Sub


		'************************************************************************
'        The function "touches" real   -  it is used  to  avoid  compiler  messages
'        about unused variables (in rare cases when we do NOT want to remove  these
'        variables).
'
'          -- ALGLIB --
'             Copyright 17.09.2012 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub touchreal(ByRef a As Double)
		End Sub


		'************************************************************************
'        The function convert integer value to real value.
'
'          -- ALGLIB --
'             Copyright 17.09.2012 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function inttoreal(a As Integer) As Double
			Dim result As Double = 0

			result = a
			Return result
		End Function


		'************************************************************************
'        The function calculates binary logarithm.
'
'        NOTE: it costs twice as much as Ln(x)
'
'          -- ALGLIB --
'             Copyright 17.09.2012 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function logbase2(x As Double) As Double
			Dim result As Double = 0

			result = System.Math.Log(x) / System.Math.Log(2)
			Return result
		End Function


		'************************************************************************
'        This function compares two numbers for approximate equality, with tolerance
'        to errors as large as max(|a|,|b|)*tol.
'
'
'          -- ALGLIB --
'             Copyright 02.12.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function approxequalrel(a As Double, b As Double, tol As Double) As Boolean
			Dim result As New Boolean()

			result = CDbl(System.Math.Abs(a - b)) <= CDbl(System.Math.Max(System.Math.Abs(a), System.Math.Abs(b)) * tol)
			Return result
		End Function


		'************************************************************************
'        This  function  generates  1-dimensional  general  interpolation task with
'        moderate Lipshitz constant (close to 1.0)
'
'        If N=1 then suborutine generates only one point at the middle of [A,B]
'
'          -- ALGLIB --
'             Copyright 02.12.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub taskgenint1d(a As Double, b As Double, n As Integer, ByRef x As Double(), ByRef y As Double())
			Dim i As Integer = 0
			Dim h As Double = 0

			x = New Double(-1) {}
			y = New Double(-1) {}

			alglib.ap.assert(n >= 1, "TaskGenInterpolationEqdist1D: N<1!")
			x = New Double(n - 1) {}
			y = New Double(n - 1) {}
			If n > 1 Then
				x(0) = a
                y(0) = 2 * Math.randomreal() - 1
                h = (b - a) / (n - 1)
                For i = 1 To n - 1
                    If i <> n - 1 Then
                        x(i) = a + (i + 0.2 * (2 * Math.randomreal() - 1)) * h
                    Else
                        x(i) = b
                    End If
                    y(i) = y(i - 1) + (2 * Math.randomreal() - 1) * (x(i) - x(i - 1))
                Next
            Else
                x(0) = 0.5 * (a + b)
                y(0) = 2 * Math.randomreal() - 1
            End If
        End Sub


        '************************************************************************
        '        This function generates  1-dimensional equidistant interpolation task with
        '        moderate Lipshitz constant (close to 1.0)
        '
        '        If N=1 then suborutine generates only one point at the middle of [A,B]
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub taskgenint1dequidist(a As Double, b As Double, n As Integer, ByRef x As Double(), ByRef y As Double())
            Dim i As Integer = 0
            Dim h As Double = 0

            x = New Double(-1) {}
            y = New Double(-1) {}

            alglib.ap.assert(n >= 1, "TaskGenInterpolationEqdist1D: N<1!")
            x = New Double(n - 1) {}
            y = New Double(n - 1) {}
            If n > 1 Then
                x(0) = a
                y(0) = 2 * Math.randomreal() - 1
                h = (b - a) / (n - 1)
                For i = 1 To n - 1
                    x(i) = a + i * h
                    y(i) = y(i - 1) + (2 * Math.randomreal() - 1) * h
                Next
            Else
                x(0) = 0.5 * (a + b)
                y(0) = 2 * Math.randomreal() - 1
            End If
        End Sub


        '************************************************************************
        '        This function generates  1-dimensional Chebyshev-1 interpolation task with
        '        moderate Lipshitz constant (close to 1.0)
        '
        '        If N=1 then suborutine generates only one point at the middle of [A,B]
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub taskgenint1dcheb1(a As Double, b As Double, n As Integer, ByRef x As Double(), ByRef y As Double())
            Dim i As Integer = 0

            x = New Double(-1) {}
            y = New Double(-1) {}

            alglib.ap.assert(n >= 1, "TaskGenInterpolation1DCheb1: N<1!")
            x = New Double(n - 1) {}
            y = New Double(n - 1) {}
            If n > 1 Then
                For i = 0 To n - 1
                    x(i) = 0.5 * (b + a) + 0.5 * (b - a) * System.Math.Cos(System.Math.PI * (2 * i + 1) / (2 * n))
                    If i = 0 Then
                        y(i) = 2 * Math.randomreal() - 1
                    Else
                        y(i) = y(i - 1) + (2 * Math.randomreal() - 1) * (x(i) - x(i - 1))
                    End If
                Next
            Else
                x(0) = 0.5 * (a + b)
                y(0) = 2 * Math.randomreal() - 1
            End If
        End Sub


        '************************************************************************
        '        This function generates  1-dimensional Chebyshev-2 interpolation task with
        '        moderate Lipshitz constant (close to 1.0)
        '
        '        If N=1 then suborutine generates only one point at the middle of [A,B]
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub taskgenint1dcheb2(a As Double, b As Double, n As Integer, ByRef x As Double(), ByRef y As Double())
            Dim i As Integer = 0

            x = New Double(-1) {}
            y = New Double(-1) {}

            alglib.ap.assert(n >= 1, "TaskGenInterpolation1DCheb2: N<1!")
            x = New Double(n - 1) {}
            y = New Double(n - 1) {}
            If n > 1 Then
                For i = 0 To n - 1
                    x(i) = 0.5 * (b + a) + 0.5 * (b - a) * System.Math.Cos(System.Math.PI * i / (n - 1))
                    If i = 0 Then
                        y(i) = 2 * Math.randomreal() - 1
                    Else
                        y(i) = y(i - 1) + (2 * Math.randomreal() - 1) * (x(i) - x(i - 1))
                    End If
                Next
            Else
                x(0) = 0.5 * (a + b)
                y(0) = 2 * Math.randomreal() - 1
            End If
        End Sub


        '************************************************************************
        '        This function checks that all values from X[] are distinct. It does more
        '        than just usual floating point comparison:
        '        * first, it calculates max(X) and min(X)
        '        * second, it maps X[] from [min,max] to [1,2]
        '        * only at this stage actual comparison is done
        '
        '        The meaning of such check is to ensure that all values are "distinct enough"
        '        and will not cause interpolation subroutine to fail.
        '
        '        NOTE:
        '            X[] must be sorted by ascending (subroutine ASSERT's it)
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function aredistinct(x As Double(), n As Integer) As Boolean
            Dim result As New Boolean()
            Dim a As Double = 0
            Dim b As Double = 0
            Dim i As Integer = 0
            Dim nonsorted As New Boolean()

            alglib.ap.assert(n >= 1, "APSERVAreDistinct: internal error (N<1)")
            If n = 1 Then

                '
                ' everything is alright, it is up to caller to decide whether it
                ' can interpolate something with just one point
                '
                result = True
                Return result
            End If
            a = x(0)
            b = x(0)
            nonsorted = False
            For i = 1 To n - 1
                a = System.Math.Min(a, x(i))
                b = System.Math.Max(b, x(i))
                nonsorted = nonsorted OrElse CDbl(x(i - 1)) >= CDbl(x(i))
            Next
            alglib.ap.assert(Not nonsorted, "APSERVAreDistinct: internal error (not sorted)")
            For i = 1 To n - 1
                If CDbl((x(i) - a) / (b - a) + 1) = CDbl((x(i - 1) - a) / (b - a) + 1) Then
                    result = False
                    Return result
                End If
            Next
            result = True
            Return result
        End Function


        '************************************************************************
        '        This function checks that two boolean values are the same (both  are  True 
        '        or both are False).
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function aresameboolean(v1 As Boolean, v2 As Boolean) As Boolean
            Dim result As New Boolean()

            result = (v1 AndAlso v2) OrElse (Not v1 AndAlso Not v2)
            Return result
        End Function


        '************************************************************************
        '        If Length(X)<N, resizes X
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub bvectorsetlengthatleast(ByRef x As Boolean(), n As Integer)
            If alglib.ap.len(x) < n Then
                x = New Boolean(n - 1) {}
            End If
        End Sub


        '************************************************************************
        '        If Length(X)<N, resizes X
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub ivectorsetlengthatleast(ByRef x As Integer(), n As Integer)
            If alglib.ap.len(x) < n Then
                x = New Integer(n - 1) {}
            End If
        End Sub


        '************************************************************************
        '        If Length(X)<N, resizes X
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rvectorsetlengthatleast(ByRef x As Double(), n As Integer)
            If alglib.ap.len(x) < n Then
                x = New Double(n - 1) {}
            End If
        End Sub


        '************************************************************************
        '        If Cols(X)<N or Rows(X)<M, resizes X
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixsetlengthatleast(ByRef x As Double(,), m As Integer, n As Integer)
            If m > 0 AndAlso n > 0 Then
                If alglib.ap.rows(x) < m OrElse alglib.ap.cols(x) < n Then
                    x = New Double(m - 1, n - 1) {}
                End If
            End If
        End Sub


        '************************************************************************
        '        Resizes X and:
        '        * preserves old contents of X
        '        * fills new elements by zeros
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixresize(ByRef x As Double(,), m As Integer, n As Integer)
            Dim oldx As Double(,) = New Double(-1, -1) {}
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim m2 As Integer = 0
            Dim n2 As Integer = 0

            m2 = alglib.ap.rows(x)
            n2 = alglib.ap.cols(x)
            alglib.ap.swap(x, oldx)
            x = New Double(m - 1, n - 1) {}
            For i = 0 To m - 1
                For j = 0 To n - 1
                    If i < m2 AndAlso j < n2 Then
                        x(i, j) = oldx(i, j)
                    Else
                        x(i, j) = 0.0
                    End If
                Next
            Next
        End Sub


        '************************************************************************
        '        Resizes X and:
        '        * preserves old contents of X
        '        * fills new elements by zeros
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub imatrixresize(ByRef x As Integer(,), m As Integer, n As Integer)
            Dim oldx As Integer(,) = New Integer(-1, -1) {}
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim m2 As Integer = 0
            Dim n2 As Integer = 0

            m2 = alglib.ap.rows(x)
            n2 = alglib.ap.cols(x)
            alglib.ap.swap(x, oldx)
            x = New Integer(m - 1, n - 1) {}
            For i = 0 To m - 1
                For j = 0 To n - 1
                    If i < m2 AndAlso j < n2 Then
                        x(i, j) = oldx(i, j)
                    Else
                        x(i, j) = 0
                    End If
                Next
            Next
        End Sub


        '************************************************************************
        '        This function checks that length(X) is at least N and first N values  from
        '        X[] are finite
        '
        '          -- ALGLIB --
        '             Copyright 18.06.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function isfinitevector(x As Double(), n As Integer) As Boolean
            Dim result As New Boolean()
            Dim i As Integer = 0

            alglib.ap.assert(n >= 0, "APSERVIsFiniteVector: internal error (N<0)")
            If n = 0 Then
                result = True
                Return result
            End If
            If alglib.ap.len(x) < n Then
                result = False
                Return result
            End If
            For i = 0 To n - 1
                If Not Math.isfinite(x(i)) Then
                    result = False
                    Return result
                End If
            Next
            result = True
            Return result
        End Function


        '************************************************************************
        '        This function checks that first N values from X[] are finite
        '
        '          -- ALGLIB --
        '             Copyright 18.06.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function isfinitecvector(z As complex(), n As Integer) As Boolean
            Dim result As New Boolean()
            Dim i As Integer = 0

            alglib.ap.assert(n >= 0, "APSERVIsFiniteCVector: internal error (N<0)")
            For i = 0 To n - 1
                If Not Math.isfinite(z(i).x) OrElse Not Math.isfinite(z(i).y) Then
                    result = False
                    Return result
                End If
            Next
            result = True
            Return result
        End Function


        '************************************************************************
        '        This function checks that size of X is at least MxN and values from
        '        X[0..M-1,0..N-1] are finite.
        '
        '          -- ALGLIB --
        '             Copyright 18.06.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function apservisfinitematrix(x As Double(,), m As Integer, n As Integer) As Boolean
            Dim result As New Boolean()
            Dim i As Integer = 0
            Dim j As Integer = 0

            alglib.ap.assert(n >= 0, "APSERVIsFiniteMatrix: internal error (N<0)")
            alglib.ap.assert(m >= 0, "APSERVIsFiniteMatrix: internal error (M<0)")
            If m = 0 OrElse n = 0 Then
                result = True
                Return result
            End If
            If alglib.ap.rows(x) < m OrElse alglib.ap.cols(x) < n Then
                result = False
                Return result
            End If
            For i = 0 To m - 1
                For j = 0 To n - 1
                    If Not Math.isfinite(x(i, j)) Then
                        result = False
                        Return result
                    End If
                Next
            Next
            result = True
            Return result
        End Function


        '************************************************************************
        '        This function checks that all values from X[0..M-1,0..N-1] are finite
        '
        '          -- ALGLIB --
        '             Copyright 18.06.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function apservisfinitecmatrix(x As complex(,), m As Integer, n As Integer) As Boolean
            Dim result As New Boolean()
            Dim i As Integer = 0
            Dim j As Integer = 0

            alglib.ap.assert(n >= 0, "APSERVIsFiniteCMatrix: internal error (N<0)")
            alglib.ap.assert(m >= 0, "APSERVIsFiniteCMatrix: internal error (M<0)")
            For i = 0 To m - 1
                For j = 0 To n - 1
                    If Not Math.isfinite(x(i, j).x) OrElse Not Math.isfinite(x(i, j).y) Then
                        result = False
                        Return result
                    End If
                Next
            Next
            result = True
            Return result
        End Function


        '************************************************************************
        '        This function checks that size of X is at least NxN and all values from
        '        upper/lower triangle of X[0..N-1,0..N-1] are finite
        '
        '          -- ALGLIB --
        '             Copyright 18.06.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function isfinitertrmatrix(x As Double(,), n As Integer, isupper As Boolean) As Boolean
            Dim result As New Boolean()
            Dim i As Integer = 0
            Dim j1 As Integer = 0
            Dim j2 As Integer = 0
            Dim j As Integer = 0

            alglib.ap.assert(n >= 0, "APSERVIsFiniteRTRMatrix: internal error (N<0)")
            If n = 0 Then
                result = True
                Return result
            End If
            If alglib.ap.rows(x) < n OrElse alglib.ap.cols(x) < n Then
                result = False
                Return result
            End If
            For i = 0 To n - 1
                If isupper Then
                    j1 = i
                    j2 = n - 1
                Else
                    j1 = 0
                    j2 = i
                End If
                For j = j1 To j2
                    If Not Math.isfinite(x(i, j)) Then
                        result = False
                        Return result
                    End If
                Next
            Next
            result = True
            Return result
        End Function


        '************************************************************************
        '        This function checks that all values from upper/lower triangle of
        '        X[0..N-1,0..N-1] are finite
        '
        '          -- ALGLIB --
        '             Copyright 18.06.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function apservisfinitectrmatrix(x As complex(,), n As Integer, isupper As Boolean) As Boolean
            Dim result As New Boolean()
            Dim i As Integer = 0
            Dim j1 As Integer = 0
            Dim j2 As Integer = 0
            Dim j As Integer = 0

            alglib.ap.assert(n >= 0, "APSERVIsFiniteCTRMatrix: internal error (N<0)")
            For i = 0 To n - 1
                If isupper Then
                    j1 = i
                    j2 = n - 1
                Else
                    j1 = 0
                    j2 = i
                End If
                For j = j1 To j2
                    If Not Math.isfinite(x(i, j).x) OrElse Not Math.isfinite(x(i, j).y) Then
                        result = False
                        Return result
                    End If
                Next
            Next
            result = True
            Return result
        End Function


        '************************************************************************
        '        This function checks that all values from X[0..M-1,0..N-1] are  finite  or
        '        NaN's.
        '
        '          -- ALGLIB --
        '             Copyright 18.06.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function apservisfiniteornanmatrix(x As Double(,), m As Integer, n As Integer) As Boolean
            Dim result As New Boolean()
            Dim i As Integer = 0
            Dim j As Integer = 0

            alglib.ap.assert(n >= 0, "APSERVIsFiniteOrNaNMatrix: internal error (N<0)")
            alglib.ap.assert(m >= 0, "APSERVIsFiniteOrNaNMatrix: internal error (M<0)")
            For i = 0 To m - 1
                For j = 0 To n - 1
                    If Not (Math.isfinite(x(i, j)) OrElse [Double].IsNaN(x(i, j))) Then
                        result = False
                        Return result
                    End If
                Next
            Next
            result = True
            Return result
        End Function


        '************************************************************************
        '        Safe sqrt(x^2+y^2)
        '
        '          -- ALGLIB --
        '             Copyright by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function safepythag2(x As Double, y As Double) As Double
            Dim result As Double = 0
            Dim w As Double = 0
            Dim xabs As Double = 0
            Dim yabs As Double = 0
            Dim z As Double = 0

            xabs = System.Math.Abs(x)
            yabs = System.Math.Abs(y)
            w = System.Math.Max(xabs, yabs)
            z = System.Math.Min(xabs, yabs)
            If CDbl(z) = CDbl(0) Then
                result = w
            Else
                result = w * System.Math.sqrt(1 + Math.sqr(z / w))
            End If
            Return result
        End Function


        '************************************************************************
        '        Safe sqrt(x^2+y^2)
        '
        '          -- ALGLIB --
        '             Copyright by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function safepythag3(x As Double, y As Double, z As Double) As Double
            Dim result As Double = 0
            Dim w As Double = 0

            w = System.Math.Max(System.Math.Abs(x), System.Math.Max(System.Math.Abs(y), System.Math.Abs(z)))
            If CDbl(w) = CDbl(0) Then
                result = 0
                Return result
            End If
            x = x / w
            y = y / w
            z = z / w
            result = w * System.Math.sqrt(Math.sqr(x) + Math.sqr(y) + Math.sqr(z))
            Return result
        End Function


        '************************************************************************
        '        Safe division.
        '
        '        This function attempts to calculate R=X/Y without overflow.
        '
        '        It returns:
        '        * +1, if abs(X/Y)>=MaxRealNumber or undefined - overflow-like situation
        '              (no overlfow is generated, R is either NAN, PosINF, NegINF)
        '        *  0, if MinRealNumber<abs(X/Y)<MaxRealNumber or X=0, Y<>0
        '              (R contains result, may be zero)
        '        * -1, if 0<abs(X/Y)<MinRealNumber - underflow-like situation
        '              (R contains zero; it corresponds to underflow)
        '
        '        No overflow is generated in any case.
        '
        '          -- ALGLIB --
        '             Copyright by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function saferdiv(x As Double, y As Double, ByRef r As Double) As Integer
            Dim result As Integer = 0

            r = 0


            '
            ' Two special cases:
            ' * Y=0
            ' * X=0 and Y<>0
            '
            If CDbl(y) = CDbl(0) Then
                result = 1
                If CDbl(x) = CDbl(0) Then
                    r = [Double].NaN
                End If
                If CDbl(x) > CDbl(0) Then
                    r = [Double].PositiveInfinity
                End If
                If CDbl(x) < CDbl(0) Then
                    r = [Double].NegativeInfinity
                End If
                Return result
            End If
            If CDbl(x) = CDbl(0) Then
                r = 0
                result = 0
                Return result
            End If

            '
            ' make Y>0
            '
            If CDbl(y) < CDbl(0) Then
                x = -x
                y = -y
            End If

            '
            '
            '
            If CDbl(y) >= CDbl(1) Then
                r = x / y
                If CDbl(System.Math.Abs(r)) <= CDbl(Math.minrealnumber) Then
                    result = -1
                    r = 0
                Else
                    result = 0
                End If
            Else
                If CDbl(System.Math.Abs(x)) >= CDbl(Math.maxrealnumber * y) Then
                    If CDbl(x) > CDbl(0) Then
                        r = [Double].PositiveInfinity
                    Else
                        r = [Double].NegativeInfinity
                    End If
                    result = 1
                Else
                    r = x / y
                    result = 0
                End If
            End If
            Return result
        End Function


        '************************************************************************
        '        This function calculates "safe" min(X/Y,V) for positive finite X, Y, V.
        '        No overflow is generated in any case.
        '
        '          -- ALGLIB --
        '             Copyright by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function safeminposrv(x As Double, y As Double, v As Double) As Double
            Dim result As Double = 0
            Dim r As Double = 0

            If CDbl(y) >= CDbl(1) Then

                '
                ' Y>=1, we can safely divide by Y
                '
                r = x / y
                result = v
                If CDbl(v) > CDbl(r) Then
                    result = r
                Else
                    result = v
                End If
            Else

                '
                ' Y<1, we can safely multiply by Y
                '
                If CDbl(x) < CDbl(v * y) Then
                    result = x / y
                Else
                    result = v
                End If
            End If
            Return result
        End Function


        '************************************************************************
        '        This function makes periodic mapping of X to [A,B].
        '
        '        It accepts X, A, B (A>B). It returns T which lies in  [A,B] and integer K,
        '        such that X = T + K*(B-A).
        '
        '        NOTES:
        '        * K is represented as real value, although actually it is integer
        '        * T is guaranteed to be in [A,B]
        '        * T replaces X
        '
        '          -- ALGLIB --
        '             Copyright by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub apperiodicmap(ByRef x As Double, a As Double, b As Double, ByRef k As Double)
            k = 0

            alglib.ap.assert(CDbl(a) < CDbl(b), "APPeriodicMap: internal error!")
            k = CInt(System.Math.Truncate(System.Math.Floor((x - a) / (b - a))))
            x = x - k * (b - a)
            While CDbl(x) < CDbl(a)
                x = x + (b - a)
                k = k - 1
            End While
            While CDbl(x) > CDbl(b)
                x = x - (b - a)
                k = k + 1
            End While
            x = System.Math.Max(x, a)
            x = System.Math.Min(x, b)
        End Sub


        '************************************************************************
        '        Returns random normal number using low-quality system-provided generator
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function randomnormal() As Double
            Dim result As Double = 0
            Dim u As Double = 0
            Dim v As Double = 0
            Dim s As Double = 0

            While True
                u = 2 * Math.randomreal() - 1
                v = 2 * Math.randomreal() - 1
                s = Math.sqr(u) + Math.sqr(v)
                If CDbl(s) > CDbl(0) AndAlso CDbl(s) < CDbl(1) Then

                    '
                    ' two Sqrt's instead of one to
                    ' avoid overflow when S is too small
                    '
                    s = System.Math.sqrt(-(2 * System.Math.Log(s))) / System.Math.sqrt(s)
                    result = u * s
                    Return result
                End If
            End While
            Return result
        End Function


        '************************************************************************
        '        Generates random unit vector using low-quality system-provided generator.
        '        Reallocates array if its size is too short.
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub randomunit(n As Integer, ByRef x As Double())
            Dim i As Integer = 0
            Dim v As Double = 0
            Dim vv As Double = 0

            alglib.ap.assert(n > 0, "RandomUnit: N<=0")
            If alglib.ap.len(x) < n Then
                x = New Double(n - 1) {}
            End If
            Do
                v = 0.0
                For i = 0 To n - 1
                    vv = randomnormal()
                    x(i) = vv
                    v = v + vv * vv
                Next
            Loop While CDbl(v) <= CDbl(0)
            v = 1 / System.Math.sqrt(v)
            For i = 0 To n - 1
                x(i) = x(i) * v
            Next
        End Sub


        '************************************************************************
        '        This function is used to swap two integer values
        '        ************************************************************************

        Public Shared Sub swapi(ByRef v0 As Integer, ByRef v1 As Integer)
            Dim v As Integer = 0

            v = v0
            v0 = v1
            v1 = v
        End Sub


        '************************************************************************
        '        This function is used to swap two real values
        '        ************************************************************************

        Public Shared Sub swapr(ByRef v0 As Double, ByRef v1 As Double)
            Dim v As Double = 0

            v = v0
            v0 = v1
            v1 = v
        End Sub


        '************************************************************************
        '        This function is used to increment value of integer variable
        '        ************************************************************************

        Public Shared Sub inc(ByRef v As Integer)
            v = v + 1
        End Sub


        '************************************************************************
        '        This function is used to decrement value of integer variable
        '        ************************************************************************

        Public Shared Sub dec(ByRef v As Integer)
            v = v - 1
        End Sub


        '************************************************************************
        '        This function performs two operations:
        '        1. decrements value of integer variable, if it is positive
        '        2. explicitly sets variable to zero if it is non-positive
        '        It is used by some algorithms to decrease value of internal counters.
        '        ************************************************************************

        Public Shared Sub countdown(ByRef v As Integer)
            If v > 0 Then
                v = v - 1
            Else
                v = 0
            End If
        End Sub


        '************************************************************************
        '        'bounds' value: maps X to [B1,B2]
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function boundval(x As Double, b1 As Double, b2 As Double) As Double
            Dim result As Double = 0

            If CDbl(x) <= CDbl(b1) Then
                result = b1
                Return result
            End If
            If CDbl(x) >= CDbl(b2) Then
                result = b2
                Return result
            End If
            result = x
            Return result
        End Function


        '************************************************************************
        '        Allocation of serializer: complex value
        '        ************************************************************************

        Public Shared Sub alloccomplex(s As alglib.serializer, v As complex)
            s.alloc_entry()
            s.alloc_entry()
        End Sub


        '************************************************************************
        '        Serialization: complex value
        '        ************************************************************************

        Public Shared Sub serializecomplex(s As alglib.serializer, v As complex)
            s.serialize_double(v.x)
            s.serialize_double(v.y)
        End Sub


        '************************************************************************
        '        Unserialization: complex value
        '        ************************************************************************

        Public Shared Function unserializecomplex(s As alglib.serializer) As complex
            Dim result As complex = 0

            result.x = s.unserialize_double()
            result.y = s.unserialize_double()
            Return result
        End Function


        '************************************************************************
        '        Allocation of serializer: real array
        '        ************************************************************************

        Public Shared Sub allocrealarray(s As alglib.serializer, v As Double(), n As Integer)
            Dim i As Integer = 0

            If n < 0 Then
                n = alglib.ap.len(v)
            End If
            s.alloc_entry()
            For i = 0 To n - 1
                s.alloc_entry()
            Next
        End Sub


        '************************************************************************
        '        Serialization: complex value
        '        ************************************************************************

        Public Shared Sub serializerealarray(s As alglib.serializer, v As Double(), n As Integer)
            Dim i As Integer = 0

            If n < 0 Then
                n = alglib.ap.len(v)
            End If
            s.serialize_int(n)
            For i = 0 To n - 1
                s.serialize_double(v(i))
            Next
        End Sub


        '************************************************************************
        '        Unserialization: complex value
        '        ************************************************************************

        Public Shared Sub unserializerealarray(s As alglib.serializer, ByRef v As Double())
            Dim n As Integer = 0
            Dim i As Integer = 0
            Dim t As Double = 0

            v = New Double(-1) {}

            n = s.unserialize_int()
            If n = 0 Then
                Return
            End If
            v = New Double(n - 1) {}
            For i = 0 To n - 1
                t = s.unserialize_double()
                v(i) = t
            Next
        End Sub


        '************************************************************************
        '        Allocation of serializer: Integer array
        '        ************************************************************************

        Public Shared Sub allocintegerarray(s As alglib.serializer, v As Integer(), n As Integer)
            Dim i As Integer = 0

            If n < 0 Then
                n = alglib.ap.len(v)
            End If
            s.alloc_entry()
            For i = 0 To n - 1
                s.alloc_entry()
            Next
        End Sub


        '************************************************************************
        '        Serialization: Integer array
        '        ************************************************************************

        Public Shared Sub serializeintegerarray(s As alglib.serializer, v As Integer(), n As Integer)
            Dim i As Integer = 0

            If n < 0 Then
                n = alglib.ap.len(v)
            End If
            s.serialize_int(n)
            For i = 0 To n - 1
                s.serialize_int(v(i))
            Next
        End Sub


        '************************************************************************
        '        Unserialization: complex value
        '        ************************************************************************

        Public Shared Sub unserializeintegerarray(s As alglib.serializer, ByRef v As Integer())
            Dim n As Integer = 0
            Dim i As Integer = 0
            Dim t As Integer = 0

            v = New Integer(-1) {}

            n = s.unserialize_int()
            If n = 0 Then
                Return
            End If
            v = New Integer(n - 1) {}
            For i = 0 To n - 1
                t = s.unserialize_int()
                v(i) = t
            Next
        End Sub


        '************************************************************************
        '        Allocation of serializer: real matrix
        '        ************************************************************************

        Public Shared Sub allocrealmatrix(s As alglib.serializer, v As Double(,), n0 As Integer, n1 As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0

            If n0 < 0 Then
                n0 = alglib.ap.rows(v)
            End If
            If n1 < 0 Then
                n1 = alglib.ap.cols(v)
            End If
            s.alloc_entry()
            s.alloc_entry()
            For i = 0 To n0 - 1
                For j = 0 To n1 - 1
                    s.alloc_entry()
                Next
            Next
        End Sub


        '************************************************************************
        '        Serialization: complex value
        '        ************************************************************************

        Public Shared Sub serializerealmatrix(s As alglib.serializer, v As Double(,), n0 As Integer, n1 As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0

            If n0 < 0 Then
                n0 = alglib.ap.rows(v)
            End If
            If n1 < 0 Then
                n1 = alglib.ap.cols(v)
            End If
            s.serialize_int(n0)
            s.serialize_int(n1)
            For i = 0 To n0 - 1
                For j = 0 To n1 - 1
                    s.serialize_double(v(i, j))
                Next
            Next
        End Sub


        '************************************************************************
        '        Unserialization: complex value
        '        ************************************************************************

        Public Shared Sub unserializerealmatrix(s As alglib.serializer, ByRef v As Double(,))
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim n0 As Integer = 0
            Dim n1 As Integer = 0
            Dim t As Double = 0

            v = New Double(-1, -1) {}

            n0 = s.unserialize_int()
            n1 = s.unserialize_int()
            If n0 = 0 OrElse n1 = 0 Then
                Return
            End If
            v = New Double(n0 - 1, n1 - 1) {}
            For i = 0 To n0 - 1
                For j = 0 To n1 - 1
                    t = s.unserialize_double()
                    v(i, j) = t
                Next
            Next
        End Sub


        '************************************************************************
        '        Copy integer array
        '        ************************************************************************

        Public Shared Sub copyintegerarray(src As Integer(), ByRef dst As Integer())
            Dim i As Integer = 0

            dst = New Integer(-1) {}

            If alglib.ap.len(src) > 0 Then
                dst = New Integer(alglib.ap.len(src) - 1) {}
                For i = 0 To alglib.ap.len(src) - 1
                    dst(i) = src(i)
                Next
            End If
        End Sub


        '************************************************************************
        '        Copy real array
        '        ************************************************************************

        Public Shared Sub copyrealarray(src As Double(), ByRef dst As Double())
            Dim i As Integer = 0

            dst = New Double(-1) {}

            If alglib.ap.len(src) > 0 Then
                dst = New Double(alglib.ap.len(src) - 1) {}
                For i = 0 To alglib.ap.len(src) - 1
                    dst(i) = src(i)
                Next
            End If
        End Sub


        '************************************************************************
        '        Copy real matrix
        '        ************************************************************************

        Public Shared Sub copyrealmatrix(src As Double(,), ByRef dst As Double(,))
            Dim i As Integer = 0
            Dim j As Integer = 0

            dst = New Double(-1, -1) {}

            If alglib.ap.rows(src) > 0 AndAlso alglib.ap.cols(src) > 0 Then
                dst = New Double(alglib.ap.rows(src) - 1, alglib.ap.cols(src) - 1) {}
                For i = 0 To alglib.ap.rows(src) - 1
                    For j = 0 To alglib.ap.cols(src) - 1
                        dst(i, j) = src(i, j)
                    Next
                Next
            End If
        End Sub


        '************************************************************************
        '        This function searches integer array. Elements in this array are actually
        '        records, each NRec elements wide. Each record has unique header - NHeader
        '        integer values, which identify it. Records are lexicographically sorted by
        '        header.
        '
        '        Records are identified by their index, not offset (offset = NRec*index).
        '
        '        This function searches A (records with indices [I0,I1)) for a record with
        '        header B. It returns index of this record (not offset!), or -1 on failure.
        '
        '          -- ALGLIB --
        '             Copyright 28.03.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function recsearch(ByRef a As Integer(), nrec As Integer, nheader As Integer, i0 As Integer, i1 As Integer, b As Integer()) As Integer
            Dim result As Integer = 0
            Dim mididx As Integer = 0
            Dim cflag As Integer = 0
            Dim k As Integer = 0
            Dim offs As Integer = 0

            result = -1
            While True
                If i0 >= i1 Then
                    Exit While
                End If
                mididx = (i0 + i1) \ 2
                offs = nrec * mididx
                cflag = 0
                For k = 0 To nheader - 1
                    If a(offs + k) < b(k) Then
                        cflag = -1
                        Exit For
                    End If
                    If a(offs + k) > b(k) Then
                        cflag = 1
                        Exit For
                    End If
                Next
                If cflag = 0 Then
                    result = mididx
                    Return result
                End If
                If cflag < 0 Then
                    i0 = mididx + 1
                Else
                    i1 = mididx
                End If
            End While
            Return result
        End Function


        '************************************************************************
        '        This function is used in parallel functions for recurrent division of large
        '        task into two smaller tasks.
        '
        '        It has following properties:
        '        * it works only for TaskSize>=2 (assertion is thrown otherwise)
        '        * for TaskSize=2, it returns Task0=1, Task1=1
        '        * in case TaskSize is odd,  Task0=TaskSize-1, Task1=1
        '        * in case TaskSize is even, Task0 and Task1 are approximately TaskSize/2
        '          and both Task0 and Task1 are even, Task0>=Task1
        '
        '          -- ALGLIB --
        '             Copyright 07.04.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub splitlengtheven(tasksize As Integer, ByRef task0 As Integer, ByRef task1 As Integer)
            task0 = 0
            task1 = 0

            alglib.ap.assert(tasksize >= 2, "SplitLengthEven: TaskSize<2")
            If tasksize = 2 Then
                task0 = 1
                task1 = 1
                Return
            End If
            If tasksize Mod 2 = 0 Then

                '
                ' Even division
                '
                task0 = tasksize \ 2
                task1 = tasksize \ 2
                If task0 Mod 2 <> 0 Then
                    task0 = task0 + 1
                    task1 = task1 - 1
                End If
            Else

                '
                ' Odd task size, split trailing odd part from it.
                '
                task0 = tasksize - 1
                task1 = 1
            End If
            alglib.ap.assert(task0 >= 1, "SplitLengthEven: internal error")
            alglib.ap.assert(task1 >= 1, "SplitLengthEven: internal error")
        End Sub


        '************************************************************************
        '        This function is used in parallel functions for recurrent division of large
        '        task into two smaller tasks.
        '
        '        It has following properties:
        '        * it works only for TaskSize>=2 and ChunkSize>=2
        '          (assertion is thrown otherwise)
        '        * Task0+Task1=TaskSize, Task0>0, Task1>0
        '        * Task0 and Task1 are close to each other
        '        * in case TaskSize>ChunkSize, Task0 is always divisible by ChunkSize
        '
        '          -- ALGLIB --
        '             Copyright 07.04.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub splitlength(tasksize As Integer, chunksize As Integer, ByRef task0 As Integer, ByRef task1 As Integer)
            task0 = 0
            task1 = 0

            alglib.ap.assert(chunksize >= 2, "SplitLength: ChunkSize<2")
            alglib.ap.assert(tasksize >= 2, "SplitLength: TaskSize<2")
            task0 = tasksize \ 2
            If task0 > chunksize AndAlso task0 Mod chunksize <> 0 Then
                task0 = task0 - task0 Mod chunksize
            End If
            task1 = tasksize - task0
            alglib.ap.assert(task0 >= 1, "SplitLength: internal error")
            alglib.ap.assert(task1 >= 1, "SplitLength: internal error")
        End Sub


    End Class
    Public Class scodes
        Public Shared Function getrdfserializationcode() As Integer
            Dim result As Integer = 0

            result = 1
            Return result
        End Function


        Public Shared Function getkdtreeserializationcode() As Integer
            Dim result As Integer = 0

            result = 2
            Return result
        End Function


        Public Shared Function getmlpserializationcode() As Integer
            Dim result As Integer = 0

            result = 3
            Return result
        End Function


        Public Shared Function getmlpeserializationcode() As Integer
            Dim result As Integer = 0

            result = 4
            Return result
        End Function


        Public Shared Function getrbfserializationcode() As Integer
            Dim result As Integer = 0

            result = 5
            Return result
        End Function


    End Class
    Public Class tsort
        '************************************************************************
        '        This function sorts array of real keys by ascending.
        '
        '        Its results are:
        '        * sorted array A
        '        * permutation tables P1, P2
        '
        '        Algorithm outputs permutation tables using two formats:
        '        * as usual permutation of [0..N-1]. If P1[i]=j, then sorted A[i]  contains
        '          value which was moved there from J-th position.
        '        * as a sequence of pairwise permutations. Sorted A[] may  be  obtained  by
        '          swaping A[i] and A[P2[i]] for all i from 0 to N-1.
        '          
        '        INPUT PARAMETERS:
        '            A       -   unsorted array
        '            N       -   array size
        '
        '        OUPUT PARAMETERS:
        '            A       -   sorted array
        '            P1, P2  -   permutation tables, array[N]
        '            
        '        NOTES:
        '            this function assumes that A[] is finite; it doesn't checks that
        '            condition. All other conditions (size of input arrays, etc.) are not
        '            checked too.
        '
        '          -- ALGLIB --
        '             Copyright 14.05.2008 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub tagsort(ByRef a As Double(), n As Integer, ByRef p1 As Integer(), ByRef p2 As Integer())
            Dim buf As New apserv.apbuffers()

            p1 = New Integer(-1) {}
            p2 = New Integer(-1) {}

            tagsortbuf(a, n, p1, p2, buf)
        End Sub


        '************************************************************************
        '        Buffered variant of TagSort, which accepts preallocated output arrays as
        '        well as special structure for buffered allocations. If arrays are too
        '        short, they are reallocated. If they are large enough, no memory
        '        allocation is done.
        '
        '        It is intended to be used in the performance-critical parts of code, where
        '        additional allocations can lead to severe performance degradation
        '
        '          -- ALGLIB --
        '             Copyright 14.05.2008 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub tagsortbuf(ByRef a As Double(), n As Integer, ByRef p1 As Integer(), ByRef p2 As Integer(), buf As apserv.apbuffers)
            Dim i As Integer = 0
            Dim lv As Integer = 0
            Dim lp As Integer = 0
            Dim rv As Integer = 0
            Dim rp As Integer = 0


            '
            ' Special cases
            '
            If n <= 0 Then
                Return
            End If
            If n = 1 Then
                apserv.ivectorsetlengthatleast(p1, 1)
                apserv.ivectorsetlengthatleast(p2, 1)
                p1(0) = 0
                p2(0) = 0
                Return
            End If

            '
            ' General case, N>1: prepare permutations table P1
            '
            apserv.ivectorsetlengthatleast(p1, n)
            For i = 0 To n - 1
                p1(i) = i
            Next

            '
            ' General case, N>1: sort, update P1
            '
            apserv.rvectorsetlengthatleast(buf.ra0, n)
            apserv.ivectorsetlengthatleast(buf.ia0, n)
            tagsortfasti(a, p1, buf.ra0, buf.ia0, n)

            '
            ' General case, N>1: fill permutations table P2
            '
            ' To fill P2 we maintain two arrays:
            ' * PV (Buf.IA0), Position(Value). PV[i] contains position of I-th key at the moment
            ' * VP (Buf.IA1), Value(Position). VP[i] contains key which has position I at the moment
            '
            ' At each step we making permutation of two items:
            '   Left, which is given by position/value pair LP/LV
            '   and Right, which is given by RP/RV
            ' and updating PV[] and VP[] correspondingly.
            '
            apserv.ivectorsetlengthatleast(buf.ia0, n)
            apserv.ivectorsetlengthatleast(buf.ia1, n)
            apserv.ivectorsetlengthatleast(p2, n)
            For i = 0 To n - 1
                buf.ia0(i) = i
                buf.ia1(i) = i
            Next
            For i = 0 To n - 1

                '
                ' calculate LP, LV, RP, RV
                '
                lp = i
                lv = buf.ia1(lp)
                rv = p1(i)
                rp = buf.ia0(rv)

                '
                ' Fill P2
                '
                p2(i) = rp

                '
                ' update PV and VP
                '
                buf.ia1(lp) = rv
                buf.ia1(rp) = lv
                buf.ia0(lv) = rp
                buf.ia0(rv) = lp
            Next
        End Sub


        '************************************************************************
        '        Same as TagSort, but optimized for real keys and integer labels.
        '
        '        A is sorted, and same permutations are applied to B.
        '
        '        NOTES:
        '        1.  this function assumes that A[] is finite; it doesn't checks that
        '            condition. All other conditions (size of input arrays, etc.) are not
        '            checked too.
        '        2.  this function uses two buffers, BufA and BufB, each is N elements large.
        '            They may be preallocated (which will save some time) or not, in which
        '            case function will automatically allocate memory.
        '
        '          -- ALGLIB --
        '             Copyright 11.12.2008 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub tagsortfasti(ByRef a As Double(), ByRef b As Integer(), ByRef bufa As Double(), ByRef bufb As Integer(), n As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim isascending As New Boolean()
            Dim isdescending As New Boolean()
            Dim tmpr As Double = 0
            Dim tmpi As Integer = 0


            '
            ' Special case
            '
            If n <= 1 Then
                Return
            End If

            '
            ' Test for already sorted set
            '
            isascending = True
            isdescending = True
            For i = 1 To n - 1
                isascending = isascending AndAlso a(i) >= a(i - 1)
                isdescending = isdescending AndAlso a(i) <= a(i - 1)
            Next
            If isascending Then
                Return
            End If
            If isdescending Then
                For i = 0 To n - 1
                    j = n - 1 - i
                    If j <= i Then
                        Exit For
                    End If
                    tmpr = a(i)
                    a(i) = a(j)
                    a(j) = tmpr
                    tmpi = b(i)
                    b(i) = b(j)
                    b(j) = tmpi
                Next
                Return
            End If

            '
            ' General case
            '
            If alglib.ap.len(bufa) < n Then
                bufa = New Double(n - 1) {}
            End If
            If alglib.ap.len(bufb) < n Then
                bufb = New Integer(n - 1) {}
            End If
            tagsortfastirec(a, b, bufa, bufb, 0, n - 1)
        End Sub


        '************************************************************************
        '        Same as TagSort, but optimized for real keys and real labels.
        '
        '        A is sorted, and same permutations are applied to B.
        '
        '        NOTES:
        '        1.  this function assumes that A[] is finite; it doesn't checks that
        '            condition. All other conditions (size of input arrays, etc.) are not
        '            checked too.
        '        2.  this function uses two buffers, BufA and BufB, each is N elements large.
        '            They may be preallocated (which will save some time) or not, in which
        '            case function will automatically allocate memory.
        '
        '          -- ALGLIB --
        '             Copyright 11.12.2008 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub tagsortfastr(ByRef a As Double(), ByRef b As Double(), ByRef bufa As Double(), ByRef bufb As Double(), n As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim isascending As New Boolean()
            Dim isdescending As New Boolean()
            Dim tmpr As Double = 0


            '
            ' Special case
            '
            If n <= 1 Then
                Return
            End If

            '
            ' Test for already sorted set
            '
            isascending = True
            isdescending = True
            For i = 1 To n - 1
                isascending = isascending AndAlso a(i) >= a(i - 1)
                isdescending = isdescending AndAlso a(i) <= a(i - 1)
            Next
            If isascending Then
                Return
            End If
            If isdescending Then
                For i = 0 To n - 1
                    j = n - 1 - i
                    If j <= i Then
                        Exit For
                    End If
                    tmpr = a(i)
                    a(i) = a(j)
                    a(j) = tmpr
                    tmpr = b(i)
                    b(i) = b(j)
                    b(j) = tmpr
                Next
                Return
            End If

            '
            ' General case
            '
            If alglib.ap.len(bufa) < n Then
                bufa = New Double(n - 1) {}
            End If
            If alglib.ap.len(bufb) < n Then
                bufb = New Double(n - 1) {}
            End If
            tagsortfastrrec(a, b, bufa, bufb, 0, n - 1)
        End Sub


        '************************************************************************
        '        Same as TagSort, but optimized for real keys without labels.
        '
        '        A is sorted, and that's all.
        '
        '        NOTES:
        '        1.  this function assumes that A[] is finite; it doesn't checks that
        '            condition. All other conditions (size of input arrays, etc.) are not
        '            checked too.
        '        2.  this function uses buffer, BufA, which is N elements large. It may be
        '            preallocated (which will save some time) or not, in which case
        '            function will automatically allocate memory.
        '
        '          -- ALGLIB --
        '             Copyright 11.12.2008 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub tagsortfast(ByRef a As Double(), ByRef bufa As Double(), n As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim isascending As New Boolean()
            Dim isdescending As New Boolean()
            Dim tmpr As Double = 0


            '
            ' Special case
            '
            If n <= 1 Then
                Return
            End If

            '
            ' Test for already sorted set
            '
            isascending = True
            isdescending = True
            For i = 1 To n - 1
                isascending = isascending AndAlso a(i) >= a(i - 1)
                isdescending = isdescending AndAlso a(i) <= a(i - 1)
            Next
            If isascending Then
                Return
            End If
            If isdescending Then
                For i = 0 To n - 1
                    j = n - 1 - i
                    If j <= i Then
                        Exit For
                    End If
                    tmpr = a(i)
                    a(i) = a(j)
                    a(j) = tmpr
                Next
                Return
            End If

            '
            ' General case
            '
            If alglib.ap.len(bufa) < n Then
                bufa = New Double(n - 1) {}
            End If
            tagsortfastrec(a, bufa, 0, n - 1)
        End Sub


        '************************************************************************
        '        Sorting function optimized for integer keys and real labels, can be used
        '        to sort middle of the array
        '
        '        A is sorted, and same permutations are applied to B.
        '
        '        NOTES:
        '            this function assumes that A[] is finite; it doesn't checks that
        '            condition. All other conditions (size of input arrays, etc.) are not
        '            checked too.
        '
        '          -- ALGLIB --
        '             Copyright 11.12.2008 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub tagsortmiddleir(ByRef a As Integer(), ByRef b As Double(), offset As Integer, n As Integer)
            Dim i As Integer = 0
            Dim k As Integer = 0
            Dim t As Integer = 0
            Dim tmp As Integer = 0
            Dim tmpr As Double = 0


            '
            ' Special cases
            '
            If n <= 1 Then
                Return
            End If

            '
            ' General case, N>1: sort, update B
            '
            i = 2
            Do
                t = i
                While t <> 1
                    k = t \ 2
                    If a(offset + k - 1) >= a(offset + t - 1) Then
                        t = 1
                    Else
                        tmp = a(offset + k - 1)
                        a(offset + k - 1) = a(offset + t - 1)
                        a(offset + t - 1) = tmp
                        tmpr = b(offset + k - 1)
                        b(offset + k - 1) = b(offset + t - 1)
                        b(offset + t - 1) = tmpr
                        t = k
                    End If
                End While
                i = i + 1
            Loop While i <= n
            i = n - 1
            Do
                tmp = a(offset + i)
                a(offset + i) = a(offset + 0)
                a(offset + 0) = tmp
                tmpr = b(offset + i)
                b(offset + i) = b(offset + 0)
                b(offset + 0) = tmpr
                t = 1
                While t <> 0
                    k = 2 * t
                    If k > i Then
                        t = 0
                    Else
                        If k < i Then
                            If a(offset + k) > a(offset + k - 1) Then
                                k = k + 1
                            End If
                        End If
                        If a(offset + t - 1) >= a(offset + k - 1) Then
                            t = 0
                        Else
                            tmp = a(offset + k - 1)
                            a(offset + k - 1) = a(offset + t - 1)
                            a(offset + t - 1) = tmp
                            tmpr = b(offset + k - 1)
                            b(offset + k - 1) = b(offset + t - 1)
                            b(offset + t - 1) = tmpr
                            t = k
                        End If
                    End If
                End While
                i = i - 1
            Loop While i >= 1
        End Sub


        '************************************************************************
        '        Heap operations: adds element to the heap
        '
        '        PARAMETERS:
        '            A       -   heap itself, must be at least array[0..N]
        '            B       -   array of integer tags, which are updated according to
        '                        permutations in the heap
        '            N       -   size of the heap (without new element).
        '                        updated on output
        '            VA      -   value of the element being added
        '            VB      -   value of the tag
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub tagheappushi(ByRef a As Double(), ByRef b As Integer(), ByRef n As Integer, va As Double, vb As Integer)
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim v As Double = 0

            If n < 0 Then
                Return
            End If

            '
            ' N=0 is a special case
            '
            If n = 0 Then
                a(0) = va
                b(0) = vb
                n = n + 1
                Return
            End If

            '
            ' add current point to the heap
            ' (add to the bottom, then move up)
            '
            ' we don't write point to the heap
            ' until its final position is determined
            ' (it allow us to reduce number of array access operations)
            '
            j = n
            n = n + 1
            While j > 0
                k = (j - 1) \ 2
                v = a(k)
                If CDbl(v) < CDbl(va) Then

                    '
                    ' swap with higher element
                    '
                    a(j) = v
                    b(j) = b(k)
                    j = k
                Else

                    '
                    ' element in its place. terminate.
                    '
                    Exit While
                End If
            End While
            a(j) = va
            b(j) = vb
        End Sub


        '************************************************************************
        '        Heap operations: replaces top element with new element
        '        (which is moved down)
        '
        '        PARAMETERS:
        '            A       -   heap itself, must be at least array[0..N-1]
        '            B       -   array of integer tags, which are updated according to
        '                        permutations in the heap
        '            N       -   size of the heap
        '            VA      -   value of the element which replaces top element
        '            VB      -   value of the tag
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub tagheapreplacetopi(ByRef a As Double(), ByRef b As Integer(), n As Integer, va As Double, vb As Integer)
            Dim j As Integer = 0
            Dim k1 As Integer = 0
            Dim k2 As Integer = 0
            Dim v As Double = 0
            Dim v1 As Double = 0
            Dim v2 As Double = 0

            If n < 1 Then
                Return
            End If

            '
            ' N=1 is a special case
            '
            If n = 1 Then
                a(0) = va
                b(0) = vb
                Return
            End If

            '
            ' move down through heap:
            ' * J  -   current element
            ' * K1 -   first child (always exists)
            ' * K2 -   second child (may not exists)
            '
            ' we don't write point to the heap
            ' until its final position is determined
            ' (it allow us to reduce number of array access operations)
            '
            j = 0
            k1 = 1
            k2 = 2
            While k1 < n
                If k2 >= n Then

                    '
                    ' only one child.
                    '
                    ' swap and terminate (because this child
                    ' have no siblings due to heap structure)
                    '
                    v = a(k1)
                    If CDbl(v) > CDbl(va) Then
                        a(j) = v
                        b(j) = b(k1)
                        j = k1
                    End If
                    Exit While
                Else

                    '
                    ' two childs
                    '
                    v1 = a(k1)
                    v2 = a(k2)
                    If CDbl(v1) > CDbl(v2) Then
                        If CDbl(va) < CDbl(v1) Then
                            a(j) = v1
                            b(j) = b(k1)
                            j = k1
                        Else
                            Exit While
                        End If
                    Else
                        If CDbl(va) < CDbl(v2) Then
                            a(j) = v2
                            b(j) = b(k2)
                            j = k2
                        Else
                            Exit While
                        End If
                    End If
                    k1 = 2 * j + 1
                    k2 = 2 * j + 2
                End If
            End While
            a(j) = va
            b(j) = vb
        End Sub


        '************************************************************************
        '        Heap operations: pops top element from the heap
        '
        '        PARAMETERS:
        '            A       -   heap itself, must be at least array[0..N-1]
        '            B       -   array of integer tags, which are updated according to
        '                        permutations in the heap
        '            N       -   size of the heap, N>=1
        '
        '        On output top element is moved to A[N-1], B[N-1], heap is reordered, N is
        '        decreased by 1.
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub tagheappopi(ByRef a As Double(), ByRef b As Integer(), ByRef n As Integer)
            Dim va As Double = 0
            Dim vb As Integer = 0

            If n < 1 Then
                Return
            End If

            '
            ' N=1 is a special case
            '
            If n = 1 Then
                n = 0
                Return
            End If

            '
            ' swap top element and last element,
            ' then reorder heap
            '
            va = a(n - 1)
            vb = b(n - 1)
            a(n - 1) = a(0)
            b(n - 1) = b(0)
            n = n - 1
            tagheapreplacetopi(a, b, n, va, vb)
        End Sub


        '************************************************************************
        '        Search first element less than T in sorted array.
        '
        '        PARAMETERS:
        '            A - sorted array by ascending from 0 to N-1
        '            N - number of elements in array
        '            T - the desired element
        '
        '        RESULT:
        '            The very first element's index, which isn't less than T.
        '        In the case when there aren't such elements, returns N.
        '        ************************************************************************

        Public Shared Function lowerbound(a As Double(), n As Integer, t As Double) As Integer
            Dim result As Integer = 0
            Dim l As Integer = 0
            Dim half As Integer = 0
            Dim first As Integer = 0
            Dim middle As Integer = 0

            l = n
            first = 0
            While l > 0
                half = l \ 2
                middle = first + half
                If CDbl(a(middle)) < CDbl(t) Then
                    first = middle + 1
                    l = l - half - 1
                Else
                    l = half
                End If
            End While
            result = first
            Return result
        End Function


        '************************************************************************
        '        Search first element more than T in sorted array.
        '
        '        PARAMETERS:
        '            A - sorted array by ascending from 0 to N-1
        '            N - number of elements in array
        '            T - the desired element
        '
        '            RESULT:
        '            The very first element's index, which more than T.
        '        In the case when there aren't such elements, returns N.
        '        ************************************************************************

        Public Shared Function upperbound(a As Double(), n As Integer, t As Double) As Integer
            Dim result As Integer = 0
            Dim l As Integer = 0
            Dim half As Integer = 0
            Dim first As Integer = 0
            Dim middle As Integer = 0

            l = n
            first = 0
            While l > 0
                half = l \ 2
                middle = first + half
                If CDbl(t) < CDbl(a(middle)) Then
                    l = half
                Else
                    first = middle + 1
                    l = l - half - 1
                End If
            End While
            result = first
            Return result
        End Function


        '************************************************************************
        '        Internal TagSortFastI: sorts A[I1...I2] (both bounds are included),
        '        applies same permutations to B.
        '
        '          -- ALGLIB --
        '             Copyright 06.09.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub tagsortfastirec(ByRef a As Double(), ByRef b As Integer(), ByRef bufa As Double(), ByRef bufb As Integer(), i1 As Integer, i2 As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim cntless As Integer = 0
            Dim cnteq As Integer = 0
            Dim cntgreater As Integer = 0
            Dim tmpr As Double = 0
            Dim tmpi As Integer = 0
            Dim v0 As Double = 0
            Dim v1 As Double = 0
            Dim v2 As Double = 0
            Dim vp As Double = 0


            '
            ' Fast exit
            '
            If i2 <= i1 Then
                Return
            End If

            '
            ' Non-recursive sort for small arrays
            '
            If i2 - i1 <= 16 Then
                For j = i1 + 1 To i2

                    '
                    ' Search elements [I1..J-1] for place to insert Jth element.
                    '
                    ' This code stops immediately if we can leave A[J] at J-th position
                    ' (all elements have same value of A[J] larger than any of them)
                    '
                    tmpr = a(j)
                    tmpi = j
                    For k = j - 1 To i1 Step -1
                        If a(k) <= tmpr Then
                            Exit For
                        End If
                        tmpi = k
                    Next
                    k = tmpi

                    '
                    ' Insert Jth element into Kth position
                    '
                    If k <> j Then
                        tmpr = a(j)
                        tmpi = b(j)
                        For i = j - 1 To k Step -1
                            a(i + 1) = a(i)
                            b(i + 1) = b(i)
                        Next
                        a(k) = tmpr
                        b(k) = tmpi
                    End If
                Next
                Return
            End If

            '
            ' Quicksort: choose pivot
            ' Here we assume that I2-I1>=2
            '
            v0 = a(i1)
            v1 = a(i1 + (i2 - i1) \ 2)
            v2 = a(i2)
            If v0 > v1 Then
                tmpr = v1
                v1 = v0
                v0 = tmpr
            End If
            If v1 > v2 Then
                tmpr = v2
                v2 = v1
                v1 = tmpr
            End If
            If v0 > v1 Then
                tmpr = v1
                v1 = v0
                v0 = tmpr
            End If
            vp = v1

            '
            ' now pass through A/B and:
            ' * move elements that are LESS than VP to the left of A/B
            ' * move elements that are EQUAL to VP to the right of BufA/BufB (in the reverse order)
            ' * move elements that are GREATER than VP to the left of BufA/BufB (in the normal order
            ' * move elements from the tail of BufA/BufB to the middle of A/B (restoring normal order)
            ' * move elements from the left of BufA/BufB to the end of A/B
            '
            cntless = 0
            cnteq = 0
            cntgreater = 0
            For i = i1 To i2
                v0 = a(i)
                If v0 < vp Then

                    '
                    ' LESS
                    '
                    k = i1 + cntless
                    If i <> k Then
                        a(k) = v0
                        b(k) = b(i)
                    End If
                    cntless = cntless + 1
                    Continue For
                End If
                If v0 = vp Then

                    '
                    ' EQUAL
                    '
                    k = i2 - cnteq
                    bufa(k) = v0
                    bufb(k) = b(i)
                    cnteq = cnteq + 1
                    Continue For
                End If

                '
                ' GREATER
                '
                k = i1 + cntgreater
                bufa(k) = v0
                bufb(k) = b(i)
                cntgreater = cntgreater + 1
            Next
            For i = 0 To cnteq - 1
                j = i1 + cntless + cnteq - 1 - i
                k = i2 + i - (cnteq - 1)
                a(j) = bufa(k)
                b(j) = bufb(k)
            Next
            For i = 0 To cntgreater - 1
                j = i1 + cntless + cnteq + i
                k = i1 + i
                a(j) = bufa(k)
                b(j) = bufb(k)
            Next

            '
            ' Sort left and right parts of the array (ignoring middle part)
            '
            tagsortfastirec(a, b, bufa, bufb, i1, i1 + cntless - 1)
            tagsortfastirec(a, b, bufa, bufb, i1 + cntless + cnteq, i2)
        End Sub


        '************************************************************************
        '        Internal TagSortFastR: sorts A[I1...I2] (both bounds are included),
        '        applies same permutations to B.
        '
        '          -- ALGLIB --
        '             Copyright 06.09.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub tagsortfastrrec(ByRef a As Double(), ByRef b As Double(), ByRef bufa As Double(), ByRef bufb As Double(), i1 As Integer, i2 As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim tmpr As Double = 0
            Dim tmpr2 As Double = 0
            Dim tmpi As Integer = 0
            Dim cntless As Integer = 0
            Dim cnteq As Integer = 0
            Dim cntgreater As Integer = 0
            Dim v0 As Double = 0
            Dim v1 As Double = 0
            Dim v2 As Double = 0
            Dim vp As Double = 0


            '
            ' Fast exit
            '
            If i2 <= i1 Then
                Return
            End If

            '
            ' Non-recursive sort for small arrays
            '
            If i2 - i1 <= 16 Then
                For j = i1 + 1 To i2

                    '
                    ' Search elements [I1..J-1] for place to insert Jth element.
                    '
                    ' This code stops immediatly if we can leave A[J] at J-th position
                    ' (all elements have same value of A[J] larger than any of them)
                    '
                    tmpr = a(j)
                    tmpi = j
                    For k = j - 1 To i1 Step -1
                        If a(k) <= tmpr Then
                            Exit For
                        End If
                        tmpi = k
                    Next
                    k = tmpi

                    '
                    ' Insert Jth element into Kth position
                    '
                    If k <> j Then
                        tmpr = a(j)
                        tmpr2 = b(j)
                        For i = j - 1 To k Step -1
                            a(i + 1) = a(i)
                            b(i + 1) = b(i)
                        Next
                        a(k) = tmpr
                        b(k) = tmpr2
                    End If
                Next
                Return
            End If

            '
            ' Quicksort: choose pivot
            ' Here we assume that I2-I1>=16
            '
            v0 = a(i1)
            v1 = a(i1 + (i2 - i1) \ 2)
            v2 = a(i2)
            If v0 > v1 Then
                tmpr = v1
                v1 = v0
                v0 = tmpr
            End If
            If v1 > v2 Then
                tmpr = v2
                v2 = v1
                v1 = tmpr
            End If
            If v0 > v1 Then
                tmpr = v1
                v1 = v0
                v0 = tmpr
            End If
            vp = v1

            '
            ' now pass through A/B and:
            ' * move elements that are LESS than VP to the left of A/B
            ' * move elements that are EQUAL to VP to the right of BufA/BufB (in the reverse order)
            ' * move elements that are GREATER than VP to the left of BufA/BufB (in the normal order
            ' * move elements from the tail of BufA/BufB to the middle of A/B (restoring normal order)
            ' * move elements from the left of BufA/BufB to the end of A/B
            '
            cntless = 0
            cnteq = 0
            cntgreater = 0
            For i = i1 To i2
                v0 = a(i)
                If v0 < vp Then

                    '
                    ' LESS
                    '
                    k = i1 + cntless
                    If i <> k Then
                        a(k) = v0
                        b(k) = b(i)
                    End If
                    cntless = cntless + 1
                    Continue For
                End If
                If v0 = vp Then

                    '
                    ' EQUAL
                    '
                    k = i2 - cnteq
                    bufa(k) = v0
                    bufb(k) = b(i)
                    cnteq = cnteq + 1
                    Continue For
                End If

                '
                ' GREATER
                '
                k = i1 + cntgreater
                bufa(k) = v0
                bufb(k) = b(i)
                cntgreater = cntgreater + 1
            Next
            For i = 0 To cnteq - 1
                j = i1 + cntless + cnteq - 1 - i
                k = i2 + i - (cnteq - 1)
                a(j) = bufa(k)
                b(j) = bufb(k)
            Next
            For i = 0 To cntgreater - 1
                j = i1 + cntless + cnteq + i
                k = i1 + i
                a(j) = bufa(k)
                b(j) = bufb(k)
            Next

            '
            ' Sort left and right parts of the array (ignoring middle part)
            '
            tagsortfastrrec(a, b, bufa, bufb, i1, i1 + cntless - 1)
            tagsortfastrrec(a, b, bufa, bufb, i1 + cntless + cnteq, i2)
        End Sub


        '************************************************************************
        '        Internal TagSortFastI: sorts A[I1...I2] (both bounds are included),
        '        applies same permutations to B.
        '
        '          -- ALGLIB --
        '             Copyright 06.09.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub tagsortfastrec(ByRef a As Double(), ByRef bufa As Double(), i1 As Integer, i2 As Integer)
            Dim cntless As Integer = 0
            Dim cnteq As Integer = 0
            Dim cntgreater As Integer = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim tmpr As Double = 0
            Dim tmpi As Integer = 0
            Dim v0 As Double = 0
            Dim v1 As Double = 0
            Dim v2 As Double = 0
            Dim vp As Double = 0


            '
            ' Fast exit
            '
            If i2 <= i1 Then
                Return
            End If

            '
            ' Non-recursive sort for small arrays
            '
            If i2 - i1 <= 16 Then
                For j = i1 + 1 To i2

                    '
                    ' Search elements [I1..J-1] for place to insert Jth element.
                    '
                    ' This code stops immediatly if we can leave A[J] at J-th position
                    ' (all elements have same value of A[J] larger than any of them)
                    '
                    tmpr = a(j)
                    tmpi = j
                    For k = j - 1 To i1 Step -1
                        If a(k) <= tmpr Then
                            Exit For
                        End If
                        tmpi = k
                    Next
                    k = tmpi

                    '
                    ' Insert Jth element into Kth position
                    '
                    If k <> j Then
                        tmpr = a(j)
                        For i = j - 1 To k Step -1
                            a(i + 1) = a(i)
                        Next
                        a(k) = tmpr
                    End If
                Next
                Return
            End If

            '
            ' Quicksort: choose pivot
            ' Here we assume that I2-I1>=16
            '
            v0 = a(i1)
            v1 = a(i1 + (i2 - i1) \ 2)
            v2 = a(i2)
            If v0 > v1 Then
                tmpr = v1
                v1 = v0
                v0 = tmpr
            End If
            If v1 > v2 Then
                tmpr = v2
                v2 = v1
                v1 = tmpr
            End If
            If v0 > v1 Then
                tmpr = v1
                v1 = v0
                v0 = tmpr
            End If
            vp = v1

            '
            ' now pass through A/B and:
            ' * move elements that are LESS than VP to the left of A/B
            ' * move elements that are EQUAL to VP to the right of BufA/BufB (in the reverse order)
            ' * move elements that are GREATER than VP to the left of BufA/BufB (in the normal order
            ' * move elements from the tail of BufA/BufB to the middle of A/B (restoring normal order)
            ' * move elements from the left of BufA/BufB to the end of A/B
            '
            cntless = 0
            cnteq = 0
            cntgreater = 0
            For i = i1 To i2
                v0 = a(i)
                If v0 < vp Then

                    '
                    ' LESS
                    '
                    k = i1 + cntless
                    If i <> k Then
                        a(k) = v0
                    End If
                    cntless = cntless + 1
                    Continue For
                End If
                If v0 = vp Then

                    '
                    ' EQUAL
                    '
                    k = i2 - cnteq
                    bufa(k) = v0
                    cnteq = cnteq + 1
                    Continue For
                End If

                '
                ' GREATER
                '
                k = i1 + cntgreater
                bufa(k) = v0
                cntgreater = cntgreater + 1
            Next
            For i = 0 To cnteq - 1
                j = i1 + cntless + cnteq - 1 - i
                k = i2 + i - (cnteq - 1)
                a(j) = bufa(k)
            Next
            For i = 0 To cntgreater - 1
                j = i1 + cntless + cnteq + i
                k = i1 + i
                a(j) = bufa(k)
            Next

            '
            ' Sort left and right parts of the array (ignoring middle part)
            '
            tagsortfastrec(a, bufa, i1, i1 + cntless - 1)
            tagsortfastrec(a, bufa, i1 + cntless + cnteq, i2)
        End Sub


    End Class
    Public Class basicstatops
        '************************************************************************
        '        Internal ranking subroutine.
        '
        '        INPUT PARAMETERS:
        '            X       -   array to rank
        '            N       -   array size
        '            IsCentered- whether ranks are centered or not:
        '                        * True      -   ranks are centered in such way that  their
        '                                        sum is zero
        '                        * False     -   ranks are not centered
        '            Buf     -   temporary buffers
        '            
        '        NOTE: when IsCentered is True and all X[] are equal, this  function  fills
        '              X by zeros (exact zeros are used, not sum which is only approximately
        '              equal to zero).
        '        ************************************************************************

        Public Shared Sub rankx(x As Double(), n As Integer, iscentered As Boolean, buf As apserv.apbuffers)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim tmp As Double = 0
            Dim voffs As Double = 0


            '
            ' Prepare
            '
            If n < 1 Then
                Return
            End If
            If n = 1 Then
                x(0) = 0
                Return
            End If
            If alglib.ap.len(buf.ra1) < n Then
                buf.ra1 = New Double(n - 1) {}
            End If
            If alglib.ap.len(buf.ia1) < n Then
                buf.ia1 = New Integer(n - 1) {}
            End If
            For i = 0 To n - 1
                buf.ra1(i) = x(i)
                buf.ia1(i) = i
            Next
            tsort.tagsortfasti(buf.ra1, buf.ia1, buf.ra2, buf.ia2, n)

            '
            ' Special test for all values being equal
            '
            If CDbl(buf.ra1(0)) = CDbl(buf.ra1(n - 1)) Then
                If iscentered Then
                    tmp = 0.0
                Else
                    tmp = CDbl(n - 1) / CDbl(2)
                End If
                For i = 0 To n - 1
                    x(i) = tmp
                Next
                Return
            End If

            '
            ' compute tied ranks
            '
            i = 0
            While i <= n - 1
                j = i + 1
                While j <= n - 1
                    If CDbl(buf.ra1(j)) <> CDbl(buf.ra1(i)) Then
                        Exit While
                    End If
                    j = j + 1
                End While
                For k = i To j - 1
                    buf.ra1(k) = CDbl(i + j - 1) / CDbl(2)
                Next
                i = j
            End While

            '
            ' back to x
            '
            If iscentered Then
                voffs = CDbl(n - 1) / CDbl(2)
            Else
                voffs = 0.0
            End If
            For i = 0 To n - 1
                x(buf.ia1(i)) = buf.ra1(i) - voffs
            Next
        End Sub


    End Class
    Public Class ablasf
        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixrank1f(m As Integer, n As Integer, ByRef a As complex(,), ia As Integer, ja As Integer, ByRef u As complex(), _
            iu As Integer, ByRef v As complex(), iv As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixrank1f(m As Integer, n As Integer, ByRef a As Double(,), ia As Integer, ja As Integer, ByRef u As Double(), _
            iu As Integer, ByRef v As Double(), iv As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixmvf(m As Integer, n As Integer, a As complex(,), ia As Integer, ja As Integer, opa As Integer, _
            x As complex(), ix As Integer, ByRef y As complex(), iy As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixmvf(m As Integer, n As Integer, a As Double(,), ia As Integer, ja As Integer, opa As Integer, _
            x As Double(), ix As Integer, ByRef y As Double(), iy As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixrighttrsmf(m As Integer, n As Integer, a As complex(,), i1 As Integer, j1 As Integer, isupper As Boolean, _
            isunit As Boolean, optype As Integer, x As complex(,), i2 As Integer, j2 As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixlefttrsmf(m As Integer, n As Integer, a As complex(,), i1 As Integer, j1 As Integer, isupper As Boolean, _
            isunit As Boolean, optype As Integer, x As complex(,), i2 As Integer, j2 As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixrighttrsmf(m As Integer, n As Integer, a As Double(,), i1 As Integer, j1 As Integer, isupper As Boolean, _
            isunit As Boolean, optype As Integer, x As Double(,), i2 As Integer, j2 As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixlefttrsmf(m As Integer, n As Integer, a As Double(,), i1 As Integer, j1 As Integer, isupper As Boolean, _
            isunit As Boolean, optype As Integer, x As Double(,), i2 As Integer, j2 As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixherkf(n As Integer, k As Integer, alpha As Double, a As complex(,), ia As Integer, ja As Integer, _
            optypea As Integer, beta As Double, c As complex(,), ic As Integer, jc As Integer, isupper As Boolean) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixsyrkf(n As Integer, k As Integer, alpha As Double, a As Double(,), ia As Integer, ja As Integer, _
            optypea As Integer, beta As Double, c As Double(,), ic As Integer, jc As Integer, isupper As Boolean) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixgemmf(m As Integer, n As Integer, k As Integer, alpha As Double, a As Double(,), ia As Integer, _
            ja As Integer, optypea As Integer, b As Double(,), ib As Integer, jb As Integer, optypeb As Integer, _
            beta As Double, c As Double(,), ic As Integer, jc As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel
        '
        '          -- ALGLIB routine --
        '             19.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixgemmf(m As Integer, n As Integer, k As Integer, alpha As complex, a As complex(,), ia As Integer, _
            ja As Integer, optypea As Integer, b As complex(,), ib As Integer, jb As Integer, optypeb As Integer, _
            beta As complex, c As complex(,), ic As Integer, jc As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        CMatrixGEMM kernel, basecase code for CMatrixGEMM.
        '
        '        This subroutine calculates C = alpha*op1(A)*op2(B) +beta*C where:
        '        * C is MxN general matrix
        '        * op1(A) is MxK matrix
        '        * op2(B) is KxN matrix
        '        * "op" may be identity transformation, transposition, conjugate transposition
        '
        '        Additional info:
        '        * multiplication result replaces C. If Beta=0, C elements are not used in
        '          calculations (not multiplied by zero - just not referenced)
        '        * if Alpha=0, A is not used (not multiplied by zero - just not referenced)
        '        * if both Beta and Alpha are zero, C is filled by zeros.
        '
        '        IMPORTANT:
        '
        '        This function does NOT preallocate output matrix C, it MUST be preallocated
        '        by caller prior to calling this function. In case C does not have  enough
        '        space to store result, exception will be generated.
        '
        '        INPUT PARAMETERS
        '            M       -   matrix size, M>0
        '            N       -   matrix size, N>0
        '            K       -   matrix size, K>0
        '            Alpha   -   coefficient
        '            A       -   matrix
        '            IA      -   submatrix offset
        '            JA      -   submatrix offset
        '            OpTypeA -   transformation type:
        '                        * 0 - no transformation
        '                        * 1 - transposition
        '                        * 2 - conjugate transposition
        '            B       -   matrix
        '            IB      -   submatrix offset
        '            JB      -   submatrix offset
        '            OpTypeB -   transformation type:
        '                        * 0 - no transformation
        '                        * 1 - transposition
        '                        * 2 - conjugate transposition
        '            Beta    -   coefficient
        '            C       -   PREALLOCATED output matrix
        '            IC      -   submatrix offset
        '            JC      -   submatrix offset
        '
        '          -- ALGLIB routine --
        '             27.03.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub cmatrixgemmk(m As Integer, n As Integer, k As Integer, alpha As complex, a As complex(,), ia As Integer, _
            ja As Integer, optypea As Integer, b As complex(,), ib As Integer, jb As Integer, optypeb As Integer, _
            beta As complex, c As complex(,), ic As Integer, jc As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim v As complex = 0
            Dim v00 As complex = 0
            Dim v01 As complex = 0
            Dim v10 As complex = 0
            Dim v11 As complex = 0
            Dim v00x As Double = 0
            Dim v00y As Double = 0
            Dim v01x As Double = 0
            Dim v01y As Double = 0
            Dim v10x As Double = 0
            Dim v10y As Double = 0
            Dim v11x As Double = 0
            Dim v11y As Double = 0
            Dim a0x As Double = 0
            Dim a0y As Double = 0
            Dim a1x As Double = 0
            Dim a1y As Double = 0
            Dim b0x As Double = 0
            Dim b0y As Double = 0
            Dim b1x As Double = 0
            Dim b1y As Double = 0
            Dim idxa0 As Integer = 0
            Dim idxa1 As Integer = 0
            Dim idxb0 As Integer = 0
            Dim idxb1 As Integer = 0
            Dim i0 As Integer = 0
            Dim i1 As Integer = 0
            Dim ik As Integer = 0
            Dim j0 As Integer = 0
            Dim j1 As Integer = 0
            Dim jk As Integer = 0
            Dim t As Integer = 0
            Dim offsa As Integer = 0
            Dim offsb As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0


            '
            ' if matrix size is zero
            '
            If m = 0 OrElse n = 0 Then
                Return
            End If

            '
            ' Try optimized code
            '
            If cmatrixgemmf(m, n, k, alpha, a, ia, _
                ja, optypea, b, ib, jb, optypeb, _
                beta, c, ic, jc) Then
                Return
            End If

            '
            ' if K=0, then C=Beta*C
            '
            If k = 0 Then
                If beta <> 1 Then
                    If beta <> 0 Then
                        For i = 0 To m - 1
                            For j = 0 To n - 1
                                c(ic + i, jc + j) = beta * c(ic + i, jc + j)
                            Next
                        Next
                    Else
                        For i = 0 To m - 1
                            For j = 0 To n - 1
                                c(ic + i, jc + j) = 0
                            Next
                        Next
                    End If
                End If
                Return
            End If

            '
            ' This phase is not really necessary, but compiler complains
            ' about "possibly uninitialized variables"
            '
            a0x = 0
            a0y = 0
            a1x = 0
            a1y = 0
            b0x = 0
            b0y = 0
            b1x = 0
            b1y = 0

            '
            ' General case
            '
            i = 0
            While i < m
                j = 0
                While j < n

                    '
                    ' Choose between specialized 4x4 code and general code
                    '
                    If i + 2 <= m AndAlso j + 2 <= n Then

                        '
                        ' Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        '
                        ' This submatrix is calculated as sum of K rank-1 products,
                        ' with operands cached in local variables in order to speed
                        ' up operations with arrays.
                        '
                        v00x = 0.0
                        v00y = 0.0
                        v01x = 0.0
                        v01y = 0.0
                        v10x = 0.0
                        v10y = 0.0
                        v11x = 0.0
                        v11y = 0.0
                        If optypea = 0 Then
                            idxa0 = ia + i + 0
                            idxa1 = ia + i + 1
                            offsa = ja
                        Else
                            idxa0 = ja + i + 0
                            idxa1 = ja + i + 1
                            offsa = ia
                        End If
                        If optypeb = 0 Then
                            idxb0 = jb + j + 0
                            idxb1 = jb + j + 1
                            offsb = ib
                        Else
                            idxb0 = ib + j + 0
                            idxb1 = ib + j + 1
                            offsb = jb
                        End If
                        For t = 0 To k - 1
                            If optypea = 0 Then
                                a0x = a(idxa0, offsa).x
                                a0y = a(idxa0, offsa).y
                                a1x = a(idxa1, offsa).x
                                a1y = a(idxa1, offsa).y
                            End If
                            If optypea = 1 Then
                                a0x = a(offsa, idxa0).x
                                a0y = a(offsa, idxa0).y
                                a1x = a(offsa, idxa1).x
                                a1y = a(offsa, idxa1).y
                            End If
                            If optypea = 2 Then
                                a0x = a(offsa, idxa0).x
                                a0y = -a(offsa, idxa0).y
                                a1x = a(offsa, idxa1).x
                                a1y = -a(offsa, idxa1).y
                            End If
                            If optypeb = 0 Then
                                b0x = b(offsb, idxb0).x
                                b0y = b(offsb, idxb0).y
                                b1x = b(offsb, idxb1).x
                                b1y = b(offsb, idxb1).y
                            End If
                            If optypeb = 1 Then
                                b0x = b(idxb0, offsb).x
                                b0y = b(idxb0, offsb).y
                                b1x = b(idxb1, offsb).x
                                b1y = b(idxb1, offsb).y
                            End If
                            If optypeb = 2 Then
                                b0x = b(idxb0, offsb).x
                                b0y = -b(idxb0, offsb).y
                                b1x = b(idxb1, offsb).x
                                b1y = -b(idxb1, offsb).y
                            End If
                            v00x = v00x + a0x * b0x - a0y * b0y
                            v00y = v00y + a0x * b0y + a0y * b0x
                            v01x = v01x + a0x * b1x - a0y * b1y
                            v01y = v01y + a0x * b1y + a0y * b1x
                            v10x = v10x + a1x * b0x - a1y * b0y
                            v10y = v10y + a1x * b0y + a1y * b0x
                            v11x = v11x + a1x * b1x - a1y * b1y
                            v11y = v11y + a1x * b1y + a1y * b1x
                            offsa = offsa + 1
                            offsb = offsb + 1
                        Next
                        v00.x = v00x
                        v00.y = v00y
                        v10.x = v10x
                        v10.y = v10y
                        v01.x = v01x
                        v01.y = v01y
                        v11.x = v11x
                        v11.y = v11y
                        If beta = 0 Then
                            c(ic + i + 0, jc + j + 0) = alpha * v00
                            c(ic + i + 0, jc + j + 1) = alpha * v01
                            c(ic + i + 1, jc + j + 0) = alpha * v10
                            c(ic + i + 1, jc + j + 1) = alpha * v11
                        Else
                            c(ic + i + 0, jc + j + 0) = beta * c(ic + i + 0, jc + j + 0) + alpha * v00
                            c(ic + i + 0, jc + j + 1) = beta * c(ic + i + 0, jc + j + 1) + alpha * v01
                            c(ic + i + 1, jc + j + 0) = beta * c(ic + i + 1, jc + j + 0) + alpha * v10
                            c(ic + i + 1, jc + j + 1) = beta * c(ic + i + 1, jc + j + 1) + alpha * v11
                        End If
                    Else

                        '
                        ' Determine submatrix [I0..I1]x[J0..J1] to process
                        '
                        i0 = i
                        i1 = System.Math.Min(i + 1, m - 1)
                        j0 = j
                        j1 = System.Math.Min(j + 1, n - 1)

                        '
                        ' Process submatrix
                        '
                        For ik = i0 To i1
                            For jk = j0 To j1
                                If k = 0 OrElse alpha = 0 Then
                                    v = 0
                                Else
                                    v = 0.0
                                    If optypea = 0 AndAlso optypeb = 0 Then
                                        i1_ = (ib) - (ja)
                                        v = 0.0
                                        For i_ = ja To ja + k - 1
                                            v += a(ia + ik, i_) * b(i_ + i1_, jb + jk)
                                        Next
                                    End If
                                    If optypea = 0 AndAlso optypeb = 1 Then
                                        i1_ = (jb) - (ja)
                                        v = 0.0
                                        For i_ = ja To ja + k - 1
                                            v += a(ia + ik, i_) * b(ib + jk, i_ + i1_)
                                        Next
                                    End If
                                    If optypea = 0 AndAlso optypeb = 2 Then
                                        i1_ = (jb) - (ja)
                                        v = 0.0
                                        For i_ = ja To ja + k - 1
                                            v += a(ia + ik, i_) * Math.conj(b(ib + jk, i_ + i1_))
                                        Next
                                    End If
                                    If optypea = 1 AndAlso optypeb = 0 Then
                                        i1_ = (ib) - (ia)
                                        v = 0.0
                                        For i_ = ia To ia + k - 1
                                            v += a(i_, ja + ik) * b(i_ + i1_, jb + jk)
                                        Next
                                    End If
                                    If optypea = 1 AndAlso optypeb = 1 Then
                                        i1_ = (jb) - (ia)
                                        v = 0.0
                                        For i_ = ia To ia + k - 1
                                            v += a(i_, ja + ik) * b(ib + jk, i_ + i1_)
                                        Next
                                    End If
                                    If optypea = 1 AndAlso optypeb = 2 Then
                                        i1_ = (jb) - (ia)
                                        v = 0.0
                                        For i_ = ia To ia + k - 1
                                            v += a(i_, ja + ik) * Math.conj(b(ib + jk, i_ + i1_))
                                        Next
                                    End If
                                    If optypea = 2 AndAlso optypeb = 0 Then
                                        i1_ = (ib) - (ia)
                                        v = 0.0
                                        For i_ = ia To ia + k - 1
                                            v += Math.conj(a(i_, ja + ik)) * b(i_ + i1_, jb + jk)
                                        Next
                                    End If
                                    If optypea = 2 AndAlso optypeb = 1 Then
                                        i1_ = (jb) - (ia)
                                        v = 0.0
                                        For i_ = ia To ia + k - 1
                                            v += Math.conj(a(i_, ja + ik)) * b(ib + jk, i_ + i1_)
                                        Next
                                    End If
                                    If optypea = 2 AndAlso optypeb = 2 Then
                                        i1_ = (jb) - (ia)
                                        v = 0.0
                                        For i_ = ia To ia + k - 1
                                            v += Math.conj(a(i_, ja + ik)) * Math.conj(b(ib + jk, i_ + i1_))
                                        Next
                                    End If
                                End If
                                If beta = 0 Then
                                    c(ic + ik, jc + jk) = alpha * v
                                Else
                                    c(ic + ik, jc + jk) = beta * c(ic + ik, jc + jk) + alpha * v
                                End If
                            Next
                        Next
                    End If
                    j = j + 2
                End While
                i = i + 2
            End While
        End Sub


        '************************************************************************
        '        RMatrixGEMM kernel, basecase code for RMatrixGEMM.
        '
        '        This subroutine calculates C = alpha*op1(A)*op2(B) +beta*C where:
        '        * C is MxN general matrix
        '        * op1(A) is MxK matrix
        '        * op2(B) is KxN matrix
        '        * "op" may be identity transformation, transposition
        '
        '        Additional info:
        '        * multiplication result replaces C. If Beta=0, C elements are not used in
        '          calculations (not multiplied by zero - just not referenced)
        '        * if Alpha=0, A is not used (not multiplied by zero - just not referenced)
        '        * if both Beta and Alpha are zero, C is filled by zeros.
        '
        '        IMPORTANT:
        '
        '        This function does NOT preallocate output matrix C, it MUST be preallocated
        '        by caller prior to calling this function. In case C does not have  enough
        '        space to store result, exception will be generated.
        '
        '        INPUT PARAMETERS
        '            M       -   matrix size, M>0
        '            N       -   matrix size, N>0
        '            K       -   matrix size, K>0
        '            Alpha   -   coefficient
        '            A       -   matrix
        '            IA      -   submatrix offset
        '            JA      -   submatrix offset
        '            OpTypeA -   transformation type:
        '                        * 0 - no transformation
        '                        * 1 - transposition
        '            B       -   matrix
        '            IB      -   submatrix offset
        '            JB      -   submatrix offset
        '            OpTypeB -   transformation type:
        '                        * 0 - no transformation
        '                        * 1 - transposition
        '            Beta    -   coefficient
        '            C       -   PREALLOCATED output matrix
        '            IC      -   submatrix offset
        '            JC      -   submatrix offset
        '
        '          -- ALGLIB routine --
        '             27.03.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixgemmk(m As Integer, n As Integer, k As Integer, alpha As Double, a As Double(,), ia As Integer, _
            ja As Integer, optypea As Integer, b As Double(,), ib As Integer, jb As Integer, optypeb As Integer, _
            beta As Double, c As Double(,), ic As Integer, jc As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0


            '
            ' if matrix size is zero
            '
            If m = 0 OrElse n = 0 Then
                Return
            End If

            '
            ' Try optimized code
            '
            If rmatrixgemmf(m, n, k, alpha, a, ia, _
                ja, optypea, b, ib, jb, optypeb, _
                beta, c, ic, jc) Then
                Return
            End If

            '
            ' if K=0, then C=Beta*C
            '
            If k = 0 OrElse CDbl(alpha) = CDbl(0) Then
                If CDbl(beta) <> CDbl(1) Then
                    If CDbl(beta) <> CDbl(0) Then
                        For i = 0 To m - 1
                            For j = 0 To n - 1
                                c(ic + i, jc + j) = beta * c(ic + i, jc + j)
                            Next
                        Next
                    Else
                        For i = 0 To m - 1
                            For j = 0 To n - 1
                                c(ic + i, jc + j) = 0
                            Next
                        Next
                    End If
                End If
                Return
            End If

            '
            ' Call specialized code.
            '
            ' NOTE: specialized code was moved to separate function because of strange
            '       issues with instructions cache on some systems; Having too long
            '       functions significantly slows down internal loop of the algorithm.
            '
            If optypea = 0 AndAlso optypeb = 0 Then
                rmatrixgemmk44v00(m, n, k, alpha, a, ia, _
                    ja, b, ib, jb, beta, c, _
                    ic, jc)
            End If
            If optypea = 0 AndAlso optypeb <> 0 Then
                rmatrixgemmk44v01(m, n, k, alpha, a, ia, _
                    ja, b, ib, jb, beta, c, _
                    ic, jc)
            End If
            If optypea <> 0 AndAlso optypeb = 0 Then
                rmatrixgemmk44v10(m, n, k, alpha, a, ia, _
                    ja, b, ib, jb, beta, c, _
                    ic, jc)
            End If
            If optypea <> 0 AndAlso optypeb <> 0 Then
                rmatrixgemmk44v11(m, n, k, alpha, a, ia, _
                    ja, b, ib, jb, beta, c, _
                    ic, jc)
            End If
        End Sub


        '************************************************************************
        '        RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
        '        with OpTypeA=0 and OpTypeB=0.
        '
        '        Additional info:
        '        * this function requires that Alpha<>0 (assertion is thrown otherwise)
        '
        '        INPUT PARAMETERS
        '            M       -   matrix size, M>0
        '            N       -   matrix size, N>0
        '            K       -   matrix size, K>0
        '            Alpha   -   coefficient
        '            A       -   matrix
        '            IA      -   submatrix offset
        '            JA      -   submatrix offset
        '            B       -   matrix
        '            IB      -   submatrix offset
        '            JB      -   submatrix offset
        '            Beta    -   coefficient
        '            C       -   PREALLOCATED output matrix
        '            IC      -   submatrix offset
        '            JC      -   submatrix offset
        '
        '          -- ALGLIB routine --
        '             27.03.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixgemmk44v00(m As Integer, n As Integer, k As Integer, alpha As Double, a As Double(,), ia As Integer, _
            ja As Integer, b As Double(,), ib As Integer, jb As Integer, beta As Double, c As Double(,), _
            ic As Integer, jc As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim v As Double = 0
            Dim v00 As Double = 0
            Dim v01 As Double = 0
            Dim v02 As Double = 0
            Dim v03 As Double = 0
            Dim v10 As Double = 0
            Dim v11 As Double = 0
            Dim v12 As Double = 0
            Dim v13 As Double = 0
            Dim v20 As Double = 0
            Dim v21 As Double = 0
            Dim v22 As Double = 0
            Dim v23 As Double = 0
            Dim v30 As Double = 0
            Dim v31 As Double = 0
            Dim v32 As Double = 0
            Dim v33 As Double = 0
            Dim a0 As Double = 0
            Dim a1 As Double = 0
            Dim a2 As Double = 0
            Dim a3 As Double = 0
            Dim b0 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0
            Dim b3 As Double = 0
            Dim idxa0 As Integer = 0
            Dim idxa1 As Integer = 0
            Dim idxa2 As Integer = 0
            Dim idxa3 As Integer = 0
            Dim idxb0 As Integer = 0
            Dim idxb1 As Integer = 0
            Dim idxb2 As Integer = 0
            Dim idxb3 As Integer = 0
            Dim i0 As Integer = 0
            Dim i1 As Integer = 0
            Dim ik As Integer = 0
            Dim j0 As Integer = 0
            Dim j1 As Integer = 0
            Dim jk As Integer = 0
            Dim t As Integer = 0
            Dim offsa As Integer = 0
            Dim offsb As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            alglib.ap.assert(CDbl(alpha) <> CDbl(0), "RMatrixGEMMK44V00: internal error (Alpha=0)")

            '
            ' if matrix size is zero
            '
            If m = 0 OrElse n = 0 Then
                Return
            End If

            '
            ' A*B
            '
            i = 0
            While i < m
                j = 0
                While j < n

                    '
                    ' Choose between specialized 4x4 code and general code
                    '
                    If i + 4 <= m AndAlso j + 4 <= n Then

                        '
                        ' Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        '
                        ' This submatrix is calculated as sum of K rank-1 products,
                        ' with operands cached in local variables in order to speed
                        ' up operations with arrays.
                        '
                        idxa0 = ia + i + 0
                        idxa1 = ia + i + 1
                        idxa2 = ia + i + 2
                        idxa3 = ia + i + 3
                        offsa = ja
                        idxb0 = jb + j + 0
                        idxb1 = jb + j + 1
                        idxb2 = jb + j + 2
                        idxb3 = jb + j + 3
                        offsb = ib
                        v00 = 0.0
                        v01 = 0.0
                        v02 = 0.0
                        v03 = 0.0
                        v10 = 0.0
                        v11 = 0.0
                        v12 = 0.0
                        v13 = 0.0
                        v20 = 0.0
                        v21 = 0.0
                        v22 = 0.0
                        v23 = 0.0
                        v30 = 0.0
                        v31 = 0.0
                        v32 = 0.0
                        v33 = 0.0

                        '
                        ' Different variants of internal loop
                        '
                        For t = 0 To k - 1
                            a0 = a(idxa0, offsa)
                            a1 = a(idxa1, offsa)
                            b0 = b(offsb, idxb0)
                            b1 = b(offsb, idxb1)
                            v00 = v00 + a0 * b0
                            v01 = v01 + a0 * b1
                            v10 = v10 + a1 * b0
                            v11 = v11 + a1 * b1
                            a2 = a(idxa2, offsa)
                            a3 = a(idxa3, offsa)
                            v20 = v20 + a2 * b0
                            v21 = v21 + a2 * b1
                            v30 = v30 + a3 * b0
                            v31 = v31 + a3 * b1
                            b2 = b(offsb, idxb2)
                            b3 = b(offsb, idxb3)
                            v22 = v22 + a2 * b2
                            v23 = v23 + a2 * b3
                            v32 = v32 + a3 * b2
                            v33 = v33 + a3 * b3
                            v02 = v02 + a0 * b2
                            v03 = v03 + a0 * b3
                            v12 = v12 + a1 * b2
                            v13 = v13 + a1 * b3
                            offsa = offsa + 1
                            offsb = offsb + 1
                        Next
                        If CDbl(beta) = CDbl(0) Then
                            c(ic + i + 0, jc + j + 0) = alpha * v00
                            c(ic + i + 0, jc + j + 1) = alpha * v01
                            c(ic + i + 0, jc + j + 2) = alpha * v02
                            c(ic + i + 0, jc + j + 3) = alpha * v03
                            c(ic + i + 1, jc + j + 0) = alpha * v10
                            c(ic + i + 1, jc + j + 1) = alpha * v11
                            c(ic + i + 1, jc + j + 2) = alpha * v12
                            c(ic + i + 1, jc + j + 3) = alpha * v13
                            c(ic + i + 2, jc + j + 0) = alpha * v20
                            c(ic + i + 2, jc + j + 1) = alpha * v21
                            c(ic + i + 2, jc + j + 2) = alpha * v22
                            c(ic + i + 2, jc + j + 3) = alpha * v23
                            c(ic + i + 3, jc + j + 0) = alpha * v30
                            c(ic + i + 3, jc + j + 1) = alpha * v31
                            c(ic + i + 3, jc + j + 2) = alpha * v32
                            c(ic + i + 3, jc + j + 3) = alpha * v33
                        Else
                            c(ic + i + 0, jc + j + 0) = beta * c(ic + i + 0, jc + j + 0) + alpha * v00
                            c(ic + i + 0, jc + j + 1) = beta * c(ic + i + 0, jc + j + 1) + alpha * v01
                            c(ic + i + 0, jc + j + 2) = beta * c(ic + i + 0, jc + j + 2) + alpha * v02
                            c(ic + i + 0, jc + j + 3) = beta * c(ic + i + 0, jc + j + 3) + alpha * v03
                            c(ic + i + 1, jc + j + 0) = beta * c(ic + i + 1, jc + j + 0) + alpha * v10
                            c(ic + i + 1, jc + j + 1) = beta * c(ic + i + 1, jc + j + 1) + alpha * v11
                            c(ic + i + 1, jc + j + 2) = beta * c(ic + i + 1, jc + j + 2) + alpha * v12
                            c(ic + i + 1, jc + j + 3) = beta * c(ic + i + 1, jc + j + 3) + alpha * v13
                            c(ic + i + 2, jc + j + 0) = beta * c(ic + i + 2, jc + j + 0) + alpha * v20
                            c(ic + i + 2, jc + j + 1) = beta * c(ic + i + 2, jc + j + 1) + alpha * v21
                            c(ic + i + 2, jc + j + 2) = beta * c(ic + i + 2, jc + j + 2) + alpha * v22
                            c(ic + i + 2, jc + j + 3) = beta * c(ic + i + 2, jc + j + 3) + alpha * v23
                            c(ic + i + 3, jc + j + 0) = beta * c(ic + i + 3, jc + j + 0) + alpha * v30
                            c(ic + i + 3, jc + j + 1) = beta * c(ic + i + 3, jc + j + 1) + alpha * v31
                            c(ic + i + 3, jc + j + 2) = beta * c(ic + i + 3, jc + j + 2) + alpha * v32
                            c(ic + i + 3, jc + j + 3) = beta * c(ic + i + 3, jc + j + 3) + alpha * v33
                        End If
                    Else

                        '
                        ' Determine submatrix [I0..I1]x[J0..J1] to process
                        '
                        i0 = i
                        i1 = System.Math.Min(i + 3, m - 1)
                        j0 = j
                        j1 = System.Math.Min(j + 3, n - 1)

                        '
                        ' Process submatrix
                        '
                        For ik = i0 To i1
                            For jk = j0 To j1
                                If k = 0 OrElse CDbl(alpha) = CDbl(0) Then
                                    v = 0
                                Else
                                    i1_ = (ib) - (ja)
                                    v = 0.0
                                    For i_ = ja To ja + k - 1
                                        v += a(ia + ik, i_) * b(i_ + i1_, jb + jk)
                                    Next
                                End If
                                If CDbl(beta) = CDbl(0) Then
                                    c(ic + ik, jc + jk) = alpha * v
                                Else
                                    c(ic + ik, jc + jk) = beta * c(ic + ik, jc + jk) + alpha * v
                                End If
                            Next
                        Next
                    End If
                    j = j + 4
                End While
                i = i + 4
            End While
        End Sub


        '************************************************************************
        '        RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
        '        with OpTypeA=0 and OpTypeB=1.
        '
        '        Additional info:
        '        * this function requires that Alpha<>0 (assertion is thrown otherwise)
        '
        '        INPUT PARAMETERS
        '            M       -   matrix size, M>0
        '            N       -   matrix size, N>0
        '            K       -   matrix size, K>0
        '            Alpha   -   coefficient
        '            A       -   matrix
        '            IA      -   submatrix offset
        '            JA      -   submatrix offset
        '            B       -   matrix
        '            IB      -   submatrix offset
        '            JB      -   submatrix offset
        '            Beta    -   coefficient
        '            C       -   PREALLOCATED output matrix
        '            IC      -   submatrix offset
        '            JC      -   submatrix offset
        '
        '          -- ALGLIB routine --
        '             27.03.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixgemmk44v01(m As Integer, n As Integer, k As Integer, alpha As Double, a As Double(,), ia As Integer, _
            ja As Integer, b As Double(,), ib As Integer, jb As Integer, beta As Double, c As Double(,), _
            ic As Integer, jc As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim v As Double = 0
            Dim v00 As Double = 0
            Dim v01 As Double = 0
            Dim v02 As Double = 0
            Dim v03 As Double = 0
            Dim v10 As Double = 0
            Dim v11 As Double = 0
            Dim v12 As Double = 0
            Dim v13 As Double = 0
            Dim v20 As Double = 0
            Dim v21 As Double = 0
            Dim v22 As Double = 0
            Dim v23 As Double = 0
            Dim v30 As Double = 0
            Dim v31 As Double = 0
            Dim v32 As Double = 0
            Dim v33 As Double = 0
            Dim a0 As Double = 0
            Dim a1 As Double = 0
            Dim a2 As Double = 0
            Dim a3 As Double = 0
            Dim b0 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0
            Dim b3 As Double = 0
            Dim idxa0 As Integer = 0
            Dim idxa1 As Integer = 0
            Dim idxa2 As Integer = 0
            Dim idxa3 As Integer = 0
            Dim idxb0 As Integer = 0
            Dim idxb1 As Integer = 0
            Dim idxb2 As Integer = 0
            Dim idxb3 As Integer = 0
            Dim i0 As Integer = 0
            Dim i1 As Integer = 0
            Dim ik As Integer = 0
            Dim j0 As Integer = 0
            Dim j1 As Integer = 0
            Dim jk As Integer = 0
            Dim t As Integer = 0
            Dim offsa As Integer = 0
            Dim offsb As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            alglib.ap.assert(CDbl(alpha) <> CDbl(0), "RMatrixGEMMK44V00: internal error (Alpha=0)")

            '
            ' if matrix size is zero
            '
            If m = 0 OrElse n = 0 Then
                Return
            End If

            '
            ' A*B'
            '
            i = 0
            While i < m
                j = 0
                While j < n

                    '
                    ' Choose between specialized 4x4 code and general code
                    '
                    If i + 4 <= m AndAlso j + 4 <= n Then

                        '
                        ' Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        '
                        ' This submatrix is calculated as sum of K rank-1 products,
                        ' with operands cached in local variables in order to speed
                        ' up operations with arrays.
                        '
                        idxa0 = ia + i + 0
                        idxa1 = ia + i + 1
                        idxa2 = ia + i + 2
                        idxa3 = ia + i + 3
                        offsa = ja
                        idxb0 = ib + j + 0
                        idxb1 = ib + j + 1
                        idxb2 = ib + j + 2
                        idxb3 = ib + j + 3
                        offsb = jb
                        v00 = 0.0
                        v01 = 0.0
                        v02 = 0.0
                        v03 = 0.0
                        v10 = 0.0
                        v11 = 0.0
                        v12 = 0.0
                        v13 = 0.0
                        v20 = 0.0
                        v21 = 0.0
                        v22 = 0.0
                        v23 = 0.0
                        v30 = 0.0
                        v31 = 0.0
                        v32 = 0.0
                        v33 = 0.0
                        For t = 0 To k - 1
                            a0 = a(idxa0, offsa)
                            a1 = a(idxa1, offsa)
                            b0 = b(idxb0, offsb)
                            b1 = b(idxb1, offsb)
                            v00 = v00 + a0 * b0
                            v01 = v01 + a0 * b1
                            v10 = v10 + a1 * b0
                            v11 = v11 + a1 * b1
                            a2 = a(idxa2, offsa)
                            a3 = a(idxa3, offsa)
                            v20 = v20 + a2 * b0
                            v21 = v21 + a2 * b1
                            v30 = v30 + a3 * b0
                            v31 = v31 + a3 * b1
                            b2 = b(idxb2, offsb)
                            b3 = b(idxb3, offsb)
                            v22 = v22 + a2 * b2
                            v23 = v23 + a2 * b3
                            v32 = v32 + a3 * b2
                            v33 = v33 + a3 * b3
                            v02 = v02 + a0 * b2
                            v03 = v03 + a0 * b3
                            v12 = v12 + a1 * b2
                            v13 = v13 + a1 * b3
                            offsa = offsa + 1
                            offsb = offsb + 1
                        Next
                        If CDbl(beta) = CDbl(0) Then
                            c(ic + i + 0, jc + j + 0) = alpha * v00
                            c(ic + i + 0, jc + j + 1) = alpha * v01
                            c(ic + i + 0, jc + j + 2) = alpha * v02
                            c(ic + i + 0, jc + j + 3) = alpha * v03
                            c(ic + i + 1, jc + j + 0) = alpha * v10
                            c(ic + i + 1, jc + j + 1) = alpha * v11
                            c(ic + i + 1, jc + j + 2) = alpha * v12
                            c(ic + i + 1, jc + j + 3) = alpha * v13
                            c(ic + i + 2, jc + j + 0) = alpha * v20
                            c(ic + i + 2, jc + j + 1) = alpha * v21
                            c(ic + i + 2, jc + j + 2) = alpha * v22
                            c(ic + i + 2, jc + j + 3) = alpha * v23
                            c(ic + i + 3, jc + j + 0) = alpha * v30
                            c(ic + i + 3, jc + j + 1) = alpha * v31
                            c(ic + i + 3, jc + j + 2) = alpha * v32
                            c(ic + i + 3, jc + j + 3) = alpha * v33
                        Else
                            c(ic + i + 0, jc + j + 0) = beta * c(ic + i + 0, jc + j + 0) + alpha * v00
                            c(ic + i + 0, jc + j + 1) = beta * c(ic + i + 0, jc + j + 1) + alpha * v01
                            c(ic + i + 0, jc + j + 2) = beta * c(ic + i + 0, jc + j + 2) + alpha * v02
                            c(ic + i + 0, jc + j + 3) = beta * c(ic + i + 0, jc + j + 3) + alpha * v03
                            c(ic + i + 1, jc + j + 0) = beta * c(ic + i + 1, jc + j + 0) + alpha * v10
                            c(ic + i + 1, jc + j + 1) = beta * c(ic + i + 1, jc + j + 1) + alpha * v11
                            c(ic + i + 1, jc + j + 2) = beta * c(ic + i + 1, jc + j + 2) + alpha * v12
                            c(ic + i + 1, jc + j + 3) = beta * c(ic + i + 1, jc + j + 3) + alpha * v13
                            c(ic + i + 2, jc + j + 0) = beta * c(ic + i + 2, jc + j + 0) + alpha * v20
                            c(ic + i + 2, jc + j + 1) = beta * c(ic + i + 2, jc + j + 1) + alpha * v21
                            c(ic + i + 2, jc + j + 2) = beta * c(ic + i + 2, jc + j + 2) + alpha * v22
                            c(ic + i + 2, jc + j + 3) = beta * c(ic + i + 2, jc + j + 3) + alpha * v23
                            c(ic + i + 3, jc + j + 0) = beta * c(ic + i + 3, jc + j + 0) + alpha * v30
                            c(ic + i + 3, jc + j + 1) = beta * c(ic + i + 3, jc + j + 1) + alpha * v31
                            c(ic + i + 3, jc + j + 2) = beta * c(ic + i + 3, jc + j + 2) + alpha * v32
                            c(ic + i + 3, jc + j + 3) = beta * c(ic + i + 3, jc + j + 3) + alpha * v33
                        End If
                    Else

                        '
                        ' Determine submatrix [I0..I1]x[J0..J1] to process
                        '
                        i0 = i
                        i1 = System.Math.Min(i + 3, m - 1)
                        j0 = j
                        j1 = System.Math.Min(j + 3, n - 1)

                        '
                        ' Process submatrix
                        '
                        For ik = i0 To i1
                            For jk = j0 To j1
                                If k = 0 OrElse CDbl(alpha) = CDbl(0) Then
                                    v = 0
                                Else
                                    i1_ = (jb) - (ja)
                                    v = 0.0
                                    For i_ = ja To ja + k - 1
                                        v += a(ia + ik, i_) * b(ib + jk, i_ + i1_)
                                    Next
                                End If
                                If CDbl(beta) = CDbl(0) Then
                                    c(ic + ik, jc + jk) = alpha * v
                                Else
                                    c(ic + ik, jc + jk) = beta * c(ic + ik, jc + jk) + alpha * v
                                End If
                            Next
                        Next
                    End If
                    j = j + 4
                End While
                i = i + 4
            End While
        End Sub


        '************************************************************************
        '        RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
        '        with OpTypeA=1 and OpTypeB=0.
        '
        '        Additional info:
        '        * this function requires that Alpha<>0 (assertion is thrown otherwise)
        '
        '        INPUT PARAMETERS
        '            M       -   matrix size, M>0
        '            N       -   matrix size, N>0
        '            K       -   matrix size, K>0
        '            Alpha   -   coefficient
        '            A       -   matrix
        '            IA      -   submatrix offset
        '            JA      -   submatrix offset
        '            B       -   matrix
        '            IB      -   submatrix offset
        '            JB      -   submatrix offset
        '            Beta    -   coefficient
        '            C       -   PREALLOCATED output matrix
        '            IC      -   submatrix offset
        '            JC      -   submatrix offset
        '
        '          -- ALGLIB routine --
        '             27.03.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixgemmk44v10(m As Integer, n As Integer, k As Integer, alpha As Double, a As Double(,), ia As Integer, _
            ja As Integer, b As Double(,), ib As Integer, jb As Integer, beta As Double, c As Double(,), _
            ic As Integer, jc As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim v As Double = 0
            Dim v00 As Double = 0
            Dim v01 As Double = 0
            Dim v02 As Double = 0
            Dim v03 As Double = 0
            Dim v10 As Double = 0
            Dim v11 As Double = 0
            Dim v12 As Double = 0
            Dim v13 As Double = 0
            Dim v20 As Double = 0
            Dim v21 As Double = 0
            Dim v22 As Double = 0
            Dim v23 As Double = 0
            Dim v30 As Double = 0
            Dim v31 As Double = 0
            Dim v32 As Double = 0
            Dim v33 As Double = 0
            Dim a0 As Double = 0
            Dim a1 As Double = 0
            Dim a2 As Double = 0
            Dim a3 As Double = 0
            Dim b0 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0
            Dim b3 As Double = 0
            Dim idxa0 As Integer = 0
            Dim idxa1 As Integer = 0
            Dim idxa2 As Integer = 0
            Dim idxa3 As Integer = 0
            Dim idxb0 As Integer = 0
            Dim idxb1 As Integer = 0
            Dim idxb2 As Integer = 0
            Dim idxb3 As Integer = 0
            Dim i0 As Integer = 0
            Dim i1 As Integer = 0
            Dim ik As Integer = 0
            Dim j0 As Integer = 0
            Dim j1 As Integer = 0
            Dim jk As Integer = 0
            Dim t As Integer = 0
            Dim offsa As Integer = 0
            Dim offsb As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            alglib.ap.assert(CDbl(alpha) <> CDbl(0), "RMatrixGEMMK44V00: internal error (Alpha=0)")

            '
            ' if matrix size is zero
            '
            If m = 0 OrElse n = 0 Then
                Return
            End If

            '
            ' A'*B
            '
            i = 0
            While i < m
                j = 0
                While j < n

                    '
                    ' Choose between specialized 4x4 code and general code
                    '
                    If i + 4 <= m AndAlso j + 4 <= n Then

                        '
                        ' Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        '
                        ' This submatrix is calculated as sum of K rank-1 products,
                        ' with operands cached in local variables in order to speed
                        ' up operations with arrays.
                        '
                        idxa0 = ja + i + 0
                        idxa1 = ja + i + 1
                        idxa2 = ja + i + 2
                        idxa3 = ja + i + 3
                        offsa = ia
                        idxb0 = jb + j + 0
                        idxb1 = jb + j + 1
                        idxb2 = jb + j + 2
                        idxb3 = jb + j + 3
                        offsb = ib
                        v00 = 0.0
                        v01 = 0.0
                        v02 = 0.0
                        v03 = 0.0
                        v10 = 0.0
                        v11 = 0.0
                        v12 = 0.0
                        v13 = 0.0
                        v20 = 0.0
                        v21 = 0.0
                        v22 = 0.0
                        v23 = 0.0
                        v30 = 0.0
                        v31 = 0.0
                        v32 = 0.0
                        v33 = 0.0
                        For t = 0 To k - 1
                            a0 = a(offsa, idxa0)
                            a1 = a(offsa, idxa1)
                            b0 = b(offsb, idxb0)
                            b1 = b(offsb, idxb1)
                            v00 = v00 + a0 * b0
                            v01 = v01 + a0 * b1
                            v10 = v10 + a1 * b0
                            v11 = v11 + a1 * b1
                            a2 = a(offsa, idxa2)
                            a3 = a(offsa, idxa3)
                            v20 = v20 + a2 * b0
                            v21 = v21 + a2 * b1
                            v30 = v30 + a3 * b0
                            v31 = v31 + a3 * b1
                            b2 = b(offsb, idxb2)
                            b3 = b(offsb, idxb3)
                            v22 = v22 + a2 * b2
                            v23 = v23 + a2 * b3
                            v32 = v32 + a3 * b2
                            v33 = v33 + a3 * b3
                            v02 = v02 + a0 * b2
                            v03 = v03 + a0 * b3
                            v12 = v12 + a1 * b2
                            v13 = v13 + a1 * b3
                            offsa = offsa + 1
                            offsb = offsb + 1
                        Next
                        If CDbl(beta) = CDbl(0) Then
                            c(ic + i + 0, jc + j + 0) = alpha * v00
                            c(ic + i + 0, jc + j + 1) = alpha * v01
                            c(ic + i + 0, jc + j + 2) = alpha * v02
                            c(ic + i + 0, jc + j + 3) = alpha * v03
                            c(ic + i + 1, jc + j + 0) = alpha * v10
                            c(ic + i + 1, jc + j + 1) = alpha * v11
                            c(ic + i + 1, jc + j + 2) = alpha * v12
                            c(ic + i + 1, jc + j + 3) = alpha * v13
                            c(ic + i + 2, jc + j + 0) = alpha * v20
                            c(ic + i + 2, jc + j + 1) = alpha * v21
                            c(ic + i + 2, jc + j + 2) = alpha * v22
                            c(ic + i + 2, jc + j + 3) = alpha * v23
                            c(ic + i + 3, jc + j + 0) = alpha * v30
                            c(ic + i + 3, jc + j + 1) = alpha * v31
                            c(ic + i + 3, jc + j + 2) = alpha * v32
                            c(ic + i + 3, jc + j + 3) = alpha * v33
                        Else
                            c(ic + i + 0, jc + j + 0) = beta * c(ic + i + 0, jc + j + 0) + alpha * v00
                            c(ic + i + 0, jc + j + 1) = beta * c(ic + i + 0, jc + j + 1) + alpha * v01
                            c(ic + i + 0, jc + j + 2) = beta * c(ic + i + 0, jc + j + 2) + alpha * v02
                            c(ic + i + 0, jc + j + 3) = beta * c(ic + i + 0, jc + j + 3) + alpha * v03
                            c(ic + i + 1, jc + j + 0) = beta * c(ic + i + 1, jc + j + 0) + alpha * v10
                            c(ic + i + 1, jc + j + 1) = beta * c(ic + i + 1, jc + j + 1) + alpha * v11
                            c(ic + i + 1, jc + j + 2) = beta * c(ic + i + 1, jc + j + 2) + alpha * v12
                            c(ic + i + 1, jc + j + 3) = beta * c(ic + i + 1, jc + j + 3) + alpha * v13
                            c(ic + i + 2, jc + j + 0) = beta * c(ic + i + 2, jc + j + 0) + alpha * v20
                            c(ic + i + 2, jc + j + 1) = beta * c(ic + i + 2, jc + j + 1) + alpha * v21
                            c(ic + i + 2, jc + j + 2) = beta * c(ic + i + 2, jc + j + 2) + alpha * v22
                            c(ic + i + 2, jc + j + 3) = beta * c(ic + i + 2, jc + j + 3) + alpha * v23
                            c(ic + i + 3, jc + j + 0) = beta * c(ic + i + 3, jc + j + 0) + alpha * v30
                            c(ic + i + 3, jc + j + 1) = beta * c(ic + i + 3, jc + j + 1) + alpha * v31
                            c(ic + i + 3, jc + j + 2) = beta * c(ic + i + 3, jc + j + 2) + alpha * v32
                            c(ic + i + 3, jc + j + 3) = beta * c(ic + i + 3, jc + j + 3) + alpha * v33
                        End If
                    Else

                        '
                        ' Determine submatrix [I0..I1]x[J0..J1] to process
                        '
                        i0 = i
                        i1 = System.Math.Min(i + 3, m - 1)
                        j0 = j
                        j1 = System.Math.Min(j + 3, n - 1)

                        '
                        ' Process submatrix
                        '
                        For ik = i0 To i1
                            For jk = j0 To j1
                                If k = 0 OrElse CDbl(alpha) = CDbl(0) Then
                                    v = 0
                                Else
                                    v = 0.0
                                    i1_ = (ib) - (ia)
                                    v = 0.0
                                    For i_ = ia To ia + k - 1
                                        v += a(i_, ja + ik) * b(i_ + i1_, jb + jk)
                                    Next
                                End If
                                If CDbl(beta) = CDbl(0) Then
                                    c(ic + ik, jc + jk) = alpha * v
                                Else
                                    c(ic + ik, jc + jk) = beta * c(ic + ik, jc + jk) + alpha * v
                                End If
                            Next
                        Next
                    End If
                    j = j + 4
                End While
                i = i + 4
            End While
        End Sub


        '************************************************************************
        '        RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
        '        with OpTypeA=1 and OpTypeB=1.
        '
        '        Additional info:
        '        * this function requires that Alpha<>0 (assertion is thrown otherwise)
        '
        '        INPUT PARAMETERS
        '            M       -   matrix size, M>0
        '            N       -   matrix size, N>0
        '            K       -   matrix size, K>0
        '            Alpha   -   coefficient
        '            A       -   matrix
        '            IA      -   submatrix offset
        '            JA      -   submatrix offset
        '            B       -   matrix
        '            IB      -   submatrix offset
        '            JB      -   submatrix offset
        '            Beta    -   coefficient
        '            C       -   PREALLOCATED output matrix
        '            IC      -   submatrix offset
        '            JC      -   submatrix offset
        '
        '          -- ALGLIB routine --
        '             27.03.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixgemmk44v11(m As Integer, n As Integer, k As Integer, alpha As Double, a As Double(,), ia As Integer, _
            ja As Integer, b As Double(,), ib As Integer, jb As Integer, beta As Double, c As Double(,), _
            ic As Integer, jc As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim v As Double = 0
            Dim v00 As Double = 0
            Dim v01 As Double = 0
            Dim v02 As Double = 0
            Dim v03 As Double = 0
            Dim v10 As Double = 0
            Dim v11 As Double = 0
            Dim v12 As Double = 0
            Dim v13 As Double = 0
            Dim v20 As Double = 0
            Dim v21 As Double = 0
            Dim v22 As Double = 0
            Dim v23 As Double = 0
            Dim v30 As Double = 0
            Dim v31 As Double = 0
            Dim v32 As Double = 0
            Dim v33 As Double = 0
            Dim a0 As Double = 0
            Dim a1 As Double = 0
            Dim a2 As Double = 0
            Dim a3 As Double = 0
            Dim b0 As Double = 0
            Dim b1 As Double = 0
            Dim b2 As Double = 0
            Dim b3 As Double = 0
            Dim idxa0 As Integer = 0
            Dim idxa1 As Integer = 0
            Dim idxa2 As Integer = 0
            Dim idxa3 As Integer = 0
            Dim idxb0 As Integer = 0
            Dim idxb1 As Integer = 0
            Dim idxb2 As Integer = 0
            Dim idxb3 As Integer = 0
            Dim i0 As Integer = 0
            Dim i1 As Integer = 0
            Dim ik As Integer = 0
            Dim j0 As Integer = 0
            Dim j1 As Integer = 0
            Dim jk As Integer = 0
            Dim t As Integer = 0
            Dim offsa As Integer = 0
            Dim offsb As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            alglib.ap.assert(CDbl(alpha) <> CDbl(0), "RMatrixGEMMK44V00: internal error (Alpha=0)")

            '
            ' if matrix size is zero
            '
            If m = 0 OrElse n = 0 Then
                Return
            End If

            '
            ' A'*B'
            '
            i = 0
            While i < m
                j = 0
                While j < n

                    '
                    ' Choose between specialized 4x4 code and general code
                    '
                    If i + 4 <= m AndAlso j + 4 <= n Then

                        '
                        ' Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        '
                        ' This submatrix is calculated as sum of K rank-1 products,
                        ' with operands cached in local variables in order to speed
                        ' up operations with arrays.
                        '
                        idxa0 = ja + i + 0
                        idxa1 = ja + i + 1
                        idxa2 = ja + i + 2
                        idxa3 = ja + i + 3
                        offsa = ia
                        idxb0 = ib + j + 0
                        idxb1 = ib + j + 1
                        idxb2 = ib + j + 2
                        idxb3 = ib + j + 3
                        offsb = jb
                        v00 = 0.0
                        v01 = 0.0
                        v02 = 0.0
                        v03 = 0.0
                        v10 = 0.0
                        v11 = 0.0
                        v12 = 0.0
                        v13 = 0.0
                        v20 = 0.0
                        v21 = 0.0
                        v22 = 0.0
                        v23 = 0.0
                        v30 = 0.0
                        v31 = 0.0
                        v32 = 0.0
                        v33 = 0.0
                        For t = 0 To k - 1
                            a0 = a(offsa, idxa0)
                            a1 = a(offsa, idxa1)
                            b0 = b(idxb0, offsb)
                            b1 = b(idxb1, offsb)
                            v00 = v00 + a0 * b0
                            v01 = v01 + a0 * b1
                            v10 = v10 + a1 * b0
                            v11 = v11 + a1 * b1
                            a2 = a(offsa, idxa2)
                            a3 = a(offsa, idxa3)
                            v20 = v20 + a2 * b0
                            v21 = v21 + a2 * b1
                            v30 = v30 + a3 * b0
                            v31 = v31 + a3 * b1
                            b2 = b(idxb2, offsb)
                            b3 = b(idxb3, offsb)
                            v22 = v22 + a2 * b2
                            v23 = v23 + a2 * b3
                            v32 = v32 + a3 * b2
                            v33 = v33 + a3 * b3
                            v02 = v02 + a0 * b2
                            v03 = v03 + a0 * b3
                            v12 = v12 + a1 * b2
                            v13 = v13 + a1 * b3
                            offsa = offsa + 1
                            offsb = offsb + 1
                        Next
                        If CDbl(beta) = CDbl(0) Then
                            c(ic + i + 0, jc + j + 0) = alpha * v00
                            c(ic + i + 0, jc + j + 1) = alpha * v01
                            c(ic + i + 0, jc + j + 2) = alpha * v02
                            c(ic + i + 0, jc + j + 3) = alpha * v03
                            c(ic + i + 1, jc + j + 0) = alpha * v10
                            c(ic + i + 1, jc + j + 1) = alpha * v11
                            c(ic + i + 1, jc + j + 2) = alpha * v12
                            c(ic + i + 1, jc + j + 3) = alpha * v13
                            c(ic + i + 2, jc + j + 0) = alpha * v20
                            c(ic + i + 2, jc + j + 1) = alpha * v21
                            c(ic + i + 2, jc + j + 2) = alpha * v22
                            c(ic + i + 2, jc + j + 3) = alpha * v23
                            c(ic + i + 3, jc + j + 0) = alpha * v30
                            c(ic + i + 3, jc + j + 1) = alpha * v31
                            c(ic + i + 3, jc + j + 2) = alpha * v32
                            c(ic + i + 3, jc + j + 3) = alpha * v33
                        Else
                            c(ic + i + 0, jc + j + 0) = beta * c(ic + i + 0, jc + j + 0) + alpha * v00
                            c(ic + i + 0, jc + j + 1) = beta * c(ic + i + 0, jc + j + 1) + alpha * v01
                            c(ic + i + 0, jc + j + 2) = beta * c(ic + i + 0, jc + j + 2) + alpha * v02
                            c(ic + i + 0, jc + j + 3) = beta * c(ic + i + 0, jc + j + 3) + alpha * v03
                            c(ic + i + 1, jc + j + 0) = beta * c(ic + i + 1, jc + j + 0) + alpha * v10
                            c(ic + i + 1, jc + j + 1) = beta * c(ic + i + 1, jc + j + 1) + alpha * v11
                            c(ic + i + 1, jc + j + 2) = beta * c(ic + i + 1, jc + j + 2) + alpha * v12
                            c(ic + i + 1, jc + j + 3) = beta * c(ic + i + 1, jc + j + 3) + alpha * v13
                            c(ic + i + 2, jc + j + 0) = beta * c(ic + i + 2, jc + j + 0) + alpha * v20
                            c(ic + i + 2, jc + j + 1) = beta * c(ic + i + 2, jc + j + 1) + alpha * v21
                            c(ic + i + 2, jc + j + 2) = beta * c(ic + i + 2, jc + j + 2) + alpha * v22
                            c(ic + i + 2, jc + j + 3) = beta * c(ic + i + 2, jc + j + 3) + alpha * v23
                            c(ic + i + 3, jc + j + 0) = beta * c(ic + i + 3, jc + j + 0) + alpha * v30
                            c(ic + i + 3, jc + j + 1) = beta * c(ic + i + 3, jc + j + 1) + alpha * v31
                            c(ic + i + 3, jc + j + 2) = beta * c(ic + i + 3, jc + j + 2) + alpha * v32
                            c(ic + i + 3, jc + j + 3) = beta * c(ic + i + 3, jc + j + 3) + alpha * v33
                        End If
                    Else

                        '
                        ' Determine submatrix [I0..I1]x[J0..J1] to process
                        '
                        i0 = i
                        i1 = System.Math.Min(i + 3, m - 1)
                        j0 = j
                        j1 = System.Math.Min(j + 3, n - 1)

                        '
                        ' Process submatrix
                        '
                        For ik = i0 To i1
                            For jk = j0 To j1
                                If k = 0 OrElse CDbl(alpha) = CDbl(0) Then
                                    v = 0
                                Else
                                    v = 0.0
                                    i1_ = (jb) - (ia)
                                    v = 0.0
                                    For i_ = ia To ia + k - 1
                                        v += a(i_, ja + ik) * b(ib + jk, i_ + i1_)
                                    Next
                                End If
                                If CDbl(beta) = CDbl(0) Then
                                    c(ic + ik, jc + jk) = alpha * v
                                Else
                                    c(ic + ik, jc + jk) = beta * c(ic + ik, jc + jk) + alpha * v
                                End If
                            Next
                        Next
                    End If
                    j = j + 4
                End While
                i = i + 4
            End While
        End Sub


    End Class
    Public Class ablasmkl
        '************************************************************************
        '        MKL-based kernel
        '
        '          -- ALGLIB routine --
        '             01.10.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixsyrkmkl(n As Integer, k As Integer, alpha As Double, a As Double(,), ia As Integer, ja As Integer, _
            optypea As Integer, beta As Double, c As Double(,), ic As Integer, jc As Integer, isupper As Boolean) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel
        '
        '          -- ALGLIB routine --
        '             01.10.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixherkmkl(n As Integer, k As Integer, alpha As Double, a As complex(,), ia As Integer, ja As Integer, _
            optypea As Integer, beta As Double, c As complex(,), ic As Integer, jc As Integer, isupper As Boolean) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel
        '
        '          -- ALGLIB routine --
        '             01.10.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixgemmmkl(m As Integer, n As Integer, k As Integer, alpha As Double, a As Double(,), ia As Integer, _
            ja As Integer, optypea As Integer, b As Double(,), ib As Integer, jb As Integer, optypeb As Integer, _
            beta As Double, c As Double(,), ic As Integer, jc As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel
        '
        '          -- ALGLIB routine --
        '             16.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixgemmmkl(m As Integer, n As Integer, k As Integer, alpha As complex, a As complex(,), ia As Integer, _
            ja As Integer, optypea As Integer, b As complex(,), ib As Integer, jb As Integer, optypeb As Integer, _
            beta As complex, c As complex(,), ic As Integer, jc As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel
        '
        '          -- ALGLIB routine --
        '             16.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixlefttrsmmkl(m As Integer, n As Integer, a As complex(,), i1 As Integer, j1 As Integer, isupper As Boolean, _
            isunit As Boolean, optype As Integer, x As complex(,), i2 As Integer, j2 As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel
        '
        '          -- ALGLIB routine --
        '             16.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixrighttrsmmkl(m As Integer, n As Integer, a As complex(,), i1 As Integer, j1 As Integer, isupper As Boolean, _
            isunit As Boolean, optype As Integer, x As complex(,), i2 As Integer, j2 As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel
        '
        '          -- ALGLIB routine --
        '             16.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixlefttrsmmkl(m As Integer, n As Integer, a As Double(,), i1 As Integer, j1 As Integer, isupper As Boolean, _
            isunit As Boolean, optype As Integer, x As Double(,), i2 As Integer, j2 As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel
        '
        '          -- ALGLIB routine --
        '             16.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixrighttrsmmkl(m As Integer, n As Integer, a As Double(,), i1 As Integer, j1 As Integer, isupper As Boolean, _
            isunit As Boolean, optype As Integer, x As Double(,), i2 As Integer, j2 As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        NOTE:
        '
        '        if function returned False, CholResult is NOT modified. Not ever referenced!
        '        if function returned True, CholResult is set to status of Cholesky decomposition
        '        (True on succeess).
        '
        '          -- ALGLIB routine --
        '             16.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function spdmatrixcholeskymkl(a As Double(,), offs As Integer, n As Integer, isupper As Boolean, ByRef cholresult As Boolean) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixplumkl(ByRef a As Double(,), offs As Integer, m As Integer, n As Integer, ByRef pivots As Integer()) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        NOTE: this function needs preallocated output/temporary arrays.
        '              D and E must be at least max(M,N)-wide.
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixbdmkl(a As Double(,), m As Integer, n As Integer, d As Double(), e As Double(), tauq As Double(), _
            taup As Double()) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        If ByQ is True,  TauP is not used (can be empty array).
        '        If ByQ is False, TauQ is not used (can be empty array).
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixbdmultiplybymkl(qp As Double(,), m As Integer, n As Integer, tauq As Double(), taup As Double(), z As Double(,), _
            zrows As Integer, zcolumns As Integer, byq As Boolean, fromtheright As Boolean, dotranspose As Boolean) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        NOTE: Tau must be preallocated array with at least N-1 elements.
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixhessenbergmkl(a As Double(,), n As Integer, tau As Double()) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        NOTE: Q must be preallocated N*N array
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixhessenbergunpackqmkl(a As Double(,), n As Integer, tau As Double(), q As Double(,)) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        NOTE: Tau, D, E must be preallocated arrays;
        '              length(E)=length(Tau)=N-1 (or larger)
        '              length(D)=N (or larger)
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function smatrixtdmkl(a As Double(,), n As Integer, isupper As Boolean, tau As Double(), d As Double(), e As Double()) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        NOTE: Q must be preallocated N*N array
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function smatrixtdunpackqmkl(a As Double(,), n As Integer, isupper As Boolean, tau As Double(), q As Double(,)) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        NOTE: Tau, D, E must be preallocated arrays;
        '              length(E)=length(Tau)=N-1 (or larger)
        '              length(D)=N (or larger)
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function hmatrixtdmkl(a As complex(,), n As Integer, isupper As Boolean, tau As complex(), d As Double(), e As Double()) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        NOTE: Q must be preallocated N*N array
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function hmatrixtdunpackqmkl(a As complex(,), n As Integer, isupper As Boolean, tau As complex(), q As complex(,)) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        Returns True if MKL was present and handled request (MKL  completion  code
        '        is returned as separate output parameter).
        '
        '        D and E are pre-allocated arrays with length N (both of them!). On output,
        '        D constraints singular values, and E is destroyed.
        '
        '        SVDResult is modified if and only if MKL is present.
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixbdsvdmkl(d As Double(), e As Double(), n As Integer, isupper As Boolean, u As Double(,), nru As Integer, _
            c As Double(,), ncc As Integer, vt As Double(,), ncvt As Integer, ByRef svdresult As Boolean) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based DHSEQR kernel.
        '
        '        Returns True if MKL was present and handled request.
        '
        '        WR and WI are pre-allocated arrays with length N.
        '        Z is pre-allocated array[N,N].
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixinternalschurdecompositionmkl(h As Double(,), n As Integer, tneeded As Integer, zneeded As Integer, wr As Double(), wi As Double(), _
            z As Double(,), ByRef info As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based DTREVC kernel.
        '
        '        Returns True if MKL was present and handled request.
        '
        '        NOTE: this function does NOT support HOWMNY=3!!!!
        '
        '        VL and VR are pre-allocated arrays with length N*N, if required. If particalar
        '        variables is not required, it can be dummy (empty) array.
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixinternaltrevcmkl(t As Double(,), n As Integer, side As Integer, howmny As Integer, vl As Double(,), vr As Double(,), _
            ByRef m As Integer, ByRef info As Integer) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        MKL-based kernel.
        '
        '        Returns True if MKL was present and handled request (MKL  completion  code
        '        is returned as separate output parameter).
        '
        '        D and E are pre-allocated arrays with length N (both of them!). On output,
        '        D constraints eigenvalues, and E is destroyed.
        '
        '        Z is preallocated array[N,N] for ZNeeded<>0; ignored for ZNeeded=0.
        '
        '        EVDResult is modified if and only if MKL is present.
        '
        '          -- ALGLIB routine --
        '             20.10.2014
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function smatrixtdevdmkl(d As Double(), e As Double(), n As Integer, zneeded As Integer, z As Double(,), ByRef evdresult As Boolean) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


    End Class
    Public Class blas
        Public Shared Function vectornorm2(x As Double(), i1 As Integer, i2 As Integer) As Double
            Dim result As Double = 0
            Dim n As Integer = 0
            Dim ix As Integer = 0
            Dim absxi As Double = 0
            Dim scl As Double = 0
            Dim ssq As Double = 0

            n = i2 - i1 + 1
            If n < 1 Then
                result = 0
                Return result
            End If
            If n = 1 Then
                result = System.Math.Abs(x(i1))
                Return result
            End If
            scl = 0
            ssq = 1
            For ix = i1 To i2
                If CDbl(x(ix)) <> CDbl(0) Then
                    absxi = System.Math.Abs(x(ix))
                    If CDbl(scl) < CDbl(absxi) Then
                        ssq = 1 + ssq * Math.sqr(scl / absxi)
                        scl = absxi
                    Else
                        ssq = ssq + Math.sqr(absxi / scl)
                    End If
                End If
            Next
            result = scl * System.Math.sqrt(ssq)
            Return result
        End Function


        Public Shared Function vectoridxabsmax(x As Double(), i1 As Integer, i2 As Integer) As Integer
            Dim result As Integer = 0
            Dim i As Integer = 0

            result = i1
            For i = i1 + 1 To i2
                If CDbl(System.Math.Abs(x(i))) > CDbl(System.Math.Abs(x(result))) Then
                    result = i
                End If
            Next
            Return result
        End Function


        Public Shared Function columnidxabsmax(x As Double(,), i1 As Integer, i2 As Integer, j As Integer) As Integer
            Dim result As Integer = 0
            Dim i As Integer = 0

            result = i1
            For i = i1 + 1 To i2
                If CDbl(System.Math.Abs(x(i, j))) > CDbl(System.Math.Abs(x(result, j))) Then
                    result = i
                End If
            Next
            Return result
        End Function


        Public Shared Function rowidxabsmax(x As Double(,), j1 As Integer, j2 As Integer, i As Integer) As Integer
            Dim result As Integer = 0
            Dim j As Integer = 0

            result = j1
            For j = j1 + 1 To j2
                If CDbl(System.Math.Abs(x(i, j))) > CDbl(System.Math.Abs(x(i, result))) Then
                    result = j
                End If
            Next
            Return result
        End Function


        Public Shared Function upperhessenberg1norm(a As Double(,), i1 As Integer, i2 As Integer, j1 As Integer, j2 As Integer, ByRef work As Double()) As Double
            Dim result As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            alglib.ap.assert(i2 - i1 = j2 - j1, "UpperHessenberg1Norm: I2-I1<>J2-J1!")
            For j = j1 To j2
                work(j) = 0
            Next
            For i = i1 To i2
                For j = System.Math.Max(j1, j1 + i - i1 - 1) To j2
                    work(j) = work(j) + System.Math.Abs(a(i, j))
                Next
            Next
            result = 0
            For j = j1 To j2
                result = System.Math.Max(result, work(j))
            Next
            Return result
        End Function


        Public Shared Sub copymatrix(a As Double(,), is1 As Integer, is2 As Integer, js1 As Integer, js2 As Integer, ByRef b As Double(,), _
            id1 As Integer, id2 As Integer, jd1 As Integer, jd2 As Integer)
            Dim isrc As Integer = 0
            Dim idst As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If is1 > is2 OrElse js1 > js2 Then
                Return
            End If
            alglib.ap.assert(is2 - is1 = id2 - id1, "CopyMatrix: different sizes!")
            alglib.ap.assert(js2 - js1 = jd2 - jd1, "CopyMatrix: different sizes!")
            For isrc = is1 To is2
                idst = isrc - is1 + id1
                i1_ = (js1) - (jd1)
                For i_ = jd1 To jd2
                    b(idst, i_) = a(isrc, i_ + i1_)
                Next
            Next
        End Sub


        Public Shared Sub inplacetranspose(ByRef a As Double(,), i1 As Integer, i2 As Integer, j1 As Integer, j2 As Integer, ByRef work As Double())
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim ips As Integer = 0
            Dim jps As Integer = 0
            Dim l As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If i1 > i2 OrElse j1 > j2 Then
                Return
            End If
            alglib.ap.assert(i1 - i2 = j1 - j2, "InplaceTranspose error: incorrect array size!")
            For i = i1 To i2 - 1
                j = j1 + i - i1
                ips = i + 1
                jps = j1 + ips - i1
                l = i2 - i
                i1_ = (ips) - (1)
                For i_ = 1 To l
                    work(i_) = a(i_ + i1_, j)
                Next
                i1_ = (jps) - (ips)
                For i_ = ips To i2
                    a(i_, j) = a(i, i_ + i1_)
                Next
                i1_ = (1) - (jps)
                For i_ = jps To j2
                    a(i, i_) = work(i_ + i1_)
                Next
            Next
        End Sub


        Public Shared Sub copyandtranspose(a As Double(,), is1 As Integer, is2 As Integer, js1 As Integer, js2 As Integer, ByRef b As Double(,), _
            id1 As Integer, id2 As Integer, jd1 As Integer, jd2 As Integer)
            Dim isrc As Integer = 0
            Dim jdst As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If is1 > is2 OrElse js1 > js2 Then
                Return
            End If
            alglib.ap.assert(is2 - is1 = jd2 - jd1, "CopyAndTranspose: different sizes!")
            alglib.ap.assert(js2 - js1 = id2 - id1, "CopyAndTranspose: different sizes!")
            For isrc = is1 To is2
                jdst = isrc - is1 + jd1
                i1_ = (js1) - (id1)
                For i_ = id1 To id2
                    b(i_, jdst) = a(isrc, i_ + i1_)
                Next
            Next
        End Sub


        Public Shared Sub matrixvectormultiply(a As Double(,), i1 As Integer, i2 As Integer, j1 As Integer, j2 As Integer, trans As Boolean, _
            x As Double(), ix1 As Integer, ix2 As Integer, alpha As Double, ByRef y As Double(), iy1 As Integer, _
            iy2 As Integer, beta As Double)
            Dim i As Integer = 0
            Dim v As Double = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If Not trans Then

                '
                ' y := alpha*A*x + beta*y;
                '
                If i1 > i2 OrElse j1 > j2 Then
                    Return
                End If
                alglib.ap.assert(j2 - j1 = ix2 - ix1, "MatrixVectorMultiply: A and X dont match!")
                alglib.ap.assert(i2 - i1 = iy2 - iy1, "MatrixVectorMultiply: A and Y dont match!")

                '
                ' beta*y
                '
                If CDbl(beta) = CDbl(0) Then
                    For i = iy1 To iy2
                        y(i) = 0
                    Next
                Else
                    For i_ = iy1 To iy2
                        y(i_) = beta * y(i_)
                    Next
                End If

                '
                ' alpha*A*x
                '
                For i = i1 To i2
                    i1_ = (ix1) - (j1)
                    v = 0.0
                    For i_ = j1 To j2
                        v += a(i, i_) * x(i_ + i1_)
                    Next
                    y(iy1 + i - i1) = y(iy1 + i - i1) + alpha * v
                Next
            Else

                '
                ' y := alpha*A'*x + beta*y;
                '
                If i1 > i2 OrElse j1 > j2 Then
                    Return
                End If
                alglib.ap.assert(i2 - i1 = ix2 - ix1, "MatrixVectorMultiply: A and X dont match!")
                alglib.ap.assert(j2 - j1 = iy2 - iy1, "MatrixVectorMultiply: A and Y dont match!")

                '
                ' beta*y
                '
                If CDbl(beta) = CDbl(0) Then
                    For i = iy1 To iy2
                        y(i) = 0
                    Next
                Else
                    For i_ = iy1 To iy2
                        y(i_) = beta * y(i_)
                    Next
                End If

                '
                ' alpha*A'*x
                '
                For i = i1 To i2
                    v = alpha * x(ix1 + i - i1)
                    i1_ = (j1) - (iy1)
                    For i_ = iy1 To iy2
                        y(i_) = y(i_) + v * a(i, i_ + i1_)
                    Next
                Next
            End If
        End Sub


        Public Shared Function pythag2(x As Double, y As Double) As Double
            Dim result As Double = 0
            Dim w As Double = 0
            Dim xabs As Double = 0
            Dim yabs As Double = 0
            Dim z As Double = 0

            xabs = System.Math.Abs(x)
            yabs = System.Math.Abs(y)
            w = System.Math.Max(xabs, yabs)
            z = System.Math.Min(xabs, yabs)
            If CDbl(z) = CDbl(0) Then
                result = w
            Else
                result = w * System.Math.sqrt(1 + Math.sqr(z / w))
            End If
            Return result
        End Function


        Public Shared Sub matrixmatrixmultiply(a As Double(,), ai1 As Integer, ai2 As Integer, aj1 As Integer, aj2 As Integer, transa As Boolean, _
            b As Double(,), bi1 As Integer, bi2 As Integer, bj1 As Integer, bj2 As Integer, transb As Boolean, _
            alpha As Double, ByRef c As Double(,), ci1 As Integer, ci2 As Integer, cj1 As Integer, cj2 As Integer, _
            beta As Double, ByRef work As Double())
            Dim arows As Integer = 0
            Dim acols As Integer = 0
            Dim brows As Integer = 0
            Dim bcols As Integer = 0
            Dim crows As Integer = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim l As Integer = 0
            Dim r As Integer = 0
            Dim v As Double = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0


            '
            ' Setup
            '
            If Not transa Then
                arows = ai2 - ai1 + 1
                acols = aj2 - aj1 + 1
            Else
                arows = aj2 - aj1 + 1
                acols = ai2 - ai1 + 1
            End If
            If Not transb Then
                brows = bi2 - bi1 + 1
                bcols = bj2 - bj1 + 1
            Else
                brows = bj2 - bj1 + 1
                bcols = bi2 - bi1 + 1
            End If
            alglib.ap.assert(acols = brows, "MatrixMatrixMultiply: incorrect matrix sizes!")
            If ((arows <= 0 OrElse acols <= 0) OrElse brows <= 0) OrElse bcols <= 0 Then
                Return
            End If
            crows = arows

            '
            ' Test WORK
            '
            i = System.Math.Max(arows, acols)
            i = System.Math.Max(brows, i)
            i = System.Math.Max(i, bcols)
            work(1) = 0
            work(i) = 0

            '
            ' Prepare C
            '
            If CDbl(beta) = CDbl(0) Then
                For i = ci1 To ci2
                    For j = cj1 To cj2
                        c(i, j) = 0
                    Next
                Next
            Else
                For i = ci1 To ci2
                    For i_ = cj1 To cj2
                        c(i, i_) = beta * c(i, i_)
                    Next
                Next
            End If

            '
            ' A*B
            '
            If Not transa AndAlso Not transb Then
                For l = ai1 To ai2
                    For r = bi1 To bi2
                        v = alpha * a(l, aj1 + r - bi1)
                        k = ci1 + l - ai1
                        i1_ = (bj1) - (cj1)
                        For i_ = cj1 To cj2
                            c(k, i_) = c(k, i_) + v * b(r, i_ + i1_)
                        Next
                    Next
                Next
                Return
            End If

            '
            ' A*B'
            '
            If Not transa AndAlso transb Then
                If arows * acols < brows * bcols Then
                    For r = bi1 To bi2
                        For l = ai1 To ai2
                            i1_ = (bj1) - (aj1)
                            v = 0.0
                            For i_ = aj1 To aj2
                                v += a(l, i_) * b(r, i_ + i1_)
                            Next
                            c(ci1 + l - ai1, cj1 + r - bi1) = c(ci1 + l - ai1, cj1 + r - bi1) + alpha * v
                        Next
                    Next
                    Return
                Else
                    For l = ai1 To ai2
                        For r = bi1 To bi2
                            i1_ = (bj1) - (aj1)
                            v = 0.0
                            For i_ = aj1 To aj2
                                v += a(l, i_) * b(r, i_ + i1_)
                            Next
                            c(ci1 + l - ai1, cj1 + r - bi1) = c(ci1 + l - ai1, cj1 + r - bi1) + alpha * v
                        Next
                    Next
                    Return
                End If
            End If

            '
            ' A'*B
            '
            If transa AndAlso Not transb Then
                For l = aj1 To aj2
                    For r = bi1 To bi2
                        v = alpha * a(ai1 + r - bi1, l)
                        k = ci1 + l - aj1
                        i1_ = (bj1) - (cj1)
                        For i_ = cj1 To cj2
                            c(k, i_) = c(k, i_) + v * b(r, i_ + i1_)
                        Next
                    Next
                Next
                Return
            End If

            '
            ' A'*B'
            '
            If transa AndAlso transb Then
                If arows * acols < brows * bcols Then
                    For r = bi1 To bi2
                        k = cj1 + r - bi1
                        For i = 1 To crows
                            work(i) = 0.0
                        Next
                        For l = ai1 To ai2
                            v = alpha * b(r, bj1 + l - ai1)
                            i1_ = (aj1) - (1)
                            For i_ = 1 To crows
                                work(i_) = work(i_) + v * a(l, i_ + i1_)
                            Next
                        Next
                        i1_ = (1) - (ci1)
                        For i_ = ci1 To ci2
                            c(i_, k) = c(i_, k) + work(i_ + i1_)
                        Next
                    Next
                    Return
                Else
                    For l = aj1 To aj2
                        k = ai2 - ai1 + 1
                        i1_ = (ai1) - (1)
                        For i_ = 1 To k
                            work(i_) = a(i_ + i1_, l)
                        Next
                        For r = bi1 To bi2
                            i1_ = (bj1) - (1)
                            v = 0.0
                            For i_ = 1 To k
                                v += work(i_) * b(r, i_ + i1_)
                            Next
                            c(ci1 + l - aj1, cj1 + r - bi1) = c(ci1 + l - aj1, cj1 + r - bi1) + alpha * v
                        Next
                    Next
                    Return
                End If
            End If
        End Sub


    End Class
    Public Class hblas
        Public Shared Sub hermitianmatrixvectormultiply(a As complex(,), isupper As Boolean, i1 As Integer, i2 As Integer, x As complex(), alpha As complex, _
            ByRef y As complex())
            Dim i As Integer = 0
            Dim ba1 As Integer = 0
            Dim by1 As Integer = 0
            Dim by2 As Integer = 0
            Dim bx1 As Integer = 0
            Dim bx2 As Integer = 0
            Dim n As Integer = 0
            Dim v As complex = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            n = i2 - i1 + 1
            If n <= 0 Then
                Return
            End If

            '
            ' Let A = L + D + U, where
            '  L is strictly lower triangular (main diagonal is zero)
            '  D is diagonal
            '  U is strictly upper triangular (main diagonal is zero)
            '
            ' A*x = L*x + D*x + U*x
            '
            ' Calculate D*x first
            '
            For i = i1 To i2
                y(i - i1 + 1) = a(i, i) * x(i - i1 + 1)
            Next

            '
            ' Add L*x + U*x
            '
            If isupper Then
                For i = i1 To i2 - 1

                    '
                    ' Add L*x to the result
                    '
                    v = x(i - i1 + 1)
                    by1 = i - i1 + 2
                    by2 = n
                    ba1 = i + 1
                    i1_ = (ba1) - (by1)
                    For i_ = by1 To by2
                        y(i_) = y(i_) + v * Math.conj(a(i, i_ + i1_))
                    Next

                    '
                    ' Add U*x to the result
                    '
                    bx1 = i - i1 + 2
                    bx2 = n
                    ba1 = i + 1
                    i1_ = (ba1) - (bx1)
                    v = 0.0
                    For i_ = bx1 To bx2
                        v += x(i_) * a(i, i_ + i1_)
                    Next
                    y(i - i1 + 1) = y(i - i1 + 1) + v
                Next
            Else
                For i = i1 + 1 To i2

                    '
                    ' Add L*x to the result
                    '
                    bx1 = 1
                    bx2 = i - i1
                    ba1 = i1
                    i1_ = (ba1) - (bx1)
                    v = 0.0
                    For i_ = bx1 To bx2
                        v += x(i_) * a(i, i_ + i1_)
                    Next
                    y(i - i1 + 1) = y(i - i1 + 1) + v

                    '
                    ' Add U*x to the result
                    '
                    v = x(i - i1 + 1)
                    by1 = 1
                    by2 = i - i1
                    ba1 = i1
                    i1_ = (ba1) - (by1)
                    For i_ = by1 To by2
                        y(i_) = y(i_) + v * Math.conj(a(i, i_ + i1_))
                    Next
                Next
            End If
            For i_ = 1 To n
                y(i_) = alpha * y(i_)
            Next
        End Sub


        Public Shared Sub hermitianrank2update(ByRef a As complex(,), isupper As Boolean, i1 As Integer, i2 As Integer, x As complex(), y As complex(), _
            ByRef t As complex(), alpha As complex)
            Dim i As Integer = 0
            Dim tp1 As Integer = 0
            Dim tp2 As Integer = 0
            Dim v As complex = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If isupper Then
                For i = i1 To i2
                    tp1 = i + 1 - i1
                    tp2 = i2 - i1 + 1
                    v = alpha * x(i + 1 - i1)
                    For i_ = tp1 To tp2
                        t(i_) = v * Math.conj(y(i_))
                    Next
                    v = Math.conj(alpha) * y(i + 1 - i1)
                    For i_ = tp1 To tp2
                        t(i_) = t(i_) + v * Math.conj(x(i_))
                    Next
                    i1_ = (tp1) - (i)
                    For i_ = i To i2
                        a(i, i_) = a(i, i_) + t(i_ + i1_)
                    Next
                Next
            Else
                For i = i1 To i2
                    tp1 = 1
                    tp2 = i + 1 - i1
                    v = alpha * x(i + 1 - i1)
                    For i_ = tp1 To tp2
                        t(i_) = v * Math.conj(y(i_))
                    Next
                    v = Math.conj(alpha) * y(i + 1 - i1)
                    For i_ = tp1 To tp2
                        t(i_) = t(i_) + v * Math.conj(x(i_))
                    Next
                    i1_ = (tp1) - (i1)
                    For i_ = i1 To i
                        a(i, i_) = a(i, i_) + t(i_ + i1_)
                    Next
                Next
            End If
        End Sub


    End Class
    Public Class reflections
        '************************************************************************
        '        Generation of an elementary reflection transformation
        '
        '        The subroutine generates elementary reflection H of order N, so that, for
        '        a given X, the following equality holds true:
        '
        '            ( X(1) )   ( Beta )
        '        H * (  ..  ) = (  0   )
        '            ( X(n) )   (  0   )
        '
        '        where
        '                      ( V(1) )
        '        H = 1 - Tau * (  ..  ) * ( V(1), ..., V(n) )
        '                      ( V(n) )
        '
        '        where the first component of vector V equals 1.
        '
        '        Input parameters:
        '            X   -   vector. Array whose index ranges within [1..N].
        '            N   -   reflection order.
        '
        '        Output parameters:
        '            X   -   components from 2 to N are replaced with vector V.
        '                    The first component is replaced with parameter Beta.
        '            Tau -   scalar value Tau. If X is a null vector, Tau equals 0,
        '                    otherwise 1 <= Tau <= 2.
        '
        '        This subroutine is the modification of the DLARFG subroutines from
        '        the LAPACK library.
        '
        '        MODIFICATIONS:
        '            24.12.2005 sign(Alpha) was replaced with an analogous to the Fortran SIGN code.
        '
        '          -- LAPACK auxiliary routine (version 3.0) --
        '             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
        '             Courant Institute, Argonne National Lab, and Rice University
        '             September 30, 1994
        '        ************************************************************************

        Public Shared Sub generatereflection(ByRef x As Double(), n As Integer, ByRef tau As Double)
            Dim j As Integer = 0
            Dim alpha As Double = 0
            Dim xnorm As Double = 0
            Dim v As Double = 0
            Dim beta As Double = 0
            Dim mx As Double = 0
            Dim s As Double = 0
            Dim i_ As Integer = 0

            tau = 0

            If n <= 1 Then
                tau = 0
                Return
            End If

            '
            ' Scale if needed (to avoid overflow/underflow during intermediate
            ' calculations).
            '
            mx = 0
            For j = 1 To n
                mx = System.Math.Max(System.Math.Abs(x(j)), mx)
            Next
            s = 1
            If CDbl(mx) <> CDbl(0) Then
                If CDbl(mx) <= CDbl(Math.minrealnumber / Math.machineepsilon) Then
                    s = Math.minrealnumber / Math.machineepsilon
                    v = 1 / s
                    For i_ = 1 To n
                        x(i_) = v * x(i_)
                    Next
                    mx = mx * v
                Else
                    If CDbl(mx) >= CDbl(Math.maxrealnumber * Math.machineepsilon) Then
                        s = Math.maxrealnumber * Math.machineepsilon
                        v = 1 / s
                        For i_ = 1 To n
                            x(i_) = v * x(i_)
                        Next
                        mx = mx * v
                    End If
                End If
            End If

            '
            ' XNORM = DNRM2( N-1, X, INCX )
            '
            alpha = x(1)
            xnorm = 0
            If CDbl(mx) <> CDbl(0) Then
                For j = 2 To n
                    xnorm = xnorm + Math.sqr(x(j) / mx)
                Next
                xnorm = System.Math.sqrt(xnorm) * mx
            End If
            If CDbl(xnorm) = CDbl(0) Then

                '
                ' H  =  I
                '
                tau = 0
                x(1) = x(1) * s
                Return
            End If

            '
            ' general case
            '
            mx = System.Math.Max(System.Math.Abs(alpha), System.Math.Abs(xnorm))
            beta = -(mx * System.Math.sqrt(Math.sqr(alpha / mx) + Math.sqr(xnorm / mx)))
            If CDbl(alpha) < CDbl(0) Then
                beta = -beta
            End If
            tau = (beta - alpha) / beta
            v = 1 / (alpha - beta)
            For i_ = 2 To n
                x(i_) = v * x(i_)
            Next
            x(1) = beta

            '
            ' Scale back outputs
            '
            x(1) = x(1) * s
        End Sub


        '************************************************************************
        '        Application of an elementary reflection to a rectangular matrix of size MxN
        '
        '        The algorithm pre-multiplies the matrix by an elementary reflection transformation
        '        which is given by column V and scalar Tau (see the description of the
        '        GenerateReflection procedure). Not the whole matrix but only a part of it
        '        is transformed (rows from M1 to M2, columns from N1 to N2). Only the elements
        '        of this submatrix are changed.
        '
        '        Input parameters:
        '            C       -   matrix to be transformed.
        '            Tau     -   scalar defining the transformation.
        '            V       -   column defining the transformation.
        '                        Array whose index ranges within [1..M2-M1+1].
        '            M1, M2  -   range of rows to be transformed.
        '            N1, N2  -   range of columns to be transformed.
        '            WORK    -   working array whose indexes goes from N1 to N2.
        '
        '        Output parameters:
        '            C       -   the result of multiplying the input matrix C by the
        '                        transformation matrix which is given by Tau and V.
        '                        If N1>N2 or M1>M2, C is not modified.
        '
        '          -- LAPACK auxiliary routine (version 3.0) --
        '             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
        '             Courant Institute, Argonne National Lab, and Rice University
        '             September 30, 1994
        '        ************************************************************************

        Public Shared Sub applyreflectionfromtheleft(ByRef c As Double(,), tau As Double, v As Double(), m1 As Integer, m2 As Integer, n1 As Integer, _
            n2 As Integer, ByRef work As Double())
            Dim t As Double = 0
            Dim i As Integer = 0
            Dim i_ As Integer = 0

            If (CDbl(tau) = CDbl(0) OrElse n1 > n2) OrElse m1 > m2 Then
                Return
            End If

            '
            ' w := C' * v
            '
            For i = n1 To n2
                work(i) = 0
            Next
            For i = m1 To m2
                t = v(i + 1 - m1)
                For i_ = n1 To n2
                    work(i_) = work(i_) + t * c(i, i_)
                Next
            Next

            '
            ' C := C - tau * v * w'
            '
            For i = m1 To m2
                t = v(i - m1 + 1) * tau
                For i_ = n1 To n2
                    c(i, i_) = c(i, i_) - t * work(i_)
                Next
            Next
        End Sub


        '************************************************************************
        '        Application of an elementary reflection to a rectangular matrix of size MxN
        '
        '        The algorithm post-multiplies the matrix by an elementary reflection transformation
        '        which is given by column V and scalar Tau (see the description of the
        '        GenerateReflection procedure). Not the whole matrix but only a part of it
        '        is transformed (rows from M1 to M2, columns from N1 to N2). Only the
        '        elements of this submatrix are changed.
        '
        '        Input parameters:
        '            C       -   matrix to be transformed.
        '            Tau     -   scalar defining the transformation.
        '            V       -   column defining the transformation.
        '                        Array whose index ranges within [1..N2-N1+1].
        '            M1, M2  -   range of rows to be transformed.
        '            N1, N2  -   range of columns to be transformed.
        '            WORK    -   working array whose indexes goes from M1 to M2.
        '
        '        Output parameters:
        '            C       -   the result of multiplying the input matrix C by the
        '                        transformation matrix which is given by Tau and V.
        '                        If N1>N2 or M1>M2, C is not modified.
        '
        '          -- LAPACK auxiliary routine (version 3.0) --
        '             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
        '             Courant Institute, Argonne National Lab, and Rice University
        '             September 30, 1994
        '        ************************************************************************

        Public Shared Sub applyreflectionfromtheright(ByRef c As Double(,), tau As Double, v As Double(), m1 As Integer, m2 As Integer, n1 As Integer, _
            n2 As Integer, ByRef work As Double())
            Dim t As Double = 0
            Dim i As Integer = 0
            Dim vm As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If (CDbl(tau) = CDbl(0) OrElse n1 > n2) OrElse m1 > m2 Then
                Return
            End If
            vm = n2 - n1 + 1
            For i = m1 To m2
                i1_ = (1) - (n1)
                t = 0.0
                For i_ = n1 To n2
                    t += c(i, i_) * v(i_ + i1_)
                Next
                t = t * tau
                i1_ = (1) - (n1)
                For i_ = n1 To n2
                    c(i, i_) = c(i, i_) - t * v(i_ + i1_)
                Next
            Next

            '
            ' This line is necessary to avoid spurious compiler warnings
            '
            apserv.touchint(vm)
        End Sub


    End Class
    Public Class creflections
        '************************************************************************
        '        Generation of an elementary complex reflection transformation
        '
        '        The subroutine generates elementary complex reflection H of  order  N,  so
        '        that, for a given X, the following equality holds true:
        '
        '             ( X(1) )   ( Beta )
        '        H' * (  ..  ) = (  0   ),   H'*H = I,   Beta is a real number
        '             ( X(n) )   (  0   )
        '
        '        where
        '
        '                      ( V(1) )
        '        H = 1 - Tau * (  ..  ) * ( conj(V(1)), ..., conj(V(n)) )
        '                      ( V(n) )
        '
        '        where the first component of vector V equals 1.
        '
        '        Input parameters:
        '            X   -   vector. Array with elements [1..N].
        '            N   -   reflection order.
        '
        '        Output parameters:
        '            X   -   components from 2 to N are replaced by vector V.
        '                    The first component is replaced with parameter Beta.
        '            Tau -   scalar value Tau.
        '
        '        This subroutine is the modification of CLARFG subroutines  from the LAPACK
        '        library. It has similar functionality except for the fact that it  doesn抰
        '        handle errors when intermediate results cause an overflow.
        '
        '          -- LAPACK auxiliary routine (version 3.0) --
        '             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
        '             Courant Institute, Argonne National Lab, and Rice University
        '             September 30, 1994
        '        ************************************************************************

        Public Shared Sub complexgeneratereflection(ByRef x As complex(), n As Integer, ByRef tau As complex)
            Dim j As Integer = 0
            Dim alpha As complex = 0
            Dim alphi As Double = 0
            Dim alphr As Double = 0
            Dim beta As Double = 0
            Dim xnorm As Double = 0
            Dim mx As Double = 0
            Dim t As complex = 0
            Dim s As Double = 0
            Dim v As complex = 0
            Dim i_ As Integer = 0

            tau = 0

            If n <= 0 Then
                tau = 0
                Return
            End If

            '
            ' Scale if needed (to avoid overflow/underflow during intermediate
            ' calculations).
            '
            mx = 0
            For j = 1 To n
                mx = System.Math.Max(Math.abscomplex(x(j)), mx)
            Next
            s = 1
            If CDbl(mx) <> CDbl(0) Then
                If CDbl(mx) < CDbl(1) Then
                    s = System.Math.sqrt(Math.minrealnumber)
                    v = 1 / s
                    For i_ = 1 To n
                        x(i_) = v * x(i_)
                    Next
                Else
                    s = System.Math.sqrt(Math.maxrealnumber)
                    v = 1 / s
                    For i_ = 1 To n
                        x(i_) = v * x(i_)
                    Next
                End If
            End If

            '
            ' calculate
            '
            alpha = x(1)
            mx = 0
            For j = 2 To n
                mx = System.Math.Max(Math.abscomplex(x(j)), mx)
            Next
            xnorm = 0
            If CDbl(mx) <> CDbl(0) Then
                For j = 2 To n
                    t = x(j) / mx
                    xnorm = xnorm + (t * Math.conj(t)).x
                Next
                xnorm = System.Math.sqrt(xnorm) * mx
            End If
            alphr = alpha.x
            alphi = alpha.y
            If CDbl(xnorm) = CDbl(0) AndAlso CDbl(alphi) = CDbl(0) Then
                tau = 0
                x(1) = x(1) * s
                Return
            End If
            mx = System.Math.Max(System.Math.Abs(alphr), System.Math.Abs(alphi))
            mx = System.Math.Max(mx, System.Math.Abs(xnorm))
            beta = -(mx * System.Math.sqrt(Math.sqr(alphr / mx) + Math.sqr(alphi / mx) + Math.sqr(xnorm / mx)))
            If CDbl(alphr) < CDbl(0) Then
                beta = -beta
            End If
            tau.x = (beta - alphr) / beta
            tau.y = -(alphi / beta)
            alpha = 1 / (alpha - beta)
            If n > 1 Then
                For i_ = 2 To n
                    x(i_) = alpha * x(i_)
                Next
            End If
            alpha = beta
            x(1) = alpha

            '
            ' Scale back
            '
            x(1) = x(1) * s
        End Sub


        '************************************************************************
        '        Application of an elementary reflection to a rectangular matrix of size MxN
        '
        '        The  algorithm  pre-multiplies  the  matrix  by  an  elementary reflection
        '        transformation  which  is  given  by  column  V  and  scalar  Tau (see the
        '        description of the GenerateReflection). Not the whole matrix  but  only  a
        '        part of it is transformed (rows from M1 to M2, columns from N1 to N2). Only
        '        the elements of this submatrix are changed.
        '
        '        Note: the matrix is multiplied by H, not by H'.   If  it  is  required  to
        '        multiply the matrix by H', it is necessary to pass Conj(Tau) instead of Tau.
        '
        '        Input parameters:
        '            C       -   matrix to be transformed.
        '            Tau     -   scalar defining transformation.
        '            V       -   column defining transformation.
        '                        Array whose index ranges within [1..M2-M1+1]
        '            M1, M2  -   range of rows to be transformed.
        '            N1, N2  -   range of columns to be transformed.
        '            WORK    -   working array whose index goes from N1 to N2.
        '
        '        Output parameters:
        '            C       -   the result of multiplying the input matrix C by the
        '                        transformation matrix which is given by Tau and V.
        '                        If N1>N2 or M1>M2, C is not modified.
        '
        '          -- LAPACK auxiliary routine (version 3.0) --
        '             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
        '             Courant Institute, Argonne National Lab, and Rice University
        '             September 30, 1994
        '        ************************************************************************

        Public Shared Sub complexapplyreflectionfromtheleft(ByRef c As complex(,), tau As complex, v As complex(), m1 As Integer, m2 As Integer, n1 As Integer, _
            n2 As Integer, ByRef work As complex())
            Dim t As complex = 0
            Dim i As Integer = 0
            Dim i_ As Integer = 0

            If (tau = 0 OrElse n1 > n2) OrElse m1 > m2 Then
                Return
            End If

            '
            ' w := C^T * conj(v)
            '
            For i = n1 To n2
                work(i) = 0
            Next
            For i = m1 To m2
                t = Math.conj(v(i + 1 - m1))
                For i_ = n1 To n2
                    work(i_) = work(i_) + t * c(i, i_)
                Next
            Next

            '
            ' C := C - tau * v * w^T
            '
            For i = m1 To m2
                t = v(i - m1 + 1) * tau
                For i_ = n1 To n2
                    c(i, i_) = c(i, i_) - t * work(i_)
                Next
            Next
        End Sub


        '************************************************************************
        '        Application of an elementary reflection to a rectangular matrix of size MxN
        '
        '        The  algorithm  post-multiplies  the  matrix  by  an elementary reflection
        '        transformation  which  is  given  by  column  V  and  scalar  Tau (see the
        '        description  of  the  GenerateReflection). Not the whole matrix but only a
        '        part  of  it  is  transformed (rows from M1 to M2, columns from N1 to N2).
        '        Only the elements of this submatrix are changed.
        '
        '        Input parameters:
        '            C       -   matrix to be transformed.
        '            Tau     -   scalar defining transformation.
        '            V       -   column defining transformation.
        '                        Array whose index ranges within [1..N2-N1+1]
        '            M1, M2  -   range of rows to be transformed.
        '            N1, N2  -   range of columns to be transformed.
        '            WORK    -   working array whose index goes from M1 to M2.
        '
        '        Output parameters:
        '            C       -   the result of multiplying the input matrix C by the
        '                        transformation matrix which is given by Tau and V.
        '                        If N1>N2 or M1>M2, C is not modified.
        '
        '          -- LAPACK auxiliary routine (version 3.0) --
        '             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
        '             Courant Institute, Argonne National Lab, and Rice University
        '             September 30, 1994
        '        ************************************************************************

        Public Shared Sub complexapplyreflectionfromtheright(ByRef c As complex(,), tau As complex, ByRef v As complex(), m1 As Integer, m2 As Integer, n1 As Integer, _
            n2 As Integer, ByRef work As complex())
            Dim t As complex = 0
            Dim i As Integer = 0
            Dim vm As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If (tau = 0 OrElse n1 > n2) OrElse m1 > m2 Then
                Return
            End If

            '
            ' w := C * v
            '
            vm = n2 - n1 + 1
            For i = m1 To m2
                i1_ = (1) - (n1)
                t = 0.0
                For i_ = n1 To n2
                    t += c(i, i_) * v(i_ + i1_)
                Next
                work(i) = t
            Next

            '
            ' C := C - w * conj(v^T)
            '
            For i_ = 1 To vm
                v(i_) = Math.conj(v(i_))
            Next
            For i = m1 To m2
                t = work(i) * tau
                i1_ = (1) - (n1)
                For i_ = n1 To n2
                    c(i, i_) = c(i, i_) - t * v(i_ + i1_)
                Next
            Next
            For i_ = 1 To vm
                v(i_) = Math.conj(v(i_))
            Next
        End Sub


    End Class
    Public Class sblas
        Public Shared Sub symmetricmatrixvectormultiply(a As Double(,), isupper As Boolean, i1 As Integer, i2 As Integer, x As Double(), alpha As Double, _
            ByRef y As Double())
            Dim i As Integer = 0
            Dim ba1 As Integer = 0
            Dim ba2 As Integer = 0
            Dim by1 As Integer = 0
            Dim by2 As Integer = 0
            Dim bx1 As Integer = 0
            Dim bx2 As Integer = 0
            Dim n As Integer = 0
            Dim v As Double = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            n = i2 - i1 + 1
            If n <= 0 Then
                Return
            End If

            '
            ' Let A = L + D + U, where
            '  L is strictly lower triangular (main diagonal is zero)
            '  D is diagonal
            '  U is strictly upper triangular (main diagonal is zero)
            '
            ' A*x = L*x + D*x + U*x
            '
            ' Calculate D*x first
            '
            For i = i1 To i2
                y(i - i1 + 1) = a(i, i) * x(i - i1 + 1)
            Next

            '
            ' Add L*x + U*x
            '
            If isupper Then
                For i = i1 To i2 - 1

                    '
                    ' Add L*x to the result
                    '
                    v = x(i - i1 + 1)
                    by1 = i - i1 + 2
                    by2 = n
                    ba1 = i + 1
                    ba2 = i2
                    i1_ = (ba1) - (by1)
                    For i_ = by1 To by2
                        y(i_) = y(i_) + v * a(i, i_ + i1_)
                    Next

                    '
                    ' Add U*x to the result
                    '
                    bx1 = i - i1 + 2
                    bx2 = n
                    ba1 = i + 1
                    ba2 = i2
                    i1_ = (ba1) - (bx1)
                    v = 0.0
                    For i_ = bx1 To bx2
                        v += x(i_) * a(i, i_ + i1_)
                    Next
                    y(i - i1 + 1) = y(i - i1 + 1) + v
                Next
            Else
                For i = i1 + 1 To i2

                    '
                    ' Add L*x to the result
                    '
                    bx1 = 1
                    bx2 = i - i1
                    ba1 = i1
                    ba2 = i - 1
                    i1_ = (ba1) - (bx1)
                    v = 0.0
                    For i_ = bx1 To bx2
                        v += x(i_) * a(i, i_ + i1_)
                    Next
                    y(i - i1 + 1) = y(i - i1 + 1) + v

                    '
                    ' Add U*x to the result
                    '
                    v = x(i - i1 + 1)
                    by1 = 1
                    by2 = i - i1
                    ba1 = i1
                    ba2 = i - 1
                    i1_ = (ba1) - (by1)
                    For i_ = by1 To by2
                        y(i_) = y(i_) + v * a(i, i_ + i1_)
                    Next
                Next
            End If
            For i_ = 1 To n
                y(i_) = alpha * y(i_)
            Next
            apserv.touchint(ba2)
        End Sub


        Public Shared Sub symmetricrank2update(ByRef a As Double(,), isupper As Boolean, i1 As Integer, i2 As Integer, x As Double(), y As Double(), _
            ByRef t As Double(), alpha As Double)
            Dim i As Integer = 0
            Dim tp1 As Integer = 0
            Dim tp2 As Integer = 0
            Dim v As Double = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If isupper Then
                For i = i1 To i2
                    tp1 = i + 1 - i1
                    tp2 = i2 - i1 + 1
                    v = x(i + 1 - i1)
                    For i_ = tp1 To tp2
                        t(i_) = v * y(i_)
                    Next
                    v = y(i + 1 - i1)
                    For i_ = tp1 To tp2
                        t(i_) = t(i_) + v * x(i_)
                    Next
                    For i_ = tp1 To tp2
                        t(i_) = alpha * t(i_)
                    Next
                    i1_ = (tp1) - (i)
                    For i_ = i To i2
                        a(i, i_) = a(i, i_) + t(i_ + i1_)
                    Next
                Next
            Else
                For i = i1 To i2
                    tp1 = 1
                    tp2 = i + 1 - i1
                    v = x(i + 1 - i1)
                    For i_ = tp1 To tp2
                        t(i_) = v * y(i_)
                    Next
                    v = y(i + 1 - i1)
                    For i_ = tp1 To tp2
                        t(i_) = t(i_) + v * x(i_)
                    Next
                    For i_ = tp1 To tp2
                        t(i_) = alpha * t(i_)
                    Next
                    i1_ = (tp1) - (i1)
                    For i_ = i1 To i
                        a(i, i_) = a(i, i_) + t(i_ + i1_)
                    Next
                Next
            End If
        End Sub


    End Class
    Public Class rotations
        '************************************************************************
        '        Application of a sequence of  elementary rotations to a matrix
        '
        '        The algorithm pre-multiplies the matrix by a sequence of rotation
        '        transformations which is given by arrays C and S. Depending on the value
        '        of the IsForward parameter either 1 and 2, 3 and 4 and so on (if IsForward=true)
        '        rows are rotated, or the rows N and N-1, N-2 and N-3 and so on, are rotated.
        '
        '        Not the whole matrix but only a part of it is transformed (rows from M1 to
        '        M2, columns from N1 to N2). Only the elements of this submatrix are changed.
        '
        '        Input parameters:
        '            IsForward   -   the sequence of the rotation application.
        '            M1,M2       -   the range of rows to be transformed.
        '            N1, N2      -   the range of columns to be transformed.
        '            C,S         -   transformation coefficients.
        '                            Array whose index ranges within [1..M2-M1].
        '            A           -   processed matrix.
        '            WORK        -   working array whose index ranges within [N1..N2].
        '
        '        Output parameters:
        '            A           -   transformed matrix.
        '
        '        Utility subroutine.
        '        ************************************************************************

        Public Shared Sub applyrotationsfromtheleft(isforward As Boolean, m1 As Integer, m2 As Integer, n1 As Integer, n2 As Integer, c As Double(), _
            s As Double(), a As Double(,), work As Double())
            Dim j As Integer = 0
            Dim jp1 As Integer = 0
            Dim ctemp As Double = 0
            Dim stemp As Double = 0
            Dim temp As Double = 0
            Dim i_ As Integer = 0

            If m1 > m2 OrElse n1 > n2 Then
                Return
            End If

            '
            ' Form  P * A
            '
            If isforward Then
                If n1 <> n2 Then

                    '
                    ' Common case: N1<>N2
                    '
                    For j = m1 To m2 - 1
                        ctemp = c(j - m1 + 1)
                        stemp = s(j - m1 + 1)
                        If CDbl(ctemp) <> CDbl(1) OrElse CDbl(stemp) <> CDbl(0) Then
                            jp1 = j + 1
                            For i_ = n1 To n2
                                work(i_) = ctemp * a(jp1, i_)
                            Next
                            For i_ = n1 To n2
                                work(i_) = work(i_) - stemp * a(j, i_)
                            Next
                            For i_ = n1 To n2
                                a(j, i_) = ctemp * a(j, i_)
                            Next
                            For i_ = n1 To n2
                                a(j, i_) = a(j, i_) + stemp * a(jp1, i_)
                            Next
                            For i_ = n1 To n2
                                a(jp1, i_) = work(i_)
                            Next
                        End If
                    Next
                Else

                    '
                    ' Special case: N1=N2
                    '
                    For j = m1 To m2 - 1
                        ctemp = c(j - m1 + 1)
                        stemp = s(j - m1 + 1)
                        If CDbl(ctemp) <> CDbl(1) OrElse CDbl(stemp) <> CDbl(0) Then
                            temp = a(j + 1, n1)
                            a(j + 1, n1) = ctemp * temp - stemp * a(j, n1)
                            a(j, n1) = stemp * temp + ctemp * a(j, n1)
                        End If
                    Next
                End If
            Else
                If n1 <> n2 Then

                    '
                    ' Common case: N1<>N2
                    '
                    For j = m2 - 1 To m1 Step -1
                        ctemp = c(j - m1 + 1)
                        stemp = s(j - m1 + 1)
                        If CDbl(ctemp) <> CDbl(1) OrElse CDbl(stemp) <> CDbl(0) Then
                            jp1 = j + 1
                            For i_ = n1 To n2
                                work(i_) = ctemp * a(jp1, i_)
                            Next
                            For i_ = n1 To n2
                                work(i_) = work(i_) - stemp * a(j, i_)
                            Next
                            For i_ = n1 To n2
                                a(j, i_) = ctemp * a(j, i_)
                            Next
                            For i_ = n1 To n2
                                a(j, i_) = a(j, i_) + stemp * a(jp1, i_)
                            Next
                            For i_ = n1 To n2
                                a(jp1, i_) = work(i_)
                            Next
                        End If
                    Next
                Else

                    '
                    ' Special case: N1=N2
                    '
                    For j = m2 - 1 To m1 Step -1
                        ctemp = c(j - m1 + 1)
                        stemp = s(j - m1 + 1)
                        If CDbl(ctemp) <> CDbl(1) OrElse CDbl(stemp) <> CDbl(0) Then
                            temp = a(j + 1, n1)
                            a(j + 1, n1) = ctemp * temp - stemp * a(j, n1)
                            a(j, n1) = stemp * temp + ctemp * a(j, n1)
                        End If
                    Next
                End If
            End If
        End Sub


        '************************************************************************
        '        Application of a sequence of  elementary rotations to a matrix
        '
        '        The algorithm post-multiplies the matrix by a sequence of rotation
        '        transformations which is given by arrays C and S. Depending on the value
        '        of the IsForward parameter either 1 and 2, 3 and 4 and so on (if IsForward=true)
        '        rows are rotated, or the rows N and N-1, N-2 and N-3 and so on are rotated.
        '
        '        Not the whole matrix but only a part of it is transformed (rows from M1
        '        to M2, columns from N1 to N2). Only the elements of this submatrix are changed.
        '
        '        Input parameters:
        '            IsForward   -   the sequence of the rotation application.
        '            M1,M2       -   the range of rows to be transformed.
        '            N1, N2      -   the range of columns to be transformed.
        '            C,S         -   transformation coefficients.
        '                            Array whose index ranges within [1..N2-N1].
        '            A           -   processed matrix.
        '            WORK        -   working array whose index ranges within [M1..M2].
        '
        '        Output parameters:
        '            A           -   transformed matrix.
        '
        '        Utility subroutine.
        '        ************************************************************************

        Public Shared Sub applyrotationsfromtheright(isforward As Boolean, m1 As Integer, m2 As Integer, n1 As Integer, n2 As Integer, c As Double(), _
            s As Double(), a As Double(,), work As Double())
            Dim j As Integer = 0
            Dim jp1 As Integer = 0
            Dim ctemp As Double = 0
            Dim stemp As Double = 0
            Dim temp As Double = 0
            Dim i_ As Integer = 0


            '
            ' Form A * P'
            '
            If isforward Then
                If m1 <> m2 Then

                    '
                    ' Common case: M1<>M2
                    '
                    For j = n1 To n2 - 1
                        ctemp = c(j - n1 + 1)
                        stemp = s(j - n1 + 1)
                        If CDbl(ctemp) <> CDbl(1) OrElse CDbl(stemp) <> CDbl(0) Then
                            jp1 = j + 1
                            For i_ = m1 To m2
                                work(i_) = ctemp * a(i_, jp1)
                            Next
                            For i_ = m1 To m2
                                work(i_) = work(i_) - stemp * a(i_, j)
                            Next
                            For i_ = m1 To m2
                                a(i_, j) = ctemp * a(i_, j)
                            Next
                            For i_ = m1 To m2
                                a(i_, j) = a(i_, j) + stemp * a(i_, jp1)
                            Next
                            For i_ = m1 To m2
                                a(i_, jp1) = work(i_)
                            Next
                        End If
                    Next
                Else

                    '
                    ' Special case: M1=M2
                    '
                    For j = n1 To n2 - 1
                        ctemp = c(j - n1 + 1)
                        stemp = s(j - n1 + 1)
                        If CDbl(ctemp) <> CDbl(1) OrElse CDbl(stemp) <> CDbl(0) Then
                            temp = a(m1, j + 1)
                            a(m1, j + 1) = ctemp * temp - stemp * a(m1, j)
                            a(m1, j) = stemp * temp + ctemp * a(m1, j)
                        End If
                    Next
                End If
            Else
                If m1 <> m2 Then

                    '
                    ' Common case: M1<>M2
                    '
                    For j = n2 - 1 To n1 Step -1
                        ctemp = c(j - n1 + 1)
                        stemp = s(j - n1 + 1)
                        If CDbl(ctemp) <> CDbl(1) OrElse CDbl(stemp) <> CDbl(0) Then
                            jp1 = j + 1
                            For i_ = m1 To m2
                                work(i_) = ctemp * a(i_, jp1)
                            Next
                            For i_ = m1 To m2
                                work(i_) = work(i_) - stemp * a(i_, j)
                            Next
                            For i_ = m1 To m2
                                a(i_, j) = ctemp * a(i_, j)
                            Next
                            For i_ = m1 To m2
                                a(i_, j) = a(i_, j) + stemp * a(i_, jp1)
                            Next
                            For i_ = m1 To m2
                                a(i_, jp1) = work(i_)
                            Next
                        End If
                    Next
                Else

                    '
                    ' Special case: M1=M2
                    '
                    For j = n2 - 1 To n1 Step -1
                        ctemp = c(j - n1 + 1)
                        stemp = s(j - n1 + 1)
                        If CDbl(ctemp) <> CDbl(1) OrElse CDbl(stemp) <> CDbl(0) Then
                            temp = a(m1, j + 1)
                            a(m1, j + 1) = ctemp * temp - stemp * a(m1, j)
                            a(m1, j) = stemp * temp + ctemp * a(m1, j)
                        End If
                    Next
                End If
            End If
        End Sub


        '************************************************************************
        '        The subroutine generates the elementary rotation, so that:
        '
        '        [  CS  SN  ]  .  [ F ]  =  [ R ]
        '        [ -SN  CS  ]     [ G ]     [ 0 ]
        '
        '        CS**2 + SN**2 = 1
        '        ************************************************************************

        Public Shared Sub generaterotation(f As Double, g As Double, ByRef cs As Double, ByRef sn As Double, ByRef r As Double)
            Dim f1 As Double = 0
            Dim g1 As Double = 0

            cs = 0
            sn = 0
            r = 0

            If CDbl(g) = CDbl(0) Then
                cs = 1
                sn = 0
                r = f
            Else
                If CDbl(f) = CDbl(0) Then
                    cs = 0
                    sn = 1
                    r = g
                Else
                    f1 = f
                    g1 = g
                    If CDbl(System.Math.Abs(f1)) > CDbl(System.Math.Abs(g1)) Then
                        r = System.Math.Abs(f1) * System.Math.sqrt(1 + Math.sqr(g1 / f1))
                    Else
                        r = System.Math.Abs(g1) * System.Math.sqrt(1 + Math.sqr(f1 / g1))
                    End If
                    cs = f1 / r
                    sn = g1 / r
                    If CDbl(System.Math.Abs(f)) > CDbl(System.Math.Abs(g)) AndAlso CDbl(cs) < CDbl(0) Then
                        cs = -cs
                        sn = -sn
                        r = -r
                    End If
                End If
            End If
        End Sub


    End Class
    Public Class hsschur
        Public Shared Sub rmatrixinternalschurdecomposition(h As Double(,), n As Integer, tneeded As Integer, zneeded As Integer, ByRef wr As Double(), ByRef wi As Double(), _
            ByRef z As Double(,), ByRef info As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim h1 As Double(,) = New Double(-1, -1) {}
            Dim z1 As Double(,) = New Double(-1, -1) {}
            Dim wr1 As Double() = New Double(-1) {}
            Dim wi1 As Double() = New Double(-1) {}

            wr = New Double(-1) {}
            wi = New Double(-1) {}
            info = 0


            '
            ' Allocate space
            '
            wr = New Double(n - 1) {}
            wi = New Double(n - 1) {}
            If zneeded = 2 Then
                apserv.rmatrixsetlengthatleast(z, n, n)
            End If

            '
            ' MKL version
            '
            If ablasmkl.rmatrixinternalschurdecompositionmkl(h, n, tneeded, zneeded, wr, wi, _
                z, info) Then
                Return
            End If

            '
            ' ALGLIB version
            '
            h1 = New Double(n, n) {}
            For i = 0 To n - 1
                For j = 0 To n - 1
                    h1(1 + i, 1 + j) = h(i, j)
                Next
            Next
            If zneeded = 1 Then
                z1 = New Double(n, n) {}
                For i = 0 To n - 1
                    For j = 0 To n - 1
                        z1(1 + i, 1 + j) = z(i, j)
                    Next
                Next
            End If
            internalschurdecomposition(h1, n, tneeded, zneeded, wr1, wi1, _
                z1, info)
            For i = 0 To n - 1
                wr(i) = wr1(i + 1)
                wi(i) = wi1(i + 1)
            Next
            If tneeded <> 0 Then
                For i = 0 To n - 1
                    For j = 0 To n - 1
                        h(i, j) = h1(1 + i, 1 + j)
                    Next
                Next
            End If
            If zneeded <> 0 Then
                apserv.rmatrixsetlengthatleast(z, n, n)
                For i = 0 To n - 1
                    For j = 0 To n - 1
                        z(i, j) = z1(1 + i, 1 + j)
                    Next
                Next
            End If
        End Sub


        '************************************************************************
        '        Subroutine performing  the  Schur  decomposition  of  a  matrix  in  upper
        '        Hessenberg form using the QR algorithm with multiple shifts.
        '
        '        The  source matrix  H  is  represented as  S'*H*S = T, where H - matrix in
        '        upper Hessenberg form,  S - orthogonal matrix (Schur vectors),   T - upper
        '        quasi-triangular matrix (with blocks of sizes  1x1  and  2x2  on  the main
        '        diagonal).
        '
        '        Input parameters:
        '            H   -   matrix to be decomposed.
        '                    Array whose indexes range within [1..N, 1..N].
        '            N   -   size of H, N>=0.
        '
        '
        '        Output parameters:
        '            H   ?  contains the matrix T.
        '                    Array whose indexes range within [1..N, 1..N].
        '                    All elements below the blocks on the main diagonal are equal
        '                    to 0.
        '            S   -   contains Schur vectors.
        '                    Array whose indexes range within [1..N, 1..N].
        '
        '        Note 1:
        '            The block structure of matrix T could be easily recognized: since  all
        '            the elements  below  the blocks are zeros, the elements a[i+1,i] which
        '            are equal to 0 show the block border.
        '
        '        Note 2:
        '            the algorithm  performance  depends  on  the  value  of  the  internal
        '            parameter NS of InternalSchurDecomposition  subroutine  which  defines
        '            the number of shifts in the QR algorithm (analog of  the  block  width
        '            in block matrix algorithms in linear algebra). If you require  maximum
        '            performance  on  your  machine,  it  is  recommended  to  adjust  this
        '            parameter manually.
        '
        '        Result:
        '            True, if the algorithm has converged and the parameters H and S contain
        '                the result.
        '            False, if the algorithm has not converged.
        '
        '        Algorithm implemented on the basis of subroutine DHSEQR (LAPACK 3.0 library).
        '        ************************************************************************

        Public Shared Function upperhessenbergschurdecomposition(ByRef h As Double(,), n As Integer, ByRef s As Double(,)) As Boolean
            Dim result As New Boolean()
            Dim wi As Double() = New Double(-1) {}
            Dim wr As Double() = New Double(-1) {}
            Dim info As Integer = 0

            s = New Double(-1, -1) {}

            internalschurdecomposition(h, n, 1, 2, wr, wi, _
                s, info)
            result = info = 0
            Return result
        End Function


        Public Shared Sub internalschurdecomposition(ByRef h As Double(,), n As Integer, tneeded As Integer, zneeded As Integer, ByRef wr As Double(), ByRef wi As Double(), _
            ByRef z As Double(,), ByRef info As Integer)
            Dim work As Double() = New Double(-1) {}
            Dim i As Integer = 0
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim ierr As Integer = 0
            Dim ii As Integer = 0
            Dim itemp As Integer = 0
            Dim itn As Integer = 0
            Dim its As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim l As Integer = 0
            Dim maxb As Integer = 0
            Dim nr As Integer = 0
            Dim ns As Integer = 0
            Dim nv As Integer = 0
            Dim absw As Double = 0
            Dim smlnum As Double = 0
            Dim tau As Double = 0
            Dim temp As Double = 0
            Dim tst1 As Double = 0
            Dim ulp As Double = 0
            Dim unfl As Double = 0
            Dim s As Double(,) = New Double(-1, -1) {}
            Dim v As Double() = New Double(-1) {}
            Dim vv As Double() = New Double(-1) {}
            Dim workc1 As Double() = New Double(-1) {}
            Dim works1 As Double() = New Double(-1) {}
            Dim workv3 As Double() = New Double(-1) {}
            Dim tmpwr As Double() = New Double(-1) {}
            Dim tmpwi As Double() = New Double(-1) {}
            Dim initz As New Boolean()
            Dim wantt As New Boolean()
            Dim wantz As New Boolean()
            Dim cnst As Double = 0
            Dim failflag As New Boolean()
            Dim p1 As Integer = 0
            Dim p2 As Integer = 0
            Dim vt As Double = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            wr = New Double(-1) {}
            wi = New Double(-1) {}
            info = 0


            '
            ' Set the order of the multi-shift QR algorithm to be used.
            ' If you want to tune algorithm, change this values
            '
            ns = 12
            maxb = 50

            '
            ' Now 2 < NS <= MAXB < NH.
            '
            maxb = System.Math.Max(3, maxb)
            ns = System.Math.Min(maxb, ns)

            '
            ' Initialize
            '
            cnst = 1.5
            work = New Double(System.Math.Max(n, 1)) {}
            s = New Double(ns, ns) {}
            v = New Double(ns + 1) {}
            vv = New Double(ns + 1) {}
            wr = New Double(System.Math.Max(n, 1)) {}
            wi = New Double(System.Math.Max(n, 1)) {}
            workc1 = New Double(1) {}
            works1 = New Double(1) {}
            workv3 = New Double(3) {}
            tmpwr = New Double(System.Math.Max(n, 1)) {}
            tmpwi = New Double(System.Math.Max(n, 1)) {}
            alglib.ap.assert(n >= 0, "InternalSchurDecomposition: incorrect N!")
            alglib.ap.assert(tneeded = 0 OrElse tneeded = 1, "InternalSchurDecomposition: incorrect TNeeded!")
            alglib.ap.assert((zneeded = 0 OrElse zneeded = 1) OrElse zneeded = 2, "InternalSchurDecomposition: incorrect ZNeeded!")
            wantt = tneeded = 1
            initz = zneeded = 2
            wantz = zneeded <> 0
            info = 0

            '
            ' Initialize Z, if necessary
            '
            If initz Then
                apserv.rmatrixsetlengthatleast(z, n + 1, n + 1)
                For i = 1 To n
                    For j = 1 To n
                        If i = j Then
                            z(i, j) = 1
                        Else
                            z(i, j) = 0
                        End If
                    Next
                Next
            End If

            '
            ' Quick return if possible
            '
            If n = 0 Then
                Return
            End If
            If n = 1 Then
                wr(1) = h(1, 1)
                wi(1) = 0
                Return
            End If

            '
            ' Set rows and columns 1 to N to zero below the first
            ' subdiagonal.
            '
            For j = 1 To n - 2
                For i = j + 2 To n
                    h(i, j) = 0
                Next
            Next

            '
            ' Test if N is sufficiently small
            '
            If (ns <= 2 OrElse ns > n) OrElse maxb >= n Then

                '
                ' Use the standard double-shift algorithm
                '
                internalauxschur(wantt, wantz, n, 1, n, h, _
                    wr, wi, 1, n, z, work, _
                    workv3, workc1, works1, info)

                '
                ' fill entries under diagonal blocks of T with zeros
                '
                If wantt Then
                    j = 1
                    While j <= n
                        If CDbl(wi(j)) = CDbl(0) Then
                            For i = j + 1 To n
                                h(i, j) = 0
                            Next
                            j = j + 1
                        Else
                            For i = j + 2 To n
                                h(i, j) = 0
                                h(i, j + 1) = 0
                            Next
                            j = j + 2
                        End If
                    End While
                End If
                Return
            End If
            unfl = Math.minrealnumber
            ulp = 2 * Math.machineepsilon
            smlnum = unfl * (n / ulp)

            '
            ' I1 and I2 are the indices of the first row and last column of H
            ' to which transformations must be applied. If eigenvalues only are
            ' being computed, I1 and I2 are set inside the main loop.
            '
            i1 = 1
            i2 = n

            '
            ' ITN is the total number of multiple-shift QR iterations allowed.
            '
            itn = 30 * n

            '
            ' The main loop begins here. I is the loop index and decreases from
            ' IHI to ILO in steps of at most MAXB. Each iteration of the loop
            ' works with the active submatrix in rows and columns L to I.
            ' Eigenvalues I+1 to IHI have already converged. Either L = ILO or
            ' H(L,L-1) is negligible so that the matrix splits.
            '
            i = n
            While True
                l = 1
                If i < 1 Then

                    '
                    ' fill entries under diagonal blocks of T with zeros
                    '
                    If wantt Then
                        j = 1
                        While j <= n
                            If CDbl(wi(j)) = CDbl(0) Then
                                For i = j + 1 To n
                                    h(i, j) = 0
                                Next
                                j = j + 1
                            Else
                                For i = j + 2 To n
                                    h(i, j) = 0
                                    h(i, j + 1) = 0
                                Next
                                j = j + 2
                            End If
                        End While
                    End If

                    '
                    ' Exit
                    '
                    Return
                End If

                '
                ' Perform multiple-shift QR iterations on rows and columns ILO to I
                ' until a submatrix of order at most MAXB splits off at the bottom
                ' because a subdiagonal element has become negligible.
                '
                failflag = True
                For its = 0 To itn

                    '
                    ' Look for a single small subdiagonal element.
                    '
                    For k = i To l + 1 Step -1
                        tst1 = System.Math.Abs(h(k - 1, k - 1)) + System.Math.Abs(h(k, k))
                        If CDbl(tst1) = CDbl(0) Then
                            tst1 = blas.upperhessenberg1norm(h, l, i, l, i, work)
                        End If
                        If CDbl(System.Math.Abs(h(k, k - 1))) <= CDbl(System.Math.Max(ulp * tst1, smlnum)) Then
                            Exit For
                        End If
                    Next
                    l = k
                    If l > 1 Then

                        '
                        ' H(L,L-1) is negligible.
                        '
                        h(l, l - 1) = 0
                    End If

                    '
                    ' Exit from loop if a submatrix of order <= MAXB has split off.
                    '
                    If l >= i - maxb + 1 Then
                        failflag = False
                        Exit For
                    End If

                    '
                    ' Now the active submatrix is in rows and columns L to I. If
                    ' eigenvalues only are being computed, only the active submatrix
                    ' need be transformed.
                    '
                    If its = 20 OrElse its = 30 Then

                        '
                        ' Exceptional shifts.
                        '
                        For ii = i - ns + 1 To i
                            wr(ii) = cnst * (System.Math.Abs(h(ii, ii - 1)) + System.Math.Abs(h(ii, ii)))
                            wi(ii) = 0
                        Next
                    Else

                        '
                        ' Use eigenvalues of trailing submatrix of order NS as shifts.
                        '
                        blas.copymatrix(h, i - ns + 1, i, i - ns + 1, i, s, _
                            1, ns, 1, ns)
                        internalauxschur(False, False, ns, 1, ns, s, _
                            tmpwr, tmpwi, 1, ns, z, work, _
                            workv3, workc1, works1, ierr)
                        For p1 = 1 To ns
                            wr(i - ns + p1) = tmpwr(p1)
                            wi(i - ns + p1) = tmpwi(p1)
                        Next
                        If ierr > 0 Then

                            '
                            ' If DLAHQR failed to compute all NS eigenvalues, use the
                            ' unconverged diagonal elements as the remaining shifts.
                            '
                            For ii = 1 To ierr
                                wr(i - ns + ii) = s(ii, ii)
                                wi(i - ns + ii) = 0
                            Next
                        End If
                    End If

                    '
                    ' Form the first column of (G-w(1)) (G-w(2)) . . . (G-w(ns))
                    ' where G is the Hessenberg submatrix H(L:I,L:I) and w is
                    ' the vector of shifts (stored in WR and WI). The result is
                    ' stored in the local array V.
                    '
                    v(1) = 1
                    For ii = 2 To ns + 1
                        v(ii) = 0
                    Next
                    nv = 1
                    For j = i - ns + 1 To i
                        If CDbl(wi(j)) >= CDbl(0) Then
                            If CDbl(wi(j)) = CDbl(0) Then

                                '
                                ' real shift
                                '
                                p1 = nv + 1
                                For i_ = 1 To p1
                                    vv(i_) = v(i_)
                                Next
                                blas.matrixvectormultiply(h, l, l + nv, l, l + nv - 1, False, _
                                    vv, 1, nv, 1.0, v, 1, _
                                    nv + 1, -wr(j))
                                nv = nv + 1
                            Else
                                If CDbl(wi(j)) > CDbl(0) Then

                                    '
                                    ' complex conjugate pair of shifts
                                    '
                                    p1 = nv + 1
                                    For i_ = 1 To p1
                                        vv(i_) = v(i_)
                                    Next
                                    blas.matrixvectormultiply(h, l, l + nv, l, l + nv - 1, False, _
                                        v, 1, nv, 1.0, vv, 1, _
                                        nv + 1, -(2 * wr(j)))
                                    itemp = blas.vectoridxabsmax(vv, 1, nv + 1)
                                    temp = 1 / System.Math.Max(System.Math.Abs(vv(itemp)), smlnum)
                                    p1 = nv + 1
                                    For i_ = 1 To p1
                                        vv(i_) = temp * vv(i_)
                                    Next
                                    absw = blas.pythag2(wr(j), wi(j))
                                    temp = temp * absw * absw
                                    blas.matrixvectormultiply(h, l, l + nv + 1, l, l + nv, False, _
                                        vv, 1, nv + 1, 1.0, v, 1, _
                                        nv + 2, temp)
                                    nv = nv + 2
                                End If
                            End If

                            '
                            ' Scale V(1:NV) so that max(abs(V(i))) = 1. If V is zero,
                            ' reset it to the unit vector.
                            '
                            itemp = blas.vectoridxabsmax(v, 1, nv)
                            temp = System.Math.Abs(v(itemp))
                            If CDbl(temp) = CDbl(0) Then
                                v(1) = 1
                                For ii = 2 To nv
                                    v(ii) = 0
                                Next
                            Else
                                temp = System.Math.Max(temp, smlnum)
                                vt = 1 / temp
                                For i_ = 1 To nv
                                    v(i_) = vt * v(i_)
                                Next
                            End If
                        End If
                    Next

                    '
                    ' Multiple-shift QR step
                    '
                    For k = l To i - 1

                        '
                        ' The first iteration of this loop determines a reflection G
                        ' from the vector V and applies it from left and right to H,
                        ' thus creating a nonzero bulge below the subdiagonal.
                        '
                        ' Each subsequent iteration determines a reflection G to
                        ' restore the Hessenberg form in the (K-1)th column, and thus
                        ' chases the bulge one step toward the bottom of the active
                        ' submatrix. NR is the order of G.
                        '
                        nr = System.Math.Min(ns + 1, i - k + 1)
                        If k > l Then
                            p1 = k - 1
                            p2 = k + nr - 1
                            i1_ = (k) - (1)
                            For i_ = 1 To nr
                                v(i_) = h(i_ + i1_, p1)
                            Next
                            apserv.touchint(p2)
                        End If
                        reflections.generatereflection(v, nr, tau)
                        If k > l Then
                            h(k, k - 1) = v(1)
                            For ii = k + 1 To i
                                h(ii, k - 1) = 0
                            Next
                        End If
                        v(1) = 1

                        '
                        ' Apply G from the left to transform the rows of the matrix in
                        ' columns K to I2.
                        '
                        reflections.applyreflectionfromtheleft(h, tau, v, k, k + nr - 1, k, _
                            i2, work)

                        '
                        ' Apply G from the right to transform the columns of the
                        ' matrix in rows I1 to min(K+NR,I).
                        '
                        reflections.applyreflectionfromtheright(h, tau, v, i1, System.Math.Min(k + nr, i), k, _
                            k + nr - 1, work)
                        If wantz Then

                            '
                            ' Accumulate transformations in the matrix Z
                            '
                            reflections.applyreflectionfromtheright(z, tau, v, 1, n, k, _
                                k + nr - 1, work)
                        End If
                    Next
                Next

                '
                ' Failure to converge in remaining number of iterations
                '
                If failflag Then
                    info = i
                    Return
                End If

                '
                ' A submatrix of order <= MAXB in rows and columns L to I has split
                ' off. Use the double-shift QR algorithm to handle it.
                '
                internalauxschur(wantt, wantz, n, l, i, h, _
                    wr, wi, 1, n, z, work, _
                    workv3, workc1, works1, info)
                If info > 0 Then
                    Return
                End If

                '
                ' Decrement number of remaining iterations, and return to start of
                ' the main loop with a new value of I.
                '
                itn = itn - its
                i = l - 1
            End While
        End Sub


        Private Shared Sub internalauxschur(wantt As Boolean, wantz As Boolean, n As Integer, ilo As Integer, ihi As Integer, ByRef h As Double(,), _
            ByRef wr As Double(), ByRef wi As Double(), iloz As Integer, ihiz As Integer, ByRef z As Double(,), ByRef work As Double(), _
            ByRef workv3 As Double(), ByRef workc1 As Double(), ByRef works1 As Double(), ByRef info As Integer)
            Dim i As Integer = 0
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim itn As Integer = 0
            Dim its As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim l As Integer = 0
            Dim m As Integer = 0
            Dim nh As Integer = 0
            Dim nr As Integer = 0
            Dim nz As Integer = 0
            Dim ave As Double = 0
            Dim cs As Double = 0
            Dim disc As Double = 0
            Dim h00 As Double = 0
            Dim h10 As Double = 0
            Dim h11 As Double = 0
            Dim h12 As Double = 0
            Dim h21 As Double = 0
            Dim h22 As Double = 0
            Dim h33 As Double = 0
            Dim h33s As Double = 0
            Dim h43h34 As Double = 0
            Dim h44 As Double = 0
            Dim h44s As Double = 0
            Dim s As Double = 0
            Dim smlnum As Double = 0
            Dim sn As Double = 0
            Dim sum As Double = 0
            Dim t1 As Double = 0
            Dim t2 As Double = 0
            Dim t3 As Double = 0
            Dim tst1 As Double = 0
            Dim unfl As Double = 0
            Dim v1 As Double = 0
            Dim v2 As Double = 0
            Dim v3 As Double = 0
            Dim failflag As New Boolean()
            Dim dat1 As Double = 0
            Dim dat2 As Double = 0
            Dim p1 As Integer = 0
            Dim him1im1 As Double = 0
            Dim him1i As Double = 0
            Dim hiim1 As Double = 0
            Dim hii As Double = 0
            Dim wrim1 As Double = 0
            Dim wri As Double = 0
            Dim wiim1 As Double = 0
            Dim wii As Double = 0
            Dim ulp As Double = 0

            info = 0

            info = 0
            dat1 = 0.75
            dat2 = -0.4375
            ulp = Math.machineepsilon

            '
            ' Quick return if possible
            '
            If n = 0 Then
                Return
            End If
            If ilo = ihi Then
                wr(ilo) = h(ilo, ilo)
                wi(ilo) = 0
                Return
            End If
            nh = ihi - ilo + 1
            nz = ihiz - iloz + 1

            '
            ' Set machine-dependent constants for the stopping criterion.
            ' If norm(H) <= sqrt(MaxRealNumber), overflow should not occur.
            '
            unfl = Math.minrealnumber
            smlnum = unfl * (nh / ulp)

            '
            ' I1 and I2 are the indices of the first row and last column of H
            ' to which transformations must be applied. If eigenvalues only are
            ' being computed, I1 and I2 are set inside the main loop.
            '
            i1 = 1
            i2 = n

            '
            ' ITN is the total number of QR iterations allowed.
            '
            itn = 30 * nh

            '
            ' The main loop begins here. I is the loop index and decreases from
            ' IHI to ILO in steps of 1 or 2. Each iteration of the loop works
            ' with the active submatrix in rows and columns L to I.
            ' Eigenvalues I+1 to IHI have already converged. Either L = ILO or
            ' H(L,L-1) is negligible so that the matrix splits.
            '
            i = ihi
            While True
                l = ilo
                If i < ilo Then
                    Return
                End If

                '
                ' Perform QR iterations on rows and columns ILO to I until a
                ' submatrix of order 1 or 2 splits off at the bottom because a
                ' subdiagonal element has become negligible.
                '
                failflag = True
                For its = 0 To itn

                    '
                    ' Look for a single small subdiagonal element.
                    '
                    For k = i To l + 1 Step -1
                        tst1 = System.Math.Abs(h(k - 1, k - 1)) + System.Math.Abs(h(k, k))
                        If CDbl(tst1) = CDbl(0) Then
                            tst1 = blas.upperhessenberg1norm(h, l, i, l, i, work)
                        End If
                        If CDbl(System.Math.Abs(h(k, k - 1))) <= CDbl(System.Math.Max(ulp * tst1, smlnum)) Then
                            Exit For
                        End If
                    Next
                    l = k
                    If l > ilo Then

                        '
                        ' H(L,L-1) is negligible
                        '
                        h(l, l - 1) = 0
                    End If

                    '
                    ' Exit from loop if a submatrix of order 1 or 2 has split off.
                    '
                    If l >= i - 1 Then
                        failflag = False
                        Exit For
                    End If

                    '
                    ' Now the active submatrix is in rows and columns L to I. If
                    ' eigenvalues only are being computed, only the active submatrix
                    ' need be transformed.
                    '
                    If its = 10 OrElse its = 20 Then

                        '
                        ' Exceptional shift.
                        '
                        s = System.Math.Abs(h(i, i - 1)) + System.Math.Abs(h(i - 1, i - 2))
                        h44 = dat1 * s + h(i, i)
                        h33 = h44
                        h43h34 = dat2 * s * s
                    Else

                        '
                        ' Prepare to use Francis' double shift
                        ' (i.e. 2nd degree generalized Rayleigh quotient)
                        '
                        h44 = h(i, i)
                        h33 = h(i - 1, i - 1)
                        h43h34 = h(i, i - 1) * h(i - 1, i)
                        s = h(i - 1, i - 2) * h(i - 1, i - 2)
                        disc = (h33 - h44) * 0.5
                        disc = disc * disc + h43h34
                        If CDbl(disc) > CDbl(0) Then

                            '
                            ' Real roots: use Wilkinson's shift twice
                            '
                            disc = System.Math.sqrt(disc)
                            ave = 0.5 * (h33 + h44)
                            If CDbl(System.Math.Abs(h33) - System.Math.Abs(h44)) > CDbl(0) Then
                                h33 = h33 * h44 - h43h34
                                h44 = h33 / (extschursign(disc, ave) + ave)
                            Else
                                h44 = extschursign(disc, ave) + ave
                            End If
                            h33 = h44
                            h43h34 = 0
                        End If
                    End If

                    '
                    ' Look for two consecutive small subdiagonal elements.
                    '
                    For m = i - 2 To l Step -1

                        '
                        ' Determine the effect of starting the double-shift QR
                        ' iteration at row M, and see if this would make H(M,M-1)
                        ' negligible.
                        '
                        h11 = h(m, m)
                        h22 = h(m + 1, m + 1)
                        h21 = h(m + 1, m)
                        h12 = h(m, m + 1)
                        h44s = h44 - h11
                        h33s = h33 - h11
                        v1 = (h33s * h44s - h43h34) / h21 + h12
                        v2 = h22 - h11 - h33s - h44s
                        v3 = h(m + 2, m + 1)
                        s = System.Math.Abs(v1) + System.Math.Abs(v2) + System.Math.Abs(v3)
                        v1 = v1 / s
                        v2 = v2 / s
                        v3 = v3 / s
                        workv3(1) = v1
                        workv3(2) = v2
                        workv3(3) = v3
                        If m = l Then
                            Exit For
                        End If
                        h00 = h(m - 1, m - 1)
                        h10 = h(m, m - 1)
                        tst1 = System.Math.Abs(v1) * (System.Math.Abs(h00) + System.Math.Abs(h11) + System.Math.Abs(h22))
                        If CDbl(System.Math.Abs(h10) * (System.Math.Abs(v2) + System.Math.Abs(v3))) <= CDbl(ulp * tst1) Then
                            Exit For
                        End If
                    Next

                    '
                    ' Double-shift QR step
                    '
                    For k = m To i - 1

                        '
                        ' The first iteration of this loop determines a reflection G
                        ' from the vector V and applies it from left and right to H,
                        ' thus creating a nonzero bulge below the subdiagonal.
                        '
                        ' Each subsequent iteration determines a reflection G to
                        ' restore the Hessenberg form in the (K-1)th column, and thus
                        ' chases the bulge one step toward the bottom of the active
                        ' submatrix. NR is the order of G.
                        '
                        nr = System.Math.Min(3, i - k + 1)
                        If k > m Then
                            For p1 = 1 To nr
                                workv3(p1) = h(k + p1 - 1, k - 1)
                            Next
                        End If
                        reflections.generatereflection(workv3, nr, t1)
                        If k > m Then
                            h(k, k - 1) = workv3(1)
                            h(k + 1, k - 1) = 0
                            If k < i - 1 Then
                                h(k + 2, k - 1) = 0
                            End If
                        Else
                            If m > l Then
                                h(k, k - 1) = -h(k, k - 1)
                            End If
                        End If
                        v2 = workv3(2)
                        t2 = t1 * v2
                        If nr = 3 Then
                            v3 = workv3(3)
                            t3 = t1 * v3

                            '
                            ' Apply G from the left to transform the rows of the matrix
                            ' in columns K to I2.
                            '
                            For j = k To i2
                                sum = h(k, j) + v2 * h(k + 1, j) + v3 * h(k + 2, j)
                                h(k, j) = h(k, j) - sum * t1
                                h(k + 1, j) = h(k + 1, j) - sum * t2
                                h(k + 2, j) = h(k + 2, j) - sum * t3
                            Next

                            '
                            ' Apply G from the right to transform the columns of the
                            ' matrix in rows I1 to min(K+3,I).
                            '
                            For j = i1 To System.Math.Min(k + 3, i)
                                sum = h(j, k) + v2 * h(j, k + 1) + v3 * h(j, k + 2)
                                h(j, k) = h(j, k) - sum * t1
                                h(j, k + 1) = h(j, k + 1) - sum * t2
                                h(j, k + 2) = h(j, k + 2) - sum * t3
                            Next
                            If wantz Then

                                '
                                ' Accumulate transformations in the matrix Z
                                '
                                For j = iloz To ihiz
                                    sum = z(j, k) + v2 * z(j, k + 1) + v3 * z(j, k + 2)
                                    z(j, k) = z(j, k) - sum * t1
                                    z(j, k + 1) = z(j, k + 1) - sum * t2
                                    z(j, k + 2) = z(j, k + 2) - sum * t3
                                Next
                            End If
                        Else
                            If nr = 2 Then

                                '
                                ' Apply G from the left to transform the rows of the matrix
                                ' in columns K to I2.
                                '
                                For j = k To i2
                                    sum = h(k, j) + v2 * h(k + 1, j)
                                    h(k, j) = h(k, j) - sum * t1
                                    h(k + 1, j) = h(k + 1, j) - sum * t2
                                Next

                                '
                                ' Apply G from the right to transform the columns of the
                                ' matrix in rows I1 to min(K+3,I).
                                '
                                For j = i1 To i
                                    sum = h(j, k) + v2 * h(j, k + 1)
                                    h(j, k) = h(j, k) - sum * t1
                                    h(j, k + 1) = h(j, k + 1) - sum * t2
                                Next
                                If wantz Then

                                    '
                                    ' Accumulate transformations in the matrix Z
                                    '
                                    For j = iloz To ihiz
                                        sum = z(j, k) + v2 * z(j, k + 1)
                                        z(j, k) = z(j, k) - sum * t1
                                        z(j, k + 1) = z(j, k + 1) - sum * t2
                                    Next
                                End If
                            End If
                        End If
                    Next
                Next
                If failflag Then

                    '
                    ' Failure to converge in remaining number of iterations
                    '
                    info = i
                    Return
                End If
                If l = i Then

                    '
                    ' H(I,I-1) is negligible: one eigenvalue has converged.
                    '
                    wr(i) = h(i, i)
                    wi(i) = 0
                Else
                    If l = i - 1 Then

                        '
                        ' H(I-1,I-2) is negligible: a pair of eigenvalues have converged.
                        '
                        '        Transform the 2-by-2 submatrix to standard Schur form,
                        '        and compute and store the eigenvalues.
                        '
                        him1im1 = h(i - 1, i - 1)
                        him1i = h(i - 1, i)
                        hiim1 = h(i, i - 1)
                        hii = h(i, i)
                        aux2x2schur(him1im1, him1i, hiim1, hii, wrim1, wiim1, _
                            wri, wii, cs, sn)
                        wr(i - 1) = wrim1
                        wi(i - 1) = wiim1
                        wr(i) = wri
                        wi(i) = wii
                        h(i - 1, i - 1) = him1im1
                        h(i - 1, i) = him1i
                        h(i, i - 1) = hiim1
                        h(i, i) = hii
                        If wantt Then

                            '
                            ' Apply the transformation to the rest of H.
                            '
                            If i2 > i Then
                                workc1(1) = cs
                                works1(1) = sn
                                rotations.applyrotationsfromtheleft(True, i - 1, i, i + 1, i2, workc1, _
                                    works1, h, work)
                            End If
                            workc1(1) = cs
                            works1(1) = sn
                            rotations.applyrotationsfromtheright(True, i1, i - 2, i - 1, i, workc1, _
                                works1, h, work)
                        End If
                        If wantz Then

                            '
                            ' Apply the transformation to Z.
                            '
                            workc1(1) = cs
                            works1(1) = sn
                            rotations.applyrotationsfromtheright(True, iloz, iloz + nz - 1, i - 1, i, workc1, _
                                works1, z, work)
                        End If
                    End If
                End If

                '
                ' Decrement number of remaining iterations, and return to start of
                ' the main loop with new value of I.
                '
                itn = itn - its
                i = l - 1
            End While
        End Sub


        Private Shared Sub aux2x2schur(ByRef a As Double, ByRef b As Double, ByRef c As Double, ByRef d As Double, ByRef rt1r As Double, ByRef rt1i As Double, _
            ByRef rt2r As Double, ByRef rt2i As Double, ByRef cs As Double, ByRef sn As Double)
            Dim multpl As Double = 0
            Dim aa As Double = 0
            Dim bb As Double = 0
            Dim bcmax As Double = 0
            Dim bcmis As Double = 0
            Dim cc As Double = 0
            Dim cs1 As Double = 0
            Dim dd As Double = 0
            Dim eps As Double = 0
            Dim p As Double = 0
            Dim sab As Double = 0
            Dim sac As Double = 0
            Dim scl As Double = 0
            Dim sigma As Double = 0
            Dim sn1 As Double = 0
            Dim tau As Double = 0
            Dim temp As Double = 0
            Dim z As Double = 0

            rt1r = 0
            rt1i = 0
            rt2r = 0
            rt2i = 0
            cs = 0
            sn = 0

            multpl = 4.0
            eps = Math.machineepsilon
            If CDbl(c) = CDbl(0) Then
                cs = 1
                sn = 0
            Else
                If CDbl(b) = CDbl(0) Then

                    '
                    ' Swap rows and columns
                    '
                    cs = 0
                    sn = 1
                    temp = d
                    d = a
                    a = temp
                    b = -c
                    c = 0
                Else
                    If CDbl(a - d) = CDbl(0) AndAlso extschursigntoone(b) <> extschursigntoone(c) Then
                        cs = 1
                        sn = 0
                    Else
                        temp = a - d
                        p = 0.5 * temp
                        bcmax = System.Math.Max(System.Math.Abs(b), System.Math.Abs(c))
                        bcmis = System.Math.Min(System.Math.Abs(b), System.Math.Abs(c)) * extschursigntoone(b) * extschursigntoone(c)
                        scl = System.Math.Max(System.Math.Abs(p), bcmax)
                        z = p / scl * p + bcmax / scl * bcmis

                        '
                        ' If Z is of the order of the machine accuracy, postpone the
                        ' decision on the nature of eigenvalues
                        '
                        If CDbl(z) >= CDbl(multpl * eps) Then

                            '
                            ' Real eigenvalues. Compute A and D.
                            '
                            z = p + extschursign(System.Math.sqrt(scl) * System.Math.sqrt(z), p)
                            a = d + z
                            d = d - bcmax / z * bcmis

                            '
                            ' Compute B and the rotation matrix
                            '
                            tau = blas.pythag2(c, z)
                            cs = z / tau
                            sn = c / tau
                            b = b - c
                            c = 0
                        Else

                            '
                            ' Complex eigenvalues, or real (almost) equal eigenvalues.
                            ' Make diagonal elements equal.
                            '
                            sigma = b + c
                            tau = blas.pythag2(sigma, temp)
                            cs = System.Math.sqrt(0.5 * (1 + System.Math.Abs(sigma) / tau))
                            sn = -(p / (tau * cs) * extschursign(1, sigma))

                            '
                            ' Compute [ AA  BB ] = [ A  B ] [ CS -SN ]
                            '         [ CC  DD ]   [ C  D ] [ SN  CS ]
                            '
                            aa = a * cs + b * sn
                            bb = -(a * sn) + b * cs
                            cc = c * cs + d * sn
                            dd = -(c * sn) + d * cs

                            '
                            ' Compute [ A  B ] = [ CS  SN ] [ AA  BB ]
                            '         [ C  D ]   [-SN  CS ] [ CC  DD ]
                            '
                            a = aa * cs + cc * sn
                            b = bb * cs + dd * sn
                            c = -(aa * sn) + cc * cs
                            d = -(bb * sn) + dd * cs
                            temp = 0.5 * (a + d)
                            a = temp
                            d = temp
                            If CDbl(c) <> CDbl(0) Then
                                If CDbl(b) <> CDbl(0) Then
                                    If extschursigntoone(b) = extschursigntoone(c) Then

                                        '
                                        ' Real eigenvalues: reduce to upper triangular form
                                        '
                                        sab = System.Math.sqrt(System.Math.Abs(b))
                                        sac = System.Math.sqrt(System.Math.Abs(c))
                                        p = extschursign(sab * sac, c)
                                        tau = 1 / System.Math.sqrt(System.Math.Abs(b + c))
                                        a = temp + p
                                        d = temp - p
                                        b = b - c
                                        c = 0
                                        cs1 = sab * tau
                                        sn1 = sac * tau
                                        temp = cs * cs1 - sn * sn1
                                        sn = cs * sn1 + sn * cs1
                                        cs = temp
                                    End If
                                Else
                                    b = -c
                                    c = 0
                                    temp = cs
                                    cs = -sn
                                    sn = temp
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            '
            ' Store eigenvalues in (RT1R,RT1I) and (RT2R,RT2I).
            '
            rt1r = a
            rt2r = d
            If CDbl(c) = CDbl(0) Then
                rt1i = 0
                rt2i = 0
            Else
                rt1i = System.Math.sqrt(System.Math.Abs(b)) * System.Math.sqrt(System.Math.Abs(c))
                rt2i = -rt1i
            End If
        End Sub


        Private Shared Function extschursign(a As Double, b As Double) As Double
            Dim result As Double = 0

            If CDbl(b) >= CDbl(0) Then
                result = System.Math.Abs(a)
            Else
                result = -System.Math.Abs(a)
            End If
            Return result
        End Function


        Private Shared Function extschursigntoone(b As Double) As Integer
            Dim result As Integer = 0

            If CDbl(b) >= CDbl(0) Then
                result = 1
            Else
                result = -1
            End If
            Return result
        End Function


    End Class
    Public Class trlinsolve
        '************************************************************************
        '        Utility subroutine performing the "safe" solution of system of linear
        '        equations with triangular coefficient matrices.
        '
        '        The subroutine uses scaling and solves the scaled system A*x=s*b (where  s
        '        is  a  scalar  value)  instead  of  A*x=b,  choosing  s  so  that x can be
        '        represented by a floating-point number. The closer the system  gets  to  a
        '        singular, the less s is. If the system is singular, s=0 and x contains the
        '        non-trivial solution of equation A*x=0.
        '
        '        The feature of an algorithm is that it could not cause an  overflow  or  a
        '        division by zero regardless of the matrix used as the input.
        '
        '        The algorithm can solve systems of equations with  upper/lower  triangular
        '        matrices,  with/without unit diagonal, and systems of type A*x=b or A'*x=b
        '        (where A' is a transposed matrix A).
        '
        '        Input parameters:
        '            A       -   system matrix. Array whose indexes range within [0..N-1, 0..N-1].
        '            N       -   size of matrix A.
        '            X       -   right-hand member of a system.
        '                        Array whose index ranges within [0..N-1].
        '            IsUpper -   matrix type. If it is True, the system matrix is the upper
        '                        triangular and is located in  the  corresponding  part  of
        '                        matrix A.
        '            Trans   -   problem type. If it is True, the problem to be  solved  is
        '                        A'*x=b, otherwise it is A*x=b.
        '            Isunit  -   matrix type. If it is True, the system matrix has  a  unit
        '                        diagonal (the elements on the main diagonal are  not  used
        '                        in the calculation process), otherwise the matrix is considered
        '                        to be a general triangular matrix.
        '
        '        Output parameters:
        '            X       -   solution. Array whose index ranges within [0..N-1].
        '            S       -   scaling factor.
        '
        '          -- LAPACK auxiliary routine (version 3.0) --
        '             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
        '             Courant Institute, Argonne National Lab, and Rice University
        '             June 30, 1992
        '        ************************************************************************

        Public Shared Sub rmatrixtrsafesolve(a As Double(,), n As Integer, ByRef x As Double(), ByRef s As Double, isupper As Boolean, istrans As Boolean, _
            isunit As Boolean)
            Dim normin As New Boolean()
            Dim cnorm As Double() = New Double(-1) {}
            Dim a1 As Double(,) = New Double(-1, -1) {}
            Dim x1 As Double() = New Double(-1) {}
            Dim i As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            s = 0


            '
            ' From 0-based to 1-based
            '
            normin = False
            a1 = New Double(n, n) {}
            x1 = New Double(n) {}
            For i = 1 To n
                i1_ = (0) - (1)
                For i_ = 1 To n
                    a1(i, i_) = a(i - 1, i_ + i1_)
                Next
            Next
            i1_ = (0) - (1)
            For i_ = 1 To n
                x1(i_) = x(i_ + i1_)
            Next

            '
            ' Solve 1-based
            '
            safesolvetriangular(a1, n, x1, s, isupper, istrans, _
                isunit, normin, cnorm)

            '
            ' From 1-based to 0-based
            '
            i1_ = (1) - (0)
            For i_ = 0 To n - 1
                x(i_) = x1(i_ + i1_)
            Next
        End Sub


        '************************************************************************
        '        Obsolete 1-based subroutine.
        '        See RMatrixTRSafeSolve for 0-based replacement.
        '        ************************************************************************

        Public Shared Sub safesolvetriangular(a As Double(,), n As Integer, ByRef x As Double(), ByRef s As Double, isupper As Boolean, istrans As Boolean, _
            isunit As Boolean, normin As Boolean, ByRef cnorm As Double())
            Dim i As Integer = 0
            Dim imax As Integer = 0
            Dim j As Integer = 0
            Dim jfirst As Integer = 0
            Dim jinc As Integer = 0
            Dim jlast As Integer = 0
            Dim jm1 As Integer = 0
            Dim jp1 As Integer = 0
            Dim ip1 As Integer = 0
            Dim im1 As Integer = 0
            Dim k As Integer = 0
            Dim flg As Integer = 0
            Dim v As Double = 0
            Dim vd As Double = 0
            Dim bignum As Double = 0
            Dim grow As Double = 0
            Dim rec As Double = 0
            Dim smlnum As Double = 0
            Dim sumj As Double = 0
            Dim tjj As Double = 0
            Dim tjjs As Double = 0
            Dim tmax As Double = 0
            Dim tscal As Double = 0
            Dim uscal As Double = 0
            Dim xbnd As Double = 0
            Dim xj As Double = 0
            Dim xmax As Double = 0
            Dim notran As New Boolean()
            Dim upper As New Boolean()
            Dim nounit As New Boolean()
            Dim i_ As Integer = 0

            s = 0

            upper = isupper
            notran = Not istrans
            nounit = Not isunit

            '
            ' these initializers are not really necessary,
            ' but without them compiler complains about uninitialized locals
            '
            tjjs = 0

            '
            ' Quick return if possible
            '
            If n = 0 Then
                Return
            End If

            '
            ' Determine machine dependent parameters to control overflow.
            '
            smlnum = Math.minrealnumber / (Math.machineepsilon * 2)
            bignum = 1 / smlnum
            s = 1
            If Not normin Then
                cnorm = New Double(n) {}

                '
                ' Compute the 1-norm of each column, not including the diagonal.
                '
                If upper Then

                    '
                    ' A is upper triangular.
                    '
                    For j = 1 To n
                        v = 0
                        For k = 1 To j - 1
                            v = v + System.Math.Abs(a(k, j))
                        Next
                        cnorm(j) = v
                    Next
                Else

                    '
                    ' A is lower triangular.
                    '
                    For j = 1 To n - 1
                        v = 0
                        For k = j + 1 To n
                            v = v + System.Math.Abs(a(k, j))
                        Next
                        cnorm(j) = v
                    Next
                    cnorm(n) = 0
                End If
            End If

            '
            ' Scale the column norms by TSCAL if the maximum element in CNORM is
            ' greater than BIGNUM.
            '
            imax = 1
            For k = 2 To n
                If CDbl(cnorm(k)) > CDbl(cnorm(imax)) Then
                    imax = k
                End If
            Next
            tmax = cnorm(imax)
            If CDbl(tmax) <= CDbl(bignum) Then
                tscal = 1
            Else
                tscal = 1 / (smlnum * tmax)
                For i_ = 1 To n
                    cnorm(i_) = tscal * cnorm(i_)
                Next
            End If

            '
            ' Compute a bound on the computed solution vector to see if the
            ' Level 2 BLAS routine DTRSV can be used.
            '
            j = 1
            For k = 2 To n
                If CDbl(System.Math.Abs(x(k))) > CDbl(System.Math.Abs(x(j))) Then
                    j = k
                End If
            Next
            xmax = System.Math.Abs(x(j))
            xbnd = xmax
            If notran Then

                '
                ' Compute the growth in A * x = b.
                '
                If upper Then
                    jfirst = n
                    jlast = 1
                    jinc = -1
                Else
                    jfirst = 1
                    jlast = n
                    jinc = 1
                End If
                If CDbl(tscal) <> CDbl(1) Then
                    grow = 0
                Else
                    If nounit Then

                        '
                        ' A is non-unit triangular.
                        '
                        ' Compute GROW = 1/G(j) and XBND = 1/M(j).
                        ' Initially, G(0) = max{x(i), i=1,...,n}.
                        '
                        grow = 1 / System.Math.Max(xbnd, smlnum)
                        xbnd = grow
                        j = jfirst
                        While (jinc > 0 AndAlso j <= jlast) OrElse (jinc < 0 AndAlso j >= jlast)

                            '
                            ' Exit the loop if the growth factor is too small.
                            '
                            If CDbl(grow) <= CDbl(smlnum) Then
                                Exit While
                            End If

                            '
                            ' M(j) = G(j-1) / abs(A(j,j))
                            '
                            tjj = System.Math.Abs(a(j, j))
                            xbnd = System.Math.Min(xbnd, System.Math.Min(1, tjj) * grow)
                            If CDbl(tjj + cnorm(j)) >= CDbl(smlnum) Then

                                '
                                ' G(j) = G(j-1)*( 1 + CNORM(j) / abs(A(j,j)) )
                                '
                                grow = grow * (tjj / (tjj + cnorm(j)))
                            Else

                                '
                                ' G(j) could overflow, set GROW to 0.
                                '
                                grow = 0
                            End If
                            If j = jlast Then
                                grow = xbnd
                            End If
                            j = j + jinc
                        End While
                    Else

                        '
                        ' A is unit triangular.
                        '
                        ' Compute GROW = 1/G(j), where G(0) = max{x(i), i=1,...,n}.
                        '
                        grow = System.Math.Min(1, 1 / System.Math.Max(xbnd, smlnum))
                        j = jfirst
                        While (jinc > 0 AndAlso j <= jlast) OrElse (jinc < 0 AndAlso j >= jlast)

                            '
                            ' Exit the loop if the growth factor is too small.
                            '
                            If CDbl(grow) <= CDbl(smlnum) Then
                                Exit While
                            End If

                            '
                            ' G(j) = G(j-1)*( 1 + CNORM(j) )
                            '
                            grow = grow * (1 / (1 + cnorm(j)))
                            j = j + jinc
                        End While
                    End If
                End If
            Else

                '
                ' Compute the growth in A' * x = b.
                '
                If upper Then
                    jfirst = 1
                    jlast = n
                    jinc = 1
                Else
                    jfirst = n
                    jlast = 1
                    jinc = -1
                End If
                If CDbl(tscal) <> CDbl(1) Then
                    grow = 0
                Else
                    If nounit Then

                        '
                        ' A is non-unit triangular.
                        '
                        ' Compute GROW = 1/G(j) and XBND = 1/M(j).
                        ' Initially, M(0) = max{x(i), i=1,...,n}.
                        '
                        grow = 1 / System.Math.Max(xbnd, smlnum)
                        xbnd = grow
                        j = jfirst
                        While (jinc > 0 AndAlso j <= jlast) OrElse (jinc < 0 AndAlso j >= jlast)

                            '
                            ' Exit the loop if the growth factor is too small.
                            '
                            If CDbl(grow) <= CDbl(smlnum) Then
                                Exit While
                            End If

                            '
                            ' G(j) = max( G(j-1), M(j-1)*( 1 + CNORM(j) ) )
                            '
                            xj = 1 + cnorm(j)
                            grow = System.Math.Min(grow, xbnd / xj)

                            '
                            ' M(j) = M(j-1)*( 1 + CNORM(j) ) / abs(A(j,j))
                            '
                            tjj = System.Math.Abs(a(j, j))
                            If CDbl(xj) > CDbl(tjj) Then
                                xbnd = xbnd * (tjj / xj)
                            End If
                            If j = jlast Then
                                grow = System.Math.Min(grow, xbnd)
                            End If
                            j = j + jinc
                        End While
                    Else

                        '
                        ' A is unit triangular.
                        '
                        ' Compute GROW = 1/G(j), where G(0) = max{x(i), i=1,...,n}.
                        '
                        grow = System.Math.Min(1, 1 / System.Math.Max(xbnd, smlnum))
                        j = jfirst
                        While (jinc > 0 AndAlso j <= jlast) OrElse (jinc < 0 AndAlso j >= jlast)

                            '
                            ' Exit the loop if the growth factor is too small.
                            '
                            If CDbl(grow) <= CDbl(smlnum) Then
                                Exit While
                            End If

                            '
                            ' G(j) = ( 1 + CNORM(j) )*G(j-1)
                            '
                            xj = 1 + cnorm(j)
                            grow = grow / xj
                            j = j + jinc
                        End While
                    End If
                End If
            End If
            If CDbl(grow * tscal) > CDbl(smlnum) Then

                '
                ' Use the Level 2 BLAS solve if the reciprocal of the bound on
                ' elements of X is not too small.
                '
                If (upper AndAlso notran) OrElse (Not upper AndAlso Not notran) Then
                    If nounit Then
                        vd = a(n, n)
                    Else
                        vd = 1
                    End If
                    x(n) = x(n) / vd
                    For i = n - 1 To 1 Step -1
                        ip1 = i + 1
                        If upper Then
                            v = 0.0
                            For i_ = ip1 To n
                                v += a(i, i_) * x(i_)
                            Next
                        Else
                            v = 0.0
                            For i_ = ip1 To n
                                v += a(i_, i) * x(i_)
                            Next
                        End If
                        If nounit Then
                            vd = a(i, i)
                        Else
                            vd = 1
                        End If
                        x(i) = (x(i) - v) / vd
                    Next
                Else
                    If nounit Then
                        vd = a(1, 1)
                    Else
                        vd = 1
                    End If
                    x(1) = x(1) / vd
                    For i = 2 To n
                        im1 = i - 1
                        If upper Then
                            v = 0.0
                            For i_ = 1 To im1
                                v += a(i_, i) * x(i_)
                            Next
                        Else
                            v = 0.0
                            For i_ = 1 To im1
                                v += a(i, i_) * x(i_)
                            Next
                        End If
                        If nounit Then
                            vd = a(i, i)
                        Else
                            vd = 1
                        End If
                        x(i) = (x(i) - v) / vd
                    Next
                End If
            Else

                '
                ' Use a Level 1 BLAS solve, scaling intermediate results.
                '
                If CDbl(xmax) > CDbl(bignum) Then

                    '
                    ' Scale X so that its components are less than or equal to
                    ' BIGNUM in absolute value.
                    '
                    s = bignum / xmax
                    For i_ = 1 To n
                        x(i_) = s * x(i_)
                    Next
                    xmax = bignum
                End If
                If notran Then

                    '
                    ' Solve A * x = b
                    '
                    j = jfirst
                    While (jinc > 0 AndAlso j <= jlast) OrElse (jinc < 0 AndAlso j >= jlast)

                        '
                        ' Compute x(j) = b(j) / A(j,j), scaling x if necessary.
                        '
                        xj = System.Math.Abs(x(j))
                        flg = 0
                        If nounit Then
                            tjjs = a(j, j) * tscal
                        Else
                            tjjs = tscal
                            If CDbl(tscal) = CDbl(1) Then
                                flg = 100
                            End If
                        End If
                        If flg <> 100 Then
                            tjj = System.Math.Abs(tjjs)
                            If CDbl(tjj) > CDbl(smlnum) Then

                                '
                                ' abs(A(j,j)) > SMLNUM:
                                '
                                If CDbl(tjj) < CDbl(1) Then
                                    If CDbl(xj) > CDbl(tjj * bignum) Then

                                        '
                                        ' Scale x by 1/b(j).
                                        '
                                        rec = 1 / xj
                                        For i_ = 1 To n
                                            x(i_) = rec * x(i_)
                                        Next
                                        s = s * rec
                                        xmax = xmax * rec
                                    End If
                                End If
                                x(j) = x(j) / tjjs
                                xj = System.Math.Abs(x(j))
                            Else
                                If CDbl(tjj) > CDbl(0) Then

                                    '
                                    ' 0 < abs(A(j,j)) <= SMLNUM:
                                    '
                                    If CDbl(xj) > CDbl(tjj * bignum) Then

                                        '
                                        ' Scale x by (1/abs(x(j)))*abs(A(j,j))*BIGNUM
                                        ' to avoid overflow when dividing by A(j,j).
                                        '
                                        rec = tjj * bignum / xj
                                        If CDbl(cnorm(j)) > CDbl(1) Then

                                            '
                                            ' Scale by 1/CNORM(j) to avoid overflow when
                                            ' multiplying x(j) times column j.
                                            '
                                            rec = rec / cnorm(j)
                                        End If
                                        For i_ = 1 To n
                                            x(i_) = rec * x(i_)
                                        Next
                                        s = s * rec
                                        xmax = xmax * rec
                                    End If
                                    x(j) = x(j) / tjjs
                                    xj = System.Math.Abs(x(j))
                                Else

                                    '
                                    ' A(j,j) = 0:  Set x(1:n) = 0, x(j) = 1, and
                                    ' scale = 0, and compute a solution to A*x = 0.
                                    '
                                    For i = 1 To n
                                        x(i) = 0
                                    Next
                                    x(j) = 1
                                    xj = 1
                                    s = 0
                                    xmax = 0
                                End If
                            End If
                        End If

                        '
                        ' Scale x if necessary to avoid overflow when adding a
                        ' multiple of column j of A.
                        '
                        If CDbl(xj) > CDbl(1) Then
                            rec = 1 / xj
                            If CDbl(cnorm(j)) > CDbl((bignum - xmax) * rec) Then

                                '
                                ' Scale x by 1/(2*abs(x(j))).
                                '
                                rec = rec * 0.5
                                For i_ = 1 To n
                                    x(i_) = rec * x(i_)
                                Next
                                s = s * rec
                            End If
                        Else
                            If CDbl(xj * cnorm(j)) > CDbl(bignum - xmax) Then

                                '
                                ' Scale x by 1/2.
                                '
                                For i_ = 1 To n
                                    x(i_) = 0.5 * x(i_)
                                Next
                                s = s * 0.5
                            End If
                        End If
                        If upper Then
                            If j > 1 Then

                                '
                                ' Compute the update
                                ' x(1:j-1) := x(1:j-1) - x(j) * A(1:j-1,j)
                                '
                                v = x(j) * tscal
                                jm1 = j - 1
                                For i_ = 1 To jm1
                                    x(i_) = x(i_) - v * a(i_, j)
                                Next
                                i = 1
                                For k = 2 To j - 1
                                    If CDbl(System.Math.Abs(x(k))) > CDbl(System.Math.Abs(x(i))) Then
                                        i = k
                                    End If
                                Next
                                xmax = System.Math.Abs(x(i))
                            End If
                        Else
                            If j < n Then

                                '
                                ' Compute the update
                                ' x(j+1:n) := x(j+1:n) - x(j) * A(j+1:n,j)
                                '
                                jp1 = j + 1
                                v = x(j) * tscal
                                For i_ = jp1 To n
                                    x(i_) = x(i_) - v * a(i_, j)
                                Next
                                i = j + 1
                                For k = j + 2 To n
                                    If CDbl(System.Math.Abs(x(k))) > CDbl(System.Math.Abs(x(i))) Then
                                        i = k
                                    End If
                                Next
                                xmax = System.Math.Abs(x(i))
                            End If
                        End If
                        j = j + jinc
                    End While
                Else

                    '
                    ' Solve A' * x = b
                    '
                    j = jfirst
                    While (jinc > 0 AndAlso j <= jlast) OrElse (jinc < 0 AndAlso j >= jlast)

                        '
                        ' Compute x(j) = b(j) - sum A(k,j)*x(k).
                        '   k<>j
                        '
                        xj = System.Math.Abs(x(j))
                        uscal = tscal
                        rec = 1 / System.Math.Max(xmax, 1)
                        If CDbl(cnorm(j)) > CDbl((bignum - xj) * rec) Then

                            '
                            ' If x(j) could overflow, scale x by 1/(2*XMAX).
                            '
                            rec = rec * 0.5
                            If nounit Then
                                tjjs = a(j, j) * tscal
                            Else
                                tjjs = tscal
                            End If
                            tjj = System.Math.Abs(tjjs)
                            If CDbl(tjj) > CDbl(1) Then

                                '
                                ' Divide by A(j,j) when scaling x if A(j,j) > 1.
                                '
                                rec = System.Math.Min(1, rec * tjj)
                                uscal = uscal / tjjs
                            End If
                            If CDbl(rec) < CDbl(1) Then
                                For i_ = 1 To n
                                    x(i_) = rec * x(i_)
                                Next
                                s = s * rec
                                xmax = xmax * rec
                            End If
                        End If
                        sumj = 0
                        If CDbl(uscal) = CDbl(1) Then

                            '
                            ' If the scaling needed for A in the dot product is 1,
                            ' call DDOT to perform the dot product.
                            '
                            If upper Then
                                If j > 1 Then
                                    jm1 = j - 1
                                    sumj = 0.0
                                    For i_ = 1 To jm1
                                        sumj += a(i_, j) * x(i_)
                                    Next
                                Else
                                    sumj = 0
                                End If
                            Else
                                If j < n Then
                                    jp1 = j + 1
                                    sumj = 0.0
                                    For i_ = jp1 To n
                                        sumj += a(i_, j) * x(i_)
                                    Next
                                End If
                            End If
                        Else

                            '
                            ' Otherwise, use in-line code for the dot product.
                            '
                            If upper Then
                                For i = 1 To j - 1
                                    v = a(i, j) * uscal
                                    sumj = sumj + v * x(i)
                                Next
                            Else
                                If j < n Then
                                    For i = j + 1 To n
                                        v = a(i, j) * uscal
                                        sumj = sumj + v * x(i)
                                    Next
                                End If
                            End If
                        End If
                        If CDbl(uscal) = CDbl(tscal) Then

                            '
                            ' Compute x(j) := ( x(j) - sumj ) / A(j,j) if 1/A(j,j)
                            ' was not used to scale the dotproduct.
                            '
                            x(j) = x(j) - sumj
                            xj = System.Math.Abs(x(j))
                            flg = 0
                            If nounit Then
                                tjjs = a(j, j) * tscal
                            Else
                                tjjs = tscal
                                If CDbl(tscal) = CDbl(1) Then
                                    flg = 150
                                End If
                            End If

                            '
                            ' Compute x(j) = x(j) / A(j,j), scaling if necessary.
                            '
                            If flg <> 150 Then
                                tjj = System.Math.Abs(tjjs)
                                If CDbl(tjj) > CDbl(smlnum) Then

                                    '
                                    ' abs(A(j,j)) > SMLNUM:
                                    '
                                    If CDbl(tjj) < CDbl(1) Then
                                        If CDbl(xj) > CDbl(tjj * bignum) Then

                                            '
                                            ' Scale X by 1/abs(x(j)).
                                            '
                                            rec = 1 / xj
                                            For i_ = 1 To n
                                                x(i_) = rec * x(i_)
                                            Next
                                            s = s * rec
                                            xmax = xmax * rec
                                        End If
                                    End If
                                    x(j) = x(j) / tjjs
                                Else
                                    If CDbl(tjj) > CDbl(0) Then

                                        '
                                        ' 0 < abs(A(j,j)) <= SMLNUM:
                                        '
                                        If CDbl(xj) > CDbl(tjj * bignum) Then

                                            '
                                            ' Scale x by (1/abs(x(j)))*abs(A(j,j))*BIGNUM.
                                            '
                                            rec = tjj * bignum / xj
                                            For i_ = 1 To n
                                                x(i_) = rec * x(i_)
                                            Next
                                            s = s * rec
                                            xmax = xmax * rec
                                        End If
                                        x(j) = x(j) / tjjs
                                    Else

                                        '
                                        ' A(j,j) = 0:  Set x(1:n) = 0, x(j) = 1, and
                                        ' scale = 0, and compute a solution to A'*x = 0.
                                        '
                                        For i = 1 To n
                                            x(i) = 0
                                        Next
                                        x(j) = 1
                                        s = 0
                                        xmax = 0
                                    End If
                                End If
                            End If
                        Else

                            '
                            ' Compute x(j) := x(j) / A(j,j)  - sumj if the dot
                            ' product has already been divided by 1/A(j,j).
                            '
                            x(j) = x(j) / tjjs - sumj
                        End If
                        xmax = System.Math.Max(xmax, System.Math.Abs(x(j)))
                        j = j + jinc
                    End While
                End If
                s = s / tscal
            End If

            '
            ' Scale the column norms by 1/TSCAL for return.
            '
            If CDbl(tscal) <> CDbl(1) Then
                v = 1 / tscal
                For i_ = 1 To n
                    cnorm(i_) = v * cnorm(i_)
                Next
            End If
        End Sub


    End Class
    Public Class safesolve
        '************************************************************************
        '        Real implementation of CMatrixScaledTRSafeSolve
        '
        '          -- ALGLIB routine --
        '             21.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function rmatrixscaledtrsafesolve(a As Double(,), sa As Double, n As Integer, ByRef x As Double(), isupper As Boolean, trans As Integer, _
            isunit As Boolean, maxgrowth As Double) As Boolean
            Dim result As New Boolean()
            Dim lnmax As Double = 0
            Dim nrmb As Double = 0
            Dim nrmx As Double = 0
            Dim i As Integer = 0
            Dim alpha As complex = 0
            Dim beta As complex = 0
            Dim vr As Double = 0
            Dim cx As complex = 0
            Dim tmp As Double() = New Double(-1) {}
            Dim i_ As Integer = 0

            alglib.ap.assert(n > 0, "RMatrixTRSafeSolve: incorrect N!")
            alglib.ap.assert(trans = 0 OrElse trans = 1, "RMatrixTRSafeSolve: incorrect Trans!")
            result = True
            lnmax = System.Math.Log(Math.maxrealnumber)

            '
            ' Quick return if possible
            '
            If n <= 0 Then
                Return result
            End If

            '
            ' Load norms: right part and X
            '
            nrmb = 0
            For i = 0 To n - 1
                nrmb = System.Math.Max(nrmb, System.Math.Abs(x(i)))
            Next
            nrmx = 0

            '
            ' Solve
            '
            tmp = New Double(n - 1) {}
            result = True
            If isupper AndAlso trans = 0 Then

                '
                ' U*x = b
                '
                For i = n - 1 To 0 Step -1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = a(i, i) * sa
                    End If
                    If i < n - 1 Then
                        For i_ = i + 1 To n - 1
                            tmp(i_) = sa * a(i, i_)
                        Next
                        vr = 0.0
                        For i_ = i + 1 To n - 1
                            vr += tmp(i_) * x(i_)
                        Next
                        beta = x(i) - vr
                    Else
                        beta = x(i)
                    End If

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        cx)
                    If Not result Then
                        Return result
                    End If
                    x(i) = cx.x
                Next
                Return result
            End If
            If Not isupper AndAlso trans = 0 Then

                '
                ' L*x = b
                '
                For i = 0 To n - 1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = a(i, i) * sa
                    End If
                    If i > 0 Then
                        For i_ = 0 To i - 1
                            tmp(i_) = sa * a(i, i_)
                        Next
                        vr = 0.0
                        For i_ = 0 To i - 1
                            vr += tmp(i_) * x(i_)
                        Next
                        beta = x(i) - vr
                    Else
                        beta = x(i)
                    End If

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        cx)
                    If Not result Then
                        Return result
                    End If
                    x(i) = cx.x
                Next
                Return result
            End If
            If isupper AndAlso trans = 1 Then

                '
                ' U^T*x = b
                '
                For i = 0 To n - 1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = a(i, i) * sa
                    End If
                    beta = x(i)

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        cx)
                    If Not result Then
                        Return result
                    End If
                    x(i) = cx.x

                    '
                    ' update the rest of right part
                    '
                    If i < n - 1 Then
                        vr = cx.x
                        For i_ = i + 1 To n - 1
                            tmp(i_) = sa * a(i, i_)
                        Next
                        For i_ = i + 1 To n - 1
                            x(i_) = x(i_) - vr * tmp(i_)
                        Next
                    End If
                Next
                Return result
            End If
            If Not isupper AndAlso trans = 1 Then

                '
                ' L^T*x = b
                '
                For i = n - 1 To 0 Step -1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = a(i, i) * sa
                    End If
                    beta = x(i)

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        cx)
                    If Not result Then
                        Return result
                    End If
                    x(i) = cx.x

                    '
                    ' update the rest of right part
                    '
                    If i > 0 Then
                        vr = cx.x
                        For i_ = 0 To i - 1
                            tmp(i_) = sa * a(i, i_)
                        Next
                        For i_ = 0 To i - 1
                            x(i_) = x(i_) - vr * tmp(i_)
                        Next
                    End If
                Next
                Return result
            End If
            result = False
            Return result
        End Function


        '************************************************************************
        '        Internal subroutine for safe solution of
        '
        '            SA*op(A)=b
        '            
        '        where  A  is  NxN  upper/lower  triangular/unitriangular  matrix, op(A) is
        '        either identity transform, transposition or Hermitian transposition, SA is
        '        a scaling factor such that max(|SA*A[i,j]|) is close to 1.0 in magnutude.
        '
        '        This subroutine  limits  relative  growth  of  solution  (in inf-norm)  by
        '        MaxGrowth,  returning  False  if  growth  exceeds MaxGrowth. Degenerate or
        '        near-degenerate matrices are handled correctly (False is returned) as long
        '        as MaxGrowth is significantly less than MaxRealNumber/norm(b).
        '
        '          -- ALGLIB routine --
        '             21.01.2010
        '             Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function cmatrixscaledtrsafesolve(a As complex(,), sa As Double, n As Integer, ByRef x As complex(), isupper As Boolean, trans As Integer, _
            isunit As Boolean, maxgrowth As Double) As Boolean
            Dim result As New Boolean()
            Dim lnmax As Double = 0
            Dim nrmb As Double = 0
            Dim nrmx As Double = 0
            Dim i As Integer = 0
            Dim alpha As complex = 0
            Dim beta As complex = 0
            Dim vc As complex = 0
            Dim tmp As complex() = New complex(-1) {}
            Dim i_ As Integer = 0

            alglib.ap.assert(n > 0, "CMatrixTRSafeSolve: incorrect N!")
            alglib.ap.assert((trans = 0 OrElse trans = 1) OrElse trans = 2, "CMatrixTRSafeSolve: incorrect Trans!")
            result = True
            lnmax = System.Math.Log(Math.maxrealnumber)

            '
            ' Quick return if possible
            '
            If n <= 0 Then
                Return result
            End If

            '
            ' Load norms: right part and X
            '
            nrmb = 0
            For i = 0 To n - 1
                nrmb = System.Math.Max(nrmb, Math.abscomplex(x(i)))
            Next
            nrmx = 0

            '
            ' Solve
            '
            tmp = New complex(n - 1) {}
            result = True
            If isupper AndAlso trans = 0 Then

                '
                ' U*x = b
                '
                For i = n - 1 To 0 Step -1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = a(i, i) * sa
                    End If
                    If i < n - 1 Then
                        For i_ = i + 1 To n - 1
                            tmp(i_) = sa * a(i, i_)
                        Next
                        vc = 0.0
                        For i_ = i + 1 To n - 1
                            vc += tmp(i_) * x(i_)
                        Next
                        beta = x(i) - vc
                    Else
                        beta = x(i)
                    End If

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        vc)
                    If Not result Then
                        Return result
                    End If
                    x(i) = vc
                Next
                Return result
            End If
            If Not isupper AndAlso trans = 0 Then

                '
                ' L*x = b
                '
                For i = 0 To n - 1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = a(i, i) * sa
                    End If
                    If i > 0 Then
                        For i_ = 0 To i - 1
                            tmp(i_) = sa * a(i, i_)
                        Next
                        vc = 0.0
                        For i_ = 0 To i - 1
                            vc += tmp(i_) * x(i_)
                        Next
                        beta = x(i) - vc
                    Else
                        beta = x(i)
                    End If

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        vc)
                    If Not result Then
                        Return result
                    End If
                    x(i) = vc
                Next
                Return result
            End If
            If isupper AndAlso trans = 1 Then

                '
                ' U^T*x = b
                '
                For i = 0 To n - 1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = a(i, i) * sa
                    End If
                    beta = x(i)

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        vc)
                    If Not result Then
                        Return result
                    End If
                    x(i) = vc

                    '
                    ' update the rest of right part
                    '
                    If i < n - 1 Then
                        For i_ = i + 1 To n - 1
                            tmp(i_) = sa * a(i, i_)
                        Next
                        For i_ = i + 1 To n - 1
                            x(i_) = x(i_) - vc * tmp(i_)
                        Next
                    End If
                Next
                Return result
            End If
            If Not isupper AndAlso trans = 1 Then

                '
                ' L^T*x = b
                '
                For i = n - 1 To 0 Step -1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = a(i, i) * sa
                    End If
                    beta = x(i)

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        vc)
                    If Not result Then
                        Return result
                    End If
                    x(i) = vc

                    '
                    ' update the rest of right part
                    '
                    If i > 0 Then
                        For i_ = 0 To i - 1
                            tmp(i_) = sa * a(i, i_)
                        Next
                        For i_ = 0 To i - 1
                            x(i_) = x(i_) - vc * tmp(i_)
                        Next
                    End If
                Next
                Return result
            End If
            If isupper AndAlso trans = 2 Then

                '
                ' U^H*x = b
                '
                For i = 0 To n - 1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = Math.conj(a(i, i)) * sa
                    End If
                    beta = x(i)

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        vc)
                    If Not result Then
                        Return result
                    End If
                    x(i) = vc

                    '
                    ' update the rest of right part
                    '
                    If i < n - 1 Then
                        For i_ = i + 1 To n - 1
                            tmp(i_) = sa * Math.conj(a(i, i_))
                        Next
                        For i_ = i + 1 To n - 1
                            x(i_) = x(i_) - vc * tmp(i_)
                        Next
                    End If
                Next
                Return result
            End If
            If Not isupper AndAlso trans = 2 Then

                '
                ' L^T*x = b
                '
                For i = n - 1 To 0 Step -1

                    '
                    ' Task is reduced to alpha*x[i] = beta
                    '
                    If isunit Then
                        alpha = sa
                    Else
                        alpha = Math.conj(a(i, i)) * sa
                    End If
                    beta = x(i)

                    '
                    ' solve alpha*x[i] = beta
                    '
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, nrmx, _
                        vc)
                    If Not result Then
                        Return result
                    End If
                    x(i) = vc

                    '
                    ' update the rest of right part
                    '
                    If i > 0 Then
                        For i_ = 0 To i - 1
                            tmp(i_) = sa * Math.conj(a(i, i_))
                        Next
                        For i_ = 0 To i - 1
                            x(i_) = x(i_) - vc * tmp(i_)
                        Next
                    End If
                Next
                Return result
            End If
            result = False
            Return result
        End Function


        '************************************************************************
        '        complex basic solver-updater for reduced linear system
        '
        '            alpha*x[i] = beta
        '
        '        solves this equation and updates it in overlfow-safe manner (keeping track
        '        of relative growth of solution).
        '
        '        Parameters:
        '            Alpha   -   alpha
        '            Beta    -   beta
        '            LnMax   -   precomputed Ln(MaxRealNumber)
        '            BNorm   -   inf-norm of b (right part of original system)
        '            MaxGrowth-  maximum growth of norm(x) relative to norm(b)
        '            XNorm   -   inf-norm of other components of X (which are already processed)
        '                        it is updated by CBasicSolveAndUpdate.
        '            X       -   solution
        '
        '          -- ALGLIB routine --
        '             26.01.2009
        '             Bochkanov Sergey
        '        ************************************************************************

        Private Shared Function cbasicsolveandupdate(alpha As complex, beta As complex, lnmax As Double, bnorm As Double, maxgrowth As Double, ByRef xnorm As Double, _
            ByRef x As complex) As Boolean
            Dim result As New Boolean()
            Dim v As Double = 0

            x = 0

            result = False
            If alpha = 0 Then
                Return result
            End If
            If beta <> 0 Then

                '
                ' alpha*x[i]=beta
                '
                v = System.Math.Log(Math.abscomplex(beta)) - System.Math.Log(Math.abscomplex(alpha))
                If CDbl(v) > CDbl(lnmax) Then
                    Return result
                End If
                x = beta / alpha
            Else

                '
                ' alpha*x[i]=0
                '
                x = 0
            End If

            '
            ' update NrmX, test growth limit
            '
            xnorm = System.Math.Max(xnorm, Math.abscomplex(x))
            If CDbl(xnorm) > CDbl(maxgrowth * bnorm) Then
                Return result
            End If
            result = True
            Return result
        End Function


    End Class
    Public Class hpccores
        '************************************************************************
        '        This structure stores  temporary  buffers  used  by  gradient  calculation
        '        functions for neural networks.
        '        ************************************************************************

        Public Class mlpbuffers
            Inherits apobject
            Public chunksize As Integer
            Public ntotal As Integer
            Public nin As Integer
            Public nout As Integer
            Public wcount As Integer
            Public batch4buf As Double()
            Public hpcbuf As Double()
            Public xy As Double(,)
            Public xy2 As Double(,)
            Public xyrow As Double()
            Public x As Double()
            Public y As Double()
            Public desiredy As Double()
            Public e As Double
            Public g As Double()
            Public tmp0 As Double()
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
                batch4buf = New Double(-1) {}
                hpcbuf = New Double(-1) {}
                xy = New Double(-1, -1) {}
                xy2 = New Double(-1, -1) {}
                xyrow = New Double(-1) {}
                x = New Double(-1) {}
                y = New Double(-1) {}
                desiredy = New Double(-1) {}
                g = New Double(-1) {}
                tmp0 = New Double(-1) {}
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New mlpbuffers()
                _result.chunksize = chunksize
                _result.ntotal = ntotal
                _result.nin = nin
                _result.nout = nout
                _result.wcount = wcount
                _result.batch4buf = DirectCast(batch4buf.Clone(), Double())
                _result.hpcbuf = DirectCast(hpcbuf.Clone(), Double())
                _result.xy = DirectCast(xy.Clone(), Double(,))
                _result.xy2 = DirectCast(xy2.Clone(), Double(,))
                _result.xyrow = DirectCast(xyrow.Clone(), Double())
                _result.x = DirectCast(x.Clone(), Double())
                _result.y = DirectCast(y.Clone(), Double())
                _result.desiredy = DirectCast(desiredy.Clone(), Double())
                _result.e = e
                _result.g = DirectCast(g.Clone(), Double())
                _result.tmp0 = DirectCast(tmp0.Clone(), Double())
                Return _result
            End Function
        End Class




        '************************************************************************
        '        Prepares HPC compuations  of  chunked  gradient with HPCChunkedGradient().
        '        You  have to call this function  before  calling  HPCChunkedGradient() for
        '        a new set of weights. You have to call it only once, see example below:
        '
        '        HOW TO PROCESS DATASET WITH THIS FUNCTION:
        '            Grad:=0
        '            HPCPrepareChunkedGradient(Weights, WCount, NTotal, NOut, Buf)
        '            foreach chunk-of-dataset do
        '                HPCChunkedGradient(...)
        '            HPCFinalizeChunkedGradient(Buf, Grad)
        '
        '        ************************************************************************

        Public Shared Sub hpcpreparechunkedgradient(weights As Double(), wcount As Integer, ntotal As Integer, nin As Integer, nout As Integer, buf As mlpbuffers)
            Dim i As Integer = 0
            Dim batch4size As Integer = 0
            Dim chunksize As Integer = 0

            chunksize = 4
            batch4size = 3 * chunksize * ntotal + chunksize * (2 * nout + 1)
            If alglib.ap.rows(buf.xy) < chunksize OrElse alglib.ap.cols(buf.xy) < nin + nout Then
                buf.xy = New Double(chunksize - 1, nin + (nout - 1)) {}
            End If
            If alglib.ap.rows(buf.xy2) < chunksize OrElse alglib.ap.cols(buf.xy2) < nin + nout Then
                buf.xy2 = New Double(chunksize - 1, nin + (nout - 1)) {}
            End If
            If alglib.ap.len(buf.xyrow) < nin + nout Then
                buf.xyrow = New Double(nin + (nout - 1)) {}
            End If
            If alglib.ap.len(buf.x) < nin Then
                buf.x = New Double(nin - 1) {}
            End If
            If alglib.ap.len(buf.y) < nout Then
                buf.y = New Double(nout - 1) {}
            End If
            If alglib.ap.len(buf.desiredy) < nout Then
                buf.desiredy = New Double(nout - 1) {}
            End If
            If alglib.ap.len(buf.batch4buf) < batch4size Then
                buf.batch4buf = New Double(batch4size - 1) {}
            End If
            If alglib.ap.len(buf.hpcbuf) < wcount Then
                buf.hpcbuf = New Double(wcount - 1) {}
            End If
            If alglib.ap.len(buf.g) < wcount Then
                buf.g = New Double(wcount - 1) {}
            End If
            If Not hpcpreparechunkedgradientx(weights, wcount, buf.hpcbuf) Then
                For i = 0 To wcount - 1
                    buf.hpcbuf(i) = 0.0
                Next
            End If
            buf.wcount = wcount
            buf.ntotal = ntotal
            buf.nin = nin
            buf.nout = nout
            buf.chunksize = chunksize
        End Sub


        '************************************************************************
        '        Finalizes HPC compuations  of  chunked gradient with HPCChunkedGradient().
        '        You  have to call this function  after  calling  HPCChunkedGradient()  for
        '        a new set of weights. You have to call it only once, see example below:
        '
        '        HOW TO PROCESS DATASET WITH THIS FUNCTION:
        '            Grad:=0
        '            HPCPrepareChunkedGradient(Weights, WCount, NTotal, NOut, Buf)
        '            foreach chunk-of-dataset do
        '                HPCChunkedGradient(...)
        '            HPCFinalizeChunkedGradient(Buf, Grad)
        '
        '        ************************************************************************

        Public Shared Sub hpcfinalizechunkedgradient(buf As mlpbuffers, grad As Double())
            Dim i As Integer = 0

            If Not hpcfinalizechunkedgradientx(buf.hpcbuf, buf.wcount, grad) Then
                For i = 0 To buf.wcount - 1
                    grad(i) = grad(i) + buf.hpcbuf(i)
                Next
            End If
        End Sub


        '************************************************************************
        '        Fast kernel for chunked gradient.
        '
        '        ************************************************************************

        Public Shared Function hpcchunkedgradient(weights As Double(), structinfo As Integer(), columnmeans As Double(), columnsigmas As Double(), xy As Double(,), cstart As Integer, _
            csize As Integer, batch4buf As Double(), hpcbuf As Double(), ByRef e As Double, naturalerrorfunc As Boolean) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Fast kernel for chunked processing.
        '
        '        ************************************************************************

        Public Shared Function hpcchunkedprocess(weights As Double(), structinfo As Integer(), columnmeans As Double(), columnsigmas As Double(), xy As Double(,), cstart As Integer, _
            csize As Integer, batch4buf As Double(), hpcbuf As Double()) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Stub function.
        '
        '          -- ALGLIB routine --
        '             14.06.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Private Shared Function hpcpreparechunkedgradientx(weights As Double(), wcount As Integer, hpcbuf As Double()) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


        '************************************************************************
        '        Stub function.
        '
        '          -- ALGLIB routine --
        '             14.06.2013
        '             Bochkanov Sergey
        '        ************************************************************************

        Private Shared Function hpcfinalizechunkedgradientx(buf As Double(), wcount As Integer, grad As Double()) As Boolean
            Dim result As New Boolean()

            result = False
            Return result
        End Function


    End Class
    Public Class xblas
        '************************************************************************
        '        More precise dot-product. Absolute error of  subroutine  result  is  about
        '        1 ulp of max(MX,V), where:
        '            MX = max( |a[i]*b[i]| )
        '            V  = |(a,b)|
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1], vector 1
        '            B       -   array[0..N-1], vector 2
        '            N       -   vectors length, N<2^29.
        '            Temp    -   array[0..N-1], pre-allocated temporary storage
        '
        '        OUTPUT PARAMETERS
        '            R       -   (A,B)
        '            RErr    -   estimate of error. This estimate accounts for both  errors
        '                        during  calculation  of  (A,B)  and  errors  introduced by
        '                        rounding of A and B to fit in double (about 1 ulp).
        '
        '          -- ALGLIB --
        '             Copyright 24.08.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdot(a As Double(), b As Double(), n As Integer, ByRef temp As Double(), ByRef r As Double, ByRef rerr As Double)
            Dim i As Integer = 0
            Dim mx As Double = 0
            Dim v As Double = 0

            r = 0
            rerr = 0


            '
            ' special cases:
            ' * N=0
            '
            If n = 0 Then
                r = 0
                rerr = 0
                Return
            End If
            mx = 0
            For i = 0 To n - 1
                v = a(i) * b(i)
                temp(i) = v
                mx = System.Math.Max(mx, System.Math.Abs(v))
            Next
            If CDbl(mx) = CDbl(0) Then
                r = 0
                rerr = 0
                Return
            End If
            xsum(temp, mx, n, r, rerr)
        End Sub


        '************************************************************************
        '        More precise complex dot-product. Absolute error of  subroutine  result is
        '        about 1 ulp of max(MX,V), where:
        '            MX = max( |a[i]*b[i]| )
        '            V  = |(a,b)|
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1], vector 1
        '            B       -   array[0..N-1], vector 2
        '            N       -   vectors length, N<2^29.
        '            Temp    -   array[0..2*N-1], pre-allocated temporary storage
        '
        '        OUTPUT PARAMETERS
        '            R       -   (A,B)
        '            RErr    -   estimate of error. This estimate accounts for both  errors
        '                        during  calculation  of  (A,B)  and  errors  introduced by
        '                        rounding of A and B to fit in double (about 1 ulp).
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xcdot(a As complex(), b As complex(), n As Integer, ByRef temp As Double(), ByRef r As complex, ByRef rerr As Double)
            Dim i As Integer = 0
            Dim mx As Double = 0
            Dim v As Double = 0
            Dim rerrx As Double = 0
            Dim rerry As Double = 0

            r = 0
            rerr = 0


            '
            ' special cases:
            ' * N=0
            '
            If n = 0 Then
                r = 0
                rerr = 0
                Return
            End If

            '
            ' calculate real part
            '
            mx = 0
            For i = 0 To n - 1
                v = a(i).x * b(i).x
                temp(2 * i + 0) = v
                mx = System.Math.Max(mx, System.Math.Abs(v))
                v = -(a(i).y * b(i).y)
                temp(2 * i + 1) = v
                mx = System.Math.Max(mx, System.Math.Abs(v))
            Next
            If CDbl(mx) = CDbl(0) Then
                r.x = 0
                rerrx = 0
            Else
                xsum(temp, mx, 2 * n, r.x, rerrx)
            End If

            '
            ' calculate imaginary part
            '
            mx = 0
            For i = 0 To n - 1
                v = a(i).x * b(i).y
                temp(2 * i + 0) = v
                mx = System.Math.Max(mx, System.Math.Abs(v))
                v = a(i).y * b(i).x
                temp(2 * i + 1) = v
                mx = System.Math.Max(mx, System.Math.Abs(v))
            Next
            If CDbl(mx) = CDbl(0) Then
                r.y = 0
                rerry = 0
            Else
                xsum(temp, mx, 2 * n, r.y, rerry)
            End If

            '
            ' total error
            '
            If CDbl(rerrx) = CDbl(0) AndAlso CDbl(rerry) = CDbl(0) Then
                rerr = 0
            Else
                rerr = System.Math.Max(rerrx, rerry) * System.Math.sqrt(1 + Math.sqr(System.Math.Min(rerrx, rerry) / System.Math.Max(rerrx, rerry)))
            End If
        End Sub


        '************************************************************************
        '        Internal subroutine for extra-precise calculation of SUM(w[i]).
        '
        '        INPUT PARAMETERS:
        '            W   -   array[0..N-1], values to be added
        '                    W is modified during calculations.
        '            MX  -   max(W[i])
        '            N   -   array size
        '            
        '        OUTPUT PARAMETERS:
        '            R   -   SUM(w[i])
        '            RErr-   error estimate for R
        '
        '          -- ALGLIB --
        '             Copyright 24.08.2009 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub xsum(ByRef w As Double(), mx As Double, n As Integer, ByRef r As Double, ByRef rerr As Double)
            Dim i As Integer = 0
            Dim k As Integer = 0
            Dim ks As Integer = 0
            Dim v As Double = 0
            Dim s As Double = 0
            Dim ln2 As Double = 0
            Dim chunk As Double = 0
            Dim invchunk As Double = 0
            Dim allzeros As New Boolean()
            Dim i_ As Integer = 0

            r = 0
            rerr = 0


            '
            ' special cases:
            ' * N=0
            ' * N is too large to use integer arithmetics
            '
            If n = 0 Then
                r = 0
                rerr = 0
                Return
            End If
            If CDbl(mx) = CDbl(0) Then
                r = 0
                rerr = 0
                Return
            End If
            alglib.ap.assert(n < 536870912, "XDot: N is too large!")

            '
            ' Prepare
            '
            ln2 = System.Math.Log(2)
            rerr = mx * Math.machineepsilon

            '
            ' 1. find S such that 0.5<=S*MX<1
            ' 2. multiply W by S, so task is normalized in some sense
            ' 3. S:=1/S so we can obtain original vector multiplying by S
            '
            k = CInt(System.Math.Truncate(System.Math.Round(System.Math.Log(mx) / ln2)))
            s = xfastpow(2, -k)
            While CDbl(s * mx) >= CDbl(1)
                s = 0.5 * s
            End While
            While CDbl(s * mx) < CDbl(0.5)
                s = 2 * s
            End While
            For i_ = 0 To n - 1
                w(i_) = s * w(i_)
            Next
            s = 1 / s

            '
            ' find Chunk=2^M such that N*Chunk<2^29
            '
            ' we have chosen upper limit (2^29) with enough space left
            ' to tolerate possible problems with rounding and N's close
            ' to the limit, so we don't want to be very strict here.
            '
            k = CInt(System.Math.Truncate(System.Math.Log(CDbl(536870912) / CDbl(n)) / ln2))
            chunk = xfastpow(2, k)
            If CDbl(chunk) < CDbl(2) Then
                chunk = 2
            End If
            invchunk = 1 / chunk

            '
            ' calculate result
            '
            r = 0
            For i_ = 0 To n - 1
                w(i_) = chunk * w(i_)
            Next
            While True
                s = s * invchunk
                allzeros = True
                ks = 0
                For i = 0 To n - 1
                    v = w(i)
                    k = CInt(System.Math.Truncate(v))
                    If CDbl(v) <> CDbl(k) Then
                        allzeros = False
                    End If
                    w(i) = chunk * (v - k)
                    ks = ks + k
                Next
                r = r + s * ks
                v = System.Math.Abs(r)
                If allzeros OrElse CDbl(s * n + mx) = CDbl(mx) Then
                    Exit While
                End If
            End While

            '
            ' correct error
            '
            rerr = System.Math.Max(rerr, System.Math.Abs(r) * Math.machineepsilon)
        End Sub


        '************************************************************************
        '        Fast Pow
        '
        '          -- ALGLIB --
        '             Copyright 24.08.2009 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Function xfastpow(r As Double, n As Integer) As Double
            Dim result As Double = 0

            result = 0
            If n > 0 Then
                If n Mod 2 = 0 Then
                    result = Math.sqr(xfastpow(r, n \ 2))
                Else
                    result = r * xfastpow(r, n - 1)
                End If
                Return result
            End If
            If n = 0 Then
                result = 1
            End If
            If n < 0 Then
                result = xfastpow(1 / r, -n)
            End If
            Return result
        End Function


    End Class
    Public Class linmin
        Public Class linminstate
            Inherits apobject
            Public brackt As Boolean
            Public stage1 As Boolean
            Public infoc As Integer
            Public dg As Double
            Public dgm As Double
            Public dginit As Double
            Public dgtest As Double
            Public dgx As Double
            Public dgxm As Double
            Public dgy As Double
            Public dgym As Double
            Public finit As Double
            Public ftest1 As Double
            Public fm As Double
            Public fx As Double
            Public fxm As Double
            Public fy As Double
            Public fym As Double
            Public stx As Double
            Public sty As Double
            Public stmin As Double
            Public stmax As Double
            Public width As Double
            Public width1 As Double
            Public xtrapf As Double
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New linminstate()
                _result.brackt = brackt
                _result.stage1 = stage1
                _result.infoc = infoc
                _result.dg = dg
                _result.dgm = dgm
                _result.dginit = dginit
                _result.dgtest = dgtest
                _result.dgx = dgx
                _result.dgxm = dgxm
                _result.dgy = dgy
                _result.dgym = dgym
                _result.finit = finit
                _result.ftest1 = ftest1
                _result.fm = fm
                _result.fx = fx
                _result.fxm = fxm
                _result.fy = fy
                _result.fym = fym
                _result.stx = stx
                _result.sty = sty
                _result.stmin = stmin
                _result.stmax = stmax
                _result.width = width
                _result.width1 = width1
                _result.xtrapf = xtrapf
                Return _result
            End Function
        End Class


        Public Class armijostate
            Inherits apobject
            Public needf As Boolean
            Public x As Double()
            Public f As Double
            Public n As Integer
            Public xbase As Double()
            Public s As Double()
            Public stplen As Double
            Public fcur As Double
            Public stpmax As Double
            Public fmax As Integer
            Public nfev As Integer
            Public info As Integer
            Public rstate As rcommstate
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
                x = New Double(-1) {}
                xbase = New Double(-1) {}
                s = New Double(-1) {}
                rstate = New rcommstate()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New armijostate()
                _result.needf = needf
                _result.x = DirectCast(x.Clone(), Double())
                _result.f = f
                _result.n = n
                _result.xbase = DirectCast(xbase.Clone(), Double())
                _result.s = DirectCast(s.Clone(), Double())
                _result.stplen = stplen
                _result.fcur = fcur
                _result.stpmax = stpmax
                _result.fmax = fmax
                _result.nfev = nfev
                _result.info = info
                _result.rstate = DirectCast(rstate.make_copy(), rcommstate)
                Return _result
            End Function
        End Class




        Public Const ftol As Double = 0.001
        Public Const xtol As Double = 100 * math.machineepsilon
        Public Const maxfev As Integer = 20
        Public Const stpmin As Double = 1.0E-50
        Public Const defstpmax As Double = 1.0E+50
        Public Const armijofactor As Double = 1.3


        '************************************************************************
        '        Normalizes direction/step pair: makes |D|=1, scales Stp.
        '        If |D|=0, it returns, leavind D/Stp unchanged.
        '
        '          -- ALGLIB --
        '             Copyright 01.04.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linminnormalized(ByRef d As Double(), ByRef stp As Double, n As Integer)
            Dim mx As Double = 0
            Dim s As Double = 0
            Dim i As Integer = 0
            Dim i_ As Integer = 0


            '
            ' first, scale D to avoid underflow/overflow durng squaring
            '
            mx = 0
            For i = 0 To n - 1
                mx = System.Math.Max(mx, System.Math.Abs(d(i)))
            Next
            If CDbl(mx) = CDbl(0) Then
                Return
            End If
            s = 1 / mx
            For i_ = 0 To n - 1
                d(i_) = s * d(i_)
            Next
            stp = stp / s

            '
            ' normalize D
            '
            s = 0.0
            For i_ = 0 To n - 1
                s += d(i_) * d(i_)
            Next
            s = 1 / System.Math.sqrt(s)
            For i_ = 0 To n - 1
                d(i_) = s * d(i_)
            Next
            stp = stp / s
        End Sub


        '************************************************************************
        '        THE  PURPOSE  OF  MCSRCH  IS  TO  FIND A STEP WHICH SATISFIES A SUFFICIENT
        '        DECREASE CONDITION AND A CURVATURE CONDITION.
        '
        '        AT EACH STAGE THE SUBROUTINE  UPDATES  AN  INTERVAL  OF  UNCERTAINTY  WITH
        '        ENDPOINTS  STX  AND  STY.  THE INTERVAL OF UNCERTAINTY IS INITIALLY CHOSEN
        '        SO THAT IT CONTAINS A MINIMIZER OF THE MODIFIED FUNCTION
        '
        '            F(X+STP*S) - F(X) - FTOL*STP*(GRADF(X)'S).
        '
        '        IF  A STEP  IS OBTAINED FOR  WHICH THE MODIFIED FUNCTION HAS A NONPOSITIVE
        '        FUNCTION  VALUE  AND  NONNEGATIVE  DERIVATIVE,   THEN   THE   INTERVAL  OF
        '        UNCERTAINTY IS CHOSEN SO THAT IT CONTAINS A MINIMIZER OF F(X+STP*S).
        '
        '        THE  ALGORITHM  IS  DESIGNED TO FIND A STEP WHICH SATISFIES THE SUFFICIENT
        '        DECREASE CONDITION
        '
        '            F(X+STP*S) .LE. F(X) + FTOL*STP*(GRADF(X)'S),
        '
        '        AND THE CURVATURE CONDITION
        '
        '            ABS(GRADF(X+STP*S)'S)) .LE. GTOL*ABS(GRADF(X)'S).
        '
        '        IF  FTOL  IS  LESS  THAN GTOL AND IF, FOR EXAMPLE, THE FUNCTION IS BOUNDED
        '        BELOW,  THEN  THERE  IS  ALWAYS  A  STEP  WHICH SATISFIES BOTH CONDITIONS.
        '        IF  NO  STEP  CAN BE FOUND  WHICH  SATISFIES  BOTH  CONDITIONS,  THEN  THE
        '        ALGORITHM  USUALLY STOPS  WHEN  ROUNDING ERRORS  PREVENT FURTHER PROGRESS.
        '        IN THIS CASE STP ONLY SATISFIES THE SUFFICIENT DECREASE CONDITION.
        '
        '
        '        :::::::::::::IMPORTANT NOTES:::::::::::::
        '
        '        NOTE 1:
        '
        '        This routine  guarantees that it will stop at the last point where function
        '        value was calculated. It won't make several additional function evaluations
        '        after finding good point. So if you store function evaluations requested by
        '        this routine, you can be sure that last one is the point where we've stopped.
        '
        '        NOTE 2:
        '
        '        when 0<StpMax<StpMin, algorithm will terminate with INFO=5 and Stp=StpMax
        '
        '        NOTE 3:
        '
        '        this algorithm guarantees that, if MCINFO=1 or MCINFO=5, then:
        '        * F(final_point)<F(initial_point) - strict inequality
        '        * final_point<>initial_point - after rounding to machine precision
        '        :::::::::::::::::::::::::::::::::::::::::
        '
        '
        '        PARAMETERS DESCRIPRION
        '
        '        STAGE IS ZERO ON FIRST CALL, ZERO ON FINAL EXIT
        '
        '        N IS A POSITIVE INTEGER INPUT VARIABLE SET TO THE NUMBER OF VARIABLES.
        '
        '        X IS  AN  ARRAY  OF  LENGTH N. ON INPUT IT MUST CONTAIN THE BASE POINT FOR
        '        THE LINE SEARCH. ON OUTPUT IT CONTAINS X+STP*S.
        '
        '        F IS  A  VARIABLE. ON INPUT IT MUST CONTAIN THE VALUE OF F AT X. ON OUTPUT
        '        IT CONTAINS THE VALUE OF F AT X + STP*S.
        '
        '        G IS AN ARRAY OF LENGTH N. ON INPUT IT MUST CONTAIN THE GRADIENT OF F AT X.
        '        ON OUTPUT IT CONTAINS THE GRADIENT OF F AT X + STP*S.
        '
        '        S IS AN INPUT ARRAY OF LENGTH N WHICH SPECIFIES THE SEARCH DIRECTION.
        '
        '        STP  IS  A NONNEGATIVE VARIABLE. ON INPUT STP CONTAINS AN INITIAL ESTIMATE
        '        OF A SATISFACTORY STEP. ON OUTPUT STP CONTAINS THE FINAL ESTIMATE.
        '
        '        FTOL AND GTOL ARE NONNEGATIVE INPUT VARIABLES. TERMINATION OCCURS WHEN THE
        '        SUFFICIENT DECREASE CONDITION AND THE DIRECTIONAL DERIVATIVE CONDITION ARE
        '        SATISFIED.
        '
        '        XTOL IS A NONNEGATIVE INPUT VARIABLE. TERMINATION OCCURS WHEN THE RELATIVE
        '        WIDTH OF THE INTERVAL OF UNCERTAINTY IS AT MOST XTOL.
        '
        '        STPMIN AND STPMAX ARE NONNEGATIVE INPUT VARIABLES WHICH SPECIFY LOWER  AND
        '        UPPER BOUNDS FOR THE STEP.
        '
        '        MAXFEV IS A POSITIVE INTEGER INPUT VARIABLE. TERMINATION OCCURS WHEN THE
        '        NUMBER OF CALLS TO FCN IS AT LEAST MAXFEV BY THE END OF AN ITERATION.
        '
        '        INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
        '            INFO = 0  IMPROPER INPUT PARAMETERS.
        '
        '            INFO = 1  THE SUFFICIENT DECREASE CONDITION AND THE
        '                      DIRECTIONAL DERIVATIVE CONDITION HOLD.
        '
        '            INFO = 2  RELATIVE WIDTH OF THE INTERVAL OF UNCERTAINTY
        '                      IS AT MOST XTOL.
        '
        '            INFO = 3  NUMBER OF CALLS TO FCN HAS REACHED MAXFEV.
        '
        '            INFO = 4  THE STEP IS AT THE LOWER BOUND STPMIN.
        '
        '            INFO = 5  THE STEP IS AT THE UPPER BOUND STPMAX.
        '
        '            INFO = 6  ROUNDING ERRORS PREVENT FURTHER PROGRESS.
        '                      THERE MAY NOT BE A STEP WHICH SATISFIES THE
        '                      SUFFICIENT DECREASE AND CURVATURE CONDITIONS.
        '                      TOLERANCES MAY BE TOO SMALL.
        '
        '        NFEV IS AN INTEGER OUTPUT VARIABLE SET TO THE NUMBER OF CALLS TO FCN.
        '
        '        WA IS A WORK ARRAY OF LENGTH N.
        '
        '        ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
        '        JORGE J. MORE', DAVID J. THUENTE
        '        ************************************************************************

        Public Shared Sub mcsrch(n As Integer, ByRef x As Double(), ByRef f As Double, ByRef g As Double(), s As Double(), ByRef stp As Double, _
            stpmax As Double, gtol As Double, ByRef info As Integer, ByRef nfev As Integer, ByRef wa As Double(), state As linminstate, _
            ByRef stage As Integer)
            Dim i As Integer = 0
            Dim v As Double = 0
            Dim p5 As Double = 0
            Dim p66 As Double = 0
            Dim zero As Double = 0
            Dim i_ As Integer = 0


            '
            ' init
            '
            p5 = 0.5
            p66 = 0.66
            state.xtrapf = 4.0
            zero = 0
            If CDbl(stpmax) = CDbl(0) Then
                stpmax = defstpmax
            End If
            If CDbl(stp) < CDbl(stpmin) Then
                stp = stpmin
            End If
            If CDbl(stp) > CDbl(stpmax) Then
                stp = stpmax
            End If

            '
            ' Main cycle
            '
            While True
                If stage = 0 Then

                    '
                    ' NEXT
                    '
                    stage = 2
                    Continue While
                End If
                If stage = 2 Then
                    state.infoc = 1
                    info = 0

                    '
                    '     CHECK THE INPUT PARAMETERS FOR ERRORS.
                    '
                    If CDbl(stpmax) < CDbl(stpmin) AndAlso CDbl(stpmax) > CDbl(0) Then
                        info = 5
                        stp = stpmax
                        stage = 0
                        Return
                    End If
                    If ((((((n <= 0 OrElse CDbl(stp) <= CDbl(0)) OrElse CDbl(ftol) < CDbl(0)) OrElse CDbl(gtol) < CDbl(zero)) OrElse CDbl(xtol) < CDbl(zero)) OrElse CDbl(stpmin) < CDbl(zero)) OrElse CDbl(stpmax) < CDbl(stpmin)) OrElse maxfev <= 0 Then
                        stage = 0
                        Return
                    End If

                    '
                    '     COMPUTE THE INITIAL GRADIENT IN THE SEARCH DIRECTION
                    '     AND CHECK THAT S IS A DESCENT DIRECTION.
                    '
                    v = 0.0
                    For i_ = 0 To n - 1
                        v += g(i_) * s(i_)
                    Next
                    state.dginit = v
                    If CDbl(state.dginit) >= CDbl(0) Then
                        stage = 0
                        Return
                    End If

                    '
                    '     INITIALIZE LOCAL VARIABLES.
                    '
                    state.brackt = False
                    state.stage1 = True
                    nfev = 0
                    state.finit = f
                    state.dgtest = ftol * state.dginit
                    state.width = stpmax - stpmin
                    state.width1 = state.width / p5
                    For i_ = 0 To n - 1
                        wa(i_) = x(i_)
                    Next

                    '
                    '     THE VARIABLES STX, FX, DGX CONTAIN THE VALUES OF THE STEP,
                    '     FUNCTION, AND DIRECTIONAL DERIVATIVE AT THE BEST STEP.
                    '     THE VARIABLES STY, FY, DGY CONTAIN THE VALUE OF THE STEP,
                    '     FUNCTION, AND DERIVATIVE AT THE OTHER ENDPOINT OF
                    '     THE INTERVAL OF UNCERTAINTY.
                    '     THE VARIABLES STP, F, DG CONTAIN THE VALUES OF THE STEP,
                    '     FUNCTION, AND DERIVATIVE AT THE CURRENT STEP.
                    '
                    state.stx = 0
                    state.fx = state.finit
                    state.dgx = state.dginit
                    state.sty = 0
                    state.fy = state.finit
                    state.dgy = state.dginit

                    '
                    ' NEXT
                    '
                    stage = 3
                    Continue While
                End If
                If stage = 3 Then

                    '
                    '     START OF ITERATION.
                    '
                    '     SET THE MINIMUM AND MAXIMUM STEPS TO CORRESPOND
                    '     TO THE PRESENT INTERVAL OF UNCERTAINTY.
                    '
                    If state.brackt Then
                        If CDbl(state.stx) < CDbl(state.sty) Then
                            state.stmin = state.stx
                            state.stmax = state.sty
                        Else
                            state.stmin = state.sty
                            state.stmax = state.stx
                        End If
                    Else
                        state.stmin = state.stx
                        state.stmax = stp + state.xtrapf * (stp - state.stx)
                    End If

                    '
                    '        FORCE THE STEP TO BE WITHIN THE BOUNDS STPMAX AND STPMIN.
                    '
                    If CDbl(stp) > CDbl(stpmax) Then
                        stp = stpmax
                    End If
                    If CDbl(stp) < CDbl(stpmin) Then
                        stp = stpmin
                    End If

                    '
                    '        IF AN UNUSUAL TERMINATION IS TO OCCUR THEN LET
                    '        STP BE THE LOWEST POINT OBTAINED SO FAR.
                    '
                    If (((state.brackt AndAlso (CDbl(stp) <= CDbl(state.stmin) OrElse CDbl(stp) >= CDbl(state.stmax))) OrElse nfev >= maxfev - 1) OrElse state.infoc = 0) OrElse (state.brackt AndAlso CDbl(state.stmax - state.stmin) <= CDbl(xtol * state.stmax)) Then
                        stp = state.stx
                    End If

                    '
                    '        EVALUATE THE FUNCTION AND GRADIENT AT STP
                    '        AND COMPUTE THE DIRECTIONAL DERIVATIVE.
                    '
                    For i_ = 0 To n - 1
                        x(i_) = wa(i_)
                    Next
                    For i_ = 0 To n - 1
                        x(i_) = x(i_) + stp * s(i_)
                    Next

                    '
                    ' NEXT
                    '
                    stage = 4
                    Return
                End If
                If stage = 4 Then
                    info = 0
                    nfev = nfev + 1
                    v = 0.0
                    For i_ = 0 To n - 1
                        v += g(i_) * s(i_)
                    Next
                    state.dg = v
                    state.ftest1 = state.finit + stp * state.dgtest

                    '
                    '        TEST FOR CONVERGENCE.
                    '
                    If (state.brackt AndAlso (CDbl(stp) <= CDbl(state.stmin) OrElse CDbl(stp) >= CDbl(state.stmax))) OrElse state.infoc = 0 Then
                        info = 6
                    End If
                    If ((CDbl(stp) = CDbl(stpmax) AndAlso CDbl(f) < CDbl(state.finit)) AndAlso CDbl(f) <= CDbl(state.ftest1)) AndAlso CDbl(state.dg) <= CDbl(state.dgtest) Then
                        info = 5
                    End If
                    If CDbl(stp) = CDbl(stpmin) AndAlso ((CDbl(f) >= CDbl(state.finit) OrElse CDbl(f) > CDbl(state.ftest1)) OrElse CDbl(state.dg) >= CDbl(state.dgtest)) Then
                        info = 4
                    End If
                    If nfev >= maxfev Then
                        info = 3
                    End If
                    If state.brackt AndAlso CDbl(state.stmax - state.stmin) <= CDbl(xtol * state.stmax) Then
                        info = 2
                    End If
                    If (CDbl(f) < CDbl(state.finit) AndAlso CDbl(f) <= CDbl(state.ftest1)) AndAlso CDbl(System.Math.Abs(state.dg)) <= CDbl(-(gtol * state.dginit)) Then
                        info = 1
                    End If

                    '
                    '        CHECK FOR TERMINATION.
                    '
                    If info <> 0 Then

                        '
                        ' Check guarantees provided by the function for INFO=1 or INFO=5
                        '
                        If info = 1 OrElse info = 5 Then
                            v = 0.0
                            For i = 0 To n - 1
                                v = v + (wa(i) - x(i)) * (wa(i) - x(i))
                            Next
                            If CDbl(f) >= CDbl(state.finit) OrElse CDbl(v) = CDbl(0.0) Then
                                info = 6
                            End If
                        End If
                        stage = 0
                        Return
                    End If

                    '
                    '        IN THE FIRST STAGE WE SEEK A STEP FOR WHICH THE MODIFIED
                    '        FUNCTION HAS A NONPOSITIVE VALUE AND NONNEGATIVE DERIVATIVE.
                    '
                    If (state.stage1 AndAlso CDbl(f) <= CDbl(state.ftest1)) AndAlso CDbl(state.dg) >= CDbl(System.Math.Min(ftol, gtol) * state.dginit) Then
                        state.stage1 = False
                    End If

                    '
                    '        A MODIFIED FUNCTION IS USED TO PREDICT THE STEP ONLY IF
                    '        WE HAVE NOT OBTAINED A STEP FOR WHICH THE MODIFIED
                    '        FUNCTION HAS A NONPOSITIVE FUNCTION VALUE AND NONNEGATIVE
                    '        DERIVATIVE, AND IF A LOWER FUNCTION VALUE HAS BEEN
                    '        OBTAINED BUT THE DECREASE IS NOT SUFFICIENT.
                    '
                    If (state.stage1 AndAlso CDbl(f) <= CDbl(state.fx)) AndAlso CDbl(f) > CDbl(state.ftest1) Then

                        '
                        '           DEFINE THE MODIFIED FUNCTION AND DERIVATIVE VALUES.
                        '
                        state.fm = f - stp * state.dgtest
                        state.fxm = state.fx - state.stx * state.dgtest
                        state.fym = state.fy - state.sty * state.dgtest
                        state.dgm = state.dg - state.dgtest
                        state.dgxm = state.dgx - state.dgtest
                        state.dgym = state.dgy - state.dgtest

                        '
                        '           CALL CSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        '           AND TO COMPUTE THE NEW STEP.
                        '
                        mcstep(state.stx, state.fxm, state.dgxm, state.sty, state.fym, state.dgym, _
                            stp, state.fm, state.dgm, state.brackt, state.stmin, state.stmax, _
                            state.infoc)

                        '
                        '           RESET THE FUNCTION AND GRADIENT VALUES FOR F.
                        '
                        state.fx = state.fxm + state.stx * state.dgtest
                        state.fy = state.fym + state.sty * state.dgtest
                        state.dgx = state.dgxm + state.dgtest
                        state.dgy = state.dgym + state.dgtest
                    Else

                        '
                        '           CALL MCSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        '           AND TO COMPUTE THE NEW STEP.
                        '
                        mcstep(state.stx, state.fx, state.dgx, state.sty, state.fy, state.dgy, _
                            stp, f, state.dg, state.brackt, state.stmin, state.stmax, _
                            state.infoc)
                    End If

                    '
                    '        FORCE A SUFFICIENT DECREASE IN THE SIZE OF THE
                    '        INTERVAL OF UNCERTAINTY.
                    '
                    If state.brackt Then
                        If CDbl(System.Math.Abs(state.sty - state.stx)) >= CDbl(p66 * state.width1) Then
                            stp = state.stx + p5 * (state.sty - state.stx)
                        End If
                        state.width1 = state.width
                        state.width = System.Math.Abs(state.sty - state.stx)
                    End If

                    '
                    '  NEXT.
                    '
                    stage = 3
                    Continue While
                End If
            End While
        End Sub


        '************************************************************************
        '        These functions perform Armijo line search using  at  most  FMAX  function
        '        evaluations.  It  doesn't  enforce  some  kind  of  " sufficient decrease"
        '        criterion - it just tries different Armijo steps and returns optimum found
        '        so far.
        '
        '        Optimization is done using F-rcomm interface:
        '        * ArmijoCreate initializes State structure
        '          (reusing previously allocated buffers)
        '        * ArmijoIteration is subsequently called
        '        * ArmijoResults returns results
        '
        '        INPUT PARAMETERS:
        '            N       -   problem size
        '            X       -   array[N], starting point
        '            F       -   F(X+S*STP)
        '            S       -   step direction, S>0
        '            STP     -   step length
        '            STPMAX  -   maximum value for STP or zero (if no limit is imposed)
        '            FMAX    -   maximum number of function evaluations
        '            State   -   optimization state
        '
        '          -- ALGLIB --
        '             Copyright 05.10.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub armijocreate(n As Integer, x As Double(), f As Double, s As Double(), stp As Double, stpmax As Double, _
            fmax As Integer, state As armijostate)
            Dim i_ As Integer = 0

            If alglib.ap.len(state.x) < n Then
                state.x = New Double(n - 1) {}
            End If
            If alglib.ap.len(state.xbase) < n Then
                state.xbase = New Double(n - 1) {}
            End If
            If alglib.ap.len(state.s) < n Then
                state.s = New Double(n - 1) {}
            End If
            state.stpmax = stpmax
            state.fmax = fmax
            state.stplen = stp
            state.fcur = f
            state.n = n
            For i_ = 0 To n - 1
                state.xbase(i_) = x(i_)
            Next
            For i_ = 0 To n - 1
                state.s(i_) = s(i_)
            Next
            state.rstate.ia = New Integer(0) {}
            state.rstate.ra = New Double(0) {}
            state.rstate.stage = -1
        End Sub


        '************************************************************************
        '        This is rcomm-based search function
        '
        '          -- ALGLIB --
        '             Copyright 05.10.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function armijoiteration(state As armijostate) As Boolean
            Dim result As New Boolean()
            Dim v As Double = 0
            Dim n As Integer = 0
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
                v = state.rstate.ra(0)
            Else
                n = -983
                v = -989
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
            If state.rstate.stage = 3 Then
                GoTo lbl_3
            End If

            '
            ' Routine body
            '
            If (CDbl(state.stplen) <= CDbl(0) OrElse CDbl(state.stpmax) < CDbl(0)) OrElse state.fmax < 2 Then
                state.info = 0
                result = False
                Return result
            End If
            If CDbl(state.stplen) <= CDbl(stpmin) Then
                state.info = 4
                result = False
                Return result
            End If
            n = state.n
            state.nfev = 0

            '
            ' We always need F
            '
            state.needf = True

            '
            ' Bound StpLen
            '
            If CDbl(state.stplen) > CDbl(state.stpmax) AndAlso CDbl(state.stpmax) <> CDbl(0) Then
                state.stplen = state.stpmax
            End If

            '
            ' Increase length
            '
            v = state.stplen * armijofactor
            If CDbl(v) > CDbl(state.stpmax) AndAlso CDbl(state.stpmax) <> CDbl(0) Then
                v = state.stpmax
            End If
            For i_ = 0 To n - 1
                state.x(i_) = state.xbase(i_)
            Next
            For i_ = 0 To n - 1
                state.x(i_) = state.x(i_) + v * state.s(i_)
            Next
            state.rstate.stage = 0
            GoTo lbl_rcomm
lbl_0:
            state.nfev = state.nfev + 1
            If CDbl(state.f) >= CDbl(state.fcur) Then
                GoTo lbl_4
            End If
            state.stplen = v
            state.fcur = state.f
lbl_6:
            If False Then
                GoTo lbl_7
            End If

            '
            ' test stopping conditions
            '
            If state.nfev >= state.fmax Then
                state.info = 3
                result = False
                Return result
            End If
            If CDbl(state.stplen) >= CDbl(state.stpmax) Then
                state.info = 5
                result = False
                Return result
            End If

            '
            ' evaluate F
            '
            v = state.stplen * armijofactor
            If CDbl(v) > CDbl(state.stpmax) AndAlso CDbl(state.stpmax) <> CDbl(0) Then
                v = state.stpmax
            End If
            For i_ = 0 To n - 1
                state.x(i_) = state.xbase(i_)
            Next
            For i_ = 0 To n - 1
                state.x(i_) = state.x(i_) + v * state.s(i_)
            Next
            state.rstate.stage = 1
            GoTo lbl_rcomm
lbl_1:
            state.nfev = state.nfev + 1

            '
            ' make decision
            '
            If CDbl(state.f) < CDbl(state.fcur) Then
                state.stplen = v
                state.fcur = state.f
            Else
                state.info = 1
                result = False
                Return result
            End If
            GoTo lbl_6
lbl_7:
lbl_4:

            '
            ' Decrease length
            '
            v = state.stplen / armijofactor
            For i_ = 0 To n - 1
                state.x(i_) = state.xbase(i_)
            Next
            For i_ = 0 To n - 1
                state.x(i_) = state.x(i_) + v * state.s(i_)
            Next
            state.rstate.stage = 2
            GoTo lbl_rcomm
lbl_2:
            state.nfev = state.nfev + 1
            If CDbl(state.f) >= CDbl(state.fcur) Then
                GoTo lbl_8
            End If
            state.stplen = state.stplen / armijofactor
            state.fcur = state.f
lbl_10:
            If False Then
                GoTo lbl_11
            End If

            '
            ' test stopping conditions
            '
            If state.nfev >= state.fmax Then
                state.info = 3
                result = False
                Return result
            End If
            If CDbl(state.stplen) <= CDbl(stpmin) Then
                state.info = 4
                result = False
                Return result
            End If

            '
            ' evaluate F
            '
            v = state.stplen / armijofactor
            For i_ = 0 To n - 1
                state.x(i_) = state.xbase(i_)
            Next
            For i_ = 0 To n - 1
                state.x(i_) = state.x(i_) + v * state.s(i_)
            Next
            state.rstate.stage = 3
            GoTo lbl_rcomm
lbl_3:
            state.nfev = state.nfev + 1

            '
            ' make decision
            '
            If CDbl(state.f) < CDbl(state.fcur) Then
                state.stplen = state.stplen / armijofactor
                state.fcur = state.f
            Else
                state.info = 1
                result = False
                Return result
            End If
            GoTo lbl_10
lbl_11:
lbl_8:

            '
            ' Nothing to be done
            '
            state.info = 1
            result = False
            Return result
lbl_rcomm:

            '
            ' Saving state
            '
            result = True
            state.rstate.ia(0) = n
            state.rstate.ra(0) = v
            Return result
        End Function


        '************************************************************************
        '        Results of Armijo search
        '
        '        OUTPUT PARAMETERS:
        '            INFO    -   on output it is set to one of the return codes:
        '                        * 0     improper input params
        '                        * 1     optimum step is found with at most FMAX evaluations
        '                        * 3     FMAX evaluations were used,
        '                                X contains optimum found so far
        '                        * 4     step is at lower bound STPMIN
        '                        * 5     step is at upper bound
        '            STP     -   step length (in case of failure it is still returned)
        '            F       -   function value (in case of failure it is still returned)
        '
        '          -- ALGLIB --
        '             Copyright 05.10.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub armijoresults(state As armijostate, ByRef info As Integer, ByRef stp As Double, ByRef f As Double)
            info = state.info
            stp = state.stplen
            f = state.fcur
        End Sub


        Private Shared Sub mcstep(ByRef stx As Double, ByRef fx As Double, ByRef dx As Double, ByRef sty As Double, ByRef fy As Double, ByRef dy As Double, _
            ByRef stp As Double, fp As Double, dp As Double, ByRef brackt As Boolean, stmin As Double, stmax As Double, _
            ByRef info As Integer)
            Dim bound As New Boolean()
            Dim gamma As Double = 0
            Dim p As Double = 0
            Dim q As Double = 0
            Dim r As Double = 0
            Dim s As Double = 0
            Dim sgnd As Double = 0
            Dim stpc As Double = 0
            Dim stpf As Double = 0
            Dim stpq As Double = 0
            Dim theta As Double = 0

            info = 0

            '
            '     CHECK THE INPUT PARAMETERS FOR ERRORS.
            '
            If ((brackt AndAlso (CDbl(stp) <= CDbl(System.Math.Min(stx, sty)) OrElse CDbl(stp) >= CDbl(System.Math.Max(stx, sty)))) OrElse CDbl(dx * (stp - stx)) >= CDbl(0)) OrElse CDbl(stmax) < CDbl(stmin) Then
                Return
            End If

            '
            '     DETERMINE IF THE DERIVATIVES HAVE OPPOSITE SIGN.
            '
            sgnd = dp * (dx / System.Math.Abs(dx))

            '
            '     FIRST CASE. A HIGHER FUNCTION VALUE.
            '     THE MINIMUM IS BRACKETED. IF THE CUBIC STEP IS CLOSER
            '     TO STX THAN THE QUADRATIC STEP, THE CUBIC STEP IS TAKEN,
            '     ELSE THE AVERAGE OF THE CUBIC AND QUADRATIC STEPS IS TAKEN.
            '
            If CDbl(fp) > CDbl(fx) Then
                info = 1
                bound = True
                theta = 3 * (fx - fp) / (stp - stx) + dx + dp
                s = System.Math.Max(System.Math.Abs(theta), System.Math.Max(System.Math.Abs(dx), System.Math.Abs(dp)))
                gamma = s * System.Math.sqrt(Math.sqr(theta / s) - dx / s * (dp / s))
                If CDbl(stp) < CDbl(stx) Then
                    gamma = -gamma
                End If
                p = gamma - dx + theta
                q = gamma - dx + gamma + dp
                r = p / q
                stpc = stx + r * (stp - stx)
                stpq = stx + dx / ((fx - fp) / (stp - stx) + dx) / 2 * (stp - stx)
                If CDbl(System.Math.Abs(stpc - stx)) < CDbl(System.Math.Abs(stpq - stx)) Then
                    stpf = stpc
                Else
                    stpf = stpc + (stpq - stpc) / 2
                End If
                brackt = True
            Else
                If CDbl(sgnd) < CDbl(0) Then

                    '
                    '     SECOND CASE. A LOWER FUNCTION VALUE AND DERIVATIVES OF
                    '     OPPOSITE SIGN. THE MINIMUM IS BRACKETED. IF THE CUBIC
                    '     STEP IS CLOSER TO STX THAN THE QUADRATIC (SECANT) STEP,
                    '     THE CUBIC STEP IS TAKEN, ELSE THE QUADRATIC STEP IS TAKEN.
                    '
                    info = 2
                    bound = False
                    theta = 3 * (fx - fp) / (stp - stx) + dx + dp
                    s = System.Math.Max(System.Math.Abs(theta), System.Math.Max(System.Math.Abs(dx), System.Math.Abs(dp)))
                    gamma = s * System.Math.sqrt(Math.sqr(theta / s) - dx / s * (dp / s))
                    If CDbl(stp) > CDbl(stx) Then
                        gamma = -gamma
                    End If
                    p = gamma - dp + theta
                    q = gamma - dp + gamma + dx
                    r = p / q
                    stpc = stp + r * (stx - stp)
                    stpq = stp + dp / (dp - dx) * (stx - stp)
                    If CDbl(System.Math.Abs(stpc - stp)) > CDbl(System.Math.Abs(stpq - stp)) Then
                        stpf = stpc
                    Else
                        stpf = stpq
                    End If
                    brackt = True
                Else
                    If CDbl(System.Math.Abs(dp)) < CDbl(System.Math.Abs(dx)) Then

                        '
                        '     THIRD CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        '     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DECREASES.
                        '     THE CUBIC STEP IS ONLY USED IF THE CUBIC TENDS TO INFINITY
                        '     IN THE DIRECTION OF THE STEP OR IF THE MINIMUM OF THE CUBIC
                        '     IS BEYOND STP. OTHERWISE THE CUBIC STEP IS DEFINED TO BE
                        '     EITHER STPMIN OR STPMAX. THE QUADRATIC (SECANT) STEP IS ALSO
                        '     COMPUTED AND IF THE MINIMUM IS BRACKETED THEN THE THE STEP
                        '     CLOSEST TO STX IS TAKEN, ELSE THE STEP FARTHEST AWAY IS TAKEN.
                        '
                        info = 3
                        bound = True
                        theta = 3 * (fx - fp) / (stp - stx) + dx + dp
                        s = System.Math.Max(System.Math.Abs(theta), System.Math.Max(System.Math.Abs(dx), System.Math.Abs(dp)))

                        '
                        '        THE CASE GAMMA = 0 ONLY ARISES IF THE CUBIC DOES NOT TEND
                        '        TO INFINITY IN THE DIRECTION OF THE STEP.
                        '
                        gamma = s * System.Math.sqrt(System.Math.Max(0, Math.sqr(theta / s) - dx / s * (dp / s)))
                        If CDbl(stp) > CDbl(stx) Then
                            gamma = -gamma
                        End If
                        p = gamma - dp + theta
                        q = gamma + (dx - dp) + gamma
                        r = p / q
                        If CDbl(r) < CDbl(0) AndAlso CDbl(gamma) <> CDbl(0) Then
                            stpc = stp + r * (stx - stp)
                        Else
                            If CDbl(stp) > CDbl(stx) Then
                                stpc = stmax
                            Else
                                stpc = stmin
                            End If
                        End If
                        stpq = stp + dp / (dp - dx) * (stx - stp)
                        If brackt Then
                            If CDbl(System.Math.Abs(stp - stpc)) < CDbl(System.Math.Abs(stp - stpq)) Then
                                stpf = stpc
                            Else
                                stpf = stpq
                            End If
                        Else
                            If CDbl(System.Math.Abs(stp - stpc)) > CDbl(System.Math.Abs(stp - stpq)) Then
                                stpf = stpc
                            Else
                                stpf = stpq
                            End If
                        End If
                    Else

                        '
                        '     FOURTH CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        '     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DOES
                        '     NOT DECREASE. IF THE MINIMUM IS NOT BRACKETED, THE STEP
                        '     IS EITHER STPMIN OR STPMAX, ELSE THE CUBIC STEP IS TAKEN.
                        '
                        info = 4
                        bound = False
                        If brackt Then
                            theta = 3 * (fp - fy) / (sty - stp) + dy + dp
                            s = System.Math.Max(System.Math.Abs(theta), System.Math.Max(System.Math.Abs(dy), System.Math.Abs(dp)))
                            gamma = s * System.Math.sqrt(Math.sqr(theta / s) - dy / s * (dp / s))
                            If CDbl(stp) > CDbl(sty) Then
                                gamma = -gamma
                            End If
                            p = gamma - dp + theta
                            q = gamma - dp + gamma + dy
                            r = p / q
                            stpc = stp + r * (sty - stp)
                            stpf = stpc
                        Else
                            If CDbl(stp) > CDbl(stx) Then
                                stpf = stmax
                            Else
                                stpf = stmin
                            End If
                        End If
                    End If
                End If
            End If

            '
            '     UPDATE THE INTERVAL OF UNCERTAINTY. THIS UPDATE DOES NOT
            '     DEPEND ON THE NEW STEP OR THE CASE ANALYSIS ABOVE.
            '
            If CDbl(fp) > CDbl(fx) Then
                sty = stp
                fy = fp
                dy = dp
            Else
                If CDbl(sgnd) < CDbl(0.0) Then
                    sty = stx
                    fy = fx
                    dy = dx
                End If
                stx = stp
                fx = fp
                dx = dp
            End If

            '
            '     COMPUTE THE NEW STEP AND SAFEGUARD IT.
            '
            stpf = System.Math.Min(stmax, stpf)
            stpf = System.Math.Max(stmin, stpf)
            stp = stpf
            If brackt AndAlso bound Then
                If CDbl(sty) > CDbl(stx) Then
                    stp = System.Math.Min(stx + 0.66 * (sty - stx), stp)
                Else
                    stp = System.Math.Max(stx + 0.66 * (sty - stx), stp)
                End If
            End If
        End Sub


    End Class
	Public Class ntheory
		Public Shared Sub findprimitiverootandinverse(n As Integer, ByRef proot As Integer, ByRef invproot As Integer)
			Dim candroot As Integer = 0
			Dim phin As Integer = 0
			Dim q As Integer = 0
			Dim f As Integer = 0
			Dim allnonone As New Boolean()
			Dim x As Integer = 0
			Dim lastx As Integer = 0
			Dim y As Integer = 0
			Dim lasty As Integer = 0
			Dim a As Integer = 0
			Dim b As Integer = 0
			Dim t As Integer = 0
			Dim n2 As Integer = 0

			proot = 0
			invproot = 0

			alglib.ap.assert(n >= 3, "FindPrimitiveRootAndInverse: N<3")
			proot = 0
			invproot = 0

			'
			' check that N is prime
			'
			alglib.ap.assert(isprime(n), "FindPrimitiveRoot: N is not prime")

			'
			' Because N is prime, Euler totient function is equal to N-1
			'
			phin = n - 1

			'
			' Test different values of PRoot - from 2 to N-1.
			' One of these values MUST be primitive root.
			'
			' For testing we use algorithm from Wiki (Primitive root modulo n):
			' * compute phi(N)
			' * determine the different prime factors of phi(N), say p1, ..., pk
			' * for every element m of Zn*, compute m^(phi(N)/pi) mod N for i=1..k
			'   using a fast algorithm for modular exponentiation.
			' * a number m for which these k results are all different from 1 is a
			'   primitive root.
			'
			For candroot = 2 To n - 1

				'
				' We have current candidate root in CandRoot.
				'
				' Scan different prime factors of PhiN. Here:
				' * F is a current candidate factor
				' * Q is a current quotient - amount which was left after dividing PhiN
				'   by all previous factors
				'
				' For each factor, perform test mentioned above.
				'
				q = phin
				f = 2
				allnonone = True
				While q > 1
					If q Mod f = 0 Then
						t = modexp(candroot, phin \ f, n)
						If t = 1 Then
							allnonone = False
							Exit While
						End If
						While q Mod f = 0
							q = q \ f
						End While
					End If
					f = f + 1
				End While
				If allnonone Then
					proot = candroot
					Exit For
				End If
			Next
			alglib.ap.assert(proot >= 2, "FindPrimitiveRoot: internal error (root not found)")

			'
			' Use extended Euclidean algorithm to find multiplicative inverse of primitive root
			'
			x = 0
			lastx = 1
			y = 1
			lasty = 0
			a = proot
			b = n
			While b <> 0
				q = a \ b
				t = a Mod b
				a = b
				b = t
				t = lastx - q * x
				lastx = x
				x = t
				t = lasty - q * y
				lasty = y
				y = t
			End While
			While lastx < 0
				lastx = lastx + n
			End While
			invproot = lastx

			'
			' Check that it is safe to perform multiplication modulo N.
			' Check results for consistency.
			'
			n2 = (n - 1) * (n - 1)
			alglib.ap.assert(n2 \ (n - 1) = n - 1, "FindPrimitiveRoot: internal error")
			alglib.ap.assert(proot * invproot \ proot = invproot, "FindPrimitiveRoot: internal error")
			alglib.ap.assert(proot * invproot \ invproot = proot, "FindPrimitiveRoot: internal error")
			alglib.ap.assert(proot * invproot Mod n = 1, "FindPrimitiveRoot: internal error")
		End Sub


		Private Shared Function isprime(n As Integer) As Boolean
			Dim result As New Boolean()
			Dim p As Integer = 0

			result = False
			p = 2
			While p * p <= n
				If n Mod p = 0 Then
					Return result
				End If
				p = p + 1
			End While
			result = True
			Return result
		End Function


		Private Shared Function modmul(a As Integer, b As Integer, n As Integer) As Integer
			Dim result As Integer = 0
			Dim t As Integer = 0
			Dim ra As Double = 0
			Dim rb As Double = 0

			alglib.ap.assert(a >= 0 AndAlso a < n, "ModMul: A<0 or A>=N")
			alglib.ap.assert(b >= 0 AndAlso b < n, "ModMul: B<0 or B>=N")

			'
			' Base cases
			'
			ra = a
			rb = b
			If b = 0 OrElse a = 0 Then
				result = 0
				Return result
			End If
			If b = 1 OrElse a = 1 Then
				result = a * b
				Return result
			End If
			If CDbl(ra * rb) = CDbl(a * b) Then
				result = a * b Mod n
				Return result
			End If

			'
			' Non-base cases
			'
			If b Mod 2 = 0 Then

				'
				' A*B = (A*(B/2)) * 2
				'
				' Product T=A*(B/2) is calculated recursively, product T*2 is
				' calculated as follows:
				' * result:=T-N
				' * result:=result+T
				' * if result<0 then result:=result+N
				'
				' In case integer result overflows, we generate exception
				'
				t = modmul(a, b \ 2, n)
				result = t - n
				result = result + t
				If result < 0 Then
					result = result + n
				End If
			Else

				'
				' A*B = (A*(B div 2)) * 2 + A
				'
				' Product T=A*(B/2) is calculated recursively, product T*2 is
				' calculated as follows:
				' * result:=T-N
				' * result:=result+T
				' * if result<0 then result:=result+N
				'
				' In case integer result overflows, we generate exception
				'
				t = modmul(a, b \ 2, n)
				result = t - n
				result = result + t
				If result < 0 Then
					result = result + n
				End If
				result = result - n
				result = result + a
				If result < 0 Then
					result = result + n
				End If
			End If
			Return result
		End Function


		Private Shared Function modexp(a As Integer, b As Integer, n As Integer) As Integer
			Dim result As Integer = 0
			Dim t As Integer = 0

			alglib.ap.assert(a >= 0 AndAlso a < n, "ModExp: A<0 or A>=N")
			alglib.ap.assert(b >= 0, "ModExp: B<0")

			'
			' Base cases
			'
			If b = 0 Then
				result = 1
				Return result
			End If
			If b = 1 Then
				result = a
				Return result
			End If

			'
			' Non-base cases
			'
			If b Mod 2 = 0 Then
				t = modmul(a, a, n)
				result = modexp(t, b \ 2, n)
			Else
				t = modmul(a, a, n)
				result = modexp(t, b \ 2, n)
				result = modmul(result, a, n)
			End If
			Return result
		End Function


	End Class
	Public Class ftbase
		'************************************************************************
'        This record stores execution plan for the fast transformation  along  with
'        preallocated temporary buffers and precalculated values.
'
'        FIELDS:
'            Entries         -   plan entries, one row = one entry (see below for
'                                description).
'            Buf0,Buf1,Buf2  -   global temporary buffers; some of them are allocated,
'                                some of them are not (as decided by plan generation
'                                subroutine).
'            Buffer          -   global buffer whose size is equal to plan size.
'                                There is one-to-one correspondence between elements
'                                of global buffer and elements of array transformed.
'                                Because of it global buffer can be used as temporary
'                                thread-safe storage WITHOUT ACQUIRING LOCK - each
'                                worker thread works with its part of input array,
'                                and each part of input array corresponds to distinct
'                                part of buffer.
'            
'        FORMAT OF THE ENTRIES TABLE:
'
'        Entries table is 2D array which stores one entry per row. Row format is:
'            row[0]      operation type:
'                        *  0 for "end of plan/subplan"
'                        * +1 for "reference O(N^2) complex FFT"
'                        * -1 for complex transposition
'                        * -2 for multiplication by twiddle factors of complex FFT
'                        * -3 for "start of plan/subplan"
'            row[1]      repetition count, >=1
'            row[2]      base operand size (number of microvectors), >=1
'            row[3]      microvector size (measured in real numbers), >=1
'            row[4]      parameter0, meaning depends on row[0]
'            row[5]      parameter1, meaning depends on row[0]
'
'        FORMAT OF THE DATA:
'
'        Transformation plan works with row[1]*row[2]*row[3]  real  numbers,  which
'        are (in most cases) interpreted as sequence of complex numbers. These data
'        are grouped as follows:
'        * we have row[1] contiguous OPERANDS, which can be treated separately
'        * each operand includes row[2] contiguous MICROVECTORS
'        * each microvector includes row[3] COMPONENTS, which can be treated separately
'        * pair of components form complex number, so in most cases row[3] will be even
'
'        Say, if you want to perform complex FFT of length 3, then:
'        * you have 1 operand: row[1]=1
'        * operand consists of 3 microvectors:   row[2]=3
'        * each microvector has two components:  row[3]=2
'        * a pair of subsequent components is treated as complex number
'
'        if you want to perform TWO simultaneous complex FFT's of length 3, then you
'        can choose between two representations:
'        * 1 operand, 3 microvectors, 4 components; storage format is given below:
'          [ A0X A0Y B0X B0Y A1X A1Y B1X B1Y ... ]
'          (here A denotes first sequence, B - second one).
'        * 2 operands, 3 microvectors, 2 components; storage format is given below:
'          [ A0X A0Y A1X A2Y ... B0X B0Y B1X B1Y ... ]
'        Most FFT operations are supported only for the second format, but you
'        should remember that first format sometimes can be used too.
'
'        SUPPORTED OPERATIONS:
'
'        row[0]=0:
'        * "end of plan/subplan"
'        * in case we meet entry with such type,  FFT  transformation  is  finished
'          (or we return from recursive FFT subplan, in case it was subplan).
'
'        row[0]=+1:
'        * "reference 1D complex FFT"
'        * we perform reference O(N^2) complex FFT on input data, which are treated
'          as row[1] arrays, each of row[2] complex numbers, and row[3] must be
'          equal to 2
'        * transformation is performed using temporary buffer
'
'        row[0]=opBluesteinsFFT:
'        * input array is handled with Bluestein's algorithm (by zero-padding to
'          Param0 complex numbers).
'        * this plan calls Param0-point subplan which is located at offset Param1
'          (offset is measured with respect to location of the calling entry)
'        * this plan uses precomputed quantities stored in Plan.PrecR at
'          offset Param2.
'        * transformation is performed using 4 temporary buffers, which are
'          retrieved from Plan.BluesteinPool.
'
'        row[0]=+3:
'        * "optimized 1D complex FFT"
'        * this function supports only several operand sizes: from 1 to 5.
'          These transforms are hard-coded and performed very efficiently
'
'        row[0]=opRadersFFT:
'        * input array is handled with Rader's algorithm (permutation and
'          reduction to N-1-point FFT)
'        * this plan calls N-1-point subplan which is located at offset Param0
'          (offset is measured with respect to location of the calling entry)
'        * this plan uses precomputed primitive root and its inverse (modulo N)
'          which are stored in Param1 and Param2.
'        * Param3 stores offset of the precomputed data for the plan
'        * plan length must be prime, (N-1)*(N-1) must fit into integer variable
'
'        row[0]=-1
'        * "complex transposition"
'        * input data are treated as row[1] independent arrays, which are processed
'          separately
'        * each of operands is treated as matrix with row[4] rows and row[2]/row[4]
'          columns. Each element of the matrix is microvector with row[3] components.
'        * transposition is performed using temporary buffer
'
'        row[0]=-2
'        * "multiplication by twiddle factors of complex FFT"
'        * input data are treated as row[1] independent arrays, which are processed
'          separately
'        * row[4] contains N1 - length of the "first FFT"  in  a  Cooley-Tukey  FFT
'          algorithm
'        * this function does not require temporary buffers
'
'        row[0]=-3
'        * "start of the plan"
'        * each subplan must start from this entry
'        * param0 is ignored
'        * param1 stores approximate (optimistic) estimate of KFLOPs required to
'          transform one operand of the plan. Total cost of the plan is approximately
'          equal to row[1]*param1 KFLOPs.
'        * this function does not require temporary buffers
'
'        row[0]=-4
'        * "jump"
'        * param0 stores relative offset of the jump site
'          (+1 corresponds to the next entry)
'
'        row[0]=-5
'        * "parallel call"
'        * input data are treated as row[1] independent arrays
'        * child subplan is applied independently for each of arrays - row[1] times
'        * subplan length must be equal to row[2]*row[3]
'        * param0 stores relative offset of the child subplan site
'          (+1 corresponds to the next entry)
'        * param1 stores approximate total cost of plan, measured in UNITS
'          (1 UNIT = 100 KFLOPs). Plan cost must be rounded DOWN to nearest integer.
'
'
'            
'        TODO 
'             2. from KFLOPs to UNITs, 1 UNIT = 100 000 FLOP!!!!!!!!!!!
'
'             3. from IsRoot to TaskType = {0, -1, +1}; or maybe, add IsSeparatePlan
'                to distinguish root of child subplan from global root which uses
'                separate buffer
'                
'             4. child subplans in parallel call must NOT use buffer provided by parent plan;
'                they must allocate their own local buffer
'        ************************************************************************

		Public Class fasttransformplan
			Inherits apobject
			Public entries As Integer(,)
			Public buffer As Double()
			Public precr As Double()
			Public preci As Double()
			Public bluesteinpool As alglib.smp.shared_pool
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
				entries = New Integer(-1, -1) {}
				buffer = New Double(-1) {}
				precr = New Double(-1) {}
				preci = New Double(-1) {}
				bluesteinpool = New alglib.smp.shared_pool()
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New fasttransformplan()
				_result.entries = DirectCast(entries.Clone(), Integer(,))
				_result.buffer = DirectCast(buffer.Clone(), Double())
				_result.precr = DirectCast(precr.Clone(), Double())
				_result.preci = DirectCast(preci.Clone(), Double())
				_result.bluesteinpool = DirectCast(bluesteinpool.make_copy(), alglib.smp.shared_pool)
				Return _result
			End Function
		End Class




		Public Const coltype As Integer = 0
		Public Const coloperandscnt As Integer = 1
		Public Const coloperandsize As Integer = 2
		Public Const colmicrovectorsize As Integer = 3
		Public Const colparam0 As Integer = 4
		Public Const colparam1 As Integer = 5
		Public Const colparam2 As Integer = 6
		Public Const colparam3 As Integer = 7
		Public Const colscnt As Integer = 8
		Public Const opend As Integer = 0
		Public Const opcomplexreffft As Integer = 1
		Public Const opbluesteinsfft As Integer = 2
		Public Const opcomplexcodeletfft As Integer = 3
		Public Const opcomplexcodelettwfft As Integer = 4
		Public Const opradersfft As Integer = 5
		Public Const opcomplextranspose As Integer = -1
		Public Const opcomplexfftfactors As Integer = -2
		Public Const opstart As Integer = -3
		Public Const opjmp As Integer = -4
		Public Const opparallelcall As Integer = -5
		Public Const maxradix As Integer = 6
		Public Const updatetw As Integer = 16
		Public Const recursivethreshold As Integer = 1024
		Public Const raderthreshold As Integer = 19
		Public Const ftbasecodeletrecommended As Integer = 5
		Public Const ftbaseinefficiencyfactor As Double = 1.3
		Public Const ftbasemaxsmoothfactor As Integer = 5


		'************************************************************************
'        This subroutine generates FFT plan for K complex FFT's with length N each.
'
'        INPUT PARAMETERS:
'            N           -   FFT length (in complex numbers), N>=1
'            K           -   number of repetitions, K>=1
'            
'        OUTPUT PARAMETERS:
'            Plan        -   plan
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub ftcomplexfftplan(n As Integer, k As Integer, plan As fasttransformplan)
			Dim bluesteinbuf As New apserv.srealarray()
			Dim rowptr As Integer = 0
			Dim bluesteinsize As Integer = 0
			Dim precrptr As Integer = 0
			Dim preciptr As Integer = 0
			Dim precrsize As Integer = 0
			Dim precisize As Integer = 0


			'
			' Initial check for parameters
			'
			alglib.ap.assert(n > 0, "FTComplexFFTPlan: N<=0")
			alglib.ap.assert(k > 0, "FTComplexFFTPlan: K<=0")

			'
			' Determine required sizes of precomputed real and integer
			' buffers. This stage of code is highly dependent on internals
			' of FTComplexFFTPlanRec() and must be kept synchronized with
			' possible changes in internals of plan generation function.
			'
			' Buffer size is determined as follows:
			' * N is factorized
			' * we factor out anything which is less or equal to MaxRadix
			' * prime factor F>RaderThreshold requires 4*FTBaseFindSmooth(2*F-1)
			'   real entries to store precomputed Quantities for Bluestein's
			'   transformation
			' * prime factor F<=RaderThreshold does NOT require
			'   precomputed storage
			'
			precrsize = 0
			precisize = 0
			ftdeterminespacerequirements(n, precrsize, precisize)
			If precrsize > 0 Then
				plan.precr = New Double(precrsize - 1) {}
			End If
			If precisize > 0 Then
				plan.preci = New Double(precisize - 1) {}
			End If

			'
			' Generate plan
			'
			rowptr = 0
			precrptr = 0
			preciptr = 0
			bluesteinsize = 1
			plan.buffer = New Double(2 * n * k - 1) {}
			ftcomplexfftplanrec(n, k, True, True, rowptr, bluesteinsize, _
				precrptr, preciptr, plan)
			bluesteinbuf.val = New Double(bluesteinsize - 1) {}
			alglib.smp.ae_shared_pool_set_seed(plan.bluesteinpool, bluesteinbuf)

			'
			' Check that actual amount of precomputed space used by transformation
			' plan is EXACTLY equal to amount of space allocated by us.
			'
			alglib.ap.assert(precrptr = precrsize, "FTComplexFFTPlan: internal error (PrecRPtr<>PrecRSize)")
			alglib.ap.assert(preciptr = precisize, "FTComplexFFTPlan: internal error (PrecRPtr<>PrecRSize)")
		End Sub


		'************************************************************************
'        This subroutine applies transformation plan to input/output array A.
'
'        INPUT PARAMETERS:
'            Plan        -   transformation plan
'            A           -   array, must be large enough for plan to work
'            OffsA       -   offset of the subarray to process
'            RepCnt      -   repetition count (transformation is repeatedly applied
'                            to subsequent subarrays)
'            
'        OUTPUT PARAMETERS:
'            Plan        -   plan (temporary buffers can be modified, plan itself
'                            is unchanged and can be reused)
'            A           -   transformed array
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub ftapplyplan(plan As fasttransformplan, a As Double(), offsa As Integer, repcnt As Integer)
			Dim plansize As Integer = 0
			Dim i As Integer = 0

			plansize = plan.entries(0, coloperandscnt) * plan.entries(0, coloperandsize) * plan.entries(0, colmicrovectorsize)
			For i = 0 To repcnt - 1
				ftapplysubplan(plan, 0, a, offsa + plansize * i, 0, plan.buffer, _
					1)
			Next
		End Sub


		'************************************************************************
'        Returns good factorization N=N1*N2.
'
'        Usually N1<=N2 (but not always - small N's may be exception).
'        if N1<>1 then N2<>1.
'
'        Factorization is chosen depending on task type and codelets we have.
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub ftbasefactorize(n As Integer, tasktype As Integer, ByRef n1 As Integer, ByRef n2 As Integer)
			Dim j As Integer = 0

			n1 = 0
			n2 = 0

			n1 = 0
			n2 = 0

			'
			' try to find good codelet
			'
			If n1 * n2 <> n Then
				For j = ftbasecodeletrecommended To 2 Step -1
					If n Mod j = 0 Then
						n1 = j
						n2 = n \ j
						Exit For
					End If
				Next
			End If

			'
			' try to factorize N
			'
			If n1 * n2 <> n Then
				For j = ftbasecodeletrecommended + 1 To n - 1
					If n Mod j = 0 Then
						n1 = j
						n2 = n \ j
						Exit For
					End If
				Next
			End If

			'
			' looks like N is prime :(
			'
			If n1 * n2 <> n Then
				n1 = 1
				n2 = n
			End If

			'
			' normalize
			'
			If n2 = 1 AndAlso n1 <> 1 Then
				n2 = n1
				n1 = 1
			End If
		End Sub


		'************************************************************************
'        Is number smooth?
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function ftbaseissmooth(n As Integer) As Boolean
			Dim result As New Boolean()
			Dim i As Integer = 0

			For i = 2 To ftbasemaxsmoothfactor
				While n Mod i = 0
					n = n \ i
				End While
			Next
			result = n = 1
			Return result
		End Function


		'************************************************************************
'        Returns smallest smooth (divisible only by 2, 3, 5) number that is greater
'        than or equal to max(N,2)
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function ftbasefindsmooth(n As Integer) As Integer
			Dim result As Integer = 0
			Dim best As Integer = 0

			best = 2
			While best < n
				best = 2 * best
			End While
			ftbasefindsmoothrec(n, 1, 2, best)
			result = best
			Return result
		End Function


		'************************************************************************
'        Returns  smallest  smooth  (divisible only by 2, 3, 5) even number that is
'        greater than or equal to max(N,2)
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function ftbasefindsmootheven(n As Integer) As Integer
			Dim result As Integer = 0
			Dim best As Integer = 0

			best = 2
			While best < n
				best = 2 * best
			End While
			ftbasefindsmoothrec(n, 2, 2, best)
			result = best
			Return result
		End Function


		'************************************************************************
'        Returns estimate of FLOP count for the FFT.
'
'        It is only an estimate based on operations count for the PERFECT FFT
'        and relative inefficiency of the algorithm actually used.
'
'        N should be power of 2, estimates are badly wrong for non-power-of-2 N's.
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Function ftbasegetflopestimate(n As Integer) As Double
			Dim result As Double = 0

			result = ftbaseinefficiencyfactor * (4 * n * System.Math.Log(n) / System.Math.Log(2) - 6 * n + 8)
			Return result
		End Function


		'************************************************************************
'        This function returns EXACT estimate of the space requirements for N-point
'        FFT. Internals of this function are highly dependent on details of different
'        FFTs employed by this unit, so every time algorithm is changed this function
'        has to be rewritten.
'
'        INPUT PARAMETERS:
'            N           -   transform length
'            PrecRSize   -   must be set to zero
'            PrecISize   -   must be set to zero
'            
'        OUTPUT PARAMETERS:
'            PrecRSize   -   number of real temporaries required for transformation
'            PrecISize   -   number of integer temporaries required for transformation
'
'            
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftdeterminespacerequirements(n As Integer, ByRef precrsize As Integer, ByRef precisize As Integer)
			Dim ncur As Integer = 0
			Dim f As Integer = 0
			Dim i As Integer = 0


			'
			' Determine required sizes of precomputed real and integer
			' buffers. This stage of code is highly dependent on internals
			' of FTComplexFFTPlanRec() and must be kept synchronized with
			' possible changes in internals of plan generation function.
			'
			' Buffer size is determined as follows:
			' * N is factorized
			' * we factor out anything which is less or equal to MaxRadix
			' * prime factor F>RaderThreshold requires 4*FTBaseFindSmooth(2*F-1)
			'   real entries to store precomputed Quantities for Bluestein's
			'   transformation
			' * prime factor F<=RaderThreshold requires 2*(F-1)+ESTIMATE(F-1)
			'   precomputed storage
			'
			ncur = n
			For i = 2 To maxradix
				While ncur Mod i = 0
					ncur = ncur \ i
				End While
			Next
			f = 2
			While f <= ncur
				While ncur Mod f = 0
					If f > raderthreshold Then
						precrsize = precrsize + 4 * ftbasefindsmooth(2 * f - 1)
					Else
						precrsize = precrsize + 2 * (f - 1)
						ftdeterminespacerequirements(f - 1, precrsize, precisize)
					End If
					ncur = ncur \ f
				End While
				f = f + 1
			End While
		End Sub


		'************************************************************************
'        Recurrent function called by FTComplexFFTPlan() and other functions. It
'        recursively builds transformation plan
'
'        INPUT PARAMETERS:
'            N           -   FFT length (in complex numbers), N>=1
'            K           -   number of repetitions, K>=1
'            ChildPlan   -   if True, plan generator inserts OpStart/opEnd in the
'                            plan header/footer.
'            TopmostPlan -   if True, plan generator assumes that it is topmost plan:
'                            * it may use global buffer for transpositions
'                            and there is no other plan which executes in parallel
'            RowPtr      -   index which points to past-the-last entry generated so far
'            BluesteinSize-  amount of storage (in real numbers) required for Bluestein buffer
'            PrecRPtr    -   pointer to unused part of precomputed real buffer (Plan.PrecR):
'                            * when this function stores some data to precomputed buffer,
'                              it advances pointer.
'                            * it is responsibility of the function to assert that
'                              Plan.PrecR has enough space to store data before actually
'                              writing to buffer.
'                            * it is responsibility of the caller to allocate enough
'                              space before calling this function
'            PrecIPtr    -   pointer to unused part of precomputed integer buffer (Plan.PrecI):
'                            * when this function stores some data to precomputed buffer,
'                              it advances pointer.
'                            * it is responsibility of the function to assert that
'                              Plan.PrecR has enough space to store data before actually
'                              writing to buffer.
'                            * it is responsibility of the caller to allocate enough
'                              space before calling this function
'            Plan        -   plan (generated so far)
'            
'        OUTPUT PARAMETERS:
'            RowPtr      -   updated pointer (advanced by number of entries generated
'                            by function)
'            BluesteinSize-  updated amount
'                            (may be increased, but may never be decreased)
'                
'        NOTE: in case TopmostPlan is True, ChildPlan is also must be True.
'            
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftcomplexfftplanrec(n As Integer, k As Integer, childplan As Boolean, topmostplan As Boolean, ByRef rowptr As Integer, ByRef bluesteinsize As Integer, _
			ByRef precrptr As Integer, ByRef preciptr As Integer, plan As fasttransformplan)
			Dim localbuf As New apserv.srealarray()
			Dim m As Integer = 0
			Dim n1 As Integer = 0
			Dim n2 As Integer = 0
			Dim gq As Integer = 0
			Dim giq As Integer = 0
			Dim row0 As Integer = 0
			Dim row1 As Integer = 0
			Dim row2 As Integer = 0
			Dim row3 As Integer = 0

			alglib.ap.assert(n > 0, "FTComplexFFTPlan: N<=0")
			alglib.ap.assert(k > 0, "FTComplexFFTPlan: K<=0")
			alglib.ap.assert(Not topmostplan OrElse childplan, "FTComplexFFTPlan: ChildPlan is inconsistent with TopmostPlan")

			'
			' Try to generate "topmost" plan
			'
			If topmostplan AndAlso n > recursivethreshold Then
				ftfactorize(n, False, n1, n2)
				If n1 * n2 = 0 Then

					'
					' Handle prime-factor FFT with Bluestein's FFT.
					' Determine size of Bluestein's buffer.
					'
					m = ftbasefindsmooth(2 * n - 1)
					bluesteinsize = System.Math.Max(2 * m, bluesteinsize)

					'
					' Generate plan
					'
					ftpushentry2(plan, rowptr, opstart, k, n, 2, _
						-1, ftoptimisticestimate(n))
					ftpushentry4(plan, rowptr, opbluesteinsfft, k, n, 2, _
						m, 2, precrptr, 0)
					row0 = rowptr
					ftpushentry(plan, rowptr, opjmp, 0, 0, 0, _
						0)
					ftcomplexfftplanrec(m, 1, True, True, rowptr, bluesteinsize, _
						precrptr, preciptr, plan)
					row1 = rowptr
					plan.entries(row0, colparam0) = row1 - row0
					ftpushentry(plan, rowptr, opend, k, n, 2, _
						0)

					'
					' Fill precomputed buffer
					'
					ftprecomputebluesteinsfft(n, m, plan.precr, precrptr)

					'
					' Update pointer to the precomputed area
					'
					precrptr = precrptr + 4 * m
				Else

					'
					' Handle composite FFT with recursive Cooley-Tukey which
					' uses global buffer instead of local one.
					'
					ftpushentry2(plan, rowptr, opstart, k, n, 2, _
						-1, ftoptimisticestimate(n))
					ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
						n1)
					row0 = rowptr
					ftpushentry2(plan, rowptr, opparallelcall, k * n2, n1, 2, _
						0, ftoptimisticestimate(n))
					ftpushentry(plan, rowptr, opcomplexfftfactors, k, n, 2, _
						n1)
					ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
						n2)
					row2 = rowptr
					ftpushentry2(plan, rowptr, opparallelcall, k * n1, n2, 2, _
						0, ftoptimisticestimate(n))
					ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
						n1)
					ftpushentry(plan, rowptr, opend, k, n, 2, _
						0)
					row1 = rowptr
					ftcomplexfftplanrec(n1, 1, True, False, rowptr, bluesteinsize, _
						precrptr, preciptr, plan)
					plan.entries(row0, colparam0) = row1 - row0
					row3 = rowptr
					ftcomplexfftplanrec(n2, 1, True, False, rowptr, bluesteinsize, _
						precrptr, preciptr, plan)
					plan.entries(row2, colparam0) = row3 - row2
				End If
				Return
			End If

			'
			' Prepare "non-topmost" plan:
			' * calculate factorization
			' * use local (shared) buffer
			' * update buffer size - ANY plan will need at least
			'   2*N temporaries, additional requirements can be
			'   applied later
			'
			ftfactorize(n, False, n1, n2)

			'
			' Handle FFT's with N1*N2=0: either small-N or prime-factor
			'
			If n1 * n2 = 0 Then
				If n <= maxradix Then

					'
					' Small-N FFT
					'
					If childplan Then
						ftpushentry2(plan, rowptr, opstart, k, n, 2, _
							-1, ftoptimisticestimate(n))
					End If
					ftpushentry(plan, rowptr, opcomplexcodeletfft, k, n, 2, _
						0)
					If childplan Then
						ftpushentry(plan, rowptr, opend, k, n, 2, _
							0)
					End If
					Return
				End If
				If n <= raderthreshold Then

					'
					' Handle prime-factor FFT's with Rader's FFT
					'
					m = n - 1
					If childplan Then
						ftpushentry2(plan, rowptr, opstart, k, n, 2, _
							-1, ftoptimisticestimate(n))
					End If
					ntheory.findprimitiverootandinverse(n, gq, giq)
					ftpushentry4(plan, rowptr, opradersfft, k, n, 2, _
						2, gq, giq, precrptr)
					ftprecomputeradersfft(n, gq, giq, plan.precr, precrptr)
					precrptr = precrptr + 2 * (n - 1)
					row0 = rowptr
					ftpushentry(plan, rowptr, opjmp, 0, 0, 0, _
						0)
					ftcomplexfftplanrec(m, 1, True, False, rowptr, bluesteinsize, _
						precrptr, preciptr, plan)
					row1 = rowptr
					plan.entries(row0, colparam0) = row1 - row0
					If childplan Then
						ftpushentry(plan, rowptr, opend, k, n, 2, _
							0)
					End If
				Else

					'
					' Handle prime-factor FFT's with Bluestein's FFT
					'
					m = ftbasefindsmooth(2 * n - 1)
					bluesteinsize = System.Math.Max(2 * m, bluesteinsize)
					If childplan Then
						ftpushentry2(plan, rowptr, opstart, k, n, 2, _
							-1, ftoptimisticestimate(n))
					End If
					ftpushentry4(plan, rowptr, opbluesteinsfft, k, n, 2, _
						m, 2, precrptr, 0)
					ftprecomputebluesteinsfft(n, m, plan.precr, precrptr)
					precrptr = precrptr + 4 * m
					row0 = rowptr
					ftpushentry(plan, rowptr, opjmp, 0, 0, 0, _
						0)
					ftcomplexfftplanrec(m, 1, True, False, rowptr, bluesteinsize, _
						precrptr, preciptr, plan)
					row1 = rowptr
					plan.entries(row0, colparam0) = row1 - row0
					If childplan Then
						ftpushentry(plan, rowptr, opend, k, n, 2, _
							0)
					End If
				End If
				Return
			End If

			'
			' Handle Cooley-Tukey FFT with small N1
			'
			If n1 <= maxradix Then

				'
				' Specialized transformation for small N1:
				' * N2 short inplace FFT's, each N1-point, with integrated twiddle factors
				' * N1 long FFT's
				' * final transposition
				'
				If childplan Then
					ftpushentry2(plan, rowptr, opstart, k, n, 2, _
						-1, ftoptimisticestimate(n))
				End If
				ftpushentry(plan, rowptr, opcomplexcodelettwfft, k, n1, 2 * n2, _
					0)
				ftcomplexfftplanrec(n2, k * n1, False, False, rowptr, bluesteinsize, _
					precrptr, preciptr, plan)
				ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
					n1)
				If childplan Then
					ftpushentry(plan, rowptr, opend, k, n, 2, _
						0)
				End If
				Return
			End If

			'
			' Handle general Cooley-Tukey FFT, either "flat" or "recursive"
			'
			If n <= recursivethreshold Then

				'
				' General code for large N1/N2, "flat" version without explicit recurrence
				' (nested subplans are inserted directly into the body of the plan)
				'
				If childplan Then
					ftpushentry2(plan, rowptr, opstart, k, n, 2, _
						-1, ftoptimisticestimate(n))
				End If
				ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
					n1)
				ftcomplexfftplanrec(n1, k * n2, False, False, rowptr, bluesteinsize, _
					precrptr, preciptr, plan)
				ftpushentry(plan, rowptr, opcomplexfftfactors, k, n, 2, _
					n1)
				ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
					n2)
				ftcomplexfftplanrec(n2, k * n1, False, False, rowptr, bluesteinsize, _
					precrptr, preciptr, plan)
				ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
					n1)
				If childplan Then
					ftpushentry(plan, rowptr, opend, k, n, 2, _
						0)
				End If
			Else

				'
				' General code for large N1/N2, "recursive" version - nested subplans
				' are separated from the plan body.
				'
				' Generate parent plan.
				'
				If childplan Then
					ftpushentry2(plan, rowptr, opstart, k, n, 2, _
						-1, ftoptimisticestimate(n))
				End If
				ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
					n1)
				row0 = rowptr
				ftpushentry2(plan, rowptr, opparallelcall, k * n2, n1, 2, _
					0, ftoptimisticestimate(n))
				ftpushentry(plan, rowptr, opcomplexfftfactors, k, n, 2, _
					n1)
				ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
					n2)
				row2 = rowptr
				ftpushentry2(plan, rowptr, opparallelcall, k * n1, n2, 2, _
					0, ftoptimisticestimate(n))
				ftpushentry(plan, rowptr, opcomplextranspose, k, n, 2, _
					n1)
				If childplan Then
					ftpushentry(plan, rowptr, opend, k, n, 2, _
						0)
				End If

				'
				' Generate child subplans, insert refence to parent plans
				'
				row1 = rowptr
				ftcomplexfftplanrec(n1, 1, True, False, rowptr, bluesteinsize, _
					precrptr, preciptr, plan)
				plan.entries(row0, colparam0) = row1 - row0
				row3 = rowptr
				ftcomplexfftplanrec(n2, 1, True, False, rowptr, bluesteinsize, _
					precrptr, preciptr, plan)
				plan.entries(row2, colparam0) = row3 - row2
			End If
		End Sub


		'************************************************************************
'        This function pushes one more entry to the plan. It resizes Entries matrix
'        if needed.
'
'        INPUT PARAMETERS:
'            Plan        -   plan (generated so far)
'            RowPtr      -   index which points to past-the-last entry generated so far
'            EType       -   entry type
'            EOpCnt      -   operands count
'            EOpSize     -   operand size
'            EMcvSize    -   microvector size
'            EParam0     -   parameter 0
'            
'        OUTPUT PARAMETERS:
'            Plan        -   updated plan    
'            RowPtr      -   updated pointer
'
'        NOTE: Param1 is set to -1.
'            
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftpushentry(plan As fasttransformplan, ByRef rowptr As Integer, etype As Integer, eopcnt As Integer, eopsize As Integer, emcvsize As Integer, _
			eparam0 As Integer)
			ftpushentry2(plan, rowptr, etype, eopcnt, eopsize, emcvsize, _
				eparam0, -1)
		End Sub


		'************************************************************************
'        Same as FTPushEntry(), but sets Param0 AND Param1.
'        This function pushes one more entry to the plan. It resized Entries matrix
'        if needed.
'
'        INPUT PARAMETERS:
'            Plan        -   plan (generated so far)
'            RowPtr      -   index which points to past-the-last entry generated so far
'            EType       -   entry type
'            EOpCnt      -   operands count
'            EOpSize     -   operand size
'            EMcvSize    -   microvector size
'            EParam0     -   parameter 0
'            EParam1     -   parameter 1
'            
'        OUTPUT PARAMETERS:
'            Plan        -   updated plan    
'            RowPtr      -   updated pointer
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftpushentry2(plan As fasttransformplan, ByRef rowptr As Integer, etype As Integer, eopcnt As Integer, eopsize As Integer, emcvsize As Integer, _
			eparam0 As Integer, eparam1 As Integer)
			If rowptr >= alglib.ap.rows(plan.entries) Then
				apserv.imatrixresize(plan.entries, System.Math.Max(2 * alglib.ap.rows(plan.entries), 1), colscnt)
			End If
			plan.entries(rowptr, coltype) = etype
			plan.entries(rowptr, coloperandscnt) = eopcnt
			plan.entries(rowptr, coloperandsize) = eopsize
			plan.entries(rowptr, colmicrovectorsize) = emcvsize
			plan.entries(rowptr, colparam0) = eparam0
			plan.entries(rowptr, colparam1) = eparam1
			plan.entries(rowptr, colparam2) = 0
			plan.entries(rowptr, colparam3) = 0
			rowptr = rowptr + 1
		End Sub


		'************************************************************************
'        Same as FTPushEntry(), but sets Param0, Param1, Param2 and Param3.
'        This function pushes one more entry to the plan. It resized Entries matrix
'        if needed.
'
'        INPUT PARAMETERS:
'            Plan        -   plan (generated so far)
'            RowPtr      -   index which points to past-the-last entry generated so far
'            EType       -   entry type
'            EOpCnt      -   operands count
'            EOpSize     -   operand size
'            EMcvSize    -   microvector size
'            EParam0     -   parameter 0
'            EParam1     -   parameter 1
'            EParam2     -   parameter 2
'            EParam3     -   parameter 3
'            
'        OUTPUT PARAMETERS:
'            Plan        -   updated plan    
'            RowPtr      -   updated pointer
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftpushentry4(plan As fasttransformplan, ByRef rowptr As Integer, etype As Integer, eopcnt As Integer, eopsize As Integer, emcvsize As Integer, _
			eparam0 As Integer, eparam1 As Integer, eparam2 As Integer, eparam3 As Integer)
			If rowptr >= alglib.ap.rows(plan.entries) Then
				apserv.imatrixresize(plan.entries, System.Math.Max(2 * alglib.ap.rows(plan.entries), 1), colscnt)
			End If
			plan.entries(rowptr, coltype) = etype
			plan.entries(rowptr, coloperandscnt) = eopcnt
			plan.entries(rowptr, coloperandsize) = eopsize
			plan.entries(rowptr, colmicrovectorsize) = emcvsize
			plan.entries(rowptr, colparam0) = eparam0
			plan.entries(rowptr, colparam1) = eparam1
			plan.entries(rowptr, colparam2) = eparam2
			plan.entries(rowptr, colparam3) = eparam3
			rowptr = rowptr + 1
		End Sub


		'************************************************************************
'        This subroutine applies subplan to input/output array A.
'
'        INPUT PARAMETERS:
'            Plan        -   transformation plan
'            SubPlan     -   subplan index
'            A           -   array, must be large enough for plan to work
'            ABase       -   base offset in array A, this value points to start of
'                            subarray whose length is equal to length of the plan
'            AOffset     -   offset with respect to ABase, 0<=AOffset<PlanLength.
'                            This is an offset within large PlanLength-subarray of
'                            the chunk to process.
'            Buf         -   temporary buffer whose length is equal to plan length
'                            (without taking into account RepCnt) or larger.
'            OffsBuf     -   offset in the buffer array
'            RepCnt      -   repetition count (transformation is repeatedly applied
'                            to subsequent subarrays)
'            
'        OUTPUT PARAMETERS:
'            Plan        -   plan (temporary buffers can be modified, plan itself
'                            is unchanged and can be reused)
'            A           -   transformed array
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftapplysubplan(plan As fasttransformplan, subplan As Integer, a As Double(), abase As Integer, aoffset As Integer, buf As Double(), _
			repcnt As Integer)
			Dim rowidx As Integer = 0
			Dim i As Integer = 0
			Dim n1 As Integer = 0
			Dim n2 As Integer = 0
			Dim operation As Integer = 0
			Dim operandscnt As Integer = 0
			Dim operandsize As Integer = 0
			Dim microvectorsize As Integer = 0
			Dim param0 As Integer = 0
			Dim param1 As Integer = 0
			Dim parentsize As Integer = 0
			Dim childsize As Integer = 0
			Dim chunksize As Integer = 0
			Dim lastchunksize As Integer = 0
			Dim bufa As apserv.srealarray = Nothing
			Dim bufb As apserv.srealarray = Nothing
			Dim bufc As apserv.srealarray = Nothing
			Dim bufd As apserv.srealarray = Nothing

			alglib.ap.assert(plan.entries(subplan, coltype) = opstart, "FTApplySubPlan: incorrect subplan header")
			rowidx = subplan + 1
			While plan.entries(rowidx, coltype) <> opend
				operation = plan.entries(rowidx, coltype)
				operandscnt = repcnt * plan.entries(rowidx, coloperandscnt)
				operandsize = plan.entries(rowidx, coloperandsize)
				microvectorsize = plan.entries(rowidx, colmicrovectorsize)
				param0 = plan.entries(rowidx, colparam0)
				param1 = plan.entries(rowidx, colparam1)
				apserv.touchint(param1)

				'
				' Process "jump" operation
				'
				If operation = opjmp Then
					rowidx = rowidx + plan.entries(rowidx, colparam0)
					Continue While
				End If

				'
				' Process "parallel call" operation:
				' * we perform initial check for consistency between parent and child plans
				' * we call FTSplitAndApplyParallelPlan(), which splits parallel plan into
				'   several parallel tasks
				'
				If operation = opparallelcall Then
					parentsize = operandsize * microvectorsize
					childsize = plan.entries(rowidx + param0, coloperandscnt) * plan.entries(rowidx + param0, coloperandsize) * plan.entries(rowidx + param0, colmicrovectorsize)
					alglib.ap.assert(plan.entries(rowidx + param0, coltype) = opstart, "FTApplySubPlan: incorrect child subplan header")
					alglib.ap.assert(parentsize = childsize, "FTApplySubPlan: incorrect child subplan header")
					chunksize = System.Math.Max(recursivethreshold \ childsize, 1)
					lastchunksize = operandscnt Mod chunksize
					If lastchunksize = 0 Then
						lastchunksize = chunksize
					End If
					i = 0
					While i < operandscnt
						chunksize = System.Math.Min(chunksize, operandscnt - i)
						ftapplysubplan(plan, rowidx + param0, a, abase, aoffset + i * childsize, buf, _
							chunksize)
						i = i + chunksize
					End While
					rowidx = rowidx + 1
					Continue While
				End If

				'
				' Process "reference complex FFT" operation
				'
				If operation = opcomplexreffft Then
					ftapplycomplexreffft(a, abase + aoffset, operandscnt, operandsize, microvectorsize, buf)
					rowidx = rowidx + 1
					Continue While
				End If

				'
				' Process "codelet FFT" operation
				'
				If operation = opcomplexcodeletfft Then
					ftapplycomplexcodeletfft(a, abase + aoffset, operandscnt, operandsize, microvectorsize)
					rowidx = rowidx + 1
					Continue While
				End If

				'
				' Process "integrated codelet FFT" operation
				'
				If operation = opcomplexcodelettwfft Then
					ftapplycomplexcodelettwfft(a, abase + aoffset, operandscnt, operandsize, microvectorsize)
					rowidx = rowidx + 1
					Continue While
				End If

				'
				' Process Bluestein's FFT operation
				'
				If operation = opbluesteinsfft Then
					alglib.ap.assert(microvectorsize = 2, "FTApplySubPlan: microvectorsize!=2 for Bluesteins FFT")
					alglib.smp.ae_shared_pool_retrieve(plan.bluesteinpool, bufa)
					alglib.smp.ae_shared_pool_retrieve(plan.bluesteinpool, bufb)
					alglib.smp.ae_shared_pool_retrieve(plan.bluesteinpool, bufc)
					alglib.smp.ae_shared_pool_retrieve(plan.bluesteinpool, bufd)
					ftbluesteinsfft(plan, a, abase, aoffset, operandscnt, operandsize, _
						plan.entries(rowidx, colparam0), plan.entries(rowidx, colparam2), rowidx + plan.entries(rowidx, colparam1), bufa.val, bufb.val, bufc.val, _
						bufd.val)
					alglib.smp.ae_shared_pool_recycle(plan.bluesteinpool, bufa)
					alglib.smp.ae_shared_pool_recycle(plan.bluesteinpool, bufb)
					alglib.smp.ae_shared_pool_recycle(plan.bluesteinpool, bufc)
					alglib.smp.ae_shared_pool_recycle(plan.bluesteinpool, bufd)
					rowidx = rowidx + 1
					Continue While
				End If

				'
				' Process Rader's FFT
				'
				If operation = opradersfft Then
					ftradersfft(plan, a, abase, aoffset, operandscnt, operandsize, _
						rowidx + plan.entries(rowidx, colparam0), plan.entries(rowidx, colparam1), plan.entries(rowidx, colparam2), plan.entries(rowidx, colparam3), buf)
					rowidx = rowidx + 1
					Continue While
				End If

				'
				' Process "complex twiddle factors" operation
				'
				If operation = opcomplexfftfactors Then
					alglib.ap.assert(microvectorsize = 2, "FTApplySubPlan: MicrovectorSize<>1")
					n1 = plan.entries(rowidx, colparam0)
					n2 = operandsize \ n1
					For i = 0 To operandscnt - 1
						ffttwcalc(a, abase + aoffset + i * operandsize * 2, n1, n2)
					Next
					rowidx = rowidx + 1
					Continue While
				End If

				'
				' Process "complex transposition" operation
				'
				If operation = opcomplextranspose Then
					alglib.ap.assert(microvectorsize = 2, "FTApplySubPlan: MicrovectorSize<>1")
					n1 = plan.entries(rowidx, colparam0)
					n2 = operandsize \ n1
					For i = 0 To operandscnt - 1
						internalcomplexlintranspose(a, n1, n2, abase + aoffset + i * operandsize * 2, buf)
					Next
					rowidx = rowidx + 1
					Continue While
				End If

				'
				' Error
				'
				alglib.ap.assert(False, "FTApplySubPlan: unexpected plan type")
			End While
		End Sub


		'************************************************************************
'        This subroutine applies complex reference FFT to input/output array A.
'
'        VERY SLOW OPERATION, do not use it in real life plans :)
'
'        INPUT PARAMETERS:
'            A           -   array, must be large enough for plan to work
'            Offs        -   offset of the subarray to process
'            OperandsCnt -   operands count (see description of FastTransformPlan)
'            OperandSize -   operand size (see description of FastTransformPlan)
'            MicrovectorSize-microvector size (see description of FastTransformPlan)
'            Buf         -   temporary array, must be at least OperandsCnt*OperandSize*MicrovectorSize
'            
'        OUTPUT PARAMETERS:
'            A           -   transformed array
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftapplycomplexreffft(a As Double(), offs As Integer, operandscnt As Integer, operandsize As Integer, microvectorsize As Integer, buf As Double())
			Dim opidx As Integer = 0
			Dim i As Integer = 0
			Dim k As Integer = 0
			Dim hre As Double = 0
			Dim him As Double = 0
			Dim c As Double = 0
			Dim s As Double = 0
			Dim re As Double = 0
			Dim im As Double = 0
			Dim n As Integer = 0

			alglib.ap.assert(operandscnt >= 1, "FTApplyComplexRefFFT: OperandsCnt<1")
			alglib.ap.assert(operandsize >= 1, "FTApplyComplexRefFFT: OperandSize<1")
			alglib.ap.assert(microvectorsize = 2, "FTApplyComplexRefFFT: MicrovectorSize<>2")
			n = operandsize
			For opidx = 0 To operandscnt - 1
				For i = 0 To n - 1
					hre = 0
					him = 0
					For k = 0 To n - 1
						re = a(offs + opidx * operandsize * 2 + 2 * k + 0)
						im = a(offs + opidx * operandsize * 2 + 2 * k + 1)
						c = System.Math.Cos(-(2 * System.Math.PI * k * i / n))
						s = System.Math.Sin(-(2 * System.Math.PI * k * i / n))
						hre = hre + c * re - s * im
						him = him + c * im + s * re
					Next
					buf(2 * i + 0) = hre
					buf(2 * i + 1) = him
				Next
				For i = 0 To operandsize * 2 - 1
					a(offs + opidx * operandsize * 2 + i) = buf(i)
				Next
			Next
		End Sub


		'************************************************************************
'        This subroutine applies complex codelet FFT to input/output array A.
'
'        INPUT PARAMETERS:
'            A           -   array, must be large enough for plan to work
'            Offs        -   offset of the subarray to process
'            OperandsCnt -   operands count (see description of FastTransformPlan)
'            OperandSize -   operand size (see description of FastTransformPlan)
'            MicrovectorSize-microvector size, must be 2
'            
'        OUTPUT PARAMETERS:
'            A           -   transformed array
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftapplycomplexcodeletfft(a As Double(), offs As Integer, operandscnt As Integer, operandsize As Integer, microvectorsize As Integer)
			Dim opidx As Integer = 0
			Dim n As Integer = 0
			Dim aoffset As Integer = 0
			Dim a0x As Double = 0
			Dim a0y As Double = 0
			Dim a1x As Double = 0
			Dim a1y As Double = 0
			Dim a2x As Double = 0
			Dim a2y As Double = 0
			Dim a3x As Double = 0
			Dim a3y As Double = 0
			Dim a4x As Double = 0
			Dim a4y As Double = 0
			Dim a5x As Double = 0
			Dim a5y As Double = 0
			Dim v0 As Double = 0
			Dim v1 As Double = 0
			Dim v2 As Double = 0
			Dim v3 As Double = 0
			Dim t1x As Double = 0
			Dim t1y As Double = 0
			Dim t2x As Double = 0
			Dim t2y As Double = 0
			Dim t3x As Double = 0
			Dim t3y As Double = 0
			Dim t4x As Double = 0
			Dim t4y As Double = 0
			Dim t5x As Double = 0
			Dim t5y As Double = 0
			Dim m1x As Double = 0
			Dim m1y As Double = 0
			Dim m2x As Double = 0
			Dim m2y As Double = 0
			Dim m3x As Double = 0
			Dim m3y As Double = 0
			Dim m4x As Double = 0
			Dim m4y As Double = 0
			Dim m5x As Double = 0
			Dim m5y As Double = 0
			Dim s1x As Double = 0
			Dim s1y As Double = 0
			Dim s2x As Double = 0
			Dim s2y As Double = 0
			Dim s3x As Double = 0
			Dim s3y As Double = 0
			Dim s4x As Double = 0
			Dim s4y As Double = 0
			Dim s5x As Double = 0
			Dim s5y As Double = 0
			Dim c1 As Double = 0
			Dim c2 As Double = 0
			Dim c3 As Double = 0
			Dim c4 As Double = 0
			Dim c5 As Double = 0
			Dim v As Double = 0

			alglib.ap.assert(operandscnt >= 1, "FTApplyComplexCodeletFFT: OperandsCnt<1")
			alglib.ap.assert(operandsize >= 1, "FTApplyComplexCodeletFFT: OperandSize<1")
			alglib.ap.assert(microvectorsize = 2, "FTApplyComplexCodeletFFT: MicrovectorSize<>2")
			n = operandsize

			'
			' Hard-coded transforms for different N's
			'
			alglib.ap.assert(n <= maxradix, "FTApplyComplexCodeletFFT: N>MaxRadix")
			If n = 2 Then
				For opidx = 0 To operandscnt - 1
					aoffset = offs + opidx * operandsize * 2
					a0x = a(aoffset + 0)
					a0y = a(aoffset + 1)
					a1x = a(aoffset + 2)
					a1y = a(aoffset + 3)
					v0 = a0x + a1x
					v1 = a0y + a1y
					v2 = a0x - a1x
					v3 = a0y - a1y
					a(aoffset + 0) = v0
					a(aoffset + 1) = v1
					a(aoffset + 2) = v2
					a(aoffset + 3) = v3
				Next
				Return
			End If
			If n = 3 Then
				c1 = System.Math.Cos(2 * System.Math.PI / 3) - 1
				c2 = System.Math.Sin(2 * System.Math.PI / 3)
				For opidx = 0 To operandscnt - 1
					aoffset = offs + opidx * operandsize * 2
					a0x = a(aoffset + 0)
					a0y = a(aoffset + 1)
					a1x = a(aoffset + 2)
					a1y = a(aoffset + 3)
					a2x = a(aoffset + 4)
					a2y = a(aoffset + 5)
					t1x = a1x + a2x
					t1y = a1y + a2y
					a0x = a0x + t1x
					a0y = a0y + t1y
					m1x = c1 * t1x
					m1y = c1 * t1y
					m2x = c2 * (a1y - a2y)
					m2y = c2 * (a2x - a1x)
					s1x = a0x + m1x
					s1y = a0y + m1y
					a1x = s1x + m2x
					a1y = s1y + m2y
					a2x = s1x - m2x
					a2y = s1y - m2y
					a(aoffset + 0) = a0x
					a(aoffset + 1) = a0y
					a(aoffset + 2) = a1x
					a(aoffset + 3) = a1y
					a(aoffset + 4) = a2x
					a(aoffset + 5) = a2y
				Next
				Return
			End If
			If n = 4 Then
				For opidx = 0 To operandscnt - 1
					aoffset = offs + opidx * operandsize * 2
					a0x = a(aoffset + 0)
					a0y = a(aoffset + 1)
					a1x = a(aoffset + 2)
					a1y = a(aoffset + 3)
					a2x = a(aoffset + 4)
					a2y = a(aoffset + 5)
					a3x = a(aoffset + 6)
					a3y = a(aoffset + 7)
					t1x = a0x + a2x
					t1y = a0y + a2y
					t2x = a1x + a3x
					t2y = a1y + a3y
					m2x = a0x - a2x
					m2y = a0y - a2y
					m3x = a1y - a3y
					m3y = a3x - a1x
					a(aoffset + 0) = t1x + t2x
					a(aoffset + 1) = t1y + t2y
					a(aoffset + 4) = t1x - t2x
					a(aoffset + 5) = t1y - t2y
					a(aoffset + 2) = m2x + m3x
					a(aoffset + 3) = m2y + m3y
					a(aoffset + 6) = m2x - m3x
					a(aoffset + 7) = m2y - m3y
				Next
				Return
			End If
			If n = 5 Then
				v = 2 * System.Math.PI / 5
				c1 = (System.Math.Cos(v) + System.Math.Cos(2 * v)) / 2 - 1
				c2 = (System.Math.Cos(v) - System.Math.Cos(2 * v)) / 2
				c3 = -System.Math.Sin(v)
				c4 = -(System.Math.Sin(v) + System.Math.Sin(2 * v))
				c5 = System.Math.Sin(v) - System.Math.Sin(2 * v)
				For opidx = 0 To operandscnt - 1
					aoffset = offs + opidx * operandsize * 2
					t1x = a(aoffset + 2) + a(aoffset + 8)
					t1y = a(aoffset + 3) + a(aoffset + 9)
					t2x = a(aoffset + 4) + a(aoffset + 6)
					t2y = a(aoffset + 5) + a(aoffset + 7)
					t3x = a(aoffset + 2) - a(aoffset + 8)
					t3y = a(aoffset + 3) - a(aoffset + 9)
					t4x = a(aoffset + 6) - a(aoffset + 4)
					t4y = a(aoffset + 7) - a(aoffset + 5)
					t5x = t1x + t2x
					t5y = t1y + t2y
					a(aoffset + 0) = a(aoffset + 0) + t5x
					a(aoffset + 1) = a(aoffset + 1) + t5y
					m1x = c1 * t5x
					m1y = c1 * t5y
					m2x = c2 * (t1x - t2x)
					m2y = c2 * (t1y - t2y)
					m3x = -(c3 * (t3y + t4y))
					m3y = c3 * (t3x + t4x)
					m4x = -(c4 * t4y)
					m4y = c4 * t4x
					m5x = -(c5 * t3y)
					m5y = c5 * t3x
					s3x = m3x - m4x
					s3y = m3y - m4y
					s5x = m3x + m5x
					s5y = m3y + m5y
					s1x = a(aoffset + 0) + m1x
					s1y = a(aoffset + 1) + m1y
					s2x = s1x + m2x
					s2y = s1y + m2y
					s4x = s1x - m2x
					s4y = s1y - m2y
					a(aoffset + 2) = s2x + s3x
					a(aoffset + 3) = s2y + s3y
					a(aoffset + 4) = s4x + s5x
					a(aoffset + 5) = s4y + s5y
					a(aoffset + 6) = s4x - s5x
					a(aoffset + 7) = s4y - s5y
					a(aoffset + 8) = s2x - s3x
					a(aoffset + 9) = s2y - s3y
				Next
				Return
			End If
			If n = 6 Then
				c1 = System.Math.Cos(2 * System.Math.PI / 3) - 1
				c2 = System.Math.Sin(2 * System.Math.PI / 3)
				c3 = System.Math.Cos(-(System.Math.PI / 3))
				c4 = System.Math.Sin(-(System.Math.PI / 3))
				For opidx = 0 To operandscnt - 1
					aoffset = offs + opidx * operandsize * 2
					a0x = a(aoffset + 0)
					a0y = a(aoffset + 1)
					a1x = a(aoffset + 2)
					a1y = a(aoffset + 3)
					a2x = a(aoffset + 4)
					a2y = a(aoffset + 5)
					a3x = a(aoffset + 6)
					a3y = a(aoffset + 7)
					a4x = a(aoffset + 8)
					a4y = a(aoffset + 9)
					a5x = a(aoffset + 10)
					a5y = a(aoffset + 11)
					v0 = a0x
					v1 = a0y
					a0x = a0x + a3x
					a0y = a0y + a3y
					a3x = v0 - a3x
					a3y = v1 - a3y
					v0 = a1x
					v1 = a1y
					a1x = a1x + a4x
					a1y = a1y + a4y
					a4x = v0 - a4x
					a4y = v1 - a4y
					v0 = a2x
					v1 = a2y
					a2x = a2x + a5x
					a2y = a2y + a5y
					a5x = v0 - a5x
					a5y = v1 - a5y
					t4x = a4x * c3 - a4y * c4
					t4y = a4x * c4 + a4y * c3
					a4x = t4x
					a4y = t4y
					t5x = -(a5x * c3) - a5y * c4
					t5y = a5x * c4 - a5y * c3
					a5x = t5x
					a5y = t5y
					t1x = a1x + a2x
					t1y = a1y + a2y
					a0x = a0x + t1x
					a0y = a0y + t1y
					m1x = c1 * t1x
					m1y = c1 * t1y
					m2x = c2 * (a1y - a2y)
					m2y = c2 * (a2x - a1x)
					s1x = a0x + m1x
					s1y = a0y + m1y
					a1x = s1x + m2x
					a1y = s1y + m2y
					a2x = s1x - m2x
					a2y = s1y - m2y
					t1x = a4x + a5x
					t1y = a4y + a5y
					a3x = a3x + t1x
					a3y = a3y + t1y
					m1x = c1 * t1x
					m1y = c1 * t1y
					m2x = c2 * (a4y - a5y)
					m2y = c2 * (a5x - a4x)
					s1x = a3x + m1x
					s1y = a3y + m1y
					a4x = s1x + m2x
					a4y = s1y + m2y
					a5x = s1x - m2x
					a5y = s1y - m2y
					a(aoffset + 0) = a0x
					a(aoffset + 1) = a0y
					a(aoffset + 2) = a3x
					a(aoffset + 3) = a3y
					a(aoffset + 4) = a1x
					a(aoffset + 5) = a1y
					a(aoffset + 6) = a4x
					a(aoffset + 7) = a4y
					a(aoffset + 8) = a2x
					a(aoffset + 9) = a2y
					a(aoffset + 10) = a5x
					a(aoffset + 11) = a5y
				Next
				Return
			End If
		End Sub


		'************************************************************************
'        This subroutine applies complex "integrated" codelet FFT  to  input/output
'        array A. "Integrated" codelet differs from "normal" one in following ways:
'        * it can work with MicrovectorSize>1
'        * hence, it can be used in Cooley-Tukey FFT without transpositions
'        * it performs inlined multiplication by twiddle factors of Cooley-Tukey
'          FFT with N2=MicrovectorSize/2.
'
'        INPUT PARAMETERS:
'            A           -   array, must be large enough for plan to work
'            Offs        -   offset of the subarray to process
'            OperandsCnt -   operands count (see description of FastTransformPlan)
'            OperandSize -   operand size (see description of FastTransformPlan)
'            MicrovectorSize-microvector size, must be 1
'            
'        OUTPUT PARAMETERS:
'            A           -   transformed array
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftapplycomplexcodelettwfft(a As Double(), offs As Integer, operandscnt As Integer, operandsize As Integer, microvectorsize As Integer)
			Dim opidx As Integer = 0
			Dim mvidx As Integer = 0
			Dim n As Integer = 0
			Dim m As Integer = 0
			Dim aoffset0 As Integer = 0
			Dim aoffset2 As Integer = 0
			Dim aoffset4 As Integer = 0
			Dim aoffset6 As Integer = 0
			Dim aoffset8 As Integer = 0
			Dim aoffset10 As Integer = 0
			Dim a0x As Double = 0
			Dim a0y As Double = 0
			Dim a1x As Double = 0
			Dim a1y As Double = 0
			Dim a2x As Double = 0
			Dim a2y As Double = 0
			Dim a3x As Double = 0
			Dim a3y As Double = 0
			Dim a4x As Double = 0
			Dim a4y As Double = 0
			Dim a5x As Double = 0
			Dim a5y As Double = 0
			Dim v0 As Double = 0
			Dim v1 As Double = 0
			Dim v2 As Double = 0
			Dim v3 As Double = 0
			Dim q0x As Double = 0
			Dim q0y As Double = 0
			Dim t1x As Double = 0
			Dim t1y As Double = 0
			Dim t2x As Double = 0
			Dim t2y As Double = 0
			Dim t3x As Double = 0
			Dim t3y As Double = 0
			Dim t4x As Double = 0
			Dim t4y As Double = 0
			Dim t5x As Double = 0
			Dim t5y As Double = 0
			Dim m1x As Double = 0
			Dim m1y As Double = 0
			Dim m2x As Double = 0
			Dim m2y As Double = 0
			Dim m3x As Double = 0
			Dim m3y As Double = 0
			Dim m4x As Double = 0
			Dim m4y As Double = 0
			Dim m5x As Double = 0
			Dim m5y As Double = 0
			Dim s1x As Double = 0
			Dim s1y As Double = 0
			Dim s2x As Double = 0
			Dim s2y As Double = 0
			Dim s3x As Double = 0
			Dim s3y As Double = 0
			Dim s4x As Double = 0
			Dim s4y As Double = 0
			Dim s5x As Double = 0
			Dim s5y As Double = 0
			Dim c1 As Double = 0
			Dim c2 As Double = 0
			Dim c3 As Double = 0
			Dim c4 As Double = 0
			Dim c5 As Double = 0
			Dim v As Double = 0
			Dim tw0 As Double = 0
			Dim tw1 As Double = 0
			Dim twx As Double = 0
			Dim twxm1 As Double = 0
			Dim twy As Double = 0
			Dim tw2x As Double = 0
			Dim tw2y As Double = 0
			Dim tw3x As Double = 0
			Dim tw3y As Double = 0
			Dim tw4x As Double = 0
			Dim tw4y As Double = 0
			Dim tw5x As Double = 0
			Dim tw5y As Double = 0

			alglib.ap.assert(operandscnt >= 1, "FTApplyComplexCodeletFFT: OperandsCnt<1")
			alglib.ap.assert(operandsize >= 1, "FTApplyComplexCodeletFFT: OperandSize<1")
			alglib.ap.assert(microvectorsize >= 1, "FTApplyComplexCodeletFFT: MicrovectorSize<>1")
			alglib.ap.assert(microvectorsize Mod 2 = 0, "FTApplyComplexCodeletFFT: MicrovectorSize is not even")
			n = operandsize
			m = microvectorsize \ 2

			'
			' Hard-coded transforms for different N's
			'
			alglib.ap.assert(n <= maxradix, "FTApplyComplexCodeletTwFFT: N>MaxRadix")
			If n = 2 Then
				v = -(2 * System.Math.PI / (n * m))
				tw0 = -(2 * Math.sqr(System.Math.Sin(0.5 * v)))
				tw1 = System.Math.Sin(v)
				For opidx = 0 To operandscnt - 1
					aoffset0 = offs + opidx * operandsize * microvectorsize
					aoffset2 = aoffset0 + microvectorsize
					twxm1 = 0.0
					twy = 0.0
					For mvidx = 0 To m - 1
						a0x = a(aoffset0)
						a0y = a(aoffset0 + 1)
						a1x = a(aoffset2)
						a1y = a(aoffset2 + 1)
						v0 = a0x + a1x
						v1 = a0y + a1y
						v2 = a0x - a1x
						v3 = a0y - a1y
						a(aoffset0) = v0
						a(aoffset0 + 1) = v1
						a(aoffset2) = v2 * (1 + twxm1) - v3 * twy
						a(aoffset2 + 1) = v3 * (1 + twxm1) + v2 * twy
						aoffset0 = aoffset0 + 2
						aoffset2 = aoffset2 + 2
						If (mvidx + 1) Mod updatetw = 0 Then
							v = -(2 * System.Math.PI * (mvidx + 1) / (n * m))
							twxm1 = System.Math.Sin(0.5 * v)
							twxm1 = -(2 * twxm1 * twxm1)
							twy = System.Math.Sin(v)
						Else
							v = twxm1 + tw0 + twxm1 * tw0 - twy * tw1
							twy = twy + tw1 + twxm1 * tw1 + twy * tw0
							twxm1 = v
						End If
					Next
				Next
				Return
			End If
			If n = 3 Then
				v = -(2 * System.Math.PI / (n * m))
				tw0 = -(2 * Math.sqr(System.Math.Sin(0.5 * v)))
				tw1 = System.Math.Sin(v)
				c1 = System.Math.Cos(2 * System.Math.PI / 3) - 1
				c2 = System.Math.Sin(2 * System.Math.PI / 3)
				For opidx = 0 To operandscnt - 1
					aoffset0 = offs + opidx * operandsize * microvectorsize
					aoffset2 = aoffset0 + microvectorsize
					aoffset4 = aoffset2 + microvectorsize
					twx = 1.0
					twxm1 = 0.0
					twy = 0.0
					For mvidx = 0 To m - 1
						a0x = a(aoffset0)
						a0y = a(aoffset0 + 1)
						a1x = a(aoffset2)
						a1y = a(aoffset2 + 1)
						a2x = a(aoffset4)
						a2y = a(aoffset4 + 1)
						t1x = a1x + a2x
						t1y = a1y + a2y
						a0x = a0x + t1x
						a0y = a0y + t1y
						m1x = c1 * t1x
						m1y = c1 * t1y
						m2x = c2 * (a1y - a2y)
						m2y = c2 * (a2x - a1x)
						s1x = a0x + m1x
						s1y = a0y + m1y
						a1x = s1x + m2x
						a1y = s1y + m2y
						a2x = s1x - m2x
						a2y = s1y - m2y
						tw2x = twx * twx - twy * twy
						tw2y = 2 * twx * twy
						a(aoffset0) = a0x
						a(aoffset0 + 1) = a0y
						a(aoffset2) = a1x * twx - a1y * twy
						a(aoffset2 + 1) = a1y * twx + a1x * twy
						a(aoffset4) = a2x * tw2x - a2y * tw2y
						a(aoffset4 + 1) = a2y * tw2x + a2x * tw2y
						aoffset0 = aoffset0 + 2
						aoffset2 = aoffset2 + 2
						aoffset4 = aoffset4 + 2
						If (mvidx + 1) Mod updatetw = 0 Then
							v = -(2 * System.Math.PI * (mvidx + 1) / (n * m))
							twxm1 = System.Math.Sin(0.5 * v)
							twxm1 = -(2 * twxm1 * twxm1)
							twy = System.Math.Sin(v)
							twx = twxm1 + 1
						Else
							v = twxm1 + tw0 + twxm1 * tw0 - twy * tw1
							twy = twy + tw1 + twxm1 * tw1 + twy * tw0
							twxm1 = v
							twx = v + 1
						End If
					Next
				Next
				Return
			End If
			If n = 4 Then
				v = -(2 * System.Math.PI / (n * m))
				tw0 = -(2 * Math.sqr(System.Math.Sin(0.5 * v)))
				tw1 = System.Math.Sin(v)
				For opidx = 0 To operandscnt - 1
					aoffset0 = offs + opidx * operandsize * microvectorsize
					aoffset2 = aoffset0 + microvectorsize
					aoffset4 = aoffset2 + microvectorsize
					aoffset6 = aoffset4 + microvectorsize
					twx = 1.0
					twxm1 = 0.0
					twy = 0.0
					For mvidx = 0 To m - 1
						a0x = a(aoffset0)
						a0y = a(aoffset0 + 1)
						a1x = a(aoffset2)
						a1y = a(aoffset2 + 1)
						a2x = a(aoffset4)
						a2y = a(aoffset4 + 1)
						a3x = a(aoffset6)
						a3y = a(aoffset6 + 1)
						t1x = a0x + a2x
						t1y = a0y + a2y
						t2x = a1x + a3x
						t2y = a1y + a3y
						m2x = a0x - a2x
						m2y = a0y - a2y
						m3x = a1y - a3y
						m3y = a3x - a1x
						tw2x = twx * twx - twy * twy
						tw2y = 2 * twx * twy
						tw3x = twx * tw2x - twy * tw2y
						tw3y = twx * tw2y + twy * tw2x
						a1x = m2x + m3x
						a1y = m2y + m3y
						a2x = t1x - t2x
						a2y = t1y - t2y
						a3x = m2x - m3x
						a3y = m2y - m3y
						a(aoffset0) = t1x + t2x
						a(aoffset0 + 1) = t1y + t2y
						a(aoffset2) = a1x * twx - a1y * twy
						a(aoffset2 + 1) = a1y * twx + a1x * twy
						a(aoffset4) = a2x * tw2x - a2y * tw2y
						a(aoffset4 + 1) = a2y * tw2x + a2x * tw2y
						a(aoffset6) = a3x * tw3x - a3y * tw3y
						a(aoffset6 + 1) = a3y * tw3x + a3x * tw3y
						aoffset0 = aoffset0 + 2
						aoffset2 = aoffset2 + 2
						aoffset4 = aoffset4 + 2
						aoffset6 = aoffset6 + 2
						If (mvidx + 1) Mod updatetw = 0 Then
							v = -(2 * System.Math.PI * (mvidx + 1) / (n * m))
							twxm1 = System.Math.Sin(0.5 * v)
							twxm1 = -(2 * twxm1 * twxm1)
							twy = System.Math.Sin(v)
							twx = twxm1 + 1
						Else
							v = twxm1 + tw0 + twxm1 * tw0 - twy * tw1
							twy = twy + tw1 + twxm1 * tw1 + twy * tw0
							twxm1 = v
							twx = v + 1
						End If
					Next
				Next
				Return
			End If
			If n = 5 Then
				v = -(2 * System.Math.PI / (n * m))
				tw0 = -(2 * Math.sqr(System.Math.Sin(0.5 * v)))
				tw1 = System.Math.Sin(v)
				v = 2 * System.Math.PI / 5
				c1 = (System.Math.Cos(v) + System.Math.Cos(2 * v)) / 2 - 1
				c2 = (System.Math.Cos(v) - System.Math.Cos(2 * v)) / 2
				c3 = -System.Math.Sin(v)
				c4 = -(System.Math.Sin(v) + System.Math.Sin(2 * v))
				c5 = System.Math.Sin(v) - System.Math.Sin(2 * v)
				For opidx = 0 To operandscnt - 1
					aoffset0 = offs + opidx * operandsize * microvectorsize
					aoffset2 = aoffset0 + microvectorsize
					aoffset4 = aoffset2 + microvectorsize
					aoffset6 = aoffset4 + microvectorsize
					aoffset8 = aoffset6 + microvectorsize
					twx = 1.0
					twxm1 = 0.0
					twy = 0.0
					For mvidx = 0 To m - 1
						a0x = a(aoffset0)
						a0y = a(aoffset0 + 1)
						a1x = a(aoffset2)
						a1y = a(aoffset2 + 1)
						a2x = a(aoffset4)
						a2y = a(aoffset4 + 1)
						a3x = a(aoffset6)
						a3y = a(aoffset6 + 1)
						a4x = a(aoffset8)
						a4y = a(aoffset8 + 1)
						t1x = a1x + a4x
						t1y = a1y + a4y
						t2x = a2x + a3x
						t2y = a2y + a3y
						t3x = a1x - a4x
						t3y = a1y - a4y
						t4x = a3x - a2x
						t4y = a3y - a2y
						t5x = t1x + t2x
						t5y = t1y + t2y
						q0x = a0x + t5x
						q0y = a0y + t5y
						m1x = c1 * t5x
						m1y = c1 * t5y
						m2x = c2 * (t1x - t2x)
						m2y = c2 * (t1y - t2y)
						m3x = -(c3 * (t3y + t4y))
						m3y = c3 * (t3x + t4x)
						m4x = -(c4 * t4y)
						m4y = c4 * t4x
						m5x = -(c5 * t3y)
						m5y = c5 * t3x
						s3x = m3x - m4x
						s3y = m3y - m4y
						s5x = m3x + m5x
						s5y = m3y + m5y
						s1x = q0x + m1x
						s1y = q0y + m1y
						s2x = s1x + m2x
						s2y = s1y + m2y
						s4x = s1x - m2x
						s4y = s1y - m2y
						tw2x = twx * twx - twy * twy
						tw2y = 2 * twx * twy
						tw3x = twx * tw2x - twy * tw2y
						tw3y = twx * tw2y + twy * tw2x
						tw4x = tw2x * tw2x - tw2y * tw2y
						tw4y = tw2x * tw2y + tw2y * tw2x
						a1x = s2x + s3x
						a1y = s2y + s3y
						a2x = s4x + s5x
						a2y = s4y + s5y
						a3x = s4x - s5x
						a3y = s4y - s5y
						a4x = s2x - s3x
						a4y = s2y - s3y
						a(aoffset0) = q0x
						a(aoffset0 + 1) = q0y
						a(aoffset2) = a1x * twx - a1y * twy
						a(aoffset2 + 1) = a1x * twy + a1y * twx
						a(aoffset4) = a2x * tw2x - a2y * tw2y
						a(aoffset4 + 1) = a2x * tw2y + a2y * tw2x
						a(aoffset6) = a3x * tw3x - a3y * tw3y
						a(aoffset6 + 1) = a3x * tw3y + a3y * tw3x
						a(aoffset8) = a4x * tw4x - a4y * tw4y
						a(aoffset8 + 1) = a4x * tw4y + a4y * tw4x
						aoffset0 = aoffset0 + 2
						aoffset2 = aoffset2 + 2
						aoffset4 = aoffset4 + 2
						aoffset6 = aoffset6 + 2
						aoffset8 = aoffset8 + 2
						If (mvidx + 1) Mod updatetw = 0 Then
							v = -(2 * System.Math.PI * (mvidx + 1) / (n * m))
							twxm1 = System.Math.Sin(0.5 * v)
							twxm1 = -(2 * twxm1 * twxm1)
							twy = System.Math.Sin(v)
							twx = twxm1 + 1
						Else
							v = twxm1 + tw0 + twxm1 * tw0 - twy * tw1
							twy = twy + tw1 + twxm1 * tw1 + twy * tw0
							twxm1 = v
							twx = v + 1
						End If
					Next
				Next
				Return
			End If
			If n = 6 Then
				c1 = System.Math.Cos(2 * System.Math.PI / 3) - 1
				c2 = System.Math.Sin(2 * System.Math.PI / 3)
				c3 = System.Math.Cos(-(System.Math.PI / 3))
				c4 = System.Math.Sin(-(System.Math.PI / 3))
				v = -(2 * System.Math.PI / (n * m))
				tw0 = -(2 * Math.sqr(System.Math.Sin(0.5 * v)))
				tw1 = System.Math.Sin(v)
				For opidx = 0 To operandscnt - 1
					aoffset0 = offs + opidx * operandsize * microvectorsize
					aoffset2 = aoffset0 + microvectorsize
					aoffset4 = aoffset2 + microvectorsize
					aoffset6 = aoffset4 + microvectorsize
					aoffset8 = aoffset6 + microvectorsize
					aoffset10 = aoffset8 + microvectorsize
					twx = 1.0
					twxm1 = 0.0
					twy = 0.0
					For mvidx = 0 To m - 1
						a0x = a(aoffset0 + 0)
						a0y = a(aoffset0 + 1)
						a1x = a(aoffset2 + 0)
						a1y = a(aoffset2 + 1)
						a2x = a(aoffset4 + 0)
						a2y = a(aoffset4 + 1)
						a3x = a(aoffset6 + 0)
						a3y = a(aoffset6 + 1)
						a4x = a(aoffset8 + 0)
						a4y = a(aoffset8 + 1)
						a5x = a(aoffset10 + 0)
						a5y = a(aoffset10 + 1)
						v0 = a0x
						v1 = a0y
						a0x = a0x + a3x
						a0y = a0y + a3y
						a3x = v0 - a3x
						a3y = v1 - a3y
						v0 = a1x
						v1 = a1y
						a1x = a1x + a4x
						a1y = a1y + a4y
						a4x = v0 - a4x
						a4y = v1 - a4y
						v0 = a2x
						v1 = a2y
						a2x = a2x + a5x
						a2y = a2y + a5y
						a5x = v0 - a5x
						a5y = v1 - a5y
						t4x = a4x * c3 - a4y * c4
						t4y = a4x * c4 + a4y * c3
						a4x = t4x
						a4y = t4y
						t5x = -(a5x * c3) - a5y * c4
						t5y = a5x * c4 - a5y * c3
						a5x = t5x
						a5y = t5y
						t1x = a1x + a2x
						t1y = a1y + a2y
						a0x = a0x + t1x
						a0y = a0y + t1y
						m1x = c1 * t1x
						m1y = c1 * t1y
						m2x = c2 * (a1y - a2y)
						m2y = c2 * (a2x - a1x)
						s1x = a0x + m1x
						s1y = a0y + m1y
						a1x = s1x + m2x
						a1y = s1y + m2y
						a2x = s1x - m2x
						a2y = s1y - m2y
						t1x = a4x + a5x
						t1y = a4y + a5y
						a3x = a3x + t1x
						a3y = a3y + t1y
						m1x = c1 * t1x
						m1y = c1 * t1y
						m2x = c2 * (a4y - a5y)
						m2y = c2 * (a5x - a4x)
						s1x = a3x + m1x
						s1y = a3y + m1y
						a4x = s1x + m2x
						a4y = s1y + m2y
						a5x = s1x - m2x
						a5y = s1y - m2y
						tw2x = twx * twx - twy * twy
						tw2y = 2 * twx * twy
						tw3x = twx * tw2x - twy * tw2y
						tw3y = twx * tw2y + twy * tw2x
						tw4x = tw2x * tw2x - tw2y * tw2y
						tw4y = 2 * tw2x * tw2y
						tw5x = tw3x * tw2x - tw3y * tw2y
						tw5y = tw3x * tw2y + tw3y * tw2x
						a(aoffset0 + 0) = a0x
						a(aoffset0 + 1) = a0y
						a(aoffset2 + 0) = a3x * twx - a3y * twy
						a(aoffset2 + 1) = a3y * twx + a3x * twy
						a(aoffset4 + 0) = a1x * tw2x - a1y * tw2y
						a(aoffset4 + 1) = a1y * tw2x + a1x * tw2y
						a(aoffset6 + 0) = a4x * tw3x - a4y * tw3y
						a(aoffset6 + 1) = a4y * tw3x + a4x * tw3y
						a(aoffset8 + 0) = a2x * tw4x - a2y * tw4y
						a(aoffset8 + 1) = a2y * tw4x + a2x * tw4y
						a(aoffset10 + 0) = a5x * tw5x - a5y * tw5y
						a(aoffset10 + 1) = a5y * tw5x + a5x * tw5y
						aoffset0 = aoffset0 + 2
						aoffset2 = aoffset2 + 2
						aoffset4 = aoffset4 + 2
						aoffset6 = aoffset6 + 2
						aoffset8 = aoffset8 + 2
						aoffset10 = aoffset10 + 2
						If (mvidx + 1) Mod updatetw = 0 Then
							v = -(2 * System.Math.PI * (mvidx + 1) / (n * m))
							twxm1 = System.Math.Sin(0.5 * v)
							twxm1 = -(2 * twxm1 * twxm1)
							twy = System.Math.Sin(v)
							twx = twxm1 + 1
						Else
							v = twxm1 + tw0 + twxm1 * tw0 - twy * tw1
							twy = twy + tw1 + twxm1 * tw1 + twy * tw0
							twxm1 = v
							twx = v + 1
						End If
					Next
				Next
				Return
			End If
		End Sub


		'************************************************************************
'        This subroutine precomputes data for complex Bluestein's  FFT  and  writes
'        them to array PrecR[] at specified offset. It  is  responsibility  of  the
'        caller to make sure that PrecR[] is large enough.
'
'        INPUT PARAMETERS:
'            N           -   original size of the transform
'            M           -   size of the "padded" Bluestein's transform
'            PrecR       -   preallocated array
'            Offs        -   offset
'            
'        OUTPUT PARAMETERS:
'            PrecR       -   data at Offs:Offs+4*M-1 are modified:
'                            * PrecR[Offs:Offs+2*M-1] stores Z[k]=exp(i*pi*k^2/N)
'                            * PrecR[Offs+2*M:Offs+4*M-1] stores FFT of the Z
'                            Other parts of PrecR are unchanged.
'                            
'        NOTE: this function performs internal M-point FFT. It allocates temporary
'              plan which is destroyed after leaving this function.
'
'          -- ALGLIB --
'             Copyright 08.05.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftprecomputebluesteinsfft(n As Integer, m As Integer, precr As Double(), offs As Integer)
			Dim i As Integer = 0
			Dim bx As Double = 0
			Dim by As Double = 0
			Dim plan As New fasttransformplan()


			'
			' Fill first half of PrecR with b[k] = exp(i*pi*k^2/N)
			'
			For i = 0 To 2 * m - 1
				precr(offs + i) = 0
			Next
			For i = 0 To n - 1
				bx = System.Math.Cos(System.Math.PI / n * i * i)
				by = System.Math.Sin(System.Math.PI / n * i * i)
				precr(offs + 2 * i + 0) = bx
				precr(offs + 2 * i + 1) = by
				precr(offs + 2 * ((m - i) Mod m) + 0) = bx
				precr(offs + 2 * ((m - i) Mod m) + 1) = by
			Next

			'
			' Precomputed FFT
			'
			ftcomplexfftplan(m, 1, plan)
			For i = 0 To 2 * m - 1
				precr(offs + 2 * m + i) = precr(offs + i)
			Next
			ftapplysubplan(plan, 0, precr, offs + 2 * m, 0, plan.buffer, _
				1)
		End Sub


		'************************************************************************
'        This subroutine applies complex Bluestein's FFT to input/output array A.
'
'        INPUT PARAMETERS:
'            Plan        -   transformation plan
'            A           -   array, must be large enough for plan to work
'            ABase       -   base offset in array A, this value points to start of
'                            subarray whose length is equal to length of the plan
'            AOffset     -   offset with respect to ABase, 0<=AOffset<PlanLength.
'                            This is an offset within large PlanLength-subarray of
'                            the chunk to process.
'            OperandsCnt -   number of repeated operands (length N each)
'            N           -   original data length (measured in complex numbers)
'            M           -   padded data length (measured in complex numbers)
'            PrecOffs    -   offset of the precomputed data for the plan
'            SubPlan     -   position of the length-M FFT subplan which is used by
'                            transformation
'            BufA        -   temporary buffer, at least 2*M elements
'            BufB        -   temporary buffer, at least 2*M elements
'            BufC        -   temporary buffer, at least 2*M elements
'            BufD        -   temporary buffer, at least 2*M elements
'            
'        OUTPUT PARAMETERS:
'            A           -   transformed array
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftbluesteinsfft(plan As fasttransformplan, a As Double(), abase As Integer, aoffset As Integer, operandscnt As Integer, n As Integer, _
			m As Integer, precoffs As Integer, subplan As Integer, bufa As Double(), bufb As Double(), bufc As Double(), _
			bufd As Double())
			Dim op As Integer = 0
			Dim i As Integer = 0
			Dim x As Double = 0
			Dim y As Double = 0
			Dim bx As Double = 0
			Dim by As Double = 0
			Dim ax As Double = 0
			Dim ay As Double = 0
			Dim rx As Double = 0
			Dim ry As Double = 0
			Dim p0 As Integer = 0
			Dim p1 As Integer = 0
			Dim p2 As Integer = 0

			For op = 0 To operandscnt - 1

				'
				' Multiply A by conj(Z), store to buffer.
				' Pad A by zeros.
				'
				' NOTE: Z[k]=exp(i*pi*k^2/N)
				'
				p0 = abase + aoffset + op * 2 * n
				p1 = precoffs
				For i = 0 To n - 1
					x = a(p0 + 0)
					y = a(p0 + 1)
					bx = plan.precr(p1 + 0)
					by = -plan.precr(p1 + 1)
					bufa(2 * i + 0) = x * bx - y * by
					bufa(2 * i + 1) = x * by + y * bx
					p0 = p0 + 2
					p1 = p1 + 2
				Next
				For i = 2 * n To 2 * m - 1
					bufa(i) = 0
				Next

				'
				' Perform convolution of A and Z (using precomputed
				' FFT of Z stored in Plan structure).
				'
				ftapplysubplan(plan, subplan, bufa, 0, 0, bufc, _
					1)
				p0 = 0
				p1 = precoffs + 2 * m
				For i = 0 To m - 1
					ax = bufa(p0 + 0)
					ay = bufa(p0 + 1)
					bx = plan.precr(p1 + 0)
					by = plan.precr(p1 + 1)
					bufa(p0 + 0) = ax * bx - ay * by
					bufa(p0 + 1) = -(ax * by + ay * bx)
					p0 = p0 + 2
					p1 = p1 + 2
				Next
				ftapplysubplan(plan, subplan, bufa, 0, 0, bufc, _
					1)

				'
				' Post processing:
				'     A:=conj(Z)*conj(A)/M
				' Here conj(A)/M corresponds to last stage of inverse DFT,
				' and conj(Z) comes from Bluestein's FFT algorithm.
				'
				p0 = precoffs
				p1 = 0
				p2 = abase + aoffset + op * 2 * n
				For i = 0 To n - 1
					bx = plan.precr(p0 + 0)
					by = plan.precr(p0 + 1)
					rx = bufa(p1 + 0) / m
					ry = -(bufa(p1 + 1) / m)
					a(p2 + 0) = rx * bx - ry * -by
					a(p2 + 1) = rx * -by + ry * bx
					p0 = p0 + 2
					p1 = p1 + 2
					p2 = p2 + 2
				Next
			Next
		End Sub


		'************************************************************************
'        This subroutine precomputes data for complex Rader's FFT and  writes  them
'        to array PrecR[] at specified offset. It  is  responsibility of the caller
'        to make sure that PrecR[] is large enough.
'
'        INPUT PARAMETERS:
'            N           -   original size of the transform (before reduction to N-1)
'            RQ          -   primitive root modulo N
'            RIQ         -   inverse of primitive root modulo N
'            PrecR       -   preallocated array
'            Offs        -   offset
'            
'        OUTPUT PARAMETERS:
'            PrecR       -   data at Offs:Offs+2*(N-1)-1 store FFT of Rader's factors,
'                            other parts of PrecR are unchanged.
'                            
'        NOTE: this function performs internal (N-1)-point FFT. It allocates temporary
'              plan which is destroyed after leaving this function.
'
'          -- ALGLIB --
'             Copyright 08.05.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftprecomputeradersfft(n As Integer, rq As Integer, riq As Integer, precr As Double(), offs As Integer)
			Dim q As Integer = 0
			Dim plan As New fasttransformplan()
			Dim kiq As Integer = 0
			Dim v As Double = 0


			'
			' Fill PrecR with Rader factors, perform FFT
			'
			kiq = 1
			For q = 0 To n - 2
				v = -(2 * System.Math.PI * kiq / n)
				precr(offs + 2 * q + 0) = System.Math.Cos(v)
				precr(offs + 2 * q + 1) = System.Math.Sin(v)
				kiq = kiq * riq Mod n
			Next
			ftcomplexfftplan(n - 1, 1, plan)
			ftapplysubplan(plan, 0, precr, offs, 0, plan.buffer, _
				1)
		End Sub


		'************************************************************************
'        This subroutine applies complex Rader's FFT to input/output array A.
'
'        INPUT PARAMETERS:
'            A           -   array, must be large enough for plan to work
'            ABase       -   base offset in array A, this value points to start of
'                            subarray whose length is equal to length of the plan
'            AOffset     -   offset with respect to ABase, 0<=AOffset<PlanLength.
'                            This is an offset within large PlanLength-subarray of
'                            the chunk to process.
'            OperandsCnt -   number of repeated operands (length N each)
'            N           -   original data length (measured in complex numbers)
'            SubPlan     -   position of the (N-1)-point FFT subplan which is used
'                            by transformation
'            RQ          -   primitive root modulo N
'            RIQ         -   inverse of primitive root modulo N
'            PrecOffs    -   offset of the precomputed data for the plan
'            Buf         -   temporary array
'            
'        OUTPUT PARAMETERS:
'            A           -   transformed array
'
'          -- ALGLIB --
'             Copyright 05.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftradersfft(plan As fasttransformplan, a As Double(), abase As Integer, aoffset As Integer, operandscnt As Integer, n As Integer, _
			subplan As Integer, rq As Integer, riq As Integer, precoffs As Integer, buf As Double())
			Dim opidx As Integer = 0
			Dim i As Integer = 0
			Dim q As Integer = 0
			Dim kq As Integer = 0
			Dim kiq As Integer = 0
			Dim x0 As Double = 0
			Dim y0 As Double = 0
			Dim p0 As Integer = 0
			Dim p1 As Integer = 0
			Dim ax As Double = 0
			Dim ay As Double = 0
			Dim bx As Double = 0
			Dim by As Double = 0
			Dim rx As Double = 0
			Dim ry As Double = 0

			alglib.ap.assert(operandscnt >= 1, "FTApplyComplexRefFFT: OperandsCnt<1")

			'
			' Process operands
			'
			For opidx = 0 To operandscnt - 1

				'
				' fill QA
				'
				kq = 1
				p0 = abase + aoffset + opidx * n * 2
				p1 = aoffset + opidx * n * 2
				rx = a(p0 + 0)
				ry = a(p0 + 1)
				x0 = rx
				y0 = ry
				For q = 0 To n - 2
					ax = a(p0 + 2 * kq + 0)
					ay = a(p0 + 2 * kq + 1)
					buf(p1 + 0) = ax
					buf(p1 + 1) = ay
					rx = rx + ax
					ry = ry + ay
					kq = kq * rq Mod n
					p1 = p1 + 2
				Next
				p0 = abase + aoffset + opidx * n * 2
				p1 = aoffset + opidx * n * 2
				For q = 0 To n - 2
					a(p0) = buf(p1)
					a(p0 + 1) = buf(p1 + 1)
					p0 = p0 + 2
					p1 = p1 + 2
				Next

				'
				' Convolution
				'
				ftapplysubplan(plan, subplan, a, abase, aoffset + opidx * n * 2, buf, _
					1)
				p0 = abase + aoffset + opidx * n * 2
				p1 = precoffs
				For i = 0 To n - 2
					ax = a(p0 + 0)
					ay = a(p0 + 1)
					bx = plan.precr(p1 + 0)
					by = plan.precr(p1 + 1)
					a(p0 + 0) = ax * bx - ay * by
					a(p0 + 1) = -(ax * by + ay * bx)
					p0 = p0 + 2
					p1 = p1 + 2
				Next
				ftapplysubplan(plan, subplan, a, abase, aoffset + opidx * n * 2, buf, _
					1)
				p0 = abase + aoffset + opidx * n * 2
				For i = 0 To n - 2
					a(p0 + 0) = a(p0 + 0) / (n - 1)
					a(p0 + 1) = -(a(p0 + 1) / (n - 1))
					p0 = p0 + 2
				Next

				'
				' Result
				'
				buf(aoffset + opidx * n * 2 + 0) = rx
				buf(aoffset + opidx * n * 2 + 1) = ry
				kiq = 1
				p0 = aoffset + opidx * n * 2
				p1 = abase + aoffset + opidx * n * 2
				For q = 0 To n - 2
					buf(p0 + 2 * kiq + 0) = x0 + a(p1 + 0)
					buf(p0 + 2 * kiq + 1) = y0 + a(p1 + 1)
					kiq = kiq * riq Mod n
					p1 = p1 + 2
				Next
				p0 = abase + aoffset + opidx * n * 2
				p1 = aoffset + opidx * n * 2
				For q = 0 To n - 1
					a(p0) = buf(p1)
					a(p0 + 1) = buf(p1 + 1)
					p0 = p0 + 2
					p1 = p1 + 2
				Next
			Next
		End Sub


		'************************************************************************
'        Factorizes task size N into product of two smaller sizes N1 and N2
'
'        INPUT PARAMETERS:
'            N       -   task size, N>0
'            IsRoot  -   whether taks is root task (first one in a sequence)
'            
'        OUTPUT PARAMETERS:
'            N1, N2  -   such numbers that:
'                        * for prime N:                  N1=N2=0
'                        * for composite N<=MaxRadix:    N1=N2=0
'                        * for composite N>MaxRadix:     1<=N1<=N2, N1*N2=N
'
'          -- ALGLIB --
'             Copyright 08.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftfactorize(n As Integer, isroot As Boolean, ByRef n1 As Integer, ByRef n2 As Integer)
			Dim j As Integer = 0
			Dim k As Integer = 0

			n1 = 0
			n2 = 0

			alglib.ap.assert(n > 0, "FTFactorize: N<=0")
			n1 = 0
			n2 = 0

			'
			' Small N
			'
			If n <= maxradix Then
				Return
			End If

			'
			' Large N, recursive split
			'
			If n > recursivethreshold Then
				k = CInt(System.Math.Truncate(System.Math.Ceiling(System.Math.sqrt(n)))) + 1
				alglib.ap.assert(k * k >= n, "FTFactorize: internal error during recursive factorization")
				For j = k To 2 Step -1
					If n Mod j = 0 Then
						n1 = System.Math.Min(n \ j, j)
						n2 = System.Math.Max(n \ j, j)
						Return
					End If
				Next
			End If

			'
			' N>MaxRadix, try to find good codelet
			'
			For j = maxradix To 2 Step -1
				If n Mod j = 0 Then
					n1 = j
					n2 = n \ j
					Exit For
				End If
			Next

			'
			' In case no good codelet was found,
			' try to factorize N into product of ANY primes.
			'
			If n1 * n2 <> n Then
				For j = 2 To n - 1
					If n Mod j = 0 Then
						n1 = j
						n2 = n \ j
						Exit For
					End If
					If j * j > n Then
						Exit For
					End If
				Next
			End If

			'
			' normalize
			'
			If n1 > n2 Then
				j = n1
				n1 = n2
				n2 = j
			End If
		End Sub


		'************************************************************************
'        Returns optimistic estimate of the FFT cost, in UNITs (1 UNIT = 100 KFLOPs)
'
'        INPUT PARAMETERS:
'            N       -   task size, N>0
'            
'        RESULU:
'            cost in UNITs, rounded down to nearest integer
'
'        NOTE: If FFT cost is less than 1 UNIT, it will return 0 as result.
'
'          -- ALGLIB --
'             Copyright 08.04.2013 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Function ftoptimisticestimate(n As Integer) As Integer
			Dim result As Integer = 0

			alglib.ap.assert(n > 0, "FTOptimisticEstimate: N<=0")
			result = CInt(System.Math.Truncate(System.Math.Floor(1E-05 * 5 * n * System.Math.Log(n) / System.Math.Log(2))))
			Return result
		End Function


		'************************************************************************
'        Twiddle factors calculation
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ffttwcalc(a As Double(), aoffset As Integer, n1 As Integer, n2 As Integer)
			Dim i As Integer = 0
			Dim j2 As Integer = 0
			Dim n As Integer = 0
			Dim halfn1 As Integer = 0
			Dim offs As Integer = 0
			Dim x As Double = 0
			Dim y As Double = 0
			Dim twxm1 As Double = 0
			Dim twy As Double = 0
			Dim twbasexm1 As Double = 0
			Dim twbasey As Double = 0
			Dim twrowxm1 As Double = 0
			Dim twrowy As Double = 0
			Dim tmpx As Double = 0
			Dim tmpy As Double = 0
			Dim v As Double = 0
			Dim updatetw2 As Integer = 0


			'
			' Multiplication by twiddle factors for complex Cooley-Tukey FFT
			' with N factorized as N1*N2.
			'
			' Naive solution to this problem is given below:
			'
			'     > for K:=1 to N2-1 do
			'     >     for J:=1 to N1-1 do
			'     >     begin
			'     >         Idx:=K*N1+J;
			'     >         X:=A[AOffset+2*Idx+0];
			'     >         Y:=A[AOffset+2*Idx+1];
			'     >         TwX:=Cos(-2*Pi()*K*J/(N1*N2));
			'     >         TwY:=Sin(-2*Pi()*K*J/(N1*N2));
			'     >         A[AOffset+2*Idx+0]:=X*TwX-Y*TwY;
			'     >         A[AOffset+2*Idx+1]:=X*TwY+Y*TwX;
			'     >     end;
			'
			' However, there are exist more efficient solutions.
			'
			' Each pass of the inner cycle corresponds to multiplication of one
			' entry of A by W[k,j]=exp(-I*2*pi*k*j/N). This factor can be rewritten
			' as exp(-I*2*pi*k/N)^j. So we can replace costly exponentiation by
			' repeated multiplication: W[k,j+1]=W[k,j]*exp(-I*2*pi*k/N), with
			' second factor being computed once in the beginning of the iteration.
			'
			' Also, exp(-I*2*pi*k/N) can be represented as exp(-I*2*pi/N)^k, i.e.
			' we have W[K+1,1]=W[K,1]*W[1,1].
			'
			' In our loop we use following variables:
			' * [TwBaseXM1,TwBaseY] =   [cos(2*pi/N)-1,     sin(2*pi/N)]
			' * [TwRowXM1, TwRowY]  =   [cos(2*pi*I/N)-1,   sin(2*pi*I/N)]
			' * [TwXM1,    TwY]     =   [cos(2*pi*I*J/N)-1, sin(2*pi*I*J/N)]
			'
			' Meaning of the variables:
			' * [TwXM1,TwY] is current twiddle factor W[I,J]
			' * [TwRowXM1, TwRowY] is W[I,1]
			' * [TwBaseXM1,TwBaseY] is W[1,1]
			'
			' During inner loop we multiply current twiddle factor by W[I,1],
			' during outer loop we update W[I,1].
			'
			'
			alglib.ap.assert(updatetw >= 2, "FFTTwCalc: internal error - UpdateTw<2")
			updatetw2 = updatetw \ 2
			halfn1 = n1 \ 2
			n = n1 * n2
			v = -(2 * System.Math.PI / n)
			twbasexm1 = -(2 * Math.sqr(System.Math.Sin(0.5 * v)))
			twbasey = System.Math.Sin(v)
			twrowxm1 = 0
			twrowy = 0
			offs = aoffset
			For i = 0 To n2 - 1

				'
				' Initialize twiddle factor for current row
				'
				twxm1 = 0
				twy = 0

				'
				' N1-point block is separated into 2-point chunks and residual 1-point chunk
				' (in case N1 is odd). Unrolled loop is several times faster.
				'
				For j2 = 0 To halfn1 - 1

					'
					' Processing:
					' * process first element in a chunk.
					' * update twiddle factor (unconditional update)
					' * process second element
					' * conditional update of the twiddle factor
					'
					x = a(offs + 0)
					y = a(offs + 1)
					tmpx = x * (1 + twxm1) - y * twy
					tmpy = x * twy + y * (1 + twxm1)
					a(offs + 0) = tmpx
					a(offs + 1) = tmpy
					tmpx = (1 + twxm1) * twrowxm1 - twy * twrowy
					twy = twy + (1 + twxm1) * twrowy + twy * twrowxm1
					twxm1 = twxm1 + tmpx
					x = a(offs + 2)
					y = a(offs + 3)
					tmpx = x * (1 + twxm1) - y * twy
					tmpy = x * twy + y * (1 + twxm1)
					a(offs + 2) = tmpx
					a(offs + 3) = tmpy
					offs = offs + 4
					If (j2 + 1) Mod updatetw2 = 0 AndAlso j2 < halfn1 - 1 Then

						'
						' Recalculate twiddle factor
						'
						v = -(2 * System.Math.PI * i * 2 * (j2 + 1) / n)
						twxm1 = System.Math.Sin(0.5 * v)
						twxm1 = -(2 * twxm1 * twxm1)
						twy = System.Math.Sin(v)
					Else

						'
						' Update twiddle factor
						'
						tmpx = (1 + twxm1) * twrowxm1 - twy * twrowy
						twy = twy + (1 + twxm1) * twrowy + twy * twrowxm1
						twxm1 = twxm1 + tmpx
					End If
				Next
				If n1 Mod 2 = 1 Then

					'
					' Handle residual chunk
					'
					x = a(offs + 0)
					y = a(offs + 1)
					tmpx = x * (1 + twxm1) - y * twy
					tmpy = x * twy + y * (1 + twxm1)
					a(offs + 0) = tmpx
					a(offs + 1) = tmpy
					offs = offs + 2
				End If

				'
				' update TwRow: TwRow(new) = TwRow(old)*TwBase
				'
				If i < n2 - 1 Then
					If (i + 1) Mod updatetw = 0 Then
						v = -(2 * System.Math.PI * (i + 1) / n)
						twrowxm1 = System.Math.Sin(0.5 * v)
						twrowxm1 = -(2 * twrowxm1 * twrowxm1)
						twrowy = System.Math.Sin(v)
					Else
						tmpx = twbasexm1 + twrowxm1 * twbasexm1 - twrowy * twbasey
						tmpy = twbasey + twrowxm1 * twbasey + twrowy * twbasexm1
						twrowxm1 = twrowxm1 + tmpx
						twrowy = twrowy + tmpy
					End If
				End If
			Next
		End Sub


		'************************************************************************
'        Linear transpose: transpose complex matrix stored in 1-dimensional array
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub internalcomplexlintranspose(a As Double(), m As Integer, n As Integer, astart As Integer, buf As Double())
			Dim i_ As Integer = 0
			Dim i1_ As Integer = 0

			ffticltrec(a, astart, n, buf, 0, m, _
				m, n)
			i1_ = (0) - (astart)
			For i_ = astart To astart + 2 * m * n - 1
				a(i_) = buf(i_ + i1_)
			Next
		End Sub


		'************************************************************************
'        Recurrent subroutine for a InternalComplexLinTranspose
'
'        Write A^T to B, where:
'        * A is m*n complex matrix stored in array A as pairs of real/image values,
'          beginning from AStart position, with AStride stride
'        * B is n*m complex matrix stored in array B as pairs of real/image values,
'          beginning from BStart position, with BStride stride
'        stride is measured in complex numbers, i.e. in real/image pairs.
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ffticltrec(a As Double(), astart As Integer, astride As Integer, b As Double(), bstart As Integer, bstride As Integer, _
			m As Integer, n As Integer)
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim idx1 As Integer = 0
			Dim idx2 As Integer = 0
			Dim m2 As Integer = 0
			Dim m1 As Integer = 0
			Dim n1 As Integer = 0

			If m = 0 OrElse n = 0 Then
				Return
			End If
			If System.Math.Max(m, n) <= 8 Then
				m2 = 2 * bstride
				For i = 0 To m - 1
					idx1 = bstart + 2 * i
					idx2 = astart + 2 * i * astride
					For j = 0 To n - 1
						b(idx1 + 0) = a(idx2 + 0)
						b(idx1 + 1) = a(idx2 + 1)
						idx1 = idx1 + m2
						idx2 = idx2 + 2
					Next
				Next
				Return
			End If
			If n > m Then

				'
				' New partition:
				'
				' "A^T -> B" becomes "(A1 A2)^T -> ( B1 )
				'                                  ( B2 )
				'
				n1 = n \ 2
				If n - n1 >= 8 AndAlso n1 Mod 8 <> 0 Then
					n1 = n1 + (8 - n1 Mod 8)
				End If
				alglib.ap.assert(n - n1 > 0)
				ffticltrec(a, astart, astride, b, bstart, bstride, _
					m, n1)
				ffticltrec(a, astart + 2 * n1, astride, b, bstart + 2 * n1 * bstride, bstride, _
					m, n - n1)
			Else

				'
				' New partition:
				'
				' "A^T -> B" becomes "( A1 )^T -> ( B1 B2 )
				'                     ( A2 )
				'
				m1 = m \ 2
				If m - m1 >= 8 AndAlso m1 Mod 8 <> 0 Then
					m1 = m1 + (8 - m1 Mod 8)
				End If
				alglib.ap.assert(m - m1 > 0)
				ffticltrec(a, astart, astride, b, bstart, bstride, _
					m1, n)
				ffticltrec(a, astart + 2 * m1 * astride, astride, b, bstart + 2 * m1, bstride, _
					m - m1, n)
			End If
		End Sub


		'************************************************************************
'        Recurrent subroutine for a InternalRealLinTranspose
'
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub fftirltrec(ByRef a As Double(), astart As Integer, astride As Integer, ByRef b As Double(), bstart As Integer, bstride As Integer, _
			m As Integer, n As Integer)
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim idx1 As Integer = 0
			Dim idx2 As Integer = 0
			Dim m1 As Integer = 0
			Dim n1 As Integer = 0

			If m = 0 OrElse n = 0 Then
				Return
			End If
			If System.Math.Max(m, n) <= 8 Then
				For i = 0 To m - 1
					idx1 = bstart + i
					idx2 = astart + i * astride
					For j = 0 To n - 1
						b(idx1) = a(idx2)
						idx1 = idx1 + bstride
						idx2 = idx2 + 1
					Next
				Next
				Return
			End If
			If n > m Then

				'
				' New partition:
				'
				' "A^T -> B" becomes "(A1 A2)^T -> ( B1 )
				'                                  ( B2 )
				'
				n1 = n \ 2
				If n - n1 >= 8 AndAlso n1 Mod 8 <> 0 Then
					n1 = n1 + (8 - n1 Mod 8)
				End If
				alglib.ap.assert(n - n1 > 0)
				fftirltrec(a, astart, astride, b, bstart, bstride, _
					m, n1)
				fftirltrec(a, astart + n1, astride, b, bstart + n1 * bstride, bstride, _
					m, n - n1)
			Else

				'
				' New partition:
				'
				' "A^T -> B" becomes "( A1 )^T -> ( B1 B2 )
				'                     ( A2 )
				'
				m1 = m \ 2
				If m - m1 >= 8 AndAlso m1 Mod 8 <> 0 Then
					m1 = m1 + (8 - m1 Mod 8)
				End If
				alglib.ap.assert(m - m1 > 0)
				fftirltrec(a, astart, astride, b, bstart, bstride, _
					m1, n)
				fftirltrec(a, astart + m1 * astride, astride, b, bstart + m1, bstride, _
					m - m1, n)
			End If
		End Sub


		'************************************************************************
'        recurrent subroutine for FFTFindSmoothRec
'
'          -- ALGLIB --
'             Copyright 01.05.2009 by Bochkanov Sergey
'        ************************************************************************

		Private Shared Sub ftbasefindsmoothrec(n As Integer, seed As Integer, leastfactor As Integer, ByRef best As Integer)
			alglib.ap.assert(ftbasemaxsmoothfactor <= 5, "FTBaseFindSmoothRec: internal error!")
			If seed >= n Then
				best = System.Math.Min(best, seed)
				Return
			End If
			If leastfactor <= 2 Then
				ftbasefindsmoothrec(n, seed * 2, 2, best)
			End If
			If leastfactor <= 3 Then
				ftbasefindsmoothrec(n, seed * 3, 3, best)
			End If
			If leastfactor <= 5 Then
				ftbasefindsmoothrec(n, seed * 5, 5, best)
			End If
		End Sub


	End Class
	Public Class nearunityunit
		Public Shared Function nulog1p(x As Double) As Double
			Dim result As Double = 0
			Dim z As Double = 0
			Dim lp As Double = 0
			Dim lq As Double = 0

			z = 1.0 + x
			If CDbl(z) < CDbl(0.707106781186548) OrElse CDbl(z) > CDbl(1.4142135623731) Then
				result = System.Math.Log(z)
				Return result
			End If
			z = x * x
			lp = 4.52700008624452E-05
			lp = lp * x + 0.498541028231934
			lp = lp * x + 6.5787325942061
			lp = lp * x + 29.9119193285531
			lp = lp * x + 60.9496679809878
			lp = lp * x + 57.1129635905855
			lp = lp * x + 20.0395534992013
			lq = 1.0
			lq = lq * x + 15.0629090834692
			lq = lq * x + 83.0475659679672
			lq = lq * x + 221.762398237329
			lq = lq * x + 309.098722253121
			lq = lq * x + 216.427886144959
			lq = lq * x + 60.1186604976038
			z = -(0.5 * z) + x * (z * lp / lq)
			result = x + z
			Return result
		End Function


		Public Shared Function nuexpm1(x As Double) As Double
			Dim result As Double = 0
			Dim r As Double = 0
			Dim xx As Double = 0
			Dim ep As Double = 0
			Dim eq As Double = 0

			If CDbl(x) < CDbl(-0.5) OrElse CDbl(x) > CDbl(0.5) Then
				result = System.Math.Exp(x) - 1.0
				Return result
			End If
			xx = x * x
			ep = 0.000126177193074811
			ep = ep * xx + 0.0302994407707442
			ep = ep * xx + 1.0
			eq = 3.00198505138664E-06
			eq = eq * xx + 0.00252448340349684
			eq = eq * xx + 0.227265548208155
			eq = eq * xx + 2.0
			r = x * ep
			r = r / (eq - r)
			result = r + r
			Return result
		End Function


		Public Shared Function nucosm1(x As Double) As Double
			Dim result As Double = 0
			Dim xx As Double = 0
			Dim c As Double = 0

			If CDbl(x) < CDbl(-(0.25 * System.Math.PI)) OrElse CDbl(x) > CDbl(0.25 * System.Math.PI) Then
				result = System.Math.Cos(x) - 1
				Return result
			End If
			xx = x * x
			c = 4.73775079642462E-14
			c = c * xx - 1.14702848434254E-11
			c = c * xx + 2.08767542870815E-09
			c = c * xx - 2.75573192149998E-07
			c = c * xx + 2.48015873015706E-05
			c = c * xx - 0.00138888888888889
			c = c * xx + 0.0416666666666667
			result = -(0.5 * xx) + xx * xx * c
			Return result
		End Function


	End Class
	Public Class alglibbasics


	End Class
End Class

