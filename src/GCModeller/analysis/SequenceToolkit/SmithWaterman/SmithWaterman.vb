#Region "Microsoft.VisualBasic::71bb0dd5a6db4605a0e6a5e4376a1cc9, ..\GCModeller\analysis\SequenceToolkit\SmithWaterman\SmithWaterman.vb"

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

Imports System.Linq
Imports SMRUCC.genomics.SequenceModel

''' <summary>
''' Smith-Waterman local alignment algorithm.
'''
''' Design Note: this class implements AminoAcids interface: a simple fix customized to amino acids, since that is all we deal with in this class
''' Supporting both DNA and Aminoacids, will require a more general design.
''' </summary>
Public Class SmithWaterman : Inherits GSW(Of Char)

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="subject"></param>
    ''' <param name="blosum">
    ''' If the matrix parameter is null, then the default build in blosum62 matrix will be used.
    ''' </param>
    Sub New(query As String, subject As String, Optional blosum As Blosum = Nothing)
        Call MyBase.New(query.ToArray, subject.ToArray, __blosum(blosum), Function(x) x)
    End Sub

    Private Shared Function __blosum(input As Blosum) As ISimilarity(Of Char)
        If input Is Nothing Then
            input = Blosum.FromInnerBlosum62
        End If

        Return AddressOf input.getDistance
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="cutoff">0%-100%</param>
    ''' <returns></returns>
    Public Function GetOutput(cutoff As Double, minW As Integer) As Output
        Return Output.CreateObject(Me, Function(x) x, cutoff, minW)
    End Function

    ''' <summary>
    ''' Default using ``Blosum62`` matrix.
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="subject"></param>
    ''' <param name="blosum"></param>
    ''' <returns></returns>
    Public Shared Function Align(query As FASTA.FastaToken,
                                 subject As FASTA.FastaToken,
                                 Optional blosum As Blosum = Nothing) As SmithWaterman
        Dim sw As New SmithWaterman(query.SequenceData, subject.SequenceData, blosum)
        Return sw
    End Function
End Class
