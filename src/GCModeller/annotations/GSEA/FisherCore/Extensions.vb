#Region "Microsoft.VisualBasic::b6606837eb09ae773e0fd10ef7645e44, GCModeller\annotations\GSEA\FisherCore\Extensions.vb"

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
    '    Code Lines: 23
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.17 KB


    ' Module Extensions
    ' 
    '     Function: CreateResultProfiles
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.Annotation

Public Module Extensions

    <Extension>
    Public Function CreateResultProfiles(enrich As EnrichmentResult(), catalogs As Dictionary(Of String, CatalogProfiling)) As CatalogProfiles
        Dim result As New CatalogProfiles
        Dim termIndex As Dictionary(Of String, EnrichmentResult) = enrich.ToDictionary(Function(a) a.term)

        For Each cat As CatalogProfiling In catalogs.Values
            For Each subcat In cat.SubCategory.Values
                If termIndex.ContainsKey(subcat.Catalog) Then
                    If termIndex(subcat.Catalog).pvalue >= 1 Then
                        Continue For
                    End If

                    If Not result.catalogs.ContainsKey(cat.Description) Then
                        result.catalogs(cat.Description) = New CatalogProfile
                    End If

                    result.catalogs(cat.Description).Add(subcat.Description, -Math.Log10(termIndex(subcat.Catalog).pvalue))
                End If
            Next
        Next

        Return result
    End Function
End Module

