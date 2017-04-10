#Region "Microsoft.VisualBasic::362ebfc2daced1dc9084dbd26022abc8, ..\GCModeller\engine\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\ProteinAssemblies.vb"

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

Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic

Namespace Builder

    ''' <summary>
    ''' 信号转导网络
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProteinAssemblies : Inherits IBuilder

        Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel)
            MyBase.New(MetaCyc, Model)
        End Sub

        ''' <summary>
        ''' 编译MetaCyc数据库中的Reactions表中的ProteinReaction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Invoke() As BacterialModel
            Call Printf("Start to compile protein table into the proteinassembly rules...")

            Dim ProteinAssemblies = (From rxn In MyBase.MetaCyc.GetReactions Where rxn.Types.IndexOf("Protein-Reactions") > -1 Select rxn) '筛选出蛋白质反应
            '     Model.ProteinAssemblies = (From rxn In ProteinAssemblies Select CType(rxn, GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction)).AsList

            Dim ProteinList As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) = New List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction)
            Dim ProteinCPLXCollection = (From Protein As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein
                                         In MetaCyc.GetProteins
                                         Where Not Protein.Components.IsNullorEmpty
                                         Select Protein).ToArray  '筛选出所有的蛋白质复合物

            For Each Protein In ProteinCPLXCollection '对象直接在Model中的代谢底物列表中查询
                Dim ProteinAssembly As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction = New GCML_Documents.XmlElements.Metabolism.Reaction With {.Identifier = Protein.Identifier}
                ProteinAssembly.Products = {New GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = Protein.Identifier, .StoiChiometry = 1}}
                ProteinAssembly.Reactants = (From cp In Protein.Components Select New GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = cp, .StoiChiometry = 1}).ToArray
                ProteinAssembly.UPPER_BOUND = New GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = 10}
                Call ProteinList.Add(ProteinAssembly)
            Next

            Model.ProteinAssemblies.AddRange(ProteinList)
            Call Printf("End of function:: compiler_builder_compile_proteinAssembly()")

            Return Model
        End Function
    End Class

    'Public Class Proteins : Inherits IBuilder

    '    Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As Assembly.DocumentFormat.GCMarkupLanguage.Model)
    '        Call MyBase.New(MetaCyc, Model)
    '    End Sub

    '    Public Overrides Function Invoke() As Model
    '        Dim Metabolites = MyBase.Model.Metabolism.Metabolites.ToArray
    '        Dim LQuery = From Protein In MyBase.MetaCyc.GetProteins Select GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Protein.CreateDataModel(Protein, Metabolites) '
    '        Model.Proteins = LQuery.ToArray

    '        Return MyBase.Model
    '    End Function
    'End Class

    Public Class Polypeptides : Inherits IBuilder
        Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel)
            Call MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Dim LQuery = (From tr In Model.BacteriaGenome.Transcripts Select GCML_Documents.XmlElements.Metabolism.Polypeptide.CreateObject(tr, Model, MetaCyc)).ToArray
            Model.Polypeptides = LQuery
            Dim m = (From pl In Model.Polypeptides Select pl.GenerateVector(MetaCyc)).ToArray.Count
            Return Model
        End Function
    End Class
End Namespace
