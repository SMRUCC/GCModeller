#Region "Microsoft.VisualBasic::6558fd92b6e5cf26906dfe105f966528, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\Compiler.vb"

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

    '   Total Lines: 135
    '    Code Lines: 94
    ' Comment Lines: 16
    '   Blank Lines: 25
    '     File Size: 6.36 KB


    '     Class Compiler
    ' 
    '         Properties: [Return], LogFile, StringReplacements, Version
    ' 
    '         Function: Compile, GenerateModelProperty, Link, PreCompile, Trim
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Builder

    Public Class Compiler : Inherits Compiler(Of BacterialModel)

        Dim MetaCyc As DatabaseLoadder
        Public Property StringReplacements As Escaping()
        Public Property LogFile As LogFile

        Public Overrides ReadOnly Property [Return] As BacterialModel
            Get
                Return MyBase.CompiledModel
            End Get
        End Property

        ''' <summary>
        ''' 预编译目标模型
        ''' </summary>
        ''' <param name="Path">MetaCyc数据库的数据文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function PreCompile(Path As CommandLine) As Integer
            Dim SBML As Level2.XmlFile =
                Level2.XmlFile.Load(Path.cli & "/metabolic-reactions.sbml")
            Dim Compartments As Dictionary(Of String, Integer) =
                New Dictionary(Of String, Integer)

            MyBase.CompiledModel = New BacterialModel With {
                .Metabolism = New GCML_Documents.XmlElements.Metabolism.Metabolism With {
                    .Compartments = (From Handle As Integer
                                     In SBML.Model.listOfCompartments.Sequence
                                     Select GCML_Documents.ComponentModels.Compartment.CastTo(SBML.Model.listOfCompartments(Handle))).AsList
                }
            }

            For i As Integer = 0 To MyBase.CompiledModel.Metabolism.Compartments.Count - 1
                Dim Compartment = MyBase.CompiledModel.Metabolism.Compartments(i)
                '     Compartments.Add(Compartment.id, Compartment.Handle)
            Next

            MyBase.CompiledModel.Metabolism.Metabolites = SBML.Model.listOfSpecies.AsMetabolites
            For i As Integer = 0 To MyBase.CompiledModel.Metabolism.Metabolites.Count - 1
                MyBase.CompiledModel.Metabolism.Metabolites(i).Compartment = Compartments(MyBase.CompiledModel.Metabolism.Metabolites(i).Compartment)
            Next
            MyBase.CompiledModel.Metabolism.MetabolismNetwork = (From e In SBML.Model.listOfReactions.AsParallel Select GCML_Documents.XmlElements.Metabolism.Reaction.CastTo(e, MyBase.CompiledModel)).AsList
            MyClass.MetaCyc = DatabaseLoadder.CreateInstance(MetaCycDir:=Path.cli)

            Call Trim(Model:=MyBase.CompiledModel, StringList:=Me.StringReplacements)

            Return 0
        End Function

        Private Function GenerateModelProperty() As LDM.Property
            Dim ModelProperty As New LDM.Property
            ModelProperty.Authors = New List(Of String) From {My.User.Name}
            ModelProperty.CompiledDate = Now.ToString
            ModelProperty.GUID = Guid.NewGuid.ToString

            Dim Specie = MetaCyc.Database.Species.First

            ModelProperty.Comment = Specie.Comment
            ModelProperty.SpecieId = Specie.GetFullName
            ModelProperty.URLs = New List(Of String) From {Specie.PGDBHomePage}
            ModelProperty.Name = Specie.Identifier
            ModelProperty.DBLinks = Specie.DBLinks

            Return ModelProperty
        End Function

        ''' <summary>
        ''' 替换掉模型中的UniqueID之中被转义的字符串
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <param name="StringList"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Trim(Model As BacterialModel, StringList As Escaping()) As BacterialModel
            Call Replacer.ApplyReplacements(Of GCML_Documents.ComponentModels.CompoundSpeciesReference, BacterialModel)(Model, StringList)  'String replacement

            For i As Integer = 0 To Model.Metabolism.Compartments.Count - 1
                Model.Metabolism.Compartments(i).id = Model.Metabolism.Compartments(i).id.Replace(StringList)
            Next

            For i As Integer = 0 To Model.Metabolism.MetabolismNetwork.Count - 1
                Dim Metabolites = Model.Metabolism.MetabolismNetwork(i).Metabolites

                For idx As Integer = 0 To Metabolites.Count - 1
                    Metabolites(idx).species = Metabolites(idx).species.Replace(StringList)
                Next
            Next

            Return Model
        End Function

        Protected Overrides Function Link() As Integer
            Call printf("COMPILER::LINK()")

            Call New UniqueIdTrimer(MetaCyc, MyBase.CompiledModel).Invoke()
            Call New Builder.MetabolismBuilder(MetaCyc, Model:=MyBase.CompiledModel).Invoke()
            Call New Builder.ExpressionFluxBuilder(MetaCyc, MyBase.CompiledModel).Invoke()
            Call New Builder.MappingBuilder(MetaCyc, MyBase.CompiledModel).Invoke()
            Call New Builder.RegulationNetworkBuilder(MetaCyc, MyBase.CompiledModel).Invoke()
            Call New Builder.ProteinAssemblies(MetaCyc, MyBase.CompiledModel).Invoke()
            '  Call New Builder.Proteins(MetaCyc, MyBase.CompiledModel).Invoke()
            Call New Builder.ReactionRegulators(MetaCyc, MyBase.CompiledModel).Invoke()
            Call New Builder.EnzymeActivityRegulator(MetaCyc, MyBase.CompiledModel).Invoke()
            '   Call New Builder.ExpendGeneralProtein(MetaCyc, MyBase.CompiledModel).Invoke()
            Call New Builder.Polypeptides(MetaCyc, MyBase.CompiledModel).Invoke()

            MyBase.CompiledModel.ModelProperty = GenerateModelProperty()

            Return 0
        End Function

        Public Overrides Function Compile(Optional ModelProperty As Microsoft.VisualBasic.CommandLine.CommandLine = Nothing) As BacterialModel
            Call Link()
            Call MyBase.WriteProperty(ModelProperty, MyBase.CompiledModel)
            Return Me.CompiledModel
        End Function

        Public Overrides ReadOnly Property Version As Version
            Get
                Return My.Application.Info.Version
            End Get
        End Property
    End Class
End Namespace
