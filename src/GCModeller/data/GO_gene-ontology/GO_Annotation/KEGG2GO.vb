#Region "Microsoft.VisualBasic::6b9cc06183cfc52dd64333efa3171742, data\GO_gene-ontology\GO_Annotation\KEGG2GO.vb"

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

    ' Module KEGG2GO
    ' 
    '     Function: PopulateMappings
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Module KEGG2GO

    <Extension>
    Public Iterator Function PopulateMappings(proteins As IEnumerable(Of entry)) As IEnumerable(Of (KO As String, GO As String()))
        For Each protein As entry In proteins
            Dim KO = protein.xrefs.TryGetValue("KO", [default]:=Nothing)
            Dim GO = protein.xrefs.TryGetValue("GO", [default]:=Nothing)

            If KO.IsNullOrEmpty Then
                Continue For
            ElseIf GO.IsNullOrEmpty Then
                Continue For
            End If

            For Each idRef As dbReference In KO
                Yield (idRef.id, GO.Select(Function(a) a.id).ToArray)
            Next
        Next
    End Function
End Module

