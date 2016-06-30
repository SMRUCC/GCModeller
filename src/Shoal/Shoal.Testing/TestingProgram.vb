Imports System.Dynamic
Imports NODE = System.Collections.Generic.KeyValuePair(Of Integer, Char)

Module TestingProgram

    Sub Main()

        Dim normal_test = "Call {set Z [test1 ui {FALSE}] $var new_value }     <-  OK!!! ->   sum {Test.Generate var1 1234} ""a b {c} d( e f g""  var2 {(double) 33,44,55,66} varXX ""{}{}{}}{}}{  {}{  {}{ {}}}[][][[[[[]]"" var3 [(string()) 1,2,3,4,5]"
        normal_test = "1 = [2 ef {abc} gg] + 2"
        normal_test = "test_invoke {imports lalal#alalla}   #Imports the namespace example here"
        Dim expr = Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.SyntaxParser.Parsing(normal_test)

        Dim script = "imports lalalalalla   #Imports the namespace example here
# this is the comments

library test/path/value
include {test/path value value2 var s}  #all of the elements can be dynamics

goto {44 * $var2} when {testddd sdfsd 1234} # the foto tag can be dynamics, maybe the everything in the script elements can be dynamics! 

imports gcmodeller.compiler   #Imports the namespace example here

savedfile <- {drawing from ""E:\Desktop\xcb_vcell\xcb_model\trunk\xcb.xml""}

        chipdata <- "" E\Desktop\xcb_vcell\chipData_analysis\r_script\xcbChip.csv""

metacyc <- E:\BLAST\db\MetaCyc\Xanthomonas_campestris_pv._campestris_str._8004\17.0\data
regprecise <- E:\Desktop\xcb_vcell\Regprecise_TranscriptionFactors_By_Genome.xml
# transcript_regulation <- ""E\Desktop\xcb_vcell\xcb_metabolism_regulations.csv""

                            transcript_regulation <- ""E\Desktop\xcb_vcell\Result\Metabolism.MEME_Filted_0.65\metacyc.pathways_450_0.65.csv""
44:   #just for some test!
                                                      mist2 <- E:\Desktop\xcb_vcell\xcb_mist2.xml
mist2_strp_xml <- ""E\Desktop\xcb_vcell\xcb_mist2_strp.xml""
                   string-db <- ""E\Desktop\xcb_vcell\xcb_string.xml""

                                 ptt_dir <- E:\Desktop\xcb_vcell\Xanthomonas_campestris_8004_uid15

myva_cog <- E:\Desktop\xcb_vcell\xcb_myva_COG.csv
door <- ""E:\Desktop\xcb_vcell\xcb_door.opr""

         array <- array.new ""$myva_cog,$ptt_dir,$door,$chipdata""
                   argvs <- string.format expression "" -compile -myva_cog "" {0}"" -ptt_dir ""{1}"" -door ""{2}"" -chipdata ""{3}"""" argvs $array

$argvs

#compiler <- gcmodeller.compiler -precompile metacyc $metacyc regprecise_regulator_xml $regprecise transcript_regulation $transcript_regulation mist2 $mist2 mist2_strp_xml $mist2_strp_xml stringdb $string-db
compiled_model <- invoke.compile compiler $compiler argvs $argvs
Call write.cellsystem_model model $compiled_model filepath $savedfile

argvs <- array.new $savedfile
conf <- string.format expression {0}.inf argvs $argvs

call gcmodeller.engine_kernel default_configuration $conf"


        script = "imports {system.extensions}
dd <- basename test.dll
call {basename c:\dddd\fhghthddd.exe} -> cowsay ""-type dead"" --yes"


        Dim scriptnfggh = Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.ScriptModel.ScriptParser(script, "")
        Dim shoalshellengine As New Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript
        Dim scriptmodel = New Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels.ExecuteModel(scriptnfggh, shoalshellengine)

        Call scriptmodel.Execute()

        Dim debugger As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Debugging.DebuggerListener = New Scripting.ShoalShell.Runtime.Debugging.DebuggerListener("./Shoal.exe", ".\test-shoal-debugger\")


        Call debugger.PushScript("call ""Hello world"" -> cowsay")
        Call debugger.PushScript("sfhsdkjfhsdjklfjksdf")
        Call debugger.PushScript("library ""../../../../..\ShoalShell\bin\Debug\ShoalShell.Plugins.Win32API_v2.0_22.0.7601.121__ed1d5d0cd8c60cc9a.dll""" & vbCrLf &
                                 "imports winmm.dll" & vbCrLf &
                                "filename <- ""F:\Music\NOKIA.wav""" & vbCrLf &
