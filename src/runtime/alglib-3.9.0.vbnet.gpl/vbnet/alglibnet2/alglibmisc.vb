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
'    Portable high quality random number generator state.
'    Initialized with HQRNDRandomize() or HQRNDSeed().
'
'    Fields:
'        S1, S2      -   seed values
'        V           -   precomputed value
'        MagicV      -   'magic' value used to determine whether State structure
'                        was correctly initialized.
'    ************************************************************************

	Public Class hqrndstate
		Inherits alglibobject
		'
		' Public declarations
		'

		Public Sub New()
			_innerobj = New hqrnd.hqrndstate()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New hqrndstate(DirectCast(_innerobj.make_copy(), hqrnd.hqrndstate))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As hqrnd.hqrndstate
		Public ReadOnly Property innerobj() As hqrnd.hqrndstate
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As hqrnd.hqrndstate)
			_innerobj = obj
		End Sub
	End Class

	'************************************************************************
'    HQRNDState  initialization  with  random  values  which come from standard
'    RNG.
'
'      -- ALGLIB --
'         Copyright 02.12.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub hqrndrandomize(ByRef state As hqrndstate)
		state = New hqrndstate()
		hqrnd.hqrndrandomize(state.innerobj)
		Return
	End Sub

	'************************************************************************
'    HQRNDState initialization with seed values
'
'      -- ALGLIB --
'         Copyright 02.12.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub hqrndseed(s1 As Integer, s2 As Integer, ByRef state As hqrndstate)
		state = New hqrndstate()
		hqrnd.hqrndseed(s1, s2, state.innerobj)
		Return
	End Sub

	'************************************************************************
'    This function generates random real number in (0,1),
'    not including interval boundaries
'
'    State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
'
'      -- ALGLIB --
'         Copyright 02.12.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function hqrnduniformr(state As hqrndstate) As Double

		Dim result As Double = hqrnd.hqrnduniformr(state.innerobj)
		Return result
	End Function

	'************************************************************************
'    This function generates random integer number in [0, N)
'
'    1. State structure must be initialized with HQRNDRandomize() or HQRNDSeed()
'    2. N can be any positive number except for very large numbers:
'       * close to 2^31 on 32-bit systems
'       * close to 2^62 on 64-bit systems
'       An exception will be generated if N is too large.
'
'      -- ALGLIB --
'         Copyright 02.12.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function hqrnduniformi(state As hqrndstate, n As Integer) As Integer

		Dim result As Integer = hqrnd.hqrnduniformi(state.innerobj, n)
		Return result
	End Function

	'************************************************************************
'    Random number generator: normal numbers
'
'    This function generates one random number from normal distribution.
'    Its performance is equal to that of HQRNDNormal2()
'
'    State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
'
'      -- ALGLIB --
'         Copyright 02.12.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function hqrndnormal(state As hqrndstate) As Double

		Dim result As Double = hqrnd.hqrndnormal(state.innerobj)
		Return result
	End Function

	'************************************************************************
'    Random number generator: random X and Y such that X^2+Y^2=1
'
'    State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
'
'      -- ALGLIB --
'         Copyright 02.12.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub hqrndunit2(state As hqrndstate, ByRef x As Double, ByRef y As Double)
		x = 0
		y = 0
		hqrnd.hqrndunit2(state.innerobj, x, y)
		Return
	End Sub

	'************************************************************************
'    Random number generator: normal numbers
'
'    This function generates two independent random numbers from normal
'    distribution. Its performance is equal to that of HQRNDNormal()
'
'    State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
'
'      -- ALGLIB --
'         Copyright 02.12.2009 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub hqrndnormal2(state As hqrndstate, ByRef x1 As Double, ByRef x2 As Double)
		x1 = 0
		x2 = 0
		hqrnd.hqrndnormal2(state.innerobj, x1, x2)
		Return
	End Sub

	'************************************************************************
'    Random number generator: exponential distribution
'
'    State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
'
'      -- ALGLIB --
'         Copyright 11.08.2007 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function hqrndexponential(state As hqrndstate, lambdav As Double) As Double

		Dim result As Double = hqrnd.hqrndexponential(state.innerobj, lambdav)
		Return result
	End Function

	'************************************************************************
'    This function generates  random number from discrete distribution given by
'    finite sample X.
'
'    INPUT PARAMETERS
'        State   -   high quality random number generator, must be
'                    initialized with HQRNDRandomize() or HQRNDSeed().
'            X   -   finite sample
'            N   -   number of elements to use, N>=1
'
'    RESULT
'        this function returns one of the X[i] for random i=0..N-1
'
'      -- ALGLIB --
'         Copyright 08.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function hqrnddiscrete(state As hqrndstate, x As Double(), n As Integer) As Double

		Dim result As Double = hqrnd.hqrnddiscrete(state.innerobj, x, n)
		Return result
	End Function

	'************************************************************************
'    This function generates random number from continuous  distribution  given
'    by finite sample X.
'
'    INPUT PARAMETERS
'        State   -   high quality random number generator, must be
'                    initialized with HQRNDRandomize() or HQRNDSeed().
'            X   -   finite sample, array[N] (can be larger, in this  case only
'                    leading N elements are used). THIS ARRAY MUST BE SORTED BY
'                    ASCENDING.
'            N   -   number of elements to use, N>=1
'
'    RESULT
'        this function returns random number from continuous distribution which
'        tries to approximate X as mush as possible. min(X)<=Result<=max(X).
'
'      -- ALGLIB --
'         Copyright 08.11.2011 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function hqrndcontinuous(state As hqrndstate, x As Double(), n As Integer) As Double

		Dim result As Double = hqrnd.hqrndcontinuous(state.innerobj, x, n)
		Return result
	End Function

End Class
Public Partial Class alglib


	'************************************************************************
'
'    ************************************************************************

	Public Class kdtree
		Inherits alglibobject
		'
		' Public declarations
		'

		Public Sub New()
			_innerobj = New nearestneighbor.kdtree()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New kdtree(DirectCast(_innerobj.make_copy(), nearestneighbor.kdtree))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As nearestneighbor.kdtree
		Public ReadOnly Property innerobj() As nearestneighbor.kdtree
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As nearestneighbor.kdtree)
			_innerobj = obj
		End Sub
	End Class


	'************************************************************************
'    This function serializes data structure to string.
'
'    Important properties of s_out:
'    * it contains alphanumeric characters, dots, underscores, minus signs
'    * these symbols are grouped into words, which are separated by spaces
'      and Windows-style (CR+LF) newlines
'    * although  serializer  uses  spaces and CR+LF as separators, you can 
'      replace any separator character by arbitrary combination of spaces,
'      tabs, Windows or Unix newlines. It allows flexible reformatting  of
'      the  string  in  case you want to include it into text or XML file. 
'      But you should not insert separators into the middle of the "words"
'      nor you should change case of letters.
'    * s_out can be freely moved between 32-bit and 64-bit systems, little
'      and big endian machines, and so on. You can serialize structure  on
'      32-bit machine and unserialize it on 64-bit one (or vice versa), or
'      serialize  it  on  SPARC  and  unserialize  on  x86.  You  can also 
'      serialize  it  in  C# version of ALGLIB and unserialize in C++ one, 
'      and vice versa.
'    ************************************************************************

	Public Shared Sub kdtreeserialize(obj As kdtree, ByRef s_out As String)
		Dim s As New alglib.serializer()
		s.alloc_start()
		nearestneighbor.kdtreealloc(s, obj.innerobj)
		s.sstart_str()
		nearestneighbor.kdtreeserialize(s, obj.innerobj)
		s.[stop]()
		s_out = s.get_string()
	End Sub


	'************************************************************************
'    This function unserializes data structure from string.
'    ************************************************************************

	Public Shared Sub kdtreeunserialize(s_in As String, ByRef obj As kdtree)
		Dim s As New alglib.serializer()
		obj = New kdtree()
		s.ustart_str(s_in)
		nearestneighbor.kdtreeunserialize(s, obj.innerobj)
		s.[stop]()
	End Sub

	'************************************************************************
'    KD-tree creation
'
'    This subroutine creates KD-tree from set of X-values and optional Y-values
'
'    INPUT PARAMETERS
'        XY      -   dataset, array[0..N-1,0..NX+NY-1].
'                    one row corresponds to one point.
'                    first NX columns contain X-values, next NY (NY may be zero)
'                    columns may contain associated Y-values
'        N       -   number of points, N>=0.
'        NX      -   space dimension, NX>=1.
'        NY      -   number of optional Y-values, NY>=0.
'        NormType-   norm type:
'                    * 0 denotes infinity-norm
'                    * 1 denotes 1-norm
'                    * 2 denotes 2-norm (Euclidean norm)
'
'    OUTPUT PARAMETERS
'        KDT     -   KD-tree
'
'
'    NOTES
'
'    1. KD-tree  creation  have O(N*logN) complexity and O(N*(2*NX+NY))  memory
'       requirements.
'    2. Although KD-trees may be used with any combination of N  and  NX,  they
'       are more efficient than brute-force search only when N >> 4^NX. So they
'       are most useful in low-dimensional tasks (NX=2, NX=3). NX=1  is another
'       inefficient case, because  simple  binary  search  (without  additional
'       structures) is much more efficient in such tasks than KD-trees.
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreebuild(xy As Double(,), n As Integer, nx As Integer, ny As Integer, normtype As Integer, ByRef kdt As kdtree)
		kdt = New kdtree()
		nearestneighbor.kdtreebuild(xy, n, nx, ny, normtype, kdt.innerobj)
		Return
	End Sub
	Public Shared Sub kdtreebuild(xy As Double(,), nx As Integer, ny As Integer, normtype As Integer, ByRef kdt As kdtree)
		Dim n As Integer

		kdt = New kdtree()
		n = ap.rows(xy)
		nearestneighbor.kdtreebuild(xy, n, nx, ny, normtype, kdt.innerobj)

		Return
	End Sub

	'************************************************************************
'    KD-tree creation
'
'    This  subroutine  creates  KD-tree  from set of X-values, integer tags and
'    optional Y-values
'
'    INPUT PARAMETERS
'        XY      -   dataset, array[0..N-1,0..NX+NY-1].
'                    one row corresponds to one point.
'                    first NX columns contain X-values, next NY (NY may be zero)
'                    columns may contain associated Y-values
'        Tags    -   tags, array[0..N-1], contains integer tags associated
'                    with points.
'        N       -   number of points, N>=0
'        NX      -   space dimension, NX>=1.
'        NY      -   number of optional Y-values, NY>=0.
'        NormType-   norm type:
'                    * 0 denotes infinity-norm
'                    * 1 denotes 1-norm
'                    * 2 denotes 2-norm (Euclidean norm)
'
'    OUTPUT PARAMETERS
'        KDT     -   KD-tree
'
'    NOTES
'
'    1. KD-tree  creation  have O(N*logN) complexity and O(N*(2*NX+NY))  memory
'       requirements.
'    2. Although KD-trees may be used with any combination of N  and  NX,  they
'       are more efficient than brute-force search only when N >> 4^NX. So they
'       are most useful in low-dimensional tasks (NX=2, NX=3). NX=1  is another
'       inefficient case, because  simple  binary  search  (without  additional
'       structures) is much more efficient in such tasks than KD-trees.
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreebuildtagged(xy As Double(,), tags As Integer(), n As Integer, nx As Integer, ny As Integer, normtype As Integer, _
		ByRef kdt As kdtree)
		kdt = New kdtree()
		nearestneighbor.kdtreebuildtagged(xy, tags, n, nx, ny, normtype, _
			kdt.innerobj)
		Return
	End Sub
	Public Shared Sub kdtreebuildtagged(xy As Double(,), tags As Integer(), nx As Integer, ny As Integer, normtype As Integer, ByRef kdt As kdtree)
		Dim n As Integer
		If (ap.rows(xy) <> ap.len(tags)) Then
			Throw New alglibexception("Error while calling 'kdtreebuildtagged': looks like one of arguments has wrong size")
		End If
		kdt = New kdtree()
		n = ap.rows(xy)
		nearestneighbor.kdtreebuildtagged(xy, tags, n, nx, ny, normtype, _
			kdt.innerobj)

		Return
	End Sub

	'************************************************************************
'    K-NN query: K nearest neighbors
'
'    INPUT PARAMETERS
'        KDT         -   KD-tree
'        X           -   point, array[0..NX-1].
'        K           -   number of neighbors to return, K>=1
'        SelfMatch   -   whether self-matches are allowed:
'                        * if True, nearest neighbor may be the point itself
'                          (if it exists in original dataset)
'                        * if False, then only points with non-zero distance
'                          are returned
'                        * if not given, considered True
'
'    RESULT
'        number of actual neighbors found (either K or N, if K>N).
'
'    This  subroutine  performs  query  and  stores  its result in the internal
'    structures of the KD-tree. You can use  following  subroutines  to  obtain
'    these results:
'    * KDTreeQueryResultsX() to get X-values
'    * KDTreeQueryResultsXY() to get X- and Y-values
'    * KDTreeQueryResultsTags() to get tag values
'    * KDTreeQueryResultsDistances() to get distances
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function kdtreequeryknn(kdt As kdtree, x As Double(), k As Integer, selfmatch As Boolean) As Integer

		Dim result As Integer = nearestneighbor.kdtreequeryknn(kdt.innerobj, x, k, selfmatch)
		Return result
	End Function
	Public Shared Function kdtreequeryknn(kdt As kdtree, x As Double(), k As Integer) As Integer
		Dim selfmatch As Boolean


		selfmatch = True
		Dim result As Integer = nearestneighbor.kdtreequeryknn(kdt.innerobj, x, k, selfmatch)

		Return result
	End Function

	'************************************************************************
