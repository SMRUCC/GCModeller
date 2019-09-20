#Region "Microsoft.VisualBasic::9975eec06e1c592cffaced925f5bdcda, engine\vcsm\_DEBUG_MAIN.vb"

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

    ' Module _DEBUG_MAIN
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

'Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Module _DEBUG_MAIN

    Public Sub Main()

        Call Settings.Initialize()

        '    Call CommandLines.Run("run -gene_mutations XC_1308|0 -i ""E:\Desktop\xan_vcell\Xcc8004_CellModel\trunk\Xcc8004.xml"" -f csv_tabular -t 600 -url http://localhost:1002/client?user=lab613%password=123456%database=dm1308 -interval 1 -cultivation_mediums ""E:\Desktop\xan_vcell\Xcc8004_CellModel\trunk\CultivationMediums\FullSystem.csv"" -trim_metabolism T -suppress_warn T -suppress_error T -suppress_periodic_message T")

        Call CommandLines.Run("run -i ""E:\GCModeller\CompiledAssembly\test\xcb_model\xcb.xml"" -f csv_tabular -t 5 -url x:\export\ -interval 2 -cultivation_mediums ""E:\GCModeller\CompiledAssembly\test\xcb_model\DataModels\CultivationMediums.csv"" -trim_metabolism T")


        Call CommandLines.Run("run -i ""E:\Desktop\xcb_vcell\Xcc8004_CellModel\branches\Test\Xcc8004.xml"" -f csv_tabular -t 2 -url http://localhost:1002/client?user=lab613%password=123456%database=gcmodeller_export -interval 2 -cultivation_mediums ""E:\Desktop\xcb_vcell\Xcc8004_CellModel\trunk\CultivationMediums\NY.csv"" -trim_metabolism T")

        'CommandLines.RegistryModule("registry ./models/plas.dll")
        'CommandLines.RegistryModule("registry ./models/svd.dll")

        'Call New Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant() {
        '    New Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant With {.Enzymes = New String() {"XC_0602", "XC_0643", "XC_0676", "XC_0905", "XC_0997", "XC_1007", "XC_1008", "XC_3192", "XC_3585", "XC_3992"}, .UPPER_BOUND = 20, .GeneralType = Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant.GENERAL_TYPE_ID_POLYPEPTIDE},
        '    New Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant With {.Enzymes = New String() {"XC_2718"}, .UPPER_BOUND = 20, .GeneralType = Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant.GENERAL_TYPE_ID_TRANSCRIPTS}}.SaveTo("e:\desktop\ddd.csv", False)



        '        Call New WholeGenomeMutation("run -i ""E:\Desktop\xan_vcell\CellModel\Xcc8004.xml"" -f csv_tabular -t 2 -trim_metabolism T -url http://localhost:1002/client?user=lab613%password=123456%database=gcmodeller_export -interval 1 -cultivation_mediums ""E:\Desktop\xan_vcell\CellModel\CultivationMediums\MMX.csv"" -dumpdir e:\desktop -factor 0").Invoke()


    End Sub
End Module
