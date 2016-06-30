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

	Public Class densesolverreport
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property r1() As Double
			Get
				Return _innerobj.r1
			End Get
			Set
				_innerobj.r1 = value
			End Set
		End Property
		Public Property rinf() As Double
			Get
				Return _innerobj.rinf
			End Get
			Set
				_innerobj.rinf = value
			End Set
		End Property

		Public Sub New()
			_innerobj = New densesolver.densesolverreport()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New densesolverreport(DirectCast(_innerobj.make_copy(), densesolver.densesolverreport))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As densesolver.densesolverreport
		Public ReadOnly Property innerobj() As densesolver.densesolverreport
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As densesolver.densesolverreport)
			_innerobj = obj
		End Sub
	End Class


	'************************************************************************
'
'    ************************************************************************

	Public Class densesolverlsreport
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property r2() As Double
			Get
				Return _innerobj.r2
			End Get
			Set
				_innerobj.r2 = value
			End Set
		End Property
		Public Property cx() As Double(,)
			Get
				Return _innerobj.cx
			End Get
			Set
				_innerobj.cx = value
			End Set
		End Property
		Public Property n() As Integer
			Get
				Return _innerobj.n
			End Get
			Set
				_innerobj.n = value
			End Set
		End Property
		Public Property k() As Integer
			Get
				Return _innerobj.k
			End Get
			Set
				_innerobj.k = value
			End Set
		End Property

		Public Sub New()
			_innerobj = New densesolver.densesolverlsreport()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New densesolverlsreport(DirectCast(_innerobj.make_copy(), densesolver.densesolverlsreport))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As densesolver.densesolverlsreport
		Public ReadOnly Property innerobj() As densesolver.densesolverlsreport
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As densesolver.densesolverlsreport)
			_innerobj = obj
		End Sub
	End Class

	'************************************************************************
