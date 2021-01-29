#Region "Microsoft.VisualBasic::ec718faab42374dc5f7df7aae8259f71, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\MatrixDatabase\CorrsDbAPI.vb"

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

    ' Delegate Function
    ' 
    ' 
    ' Module CorrsDbAPI
    ' 
    '     Function: FastImports, IsTrue, IsTrueFunc
    '     Class __isTRUE
    ' 
    '         Function: IsTrue
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.WGCNA

''' <summary>
''' PCC/sPCC
''' </summary>
''' <param name="g1"></param>
''' <param name="g2"></param>
''' <returns></returns>
Public Delegate Function IsTrue(g1 As String, g2 As String) As Boolean

<Package("GCModeller.Gene.Correlations",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Category:=APICategories.ResearchTools)>
Public Module CorrsDbAPI

    <ExportAPI("IsTrue?")>
    <Extension>
    Public Function IsTrueFunc(corr As ICorrelations, Optional cut As Double = 0.65) As IsTrue
        Return AddressOf New __isTRUE With {
            .corrs = corr,
            .cut = cut
        }.IsTrue
    End Function

    Private Class __isTRUE

        Public corrs As ICorrelations
        Public cut As Double

        Public Function IsTrue(g1 As String, g2 As String) As Boolean
            Return corrs.IsTrue(g1, g2, cut)
        End Function
    End Class

    <ExportAPI("IsTrue?")>
    <Extension>
    Public Function IsTrue(corr As ICorrelations, g1 As String, g2 As String, Optional cut As Double = 0.65) As Boolean
        Dim pcc As Double = corr.GetPcc(g1, g2)
        Dim spcc As Double = corr.GetSPcc(g1, g2)
        Return Math.Abs(pcc) >= cut OrElse Math.Abs(spcc) >= cut
    End Function
End Module
