#Disable Warning

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Configuration

Module Debugger

    'Interface aa
    '    Property b

    'End Interface

    'Interface b : Inherits aa
    '    Property c
    'End Interface

    Sub Main()

        Call Interpreter.LDM.SyntaxModel.LoadFile("S:\TSSs\mmx.txt")

        Dim ddddftt = HTML.TypeLinks.PageName(GetType(KeyValuePair(Of String, String)))

        Dim mslScript As String = "dim nn <= {
a,
b,
c,
d,
e,
""a b c d f h""
}
*T <= {
imports test
return rand
}"

        Dim scrr = Interpreter.Interpreter.MSLParser(mslScript)

        Dim lines As String() = Strings.Split(mslScript, vbCrLf)
        Dim parser As New Interpreter.Parser.TextTokenliser.MSLTokens
        Dim pppp As i32

        Call parser.Parsing(lines(++pppp))

        Do While Not parser.FinishYet

            Call parser.Parsing(lines(++pppp))

        Loop



        Dim sssDb = SPM.PackageModuleDb.Load(SPM.PackageModuleDb.DefaultFile)
        Dim vbc = New Compiler.VBC(sssDb)
        Call vbc.Compile("I:\Shoal\Shoal.v2\Console\bin\Debug\TestScript\HelloWorld.txt", "./test.exe")


        '    Dim ghghgh = Interpreter.SyntaxParser.Parsing("dim ff <= {$g, ""gh $g +$gh\$g""}")

        Dim tt = InputHandler.GetType("int32")

        Dim ssss As String = ""
        Dim nnnn = CTypeDynamic(ssss, GetType(Boolean))

        Dim aa = New Object() {}
        Dim bb = New String() {"444"}

        aa = bb

        Dim aaa = New List(Of Object)
        Dim bbb = New List(Of String)

        '  aaa = bbb   '除了泛型的集合，其他的搜可以转换到基本类型

        Dim cc As IEnumerable(Of Object)
        Dim dd As IEnumerable(Of String)

        cc = dd


        Dim a = GetType(Object)
        Dim b = GetType(Object)

        Console.WriteLine(Interpreter.Linker.APIHandler.Alignment.TypeEquals.TypeEquals(a, b))

        'a = GetType(aa)
        'b = GetType(Debugger.b)

        'Console.WriteLine(Interpreter.Linker.APIHandler.Alignment.TypeEquals.TypeEquals(a, b))


        a = GetType(Interpreter.Parser.Tokens.Token)
        b = GetType(Interpreter.Parser.Tokens.InternalExpression)
        Console.WriteLine(Interpreter.Linker.APIHandler.Alignment.TypeEquals.TypeEquals(a, b))

        a = GetType(Object())
        b = GetType(String())

        Console.WriteLine(Interpreter.Linker.APIHandler.Alignment.TypeEquals.TypeEquals(a, b))


        a = GetType(Generic.IEnumerable(Of String))
        b = GetType(List(Of String))

        Console.WriteLine(Interpreter.Linker.APIHandler.Alignment.TypeEquals.TypeEquals(a, b))

        a = GetType(String())
        b = GetType(List(Of String))

        Console.WriteLine(Interpreter.Linker.APIHandler.Alignment.TypeEquals.TypeEquals(a, b))


        Dim db = Scripting.ShoalShell.SPM.PackageModuleDb.Load(Scripting.ShoalShell.SPM.PackageModuleDb.DefaultFile)
        Dim TestLoader = Scripting.ShoalShell.SPM.Nodes.AssemblyParser.LoadAssembly(
            "I:\Shoal\Shoal.v2\Shoal.v2\bin\Debug\Microsoft.VisualBasic.Architecture.Framework_v3.0_22.0.76.201__8da45dcd8060cc9a.dll")

        Using spm = New SPM.ShoalPackageMgr(db)
            Call spm.MergeNamespace(TestLoader)
            Call spm.UpdateDb()

            Dim pTest = New KeyValuePair(Of String, Object)() {
                New KeyValuePair(Of String, Object)("dead", False),
                New KeyValuePair(Of String, Object)("msg", "yes")
            }
            Dim score = Interpreter.Linker.APIHandler.Alignment.FunctionCalls.OverloadsAlignment(spm.Item("cowsay").GetEntryPoint("cowsay").OverloadsAPI(0).EntryPoint, pTest)

            Call Console.WriteLine(score)

        End Using

        '       Dim src = Scripting.ShoalShell.Interpreter.SyntaxParser.Parsing("Dim ab <- 123 As Integer")

        Dim ScriptEngine = New ShoalShell.Runtime.ScriptEngine(Config.Default.SettingsData)
        Dim value = ScriptEngine.Exec("Dim ab <- 123 As Integer")

        value = ScriptEngine.Exec("
dddd < (string()) ""I:\Shoal\Shoal.v2\Shoal.v2\bin\Debug\tesssss.dat""

Imports cowsay
Dim msg <- ""Hello world!"" as string
Dim varTest = 3333333 
 dim array <= {1, 2, 3, 4, 5, 6, 7, 8} as integer
dim matrix <= {$array, $array, $array, $array}
 dim matarray <= {$matrix, $matrix, $matrix}

$array > ./tesssss.dat
dim bytes <= {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 250} as byte
$bytes > ./tesbytessss.dat

 dim ddd <= $array [3]

vartest <- $msg -> cowsay /dead
*4 <- $vartest -> cowsay /dead
$msg > ./testy.txt")

        value = ScriptEngine.Strings.Format("------$msggg----&HOME\$msg------------")


        value = ScriptEngine.Exec("
Imports GDI+
./112649252034fea52ao.jpg -> GrayBitmap
# call $ -> WriteTo ./test.png
call $ -> WriteTo path.save ./test_path2.png")


        Dim Script As String =
"
$Msgbox
Msgbox
String.IsEmpty arg1
die ""nothing test"" when {FALSE}

# 系统保留的命令行函数Command，和VB一样的
var args      <- Command          As CommandLine #die ""null exception!"" when {args is nothing}

a << ""1 + 2""
b >> ef

# 关于 => 运算符的使用
# 当左端的词元为变量的时候表示对字典对象的引用
# 其他的情况则为函数指针
#   testVar    =  $args => --ssl
var open      <- {$args => /open} As String
var auto-flush = {$args => /auto} As Boolean

# 测试1.  方法调用
# ::表示命名空间和函数之间的分隔，当没有导入命名空间的时候需要使用  命名空间::函数 的引用方式来调用函数  
       var <- namespace::function arg1, arg2, arg3                  #使用逗号的时候变量的排列顺序必须要和函数的定义一致
var = $var -> namespace::function param1 arg1 param2 arg2 /param3   #直接使用空格来分隔的时候顺序的位置可以不一致，并且可以使用开关来表示Boolean类型的参数值为真
Call  $var -> namespace::function param1 arg1 /param2               #进行方法调用必须要使用Call开头         

RE:

Call function param1 arg1 --param2 

die ""xxxxxx empty"" when {String.IsEmpty arg1}        #返回值是Nothing

On Error Resume Next

Goto RE
Goto RE When {function $var2}

Dim variable = {expression} As type                                 #变量variable的类型固定为Type类型，变量的定义使用Dim开始
Dim variable = {expression}                                         #变量类型为Object类型
Dim variable = {expression} As object                               #这一句变量的申明语句和上一个语句的含义相同

#For i in <collection>  => {Delegate(i)}
#Do While <expression>  => {Delegate(expression)}
#If       <expression>  => {Delegate(expression)}
#ElseIf   <expression>  => {Delegate(expression)}
#Else                   => {Delegate(expression)}"

        Dim ScriptObject = Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.SyntaxModel.ScriptParser(Script, "/home/xieguigang/test.sh")
    End Sub
End Module
