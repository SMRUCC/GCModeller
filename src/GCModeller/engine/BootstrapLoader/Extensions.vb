#Region "Microsoft.VisualBasic::b7ece50a430d061b498b6d57541d71e6, GCModeller\engine\BootstrapLoader\Extensions.vb"

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

    '   Total Lines: 18
    '    Code Lines: 16
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 684 B


    ' Module Extensions
    ' 
    '     Function: variables
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule

<HideModuleName>
Public Module Extensions

    <Extension>
    Public Iterator Function variables(massTable As MassTable, complex As Protein) As IEnumerable(Of Variable)
        For Each compound In complex.compounds
            Yield massTable.variable(compound)
        Next
        For Each peptide In complex.polypeptides
            Yield massTable.variable(peptide)
        Next
    End Function
End Module
