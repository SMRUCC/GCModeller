Imports RDotNet.Internals
Imports System.Runtime.Serialization

''' <summary>
''' Thrown when an engine comes to an error.
''' </summary>
<Serializable> _
Public Class ParseException
	Inherits Exception
	' (http://msdn.microsoft.com/en-us/library/vstudio/system.applicationexception%28v=vs.110%29.aspx)
	' "If you are designing an application that needs to create its own exceptions,
	' you are advised to derive custom exceptions from the Exception class"
	Private Const StatusFieldName As String = "status"

	Private Const ErrorStatementFieldName As String = "errorStatement"
	Private ReadOnly m_errorStatement As String
	Private ReadOnly m_status As ParseStatus

	''' <summary>
	''' Creates a new instance.
	''' </summary>
	Private Sub New()
		Me.New(ParseStatus.Null, "", "")
		' This does not internally occur. See Parse.h in R_HOME/include/R_ext/Parse.h
	End Sub

	''' <summary>
	''' Creates a new instance with the specified error.
	''' </summary>
	''' <param name="status">The error status</param>
	''' <param name="errorStatement">The statement that failed to be parsed</param>
	''' <param name="errorMsg">The error message given by the native R engine</param>
	Public Sub New(status As ParseStatus, errorStatement As String, errorMsg As String)
		MyBase.New(MakeErrorMsg(status, errorStatement, errorMsg))
		Me.m_status = status
		Me.m_errorStatement = errorStatement
	End Sub

	Private Shared Function MakeErrorMsg(status As ParseStatus, errorStatement As String, errorMsg As String) As String
		Return String.Format("Status {2} for {0} : {1}", errorStatement, errorMsg, status)
	End Function

	''' <summary>
	''' Creates a new ParseException
	''' </summary>
	''' <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
	''' <param name="context"></param>
	Protected Sub New(info As SerializationInfo, context As StreamingContext)
		MyBase.New(info, context)
		Me.m_status = CType(info.GetValue(StatusFieldName, GetType(ParseStatus)), ParseStatus)
		Me.m_errorStatement = info.GetString(ErrorStatementFieldName)
	End Sub

	''' <summary>
	''' The error.
	''' </summary>
	Public ReadOnly Property Status() As ParseStatus
		Get
			Return Me.m_status
		End Get
	End Property

	''' <summary>
	''' The statement caused the error.
	''' </summary>
	Public ReadOnly Property ErrorStatement() As String
		Get
			Return Me.m_errorStatement
		End Get
	End Property

	''' <summary>
	''' Sets the serialization info about the exception thrown
	''' </summary>
	''' <param name="info">Serialized object data.</param>
	''' <param name="context">Contextual information about the source or destination</param>
	Public Overrides Sub GetObjectData(info As SerializationInfo, context As StreamingContext)
		MyBase.GetObjectData(info, context)
		info.AddValue(StatusFieldName, Me.m_status)
		info.AddValue(ErrorStatementFieldName, Me.m_errorStatement)
	End Sub
End Class
