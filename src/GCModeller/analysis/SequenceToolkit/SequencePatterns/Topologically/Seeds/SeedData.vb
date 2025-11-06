#Region "Microsoft.VisualBasic::6fc8b53315efc777c8b55409a4d35124, analysis\SequenceToolkit\SequencePatterns\Topologically\Seeds\SeedData.vb"

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

    '   Total Lines: 39
    '    Code Lines: 30 (76.92%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (23.08%)
    '     File Size: 1.23 KB


    '     Structure SeedData
    ' 
    '         Function: Initialize, Load, Save, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Topologically.Seeding

    <Serializable> Public Structure SeedData

        Public Seeds As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Save(path As String) As Boolean
            Return Seeds.SaveTo(path)
        End Function

        Public Shared Function Load(path As String) As SeedData
            Return New SeedData With {
                .Seeds = path.ReadAllLines
            }
        End Function

        Public Shared Function Initialize(chars As Char(), max As Integer) As SeedData
            Dim seedsBuf As List(Of String) = InitializeSeeds(chars, 1).Distinct.AsList
            Dim tmp As String() = seedsBuf

            For i As Integer = 2 To max
                tmp = ExtendSequence(tmp, chars).Distinct.ToArray
                seedsBuf += tmp
                Call $"{New String("-"c, 20)}>  {i}bp".debug
            Next

            Return New SeedData With {
                .Seeds = seedsBuf
            }
        End Function
    End Structure
End Namespace
