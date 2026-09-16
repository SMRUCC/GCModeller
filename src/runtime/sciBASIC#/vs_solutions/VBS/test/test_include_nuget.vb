' =============================================================
' demo: #include 引入 nuget 程序包
'
'   vbs ./test/test_include_nuget.vb
'
' 语法:
'   #include "nuget-package-name"            ' 自动解析最新稳定版
'   #include "nuget-package-name@version"    ' 指定版本或版本范围
'
' 引擎会解析包内 .nuspec 声明的依赖并递归解析全部传递依赖;
' 包被解压到本地缓存 ~/.nuget/packages/<id>/<version>/, 二次运行不再联网。
' 下面是显式指定版本, 并通过包含传递依赖的包验证依赖自动解析。
' =============================================================
#include "Newtonsoft.Json@13.0.3"
#include "System.Text.Json@8.0.5"

Imports Newtonsoft.Json

Dim person As New Newtonsoft.Json.Linq.JObject()

person("name") = "asuka"
person("age") = 18
person("tags") = New Newtonsoft.Json.Linq.JArray({"vbs", "script"})

Call Console.WriteLine("---------- Newtonsoft.Json (无依赖) ----------")
Call Console.WriteLine(JsonConvert.SerializeObject(person, Formatting.Indented))

Call Console.WriteLine("---------- System.Text.Json (含传递依赖) ----------")
Call Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(New With {.ok = True, .value = 42}))

Call Console.WriteLine("---------- 解析出的程序集 ----------")
Call Console.WriteLine("Includes() = " & Includes().Length & " 项")

For Each dll In Includes()
    Call Console.WriteLine("    -> " & dll)
Next
