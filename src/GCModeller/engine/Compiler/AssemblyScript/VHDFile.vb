Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Commands
Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript

    Public Class VHDFile

        ''' <summary>
        ''' build from a base model
        ''' </summary>
        ''' <returns></returns>
        Public Property base As From
        Public Property metadata As Label()
        Public Property maintainers As Maintainer()
        Public Property keywords As Keywords()
        Public Property environment As Env()
        Public Property modifications As Modification()

        Public Shared Function Parse(script As String) As VHDFile
            Dim scanner As New Scanner(script.SolveStream)
            Dim tokenList = scanner.GetTokens.ToArray
            Dim assemblyCode As Token() = tokenList.Where(Function(a) Not a.name = Tokens.comment).ToArray
            Dim directive = assemblyCode _
                .Split(Function(a) a.name = Tokens.keyword, DelimiterLocation.NextFirst) _
                .Where(Function(a) a.Length > 0) _
                .GroupBy(Function(a) a(Scan0).text) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function)
            Dim vhd As New VHDFile

            If Not directive.ContainsKey("FROM") Then
                Throw New DataException("no base model!")
            ElseIf directive("FROM").Length > 1 Then
                Throw New DataException("duplicated base model")
            Else
                vhd.base = New From(directive("FROM").First)
            End If

            If Not directive.ContainsKey("MAINTAINER") Then
                vhd.maintainers = {New Maintainer With {.authorName = My.User.Name}}
            Else
                vhd.maintainers = directive("MAINTAINER") _
                    .Select(Function(a) New Maintainer(a)) _
                    .ToArray
            End If

            vhd.keywords = directive _
                .TryGetValue("KEYWORDS") _
                .SafeQuery _
                .Select(Function(a) New Keywords(a)) _
                .ToArray
            vhd.metadata = directive _
                .TryGetValue("LABEL") _
                .SafeQuery _
                .Select(Function(a) New Label(a)) _
                .ToArray
            vhd.environment = directive _
                .TryGetValue("ENV") _
                .SafeQuery _
                .Select(Function(a) New Env(a)) _
                .ToArray

            Return vhd
        End Function

    End Class
End Namespace