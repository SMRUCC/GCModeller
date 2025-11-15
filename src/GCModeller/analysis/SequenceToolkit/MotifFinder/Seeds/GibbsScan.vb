#Region "Microsoft.VisualBasic::886b79d7e64e693d5d5d64aa93c5db3e, analysis\SequenceToolkit\MotifFinder\Seeds\GibbsScan.vb"

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

'   Total Lines: 20
'    Code Lines: 16 (80.00%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 4 (20.00%)
'     File Size: 664 B


' Class GibbsScan
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: GetSeeds
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.GibbsSampling
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class GibbsScan : Inherits SeedScanner

    Public Sub New(param As PopulatorParameter, debug As Boolean)
        MyBase.New(param, debug)
    End Sub

    Public Overrides Iterator Function GetSeeds(regions() As FastaSeq) As IEnumerable(Of HSP)
        For i As Integer = param.minW To param.maxW
            Dim gibbs As New Gibbs(regions.Select(Function(a) a.SequenceData).ToArray, i)
            Dim find = gibbs.sample

            For Each hit In find
                Yield New HSP
            Next
        Next
    End Function
End Class
