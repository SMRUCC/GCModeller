#Region "Microsoft.VisualBasic::f6964ebe7b1108fbba8e69d19a70a33f, shoalAPI\Expasy.vb"

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

    ' Module Expasy
    ' 
    '     Function: EnzymeClassification, ReadEnzymeClass, SaveResult
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("Expasy", Category:=APICategories.ResearchTools)>
Public Module Expasy

    <ExportAPI("Read.Csv.EnzymeClass")>
    Public Function ReadEnzymeClass(path As String) As LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT()
        Return path.LoadCsv(Of LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT)(False).ToArray
    End Function

    <ExportAPI("EC.Classify.Process")>
    Public Function EnzymeClassification(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT)) _
        As LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT()
        Return LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.EnzymeClassification(data)
    End Function

    <ExportAPI("Write.Csv.EnzymeClass")>
    Public Function SaveResult(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT),
                               saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function
End Module
