imports "vcellkit.modeller" from "vcellkit";

let modelFile as string = "K:\20200226\metabolism\vcell\model.Xml";

modelFile
:> read.vcell
:> apply.kinetics(cache = `${dirname(modelFile)}/.cache`)
:> xml
:> writeLines(con = `${dirname(modelFile)}/${basename(modelFile)}.applyKinetics.XML`)
;