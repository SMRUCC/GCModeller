Imports System.Text
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.base
Imports RDotNET.Extensions.VisualBasic.flashClust
Imports RDotNET.Extensions.VisualBasic.Graphics
Imports RDotNET.Extensions.VisualBasic.grDevices
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.stats
Imports RDotNET.Extensions.VisualBasic.utils.read.table
Imports RDotNET.Extensions.VisualBasic.dynamicTreeCut

Namespace WGCNA.App

    Public Class WGCNA_samples : Inherits WGCNA

        Public Property readData As readTableAPI
        Public Property LocusMap As String
        Public Property goodSamplesGenes As goodSamplesGenes
        Public Property save As save
        Public Property imageOut As grDevice
        Public Property plot As plot

        Public Property saveGoodGenes As writeTableAPI
        Public Property saveGoodSamples As writeTableAPI
        ''' <summary>
        ''' 建议使用average方法
        ''' </summary>
        ''' <returns></returns>
        Public Property hclust As flashClust
        Public Property dist As dist

        Const myData As String = "myData"
        Const datExpr As String = "datExpr"
        Const gsg As String = "gsg"
        Const sampleTree As String = "sampleTree"

        Sub New()
            Call MyBase.New
            Requires = MyBase.__requires + "flashClust"
        End Sub

        Protected Overrides Function __R_script() As String
            Dim sbr As ScriptBuilder =
                New ScriptBuilder(4096) + New options With {
                    .stringsAsFactors = False
            }
            Dim paste As New paste With {
                .collapse = ", "
            }

            sbr += myData <= readData
            sbr += [dim](myData)
            sbr += names(myData)
            sbr += datExpr <= [as].data.frame(t($"{myData}[, -c(1)]"))
            sbr += names(datExpr) <= $"{myData}${LocusMap}"
            sbr += rownames(datExpr) <= names(myData)(-c(1))
            sbr += gsg <= goodSamplesGenes(datExpr, verbose:=3)
            sbr += [if]("!gsg$allOK", Function() As String
                                          Dim sb As New ScriptBuilder

                                          sb += [if]("Sum(!gsg$goodGenes) > 0",
                                                     Function() printFlush(paste.Func("Removing genes:", paste.Func(names(datExpr)("!gsg$goodGenes")))))
                                          sb += [if]("Sum(!gsg$goodSamples) > 0",
                                                     Function() printFlush(paste.Func("Removing samples:", paste.Func(rownames(datExpr)("!gsg$goodSamples")))))
                                          sb += "datExpr = datExpr[gsg$goodSamples, gsg$goodGenes]"

                                          Return sb.ToString
                                      End Function)

            sbr += saveGoodGenes(names(datExpr)("!gsg$goodGenes"))
            sbr += saveGoodSamples(names(datExpr)("!gsg$goodSamples"))
            sbr += sampleTree <= hclust(dist(datExpr)) ' 根据样本表达量使用平均距离法建树  
            sbr += imageOut.Plot(Function() As String
                                     Dim sb As New ScriptBuilder
                                     sb += "par(cex = 0.6)"
                                     sb += "par(mar = c(0, 4, 2, 0))"
                                     sb += plot(sampleTree)

                                     Return sb.ToString
                                 End Function)

            sbr += save

            Return sbr.ToString
        End Function
    End Class
End Namespace