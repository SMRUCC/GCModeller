Imports System.Text.RegularExpressions

Namespace Runtime.Objects.ObjectModels

    ''' <summary>
    ''' ShellScript information header parser
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ScriptInfo

        Public Property Authors As String()
        Public Property Description As String
        Public Property References As String()
        Public Property Version As String
        Public Property MissingReferences As String()
        Public Property ModuleName As String
        Public Property FilePath As String
        Public Property Contacts As String

        Public Const Field As String = "#!\s+{0}: .+"

        Public Function DisplayInfo() As Boolean
            Throw New NotImplementedException
        End Function

        Public Shared Function LoadInfo(path As String, Optional Registry As ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry = Nothing) As ScriptInfo
            Dim Script As ScriptInfo = New ScriptInfo With {.FilePath = path}
            Dim InfoLines As String() = (From strLine As String In IO.File.ReadAllLines(path)
                                         Let Trimed As String = Trim(strLine)
                                         Where Not String.IsNullOrEmpty(Trimed) AndAlso String.Equals(Mid(Trimed, 1, 2), "#!")
                                         Select Trimed).ToArray
            Dim GetValue = Function(Keyword As String) As String()
                               Return (From strLine As String In InfoLines
                                       Let MatchedHead As String = Regex.Match(strLine, String.Format(Field, Keyword), RegexOptions.IgnoreCase).Value
                                       Where Not String.IsNullOrEmpty(MatchedHead)
                                       Select Trim(strLine.Replace(MatchedHead, ""))).ToArray
                           End Function

            Dim strTemp As String()
            strTemp = GetValue("author")
            If Not strTemp.IsNullOrEmpty Then
                Script.Authors = Strings.Split(strTemp.First, "; ")
            End If

            strTemp = GetValue("description")
            If Not strTemp.IsNullOrEmpty Then
                Script.Description = strTemp.First
            End If

            strTemp = GetValue("version")
            If Not strTemp.IsNullOrEmpty Then
                Script.Version = strTemp.First
            End If

            strTemp = GetValue("module name")
            If Not strTemp.IsNullOrEmpty Then
                Script.ModuleName = strTemp.First
            End If

            strTemp = GetValue("contacts")
            If Not strTemp.IsNullOrEmpty Then
                Script.Contacts = strTemp.First()
            End If

            Dim [Imports] As String() = (From strLine As String In IO.File.ReadAllLines(path)
                                         Let Trimed As String = Trim(strLine)
                                         Where Regex.Match(Trimed, "imports\s+", RegexOptions.IgnoreCase).Success
                                         Select Mid(Trimed, 8).Trim).ToArray
            Dim References As List(Of String) = New Generic.List(Of String)
            Dim MissingReferences As List(Of String) = New Generic.List(Of String)

            If Registry Is Nothing Then
                Registry = ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateDefault
            End If

            For Each Ns In [Imports]
                Try
                    Dim [Module] = Registry.GetAssemblyPaths(Ns)
                    Call References.AddRange([Module])
                Catch ex As Exception
                    Call MissingReferences.Add(Ns)
                End Try
            Next

            Script.References = References.ToArray
            Script.MissingReferences = MissingReferences.ToArray

            Return Script
        End Function
    End Class
End Namespace