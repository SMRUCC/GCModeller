Public Module Debugger

    Sub Main()

        Dim Script As String =
"# ::表示命名空间和函数之间的分隔，当没有导入命名空间的时候需要使用  命名空间::函数 的引用方式来调用函数  
var <- namespace::function arg1, arg2, arg3                         #使用逗号的时候变量的排列顺序必须要和函数的定义一致
var = $var -> namespace::function param1 arg1 param2 arg2 /param3   #直接使用空格来分隔的时候顺序的位置可以不一致，并且可以使用开关来表示Boolean类型的参数值为真
Call $var -> namespace::function param1 arg1 /param2                #进行方法调用必须要使用Call开头                            

Dim variable = {expression} As type                                 #变量variable的类型固定为Type类型，变量的定义使用Dim开始
Dim variable = {expression}                                         #变量类型为Object类型
Dim variable = {expression} As object                               #这一句变量的申明语句和上一个语句的含义相同

For i in <collection> => {Delegate(i)}
Do While <expression> => {Delegate(expression)}
If <expression>       => {Delegate(expression)}
ElseIf <expression>   => {Delegate(expression)}
Else                  => {Delegate(expression)}"

        Dim ScriptObject = Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.ScriptModel.ScriptParser(Script, "/home/xieguigang/test.sh")

    End Sub

End Module
