#Region "Microsoft.VisualBasic::4f41ab41ed145d8c1b5aa449b447f6b5, engine\IO\GCTabular\Compiler\Extract_SBML_GeneralSubstrates.vb"

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

    '     Module Extract_SBML_GeneralSubstrates
    ' 
    '         Sub: Analysis
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Model.SBML

Namespace Compiler.Components

    Module Extract_SBML_GeneralSubstrates

        ''' <summary>
        ''' 将通用的代谢物进行展开
        ''' </summary>
        ''' <param name="Metabolites"></param>
        ''' <param name="MetaCyc">目标细菌的MetaCyc数据库</param>
        ''' <remarks></remarks>
        Public Sub Analysis(ByRef Metabolites As Dictionary(Of FileStream.Metabolite),
                            MetaCyc As DatabaseLoadder,
                            Model As FileStream.IO.XmlresxLoader,
                            Logging As LogFile)

            Dim SBML As Level2.XmlFile = Level2.XmlFile.Load(MetaCyc.SBMLMetabolismModel)

            Call Logging.WriteLine("Start to merge the sbml metabolite with datamodels...", "Extract_SBML_GeneralSubstrates->Analysis()")

            For Each Metabolite In SBML.Model.listOfSpecies       '首先按照MetaCycId查找，查找不到的时候在添加
                Dim Id As String = Metabolite.GetTrimmedId
                Dim LQuery = (From item In Metabolites.AsParallel
                              Where String.Equals(item.Value.MetaCycId, Id, StringComparison.OrdinalIgnoreCase)
                              Select item).ToArray

                If LQuery.IsNullOrEmpty Then '添加新的
                    Dim MetaboliteModel = FileStream.Metabolite.CreateObject(Metabolite)
                    Call Metabolites.Add(MetaboliteModel.Identifier, MetaboliteModel)
                Else
                    '查找到了
                End If
            Next
        End Sub
    End Module
End Namespace
