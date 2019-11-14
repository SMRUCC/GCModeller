#Region "Microsoft.VisualBasic::7b6075b751bd31afc4571b1f354d8e4e, analysis\Motifs\SharedDataModels\ModuleClassAPI.vb"

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

    '     Module ModuleClassAPIExtension
    ' 
    '         Function: __firstCount, (+2 Overloads) Fill
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace DocumentFormat

    Public Module ModuleClassAPIExtension

        <Extension> Public Function Fill(Of T As RegulatesFootprints)(api As BriteHEntry.ModuleClassAPI, x As T) As T
            If Not api.Genehash.ContainsKey(x.ORF) Then
                Return x
            End If

            Dim mods As PathwayBrief() = api.Genehash(x.ORF)
            Dim A = __firstCount(mods, api.GetXType)
            Dim B = __firstCount(mods, api.GetXClass)
            Dim C = __firstCount(mods, api.GetXCategory)

            x.Category = C
            x.Class = B
            x.Type = A

            Return x
        End Function

        Private Function __firstCount(mods As PathwayBrief(), __getType As Func(Of PathwayBrief, String)) As String
            Dim cls = (From x In mods Select s = __getType(x) Group s By s Into Count)
            Dim orders = (From x In cls
                          Select x
                          Order By x.Count Descending).First.s
            Return orders
        End Function

        <Extension> Public Function Fill(Of T As RegulatesFootprints)(api As BriteHEntry.ModuleClassAPI, source As IEnumerable(Of T)) As T()
            Dim LQuery = (From x In source.AsParallel Select api.Fill(x)).ToArray
            Return LQuery
        End Function
    End Module
End Namespace
