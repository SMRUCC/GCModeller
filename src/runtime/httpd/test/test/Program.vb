#Region "Microsoft.VisualBasic::8e060370acc485f3b02700b631fdf767, test\test\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 25
    '    Code Lines: 18 (72.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (28.00%)
    '     File Size: 836 B


    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Module Program
    Sub Main(args As String())
        Dim ssfile = Flute.SessionManager.Open("Z:/", "asjkdfshdfjksfs".MD5)

        Console.WriteLine(ssfile.OpenKeyString("abc"))
        Console.WriteLine(ssfile.OpenKeyDouble("abc1"))
        Console.WriteLine(ssfile.OpenKeyInteger("abc2"))

        ssfile.SaveKey("abc", "hello world")
        ssfile.SaveKey("abc1", 111.96)
        ssfile.SaveKey("abc2", 99999)

        Console.WriteLine(ssfile.OpenKeyString("abc"))
        Console.WriteLine(ssfile.OpenKeyDouble("abc1"))
        Console.WriteLine(ssfile.OpenKeyInteger("abc2"))

        ssfile.SaveKey("abc", "hello!~")

        Console.WriteLine(ssfile.OpenKeyString("abc"))

        ssfile.SaveKey("abc", "hello world!!!!")

        Console.WriteLine(ssfile.OpenKeyString("abc"))
    End Sub
End Module

