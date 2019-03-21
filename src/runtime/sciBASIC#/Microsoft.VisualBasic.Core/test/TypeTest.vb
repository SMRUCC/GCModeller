Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Module TypeTest

    Sub Main()
        Dim o As UnionType(Of String, Integer(), Char())

        o = "string"

        Console.WriteLine(CStr(o))
        Console.WriteLine(o Like GetType(String))
        Console.WriteLine(o Like GetType(Integer()))
        Console.WriteLine(o Like GetType(Char()))

        o = {1, 2, 1231, 31, 23, 12}
        Console.WriteLine(CType(o, Integer()).GetJson)
        Console.WriteLine(o Like GetType(String))
        Console.WriteLine(o Like GetType(Integer()))
        Console.WriteLine(o Like GetType(Char()))

        o = {"f"c, "s"c, "d"c, "f"c, "s"c, "d"c, "f"c, "s"c, "d"c, "f"c, "s"c, "d"c, "f"c}
        Console.WriteLine(CType(o, Char()).GetJson)
        Console.WriteLine(o Like GetType(String))
        Console.WriteLine(o Like GetType(Integer()))
        Console.WriteLine(o Like GetType(Char()))

        Pause()
    End Sub
End Module