'    R-NN query: all points within R-sphere centered at X
'
'    INPUT PARAMETERS
'        KDT         -   KD-tree
'        X           -   point, array[0..NX-1].
'        R           -   radius of sphere (in corresponding norm), R>0
'        SelfMatch   -   whether self-matches are allowed:
'                        * if True, nearest neighbor may be the point itself
'                          (if it exists in original dataset)
'                        * if False, then only points with non-zero distance
'                          are returned
'                        * if not given, considered True
'
'    RESULT
'        number of neighbors found, >=0
'
'    This  subroutine  performs  query  and  stores  its result in the internal
'    structures of the KD-tree. You can use  following  subroutines  to  obtain
'    actual results:
'    * KDTreeQueryResultsX() to get X-values
'    * KDTreeQueryResultsXY() to get X- and Y-values
'    * KDTreeQueryResultsTags() to get tag values
'    * KDTreeQueryResultsDistances() to get distances
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function kdtreequeryrnn(kdt As kdtree, x As Double(), r As Double, selfmatch As Boolean) As Integer

		Dim result As Integer = nearestneighbor.kdtreequeryrnn(kdt.innerobj, x, r, selfmatch)
		Return result
	End Function
	Public Shared Function kdtreequeryrnn(kdt As kdtree, x As Double(), r As Double) As Integer
		Dim selfmatch As Boolean


		selfmatch = True
		Dim result As Integer = nearestneighbor.kdtreequeryrnn(kdt.innerobj, x, r, selfmatch)

		Return result
	End Function

	'************************************************************************
'    K-NN query: approximate K nearest neighbors
'
'    INPUT PARAMETERS
'        KDT         -   KD-tree
'        X           -   point, array[0..NX-1].
'        K           -   number of neighbors to return, K>=1
'        SelfMatch   -   whether self-matches are allowed:
'                        * if True, nearest neighbor may be the point itself
'                          (if it exists in original dataset)
'                        * if False, then only points with non-zero distance
'                          are returned
'                        * if not given, considered True
'        Eps         -   approximation factor, Eps>=0. eps-approximate  nearest
'                        neighbor  is  a  neighbor  whose distance from X is at
'                        most (1+eps) times distance of true nearest neighbor.
'
'    RESULT
'        number of actual neighbors found (either K or N, if K>N).
'
'    NOTES
'        significant performance gain may be achieved only when Eps  is  is  on
'        the order of magnitude of 1 or larger.
'
'    This  subroutine  performs  query  and  stores  its result in the internal
'    structures of the KD-tree. You can use  following  subroutines  to  obtain
'    these results:
'    * KDTreeQueryResultsX() to get X-values
'    * KDTreeQueryResultsXY() to get X- and Y-values
'    * KDTreeQueryResultsTags() to get tag values
'    * KDTreeQueryResultsDistances() to get distances
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function kdtreequeryaknn(kdt As kdtree, x As Double(), k As Integer, selfmatch As Boolean, eps As Double) As Integer

		Dim result As Integer = nearestneighbor.kdtreequeryaknn(kdt.innerobj, x, k, selfmatch, eps)
		Return result
	End Function
	Public Shared Function kdtreequeryaknn(kdt As kdtree, x As Double(), k As Integer, eps As Double) As Integer
		Dim selfmatch As Boolean


		selfmatch = True
		Dim result As Integer = nearestneighbor.kdtreequeryaknn(kdt.innerobj, x, k, selfmatch, eps)

		Return result
	End Function

	'************************************************************************