"call winmm.dll playsounda lpszname $filename hmodule 0 dwflags &SND_FILENAME")

        '  Pause(")")


        Console.ReadLine()

        Call debugger.PushScript("?")

        Console.ReadLine()
        '  debugger.Free()


        End

        Dim test_expression As String = "$($(test2 pp $rt) -> test_func par1 $ffjhg par2 $dee) -> test3 p3 $($(test5 de) -> test4 ppp $gr)"

        Dim hhh = evalue(test_expression)

        '     Dim ex = Microsoft.VisualBasic.Shoal.Runtime.Objects.ObjectModels.ScriptCodeLine.InternalParser(test_expression)

        '   Call Microsoft.VisualBasic.Parallel.LQuerySchedule.PBS_TEST()

        '    Pause()

        'Dim Shl As Object = Microsoft.VisualBasic.Shoal.Runtime.Objects.Dynamics.CreateDefaultEnvironment("E:\GCModeller\CompiledAssembly")

        ''Call Shl.Evaluate("dd <- 1234")
        ''Dim Msg As String = Shl.dd

        ''Call Console.WriteLine("Dynamic variable dd is {0}", Msg)

        ''Call Shl.evaluate("edf <- 33")


        'Call Shl.imports("assemblyfile.io")

        'Dim fasta = Shl.read.fasta("E:\BLAST\db\xcb_prot.fsa")

        'Call Console.WriteLine(fasta.ToString)
        'Call Console.Read()
    End Sub

    Function evalue(expression As String) As Expression
        Dim OPTR As New Stack(Of NODE)
        Dim model = New Expression With {.Tokens = New List(Of Expression)}

        Call OPTR.Push(New NODE(-1, "("c))

        For i As Integer = 0 To expression.Length - 1
            Dim ch = expression(i)

            Call Console.Write(ch)

            If ch <> "("c AndAlso ch <> ")"c Then
                Continue For
            End If

            '下面的过程之中ch变量只有"("或者")"这两个值
            If OPTR.Count = 0 Then
                Exit For
            End If

            Dim c = OPTR.Peek
            Dim p = InternalCompare(c.Value, ch)

            If p < 0 Then
                OPTR.Push(New NODE(i, ch))
                i += 1
                Call model.Tokens.Add(evalue(expression, i, OPTR))
            ElseIf p = 0 Then
                c = OPTR.Pop
                Dim simpletoken = Mid(expression, c.Key + 2, i - c.Key - 1)
                Call model.Add(New Expression With {.TokenExpression = simpletoken})
            End If
        Next

        Return model
    End Function

    Function InternalCompare(op1 As Char, op2 As Char) As Integer
        If op1 = "("c Then
            If op2 = "("c Then
                Return -1
            Else
                Return 0
            End If
        Else
            If op2 = "("c Then
                Return -1
            Else
                Return 1
            End If
        End If
    End Function

    Function evalue(expression As String, ByRef start As Integer, ByRef optr As Stack(Of NODE)) As Expression
        Dim model = New Expression With {.Tokens = New List(Of Expression)}

        For i As Integer = start To expression.Length - 1
            Dim ch = expression(i)

            Call Console.Write(ch)

            If ch <> "("c AndAlso ch <> ")"c Then
                Continue For
            End If

            '下面的过程之中ch变量只有"("或者")"这两个值
            Dim c = optr.Peek
            If c.Key = -1 Then
                '到站定了
                start = i
                Return model
            End If
            Dim p = InternalCompare(c.Value, ch)

            If p < 0 Then
                optr.Push(New NODE(i, ch))
                i += 1
                Call model.Tokens.Add(evalue(expression, i, optr))
            ElseIf p = 0 Then
                c = optr.Pop
                Dim simpletoken = Mid(expression, c.Key + 2, i - c.Key - 1)
                Call model.Add(New Expression With {.TokenExpression = simpletoken})
            End If
        Next

        Return model
    End Function

    ''' <summary>
    ''' 一个表达式
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Expression

        ''' <summary>
        ''' 一个表达式的最简单的一个词元可能是由其他的表达式构成的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Tokens As List(Of Expression)
        ''' <summary>
        ''' 当前的词元，假若本表达式对象是最简单的表达式的话，则本属性即为本表达式的值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TokenExpression As String

        Public Overrides Function ToString() As String
            If Not String.IsNullOrEmpty(TokenExpression) Then
                Return TokenExpression
            Else
                Return "$[ " & String.Join(" ", (From item In Tokens Let s As String = item.ToString Select s).ToArray) & " ]"
            End If
        End Function

        Public Sub Add(ExpressionToken As Expression)
            Call Tokens.Add(ExpressionToken)
        End Sub
    End Structure
End Module
