#Region "Microsoft.VisualBasic::12a85c035c90be1c7e76b688898ce5ae, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\SeedData.vb"

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

Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports Microsoft.VisualBasic

Namespace Topologically

    <Serializable> Public Structure SeedData

        Public Seeds As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Save(path As String) As Boolean
            Return Serialize(path)
        End Function

        Public Shared Function Load(path As String) As SeedData
            Return path.Load(Of SeedData)
        End Function

        Public Shared Function Initialize(chars As Char(), max As Integer) As SeedData
            Dim seedsBuf As List(Of String) = InitializeSeeds(chars, 1).Distinct.ToList
            Dim tmp As String() = seedsBuf

            For i As Integer = 2 To max
                tmp = New List(Of String)(ExtendSequence(tmp, chars).Distinct)
                seedsBuf += tmp
                Call $"{New String("-", 20)}>  {i}bp".__DEBUG_ECHO
            Next

            Return New SeedData With {
                .Seeds = seedsBuf
            }
        End Function
    End Structure
End Namespace