'    X-values from last query
'
'    INPUT PARAMETERS
'        KDT     -   KD-tree
'        X       -   possibly pre-allocated buffer. If X is too small to store
'                    result, it is resized. If size(X) is enough to store
'                    result, it is left unchanged.
'
'    OUTPUT PARAMETERS
'        X       -   rows are filled with X-values
'
'    NOTES
'    1. points are ordered by distance from the query point (first = closest)
'    2. if  XY is larger than required to store result, only leading part  will
'       be overwritten; trailing part will be left unchanged. So  if  on  input
'       XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
'       XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
'       you want function  to  resize  array  according  to  result  size,  use
'       function with same name and suffix 'I'.
'
'    SEE ALSO
'    * KDTreeQueryResultsXY()            X- and Y-values
'    * KDTreeQueryResultsTags()          tag values
'    * KDTreeQueryResultsDistances()     distances
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreequeryresultsx(kdt As kdtree, ByRef x As Double(,))

		nearestneighbor.kdtreequeryresultsx(kdt.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    X- and Y-values from last query
'
'    INPUT PARAMETERS
'        KDT     -   KD-tree
'        XY      -   possibly pre-allocated buffer. If XY is too small to store
'                    result, it is resized. If size(XY) is enough to store
'                    result, it is left unchanged.
'
'    OUTPUT PARAMETERS
'        XY      -   rows are filled with points: first NX columns with
'                    X-values, next NY columns - with Y-values.
'
'    NOTES
'    1. points are ordered by distance from the query point (first = closest)
'    2. if  XY is larger than required to store result, only leading part  will
'       be overwritten; trailing part will be left unchanged. So  if  on  input
'       XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
'       XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
'       you want function  to  resize  array  according  to  result  size,  use
'       function with same name and suffix 'I'.
'
'    SEE ALSO
'    * KDTreeQueryResultsX()             X-values
'    * KDTreeQueryResultsTags()          tag values
'    * KDTreeQueryResultsDistances()     distances
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreequeryresultsxy(kdt As kdtree, ByRef xy As Double(,))

		nearestneighbor.kdtreequeryresultsxy(kdt.innerobj, xy)
		Return
	End Sub

	'************************************************************************
'    Tags from last query
'
'    INPUT PARAMETERS
'        KDT     -   KD-tree
'        Tags    -   possibly pre-allocated buffer. If X is too small to store
'                    result, it is resized. If size(X) is enough to store
'                    result, it is left unchanged.
'
'    OUTPUT PARAMETERS
'        Tags    -   filled with tags associated with points,
'                    or, when no tags were supplied, with zeros
'
'    NOTES
'    1. points are ordered by distance from the query point (first = closest)
'    2. if  XY is larger than required to store result, only leading part  will
'       be overwritten; trailing part will be left unchanged. So  if  on  input
'       XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
'       XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
'       you want function  to  resize  array  according  to  result  size,  use
'       function with same name and suffix 'I'.
'
'    SEE ALSO
'    * KDTreeQueryResultsX()             X-values
'    * KDTreeQueryResultsXY()            X- and Y-values
'    * KDTreeQueryResultsDistances()     distances
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreequeryresultstags(kdt As kdtree, ByRef tags As Integer())

		nearestneighbor.kdtreequeryresultstags(kdt.innerobj, tags)
		Return
	End Sub

	'************************************************************************
'    Distances from last query
'
'    INPUT PARAMETERS
'        KDT     -   KD-tree
'        R       -   possibly pre-allocated buffer. If X is too small to store
'                    result, it is resized. If size(X) is enough to store
'                    result, it is left unchanged.
'
'    OUTPUT PARAMETERS
'        R       -   filled with distances (in corresponding norm)
'
'    NOTES
'    1. points are ordered by distance from the query point (first = closest)
'    2. if  XY is larger than required to store result, only leading part  will
'       be overwritten; trailing part will be left unchanged. So  if  on  input
'       XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
'       XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
'       you want function  to  resize  array  according  to  result  size,  use
'       function with same name and suffix 'I'.
'
'    SEE ALSO
'    * KDTreeQueryResultsX()             X-values
'    * KDTreeQueryResultsXY()            X- and Y-values
'    * KDTreeQueryResultsTags()          tag values
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreequeryresultsdistances(kdt As kdtree, ByRef r As Double())

		nearestneighbor.kdtreequeryresultsdistances(kdt.innerobj, r)
		Return
	End Sub

	'************************************************************************
'    X-values from last query; 'interactive' variant for languages like  Python
'    which   support    constructs   like  "X = KDTreeQueryResultsXI(KDT)"  and
'    interactive mode of interpreter.
'
'    This function allocates new array on each call,  so  it  is  significantly
'    slower than its 'non-interactive' counterpart, but it is  more  convenient
'    when you call it from command line.
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreequeryresultsxi(kdt As kdtree, ByRef x As Double(,))
		x = New Double(-1, -1) {}
		nearestneighbor.kdtreequeryresultsxi(kdt.innerobj, x)
		Return
	End Sub

	'************************************************************************
'    XY-values from last query; 'interactive' variant for languages like Python
'    which   support    constructs   like "XY = KDTreeQueryResultsXYI(KDT)" and
'    interactive mode of interpreter.
'
'    This function allocates new array on each call,  so  it  is  significantly
'    slower than its 'non-interactive' counterpart, but it is  more  convenient
'    when you call it from command line.
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreequeryresultsxyi(kdt As kdtree, ByRef xy As Double(,))
		xy = New Double(-1, -1) {}
		nearestneighbor.kdtreequeryresultsxyi(kdt.innerobj, xy)
		Return
	End Sub

	'************************************************************************
'    Tags  from  last  query;  'interactive' variant for languages like  Python
'    which  support  constructs  like "Tags = KDTreeQueryResultsTagsI(KDT)" and
'    interactive mode of interpreter.
'
'    This function allocates new array on each call,  so  it  is  significantly
'    slower than its 'non-interactive' counterpart, but it is  more  convenient
'    when you call it from command line.
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreequeryresultstagsi(kdt As kdtree, ByRef tags As Integer())
		tags = New Integer(-1) {}
		nearestneighbor.kdtreequeryresultstagsi(kdt.innerobj, tags)
		Return
	End Sub

	'************************************************************************
'    Distances from last query; 'interactive' variant for languages like Python
'    which  support  constructs   like  "R = KDTreeQueryResultsDistancesI(KDT)"
'    and interactive mode of interpreter.
'
'    This function allocates new array on each call,  so  it  is  significantly
'    slower than its 'non-interactive' counterpart, but it is  more  convenient
'    when you call it from command line.
'
'      -- ALGLIB --
'         Copyright 28.02.2010 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub kdtreequeryresultsdistancesi(kdt As kdtree, ByRef r As Double())
		r = New Double(-1) {}
		nearestneighbor.kdtreequeryresultsdistancesi(kdt.innerobj, r)
		Return
	End Sub

End Class
Public Partial Class alglib


	'************************************************************************
'
'    ************************************************************************

	Public Class xdebugrecord1
		Inherits alglibobject
		'
		' Public declarations
		'
		Public Property i() As Integer
			Get
				Return _innerobj.i
			End Get
			Set
				_innerobj.i = value
			End Set
		End Property
		Public Property c() As complex
			Get
				Return _innerobj.c
			End Get
			Set
				_innerobj.c = value
			End Set
		End Property
		Public Property a() As Double()
			Get
				Return _innerobj.a
			End Get
			Set
				_innerobj.a = value
			End Set
		End Property

		Public Sub New()
			_innerobj = New xdebug.xdebugrecord1()
		End Sub

		Public Overrides Function make_copy() As alglib.alglibobject
			Return New xdebugrecord1(DirectCast(_innerobj.make_copy(), xdebug.xdebugrecord1))
		End Function

		'
		' Although some of declarations below are public, you should not use them
		' They are intended for internal use only
		'
		Private _innerobj As xdebug.xdebugrecord1
		Public ReadOnly Property innerobj() As xdebug.xdebugrecord1
			Get
				Return _innerobj
			End Get
		End Property
		Public Sub New(obj As xdebug.xdebugrecord1)
			_innerobj = obj
		End Sub
	End Class

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Creates and returns XDebugRecord1 structure:
'    * integer and complex fields of Rec1 are set to 1 and 1+i correspondingly
'    * array field of Rec1 is set to [2,3]
'
'      -- ALGLIB --
'         Copyright 27.05.2014 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebuginitrecord1(ByRef rec1 As xdebugrecord1)
		rec1 = New xdebugrecord1()
		xdebug.xdebuginitrecord1(rec1.innerobj)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Counts number of True values in the boolean 1D array.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function xdebugb1count(a As Boolean()) As Integer

		Dim result As Integer = xdebug.xdebugb1count(a)
		Return result
	End Function

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Replace all values in array by NOT(a[i]).
'    Array is passed using "shared" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugb1not(ByRef a As Boolean())

		xdebug.xdebugb1not(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Appends copy of array to itself.
'    Array is passed using "var" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugb1appendcopy(ByRef a As Boolean())

		xdebug.xdebugb1appendcopy(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Generate N-element array with even-numbered elements set to True.
'    Array is passed using "out" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugb1outeven(n As Integer, ByRef a As Boolean())
		a = New Boolean(-1) {}
		xdebug.xdebugb1outeven(n, a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Returns sum of elements in the array.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function xdebugi1sum(a As Integer()) As Integer

		Dim result As Integer = xdebug.xdebugi1sum(a)
		Return result
	End Function

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Replace all values in array by -A[I]
'    Array is passed using "shared" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugi1neg(ByRef a As Integer())

		xdebug.xdebugi1neg(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Appends copy of array to itself.
'    Array is passed using "var" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugi1appendcopy(ByRef a As Integer())

		xdebug.xdebugi1appendcopy(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Generate N-element array with even-numbered A[I] set to I, and odd-numbered
'    ones set to 0.
'
'    Array is passed using "out" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugi1outeven(n As Integer, ByRef a As Integer())
		a = New Integer(-1) {}
		xdebug.xdebugi1outeven(n, a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Returns sum of elements in the array.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function xdebugr1sum(a As Double()) As Double

		Dim result As Double = xdebug.xdebugr1sum(a)
		Return result
	End Function

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Replace all values in array by -A[I]
'    Array is passed using "shared" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugr1neg(ByRef a As Double())

		xdebug.xdebugr1neg(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Appends copy of array to itself.
'    Array is passed using "var" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugr1appendcopy(ByRef a As Double())

		xdebug.xdebugr1appendcopy(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Generate N-element array with even-numbered A[I] set to I*0.25,
'    and odd-numbered ones are set to 0.
'
'    Array is passed using "out" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugr1outeven(n As Integer, ByRef a As Double())
		a = New Double(-1) {}
		xdebug.xdebugr1outeven(n, a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Returns sum of elements in the array.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function xdebugc1sum(a As complex()) As complex

		Dim result As complex = xdebug.xdebugc1sum(a)
		Return result
	End Function

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Replace all values in array by -A[I]
'    Array is passed using "shared" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugc1neg(ByRef a As complex())

		xdebug.xdebugc1neg(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Appends copy of array to itself.
'    Array is passed using "var" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugc1appendcopy(ByRef a As complex())

		xdebug.xdebugc1appendcopy(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Generate N-element array with even-numbered A[K] set to (x,y) = (K*0.25, K*0.125)
'    and odd-numbered ones are set to 0.
'
'    Array is passed using "out" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugc1outeven(n As Integer, ByRef a As complex())
		a = New complex(-1) {}
		xdebug.xdebugc1outeven(n, a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Counts number of True values in the boolean 2D array.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function xdebugb2count(a As Boolean(,)) As Integer

		Dim result As Integer = xdebug.xdebugb2count(a)
		Return result
	End Function

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Replace all values in array by NOT(a[i]).
'    Array is passed using "shared" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugb2not(ByRef a As Boolean(,))

		xdebug.xdebugb2not(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Transposes array.
'    Array is passed using "var" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugb2transpose(ByRef a As Boolean(,))

		xdebug.xdebugb2transpose(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Generate MxN matrix with elements set to "Sin(3*I+5*J)>0"
'    Array is passed using "out" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugb2outsin(m As Integer, n As Integer, ByRef a As Boolean(,))
		a = New Boolean(-1, -1) {}
		xdebug.xdebugb2outsin(m, n, a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Returns sum of elements in the array.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function xdebugi2sum(a As Integer(,)) As Integer

		Dim result As Integer = xdebug.xdebugi2sum(a)
		Return result
	End Function

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Replace all values in array by -a[i,j]
'    Array is passed using "shared" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugi2neg(ByRef a As Integer(,))

		xdebug.xdebugi2neg(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Transposes array.
'    Array is passed using "var" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugi2transpose(ByRef a As Integer(,))

		xdebug.xdebugi2transpose(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Generate MxN matrix with elements set to "Sign(Sin(3*I+5*J))"
'    Array is passed using "out" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugi2outsin(m As Integer, n As Integer, ByRef a As Integer(,))
		a = New Integer(-1, -1) {}
		xdebug.xdebugi2outsin(m, n, a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Returns sum of elements in the array.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function xdebugr2sum(a As Double(,)) As Double

		Dim result As Double = xdebug.xdebugr2sum(a)
		Return result
	End Function

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Replace all values in array by -a[i,j]
'    Array is passed using "shared" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugr2neg(ByRef a As Double(,))

		xdebug.xdebugr2neg(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Transposes array.
'    Array is passed using "var" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugr2transpose(ByRef a As Double(,))

		xdebug.xdebugr2transpose(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Generate MxN matrix with elements set to "Sin(3*I+5*J)"
'    Array is passed using "out" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugr2outsin(m As Integer, n As Integer, ByRef a As Double(,))
		a = New Double(-1, -1) {}
		xdebug.xdebugr2outsin(m, n, a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Returns sum of elements in the array.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function xdebugc2sum(a As complex(,)) As complex

		Dim result As complex = xdebug.xdebugc2sum(a)
		Return result
	End Function

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Replace all values in array by -a[i,j]
'    Array is passed using "shared" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugc2neg(ByRef a As complex(,))

		xdebug.xdebugc2neg(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Transposes array.
'    Array is passed using "var" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugc2transpose(ByRef a As complex(,))

		xdebug.xdebugc2transpose(a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Generate MxN matrix with elements set to "Sin(3*I+5*J),Cos(3*I+5*J)"
'    Array is passed using "out" convention.
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Sub xdebugc2outsincos(m As Integer, n As Integer, ByRef a As complex(,))
		a = New complex(-1, -1) {}
		xdebug.xdebugc2outsincos(m, n, a)
		Return
	End Sub

	'************************************************************************
'    This is debug function intended for testing ALGLIB interface generator.
'    Never use it in any real life project.
'
'    Returns sum of a[i,j]*(1+b[i,j]) such that c[i,j] is True
'
'      -- ALGLIB --
'         Copyright 11.10.2013 by Bochkanov Sergey
'    ************************************************************************

	Public Shared Function xdebugmaskedbiasedproductsum(m As Integer, n As Integer, a As Double(,), b As Double(,), c As Boolean(,)) As Double

		Dim result As Double = xdebug.xdebugmaskedbiasedproductsum(m, n, a, b, c)
		Return result
	End Function

End Class
Public Partial Class alglib
	Public Class hqrnd
		'************************************************************************
'        Portable high quality random number generator state.
'        Initialized with HQRNDRandomize() or HQRNDSeed().
'
'        Fields:
'            S1, S2      -   seed values
'            V           -   precomputed value
'            MagicV      -   'magic' value used to determine whether State structure
'                            was correctly initialized.
'        ************************************************************************

		Public Class hqrndstate
			Inherits apobject
			Public s1 As Integer
			Public s2 As Integer
			Public magicv As Integer
			Public Sub New()
				init()
			End Sub
			Public Overrides Sub init()
			End Sub
			Public Overrides Function make_copy() As alglib.apobject
				Dim _result As New hqrndstate()
				_result.s1 = s1
				_result.s2 = s2
				_result.magicv = magicv
				Return _result
			End Function
		End Class




		Public Const hqrndmax As Integer = 2147483561
		Public Const hqrndm1 As Integer = 2147483563
		Public Const hqrndm2 As Integer = 2147483399
		Public Const hqrndmagic As Integer = 1634357784


		'************************************************************************
'        HQRNDState  initialization  with  random  values  which come from standard
'        RNG.
'
'          -- ALGLIB --
'             Copyright 02.12.2009 by Bochkanov Sergey
'        ************************************************************************

		Public Shared Sub hqrndrandomize(state As hqrndstate)
			Dim s0 As Integer = 0
			Dim s1 As Integer = 0

            s0 = Math.randominteger(hqrndm1)
            s1 = Math.randominteger(hqrndm2)
            hqrndseed(s0, s1, state)
        End Sub


        '************************************************************************
        '        HQRNDState initialization with seed values
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub hqrndseed(s1 As Integer, s2 As Integer, state As hqrndstate)

            '
            ' Protection against negative seeds:
            '
            '     SEED := -(SEED+1)
            '
            ' We can use just "-SEED" because there exists such integer number  N
            ' that N<0, -N=N<0 too. (This number is equal to 0x800...000).   Need
            ' to handle such seed correctly forces us to use  a  bit  complicated
            ' formula.
            '
            If s1 < 0 Then
                s1 = -(s1 + 1)
            End If
            If s2 < 0 Then
                s2 = -(s2 + 1)
            End If
            state.s1 = s1 Mod (hqrndm1 - 1) + 1
            state.s2 = s2 Mod (hqrndm2 - 1) + 1
            state.magicv = hqrndmagic
        End Sub


        '************************************************************************
        '        This function generates random real number in (0,1),
        '        not including interval boundaries
        '
        '        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function hqrnduniformr(state As hqrndstate) As Double
            Dim result As Double = 0

            result = CDbl(hqrndintegerbase(state) + 1) / CDbl(hqrndmax + 2)
            Return result
        End Function


        '************************************************************************
        '        This function generates random integer number in [0, N)
        '
        '        1. State structure must be initialized with HQRNDRandomize() or HQRNDSeed()
        '        2. N can be any positive number except for very large numbers:
        '           * close to 2^31 on 32-bit systems
        '           * close to 2^62 on 64-bit systems
        '           An exception will be generated if N is too large.
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function hqrnduniformi(state As hqrndstate, n As Integer) As Integer
            Dim result As Integer = 0
            Dim maxcnt As Integer = 0
            Dim mx As Integer = 0
            Dim a As Integer = 0
            Dim b As Integer = 0

            alglib.ap.assert(n > 0, "HQRNDUniformI: N<=0!")
            maxcnt = hqrndmax + 1

            '
            ' Two branches: one for N<=MaxCnt, another for N>MaxCnt.
            '
            If n > maxcnt Then

                '
                ' N>=MaxCnt.
                '
                ' We have two options here:
                ' a) N is exactly divisible by MaxCnt
                ' b) N is not divisible by MaxCnt
                '
                ' In both cases we reduce problem on interval spanning [0,N)
                ' to several subproblems on intervals spanning [0,MaxCnt).
                '
                If n Mod maxcnt = 0 Then

                    '
                    ' N is exactly divisible by MaxCnt.
                    '
                    ' [0,N) range is dividided into N/MaxCnt bins,
                    ' each of them having length equal to MaxCnt.
                    '
                    ' We generate:
                    ' * random bin number B
                    ' * random offset within bin A
                    ' Both random numbers are generated by recursively
                    ' calling HQRNDUniformI().
                    '
                    ' Result is equal to A+MaxCnt*B.
                    '
                    alglib.ap.assert(n \ maxcnt <= maxcnt, "HQRNDUniformI: N is too large")
                    a = hqrnduniformi(state, maxcnt)
                    b = hqrnduniformi(state, n \ maxcnt)
                    result = a + maxcnt * b
                Else

                    '
                    ' N is NOT exactly divisible by MaxCnt.
                    '
                    ' [0,N) range is dividided into Ceil(N/MaxCnt) bins,
                    ' each of them having length equal to MaxCnt.
                    '
                    ' We generate:
                    ' * random bin number B in [0, Ceil(N/MaxCnt)-1]
                    ' * random offset within bin A
                    ' * if both of what is below is true
                    '   1) bin number B is that of the last bin
                    '   2) A >= N mod MaxCnt
                    '   then we repeat generation of A/B.
                    '   This stage is essential in order to avoid bias in the result.
                    ' * otherwise, we return A*MaxCnt+N
                    '
                    alglib.ap.assert(n \ maxcnt + 1 <= maxcnt, "HQRNDUniformI: N is too large")
                    result = -1
                    Do
                        a = hqrnduniformi(state, maxcnt)
                        b = hqrnduniformi(state, n \ maxcnt + 1)
                        If b = n \ maxcnt AndAlso a >= n Mod maxcnt Then
                            Continue Do
                        End If
                        result = a + maxcnt * b
                    Loop While result < 0
                End If
            Else

                '
                ' N<=MaxCnt
                '
                ' Code below is a bit complicated because we can not simply
                ' return "HQRNDIntegerBase() mod N" - it will be skewed for
                ' large N's in [0.1*HQRNDMax...HQRNDMax].
                '
                mx = maxcnt - maxcnt Mod n
                Do
                    result = hqrndintegerbase(state)
                Loop While result >= mx
                result = result Mod n
            End If
            Return result
        End Function


        '************************************************************************
        '        Random number generator: normal numbers
        '
        '        This function generates one random number from normal distribution.
        '        Its performance is equal to that of HQRNDNormal2()
        '
        '        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function hqrndnormal(state As hqrndstate) As Double
            Dim result As Double = 0
            Dim v1 As Double = 0
            Dim v2 As Double = 0

            hqrndnormal2(state, v1, v2)
            result = v1
            Return result
        End Function


        '************************************************************************
        '        Random number generator: random X and Y such that X^2+Y^2=1
        '
        '        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub hqrndunit2(state As hqrndstate, ByRef x As Double, ByRef y As Double)
            Dim v As Double = 0
            Dim mx As Double = 0
            Dim mn As Double = 0

            x = 0
            y = 0

            Do
                hqrndnormal2(state, x, y)
            Loop While Not (CDbl(x) <> CDbl(0) OrElse CDbl(y) <> CDbl(0))
            mx = System.Math.Max(System.Math.Abs(x), System.Math.Abs(y))
            mn = System.Math.Min(System.Math.Abs(x), System.Math.Abs(y))
            v = mx * System.Math.sqrt(1 + Math.sqr(mn / mx))
            x = x / v
            y = y / v
        End Sub


        '************************************************************************
        '        Random number generator: normal numbers
        '
        '        This function generates two independent random numbers from normal
        '        distribution. Its performance is equal to that of HQRNDNormal()
        '
        '        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
        '
        '          -- ALGLIB --
        '             Copyright 02.12.2009 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub hqrndnormal2(state As hqrndstate, ByRef x1 As Double, ByRef x2 As Double)
            Dim u As Double = 0
            Dim v As Double = 0
            Dim s As Double = 0

            x1 = 0
            x2 = 0

            While True
                u = 2 * hqrnduniformr(state) - 1
                v = 2 * hqrnduniformr(state) - 1
                s = Math.sqr(u) + Math.sqr(v)
                If CDbl(s) > CDbl(0) AndAlso CDbl(s) < CDbl(1) Then

                    '
                    ' two Sqrt's instead of one to
                    ' avoid overflow when S is too small
                    '
                    s = System.Math.sqrt(-(2 * System.Math.Log(s))) / System.Math.sqrt(s)
                    x1 = u * s
                    x2 = v * s
                    Return
                End If
            End While
        End Sub


        '************************************************************************
        '        Random number generator: exponential distribution
        '
        '        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().
        '
        '          -- ALGLIB --
        '             Copyright 11.08.2007 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function hqrndexponential(state As hqrndstate, lambdav As Double) As Double
            Dim result As Double = 0

            alglib.ap.assert(CDbl(lambdav) > CDbl(0), "HQRNDExponential: LambdaV<=0!")
            result = -(System.Math.Log(hqrnduniformr(state)) / lambdav)
            Return result
        End Function


        '************************************************************************
        '        This function generates  random number from discrete distribution given by
        '        finite sample X.
        '
        '        INPUT PARAMETERS
        '            State   -   high quality random number generator, must be
        '                        initialized with HQRNDRandomize() or HQRNDSeed().
        '                X   -   finite sample
        '                N   -   number of elements to use, N>=1
        '
        '        RESULT
        '            this function returns one of the X[i] for random i=0..N-1
        '
        '          -- ALGLIB --
        '             Copyright 08.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function hqrnddiscrete(state As hqrndstate, x As Double(), n As Integer) As Double
            Dim result As Double = 0

            alglib.ap.assert(n > 0, "HQRNDDiscrete: N<=0")
            alglib.ap.assert(n <= alglib.ap.len(x), "HQRNDDiscrete: Length(X)<N")
            result = x(hqrnduniformi(state, n))
            Return result
        End Function


        '************************************************************************
        '        This function generates random number from continuous  distribution  given
        '        by finite sample X.
        '
        '        INPUT PARAMETERS
        '            State   -   high quality random number generator, must be
        '                        initialized with HQRNDRandomize() or HQRNDSeed().
        '                X   -   finite sample, array[N] (can be larger, in this  case only
        '                        leading N elements are used). THIS ARRAY MUST BE SORTED BY
        '                        ASCENDING.
        '                N   -   number of elements to use, N>=1
        '
        '        RESULT
        '            this function returns random number from continuous distribution which  
        '            tries to approximate X as mush as possible. min(X)<=Result<=max(X).
        '
        '          -- ALGLIB --
        '             Copyright 08.11.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function hqrndcontinuous(state As hqrndstate, x As Double(), n As Integer) As Double
            Dim result As Double = 0
            Dim mx As Double = 0
            Dim mn As Double = 0
            Dim i As Integer = 0

            alglib.ap.assert(n > 0, "HQRNDContinuous: N<=0")
            alglib.ap.assert(n <= alglib.ap.len(x), "HQRNDContinuous: Length(X)<N")
            If n = 1 Then
                result = x(0)
                Return result
            End If
            i = hqrnduniformi(state, n - 1)
            mn = x(i)
            mx = x(i + 1)
            alglib.ap.assert(CDbl(mx) >= CDbl(mn), "HQRNDDiscrete: X is not sorted by ascending")
            If CDbl(mx) <> CDbl(mn) Then
                result = (mx - mn) * hqrnduniformr(state) + mn
            Else
                result = mn
            End If
            Return result
        End Function


        '************************************************************************
        '        This function returns random integer in [0,HQRNDMax]
        '
        '        L'Ecuyer, Efficient and portable combined random number generators
        '        ************************************************************************

        Private Shared Function hqrndintegerbase(state As hqrndstate) As Integer
            Dim result As Integer = 0
            Dim k As Integer = 0

            alglib.ap.assert(state.magicv = hqrndmagic, "HQRNDIntegerBase: State is not correctly initialized!")
            k = state.s1 \ 53668
            state.s1 = 40014 * (state.s1 - k * 53668) - k * 12211
            If state.s1 < 0 Then
                state.s1 = state.s1 + 2147483563
            End If
            k = state.s2 \ 52774
            state.s2 = 40692 * (state.s2 - k * 52774) - k * 3791
            If state.s2 < 0 Then
                state.s2 = state.s2 + 2147483399
            End If

            '
            ' Result
            '
            result = state.s1 - state.s2
            If result < 1 Then
                result = result + 2147483562
            End If
            result = result - 1
            Return result
        End Function


    End Class
    Public Class nearestneighbor
        Public Class kdtree
            Inherits apobject
            Public n As Integer
            Public nx As Integer
            Public ny As Integer
            Public normtype As Integer
            Public xy As Double(,)
            Public tags As Integer()
            Public boxmin As Double()
            Public boxmax As Double()
            Public nodes As Integer()
            Public splits As Double()
            Public x As Double()
            Public kneeded As Integer
            Public rneeded As Double
            Public selfmatch As Boolean
            Public approxf As Double
            Public kcur As Integer
            Public idx As Integer()
            Public r As Double()
            Public buf As Double()
            Public curboxmin As Double()
            Public curboxmax As Double()
            Public curdist As Double
            Public debugcounter As Integer
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
                xy = New Double(-1, -1) {}
                tags = New Integer(-1) {}
                boxmin = New Double(-1) {}
                boxmax = New Double(-1) {}
                nodes = New Integer(-1) {}
                splits = New Double(-1) {}
                x = New Double(-1) {}
                idx = New Integer(-1) {}
                r = New Double(-1) {}
                buf = New Double(-1) {}
                curboxmin = New Double(-1) {}
                curboxmax = New Double(-1) {}
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New kdtree()
                _result.n = n
                _result.nx = nx
                _result.ny = ny
                _result.normtype = normtype
                _result.xy = DirectCast(xy.Clone(), Double(,))
                _result.tags = DirectCast(tags.Clone(), Integer())
                _result.boxmin = DirectCast(boxmin.Clone(), Double())
                _result.boxmax = DirectCast(boxmax.Clone(), Double())
                _result.nodes = DirectCast(nodes.Clone(), Integer())
                _result.splits = DirectCast(splits.Clone(), Double())
                _result.x = DirectCast(x.Clone(), Double())
                _result.kneeded = kneeded
                _result.rneeded = rneeded
                _result.selfmatch = selfmatch
                _result.approxf = approxf
                _result.kcur = kcur
                _result.idx = DirectCast(idx.Clone(), Integer())
                _result.r = DirectCast(r.Clone(), Double())
                _result.buf = DirectCast(buf.Clone(), Double())
                _result.curboxmin = DirectCast(curboxmin.Clone(), Double())
                _result.curboxmax = DirectCast(curboxmax.Clone(), Double())
                _result.curdist = curdist
                _result.debugcounter = debugcounter
                Return _result
            End Function
        End Class




        Public Const splitnodesize As Integer = 6
        Public Const kdtreefirstversion As Integer = 0


        '************************************************************************
        '        KD-tree creation
        '
        '        This subroutine creates KD-tree from set of X-values and optional Y-values
        '
        '        INPUT PARAMETERS
        '            XY      -   dataset, array[0..N-1,0..NX+NY-1].
        '                        one row corresponds to one point.
        '                        first NX columns contain X-values, next NY (NY may be zero)
        '                        columns may contain associated Y-values
        '            N       -   number of points, N>=0.
        '            NX      -   space dimension, NX>=1.
        '            NY      -   number of optional Y-values, NY>=0.
        '            NormType-   norm type:
        '                        * 0 denotes infinity-norm
        '                        * 1 denotes 1-norm
        '                        * 2 denotes 2-norm (Euclidean norm)
        '                        
        '        OUTPUT PARAMETERS
        '            KDT     -   KD-tree
        '            
        '            
        '        NOTES
        '
        '        1. KD-tree  creation  have O(N*logN) complexity and O(N*(2*NX+NY))  memory
        '           requirements.
        '        2. Although KD-trees may be used with any combination of N  and  NX,  they
        '           are more efficient than brute-force search only when N >> 4^NX. So they
        '           are most useful in low-dimensional tasks (NX=2, NX=3). NX=1  is another
        '           inefficient case, because  simple  binary  search  (without  additional
        '           structures) is much more efficient in such tasks than KD-trees.
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreebuild(xy As Double(,), n As Integer, nx As Integer, ny As Integer, normtype As Integer, kdt As kdtree)
            Dim tags As Integer() = New Integer(-1) {}
            Dim i As Integer = 0

            alglib.ap.assert(n >= 0, "KDTreeBuild: N<0")
            alglib.ap.assert(nx >= 1, "KDTreeBuild: NX<1")
            alglib.ap.assert(ny >= 0, "KDTreeBuild: NY<0")
            alglib.ap.assert(normtype >= 0 AndAlso normtype <= 2, "KDTreeBuild: incorrect NormType")
            alglib.ap.assert(alglib.ap.rows(xy) >= n, "KDTreeBuild: rows(X)<N")
            alglib.ap.assert(alglib.ap.cols(xy) >= nx + ny OrElse n = 0, "KDTreeBuild: cols(X)<NX+NY")
            alglib.ap.assert(apserv.apservisfinitematrix(xy, n, nx + ny), "KDTreeBuild: XY contains infinite or NaN values")
            If n > 0 Then
                tags = New Integer(n - 1) {}
                For i = 0 To n - 1
                    tags(i) = 0
                Next
            End If
            kdtreebuildtagged(xy, tags, n, nx, ny, normtype, _
                kdt)
        End Sub


        '************************************************************************
        '        KD-tree creation
        '
        '        This  subroutine  creates  KD-tree  from set of X-values, integer tags and
        '        optional Y-values
        '
        '        INPUT PARAMETERS
        '            XY      -   dataset, array[0..N-1,0..NX+NY-1].
        '                        one row corresponds to one point.
        '                        first NX columns contain X-values, next NY (NY may be zero)
        '                        columns may contain associated Y-values
        '            Tags    -   tags, array[0..N-1], contains integer tags associated
        '                        with points.
        '            N       -   number of points, N>=0
        '            NX      -   space dimension, NX>=1.
        '            NY      -   number of optional Y-values, NY>=0.
        '            NormType-   norm type:
        '                        * 0 denotes infinity-norm
        '                        * 1 denotes 1-norm
        '                        * 2 denotes 2-norm (Euclidean norm)
        '
        '        OUTPUT PARAMETERS
        '            KDT     -   KD-tree
        '
        '        NOTES
        '
        '        1. KD-tree  creation  have O(N*logN) complexity and O(N*(2*NX+NY))  memory
        '           requirements.
        '        2. Although KD-trees may be used with any combination of N  and  NX,  they
        '           are more efficient than brute-force search only when N >> 4^NX. So they
        '           are most useful in low-dimensional tasks (NX=2, NX=3). NX=1  is another
        '           inefficient case, because  simple  binary  search  (without  additional
        '           structures) is much more efficient in such tasks than KD-trees.
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreebuildtagged(xy As Double(,), tags As Integer(), n As Integer, nx As Integer, ny As Integer, normtype As Integer, _
            kdt As kdtree)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim maxnodes As Integer = 0
            Dim nodesoffs As Integer = 0
            Dim splitsoffs As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            alglib.ap.assert(n >= 0, "KDTreeBuildTagged: N<0")
            alglib.ap.assert(nx >= 1, "KDTreeBuildTagged: NX<1")
            alglib.ap.assert(ny >= 0, "KDTreeBuildTagged: NY<0")
            alglib.ap.assert(normtype >= 0 AndAlso normtype <= 2, "KDTreeBuildTagged: incorrect NormType")
            alglib.ap.assert(alglib.ap.rows(xy) >= n, "KDTreeBuildTagged: rows(X)<N")
            alglib.ap.assert(alglib.ap.cols(xy) >= nx + ny OrElse n = 0, "KDTreeBuildTagged: cols(X)<NX+NY")
            alglib.ap.assert(apserv.apservisfinitematrix(xy, n, nx + ny), "KDTreeBuildTagged: XY contains infinite or NaN values")

            '
            ' initialize
            '
            kdt.n = n
            kdt.nx = nx
            kdt.ny = ny
            kdt.normtype = normtype
            kdt.kcur = 0

            '
            ' N=0 => quick exit
            '
            If n = 0 Then
                Return
            End If

            '
            ' Allocate
            '
            kdtreeallocdatasetindependent(kdt, nx, ny)
            kdtreeallocdatasetdependent(kdt, n, nx, ny)

            '
            ' Initial fill
            '
            For i = 0 To n - 1
                For i_ = 0 To nx - 1
                    kdt.xy(i, i_) = xy(i, i_)
                Next
                i1_ = (0) - (nx)
                For i_ = nx To 2 * nx + ny - 1
                    kdt.xy(i, i_) = xy(i, i_ + i1_)
                Next
                kdt.tags(i) = tags(i)
            Next

            '
            ' Determine bounding box
            '
            For i_ = 0 To nx - 1
                kdt.boxmin(i_) = kdt.xy(0, i_)
            Next
            For i_ = 0 To nx - 1
                kdt.boxmax(i_) = kdt.xy(0, i_)
            Next
            For i = 1 To n - 1
                For j = 0 To nx - 1
                    kdt.boxmin(j) = System.Math.Min(kdt.boxmin(j), kdt.xy(i, j))
                    kdt.boxmax(j) = System.Math.Max(kdt.boxmax(j), kdt.xy(i, j))
                Next
            Next

            '
            ' prepare tree structure
            ' * MaxNodes=N because we guarantee no trivial splits, i.e.
            '   every split will generate two non-empty boxes
            '
            maxnodes = n
            kdt.nodes = New Integer(splitnodesize * 2 * maxnodes - 1) {}
            kdt.splits = New Double(2 * maxnodes - 1) {}
            nodesoffs = 0
            splitsoffs = 0
            For i_ = 0 To nx - 1
                kdt.curboxmin(i_) = kdt.boxmin(i_)
            Next
            For i_ = 0 To nx - 1
                kdt.curboxmax(i_) = kdt.boxmax(i_)
            Next
            kdtreegeneratetreerec(kdt, nodesoffs, splitsoffs, 0, n, 8)
        End Sub


        '************************************************************************
        '        K-NN query: K nearest neighbors
        '
        '        INPUT PARAMETERS
        '            KDT         -   KD-tree
        '            X           -   point, array[0..NX-1].
        '            K           -   number of neighbors to return, K>=1
        '            SelfMatch   -   whether self-matches are allowed:
        '                            * if True, nearest neighbor may be the point itself
        '                              (if it exists in original dataset)
        '                            * if False, then only points with non-zero distance
        '                              are returned
        '                            * if not given, considered True
        '
        '        RESULT
        '            number of actual neighbors found (either K or N, if K>N).
        '
        '        This  subroutine  performs  query  and  stores  its result in the internal
        '        structures of the KD-tree. You can use  following  subroutines  to  obtain
        '        these results:
        '        * KDTreeQueryResultsX() to get X-values
        '        * KDTreeQueryResultsXY() to get X- and Y-values
        '        * KDTreeQueryResultsTags() to get tag values
        '        * KDTreeQueryResultsDistances() to get distances
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function kdtreequeryknn(kdt As kdtree, x As Double(), k As Integer, selfmatch As Boolean) As Integer
            Dim result As Integer = 0

            alglib.ap.assert(k >= 1, "KDTreeQueryKNN: K<1!")
            alglib.ap.assert(alglib.ap.len(x) >= kdt.nx, "KDTreeQueryKNN: Length(X)<NX!")
            alglib.ap.assert(apserv.isfinitevector(x, kdt.nx), "KDTreeQueryKNN: X contains infinite or NaN values!")
            result = kdtreequeryaknn(kdt, x, k, selfmatch, 0.0)
            Return result
        End Function


        '************************************************************************
        '        R-NN query: all points within R-sphere centered at X
        '
        '        INPUT PARAMETERS
        '            KDT         -   KD-tree
        '            X           -   point, array[0..NX-1].
        '            R           -   radius of sphere (in corresponding norm), R>0
        '            SelfMatch   -   whether self-matches are allowed:
        '                            * if True, nearest neighbor may be the point itself
        '                              (if it exists in original dataset)
        '                            * if False, then only points with non-zero distance
        '                              are returned
        '                            * if not given, considered True
        '
        '        RESULT
        '            number of neighbors found, >=0
        '
        '        This  subroutine  performs  query  and  stores  its result in the internal
        '        structures of the KD-tree. You can use  following  subroutines  to  obtain
        '        actual results:
        '        * KDTreeQueryResultsX() to get X-values
        '        * KDTreeQueryResultsXY() to get X- and Y-values
        '        * KDTreeQueryResultsTags() to get tag values
        '        * KDTreeQueryResultsDistances() to get distances
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function kdtreequeryrnn(kdt As kdtree, x As Double(), r As Double, selfmatch As Boolean) As Integer
            Dim result As Integer = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            alglib.ap.assert(CDbl(r) > CDbl(0), "KDTreeQueryRNN: incorrect R!")
            alglib.ap.assert(alglib.ap.len(x) >= kdt.nx, "KDTreeQueryRNN: Length(X)<NX!")
            alglib.ap.assert(apserv.isfinitevector(x, kdt.nx), "KDTreeQueryRNN: X contains infinite or NaN values!")

            '
            ' Handle special case: KDT.N=0
            '
            If kdt.n = 0 Then
                kdt.kcur = 0
                result = 0
                Return result
            End If

            '
            ' Prepare parameters
            '
            kdt.kneeded = 0
            If kdt.normtype <> 2 Then
                kdt.rneeded = r
            Else
                kdt.rneeded = Math.sqr(r)
            End If
            kdt.selfmatch = selfmatch
            kdt.approxf = 1
            kdt.kcur = 0

            '
            ' calculate distance from point to current bounding box
            '
            kdtreeinitbox(kdt, x)

            '
            ' call recursive search
            ' results are returned as heap
            '
            kdtreequerynnrec(kdt, 0)

            '
            ' pop from heap to generate ordered representation
            '
            ' last element is not pop'ed because it is already in
            ' its place
            '
            result = kdt.kcur
            j = kdt.kcur
            For i = kdt.kcur To 2 Step -1
                tsort.tagheappopi(kdt.r, kdt.idx, j)
            Next
            Return result
        End Function


        '************************************************************************
        '        K-NN query: approximate K nearest neighbors
        '
        '        INPUT PARAMETERS
        '            KDT         -   KD-tree
        '            X           -   point, array[0..NX-1].
        '            K           -   number of neighbors to return, K>=1
        '            SelfMatch   -   whether self-matches are allowed:
        '                            * if True, nearest neighbor may be the point itself
        '                              (if it exists in original dataset)
        '                            * if False, then only points with non-zero distance
        '                              are returned
        '                            * if not given, considered True
        '            Eps         -   approximation factor, Eps>=0. eps-approximate  nearest
        '                            neighbor  is  a  neighbor  whose distance from X is at
        '                            most (1+eps) times distance of true nearest neighbor.
        '
        '        RESULT
        '            number of actual neighbors found (either K or N, if K>N).
        '            
        '        NOTES
        '            significant performance gain may be achieved only when Eps  is  is  on
        '            the order of magnitude of 1 or larger.
        '
        '        This  subroutine  performs  query  and  stores  its result in the internal
        '        structures of the KD-tree. You can use  following  subroutines  to  obtain
        '        these results:
        '        * KDTreeQueryResultsX() to get X-values
        '        * KDTreeQueryResultsXY() to get X- and Y-values
        '        * KDTreeQueryResultsTags() to get tag values
        '        * KDTreeQueryResultsDistances() to get distances
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function kdtreequeryaknn(kdt As kdtree, x As Double(), k As Integer, selfmatch As Boolean, eps As Double) As Integer
            Dim result As Integer = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            alglib.ap.assert(k > 0, "KDTreeQueryAKNN: incorrect K!")
            alglib.ap.assert(CDbl(eps) >= CDbl(0), "KDTreeQueryAKNN: incorrect Eps!")
            alglib.ap.assert(alglib.ap.len(x) >= kdt.nx, "KDTreeQueryAKNN: Length(X)<NX!")
            alglib.ap.assert(apserv.isfinitevector(x, kdt.nx), "KDTreeQueryAKNN: X contains infinite or NaN values!")

            '
            ' Handle special case: KDT.N=0
            '
            If kdt.n = 0 Then
                kdt.kcur = 0
                result = 0
                Return result
            End If

            '
            ' Prepare parameters
            '
            k = System.Math.Min(k, kdt.n)
            kdt.kneeded = k
            kdt.rneeded = 0
            kdt.selfmatch = selfmatch
            If kdt.normtype = 2 Then
                kdt.approxf = 1 / Math.sqr(1 + eps)
            Else
                kdt.approxf = 1 / (1 + eps)
            End If
            kdt.kcur = 0

            '
            ' calculate distance from point to current bounding box
            '
            kdtreeinitbox(kdt, x)

            '
            ' call recursive search
            ' results are returned as heap
            '
            kdtreequerynnrec(kdt, 0)

            '
            ' pop from heap to generate ordered representation
            '
            ' last element is non pop'ed because it is already in
            ' its place
            '
            result = kdt.kcur
            j = kdt.kcur
            For i = kdt.kcur To 2 Step -1
                tsort.tagheappopi(kdt.r, kdt.idx, j)
            Next
            Return result
        End Function


        '************************************************************************
        '        X-values from last query
        '
        '        INPUT PARAMETERS
        '            KDT     -   KD-tree
        '            X       -   possibly pre-allocated buffer. If X is too small to store
        '                        result, it is resized. If size(X) is enough to store
        '                        result, it is left unchanged.
        '
        '        OUTPUT PARAMETERS
        '            X       -   rows are filled with X-values
        '
        '        NOTES
        '        1. points are ordered by distance from the query point (first = closest)
        '        2. if  XY is larger than required to store result, only leading part  will
        '           be overwritten; trailing part will be left unchanged. So  if  on  input
        '           XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
        '           XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
        '           you want function  to  resize  array  according  to  result  size,  use
        '           function with same name and suffix 'I'.
        '
        '        SEE ALSO
        '        * KDTreeQueryResultsXY()            X- and Y-values
        '        * KDTreeQueryResultsTags()          tag values
        '        * KDTreeQueryResultsDistances()     distances
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreequeryresultsx(kdt As kdtree, ByRef x As Double(,))
            Dim i As Integer = 0
            Dim k As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If kdt.kcur = 0 Then
                Return
            End If
            If alglib.ap.rows(x) < kdt.kcur OrElse alglib.ap.cols(x) < kdt.nx Then
                x = New Double(kdt.kcur - 1, kdt.nx - 1) {}
            End If
            k = kdt.kcur
            For i = 0 To k - 1
                i1_ = (kdt.nx) - (0)
                For i_ = 0 To kdt.nx - 1
                    x(i, i_) = kdt.xy(kdt.idx(i), i_ + i1_)
                Next
            Next
        End Sub


        '************************************************************************
        '        X- and Y-values from last query
        '
        '        INPUT PARAMETERS
        '            KDT     -   KD-tree
        '            XY      -   possibly pre-allocated buffer. If XY is too small to store
        '                        result, it is resized. If size(XY) is enough to store
        '                        result, it is left unchanged.
        '
        '        OUTPUT PARAMETERS
        '            XY      -   rows are filled with points: first NX columns with
        '                        X-values, next NY columns - with Y-values.
        '
        '        NOTES
        '        1. points are ordered by distance from the query point (first = closest)
        '        2. if  XY is larger than required to store result, only leading part  will
        '           be overwritten; trailing part will be left unchanged. So  if  on  input
        '           XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
        '           XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
        '           you want function  to  resize  array  according  to  result  size,  use
        '           function with same name and suffix 'I'.
        '
        '        SEE ALSO
        '        * KDTreeQueryResultsX()             X-values
        '        * KDTreeQueryResultsTags()          tag values
        '        * KDTreeQueryResultsDistances()     distances
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreequeryresultsxy(kdt As kdtree, ByRef xy As Double(,))
            Dim i As Integer = 0
            Dim k As Integer = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            If kdt.kcur = 0 Then
                Return
            End If
            If alglib.ap.rows(xy) < kdt.kcur OrElse alglib.ap.cols(xy) < kdt.nx + kdt.ny Then
                xy = New Double(kdt.kcur - 1, kdt.nx + (kdt.ny - 1)) {}
            End If
            k = kdt.kcur
            For i = 0 To k - 1
                i1_ = (kdt.nx) - (0)
                For i_ = 0 To kdt.nx + kdt.ny - 1
                    xy(i, i_) = kdt.xy(kdt.idx(i), i_ + i1_)
                Next
            Next
        End Sub


        '************************************************************************
        '        Tags from last query
        '
        '        INPUT PARAMETERS
        '            KDT     -   KD-tree
        '            Tags    -   possibly pre-allocated buffer. If X is too small to store
        '                        result, it is resized. If size(X) is enough to store
        '                        result, it is left unchanged.
        '
        '        OUTPUT PARAMETERS
        '            Tags    -   filled with tags associated with points,
        '                        or, when no tags were supplied, with zeros
        '
        '        NOTES
        '        1. points are ordered by distance from the query point (first = closest)
        '        2. if  XY is larger than required to store result, only leading part  will
        '           be overwritten; trailing part will be left unchanged. So  if  on  input
        '           XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
        '           XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
        '           you want function  to  resize  array  according  to  result  size,  use
        '           function with same name and suffix 'I'.
        '
        '        SEE ALSO
        '        * KDTreeQueryResultsX()             X-values
        '        * KDTreeQueryResultsXY()            X- and Y-values
        '        * KDTreeQueryResultsDistances()     distances
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreequeryresultstags(kdt As kdtree, ByRef tags As Integer())
            Dim i As Integer = 0
            Dim k As Integer = 0

            If kdt.kcur = 0 Then
                Return
            End If
            If alglib.ap.len(tags) < kdt.kcur Then
                tags = New Integer(kdt.kcur - 1) {}
            End If
            k = kdt.kcur
            For i = 0 To k - 1
                tags(i) = kdt.tags(kdt.idx(i))
            Next
        End Sub


        '************************************************************************
        '        Distances from last query
        '
        '        INPUT PARAMETERS
        '            KDT     -   KD-tree
        '            R       -   possibly pre-allocated buffer. If X is too small to store
        '                        result, it is resized. If size(X) is enough to store
        '                        result, it is left unchanged.
        '
        '        OUTPUT PARAMETERS
        '            R       -   filled with distances (in corresponding norm)
        '
        '        NOTES
        '        1. points are ordered by distance from the query point (first = closest)
        '        2. if  XY is larger than required to store result, only leading part  will
        '           be overwritten; trailing part will be left unchanged. So  if  on  input
        '           XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
        '           XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
        '           you want function  to  resize  array  according  to  result  size,  use
        '           function with same name and suffix 'I'.
        '
        '        SEE ALSO
        '        * KDTreeQueryResultsX()             X-values
        '        * KDTreeQueryResultsXY()            X- and Y-values
        '        * KDTreeQueryResultsTags()          tag values
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreequeryresultsdistances(kdt As kdtree, ByRef r As Double())
            Dim i As Integer = 0
            Dim k As Integer = 0

            If kdt.kcur = 0 Then
                Return
            End If
            If alglib.ap.len(r) < kdt.kcur Then
                r = New Double(kdt.kcur - 1) {}
            End If
            k = kdt.kcur

            '
            ' unload norms
            '
            ' Abs() call is used to handle cases with negative norms
            ' (generated during KFN requests)
            '
            If kdt.normtype = 0 Then
                For i = 0 To k - 1
                    r(i) = System.Math.Abs(kdt.r(i))
                Next
            End If
            If kdt.normtype = 1 Then
                For i = 0 To k - 1
                    r(i) = System.Math.Abs(kdt.r(i))
                Next
            End If
            If kdt.normtype = 2 Then
                For i = 0 To k - 1
                    r(i) = System.Math.sqrt(System.Math.Abs(kdt.r(i)))
                Next
            End If
        End Sub


        '************************************************************************
        '        X-values from last query; 'interactive' variant for languages like  Python
        '        which   support    constructs   like  "X = KDTreeQueryResultsXI(KDT)"  and
        '        interactive mode of interpreter.
        '
        '        This function allocates new array on each call,  so  it  is  significantly
        '        slower than its 'non-interactive' counterpart, but it is  more  convenient
        '        when you call it from command line.
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreequeryresultsxi(kdt As kdtree, ByRef x As Double(,))
            x = New Double(-1, -1) {}

            kdtreequeryresultsx(kdt, x)
        End Sub


        '************************************************************************
        '        XY-values from last query; 'interactive' variant for languages like Python
        '        which   support    constructs   like "XY = KDTreeQueryResultsXYI(KDT)" and
        '        interactive mode of interpreter.
        '
        '        This function allocates new array on each call,  so  it  is  significantly
        '        slower than its 'non-interactive' counterpart, but it is  more  convenient
        '        when you call it from command line.
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreequeryresultsxyi(kdt As kdtree, ByRef xy As Double(,))
            xy = New Double(-1, -1) {}

            kdtreequeryresultsxy(kdt, xy)
        End Sub


        '************************************************************************
        '        Tags  from  last  query;  'interactive' variant for languages like  Python
        '        which  support  constructs  like "Tags = KDTreeQueryResultsTagsI(KDT)" and
        '        interactive mode of interpreter.
        '
        '        This function allocates new array on each call,  so  it  is  significantly
        '        slower than its 'non-interactive' counterpart, but it is  more  convenient
        '        when you call it from command line.
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreequeryresultstagsi(kdt As kdtree, ByRef tags As Integer())
            tags = New Integer(-1) {}

            kdtreequeryresultstags(kdt, tags)
        End Sub


        '************************************************************************
        '        Distances from last query; 'interactive' variant for languages like Python
        '        which  support  constructs   like  "R = KDTreeQueryResultsDistancesI(KDT)"
        '        and interactive mode of interpreter.
        '
        '        This function allocates new array on each call,  so  it  is  significantly
        '        slower than its 'non-interactive' counterpart, but it is  more  convenient
        '        when you call it from command line.
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreequeryresultsdistancesi(kdt As kdtree, ByRef r As Double())
            r = New Double(-1) {}

            kdtreequeryresultsdistances(kdt, r)
        End Sub


        '************************************************************************
        '        Serializer: allocation
        '
        '          -- ALGLIB --
        '             Copyright 14.03.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreealloc(s As alglib.serializer, tree As kdtree)

            '
            ' Header
            '
            s.alloc_entry()
            s.alloc_entry()

            '
            ' Data
            '
            s.alloc_entry()
            s.alloc_entry()
            s.alloc_entry()
            s.alloc_entry()
            apserv.allocrealmatrix(s, tree.xy, -1, -1)
            apserv.allocintegerarray(s, tree.tags, -1)
            apserv.allocrealarray(s, tree.boxmin, -1)
            apserv.allocrealarray(s, tree.boxmax, -1)
            apserv.allocintegerarray(s, tree.nodes, -1)
            apserv.allocrealarray(s, tree.splits, -1)
        End Sub


        '************************************************************************
        '        Serializer: serialization
        '
        '          -- ALGLIB --
        '             Copyright 14.03.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreeserialize(s As alglib.serializer, tree As kdtree)

            '
            ' Header
            '
            s.serialize_int(scodes.getkdtreeserializationcode())
            s.serialize_int(kdtreefirstversion)

            '
            ' Data
            '
            s.serialize_int(tree.n)
            s.serialize_int(tree.nx)
            s.serialize_int(tree.ny)
            s.serialize_int(tree.normtype)
            apserv.serializerealmatrix(s, tree.xy, -1, -1)
            apserv.serializeintegerarray(s, tree.tags, -1)
            apserv.serializerealarray(s, tree.boxmin, -1)
            apserv.serializerealarray(s, tree.boxmax, -1)
            apserv.serializeintegerarray(s, tree.nodes, -1)
            apserv.serializerealarray(s, tree.splits, -1)
        End Sub


        '************************************************************************
        '        Serializer: unserialization
        '
        '          -- ALGLIB --
        '             Copyright 14.03.2011 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub kdtreeunserialize(s As alglib.serializer, tree As kdtree)
            Dim i0 As Integer = 0
            Dim i1 As Integer = 0


            '
            ' check correctness of header
            '
            i0 = s.unserialize_int()
            alglib.ap.assert(i0 = scodes.getkdtreeserializationcode(), "KDTreeUnserialize: stream header corrupted")
            i1 = s.unserialize_int()
            alglib.ap.assert(i1 = kdtreefirstversion, "KDTreeUnserialize: stream header corrupted")

            '
            ' Unserialize data
            '
            tree.n = s.unserialize_int()
            tree.nx = s.unserialize_int()
            tree.ny = s.unserialize_int()
            tree.normtype = s.unserialize_int()
            apserv.unserializerealmatrix(s, tree.xy)
            apserv.unserializeintegerarray(s, tree.tags)
            apserv.unserializerealarray(s, tree.boxmin)
            apserv.unserializerealarray(s, tree.boxmax)
            apserv.unserializeintegerarray(s, tree.nodes)
            apserv.unserializerealarray(s, tree.splits)
            kdtreealloctemporaries(tree, tree.n, tree.nx, tree.ny)
        End Sub


        '************************************************************************
        '        Rearranges nodes [I1,I2) using partition in D-th dimension with S as threshold.
        '        Returns split position I3: [I1,I3) and [I3,I2) are created as result.
        '
        '        This subroutine doesn't create tree structures, just rearranges nodes.
        '        ************************************************************************

        Private Shared Sub kdtreesplit(kdt As kdtree, i1 As Integer, i2 As Integer, d As Integer, s As Double, ByRef i3 As Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim ileft As Integer = 0
            Dim iright As Integer = 0
            Dim v As Double = 0

            i3 = 0

            alglib.ap.assert(kdt.n > 0, "KDTreeSplit: internal error")

            '
            ' split XY/Tags in two parts:
            ' * [ILeft,IRight] is non-processed part of XY/Tags
            '
            ' After cycle is done, we have Ileft=IRight. We deal with
            ' this element separately.
            '
            ' After this, [I1,ILeft) contains left part, and [ILeft,I2)
            ' contains right part.
            '
            ileft = i1
            iright = i2 - 1
            While ileft < iright
                If CDbl(kdt.xy(ileft, d)) <= CDbl(s) Then

                    '
                    ' XY[ILeft] is on its place.
                    ' Advance ILeft.
                    '
                    ileft = ileft + 1
                Else

                    '
                    ' XY[ILeft,..] must be at IRight.
                    ' Swap and advance IRight.
                    '
                    For i = 0 To 2 * kdt.nx + kdt.ny - 1
                        v = kdt.xy(ileft, i)
                        kdt.xy(ileft, i) = kdt.xy(iright, i)
                        kdt.xy(iright, i) = v
                    Next
                    j = kdt.tags(ileft)
                    kdt.tags(ileft) = kdt.tags(iright)
                    kdt.tags(iright) = j
                    iright = iright - 1
                End If
            End While
            If CDbl(kdt.xy(ileft, d)) <= CDbl(s) Then
                ileft = ileft + 1
            Else
                iright = iright - 1
            End If
            i3 = ileft
        End Sub


        '************************************************************************
        '        Recursive kd-tree generation subroutine.
        '
        '        PARAMETERS
        '            KDT         tree
        '            NodesOffs   unused part of Nodes[] which must be filled by tree
        '            SplitsOffs  unused part of Splits[]
        '            I1, I2      points from [I1,I2) are processed
        '            
        '        NodesOffs[] and SplitsOffs[] must be large enough.
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub kdtreegeneratetreerec(kdt As kdtree, ByRef nodesoffs As Integer, ByRef splitsoffs As Integer, i1 As Integer, i2 As Integer, maxleafsize As Integer)
            Dim n As Integer = 0
            Dim nx As Integer = 0
            Dim ny As Integer = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim oldoffs As Integer = 0
            Dim i3 As Integer = 0
            Dim cntless As Integer = 0
            Dim cntgreater As Integer = 0
            Dim minv As Double = 0
            Dim maxv As Double = 0
            Dim minidx As Integer = 0
            Dim maxidx As Integer = 0
            Dim d As Integer = 0
            Dim ds As Double = 0
            Dim s As Double = 0
            Dim v As Double = 0
            Dim v0 As Double = 0
            Dim v1 As Double = 0
            Dim i_ As Integer = 0
            Dim i1_ As Integer = 0

            alglib.ap.assert(kdt.n > 0, "KDTreeGenerateTreeRec: internal error")
            alglib.ap.assert(i2 > i1, "KDTreeGenerateTreeRec: internal error")

            '
            ' Generate leaf if needed
            '
            If i2 - i1 <= maxleafsize Then
                kdt.nodes(nodesoffs + 0) = i2 - i1
                kdt.nodes(nodesoffs + 1) = i1
                nodesoffs = nodesoffs + 2
                Return
            End If

            '
            ' Load values for easier access
            '
            nx = kdt.nx
            ny = kdt.ny

            '
            ' Select dimension to split:
            ' * D is a dimension number
            ' In case bounding box has zero size, we enforce creation of the leaf node.
            '
            d = 0
            ds = kdt.curboxmax(0) - kdt.curboxmin(0)
            For i = 1 To nx - 1
                v = kdt.curboxmax(i) - kdt.curboxmin(i)
                If CDbl(v) > CDbl(ds) Then
                    ds = v
                    d = i
                End If
            Next
            If CDbl(ds) = CDbl(0) Then
                kdt.nodes(nodesoffs + 0) = i2 - i1
                kdt.nodes(nodesoffs + 1) = i1
                nodesoffs = nodesoffs + 2
                Return
            End If

            '
            ' Select split position S using sliding midpoint rule,
            ' rearrange points into [I1,I3) and [I3,I2).
            '
            ' In case all points has same value of D-th component
            ' (MinV=MaxV) we enforce D-th dimension of bounding
            ' box to become exactly zero and repeat tree construction.
            '
            s = kdt.curboxmin(d) + 0.5 * ds
            i1_ = (i1) - (0)
            For i_ = 0 To i2 - i1 - 1
                kdt.buf(i_) = kdt.xy(i_ + i1_, d)
            Next
            n = i2 - i1
            cntless = 0
            cntgreater = 0
            minv = kdt.buf(0)
            maxv = kdt.buf(0)
            minidx = i1
            maxidx = i1
            For i = 0 To n - 1
                v = kdt.buf(i)
                If CDbl(v) < CDbl(minv) Then
                    minv = v
                    minidx = i1 + i
                End If
                If CDbl(v) > CDbl(maxv) Then
                    maxv = v
                    maxidx = i1 + i
                End If
                If CDbl(v) < CDbl(s) Then
                    cntless = cntless + 1
                End If
                If CDbl(v) > CDbl(s) Then
                    cntgreater = cntgreater + 1
                End If
            Next
            If CDbl(minv) = CDbl(maxv) Then

                '
                ' In case all points has same value of D-th component
                ' (MinV=MaxV) we enforce D-th dimension of bounding
                ' box to become exactly zero and repeat tree construction.
                '
                v0 = kdt.curboxmin(d)
                v1 = kdt.curboxmax(d)
                kdt.curboxmin(d) = minv
                kdt.curboxmax(d) = maxv
                kdtreegeneratetreerec(kdt, nodesoffs, splitsoffs, i1, i2, maxleafsize)
                kdt.curboxmin(d) = v0
                kdt.curboxmax(d) = v1
                Return
            End If
            If cntless > 0 AndAlso cntgreater > 0 Then

                '
                ' normal midpoint split
                '
                kdtreesplit(kdt, i1, i2, d, s, i3)
            Else

                '
                ' sliding midpoint
                '
                If cntless = 0 Then

                    '
                    ' 1. move split to MinV,
                    ' 2. place one point to the left bin (move to I1),
                    '    others - to the right bin
                    '
                    s = minv
                    If minidx <> i1 Then
                        For i = 0 To 2 * nx + ny - 1
                            v = kdt.xy(minidx, i)
                            kdt.xy(minidx, i) = kdt.xy(i1, i)
                            kdt.xy(i1, i) = v
                        Next
                        j = kdt.tags(minidx)
                        kdt.tags(minidx) = kdt.tags(i1)
                        kdt.tags(i1) = j
                    End If
                    i3 = i1 + 1
                Else

                    '
                    ' 1. move split to MaxV,
                    ' 2. place one point to the right bin (move to I2-1),
                    '    others - to the left bin
                    '
                    s = maxv
                    If maxidx <> i2 - 1 Then
                        For i = 0 To 2 * nx + ny - 1
                            v = kdt.xy(maxidx, i)
                            kdt.xy(maxidx, i) = kdt.xy(i2 - 1, i)
                            kdt.xy(i2 - 1, i) = v
                        Next
                        j = kdt.tags(maxidx)
                        kdt.tags(maxidx) = kdt.tags(i2 - 1)
                        kdt.tags(i2 - 1) = j
                    End If
                    i3 = i2 - 1
                End If
            End If

            '
            ' Generate 'split' node
            '
            kdt.nodes(nodesoffs + 0) = 0
            kdt.nodes(nodesoffs + 1) = d
            kdt.nodes(nodesoffs + 2) = splitsoffs
            kdt.splits(splitsoffs + 0) = s
            oldoffs = nodesoffs
            nodesoffs = nodesoffs + splitnodesize
            splitsoffs = splitsoffs + 1

            '
            ' Recirsive generation:
            ' * update CurBox
            ' * call subroutine
            ' * restore CurBox
            '
            kdt.nodes(oldoffs + 3) = nodesoffs
            v = kdt.curboxmax(d)
            kdt.curboxmax(d) = s
            kdtreegeneratetreerec(kdt, nodesoffs, splitsoffs, i1, i3, maxleafsize)
            kdt.curboxmax(d) = v
            kdt.nodes(oldoffs + 4) = nodesoffs
            v = kdt.curboxmin(d)
            kdt.curboxmin(d) = s
            kdtreegeneratetreerec(kdt, nodesoffs, splitsoffs, i3, i2, maxleafsize)
            kdt.curboxmin(d) = v
        End Sub


        '************************************************************************
        '        Recursive subroutine for NN queries.
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub kdtreequerynnrec(kdt As kdtree, offs As Integer)
            Dim ptdist As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim nx As Integer = 0
            Dim i1 As Integer = 0
            Dim i2 As Integer = 0
            Dim d As Integer = 0
            Dim s As Double = 0
            Dim v As Double = 0
            Dim t1 As Double = 0
            Dim childbestoffs As Integer = 0
            Dim childworstoffs As Integer = 0
            Dim childoffs As Integer = 0
            Dim prevdist As Double = 0
            Dim todive As New Boolean()
            Dim bestisleft As New Boolean()
            Dim updatemin As New Boolean()

            alglib.ap.assert(kdt.n > 0, "KDTreeQueryNNRec: internal error")

            '
            ' Leaf node.
            ' Process points.
            '
            If kdt.nodes(offs) > 0 Then
                i1 = kdt.nodes(offs + 1)
                i2 = i1 + kdt.nodes(offs)
                For i = i1 To i2 - 1

                    '
                    ' Calculate distance
                    '
                    ptdist = 0
                    nx = kdt.nx
                    If kdt.normtype = 0 Then
                        For j = 0 To nx - 1
                            ptdist = System.Math.Max(ptdist, System.Math.Abs(kdt.xy(i, j) - kdt.x(j)))
                        Next
                    End If
                    If kdt.normtype = 1 Then
                        For j = 0 To nx - 1
                            ptdist = ptdist + System.Math.Abs(kdt.xy(i, j) - kdt.x(j))
                        Next
                    End If
                    If kdt.normtype = 2 Then
                        For j = 0 To nx - 1
                            ptdist = ptdist + Math.sqr(kdt.xy(i, j) - kdt.x(j))
                        Next
                    End If

                    '
                    ' Skip points with zero distance if self-matches are turned off
                    '
                    If CDbl(ptdist) = CDbl(0) AndAlso Not kdt.selfmatch Then
                        Continue For
                    End If

                    '
                    ' We CAN'T process point if R-criterion isn't satisfied,
                    ' i.e. (RNeeded<>0) AND (PtDist>R).
                    '
                    If CDbl(kdt.rneeded) = CDbl(0) OrElse CDbl(ptdist) <= CDbl(kdt.rneeded) Then

                        '
                        ' R-criterion is satisfied, we must either:
                        ' * replace worst point, if (KNeeded<>0) AND (KCur=KNeeded)
                        '   (or skip, if worst point is better)
                        ' * add point without replacement otherwise
                        '
                        If kdt.kcur < kdt.kneeded OrElse kdt.kneeded = 0 Then

                            '
                            ' add current point to heap without replacement
                            '
                            tsort.tagheappushi(kdt.r, kdt.idx, kdt.kcur, ptdist, i)
                        Else

                            '
                            ' New points are added or not, depending on their distance.
                            ' If added, they replace element at the top of the heap
                            '
                            If CDbl(ptdist) < CDbl(kdt.r(0)) Then
                                If kdt.kneeded = 1 Then
                                    kdt.idx(0) = i
                                    kdt.r(0) = ptdist
                                Else
                                    tsort.tagheapreplacetopi(kdt.r, kdt.idx, kdt.kneeded, ptdist, i)
                                End If
                            End If
                        End If
                    End If
                Next
                Return
            End If

            '
            ' Simple split
            '
            If kdt.nodes(offs) = 0 Then

                '
                ' Load:
                ' * D  dimension to split
                ' * S  split position
                '
                d = kdt.nodes(offs + 1)
                s = kdt.splits(kdt.nodes(offs + 2))

                '
                ' Calculate:
                ' * ChildBestOffs      child box with best chances
                ' * ChildWorstOffs     child box with worst chances
                '
                If CDbl(kdt.x(d)) <= CDbl(s) Then
                    childbestoffs = kdt.nodes(offs + 3)
                    childworstoffs = kdt.nodes(offs + 4)
                    bestisleft = True
                Else
                    childbestoffs = kdt.nodes(offs + 4)
                    childworstoffs = kdt.nodes(offs + 3)
                    bestisleft = False
                End If

                '
                ' Navigate through childs
                '
                For i = 0 To 1

                    '
                    ' Select child to process:
                    ' * ChildOffs      current child offset in Nodes[]
                    ' * UpdateMin      whether minimum or maximum value
                    '                  of bounding box is changed on update
                    '
                    If i = 0 Then
                        childoffs = childbestoffs
                        updatemin = Not bestisleft
                    Else
                        updatemin = bestisleft
                        childoffs = childworstoffs
                    End If

                    '
                    ' Update bounding box and current distance
                    '
                    If updatemin Then
                        prevdist = kdt.curdist
                        t1 = kdt.x(d)
                        v = kdt.curboxmin(d)
                        If CDbl(t1) <= CDbl(s) Then
                            If kdt.normtype = 0 Then
                                kdt.curdist = System.Math.Max(kdt.curdist, s - t1)
                            End If
                            If kdt.normtype = 1 Then
                                kdt.curdist = kdt.curdist - System.Math.Max(v - t1, 0) + s - t1
                            End If
                            If kdt.normtype = 2 Then
                                kdt.curdist = kdt.curdist - Math.sqr(System.Math.Max(v - t1, 0)) + Math.sqr(s - t1)
                            End If
                        End If
                        kdt.curboxmin(d) = s
                    Else
                        prevdist = kdt.curdist
                        t1 = kdt.x(d)
                        v = kdt.curboxmax(d)
                        If CDbl(t1) >= CDbl(s) Then
                            If kdt.normtype = 0 Then
                                kdt.curdist = System.Math.Max(kdt.curdist, t1 - s)
                            End If
                            If kdt.normtype = 1 Then
                                kdt.curdist = kdt.curdist - System.Math.Max(t1 - v, 0) + t1 - s
                            End If
                            If kdt.normtype = 2 Then
                                kdt.curdist = kdt.curdist - Math.sqr(System.Math.Max(t1 - v, 0)) + Math.sqr(t1 - s)
                            End If
                        End If
                        kdt.curboxmax(d) = s
                    End If

                    '
                    ' Decide: to dive into cell or not to dive
                    '
                    If CDbl(kdt.rneeded) <> CDbl(0) AndAlso CDbl(kdt.curdist) > CDbl(kdt.rneeded) Then
                        todive = False
                    Else
                        If kdt.kcur < kdt.kneeded OrElse kdt.kneeded = 0 Then

                            '
                            ' KCur<KNeeded (i.e. not all points are found)
                            '
                            todive = True
                        Else

                            '
                            ' KCur=KNeeded, decide to dive or not to dive
                            ' using point position relative to bounding box.
                            '
                            todive = CDbl(kdt.curdist) <= CDbl(kdt.r(0) * kdt.approxf)
                        End If
                    End If
                    If todive Then
                        kdtreequerynnrec(kdt, childoffs)
                    End If

                    '
                    ' Restore bounding box and distance
                    '
                    If updatemin Then
                        kdt.curboxmin(d) = v
                    Else
                        kdt.curboxmax(d) = v
                    End If
                    kdt.curdist = prevdist
                Next
                Return
            End If
        End Sub


        '************************************************************************
        '        Copies X[] to KDT.X[]
        '        Loads distance from X[] to bounding box.
        '        Initializes CurBox[].
        '
        '          -- ALGLIB --
        '             Copyright 28.02.2010 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub kdtreeinitbox(kdt As kdtree, x As Double())
            Dim i As Integer = 0
            Dim vx As Double = 0
            Dim vmin As Double = 0
            Dim vmax As Double = 0

            alglib.ap.assert(kdt.n > 0, "KDTreeInitBox: internal error")

            '
            ' calculate distance from point to current bounding box
            '
            kdt.curdist = 0
            If kdt.normtype = 0 Then
                For i = 0 To kdt.nx - 1
                    vx = x(i)
                    vmin = kdt.boxmin(i)
                    vmax = kdt.boxmax(i)
                    kdt.x(i) = vx
                    kdt.curboxmin(i) = vmin
                    kdt.curboxmax(i) = vmax
                    If CDbl(vx) < CDbl(vmin) Then
                        kdt.curdist = System.Math.Max(kdt.curdist, vmin - vx)
                    Else
                        If CDbl(vx) > CDbl(vmax) Then
                            kdt.curdist = System.Math.Max(kdt.curdist, vx - vmax)
                        End If
                    End If
                Next
            End If
            If kdt.normtype = 1 Then
                For i = 0 To kdt.nx - 1
                    vx = x(i)
                    vmin = kdt.boxmin(i)
                    vmax = kdt.boxmax(i)
                    kdt.x(i) = vx
                    kdt.curboxmin(i) = vmin
                    kdt.curboxmax(i) = vmax
                    If CDbl(vx) < CDbl(vmin) Then
                        kdt.curdist = kdt.curdist + vmin - vx
                    Else
                        If CDbl(vx) > CDbl(vmax) Then
                            kdt.curdist = kdt.curdist + vx - vmax
                        End If
                    End If
                Next
            End If
            If kdt.normtype = 2 Then
                For i = 0 To kdt.nx - 1
                    vx = x(i)
                    vmin = kdt.boxmin(i)
                    vmax = kdt.boxmax(i)
                    kdt.x(i) = vx
                    kdt.curboxmin(i) = vmin
                    kdt.curboxmax(i) = vmax
                    If CDbl(vx) < CDbl(vmin) Then
                        kdt.curdist = kdt.curdist + Math.sqr(vmin - vx)
                    Else
                        If CDbl(vx) > CDbl(vmax) Then
                            kdt.curdist = kdt.curdist + Math.sqr(vx - vmax)
                        End If
                    End If
                Next
            End If
        End Sub


        '************************************************************************
        '        This function allocates all dataset-independent array  fields  of  KDTree,
        '        i.e.  such  array  fields  that  their dimensions do not depend on dataset
        '        size.
        '
        '        This function do not sets KDT.NX or KDT.NY - it just allocates arrays
        '
        '          -- ALGLIB --
        '             Copyright 14.03.2011 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub kdtreeallocdatasetindependent(kdt As kdtree, nx As Integer, ny As Integer)
            alglib.ap.assert(kdt.n > 0, "KDTreeAllocDatasetIndependent: internal error")
            kdt.x = New Double(nx - 1) {}
            kdt.boxmin = New Double(nx - 1) {}
            kdt.boxmax = New Double(nx - 1) {}
            kdt.curboxmin = New Double(nx - 1) {}
            kdt.curboxmax = New Double(nx - 1) {}
        End Sub


        '************************************************************************
        '        This function allocates all dataset-dependent array fields of KDTree, i.e.
        '        such array fields that their dimensions depend on dataset size.
        '
        '        This function do not sets KDT.N, KDT.NX or KDT.NY -
        '        it just allocates arrays.
        '
        '          -- ALGLIB --
        '             Copyright 14.03.2011 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub kdtreeallocdatasetdependent(kdt As kdtree, n As Integer, nx As Integer, ny As Integer)
            alglib.ap.assert(n > 0, "KDTreeAllocDatasetDependent: internal error")
            kdt.xy = New Double(n - 1, 2 * nx + (ny - 1)) {}
            kdt.tags = New Integer(n - 1) {}
            kdt.idx = New Integer(n - 1) {}
            kdt.r = New Double(n - 1) {}
            kdt.x = New Double(nx - 1) {}
            kdt.buf = New Double(System.Math.Max(n, nx) - 1) {}
            kdt.nodes = New Integer(splitnodesize * 2 * n - 1) {}
            kdt.splits = New Double(2 * n - 1) {}
        End Sub


        '************************************************************************
        '        This function allocates temporaries.
        '
        '        This function do not sets KDT.N, KDT.NX or KDT.NY -
        '        it just allocates arrays.
        '
        '          -- ALGLIB --
        '             Copyright 14.03.2011 by Bochkanov Sergey
        '        ************************************************************************

        Private Shared Sub kdtreealloctemporaries(kdt As kdtree, n As Integer, nx As Integer, ny As Integer)
            alglib.ap.assert(n > 0, "KDTreeAllocTemporaries: internal error")
            kdt.x = New Double(nx - 1) {}
            kdt.idx = New Integer(n - 1) {}
            kdt.r = New Double(n - 1) {}
            kdt.buf = New Double(System.Math.Max(n, nx) - 1) {}
            kdt.curboxmin = New Double(nx - 1) {}
            kdt.curboxmax = New Double(nx - 1) {}
        End Sub


    End Class
    Public Class xdebug
        Public Class xdebugrecord1
            Inherits apobject
            Public i As Integer
            Public c As complex
            Public a As Double()
            Public Sub New()
                init()
            End Sub
            Public Overrides Sub init()
                a = New Double(-1) {}
            End Sub
            Public Overrides Function make_copy() As alglib.apobject
                Dim _result As New xdebugrecord1()
                _result.i = i
                _result.c = c
                _result.a = DirectCast(a.Clone(), Double())
                Return _result
            End Function
        End Class




        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Creates and returns XDebugRecord1 structure:
        '        * integer and complex fields of Rec1 are set to 1 and 1+i correspondingly
        '        * array field of Rec1 is set to [2,3]
        '
        '          -- ALGLIB --
        '             Copyright 27.05.2014 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebuginitrecord1(rec1 As xdebugrecord1)
            rec1.i = 1
            rec1.c.x = 1
            rec1.c.y = 1
            rec1.a = New Double(1) {}
            rec1.a(0) = 2
            rec1.a(1) = 3
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Counts number of True values in the boolean 1D array.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function xdebugb1count(a As Boolean()) As Integer
            Dim result As Integer = 0
            Dim i As Integer = 0

            result = 0
            For i = 0 To alglib.ap.len(a) - 1
                If a(i) Then
                    result = result + 1
                End If
            Next
            Return result
        End Function


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Replace all values in array by NOT(a[i]).
        '        Array is passed using "shared" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugb1not(a As Boolean())
            Dim i As Integer = 0

            For i = 0 To alglib.ap.len(a) - 1
                a(i) = Not a(i)
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Appends copy of array to itself.
        '        Array is passed using "var" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugb1appendcopy(ByRef a As Boolean())
            Dim i As Integer = 0
            Dim b As Boolean() = New Boolean(-1) {}

            b = New Boolean(alglib.ap.len(a) - 1) {}
            For i = 0 To alglib.ap.len(b) - 1
                b(i) = a(i)
            Next
            a = New Boolean(2 * alglib.ap.len(b) - 1) {}
            For i = 0 To alglib.ap.len(a) - 1
                a(i) = b(i Mod alglib.ap.len(b))
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Generate N-element array with even-numbered elements set to True.
        '        Array is passed using "out" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugb1outeven(n As Integer, ByRef a As Boolean())
            Dim i As Integer = 0

            a = New Boolean(-1) {}

            a = New Boolean(n - 1) {}
            For i = 0 To alglib.ap.len(a) - 1
                a(i) = i Mod 2 = 0
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Returns sum of elements in the array.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function xdebugi1sum(a As Integer()) As Integer
            Dim result As Integer = 0
            Dim i As Integer = 0

            result = 0
            For i = 0 To alglib.ap.len(a) - 1
                result = result + a(i)
            Next
            Return result
        End Function


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Replace all values in array by -A[I]
        '        Array is passed using "shared" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugi1neg(a As Integer())
            Dim i As Integer = 0

            For i = 0 To alglib.ap.len(a) - 1
                a(i) = -a(i)
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Appends copy of array to itself.
        '        Array is passed using "var" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugi1appendcopy(ByRef a As Integer())
            Dim i As Integer = 0
            Dim b As Integer() = New Integer(-1) {}

            b = New Integer(alglib.ap.len(a) - 1) {}
            For i = 0 To alglib.ap.len(b) - 1
                b(i) = a(i)
            Next
            a = New Integer(2 * alglib.ap.len(b) - 1) {}
            For i = 0 To alglib.ap.len(a) - 1
                a(i) = b(i Mod alglib.ap.len(b))
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Generate N-element array with even-numbered A[I] set to I, and odd-numbered
        '        ones set to 0.
        '
        '        Array is passed using "out" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugi1outeven(n As Integer, ByRef a As Integer())
            Dim i As Integer = 0

            a = New Integer(-1) {}

            a = New Integer(n - 1) {}
            For i = 0 To alglib.ap.len(a) - 1
                If i Mod 2 = 0 Then
                    a(i) = i
                Else
                    a(i) = 0
                End If
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Returns sum of elements in the array.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function xdebugr1sum(a As Double()) As Double
            Dim result As Double = 0
            Dim i As Integer = 0

            result = 0
            For i = 0 To alglib.ap.len(a) - 1
                result = result + a(i)
            Next
            Return result
        End Function


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Replace all values in array by -A[I]
        '        Array is passed using "shared" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugr1neg(a As Double())
            Dim i As Integer = 0

            For i = 0 To alglib.ap.len(a) - 1
                a(i) = -a(i)
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Appends copy of array to itself.
        '        Array is passed using "var" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugr1appendcopy(ByRef a As Double())
            Dim i As Integer = 0
            Dim b As Double() = New Double(-1) {}

            b = New Double(alglib.ap.len(a) - 1) {}
            For i = 0 To alglib.ap.len(b) - 1
                b(i) = a(i)
            Next
            a = New Double(2 * alglib.ap.len(b) - 1) {}
            For i = 0 To alglib.ap.len(a) - 1
                a(i) = b(i Mod alglib.ap.len(b))
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Generate N-element array with even-numbered A[I] set to I*0.25,
        '        and odd-numbered ones are set to 0.
        '
        '        Array is passed using "out" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugr1outeven(n As Integer, ByRef a As Double())
            Dim i As Integer = 0

            a = New Double(-1) {}

            a = New Double(n - 1) {}
            For i = 0 To alglib.ap.len(a) - 1
                If i Mod 2 = 0 Then
                    a(i) = i * 0.25
                Else
                    a(i) = 0
                End If
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Returns sum of elements in the array.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function xdebugc1sum(a As complex()) As complex
            Dim result As complex = 0
            Dim i As Integer = 0

            result = 0
            For i = 0 To alglib.ap.len(a) - 1
                result = result + a(i)
            Next
            Return result
        End Function


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Replace all values in array by -A[I]
        '        Array is passed using "shared" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugc1neg(a As complex())
            Dim i As Integer = 0

            For i = 0 To alglib.ap.len(a) - 1
                a(i) = -a(i)
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Appends copy of array to itself.
        '        Array is passed using "var" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugc1appendcopy(ByRef a As complex())
            Dim i As Integer = 0
            Dim b As complex() = New complex(-1) {}

            b = New complex(alglib.ap.len(a) - 1) {}
            For i = 0 To alglib.ap.len(b) - 1
                b(i) = a(i)
            Next
            a = New complex(2 * alglib.ap.len(b) - 1) {}
            For i = 0 To alglib.ap.len(a) - 1
                a(i) = b(i Mod alglib.ap.len(b))
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Generate N-element array with even-numbered A[K] set to (x,y) = (K*0.25, K*0.125)
        '        and odd-numbered ones are set to 0.
        '
        '        Array is passed using "out" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugc1outeven(n As Integer, ByRef a As complex())
            Dim i As Integer = 0

            a = New complex(-1) {}

            a = New complex(n - 1) {}
            For i = 0 To alglib.ap.len(a) - 1
                If i Mod 2 = 0 Then
                    a(i).x = i * 0.25
                    a(i).y = i * 0.125
                Else
                    a(i) = 0
                End If
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Counts number of True values in the boolean 2D array.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function xdebugb2count(a As Boolean(,)) As Integer
            Dim result As Integer = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            result = 0
            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    If a(i, j) Then
                        result = result + 1
                    End If
                Next
            Next
            Return result
        End Function


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Replace all values in array by NOT(a[i]).
        '        Array is passed using "shared" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugb2not(a As Boolean(,))
            Dim i As Integer = 0
            Dim j As Integer = 0

            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    a(i, j) = Not a(i, j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Transposes array.
        '        Array is passed using "var" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugb2transpose(ByRef a As Boolean(,))
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim b As Boolean(,) = New Boolean(-1, -1) {}

            b = New Boolean(alglib.ap.rows(a) - 1, alglib.ap.cols(a) - 1) {}
            For i = 0 To alglib.ap.rows(b) - 1
                For j = 0 To alglib.ap.cols(b) - 1
                    b(i, j) = a(i, j)
                Next
            Next
            a = New Boolean(alglib.ap.cols(b) - 1, alglib.ap.rows(b) - 1) {}
            For i = 0 To alglib.ap.rows(b) - 1
                For j = 0 To alglib.ap.cols(b) - 1
                    a(j, i) = b(i, j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Generate MxN matrix with elements set to "Sin(3*I+5*J)>0"
        '        Array is passed using "out" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugb2outsin(m As Integer, n As Integer, ByRef a As Boolean(,))
            Dim i As Integer = 0
            Dim j As Integer = 0

            a = New Boolean(-1, -1) {}

            a = New Boolean(m - 1, n - 1) {}
            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    a(i, j) = CDbl(System.Math.Sin(3 * i + 5 * j)) > CDbl(0)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Returns sum of elements in the array.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function xdebugi2sum(a As Integer(,)) As Integer
            Dim result As Integer = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            result = 0
            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    result = result + a(i, j)
                Next
            Next
            Return result
        End Function


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Replace all values in array by -a[i,j]
        '        Array is passed using "shared" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugi2neg(a As Integer(,))
            Dim i As Integer = 0
            Dim j As Integer = 0

            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    a(i, j) = -a(i, j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Transposes array.
        '        Array is passed using "var" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugi2transpose(ByRef a As Integer(,))
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim b As Integer(,) = New Integer(-1, -1) {}

            b = New Integer(alglib.ap.rows(a) - 1, alglib.ap.cols(a) - 1) {}
            For i = 0 To alglib.ap.rows(b) - 1
                For j = 0 To alglib.ap.cols(b) - 1
                    b(i, j) = a(i, j)
                Next
            Next
            a = New Integer(alglib.ap.cols(b) - 1, alglib.ap.rows(b) - 1) {}
            For i = 0 To alglib.ap.rows(b) - 1
                For j = 0 To alglib.ap.cols(b) - 1
                    a(j, i) = b(i, j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Generate MxN matrix with elements set to "Sign(Sin(3*I+5*J))"
        '        Array is passed using "out" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugi2outsin(m As Integer, n As Integer, ByRef a As Integer(,))
            Dim i As Integer = 0
            Dim j As Integer = 0

            a = New Integer(-1, -1) {}

            a = New Integer(m - 1, n - 1) {}
            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    a(i, j) = System.Math.Sign(System.Math.Sin(3 * i + 5 * j))
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Returns sum of elements in the array.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function xdebugr2sum(a As Double(,)) As Double
            Dim result As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            result = 0
            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    result = result + a(i, j)
                Next
            Next
            Return result
        End Function


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Replace all values in array by -a[i,j]
        '        Array is passed using "shared" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugr2neg(a As Double(,))
            Dim i As Integer = 0
            Dim j As Integer = 0

            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    a(i, j) = -a(i, j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Transposes array.
        '        Array is passed using "var" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugr2transpose(ByRef a As Double(,))
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim b As Double(,) = New Double(-1, -1) {}

            b = New Double(alglib.ap.rows(a) - 1, alglib.ap.cols(a) - 1) {}
            For i = 0 To alglib.ap.rows(b) - 1
                For j = 0 To alglib.ap.cols(b) - 1
                    b(i, j) = a(i, j)
                Next
            Next
            a = New Double(alglib.ap.cols(b) - 1, alglib.ap.rows(b) - 1) {}
            For i = 0 To alglib.ap.rows(b) - 1
                For j = 0 To alglib.ap.cols(b) - 1
                    a(j, i) = b(i, j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Generate MxN matrix with elements set to "Sin(3*I+5*J)"
        '        Array is passed using "out" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugr2outsin(m As Integer, n As Integer, ByRef a As Double(,))
            Dim i As Integer = 0
            Dim j As Integer = 0

            a = New Double(-1, -1) {}

            a = New Double(m - 1, n - 1) {}
            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    a(i, j) = System.Math.Sin(3 * i + 5 * j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Returns sum of elements in the array.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function xdebugc2sum(a As complex(,)) As complex
            Dim result As complex = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            result = 0
            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    result = result + a(i, j)
                Next
            Next
            Return result
        End Function


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Replace all values in array by -a[i,j]
        '        Array is passed using "shared" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugc2neg(a As complex(,))
            Dim i As Integer = 0
            Dim j As Integer = 0

            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    a(i, j) = -a(i, j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Transposes array.
        '        Array is passed using "var" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugc2transpose(ByRef a As complex(,))
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim b As complex(,) = New complex(-1, -1) {}

            b = New complex(alglib.ap.rows(a) - 1, alglib.ap.cols(a) - 1) {}
            For i = 0 To alglib.ap.rows(b) - 1
                For j = 0 To alglib.ap.cols(b) - 1
                    b(i, j) = a(i, j)
                Next
            Next
            a = New complex(alglib.ap.cols(b) - 1, alglib.ap.rows(b) - 1) {}
            For i = 0 To alglib.ap.rows(b) - 1
                For j = 0 To alglib.ap.cols(b) - 1
                    a(j, i) = b(i, j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Generate MxN matrix with elements set to "Sin(3*I+5*J),Cos(3*I+5*J)"
        '        Array is passed using "out" convention.
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Sub xdebugc2outsincos(m As Integer, n As Integer, ByRef a As complex(,))
            Dim i As Integer = 0
            Dim j As Integer = 0

            a = New complex(-1, -1) {}

            a = New complex(m - 1, n - 1) {}
            For i = 0 To alglib.ap.rows(a) - 1
                For j = 0 To alglib.ap.cols(a) - 1
                    a(i, j).x = System.Math.Sin(3 * i + 5 * j)
                    a(i, j).y = System.Math.Cos(3 * i + 5 * j)
                Next
            Next
        End Sub


        '************************************************************************
        '        This is debug function intended for testing ALGLIB interface generator.
        '        Never use it in any real life project.
        '
        '        Returns sum of a[i,j]*(1+b[i,j]) such that c[i,j] is True
        '
        '          -- ALGLIB --
        '             Copyright 11.10.2013 by Bochkanov Sergey
        '        ************************************************************************

        Public Shared Function xdebugmaskedbiasedproductsum(m As Integer, n As Integer, a As Double(,), b As Double(,), c As Boolean(,)) As Double
            Dim result As Double = 0
            Dim i As Integer = 0
            Dim j As Integer = 0

            alglib.ap.assert(m >= alglib.ap.rows(a))
            alglib.ap.assert(m >= alglib.ap.rows(b))
            alglib.ap.assert(m >= alglib.ap.rows(c))
            alglib.ap.assert(n >= alglib.ap.cols(a))
            alglib.ap.assert(n >= alglib.ap.cols(b))
            alglib.ap.assert(n >= alglib.ap.cols(c))
            result = 0.0
            For i = 0 To m - 1
                For j = 0 To n - 1
                    If c(i, j) Then
                        result = result + a(i, j) * (1 + b(i, j))
                    End If
                Next
            Next
            Return result
        End Function


    End Class
End Class

