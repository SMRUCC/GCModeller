#Region "Microsoft.VisualBasic::c8ffc0a13141e06071d69f4caa132651, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\ToolsAPI\SiteSigma.vb"

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
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.DifferenceMeasurement

''' <summary>
''' 基因组两两比较所得到的位点距离数据
''' </summary>
''' <remarks></remarks>
Public Class SiteSigma
    <Column("Site")> Public Property Site As Integer
    <Column("Sigma")> Public Property Sigma As Double
    Public Property Similarity As DeltaSimilarity1998.SimilarDiscriptions
End Class
