#Region "Microsoft.VisualBasic::e7991b4396ec1e561c2daa6bb921343c, GO_gene-ontology\GeneOntology\DAG\Axioms.vb"

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

' Module Axioms
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data.GeneOntology.OBO

''' <summary>
''' 任意距离的term之间的关系推断规则
''' </summary>
Public Module Axioms

    ''' <summary>
    ''' 推断出两个GO词条<paramref name="a"/>和<paramref name="b"/>之间的关系
    ''' </summary>
    ''' <param name="go">The go database</param>
    ''' <param name="a"><see cref="Term.id"/></param>
    ''' <param name="b"><see cref="Term.id"/></param>
    ''' <returns></returns>
    <Extension>
    Public Function Infer(go As GO_OBO, a$, b$) As OntologyRelations

    End Function
End Module
