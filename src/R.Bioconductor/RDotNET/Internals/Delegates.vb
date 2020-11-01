Imports System.Runtime.InteropServices
Imports RDotNet.Utilities
Imports UnixRStart = RDotNet.Internals.Unix.RStart
Imports WindowsRStart = RDotNet.Internals.Windows.RStart

Namespace Internals
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_setStartTime()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_initialize_R(ByVal ac As Integer, ByVal argv As String()) As Integer

    ''' <summary>
    ''' Necessary to call at initialization on Windows, otherwise some (all?) of the startup parameters are NOT picked
    ''' </summary>
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub cmdlineoptions(ByVal ac As Integer, ByVal argv As String())
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_set_command_line_arguments(ByVal argc As Integer, ByVal argv As String())
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_DefParams_Unix(ByRef start As UnixRStart)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_DefParams_Windows(ByRef start As WindowsRStart)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_SetParams_Unix(ByRef start As UnixRStart)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_SetParams_Windows(ByRef start As WindowsRStart)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub setup_Rmainloop()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_ReplDLLinit()

    ''' <summary>
    ''' Initialise R for embedding
    ''' </summary>
    ''' <param name="argc">The length of argv</param>
    ''' <param name="argv">arguments passed to the embedded engine</param>
    ''' <remarks>
    ''' <code>
    ''' int Rf_initEmbeddedR(int argc, char **argv)
    ''' {
    '''    Rf_initialize_R(argc, argv);
    '''   // R_Interactive is set to true in unix Rembedded.c, not gnuwin
    '''    R_Interactive = TRUE;  /* Rf_initialize_R set this based on isatty */
    '''    setup_Rmainloop();
    '''    return(1);
    ''' }
    ''' </code>
    ''' </remarks>
    ''' <returns></returns>
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_initEmbeddedR(ByVal argc As Integer, ByVal argv As String()) As Integer
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub Rf_endEmbeddedR(ByVal fatal As Integer)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_RunExitFinalizers()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub Rf_CleanEd()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_CleanTempDir()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_protect(ByVal sexp As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_PreserveObject(ByVal sexp As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_ReleaseObject(ByVal sexp As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub Rf_unprotect(ByVal count As Integer)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub Rf_unprotect_ptr(ByVal sexp As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_install(ByVal s As String) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_mkString(ByVal s As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_mkChar(ByVal s As String) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_asCharacterFactor(ByVal sexp As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_allocVector(ByVal type As SymbolicExpressionType, ByVal length As Integer) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_coerceVector(ByVal sexp As IntPtr, ByVal type As SymbolicExpressionType) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isVector(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isFrame(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isS4(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_length(ByVal sexp As IntPtr) As Integer
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_allocMatrix(ByVal type As SymbolicExpressionType, ByVal rowCount As Integer, ByVal columnCount As Integer) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isMatrix(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_nrows(ByVal sexp As IntPtr) As Integer
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_ncols(ByVal sexp As IntPtr) As Integer
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_allocList(ByVal length As Integer) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isList(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_eval(ByVal statement As IntPtr, ByVal environment As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function R_tryEval(ByVal statement As IntPtr, ByVal environment As IntPtr, <Out> ByRef errorOccurred As Boolean) As IntPtr

    ''' <summary>
    ''' A delegate for the R native Rf_PrintValue function
    ''' </summary>
    ''' <param name="value">Pointer to a symbolic expression</param>
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Public Delegate Sub Rf_PrintValue(ByVal value As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function R_ParseVector(ByVal statement As IntPtr, ByVal statementCount As Integer, <Out> ByRef status As ParseStatus, ByVal __ As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_findVar(ByVal name As IntPtr, ByVal environment As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub Rf_setVar(ByVal name As IntPtr, ByVal value As IntPtr, ByVal environment As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub Rf_defineVar(ByVal name As IntPtr, ByVal value As IntPtr, ByVal environment As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_getAttrib(ByVal sexp As IntPtr, ByVal name As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_setAttrib(ByVal sexp As IntPtr, ByVal name As IntPtr, ByVal value As IntPtr) As IntPtr

    'SEXP R_do_slot(SEXP obj, SEXP name);
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function R_do_slot(ByVal sexp As IntPtr, ByVal name As IntPtr) As IntPtr

    'SEXP R_do_slot_assign(SEXP obj, SEXP name, SEXP value);
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function R_do_slot_assign(ByVal sexp As IntPtr, ByVal name As IntPtr, ByVal value As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function R_getClassDef(ByVal what As String) As IntPtr

    'int R_has_slot(SEXP obj, SEXP name)
    Friend Delegate Function R_has_slot(ByVal sexp As IntPtr, ByVal name As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub SET_TAG(ByVal sexp As IntPtr, ByVal tag As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isEnvironment(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isExpression(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isSymbol(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isLanguage(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isFunction(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isFactor(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_isOrdered(ByVal sexp As IntPtr) As Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function R_lsInternal(ByVal environment As IntPtr, ByVal all As Boolean) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_applyClosure(ByVal [call] As IntPtr, ByVal value As IntPtr, ByVal arguments As IntPtr, ByVal environment As IntPtr, ByVal suppliedEnvironment As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_VectorToPairList(ByVal sexp As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_allocSExp(ByVal type As SymbolicExpressionType) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_NewEnvironment(ByVal names As IntPtr, ByVal values As IntPtr, ByVal parent As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_cons(ByVal sexp As IntPtr, ByVal [next] As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function Rf_lcons(ByVal sexp As IntPtr, ByVal [next] As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_gc()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function STRING_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function VECTOR_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function INTEGER_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr) As Integer
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub SET_INTEGER_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr, ByVal value As Integer)

    'R_xlen_t INTEGER_GET_REGION(SEXP sx, R_xlen_t i, R_xlen_t n, int* buf);
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function INTEGER_GET_REGION(ByVal sx As IntPtr, ByVal i As IntPtr, ByVal n As IntPtr, ByVal buf As Integer()) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function REAL_GET_REGION(ByVal sx As IntPtr, ByVal i As IntPtr, ByVal n As IntPtr, ByVal buf As Double()) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function DATAPTR_OR_NULL(ByVal sexp As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function LOGICAL_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr) As Integer
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub SET_LOGICAL_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr, ByVal value As Integer)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function REAL_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr) As Double
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub SET_REAL_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr, ByVal value As Double)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function COMPLEX_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr) As Rcomplex
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub SET_COMPLEX_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr, ByVal value As Rcomplex)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function RAW_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr) As Byte
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub SET_RAW_ELT(ByVal sexp As IntPtr, ByVal index As IntPtr, ByVal value As Integer)
End Namespace