'    Dense solver.
'
'    This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
'    real matrix, x and b are vectors.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * iterative refinement
'    * O(N^3) complexity
'
'    COMMERCIAL EDITION OF ALGLIB:
'
'      ! Commercial version of ALGLIB includes two  important  improvements  of
'      ! this function, which can be used from C++ and C#:
'      ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'      ! * multicore support
'      !
'      ! Intel MKL gives approximately constant  (with  respect  to  number  of
'      ! worker threads) acceleration factor which depends on CPU  being  used,
'      ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'      ! comparison.
'      !
'      ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'      ! * about 2-3x faster than ALGLIB for C++ without MKL
'      ! * about 7-10x faster than "pure C#" edition of ALGLIB
'      ! Difference in performance will be more striking  on  newer  CPU's with
'      ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'      ! problem whose size is at least 128, with best  efficiency achieved for
'      ! N's larger than 512.
'      !
'      ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'      ! of this function. We should note that LU decomposition  is  harder  to
'      ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
'      ! many internal synchronization points which can not be avoided. However
'      ! parallelism starts to be profitable starting  from  N=1024,  achieving
'      ! near-linear speedup for N=4096 or higher.
'      !
'      ! In order to use multicore features you have to:
'      ! * use commercial version of ALGLIB
'      ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'      !   multicore code will be used (for multicore support)
'      !
'      ! We recommend you to read 'Working with commercial version' section  of
'      ! ALGLIB Reference Manual in order to find out how to  use  performance-
'      ! related features provided by commercial edition of ALGLIB.
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        N       -   size of A
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   return code:
'                    * -3    A is singular, or VERY close to singular.
'                            X is filled by zeros in such cases.
'                    * -1    N<=0 was passed
'                    *  1    task is solved (but matrix A may be ill-conditioned,
'                            check R1/RInf parameters for condition numbers).
'        Rep     -   solver report, see below for more info
'        X       -   array[0..N-1], it contains:
'                    * solution of A*x=b if A is non-singular (well-conditioned
'                      or ill-conditioned, but not very close to singular)
'                    * zeros,  if  A  is  singular  or  VERY  close to singular
'                      (in this case Info=-3).
'
'    SOLVER REPORT
'
'    Subroutine sets following fields of the Rep structure:
'    * R1        reciprocal of condition number: 1/cond(A), 1-norm.
'    * RInf      reciprocal of condition number: 1/cond(A), inf-norm.
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub rmatrixsolve(a As Double(,), n As Integer, b As Double(), ByRef info As Integer, ByRef rep As densesolverreport, ByRef x As Double())
		info = 0
		rep = New densesolverreport()
		x = New Double(-1) {}
		densesolver.rmatrixsolve(a, n, b, info, rep.innerobj, x)
		Return
	End Sub


	Public Shared Sub smp_rmatrixsolve(a As Double(,), n As Integer, b As Double(), ByRef info As Integer, ByRef rep As densesolverreport, ByRef x As Double())
		info = 0
		rep = New densesolverreport()
		x = New Double(-1) {}
		densesolver._pexec_rmatrixsolve(a, n, b, info, rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver.
'
'    Similar to RMatrixSolve() but solves task with multiple right parts (where
'    b and x are NxM matrices).
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * optional iterative refinement
'    * O(N^3+M*N^2) complexity
'
'    COMMERCIAL EDITION OF ALGLIB:
'
'      ! Commercial version of ALGLIB includes two  important  improvements  of
'      ! this function, which can be used from C++ and C#:
'      ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'      ! * multicore support
'      !
'      ! Intel MKL gives approximately constant  (with  respect  to  number  of
'      ! worker threads) acceleration factor which depends on CPU  being  used,
'      ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'      ! comparison.
'      !
'      ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'      ! * about 2-3x faster than ALGLIB for C++ without MKL
'      ! * about 7-10x faster than "pure C#" edition of ALGLIB
'      ! Difference in performance will be more striking  on  newer  CPU's with
'      ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'      ! problem whose size is at least 128, with best  efficiency achieved for
'      ! N's larger than 512.
'      !
'      ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'      ! of this function. We should note that LU decomposition  is  harder  to
'      ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
'      ! many internal synchronization points which can not be avoided. However
'      ! parallelism starts to be profitable starting  from  N=1024,  achieving
'      ! near-linear speedup for N=4096 or higher.
'      !
'      ! In order to use multicore features you have to:
'      ! * use commercial version of ALGLIB
'      ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'      !   multicore code will be used (for multicore support)
'      !
'      ! We recommend you to read 'Working with commercial version' section  of
'      ! ALGLIB Reference Manual in order to find out how to  use  performance-
'      ! related features provided by commercial edition of ALGLIB.
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        N       -   size of A
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'        RFS     -   iterative refinement switch:
'                    * True - refinement is used.
'                      Less performance, more precision.
'                    * False - refinement is not used.
'                      More performance, less precision.
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub rmatrixsolvem(a As Double(,), n As Integer, b As Double(,), m As Integer, rfs As Boolean, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As Double(,))
		info = 0
		rep = New densesolverreport()
		x = New Double(-1, -1) {}
		densesolver.rmatrixsolvem(a, n, b, m, rfs, info, _
			rep.innerobj, x)
		Return
	End Sub


	Public Shared Sub smp_rmatrixsolvem(a As Double(,), n As Integer, b As Double(,), m As Integer, rfs As Boolean, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As Double(,))
		info = 0
		rep = New densesolverreport()
		x = New Double(-1, -1) {}
		densesolver._pexec_rmatrixsolvem(a, n, b, m, rfs, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver.
'
'    This  subroutine  solves  a  system  A*X=B,  where A is NxN non-denegerate
'    real matrix given by its LU decomposition, X and B are NxM real matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * O(N^2) complexity
'    * condition number estimation
'
'    No iterative refinement  is provided because exact form of original matrix
'    is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    INPUT PARAMETERS
'        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
'        P       -   array[0..N-1], pivots array, RMatrixLU result
'        N       -   size of A
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub rmatrixlusolve(lua As Double(,), p As Integer(), n As Integer, b As Double(), ByRef info As Integer, ByRef rep As densesolverreport, _
		ByRef x As Double())
		info = 0
		rep = New densesolverreport()
		x = New Double(-1) {}
		densesolver.rmatrixlusolve(lua, p, n, b, info, rep.innerobj, _
			x)
		Return
	End Sub

	'************************************************************************
'    Dense solver.
'
'    Similar to RMatrixLUSolve() but solves task with multiple right parts
'    (where b and x are NxM matrices).
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * O(M*N^2) complexity
'    * condition number estimation
'
'    No iterative refinement  is provided because exact form of original matrix
'    is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    INPUT PARAMETERS
'        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
'        P       -   array[0..N-1], pivots array, RMatrixLU result
'        N       -   size of A
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub rmatrixlusolvem(lua As Double(,), p As Integer(), n As Integer, b As Double(,), m As Integer, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As Double(,))
		info = 0
		rep = New densesolverreport()
		x = New Double(-1, -1) {}
		densesolver.rmatrixlusolvem(lua, p, n, b, m, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver.
'
'    This  subroutine  solves  a  system  A*x=b,  where BOTH ORIGINAL A AND ITS
'    LU DECOMPOSITION ARE KNOWN. You can use it if for some  reasons  you  have
'    both A and its LU decomposition.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * iterative refinement
'    * O(N^2) complexity
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
'        P       -   array[0..N-1], pivots array, RMatrixLU result
'        N       -   size of A
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolveM
'        Rep     -   same as in RMatrixSolveM
'        X       -   same as in RMatrixSolveM
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub rmatrixmixedsolve(a As Double(,), lua As Double(,), p As Integer(), n As Integer, b As Double(), ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As Double())
		info = 0
		rep = New densesolverreport()
		x = New Double(-1) {}
		densesolver.rmatrixmixedsolve(a, lua, p, n, b, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver.
'
'    Similar to RMatrixMixedSolve() but  solves task with multiple right  parts
'    (where b and x are NxM matrices).
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * iterative refinement
'    * O(M*N^2) complexity
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
'        P       -   array[0..N-1], pivots array, RMatrixLU result
'        N       -   size of A
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolveM
'        Rep     -   same as in RMatrixSolveM
'        X       -   same as in RMatrixSolveM
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub rmatrixmixedsolvem(a As Double(,), lua As Double(,), p As Integer(), n As Integer, b As Double(,), m As Integer, _
		ByRef info As Integer, ByRef rep As densesolverreport, ByRef x As Double(,))
		info = 0
		rep = New densesolverreport()
		x = New Double(-1, -1) {}
		densesolver.rmatrixmixedsolvem(a, lua, p, n, b, m, _
			info, rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixSolveM(), but for complex matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * iterative refinement
'    * O(N^3+M*N^2) complexity
'
'    COMMERCIAL EDITION OF ALGLIB:
'
'      ! Commercial version of ALGLIB includes two  important  improvements  of
'      ! this function, which can be used from C++ and C#:
'      ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'      ! * multicore support
'      !
'      ! Intel MKL gives approximately constant  (with  respect  to  number  of
'      ! worker threads) acceleration factor which depends on CPU  being  used,
'      ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'      ! comparison.
'      !
'      ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'      ! * about 2-3x faster than ALGLIB for C++ without MKL
'      ! * about 7-10x faster than "pure C#" edition of ALGLIB
'      ! Difference in performance will be more striking  on  newer  CPU's with
'      ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'      ! problem whose size is at least 128, with best  efficiency achieved for
'      ! N's larger than 512.
'      !
'      ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'      ! of this function. We should note that LU decomposition  is  harder  to
'      ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
'      ! many internal synchronization points which can not be avoided. However
'      ! parallelism starts to be profitable starting  from  N=1024,  achieving
'      ! near-linear speedup for N=4096 or higher.
'      !
'      ! In order to use multicore features you have to:
'      ! * use commercial version of ALGLIB
'      ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'      !   multicore code will be used (for multicore support)
'      !
'      ! We recommend you to read 'Working with commercial version' section  of
'      ! ALGLIB Reference Manual in order to find out how to  use  performance-
'      ! related features provided by commercial edition of ALGLIB.
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        N       -   size of A
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'        RFS     -   iterative refinement switch:
'                    * True - refinement is used.
'                      Less performance, more precision.
'                    * False - refinement is not used.
'                      More performance, less precision.
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub cmatrixsolvem(a As complex(,), n As Integer, b As complex(,), m As Integer, rfs As Boolean, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As complex(,))
		info = 0
		rep = New densesolverreport()
		x = New complex(-1, -1) {}
		densesolver.cmatrixsolvem(a, n, b, m, rfs, info, _
			rep.innerobj, x)
		Return
	End Sub


	Public Shared Sub smp_cmatrixsolvem(a As complex(,), n As Integer, b As complex(,), m As Integer, rfs As Boolean, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As complex(,))
		info = 0
		rep = New densesolverreport()
		x = New complex(-1, -1) {}
		densesolver._pexec_cmatrixsolvem(a, n, b, m, rfs, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixSolve(), but for complex matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * iterative refinement
'    * O(N^3) complexity
'
'    COMMERCIAL EDITION OF ALGLIB:
'
'      ! Commercial version of ALGLIB includes two  important  improvements  of
'      ! this function, which can be used from C++ and C#:
'      ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'      ! * multicore support
'      !
'      ! Intel MKL gives approximately constant  (with  respect  to  number  of
'      ! worker threads) acceleration factor which depends on CPU  being  used,
'      ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'      ! comparison.
'      !
'      ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'      ! * about 2-3x faster than ALGLIB for C++ without MKL
'      ! * about 7-10x faster than "pure C#" edition of ALGLIB
'      ! Difference in performance will be more striking  on  newer  CPU's with
'      ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'      ! problem whose size is at least 128, with best  efficiency achieved for
'      ! N's larger than 512.
'      !
'      ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'      ! of this function. We should note that LU decomposition  is  harder  to
'      ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
'      ! many internal synchronization points which can not be avoided. However
'      ! parallelism starts to be profitable starting  from  N=1024,  achieving
'      ! near-linear speedup for N=4096 or higher.
'      !
'      ! In order to use multicore features you have to:
'      ! * use commercial version of ALGLIB
'      ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'      !   multicore code will be used (for multicore support)
'      !
'      ! We recommend you to read 'Working with commercial version' section  of
'      ! ALGLIB Reference Manual in order to find out how to  use  performance-
'      ! related features provided by commercial edition of ALGLIB.
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        N       -   size of A
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub cmatrixsolve(a As complex(,), n As Integer, b As complex(), ByRef info As Integer, ByRef rep As densesolverreport, ByRef x As complex())
		info = 0
		rep = New densesolverreport()
		x = New complex(-1) {}
		densesolver.cmatrixsolve(a, n, b, info, rep.innerobj, x)
		Return
	End Sub


	Public Shared Sub smp_cmatrixsolve(a As complex(,), n As Integer, b As complex(), ByRef info As Integer, ByRef rep As densesolverreport, ByRef x As complex())
		info = 0
		rep = New densesolverreport()
		x = New complex(-1) {}
		densesolver._pexec_cmatrixsolve(a, n, b, info, rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixLUSolveM(), but for complex matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * O(M*N^2) complexity
'    * condition number estimation
'
'    No iterative refinement  is provided because exact form of original matrix
'    is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    INPUT PARAMETERS
'        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
'        P       -   array[0..N-1], pivots array, RMatrixLU result
'        N       -   size of A
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub cmatrixlusolvem(lua As complex(,), p As Integer(), n As Integer, b As complex(,), m As Integer, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As complex(,))
		info = 0
		rep = New densesolverreport()
		x = New complex(-1, -1) {}
		densesolver.cmatrixlusolvem(lua, p, n, b, m, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixLUSolve(), but for complex matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * O(N^2) complexity
'    * condition number estimation
'
'    No iterative refinement is provided because exact form of original matrix
'    is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    INPUT PARAMETERS
'        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
'        P       -   array[0..N-1], pivots array, CMatrixLU result
'        N       -   size of A
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub cmatrixlusolve(lua As complex(,), p As Integer(), n As Integer, b As complex(), ByRef info As Integer, ByRef rep As densesolverreport, _
		ByRef x As complex())
		info = 0
		rep = New densesolverreport()
		x = New complex(-1) {}
		densesolver.cmatrixlusolve(lua, p, n, b, info, rep.innerobj, _
			x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixMixedSolveM(), but for complex matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * iterative refinement
'    * O(M*N^2) complexity
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
'        P       -   array[0..N-1], pivots array, CMatrixLU result
'        N       -   size of A
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolveM
'        Rep     -   same as in RMatrixSolveM
'        X       -   same as in RMatrixSolveM
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub cmatrixmixedsolvem(a As complex(,), lua As complex(,), p As Integer(), n As Integer, b As complex(,), m As Integer, _
		ByRef info As Integer, ByRef rep As densesolverreport, ByRef x As complex(,))
		info = 0
		rep = New densesolverreport()
		x = New complex(-1, -1) {}
		densesolver.cmatrixmixedsolvem(a, lua, p, n, b, m, _
			info, rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixMixedSolve(), but for complex matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * iterative refinement
'    * O(N^2) complexity
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
'        P       -   array[0..N-1], pivots array, CMatrixLU result
'        N       -   size of A
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolveM
'        Rep     -   same as in RMatrixSolveM
'        X       -   same as in RMatrixSolveM
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub cmatrixmixedsolve(a As complex(,), lua As complex(,), p As Integer(), n As Integer, b As complex(), ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As complex())
		info = 0
		rep = New densesolverreport()
		x = New complex(-1) {}
		densesolver.cmatrixmixedsolve(a, lua, p, n, b, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixSolveM(), but for symmetric positive definite
'    matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * O(N^3+M*N^2) complexity
'    * matrix is represented by its upper or lower triangle
'
'    No iterative refinement is provided because such partial representation of
'    matrix does not allow efficient calculation of extra-precise  matrix-vector
'    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    COMMERCIAL EDITION OF ALGLIB:
'
'      ! Commercial version of ALGLIB includes two  important  improvements  of
'      ! this function, which can be used from C++ and C#:
'      ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'      ! * multicore support
'      !
'      ! Intel MKL gives approximately constant  (with  respect  to  number  of
'      ! worker threads) acceleration factor which depends on CPU  being  used,
'      ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'      ! comparison.
'      !
'      ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'      ! * about 2-3x faster than ALGLIB for C++ without MKL
'      ! * about 7-10x faster than "pure C#" edition of ALGLIB
'      ! Difference in performance will be more striking  on  newer  CPU's with
'      ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'      ! problem whose size is at least 128, with best  efficiency achieved for
'      ! N's larger than 512.
'      !
'      ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'      ! of this function. We should note that Cholesky decomposition is harder
'      ! to parallelize than, say, matrix-matrix product - this  algorithm  has
'      ! several synchronization points which  can  not  be  avoided.  However,
'      ! parallelism starts to be profitable starting from N=500.
'      !
'      ! In order to use multicore features you have to:
'      ! * use commercial version of ALGLIB
'      ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'      !   multicore code will be used (for multicore support)
'      !
'      ! We recommend you to read 'Working with commercial version' section  of
'      ! ALGLIB Reference Manual in order to find out how to  use  performance-
'      ! related features provided by commercial edition of ALGLIB.
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        N       -   size of A
'        IsUpper -   what half of A is provided
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve.
'                    Returns -3 for non-SPD matrices.
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub spdmatrixsolvem(a As Double(,), n As Integer, isupper As Boolean, b As Double(,), m As Integer, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As Double(,))
		info = 0
		rep = New densesolverreport()
		x = New Double(-1, -1) {}
		densesolver.spdmatrixsolvem(a, n, isupper, b, m, info, _
			rep.innerobj, x)
		Return
	End Sub


	Public Shared Sub smp_spdmatrixsolvem(a As Double(,), n As Integer, isupper As Boolean, b As Double(,), m As Integer, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As Double(,))
		info = 0
		rep = New densesolverreport()
		x = New Double(-1, -1) {}
		densesolver._pexec_spdmatrixsolvem(a, n, isupper, b, m, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixSolve(), but for SPD matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * O(N^3) complexity
'    * matrix is represented by its upper or lower triangle
'
'    No iterative refinement is provided because such partial representation of
'    matrix does not allow efficient calculation of extra-precise  matrix-vector
'    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    COMMERCIAL EDITION OF ALGLIB:
'
'      ! Commercial version of ALGLIB includes two  important  improvements  of
'      ! this function, which can be used from C++ and C#:
'      ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'      ! * multicore support
'      !
'      ! Intel MKL gives approximately constant  (with  respect  to  number  of
'      ! worker threads) acceleration factor which depends on CPU  being  used,
'      ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'      ! comparison.
'      !
'      ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'      ! * about 2-3x faster than ALGLIB for C++ without MKL
'      ! * about 7-10x faster than "pure C#" edition of ALGLIB
'      ! Difference in performance will be more striking  on  newer  CPU's with
'      ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'      ! problem whose size is at least 128, with best  efficiency achieved for
'      ! N's larger than 512.
'      !
'      ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'      ! of this function. We should note that Cholesky decomposition is harder
'      ! to parallelize than, say, matrix-matrix product - this  algorithm  has
'      ! several synchronization points which  can  not  be  avoided.  However,
'      ! parallelism starts to be profitable starting from N=500.
'      !
'      ! In order to use multicore features you have to:
'      ! * use commercial version of ALGLIB
'      ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'      !   multicore code will be used (for multicore support)
'      !
'      ! We recommend you to read 'Working with commercial version' section  of
'      ! ALGLIB Reference Manual in order to find out how to  use  performance-
'      ! related features provided by commercial edition of ALGLIB.
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        N       -   size of A
'        IsUpper -   what half of A is provided
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'                    Returns -3 for non-SPD matrices.
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub spdmatrixsolve(a As Double(,), n As Integer, isupper As Boolean, b As Double(), ByRef info As Integer, ByRef rep As densesolverreport, _
		ByRef x As Double())
		info = 0
		rep = New densesolverreport()
		x = New Double(-1) {}
		densesolver.spdmatrixsolve(a, n, isupper, b, info, rep.innerobj, _
			x)
		Return
	End Sub


	Public Shared Sub smp_spdmatrixsolve(a As Double(,), n As Integer, isupper As Boolean, b As Double(), ByRef info As Integer, ByRef rep As densesolverreport, _
		ByRef x As Double())
		info = 0
		rep = New densesolverreport()
		x = New Double(-1) {}
		densesolver._pexec_spdmatrixsolve(a, n, isupper, b, info, rep.innerobj, _
			x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixLUSolveM(), but for SPD matrices  represented
'    by their Cholesky decomposition.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * O(M*N^2) complexity
'    * condition number estimation
'    * matrix is represented by its upper or lower triangle
'
'    No iterative refinement is provided because such partial representation of
'    matrix does not allow efficient calculation of extra-precise  matrix-vector
'    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    INPUT PARAMETERS
'        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
'                    SPDMatrixCholesky result
'        N       -   size of CHA
'        IsUpper -   what half of CHA is provided
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub spdmatrixcholeskysolvem(cha As Double(,), n As Integer, isupper As Boolean, b As Double(,), m As Integer, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As Double(,))
		info = 0
		rep = New densesolverreport()
		x = New Double(-1, -1) {}
		densesolver.spdmatrixcholeskysolvem(cha, n, isupper, b, m, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixLUSolve(), but for  SPD matrices  represented
'    by their Cholesky decomposition.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * O(N^2) complexity
'    * condition number estimation
'    * matrix is represented by its upper or lower triangle
'
'    No iterative refinement is provided because such partial representation of
'    matrix does not allow efficient calculation of extra-precise  matrix-vector
'    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    INPUT PARAMETERS
'        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
'                    SPDMatrixCholesky result
'        N       -   size of A
'        IsUpper -   what half of CHA is provided
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub spdmatrixcholeskysolve(cha As Double(,), n As Integer, isupper As Boolean, b As Double(), ByRef info As Integer, ByRef rep As densesolverreport, _
		ByRef x As Double())
		info = 0
		rep = New densesolverreport()
		x = New Double(-1) {}
		densesolver.spdmatrixcholeskysolve(cha, n, isupper, b, info, rep.innerobj, _
			x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixSolveM(), but for Hermitian positive definite
'    matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * O(N^3+M*N^2) complexity
'    * matrix is represented by its upper or lower triangle
'
'    No iterative refinement is provided because such partial representation of
'    matrix does not allow efficient calculation of extra-precise  matrix-vector
'    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    COMMERCIAL EDITION OF ALGLIB:
'
'      ! Commercial version of ALGLIB includes two  important  improvements  of
'      ! this function, which can be used from C++ and C#:
'      ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'      ! * multicore support
'      !
'      ! Intel MKL gives approximately constant  (with  respect  to  number  of
'      ! worker threads) acceleration factor which depends on CPU  being  used,
'      ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'      ! comparison.
'      !
'      ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'      ! * about 2-3x faster than ALGLIB for C++ without MKL
'      ! * about 7-10x faster than "pure C#" edition of ALGLIB
'      ! Difference in performance will be more striking  on  newer  CPU's with
'      ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'      ! problem whose size is at least 128, with best  efficiency achieved for
'      ! N's larger than 512.
'      !
'      ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'      ! of this function. We should note that Cholesky decomposition is harder
'      ! to parallelize than, say, matrix-matrix product - this  algorithm  has
'      ! several synchronization points which  can  not  be  avoided.  However,
'      ! parallelism starts to be profitable starting from N=500.
'      !
'      ! In order to use multicore features you have to:
'      ! * use commercial version of ALGLIB
'      ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'      !   multicore code will be used (for multicore support)
'      !
'      ! We recommend you to read 'Working with commercial version' section  of
'      ! ALGLIB Reference Manual in order to find out how to  use  performance-
'      ! related features provided by commercial edition of ALGLIB.
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        N       -   size of A
'        IsUpper -   what half of A is provided
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve.
'                    Returns -3 for non-HPD matrices.
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub hpdmatrixsolvem(a As complex(,), n As Integer, isupper As Boolean, b As complex(,), m As Integer, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As complex(,))
		info = 0
		rep = New densesolverreport()
		x = New complex(-1, -1) {}
		densesolver.hpdmatrixsolvem(a, n, isupper, b, m, info, _
			rep.innerobj, x)
		Return
	End Sub


	Public Shared Sub smp_hpdmatrixsolvem(a As complex(,), n As Integer, isupper As Boolean, b As complex(,), m As Integer, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As complex(,))
		info = 0
		rep = New densesolverreport()
		x = New complex(-1, -1) {}
		densesolver._pexec_hpdmatrixsolvem(a, n, isupper, b, m, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixSolve(),  but for Hermitian positive definite
'    matrices.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * condition number estimation
'    * O(N^3) complexity
'    * matrix is represented by its upper or lower triangle
'
'    No iterative refinement is provided because such partial representation of
'    matrix does not allow efficient calculation of extra-precise  matrix-vector
'    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    COMMERCIAL EDITION OF ALGLIB:
'
'      ! Commercial version of ALGLIB includes two  important  improvements  of
'      ! this function, which can be used from C++ and C#:
'      ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'      ! * multicore support
'      !
'      ! Intel MKL gives approximately constant  (with  respect  to  number  of
'      ! worker threads) acceleration factor which depends on CPU  being  used,
'      ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'      ! comparison.
'      !
'      ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'      ! * about 2-3x faster than ALGLIB for C++ without MKL
'      ! * about 7-10x faster than "pure C#" edition of ALGLIB
'      ! Difference in performance will be more striking  on  newer  CPU's with
'      ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'      ! problem whose size is at least 128, with best  efficiency achieved for
'      ! N's larger than 512.
'      !
'      ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'      ! of this function. We should note that Cholesky decomposition is harder
'      ! to parallelize than, say, matrix-matrix product - this  algorithm  has
'      ! several synchronization points which  can  not  be  avoided.  However,
'      ! parallelism starts to be profitable starting from N=500.
'      !
'      ! In order to use multicore features you have to:
'      ! * use commercial version of ALGLIB
'      ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'      !   multicore code will be used (for multicore support)
'      !
'      ! We recommend you to read 'Working with commercial version' section  of
'      ! ALGLIB Reference Manual in order to find out how to  use  performance-
'      ! related features provided by commercial edition of ALGLIB.
'
'    INPUT PARAMETERS
'        A       -   array[0..N-1,0..N-1], system matrix
'        N       -   size of A
'        IsUpper -   what half of A is provided
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'                    Returns -3 for non-HPD matrices.
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub hpdmatrixsolve(a As complex(,), n As Integer, isupper As Boolean, b As complex(), ByRef info As Integer, ByRef rep As densesolverreport, _
		ByRef x As complex())
		info = 0
		rep = New densesolverreport()
		x = New complex(-1) {}
		densesolver.hpdmatrixsolve(a, n, isupper, b, info, rep.innerobj, _
			x)
		Return
	End Sub


	Public Shared Sub smp_hpdmatrixsolve(a As complex(,), n As Integer, isupper As Boolean, b As complex(), ByRef info As Integer, ByRef rep As densesolverreport, _
		ByRef x As complex())
		info = 0
		rep = New densesolverreport()
		x = New complex(-1) {}
		densesolver._pexec_hpdmatrixsolve(a, n, isupper, b, info, rep.innerobj, _
			x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixLUSolveM(), but for HPD matrices  represented
'    by their Cholesky decomposition.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * O(M*N^2) complexity
'    * condition number estimation
'    * matrix is represented by its upper or lower triangle
'
'    No iterative refinement is provided because such partial representation of
'    matrix does not allow efficient calculation of extra-precise  matrix-vector
'    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    INPUT PARAMETERS
'        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
'                    HPDMatrixCholesky result
'        N       -   size of CHA
'        IsUpper -   what half of CHA is provided
'        B       -   array[0..N-1,0..M-1], right part
'        M       -   right part size
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub hpdmatrixcholeskysolvem(cha As complex(,), n As Integer, isupper As Boolean, b As complex(,), m As Integer, ByRef info As Integer, _
		ByRef rep As densesolverreport, ByRef x As complex(,))
		info = 0
		rep = New densesolverreport()
		x = New complex(-1, -1) {}
		densesolver.hpdmatrixcholeskysolvem(cha, n, isupper, b, m, info, _
			rep.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    Dense solver. Same as RMatrixLUSolve(), but for  HPD matrices  represented
'    by their Cholesky decomposition.
'
'    Algorithm features:
'    * automatic detection of degenerate cases
'    * O(N^2) complexity
'    * condition number estimation
'    * matrix is represented by its upper or lower triangle
'
'    No iterative refinement is provided because such partial representation of
'    matrix does not allow efficient calculation of extra-precise  matrix-vector
'    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
'    need iterative refinement.
'
'    INPUT PARAMETERS
'        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
'                    SPDMatrixCholesky result
'        N       -   size of A
'        IsUpper -   what half of CHA is provided
'        B       -   array[0..N-1], right part
'
'    OUTPUT PARAMETERS
'        Info    -   same as in RMatrixSolve
'        Rep     -   same as in RMatrixSolve
'        X       -   same as in RMatrixSolve
'
'      -- ALGLIB --
'         Copyright 27.01.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub hpdmatrixcholeskysolve(cha As complex(,), n As Integer, isupper As Boolean, b As complex(), ByRef info As Integer, ByRef rep As densesolverreport, _
		ByRef x As complex())
		info = 0
		rep = New densesolverreport()
		x = New complex(-1) {}
		densesolver.hpdmatrixcholeskysolve(cha, n, isupper, b, info, rep.innerobj, _
			x)
		Return
	End Sub

	'************************************************************************
'    Dense solver.
'
'    This subroutine finds solution of the linear system A*X=B with non-square,
'    possibly degenerate A.  System  is  solved in the least squares sense, and
'    general least squares solution  X = X0 + CX*y  which  minimizes |A*X-B| is
'    returned. If A is non-degenerate, solution in the usual sense is returned.
'
'    Algorithm features:
'    * automatic detection (and correct handling!) of degenerate cases
'    * iterative refinement
'    * O(N^3) complexity
'
'    COMMERCIAL EDITION OF ALGLIB:
'
'      ! Commercial version of ALGLIB includes one  important  improvement   of
'      ! this function, which can be used from C++ and C#:
'      ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'      !
'      ! Intel MKL gives approximately constant  (with  respect  to  number  of
'      ! worker threads) acceleration factor which depends on CPU  being  used,
'      ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'      ! comparison.
'      !
'      ! Generally, commercial ALGLIB is several times faster than  open-source
'      ! generic C edition, and many times faster than open-source C# edition.
'      !
'      ! Multithreaded acceleration is only partially supported (some parts are
'      ! optimized, but most - are not).
'      !
'      ! We recommend you to read 'Working with commercial version' section  of
'      ! ALGLIB Reference Manual in order to find out how to  use  performance-
'      ! related features provided by commercial edition of ALGLIB.
'
'    INPUT PARAMETERS
'        A       -   array[0..NRows-1,0..NCols-1], system matrix
'        NRows   -   vertical size of A
'        NCols   -   horizontal size of A
'        B       -   array[0..NCols-1], right part
'        Threshold-  a number in [0,1]. Singular values  beyond  Threshold  are
'                    considered  zero.  Set  it to 0.0, if you don't understand
'                    what it means, so the solver will choose good value on its
'                    own.
'
'    OUTPUT PARAMETERS
'        Info    -   return code:
'                    * -4    SVD subroutine failed
'                    * -1    if NRows<=0 or NCols<=0 or Threshold<0 was passed
'                    *  1    if task is solved
'        Rep     -   solver report, see below for more info
'        X       -   array[0..N-1,0..M-1], it contains:
'                    * solution of A*X=B (even for singular A)
'                    * zeros, if SVD subroutine failed
'
'    SOLVER REPORT
'
'    Subroutine sets following fields of the Rep structure:
'    * R2        reciprocal of condition number: 1/cond(A), 2-norm.
'    * N         = NCols
'    * K         dim(Null(A))
'    * CX        array[0..N-1,0..K-1], kernel of A.
'                Columns of CX store such vectors that A*CX[i]=0.
'
'      -- ALGLIB --
'         Copyright 24.08.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub rmatrixsolvels(a As Double(,), nrows As Integer, ncols As Integer, b As Double(), threshold As Double, ByRef info As Integer, _
		ByRef rep As densesolverlsreport, ByRef x As Double())
		info = 0
		rep = New densesolverlsreport()
		x = New Double(-1) {}
		densesolver.rmatrixsolvels(a, nrows, ncols, b, threshold, info, _
			rep.innerobj, x)
		Return
	End Sub


	Public Shared Sub smp_rmatrixsolvels(a As Double(,), nrows As Integer, ncols As Integer, b As Double(), threshold As Double, ByRef info As Integer, _
		ByRef rep As densesolverlsreport, ByRef x As Double())
		info = 0
		rep = New densesolverlsreport()
		x = New Double(-1) {}
		densesolver._pexec_rmatrixsolvels(a, nrows, ncols, b, threshold, info, _
			rep.innerobj, x)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    This object stores state of the LinLSQR method.
'
'    You should use ALGLIB functions to work with this object.
'    ************************************************************************

	Public Class linlsqrstate
		Inherits alglibobject
		'
		' Public declarations
		'

		Public Sub New()
			_innerobj = New linlsqr.linlsqrstate()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New linlsqrstate(DirectCast(_innerobj.make_copy(), linlsqr.linlsqrstate))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As linlsqr.linlsqrstate
		Public ReadOnly Property innerobj() As linlsqr.linlsqrstate
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As linlsqr.linlsqrstate)
			_innerobj = obj
		End Sub
	End Class


	'************************************************************************
'
'    ************************************************************************

	Public Class linlsqrreport
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property iterationscount() As Integer
			Get
				Return _innerobj.iterationscount
			End Get
			Set
				_innerobj.iterationscount = value
			End Set
		End Property
		Public Property nmv() As Integer
			Get
				Return _innerobj.nmv
			End Get
			Set
				_innerobj.nmv = value
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
			_innerobj = New linlsqr.linlsqrreport()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New linlsqrreport(DirectCast(_innerobj.make_copy(), linlsqr.linlsqrreport))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As linlsqr.linlsqrreport
		Public ReadOnly Property innerobj() As linlsqr.linlsqrreport
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As linlsqr.linlsqrreport)
			_innerobj = obj
		End Sub
	End Class

	'************************************************************************
'    This function initializes linear LSQR Solver. This solver is used to solve
'    non-symmetric (and, possibly, non-square) problems. Least squares solution
'    is returned for non-compatible systems.
'
'    USAGE:
'    1. User initializes algorithm state with LinLSQRCreate() call
'    2. User tunes solver parameters with  LinLSQRSetCond() and other functions
'    3. User  calls  LinLSQRSolveSparse()  function which takes algorithm state
'       and SparseMatrix object.
'    4. User calls LinLSQRResults() to get solution
'    5. Optionally, user may call LinLSQRSolveSparse() again to  solve  another
'       problem  with different matrix and/or right part without reinitializing
'       LinLSQRState structure.
'
'    INPUT PARAMETERS:
'        M       -   number of rows in A
'        N       -   number of variables, N>0
'
'    OUTPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'      -- ALGLIB --
'         Copyright 30.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub linlsqrcreate(m As Integer, n As Integer, ByRef state As linlsqrstate)
		state = New linlsqrstate()
		linlsqr.linlsqrcreate(m, n, state.innerobj)
		Return
	End Sub

	'************************************************************************
'    This  function  changes  preconditioning  settings of LinLSQQSolveSparse()
'    function. By default, SolveSparse() uses diagonal preconditioner,  but  if
'    you want to use solver without preconditioning, you can call this function
'    which forces solver to use unit matrix for preconditioning.
'
'    INPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'      -- ALGLIB --
'         Copyright 19.11.2012 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub linlsqrsetprecunit(state As linlsqrstate)

		linlsqr.linlsqrsetprecunit(state.innerobj)
		Return
	End Sub

	'************************************************************************
'    This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
'    function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
'    preconditioner. This preconditioning mode is active by default.
'
'    INPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'      -- ALGLIB --
'         Copyright 19.11.2012 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub linlsqrsetprecdiag(state As linlsqrstate)

		linlsqr.linlsqrsetprecdiag(state.innerobj)
		Return
	End Sub

	'************************************************************************
'    This function sets optional Tikhonov regularization coefficient.
'    It is zero by default.
'
'    INPUT PARAMETERS:
'        LambdaI -   regularization factor, LambdaI>=0
'
'    OUTPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'      -- ALGLIB --
'         Copyright 30.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub linlsqrsetlambdai(state As linlsqrstate, lambdai As Double)

		linlsqr.linlsqrsetlambdai(state.innerobj, lambdai)
		Return
	End Sub

	'************************************************************************
'    Procedure for solution of A*x=b with sparse A.
'
'    INPUT PARAMETERS:
'        State   -   algorithm state
'        A       -   sparse M*N matrix in the CRS format (you MUST contvert  it
'                    to CRS format  by  calling  SparseConvertToCRS()  function
'                    BEFORE you pass it to this function).
'        B       -   right part, array[M]
'
'    RESULT:
'        This function returns no result.
'        You can get solution by calling LinCGResults()
'
'    NOTE: this function uses lightweight preconditioning -  multiplication  by
'          inverse of diag(A). If you want, you can turn preconditioning off by
'          calling LinLSQRSetPrecUnit(). However, preconditioning cost is   low
'          and preconditioner is very important for solution  of  badly  scaled
'          problems.
'
'      -- ALGLIB --
'         Copyright 30.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub linlsqrsolvesparse(state As linlsqrstate, a As sparsematrix, b As Double())

		linlsqr.linlsqrsolvesparse(state.innerobj, a.innerobj, b)
		Return
	End Sub

	'************************************************************************
'    This function sets stopping criteria.
'
'    INPUT PARAMETERS:
'        EpsA    -   algorithm will be stopped if ||A^T*Rk||/(||A||*||Rk||)<=EpsA.
'        EpsB    -   algorithm will be stopped if ||Rk||<=EpsB*||B||
'        MaxIts  -   algorithm will be stopped if number of iterations
'                    more than MaxIts.
'
'    OUTPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'    NOTE: if EpsA,EpsB,EpsC and MaxIts are zero then these variables will
'    be setted as default values.
'
'      -- ALGLIB --
'         Copyright 30.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub linlsqrsetcond(state As linlsqrstate, epsa As Double, epsb As Double, maxits As Integer)

		linlsqr.linlsqrsetcond(state.innerobj, epsa, epsb, maxits)
		Return
	End Sub

	'************************************************************************
'    LSQR solver: results.
'
'    This function must be called after LinLSQRSolve
'
'    INPUT PARAMETERS:
'        State   -   algorithm state
'
'    OUTPUT PARAMETERS:
'        X       -   array[N], solution
'        Rep     -   optimization report:
'                    * Rep.TerminationType completetion code:
'                        *  1    ||Rk||<=EpsB*||B||
'                        *  4    ||A^T*Rk||/(||A||*||Rk||)<=EpsA
'                        *  5    MaxIts steps was taken
'                        *  7    rounding errors prevent further progress,
'                                X contains best point found so far.
'                                (sometimes returned on singular systems)
'                    * Rep.IterationsCount contains iterations count
'                    * NMV countains number of matrix-vector calculations
'
'      -- ALGLIB --
'         Copyright 30.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub linlsqrresults(state As linlsqrstate, ByRef x As Double(), ByRef rep As linlsqrreport)
		x = New Double(-1) {}
		rep = New linlsqrreport()
		linlsqr.linlsqrresults(state.innerobj, x, rep.innerobj)
		Return
	End Sub

	'************************************************************************
'    This function turns on/off reporting.
'
'    INPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'        NeedXRep-   whether iteration reports are needed or not
'
'    If NeedXRep is True, algorithm will call rep() callback function if  it is
'    provided to MinCGOptimize().
'
'      -- ALGLIB --
'         Copyright 30.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub linlsqrsetxrep(state As linlsqrstate, needxrep As Boolean)

		linlsqr.linlsqrsetxrep(state.innerobj, needxrep)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'    This object stores state of the linear CG method.
'
'    You should use ALGLIB functions to work with this object.
'    Never try to access its fields directly!
'    ************************************************************************

	Public Class lincgstate
		Inherits alglibobject
		'
		' Public declarations
		'

		Public Sub New()
			_innerobj = New lincg.lincgstate()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New lincgstate(DirectCast(_innerobj.make_copy(), lincg.lincgstate))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As lincg.lincgstate
		Public ReadOnly Property innerobj() As lincg.lincgstate
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As lincg.lincgstate)
			_innerobj = obj
		End Sub
	End Class


	'************************************************************************
'
'    ************************************************************************

	Public Class lincgreport
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property iterationscount() As Integer
			Get
				Return _innerobj.iterationscount
			End Get
			Set
				_innerobj.iterationscount = value
			End Set
		End Property
		Public Property nmv() As Integer
			Get
				Return _innerobj.nmv
			End Get
			Set
				_innerobj.nmv = value
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
		Public Property r2() As Double
			Get
				Return _innerobj.r2
			End Get
			Set
				_innerobj.r2 = value
			End Set
		End Property

		Public Sub New()
			_innerobj = New lincg.lincgreport()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New lincgreport(DirectCast(_innerobj.make_copy(), lincg.lincgreport))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As lincg.lincgreport
		Public ReadOnly Property innerobj() As lincg.lincgreport
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As lincg.lincgreport)
			_innerobj = obj
		End Sub
	End Class

	'************************************************************************
'    This function initializes linear CG Solver. This solver is used  to  solve
'    symmetric positive definite problems. If you want  to  solve  nonsymmetric
'    (or non-positive definite) problem you may use LinLSQR solver provided  by
'    ALGLIB.
'
'    USAGE:
'    1. User initializes algorithm state with LinCGCreate() call
'    2. User tunes solver parameters with  LinCGSetCond() and other functions
'    3. Optionally, user sets starting point with LinCGSetStartingPoint()
'    4. User  calls LinCGSolveSparse() function which takes algorithm state and
'       SparseMatrix object.
'    5. User calls LinCGResults() to get solution
'    6. Optionally, user may call LinCGSolveSparse()  again  to  solve  another
'       problem  with different matrix and/or right part without reinitializing
'       LinCGState structure.
'
'    INPUT PARAMETERS:
'        N       -   problem dimension, N>0
'
'    OUTPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'      -- ALGLIB --
'         Copyright 14.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgcreate(n As Integer, ByRef state As lincgstate)
		state = New lincgstate()
		lincg.lincgcreate(n, state.innerobj)
		Return
	End Sub

	'************************************************************************
'    This function sets starting point.
'    By default, zero starting point is used.
'
'    INPUT PARAMETERS:
'        X       -   starting point, array[N]
'
'    OUTPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'      -- ALGLIB --
'         Copyright 14.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgsetstartingpoint(state As lincgstate, x As Double())

		lincg.lincgsetstartingpoint(state.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
'    function. By default, SolveSparse() uses diagonal preconditioner,  but  if
'    you want to use solver without preconditioning, you can call this function
'    which forces solver to use unit matrix for preconditioning.
'
'    INPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'      -- ALGLIB --
'         Copyright 19.11.2012 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgsetprecunit(state As lincgstate)

		lincg.lincgsetprecunit(state.innerobj)
		Return
	End Sub

	'************************************************************************
'    This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
'    function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
'    preconditioner. This preconditioning mode is active by default.
'
'    INPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'      -- ALGLIB --
'         Copyright 19.11.2012 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgsetprecdiag(state As lincgstate)

		lincg.lincgsetprecdiag(state.innerobj)
		Return
	End Sub

	'************************************************************************
'    This function sets stopping criteria.
'
'    INPUT PARAMETERS:
'        EpsF    -   algorithm will be stopped if norm of residual is less than
'                    EpsF*||b||.
'        MaxIts  -   algorithm will be stopped if number of iterations is  more
'                    than MaxIts.
'
'    OUTPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'    NOTES:
'    If  both  EpsF  and  MaxIts  are  zero then small EpsF will be set to small
'    value.
'
'      -- ALGLIB --
'         Copyright 14.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgsetcond(state As lincgstate, epsf As Double, maxits As Integer)

		lincg.lincgsetcond(state.innerobj, epsf, maxits)
		Return
	End Sub

	'************************************************************************
'    Procedure for solution of A*x=b with sparse A.
'
'    INPUT PARAMETERS:
'        State   -   algorithm state
'        A       -   sparse matrix in the CRS format (you MUST contvert  it  to
'                    CRS format by calling SparseConvertToCRS() function).
'        IsUpper -   whether upper or lower triangle of A is used:
'                    * IsUpper=True  => only upper triangle is used and lower
'                                       triangle is not referenced at all
'                    * IsUpper=False => only lower triangle is used and upper
'                                       triangle is not referenced at all
'        B       -   right part, array[N]
'
'    RESULT:
'        This function returns no result.
'        You can get solution by calling LinCGResults()
'
'    NOTE: this function uses lightweight preconditioning -  multiplication  by
'          inverse of diag(A). If you want, you can turn preconditioning off by
'          calling LinCGSetPrecUnit(). However, preconditioning cost is low and
'          preconditioner  is  very  important  for  solution  of  badly scaled
'          problems.
'
'      -- ALGLIB --
'         Copyright 14.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgsolvesparse(state As lincgstate, a As sparsematrix, isupper As Boolean, b As Double())

		lincg.lincgsolvesparse(state.innerobj, a.innerobj, isupper, b)
		Return
	End Sub

	'************************************************************************
'    CG-solver: results.
'
'    This function must be called after LinCGSolve
'
'    INPUT PARAMETERS:
'        State   -   algorithm state
'
'    OUTPUT PARAMETERS:
'        X       -   array[N], solution
'        Rep     -   optimization report:
'                    * Rep.TerminationType completetion code:
'                        * -5    input matrix is either not positive definite,
'                                too large or too small
'                        * -4    overflow/underflow during solution
'                                (ill conditioned problem)
'                        *  1    ||residual||<=EpsF*||b||
'                        *  5    MaxIts steps was taken
'                        *  7    rounding errors prevent further progress,
'                                best point found is returned
'                    * Rep.IterationsCount contains iterations count
'                    * NMV countains number of matrix-vector calculations
'
'      -- ALGLIB --
'         Copyright 14.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgresults(state As lincgstate, ByRef x As Double(), ByRef rep As lincgreport)
		x = New Double(-1) {}
		rep = New lincgreport()
		lincg.lincgresults(state.innerobj, x, rep.innerobj)
		Return
	End Sub

	'************************************************************************
'    This function sets restart frequency. By default, algorithm  is  restarted
'    after N subsequent iterations.
'
'      -- ALGLIB --
'         Copyright 14.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgsetrestartfreq(state As lincgstate, srf As Integer)

		lincg.lincgsetrestartfreq(state.innerobj, srf)
		Return
	End Sub

	'************************************************************************
'    This function sets frequency of residual recalculations.
'
'    Algorithm updates residual r_k using iterative formula,  but  recalculates
'    it from scratch after each 10 iterations. It is done to avoid accumulation
'    of numerical errors and to stop algorithm when r_k starts to grow.
'
'    Such low update frequence (1/10) gives very  little  overhead,  but  makes
'    algorithm a bit more robust against numerical errors. However, you may
'    change it
'
'    INPUT PARAMETERS:
'        Freq    -   desired update frequency, Freq>=0.
'                    Zero value means that no updates will be done.
'
'      -- ALGLIB --
'         Copyright 14.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgsetrupdatefreq(state As lincgstate, freq As Integer)

		lincg.lincgsetrupdatefreq(state.innerobj, freq)
		Return
	End Sub

	'************************************************************************
'    This function turns on/off reporting.
'
'    INPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'        NeedXRep-   whether iteration reports are needed or not
'
'    If NeedXRep is True, algorithm will call rep() callback function if  it is
'    provided to MinCGOptimize().
'
'      -- ALGLIB --
'         Copyright 14.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub lincgsetxrep(state As lincgstate, needxrep As Boolean)

		lincg.lincgsetxrep(state.innerobj, needxrep)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'
'    ************************************************************************

	Public Class nleqstate
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
		Public Property needfij() As Boolean
			Get
				Return _innerobj.needfij
			End Get
			Set
				_innerobj.needfij = value
			End Set
		End Property
		Public Property xupdated() As Boolean
			Get
				Return _innerobj.xupdated
			End Get
			Set
				_innerobj.xupdated = value
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
		Public ReadOnly Property fi() As Double()
			Get
				Return _innerobj.fi
			End Get
		End Property
		Public ReadOnly Property j() As Double(,)
			Get
				Return _innerobj.j
			End Get
		End Property
		Public ReadOnly Property x() As Double()
			Get
				Return _innerobj.x
			End Get
		End Property

		Public Sub New()
			_innerobj = New nleq.nleqstate()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New nleqstate(DirectCast(_innerobj.make_copy(), nleq.nleqstate))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As nleq.nleqstate
		Public ReadOnly Property innerobj() As nleq.nleqstate
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As nleq.nleqstate)
			_innerobj = obj
		End Sub
	End Class


	'************************************************************************
'
'    ************************************************************************

	Public Class nleqreport
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property iterationscount() As Integer
			Get
				Return _innerobj.iterationscount
			End Get
			Set
				_innerobj.iterationscount = value
			End Set
		End Property
		Public Property nfunc() As Integer
			Get
				Return _innerobj.nfunc
			End Get
			Set
				_innerobj.nfunc = value
			End Set
		End Property
		Public Property njac() As Integer
			Get
				Return _innerobj.njac
			End Get
			Set
				_innerobj.njac = value
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
			_innerobj = New nleq.nleqreport()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New nleqreport(DirectCast(_innerobj.make_copy(), nleq.nleqreport))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As nleq.nleqreport
		Public ReadOnly Property innerobj() As nleq.nleqreport
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As nleq.nleqreport)
			_innerobj = obj
		End Sub
	End Class

	'************************************************************************
'                    LEVENBERG-MARQUARDT-LIKE NONLINEAR SOLVER
'
'    DESCRIPTION:
'    This algorithm solves system of nonlinear equations
'        F[0](x[0], ..., x[n-1])   = 0
'        F[1](x[0], ..., x[n-1])   = 0
'        ...
'        F[M-1](x[0], ..., x[n-1]) = 0
'    with M/N do not necessarily coincide.  Algorithm  converges  quadratically
'    under following conditions:
'        * the solution set XS is nonempty
'        * for some xs in XS there exist such neighbourhood N(xs) that:
'          * vector function F(x) and its Jacobian J(x) are continuously
'            differentiable on N
'          * ||F(x)|| provides local error bound on N, i.e. there  exists  such
'            c1, that ||F(x)||>c1*distance(x,XS)
'    Note that these conditions are much more weaker than usual non-singularity
'    conditions. For example, algorithm will converge for any  affine  function
'    F (whether its Jacobian singular or not).
'
'
'    REQUIREMENTS:
'    Algorithm will request following information during its operation:
'    * function vector F[] and Jacobian matrix at given point X
'    * value of merit function f(x)=F[0]^2(x)+...+F[M-1]^2(x) at given point X
'
'
'    USAGE:
'    1. User initializes algorithm state with NLEQCreateLM() call
'    2. User tunes solver parameters with  NLEQSetCond(),  NLEQSetStpMax()  and
'       other functions
'    3. User  calls  NLEQSolve()  function  which  takes  algorithm  state  and
'       pointers (delegates, etc.) to callback functions which calculate  merit
'       function value and Jacobian.
'    4. User calls NLEQResults() to get solution
'    5. Optionally, user may call NLEQRestartFrom() to  solve  another  problem
'       with same parameters (N/M) but another starting  point  and/or  another
'       function vector. NLEQRestartFrom() allows to reuse already  initialized
'       structure.
'
'
'    INPUT PARAMETERS:
'        N       -   space dimension, N>1:
'                    * if provided, only leading N elements of X are used
'                    * if not provided, determined automatically from size of X
'        M       -   system size
'        X       -   starting point
'
'
'    OUTPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'
'
'    NOTES:
'    1. you may tune stopping conditions with NLEQSetCond() function
'    2. if target function contains exp() or other fast growing functions,  and
'       optimization algorithm makes too large steps which leads  to  overflow,
'       use NLEQSetStpMax() function to bound algorithm's steps.
'    3. this  algorithm  is  a  slightly  modified implementation of the method
'       described  in  'Levenberg-Marquardt  method  for constrained  nonlinear
'       equations with strong local convergence properties' by Christian Kanzow
'       Nobuo Yamashita and Masao Fukushima and further  developed  in  'On the
'       convergence of a New Levenberg-Marquardt Method'  by  Jin-yan  Fan  and
'       Ya-Xiang Yuan.
'
'
'      -- ALGLIB --
'         Copyright 20.08.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub nleqcreatelm(n As Integer, m As Integer, x As Double(), ByRef state As nleqstate)
		state = New nleqstate()
		nleq.nleqcreatelm(n, m, x, state.innerobj)
		Return
	End Sub
	Public Shared Sub nleqcreatelm(m As Integer, x As Double(), ByRef state As nleqstate)
		Dim n As Integer

		state = New nleqstate()
		n = ap.len(x)
		nleq.nleqcreatelm(n, m, x, state.innerobj)

		Return
	End Sub

	'************************************************************************
'    This function sets stopping conditions for the nonlinear solver
'
'    INPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'        EpsF    -   >=0
'                    The subroutine finishes  its work if on k+1-th iteration
'                    the condition ||F||<=EpsF is satisfied
'        MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
'                    iterations is unlimited.
'
'    Passing EpsF=0 and MaxIts=0 simultaneously will lead to  automatic
'    stopping criterion selection (small EpsF).
'
'    NOTES:
'
'      -- ALGLIB --
'         Copyright 20.08.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub nleqsetcond(state As nleqstate, epsf As Double, maxits As Integer)

		nleq.nleqsetcond(state.innerobj, epsf, maxits)
		Return
	End Sub

	'************************************************************************
'    This function turns on/off reporting.
'
'    INPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'        NeedXRep-   whether iteration reports are needed or not
'
'    If NeedXRep is True, algorithm will call rep() callback function if  it is
'    provided to NLEQSolve().
'
'      -- ALGLIB --
'         Copyright 20.08.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub nleqsetxrep(state As nleqstate, needxrep As Boolean)

		nleq.nleqsetxrep(state.innerobj, needxrep)
		Return
	End Sub

	'************************************************************************
'    This function sets maximum step length
'
'    INPUT PARAMETERS:
'        State   -   structure which stores algorithm state
'        StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
'                    want to limit step length.
'
'    Use this subroutine when target function  contains  exp()  or  other  fast
'    growing functions, and algorithm makes  too  large  steps  which  lead  to
'    overflow. This function allows us to reject steps that are too large  (and
'    therefore expose us to the possible overflow) without actually calculating
'    function value at the x+stp*d.
'
'      -- ALGLIB --
'         Copyright 20.08.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub nleqsetstpmax(state As nleqstate, stpmax As Double)

		nleq.nleqsetstpmax(state.innerobj, stpmax)
		Return
	End Sub

	'************************************************************************
'    This function provides reverse communication interface
'    Reverse communication interface is not documented or recommended to use.
'    See below for functions which provide better documented API
'    ************************************************************************

	Public Shared Function nleqiteration(state As nleqstate) As Boolean

		Dim result As Boolean = nleq.nleqiteration(state.innerobj)
		Return result
	End Function
	'************************************************************************
'    This family of functions is used to launcn iterations of nonlinear solver
'
'    These functions accept following parameters:
'        func    -   callback which calculates function (or merit function)
'                    value func at given point x
'        jac     -   callback which calculates function vector fi[]
'                    and Jacobian jac at given point x
'        rep     -   optional callback which is called after each iteration
'                    can be null
'        obj     -   optional object which is passed to func/grad/hess/jac/rep
'                    can be null
'
'
'      -- ALGLIB --
'         Copyright 20.03.2009 by Bochkanov Sergey
'
'    ************************************************************************

	Public Shared Sub nleqsolve(state As nleqstate, func As ndimensional_func, jac As ndimensional_jac, rep As ndimensional_rep, obj As Object)
		If func Is Nothing Then
			Throw New alglibexception("ALGLIB: error in 'nleqsolve()' (func is null)")
		End If
		If jac Is Nothing Then
			Throw New alglibexception("ALGLIB: error in 'nleqsolve()' (jac is null)")
		End If
		While alglib.nleqiteration(state)
			If state.needf Then
				func(state.x, state.innerobj.f, obj)
				Continue While
			End If
			If state.needfij Then
				jac(state.x, state.innerobj.fi, state.innerobj.j, obj)
				Continue While
			End If
			If state.innerobj.xupdated Then
                Call rep(state.innerobj.x, state.innerobj.f, obj)
				Continue While
			End If
			Throw New alglibexception("ALGLIB: error in 'nleqsolve' (some derivatives were not provided?)")
		End While
	End Sub



	'************************************************************************
'    NLEQ solver results
'
'    INPUT PARAMETERS:
'        State   -   algorithm state.
'
'    OUTPUT PARAMETERS:
'        X       -   array[0..N-1], solution
'        Rep     -   optimization report:
'                    * Rep.TerminationType completetion code:
'                        * -4    ERROR:  algorithm   has   converged   to   the
'                                stationary point Xf which is local minimum  of
'                                f=F[0]^2+...+F[m-1]^2, but is not solution  of
'                                nonlinear system.
'                        *  1    sqrt(f)<=EpsF.
'                        *  5    MaxIts steps was taken
'                        *  7    stopping conditions are too stringent,
'                                further improvement is impossible
'                    * Rep.IterationsCount contains iterations count
'                    * NFEV countains number of function calculations
'                    * ActiveConstraints contains number of active constraints
'
'      -- ALGLIB --
'         Copyright 20.08.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub nleqresults(state As nleqstate, ByRef x As Double(), ByRef rep As nleqreport)
		x = New Double(-1) {}
		rep = New nleqreport()
		nleq.nleqresults(state.innerobj, x, rep.innerobj)
		Return
	End Sub

	'************************************************************************
'    NLEQ solver results
'
'    Buffered implementation of NLEQResults(), which uses pre-allocated  buffer
'    to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
'    intended to be used in the inner cycles of performance critical algorithms
'    where array reallocation penalty is too large to be ignored.
'
'      -- ALGLIB --
'         Copyright 20.08.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub nleqresultsbuf(state As nleqstate, ByRef x As Double(), rep As nleqreport)

		nleq.nleqresultsbuf(state.innerobj, x, rep.innerobj)
		Return
	End Sub

	'************************************************************************
'    This  subroutine  restarts  CG  algorithm from new point. All optimization
'    parameters are left unchanged.
'
'    This  function  allows  to  solve multiple  optimization  problems  (which
'    must have same number of dimensions) without object reallocation penalty.
'
'    INPUT PARAMETERS:
'        State   -   structure used for reverse communication previously
'                    allocated with MinCGCreate call.
'        X       -   new starting point.
'        BndL    -   new lower bounds
'        BndU    -   new upper bounds
'
'      -- ALGLIB --
'         Copyright 30.07.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub nleqrestartfrom(state As nleqstate, x As Double())

		nleq.nleqrestartfrom(state.innerobj, x)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'
'    ************************************************************************

	Public Class polynomialsolverreport
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property maxerr() As Double
			Get
				Return _innerobj.maxerr
			End Get
			Set
				_innerobj.maxerr = value
			End Set
		End Property

		Public Sub New()
			_innerobj = New polynomialsolver.polynomialsolverreport()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New polynomialsolverreport(DirectCast(_innerobj.make_copy(), polynomialsolver.polynomialsolverreport))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As polynomialsolver.polynomialsolverreport
		Public ReadOnly Property innerobj() As polynomialsolver.polynomialsolverreport
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As polynomialsolver.polynomialsolverreport)
			_innerobj = obj
		End Sub
	End Class

	'************************************************************************
'    Polynomial root finding.
'
'    This function returns all roots of the polynomial
'        P(x) = a0 + a1*x + a2*x^2 + ... + an*x^n
'    Both real and complex roots are returned (see below).
'
'    INPUT PARAMETERS:
'        A       -   array[N+1], polynomial coefficients:
'                    * A[0] is constant term
'                    * A[N] is a coefficient of X^N
'        N       -   polynomial degree
'
'    OUTPUT PARAMETERS:
'        X       -   array of complex roots:
'                    * for isolated real root, X[I] is strictly real: IMAGE(X[I])=0
'                    * complex roots are always returned in pairs - roots occupy
'                      positions I and I+1, with:
'                      * X[I+1]=Conj(X[I])
'                      * IMAGE(X[I]) > 0
'                      * IMAGE(X[I+1]) = -IMAGE(X[I]) < 0
'                    * multiple real roots may have non-zero imaginary part due
'                      to roundoff errors. There is no reliable way to distinguish
'                      real root of multiplicity 2 from two  complex  roots  in
'                      the presence of roundoff errors.
'        Rep     -   report, additional information, following fields are set:
'                    * Rep.MaxErr - max( |P(xi)| )  for  i=0..N-1.  This  field
'                      allows to quickly estimate "quality" of the roots  being
'                      returned.
'
'    NOTE:   this function uses companion matrix method to find roots. In  case
'            internal EVD  solver  fails  do  find  eigenvalues,  exception  is
'            generated.
'
'    NOTE:   roots are not "polished" and  no  matrix  balancing  is  performed
'            for them.
'
'      -- ALGLIB --
'         Copyright 24.02.2014 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub polynomialsolve(a As Double(), n As Integer, ByRef x As complex(), ByRef rep As polynomialsolverreport)
		x = New complex(-1) {}
		rep = New polynomialsolverreport()
		polynomialsolver.polynomialsolve(a, n, x, rep.innerobj)
		Return
	End Sub

End Class
Public Partial Class alglib
	Public Class densesolver
		Public Class densesolverreport
			Inherits apobject
			Public r1 As Double
			Public rinf As Double
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New densesolverreport()
				_result.r1 = r1
				_result.rinf = rinf
				Return _result
			End Function
		End Class


		Public Class densesolverlsreport
			Inherits apobject
			Public r2 As Double
			Public cx As Double(,)
			Public n As Integer
			Public k As Integer
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
				cx = New Double(-1, -1) {}
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New densesolverlsreport()
				_result.r2 = r2
				_result.cx = DirectCast(cx.Clone(), Double(,))
				_result.n = n
				_result.k = k
				Return _result
			End Function
		End Class




		'************************************************************************
'        Dense solver.
'
'        This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
'        real matrix, x and b are vectors.
'
'        Algorithm features:
'        * automatic detection of degenerate cases
'        * condition number estimation
'        * iterative refinement
'        * O(N^3) complexity
'
'        COMMERCIAL EDITION OF ALGLIB:
'
'          ! Commercial version of ALGLIB includes two  important  improvements  of
'          ! this function, which can be used from C++ and C#:
'          ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'          ! * multicore support
'          !
'          ! Intel MKL gives approximately constant  (with  respect  to  number  of
'          ! worker threads) acceleration factor which depends on CPU  being  used,
'          ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'          ! comparison.
'          !
'          ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'          ! * about 2-3x faster than ALGLIB for C++ without MKL
'          ! * about 7-10x faster than "pure C#" edition of ALGLIB
'          ! Difference in performance will be more striking  on  newer  CPU's with
'          ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'          ! problem whose size is at least 128, with best  efficiency achieved for
'          ! N's larger than 512.
'          !
'          ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'          ! of this function. We should note that LU decomposition  is  harder  to
'          ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
'          ! many internal synchronization points which can not be avoided. However
'          ! parallelism starts to be profitable starting  from  N=1024,  achieving
'          ! near-linear speedup for N=4096 or higher.
'          !
'          ! In order to use multicore features you have to:
'          ! * use commercial version of ALGLIB
'          ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'          !   multicore code will be used (for multicore support)
'          !
'          ! We recommend you to read 'Working with commercial version' section  of
'          ! ALGLIB Reference Manual in order to find out how to  use  performance-
'          ! related features provided by commercial edition of ALGLIB.
'
'        INPUT PARAMETERS
'            A       -   array[0..N-1,0..N-1], system matrix
'            N       -   size of A
'            B       -   array[0..N-1], right part
'
'        OUTPUT PARAMETERS
'            Info    -   return code:
'                        * -3    A is singular, or VERY close to singular.
'                                X is filled by zeros in such cases.
'                        * -1    N<=0 was passed
'                        *  1    task is solved (but matrix A may be ill-conditioned,
'                                check R1/RInf parameters for condition numbers).
'            Rep     -   solver report, see below for more info
'            X       -   array[0..N-1], it contains:
'                        * solution of A*x=b if A is non-singular (well-conditioned
'                          or ill-conditioned, but not very close to singular)
'                        * zeros,  if  A  is  singular  or  VERY  close to singular
'                          (in this case Info=-3).
'
'        SOLVER REPORT
'
'        Subroutine sets following fields of the Rep structure:
'        * R1        reciprocal of condition number: 1/cond(A), 1-norm.
'        * RInf      reciprocal of condition number: 1/cond(A), inf-norm.
'
'          -- ALGLIB --
'             Copyright 27.01.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub rmatrixsolve(a As Double(,), n As Integer, b As Double(), ByRef info As Integer, rep As densesolverreport, ByRef x As Double())
			Dim bm As Double(,) = New Double(-1, -1) {}
			Dim xm As Double(,) = New Double(-1, -1) {}
			Dim i_ As Integer = 0

			info = 0
			x = New Double(-1) {}

			If n <= 0 Then
				info = -1
				Return
			End If
			bm = New Double(n - 1, 0) {}
			For i_ = 0 To n - 1
				bm(i_, 0) = b(i_)
			Next
			rmatrixsolvem(a, n, bm, 1, True, info, _
				rep, xm)
			x = New Double(n - 1) {}
			For i_ = 0 To n - 1
				x(i_) = xm(i_, 0)
			Next
		End Sub


		'************************************************************************
'        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
'        ************************************************************************

		Public Shared Sub _pexec_rmatrixsolve(a As Double(,), n As Integer, b As Double(), ByRef info As Integer, rep As densesolverreport, ByRef x As Double())
			rmatrixsolve(a, n, b, info, rep, x)
		End Sub


		'************************************************************************
'        Dense solver.
'
'        Similar to RMatrixSolve() but solves task with multiple right parts (where
'        b and x are NxM matrices).
'
'        Algorithm features:
'        * automatic detection of degenerate cases
'        * condition number estimation
'        * optional iterative refinement
'        * O(N^3+M*N^2) complexity
'
'        COMMERCIAL EDITION OF ALGLIB:
'
'          ! Commercial version of ALGLIB includes two  important  improvements  of
'          ! this function, which can be used from C++ and C#:
'          ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
'          ! * multicore support
'          !
'          ! Intel MKL gives approximately constant  (with  respect  to  number  of
'          ! worker threads) acceleration factor which depends on CPU  being  used,
'          ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
'          ! comparison.
'          !
'          ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
'          ! * about 2-3x faster than ALGLIB for C++ without MKL
'          ! * about 7-10x faster than "pure C#" edition of ALGLIB
'          ! Difference in performance will be more striking  on  newer  CPU's with
'          ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
'          ! problem whose size is at least 128, with best  efficiency achieved for
'          ! N's larger than 512.
'          !
'          ! Commercial edition of ALGLIB also supports multithreaded  acceleration
'          ! of this function. We should note that LU decomposition  is  harder  to
'          ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
'          ! many internal synchronization points which can not be avoided. However
'          ! parallelism starts to be profitable starting  from  N=1024,  achieving
'          ! near-linear speedup for N=4096 or higher.
'          !
'          ! In order to use multicore features you have to:
'          ! * use commercial version of ALGLIB
'          ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
'          !   multicore code will be used (for multicore support)
'          !
'          ! We recommend you to read 'Working with commercial version' section  of
'          ! ALGLIB Reference Manual in order to find out how to  use  performance-
'          ! related features provided by commercial edition of ALGLIB.
'
'        INPUT PARAMETERS
'            A       -   array[0..N-1,0..N-1], system matrix
'            N       -   size of A
'            B       -   array[0..N-1,0..M-1], right part
'            M       -   right part size
'            RFS     -   iterative refinement switch:
'                        * True - refinement is used.
'                          Less performance, more precision.
'                        * False - refinement is not used.
'                          More performance, less precision.
'
'        OUTPUT PARAMETERS
'            Info    -   same as in RMatrixSolve
'            Rep     -   same as in RMatrixSolve
'            X       -   same as in RMatrixSolve
'
'          -- ALGLIB --
'             Copyright 27.01.2010 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub rmatrixsolvem(a As Double(,), n As Integer, b As Double(,), m As Integer, rfs As Boolean, ByRef info As Integer, _
			rep As densesolverreport, ByRef x As Double(,))
			Dim da As Double(,) = New Double(-1, -1) {}
			Dim emptya As Double(,) = New Double(-1, -1) {}
			Dim p As Integer() = New Integer(-1) {}
			Dim scalea As Double = 0
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim i_ As Integer = 0

			info = 0
			x = New Double(-1, -1) {}


			'
			' prepare: check inputs, allocate space...
			'
			If n <= 0 OrElse m <= 0 Then
				info = -1
				Return
			End If
			da = New Double(n - 1, n - 1) {}

			'
			' 1. scale matrix, max(|A[i,j]|)
			' 2. factorize scaled matrix
			' 3. solve
			'
			scalea = 0
			For i = 0 To n - 1
				For j = 0 To n - 1
                    scalea = System.Math.Max(scalea, System.Math.Abs(a(i, j)))
                Next
            Next
            If CDbl(scalea) = CDbl(0) Then
                scalea = 1
            End If
            scalea = 1 / scalea
            For i = 0 To n - 1
                For i_ = 0 To n - 1
                    da(i, i_) = a(i, i_)
                Next
            Next
            trfac.rmatrixlu(da, n, n, p)
            If rfs Then
                rmatrixlusolveinternal(da, p, scalea, n, a, True, _
                    b, m, info, rep, x)
            Else
                rmatrixlusolveinternal(da, p, scalea, n, emptya, False, _
                    b, m, info, rep, x)
            End If
        End Sub


        '************************************************************************
        '        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
        '        ************************************************************************

        Public Shared Sub _pexec_rmatrixsolvem(a As Double(,), n As Integer, b As Double(,), m As Integer, rfs As Boolean, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As Double(,))
            rmatrixsolvem(a, n, b, m, rfs, info, _
                rep, x)
        End Sub


        '************************************************************************
        '        Dense solver.
        '
        '        This  subroutine  solves  a  system  A*X=B,  where A is NxN non-denegerate
        '        real matrix given by its LU decomposition, X and B are NxM real matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * O(N^2) complexity
        '        * condition number estimation
        '
        '        No iterative refinement  is provided because exact form of original matrix
        '        is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        INPUT PARAMETERS
        '            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        '            P       -   array[0..N-1], pivots array, RMatrixLU result
        '            N       -   size of A
        '            B       -   array[0..N-1], right part
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '            
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixlusolve(lua As Double(,), p As Integer(), n As Integer, b As Double(), ByRef info As Integer, rep As densesolverreport, _
            ByRef x As Double())
            Dim bm As Double(,) = New Double(-1, -1) {}
            Dim xm As Double(,) = New Double(-1, -1) {}
            Dim i_ As Integer = 0

            info = 0
            x = New Double(-1) {}

            If n <= 0 Then
                info = -1
                Return
            End If
            bm = New Double(n - 1, 0) {}
            For i_ = 0 To n - 1
                bm(i_, 0) = b(i_)
            Next
            rmatrixlusolvem(lua, p, n, bm, 1, info, _
                rep, xm)
            x = New Double(n - 1) {}
            For i_ = 0 To n - 1
                x(i_) = xm(i_, 0)
            Next
        End Sub


        '************************************************************************
        '        Dense solver.
        '
        '        Similar to RMatrixLUSolve() but solves task with multiple right parts
        '        (where b and x are NxM matrices).
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * O(M*N^2) complexity
        '        * condition number estimation
        '
        '        No iterative refinement  is provided because exact form of original matrix
        '        is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        INPUT PARAMETERS
        '            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        '            P       -   array[0..N-1], pivots array, RMatrixLU result
        '            N       -   size of A
        '            B       -   array[0..N-1,0..M-1], right part
        '            M       -   right part size
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixlusolvem(lua As Double(,), p As Integer(), n As Integer, b As Double(,), m As Integer, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As Double(,))
            Dim emptya As Double(,) = New Double(-1, -1) {}
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim scalea As Double = 0

            info = 0
            x = New Double(-1, -1) {}


            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If

            '
            ' 1. scale matrix, max(|U[i,j]|)
            '    we assume that LU is in its normal form, i.e. |L[i,j]|<=1
            ' 2. solve
            '
            scalea = 0
            For i = 0 To n - 1
                For j = i To n - 1
                    scalea = System.Math.Max(scalea, System.Math.Abs(lua(i, j)))
                Next
            Next
            If CDbl(scalea) = CDbl(0) Then
                scalea = 1
            End If
            scalea = 1 / scalea
            rmatrixlusolveinternal(lua, p, scalea, n, emptya, False, _
                b, m, info, rep, x)
        End Sub


        '************************************************************************
        '        Dense solver.
        '
        '        This  subroutine  solves  a  system  A*x=b,  where BOTH ORIGINAL A AND ITS
        '        LU DECOMPOSITION ARE KNOWN. You can use it if for some  reasons  you  have
        '        both A and its LU decomposition.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * iterative refinement
        '        * O(N^2) complexity
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        '            P       -   array[0..N-1], pivots array, RMatrixLU result
        '            N       -   size of A
        '            B       -   array[0..N-1], right part
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolveM
        '            Rep     -   same as in RMatrixSolveM
        '            X       -   same as in RMatrixSolveM
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixmixedsolve(a As Double(,), lua As Double(,), p As Integer(), n As Integer, b As Double(), ByRef info As Integer, _
            rep As densesolverreport, ByRef x As Double())
            Dim bm As Double(,) = New Double(-1, -1) {}
            Dim xm As Double(,) = New Double(-1, -1) {}
            Dim i_ As Integer = 0

            info = 0
            x = New Double(-1) {}

            If n <= 0 Then
                info = -1
                Return
            End If
            bm = New Double(n - 1, 0) {}
            For i_ = 0 To n - 1
                bm(i_, 0) = b(i_)
            Next
            rmatrixmixedsolvem(a, lua, p, n, bm, 1, _
                info, rep, xm)
            x = New Double(n - 1) {}
            For i_ = 0 To n - 1
                x(i_) = xm(i_, 0)
            Next
        End Sub


        '************************************************************************
        '        Dense solver.
        '
        '        Similar to RMatrixMixedSolve() but  solves task with multiple right  parts
        '        (where b and x are NxM matrices).
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * iterative refinement
        '        * O(M*N^2) complexity
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        '            P       -   array[0..N-1], pivots array, RMatrixLU result
        '            N       -   size of A
        '            B       -   array[0..N-1,0..M-1], right part
        '            M       -   right part size
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolveM
        '            Rep     -   same as in RMatrixSolveM
        '            X       -   same as in RMatrixSolveM
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixmixedsolvem(a As Double(,), lua As Double(,), p As Integer(), n As Integer, b As Double(,), m As Integer, _
            ByRef info As Integer, rep As densesolverreport, ByRef x As Double(,))
            Dim scalea As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            info = 0
            x = New Double(-1, -1) {}


            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If

            '
            ' 1. scale matrix, max(|A[i,j]|)
            ' 2. factorize scaled matrix
            ' 3. solve
            '
            scalea = 0
            For i = 0 To n - 1
                For j = 0 To n - 1
                    scalea = System.Math.Max(scalea, System.Math.Abs(a(i, j)))
                Next
            Next
            If CDbl(scalea) = CDbl(0) Then
                scalea = 1
            End If
            scalea = 1 / scalea
            rmatrixlusolveinternal(lua, p, scalea, n, a, True, _
                b, m, info, rep, x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixSolveM(), but for complex matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * iterative refinement
        '        * O(N^3+M*N^2) complexity
        '
        '        COMMERCIAL EDITION OF ALGLIB:
        '
        '          ! Commercial version of ALGLIB includes two  important  improvements  of
        '          ! this function, which can be used from C++ and C#:
        '          ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
        '          ! * multicore support
        '          !
        '          ! Intel MKL gives approximately constant  (with  respect  to  number  of
        '          ! worker threads) acceleration factor which depends on CPU  being  used,
        '          ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
        '          ! comparison.
        '          !
        '          ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
        '          ! * about 2-3x faster than ALGLIB for C++ without MKL
        '          ! * about 7-10x faster than "pure C#" edition of ALGLIB
        '          ! Difference in performance will be more striking  on  newer  CPU's with
        '          ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
        '          ! problem whose size is at least 128, with best  efficiency achieved for
        '          ! N's larger than 512.
        '          !
        '          ! Commercial edition of ALGLIB also supports multithreaded  acceleration
        '          ! of this function. We should note that LU decomposition  is  harder  to
        '          ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
        '          ! many internal synchronization points which can not be avoided. However
        '          ! parallelism starts to be profitable starting  from  N=1024,  achieving
        '          ! near-linear speedup for N=4096 or higher.
        '          !
        '          ! In order to use multicore features you have to:
        '          ! * use commercial version of ALGLIB
        '          ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
        '          !   multicore code will be used (for multicore support)
        '          !
        '          ! We recommend you to read 'Working with commercial version' section  of
        '          ! ALGLIB Reference Manual in order to find out how to  use  performance-
        '          ! related features provided by commercial edition of ALGLIB.
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            N       -   size of A
        '            B       -   array[0..N-1,0..M-1], right part
        '            M       -   right part size
        '            RFS     -   iterative refinement switch:
        '                        * True - refinement is used.
        '                          Less performance, more precision.
        '                        * False - refinement is not used.
        '                          More performance, less precision.
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub cmatrixsolvem(a As complex(,), n As Integer, b As complex(,), m As Integer, rfs As Boolean, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As complex(,))
            Dim da As complex(,) = New complex(-1, -1) {}
            Dim emptya As complex(,) = New complex(-1, -1) {}
            Dim p As Integer() = New Integer(-1) {}
            Dim scalea As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim i_ As Integer = 0

            info = 0
            x = New complex(-1, -1) {}


            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If
            da = New complex(n - 1, n - 1) {}

            '
            ' 1. scale matrix, max(|A[i,j]|)
            ' 2. factorize scaled matrix
            ' 3. solve
            '
            scalea = 0
            For i = 0 To n - 1
                For j = 0 To n - 1
                    scalea = System.Math.Max(scalea, Math.abscomplex(a(i, j)))
                Next
            Next
            If CDbl(scalea) = CDbl(0) Then
                scalea = 1
            End If
            scalea = 1 / scalea
            For i = 0 To n - 1
                For i_ = 0 To n - 1
                    da(i, i_) = a(i, i_)
                Next
            Next
            trfac.cmatrixlu(da, n, n, p)
            If rfs Then
                cmatrixlusolveinternal(da, p, scalea, n, a, True, _
                    b, m, info, rep, x)
            Else
                cmatrixlusolveinternal(da, p, scalea, n, emptya, False, _
                    b, m, info, rep, x)
            End If
        End Sub


        '************************************************************************
        '        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
        '        ************************************************************************

        Public Shared Sub _pexec_cmatrixsolvem(a As complex(,), n As Integer, b As complex(,), m As Integer, rfs As Boolean, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As complex(,))
            cmatrixsolvem(a, n, b, m, rfs, info, _
                rep, x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixSolve(), but for complex matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * iterative refinement
        '        * O(N^3) complexity
        '
        '        COMMERCIAL EDITION OF ALGLIB:
        '
        '          ! Commercial version of ALGLIB includes two  important  improvements  of
        '          ! this function, which can be used from C++ and C#:
        '          ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
        '          ! * multicore support
        '          !
        '          ! Intel MKL gives approximately constant  (with  respect  to  number  of
        '          ! worker threads) acceleration factor which depends on CPU  being  used,
        '          ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
        '          ! comparison.
        '          !
        '          ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
        '          ! * about 2-3x faster than ALGLIB for C++ without MKL
        '          ! * about 7-10x faster than "pure C#" edition of ALGLIB
        '          ! Difference in performance will be more striking  on  newer  CPU's with
        '          ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
        '          ! problem whose size is at least 128, with best  efficiency achieved for
        '          ! N's larger than 512.
        '          !
        '          ! Commercial edition of ALGLIB also supports multithreaded  acceleration
        '          ! of this function. We should note that LU decomposition  is  harder  to
        '          ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
        '          ! many internal synchronization points which can not be avoided. However
        '          ! parallelism starts to be profitable starting  from  N=1024,  achieving
        '          ! near-linear speedup for N=4096 or higher.
        '          !
        '          ! In order to use multicore features you have to:
        '          ! * use commercial version of ALGLIB
        '          ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
        '          !   multicore code will be used (for multicore support)
        '          !
        '          ! We recommend you to read 'Working with commercial version' section  of
        '          ! ALGLIB Reference Manual in order to find out how to  use  performance-
        '          ! related features provided by commercial edition of ALGLIB.
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            N       -   size of A
        '            B       -   array[0..N-1], right part
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub cmatrixsolve(a As complex(,), n As Integer, b As complex(), ByRef info As Integer, rep As densesolverreport, ByRef x As complex())
            Dim bm As complex(,) = New complex(-1, -1) {}
            Dim xm As complex(,) = New complex(-1, -1) {}
            Dim i_ As Integer = 0

            info = 0
            x = New complex(-1) {}

            If n <= 0 Then
                info = -1
                Return
            End If
            bm = New complex(n - 1, 0) {}
            For i_ = 0 To n - 1
                bm(i_, 0) = b(i_)
            Next
            cmatrixsolvem(a, n, bm, 1, True, info, _
                rep, xm)
            x = New complex(n - 1) {}
            For i_ = 0 To n - 1
                x(i_) = xm(i_, 0)
            Next
        End Sub


        '************************************************************************
        '        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
        '        ************************************************************************

        Public Shared Sub _pexec_cmatrixsolve(a As complex(,), n As Integer, b As complex(), ByRef info As Integer, rep As densesolverreport, ByRef x As complex())
            cmatrixsolve(a, n, b, info, rep, x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixLUSolveM(), but for complex matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * O(M*N^2) complexity
        '        * condition number estimation
        '
        '        No iterative refinement  is provided because exact form of original matrix
        '        is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        INPUT PARAMETERS
        '            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        '            P       -   array[0..N-1], pivots array, RMatrixLU result
        '            N       -   size of A
        '            B       -   array[0..N-1,0..M-1], right part
        '            M       -   right part size
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub cmatrixlusolvem(lua As complex(,), p As Integer(), n As Integer, b As complex(,), m As Integer, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As complex(,))
            Dim emptya As complex(,) = New complex(-1, -1) {}
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim scalea As Double = 0

            info = 0
            x = New complex(-1, -1) {}


            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If

            '
            ' 1. scale matrix, max(|U[i,j]|)
            '    we assume that LU is in its normal form, i.e. |L[i,j]|<=1
            ' 2. solve
            '
            scalea = 0
            For i = 0 To n - 1
                For j = i To n - 1
                    scalea = System.Math.Max(scalea, Math.abscomplex(lua(i, j)))
                Next
            Next
            If CDbl(scalea) = CDbl(0) Then
                scalea = 1
            End If
            scalea = 1 / scalea
            cmatrixlusolveinternal(lua, p, scalea, n, emptya, False, _
                b, m, info, rep, x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixLUSolve(), but for complex matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * O(N^2) complexity
        '        * condition number estimation
        '
        '        No iterative refinement is provided because exact form of original matrix
        '        is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        INPUT PARAMETERS
        '            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        '            P       -   array[0..N-1], pivots array, CMatrixLU result
        '            N       -   size of A
        '            B       -   array[0..N-1], right part
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub cmatrixlusolve(lua As complex(,), p As Integer(), n As Integer, b As complex(), ByRef info As Integer, rep As densesolverreport, _
            ByRef x As complex())
            Dim bm As complex(,) = New complex(-1, -1) {}
            Dim xm As complex(,) = New complex(-1, -1) {}
            Dim i_ As Integer = 0

            info = 0
            x = New complex(-1) {}

            If n <= 0 Then
                info = -1
                Return
            End If
            bm = New complex(n - 1, 0) {}
            For i_ = 0 To n - 1
                bm(i_, 0) = b(i_)
            Next
            cmatrixlusolvem(lua, p, n, bm, 1, info, _
                rep, xm)
            x = New complex(n - 1) {}
            For i_ = 0 To n - 1
                x(i_) = xm(i_, 0)
            Next
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixMixedSolveM(), but for complex matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * iterative refinement
        '        * O(M*N^2) complexity
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        '            P       -   array[0..N-1], pivots array, CMatrixLU result
        '            N       -   size of A
        '            B       -   array[0..N-1,0..M-1], right part
        '            M       -   right part size
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolveM
        '            Rep     -   same as in RMatrixSolveM
        '            X       -   same as in RMatrixSolveM
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub cmatrixmixedsolvem(a As complex(,), lua As complex(,), p As Integer(), n As Integer, b As complex(,), m As Integer, _
            ByRef info As Integer, rep As densesolverreport, ByRef x As complex(,))
            Dim scalea As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            info = 0
            x = New complex(-1, -1) {}


            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If

            '
            ' 1. scale matrix, max(|A[i,j]|)
            ' 2. factorize scaled matrix
            ' 3. solve
            '
            scalea = 0
            For i = 0 To n - 1
                For j = 0 To n - 1
                    scalea = System.Math.Max(scalea, Math.abscomplex(a(i, j)))
                Next
            Next
            If CDbl(scalea) = CDbl(0) Then
                scalea = 1
            End If
            scalea = 1 / scalea
            cmatrixlusolveinternal(lua, p, scalea, n, a, True, _
                b, m, info, rep, x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixMixedSolve(), but for complex matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * iterative refinement
        '        * O(N^2) complexity
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        '            P       -   array[0..N-1], pivots array, CMatrixLU result
        '            N       -   size of A
        '            B       -   array[0..N-1], right part
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolveM
        '            Rep     -   same as in RMatrixSolveM
        '            X       -   same as in RMatrixSolveM
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub cmatrixmixedsolve(a As complex(,), lua As complex(,), p As Integer(), n As Integer, b As complex(), ByRef info As Integer, _
            rep As densesolverreport, ByRef x As complex())
            Dim bm As complex(,) = New complex(-1, -1) {}
            Dim xm As complex(,) = New complex(-1, -1) {}
            Dim i_ As Integer = 0

            info = 0
            x = New complex(-1) {}

            If n <= 0 Then
                info = -1
                Return
            End If
            bm = New complex(n - 1, 0) {}
            For i_ = 0 To n - 1
                bm(i_, 0) = b(i_)
            Next
            cmatrixmixedsolvem(a, lua, p, n, bm, 1, _
                info, rep, xm)
            x = New complex(n - 1) {}
            For i_ = 0 To n - 1
                x(i_) = xm(i_, 0)
            Next
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixSolveM(), but for symmetric positive definite
        '        matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * O(N^3+M*N^2) complexity
        '        * matrix is represented by its upper or lower triangle
        '
        '        No iterative refinement is provided because such partial representation of
        '        matrix does not allow efficient calculation of extra-precise  matrix-vector
        '        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        COMMERCIAL EDITION OF ALGLIB:
        '
        '          ! Commercial version of ALGLIB includes two  important  improvements  of
        '          ! this function, which can be used from C++ and C#:
        '          ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
        '          ! * multicore support
        '          !
        '          ! Intel MKL gives approximately constant  (with  respect  to  number  of
        '          ! worker threads) acceleration factor which depends on CPU  being  used,
        '          ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
        '          ! comparison.
        '          !
        '          ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
        '          ! * about 2-3x faster than ALGLIB for C++ without MKL
        '          ! * about 7-10x faster than "pure C#" edition of ALGLIB
        '          ! Difference in performance will be more striking  on  newer  CPU's with
        '          ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
        '          ! problem whose size is at least 128, with best  efficiency achieved for
        '          ! N's larger than 512.
        '          !
        '          ! Commercial edition of ALGLIB also supports multithreaded  acceleration
        '          ! of this function. We should note that Cholesky decomposition is harder
        '          ! to parallelize than, say, matrix-matrix product - this  algorithm  has
        '          ! several synchronization points which  can  not  be  avoided.  However,
        '          ! parallelism starts to be profitable starting from N=500.
        '          !
        '          ! In order to use multicore features you have to:
        '          ! * use commercial version of ALGLIB
        '          ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
        '          !   multicore code will be used (for multicore support)
        '          !
        '          ! We recommend you to read 'Working with commercial version' section  of
        '          ! ALGLIB Reference Manual in order to find out how to  use  performance-
        '          ! related features provided by commercial edition of ALGLIB.
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            N       -   size of A
        '            IsUpper -   what half of A is provided
        '            B       -   array[0..N-1,0..M-1], right part
        '            M       -   right part size
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve.
        '                        Returns -3 for non-SPD matrices.
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub spdmatrixsolvem(a As Double(,), n As Integer, isupper As Boolean, b As Double(,), m As Integer, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As Double(,))
            Dim da As Double(,) = New Double(-1, -1) {}
            Dim sqrtscalea As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim j1 As Integer = 0
            Dim j2 As Integer = 0
            Dim i_ As Integer = 0

            info = 0
            x = New Double(-1, -1) {}


            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If
            da = New Double(n - 1, n - 1) {}

            '
            ' 1. scale matrix, max(|A[i,j]|)
            ' 2. factorize scaled matrix
            ' 3. solve
            '
            sqrtscalea = 0
            For i = 0 To n - 1
                If isupper Then
                    j1 = i
                    j2 = n - 1
                Else
                    j1 = 0
                    j2 = i
                End If
                For j = j1 To j2
                    sqrtscalea = System.Math.Max(sqrtscalea, System.Math.Abs(a(i, j)))
                Next
            Next
            If CDbl(sqrtscalea) = CDbl(0) Then
                sqrtscalea = 1
            End If
            sqrtscalea = 1 / sqrtscalea
            sqrtscalea = System.Math.sqrt(sqrtscalea)
            For i = 0 To n - 1
                If isupper Then
                    j1 = i
                    j2 = n - 1
                Else
                    j1 = 0
                    j2 = i
                End If
                For i_ = j1 To j2
                    da(i, i_) = a(i, i_)
                Next
            Next
            If Not trfac.spdmatrixcholesky(da, n, isupper) Then
                x = New Double(n - 1, m - 1) {}
                For i = 0 To n - 1
                    For j = 0 To m - 1
                        x(i, j) = 0
                    Next
                Next
                rep.r1 = 0
                rep.rinf = 0
                info = -3
                Return
            End If
            info = 1
            spdmatrixcholeskysolveinternal(da, sqrtscalea, n, isupper, a, True, _
                b, m, info, rep, x)
        End Sub


        '************************************************************************
        '        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
        '        ************************************************************************

        Public Shared Sub _pexec_spdmatrixsolvem(a As Double(,), n As Integer, isupper As Boolean, b As Double(,), m As Integer, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As Double(,))
            spdmatrixsolvem(a, n, isupper, b, m, info, _
                rep, x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixSolve(), but for SPD matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * O(N^3) complexity
        '        * matrix is represented by its upper or lower triangle
        '
        '        No iterative refinement is provided because such partial representation of
        '        matrix does not allow efficient calculation of extra-precise  matrix-vector
        '        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        COMMERCIAL EDITION OF ALGLIB:
        '
        '          ! Commercial version of ALGLIB includes two  important  improvements  of
        '          ! this function, which can be used from C++ and C#:
        '          ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
        '          ! * multicore support
        '          !
        '          ! Intel MKL gives approximately constant  (with  respect  to  number  of
        '          ! worker threads) acceleration factor which depends on CPU  being  used,
        '          ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
        '          ! comparison.
        '          !
        '          ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
        '          ! * about 2-3x faster than ALGLIB for C++ without MKL
        '          ! * about 7-10x faster than "pure C#" edition of ALGLIB
        '          ! Difference in performance will be more striking  on  newer  CPU's with
        '          ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
        '          ! problem whose size is at least 128, with best  efficiency achieved for
        '          ! N's larger than 512.
        '          !
        '          ! Commercial edition of ALGLIB also supports multithreaded  acceleration
        '          ! of this function. We should note that Cholesky decomposition is harder
        '          ! to parallelize than, say, matrix-matrix product - this  algorithm  has
        '          ! several synchronization points which  can  not  be  avoided.  However,
        '          ! parallelism starts to be profitable starting from N=500.
        '          !
        '          ! In order to use multicore features you have to:
        '          ! * use commercial version of ALGLIB
        '          ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
        '          !   multicore code will be used (for multicore support)
        '          !
        '          ! We recommend you to read 'Working with commercial version' section  of
        '          ! ALGLIB Reference Manual in order to find out how to  use  performance-
        '          ! related features provided by commercial edition of ALGLIB.
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            N       -   size of A
        '            IsUpper -   what half of A is provided
        '            B       -   array[0..N-1], right part
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '                        Returns -3 for non-SPD matrices.
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub spdmatrixsolve(a As Double(,), n As Integer, isupper As Boolean, b As Double(), ByRef info As Integer, rep As densesolverreport, _
            ByRef x As Double())
            Dim bm As Double(,) = New Double(-1, -1) {}
            Dim xm As Double(,) = New Double(-1, -1) {}
            Dim i_ As Integer = 0

            info = 0
            x = New Double(-1) {}

            If n <= 0 Then
                info = -1
                Return
            End If
            bm = New Double(n - 1, 0) {}
            For i_ = 0 To n - 1
                bm(i_, 0) = b(i_)
            Next
            spdmatrixsolvem(a, n, isupper, bm, 1, info, _
                rep, xm)
            x = New Double(n - 1) {}
            For i_ = 0 To n - 1
                x(i_) = xm(i_, 0)
            Next
        End Sub


        '************************************************************************
        '        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
        '        ************************************************************************

        Public Shared Sub _pexec_spdmatrixsolve(a As Double(,), n As Integer, isupper As Boolean, b As Double(), ByRef info As Integer, rep As densesolverreport, _
            ByRef x As Double())
            spdmatrixsolve(a, n, isupper, b, info, rep, _
                x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixLUSolveM(), but for SPD matrices  represented
        '        by their Cholesky decomposition.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * O(M*N^2) complexity
        '        * condition number estimation
        '        * matrix is represented by its upper or lower triangle
        '
        '        No iterative refinement is provided because such partial representation of
        '        matrix does not allow efficient calculation of extra-precise  matrix-vector
        '        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        INPUT PARAMETERS
        '            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
        '                        SPDMatrixCholesky result
        '            N       -   size of CHA
        '            IsUpper -   what half of CHA is provided
        '            B       -   array[0..N-1,0..M-1], right part
        '            M       -   right part size
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub spdmatrixcholeskysolvem(cha As Double(,), n As Integer, isupper As Boolean, b As Double(,), m As Integer, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As Double(,))
            Dim emptya As Double(,) = New Double(-1, -1) {}
            Dim sqrtscalea As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim j1 As Integer = 0
            Dim j2 As Integer = 0

            info = 0
            x = New Double(-1, -1) {}


            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If

            '
            ' 1. scale matrix, max(|U[i,j]|)
            ' 2. factorize scaled matrix
            ' 3. solve
            '
            sqrtscalea = 0
            For i = 0 To n - 1
                If isupper Then
                    j1 = i
                    j2 = n - 1
                Else
                    j1 = 0
                    j2 = i
                End If
                For j = j1 To j2
                    sqrtscalea = System.Math.Max(sqrtscalea, System.Math.Abs(cha(i, j)))
                Next
            Next
            If CDbl(sqrtscalea) = CDbl(0) Then
                sqrtscalea = 1
            End If
            sqrtscalea = 1 / sqrtscalea
            spdmatrixcholeskysolveinternal(cha, sqrtscalea, n, isupper, emptya, False, _
                b, m, info, rep, x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixLUSolve(), but for  SPD matrices  represented
        '        by their Cholesky decomposition.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * O(N^2) complexity
        '        * condition number estimation
        '        * matrix is represented by its upper or lower triangle
        '
        '        No iterative refinement is provided because such partial representation of
        '        matrix does not allow efficient calculation of extra-precise  matrix-vector
        '        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        INPUT PARAMETERS
        '            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
        '                        SPDMatrixCholesky result
        '            N       -   size of A
        '            IsUpper -   what half of CHA is provided
        '            B       -   array[0..N-1], right part
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub spdmatrixcholeskysolve(cha As Double(,), n As Integer, isupper As Boolean, b As Double(), ByRef info As Integer, rep As densesolverreport, _
            ByRef x As Double())
            Dim bm As Double(,) = New Double(-1, -1) {}
            Dim xm As Double(,) = New Double(-1, -1) {}
            Dim i_ As Integer = 0

            info = 0
            x = New Double(-1) {}

            If n <= 0 Then
                info = -1
                Return
            End If
            bm = New Double(n - 1, 0) {}
            For i_ = 0 To n - 1
                bm(i_, 0) = b(i_)
            Next
            spdmatrixcholeskysolvem(cha, n, isupper, bm, 1, info, _
                rep, xm)
            x = New Double(n - 1) {}
            For i_ = 0 To n - 1
                x(i_) = xm(i_, 0)
            Next
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixSolveM(), but for Hermitian positive definite
        '        matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * O(N^3+M*N^2) complexity
        '        * matrix is represented by its upper or lower triangle
        '
        '        No iterative refinement is provided because such partial representation of
        '        matrix does not allow efficient calculation of extra-precise  matrix-vector
        '        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        COMMERCIAL EDITION OF ALGLIB:
        '
        '          ! Commercial version of ALGLIB includes two  important  improvements  of
        '          ! this function, which can be used from C++ and C#:
        '          ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
        '          ! * multicore support
        '          !
        '          ! Intel MKL gives approximately constant  (with  respect  to  number  of
        '          ! worker threads) acceleration factor which depends on CPU  being  used,
        '          ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
        '          ! comparison.
        '          !
        '          ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
        '          ! * about 2-3x faster than ALGLIB for C++ without MKL
        '          ! * about 7-10x faster than "pure C#" edition of ALGLIB
        '          ! Difference in performance will be more striking  on  newer  CPU's with
        '          ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
        '          ! problem whose size is at least 128, with best  efficiency achieved for
        '          ! N's larger than 512.
        '          !
        '          ! Commercial edition of ALGLIB also supports multithreaded  acceleration
        '          ! of this function. We should note that Cholesky decomposition is harder
        '          ! to parallelize than, say, matrix-matrix product - this  algorithm  has
        '          ! several synchronization points which  can  not  be  avoided.  However,
        '          ! parallelism starts to be profitable starting from N=500.
        '          !
        '          ! In order to use multicore features you have to:
        '          ! * use commercial version of ALGLIB
        '          ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
        '          !   multicore code will be used (for multicore support)
        '          !
        '          ! We recommend you to read 'Working with commercial version' section  of
        '          ! ALGLIB Reference Manual in order to find out how to  use  performance-
        '          ! related features provided by commercial edition of ALGLIB.
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            N       -   size of A
        '            IsUpper -   what half of A is provided
        '            B       -   array[0..N-1,0..M-1], right part
        '            M       -   right part size
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve.
        '                        Returns -3 for non-HPD matrices.
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub hpdmatrixsolvem(a As complex(,), n As Integer, isupper As Boolean, b As complex(,), m As Integer, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As complex(,))
            Dim da As complex(,) = New complex(-1, -1) {}
            Dim sqrtscalea As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim j1 As Integer = 0
            Dim j2 As Integer = 0
            Dim i_ As Integer = 0

            info = 0
            x = New complex(-1, -1) {}


            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If
            da = New complex(n - 1, n - 1) {}

            '
            ' 1. scale matrix, max(|A[i,j]|)
            ' 2. factorize scaled matrix
            ' 3. solve
            '
            sqrtscalea = 0
            For i = 0 To n - 1
                If isupper Then
                    j1 = i
                    j2 = n - 1
                Else
                    j1 = 0
                    j2 = i
                End If
                For j = j1 To j2
                    sqrtscalea = System.Math.Max(sqrtscalea, Math.abscomplex(a(i, j)))
                Next
            Next
            If CDbl(sqrtscalea) = CDbl(0) Then
                sqrtscalea = 1
            End If
            sqrtscalea = 1 / sqrtscalea
            sqrtscalea = System.Math.sqrt(sqrtscalea)
            For i = 0 To n - 1
                If isupper Then
                    j1 = i
                    j2 = n - 1
                Else
                    j1 = 0
                    j2 = i
                End If
                For i_ = j1 To j2
                    da(i, i_) = a(i, i_)
                Next
            Next
            If Not trfac.hpdmatrixcholesky(da, n, isupper) Then
                x = New complex(n - 1, m - 1) {}
                For i = 0 To n - 1
                    For j = 0 To m - 1
                        x(i, j) = 0
                    Next
                Next
                rep.r1 = 0
                rep.rinf = 0
                info = -3
                Return
            End If
            info = 1
            hpdmatrixcholeskysolveinternal(da, sqrtscalea, n, isupper, a, True, _
                b, m, info, rep, x)
        End Sub


        '************************************************************************
        '        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
        '        ************************************************************************

        Public Shared Sub _pexec_hpdmatrixsolvem(a As complex(,), n As Integer, isupper As Boolean, b As complex(,), m As Integer, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As complex(,))
            hpdmatrixsolvem(a, n, isupper, b, m, info, _
                rep, x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixSolve(),  but for Hermitian positive definite
        '        matrices.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * condition number estimation
        '        * O(N^3) complexity
        '        * matrix is represented by its upper or lower triangle
        '
        '        No iterative refinement is provided because such partial representation of
        '        matrix does not allow efficient calculation of extra-precise  matrix-vector
        '        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        COMMERCIAL EDITION OF ALGLIB:
        '
        '          ! Commercial version of ALGLIB includes two  important  improvements  of
        '          ! this function, which can be used from C++ and C#:
        '          ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
        '          ! * multicore support
        '          !
        '          ! Intel MKL gives approximately constant  (with  respect  to  number  of
        '          ! worker threads) acceleration factor which depends on CPU  being  used,
        '          ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
        '          ! comparison.
        '          !
        '          ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
        '          ! * about 2-3x faster than ALGLIB for C++ without MKL
        '          ! * about 7-10x faster than "pure C#" edition of ALGLIB
        '          ! Difference in performance will be more striking  on  newer  CPU's with
        '          ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
        '          ! problem whose size is at least 128, with best  efficiency achieved for
        '          ! N's larger than 512.
        '          !
        '          ! Commercial edition of ALGLIB also supports multithreaded  acceleration
        '          ! of this function. We should note that Cholesky decomposition is harder
        '          ! to parallelize than, say, matrix-matrix product - this  algorithm  has
        '          ! several synchronization points which  can  not  be  avoided.  However,
        '          ! parallelism starts to be profitable starting from N=500.
        '          !
        '          ! In order to use multicore features you have to:
        '          ! * use commercial version of ALGLIB
        '          ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
        '          !   multicore code will be used (for multicore support)
        '          !
        '          ! We recommend you to read 'Working with commercial version' section  of
        '          ! ALGLIB Reference Manual in order to find out how to  use  performance-
        '          ! related features provided by commercial edition of ALGLIB.
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..N-1,0..N-1], system matrix
        '            N       -   size of A
        '            IsUpper -   what half of A is provided
        '            B       -   array[0..N-1], right part
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '                        Returns -3 for non-HPD matrices.
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub hpdmatrixsolve(a As complex(,), n As Integer, isupper As Boolean, b As complex(), ByRef info As Integer, rep As densesolverreport, _
            ByRef x As complex())
            Dim bm As complex(,) = New complex(-1, -1) {}
            Dim xm As complex(,) = New complex(-1, -1) {}
            Dim i_ As Integer = 0

            info = 0
            x = New complex(-1) {}

            If n <= 0 Then
                info = -1
                Return
            End If
            bm = New complex(n - 1, 0) {}
            For i_ = 0 To n - 1
                bm(i_, 0) = b(i_)
            Next
            hpdmatrixsolvem(a, n, isupper, bm, 1, info, _
                rep, xm)
            x = New complex(n - 1) {}
            For i_ = 0 To n - 1
                x(i_) = xm(i_, 0)
            Next
        End Sub


        '************************************************************************
        '        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
        '        ************************************************************************

        Public Shared Sub _pexec_hpdmatrixsolve(a As complex(,), n As Integer, isupper As Boolean, b As complex(), ByRef info As Integer, rep As densesolverreport, _
            ByRef x As complex())
            hpdmatrixsolve(a, n, isupper, b, info, rep, _
                x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixLUSolveM(), but for HPD matrices  represented
        '        by their Cholesky decomposition.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * O(M*N^2) complexity
        '        * condition number estimation
        '        * matrix is represented by its upper or lower triangle
        '
        '        No iterative refinement is provided because such partial representation of
        '        matrix does not allow efficient calculation of extra-precise  matrix-vector
        '        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        INPUT PARAMETERS
        '            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
        '                        HPDMatrixCholesky result
        '            N       -   size of CHA
        '            IsUpper -   what half of CHA is provided
        '            B       -   array[0..N-1,0..M-1], right part
        '            M       -   right part size
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub hpdmatrixcholeskysolvem(cha As complex(,), n As Integer, isupper As Boolean, b As complex(,), m As Integer, ByRef info As Integer, _
            rep As densesolverreport, ByRef x As complex(,))
            Dim emptya As complex(,) = New complex(-1, -1) {}
            Dim sqrtscalea As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim j1 As Integer = 0
            Dim j2 As Integer = 0

            info = 0
            x = New complex(-1, -1) {}


            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If

            '
            ' 1. scale matrix, max(|U[i,j]|)
            ' 2. factorize scaled matrix
            ' 3. solve
            '
            sqrtscalea = 0
            For i = 0 To n - 1
                If isupper Then
                    j1 = i
                    j2 = n - 1
                Else
                    j1 = 0
                    j2 = i
                End If
                For j = j1 To j2
                    sqrtscalea = System.Math.Max(sqrtscalea, Math.abscomplex(cha(i, j)))
                Next
            Next
            If CDbl(sqrtscalea) = CDbl(0) Then
                sqrtscalea = 1
            End If
            sqrtscalea = 1 / sqrtscalea
            hpdmatrixcholeskysolveinternal(cha, sqrtscalea, n, isupper, emptya, False, _
                b, m, info, rep, x)
        End Sub


        '************************************************************************
        '        Dense solver. Same as RMatrixLUSolve(), but for  HPD matrices  represented
        '        by their Cholesky decomposition.
        '
        '        Algorithm features:
        '        * automatic detection of degenerate cases
        '        * O(N^2) complexity
        '        * condition number estimation
        '        * matrix is represented by its upper or lower triangle
        '
        '        No iterative refinement is provided because such partial representation of
        '        matrix does not allow efficient calculation of extra-precise  matrix-vector
        '        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        '        need iterative refinement.
        '
        '        INPUT PARAMETERS
        '            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
        '                        SPDMatrixCholesky result
        '            N       -   size of A
        '            IsUpper -   what half of CHA is provided
        '            B       -   array[0..N-1], right part
        '
        '        OUTPUT PARAMETERS
        '            Info    -   same as in RMatrixSolve
        '            Rep     -   same as in RMatrixSolve
        '            X       -   same as in RMatrixSolve
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub hpdmatrixcholeskysolve(cha As complex(,), n As Integer, isupper As Boolean, b As complex(), ByRef info As Integer, rep As densesolverreport, _
            ByRef x As complex())
            Dim bm As complex(,) = New complex(-1, -1) {}
            Dim xm As complex(,) = New complex(-1, -1) {}
            Dim i_ As Integer = 0

            info = 0
            x = New complex(-1) {}

            If n <= 0 Then
                info = -1
                Return
            End If
            bm = New complex(n - 1, 0) {}
            For i_ = 0 To n - 1
                bm(i_, 0) = b(i_)
            Next
            hpdmatrixcholeskysolvem(cha, n, isupper, bm, 1, info, _
                rep, xm)
            x = New complex(n - 1) {}
            For i_ = 0 To n - 1
                x(i_) = xm(i_, 0)
            Next
        End Sub


        '************************************************************************
        '        Dense solver.
        '
        '        This subroutine finds solution of the linear system A*X=B with non-square,
        '        possibly degenerate A.  System  is  solved in the least squares sense, and
        '        general least squares solution  X = X0 + CX*y  which  minimizes |A*X-B| is
        '        returned. If A is non-degenerate, solution in the usual sense is returned.
        '
        '        Algorithm features:
        '        * automatic detection (and correct handling!) of degenerate cases
        '        * iterative refinement
        '        * O(N^3) complexity
        '
        '        COMMERCIAL EDITION OF ALGLIB:
        '
        '          ! Commercial version of ALGLIB includes one  important  improvement   of
        '          ! this function, which can be used from C++ and C#:
        '          ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
        '          !
        '          ! Intel MKL gives approximately constant  (with  respect  to  number  of
        '          ! worker threads) acceleration factor which depends on CPU  being  used,
        '          ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
        '          ! comparison.
        '          !
        '          ! Generally, commercial ALGLIB is several times faster than  open-source
        '          ! generic C edition, and many times faster than open-source C# edition.
        '          !
        '          ! Multithreaded acceleration is only partially supported (some parts are
        '          ! optimized, but most - are not).
        '          !
        '          ! We recommend you to read 'Working with commercial version' section  of
        '          ! ALGLIB Reference Manual in order to find out how to  use  performance-
        '          ! related features provided by commercial edition of ALGLIB.
        '
        '        INPUT PARAMETERS
        '            A       -   array[0..NRows-1,0..NCols-1], system matrix
        '            NRows   -   vertical size of A
        '            NCols   -   horizontal size of A
        '            B       -   array[0..NCols-1], right part
        '            Threshold-  a number in [0,1]. Singular values  beyond  Threshold  are
        '                        considered  zero.  Set  it to 0.0, if you don't understand
        '                        what it means, so the solver will choose good value on its
        '                        own.
        '                        
        '        OUTPUT PARAMETERS
        '            Info    -   return code:
        '                        * -4    SVD subroutine failed
        '                        * -1    if NRows<=0 or NCols<=0 or Threshold<0 was passed
        '                        *  1    if task is solved
        '            Rep     -   solver report, see below for more info
        '            X       -   array[0..N-1,0..M-1], it contains:
        '                        * solution of A*X=B (even for singular A)
        '                        * zeros, if SVD subroutine failed
        '
        '        SOLVER REPORT
        '
        '        Subroutine sets following fields of the Rep structure:
        '        * R2        reciprocal of condition number: 1/cond(A), 2-norm.
        '        * N         = NCols
        '        * K         dim(Null(A))
        '        * CX        array[0..N-1,0..K-1], kernel of A.
        '                    Columns of CX store such vectors that A*CX[i]=0.
        '
        '          -- ALGLIB --
        '             Copyright 24.08.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub rmatrixsolvels(a As Double(,), nrows As Integer, ncols As Integer, b As Double(), threshold As Double, ByRef info As Integer, _
            rep As densesolverlsreport, ByRef x As Double())
            Dim sv As Double() = New Double(-1) {}
            Dim u As Double(,) = New Double(-1, -1) {}
            Dim vt As Double(,) = New Double(-1, -1) {}
            Dim rp As Double() = New Double(-1) {}
            Dim utb As Double() = New Double(-1) {}
            Dim sutb As Double() = New Double(-1) {}
            Dim tmp As Double() = New Double(-1) {}
            Dim ta As Double() = New Double(-1) {}
            Dim tx As Double() = New Double(-1) {}
            Dim buf As Double() = New Double(-1) {}
            Dim w As Double() = New Double(-1) {}
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim nsv As Integer = 0
            Dim kernelidx As Integer = 0
            Dim v As Double = 0
            Dim verr As Double = 0
            Dim svdfailed As New Boolean()
            Dim zeroa As New Boolean()
            Dim rfs As Integer = 0
            Dim nrfs As Integer = 0
            Dim terminatenexttime As New Boolean()
            Dim smallerr As New Boolean()
            Dim i_ As Integer = 0

            info = 0
            x = New Double(-1) {}

            If (nrows <= 0 OrElse ncols <= 0) OrElse CDbl(threshold) < CDbl(0) Then
                info = -1
                Return
            End If
            If CDbl(threshold) = CDbl(0) Then
                threshold = 1000 * Math.machineepsilon
            End If

            '
            ' Factorize A first
            '
            svdfailed = Not svd.rmatrixsvd(a, nrows, ncols, 1, 2, 2, _
                sv, u, vt)
            zeroa = CDbl(sv(0)) = CDbl(0)
            If svdfailed OrElse zeroa Then
                If svdfailed Then
                    info = -4
                Else
                    info = 1
                End If
                x = New Double(ncols - 1) {}
                For i = 0 To ncols - 1
                    x(i) = 0
                Next
                rep.n = ncols
                rep.k = ncols
                rep.cx = New Double(ncols - 1, ncols - 1) {}
                For i = 0 To ncols - 1
                    For j = 0 To ncols - 1
                        If i = j Then
                            rep.cx(i, j) = 1
                        Else
                            rep.cx(i, j) = 0
                        End If
                    Next
                Next
                rep.r2 = 0
                Return
            End If
            nsv = System.Math.Min(ncols, nrows)
            If nsv = ncols Then
                rep.r2 = sv(nsv - 1) / sv(0)
            Else
                rep.r2 = 0
            End If
            rep.n = ncols
            info = 1

            '
            ' Iterative refinement of xc combined with solution:
            ' 1. xc = 0
            ' 2. calculate r = bc-A*xc using extra-precise dot product
            ' 3. solve A*y = r
            ' 4. update x:=x+r
            ' 5. goto 2
            '
            ' This cycle is executed until one of two things happens:
            ' 1. maximum number of iterations reached
            ' 2. last iteration decreased error to the lower limit
            '
            utb = New Double(nsv - 1) {}
            sutb = New Double(nsv - 1) {}
            x = New Double(ncols - 1) {}
            tmp = New Double(ncols - 1) {}
            ta = New Double(ncols) {}
            tx = New Double(ncols) {}
            buf = New Double(ncols) {}
            For i = 0 To ncols - 1
                x(i) = 0
            Next
            kernelidx = nsv
            For i = 0 To nsv - 1
                If CDbl(sv(i)) <= CDbl(threshold * sv(0)) Then
                    kernelidx = i
                    Exit For
                End If
            Next
            rep.k = ncols - kernelidx
            nrfs = densesolverrfsmaxv2(ncols, rep.r2)
            terminatenexttime = False
            rp = New Double(nrows - 1) {}
            For rfs = 0 To nrfs
                If terminatenexttime Then
                    Exit For
                End If

                '
                ' calculate right part
                '
                If rfs = 0 Then
                    For i_ = 0 To nrows - 1
                        rp(i_) = b(i_)
                    Next
                Else
                    smallerr = True
                    For i = 0 To nrows - 1
                        For i_ = 0 To ncols - 1
                            ta(i_) = a(i, i_)
                        Next
                        ta(ncols) = -1
                        For i_ = 0 To ncols - 1
                            tx(i_) = x(i_)
                        Next
                        tx(ncols) = b(i)
                        xblas.xdot(ta, tx, ncols + 1, buf, v, verr)
                        rp(i) = -v
                        smallerr = smallerr AndAlso CDbl(System.Math.Abs(v)) < CDbl(4 * verr)
                    Next
                    If smallerr Then
                        terminatenexttime = True
                    End If
                End If

                '
                ' solve A*dx = rp
                '
                For i = 0 To ncols - 1
                    tmp(i) = 0
                Next
                For i = 0 To nsv - 1
                    utb(i) = 0
                Next
                For i = 0 To nrows - 1
                    v = rp(i)
                    For i_ = 0 To nsv - 1
                        utb(i_) = utb(i_) + v * u(i, i_)
                    Next
                Next
                For i = 0 To nsv - 1
                    If i < kernelidx Then
                        sutb(i) = utb(i) / sv(i)
                    Else
                        sutb(i) = 0
                    End If
                Next
                For i = 0 To nsv - 1
                    v = sutb(i)
                    For i_ = 0 To ncols - 1
                        tmp(i_) = tmp(i_) + v * vt(i, i_)
                    Next
                Next

                '
                ' update x:  x:=x+dx
                '
                For i_ = 0 To ncols - 1
                    x(i_) = x(i_) + tmp(i_)
                Next
            Next

            '
            ' fill CX
            '
            If rep.k > 0 Then
                rep.cx = New Double(ncols - 1, rep.k - 1) {}
                For i = 0 To rep.k - 1
                    For i_ = 0 To ncols - 1
                        rep.cx(i_, i) = vt(kernelidx + i, i_)
                    Next
                Next
            End If
        End Sub


        '************************************************************************
        '        Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
        '        ************************************************************************

        Public Shared Sub _pexec_rmatrixsolvels(a As Double(,), nrows As Integer, ncols As Integer, b As Double(), threshold As Double, ByRef info As Integer, _
            rep As densesolverlsreport, ByRef x As Double())
            rmatrixsolvels(a, nrows, ncols, b, threshold, info, _
                rep, x)
        End Sub


        '************************************************************************
        '        Internal LU solver
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub rmatrixlusolveinternal(lua As Double(,), p As Integer(), scalea As Double, n As Integer, a As Double(,), havea As Boolean, _
            b As Double(,), m As Integer, ByRef info As Integer, rep As densesolverreport, ByRef x As Double(,))
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim rfs As Integer = 0
            Dim nrfs As Integer = 0
            Dim xc As Double() = New Double(-1) {}
            Dim y As Double() = New Double(-1) {}
            Dim bc As Double() = New Double(-1) {}
            Dim xa As Double() = New Double(-1) {}
            Dim xb As Double() = New Double(-1) {}
            Dim tx As Double() = New Double(-1) {}
            Dim v As Double = 0
            Dim verr As Double = 0
            Dim mxb As Double = 0
            Dim scaleright As Double = 0
            Dim smallerr As New Boolean()
            Dim terminatenexttime As New Boolean()
            Dim i_ As Integer = 0

            info = 0
            x = New Double(-1, -1) {}

            alglib.ap.assert(CDbl(scalea) > CDbl(0))

            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If
            For i = 0 To n - 1
                If p(i) > n - 1 OrElse p(i) < i Then
                    info = -1
                    Return
                End If
            Next
            x = New Double(n - 1, m - 1) {}
            y = New Double(n - 1) {}
            xc = New Double(n - 1) {}
            bc = New Double(n - 1) {}
            tx = New Double(n) {}
            xa = New Double(n) {}
            xb = New Double(n) {}

            '
            ' estimate condition number, test for near singularity
            '
            rep.r1 = rcond.rmatrixlurcond1(lua, n)
            rep.rinf = rcond.rmatrixlurcondinf(lua, n)
            If CDbl(rep.r1) < CDbl(rcond.rcondthreshold()) OrElse CDbl(rep.rinf) < CDbl(rcond.rcondthreshold()) Then
                For i = 0 To n - 1
                    For j = 0 To m - 1
                        x(i, j) = 0
                    Next
                Next
                rep.r1 = 0
                rep.rinf = 0
                info = -3
                Return
            End If
            info = 1

            '
            ' solve
            '
            For k = 0 To m - 1

                '
                ' copy B to contiguous storage
                '
                For i_ = 0 To n - 1
                    bc(i_) = b(i_, k)
                Next

                '
                ' Scale right part:
                ' * MX stores max(|Bi|)
                ' * ScaleRight stores actual scaling applied to B when solving systems
                '   it is chosen to make |scaleRight*b| close to 1.
                '
                mxb = 0
                For i = 0 To n - 1
                    mxb = System.Math.Max(mxb, System.Math.Abs(bc(i)))
                Next
                If CDbl(mxb) = CDbl(0) Then
                    mxb = 1
                End If
                scaleright = 1 / mxb

                '
                ' First, non-iterative part of solution process.
                ' We use separate code for this task because
                ' XDot is quite slow and we want to save time.
                '
                For i_ = 0 To n - 1
                    xc(i_) = scaleright * bc(i_)
                Next
                rbasiclusolve(lua, p, scalea, n, xc, tx)

                '
                ' Iterative refinement of xc:
                ' * calculate r = bc-A*xc using extra-precise dot product
                ' * solve A*y = r
                ' * update x:=x+r
                '
                ' This cycle is executed until one of two things happens:
                ' 1. maximum number of iterations reached
                ' 2. last iteration decreased error to the lower limit
                '
                If havea Then
                    nrfs = densesolverrfsmax(n, rep.r1, rep.rinf)
                    terminatenexttime = False
                    For rfs = 0 To nrfs - 1
                        If terminatenexttime Then
                            Exit For
                        End If

                        '
                        ' generate right part
                        '
                        smallerr = True
                        For i_ = 0 To n - 1
                            xb(i_) = xc(i_)
                        Next
                        For i = 0 To n - 1
                            For i_ = 0 To n - 1
                                xa(i_) = scalea * a(i, i_)
                            Next
                            xa(n) = -1
                            xb(n) = scaleright * bc(i)
                            xblas.xdot(xa, xb, n + 1, tx, v, verr)
                            y(i) = -v
                            smallerr = smallerr AndAlso CDbl(System.Math.Abs(v)) < CDbl(4 * verr)
                        Next
                        If smallerr Then
                            terminatenexttime = True
                        End If

                        '
                        ' solve and update
                        '
                        rbasiclusolve(lua, p, scalea, n, y, tx)
                        For i_ = 0 To n - 1
                            xc(i_) = xc(i_) + y(i_)
                        Next
                    Next
                End If

                '
                ' Store xc.
                ' Post-scale result.
                '
                v = scalea * mxb
                For i_ = 0 To n - 1
                    x(i_, k) = v * xc(i_)
                Next
            Next
        End Sub


        '************************************************************************
        '        Internal Cholesky solver
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub spdmatrixcholeskysolveinternal(cha As Double(,), sqrtscalea As Double, n As Integer, isupper As Boolean, a As Double(,), havea As Boolean, _
            b As Double(,), m As Integer, ByRef info As Integer, rep As densesolverreport, ByRef x As Double(,))
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim xc As Double() = New Double(-1) {}
            Dim y As Double() = New Double(-1) {}
            Dim bc As Double() = New Double(-1) {}
            Dim xa As Double() = New Double(-1) {}
            Dim xb As Double() = New Double(-1) {}
            Dim tx As Double() = New Double(-1) {}
            Dim v As Double = 0
            Dim mxb As Double = 0
            Dim scaleright As Double = 0
            Dim i_ As Integer = 0

            info = 0
            x = New Double(-1, -1) {}

            alglib.ap.assert(CDbl(sqrtscalea) > CDbl(0))

            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If
            x = New Double(n - 1, m - 1) {}
            y = New Double(n - 1) {}
            xc = New Double(n - 1) {}
            bc = New Double(n - 1) {}
            tx = New Double(n) {}
            xa = New Double(n) {}
            xb = New Double(n) {}

            '
            ' estimate condition number, test for near singularity
            '
            rep.r1 = rcond.spdmatrixcholeskyrcond(cha, n, isupper)
            rep.rinf = rep.r1
            If CDbl(rep.r1) < CDbl(rcond.rcondthreshold()) Then
                For i = 0 To n - 1
                    For j = 0 To m - 1
                        x(i, j) = 0
                    Next
                Next
                rep.r1 = 0
                rep.rinf = 0
                info = -3
                Return
            End If
            info = 1

            '
            ' solve
            '
            For k = 0 To m - 1

                '
                ' copy B to contiguous storage
                '
                For i_ = 0 To n - 1
                    bc(i_) = b(i_, k)
                Next

                '
                ' Scale right part:
                ' * MX stores max(|Bi|)
                ' * ScaleRight stores actual scaling applied to B when solving systems
                '   it is chosen to make |scaleRight*b| close to 1.
                '
                mxb = 0
                For i = 0 To n - 1
                    mxb = System.Math.Max(mxb, System.Math.Abs(bc(i)))
                Next
                If CDbl(mxb) = CDbl(0) Then
                    mxb = 1
                End If
                scaleright = 1 / mxb

                '
                ' First, non-iterative part of solution process.
                ' We use separate code for this task because
                ' XDot is quite slow and we want to save time.
                '
                For i_ = 0 To n - 1
                    xc(i_) = scaleright * bc(i_)
                Next
                spdbasiccholeskysolve(cha, sqrtscalea, n, isupper, xc, tx)

                '
                ' Store xc.
                ' Post-scale result.
                '
                v = Math.sqr(sqrtscalea) * mxb
                For i_ = 0 To n - 1
                    x(i_, k) = v * xc(i_)
                Next
            Next
        End Sub


        '************************************************************************
        '        Internal LU solver
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub cmatrixlusolveinternal(lua As complex(,), p As Integer(), scalea As Double, n As Integer, a As complex(,), havea As Boolean, _
            b As complex(,), m As Integer, ByRef info As Integer, rep As densesolverreport, ByRef x As complex(,))
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim rfs As Integer = 0
            Dim nrfs As Integer = 0
            Dim xc As complex() = New complex(-1) {}
            Dim y As complex() = New complex(-1) {}
            Dim bc As complex() = New complex(-1) {}
            Dim xa As complex() = New complex(-1) {}
            Dim xb As complex() = New complex(-1) {}
            Dim tx As complex() = New complex(-1) {}
            Dim tmpbuf As Double() = New Double(-1) {}
            Dim v As complex = 0
            Dim verr As Double = 0
            Dim mxb As Double = 0
            Dim scaleright As Double = 0
            Dim smallerr As New Boolean()
            Dim terminatenexttime As New Boolean()
            Dim i_ As Integer = 0

            info = 0
            x = New complex(-1, -1) {}

            alglib.ap.assert(CDbl(scalea) > CDbl(0))

            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If
            For i = 0 To n - 1
                If p(i) > n - 1 OrElse p(i) < i Then
                    info = -1
                    Return
                End If
            Next
            x = New complex(n - 1, m - 1) {}
            y = New complex(n - 1) {}
            xc = New complex(n - 1) {}
            bc = New complex(n - 1) {}
            tx = New complex(n - 1) {}
            xa = New complex(n) {}
            xb = New complex(n) {}
            tmpbuf = New Double(2 * n + 1) {}

            '
            ' estimate condition number, test for near singularity
            '
            rep.r1 = rcond.cmatrixlurcond1(lua, n)
            rep.rinf = rcond.cmatrixlurcondinf(lua, n)
            If CDbl(rep.r1) < CDbl(rcond.rcondthreshold()) OrElse CDbl(rep.rinf) < CDbl(rcond.rcondthreshold()) Then
                For i = 0 To n - 1
                    For j = 0 To m - 1
                        x(i, j) = 0
                    Next
                Next
                rep.r1 = 0
                rep.rinf = 0
                info = -3
                Return
            End If
            info = 1

            '
            ' solve
            '
            For k = 0 To m - 1

                '
                ' copy B to contiguous storage
                '
                For i_ = 0 To n - 1
                    bc(i_) = b(i_, k)
                Next

                '
                ' Scale right part:
                ' * MX stores max(|Bi|)
                ' * ScaleRight stores actual scaling applied to B when solving systems
                '   it is chosen to make |scaleRight*b| close to 1.
                '
                mxb = 0
                For i = 0 To n - 1
                    mxb = System.Math.Max(mxb, Math.abscomplex(bc(i)))
                Next
                If CDbl(mxb) = CDbl(0) Then
                    mxb = 1
                End If
                scaleright = 1 / mxb

                '
                ' First, non-iterative part of solution process.
                ' We use separate code for this task because
                ' XDot is quite slow and we want to save time.
                '
                For i_ = 0 To n - 1
                    xc(i_) = scaleright * bc(i_)
                Next
                cbasiclusolve(lua, p, scalea, n, xc, tx)

                '
                ' Iterative refinement of xc:
                ' * calculate r = bc-A*xc using extra-precise dot product
                ' * solve A*y = r
                ' * update x:=x+r
                '
                ' This cycle is executed until one of two things happens:
                ' 1. maximum number of iterations reached
                ' 2. last iteration decreased error to the lower limit
                '
                If havea Then
                    nrfs = densesolverrfsmax(n, rep.r1, rep.rinf)
                    terminatenexttime = False
                    For rfs = 0 To nrfs - 1
                        If terminatenexttime Then
                            Exit For
                        End If

                        '
                        ' generate right part
                        '
                        smallerr = True
                        For i_ = 0 To n - 1
                            xb(i_) = xc(i_)
                        Next
                        For i = 0 To n - 1
                            For i_ = 0 To n - 1
                                xa(i_) = scalea * a(i, i_)
                            Next
                            xa(n) = -1
                            xb(n) = scaleright * bc(i)
                            xblas.xcdot(xa, xb, n + 1, tmpbuf, v, verr)
                            y(i) = -v
                            smallerr = smallerr AndAlso CDbl(Math.abscomplex(v)) < CDbl(4 * verr)
                        Next
                        If smallerr Then
                            terminatenexttime = True
                        End If

                        '
                        ' solve and update
                        '
                        cbasiclusolve(lua, p, scalea, n, y, tx)
                        For i_ = 0 To n - 1
                            xc(i_) = xc(i_) + y(i_)
                        Next
                    Next
                End If

                '
                ' Store xc.
                ' Post-scale result.
                '
                v = scalea * mxb
                For i_ = 0 To n - 1
                    x(i_, k) = v * xc(i_)
                Next
            Next
        End Sub


        '************************************************************************
        '        Internal Cholesky solver
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub hpdmatrixcholeskysolveinternal(cha As complex(,), sqrtscalea As Double, n As Integer, isupper As Boolean, a As complex(,), havea As Boolean, _
            b As complex(,), m As Integer, ByRef info As Integer, rep As densesolverreport, ByRef x As complex(,))
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim k As Integer = 0
            Dim xc As complex() = New complex(-1) {}
            Dim y As complex() = New complex(-1) {}
            Dim bc As complex() = New complex(-1) {}
            Dim xa As complex() = New complex(-1) {}
            Dim xb As complex() = New complex(-1) {}
            Dim tx As complex() = New complex(-1) {}
            Dim v As Double = 0
            Dim mxb As Double = 0
            Dim scaleright As Double = 0
            Dim i_ As Integer = 0

            info = 0
            x = New complex(-1, -1) {}

            alglib.ap.assert(CDbl(sqrtscalea) > CDbl(0))

            '
            ' prepare: check inputs, allocate space...
            '
            If n <= 0 OrElse m <= 0 Then
                info = -1
                Return
            End If
            x = New complex(n - 1, m - 1) {}
            y = New complex(n - 1) {}
            xc = New complex(n - 1) {}
            bc = New complex(n - 1) {}
            tx = New complex(n) {}
            xa = New complex(n) {}
            xb = New complex(n) {}

            '
            ' estimate condition number, test for near singularity
            '
            rep.r1 = rcond.hpdmatrixcholeskyrcond(cha, n, isupper)
            rep.rinf = rep.r1
            If CDbl(rep.r1) < CDbl(rcond.rcondthreshold()) Then
                For i = 0 To n - 1
                    For j = 0 To m - 1
                        x(i, j) = 0
                    Next
                Next
                rep.r1 = 0
                rep.rinf = 0
                info = -3
                Return
            End If
            info = 1

            '
            ' solve
            '
            For k = 0 To m - 1

                '
                ' copy B to contiguous storage
                '
                For i_ = 0 To n - 1
                    bc(i_) = b(i_, k)
                Next

                '
                ' Scale right part:
                ' * MX stores max(|Bi|)
                ' * ScaleRight stores actual scaling applied to B when solving systems
                '   it is chosen to make |scaleRight*b| close to 1.
                '
                mxb = 0
                For i = 0 To n - 1
                    mxb = System.Math.Max(mxb, Math.abscomplex(bc(i)))
                Next
                If CDbl(mxb) = CDbl(0) Then
                    mxb = 1
                End If
                scaleright = 1 / mxb

                '
                ' First, non-iterative part of solution process.
                ' We use separate code for this task because
                ' XDot is quite slow and we want to save time.
                '
                For i_ = 0 To n - 1
                    xc(i_) = scaleright * bc(i_)
                Next
                hpdbasiccholeskysolve(cha, sqrtscalea, n, isupper, xc, tx)

                '
                ' Store xc.
                ' Post-scale result.
                '
                v = Math.sqr(sqrtscalea) * mxb
                For i_ = 0 To n - 1
                    x(i_, k) = v * xc(i_)
                Next
            Next
        End Sub


        '************************************************************************
        '        Internal subroutine.
        '        Returns maximum count of RFS iterations as function of:
        '        1. machine epsilon
        '        2. task size.
        '        3. condition number
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Function densesolverrfsmax(n As Integer, r1 As Double, rinf As Double) As Integer
            Dim result As Integer = 0

            result = 5
            Return result
        End Function


        '************************************************************************
        '        Internal subroutine.
        '        Returns maximum count of RFS iterations as function of:
        '        1. machine epsilon
        '        2. task size.
        '        3. norm-2 condition number
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Function densesolverrfsmaxv2(n As Integer, r2 As Double) As Integer
            Dim result As Integer = 0

            result = densesolverrfsmax(n, 0, 0)
            Return result
        End Function


        '************************************************************************
        '        Basic LU solver for ScaleA*PLU*x = y.
        '
        '        This subroutine assumes that:
        '        * L is well-scaled, and it is U which needs scaling by ScaleA.
        '        * A=PLU is well-conditioned, so no zero divisions or overflow may occur
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub rbasiclusolve(lua As Double(,), p As Integer(), scalea As Double, n As Integer, ByRef xb As Double(), ByRef tmp As Double())
            Dim i As Integer = 0
            Dim v As Double = 0
            Dim i_ As Integer = 0

            For i = 0 To n - 1
                If p(i) <> i Then
                    v = xb(i)
                    xb(i) = xb(p(i))
                    xb(p(i)) = v
                End If
            Next
            For i = 1 To n - 1
                v = 0.0
                For i_ = 0 To i - 1
                    v += lua(i, i_) * xb(i_)
                Next
                xb(i) = xb(i) - v
            Next
            xb(n - 1) = xb(n - 1) / (scalea * lua(n - 1, n - 1))
            For i = n - 2 To 0 Step -1
                For i_ = i + 1 To n - 1
                    tmp(i_) = scalea * lua(i, i_)
                Next
                v = 0.0
                For i_ = i + 1 To n - 1
                    v += tmp(i_) * xb(i_)
                Next
                xb(i) = (xb(i) - v) / (scalea * lua(i, i))
            Next
        End Sub


        '************************************************************************
        '        Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.
        '
        '        This subroutine assumes that:
        '        * A*ScaleA is well scaled
        '        * A is well-conditioned, so no zero divisions or overflow may occur
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub spdbasiccholeskysolve(cha As Double(,), sqrtscalea As Double, n As Integer, isupper As Boolean, ByRef xb As Double(), ByRef tmp As Double())
            Dim i As Integer = 0
            Dim v As Double = 0
            Dim i_ As Integer = 0


            '
            ' A = L*L' or A=U'*U
            '
            If isupper Then

                '
                ' Solve U'*y=b first.
                '
                For i = 0 To n - 1
                    xb(i) = xb(i) / (sqrtscalea * cha(i, i))
                    If i < n - 1 Then
                        v = xb(i)
                        For i_ = i + 1 To n - 1
                            tmp(i_) = sqrtscalea * cha(i, i_)
                        Next
                        For i_ = i + 1 To n - 1
                            xb(i_) = xb(i_) - v * tmp(i_)
                        Next
                    End If
                Next

                '
                ' Solve U*x=y then.
                '
                For i = n - 1 To 0 Step -1
                    If i < n - 1 Then
                        For i_ = i + 1 To n - 1
                            tmp(i_) = sqrtscalea * cha(i, i_)
                        Next
                        v = 0.0
                        For i_ = i + 1 To n - 1
                            v += tmp(i_) * xb(i_)
                        Next
                        xb(i) = xb(i) - v
                    End If
                    xb(i) = xb(i) / (sqrtscalea * cha(i, i))
                Next
            Else

                '
                ' Solve L*y=b first
                '
                For i = 0 To n - 1
                    If i > 0 Then
                        For i_ = 0 To i - 1
                            tmp(i_) = sqrtscalea * cha(i, i_)
                        Next
                        v = 0.0
                        For i_ = 0 To i - 1
                            v += tmp(i_) * xb(i_)
                        Next
                        xb(i) = xb(i) - v
                    End If
                    xb(i) = xb(i) / (sqrtscalea * cha(i, i))
                Next

                '
                ' Solve L'*x=y then.
                '
                For i = n - 1 To 0 Step -1
                    xb(i) = xb(i) / (sqrtscalea * cha(i, i))
                    If i > 0 Then
                        v = xb(i)
                        For i_ = 0 To i - 1
                            tmp(i_) = sqrtscalea * cha(i, i_)
                        Next
                        For i_ = 0 To i - 1
                            xb(i_) = xb(i_) - v * tmp(i_)
                        Next
                    End If
                Next
            End If
        End Sub


        '************************************************************************
        '        Basic LU solver for ScaleA*PLU*x = y.
        '
        '        This subroutine assumes that:
        '        * L is well-scaled, and it is U which needs scaling by ScaleA.
        '        * A=PLU is well-conditioned, so no zero divisions or overflow may occur
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub cbasiclusolve(lua As complex(,), p As Integer(), scalea As Double, n As Integer, ByRef xb As complex(), ByRef tmp As complex())
            Dim i As Integer = 0
            Dim v As complex = 0
            Dim i_ As Integer = 0

            For i = 0 To n - 1
                If p(i) <> i Then
                    v = xb(i)
                    xb(i) = xb(p(i))
                    xb(p(i)) = v
                End If
            Next
            For i = 1 To n - 1
                v = 0.0
                For i_ = 0 To i - 1
                    v += lua(i, i_) * xb(i_)
                Next
                xb(i) = xb(i) - v
            Next
            xb(n - 1) = xb(n - 1) / (scalea * lua(n - 1, n - 1))
            For i = n - 2 To 0 Step -1
                For i_ = i + 1 To n - 1
                    tmp(i_) = scalea * lua(i, i_)
                Next
                v = 0.0
                For i_ = i + 1 To n - 1
                    v += tmp(i_) * xb(i_)
                Next
                xb(i) = (xb(i) - v) / (scalea * lua(i, i))
            Next
        End Sub


        '************************************************************************
        '        Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.
        '
        '        This subroutine assumes that:
        '        * A*ScaleA is well scaled
        '        * A is well-conditioned, so no zero divisions or overflow may occur
        '
        '          -- ALGLIB --
        '             Copyright 27.01.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub hpdbasiccholeskysolve(cha As complex(,), sqrtscalea As Double, n As Integer, isupper As Boolean, ByRef xb As complex(), ByRef tmp As complex())
            Dim i As Integer = 0
            Dim v As complex = 0
            Dim i_ As Integer = 0


            '
            ' A = L*L' or A=U'*U
            '
            If isupper Then

                '
                ' Solve U'*y=b first.
                '
                For i = 0 To n - 1
                    xb(i) = xb(i) / (sqrtscalea * Math.conj(cha(i, i)))
                    If i < n - 1 Then
                        v = xb(i)
                        For i_ = i + 1 To n - 1
                            tmp(i_) = sqrtscalea * Math.conj(cha(i, i_))
                        Next
                        For i_ = i + 1 To n - 1
                            xb(i_) = xb(i_) - v * tmp(i_)
                        Next
                    End If
                Next

                '
                ' Solve U*x=y then.
                '
                For i = n - 1 To 0 Step -1
                    If i < n - 1 Then
                        For i_ = i + 1 To n - 1
                            tmp(i_) = sqrtscalea * cha(i, i_)
                        Next
                        v = 0.0
                        For i_ = i + 1 To n - 1
                            v += tmp(i_) * xb(i_)
                        Next
                        xb(i) = xb(i) - v
                    End If
                    xb(i) = xb(i) / (sqrtscalea * cha(i, i))
                Next
            Else

                '
                ' Solve L*y=b first
                '
                For i = 0 To n - 1
                    If i > 0 Then
                        For i_ = 0 To i - 1
                            tmp(i_) = sqrtscalea * cha(i, i_)
                        Next
                        v = 0.0
                        For i_ = 0 To i - 1
                            v += tmp(i_) * xb(i_)
                        Next
                        xb(i) = xb(i) - v
                    End If
                    xb(i) = xb(i) / (sqrtscalea * cha(i, i))
                Next

                '
                ' Solve L'*x=y then.
                '
                For i = n - 1 To 0 Step -1
                    xb(i) = xb(i) / (sqrtscalea * Math.conj(cha(i, i)))
                    If i > 0 Then
                        v = xb(i)
                        For i_ = 0 To i - 1
                            tmp(i_) = sqrtscalea * Math.conj(cha(i, i_))
                        Next
                        For i_ = 0 To i - 1
                            xb(i_) = xb(i_) - v * tmp(i_)
                        Next
                    End If
                Next
            End If
        End Sub


    End Class
    Public Class linlsqr
        '************************************************************************
        '        This object stores state of the LinLSQR method.
        '
        '        You should use ALGLIB functions to work with this object.
        '        ************************************************************************

        Public Class linlsqrstate
            Inherits apobject
            Public nes As normestimator.normestimatorstate
            Public rx As Double()
            Public b As Double()
            Public n As Integer
            Public m As Integer
            Public prectype As Integer
            Public ui As Double()
            Public uip1 As Double()
            Public vi As Double()
            Public vip1 As Double()
            Public omegai As Double()
            Public omegaip1 As Double()
            Public alphai As Double
            Public alphaip1 As Double
            Public betai As Double
            Public betaip1 As Double
            Public phibari As Double
            Public phibarip1 As Double
            Public phii As Double
            Public rhobari As Double
            Public rhobarip1 As Double
            Public rhoi As Double
            Public ci As Double
            Public si As Double
            Public theta As Double
            Public lambdai As Double
            Public d As Double()
            Public anorm As Double
            Public bnorm2 As Double
            Public dnorm As Double
            Public r2 As Double
            Public x As Double()
            Public mv As Double()
            Public mtv As Double()
            Public epsa As Double
            Public epsb As Double
            Public epsc As Double
            Public maxits As Integer
            Public xrep As Boolean
            Public xupdated As Boolean
            Public needmv As Boolean
            Public needmtv As Boolean
            Public needmv2 As Boolean
            Public needvmv As Boolean
            Public needprec As Boolean
            Public repiterationscount As Integer
            Public repnmv As Integer
            Public repterminationtype As Integer
            Public running As Boolean
            Public tmpd As Double()
            Public tmpx As Double()
            Public rstate As rcommstate
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
                nes = New normestimator.normestimatorstate()
                rx = New Double(-1) {}
                b = New Double(-1) {}
                ui = New Double(-1) {}
                uip1 = New Double(-1) {}
                vi = New Double(-1) {}
                vip1 = New Double(-1) {}
                omegai = New Double(-1) {}
                omegaip1 = New Double(-1) {}
                d = New Double(-1) {}
                x = New Double(-1) {}
                mv = New Double(-1) {}
                mtv = New Double(-1) {}
                tmpd = New Double(-1) {}
                tmpx = New Double(-1) {}
                rstate = New rcommstate()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New linlsqrstate()
                _result.nes = DirectCast(nes.make_copy(), normestimator.normestimatorstate)
                _result.rx = DirectCast(rx.Clone(), Double())
                _result.b = DirectCast(b.Clone(), Double())
                _result.n = n
                _result.m = m
                _result.prectype = prectype
                _result.ui = DirectCast(ui.Clone(), Double())
                _result.uip1 = DirectCast(uip1.Clone(), Double())
                _result.vi = DirectCast(vi.Clone(), Double())
                _result.vip1 = DirectCast(vip1.Clone(), Double())
                _result.omegai = DirectCast(omegai.Clone(), Double())
                _result.omegaip1 = DirectCast(omegaip1.Clone(), Double())
                _result.alphai = alphai
                _result.alphaip1 = alphaip1
                _result.betai = betai
                _result.betaip1 = betaip1
                _result.phibari = phibari
                _result.phibarip1 = phibarip1
                _result.phii = phii
                _result.rhobari = rhobari
                _result.rhobarip1 = rhobarip1
                _result.rhoi = rhoi
                _result.ci = ci
                _result.si = si
                _result.theta = theta
                _result.lambdai = lambdai
                _result.d = DirectCast(d.Clone(), Double())
                _result.anorm = anorm
                _result.bnorm2 = bnorm2
                _result.dnorm = dnorm
                _result.r2 = r2
                _result.x = DirectCast(x.Clone(), Double())
                _result.mv = DirectCast(mv.Clone(), Double())
                _result.mtv = DirectCast(mtv.Clone(), Double())
                _result.epsa = epsa
                _result.epsb = epsb
                _result.epsc = epsc
                _result.maxits = maxits
                _result.xrep = xrep
                _result.xupdated = xupdated
                _result.needmv = needmv
                _result.needmtv = needmtv
                _result.needmv2 = needmv2
                _result.needvmv = needvmv
                _result.needprec = needprec
                _result.repiterationscount = repiterationscount
                _result.repnmv = repnmv
                _result.repterminationtype = repterminationtype
                _result.running = running
                _result.tmpd = DirectCast(tmpd.Clone(), Double())
                _result.tmpx = DirectCast(tmpx.Clone(), Double())
                _result.rstate = DirectCast(rstate.make_copy(), rcommstate)
                Return _result
            End Function
        End Class


        Public Class linlsqrreport
            Inherits apobject
            Public iterationscount As Integer
            Public nmv As Integer
            Public terminationtype As Integer
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New linlsqrreport()
                _result.iterationscount = iterationscount
                _result.nmv = nmv
                _result.terminationtype = terminationtype
                Return _result
            End Function
        End Class




        Public Const atol As Double = 0.000001
        Public Const btol As Double = 0.000001


        '************************************************************************
        '        This function initializes linear LSQR Solver. This solver is used to solve
        '        non-symmetric (and, possibly, non-square) problems. Least squares solution
        '        is returned for non-compatible systems.
        '
        '        USAGE:
        '        1. User initializes algorithm state with LinLSQRCreate() call
        '        2. User tunes solver parameters with  LinLSQRSetCond() and other functions
        '        3. User  calls  LinLSQRSolveSparse()  function which takes algorithm state 
        '           and SparseMatrix object.
        '        4. User calls LinLSQRResults() to get solution
        '        5. Optionally, user may call LinLSQRSolveSparse() again to  solve  another  
        '           problem  with different matrix and/or right part without reinitializing 
        '           LinLSQRState structure.
        '          
        '        INPUT PARAMETERS:
        '            M       -   number of rows in A
        '            N       -   number of variables, N>0
        '
        '        OUTPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '          -- ALGLIB --
        '             Copyright 30.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrcreate(m As Integer, n As Integer, state As linlsqrstate)
            Dim i As Integer = 0

            alglib.ap.assert(m > 0, "LinLSQRCreate: M<=0")
            alglib.ap.assert(n > 0, "LinLSQRCreate: N<=0")
            state.m = m
            state.n = n
            state.prectype = 0
            state.epsa = atol
            state.epsb = btol
            state.epsc = 1 / System.Math.sqrt(Math.machineepsilon)
            state.maxits = 0
            state.lambdai = 0
            state.xrep = False
            state.running = False

            '
            ' * allocate arrays
            ' * set RX to NAN (just for the case user calls Results() without 
            '   calling SolveSparse()
            ' * set B to zero
            '
            normestimator.normestimatorcreate(m, n, 2, 2, state.nes)
            state.rx = New Double(state.n - 1) {}
            state.ui = New Double(state.m + (state.n - 1)) {}
            state.uip1 = New Double(state.m + (state.n - 1)) {}
            state.vip1 = New Double(state.n - 1) {}
            state.vi = New Double(state.n - 1) {}
            state.omegai = New Double(state.n - 1) {}
            state.omegaip1 = New Double(state.n - 1) {}
            state.d = New Double(state.n - 1) {}
            state.x = New Double(state.m + (state.n - 1)) {}
            state.mv = New Double(state.m + (state.n - 1)) {}
            state.mtv = New Double(state.n - 1) {}
            state.b = New Double(state.m - 1) {}
            For i = 0 To n - 1
                state.rx(i) = [Double].NaN
            Next
            For i = 0 To m - 1
                state.b(i) = 0
            Next
            state.rstate.ia = New Integer(1) {}
            state.rstate.ra = New Double(0) {}
            state.rstate.stage = -1
        End Sub


        '************************************************************************
        '        This function sets right part. By default, right part is zero.
        '
        '        INPUT PARAMETERS:
        '            B       -   right part, array[N].
        '
        '        OUTPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '          -- ALGLIB --
        '             Copyright 30.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrsetb(state As linlsqrstate, b As Double())
            Dim i As Integer = 0

            alglib.ap.assert(Not state.running, "LinLSQRSetB: you can not change B when LinLSQRIteration is running")
            alglib.ap.assert(state.m <= alglib.ap.len(b), "LinLSQRSetB: Length(B)<M")
            alglib.ap.assert(apserv.isfinitevector(b, state.m), "LinLSQRSetB: B contains infinite or NaN values")
            state.bnorm2 = 0
            For i = 0 To state.m - 1
                state.b(i) = b(i)
                state.bnorm2 = state.bnorm2 + b(i) * b(i)
            Next
        End Sub


        '************************************************************************
        '        This  function  changes  preconditioning  settings of LinLSQQSolveSparse()
        '        function. By default, SolveSparse() uses diagonal preconditioner,  but  if
        '        you want to use solver without preconditioning, you can call this function
        '        which forces solver to use unit matrix for preconditioning.
        '
        '        INPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '          -- ALGLIB --
        '             Copyright 19.11.2012 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrsetprecunit(state As linlsqrstate)
            alglib.ap.assert(Not state.running, "LinLSQRSetPrecUnit: you can not change preconditioner, because function LinLSQRIteration is running!")
            state.prectype = -1
        End Sub


        '************************************************************************
        '        This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
        '        function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
        '        preconditioner. This preconditioning mode is active by default.
        '
        '        INPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '          -- ALGLIB --
        '             Copyright 19.11.2012 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrsetprecdiag(state As linlsqrstate)
            alglib.ap.assert(Not state.running, "LinLSQRSetPrecDiag: you can not change preconditioner, because function LinCGIteration is running!")
            state.prectype = 0
        End Sub


        '************************************************************************
        '        This function sets optional Tikhonov regularization coefficient.
        '        It is zero by default.
        '
        '        INPUT PARAMETERS:
        '            LambdaI -   regularization factor, LambdaI>=0
        '
        '        OUTPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '            
        '          -- ALGLIB --
        '             Copyright 30.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrsetlambdai(state As linlsqrstate, lambdai As Double)
            alglib.ap.assert(Not state.running, "LinLSQRSetLambdaI: you can not set LambdaI, because function LinLSQRIteration is running")
            alglib.ap.assert(Math.isfinite(lambdai) AndAlso CDbl(lambdai) >= CDbl(0), "LinLSQRSetLambdaI: LambdaI is infinite or NaN")
            state.lambdai = lambdai
        End Sub


        '************************************************************************
        '
        '          -- ALGLIB --
        '             Copyright 30.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function linlsqriteration(state As linlsqrstate) As Boolean
            Dim result As New Boolean()
            Dim summn As Integer = 0
            Dim bnorm As Double = 0
            Dim i As Integer = 0
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
                summn = state.rstate.ia(0)
                i = state.rstate.ia(1)
                bnorm = state.rstate.ra(0)
            Else
                summn = -983
                i = -989
                bnorm = -834
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
            If state.rstate.stage = 4 Then
                GoTo lbl_4
            End If
            If state.rstate.stage = 5 Then
                GoTo lbl_5
            End If
            If state.rstate.stage = 6 Then
                GoTo lbl_6
            End If

            '
            ' Routine body
            '
            alglib.ap.assert(alglib.ap.len(state.b) > 0, "LinLSQRIteration: using non-allocated array B")
            bnorm = System.Math.sqrt(state.bnorm2)
            state.running = True
            state.repnmv = 0
            clearrfields(state)
            state.repiterationscount = 0
            summn = state.m + state.n
            state.r2 = state.bnorm2

            '
            'estimate for ANorm
            '
            normestimator.normestimatorrestart(state.nes)
lbl_7:
            If Not normestimator.normestimatoriteration(state.nes) Then
                GoTo lbl_8
            End If
            If Not state.nes.needmv Then
                GoTo lbl_9
            End If
            For i_ = 0 To state.n - 1
                state.x(i_) = state.nes.x(i_)
            Next
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needmv = True
            state.rstate.stage = 0
            GoTo lbl_rcomm
lbl_0:
            state.needmv = False
            For i_ = 0 To state.m - 1
                state.nes.mv(i_) = state.mv(i_)
            Next
            GoTo lbl_7
lbl_9:
            If Not state.nes.needmtv Then
                GoTo lbl_11
            End If
            For i_ = 0 To state.m - 1
                state.x(i_) = state.nes.x(i_)
            Next

            '
            'matrix-vector multiplication
            '
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needmtv = True
            state.rstate.stage = 1
            GoTo lbl_rcomm
lbl_1:
            state.needmtv = False
            For i_ = 0 To state.n - 1
                state.nes.mtv(i_) = state.mtv(i_)
            Next
            GoTo lbl_7
lbl_11:
            GoTo lbl_7
lbl_8:
            normestimator.normestimatorresults(state.nes, state.anorm)

            '
            'initialize .RX by zeros
            '
            For i = 0 To state.n - 1
                state.rx(i) = 0
            Next

            '
            'output first report
            '
            If Not state.xrep Then
                GoTo lbl_13
            End If
            For i_ = 0 To state.n - 1
                state.x(i_) = state.rx(i_)
            Next
            clearrfields(state)
            state.xupdated = True
            state.rstate.stage = 2
            GoTo lbl_rcomm
lbl_2:
            state.xupdated = False
lbl_13:

            '
            ' LSQR, Step 0.
            '
            ' Algorithm outline corresponds to one which was described at p.50 of
            ' "LSQR - an algorithm for sparse linear equations and sparse least 
            ' squares" by C.Paige and M.Saunders with one small addition - we
            ' explicitly extend system matrix by additional N lines in order 
            ' to handle non-zero lambda, i.e. original A is replaced by
            '         [ A        ]
            ' A_mod = [          ]
            '         [ lambda*I ].
            '
            ' Step 0:
            '     x[0]          = 0
            '     beta[1]*u[1]  = b
            '     alpha[1]*v[1] = A_mod'*u[1]
            '     w[1]          = v[1]
            '     phiBar[1]     = beta[1]
            '     rhoBar[1]     = alpha[1]
            '     d[0]          = 0
            '
            ' NOTE:
            ' There are three criteria for stopping:
            ' (S0) maximum number of iterations
            ' (S1) ||Rk||<=EpsB*||B||;
            ' (S2) ||A^T*Rk||/(||A||*||Rk||)<=EpsA.
            ' It is very important that S2 always checked AFTER S1. It is necessary
            ' to avoid division by zero when Rk=0.
            '
            state.betai = bnorm
            If CDbl(state.betai) = CDbl(0) Then

                '
                ' Zero right part
                '
                state.running = False
                state.repterminationtype = 1
                result = False
                Return result
            End If
            For i = 0 To summn - 1
                If i < state.m Then
                    state.ui(i) = state.b(i) / state.betai
                Else
                    state.ui(i) = 0
                End If
                state.x(i) = state.ui(i)
            Next
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needmtv = True
            state.rstate.stage = 3
            GoTo lbl_rcomm
lbl_3:
            state.needmtv = False
            For i = 0 To state.n - 1
                state.mtv(i) = state.mtv(i) + state.lambdai * state.ui(state.m + i)
            Next
            state.alphai = 0
            For i = 0 To state.n - 1
                state.alphai = state.alphai + state.mtv(i) * state.mtv(i)
            Next
            state.alphai = System.Math.sqrt(state.alphai)
            If CDbl(state.alphai) = CDbl(0) Then

                '
                ' Orthogonality stopping criterion is met
                '
                state.running = False
                state.repterminationtype = 4
                result = False
                Return result
            End If
            For i = 0 To state.n - 1
                state.vi(i) = state.mtv(i) / state.alphai
                state.omegai(i) = state.vi(i)
            Next
            state.phibari = state.betai
            state.rhobari = state.alphai
            For i = 0 To state.n - 1
                state.d(i) = 0
            Next
            state.dnorm = 0
lbl_15:

            '
            ' Steps I=1, 2, ...
            '
            If False Then
                GoTo lbl_16
            End If

            '
            ' At I-th step State.RepIterationsCount=I.
            '
            state.repiterationscount = state.repiterationscount + 1

            '
            ' Bidiagonalization part:
            '     beta[i+1]*u[i+1]  = A_mod*v[i]-alpha[i]*u[i]
            '     alpha[i+1]*v[i+1] = A_mod'*u[i+1] - beta[i+1]*v[i]
            '     
            ' NOTE:  beta[i+1]=0 or alpha[i+1]=0 will lead to successful termination
            '        in the end of the current iteration. In this case u/v are zero.
            ' NOTE2: algorithm won't fail on zero alpha or beta (there will be no
            '        division by zero because it will be stopped BEFORE division
            '        occurs). However, near-zero alpha and beta won't stop algorithm
            '        and, although no division by zero will happen, orthogonality 
            '        in U and V will be lost.
            '
            For i_ = 0 To state.n - 1
                state.x(i_) = state.vi(i_)
            Next
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needmv = True
            state.rstate.stage = 4
            GoTo lbl_rcomm
lbl_4:
            state.needmv = False
            For i = 0 To state.n - 1
                state.mv(state.m + i) = state.lambdai * state.vi(i)
            Next
            state.betaip1 = 0
            For i = 0 To summn - 1
                state.uip1(i) = state.mv(i) - state.alphai * state.ui(i)
                state.betaip1 = state.betaip1 + state.uip1(i) * state.uip1(i)
            Next
            If CDbl(state.betaip1) <> CDbl(0) Then
                state.betaip1 = System.Math.sqrt(state.betaip1)
                For i = 0 To summn - 1
                    state.uip1(i) = state.uip1(i) / state.betaip1
                Next
            End If
            For i_ = 0 To state.m - 1
                state.x(i_) = state.uip1(i_)
            Next
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needmtv = True
            state.rstate.stage = 5
            GoTo lbl_rcomm
lbl_5:
            state.needmtv = False
            For i = 0 To state.n - 1
                state.mtv(i) = state.mtv(i) + state.lambdai * state.uip1(state.m + i)
            Next
            state.alphaip1 = 0
            For i = 0 To state.n - 1
                state.vip1(i) = state.mtv(i) - state.betaip1 * state.vi(i)
                state.alphaip1 = state.alphaip1 + state.vip1(i) * state.vip1(i)
            Next
            If CDbl(state.alphaip1) <> CDbl(0) Then
                state.alphaip1 = System.Math.sqrt(state.alphaip1)
                For i = 0 To state.n - 1
                    state.vip1(i) = state.vip1(i) / state.alphaip1
                Next
            End If

            '
            ' Build next orthogonal transformation
            '
            state.rhoi = apserv.safepythag2(state.rhobari, state.betaip1)
            state.ci = state.rhobari / state.rhoi
            state.si = state.betaip1 / state.rhoi
            state.theta = state.si * state.alphaip1
            state.rhobarip1 = -(state.ci * state.alphaip1)
            state.phii = state.ci * state.phibari
            state.phibarip1 = state.si * state.phibari

            '
            ' Update .RNorm
            '
            ' This tricky  formula  is  necessary  because  simply  writing
            ' State.R2:=State.PhiBarIP1*State.PhiBarIP1 does NOT guarantees
            ' monotonic decrease of R2. Roundoff error combined with 80-bit
            ' precision used internally by Intel chips allows R2 to increase
            ' slightly in some rare, but possible cases. This property is
            ' undesirable, so we prefer to guard against R increase.
            '
            state.r2 = System.Math.Min(state.r2, state.phibarip1 * state.phibarip1)

            '
            ' Update d and DNorm, check condition-related stopping criteria
            '
            For i = 0 To state.n - 1
                state.d(i) = 1 / state.rhoi * (state.vi(i) - state.theta * state.d(i))
                state.dnorm = state.dnorm + state.d(i) * state.d(i)
            Next
            If CDbl(System.Math.sqrt(state.dnorm) * state.anorm) >= CDbl(state.epsc) Then
                state.running = False
                state.repterminationtype = 7
                result = False
                Return result
            End If

            '
            ' Update x, output report
            '
            For i = 0 To state.n - 1
                state.rx(i) = state.rx(i) + state.phii / state.rhoi * state.omegai(i)
            Next
            If Not state.xrep Then
                GoTo lbl_17
            End If
            For i_ = 0 To state.n - 1
                state.x(i_) = state.rx(i_)
            Next
            clearrfields(state)
            state.xupdated = True
            state.rstate.stage = 6
            GoTo lbl_rcomm
lbl_6:
            state.xupdated = False
lbl_17:

            '
            ' Check stopping criteria
            ' 1. achieved required number of iterations;
            ' 2. ||Rk||<=EpsB*||B||;
            ' 3. ||A^T*Rk||/(||A||*||Rk||)<=EpsA;
            '
            If state.maxits > 0 AndAlso state.repiterationscount >= state.maxits Then

                '
                ' Achieved required number of iterations
                '
                state.running = False
                state.repterminationtype = 5
                result = False
                Return result
            End If
            If CDbl(state.phibarip1) <= CDbl(state.epsb * bnorm) Then

                '
                ' ||Rk||<=EpsB*||B||, here ||Rk||=PhiBar
                '
                state.running = False
                state.repterminationtype = 1
                result = False
                Return result
            End If
            If CDbl(state.alphaip1 * System.Math.Abs(state.ci) / state.anorm) <= CDbl(state.epsa) Then

                '
                ' ||A^T*Rk||/(||A||*||Rk||)<=EpsA, here ||A^T*Rk||=PhiBar*Alpha[i+1]*|.C|
                '
                state.running = False
                state.repterminationtype = 4
                result = False
                Return result
            End If

            '
            ' Update omega
            '
            For i = 0 To state.n - 1
                state.omegaip1(i) = state.vip1(i) - state.theta / state.rhoi * state.omegai(i)
            Next

            '
            ' Prepare for the next iteration - rename variables:
            ' u[i]   := u[i+1]
            ' v[i]   := v[i+1]
            ' rho[i] := rho[i+1]
            ' ...
            '
            For i_ = 0 To summn - 1
                state.ui(i_) = state.uip1(i_)
            Next
            For i_ = 0 To state.n - 1
                state.vi(i_) = state.vip1(i_)
            Next
            For i_ = 0 To state.n - 1
                state.omegai(i_) = state.omegaip1(i_)
            Next
            state.alphai = state.alphaip1
            state.betai = state.betaip1
            state.phibari = state.phibarip1
            state.rhobari = state.rhobarip1
            GoTo lbl_15
lbl_16:
            result = False
            Return result
lbl_rcomm:

            '
            ' Saving state
            '
            result = True
            state.rstate.ia(0) = summn
            state.rstate.ia(1) = i
            state.rstate.ra(0) = bnorm
            Return result
        End Function


        '************************************************************************
        '        Procedure for solution of A*x=b with sparse A.
        '
        '        INPUT PARAMETERS:
        '            State   -   algorithm state
        '            A       -   sparse M*N matrix in the CRS format (you MUST contvert  it 
        '                        to CRS format  by  calling  SparseConvertToCRS()  function
        '                        BEFORE you pass it to this function).
        '            B       -   right part, array[M]
        '
        '        RESULT:
        '            This function returns no result.
        '            You can get solution by calling LinCGResults()
        '            
        '        NOTE: this function uses lightweight preconditioning -  multiplication  by
        '              inverse of diag(A). If you want, you can turn preconditioning off by
        '              calling LinLSQRSetPrecUnit(). However, preconditioning cost is   low
        '              and preconditioner is very important for solution  of  badly  scaled
        '              problems.
        '
        '          -- ALGLIB --
        '             Copyright 30.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrsolvesparse(state As linlsqrstate, a As sparse.sparsematrix, b As Double())
            Dim n As Integer = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim t0 As Integer = 0
            Dim t1 As Integer = 0
            Dim v As Double = 0

            n = state.n
            alglib.ap.assert(Not state.running, "LinLSQRSolveSparse: you can not call this function when LinLSQRIteration is running")
            alglib.ap.assert(alglib.ap.len(b) >= state.m, "LinLSQRSolveSparse: Length(B)<M")
            alglib.ap.assert(apserv.isfinitevector(b, state.m), "LinLSQRSolveSparse: B contains infinite or NaN values")

            '
            ' Allocate temporaries
            '
            apserv.rvectorsetlengthatleast(state.tmpd, n)
            apserv.rvectorsetlengthatleast(state.tmpx, n)

            '
            ' Compute diagonal scaling matrix D
            '
            If state.prectype = 0 Then

                '
                ' Default preconditioner - inverse of column norms
                '
                For i = 0 To n - 1
                    state.tmpd(i) = 0
                Next
                t0 = 0
                t1 = 0
                While sparse.sparseenumerate(a, t0, t1, i, j, v)
                    state.tmpd(j) = state.tmpd(j) + Math.sqr(v)
                End While
                For i = 0 To n - 1
                    If CDbl(state.tmpd(i)) > CDbl(0) Then
                        state.tmpd(i) = 1 / System.Math.sqrt(state.tmpd(i))
                    Else
                        state.tmpd(i) = 1
                    End If
                Next
            Else

                '
                ' No diagonal scaling
                '
                For i = 0 To n - 1
                    state.tmpd(i) = 1
                Next
            End If

            '
            ' Solve.
            '
            ' Instead of solving A*x=b we solve preconditioned system (A*D)*(inv(D)*x)=b.
            ' Transformed A is not calculated explicitly, we just modify multiplication
            ' by A or A'. After solution we modify State.RX so it will store untransformed
            ' variables
            '
            linlsqrsetb(state, b)
            linlsqrrestart(state)
            While linlsqriteration(state)
                If state.needmv Then
                    For i = 0 To n - 1
                        state.tmpx(i) = state.tmpd(i) * state.x(i)
                    Next
                    sparse.sparsemv(a, state.tmpx, state.mv)
                End If
                If state.needmtv Then
                    sparse.sparsemtv(a, state.x, state.mtv)
                    For i = 0 To n - 1
                        state.mtv(i) = state.tmpd(i) * state.mtv(i)
                    Next
                End If
            End While
            For i = 0 To n - 1
                state.rx(i) = state.tmpd(i) * state.rx(i)
            Next
        End Sub


        '************************************************************************
        '        This function sets stopping criteria.
        '
        '        INPUT PARAMETERS:
        '            EpsA    -   algorithm will be stopped if ||A^T*Rk||/(||A||*||Rk||)<=EpsA.
        '            EpsB    -   algorithm will be stopped if ||Rk||<=EpsB*||B||
        '            MaxIts  -   algorithm will be stopped if number of iterations
        '                        more than MaxIts.
        '
        '        OUTPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '        NOTE: if EpsA,EpsB,EpsC and MaxIts are zero then these variables will
        '        be setted as default values.
        '            
        '          -- ALGLIB --
        '             Copyright 30.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrsetcond(state As linlsqrstate, epsa As Double, epsb As Double, maxits As Integer)
            alglib.ap.assert(Not state.running, "LinLSQRSetCond: you can not call this function when LinLSQRIteration is running")
            alglib.ap.assert(Math.isfinite(epsa) AndAlso CDbl(epsa) >= CDbl(0), "LinLSQRSetCond: EpsA is negative, INF or NAN")
            alglib.ap.assert(Math.isfinite(epsb) AndAlso CDbl(epsb) >= CDbl(0), "LinLSQRSetCond: EpsB is negative, INF or NAN")
            alglib.ap.assert(maxits >= 0, "LinLSQRSetCond: MaxIts is negative")
            If (CDbl(epsa) = CDbl(0) AndAlso CDbl(epsb) = CDbl(0)) AndAlso maxits = 0 Then
                state.epsa = atol
                state.epsb = btol
                state.maxits = state.n
            Else
                state.epsa = epsa
                state.epsb = epsb
                state.maxits = maxits
            End If
        End Sub


        '************************************************************************
        '        LSQR solver: results.
        '
        '        This function must be called after LinLSQRSolve
        '
        '        INPUT PARAMETERS:
        '            State   -   algorithm state
        '
        '        OUTPUT PARAMETERS:
        '            X       -   array[N], solution
        '            Rep     -   optimization report:
        '                        * Rep.TerminationType completetion code:
        '                            *  1    ||Rk||<=EpsB*||B||
        '                            *  4    ||A^T*Rk||/(||A||*||Rk||)<=EpsA
        '                            *  5    MaxIts steps was taken
        '                            *  7    rounding errors prevent further progress,
        '                                    X contains best point found so far.
        '                                    (sometimes returned on singular systems)
        '                        * Rep.IterationsCount contains iterations count
        '                        * NMV countains number of matrix-vector calculations
        '                        
        '          -- ALGLIB --
        '             Copyright 30.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrresults(state As linlsqrstate, ByRef x As Double(), rep As linlsqrreport)
            Dim i_ As Integer = 0

            x = New Double(-1) {}

            alglib.ap.assert(Not state.running, "LinLSQRResult: you can not call this function when LinLSQRIteration is running")
            If alglib.ap.len(x) < state.n Then
                x = New Double(state.n - 1) {}
            End If
            For i_ = 0 To state.n - 1
                x(i_) = state.rx(i_)
            Next
            rep.iterationscount = state.repiterationscount
            rep.nmv = state.repnmv
            rep.terminationtype = state.repterminationtype
        End Sub


        '************************************************************************
        '        This function turns on/off reporting.
        '
        '        INPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '            NeedXRep-   whether iteration reports are needed or not
        '
        '        If NeedXRep is True, algorithm will call rep() callback function if  it is
        '        provided to MinCGOptimize().
        '
        '          -- ALGLIB --
        '             Copyright 30.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrsetxrep(state As linlsqrstate, needxrep As Boolean)
            state.xrep = needxrep
        End Sub


        '************************************************************************
        '        This function restarts LinLSQRIteration
        '
        '          -- ALGLIB --
        '             Copyright 30.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub linlsqrrestart(state As linlsqrstate)
            state.rstate.ia = New Integer(1) {}
            state.rstate.ra = New Double(0) {}
            state.rstate.stage = -1
            clearrfields(state)
        End Sub


        '************************************************************************
        '        Clears request fileds (to be sure that we don't forgot to clear something)
        '        ************************************************************************

        Private Shared Sub clearrfields(state As linlsqrstate)
            state.xupdated = False
            state.needmv = False
            state.needmtv = False
            state.needmv2 = False
            state.needvmv = False
            state.needprec = False
        End Sub


    End Class
    Public Class lincg
        '************************************************************************
        '        This object stores state of the linear CG method.
        '
        '        You should use ALGLIB functions to work with this object.
        '        Never try to access its fields directly!
        '        ************************************************************************

        Public Class lincgstate
            Inherits apobject
            Public rx As Double()
            Public b As Double()
            Public n As Integer
            Public prectype As Integer
            Public cx As Double()
            Public cr As Double()
            Public cz As Double()
            Public p As Double()
            Public r As Double()
            Public z As Double()
            Public alpha As Double
            Public beta As Double
            Public r2 As Double
            Public meritfunction As Double
            Public x As Double()
            Public mv As Double()
            Public pv As Double()
            Public vmv As Double
            Public startx As Double()
            Public epsf As Double
            Public maxits As Integer
            Public itsbeforerestart As Integer
            Public itsbeforerupdate As Integer
            Public xrep As Boolean
            Public xupdated As Boolean
            Public needmv As Boolean
            Public needmtv As Boolean
            Public needmv2 As Boolean
            Public needvmv As Boolean
            Public needprec As Boolean
            Public repiterationscount As Integer
            Public repnmv As Integer
            Public repterminationtype As Integer
            Public running As Boolean
            Public tmpd As Double()
            Public rstate As rcommstate
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
                rx = New Double(-1) {}
                b = New Double(-1) {}
                cx = New Double(-1) {}
                cr = New Double(-1) {}
                cz = New Double(-1) {}
                p = New Double(-1) {}
                r = New Double(-1) {}
                z = New Double(-1) {}
                x = New Double(-1) {}
                mv = New Double(-1) {}
                pv = New Double(-1) {}
                startx = New Double(-1) {}
                tmpd = New Double(-1) {}
                rstate = New rcommstate()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New lincgstate()
                _result.rx = DirectCast(rx.Clone(), Double())
                _result.b = DirectCast(b.Clone(), Double())
                _result.n = n
                _result.prectype = prectype
                _result.cx = DirectCast(cx.Clone(), Double())
                _result.cr = DirectCast(cr.Clone(), Double())
                _result.cz = DirectCast(cz.Clone(), Double())
                _result.p = DirectCast(p.Clone(), Double())
                _result.r = DirectCast(r.Clone(), Double())
                _result.z = DirectCast(z.Clone(), Double())
                _result.alpha = alpha
                _result.beta = beta
                _result.r2 = r2
                _result.meritfunction = meritfunction
                _result.x = DirectCast(x.Clone(), Double())
                _result.mv = DirectCast(mv.Clone(), Double())
                _result.pv = DirectCast(pv.Clone(), Double())
                _result.vmv = vmv
                _result.startx = DirectCast(startx.Clone(), Double())
                _result.epsf = epsf
                _result.maxits = maxits
                _result.itsbeforerestart = itsbeforerestart
                _result.itsbeforerupdate = itsbeforerupdate
                _result.xrep = xrep
                _result.xupdated = xupdated
                _result.needmv = needmv
                _result.needmtv = needmtv
                _result.needmv2 = needmv2
                _result.needvmv = needvmv
                _result.needprec = needprec
                _result.repiterationscount = repiterationscount
                _result.repnmv = repnmv
                _result.repterminationtype = repterminationtype
                _result.running = running
                _result.tmpd = DirectCast(tmpd.Clone(), Double())
                _result.rstate = DirectCast(rstate.make_copy(), rcommstate)
                Return _result
            End Function
        End Class


        Public Class lincgreport
            Inherits apobject
            Public iterationscount As Integer
            Public nmv As Integer
            Public terminationtype As Integer
            Public r2 As Double
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New lincgreport()
                _result.iterationscount = iterationscount
                _result.nmv = nmv
                _result.terminationtype = terminationtype
                _result.r2 = r2
                Return _result
            End Function
        End Class




        Public Const defaultprecision As Double = 0.000001


        '************************************************************************
        '        This function initializes linear CG Solver. This solver is used  to  solve
        '        symmetric positive definite problems. If you want  to  solve  nonsymmetric
        '        (or non-positive definite) problem you may use LinLSQR solver provided  by
        '        ALGLIB.
        '
        '        USAGE:
        '        1. User initializes algorithm state with LinCGCreate() call
        '        2. User tunes solver parameters with  LinCGSetCond() and other functions
        '        3. Optionally, user sets starting point with LinCGSetStartingPoint()
        '        4. User  calls LinCGSolveSparse() function which takes algorithm state and
        '           SparseMatrix object.
        '        5. User calls LinCGResults() to get solution
        '        6. Optionally, user may call LinCGSolveSparse()  again  to  solve  another
        '           problem  with different matrix and/or right part without reinitializing
        '           LinCGState structure.
        '          
        '        INPUT PARAMETERS:
        '            N       -   problem dimension, N>0
        '
        '        OUTPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgcreate(n As Integer, state As lincgstate)
            Dim i As Integer = 0

            alglib.ap.assert(n > 0, "LinCGCreate: N<=0")
            state.n = n
            state.prectype = 0
            state.itsbeforerestart = n
            state.itsbeforerupdate = 10
            state.epsf = defaultprecision
            state.maxits = 0
            state.xrep = False
            state.running = False

            '
            ' * allocate arrays
            ' * set RX to NAN (just for the case user calls Results() without 
            '   calling SolveSparse()
            ' * set starting point to zero
            ' * we do NOT initialize B here because we assume that user should
            '   initializate it using LinCGSetB() function. In case he forgets
            '   to do so, exception will be thrown in the LinCGIteration().
            '
            state.rx = New Double(state.n - 1) {}
            state.startx = New Double(state.n - 1) {}
            state.b = New Double(state.n - 1) {}
            For i = 0 To state.n - 1
                state.rx(i) = [Double].NaN
                state.startx(i) = 0.0
                state.b(i) = 0
            Next
            state.cx = New Double(state.n - 1) {}
            state.p = New Double(state.n - 1) {}
            state.r = New Double(state.n - 1) {}
            state.cr = New Double(state.n - 1) {}
            state.z = New Double(state.n - 1) {}
            state.cz = New Double(state.n - 1) {}
            state.x = New Double(state.n - 1) {}
            state.mv = New Double(state.n - 1) {}
            state.pv = New Double(state.n - 1) {}
            updateitersdata(state)
            state.rstate.ia = New Integer(0) {}
            state.rstate.ra = New Double(2) {}
            state.rstate.stage = -1
        End Sub


        '************************************************************************
        '        This function sets starting point.
        '        By default, zero starting point is used.
        '
        '        INPUT PARAMETERS:
        '            X       -   starting point, array[N]
        '
        '        OUTPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgsetstartingpoint(state As lincgstate, x As Double())
            Dim i_ As Integer = 0

            alglib.ap.assert(Not state.running, "LinCGSetStartingPoint: you can not change starting point because LinCGIteration() function is running")
            alglib.ap.assert(state.n <= alglib.ap.len(x), "LinCGSetStartingPoint: Length(X)<N")
            alglib.ap.assert(apserv.isfinitevector(x, state.n), "LinCGSetStartingPoint: X contains infinite or NaN values!")
            For i_ = 0 To state.n - 1
                state.startx(i_) = x(i_)
            Next
        End Sub


        '************************************************************************
        '        This function sets right part. By default, right part is zero.
        '
        '        INPUT PARAMETERS:
        '            B       -   right part, array[N].
        '
        '        OUTPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgsetb(state As lincgstate, b As Double())
            Dim i_ As Integer = 0

            alglib.ap.assert(Not state.running, "LinCGSetB: you can not set B, because function LinCGIteration is running!")
            alglib.ap.assert(alglib.ap.len(b) >= state.n, "LinCGSetB: Length(B)<N")
            alglib.ap.assert(apserv.isfinitevector(b, state.n), "LinCGSetB: B contains infinite or NaN values!")
            For i_ = 0 To state.n - 1
                state.b(i_) = b(i_)
            Next
        End Sub


        '************************************************************************
        '        This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
        '        function. By default, SolveSparse() uses diagonal preconditioner,  but  if
        '        you want to use solver without preconditioning, you can call this function
        '        which forces solver to use unit matrix for preconditioning.
        '
        '        INPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '          -- ALGLIB --
        '             Copyright 19.11.2012 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgsetprecunit(state As lincgstate)
            alglib.ap.assert(Not state.running, "LinCGSetPrecUnit: you can not change preconditioner, because function LinCGIteration is running!")
            state.prectype = -1
        End Sub


        '************************************************************************
        '        This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
        '        function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
        '        preconditioner. This preconditioning mode is active by default.
        '
        '        INPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '          -- ALGLIB --
        '             Copyright 19.11.2012 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgsetprecdiag(state As lincgstate)
            alglib.ap.assert(Not state.running, "LinCGSetPrecDiag: you can not change preconditioner, because function LinCGIteration is running!")
            state.prectype = 0
        End Sub


        '************************************************************************
        '        This function sets stopping criteria.
        '
        '        INPUT PARAMETERS:
        '            EpsF    -   algorithm will be stopped if norm of residual is less than 
        '                        EpsF*||b||.
        '            MaxIts  -   algorithm will be stopped if number of iterations is  more 
        '                        than MaxIts.
        '
        '        OUTPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '        NOTES:
        '        If  both  EpsF  and  MaxIts  are  zero then small EpsF will be set to small 
        '        value.
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgsetcond(state As lincgstate, epsf As Double, maxits As Integer)
            alglib.ap.assert(Not state.running, "LinCGSetCond: you can not change stopping criteria when LinCGIteration() is running")
            alglib.ap.assert(Math.isfinite(epsf) AndAlso CDbl(epsf) >= CDbl(0), "LinCGSetCond: EpsF is negative or contains infinite or NaN values")
            alglib.ap.assert(maxits >= 0, "LinCGSetCond: MaxIts is negative")
            If CDbl(epsf) = CDbl(0) AndAlso maxits = 0 Then
                state.epsf = defaultprecision
                state.maxits = maxits
            Else
                state.epsf = epsf
                state.maxits = maxits
            End If
        End Sub


        '************************************************************************
        '        Reverse communication version of linear CG.
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function lincgiteration(state As lincgstate) As Boolean
            Dim result As New Boolean()
            Dim i As Integer = 0
            Dim uvar As Double = 0
            Dim bnorm As Double = 0
            Dim v As Double = 0
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
                i = state.rstate.ia(0)
                uvar = state.rstate.ra(0)
                bnorm = state.rstate.ra(1)
                v = state.rstate.ra(2)
            Else
                i = -983
                uvar = -989
                bnorm = -834
                v = 900
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
            If state.rstate.stage = 4 Then
                GoTo lbl_4
            End If
            If state.rstate.stage = 5 Then
                GoTo lbl_5
            End If
            If state.rstate.stage = 6 Then
                GoTo lbl_6
            End If
            If state.rstate.stage = 7 Then
                GoTo lbl_7
            End If

            '
            ' Routine body
            '
            alglib.ap.assert(alglib.ap.len(state.b) > 0, "LinCGIteration: B is not initialized (you must initialize B by LinCGSetB() call")
            state.running = True
            state.repnmv = 0
            clearrfields(state)
            updateitersdata(state)

            '
            ' Start 0-th iteration
            '
            For i_ = 0 To state.n - 1
                state.rx(i_) = state.startx(i_)
            Next
            For i_ = 0 To state.n - 1
                state.x(i_) = state.rx(i_)
            Next
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needvmv = True
            state.rstate.stage = 0
            GoTo lbl_rcomm
lbl_0:
            state.needvmv = False
            bnorm = 0
            state.r2 = 0
            state.meritfunction = 0
            For i = 0 To state.n - 1
                state.r(i) = state.b(i) - state.mv(i)
                state.r2 = state.r2 + state.r(i) * state.r(i)
                state.meritfunction = state.meritfunction + state.mv(i) * state.rx(i) - 2 * state.b(i) * state.rx(i)
                bnorm = bnorm + state.b(i) * state.b(i)
            Next
            bnorm = System.Math.sqrt(bnorm)

            '
            ' Output first report
            '
            If Not state.xrep Then
                GoTo lbl_8
            End If
            For i_ = 0 To state.n - 1
                state.x(i_) = state.rx(i_)
            Next
            clearrfields(state)
            state.xupdated = True
            state.rstate.stage = 1
            GoTo lbl_rcomm
lbl_1:
            state.xupdated = False
lbl_8:

            '
            ' Is x0 a solution?
            '
            If Not Math.isfinite(state.r2) OrElse CDbl(System.Math.sqrt(state.r2)) <= CDbl(state.epsf * bnorm) Then
                state.running = False
                If Math.isfinite(state.r2) Then
                    state.repterminationtype = 1
                Else
                    state.repterminationtype = -4
                End If
                result = False
                Return result
            End If

            '
            ' Calculate Z and P
            '
            For i_ = 0 To state.n - 1
                state.x(i_) = state.r(i_)
            Next
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needprec = True
            state.rstate.stage = 2
            GoTo lbl_rcomm
lbl_2:
            state.needprec = False
            For i = 0 To state.n - 1
                state.z(i) = state.pv(i)
                state.p(i) = state.z(i)
            Next

            '
            ' Other iterations(1..N)
            '
            state.repiterationscount = 0
lbl_10:
            If False Then
                GoTo lbl_11
            End If
            state.repiterationscount = state.repiterationscount + 1

            '
            ' Calculate Alpha
            '
            For i_ = 0 To state.n - 1
                state.x(i_) = state.p(i_)
            Next
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needvmv = True
            state.rstate.stage = 3
            GoTo lbl_rcomm
lbl_3:
            state.needvmv = False
            If Not Math.isfinite(state.vmv) OrElse CDbl(state.vmv) <= CDbl(0) Then

                '
                ' a) Overflow when calculating VMV
                ' b) non-positive VMV (non-SPD matrix)
                '
                state.running = False
                If Math.isfinite(state.vmv) Then
                    state.repterminationtype = -5
                Else
                    state.repterminationtype = -4
                End If
                result = False
                Return result
            End If
            state.alpha = 0
            For i = 0 To state.n - 1
                state.alpha = state.alpha + state.r(i) * state.z(i)
            Next
            state.alpha = state.alpha / state.vmv
            If Not Math.isfinite(state.alpha) Then

                '
                ' Overflow when calculating Alpha
                '
                state.running = False
                state.repterminationtype = -4
                result = False
                Return result
            End If

            '
            ' Next step toward solution
            '
            For i = 0 To state.n - 1
                state.cx(i) = state.rx(i) + state.alpha * state.p(i)
            Next

            '
            ' Calculate R:
            ' * use recurrent relation to update R
            ' * at every ItsBeforeRUpdate-th iteration recalculate it from scratch, using matrix-vector product
            '   in case R grows instead of decreasing, algorithm is terminated with positive completion code
            '
            If Not (state.itsbeforerupdate = 0 OrElse state.repiterationscount Mod state.itsbeforerupdate <> 0) Then
                GoTo lbl_12
            End If

            '
            ' Calculate R using recurrent formula
            '
            For i = 0 To state.n - 1
                state.cr(i) = state.r(i) - state.alpha * state.mv(i)
                state.x(i) = state.cr(i)
            Next
            GoTo lbl_13
lbl_12:

            '
            ' Calculate R using matrix-vector multiplication
            '
            For i_ = 0 To state.n - 1
                state.x(i_) = state.cx(i_)
            Next
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needmv = True
            state.rstate.stage = 4
            GoTo lbl_rcomm
lbl_4:
            state.needmv = False
            For i = 0 To state.n - 1
                state.cr(i) = state.b(i) - state.mv(i)
                state.x(i) = state.cr(i)
            Next

            '
            ' Calculating merit function
            ' Check emergency stopping criterion
            '
            v = 0
            For i = 0 To state.n - 1
                v = v + state.mv(i) * state.cx(i) - 2 * state.b(i) * state.cx(i)
            Next
            If CDbl(v) < CDbl(state.meritfunction) Then
                GoTo lbl_14
            End If
            For i = 0 To state.n - 1
                If Not Math.isfinite(state.rx(i)) Then
                    state.running = False
                    state.repterminationtype = -4
                    result = False
                    Return result
                End If
            Next

            '
            'output last report
            '
            If Not state.xrep Then
                GoTo lbl_16
            End If
            For i_ = 0 To state.n - 1
                state.x(i_) = state.rx(i_)
            Next
            clearrfields(state)
            state.xupdated = True
            state.rstate.stage = 5
            GoTo lbl_rcomm
lbl_5:
            state.xupdated = False
lbl_16:
            state.running = False
            state.repterminationtype = 7
            result = False
            Return result
lbl_14:
            state.meritfunction = v
lbl_13:
            For i_ = 0 To state.n - 1
                state.rx(i_) = state.cx(i_)
            Next

            '
            ' calculating RNorm
            '
            ' NOTE: monotonic decrease of R2 is not guaranteed by algorithm.
            '
            state.r2 = 0
            For i = 0 To state.n - 1
                state.r2 = state.r2 + state.cr(i) * state.cr(i)
            Next

            '
            'output report
            '
            If Not state.xrep Then
                GoTo lbl_18
            End If
            For i_ = 0 To state.n - 1
                state.x(i_) = state.rx(i_)
            Next
            clearrfields(state)
            state.xupdated = True
            state.rstate.stage = 6
            GoTo lbl_rcomm
lbl_6:
            state.xupdated = False
lbl_18:

            '
            'stopping criterion
            'achieved the required precision
            '
            If Not Math.isfinite(state.r2) OrElse CDbl(System.Math.sqrt(state.r2)) <= CDbl(state.epsf * bnorm) Then
                state.running = False
                If Math.isfinite(state.r2) Then
                    state.repterminationtype = 1
                Else
                    state.repterminationtype = -4
                End If
                result = False
                Return result
            End If
            If state.repiterationscount >= state.maxits AndAlso state.maxits > 0 Then
                For i = 0 To state.n - 1
                    If Not Math.isfinite(state.rx(i)) Then
                        state.running = False
                        state.repterminationtype = -4
                        result = False
                        Return result
                    End If
                Next

                '
                'if X is finite number
                '
                state.running = False
                state.repterminationtype = 5
                result = False
                Return result
            End If
            For i_ = 0 To state.n - 1
                state.x(i_) = state.cr(i_)
            Next

            '
            'prepere of parameters for next iteration
            '
            state.repnmv = state.repnmv + 1
            clearrfields(state)
            state.needprec = True
            state.rstate.stage = 7
            GoTo lbl_rcomm
lbl_7:
            state.needprec = False
            For i_ = 0 To state.n - 1
                state.cz(i_) = state.pv(i_)
            Next
            If state.repiterationscount Mod state.itsbeforerestart <> 0 Then
                state.beta = 0
                uvar = 0
                For i = 0 To state.n - 1
                    state.beta = state.beta + state.cz(i) * state.cr(i)
                    uvar = uvar + state.z(i) * state.r(i)
                Next

                '
                'check that UVar is't INF or is't zero
                '
                If Not Math.isfinite(uvar) OrElse CDbl(uvar) = CDbl(0) Then
                    state.running = False
                    state.repterminationtype = -4
                    result = False
                    Return result
                End If

                '
                'calculate .BETA
                '
                state.beta = state.beta / uvar

                '
                'check that .BETA neither INF nor NaN
                '
                If Not Math.isfinite(state.beta) Then
                    state.running = False
                    state.repterminationtype = -1
                    result = False
                    Return result
                End If
                For i = 0 To state.n - 1
                    state.p(i) = state.cz(i) + state.beta * state.p(i)
                Next
            Else
                For i_ = 0 To state.n - 1
                    state.p(i_) = state.cz(i_)
                Next
            End If

            '
            'prepere data for next iteration
            '
            For i = 0 To state.n - 1

                '
                'write (k+1)th iteration to (k )th iteration
                '
                state.r(i) = state.cr(i)
                state.z(i) = state.cz(i)
            Next
            GoTo lbl_10
lbl_11:
            result = False
            Return result
lbl_rcomm:

            '
            ' Saving state
            '
            result = True
            state.rstate.ia(0) = i
            state.rstate.ra(0) = uvar
            state.rstate.ra(1) = bnorm
            state.rstate.ra(2) = v
            Return result
        End Function


        '************************************************************************
        '        Procedure for solution of A*x=b with sparse A.
        '
        '        INPUT PARAMETERS:
        '            State   -   algorithm state
        '            A       -   sparse matrix in the CRS format (you MUST contvert  it  to 
        '                        CRS format by calling SparseConvertToCRS() function).
        '            IsUpper -   whether upper or lower triangle of A is used:
        '                        * IsUpper=True  => only upper triangle is used and lower
        '                                           triangle is not referenced at all 
        '                        * IsUpper=False => only lower triangle is used and upper
        '                                           triangle is not referenced at all
        '            B       -   right part, array[N]
        '
        '        RESULT:
        '            This function returns no result.
        '            You can get solution by calling LinCGResults()
        '            
        '        NOTE: this function uses lightweight preconditioning -  multiplication  by
        '              inverse of diag(A). If you want, you can turn preconditioning off by
        '              calling LinCGSetPrecUnit(). However, preconditioning cost is low and
        '              preconditioner  is  very  important  for  solution  of  badly scaled
        '              problems.
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgsolvesparse(state As lincgstate, a As sparse.sparsematrix, isupper As Boolean, b As Double())
            Dim n As Integer = 0
            Dim i As Integer = 0
            Dim v As Double = 0
            Dim vmv As Double = 0
            Dim i_ As Integer = 0

            n = state.n
            alglib.ap.assert(alglib.ap.len(b) >= state.n, "LinCGSetB: Length(B)<N")
            alglib.ap.assert(apserv.isfinitevector(b, state.n), "LinCGSetB: B contains infinite or NaN values!")

            '
            ' Allocate temporaries
            '
            apserv.rvectorsetlengthatleast(state.tmpd, n)

            '
            ' Compute diagonal scaling matrix D
            '
            If state.prectype = 0 Then

                '
                ' Default preconditioner - inverse of matrix diagonal
                '
                For i = 0 To n - 1
                    v = sparse.sparsegetdiagonal(a, i)
                    If CDbl(v) > CDbl(0) Then
                        state.tmpd(i) = 1 / System.Math.sqrt(v)
                    Else
                        state.tmpd(i) = 1
                    End If
                Next
            Else

                '
                ' No diagonal scaling
                '
                For i = 0 To n - 1
                    state.tmpd(i) = 1
                Next
            End If

            '
            ' Solve
            '
            lincgrestart(state)
            lincgsetb(state, b)
            While lincgiteration(state)

                '
                ' Process different requests from optimizer
                '
                If state.needmv Then
                    sparse.sparsesmv(a, isupper, state.x, state.mv)
                End If
                If state.needvmv Then
                    sparse.sparsesmv(a, isupper, state.x, state.mv)
                    vmv = 0.0
                    For i_ = 0 To state.n - 1
                        vmv += state.x(i_) * state.mv(i_)
                    Next
                    state.vmv = vmv
                End If
                If state.needprec Then
                    For i = 0 To n - 1
                        state.pv(i) = state.x(i) * Math.sqr(state.tmpd(i))
                    Next
                End If
            End While
        End Sub


        '************************************************************************
        '        CG-solver: results.
        '
        '        This function must be called after LinCGSolve
        '
        '        INPUT PARAMETERS:
        '            State   -   algorithm state
        '
        '        OUTPUT PARAMETERS:
        '            X       -   array[N], solution
        '            Rep     -   optimization report:
        '                        * Rep.TerminationType completetion code:
        '                            * -5    input matrix is either not positive definite,
        '                                    too large or too small                            
        '                            * -4    overflow/underflow during solution
        '                                    (ill conditioned problem)
        '                            *  1    ||residual||<=EpsF*||b||
        '                            *  5    MaxIts steps was taken
        '                            *  7    rounding errors prevent further progress,
        '                                    best point found is returned
        '                        * Rep.IterationsCount contains iterations count
        '                        * NMV countains number of matrix-vector calculations
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgresults(state As lincgstate, ByRef x As Double(), rep As lincgreport)
            Dim i_ As Integer = 0

            x = New Double(-1) {}

            alglib.ap.assert(Not state.running, "LinCGResult: you can not get result, because function LinCGIteration has been launched!")
            If alglib.ap.len(x) < state.n Then
                x = New Double(state.n - 1) {}
            End If
            For i_ = 0 To state.n - 1
                x(i_) = state.rx(i_)
            Next
            rep.iterationscount = state.repiterationscount
            rep.nmv = state.repnmv
            rep.terminationtype = state.repterminationtype
            rep.r2 = state.r2
        End Sub


        '************************************************************************
        '        This function sets restart frequency. By default, algorithm  is  restarted
        '        after N subsequent iterations.
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgsetrestartfreq(state As lincgstate, srf As Integer)
            alglib.ap.assert(Not state.running, "LinCGSetRestartFreq: you can not change restart frequency when LinCGIteration() is running")
            alglib.ap.assert(srf > 0, "LinCGSetRestartFreq: non-positive SRF")
            state.itsbeforerestart = srf
        End Sub


        '************************************************************************
        '        This function sets frequency of residual recalculations.
        '
        '        Algorithm updates residual r_k using iterative formula,  but  recalculates
        '        it from scratch after each 10 iterations. It is done to avoid accumulation
        '        of numerical errors and to stop algorithm when r_k starts to grow.
        '
        '        Such low update frequence (1/10) gives very  little  overhead,  but  makes
        '        algorithm a bit more robust against numerical errors. However, you may
        '        change it 
        '
        '        INPUT PARAMETERS:
        '            Freq    -   desired update frequency, Freq>=0.
        '                        Zero value means that no updates will be done.
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgsetrupdatefreq(state As lincgstate, freq As Integer)
            alglib.ap.assert(Not state.running, "LinCGSetRUpdateFreq: you can not change update frequency when LinCGIteration() is running")
            alglib.ap.assert(freq >= 0, "LinCGSetRUpdateFreq: non-positive Freq")
            state.itsbeforerupdate = freq
        End Sub


        '************************************************************************
        '        This function turns on/off reporting.
        '
        '        INPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '            NeedXRep-   whether iteration reports are needed or not
        '
        '        If NeedXRep is True, algorithm will call rep() callback function if  it is
        '        provided to MinCGOptimize().
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgsetxrep(state As lincgstate, needxrep As Boolean)
            state.xrep = needxrep
        End Sub


        '************************************************************************
        '        Procedure for restart function LinCGIteration
        '
        '          -- ALGLIB --
        '             Copyright 14.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub lincgrestart(state As lincgstate)
            state.rstate.ia = New Integer(0) {}
            state.rstate.ra = New Double(2) {}
            state.rstate.stage = -1
            clearrfields(state)
        End Sub


        '************************************************************************
        '        Clears request fileds (to be sure that we don't forgot to clear something)
        '        ************************************************************************

        Private Shared Sub clearrfields(state As lincgstate)
            state.xupdated = False
            state.needmv = False
            state.needmtv = False
            state.needmv2 = False
            state.needvmv = False
            state.needprec = False
        End Sub


        '************************************************************************
        '        Clears request fileds (to be sure that we don't forgot to clear something)
        '        ************************************************************************

        Private Shared Sub updateitersdata(state As lincgstate)
            state.repiterationscount = 0
            state.repnmv = 0
            state.repterminationtype = 0
        End Sub


    End Class
    Public Class nleq
        Public Class nleqstate
            Inherits apobject
            Public n As Integer
            Public m As Integer
            Public epsf As Double
            Public maxits As Integer
            Public xrep As Boolean
            Public stpmax As Double
            Public x As Double()
            Public f As Double
            Public fi As Double()
            Public j As Double(,)
            Public needf As Boolean
            Public needfij As Boolean
            Public xupdated As Boolean
            Public rstate As rcommstate
            Public repiterationscount As Integer
            Public repnfunc As Integer
            Public repnjac As Integer
            Public repterminationtype As Integer
            Public xbase As Double()
            Public fbase As Double
            Public fprev As Double
            Public candstep As Double()
            Public rightpart As Double()
            Public cgbuf As Double()
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
                x = New Double(-1) {}
                fi = New Double(-1) {}
                j = New Double(-1, -1) {}
                rstate = New rcommstate()
                xbase = New Double(-1) {}
                candstep = New Double(-1) {}
                rightpart = New Double(-1) {}
                cgbuf = New Double(-1) {}
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New nleqstate()
                _result.n = n
                _result.m = m
                _result.epsf = epsf
                _result.maxits = maxits
                _result.xrep = xrep
                _result.stpmax = stpmax
                _result.x = DirectCast(x.Clone(), Double())
                _result.f = f
                _result.fi = DirectCast(fi.Clone(), Double())
                _result.j = DirectCast(j.Clone(), Double(,))
                _result.needf = needf
                _result.needfij = needfij
                _result.xupdated = xupdated
                _result.rstate = DirectCast(rstate.make_copy(), rcommstate)
                _result.repiterationscount = repiterationscount
                _result.repnfunc = repnfunc
                _result.repnjac = repnjac
                _result.repterminationtype = repterminationtype
                _result.xbase = DirectCast(xbase.Clone(), Double())
                _result.fbase = fbase
                _result.fprev = fprev
                _result.candstep = DirectCast(candstep.Clone(), Double())
                _result.rightpart = DirectCast(rightpart.Clone(), Double())
                _result.cgbuf = DirectCast(cgbuf.Clone(), Double())
                Return _result
            End Function
        End Class


        Public Class nleqreport
            Inherits apobject
            Public iterationscount As Integer
            Public nfunc As Integer
            Public njac As Integer
            Public terminationtype As Integer
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New nleqreport()
                _result.iterationscount = iterationscount
                _result.nfunc = nfunc
                _result.njac = njac
                _result.terminationtype = terminationtype
                Return _result
            End Function
        End Class




        '************************************************************************
        '                        LEVENBERG-MARQUARDT-LIKE NONLINEAR SOLVER
        '
        '        DESCRIPTION:
        '        This algorithm solves system of nonlinear equations
        '            F[0](x[0], ..., x[n-1])   = 0
        '            F[1](x[0], ..., x[n-1])   = 0
        '            ...
        '            F[M-1](x[0], ..., x[n-1]) = 0
        '        with M/N do not necessarily coincide.  Algorithm  converges  quadratically
        '        under following conditions:
        '            * the solution set XS is nonempty
        '            * for some xs in XS there exist such neighbourhood N(xs) that:
        '              * vector function F(x) and its Jacobian J(x) are continuously
        '                differentiable on N
        '              * ||F(x)|| provides local error bound on N, i.e. there  exists  such
        '                c1, that ||F(x)||>c1*distance(x,XS)
        '        Note that these conditions are much more weaker than usual non-singularity
        '        conditions. For example, algorithm will converge for any  affine  function
        '        F (whether its Jacobian singular or not).
        '
        '
        '        REQUIREMENTS:
        '        Algorithm will request following information during its operation:
        '        * function vector F[] and Jacobian matrix at given point X
        '        * value of merit function f(x)=F[0]^2(x)+...+F[M-1]^2(x) at given point X
        '
        '
        '        USAGE:
        '        1. User initializes algorithm state with NLEQCreateLM() call
        '        2. User tunes solver parameters with  NLEQSetCond(),  NLEQSetStpMax()  and
        '           other functions
        '        3. User  calls  NLEQSolve()  function  which  takes  algorithm  state  and
        '           pointers (delegates, etc.) to callback functions which calculate  merit
        '           function value and Jacobian.
        '        4. User calls NLEQResults() to get solution
        '        5. Optionally, user may call NLEQRestartFrom() to  solve  another  problem
        '           with same parameters (N/M) but another starting  point  and/or  another
        '           function vector. NLEQRestartFrom() allows to reuse already  initialized
        '           structure.
        '
        '
        '        INPUT PARAMETERS:
        '            N       -   space dimension, N>1:
        '                        * if provided, only leading N elements of X are used
        '                        * if not provided, determined automatically from size of X
        '            M       -   system size
        '            X       -   starting point
        '
        '
        '        OUTPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '
        '
        '        NOTES:
        '        1. you may tune stopping conditions with NLEQSetCond() function
        '        2. if target function contains exp() or other fast growing functions,  and
        '           optimization algorithm makes too large steps which leads  to  overflow,
        '           use NLEQSetStpMax() function to bound algorithm's steps.
        '        3. this  algorithm  is  a  slightly  modified implementation of the method
        '           described  in  'Levenberg-Marquardt  method  for constrained  nonlinear
        '           equations with strong local convergence properties' by Christian Kanzow
        '           Nobuo Yamashita and Masao Fukushima and further  developed  in  'On the
        '           convergence of a New Levenberg-Marquardt Method'  by  Jin-yan  Fan  and
        '           Ya-Xiang Yuan.
        '
        '
        '          -- ALGLIB --
        '             Copyright 20.08.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub nleqcreatelm(n As Integer, m As Integer, x As Double(), state As nleqstate)
            alglib.ap.assert(n >= 1, "NLEQCreateLM: N<1!")
            alglib.ap.assert(m >= 1, "NLEQCreateLM: M<1!")
            alglib.ap.assert(alglib.ap.len(x) >= n, "NLEQCreateLM: Length(X)<N!")
            alglib.ap.assert(apserv.isfinitevector(x, n), "NLEQCreateLM: X contains infinite or NaN values!")

            '
            ' Initialize
            '
            state.n = n
            state.m = m
            nleqsetcond(state, 0, 0)
            nleqsetxrep(state, False)
            nleqsetstpmax(state, 0)
            state.x = New Double(n - 1) {}
            state.xbase = New Double(n - 1) {}
            state.j = New Double(m - 1, n - 1) {}
            state.fi = New Double(m - 1) {}
            state.rightpart = New Double(n - 1) {}
            state.candstep = New Double(n - 1) {}
            nleqrestartfrom(state, x)
        End Sub


        '************************************************************************
        '        This function sets stopping conditions for the nonlinear solver
        '
        '        INPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '            EpsF    -   >=0
        '                        The subroutine finishes  its work if on k+1-th iteration
        '                        the condition ||F||<=EpsF is satisfied
        '            MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
        '                        iterations is unlimited.
        '
        '        Passing EpsF=0 and MaxIts=0 simultaneously will lead to  automatic
        '        stopping criterion selection (small EpsF).
        '
        '        NOTES:
        '
        '          -- ALGLIB --
        '             Copyright 20.08.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub nleqsetcond(state As nleqstate, epsf As Double, maxits As Integer)
            alglib.ap.assert(Math.isfinite(epsf), "NLEQSetCond: EpsF is not finite number!")
            alglib.ap.assert(CDbl(epsf) >= CDbl(0), "NLEQSetCond: negative EpsF!")
            alglib.ap.assert(maxits >= 0, "NLEQSetCond: negative MaxIts!")
            If CDbl(epsf) = CDbl(0) AndAlso maxits = 0 Then
                epsf = 0.000001
            End If
            state.epsf = epsf
            state.maxits = maxits
        End Sub


        '************************************************************************
        '        This function turns on/off reporting.
        '
        '        INPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '            NeedXRep-   whether iteration reports are needed or not
        '
        '        If NeedXRep is True, algorithm will call rep() callback function if  it is
        '        provided to NLEQSolve().
        '
        '          -- ALGLIB --
        '             Copyright 20.08.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub nleqsetxrep(state As nleqstate, needxrep As Boolean)
            state.xrep = needxrep
        End Sub


        '************************************************************************
        '        This function sets maximum step length
        '
        '        INPUT PARAMETERS:
        '            State   -   structure which stores algorithm state
        '            StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
        '                        want to limit step length.
        '
        '        Use this subroutine when target function  contains  exp()  or  other  fast
        '        growing functions, and algorithm makes  too  large  steps  which  lead  to
        '        overflow. This function allows us to reject steps that are too large  (and
        '        therefore expose us to the possible overflow) without actually calculating
        '        function value at the x+stp*d.
        '
        '          -- ALGLIB --
        '             Copyright 20.08.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub nleqsetstpmax(state As nleqstate, stpmax As Double)
            alglib.ap.assert(Math.isfinite(stpmax), "NLEQSetStpMax: StpMax is not finite!")
            alglib.ap.assert(CDbl(stpmax) >= CDbl(0), "NLEQSetStpMax: StpMax<0!")
            state.stpmax = stpmax
        End Sub


        '************************************************************************
        '
        '          -- ALGLIB --
        '             Copyright 20.03.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function nleqiteration(state As nleqstate) As Boolean
            Dim result As New Boolean()
            Dim n As Integer = 0
            Dim m As Integer = 0
            Dim i As Integer = 0
            Dim lambdaup As Double = 0
            Dim lambdadown As Double = 0
            Dim lambdav As Double = 0
            Dim rho As Double = 0
            Dim mu As Double = 0
            Dim stepnorm As Double = 0
            Dim b As New Boolean()
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
                b = state.rstate.ba(0)
                lambdaup = state.rstate.ra(0)
                lambdadown = state.rstate.ra(1)
                lambdav = state.rstate.ra(2)
                rho = state.rstate.ra(3)
                mu = state.rstate.ra(4)
                stepnorm = state.rstate.ra(5)
            Else
                n = -983
                m = -989
                i = -834
                b = False
                lambdaup = -287
                lambdadown = 364
                lambdav = 214
                rho = -338
                mu = -686
                stepnorm = 912
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
            If state.rstate.stage = 4 Then
                GoTo lbl_4
            End If

            '
            ' Routine body
            '

            '
            ' Prepare
            '
            n = state.n
            m = state.m
            state.repterminationtype = 0
            state.repiterationscount = 0
            state.repnfunc = 0
            state.repnjac = 0

            '
            ' Calculate F/G, initialize algorithm
            '
            clearrequestfields(state)
            state.needf = True
            state.rstate.stage = 0
            GoTo lbl_rcomm
lbl_0:
            state.needf = False
            state.repnfunc = state.repnfunc + 1
            For i_ = 0 To n - 1
                state.xbase(i_) = state.x(i_)
            Next
            state.fbase = state.f
            state.fprev = Math.maxrealnumber
            If Not state.xrep Then
                GoTo lbl_5
            End If

            '
            ' progress report
            '
            clearrequestfields(state)
            state.xupdated = True
            state.rstate.stage = 1
            GoTo lbl_rcomm
lbl_1:
            state.xupdated = False
lbl_5:
            If CDbl(state.f) <= CDbl(Math.sqr(state.epsf)) Then
                state.repterminationtype = 1
                result = False
                Return result
            End If

            '
            ' Main cycle
            '
            lambdaup = 10
            lambdadown = 0.3
            lambdav = 0.001
            rho = 1
lbl_7:
            If False Then
                GoTo lbl_8
            End If

            '
            ' Get Jacobian;
            ' before we get to this point we already have State.XBase filled
            ' with current point and State.FBase filled with function value
            ' at XBase
            '
            clearrequestfields(state)
            state.needfij = True
            For i_ = 0 To n - 1
                state.x(i_) = state.xbase(i_)
            Next
            state.rstate.stage = 2
            GoTo lbl_rcomm
lbl_2:
            state.needfij = False
            state.repnfunc = state.repnfunc + 1
            state.repnjac = state.repnjac + 1
            ablas.rmatrixmv(n, m, state.j, 0, 0, 1, _
                state.fi, 0, state.rightpart, 0)
            For i_ = 0 To n - 1
                state.rightpart(i_) = -1 * state.rightpart(i_)
            Next
lbl_9:

            '
            ' Inner cycle: find good lambda
            '
            If False Then
                GoTo lbl_10
            End If

            '
            ' Solve (J^T*J + (Lambda+Mu)*I)*y = J^T*F
            ' to get step d=-y where:
            ' * Mu=||F|| - is damping parameter for nonlinear system
            ' * Lambda   - is additional Levenberg-Marquardt parameter
            '              for better convergence when far away from minimum
            '
            For i = 0 To n - 1
                state.candstep(i) = 0
            Next
            fbls.fblssolvecgx(state.j, m, n, lambdav, state.rightpart, state.candstep, _
                state.cgbuf)

            '
            ' Normalize step (it must be no more than StpMax)
            '
            stepnorm = 0
            For i = 0 To n - 1
                If CDbl(state.candstep(i)) <> CDbl(0) Then
                    stepnorm = 1
                    Exit For
                End If
            Next
            linmin.linminnormalized(state.candstep, stepnorm, n)
            If CDbl(state.stpmax) <> CDbl(0) Then
                stepnorm = System.Math.Min(stepnorm, state.stpmax)
            End If

            '
            ' Test new step - is it good enough?
            ' * if not, Lambda is increased and we try again.
            ' * if step is good, we decrease Lambda and move on.
            '
            ' We can break this cycle on two occasions:
            ' * step is so small that x+step==x (in floating point arithmetics)
            ' * lambda is so large
            '
            For i_ = 0 To n - 1
                state.x(i_) = state.xbase(i_)
            Next
            For i_ = 0 To n - 1
                state.x(i_) = state.x(i_) + stepnorm * state.candstep(i_)
            Next
            b = True
            For i = 0 To n - 1
                If CDbl(state.x(i)) <> CDbl(state.xbase(i)) Then
                    b = False
                    Exit For
                End If
            Next
            If b Then

                '
                ' Step is too small, force zero step and break
                '
                stepnorm = 0
                For i_ = 0 To n - 1
                    state.x(i_) = state.xbase(i_)
                Next
                state.f = state.fbase
                GoTo lbl_10
            End If
            clearrequestfields(state)
            state.needf = True
            state.rstate.stage = 3
            GoTo lbl_rcomm
lbl_3:
            state.needf = False
            state.repnfunc = state.repnfunc + 1
            If CDbl(state.f) < CDbl(state.fbase) Then

                '
                ' function value decreased, move on
                '
                decreaselambda(lambdav, rho, lambdadown)
                GoTo lbl_10
            End If
            If Not increaselambda(lambdav, rho, lambdaup) Then

                '
                ' Lambda is too large (near overflow), force zero step and break
                '
                stepnorm = 0
                For i_ = 0 To n - 1
                    state.x(i_) = state.xbase(i_)
                Next
                state.f = state.fbase
                GoTo lbl_10
            End If
            GoTo lbl_9
lbl_10:

            '
            ' Accept step:
            ' * new position
            ' * new function value
            '
            state.fbase = state.f
            For i_ = 0 To n - 1
                state.xbase(i_) = state.xbase(i_) + stepnorm * state.candstep(i_)
            Next
            state.repiterationscount = state.repiterationscount + 1

            '
            ' Report new iteration
            '
            If Not state.xrep Then
                GoTo lbl_11
            End If
            clearrequestfields(state)
            state.xupdated = True
            state.f = state.fbase
            For i_ = 0 To n - 1
                state.x(i_) = state.xbase(i_)
            Next
            state.rstate.stage = 4
            GoTo lbl_rcomm
lbl_4:
            state.xupdated = False
lbl_11:

            '
            ' Test stopping conditions on F, step (zero/non-zero) and MaxIts;
            ' If one of the conditions is met, RepTerminationType is changed.
            '
            If CDbl(System.Math.sqrt(state.f)) <= CDbl(state.epsf) Then
                state.repterminationtype = 1
            End If
            If CDbl(stepnorm) = CDbl(0) AndAlso state.repterminationtype = 0 Then
                state.repterminationtype = -4
            End If
            If state.repiterationscount >= state.maxits AndAlso state.maxits > 0 Then
                state.repterminationtype = 5
            End If
            If state.repterminationtype <> 0 Then
                GoTo lbl_8
            End If

            '
            ' Now, iteration is finally over
            '
            GoTo lbl_7
lbl_8:
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
            state.rstate.ba(0) = b
            state.rstate.ra(0) = lambdaup
            state.rstate.ra(1) = lambdadown
            state.rstate.ra(2) = lambdav
            state.rstate.ra(3) = rho
            state.rstate.ra(4) = mu
            state.rstate.ra(5) = stepnorm
            Return result
        End Function


        '************************************************************************
        '        NLEQ solver results
        '
        '        INPUT PARAMETERS:
        '            State   -   algorithm state.
        '
        '        OUTPUT PARAMETERS:
        '            X       -   array[0..N-1], solution
        '            Rep     -   optimization report:
        '                        * Rep.TerminationType completetion code:
        '                            * -4    ERROR:  algorithm   has   converged   to   the
        '                                    stationary point Xf which is local minimum  of
        '                                    f=F[0]^2+...+F[m-1]^2, but is not solution  of
        '                                    nonlinear system.
        '                            *  1    sqrt(f)<=EpsF.
        '                            *  5    MaxIts steps was taken
        '                            *  7    stopping conditions are too stringent,
        '                                    further improvement is impossible
        '                        * Rep.IterationsCount contains iterations count
        '                        * NFEV countains number of function calculations
        '                        * ActiveConstraints contains number of active constraints
        '
        '          -- ALGLIB --
        '             Copyright 20.08.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub nleqresults(state As nleqstate, ByRef x As Double(), rep As nleqreport)
            x = New Double(-1) {}

            nleqresultsbuf(state, x, rep)
        End Sub


        '************************************************************************
        '        NLEQ solver results
        '
        '        Buffered implementation of NLEQResults(), which uses pre-allocated  buffer
        '        to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
        '        intended to be used in the inner cycles of performance critical algorithms
        '        where array reallocation penalty is too large to be ignored.
        '
        '          -- ALGLIB --
        '             Copyright 20.08.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub nleqresultsbuf(state As nleqstate, ByRef x As Double(), rep As nleqreport)
            Dim i_ As Integer = 0

            If alglib.ap.len(x) < state.n Then
                x = New Double(state.n - 1) {}
            End If
            For i_ = 0 To state.n - 1
                x(i_) = state.xbase(i_)
            Next
            rep.iterationscount = state.repiterationscount
            rep.nfunc = state.repnfunc
            rep.njac = state.repnjac
            rep.terminationtype = state.repterminationtype
        End Sub


        '************************************************************************
        '        This  subroutine  restarts  CG  algorithm from new point. All optimization
        '        parameters are left unchanged.
        '
        '        This  function  allows  to  solve multiple  optimization  problems  (which
        '        must have same number of dimensions) without object reallocation penalty.
        '
        '        INPUT PARAMETERS:
        '            State   -   structure used for reverse communication previously
        '                        allocated with MinCGCreate call.
        '            X       -   new starting point.
        '            BndL    -   new lower bounds
        '            BndU    -   new upper bounds
        '
        '          -- ALGLIB --
        '             Copyright 30.07.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub nleqrestartfrom(state As nleqstate, x As Double())
            Dim i_ As Integer = 0

            alglib.ap.assert(alglib.ap.len(x) >= state.n, "NLEQRestartFrom: Length(X)<N!")
            alglib.ap.assert(apserv.isfinitevector(x, state.n), "NLEQRestartFrom: X contains infinite or NaN values!")
            For i_ = 0 To state.n - 1
                state.x(i_) = x(i_)
            Next
            state.rstate.ia = New Integer(2) {}
            state.rstate.ba = New Boolean(0) {}
            state.rstate.ra = New Double(5) {}
            state.rstate.stage = -1
            clearrequestfields(state)
        End Sub


        '************************************************************************
        '        Clears request fileds (to be sure that we don't forgot to clear something)
        '        ************************************************************************

        Private Shared Sub clearrequestfields(state As nleqstate)
            state.needf = False
            state.needfij = False
            state.xupdated = False
        End Sub


        '************************************************************************
        '        Increases lambda, returns False when there is a danger of overflow
        '        ************************************************************************

        Private Shared Function increaselambda(ByRef lambdav As Double, ByRef nu As Double, lambdaup As Double) As Boolean
            Dim result As New Boolean()
            Dim lnlambda As Double = 0
            Dim lnnu As Double = 0
            Dim lnlambdaup As Double = 0
            Dim lnmax As Double = 0

            result = False
            lnlambda = System.Math.Log(lambdav)
            lnlambdaup = System.Math.Log(lambdaup)
            lnnu = System.Math.Log(nu)
            lnmax = 0.5 * System.Math.Log(Math.maxrealnumber)
            If CDbl(lnlambda + lnlambdaup + lnnu) > CDbl(lnmax) Then
                Return result
            End If
            If CDbl(lnnu + System.Math.Log(2)) > CDbl(lnmax) Then
                Return result
            End If
            lambdav = lambdav * lambdaup * nu
            nu = nu * 2
            result = True
            Return result
        End Function


        '************************************************************************
        '        Decreases lambda, but leaves it unchanged when there is danger of underflow.
        '        ************************************************************************

        Private Shared Sub decreaselambda(ByRef lambdav As Double, ByRef nu As Double, lambdadown As Double)
            nu = 1
            If CDbl(System.Math.Log(lambdav) + System.Math.Log(lambdadown)) < CDbl(System.Math.Log(Math.minrealnumber)) Then
                lambdav = Math.minrealnumber
            Else
                lambdav = lambdav * lambdadown
            End If
        End Sub


    End Class
    Public Class polynomialsolver
        Public Class polynomialsolverreport
            Inherits apobject
            Public maxerr As Double
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New polynomialsolverreport()
                _result.maxerr = maxerr
                Return _result
            End Function
        End Class




        '************************************************************************
        '        Polynomial root finding.
        '
        '        This function returns all roots of the polynomial
        '            P(x) = a0 + a1*x + a2*x^2 + ... + an*x^n
        '        Both real and complex roots are returned (see below).
        '
        '        INPUT PARAMETERS:
        '            A       -   array[N+1], polynomial coefficients:
        '                        * A[0] is constant term
        '                        * A[N] is a coefficient of X^N
        '            N       -   polynomial degree
        '
        '        OUTPUT PARAMETERS:
        '            X       -   array of complex roots:
        '                        * for isolated real root, X[I] is strictly real: IMAGE(X[I])=0
        '                        * complex roots are always returned in pairs - roots occupy
        '                          positions I and I+1, with:
        '                          * X[I+1]=Conj(X[I])
        '                          * IMAGE(X[I]) > 0
        '                          * IMAGE(X[I+1]) = -IMAGE(X[I]) < 0
        '                        * multiple real roots may have non-zero imaginary part due
        '                          to roundoff errors. There is no reliable way to distinguish
        '                          real root of multiplicity 2 from two  complex  roots  in
        '                          the presence of roundoff errors.
        '            Rep     -   report, additional information, following fields are set:
        '                        * Rep.MaxErr - max( |P(xi)| )  for  i=0..N-1.  This  field
        '                          allows to quickly estimate "quality" of the roots  being
        '                          returned.
        '
        '        NOTE:   this function uses companion matrix method to find roots. In  case
        '                internal EVD  solver  fails  do  find  eigenvalues,  exception  is
        '                generated.
        '
        '        NOTE:   roots are not "polished" and  no  matrix  balancing  is  performed
        '                for them.
        '
        '          -- ALGLIB --
        '             Copyright 24.02.2014 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub polynomialsolve(a As Double(), n As Integer, ByRef x As complex(), rep As polynomialsolverreport)
            Dim c As Double(,) = New Double(-1, -1) {}
            Dim vl As Double(,) = New Double(-1, -1) {}
            Dim vr As Double(,) = New Double(-1, -1) {}
            Dim wr As Double() = New Double(-1) {}
            Dim wi As Double() = New Double(-1) {}
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim status As New Boolean()
            Dim nz As Integer = 0
            Dim ne As Integer = 0
            Dim v As complex = 0
            Dim vv As complex = 0

            a = DirectCast(a.Clone(), Double())
            x = New complex(-1) {}

            alglib.ap.assert(n > 0, "PolynomialSolve: N<=0")
            alglib.ap.assert(alglib.ap.len(a) >= n + 1, "PolynomialSolve: Length(A)<N+1")
            alglib.ap.assert(apserv.isfinitevector(a, n + 1), "PolynomialSolve: A contains infitite numbers")
            alglib.ap.assert(CDbl(a(n)) <> CDbl(0), "PolynomialSolve: A[N]=0")

            '
            ' Prepare
            '
            x = New complex(n - 1) {}

            '
            ' Normalize A:
            ' * analytically determine NZ zero roots
            ' * quick exit for NZ=N
            ' * make residual NE-th degree polynomial monic
            '   (here NE=N-NZ)
            '
            nz = 0
            While nz < n AndAlso CDbl(a(nz)) = CDbl(0)
                nz = nz + 1
            End While
            ne = n - nz
            For i = nz To n
                a(i - nz) = a(i) / a(n)
            Next

            '
            ' For NZ<N, build companion matrix and find NE non-zero roots
            '
            If ne > 0 Then
                c = New Double(ne - 1, ne - 1) {}
                For i = 0 To ne - 1
                    For j = 0 To ne - 1
                        c(i, j) = 0
                    Next
                Next
                c(0, ne - 1) = -a(0)
                For i = 1 To ne - 1
                    c(i, i - 1) = 1
                    c(i, ne - 1) = -a(i)
                Next
                status = evd.rmatrixevd(c, ne, 0, wr, wi, vl, _
                    vr)
                alglib.ap.assert(status, "PolynomialSolve: inernal error - EVD solver failed")
                For i = 0 To ne - 1
                    x(i).x = wr(i)
                    x(i).y = wi(i)
                Next
            End If

            '
            ' Remaining NZ zero roots
            '
            For i = ne To n - 1
                x(i) = 0
            Next

            '
            ' Rep
            '
            rep.maxerr = 0
            For i = 0 To ne - 1
                v = 0
                vv = 1
                For j = 0 To ne
                    v = v + a(j) * vv
                    vv = vv * x(i)
                Next
                rep.maxerr = System.Math.Max(rep.maxerr, Math.abscomplex(v))
            Next
        End Sub


    End Class
End Class

