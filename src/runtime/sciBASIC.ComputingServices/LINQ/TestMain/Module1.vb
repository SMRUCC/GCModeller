#Region "Microsoft.VisualBasic::b5b73d4049cd06306a2eb6ef987c504b, ..\sciBASIC.ComputingServices\LINQ\TestMain\Module1.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.Linq.Framework.Provider
Imports Microsoft.VisualBasic.Linq.Framework.Provider.ImportsAPI
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode
Imports Microsoft.VisualBasic.Linq.LDM.Statements.Tokens
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Linq.Extensions

Module Module1

    Sub Main()





        '     Dim a As Double()
        '     Dim b As Double()
        '     Dim llll = (From x In a
        '    Join y In b
        '   On (x / 6) Equals (y Mod 3 = 5)
        '  Where (x + y) < 0
        ' Select Case x, y)

        Dim tttlquery = (From line As String
                             In IO.File.ReadAllLines("X:\New Text Document.txt")
                         Let tokens = (From ss As String In line.Trim.Split Where Not String.IsNullOrEmpty(ss) Select ss).ToArray
                         Select tokens).ToArray

        For Each line In tttlquery

            For Each x In line
                Call Console.Write(x & vbTab)
            Next
            Call Console.WriteLine()

        Next


        Dim cp = Microsoft.VisualBasic.Linq.Framework.DynamicCode.DynamicCompiler.DefaultCompiler
        Dim test2222 = LDM.Statements.Tokens.WhereClosure.CreateLinqWhere("$x mod 6=0", GetType(Integer), cp)

        test2222(6).__DEBUG_ECHO
        test2222(12).__DEBUG_ECHO
        test2222(13).__DEBUG_ECHO


        Dim run As New Script.DynamicsRuntime
        Call run.SetObject("test1", {12, 36, 34, 7865, 84, 34, 6, 12, 9, 8})

        Dim lllll As String = "from x  as integer in $test1 where x > 100 select x, xx=x^2"
        Dim resultffff = (From x In run.EXEC(lllll) Select x).ToArray

        'Dim RQLQuery = (From x As Integer
        '                In New RQL.API.Repository(Of Integer)("http://127.0.0.1/int32").Where("$x mod 6 = 1").AsLinq(Of Integer)
        '                Where x > 100 Select x)

        'Dim query2 = (From x As Integer
        '              In New RQL.API.Repository(Of Integer)("http://127.0.0.1/int32").Where(Function(xx) xx Mod 6 = 1).AsLinq(Of Integer)
        '              Where x > 100
        '              Select x)

        ''   Dim query3 = (From x As Integer
        ''                In New RQL.API.Repository(Of Integer)("http://127.0.0.1/int32?where=$ mod6 =1")
        '' Where x > 100
        ''Select Case x)
        'Dim id, number
        'Dim SQL As String = $"update table set id={id} where uid={number}"
        Call RunTask(Sub()
                         MsgBox("click to start!")

                         Dim qlll = (From x As Integer In New RQL.API.Repository(Of Integer)("http://127.0.0.1/test123.vb").Where("$x mod 6 =1").AsLinq(Of Integer) Select Time(Sub() x.__DEBUG_ECHO)).ToArray


                         MsgBox("test2")


                         Dim lquerrrr = (From i As Integer In 1000000.SeqIterator.AsParallel Select "http://127.0.0.1/test123.vb".GetRequest.__DEBUG_ECHO).ToArray
                         MsgBox("DONE!")
                         For i As Integer = 0 To 100000
                             Call Trace.WriteLine("http://127.0.0.1/test123.vb".GET)
                         Next
                     End Sub)

        Dim svr As New RQL.RESTProvider
        Call svr.AddLinq("/test123.vb", "E:\Microsoft.VisualBasic.Parallel\trunk\LINQ\ints.txt", AddressOf Microsoft.VisualBasic.Linq.Framework.Provider.GetInt32)
        Call svr.Run()

        Dim stststs = LDM.Statements.LinqStatement.TryParse( _
 _
            "From x As Integer In ""E:\Microsoft.VisualBasic.Parallel\trunk\LINQ\ints.txt"" Let add = x + 50 Where add > 0 Let cc = add ^ 2 let abc as double = cc mod 99 +11.025R Select abc, cc, x, add, nn = cc+ x/ add * 22 mod 5, gg = math.max(cc,add)")

        Dim runt As New Script.DynamicsRuntime
        Dim result = (From x In runt.EXEC(stststs) Select x).ToArray

        Dim source = {1, 2, 3, 4, 5, 6, 7}

        Dim LQuery = (From x As Integer In source Let add = x + 50 Where add > 0 Let cc = add ^ 2 Select cc, x, add, nn = New Double() {cc, x, add * 22}.Sum)


        Dim code As String = LinqClosure.BuildClosure("x", GetType(Integer), {"add = x + 50 "}, {"cc = add ^ 2"}, {"cc", "x", "add", "nn = cc+ x+ Add * 22"}, "add > 0")

        Call Console.WriteLine(code)

        Dim compiler As New Framework.DynamicCode.DynamicCompiler

        Dim ttttttttttt = compiler.Compile(code)

        Dim aa = ttttttttttt(1000000000)



        Dim itt As New Iterator(source)

        Do While itt.MoveNext
            Call __DEBUG_ECHO(Scripting.ToString(itt.Current) & " --> " & itt.ReadDone)
        Loop
        Call Scripting.ToString(itt.Current).__DEBUG_ECHO

        Dim s As String = "instr($s, cstr( $s->length), 8)"
        Dim typew = GetType(String)
        '     Dim www = WhereClosure.CreateLinqWhere(s, typew)
        Dim types As TypeRegistry = TypeRegistry.LoadDefault
        Dim api As APIProvider = APIProvider.LoadDefault


        Dim p As New Parser.Parser
        Dim n = p.ParseExpression("$($(test2 pp $rt) -> test_func par1 $ffjhg par2 $dee) -> test3 p3 $($(test5 de) -> test4 ppp $gr)")
    End Sub
End Module

