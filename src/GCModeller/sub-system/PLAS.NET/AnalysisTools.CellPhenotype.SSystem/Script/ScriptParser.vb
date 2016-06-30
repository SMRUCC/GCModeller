Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.Linq
Imports LANS.SystemsBiology.AnalysisTools.CellPhenotype.SSystem.Kernel.ObjectModels

Namespace Script

    Public Module ScriptParser

        Public Function sEquationParser(line As String) As SEquation

        End Function

        Public Function ExperimentParser(line As String) As Experiment
            Dim Tokens As String() = line.Split
            Dim Dict As New Dictionary(Of String, String)
            Dim Disturb As New Experiment

            For i As Integer = 1 To Tokens.Length - 2 Step 2
                Dict.Add(Tokens(i), Tokens(i + 1))
            Next

            Disturb.Start = Val(Dict("START"))
            Disturb.Interval = Val(Dict("INTERVAL"))
            Disturb.Kicks = Val(Dict("KICKS"))
            Disturb.Id = Dict("OBJECT")
            Dim Value As String = Dict("VALUE")

            If InStr(Value, "++") = 1 Then
                Disturb.DisturbType = Types.Increase
                Disturb.Value = Val(Mid(Value, 3))
            ElseIf InStr(Value, "--") = 1 Then
                Disturb.DisturbType = Types.Decrease
                Disturb.Value = Val(Mid(Value, 3))
            Else
                Disturb.DisturbType = Types.ChangeTo
                Disturb.Value = Val(Value)
            End If

            Return Disturb
        End Function

        Public Function ParseFile(path As String) As Model
            Dim lines As String() = path.ReadAllLines
            Dim tokens As Token(Of Tokens)() = TokenIcer.TryParse(lines)
            Dim typeTokens = (From x As Token(Of Tokens)
                              In tokens
                              Select x
                              Group x By x.Type Into Group) _
                                   .ToDictionary(Function(x) x.Type,
                                                 Function(x) x.Group.ToArray)

            Dim equations = typeTokens(Script.Tokens.Reaction).ToArray(Function(x) sEquationParser(x.Text))
            Dim inits = typeTokens(Script.Tokens.InitValue).ToArray(Function(x) CType(x.Text, Var))
            Dim Disturbs As Experiment() = typeTokens(Script.Tokens.Disturb).ToArray(Function(x) ExperimentParser(x.Text))
            Dim FinalTime As Integer

            If Not typeTokens.ContainsKey(Script.Tokens.Time) Then
                FinalTime = 100
            Else
                FinalTime = Val(typeTokens(Script.Tokens.Time).First.Text)
            End If

            Dim Title As String

            If Not typeTokens.ContainsKey(Script.Tokens.Title) Then
                Title = "UNNAMED TITLE"
            Else
                Title = typeTokens(Script.Tokens.Title).First.Text
            End If

            Dim Comments As String() = typeTokens(Script.Tokens.Comment).ToArray(Function(x) x.Text)
            Dim NameList As String() = typeTokens(Script.Tokens.Alias).ToArray(Function(x) x.Text)
            Dim model As Model = New Model With {
                .sEquations = equations,
                .Vars = inits,
                .Experiments = Disturbs,
                .Comment = Comments.JoinBy(vbCrLf),
                .FinalTime = FinalTime,
                .Title = Title
            }

            For Each s As String In NameList
                s = Mid(s, 7)
                Dim Name As String = s.Split.First
                model.FindObject(s).Title = Mid(s, Len(Name) + 2)
            Next

            For Each s As String In typeTokens(Script.Tokens.SubsComments).ToArray(Function(x) x.Text)
                s = Mid(s, 9)
                Dim Name As String = s.Split.First
                model.FindObject(Name).Comment = Mid(s, Len(Name) + 2)
            Next

            model.UserFunc = typeTokens(Script.Tokens.Function).ToArray(Function(x) CType(x.Text, [Function]))
            model.Constant = typeTokens(Script.Tokens.Constant).ToArray(Function(x) ScriptParser.ConstantParser(x.Text))

            Return model
        End Function

        Public Function ConstantParser(expr As String) As Constant
            Dim name As String = expr.Trim.ShadowCopy(expr).Split.First
            expr = Mid(expr, name.Length + 1).Trim
            Return New Constant With {
                .Expression = expr,
                .Name = name
            }
        End Function
    End Module
End Namespace