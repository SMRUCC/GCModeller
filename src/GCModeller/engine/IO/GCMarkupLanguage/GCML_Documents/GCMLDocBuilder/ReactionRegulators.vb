#Region "Microsoft.VisualBasic::e88b6bc868139b8419fd324f5f7b58f3, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\ReactionRegulators.vb"

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

    '   Total Lines: 56
    '    Code Lines: 40
    ' Comment Lines: 6
    '   Blank Lines: 10
    '     File Size: 3.05 KB


    '     Class ReactionRegulators
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateRegulator, Invoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Microsoft.VisualBasic.Extensions

Namespace Builder

    Public Class ReactionRegulators : Inherits IBuilder

        Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel)
            Call MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Dim Regulators = (From regr As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Regulation
                              In MetaCyc.GetRegulations
                              Where regr.Types.IndexOf("Regulation-of-Reactions") > -1
                              Select regr).ToArray         '获取所有的ReactionRegulator
            Dim Metabolites = Model.Metabolism.Metabolites.ToArray

            For Each Reaction In Model.Metabolism.MetabolismNetwork
                Dim LQuery = (From regr In Regulators Where String.Equals(Reaction.Identifier, regr.RegulatedEntity) Select regr).ToArray '选择调控本反映的调控因子
                If Not LQuery.IsNullOrEmpty Then '可能有多个调控因子
                    'Reaction.Regulators = (From regr As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Regulation
                    '                       In LQuery
                    '                       Let Regulator = CreateRegulator(regr, Metabolites)
                    '                       Where Not Regulator Is Nothing
                    '                       Select Regulator).AsList
                End If
            Next

            Return Model
        End Function

        Private Shared Function CreateRegulator(RegulatorBaseType As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Regulation,
                                                Metabolites As GCML_Documents.XmlElements.Metabolism.Metabolite()) _
            As GCML_Documents.XmlElements.SignalTransductions.Regulator

            Dim Regulator As GCML_Documents.XmlElements.SignalTransductions.Regulator =
                New GCML_Documents.XmlElements.SignalTransductions.Regulator With
                {
                    .Identifier = RegulatorBaseType.Identifier}
            Regulator.Activation = String.Equals(RegulatorBaseType.Mode, "+")
            Regulator.PhysiologicallyRelevant = String.Equals(RegulatorBaseType.PhysiologicallyRelevant, "T")
            Regulator.Types = RegulatorBaseType.Types.ToArray

            Dim Regulators = (From met In Metabolites Where String.Equals(met.Identifier, RegulatorBaseType.Regulator) Select met).ToArray
            If Regulators.IsNullOrEmpty Then
                Call Printf("[EXCEPTION]Unknown metabolite compound: #UNIQUE-ID=::%s", RegulatorBaseType.Regulator)
                Return Nothing
            Else
                '   Regulator.pHwnd = Regulators.First.Hwnd
            End If

            Return Regulator
        End Function
    End Class
End Namespace
