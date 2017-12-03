#Region "Microsoft.VisualBasic::0d663bbfa5f47abc66ce4e29bac23fbb, ..\GCModeller\core\Bio.Assembly\Test\chebiTest.vb"

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

Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ComponentModel

Module chebiTest

    Sub Main()

        Dim tables As New TSVTables("D:\smartnucl_integrative\DATA\ChEBI\tsv")

        Dim alltypes = tables.GetChemicalData.Select(Function(c) c.TYPE).Distinct.JoinBy(";  ")

        Dim prope = tables.GetChemicalData().CreateProperty
        Dim handle = New HandledList(Of ChemicalProperty)(prope)


        Dim a = handle(16947)
        Dim b = handle(30769)

        Dim model = EntityList.LoadDirectory("D:\smartnucl_integrative\DATA\ChEBI\chebi_cache\")
        Dim list = model.AsList
        Dim table = model.ToSearchModel

        Call model.GetXml.SaveTo("./chebi-100.xml")
    End Sub
End Module

