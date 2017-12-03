#Region "Microsoft.VisualBasic::3c0f210184c0fe61fb6b78ec6ec7529d, ..\GCModeller\annotations\GO\PlantRegMap\PlantRegMap.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region


Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Microarray

Namespace PlantRegMap

    ''' <summary>
    ''' PlantRegMap. go enrichment result output
    ''' </summary>
    Public Class PlantRegMap_GoTermEnrichment
        Implements IGoTerm
        Implements IGoTermEnrichment

        <Column("GO.ID")> Public Property GoID As String Implements IGoTerm.Go_ID

        Public Property Term As String
        Public Property Annotated As String
        Public Property Count As String
        Public Property Expected As String
        <Column("p-value")> Public Property pvalue As Double Implements IGoTermEnrichment.Pvalue, IGoTermEnrichment.CorrectedPvalue
        <Column("q-value")> Public Property qvalue As Double
        Public Property Aspect As String
        Public Property Genes As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
