#Region "Microsoft.VisualBasic::0b1e1e449664dfbb7f1b4280b359aff5, sub-system\FBA\FBA_DP\Visual\FluxVisual.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'Imports System.Drawing
'Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
'Imports SMRUCC.genomics.Assembly.SBML
'Imports SMRUCC.genomics.Assembly.SBML.Level2
'Imports SMRUCC.genomics.GCModeller.AnalysisTools.ModelSolvers.FBA.Models.rFBA
'Imports Microsoft.VisualBasic.DocumentFormat.Csv

'Public Module FluxVisual

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="coefficient"><see cref="RPKMStat"/> Csv文件的路径</param>
'    ''' <param name="model"></param>
'    ''' <returns></returns>
'    ''' <param name="mods">KEGG Modules DIR</param>
'    Public Function DrawingModule(coefficient As String, model As XmlFile, mods As String) As Image
'        Dim MAT As IO.File = IO.File.FastLoad(coefficient) ' 由于文件可能比较大，直接使用反射加载可能比较慢，由于id号之中没有逗号，所以直接使用fastLoad加载数据
'        Dim source As RPKMStat() = MAT.AsDataSource(Of RPKMStat)(False)
'        Dim modules = (From file As String
'                       In FileIO.FileSystem.GetFiles(mods, FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel
'                       Select file.LoadXml(Of [Module])).ToArray
'        Dim kgRxns = ExportServices.LoadReactions(GCModeller.FileSystem.KEGG.GetReactions)

'    End Function
'End Module
