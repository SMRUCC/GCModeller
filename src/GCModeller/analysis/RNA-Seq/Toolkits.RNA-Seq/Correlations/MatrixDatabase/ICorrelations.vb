#Region "Microsoft.VisualBasic::77b70521fac935ece60eaed2ecd1b9b5, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\MatrixDatabase\ICorrelations.vb"

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

    ' Interface ICorrelations
    ' 
    '     Function: GetPcc, GetPccGreaterThan, GetPccSignificantThan, GetSPcc, GetWGCNAWeight
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis
Imports SMRUCC.genomics.Analysis.RNA_Seq

Public Interface ICorrelations

    ''' <summary>
    ''' 无方向性的
    ''' </summary>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <returns></returns>
    Function GetPcc(id1 As String, id2 As String) As Double

    ''' <summary>
    ''' 无方向性的
    ''' </summary>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <returns></returns>
    Function GetSPcc(id1 As String, id2 As String) As Double

    ''' <summary>
    ''' 无方向性的
    ''' </summary>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <returns></returns>
    Function GetWGCNAWeight(id1 As String, id2 As String) As Double
    Function GetPccGreaterThan(id As String, cutoff As Double) As Dictionary(Of String, Double)

    ''' <summary>
    ''' <see cref="GetPccGreaterThan"/>不取绝对值，这个函数是取绝对值的
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="cutoff"></param>
    ''' <returns></returns>
    Function GetPccSignificantThan(id As String, cutoff As Double) As Dictionary(Of String, Double)
End Interface
