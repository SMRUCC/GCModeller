Imports System.Runtime.InteropServices
Imports UnixRStart = RDotNet.Internals.Unix.RStart
Imports WindowsRStart = RDotNet.Internals.Windows.RStart

Namespace Internals
	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_setStartTime()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_initialize_R(ac As Integer, argv As String()) As Integer

	''' <summary>
	''' Necessary to call at initialization on Windows, otherwise some (all?) of the startup parameters are NOT picked
	''' </summary>
	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub cmdlineoptions(ac As Integer, argv As String())

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_set_command_line_arguments(argc As Integer, argv As String())

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_DefParams_Unix(ByRef start As UnixRStart)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_DefParams_Windows(ByRef start As WindowsRStart)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_SetParams_Unix(ByRef start As UnixRStart)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_SetParams_Windows(ByRef start As WindowsRStart)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub setup_Rmainloop()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_ReplDLLinit()

	''' <summary>
	''' Initialise R for embedding
	''' </summary>
	''' <param name="argc">The length of argv</param>
	''' <param name="argv">arguments passed to the embedded engine</param>
	''' <remarks>
	''' <code>
	''' int Rf_initEmbeddedR(int argc, char **argv)
	'''{
	'''    Rf_initialize_R(argc, argv);
	'''   // R_Interactive is set to true in unix Rembedded.c, not gnuwin
	'''    R_Interactive = TRUE;  /* Rf_initialize_R set this based on isatty */
	'''    setup_Rmainloop();
	'''    return(1);
	'''}
	''' </code>
	''' </remarks>
	''' <returns></returns>
	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_initEmbeddedR(argc As Integer, argv As String()) As Integer

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub Rf_endEmbeddedR(fatal As Integer)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_RunExitFinalizers()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub Rf_CleanEd()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_CleanTempDir()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_protect(sexp As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_PreserveObject(sexp As IntPtr)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_ReleaseObject(sexp As IntPtr)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub Rf_unprotect(count As Integer)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub Rf_unprotect_ptr(sexp As IntPtr)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_install(s As String) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_mkString(s As String) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_mkChar(s As String) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_asCharacterFactor(sexp As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_allocVector(type As SymbolicExpressionType, length As Integer) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_coerceVector(sexp As IntPtr, type As SymbolicExpressionType) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isVector(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isFrame(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isS4(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_length(sexp As IntPtr) As Integer

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_allocMatrix(type As SymbolicExpressionType, rowCount As Integer, columnCount As Integer) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isMatrix(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_nrows(sexp As IntPtr) As Integer

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_ncols(sexp As IntPtr) As Integer

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_allocList(length As Integer) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isList(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_eval(statement As IntPtr, environment As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function R_tryEval(statement As IntPtr, environment As IntPtr, ByRef errorOccurred As Boolean) As IntPtr

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Function R_ParseVector(statement As IntPtr, statementCount As Integer, ByRef status As ParseStatus, p As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_findVar(name As IntPtr, environment As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub Rf_setVar(name As IntPtr, value As IntPtr, environment As IntPtr)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub Rf_defineVar(name As IntPtr, value As IntPtr, environment As IntPtr)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_getAttrib(sexp As IntPtr, name As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_setAttrib(sexp As IntPtr, name As IntPtr, value As IntPtr) As IntPtr

	'SEXP R_do_slot(SEXP obj, SEXP name);
	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function R_do_slot(sexp As IntPtr, name As IntPtr) As IntPtr

	'SEXP R_do_slot_assign(SEXP obj, SEXP name, SEXP value);
	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function R_do_slot_assign(sexp As IntPtr, name As IntPtr, value As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function R_getClassDef(what As String) As IntPtr

	'int R_has_slot(SEXP obj, SEXP name)
	Friend Delegate Function R_has_slot(sexp As IntPtr, name As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isEnvironment(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isExpression(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isSymbol(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isLanguage(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isFunction(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isFactor(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_isOrdered(sexp As IntPtr) As Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function R_lsInternal(environment As IntPtr, all As Boolean) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_applyClosure([call] As IntPtr, value As IntPtr, arguments As IntPtr, environment As IntPtr, suppliedEnvironment As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_VectorToPairList(sexp As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_allocSExp(type As SymbolicExpressionType) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_NewEnvironment(names As IntPtr, values As IntPtr, parent As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_cons(sexp As IntPtr, [next] As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function Rf_lcons(sexp As IntPtr, [next] As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub R_gc()
End Namespace
