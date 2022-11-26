#Region "Microsoft.VisualBasic::52e259889348124bf916dd61da47f6cd, GCModeller\analysis\SequenceToolkit\ClusterMatrix\Matrix\SmithWaterman.vb"

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
    '    Code Lines: 22
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 924 B


    ' Module Matrix
    ' 
    '     Function: SimilarityMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module Matrix

    <Extension>
    Public Function SimilarityMatrix(source As FastaFile,
                                     Optional args As NameValueCollection = Nothing,
                                     Optional method As MatrixMethods = MatrixMethods.NeedlemanWunsch,
                                     Optional ByRef out As StreamWriter = Nothing) As DataSet()
        If args Is Nothing Then
            args = New NameValueCollection
        End If

        Select Case method
            Case MatrixMethods.NeedlemanWunsch
                Return source.NeedlemanWunsch(out)
            Case Else
                Return source.NeedlemanWunsch(out)
        End Select
    End Function
End Module
