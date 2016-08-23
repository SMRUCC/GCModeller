#Region "Microsoft.VisualBasic::52ebc09825efe0d055c08bff46adfd07, ..\R.Bioconductor\Bioconductor\Bioconductor\Test.vb"

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

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.DataStructures.BinaryTree
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports RDotNet.Extensions.Bioinformatics
Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.gplots
Imports RDotNet.Extensions.VisualBasic.grDevices
Imports RDotNet.Extensions.VisualBasic.utils.read.table
Imports SMRUCC.R.CRAN.Bioconductor.Web
Imports SMRUCC.R.CRAN.Bioconductor.Web.Packages
Imports Element = System.Xml.XmlElement
Imports MatchResult = System.Text.RegularExpressions.MatchCollection
Imports Node = System.Xml.XmlNode
Imports NodeList = System.Xml.XmlNodeList
Imports PhyloNode = Microsoft.VisualBasic.ComponentModel.DataStructures.BinaryTree.TreeNode(Of Integer)
Imports RegExp = System.Text.RegularExpressions.Regex


Module Test

    Dim heatmap As String = <R>

                                library(RColorBrewer)

df &lt;- read.csv(file="F:\\1.13.RegPrecise_network\\FBA\\xcam314565\\19.0\\data\\metabolic-reactions.rFBA\\heatmap\\objfunc-30__scales.csv", 
header=TRUE, 
sep=",", 
quote="\"", 
dec=".", 
fill=TRUE, 
comment.char="")
row.names(df) &lt;- df$Locus
df &lt;- df[,-1]
df &lt;- data.matrix(df)

library(gplots)

tiff(compression=c("none", "rle", "lzw", "jpeg", "zip", "lzw+p", "zip+p"), 
filename="F:/1.13.RegPrecise_network/FBA/xcam314565/19.0/data/metabolic-reactions.rFBA/heatmap/objfunc-30__scales.tiff", 
width=4000, 
height=3200, 
units="px", 
pointsize=12, 
bg="white", 
res=NA, 
family="", 
restoreConsole=TRUE, 
type=c("windows", "cairo"))

result &lt;- heatmap.2(df, col=redgreen(75), scale="row",   margin=c(15,15)  ,    key=TRUE, symkey=FALSE, density.info="none", trace="none", cexRow=2,cexCol=2)
dev.off()




                            </R>

    Sub Main()

        Call RDotNet.Extensions.VisualBasic.TryInit()

        Dim afsfdddddd = sparcc.API.sparcc("G:\GCModeller\GCModeller\R\r-sparcc\example\fake_data.txt", 1)


        Dim vv As New VennDiagram.vennDiagramPlot







        Dim hm As New Heatmap With {
            .dataset = New readcsv("E:\R.Bioinformatics\datasets\ppg2008.csv"),
            .heatmap = heatmap2.Puriney,
            .image = New tiff("x:/ffff.tiff", 8000, 6500)
        }

        Dim r As String = hm.RScript

        Call r.SaveTo("x:\dddd.r")
        '     Call RSystem.REngine.WriteLine(r)

        Dim resultooo = heatmap2OUT.RParser(hm.output)
        resultooo.locus = hm.locusId
        resultooo.samples = hm.samples


        Call resultooo.GetJson.SaveTo("x:\heatmap.2.sample.json")



        Dim xx As Pointer = 5

        Dim ndd As Integer = +xx

        Dim xxsss = +ndd


        Dim ddd As String() = LoadJsonFile(Of String())("E:\R.Bioinformatics\datasets\heatmap_testOUT.json")


        Dim hhhhh = heatmap2OUT.RParser(ddd)

        Call hhhhh.GetJson.SaveTo("x:\ddd.json")

        '    Call RSystem.REngine.WriteLine(Test.heatmap)

        '    Dim result = RSystem.REngine.WriteLine("result")

        '   Call result.GetJson.SaveTo("E:\R.Bioinformatics\datasets\heatmap_testOUT.json")








        Dim rp = Web.Repository.LoadDefault
        Dim pp = rp.softwares.First
        pp.GetDetails("E:\R.Bioinformatics\Bioconductor\ParserTest.html".ReadAllText)
    End Sub

End Module

