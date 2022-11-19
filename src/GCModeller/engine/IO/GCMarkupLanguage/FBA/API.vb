#Region "Microsoft.VisualBasic::66e9289e922d3690ab940790a6356b00, GCModeller\engine\IO\GCMarkupLanguage\FBA\API.vb"

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

    '   Total Lines: 39
    '    Code Lines: 32
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.66 KB


    '     Module API
    ' 
    '         Function: (+3 Overloads) Compile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Model.SBML

Namespace FBACompatibility

    <Package("GCML.FBA.Compiler")>
    Public Module API

        <ExportAPI("Compile")>
        Public Function Compile(SBMl2 As Level2.XmlFile) As Model
            Dim Model As Model = New Model
            Model.Reactions = (From flux In SBMl2.MetabolismNetwork
                               Select New MetabolismFlux With {
                                   .Identifier = flux.Key,
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
