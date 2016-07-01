#Region "Microsoft.VisualBasic::dd935d06e690f69397fe318099531cd9, ..\GCModeller\engine\GCMarkupLanguage\FBA\API.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.Assembly.SBML
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace FBACompatibility

    <PackageNamespace("GCML.FBA.Compiler")>
    Public Module API

        <ExportAPI("Compile")>
        Public Function Compile(SBMl2 As Level2.XmlFile) As Model
            Dim Model As Model = New Model
            Model.Reactions = (From flux In SBMl2.MetabolismNetwork
                               Select New MetabolismFlux With {
                                   .Identifier = flux.Identifier,
                                   .LOWER_BOUND = flux.LOWER_BOUND,
                                   .UPPER_BOUND = flux.UPPER_BOUND}).ToArray
            Model.MAT = (From metabolite As FLuxBalanceModel.IMetabolite
                         In SBMl2.Metabolites
                         Select metabolite.Generate(SBMl2.MetabolismNetwork)).ToArray

            Return Model
        End Function

        Public Function Compile(Of TR As ICompoundSpecies)(IFBAC2 As FLuxBalanceModel.I_FBAC2(Of TR)) As Model
            Dim Model As Model = New Model
            Model.MAT = (From Metabolites In IFBAC2.Metabolites Select Metabolites.Generate(IFBAC2.MetabolismNetwork)).ToArray '
            Model.Reactions = (From reaction In IFBAC2.MetabolismNetwork Select MetabolismFlux.Convert(Flux:=reaction)).ToArray  '

            Return Model
        End Function

        <ExportAPI("Compile")>
        Public Function Compile(SBML3 As Level3.XmlFile) As Model
            Throw New NotImplementedException
        End Function
    End Module
End Namespace
