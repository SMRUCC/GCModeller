Imports System.Text

Namespace Runtime.Objects.ObjectModels.Exceptions

    ''' <summary>
    ''' One or more circle reference was detected from the include list.(在多个脚本之间使用include语句产生了循环引用)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CircularReferencesException : Inherits ShoalScriptException

        Public Property IncludeList As String()

        Sub New()
            Call MyBase.New("Cycle Reference in the script! One or more circle reference was detected from the include list!")
        End Sub

        Public Overrides ReadOnly Property ExceptionType As String
            Get
                Return GetType(CircularReferencesException).FullName
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(MyBase.ToString)
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine("Include List:")
            For Each item In IncludeList
                Call sBuilder.AppendLine(item)
            Next

            Dim CycleIncluded = (From item In (From strLine As String
                                               In IncludeList
                                               Select FileIO.FileSystem.GetFileInfo(strLine).FullName
                                               Group FullName By FullName Into Group).ToArray
                                 Where item.Group.Count > 1
                                 Select item).ToArray

            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine("Cycle Reference at: ")
            For Each Line In CycleIncluded
                Call sBuilder.AppendLine(Line.FullName)
            Next

            Return sBuilder.ToString
        End Function
    End Class
End Namespace