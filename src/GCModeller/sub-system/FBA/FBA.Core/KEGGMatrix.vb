#Region "Microsoft.VisualBasic::7db3c3a6b53273b2e2273640eb22f034, GCModeller\sub-system\FBA\FBA.Core\KEGGMatrix.vb"

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

    '   Total Lines: 41
    '    Code Lines: 37
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 1.61 KB


    ' Module KEGGMatrix
    ' 
    '     Function: CreateKeggMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Module KEGGMatrix

    <Extension>
    Public Function CreateKeggMatrix(keggNetwork As IEnumerable(Of Reaction)) As Matrix
        Dim graph As Equation() = keggNetwork.Select(Function(r) r.ReactionModel).ToArray
        Dim allCompounds As String() = graph _
            .Select(Function(r) r.GetMetabolites) _
            .IteratesALL _
            .Select(Function(sp) sp.ID) _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToArray
        Dim matrix As Double()() = allCompounds _
            .Select(Function(id)
                        Return graph _
                            .Select(Function(r)
                                        Return r.GetCoEfficient(id, directional:=True)
                                    End Function) _
                            .ToArray
                    End Function) _
            .ToArray

        Return New Matrix With {
            .Matrix = matrix,
            .Compounds = allCompounds,
            .Flux = graph _
                .ToDictionary(Function(flux) flux.Id,
                              Function(flux)
                                  Return New DoubleRange(-10, 10)
                              End Function),
            .Targets = graph.Select(Function(r) r.Id).ToArray
        }
    End Function

End Module
