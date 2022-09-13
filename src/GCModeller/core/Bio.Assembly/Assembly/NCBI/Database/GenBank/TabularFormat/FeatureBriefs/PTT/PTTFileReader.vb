#Region "Microsoft.VisualBasic::c6175dda54a4ebee1b2b302b2208fa53, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\PTT\PTTFileReader.vb"

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

    '   Total Lines: 33
    '    Code Lines: 24
    ' Comment Lines: 4
    '   Blank Lines: 5
    '     File Size: 1.38 KB


    '     Module PTTFileReader
    ' 
    '         Function: Read
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Module PTTFileReader

        ''' <summary>
        ''' 出错不会被处理，而<see cref="PTT.Load(String, Boolean)"/>函数则会处理错误，返回Nothing
        ''' </summary>
        ''' <returns></returns>
        Public Function Read(path As String, Optional FillBlankName As Boolean = False) As PTT
            Dim lines As String() = File.ReadAllLines(path)
            Dim PTT As New PTT With {
                .Title = lines(0)
            }

            lines = (From s As String In lines.Skip(3) Where Not String.IsNullOrWhiteSpace(s) Select s).ToArray
            Dim Genes As GeneBrief() = New GeneBrief(lines.Length - 1) {}
            For i As Integer = 0 To lines.Length - 1
                Dim strLine As String = lines(i)
                Genes(i) = GeneBrief.DocumentParser(strLine, FillBlankName)
            Next
            PTT.GeneObjects = Genes
            Dim strTemp As String = r.Match(PTT.Title, " - \d+\.\.\d+").Value
            PTT.Size = Val(Strings.Split(strTemp, "..").Last)
            PTT.Title = PTT.Title.Replace(strTemp, "")

            Return PTT
        End Function
    End Module
End Namespace
