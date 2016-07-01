#Region "Microsoft.VisualBasic::7e674beffe57cb115d764c84b08a642b, ..\GCModeller\analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\MatrixDatabase\ICorrelations.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Toolkits.RNA_Seq
Imports SMRUCC.genomics.Toolkits

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
