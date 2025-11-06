#Region "Microsoft.VisualBasic::c251738d7d994057766238f350e0dec6, engine\Model\Cellular\Molecule\Phenotype.vb"

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

'   Total Lines: 28
'    Code Lines: 8 (28.57%)
' Comment Lines: 14 (50.00%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 6 (21.43%)
'     File Size: 863 B


'     Structure Phenotype
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Namespace Cellular.Molecule

    ''' <summary>
    ''' 在进行计算的时候，是以代谢反应过程为基础的
    ''' 代谢途径等信息可以在导出结果之后做分类分析
    ''' 所以在模型之中并没有包含有代谢途径的信息？？？
    ''' </summary>
    Public Structure Phenotype

        ''' <summary>
        ''' enzyme = protein + RNA
        ''' </summary>
        Public enzymes As String()

        ''' <summary>
        ''' A cellular network graph consist of collection of the biological reactions
        ''' </summary>
        Public fluxes As Reaction()

        ''' <summary>
        ''' Some protein is not an enzyme
        ''' </summary>
        Public proteins As Protein()

        Public Function GetMetaboliteSymbolNames() As Dictionary(Of String, String)
            Dim symbolNames As New Dictionary(Of String, String)

            For Each flux As Reaction In fluxes
                For Each cpd As String In flux.AllCompounds
                    symbolNames(cpd) = cpd
                Next
            Next

            Return symbolNames
        End Function
    End Structure
End Namespace
