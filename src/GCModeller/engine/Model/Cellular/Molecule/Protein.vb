#Region "Microsoft.VisualBasic::d8bfcb735248004c0e3dd29141ab3df0, engine\Model\Cellular\Molecule\Protein.vb"

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

    '     Structure Protein
    ' 
    '         Properties: ProteinID
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Cellular.Molecule

    ''' <summary>
    ''' Protein Modification
    ''' 
    ''' ``{polypeptide} + compounds -> protein``
    ''' </summary>
    Public Structure Protein

        Dim polypeptides As String()
        Dim compounds As String()

        Public Property ProteinID As String

        ''' <summary>
        ''' 这个蛋白质是由一条多肽链所构成的
        ''' </summary>
        ''' <param name="proteinId"></param>
        Sub New(proteinId As String)
            polypeptides = {proteinId}
        End Sub

    End Structure
End Namespace
