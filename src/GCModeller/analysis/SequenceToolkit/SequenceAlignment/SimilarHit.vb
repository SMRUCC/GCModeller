#Region "Microsoft.VisualBasic::c876dfb3771f597edfa4f249fa3cdc85, analysis\SequenceToolkit\SequenceAlignment\SimilarHit.vb"

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

    '   Total Lines: 23
    '    Code Lines: 17 (73.91%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (26.09%)
    '     File Size: 511 B


    ' Class SimilarHit
    ' 
    '     Properties: IsUniqued, SeqID, Similar, Size
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region


Public Class SimilarHit

    Public Property SeqID As String
    Public Property Similar As Dictionary(Of String, Double)

    Public ReadOnly Property IsUniqued As Boolean
        Get
            Return Similar.IsNullOrEmpty
        End Get
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return Similar.TryCount
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return SeqID
    End Function

End Class

