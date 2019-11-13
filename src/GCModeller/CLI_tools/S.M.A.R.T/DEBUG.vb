#Region "Microsoft.VisualBasic::9b7e403e0408633ea81dd3466213a15a, CLI_tools\S.M.A.R.T\DEBUG.vb"

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

    ' Module DEBUG
    ' 
    '     Sub: main
    ' 
    ' /********************************************************************************/

#End Region

Module DEBUG

    Sub main()

        'Call CommandLines.Set("set blastbin ""e:\BLAST\bin""")
        'Call CommandLines.Set("set blastdb ""e:\blast\db""")
        'Call CommandLines.Set("set blastapp ""localblast""")



        'Dim blast = NCBI.Extensions.LocalBLAST.InteropService.CreateInstance(New NCBI.Extensions.LocalBLAST.InteropService.InitializeParameter(Settings.BlastBin, NCBI.Extensions.LocalBLAST.InteropService.Program.LocalBlast))

        'Dim cp = New SMRUCC.genomics.SimpleProteinModularArchitecture.CompileDomains(blast, New SMRUCC.genomics.Assembly.CDD.CDDInfo.CDDLoader("e:\BLAST\db\CDD"), "E:\TEMP")

        'Call cp.ExportDb("Pfam").Save("D:\pfam.csv")

        'Dim str = cp.Performance("E:\xan\Xanthomonas campestris pv. campestris str. 8004..fsa", "match XC_\d+[|].+", Global.Settings.DataCache)
        '   Dim cache = (Global.Program.Settings.DataCache & "\Xanthomonas campestris pv. campestris str. 8004..fsa.xml").LoadXml(Of SMARTDB)()

        '   Call cache.Export.Save("E:\xan\Xanthomonas campestris pv. campestris str. 8004.csv", False)


        'str = cp.Performance("E:\xan\Xanthomonas campestris pv. campestris str. ATCC 33913..fsa", "match XCC\d+[|].+", Global.Settings.DataCache)
        'Call (Global.Settings.DataCache & "\Xanthomonas campestris pv. campestris str. ATCC 33913..fsa.xml").LoadXml(Of SMARTDB)().Export.Save("E:\xan\Xanthomonas campestris pv. campestris str. ATCC 33913.csv")

        'str = cp.Performance("E:\xan\Xanthomonas campestris pv. campestris str. B100..fsa", "match xccb100_\d+[|].+", Global.Settings.DataCache)
        'Call (Global.Settings.DataCache & "\Xanthomonas campestris pv. campestris str. B100..fsa.xml").LoadXml(Of SMARTDB)().Export.Save("E:\xan\Xanthomonas campestris pv. campestris str. B100.csv")

        'str = cp.Performance("E:\xan\Xanthomonas campestris pv. raphani 756C..fsa", "match XCR_\d+[|].+", Global.Settings.DataCache)
        'Call (Global.Settings.DataCache & "\Xanthomonas campestris pv. raphani 756C..fsa.xml").LoadXml(Of SMARTDB)().Export.Save("E:\xan\Xanthomonas campestris pv. raphani 756C.csv")

        'str = cp.Performance("E:\xan\Xanthomonas campestris pv. vesicatoria str. 85-10..fsa", "match XCV\d+[|].+", Global.Settings.DataCache)
        'Call (Global.Settings.DataCache & "\Xanthomonas campestris pv. vesicatoria str. 85-10..fsa.xml").LoadXml(Of SMARTDB)().Export.Save("E:\xan\Xanthomonas campestris pv. vesicatoria str. 85-10.csv")

        'str = cp.Performance("E:\xan\Xanthomonas citri subsp. citri Aw12879..fsa", "match XCAW_\d+[|].+", Global.Settings.DataCache)
        'Call (Global.Settings.DataCache & "\Xanthomonas citri subsp. citri Aw12879..fsa.xml").LoadXml(Of SMARTDB)().Export.Save("E:\xan\Xanthomonas citri subsp. citri Aw12879.csv")

        'str = cp.Performance("E:\xan\Xanthomonas oryzae pv. oryzae KACC 10331..fsa", "match XOO\d+[|].+", Global.Settings.DataCache)
        'Call (Global.Settings.DataCache & "\Xanthomonas oryzae pv. oryzae KACC 10331..fsa.xml").LoadXml(Of SMARTDB)().Export.Save("E:\xan\Xanthomonas oryzae pv. oryzae KACC 10331.csv")

        'str = cp.Performance("E:\xan\Xanthomonas oryzae pv. oryzae MAFF 311018..fsa", "match XOO_\d+[|].+", Global.Settings.DataCache)
        'Call (Global.Settings.DataCache & "\Xanthomonas oryzae pv. oryzae MAFF 311018..fsa.xml").LoadXml(Of SMARTDB)().Export.Save("E:\xan\Xanthomonas oryzae pv. oryzae MAFF 311018.csv")

        'str = cp.Performance("E:\xan\Xanthomonas oryzae pv. oryzae PXO99A..fsa", "match PXO_\d+[|].+", Global.Settings.DataCache)
        'Call (Global.Settings.DataCache & "\Xanthomonas oryzae pv. oryzae PXO99A..fsa.xml").LoadXml(Of SMARTDB)().Export.Save("E:\xan\Xanthomonas oryzae pv. oryzae PXO99A.csv")

        'str = cp.Performance("E:\xan\Xanthomonas oryzae pv. oryzicola BLS256..fsa", "match XOC_\d+[|].+", Global.Settings.DataCache)
        'Call (Global.Settings.DataCache & "\Xanthomonas oryzae pv. oryzicola BLS256..fsa.xml").LoadXml(Of SMARTDB)().Export.Save("E:\xan\Xanthomonas oryzae pv. oryzicola BLS256.csv")

        'Dim query = New DomainQuery(l)

        'Call query.Query("PF00512")
        'Call query.Query("PF07536")
        'Call query.Query("PF07568")
        'Call query.Query("PF07730")
        'Call query.Query("PF06580")
        'Call query.Query("PF01627")
        'Call query.Query("PF02518")
        'Call query.Query("PF00072")

        'Call SMRUCC.genomics.Assembly.ProteinInteractionNetwork.ExportNetwork(SMRUCC.genomics.Assembly.ProteinInteractionNetwork.BuildInteraction(l.Proteins,
        '     "D:\GCModeller\database\domine.xml".LoadXml(Of SMRUCC.genomics.Assembly.DOMINE.Database)), "d:\8004_prot_interactions.txt")

        '  Call New SMRUCC.genomics.Assembly.DOMINE.Database.Imports().InvokeAction("D:\BLAST\domine-tables-2.0\").Save("D:\GCModeller\database\domine.xml")

        'Dim signalDomains = IO.File.ReadAllLines("D:\BLAST\db\CDD\SignalDomains.txt")
        'Dim list = (From p In l.Proteins Where Not p.ContainsDomain(signalDomains).IsNullOrEmpty Select p).ToArray

        'Call New SMARTDB() With {.Proteins = list}.GetXml.Save("d:\8004_signal_proteins.xml")

        'Call SMRUCC.genomics.Assembly.ProteinInteractionNetwork.ExportNetwork(SMRUCC.genomics.Assembly.ProteinInteractionNetwork.BuildInteraction(list,
        '            "D:\GCModeller\database\domine.xml".LoadXml(Of SMRUCC.genomics.Assembly.DOMINE.Database)), "d:\signal_interactions.txt")

    End Sub
End Module
