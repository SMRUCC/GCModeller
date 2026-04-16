#Region "Microsoft.VisualBasic::af54ddd709fb072e67470e575d4045f2, annotations\GSEA\GSEA.KnowledgeBase.Extensions\Metabolism\Metpa\pathIds.vb"

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

    '   Total Lines: 24
    '    Code Lines: 18 (75.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (25.00%)
    '     File Size: 875 B


    '     Class pathIds
    ' 
    '         Properties: ids, pathwayNames
    ' 
    '         Function: FromPathways, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Metabolism.Metpa

    Public Class pathIds

        Public Property pathwayNames As String()
        Public Property ids As String()

        Public Overrides Function ToString() As String
            Return $"has {ids.Length} pathways..."
        End Function

        Public Shared Function FromPathways(Of T As PathwayBrief)(pathways As T(), Optional keggId As String = Nothing) As pathIds
            Dim ids = pathways.Select(Function(m) If(keggId.StringEmpty, m.EntryId, keggId & m.briteID)).ToArray
            Dim pathwayNames = pathways.Select(Function(m) m.name.Replace(" - Reference pathway", "")).ToArray

            Return New pathIds With {
                .ids = ids,
                .pathwayNames = pathwayNames
            }
        End Function
    End Class
End Namespace
