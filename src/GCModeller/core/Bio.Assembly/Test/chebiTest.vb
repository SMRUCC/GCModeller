#Region "Microsoft.VisualBasic::8119667dff13207a896803c223893051, ..\GCModeller\core\Bio.Assembly\test\chebiTest.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML

Module chebiTest

    Sub Main()

        Dim tables As New TSVTables("D:\smartnucl_integrative\DATA\ChEBI\tsv")

        Dim alltypes = tables.GetChemicalData.Select(Function(c) c.TYPE).Distinct.JoinBy(";  ")

        Dim prope = tables.GetChemicalData().CreateProperty
        Dim handle = New HashList(Of ChemicalProperty)(prope)


        Dim a = handle(16947)
        Dim b = handle(30769)

        Dim model = EntityList.LoadDirectory("D:\smartnucl_integrative\DATA\ChEBI\chebi_cache\")
        Dim list = model.AsList
        Dim table = model.ToSearchModel

        Call model.GetXml.SaveTo("./chebi-100.xml")
    End Sub
End Module
