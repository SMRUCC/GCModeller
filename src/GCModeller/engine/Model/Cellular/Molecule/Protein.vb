#Region "Microsoft.VisualBasic::77d0c1b5fef6c5fdd66202c9a0480dde, engine\Model\Cellular\Molecule\Protein.vb"

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

    '   Total Lines: 40
    '    Code Lines: 20 (50.00%)
    ' Comment Lines: 13 (32.50%)
    '    - Xml Docs: 92.31%
    ' 
    '   Blank Lines: 7 (17.50%)
    '     File Size: 1.18 KB


    '     Structure Protein
    ' 
    '         Properties: isAutoConstructed, ProteinID
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
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

        ''' <summary>
        ''' the unique id of current protein complex
        ''' </summary>
        ''' <returns></returns>
        Public Property ProteinID As String

        Public ReadOnly Property isAutoConstructed As Boolean
            Get
                Return polypeptides.TryCount = 1 AndAlso
                    compounds.IsNullOrEmpty AndAlso
                    polypeptides.First = ProteinID
            End Get
        End Property

        ''' <summary>
        ''' 这个蛋白质是由一条多肽链所构成的
        ''' </summary>
        ''' <param name="proteinId"></param>
        Sub New(proteinId As String)
            polypeptides = {proteinId}
        End Sub

        Public Overrides Function ToString() As String
            Return If(isAutoConstructed, "[auto-construct] ", "") & ProteinID
        End Function

    End Structure
End Namespace
