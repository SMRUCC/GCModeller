Imports System.Runtime.Serialization
Imports RDotNet.Internals

' (http://msdn.microsoft.com/en-us/library/vstudio/system.applicationexception%28v=vs.110%29.aspx)
' "If you are designing an application that needs to create its own exceptions,
' you are advised to derive custom exceptions from the Exception class"

''' <summary>
''' Thrown when an engine comes to an error.
''' </summary>
<Serializable>
Public Class ParseException
    Inherits Exception

    Private Const StatusFieldName As String = "status"
    Private Const ErrorStatementFieldName As String = "errorStatement"
    Private ReadOnly errorStatementField As String
    Private ReadOnly statusField As ParseStatus

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
    Public Sub New(ByVal status As ParseStatus, ByVal errorStatement As String, ByVal errorMsg As String)
        MyBase.New(MakeErrorMsg(status, errorStatement, errorMsg))
        statusField = status
        errorStatementField = errorStatement
    End Sub

    Private Shared Function MakeErrorMsg(ByVal status As ParseStatus, ByVal errorStatement As String, ByVal errorMsg As String) As String
        Return String.Format("Status {2} for {0} : {1}", errorStatement, errorMsg, status)
    End Function

    ''' <summary>
    ''' Creates a new ParseException
    ''' </summary>
    ''' <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialised object data about the exception being thrown.</param>
    ''' <param name="context"></param>
    Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.New(info, context)
        statusField = CType(info.GetValue(StatusFieldName, GetType(ParseStatus)), ParseStatus)
        errorStatementField = info.GetString(ErrorStatementFieldName)
    End Sub

    ''' <summary>
    ''' The error.
    ''' </summary>
    Public ReadOnly Property Status As ParseStatus
        Get
            Return statusField
        End Get
    End Property

    ''' <summary>
    ''' The statement caused the error.
    ''' </summary>
    Public ReadOnly Property ErrorStatement As String
        Get
            Return errorStatementField
        End Get
    End Property

    ''' <summary>
    ''' Sets the serialization info about the exception thrown
    ''' </summary>
    ''' <param name="info">Serialised object data.</param>
    ''' <param name="context">Contextual information about the source or destination</param>
    Public Overrides Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.GetObjectData(info, context)
        info.AddValue(StatusFieldName, statusField)
        info.AddValue(ErrorStatementFieldName, errorStatementField)
    End Sub
End Class

