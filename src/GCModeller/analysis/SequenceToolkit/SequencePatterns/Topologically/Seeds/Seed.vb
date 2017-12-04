#Region "Microsoft.VisualBasic::58d420d012edcc4633af1b62f1270851, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Seeds\Seed.vb"

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

Namespace Topologically.Seeding

    Public Structure Seed

        Public ReadOnly Property Sequence As String
        Public ReadOnly Property Parent As String

        Sub New(seq$)
            Sequence = seq
            Parent = Mid(seq, 1, seq.Length - 1)
        End Sub

        Public Function IsMyParent(seed$) As Boolean
            Return InStr(Sequence, seed) = 1
        End Function

        Public Overrides Function ToString() As String
            Return Parent & " --> " & Sequence
        End Function
    End Structure
End Namespace
