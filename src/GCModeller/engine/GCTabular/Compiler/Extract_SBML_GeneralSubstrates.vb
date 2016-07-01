Imports LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem
Imports LANS.SystemsBiology.Assembly.SBML

Namespace Compiler.Components

    Module Extract_SBML_GeneralSubstrates

        ''' <summary>
        ''' 将通用的代谢物进行展开
        ''' </summary>
        ''' <param name="Metabolites"></param>
        ''' <param name="MetaCyc">目标细菌的MetaCyc数据库</param>
        ''' <remarks></remarks>
        Public Sub Analysis(ByRef Metabolites As Dictionary(Of FileStream.Metabolite),
                            MetaCyc As DatabaseLoadder,
                            Model As FileStream.IO.XmlresxLoader,
                            Logging As Logging.LogFile)

            Dim SBML As Level2.XmlFile = Level2.XmlFile.Load(MetaCyc.SBMLMetabolismModel)

            Call Logging.WriteLine("Start to merge the sbml metabolite with datamodels...", "Extract_SBML_GeneralSubstrates->Analysis()")

            For Each Metabolite In SBML.Model.listOfSpecies       '首先按照MetaCycId查找，查找不到的时候在添加
                Dim Id As String = Metabolite.GetTrimmedId
                Dim LQuery = (From item In Metabolites.AsParallel
                              Where String.Equals(item.Value.MetaCycId, Id, StringComparison.OrdinalIgnoreCase)
                              Select item).ToArray

                If LQuery.IsNullOrEmpty Then '添加新的
                    Dim MetaboliteModel = FileStream.Metabolite.CreateObject(Metabolite)
                    Call Metabolites.Add(MetaboliteModel.Identifier, MetaboliteModel)
                Else
                    '查找到了
                End If
            Next
        End Sub
    End Module
End Namespace