#Region "Microsoft.VisualBasic::2bf1143b305e4674589e851bd69736f0, engine\BootstrapLoader\BootLoader\Extensions.vb"

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

    '   Total Lines: 29
    '    Code Lines: 21 (72.41%)
    ' Comment Lines: 6 (20.69%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (6.90%)
    '     File Size: 1.20 KB


    ' Module Extensions
    ' 
    '     Function: variables
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule

<HideModuleName>
Public Module Extensions

    ''' <summary>
    ''' Populate all the <paramref name="complex"/> related components.
    ''' </summary>
    ''' <param name="massTable"></param>
    ''' <param name="complex"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function variables(massTable As MassTable, complex As Protein, cellular_id As String, polypeptideIds As Index(Of String)) As IEnumerable(Of Variable)
        For Each compound As String In complex.compounds.SafeQuery
            Yield massTable.variable(compound, cellular_id)
        Next
        For Each peptide As String In complex.polypeptides
            If peptide Like polypeptideIds Then
                Yield massTable.variable("*" & peptide, cellular_id)
            Else
                Yield massTable.variable(peptide, cellular_id)
            End If
        Next
    End Function
End Module
