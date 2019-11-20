#Region "Microsoft.VisualBasic::dde3adcf1d8f220cf3787fa149e0e51d, sub-system\FBA\FBA_DP\FBA\Models\gcFBA\ARGVS.vb"

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

    '     Class rFBA_ARGVS
    ' 
    '         Properties: baseFactor, DirectedFactor, FluxBoundOverrides, FluxOverrides, forceEnzymeRev
    '                     PCCw, sPCCw, SupressImpact
    ' 
    '         Function: ToString
    ' 
    '     Class Modifier
    ' 
    '         Properties: Comments, locus, modify
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Models.rFBA

    Public Class rFBA_ARGVS
        Public Property baseFactor As Double = 2.5
        Public Property PCCw As Double = 0.7
        Public Property sPCCw As Double = 0.25
        Public Property FluxBoundOverrides As Double = 25
        Public Property SupressImpact As Double = 1.2
        Public Property FluxOverrides As Boolean = True
        ''' <summary>
        ''' 假若反应是单向的，则乘以这个倍增系数
        ''' </summary>
        ''' <returns></returns>
        Public Property DirectedFactor As Double = 1.5
        Public Property forceEnzymeRev As Boolean = True

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' 基因突变的设置参数
    ''' </summary>
    Public Class Modifier : Implements INamedValue

        Public Property locus As String Implements INamedValue.Key
        ''' <summary>
        ''' 突变修饰
        ''' </summary>
        ''' <returns></returns>
        Public Property modify As Double
        Public Property Comments As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
