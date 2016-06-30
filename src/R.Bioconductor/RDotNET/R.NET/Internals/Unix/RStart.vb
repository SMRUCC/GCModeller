Imports System.Runtime.InteropServices

Namespace Internals.Unix
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure RStart
		<MarshalAs(UnmanagedType.Bool)> _
		Public R_Quiet As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public R_Slave As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public R_Interactive As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public R_Verbose As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public LoadSiteFile As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public LoadInitFile As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public DebugInitFile As Boolean

		Public RestoreAction As StartupRestoreAction
		Public SaveAction As StartupSaveAction
		Friend vsize As UIntPtr
		Friend nsize As UIntPtr
		Friend max_vsize As UIntPtr
		Friend max_nsize As UIntPtr
		Friend ppsize As UIntPtr

		<MarshalAs(UnmanagedType.Bool)> _
		Public NoRenviron As Boolean
	End Structure
End Namespace
